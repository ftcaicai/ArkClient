using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class AsPlayerFsm : AsBaseComponent
{
	#region - member -
	public enum ePlayerFsmStateType{NONE,
		CHOICE, EMPTY_SLOT, WAIT_DELETION, CUSTOMIZE,
		IDLE, IDLE_ACTION,
		RUN, RUN_NPC, RUN_OBJECT, RUN_OTHERUSER, RUN_COLLECT, WORK_COLLECT, READY_COLLECT,
		BATTLE_IDLE, BATTLE_RUN, DASH, JUMP, WAKE, HIT,
		CONDITION_STUN, CONDITION_SLEEP, CONDITION_FREEZE, CONDITION_FORCEDMOVE, CONDITION_FEAR, CONDITION_BINDING, CONDITION_AIRBONE, CONDITION_TRANSFORM,
		DEATH, LEVEL_UP,
		SKILL_READY, SKILL_HIT, SKILL_FINISH,
//		BASIC_ATTACK, COMMAND_SKILL_1, COMMAND_SKILL_2, ACTIVE_SKILL_1, ACTIVE_SKILL_2,//_CHARGE, ACTIVE_SKILL_2_CHARGE_ACTION,
		CHARACTERISTIC_SKILL,
		OBJ_TRAP, OBJ_STEPPING, PRODUCT,
		PRIVATESHOP
	}

	[SerializeField]
	ePlayerFsmStateType m_CurState;

	AsUserEntity m_UserEntity;public AsUserEntity UserEntity{get{return m_UserEntity;}}

	protected Dictionary<ePlayerFsmStateType, AsBaseFsmState<ePlayerFsmStateType, AsPlayerFsm>> m_dicFsmState =
		new Dictionary<ePlayerFsmStateType, AsBaseFsmState<ePlayerFsmStateType, AsPlayerFsm>>();
	protected AsBaseFsmState<ePlayerFsmStateType, AsPlayerFsm> m_CurrentFsmState;
	public ePlayerFsmStateType CurrnetFsmStateType{get{return m_CurrentFsmState.FsmStateType;}}
	protected AsBaseFsmState<ePlayerFsmStateType, AsPlayerFsm> m_OldFsmState;
	public ePlayerFsmStateType OldFsmStateType{get{return m_OldFsmState.FsmStateType;}}

	bool m_TargetForQuestItem = false;
	AsBaseEntity m_Target = null;


	public bool TargetForQuest
	{
		get { return m_TargetForQuestItem; }
	}

	public AsBaseEntity Target
	{
		get	{ return m_Target; }
		set
		{
			m_Target = value;
			m_TargetForQuestItem = false;
			AsHUDController.Instance.SetTargetInfo( m_Target);
			
			#region - auto combat -
			AutoCombatManager.Instance.TargetChangeProcess(this);
			#endregion
		}
	}

	public float targetDistance
	{
		get
		{
			if( m_Target != null)
				return Vector3.Distance( m_Target.transform.position, transform.position);
			else
				return 0;
		}
	}

	public bool WeaponEquip
	{
		get
		{
			if( m_UserEntity.getCharView[0].nItemTableIdx == 0)
				return false;
			else
				return true;
		}
	}

	float m_ElapsedCoolTime_BaseAttack;
	public float ElapsedCoolTime_BaseAttack
	{
		get	{ return m_ElapsedCoolTime_BaseAttack; }
		set	{ m_ElapsedCoolTime_BaseAttack = value; }
	}

//	Msg_Action_Kept m_KeptMessage;
//	public Msg_Action_Kept KeptMessage{
//		get{
//			Msg_Action_Kept temp = m_KeptMessage;
//			m_KeptMessage = null;
//			return temp;
//		}
//		set{
//			m_KeptMessage = value;
//		}
//	}

	[SerializeField]
	Tbl_Action_Record m_CurrentAction;
	public Tbl_Action_Record CurrentAction
	{
		get	{ return m_CurrentAction; }
		set	{ m_CurrentAction = value; }
	}

	//HIT LATENCY( for sync)
	Msg_OtherCharAttackNpc1 m_AttackSC;
//	Msg_HitPotency m_HitPotency;

	//fx
	ElementProcessor m_ElementProcessor;

	//etc
	[SerializeField]
	float m_IdleActionCycle = 10f;
	public float IdleActionCycle{get{return m_IdleActionCycle;}}
	[SerializeField]
	float m_IdleActionRevision = 2f;
	public float IdleActionRevision{get{return m_IdleActionRevision;}}

	//debug
	public bool showSendingPacket = false;

	private bool m_bUseCoolTime = true;
	public bool bUseCoolTime { set{ m_bUseCoolTime = value;} get{ return m_bUseCoolTime;}}

	float m_MonsterReleaseDistance = 12;
	float m_UserReleaseDistance = 20;
	float m_NpcReleaseDistance = 10;

	public float UserReleaseDistance
	{
		get { return m_UserReleaseDistance; }
	}

	Vector3 m_InitSize = Vector3.one;
	float m_Character_Size = 1f; public float Character_Size{get{return m_Character_Size;}}
	
	//pet
	AsNpcEntity m_Pet;
	#endregion

	#region - init & release -
	void Awake()
	{
		m_ComponentType = eComponentType.FSM_PLAYER;

		m_UserEntity = GetComponent<AsUserEntity>();
		if( m_UserEntity == null) Debug.LogError( "AsPlayerFsm::Init: no user entity attached");
		m_UserEntity.FsmType = eFsmType.PLAYER;

		m_MonsterReleaseDistance = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 27).Value * 0.01f;
		m_UserReleaseDistance = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 28).Value * 0.01f;
		m_NpcReleaseDistance = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 29).Value * 0.01f;
		
		#region - msg -
		MsgRegistry.RegisterFunction( eMessageType.LEVEL_UP, OnLevelUp);

		MsgRegistry.RegisterFunction( eMessageType.CHOICE, OnChoice);
		MsgRegistry.RegisterFunction( eMessageType.ZONE_WARP, OnZoneWarp);
		MsgRegistry.RegisterFunction( eMessageType.ZONE_WARP_END, OnZoneWarpEnd);

		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED_DUMMY, OnModelLoaded_Dummy);

//		MsgRegistry.RegisterFunction( eMessageType.INPUT, OnInput);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_RUB, OnRubMove);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_MOVE, OnInputMove);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_AUTO_MOVE, OnInputMove);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_ATTACK, OnInputAttack);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_END_INFORM, OnMoveEndInform);
		MsgRegistry.RegisterFunction( eMessageType.JUMP_STOP, OnJumpStop);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_ATTACK_NPC1, OnAttackSC);
		MsgRegistry.RegisterFunction( eMessageType.NPC_ATTACK_CHAR2, OnDamaged);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_ATTACK_NPC3, OnAffected);
		
		//PET
		MsgRegistry.RegisterFunction( eMessageType.PET_DATA_INDICATE, OnPetDataIndicate);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_READY, OnPetSkillReady);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_RESULT, OnPetSkillResult);
		MsgRegistry.RegisterFunction( eMessageType.PET_NAME_CHANGE, OnPetNameChange);
		MsgRegistry.RegisterFunction( eMessageType.PET_DELETE, OnPetDelete);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_CHANGE_RESULT, OnPetSkillChangeResult);
		MsgRegistry.RegisterFunction( eMessageType.PET_FEEDING, OnPetFeeding);
		MsgRegistry.RegisterFunction( eMessageType.PET_SCRIPT, OnPetScript);
		MsgRegistry.RegisterFunction( eMessageType.PET_LEVELUP, OnPetLevelUp);
		MsgRegistry.RegisterFunction( eMessageType.PET_EVOLUTION, OnPetEvolution);
		MsgRegistry.RegisterFunction( eMessageType.PET_ITEM_VIEW, OnItemView);
		MsgRegistry.RegisterFunction( eMessageType.PET_SKILL_GET, OnPetSkillGet);
		MsgRegistry.RegisterFunction( eMessageType.PET_POSITION_REFRESH, OnPetPositionRefresh);
		MsgRegistry.RegisterFunction( eMessageType.PET_HUNGRY_INDICATE, OnPetHungryIndicate);
		MsgRegistry.RegisterFunction( eMessageType.PET_EFFECT_INDICATE, OnPetEffectIndicate);

		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_END, OnAnimationEnd);

		MsgRegistry.RegisterFunction( eMessageType.RELEASE_TENSION, OnReleaseTension);

		//REFLECTION REFRESH
		MsgRegistry.RegisterFunction( eMessageType.REFLECTION_REFRESH, OnReflectionRefresh);

		//HIT EXECUTION
		MsgRegistry.RegisterFunction( eMessageType.HIT_EXECUTION, OnHitExecution);

		//SKILL
		MsgRegistry.RegisterFunction( eMessageType.INPUT_BEGIN_CHARGE, OnBeginCharge);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_CANCEL_CHARGE, OnCancelCharge);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_SLOT_ACTIVE, OnSkillActive);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_DRAG_STRAIGHT, OnSkillDragStraight);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_ARC, OnSkillArc);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_CIRCLE, OnSkillCircle);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_DOUBLE_TAB, OnSkillDoubleTap);
		MsgRegistry.RegisterFunction( eMessageType.PLAYER_USE_ACTIONITEM, OnActionItem);
		MsgRegistry.RegisterFunction( eMessageType.OTHER_CHAR_SKILL_STANCE, OnStance);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_BUFF_ACCURE, OnBuffAccure);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_SKILL_ADD, OnAdd);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_SKILL_SOULSTONE, OnSoulStone);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_SKILL_PVP_AGGRO, OnPvpAggro);
		MsgRegistry.RegisterFunction( eMessageType.TARGET_INDICATION, OnTargetIndication);

		MsgRegistry.RegisterFunction( eMessageType.SKILL_CHARGE_COMPLETE, OnCommandSkillCoolTimeComplete);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_SHIELD_BEGIN, OnSkillShieldBegin);
		MsgRegistry.RegisterFunction( eMessageType.INPUT_SHIELD_END, OnSkillShieldEnd);

		MsgRegistry.RegisterFunction( eMessageType.PLAYER_SKILL_LEARN, OnSkillLearn);

		//FORCED MOVEMENT
		MsgRegistry.RegisterFunction( eMessageType.FORCED_MOVEMENT_RESULT, OnForcedMovementResult);//obsolete
