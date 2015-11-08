using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuGacha : CashStoreMenu
{
	public SpriteText txtAd = null;
	public UIButton btnLineup = null;
	public UIButton btnNote = null;
	public GameObject objLineupPopupPrefab;
	public GameObject objLineupPopup;

	protected List<Store_Item_Info_Table> listFilteredItem = new List<Store_Item_Info_Table>();

	public virtual void ShowNoteWebPage(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (AsHudDlgMgr.Instance.IsOpenCashStore)
				if (AsHudDlgMgr.Instance.cashStore.IsLockInput == false)
					Application.OpenURL("http://arksphere.wemade.jp/bannerLink/09/");
		}
	}

	public virtual void ShowLineupPopup(eCashStoreMainCategory _mainCategory)
	{
		if (AsHudDlgMgr.Instance.IsOpenCashStore)
			AsHudDlgMgr.Instance.cashStore.LockInput(true);
		
		objLineupPopup = GameObject.Instantiate(objLineupPopupPrefab) as GameObject;

		objLineupPopup.transform.parent = AsHudDlgMgr.Instance.cashStore.transform;
		objLineupPopup.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);

		Dictionary<int, Item> dicLineupItem = new Dictionary<int, Item>();

		GachaLineupPopupController popController = objLineupPopup.GetComponent<GachaLineupPopupController>();


		#region - for array -
		//foreach (Store_Item_Info_Table itemRecord in filteredListItem)
		//{
		//    Item item = ItemMgr.ItemManagement.GetItem(itemRecord.ID);
		//    ItemData itemData = item.ItemData;

		//    Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(itemData.m_iItem_Rand_ID);

		//    if (lotteryRecord == null)
		//        continue;

		//    foreach (int lotteryItemID in lotteryRecord.idlist)
		//    {
		//        Tbl_RandItem_Record randomItemRecord = AsTableManager.Instance.GetTbl_RandItem_Record(lotteryItemID);

		//        if (randomItemRecord == null)
		//            continue;

		//        foreach (int itemID in randomItemRecord.idlist)
		//        {
		//            Item pickItem = ItemMgr.ItemManagement.GetItem(itemID);

		//            if (!dicLineupItem.ContainsKey(itemID))
		//                dicLineupItem.Add(itemID, pickItem);
		//        }
		//    }
		//}
		#endregion

		#region - only one -
		if (listFilteredItem.Count <= 0)
			return;

		Item item = ItemMgr.ItemManagement.GetItem(listFilteredItem[0].ID);
		ItemData itemData = item.ItemData;
        List<Item> listItem = new List<Item>();
        List<int>  listQuantity = new List<int>();

		Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(itemData.m_iItem_Rand_ID);

		if (lotteryRecord == null)
			return;

		foreach (int lotteryItemID in lotteryRecord.idlist)
		{
			Tbl_RandItem_Record randomItemRecord = AsTableManager.Instance.GetTbl_RandItem_Record(lotteryItemID);

			if (randomItemRecord == null)
				continue;

			int count = 0;
            foreach (int itemID in randomItemRecord.idlist)
            {
                if (itemID == 0)
                    continue;

                Item pickItem = ItemMgr.ItemManagement.GetItem(itemID);

                listItem.Add(pickItem);
                listQuantity.Add(randomItemRecord.quentities[count++]);
            }
		}
		#endregion

        popController.ShowLineUp(_mainCategory, listItem, listQuantity);
	}
}
