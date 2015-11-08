#define INDUN_NEW
//#define USE_GUILD_NEW

using UnityEngine;
using System.Collections;
using System.Globalization;

public enum eMainBtnType
{
	POST = 0,
	SKILL,
	PRODUCT,
	GUILD,
	PARTY,
	SOCIAL,
	SEARCH_STORE,
	RANKING,
	EVENT,
	SYNTHESIS,
	NOTICE,
	SYSTEM,
	LOBBY,
	PVP,
	
	INVEN,
	QUEST,
	
	PET, //$yde
	LEVELUP_BONUS
}

public class AsHudDlgMgr : MonoBehaviour
{
	public enum eDlgPresentState : int
	{
		None = 0x0000,
		QuestInfo = 0x0001,
		NpcMenu = 0x0002,
	};

	public static eDlgPresentState dlgPresentState = eDlgPresentState.None;

	// Dialog
	[HideInInspector]public UIInvenDlg invenDlg;

	GameObject storageObj = null;
	GameObject storeObj = null;
	GameObject cashStoreObj = null;
	GameObject pstoreObj = null;
	GameObject questMiniObj = null;

	[HideInInspector]public PlayerStatusDlg playerStatusDlg = null;
	[HideInInspector]public PlayerPartsOptionDlg playerPartsOptionDlg = null;
	private GameObject playerStatusObject;
	[HideInInspector]public EnchantDlg enchantDlg;
	[HideInInspector]public StrengthenDlg strengthenDlg;
	[HideInInspector]public AsUIRandItem randItemUI;
	[HideInInspector]public UIRandomItemDlg randomItemDlg;
	[HideInInspector]public AsUIItemGetEquipAlram uiItemGetEquipAlarmDlg;
	[HideInInspector]public UIWorldMapSettingDlg uiWorldMapSettingDlg;
	public WorldMapDlg worldMapDlg;
	public SpriteText textWaypointBtn;
	[SerializeField]SpriteText txtChannelBtn;
	[HideInInspector]public ProductionDlg productionDlg;
	[HideInInspector]public AsExpireItemView expireItemViewDlg;
	private body1_SC_ITEM_TIME_EXPIRE m_ReceiveItemTimeExpire = null;
	private bool m_isSendProductionDlgOpen = false;
	public bool SendProductionDlgOpen
	{
		get{return m_isSendProductionDlgOpen;}
		set{m_isSendProductionDlgOpen = value;}
	}

	//$ yde - 20130121
	[HideInInspector] public UIStorageDlg storageDlg = null; // $yde - 20130115
	[HideInInspector] public UIStorageAdditionDlg storageAdditionDlg;
	[HideInInspector] public UIStorageQuantityDlg storageQuantityDlg;
	[HideInInspector] public UIPStoreDlg pstoreDlg = null;
	[HideInInspector] public int useRandItemIdx = 0;
	UIPetSynthesisDlg PetSynthesisDlg{get{return AsPetManager.Instance.PetSynthesisDlg;}}
	//-$ yde - 20130121

	// < ilmeda, 20120914
	[HideInInspector]public UITradeDlg tradeDlg;
	[HideInInspector]public UITradePopup_Gold tradePopup_Gold;
	[HideInInspector]public UITradePopup_Quantity tradePopup_Quantity;
	// ilmeda, 20120914 >

	//	Skill
	#region -SkillShop
	[HideInInspector]public GameObject skillShopObj = null;
	#endregion

	#region -SkillBook
	[HideInInspector]public GameObject skillBookObj = null;
	[HideInInspector]public AsSkillbookDlg skillBookDlg = null;
	#endregion

	// quest
	[HideInInspector]public QuestBookController   questBook = null;
	[HideInInspector]public QuestAcceptUIControll questAccept = null;
	[HideInInspector]public AsQuestMiniViewMgr    questMiniView = null;
	GameObject questBookObject = null;
	GameObject questAcceptObject = null;

	[HideInInspector]public AsNpcStore npcStore   = null;
	[HideInInspector]public AsCashStore cashStore = null;


	#region -PostBox
	[HideInInspector]public GameObject postBoxDlgObj = null;
	[HideInInspector]public AsPostBoxDlg postBoxDlg = null;
	#endregion

	public QuestProgressionMsgMgr questCompleteMsgManager;
	public AsPartyAndQuestToggleMgr partyAndQuestToggleMgr;
	[SerializeField] UIButton petBtn_; //$yde
	public QuestWantedBtnControll wantedQuestBtn;
	[HideInInspector] public AsRecommendInfo recommendInfoDlg = null;

	public UIButton InstantDungeonBtn = null;
	public SpriteText IndunLimitCount = null;
	public GameObject goIndunLimitCountBg = null;

	public int invenPageIdx = 0;

	public GameObject questCompleteMsgDummy = null;

	public GameObject useItemToMonTriggerDummy = null;

	public AsUserEntity cashShopEntity = null;

	//public AsNpcEntity cashShopPetEntity = null;

	#region -ChannelSelectDlg
	private GameObject channelSelectDlg = null;
	public GameObject ChannelSelectDlg
	{
		set	{ channelSelectDlg = value; }
		get	{ return channelSelectDlg; }
	}
	#endregion

	#region -SystemDlg
	private GameObject systemDlg = null;
	#endregion

	#region -LobiRecDlg
	private GameObject lobiRecDlg = null;
	#endregion

	#region -GuildDlg
	private GameObject guildDlg = null;
	public GameObject GuildDlg
	{
		set	{ guildDlg = value; }
		get	{ return guildDlg; }
	}
	private GameObject guildDetailInfoDlg = null;
	public GameObject GuildDetailInfoDlg
	{
		set	{ guildDetailInfoDlg = value; }
		get	{ return guildDetailInfoDlg; }
	}
	#endregion

	#region -DelegateImage
	private GameObject delegateImageDlg = null;
	public GameObject DelegateImageDlg
	{
		set	{ delegateImageDlg = value; }
		get	{ return delegateImageDlg; }
	}
	#endregion

	#region close time
	public float maxCloseTime = 10.0f;
	private float m_fCurTime = 0.0f;
	private AsMenuBtnAction m_MenuBtnAction = null;
	#endregion

	[HideInInspector]
	public AsOptionDlg m_OptionDlg = null;
	[HideInInspector]
	public AsSynthesisMainDlg m_SynthesisDlg = null;
	[HideInInspector]
	public AsSynthesisCosDlg m_SynCosDlg = null;
	[HideInInspector]
	public AsSynthesisOptionDlg m_SynOptionDlg = null;
	[HideInInspector]
	public AsSynthesisEnchantDlg m_SynEnchantDlg = null;
	[HideInInspector]
	public AsDisassembleDlg m_SynDisDlg = null;	
#if INDUN_NEW
	[HideInInspector]
	public AsInstantDungeonDlg_new m_InstantDungeonDlg = null;
#else
	[HideInInspector]
	public AsInstantDungeonDlg m_InstantDungeonDlg = null;
#endif

	private AsRankingDlg rankingDlg = null;
    private AsRankApRewardDlg rankingApRewardDlg = null;
	
	[HideInInspector]
	public AsChatServerDlg m_ChatServerDlg = null;
	
	[HideInInspector]
	public AsReNameDlg 		m_ReNameDlg = null;
	
	
	[HideInInspector]
	public AsMainMenuDlg	m_MainMenuDlg = null;
	
	#region -in game event-
	[HideInInspector]
	public AsInGameEventListDlg m_InGameEventListDlg = null;
	public AsInGameEventAchievementDlg m_InGameEventAchievementDlg = null;
	#endregion

	public enum eBTN_TYPE
	{
		ACTIVE_BTN=0,
		CHAR_INFO_BTN,
		INVEN_BTN,
		MAKING_BTN,
		SKILL_BTN,
		QUEST_BTN,
		COMMUNITY_BTN,
		SYSTEM_BTN,
		MAX_BTN
	}

	public ProductionDlg.ePRODUCTION_STATE productTabIndex = ProductionDlg.ePRODUCTION_STATE.TECHNOLOGY;
	public eITEM_PRODUCT_TECHNIQUE_TYPE productRadioIndex;
	public static eCLASS productRadioClassIndex = eCLASS.DIVINEKNIGHT;
	public int productionItemId = 0;
	public bool productionRandItem = false;

	// common control
	public UIButton[] tabBtn;
	public UIButton	mainBtn;
	public UIButton	invenBtn;
	public UIButton	questBtn;
	public UIButton cashShopBtn;
	public UIButton channelDlgBtn;
	public UIButton portalDlgBtn;
	public UIButton emoticonBtn;
	public UIButton autoCombatBtn;
	public UIButton synDlgBtn;
	public UIButton	lobiRecBtn;
	public SimpleSprite lobiRecImage = null;
	public SimpleSprite lobiIncameraPos = null;
	private float m_fFadeOutTime = 0.0f;
	private bool  m_bFadeOut = true;
	[SerializeField] UIButton lvUpBonusBtn;
	Vector3 lvUpBtnPos;
	private bool m_bIndunDlgCoroutine = false;

	//DOUBLE CLICK
	public float doubleClickDownTime = 0.3f;
	float m_fFirstClickedTime = 0.0f;
	Ray m_FirstClickedRay;
	bool m_bHaveFirstClickedRay = false;

	[HideInInspector]public GameObject newPostMail = null;
	public GameObject newInvenMail = null;
	[HideInInspector]public GameObject newProductMail = null;
	[HideInInspector]public GameObject newSkillMail = null;
	[HideInInspector]public GameObject newSocialMail = null;
	[HideInInspector]public GameObject newGuildMail = null;
	public GameObject newMenuMail = null;
	[SerializeField] SpriteText myGold = null;

	private AsMessageBox m_msgBox;
	public AsNoticesDlg NoticeDlg = null;
	public AsMessageBoxGender GenderDlg = null;
	public AsRankPopup RankPopup = null;

	private CoolTimeGroup questMoveCoolTime = new CoolTimeGroup(0, 5.0f);
	public CoolTimeGroup QuestMoveCoolTime { get { return questMoveCoolTime; } }

	public bool isOpenMsgBox
	{
		get
		{
			if( null == m_msgBox)
				return false;

			return m_msgBox.gameObject.active;
		}
	}

	public void SetMsgBox( AsMessageBox _msg)
	{
		if( null != m_msgBox)
		{
			GameObject.DestroyObject( m_msgBox.gameObject);
		}

		m_msgBox = _msg;
	}

	public void CloseMsgBox()
	{
		if( null != m_msgBox)
		{
			GameObject.DestroyObject( m_msgBox.gameObject);
		}
	}

	// static
	private static AsHudDlgMgr ms_kIstance = null;

	public static AsHudDlgMgr Instance
	{
		get	{ return ms_kIstance; }
	}

	private System.Collections.Generic.Dictionary<int, GameObject> dicQuestRelationUI = new System.Collections.Generic.Dictionary<int, GameObject>();

	public void Init( bool bSendTradeCancel = true)
	{
		// < ilmeda, 20120914
		CloseTradeDlg( bSendTradeCancel);
		CloseTradeGoldDlg();
		CloseTradeQuantityDlg();
		// ilmeda, 20120914 >

		CloseInven();
		
		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();

		ClosePlayerStatus();
		CloseEnchantDlg();
		CloseStrengthenDlg();

		//	Skill
		CloseSkillshop();
		CloseSkillBook();
		//	PostBox
		ClosePostBoxDlg();

		CloseOptionDlg();
		CloseInstantDungeonDlg();
		AsPvpDlgManager.Instance.ClosePvpDlg();
		CloseInstantDungeonDlg();

		AsPartyManager.Instance.InitUI();
		AsSocialManager.Instance.InitUI();



		randomItemDlg.Close();

		AsQuickSlotManager.Instance.Init();
		//#14635
		AsSocialManager.Instance.CloseAllSocailUI();

		if( true == IsOpenNpcStore)
			CloseNpcStore();

		if( true == IsOpenCashStore)
			CloseCashStore();

		if( true == IsOpenQuestBook)
			CloseQuestBook();

		if( null != channelSelectDlg)
			CloseChannelSelectDlg();

		if( null != systemDlg)
			CloseSystemDlg();

		if( null != lobiRecDlg)
			CloseLobiRecDlg();

		if( null != guildDlg)
			CloseGuildDlg();

		if( null != productionDlg)
			CloseProductionDlg();

		if( null != wantedQuestBtn && ArkQuestmanager.instance.IsAcceptedWantedQuest() == false)
			wantedQuestBtn.gameObject.SetActiveRecursively( false);

		if(!AsLobiSDKManager.Instance.IsCapturing) lobiRecBtn.gameObject.SetActiveRecursively(false);

		if( petBtn_  != null) petBtn_.gameObject.SetActive( false); //$yde
		
		if( null != m_ChatServerDlg)
			CloseChatServerDlg();

		if( null != m_InGameEventListDlg)
			CloseInGameEventListDlg();

		if (null != m_InGameEventAchievementDlg)
			CloseInGameEventAchieveDlg();
		
		if( null != m_ReNameDlg)
			CloseReNameDlg();
		
		if( null != m_MainMenuDlg)
			CloseMainMenuDlg();
		
		_Init_IndunBtn();

		if( ItemMgr.HadItemManagement.Inven.isNewReceiveItems)
			SetNewInvenImg( true);
		else
			SetNewInvenImg( false);

		CheckNewMenuImg();

		CreateQuestRelationUIRecord();
		UpdateQuestRelationUI();

		if( null != TooltipMgr.Instance && true == TooltipMgr.Instance.IsOpenAny())
			TooltipMgr.Instance.Clear();

		m_bIndunDlgCoroutine = false;
	}

	public bool CheckOpenUI()
	{
		bool isClose = false;
		
		if( true == IsOpenTrade)
		{
			CloseTradeDlg( true);
			isClose = true;
		}
		
		if( true == AsNotify.Instance.IsOpenMessageBox())
		{
			AsNotify.Instance.CloseAllMessageBox();
			return true;
		}
		
		if( true == AsNotify.Instance.IsOpenDeathDlgFriend)
		{
			AsNotify.Instance.CloseDeathDlgFriend();
			isClose = true;
		}

		if( true == isOpenMsgBox)
		{
			CloseMsgBox();
			isClose = true;
		}

		if( true == IsOpenInven)
		{
			CloseInven();
			isClose = true;
		}

		if( true == IsOpenPlayerStatus)
		{
			ClosePlayerStatus();
			isClose = true;
		}
		
		if( true == IsOpenEnchantDlg)
		{
			CloseEnchantDlg();
			isClose = true;
		}
		
		if( true == IsOpenStrengthenDlg)
		{
			CloseStrengthenDlg();
			isClose = true;
		}

		if( true == IsOpenedSkillshop)
		{
			CloseSkillshop();
			isClose = true;
		}
		
		if( true == IsOpenedSkillBook)
		{
			CloseSkillBook();
			isClose = true;
		}
		
		if( true == IsOpenedPostBox)
		{
			ClosePostBoxDlg();
			isClose = true;
		}

		if( true == IsOpenOptionDlg)
		{
			CloseOptionDlg();
			isClose = true;
		}
		
		if( true == IsOpenInstantDungeonDlg)
		{
			CloseInstantDungeonDlg();
			isClose = true;
		}
		
		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
		{
			AsPvpDlgManager.Instance.ClosePvpDlg();
			isClose = true;
		}
		
		if( true == IsOpenInstantDungeonDlg)
		{
			CloseInstantDungeonDlg();
			isClose = true;
		}

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyDlg || true == AsPartyManager.Instance.PartyUI.IsOpenPartyList
			|| true == AsPartyManager.Instance.PartyUI.IsOpenPartyMatching || true == AsPartyManager.Instance.PartyUI.IsOpenPartyCreate || true == AsPartyManager.Instance.PartyUI.IsOpenPartyPRDlg)
		{
			AsPartyManager.Instance.PartyUI.ClosePartyList();
			isClose = true;
		}
		
