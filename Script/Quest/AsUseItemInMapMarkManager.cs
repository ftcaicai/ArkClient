using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;

public class AsUseItemInMapMarkManager : MonoBehaviour {

    public  Dictionary<int, Dictionary<int, GameObject>>  dicUseItemInMapMark = new Dictionary<int, Dictionary<int, GameObject>>();
    public  Dictionary<GameObject, QuestUseItemInMapMark> dicUesItemInMapMarkController = new Dictionary<GameObject,QuestUseItemInMapMark>();
    private GameObject objModel                            = null;
    private static AsUseItemInMapMarkManager s_Instance = null;
    #region -Instance-
    public static AsUseItemInMapMarkManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(AsUseItemInMapMarkManager)) as AsUseItemInMapMarkManager;
            }

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AsUseItemInMapMarkManager");
                s_Instance = obj.AddComponent(typeof(AsUseItemInMapMarkManager)) as AsUseItemInMapMarkManager;
                Debug.Log("Could not locate an AsUseItemInMapTriggerManager object.\n AsUseItemInMapTriggerManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }
    #endregion

    // Use this for initialization
	void LoadModel() 
    {
		if( null == objModel)
            objModel = ResourceLoad.LoadGameObject("UI/Optimization/Prefab/GUI_Balloon_QuestItem");
	}

    public void AllocateUseItemInMapMark(int _questID, int _intoCount, AchUseItemInMap _useItemInMap)
    {
        LoadModel();

        // find Y
        Ray ray = new Ray(new Vector3(_useItemInMap.MapLocationX, 100.0f, _useItemInMap.MapLocationZ), Vector3.down);
        RaycastHit raycasHit;
        float fFindY = 0.0f;
        if (Physics.Raycast(ray, out raycasHit, 1000.0f, 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Default")))
            fFindY = raycasHit.point.y;

        GameObject objMark         = GameObject.Instantiate(objModel) as GameObject;
        objMark.transform.parent   = transform;

        GameObject itemIcon = AsStore.GetItemIcon(_useItemInMap.ItemID.ToString(), 1);

        UISlotItem slotItem = itemIcon.GetComponent<UISlotItem>();


        if (slotItem != null)
        {
            GameObject.DestroyImmediate(slotItem.coolTime.gameObject);
            slotItem.coolTime = null;
        }

        slotItem.iconImg.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
        itemIcon.transform.parent = objMark.transform.transform;
        itemIcon.transform.localPosition = Vector3.zero;

        QuestUseItemInMapMark useItemInMapMark = objMark.AddComponent<QuestUseItemInMapMark>();

        useItemInMapMark.itemID          = _useItemInMap.ItemID;
		useItemInMapMark.mapInfo         =  new AchMapInto(_useItemInMap.AssignedQuestID, _useItemInMap.MapID, _useItemInMap.MapLocationX,
										                   0.0f, _useItemInMap.MapLocationZ);
        useItemInMapMark.collider        = slotItem.iconImg.gameObject.AddComponent<BoxCollider>();
        useItemInMapMark.vPos3D          = new Vector3(_useItemInMap.MapLocationX, fFindY, _useItemInMap.MapLocationZ);
        useItemInMapMark.UpdatePos();

        #region -old-
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = useItemInMapMark.vPos3D;

        //QuestIntoMapTrigger trigger = objTrigger.AddComponent<QuestIntoMapTrigger>();
        //trigger.questTableID        = _questID;
        //trigger.radius              = _useItemInMap.MapLocationRadius;

        //SphereCollider collider = objTrigger.GetComponent<SphereCollider>();

        //if (collider == null)
        //    collider = objTrigger.AddComponent<SphereCollider>();

        //collider.radius    = _useItemInMap.MapLocationRadius / objTrigger.transform.localScale.x;
        //collider.isTrigger = true;
        #endregion

        // add
        if (dicUseItemInMapMark.ContainsKey(_questID))
            dicUseItemInMapMark[_questID].Add(_useItemInMap.AchievementNum, objMark);
        else
        {
            dicUseItemInMapMark.Add(_questID, new Dictionary<int, GameObject>());
            dicUseItemInMapMark[_questID].Add(_useItemInMap.AchievementNum, objMark);
        }

        if (!dicUesItemInMapMarkController.ContainsKey(objMark))
            dicUesItemInMapMarkController.Add(objMark, useItemInMapMark);
    }

    public void UpdateUseItemInMapMark(int _mapID)
    {
        ClearMark();

        List<ArkQuest> listQuest = ArkQuestmanager.instance.GetQuests();

        foreach (ArkQuest quest in listQuest)
        {
            if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
            {
                QuestData questData        = quest.GetQuestData();
                int questID                = questData.Info.ID;
                List<AchUseItemInMap> list = questData.Achievement.GetDatas<AchUseItemInMap>();

                // 오브젝트 생성
                foreach (AchUseItemInMap useItem in list)
                {
                    if (useItem.MapID == _mapID && !useItem.IsComplete)
                        AllocateUseItemInMapMark(questID, useItem.AchievementNum, useItem);
                }
            }
        }
    }

    public void DeleteMark(int _questID, int _achievementIdx = -1)
    {
        if (dicUseItemInMapMark.ContainsKey(_questID))
        {
            if (_achievementIdx == -1)
            {
                foreach (GameObject mark in dicUseItemInMapMark[_questID].Values)
                {
                    if (dicUesItemInMapMarkController.ContainsKey(mark))
                        dicUesItemInMapMarkController.Remove(mark);

                    GameObject.DestroyImmediate(mark);
                }
                dicUseItemInMapMark.Remove(_questID);
            }
            else
            {
                if (dicUseItemInMapMark[_questID].ContainsKey(_achievementIdx))
                {
                    GameObject objMark = dicUseItemInMapMark[_questID][_achievementIdx];

                    if (dicUesItemInMapMarkController.ContainsKey(objMark))
                        dicUesItemInMapMarkController.Remove(objMark);

                    GameObject.DestroyImmediate(objMark);
                    dicUseItemInMapMark[_questID].Remove(_achievementIdx);
                }
            }
        }
    }

    public void ClearMark()
    {
        foreach (Dictionary<int, GameObject> dicMark in dicUseItemInMapMark.Values)
        {
            foreach(GameObject obj in dicMark.Values)
                GameObject.DestroyImmediate(obj);
        }
      
        dicUseItemInMapMark.Clear();
        dicUesItemInMapMarkController.Clear();
    }

    public void ProcessDoubleTap(Ray _ray)
    {
        foreach (QuestUseItemInMapMark useItemController in dicUesItemInMapMarkController.Values)
        {
            if (useItemController.UseItem(_ray))
                break;
        }
    }

	public List<AchMapInto> GetAchUseItemInMapList(int _mapID)
	{
		List<AchMapInto> result = new List<AchMapInto>();

		foreach (QuestUseItemInMapMark mark in dicUesItemInMapMarkController.Values)
		{
			if (mark.mapInfo.MapID == _mapID)
				result.Add(mark.mapInfo);
		}

		return result;
	}
}

