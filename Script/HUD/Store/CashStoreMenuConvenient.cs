using UnityEngine;
using System.Collections.Generic;

public class CashStoreMenuConvenient : CashStoreMenuCostume {

	public UIRadioBtn[] btnRadios = null;
	public int[] btnRadioNameIDs = null;
	public eCashStoreSubCategory[] subCategories = null;
	public GameObject hairViewObject;
	public SimpleSprite upArrowForCategory;
	public SimpleSprite downArrowForCategory;

	public float scrollPosHairY;
	public float scrollPosOtherY;
	public float scrollHairHeight;
	public float scrollOtherHeight;

	bool playSelectSound = false;

	eCashStoreSubCategory nowSubCatogory = eCashStoreSubCategory.HAIR;

	private GameObject prevCategory = null;
	private Dictionary<eCashStoreSubCategory, UIRadioBtn> dicRadioBtn = new Dictionary<eCashStoreSubCategory, UIRadioBtn>();
	private Dictionary<int, List<Store_Item_Info_Table>> dicHairSet = new Dictionary<int, List<Store_Item_Info_Table>>();
	private Dictionary<int, string> dicHairSetName = new Dictionary<int, string>();

	public override void InitMenu(eCLASS _userClass)
	{
		userClass = _userClass;

		InitComponent();

		ConstructHairList();

		listCategory.SetValueChangedDelegate(CategoryValueChange);

		ForcedSelectCategory(eCashStoreSubCategory.HAIR);
	}

	void SelectFirstCategory()
	{
		nowSubCatogory = eCashStoreSubCategory.HAIR;
		btnRadios[0].Value = true;
		playSelectSound = false;
		listCategory.SetSelectedItem(0);
	}

	void Update()
	{
		if (mainScrollList.Count >= 1)
			CheckMorelistMark(mainScrollList);

		UpdateForModel();

		CheckMorelistMarkForCategory();
	}

	protected void UpdateForModel()
	{
		if (null != AsHudDlgMgr.Instance.cashShopEntity &&
			eModelLoadingState.Finished == AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState() &&
			true == m_isShowInit &&
			null != backgroundObject)
		{
			if (m_fEffectDlayTime < 0.1f)
			{
				m_fEffectDlayTime += Time.deltaTime;
				return;
			}

		
			AsHudDlgMgr.Instance.cashShopEntity.transform.position = backgroundObject.transform.position + new Vector3(-0.1f, -3.3f, -4f);
		
			AsHudDlgMgr.Instance.cashShopEntity.transform.rotation = Quaternion.Euler(25f, -147f, 15.5f) * Quaternion.AngleAxis(m_PartsRot, Vector3.up);
			ResourceLoad.SetLayerHierArchy(AsHudDlgMgr.Instance.cashShopEntity.transform, LayerMask.NameToLayer("GUI"));

			if (null != AsHudDlgMgr.Instance.cashStore)
			{
				AsHudDlgMgr.Instance.cashStore.LockInput(false);
				AsHudDlgMgr.Instance.cashStore.HideLoadingIndigator();
			}
			m_isShowInit = false;
		}
		else
		{
			if (m_isLeftRot)
				SetPartsLeftRot();

			if (m_isRightRot)
				SetPartsRightRot();
		}
	}

	public void ConstructHairList()
	{
		dicHairSet.Clear();
		dicHairSetName.Clear();

		List<Store_Item_Info_Table> listItemInfo = AsTableManager.Instance.GetCashStoreItems(userClass, eCashStoreMainCategory.CONVENIENCE,eCashStoreSubCategory.HAIR);

		foreach (Store_Item_Info_Table info in listItemInfo)
		{
			if (!dicHairSet.ContainsKey(info.SetItemID))
			{
				dicHairSet.Add(info.SetItemID, new List<Store_Item_Info_Table>());
				dicHairSet[info.SetItemID].Add(info);

				// get set name id
				Tbl_CashStoreCostume_Record setRecord = AsTableManager.Instance.GetTbl_CashstoreCostume_Record(info.SetItemID);

				if (setRecord == null)
				{
					Debug.LogWarning("Not Exist Set ID =" +info.SetItemID);
					continue;
				}

				int setNameID = setRecord.nameID;

				// make set name dic
				dicHairSetName.Add(info.SetItemID, AsTableManager.Instance.GetTbl_String(setNameID));

				Debug.LogWarning("Add Set Name" + AsTableManager.Instance.GetTbl_String(setNameID));
			}
			else
			{
				dicHairSet[info.SetItemID].Add(info);
			}
		}
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
	}

	public override void Reset()
	{
		base.Reset();

		playSelectSound = false;

		if (null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
			AsHudDlgMgr.Instance.cashShopEntity = null;
		}
	}

	public override void SetCategory(eCashStoreSubCategory _eSubCategory)
	{
		UIRadioBtn btn = dicRadioBtn.ContainsKey(_eSubCategory) ? dicRadioBtn[_eSubCategory] : null;

		if (btn != null)
		{
			nowSubCatogory = _eSubCategory;
			btn.Value = true;
			ForcedSelectCategory(_eSubCategory);
		}
	}