		if( null != AsPartyManager.Instance.PartyInfoDlg)
		{
			AsPartyManager.Instance.ClosePartyInfoDlg();
			isClose = true;
		}
		
		if( null != AsPartyManager.Instance.GetPartyDiceDlg())
		{
			if( true == AsPartyManager.Instance.GetPartyDiceDlg().IsUseing)
			{
				AsPartyManager.Instance.ClosePartyDiceDlg();
				isClose = true;
			}
		}

		if( true == AsSocialManager.Instance.IsOpenSocialDlg())
		{
			AsSocialManager.Instance.CloseAllSocailUI();
			isClose = true;
		}

		if( true == isOpenRandomItemDlg)
		{
			CloseRandItemUI();
			isClose = true;
		}

		if( true == IsOpenNpcStore)
		{
			CloseNpcStore();
			isClose = true;
		}

		if( true == IsOpenCashStore)
		{
			if( false == cashStore.IsLockInput )
			{
				if( null != cashStore)
					cashStore.Close();
			}
			isClose = true;
		}

		if( true == IsOpenQuestBook)
		{
			CloseQuestBook();
			isClose = true;
		}
		
		if( true == IsOpenQuestAccept)
		{
			CloseQuestAccept();
			isClose = true;
		}
		
		if( true == IsOpenPStore)
		{
			ClosePStore();
			isClose = true;
		}
		
		if( true == IsOpenRankingDlg)
		{
			if( null != RankPopup)
			{
				RankPopup.Close();
				isClose = true;
			}
			else
			{
				CloseRankingDlg();
				isClose = true;
			}
		}
		
		if( true == IsOpenWorldMapDlg)
		{
			CloseWaypointMapDlg();
			isClose = true;
		}

		if( null != channelSelectDlg)
		{
			CloseChannelSelectDlg();
			isClose = true;
		}

		if( null != systemDlg)
		{
			if( null != NoticeDlg)
			{
				NoticeDlg.Close();
				isClose = true;
			}
			else if( null != GenderDlg)
			{
				GenderDlg.Close();
				isClose = true;
			}
			else
			{
				CloseSystemDlg();
				isClose = true;
			}
		}

		if( null != lobiRecDlg)
		{
			CloseLobiRecDlg();
			isClose = true;
		}

		if( null != NoticeDlg)
		{
			NoticeDlg.Close();
			isClose = true;
		}
		
		if( null != GenderDlg)
		{
			//GenderDlg.Close();
			GenderDlg.OnCloseBtn();
			isClose = true;
		}

		if( null != guildDlg)
		{
			CloseGuildDlg();
			isClose = true;
		}

		if( null != productionDlg)
		{
			CloseProductionDlg();
			isClose = true;
		}
		
		if( true == AsChatFullPanel.Instance.IsOpen())
		{
			AsChatFullPanel.Instance.Close();
			isClose = true;
		}
		
		if( null != DelegateImageDlg)
		{
			AsDelegateImageDlg dlg = delegateImageDlg.GetComponentInChildren<AsDelegateImageDlg>();
			if( null != dlg)
				dlg.Close();
			CloseDelegateImageDlg();
			isClose = true;
		}
		
		if( true == AsGameGuideManager.Instance.IsOpenGameGuideListDlg())
		{
			AsGameGuideManager.Instance.CloseGameGuideListDlg();
			isClose = true;
		}
		
		if( true == AsGameGuideManager.Instance.IsOpenGameGuide())
		{
			AsGameGuideManager.Instance.CloseGameGuide();
			isClose = true;
		}
		
		if( true == _isOpenNpcMenu())
		{
			_CloseNpcMenu();
			isClose = true;
		}
		
		if( null != TooltipMgr.Instance && true == TooltipMgr.Instance.IsOpenAny())
		{
			TooltipMgr.Instance.Clear();
			isClose = true;
		}
		
		if( true == AsHUDController.Instance.m_UserMenu.isVisible)
		{
			AsHUDController.Instance.m_UserMenu.Close();
			isClose = true;
		}
		
		if( true == AsEmotionManager.Instance.IsOpenEmotionPanel())
		{
			AsEmotionManager.Instance.CloseEmoticonPanel();
			isClose = true;
		}
		
		if( null != AsQuickSlotManager.Instance && true == AsQuickSlotManager.Instance.isOpenSkillTooltip )
		{
			AsQuickSlotManager.Instance.CloseSkillTooltip();
			isClose = true;
		}
		
		if( null != m_ChatServerDlg)
		{
			CloseChatServerDlg();
			isClose = true;
		}
		
		if( null != m_ReNameDlg )
		{
			CloseReNameDlg();
			isClose = true;
		}
		
		if( null != m_MainMenuDlg )
		{
			CloseMainMenuDlg();
			isClose = true;
		}
		
