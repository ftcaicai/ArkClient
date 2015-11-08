using UnityEngine;
using System.Collections;

public class AsPlayerState_Work_Collect : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	#region - member -
	public enum eState {Ready, Hit, Finish}
//	eState m_State = eState.Ready;
	
	Msg_MoveBase m_MoveInfo;
	#endregion
	#region - init -
	public AsPlayerState_Work_Collect(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.WORK_COLLECT, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
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
		m_dicMessageTreat.Add(eMessageType.COLLECT_RESULT, OnCollectResult);		
		m_dicMessageTreat.Add(eMessageType.CONDITION_BINDING, OnBinding);
	}
	
	public override void Init ()
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Collection");
	}
	#endregion
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		Debug.Log("AsPlayerState_Work_Collect::Enter: ");
		
//		m_State = eState.Ready;
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Hit);
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.HitAnimation.FileName));		
		
		m_OwnerFsm.UserEntity.ShowCollectImg(true);
	}

	public override void Update()
	{
	
	}
	
	public override void Exit()
	{	
		m_OwnerFsm.UserEntity.ShowCollectImg(false);
	}
	#endregion
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
//		switch(m_State)
//		{
//		case eState.Ready:
//			BeginHitState();
//			break;
//		case eState.Hit:
////			BeginFinishState();
//			break;
//		case eState.Finish:
//			EndFinishState();
//			break;
//		}
	}
	
	void OnMoveEnd(AsIMessage _msg)
	{		
		
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
			if( m_OwnerFsm.Target == npc )
			{
				return;
			}			
		}
		
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
	
	void OnCollectResult(AsIMessage _msg)
	{
		Msg_CollectResult _collectInfo = _msg as Msg_CollectResult;			
	
		switch( _collectInfo.eCollectState )
		{
		case eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE:
//			BeginFinishState();
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_CANCEL:	
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);		
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_START:
			//Debug.LogError("error AsPlayerState_Work_Collect::OnCollectInfo()[eCOLLECT_STATE.eCOLLECT_STATE_START]");
			break;		
		}	
	}
	
	void OnBinding(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING, _msg);
	}
	#endregion
	#region - process -
	void BeginHitState()
	{
//		m_State = eState.Hit;
			
//		m_OwnerFsm.PlayElements(eActionStep.Hit);
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.HitAnimation.FileName));
	}
	
	void BeginFinishState()
	{
//		m_State = eState.Finish;
		
//		m_OwnerFsm.PlayElements(eActionStep.Finish);
//		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_Action.FinishAnimation.FileName));
	}
	
	void EndFinishState()
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
	}
	#endregion
}

