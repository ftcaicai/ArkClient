using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;
using System.Text;

public class ArkQuestmanager : MonoBehaviour
{
    public class QuestTargetMarkInfo
    {
        public int targetID = -1;
        public bool champion = false;

        public QuestTargetMarkInfo(int _id, bool _champion)
        {
            targetID = _id;
            champion = _champion;
        }
    }

	private Dictionary<int, GameObject>	dicQuestObjects = new Dictionary<int, GameObject>();
	private Dictionary<int, ArkQuest>	dicQuests = new Dictionary<int, ArkQuest>();
	private Dictionary<int, ArkQuest>	dicQuestContainQuestItem = new Dictionary<int, ArkQuest>();
	private Dictionary<int, ArkQuest>	dicQuestContainGoldAchieve = new Dictionary<int, ArkQuest>();
	private Dictionary<int, List<int>>	dicNpcDailyQuests = new Dictionary<int, List<int>>();
	private Dictionary<int, List<int>>	dicUseItemToTarget = new Dictionary<int, List<int>>();
	private Dictionary<int, List<int>>	dicKillMonKindSkillItem = new Dictionary<int, List<int>>();
	private Dictionary<int, List<int>>	dicGetItemFromMonKindSkillItem = new Dictionary<int, List<int>>();
	private Dictionary<int, List<int>>	dicQuestCollectSkillItem = new Dictionary<int, List<int>>();
	private Dictionary<int, List<AchUseItemToTarget>> dicUseItemToTargetAch = new Dictionary<int, List<AchUseItemToTarget>>();
	private Dictionary<int, List<AchKillMonsterKind>> dicKillMonsterKindAch = new Dictionary<int, List<AchKillMonsterKind>>();
	private Dictionary<int, List<AchGetQuestCollection>> dicGetQuestCollection = new Dictionary<int, List<AchGetQuestCollection>>();
	private Dictionary<int, List<AchGetItemFromMonsterKind>> dicGetItemFromMonKind = new Dictionary<int, List<AchGetItemFromMonsterKind>>();
	private List<int>	listUseNpcDailyGroup = new List<int>();
	private List<int>	listMetaQuest = new List<int>();
    private List<QuestTargetMarkInfo> listMonInfoConnectionQuest = new List<QuestTargetMarkInfo>();
	private ArkQuest	acceptedWantedQuest = null;
	private List<int>	listDailyQuestIndex = new List<int>();
	private List<int>	listCompleteQuestIdx = new List<int>();
	private List<ArkQuest>	listDailyQuest = new List<ArkQuest>();
	private List<ArkQuest>	listSortedQuest = new List<ArkQuest>();
	private QuestData		recentlyClearQuestData = null;
	private AsMessageBox	partyMatchingMsgBox = null;
	private	static ArkQuestmanager	s_Instance = null;
	public	static QuestResultUI	resultUI = null;
	public	static QuestGuideArrow	guideArrow = null;
	private	static QuestTimeDlg		questTime = null;
	public	QuestData				warpQuestData	= null;
	public	bool					visibleTimeUI = false;

    private AsDlgBalloonMsgBoxQuest questMsgBox = null;

	public static ArkQuestmanager instance
	{
		get
		{
			if (s_Instance == null)
				s_Instance = FindObjectOfType(typeof(ArkQuestmanager)) as ArkQuestmanager;

			if (s_Instance == null)
			{
				GameObject obj = new GameObject("QuestManager");
				s_Instance = obj.AddComponent(typeof(ArkQuestmanager)) as ArkQuestmanager;
				QuestMessageBroadCaster.Init(obj);

				Debug.Log("Could not locate an QuestManager object.\n QuestManager was Generated Automaticly.");
			}
			return s_Instance;
		}
	}

	void Awake()
	{
		GameObject objResultUI = new GameObject("QuestResultUI");
		resultUI = objResultUI.AddComponent<QuestResultUI>();
		objResultUI.transform.parent = transform;
		resultUI.Init();

		GameObject guideArrowObj = GameObject.Instantiate(Resources.Load("UI/AsGUI/GUI_QuestGuideArrowMgr")) as GameObject;
		guideArrow = guideArrowObj.GetComponent<QuestGuideArrow>();
		guideArrowObj.transform.parent = transform;
		guideArrow.Init();

		Vector3 vCamPos = UIManager.instance.rayCamera.transform.position;

		guideArrowObj.transform.localPosition = new Vector3(vCamPos.x, -3.5f, 0.0f);

		DontDestroyOnLoad(gameObject);
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
	}

	public void ResetQuestManager()
	{
		foreach (ArkQuest quest in dicQuests.Values)
			GameObject.DestroyImmediate(quest.gameObject);

		dicQuestObjects.Clear();
		dicQuests.Clear();
		dicQuestContainQuestItem.Clear();
		dicQuestContainGoldAchieve.Clear();
		dicUseItemToTarget.Clear();
		dicKillMonKindSkillItem.Clear();
		dicGetItemFromMonKindSkillItem.Clear();
		dicQuestCollectSkillItem.Clear();
		dicGetItemFromMonKind.Clear();
		listMetaQuest.Clear();
		dicUseItemToTargetAch.Clear();
		dicKillMonsterKindAch.Clear();
		listDailyQuestIndex.Clear();
		listCompleteQuestIdx.Clear();
		listDailyQuest.Clear();
		dicGetQuestCollection.Clear();
		listUseNpcDailyGroup.Clear();
		ArkQuestmanager.instance.ResetNpcDailyQuestList();
        listMonInfoConnectionQuest.Clear();

		if (questTime != null)
		{
			GameObject.DestroyImmediate(questTime.gameObject);
			questTime = null;
			Debug.LogWarning("quest mgr reset & quest time destory");
		}

		AsUseItemToMonTriggerManager.instance.RemaveUseItemToMonTriggerAll();

		AsTableManager.Instance.ResetTbl_Quests();

		QuestTutorialMgr.Instance.Reset();
	}

	void OnApplicationQuit()
	{
		s_Instance = null;
	}

	/// <summary>
	/// �Ϸ� ����Ʈ ���Ͽ� �߰��Ѵ�.
	/// </summary>
	/// <param name="_idx"></param>
	public void SetCompleteQuest(int _questID, QuestData _questData)
	{
		if (!listCompleteQuestIdx.Contains(_questID))
		{
			listCompleteQuestIdx.Add(_questID);

			if (_questData != null)
			{
				// add npc daily group
				if (_questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
					if (!listUseNpcDailyGroup.Contains(_questData.Info.NpcDailyGroupID))
						listUseNpcDailyGroup.Add(_questData.Info.NpcDailyGroupID);
			}
		}
	}

	public void SetCompleteMetaQuest(int _questID)
	{
		if (listMetaQuest.Contains(_questID))
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_META_QUEST, new AchMetaQuest(_questID, true));
	}

	/// <summary>
	/// �Ϸ� ����Ʈ ���Ͽ��� �����Ѵ�.
	/// </summary>
	/// <param name="_idx"></param>
	public void RemoveCompleteQuest(int _idx)
	{
		if (!listCompleteQuestIdx.Contains(_idx))
			listCompleteQuestIdx.Remove(_idx);
	}

	public bool IsCompleteQuest(int _questID)
	{
		return listCompleteQuestIdx.Contains(_questID) && AsTableManager.Instance.GetTbl_QuestData(_questID).CanRepeat() == false;
	}

	public bool IsNothingQuest(int _questID)
	{
		QuestData questData = AsTableManager.Instance.GetTbl_QuestData(_questID);

		if (questData == null)
			return true;

		return questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING;
	}

	public List<int> GetCompleteQuestIDList()
	{
		return listCompleteQuestIdx;
	}

	public List<int> GetProgressQuestIDList()
	{
		List<int> progressList = new List<int>();

		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
				progressList.Add(quest.GetQuestData().Info.ID);
		}