		return isClose;
	}

	void CreateQuestRelationUIRecord()
	{
		dicQuestRelationUI.Clear();

		eCLASS nowClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);

		System.Collections.Generic.Dictionary<int, PrepareOpenUIType> dic = AsTableManager.Instance.GetTbl_PrepareOpenUI( nowClass);

		GameObject objUI = null;
		foreach( System.Collections.Generic.KeyValuePair<int, PrepareOpenUIType> pair in dic)
		{
			switch( pair.Value)
			{
					
			case PrepareOpenUIType.QUEST_BOOK_BTN:
				objUI = questBtn.gameObject;
				break;
			case PrepareOpenUIType.CHANNEL:
				objUI = channelDlgBtn.gameObject;
				break;
			case PrepareOpenUIType.WARP:
				objUI = portalDlgBtn.gameObject;
				break;
			case PrepareOpenUIType.EMOTICON:
				objUI = emoticonBtn.gameObject;
				break;
			case PrepareOpenUIType.INVENTORY:
				objUI = invenBtn.gameObject;
				break;
			case PrepareOpenUIType.AUTO_COMBAT:
				objUI = autoCombatBtn.gameObject;
				break;
			case PrepareOpenUIType.PVP:
				objUI = null;	// this button move to AsMainMenuDlg
				break;
			default:
				continue;
			}

			dicQuestRelationUI.Add( pair.Key, objUI);
		}
	}

	public void UpdateQuestRelationUI( int _questID = -1)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.UpdateQuestRelationUI(_questID);
		}
		
		if( _questID == -1)
		{
			foreach( System.Collections.Generic.KeyValuePair<int, GameObject> keyPair in dicQuestRelationUI)
			{
				if( keyPair.Value == null )
					continue;
				
				if( ArkQuestmanager.instance.IsNothingQuest( keyPair.Key) == true)
					keyPair.Value.SetActiveRecursively( false);
				else
				{
					if( keyPair.Value.active == false)
					{
						keyPair.Value.SetActiveRecursively( true);
						ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", keyPair.Value.transform, Vector3.zero);
					}
				}
			}
		}
		else
		{
			if( !dicQuestRelationUI.ContainsKey( _questID))
				return;
			
			if( dicQuestRelationUI[_questID] == null )
				return;

			if( ArkQuestmanager.instance.IsNothingQuest( _questID) == true)
				dicQuestRelationUI[_questID].SetActiveRecursively( false);
			else
			{
				if( dicQuestRelationUI[_questID].active == false)
				{
					dicQuestRelationUI[_questID].SetActiveRecursively( true);
					ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", dicQuestRelationUI[_questID].transform, Vector3.zero);
				}
			}
		}
	}

	void OnEnable()
	{
		if( 0 < AsRankChangeAlarmManager.Instance.Count)
			StartCoroutine( "PromptAlarm", false);

#if INDUN_NEW
		goIndunLimitCountBg.SetActive( false);
#endif
	}

	//$yde
	void OnDisable()
	{
		RemoveAlarm();
		CloseStorage();
		ClosePStore();
	}

	// Dialog function

	private void AllDlgClose()
	{
		CloseInven();
	}

	public UIButton GetTabBtn( int iIndex)
	{
		if( null == tabBtn)
		{
			Debug.LogError( "AsHudDlgMgr::GetTabBtn() [ null == tabBtn ] ");
			return null;
		}

		if( tabBtn.Length <= iIndex)
		{
			Debug.LogError( "AsHudDlgMgr::GetTabBtn() [ tabBtn.Length <= iIndex ]");
			return null;
		}

		return tabBtn[iIndex];
	}

	// expire item view
	public void ReceiveItemTimeExpire( body1_SC_ITEM_TIME_EXPIRE dataRoot)
	{
		m_ReceiveItemTimeExpire = dataRoot;
		UpdateItemTimeExpire();
	}

	protected void UpdateItemTimeExpire()
	{
		if( null == m_ReceiveItemTimeExpire)
			return;

		if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState)
		{
			if( false == IsOpenExpireItemViewDlg)
				OpenExpireItemViewdlg( m_ReceiveItemTimeExpire);
			else
				expireItemViewDlg.AddList( m_ReceiveItemTimeExpire);

			m_ReceiveItemTimeExpire = null;
		}
	}

	public void OpenExpireItemViewdlg( body1_SC_ITEM_TIME_EXPIRE dataRoot)
	{
		if( null == dataRoot)
		{
			Debug.LogError( "AsHudDlgMgr::OpenExpireItemViewdlg()[ body1_SC_ITEM_TIME_EXPIRE == datas ]");
			return;
		}

		if( null != expireItemViewDlg)
		{
			Debug.LogWarning( "AsHudDlgMgr::OpenExpireItemViewdlg()[ null != expireItemViewDlg");
			return;
		}

		GameObject objIns  = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_ItemTimeLimited");

		expireItemViewDlg = objIns.GetComponent<AsExpireItemView>();
		if( null == expireItemViewDlg)
		{
			GameObject.DestroyObject( objIns);
			AsUtil.ShutDown( "AsHudDlgMgr::OpenExpireItemViewdlg()[ null == AsExpireItemView ]");
		}

		expireItemViewDlg.Open( dataRoot);
	}

	public void CloseExpireItemViewDlg()
	{
		if( null == expireItemViewDlg)
			return;

		expireItemViewDlg.Close();

		GameObject.DestroyObject( expireItemViewDlg.gameObject);
	}

	public bool IsOpenExpireItemViewDlg
	{
		get	{ return ( null != expireItemViewDlg); }
	}

	//productionDlg

	public bool IsOpenProductionDlg
	{
		get
		{
			if( null == productionDlg)
				return false;

			return productionDlg.gameObject.active;
		}
	}

	public void OpenProductionDlg( body1_SC_ITEM_PRODUCT_INFO _data)
	{
		if( false == m_isSendProductionDlgOpen)
			return;

		if( null != productionDlg)
		{
			Debug.LogWarning( "AsHudDlgMgr::OpenProductionDlg()[ null != productionDlg");
			return;
		}

		GameObject objIns  = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Production_new");

		productionDlg = objIns.GetComponent<ProductionDlg>();
		if( null == productionDlg)
		{
			GameObject.DestroyObject( objIns);
			AsUtil.ShutDown( "AsHudDlgMgr::OpenProductionDlg()[ null == productionDlg ]");
		}

		productionDlg.Open( _data);

		m_isSendProductionDlgOpen = false;
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
	}

	public void CloseProductionDlg()
	{
		if( null == productionDlg)
			return;

		productionDlg.Close();

		GameObject.DestroyObject( productionDlg.gameObject);
	}


	//worldMapDlg
	public bool IsOpenWorldMapDlg
	{
		get
		{
			if( null == worldMapDlg)
				return false;

			if( false == worldMapDlg.gameObject.active)
				return false;

			return worldMapDlg.isBackGround;
		}
	}

	public void OpenWorldMapDlg()
	{
		worldMapDlg.gameObject.SetActiveRecursively( true);
		worldMapDlg.OpenWorldMap();
	}

	public void CloseWorldMapDlg()
	{
		worldMapDlg.CloseWorldMap();
	}

	public void OpenWorldMapRadioBtn()
	{
		worldMapDlg.ShowWorldMapBtn( true);
	}

	public bool IsWaypointMapDlg
	{
		get
		{
			if( null == worldMapDlg)
				return false;

			if( false == worldMapDlg.gameObject.active)
				return false;

			return 	WorldMapDlg.eDLG_STATE.WAYPOINT_AREA_DLG == worldMapDlg.getCurDlgState ||
					WorldMapDlg.eDLG_STATE.WAYPOINT_WORLD_DLG == worldMapDlg.getCurDlgState ;
		}
	}

	public void DeathPlayerUIState()
	{
		AsHudDlgMgr.Instance.CloseWaypointMapDlg();
		AsHudDlgMgr.Instance.CloseWorldMapDlg();
		AsHudDlgMgr.Instance.OpenWorldMapRadioBtn();
	}

	public void OpenWaypointMapDlg()
	{
		AsChatFullPanel.Instance.Close();

		if( null == TerrainMgr.Instance)
			return;

		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;
		
		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		Map _map = TerrainMgr.Instance.GetCurrentMap();
		if( null == _map)
			return;

		if( eMAP_TYPE.Tutorial == _map.MapData.getMapType)
			return;

		if( true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg)
			AsHudDlgMgr.Instance.CloseWorldMapDlg();

		if( false == AsUserInfo.Instance.isReceiveWaypointList)
			AsCommonSender.SendWayPoint();

		worldMapDlg.gameObject.SetActiveRecursively( true);
		worldMapDlg.OpenWaypointMap( AsHUDController.Instance.targetInfo.getCurTargetEntity);

		CollapseMenuBtn();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_MINIMAP_WARP));
	}
	
	public void CloseWaypointMapDlg()
	{
		worldMapDlg.CloseWaypointMap();
	}

	public bool IsOpenInven
	{
		get
		{
			if( null == invenDlg)
				return false;

			return invenDlg.gameObject.active;
		}
	}

	public bool IsOpenStorage // $yde - 20130115
	{
		get
		{
			if( null == storageDlg)
				return false;
			return storageDlg.gameObject.active;
		}
	}

	public bool IsOpenQuestAccept
	{
		get
		{
			if( null == questAcceptObject)
				return false;

			return questAcceptObject.gameObject.active;
		}
	}

	public bool IsOpenPStore
	{
		get
		{
			if( null == pstoreDlg)
				return false;

			return pstoreDlg.gameObject.active;
		}
	}

	public bool IsOpenPStoreGoodsBox
	{
		get
		{
			if( null == pstoreDlg)
				return false;

			return pstoreDlg.GoodsBox;
		}
	}

	public bool IsOpenNpcStore
	{
		get
		{
			if( storeObj == null)
				return false;
			else
				return true;
		}
	}

	public bool IsOpenCashStore
	{
		get
		{
			if( cashStoreObj == null)
				return false;

			return true;
		}
	}

	public bool IsOpenQuestMiniView
	{
		get
		{
			if( questMiniObj == null)
				return false;

			return true;
		}
	}

	public bool IsOpenEnchantDlg
	{
		get
		{
			if( null == enchantDlg)
				return false;

			return enchantDlg.gameObject.active;
		}
	}

	public bool IsOpenStrengthenDlg
	{
		get
		{
			if( null == strengthenDlg)
				return false;

			return strengthenDlg.gameObject.active;
		}
	}

	public bool IsOpenRandItemUI
	{
		get
		{
			if( null == randItemUI)
				return false;

			return randItemUI.gameObject.active;
		}
	}

	public bool IsOpenRandItemIsInven
	{
		get
		{
			if( null == randItemUI)
				return false;

			if( false == randItemUI.gameObject.active)
				return false;

			return randItemUI.isCanUseDlg;
		}
	}

	public bool IsOpenPlayerStatus
	{
		get
		{
			if( null == playerStatusDlg)
				return false;

			return playerStatusDlg.gameObject.active;
		}
	}

	public bool IsPlayerPartsOptionDlg
	{
		get
		{
			if( null == playerPartsOptionDlg)
				return false;

			return playerPartsOptionDlg.gameObject.active;
		}
	}
	

	// < ilmeda, 20120914
	public bool IsOpenTrade
	{
		get
		{
			if( null == tradeDlg)
				return false;

			return tradeDlg.gameObject.active;
		}
	}

	public bool IsOpenTradeRequestMsgBox
	{
		get
		{
			if( true == AsTradeManager.Instance.isOpenRequestWaitMsgBox()
				|| true == AsTradeManager.Instance.isOpenRequestChoiceMsgBox())
				return true;

			return false;
		}
	}
	// ilmeda, 20120914 >

	public void OpenRandomItemDlg( RealItem _realItem)
	{
		if( null == _realItem)
			return;

		if( -1 >= ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot())
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}


		Tbl_Lottery_Record _randomRecord = AsTableManager.Instance.GetTbl_Lottery_Record( _realItem.item.ItemData.m_iItem_Rand_ID);
		if( null == _randomRecord)
			return;

		if( true == _randomRecord.needEffect)
		{
			_realItem.SendUseItem( true);
			return;
		}

		randomItemDlg.gameObject.active = true;
		randomItemDlg.Open( _realItem);
	}

	public bool isOpenRandomItemDlg
	{
		get
		{
			if( null == randomItemDlg)
				return false;

			return randomItemDlg.isOpen;
		}
	}


	public bool isOpenWorldMapSettingDlg
	{
		get
		{
			if( null == uiWorldMapSettingDlg)
				return false;

			return true;
		}
	}

	public void OpenWorldMapSettingDlg()
	{
		GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/Worldmap/MessageBox_Minimap");

		if( null == obj)
			return;

		uiWorldMapSettingDlg = obj.GetComponent<UIWorldMapSettingDlg>();
		if( null == uiWorldMapSettingDlg)
		{
			GameObject.Destroy( obj);
			return;
		}

		uiWorldMapSettingDlg.Open();
	}

	public void CloseWorldMapSettingDlg()
	{
		if( null == uiWorldMapSettingDlg)
			return;

		GameObject.Destroy( uiWorldMapSettingDlg.gameObject);
		uiWorldMapSettingDlg = null;
	}


	public void OpenItemGetEquipAlarmDlg( sITEM _sitem)
	{
		if( null == _sitem)
			return;

		DeleteItemGetEquipAlramDlg();

		GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_ItemGetAlram");

		if( null == obj)
			return;

		uiItemGetEquipAlarmDlg = obj.GetComponent<AsUIItemGetEquipAlram>();
		if( null == uiItemGetEquipAlarmDlg)
		{
			DeleteItemGetEquipAlramDlg();
			return;
		}

		uiItemGetEquipAlarmDlg.Open( _sitem, AsUserInfo.Instance.GetCurrentUserEntity());
	}

	public void DeleteItemGetEquipAlramDlg()
	{
		if( null == uiItemGetEquipAlarmDlg)
			return;

		GameObject.Destroy( uiItemGetEquipAlarmDlg.gameObject);
		uiItemGetEquipAlarmDlg = null;
	}

	public bool isOpenItemGetEquipAlarmDlg
	{
		get
		{
			if( null == uiItemGetEquipAlarmDlg)
				return false;

			return true;
		}
	}

	public bool IsOpenedSkillshop
	{
		get	{ return ( null != skillShopObj) ? true : false; }
	}

	public bool IsOpenedPostBox
	{
		get	{ return ( null != postBoxDlgObj) ? true : false; }
	}

	public bool IsOpenedSkillBook
	{
		get	{ return ( null != skillBookObj) ? true : false; }
	}

	public bool IsDontMoveState
	{
		get	{  return ( true == IsOpenTrade)
				|| ( true == IsOpenEnchantDlg)
				|| ( true == IsOpenStrengthenDlg)
				|| ( true == IsOpenedPostBox)
				|| ( true == IsOpenStorage)
				|| ( true == IsOpenNpcStore)
				|| ( true == IsOpenCashStore)
				|| ( true == IsOpenPStore)
				|| ( true == AsUserInfo.Instance.isProductionProgress)
				|| ( ePStoreState.User_Opening == AsPStoreManager.Instance.storeState)
				|| ( ePStoreState.Another_Opened == AsPStoreManager.Instance.storeState)
				|| ( ePStoreState.User_Standby == AsPStoreManager.Instance.storeState)
				|| ( ePStoreState.User_Folded == AsPStoreManager.Instance.storeState)
				|| ( true == IsOpenSynthesisDlg)
				|| ( true == IsOpenedSkillshop)
				|| ( true == IsOpenInstantDungeonDlg)
				|| ( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				|| ( true == IsOpenSynEnchantDlg)
				|| ( true == IsOpenSynOptionDlg)
				|| ( true == IsOpenSynDisDlg)
				|| ( true == IsOpenSynCosDlg)
				|| ( null != AsPetManager.Instance.PetUpgradeDlg )
				|| ( null != AsPetManager.Instance.PetSynthesisDlg )
					? true : false; }
	}

	public bool IsOpenOptionDlg
	{
		get
		{
			if( null == m_OptionDlg)
				return false;

			return m_OptionDlg.gameObject.active;
		}
	}
	
	public bool IsOpenChatServerDlg
	{
		get
		{
			if( null == m_ChatServerDlg)
				return false;
			return m_ChatServerDlg.gameObject.active;
		}
	}
	
	public bool IsOpenReNameDlg
	{
		get
		{
			if( null == m_ReNameDlg )
				return false;
			return m_ReNameDlg.gameObject.activeSelf;
		}
	}
	
	public AsReNameDlg	ReNameDlg
	{
		get{return m_ReNameDlg;}
	}
	
	public bool IsOpenMainMenuDlg
	{
		get
		{
			if( null == m_MainMenuDlg )
				return false;
			return m_MainMenuDlg.gameObject.activeSelf;
		}
	}
	
	public AsMainMenuDlg MainMenuDlg
	{
		get{return m_MainMenuDlg;}
	}
	
	public bool IsOpenSystemDlg
	{
		get { return systemDlg != null; }
	}

	public bool IsOpenLobiRecDlg
	{
		get { return lobiRecDlg != null; }
	}

	public bool IsOpenQuestBook
	{
		get	{ return questBook != null; }
	}

	public bool IsOpenSynthesisDlg
	{
		get
		{
			if( null == m_SynthesisDlg)
				return false;

			return m_SynthesisDlg.gameObject.active;
		}
	}

	public bool IsOpenInstantDungeonDlg
	{
		get
		{
			if( null == m_InstantDungeonDlg)
				return false;

			return m_InstantDungeonDlg.gameObject.active;
		}
	}

	public bool IsOpenRankingDlg
	{
		get
		{
			if( null == rankingDlg)
				return false;

			return rankingDlg.gameObject.active;
		}
	}

	public bool IsOpenInGameEventListDlg
	{
		get
		{
			if( null == m_InGameEventListDlg)
				return false;

			return true;
		}
	}

	public bool IsOpenInGameEventAchievementDlg
	{
		get
		{
			if( null == m_InGameEventAchievementDlg)
				return false;

			return true;
		}
	}

	public void OpenInven()
	{
		if( true == IsOpenInven)
			return;

		if( true == IsOpenOptionDlg)
			CloseOptionDlg();

		if( true == IsOpenedSkillBook)
			CloseSkillBook();

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();

		if( true == IsOpenQuestBook)
			CloseQuestBook();
		
		if( true == IsOpenSynthesisDlg )		
			CloseSynthesisDlg();

		if(BonusManager.Instance.CheckLevelUpOpened() == true)
			BonusManager.Instance.CloseLevelUpWindow();

		// need code
		GameObject objIns  = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Inven");

		invenDlg = objIns.GetComponent<UIInvenDlg>();
		if( null == invenDlg)
		{
			GameObject.DestroyObject( objIns);
			AsUtil.ShutDown( "AsHudDlgMgr::OpenInven()[ null == invenDlg ]");
		}

		invenDlg.Open();
		//SetCloseBtnActive( true);
		DeleteRewardMsgBox();
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_INVENTORY));
	}

	public void DeleteRewardMsgBox()
	{
		if( questCompleteMsgDummy == null)
			return;

		// delete child object
		if( questCompleteMsgDummy.transform.childCount > 0)
		{
			Transform tmPreviousChild = questCompleteMsgDummy.transform.GetChild(0);

			if( tmPreviousChild != null)
			{
				tmPreviousChild.transform.parent = null;
				GameObject.DestroyImmediate( tmPreviousChild.gameObject);
			}
		}
	}

	public void OpenQuestBook()
	{
		if( questBook == null)
		{
			questBookObject = Instantiate( Resources.Load( "UI/AsGUI/GUI_QuestBook")) as GameObject;

			if( questBookObject != null)
				questBook = questBookObject.GetComponentInChildren<QuestBookController>();

			if( true == IsOpenInven)
				CloseInven();

			if( questBook != null)
				questBook.Show();
		}
		else
		{
			if( true == IsOpenInven)
				CloseInven();

			questBook.Show();
		}
	}

	public void OpenQuestAcceptUI( ArkSphereQuestTool.QuestData _questData, bool _isCallQuestList)
	{
		if( questAccept == null)
		{
			questAcceptObject = Instantiate( Resources.Load( "UI/AsGUI/GUI_QuestAcceptUI")) as GameObject;

			if( questAcceptObject != null)
			{
				questAccept = questAcceptObject.GetComponentInChildren<QuestAcceptUIControll>();

				if( questAccept != null)
					questAccept.ShowQuestInfo( _questData, _isCallQuestList);
			}
		}
		else
		{
			if( questAccept != null)
				questAccept.ShowQuestInfo( _questData, _isCallQuestList);
		}
	}

	public void OpenStorage() // $yde - 20130115
	{
		// all dlg close
		Init();

		storageObj = Instantiate( Resources.Load( "UI/AsGUI/GUI_Storage")) as GameObject;
		if( storageObj != null)
		{
			storageObj.transform.parent = AsHUDController.Instance.gameObject.transform;
			storageDlg = storageObj.GetComponentInChildren<UIStorageDlg>();
		}

		if( null == storageDlg)
		{
			Debug.LogError( "null == storageDlg");
			return;
		}

		if( true == IsOpenOptionDlg)
			CloseOptionDlg();

		if( true == IsOpenedSkillBook)
			CloseSkillBook();

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		storageDlg.gameObject.SetActiveRecursively( true);
		storageDlg.Open();
		//SetCloseBtnActive( true);
	}

	public void OpenPStore()
	{
		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		pstoreObj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_PersonalStore");
		if( pstoreObj != null)
		{
			pstoreObj.transform.parent = AsHUDController.Instance.gameObject.transform;
			pstoreDlg = pstoreObj.GetComponentInChildren<UIPStoreDlg>();
		}

		if( null == pstoreDlg)
		{
			Debug.LogError( "null == pstoreDlg");
			return;
		}

		if( true == IsOpenOptionDlg)
			CloseOptionDlg();

		if( true == IsOpenedSkillBook)
			CloseSkillBook();

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenProductionDlg)
			CloseProductionDlg();

		if( true == IsOpenQuestBook)
			CloseQuestBook();

		if( null != channelSelectDlg)
			CloseChannelSelectDlg();

		if( null != systemDlg)
			CloseSystemDlg();

		if( null != lobiRecDlg)
			CloseLobiRecDlg();

		if( null != guildDlg)
			CloseGuildDlg();

		AsSocialManager.Instance.SocialUI.CloseSocailDlg();

		pstoreDlg.gameObject.SetActiveRecursively( true);
		pstoreDlg.Open();

		//SetCloseBtnActive( true);
	}

	public void OpenNpcStore(int _sessionID, int _npcID, eCLASS _class)
	{
		if( storeObj == null && !IsOpenCashStore)
		{
			storeObj = GameObject.Instantiate( Resources.Load( "UI/AsGUI/GUI_NpcStore")) as GameObject;
			if( storeObj != null)
			{
				this.npcStore = storeObj.GetComponentInChildren<AsNpcStore>();
				npcStore.SetNpcSessionID(_sessionID);
				npcStore.InitilizeStore( _npcID, _class);

				// open inventory
				OpenInven();
				//npcStore.invenCollider = invenDlg.collider;
			}

			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_MoveStopIndication());
		}
	}

	public void OpenQuestMiniView()
	{
		if( !IsOpenQuestMiniView)
		{
			questMiniObj = GameObject.Instantiate( Resources.Load( "UI/AsGUI/GUI_QuestMini")) as GameObject;

			if( questMiniObj != null)
			{
				questMiniView = questMiniObj.GetComponentInChildren<AsQuestMiniViewMgr>();

				if( questMiniView != null)
					questMiniView.UpdateQuetMiniViewMgr();
			}
		}
		else
			questMiniView.UpdateQuetMiniViewMgr();
	}

	public void UpdateQuestMiniView()
	{
		if( IsOpenQuestMiniView)
			questMiniView.UpdateQuetMiniViewMgr();
	}

	public void NotEnoughMiracleProcessInGame()
	{
		CloseQuestBook();
		CloseQuestAccept();
		CloseNpcStore();
		OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);
	}

	public bool CheckMinorForCashStore(bool _normalOpen, eCLASS _class, eCashStoreMenuMode _mode, eCashStoreSubCategory _category, int _idx, bool _toMiralce = false)
	{
		AsMinorCheckInfo checker = new AsMinorCheckInfo();

		bool loadFile = checker.LoadFile();

		bool canOpen = checker.CheckMinorInfo();

		if (loadFile == false || canOpen == false)
		{
			GameObject obj = ResourceLoad.CreateGameObject("UI/AsGUI/GUI_MinorCheck");

			AsMinorCheckerDlg dlg = obj.GetComponent<AsMinorCheckerDlg>();
			dlg.Show(_normalOpen, _class, _mode, _category, _idx, _toMiralce);
			return false;
		}
		else // exist file
		{
			return true;
		}
	}

	public void OpenCashStore(int _npcID, eCLASS _class, eCashStoreMenuMode _mode, eCashStoreSubCategory _category, int _idx, bool _notCheckMinor = false)
	{
		if (true == IsOpenNpcStore || true == AsCommonSender.isSendWarp)
			return;

		if (_notCheckMinor == false && _mode == eCashStoreMenuMode.CHARGE_MIRACLE)
			if (CheckMinorForCashStore(false, _class, _mode, _category, _idx) == false)
				return;

		if (IsOpenCashStore == true)
		{
			cashStore.ActivateItem(_mode, _category, _idx);
			return;
		}

		if (true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if (true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		if (true == AsEntityManager.Instance.UserEntity.GetProperty<bool>(eComponentProperty.SHOP_OPENING))
		{
			SetMsgBox(AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365)));
			cashShopBtn.SetState(0);
			return;
		}

		Init();

		cashStoreObj = GameObject.Instantiate(Resources.Load("UI/MiracleShop/GUI_MiracleShop")) as GameObject;

		if (null != cashStoreObj)
		{
			cashStore = cashStoreObj.GetComponentInChildren<AsCashStore>();
			cashStore.InitilizeStore(_npcID, _class);
			cashStore.SetGotoMenuAfterInit(_mode, _category, _idx);
		}

		AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_MoveStopIndication());
	}

	public void OpenEnchantDlg( RealItem _item)
	{
		//if( true == IsOpenTrade)
		//	return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;

		}

		if( false == AsUserInfo.Instance.IsEquipChaingEnable())
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));

			return;
		}

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( true == IsOpenNpcStore)
			CloseNpcStore();

		if( true == IsOpenPStore)
			ClosePStore();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenTrade)
			CloseTradeDlg( true);

		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();

		if( true == IsOpenStorage)
			CloseStorage();

		if( false == IsOpenInven)
			OpenInven();

		if( null != guildDlg)
			CloseGuildDlg();

		if( null != systemDlg)
			CloseSystemDlg();

		if( null != lobiRecDlg)
			CloseLobiRecDlg();

		if( null != channelSelectDlg)
			CloseChannelSelectDlg();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
		
		AsSocialManager.Instance.SocialUI.CloseSocailDlg();

		if( null == _item)
		{
			Debug.LogError( "OpenEnchantDlg()[null == _item ]");
			return;
		}

		if( null == enchantDlg)
		{
			GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Socket");
			if( null == obj)
			{
				Debug.LogError( "faile resource.load [ UI/AsGUI/GUI_Socket ]");
				return;
			}

			enchantDlg = obj.GetComponent<EnchantDlg>();
			if( null == enchantDlg)
			{
				GameObject.Destroy( obj);
				return;
			}
		}

		enchantDlg.Open( _item);
	}

	public void OpenRandItemUI( int iSlot, RealItem _realItem)
	{
		CloseRandItemUI();

		if( null == _realItem)
		{
			Debug.LogError( "OpenRandItemUI()[ error null == useRandItemIdx ]");
			return;
		}

		if( null == randItemUI)
		{
			GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_RandomBox");
			if( null == obj)
			{
				Debug.LogError( "faile resource.load [ UI/AsGUI/GUI_RandomBox ]");
				return;
			}

			randItemUI = obj.GetComponent<AsUIRandItem>();
			if( null == randItemUI)
			{
				GameObject.Destroy( obj);
				return;
			}
		}

		randItemUI.Open( useRandItemIdx, _realItem.item.ItemID, _realItem, iSlot);
		useRandItemIdx = 0;
	}

	public void OpenStrengthenDlg()
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
		{
			SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		if( false == AsUserInfo.Instance.IsEquipChaingEnable())
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1633),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));

			return;
		}

		if( true == IsOpenTrade)
			return;

		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();

		if( null == strengthenDlg)
		{
			GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Strengthen_new");
			if( null == obj)
			{
				Debug.LogError( "faile resource.load [ UI/AsGUI/GUI_Strengthen_new ]");
				return;
			}
			else
			{
				if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.INTENSIFY) != null)
					AsCommonSender.SendClearOpneUI( OpenUIType.INTENSIFY);
			}

			strengthenDlg = obj.GetComponent<StrengthenDlg>();
			if( null == strengthenDlg)
			{
				GameObject.Destroy( obj);
				return;
			}
		}

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( null != systemDlg)
			CloseSystemDlg();

		if( null != lobiRecDlg)
			CloseLobiRecDlg();

		if( null != channelSelectDlg)
			CloseChannelSelectDlg();

		if( true == IsOpenProductionDlg)
			CloseProductionDlg();

		if( null != guildDlg)
			CloseGuildDlg();

		if( true == IsOpenQuestBook)
			CloseQuestBook();

		if( true == IsOpenStorage)
			CloseStorage();

		if( true == IsOpenPStore)
			ClosePStore();

		if( false == IsOpenInven)
			OpenInven();
		
		if (IsOpenMainMenuDlg == true)
			CloseMainMenuDlg();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde

		AsSocialManager.Instance.SocialUI.CloseSocailDlg();

		strengthenDlg.Open();

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_STRENGTHEN_UI));
	}

	public void OpenPlayerStatusDlg( float fPosZ = -1f)
	{
		// < ilmeda, 20120917
		if( true == IsOpenTrade)
			return;
		// ilmeda, 20120917 >

		if( true == IsOpenCashStore)
			CloseCashStore();

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( null != recommendInfoDlg)
			CloseRecommendInfoDlg();

		if( IsOpenProductionDlg)
			productionDlg.CloseProductonPlanTab();

		if( true == IsOpenInstantDungeonDlg)
			CloseInstantDungeonDlg();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();
		
		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
		BonusManager.Instance.CloseLevelUpWindow(); //$yde

		AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
		AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();//#19720
		if( IsOpenPlayerStatus)
			ClosePlayerStatus();

		playerStatusObject = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Status_1");
		if( null == playerStatusObject)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == UI/AsGUI/GUI_Status_1 ]");
			return;
		}

		playerStatusDlg = playerStatusObject.GetComponentInChildren< PlayerStatusDlg >();
		if( null == playerStatusDlg)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == PlayerStatusDlg ]");
			return;
		}

		playerStatusDlg.Open( fPosZ);
	}
	
	public void OpenPlayerPartsOptionDlg()
	{
		// < ilmeda, 20120917
		if( true == IsOpenTrade)
			return;
		// ilmeda, 20120917 >

		GameObject _obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_PlayerPartsEdit");
		if( null == _obj)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == UI/AsGUI/GUI_PlayerPartsEdit ]");
			return;
		}

		playerPartsOptionDlg = _obj.GetComponentInChildren< PlayerPartsOptionDlg >();
		if( null == playerPartsOptionDlg)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == PlayerStatusDlg ]");
			return;
		}

		playerPartsOptionDlg.Open();
	}
	

	//dopamin
	public void OpenOtherUserStatusDlg( body1_SC_OTHER_INFO _data, float fPosZ = -1f)
	{
		// < ilmeda, 20120917
		if( true == IsOpenTrade)
			return;
		// ilmeda, 20120917 >

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( IsOpenProductionDlg)
			productionDlg.CloseProductonPlanTab();
		
		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();

		CloseRecommendInfoDlg();	// #17479

		AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
		AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();//#19720

		if( IsOpenPlayerStatus)
			ClosePlayerStatus();

		playerStatusObject = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_Status_1");
		if( null == playerStatusObject)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == UI/AsGUI/GUI_Status_1 ]");
			return;
		}

		playerStatusDlg = playerStatusObject.GetComponentInChildren< PlayerStatusDlg >();
		if( null == playerStatusDlg)
		{
			Debug.LogError( "AsHuddlgMgr::OpenPlayerStatusDlg()[ null == PlayerStatusDlg ]");
			return;
		}

		playerStatusDlg.OpenOtherUser( _data, fPosZ);
	}

	/*
	public void OpenOtherUserStatusDlg( body1_SC_GUILD_MEMBER_INFO_DETAIL_RESULT _data)
	{
		// < ilmeda, 20120917
		if( true == IsOpenTrade)
			return;
		// ilmeda, 20120917 >

		if( null == playerStatusDlg)
		{
			Debug.LogError( "null == playerStatusDlg");
			return;
		}

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillSettingDlg)
			CloseSkillSettingDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		playerStatusDlg.gameObject.SetActiveRecursively( true);
		playerStatusDlg.OpenOtherUser( _data);
	}
	 */


	// < ilmeda, 20120914
	public void OpenTradeDlg( string strUserPlayer, string strOtherPlayer)
	{
//		if( null == tradeDlg)
//		{
//			Debug.LogError( "null == tradeDlg");
//			return;
//		}

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( null != channelSelectDlg)
			CloseChannelSelectDlg();

		if( null != systemDlg)
			CloseSystemDlg();

		if( null != lobiRecDlg)
			CloseLobiRecDlg();

		if( null != guildDlg)
			CloseGuildDlg();

		if( null != productionDlg)
			CloseProductionDlg();

		if( true == IsOpenQuestBook)
			CloseQuestBook();

		if( true == IsOpenStorage)
			CloseStorage();

		if( true == IsOpenQuestAccept)
			CloseQuestAccept();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
		
		AsSocialManager.Instance.CloseAllSocailUI();

		if( false == IsOpenTrade)
		{
			//tradeDlg.Open( strUserPlayer, strOtherPlayer);
			StartCoroutine( OpenTradeDlg_Coroutine( strUserPlayer, strOtherPlayer));
		}

		if( false == IsOpenInven)
			OpenInven();
	}
	// ilmeda, 20120914 >

	public IEnumerator OpenTradeDlg_Coroutine( string strUserPlayer, string strOtherPlayer)
	{
		if( false == IsOpenTrade)
		{
			if( null == tradeDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Trade");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				tradeDlg = go.GetComponentInChildren<UITradeDlg>();

				if( null != tradeDlg)
				{
					tradeDlg.Open( go, strUserPlayer, strOtherPlayer);
				}
			}
		}
	}

	public IEnumerator OpenSynthesisDlg()
	{
		if( true == IsOpenPlayerStatus )
			ClosePlayerStatus();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
		
		if( false == IsOpenSynthesisDlg)
		{
			if( null == m_SynthesisDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Synthesis0_Menu");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_SynthesisDlg = go.GetComponentInChildren<AsSynthesisMainDlg>();
				
				//if( false == IsOpenInven)
				//	OpenInven();
//				if( null != m_SynthesisDlg)
//				{
//					m_SynthesisDlg.Open( go);
//
//					if( false == IsOpenInven)
//						OpenInven();
//				}
			}
		}
		else
		{
			CloseSynthesisDlg();
		}
	}
	
	public bool IsOpenSynCosDlg
	{
		get
		{
			if( null == m_SynCosDlg )
				return false;
			
			return m_SynCosDlg.gameObject.activeSelf;
		}
	}
	
	public bool IsOpenSynOptionDlg
	{
		get
		{
			if( null == m_SynOptionDlg )
				return false;
			
			return m_SynOptionDlg.gameObject.activeSelf;
		}
	}
	
	public bool IsOpenSynEnchantDlg
	{
		get
		{
			if( null == m_SynEnchantDlg )
				return false;
			
			return m_SynEnchantDlg.gameObject.activeSelf;
		}
	}
	
	public bool IsOpenSynDisDlg
	{
		get
		{
			if( null == m_SynDisDlg )
				return false;
			
			return m_SynDisDlg.gameObject.activeSelf;
		}
	}

	public void CloseSynCosDlg()
	{
		if( null == m_SynCosDlg )
			return;
		
		m_SynCosDlg.Close();
		GameObject.Destroy(m_SynCosDlg.gameObject);			
	}
	
	public void CloseSynOptionDlg()
	{
		if( null == m_SynOptionDlg )
			return;
		
		m_SynOptionDlg.Close();
		GameObject.Destroy(m_SynOptionDlg.gameObject);	
	}
	
	public void CloseSynEnchantDlg()
	{
		if( null == m_SynEnchantDlg )
			return;
		
		m_SynEnchantDlg.Close();
		GameObject.Destroy(m_SynEnchantDlg.gameObject);	
	}
	
	public void CloseSynDisDlg()
	{
		if( null == m_SynDisDlg )
			return;
		
		m_SynDisDlg.Close();
		GameObject.Destroy(m_SynDisDlg.gameObject);	
	}

	public void OpenSynthisisDlg_Coroutine()
	{
		StartCoroutine( OpenSynthesisDlg());
	}

