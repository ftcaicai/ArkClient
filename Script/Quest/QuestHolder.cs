using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;

public enum QuestMarkType
{
	NOTHING,
	HAVE,
	CLEAR,
	CLEAR_REMAINTALK,
	CLEAR_AND_HAVE,
	PROGRESS,
	UPPERLEVEL,
	TALK_CLEAR,
	TALK_HAVE,
	HAVE_EVENT,
	HAVE_EVENT_AND_PROGRESS,
	LOWERLEVEL,
	LOWERLEVEL_AND_HAVE_EVENT,
	HAVE_NPC_DAILY,
	CLEAR_NPC_DAILY,
};

public class QuestHolder : MonoBehaviour
{
	int npcTableID = 0;
	bool settingQuest = false;
	public List<int> listQuest = new List<int>();
	public List<int> listNpcDailyQuest = new List<int>();
	public List<int> listSuggestQuest = new List<int>();
	public List<int> listCompleteQuest = new List<int>();
	List<Tbl_Quest_Record> listQuestRecord = new List<Tbl_Quest_Record>();
	public QuestMarkType nowQuetMarkType = QuestMarkType.NOTHING;

	public QuestMarkType NowMarkType { get { return nowQuetMarkType; } }

	void Start()
	{

	}

	public static bool HaveQuest(int _npcID)
	{
		Tbl_Quest_Record[] quests = AsTableManager.Instance.GetTbl_QuestRecordsByNpc(_npcID);

		if (quests == null)
			return false;
		else
			return true;
	}

	public void SettingQuest(int _npcID)
	{
		bool debug = false;

		listQuest.Clear();
		listQuestRecord.Clear();

		npcTableID = _npcID;

		Tbl_Quest_Record[] records = AsTableManager.Instance.GetTbl_QuestRecordsByNpc(npcTableID);

		if (records == null)
		{
			settingQuest = true;
			return;
		}


		Tbl_Npc_Record npcRecord = AsTableManager.Instance.GetTbl_Npc_Record(_npcID);

		if (debug)
			Debug.LogWarning("------------------------------------[" + AsTableManager.Instance.GetTbl_String(npcRecord.m_iNpcNameId)+ "]");


		foreach (Tbl_Quest_Record record in records)
		{
			if (record.QuestDataInfo.Info.QuestType == QuestType.QUEST_NPC_DAILY)
				if (record.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
					continue;
				else
				{
					if (!listNpcDailyQuest.Contains(record.QuestDataInfo.Info.ID))
						listNpcDailyQuest.Add(record.QuestDataInfo.Info.ID);

					if (debug)
						Debug.LogWarning("Set NPC Daily ID = " + record.QuestDataInfo.Info.ID);
				}

			listQuestRecord.Add(record);
		}

		if (debug)
			Debug.LogWarning("====================================");

		listCompleteQuest	= AsTableManager.Instance.GetTbl_QuestIDByNpcList(npcTableID, QuestTableType.QUEST_TABLE_COMPLETE);
		listSuggestQuest	= AsTableManager.Instance.GetTbl_QuestIDByNpcList(npcTableID, QuestTableType.QUEST_TABLE_SUGGEST);

		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			listQuest.Add(questRecord.QuestDataInfo.Info.ID);
			ArkQuest.SetQuestStrings(questRecord.QuestDataInfo);
		}

		settingQuest = true;
	}

	public void UpdateForNpcDaily()
	{
		// clear previous npc daily quest
		List<int> listPrev = new List<int>(listNpcDailyQuest);

		foreach (int questID in listPrev)
		{
			Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord(questID);

			if (questRecord == null)
				continue;

			if (questRecord.QuestDataInfo.Info.QuestType != QuestType.QUEST_NPC_DAILY)
				continue;

			if (questRecord.QuestDataInfo.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_NOTHING)
				continue;

			if (listQuestRecord.Contains(questRecord))
				listQuestRecord.Remove(questRecord);

			if (listQuest.Contains(questID))
				listQuest.Remove(questID);

			if (listSuggestQuest.Contains(questID))
				listSuggestQuest.Remove(questID);

			if (listCompleteQuest.Contains(questID))
				listCompleteQuest.Remove(questID);

			if (listNpcDailyQuest.Contains(questID))
				listNpcDailyQuest.Remove(questID);
		}

		List<int> listNpcDailyPickup = ArkQuestmanager.instance.GetNpcDailyQuestList(npcTableID);

		foreach (int questID in listNpcDailyPickup)
		{
			Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord(questID);

			if (questRecord == null)
				continue;

			QuestData questData = questRecord.QuestDataInfo;

			// check npc id
			if (questData.Info.QuestSuggestNpcID != npcTableID)
				continue;

			if (listQuest.Contains(questID))
				continue;

			if (listNpcDailyQuest.Contains(questID))
				continue;

			if (!ArkQuestmanager.instance.CanAcceptNpcDailyQuest(questData.Info.NpcDailyGroupID))
			{
				Debug.LogWarning("npc Daily Quest group["+questData.Info.NpcDailyGroupID+"] already added = " + questID);
				continue;
			}

			listQuestRecord.Add(questRecord);
			listQuest.Add(questID);
			listNpcDailyQuest.Add(questID);

			if (npcTableID == questRecord.QuestDataInfo.Info.QuestCompleteNpcID)
			{
				if (!listCompleteQuest.Contains(questID))
					listCompleteQuest.Add(questID);
			}

			if (npcTableID == questRecord.QuestDataInfo.Info.QuestSuggestNpcID)
			{
				if (!listSuggestQuest.Contains(questID))
					listSuggestQuest.Add(questID);
			}

			ArkQuest.SetQuestStrings(questRecord.QuestDataInfo);
		}
	}