//		MsgRegistry.RegisterFunction( eMessageType.FORCEDMOVE_USER_INDICATION, OnForcedMoveIndicate);

		//OBJECT ACTION
		MsgRegistry.RegisterFunction( eMessageType.OBJ_TRAP_MSG, OnObjTrapAction);
		MsgRegistry.RegisterFunction( eMessageType.OBJ_STEPPING_MSG, OnObjSteppingAction);

		//CLICK
		MsgRegistry.RegisterFunction( eMessageType.PLAYER_CLICK, OnPlayerClick);

		//NPC
		MsgRegistry.RegisterFunction( eMessageType.NPC_CLICK, OnNpcClick);
		MsgRegistry.RegisterFunction( eMessageType.COLLECT_CLICK, OnCollectClick);
		MsgRegistry.RegisterFunction( eMessageType.OBJECT_CLICK, OnObjectClick);
		MsgRegistry.RegisterFunction( eMessageType.COLLECT_INFO, OnCollectInfo);
		MsgRegistry.RegisterFunction( eMessageType.COLLECT_RESULT, OnCollectResult);//obsolete

		//ACT
		MsgRegistry.RegisterFunction( eMessageType.INPUT_DASH, OnInputDash);

		//SKILL EFFECT
		MsgRegistry.RegisterFunction( eMessageType.OTHER_CHAR_SKILL_EFFECT, OnSkillEffect);

		//BUFF
		MsgRegistry.RegisterFunction( eMessageType.CHAR_BUFF, OnBuff);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_DEBUFF, OnDeBuff);
		MsgRegistry.RegisterFunction( eMessageType.BUFF_REFRESH, OnBuffRefresh);
		MsgRegistry.RegisterFunction( eMessageType.DEATH_DEBUFF_CLEAR, OnDeathDebuffClear);
		MsgRegistry.RegisterFunction( eMessageType.CHAR_DEBUFF_RESIST, OnDebuffResist);
		MsgRegistry.RegisterFunction( eMessageType.DEATH_PENALTY_INDICATE, OnDeathPenalty);

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
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_BINDING, OnConditionBinding);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_BINDING, OnRecoverConditionBinding);

		MsgRegistry.RegisterFunction( eMessageType.CONDITION_FORCEDMOVE, OnConditionForcedMove);
		MsgRegistry.RegisterFunction( eMessageType.FORCEDMOVE_SYNC, OnForcedMoveSync);
		
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_BLANK, OnConditionBlank);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_BLANK, OnRecoverConditionBlank);
		
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_AIRBONE, OnConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_AIRBONE, OnRecoverConditionAirBone);
		MsgRegistry.RegisterFunction( eMessageType.CONDITION_TRANSFORM, OnConditionTransform);
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_CONDITION_TRANSFORM, OnRecoverConditionTransform);

		// CHEAT
		MsgRegistry.RegisterFunction( eMessageType.CHEAT_DEATH, OnCheatDeath);

		// mob casting
		MsgRegistry.RegisterFunction( eMessageType.MOB_CASTING, OnMobCasting);

		MsgRegistry.RegisterFunction( eMessageType.CHAR_RESURRECTION, OnResurrection);

		MsgRegistry.RegisterFunction( eMessageType.OTHER_USER_CLICK, OnOtherUserClick);

		//exception
		MsgRegistry.RegisterFunction( eMessageType.RECOVER_STATE, OnRecoverState);

		//death by other reason( ex. buff)
		MsgRegistry.RegisterFunction( eMessageType.DEATH_INDICATION, OnDeath);

		//private shop
		MsgRegistry.RegisterFunction( eMessageType.OPEN_PRIVATESHOP, OnOpenPrivateShop);
		MsgRegistry.RegisterFunction( eMessageType.CLOSE_PRIVATESHOP, OnClosePrivateShop);

		//emotion
		MsgRegistry.RegisterFunction( eMessageType.EMOTION_INDICATION, OnEmotionIndicate);
		MsgRegistry.RegisterFunction( eMessageType.EMOTICON_SEAT_INDICATION, OnEmotionSeatIndicate);
		MsgRegistry.RegisterFunction( eMessageType.BALLOON, OnBalloonIndicate);

		MsgRegistry.RegisterFunction( eMessageType.PRODUCT, OnProduct);
		
		// AUTO COMBAT
		MsgRegistry.RegisterFunction( eMessageType.AUTOCOMBAT_ON, OnAutoCombatOn);
		MsgRegistry.RegisterFunction( eMessageType.AUTOCOMBAT_OFF, OnAutoCombatOff);
		#endregion
		#region - state -
		m_dicFsmState.Add( ePlayerFsmStateType.CHOICE, new AsPlayerState_Choice( this));
//		m_dicFsmState.Add( ePlayerFsmStateType.STANDBY, new AsPlayerState_Standby( this));

		m_dicFsmState.Add( ePlayerFsmStateType.IDLE, new AsPlayerState_Idle( this));
		m_dicFsmState.Add( ePlayerFsmStateType.RUN, new AsPlayerState_Run( this));
		m_dicFsmState.Add( ePlayerFsmStateType.RUN_OTHERUSER, new AsPlayerState_Run_OtherUser( this));
		m_dicFsmState.Add( ePlayerFsmStateType.RUN_NPC, new AsPlayerState_Run_Npc( this));
		m_dicFsmState.Add( ePlayerFsmStateType.RUN_OBJECT, new AsPlayerState_Run_Object( this));

		m_dicFsmState.Add( ePlayerFsmStateType.BATTLE_IDLE, new AsPlayerState_BattleIdle( this));
		m_dicFsmState.Add( ePlayerFsmStateType.BATTLE_RUN, new AsPlayerState_BattleRun( this));
//		m_dicFsmState.Add( ePlayerFsmStateType.DASH, new AsPlayerState_Dash( this));
//		m_dicFsmState.Add( ePlayerFsmStateType.RECOVER, new AsPlayerState_Recover( this));
		m_dicFsmState.Add( ePlayerFsmStateType.WAKE, new AsPlayerState_Wake( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_STUN, new AsPlayerState_Condition_Stun( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_FREEZE, new AsPlayerState_Condition_Freeze( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_SLEEP, new AsPlayerState_Condition_Sleep( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_FORCEDMOVE, new AsPlayerState_Condition_ForcedMove( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_FEAR, new AsPlayerState_Condition_Fear( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_BINDING, new AsPlayerState_Condition_Bind( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_AIRBONE, new AsPlayerState_Condition_AirBone( this));
		m_dicFsmState.Add( ePlayerFsmStateType.CONDITION_TRANSFORM, new AsPlayerState_Condition_Transform( this));

		m_dicFsmState.Add( ePlayerFsmStateType.DEATH, new AsPlayerState_Death( this));
//		m_dicFsmState.Add( ePlayerFsmStateType.LEVEL_UP, new AsPlayerState_LevelUp( this));

		m_dicFsmState.Add( ePlayerFsmStateType.SKILL_READY, new AsPlayerState_SkillReady( this));
		m_dicFsmState.Add( ePlayerFsmStateType.SKILL_HIT, new AsPlayerState_SkillHit( this));
		m_dicFsmState.Add( ePlayerFsmStateType.SKILL_FINISH, new AsPlayerState_SkillFinish( this));
		m_dicFsmState.Add( ePlayerFsmStateType.OBJ_STEPPING, new AsPlayerState_ObjStepping( this));

		m_dicFsmState.Add( ePlayerFsmStateType.PRIVATESHOP, new AsPlayerState_PrivateShop( this));

		m_dicFsmState.Add( ePlayerFsmStateType.RUN_COLLECT, new AsPlayerState_Run_Collect( this));
		m_dicFsmState.Add( ePlayerFsmStateType.WORK_COLLECT, new AsPlayerState_Work_Collect( this));
		m_dicFsmState.Add( ePlayerFsmStateType.READY_COLLECT, new AsplayerState_Ready_Collect( this));
		m_dicFsmState.Add( ePlayerFsmStateType.PRODUCT, new AsPlayerState_Product( this));
		#endregion
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);
		
		m_InitSize = m_Entity.transform.localScale;

		m_ElementProcessor = new ElementProcessor( m_Entity);
		m_Entity.SetConditionCheckDelegate( m_ElementProcessor.GetEnableCondition);
		m_Entity.SetBuffCheckDelegate( m_ElementProcessor.CheckBuffInclusion);
		m_Entity.SetTargetShopOpening( TargetShopOpening);

		gameObject.layer = LayerMask.NameToLayer( "Player");
	}

	public override void InterInit( AsBaseEntity _entity)
	{
//		if( m_Entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == false)
//		{
			m_dicFsmState[ePlayerFsmStateType.CHOICE].Init();

			m_dicFsmState[ePlayerFsmStateType.IDLE].Init();
			m_dicFsmState[ePlayerFsmStateType.RUN].Init();
			m_dicFsmState[ePlayerFsmStateType.RUN_OTHERUSER].Init();
			m_dicFsmState[ePlayerFsmStateType.RUN_NPC].Init();
			m_dicFsmState[ePlayerFsmStateType.RUN_OBJECT].Init();

			m_dicFsmState[ePlayerFsmStateType.BATTLE_IDLE].Init();
			m_dicFsmState[ePlayerFsmStateType.BATTLE_RUN].Init();
			m_dicFsmState[ePlayerFsmStateType.WAKE].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_STUN].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_FREEZE].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_SLEEP].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_FORCEDMOVE].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_AIRBONE].Init();
			m_dicFsmState[ePlayerFsmStateType.CONDITION_TRANSFORM].Init();
			m_dicFsmState[ePlayerFsmStateType.DEATH].Init();

			m_dicFsmState[ePlayerFsmStateType.SKILL_READY].Init();
			m_dicFsmState[ePlayerFsmStateType.SKILL_HIT].Init();
			m_dicFsmState[ePlayerFsmStateType.SKILL_FINISH].Init();
			m_dicFsmState[ePlayerFsmStateType.OBJ_STEPPING].Init();

			m_dicFsmState[ePlayerFsmStateType.PRIVATESHOP].Init();

			m_dicFsmState[ePlayerFsmStateType.RUN_COLLECT].Init();
			m_dicFsmState[ePlayerFsmStateType.WORK_COLLECT].Init();
			m_dicFsmState[ePlayerFsmStateType.READY_COLLECT].Init();
			m_dicFsmState[ePlayerFsmStateType.PRODUCT].Init();
//		}
	}

	public override void LateInit( AsBaseEntity _entity)
	{
		SetPlayerFsmState( ePlayerFsmStateType.IDLE);

//		if( m_Entity.AnimEnableViaShop == true)
//		{
//			SetPlayerFsmState( ePlayerFsmStateType.IDLE);
//		}
//		else
//		{
//			SetPlayerFsmState( ePlayerFsmStateType.PRIVATESHOP);
//		}
	}

	public override void LastInit( AsBaseEntity _entity)
	{
		
	}

	void Start()
	{
		AsPStoreManager.Instance.PlayerLoaded();
		
		topInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
		secondInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
		sendedInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
	}

	void OnDisable()
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.Exit();
		
		if( m_Pet != null)
		{
			AsEntityManager.Instance.RemovePet( m_Pet);
		}
	}

//	void OnDestroy()
//	{
//		AsUserInfo.Instance.dicBuff.Clear();
//	}
	#endregion

	#region - fsm -
	public void SetPlayerFsmState( ePlayerFsmStateType _type, AsIMessage _msg)
	{

//		Debug.LogWarning( "SetPlayerFsmState: same state = " + _type + "old: " + m_OldFsmState + "cur : " + m_CurState);


		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == _type)
			{
				Debug.LogWarning( "AsPlayerFsm::SetPlayerFsmState: same state = " + _type);
				return;
			}

			m_CurrentFsmState.Exit();
			m_OldFsmState = m_CurrentFsmState;
		}
//		else
//			Debug.LogWarning( "[AsBaseFsm]SetFsmState: current state");

		if( m_dicFsmState.ContainsKey( _type) == true)
		{
			m_CurState = _type;
			m_CurrentFsmState = m_dicFsmState[_type];
			m_CurrentFsmState.Enter( _msg);

//			switch( _type)
//			{
//			case ePlayerFsmStateType.IDLE:
//				StartCoroutine( "BeginIdleChecking");
//				break;
//			}
		}
		else
			Debug.LogError( "[AsBaseFsm]SetFsmState: not registered state : [" + _type + "]");
	}
	public void SetPlayerFsmState( ePlayerFsmStateType _type)
	{
		SetPlayerFsmState( _type, null);
	}
	#endregion

	#region - update & override -
	void Update()
	{
		m_CurrentFsmState.Update();
		CoolTimeProcess_BaseAttack();
		CheckDistanceBetweenTarget();
		m_ElementProcessor.Update();

		if( ( null == m_Target) && ( true == AsHUDController.Instance.targetInfo.Enable))
			AsHUDController.Instance.SetTargetInfo( null);
			//dpamin
			//AsHUDController.Instance.targetInfo.Enable = false;

//		if( Time.deltaTime > 0.04f)
//		{
//			Debug.LogWarning( "AsPlayerFsm::Update: elapsed time is " + Time.deltaTime);
//			Debug.LogWarning( "AsPlayerFsm::Update: state is " + m_CurState);
//			Debug.LogWarning( "AsPlayerFsm::Update: target is " + m_Target);
//		}
	}

	public void UpdateTargetInfoByOtherUser( Msg_OtherCharAttackNpc2 attack)
	{
		AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( attack.npcIdx_);
		if( null == npcEntity)
			return;

		if( m_Target != npcEntity)
			return;

		if( false == AsHUDController.Instance.UpdateTargetSimpleInfo( m_Target))
			Target = null;
	}

	void CoolTimeProcess_BaseAttack()
	{
		if( m_ElapsedCoolTime_BaseAttack > 0)
			m_ElapsedCoolTime_BaseAttack -= Time.deltaTime;
		else if( m_ElapsedCoolTime_BaseAttack < 0)
			m_ElapsedCoolTime_BaseAttack = 0;
	}

	void CheckDistanceBetweenTarget()
	{
		if( m_Target != null)
		{
			switch( m_Target.FsmType)
			{
			case eFsmType.MONSTER:
				if( Vector3.Distance( transform.position, Target.transform.position) > m_MonsterReleaseDistance)
					Target = null;
				break;
			case eFsmType.NPC:
			case eFsmType.OBJECT:
			case eFsmType.COLLECTION:
				if( m_CurState != ePlayerFsmStateType.BATTLE_RUN &&
					m_CurState != ePlayerFsmStateType.RUN_NPC &&
					m_CurState != ePlayerFsmStateType.IDLE &&
					m_CurState != ePlayerFsmStateType.SKILL_HIT &&
					m_CurState != ePlayerFsmStateType.SKILL_FINISH &&
					m_CurState != ePlayerFsmStateType.RUN_COLLECT
					)
				{
					if( Vector3.Distance( transform.position, Target.transform.position) > m_NpcReleaseDistance)
						Target = null;
				}
				break;
			case eFsmType.OTHER_USER:
				if( Vector3.Distance( transform.position, Target.transform.position) > m_UserReleaseDistance)
					Target = null;
				break;
			}
		}
	}

