using UnityEngine;
using System.Collections;

public class AsObjState_Storage : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{
	public enum ePlayerComing {On, Off}
	ePlayerComing m_PlayerComing = ePlayerComing.Off;
	
    public AsObjState_Storage(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.STORAGE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnClick);
		m_dicMessageTreat.Add(eMessageType.NPC_BEGIN_DIALOG, OnNpcBeginDialog);
	}

    public override void Enter(AsIMessage _msg)
    {
       
    }
    public override void Update()
    {
		switch(m_PlayerComing)
		{
		case ePlayerComing.On:
			float distance = Vector3.Distance(m_OwnerFsm.transform.position, m_OwnerFsm.Target.transform.position);
			
//			if(distance < 3f)
//			{
//				AsHudDlgMgr.Instance.OpenStorage();
//			}
			if(distance > AsNpcFsm.s_DialogReleaseDistance)
			{				
				m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Storage_Close"));
				
				AsHudDlgMgr.Instance.CloseStorage();
				m_PlayerComing = ePlayerComing.Off;
			}
			break;
		}
    }
    public override void Exit()
    {
    }
	
	void OnClick(AsIMessage _msg)
	{
		m_PlayerComing = ePlayerComing.On;
		m_OwnerFsm.Target = AsUserInfo.Instance.GetCurrentUserEntity();
	}
	
	void OnNpcBeginDialog(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Storage_Open"));
		
		AsHudDlgMgr.Instance.OpenStorage();
	}
}