using UnityEngine;
using System.Collections;

public class ProductionTechItemOpen : ProductionTechnologyItem
{
	public UIButton btnCashUp;	
	public UIButton btnCashLevelUp;	
	public UIProgressBar gaugeBar;
	public SpriteText textExpShow;
	public SpriteText textLevelUp;
	public SpriteText textCashCost;
	
	private int m_SphereCost = 0;
	
	System.Text.StringBuilder m_sbExpShow = new System.Text.StringBuilder();
	
	public void SendCashLevelUp()
	{
        if ((long)m_SphereCost > AsUserInfo.Instance.nMiracle)
		{
			AsHudDlgMgr.Instance.CloseProductionDlg();
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();
			return;	
		}
		
		AsCommonSender.SendItemProductCashLevelUp( getProductTechType );
	}
	
	private void CashLevelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{				
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
				return;	
			
			
			string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
			string strText = string.Format( AsTableManager.Instance.GetTbl_String( 265 ), GetName(getProductTechType) );		
			
			AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.CashMessageBox( (long)m_SphereCost, strTitle, strText, this, "SendCashLevelUp", 
				AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION ) ); 
			
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
			
		}
	}
	
	
	public override void Open( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, sPRODUCT_INFO _info )
	{
		if( null == _info )
		{
			Debug.LogError("ProductionTechItemOpen::Open()[ null == sPRODUCT_INFO ]");
			return;
		}
		base.Open( _eType, _info );	
		
		Tbl_Technic_Record record = AsTableManager.Instance.GetTechnicTable().GetRecord( getProductTechType, _info.nLevel );
		if( null == record )
		{
			Debug.LogError("ProductionTechItemOpen::Open()[ null == Tbl_Technic_Record ] type : " + getProductTechType + " level : " + _info.nLevel );
			return;
		}		
		
		
		int iCurExp = _info.nTotExp - record.getExp;
		int iCurMaxExp = record.getExp;		
		m_SphereCost = 0;
		
		Tbl_Technic_Record nextLevelRecord = AsTableManager.Instance.GetTechnicTable().GetRecord( getProductTechType, _info.nLevel+1 );
		if( null != nextLevelRecord )
		{				
			iCurMaxExp = nextLevelRecord.getExp - iCurMaxExp;
			
			if( nextLevelRecord.getMiracle == 0 )
			{
				Debug.LogError("ProductionTechItemOpen::Open()[nextLevelRecord.getMiracle == 0]");
				return;
			}
			
			int iTotalExp = _info.nTotExp;
			int iInfoExp = nextLevelRecord.getExp;
			
			m_SphereCost = (iInfoExp - iTotalExp) / nextLevelRecord.getMiracle + 
				((iInfoExp - iTotalExp)% nextLevelRecord.getMiracle == 0 ? 0 : 1 );
		}
		else
		{
			Tbl_Technic_Record preLevelRecord = AsTableManager.Instance.GetTechnicTable().GetRecord( getProductTechType, _info.nLevel-1 );
			if( null != preLevelRecord )
			{
				iCurMaxExp = iCurMaxExp - preLevelRecord.getExp;				
				iCurExp = _info.nTotExp - preLevelRecord.getExp;
			}
		}
		
		
	
		textCashCost.Text = m_SphereCost.ToString();
		//textExpShow.Text = AsTableManager.Instance.GetTbl_String( 1014 ) + " " + iCurExp.ToString() + "/" + iCurMaxExp.ToString();
		m_sbExpShow.Append( AsTableManager.Instance.GetTbl_String( 1014 ) );
		m_sbExpShow.Append(" ");
		m_sbExpShow.Append(iCurExp);
		m_sbExpShow.Append("/");
		m_sbExpShow.Append(iCurMaxExp);
		textExpShow.Text =m_sbExpShow.ToString();
		
		
		if( 0 != iCurMaxExp )
			gaugeBar.Value = (float)iCurExp / (float)iCurMaxExp;
		
		int iMaxLevel = AsTableManager.Instance.GetTechnicTable().GetMaxLevel( (int)_eType );
		if( iMaxLevel <= _info.nLevel )
		{
			
			btnCashUp.controlIsEnabled = false;
			btnCashLevelUp.controlIsEnabled = false;				
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		textLevelUp.Text = AsTableManager.Instance.GetTbl_String(1015);
		btnCashLevelUp.SetInputDelegate(CashLevelBtnDelegate);
		btnCashUp.SetInputDelegate(CashLevelBtnDelegate);		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
