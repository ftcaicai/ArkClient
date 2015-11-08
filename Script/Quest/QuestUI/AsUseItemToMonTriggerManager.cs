using UnityEngine;
using System.Collections.Generic;

public class UseItemToMonTriggerInfo
{
	public int mapID = 0;
	public Dictionary<int, List<AchUseItemToTarget>> dicUseItemToTarget = new Dictionary<int, List<AchUseItemToTarget>>();
	public Dictionary<int, UISlotItem>               dicUseItemToMonTrigger = new Dictionary<int,UISlotItem>();
	public Dictionary<int, CoolTimeGroup>            dicUseItemTOMonCoolTimeGroup = new Dictionary<int,CoolTimeGroup>();
	public Dictionary<int, BoxCollider>              dicUseItemToMonTriggerCollider = new Dictionary<int,BoxCollider>();

	public UseItemToMonTriggerInfo(int _MapID)
	{
		mapID = _MapID;
	}

	public bool HaveThisItem(int _itemID)
	{
		return dicUseItemToTarget.ContainsKey(_itemID);
	}

	public void AddUseItemToMon(AchUseItemToTarget _useItemToMon)
	{
		int itemID = _useItemToMon.ItemID;

		if (!dicUseItemToTarget.ContainsKey(itemID))
			dicUseItemToTarget.Add(itemID, new List<AchUseItemToTarget>());
		
		dicUseItemToTarget[itemID].Add(_useItemToMon);
	}

	public int CreateTrigger()
	{
		int count = 0;
		foreach (int itemID in dicUseItemToTarget.Keys)
		{
			if (dicUseItemToMonTrigger.ContainsKey(itemID))
				continue;

			if (count >= AsUseItemToMonTriggerManager.MaxCount)
				return count;

			// create icon
			GameObject itemIcon = AsStore.GetItemIcon(itemID.ToString(), 1);
			itemIcon.transform.parent = AsHudDlgMgr.Instance.useItemToMonTriggerDummy.transform;
			itemIcon.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

			// create effect
			GameObject objEffect      = ResourceLoad.CreateUI("UI/AsGUI/GUI_EquipEffect", itemIcon.transform, Vector3.zero);
			AsEquipEffect equipEffect = objEffect.GetComponentInChildren<AsEquipEffect>();
			equipEffect.followTM      = itemIcon.transform;

			UISlotItem slotItem = itemIcon.GetComponent<UISlotItem>();
			slotItem.slotType = UISlotItem.eSLOT_TYPE.USE_COOLTIME;
			slotItem.ShowCoolTime(false);
			slotItem.coolTime.AlignScreenPos();

			Item item = ItemMgr.ItemManagement.GetItem(itemID);

			if (item != null)
			{
				CoolTimeGroup coolGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup(item.ItemData.itemSkill, item.ItemData.itemSkillLevel);

				if (coolGroup != null)
				{
					if (!dicUseItemTOMonCoolTimeGroup.ContainsKey(itemID))
						dicUseItemTOMonCoolTimeGroup.Add(itemID, coolGroup);
				}
			}

			dicUseItemToMonTriggerCollider.Add(itemID, slotItem.iconImg.gameObject.AddComponent<BoxCollider>());

			dicUseItemToMonTrigger.Add(itemID, slotItem);

			count++;

			AsUseItemToMonTriggerManager.instance.UpdateUseItemToTarget(itemID);

			Debug.LogWarning("Create Item Trigger = " + itemID.ToString());
		}

		return count;
	}

	public bool IsCompleteAchievement()
	{
		foreach (List<AchUseItemToTarget> list in dicUseItemToTarget.Values)
		{
			foreach (AchUseItemToTarget target in list)
			{
				if (target.IsComplete == false)
					return false;
			}
		}

		return true;
	}

	public void Clean()
	{
		foreach (UISlotItem trigger in dicUseItemToMonTrigger.Values)
			GameObject.DestroyImmediate(trigger.gameObject);

		dicUseItemToMonTrigger.Clear();
		dicUseItemToTarget.Clear();
		dicUseItemToMonTriggerCollider.Clear();
		dicUseItemTOMonCoolTimeGroup.Clear();
	}

