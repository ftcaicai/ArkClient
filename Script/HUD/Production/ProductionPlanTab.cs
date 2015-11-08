using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public class ProductionPlanTab : MonoBehaviour
{
	public SpriteText textItemName;
	public SimpleSprite iconImgPos;
	public Vector2 minusItemSize;

	public UIButton btnExit;
	public UIButton btnMaking;
	public SpriteText textbtnMaking;

	public SpriteText textNeedTime;
	public SpriteText textMakeExp;
	public SpriteText textExp;
	public SpriteText textBtnMaking;

	public SpriteText[] textMatTypes;
	public SpriteText[] textMats;
	public SimpleSprite[] iconImgPosMats;

	public SpriteText textMakingCost;

	public SpriteText textNeedTimeTitle;
	public SpriteText textMakeExpTitle;
	public SpriteText textExpTitle;
	public SpriteText textNeedGoldTitle;
	public SpriteText textMakingCount;

	public SpriteText textTitle;
	public SpriteText textLevel;
	public SpriteText textGrade;
	public SpriteText textOption;
	public SpriteText textOption_1;
	public SpriteText textOptionShow;	
	public SpriteText textShowLevel;
	public SpriteText textRandomPrint;

	private UISlotItem[] m_MatSlotItems = new UISlotItem[4];
	private UISlotItem m_SlotItem;
	private Tbl_Production_Record m_record;
	private UISlotItem m_DownSlotItem;
	private Item m_item = null;
	private Item[] m_itemList = new Item[4];

	private StringBuilder m_sbMatsTemp = new StringBuilder();
	
	private AsMessageBox m_msgBox;
	private bool m_isRandomItem = false;

	public void SetActiveMaking( bool isActive)
	{
		btnMaking.controlIsEnabled = isActive;
		if( true == isActive)
			textBtnMaking.Color = Color.black;
		else
			textBtnMaking.Color = Color.gray;
	}

	public void Close()
	{
	}

	public void Open( Tbl_Production_Record _record)
	{
		m_record = _record;
		if( null == _record)
		{
			Debug.LogError("ProductionListTab::InsertItem()[ null == Tbl_Production_Record ]");
			return;
		}

		m_item = ItemMgr.ItemManagement.GetItem( _record.getItemID);
		if( null == m_item)
		{
			Debug.LogError("ProductionListTab::InsertItem()[ null == item ] id : " + _record.getItemID);
			return;
		}

		if( null != m_SlotItem)
			GameObject.DestroyObject( m_SlotItem.gameObject);
	

		if( 1 < m_record.getItemCount)
			textMakingCount.Text = m_record.getItemCount.ToString();
		else
			textMakingCount.Text = string.Empty;

		textItemName.Text = AsTableManager.Instance.GetTbl_String( m_item.ItemData.nameId);
		textNeedTime.Text = AsMath.GetDateConvertRemainTime( (int)_record.itemTime, AsTableManager.Instance.GetTbl_String(88),
			AsTableManager.Instance.GetTbl_String(89), AsTableManager.Instance.GetTbl_String(90));
		textMakeExp.Text = _record.iExpertism.ToString();
		textExp.Text = _record.iExp.ToString();
		textMakingCost.Text = _record.iGold.ToString( "#,#0", CultureInfo.InvariantCulture);

		m_sbMatsTemp.Length = 0;
		m_sbMatsTemp.Append( "Lv.");
		m_sbMatsTemp.Append( m_item.ItemData.levelLimit);

		textLevel.Text = m_sbMatsTemp.ToString();
		textGrade.Text = m_item.GetStrGrade();
		textOptionShow.Text = string.Empty;
		if( null!= textRandomPrint)
			textRandomPrint.Text = string.Empty;
		
		if( null != textShowLevel )
			textShowLevel.Text = string.Empty;
		
		
		textOption.Text = string.Empty;	
		textOption_1.Text = string.Empty;
		m_isRandomItem = _record.IsRandItemType();
		if( false == m_isRandomItem )
		{
			m_SlotItem = ResourceLoad.CreateItemIcon( m_item.GetIcon(), iconImgPos, Vector3.back, minusItemSize, false);
			
			textOption_1.Text = string.Empty;
			int iMin = 0;
			int iMax = 0;
			m_sbMatsTemp.Length= 0;
			if( Item.eITEM_TYPE.EquipItem == m_item.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == m_item.ItemData.GetItemType())
			{
				switch( (Item.eEQUIP) m_item.ItemData.GetSubType())
				{
				case Item.eEQUIP.Weapon:
					if( m_item.ItemData.needClass == eCLASS.CLERIC || m_item.ItemData.needClass == eCLASS.MAGICIAN)
					{
						iMin = m_item.ItemData.matkDmgMin;
						iMax = m_item.ItemData.matkDmgMax;
						m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1059));
					}
					else
					{
						iMin = m_item.ItemData.parkDmgMin;
						iMax = m_item.ItemData.parkDmgMax;
						m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1051));
					}
	
					m_sbMatsTemp.Append( iMin);
					m_sbMatsTemp.Append( "~");
					m_sbMatsTemp.Append( iMax);
					textOption.Text = m_sbMatsTemp.ToString();
					break;
				case Item.eEQUIP.Ring:
				case Item.eEQUIP.Earring:
				case Item.eEQUIP.Necklace:
					textOption.Text = m_sbMatsTemp.ToString();
					break;
				default:
					m_sbMatsTemp.Length = 0;
					m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1052));
					m_sbMatsTemp.Append( m_item.ItemData.pDef);
					textOption.Text = m_sbMatsTemp.ToString();
	
					m_sbMatsTemp.Length = 0;
					m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1053));
					m_sbMatsTemp.Append( m_item.ItemData.mDef);
					textOption_1.Text = m_sbMatsTemp.ToString();
					break;
				}
	
				m_sbMatsTemp.Length = 0;
				m_sbMatsTemp.Append( Color.red);
				m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1001));
				textOptionShow.Text = m_sbMatsTemp.ToString();
			}
			else if( Item.eITEM_TYPE.ActionItem == m_item.ItemData.GetItemType())
			{
				Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( m_item.ItemData.itemSkill);
				if( null == record)
				{
					Debug.LogError("TooltipInfoDlg::SetConsumeItem() [ null == record ] skill level id : " + m_item.ItemData.itemSkill);
					return;
				}
	
				string szDesc = AsTableManager.Instance.GetTbl_String( record.Description_Index);
				szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, m_item.ItemData.itemSkill, m_item.ItemData.itemSkillLevel, 0);
				textOption.Text = szDesc;
			}
			else
			{
				m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String( m_item.ItemData.destId));
				textOption.Text = m_sbMatsTemp.ToString();
			}
		}
		else
		{
			string levelTemp = string.Format( AsTableManager.Instance.GetTbl_String(901), m_item.ItemData.levelLimit );
			if( null != textShowLevel )
				textShowLevel.Text = levelTemp;	
			
			textLevel.Text = levelTemp;
			
			textGrade.Text = string.Empty;
			m_sbMatsTemp.Length = 0;
			m_sbMatsTemp.Append( Color.red);
			m_sbMatsTemp.Append( AsTableManager.Instance.GetTbl_String(1001));
			//textOptionShow.Text = m_sbMatsTemp.ToString();
			if( null!= textRandomPrint)
				textRandomPrint.Text = m_sbMatsTemp.ToString();
		}
		
		for( int i = 0; i < 4; ++i)
		{
			if( null != m_MatSlotItems[i])
				GameObject.DestroyObject( m_MatSlotItems[i].gameObject);

			int iItemId = 0;
			switch( i)
			{
			case 0:
				iItemId = _record.iBaseID;
				break;
			case 1:
				iItemId = _record.iSubID_1;
				break;
			case 2:
				iItemId = _record.iSubID_2;
				break;
			case 3:
				iItemId = _record.iOpID;
				break;
			}

			if( int.MaxValue == iItemId)
				continue;

			Item _matItem = ItemMgr.ItemManagement.GetItem( iItemId);
			if( null == _matItem)
			{
				Debug.LogError("ProductionPlanTab::Open()[ no find item ] id: " + iItemId);
				continue;
			}
			m_itemList[i] = _matItem;
			m_MatSlotItems[i] = ResourceLoad.CreateItemIcon( _matItem.GetIcon(), iconImgPosMats[i], Vector3.back, minusItemSize, false);
		}

		IsCheckReadyMaking();

		if( AsHudDlgMgr.Instance.IsOpenPlayerStatus)
			AsHudDlgMgr.Instance.ClosePlayerStatus();
	}

	public void IsCheckReadyMaking()
	{
		if( null == m_record)
			return;

		bool isActiveMaking = true;

		for( int i = 0; i < 4; ++i)
		{
			int iItemId = 0;
			int iNeedCount = 0;
			switch( i)
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

			if( 0 != iItemId && int.MaxValue != iItemId && int.MaxValue != iNeedCount)
			{
				int iHaveCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(iItemId);
				//textMats[i].Text = iHaveCount.ToString() + "/" + iNeedCount.ToString();
				m_sbMatsTemp.Remove( 0, m_sbMatsTemp.Length);
				m_sbMatsTemp.Append( iHaveCount);
				m_sbMatsTemp.Append( "/");
				m_sbMatsTemp.Append( iNeedCount);
				textMats[i].Text = m_sbMatsTemp.ToString();

				if( iHaveCount < iNeedCount)
					isActiveMaking = false;
			}
			else
			{
				textMats[i].Text = string.Empty;
			}
		}

		if( AsUserInfo.Instance.SavedCharStat.nGold < m_record.iGold)
		{
			SetActiveMaking( false);
			return;
		}

		SetActiveMaking(isActiveMaking);
	}

	private void BackBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsHudDlgMgr.Instance.IsOpenProductionDlg)
				AsHudDlgMgr.Instance.productionDlg.CloseProductonPlanTab();
		}
	}

	private void MakingBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false == AsHudDlgMgr.Instance.IsOpenProductionDlg)
				return;
			
			if (true == AsCommonSender.isSendItemProductRegister)
				return;			

			byte nslotidx = AsHudDlgMgr.Instance.productionDlg.GetCanUseProgIndex();
			if( byte.MaxValue == nslotidx)
			{
				if( null == m_msgBox )
				{
					m_msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(267), null, string.Empty,
						AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				}
				return;
			}
			
			AsHudDlgMgr.Instance.productionItemId = m_record.getItemID;
			
			if( false == m_isRandomItem )
			{
				AsHudDlgMgr.Instance.productionRandItem = false;
				
			}
			else
			{				
				AsHudDlgMgr.Instance.productionRandItem = true;
			}
			AsCommonSender.SendItemProduct( nslotidx, m_record.getIndex);
			AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_MoveStopIndication());
			AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_Production_Start", Vector3.zero, false);
		}
	}

	void Awake()
	{
		btnExit.SetInputDelegate( BackBtnDelegate);
		btnMaking.SetInputDelegate( MakingBtnDelegate);

		textMatTypes[0].Text = AsTableManager.Instance.GetTbl_String(1023);
		textMatTypes[1].Text = AsTableManager.Instance.GetTbl_String(1024);
		textMatTypes[2].Text = AsTableManager.Instance.GetTbl_String(1025);
		textMatTypes[3].Text = AsTableManager.Instance.GetTbl_String(1291);

		textNeedTimeTitle.Text = AsTableManager.Instance.GetTbl_String(1020);
		textMakeExpTitle.Text = AsTableManager.Instance.GetTbl_String(1014);
		textExpTitle.Text = AsTableManager.Instance.GetTbl_String(1021);
		textNeedGoldTitle.Text = AsTableManager.Instance.GetTbl_String(1026);
		textbtnMaking.Text = AsTableManager.Instance.GetTbl_String(1019);
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1678);

		foreach( SpriteText _text in textMats)
		{
			_text.Text = string.Empty;
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	//input
	public void GuiInputDown( Ray inputRay)
	{
		m_DownSlotItem = null;

		if( ( null != m_SlotItem) && ( true == AsUtil.PtInCollider( iconImgPos.collider, inputRay)))
		{
			m_DownSlotItem = m_SlotItem;
			return;
		}

		for( int i = 0; i < 4; ++i)
		{
			if( ( null != m_MatSlotItems[i]) && ( true == AsUtil.PtInCollider( iconImgPosMats[i].collider, inputRay)))
			{
				m_DownSlotItem = m_MatSlotItems[i];
				return;
			}
		}
	}

	public void GuiInputUp(Ray inputRay)
	{
		if( null != m_DownSlotItem)
		{
			if( m_DownSlotItem == m_SlotItem && true == AsUtil.PtInCollider( iconImgPos.collider, inputRay))
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_item, true);
				return;
			}

			for( int i = 0; i < 4; ++i)
			{
				if( null == m_MatSlotItems[i] || null == m_itemList[i] || null == iconImgPosMats[i])
					continue;

				if( m_DownSlotItem != m_MatSlotItems[i])
					continue;

				if( true == AsUtil.PtInCollider( iconImgPosMats[i].collider, inputRay))
				{
					AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
					TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_itemList[i]);
					return;
				}
			}
		}
	}
}
