using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;
using System.Text;

public class QuestListControll : UIMessageBase
{
    public  UIScrollList     questList;
    public  UIButton         questCloseBtn;
    public  SimpleSprite     spriteMoreList = null;
    public  SpriteText       spriteTitle    = null;
    public  GameObject       questOwnObject = null;
    public  GameObject       questPrefab;
    public  string[]         stateColors;
    public  string           stateColorUpperLv;
    private AsNpcEntity      npcEntity                  = null;
    private bool             visible                    = false;
    private QuestData        clickQuestData             = null;
    private List<QuestData>  listQuests                 = new List<QuestData>();
    private List<int>        listUpperLevel             = new List<int>();
    private List<UIListItem> listUIListItem             = new List<UIListItem>();
    private Dictionary<GameObject, int>    dicQuest     = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, string> dicQuestName = new Dictionary<GameObject, string>();

    public Color colorNormal;
    public Color colorProgressIn;
    public Color colorClear;
    public Color colorImmediatelyClear;
    public Color colorComplete;
    public Color colorFail;
    public Color colorUpperLv;

    #region -delegate-
    public delegate void dele_EventProcess();
    public dele_EventProcess Click_Event;
    public dele_EventProcess Close_Event;
    #endregion

    #region -property-
    public bool isDebugMode = false;
    public bool Visible
    {
        set
        {
            visible = value;
            questList.gameObject.SetActive(visible);
        }
        get { return visible; }
    }
    #endregion

    // Use this for initialization
	void Start () 
    {
        spriteMoreList.Hide(true);
	}

    public void Init()
    {
        questList.itemSpacing = 0.4f;
        questList.SetValueChangedDelegate(SelectQuestDelegate);

        stateColors    = new string[6];

        for (int i = 0; i < 6; i++)
        {
            switch ((QuestProgressState)i)
            {
                case QuestProgressState.QUEST_PROGRESS_NOTHING:
                    stateColors[i] = colorNormal.ToString();//"RGBA(1.0, 1.0, 1.0, 1.0)";
                    break;
                case QuestProgressState.QUEST_PROGRESS_IN:
                    stateColors[i] = colorProgressIn.ToString();//"RGBA(0.082, 0.66, 1.0, 1.0)";
                    break;
                case QuestProgressState.QUEST_PROGRESS_CLEAR:
                      stateColors[i] = colorClear.ToString();//"RGBA(1.0, 0.858, 0.3, 1.0)";
                    break;
                case QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR:
                    stateColors[i] = colorImmediatelyClear.ToString();//"RGBA(1.0, 0.858, 0.3, 1.0)";
                    break;
                case QuestProgressState.QUEST_PROGRESS_COMPLETE:
                    stateColors[i] = colorComplete.ToString();//"RGBA(0.7, 0.7, 0.7, 1.0)";
                    break;
                case QuestProgressState.QUEST_PROGRESS_FAIL:
                    stateColors[i] = colorFail.ToString();//"RGBA(1.0, 0.352, 0.262, 1.0)";
                    break;
            }
        }

        stateColorUpperLv = colorUpperLv.ToString();// "RGBA(0.392, 0.392, 0.392, 1.0)";
    }