	public List<QuestData> GetAllQuestData()
	{
		List<QuestData> listQuestData = new List<QuestData>();

		foreach (int questID in listQuest)
		{
			QuestData data = AsTableManager.Instance.GetTbl_QuestRecord(questID).QuestDataInfo;

			if (data == null)
				continue;

				listQuestData.Add(data);
		}

		return listQuestData;
	}

	public QuestMarkType UpdateQuestMark(int _npcID)
	{
		if (!settingQuest)
			return nowQuetMarkType;

		AsNpcEntity npcEntity = AsEntityManager.Instance.GetNPCEntityByTableID(_npcID);
		AsUserEntity userEntity = AsEntityManager.Instance.UserEntity;
		int userLv = userEntity.GetProperty<int>(eComponentProperty.LEVEL);

		if (listQuestRecord.Count <= 0)
		{
			// check have talk
			if (ArkQuestmanager.instance.HaveTalkAchievement(_npcID))
			{
				nowQuetMarkType = QuestMarkType.TALK_HAVE;
				SetQuestMarkPanel(npcEntity, QuestMarkType.TALK_HAVE);
			}
			else
			{
				nowQuetMarkType = QuestMarkType.NOTHING;
				SetQuestMarkPanel(npcEntity, QuestMarkType.NOTHING);
			}

			return nowQuetMarkType;
		}


		// check clear
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listCompleteQuest.Contains(questRecord.QuestDataInfo.Info.ID) == false)
				continue;

			if (questRecord.QuestDataInfo.Info.QuestType == QuestType.QUEST_NPC_DAILY)
				continue;

			if (questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
			{
				nowQuetMarkType = QuestMarkType.CLEAR;
				SetQuestMarkPanel(npcEntity, QuestMarkType.CLEAR);
				return nowQuetMarkType;
			}
		}