	public void RadioBtnInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && AsCashStore.Instance.IsLockInput == false)
		{
			eCashStoreSubCategory eSubCategory = subCategories[(int)ptr.targetObj.Data];

			AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

			ForcedSelectCategory(eSubCategory);
		}
	}

	public void ForcedSelectCategory(eCashStoreSubCategory _eSubCategory)
	{
		if (_eSubCategory == eCashStoreSubCategory.HAIR)
		{
			mainScrollList.ClearListSync(true);
			SettingForHair();
			SelectFirstCategory();
		}
		else
		{
			SettingForOther();
			AddListItem(_eSubCategory);
		}
	}

	public void SettingForHair()
	{
		hairViewObject.SetActive(true);

		listCategory.gameObject.SetActive(true);

		MakeHairList();

		Vector3 scrollPos = mainScrollList.gameObject.transform.localPosition;
		scrollPos.y = scrollPosHairY;
		mainScrollList.gameObject.transform.localPosition = scrollPos;
		mainScrollList.viewableArea = new Vector2(mainScrollList.viewableArea.x, scrollHairHeight);

		leftRot.gameObject.SetActive(true);
		rightRot.gameObject.SetActive(true);

		UpdateArrowPos();
	}

	public void MakeHairList()
	{
		listCategory.ClearListSync(true);

		foreach (int setID in dicHairSetName.Keys)
		{
			UIListItem newListItem = listCategory.CreateItem(objCategoryListItem) as UIListItem;
			newListItem.Text = dicHairSetName[setID];
			newListItem.Data = setID;
		}
	}

	public void SettingForOther()
	{
		// remove character
		if (null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
			AsHudDlgMgr.Instance.cashShopEntity = null;
		}

		listCategory.gameObject.SetActive(false);

		hairViewObject.SetActive(false);

		Vector3 scrollPos = mainScrollList.gameObject.transform.localPosition;
		scrollPos.y = scrollPosOtherY;
		mainScrollList.gameObject.transform.localPosition = scrollPos;
		mainScrollList.viewableArea = new Vector2(mainScrollList.viewableArea.x, scrollOtherHeight);

		leftRot.gameObject.SetActive(false);
		rightRot.gameObject.SetActive(false);

		UpdateArrowPos();
	}

	public void AddListItem(eCashStoreSubCategory _subCategory)
	{
		mainScrollList.ClearListSync(true);

		nowSubCatogory = _subCategory;

		List<Store_Item_Info_Table> listItem = AsTableManager.Instance.GetCashStoreItems(userClass, eCashStoreMainCategory.CONVENIENCE, nowSubCatogory);

		listFilteredItem = new List<Store_Item_Info_Table>();

		int usrLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);

		//int checkLevel = GetGachaLevel(usrLevel);

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

		float newUpPosX		= scrollPos.x + mainScrollList.viewableArea.x * 0.5f;
		float newUpPosY		= scrollPos.y + mainScrollList.viewableArea.y * 0.5f - upArrowSprite.height * 0.5f;
		float nweDownPosX = scrollPos.x + mainScrollList.viewableArea.x * 0.5f;
		float newDownPosY	= scrollPos.y - mainScrollList.viewableArea.y * 0.5f + upArrowSprite.height * 0.5f;

		upArrowSprite.gameObject.transform.localPosition = new Vector3(newUpPosX, newUpPosY, -1.0f);
		downArrowSprite.gameObject.transform.localPosition = new Vector3(nweDownPosX, newDownPosY, -1.0f);
	}

	public void CategoryValueChange(IUIObject obj)
	{
		if (null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			if (eModelLoadingState.Finished != AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState())
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

		if (dicHairSet.ContainsKey(setID))
		{
			// make filtered list
			listFilteredItem = new List<Store_Item_Info_Table>();
			listFilteredItem.AddRange(dicHairSet[setID]);

			// hair items
			addItemToList(listFilteredItem);

			// create character
			CreateRenderTarget(userClass, AsEntityManager.Instance.UserEntity.GetProperty<eGENDER>(eComponentProperty.GENDER), listFilteredItem[0].ID);

			prevCategory = listCategory.SelectedItem.gameObject;

			if (playSelectSound == true)
				AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
			else
				playSelectSound = true;
		
		}
		else
			Debug.LogWarning("not contain key = " + setID);
	}

	public override void ClickMainListItemProcess(StoreItemInfoUI _storeItemInfoUI)
	{
		// only process hair item
		if (nowSubCatogory == eCashStoreSubCategory.HAIR)
		{
			if (null != AsHudDlgMgr.Instance.cashShopEntity)
			{
				if (eModelLoadingState.Finished != AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState())
					return;
			}

			if (null != AsHudDlgMgr.Instance.cashStore)
			{
				AsHudDlgMgr.Instance.cashStore.LockInput(true);
				AsHudDlgMgr.Instance.cashStore.ShowLoadingIndigator(string.Empty);
			}

			CreateRenderTarget(userClass, AsEntityManager.Instance.UserEntity.GetProperty<eGENDER>(eComponentProperty.GENDER), int.Parse(_storeItemInfoUI.itemID));
		}
	}


	public  void CheckMorelistMarkForCategory()
	{
		if (listCategory== null || upArrowForCategory == null || downArrowForCategory == null)
			return;

		if (listCategory.Count > 0 && listCategory.gameObject.activeSelf)
		{
			if (!listCategory.IsShowingLastItem())
			{
				if (downArrowForCategory.IsHidden() == true)
					downArrowForCategory.Hide(false);
			}
			else
			{
				if (downArrowForCategory.IsHidden() == false)
					downArrowForCategory.Hide(true);
			}

			if (!listCategory.IsShowingFirstItem())
			{
				if (upArrowForCategory.IsHidden() == true)
					upArrowForCategory.Hide(false);
			}
			else
			{
				if (upArrowForCategory.IsHidden() == false)
					upArrowForCategory.Hide(true);
			}
		}
		else
		{
			if (downArrowForCategory.IsHidden() == false)
				downArrowForCategory.Hide(true);

			if (upArrowForCategory.IsHidden() == false)
				upArrowForCategory.Hide(true);
		}
	}
}
