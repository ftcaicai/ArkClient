using UnityEngine;
using System.Collections.Generic;

public class AsUseItemToTargetPanelManager : MonoBehaviour
{
    private GameObject objModel = null;
	// begin kij
	//private CoolTimeGroup m_CoolTimeGroup;
	//private UISlotItem m_SlotItem;
	//end kij
	
    private static AsUseItemToTargetPanelManager s_Instance = null;
    public Dictionary<AsBaseEntity, AsPanel_UseItemToTarget> dicUseItemToTargetPanel = new Dictionary<AsBaseEntity, AsPanel_UseItemToTarget>();
    #region -Instance-
    public static AsUseItemToTargetPanelManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(AsUseItemToTargetPanelManager)) as AsUseItemToTargetPanelManager;
            }

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AsUseItemToTargetPanelManager");
                s_Instance = obj.AddComponent(typeof(AsUseItemToTargetPanelManager)) as AsUseItemToTargetPanelManager;
                Debug.Log("Could not locate an UseItemToTargetPanelManager object.\n UseItemToTargetPanelManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }
    #endregion

    void LoadModel()
    {
        if (null == objModel)
            objModel = ResourceLoad.LoadGameObject("UI/Optimization/Prefab/GUI_Balloon_QuestItem");
    }

    public void CreatePanel(AsBaseEntity _entity)
    {
        int npcTableID = _entity.GetProperty<int>(eComponentProperty.NPC_ID);

        if (dicUseItemToTargetPanel.ContainsKey(_entity))
            return;

        if (AsTableManager.Instance.IsUseItemToTargetEntity(npcTableID))
        {
            AchUseItemToTarget useItemToTargetAch = ArkQuestmanager.instance.FindUseItemToTargetAchievement(npcTableID);

            if (useItemToTargetAch == null)
                return;

            if (ArkQuestmanager.instance.IsClearQuest(useItemToTargetAch.AssignedQuestID))
                return;

            if (useItemToTargetAch.IsComplete == true)
                return;

            LoadModel();

            // test
            string npcName = _entity.GetProperty<string>(eComponentProperty.NAME);

            Debug.LogWarning(npcName + " is Use Item To Target");

            // make model
            GameObject objMark = GameObject.Instantiate(objModel) as GameObject;
            objMark.transform.parent = transform;
			
			//begin kij 			
			Item _item = ItemMgr.ItemManagement.GetItem( useItemToTargetAch.ItemID );
			if( null == _item )
				return;
			
			CoolTimeGroup coolTimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( _item.ItemData.itemSkill, _item.ItemData.itemSkillLevel );
			if( null == coolTimeGroup )
				return;		
			
			// end kij
            GameObject itemIcon = AsStore.GetItemIcon(useItemToTargetAch.ItemID.ToString(), 1);
		
            UISlotItem slotItem = itemIcon.GetComponent<UISlotItem>();
			
            BoxCollider itemCollider  = slotItem.iconImg.gameObject.AddComponent<BoxCollider>();
            itemIcon.transform.parent = objMark.transform.transform;
            itemIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);

            //begin kij 
			slotItem.slotType = UISlotItem.eSLOT_TYPE.USE_COOLTIME;
			slotItem.ShowCoolTime(false);
			slotItem.coolTime.AlignScreenPos();
            //end kij

            AsPanel_UseItemToTarget useItemToTargetPanel = objMark.AddComponent<AsPanel_UseItemToTarget>();

			useItemToTargetPanel.Initilize(_entity, itemCollider, _entity.namePanel.NameText.BaseHeight + 2.5f, useItemToTargetAch, slotItem, coolTimeGroup);

            if (!dicUseItemToTargetPanel.ContainsKey(_entity))
                dicUseItemToTargetPanel.Add(_entity, useItemToTargetPanel);

			UpdateUseItemToTargetPanel(useItemToTargetAch.ItemID);
        }
    }

    public void DeletePanel(AsBaseEntity _entity)
    {
        if (dicUseItemToTargetPanel.ContainsKey(_entity))
        {
            Debug.LogWarning(_entity.GetProperty<string>(eComponentProperty.NAME) + "use item to panel is delete");

            dicUseItemToTargetPanel.Remove(_entity);
        }
    }

    public void DeletePanel(int _questID)
    {
        List<KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget>> listPair = new List<KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget>>();

        foreach (KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget> pairUseItemToTarget in dicUseItemToTargetPanel)
        {
            if (pairUseItemToTarget.Value.GetAchUseItemToTarget.AssignedQuestID == _questID)
            {
                listPair.Add(pairUseItemToTarget);
				
				if (pairUseItemToTarget.Value != null)
					if (pairUseItemToTarget.Value.gameObject != null)
						pairUseItemToTarget.Value.gameObject.SetActive(false);
            }
        }

        foreach (KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget> useItemToTargetPair in listPair)
        {
            // delete panel
			if (useItemToTargetPair.Value != null)
				if (useItemToTargetPair.Value.gameObject != null)
					GameObject.Destroy(useItemToTargetPair.Value.gameObject);

            // remove dic
			dicUseItemToTargetPanel.Remove(useItemToTargetPair.Key);

            // on quest Panel
            if (useItemToTargetPair.Key.questPanel != null)
				if(useItemToTargetPair.Key.questPanel.gameObject != null)
					useItemToTargetPair.Key.questPanel.gameObject.SetActive(true);
        }
    }

    public void DeletePanel(int _questID, int _achNum)
    {
        List<KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget>> listPair = new List<KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget>>();

        foreach (KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget> pairUseItemToTarget in dicUseItemToTargetPanel)
        {
            AchUseItemToTarget useItemToTarget = pairUseItemToTarget.Value.GetAchUseItemToTarget;

            if (useItemToTarget.AssignedQuestID == _questID && useItemToTarget.AchievementNum == _achNum)
            {
				if (pairUseItemToTarget.Value != null)
					if (pairUseItemToTarget.Value.gameObject != null)
						pairUseItemToTarget.Value.gameObject.SetActive(false);

                listPair.Add(pairUseItemToTarget);

                break;
            }
        }

        foreach (KeyValuePair<AsBaseEntity, AsPanel_UseItemToTarget> pairUseItemToTarget in listPair)
        {
            // delete panel
			if (pairUseItemToTarget.Value != null)
				if (pairUseItemToTarget.Value.gameObject != null)
					GameObject.Destroy(pairUseItemToTarget.Value.gameObject);

            dicUseItemToTargetPanel.Remove(pairUseItemToTarget.Key);

            if (pairUseItemToTarget.Key.questPanel != null)
                pairUseItemToTarget.Key.questPanel.gameObject.SetActive(true);
        }
    }

    public void ProcessDoubleTap(Ray _ray)
    {
        foreach (AsPanel_UseItemToTarget useItemToTarget in dicUseItemToTargetPanel.Values)
        {
            if (useItemToTarget.UseItem(_ray))
                break;
        }
    }

    public void UpdateUseItemToTargetPanel()
    {
        List<AsNpcEntity> listNpcEntity  = AsEntityManager.Instance.GetEntityInRange(UseItemToTargetType.NPC, AsEntityManager.Instance.UserEntity.transform.position, 0.0f, 30.0f, false);
        List<AsNpcEntity> listCollEntity = AsEntityManager.Instance.GetEntityInRange(UseItemToTargetType.COLLECTION, AsEntityManager.Instance.UserEntity.transform.position, 0.0f, 30.0f, false);

        List<AsNpcEntity> listResult = new List<AsNpcEntity>();
        listResult.AddRange(listNpcEntity);
        listResult.AddRange(listCollEntity);

        foreach (AsNpcEntity npc in listResult)
            CreatePanel(npc);
    }
	
	
	// begin kij
	void Update()
	{

		foreach (AsPanel_UseItemToTarget useItemToTarget in dicUseItemToTargetPanel.Values)
		{
			if (useItemToTarget.CoolTimeGroup != null && useItemToTarget.SlotItem != null)
			{

				if (useItemToTarget.CoolTimeGroup.isCoolTimeActive == true)
				{
					//Debug.Log("cool = " + m_CoolTimeGroup.getCoolTimeValue);
					useItemToTarget.SlotItem.SetCooolTimeValue(useItemToTarget.CoolTimeGroup.getCoolTimeValue);
					useItemToTarget.SlotItem.coolTime.AlignScreenPos();
				}
				else
				{
					useItemToTarget.SlotItem.ShowCoolTime(false);
				}
			}

		}
		//if( null != m_CoolTimeGroup && null != m_SlotItem )
		//{
		//    if (m_CoolTimeGroup.isCoolTimeActive)
		//    {
		//        //Debug.Log("cool = " + m_CoolTimeGroup.getCoolTimeValue);
		//        m_SlotItem.SetCooolTimeValue(m_CoolTimeGroup.getCoolTimeValue);
		//        m_SlotItem.coolTime.AlignScreenPos();
		//    }
		//    else
		//        m_SlotItem.ShowCoolTime(false);
		//}	
	}
	
	// end kij

	public void UpdateUseItemToTargetPanel(int _itemID)
	{
		if (!ArkQuestmanager.instance.HaveUseItemToTarget(_itemID))
			return;

		AsPanel_UseItemToTarget useItemToNpc = null;

		foreach (AsPanel_UseItemToTarget useItem in dicUseItemToTargetPanel.Values)
		{
			if (useItem.GetAchUseItemToTarget.ItemID == _itemID)
			{
				useItemToNpc = useItem;
				break;
			}
		}

		if (useItemToNpc == null)
			return;

		UISlotItem slotItem = useItemToNpc.GetComponentInChildren<UISlotItem>();

		if (slotItem == null)
			return;

		int itemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(_itemID);

		if (itemCount <= 0)
			slotItem.iconImg.renderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		else
			slotItem.iconImg.renderer.material.SetColor("_Color", Color.white);
	}
}