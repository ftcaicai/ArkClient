using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GachaLineupPopupController : MonoBehaviour
{
	public GameObject listItemPrefab;
	public SimpleSprite upArrowSprite;
	public SimpleSprite downArrowSprite;
	public UIScrollList scrollList;
	public SpriteText txtTitle;
	public UIButton btnClose;
	public SpriteText txtIcon;
	public SpriteText txtGrade;
	public SpriteText txtLevel;
	public SpriteText txtMin;
	public SpriteText txtMax;
	public SpriteText txtOption;
	public SpriteText txtRandomOption;
	public SpriteText txtRandomDesc;


	List<GachaEquipLineupListItemController> listLineupListItemCon = new List<GachaEquipLineupListItemController>();

    List<int> listQuantity = new List<int>();
    List<Item> listItem = new List<Item>();

    Color colorStrength = Color.white;
	

	// Use this for initialization
	void Start () 
	{
		if (txtTitle != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);

		if (txtIcon != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtIcon);

		if (txtGrade != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtGrade);

		if (txtLevel != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtLevel);

		if (txtMin != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMin);

		if (txtMax != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMax);

		if (txtOption != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtOption);


		if (txtRandomOption != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtRandomOption);

		if (txtRandomDesc != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtRandomDesc);

		btnClose.SetInputDelegate(CloseProcess);
		scrollList.SetInputDelegate(InputDelegate);
	}


	void Update()
	{
		if (scrollList != null)
			if (scrollList.gameObject.activeSelf)
				CheckMorelistMark();
	}

	void SetString(eCashStoreMainCategory _mainCategory)
	{
		txtIcon.Text	= AsTableManager.Instance.GetTbl_String(2169);
		txtGrade.Text	= AsTableManager.Instance.GetTbl_String(2168);
		txtLevel.Text	= AsTableManager.Instance.GetTbl_String(1724);
		txtOption.Text	= AsTableManager.Instance.GetTbl_String(2242);
		txtRandomOption.Text= AsTableManager.Instance.GetTbl_String(2244);
		

		if (_mainCategory == eCashStoreMainCategory.WEAPON)
		{
			txtTitle.Text = AsTableManager.Instance.GetTbl_String(2239);
			txtMin.Text = AsTableManager.Instance.GetTbl_String(2240);
			txtMax.Text = AsTableManager.Instance.GetTbl_String(2241);
			txtRandomDesc.Text = AsTableManager.Instance.GetTbl_String(2245);
		}
		else if (_mainCategory == eCashStoreMainCategory.EQUIPMENT)
		{
			txtTitle.Text = AsTableManager.Instance.GetTbl_String(2248);
			txtMin.Text = AsTableManager.Instance.GetTbl_String(2249);
			txtMax.Text = AsTableManager.Instance.GetTbl_String(2250);
			txtRandomDesc.Text = AsTableManager.Instance.GetTbl_String(2251);
		}
	}

	public void ShowLineUp(eCashStoreMainCategory _mainCategory,  List<Item> _listItem, List<int> _listQuantities)
	{
        listQuantity = _listQuantities;
        listItem = _listItem;

		AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

		SetString(_mainCategory);

		listLineupListItemCon.Clear();

		Debug.LogWarning("Item count = " + _listItem.Count);

		int count = 0;

		foreach (Item item in _listItem)
		{
			ItemData itemData = item.ItemData;

			Item.eITEM_TYPE equipType = itemData.GetItemType();

			IUIListObject newListItem = scrollList.CreateItem(listItemPrefab);

			newListItem.gameObject.name = listLineupListItemCon.Count.ToString();

			GachaEquipLineupListItemController listItemController = newListItem.gameObject.GetComponent<GachaEquipLineupListItemController>();

			listLineupListItemCon.Add(listItemController);

			listItemController.itemID = itemData.m_iID;

			Item nowItem = ItemMgr.ItemManagement.GetItem(itemData.m_iID);

			Item.eEQUIP eEquip = (Item.eEQUIP)itemData.GetSubType();

			int quentity = nowItem.ItemData.GetItemType() == Item.eITEM_TYPE.EquipItem ?  0 : _listQuantities[count] ;

			if (listItemController != null)
			{
				GameObject objIcon = AsCashStore.GetItemIcon(itemData.m_iID.ToString(), quentity);

                StringBuilder sbItemName = new StringBuilder();

                if (nowItem.ItemData.GetItemType() == Item.eITEM_TYPE.EquipItem && _listQuantities[count] >= 1)
                {
                    sbItemName.Append(colorStrength);
                    sbItemName.Append("+");
                    sbItemName.Append(_listQuantities[count]);
                    sbItemName.Append(" ");
                    sbItemName.Append(nowItem.ItemData.GetGradeColor());
                    sbItemName.Append(AsTableManager.Instance.GetTbl_String(nowItem.ItemData.nameId));
                }
                else
                {
                    sbItemName.Insert(0, nowItem.ItemData.GetGradeColor().ToString());
                    sbItemName.AppendFormat("{0}", AsTableManager.Instance.GetTbl_String(nowItem.ItemData.nameId));
                }

				string name  	= sbItemName.ToString();
				string level	= itemData.levelLimit.ToString();
				string txtMin		= string.Empty;
				string txtMax		= string.Empty;
				string option	= itemData.destId == int.MaxValue ? string.Empty : AsTableManager.Instance.GetTbl_String(itemData.destId);

				if (equipType != Item.eITEM_TYPE.EquipItem)
					option = string.Empty;


                Tbl_Strengthen_Record strengthenRecord = null;

				if (nowItem.ItemData.GetItemType() == Item.eITEM_TYPE.EquipItem && _listQuantities[count] >= 1)
				{
                     strengthenRecord = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord(nowItem.ItemData.GetItemType(),
                                                       (Item.eEQUIP)nowItem.ItemData.GetSubType(), nowItem.ItemData.grade, nowItem.ItemData.levelLimit, _listQuantities[count]);
                }


				// find item min max
                int min = 0;
                int max = 0;

				if (equipType == Item.eITEM_TYPE.EquipItem )
				{
					if (Item.eEQUIP.Weapon == eEquip)
					{
						if (itemData.needClass == eCLASS.CLERIC || itemData.needClass == eCLASS.MAGICIAN)
						{
                            min = strengthenRecord == null ? itemData.matkDmgMin : itemData.matkDmgMin + itemData.matkDmgMin * strengthenRecord.getStrengthenRatiog / 1000;
                            max = strengthenRecord == null ? itemData.matkDmgMax : itemData.matkDmgMax + itemData.matkDmgMax * strengthenRecord.getStrengthenRatiog / 1000;
						}
						else
						{
                            min = strengthenRecord == null ? itemData.parkDmgMin : itemData.parkDmgMin + itemData.parkDmgMin * strengthenRecord.getStrengthenRatiog / 1000;
                            max = strengthenRecord == null ? itemData.parkDmgMax : itemData.parkDmgMax + itemData.parkDmgMax * strengthenRecord.getStrengthenRatiog / 1000;
						}
					}
					else if (Item.eEQUIP.Ring == eEquip || Item.eEQUIP.Necklace == eEquip || Item.eEQUIP.Earring == eEquip)
					{
						min = max = 0;
					}
					else
					{
						min = strengthenRecord == null ? itemData.pDef : itemData.pDef + itemData.pDef * strengthenRecord.getStrengthenRatiog / 1000;
						max = strengthenRecord == null ? itemData.mDef : itemData.mDef + itemData.mDef * strengthenRecord.getStrengthenRatiog / 1000;
					}
				}


                if (equipType == Item.eITEM_TYPE.EquipItem)
                {
                    txtMin = min.ToString();
                    txtMax = max.ToString();
                }
                else
                {
                    txtMin = string.Empty;
                    txtMax = string.Empty;
                }

				listItemController.SetInfo(objIcon, name, level, txtMin, txtMax, option);
                listItemController.idx = count;
			}

			count++;
		}
	}


	public void CloseProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (AsHudDlgMgr.Instance.IsOpenCashStore)
				AsHudDlgMgr.Instance.cashStore.LockInput(false);
			
			Destroy(gameObject);
		}
	}

	void ShowTooltip(int _itemID, int _streng)
	{
		Item itemData = ItemMgr.ItemManagement.GetItem(_itemID);

        if (itemData != null)
        {
            if (itemData.ItemData.GetItemType() == Item.eITEM_TYPE.EquipItem)
            {
                sITEM sitem = new sITEM();
                sitem.nItemTableIdx = _itemID;
                sitem.nStrengthenCount = (byte)_streng;
                sitem.nTradeCount = itemData.ItemData.m_sbItemTradeLimit;

                RealItem realItem = new RealItem(sitem, 0);

                if (TooltipMgr.Instance != null)
                    TooltipMgr.Instance.OpenToolTipForCash(TooltipMgr.eOPEN_DLG.left, realItem);
            }
            else
            {
                if (TooltipMgr.Instance != null)
                    TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.left, _itemID);
            }
        }

	}

	public void ValueChangedDelegate(IUIObject obj)
	{
	

	}

	public void InputDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			foreach (GachaEquipLineupListItemController con in listLineupListItemCon)
			{
				con.boxCollider.enabled = true;
				if (AsUtil.PtInCollider(con.boxCollider, ptr.ray))
				{
					ShowTooltip(con.itemID, listQuantity[con.idx]);
					con.boxCollider.enabled = false;
					break;
				}
				con.boxCollider.enabled = false;
			}
		}

	}

	void CheckMorelistMark()
	{
		if (scrollList == null)
			return;

		if (scrollList.Count > 0 && scrollList.gameObject.activeSelf)
		{
			if (!scrollList.IsShowingLastItem())
			{
				if (downArrowSprite.IsHidden() == true)
					downArrowSprite.Hide(false);
			}
			else
			{
				if (downArrowSprite.IsHidden() == false)
					downArrowSprite.Hide(true);
			}

			if (!scrollList.IsShowingFirstItem())
			{
				if (upArrowSprite.IsHidden() == true)
					upArrowSprite.Hide(false);
			}
			else
			{
				if (upArrowSprite.IsHidden() == false)
					upArrowSprite.Hide(true);
			}
		}
		else
		{
			if (downArrowSprite.IsHidden() == false)
				downArrowSprite.Hide(true);

			if (upArrowSprite.IsHidden() == false)
				upArrowSprite.Hide(true);
		}
	}
}
