using UnityEngine;
using System.Collections.Generic;

public class QuestListIconController : MonoBehaviour {

    public QuestType nowQuestType = QuestType.QUEST_NONE;
    public SimpleSprite[] icons;
	private bool m_isUseEvent = false; //kij 2013-10-02


    void OnEnable()
    {
        SetQuestIconType(nowQuestType);
    }

    public void SetQuestIconType(QuestType _questType)
    {
		// begin kij 2013-10-02
		if( true== m_isUseEvent)
		{
			DisableAllIcon();
			return;
		}
		// end kij 2013-10-02

        if (_questType == QuestType.QUEST_DAILY || _questType == QuestType.QUEST_MAIN || _questType == QuestType.QUEST_NPC_DAILY ||
			_questType == QuestType.QUEST_FIELD || _questType == QuestType.QUEST_BOSS || _questType == QuestType.QUEST_PVP)
        {
            nowQuestType = _questType;

            DisableAllIcon();

			if (_questType == QuestType.QUEST_MAIN)
				icons[0].gameObject.SetActive(true);
			else if (_questType == QuestType.QUEST_FIELD)
				icons[1].gameObject.SetActive(true);
			else if (_questType == QuestType.QUEST_DAILY)
				icons[2].gameObject.SetActive(true);
			else if (_questType == QuestType.QUEST_BOSS)
				icons[3].gameObject.SetActive(true);
			else if (_questType == QuestType.QUEST_PVP)
				icons[4].gameObject.SetActive(true);
			else if (_questType == QuestType.QUEST_NPC_DAILY)
				icons[5].gameObject.SetActive(true);
        }
    }	
	
	// begin kij 2013-10-02
	public void SetUseEvent()
	{
		m_isUseEvent = true;
		DisableAllIcon();
	}
	// end kij 2013-10-02

    void DisableAllIcon()
    {
        foreach (SimpleSprite sprite in icons)
			sprite.gameObject.SetActive(false);
    }
}
