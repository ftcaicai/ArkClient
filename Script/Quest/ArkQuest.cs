using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;
using System.Text;

public class ArkQuest : MonoBehaviour  {

    private QuestData       questData      = null;
    private AchTimeLimit    timeLimit      = null;
    private AchTimeSurvival timeSurvival   = null;
    private bool            sendClear      = false;
    private bool            completeQuest  = false;
	private bool            suggestQuest   = false;
    private SpriteText      timeSpriteText = null;
    private static float    toMinute       = 1.0f / 60.0f;
    private float           startTime      = 0.0f;

    public bool IsCompleteQuest
    {
        get { return completeQuest; }
        set { completeQuest = value; }
    }

    public SpriteText TimeSpriteText
    {
        get { return timeSpriteText;  }
        set { timeSpriteText = value; }
    }
	
	public bool IsSuggestQeust
	{
		get { return suggestQuest; }
        set { suggestQuest = value; }
	}

    public QuestProgressState NowQuestProgressState
    {
        get { return questData.NowQuestProgressState; }
        set { questData.NowQuestProgressState = value; }
    }

    private Dictionary<QuestProgressState, List<QuestTalkInfo>> dicQuestTalks = new Dictionary<QuestProgressState, List<QuestTalkInfo>>();

    void Start()
    {
     
    }
	
	// Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    string TimeToText(float fTime)
    {
        int minute = (int)(fTime * toMinute);
        float sec  = fTime - ((float)minute * 60.0f);

        if (sec <= 0.01)
            sec = 0.0f;

        return string.Format("{0:00}:{1:00.00}", minute, sec);
    }

    public List<QuestTalkInfo> GetTalkInfo(QuestProgressState _state)
    {
        if (dicQuestTalks.ContainsKey(_state))
            return dicQuestTalks[_state];
        else
            return new List<QuestTalkInfo>();
    }

    public void SetQuestData(QuestData _questData)
    {
        questData = _questData;

        SetQuestString(questData);

        Tbl_QuestTalk_Record talkRecord = AsTableManager.Instance.GetTbl_QuestTalk(_questData.Info.ID);

        if (talkRecord != null)
            dicQuestTalks = talkRecord.questTalk.GetTalkStringAll();
    }

    public static void SetQuestString(QuestData _questData)
    {
        Tbl_QuestStringRecord stringRecord = AsTableManager.Instance.GetTbl_QuestString_Record(_questData.Info.ID);
        _questData.Info.Name = stringRecord.Info.Title;
        _questData.Info.Explain = stringRecord.Info.Explain;
        _questData.Info.Achievement = stringRecord.Info.Achievement;
    }

    public static void SetQuestStrings(QuestData _questData)
    {
        Tbl_QuestStringRecord stringRecord = AsTableManager.Instance.GetTbl_QuestString_Record(_questData.Info.ID);

        if (stringRecord == null)
        {
            Debug.LogError("Quest string(" + _questData.Info.ID + ") is null");
            return;
        }

        _questData.Info.Name = stringRecord.Info.Title;
        _questData.Info.Explain = stringRecord.Info.Explain;
        _questData.Info.Achievement = stringRecord.Info.Achievement;


        // 퀘스트 목표 스트링 추가
        Tbl_QuestStringRecord questString = AsTableManager.Instance.GetTbl_QuestString_Record(_questData.Info.ID);

        if (questString != null)
        {
            List<AchBase> listAchievement = _questData.Achievement.GetDatas();
            string[] achStrings = questString.Info.Achievement.Split('\r');

            int count = achStrings.Length;

            for (int i = 0; i < count; i++)
            {
                if (achStrings[i] == null)
                    continue;

                if (i < listAchievement.Count)
                    listAchievement[i].Achievement = achStrings[i].Trim();
            }
        }
        
        Tbl_QuestTalk_Record talkRecord = AsTableManager.Instance.GetTbl_QuestTalk(_questData.Info.ID);

        if (talkRecord != null)
            _questData.SetQuestTalks(talkRecord.questTalk.GetTalkStringAll());
        else
            Debug.LogWarning("Talk is not exist = " + _questData.Info.ID);
    }

    public void ResetQuestAchievement()
    {
        questData.Achievement.Reset();
    }

    public bool IsPossibleAccept(AsUserEntity _user)
    {
        return IsPossibleAccept(_user, questData);
    }

    public static bool IsPossibleAccept(AsUserEntity _user, QuestData _questData)
    {
        return IsPossibleAccept(_user, _questData, 0);
    }

