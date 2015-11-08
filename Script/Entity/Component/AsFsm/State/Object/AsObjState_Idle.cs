using UnityEngine;
using System.Collections;

public class AsObjState_Idle : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{

    public AsObjState_Idle(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.IDLE, _fsm)
	{
	}

    public override void Enter(AsIMessage _msg)
    {
       
    }
    public override void Update()
    {
       
    }
    public override void Exit()
    {
    }
}
