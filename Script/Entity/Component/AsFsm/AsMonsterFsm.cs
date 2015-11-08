using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsMonsterFsm : AsBaseComponent
{
	public enum eMonsterFsmStateType
	{
		NONE,
		APPEAR, IDLE, RUN, DEATH,
		CONDITION_STUN, CONDITION_FREEZE, CONDITION_SLEEP, CONDITION_FORCEDMOVE, CONDITION_FEAR, CONDITION_BINDING, CONDITION_AIRBONE, CONDITION_TRANSFORM,
		SKILL_READY, SKILL_HIT, SKILL_FINISH
	}

	#region - member -
	public eMonsterFsmStateType state_;
	AsNpcEntity m_MonsterEntity;public AsNpcEntity MonsterEntity	{ get { return m_MonsterEntity; } }

	protected Dictionary<eMonsterFsmStateType, AsBaseFsmState<eMonsterFsmStateType, AsMonsterFsm>> m_dicFsmState =
		new Dictionary<eMonsterFsmStateType, AsBaseFsmState<eMonsterFsmStateType, AsMonsterFsm>>();
	protected AsBaseFsmState<eMonsterFsmStateType, AsMonsterFsm> m_CurrentFsmState;
	public eMonsterFsmStateType CurrnetFsmStateType	{ get { return m_CurrentFsmState.FsmStateType; } }
	protected AsBaseFsmState<eMonsterFsmStateType, AsMonsterFsm> m_OldFsmState;
	public eMonsterFsmStateType OldFsmStateType	{ get { return m_OldFsmState.FsmStateType; } }
	private List<CNpcBuffTempData> m_BuffTempDataList = new List<CNpcBuffTempData>();
	private bool m_isNeedBuffReset = false;
	public List<CNpcBuffTempData> getBuffTempList
	{
		get	{ return m_BuffTempDataList; }
	}

	public void SetUseNeedBuffReset()
	{
		m_isNeedBuffReset = false;
	}

	public bool isNeedBuffReset
	{
		get	{ return m_isNeedBuffReset; }
	}

	AsUserEntity m_Target;
	public AsUserEntity Target	{ get { return m_Target; }	set { m_Target = value; } }
	public float targetDistance	{ get { return Vector3.Distance( m_Target.transform.position, transform.position); } }

	Msg_NpcAttackChar1 m_AttackSC;

	//fx
	ElementProcessor m_ElementProcessor;
	private float fadeTime = 0.0f;

	public bool showLog_ = false;

	//fade
	Renderer[] m_Renderers = null;
	public Renderer[] Renderers	{ get { return m_Renderers; } }

	Vector3 m_InitSize = Vector3.one;
	float m_Character_Size = 1f;
	public float Charracter_Size	{ get { return m_Character_Size; } }

	Msg_NpcMoveIndicate m_LastMoveIndication = null;
	public Msg_NpcMoveIndicate LastMoveIndication	{ get { return m_LastMoveIndication; } }
	#endregion

	#region - init & release -
	void Awake()
	{
		m_ComponentType = eComponentType.FSM_MONSTER;

		m_MonsterEntity = GetComponent<AsNpcEntity>();
		if( m_MonsterEntity == null)
			Debug.LogError( "AsMonsterFsm::Init: no user entity attached");
		m_MonsterEntity.FsmType = eFsmType.MONSTER;		
		
		#region - msg -
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_NPC_INDICATION, OnMoveIndication);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_END_INFORM, OnMoveEnd);
		MsgRegistry.RegisterFunction( eMessageType.NPC_ATTACK_CHAR1, OnAttackIndication);
		MsgRegistry.RegisterFunction( eMessageType.HIT_EXECUTION, OnHitExecution);
