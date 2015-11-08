using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipMgr : MonoBehaviour
{
	public enum eCommonState
	{
		NONE = -1,
		Buy = 0,
		Sell,
		Sell_Btn,
		Socket,
		Strength,
		Socket_Strength,
		Equip,
        CashStoreEquip,
		Max
	}

	public enum eOPEN_DLG
	{
		normal,
		right,
		left,
	}

	//---------------------------------------------------------------------
	/* Static Variable */
	//---------------------------------------------------------------------
	private static TooltipMgr ms_kIstance = null;
	public static eOPEN_DLG openDlgState = eOPEN_DLG.normal;

	//---------------------------------------------------------------------
	/* Static function*/
	//---------------------------------------------------------------------
	public static TooltipMgr Instance
	{
		get { return ms_kIstance; }
	}

	//---------------------------------------------------------------------
	/* Variable*/
	//---------------------------------------------------------------------
	public GameObject tooltipParent;
	public Color colorItemLimit;
	public Color colorEquipEnable;
	public TooltipObject tooltipTwoPosition_1;
	public TooltipObject tooltipTwoPosition_2;
	public TooltipObject tooltipOnePosition;
	public string infoTooltipPath = "UI/Tooltip/Tooltip01";
	public string comonTooltipPath = "UI/Tooltip/Tooltip04";
	public string setItemTooltipPath = "UI/Tooltip/Tooltip02";
	public string enchantTooltipPath = "UI/Tooltip/Tooltip03";
	public string gaugeTooltipPath = "UI/Tooltip/Tooltip03_Guage";
	public string socialTooltipPath = "UI/Tooltip/Tooltip04_SC";
	public string cashTooltipPath = "UI/Tooltip/Tooltip04_Cash";

	private string eventTooltipPath = "UI/AsGUI/Event/Event_Tooltip";
	private TooltipEventDlg m_tooltipEventDlg;
	private MonoBehaviour m_useBtnScript = null;

	public void ClearEventDlg()
	{
		if( null != m_tooltipEventDlg)
			GameObject.DestroyObject( m_tooltipEventDlg.gameObject);

		m_tooltipEventDlg = null;
	}

	public void OpenEventDlg( body2_SC_EVENT_LIST _data)
	{
		ClearEventDlg();

		GameObject obTempTooltip = ResourceLoad.CreateGameObject( eventTooltipPath, tooltipOnePosition.transform);
		if( null == obTempTooltip)
			return;

		m_tooltipEventDlg = obTempTooltip.GetComponent<TooltipEventDlg>();
		if( null == m_tooltipEventDlg)
		{
			Debug.LogError( "TooltipMgr::OpenEventDlg()[ not find Component( TooltipEventDlg) ] path : " + eventTooltipPath);
			DestroyObject( obTempTooltip);
			return;
		}

		m_tooltipEventDlg.Open( _data);
	}

	// Set Tooltip
	private void SetTooltip( TooltipMgr.eOPEN_DLG _openDlgState, TooltipObject tooltipObject, sITEM _sitem, bool isRandomItemAuto,
							bool isEquip, MonoBehaviour script, string method, eCommonState commonState, float fOffsetZ)
	{
		if( null == _sitem)
			return;

		Item _item = ItemMgr.ItemManagement.GetItem( _sitem.nItemTableIdx);
		if( null == _item)
			return;

		AddTooltipInfo( tooltipObject, _sitem, isRandomItemAuto, isEquip);
		AddTooltipSet( tooltipObject, _item);
		if( _item.ItemData.CheckPetItem() == false)
			AddTooltipEnchant( tooltipObject, _sitem);
		AddTooltipGauge (tooltipObject, _sitem, _item.ItemData);
		AddTooltipCommon( tooltipObject, _item, _sitem, script, method, commonState);
		tooltipObject.ResetPosition( _openDlgState, fOffsetZ);
	}

	private void SetTooltip( TooltipMgr.eOPEN_DLG _openDlgState, TooltipObject tooltipObject, RealItem _realitem, bool isRandomItemAuto,
							bool isEquip, MonoBehaviour script, string method, eCommonState commonState, float fOffsetZ)
	{
		if( null == _realitem)
			return;

		AddTooltipInfo( tooltipObject, _realitem.sItem, isRandomItemAuto, isEquip);
		AddTooltipSet( tooltipObject, _realitem.item);
		if( _realitem.item.ItemData.CheckPetItem() == false)
			AddTooltipEnchant( tooltipObject, _realitem.sItem);
		AddTooltipGauge (tooltipObject, _realitem.sItem, _realitem.item.ItemData);
		AddTooltipCommon( tooltipObject, _realitem, script, method, commonState);
		tooltipObject.ResetPosition( _openDlgState, fOffsetZ);
	}

	private void SetTooltip( TooltipMgr.eOPEN_DLG _openDlgState, TooltipObject tooltipObject, Item _item, bool isRandomItemAuto,
							bool isEquip, MonoBehaviour script, string method, eCommonState commonState, float fOffsetZ)
	{

		if( null == _item)
			return;

		AddTooltipInfo( tooltipObject, _item, isRandomItemAuto, isEquip);
		AddTooltipSet( tooltipObject, _item);
		AddTooltipCommon( tooltipObject, _item, null, script, method, commonState);
		tooltipObject.ResetPosition( _openDlgState, fOffsetZ);
	}


    private void SetTooltipForCashStore(TooltipMgr.eOPEN_DLG _openDlgState, TooltipObject tooltipObject, RealItem _realitem, bool isRandomItemAuto,
                        bool isEquip, MonoBehaviour script, string method, eCommonState commonState, float fOffsetZ)
    {
        if (null == _realitem)
            return;

        AddTooltipInfo(tooltipObject, _realitem.sItem, isRandomItemAuto, isEquip);
        AddTooltipSet(tooltipObject, _realitem.item);
        AddTooltipCommon(tooltipObject, _realitem, script, method, commonState);
        tooltipObject.ResetPosition(_openDlgState, fOffsetZ);
    }


    //OpenTooltip
	// --------------

    public void OpenToolTipForCash(TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem)
    {
        if (null == _realItem)
        {
            Debug.LogError("TooltipMgr::OpenSetTooltip() [ null == RealItem ]");
            return;
        }
        
        Clear();
        SetTooltipForCashStore(_openDlgState, tooltipOnePosition, _realItem, false, false, null, "", eCommonState.Sell, 0f);
    }

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem, eCommonState _eState = eCommonState.Sell, bool isEquip = false)
	{
		if( null == _realItem)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == RealItem ]");
			return;
		}

		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _realItem, false, isEquip, null, "", _eState, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem, bool isEquip, eCommonState commonState)
	{
		if( null == _realItem)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == RealItem ]");
			return;
		}

		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _realItem, false, isEquip, null, "", commonState, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem, MonoBehaviour script, string method, eCommonState commonState)
	{
		if( null == _realItem)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == RealItem ]");
			return;
		}

		Clear();

		m_useBtnScript = script;
		SetTooltip( _openDlgState, tooltipOnePosition, _realItem, false, false, script, method, commonState, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, Item _item, MonoBehaviour script, string method, eCommonState commonState)
	{
		if( null == _item)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item ]");
			return;
		}

		Clear();
		m_useBtnScript = script;
		SetTooltip( _openDlgState, tooltipOnePosition, _item, false, false, script, method, commonState, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, Item _item, bool isRandomItemAuto = false, float fOffsetZ = 0f)
	{
		if( null == _item)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item ]");
			return;
		}
		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _item, isRandomItemAuto, false, null, "", eCommonState.Sell, fOffsetZ);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, int iItemID, bool isRandomItemAuto = false)
	{
		Item _item = ItemMgr.ItemManagement.GetItem( iItemID);
		if( null == _item)
		{
			Debug.LogError( "TooltipMgr::OpenTooltip()[ null == Item ] item id : " + iItemID);
			return;
		}
		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _item, isRandomItemAuto, false, null, "", eCommonState.Sell, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem_1, Item _item_2, bool isRandomItemAuto_2 = false, float fOffsetZ = 0.0f)
	{
		if( null == _realItem_1 || null == _item_2)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item_1 || null == item_2 ]");
			return;
		}

		if (_openDlgState == TooltipMgr.eOPEN_DLG.right)
			_openDlgState = ConvertOpenDlgState_In_Pad_Device ();

		Clear();
		SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1.sItem, true, true, null, "", eCommonState.Sell, fOffsetZ);
		SetTooltip( _openDlgState, tooltipTwoPosition_2, _item_2, isRandomItemAuto_2, false, null, "", eCommonState.Sell, fOffsetZ);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem_1, Item _item_2, MonoBehaviour script_2, string method_2, eCommonState commonState)
	{
		if( null == _realItem_1 || null == _item_2)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item_1 || null == item_2 ]");
			return;
		}

		if (_openDlgState == TooltipMgr.eOPEN_DLG.right)
			_openDlgState = ConvertOpenDlgState_In_Pad_Device ();

		Clear();

		m_useBtnScript = script_2;

		SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1.sItem, true, true, null, "", eCommonState.Sell, 0f);
		SetTooltip( _openDlgState, tooltipTwoPosition_2, _item_2, false, false, script_2, method_2, commonState, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem_1, RealItem _realItem_2, MonoBehaviour script_2, string method_2,
		eCommonState commonState_1, eCommonState commonState_2)
	{
		if( null == _realItem_1 || null == _realItem_2)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item_1 || null == item_2 ]");
			return;
		}

		if (_openDlgState == TooltipMgr.eOPEN_DLG.right)
			_openDlgState = ConvertOpenDlgState_In_Pad_Device ();

		Clear();

		SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1, true, true, null, "", commonState_1, 0f);
		SetTooltip( _openDlgState, tooltipTwoPosition_2, _realItem_2, false, false, script_2, method_2, commonState_2, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, sITEM _realItem_1, RealItem _realItem_2, MonoBehaviour script_2, string method_2,
		eCommonState commonState_1, eCommonState commonState_2)
	{
		if( null == _realItem_1 || null == _realItem_2)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item_1 || null == item_2 ]");
			return;
		}

		if (_openDlgState == TooltipMgr.eOPEN_DLG.right)
			_openDlgState = ConvertOpenDlgState_In_Pad_Device ();

		Clear();

		SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1, true, true, null, "", commonState_1, 0f);
		SetTooltip( _openDlgState, tooltipTwoPosition_2, _realItem_2, false, false, script_2, method_2, commonState_2, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem_1, RealItem _realItem_2)
	{
		if( null == _realItem_1 || null == _realItem_2)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == item_1 || null == item_2 ]");
			return;
		}

		if (_openDlgState == TooltipMgr.eOPEN_DLG.right)
			_openDlgState = ConvertOpenDlgState_In_Pad_Device ();

		Clear();

		SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1.sItem, true, true, null, "", eCommonState.Sell, 0f);
		SetTooltip( _openDlgState, tooltipTwoPosition_2, _realItem_2.sItem, false, false, null, "", eCommonState.Sell, 0f);
	}

	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, sITEM _sitem, float fOffsetZ)
	{
		if( null == _sitem)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == _sitem ]");
			return;
		}

		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _sitem, true, false, null, "", eCommonState.Sell, fOffsetZ);
	}
	
	//$yde
	public void OpenTooltip( TooltipMgr.eOPEN_DLG _openDlgState, sITEM _sitem, MonoBehaviour script_1, string method_1, float fOffsetZ)
	{
		if( null == _sitem)
		{
			Debug.LogError( "TooltipMgr::OpenSetTooltip() [ null == _sitem ]");
			return;
		}

		Clear();
		SetTooltip( _openDlgState, tooltipOnePosition, _sitem, false, false, script_1, method_1, eCommonState.Buy, fOffsetZ);
	}

	public void OpenShopTooltip( TooltipMgr.eOPEN_DLG _openDlgState, RealItem _realItem_1, Item _item, MonoBehaviour script, string method, int buyAmount = -1, int tradeItemID = -1, int tradeItemCount = -1)
	{
		if( null == _item)
		{
			Debug.LogError( "TooltipMgr::OpenShopTooltip() [ null == _sitem ]");
			return;
		}

		Clear();

		TooltipObject _temp = null;
		if( null != _realItem_1)
		{
			SetTooltip( _openDlgState, tooltipTwoPosition_1, _realItem_1.sItem, true, true, null, "", eCommonState.NONE, 0f);
			_temp = tooltipTwoPosition_2;
		}
		else
		{
			_temp = tooltipOnePosition;
		}

		AddTooltipInfo( _temp, _item, false, false);
		AddTooltipSet( _temp, _item );
		TooltipCommDlg tempCommon = AddTooltipCommon( _temp, _item, null, script, method, eCommonState.Buy);

		if (buyAmount == -1)
			buyAmount = _item.ItemData.buyAmount;

		if (tradeItemID != -1)
		{
			tempCommon.imgGold.Hide(true);

			GameObject objGoods = AsNpcStore.GetItemIcon(tradeItemID.ToString(), 1);
			objGoods.transform.parent = tempCommon.imgGold.transform.parent;
			objGoods.transform.localPosition = tempCommon.imgGold.transform.localPosition;
			objGoods.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

			if (tradeItemCount <= 0)
				tempCommon.btnBuy.gameObject.SetActive(false);
			else
				tempCommon.btnBuy.gameObject.SetActive(true);
		}
	
		tempCommon.SetBuyPrice(buyAmount);

		_temp.ResetPosition( _openDlgState, 0f);
	}

	public void Clear()
	{
		ClearEventDlg();
		tooltipTwoPosition_1.Clear();
		tooltipTwoPosition_2.Clear();
		tooltipOnePosition.Clear();

		m_useBtnScript = null;
		
		#region - pstore search -
		UIPStoreSearchSlot.TooltipActivate( false);
		#endregion
	}
	
	public bool IsOpenAny()
	{
		return true == tooltipTwoPosition_1.isOpen || 
			true == tooltipTwoPosition_2.isOpen || 
			true == tooltipOnePosition.isOpen || 
			null != m_tooltipEventDlg;			
	}

	public bool IsUseBtnScript
	{
		get { return ( null != m_useBtnScript); }
	}

	public void InputUp()
	{
	}

	// add info tootip
	protected void AddTooltipInfo( TooltipObject tooltipObject, Item _item, bool isRandomItemAuto, bool isEquip)
	{
		TooltipInfoDlg tempToolTip = CreateInfoDlg( infoTooltipPath);
		if( null == tempToolTip)
			return;

		tempToolTip.Open( _item, isRandomItemAuto, isEquip);
		tooltipObject.AddTooltipDlg( tempToolTip);
	}

	protected void AddTooltipInfo( TooltipObject tooltipObject, sITEM _sitem, bool isRandomItemAuto, bool isEquip)
	{
		TooltipInfoDlg tempToolTip = CreateInfoDlg( infoTooltipPath);
		if( null == tempToolTip)
			return;

		tempToolTip.Open( _sitem, isEquip);
		tooltipObject.AddTooltipDlg( tempToolTip);
	}

	// add setItem tooltip
	protected void AddTooltipSet( TooltipObject tooltipObject, Item _item)
	{
		if( _item.ItemData.getSetGroupID == int.MaxValue)
			return;

		Tbl_SetItem_Record setRecord = AsTableManager.Instance.GetTbl_SetItem_Record( _item.ItemData.getSetGroupID);
		if( null == setRecord)
			return;

		if( setRecord.skillId == int.MaxValue || 0 == setRecord.skillId)
			return;

		TooltipSetDlg tempToolTip = CreateSetDlg( setItemTooltipPath);
		if( null == tempToolTip)
			return;

		tempToolTip.Open( _item);
		tooltipObject.AddTooltipDlg( tempToolTip);
	}

	// add Enchant tooltip
	protected void AddTooltipEnchant( TooltipObject tooltipObject, sITEM _sItem )
	{
		if( false == IsHaveEnchant( _sItem.nEnchantInfo))
			return;

		TooltipEnchantDlg tempToolTip = CreateEnchantDlg( enchantTooltipPath);
		if( null == tempToolTip)
			return;

		tempToolTip.Open( _sItem);
		tooltipObject.AddTooltipDlg( tempToolTip);
	}

	// add Gauge tooltip
	protected void AddTooltipGauge( TooltipObject tooltipObject, sITEM _sItem , ItemData _itemData)
	{
		if (_itemData.GetItemType () != Item.eITEM_TYPE.CosEquipItem)
			return;

		if (_itemData.m_Item_MixEnchant == false)
			return;

		TooltipGaugeDlg gaugeToolTip = CreateGaugeDlg( gaugeTooltipPath);
		if( null == gaugeToolTip)
			return;
		
		gaugeToolTip.Open( _sItem , _itemData );
		tooltipObject.AddTooltipDlg( gaugeToolTip);
	}

	// add Common tooltip
	protected TooltipCommDlg AddTooltipCommon( TooltipObject tooltipObject, Item _item, sITEM _sitem, MonoBehaviour btnScript, string btnMethod,eCommonState commonState)
	{
		TooltipCommDlg tempToolTip = null;

		if( commonState == eCommonState.Buy && _item.ItemData.getGoodsType == Item.eGOODS_TYPE.Point)
			tempToolTip = CreateCommonDlg( socialTooltipPath);
		else if( commonState == eCommonState.Buy && _item.ItemData.getGoodsType == Item.eGOODS_TYPE.Cash)
			tempToolTip = CreateCommonDlg( cashTooltipPath);
		else
			tempToolTip = CreateCommonDlg( comonTooltipPath);

		if( null == tempToolTip)
			return null;

		tempToolTip.Open( _item, _sitem, btnScript, btnMethod, commonState);
		tooltipObject.AddTooltipDlg( tempToolTip);

		return tempToolTip;
	}

	protected TooltipCommDlg AddTooltipCommon( TooltipObject tooltipObject, RealItem _realItem, MonoBehaviour btnScript, string btnMethod,eCommonState commonState)
	{
		TooltipCommDlg tempToolTip = null;

		if( commonState == eCommonState.Buy && _realItem.item.ItemData.getGoodsType == Item.eGOODS_TYPE.Point)
			tempToolTip = CreateCommonDlg( socialTooltipPath);
		else if( commonState == eCommonState.Buy && _realItem.item.ItemData.getGoodsType == Item.eGOODS_TYPE.Cash)
			tempToolTip = CreateCommonDlg( cashTooltipPath);
        else if (commonState == eCommonState.CashStoreEquip)
        {
            tempToolTip = CreateCommonDlg(comonTooltipPath);
           // tempToolTip.spri

        }
        else
			tempToolTip = CreateCommonDlg( comonTooltipPath);

		if( null == tempToolTip)
			return null;

		tempToolTip.Open( _realItem, btnScript, btnMethod, commonState);
		tooltipObject.AddTooltipDlg( tempToolTip);

		return tempToolTip;
	}

	// Create
	protected TooltipInfoDlg CreateInfoDlg( string strPath)
	{
		GameObject obTempTooltip = ResourceLoad.CreateGameObject( strPath, tooltipParent.transform);
		if( null == obTempTooltip)
			return null;

		TooltipInfoDlg tempToolTip = obTempTooltip.GetComponent<TooltipInfoDlg>();
		if( null == tempToolTip)
		{
			Debug.LogError( "TooltipMgr::CreateInfoDlg()[ not find Component( TooltipInfoDlg) ] path : " + strPath);
			DestroyObject( obTempTooltip);
			return null;
		}

		return tempToolTip;
	}

	protected TooltipSetDlg CreateSetDlg( string strPath)
	{
		GameObject obTempTooltip = ResourceLoad.CreateGameObject( strPath, tooltipParent.transform);
		if( null == obTempTooltip)
			return null;

		TooltipSetDlg tempToolTip = obTempTooltip.GetComponent<TooltipSetDlg>();
		if( null == tempToolTip)
		{
			Debug.LogError( "TooltipMgr::CreateSetDlg()[ not find Component( TooltipSetDlg) ] path : " + strPath);
			DestroyObject( obTempTooltip);
			return null;
		}

		return tempToolTip;
	}

	protected TooltipEnchantDlg CreateEnchantDlg( string strPath)
	{
		GameObject obTempTooltip = ResourceLoad.CreateGameObject( strPath, tooltipParent.transform);
		if( null == obTempTooltip)
			return null;

		TooltipEnchantDlg tempToolTip = obTempTooltip.GetComponent<TooltipEnchantDlg>();
		if( null == tempToolTip)
		{
			Debug.LogError( "TooltipMgr::CreateEnchantDlg()[ not find Component( TooltipEnchantDlg) ] path : " + strPath);
			DestroyObject( obTempTooltip);
			return null;
		}

		return tempToolTip;
	}

	protected TooltipGaugeDlg CreateGaugeDlg( string strPath)
	{
		GameObject goGaugeTooltip = ResourceLoad.CreateGameObject( strPath, tooltipParent.transform);
		if( null == goGaugeTooltip)
			return null;
		
		TooltipGaugeDlg gaugeToolTip = goGaugeTooltip.GetComponent<TooltipGaugeDlg>();
		if( null == gaugeToolTip)
		{
			Debug.LogError( "TooltipMgr::CreateGaugeDlg()[ not find Component( TooltipGaugeDlg) ] path : " + strPath);
			DestroyObject( goGaugeTooltip);
			return null;
		}
		
		return gaugeToolTip;
	}

	protected TooltipCommDlg CreateCommonDlg( string strPath)
	{
		GameObject obTempTooltip = ResourceLoad.CreateGameObject( strPath, tooltipParent.transform);
		if( null == obTempTooltip)
			return null;

		TooltipCommDlg tempToolTip = obTempTooltip.GetComponent<TooltipCommDlg>();
		if( null == tempToolTip)
		{
			Debug.LogError( "TooltipMgr::CreateCommonDlg()[ not find Component( TooltipCommDlg) ] path : " + strPath);
			DestroyObject( obTempTooltip);
			return null;
		}

		return tempToolTip;
	}

	//	in pad device(ipad....) , Since the aspect ratio and convert the tooltip position.
	protected TooltipMgr.eOPEN_DLG ConvertOpenDlgState_In_Pad_Device()
	{
		TooltipMgr.eOPEN_DLG _dlgOpenState = TooltipMgr.eOPEN_DLG.right;

		float width = Screen.width;
		float height = Screen.height;

		float fRatio = width / height;

		if( fRatio < 1.5f )
			_dlgOpenState = TooltipMgr.eOPEN_DLG.left;

		return _dlgOpenState;
	}

	// check
	public static bool IsHaveEnchant( int[] enchants)
	{
		foreach( int iData in enchants)
		{
			if( -1 != iData)
				return true;
		}

		return false;
	}

	public static bool IsEnableEnchant( int[] enchants, int strengthenCount)
	{
		for( int i=0; i<enchants.Length; ++i)
		{
			if( i == 1)
			{
				if( strengthenCount >= ItemMgr.Instance.GetEnchantStrengthenCount())
					return true;
			}
			else
			{
				if( -1 != enchants[i])
					return true;
			}
		}

		return false;
	}

	// Awake
	void Awake()
	{
		ms_kIstance = this;

		tooltipTwoPosition_1.dlgPosState = TooltipObject.ePOSITION.LEFT;
		tooltipTwoPosition_2.dlgPosState = TooltipObject.ePOSITION.RIGHT;
		tooltipOnePosition.dlgPosState = TooltipObject.ePOSITION.CENTER;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}