		return progressList;
	}

	public bool IsClearQuest(int _questID)
	{
		if (!dicQuests.ContainsKey(_questID))
			return false;

		if (dicQuests[_questID].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR ||
			dicQuests[_questID].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
			return true;
		else
			return false;
	}

	public bool IsFailQuest(int _questID)
	{
		if (!dicQuests.ContainsKey(_questID))
			return false;

		if (dicQuests[_questID].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
			return true;
		else
			return false;

	}

	public bool IsAcceptedQuest(int _questID)
	{
		return dicQuests.ContainsKey(_questID);
	}

	public bool IsProgressQuest(int _questID)
	{
		if (dicQuests.ContainsKey(_questID))
			if (dicQuests[_questID].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
				return true;

		return false;
	}

    public bool HaveProgressQuest()
    {
        foreach (ArkQuest quest in dicQuests.Values)
        {
            if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                return true;
        }

        return false;
    }

	public bool IsAcceptedQuestRange(int _questMin, int _questMax)
	{
		for (int i = _questMin; i <= _questMax; i++)
		{
			if (dicQuests.ContainsKey(i))
				if (dicQuests[i].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
					return true;
		}

		return false;
	}

	public bool IsClearQuestRange(int _questMin, int _questMax)
	{
		for (int i = _questMin; i <= _questMax; i++)
		{
			if (IsClearQuest(i))
				return true;
		}

		return false;
	}

	bool DeleteQuest(int _questId)
	{
		// delete quest item
		if (dicQuestContainQuestItem.ContainsKey(_questId))
			dicQuestContainQuestItem.Remove(_questId);

		if (dicQuestContainGoldAchieve.ContainsKey(_questId))
			dicQuestContainGoldAchieve.Remove(_questId);

		if (dicUseItemToTarget.ContainsKey(_questId))
		{
			dicUseItemToTarget[_questId].Clear();
			dicUseItemToTarget.Remove(_questId);
		}

		if (dicKillMonKindSkillItem.ContainsKey(_questId))
		{
			dicKillMonKindSkillItem[_questId].Clear();
			dicKillMonKindSkillItem.Remove(_questId);
		}

		if (dicGetItemFromMonKindSkillItem.ContainsKey(_questId))
		{
			dicGetItemFromMonKindSkillItem[_questId].Clear();
			dicGetItemFromMonKindSkillItem.Remove(_questId);
		}

		if (dicQuestCollectSkillItem.ContainsKey(_questId))
		{
			dicQuestCollectSkillItem[_questId].Clear();
			dicQuestCollectSkillItem.Remove(_questId);
		}

		if (listMetaQuest.Contains(_questId))
			listMetaQuest.Remove(_questId);

		if (dicUseItemToTargetAch.ContainsKey(_questId))
			dicUseItemToTargetAch.Remove(_questId);

		if (dicKillMonsterKindAch.ContainsKey(_questId))
			dicKillMonsterKindAch.Remove(_questId);

		if (dicGetItemFromMonKind.ContainsKey(_questId))
			dicGetItemFromMonKind.Remove(_questId);

		// delete quest
		if (!dicQuestObjects.ContainsKey(_questId))
			return false;

		GameObject delObject = dicQuestObjects[_questId];
		CheckTimeAchievement(delObject.GetComponent<ArkQuest>(), true);
		dicQuestObjects.Remove(_questId);
		dicQuests.Remove(_questId);
		GameObject.DestroyImmediate(delObject);
		delObject = null;
		return true;
	}

	void CheckHaveQuestItem(ArkQuest _quest)
	{
		int itemCount = 0;
		int questID = _quest.GetQuestData().Info.ID;
		QuestAchievement ach = _quest.GetQuestData().Achievement;
		itemCount += ach.GetDataCount<AchGetItem>();
		itemCount += ach.GetDataCount<AchGetItemFromMonster>();
		itemCount += ach.GetDataCount<AchGetItemFromMonsterKind>();
		itemCount += ach.GetDataCount<AchGetQuestCollection>();

		if (itemCount > 0)
		{
			if (!dicQuestContainQuestItem.ContainsKey(questID))
				dicQuestContainQuestItem.Add(questID, _quest);
		}
	}

	void CheckHaveGetQuestCollection(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		List<AchGetQuestCollection> listQuestCollection = questData.Achievement.GetDatas<AchGetQuestCollection>();

		if (listQuestCollection.Count > 0)
		{
			List<AchGetQuestCollection> listAch = new List<AchGetQuestCollection>();

			foreach (AchGetQuestCollection getQuestCollection in listQuestCollection)
				listAch.Add(getQuestCollection);

			if (dicGetQuestCollection.ContainsKey(questData.Info.ID))
				dicGetQuestCollection[questData.Info.ID] = listAch;
			else
				dicGetQuestCollection.Add(questData.Info.ID, listAch);
		}
	}

	void CheckGetLevel(ArkQuest _quest)
	{
		List<AchGetLevel> listGetLevel = _quest.GetQuestData().Achievement.GetDatas<AchGetLevel>();

		foreach (AchGetLevel getLevel in listGetLevel)
		{
			int nowLevel = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
			if (nowLevel >= getLevel.Level)
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_LEVEL_UP, new AchGetLevel(nowLevel));
		}
	}

	void CheckTimeAchievement(ArkQuest _quest, bool _isDrop)
	{
		if (_quest == null)
		{
			Debug.LogError("(checkTimeAchievement) ArkQeust is null");
			return;
		}

		if (_quest.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
			return;

		// check time achievement
		List<AchTimeLimit> listTimeLimit = _quest.GetQuestData().Achievement.GetDatas<AchTimeLimit>();
		List<AchTimeSurvival> listTimeSruvival = _quest.GetQuestData().Achievement.GetDatas<AchTimeSurvival>();


		if (_isDrop == true && (listTimeLimit.Count > 0 || listTimeSruvival.Count > 0))
		{
			if (questTime != null)
				GameObject.DestroyImmediate(questTime.gameObject);

			return;
		}

		if (listTimeLimit.Count > 0 || listTimeSruvival.Count > 0)
		{
			if (_quest.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_FAIL)
			{
				if (questTime == null)
				{
					GameObject questTimeObj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/QuestTimeDlg")) as GameObject;
					questTime = questTimeObj.GetComponent<QuestTimeDlg>();
				}
				else
					questTime.gameObject.SetActiveRecursively(true);


				if (questTime != null)
					_quest.TimeSpriteText = questTime.questTimeText;

				if (AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
					questTime.questTimeText.gameObject.layer = LayerMask.NameToLayer("Default");

				visibleTimeUI = true;
			}
			else
			{
				if (questTime != null)
				{
					GameObject.DestroyImmediate(questTime.gameObject);
					questTime = null;
				}

				visibleTimeUI = false;
			}
		}
	}

	void CheckAlreayHaveItem(ArkQuest _quest)
	{
		List<int> listID = _quest.GetQuestItemIDList();

		foreach (int itemID in listID)
		{
			int itemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(itemID);

			if (itemCount > 0)
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM_COUNT_CHANGE, new AchGetItem(itemID, itemCount));
		}
	}

	void CheckAlreadyEquipItem(ArkQuest _quest)
	{
		QuestData data = _quest.GetQuestData();

		List<AchEquipItem> listAchEquipItem = data.Achievement.GetDatas<AchEquipItem>();

		foreach (AchEquipItem equip in listAchEquipItem)
		{
			if (equip.IsComplete == true)
				continue;

			if (ItemMgr.HadItemManagement.Inven.IsHaveEquipItem(equip.ItemID) || ItemMgr.HadItemManagement.Inven.IsHaveCosEquipItem(equip.ItemID))
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_EQUIP_ITEM, new AchEquipItem(equip.ItemID));
		}
	}

	void CheckFriendship(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		List<AchFriendship> listFriendship = questData.Achievement.GetDatas<AchFriendship>();

		foreach (AchFriendship friendship in listFriendship)
			AsCommonSender.SendQuestInfo(questData.Info.ID, friendship.AchievementNum);
	}

	void CheckUseItemToTarget(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		int questID = questData.Info.ID;

		if (!dicUseItemToTarget.ContainsKey(questID))
		{
			List<AchUseItemToTarget> useItemToTargetList = questData.Achievement.GetDatas<AchUseItemToTarget>();

			dicUseItemToTarget.Add(questID, new List<int>());
			dicUseItemToTargetAch.Add(questID, new List<AchUseItemToTarget>());

			foreach (AchUseItemToTarget useItem in useItemToTargetList)
			{
				if (!dicUseItemToTarget[questID].Contains(useItem.ItemID))
					dicUseItemToTarget[questID].Add(useItem.ItemID);

				dicUseItemToTargetAch[questID].Add(useItem);
			}
		}
	}

	void CheckKillMonKindSkillItem(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		int questID = questData.Info.ID;

		if (!dicKillMonKindSkillItem.ContainsKey(questID))
		{
			List<AchKillMonsterKind> killMonList = questData.Achievement.GetDatas<AchKillMonsterKind>();

			dicKillMonKindSkillItem.Add(questID, new List<int>());
			dicKillMonsterKindAch.Add(questID, new List<AchKillMonsterKind>());

			foreach (AchKillMonsterKind killMonKind in killMonList)
			{
				if (killMonKind.SkillItemID == 0)
					continue;

				if (!dicKillMonKindSkillItem[questID].Contains(killMonKind.SkillItemID))
					dicKillMonKindSkillItem[questID].Add(killMonKind.SkillItemID);

				dicKillMonsterKindAch[questID].Add(killMonKind);
			}
		}
	}

	void CheckGetItemFromMonKindSkillItem(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		int questID = questData.Info.ID;

		if (!dicGetItemFromMonKindSkillItem.ContainsKey(questID))
		{
			List<AchGetItemFromMonsterKind> getItemFromMonList = questData.Achievement.GetDatas<AchGetItemFromMonsterKind>();

			dicGetItemFromMonKindSkillItem.Add(questID, new List<int>());
			dicGetItemFromMonKind.Add(questID, new List<AchGetItemFromMonsterKind>());

			foreach (AchGetItemFromMonsterKind getItemFromMon in getItemFromMonList)
			{
				if (getItemFromMon.SkillItemID == 0)
					continue;

				if (!dicGetItemFromMonKindSkillItem[questID].Contains(getItemFromMon.SkillItemID))
					dicGetItemFromMonKindSkillItem[questID].Add(getItemFromMon.SkillItemID);

				dicGetItemFromMonKind[questID].Add(getItemFromMon);
			}
		}
	}

	void CheckQuestCollectionSkillItem(ArkQuest _quest)
	{
		QuestData questData = _quest.GetQuestData();

		int questID = questData.Info.ID;

		if (!dicQuestCollectSkillItem.ContainsKey(questID))
		{
			List<AchGetQuestCollection> getQuestCollection = questData.Achievement.GetDatas<AchGetQuestCollection>();

			dicQuestCollectSkillItem.Add(questID, new List<int>());

			foreach (AchGetQuestCollection questCollection in getQuestCollection)
			{
				if (questCollection.SkillItemID == 0)
					continue;

				if (!dicQuestCollectSkillItem[questID].Contains(questCollection.SkillItemID))
					dicQuestCollectSkillItem[questID].Add(questCollection.SkillItemID);
			}
		}
	}

	public AchUseItemToTarget FindUseItemToTargetAchievement(int _npcID)
	{
		AchUseItemToTarget find = null;

		foreach (KeyValuePair<int, List<AchUseItemToTarget>> pair in dicUseItemToTargetAch)
		{
			foreach (AchUseItemToTarget useItemToTarget in pair.Value)
			{
				if (useItemToTarget.targetID != _npcID)
					continue;

				if (useItemToTarget.IsComplete == true)
					continue;

				if (find != null)
				{
					if (useItemToTarget.AssignedQuestID <= find.AssignedQuestID)
						find = useItemToTarget;
				}
				else
				{
					find = useItemToTarget;
				}
			}
		}

		return find;
	}

	void CheckAlreadyHaveProduceMastery(ArkQuest _quest)
	{
		List<AchProductionMastery> listProduceMastery = _quest.GetQuestData().Achievement.GetDatas<AchProductionMastery>();

		if (listProduceMastery.Count > 0)
		{
			foreach (AchProductionMastery mastery in listProduceMastery)
			{
				if (mastery.ProductionType == eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX)
				{
					int lv = AsUserInfo.Instance.HaveProductTechLvMax();
					if (lv != 0)
						QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_PRODUCE_MASTERY, new AchProductionMastery(mastery.ProductionType, lv));
				}
				else
				{
					int lv = AsUserInfo.Instance.GetProductTechniqueLv(mastery.ProductionType);

					if (lv != 0)
						QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_PRODUCE_MASTERY, new AchProductionMastery(mastery.ProductionType, lv));
				}
			}
		}
	}

	void CheckAlreadyHaveSkill(ArkQuest _quest)
	{
		List<AchGetSkill> listGetSkill = _quest.GetQuestData().Achievement.GetDatas<AchGetSkill>();

		if (listGetSkill.Count > 0)
		{
			foreach (AchGetSkill getskill in listGetSkill)
			{
				Tbl_SkillBook_Record skillBookRecord = AsTableManager.Instance.GetTbl_SkillBook_Record(getskill.SkillID);

				if (skillBookRecord == null)
					return;

				bool haveSkillFirst = SkillBook.Instance.ContainSkillTableID(skillBookRecord.Skill1_Index);
				bool haveSkillSecond = SkillBook.Instance.ContainSkillTableID(skillBookRecord.Skill2_Index);

				if (haveSkillFirst || haveSkillSecond)
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_SKILL, getskill);
			}
		}
	}

	void CheckAlreadyHaveSocialPoint(ArkQuest _quest)
	{
		List<AchSocialPoint> listSocialPoint = _quest.GetQuestData().Achievement.GetDatas<AchSocialPoint>();

		if (listSocialPoint.Count > 0)
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_SOCIAL_POINT, new AchSocialPoint(AsSocialManager.Instance.SocialData.SocialPoint));
	}

	public void CheckVisibleTimeUI()
	{
		if (visibleTimeUI == true)
		{
			if (questTime != null)
				questTime.questTimeText.gameObject.layer = LayerMask.NameToLayer("GUI");
			else
				Debug.LogWarning("quest time is null");
		}
	}

	public void AddDailyQuestIdx(int[] _indecies)
	{
		// ����
		List<int> sortedIndex = new List<int>();
		foreach (int index in _indecies)
			sortedIndex.Add(index);

		sortedIndex.Sort();

		foreach (ArkQuest quest in listDailyQuest)
			GameObject.DestroyImmediate(quest.gameObject);

		listDailyQuest.Clear();
		listDailyQuestIndex.Clear();

		GameObject dailyRoot = ArkDailyQuests.instance.gameObject;

		foreach (int idx in sortedIndex)
		{
			ArkQuest quest = ArkQuest.CreateArkQuest(idx, dailyRoot, "DailyQuest_");

			QuestData questData = quest.GetQuestData();

			List<ConditionLevel> listConditionLevel = questData.Condition.GetCondition<ConditionLevel>();

			if (listConditionLevel.Count > 0)
			{
				if (listConditionLevel[0].CheckAcceptForDaily() == false)
				{
					GameObject.DestroyImmediate(quest.gameObject);
					continue;
				}
			}

			listDailyQuestIndex.Add(idx);

			if (quest != null)
				listDailyQuest.Add(quest);
		}

		if (AsHudDlgMgr.Instance.IsOpenQuestBook)
			AsHudDlgMgr.Instance.questBook.UpdateDailyList();
	}

	public ArkQuest[] GetDailyQuest()
	{
		return (ArkQuest[])listDailyQuest.ToArray();
	}

	/// <summary>
	/// ������ ����Ʈ ������ ��û�Ѵ�.
	/// </summary>
	/// <param name="_questID"></param>
	public void RequestAcceptQuest(int _questID)
	{
		AsCommonSender.SendAcceptQuest(_questID);
	}

	public void RequestGiveupQuest(int _questID)
	{
		AsCommonSender.SendDropQuest(_questID);
	}

	public void RequestClearQuest(int _questID)
	{
		AsCommonSender.SendClearQuest(_questID);
	}

	public void RequestImmediatelyClearQuest(int _questID)
	{
		AsCommonSender.SendImmediatelyClearQuest(_questID);
	}

	public void RequestCompleteQuest(int _questID)
	{
		AsCommonSender.SendCompleteQuest(_questID);
	}

	public void RequestCompleteQuest(int _questID, byte _selectItemIdx)
	{
		AsCommonSender.SendCompleteQuest(_questID, _selectItemIdx);
	}

	public ArkQuest GetQuest(int _questID)
	{
		if (dicQuests.ContainsKey(_questID))
			return dicQuests[_questID];
		else
			return null;
	}

	public QuestProgressState GetQuestState(int _questID)
	{
		if (dicQuests.ContainsKey(_questID))
			return dicQuests[_questID].NowQuestProgressState;
		else
			return QuestProgressState.QUEST_PROGRESS_NOTHING;
	}

	public List<ArkQuest> GetQuests()
	{
		List<ArkQuest> listQuest = new List<ArkQuest>();

		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.GetQuestData().Info.QuestType != QuestType.QUEST_WANTED)
				listQuest.Add(quest);
		}

		return listQuest;
	}

	public bool ContainTalkWithNPC(int _npcID)
	{
		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.ContainTalkWithNPC(_npcID))
			{
				if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
					return true;
			}
		}

		return false;
	}

	public List<int> GetQuestItemList()
	{
		List<int> listItem = new List<int>();
		foreach (ArkQuest quest in dicQuests.Values)
			listItem.AddRange(quest.GetQuestItemIDList());

		return listItem;
	}

	public List<int> GetQuestIDs()
	{
		List<int> listQuestID = new List<int>();

		foreach (ArkQuest quest in dicQuests.Values)
			listQuestID.Add(quest.GetQuestData().Info.ID);

		return listQuestID;
	}

	public List<int> GetAchEquipItemIDs()
	{
		List<int> idList = new List<int>();

		foreach (ArkQuest quest in dicQuests.Values)
		{
			QuestData questData = quest.GetQuestData();

			if (questData.Info.QuestType != QuestType.QUEST_WANTED && 
				questData.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_COMPLETE &&
				questData.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_FAIL)
			{
				List<AchEquipItem> innerList =  questData.Achievement.GetDatas<AchEquipItem>();

				foreach (AchEquipItem equipItem in innerList)
					idList.Add(equipItem.ItemID);
			}
		}

		return idList;
	}

	public void ImmediatelyClearQuest(int _questID)
	{
		ClearQuestCore(_questID, true);
	}

	public void ClearQuest(int _questID)
	{
		ClearQuestCore(_questID);
	}

	void ClearQuestCore(int _questID, bool bImmediately = false)
	{
		if (dicQuestObjects.ContainsKey(_questID))
		{
			// ����Ʈ Ŭ���� ó��
			ArkQuest quest = dicQuestObjects[_questID].GetComponent<ArkQuest>();

            QuestData questData = quest.GetQuestData();

            // remove Target Mon Info
            RemoveMonInfoForTargetMark(questData);

			if (bImmediately == true)
			{
				quest.SetQuestImmediatelyClear();
				quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR;
			}
			else
			{
				quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_CLEAR;
			}

            if (questData.Info.QuestType == QuestType.QUEST_WANTED)
                questData.Achievement.SetAllComplete();

			// add npc daily group
            if (questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
                if (!listUseNpcDailyGroup.Contains(questData.Info.NpcDailyGroupID))
                    listUseNpcDailyGroup.Add(questData.Info.NpcDailyGroupID);

			AsIntoMapTriggerManager.instance.DeleteTrigger(_questID);
			AsUseItemToTargetPanelManager.instance.DeletePanel(_questID);
			AsUseItemToMonTriggerManager.instance.RemoveUseItemToMonTrigger(_questID);
			UpdateSortQuestList();
			resultUI.ShowResultMessage(QuestResult.QUEST_CLEAR);
			UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE);

			if (AsHudDlgMgr.Instance != null)
				if (AsHudDlgMgr.Instance.IsOpenQuestBook == true)
					AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();

			if (QuestTutorialMgr.Instance.IsTutorialQuest(_questID))
				QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLEAR_QUEST, _questID));

			CheckQuestMarkAllNpcAndCollecion();

			AsNpcMenu npcMenu = AsHUDController.Instance.m_NpcMenu;

			if (AsHudDlgMgr.Instance != null)
			{
				AsHudDlgMgr.Instance.UpdateQuestBookBtn();

				if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
					AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();

				if (bImmediately == true)
					AsHudDlgMgr.Instance.CloseQuestAccept();
			}

            ShowQuestClearMsg(questData);

			// hide time dlg
			if (quest.HaveTimeQuest())
			{
				if (questTime != null)
					questTime.gameObject.SetActive(false);
			}

			if (npcMenu == null)
			{
				Debug.Log("menu is null");
				return;
			}

			if (quest.ContainTalkWithNPC() == true && npcMenu.m_bWaitOpenNpcMenu == true)
			{
				if (npcMenu.gameObject.activeSelf)
					npcMenu.LockInput = false;

				npcMenu.m_bWaitOpenNpcMenu = false;
				AsLoadingIndigator.Instance.HideIndigator();
				npcMenu.OpenAfterTalkClear();
			}
			else
			{
				AsLoadingIndigator.Instance.HideIndigator();
			}

			if (TerrainMgr.Instance.GetCurrentMap().MapData.getMapType == eMAP_TYPE.Tutorial)
			{
				if (AsHudDlgMgr.Instance.IsOpenedSkillshop == true)
					AsHudDlgMgr.Instance.CloseSkillshop();
			}
		}
	}

	void ShowQuestClearMsg(QuestData _questData)
	{
		string msg = string.Empty;

		int stringIdx = 0;
		int npcID = 0;
		Tbl_Npc_Record npcRecord = null;

		if (_questData.Info.QuestType == QuestType.QUEST_MAIN || _questData.Info.QuestType == QuestType.QUEST_FIELD ||
			_questData.Info.QuestType == QuestType.QUEST_BOSS || _questData.Info.QuestType == QuestType.QUEST_PVP || _questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
		{
			npcID = _questData.Info.QuestCompleteNpcID;
			npcRecord = AsTableManager.Instance.GetTbl_Npc_Record(npcID);
			stringIdx = 847;
		}
		else if (_questData.Info.QuestType == QuestType.QUEST_DAILY)
			stringIdx = 848;
		else if (_questData.Info.QuestType == QuestType.QUEST_WANTED)
			stringIdx = 860;


		if (_questData.Info.QuestType == QuestType.QUEST_MAIN || _questData.Info.QuestType == QuestType.QUEST_FIELD ||
			_questData.Info.QuestType == QuestType.QUEST_BOSS || _questData.Info.QuestType == QuestType.QUEST_PVP || _questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
			msg = string.Format(AsTableManager.Instance.GetTbl_String(stringIdx), _questData.Info.Name, npcRecord.NpcName);
		else
			msg = string.Format(AsTableManager.Instance.GetTbl_String(stringIdx), _questData.Info.Name);

		if (AsHudDlgMgr.Instance != null)
			if (AsHudDlgMgr.Instance.questCompleteMsgManager != null)
			{
				AsHudDlgMgr.Instance.questCompleteMsgManager.AddMessage(msg, _questData.Info.QuestType);

			}


		// quest clear chat msg
		string chatMsg = string.Format(AsTableManager.Instance.GetTbl_String(2102), _questData.Info.Name);

		if (TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Tutorial) == true)
		{
			StringBuilder sb = new StringBuilder("[");
			sb.Append(AsUtil.GetRealString(System.Text.Encoding.UTF8.GetString(AsUserInfo.Instance.GetCurrentUserCharacterInfo().szCharName)));
			sb.Append("]:");
			sb.Append(chatMsg);

			AsChatManager.Instance.InsertChat(sb.ToString(), eCHATTYPE.eCHATTYPE_PUBLIC, true);

			AsChatManager.Instance.ShowChatBalloon(AsEntityManager.Instance.UserEntity.UniqueId, chatMsg, eCHATTYPE.eCHATTYPE_PUBLIC);
		}
		else
		{
			body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE(chatMsg, eCHATTYPE.eCHATTYPE_PUBLIC, false);
			byte[] data = chat.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send(data);
		}
	}

	/// <summary>
	/// ����Ʈ�� �����Ѵ�
	/// </summary>
	/// <param name="_questID"></param>
	public void AcceptQuest(int _questID, bool bIsRunning)
	{
		#region -GameGuide_QuestAccept
		AsGameGuideManager.Instance.CheckUp(eGameGuideType.QuestAgree, _questID);
		#endregion

		ArkQuest quest = ArkQuest.CreateArkQuest(_questID, gameObject).GetComponent<ArkQuest>();

		QuestData questData = quest.GetQuestData();

		// wanted quest exception 
		if (questData.Info.QuestType == QuestType.QUEST_WANTED)
		{
			if (acceptedWantedQuest != null)
			{
				GameObject.DestroyImmediate(dicQuestObjects[_questID].gameObject);

				if (dicQuestObjects.ContainsKey(_questID))
				{
					dicQuestObjects.Remove(_questID);
					dicQuests.Remove(_questID);
				}
			}

			acceptedWantedQuest = quest;

			if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR)
				questData.Achievement.SetAllComplete();
		}

		if (bIsRunning == false)
			quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_IN;

        // add Monster Target Info
        if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
            AddMonInfoForTargetMark(questData);

		// check time achievement
		CheckTimeAchievement(quest, false);

		quest.StartTimeLimit();

		// check quest Item
		CheckHaveQuestItem(quest);

		// check get quest collection
		CheckHaveGetQuestCollection(quest);

		// add Quest
		if (!dicQuestObjects.ContainsKey(_questID))
		{
			Debug.Log("Add Quest " + _questID);
			dicQuestObjects.Add(_questID, quest.gameObject);
			dicQuests.Add(_questID, quest);
		}

		// add npc daily group
		if (questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
			if (!listUseNpcDailyGroup.Contains(questData.Info.NpcDailyGroupID))
				listUseNpcDailyGroup.Add(questData.Info.NpcDailyGroupID);

		if (bIsRunning == false && quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
		{
			if (questData.Achievement.CheckHaveAchType(typeof(AchMapInto)))
				AsIntoMapTriggerManager.instance.UpdateIntoMapTrigger(TerrainMgr.Instance.GetCurMapID());

			if (questData.Achievement.CheckHaveAchType(typeof(AchUseItemInMap)))
				AsUseItemInMapMarkManager.instance.UpdateUseItemInMapMark(TerrainMgr.Instance.GetCurMapID());

			if (questData.Achievement.CheckHaveAchType(typeof(AchGetRankPoint)))
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_RANK_POINT, new AchGetRankPoint(AsUserInfo.Instance.RankPoint));

			UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE);

			if (AsHudDlgMgr.Instance.IsOpenQuestBook)
			{
				if (questData.Info.QuestType == QuestType.QUEST_DAILY)
					AsHudDlgMgr.Instance.questBook.UpdateDailyList();
				else
					AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();
			}

			resultUI.ShowResultMessage(QuestResult.QUEST_ACCEPT);
			guideArrow.ShowGuideArrow(questData.Info.QuestGuideDirect);

			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.ACCEPT_QUEST, questData.Info.ID));
		}

		CheckAlreayHaveItem(quest);
		CheckUseItemToTarget(quest);
		CheckAlreadyEquipItem(quest);
		CheckKillMonKindSkillItem(quest);
		CheckGetItemFromMonKindSkillItem(quest);
		CheckQuestCollectionSkillItem(quest);

		// quest item
		AsUseItemToMonTriggerManager.instance.AddUseItemToMonTrigger(questData);

		// check add party
		if (bIsRunning == false)
		{
			List<AchAddParty> listParty = questData.Achievement.GetDatas<AchAddParty>();

			if (listParty.Count > 0)
			{
				if (AsPartyManager.Instance != null)
					if (AsPartyManager.Instance.IsPartying == true)
					{
						Dictionary<uint, AS_PARTY_USER> dicMember = AsPartyManager.Instance.GetPartyMemberList();

						if (dicMember != null)
						{
							int partyMemCount = dicMember.Count;
							QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_PARTY, new AchAddParty(partyMemCount));
						}
					}
			}
		}


		// check minimap
		if (bIsRunning == false)
		{
			if (CheckHaveOpenUIType(OpenUIType.MINIMAP) != null && AsHudDlgMgr.Instance != null)
				if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
					AsCommonSender.SendClearOpneUI(OpenUIType.MINIMAP);
		}

		if (bIsRunning == false)
			AsUseItemToTargetPanelManager.instance.UpdateUseItemToTargetPanel();

		// Check Gold Quest
		if (questData.Achievement.GetDatas<AchGold>().Count > 0)
		{
			dicQuestContainGoldAchieve.Add(questData.Info.ID, quest);
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_GOLD, new AchGold(GoldAchType.GOLD_GET, AsUserInfo.Instance.SavedCharStat.nGold));
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_BACK_GOLD, new AchGold(GoldAchType.GOLD_GET_BACK, AsUserInfo.Instance.SavedCharStat.nGold));
		}

		CheckAlreadyHaveSkill(quest);
		CheckAlreadyHaveProduceMastery(quest);

		if (bIsRunning == false)
		{
			CheckFriendship(quest);

			// check Designation
			List<AchDesignation> listDesignation = questData.Achievement.GetDatas<AchDesignation>();
			{
				foreach (AchDesignation ach in listDesignation)
				{
					if (ach.type == DesignationAchType.DESIGNATION_GET)
					{
						if (AsDesignationManager.Instance.IsObtainedDesignation(ach.data))
							QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_DESIGNATION, new AchDesignation(DesignationAchType.DESIGNATION_GET, ach.data));
					}
					else if (ach.type == DesignationAchType.DESIGNATION_CHANGE)
					{
						if (ach.data == 0)
							continue;

						DesignationData data = AsDesignationManager.Instance.GetCurrentDesignation();

						if (data == null)
							continue;

						if (ach.data == data.id)
							QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_DESIGNATION, new AchDesignation(DesignationAchType.DESIGNATION_ALREADY_EQUIP, data.id));
					}
				}
			}

			if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
				AsHudDlgMgr.Instance.worldMapDlg.UpdateZoneMapImg();

			List<AchMetaQuest> listMetaQuest = questData.Achievement.GetDatas<AchMetaQuest>();
			foreach (AchMetaQuest meta in listMetaQuest)
			{
				if (listCompleteQuestIdx.Contains(meta.questID))
				{
					QuestData questComplete = AsTableManager.Instance.GetTbl_QuestData(meta.questID);

					if (questComplete != null)
						if (questComplete.CanRepeat() == false)
							QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_META_QUEST, new AchMetaQuest(meta.questID));
				}
			}
		}

		if (bIsRunning == true)
		{
			List<AchMetaQuest> metas = questData.Achievement.GetDatas<AchMetaQuest>();
			foreach (AchMetaQuest meta in metas)
			{
				if (!listMetaQuest.Contains(meta.questID))
					listMetaQuest.Add(meta.questID);
			}
		}

		CheckQuestMarkAllNpcAndCollecion();

		if (bIsRunning == false && quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
		{
			CheckGetLevel(quest);

			CheckAlreadyHaveSocialPoint(quest);

			AskWarp(quest.GetQuestData());
		}


		if (bIsRunning == false && AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
			AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();

		// quest miniView
		if (bIsRunning == false)
			UpdateQuestMiniView();

		// questList 
		if (AsHUDController.Instance != null && bIsRunning == false)
		{
			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.UpdateQuestRelationUI(_questID);

			if (AsHUDController.Instance.m_NpcMenu != null)
			{
				QuestHolder questHolder = AsHUDController.Instance.m_NpcMenu.questHolder;

				if (questHolder != null)
				{
					QuestMarkType questMarkType = questHolder.GetQuestMarkType();

					if (questMarkType == QuestMarkType.NOTHING || questMarkType == QuestMarkType.PROGRESS ||
						questMarkType == QuestMarkType.CLEAR || questMarkType == QuestMarkType.TALK_CLEAR || questMarkType == QuestMarkType.CLEAR_NPC_DAILY ||
						questMarkType == QuestMarkType.CLEAR_REMAINTALK || questMarkType == QuestMarkType.TALK_HAVE ||
						questMarkType == QuestMarkType.HAVE_EVENT || questMarkType == QuestMarkType.HAVE_EVENT_AND_PROGRESS)
					{
						UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_TALK_CLOSE);
						AsHudDlgMgr.Instance.CloseQuestAccept();
						AskQuestPartyMatching(questData);
						return;
					}
					else if (questMarkType == QuestMarkType.HAVE || questMarkType == QuestMarkType.CLEAR_AND_HAVE || questMarkType == QuestMarkType.HAVE_NPC_DAILY ||
							 questMarkType == QuestMarkType.UPPERLEVEL || questMarkType == QuestMarkType.LOWERLEVEL) 
					{
						UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_SHOW);
						AskQuestPartyMatching(questData);
					}

					AsHudDlgMgr.Instance.CloseQuestAccept();
				}
				else  // if DailyQuest
					AsHudDlgMgr.Instance.CloseQuestAccept();
			}
		}
	}

	void UpdateQuestMiniView()
	{
		UpdateSortQuestList();

		if (AsHudDlgMgr.Instance != null)
		{
			if (AsHudDlgMgr.Instance.IsOpenQuestMiniView)
				AsHudDlgMgr.Instance.UpdateQuestMiniView();
			else
			{
				if (listSortedQuest.Count == 1 && AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.AddNewQuestProcess();
			}
		}
	}



	public void CompleteQuest(int _questID)
	{
		#region -GameGuide_QuestComplete
		AsGameGuideManager.Instance.CheckUp(eGameGuideType.QuestComplete, _questID);
		#endregion

		if (dicQuestObjects.ContainsKey(_questID))
		{
			ArkQuest quest = dicQuestObjects[_questID].GetComponent<ArkQuest>();
			QuestData questData = quest.GetQuestData();

			DeleteQuest(questData.Info.ID);
			resultUI.ShowResultMessage(QuestResult.QUEST_COMPLETE);

			if (questData.Info.QuestType == QuestType.QUEST_DAILY)
			{
				quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_COMPLETE;
			}
			else
			{
				questData.IncreaseQuestCompleteCount();

				if (questData.CanRepeat() == false)
				{
					quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_COMPLETE;
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_META_QUEST, new AchMetaQuest(questData.Info.ID));
				}
				else
				{
					quest.GetQuestData().Achievement.Reset();
					quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
				}
			}

			if (questData.Info.QuestType == QuestType.QUEST_DAILY)
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_COMPLETE_DAILY_QUEST, new AchCompleteDailyQuest(questData.Info.QuestMapID, 1));

			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_COMPLETE_QUEST, new AchCompleteQuest(questData.Info.QuestType, questData.Info.QuestMapID, 1));

			// �Ϸ� ����Ʈ ����
			SetCompleteQuest(_questID, questData);

			CheckQuestMarkAllNpcAndCollecion();

			AfterProcessingForNpcTalkUI();

			AsHudDlgMgr.Instance.UpdateQuestBookBtn();

			//UpdateSortQuestList();
			UpdateQuestMiniView();

			if (dicGetQuestCollection.ContainsKey(_questID))
				dicGetQuestCollection.Remove(_questID);

			if (questData.Info.QuestType == QuestType.QUEST_WANTED)
			{
				acceptedWantedQuest = null;

				if (AsHudDlgMgr.Instance.wantedQuestBtn != null)
					AsHudDlgMgr.Instance.wantedQuestBtn.gameObject.SetActiveRecursively(false);
			}

			if (AsHudDlgMgr.Instance.IsOpenQuestBook)
				AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();

			if (AsHudDlgMgr.Instance.IsOpenQuestBook)
				AsHudDlgMgr.Instance.questBook.UpdateDailyList();

			AsHudDlgMgr.Instance.CloseQuestAccept();

			if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
				AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();

			// show quest reward Msg
			ShowRewardItemMsgBox(_questID);
		}
	}

    public void ShowProgressInQuestMsgBox(float _visivleTime = 8.0f)
    {
        if (questMsgBox != null)
            questMsgBox.CloseMsgBox();

        if (AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
            return;

        if (AsHudDlgMgr.Instance.questBtn.gameObject.activeSelf == false)
            return;

        List<int> progressInQuestList = ArkQuestmanager.instance.GetProgressQuestIDList();

        if (progressInQuestList.Count <= 0)
            return;

        GameObject balloonPrefab = ResourceLoad.LoadGameObject("UI/Optimization/Prefab/GUI_Balloon_QuestState");

        GameObject objectBallonMsg = GameObject.Instantiate(balloonPrefab) as GameObject;

        questMsgBox = objectBallonMsg.GetComponent<AsDlgBalloonMsgBoxQuest>();

        questMsgBox.tailType = AsDlgBalloonMsgBox.TailType.CENTER;
        questMsgBox.visibleTime = _visivleTime;
        questMsgBox.Init();
        questMsgBox.UpdateText();

        questMsgBox.transform.parent = AsHudDlgMgr.Instance.questBtn.transform;
        questMsgBox.transform.localPosition = new Vector3(-AsHudDlgMgr.Instance.questBtn.width * 0.25f, -AsHudDlgMgr.Instance.questBtn.height, 6.0f);
    }

    public void CloseProgressInQuestMsgBox()
    {
        if (questMsgBox != null)
        {
            GameObject.Destroy(questMsgBox.gameObject);
            questMsgBox = null;
        }
    }

	void ShowRewardItemMsgBox(int _questID)
	{
		if (AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
			return;

		Tbl_QuestReward_Record reward = AsTableManager.Instance.GetTbl_QuestRewardRecord(_questID);

		eCLASS userClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);

		List<Item> listRewardItem = new List<Item>();

		if (reward != null)
		{
			if (reward.Reward.Items.Count <= 0)
				return;

			StringBuilder sb = new StringBuilder();
			StringBuilder sbForShape = new StringBuilder();

			int count = 0;
			foreach (RewardItem rewardItem in reward.Reward.Items)
			{
				if (rewardItem.Class == userClass || rewardItem.Class == eCLASS.NONE)
				{
					Item item = ItemMgr.ItemManagement.GetItem(rewardItem.ID);

					listRewardItem.Add(item);

					string itemName = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
					string szColor = string.Empty;

					switch (item.ItemData.grade)
					{
						case Item.eGRADE.Normal:
							szColor = "RGBA(1.0, 1.0, 1.0, 1.0)";
							break;

						case Item.eGRADE.Magic:
							szColor = "RGBA(0.752, 0.988, 0.223, 1.0)";
							break;

						case Item.eGRADE.Rare:
							szColor = "RGBA(0.462, 0.756, 1.0, 1.0)";
							break;

						case Item.eGRADE.Epic:
							szColor = "RGBA(1.0, 0.541, 0.66, 1.0)";
							break;

						case Item.eGRADE.Ark:
							szColor = "RGBA(1.0, 0.541, 0.66, 1.0)";
							break;

						default:
							szColor = Color.white.ToString();
							break;
					}

					if (count != 0)
					{
						sb.Append("RGBA(1.0, 1.0, 1.0, 1.0),");
						sbForShape.Append(",");
					}

					sb.Append(szColor);
					sb.Append(itemName);
					sbForShape.Append(itemName);

					count++;
				}
			}

			if (count <= 0)
				return;

			sb.Append("RGBA(1.0, 1.0, 1.0, 1.0)");

			ShowBalloonRewardMsgBox(string.Format(AsTableManager.Instance.GetTbl_String(4084), sb.ToString()), string.Format(AsTableManager.Instance.GetTbl_String(4084), sbForShape.ToString()), 5.0f);

			if (listRewardItem.Count > 0)
			{
				Item item = listRewardItem[0];
				if (null == item)
				{
					Debug.LogError("ShowRewardItemMsgBox::PlayDropSound() [ null == _item ]");
					return;
				}

				if (string.Compare("NONE", item.ItemData.getStrDropSound, true) == 0)
				{
					Debug.LogError("ShowRewardItemMsgBox::PlayDropSound()[getStrDropSound == NONE] id : " + item.ItemID);
					return;
				}

				AsSoundManager.Instance.PlaySound(item.ItemData.getStrDropSound, Vector3.zero, false);
			}
		}
	}

	public void DropQuest(int _questID)
	{
		// del npc daily group
		QuestData questData = dicQuests[_questID].GetQuestData();

		if (questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
			if (listUseNpcDailyGroup.Contains(questData.Info.NpcDailyGroupID))
				listUseNpcDailyGroup.Remove(questData.Info.NpcDailyGroupID);

        // remove Target Mon Info
        RemoveMonInfoForTargetMark(questData);

		DeleteQuest(_questID);
		AsTableManager.Instance.SetTbl_QuesrRecordState(_questID, QuestProgressState.QUEST_PROGRESS_NOTHING);
		AsTableManager.Instance.ResetTbl_QuesrRecord(_questID);
		AsIntoMapTriggerManager.instance.DeleteTrigger(_questID);
		AsUseItemInMapMarkManager.instance.DeleteMark(_questID);
		AsUseItemToTargetPanelManager.instance.DeletePanel(_questID);
		AsUseItemToMonTriggerManager.instance.RemoveUseItemToMonTrigger(_questID);
		UpdateSortQuestList();

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.UpdateQuestRelationUI(_questID);

		if (dicGetQuestCollection.ContainsKey(_questID))
			dicGetQuestCollection.Remove(_questID);

		CheckQuestMarkAllNpcAndCollecion();

		AfterProcessingForNpcTalkUI();

		if (QuestTutorialMgr.Instance.IsTutorialQuest(_questID))
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.DROP_QUEST, _questID));

		AsHudDlgMgr.Instance.UpdateQuestBookBtn();

		if (AsHudDlgMgr.Instance.IsOpenQuestBook)
			AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.UpdateQuestMiniView();

		AsHudDlgMgr.Instance.CloseQuestAccept();
		resultUI.ShowResultMessage(QuestResult.QUEST_GIVEUP);

		if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
			AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();
	}

	public int GetClearQuestCount()
	{
		int count = 0;

		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.GetQuestData().Info.QuestType != QuestType.QUEST_WANTED)
				if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
					count++;
		}

		return count;
	}

	void AfterProcessingForNpcTalkUI()
	{
		QuestHolder questHolder = AsHUDController.Instance.m_NpcMenu.questHolder;

		if (questHolder != null)
		{
			QuestMarkType markType = questHolder.GetQuestMarkType();

			if (markType == QuestMarkType.HAVE || markType == QuestMarkType.CLEAR_AND_HAVE)
			{
				if (AsHudDlgMgr.Instance.IsOpenQuestBook)
					AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();

				UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE);
				UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_SHOW);
			}
			else
			{
				UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_TALK_CLOSE);
			}
		}
		else
		{
			UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_TALK_RESET);
			UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_TALK_MENUBUTTON_UPDATE);
		}
	}

	public void FailQuest(int _questID)
	{
        QuestData questData = dicQuests[_questID].GetQuestData();

        // remove Target Mon Info
        RemoveMonInfoForTargetMark(questData);

		AsTableManager.Instance.SetTbl_QuesrRecordState(_questID, QuestProgressState.QUEST_PROGRESS_FAIL);
		AsIntoMapTriggerManager.instance.DeleteTrigger(_questID);
		AsUseItemToTargetPanelManager.instance.DeletePanel(_questID);
		AsUseItemToMonTriggerManager.instance.RemoveUseItemToMonTrigger(_questID);
		resultUI.ShowResultMessage(QuestResult.QUEST_FAILED);
		CheckQuestMarkAllNpcAndCollecion();
		UpdateSortQuestList();

 

		if (dicQuests.ContainsKey(_questID))
		{
			ArkQuest failedQuest = dicQuests[_questID];
			if (failedQuest.HaveTimeQuest())
			{
				if (questTime != null)
					questTime.gameObject.SetActiveRecursively(false);
			}
		}

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.UpdateQuestMiniView();

		if (AsHudDlgMgr.Instance.IsOpenQuestBook)
			AsHudDlgMgr.Instance.questBook.UpdateAcceptedList();

		if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
			AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();

		UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE);
	}

	/// <summary>
	/// ��ü������ ó�������� ���� ������  fail ��Ŷó���� �ٲ��� �Ѵ�.
	/// </summary>
	public void QuestFailByDeath()
	{
		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				List<AchTimeSurvival> listTimeSurvival = quest.GetQuestData().Achievement.GetDatas<AchTimeSurvival>();

				if (listTimeSurvival.Count >= 1)
				{
					quest.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_FAIL;
					resultUI.ShowResultMessage(QuestResult.QUEST_FAILED);
				}
			}
		}
	}

	public void ProcessMessage(QuestMessageObject questMessage)
	{
		foreach (ArkQuest quest in dicQuests.Values)
			quest.ProcessMessage(questMessage);
	}

	public bool IsCompleteQuestAchievementRange(int _questMin, int _questMax, int _achievementIdx)
	{
		for (int i = _questMin; i <= _questMax; i++)
		{
			if (dicQuests.ContainsKey(i))
			{
				QuestData questData = dicQuests[i].GetQuestData();

				AchBase ach = questData.Achievement.GetAchievement(_achievementIdx);

				if (ach != null)
					if (ach.IsComplete == true)
						return true;
			}
		}

		return false;
	}

	public bool IsCompleteAchievement(int _questID, int _achIdx)
	{
		if (dicQuests.ContainsKey(_questID))
		{
			QuestData data = dicQuests[_questID].GetQuestData();

			AchBase achievement = data.Achievement.GetAchievement(_achIdx);

			if (achievement == null)
				return false;

			return achievement.IsComplete;
		}
		else
			return false;
	}

	public void GernerateQuestItemMsg(int _itemID, int _itemCount)
	{
		List<int> haveQuestItemList = ArkQuestmanager.instance.GetQuestItemList();

		if (haveQuestItemList.Contains(_itemID))
		{
			AchGetItem getItem = new AchGetItem(_itemID, _itemCount);
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM, getItem);
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM_FROM_MONSTER, getItem);
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM_FROM_MONSTER_KIND, getItem);
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_QUEST_COLLECTION, getItem);
		}
	}

	public void ToLoginScene()
	{
		WemeSdkManager.Instance.WemeLogout();
	}
	
	public void AskWarp(QuestData _questData)
	{
		//2014.05.16 dopamin
		if (234018 == _questData.Info.ID || 234019 == _questData.Info.ID || 234020 == _questData.Info.ID || 234030 == _questData.Info.ID )
		{
			if(true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1922),
					this, "ToLoginScene", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}
		}

		if (_questData.Preparation == null)
			return;

		if (_questData.Preparation.MoveMap.MapID == -1)
			return;

		warpQuestData = _questData;
		AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(145), AsTableManager.Instance.GetTbl_String(141),
										AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(997),
										this, "AskWarpCore", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	public void AskQuestPartyMatching(QuestData _data)
	{
		if (_data.Info.PartyQuest == false || AsGameMain.GetOptionState(OptionBtnType.OptionBtnType_QuestPartyMatching) == false)
			return;

		if (AsPartyManager.Instance.IsPartying == true)
			return;

		recentlyClearQuestData = _data;

		string msg = string.Format(AsTableManager.Instance.GetTbl_String(2101), TerrainMgr.Instance.GetMap(_data.Info.QuestMapID).MapData.GetName());

		if (partyMatchingMsgBox != null)
			partyMatchingMsgBox.Close();

		partyMatchingMsgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), msg, this, "MatchingQuestParty", "CancelQuestParty", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	void MatchingQuestParty()
	{
		if (recentlyClearQuestData == null)
			return;

		if (recentlyClearQuestData.Info.PartyQuest == false || recentlyClearQuestData.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
			return;

		AsSoundManager.Instance.PlaySound("Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsPartySender.SendPartySearch(recentlyClearQuestData.Info.QuestMapID);

		recentlyClearQuestData = null;
	}

	void CancelQuestParty()
	{
		recentlyClearQuestData = null;
	}

	void AskWarpCore()
	{
		if (warpQuestData.Preparation.MoveMap != null)
			AsCommonSender.SendQuestWarp(warpQuestData.Info.ID, warpQuestData.Preparation.MoveMap.MapID, new Vector3(warpQuestData.Preparation.MoveMap.MapLocationX, 0.0f,
																													 warpQuestData.Preparation.MoveMap.MapLocationZ));

		UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_TALK_CLOSE);
	}

	#region cheat
	public void ResetQuest(int _questID)
	{
		QuestData questData = AsTableManager.Instance.GetTbl_QuestRecord(_questID).QuestDataInfo;

		if (questData != null)
		{
			if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_COMPLETE)
			{
				questData.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
				questData.Info.RepeatCurrent = 0;
				questData.Achievement.Reset();
			}
		}
	}

	public void ResetQuestAll()
	{
		Tbl_Quest_Record[] questRecords = AsTableManager.Instance.GetTbl_QuestRecords();

		foreach (Tbl_Quest_Record questRecord in questRecords)
			ResetQuest(questRecord.QuestDataInfo.Info.ID);
	}
	#endregion

	public void CheckQuestMarkAllNpcAndCollecion()
	{
		List<AsNpcEntity> listCollecion = AsEntityManager.Instance.GetEntityListByFsmType(eFsmType.COLLECTION);

		// npc quest update
		QuestHolderManager.instance.UpdateQuestHolder();

		foreach (AsNpcEntity collection in listCollecion)
		{
			if (collection.collectionMark != null)
				collection.collectionMark.UpdateQuestCollectionMark();
		}
	}

	public AchOpenUI CheckHaveOpenUIType(OpenUIType _openUIType)
	{
		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				List<AchOpenUI> listOpenUI = quest.GetQuestData().Achievement.GetDatas<AchOpenUI>();

				foreach (AchOpenUI openUI in listOpenUI)
					if (openUI.type == _openUIType)
						return openUI;
			}
		}

		return null;
	}


	/// <summary>
	/// find a quest contains talk achievement
	/// </summary>
	/// <param name="_npcTableIdx"></param>
	/// <returns></returns>
	public bool HaveTalkAchievement(int _npcTableIdx)
	{
		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				QuestData questData = quest.GetQuestData();

				List<AchTalkNPC> listTalkNpc = questData.Achievement.GetDatas<AchTalkNPC>();

				foreach (AchTalkNPC talkNpc in listTalkNpc)
				{
					if (talkNpc.NpcID == _npcTableIdx && talkNpc.IsComplete == false)
						return true;
				}
			}
		}

		return false;
	}

	public bool HaveCondition(ConditionBase _condition)
	{
		foreach (ArkQuest quest in dicQuests.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				QuestData questData = quest.GetQuestData();

				int conCount = questData.Condition.GetconditionCount(_condition);

				if (conCount > 0)
					return true;
			}
		}

		return false;
	}

	public bool HaveGoldAchieve()
	{
		foreach (ArkQuest quest in dicQuestContainGoldAchieve.Values)
		{
			if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN || quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
				return true;
		}

		return false;
	}

	public bool HaveUseItemToTarget(int _itemID)
	{
		foreach (int questID in dicUseItemToTarget.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (dicUseItemToTarget[questID].Contains(_itemID))
				return true;
		}

		return false;
	}

	public bool HaveKillMonKindSkillItem(int _itemID)
	{
		foreach (int questID in dicKillMonKindSkillItem.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (dicKillMonKindSkillItem[questID].Contains(_itemID))
				return true;
		}

		return false;
	}

	public bool HaveGetitemFromMonKindSkillItem(int _itemID)
	{
		foreach (int questID in dicGetItemFromMonKindSkillItem.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (dicGetItemFromMonKindSkillItem[questID].Contains(_itemID))
				return true;
		}

		return false;
	}

	public bool HaveQuestCollectionSkillItem(int _itemID)
	{
		foreach (int questID in dicQuestCollectSkillItem.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (dicQuestCollectSkillItem[questID].Contains(_itemID))
				return true;
		}

		return false;
	}

	public AchUseItemToTarget GetUseItemToTargetInfo(int _itemID, int _targetID)
	{
		foreach (int questID in dicUseItemToTarget.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (!dicUseItemToTarget[questID].Contains(_itemID))
				continue;

			if (!dicUseItemToTarget.ContainsKey(questID))
				continue;

			List<AchUseItemToTarget> listUseItemToTarget = dicUseItemToTargetAch[questID];

			foreach (AchUseItemToTarget useItemToTarget in listUseItemToTarget)
			{
				if (_targetID == -1)
				{
					if (useItemToTarget.ItemID == _itemID)
						return useItemToTarget;
				}
				else
				{
					if (useItemToTarget.ItemID == _itemID && useItemToTarget.targetID == _targetID)
						return useItemToTarget;
				}
			}
		}

		return null;
	}

	public AchKillMonsterKind GetKillMonKindSkill(int _itemID)
	{
		foreach (int questID in dicKillMonsterKindAch.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (!dicKillMonKindSkillItem.ContainsKey(questID))
				continue;

			if (!dicKillMonKindSkillItem[questID].Contains(_itemID))
				continue;

			List<AchKillMonsterKind> listKillMonKind = dicKillMonsterKindAch[questID];

			foreach (AchKillMonsterKind killMonKind in listKillMonKind)
			{
				if (killMonKind.SkillItemID == _itemID)
					return killMonKind;
			}
		}

		return null;
	}

	public AchGetItemFromMonsterKind GetItemFromMonKindSkill(int _itemID)
	{
		foreach (int questID in dicGetItemFromMonKind.Keys)
		{
			if (!dicQuests.ContainsKey(questID))
				continue;

			if (dicQuests[questID].NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
				continue;

			if (!dicGetItemFromMonKindSkillItem.ContainsKey(questID))
				continue;

			if (!dicGetItemFromMonKindSkillItem[questID].Contains(_itemID))
				continue;

			List<AchGetItemFromMonsterKind> listGetItemFromMon = dicGetItemFromMonKind[questID];

			foreach (AchGetItemFromMonsterKind getItemFromMon in listGetItemFromMon)
			{
				if (getItemFromMon.SkillItemID == _itemID)
					return getItemFromMon;
			}
		}

		return null;
	}

	public List<ArkQuest> GetQuestHaveGetQuestCollectionAch(int _npcID)
	{
		List<ArkQuest> listContainQuest = new List<ArkQuest>();

		foreach (int questID in dicGetQuestCollection.Keys)
		{
			foreach (AchGetQuestCollection questCollection in dicGetQuestCollection[questID])
			{
				if (questCollection.IsComplete == true)
					continue;

				if (_npcID == questCollection.CollectionID && dicQuests.ContainsKey(questID) == true)
					listContainQuest.Add(dicQuests[questID]);
			}
		}

		return listContainQuest;
	}

	public void UpdateQuestItem()
	{
		foreach (ArkQuest quest in dicQuestContainQuestItem.Values)
		{
			QuestData questData = quest.GetQuestData();
			List<AchGetItem> listGetItem = questData.Achievement.GetQeustGetItemAchievenemts();

			foreach (AchGetItem getItem in listGetItem)
			{
				int orgItemCount = getItem.CommonCount;
				int nowItemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(getItem.ItemID);

				if (nowItemCount != orgItemCount)
				{
					AchGetItem item = new AchGetItem(getItem.ItemID, nowItemCount);
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM_COUNT_CHANGE, item);
				}
			}
		}
	}

	public ArkQuest GetWantedQuest()
	{
		return acceptedWantedQuest;
	}

	public bool IsAcceptedWantedQuest()
	{
		return acceptedWantedQuest != null;
	}

	public void UpdateSortQuestList()
	{
		SortAcceptQuestList();
	}

	public List<ArkQuest> GetSortedQuestList()
	{
		return listSortedQuest;
	}

	public List<ArkQuest> GetSortedQuestListForQuestMini()
	{
		List<ArkQuest> listQuest = GetSortedQuestList();
		List<ArkQuest> listQuestFront = new List<ArkQuest>();

		foreach (ArkQuest quest in listQuest)
		{
			if (quest.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_CLEAR && quest.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
				listQuestFront.Add(quest);

			if (listQuestFront.Count >= 3)
				break;
		}

		return listQuestFront;
	}

	void SortAcceptQuestList()
	{
		int nowMapID = TerrainMgr.Instance.GetCurMapID();
		int questMapID = 0;

		List<ArkQuest> listQuestAccepted = GetQuests();

		SortedDictionary<int, ArkQuest> dicProgress = new SortedDictionary<int, ArkQuest>();
		SortedDictionary<int, ArkQuest> dicClear = new SortedDictionary<int, ArkQuest>();
		SortedDictionary<int, ArkQuest> dicFail = new SortedDictionary<int, ArkQuest>();
		SortedDictionary<int, ArkQuest> dicProgressThisRegion = new SortedDictionary<int, ArkQuest>();
		SortedDictionary<int, ArkQuest> dicClearThisRegion = new SortedDictionary<int, ArkQuest>();
		SortedDictionary<int, ArkQuest> dicFailThisRegion = new SortedDictionary<int, ArkQuest>();
		listSortedQuest.Clear();

		// Group
		foreach (ArkQuest quest in listQuestAccepted)
		{
			ArkSphereQuestTool.QuestData questData = quest.GetQuestData();
			QuestProgressState state = questData.NowQuestProgressState;

			questMapID = questData.Info.ID / 10000;

			if (state == QuestProgressState.QUEST_PROGRESS_FAIL)
			{
				if (questMapID == nowMapID)
					dicFailThisRegion.Add(questData.Info.ID, quest);
				else
					dicFail.Add(questData.Info.ID, quest);

			}
			else if (state == QuestProgressState.QUEST_PROGRESS_CLEAR || state == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
			{
				if (questMapID == nowMapID)
					dicClearThisRegion.Add(questData.Info.ID, quest);
				else
					dicClear.Add(questData.Info.ID, quest);
			}
			else if (state == QuestProgressState.QUEST_PROGRESS_IN)
			{
				if (questMapID == nowMapID)
					dicProgressThisRegion.Add(questData.Info.ID, quest);
				else
					dicProgress.Add(questData.Info.ID, quest);
			}
		}
		// fail
		listSortedQuest.AddRange(dicFailThisRegion.Values);
		listSortedQuest.AddRange(dicFail.Values);

		// clear
		listSortedQuest.AddRange(dicClearThisRegion.Values);
		listSortedQuest.AddRange(dicClear.Values);

		// progress
		listSortedQuest.AddRange(dicProgressThisRegion.Values);
		listSortedQuest.AddRange(dicProgress.Values);
	}

	public static QuestProgressState ConvertQuestProgressNetToClient(QuestProgressStateNet _questProgressStateNet)
	{
		if (_questProgressStateNet == QuestProgressStateNet.QUEST_START)
			return QuestProgressState.QUEST_PROGRESS_IN;
		else if (_questProgressStateNet == QuestProgressStateNet.QUEST_CLEAR)
			return QuestProgressState.QUEST_PROGRESS_CLEAR;
		else if (_questProgressStateNet == QuestProgressStateNet.QUEST_CLEAR_IMMEDIATELY)
			return QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR;
		else if (_questProgressStateNet == QuestProgressStateNet.QUEST_FAIL)
			return QuestProgressState.QUEST_PROGRESS_FAIL;
		else if (_questProgressStateNet == QuestProgressStateNet.QUEST_WANTED_NEW)
			return QuestProgressState.QUEST_PROGRESS_IN;
		else
			return QuestProgressState.QUEST_PROGRESS_MAX;
	}

	public int SetUseItemToTarget(int _slot, int _npcID)
	{
		// for quest ( use item to target) kb
		Item item = ItemMgr.HadItemManagement.Inven.invenSlots[_slot].realItem.item;
		int targetID = 0;

		if (item == null)
			return targetID;

		int ItemID = item.ItemID;

		bool bHaveUseItemToTarget = HaveUseItemToTarget(ItemID);

		AsPlayerFsm usrFsm = AsEntityManager.Instance.UserEntity.GetComponent<AsPlayerFsm>();

		if (item.ItemData.m_eItemType != Item.eITEM_TYPE.UseItem)
			return targetID;

		AsBaseEntity targetEntity = usrFsm.Target;

		if (targetEntity != null)
		{
			eFsmType fsmType = targetEntity.FsmType;

			if (bHaveUseItemToTarget == true)
			{
				if (fsmType != eFsmType.PLAYER && fsmType != eFsmType.OTHER_USER)
				{
					AsNpcEntity targetNpc = targetEntity as AsNpcEntity;
					targetID = targetNpc.sessionId_;
				}
				else
					Debug.Log("Target is player");
			}
		}
		else
		{
			if (bHaveUseItemToTarget == false)
				return targetID;

			AchUseItemToTarget useItemToTarget = ArkQuestmanager.instance.GetUseItemToTargetInfo(ItemID, _npcID);

			if (useItemToTarget == null)
				return targetID;


			List<AsNpcEntity> listEntity = AsEntityManager.Instance.GetEntityInRange(useItemToTarget.targetType, AsEntityManager.Instance.UserEntity.transform.position, 0.0f, 10.0f);

			foreach (AsNpcEntity npc in listEntity)
			{
				if (useItemToTarget.targetType == UseItemToTargetType.MONSTER)
				{
					int kindID = npc.GetProperty<int>(eComponentProperty.MONSTER_KIND_ID);

					if (useItemToTarget.Champion == true)
					{
						eMonster_Grade grade = npc.GetProperty<eMonster_Grade>(eComponentProperty.GRADE);

						if (useItemToTarget.targetID == kindID && grade == eMonster_Grade.Champion)
						{
							targetID = npc.sessionId_;
							usrFsm.SetTargetForQuestItem(npc);
							return targetID;
						}
					}
					else
					{
						if (useItemToTarget.targetID == kindID)
						{
							targetID = npc.sessionId_;
							usrFsm.SetTargetForQuestItem(npc);
							return targetID;
						}
					}
				}
				else
				{
					AchUseItemToTarget useItem = ArkQuestmanager.instance.GetUseItemToTargetInfo(ItemID, npc.TableIdx);

					if (useItem != null)
					{
						targetID = npc.sessionId_;
						usrFsm.SetTargetForQuestItem(npc);
						return targetID;
					}
				}
			}
		}

		return targetID;
	}

	public int SetTargetForQuestSkillItem(int _slot)
	{
		// for quest ( use item to target) kb
		Item item = ItemMgr.HadItemManagement.Inven.invenSlots[_slot].realItem.item;
		int targetID = 0;

		if (item == null)
			return 0;

		int ItemID  = item.ItemID;
		int subType = item.ItemData.GetSubType();

		bool bHaveUseItemToTarget = HaveKillMonKindSkillItem(ItemID);
		bool bHaveGetItemFromMon = HaveGetitemFromMonKindSkillItem(ItemID);
		bool bHaveQuestCollection = HaveQuestCollectionSkillItem(ItemID);

		if (item.ItemData.m_eItemType != Item.eITEM_TYPE.UseItem)
			return 0;

		if (bHaveUseItemToTarget == false && bHaveGetItemFromMon == false && bHaveQuestCollection == false)
		{
			if (subType != (int)Item.eUSE_ITEM.ConsumeQuest && subType != (int)Item.eUSE_ITEM.InfiniteQuest)
				return 0;
		}

		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record(item.ItemData.itemSkill);

		if (skillRecord == null)
			return 0;
		else
		{
			if (skillRecord.Skill_TargetType == eSkill_TargetType.Self)
				return 0;
		}

		AsPlayerFsm usrFsm = AsEntityManager.Instance.UserEntity.GetComponent<AsPlayerFsm>();

		AsBaseEntity targetEntity = usrFsm.Target;

		if (targetEntity != null)
		{
			eFsmType fsmType = targetEntity.FsmType;

			if (bHaveUseItemToTarget == true || bHaveGetItemFromMon == true || bHaveQuestCollection == true)
			{
				if (fsmType != eFsmType.PLAYER && fsmType != eFsmType.OTHER_USER)
				{
					AsNpcEntity targetNpc = targetEntity as AsNpcEntity;
					targetID = targetNpc.sessionId_;
				}
				else
					Debug.Log("Target is player");
			}
		}
		else
		{
			int targetKindID = 0;
			AchKillMonsterKind killMonKind = ArkQuestmanager.instance.GetKillMonKindSkill(ItemID);
			AchGetItemFromMonsterKind getItemFromMon = ArkQuestmanager.instance.GetItemFromMonKindSkill(ItemID);

			if (killMonKind == null && getItemFromMon == null)
				return targetID;

			if (killMonKind != null)
				targetKindID = killMonKind.MonsterKindID;
			else if (getItemFromMon != null)
				targetKindID = getItemFromMon.MonsterKindID;

			List<AsNpcEntity> listEntity = AsEntityManager.Instance.GetEntityInRange(UseItemToTargetType.MONSTER, AsEntityManager.Instance.UserEntity.transform.position, 0.0f, 10.0f);

			foreach (AsNpcEntity npc in listEntity)
			{
				int kindID = npc.GetProperty<int>(eComponentProperty.MONSTER_KIND_ID);

				if (targetKindID == kindID)
				{
					targetID = npc.sessionId_;
					usrFsm.SetTargetForQuestItem(npc);
					return targetID;
				}
			}
		}

		return targetID;
	}

	public void ShowBalloonRewardMsgBox(string _msg, string _withoutColorTagMsgForSize, float _exposureTime)
	{
		AsHudDlgMgr.Instance.DeleteRewardMsgBox();

		GameObject balloonPrefab = ResourceLoad.LoadGameObject("UI/Optimization/Prefab/GUI_Balloon_QuestDone");

		GameObject objectBallonMsg = GameObject.Instantiate(balloonPrefab) as GameObject;

		AsDlgBalloonMsgBox msgBox = objectBallonMsg.GetComponent<AsDlgBalloonMsgBox>();

		msgBox.transform.parent = AsHudDlgMgr.Instance.questCompleteMsgDummy.transform;
		msgBox.transform.localPosition = Vector3.zero;
		msgBox.visibleTime = _exposureTime;

		msgBox.SetText(_msg, _withoutColorTagMsgForSize);
	}

	public bool CanAcceptNpcDailyQuest(int _npcDailyQuestGroup)
	{
		return !listUseNpcDailyGroup.Contains(_npcDailyQuestGroup);
	}

	public void SetNpcDailyQuest(List<int> _listQuestID)
	{
		foreach (int questID in _listQuestID)
		{
			Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord(questID);
			QuestInfo info = questRecord.QuestDataInfo.Info;

			if (dicNpcDailyQuests.ContainsKey(info.QuestSuggestNpcID))
			{
				dicNpcDailyQuests[info.QuestSuggestNpcID].Add(questID);
			}
			else
			{
				dicNpcDailyQuests.Add(info.QuestSuggestNpcID, new List<int>());
				dicNpcDailyQuests[info.QuestSuggestNpcID].Add(questID);
			}
		}

		QuestHolderManager.instance.UpdateQuestHolder();
	}

	public List<int> GetNpcDailyQuestList(int _npcID)
	{
		if (dicNpcDailyQuests.ContainsKey(_npcID))
			return dicNpcDailyQuests[_npcID];
		else
			return new List<int>();
	}

	public void ResetNpcDailyQuestList()
	{
		Debug.LogWarning("Npc Daily Quest Reset");
		dicNpcDailyQuests.Clear();
	}

    public bool CheckShowTargetMark(int _monsterID, int _monsterKindID, bool _champion)
    {
        if (listMonInfoConnectionQuest.Count <= 0)
            return false;

        QuestTargetMarkInfo findMon     = FindTargetInList(_monsterID, _champion);
        QuestTargetMarkInfo findMonKind = FindTargetInList(_monsterKindID, _champion);


        if (findMon == null && findMonKind == null)
            return false;
        else
            return true;
    }

    public void AddMonInfoForTargetMark(QuestData _questData)
    {
        if (_questData == null)
        {
            Debug.LogWarning("Quest Data is null");
            return;
        }

        // Item Drop Mon
        foreach (PrepareItemDropMonster itemDropMon in _questData.Preparation.ItemDropMonsterList)
            listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(itemDropMon.TargetMonsterID, itemDropMon.Champion));

        // Item Drop Mon Kind
        foreach (PrepareItemDropMonsterKind itemDropMonKind in _questData.Preparation.ItemDromMonsterKindList)
            listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion));

        // Use Item To Target
        List<AchUseItemToTarget> listUseItemToTarget = _questData.Achievement.GetDatas<AchUseItemToTarget>();
        foreach (AchUseItemToTarget useItemToTarget in listUseItemToTarget)
        {
            if (useItemToTarget.targetType == UseItemToTargetType.MONSTER && useItemToTarget.IsComplete == false)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(useItemToTarget.targetID, useItemToTarget.Champion));
        }

        // Kill Monster
        List<AchKillMonster> listKillMon = _questData.Achievement.GetDatas<AchKillMonster>();
        foreach (AchKillMonster killMon in listKillMon)
        {
            if (killMon.IsComplete == false)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(killMon.MonsterID, killMon.Champion));
        }


        // Kill Monster Kind
        List<AchKillMonsterKind> listKillMonKind = _questData.Achievement.GetDatas<AchKillMonsterKind>();
        foreach (AchKillMonsterKind killMonKind in listKillMonKind)
        {
            if (killMonKind.IsComplete == false)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(killMonKind.MonsterKindID, killMonKind.Champion));
        }

        // Get Item From Mon
        List<AchGetItemFromMonster> listGetItemFromMon = _questData.Achievement.GetDatas<AchGetItemFromMonster>();
        foreach (AchGetItemFromMonster getItemFromMon in listGetItemFromMon)
        {
            if (getItemFromMon.IsComplete == false)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(getItemFromMon.MonsterID, getItemFromMon.Champion));
        }

        // Get Item From Mon
        List<AchGetItemFromMonsterKind> listGetItemFromMonKind = _questData.Achievement.GetDatas<AchGetItemFromMonsterKind>();
        foreach (AchGetItemFromMonsterKind getItemFromMonKind in listGetItemFromMonKind)
        {
            if (getItemFromMonKind.IsComplete == false)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(getItemFromMonKind.MonsterKindID, getItemFromMonKind.Champion));
        }

        AsHUDController.Instance.panelManager.UpdateMonsterNamePanel();
    }

    public void AddMonInfoForTargetMarkAch(AchGetItem _achGetItem)
    {
        System.Type achType = _achGetItem.GetType();

        int questID = _achGetItem.AssignedQuestID;

        ArkQuest quest = GetQuest(questID);

        if (quest == null)
        {
            Debug.LogError("Get Quest is null = " + questID);
            return;
        }

        QuestData questData = quest.GetQuestData();

        if (questData == null)
        {
            Debug.LogError("Get QuestData is null = " + questID);
            return;
        }

        Item item = null;
        Item.eITEM_TYPE itemType = Item.eITEM_TYPE.None;
        int itemSubType = -1;


        // Item Drop Mon
        foreach (PrepareItemDropMonster itemDropMon in questData.Preparation.ItemDropMonsterList)
        {
            if (_achGetItem.ItemID == itemDropMon.ItemID)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(itemDropMon.TargetMonsterID, itemDropMon.Champion));
        }

        // Item Drop Mon Kind
        foreach (PrepareItemDropMonsterKind itemDropMonKind in questData.Preparation.ItemDromMonsterKindList)
        {
            item = ItemMgr.ItemManagement.GetItem(itemDropMonKind.ItemID);

            itemType = item.ItemData.GetItemType();

            itemSubType = (int)item.ItemData.GetSubType();

            if (itemType == Item.eITEM_TYPE.UseItem && itemSubType == (int)Item.eUSE_ITEM.Random)
            {
                Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(item.ItemData.m_iItem_Rand_ID);

                if (lotteryRecord != null)
                {
                    foreach (int randomID in lotteryRecord.idlist)
                    {
                        Tbl_RandItem_Record recordRandom = AsTableManager.Instance.GetTbl_RandItem_Record(randomID);

                        if (recordRandom.idlist.Contains(_achGetItem.ItemID))
                        {
                            listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion));
                            break;
                        }
                    }
                }
            }

            if (_achGetItem.ItemID == itemDropMonKind.ItemID)
                listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion));
        }

        // get item from mon or get item from mon kind
        if (achType == typeof(AchGetItemFromMonster) || achType == typeof(AchGetItemFromMonsterKind))
            listMonInfoConnectionQuest.Add(new QuestTargetMarkInfo(_achGetItem.Target, _achGetItem.Champion));

        AsHUDController.Instance.panelManager.UpdateMonsterNamePanel();
    }

    public void RemoveMonInfoForTargetMarkAch(AchBase _clearAchieve)
    {
        System.Type achType = _clearAchieve.GetType();

        bool checkGetItem = false;

        if (achType == typeof(AchGetItem) || achType == typeof(AchGetQuestCollection))
        {
            checkGetItem = true;
        }
        else if (achType == typeof(AchUseItemToTarget) || achType == typeof(AchKillMonster) ||
                 achType == typeof(AchKillMonsterKind)|| achType == typeof(AchGetItemFromMonster) || achType == typeof(AchGetItemFromMonsterKind))
        {
            checkGetItem = false;
        }
        else
        {
            return;
        }

        int questID = _clearAchieve.AssignedQuestID;

        ArkQuest quest = GetQuest(questID);

        if (quest == null)
        {
            Debug.LogError("Get Quest is null = " + questID);
            return;
        }

        QuestData questData = quest.GetQuestData();

        if (questData == null)
        {
            Debug.LogError("Get QuestData is null = " + questID);
            return;
        }

        if (checkGetItem == true)
        {
            AchGetItem achGetItem = _clearAchieve as AchGetItem;

            Item item = null;
            Item.eITEM_TYPE itemType = Item.eITEM_TYPE.None;
            int itemSubType = -1;
            
            // Item Drop Mon
            foreach (PrepareItemDropMonster itemDropMon in questData.Preparation.ItemDropMonsterList)
            {
                if (itemDropMon.ItemID == achGetItem.ItemID)
                    RemoveTargetInfo(itemDropMon.TargetMonsterID, itemDropMon.Champion);
            }


            // Item Drop Mon Kind
            foreach (PrepareItemDropMonsterKind itemDropMonKind in questData.Preparation.ItemDromMonsterKindList)
            {
                item = ItemMgr.ItemManagement.GetItem(itemDropMonKind.ItemID);

                itemType = item.ItemData.GetItemType();

                itemSubType = (int)item.ItemData.GetSubType();

                if (itemType == Item.eITEM_TYPE.UseItem)
                {
                    if (itemSubType == (int)Item.eUSE_ITEM.Random)
                    {
                        Tbl_Lottery_Record lotteryRecord = AsTableManager.Instance.GetTbl_Lottery_Record(item.ItemData.m_iItem_Rand_ID);

                        if (lotteryRecord != null)
                        {
                            foreach (int randomID in lotteryRecord.idlist)
                            {
                                Tbl_RandItem_Record recordRandom = AsTableManager.Instance.GetTbl_RandItem_Record(randomID);

                                if (recordRandom.idlist.Contains(achGetItem.ItemID))
                                {
                                    RemoveTargetInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (itemDropMonKind.ItemID == achGetItem.ItemID)
                        RemoveTargetInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion);
                }
            }
        }
        else if (achType == typeof(AchUseItemToTarget))
        {
            AchUseItemToTarget useItemToTarget = _clearAchieve as AchUseItemToTarget;

            if (useItemToTarget.targetType == UseItemToTargetType.NPC)
            {
                int itemID = useItemToTarget.ItemID;

                // Item Drop Mon
                foreach (PrepareItemDropMonster itemDropMon in questData.Preparation.ItemDropMonsterList)
                {
                    if (itemDropMon.ItemID == itemID)
                        RemoveTargetInfo(itemDropMon.TargetMonsterID, itemDropMon.Champion);
                }


                // Item Drop Mon Kind
                foreach (PrepareItemDropMonsterKind itemDropMonKind in questData.Preparation.ItemDromMonsterKindList)
                {
                    if (itemDropMonKind.ItemID == itemID)
                        RemoveTargetInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion);

                }
            }
            else
            {
                RemoveTargetInfo(_clearAchieve.Target, _clearAchieve.Champion);
            }

        }
        else
        {
            RemoveTargetInfo(_clearAchieve.Target, _clearAchieve.Champion);
        }

        AsHUDController.Instance.panelManager.UpdateMonsterNamePanel();
    }

    public void RemoveMonInfoForTargetMark(QuestData _questData)
    {
        if (_questData == null)
        {
            Debug.LogWarning("Quest Data is null");
            return;
        }


        // Item Drop Mon

        List<AchGetItem> listGetItem = _questData.Achievement.GetDatas<AchGetItem>();

        foreach (AchGetItem getItem in listGetItem)
        {
            if (getItem.IsComplete == true)
                continue;

            foreach (PrepareItemDropMonster itemDropMon in _questData.Preparation.ItemDropMonsterList)
            {
                if (itemDropMon.ItemID == getItem.ItemID)
                    RemoveTargetInfo(itemDropMon.TargetMonsterID, itemDropMon.Champion);
            }

        }

        // Item Drop Mon Kind

        foreach (AchGetItem getItem in listGetItem)
        {
            if (getItem.IsComplete == true)
                continue;

            foreach (PrepareItemDropMonsterKind itemDropMonKind in _questData.Preparation.ItemDromMonsterKindList)
            {
                if (itemDropMonKind.ItemID == getItem.ItemID)
                    RemoveTargetInfo(itemDropMonKind.TargetMonsterKindID, itemDropMonKind.Champion);
            }
        }

        // Use Item To Target
        List<AchUseItemToTarget> listUseItemToTarget = _questData.Achievement.GetDatas<AchUseItemToTarget>();
        foreach (AchUseItemToTarget useItemToTarget in listUseItemToTarget)
        {
            if (useItemToTarget.IsComplete == false)
                if (useItemToTarget.targetType == UseItemToTargetType.MONSTER)
                    RemoveTargetInfo(useItemToTarget.targetID, useItemToTarget.Champion);
        }

        // Kill Monster
        List<AchKillMonster> listKillMon = _questData.Achievement.GetDatas<AchKillMonster>();
        foreach (AchKillMonster killMon in listKillMon)
        {
            if (killMon.IsComplete == false)
                RemoveTargetInfo(killMon.MonsterID, killMon.Champion);
        }

        // Kill Monster Kind
        List<AchKillMonsterKind> listKillMonKind = _questData.Achievement.GetDatas<AchKillMonsterKind>();
        foreach (AchKillMonsterKind killMonKind in listKillMonKind)
        {
            if (killMonKind.IsComplete == false)
                RemoveTargetInfo(killMonKind.MonsterKindID, killMonKind.Champion);
        }

        // Get Item From Mon
        List<AchGetItemFromMonster> listGetItemFromMon = _questData.Achievement.GetDatas<AchGetItemFromMonster>();
        foreach (AchGetItemFromMonster getItemFromMon in listGetItemFromMon)
        {
            if (getItemFromMon.IsComplete == false)
                RemoveTargetInfo(getItemFromMon.MonsterID, getItemFromMon.Champion);
        }

        // Get Item From Mon
        List<AchGetItemFromMonsterKind> listGetItemFromMonKind = _questData.Achievement.GetDatas<AchGetItemFromMonsterKind>();
        foreach (AchGetItemFromMonsterKind getItemFromMonKind in listGetItemFromMonKind)
        {
            if (getItemFromMonKind.IsComplete == false)
                RemoveTargetInfo(getItemFromMonKind.MonsterKindID, getItemFromMonKind.Champion);
        }

        AsHUDController.Instance.panelManager.UpdateMonsterNamePanel();
    }

    QuestTargetMarkInfo FindTargetInList(int _id, bool _champion)
    {
        foreach (QuestTargetMarkInfo info in listMonInfoConnectionQuest)
        {
            if (info.targetID != _id)
                continue;

            if (_champion == true)
                return info;
            else
            {
                if (info.champion == false)
                    return info;
            }
        }

        return null;
    }

    bool RemoveTargetInfo(int _id, bool _champion)
    {
        QuestTargetMarkInfo findInfo = null;

        foreach (QuestTargetMarkInfo info in listMonInfoConnectionQuest)
        {
            if (info.targetID == _id && info.champion == _champion)
            {
                findInfo = info;
                break;
            }
        }

        if (findInfo != null)
        {
            listMonInfoConnectionQuest.Remove(findInfo);
            return true;
        }
        else
        {
            return false;
        }
    }
    
}