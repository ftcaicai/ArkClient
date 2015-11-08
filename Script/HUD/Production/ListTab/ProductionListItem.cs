using UnityEngine;
using System.Collections;


public class ProductionListItem : MonoBehaviour
{
	public UIButton 	btnMake;
	public SpriteText 	textName;
	public SimpleSprite iconImgPos; 
	public Vector2 minusItemSize;
	public SpriteText textBtn;	
	public SpriteText textShowLevel;
	
	
	private UISlotItem m_SlotItem;
	private UISlotItem m_DownSlotItem;
	
	private Item m_item = null;
	private Tbl_Production_Record m_record;
	private System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();
	
	public void Open( Item _item, Tbl_Production_Record _record )
	{
		if( null == _item )
		{
			Debug.LogError("ProductionListItem::Open()[ null == Item ]");
			return;
		}
		
		if( null == _record )
		{			
			Debug.LogError("ProductionListItem::Open()[ null == Tbl_Production_Record ]");
			return;
		}
		
		m_record = _record;
		
		m_item = _item;	
		textName.Text = AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId );
		
		if( null != textShowLevel )
			textShowLevel.Text = string.Empty;
			
		if( null != m_SlotItem )
			GameObject.DestroyObject( m_SlotItem.gameObject );
		
		if( false == _record.IsRandItemType() )
		{
			m_SlotItem = ResourceLoad.CreateItemIcon( m_item.GetIcon(), iconImgPos, new Vector3(0f,0f,-0.2f), minusItemSize );
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
		IsCheckReadyMaking();
	}
	
	public void IsCheckReadyMaking()
	{		
		if( null == m_record )
			return;		
		
		bool isActiveMaking = true;
		
		for( int i=0; i<4; ++i )
		{			
			int iItemId = 0;
			int iNeedCount = 0;				
			switch( i )
			{
			case 0:
				iItemId = m_record.iBaseID;
				iNeedCount = m_record.iBaseCount;
				break;
			case 1:
				iItemId = m_record.iSubID_1;
				iNeedCount = m_record.iSubCount_1;
				break;
			case 2:
				iItemId = m_record.iSubID_2;
				iNeedCount = m_record.iSubCount_2;
				break;
			case 3:
				iItemId = m_record.iOpID;
				iNeedCount = m_record.iOpCount;
				break;
			}
			
			if( 0 != iItemId && int.MaxValue != iItemId && int.MaxValue != iNeedCount )
			{
				int iHaveCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(iItemId);	
			
				
				
				if( iHaveCount < iNeedCount )
				{
					isActiveMaking = false;				
				}				
			}			
		}
		
		if( AsUserInfo.Instance.SavedCharStat.nGold < m_record.iGold )
		{
			SetActiveMaking( false );			
			return;
		}	
		
		SetActiveMaking(isActiveMaking);		
	}
	
	public void SetActiveMaking( bool isActive)
	{		
		m_sbTemp.Length = 0;
		
		if( true == isActive)
		{
			m_sbTemp.Append( Color.green);
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1019));
			textBtn.Text = m_sbTemp.ToString();
		}
		else			
		{
			m_sbTemp.Append( Color.gray);
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1019));
			textBtn.Text = m_sbTemp.ToString();
		}
	}
	
	private void MakeBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			//dopamin				
			if(AsPartyManager.Instance.IsPartyNotice)
			{		
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(AsTableManager.Instance.GetTbl_String(1728));
				return;
			}
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			
			if( AsHudDlgMgr.Instance.IsOpenProductionDlg )
				AsHudDlgMgr.Instance.productionDlg.OpenPlanTap( m_record );
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		
		btnMake.SetInputDelegate(MakeBtnDelegate);
	}
	
	
	//input	
	public void GuiInputDown( Ray inputRay )
	{		
		m_DownSlotItem = null;		
		
//		if( null != m_SlotItem && true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
		if( ( null != m_SlotItem) && ( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay)))
		{								
			m_DownSlotItem = m_SlotItem;		
		}						
	}	
		  
	public void GuiInputUp(Ray inputRay)
	{ 
		if( null != m_DownSlotItem && null != m_SlotItem )
		{		
//			if( true == iconImgPos.collider.bounds.IntersectRay( inputRay ) )
			if( true == AsUtil.PtInCollider( m_SlotItem.iconImg.collider, inputRay))
			{										
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_item, true );							
			}	
		}		
	}
}