#if INDUN_NEW
	public IEnumerator OpenInstantDungeonDlg(int nIndunBranchTable_LastEnter)
	{
		if( false == IsOpenInstantDungeonDlg)
		{
			if( null == m_InstantDungeonDlg && false == m_bIndunDlgCoroutine)
			{
				m_bIndunDlgCoroutine = true;

				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_InDeon_New");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_InstantDungeonDlg = go.GetComponentInChildren<AsInstantDungeonDlg_new>();

				if( null != m_InstantDungeonDlg)
				{
					m_InstantDungeonDlg.Open( go, nIndunBranchTable_LastEnter);
				}

				m_bIndunDlgCoroutine = false;
			}
		}
		else
		{
			CloseInstantDungeonDlg();
		}
	}
#else
	public IEnumerator OpenInstantDungeonDlg()
	{
		if( false == IsOpenInstantDungeonDlg)
		{
			if( null == m_InstantDungeonDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_InDeon");
				yield return obj;
				
				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_InstantDungeonDlg = go.GetComponentInChildren<AsInstantDungeonDlg>();
				
				if( null != m_InstantDungeonDlg)
				{
					m_InstantDungeonDlg.Open( go);
				}
			}
		}
		else
		{
			CloseInstantDungeonDlg();
		}
	}
