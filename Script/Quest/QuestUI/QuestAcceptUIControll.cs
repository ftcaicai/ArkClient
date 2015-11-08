using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


public class QuestAcceptUIControll : UIMessageBase
{
	public enum RewardItemType
	{
		SKILL,
		ITEM,
	}

	public class RewardItemData
	{
		public RewardItemType type;
		public int id;
		public object externalData1;
		public object externalData2;
	
		public RewardItemData( RewardItemType _type, int _id, object _exData1, object _exData2 = null)
		{
			type = _type;
			id = _id;
			externalData1 = _exData1;
			externalData2 = _exData2;
		}
	}

	public enum QuestAcceptUIState
	{
		QuestAccept_Suggest,
		QuestAccept_Progress,
		QuestAccept_Clear,
		QuestAccept_Reward,
		QuestAccept_Fail,
	}

	QuestAcceptUIState nowState = QuestAcceptUIState.QuestAccept_Suggest;
	Dictionary<GameObject, int> dicSelectItem = new Dictionary<GameObject, int>();
	Dictionary<GameObject, int> dicItem = new Dictionary<GameObject, int>();
	List<GameObject> listItemObject = new List<GameObject>();
	List<GameObject> listSelectItemObject = new List<GameObject>();
	List<RewardItem> listSelectItem;
	List<RewardItem> listItem;
	List<RewardItemData> listRewardItemData = new List<RewardItemData>();
	QuestClearType questClearType = QuestClearType.NONE;
	int questClearPrice = 0;
	int nowSelectItem = -1;
	int nowItem = -1;
	bool bCallQuestlist = false;
	bool bLock = false;
	AsMessageBox messageBox = null;
	ArkSphereQuestTool.QuestData nowQuestData = null;
	Tbl_QuestMove_Record questMoveRecord = null;

	#region -property-
	public float callFromQuestBookZ;
	public float callFromTalkZ;
	public SpriteText titleLabel;
	public SpriteText explainLabel;
	public GameObject[] questIcons;
	public SpriteText[] achieveLabels;
	public SpriteText[] tabs;
	public SimpleSprite[] spritesWidth;
	public SpriteText rewardDesignaionLabel;
	public SpriteText questLevelLabel;
	public SpriteText questRegionLabel;
	public SpriteText questLocationLabel;
	public SpriteText rewardLabel1;
	public SpriteText rewardLabel2;
	public SpriteText rewardLavelMiracle;
	public SimpleSprite miracleSprite;
	public SimpleSprite goldSprite;
	public SimpleSprite expSprite;
	public AsNotify popupCashNotify;
	public GameObject slotParent;
	public GameObject selectObj;
	public UIButton buttonCashDone;
	public UIButton buttonClose;
	public UIButton buttonAccept;
	public UIButton buttonReward;
	public UIButton buttonGiveup;
	public UIButton buttonQuestMove;
	public Transform[] slotItemTMs;
	private Vector2 vItemSize;
	public Color[] colorTexts;
	public string[] colorTags;
	public AsSkillTooltipInBook toolTipInBook = null;
	AsSkillTooltipInBook m_skillTooltip = null;
	#endregion

	float fOffsetIcon = -1.0f;
	float fOffsetToolTip = -3.0f;
	float fBackgroundWidth = 0.0f;

	float[] fRewardSpritePosositions = null;
	int nRewardSpritePosCount = 0;

	public bool LockInput
	{
		get { return bLock; }
		set
		{
			bLock = value;

			if( bLock == false)
				AsLoadingIndigator.Instance.HideIndigator();
		}
	}

	// Use this for initialization
	void Awake()
	{
		buttonAccept.SetInputDelegate( buttonClick);
		buttonReward.SetInputDelegate( buttonClick);
		buttonGiveup.SetInputDelegate( buttonClick);
		buttonClose.SetInputDelegate( buttonClick);
		buttonQuestMove.SetInputDelegate( buttonClick);
		buttonCashDone.SetInputDelegate( CashClearButtonClick);

		fRewardSpritePosositions = new float[] { expSprite.transform.localPosition.x,
			goldSprite.transform.localPosition.x,
			miracleSprite.transform.localPosition.x };

		fBackgroundWidth = 0.0f;
		foreach( SimpleSprite sprite in spritesWidth)
			fBackgroundWidth += sprite.width;
	}