//		MsgRegistry.RegisterFunction( eMessageType.INPUT, OnClickedByPlayer);

		MsgRegistry.RegisterFunction( eMessageType.CHAR_ATTACK_NPC2, OnDamaged);
		MsgRegistry.RegisterFunction( eMessageType.NPC_ATTACK_CHAR3, OnAffected);
		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_END, OnAnimationEnd);
		MsgRegistry.RegisterFunction( eMessageType.MOB_COMBAT_FREE, OnCombatFree);
		MsgRegistry.RegisterFunction( eMessageType.NPC_STATUS, OnNpcStatus);

		//SKILL EFFECT
		MsgRegistry.RegisterFunction( eMessageType.NPC_SKILL_EFFECT, OnSkillEffect);

		//BUFF
		MsgRegistry.RegisterFunction( eMessageType.NPC_BUFF, OnBuff);
		MsgRegistry.RegisterFunction( eMessageType.NPC_DEBUFF, OnDeBuff);
		MsgRegistry.RegisterFunction( eMessageType.BUFF_INCLUSION, OnBuffInclusion);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_DEBUFF_RESIST, OnDebuffResist);

		//ATTRIBUTE
		MsgRegistry.RegisterFunction( eMessageType.ATTACK_SPEED_REFRESH, OnAttackSpeedRefresh);

		//CONDITION
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_STUN, OnConditionStun);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_STUN, OnRecoverConditionStun);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_FREEZE, OnConditionFreeze);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_FREEZE, OnRecoverConditionFreeze);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_SLEEP, OnConditionSleep);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_SLEEP, OnRecoverConditionSleep);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_SIZECONTROL, OnConditionSizeControl);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_SIZECONTROL, OnRecoverConditionSizeControl);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_FEAR, OnConditionFear);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_FEAR, OnRecoverConditionFear);
		
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_AIRBONE, OnConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_AIRBONE, OnRecoverConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_TRANSFORM, OnConditionTransform);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverConditionTransform);

		//exception
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_STATE, OnRecoverState);

		//death by other reason( ex. buff)
		MsgRegistry.RegisterFunction( eMessageType.DEATH_INDICATION, OnDeath);

		MsgRegistry.RegisterFunction( eMessageType.SHAKE_INDICATION, OnShake);
		
		MsgRegistry.RegisterFunction( eMessageType.BALLOON, OnBalloonIndicate);
		#endregion
		#region - state -
		m_dicFsmState.Add( eMonsterFsmStateType.APPEAR, new AsMonsterState_Appear( this));
		m_dicFsmState.Add( eMonsterFsmStateType.IDLE, new AsMonsterState_Idle( this));
		m_dicFsmState.Add( eMonsterFsmStateType.RUN, new AsMonsterState_Run( this));
		m_dicFsmState.Add( eMonsterFsmStateType.DEATH, new AsMonsterState_Death( this));

		m_dicFsmState.Add( eMonsterFsmStateType.SKILL_READY, new AsMonsterState_SkillReady( this));
		m_dicFsmState.Add( eMonsterFsmStateType.SKILL_HIT, new AsMonsterState_SkillHit( this));
		m_dicFsmState.Add( eMonsterFsmStateType.SKILL_FINISH, new AsMonsterState_SkillFinish( this));

		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_STUN, new AsMonsterState_Condition_Stun( this));
		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_FREEZE, new AsMonsterState_Condition_Freeze( this));
		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_SLEEP, new AsMonsterState_Condition_Sleep( this));
		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_FEAR, new AsMonsterState_Condition_Fear( this));
		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_AIRBONE, new AsMonsterState_Condition_AirBone( this));
		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_TRANSFORM, new AsMonsterState_Condition_Transform( this));

		m_dicFsmState.Add( eMonsterFsmStateType.CONDITION_FORCEDMOVE, new AsMonsterState_Condition_ForcedMove( this));
		#endregion
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);
		
		m_InitSize = m_Entity.transform.localScale;

		m_ElementProcessor = new ElementProcessor( m_Entity);
		m_Entity.SetConditionCheckDelegate( m_ElementProcessor.GetEnableCondition);
		m_Entity.SetBuffCheckDelegate( m_ElementProcessor.CheckBuffInclusion);
		m_Entity.SetCharacterSize( GetCharacterSize);

		gameObject.layer = LayerMask.NameToLayer( "Monster");
	}

	public override void InterInit( AsBaseEntity _entity)
	{
		m_dicFsmState[eMonsterFsmStateType.APPEAR].Init();
		m_dicFsmState[eMonsterFsmStateType.IDLE].Init();
		m_dicFsmState[eMonsterFsmStateType.RUN].Init();
		m_dicFsmState[eMonsterFsmStateType.DEATH].Init();

		m_dicFsmState[eMonsterFsmStateType.SKILL_READY].Init();
		m_dicFsmState[eMonsterFsmStateType.SKILL_HIT].Init();
		m_dicFsmState[eMonsterFsmStateType.SKILL_FINISH].Init();

		m_dicFsmState[eMonsterFsmStateType.CONDITION_STUN].Init();
		m_dicFsmState[eMonsterFsmStateType.CONDITION_FREEZE].Init();
		m_dicFsmState[eMonsterFsmStateType.CONDITION_SLEEP].Init();
		m_dicFsmState[eMonsterFsmStateType.CONDITION_AIRBONE].Init();
		m_dicFsmState[eMonsterFsmStateType.CONDITION_TRANSFORM].Init();

		m_dicFsmState[eMonsterFsmStateType.CONDITION_FORCEDMOVE].Init();
	}

	public override void LateInit( AsBaseEntity _entity)
	{
		SetMonsterFsmState( eMonsterFsmStateType.IDLE);
	}

	public override void LastInit( AsBaseEntity _entity)
	{
		m_InitSize = m_Entity.transform.localScale;
	}

	void Start()
	{
		m_InitSize = m_MonsterEntity.transform.localScale;

		gameObject.name += "<" + m_Entity.GetProperty<string>( eComponentProperty.CLASS) + ">";

		float hp = m_MonsterEntity.GetProperty<float>( eComponentProperty.HP_CUR);
		bool living = m_MonsterEntity.GetProperty<bool>( eComponentProperty.LIVING);
		if( ( 1 > hp) || ( false == living))
		{
			SetMonsterFsmState( eMonsterFsmStateType.DEATH, new Msg_EnterWorld_Death( true));
			AsEntityManager.Instance.RemoveEntity( m_MonsterEntity);
		}
	}

	void OnDisable()
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.Exit();
	}
	#endregion

	#region - fsm -
	public void SetMonsterFsmState( eMonsterFsmStateType _type, AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == _type)
			{
				Debug.LogWarning( "AsMonsterFsm::SetMonsterFsmState: same state = " + _type);
				return;
			}
			
			m_CurrentFsmState.Exit();
			ReleaseElements();
			m_OldFsmState = m_CurrentFsmState;
		}

		if( m_dicFsmState.ContainsKey( _type) == true)
		{
			state_ = _type;
			m_CurrentFsmState = m_dicFsmState[_type];
			m_CurrentFsmState.Enter( _msg);
		}
	}

	public void SetMonsterFsmState( eMonsterFsmStateType _type)
	{
		SetMonsterFsmState( _type, null);
	}
	#endregion

	#region - update -
	void Update()
	{
		m_CurrentFsmState.Update();
		m_ElementProcessor.Update();

		for (int i=0; i<m_BuffTempDataList.Count; i++) 
		{
			CNpcBuffTempData _data = m_BuffTempDataList[i];
			_data.Update();
		}
	}

	void FixedUpdate()
	{
	}
	#endregion

	#region - fx -
	public void SetAction( Tbl_Action_Record _record)
	{
		m_ElementProcessor.SetAction( _record);
	}

	public void PlayElements( eActionStep _step, float _animSpeed)
	{
		m_ElementProcessor.PlayElement( _step, Vector3.zero, null, _animSpeed);
	}

	public void PlayElements( eActionStep _step, Vector3 _pos, Transform _targetTrn)
	{
		m_ElementProcessor.PlayElement( _step, _pos, _targetTrn);
	}

	public void PlayElements( eActionStep _step, Vector3 _pos, Transform _targetTrn, float _animSpeed)
	{
		m_ElementProcessor.PlayElement( _step, _pos, _targetTrn, _animSpeed);
	}

	public void ReleaseElements()
	{
		m_ElementProcessor.Release( false);
	}
	#endregion

	#region - skill effect from server -
	void OnSkillEffect( AsIMessage _msg)
	{
		Msg_NpcSkillEffect skill = _msg as Msg_NpcSkillEffect;

		m_ElementProcessor.PotencyProcess( skill);

		Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record( skill.skillTableIdx_);
		if( skillRecord.SkillName_Print != eSkillNamePrint.None)
		{
			string str = AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index);
			m_Entity.SkillNameShout( eSkillNameShoutType.Self, str);
		}
	}
	#endregion

	#region - buff -
	void OnBuff( AsIMessage _msg)
	{
		Msg_NpcBuff buff = _msg as Msg_NpcBuff;

		foreach( Msg_NpcBuff_Body body in buff.body_)
		{
			m_ElementProcessor.SetBuff( body, buff.effect_);

			if( 0 < body.duration_ && false == IsSameBuff( body.skillTableIdx_, body.potencyIdx_))
			{
				m_BuffTempDataList.Add( new CNpcBuffTempData( body.serverData));
				m_isNeedBuffReset = true;
			}
		}
	}

	bool IsSameBuff( int skillidx, int potencyIdx)
	{
		foreach( CNpcBuffTempData _data in m_BuffTempDataList)
		{
			if( null == _data.getNpcBuff)
				continue;

			if( _data.getNpcBuff.nSkillTableIdx == skillidx && _data.getNpcBuff.nPotencyIdx == potencyIdx)
				return true;
		}

		return false;
	}

	void OnDeBuff( AsIMessage _msg)
	{
		Msg_NpcDeBuff deBuff = _msg as Msg_NpcDeBuff;

		m_ElementProcessor.RemoveBuff( deBuff);

		foreach( CNpcBuffTempData _data in m_BuffTempDataList)
		{
			if( _data.getNpcBuff.eType == deBuff.serverData.eType)
			{
				m_BuffTempDataList.Remove( _data);
				m_isNeedBuffReset = true;
				break;
			}
		}
	}

	void OnBuffInclusion( AsIMessage _msg)
	{
		Msg_BuffInclusion inclusion = _msg as Msg_BuffInclusion;

		if( m_ElementProcessor.CheckBuffInclusion( inclusion.skillIdx_) == true)
		{
			inclusion.sender_.HandleMessage( new Msg_TargetIndication( m_Entity, inclusion.MessageType));
		}
	}
	
	void OnDebuffResist( AsIMessage _msg)
	{
		AsHUDController.Instance.panelManager.ShowDebuffResist( Entity.gameObject);
	}
	#endregion

	#region - attribute -
	void OnAttackSpeedRefresh( AsIMessage _msg)
	{
		Msg_AttackSpeedRefresh refresh = _msg as Msg_AttackSpeedRefresh;

		m_Entity.SetProperty( eComponentProperty.ATTACK_SPEED, refresh.attackSpeed_);
	}
	#endregion

	#region - condition -
	void OnConditionStun( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_STUN);
	}

	void OnRecoverConditionStun( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionFreeze( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_FREEZE);
	}

	void OnRecoverConditionFreeze( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionSleep( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_SLEEP);
	}

	void OnRecoverConditionSleep( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionSizeControl( AsIMessage _msg)
	{
		m_Character_Size = 1;
		transform.localScale = m_InitSize;
		
		Msg_ConditionIndicate_SizeControl size = _msg as Msg_ConditionIndicate_SizeControl;
//		m_Character_Size += size.potency_.Potency_Value * 0.001f;
		m_Character_Size = size.size_;
		transform.localScale *= m_Character_Size;
	}

	void OnRecoverConditionSizeControl( AsIMessage _msg)
	{
		m_Character_Size = 1;
		transform.localScale = m_InitSize;
	}

	void OnConditionFear( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_FEAR);
	}

	void OnRecoverConditionFear( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnConditionAirBone( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_AIRBONE);
	}

	void OnRecoverConditionAirBone( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnConditionTransform( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.CONDITION_TRANSFORM, _msg);
	}

	void OnRecoverConditionTransform( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - hit execution -
	void OnHitExecution( AsIMessage _msg)
	{
		if( null == m_AttackSC)
			return;

		ProjectileHit();

		if( m_AttackSC.attacker_ == null || m_AttackSC.skillLv_ == null || m_AttackSC.action_ == null)
		{
			Debug.LogWarning( "AsMonsterFsm::OnHitExecution: error occured while processing npc attack msg.");
			return;
		}

		foreach( Msg_NpcAttackChar2 attack2 in m_AttackSC.bodyChar_)
		{
			AsEntityManager.Instance.DispatchMessageByUniqueKey( attack2.charUniqKey_, attack2);

//			if( AsUserInfo.Instance.GetCurrentUserEntity().UniqueId == attack2.charUniqKey_)
//			{
//				Debug.Log( "AsMonsterFsm::OnHitExecution: id:" + m_MonsterEntity.SessionId + " deals damage to user entity ( hp:" + attack2.hpCur_);
//			}

			if( attack2.reflection_ > 0 && Entity != null)
			{
				if( null != AsHUDController.Instance)
				{
					AsHUDController.Instance.panelManager.ShowNumberPanel( Entity.gameObject, ( int)attack2.reflection_, attack2.eDamageType_, true);	// need reflection font
					m_Entity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);
					AsEntityManager.Instance.MessageToPlayer( new Msg_ReflectionRefresh( Entity));
				}
			}
		}

		foreach( Msg_NpcAttackChar3 attack3 in m_AttackSC.bodyNpc_)
		{
			AsEntityManager.Instance.DispatchMessageByNpcSessionId( attack3.npcIdx_, attack3);
		}

		for( int i = 0; i < m_AttackSC.skill_.listSkillPotency.Count; ++i)
		{
			if( ePotency_Type.Heal == m_AttackSC.skill_.listSkillPotency[i].Potency_Type
				&& ePotency_Target.Self == m_AttackSC.skill_.listSkillPotency[i].Potency_Target)
			{
				if( m_MonsterEntity.GetInstanceID() == AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID())
				{
					int nHealHP = ( int)( m_AttackSC.hpCur_ - AsHUDController.Instance.targetInfo.hp_cur);
					if( nHealHP > 0)
						AsHUDController.Instance.panelManager.ShowNumberPanel_HP( Entity.gameObject, nHealHP);

					AsHUDController.Instance.UpdateTargetSimpleInfo( m_MonsterEntity);
				}
				continue;
			}
		}

		if( 1 > m_AttackSC.hpCur_)
		{
			if( AsHUDController.Instance.targetInfo.getCurTargetEntity != null &&
				m_MonsterEntity.GetInstanceID() == AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID())
			{
				AsHUDController.Instance.SetTargetInfo( null);
			}

			float curHP = m_MonsterEntity.GetProperty<float>( eComponentProperty.HP_CUR);
			if( curHP > m_AttackSC.hpCur_)
				m_MonsterEntity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);

			#region -MonsterLines
			if( m_AttackSC.charUniqKey_[0] == AsUserInfo.Instance.SavedCharStat.uniqKey_)
				PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.Death));
			#endregion

			DeathProcess( m_AttackSC);
		}

		m_AttackSC = null;
	}

	void ProjectileHit()
	{
		Tbl_Action_Record record = m_AttackSC.action_;

		if( record.HitAnimation != null && record.HitAnimation.hitInfo.HitType == eHitType.ProjectileTarget)
		{
			foreach(uint node in m_AttackSC.charUniqKey_)
			{
				AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId( node);
				if( entity != null &&  entity.GetProperty<float>( eComponentProperty.HP_CUR) > 0) //dopamin
				{
					AsEffectManager.Instance.PlayEffect( record.HitAnimation.hitInfo.HitProjectileHitFileName,
						entity.transform, false, 0);
				}
			}
		}
	}
	#endregion

	#region - damaged -
	void OnDamaged( AsIMessage _msg)
	{
		Msg_OtherCharAttackNpc2 attack = _msg as Msg_OtherCharAttackNpc2;

		float curHP = m_MonsterEntity.GetProperty<float>( eComponentProperty.HP_CUR);
		if( curHP > attack.hpCur_)
			m_MonsterEntity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);
		
		#region -Authority
		if( true == attack.authorityOwnerIsMe)
		{
			AsHUDController.Instance.targetInfo.SetAuthority( attack.authorityInMe);
			if( null != m_MonsterEntity.namePanel)
				m_MonsterEntity.namePanel.SetAuthority( attack.authorityInMe);
		}
		#endregion

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

		#region -MonsterLines
		if( attack.parent_.charUniqKey_ == AsUserInfo.Instance.SavedCharStat.uniqKey_)
		{
			float maxHP = m_MonsterEntity.GetProperty<float>( eComponentProperty.HP_MAX);
			PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.HpProb, eConditionValueCheck.Range, ( int)attack.hpCur_, ( int)maxHP));
		}
		#endregion

		if( 1 > attack.hpCur_)
		{
			#region -MonsterLines
			if( attack.parent_.charUniqKey_ == AsUserInfo.Instance.SavedCharStat.uniqKey_)
				PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.Death));
			#endregion

			DeathProcess( attack);
		}
	}