    public void FindQuestFromObject(GameObject gameobject)
    {
        Init();

        questOwnObject = gameobject;

        npcEntity      = questOwnObject.GetComponentInChildren<AsNpcEntity>();

        if (UpdateQuestList() == true)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.SHOW_QUESTLIST));
    }

    public void Close()
    {
        AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
        questOwnObject = null;
		DeleteAllQuest();
        gameObject.SetActive(false);
    }

    public bool UpdateQuestList()
    {
        if (questOwnObject != null)
        {
            listUIListItem.Clear();
            listQuests.Clear();
            listUpperLevel.Clear();
            DeleteAllQuest();

			Debug.LogWarning("Update Quest List = " + npcEntity.TableIdx);


            QuestHolder questHolder = QuestHolderManager.instance.GetQuestHolder(npcEntity.TableIdx);

            if (questHolder == null)
            {
                Debug.LogWarning("Quest Hoder is null");
				return false;
            }

            List<QuestData> questDatas = questHolder.GetAllQuestData();
            List<int> listQuestIDAccepted = ArkQuestmanager.instance.GetQuestIDs();


            // 수락 가능한 녀석만 추가
            List<QuestData> listQuestListWithoutDaily = new List<QuestData>();
            List<QuestData> listQuestListAddLevel = new List<QuestData>();
            AsUserEntity user = AsEntityManager.Instance.UserEntity;

            if (user != null)
            {
                foreach (QuestData questData in questDatas)
                {
                    if (questData.Info.QuestType == QuestType.QUEST_DAILY)
                        continue;

                    if (questData.Info.QuestSuggestNpcID == npcEntity.TableIdx)// 제안 가능
                    {
                        if (questData.Info.QuestCompleteNpcID == npcEntity.TableIdx) // 완료가능
                        {
							if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
								listQuestListWithoutDaily.Add(questData);
                            else if (ArkQuest.IsPossibleAccept(user, questData))
                                listQuestListWithoutDaily.Add(questData);
                            else
                            {
                                if (ArkQuest.IsPossibleAccept(user, questData, 3))
                                {
                                    listUpperLevel.Add(questData.Info.ID);
                                    listQuestListAddLevel.Add(questData);
                                }
                            }
                        }
                        else // 제안가능 & 완료아님
                        {
                            if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING || questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                            {
                                if (ArkQuest.IsPossibleAccept(user, questData))
                                    listQuestListWithoutDaily.Add(questData);
                                else
                                {
                                    if (ArkQuest.IsPossibleAccept(user, questData, 3))
                                    {
                                        listUpperLevel.Add(questData.Info.ID);
                                        listQuestListAddLevel.Add(questData);
                                    }
                                }
                            }
                        }
                    }
                    else // 수락된 퀘스트 & 완료 되었을 경우
                    {
                        if (listQuestIDAccepted.Contains(questData.Info.ID) && (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR ||
                                                                                questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)) // if is it complete npc check clear
                            listQuestListWithoutDaily.Add(questData);
                    }
                }
            }
            else
                Debug.Log("User entity is null");

            // 실패
            foreach (QuestData questData in listQuestListWithoutDaily)
            {
                if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
                {
                    listQuests.Add(questData);
                    AddQuest(questData);
                }
            }

            // 완료
            foreach (QuestData questData in listQuestListWithoutDaily)
            {
                if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
                {
                    listQuests.Add(questData);
                    AddQuest(questData);
                }
            }

            // 기본
            foreach (QuestData questData in listQuestListWithoutDaily)
            {
                if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
                {
                    listQuests.Add(questData);
                    AddQuest(questData);
                }
            }


            // 진행중
            foreach (QuestData questData in listQuestListWithoutDaily)
            {
                if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                {
                    listQuests.Add(questData);
                    AddQuest(questData);
                }
            }

            // up level
            foreach (QuestData questData in listQuestListAddLevel)
            {
                if (questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
                {
                    listQuests.Add(questData);
                    AddQuest(questData);
                }
            }

            //// 끝
            //foreach (ArkQuest quest in listQuestListWithoutDaily)
            //{
            //    if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_COMPLETE)
            //    {
            //        listQuests.Add(quest);
            //        AddQuest(quest.GetQuestData().Info.Name, quest.GetQuestData().Info.ID);
            //    }
            //}

            UpdateColor();

            CheckMorelistMark();

            if (spriteTitle != null && isDebugMode == true)
                spriteTitle.Text = questHolder.nowQuetMarkType.ToString();

			return true;
        }

		return false;
    }

    private void CloseBtnDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
            Close();
    }

    public void SelectQuestDelegate(IUIObject obj)
    {
        if (obj != questList)
            return;

        if (questList.SelectedItem == null)
            return;

        AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
        int nClickIdx = dicQuest[questList.SelectedItem.gameObject];
        clickQuestData = listQuests[nClickIdx];
        Click_Event();
    }

    public void UpdateColor()
    {
        foreach (GameObject obj in dicQuest.Keys)
        {
            if (!dicQuest.ContainsKey(obj) || !dicQuestName.ContainsKey(obj))
            {
                Debug.Log("quest controll.cs  not exist obj");
                continue;
            }

            int idx          = dicQuest[obj];
            string questName = dicQuestName[obj];

            if (idx <= -1 || listQuests.Count <= idx)
            {
                Debug.Log("index out of range =  "+ idx);
                continue;
            }

            QuestProgressState nowState = listQuests[idx].NowQuestProgressState;
            int intState = (int)nowState;

            StringBuilder sb = null;

            // +3 level
            if (listUpperLevel.Contains(listQuests[idx].Info.ID))
            {
                sb = new StringBuilder(stateColorUpperLv);
                sb.Append(questName);
                sb.Append(listQuests[idx].GetRepeatString());
                listUIListItem[idx].Text = sb.ToString();
            }
            else
            {
                sb = new StringBuilder(stateColors[intState]);
                sb.Append(questName);
                sb.Append(GetStateString(nowState));
                sb.Append(listQuests[idx].GetRepeatString());
                listUIListItem[idx].Text = sb.ToString();
            }
        }
    }

    public string GetStateString(QuestProgressState _state)
    {
        StringBuilder sb = new StringBuilder(" ");

        if (_state == QuestProgressState.QUEST_PROGRESS_IN)
            sb.Append(AsTableManager.Instance.GetTbl_String(813));
        else if (_state == QuestProgressState.QUEST_PROGRESS_CLEAR || _state == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
            sb.Append(AsTableManager.Instance.GetTbl_String(814));
        else if (_state == QuestProgressState.QUEST_PROGRESS_FAIL)
            sb.Append(AsTableManager.Instance.GetTbl_String(815));
        else
            return string.Empty;

        return sb.ToString();
    }

    public QuestData GetClickQuestData()
    {
        return clickQuestData;
    }

    public bool IsUpperLevelQuest(QuestData _questData)
    {
        return listUpperLevel.Contains(_questData.Info.ID);
    }

    void OnEnable()
    {
        CheckMorelistMark();
    }

    void CheckMorelistMark()
    {
        if (questList == null)
            return;

        if (questList.Count > 0 && questList.gameObject.activeSelf)
        {
            if (!questList.IsShowingLastItem())
            {
                if (spriteMoreList.IsHidden() == true)
                    spriteMoreList.Hide(false);
            }
            else
            {
                if (spriteMoreList.IsHidden() == false)
                    spriteMoreList.Hide(true);
            }
        }
        else
        {
            if (spriteMoreList.IsHidden() == false)
                spriteMoreList.Hide(true);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckMorelistMark();
    }

    public QuestData GetClearQuest()
    {
        if (listQuests == null)
            return null;

        if (listQuests.Count <= 0)
            return null;

        if (listQuests[0].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || listQuests[0].NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
            return listQuests[0];
        else
            return null;
    }

    void AddQuest(QuestData _questData)
    {
        int nQuestCount = questList.Count;
		IUIListObject item = questList.CreateItem(questPrefab, nQuestCount, _questData.Info.Name);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(item.TextObj);

        QuestListIconController questListIconCon = item.gameObject.GetComponent<QuestListIconController>();

        if (questListIconCon != null)
            questListIconCon.SetQuestIconType(_questData.Info.QuestType);

        if (!dicQuest.ContainsKey(item.gameObject))
        {
            dicQuest.Add(item.gameObject, nQuestCount);
            dicQuestName.Add(item.gameObject, _questData.Info.Name);
            listUIListItem.Add(item.gameObject.GetComponent<UIListItem>());
        }
    }

    public void DeleteAllQuest()
    {
        dicQuest.Clear();
        dicQuestName.Clear();
        questList.ClearListSync(true);
    }

    /// <summary>
    /// 아무것도 선택되지 않은 상태로 돌린다.
    /// </summary>
    public void NoneSelect()
    {
        questList.SelectedItem = null;
    }

    public override void ProcessUIMessage(UIMessageObject message)
    {
        if (gameObject.activeSelf == false)
            return;

        switch (message.messageType)
        {
            case UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE:
                UpdateQuestList();
                break;

            case UIMessageType.UI_MESSAGE_QUESTLIST_NONESELECT:
                NoneSelect();
                break;

            case UIMessageType.UI_MESSAGE_QUESTLIST_CLOSE:
                Close();
                break;
        }
    }
}
