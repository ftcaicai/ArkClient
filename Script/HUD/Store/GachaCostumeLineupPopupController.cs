using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GachaCostumeLineupTextInfo
{
	public SpriteText spriteText;
	public int textID;
}

public class GachaCostumeLineupPopupController : MonoBehaviour
{
	public GameObject listItemPrefab;
	public SimpleSprite upArrowSprite;
	public SimpleSprite downArrowSprite;
	public UIScrollList scrollList;
	public SpriteText txtTitle;
	public UIButton btnClose;

	public GachaCostumeLineupTextInfo[] textInfo;

	// Use this for initialization
	void Start () 
	{
		if (txtTitle != null)
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);

		foreach (GachaCostumeLineupTextInfo info in textInfo)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(info.spriteText);
			info.spriteText.Text = AsTableManager.Instance.GetTbl_String(info.textID);
		}

		txtTitle.Text = AsTableManager.Instance.GetTbl_String(2253);

		btnClose.SetInputDelegate(CloseProcess);
	}

	void Update()
	{
		if (scrollList != null)
			if (scrollList.gameObject.activeSelf)
				CheckMorelistMark();
	}

	public void ShowLineUp(List<int> _listItem)
	{
		List<int> filteredListItem = new List<int>();
		List<string> forIconFilteredListItem = new List<string>();

		foreach (int id in _listItem)
		{
			Item item = ItemMgr.ItemManagement.GetItem(id);

			if (forIconFilteredListItem.Contains(item.ItemData.m_strIcon))
				continue;
			else
				forIconFilteredListItem.Add(item.ItemData.m_strIcon);

			if (!filteredListItem.Contains(id))
				filteredListItem.Add(id);
		}

		List<int> dummyList =  new List<int>();
		for (int count = 0; count < filteredListItem.Count; count++)
		{
			dummyList.Add(filteredListItem[count]);

			if (dummyList.Count >= 5)
			{
				AddListItem(dummyList);
				dummyList.Clear();
			}
		}

		if (dummyList.Count >= 1)
		{
			AddListItem(dummyList);
			dummyList.Clear();
		}
	}

	void AddListItem(List<int> listItem)
	{
		IUIListObject newListItem = scrollList.CreateItem(listItemPrefab);

		GachaCostumeLineupListItemController listItemController = newListItem.gameObject.GetComponent<GachaCostumeLineupListItemController>();

		listItemController.AddItemIcons(listItem.ToArray());
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
}