//	void OnApplicationPause( bool _pause)
//	{
//		if( m_CurrentFsmState != null)
//			m_CurrentFsmState.MessageProcess( new Msg_ApplicationPause( true));
//	}
	#endregion

	//PUBLIC
	#region - check hostile existence in range -
	public AsNpcEntity MonsterExistInRange( float _range)
	{
		AsNpcEntity monster;
		float dist = AsEntityManager.Instance.GetNearestMonsterExceptObject( transform.position, out monster);
		if( dist < _range)
			return monster;

		return null;
	}

	public AsBaseEntity HostileEntityExistInRange( float _range)
	{
		AsBaseEntity entity;
		float dist = AsEntityManager.Instance.GetNearestHostileEntity( transform.position, out entity);
		if( dist < _range)
			return entity;

		return null;
	}
	
	public SuitableBasicSkill GetSuitableBasicSkill( float _dist)
	{
		eCLASS __class = m_Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		return AsTableManager.Instance.GetSuitableBasicSkill( __class, _dist);
	}
	#endregion

	#region - cool time -
	public void BeginCommandSkillCoolDown( COMMAND_SKILL_TYPE _type)
	{
		if( null == AsSkillDelegatorManager.Instance)
			return;

		if( false == m_bUseCoolTime)
			return;

		AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)_type] = false;
		AsSkillDelegatorManager.Instance.AddDelegator( _type);
	}
	#endregion

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

	public void ReleaseElements( bool _canceled)
	{
		if( _canceled == false)
			m_ElementProcessor.Release( false);
		else
			m_ElementProcessor.Release( true);
	}

//	public void CancelElements()
//	{
//		m_ElementProcessor.Cancel();
//	}

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

	//PRIVATE
	public bool CheckMap_Village()
	{
		Map map = TerrainMgr.Instance.GetCurrentMap();
		eMAP_TYPE mapType = map.MapData.getMapType;
		switch( mapType)
		{
		case eMAP_TYPE.Field:
		case eMAP_TYPE.Tutorial:
		case eMAP_TYPE.Indun:
			return false;
		case eMAP_TYPE.Town:
			return true;
		}

		return false;
	}

	bool CheckSpecificWindowOpened()
	{
		if( AsSocialManager.Instance.SocialUI.IsOpenSocailStoreDlg == true ||
			AsHudDlgMgr.Instance.IsOpenedPostBox == true ||
			AsHudDlgMgr.Instance.IsOpenTrade == true ||
			AsHudDlgMgr.Instance.IsOpenEnchantDlg == true ||
			AsHudDlgMgr.Instance.IsOpenCashStore == true)
			return true;

		return false;
	}

	bool CheckSkillMovable( Msg_Player_Skill_Ready _ready)
	{
		Tbl_Action_HitAnimation hitAnim = _ready.actionRecord.HitAnimation;
		if( hitAnim == null)
			return true;

		switch( hitAnim.MoveType)
		{
		case eHitMoveType.Dash:
		case eHitMoveType.TargetDash:
		case eHitMoveType.TabDash:
			float moveDist = hitAnim.MoveDistance * 0.01f;
			Vector3 dir = Vector3.zero;

			switch( _ready.skillRecord.Attack_Direction)
			{
			case eATTACK_DIRECTION.Camera:
				dir = Camera.mainCamera.transform.position;
				break;
			case eATTACK_DIRECTION.FingerPoint:
			case eATTACK_DIRECTION.LineEnd:
				dir = GetWorldPositionFromCamera( _ready.tail);
				break;
			case eATTACK_DIRECTION.LineMiddle:
				dir = GetWorldPositionFromCamera( _ready.center);
				break;
			case eATTACK_DIRECTION.LineStart:
				dir = GetWorldPositionFromCamera( _ready.head);
				break;
			case eATTACK_DIRECTION.Target:
				if( Target != null)
					dir = Target.transform.position;
				else
					return false;
				break;
			default:
				return true;
			}

			dir = dir - transform.position; dir.Normalize();
			Vector3 dest = dir * moveDist + transform.position;
			float dist = m_Entity.GetNavPathDistance( transform.position, dest, true);
			if( dist > moveDist + 0.1f) // revision
				return false;
			else
				return true;
		default:
			return true;
		}
	}

	Vector3 GetWorldPositionFromCamera( Vector3 _screenPos)
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay( _screenPos);
		RaycastHit hit = new RaycastHit();
		if( Physics.Raycast( ray, out hit) == true)
		{
			return hit.point;
		}

		return Vector3.zero;
	}

	// msg -------------------------------------------------------------------------------
	//////////////////////////////////////////////////////////////////////////////////////

	//BASIC//
	#region - model loaded -
	void OnModelLoaded( AsIMessage _msg)
	{
		m_ElementProcessor.ModelLoaded();		
		m_ElementProcessor.PlayerModelLoaded();
		
		PlayAction_Idle();
				
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
	
		// if( AsGameMain.s_gameState != GAME_STATE.STATE_INGAME &&	
		// AsGameMain.s_gameState != GAME_STATE.STATE_LOADING)		
		// {		
		// Debug.Log( "AsPlayerFsm::OnModelLoaded: AsGameMain.s_gameState == " + AsGameMain.s_gameState);		
		// return;		
		// }
		
		float hp = m_Entity.GetProperty<float>( eComponentProperty.HP_CUR);	
		bool living = m_Entity.GetProperty<bool>( eComponentProperty.LIVING);
	
		//debug		
		Debug.Log( "AsPlayerFsm::OnModelLoaded: hp = " + hp);		
		Debug.Log( "AsPlayerFsm::OnModelLoaded: living = " + living);
				
		if( hp < 1 || living == false)		
		{
			if( TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Pvp) == false)
			{
				Debug.Log( "AsPlayerFsm::OnModelLoaded: AsGameMain.s_gameState == " + AsGameMain.s_gameState);			
				AsNotify.Instance.DeathDlg();			
			}
			
			if( m_CurrentFsmState == null || m_CurrentFsmState.FsmStateType != ePlayerFsmStateType.DEATH)		
				SetPlayerFsmState( ePlayerFsmStateType.DEATH, new Msg_EnterWorld_Death( true));		
			else		
				m_CurrentFsmState.MessageProcess( new Msg_EnterWorld_Death( true));		
		}		
		else
		{		
			if( m_CurrentFsmState != null)		
				m_CurrentFsmState.MessageProcess( _msg);		
		}
	}

	
	void OnModelLoaded_Dummy( AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - level up -
	Msg_Level_Up m_LevelUp = null;
	void OnLevelUp( AsIMessage _msg)
	{
		m_LevelUp = _msg as Msg_Level_Up;
		LevelUpExecution();
		BonusManager.Instance.PlayerLevelUp(m_LevelUp);
//		AsHudDlgMgr.Instance.PlayerLevelUp();
	}

	void LevelUpExecution()
	{
		m_Entity.SetProperty( eComponentProperty.LEVEL, m_LevelUp.data_.nLevel);
//		m_Entity.SetProperty( eComponentProperty.HP_CUR, m_LevelUp.data_.fHpCur);
		m_Entity.SetProperty( eComponentProperty.HP_CUR, m_LevelUp.data_.sFinalStatus.fHPMax);
		m_Entity.SetProperty( eComponentProperty.HP_MAX, m_LevelUp.data_.sFinalStatus.fHPMax);
//		m_Entity.SetProperty( eComponentProperty.MP_CUR, m_LevelUp.data_.fMpCur);
		m_Entity.SetProperty( eComponentProperty.MP_CUR, m_LevelUp.data_.sFinalStatus.fMPMax);
		m_Entity.SetProperty( eComponentProperty.MP_MAX, m_LevelUp.data_.sFinalStatus.fMPMax);
//		m_Entity.SetProperty( eComponentProperty.ATTACK, m_LevelUp.data_.sFinalStatus.nAttr_Dmg);
		m_Entity.SetProperty( eComponentProperty.DEFENCE, m_LevelUp.data_.sFinalStatus.nPhysic_Def);
		
		AsUserInfo.Instance.SavedCharStat.level_ = m_LevelUp.data_.nLevel;
		
		AsUserInfo.Instance.SaveCurCharStat( m_LevelUp.data_);
//		AsUserInfo.Instance.SavedCharStat.hpCur_ = m_LevelUp.data_.fHpCur;
//		AsUserInfo.Instance.SavedCharStat.mpCur_ = m_LevelUp.data_.fMpCur;

		AsMyProperty.Instance.LevelUpDisplay();

		if( null != UserEntity.ModelObject)
		{
			AsEffectManager.Instance.PlayEffect( "FX/Effect/Common/Fx_Common_LevelUp",UserEntity.ModelObject.transform,false,0f);
			AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_Levelup_Eff", UserEntity.ModelObject.transform.position, false);
		}

		if( null != AsHudDlgMgr.Instance)
		{
			if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				AsHudDlgMgr.Instance.playerStatusDlg.ResetPageText();
		}

		ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();

		SkillBook.Instance.LevelUpProcess();
		AsUserInfo.Instance.SendLevelUpActiveWaypoint( m_LevelUp.data_.nLevel);
		AsEventNotifyMgr.Instance.LevelUpNotify.LevelUpProcess();
		AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eLevel, m_LevelUp.data_.nLevel);
		m_LevelUp = null;
	}
	#endregion

	#region - choice -
	void OnChoice( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CHOICE);
	}
	#endregion

	#region - zone warp -
//	bool m_ZoneWarping = false;
	void OnZoneWarp( AsIMessage _msg)
	{
		StopCoroutine( "RefreshDelay");
	}

	void OnZoneWarpEnd( AsIMessage _msg)
	{
		topInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
		secondInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
		sendedInfo = new SavedMoveInfo( eMoveType.Sync_Stop, transform.position, transform.position);
	}
	#endregion

	public void SendProductProgressCancel()
	{
		AsCommonSender.SendItemProductProgress( false);
	}
	
	//MOVE
	#region - input -
	void OnInputMove( AsIMessage _msg)
	{
		if( CheckSpecificWindowOpened() == true)
			return;

		if( true == AsUserInfo.Instance.isProductionProgress)
		{
			string strTitle = AsTableManager.Instance.GetTbl_String(126);
			string strText = AsTableManager.Instance.GetTbl_String(268);

			if( true != AsHudDlgMgr.Instance.isOpenMsgBox)
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( strTitle, strText, this, "SendProductProgressCancel",
					AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
			}
			AsInputManager.Instance.BlockHoldMove();
			return;
		}

		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState)
			|| AsInputManager.Instance.m_Activate == false)
//		if( null != AsHudDlgMgr.Instance && ( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg))
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnInputAttack( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState)
			|| AsInputManager.Instance.m_Activate == false)
