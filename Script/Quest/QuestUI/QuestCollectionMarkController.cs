using UnityEngine;
using System.Collections.Generic;

public class QuestCollectionMarkController : MonoBehaviour {

    private GameObject markObj;
    private bool       visible = false;
    private int        npcID;
    private AsPanel_Name panelName = null;

    public  bool       Visible 
    {
        set
        {
            if (markObj == null)
                return;

            visible = value;

            markObj.SetActiveRecursively(value);
        }
        get {  return visible;  }
    }

    public void Init(int _npcID, AsPanel_Name _namePanel, AsBaseEntity _entity)
    {
        npcID     = _npcID;
        panelName = _namePanel;

        Transform tmDummyTop = ResourceLoad.SearchHierarchyTransform(transform, "DummyLeadTop");

        markObj = ResourceLoad.CreateGameObject("UseScript/Object/Exclamation_Blue");

        if (markObj != null)
        {
            if (tmDummyTop != null)
            {
                markObj.transform.position = tmDummyTop.transform.position;
                markObj.transform.parent = transform;
            }
            else
			{
				if( true == _entity.isKeepDummyObj)
				{
					Vector3 vPos = _entity.transform.position;
					vPos.y += _entity.characterController.height;
					markObj.transform.position = vPos;
					markObj.transform.parent = _entity.transform;
				}
				else
					Debug.Log("dummy is null");
			}

            markObj.SetActiveRecursively(false);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void UpdateQuestCollectionMark()
    {
        List<ArkQuest> listQuest = ArkQuestmanager.instance.GetQuestHaveGetQuestCollectionAch(npcID);

        foreach (ArkQuest quest in listQuest)
        {
            if (quest == null)
            {
                Debug.Log("quest is null (in update Quest CollectionMark)");
                continue;
            }

            if (quest.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
            {
                //panelName.gameObject.SetActiveRecursively(true);
				panelName.NameText.Hide( false);
                Visible = true;
                return;
            }
        }

        //panelName.gameObject.SetActiveRecursively(false);
		panelName.NameText.Hide( true);
        Visible = false;
    }
}
