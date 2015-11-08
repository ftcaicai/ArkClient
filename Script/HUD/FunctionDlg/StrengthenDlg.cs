using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrengthenDlg : MonoBehaviour 
{
    public SpriteText textTitle;
    public UIButton btnSpecial;
	public SpriteText textEquipExplain;
	
    public UIInvenSlot equipItemSlot;
    public Collider equipItemCollider;
	public Collider rectCollider;

    public SpriteText textNeedStrengthenItemText;
    public SpriteText textNeedGoldText;   
	public SpriteText textNeedCash;   

    public UIProgressBar probarComplete;

    public UIButton btnOk;
    public SpriteText textOk;
    public UIButton btnCancel;
    public SpriteText textResult;
	
    public GameObject goItemParent;     
    private AsMessageBox m_popupDlg; 	
	
	public Color NeedStrengthenItemTextColor;
	private System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();
	
    private bool m_isUseCash = false;
    private Tbl_Strengthen_Record m_CurStrengthenRecord;
    
    public float maxProgressTime = 2.0f;
    private float m_fProgressTime = 0.0f;
    private bool m_bProgressStated = false;	
    private bool m_bInputDown = false;
	private bool m_bResetOkBtnHold = false;
	
	
	private bool m_needFailedAction = false;
	private float m_maxFailedTime = 0.0f;
	
//	private System.Text.StringBuilder m_sbStrengthenShow = new System.Text.StringBuilder();
	
	
	public GameObject goStartEffect;
	public GameObject goMiddleEffect;
	public GameObject goEffectSucc;
	public GameObject goEffectFailed;
	public GameObject goEffectFailedBroken;
	
	
	public SimpleSprite m_StrengthenIcon_1;
	public SimpleSprite m_StrengthenIcon_2;

	//private int m_iStrengthenItemChange = 6;
	
	
	public void SetMiddleEffect( bool isEnable )
	{
		if( null == goMiddleEffect )
			return;
		
		
		goMiddleEffect.SetActiveRecursively( isEnable );
	}
	
	public void SetEndEffect( bool isEnable, eRESULTCODE _result = eRESULTCODE.eRESULT_MAX )
	{
		if( false == isEnable )
		{
			goEffectSucc.SetActiveRecursively( false );
			goEffectFailed.SetActiveRecursively( false );
			goEffectFailedBroken.SetActiveRecursively( false );
			return;
		}
		
		switch( _result )
		{
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_SUCCESS:
			goEffectSucc.SetActiveRecursively( isEnable );
			break;
			
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL:
			goEffectFailedBroken.SetActiveRecursively( isEnable );
			break;
			
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL_PROTECT:
			goEffectFailed.SetActiveRecursively( isEnable );
			break;
		}
		
	}
		
	


    // Use this for initialization
    void Start()
    {
        textTitle.Text = AsTableManager.Instance.GetTbl_String(1348);
		btnSpecial.Text = AsTableManager.Instance.GetTbl_String( 1341 ); 
		textOk.Text = AsTableManager.Instance.GetTbl_String(1340);    
		
		
		btnOk.SetInputDelegate(OkBtnDelegate);
        btnCancel.SetInputDelegate(CancelBtnDelegate);
		btnSpecial.SetInputDelegate(SpecialBtnDelegate); 
		
		
		SetMiddleEffect(false);
		SetEndEffect(false);
    }

  
    void Update()
    {
        if (true == m_bProgressStated)
        {          
            float fValue = Time.time - m_fProgressTime;
            probarComplete.Value = fValue / maxProgressTime;

            if (1.0f <= probarComplete.Value)
            {
                m_bProgressStated = false;
                probarComplete.Value = 0.0f;			
				
				if( false == AsUserInfo.Instance.IsEquipChaingEnable() )
				{
					if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
					{					
						AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
					}
					
					return;
				}

                AsCommonSender.SendItemStrengthen(eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_END, 
					(short)equipItemSlot.slotItem.realItem.getSlot, m_isUseCash);

            }
        }

        if (true == m_needFailedAction)
        {
            m_maxFailedTime -= Time.deltaTime;
            if (m_maxFailedTime < 0.0f)
            {
                m_maxFailedTime = 0.0f;
                m_needFailedAction = false;
                m_bResetOkBtnHold = false;
				SetEnableCancelBtn(true);           
                ResetRealItems();
            }
        }	
    }


    public void Open()
    {
		
		textEquipExplain.Text = string.Empty;
		textNeedCash.Text = string.Empty;
		textResult.Text = string.Empty;
        SetNoReadyStrength();		
		SetEnableCancelBtn(true);
        SetStrengthenIcon(0);
    }

    private void SetNoReadyStrength()
    {
        textNeedStrengthenItemText.Text = "";
        textNeedGoldText.Text = "";
        SetEnableOkBtn(false);     
    }
	
	private void SetEnableOkBtn( bool isEnable)
	{
		btnSpecial.controlIsEnabled = isEnable;
		btnOk.controlIsEnabled = isEnable;
		if( true == isEnable )
		{			
			m_sbTemp.Length = 0;
			//m_sbTemp.Append( Color.white );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1341) );
			btnSpecial.spriteText.Color = Color.black;
			btnSpecial.spriteText.Text = m_sbTemp.ToString();			
			
			m_sbTemp.Length = 0;
			//m_sbTemp.Append( Color.white );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1340) );
			textOk.Color = Color.black;
			textOk.Text = m_sbTemp.ToString();		
			
			CheckCashBtnState();
		}
		else
		{
			
			m_sbTemp.Length = 0;
			//m_sbTemp.Append( Color.gray );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1341) );
			btnSpecial.spriteText.Color = Color.gray;
			btnSpecial.spriteText.Text = m_sbTemp.ToString();	
			
			
			m_sbTemp.Length = 0;
			//m_sbTemp.Append( Color.gray );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1340) );
			textOk.Color = Color.gray;
			textOk.Text = m_sbTemp.ToString();	
		}
	}
	
	private void CheckCashBtnState()
	{
		if( null == m_CurStrengthenRecord || 0 >= m_CurStrengthenRecord.getMiracle )
		{
			btnSpecial.controlIsEnabled = false;
			m_sbTemp.Length = 0;
			//m_sbTemp.Append( Color.gray );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1341) );
			btnSpecial.spriteText.Color = Color.gray;
			btnSpecial.spriteText.Text = m_sbTemp.ToString();		
		}		
	}
	
	
	
	private void SetEnableCancelBtn( bool isEnable)
	{
		btnCancel.controlIsEnabled = isEnable;		
	}
	
	
	

    private void CancelBtnDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
			m_isUseCash = false;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
            AsHudDlgMgr.Instance.CloseStrengthenDlg();
        }
    }
	
	private void SpecialBtnDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
			if( true == AsCommonSender.isSendStrengthen )
				return;
			
			m_isUseCash = true;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);   			
			textResult.Text = string.Empty;
			SetPopupDlg( AsNotify.Instance.CashMessageBox( m_CurStrengthenRecord.getMiracle, AsTableManager.Instance.GetTbl_String(1341), 
													AsTableManager.Instance.GetTbl_String(1339), 
													this, "SendStrengthPacket", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION ) );
        }
	}
	
	public void SendStrengthPacket()
	{
		if( true == m_isUseCash &&  AsUserInfo.Instance.nMiracle < m_CurStrengthenRecord.getMiracle )
		{			
			AsHudDlgMgr.Instance.CloseStrengthenDlg();
			AsHudDlgMgr.Instance.OpenNeedSphereDlg();
			return;
		}
		
		SendItemStrengthen();
	}
	
	private void SendItemStrengthen()
	{
		m_bProgressStated = true;
        m_fProgressTime = Time.time;
		SetNoReadyStrength();		
		
		
		SetMiddleEffect(true);
        AsSoundManager.Instance.PlaySound("Sound/Interface/S6012_EFF_StrengthenProgress", Vector3.zero, false);
        m_bResetOkBtnHold = true;
		
		CloseResultPopupDlg();	
	}

    private void OkBtnDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {           
			if( true == AsCommonSender.isSendStrengthen )
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( false == AsUserInfo.Instance.IsEquipChaingEnable() )
			{
				if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
				{					
					AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				}
				
				return;
			}

			if( null == m_CurStrengthenRecord )	
			{
				Debug.LogError("StrengthenDlg::OkBtnDelegate() [ null == m_CurStrengthenRecord ] ");
				return;
			}
			
			m_isUseCash = false;
			textResult.Text = string.Empty;
			AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1340), AsTableManager.Instance.GetTbl_String(1342),
									this, "SendItemStrengthen", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) );
			
			
		
        }
    }

	
	
	public void SetPopupDlg( AsMessageBox _popup )
	{
		if( null!= m_popupDlg )
		{
			GameObject.Destroy(m_popupDlg.gameObject);
		}
		
		
		m_popupDlg = _popup;
	}
	
	public void CloseResultPopupDlg()
	{
		if( null != m_popupDlg )
			GameObject.Destroy( m_popupDlg.gameObject );
	}
	
	public bool IsResultPopupDlgOpen
	{
		get
		{
			if( null == m_popupDlg )
				return false;
				
			return true;
		}		
	}

	

    public bool IsEquipRect(Ray inputRay)
    {
        if (null == equipItemCollider)
        {
            Debug.LogError("StrengthenDlg::IsEquipRect() [ null == equipItemCollider ]");
            return false;
        }

//        return equipItemCollider.bounds.IntersectRay(inputRay);
		return AsUtil.PtInCollider( equipItemCollider, inputRay);
    }
	
	
	public void ReciveResult( body_SC_ITEM_STRENGTHEN_RESULT _data )
	{			
		switch( _data.eStrengthenType )
		{
		case eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_START:			
			//AsSlotEffectManager.Instance.SetStrengthenIngEffect( true, equipItemSlot.transform.position );
			break;
			
		case eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_END:
			
			//ClearEffectList();
			
			SetMiddleEffect( false );
			SetEndEffect(true, _data.eResult);
			
			
			
			
			if (eRESULTCODE.eRESULT_ITEM_STRENGTHEN_SUCCESS == _data.eResult)
            {
				m_sbTemp.Length = 0;
				m_sbTemp.Append( new Color( 0.3f, 0.7f, 0.9f, 1.0f) );
				m_sbTemp.Append( AsTableManager.Instance.GetTbl_String( 76 ) );
				textResult.Text = m_sbTemp.ToString();
				//AsSlotEffectManager.Instance.SetStrengthenSuccEffect( true, equipItemSlot.transform.position );
                //QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_EXTENSTION, new AchExtension(UIExtensionType.SUCCESS_INTENSIFY));
            }
            else
            {
				if( eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL == _data.eResult)
				{
					m_sbTemp.Length = 0;
					m_sbTemp.Append( new Color( 0.8f, 0.4f, 1.0f, 1.0f)  );
					m_sbTemp.Append( AsTableManager.Instance.GetTbl_String( 1343 ) );
				}
				else
				{
					m_sbTemp.Length = 0;
					m_sbTemp.Append( Color.red );
					m_sbTemp.Append( AsTableManager.Instance.GetTbl_String( 77 ) );
				}
				
				
				textResult.Text = m_sbTemp.ToString();
				//AsSlotEffectManager.Instance.SetStrengthenFailedEffect( true, equipItemSlot.transform.position );
                QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_EXTENSTION, new AchExtension(UIExtensionType.FAIL_INTENSIFY));
            }
			if( null == equipItemSlot.slotItem )
			{
				Debug.LogError("StrengthenDlg::ReciveResult()[ null == equipItemSlot.slotItem ] ");
				return;
			}
			
			//OpenResultPopupDlg( _data.eResult, equipItemSlot.slotItem.realItem );						
			m_needFailedAction = true;
			
			break;
		}
		
		
	}
		
	
	public void ResetRealItems()
	{			
		if( true == m_bResetOkBtnHold )
			return;
				
		if( null == equipItemSlot.slotItem )
			return;	
		
		RealItem _realitem =  ItemMgr.HadItemManagement.Inven.GetIvenItem(equipItemSlot.slotItem.realItem.getSlot);
		if( null == _realitem )
		{
			textEquipExplain.Text = string.Empty;
			equipItemSlot.DeleteSlotItem();
			return;
		}
	
		ResetStagethenRecore( equipItemSlot.slotItem.realItem, false );
		
		SetEquipExplainText( equipItemSlot.slotItem.realItem );
		SetNeedGold(m_CurStrengthenRecord);
		SetStrengthenItemCount(m_CurStrengthenRecord, equipItemSlot.slotItem.realItem.sItem.nStrengthenCount );	
		ResetCashState();
		ResetOkBtnState();	
	}

    

    
    private bool IsExist(List<int> cashitems, int iItemId)
    {
        foreach (int _cashitemId in cashitems)
        {
            if (_cashitemId == iItemId)
            {
                return true;
            }
        }

        return false;
    }

    
	private void SetStrengthenIcon( int iIndex )
	{
		if( 0 == iIndex )
		{
			m_StrengthenIcon_1.gameObject.active = true;
			m_StrengthenIcon_2.gameObject.active = false;
		}
		else
		{
			m_StrengthenIcon_2.gameObject.active = true;
			m_StrengthenIcon_1.gameObject.active = false;
		}
	}
   
	
	private int IsStrengthenItemID( Item.eITEM_TYPE itemtype, Item.eGRADE _grade )
	{
		if( itemtype == Item.eITEM_TYPE.EquipItem )
		{
			if( Item.eGRADE.Normal == _grade || 
				Item.eGRADE.Magic == _grade ||
				Item.eGRADE.Rare == _grade )
			{
				return 0;
			}
			else
			{
				return 1;
			}			
		}
		/*else if( record.getItemType != Item.eITEM_TYPE.CosEquipItem )
		{
			return 1;			
		}*/
		
		return 1;
	}
	
	private void SetStrengthenItemCount( Tbl_Strengthen_Record record, byte strengthenCount )
	{
		if( null == record )
			return;
		
		int TagetItemCount = record.getStrengthenStuffCount;
		int haveItemCount = 0;
		
		/*if( m_iStrengthenItemChange > strengthenCount && record.getItemType != Item.eITEM_TYPE.CosEquipItem )
		{
			haveItemCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(0);
			SetStrengthenIcon(0);
		}
		else
		{
			haveItemCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(1);
			SetStrengthenIcon(1);
		}*/
		
		if( 0 == IsStrengthenItemID( record.getItemType, record.getItemQuality )  )
		{
			haveItemCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(0);
			SetStrengthenIcon(0);
		}
		else
		{
			haveItemCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(1);
			SetStrengthenIcon(1);
		}
		
		
		
		
		
		m_sbTemp.Length = 0;
		if( TagetItemCount > haveItemCount )
		{
			m_sbTemp.Append( Color.red.ToString() );
			m_sbTemp.Append( TagetItemCount );
			textNeedStrengthenItemText.Text = m_sbTemp.ToString();
		}	
		else
		{
			m_sbTemp.Append( NeedStrengthenItemTextColor.ToString() );
			m_sbTemp.Append( TagetItemCount );
			textNeedStrengthenItemText.Text = m_sbTemp.ToString();
		}	
		
	}
	
	private void SetNeedGold( Tbl_Strengthen_Record _record )
	{		
		if( null == _record )
			return;
		
		ulong iTargetGold = (ulong)_record.getStrengthenCost;
		ulong iHaveGole = AsUserInfo.Instance.SavedCharStat.nGold;
		m_sbTemp.Length = 0;
		if( iTargetGold > iHaveGole )
		{
			m_sbTemp.Append( Color.red.ToString() );
			m_sbTemp.Append( iTargetGold );
			textNeedGoldText.Text = m_sbTemp.ToString();			
		}	
		else
		{
			m_sbTemp.Append( Color.yellow.ToString() );
			m_sbTemp.Append( iTargetGold );
			textNeedGoldText.Text = iTargetGold.ToString();
		}		
	}

    //-------------------------------------------------------
    public bool IsRect(Ray inputRay)
    {
        if (null == rectCollider)
            return false;

        return rectCollider.bounds.IntersectRay(inputRay);
	//	return AsUtil.PtInCollider( rectCollider, inputRay);
    }
	
	
	// input 
    public void GuiInputDown(Ray inputRay)
    {
        if (false == IsRect(inputRay))
            return;

        m_bInputDown = true;
    }

    public void GuiInputUp(Ray inputRay)
    {
		
		if( null != m_popupDlg)
		{
			AsItemViewMessageBox viewMsg = m_popupDlg as AsItemViewMessageBox;
			if( null!= viewMsg )
			{
				if( true == viewMsg.IsIconRect( inputRay ) && null != equipItemSlot.slotItem )
				{
					TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.normal, equipItemSlot.slotItem.realItem);
					return;
				}
			}
		}
		
		
        if (false == m_bInputDown)
            return;
            

        m_bInputDown = false;
	

        if (true == equipItemSlot.IsIntersect(inputRay))
        {
            if( null != equipItemSlot.slotItem )
			{
				TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.normal, equipItemSlot.slotItem.realItem); 
			}
           	return;
        }		
		
		if( m_StrengthenIcon_1.gameObject.active && null != m_StrengthenIcon_1.collider )
		{
			if( m_StrengthenIcon_1.collider.bounds.IntersectRay( inputRay ) )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, ItemMgr.HadItemManagement.Inven.GetStrengthenItemID(0, 0) );
				return;
			}
			
		}		
		else if( m_StrengthenIcon_2.gameObject.active )
		{
			if( m_StrengthenIcon_2.collider.bounds.IntersectRay( inputRay ) )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, ItemMgr.HadItemManagement.Inven.GetStrengthenItemID(1, 0) );
				return;
			}
		}
    }	
	
	
	public static bool IsCanStrengthenItem( RealItem _realItem )
	{
		if( null == _realItem )
			return false;
		
		if( false == _realItem.item.ItemData.isItemStrengthen )
			return false;
		
		if( !(Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() || 
			Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType()) )
			return false;
		
		if( _realItem.sItem.nStrengthenCount >= 10 )
				return false;
		
		if( IsCanStrengthenCosEquip( _realItem ) == false )
		{
			return false;
		}
		
		if( null == AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( _realItem.item.ItemData.GetItemType(), (Item.eEQUIP)_realItem.item.ItemData.GetSubType(), 
			_realItem.item.ItemData.grade, _realItem.item.ItemData.levelLimit, _realItem.sItem.nStrengthenCount+1 ) )		
			return false;
		
		
		return true;
	}
	
	public void ResetUIInvenItem( RealItem _realItem )
	{
		if( true == m_bProgressStated )
			return; 
		if( null == _realItem )					
			return;	

		bool isCanStrengthen = IsCanStrengthenItem( _realItem );
		if( isCanStrengthen == false )
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(75),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
			return;
		}
		
		if( false == ResetStagethenRecore( _realItem ) )
			return;
		
		if( Item.eITEM_TYPE.EquipItem == _realItem.item.ItemData.GetItemType() || 
			Item.eITEM_TYPE.CosEquipItem == _realItem.item.ItemData.GetItemType() )
		{
			SetEquipItem(_realItem);
		}
		else
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(75),
												null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );			
		}
	}
	
	
	private void SetEquipExplainText( RealItem _realItem )
	{
		if( null == _realItem )
			return;
		
		if( null == textEquipExplain )
			return;
		
		m_sbTemp.Length = 0;
		
		if( 0 < _realItem.sItem.nStrengthenCount )
		{
			m_sbTemp.Append( NeedStrengthenItemTextColor.ToString() );
			m_sbTemp.Append( "+" );
			m_sbTemp.Append( _realItem.sItem.nStrengthenCount );
			m_sbTemp.Append( " " );
		}
		m_sbTemp.Append( _realItem.item.ItemData.GetGradeColor().ToString() );
		m_sbTemp.Append( AsTableManager.Instance.GetTbl_String( _realItem.item.ItemData.nameId ) );		
		textEquipExplain.Text = m_sbTemp.ToString(); 
		
	}
	
	private void SetEquipItem( RealItem _realItem )
	{
		if( true == AsHudDlgMgr.Instance.isOpenMsgBox )
			return;
		
		if( true == IsResultPopupDlgOpen ) 
			return;
		
		if( null == _realItem )					
			return;			
		
		m_bProgressStated = false;
        probarComplete.Value = 0.0f;		
		
		equipItemSlot.DeleteSlotItem();
		equipItemSlot.CreateSlotItem( _realItem, goItemParent.transform );
		AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.m_strDropSound, Vector3.zero, false );
		
		textResult.Text = string.Empty;
		SetEquipExplainText( _realItem );
		
		
		SetNeedGold( m_CurStrengthenRecord );
		SetStrengthenItemCount( m_CurStrengthenRecord, _realItem.sItem.nStrengthenCount );	
		ResetOkBtnState();
		ResetCashState();
	}
	
	private bool ResetStagethenRecore( RealItem _realItem, bool isShowMessage = true )
	{
		if( null == _realItem )
		{
			m_CurStrengthenRecord = null;
			return false;
		}
		
		m_CurStrengthenRecord = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( _realItem.item.ItemData.GetItemType(), (Item.eEQUIP)_realItem.item.ItemData.GetSubType(), 
			_realItem.item.ItemData.grade, _realItem.item.ItemData.levelLimit, _realItem.sItem.nStrengthenCount+1 );
		if( null == m_CurStrengthenRecord )
		{
			Debug.LogWarning("strengthen equip failed [ level : " + _realItem.item.ItemData.levelLimit + 
				" q : " + _realItem.item.ItemData.grade + " streng : " + (_realItem.sItem.nStrengthenCount+1) );	
			
			if( true == isShowMessage )
			{
				if( false == AsHudDlgMgr.Instance.isOpenMsgBox )
				{
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(75),
													null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				}
				else
				{
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(75), eCHATTYPE.eCHATTYPE_SYSTEM);
				}
			}
			
			return false;
		}
		
		return true;
	}
	
	public void ResetCashState()
	{
		
		if( null == m_CurStrengthenRecord )
			return;
		
		textNeedCash.Text = m_CurStrengthenRecord.getMiracle.ToString();		
	}
	
	
	public void ResetOkBtnState()
	{
		if( null == equipItemSlot.slotItem )
		{
			SetEnableOkBtn(false);			
			return;
		}
		
		if( 10 <= equipItemSlot.slotItem.realItem.sItem.nStrengthenCount )
		{
			SetEnableOkBtn(false);
			return;
		}
		
		if( IsCanStrengthenCosEquip( equipItemSlot.slotItem.realItem ) == false )
		{
			SetEnableOkBtn(false);
			return;
		}
		
		RealItem	_realItem = equipItemSlot.slotItem.realItem;
		if( null == AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( _realItem.item.ItemData.GetItemType(), (Item.eEQUIP)_realItem.item.ItemData.GetSubType(), 
			_realItem.item.ItemData.grade, _realItem.item.ItemData.levelLimit, _realItem.sItem.nStrengthenCount+1 ) )		
		{
			SetEnableOkBtn(false);
			return;
		}
		
		
		int iTargetStuffCount = m_CurStrengthenRecord.getStrengthenStuffCount;
		
		int iHaveStuffCount = 0;
		
		/*if( m_iStrengthenItemChange > equipItemSlot.slotItem.realItem.sItem.nStrengthenCount && 
			equipItemSlot.slotItem.realItem.item.ItemData.GetItemType() != Item.eITEM_TYPE.CosEquipItem )
		{
			iHaveStuffCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(0);
			SetStrengthenIcon(0);
		}
		else
		{
			iHaveStuffCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(1);
			SetStrengthenIcon(1);
		}*/
		
		if( 0 == IsStrengthenItemID( equipItemSlot.slotItem.realItem.item.ItemData.GetItemType(), 
			equipItemSlot.slotItem.realItem.item.ItemData.grade )  )
		{
			iHaveStuffCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(0);
			SetStrengthenIcon(0);
		}
		else
		{
			iHaveStuffCount = ItemMgr.HadItemManagement.Inven.GetStrengthenItemTotalCount(1);
			SetStrengthenIcon(1);
		}
		
		
		if( iTargetStuffCount > iHaveStuffCount )
		{
			SetEnableOkBtn(false);
			return;
		}
		
		ulong iTargetGold = (ulong)m_CurStrengthenRecord.getStrengthenCost;
		ulong iHaveGole = AsUserInfo.Instance.SavedCharStat.nGold;
		if( iTargetGold > iHaveGole )
		{
			SetEnableOkBtn(false);
			return;
		}	
		SetEnableOkBtn(true);		
	}
	
	public void Close()
	{
		equipItemSlot.DeleteSlotItem();			
		CloseResultPopupDlg();
        QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_STRENGTHEN_UI));
    }
	
	
	public static bool IsCanStrengthenCosEquip( RealItem _realItem )
	{
		bool 	isStrengthen = true;
		
		ItemData	slotItemData 	= _realItem.item.ItemData;
		sITEM		slotSItem		= _realItem.sItem;
		if( slotItemData.GetItemType() == Item.eITEM_TYPE.CosEquipItem )
		{
			if( slotItemData.GetSubType() == (int)Item.eEQUIP.Fairy || slotItemData.GetSubType() == (int)Item.eEQUIP.Wing )
				isStrengthen = false;
		
			if( slotItemData.grade == Item.eGRADE.Normal )
				isStrengthen = false;
			
			if( slotItemData.grade == Item.eGRADE.Magic || slotItemData.grade == Item.eGRADE.Rare )
			{
				if( slotSItem.nStrengthenCount >= 3 )
					isStrengthen = false;
			}
		}
		
		return isStrengthen;
	}
	
}
