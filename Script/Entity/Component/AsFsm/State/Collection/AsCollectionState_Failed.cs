using UnityEngine;
using System.Collections;

public class AsCollectionState_Failed : AsBaseFsmState<AsCollectionFsm.eCollectionFsmStateType, AsCollectionFsm>
{

    public AsCollectionState_Failed(AsCollectionFsm _fsm)
        : base(AsCollectionFsm.eCollectionFsmStateType.FAILED, _fsm)
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