//	void ForcedMoveProcess( Msg_OtherCharAttackNpc2 _msg)// obsolete now
//	{
//		Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record( _msg.skillLv_.Skill_GroupIndex);
//
//		for( int i=0; i<skill.listSkillPotency.Count; ++i)
//		{
//			if( skill.listSkillPotency[i].Potency_Type == ePotency_Type.Move)
//			{
//				Msg_ConditionIndicate_ForcedMove forcedMove = new Msg_ConditionIndicate_ForcedMove( 
//					_msg.position_,
//					_msg.skillLv_.listSkillLevelPotency[i].Potency_Value,
////					_msg.skillLv_.listSkillLevelPotency[i].Potency_Duration);
//					300);
//
//				SetMonsterFsmState( eMonsterFsmStateType.CONDITION_FORCEDMOVE, forcedMove);
//				break;
//			}
//
//			++i;
//		}
//	}
	#endregion

	#region - affected -
	void OnAffected( AsIMessage _msg)
	{
		if( m_Entity == null)
			return;

		Msg_NpcAttackChar3 attack = _msg as Msg_NpcAttackChar3;

		m_Entity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

//		#region - skill name shout -
//		if( attack.parent_.skill_.SkillName_Print == true)
//		{
//			string skillName = AsTableManager.Instance.GetTbl_String( attack.parent_.skill_.SkillName_Index);
//			m_Entity.SkillNameShout( eSkillNameShoutType.Benefit, skillName);
//		}
//		#endregion
	}
	#endregion

	#region - msg -
	void OnModelLoaded( AsIMessage _msg)
	{
		m_ElementProcessor.ModelLoaded();

		if( m_Entity.GetProperty<bool>( eComponentProperty.LIVING) == true)
		{
			m_Renderers = MonsterEntity.ModelObject.GetComponentsInChildren<Renderer>();
			foreach( Renderer ren in m_Renderers)
				ren.material.renderQueue = 3000;	// Transparent

			StartCoroutine( FadeIn());
		}
		else
		{
			if( ( null == MonsterEntity) || ( null == MonsterEntity.ModelObject) || ( null == MonsterEntity.ModelObject.gameObject))
			{
				Debug.LogError( "AsMonsterFsm::OnModelLoaded()[ null == monsterEntity ]");
				return;
			}

			MonsterEntity.ModelObject.gameObject.SetActiveRecursively( false);
		}
	}

	void OnMoveIndication( AsIMessage _msg)
	{
		m_LastMoveIndication = _msg as Msg_NpcMoveIndicate;

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnAttackIndication( AsIMessage _msg)
	{
		if( m_AttackSC != null && m_AttackSC.ready_ == false)
		{
			OnHitExecution( _msg);
		}

		m_AttackSC = _msg as Msg_NpcAttackChar1;
		
//		float animSpeed = GetCompleteAttackSpeed();
//		m_AttackSC.SetAnimSpeed( animSpeed);

		if( showLog_ == true)
			Debug.Log( "AsMonsterFsm::OnAttackIndication: m_AttackSc.ready_ == " + m_AttackSC.ready_ + "[" + Time.timeSinceLevelLoad + "]");

		if( m_AttackSC.ready_ == false)// || m_CurrentFsmState.FsmStateType == eMonsterFsmStateType.CONDITION_FORCEDMOVE)
		{
			OnHitExecution( m_AttackSC);
		}
		else
		{
			m_CurrentFsmState.MessageProcess( m_AttackSC);
		}

		#region -MonsterLines
		if( ( null != m_AttackSC) && ( m_AttackSC.charUniqKey_[0] == AsUserInfo.Instance.SavedCharStat.uniqKey_))
			PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.Skill, eConditionValueCheck.Equal, m_AttackSC.skill_.Index));
		#endregion
	}

	void OnAnimationEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCombatFree( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);

		float hpMax = m_MonsterEntity.GetProperty<float>( eComponentProperty.HP_MAX);
		m_MonsterEntity.SetProperty( eComponentProperty.HP_CUR, hpMax);

		m_Target = null;

		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null != playerFsm)
		{
			if( m_MonsterEntity == playerFsm.Target)
			{
				AsHUDController.Instance.SetTargetInfo( m_MonsterEntity);
			}
		}
	}

	int m_Statusidx;
	void OnNpcStatus( AsIMessage _msg)
	{
		Msg_NpcStatus status = _msg as Msg_NpcStatus;

		if( CheckObjectMonster() == true)
			return;

		StartCoroutine( "NpcStatusProcess", status);
	}

	private AsChatBalloonBase prevBalloon = null;
	void PromptLines( string line)
	{
		if( null == line)
			return;

		if( null != prevBalloon)
		{
			GameObject.DestroyImmediate( prevBalloon.gameObject);
			prevBalloon = null;
		}

		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/Optimization/Prefab/GUI_Balloon")) as GameObject;
		Debug.Assert( null != go);
		AsChatBalloonBase balloon = go.GetComponent<AsChatBalloonBase>();
		Debug.Assert( null != balloon);
		balloon.OwnerType = eBalloonOwnerType.Monster;
		balloon.gameObject.transform.localPosition = Vector3.zero;
		balloon.Owner = m_Entity;
		balloon.SetText( line);
		prevBalloon = balloon;
	}

	IEnumerator NpcStatusProcess( Msg_NpcStatus _status)
	{
		while( null == m_Entity.ModelObject)
		{
			yield return null;
		}

		AsEffectManager.Instance.RemoveEffectEntity( m_Statusidx);

		switch( _status.status_)
		{
		case eNPCSTATUS.eNPCSTATUS_FINDENEMY:
			AsEffectManager.Instance.PlayEffect( "Fx/Effect/COMMON/Fx_Common_Add", Entity.ModelObject.transform, false, 0);
			#region -MonsterLines
			if( _status.sessionIdx_ == AsUserInfo.Instance.SavedCharStat.sessionKey_)
			{
				yield return new WaitForSeconds( 1.5f);
				PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.Add));
			}
			#endregion
			break;
		case eNPCSTATUS.eNPCSTATUS_RUNAWAY_START:
			m_Statusidx = AsEffectManager.Instance.PlayEffect( "Fx/Effect/COMMON/Fx_Common_RunAway", Entity.ModelObject.transform, false, 0	);
			break;
		case eNPCSTATUS.eNPCSTATUS_RUNAWAY_END:
			AsEffectManager.Instance.RemoveEffectEntity( m_Statusidx);
			break;
		case eNPCSTATUS.eNPCSTATUS_RESIST_DEBUFF:
			AsHUDController.Instance.panelManager.ShowDebuffResist( Entity.gameObject);
			break;
		}
	}

	void OnMoveEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
		//SetPlayerFsmState( eMonsterFsmStateType.IDLE);
	}

	void OnRecoverState( AsIMessage _msg)
	{
		SetMonsterFsmState( eMonsterFsmStateType.IDLE);
	}

	void OnDeath( AsIMessage _msg)
	{
		m_ElementProcessor.ReleaseBuff();
		SetMonsterFsmState( eMonsterFsmStateType.DEATH, _msg);

		#region -GameGuide_Monster
		AsGameGuideManager.Instance.CheckUp( eGameGuideType.Monster, m_MonsterEntity.TableIdx);
		#endregion

//		#region -MonsterLines
//		PromptLines( AsMonsterLineManager.Instance.GetLine( m_MonsterEntity.TableIdx, eMonsterLineType.Death));
//		#endregion
	}

	void OnShake( AsIMessage _msg)
	{
		StopCoroutine( "ShakeProcess");
		StartCoroutine( "ShakeProcess", _msg);
	}

	IEnumerator ShakeProcess( Msg_ShakeIndication _msg)
	{
		if( null != m_Entity.ModelObject)
		{
//			Msg_ShakeIndication shake = _msg as Msg_ShakeIndication;

			m_Entity.ModelObject.transform.position = Vector3.zero;

//			float time = shake.time_;
//			float dist = shake.dist_;
		}

		yield return null;
	}
	
	void OnBalloonIndicate( AsIMessage _msg)
	{
		Msg_BalloonIndicate balloon = _msg as Msg_BalloonIndicate;
		string str = AsTableManager.Instance.GetTbl_String( (int)balloon.potency_.Potency_IntValue);
		
		PromptLines( str);
	}
	#endregion

	#region - method -
	void DeathProcess( AsIMessage _msg)
	{
		m_ElementProcessor.ReleaseBuff();
		SetMonsterFsmState( eMonsterFsmStateType.DEATH, _msg);
	}

	public void DeathFade()
	{
		StartCoroutine( "FadeOut");
	}

	IEnumerator FadeIn()
	{
		if( null == m_Renderers)
			yield break;

		fadeTime = 0.0f;

		while( true)
		{
			fadeTime += Time.deltaTime;
			float alpha = Mathf.Clamp01( fadeTime * 0.5f);

			foreach( Renderer ren in m_Renderers)
			{
				if( null == ren)
					continue;

				ren.material.SetFloat( "_Alpha", alpha);
				if( 1.0f <= alpha)
					ren.material.renderQueue = 2000;
			}

			if( 1.0f <= alpha)
				yield break;

			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		if( null == m_Renderers)
			yield break;

		eMonster_Grade eGrade = m_MonsterEntity.GetProperty<eMonster_Grade>( eComponentProperty.GRADE);
		if( eMonster_Grade.Boss == eGrade)
			yield return new WaitForSeconds( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 56).Value * 0.001f);

		fadeTime = 1.0f;

		while( true)
		{
			fadeTime -= Time.deltaTime;
			float alpha = Mathf.Clamp01( fadeTime);

			foreach( Renderer ren in m_Renderers)
			{
				if( null == ren)
					continue;

				ren.material.SetFloat( "_Alpha", alpha);
			}

			if( 0.0f >= alpha)
			{
				if( ( null == MonsterEntity) || ( null == MonsterEntity.ModelObject) || ( null == MonsterEntity.ModelObject.gameObject))
				{
					Debug.LogError( "AsMonsterFsm::FadeOut()[ null == monsterEntity ]");
					yield break;
				}

				MonsterEntity.ModelObject.gameObject.SetActiveRecursively( false);

				#region -MonsterLines
				if( null != prevBalloon)
				{
					prevBalloon.Owner = null;
					prevBalloon = null;
				}
				#endregion

				yield break;
			}

			yield return null;
		}
	}

	bool CheckObjectMonster()
	{
		int monsterId = m_Entity.GetProperty<int>( eComponentProperty.MONSTER_ID);
		Tbl_Monster_Record monsterRecord = AsTableManager.Instance.GetTbl_Monster_Record( monsterId);

		if( monsterRecord != null && ( monsterRecord.Grade == eMonster_Grade.DObject || monsterRecord.Grade == eMonster_Grade.QObject))
			return true;
		else
			return false;
	}

	public bool CheckLastMoveIndicated()
	{
		if( m_LastMoveIndication != null)
		{
			if( m_LastMoveIndication.forceMove_ == false)
				return true;
		}

		return false;
	}

	public bool CheckLastMoveIndication_ForcedMove()
	{
		if( m_LastMoveIndication != null)
		{
			if( m_LastMoveIndication.forceMove_ == true)
				return true;
		}

		return false;
	}

	public void SetLastMoveIndication( Msg_NpcMoveIndicate _msg)
	{
		m_LastMoveIndication = _msg;
	}

	public void LastMoveIndicationUsed()
	{
		m_LastMoveIndication = null;
	}
	
