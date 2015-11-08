using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuGachaFree : CashStoreMenuGacha
{
	public UIButton btnOpenGacha;
	public SpriteText txtRemainOpenCount;
	public SimpleSprite spriteFreeMark;

	public override void InitMenu(eCLASS _userClass)
	{
		base.InitMenu(_userClass);

		InitComponent();

		btnLineup.SetInputDelegate(ProcessLineup);

		btnOpenGacha.SetInputDelegate(OpenGacha);

		int usrLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
		int checkLevel = GetGachaLevel(usrLevel);

		listFilteredItem = new List<Store_Item_Info_Table>();
		
		List<Store_Item_Info_Table> listFree = AsTableManager.Instance.GetCashStoreItems(_userClass, eCashStoreMainCategory.FREE, eCashStoreSubCategory.NONE);

		foreach (Store_Item_Info_Table itemInfo in listFree)
		{
			Item item  = ItemMgr.ItemManagement.GetItem(itemInfo.ID);

			if (item.ItemData.levelLimit == checkLevel)
				listFilteredItem.Add(itemInfo);
		}

		btnNote.SetInputDelegate(ShowNoteWebPage);

		if (AsCashStore.Instance != null)
			spriteFreeMark.gameObject.SetActive(AsCashStore.Instance.FreeGachaPoint == 1);
	}

	public void InitComponent()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtAd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnLineup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNote.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtRemainOpenCount);

		txtAd.Text = AsTableManager.Instance.GetTbl_String(2268);
		btnLineup.Text = AsTableManager.Instance.GetTbl_String(2271);
		btnNote.Text = AsTableManager.Instance.GetTbl_String(2237);
		btnOpenGacha.Text = AsTableManager.Instance.GetTbl_String(2269);

		UpdateFreeGachaPoint();
	}

	public override void Reset()
	{
		base.Reset();
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

			GachaLineupFreePopupController popupController = objLineupPopup.GetComponent<GachaLineupFreePopupController>();

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

					int count = 0;
					foreach (int itemID in randomRecord.idlist)
					{
						listItemID.Add(itemID);
						if (randomRecord.quentities.Count > count)
							listItemCount.Add(randomRecord.quentities[count++]);
					}
				}
			}

			AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

			popupController.ShowLineUp(listItemID.ToArray(), listItemCount.ToArray());
		}
	}

	public void OpenGacha(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && AsCashStore.Instance.IsLockInput == false)
		{
			AsCashStore.Instance.LockInput(true);

			int totalGachaPoint = UpdateFreeGachaPoint();

			if (totalGachaPoint <= 0)
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2331), this, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
			else
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2326), AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
				this, "OpenFreeGacha", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION, false);
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

	public override int UpdateFreeGachaPoint()
	{
		int freeGachaCoupon = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("Gachare_ChargeItem").Value;

		int gachaCouponItemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(freeGachaCoupon);

		int totalGachaPoint = AsCashStore.Instance.FreeGachaPoint + gachaCouponItemCount;

		txtRemainOpenCount.Text = string.Format(AsTableManager.Instance.GetTbl_String(2270), totalGachaPoint);

		if (AsCashStore.Instance != null)
			spriteFreeMark.gameObject.SetActive(AsCashStore.Instance.FreeGachaPoint == 1);

		return totalGachaPoint;
	}
}
