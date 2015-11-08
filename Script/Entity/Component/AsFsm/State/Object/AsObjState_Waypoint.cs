using UnityEngine;
using System.Collections;

public class AsObjState_Waypoint : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{
	Tbl_WarpData_Record m_warpDataRecord;
	ParticleSystem[] m_particleList;
	bool m_isActive = false;
	bool m_isUpdate = false;
	
    public AsObjState_Waypoint(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.WAYPOINT, _fsm)
	{
		//m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);	
		m_dicMessageTreat.Add(eMessageType.NPC_BEGIN_DIALOG, OnNpcClick);	
		
	}
	
	private void SetActiveAction( bool isActive )
	{		
		if( isActive == m_isActive )
			return;
		
		m_isActive = isActive;
		if( true == isActive )
		{
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("WayPoint"));			
			
			if( null != m_particleList )				
			{				
				foreach( ParticleSystem _ps in m_particleList )
				{
					_ps.gameObject.active = true;
				}
			}
		}
		else
		{			
			if( null != m_particleList )				
			{
				
				foreach( ParticleSystem _ps in m_particleList )
				{
					_ps.gameObject.active = false;
				}
			}
		}
		
		
	}
	

    public override void Enter(AsIMessage _msg)
    {		
		int iCurMapId = TerrainMgr.Instance.GetCurMapID();
		m_warpDataRecord = AsTableManager.Instance.GetWarpDataIdxToUseMapID( iCurMapId );
		if( null == m_warpDataRecord )
		{
			Debug.LogError("AsObjState_Waypoint::Enter()[ null == warpDataRecord ] id : " + iCurMapId);
			return;
		}      
    }
    public override void Update()
    {   		
		if( false == m_isUpdate )
		{
			m_particleList = m_OwnerFsm.transform.GetComponentsInChildren<ParticleSystem>();
			
			if( null != m_particleList && 0 < m_particleList.Length )
			{
				foreach( ParticleSystem _ps in m_particleList )
					_ps.gameObject.active = false;
			}	
			
			m_isUpdate = true;
		}
		else if( false == m_isActive )
		{
			if( AsUserInfo.Instance.IsExistWaypoint( m_warpDataRecord.Index ) )
			{
				SetActiveAction(true);
			}
			else
			{
				SetActiveAction(false);
			}	
		}
		
    }
    public override void Exit()
    {
		
    }
	
	void OnNpcClick(AsIMessage _msg)
	{
		if( true == AsHudDlgMgr.Instance.IsWaypointMapDlg )
		{
			return;
		}
		
		if( false == AsUserInfo.Instance.IsLiving() )
			return;			
		
		if( null == m_warpDataRecord )
		{			
			return;
		}
		
		
		/*if( false == m_warpDataRecord.isActive )
		{
			if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
			{
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( string.Empty, AsTableManager.Instance.GetTbl_String(873),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
			}
			return;
		}*/
		
		if( true == m_isActive )
		{
			AsHudDlgMgr.Instance.OpenWaypointMapDlg();
			return;
		}
	
		AsUserInfo.Instance.ClickWaypointNpc( m_warpDataRecord.Index );		
	}
				
	
}