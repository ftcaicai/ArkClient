using UnityEngine;
using System.Collections.Generic;
using ArkSphereQuestTool;
using System.Text;

public enum QuestMiniViewUpdateState
{
	NONE,
	COMPLETE_ACHIEVEMENT,
	CLEAR_QUEST,
	CLEAR_START,
	CLEAR_HIDING,
}

public enum QuestMiniViewState
{
	NONE,
	CLEAR_ACHIEVEMENT,
	CLEAR_QUEST,
}


public class AsQuestMiniView : MonoBehaviour
{
	public float offsetIconToTitle;
	public float offsetTitleToAchieveWidth;
	public float offsetTitleToAchieveHeight;
	public float offsetAchieveToCount;
	public float offsetAchieve;

	public GameObject objAchieveParent;
	public SimpleSprite[] questIcons;
	public SpriteText txtTitle;
	public SpriteText[] txtAchievements;
	public SpriteText[] txtAchCounts;
	private List<int> listAchIndex = new List<int>();
	private List<AchBase> listAchievement = new List<AchBase>();
	private List<bool> listAchCompleteCheck = new List<bool>();
	private List<bool> listAchDisable = new List<bool>();
	private List<float> listAchRemainTime = new List<float>();
	private QuestData questData;
	private string achNewString = string.Empty;
	private float achHideTime = 0.0f;
	private float clearHideTime = 0.0f;
	private bool clearQuest = false;
	private bool hide = false;
	private Vector2 size = Vector2.zero;

	Color normalColor = Color.white;
	Color clearColor = Color.green;
	Color failColor = Color.red;

	public void Initilize( QuestData _questData, float _achHideTime)
	{
		achHideTime = _achHideTime;
		SetQuestData( _questData);
	}

	void SetQuestData( QuestData _qustData)
	{
		clearQuest = false;
		hide = false;
		clearHideTime = achHideTime;
		listAchCompleteCheck.Clear();
		listAchRemainTime.Clear();
		listAchDisable.Clear();

		questData = _qustData;
		listAchievement = questData.Achievement.GetDatas();

		// check achieveCount;
		listAchIndex.Clear();
		int count = 0;
		for( int i = 0; i < listAchievement.Count; i++)
		{
			if( listAchievement[i].IsComplete == false)
			{
				listAchIndex.Add( count);
				listAchCompleteCheck.Add( false);
				listAchDisable.Add( false);
				listAchRemainTime.Add( 2.0f);
			}
			else if (listAchievement[i].QuestMessageType == QuestMessages.QM_TIME_LIMIT)
			{
				listAchIndex.Add(count);
				listAchCompleteCheck.Add(false);
				listAchDisable.Add(false);
				listAchRemainTime.Add(2.0f);
			}

			count++;
		}

		// icon
		SetQuestIcon( _qustData);

		// achievement
		SetQuestInfoString( _qustData);

		// reposition
		RepositionAchievements();
	}

	public void ResetQuestClearTime()
	{
		clearHideTime = achHideTime;
	}

