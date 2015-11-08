using UnityEngine;
using System.Collections.Generic;

public class AsQuestMiniViewMgr : MonoBehaviour
{
	public AsDlgBase bgGrid;
	public AsQuestMiniView[] miniViews;
	public float offsetViewHeight = 0.3f;
	public float hideTime = 0.0f;
	public bool updateAchieve = false;
	public bool updateQuest = false;
	public bool clearStart = false;
	public bool hiding = false;
	QuestMiniViewUpdateState updateState = QuestMiniViewUpdateState.NONE;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		updateAchieve = false;
		updateQuest = false;
		hiding = false;
		clearStart = false;

		for( int i = 0; i < miniViews.Length; i++)
		{
			updateState = miniViews[i].UpdateAchievement();

			switch( updateState)
			{
			case QuestMiniViewUpdateState.COMPLETE_ACHIEVEMENT:
				updateAchieve = true;
				break;
			case QuestMiniViewUpdateState.CLEAR_QUEST:
				updateQuest = true;
				break;
			case QuestMiniViewUpdateState.CLEAR_HIDING:
				hiding = true;
				break;
			case QuestMiniViewUpdateState.CLEAR_START:
				clearStart = true;
				break;
			}
		}

		if( clearStart == true)
		{
			for( int i = 0; i < miniViews.Length; i++)
				miniViews[i].ResetQuestClearTime();
		}

		if( updateQuest == true)
		{
			List<ArkQuest> listSortedQuest = ArkQuestmanager.instance.GetSortedQuestListForQuestMini();
			if( listSortedQuest.Count <= 0)
			{
				NothingQuestProcess();
			}
			else
			{
				SetQuests( ArkQuestmanager.instance.GetSortedQuestListForQuestMini());
				UpdateBackSizeAndPosition();
			}
		}
		else if( updateAchieve == true)
		{
			UpdateBackSizeAndPosition();
		}
	}

	public void UpdateQuetMiniViewMgr()
	{
		if( hiding == false)
		{
			List<ArkQuest> listQuest = ArkQuestmanager.instance.GetSortedQuestListForQuestMini();

			// if quest count is 0
			if( listQuest.Count <= 0 && AsHudDlgMgr.Instance != null)
			{
				if( AsPartyManager.Instance.GetPartyMemberList().Count > 1) // have party
				{
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.SetState( AsPartyAndQuestToggleMgr.PartyAndQuestToggleState.OpenParty);
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.SetBtnState( AsPartyAndQuestToggleMgr.PartyAndQuestToggleState.OpenParty);
					AsHudDlgMgr.Instance.partyAndQuestToggleMgr.OpenParty();
				}
				else
				{
					if( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null)
					{
						AsHudDlgMgr.Instance.partyAndQuestToggleMgr.SetState( AsPartyAndQuestToggleMgr.PartyAndQuestToggleState.CloseQuestMini);
						AsHudDlgMgr.Instance.partyAndQuestToggleMgr.SetBtnState( AsPartyAndQuestToggleMgr.PartyAndQuestToggleState.CloseQuestMini);
					}
				}

				AsHudDlgMgr.Instance.CloseQuestMiniView();

				return;
			}

			SetQuests( listQuest);
			UpdateBackSizeAndPosition();
		}
	}

	public void NothingQuestProcess()
	{
		if( ( AsHudDlgMgr.Instance != null) && ( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null))
			AsHudDlgMgr.Instance.partyAndQuestToggleMgr.NothingQuestProcess();
	}

	public void SetQuests( List<ArkQuest> _listQuest)
	{
		for( int i = 0; i < miniViews.Length; i++)
		{
			if( i < _listQuest.Count)
			{
				miniViews[i].gameObject.SetActiveRecursively( true);
				miniViews[i].Initilize( _listQuest[i].GetQuestData(), hideTime);
			}
			else
			{
				miniViews[i].gameObject.SetActiveRecursively( false);
			}
		}
	}

	public void UpdateBackSizeAndPosition()
	{
		// calcul back ground size
		float fMaxWidth = 0.0f;
		float fHeight = 0.0f;
		List<Vector2> listSize = new List<Vector2>();

		for( int i = 0; i < miniViews.Length; i++)
		{
			if( miniViews[i].gameObject.active == true)
			{
				Vector2 vSize = miniViews[i].GetNowSize();

				listSize.Add( vSize);

				if( vSize.x > fMaxWidth)
					fMaxWidth = vSize.x;

				fHeight += vSize.y;
			}
		}

		if( listSize.Count >= 1)
			fHeight += offsetViewHeight * ( listSize.Count - 1);

		if( bgGrid.gameObject.active == false)
			bgGrid.gameObject.SetActiveRecursively( true);

		bgGrid.width  = fMaxWidth;
		bgGrid.height = fHeight;
		bgGrid.Assign();

		// reposition view
		float fStartY = fHeight * 0.5f;
		int count = 0;
		for( int i = 0; i < miniViews.Length; i++)
		{
			if( miniViews[i].gameObject.active == true)
			{
				miniViews[i].transform.localPosition = new Vector3( fMaxWidth * -0.5f, fStartY, -1.0f);

				fStartY -= ( listSize[count].y + offsetViewHeight);

				count++;
			}
		}
	}
}
