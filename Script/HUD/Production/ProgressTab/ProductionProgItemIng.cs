using UnityEngine;
using System.Collections;
using System.Text;


public class ProductionProgItemIng : ProductionProgressItem
{
	public UIButton btnCash;
	public UIButton btnIconCash;
	public SpriteText textCashBtn;
	public SpriteText textShowLevel;
	
	public UIButton btnCancel;
	public SpriteText textCashCost;
	public UIProgressBar gaugeBar;
	public SimpleSprite iconImgPos;
	public Vector2 minusItemSize;
	public SpriteText textTimeValue;
	public SpriteText textItemName;
	
	private UISlotItem m_SlotItem;
	private ProductionProgData m_ProgData;
	private UISlotItem m_DownSlotItem;
	private Item m_item = null;
	
	private StringBuilder m_sbCancel = new StringBuilder();
	private StringBuilder m_sbTimeShow = new StringBuilder();
	private long m_cashSphere = 0;
	
	public void Open( byte _iIndex, ProductionProgData _progData )
	{
		SetSlot(_iIndex);
		
		if( null != textShowLevel )
			textShowLevel.Text = string.Empty;
			
		
		Tbl_Production_Record record = AsTableManager.Instance.GetProductionTable().GetRecord( _progData.getServerData.nRecipeIndex );
		if( null == record )
		{
			Debug.LogError("ProductionProgItemIng::Open()[ recipe index : " + _progData.getServerData.nRecipeIndex );
			return;
		}
		
		m_item = ItemMgr.ItemManagement.GetItem( record.getItemID );
		if( null == m_item )
		{
			Debug.LogError("ProductionProgItemIng::Open()[ item index : " + record.getItemID );
			return;
		}
		
		textItemName.Text = AsTableManager.Instance.GetTbl_String( m_item.ItemData.nameId );
		
		if( null != m_SlotItem )
		{
			GameObject.DestroyObject( m_SlotItem.gameObject );
		}
		
		if( false == record.IsRandItemType() )
		{
			m_SlotItem = ResourceLoad.CreateItemIcon( m_item.GetIcon(), iconImgPos, Vector3.back, minusItemSize );
			GameObject.DestroyImmediate( m_SlotItem.coolTime.gameObject);
			GameObject.DestroyImmediate( m_SlotItem.itemCountText.gameObject);
			m_SlotItem.iconImg.renderer.enabled = false;
			
			
			UIListItem _listitem = gameObject.GetComponent<UIListItem>();
			if( null != m_SlotItem && null != _listitem )
			{
				_listitem.layers[0] = m_SlotItem.iconImg;
			}
		}
		else
		{
			if( null != textShowLevel )
				textShowLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String(901), m_item.ItemData.levelLimit );
		}
		m_ProgData = _progData;	
		
		
		
		m_cashSphere = GetCashCost();
		textCashCost.Text = m_cashSphere.ToString();
	}
	
	
	private long GetCashCost()
	{
		if( null == m_ProgData )
			return 0;	
		
		
		float fSphere = (float)m_ProgData.getNeedSphere;
		if( 0f == fSphere )
		{
			Debug.LogError("ProductionProgItemIng::GetCashCost()[ 0 == m_ProgData.getNeedSphere ]");
			return 0;
		}
		
		return (long)(m_ProgData.GetCashTime() / fSphere + 
				(m_ProgData.GetCashTime() % fSphere == 0f ? 0f : 1f ));
	}
	
	public void SendCashDirect()
	{
		if( null == m_ProgData )
		{
			Debug.LogError("ProductionProgItemIng::SendCashDirect()[ null == ProductionProgData ]");	
			return;
		}
		
		if (AsUserInfo.Instance.nMiracle < m_cashSphere)
		{
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();			
			return;
		}		
		
		AsCommonSender.SendItemProductCashDirect( m_ProgData.getProductSlot );
	}
	
	
	
	
	public void SendCancel()
	{
		if( null == m_ProgData )
		{
			Debug.LogError("ProductionProgItemIng::SendCancel()[ null == ProductionProgData ]");	
			return;
		}
		
		AsCommonSender.SendItemProductCancel( m_ProgData.getProductSlot );
	}
	
	
	
	private void CashBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
				return;
			
			if( null == m_ProgData )
			{
				Debug.LogError("ProductionProgItemIng::CashBtnDelegate()[ null = m_ProgData ]");
				return;
			}
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			
			string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
			string strText = AsTableManager.Instance.GetTbl_String( 270 );		
			
			if( false == AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
			{
				AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.CashMessageBox( m_cashSphere, strTitle, strText, this, "SendCashDirect", 
					AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION ) ); 
			}
			
		}
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			
//			string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
			/*string strText = AsTableManager.Instance.GetTbl_String( 269 ) + "\n" + 
				Color.red.ToString() + AsTableManager.Instance.GetTbl_String( 1337 );	*/		
			
			if( false == AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
			{
				AsHudDlgMgr.Instance.productionDlg.SetMessageBox( 
					AsNotify.Instance.MessageBox( 	AsTableManager.Instance.GetTbl_String( 126 ), m_sbCancel.ToString(), this, "SendCancel", 
													AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) ); 
			}
		}
	}
	
	
	void Awake()	
	{
		m_sbCancel.Append ( AsTableManager.Instance.GetTbl_String( 269 ) );
		m_sbCancel.Append ( "\n" );
		m_sbCancel.Append ( Color.red );
		m_sbCancel.Append ( AsTableManager.Instance.GetTbl_String( 1337 ) );
	}
	
	// Use this for initialization
	void Start () 
	{
		btnCash.SetInputDelegate(CashBtnDelegate);
		btnIconCash.SetInputDelegate(CashBtnDelegate);
		btnCancel.SetInputDelegate(CancelBtnDelegate);
		textCashBtn.Text = AsTableManager.Instance.GetTbl_String(1320);
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(1027);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != gaugeBar && null != m_ProgData )
		{
			gaugeBar.Value = m_ProgData.GetValue();
			int remaintime = (int)m_ProgData.GetRemainTime();
			if( 0 > remaintime )
				remaintime = 0;
			
			m_sbTimeShow.Remove( 0, m_sbTimeShow.Length );
			m_sbTimeShow.Append( AsTableManager.Instance.GetTbl_String(1103) );
			m_sbTimeShow.Append( "  " );
            m_sbTimeShow.Append(AsMath.GetDateConvertRemainTime(remaintime, AsTableManager.Instance.GetTbl_String(88),
                AsTableManager.Instance.GetTbl_String(89),
                AsTableManager.Instance.GetTbl_String(90)));
			
			textTimeValue.Text = m_sbTimeShow.ToString();
			//textTimeValue.Text = AsTableManager.Instance.GetTbl_String(1103) + "  " + AsMath.GetDateConvertRemainTime( remaintime, "h", "m", "s" );
			
			
			m_cashSphere = GetCashCost();
			textCashCost.Text = m_cashSphere.ToString();
		}
	}
	
	//input	
	public override void GuiInputDown( Ray inputRay)
	{
		m_DownSlotItem = null;
		
//		if( null != m_SlotItem && true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
		if( ( null != m_SlotItem) && ( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay)))
		{
			m_DownSlotItem = m_SlotItem;
		}
	}
		  
	public override void GuiInputUp( Ray inputRay)
	{
		if( ( null != m_DownSlotItem) && ( null != m_SlotItem))
		{
//			if( true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
			if( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay))
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_item );
			}
		}
	}
}