//	float GetCompleteAttackSpeed()
//	{
//		if(m_AttackSC == null)
//		{
//			Debug.Log("AsMonsterFsm::GetCompleteAttackSpeed: m_Attack == null. will return 1f");
//			return 1f;
//		}
//		else
//			return m_AttackSC.action_.ActionSpeed * 0.001f * m_Entity.GetProperty<float>( eComponentProperty.ATTACK_SPEED);
//	}
	#endregion

	#region - deledate -
	float GetCharacterSize()
	{
		return m_Character_Size;
	}
	#endregion

	#region - gizmo -
	void OnDrawGizmos()
	{
		/*
		if( m_Entity.ModelObject == null)
			return;

		Tbl_Action_Record tblAction = null;

		switch( CurrnetFsmStateType)
		{
		case eMonsterFsmStateType.SKILL_READY:
			AsMonsterState_SkillReady skillReady = ( AsMonsterState_SkillReady)m_CurrentFsmState;
			tblAction = skillReady.Action;
			break;
		case eMonsterFsmStateType.SKILL_HIT:
			AsMonsterState_SkillHit skillHit = ( AsMonsterState_SkillHit)m_CurrentFsmState;
			tblAction = skillHit.Action;
			break;
		case eMonsterFsmStateType.SKILL_FINISH:
			AsMonsterState_SkillFinish skillFinish = ( AsMonsterState_SkillFinish)m_CurrentFsmState;
			tblAction = skillFinish.Action;
			break;
		}

		if( ( null != tblAction) && ( eHitAreaShape.Point != tblAction.HitAnimation.hitInfo.AreaShape))
		{
			float maxDist = tblAction.HitAnimation.hitInfo.HitMaxDistance * 0.01f;
			float minDist = tblAction.HitAnimation.hitInfo.HitMinDistance;
			float angle = tblAction.HitAnimation.hitInfo.HitAngle;
			float centerAngle = tblAction.HitAnimation.hitInfo.HitCenterDirectionAngle;

			Vector3 pos = MonsterEntity.ModelObject.transform.position;
			Vector3 dir = MonsterEntity.ModelObject.transform.forward * maxDist;
			dir = Quaternion.AngleAxis( centerAngle, Vector3.up) * dir;
			Vector3 rot1 = Quaternion.AngleAxis( angle * 0.5f, Vector3.up) * dir;
			Vector3 rot2 = Quaternion.AngleAxis( -angle * 0.5f, Vector3.up) * dir;

			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere( pos, minDist);
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere( pos, maxDist);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine( pos, pos + ( dir * 2.0f));

			if( 0.0f < angle)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawLine( pos, pos + rot1);
				Gizmos.DrawLine( pos, pos+ rot2);
			}
		}
		else
		{
			float viewDist = m_MonsterEntity.GetProperty<float>( eComponentProperty.VIEW_DISTANCE);
			float attDist = m_MonsterEntity.GetProperty<float>( eComponentProperty.ATTACK_DISTANCE);
//			float angle = m_MonsterEntity.GetProperty<float>( eComponentProperty.VIEW_ANGLE);
			float angle = 30.0f;

			Vector3 dir = m_MonsterEntity.ModelObject.transform.forward * viewDist;
			Vector3 rot1 = Quaternion.AngleAxis( angle, Vector3.up) * dir;
			Vector3 rot2 = Quaternion.AngleAxis( -angle, Vector3.up) * dir;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere( m_MonsterEntity.ModelObject.transform.position, viewDist);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere( m_MonsterEntity.ModelObject.transform.position, attDist);
			Gizmos.color = Color.green;
			Gizmos.DrawLine( m_MonsterEntity.ModelObject.transform.position, m_MonsterEntity.ModelObject.transform.position + ( dir * 2.0f));

			if( 0.0f < angle)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine( m_MonsterEntity.ModelObject.transform.position, m_MonsterEntity.ModelObject.transform.position + rot1);
				Gizmos.DrawLine( m_MonsterEntity.ModelObject.transform.position, m_MonsterEntity.ModelObject.transform.position + rot2);
			}
		}*/
	}
	#endregion
	#region - public -
	public void TransformBegin()
	{
		if(m_Entity.ModelObject != null)
			m_Entity.ModelObject.SetActive(false);
	}
	
	public void TransformEnd()
	{
		if(m_Entity.ModelObject != null)
			m_Entity.ModelObject.SetActive(true);
	}
	#endregion
}

#region - npc buff temp data -
public class CNpcBuffTempData
{
	body2_SC_NPC_BUFF m_npcBuffData;
//	private float m_fMaxCoolTime = 1.0f;
//    private float m_fCurCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f;

	public body2_SC_NPC_BUFF getNpcBuff
	{
		get
		{
			return m_npcBuffData;
		}
	}

	public float getRemainCoolTime
	{
		get
		{
			return m_fRemainCoolTime;
		}
	}

	public CNpcBuffTempData( body2_SC_NPC_BUFF data)
	{
		m_npcBuffData = data;

		if ( 0 < m_npcBuffData.nDuration)
		{
			m_fRemainCoolTime = ( ( float)m_npcBuffData.nDuration)*0.001f;
		}
	}

	public void SetMinusRemainTime( float fMinTime)
	{
		m_fRemainCoolTime -= fMinTime;
		if ( 0.0f > m_fRemainCoolTime)
			m_fRemainCoolTime = 0.0f;

//		m_fCurCoolTime = ( m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;
	}

	public void Update()
	{
		if ( 0.0f < m_fRemainCoolTime)
		{
			SetMinusRemainTime( Time.deltaTime);
		}
	}
}
#endregion