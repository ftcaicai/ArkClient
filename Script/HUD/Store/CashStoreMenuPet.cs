using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuPet : CashStoreMenuGacha
{
	public UIRadioBtn[] btnRadios = null;
	public int[] btnRadioNameIDs = null;
	public eCashStoreSubCategory[] subCategories = null;

	public UIButton gachaBtn;
	public SimpleSprite upArrowForCategory;
	public SimpleSprite downArrowForCategory;

	eCashStoreSubCategory nowSubCatogory = eCashStoreSubCategory.PETEGG;

	private GameObject prevCategory = null;
	private Dictionary<eCashStoreSubCategory, UIRadioBtn> dicRadioBtn = new Dictionary<eCashStoreSubCategory, UIRadioBtn>();
	private Dictionary<int, List<Store_Item_Info_Table>> dicNowSetItem = new Dictionary<int, List<Store_Item_Info_Table>>();
	private Dictionary<int, List<Store_Item_Info_Table>> dicEggSetItem = new Dictionary<int, List<Store_Item_Info_Table>>();
	private Dictionary<int, List<Store_Item_Info_Table>> dicAccSetItem = new Dictionary<int, List<Store_Item_Info_Table>>();
	private Dictionary<int, string> dicNowSetName = new Dictionary<int, string>();
	private Dictionary<int, string> dicEggSetName = new Dictionary<int, string>();
	private Dictionary<int, string> dicAccSetName = new Dictionary<int, string>();
	private List<Store_Item_Info_Table> listFeedItem = new List<Store_Item_Info_Table>();
	private List<Store_Item_Info_Table> listFilteredItem = new List<Store_Item_Info_Table>();

	public override void InitMenu(eCLASS _userClass)
	{
		base.InitMenu(_userClass);

		InitComponent();

		ConstructDictionary();
	
		btnRadios[0].Value = true;

		CategoryRadioForcedOn(eCashStoreSubCategory.PETEGG);
	}

	void SelectMainListItem(int _listItemIdx)
	{
		mainScrollList.DidSelect(mainScrollList.GetItem(_listItemIdx));
	}

	void Update()
	{
		if (mainScrollList != null)
		{
			if (mainScrollList.Count >= 1)
				CheckMorelistMark(mainScrollList);
		}
	}

	public void ConstructDictionary()
	{
		dicEggSetItem.Clear();
		dicEggSetName.Clear();

		dicAccSetItem.Clear();
		dicAccSetName.Clear();

		listFeedItem.Clear();

		// for egg
		List<Store_Item_Info_Table> listItemInfo = null;

		// for egg
		listFilteredItem = new List<Store_Item_Info_Table>();

		List<Store_Item_Info_Table> listPetEgg = AsTableManager.Instance.GetCashStoreItems(eCLASS.All, eCashStoreMainCategory.PET, eCashStoreSubCategory.PETEGG);

		foreach (Store_Item_Info_Table itemInfo in listPetEgg)
		{
			Item item = ItemMgr.ItemManagement.GetItem(itemInfo.ID);

			listFilteredItem.Add(itemInfo);
		}


		// for acc
		listItemInfo = AsTableManager.Instance.GetCashStoreItems(eCLASS.PET, eCashStoreMainCategory.PET, eCashStoreSubCategory.PETACC);

		foreach (Store_Item_Info_Table info in listItemInfo)
		{
			if (!dicAccSetItem.ContainsKey(info.SetItemID))
			{
				dicAccSetItem.Add(info.SetItemID, new List<Store_Item_Info_Table>());
				dicAccSetItem[info.SetItemID].Add(info);

				// get set name id
				Tbl_CashStoreCostume_Record setRecord = AsTableManager.Instance.GetTbl_CashstoreCostume_Record(info.SetItemID);

				if (setRecord == null)
				{
					Debug.LogWarning("Not Exist Set ID =" + info.SetItemID);
					continue;
				}

				int setNameID = setRecord.nameID;

				// make set name dic
				dicAccSetName.Add(info.SetItemID, AsTableManager.Instance.GetTbl_String(setNameID));

				Debug.LogWarning("Add Acc Set Name" + AsTableManager.Instance.GetTbl_String(setNameID));
			}
			else
			{
				dicAccSetItem[info.SetItemID].Add(info);
			}
		}

		// for feed
		listFeedItem = AsTableManager.Instance.GetCashStoreItems(eCLASS.PET, eCashStoreMainCategory.PET, eCashStoreSubCategory.PETFOOD);
	}

	public void InitComponent()
	{
		int count = 0;
		foreach (UIRadioBtn radio in btnRadios)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(radio.spriteText);
			radio.Text = AsTableManager.Instance.GetTbl_String(btnRadioNameIDs[count]);
			radio.Data = count;
			count++;

			radio.SetInputDelegate(RadioBtnInput);
		}

		dicRadioBtn.Clear();

		for (int i = 0; i < subCategories.Length; i++)
			dicRadioBtn.Add(subCategories[i], btnRadios[i]);

		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtAd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnLineup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNote.spriteText);


		txtAd.Text = AsTableManager.Instance.GetTbl_String(2235);
		btnLineup.Text = AsTableManager.Instance.GetTbl_String(2236);
		btnNote.Text = AsTableManager.Instance.GetTbl_String(2237);

		gachaBtn.SetInputDelegate(OpenGacha);

		btnLineup.SetInputDelegate(ProcessLineup);

		btnNote.SetInputDelegate(ShowNoteWebPage);
	}

	public override void Reset()
	{
		base.Reset();

	}

	public void RadioBtnInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			nowSubCatogory = subCategories[(int)ptr.targetObj.Data];

            AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

			CategoryRadioForcedOn(nowSubCatogory);
		}
	}

	void CategoryRadioForcedOn(eCashStoreSubCategory _subCategory)
	{
		if (_subCategory == eCashStoreSubCategory.PETEGG)
			SettingForEgg(_subCategory);
		else
			Setting(_subCategory);

		// setting for desc
		SettingDescription(_subCategory);
	}

	void SettingDescription(eCashStoreSubCategory _subCategory)
	{
		if (_subCategory == eCashStoreSubCategory.PETEGG)
			txtAd.Text = AsTableManager.Instance.GetTbl_String(2735);
		else if (_subCategory == eCashStoreSubCategory.PETACC)
			txtAd.Text = AsTableManager.Instance.GetTbl_String(2736);
		else if (_subCategory == eCashStoreSubCategory.PETFOOD)
			txtAd.Text = AsTableManager.Instance.GetTbl_String(84711);
		else if (_subCategory == eCashStoreSubCategory.PETPOTION)
			txtAd.Text = AsTableManager.Instance.GetTbl_String(84712);
	}

	public void SettingForEgg(eCashStoreSubCategory _subCategory)
	{
		if (gachaBtn != null) gachaBtn.gameObject.SetActive(true);

		mainScrollList.ClearListSync(true);

		mainScrollList.gameObject.SetActive(false);

		btnNote.gameObject.SetActive(true);

		btnLineup.gameObject.SetActive(true);

		listFilteredItem.Clear();

		nowSubCatogory = _subCategory;

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(eCLASS.PET, eCashStoreMainCategory.PET, nowSubCatogory);

		if (listItem.Count >= 1)
		{
			listFilteredItem.Add(listItem[0]);

			gachaBtn.spriteText.Text = listItem[0].Price.ToString();
		}

		UpdateArrowPos();
	}

	public void Setting(eCashStoreSubCategory _subCategory)
	{
		if (gachaBtn != null) gachaBtn.gameObject.SetActive(false);

		btnNote.gameObject.SetActive(false);

		btnLineup.gameObject.SetActive(false);

		mainScrollList.gameObject.SetActive(true);

		AddListItem(_subCategory);

		UpdateArrowPos();
	}

	public void AddListItem(eCashStoreSubCategory _subCategory)
	{
		mainScrollList.ClearListSync(true);

		nowSubCatogory = _subCategory;

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(eCLASS.PET, eCashStoreMainCategory.PET, nowSubCatogory);

		listFilteredItem = new List<Store_Item_Info_Table>();

		foreach (Store_Item_Info_Table itemInfo in listItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem(itemInfo.ID);

			if (item == null)
				continue;

			listFilteredItem.Add(itemInfo);
		}

		addItemToList(listFilteredItem);
	}

	public void UpdateArrowPos()
	{
		if (upArrowSprite == null || downArrowSprite == null)
			return;

		Vector3 scrollPos = mainScrollList.transform.localPosition;

		float newUpPosX = scrollPos.x + mainScrollList.viewableArea.x * 0.5f;
		float newUpPosY = scrollPos.y + mainScrollList.viewableArea.y * 0.5f - upArrowSprite.height * 0.5f;
		float nweDownPosX = scrollPos.x + mainScrollList.viewableArea.x * 0.5f;
		float newDownPosY = scrollPos.y - mainScrollList.viewableArea.y * 0.5f + upArrowSprite.height * 0.5f;

		upArrowSprite.gameObject.transform.localPosition = new Vector3(newUpPosX, newUpPosY, -1.0f);
		downArrowSprite.gameObject.transform.localPosition = new Vector3(nweDownPosX, newDownPosY, -1.0f);
	}

	//public override void ClickMainListItemProcess(StoreItemInfoUI _storeItemInfoUI)
	//{
	//    // only process hair item
	//    if (nowSubCatogory == eCashStoreSubCategory.PETACC)
	//    {
	//        if (null != AsHudDlgMgr.Instance.cashStore)
	//        {
	//            AsHudDlgMgr.Instance.cashStore.LockInput(true);
	//            AsHudDlgMgr.Instance.cashStore.ShowLoadingIndigator(string.Empty);
	//        }
	//    }
	//}

	public override void SetCategory(eCashStoreSubCategory _eSubCategory)
	{
		UIRadioBtn btn = dicRadioBtn.ContainsKey(_eSubCategory) ? dicRadioBtn[_eSubCategory] : null;

		if (btn != null)
		{
			btn.Value = true;
			nowSubCatogory = _eSubCategory;
			CategoryRadioForcedOn(_eSubCategory);
			listCategory.SetSelectedItem(0);
		}
	}

	public void OpenGacha(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && AsCashStore.Instance.IsLockInput == false)
		{
			AsCashStore.Instance.LockInput(true);

			if (listFilteredItem.Count >= 1)
			{
				int itemID = listFilteredItem[0].ID;

				Item item = ItemMgr.ItemManagement.GetItem(itemID);

				string itemName = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);

				string itemPrice = listFilteredItem[0].Price.ToString();

				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126),
											 string.Format(AsTableManager.Instance.GetTbl_String(1411), itemName, itemPrice),
											 this, "OpenFreeGacha", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
		}
	}

	void OpenFreeGacha()
	{
		int freeGachaCoupon = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("Gachare_ChargeItem").Value;

		List<RealItem> slotList = ItemMgr.HadItemManagement.Inven.GetHaveInvenItemsID(freeGachaCoupon);

		int slot = 0;

		if (AsCashStore.Instance.FreeGachaPoint <= 0)
			slot = slotList.Count == 0 ? 0 : slotList[0].getSlot;

		StoreItemInfoUI info = new StoreItemInfoUI(Store_Item_Type.GachaItem, listFilteredItem[0].ID.ToString(), listFilteredItem[0].Price.ToString(), listFilteredItem[0].Count, listFilteredItem[0].Key, string.Empty);

		if (AsHudDlgMgr.Instance.IsOpenCashStore == true)
			AsHudDlgMgr.Instance.cashStore.SetPrevStoreItemInfoUI(info);

		AsCommonSender.SendRequestBuyCashItemJpn(listFilteredItem[0].Key, listFilteredItem[0].ID, 1, slot);
	}

	void Cancel()
	{
		AsCashStore.Instance.LockInput(false);
	}

	public void ProcessLineup(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && objLineupPopup == null && AsCashStore.Instance.IsLockInput == false)
		{
			// lock input
			if (AsHudDlgMgr.Instance.IsOpenCashStore)
				AsHudDlgMgr.Instance.cashStore.LockInput(true);

			objLineupPopup = GameObject.Instantiate(objLineupPopupPrefab) as GameObject;

			objLineupPopup.transform.parent = AsHudDlgMgr.Instance.cashStore.transform;
			objLineupPopup.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);

			GachaLineupPetPopupController popupController = objLineupPopup.GetComponent<GachaLineupPetPopupController>();

			List<int> listItemID = new List<int>();
			List<int> listItemCount = new List<int>();

			foreach (Store_Item_Info_Table storeInfo in listFilteredItem)
			{
				Item item = ItemMgr.ItemManagement.GetItem(storeInfo.ID);

				if (item == null)
					continue;

				int lotteryID = item.ItemData.m_iItem_Rand_ID;

				Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(lotteryID);

				if (lotteryRecord == null)
					continue;

				foreach (int randomID in lotteryRecord.idlist)
				{
					Tbl_RandItem_Record randomRecord = AsTableManager.Instance.GetTbl_RandItem_Record(randomID);

					if (randomRecord == null)
						continue;

					foreach (int innerItemID in randomRecord.idlist)
					{
						Item innerItem = ItemMgr.ItemManagement.GetItem(innerItemID);

						listItemID.Add(innerItem.ItemData.m_iItem_Rand_ID);
						listItemCount.Add(1);
					}
				}
			}

			AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

			popupController.ShowLineUp(listItemID, listItemCount);
		}
	}

}