//		if( null != AsHudDlgMgr.Instance && ( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg))
			return;

		if( CheckMap_Village() == true)
		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsMyProperty.Instance.AlertSkillInTown();
			return;
		}

		if( WeaponEquip == true)
			m_CurrentFsmState.MessageProcess( _msg);
		else
			AsMyProperty.Instance.AlertNoWeapon();
	}
	#endregion
	#region - move end -
	void OnMoveEndInform( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion	
	#region - movement synchronize -
//	eMoveType moveType = eMoveType.Normal;
//	Vector3 inputDest = Vector3.zero;
//	Vector3 curPos = Vector3.zero;
	
	float sqrMaxMoveRange = 144f;
	
	SavedMoveInfo topInfo;
	SavedMoveInfo secondInfo;
	SavedMoveInfo sendedInfo;
	
	bool enableRefresh = true;
	float refreshRate = 0.5f;
	
	public void MovePacketSynchronize()
	{
		if( CheckWarpConfirmed() == true)
		{
//			Debug.LogError("AsPlayerFsm::MovePacketSynchronize: trying send move packet");
			return;
		}
		
		StopCoroutine( "RefreshDelay");

		enableRefresh = true;

//		Debug.Log( "RefreshDelay: curPos = " + curPos + ", destPos = " + inputDest + ", dist = " + Vector3.Distance( curPos, inputDest));
		
		StackMovePacket( eMoveType.Sync_Stop, transform.position, transform.position);
		SendingPacket();
	}

	public void MovePacketSynchronize_Skill()
	{
		if( CheckWarpConfirmed() == true)
		{
//			Debug.LogError("AsPlayerFsm::MovePacketSynchronize: trying send move packet");
			return;
		}
		
		StopCoroutine( "RefreshDelay");

		enableRefresh = true;

//		Debug.Log( "RefreshDelay: curPos = " + curPos + ", destPos = " + inputDest + ", dist = " + Vector3.Distance( curPos, inputDest));

		StackMovePacket( eMoveType.Sync_Stop, transform.position, transform.position);
		SendingPacket();
	}

	public void MovePacketRefresh( eMoveType _type, Vector3 _curPos, Vector3 _destPos)
	{
		if( CheckWarpConfirmed() == true)
		{
//			Debug.LogError("AsPlayerFsm::MovePacketSynchronize: trying send move packet");
			return;
		}
		
		StackMovePacket( _type, _curPos, _destPos);

//		Debug.Log( "MovePacketRefresh: curPos = " + _curPos + ", destPos = " + _destPos + ", dist = " + Vector3.Distance( _curPos, _destPos));
		if( enableRefresh == true)
		{
			SendingPacket();
			StartCoroutine( "RefreshDelay", refreshRate);
		}
	}

	IEnumerator RefreshDelay( float _time)
	{
		enableRefresh = false;

		yield return new WaitForSeconds( _time);

		enableRefresh = true;

//		Debug.Log( "RefreshDelay: curPos = " + curPos + ", destPos = " + inputDest + ", dist = " + Vector3.Distance( curPos, inputDest));	
		if( CheckWarpConfirmed() == true)
		{
//			Debug.LogError("AsPlayerFsm::MovePacketSynchronize: trying send move packet");
			yield break;
		}
		
		SendingPacket();
	}
	
	bool CheckWarpConfirmed()
	{
		if( AsCommonSender.isSendWarp == true)
			return true;
		else
			return false;
	}

	public void ClearMovePacket()
	{
		enableRefresh = true;
		StopCoroutine( "RefreshDelay");
	}
	
	void StackMovePacket( eMoveType _type, Vector3 _curPos, Vector3 _destPos)
	{
		topInfo = new SavedMoveInfo( _type, _curPos, _destPos);
		secondInfo = topInfo;
	}
	
//	void SendingPacket( eMoveType _moveType, Vector3 _curPosition, Vector3 _destPosition)
	void SendingPacket()
	{
		Vector3 variance = sendedInfo.sCurPosition - topInfo.sCurPosition;
		if(variance.sqrMagnitude < sqrMaxMoveRange)
		{
			sendedInfo = secondInfo = topInfo;
			AsCommonSender.SendMove( topInfo);
		}
		else
		{
			Debug.LogError("AsPlayerFsm::SendingPacket: ( sendedInfo.sCurPosition[" + sendedInfo.sCurPosition +
				"] - topInfo.sCurPosition[" + topInfo.sCurPosition +
				"] ).sqrMagnitude[" + variance +
				"] > sqrMaxMoveRange[" + sqrMaxMoveRange +
				"]). second stack move info will be processed");
			
			Vector3 innerVariance = sendedInfo.sCurPosition - secondInfo.sCurPosition;
			if( innerVariance.sqrMagnitude < sqrMaxMoveRange)
			{
				AsCommonSender.SendMove( secondInfo);
				AsCommonSender.SendMove( topInfo);
				sendedInfo = topInfo = secondInfo;
			}
			else
			{
				Debug.LogError("AsPlayerFsm::SendingPacket: ( sendedInfo.sCurPosition[" + sendedInfo.sCurPosition +
					"] - second.sCurPosition[" + secondInfo.sCurPosition +
					"] ).sqrMagnitude[" + innerVariance +
					"] > sqrMAxMoveRange[" + sqrMaxMoveRange +
					"]). movement will not be processed");
				
				return;
			}
		}
	}
	#endregion
	#region - forced movement -
	void OnForcedMovementResult( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnForcedMoveIndicate( AsIMessage _msg)
	{
	}
	#endregion
	
	#region - jump -
	void OnJumpStop( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion
	
	//BUFF & PROPETRY
	#region - condition -
	void OnConditionStun( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_STUN);
	}

	void OnRecoverConditionStun( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionFreeze( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_FREEZE);
	}

	void OnRecoverConditionFreeze( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionSleep( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_SLEEP);
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
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_FEAR);
	}

	void OnRecoverConditionFear( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionBinding( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnRecoverConditionBinding( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnConditionForcedMove( AsIMessage _msg)
	{
		if(m_Entity.GetProperty<bool>(eComponentProperty.LIVING) == true)
		{
			if( m_CurrentFsmState.FsmStateType == ePlayerFsmStateType.CONDITION_BINDING)
			{
				Msg_ConditionIndicate_ForcedMove forced = _msg as Msg_ConditionIndicate_ForcedMove;
				forced.PrevState_Bind();
			}
			
			SetPlayerFsmState( ePlayerFsmStateType.CONDITION_FORCEDMOVE, _msg);
		}
	}

	void OnForcedMoveSync( AsIMessage _msg)
	{
		Msg_ForcedMove_Sync sync = _msg as Msg_ForcedMove_Sync;
		AsCommonSender.SendMove( eMoveType.Sync_Stop, sync.destination_, sync.destination_);
	}
	
	GameObject m_BlankEffect = null;
	void OnConditionBlank( AsIMessage _msg)
	{
		m_BlankEffect = Instantiate( Resources.Load("UseScript/BlankEffect")) as GameObject;
		if( m_BlankEffect == null)
		{
			Debug.LogError("AsPlayerFsm::OnConditionBlank: there is no blank effect object");
			return;
		}
	}

	void OnRecoverConditionBlank( AsIMessage _msg)
	{
		Destroy( m_BlankEffect);
	}
	
	void OnConditionAirBone( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_AIRBONE, _msg);
	}

	void OnRecoverConditionAirBone( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnConditionTransform( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.CONDITION_TRANSFORM, _msg);
	}

	void OnRecoverConditionTransform( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
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
			string str = AsTableManager.Instance.GetTbl_String(skillRecord.SkillName_Index);
			m_Entity.SkillNameShout( eSkillNameShoutType.Self, str);
		}

		if( ePotency_Type.DebuffResist == skillRecord.listSkillPotency[ skill.potencyIdx_].Potency_Type)
			AsHUDController.Instance.panelManager.ShowDebuffResist( Entity.gameObject);
	}
	#endregion
	#region - buff -
	void OnBuff( AsIMessage _msg)
	{
		Msg_CharBuff buff = _msg as Msg_CharBuff;

		foreach( Msg_CharBuff_Body body in buff.listBuff_)
		{
			m_ElementProcessor.SetBuff( buff, body);
		}

		AsUserInfo.Instance.StoreBuff( buff);
	}

	void OnDeBuff( AsIMessage _msg)
	{
		Msg_CharDeBuff deBuff = _msg as Msg_CharDeBuff;

		m_ElementProcessor.RemoveBuff( deBuff);

		AsUserInfo.Instance.ReleaseBuff( deBuff);
	}

	void OnBuffRefresh( AsIMessage _msg)
	{
//		Msg_BuffRefresh msg = _msg as Msg_BuffRefresh;

		foreach( KeyValuePair<int, Dictionary<int, Msg_CharBuff_Body>> dic in AsUserInfo.Instance.dicBuff)
		{
			foreach( KeyValuePair<int, Msg_CharBuff_Body> pair in dic.Value)
			{
				m_ElementProcessor.SetBuff( pair.Value);
			}
		}
	}

	void OnDeathDebuffClear( AsIMessage _msg)
	{
//		Msg_DeathDebuffClear msg = _msg as Msg_DeathDebuffClear;

		if( null != m_Entity.ModelObject)
		{
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_SkillLearn_01",
				m_Entity.ModelObject.transform, false, 0, 1);
			AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_SkillLearn_Eff",
				m_Entity.transform.position, false);
		}
	}
	
	void OnDebuffResist( AsIMessage _msg)
	{
		AsHUDController.Instance.panelManager.ShowDebuffResist( Entity.gameObject);
	}
	
	static bool s_DeathPenaltyApplied = false; public bool DeathPenaltyApplied{get{return s_DeathPenaltyApplied;}}
	void OnDeathPenalty( AsIMessage _msg)
	{
		Msg_DeathPenaltyIndicate penalty = _msg as Msg_DeathPenaltyIndicate;
		s_DeathPenaltyApplied = penalty.penalty_;
	}
	#endregion
	#region - attribute -
	void OnAttackSpeedRefresh( AsIMessage _msg)
	{
		Msg_AttackSpeedRefresh refresh = _msg as Msg_AttackSpeedRefresh;

		m_Entity.SetProperty( eComponentProperty.ATTACK_SPEED, refresh.attackSpeed_);
	}
	#endregion

	#region - anim end -
	void OnAnimationEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - attack sc & hit execution & damagaed & affected -
	void OnAttackSC( AsIMessage _msg)
	{
		if( m_AttackSC != null && m_AttackSC.ready_ == false)
		{
			OnHitExecution( _msg);
		}

		m_AttackSC = _msg as Msg_OtherCharAttackNpc1;
		if( m_AttackSC.ready_ == false)
			OnHitExecution( _msg);
	}
	
	void OnDamaged( AsIMessage _msg)
	{
		if( m_Entity == null)
			return;

		Msg_NpcAttackChar2 attack = _msg as Msg_NpcAttackChar2;
		m_UserEntity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);
		m_UserEntity.SetProperty( eComponentProperty.MP_CUR, attack.mpCur_);
//		Debug.Log( "PlayerFsm::OnDamage " + attack.hpCur_ + " ( from " + attack.);
		AsUserInfo.Instance.SavedCharStat.hpCur_ = attack.hpCur_;

		m_UserEntity.HandleMessage( new Msg_CombatBegin());

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

		#region - hud -
		// < ilmeda 20120425
		AsHUDController hud = null;
		GameObject go = null;
		if( AsHUDController.Instance != null)
			go = AsHUDController.Instance.gameObject;

		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			hud.panelManager.ShowNumberPanel( m_UserEntity.gameObject, (int)attack.damage_, attack.eDamageType_, attack.parent_.action_, false);

//#if _PVP
			if( (int)attack.hpHeal_ > 0)
				hud.panelManager.ShowNumberPanel( m_UserEntity, (int)attack.hpHeal_, eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_HP, attack.eDamageType_);

			if( (int)attack.mpHeal_ > 0)
				hud.panelManager.ShowNumberPanel( m_UserEntity, (int)attack.mpHeal_, eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_MP, attack.eDamageType_);
