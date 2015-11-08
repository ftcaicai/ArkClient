using UnityEngine;
using System.Collections;

public class AsPlayerState_Wake : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	public AsPlayerState_Wake(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.WAKE, _fsm)
	{
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
////		int att = m_OwnerFsm.UserEntity.GetProperty<int>(eComponentProperty.ATTRIBUTE);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
	}
	#endregion
	#region - process -
	 #region - input -
	void InputProcess(Msg_Input _msg)
	{
		switch(_msg.type_)
		{
		case eInputType.SINGLE_UP:
//			if(_msg.worldPosition_ != Vector3.zero)
//			{
//				if(_msg.inputObject_.layer == LayerMask.NameToLayer("Terrain"))
//				{
//					m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
//				}
//			}
			break;
		}
	}
	 #endregion
	#endregion
}
