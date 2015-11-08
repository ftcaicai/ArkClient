using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class AsInGameEventAchievementDlg: MonoBehaviour 
{
	public int 			npcID;
	public int          eventID;
	public Tbl_Event_Record nowEventRecord;

	public UIButton		btnClose;
	public UIButton     btnBackToEventList;
	public SpriteText	txtTitle;
	public SpriteText   txtTitleSub1;
	public SpriteText   txtTitleSub2;
	//public SpriteText   txtInfo;
	public GameObject   rewardInfoListItemPrefab;
	public GameObject   rewardListItemPrefab;
	
	public UIScrollList rewardInfoScroll;
	public UIScrollList rewardScroll;
	public GameObject   objParent;
	
	void Start()
	{
		btnClose.SetInputDelegate(CloseBtnInputProcess);
		btnBackToEventList.SetInputDelegate(BackToEventBtnInputProcess);

		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnBackToEventList.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitleSub1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitleSub2);
		btnBackToEventList.Text = AsTableManager.Instance.GetTbl_String(2155);

		txtTitleSub1.Text = AsTableManager.Instance.GetTbl_String(4263);
		txtTitleSub2.Text = AsTableManager.Instance.GetTbl_String(4264);
	}
	
	public void Show(int _npcID, int _eventID)
	{
		AsSoundManager.Instance.PlaySound(AsSoundPath.WindowOpen, Vector3.zero, false, false);

		npcID = _npcID;
		eventID = _eventID;

		nowEventRecord = AsTableManager.Instance.GetTbl_Event(_eventID);

		txtTitle.Text = AsTableManager.Instance.GetTbl_String(nowEventRecord.titleID);

		foreach (EventAchievement achieve in nowEventRecord.listEventAchievement)
			AddRewardItem(achieve);

		UpdateRewardBtn();

		// info
		UIListItemContainer item = rewardInfoScroll.CreateItem(rewardInfoListItemPrefab, 0, true) as UIListItemContainer;
		
		//txtInfo = item.gameObject.GetComponentInChildren<SpriteText>();
		//txtInfo.Text = AsTableManager.Instance.GetTbl_String(nowEventRecord.txtID);

		item.Text = RepairString(AsTableManager.Instance.GetTbl_String(nowEventRecord.txtID), item.spriteText);

		//item.Text = AsTableManager.Instance.GetTbl_String(nowEventRecord.txtID);
		

		item.UpdateCollider();
		item.UpdateCamera();
	}

	string RepairString(string _orgString, SpriteText _spriteText)
	{
		StringBuilder sb = new StringBuilder();

		Debug.LogWarning("Max width = " + _spriteText.maxWidth);

		string[] splits = _orgString.Split('\n');

		foreach (string sz in splits)
		{
			string szTemp = sz.Replace(" ", "");

			sb.Append(szTemp);
		}

		return sb.ToString();
	}
	
	// close
	void CloseBtnInputProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			Close();
	}
	
	// to info list
	void BackToEventBtnInputProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsHudDlgMgr.Instance.OpenInGameEventListDlg(npcID);
			Close(true);
		}	
	}

	public void Close(bool _backToList = false)
	{
		if (_backToList == false)
			AsSoundManager.Instance.PlaySound(AsSoundPath.WindowClose, Vector3.zero, false, false);
		
		Destroy(objParent);
	}

	// tooltip
	void ClickAchievementProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			EventRewardListItemControll itemController = ptr.targetObj.gameObject.GetComponentInChildren<EventRewardListItemControll>();

			TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.normal, itemController.eventAchievement.itemID);
		}
	}

	void ClickRewardProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			byte idx = (byte)ptr.targetObj.Data;
			AsCommonSender.SendRewardInGameEvent(eventID, idx);
			AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
		}
	}
	
	void AddRewardItem(EventAchievement _achieve)
	{
		int scrollListCount = rewardScroll.Count;
		UIListItem listItem = rewardScroll.CreateItem(rewardListItemPrefab, scrollListCount, true) as UIListItem;
		
		EventRewardListItemControll itemController = listItem.gameObject.GetComponentInChildren<EventRewardListItemControll>();

		listItem.SetOffset(new Vector3(0.0f, 0.0f, -1.0f));

		AsLanguageManager.Instance.SetFontFromSystemLanguage(itemController.btnReward.spriteText);
		itemController.btnReward.Text = AsTableManager.Instance.GetTbl_String(1312);

		// icon
		Item itemData = ItemMgr.ItemManagement.GetItem(_achieve.itemID);
		GameObject icon = itemData.GetIcon();
		GameObject objIconInstantiate = GameObject.Instantiate( icon) as GameObject;
		UISlotItem slotItem = objIconInstantiate.GetComponent<UISlotItem>();
		itemController.icon = slotItem;
		GameObject.Destroy(itemController.icon.coolTime);
		objIconInstantiate.transform.parent = itemController.objIconBg.transform;
		objIconInstantiate.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);

		// delegate
		listItem.SetInputDelegate(ClickAchievementProcess);
		itemController.btnReward.SetInputDelegate(ClickRewardProcess);

		// data setting
		itemController.btnReward.Data = (byte)scrollListCount;
		itemController.eventAchievement = _achieve;
	}

	public void UpdateRewardBtn()
	{
		List<IUIListObject> list = rewardScroll.GetItems();
		int idx = 0;

		foreach (UIListItem listItem in list)
		{
			EventRewardListItemControll controller = listItem.gameObject.GetComponentInChildren<EventRewardListItemControll>();
			EventAchievement achievement = controller.eventAchievement;

			Item item = ItemMgr.ItemManagement.GetItem(achievement.itemID);
			int itemTotalCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(achievement.itemID);

			listItem.gameObject.name = achievement.itemID.ToString();

			StringBuilder sb = new StringBuilder(AsTableManager.Instance.GetTbl_String(item.ItemData.nameId));
			sb.Append("(");
			sb.Append(itemTotalCount.ToString());
			sb.Append("/");
			sb.Append(achievement.itemCount.ToString());
			sb.Append(")");
			listItem.spriteText.Text = sb.ToString();

			Color orgAchieveColor = listItem.spriteText.Color;

			UIListItem uiListItem = listItem as UIListItem;

			if (achievement.itemCount > itemTotalCount)
			{
				// reward
				controller.btnReward.gameObject.SetActive(false);

				uiListItem.spriteText.Color = new Color(1.0f, 0.51f, 0.0f, 1.0f);

				// icon
				controller.objShade.SetActive(true);
			}
			else
			{
				// reward
				controller.btnReward.gameObject.SetActive(true);
				controller.objShade.SetActive(false);
			}

			idx++;
		}
	}

}