#endif

	public void OpenInstantDungeonDlg_Coroutine(int nIndunBranchTable_LastEnter)
	{
		StartCoroutine( OpenInstantDungeonDlg( nIndunBranchTable_LastEnter));
	}

	#region -Ranking
	public void OpenRankingDlg( body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT data)
	{
		if( null == rankingDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Ranking");
			Debug.Assert( null != obj);
			GameObject go = GameObject.Instantiate( obj) as GameObject;
			Debug.Assert( null != go);
			rankingDlg = go.GetComponentInChildren<AsRankingDlg>();
		}

		if( true == IsOpenInstantDungeonDlg)
			CloseInstantDungeonDlg();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();

		rankingDlg.Init( data);
	}

	public void CloseRankingDlg()
	{
		if( null == rankingDlg)
			return;

		GameObject.Destroy( rankingDlg.gameObject.transform.parent.gameObject);
		rankingDlg = null;
	}

    public void OpenApRankRewardDlg(body_SC_AP_RANK_RESULT data)
    {
        if (null == rankingApRewardDlg)
        {
            GameObject obj = ResourceLoad.LoadGameObject("GUI_RankingAPRewardPopup");
            Debug.Assert(null != obj);
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            Debug.Assert(null != go);
            rankingApRewardDlg = go.GetComponentInChildren<AsRankApRewardDlg>();

            rankingApRewardDlg.Show((uint)data.nRank, data.nRewardGroup);
        }
    }

	public void InsertWorldRankData( body_SC_RANK_TOP_LOAD_RESULT data)
	{
		if( null == rankingDlg)
			return;

		rankingDlg.InsertWorldData( data);
	}

	public void InsertWorldRankData( body_SC_RANK_MYRANK_LOAD_RESULT data)
	{
		if( null == rankingDlg)
			return;

		rankingDlg.InsertWorldData( data);
	}

	public void InsertFriendRankData( body_SC_RANK_MYFRIEND_LOAD_RESULT data)
	{
		if( null == rankingDlg)
			return;

		rankingDlg.InsertFriendData( data);
	}


	bool isRunCoroutine = false;
	GameObject curRankAlarm = null;
	public void PromptRankChangeAlarm( body_SC_RANK_CHANGE_MYRANK data)
	{
		AsRankChangeAlarmManager.Instance.Insert( data);

		if( false == isRunCoroutine)
			StartCoroutine( "PromptAlarm", true);
	}

	public void PromptRankChangeAlarm()
	{
		if( false == isRunCoroutine)
			StartCoroutine( "PromptAlarm", true);
	}

	void RemoveAlarm()
	{
		isRunCoroutine = false;
		StopCoroutine( "PromptAlarm");
		if( null != curRankAlarm)
		{
			GameObject.Destroy( curRankAlarm);
			curRankAlarm = null;
		}
	}

	public void RemoveCashShopModel()
	{
		if (cashShopEntity != null)
		{
			AsEntityManager.Instance.RemoveEntity(cashShopEntity);
			cashShopEntity = null;
		}

		//if (cashShopPetEntity != null)
		//{
		//    AsEntityManager.Instance.RemoveEntity(cashShopPetEntity);
		//    cashShopPetEntity = null;
		//}
	}

	IEnumerator PromptAlarm( bool immediate)
	{
		isRunCoroutine = true;

		if( false == immediate)
			yield return new WaitForSeconds( 7.0f);

		while( true)
		{
			if( 0 == AsRankChangeAlarmManager.Instance.Count && false == TerrainMgr.Instance.isNeedTerrainView)
			{
				isRunCoroutine = false;
				break;
			}

			string strUIPath;

			if( true == TerrainMgr.Instance.isNeedTerrainView)
			{
				strUIPath = "UI/AsGUI/GUI_RegionChangeAlarm";
			}
			else
			{
				strUIPath = "UI/AsGUI/GUI_RankChangeAlarm";
			}

			GameObject go = ResourceLoad.CreateGameObject( strUIPath);
			Debug.Assert( null != go);
			curRankAlarm = go;

			if( true == TerrainMgr.Instance.isNeedTerrainView)
			{
				AsUITerrainViewDlg terrainAlarm = go.GetComponent<AsUITerrainViewDlg>();
				Debug.Assert( null != terrainAlarm);

				terrainAlarm.Open();
				TerrainMgr.Instance.isNeedTerrainView = false;
				yield return new WaitForSeconds( 7.0f);
			}
			else
			{
				AsRankChangeAlarm rankAlarm = go.GetComponent<AsRankChangeAlarm>();
				Debug.Assert( null != rankAlarm);
				body_SC_RANK_CHANGE_MYRANK data = AsRankChangeAlarmManager.Instance.Get(0);
				Debug.Assert( null != data);
				rankAlarm.Init( data);
				yield return new WaitForSeconds( 7.0f);
				AsRankChangeAlarmManager.Instance.Remove(0);
			}

			GameObject.Destroy( go);
			curRankAlarm = null;
		}
	}
	#endregion

	//	Skill
	public void OpenSkillshop( int npcID)
	{
		if( true == IsOpenTrade)
			return;

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
		
		CloseRecommendInfoDlg();	// 17479

		skillShopObj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_SkillShop");
		Debug.Assert( null != skillShopObj);
		AsSkillshopDlg dlg = skillShopObj.GetComponentInChildren<AsSkillshopDlg>();
		Debug.Assert( null != dlg);
		dlg.closeDel = CloseSkillshop;
		dlg.Open( npcID);

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SKILL_SHOP_BTN, 0));
	}

	public void OpenSkillBook()
	{
		if( true == IsOpenOptionDlg)
			CloseOptionDlg();

		if( true == IsOpenedSkillBook)
			CloseSkillBook();

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();

		if( false == IsOpenedSkillBook)
		{
			skillBookObj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_SkillBook");
			Debug.Assert( null != skillBookObj);
			skillBookDlg = skillBookObj.GetComponentInChildren<AsSkillbookDlg>();
			Debug.Assert( null != skillBookDlg);
			skillBookDlg.closeDel = CloseSkillBook;
			skillBookDlg.Open();
		}
		else
		{
			skillBookDlg = skillBookObj.GetComponentInChildren<AsSkillbookDlg>();
			Debug.Assert( null != skillBookDlg);
			skillBookDlg.closeDel = CloseSkillBook;
			skillBookDlg.Close();
		}
	}

	#region -PostBox
	public void OpenPostBoxDlg()
	{
		if( true == IsOpenTrade)
			return;

		if( true == AsCommonSender.isSendWarp)
			return;

		if( /*true == AsPvpManager.Instance.CheckMatching() ||*/ true == AsPvpManager.Instance.CheckInArena())
			return;

		if( /*true == AsInstanceDungeonManager.Instance.CheckMatching() ||*/ true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( true == IsOpenInstantDungeonDlg)
			CloseInstantDungeonDlg();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( false == IsOpenedPostBox)
		{
			postBoxDlgObj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_PostBox");
			Debug.Assert( null != postBoxDlgObj);
			postBoxDlg = postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
			Debug.Assert( null != postBoxDlg);
			postBoxDlg.closeDel = ClosePostBoxDlg;
			postBoxDlg.Open();
		}
		else
		{
			postBoxDlg = postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
			Debug.Assert( null != postBoxDlg);
			postBoxDlg.closeDel = ClosePostBoxDlg;
			postBoxDlg.Close();
		}
//		if( false == IsOpenedPostBox)
//			postBoxDlg.Open();
//		else
//			postBoxDlg.Close();
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
	}

	public void OpenWritePostBoxDlg( string receiver)
	{
		if( true == IsOpenTrade)
			return;

		if( true == AsCommonSender.isSendWarp)
			return;

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( true == IsOpenInstantDungeonDlg)
			CloseInstantDungeonDlg();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();

		if( false == IsOpenedPostBox)
		{
			postBoxDlgObj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_PostBox");
			Debug.Assert( null != postBoxDlgObj);
		}

		postBoxDlg = postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
		Debug.Assert( null != postBoxDlg);
		postBoxDlg.closeDel = ClosePostBoxDlg;
		postBoxDlg.Open( ePostBoxDlgState.WriteTab);
//		postBoxDlg.SetMailSender( receiver);
		StartCoroutine( "SetMailSender", receiver);
		
		AsPetManager.Instance.ClosePetDlgByOtherWindow(); //$yde
	}

	private IEnumerator SetMailSender( string receiver)
	{
		yield return null;

		postBoxDlg.SetMailSender( receiver);
	}
	
	public void PetDlgOpened()
	{
		// < ilmeda, 20120917
		if( true == IsOpenTrade)
			return;
		// ilmeda, 20120917 >

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenCashStore)
			CloseCashStore();

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		if( true == IsOpenedSkillshop)
			CloseSkillshop();

		if( true == IsOpenEnchantDlg)
			CloseEnchantDlg();

		if( true == IsOpenStrengthenDlg)
			CloseStrengthenDlg();

		if( null != recommendInfoDlg)
			CloseRecommendInfoDlg();

		if( IsOpenProductionDlg)
			productionDlg.CloseProductonPlanTab();

		if( true == IsOpenInstantDungeonDlg)
			CloseInstantDungeonDlg();

		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();
		
		if( true == IsOpenSynthesisDlg)
			CloseSynthesisDlg();
		if( true == IsOpenSynCosDlg )
			CloseSynCosDlg();
		if( true == IsOpenSynDisDlg )
			CloseSynDisDlg();
		if( true == IsOpenSynEnchantDlg )
			CloseSynEnchantDlg();
		if( true == IsOpenSynOptionDlg )
			CloseSynOptionDlg();
		

		AsSocialManager.Instance.SocialUI.CloseFindFriendDlg();
		AsSocialManager.Instance.SocialUI.CloseSocialStoreDlg();
		AsSocialManager.Instance.SocialUI.CloseSocailCloneDlg();//#19720
	}
	#endregion

	public IEnumerator OpenOptionDlg()
	{
		if( true == IsOpenedSkillBook)
			CloseSkillBook();

		if( true == IsOpenInven)
			CloseInven();

		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();

		if( false == IsOpenOptionDlg)
		{
			if( null == m_OptionDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Option");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_OptionDlg = go.GetComponentInChildren<AsOptionDlg>();

				if( null != m_OptionDlg)
					m_OptionDlg.Open( go);
			}
		}
		else
		{
			CloseOptionDlg();
		}
	}
	
	public IEnumerator OpenChatServerDlg(int nSlotIndex)
	{
		if( false == IsOpenChatServerDlg)
		{
			if( null == m_ChatServerDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_ChatServer");
				yield return obj;
				
				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_ChatServerDlg = go.GetComponentInChildren<AsChatServerDlg>();
				
				if( null != m_ChatServerDlg)
					m_ChatServerDlg.Open( go, nSlotIndex);
			}
		}
		else
			CloseChatServerDlg();
	}
	
	public IEnumerator	OpenReNameDlg(int nSlotIndex , AsReNameDlg.eReNameType changeType )
	{
		if( false == IsOpenReNameDlg )
		{
			if( null == m_ReNameDlg )
			{
				GameObject	obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_ReName" );
				yield return obj;
				
				GameObject go = GameObject.Instantiate( obj ) as GameObject;
				m_ReNameDlg = go.GetComponentInChildren<AsReNameDlg>();
				
				if( null != m_ReNameDlg )
					m_ReNameDlg.Open( go , nSlotIndex , changeType );
			}
		}
		else
		{
			CloseReNameDlg();
		}
	}

	public IEnumerator	OpenMainMenuDlg()
	{
		if( false == IsOpenMainMenuDlg )
		{
			if( null == m_MainMenuDlg )
			{
				GameObject	obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_SystemMenu" );
				yield return obj;
				
				GameObject go = GameObject.Instantiate( obj ) as GameObject;
				m_MainMenuDlg = go.GetComponentInChildren<AsMainMenuDlg>();
				
				if( null != m_MainMenuDlg )
					m_MainMenuDlg.Open(go);
			}
		}
		else
		{
			CloseMainMenuDlg();
		}
	}
	
	public void OpenInGameEventListDlg(int _npcID = -1)
	{
		if (IsOpenInGameEventListDlg == true)
			return;
		
		if (true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		int activeEventCount = 0;
		GameObject objEventListDlg = ResourceLoad.CreateGameObject("UI/AsGUI/GUI_InGameEventInfoDlg");
		if (objEventListDlg != null)
		{
			m_InGameEventListDlg = objEventListDlg.GetComponentInChildren<AsInGameEventListDlg>();
			
			if (m_InGameEventListDlg != null)
				activeEventCount = m_InGameEventListDlg.Show(_npcID);

			if (activeEventCount == 0)
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2156), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, false);
				CloseInGameEventListDlg();
			}

			_CloseNpcMenu();
		}
	}

	public void OpenInGameEventAchievementDlg(int _npcID , int _eventID)
	{
		if (IsOpenInGameEventAchievementDlg == true)
			return;
		
		if (true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		GameObject objEventAchieveDlg = ResourceLoad.CreateGameObject("UI/AsGUI/GUI_InGameEventAchievementDlg");
		if (objEventAchieveDlg != null)
		{
			m_InGameEventAchievementDlg = objEventAchieveDlg.GetComponentInChildren<AsInGameEventAchievementDlg>();
			m_InGameEventAchievementDlg.Show(_npcID, _eventID);
		}
	}

	public void UpdateInGameEventAchievementDlg()
	{
		if (IsOpenInGameEventAchievementDlg == true)
			m_InGameEventAchievementDlg.UpdateRewardBtn();
	}

	public void CloseInGameEventListDlg(bool _useSound = true)
	{
		if (null == m_InGameEventListDlg)
			return;

		m_InGameEventListDlg.Close(_useSound);
		m_InGameEventListDlg = null;
		
		
	}

	public void CloseInGameEventAchieveDlg()
	{
		if (null == m_InGameEventAchievementDlg)
			return;
		
		m_InGameEventAchievementDlg.Close();
		m_InGameEventAchievementDlg = null;
		
		
	}

	public void ClosePlayerStatus()
	{
		if( null == playerStatusObject)
			return;

		playerStatusDlg.Close();
		GameObject.DestroyObject( playerStatusObject);
		playerStatusDlg = null;
		playerStatusObject = null;
	}
	
	public void ClosePlayerPartsOptionDlg()
	{
		if( null == playerPartsOptionDlg)
			return;

		playerPartsOptionDlg.Close();
		GameObject.DestroyObject( playerPartsOptionDlg.gameObject );
		playerPartsOptionDlg = null;		
	}	
		
		
	private void DeleteInven()
	{
		if( null == invenDlg)
			return;

		invenDlg.Close();
		GameObject.DestroyObject( invenDlg.gameObject);
		invenDlg = null;
	}

	public void CloseInven()
	{
		if( null == invenDlg)
		{
			return;
		}

		ClosePlayerStatus();

		if( true == IsOpenTrade)
		{
			tradeDlg.OpenMsgBox_TradeCancel();
		}
		else
		{
			//SetCloseBtnActive( false);
			DeleteInven();
			if( true == IsOpenNpcStore)
				CloseNpcStore();

			if( true == IsOpenCashStore)
				CloseCashStore();

			if( true == IsOpenSynthesisDlg)
				CloseSynthesisDlg();
			if( true == IsOpenSynCosDlg )
				CloseSynCosDlg();
			if( true == IsOpenSynDisDlg )
				CloseSynDisDlg();
			if( true == IsOpenSynEnchantDlg )
				CloseSynEnchantDlg();
			if( true == IsOpenSynOptionDlg )
				CloseSynOptionDlg();

			if( true == IsOpenStrengthenDlg)
				CloseStrengthenDlg();

			if( true == IsOpenEnchantDlg)
				CloseEnchantDlg();

			if( true == IsOpenPStore)
				ClosePStore();

			if( true == IsOpenStorage)
				CloseStorage();
		}
	}

	public void CloseStorage() // $yde - 20130115
	{
		if( null == storageDlg)
		{
//			Debug.LogWarning( "null == storageDlg");
			return;
		}

		if( null != storageDlg)
			storageDlg.Close();

		Destroy( storageObj);
		storageDlg = null;
	}

	public void ClosePStore() // $yde - 20130115
	{
		if( null == pstoreDlg)
		{
//			Debug.LogWarning( "null == pstoreDlg");
			return;
		}

		pstoreDlg.Close();

		Destroy( pstoreObj);
	}

	public void CloseNpcStore()
	{
		if( storeObj != null)
		{
			Destroy( storeObj);
			storeObj = null;

			if( IsOpenInven == true)
				DeleteInven();
		}
	}

	public void CloseQuestAccept()
	{
		if( questAcceptObject != null)
		{
			questAccept.Close();
			Destroy( questAcceptObject);
			questAcceptObject = null;
			questAccept = null;
		}
	}

	public void CloseCashStore()
	{
		if( cashStoreObj != null)
		{
            if (null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.cashShopEntity)
			{
				AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
				AsHudDlgMgr.Instance.cashShopEntity = null;
			}

			//if (null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.cashShopPetEntity)
			//{
			//    AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopPetEntity);
			//    AsHudDlgMgr.Instance.cashShopPetEntity = null;
			//}
            
			Destroy( cashStoreObj);
			cashStoreObj = null;          

			if( IsOpenInven == true)
				DeleteInven();
		}
	}

	public void CloseEnchantDlg()
	{
		if( null == enchantDlg)
			return;

		enchantDlg.Close();
		GameObject.Destroy( enchantDlg.gameObject);
	}

	public void CloseStrengthenDlg()
	{
		if( false == IsOpenStrengthenDlg)
			return;

		strengthenDlg.Close();
		GameObject.DestroyObject( strengthenDlg.gameObject);

		if( true == IsOpenInven)
			CloseInven();
	}

	public void CloseRecommendInfoDlg()
	{
		if( null == recommendInfoDlg)
			return;

		recommendInfoDlg.Close();
	}

	public void CloseRandItemUI()
	{
		if( false == IsOpenRandItemUI)
			return;

		randItemUI.Close();
		GameObject.DestroyObject( randItemUI.gameObject);
	}

	// < ilmeda, 20120914
	public void CloseTradeDlg( bool bSendTradeCancel)
	{
		if( null == tradeDlg)
		{
			//Debug.LogError( "null == tradeDlg");
			return;
		}

		if( true == IsOpenTrade)
			tradeDlg.Close( bSendTradeCancel);
		if( false == IsOpenTrade && true == bSendTradeCancel)
			AsCommonSender.SendTradeCancel();
		if( true == IsOpenInven)
			CloseInven();
		if( true == IsOpenTradePopup_Gold)
			CloseTradeGoldDlg();
		if( true == IsOpenTradePopup_Quantity)
			CloseTradeQuantityDlg();
	}
	// ilmeda, 20120914 >

	public void CloseSynthesisDlg()
	{
		if( null == m_SynthesisDlg)
			return;

		m_SynthesisDlg.Close();
		GameObject.Destroy(m_SynthesisDlg.gameObject);

		if( true == IsOpenInven)
			CloseInven();
	}

	public void CloseInstantDungeonDlg()
	{
		if( null == m_InstantDungeonDlg)
			return;

		m_InstantDungeonDlg.Close();
	}

	//	Skill
	public void CloseSkillshop()
	{
		if( null == skillShopObj)
			return;

		GameObject.Destroy( skillShopObj);
		skillShopObj = null;
	}

	public void CloseSkillBook()
	{
		if( null == skillBookObj)
			return;

		GameObject.Destroy( skillBookObj);
		skillBookObj = null;
		skillBookDlg = null;
	}

	#region -Postbox
	public void ClosePostBoxDlg()
	{
		if( null == postBoxDlgObj)
			return;

		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( true == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();

		AsCommonSender.isSendPostList = false;

		postBoxDlg = null;
		GameObject.DestroyImmediate( postBoxDlgObj);
		postBoxDlgObj = null;
	}
	#endregion

	public void CloseOptionDlg()
	{
		if( null == m_OptionDlg)
			return;

		m_OptionDlg.Close();
	}
	
	public void CloseChatServerDlg()
	{
		if( null == m_ChatServerDlg)
			return;
		
		m_ChatServerDlg.Close();
	}
	
	public void CloseReNameDlg()
	{
		if( null == m_ReNameDlg )
			return;
		
		m_ReNameDlg.Close();
	}
	
	public void CloseMainMenuDlg()
	{
		if( null == m_MainMenuDlg )
			return;
		
		m_MainMenuDlg.Close();
	}
	
	public void CloseQuestBook()
	{
		if( questBookObject != null)
		{
			questBook.Close();
			Destroy( questBookObject);
			questBookObject = null;
			questBook = null;
		}
	}

	public void CloseQuestMiniView()
	{
		if( questMiniObj != null)
		{
			Destroy( questMiniObj);
			questMiniObj = null;
			questMiniView = null;
		}
	}

	public void UpdateQuestBookBtn()
	{
		QuestBookBtnControll questBookBtn = questBtn.gameObject.GetComponent<QuestBookBtnControll>();

		if( questBookBtn != null)
			questBookBtn.UpdateQuestBookBtn();
	}

	// < ilmeda, 20120914
	public bool IsOpenTradePopup_Gold
	{
		get
		{
			if( null == tradePopup_Gold)
				return false;

			return tradePopup_Gold.gameObject.active;
		}
	}

	public bool IsOpenTradePopup_Quantity
	{
		get
		{
			if( null == tradePopup_Quantity)
				return false;

			return tradePopup_Quantity.gameObject.active;
		}
	}

	public bool IsOpenTradePopup_ItemRegistrationCancel
	{
		get
		{
			if( null == tradeDlg)
				return false;

			return tradeDlg.isOpenItemRegistrationCancel();
		}
	}

	public bool IsOpenTradePopup_TradeCancel
	{
		get
		{
			if( null == tradeDlg)
				return false;

			return tradeDlg.isOpenTradeCancel();
		}
	}

	public bool IsOpenTradePopup_TradeError
	{
		get
		{
			if( null == tradeDlg)
				return false;

			return tradeDlg.isOpenTradeError();
		}
	}

	public void OpenTradeErrorMsgBox( string strTitle, string strErrMsg, bool bCloseTradeDlg = false)
	{
		if( null == tradeDlg)
			return;

		tradeDlg.OpenMsgBox_TradeError( strTitle, strErrMsg, bCloseTradeDlg);
	}

	public void OpenTradeGoldDlg()
	{
//		if( null == tradePopup_Gold)
//		{
//			Debug.LogError( "null == tradePopup_Gold");
//			return;
//		}
//
//		tradePopup_Gold.Open();

		StartCoroutine( OpenTradeGoldDlg_Coroutine());
	}

	public IEnumerator OpenTradeGoldDlg_Coroutine()
	{
		if( false == IsOpenTradePopup_Gold)
		{
			if( null == tradePopup_Gold)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Trade_Popup_Gold");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				tradePopup_Gold = go.GetComponentInChildren<UITradePopup_Gold>();

				if( null != tradePopup_Gold)
				{
					tradePopup_Gold.Open( go);
				}
			}
		}
	}

	public void CloseTradeGoldDlg()
	{
		if( null == tradePopup_Gold)
		{
			//Debug.LogError( "null == tradePopup_Gold");
			return;
		}

		tradePopup_Gold.Close();
	}

	public void OpenTradeQuantityDlg( RealItem realItem)
	{
//		if( null == tradePopup_Quantity)
//		{
//			Debug.LogError( "null == tradePopup_Quantity");
//			return;
//		}
//
//		tradePopup_Quantity.Open( realItem);

		StartCoroutine( OpenTradeQuantityDlg_Coroutine( realItem));
	}

	public IEnumerator OpenTradeQuantityDlg_Coroutine( RealItem realItem)
	{
		if( false == IsOpenTradePopup_Quantity)
		{
			if( null == tradePopup_Quantity)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Trade_Popup_Quantity");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				tradePopup_Quantity = go.GetComponentInChildren<UITradePopup_Quantity>();

				if( null != tradePopup_Quantity)
				{
					tradePopup_Quantity.Open( go, realItem);
				}
			}
		}
	}

	public void CloseTradeQuantityDlg()
	{
		if( null == tradePopup_Quantity)
		{
			//Debug.LogError( "null == tradePopup_Quantity");
			return;
		}

		tradePopup_Quantity.Close();
	}

	public void SendMoveItem_InvenToTrade( RealItem realItem)
	{
		if( realItem.sItem.nOverlapped > 1)
		{
			if( false == tradeDlg.GetLock( false))
				OpenTradeQuantityDlg( realItem);
		}
		else
			tradeDlg.SendMoveItem_InvenToTrade( realItem, 1);
	}

	public void SendMoveItem_InvenToTrade( RealItem realItem, int nItemCount)
	{
		tradeDlg.SendMoveItem_InvenToTrade( realItem, nItemCount);
	}

	public void SailItem( RealItem reamItem, int nItemCount)
	{
	}

	public void SetTradeItemSlot( bool bOwnerIsMe, bool bAddOrDel, int nInvenSlot, int nTradeSlot, sITEM sTradeItem)
	{
		if( true == bAddOrDel)
		{
			if( true == bOwnerIsMe)
			{
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( nInvenSlot, true);
				if( true == AsHudDlgMgr.Instance.IsOpenInven)
					AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
			}

			RealItem realItem = new RealItem( sTradeItem, nInvenSlot);
			tradeDlg.SetSlotItem( bOwnerIsMe, nTradeSlot, realItem);
		}
		else
		{
			if( true == bOwnerIsMe)
			{
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( nInvenSlot, false);
				if( true == AsHudDlgMgr.Instance.IsOpenInven)
					AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
			}

			tradeDlg.SetSlotItem( bOwnerIsMe, nTradeSlot, null);
		}
	}

	public void SetTradeLock( bool isLeft, bool bLock)
	{
		tradeDlg.SetLock( isLeft, bLock);
	}

	public void SetTradeGold( bool isLeft, long nGold)
	{
		tradeDlg.SetGold( isLeft, nGold);
	}
	// ilmeda, 20120914 >

	public void SetCashStoreObj( GameObject obj)
	{
		cashStoreObj = obj;
	}

	// storage
	public bool IsOpenStorageAdditionDlg
	{
		get
		{
			if( null == storageAdditionDlg)
				return false;

			return storageAdditionDlg.gameObject.active;
		}
	}

	public bool IsOpenStorageQuantityDlg
	{
		get
		{
			if( null == storageQuantityDlg)
				return false;

			return storageQuantityDlg.gameObject.active;
		}
	}

	public void OpenStorageAdditionDlg()
	{
		Debug.Log( "Invalid calling");
	}

	public void CloseStorageAdditionDlg()
	{
		if( null == storageAdditionDlg)
		{
			Debug.LogError( "null == storageAdditionDlg");
			return;
		}

		storageAdditionDlg.Close();
	}

	public void OpenStorageQuantityDlg( eSTORAGE_MOVE_TYPE _moveType, int _rootSlotIdx, int _targetSlotIdx, RealItem _slot)
	{
		if( null == storageQuantityDlg)
		{
			Debug.LogError( "null == storageQuantityDlg");
			return;
		}

		storageQuantityDlg.Open( _moveType, _rootSlotIdx, _targetSlotIdx, _slot);
	}

	public void CloseStorageQuantityDlg()
	{
		if( null == storageQuantityDlg)
		{
			Debug.LogError( "null == storageQuantityDlg");
			return;
		}

		storageQuantityDlg.Close();
	}

	public bool SendMoveItem_InvenToStorage( Ray inputRay, UIInvenSlot invenSlot)
	{
		if( invenSlot.slotItem.realItem.item.ItemData.m_bItem_Storage_Limit == true)
		{
			SetMsgBox( 
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(195),
				AsNotify.MSG_BOX_TYPE.MBT_OK));//,AsNotify.MSG_BOX_ICON.MBI_ERROR));
			return false;
		}

		int slotIndex = storageDlg.GetSlotIndex( inputRay);
		if( slotIndex == -1)
			return false;

		if( invenSlot.slotItem.realItem.sItem.nOverlapped > 1)
		{
			OpenStorageQuantityDlg( eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_DRAG_INPUT, invenSlot.slotIndex, slotIndex, invenSlot.slotItem.realItem);
			return true;
		}
		else
		{
			if( slotIndex != -1)
			{
				body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE( eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_DRAG_INPUT,
					( short)invenSlot.slotIndex, 1, ( short)slotIndex);
				
				AsSoundManager.Instance.PlaySound( invenSlot.slotItem.realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
				AsCommonSender.Send( move.ClassToPacketBytes());
				invenSlot.DeleteSlotItem();				
				//Debug.Log( "SendMoveItem_InvenToStorage: packet send. start slot index = " + move.nStartSlot + ", count = " + move.nStartCount + ", dest slot index = " + move.nDestSlot);

				return true;
			}
			else
				return false;
		}
	}

	public bool SendMoveItem_StorageToInven( Ray inputRay, UIStorageSlot storageSlot)
	{
		if( false == IsOpenInven)
			return false;

		int slotIndex = invenDlg.GetSlotIndex( inputRay);
		if( slotIndex == -1)
			return false;

		if( storageSlot.slotItem.realItem.sItem.nOverlapped > 1)
		{
			OpenStorageQuantityDlg( eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_DRAG_OUTPUT, storageSlot.slotIndex, slotIndex, storageSlot.slotItem.realItem);
			return true;
		}
		else
		{
			if( slotIndex != -1)
			{
				body_CS_STORAGE_MOVE move = new body_CS_STORAGE_MOVE( eSTORAGE_MOVE_TYPE.eSTORAGE_MOVE_TYPE_DRAG_OUTPUT,
					( short)storageSlot.slotIndex, 1, ( short)slotIndex);
				AsCommonSender.Send( move.ClassToPacketBytes());
				storageSlot.DeleteSlotItem();

				Debug.Log( "SendMoveItem_StorageToInven: packet send. start slot index = " + move.nStartSlot + ", count = " + move.nStartCount + ", dest slot index = " + move.nDestSlot);

				return true;
			}
			else
				return false;
		}

		return true;
	}

	// control function
	public void CollapseMenuBtn()
	{
		UIButton btn = GetTabBtn( ( int)eBTN_TYPE.ACTIVE_BTN);
		if( null == btn)
			return;

		AsMenuBtnAction action = btn.GetComponent<AsMenuBtnAction>();
		action.ForcedCollapse();
	}

	private void ActiveBtnAction()
	{
		UIButton btn = GetTabBtn( ( int)eBTN_TYPE.ACTIVE_BTN);
		if( null == btn)
			return;

		AsMenuBtnAction action = btn.GetComponent<AsMenuBtnAction>();
		m_MenuBtnAction = action;
		action.Expand();
		if( true == action.getExpanded)
			m_fCurTime = Time.time;
	}

	private void ActiveBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsChatFullPanel.Instance.Close();

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == _isOpenNpcMenu())
				return;

			SetNewPostImg( AsUserInfo.Instance.NewMail);
			SetNewProductImg( AsUserInfo.Instance.IsHaveProductionInfoComplete());
			SetNewSkillImg( SkillBook.Instance.isNewSkillAdd);

