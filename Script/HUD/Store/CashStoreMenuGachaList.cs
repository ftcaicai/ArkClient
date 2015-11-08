using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuGachaList : CashStoreMenuGacha
{
	public UIButton btnOpenGacha;

	public override void InitMenu(eCLASS _userClass)
	{
		base.InitMenu(_userClass);

		InitComponent();

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(_userClass, eCashStoreMainCategory.WEAPON , eCashStoreSubCategory.NONE);
		listFilteredItem = new List<Store_Item_Info_Table>();

		int usrLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);

		int checkLevel = GetGachaLevel(usrLevel);

		foreach (Store_Item_Info_Table info in listItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem(info.ID);

			if (item == null)
				continue;

			if (item.ItemData.levelLimit == checkLevel)
				listFilteredItem.Add(info);
		}

		addItemToList(listFilteredItem);

		btnLineup.SetInputDelegate(ProcessLineup);

		btnNote.SetInputDelegate(ShowNoteWebPage);
	}

	public void InitComponent()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtAd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnLineup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNote.spriteText);
		txtAd.Text = AsTableManager.Instance.GetTbl_String(2235);
		btnLineup.Text = AsTableManager.Instance.GetTbl_String(2236);
		btnNote.Text = AsTableManager.Instance.GetTbl_String(2237);
	}

	public override void Reset()
	{
		base.Reset();
	}

	public void ProcessLineup(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && objLineupPopup == null && AsCashStore.Instance.IsLockInput == false)
			ShowLineupPopup(eCashStoreMainCategory.WEAPON);
	}

}