    public static bool IsPossibleAccept(AsUserEntity _user, QuestData _questData, int _addLevel)
    {
        if (_questData.Condition.CanAccept() == false)
            return false;

        // 레벨 확인
        List<ConditionLevel> listConditionLevel = _questData.Condition.GetCondition<ConditionLevel>();
        ConditionLevel levelConditon = new ConditionLevel();
        if (listConditionLevel.Count > 0)
        {
            int level = _user.GetProperty<int>(eComponentProperty.LEVEL) + _addLevel;
            levelConditon.CommonValue = level;

            if (!listConditionLevel[0].CheckAccept(levelConditon))
                return false;
        }

        // 퀘스트 패스 확인  or
        bool bAcceptAnd = true;
        bool bAcceptOr  = false; 
        int  nOrCount   = 0;
        List<ConditionQuestPass> listQuestPass = _questData.Condition.GetCondition<ConditionQuestPass>();

        if (listQuestPass.Count > 0)
        {
            foreach (ConditionQuestPass questPass in listQuestPass)
            {
                if (questPass.QuestPassType == QuestPassType.AND)
                    bAcceptAnd &= ArkQuestmanager.instance.IsCompleteQuest(questPass.QuestID);
                else
                {
                    bAcceptOr |= ArkQuestmanager.instance.IsCompleteQuest(questPass.QuestID);
                    nOrCount++;
                }
            }

            if (nOrCount > 0)
            {
                if (bAcceptAnd == false)
                    return false;

                if (bAcceptOr == false)
                    return false;
            }
            else
            {
                if (bAcceptAnd == false)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Time Limit 카운트 시작
    /// </summary>
    public void StartTimeLimit()
    {
        if (questData == null)
            return;

        List<AchTimeLimit> listTimeLimit = questData.Achievement.GetDatas<AchTimeLimit>();

        if (listTimeLimit.Count >= 1)
        {
            timeLimit = listTimeLimit[0];

            Debug.LogWarning("Time limit start");

            timeLimit.Start = true;

            startTime = Time.realtimeSinceStartup;
            return;
        }

        List<AchTimeSurvival> listTimeSurvival = questData.Achievement.GetDatas<AchTimeSurvival>();

        if (listTimeSurvival.Count >= 1)
        {
            timeSurvival = listTimeSurvival[0];

            Debug.LogWarning("Time survival start");

            timeSurvival.Start = true;

            startTime = Time.realtimeSinceStartup;

            return;
        }
    }

    void CheckTime()
    {
        if (questData == null)
            return;

        if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
        {
            if (timeSurvival != null || timeLimit != null)
            {
                float fNowTime = 0.0f;

                float delthaTime = Time.realtimeSinceStartup - startTime;

                startTime = Time.realtimeSinceStartup;

                if (timeLimit != null && timeLimit.IsEnd == false)
                {
                    fNowTime = timeLimit.DiscountTime(delthaTime);

                    if (timeSpriteText != null)
                        timeSpriteText.Text = TimeToText(fNowTime);

                    if (timeLimit.IsEnd)
                        NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_FAIL;
                }
                else if (timeSurvival != null && timeSurvival.IsEnd == false)
                {
                    fNowTime = timeSurvival.DiscountTime(delthaTime);

                    if (timeSpriteText != null)
                        timeSpriteText.Text = TimeToText(fNowTime);

                    if (timeSurvival.IsEnd)
                        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_TIME_SURVIVAL, new object());
                }
            }
        }
    }

    public bool HaveTimeQuest()
    {
        return timeLimit != null || timeSurvival != null;
    }

    public QuestData GetQuestData()
    {
        return questData;
    }

    public void SetQuestImmediatelyClear()
    {
        List<AchBase> listAchievement = questData.Achievement.GetDatas();

        foreach (AchBase ach in listAchievement)
        {
            ach.SetImmediatelyClear();
            NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR;
        }
    }

    public bool ContainTalkWithNPC(int _npcID)
    {
        return questData.Achievement.ContainTalkWithNpc(_npcID);
    }

    public bool ContainTalkWithNPC()
    {
        return questData.Achievement.GetDataCount<AchTalkNPC>() >= 1 ? true : false;
    }

    public void ProcessMessage(object _messageObject)
    {
        // wanted는 제외
        if (questData.Info.QuestType == QuestType.QUEST_WANTED)
            return;

		if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN || NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR)
		{
			QuestMessageObject messageObject = (QuestMessageObject)_messageObject;
			questData.Achievement.ProcessMessage(messageObject.MessageType, messageObject.Data);

			if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				// time limit 가 있어야 되는 경우 따로 체크
				if (timeLimit != null)
					questData.Achievement.ProcessMessage(QuestMessages.QM_TIME_LIMIT, new Object());

				// time Survivla이 있을경우 따로 체크
				if (timeSurvival != null)
					questData.Achievement.ProcessMessage(QuestMessages.QM_TIME_SURVIVAL, new Object());
			}

			// 퀘스트 완료 체크
			if (questData.Achievement.IsComplete())
			{
				if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN && sendClear == false)
				{
					Debug.LogWarning("req quest clear = " + questData.Info.Name);
					sendClear = true;
					ArkQuestmanager.instance.RequestClearQuest(questData.Info.ID);
				}
			}
			else // not complete
			{
				if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR)
				{
					sendClear = false;
					NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_IN;
					ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();
					ArkQuestmanager.instance.RequestClearQuest(questData.Info.ID);

					if (AsHudDlgMgr.Instance != null)
						if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == true)
							AsHudDlgMgr.Instance.questMiniView.UpdateQuetMiniViewMgr();
				}
			}
		}
    }

    public List<int> GetQuestItemIDList()
    {
        List<int> listItem = new List<int>();

        if (NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN ||
            NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR ||
            NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
        {
            QuestAchievement ach = questData.Achievement;

            List<AchGetItem>                listGetItem            = ach.GetDatas<AchGetItem>();
            List<AchGetItemFromMonster>     listGetItemFromMon     = ach.GetDatas<AchGetItemFromMonster>();
            List<AchGetItemFromMonsterKind> listGetItemFromMonKind = ach.GetDatas<AchGetItemFromMonsterKind>();
            List<AchGetQuestCollection>     listGetQuestCollection = ach.GetDatas<AchGetQuestCollection>();

            foreach (AchGetItem item in listGetItem)
            {
                if (!listItem.Contains(item.ItemID))
                    listItem.Add(item.ItemID);
            }

            foreach (AchGetItemFromMonster item in listGetItemFromMon)
            {
                if (!listItem.Contains(item.ItemID))
                    listItem.Add(item.ItemID);
            }

            foreach (AchGetItemFromMonsterKind item in listGetItemFromMonKind)
            {
                if (!listItem.Contains(item.ItemID))
                    listItem.Add(item.ItemID);
            }

            foreach (AchGetQuestCollection item in listGetQuestCollection)
            {
                if (!listItem.Contains(item.ItemID))
                {
                    listItem.Add(item.ItemID);
                    Debug.Log(item.ItemID);
                }
            }
        }

        return listItem;
    }

    public static ArkQuest CreateArkQuest(int _questID, GameObject _objParents)
    {
        return CreateArkQuestCore(_questID, _objParents, "Quest_");
    }
	
	public static ArkQuest CreateArkQuest(int _questID, GameObject _objParents, string _lastName)
	{
		return CreateArkQuestCore(_questID, _objParents, _lastName);
	}
	
	static ArkQuest CreateArkQuestCore(int _questID, GameObject _objParents,  string _lastName)
    {
        Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord(_questID);
        if (questRecord != null)
        {
            StringBuilder sb = null;

            if (questRecord.QuestDataInfo.Info.QuestType == QuestType.QUEST_WANTED)
                sb = new StringBuilder("Wanted_");
            else if (questRecord.QuestDataInfo.Info.QuestType == QuestType.QUEST_DAILY)
                sb = new StringBuilder("Daily_");
            else
                sb = new StringBuilder("Main_");

            sb.Append(_lastName);
            sb.Append(questRecord.QuestDataInfo.Info.ID);

            GameObject questObj = new GameObject(sb.ToString());
            ArkQuest arkQuest = questObj.AddComponent<ArkQuest>();
            arkQuest.SetQuestData(questRecord.QuestDataInfo);
            questObj.transform.parent = _objParents.transform;

            // 퀘스트 목표 스트링 추가
            Tbl_QuestStringRecord questString = AsTableManager.Instance.GetTbl_QuestString_Record(_questID);

            if (questString != null)
            {
                List<AchBase> listAchievement = questRecord.QuestDataInfo.Achievement.GetDatas();
                string[] achStrings = questString.Info.Achievement.Split('\r');

                int count = achStrings.Length;

                for (int i = 0; i < count; i++)
                {
                    if (achStrings[i] != null)
                    {
                        if (i < listAchievement.Count)
                            listAchievement[i].Achievement = achStrings[i].Trim();
                    }
                }
            }
            return arkQuest;
        }
        return null;
    }
}
