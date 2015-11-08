using UnityEngine;
using System.Collections;

public class AsOtherUserState_ObjStepping : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> 
{
    ObjStepping m_ObjStepping = null;
    AsNpcEntity m_ObjEntity = null;
	
	string m_strJump = "Jump";
	AnimationClip m_AnimationClip = null;
	

    public AsOtherUserState_ObjStepping(AsOtherUserFsm _fsm)
        : base(AsOtherUserFsm.eOtherUserFsmStateType.OBJ_STEPPING, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.JUMP_STOP, OnJumpStopMsgProc);
		m_dicMessageTreat.Add(eMessageType.OBJ_STEPPING_MSG, OnObjSteppingMsgProc);
	}
	
	
	public override void Enter(AsIMessage _msg)
	{		
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationClipReceiver(m_strJump, SetClip));
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
	}
	
    protected void OnJumpStopMsgProc( AsIMessage _msg )
    {
        m_ObjEntity.HandleMessage(new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LANDING));

        if (true == ObjSteppingMgr.Instance.IsEndObject(m_ObjStepping, m_ObjEntity))
        {
            m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE);
            m_ObjEntity.HandleMessage(new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.IDLE));            
        }
        else
        {           
           // m_ObjEntity.HandleMessage(new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LEAF_IDLE));
        }	
    }

    protected void OnObjSteppingMsgProc(AsIMessage _msg)
    {
        Msg_ObjStepping objSteppingMsg = _msg as Msg_ObjStepping;
        if (null == objSteppingMsg)
        {
            Debug.LogError("null == objSteppingMsg");
            return;
        }
		
		float fTime = 0.5f;
        switch (objSteppingMsg.m_eSteppingState)
        {
            case ObjStepping.eSTEPPIG_STATE.JUMP_START:
				if( null != m_ObjEntity )
				{
					m_ObjEntity.HandleMessage( new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LEAF_IDLE) );
				}
                m_ObjStepping = ObjSteppingMgr.Instance.GetObjStepping(objSteppingMsg.m_AsObjectEntity.GetProperty<int>(eComponentProperty.GROUP_INDEX));
                m_ObjEntity = objSteppingMsg.m_AsObjectEntity;
                				
				m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_AnimationIndicate(m_strJump));
				if( null != m_AnimationClip )
				{
					fTime = m_AnimationClip.length;	
					Debug.Log("jump Animatono time = " +fTime);
				}				
			
				m_OwnerFsm.OtherUserEntity.HandleMessage(new Msg_Jump(objSteppingMsg.m_AsObjectEntity.transform.position, fTime));
                break;
        }
    }
	
	void SetClip(AnimationClip _clip)
	{
		m_AnimationClip = _clip;
	}
}