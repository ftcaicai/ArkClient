using UnityEngine;
using System.Collections.Generic;

public class GachaLineupFreePopupController : MonoBehaviour
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
	public SpriteText txtItemName;

	List<GachaFreeLineupListItemController> listLineupListItemCon = new List<GachaFreeLineupListItemController>();

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

		if (txtItemName != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtItemName);

		btnClose.SetInputDelegate(CloseProcess);

		scrollList.SetInputDelegate(InputDelegate);
	}


	void Update()
	{
		if (scrollList != null)
			if (scrollList.gameObject.activeSelf)
				CheckMorelistMark();
	}

	void SetString()
	{
		txtTitle.Text	= AsTableManager.Instance.GetTbl_String(2271);
		txtIcon.Text	= AsTableManager.Instance.GetTbl_String(2169);
		txtGrade.Text	= AsTableManager.Instance.GetTbl_String(2168);
		txtLevel.Text	= AsTableManager.Instance.GetTbl_String(1724);
		txtItemName.Text= AsTableManager.Instance.GetTbl_String(2273);
	}

	public void ShowLineUp(int[] itemIDs, int[] itemCounts)
	{
		SetString();

		listLineupListItemCon.Clear();

		int count = 0;

		foreach (int itemID in itemIDs)
		{
			IUIObject newListItem = scrollList.CreateItem(listItemPrefab);

			GachaFreeLineupListItemController newListItemController = newListItem.gameObject.GetComponent<GachaFreeLineupListItemController>();

			if (newListItemController == null)
			{
				count++;
				continue;
			}

			Item newItem = ItemMgr.ItemManagement.GetItem(itemID);

			if (newItem == null)
			{
				count++;
				continue;
			}

			// add list
			listLineupListItemCon.Add(newListItemController);
			newListItemController.itemID = itemID;

			int itemCount = itemCounts.Length > count ? itemCounts[count] : 0;

			GameObject objIcon = AsCashStore.GetItemIcon(newItem.ItemID.ToString(),  itemCount);

			newListItemController.SetInfo(objIcon, newItem.GetStrGrade(), newItem.ItemData.levelLimit, AsTableManager.Instance.GetTbl_String(newItem.ItemData.nameId));

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

	public void InputDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			foreach (GachaFreeLineupListItemController con in listLineupListItemCon)
			{
				con.boxCollider.enabled = true;
				if (AsUtil.PtInCollider(con.boxCollider, ptr.ray))
				{
					ShowTooltip(con.itemID);
					con.boxCollider.enabled = false;
					break;
				}
				con.boxCollider.enabled = false;
			}
		}
	}

	void ShowTooltip(int _itemID)
	{
		AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

		Item itemData = ItemMgr.ItemManagement.GetItem(_itemID);

		if (itemData != null)
		{
			RealItem haveitem = null;

			if (Item.eITEM_TYPE.EquipItem == itemData.ItemData.GetItemType())
				haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem(itemData.ItemData.GetSubType());
			else if (Item.eITEM_TYPE.CosEquipItem == itemData.ItemData.GetItemType())
				haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem(itemData.ItemData.GetSubType());

			if (TooltipMgr.Instance != null)
				TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.left, _itemID);
		}
	}
}