	public bool UseItem(Ray _ray)
	{
		foreach (KeyValuePair<int, BoxCollider> pair in dicUseItemToMonTriggerCollider)
		{
			if (AsUtil.PtInCollider(pair.Value, _ray))
			{

				RealItem item = ItemMgr.HadItemManagement.Inven.GetRealItem(pair.Key);

				if (item != null)
				{
					if (item.IsCanCoolTimeActive() == false)
					{
						AsCommonSender.SendUseItem(item.getSlot);
						return true;
					}
				}
				else
				{
					string msg = AsTableManager.Instance.GetTbl_String(954);
					AsChatManager.Instance.InsertChat(msg, eCHATTYPE.eCHATTYPE_SYSTEM);
					AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(msg, true);
				}
			}
		}

		return false;
	}

	public void UpdateCoolTime()
	{
		foreach (KeyValuePair<int, UISlotItem> pairSlotItem in dicUseItemToMonTrigger)
		{
			if (dicUseItemTOMonCoolTimeGroup.ContainsKey(pairSlotItem.Key))
			{
				CoolTimeGroup coolGroup = dicUseItemTOMonCoolTimeGroup[pairSlotItem.Key];

				if (coolGroup.isCoolTimeActive)
				{
					pairSlotItem.Value.SetCooolTimeValue(coolGroup.getCoolTimeValue);
					pairSlotItem.Value.coolTime.AlignScreenPos();
				}
				else
					pairSlotItem.Value.ShowCoolTime(false);
			}
		}
	}
}

public class AsUseItemToMonTriggerManager : MonoBehaviour {

	private SortedDictionary<int, UseItemToMonTriggerInfo> dicUseItemToMonTriggerInfo = new SortedDictionary<int, UseItemToMonTriggerInfo>();
	public  int   itemCount = 0;
	public  float posZ      = 5.0f;
	public static int MaxCount = 2;
	#region -Instance-
	private static AsUseItemToMonTriggerManager s_Instance = null;

