using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsInGameEventManager : MonoBehaviour
{
	private static AsInGameEventManager s_Instance = null;

	private Dictionary<int, AS_SC_NPC_APPEAR_2> dicNpcAppear = new Dictionary<int, AS_SC_NPC_APPEAR_2>();

	float fTime = 0.0f;
	#region -Instance-
	public static AsInGameEventManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = FindObjectOfType(typeof(AsInGameEventManager)) as AsInGameEventManager;
			}

			if (s_Instance == null)
			{
				GameObject obj = new GameObject("AsInGameEventManager");
				s_Instance = obj.AddComponent(typeof(AsInGameEventManager)) as AsInGameEventManager;
				Debug.Log("Could not locate an AsInGameEventManager object.\n AsInGameEventManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}
	#endregion

	void Start()
	{

	}

	public void ForcedUpdate()
	{
		fTime = 0.0f;
		UpdateCore();
	}

	void Update()
	{
		if (fTime >= 20.0f)
		{
			UpdateCore();
			fTime = 0.0f;

		}
		else
			fTime += Time.deltaTime;
	}

	public void AddNpcAppear(AS_SC_NPC_APPEAR_2 _npcAppear)
	{
		if (!dicNpcAppear.ContainsKey(_npcAppear.nNpcTableIdx))
			dicNpcAppear.Add(_npcAppear.nNpcTableIdx, _npcAppear);
	}

	public void DelNpcAppear(AS_SC_NPC_DISAPPEAR_2 _npcDisAppear)
	{
		if (!dicNpcAppear.ContainsKey(_npcDisAppear.nNpcIdx))
			dicNpcAppear.Remove(_npcDisAppear.nNpcIdx);
	}

	public bool CheckEventViewNpc(int _npcTableID)
	{
		return AsTableManager.Instance.CheckViewEventNpc(_npcTableID);
	}

	public bool CheckRunningEventviewNpc(int _npcTableID)
	{
		return AsTableManager.Instance.CheckViewEventNpcAppear(_npcTableID, System.DateTime.Now.AddTicks(AsGameMain.serverTickGap));
	}

	void CheckViewNpc()
	{
		// visible event npc
		List<Tbl_Event_Record> listEvents = AsTableManager.Instance.GetTbl_Event(System.DateTime.Now.AddTicks(AsGameMain.serverTickGap), true);

		foreach (Tbl_Event_Record eventRecord in listEvents)
		{
			if (dicNpcAppear.ContainsKey(eventRecord.npcID))
			{
				AS_SC_NPC_APPEAR_2 npcAppear = dicNpcAppear[eventRecord.npcID];
				dicNpcAppear.Remove(eventRecord.npcID);
				AsEntityManager.Instance.NpcAppearForEvent(npcAppear);
			}
		}

		//invisible event npc
		listEvents = AsTableManager.Instance.GetTbl_EventCanNotProgress(System.DateTime.Now.AddTicks(AsGameMain.serverTickGap), true);

		List<AsNpcEntity> listRemovingNpc = new List<AsNpcEntity>();

		foreach (Tbl_Event_Record eventRecord in listEvents)
		{
			AsNpcEntity npcEntity = AsEntityManager.Instance.GetNPCEntityByTableID(eventRecord.npcID);

			if (npcEntity != null)
				listRemovingNpc.Add(npcEntity);
		}

		foreach (AsNpcEntity npcEntity in listRemovingNpc)
			AsEntityManager.Instance.RemoveEntity(npcEntity);
	}

	public void UpdateByNpcEntity(AsNpcEntity _npcEntity)
	{
		QuestHolder questHolder = QuestHolderManager.instance.GetQuestHolder(_npcEntity.TableIdx);

		if (questHolder == null)
			return;

		List<Tbl_Event_Record> listEvent = AsTableManager.Instance.GetTbl_Event(_npcEntity.TableIdx, System.DateTime.Now.AddTicks(AsGameMain.serverTickGap));
		List<Tbl_Event_Record> listFiltered = new List<Tbl_Event_Record>();

		foreach (Tbl_Event_Record record in listFiltered)
		{
			if (record.viewList == true)
				continue;

			listFiltered.Add(record);
		}

		if (listFiltered.Count > 0)
		{
			if (questHolder.NowMarkType == QuestMarkType.NOTHING || questHolder.nowQuetMarkType == QuestMarkType.UPPERLEVEL || questHolder.nowQuetMarkType == QuestMarkType.PROGRESS)
			{
				if (questHolder.NowMarkType == QuestMarkType.PROGRESS)
				{
					questHolder.nowQuetMarkType = QuestMarkType.HAVE_EVENT_AND_PROGRESS;
					QuestHolder.SetQuestMarkPanel(_npcEntity, QuestMarkType.HAVE_EVENT_AND_PROGRESS);
				}
				else
				{
					questHolder.nowQuetMarkType = QuestMarkType.HAVE_EVENT;
					QuestHolder.SetQuestMarkPanel(_npcEntity, QuestMarkType.HAVE_EVENT);
				}
			}
		}
		else
		{
			if (questHolder.NowMarkType == QuestMarkType.HAVE_EVENT || questHolder.NowMarkType == QuestMarkType.HAVE_EVENT_AND_PROGRESS)
				questHolder.UpdateQuestMark(_npcEntity.TableIdx);
		}
	}

	void UpdateCore()
	{
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
			return;
		// check npc View
		CheckViewNpc();

		List<AsNpcEntity> listEntity = AsEntityManager.Instance.GetEntityInRange(UseItemToTargetType.NPC, AsEntityManager.Instance.UserEntity.transform.position, 0.0f, 10.0f);

        if (listEntity.Count <= 0)
            return;

        for (int i = 0; i < listEntity.Count; i++ )
            UpdateByNpcEntity(listEntity[i]);
	}

	public void Reset()
	{
		if (dicNpcAppear != null)
			dicNpcAppear.Clear();
	}
}