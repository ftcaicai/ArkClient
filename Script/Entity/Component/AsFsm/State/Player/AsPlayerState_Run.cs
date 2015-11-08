using UnityEngine;
using System.Collections;

public class AsPlayerState_Run : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	Msg_MoveBase m_MoveInfo;
//	private AudioSource effSound = null;
	
	public AsPlayerState_Run(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.RUN, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnMoveEnd);
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
		m_dicMessageTreat.Add(eMessageType.CONDITION_BINDING, OnBinding);
		
		m_dicMessageTreat.Add(eMessageType.RELEASE_TENSION, OnReleaseTension);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_CombatEnd());
		CheckCombat();
		
		//walk order
		OnBeginMove(_msg);
		
		m_Timer = Time.time;
		//m_OwnerFsm.InvokeRepeating("PositionIndication", 1, 1);
	}

	public override void Update()
	{
		RefreshPosition();
	}
	
	public override void Exit()
	{
//		AsCommonSender.SendMove(eMoveType.Sync_Move, m_OwnerFsm.transform.position,
//			m_OwnerFsm.transform.position);//stop
		
		m_OwnerFsm.ReleaseElements();
		
		AsInputManager.Instance.AutoMoveStopped();
	}
	#endregion
	
	#region - enter -
	void CheckCombat()
	{
		bool combat = m_OwnerFsm.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
		
//		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
		
		
		
		if(combat == true && m_OwnerFsm.WeaponEquip == true)
		{
			m_OwnerFsm.PlayAction_BattleRun();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("BattleRun");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("BattleRun", 0.1f, animSpeed));
		}
		else
		{
			m_OwnerFsm.PlayAction_Run();
			
			float animSpeed = GetAnimationSpeedByMove_UserEntity("Run");
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));
		}
	}
	
	void MoveProcess(AsIMessage _msg)
	{
		switch(_msg.MessageType)
		{
		case eMessageType.INPUT_MOVE:
			
			Msg_Input_Move move = _msg as Msg_Input_Move;
			
			Msg_MoveInfo moveInfoMsg = new Msg_MoveInfo(move.worldPos_);
			m_OwnerFsm.UserEntity.HandleMessage(moveInfoMsg);
			
			if(AsNetworkMessageHandler.Instance != null)
				m_OwnerFsm.MovePacketRefresh(eMoveType.Normal, m_OwnerFsm.transform.position, move.worldPos_);
//	            AsCommonSender.SendMove(eMoveType.Normal, m_OwnerFsm.transform.position, move.worldPos_);                   
			
			m_MoveInfo = moveInfoMsg;
			
			break;
		case eMessageType.INPUT_AUTO_MOVE:
			
			Msg_Input_Auto_Move auto = _msg as Msg_Input_Auto_Move;
			
			Msg_AutoMove autoMoveMsg = new Msg_AutoMove(m_OwnerFsm.transform.position, auto.worldPos_);
			m_OwnerFsm.UserEntity.HandleMessage(autoMoveMsg);
			
			if(AsNetworkMessageHandler.Instance != null)
				m_OwnerFsm.MovePacketRefresh(eMoveType.Auto, m_OwnerFsm.transform.position, auto.worldPos_);                   
//	            AsCommonSender.SendMove(eMoveType.Auto, m_OwnerFsm.transform.position, auto.worldPos_);                   
			
			m_MoveInfo = autoMoveMsg;
			
			break;
		}
	}
	 #endregion
	
	#region - update -
	float m_Timer = 1;
	float m_Interval = 1;
	void RefreshPosition()
	{
		if(Time.time - m_Timer > m_Interval)
		{
			m_Timer = Time.time;
//			AsCommonSender.SendMove(eMoveType.Normal,
//				m_OwnerFsm.transform.position, m_MoveInfo.GetTargetPosition(m_OwnerFsm.transform.position));
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal,
				m_OwnerFsm.transform.position, m_MoveInfo.GetTargetPosition(m_OwnerFsm.transform.position));
		}
	}
	#endregion
	
	#region - msg -
	void OnMoveEnd(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	
	void OnBeginMove(AsIMessage _msg)
	{
		MoveProcess(_msg);
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
		Msg_OtherUserClick otherUserClick =  _msg as Msg_OtherUserClick;
		AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId(otherUserClick.idx_);	
		if(entity.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
		{
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
		}
		else
		{
			if(entity.GetProperty<bool>(eComponentProperty.LIVING) == true)
			{			
				if(TargetDecider.CheckOtherUserIsEnemy(entity) == true)
				{
					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
				}
			}
		}
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
	}
	
	void OnCollectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT, _msg);		
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
	}
	
	void OnBeginDash(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
	}
	
	void OnCheatDeath( AsIMessage _msg)
	{
		m_OwnerFsm.CheatDeath( _msg);
	}
	
	void OnBinding(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, _msg);
	}
	#endregion
	#region - release tension -
	void OnReleaseTension(AsIMessage _msg)
	{
//		#region - anim speed -
//		float moveSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
//		#endregion
		
		m_OwnerFsm.PlayAction_Run();
		
		float animSpeed = GetAnimationSpeedByMove_UserEntity("Run");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Run", 0.1f, animSpeed));
	}
	#endregion
}

