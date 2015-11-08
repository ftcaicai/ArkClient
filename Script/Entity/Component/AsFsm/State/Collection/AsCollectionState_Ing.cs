using UnityEngine;
using System.Collections;

public class AsCollectionState_Ing : AsBaseFsmState<AsCollectionFsm.eCollectionFsmStateType, AsCollectionFsm>
{		
	Msg_CollectInfo m_collectInfo;
		
    public AsCollectionState_Ing(AsCollectionFsm _fsm)
        : base(AsCollectionFsm.eCollectionFsmStateType.ING, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.COLLECT_START, OnCollectStart);
		m_dicMessageTreat.Add(eMessageType.COLLECT_INFO, OnCollectInfo);
	}

    public override void Enter(AsIMessage _msg)
    {
		Debug.Log("AsCollectionState_Ing::Enter: ");
		
		if( null == _msg )
		{
			Debug.LogError("AsCollectionState_Ing::Enter()[ null == _msg ]");
			return;
		}
		m_collectInfo = _msg as Msg_CollectInfo;
		
		
		if( true == IsPlayer() )
		{		
      		float iCMaxHp =  m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.HP_MAX);
			m_OwnerFsm.Entity.SetProperty(eComponentProperty.HP_CUR, iCMaxHp );
		}
    }
	
	
	
    public override void Update()
    {
		if( true == IsPlayer() )
		{		
	       	float iCurHp = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.HP_CUR);
			iCurHp -= Time.deltaTime;
			
			m_OwnerFsm.Entity.SetProperty( eComponentProperty.HP_CUR, iCurHp );	
			
			if( 0 >= iCurHp )
			{
				AsCommonSender.SendCollectRequest( m_OwnerFsm.CollectionEntity.SessionId, eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE );				
			}
			
			if( null != AsHUDController.Instance.targetInfo.getCurTargetEntity)
			{
				if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == m_OwnerFsm.CollectionEntity.GetInstanceID())
					AsHUDController.Instance.targetInfo.UpdateCollectionSimpleInfo( m_OwnerFsm.CollectionEntity );
//					AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( m_OwnerFsm.CollectionEntity );
			}
		}
    }
	
    public override void Exit()
    {
		if( true == IsPlayer() )
		{		
			float iCMaxHp =  m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.HP_MAX);
			m_OwnerFsm.Entity.SetProperty(eComponentProperty.HP_CUR, iCMaxHp );
			if( null != AsHUDController.Instance.targetInfo.getCurTargetEntity)
			{
				if( AsHUDController.Instance.targetInfo.getCurTargetEntity.GetInstanceID() == m_OwnerFsm.CollectionEntity.GetInstanceID())
					AsHUDController.Instance.targetInfo.UpdateCollectionSimpleInfo( m_OwnerFsm.CollectionEntity );
//					AsHUDController.Instance.targetInfo.UpdateTargetSimpleInfo( m_OwnerFsm.CollectionEntity );
			}
		}
    }
	
	private bool IsPlayer()
	{
		if( null == m_collectInfo )
			return false;
		
		if( m_collectInfo.collectInfo.nCollectorUniqKey != AsUserInfo.Instance.SavedCharStat.uniqKey_ )
			return false;
		
		return true;
	}
	
	
	void OnCollectStart(AsIMessage _msg)
	{
		Msg_StartCollect _collectStart = _msg as Msg_StartCollect;
		
		if( false == IsPlayer() )
		{		
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(280), eCHATTYPE.eCHATTYPE_SYSTEM);
			_collectStart.playerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE );		
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
			m_OwnerFsm.SetCollectionFsmState( AsCollectionFsm.eCollectionFsmStateType.IDLE );
			break;
		case eCOLLECT_STATE.eCOLLECT_STATE_START:
			Debug.LogError("error AsCollectionState_Ing::OnCollectInfo()[eCOLLECT_STATE.eCOLLECT_STATE_START]");
			break;		
		}	
	}
}