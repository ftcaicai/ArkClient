using UnityEngine;
using System.Collections;

public class AsCollectionState_Succ : AsBaseFsmState<AsCollectionFsm.eCollectionFsmStateType, AsCollectionFsm>
{

    public AsCollectionState_Succ(AsCollectionFsm _fsm)
        : base(AsCollectionFsm.eCollectionFsmStateType.SUCC, _fsm)
	{
	}

    public override void Enter(AsIMessage _msg)
    {
		if( null == m_OwnerFsm || null == m_OwnerFsm.Entity || null == m_OwnerFsm.Entity.ModelObject )
			return;
		
      	AsEffectManager.Instance.PlayEffect("FX/Effect/Item/Fx_Collection_Finish", m_OwnerFsm.Entity.ModelObject.transform.position, false, 0 );
		
    }
    public override void Update()
    {
       
    }
    public override void Exit()
    {
    }
}
