using UnityEngine;
using System.Collections;

public class AsPlayerState_Run_OtherUser : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	Msg_MoveBase m_MoveInfo = null;
	
	public AsPlayerState_Run_OtherUser(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_END_INFORM, OnMoveEnd);
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginMove);
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
		OnOtherUserClick(_msg);
		CheckDistance();
		
		m_Timer = Time.time;
	}

	public override void Update()
	{
		RefreshPosition();
		CheckDistance();
	}
	
	public override void Exit()
	{
//		AsCommonSender.SendMove(eMoveType.Sync_Move, m_OwnerFsm.transform.position,
//			m_OwnerFsm.transform.position);//stop
		
//		AsSoundManager.Instance.StopSound( effSound);
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - enter -
	void CheckCombat()
	{
//		bool combat = m_OwnerFsm.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
//		float moveSpeed = m_OwnerFsm.UserEntity.GetProperty<float>(eComponentProperty.MOVE_SPEED);
//		eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
//		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
//		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
//		float animSpeed = moveSpeed / classRecord.MoveSpeed * 100;
		
		bool combat = m_OwnerFsm.UserEntity.GetProperty<bool>(eComponentProperty.COMBAT);
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
	
	void MoveProcess(AsUserEntity _otherUser)
	{
		_MoveProcess(_otherUser.transform.position);
	}
	
	void MoveProcess(AsNpcEntity _npc)
	{
		_MoveProcess(_npc.transform.position);
	}
	
	void _MoveProcess(Vector3 _pos)
	{
		Msg_MoveInfo moveInfoMsg = new Msg_MoveInfo(_pos);
		m_OwnerFsm.UserEntity.HandleMessage(moveInfoMsg);
		
		if(AsNetworkMessageHandler.Instance != null)
		{
            //AsCommonSender.SendMove(eMoveType.Normal, m_OwnerFsm.transform.position, moveInfoMsg.targetPosition_);                   
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, moveInfoMsg.targetPosition_);
		}
		
		m_MoveInfo = moveInfoMsg;
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
	
	void CheckDistance()
	{
		if(m_OwnerFsm.Target == null)
		{
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			return;
		}
			
		float dist = Vector3.Distance(m_OwnerFsm.Target.transform.position, m_OwnerFsm.transform.position);
		if(dist < AsNpcFsm.s_DialogEnableDistance)
		{
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
			{
				AsUserEntity entity = m_OwnerFsm.Target as AsUserEntity;
				AsPStoreManager.Instance.Request_Enter(entity.ShopUId, entity.GetProperty<string>(eComponentProperty.NAME), entity.shopContent);
			}
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
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
		Msg_OtherUserClick click = _msg as Msg_OtherUserClick;
		
		AsUserEntity otherUser = AsEntityManager.Instance.GetUserEntityByUniqueId(click.idx_);
		if(otherUser != null)
		{
			m_OwnerFsm.Target = otherUser;
			MoveProcess(otherUser);
		}
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		Msg_NpcClick click = _msg as Msg_NpcClick;
		
		AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId(click.idx_);
		if(npc != null)
		{
			m_OwnerFsm.Target = npc;
			MoveProcess(npc);
		}
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
		m_OwnerFsm.PlayAction_Run();
		
//		float animSpeed = GetAnimationSpeedByMove_UserEntity("Run");
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate("Run"));
	}
	#endregion
}