	public QuestMiniViewUpdateState UpdateAchievement()
	{
		QuestMiniViewUpdateState state = QuestMiniViewUpdateState.NONE;

		if( gameObject.active == false || questData == null || hide == true)
			return QuestMiniViewUpdateState.NONE;

		float fDeltaTime = Time.deltaTime;

		// check quest clear
		if( questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR ||
			questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
		{
			if( clearQuest == false)
			{
				clearQuest = true;
				clearHideTime = achHideTime;
				txtTitle.Text = clearColor + txtTitle.Text;
				ResetAllRemainTime();
				SetAllAchievementGreen();
				return QuestMiniViewUpdateState.CLEAR_START;
			}
		}

		// check quest clear
		if( clearQuest == true)
		{
			clearHideTime -= fDeltaTime;

			if( clearHideTime <= 0.0f)
			{
				hide = true;

				clearHideTime = 0.0f;

				gameObject.SetActiveRecursively( false);

				return QuestMiniViewUpdateState.CLEAR_QUEST;
			}

			return QuestMiniViewUpdateState.CLEAR_HIDING;
		}

		// check hide time
		for( int i = 0; i < listAchIndex.Count; i++)
		{
			if( listAchCompleteCheck[i] == false || listAchDisable[i] == true)
				continue;

			int achIdx = listAchIndex[i];

			listAchRemainTime[i] -= fDeltaTime;

			if( listAchRemainTime[i] > 0.0f)
				continue;

			listAchRemainTime[i] = 0.0f;
			listAchDisable[i] = true;

			if( txtAchievements[achIdx].enabled == true)
				txtAchievements[achIdx].gameObject.SetActiveRecursively( false);

			if( txtAchCounts[achIdx].enabled == true)
				txtAchCounts[achIdx].gameObject.SetActiveRecursively( false);

			// reposition
			RepositionAchievements();

			state = QuestMiniViewUpdateState.COMPLETE_ACHIEVEMENT;
		}

		// check achieve complete
		for( int i = 0; i < listAchIndex.Count; i++)
		{
			int idx = listAchIndex[i];

			achNewString = listAchievement[idx].GetProgressStringOnly();

			// update count
			if( ( txtAchCounts[idx].gameObject.active == true) && ( txtAchCounts[idx].Text != achNewString))
				txtAchCounts[idx].Text = achNewString;

			// check
			if( listAchievement[idx].IsComplete == true && listAchCompleteCheck[i] == false && listAchievement[i].QuestMessageType != QuestMessages.QM_TIME_LIMIT)
			{
				listAchCompleteCheck[i] = true;
				txtAchievements[idx].Text = clearColor + txtAchievements[idx].Text;
				txtAchCounts[idx].gameObject.SetActiveRecursively( false);

				ResetAllRemainTime();
			}
		}

		return state;
	}

	void ResetAllRemainTime()
	{
		for( int i = 0; i < listAchIndex.Count ; i++)
		{
			if( listAchCompleteCheck[i] == true)
				listAchRemainTime[i] = achHideTime;
		}
	}

	void SetAllAchievementGreen()
	{
		for( int i = 0; i < listAchIndex.Count; i++)
		{
			int idx = listAchIndex[i];

			achNewString = listAchievement[idx].GetProgressStringOnly();

			txtAchievements[idx].Text = clearColor + txtAchievements[idx].Text;
			txtAchCounts[idx].gameObject.SetActiveRecursively( false);
		}
	}

	void SetQuestInfoString( QuestData _questData)
	{
		StringBuilder sbTitle = new StringBuilder();

		Color titleColor = normalColor;

		if( _questData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
			titleColor = failColor;

		sbTitle.Append( titleColor.ToString());

		sbTitle.Append( _questData.Info.Name);

		// title
		txtTitle.gameObject.SetActiveRecursively( true);
		txtTitle.Text = sbTitle.ToString();

		// achievement
		for( int i = 0; i < txtAchievements.Length; i++)
		{
			AchBase achieve = _questData.Achievement.GetData( i);

			if( achieve != null)
			{
				bool isComplete = achieve.IsComplete;

				if (achieve.QuestMessageType == QuestMessages.QM_TIME_LIMIT)
					isComplete = false;

				if ( isComplete == true)
				{
					txtAchievements[i].gameObject.SetActiveRecursively( false);
					txtAchCounts[i].gameObject.SetActiveRecursively( false);
				}
				else
				{
					txtAchievements[i].gameObject.SetActiveRecursively( true);
					txtAchCounts[i].gameObject.SetActiveRecursively( true);
					txtAchievements[i].Text = normalColor + achieve.GetAchievementString();
					txtAchCounts[i].transform.localPosition = new Vector3( txtAchievements[i].TotalWidth + offsetAchieveToCount, 0.0f, 0.0f);

					if( achieve.QuestMessageType == QuestMessages.QM_TALK_WITH_NPC)
						txtAchCounts[i].gameObject.SetActiveRecursively( false);
					else
						txtAchCounts[i].Text = normalColor + achieve.GetProgressStringOnly();
				}
			}
			else
			{
				txtAchievements[i].gameObject.SetActiveRecursively( false);
				txtAchCounts[i].gameObject.SetActiveRecursively( false);
			}
		}
	}

	void RepositionAchievements()
	{
		txtTitle.gameObject.transform.localPosition = new Vector3( questIcons[0].width + offsetIconToTitle, 0.0f, 0.0f);
		objAchieveParent.transform.localPosition = new Vector3( offsetTitleToAchieveWidth, -txtTitle.BaseHeight * 0.5f -txtAchievements[0].BaseHeight * 0.5f - offsetTitleToAchieveHeight, 0.0f);

		float fHeight = 0.0f;

		for( int i = 0; i < txtAchievements.Length; i++)
		{
			if( txtAchievements[i].gameObject.active == true)
			{
				txtAchievements[i].transform.localPosition = new Vector3( 0.0f, fHeight, 0.0f);
				fHeight -= ( txtAchievements[i].BaseHeight + offsetAchieve);
			}
		}
	}

	void SetQuestIcon( QuestData _questData)
	{
		DisableAllIcon();

		switch( _questData.Info.QuestType)
		{
		case QuestType.QUEST_MAIN:
			questIcons[0].gameObject.SetActive( true);
			break;
		case QuestType.QUEST_FIELD:
			questIcons[1].gameObject.SetActive(true);
			break;
		case QuestType.QUEST_DAILY:
			questIcons[2].gameObject.SetActive(true);
			break;
		case QuestType.QUEST_BOSS:
			questIcons[3].gameObject.SetActive(true);
			break;
		case QuestType.QUEST_PVP:
			{
				if (questIcons.Length >= 5)
					questIcons[4].gameObject.SetActive(true);
			}
			break;
		case QuestType.QUEST_NPC_DAILY:
			questIcons[5].gameObject.SetActive(true);
			break;
		}
	}

	void DisableAllIcon()
	{
		for( int i = 0; i < questIcons.Length; i++)
			questIcons[i].gameObject.SetActive(false);
	}

	public Vector2 GetNowSize()
	{
		float fHeight = 0.0f;

		// height
		fHeight += txtTitle.BaseHeight + offsetTitleToAchieveHeight;

		int achCount = 0;
		for( int i = 0; i < txtAchievements.Length; i++)
		{
			if( txtAchievements[i].gameObject.active == false)
				continue;

			fHeight += txtAchievements[i].BaseHeight;
			achCount++;
		}

		fHeight += offsetAchieve * achCount;

		fHeight = fHeight > questIcons[0].height ? fHeight : questIcons[0].height;

		// width
		float titleWidth = questIcons[0].width + offsetIconToTitle + txtTitle.TotalWidth;
		float nowAchWidth = 0.0f;
		float achWidthMax = 0.0f;

		for( int i = 0; i < txtAchievements.Length; i++)
		{
			if( txtAchievements[i].gameObject.active == false)
				continue;

			nowAchWidth = questIcons[0].width + offsetIconToTitle + offsetAchieveToCount + offsetTitleToAchieveWidth + txtAchievements[i].TotalWidth + txtAchCounts[i].TotalWidth;

			if( nowAchWidth > achWidthMax)
				achWidthMax = nowAchWidth;
		}

		achWidthMax = achWidthMax > titleWidth ? achWidthMax : titleWidth;

		size = new Vector2( achWidthMax, fHeight);

		return size;
	}

}
