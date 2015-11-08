using UnityEngine;
using System.Collections;

public class QuestBookItemController : MonoBehaviour {

    public GameObject[]       questProgressIcon;
    public QuestType          questType          = QuestType.QUEST_MAIN;
    public QuestProgressState questProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
	// Use this for initialization
	void Start ()
    {
     
	
	}

    void OnEnable()
    {
        UpdateQuestStateIcon();
    }

    public void UpdateQuestStateIcon()
    {
        foreach (GameObject icon in questProgressIcon)
            icon.SetActiveRecursively(false);

        if (questType == QuestType.QUEST_DAILY)
        {
            if (questProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
                questProgressIcon[3].SetActiveRecursively(true);
            else if (questProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
                questProgressIcon[1].SetActiveRecursively(true);
            //else
            //    questProgressIcon[0].SetActiveRecursively(true);
        }
        else
        {
            if (questProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
                questProgressIcon[3].SetActiveRecursively(true);
            else if (questProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || questProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR) 
                questProgressIcon[2].SetActiveRecursively(true);
        }

    }
}
