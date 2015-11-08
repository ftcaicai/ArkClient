using UnityEngine;
using System.Collections;

public class AsObjState_Stepping : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{
	
	Tbl_Object_Record m_objRecord;
	
    public AsObjState_Stepping(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.STEPPING_LEAF, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.OBJ_STEPPING_MSG, SteppingMsgProc);
	}

    public override void Enter(AsIMessage _msg)
    {
        m_objRecord = AsTableManager.Instance.GetTbl_Object_Record(m_OwnerFsm.ObjectEntity.TableIdx);
        if (null == m_objRecord)
        {
            Debug.LogError("null == m_objRecord");
            return;            
        }
    }
    public override void Update()
    {
       
    }
    public override void Exit()
    {
    }

	protected void SteppingMsgProc(AsIMessage _msg)
	{
		Msg_ObjStepping objSteppingMsg = _msg as Msg_ObjStepping;
		if(null == objSteppingMsg)
		{
			Debug.LogError("null == objSteppingMsg");
			return;
		}
		
		
		switch( objSteppingMsg.m_eSteppingState )
		{
		case ObjStepping.eSTEPPIG_STATE.JUMP_START:			
			break;	
		case ObjStepping.eSTEPPIG_STATE.LANDING:
			if(string.Compare("NONE", m_objRecord.StartActionAni, true) != 0)
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_objRecord.StartActionAni));
			break;				
		case ObjStepping.eSTEPPIG_STATE.LEAF_IDLE:	
			if(string.Compare("NONE", m_objRecord.DoActionAni, true) != 0)
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(m_objRecord.DoActionAni));
			break;	
		}
	}
}
