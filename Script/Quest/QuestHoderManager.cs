using UnityEngine;
using System.Collections.Generic;

public class QuestHolderManager : MonoBehaviour 
{

    private static QuestHolderManager s_Instance = null;
    public Dictionary<int, QuestHolder> dicQuestHolder = new Dictionary<int, QuestHolder>();

    #region -intance-
    public static QuestHolderManager instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = FindObjectOfType(typeof(QuestHolderManager)) as QuestHolderManager;

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("QuestHolderManager");
                s_Instance = obj.AddComponent(typeof(QuestHolderManager)) as QuestHolderManager;

                Debug.Log("Could not locate an QuestHolderManager object.\n QuestHolderManager was Generated Automaticly.");
            }
            return s_Instance;
        }
    }
    #endregion

    public void MakeQuestHolder(int _mapID)
    {
        RemoveQuestHolder();

        Tbl_ZoneMap_Record zoneMapRecord = AsTableManager.Instance.GetZoneMapRecord(_mapID);

        if (zoneMapRecord == null)
            return;

        List<Tbl_ZoneMap_Record.NpcData> listNpc = zoneMapRecord.getNpcList;

        foreach (Tbl_ZoneMap_Record.NpcData npc in listNpc)
        {
            int npcID = npc.iNpcId;

            if (!dicQuestHolder.ContainsKey(npcID))
            {
                QuestHolder questHolder = MakeQuestHolderObject(npcID);
                questHolder.SettingQuest(npcID);
                dicQuestHolder.Add(npcID, questHolder);
            }
        }
    }

    QuestHolder MakeQuestHolderObject(int _npcID)
    {
        GameObject objQuestHolder = new GameObject(_npcID.ToString());
        objQuestHolder.transform.parent = transform;
        QuestHolder questHolder = objQuestHolder.AddComponent<QuestHolder>();
        return questHolder;
    }

    // for spawn npc
    public void MakeQuestHolderNpc(int _npcID)
    {
        if (dicQuestHolder.ContainsKey(_npcID))
        {
            dicQuestHolder[_npcID].SettingQuest(_npcID);
        }
        else
        {
            QuestHolder newQuestHolder = MakeQuestHolderObject(_npcID);
            newQuestHolder.SettingQuest(_npcID);
            dicQuestHolder.Add(_npcID, newQuestHolder);
        }
    }

    public void RemoveQuestHolder()
    {
        foreach (QuestHolder holder in dicQuestHolder.Values)
            GameObject.DestroyImmediate(holder.gameObject);

        dicQuestHolder.Clear();
    }

    public void UpdateQuestHolder()
    {
		foreach (int npcID in dicQuestHolder.Keys)
		{
			dicQuestHolder[npcID].UpdateForNpcDaily();
			dicQuestHolder[npcID].UpdateQuestMark(npcID);
		}
    }

    public void UpdateQuestHolder(int _npcID)
    {
		if (dicQuestHolder.ContainsKey(_npcID))
		{
			dicQuestHolder[_npcID].UpdateForNpcDaily();
			dicQuestHolder[_npcID].UpdateQuestMark(_npcID);
		}
	}

    public QuestHolder GetQuestHolder(int _npcID)
    {
        if (dicQuestHolder.ContainsKey(_npcID))
            return dicQuestHolder[_npcID];
        else
            return null;
    }

	void Update()
	{

	}
}