	void Start()
	{
		selectObj.SetActiveRecursively( false);
	}

	void SetText()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonAccept.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonGiveup.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonReward.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonCashDone.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( questLevelLabel);

		foreach( SpriteText st in tabs)
			AsLanguageManager.Instance.SetFontFromSystemLanguage( st);

		buttonAccept.Text = AsTableManager.Instance.GetTbl_String(37922);
		buttonGiveup.Text = AsTableManager.Instance.GetTbl_String(37923);
		buttonReward.Text = AsTableManager.Instance.GetTbl_String(37924);
	
		tabs[0].Text = AsTableManager.Instance.GetTbl_String(37916);
		tabs[1].Text = AsTableManager.Instance.GetTbl_String(37917);
		tabs[2].Text = AsTableManager.Instance.GetTbl_String(37918);
	
		buttonCashDone.Text = AsTableManager.Instance.GetTbl_String(816);
	}

	public void Close()
	{
		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

		AsHudDlgMgr.dlgPresentState &= ~AsHudDlgMgr.eDlgPresentState.QuestInfo;

		bCallQuestlist = false;
		nowQuestData = null;
		nowSelectItem = -1;
		nowItem = -1;

		foreach( GameObject obj in listItemObject)
			GameObject.DestroyImmediate( obj);

		foreach( GameObject obj in listSelectItemObject)
			GameObject.DestroyImmediate( obj);

		listItemObject.Clear();
		listSelectItemObject.Clear();
		dicSelectItem.Clear();
		dicItem.Clear();

		if( messageBox != null)
			messageBox.Close();

		bLock = false;

		TooltipMgr.Instance.Clear();

		if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_QUEST_ACCEPT_UI));
		
		if( null != m_skillTooltip )
			GameObject.Destroy( m_skillTooltip.gameObject );
	}

	public void buttonClick( ref POINTER_INFO ptr)
	{
		if( bLock == true)
			return;

		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( ptr.targetObj.gameObject == buttonAccept.gameObject)
			{
				if( nowQuestData != null)
					ArkQuestmanager.instance.RequestAcceptQuest( nowQuestData.Info.ID);
			}
			else if( /*ptr.targetObj.gameObject == buttonRefuse.gameObject || ptr.targetObj.gameObject == buttonQuestClose.gameObject ||*/ ptr.targetObj.gameObject == buttonClose.gameObject)
			{
				if( bCallQuestlist == true)
				{
					QuestHolder questHolder = AsHUDController.Instance.m_NpcMenu.questHolder;

					if( questHolder != null)
					{
						if( questHolder.GetQuestMarkType() == QuestMarkType.HAVE)
						{
							UIMessageBroadcaster.instance.SendUIMessage( UIMessageType.UI_MESSAGE_QUESTLIST_SHOW);
						}
						else
						{
							UIMessageBroadcaster.instance.SendUIMessage( UIMessageType.UI_MESSAGE_TALK_RESET);
							UIMessageBroadcaster.instance.SendUIMessage( UIMessageType.UI_MESSAGE_TALK_MENUBUTTON_UPDATE);
						}
					}
					else
					{
						UIMessageBroadcaster.instance.SendUIMessage( UIMessageType.UI_MESSAGE_TALK_RESET);
						UIMessageBroadcaster.instance.SendUIMessage( UIMessageType.UI_MESSAGE_TALK_MENUBUTTON_UPDATE);
					}
				}

				if( bLock == false)
					AsHudDlgMgr.Instance.CloseQuestAccept();
			}
			else if( ptr.targetObj.gameObject == buttonReward.gameObject)
			{
				if( nowQuestData == null)
				{
					Debug.LogWarning( "Quest Complete[Quest data is null]");
					return;
				}

				if( nowQuestData.Info.QuestType == QuestType.QUEST_WANTED)
				{
					AsCommonSender.SendCompleteWantedQuest( nowQuestData.Info.ID);
				}
				else
				{
					if( listSelectItem.Count <= 0)
					{
						ArkQuestmanager.instance.RequestCompleteQuest( nowQuestData.Info.ID);
					}
					else
					{
						if( nowSelectItem == -1)
						{
							messageBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
							AsTableManager.Instance.GetTbl_String(833),
							AsNotify.MSG_BOX_TYPE.MBT_OK,
							AsNotify.MSG_BOX_ICON.MBI_NOTICE);
						}
						else
						{
							ArkQuestmanager.instance.RequestCompleteQuest( nowQuestData.Info.ID, ( byte)nowSelectItem);
						}
					}
				}
			}
			else if( ptr.targetObj.gameObject == buttonGiveup.gameObject)
			{
				AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
				if( messageBox == null)
				{
					messageBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(144), AsTableManager.Instance.GetTbl_String(140),
					AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
					this, "GiveupNowQuestOk", "GiveupNowQuestCancel",
					AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION, false, true);
				}
			}

			else if (ptr.targetObj.gameObject == buttonQuestMove.gameObject)
			{
				if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
					return;

				questMoveRecord = AsTableManager.Instance.GetTbl_QuestMove_Record(nowQuestData.Info.ID);

				if (questMoveRecord == null)
					return;

				if (AsHudDlgMgr.Instance.QuestMoveCoolTime.isCoolTimeActive == true)
				{
					AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(4260));
					return;
				}

				int mapID	= 0;
				Vector3 pos = Vector3.zero;

				if (nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
				{
					mapID	= questMoveRecord.startMapID;
					pos		= questMoveRecord.startPos;
		
				}
				else if (nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
				{

					mapID = questMoveRecord.endMapID;
					pos = questMoveRecord.endPos;
				}
				else
				{
					return;
				}

				AskQuestWarp();
			}
		}
	}

	void AskQuestWarp()
	{
		AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(145), AsTableManager.Instance.GetTbl_String(141),
									 AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(997),
									 this, "QuestWarp", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void QuestWarp()
	{
		// cool time 5 sec
		AsHudDlgMgr.Instance.QuestMoveCoolTime.RemainCooltime(5000.0f, 5000.0f);

		int mapID = 0;
		Vector3 pos = Vector3.zero;

		if (questMoveRecord == null)
			return;

		if (nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
		{
			mapID = questMoveRecord.startMapID;
			pos = questMoveRecord.startPos;

		}
		else if (nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR || nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
		{
			mapID = questMoveRecord.endMapID;
			pos = questMoveRecord.endPos;
		}
		else
		{
			return;
		}

		AsCommonSender.SendQuestWarp(questMoveRecord.questID, mapID, pos);
		AsHudDlgMgr.Instance.CloseQuestAccept();
		AsHudDlgMgr.Instance.CloseQuestBook();
	}

	public void CashClearButtonClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

			if( bLock == false)
			{
				bLock = true;
				string title = AsTableManager.Instance.GetTbl_String(816);
				string msg = AsTableManager.Instance.GetTbl_String(817);
				messageBox = AsNotify.Instance.CashMessageBox( ( long)questClearPrice, title, msg, this, "ClearQuestCash", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL);
			}
		}
	}

	void GiveupNowQuestOk()
	{
		ArkQuestmanager.instance.RequestGiveupQuest( nowQuestData.Info.ID);
		messageBox = null;
	}

	void GiveupNowQuestCancel()
	{
		messageBox = null;
	}

	public void ItemSelectClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			selectObj.SetActiveRecursively( true);
			Vector3 vTargetPos = ptr.targetObj.transform.localPosition;
			selectObj.transform.localPosition = new Vector3( vTargetPos.x, vTargetPos.y, -2.0f);
			nowSelectItem = dicSelectItem[ptr.targetObj.gameObject];
			OpenItemToolTip( listSelectItem[nowSelectItem].ID);
		}
	}

	public void ItemClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			nowItem = dicItem[ptr.targetObj.gameObject];

			if( listRewardItemData[nowItem].type == RewardItemType.ITEM)
				OpenItemToolTip( listRewardItemData[nowItem].id);
			else if( listRewardItemData[nowItem].type == RewardItemType.SKILL)
			{
				AsSkillTooltipInBook tip = GameObject.Instantiate( toolTipInBook) as AsSkillTooltipInBook;
				Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( listRewardItemData[nowItem].id);
				Tbl_SkillLevel_Record skillLvRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( (int)listRewardItemData[nowItem].externalData1, listRewardItemData[nowItem].id);
	
				tip.transform.position = new Vector3( transform.position.x - 8.0f, transform.position.y, transform.position.z - 5.0f);
	
				tip.ID = listRewardItemData[nowItem].id;
				tip.Level = (int)listRewardItemData[nowItem].externalData1;
				tip.Init( skillRecord, skillLvRecord);
				m_skillTooltip = tip;
			}
		}
	}

	void OpenItemToolTip( int _itemID)
	{
		if( messageBox != null)
			return;

		Item itemData = ItemMgr.ItemManagement.GetItem( _itemID);

		if( itemData != null)
		{
			RealItem haveitem = null;
			if( Item.eITEM_TYPE.EquipItem == itemData.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == itemData.ItemData.GetItemType())
				haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem( itemData.ItemData.GetSubType());

			if( null == haveitem)
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, itemData, false, transform.position.z + fOffsetToolTip);
			else
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, haveitem, itemData, false, transform.position.z + fOffsetToolTip);
		}
	}

	public void ShowQuestClearCashBtn( QuestProgressState _qeustProgress, QuestClearType _clearType, int _price)
	{
		QuestClearBtnContoller controller = buttonCashDone.gameObject.GetComponent<QuestClearBtnContoller>();
	
		if( controller == null)
			buttonCashDone.gameObject.SetActiveRecursively( false);
		else
			controller.ShowButton( _qeustProgress, _clearType, _price);
	}

	void SetQuestIcon( QuestType _questType)
	{
		foreach( GameObject icon in questIcons)
			icon.SetActive( false);

		switch( _questType)
		{
		case QuestType.QUEST_MAIN:
			questIcons[0].gameObject.SetActive(true);
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
		case QuestType.QUEST_NPC_DAILY:
			questIcons[5].gameObject.SetActive(true);
			break;
		}
	}

	void SettingPositionByQuestBook()
	{
		AsScreenPositioner screenPositioner = gameObject.transform.parent.GetComponent<AsScreenPositioner>();

		if( screenPositioner != null)
			Component.Destroy( screenPositioner);

		Camera cam = UIManager.instance.rayCamera;

		float screenHalfWidth = cam.orthographicSize * (float)Screen.width / (float)Screen.height;
		float screenStartX = cam.transform.position.x - screenHalfWidth;
		float questBookLeftX = AsHudDlgMgr.Instance.questBook.transform.position.x - AsHudDlgMgr.Instance.questBook.QuestBookWidth * 0.5f;
		float centerHalfWidth = ( questBookLeftX - screenStartX) * 0.5f;
		float questAcceptUIX = questBookLeftX - centerHalfWidth;

		if (AsUtil.IsScreenPadRatio () == true) 
		{
			questAcceptUIX = -500.0f - screenHalfWidth + fBackgroundWidth * 0.5f;
		}


		gameObject.transform.position = new Vector3( questAcceptUIX, gameObject.transform.position.y, -6.0f);
	}

	public void ShowQuestInfo( ArkSphereQuestTool.QuestData _questData, bool _isCallQuestList)
	{
		bLock = false;

		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

		int userLv = AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);

		selectObj.SetActiveRecursively( false);
		gameObject.SetActiveRecursively( true);

		if( messageBox != null)
			messageBox.Close();

		nRewardSpritePosCount = 0;

		if( _isCallQuestList == false)
			SettingPositionByQuestBook();

		SetText();
		
		questClearType = _questData.Info.QuestClearType;
		questClearPrice = _questData.Info.ClearPrice;

		if( _isCallQuestList == false)
			ShowQuestClearCashBtn( _questData.NowQuestProgressState, questClearType, questClearPrice);
		else
			buttonCashDone.gameObject.SetActiveRecursively( false);

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_QUEST_ACCEPT_UI, 0));

		AsHudDlgMgr.dlgPresentState |= AsHudDlgMgr.eDlgPresentState.QuestInfo;

		bCallQuestlist = _isCallQuestList;
		nowSelectItem = -1;
		nowItem = -1;
		listItem = null;
		listSelectItem = null;
		vItemSize = new Vector2( slotItemTMs[0].gameObject.GetComponent<BoxCollider>().size.x, slotItemTMs[0].gameObject.GetComponent<BoxCollider>().size.y);

		nowQuestData = _questData;
		
		QuestProgressState nowProgress = _questData.NowQuestProgressState;

		if( nowProgress == QuestProgressState.QUEST_PROGRESS_NOTHING)
			nowState = QuestAcceptUIState.QuestAccept_Suggest;
		else if( nowProgress == QuestProgressState.QUEST_PROGRESS_IN)
			nowState = QuestAcceptUIState.QuestAccept_Progress;
		else if( nowProgress == QuestProgressState.QUEST_PROGRESS_FAIL)
			nowState = QuestAcceptUIState.QuestAccept_Fail;
		else if( nowProgress == QuestProgressState.QUEST_PROGRESS_CLEAR || nowProgress == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
		{
			if( _isCallQuestList == true)
				nowState = QuestAcceptUIState.QuestAccept_Reward;
			else
			{
				if( nowQuestData.Info.QuestType == QuestType.QUEST_DAILY)
					nowState = QuestAcceptUIState.QuestAccept_Reward;
				else
					nowState = QuestAcceptUIState.QuestAccept_Clear;
			}
		}
		else
		{
			AsHudDlgMgr.Instance.CloseQuestAccept();
			return;
		}

		// quest Level
		List<ConditionLevel> level = _questData.Condition.GetCondition<ConditionLevel>();

		if( _questData.Info.QuestType == QuestType.QUEST_DAILY)
			questLevelLabel.Text = AsTableManager.Instance.GetTbl_String(840);
		else if( level.Count == 0)
			questLevelLabel.Text = "";
		else
			questLevelLabel.Text = "[Lv." + level[0].MinLevel.ToString() + "]";

		// quest location name
		int mapID = _questData.Info.ID / 10000;
		Map map = TerrainMgr.Instance.GetMap( mapID);
		questRegionLabel.Text = map.MapData.GetName();
		questLocationLabel.Text = AsTableManager.Instance.GetTbl_String(_questData.Info.QuestLocationNameID);

		Tbl_QuestReward_Record reward = AsTableManager.Instance.GetTbl_QuestRewardRecord( _questData.Info.ID);

		if( reward != null)
		{
			// show button
			ShowButton( nowState);
			
			StringBuilder sbForTitle = new StringBuilder( nowQuestData.Info.Name);
			sbForTitle.Append( nowQuestData.GetRepeatString());
			
			// set title
			titleLabel.Text = sbForTitle.ToString();
			
			// set explain
			explainLabel.Text = TransColorText( nowQuestData.Info.Explain);
			
			// set achievement
			UpdateAchievementText( nowQuestData.Achievement.GetDatas());
			
			// set quest icon
			SetQuestIcon( _questData.Info.QuestType);
			
			// set exp
			int exp = reward.Reward.ExperiencePoint;
			bool isMaxLv = false;
			double goldExchangeRatio = 0.0f;
			if (AsTableManager.Instance.GetTbl_GlobalWeight_Record(137) != null)
				goldExchangeRatio = AsTableManager.Instance.GetTbl_GlobalWeight_Record(137).Value * 0.001f;

			if (AsTableManager.Instance.GetTbl_GlobalWeight_Record(69) != null)
				if (userLv == (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(69).Value)
					isMaxLv = true;

			if( exp <= 0)
				expSprite.gameObject.SetActiveRecursively( false);
			else
			{
				expSprite.gameObject.SetActiveRecursively( true);
				
				if (isMaxLv == true)
					rewardLabel1.Text = "MAX";
				else
					rewardLabel1.Text = exp.ToString( "#,#0", CultureInfo.InvariantCulture);

				expSprite.gameObject.transform.localPosition = new Vector3( fRewardSpritePosositions[nRewardSpritePosCount++],
																			expSprite.gameObject.transform.localPosition.y,
																			expSprite.gameObject.transform.localPosition.z);
			}

			// gold
			int money   = reward.Reward.Money;
			int exMoney = 0;
			double exGold = 0.0f;
			if( isMaxLv == true )
			{
				exGold = System.Math.Round((double)exp * goldExchangeRatio * 0.1f) * 10.0f;
			}
			
			if( money <= 0)
			{
				goldSprite.gameObject.SetActiveRecursively( false);
			}
			else
			{
				exMoney = (int)exGold;

				goldSprite.gameObject.SetActiveRecursively( true);

				if (isMaxLv == true)
				{
					StringBuilder sbMoney = new StringBuilder(money.ToString("#,#0", CultureInfo.InvariantCulture));
					sbMoney.Append("(");
					sbMoney.Append("+");
					sbMoney.Append(exMoney.ToString("#,#0", CultureInfo.InvariantCulture));
					sbMoney.Append(")");
					rewardLabel2.Text = sbMoney.ToString();
				}
				else
					rewardLabel2.Text = money.ToString("#,#0", CultureInfo.InvariantCulture);


				goldSprite.gameObject.transform.localPosition = new Vector3( fRewardSpritePosositions[nRewardSpritePosCount++],
						goldSprite.gameObject.transform.localPosition.y,
						goldSprite.gameObject.transform.localPosition.z);
			}
			

			// Miracle
			int miracle = reward.Reward.Miracle;
			if( miracle <= 0)
				miracleSprite.gameObject.SetActiveRecursively( false);
			else
			{
				miracleSprite.gameObject.SetActiveRecursively( true);
				rewardLavelMiracle.Text = miracle.ToString();
				miracleSprite.gameObject.transform.localPosition = new Vector3( fRewardSpritePosositions[nRewardSpritePosCount++],
						miracleSprite.gameObject.transform.localPosition.y,
						miracleSprite.gameObject.transform.localPosition.z);
			}

			// skill
			listRewardItemData.Clear();

			foreach( RewardSkill skill in reward.Reward.skill)
				listRewardItemData.Add( new RewardItemData( RewardItemType.SKILL, skill.ID, skill.Lv));

			// item
			foreach( RewardItem item in reward.Reward.Items)
				listRewardItemData.Add( new RewardItemData( RewardItemType.ITEM, item.ID, item.Class, item.Count));

			listItem = reward.Reward.Items;
			listSelectItem = reward.Reward.ItemsSelect;

			AddItemIcon( listRewardItemData, slotItemTMs);

			AddItemSelectIcon( listSelectItem, slotItemTMs);

			if( reward.Reward.designation.Count > 0)
			{
				RewardDesignation designaion = reward.Reward.designation[0];
	
				DesignationData designaionData = AsDesignationManager.Instance.GetDesignation( designaion.designationID);
	
				if( designaionData != null)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append( AsTableManager.Instance.GetTbl_String(880));
					sb.Append( " ");
					sb.Append( AsTableManager.Instance.GetTbl_String(designaionData.name));
					rewardDesignaionLabel.Text = sb.ToString();
				}
			}
			else
				rewardDesignaionLabel.gameObject.SetActiveRecursively( false);
		}
		else
		{
			rewardDesignaionLabel.gameObject.SetActiveRecursively( false);
		}
	}

	void UpdateAchievementText( List<AchBase> _listAch)
	{
		for( int i = 0; i < achieveLabels.Length; i++)
		{
			if( _listAch.Count > i)
			{
				if( _listAch[i].IsComplete == true && _listAch[i].QuestMessageType != QuestMessages.QM_TIME_LIMIT)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append( _listAch[i].GetAchievementString());
					sb.Append( "(");
					sb.Append( AsTableManager.Instance.GetTbl_String(131));
					sb.Append( ")");
					achieveLabels[i].Text = sb.ToString();
				}
				else
					achieveLabels[i].Text = _listAch[i].GetProgressString();
			}
			else
				achieveLabels[i].Text = string.Empty;
		}
	}

	private void AddItemIcon( List<RewardItemData> listItemData, Transform[] slotTransforms)
	{
		dicItem.Clear();
		foreach( GameObject icon in listItemObject)
			GameObject.Destroy( icon);

		listItemObject.Clear();

		if( listItemData.Count <= 0)
			return;

		eCLASS userClass = AsUserInfo.Instance.GetCurrentUserCharacterInfo().eClass;
		int count = 0;
		int posCount = 0;
		foreach( RewardItemData item in listItemData)
		{
			// check class
			if( item.type == RewardItemType.ITEM)
			{
				eCLASS itemClass = (eCLASS)item.externalData1;

				if( itemClass != eCLASS.NONE && itemClass != eCLASS.All && itemClass != userClass)
				{
					count++;
					continue;
				}
			}
			else if( item.type == RewardItemType.SKILL)
			{
				Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( item.id);

				if( skillRecord.Class != userClass)
				{
					count++;
					continue;
				}
			}

			// icon
			GameObject icon = null;

			if( item.type == RewardItemType.ITEM)
			{
				Item itemData = ItemMgr.ItemManagement.GetItem( item.id);
				icon = itemData.GetIcon();
			}
			else if( item.type == RewardItemType.SKILL)
				icon = Resources.Load( AsTableManager.Instance.GetTbl_Skill_Record( item.id).Skill_Icon) as GameObject;

			if( null == icon)
				return;

			GameObject objIconInstantiate = GameObject.Instantiate( icon) as GameObject;
			UISlotItem slotItem = objIconInstantiate.GetComponent<UISlotItem>();
			SimpleSprite simpleSprite = slotItem.iconImg.GetComponent<SimpleSprite>();

			UIButton button = objIconInstantiate.AddComponent<UIButton>();
			button.width = vItemSize.x;
			button.height = vItemSize.y;

			button.SetInputDelegate( ItemClick);
			dicItem.Add( objIconInstantiate, count);

			MeshRenderer meshRenderer = objIconInstantiate.GetComponent<MeshRenderer>();

			if( meshRenderer != null)
				Component.DestroyImmediate( meshRenderer);

			objIconInstantiate.transform.parent = slotParent.transform;
			objIconInstantiate.transform.position = slotTransforms[posCount].position;
			objIconInstantiate.transform.Translate( 0.0f, 0.0f, fOffsetIcon);
			slotItem.coolTime.gameObject.SetActiveRecursively( false);

			float iconScaleX = vItemSize.x / simpleSprite.width;
			float iconScaleY = vItemSize.y / simpleSprite.height;

			objIconInstantiate.transform.localScale = new Vector3( iconScaleX, iconScaleY, 1.0f);

			if( item.type == RewardItemType.ITEM)
			{
				if( (int)item.externalData2 != 1)
					slotItem.itemCountText.Text = ( (int)item.externalData2).ToString();
				else
					slotItem.itemCountText.Text = string.Empty;
			}

			listItemObject.Add( objIconInstantiate);

			count++;
			posCount++;
		}
	}

	private void AddItemSelectIcon( List<RewardItem> listItem, Transform[] slotTransforms)
	{
		dicSelectItem.Clear();

		foreach( GameObject icon in listSelectItemObject)
			GameObject.Destroy( icon);

		listSelectItemObject.Clear();
		nowItem = -1;
		selectObj.SetActiveRecursively( false);

		if( listItemObject.Count > 0)
			return;

		if( listItem.Count <= 0)
			return;

		int count = 0;
		foreach( RewardItem item in listItem)
		{
			Item itemData = ItemMgr.ItemManagement.GetItem( item.ID);

			GameObject icon = itemData.GetIcon();

			if( null == icon)
				return;

			GameObject objIconInstantiate = GameObject.Instantiate( icon) as GameObject;
			UISlotItem slotItem = objIconInstantiate.GetComponent<UISlotItem>();
			SimpleSprite simpleSprite = slotItem.iconImg.GetComponent<SimpleSprite>();

			UIButton button = objIconInstantiate.AddComponent<UIButton>();
			button.width = simpleSprite.width;
			button.height = simpleSprite.height;
			
			button.SetInputDelegate( ItemSelectClick);
			dicSelectItem.Add( objIconInstantiate, count);

			MeshRenderer meshRenderer = objIconInstantiate.GetComponent<MeshRenderer>();

			if( meshRenderer != null)
				Component.DestroyImmediate( meshRenderer);

			objIconInstantiate.transform.parent = slotParent.transform;
			objIconInstantiate.transform.position = slotTransforms[count].position;
			objIconInstantiate.transform.Translate( 0.0f, 0.0f, fOffsetIcon);
			slotItem.coolTime.gameObject.SetActiveRecursively( false);

			float iconScaleX = vItemSize.x / simpleSprite.width;
			float iconScaleY = vItemSize.y / simpleSprite.height;

			objIconInstantiate.transform.localScale = new Vector3( iconScaleX, iconScaleY, 1.0f);

			if( item.Count != 1)
				slotItem.itemCountText.Text = item.Count.ToString();
			else
				slotItem.itemCountText.Text = string.Empty;

			listSelectItemObject.Add( objIconInstantiate);

			count++;
		}
	}

	void ClearQuestCash()
	{
		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
		AsLoadingIndigator.Instance.ShowIndigator( string.Empty);
		ArkQuestmanager.instance.RequestImmediatelyClearQuest( nowQuestData.Info.ID);
	}

	void Cancel()
	{
		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
		bLock = false;
	}

	void OnDisable()
	{
		bLock = false;
		selectObj.SetActiveRecursively( false);
	}

	public void ShowButton( QuestAcceptUIState _state)
	{
		buttonAccept.gameObject.SetActiveRecursively(false);
		buttonReward.gameObject.SetActiveRecursively(false);
		buttonGiveup.gameObject.SetActiveRecursively(false);
		buttonQuestMove.gameObject.SetActiveRecursively(false);

		Tbl_QuestMove_Record questMove = AsTableManager.Instance.GetTbl_QuestMove_Record(nowQuestData.Info.ID);

		switch( nowState)
		{
		case QuestAcceptUIState.QuestAccept_Suggest:
			buttonAccept.gameObject.SetActiveRecursively( true);
			break;
		case QuestAcceptUIState.QuestAccept_Reward:
			buttonReward.gameObject.SetActiveRecursively( true);
			break;
		case QuestAcceptUIState.QuestAccept_Progress:
			if( nowQuestData.Info.QuestType != QuestType.QUEST_WANTED)
				buttonGiveup.gameObject.SetActiveRecursively( true);

			if (questMove != null)
			{
				buttonQuestMove.Text = AsTableManager.Instance.GetTbl_String(2118);
				buttonQuestMove.gameObject.SetActiveRecursively(true);
			}
			break;
		case QuestAcceptUIState.QuestAccept_Fail:
			if( nowQuestData.Info.QuestType != QuestType.QUEST_WANTED)
				buttonGiveup.gameObject.SetActiveRecursively( true);
			break;
		case QuestAcceptUIState.QuestAccept_Clear:
			buttonGiveup.gameObject.SetActiveRecursively( true);

			if (questMove != null)
			{
				buttonQuestMove.Text = AsTableManager.Instance.GetTbl_String(2119);
				buttonQuestMove.gameObject.SetActiveRecursively(true);
			}

			break;
		}
	}

	public void ShowQuestFullMessageBox()
	{
		messageBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(841), this, "AfterProcessQuestFull", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	public string TransColorText( string _text)
	{
		StringBuilder sbText = new StringBuilder( _text);

		foreach( string tag in colorTags)
			sbText.Replace( tag.ToUpper(), tag.ToLower());

		int count = 0;
		foreach( string szTag in colorTags)
		{
			sbText = sbText.Replace( szTag.ToLower(), string.Format( "RGBA( {0:F2}, {1:F2}, {2:F2}, {3:F2})", colorTexts[count].r, colorTexts[count].g, colorTexts[count].b, colorTexts[count].a));
			count++;
		}

		return sbText.ToString();
	}

	public override void ProcessUIMessage( UIMessageObject message)
	{
		switch ( message.messageType)
		{
		case UIMessageType.UI_MESSAGE_QUESTACCEPT_UPDATE_CASHDONE:
			{
				if( nowQuestData != null)
				{
					ShowQuestClearCashBtn( nowQuestData.NowQuestProgressState, nowQuestData.Info.QuestClearType, nowQuestData.Info.ClearPrice);
					UpdateAchievementText( nowQuestData.Achievement.GetDatas());
				}
			}
			break;
		}
	}

	public void UpdateCashDone()
	{
		if( nowQuestData != null)
		{
			ShowQuestClearCashBtn( nowQuestData.NowQuestProgressState, nowQuestData.Info.QuestClearType, nowQuestData.Info.ClearPrice);
			UpdateAchievementText( nowQuestData.Achievement.GetDatas());
		}
	}
}
