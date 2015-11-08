using UnityEngine;
using System.Collections;

public class AsPlayerState_ObjStepping : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	ObjStepping m_ObjStepping = null;
	AsNpcEntity m_ObjEntity = null;
	
//	AsCharJump m_CharJump = new AsCharJump();
	
	string m_strJump = "Jump";
	string m_strIdle = "LeafStand";
	
	AnimationClip m_AnimationClip = null;
	AudioSource audioSource = null;
	
    public AsPlayerState_ObjStepping(AsPlayerFsm _fsm)
        : base(AsPlayerFsm.ePlayerFsmStateType.OBJ_STEPPING, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.OBJ_STEPPING_MSG, OnSteppingMsgProc);
		m_dicMessageTreat.Add(eMessageType.JUMP_STOP, OnJumpEnd);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}


    public override void Enter(AsIMessage _msg)
    {
        m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationClipReceiver(m_strJump, SetClip));
		audioSource = AsSoundManager.Instance.PlaySound( "S0062_EFF_Jump", m_OwnerFsm.UserEntity.ModelObject.transform.position, true);
    }

    public override void Update()
    {		
        if( null != m_ObjStepping )
		{
			if( ObjStepping.eSTEPPIG_STATE.JUMP_START == m_ObjStepping.GetSteppingState() )		
			{
				JumpStart();
			}			
		}
    }

    public override void Exit()
    {
        AsSoundManager.Instance.StopSound( audioSource);
		m_OwnerFsm.Entity.HandleMessage(new Msg_CombatBegin());
    }

//    public override void MessageProcess(AsIMessage _msg)
//    {
//        switch (_msg.MessageType)
//        {
//		case eMessageType.OBJ_STEPPING_MSG:
//			SteppingMsgProc(_msg);
//			break;
//			
//		case eMessageType.JUMP_STOP:
//			JumpEnd();
//			break;
//		case eMessageType.ANIMATION_END:
//			AnimationEnd(_msg );
//			break;
//        }      
//    }
	
			
	protected void OnAnimationEnd( AsIMessage _msg )
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_strJump )
		{
			m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate(m_strIdle));
		}
	}
	protected void JumpStart()
	{
		if(null == m_ObjEntity)
			return;		
	
      
		Debug.Log("JumpStart()");
		
		float fTime = 0.5f;
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_AnimationIndicate(m_strJump));
		if( null != m_AnimationClip )
		{
			fTime = m_AnimationClip.length;	
			Debug.Log("jump Animatono time = " +fTime);
		}
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_Jump( m_ObjEntity.transform.position, fTime ));
		m_ObjStepping.SetSteppingState(ObjStepping.eSTEPPIG_STATE.LANDING);
		//m_CharJump.StartJump(m_ObjEntity.transform.position );
		
	}
	
	protected void OnJumpEnd(AsIMessage _msg)
	{
		Debug.Log("JumpEnd()");
		m_ObjEntity.HandleMessage( new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LANDING) );	
		
		if( true == ObjSteppingMgr.Instance.IsEndObject(m_ObjStepping, m_ObjEntity) )
		{
			m_ObjStepping.SetSteppingState(ObjStepping.eSTEPPIG_STATE.IDLE);
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
			m_ObjEntity.HandleMessage( new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.IDLE) );
            AsEntityManager.Instance.UserEntity.linkIndex_ = 0;
		}
		else
		{
			m_ObjStepping.SetSteppingState(ObjStepping.eSTEPPIG_STATE.LEAF_IDLE);
			//m_ObjEntity.HandleMessage( new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LEAF_IDLE) );
		}			
	}

	protected void OnSteppingMsgProc(AsIMessage _msg)
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
			if( null != m_ObjEntity)
			{				
				m_ObjEntity.HandleMessage( new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.LEAF_IDLE) );
			}
			m_ObjStepping = ObjSteppingMgr.Instance.GetObjStepping( objSteppingMsg.m_AsObjectEntity.GetProperty<int>(eComponentProperty.GROUP_INDEX) );
			if( null == m_ObjStepping )
				Debug.LogError("null == m_ObjStepping");
			m_ObjStepping.SetSteppingState(ObjStepping.eSTEPPIG_STATE.JUMP_START); 
			m_ObjEntity = objSteppingMsg.m_AsObjectEntity;
			break;			
		}
	}
	
	void SetClip(AnimationClip _clip)
	{
		m_AnimationClip = _clip;
	}
}
