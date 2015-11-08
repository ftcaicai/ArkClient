using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;

public class AsIntoMapTriggerManager : MonoBehaviour {

    public  Dictionary<int, Dictionary<int, GameObject>> dicIntoMapTriger = new Dictionary<int, Dictionary<int, GameObject>>();
	public  Dictionary<int, Dictionary<int, AchMapInto>> dicIntoMapInfo   = new Dictionary<int, Dictionary<int,AchMapInto>>();
    private GameObject objModel                           = null;
    private static AsIntoMapTriggerManager s_Instance     = null;
    #region -Instance-
    public static AsIntoMapTriggerManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(AsIntoMapTriggerManager)) as AsIntoMapTriggerManager;
            }

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AsIntoMapTrigerManager");
                s_Instance = obj.AddComponent(typeof(AsIntoMapTriggerManager)) as AsIntoMapTriggerManager;
                Debug.Log("Could not locate an AsIntoMapTrigerManager object.\n AsIntoMapTrigerManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }
    #endregion

    // Use this for initialization
	void LoadModel() 
    {
//        if (objModel == null)
//            objModel = ResourceLoad.LoadGameObject("Character/Npc/Object/Question_Blue") as GameObject;
		if( null == objModel)
			objModel = ResourceLoad.LoadGameObject( "UseScript/Object/Question_Blue");
	}

    public void AllocateIntoMapTriger(int _questID, int _intoCount, AchMapInto _mapInto)
    {
        LoadModel();

        GameObject objTrigger         = GameObject.Instantiate(objModel) as GameObject;
        objTrigger.transform.position = new Vector3(_mapInto.MapLocationX, _mapInto.MapLocationY, _mapInto.MapLocationZ);
        objTrigger.transform.parent   = transform;
        objTrigger.layer              = LayerMask.NameToLayer("Hide");

        QuestIntoMapTrigger trigger = objTrigger.AddComponent<QuestIntoMapTrigger>();
        trigger.questTableID        = _questID;
		trigger.mapInfo             = _mapInto;
        trigger.radius              = _mapInto.MapLocationRadius;

        SphereCollider collider = objTrigger.GetComponent<SphereCollider>();//objTrigger.AddComponent<SphereCollider>();

        if (collider == null)
            collider = objTrigger.AddComponent<SphereCollider>();

        collider.radius    = _mapInto.MapLocationRadius / objTrigger.transform.localScale.x;
        collider.isTrigger = true;

        // add
		if (dicIntoMapTriger.ContainsKey(_questID))
		{
			dicIntoMapTriger[_questID].Add(_mapInto.AchievementNum, objTrigger);
			dicIntoMapInfo[_questID].Add(_mapInto.AchievementNum, _mapInto);
		}
		else
		{
			dicIntoMapTriger.Add(_questID, new Dictionary<int, GameObject>());
			dicIntoMapInfo.Add(_questID, new Dictionary<int, AchMapInto>());
			dicIntoMapTriger[_questID].Add(_mapInto.AchievementNum, objTrigger);
			dicIntoMapInfo[_questID].Add(_mapInto.AchievementNum, _mapInto);
		}
    }

    public void UpdateIntoMapTrigger(int _mapID)
    {
        ClearTrigger();

        List<ArkQuest> listQuest = ArkQuestmanager.instance.GetQuests();

        foreach (ArkQuest quest in listQuest)
        {
            if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
            {
                QuestData questData   = quest.GetQuestData();
                int questID           = questData.Info.ID;
                List<AchMapInto> list = questData.Achievement.GetDatas<AchMapInto>();

                // 오브젝트 생성
                foreach (AchMapInto mapInto in list)
                {
                    if (mapInto.MapID == _mapID && !mapInto.IsComplete)
                        AllocateIntoMapTriger(questID, mapInto.AchievementNum, mapInto);
                }
            }
        }
    }

    public void DeleteTrigger(int _questID, int _achievementIdx = -1)
    {
        if (dicIntoMapTriger.ContainsKey(_questID))
        {
            if (_achievementIdx == -1)
            {
                foreach (GameObject mark in dicIntoMapTriger[_questID].Values)
                    GameObject.DestroyImmediate(mark);

                dicIntoMapTriger.Remove(_questID);
				dicIntoMapInfo.Remove(_questID);
            }
            else
            {
                if (dicIntoMapTriger[_questID].ContainsKey(_achievementIdx))
                {
                    GameObject.DestroyImmediate(dicIntoMapTriger[_questID][_achievementIdx]);
                    dicIntoMapTriger[_questID].Remove(_achievementIdx);
					dicIntoMapInfo[_questID].Remove(_achievementIdx);
                }
            }
        }
    }

    public void ClearTrigger()
    {
        foreach (Dictionary<int, GameObject> dicMark in dicIntoMapTriger.Values)
        {
            foreach(GameObject obj in dicMark.Values)
                GameObject.DestroyImmediate(obj);
        }

		dicIntoMapInfo.Clear();
        dicIntoMapTriger.Clear();
    }

	public List<AchMapInto> GetAchUseItemInMapList(int _mapID)
	{
		List<AchMapInto> result = new List<AchMapInto>();

		foreach (Dictionary<int, AchMapInto> dic in dicIntoMapInfo.Values)
		{
			foreach (AchMapInto mapinto in dic.Values)
			{
				if (mapinto.MapID == _mapID)
					result.Add(mapinto);
			}
		}

		return result;
	}
}
