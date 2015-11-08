using UnityEngine;
using System.Collections;



	
public class AsPlayerState_Run_Collect : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	Msg_MoveBase m_MoveInfo;
	
	public AsPlayerState_Run_Collect(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT, _fsm)
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
		Debug.Log("AsPlayerState_Run_Collect::Enter: ");
			
		m_OwnerFsm.Entity.HandleMessage(new Msg_CombatEnd());
		CheckCombat();
		
		//walk order
		OnCollectClick(_msg);
		CheckDistance();
		
		m_Timer = Time.time;
		//m_OwnerFsm.InvokeRepeating("PositionIndication", 1, 1);
	}

	public override void Update()
	{
		RefreshPosition();
		CheckDistance();
		
//		AsSoundManager.Instance.UpdatePosition( effSound, m_OwnerFsm.UserEntity.ModelObject.transform.position);
	}
	
	public override void Exit()
	{
//		AsCommonSender.SendMove(eMoveType.Sync_Move, m_OwnerFsm.transform.position,
//			m_OwnerFsm.transform.position);//stop
//		m_OwnerFsm.MovePacketRefresh( eMoveType.Sync_Move, m_OwnerFsm.transform.position,
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
	
	void MoveProcess(AsNpcEntity _npc)
	{
		Msg_MoveInfo moveInfoMsg = new Msg_MoveInfo(_npc.transform.position);
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
		if( IsCheckDistance() )
		{
//            m_OwnerFsm.Entity.HandleMessage(new Msg_ZoneWarp());
			m_OwnerFsm.MovePacketSynchronize();
			m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		}
	}
	#endregion
	
	private bool IsCheckDistance()
	{
		if( null == m_OwnerFsm.Target )
		{
			//m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			return false;
		}
		
		float dist = Vector3.Distance(m_OwnerFsm.Target.transform.position, m_OwnerFsm.transform.position);
		if( dist < AsNpcFsm.s_CollectEnableDistance )
		{
			return true;
		}
		
		return false;
	}
	
	#region - msg -
	void OnMoveEnd(AsIMessage _msg)
	{			
		MoveEndAction();		
		
	}
	
	private void MoveEndAction()
	{
		//AsCommonSender.SendMove(eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		
		if( null == m_OwnerFsm.Target )
		{
			m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.IDLE );	
			return;
		}
		
		if( eFsmType.COLLECTION != m_OwnerFsm.Target.FsmType )
		{
			m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.IDLE );	
			return;
		}		
		
		m_OwnerFsm.Target.HandleMessage( new Msg_StartCollect( m_OwnerFsm ) );	
	}
	
	void OnBeginMove(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
//		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
	}
	
	void OnCollectClick(AsIMessage _msg)
	{
		Msg_CollectClick click = _msg as Msg_CollectClick;
		
		
		AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId(click.idx_);
		if(npc != null)
		{
			if( null != AsHUDController.Instance.targetInfo.getCollectEntity )
			{
				if( npc == AsHUDController.Instance.targetInfo.getCollectEntity )
				{
					if( IsCheckDistance() )
					{
						MoveEndAction();
						return;
					}					
				}
			}
		
			m_OwnerFsm.Target = npc;		
			MoveProcess(npc);
		}
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