//#endif
		}
		// ilmeda 20120425 >
		#endregion
		
		AutoCombatManager.Instance.DamagedProcess();

		if( attack.hpCur_ < 1)
		{
			UserEntity.SetProperty( eComponentProperty.LIVING, false);
			#region - quest -
			ArkQuestmanager.instance.QuestFailByDeath();
			#endregion
			StartCoroutine( DeathProcess( _msg));
		}
	}
	
	void OnAffected( AsIMessage _msg)
	{
		if( m_Entity == null)
			return;

		Msg_OtherCharAttackNpc3 attack = _msg as Msg_OtherCharAttackNpc3;

		if( attack.damage_ > 0)
			m_UserEntity.HandleMessage( new Msg_CombatBegin());

		m_ElementProcessor.PotencyProcess( attack);
		m_CurrentFsmState.MessageProcess( attack);

		#region - hud -
		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			bool getPositiveEffect = false;
			if( (int)attack.hpHeal_ > 0)
			{
				getPositiveEffect = true;
				hud.panelManager.ShowNumberPanel( m_UserEntity, (int)attack.hpHeal_, eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_HP, attack.eDamageType);
			}

			if( (int)attack.mpHeal_ > 0)
			{
				getPositiveEffect = true;
				hud.panelManager.ShowNumberPanel( m_UserEntity, (int)attack.mpHeal_, eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_MP, attack.eDamageType);
			}
			
			if(getPositiveEffect == true && attack.parent_.attacker_ != m_UserEntity)
				AsEmotionManager.Instance.Event_Condition_GetBuff();
//#if _PVP
			if( (int)attack.damage_ > 0)
				hud.panelManager.ShowNumberPanel( m_UserEntity.gameObject, (int)attack.damage_, attack.eDamageType, attack.parent_.action_, false);

			if( attack.eDamageType != eDAMAGETYPE.eDAMAGETYPE_CRITICAL)
			{
				if( ( int)attack.hpHeal_ < 0)
				{
					int nDamage = ( int)attack.hpHeal_ * -1;
					hud.panelManager.ShowNumberPanel( m_UserEntity.gameObject, nDamage, attack.eDamageType, attack.parent_.action_, false);
				}
				else
					hud.panelManager.ShowNumberPanel( m_UserEntity.gameObject, (int)attack.damage_, attack.eDamageType, attack.parent_.action_, false);
			}
			else if( (int)attack.damage_ > 0)
				hud.panelManager.ShowNumberPanel( m_UserEntity.gameObject, (int)attack.damage_, attack.eDamageType, attack.parent_.action_, false);
//#endif
		}
		#endregion

		m_Entity.SetProperty( eComponentProperty.HP_CUR, attack.hpCur_);
		m_Entity.SetProperty( eComponentProperty.MP_CUR, attack.mpCur_);

		#region - skill name shout -
		if( attack.parent_.skill_.SkillName_Print != eSkillNamePrint.None)
		{
			if( attack.parent_.skill_.CheckPotencyTypeIncludeHeal() == true)
			{
				string skillName = AsTableManager.Instance.GetTbl_String(attack.parent_.skill_.SkillName_Index);
				m_Entity.SkillNameShout( eSkillNameShoutType.Benefit, skillName);
			}
		}
		#endregion
	}
	
	void OnHitExecution( AsIMessage _msg)
	{
		if( m_AttackSC == null)
			return;

//		if( m_AttackSC.action_.HitAnimation != null &&
//				m_AttackSC.action_.HitAnimation.hitInfo != null &&
//				m_AttackSC.action_.HitAnimation.hitInfo.HitType == eHitType.ProjectileTarget)
//		{
//			Tbl_Action_HitInfo hitInfo = m_AttackSC.action_.HitAnimation.hitInfo;
//
//			Debug.Log( "AsPlayerFsm::OnAttackSC: ( time:" + System.DateTime.Now + ") projectile target =  " + hitInfo.HitProjectileHitFileName);
//		}

		Int32 mainTargetId = m_AttackSC.npcIdx_[0];

		ProjectileHit();

		if( m_AttackSC.hpHeal_ > 0)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			AsHUDController.Instance.panelManager.ShowNumberPanel( userEntity, (int)( m_AttackSC.hpHeal_), eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL, AsPanelManager.eCustomFontType.eCustomFontType_HP);
//			userEntity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);
		}

		//npc
		foreach( Msg_OtherCharAttackNpc2 attack2 in m_AttackSC.npcBody_)
		{
			AsEntityManager.Instance.DispatchMessageByNpcSessionId( attack2.npcIdx_, attack2);

			#region - hud -
			// < ilmeda 20120425
			if( null != AsHUDController.Instance)
			{
				AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( attack2.npcIdx_);
				if( null != npcEntity)
				{
					AsHUDController.Instance.panelManager.ShowNumberPanel( npcEntity.gameObject, (int)attack2.damage_, attack2.eDamageType_, attack2.parent_.action_, true);//, m_AttackSC.actionTableIdx_);
					
					if(attack2.heal_ < 0f)
						AsHUDController.Instance.panelManager.ShowNumberPanel( npcEntity.gameObject, (int)-attack2.heal_, attack2.eDamageType_, attack2.parent_.action_, true);//, m_AttackSC.actionTableIdx_);
					
					if( (int)attack2.reflection_ > 0)
					{
						AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
						AsHUDController.Instance.panelManager.ShowNumberPanel( userEntity.transform.gameObject, (int)( attack2.reflection_), attack2.eDamageType_, false);
					}

					if( null != AsHUDController.Instance.targetInfo.getCurTargetEntity)
					{
						if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == ( ( AsBaseEntity)npcEntity).GetInstanceID())
							AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( ( AsBaseEntity)npcEntity);
					}
				}
			}
			// ilmeda 20120425 >
			if( attack2.npcIdx_ == mainTargetId)
			{
				if( null != AsHUDController.Instance)
				{
					AsHUDController.Instance.targetInfo.HpCur = (int)attack2.hpCur_;

					if( 0 >= attack2.hpCur_)
					{
						m_Target = null;
						AsHUDController.Instance.targetInfo.Enable = false;
					}
					else
					{
						if( false == AsHUDController.Instance.targetInfo.Enable)
							AsHUDController.Instance.targetInfo.Enable = true;
					}
				}
			}
			#endregion
		}

		//pc
		foreach( Msg_OtherCharAttackNpc3 attack3 in m_AttackSC.charBody_)
		{
			AsEntityManager.Instance.DispatchMessageByUniqueKey( attack3.charUniqKey_, attack3);
			
			#region - hud -
			if( attack3.reflection_ > 0 && Entity != null)
			{
				if( null != AsHUDController.Instance)
				{
					AsHUDController.Instance.panelManager.ShowNumberPanel( Entity.gameObject, (int)attack3.reflection_, attack3.eDamageType, true);	// need reflection font
					m_Entity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);
					AsEntityManager.Instance.MessageToPlayer( new Msg_ReflectionRefresh( Entity));
				}
			}
			#endregion
		}

		m_Entity.SetProperty( eComponentProperty.HP_CUR, m_AttackSC.hpCur_);// life steal
		AsUserInfo.Instance.SavedCharStat.hpCur_ = m_AttackSC.hpCur_;

		if( m_AttackSC.hpCur_ < 1)
		{
			UserEntity.SetProperty( eComponentProperty.LIVING, false);
			#region - quest -
			ArkQuestmanager.instance.QuestFailByDeath();
			#endregion
			StartCoroutine( DeathProcess( _msg));
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
				if( entity != null &&  entity.GetProperty<float>( eComponentProperty.HP_CUR) > 0)  //dopamin
				{
					AsEffectManager.Instance.PlayEffect( record.HitAnimation.hitInfo.HitProjectileHitFileName,
						entity.transform, false, 0);
				}
			}
		}
	}
	
	void Timer_LateExecution( System.Object _msg)
	{
		OnHitExecution( m_AttackSC);
	}
	#endregion
	
	#region - pet -
	Msg_PetDataIndicate m_PetData;
	Msg_PetItemView m_PetItem;
	void OnPetDataIndicate( AsIMessage _msg)
	{
		Msg_PetDataIndicate indicate = _msg as Msg_PetDataIndicate;
		m_Pet = AsEntityManager.Instance.PetAppear( indicate.data_);
		m_Pet.HandleMessage( indicate);
		
		m_PetData = indicate;
		
		m_UserEntity.OwnPet( true);
		
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

	void OnPetSkillReady( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}

	void OnPetSkillResult( AsIMessage _msg)
	{
		if( m_Pet != null)
			m_Pet.HandleMessage( _msg);
	}
	
	void OnPetNameChange( AsIMessage _msg)
	{
		if( m_Pet != null)
			m_Pet.HandleMessage( _msg);
	}
	
	void OnPetDelete( AsIMessage _msg)
	{
		if(m_Pet != null)
			AsEntityManager.Instance.RemovePet( m_Pet);
		else
			Debug.LogError("AsPlayerFsm:: OnPetDelete: there is no pet. invalid release");

		m_UserEntity.OwnPet( false);
	}
	
	void OnPetSkillChangeResult( AsIMessage _msg)
	{
		if( m_Pet != null)
			m_Pet.HandleMessage( _msg);
	}
	
	void OnPetFeeding( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	
	void OnPetScript( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
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
		
		StartCoroutine( HatchPerform());
	}
	
	void OnItemView( AsIMessage _msg)
	{
		Msg_PetItemView view = _msg as Msg_PetItemView;
		if(m_Pet != null)
			m_Pet.SetPetItemView( view);
		
		m_PetItem = view;
	}
	
	void OnPetSkillGet( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	
	void OnPetPositionRefresh( AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}
	
	void OnPetHungryIndicate(AsIMessage _msg)
	{
		if( m_Pet != null) m_Pet.HandleMessage( _msg);
	}

	void OnPetEffectIndicate(AsIMessage _msg)
	{
		if(m_Pet != null)
		{
			Msg_PetEffectIndicate effect = _msg as Msg_PetEffectIndicate;
			AsEffectManager.Instance.PlayEffect(effect.path_, m_Pet.transform.position, false, 0f, 1f);
		}
	}
	#endregion

	#region - cheat death -
	public void CheatDeath( AsIMessage _msg)
	{
		//StartCoroutine( DeathProcess( _msg));
	}
	#endregion

	#region - death process -
	IEnumerator DeathProcess( AsIMessage _msg)
	{
		#region - condition & auto combat & accure -
		AsEmotionManager.Instance.Event_Condition_Death();
		
		AutoCombatManager.Instance.DeathProcess();
		
		AsEffectManager.Instance.StopEffect(m_AccureDodge);
		AsEffectManager.Instance.StopEffect(m_AccureCritical);
		
		m_AccureDodge = 0;
		m_AccureCritical = 0;
		#endregion
		
		AsHudDlgMgr.Instance.Init();
		AsHudDlgMgr.Instance.CollapseMenuBtn();
		AsHudDlgMgr.Instance.CloseDelegateImageDlg();
		AsHudDlgMgr.Instance.CloseQuestAccept();
		AsGameGuideManager.Instance.CloseGameGuide();
		AsGameGuideManager.Instance.CloseGameGuideListDlg();

//		yield return new WaitForSeconds( 0.01f);
		
		SetPlayerFsmState( ePlayerFsmStateType.DEATH);
		m_ElementProcessor.ReleaseBuff();
		
		//pet
		AsPetManager.Instance.PlayerDeath();

		yield return new WaitForSeconds( 2.0f);

		if( TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Pvp) == false)
			AsNotify.Instance.DeathDlg();

		// < ilmeda 20120426
		AsHUDController.Instance.targetInfo.Enable = false;
		// ilmeda 20120426 >
	}

	private void ToCharacterSelectScene()
	{
		AS_CG_RETURN_CHARSELECT retCharSelect = new AS_CG_RETURN_CHARSELECT();
		byte[] data = retCharSelect.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	#endregion

	#region - release tension -
	void OnReleaseTension( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - reflection -
	void OnReflectionRefresh( AsIMessage _msg)
	{
		Msg_ReflectionRefresh reflection = _msg as Msg_ReflectionRefresh;
		if( reflection.entity_ == Target)
		{
			AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( Target);
		}
	}
	#endregion

	//SKILL
	#region - charge & active & drag & arc & circle & double tap & action item & stance -
	void OnBeginCharge( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

		if( CheckMap_Village() == true)
		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsMyProperty.Instance.AlertSkillInTown();
			return;
		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		Msg_Input_Begin_Charge msg = _msg as Msg_Input_Begin_Charge;
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( msg.skillIdx_);

		eGENDER gender = Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( record, gender);
		ready.SetCharging();
		
		if(TargetDecider.sSkillUsable_TargetCharge(this, ready) == false)
			return;

		if( ready.constructSucceed == true)
			m_CurrentFsmState.MessageProcess( ready);
	}
	void OnCancelCharge( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	void OnSkillActive( AsIMessage _msg)
	{
//		Msg_Input_Slot_Active __msg = _msg as Msg_Input_Slot_Active;
//		Debug.Log( "AsPlayerFsm::OnSkillActive: __msg.skillIdx_ = " + __msg.skillIdx_);

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

		if( CheckMap_Village() == true)
		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsMyProperty.Instance.AlertSkillInTown();
			return;
		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		Msg_Input_Slot_Active msg = _msg as Msg_Input_Slot_Active;
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( msg.skillIdx_);

		bool permit = true;

		if( record.Skill_Type == eSKILL_TYPE.SlotBase && m_ElapsedCoolTime_BaseAttack > 0)
		{
			Debug.Log( "AsPlayerFsm::OnSkillActive: SlotBase skill is used in base attack cool time.");
			permit = false;
		}
		
//		if(record.Skill_Type == eSKILL_TYPE.Stance)
//		{
//			body_CS_CHAR_SKILL_STANCE stance = new body_CS_CHAR_SKILL_STANCE();
//			byte[] bytes = stance.ClassToPacketBytes();
//			AsCommonSender.Send(bytes);
//			
////			AsCommonSender.BeginSkillUseProcess( record.Index);
//		}

		if( permit == true)
		{
			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);

			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( record, gender, msg.step_,
				null, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
				m_Entity.transform.forward, eClockWise.CW);

			if( ready.constructSucceed == true)
			{
				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;
				
				if( ready.CheckValidTargeting( m_Target) == false)
				{
					Debug.Log( "AsPlayerFsm::OnSkillActive: m_Target.GetInstanceID() = " + m_Target.GetInstanceID());
					Target = null;
					return;
				}

				if( Target != null &&
					m_Entity.GetNavPathDistance( Entity.transform.position, Target.transform.position, true) >
					ready.skillLvRecord.Usable_Distance * 0.01f)// * m_Character_Size)
				{
//					Debug.Log( "AsPlayerFsm::OnSkillActive: msg.skillIdx_ = " + msg.skillIdx_ + ", BattleRun state began [time:" + Time.realtimeSinceStartup + "]");
					Msg_Player_Skill_Target_Move targetMove = new Msg_Player_Skill_Target_Move( ready);
					m_CurrentFsmState.MessageProcess( targetMove);
				}
				else
				{
//					Debug.Log( "AsPlayerFsm::OnSkillActive: msg.skillIdx_ = " + msg.skillIdx_ + ", SkillReady state began [time:" + Time.realtimeSinceStartup + "]");
					m_CurrentFsmState.MessageProcess( ready);
				}
			}
		}
	}
	void OnSkillDragStraight( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

//		if( CheckMap_Village() == true)
//		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
//			return;
//		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		if( AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)COMMAND_SKILL_TYPE._STRAIGHT] == true)
		{
			eCLASS __class = UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);

			if( false == IsAvailableCommandSkill( __class, eCommand_Type.Straight))
				return;

			Msg_Input_DragStraight msg = _msg as Msg_Input_DragStraight;
			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( 
				__class, eSKILL_TYPE.Command, eCommand_Type.Straight, gender,
				null, Vector3.zero,
				msg.head_, msg.center_, msg.tail_, msg.direction_, eClockWise.CW);

			if( ready.constructSucceed == true && SkillBook.Instance.dicCurSkill.ContainsKey( ready.skillRecord.Index) == true)
			{
				if( TargetDecider.CheckDisableSkillByMap(ready.skillRecord) == true)
					return;
				
				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;

				if( m_Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) == false &&
					ready.skillRecord.CheckSkillUsingOnly_Movable() == true)
				{
					AsMyProperty.Instance.AlertState();
					return;
				}

//				if( CheckSkillMovable( ready) == false)
//					return;

				if( CheckMap_Village() == true)
				{
//					string content = AsTableManager.Instance.GetTbl_String(830);
//					AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
					AsMyProperty.Instance.AlertSkillInTown();
				}
				else if( false == IsAvailableCommandSkill( __class, eCommand_Type.Straight))
					return;
				else
					m_CurrentFsmState.MessageProcess( ready);
			}
		}
		else
		{
			AsMyProperty.Instance.AlertCoolTime( COMMAND_SKILL_TYPE._STRAIGHT);
		}
	}
	void OnSkillArc( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

//		if( CheckMap_Village() == true)
//		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
//			return;
//		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		Msg_Input_Arc msg = _msg as Msg_Input_Arc;
		eCommand_Type commandType = eCommand_Type.ArcCW;
		COMMAND_SKILL_TYPE commandSkillType = COMMAND_SKILL_TYPE._ARC_CW;

		switch( msg.cw_)
		{
		case eClockWise.CW:
			commandType = eCommand_Type.ArcCW;
			commandSkillType = COMMAND_SKILL_TYPE._ARC_CW;
			break;
		case eClockWise.CCW:
			commandType = eCommand_Type.ArcCCW;
			commandSkillType = COMMAND_SKILL_TYPE._ARC_CCW;
			break;
		}

		if( AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)commandSkillType] == true)
		{
			eCLASS __class = UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);

//			Msg_Input_Arc msg = _msg as Msg_Input_Arc;
			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( 
				__class, eSKILL_TYPE.Command, commandType, gender,
				null, Vector3.zero,
				msg.head_, msg.center_, msg.tail_, msg.direction_, msg.cw_);

			if( ready.constructSucceed == true && SkillBook.Instance.dicCurSkill.ContainsKey( ready.skillRecord.Index) == true)
			{
				if( TargetDecider.CheckDisableSkillByMap(ready.skillRecord) == true)
					return;
				
				if( AsCommonSender.CheckSkillUseProcessing() == true)
				{
//					Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( AsCommonSender.usedSkillIdx);
//					Debug.LogWarning( "AsPlayerFsm::OnSkillArc: skill index = " + skillRecord.Index + ", skill command type = " + skillRecord.Command_Type);
					return;
				}

				if( m_Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) == false &&
					ready.skillRecord.CheckSkillUsingOnly_Movable() == true)
				{
					AsMyProperty.Instance.AlertState();
					return;
				}

				if( CheckMap_Village() == true)
				{
//					string content = AsTableManager.Instance.GetTbl_String(830);
//					AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
					AsMyProperty.Instance.AlertSkillInTown();
				}
				else if( false == IsAvailableCommandSkill( __class, commandType))
					return;
				else
					m_CurrentFsmState.MessageProcess( ready);
			}
		}
		else
		{
			AsMyProperty.Instance.AlertCoolTime( commandSkillType);
		}
	}
	void OnSkillCircle( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

//		if( CheckMap_Village() == true)
//		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
//			return;
//		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		Msg_Input_Circle msg = _msg as Msg_Input_Circle;
		eCommand_Type commandType = eCommand_Type.CircleCW;
		COMMAND_SKILL_TYPE commandSkillType = COMMAND_SKILL_TYPE._CIRCLE_CW;

		switch( msg.cw_)
		{
		case eClockWise.CW:
			commandType = eCommand_Type.CircleCW;
			commandSkillType = COMMAND_SKILL_TYPE._CIRCLE_CW;
			break;
		case eClockWise.CCW:
			commandType = eCommand_Type.CircleCCW;
			commandSkillType = COMMAND_SKILL_TYPE._CIRCLE_CCW;
			break;
		}

		if( AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)commandSkillType] == true)
		{
			eCLASS __class = UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);

			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( 
				__class, eSKILL_TYPE.Command, commandType, gender,
				null, Vector3.zero,
				msg.head_, msg.center_, msg.tail_, msg.direction_, msg.cw_);

			if( ready.constructSucceed == true && SkillBook.Instance.dicCurSkill.ContainsKey( ready.skillRecord.Index) == true)
			{
				if( TargetDecider.CheckDisableSkillByMap(ready.skillRecord) == true)
					return;
				
				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;

				if( m_Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) == false &&
					ready.skillRecord.CheckSkillUsingOnly_Movable() == true)
				{
					AsMyProperty.Instance.AlertState();
					return;
				}

				if( CheckMap_Village() == true)
				{
//					string content = AsTableManager.Instance.GetTbl_String(830);
//					AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
					AsMyProperty.Instance.AlertSkillInTown();
				}
				else if( false == IsAvailableCommandSkill( __class, commandType))
					return;
				else
					m_CurrentFsmState.MessageProcess( ready);
			}
		}
		else
		{
			AsMyProperty.Instance.AlertCoolTime( commandSkillType);
		}
	}
	void OnSkillDoubleTap( AsIMessage _msg)
	{
		Msg_Input_DoubleTab doubleTap = _msg as Msg_Input_DoubleTab;
		
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			if( doubleTap.type_ == Msg_Input_DoubleTab.eDoubleTabType.Player)
				m_CurrentFsmState.MessageProcess( new Msg_OpenPrivateShopUI());

			return;
		}

		if( CheckSpecificWindowOpened() == true)
			return;