//			ActiveBtnAction();
			OpenMainMenu();
		}
	}

	public void SetNewInvenImg( bool _isActive)
	{
		if( null == newInvenMail)
			return;

		if( _isActive == newInvenMail.gameObject.active)
			return;

		newInvenMail.gameObject.active = _isActive;
	}
	
	public void SetNewPostImg( bool _isActive)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetNewImg(eMainBtnType.POST , _isActive);
		}
		
		CheckNewMenuImg();
	}

	public void SetNewProductImg( bool _isActive)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetNewImg(eMainBtnType.PRODUCT , _isActive);
		}
		
		CheckNewMenuImg();
	}

	public void SetNewSkillImg( bool _isActive)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetNewImg(eMainBtnType.SKILL , _isActive);
		}
		
		CheckNewMenuImg();
	}

	public void SetNewSocialImg( bool _isActive)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetNewImg(eMainBtnType.SOCIAL , _isActive);
		}
		
		CheckNewMenuImg();
	}

	public bool isnewSocialMail
	{
		get
		{

			if( null == newSocialMail)
				return false;

			return newSocialMail.gameObject.active;
		}
	}

	public void SetNewGuildImg( bool _isActive)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetNewImg(eMainBtnType.GUILD , _isActive);
		}
		
		CheckNewMenuImg();
	}

	public bool isnewGuildMail
	{
		get
		{
			if( null == newGuildMail)
				return false;

			return newGuildMail.gameObject.active;
		}
	}

	public void SetNewMenuImg( bool _isActive)
	{
		if( null == newMenuMail)
			return;

		newMenuMail.gameObject.active = _isActive;
	}

	public void CheckNewMenuImg()
	{
		if( null == newMenuMail)
			return;

		if( AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
		{
			SetNewMenuImg( false);
			return;
		}

		if( true == AsUserInfo.Instance.NewMail)
		{
			SetNewMenuImg( true);
			return;
		}

		if( true == isnewGuildMail)
		{
			SetNewMenuImg( true);
			return;
		}

		if( true == isnewSocialMail)
		{
			SetNewMenuImg( true);
			return;
		}

		if( null != SkillBook.Instance && true == SkillBook.Instance.isNewSkillAdd)
		{
			SetNewMenuImg( true);
			return;
		}

		if( true == AsUserInfo.Instance.IsHaveProductionInfoComplete())
		{
			SetNewMenuImg( true);
			return;
		}

		SetNewMenuImg( false);
	}

	private void InvenBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == _isOpenNpcMenu())
				return;

			if( isOpenMsgBox == true)
				return;

			if( false == IsOpenInven)
			{
				if( false == IsOpenPlayerStatus)
				{
					if( false == IsOpenedPostBox)
						OpenPlayerStatusDlg();
				}

				OpenInven();
			}
			else
			{
				CloseInven();
			}

			m_fCurTime = Time.time;
		}
	}

	private void StorageBtnDelegate( ref POINTER_INFO ptr) //$ yde - 20130115
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( false == IsOpenStorage)
				OpenStorage();
			else
				CloseStorage();

			m_fCurTime = Time.time;
		}
	}


	private void QuestBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == _isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)//$yde
			{
				PopUpDisableByPStore();
				return;
			}

			OpenQuestBook();

			m_fCurTime = Time.time;
		}
	}

	#region -ChannelSelectDlg_Impl
	private void ChannelSelectBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == _isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)//$yde
			{
				PopUpDisableByPStore();
				return;
			}

			Debug.Log( "ChannelSelectBtnDelegate");

			if( null == channelSelectDlg)
				OpenChannelSelectDlg();
			else
				CloseChannelSelectDlg();
		}
	}

	public void OpenChannelSelectDlg()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		AsChatFullPanel.Instance.Close();

		body_CG_CHANNEL_LIST channelList = new body_CG_CHANNEL_LIST( true);
		byte[] data = channelList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	public void CloseChannelSelectDlg()
	{
		GameObject.DestroyImmediate( channelSelectDlg);
		channelSelectDlg = null;
	}
	#endregion
	
	#region -SystemDlg_Impl
	public void OpenSystemDlg()
	{
		systemDlg = Instantiate( Resources.Load( "UI/AsGUI/GUI_System")) as GameObject;

		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.MainMenuDlg.Close();
	}

	public void CloseSystemDlg()
	{
		GameObject.DestroyImmediate( systemDlg);
		systemDlg = null;
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_SYSTEM));
	}
	#endregion

	#region -LobiRecDlg_Impl
	public void OpenLobiRecDlg()
	{
		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.MainMenuDlg.Close();

		lobiRecDlg = Instantiate( Resources.Load( "UI/AsGUI/GUI_LobbyMenu")) as GameObject;

		AsLobiRecDlg lobiDlg = lobiRecDlg.GetComponentInChildren<AsLobiRecDlg>();
		lobiDlg.Init();


	}
	
	public void CloseLobiRecDlg()
	{
		GameObject.Destroy( lobiRecDlg);
		lobiRecDlg = null;
	}

	private void lobiRecBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsLobiSDKManager.Instance.StopCapturing();
			lobiRecBtn.gameObject.SetActiveRecursively(false);
		}
	}
	
	public void ShowLobiRecBtn()
	{

		m_bFadeOut =true;
		m_fFadeOutTime = Time.time;
		lobiRecBtn.gameObject.SetActiveRecursively(true);
		#if UNITY_IPHONE
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPointByUICamera( lobiIncameraPos.transform.position);
		Debug.Log("ShowLobiRecBtn :"+screenPos.ToString() + "lobiIncameraPos.width :"+lobiIncameraPos.PixelSize.x.ToString() );
		AsLobiSDKManager.Instance.SetWipePosition(screenPos.x -  (int)(lobiIncameraPos.PixelSize.x * 0.5f) , Screen.height - (screenPos.y +  (int)(lobiIncameraPos.PixelSize.x * 0.5f) ) ,  lobiIncameraPos.PixelSize.x );
		#endif
	}
	#endregion


	#region -GuildDlg_Impl
	public void OpenGuildDlg(bool bRequestGuildList)
	{
#if USE_GUILD_NEW
		if( null == AsHudDlgMgr.Instance.GuildDlg )
		{
			GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild_new/GUI_Guild")) as GameObject;
			AsHudDlgMgr.Instance.GuildDlg = go;
			AsGuildNewDlg guildDlg = go.GetComponentInChildren<AsGuildNewDlg>();
			guildDlg.Init();
		}
#else
		if( null == AsHudDlgMgr.Instance.GuildDlg )
		{
			GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_Guild")) as GameObject;
			AsHudDlgMgr.Instance.GuildDlg = go;
			
			AsGuildDlg guildDlg = go.GetComponentInChildren<AsGuildDlg>();
			guildDlg.Init();
		}
		
		body_CS_GUILD_INFO guildInfo = new body_CS_GUILD_INFO(bRequestGuildList);
		byte[] data = guildInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
#endif
	}

	public void CloseGuildDlg()
	{
		AsCommonSender.isSendGuild = false;
		GameObject.DestroyImmediate( guildDlg);
		guildDlg = null;
	}
	#endregion

	#region -Designation
	public void SetDesignationText()
	{
		if( null == playerStatusDlg)
			return;

		playerStatusDlg.SetDesignationText();
	}
	#endregion

	private void OptionBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			OpenOption();
		}
	}
	
	public void OpenOption()
	{
		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( true == _isOpenNpcMenu())
			return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)//$yde
		{
			PopUpDisableByPStore();
			return;
		}

		if( false == IsOpenOptionDlg)
			StartCoroutine( OpenOptionDlg());
		else
			CloseOptionDlg();
	}
	
	public void OpenChatServer(int nSlotIndex)
	{
		if( true == AsUserInfo.Instance.IsDied())
			return;
		
		if( false == IsOpenChatServerDlg)
			StartCoroutine( OpenChatServerDlg( nSlotIndex));
		else
			CloseChatServerDlg();
	}
	
	public void OpenReName(int nSlotIndex , AsReNameDlg.eReNameType changeType )
	{
		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( false == IsOpenReNameDlg )
			StartCoroutine( OpenReNameDlg( nSlotIndex , changeType ) );
		else
			CloseReNameDlg();
	}
	
	public void OpenMainMenu()
	{
		if( false == IsOpenMainMenuDlg )
			StartCoroutine( OpenMainMenuDlg() );
		else
			CloseMainMenuDlg();
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AllDlgClose();
		}
	}

	public void PopUpDisableByPStore()
	{
		if( isOpenMsgBox == false)
			m_msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365), AsNotify.MSG_BOX_TYPE.MBT_OK,AsNotify.MSG_BOX_ICON.MBI_ERROR);
	}

	// Awake
	void Awake()
	{
		ms_kIstance = this;
		
		if( null != synDlgBtn )
			synDlgBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(1223);
	}

	// Use this for initialization
	void Start()
	{
		if( null == tabBtn)
		{
			Debug.LogError( "null == tabBtn");
			return;
		}

		mainBtn.SetInputDelegate( ActiveBtnDelegate);
		mainBtn.Text = AsTableManager.Instance.GetTbl_String(821);
		
		invenBtn.SetInputDelegate( InvenBtnDelegate);
		invenBtn.Text = AsTableManager.Instance.GetTbl_String(820);

		questBtn.SetInputDelegate( QuestBtnDelegate);
		questBtn.Text = AsTableManager.Instance.GetTbl_String(819);

		lobiRecBtn.SetInputDelegate( lobiRecBtnDelegate);
		lobiRecBtn.Text = AsTableManager.Instance.GetTbl_String(19018);

		if( null != textWaypointBtn)
			textWaypointBtn.Text = AsTableManager.Instance.GetTbl_String(866);

		txtChannelBtn.Text = AsTableManager.Instance.GetTbl_String(4012);

		if( null != cashShopBtn)
			cashShopBtn.Text = AsTableManager.Instance.GetTbl_String(818);
		
		lvUpBonusBtn.SetInputDelegate( OnLevelUpBonusBtn);
		lvUpBonusBtn.Text = AsTableManager.Instance.GetTbl_String(4105);

		AllDlgClose();
		ClosePlayerStatus();

		CheckNewMenuImg();

#if INDUN_NEW
		goIndunLimitCountBg.SetActive( false);
#endif
	}

	// Update is called once per frame
	void Update()
	{
		myGold.Text = AsUserInfo.Instance.SavedCharStat.nGold.ToString( "#,#0", CultureInfo.InvariantCulture);

		if( true == m_bHaveFirstClickedRay)
		{
			if( Time.time - m_fFirstClickedTime > doubleClickDownTime)
			{
				GuiInputClickUp( m_FirstClickedRay);
				m_bHaveFirstClickedRay = false;
			}
		}

		if( null != m_MenuBtnAction)
		{
			if( true == m_MenuBtnAction.getExpanded)
			{
				if( Time.time - m_fCurTime >= maxCloseTime)
					m_MenuBtnAction.Expand();
			}
		}

		UpdateItemTimeExpire();

		if (questMoveCoolTime != null)
			questMoveCoolTime.Update();

		if(AsLobiSDKManager.Instance.IsCapturing)
		{
			if( ( Time.time - m_fFadeOutTime) > 0.5f)
			{
				m_bFadeOut =!m_bFadeOut;
				if(m_bFadeOut)
					lobiRecImage.gameObject.SetActiveRecursively(true);
				else
					lobiRecImage.gameObject.SetActiveRecursively(false);
				m_fFadeOutTime = Time.time;
			}
		}
	}

	private void OpenPlayerStatusDlgButton()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsChatFullPanel.Instance.Close();

		if( false == IsOpenPlayerStatus)
			OpenPlayerStatusDlg();
		else
			ClosePlayerStatus();
	}

	#region -DelegateImage
	private void OpenDelegateImageDlg()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsUserInfo.Instance.IsDied())
			return;
		
		AsChatFullPanel.Instance.Close();

		delegateImageDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_DelegateImage")) as GameObject;
		Debug.Assert( null != delegateImageDlg);

		if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.OPEN_REPRESENTATIVE_IMAGE) != null)
			AsCommonSender.SendClearOpneUI( OpenUIType.OPEN_REPRESENTATIVE_IMAGE);
	}

	public void CloseDelegateImageDlg()
	{
		if( null == delegateImageDlg)
			return;

		Destroy( delegateImageDlg);
		delegateImageDlg = null;
	}
	#endregion

	public void FirstMouseInput( Camera uiCamera)
	{
		if( Input.GetMouseButtonDown(0) == true)
		{
		}
		else if( Input.GetMouseButton(0) == true)
		{
		}
		else if( Input.GetMouseButtonUp(0) == true)
		{
			if( null != TooltipMgr.Instance)
				TooltipMgr.Instance.Clear();
		}
	}

	public void FirstTouchInput( Camera uiCamera)
	{
		if( Input.touchCount == 1)
		{
			if( Input.GetTouch(0).phase == TouchPhase.Began)
			{
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
			{
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{
				if( null != TooltipMgr.Instance)
					TooltipMgr.Instance.Clear();
			}
		}
		else if( Input.touchCount >= 2)
		{
			if( Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch( 1).phase == TouchPhase.Began)
			{
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Moved	 || Input.GetTouch( 1).phase == TouchPhase.Moved &&
				Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch( 1).phase == TouchPhase.Stationary)
			{
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled &&
				Input.GetTouch( 1).phase == TouchPhase.Ended || Input.GetTouch( 1).phase == TouchPhase.Canceled)
			{
				if( null != TooltipMgr.Instance)
					TooltipMgr.Instance.Clear();
			}
		}
	}

	// input update
	public void MouseInput( Camera uiCamera)
	{
		Ray inputRay = uiCamera.ScreenPointToRay( Input.mousePosition);

		if( Input.GetMouseButtonDown(0) == true)
			InputDown( inputRay);
		else if( Input.GetMouseButton(0) == true)
			InputMove( inputRay);
		else if( Input.GetMouseButtonUp(0) == true)
			InputUp( inputRay);
	}

	public void TouchInput( Camera uiCamera)
	{
		if( Input.touchCount == 1)
		{
			Ray inputRay = uiCamera.ScreenPointToRay( Input.GetTouch(0).position);

			if( Input.GetTouch(0).phase == TouchPhase.Began)
				InputDown( inputRay);
			else if( Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
				InputMove( inputRay);
			else if( Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
				InputUp( inputRay);
		}
		else if( Input.touchCount >= 2)
		{
			Ray inputRay = uiCamera.ScreenPointToRay( Input.mousePosition);

			GUIInputTwoTouchDown( inputRay);
		}
	}

	public void GuiMouseInput( Camera uiCamera)
	{
		Ray inputRay = uiCamera.ScreenPointToRay( Input.mousePosition);

		if( Input.GetMouseButtonDown(0) == true)
		{
			GuiInputDown( inputRay);
		}
		else if( Input.GetMouseButton(0) == true)
		{
			GuiInputMove( inputRay);
		}
		else if( Input.GetMouseButtonUp(0) == true)
		{
			if( true == m_bHaveFirstClickedRay)
			{
				GuiInputDClickUp( inputRay);

				m_bHaveFirstClickedRay = false;
			}
			else
			{
				m_bHaveFirstClickedRay = true;
				m_FirstClickedRay = inputRay;
				m_fFirstClickedTime = Time.time;
			}

			GuiInputUp( inputRay);
		}
	}

	public void GuiTouchInput( Camera uiCamera)
	{
		if( Input.touchCount == 1)
		{
			Ray inputRay = uiCamera.ScreenPointToRay( Input.mousePosition);

			if( Input.GetTouch(0).phase == TouchPhase.Began)
			{
				GuiInputDown( inputRay);
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				GuiInputMove( inputRay);
			}
			else if( Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{
				if( true == m_bHaveFirstClickedRay)
				{
					GuiInputDClickUp( inputRay);
					m_bHaveFirstClickedRay = false;
				}
				else
				{
					m_bHaveFirstClickedRay = true;
					m_FirstClickedRay = inputRay;
					m_fFirstClickedTime = Time.time;
				}

				GuiInputUp( inputRay);
			}
		}
		else if( Input.touchCount >= 2)
		{
			Ray inputRay = uiCamera.ScreenPointToRay( Input.mousePosition);
			GUIInputTwoTouchDown( inputRay);
		}
	}

	private void GuiInputDown( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.GuiInputDown( inputRay);

		if( null != npcStore)
			npcStore.InputDown( inputRay);

		if( null != cashStore)
			cashStore.InputDown( inputRay);

		if( null != storageDlg)
			storageDlg.GuiInputDown( inputRay); //$ yde - 20130115

		if( null != playerStatusDlg)
			playerStatusDlg.GuiInputDown( inputRay);
		if( null != tradeDlg)
			tradeDlg.GuiInputDown( inputRay); // ilmeda, 20120914
		if( IsOpenEnchantDlg)
			enchantDlg.GuiInputDown( inputRay);
		AsQuickSlotManager.Instance.GuiInputDown( inputRay);
		if( null != skillBookDlg)
			skillBookDlg.GuiInputDown( inputRay);
		if( IsOpenStrengthenDlg)
			strengthenDlg.GuiInputDown( inputRay);
		AsSocialManager.Instance.InputDown( inputRay);
		AsPartyManager.Instance.GuiInputDown( inputRay);
		if( null != postBoxDlg)
			postBoxDlg.GuiInputDown( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.GuiInputDown( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.GuiInputDown( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.GuiInputDown( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.GuiInputDown( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.GuiInputDown( inputRay);

		if( true == IsOpenWorldMapDlg)
			worldMapDlg.GuiInputDown( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.GuiInputDown( inputRay);
	}

	private void GuiInputMove( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.GuiInputMove( inputRay);
		if( null != storageDlg)
			storageDlg.GuiInputMove( inputRay); //$ yde - 20130115

		if( null != playerStatusDlg)
			playerStatusDlg.GuiInputMove( inputRay);
		if( null != tradeDlg)
			tradeDlg.GuiInputMove( inputRay); // ilmeda, 20120914
		AsQuickSlotManager.Instance.GuiInputMove( inputRay);
		if( null != skillBookDlg)
			skillBookDlg.GuiInputMove( inputRay);
		if( null != postBoxDlg)
			postBoxDlg.GuiInputMove( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.GuiInputMove( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.GuiInputMove( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.GuiInputMove( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.GuiInputMove( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.GuiInputMove( inputRay);

		if( true == IsOpenWorldMapDlg)
			worldMapDlg.GuiInputMove( inputRay);

		if( true == IsOpenProductionDlg)
			productionDlg.GuiInputDown( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.GuiInputMove( inputRay);
	}

	private void GuiInputUp( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.GuiInputUp( inputRay);
		if( null != storageDlg) storageDlg.GuiInputUp( inputRay); //$ yde - 20130416
		AsQuickSlotManager.Instance.GuiInputUp( inputRay);
		if( null != skillBookDlg)
			skillBookDlg.GuiInputUp( inputRay);
		if( IsOpenStrengthenDlg)
			strengthenDlg.GuiInputUp( inputRay);
		AsPartyManager.Instance.GuiInputUp( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.GuiInputUp( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.GuiInputUp( inputRay);
		

		if( true == IsOpenWorldMapDlg)
			worldMapDlg.GuiInputUp( inputRay);

		if( IsOpenRandItemUI)
			randItemUI.GuiInputUp( inputRay);
	}

	private void GUIInputTwoTouchDown( Ray inputRay)
	{
		if( IsOpenNpcStore == true)
			npcStore.InputOverTwoTouchUp( inputRay);
		if( IsOpenCashStore == true)
			cashStore.InputOverTwoTouchUp( inputRay);
	}

	private void GuiInputClickUp( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.GuiInputClickUp( inputRay);
		if( null != storageDlg)
			storageDlg.GuiInputClickUp( inputRay); //$ yde - 20130115
		if( null != playerStatusDlg)
			playerStatusDlg.GuiInputUp( inputRay);
		if( null != tradeDlg)
			tradeDlg.GuiInputUp( inputRay); // ilmeda, 20120914
		if( IsOpenEnchantDlg)
			enchantDlg.GuiInputUp( inputRay);
		if( null != postBoxDlg)
			postBoxDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.GuiInputUp( inputRay);
		
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.GuiInputUp( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.GuiInputUp( inputRay);

		if( true == IsOpenProductionDlg)
			productionDlg.GuiInputUp( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.GuiInputUp( inputRay);

        if( null != rankingDlg)
            rankingDlg.GuiInputUp( inputRay);
		
		BonusManager.Instance.GuiInputClickUp( inputRay );
	}

	private void GuiInputDClickUp( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.GuiInputDClickUp( inputRay);
		if( null != storageDlg)
			storageDlg.GuiInputDClickUp( inputRay); //$ yde - 20130115
		if( null != playerStatusDlg)
			playerStatusDlg.GuiInputDClickUp( inputRay);
		if( null != tradeDlg)
			tradeDlg.GuiInputDClickUp( inputRay); // ilmeda, 20120914
		if( null != postBoxDlg)
			postBoxDlg.GuiInputDClickUp( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.GuiInputDClickUp( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.GuiInputDClickUp( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.GuiInputDClickUp( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.GuiInputDClickUp( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.GuiInputDClickUp( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.GuiInputDClickUp( inputRay); //$ yde - 20130115

		AsUseItemInMapMarkManager.instance.ProcessDoubleTap( inputRay);
		AsUseItemToTargetPanelManager.instance.ProcessDoubleTap( inputRay);
		AsUseItemToMonTriggerManager.instance.ProcessDoubleTap( inputRay);
	}

	private void InputDown( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.InputDown( inputRay);
		if( null != npcStore)
			npcStore.InputDown( inputRay);
		if( null != cashStore)
			cashStore.InputDown( inputRay);
		if( null != storageDlg)
			storageDlg.InputDown( inputRay); //$ yde - 20130115

		if( null != playerStatusDlg)
			playerStatusDlg.GuiInputDown( inputRay);
		if( null != tradeDlg)
			tradeDlg.InputDown( inputRay); // ilmeda, 20120914
		AsSocialManager.Instance.InputDown( inputRay);
		AsPartyManager.Instance.InputDown( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.InputDown( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.InputDown( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.InputDown( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.InputDown( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.InputDown( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.InputDown( inputRay); //$ yde - 20130115
	}

	private void InputMove( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.InputMove( inputRay);
		if( null != npcStore)
			npcStore.InputMove( inputRay);
		if( null != cashStore)
			cashStore.InputMove( inputRay);

		if( null != storageDlg)
			storageDlg.InputMove( inputRay); //$ yde - 20130115

		if( null != playerStatusDlg)
			playerStatusDlg.InputMove( inputRay);
		if( null != tradeDlg)
			tradeDlg.InputMove( inputRay); // ilmeda, 20120914
		AsQuickSlotManager.Instance.InputMove( inputRay);
		if( null != skillBookDlg)
			skillBookDlg.InputMove( inputRay);
		if( null != postBoxDlg)
			postBoxDlg.InputMove( inputRay);
		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.InputMove( inputRay);
		
		if( true == IsOpenSynCosDlg)
			m_SynCosDlg.InputMove( inputRay);
		if( true == IsOpenSynDisDlg)
			m_SynDisDlg.InputMove( inputRay);
		if( true == IsOpenSynEnchantDlg)
			m_SynEnchantDlg.InputMove( inputRay);
		if( true == IsOpenSynOptionDlg)
			m_SynOptionDlg.InputMove( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.InputMove( inputRay); //$ yde - 20130115
	}

	private void InputUp( Ray inputRay)
	{
		if( IsOpenInven)
			invenDlg.InputUp( inputRay);
		if( null != npcStore)
			npcStore.InputUp( inputRay);
		if( null != cashStore)
			cashStore.InputUp( inputRay);

		if( null != storageDlg)
			storageDlg.InputUp( inputRay); //$ yde - 20130115

		if( null != playerStatusDlg)
			playerStatusDlg.InputUp( inputRay);
		if( null != tradeDlg)
			tradeDlg.InputUp( inputRay); // ilmeda, 20120914
		AsQuickSlotManager.Instance.InputUp( inputRay);
		if( null != skillBookDlg)
			skillBookDlg.InputUp( inputRay);
		if( null != postBoxDlg)
			postBoxDlg.InputUp( inputRay);

		AsSocialManager.Instance.InputUp( inputRay);

		if( true == IsOpenSynthesisDlg)
			m_SynthesisDlg.InputUp( inputRay);

		if( null != pstoreDlg)
			pstoreDlg.InputUp( inputRay); //$ yde - 20130115
	}

	public void RecvSkillReset()
	{
		CloseSkillBook();
		CloseSkillshop();

		if( IsOpenInven)
			invenDlg.OpenSkillResetSucceeded();
	}

	public bool _isOpenNpcMenu()
	{
		if( null != AsHUDController.Instance.m_NpcMenu)
			return AsHUDController.Instance.m_NpcMenu.gameObject.active;

		return false;
	}
	
	private void _CloseNpcMenu()
	{
		if( null != AsHUDController.Instance.m_NpcMenu)
			AsHUDController.Instance.m_NpcMenu.Close();
	}

	public void OpenNeedSphereDlg()
	{
		if( AsGameMain.useCashShop == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), this, "OpenSphereNeed", "",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
		}
		else
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), AsNotify.MSG_BOX_TYPE.MBT_OK));
	}

	public void OpenNeedGoldDlg()
	{
		if( AsGameMain.useCashShop == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(1575), AsTableManager.Instance.GetTbl_String(28), AsNotify.MSG_BOX_TYPE.MBT_OK));
		}
		else
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(388), AsNotify.MSG_BOX_TYPE.MBT_OK));
		}
	}

	public void OpenSphereNeed()
	{
		AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);
	}

	public void OpenCashStore()
	{
		//2014.05.16
		if( true == WemeSdkManager.Instance.IsServiceGuest)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}
		
		AsSoundManager.Instance.PlaySound( AsSoundPath.CashshopButton, Vector3.zero, false);

		AsChatFullPanel.Instance.Close();

		AsHudDlgMgr.Instance.CloseRecommendInfoDlg();	// #17479

		AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.NONE, eCashStoreSubCategory.NONE, 0, true);
	}

	public void SetCashStoreBtnFreeMark(bool _value)
	{
		if (cashShopBtn != null)
		{
			CashShopBtnController btnController = cashShopBtn.GetComponent<CashShopBtnController>();
			if (btnController != null)
				btnController.SetFreeMark(_value);
		}
	}

	private void OnInstantDungeonBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if (IsOpenWorldMapDlg == true)
			return;

		AsChatFullPanel.Instance.Close();

		AsInstanceDungeonManager.Instance.OnBtnIndunInfo();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}

	private void _Init_IndunBtn()
	{
		if( true == AsInstanceDungeonManager.Instance.isMatching)
		{
			InstantDungeonBtn.Text = AsTableManager.Instance.GetTbl_String(900);
			AsInstanceDungeonManager.Instance.SetActiveMatchingEff( true);
		}
		else
		{
			//InstantDungeonBtn.Text = AsTableManager.Instance.GetTbl_String(1689);
			//AsInstanceDungeonManager.Instance.SetActiveMatchingEff( false);

			if( true == TargetDecider.CheckCurrentMapIsIndun())
				InstantDungeonBtn.Text = AsTableManager.Instance.GetTbl_String( 2346); // indun exit text
			else
				InstantDungeonBtn.Text = AsTableManager.Instance.GetTbl_String(1689);

			AsInstanceDungeonManager.Instance.SetActiveMatchingEff( false);
		}

		InstantDungeonBtn.SetInputDelegate( OnInstantDungeonBtn);
		//InstantDungeonBtn.gameObject.SetActiveRecursively( true);
		if( AsUserInfo.Instance.SavedCharStat.level_ >= 8)
			InstantDungeonBtn.gameObject.SetActive( true);
		else
			InstantDungeonBtn.gameObject.SetActive( false);
	}
	
	void OnLevelUpBonusBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		if( true == IsOpenPlayerStatus)
			ClosePlayerStatus();

		if( true == IsOpenInven)
			CloseInven();
		
		if( true == IsOpenOptionDlg)
			CloseOptionDlg();
		
		if( true == IsOpenedSkillBook)
			CloseSkillBook();
		
		if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
			AsPartyManager.Instance.PartyUI.ClosePartyList();
		
		if( true == IsOpenQuestBook)
			CloseQuestBook();
		
		if( true == IsOpenSynthesisDlg )		
			CloseSynthesisDlg();

		if( true == IsOpenedPostBox)
			ClosePostBoxDlg();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		BonusManager.Instance.OpenLevelBonusWindow();
	} 

	public void SetPvpBtnText( string strText)
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetBtnText( eMainBtnType.PVP , strText );
		}
	}
	
	public void SetPvpBtnRollingEffect( bool isEffect )
	{
		if( IsOpenMainMenuDlg == true )
		{
			m_MainMenuDlg.SetRollingEffect( eMainBtnType.PVP , isEffect );
		}
	}
	
	public void SetIndunBtnText(string strText)
	{
		InstantDungeonBtn.Text = strText;
	}

	public void SetIndunLimitCount(short nLimitCount)
	{
		IndunLimitCount.Text = nLimitCount.ToString();
	}

	public PlayerStatusDlg	GetPlayerStatusDlg()
	{
		return playerStatusDlg;
	}
	
//	public void PlayerLevelUp()
//	{
//		if(AsUserInfo.Instance.SavedCharStat.level_ > Tbl_UserLevel_Table.FirstLevelUpRewardLv)
//		{
////			lvUpBonusBtn.transform.position = lvUpBtnPos;
//
//			BonusManager.Instance.SetLevelUpBonusBtnActive(true);
//		}
//	}
}