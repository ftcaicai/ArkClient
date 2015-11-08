using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using ArkSphereQuestTool;

public enum QuestTableType
{
    QUEST_TABLE_SUGGEST,
    QUEST_TABLE_COMPLETE,


}


public class Tbl_Quest_Record : AsTableRecord
{
    QuestData m_Quest; public QuestData QuestDataInfo { get { return m_Quest; } }

    public Tbl_Quest_Record(XmlElement _element)
    {
        try
        {
            XmlNode root = (XmlElement)_element;
           
            m_Quest =  QuestData.GetQuestDataFromXml(root.ChildNodes);
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e.Message);
        }
    }
}

public class Tbl_Quest_Table : AsTableBase
{
    SortedList<int, Tbl_Quest_Record>       m_ResourceTable                 = new SortedList<int, Tbl_Quest_Record>();        // 퀘스트 ID에 따라
	Dictionary<int, PrepareOpenUIType>      m_ResourcePreOpenUI             = new Dictionary<int,PrepareOpenUIType>();
    Dictionary<int, Tbl_Quest_Record>       m_ResourceTableDaily            = new Dictionary<int,Tbl_Quest_Record>();
	Dictionary<int, List<Tbl_Quest_Record>> m_ResourceTableBySuggestNpcID   = new Dictionary<int, List<Tbl_Quest_Record>>();  // 제안하는 NPC ID
	Dictionary<int, List<Tbl_Quest_Record>> m_ResourceTableByCompleteNpcID  = new Dictionary<int, List<Tbl_Quest_Record>>();  // 수락하는 NPC ID
    Dictionary<int, List<Tbl_Quest_Record>> m_ResourceTableByNpcID          = new Dictionary<int, List<Tbl_Quest_Record>>();           // 그 NPC와 관련된 퀘스트
    List<int>                               m_ListUseItemToTargetNpcID      = new List<int>();

