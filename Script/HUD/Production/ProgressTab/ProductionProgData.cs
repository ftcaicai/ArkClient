using UnityEngine;
using System.Collections;

public class ProductionProgData 
{	
	private byte m_nProductSlot;
	private sPRODUCT_SLOT m_ServerData;
	private Tbl_Production_Record m_record;
	
	private float m_fStartRealTime = 0f;
	private float m_remainTime = 0f;
	private bool m_bProgress = false;
	private byte m_slotIndex;
	private int  m_nPushKey;
	
	public sPRODUCT_SLOT getServerData	
	{
		get	
		{
			return m_ServerData;
		}
	}
	
	public byte getProductSlot
	{
		get
		{
			return m_nProductSlot;
		}
	}
	
	public ulong getNeedSphere
	{
		get
		{
			if( null == m_record )
				return 0;
			
			return (ulong)m_record.miracle;
		}
	}
	
	
	public int  PushKey
	{
		get	{	return m_nPushKey;	}
		set {   m_nPushKey = value; }
	}
	
	public ProductionProgData( byte _nSlot, sPRODUCT_SLOT _data, bool bProgress )
	{
		SetData( _nSlot, _data, bProgress );
	}
	
	public void SetData( byte _nSlot, sPRODUCT_SLOT _data, bool bProgress )
	{				
		m_nProductSlot = _nSlot;
		m_ServerData = _data;
		m_bProgress = bProgress;
		
				
		if( 0 != _data.nRecipeIndex )
		{
			m_record = AsTableManager.Instance.GetProductionTable().GetRecord( _data.nRecipeIndex );
			if( null == m_record )
			{
				Debug.LogError("ProductionProgData::SetData()[ null == Tbl_Production_Record] id : " + _data.nRecipeIndex );
				return;
			}
		}
		else
		{
			return;
		}		
		
		// reamin time
		m_remainTime = (float)_data.nProductTime * 0.001f;
		m_fStartRealTime = Time.realtimeSinceStartup;	
		
	
		
		
	}	
	
	public void SetProgress( bool isProgress )
	{
		m_bProgress = isProgress;	
		if( true == isProgress )
		{
			m_fStartRealTime = Time.realtimeSinceStartup; 				
		}		
	}
	
	public float GetRemainTime()
	{
		return m_remainTime;
	}
	
	public void SetReciveBtn( int iItemID, byte slot )
	{
		string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
		string strText = AsTableManager.Instance.GetTbl_String( 274 );		
		
		m_slotIndex = slot;
		
		if( false == AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
		{
			AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.ItemViewMessageBox( iItemID, string.Empty, strTitle, strText, SendProductionReceive, 
									AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION, false) ); 
				
		}
	}
	
	public void SendProductionReceive()
	{
		AsCommonSender.SendItemProductReceive(m_slotIndex);
	}
	
	
	
	public float GetValue()
	{
		if( null == m_record )
			return 0f;
			
		if( 0f == m_record.itemTime )
		{
			Debug.LogError("ProductionProgData::GetValue() record id : " + m_record.getIndex );
			return 0f;
		}
		
		float fValue = (m_record.itemTime-m_remainTime)/m_record.itemTime;
		if( 0f > fValue )
			fValue = 0f;
		return fValue;
	}
	
	public float GetCashTime()
	{
		if( null == m_record )
			return 0f;
		
		return m_remainTime * 1000f;
	}
	
	public void Update()
	{
		if( true == m_bProgress )
		{
			m_remainTime -= (Time.realtimeSinceStartup - m_fStartRealTime);
			m_fStartRealTime = Time.realtimeSinceStartup;
		}
	}

}