	public static AsUseItemToMonTriggerManager instance
	{
		get
		{
			if (s_Instance == null)
				s_Instance = FindObjectOfType(typeof(AsUseItemToMonTriggerManager)) as AsUseItemToMonTriggerManager;

			if (s_Instance == null)
			{
				GameObject obj = new GameObject("AsUseItemToMonTriggerManager");
				s_Instance = obj.AddComponent(typeof(AsUseItemToMonTriggerManager)) as AsUseItemToMonTriggerManager;
				Debug.Log("Could not locate an AsUseItemToMonTriggerManager object.\n AsUseItemToMonTriggerManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}
	#endregion

	void Awake() 
	{
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Add trigger
	/// </summary>
	/// <param name="_questData"></param>
	public void AddUseItemToMonTrigger(ArkSphereQuestTool.QuestData _questData)
	{
		if (_questData.NowQuestProgressState != QuestProgressState.QUEST_PROGRESS_IN)
			return;

		List<AchUseItemToTarget> listUseItemToTarget = _questData.Achievement.GetDatas<AchUseItemToTarget>();
		int questID = _questData.Info.ID;

		// item group
		foreach (AchUseItemToTarget useItemToTarget in listUseItemToTarget)
		{
			if (useItemToTarget.targetType != UseItemToTargetType.MONSTER)
				continue;

			// not contain
			if (!dicUseItemToMonTriggerInfo.ContainsKey(questID))
				dicUseItemToMonTriggerInfo.Add(questID, new UseItemToMonTriggerInfo(questID / 10000));

			dicUseItemToMonTriggerInfo[questID].AddUseItemToMon(useItemToTarget);
		}

		if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
			CreateAllTrigger();
			RepoisitionTrigger();
		}
	}


	/// <summary>
	/// Remove trigger
	/// </summary>
	/// <param name="_questID"></param>
	public void RemoveUseItemToMonTrigger(int _questID)
	{
		if (dicUseItemToMonTriggerInfo.ContainsKey(_questID))
		{
			dicUseItemToMonTriggerInfo[_questID].Clean();
			dicUseItemToMonTriggerInfo.Remove(_questID);
		}

		RepoisitionTrigger();

		itemCount = dicUseItemToMonTriggerInfo.Count;
	}

	public void RemaveUseItemToMonTriggerAll()
	{
		foreach (UseItemToMonTriggerInfo info in dicUseItemToMonTriggerInfo.Values)
			info.Clean();

		dicUseItemToMonTriggerInfo.Clear();

		itemCount = 0;
	}

	public void CreateAllTrigger()
	{
		Map currentMap = TerrainMgr.Instance.GetCurrentMap();

		// exception indum
		if (currentMap.MapData.getMapType == eMAP_TYPE.Indun)
			return;

		int count = 0;
		foreach (UseItemToMonTriggerInfo info in dicUseItemToMonTriggerInfo.Values)
		{
			if (count >= MaxCount)
				break;

			count += info.CreateTrigger();
		}

		itemCount = count;
	}

	/// <summary>
	/// Update Trigger and RepositionTrigger
	/// </summary>
	public bool CheckCompleteTrigger()
	{
		bool remove = false;
		List<int> removeList = new List<int>();
		// check
		foreach (KeyValuePair<int, UseItemToMonTriggerInfo> keyPair in dicUseItemToMonTriggerInfo)
		{
			if (keyPair.Value.IsCompleteAchievement() == true)
			{
				removeList.Add(keyPair.Key);
				keyPair.Value.Clean();
				remove = true;
			}
		}

		foreach (int idx in removeList)
			dicUseItemToMonTriggerInfo.Remove(idx);

		if (remove == true)
			RepoisitionTrigger();

		return remove;
	}

	public void RepoisitionTrigger()
	{
		// 남은 아이콘 소팅 코드
		float fAddPos = 0.0f;
		float fOffset = 0.4f;
		int count = 0;

		List<int> listNowMapQuest = new List<int>();

		#region exception indun
		int nowMapID = TerrainMgr.Instance.GetCurMapID();

		Map currentMap = TerrainMgr.Instance.GetCurrentMap();

		if (currentMap.MapData.getMapType == eMAP_TYPE.Indun)
		{
			foreach (KeyValuePair<int, UseItemToMonTriggerInfo> UseItemToMonPair in dicUseItemToMonTriggerInfo)
			{
				foreach (UISlotItem trigger in UseItemToMonPair.Value.dicUseItemToMonTrigger.Values)
					trigger.gameObject.SetActive(false);
			}

			return;
		}
		#endregion


		// now map
		foreach (KeyValuePair<int, UseItemToMonTriggerInfo> UseItemToMonPair in dicUseItemToMonTriggerInfo)
		{
			if (UseItemToMonPair.Value.mapID != nowMapID)
				continue;

			listNowMapQuest.Add(UseItemToMonPair.Key);

			foreach (UISlotItem trigger in UseItemToMonPair.Value.dicUseItemToMonTrigger.Values)
			{
				if (count >= MaxCount)
				{
					trigger.gameObject.SetActiveRecursively(false);
				}
				else
				{
					if (trigger.gameObject.active == false)
						trigger.gameObject.SetActiveRecursively(true);

					trigger.transform.localPosition = new Vector3(0.0f, fAddPos, posZ);
					fAddPos -= (trigger.iconImg.height + fOffset);
				}

				count++;
			}
		}


		foreach (KeyValuePair<int, UseItemToMonTriggerInfo> UseItemToMonPair in dicUseItemToMonTriggerInfo)
		{
			if (listNowMapQuest.Contains(UseItemToMonPair.Key))
				continue;

			foreach (UISlotItem trigger in UseItemToMonPair.Value.dicUseItemToMonTrigger.Values)
			{
				if (count >= MaxCount)
				{
					trigger.gameObject.SetActiveRecursively(false);
				}
				else
				{
					if (trigger.gameObject.active == false)
						trigger.gameObject.SetActiveRecursively(true);

					trigger.transform.localPosition = new Vector3(0.0f, fAddPos, posZ);
					fAddPos -= (trigger.iconImg.height + fOffset);
				}

				count++;
			}
		}
	}

	void Update()
	{
		foreach (UseItemToMonTriggerInfo info in dicUseItemToMonTriggerInfo.Values)
			info.UpdateCoolTime();
	}

	public void ProcessDoubleTap(Ray _ray)
	{
		foreach (UseItemToMonTriggerInfo info in dicUseItemToMonTriggerInfo.Values)
		{
			if (info.UseItem(_ray) == true)
				break;
		}
	}


	public void UpdateUseItemToTarget(int _itemID)
	{
		if (!ArkQuestmanager.instance.HaveUseItemToTarget(_itemID))
			return;

		UseItemToMonTriggerInfo findInfo = null;
		foreach (UseItemToMonTriggerInfo triggerInfo in dicUseItemToMonTriggerInfo.Values)
		{
			if (triggerInfo.HaveThisItem(_itemID))
			{
				findInfo = triggerInfo;
				break;
			}
		}

		if (findInfo == null)
			return;

		int itemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(_itemID);

		if (itemCount <= 0)
			findInfo.dicUseItemToMonTrigger[_itemID].iconImg.renderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1.0f));
		else
			findInfo.dicUseItemToMonTrigger[_itemID].iconImg.renderer.material.SetColor("_Color", Color.white);
	}

}
