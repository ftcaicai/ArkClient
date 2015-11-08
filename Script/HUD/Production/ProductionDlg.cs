using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductionDlg : MonoBehaviour 
{	
	public enum ePRODUCTION_STATE
	{
		NONE,
		TECHNOLOGY,
		LIST,
		PROGRESS,
		//PLAN,
	};
	
	public UIPanelTab technologyTab;
	public UIPanelTab listTab;
	public UIPanelTab progressTab;
	public UIButton closeBtn;	
	
	public SpriteText textTitle;
	public SpriteText textTechnologyTab;
	public SpriteText textListTab;
	public SpriteText textProgressTab;
	
	
	public ProductionListTab productionListTab;
	public ProductionProgressTab productionProgressTab;
	public ProductionTechnologyTab productionTechnologyTab;
	public ProductionPlanTab productionPlanTab;
	
	public SimpleSprite newImg;
	
	private ePRODUCTION_STATE m_eCurState = ePRODUCTION_STATE.NONE;	
	private body1_SC_ITEM_PRODUCT_INFO m_ItemProductInfo = null;
	private byte m_iTechOpenNum = 0;
	private Dictionary<byte, ProductionProgData> m_ProgInfoList = new Dictionary<byte, ProductionProgData>();
	
	
	private AsMessageBox m_messageBox;
	
	public void SetMessageBox( AsMessageBox _box )
	{
		if( null != m_messageBox )
		{
			GameObject.Destroy(m_messageBox.gameObject);
		}
		
		m_messageBox = _box;
	}
	
	public bool isOpenMessageBox	
	{
		get
		{
			if( null == m_messageBox )
				return false;
			
			return m_messageBox.gameObject.active;
		}
	}
	
	public void ClearMessageBox()
	{
		if( null != m_messageBox )	
		{
			GameObject.Destroy( m_messageBox.gameObject );
		}
	}
	
	public void ResetReceiveItem()
	{
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:			
			break;
		case ePRODUCTION_STATE.LIST:			
			productionListTab.ResetListItemInfo();
			break;
		case ePRODUCTION_STATE.PROGRESS:			
			break;
			
		/*case ePRODUCTION_STATE.PLAN:
			productionPlanTab.IsCheckReadyMaking();
			break;*/
		}
		if( productionPlanTab.gameObject.active )
			productionPlanTab.IsCheckReadyMaking();
	}

	
	public byte getTechOpenNum
	{
		get
		{
			return m_iTechOpenNum;
		}
	}
	
	public body1_SC_ITEM_PRODUCT_INFO getItemProductInfo
	{
		get
		{
			return m_ItemProductInfo;
		}
	}
	
	
	public void Open( body1_SC_ITEM_PRODUCT_INFO _data )
	{
		m_ItemProductInfo = _data;
		m_iTechOpenNum = GetTechniqueInfoCount(m_ItemProductInfo);
		ReceiveProgInfoList( m_ItemProductInfo );
		SetTab( AsHudDlgMgr.Instance.productTabIndex );

		CloseProductonPlanTab();

        if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.PRODUCTION) != null)
            AsCommonSender.SendClearOpneUI(OpenUIType.PRODUCTION);

		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.m_MainMenuDlg.Close();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_PRODUCT));

		CheckNewImg();
	}
	
	public void CheckNewImg()
	{
		if( AsUserInfo.Instance.IsHaveProductionInfoComplete())
		{
			SetNewImg( true );
		}
		else
		{
			SetNewImg( false );
		}

	}
	
	public void SetNewImg( bool _isActive )
	{
		if( null != newImg )
			newImg.gameObject.active = _isActive;
	}
	
	
	public void Close()
	{
		m_eCurState = ePRODUCTION_STATE.NONE;
		ClearMessageBox();
	}
	
	
	public byte GetCanUseProgIndex()
	{		
		
		foreach( KeyValuePair<byte, ProductionProgData> _value in m_ProgInfoList )
		{
			if( null == _value.Value )
				continue;
			
			if( null == _value.Value.getServerData )
				continue;
			
			if( 0 == _value.Value.getServerData.nRecipeIndex )
				return _value.Value.getProductSlot;
		}
		
		return byte.MaxValue;
	}
	
	// Receive
	
	public void ReceiveProgress( bool bProgress )
	{
		m_ItemProductInfo.bProgress = bProgress;
		
		if( ePRODUCTION_STATE.PROGRESS == m_eCurState )
		{
			productionProgressTab.SetPauseState( m_ItemProductInfo.bProgress );		
		}	
		
		foreach( KeyValuePair<byte, ProductionProgData> _value in m_ProgInfoList )
		{
			_value.Value.SetProgress( m_ItemProductInfo.bProgress );
		}
	}
	
	public void ReceiveProgInfoList( body1_SC_ITEM_PRODUCT_INFO _infoList )
	{
		if( null == _infoList || null == _infoList.body )
			return;
		
		foreach( body2_SC_ITEM_PRODUCT_INFO _infoData in _infoList.body )
		{
			ReceiveAddProgInfoData(_infoData.nProductSlot, _infoData.sSlotInfo );
		}		
	}
	
	public void ReceiveAddProgInfoData( byte nProductSlot, sPRODUCT_SLOT productSlot )
	{	
		if( false == m_ProgInfoList.ContainsKey( nProductSlot ) )
		{
			m_ProgInfoList.Add( nProductSlot, new ProductionProgData( nProductSlot, productSlot, getItemProductInfo.bProgress ) );			
		}
		else
		{
			m_ProgInfoList[nProductSlot].SetData( nProductSlot, productSlot, getItemProductInfo.bProgress );
		}
		
		
		
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:			
			break;
		case ePRODUCTION_STATE.LIST:			
			break;
		case ePRODUCTION_STATE.PROGRESS:	
			productionProgressTab.Open( getItemProductInfo.bProgress, getTechOpenNum, (byte)m_ProgInfoList.Count, m_ProgInfoList );
			//productionProgressTab.PlayLineEffect( (byte)(m_ProgInfoList.Count-1) );
			break;			
		//case ePRODUCTION_STATE.PLAN:			
		//	break;
		}
	}	
	
	public void ReceiveCashSlotOpen( byte nslot )
	{
		
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:			
			break;
		case ePRODUCTION_STATE.LIST:			
			break;
		case ePRODUCTION_STATE.PROGRESS:	
			productionProgressTab.PlayLineEffect( nslot );
			break;		
		}
	}
		
	
	public void ReceiveTechniqueRegister( body_SC_ITEM_PRODUCT_TECHNIQUE_INFO _data )
	{
		if( m_ItemProductInfo.sProductInfo.Length <= _data.eProductType )
		{
			Debug.LogError("ProductionDlg::SetTechniqueRegister()[ m_ItemProductInfo.sProductInfo.Length <= _data.eProductType ] eProductType : "+
				_data.eProductType );
			return;
		}
		
		m_ItemProductInfo.sProductInfo[_data.eProductType] = _data.sProductInfo;
		m_iTechOpenNum = GetTechniqueInfoCount(m_ItemProductInfo);
		
		if( ePRODUCTION_STATE.TECHNOLOGY == m_eCurState && true == productionTechnologyTab.gameObject.active )
		{
			productionTechnologyTab.Open( m_iTechOpenNum, getItemProductInfo.sProductInfo );			
		}
		
	}
	
	
	
	
	
	
	private byte GetTechniqueInfoCount( body1_SC_ITEM_PRODUCT_INFO _infolist )
	{
		if( null == _infolist )
			return 0;
		
		byte iCount = 0;
		foreach( sPRODUCT_INFO _info in _infolist.sProductInfo )
		{
			if( 0 < _info.nLevel )
			{
				++iCount;
			}
		}
		
		return iCount;
	}
	
	
	// Progress
	public void OnProgressClicked()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		SetTab( ePRODUCTION_STATE.PROGRESS );
	}
	
	public void OnListClicked()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		SetTab( ePRODUCTION_STATE.LIST );
	}
	
	// Technology
	public void OnTechnologyTabClicked()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		SetTab( ePRODUCTION_STATE.TECHNOLOGY );	
	}
	
	
	public void SetTab( ePRODUCTION_STATE eState )
	{		
		if( eState == m_eCurState)
			return;
		
		m_eCurState = eState;
		
		AsHudDlgMgr.Instance.productTabIndex = eState;
		
		technologyTab.SetState(1);
		listTab.SetState(1);
		progressTab.SetState(1);
		
		CloseListTab();
		CloseTechnologyTab();
		CloseProgressTab();		
		//CloseProductonPlanTab();
		
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:
			technologyTab.SetState(0);
			productionTechnologyTab.gameObject.SetActiveRecursively(true);
			productionTechnologyTab.Open( m_iTechOpenNum, getItemProductInfo.sProductInfo );
			break;
		case ePRODUCTION_STATE.LIST:
			listTab.SetState(0);	
			productionListTab.gameObject.SetActiveRecursively(true);
			productionListTab.Open( getItemProductInfo );
			break;
		case ePRODUCTION_STATE.PROGRESS:
			progressTab.SetState(0);	
			productionProgressTab.gameObject.SetActiveRecursively(true);
			productionProgressTab.Open( getItemProductInfo.bProgress, getTechOpenNum, (byte)m_ProgInfoList.Count, m_ProgInfoList );
			break;
			
		/*case ePRODUCTION_STATE.PLAN:
			Debug.LogError("ProductionDlg::SetTab()[ false ePRODUCTION_STATE.PLAN ");
			break;*/
		}
		
		ClearMessageBox();
	}
	
	public void OpenPlanTap( Tbl_Production_Record _record )
	{
		if( null == _record )
			return;
		
		//if( ePRODUCTION_STATE.PLAN == m_eCurState)
		//	return;
		
		//m_eCurState = ePRODUCTION_STATE.PLAN;
		
		//technologyTab.SetState(1);
		//listTab.SetState(0);
		//progressTab.SetState(1);
		
		//CloseListTab();
		//CloseTechnologyTab();
		//CloseProgressTab();	
		
		productionPlanTab.gameObject.SetActiveRecursively(true);
		productionPlanTab.Open( _record );		
	}
	
	protected void CloseListTab()
	{
		productionListTab.Close();
		productionListTab.gameObject.SetActiveRecursively(false);
	}
	
	protected void CloseProgressTab()
	{
		productionProgressTab.Close();
		productionProgressTab.gameObject.SetActiveRecursively(false);
	}
	
	protected void CloseTechnologyTab()
	{
		productionTechnologyTab.Close();
		productionTechnologyTab.gameObject.SetActiveRecursively(false);
	}
	
	public void CloseProductonPlanTab()
	{		
		productionPlanTab.Close();
		productionPlanTab.gameObject.SetActiveRecursively(false);
	}
	
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseProductionDlg();					
		}
	}

	// Use this for initialization
	void Start () 
	{	
		closeBtn.SetInputDelegate(CloseBtnDelegate);
		
		
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1292);
		textTechnologyTab.Text = AsTableManager.Instance.GetTbl_String(1003);
		textListTab.Text = AsTableManager.Instance.GetTbl_String(1004);
		textProgressTab.Text = AsTableManager.Instance.GetTbl_String(1005);
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach( KeyValuePair<byte, ProductionProgData> _value in m_ProgInfoList )
		{
			_value.Value.Update();
		}
	}
	
	
	
	//input	
	public void GuiInputDown( Ray inputRay )
	{		
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:		
			break;
		case ePRODUCTION_STATE.LIST:	
			productionListTab.GuiInputDown(inputRay);
			break;
		case ePRODUCTION_STATE.PROGRESS:	
			productionProgressTab.GuiInputDown(inputRay);
			break;
			
		/*case ePRODUCTION_STATE.PLAN:	
			productionPlanTab.GuiInputDown(inputRay);
			break;*/
		}
		
		productionPlanTab.GuiInputDown(inputRay);
	}	
		  
	public void GuiInputUp(Ray inputRay)
	{ 
		switch( m_eCurState )
		{
		case ePRODUCTION_STATE.TECHNOLOGY:		
			break;
		case ePRODUCTION_STATE.LIST:	
			productionListTab.GuiInputUp(inputRay);
			break;
		case ePRODUCTION_STATE.PROGRESS:
			productionProgressTab.GuiInputUp(inputRay);
			break;
			
		/*case ePRODUCTION_STATE.PLAN:	
			productionPlanTab.GuiInputUp(inputRay);
			break;*/
		}
		
		productionPlanTab.GuiInputUp(inputRay);
	}
}