		// check npc daily Clear
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listCompleteQuest.Contains(questRecord.QuestDataInfo.Info.ID) == false)
				continue;

			if (questRecord.QuestDataInfo.Info.QuestType != QuestType.QUEST_NPC_DAILY)
				continue;

			if (questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
			{
				nowQuetMarkType = QuestMarkType.CLEAR_NPC_DAILY;
				SetQuestMarkPanel(npcEntity, QuestMarkType.CLEAR_NPC_DAILY);
				return nowQuetMarkType;
			}
		}

		// check talk Clear
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listCompleteQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (!questData.Achievement.ContainTalkWithNpcOnly())
					continue;

				List<AchTalkNPC> listTalkNPC = questData.Achievement.GetDatas<AchTalkNPC>();

				if (listTalkNPC[0].NpcID == npcTableID)
				{
					nowQuetMarkType = QuestMarkType.TALK_CLEAR;

					SetQuestMarkPanel(npcEntity, QuestMarkType.TALK_CLEAR);
					return nowQuetMarkType;
				}
			}
		}


		// check remain talk
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listCompleteQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (questData.Achievement.IsCompleteWithoutTalkWithNPC() == true)
				{
					AchTalkNPC talkNpc = questData.Achievement.GetTalkWithNpc();

					if (talkNpc == null)
						continue;

					if (talkNpc.NpcID == _npcID)
					{
						nowQuetMarkType = QuestMarkType.CLEAR_REMAINTALK;
						SetQuestMarkPanel(npcEntity, QuestMarkType.CLEAR_REMAINTALK);
						return nowQuetMarkType;
					}
				}
			}
		}

		// check have talk
		if (ArkQuestmanager.instance.HaveTalkAchievement(_npcID))
		{
			nowQuetMarkType = QuestMarkType.TALK_HAVE;
			SetQuestMarkPanel(npcEntity, QuestMarkType.TALK_HAVE);
			return nowQuetMarkType;
		}

		// lower Level
		List<bool> listLower = new List<bool>();
		int lvGap = 0;
		bool haveNpcDaily = false;
		bool cancelLowerLevelByNpcDaily = false;
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (ArkQuest.IsPossibleAccept(userEntity, questData) == false)
					continue;

				if (questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
				{
					haveNpcDaily = true;
					continue;
				}
		
				List<ConditionLevel> listConditionLevel = questData.Condition.GetCondition<ConditionLevel>();

				if (listConditionLevel.Count > 0)
				{
					lvGap = userLv - listConditionLevel[0].MinLevel;
					listLower.Add(lvGap >= 10);
				}
				else
					listLower.Add(false);
			}
		}

		if (listLower.Count > 0 && !listLower.Contains(false) )
		{
			if (haveNpcDaily == false)
			{
				QuestMarkType eventMark = CheckInGameEvent(_npcID, userEntity, npcEntity);

				if (eventMark != QuestMarkType.NOTHING)
				{
					nowQuetMarkType = QuestMarkType.LOWERLEVEL_AND_HAVE_EVENT;
					SetQuestMarkPanel(npcEntity, QuestMarkType.LOWERLEVEL_AND_HAVE_EVENT);
					return QuestMarkType.LOWERLEVEL_AND_HAVE_EVENT;
				}

				nowQuetMarkType = QuestMarkType.LOWERLEVEL;
				SetQuestMarkPanel(npcEntity, QuestMarkType.LOWERLEVEL);
				return nowQuetMarkType;
			}
			else
			{
				cancelLowerLevelByNpcDaily = true;
			}
		}

		// have
		if (cancelLowerLevelByNpcDaily == false)
		{
			foreach (Tbl_Quest_Record questRecord in listQuestRecord)
			{
				if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
				{
					QuestData questData = questRecord.QuestDataInfo;

					if (ArkQuest.IsPossibleAccept(userEntity, questData) == false)
						continue;

					if (questData.Info.QuestType == QuestType.QUEST_NPC_DAILY)
						continue;

					nowQuetMarkType = QuestMarkType.HAVE;
					SetQuestMarkPanel(npcEntity, QuestMarkType.HAVE);
					return nowQuetMarkType;
				}
			}
		}

		// npc daily have
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (ArkQuest.IsPossibleAccept(userEntity, questData) == false)
					continue;

				if (questData.Info.QuestType != QuestType.QUEST_NPC_DAILY)
					continue;

				nowQuetMarkType = QuestMarkType.HAVE_NPC_DAILY;
				SetQuestMarkPanel(npcEntity, QuestMarkType.HAVE_NPC_DAILY);
				return nowQuetMarkType;
			}
		}

		// check have event
		QuestMarkType type = CheckInGameEvent(_npcID, userEntity, npcEntity);
		if (type != QuestMarkType.NOTHING)
			return type;

		// upper Level
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (ArkQuest.IsPossibleAccept(userEntity, questData, 3) == true)
				{
					nowQuetMarkType = QuestMarkType.UPPERLEVEL;
					SetQuestMarkPanel(npcEntity, QuestMarkType.UPPERLEVEL);
					return nowQuetMarkType;
				}
			}
		}

		// check progress
		foreach (Tbl_Quest_Record questRecord in listQuestRecord)
		{
			if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			{
				QuestData questData = questRecord.QuestDataInfo;

				if (ArkQuest.IsPossibleAccept(userEntity, questData) == true)
				{
					nowQuetMarkType = QuestMarkType.PROGRESS;
					SetQuestMarkPanel(npcEntity, QuestMarkType.PROGRESS);
					return nowQuetMarkType;
				}
			}
		}

		nowQuetMarkType = QuestMarkType.NOTHING;
		SetQuestMarkPanel(npcEntity, QuestMarkType.NOTHING);
		return nowQuetMarkType;
	}

	public QuestMarkType GetQuestMarkType()
	{
		return nowQuetMarkType;
	}

	public static void SetQuestMarkPanel(AsNpcEntity _npcEntity, QuestMarkType _markType)
	{
		if (_npcEntity != null)
		{
			if (_npcEntity.questPanel != null)
				_npcEntity.questPanel.SetMarkType(_markType);
		}
	}

	private QuestMarkType CheckInGameEvent(int _npcID, AsUserEntity _userEntity, AsNpcEntity _npcEntity)
	{
		// check have event
		List<Tbl_Event_Record> listEvent = AsTableManager.Instance.GetTbl_Event(_npcID, System.DateTime.Now.AddTicks(AsGameMain.serverTickGap));
		List<Tbl_Event_Record> listFiltered = new List<Tbl_Event_Record>();

		// filter list view
		foreach (Tbl_Event_Record record in listEvent)
		{
			if (record.viewList == true)
				continue;

			listFiltered.Add(record);
		}

		if (listFiltered.Count > 0)
		{
			// check have event and progress
			foreach (Tbl_Quest_Record questRecord in listQuestRecord)
			{
				if (listSuggestQuest.Contains(questRecord.QuestDataInfo.Info.ID) && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
				{
					QuestData questData = questRecord.QuestDataInfo;

					if (ArkQuest.IsPossibleAccept(_userEntity, questData) == true)
					{
						nowQuetMarkType = QuestMarkType.HAVE_EVENT_AND_PROGRESS;
						SetQuestMarkPanel(_npcEntity, QuestMarkType.HAVE_EVENT_AND_PROGRESS);
						return nowQuetMarkType;
					}
				}
			}

			nowQuetMarkType = QuestMarkType.HAVE_EVENT;
			SetQuestMarkPanel(_npcEntity, QuestMarkType.HAVE_EVENT);
			return nowQuetMarkType;
		}
		else
		{
			return QuestMarkType.NOTHING;
		}
	}
}
