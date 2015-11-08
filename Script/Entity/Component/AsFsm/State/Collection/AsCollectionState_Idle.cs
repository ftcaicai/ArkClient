using UnityEngine;
using System.Collections;

public class AsCollectionState_Idle : AsBaseFsmState<AsCollectionFsm.eCollectionFsmStateType, AsCollectionFsm>
{

    public AsCollectionState_Idle(AsCollectionFsm _fsm)
        : base(AsCollectionFsm.eCollectionFsmStateType.IDLE, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.COLLECT_START, OnCollectStart);
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
		
	}

    public override void Enter(AsIMessage _msg)
    {
		float iMax = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.HP_MAX );
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.HP_CUR, iMax );
    }
    public override void Update()
    {
       
    }
    public override void Exit()
    {
    }
	
	void OnCollectStart(AsIMessage _msg)
	{
		Msg_StartCollect _collectStart = _msg as Msg_StartCollect;
		
		
		eITEM_PRODUCT_TECHNIQUE_TYPE _type = (eITEM_PRODUCT_TECHNIQUE_TYPE)m_OwnerFsm.CollectionEntity.GetProperty<int>(eComponentProperty.COLLECTOR_TECHNIC_TYPE);
		int iLevel = m_OwnerFsm.CollectionEntity.GetProperty<int>(eComponentProperty.LEVEL);
		
		if( false == AsUserInfo.Instance.IsEnableProductTechniqueType( _type, iLevel, true ) )
		{
			_collectStart.playerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);		
			return;
		}
		
		
		
		if( true == AsCommonSender.SendCollectRequest( m_OwnerFsm.CollectionEntity.SessionId, eCOLLECT_STATE.eCOLLECT_STATE_START ) )
		{
			_collectStart.playerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.READY_COLLECT);		
		}
		else
		{
			_collectStart.playerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);	
		}
	}
	
	void OnCollectInfo(AsIMessage _msg)
	{
		Msg_CollectInfo _collectInfo = _msg as Msg_CollectInfo;		
		
		switch( (eCOLLECT_STATE)_collectInfo.collectInfo.eCollectState )
		{
		case eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE:				
			m_OwnerFsm.SetCollectionFsmState( AsCollectionFsm.eCollectionFsmStateType.SUCC );
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_CANCEL:
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_START:
			m_OwnerFsm.SetCollectionFsmState( AsCollectionFsm.eCollectionFsmStateType.ING, _msg );
			break;		
		}	
	}
}
