using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuGachaRadio : CashStoreMenuGacha
{
	public UIRadioBtn[] btnRadios = null;
	public int[] btnRadioNameIDs = null;
	public int[] explainIDs = null;
	public eCashStoreSubCategory[] subCategories = null;
	public GameObject[] subCategoryItmes = null;

	eCashStoreSubCategory nowSubCatogory = eCashStoreSubCategory.EQUIPHIGH;

	Dictionary<eCashStoreSubCategory, UIRadioBtn> dicRadioBtn = new Dictionary<eCashStoreSubCategory, UIRadioBtn>();
	Dictionary<eCashStoreSubCategory, int> dicExplainID = new Dictionary<eCashStoreSubCategory, int>();
	Dictionary<eCashStoreSubCategory, GameObject> dicCategoryItem = new Dictionary<eCashStoreSubCategory, GameObject>();

	public override void InitMenu(eCLASS _userClass)
	{
		base.InitMenu(_userClass);

		InitComponent();
	}

	public void InitComponent()
	{
		dicExplainID.Clear();

		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtAd);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnLineup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNote.spriteText);


		txtAd.Text = AsTableManager.Instance.GetTbl_String(2235);
		btnLineup.Text = AsTableManager.Instance.GetTbl_String(2236);
		btnNote.Text = AsTableManager.Instance.GetTbl_String(2237);

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

		if (subCategories != null)
		{
			for (int i = 0; i < subCategories.Length; i++)
				dicExplainID.Add(subCategories[i], explainIDs[i]);
		}

		dicCategoryItem.Clear();
		if (subCategoryItmes != null)
		{
			for (int i = 0; i < subCategoryItmes.Length; i++)
				dicCategoryItem.Add(subCategories[i], subCategoryItmes[i]);
		}

		nowSubCatogory = eCashStoreSubCategory.EQUIPHIGH;
		mainListItem = subCategoryItmes[0].GetComponent<UIListItemContainer>();
		btnRadios[0].Value = true;
		AddListItem(subCategories[0]);

		btnLineup.SetInputDelegate(ProcessLineup);

		btnNote.AddInputDelegate(ShowNoteWebPage);
	}

	public override void Reset()
	{
		base.Reset();
	}

	public void RadioBtnInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
			AddListItem(subCategories[(int)ptr.targetObj.Data]);
		}
	}

	public override void SetCategory(eCashStoreSubCategory _eSubCategory)
	{
		UIRadioBtn btn = dicRadioBtn.ContainsKey(_eSubCategory) == true ? dicRadioBtn[_eSubCategory] : null;

		if (btn != null)
		{
			btn.Value = true;
			AddListItem(_eSubCategory);
		}
	}

	public void AddListItem(eCashStoreSubCategory _subCategory)
	{
		if (dicExplainID.ContainsKey(_subCategory))
			txtAd.Text = AsTableManager.Instance.GetTbl_String(dicExplainID[_subCategory]);

		mainListItem = dicCategoryItem[_subCategory].GetComponent<UIListItemContainer>();

		mainScrollList.ClearListSync(true);

		nowSubCatogory = _subCategory;

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(userClass, eCashStoreMainCategory.EQUIPMENT, nowSubCatogory);

		listFilteredItem = new List<Store_Item_Info_Table>();

		int usrLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);

		int checkLevel = GetGachaLevel(usrLevel);

		foreach (Store_Item_Info_Table itemInfo in listItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem(itemInfo.ID);

			if (item == null)
				continue;

			if (item.ItemData.levelLimit == checkLevel)
				listFilteredItem.Add(itemInfo);
		}

		addItemToList(listFilteredItem);
	}

	public void ProcessLineup(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && objLineupPopup == null && AsCashStore.Instance.IsLockInput == false)
			ShowLineupPopup(eCashStoreMainCategory.EQUIPMENT);
	}
	
}