//		if( CheckMap_Village() == true)
//		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
//			return;
//		}

		if( WeaponEquip == false)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return;
		}

		eCLASS __class = UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		Tbl_Skill_Record skill = null;
		int idx = -1;
		eCommandPicking_Type pickingType = eCommandPicking_Type.NONE;

		switch( doubleTap.type_)
		{
		case Msg_Input_DoubleTab.eDoubleTabType.Terrain:
		case Msg_Input_DoubleTab.eDoubleTabType.OtherUser:
		case Msg_Input_DoubleTab.eDoubleTabType.Monster:
			idx = (int)COMMAND_SKILL_TYPE._DOUBLE_TAP_MONSTER;
			pickingType = eCommandPicking_Type.FingerPoint;
			break;
		case Msg_Input_DoubleTab.eDoubleTabType.Player:
			idx = (int)COMMAND_SKILL_TYPE._DOUBLE_TAP_PLAYER;
			pickingType = eCommandPicking_Type.Self;
			break;
//		case Msg_Input_DoubleTab.eDoubleTabType.OtherUser:
//			idx = (int)COMMAND_SKILL_TYPE._DOUBLE_TAP_OTHERUSER;
//			break;
//		case Msg_Input_DoubleTab.eDoubleTabType.Monster:
//			idx = (int)COMMAND_SKILL_TYPE._DOUBLE_TAP_MONSTER;
//			pickingType = eCommandPicking_Type.Enemy;
//			break;
		}

//		skill = AsTableManager.Instance.GetTbl_Skill_RecordByPickingType( __class, pickingType);
		skill = SkillBook.Instance.GetLearnedDoubleTapSkill( pickingType);
		if( null == skill)
			return;

		if( SkillBook.Instance.dicCurSkill.ContainsKey( skill.Index) == false)
			return;

		else if( CheckMap_Village() == true)
		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsMyProperty.Instance.AlertSkillInTown();
			return;
		}

		if( AsUserInfo.Instance.CommandSkillCoolTimeCompletion[idx] == true)
		{
			if( false == IsAvailableDoubleTapSkill( __class, pickingType))
				return;

			if( skill == null)
			{
				Debug.LogError( "AsPlayerFsm::OnSkillDoubleTap: this character[" + __class + "] doesnt have [" + doubleTap.type_ + "]type skill");
				return;
			}

			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);

			AsBaseEntity entity = AsEntityManager.Instance.GetEntityByInstanceId( doubleTap.input_.inputObject_.GetInstanceID());
			Vector3 skillPos = Vector3.zero;
			if( entity == null)
				skillPos = doubleTap.input_.worldPosition_;
			else
				skillPos = entity.transform.position;

			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( skill, gender, 1,
				entity, skillPos,
				doubleTap.input_.screenPosition_, doubleTap.input_.screenPosition_, doubleTap.input_.screenPosition_,
				doubleTap.input_.worldPosition_ - m_Entity.transform.position, eClockWise.CW);

//			if( ready.constructSucceed == true)
//				m_CurrentFsmState.MessageProcess( ready);

			if( ready.constructSucceed == true)
			{
				if( TargetDecider.CheckDisableSkillByMap(ready.skillRecord) == true)
					return;
				
				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;

				if( m_Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) == false &&
					ready.skillRecord.CheckSkillUsingOnly_Movable() == true)
				{
					AsMyProperty.Instance.AlertState();
					return;
				}

				if( Vector3.Distance( Entity.transform.position, ready.picked) >
					ready.skillLvRecord.Usable_Distance * 0.01f)// * m_Character_Size)
				{
					Msg_Player_Skill_Target_Move targetMove = new Msg_Player_Skill_Target_Move( ready, AsPlayerState_BattleRun.eRunType.NonTarget);
					m_CurrentFsmState.MessageProcess( targetMove);
				}
				else
					m_CurrentFsmState.MessageProcess( ready);
			}
		}
		else
		{
			AsMyProperty.Instance.AlertCoolTime( ( COMMAND_SKILL_TYPE)idx);
		}
	}
	void OnActionItem( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		if( CheckSpecificWindowOpened() == true)
			return;

		Msg_Player_Use_ActionItem msg = _msg as Msg_Player_Use_ActionItem;
		ItemData itemData = msg.realItem.item.ItemData;
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( itemData.itemSkill);
		eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( itemData, msg.realItem.getSlot, gender);
		ready.SetCurrentTarget( Target);

		if( CheckMap_Village() == true && ready.constructSucceed == true)
		{
//			string content = AsTableManager.Instance.GetTbl_String(830);
//			AsChatManager.Instance.InsertChat( content, eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsMyProperty.Instance.AlertSkillInTown();
			return;
		}

//		Msg_Player_Use_ActionItem msg = _msg as Msg_Player_Use_ActionItem;
//		ItemData itemData = msg.realItem.item.ItemData;
//		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( itemData.itemSkill);

		bool permit = true;

		if( record.Skill_Type == eSKILL_TYPE.SlotBase && m_ElapsedCoolTime_BaseAttack > 0)
			permit = false;

		if( permit == true)
		{
//			eGENDER gender = UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);
//			Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( itemData, msg.realItem.getSlot, gender);

			if( ready.constructSucceed == true)
			{
				if( WeaponEquip == false)
				{
					AsMyProperty.Instance.AlertNoWeapon();
					return;
				}

				if( null == msg || null == msg.realItem || msg.realItem.CheckSkillUsable() == false)
					return;

				int iCurLevel = m_Entity.GetProperty<int>( eComponentProperty.LEVEL);
				if( iCurLevel < itemData.levelLimit)
				{
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1651), eCHATTYPE.eCHATTYPE_SYSTEM);
					AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(1651));
					return;
				}

				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;

				if( m_Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) == false &&
					ready.skillRecord.CheckSkillUsingOnly_Movable() == true)
				{
					AsMyProperty.Instance.AlertState();
					return;
				}

