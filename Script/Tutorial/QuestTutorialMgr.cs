using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class QuestTutorialMgr : MonoBehaviour
{
	private List<int> listQuestID = new List<int>();
	private float fFingerOffsetZ = -2.0f;
	private static QuestTutorialMgr s_Instance = null;

	public TutorialFingerController fingerController = null;
	public List<QuestTutorial> listTutorial = new List<QuestTutorial>();
	public QuestTutorial nowSelectTutorial = null;
	public Vector2 imageHalfSize;
	public TutorialStepInfo nowStep = null;
	public int firstSelectNpcID = 0;
	public bool firstConnect = false;
	public bool attendBonus = false;
	public bool waitUntilNpcLoad = false;
	public float waitTime = 0.0f;

	#region -instance-
	public static QuestTutorialMgr Instance
	{
		get
		{
			if( s_Instance == null)
				s_Instance = FindObjectOfType( typeof( QuestTutorialMgr)) as QuestTutorialMgr;

			if( s_Instance == null)
			{
				GameObject obj = new GameObject( "QuestTutorialMgr");
				s_Instance = obj.AddComponent( typeof( QuestTutorialMgr)) as QuestTutorialMgr;

				Debug.Log( "Could not locate an QuestTutorialMgr object.\n QuestTutorialMgr was Generated Automaticly.");
			}

			return s_Instance;
		}
	}
	#endregion

	void LoadFingerPointer()
	{
		if( fingerController == null)
		{
			GameObject objFingerPointer = GameObject.Instantiate( Resources.Load( "UI/AsGUI/GUI_TutorialFinger")) as GameObject;
			fingerController = objFingerPointer.GetComponent<TutorialFingerController>();
			objFingerPointer.transform.parent = transform;
			objFingerPointer.transform.localPosition = Vector3.zero;
			fingerController.SetFingerType( TutorialFingerType.HIDE);
		}
	}

	// Use this for initialization
	void Start()
	{
		LoadFingerPointer();
		DontDestroyOnLoad( this);
	}

	public void LoadTutorial( string _path)
	{
		XmlElement root = AsTableBase.GetXmlRootElement( _path);

		if( root.Attributes["StartNpcID"] != null)
			firstSelectNpcID = System.Convert.ToInt32( root.Attributes["StartNpcID"].Value);

		XmlNodeList nodes = root.SelectNodes( "Tutorial");

		foreach ( XmlNode node in nodes)
		{
			QuestTutorial questTutorial = new QuestTutorial();
			questTutorial.LoadFromXml( node);
			listTutorial.Add( questTutorial);

			// save quest id
			foreach ( TutorialConditionBase condtion in questTutorial.listCondition)
			{
				if( condtion.type == TutorialConditionType.QUEST_ID)
				{
					TutorialConditionUseInt condQuestID = condtion as TutorialConditionUseInt;
					if( !listQuestID.Contains( condQuestID.value))
						listQuestID.Add( condQuestID.value);
				}
			}
		}
	}

	public void ProcessQuestTutorialMsg( QuestTutorialMsgInfo _msgInfo)
	{
		// accept quest
		if( _msgInfo.message == QuestTutorialMsg.ACCEPT_QUEST)
		{
			foreach ( QuestTutorial tutorial in listTutorial)
			{
				if( tutorial.CanProcess( _msgInfo) == true)
				{
					nowSelectTutorial = tutorial;
					TutorialStepInfo stepInfo = nowSelectTutorial.ForcedStartStepZero();

					if( stepInfo != null)
					{
						nowStep = stepInfo;
						ShowFinger( stepInfo);
							return;
					}
				}
			}
		}

		if( nowSelectTutorial == null)
			return;

		// clear Quest
		if( _msgInfo.message == QuestTutorialMsg.CLEAR_QUEST || _msgInfo.message == QuestTutorialMsg.DROP_QUEST)
		{
			nowStep = null;

			if( _msgInfo.message == QuestTutorialMsg.DROP_QUEST)
			{
				if( nowSelectTutorial.notProcessMsg == true)
					nowSelectTutorial.notProcessMsg = false;
			}

			nowSelectTutorial = null;
			HideFinger();
		}
		else if( nowSelectTutorial != null)
		{
			// close Msg
			if( nowSelectTutorial.listCloseMsg.Contains( _msgInfo.message))
			{
				Debug.LogWarning( "Close accept this msg = "+_msgInfo.message);
				nowStep = null;
				HideFinger();

				// check restart step
				if( nowSelectTutorial.listStartStepMsg.Count == 0 && nowSelectTutorial.notRestartAfterClose == false)
				{
					TutorialStepInfo stepInfo = nowSelectTutorial.ForcedProgressStep( 0);
					if( stepInfo != null)
					{
						nowStep = stepInfo;
						ShowFinger( stepInfo);
					}
				}
			}
			else if( nowSelectTutorial.listUpdateMsg.Contains( _msgInfo.message))
			{
				nowStep = nowSelectTutorial.ProcessUpdateMessage();
				if( nowStep != null)
				ShowFinger( nowStep);
			}
			else
			{
				if( nowSelectTutorial.CanProcess( _msgInfo) == false)
					return;

				TutorialStepInfo step = nowSelectTutorial.ProcessMessage( _msgInfo);

				if( step == null)
					return;

				if( step.type == TutorialStepType.HIDE || step.type == TutorialStepType.LAST_HIDE)
				{
					nowStep = step;
					HideFinger();

					if( step.type == TutorialStepType.LAST_HIDE)
						nowSelectTutorial.notProcessMsg = true;
				}
				else
				{
					// check not process ----> last hide
					if( nowSelectTutorial.notProcessMsg == false)
					{
						nowStep = step;

						if( nowStep.type == TutorialStepType.OVER_INVEN_ITEM)
							SetInvenPageExistThisItem( nowStep.listOption[0]);
	
						ShowFinger( step);
					}
				}
			}
		}
	}

	void SetInvenPageExistThisItem( int _itemID)
	{
		RealItem item = ItemMgr.HadItemManagement.Inven.GetRealItem( _itemID);

		if( item == null)
			return;

		int itemPage = ( int)( ( float)( item.getSlot - Inventory.useInvenSlotBeginIndex) / ( float)Inventory.useInvenSlotNumInPage);

		AsHudDlgMgr.Instance.invenPageIdx = itemPage;

		if( AsHudDlgMgr.Instance.invenDlg != null)
		{
			UIInvenDlg invenDlg = AsHudDlgMgr.Instance.invenDlg;
			invenDlg.page.SetPage( itemPage);
			invenDlg.ResetSlotItmes();
		}
	}

	void ShowFingerCore( TutorialFingerType _type, bool _double, GameObject _targetObject)
	{
		if( fingerController == null)
			return;

		fingerController.SetFingerType( _type, _double);

		SetFingerPosition( _targetObject);
	}

	void SetFingerPosition( GameObject _target)
	{
		//if( _target.transform.parent != null)
		//{
		// vFingerOffset.z = fFingerOffsetZ;
		// // vFingerOffset += new Vector3( 0.0f, 0.0f, _target.transform.parent.transform.position.z);
		//}
	
		fingerController.transform.localPosition = ConvertForPointer( imageHalfSize, _target.transform.position);
	}

	Vector3 ConvertForPointer( Vector2 imageSize, Vector3 point)
	{
		return new Vector3( point.x - imageSize.x, point.y - imageHalfSize.y * 0.5f, point.z + fFingerOffsetZ);
	}

	void HideFinger()
	{
		//if( fingerController.fingerType != TutorialFingerType.HIDE)
			fingerController.SetFingerType( TutorialFingerType.HIDE);
	}

	void Update()
	{
		if( nowStep != null)
		{
			if( nowStep.type == TutorialStepType.OVER_OBJECT_UPDATE || nowStep.type == TutorialStepType.OVER_OBJECT_OUT_HUD_UPDATE ||
				nowStep.type == TutorialStepType.OVER_DESIGNATION_LIST || nowStep.type == TutorialStepType.OVER_INVEN_ITEM ||
				nowStep.type == TutorialStepType.OVER_KILLSHOP || nowStep.type == TutorialStepType.OVER_SOCIALHISTORY_LIST ||
				nowStep.type == TutorialStepType.OVER_LISTITEM_OUT_HUD)
			{
				if( nowSelectTutorial.CheckCompleteAchievementUpdate() == true)
				{
					HideFinger();
					return;
				}

				if( nowStep.objectTarget != null)
				{
					if( nowStep.objectTarget.active == true)
					{
						if( fingerController.gameObject.active == false)
							fingerController.gameObject.SetActiveRecursively( true);

						SetFingerPosition( nowStep.objectTarget);
					}
					else
					{
						fingerController.gameObject.SetActiveRecursively( false);
					}
				}
				else
				{
					fingerController.gameObject.SetActiveRecursively( false);
				}
			}
		}
		else
		{
			HideFinger();
		}

		// start
		// if( waitUntilNpcLoad == true)
		#region -AccountGender
		if( ( true == waitUntilNpcLoad) && ( eGENDER.eGENDER_NOTHING != AsUserInfo.Instance.accountGender))
		#endregion
		{
			waitTime += Time.deltaTime;

			if( waitTime >= 0.5f)
			{
				waitTime = 0.0f;
	
				StartQuestTutorial();
			}
		}
	}

	void ShowFinger( TutorialStepInfo _step)
	{
		GameObject findTarget = FindTargetObject( _step);

		if( findTarget != null)
		{
			ShowFingerCore( _step.fingerType, _step.doubleTouch, findTarget);

			//if( nowStep.type == TutorialStepType.OVER_OBJECT_UPDATE || nowStep.type == TutorialStepType.OVER_OBJECT_OUT_HUD_UPDATE ||
			// nowStep.type == TutorialStepType.OVER_DESIGNATION_LIST || nowStep.type == TutorialStepType.OVER_INVEN_ITEM ||
			// nowStep.type == TutorialStepType.OVER_KILLSHOP)
			nowStep.objectTarget = findTarget;
		}
		else
		{
			HideFinger();
		}
	}

	public bool IsTutorialQuest( int _questID)
	{
		return listQuestID.Contains( _questID);
	}

	public void StartAcceptedQuestTutorial()
	{
		// set default tutorial
		nowSelectTutorial = listTutorial[0];

		List<int> listProgressQuest = ArkQuestmanager.instance.GetProgressQuestIDList();

		foreach ( int questID in listProgressQuest)
		{
			if( IsTutorialQuest( questID))
				ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.ACCEPT_QUEST, questID));
		}
	}

	public void StartQuestTutorial()
	{
		if( firstConnect == false)
			return;

		AsNpcEntity npcEntity = AsEntityManager.Instance.GetNPCEntityByTableID( firstSelectNpcID) as AsNpcEntity;

		if( npcEntity != null && eModelLoadingState.Finished == npcEntity.CheckModelLoadingState() && eGENDER.eGENDER_NOTHING != AsUserInfo.Instance.accountGender)
		{
			waitUntilNpcLoad = false;
			waitTime = 0.0f;
			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_NpcClick( npcEntity.sessionId_));
			AsEntityManager.Instance.DispatchMessageByNpcSessionId( npcEntity.sessionId_, new Msg_NpcClick( npcEntity.sessionId_));
		}
		else
		{
			waitUntilNpcLoad = true;
		}
	}

	public void Reset()
	{
		firstConnect = false;
		attendBonus = false;
		HideFinger();
		nowSelectTutorial = null;
		if( nowStep != null)
		{
			nowStep.objectTarget = null;
			nowStep = null;
		}
	}

	public bool HaveNextMsgInNowTutorial( QuestTutorialMsg _msg)
	{
		if( nowSelectTutorial == null)
			return false;

		return nowSelectTutorial.listNextStepMsg.Contains( _msg);
	}

	#region util
	public static GameObject GetGameObjectOutHUD( string _uiPath, out string[] _splits)
	{
		_splits = _uiPath.Split('/');

		return GameObject.Find( _splits[0]);
	}

	public static GameObject FindTargetObject( TutorialStepInfo _step)
	{
		string[] splits;

		if( _step.type == TutorialStepType.OVER_QUEST_LIST)
		{
			Transform tm = AsHUDController.Instance.m_NpcMenu.m_questScroll.transform.GetChild( 0).GetChild( _step.listOption[0]);
			if( tm != null)
				return tm.gameObject;
			else
				return null;
		}
		else if( _step.type == TutorialStepType.OVER_LISTITEM_OUT_HUD)
		{
			if( AsSocialManager.Instance == null)
				return null;

			if( AsSocialManager.Instance.IsOpenSocialDlg() == false)
				return null;

			if( AsSocialManager.Instance.SocialUI.IsOpenSocailStoreDlg == false)
				return null;

			IUIListObject findItem = AsSocialManager.Instance.SocialUI.m_SocialStoreDlg.GetStoreItemInfo( _step.listOption[0]);

			if( findItem == null)
				return null;
			else
				return findItem.gameObject;
		}
		else if( _step.type == TutorialStepType.OVER_DESIGNATION_LIST || _step.type == TutorialStepType.OVER_SOCIALHISTORY_LIST)
		{
			GameObject findObject = GetGameObjectOutHUD( _step.uiPath, out splits);

			if( findObject != null)
			{
				Transform tm = findObject.transform.FindChild( _step.uiPath.Replace( splits[0] + "/", ""));
				if( tm == null)
					return null;

				UIScrollList scroll = tm.gameObject.GetComponentInChildren<UIScrollList>();
				if( scroll == null)
					return null;

				if( _step.type == TutorialStepType.OVER_DESIGNATION_LIST)
				{
					int idx = AsDesignationManager.Instance.GetDesignationIdx( _step.listOption[0]);
					if( idx == -1)
						return null;

					int realIdx = scroll.Count - idx - 1;

					IUIListObject listItem = scroll.GetItem( realIdx);
					if( listItem == null)
						return null;

					scroll.ScrollToItem( realIdx, 0.0f);

					return listItem.gameObject;
				}
				else if( _step.type == TutorialStepType.OVER_SOCIALHISTORY_LIST)
				{
					List<IUIListObject> listItem = scroll.GetItems();

					foreach ( IUIListObject item in listItem)
					{
						AsGameHistoryItem history = item.gameObject.GetComponentInChildren<AsGameHistoryItem>();

						if( history.m_FaceBookBtn.controlIsEnabled == true)
							return history.m_FaceBookBtn.gameObject;
					}

					return null;
				}
			}
			else
			{
				return null;
			}
		}
		else if( _step.type == TutorialStepType.OVER_KILLSHOP)
		{
			if( false == AsHudDlgMgr.Instance.IsOpenedSkillshop)
				return null;

			AsSkillshopDlg dlg = AsHudDlgMgr.Instance.skillShopObj.GetComponentInChildren<AsSkillshopDlg>();
			Debug.Assert( null != dlg);
			UIScrollList list = dlg.tab.panels[( int)eShopTabState.ChoiceTab].getList();
			if( list.transform.GetChild( 0).childCount > 0)
			{
				Transform tm = list.transform.GetChild( 0).GetChild( _step.listOption[0]);
				if( tm != null)
					return tm.gameObject;
				else
					return null;
			}
		}
		else if( _step.type == TutorialStepType.OVER_INVEN_ITEM)
		{
			GameObject findObject = null;
			Transform tm = GameObject.Find( _step.uiPath).transform;

			if( tm != null)
				findObject = tm.gameObject;
			else
				return null;

			RealItem findItem = null;

			foreach ( int itemID in _step.listOption)
			{
				RealItem item = ItemMgr.HadItemManagement.Inven.GetRealItem( itemID);
				if( item != null)
				{
					findItem = item;
					break;
				}
			}

			if( findItem != null)
			{
				UIInvenDlg invenDlg = findObject.GetComponent<UIInvenDlg>();
				if( invenDlg != null)
				{
					if( invenDlg.page == null)
						return null;

					int itemPage = ( int)( ( float)( findItem.getSlot - Inventory.useInvenSlotBeginIndex) / ( float)Inventory.useInvenSlotNumInPage);

					if( itemPage == invenDlg.page.curPage)
					{
						int itemSlot = ( findItem.getSlot - Inventory.useInvenSlotBeginIndex) % Inventory.useInvenSlotNumInPage;
						return invenDlg.invenslots[itemSlot].gameObject;
					}
					else
					{
						return null;
					}
				}
				else
					return null;
			}
		}
		else if( _step.type == TutorialStepType.OVER_OBJECT || _step.type == TutorialStepType.OVER_OBJECT_UPDATE)
		{
			Transform tm = AsHUDController.Instance.transform.FindChild( _step.uiPath);

			if( tm != null)
				return tm.gameObject;
			else
				return null;
		}
		else if( _step.type == TutorialStepType.OVER_OBJECT_OUT_HUD || _step.type == TutorialStepType.OVER_OBJECT_OUT_HUD_UPDATE)
		{
			GameObject findObject = GetGameObjectOutHUD( _step.uiPath, out splits);

			if (findObject != null)
			{	
				string path = _step.uiPath.Remove(0, splits[0].Length + 1);

				Transform tm = findObject.transform.FindChild(path);
				if (tm == null)
					return null;
				else
					return tm.gameObject;
			}
			else
			{
				Debug.Log("Not found = " + _step.uiPath);
				return null;
			}
		}
	
		return null;
	}
	#endregion
}
