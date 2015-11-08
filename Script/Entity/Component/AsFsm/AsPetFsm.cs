using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPetFsm : AsBaseComponent
{
	public enum ePetFsmStateType
	{
		NONE,
		DISPLAY,
		HATCH,
		IDLE, RUN,
		SKILL_READY, SKILL_HIT, SKILL_FINISH
	}

	#region - member -
	public static readonly float s_sqrFollowOwnerRange = 2f * 2f;
	public static readonly float s_sqrStopFollowRange = 1f * 1f;
	public static readonly float s_sqrAbsoluteFollowRange = 20f * 20f;
	
	public ePetFsmStateType state_;
	AsNpcEntity m_PetEntity;public AsNpcEntity PetEntity	{ get { return m_PetEntity; } }

	protected Dictionary<ePetFsmStateType, AsBaseFsmState<ePetFsmStateType, AsPetFsm>> m_dicFsmState =
		new Dictionary<ePetFsmStateType, AsBaseFsmState<ePetFsmStateType, AsPetFsm>>();
	protected AsBaseFsmState<ePetFsmStateType, AsPetFsm> m_CurrentFsmState;
	public ePetFsmStateType CurrnetFsmStateType	{ get { return m_CurrentFsmState.FsmStateType; } }
	protected AsBaseFsmState<ePetFsmStateType, AsPetFsm> m_OldFsmState;
	public ePetFsmStateType OldFsmStateType	{ get { return m_OldFsmState.FsmStateType; } }

	//fx
	ElementProcessor m_ElementProcessor;
	
	Vector3 m_InitSize = Vector3.one;
	float m_Character_Size = 1f;
	public float Charracter_Size	{ get { return m_Character_Size; } }
	
	AsUserEntity m_PetOwner; public AsUserEntity PetOwner{get{return m_PetOwner;}}
	
	sPETSKILL[] m_PetSkill;
	float[] m_RemainCoolTime = new float[]{ 999f, 999f, 999f};
	public bool CheckSkillUsable( ePET_SKILL_TYPE _type)
	{
		if( m_RemainCoolTime[ (int)_type] <= 0f)
			return true;
		else
			return false;
	}
	
	public sPETSKILL GetSkill( ePET_SKILL_TYPE _type)
	{
		if( m_PetSkill[ (int)_type] != null)
			return m_PetSkill[ (int)_type];
		else
		{
			Debug.LogError( "AsPetFsm:: GetSkill: there is no skill. type = " + _type);
			return null;
		}
	}
	
	Msg_PetDataIndicate m_PetData;
	#endregion

	#region - init & release -
	void Awake()
	{
		m_ComponentType = eComponentType.FSM_MONSTER;

		m_PetEntity = GetComponent<AsNpcEntity>();
		if( m_PetEntity == null)
			Debug.LogError( "AsPetFsm::Init: no user entity attached");
		m_PetEntity.FsmType = eFsmType.PET;		
		
		#region - msg -
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_NPC_INDICATION, OnMoveIndication);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_END_INFORM, OnMoveEnd);
		
		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_END, OnAnimationEnd);
		
		MsgRegistry.RegisterFunction( eMessageType.BALLOON, OnBalloonIndicate);
		
		MsgRegistry.RegisterFunction( eMessageType.PET_DATA_INDICATE, OnPetDataIndicate);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_READY, OnPetSkillReady);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_RESULT, OnPetSkillResult);
		
		MsgRegistry.RegisterFunction( eMessageType.PET_NAME_CHANGE, OnPetNameChange);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_CHANGE_RESULT, OnPetSkillChangeResult);
		
		MsgRegistry.RegisterFunction( eMessageType.PET_HATCH, OnHatch);
		MsgRegistry.RegisterFunction( eMessageType.PET_FEEDING, OnFeeding);
		MsgRegistry.RegisterFunction( eMessageType.PET_SCRIPT, OnPetScript);
		MsgRegistry.RegisterFunction( eMessageType.PET_LEVELUP, OnPetLevelUp);
		MsgRegistry.RegisterFunction( eMessageType.PET_EVOLUTION, OnPetEvolution);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_GET, OnPetSkillGet);
		MsgRegistry.RegisterFunction( eMessageType.PET_POSITION_REFRESH, OnPetPositionRefresh);
		MsgRegistry.RegisterFunction( eMessageType.PET_HUNGRY_INDICATE, OnPetHungryIndicate);
		#endregion
		#region - state -
		m_dicFsmState.Add( ePetFsmStateType.HATCH, new AsPetState_Hatch( this));
		m_dicFsmState.Add( ePetFsmStateType.DISPLAY, new AsPetState_Display( this));
		m_dicFsmState.Add( ePetFsmStateType.IDLE, new AsPetState_Idle( this));
		m_dicFsmState.Add( ePetFsmStateType.RUN, new AsPetState_Run( this));

		m_dicFsmState.Add( ePetFsmStateType.SKILL_READY, new AsPetState_SkillReady( this));
		m_dicFsmState.Add( ePetFsmStateType.SKILL_HIT, new AsPetState_SkillHit( this));
		m_dicFsmState.Add( ePetFsmStateType.SKILL_FINISH, new AsPetState_SkillFinish( this));
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

		gameObject.layer = LayerMask.NameToLayer( "Pet");
	}

	public override void InterInit( AsBaseEntity _entity)
	{
		m_dicFsmState[ePetFsmStateType.IDLE].Init();
		m_dicFsmState[ePetFsmStateType.RUN].Init();

		m_dicFsmState[ePetFsmStateType.SKILL_READY].Init();
		m_dicFsmState[ePetFsmStateType.SKILL_HIT].Init();
		m_dicFsmState[ePetFsmStateType.SKILL_FINISH].Init();
	}

	public override void LateInit( AsBaseEntity _entity)
	{
		SetPetFsmState( ePetFsmStateType.IDLE);
	}

	public override void LastInit( AsBaseEntity _entity)
	{
		m_InitSize = m_Entity.transform.localScale;
	}

	void Start()
	{
		m_InitSize = m_PetEntity.transform.localScale;

		if(m_PetOwner.FsmType == eFsmType.PLAYER)
			m_ScriptCoolTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetScriptCool").Value * 0.001f;
	}

	void OnDisable()
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.Exit();
	}
	#endregion

	#region - fsm -
	public void SetPetFsmState( ePetFsmStateType _type, AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == _type)
			{
				Debug.LogWarning( "AsPetFsm::SetPetFsmState: same state = " + _type);
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

	public void SetPetFsmState( ePetFsmStateType _type)
	{
		SetPetFsmState( _type, null);
	}
	#endregion

	#region - update -
	void Update()
	{
		m_CurrentFsmState.Update();
		m_ElementProcessor.Update();

		ScriptCoolTimeProc();
	}

	void LateUpdate()
	{
		EffectUpdate();
	}
	
	void CoolTimeProc()
	{
		for( int i=0; i<(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX; ++i)
		{
			if( m_RemainCoolTime[i] > 0f)
				m_RemainCoolTime[i] -= Time.deltaTime;
		}
	}

	float m_ScriptTime = 0f;
	float m_ScriptCoolTime = -1f;
	void ScriptCoolTimeProc()
	{
		if(m_ScriptCoolTime < 0f)
			return;

		m_ScriptTime += Time.deltaTime;
		if(m_ScriptTime > m_ScriptCoolTime)
		{
			m_CurrentFsmState.MessageProcess(new Msg_Pet_Script_Indicate(m_PetData));
			m_ScriptTime = 0f;
		}
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

	#region - hit execution -
	void OnHitExecution( AsIMessage _msg)
	{
	}

	void ProjectileHit()
	{
	}
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
	}
	#endregion

	#region - msg -
	void OnModelLoaded( AsIMessage _msg)
	{
		m_ElementProcessor.ModelLoaded();

		StartCoroutine(_PlayerLogin_CR());
		
		m_DummyLeadTop = m_Entity.GetDummyTransform( "DummyLeadTop");
		
		StartCoroutine( _ShopProc_CR());
	}

	IEnumerator _PlayerLogin_CR()
	{
		while(true)
		{
			yield return null;
			
			if(m_PetOwner != null)
				break;
		}

		if(m_PetOwner.FsmType == eFsmType.PLAYER)
			AsPetManager.Instance.PlayerLogin();
	}
	
	IEnumerator _ShopProc_CR()
	{
		while(true)
		{
			yield return null;
			
			if( m_PetOwner != null)
			{
				if( m_PetOwner.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
				{
					m_Entity.ModelObject.SetActive( false);
					m_Entity.namePanel.gameObject.SetActive( false);
				}
				
				break;
			}
		}
	}

	void OnMoveIndication( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnAttackIndication( AsIMessage _msg)
	{
//		if( m_AttackSC.ready_ == false)// || m_CurrentFsmState.FsmStateType == ePetFsmStateType.CONDITION_FORCEDMOVE)
//		{
//			OnHitExecution( m_AttackSC);
//		}
//		else
//		{
//			m_CurrentFsmState.MessageProcess( m_AttackSC);
//		}

		#region -PetLines
//		if( ( null != m_AttackSC) && ( m_AttackSC.charUniqKey_[0] == AsUserInfo.Instance.SavedCharStat.uniqKey_))
//			PromptLines( AsPetLineManager.Instance.GetLine( m_PetEntity.TableIdx, ePetLineType.Skill, eConditionValueCheck.Equal, m_AttackSC.skill_.Index));
		#endregion
	}

	void OnAnimationEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	int m_Statusidx;
	void OnNpcStatus( AsIMessage _msg)
	{
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
			#region -PetLines
//			if( _status.sessionIdx_ == AsUserInfo.Instance.SavedCharStat.sessionKey_)
//			{
//				yield return new WaitForSeconds( 1.5f);
//				PromptLines( AsPetLineManager.Instance.GetLine( m_PetEntity.TableIdx, ePetLineType.Add));
//			}
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
		//SetPlayerFsmState( ePetFsmStateType.IDLE);
	}

	void OnRecoverState( AsIMessage _msg)
	{
		SetPetFsmState( ePetFsmStateType.IDLE);
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
			m_Entity.ModelObject.transform.position = Vector3.zero;
		}

		yield return null;
	}
	
	void OnBalloonIndicate( AsIMessage _msg)
	{
		Msg_BalloonIndicate balloon = _msg as Msg_BalloonIndicate;
		string str = AsTableManager.Instance.GetTbl_String( (int)balloon.potency_.Potency_IntValue);
		
		PromptLines( str);
	}

	//pet
	void OnPetDataIndicate( AsIMessage _msg)
	{
		Msg_PetDataIndicate data = _msg as Msg_PetDataIndicate;
		m_PetData = data;
		
		m_PetOwner = data.data_.owner_;
		gameObject.name += "<" + m_PetOwner.name + "'s pet" + ">";
		
		m_PetEntity.SetPetItemView( m_PetData.data_.itemIdx_);
		
		if(data.display_ == true)
			SetPetFsmState(AsPetFsm.ePetFsmStateType.DISPLAY);
	}
	
	void OnPetSkillReady( AsIMessage _msg)
	{
		SetPetFsmState(AsPetFsm.ePetFsmStateType.SKILL_READY, _msg);
	}
	
	void OnPetSkillResult( AsIMessage _msg)
	{
//		Msg_Pet_Skill_Result result = _msg as Msg_Pet_Skill_Result;
//		
//		for( int i=0; i<m_PetSkill.Length; ++i)
//		{
//			if( m_PetSkill[i] != null && m_PetSkill[i].nSkillTableIdx == result.result_.nSkillIdx)
//				m_RemainCoolTime[i] = (float)result.result_.nCoolTime * 0.001f;
//		}
	}
	
	void OnPetNameChange( AsIMessage _msg)
	{
		Msg_PetNameChange change = _msg as Msg_PetNameChange;
		string name = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( change.name_));
		m_Entity.SetProperty( eComponentProperty.NAME, name);
		
		m_Entity.namePanel.SetPetName( name);
	}
	
	void OnPetSkillChangeResult( AsIMessage _msg)
	{
//		Msg_Pet_Skill_Change_Result result = _msg as Msg_Pet_Skill_Change_Result;
//		m_PetSkill[ (int)result.result_.eSkillType].nSkillTableIdx = result.result_.nSkillIdx;
	}
	
	void OnHatch( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnFeeding( AsIMessage _msg)
	{
		Msg_Pet_Feeding feed = _msg as Msg_Pet_Feeding;
		
		m_CurrentFsmState.MessageProcess( _msg);

		AsEffectManager.Instance.PlayEffect("FX/Effect/COMMON/Fx_Pet_Common", transform.position, false, 0f, 1f);
	}
	
	void OnPetScript( AsIMessage _msg)
	{
		if( m_PetOwner.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
			return;
		
		Msg_Pet_Script script = _msg as Msg_Pet_Script;
		StartCoroutine( _PetScript(script));
	}
	IEnumerator _PetScript( Msg_Pet_Script _script)
	{
		while(true)
		{
			yield return null;
			
			if( m_Entity.CheckModelLoadingState() == eModelLoadingState.Finished)
				break;
		}
		
		AsChatBalloonBase balloon = m_PetEntity.AddChatBalloon( AsChatFullPanel.Instance.chatBalloon, _script.word_);
		balloon.isNeedDelete = true;
	}
	
	void OnPetLevelUp( AsIMessage _msg)
	{
		Msg_Pet_LevelUp levelUp = _msg as Msg_Pet_LevelUp;

		AsEffectManager.Instance.PlayEffect("FX/Effect/COMMON/Fx_Pet_Common", transform.position, false, 0f, 1f);
		
		PetLevelUpDisplay();
		
//		if( levelUp.skillUp_ == true)
//			PetSkillUpDisplay();
	}
	
	
	void OnPetEvolution( AsIMessage _msg)
	{
		Msg_Pet_Evolution evo = _msg as Msg_Pet_Evolution;
		AsEntityManager.Instance.RemovePet( m_PetEntity);
		
		m_PetData.data_.nLevel_ = evo.nextLv_;
		
		m_PetOwner.HandleMessage( m_PetData);
		
//		AsEntityManager.Instance.PetAppear( m_PetData.data_);
	}
	
	void OnPetSkillGet( AsIMessage _msg)
	{
		Msg_Pet_Skill_Get skillGet = _msg as Msg_Pet_Skill_Get;
		m_PetSkill[ (int)skillGet.type_].nSkillTableIdx = skillGet.skill_.nSkillTableIdx;
		m_PetSkill[ (int)skillGet.type_].nLevel = skillGet.skill_.nLevel;
		m_PetSkill[ (int)skillGet.type_].nCoolTime = skillGet.skill_.nCoolTime;
		
		m_RemainCoolTime[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_ACTIVE] = skillGet.skill_.nCoolTime * 0.001f;
		
		PetSkillGetDisplay();
	}
	
	void OnPetPositionRefresh( AsIMessage _msg)
	{
		if( m_PetOwner != null)
		{
			Vector3 pos = m_PetOwner.GetRandomValidPosisionInRange( AsBaseEntity.s_PetRange);
			pos.y = TerrainMgr.GetTerrainHeight( pos);
			m_Entity.transform.position = pos;
			
			SetPetFsmState(ePetFsmStateType.IDLE);
		}
	}
	
	void OnPetHungryIndicate(AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess(_msg);
	}
	#endregion

	#region - public -
	public float GetOwnerMoveSpeed()
	{
		if( m_PetOwner != null)
		{
			return m_PetOwner.GetProperty<float>(eComponentProperty.MOVE_SPEED);
		}
		else
		{
			Debug.LogError( "AsPetFsm::GetOwnerMoveSpeed: there is no owner");
			return 1f;
		}
	}

	void CheckAbsoluteFollow_Inv()
	{
		if(PetOwner == null)
			return;

		Vector3 distVec = PetOwner.transform.position - transform.position;
		if( distVec.sqrMagnitude > s_sqrFollowOwnerRange)
		{
			transform.position = PetOwner.transform.position;
			Debug.LogWarning("AsPetFsm:: CheckAbsoluteFollow_Inv: pet's position is too much far away from owner. pet position is set as owner.");
		}
	}
	#endregion
	#region - method -
	#region - effect -
	bool m_PetLvUpEffStarted = false;
	GameObject m_PetLvUpEff;
	void PetLevelUpDisplay()
	{
		m_PetLvUpEff = Instantiate( Resources.Load( "UI/UI_effect/GamePlay/Prefab/PetLevelUp")) as GameObject;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6201_EFF_PetLevel", m_Entity.ModelObject.transform.position, false);

		UIPanel panel = m_PetLvUpEff.GetComponent<UIPanel>();
		panel.BringIn();

		StartCoroutine( PetLevelUpDisplay_CR());
	}
	IEnumerator PetLevelUpDisplay_CR()
	{
		m_PetLvUpEffStarted = true;
		
		yield return new WaitForSeconds( 4.0f);

		GameObject.DestroyImmediate( m_PetLvUpEff);
		
		m_PetLvUpEffStarted = false;
	}
	
	bool m_PetSkillUpEffStarted = false;
	GameObject m_PetSkillUpEff;
	void PetSkillUpDisplay()
	{
		StartCoroutine( PetSkillUpDisplay_CR());
	}
	IEnumerator PetSkillUpDisplay_CR()
	{
		m_PetSkillUpEffStarted = true;
		
		while(true)
		{
			yield return null;
			
			if( m_PetLvUpEffStarted == false && m_PetSkillGetEffStarted == false)
				break;
		}
		
		m_PetSkillUpEff = Instantiate( Resources.Load( "UI/UI_effect/GamePlay/Prefab/PetSkillUp")) as GameObject;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6201_EFF_PetSkill", m_Entity.ModelObject.transform.position, false);

		UIPanel panel = m_PetSkillUpEff.GetComponent<UIPanel>();
		panel.BringIn();
		
		yield return new WaitForSeconds( 4.0f);

		GameObject.DestroyImmediate( m_PetSkillUpEff);
		
		m_PetSkillUpEffStarted = false;
	}
	
	bool m_PetSkillGetEffStarted = false;
	GameObject m_PetSkillGetEff;
	void PetSkillGetDisplay()
	{
		StartCoroutine( PetSkillGetDisplay_CR());
	}
	IEnumerator PetSkillGetDisplay_CR()
	{
		m_PetSkillGetEffStarted = true;
		
		while(true)
		{
			yield return null;
			
			if( m_PetLvUpEffStarted == false)
				break;
		}
		
		m_PetSkillGetEff = Instantiate( Resources.Load( "UI/UI_effect/GamePlay/Prefab/PetGetSkill")) as GameObject;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6201_EFF_PetSkill", m_Entity.ModelObject.transform.position, false);

		UIPanel panel = m_PetSkillGetEff.GetComponent<UIPanel>();
		panel.BringIn();
		
		yield return new WaitForSeconds( 4.0f);

		GameObject.DestroyImmediate( m_PetSkillGetEff);
		
		m_PetSkillGetEffStarted = false;
	}
	
	Transform m_DummyLeadTop;
	void EffectUpdate()
	{
		if( m_PetLvUpEff != null)
			m_PetLvUpEff.transform.position = _EffectPosUpdate();
		
		if( m_PetSkillUpEff != null)
			m_PetSkillUpEff.transform.position = _EffectPosUpdate();
		
		if( m_PetSkillGetEff != null)
			m_PetSkillGetEff.transform.position = _EffectPosUpdate();
	}
	Vector3 _EffectPosUpdate()
	{
		if( m_DummyLeadTop == null)
			return Vector3.zero;
		
		Vector3 vScreenPos = CameraMgr.Instance.WorldToScreenPoint( m_DummyLeadTop.position);
		vScreenPos.y += 10f;
		Vector3 retPos = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		retPos.z += zPos;
		return retPos;
	}
	
	float zPos = 35f;
	#endregion
	#endregion

	#region - deledate -
	float GetCharacterSize()
	{
		return m_Character_Size;
	}
	#endregion
}