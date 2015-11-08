using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsOtherUserFsm : AsBaseComponent {

	public enum eOtherUserFsmStateType{
		IDLE, RUN, IDLE_ACTION, DEATH,
		CONDITION_STUN, CONDITION_FREEZE, CONDITION_SLEEP, CONDITION_FORCEDMOVE, CONDITION_FEAR, CONDITION_BINDING, CONDITION_AIRBONE, CONDITION_TRANSFORM,
		DASH,
		SKILL_READY, SKILL_HIT, SKILL_FINISH,
		SKILL, OBJ_STEPPING, OBJ_TRAP,
		PRIVATESHOP,
		COLLECT_WORK,
	}

	#region - member -
	public eOtherUserFsmStateType state_;

	AsUserEntity m_OtherUserEntity;public AsUserEntity OtherUserEntity{get{return m_OtherUserEntity;}}

	protected Dictionary<eOtherUserFsmStateType, AsBaseFsmState<eOtherUserFsmStateType, AsOtherUserFsm>> m_dicFsmState =
		new Dictionary<eOtherUserFsmStateType, AsBaseFsmState<eOtherUserFsmStateType, AsOtherUserFsm>>();
	protected AsBaseFsmState<eOtherUserFsmStateType, AsOtherUserFsm> m_CurrentFsmState;
	public eOtherUserFsmStateType CurrnetFsmStateType{get{return m_CurrentFsmState.FsmStateType;}}
	protected AsBaseFsmState<eOtherUserFsmStateType, AsOtherUserFsm> m_OldFsmState;
	public eOtherUserFsmStateType OldFsmStateType{get{return m_OldFsmState.FsmStateType;}}

	AsBaseEntity m_Target;
	public AsBaseEntity Target
	{
		get
		{
			return m_Target;
		}
		set
		{
			StopCoroutine( "ReleaseTaget_Coroutine");
			StartCoroutine( "ReleaseTaget_Coroutine");
			m_Target = value;
		}
	}
	public float targetDistance
	{
		get
		{
			if( m_Target != null)
				return Vector3.Distance( m_Target.transform.position, transform.position);
			else
				return 0.4f;
		}
	}

	public bool WeaponEquip
	{
		get{
			if( m_OtherUserEntity.getCharView[0].nItemTableIdx == 0)
				return false;
			else
				return true;
		}
	}

	//begin kij buff
	private List<CNpcBuffTempData> m_BuffTempDataList = new List<CNpcBuffTempData>();
	private bool m_isNeedBuffReset = false;
	public List<CNpcBuffTempData> getBuffTempList
	{
		get
		{
			return m_BuffTempDataList;
		}
	}

	public void SetUseNeedBuffReset()
	{
		m_isNeedBuffReset = false;
	}
	public bool isNeedBuffReset
	{
		get
		{
			return m_isNeedBuffReset;
		}
	}
	// end kij buff

	//HIT LATENCY( for sync)
	Msg_OtherCharAttackNpc1 m_AttackSC;

	//etc
	[SerializeField]
	float m_IdleActionCycle = 10f;
	public float IdleActionCycle{get{return m_IdleActionCycle;}}
	[SerializeField]
	float m_IdleActionRevision = 2f;
	public float IdleActionRevision{get{return m_IdleActionRevision;}}

//	static float m_SyncDistance = 0.5f;

	//fx
	ElementProcessor m_ElementProcessor;

	//debug
	public bool showReceiveAttackMsg = false;

	Vector3 m_InitSize = Vector3.one;
	float m_Character_Size = 1f; public float Charracter_Size{get{return m_Character_Size;}}
	
	StanceInfo m_StanceInfo = null;
	
	//pet
	AsNpcEntity m_Pet;
	#endregion
	#region - init & release-
	void Awake()
	{
		m_ComponentType = eComponentType.FSM_OTHER_USER;

		m_OtherUserEntity = GetComponent<AsUserEntity>();
		if( m_OtherUserEntity == null) Debug.LogError( "AsOtherUserFsm::Init: no other user entity attached");
		m_OtherUserEntity.FsmType = eFsmType.OTHER_USER;

		#region - msg -
		//MsgRegistry.RegisterFunction( eMessageType.INPUT, OnInput);
		MsgRegistry.RegisterFunction( eMessageType.LEVEL_UP, OnLevelUp);//#16862 dopamin
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED_DUMMY, OnModelLoaded_Dummy);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_OTHER_USER_INDICATION, OnMove);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_END_INFORM, OnMoveEnd);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_ATTACK_NPC1, OnAttack);
		MsgRegistry.RegisterFunction( eMessageType.HIT_EXECUTION, OnHitExecution);
		MsgRegistry.RegisterFunction( eMessageType.NPC_ATTACK_CHAR2, OnDamaged);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_ATTACK_NPC3, OnAffected);
		
		//PET
		MsgRegistry.RegisterFunction( eMessageType.PET_DATA_INDICATE, OnPetDataIndicate);
		MsgRegistry.RegisterFunction( eMessageType.PET_NAME_CHANGE, OnPetNameChange);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_READY, OnPetSkillReady);
		MsgRegistry.RegisterFunction( eMessageType.PET_DELETE, OnPetDelete);
		MsgRegistry.RegisterFunction( eMessageType.PET_ITEM_VIEW, OnItemView);
		MsgRegistry.RegisterFunction( eMessageType.PET_LEVELUP, OnPetLevelUp);
		MsgRegistry.RegisterFunction( eMessageType.PET_EVOLUTION, OnPetEvolution);
		MsgRegistry.RegisterFunction( eMessageType.PET_POSITION_REFRESH, OnPetPositionRefresh);

		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_END, OnAnimationEnd);

		MsgRegistry.RegisterFunction( eMessageType.RELEASE_TENSION, OnReleaseTension);
        MsgRegistry.RegisterFunction( eMessageType.JUMP_STOP, OnJumpStop);
		MsgRegistry.RegisterFunction( eMessageType.OBJ_STEPPING_MSG, OnSteppingMsg);
		MsgRegistry.RegisterFunction( eMessageType.OBJ_TRAP_MSG, OnTrapMsg);
		
		//STANCE
		MsgRegistry.RegisterFunction( eMessageType.OTHER_CHAR_SKILL_STANCE, OnStance);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_BUFF_ACCURE, OnBuffAccure);
		
		//SKILL EFFECT
		MsgRegistry.RegisterFunction( eMessageType.OTHER_CHAR_SKILL_EFFECT, OnSkillEffect);

		//BUFF
		MsgRegistry.RegisterFunction( eMessageType.CHAR_BUFF, OnBuff);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_DEBUFF, OnDeBuff);
		MsgRegistry.RegisterFunction( eMessageType.BUFF_REFRESH, OnBuffRefresh);
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
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_FORCEDMOVE, OnConditionForcedMove);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_FEAR, OnConditionFear);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_FEAR, OnRecoverConditionFear);
		
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_AIRBONE, OnConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_AIRBONE, OnRecoverConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_TRANSFORM, OnConditionTransform);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverConditionTransform);

		MsgRegistry.RegisterFunction( eMessageType.COLLECT_INFO, OnCollectInfo);

		MsgRegistry.RegisterFunction( eMessageType.CHAR_RESURRECTION, OnResurrection);

		//exception
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_STATE, OnRecoverState);

		//death by other reason( ex. buff)
		MsgRegistry.RegisterFunction( eMessageType.DEATH_INDICATION, OnDeath);

		//private shop
		MsgRegistry.RegisterFunction( eMessageType.OPEN_PRIVATESHOP, OnOpenPrivateShop);
		MsgRegistry.RegisterFunction( eMessageType.CLOSE_PRIVATESHOP, OnClosePrivateShop);

		//emotion
		MsgRegistry.RegisterFunction( eMessageType.EMOTION_INDICATION, OnEmotionIndicate);
		MsgRegistry.RegisterFunction( eMessageType.EMOTICON_SEAT_INDICATION, OnEmoticonSeatIndicate);
		MsgRegistry.RegisterFunction( eMessageType.BALLOON, OnBalloonIndicate);
		#endregion
		#region - state -
		m_dicFsmState.Add( eOtherUserFsmStateType.IDLE, new AsOtherUserState_Idle( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.RUN, new AsOtherUserState_Run( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.DEATH, new AsOtherUserState_Death( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_STUN, new AsOtherUserState_Condition_Stun( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_FREEZE, new AsOtherUserState_Condition_Freeze( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_SLEEP, new AsOtherUserState_Condition_Sleep( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_FORCEDMOVE, new AsOtherUserState_Condition_ForcedMove( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_FEAR, new AsOtherUserState_Condition_Fear( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_AIRBONE, new AsOtherUserState_Condition_AirBone( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.CONDITION_TRANSFORM, new AsOtherUserState_Condition_Transform( this));

//		m_dicFsmState.Add( eOtherUserFsmStateType.DASH, new AsOtherUserState_Dash( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.SKILL_READY, new AsOtherUserState_SkillReady( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.SKILL_HIT, new AsOtherUserState_SkillHit( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.SKILL_FINISH, new AsOtherUserState_SkillFinish( this));
        m_dicFsmState.Add( eOtherUserFsmStateType.OBJ_STEPPING, new AsOtherUserState_ObjStepping( this));

		m_dicFsmState.Add( eOtherUserFsmStateType.PRIVATESHOP, new AsOtherUserState_PrivateShop( this));
		m_dicFsmState.Add( eOtherUserFsmStateType.COLLECT_WORK, new AsOtherUserState_Work_Collect( this));
		#endregion
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);
		
		m_InitSize = m_Entity.transform.localScale;

		_entity.SetGetterTarget( GetterTarget_Dlt);

		m_ElementProcessor = new ElementProcessor( m_Entity);
		m_Entity.SetConditionCheckDelegate( m_ElementProcessor.GetEnableCondition);
		m_Entity.SetBuffCheckDelegate( m_ElementProcessor.CheckBuffInclusion);

		gameObject.layer = LayerMask.NameToLayer( "OtherUser");
	}

	public override void InterInit( AsBaseEntity _entity)
	{
		if( m_Entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == false)
		{
			m_dicFsmState[eOtherUserFsmStateType.IDLE].Init();
			m_dicFsmState[eOtherUserFsmStateType.RUN].Init();
			m_dicFsmState[eOtherUserFsmStateType.DEATH].Init();
			m_dicFsmState[eOtherUserFsmStateType.CONDITION_STUN].Init();
			m_dicFsmState[eOtherUserFsmStateType.CONDITION_FREEZE].Init();
			m_dicFsmState[eOtherUserFsmStateType.CONDITION_SLEEP].Init();
			m_dicFsmState[eOtherUserFsmStateType.CONDITION_AIRBONE].Init();
			m_dicFsmState[eOtherUserFsmStateType.CONDITION_TRANSFORM].Init();
			
	//		m_dicFsmState.Add( eOtherUserFsmStateType.DASH, new AsOtherUserState_Dash( this));
			m_dicFsmState[eOtherUserFsmStateType.SKILL_READY].Init();
			m_dicFsmState[eOtherUserFsmStateType.SKILL_HIT].Init();
			m_dicFsmState[eOtherUserFsmStateType.SKILL_FINISH].Init();
	        m_dicFsmState[eOtherUserFsmStateType.OBJ_STEPPING].Init();

			m_dicFsmState[eOtherUserFsmStateType.PRIVATESHOP].Init();
			m_dicFsmState[eOtherUserFsmStateType.COLLECT_WORK].Init();
		}
	}

	public override void LateInit( AsBaseEntity _entity)
	{
//		SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);

		if( m_Entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == false)
		{
			SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);
		}
		else
		{
			SetOtherUserFsmState( eOtherUserFsmStateType.PRIVATESHOP);
		}
	}

	public override void LastInit( AsBaseEntity _entity)
	{
		
	}

	void Start()
	{
//		if( m_Entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == true)
//		{
//			if( m_CurrentFsmState == null || m_CurrentFsmState.FsmStateType != eOtherUserFsmStateType.PRIVATESHOP)
//				SetOtherUserFsmState( eOtherUserFsmStateType.PRIVATESHOP);
//		}
//		else
//		{
//			float hp = m_OtherUserEntity.GetProperty<float>( eComponentProperty.HP_CUR);
//			if( hp < 1)
//			{
//				SetOtherUserFsmState( eOtherUserFsmStateType.DEATH, new Msg_EnterWorld_Death( true));
//			}
//			else
//			{
//				SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);
//			}
//		}
	}

	void OnDestroy()
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.Exit();
		
		if( m_Pet != null)
			AsEntityManager.Instance.RemovePet( m_Pet);
	}
	#endregion
	#region - fsm -
	public void SetOtherUserFsmState( eOtherUserFsmStateType _type, AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == _type)
			{
				Debug.LogWarning( "AsOtherUserFsm::SetOtherUserFsmState: same state = " + _type);
				return;
			}

			m_CurrentFsmState.Exit();
			m_OldFsmState = m_CurrentFsmState;
		}

		if( m_dicFsmState.ContainsKey( _type) == true)
		{
			state_ = _type;
			m_CurrentFsmState = m_dicFsmState[_type];
			m_CurrentFsmState.Enter( _msg);
		}
	}

	public void SetOtherUserFsmState( eOtherUserFsmStateType _type)
	{
		SetOtherUserFsmState( _type, null);
	}
	#endregion
	#region - update -
	void Update()
	{
		if( null != m_CurrentFsmState)
			m_CurrentFsmState.Update();

		if( null != m_ElementProcessor)
			m_ElementProcessor.Update();

		for(int i=0; i<m_BuffTempDataList.Count; ++i)
		{
			m_BuffTempDataList[i].Update();
		}
	}
	#endregion = update =

	#region - element process -
	public void SetAction( Tbl_Action_Record _record)
	{
		m_ElementProcessor.SetAction( _record);
	}

	public void PlayElements( eActionStep _step)
	{
		m_ElementProcessor.PlayElement( _step, Vector3.zero, null);
	}

	public void PlayElements( eActionStep _step, Vector3 _pos, Transform _targetTrn, float _animSpeed)
	{
		m_ElementProcessor.PlayElement( _step, _pos, _targetTrn, _animSpeed);
	}

	public void ReleaseElements()
	{
		m_ElementProcessor.Release( false);
	}

	public void PlayAction_Idle()
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "Idle");
		SetAction( action);
		PlayElements( eActionStep.Entire);
	}
	public void PlayAction_BattleIdle()
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "BattleIdle");
		SetAction( action);
		PlayElements( eActionStep.Entire);
	}
	public void PlayAction_IdleAction()
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "IdleAction");
		SetAction( action);
		PlayElements( eActionStep.Entire);
	}
	public void PlayAction_Run()
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "Run");
		SetAction( action);
		PlayElements( eActionStep.Entire);
	}
	public void PlayAction_BattleRun()
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( __class, gender, "BattleRun");
		SetAction( action);
		PlayElements( eActionStep.Entire);
	}
	#endregion
	#region - skill effect from server -
	void OnSkillEffect( AsIMessage _msg)
	{
		Msg_OtherCharSkillEffect skill = _msg as Msg_OtherCharSkillEffect;

		m_ElementProcessor.PotencyProcess( skill);

		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( skill.skillTableIdx_);
		if( skillRecord.SkillName_Print != eSkillNamePrint.None)
		{
			string str = AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index);
			m_Entity.SkillNameShout( eSkillNameShoutType.Self, str);
		}

		if( ePotency_Type.DebuffResist == skillRecord.listSkillPotency[ skill.potencyIdx_].Potency_Type)
			AsHUDController.Instance.panelManager.ShowDebuffResist( Entity.gameObject);
	}
	#endregion
	#region - stance -
	void OnStance( AsIMessage _msg)
	{
		Msg_OtherCharSkillStance stance = _msg as Msg_OtherCharSkillStance;
		m_StanceInfo = new StanceInfo( stance);
		m_OtherUserEntity.SetStance( m_StanceInfo);
		
//		Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record( stance.stanceSkill_);
//		eGENDER gender = m_Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		
		Msg_OtherCharAttackNpc1 attack = new Msg_OtherCharAttackNpc1( stance);
		Msg_OtherCharAttackNpc_Ready ready = new Msg_OtherCharAttackNpc_Ready( attack);
		m_CurrentFsmState.MessageProcess( ready);
		
		m_OtherUserEntity.HandleMessage( new Msg_CombatBegin());
		
		m_ElementProcessor.PotencyProcess( stance);
	}
	void OnBuffAccure( AsIMessage _msg)
	{
		Msg_CharBuffAccure accure = _msg as Msg_CharBuffAccure;
		
		StartCoroutine(BuffAccure_CR(accure));
	}
	int m_AccureCritical = 0;
	int m_AccureDodge = 0;
	IEnumerator BuffAccure_CR(Msg_CharBuffAccure _accure)
	{
		while(true)
		{
			if( null == OtherUserEntity.ModelObject)
				yield return null;
			else
			{
				if( m_AccureDodge != 0 && _accure.info_.bDodgeChance == false)
				{
					AsEffectManager.Instance.StopEffect(m_AccureDodge);
					m_AccureDodge = 0;
				}
				if( m_AccureDodge == 0 && _accure.info_.bDodgeChance == true)
					m_AccureDodge = AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_DodgeChance", OtherUserEntity.ModelObject.transform, false, 0f);

				if( m_AccureCritical != 0 && _accure.info_.bCriticalChance == false)
				{
					AsEffectManager.Instance.StopEffect(m_AccureCritical);
					m_AccureCritical = 0;
				}
				if( m_AccureCritical == 0 && _accure.info_.bCriticalChance == true)
					m_AccureCritical = AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_CriticalChance", OtherUserEntity.ModelObject.transform, false, 0f);
					
				break;
			}
		}
	}
	#endregion
	#region - buff -
	void OnBuff( AsIMessage _msg)
	{
		Msg_CharBuff buff = _msg as Msg_CharBuff;

		foreach( Msg_CharBuff_Body body in buff.listBuff_)
		{
			m_ElementProcessor.SetBuff( buff, body);

			if( 0 < body.duration_ && false == IsSameBuff( body.skillTableIdx_, body.potencyIdx_))
			{
				body2_SC_NPC_BUFF data = new body2_SC_NPC_BUFF();
				data.bUpdate = body.serverData.bUpdate;
				data.nSkillTableIdx = body.serverData.nSkillTableIdx;
				data.nSkillLevel = body.serverData.nSkillLevel;
				data.nChargeStep = body.serverData.nChargeStep;
				data.nPotencyIdx = body.serverData.nPotencyIdx;
				data.eType = body.serverData.eType;
				data.nDuration = body.serverData.nDuration;

				m_BuffTempDataList.Add( new CNpcBuffTempData( data));
				m_isNeedBuffReset = true;
			}

//			m_Entity.SetProperty( eComponentProperty.CONDITION,
//				AsEnumConverter.GetConditionFromBuffType( body.type_));
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
		Msg_CharDeBuff deBuff = _msg as Msg_CharDeBuff;

		m_ElementProcessor.RemoveBuff( deBuff);
//		ePotency_Enable_Condition condition = m_Entity.GetProperty<ePotency_Enable_Condition>( eComponentProperty.CONDITION);
//		if( m_Entity.GetProperty<ePotency_Enable_Condition>( eComponentProperty.CONDITION) == condition)
//			m_Entity.SetProperty( eComponentProperty.CONDITION, ePotency_Enable_Condition.NONE);

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
	
	void OnBuffRefresh( AsIMessage _msg)
	{
//		Msg_BuffRefresh msg = _msg as Msg_BuffRefresh;

		m_ElementProcessor.RefreshBuff();
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
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_STUN);
	}

	void OnRecoverConditionStun( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionFreeze( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_FREEZE);
	}

	void OnRecoverConditionFreeze( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionSleep( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_SLEEP);
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

	void OnCollectInfo( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionForcedMove( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_FORCEDMOVE, _msg);
	}

	void OnConditionFear( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_FEAR);
	}

	void OnRecoverConditionFear( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnConditionAirBone( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_AIRBONE, _msg);
	}

	void OnRecoverConditionAirBone( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnConditionTransform( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.CONDITION_TRANSFORM, _msg);
	}

	void OnRecoverConditionTransform( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	//MESSAGE
	#region - level up -
	Msg_Level_Up m_LevelUp = null;
	void OnLevelUp( AsIMessage _msg)
	{
		m_LevelUp = _msg as Msg_Level_Up;
		LevelUpExecution();
		
		#region - condition -
		AsEmotionManager.Instance.Event_Condition_LevelUp();
		#endregion
	}

	void LevelUpExecution()
	{
		m_Entity.SetProperty( eComponentProperty.LEVEL, m_LevelUp.data_.nLevel);
		m_Entity.SetProperty( eComponentProperty.HP_CUR, m_LevelUp.data_.sFinalStatus.fHPMax);
		m_Entity.SetProperty( eComponentProperty.HP_MAX, m_LevelUp.data_.sFinalStatus.fHPMax);
		m_Entity.SetProperty( eComponentProperty.MP_CUR, m_LevelUp.data_.sFinalStatus.fMPMax);
		m_Entity.SetProperty( eComponentProperty.MP_MAX, m_LevelUp.data_.sFinalStatus.fMPMax);
//		m_Entity.SetProperty( eComponentProperty.ATTACK, m_LevelUp.data_.sFinalStatus.nAttr_Dmg);
//		m_Entity.SetProperty( eComponentProperty.DEFENCE, m_LevelUp.data_.sFinalStatus.nPhysic_Def);

//		AsMyProperty.Instance.LevelUpDisplay();

	    //#16862 dopamin
		if( null != Entity.ModelObject)
		{
			AsEffectManager.Instance.PlayEffect( "Fx/Effect/Common/Fx_Common_LevelUp",Entity.ModelObject.transform,false,0f);
			AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_Levelup_Eff", Entity.ModelObject.transform.position, false);
		}

		if( null != AsHudDlgMgr.Instance)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				AsHudDlgMgr.Instance.playerStatusDlg.ResetPageText();
		}

		m_LevelUp = null;
	}
	#endregion
	#region - model loaded -
	void OnModelLoaded( AsIMessage _msg)
	{
		// Debug.LogWarning( "AsOtherUserFsm::OnModelLoaded: cur state = " + m_CurrentFsmState.FsmStateType);
		
		m_ElementProcessor.ModelLoaded();
		
		StartCoroutine(SceneLoadingComplete_CR( _msg));
	}
	
	
	IEnumerator SceneLoadingComplete_CR( AsIMessage _msg)
	{
		while(true)
		{
			if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
				break;
			
			yield return null;
		}
		
		if( m_Entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == false)	
		{		
			float hp = m_OtherUserEntity.GetProperty<float>( eComponentProperty.HP_CUR);		
			Debug.Log( "AsOtherUserFsm::OnModelLoaded: hp = " + hp);		
			if( hp < 1)		
			{		
				SetOtherUserFsmState( eOtherUserFsmStateType.DEATH, new Msg_EnterWorld_Death( true));		
			}		
			else		
			{		
				SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);		
				PlayAction_Idle();		
			}	
		}
	
		if( m_CurrentFsmState != null)		
			m_CurrentFsmState.MessageProcess( _msg);
	}

	
	void OnModelLoaded_Dummy( AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion
	#region - input & move -
	void OnInput( AsIMessage _msg)
	{
//		Msg_Input msg = _msg as Msg_Input;
//
//		if( msg.type_ == eInputType.INPUT_UP)
//		{
//			if( msg.worldPosition_ != Vector3.zero)
//			{
//				Msg_MoveInfo moveInfoMsg = new Msg_MoveInfo( msg.inputObject_.GetInstanceID(), msg.worldPosition_);
//				m_Entity.HandleMessage( moveInfoMsg);
//			}
//		}
	}
	
	void OnMove( AsIMessage _msg)
	{
		Msg_OtherUserMoveIndicate msg = _msg as Msg_OtherUserMoveIndicate;

//		if( msg.moveType_ == eMoveType.Combat)
//			m_Entity.HandleMessage( new Msg_CombatBegin());
//
//		if( msg.moveType_ == eMoveType.Sync_Move)// || msg.moveType_ == eMoveType.Sync_Combat)
//		{
//			if( Vector3.Distance( msg.destPosition_, transform.position) > m_SyncDistance)
//				m_CurrentFsmState.MessageProcess( _msg);
//
//			SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);
//			return;
//		}
		float fy = TerrainMgr.GetTerrainHeight(msg.destPosition_ );
		float fy_1 = TerrainMgr.GetTerrainHeight( msg.curPosition_ );
		Vector3 temp = msg.destPosition_;
		temp.y = fy;
		
		Vector3 temp_1 = msg.curPosition_;
		temp_1.y = fy_1;
		 
		msg.destPosition_ = temp; 
		msg.curPosition_ = temp_1;

		switch( msg.moveType_)
		{
		case eMoveType.Combat:
			m_Entity.HandleMessage( new Msg_CombatBegin());
			m_CurrentFsmState.MessageProcess( _msg);
			break;
//		case eMoveType.Sync_Move:
//
//			break;
		case eMoveType.Sync_Stop:
			m_Entity.transform.position = msg.destPosition_;
			m_Entity.HandleMessage( new Msg_MoveStopIndication());
			break;
		default:
			m_CurrentFsmState.MessageProcess( _msg);
			break;
		}

//		if( msg.destPosition_ != msg.curPosition_)
//		{
//			if( msg.moveType_ == eMoveType.Dash && CurrnetFsmStateType != eOtherUserFsmStateType.DASH)
//				SetOtherUserFsmState( eOtherUserFsmStateType.DASH, msg);
//			else
//				m_CurrentFsmState.MessageProcess( _msg);
//		}
	}
	
	void OnMoveEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion
	#region - object interaction -
    void OnJumpStop( AsIMessage _msg)
    {
        m_CurrentFsmState.MessageProcess( _msg);
    }

	void OnSteppingMsg( AsIMessage _msg)
	{
		if ( state_ != eOtherUserFsmStateType.OBJ_STEPPING)
            SetOtherUserFsmState( eOtherUserFsmStateType.OBJ_STEPPING);

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnTrapMsg( AsIMessage _msg)
	{
		if ( state_ != eOtherUserFsmStateType.OBJ_TRAP)
            SetOtherUserFsmState( eOtherUserFsmStateType.OBJ_TRAP);

		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion
	void OnResurrection( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#region - skill & pet -
	void OnAttack( AsIMessage _msg)
	{
		if( m_AttackSC != null && m_AttackSC.ready_ == false)
			OnHitExecution( _msg);

		m_AttackSC = _msg as Msg_OtherCharAttackNpc1;
		m_OtherUserEntity.HandleMessage( new Msg_CombatBegin());
		
		if( m_AttackSC.ready_ == true)
		{
//			float attackSpeed = m_Entity.GetProperty<float>( eComponentProperty.ATTACK_SPEED);
			Msg_OtherCharAttackNpc_Ready ready = new Msg_OtherCharAttackNpc_Ready( m_AttackSC);//, attackSpeed);
			if( ready.CheckNonAnimation() == false)
				m_CurrentFsmState.MessageProcess( ready);
		}
		else
		{
//			AsTimerManager.Instance.SetTimer( 3, Timer_LateExecution);

//			int actionIdx = m_AttackSC.actionTableIdx_;
//			Tbl_Action_Record record = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);
//			if( record.HitAnimation.hitInfo.HitType != eHitType.ProjectileTarget)
//			{
				OnHitExecution( _msg);
//			}
		}

		if( showReceiveAttackMsg == true)
		{
			Debug.Log( "AsOtherUserFsm::OnAttack: session id=" + m_AttackSC.sessionId_ + " charUniqKey_" + m_AttackSC.charUniqKey_ +
				" npc id=" + m_AttackSC.npcIdx_ + " target pos=" + m_AttackSC.targeting_ + " direction=" + m_AttackSC.direction_ +
				" skill table idx=" + m_AttackSC.skillTableIdx_ + " skill level=" + m_AttackSC.skillLevel_ +
				" action table idx=" + m_AttackSC.actionTableIdx_ + " charge step=" + m_AttackSC.chargeStep_ +
				" casting=" + m_AttackSC.casting_ + " ready=" + m_AttackSC.ready_ +
				" cur hp=" + m_AttackSC.hpCur_ + " npc cnt=" + m_AttackSC.npcCnt_ + " char cnt=" + m_AttackSC.charCnt_);
		}
	}
	
	void OnHitExecution( AsIMessage _msg)
	{
		if( m_AttackSC == null)
		{
//			Debug.LogWarning( "AsOtherUserFsm::OnHitExecution: no saved attack packet.");
			return;
		}

		ProjectileHit();

		#region - hud -
//		// < ilmeda 20120425
//		if( null != AsHUDController.Instance)
//		{
//			AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( attack2.npcIdx_);
//			if( null != npcEntity)
//			{
//				AsHUDController.Instance.panelManager.ShowNumberPanel( npcEntity.gameObject, ( int)attack2.damage_, attack2.eDamageType, attack2.parent_.action_, true);//, m_AttackSC.actionTableIdx_);
//
//				if( ( int)attack2.reflection_ > 0)
//				{
//					AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//					AsHUDController.Instance.panelManager.ShowNumberPanel( userEntity.transform.gameObject, ( int)( attack2.reflection_), attack2.eDamageType, false);
//				}
//
//				if( null != AsHUDController.Instance.targetInfo.getCurTargetEntity)
//				{
//					if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == ( ( AsBaseEntity)npcEntity).GetInstanceID())
//						AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( ( AsBaseEntity)npcEntity);
//				}
//			}
//		}
		#endregion

		foreach( Msg_OtherCharAttackNpc2 attack2 in m_AttackSC.npcBody_)
		{
			AsEntityManager.Instance.DispatchMessageByNpcSessionId( attack2.npcIdx_, attack2);

			AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
			if( null != playerFsm)
				playerFsm.UpdateTargetInfoByOtherUser( attack2);
		}

		//pc
		foreach( Msg_OtherCharAttackNpc3 attack3 in m_AttackSC.charBody_)
		{
			AsEntityManager.Instance.DispatchMessageByUniqueKey( attack3.charUniqKey_, attack3);

			if( attack3.reflection_ > 0 && Entity != null)
			{
				if( null != AsHUDController.Instance)
				{
					AsHUDController.Instance.panelManager.ShowNumberPanel( Entity.gameObject, ( int)attack3.reflection_, attack3.eDamageType, true);	// need reflection font
					m_Entity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);
					AsEntityManager.Instance.MessageToPlayer( new Msg_ReflectionRefresh( Entity));
				}
			}
		}

		m_AttackSC = null;//release
	}

	void ProjectileHit()
	{
		int actionIdx = m_AttackSC.actionTableIdx_;
		Tbl_Action_Record record = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);
		if( record.HitAnimation != null && record.HitAnimation.hitInfo.HitType == eHitType.ProjectileTarget)
		{
			foreach(int node in m_AttackSC.npcIdx_)
			{
				AsNpcEntity entity = AsEntityManager.Instance.GetNpcEntityBySessionId( node);
				if( entity != null &&   entity.GetProperty<float>( eComponentProperty.HP_CUR) > 0)  //dopamin
				{
					AsEffectManager.Instance.PlayEffect( record.HitAnimation.hitInfo.HitProjectileHitFileName,
						entity.transform, false, 0);
				}
			}
		}
	}
	
	void OnDamaged( AsIMessage _msg)
	{
		Msg_NpcAttackChar2 attack = _msg as Msg_NpcAttackChar2;
		m_Entity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);

		m_OtherUserEntity.HandleMessage( new Msg_CombatBegin());

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

//		#region - skill name shout -
//		if( attack.parent_.skill_.SkillName_Print == true)
//		{
//			string skillName = AsTableManager.Instance.GetTbl_String( attack.parent_.skill_.SkillName_Index);
//			m_Entity.SkillNameShout( eSkillNameShoutType.Harm, skillName);
//		}
//		#endregion

		if( attack.hpCur_ < 1)
		{
			StartCoroutine( DeathProcess( _msg));
		}

		if( null != AsHUDController.Instance.targetInfo.getCurTargetEntity)
		{
			if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == m_Entity.GetInstanceID())
				AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( m_Entity);
		}
	}
	
	void OnAffected( AsIMessage _msg)
	{
		if( m_Entity == null)
			return;

		Msg_OtherCharAttackNpc3 attack = _msg as Msg_OtherCharAttackNpc3;

		if( attack.damage_ > 0)
			 m_Entity.HandleMessage( new Msg_CombatBegin());

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

		#region - hud -
		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
			hud = go.GetComponent<AsHUDController>();

		if( null != hud)
		{
			if( ( int)attack.hpHeal_ > 0)
				hud.panelManager.ShowNumberPanel( m_OtherUserEntity, ( int)attack.hpHeal_, eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_HP, attack.eDamageType);
//#if _PVP
			if( TargetDecider.CheckOtherUserIsEnemy( m_Entity) == true)
			{
				if( attack.eDamageType != eDAMAGETYPE.eDAMAGETYPE_CRITICAL)
				{
					if( ( int)attack.hpHeal_ < 0)
					{
						int nDamage = ( int)attack.hpHeal_ * -1;
						hud.panelManager.ShowNumberPanel( m_OtherUserEntity.gameObject, nDamage, attack.eDamageType, attack.parent_.action_, true);
					}
					else
						hud.panelManager.ShowNumberPanel( m_OtherUserEntity.gameObject, ( int)attack.damage_, attack.eDamageType, attack.parent_.action_, true);
				}
				else if( ( int)attack.damage_ > 0)
					hud.panelManager.ShowNumberPanel( m_OtherUserEntity.gameObject, ( int)attack.damage_, attack.eDamageType, attack.parent_.action_, true);
			}//&& ( int)attack.damage_ > 0)
			//	hud.panelManager.ShowNumberPanel( m_OtherUserEntity.gameObject, ( int)attack.damage_, attack.eDamageType, attack.parent_.action_);
//#endif
		}
		#endregion

		m_Entity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);

		#region - skill name shout -
		if( attack.parent_.skill_.SkillName_Print != eSkillNamePrint.None &&
			attack.parent_.charUniqKey_ == AsUserInfo.Instance.GetCurrentUserEntity().UniqueId)
		{
			if( attack.parent_.skill_.CheckPotencyTypeIncludeHeal() == true)
			{
				string skillName = AsTableManager.Instance.GetTbl_String( attack.parent_.skill_.SkillName_Index);
				m_Entity.SkillNameShout( eSkillNameShoutType.Benefit, skillName);
			}
		}
		#endregion
		#region - target hud refresh -
		if( null != hud && AsHUDController.Instance.targetInfo.getCurTargetEntity != null)
		{
			if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == m_Entity.GetInstanceID())
				hud.targetInfo.UpdateTargetSimpleInfo( m_Entity);
		}
		#endregion
	}
	
	Msg_PetDataIndicate m_PetData;
	Msg_PetItemView m_PetItem;
	void OnPetDataIndicate( AsIMessage _msg)
	{
		Msg_PetDataIndicate indicate = _msg as Msg_PetDataIndicate;
		m_Pet = AsEntityManager.Instance.PetAppear( indicate.data_);
		m_Pet.HandleMessage( indicate);
		
		m_PetData = indicate;
		
		m_OtherUserEntity.OwnPet( true);

		if( indicate.data_.initial_ == true)
			StartCoroutine( HatchPerform());
	}
	
	IEnumerator HatchPerform()
	{
		while(true)
		{
			yield return null;
			
			if( m_Pet.CheckModelLoadingState() == eModelLoadingState.Finished)
				break;
		}
		
		if( m_Pet != null) m_Pet.HandleMessage( new Msg_PetHatchIndicate());
	}
	
	void OnPetNameChange( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	
	void OnPetSkillReady( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	
	void OnPetDelete( AsIMessage _msg)
	{
		if(m_Pet != null)
			AsEntityManager.Instance.RemovePet( m_Pet);
		else
			Debug.LogError("AsOtherUserFsm:: OnPetDelete: there is no pet. invalid release");

		m_OtherUserEntity.OwnPet( false);
	}
	
	void OnItemView( AsIMessage _msg)
	{
		Msg_PetItemView view = _msg as Msg_PetItemView;
		if(m_Pet != null)
			m_Pet.SetPetItemView( view);
		
		m_PetItem = view;
	}
	
	void OnPetLevelUp( AsIMessage _msg)
	{
		if( m_Pet != null)
		{
			m_Pet.HandleMessage( _msg);
			
			Msg_Pet_LevelUp lvUp = _msg as Msg_Pet_LevelUp;
//			m_PetData.data_.nLevel_ = lvUp.levelUp_.nLevel;
		}
	}
	
	void OnPetEvolution( AsIMessage _msg)
	{
		AsEntityManager.Instance.RemovePet( m_Pet);
		
		m_Pet = AsEntityManager.Instance.PetAppear( m_PetData.data_);
		m_Pet.HandleMessage( m_PetData);
		
		if(m_PetItem != null)
			m_Pet.SetPetItemView( m_PetItem);
	}
	
	void OnPetPositionRefresh( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	#endregion

	#region - death process -
	IEnumerator DeathProcess( AsIMessage _msg)
	{
		#region - accure -
		AsEffectManager.Instance.StopEffect(m_AccureDodge);
		AsEffectManager.Instance.StopEffect(m_AccureCritical);
		
		m_AccureDodge = 0;
		m_AccureCritical = 0;
		#endregion
		
		yield return new WaitForSeconds( 0.5f);

//		m_ElementProcessor.ReleaseBuff();
		SetOtherUserFsmState( eOtherUserFsmStateType.DEATH);
	}
	#endregion

	#region - shop & emotion -
	void OnOpenPrivateShop( AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == eOtherUserFsmStateType.PRIVATESHOP)
				m_CurrentFsmState.MessageProcess( _msg);
			else
				SetOtherUserFsmState( eOtherUserFsmStateType.PRIVATESHOP, _msg);
		}
	}
	void OnClosePrivateShop( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnEmotionIndicate( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnEmoticonSeatIndicate( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnBalloonIndicate( AsIMessage _msg)
	{
		Msg_BalloonIndicate balloon = _msg as Msg_BalloonIndicate;
		string str = AsTableManager.Instance.GetTbl_String( (int)balloon.potency_.Potency_IntValue);
		
		AsChatManager.Instance.ShowChatBalloon( m_OtherUserEntity.UniqueId, str, eCHATTYPE.eCHATTYPE_PUBLIC);
	}
	#endregion

	#region - anim end -
	void OnAnimationEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - release tension -
	void OnReleaseTension( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - exception -
	void OnRecoverState( AsIMessage _msg)
	{
		SetOtherUserFsmState( eOtherUserFsmStateType.IDLE);
	}
	#endregion
	#region - death by other reason -
	void OnDeath( AsIMessage _msg)
	{
		StartCoroutine( DeathProcess( _msg));
	}
	#endregion

	#region - delegate -
	AsBaseEntity GetterTarget_Dlt()
	{
		return Target;
	}
	#endregion
	#region - coroutine -
	IEnumerator ReleaseTaget_Coroutine()
	{
		float interval = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 48).Value;

		yield return new WaitForSeconds( interval);

		m_Target = null;
	}
	#endregion
	#region - public -
	public void EnterPStore()
	{
		StartCoroutine( _PetModelProeByPStore( false));
	}
	public void ExitPStore()
	{
		StartCoroutine( _PetModelProeByPStore( true));
	}
	IEnumerator _PetModelProeByPStore( bool _active)
	{
		while(true)
		{
			yield return null;
			
			if( m_Pet != null && m_Pet.CheckModelLoadingState() == eModelLoadingState.Finished)
			{
				m_Pet.ModelObject.SetActive( _active);
				m_Pet.namePanel.gameObject.SetActive( _active);
				
				break;
			}
		}
	}

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
