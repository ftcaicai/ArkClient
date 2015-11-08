using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuGachaCostume : CashStoreMenuCostume
{
	Dictionary<int, List<Store_Item_Info_Table>> dicCostume = new Dictionary<int, List<Store_Item_Info_Table>>();
	Dictionary<int, int> dicCostumeName = new Dictionary<int, int>();
	
	GameObject prevCategory = null;

	bool playCategorySound = false;
	
	public override void InitMenu(eCLASS _userClass)
	{
		base.InitMenu(_userClass);

		InitComponent();
		
		// make costume list
		ConstructCostumeList(_userClass);

		btnLineup.SetInputDelegate(ProcessLineup);

		listCategory.SetValueChangedDelegate(CategoryValueChange);

		if (listCategory.Count >= 1)
			listCategory.SetSelectedItem(0);

		btnNote.SetInputDelegate(ShowNoteWebPage);
	}

	void Update()
	{
		if (listCategory.Count >= 1)
			CheckMorelistMark(listCategory);

		UpdateForModel();
	}

	public void InitComponent()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtAd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnLineup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNote.spriteText);
		txtAd.Text = AsTableManager.Instance.GetTbl_String(2252);
		btnLineup.Text = AsTableManager.Instance.GetTbl_String(2253);
		btnNote.Text = AsTableManager.Instance.GetTbl_String(2237);
	}

	public override void Reset()
	{
		base.Reset();

		playCategorySound = false;

		if (null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
			AsHudDlgMgr.Instance.cashShopEntity = null;
		}
	}

	public void ProcessLineup(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && objLineupPopup == null && AsCashStore.Instance.IsLockInput == false)
			ShowLineupPopup(eCashStoreMainCategory.COSTUME);
	}

	public override void ShowLineupPopup(eCashStoreMainCategory _mainCategory)
	{
		if (listFilteredItem.Count <= 0)
			return;

		AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

		if (AsHudDlgMgr.Instance.IsOpenCashStore)
			AsHudDlgMgr.Instance.cashStore.LockInput(true);

		objLineupPopup = GameObject.Instantiate(objLineupPopupPrefab) as GameObject;
		objLineupPopup.transform.parent = AsHudDlgMgr.Instance.cashStore.transform;
		objLineupPopup.transform.localPosition = new Vector3(0.0f, 0.0f, -13.0f);

		List<int> listItemID = new List<int>();
		
		Item item = ItemMgr.ItemManagement.GetItem(listFilteredItem[0].ID);
		
		if (item == null)
			return;

		int lotteryID = item.ItemData.m_iItem_Rand_ID;

		Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(lotteryID);

		foreach (int randomID in lotteryRecord.idlist)
		{
			Tbl_RandItem_Record randomRecord = AsTableManager.Instance.GetTbl_RandItem_Record(randomID);

			if (randomRecord != null)
				listItemID.AddRange(randomRecord.idlist);
		}

		GachaCostumeLineupPopupController popController = objLineupPopup.GetComponent<GachaCostumeLineupPopupController>();

		if (popController != null)
			popController.ShowLineUp(listItemID);
	}

	public void ConstructCostumeList(eCLASS _userClass)
	{
		dicCostume.Clear();
		dicCostumeName.Clear();

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(_userClass, eCashStoreMainCategory.COSTUME , eCashStoreSubCategory.NONE);

		int usrLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);

		int checkLevel = GetGachaLevel(usrLevel);

		foreach (Store_Item_Info_Table info in listItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem(info.ID);

			if (item == null)
				continue;

			if (item.ItemData.levelLimit <= checkLevel)
			{
				if (!dicCostume.ContainsKey(info.SetItemID))
				{
					dicCostume.Add(info.SetItemID, new List<Store_Item_Info_Table>());
					dicCostumeName.Add(info.SetItemID, AsTableManager.Instance.GetTbl_CashstoreCostume_Record(info.SetItemID).nameID);
				}
				
				dicCostume[info.SetItemID].Add(info);
			}
		}

		MakeCostumeList();
	}

	void MakeCostumeList()
	{
		listCategory.ClearListSync(true);

		// make costume list
		foreach (int setID in dicCostumeName.Keys)
		{
			UIListItem newListItem = listCategory.CreateItem(objCategoryListItem) as UIListItem;
			newListItem.Text = AsTableManager.Instance.GetTbl_String(dicCostumeName[setID]);
			newListItem.Data = setID;
		}
	}

	public void CategoryValueChange(IUIObject obj)
	{

		if( null != AsHudDlgMgr.Instance.cashShopEntity )
		{
			if( eModelLoadingState.Finished != AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState() )
				return;
		}

		if (prevCategory != null)
			if (listCategory.SelectedItem.gameObject == prevCategory)
				return;

		if (null != AsHudDlgMgr.Instance.cashStore)
		{
			AsHudDlgMgr.Instance.cashStore.LockInput(true);
			AsHudDlgMgr.Instance.cashStore.ShowLoadingIndigator(string.Empty);
		}

		int setID = (int)listCategory.SelectedItem.Data;

		if (dicCostume.ContainsKey(setID))
		{
			// make filtered list
			listFilteredItem = new List<Store_Item_Info_Table>();
			listFilteredItem.AddRange(dicCostume[setID]);

			// add gacha
			addItemToList(dicCostume[setID]);

			if (dicCostume[setID].Count <= 0)
				return;

			if (playCategorySound == true)
				AsSoundManager.Instance.PlaySound(AsSoundPath.Cashshop_Category_Costume, Vector3.zero, false);
			else
				playCategorySound = true;

			// create model
			Tbl_CashStoreCostume_Record costumeSetRecord = AsTableManager.Instance.GetTbl_CashstoreCostume_Record(setID);

			if (costumeSetRecord.itemIDs.Length >= 1)
				CreateRenderTarget(userClass, AsEntityManager.Instance.UserEntity.GetProperty<eGENDER>(eComponentProperty.GENDER), costumeSetRecord.itemIDs);

			prevCategory = listCategory.SelectedItem.gameObject;
		}
		else
			Debug.LogWarning("not contain key = " + setID);
	}

	
}
