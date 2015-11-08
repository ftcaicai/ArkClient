using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class QuestBookController : UIMessageBase
{
	public enum QuestBookMode
	{
		ACCEPTED,
		DAILY,
		MAX,
	}

	public float adPressTime = 1.0f;
	public float adCoolTime  = 3.0f;
	public UIPanelTab[] tabPanels = null;
	public SimpleSprite[] widthSprites = null;
	public UIButton buttonClose = null;
	public QuestBookMode nowMode = QuestBookMode.ACCEPTED;
	public UIScrollList questList;
	public GameObject questItemPrefab;
	public GameObject dailyQuestNothingText;
	public SpriteText noAcceptQuest = null;
	public SpriteText m_TextTitle;
	public SpriteText m_TextAccepted;
	public SpriteText m_TextDaily;

	private float backgroundWidth = 0.0f;
	private GameObject questItem;
	private List<ArkQuest> listQuest = new List<ArkQuest>();
	private Dictionary<GameObject, int> dicQuestListItem = new Dictionary<GameObject, int>();
	public float QuestBookWidth { get { return backgroundWidth; } }

	private float	pressTime	= 0;
	private float	nowAdCoolTime	= 0.0f;
	private Vector3	pressPos	= Vector3.zero;
	private bool	startPress	= false;
	private bool	pressing	= false;
	private bool	showedAdvertise	= false;
	private GameObject pressObject	= null;

	// Use this for initialization
	void Start()
	{
		CreateQuestBookItem();
		buttonClose.AddInputDelegate(ButtonEvent);

		questList.SetValueChangedDelegate(SelectQuest);

		SpriteText quetNothingSprieteText = dailyQuestNothingText.GetComponent<SpriteText>();

		if (quetNothingSprieteText != null)
			quetNothingSprieteText.Text = AsTableManager.Instance.GetTbl_String(146);

		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextAccepted);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextDaily);

		noAcceptQuest.Text = AsTableManager.Instance.GetTbl_String(1471);
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(37913);
		m_TextAccepted.Text = AsTableManager.Instance.GetTbl_String(37914);
		m_TextDaily.Text = AsTableManager.Instance.GetTbl_String(37915);


		tabPanels[0].SetState(0);
		tabPanels[1].SetState(1);
		tabPanels[0].Value = true;

		foreach (SimpleSprite sprite in widthSprites)
			backgroundWidth += sprite.width;
	}

	void CreateQuestBookItem()
	{
		if (questItem == null)
		{
			questItem = (GameObject)GameObject.Instantiate(questItemPrefab);
			questItem.transform.parent = transform;
			questItem.gameObject.layer = LayerMask.NameToLayer("GUI");
			questItem.SetActive(false);
		}
	}

	void ButtonEvent(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (ptr.targetObj == buttonClose)
			{
				AsSoundManager.Instance.PlaySound("Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				AsHudDlgMgr.Instance.CloseQuestBook();
				AsHudDlgMgr.Instance.CloseQuestAccept();
			}
		}
	}

	public void SelectQuest(IUIObject obj)
	{
		AsSoundManager.Instance.PlaySound("Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if (obj == questList && showedAdvertise == false)
		{
			if (questList.SelectedItem != null)
				AsHudDlgMgr.Instance.OpenQuestAcceptUI(listQuest[questList.SelectedItem.Index].GetQuestData(), false);
		}

		showedAdvertise = false;
	}

	void ListItemInputEvent(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			if (nowAdCoolTime > 0.0f)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(AsTableManager.Instance.GetTbl_String(2133), true);
				showedAdvertise = true;
				return;
			}

			pressObject = ptr.targetObj.gameObject;

			if (CanAdvertising() == false)
			{
				pressObject = null;
				return;
			}

			startPress	= true;
			pressPos	= Input.mousePosition;
			pressTime	= 0.0f;
		}
		else if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (pressTime < adPressTime)
			{
				startPress	= false;
				pressTime	= 0.0f;
				pressObject = null;
			}
		}
	}

	void Update()
	{
		CheckAdInput();

		CheckAdCoolTime();
	}


	void CheckAdCoolTime()
	{
		if (nowAdCoolTime > 0.0f)
			nowAdCoolTime -= Time.deltaTime;
	}

	void CheckAdInput()
	{
		if (startPress != true)
			return;

		pressTime += Time.deltaTime;

		if (pressTime >= adPressTime)
		{
			pressPos = Vector3.zero;
			pressTime = 0.0f;
			startPress = false;
			showedAdvertise = true;

			ShowAdMsg();
		}
		else if (pressPos != Input.mousePosition)
		{
			startPress = false;
			pressPos = Vector3.zero;
			pressTime = 0.0f;
			pressObject = null;
		}
	}

	bool CanAdvertising()
	{
		if (!dicQuestListItem.ContainsKey(pressObject))
			return false;

		int idx = dicQuestListItem[pressObject];

		ArkQuest quest = listQuest[idx];

		return quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN;
	}

	void ShowAdMsg()
	{
		if (pressObject == null)
			return;

		if (!dicQuestListItem.ContainsKey(pressObject))
			return;

		int idx = dicQuestListItem[pressObject];

		ArkQuest quest = listQuest[idx];

		ArkSphereQuestTool.QuestData questData = quest.GetQuestData();

		StringBuilder sb = new StringBuilder(questData.Info.GetQuestTypeString());
		sb.Append(questData.Info.Name);
		sb.Append(" ");
		sb.Append(AsTableManager.Instance.GetTbl_String(2132));

		body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE(sb.ToString(), eCHATTYPE.eCHATTYPE_PUBLIC, false);
		byte[] data = chat.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send(data);

		// set cool Time
		nowAdCoolTime = adCoolTime;
	}

	void OnEnable()
	{
		if (questItem != null)
			questItem.SetActive(false);
	}

	public void TabEventAccepted()
	{
		AsSoundManager.Instance.PlaySound("Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		ShowAcceptedQeustList();
		dailyQuestNothingText.SetActive(false);
	}

	public void TabEventDaily()
	{
		AsSoundManager.Instance.PlaySound("Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		if (nowMode != QuestBookMode.DAILY)
			ShowDailyQuestList();
		noAcceptQuest.gameObject.SetActive(false);
	}

	void ClearList()
	{
		if (questList != null)
			questList.ClearListSync(true);

		listQuest.Clear();
	}

	public void Close()
	{
		nowMode = QuestBookMode.ACCEPTED;

		ClearList();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_QUESTBOOK));
	}

	UIListItem AddQuest(ArkQuest quest)
	{
		ArkSphereQuestTool.QuestData data = quest.GetQuestData();
		UIListItem item = questList.CreateItem(questItem, questList.Count, true) as UIListItem;// quest.GetQuestData().Info.Name);

		item.SetInputDelegate(ListItemInputEvent);

		item.spriteText.Text = data.Info.Name;

		QuestBookItemController questBookItemController = item.GetComponent<QuestBookItemController>();

		questBookItemController.questType = data.Info.QuestType;
		questBookItemController.questProgressState = data.NowQuestProgressState;

		questBookItemController.UpdateQuestStateIcon();

		QuestListIconController questIconCon = item.gameObject.GetComponent<QuestListIconController>();

		if (questIconCon != null)
			questIconCon.SetQuestIconType(data.Info.QuestType);

		item.SetOffset(new Vector3(0.0f, 0.0f, -0.2f));
		item.gameObject.SetActive(false);

		return item;
	}

	public void Show()
	{
		gameObject.SetActive(true);
		dailyQuestNothingText.SetActive(false);
		ShowAcceptedQeustList();
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_QUESTBOOK));
        ArkQuestmanager.instance.CloseProgressInQuestMsgBox();
	}

	void ShowAcceptedQeustList()
	{
		CreateQuestBookItem();
		nowMode = QuestBookMode.ACCEPTED;
		UpdateAcceptedList();
	}

	public void ShowDailyQuestList()
	{
		ClearList();

		nowMode = QuestBookMode.DAILY;

		AsCommonSender.SendRequestDailyQuests();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.TAP_QUESTBOOK_DAILYQUEST));

		if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_DAILY_QUEST) != null)
			AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_DAILY_QUEST);
	}

	public void UpdateAcceptedList()
	{
		if (nowMode == QuestBookMode.DAILY)
			return;

		ClearList();

		List<ArkQuest> listSotredAcceptQuest = ArkQuestmanager.instance.GetSortedQuestList();

		noAcceptQuest.gameObject.SetActive(0 == listSotredAcceptQuest.Count);

		dicQuestListItem.Clear();

		foreach (ArkQuest quest in listSotredAcceptQuest)
		{
			UIListItem item = AddQuest(quest);
			listQuest.Add(quest);

			if (!dicQuestListItem.ContainsKey(item.gameObject))
				dicQuestListItem.Add(item.gameObject, listQuest.Count - 1);
		}

		questList.RepositionItems();
	}

	public void UpdateDailyList()
	{
		if (nowMode == QuestBookMode.ACCEPTED)
			return;

		ClearList();

		ArkQuest[] dailyQuests = ArkQuestmanager.instance.GetDailyQuest();

		if (dailyQuests.Length == 0)
			dailyQuestNothingText.SetActive(true);
		else
			dailyQuestNothingText.SetActive(false);



		foreach (ArkQuest quest in dailyQuests)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
			{
				AddQuest(quest);
				listQuest.Add(quest);
			}
		}

		questList.RepositionItems();


		if (dailyQuests.Length == 0 || listQuest.Count == 0)
			dailyQuestNothingText.SetActive(true);
		else
			dailyQuestNothingText.SetActive(false);
	}

	public override void ProcessUIMessage(UIMessageObject message)
	{
		if (gameObject.activeSelf == false)
			return;
	}
}
