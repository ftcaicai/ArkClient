using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductionProgressTab : MonoBehaviour 
{		 
	public GameObject goResProgItemClose;
	public GameObject goResProgItemIng;
	public GameObject goResProgItemOpen;
	public GameObject goResProgItemComplete;
	public UIScrollList progresslList;
	public UIButton btnSlotAdd;
	public UIButton btnPause;
	public SpriteText textPause;
	public SpriteText textSlotAdd;
	public SpriteText textSlotAddCash;
	
	public SpriteText textNoList;
	
	private long m_iSlotAddCash = 0;
	
	[SerializeField] GameObject[] effectOpen = new GameObject[5];
	
	public void SetActiveSlotAdd( bool isActive)
	{
		btnSlotAdd.controlIsEnabled = isActive;
		
		if( true == isActive)
			textSlotAdd.Color = Color.black;
		else
			textSlotAdd.Color = Color.gray;
	}
	
	public void SetActivePause( bool isActive)
	{
		btnPause.controlIsEnabled = isActive;
		
		if( true == isActive)
			textPause.Color = Color.black;
		else
			textPause.Color = Color.gray;
	}
	
	public void SetPauseState( bool isPause)
	{
		if( false == isPause)
			textPause.Text = AsTableManager.Instance.GetTbl_String(1333);
		else
			textPause.Text = AsTableManager.Instance.GetTbl_String(1322);
	}
	
	public void PlayLineEffect(int _line)
	{
		if( effectOpen.Length <= _line )
			return;
		
		effectOpen[_line].SetActiveRecursively(false);
		effectOpen[_line].SetActiveRecursively(true);
	}
	
	public void Open( bool isPauseState, byte nTechCount, byte nSlotOpenNum, Dictionary<byte, ProductionProgData> progDataList )
	{		
		progresslList.ClearList( true);
		
		foreach(GameObject obj in effectOpen)
		{
			obj.SetActiveRecursively(false);
		}
		
		if( 0 == nTechCount )
		{
			SetActiveSlotAdd(false);
			SetActivePause(false);
			SetPauseState( false );
			return;
		}		
		
		
		
		if( AsGameDefine.eITEM_PRODUCT_SLOT_COUNT <= nSlotOpenNum )
		{
			SetActiveSlotAdd(false);
		}
		else
		{
			SetActiveSlotAdd(true);
		}
		
		SetPauseState( isPauseState );
		bool isPauseBtnActive = false;
		
		for( byte i=0; i<AsGameDefine.eITEM_PRODUCT_SLOT_COUNT; ++i )
		{
			if( i >= nSlotOpenNum )
			{
				InsertItemClose(i);
			}
			else
			{
				if( false == progDataList.ContainsKey( i ) )
				{
					InsertItemOpen( i );
				}
				else
				{
					if( 0 == progDataList[i].getServerData.nRecipeIndex )
					{
						InsertItemOpen(i);						
					}
					else if( 0 == progDataList[i].getServerData.nProductTime )
					{
						InsertItemComplete( i, progDataList[i] );									
					}
					else
					{
						InsertItemIng( i, progDataList[i] );	
						isPauseBtnActive = true;
					}					
				}					
			}				
		}	
		
		if( false == isPauseBtnActive )
		{
			SetActivePause(false);
		}
		else
		{
			SetActivePause(true);
		}
		
		progresslList.ScrollToItem( 0, 0.0f);
//		progresslList.ScrollListTo( 0.0f);
	}
	
	public void Close()
	{
		progresslList.ClearList(true);
	}
	
	public void InsertItemClose( byte _iIndex )
	{		
		UIListItem item = progresslList.CreateItem( goResProgItemClose ) as UIListItem;		
		ProductionProgItemClose _script  = item.gameObject.GetComponent<ProductionProgItemClose>();	
		_script.Open(_iIndex);
	}
	
	public void InsertItemIng( byte _iIndex, ProductionProgData _progData )
	{		
		UIListItem item = progresslList.CreateItem( goResProgItemIng ) as UIListItem;		
		ProductionProgItemIng _progressItem  = item.gameObject.GetComponent<ProductionProgItemIng>();	
		_progressItem.Open( _iIndex, _progData );
	}
	
	public void InsertItemOpen( byte _iIndex )
	{		
		UIListItem item = progresslList.CreateItem( goResProgItemOpen ) as UIListItem;		
		ProductionProgItemOpen _progressItem  = item.gameObject.GetComponent<ProductionProgItemOpen>();	
		_progressItem.Open(_iIndex);
	}
				
	public void InsertItemComplete( byte _iIndex, ProductionProgData _progData )
	{
		UIListItem item = progresslList.CreateItem( goResProgItemComplete ) as UIListItem;		
		ProductionProgItemComplete _progressItem  = item.gameObject.GetComponent<ProductionProgItemComplete>();	
		_progressItem.Open( _iIndex, _progData );			
	}
	
	
	public void SendProductCashSlotOpen()
	{
        if (m_iSlotAddCash > AsUserInfo.Instance.nMiracle)
		{
			AsHudDlgMgr.Instance.CloseProductionDlg();
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();
			return;	
		}
		
		AsCommonSender.SendItemProductCashSlotOpen();
	}
	
	private void SlotAddBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{					
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			
			string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
			string strText = AsTableManager.Instance.GetTbl_String( 271 );		
			
			if( false == AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
			{
				AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.CashMessageBox( m_iSlotAddCash, strTitle, strText, this, "SendProductCashSlotOpen", 
					AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION ) ); 
			}
		}
	}
	
	
	
	private void PauseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);				
			
			if( AsHudDlgMgr.Instance.IsOpenProductionDlg )
			{
				bool isSendProgress = !AsUserInfo.Instance.isProductionProgress;
				AsCommonSender.SendItemProductProgress( isSendProgress );
				if( true == isSendProgress )
					AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_MoveStopIndication() );
			}
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{		
		btnSlotAdd.SetInputDelegate(SlotAddBtnDelegate);
		btnPause.SetInputDelegate(PauseBtnDelegate);		
		
		textSlotAdd.Text = AsTableManager.Instance.GetTbl_String(1323);
		textNoList.Text = AsTableManager.Instance.GetTbl_String(1335);
		
		
		
		Tbl_GlobalWeight_Record _record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 26 );
		if( null != _record )
		{
			m_iSlotAddCash = (long)_record.Value;
		}
		else
		{
			Debug.LogError("ProductionProgressTab::Start()[ not find Tbl_GlobalWeight_Record [ id : " + 26);
		}	
		textSlotAddCash.Text = m_iSlotAddCash.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	//input	
	public void GuiInputDown( Ray inputRay )
	{		
		for( int i = 0; i < progresslList.Count; i++)
		{
			UIListItem listItem = progresslList.GetItem(i) as UIListItem;
			
			ProductionProgressItem baseItem = listItem.GetComponent<ProductionProgressItem>();
			if( null != baseItem )
				baseItem.GuiInputDown(inputRay);
		}	
	}	
		  
	public void GuiInputUp(Ray inputRay)
	{ 
		for( int i = 0; i < progresslList.Count; i++)
		{
			UIListItem listItem = progresslList.GetItem(i) as UIListItem;
			
			ProductionProgressItem baseItem = listItem.GetComponent<ProductionProgressItem>();
			if( null != baseItem )
				baseItem.GuiInputUp(inputRay);
		}			
	}
}
