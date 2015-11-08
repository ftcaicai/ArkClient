using UnityEngine;
using System.Collections;

public class AsplayerState_Ready_Collect : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {

	public AsplayerState_Ready_Collect(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.READY_COLLECT, _fsm)
	{					
		m_dicMessageTreat.Add(eMessageType.COLLECT_RESULT, OnCollectResult);
		
		
		
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
		
	}
	
	
	public override void Enter(AsIMessage _msg)
	{
		Debug.Log("AsplayerState_Ready_Collect::Enter: ");
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Idle"));
	}

	public override void Update()
	{
		if( null == m_OwnerFsm.Target)
			return;
		
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position;
		m_OwnerFsm.transform.rotation = Quaternion.Slerp(m_OwnerFsm.transform.rotation, Quaternion.LookRotation(dir), 20.0f * Time.deltaTime);
		Vector3 rot = m_OwnerFsm.transform.rotation.eulerAngles;
		rot.x = 0;rot.z = 0;
		m_OwnerFsm.transform.rotation = Quaternion.Euler(rot);
	}
	
	public override void Exit()
	{		
		if( null == m_OwnerFsm.Target)
			return;
		
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.transform.position;
		m_OwnerFsm.transform.rotation = Quaternion.LookRotation(dir.normalized);
		Vector3 rot = m_OwnerFsm.transform.rotation.eulerAngles;
		rot.x = 0;rot.z = 0;
		m_OwnerFsm.transform.rotation = Quaternion.Euler(rot);
	}
	
	void OnCollectResult(AsIMessage _msg)
	{
		Msg_CollectResult _collectInfo = _msg as Msg_CollectResult;		
		
		switch( _collectInfo.eCollectState )
		{
		case eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE:
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_CANCEL:
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_START:
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.WORK_COLLECT);
			break;		
		}	
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
}