//				if( SetTargetInActionItem( record) == false)
//					return;
				
				TargetDecider.sTargeting_AsSlot( this, ready.skillRecord);
				
				if( TargetDecider.CheckValidSkill( this, ready.skillRecord) == false)
					return;

				if( m_Target == null &&
					( ready.skillRecord.Skill_Type == eSKILL_TYPE.Target ||
					ready.skillRecord.Skill_Type == eSKILL_TYPE.SlotBase))
					return;
				
				if( TargetDecider.TargetSkillCheck( ready.skillRecord.Index, this) == false)
					return;

				if( ready.CheckValidTargeting( m_Target) == false)
					return;

				if( Target != null &&
					m_Entity.GetNavPathDistance( Entity.transform.position, Target.transform.position, true) >
					ready.skillLvRecord.Usable_Distance * 0.01f)// * m_Character_Size)
				{
					Debug.Log( "AsPlayerFsm::OnSkillActive: msg.skillIdx_ = " + itemData.itemSkill + ", BattleRun state began [time:" + Time.realtimeSinceStartup + "]");
					Msg_Player_Skill_Target_Move targetMove = new Msg_Player_Skill_Target_Move( ready);
					m_CurrentFsmState.MessageProcess( targetMove);
				}
				else
				{
					Debug.Log( "AsPlayerFsm::OnSkillActive: msg.skillIdx_ = " + itemData.itemSkill + ", SkillReady state began [time:" + Time.realtimeSinceStartup + "]");
					m_CurrentFsmState.MessageProcess( ready);
				}
			}
			else if( ready.skillRecord.CheckNonAnimation() == true)
			{
				if( msg.realItem.CheckSkillUsable() == false)
					return;

				int iCurLevel = m_Entity.GetProperty<int>( eComponentProperty.LEVEL);
				if( iCurLevel < itemData.levelLimit)
				{
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1651), eCHATTYPE.eCHATTYPE_SYSTEM);
					AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(1651));
					return;
				}
				
				if( AsCommonSender.CheckSkillUseProcessing() == true)
					return;
				
				TargetDecider.sTargeting_AsSlot( this, ready.skillRecord);
				
				if( TargetDecider.CheckValidSkill( this, ready.skillRecord) == false)
					return;
				
				if( TargetDecider.TargetSkillCheck( ready.skillRecord.Index, this) == false)
					return;

				int mainTargetIdxNpc = 0;
				uint mainTargetIdxUser = 0;

				TargetDecider decider = new TargetDecider();
				if( decider.SetTarget( this, ready) == true)
				{
					mainTargetIdxNpc = decider.listTargets_Npc[0];
					mainTargetIdxUser = decider.listTargets_User[0];

					if( mainTargetIdxNpc != int.MaxValue && mainTargetIdxUser != uint.MaxValue)
					{
						AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC( ready.skillLvRecord.Skill_GroupIndex,
							ready.skillLvRecord.Skill_Level, ready.actionRecord.Index,
							0, false, false, ready.itemSlot,
							decider.listTargets_Npc , decider.listTargets_User, ready.picked, transform.forward,
							decider.decidedEntities.TargetCount, decider.decidedEntities.CharCount,
							decider.decidedEntities.listTarget.ToArray(), decider.decidedEntities.listChar.ToArray());

						if( showSendingPacket == true)
						{
							Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( attack.nActionTableIdx);

							Debug.Log( "OnActionItem::SendingPacket: action name=" + action.ActionName +
								" skill index=" + attack.nSkillTableIdx +
								" skill level=" + attack.nSkillLevel + " action index=" + attack.nActionTableIdx +
								" charge step=" + attack.nChargeStep + " casting=" + attack.bCasting + " attack.nSlot =" + attack.nSlot +
								" main target id=" + attack.nNpcIdx[0] + " skill pos=" + attack.sTargeting +
								" transform.forward=" + transform.forward);
						}

//						byte[] bytes = attack.ClassToPacketBytes();
//						AsCommonSender.Send( bytes);
						ePotency_Type potencyType = ready.skillRecord.listSkillPotency[0].Potency_Type;
						if( ePotency_Type.NormalHardPlayCount == potencyType || ePotency_Type.HellPlayCount == potencyType || ePotency_Type.HellPlayCountAll == potencyType)
						{
							if( null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.invenDlg)
							{
								AsHudDlgMgr.Instance.invenDlg.ConfirmIndunCountItem( potencyType, attack, itemData, ready.skillRecord.Index);
							}
						}
						else
						{
							byte[] bytes = attack.ClassToPacketBytes();
							AsCommonSender.Send( bytes);
						}
					}
				}
			}
		}
	}
	void OnStance( AsIMessage _msg)
	{
		Msg_OtherCharSkillStance stance = _msg as Msg_OtherCharSkillStance;
		
		m_ElementProcessor.PotencyProcess( stance);
		
	}
	void OnBuffAccure( AsIMessage _msg)
	{
		Debug.Log("AsPlayerFsm::OnBuffAccure: --------------");
		
		Msg_CharBuffAccure accure = _msg as Msg_CharBuffAccure;
		
		StartCoroutine(BuffAccure_CR(accure));
	}
	int m_AccureCritical = 0;
	int m_AccureDodge = 0;
	IEnumerator BuffAccure_CR(Msg_CharBuffAccure _accure)
	{
		while(true)
		{
			if( null == UserEntity.ModelObject)
				yield return null;
			else
			{
				if( m_AccureDodge != 0 && _accure.info_.bDodgeChance == false)
				{
					AsEffectManager.Instance.StopEffect(m_AccureDodge);
					m_AccureDodge = 0;
				}
				if( m_AccureDodge == 0 && _accure.info_.bDodgeChance == true)
					m_AccureDodge = AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_DodgeChance", UserEntity.ModelObject.transform, false, 0f);

				if( m_AccureCritical != 0 && _accure.info_.bCriticalChance == false)
				{
					AsEffectManager.Instance.StopEffect(m_AccureCritical);
					m_AccureCritical = 0;
				}
				if( m_AccureCritical == 0 && _accure.info_.bCriticalChance == true)
					m_AccureCritical = AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_CriticalChance", UserEntity.ModelObject.transform, false, 0f);
					
				break;
			}
		}
	}
	void OnAdd( AsIMessage _msg)
	{
		Msg_CharSkillAdd __add = _msg as Msg_CharSkillAdd;
		Tbl_SkillLevel_Record lv = AsTableManager.Instance.GetTbl_SkillLevel_Record(__add.skillLevel_, __add.skillTableIdx_);
		
		AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC(__add.skillTableIdx_, __add.skillLevel_, lv.SkillAction_Index, 0, false, false,
			0, new int[TargetDecider.MAX_SKILL_TARGET], new uint[TargetDecider.MAX_SKILL_TARGET], Vector3.zero, Vector3.zero,
			0, 0, new body2_AS_CS_CHAR_ATTACK_NPC[0], new body3_AS_CS_CHAR_ATTACK_NPC[0]);
		
		byte[] bytes = attack.ClassToPacketBytes();
		AsCommonSender.Send( bytes);
	}
	void OnSoulStone( AsIMessage _msg)
	{
		Msg_CharSkillSoulStone soul = _msg as Msg_CharSkillSoulStone;
		soul.SetPlayerFsm( this);

		eGENDER gender = m_Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( soul.soul_.nSkillTableIdx, soul.soul_.nSkillLevel, gender);
		ready.SetCurrentTarget( Target);

		if( ready.skillRecord.CheckNonAnimation() == true)
		{
			TargetDecider.sTargeting_AsSlot( this, ready.skillRecord);

			if( TargetDecider.CheckValidSkill( this, ready.skillRecord) == false)
				return;

			if( TargetDecider.TargetSkillCheck( ready.skillRecord.Index, this) == false)
				return;

			int mainTargetIdxNpc = 0;
			uint mainTargetIdxUser = 0;

			TargetDecider decider = new TargetDecider();			
			if( decider.SetTarget( this, ready) == true)
			{
				mainTargetIdxNpc = decider.listTargets_Npc[0];
				mainTargetIdxUser = decider.listTargets_User[0];

				if( mainTargetIdxNpc != int.MaxValue && mainTargetIdxUser != uint.MaxValue)
				{
					AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC( ready.skillLvRecord.Skill_GroupIndex,
						ready.skillLvRecord.Skill_Level, ready.actionRecord.Index,
						0, false, false, ready.itemSlot,
						decider.listTargets_Npc , decider.listTargets_User, ready.picked, transform.forward,
						decider.decidedEntities.TargetCount, decider.decidedEntities.CharCount,
						decider.decidedEntities.listTarget.ToArray(), decider.decidedEntities.listChar.ToArray());
					
					attack.SetEquipSlot( soul.soul_.eEquipSlot);

//					if( showSendingPacket == true)
//					{
						Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record( attack.nActionTableIdx);

						Debug.Log( "OnActionItem::SendingPacket: action name=" + action.ActionName +
							" skill index=" + attack.nSkillTableIdx +
							" skill level=" + attack.nSkillLevel + " action index=" + attack.nActionTableIdx +
							" charge step=" + attack.nChargeStep + " casting=" + attack.bCasting + " attack.nSlot =" + attack.nSlot +
							" main target id=" + attack.nNpcIdx[0] + " skill pos=" + attack.sTargeting +
							" transform.forward=" + transform.forward);
//					}

					byte[] bytes = attack.ClassToPacketBytes();
					AsCommonSender.Send( bytes);
				}
			}
		}
	}
	void OnPvpAggro( AsIMessage _msg)
	{
		Msg_CharSkillPvpAggro aggro = _msg as Msg_CharSkillPvpAggro;
		
		Target = AsEntityManager.Instance.GetUserEntityByUniqueId( aggro.charUniqKey_);
	}
	void OnSkillShieldBegin( AsIMessage _msg)
	{
//		eCLASS _class = UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//
//		Msg_Player_Skill_Ready ready = new Msg_Player_Skill_Ready( _class, eSKILL_TYPE.Active, eSKILL_INPUT_TYPE.NONE, 1
//			, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
//
//		if( true == ready.constructSucceed)
//			m_CurrentFsmState.MessageProcess( ready);
	}
	void OnSkillShieldEnd( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion
	#region - command skill cool down -
	void OnCommandSkillCoolTimeComplete( AsIMessage _msg)
	{
		Msg_Skill_Charge_Complete msg = _msg as Msg_Skill_Charge_Complete;
		AsUserInfo.Instance.CommandSkillCoolTimeCompletion[(int)msg.type] = true;
	}
	#endregion	
	#region - mana consumption check -
	bool IsAvailableDoubleTapSkill( eCLASS _class, eCommandPicking_Type _pickingType)
	{
		float curMP = UserEntity.GetProperty<float>( eComponentProperty.MP_CUR);
//		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_RecordByPickingType( _class, _pickingType);
		Tbl_Skill_Record skillRecord = SkillBook.Instance.GetLearnedDoubleTapSkill( _pickingType);

		if( null == skillRecord)
		{
			Debug.LogError( "AsPlayerFsm:: IsAvailableDoubleTapSkill: character has no [" + _pickingType + "] double tap skill");
			return false;
		}

		SkillView view = null;
		if( SkillBook.Instance.dicCurSkill.ContainsKey( skillRecord.Index) == true)
			view = SkillBook.Instance.dicCurSkill[skillRecord.Index];
		else
			return false;

		if( view == null)
			return false;

		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( view.nSkillLevel, view.nSkillTableIdx);

		if( curMP < ( float)skillLevelRecord.Mp_Decrease)
		{
			AsMyProperty.Instance.AlertMP();
			return false;
		}

		return true;
	}

	bool IsAvailableCommandSkill( eCLASS _class, eCommand_Type type)
	{
		float curMP = UserEntity.GetProperty<float>( eComponentProperty.MP_CUR);
//		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _class, eSKILL_TYPE.Command, type);
		Tbl_Skill_Record skillRecord = SkillBook.Instance.GetLearnedCommandSkill( type);

		// < ilmeda 20120420
		if( null == skillRecord)
			return false;
		// ilmeda 20120420 >

		SkillView view = null;
		if( SkillBook.Instance.dicCurSkill.ContainsKey( skillRecord.Index) == true)
			view = SkillBook.Instance.dicCurSkill[skillRecord.Index];
		else
			return false;

		if( view == null)
			return false;

		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( view.nSkillLevel, view.nSkillTableIdx);

		if( curMP < ( float)skillLevelRecord.Mp_Decrease)
		{
			AsMyProperty.Instance.AlertMP();
//			GameObject go = AsHUDController.Instance.gameObject;
//			if( null != go)
//			{
//				AsHUDController hud = go.GetComponent<AsHUDController>();
//				hud.AlertMP();
//			}

			return false;
		}

		return true;
	}

	bool IsAvailableActiveSkill( int skillIndex, int step)
	{
		float curMP = UserEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skillIndex, step);

		if( curMP < ( float)skillLevelRecord.Mp_Decrease)
			return false;

		return true;
	}
	#endregion
	
	void OnSkillLearn( AsIMessage _msg)
	{
		Msg_Player_Skill_Learn msg = _msg as Msg_Player_Skill_Learn;
		if( null == UserEntity.ModelObject)
			return;

		if( msg.bLearnSkill == true)
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_SkillLearn_01",UserEntity.ModelObject.transform,false,0f);
		else
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_ArkSkillGet", UserEntity.ModelObject.transform, false, 0f);
	}

	void OnTargetIndication( AsIMessage _msg)
	{
		Msg_TargetIndication indication = _msg as Msg_TargetIndication;
		
		if( indication.sender_.ContainProperty(eComponentProperty.LIVING) == true &&
			indication.sender_.GetProperty<bool>(eComponentProperty.LIVING) == false)
			return;

		if( Target == null)
		{
			Target = indication.sender_;
		}
		else
		{
			float curDist = Vector3.Distance( m_Entity.transform.position, Target.transform.position);
			float indicatedDist = Vector3.Distance( m_Entity.transform.position, indication.sender_.transform.position);

			if( indicatedDist < curDist)
			{
				Target = indication.sender_;
				Debug.Log( "AsPlayerFsm::OnTargetIndication: target is changed by containing buff type");
			}
		}
	}

	//ACT
	#region - dash -
	void OnInputDash( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	// CHEAT
	#region - cheat -
	void OnCheatDeath( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnResurrection( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);

		// promotion
		AsPromotionManager.Instance.Revived();
	}
	#endregion

	void OnMobCasting( AsIMessage _msg)
	{
		Debug.Log( "OnMobCasting");

		Msg_Mob_Cast msg = _msg as Msg_Mob_Cast;
		if( false == msg.castInfo.casting_)
			return;

		AsHUDController.Instance.targetInfo.StartCast( msg.castInfo.castingMilliSec_ * 0.001f);
	}

	#region - obj -
	void OnObjTrapAction( AsIMessage _msg)
	{
		if( m_CurState != ePlayerFsmStateType.OBJ_TRAP)
			SetPlayerFsmState( ePlayerFsmStateType.OBJ_TRAP);
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnObjSteppingAction( AsIMessage _msg)
	{
		if( m_CurState != ePlayerFsmStateType.OBJ_STEPPING)
			SetPlayerFsmState( ePlayerFsmStateType.OBJ_STEPPING);
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - click -
	void OnPlayerClick( AsIMessage _msg)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			m_CurrentFsmState.MessageProcess( new Msg_PlayerClick());
			return;
		}
	}
	
	void OnOtherUserClick( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
//		if( null != AsHudDlgMgr.Instance && ( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg))
			return;

		Msg_OtherUserClick otherUserClick =  _msg as Msg_OtherUserClick;
//		AsHUDController.Instance.SetTargetUser( ( ushort)( otherUserClick.idx_));

//		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( ( ushort)( otherUserClick.idx_));
//		if( null == entity)
//			return;
//		Target = entity[0];

		Debug.Log( "AsPlayerFsm:: OnOtherUserClick: otherUserClick.idx_ " + otherUserClick.idx_);
		AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId( otherUserClick.idx_);
		Debug.Log( "AsPlayerFsm:: OnOtherUserClick: entity = " + entity);

		if( TargetDecider.CheckOtherUserIsEnemy( entity) == false)
			Target = entity;

//		if( entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == true)
//		{
			m_CurrentFsmState.MessageProcess( _msg);
//		}
	}

	void OnNpcClick( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
//		if( null != AsHudDlgMgr.Instance && ( true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg))
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCollectClick( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCollectInfo( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCollectResult( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnObjectClick( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	#region - exception -
	void OnRecoverState( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.IDLE);
	}
	#endregion

	#region - death by other reason -
	void OnDeath( AsIMessage _msg)
	{
		StartCoroutine( DeathProcess( _msg));
	}
	#endregion

	#region - shop -
	void OnOpenPrivateShop( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.PRIVATESHOP, _msg);
	}

	void OnClosePrivateShop( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	#endregion

	void OnProduct( AsIMessage _msg)
	{
		SetPlayerFsmState( ePlayerFsmStateType.PRODUCT);
		//m_CurrentFsmState.MessageProcess( _msg);
	}

	#region - emotion -
	void OnEmotionIndicate( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);

//		if( Entity.GetProperty<bool>( eComponentProperty.COMBAT) == true)
//			return;
//
//		Msg_EmotionIndicate emotion = _msg as Msg_EmotionIndicate;
//		eCLASS __class = Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//		eGENDER gender = Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
//
//		int action = -1;
//
//		switch( __class)
//		{
//		case eCLASS.DIVINEKNIGHT:
//			if( gender == eGENDER.eGENDER_MALE)
//				action = emotion.record_.DivineKnightAction_Male;
//			else if( gender == eGENDER.eGENDER_FEMALE)
//				action = emotion.record_.DivineKnightAction_Female;
//			break;
//		case eCLASS.CLERIC:
//			if( gender == eGENDER.eGENDER_MALE)
//				action = emotion.record_.ClericAction_Male;
//			else if( gender == eGENDER.eGENDER_FEMALE)
//				action = emotion.record_.ClericAction_Female;
//			break;
//		case eCLASS.MAGICIAN:
//			if( gender == eGENDER.eGENDER_MALE)
//				action = emotion.record_.MagicianAction_Male;
//			else if( gender == eGENDER.eGENDER_FEMALE)
//				action = emotion.record_.MagicianAction_Female;
//			break;
//		}
	}

	void OnEmotionSeatIndicate( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnBalloonIndicate( AsIMessage _msg)
	{
		Msg_BalloonIndicate balloon = _msg as Msg_BalloonIndicate;
		string str = AsTableManager.Instance.GetTbl_String( (int)balloon.potency_.Potency_IntValue);
		
		AsChatManager.Instance.ShowChatBalloon( m_UserEntity.UniqueId, str, eCHATTYPE.eCHATTYPE_PUBLIC);
	}
	#endregion
	
	#region - auto combat -
	void OnAutoCombatOn( AsIMessage _msg)
	{
		StartCoroutine("AutoCombatTargetSearch");
	}
	
	void OnAutoCombatOff( AsIMessage _msg)
	{
		Msg_AutoCombat_Off off = _msg as Msg_AutoCombat_Off;
		
		if(off.release_ == true)
			Target = null;
		
		StopCoroutine("AutoCombatTargetSearch");
	}
	
	IEnumerator AutoCombatTargetSearch()
	{
		float interval = AsTableManager.Instance.GetTbl_GlobalWeight_Record(135).Value * 0.001f;
		
		while(true)
		{
//			m_CurrentFsmState.MessageProcess( new Msg_AutoCombat_Search());
			
			eCLASS __class = m_UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
			Tbl_Skill_Record record = AsTableManager.Instance.GetRandomBaseSkill(__class);
			
			m_CurrentFsmState.MessageProcess( new Msg_AutoCombat_Search( record.Index, AsSlotEffectManager.Instance.chargeGuage.Step));
			
			yield return new WaitForSeconds(interval);
		}
	}
	#endregion
	
	#region - for debug -
	#region - gizmo -
	void OnDrawGizmos()
	{
		if( m_Entity.ModelObject != null)
		{
			float viewDist = m_UserEntity.GetProperty<float>( eComponentProperty.VIEW_DISTANCE);
			float attDist = m_UserEntity.GetProperty<float>( eComponentProperty.ATTACK_DISTANCE);
	//		float angle = m_MonsterEntity.GetProperty<float>( eComponentProperty.VIEW_ANGLE);
			float angle = 45.0f;

			Vector3 dir = m_UserEntity.ModelObject.transform.forward * viewDist;
			Vector3 rot1 = Quaternion.AngleAxis( angle, Vector3.up) * dir;
			Vector3 rot2 = Quaternion.AngleAxis( -angle, Vector3.up) * dir;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere( m_UserEntity.ModelObject.transform.position, viewDist);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere( m_UserEntity.ModelObject.transform.position, attDist);
			Gizmos.color = Color.green;
			Gizmos.DrawLine( m_UserEntity.ModelObject.transform.position, m_UserEntity.ModelObject.transform.position + ( dir * 2.0f));

			if( 0.0f < angle)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine( m_UserEntity.ModelObject.transform.position, m_UserEntity.ModelObject.transform.position + rot1);
				Gizmos.DrawLine( m_UserEntity.ModelObject.transform.position, m_UserEntity.ModelObject.transform.position + rot2);
			}

			if( m_CurState == ePlayerFsmStateType.SKILL_HIT)
			{
				m_CurrentFsmState.OnDrawGizmos();
			}
		}
	}
	#endregion

	void OnRubMove( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	#region - deledate -
	bool TargetShopOpening()
	{
		if( Target == null || Target.ContainProperty( eComponentProperty.SHOP_OPENING) == false)
			return false;
		else
			return Target.GetProperty<bool>( eComponentProperty.SHOP_OPENING);
	}
	#endregion

	public void ShowPoint( Vector3 _pos)
	{
		Color color = Color.Lerp( Color.blue, Color.yellow, UnityEngine.Random.Range( 0f, 1f));

		GameObject obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
		obj.transform.position = _pos;
		obj.renderer.material.color = color;

		List<GameObject> listObj = new List<GameObject>();
		listObj.Add( obj);

		StartCoroutine( PathLife( listObj));
	}

	IEnumerator PathLife( List<GameObject> _listObj)
	{
		yield return new WaitForSeconds( 5);

		foreach( GameObject obj in _listObj)
		{
			Destroy( obj);
		}

		_listObj.Clear();
	}
	#endregion
	
	#region - public -
	public void SetTargetForQuestItem( AsBaseEntity _target)
	{
		Target = _target;
		m_TargetForQuestItem = true;
	}

	public void ReleaseTarget()
	{
		Target = null;
	}
	
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
	
//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(900, 600, 80, 30), "airbone") == true)
//		{
//			Msg_ConditionIndicate_AirBone airbone = new Msg_ConditionIndicate_AirBone( new Msg_CharBuff_Body( 2000));
//			m_Entity.HandleMessage( airbone);
//		}
//		
//		if(GUI.Button(new Rect(980, 600, 80, 30), "recover") == true)
//		{
//			Msg_ConditionRecover_AirBone recover = new Msg_ConditionRecover_AirBone();
//			m_Entity.HandleMessage( recover);
//		}
//	}	
}

#region - class -
//public enum eAutoCombatState {Off, On}//, Ended}

public class SuitableBasicSkill
{
	bool m_InRange = false; public bool InRange{get{return m_InRange;}}
	Tbl_Skill_Record m_Skill; public Tbl_Skill_Record Skill{get{return m_Skill;}}
	
	public SuitableBasicSkill(bool _inRange, Tbl_Skill_Record _skill)
	{
		m_InRange = _inRange;
		m_Skill = _skill;
	}
}
#endregion