    public Tbl_Quest_Table(string _path)
    {
        // m_TableType = eTableType.NPC;

        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root = GetXmlRootElement(_path);

            XmlNodeList nodes = root.SelectNodes("Quest");

            foreach (XmlNode node in nodes)
            {
                Tbl_Quest_Record record = new Tbl_Quest_Record((XmlElement)node);
                if (m_ResourceTable.ContainsKey(record.QuestDataInfo.Info.ID) == false)
                    m_ResourceTable.Add(record.QuestDataInfo.Info.ID, record);
                else
                    System.Diagnostics.Trace.WriteLine("Tbl_Quest::LoadTable: There are same id(" + record.QuestDataInfo.Info.ID + ") xml node.");

                if (record.QuestDataInfo.Info.QuestType != QuestType.QUEST_DAILY && record.QuestDataInfo.Info.QuestType != QuestType.QUEST_WANTED)
                {
                    int suggestNpcID  = record.QuestDataInfo.Info.QuestSuggestNpcID;
                    int completeNpcID = record.QuestDataInfo.Info.QuestCompleteNpcID;

                    //제안하는 NPC의 ID에 따라
                    if (!m_ResourceTableBySuggestNpcID.ContainsKey(suggestNpcID))
                        m_ResourceTableBySuggestNpcID.Add(suggestNpcID, new List<Tbl_Quest_Record>());

                    m_ResourceTableBySuggestNpcID[suggestNpcID].Add(record);


                    //완료하는 NPC의 ID에 따라
                    if (!m_ResourceTableByCompleteNpcID.ContainsKey(completeNpcID))
                        m_ResourceTableByCompleteNpcID.Add(completeNpcID, new List<Tbl_Quest_Record>());

                    m_ResourceTableByCompleteNpcID[completeNpcID].Add(record);

                    //NPC ID와 관련된 quest
                    if (!m_ResourceTableByNpcID.ContainsKey(suggestNpcID))
                        m_ResourceTableByNpcID.Add(suggestNpcID, new List<Tbl_Quest_Record>());

                    if (!m_ResourceTableByNpcID.ContainsKey(completeNpcID))
                        m_ResourceTableByNpcID.Add(completeNpcID, new List<Tbl_Quest_Record>());

                    if (suggestNpcID == completeNpcID)
                    {
                        m_ResourceTableByNpcID[suggestNpcID].Add(record);
                    }
                    else
                    {
                        m_ResourceTableByNpcID[suggestNpcID].Add(record);
                        m_ResourceTableByNpcID[completeNpcID].Add(record);
                    }
                }
                else if (record.QuestDataInfo.Info.QuestType == QuestType.QUEST_DAILY)
                    m_ResourceTableDaily.Add(record.QuestDataInfo.Info.ID, record);


				// Use Item To Target
				List<AchUseItemToTarget> listUseItemToTarget = record.QuestDataInfo.Achievement.GetUseItemToTargetList();
				foreach (AchUseItemToTarget target in listUseItemToTarget)
				{
					if (!m_ListUseItemToTargetNpcID.Contains(target.targetID))
						m_ListUseItemToTargetNpcID.Add(target.targetID);
				}

				// prepare Open U.I
				List<PrepareOpenUI> listPreOpenUI = record.QuestDataInfo.Preparation.OpenUIList;
				if (listPreOpenUI.Count > 0)
					m_ResourcePreOpenUI.Add(record.QuestDataInfo.Info.ID, listPreOpenUI[0].UIType);
            }
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e);
        }
    }

    public void SetQuestRecordState(int _id, QuestProgressState _state, int _repeat)
    {
        if (m_ResourceTable.ContainsKey(_id))
        {
            m_ResourceTable[_id].QuestDataInfo.NowQuestProgressState = _state;
            
            if (_repeat != -1)
                m_ResourceTable[_id].QuestDataInfo.Info.RepeatCurrent = (byte)_repeat;
        }
        else
            Debug.Log("Not contain key (SetQuestRecordState)");
    }

    public void ResetQuestRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id))
            m_ResourceTable[_id].QuestDataInfo.Achievement.Reset();
        else
            Debug.Log("Not contain key (ResetQuestRecord)");
    }

    public Tbl_Quest_Record GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
        {
            return m_ResourceTable[_id];
        }

        return null;
    }

    public Tbl_Quest_Record[] GetRecordAll()
    {
        List<Tbl_Quest_Record> list = new List<Tbl_Quest_Record>();
        foreach (Tbl_Quest_Record record in m_ResourceTable.Values)
            list.Add(record);

        return list.ToArray();
    }

    public Tbl_Quest_Record[] GetDailyRecords()
    {
        List<Tbl_Quest_Record> list = new List<Tbl_Quest_Record>();
        foreach (Tbl_Quest_Record record in m_ResourceTableDaily.Values)
            list.Add(record);

        return list.ToArray();
    }

    public void ResetCompleteDailyRecords()
    {
        foreach (Tbl_Quest_Record record in m_ResourceTableDaily.Values)
        {
            if (record.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_COMPLETE)
            {
                record.QuestDataInfo.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
                record.QuestDataInfo.Achievement.Reset();
            }
        }
    }

    public Tbl_Quest_Record[] GetRecordByID(int _npcTableID, QuestTableType _tableType)
    {
        if (_tableType == QuestTableType.QUEST_TABLE_SUGGEST)
        {
            if (m_ResourceTableBySuggestNpcID.ContainsKey(_npcTableID))
                return m_ResourceTableBySuggestNpcID[_npcTableID].ToArray();
            else
                return null;
        }
        else
        {
            if (m_ResourceTableByCompleteNpcID.ContainsKey(_npcTableID))
                return m_ResourceTableByCompleteNpcID[_npcTableID].ToArray();
            else
                return null;
        }
    }

    public Tbl_Quest_Record[] GetRecordByID(int _npcTableID)
    {
        if (m_ResourceTableByNpcID.ContainsKey(_npcTableID))
            return m_ResourceTableByNpcID[_npcTableID].ToArray();
        else
            return null;
    }

    public void ResetQuests()
    {
        foreach (Tbl_Quest_Record record in m_ResourceTable.Values)
        {
            record.QuestDataInfo.NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
            record.QuestDataInfo.Info.RepeatCurrent = 0;
            record.QuestDataInfo.Achievement.Reset();
        }
    }

    public bool IsContainUseItemToTargetID(int _npcTableID)
    {
        return m_ListUseItemToTargetNpcID.Contains(_npcTableID);
    }

	public Dictionary<int, PrepareOpenUIType> GetPrepareOpenUI(eCLASS _eClass)
	{
		Dictionary<int, PrepareOpenUIType> dicOpen = new Dictionary<int, PrepareOpenUIType>();

		foreach (KeyValuePair<int, PrepareOpenUIType> pair in m_ResourcePreOpenUI)
		{
			Tbl_Quest_Record tblQuest = GetRecord(pair.Key);
			List < ConditionClass> listCondClass = tblQuest.QuestDataInfo.Condition.GetCondition<ConditionClass>();

			if (listCondClass.Count <= 0)
			{
				dicOpen.Add(pair.Key, pair.Value);
			}
			else
			{
				foreach (ConditionClass cond in listCondClass)
					if (cond.ClassType == _eClass)
						dicOpen.Add(pair.Key, pair.Value);
			}
		}

		return dicOpen;
	}
}


