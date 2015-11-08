using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GachaLineupPetPopupController : MonoBehaviour
{
	public GameObject listItemPrefab;
	public SimpleSprite upArrowSprite;
	public SimpleSprite downArrowSprite;
	public UIScrollList scrollList;
	public SpriteText txtTitle;
	public UIButton btnClose;
	public SpriteText txtIcon;
	public SpriteText txtName;
	public SpriteText txtGrade;
	public SpriteText txtDesc;


	List<GachaPetLineupListItemController> listLineupListItemCon = new List<GachaPetLineupListItemController>();
	

	// Use this for initialization
	void Start () 
	{
		if (txtTitle != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);

		if (txtIcon != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtIcon);

		if (txtGrade != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtGrade);

		btnClose.SetInputDelegate(CloseProcess);
	}


	void Update()
	{
		if (scrollList != null)
			if (scrollList.gameObject.activeSelf)
				CheckMorelistMark();
	}

	void SetString()
	{
		txtIcon.Text	= AsTableManager.Instance.GetTbl_String(2169);
		txtName.Text    = AsTableManager.Instance.GetTbl_String(2733);
		txtGrade.Text	= AsTableManager.Instance.GetTbl_String(2168);
		txtDesc.Text	= AsTableManager.Instance.GetTbl_String(4104);;
	}

	public void ShowLineUp(List<int> listItem, List<int> listQuantities)
	{
		AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);

		SetString();

		listLineupListItemCon.Clear();

		int count = 0;

		foreach (int itemID in listItem)
		{
			Tbl_Pet_Record petRecord = AsTableManager.Instance.GetPetRecord(itemID);

			IUIListObject newListItem = scrollList.CreateItem(listItemPrefab);

			newListItem.gameObject.name = listLineupListItemCon.Count.ToString();

			GachaPetLineupListItemController listItemController = newListItem.gameObject.GetComponent<GachaPetLineupListItemController>();

			listLineupListItemCon.Add(listItemController);

			//Item nowItem = ItemMgr.ItemManagement.GetItem(itemData.m_iID);

			int quentity = listQuantities.Count > count ? listQuantities[count] : 0;

			if (listItemController != null)
			{
				//GameObject objIcon = AsCashStore.GetItemIcon(itemData.m_iDestID.ToString(), quentity);

				GameObject prefabPetIcon = ResourceLoad.LoadGameObject(petRecord.Icon);

				GameObject objIconInstantiate = GameObject.Instantiate(prefabPetIcon) as GameObject;


				//StringBuilder sbItemName = new StringBuilder();
				//sbItemName.Insert(0, nowItem.ItemData.GetGradeColor().ToString());
				//sbItemName.AppendFormat("{0}", AsTableManager.Instance.GetTbl_String(nowItem.ItemData.nameId));

				string name  = petRecord.Name;
				string level = petRecord.StarGrade.ToString();
				string desc	 = petRecord.Desc == int.MaxValue ? string.Empty : AsTableManager.Instance.GetTbl_String(petRecord.Desc);

				listItemController.SetInfo(objIconInstantiate, name, level, desc);
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

	public void ValueChangedDelegate(IUIObject obj)
	{
	

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
