using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsMainMenuDlg : MonoBehaviour 
{
	private GameObject m_goRoot = null;
	
	public SpriteText 	m_TextTitle;
	
	public UIButton postBtn;
	public UIButton skillBtn;
	public UIButton productBtn;
	public UIButton guildBtn;
	public UIButton partyBtn;
	public UIButton socialBtn;
	public UIButton searchStoreBtn;
	public UIButton rankingBtn;
	public UIButton eventBtn;
	public UIButton synthesisBtn;
	public UIButton noticeBtn;
	public UIButton systemBtn;
	public UIButton lobbyBtn;
	public UIButton pvpBtn;
	
	[SerializeField] UIButton petBtn;
	
	public GameObject	pvpRollingEffect;
	
	Dictionary<eMainBtnType , UIButton>		m_dicBtn = new Dictionary<eMainBtnType, UIButton>();
	
	Dictionary<int, GameObject> m_dicQuestRelationUI = new Dictionary<int, GameObject>();
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		
		_Init();
		
		m_goRoot = goRoot;

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.TAP_MENU_BTN));
	}
	
	public void Close()
	{
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_OPTION_MENU));
		gameObject.SetActiveRecursively( false);
		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	public void CloseForQuestTutorial()
	{
		gameObject.SetActiveRecursively(false);
		if (null != m_goRoot)
			Destroy(m_goRoot);
	}
	
	void AddButton( eMainBtnType _type , UIButton _btn , string _text , EZInputDelegate _del , bool isNewImg = false , GameObject goRollingEffect = null , bool isRollingEffect = false )
	{
		if( m_dicBtn.ContainsKey(_type) == true )
			return;
		
		GameObject	go = _btn.gameObject;
		
		_btn.Text = _text;
		_btn.SetInputDelegate( _del );
		
		AsButtonEffect btnEffect = go.AddComponent<AsButtonEffect>();
		
		m_dicBtn.Add( _type , _btn );
		
		if( isNewImg == true )
			SetNewImg( _type , isNewImg );
		
		if( goRollingEffect != null )
		{
			btnEffect.RollingEffect = goRollingEffect;
			btnEffect.SetRollingEffect( isRollingEffect );
		}
	}
	
	// < private
	private void _Init()
	{
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(821);
		
		AddButton( eMainBtnType.POST 		, postBtn 		, AsTableManager.Instance.GetTbl_String(824) , PostBtnDelegate 		, AsUserInfo.Instance.NewMail );
		AddButton( eMainBtnType.SKILL 		, skillBtn 		, AsTableManager.Instance.GetTbl_String(823) , SkillBtnDelegate 	, SkillBook.Instance.isNewSkillAdd );
		AddButton( eMainBtnType.PRODUCT		, productBtn	, AsTableManager.Instance.GetTbl_String(822) , MakingBtnDelegate	, AsUserInfo.Instance.IsHaveProductionInfoComplete() );
		
		AddButton( eMainBtnType.GUILD		, guildBtn		, AsTableManager.Instance.GetTbl_String(827) , GuildBtnDelegate );
		AddButton( eMainBtnType.PARTY		, partyBtn		, AsTableManager.Instance.GetTbl_String(826) , PartyBtnDelegate );
		AddButton( eMainBtnType.SOCIAL		, socialBtn		, AsTableManager.Instance.GetTbl_String(825) , CommunityBtnDelegate );
		
		AddButton( eMainBtnType.SEARCH_STORE, searchStoreBtn, AsTableManager.Instance.GetTbl_String(2159) , PStoreSearchBtnDelegate );
		AddButton( eMainBtnType.RANKING		, rankingBtn	, AsTableManager.Instance.GetTbl_String(1662) , OnRankBtn );
		AddButton( eMainBtnType.EVENT		, eventBtn		, AsTableManager.Instance.GetTbl_String(1100) , OnEventList );
		
		AddButton( eMainBtnType.SYNTHESIS	, synthesisBtn	, AsTableManager.Instance.GetTbl_String(1223) , OpenSynthesisMainDlg );
		AddButton( eMainBtnType.NOTICE		, noticeBtn		, AsTableManager.Instance.GetTbl_String(1255) , OpenNoticeDlg );
		AddButton( eMainBtnType.SYSTEM		, systemBtn		, AsTableManager.Instance.GetTbl_String(828)  , SystemBtnDelegate );
		
		AddButton( eMainBtnType.LOBBY		, lobbyBtn		, AsTableManager.Instance.GetTbl_String(1792) , OnLobbyBtn );
		
		AddButton( eMainBtnType.PET		, petBtn		, AsTableManager.Instance.GetTbl_String(2740) , OnPetBtn );
		
		string textPVP = "";
		bool isRollingEffect = false;
		if( true == AsPvpManager.Instance.isMatching)
		{
			textPVP = AsTableManager.Instance.GetTbl_String(900);
			isRollingEffect = true;
		}
		else
		{
			textPVP = AsTableManager.Instance.GetTbl_String(883);
			isRollingEffect = false;
		}
		AddButton( eMainBtnType.PVP			, pvpBtn		, textPVP 									, OnPvpBtn , false , pvpRollingEffect , isRollingEffect );
		
		//	quest relation ui
		CreateQuestRelationUIRecord();
		UpdateQuestRelationUI();


		if (AsReviewConditionManager.Instance.IsReviewCondition (eREVIEW_CONDITION.MARKETING_BANNER) == false) 
		{
			noticeBtn.gameObject.SetActive(false);
			lobbyBtn.gameObject.SetActive(false);
		}
	}
	
	public void SetNewImg( eMainBtnType _type , bool _isActive)
	{
		if( m_dicBtn.ContainsKey(_type) == true )
		{
			AsButtonEffect	btnEffect = m_dicBtn[_type].gameObject.GetComponent<AsButtonEffect>();
			if( btnEffect != null )
				btnEffect.SetNewImg( _isActive );
		}
	}	

	public void SetRollingEffect( eMainBtnType _type , bool _isActive)
	{
		if( m_dicBtn.ContainsKey(_type) == true )
		{
			AsButtonEffect	btnEffect = m_dicBtn[_type].gameObject.GetComponent<AsButtonEffect>();
			if( btnEffect != null )
				btnEffect.SetRollingEffect(_isActive);
		}
	}	
	
	public void SetBtnText( eMainBtnType _type , string _text )
	{
		if( m_dicBtn.ContainsKey(_type) == true )
		{
			m_dicBtn[_type].Text = _text;
		}
	}
	
	public UIButton GetBtn( eMainBtnType _type )
	{
		if( m_dicBtn.ContainsKey(_type) == true )
			return m_dicBtn[_type];
		
		return null;
	}
	
	void CreateQuestRelationUIRecord()
	{
		m_dicQuestRelationUI.Clear();

		eCLASS nowClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);

		Dictionary<int, PrepareOpenUIType> dic = AsTableManager.Instance.GetTbl_PrepareOpenUI( nowClass);

		GameObject objUI = null;
		foreach( KeyValuePair<int, PrepareOpenUIType> pair in dic)
		{
			switch( pair.Value)
			{
			case PrepareOpenUIType.PVP:
				UIButton pvpButton = GetBtn( eMainBtnType.PVP );
				objUI = pvpButton.gameObject;
				break;
			default:
				continue;
			}

			m_dicQuestRelationUI.Add( pair.Key, objUI);
		}
	}	
	
	public void UpdateQuestRelationUI( int _questID = -1)
	{
		if( _questID == -1)
		{
			foreach( KeyValuePair<int, GameObject> keyPair in m_dicQuestRelationUI)
			{
				if( keyPair.Value == null )
					continue;
				
				if( ArkQuestmanager.instance.IsNothingQuest( keyPair.Key) == true)
				{
					keyPair.Value.SetActive(false);
				}
				else
				{
					if( keyPair.Value.activeSelf == false)
					{
						keyPair.Value.SetActive(true);
						ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", keyPair.Value.transform, Vector3.zero);
					}
				}
			}
		}
		else
		{
			if( m_dicQuestRelationUI.ContainsKey( _questID) == true && m_dicQuestRelationUI[_questID] != null )
			{
				if( ArkQuestmanager.instance.IsNothingQuest( _questID) == true)
				{
					m_dicQuestRelationUI[_questID].SetActive(false);
				}
				else
				{
					if( m_dicQuestRelationUI[_questID].active == false)
					{
						m_dicQuestRelationUI[_questID].SetActive(true);
						ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", m_dicQuestRelationUI[_questID].transform, Vector3.zero);
					}
				}
			}
		}
	}	
	
	#region btnDelegate	
	private void _BtnDelegate_Cancel(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void PostBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			//2014.05.16
			if( true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu() )
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}

			Debug.Log( "PostBtnDelegate");

			AsHudDlgMgr.Instance.CloseSynthesisDlg();

			BonusManager.Instance.CloseLevelUpWindow();

			if( false == AsHudDlgMgr.Instance.IsOpenedPostBox)
				AsHudDlgMgr.Instance.OpenPostBoxDlg();
			else
				AsHudDlgMgr.Instance.ClosePostBoxDlg();
			
			Close();
		}
	}		
	
	private void SkillBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsChatFullPanel.Instance.Close();

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsHudDlgMgr.Instance.IsOpenedSkillBook)
				AsHudDlgMgr.Instance.CloseSkillBook();
			else
				AsHudDlgMgr.Instance.OpenSkillBook();

			Close();
		}
	}
	
	private void MakingBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;
			
			//2014.05.16
			if( true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( true == AsUserInfo.Instance.IsDied())
				return;
			
			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}
			
			if( false == AsHudDlgMgr.Instance.IsOpenProductionDlg)
			{
				AsHudDlgMgr.Instance.SendProductionDlgOpen = true;
				AsCommonSender.SendItemProductInfo();
			}
			else
			{
				AsHudDlgMgr.Instance.CloseProductionDlg();
			}

			//Close();
		}
	}	
	
	private void GuildBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Tutorial))
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2078),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}

			if( true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( true == AsUserInfo.Instance.IsDied())
				return;
			
			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( true == AsCommonSender.isSendWarp)
			{
				Debug.LogWarning( "return is send Warp");
				return;
			}
			
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}
			
			if( true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;
			
			bool bRequestGuildList = false;
			if( null == AsUserInfo.Instance.GuildData)
				bRequestGuildList = true;

			if( null == AsHudDlgMgr.Instance.GuildDlg )
				AsHudDlgMgr.Instance.OpenGuildDlg(bRequestGuildList);
			else
				AsHudDlgMgr.Instance.CloseGuildDlg();
			
			Close();
		}
	}
	
	private void PartyBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Tutorial))
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2078),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}

			//2014.05.16
			if( true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( true == AsUserInfo.Instance.IsDied())
				return;

			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}
	
			if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;

			if( true == AsPartyManager.Instance.PartyUI.IsOpenPartyList)
				AsPartyManager.Instance.PartyUI.ClosePartyList();
			else
				AsPartyManager.Instance.PartyUI.OpenPartyList();

			//Close();
		}
	}	
	
	private void CommunityBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsChatFullPanel.Instance.Close();

			//#21465
			if( true == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Tutorial))
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2078),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}

			//2014.05.16
			if( true == WemeSdkManager.Instance.IsServiceGuest)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1923), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( true == AsUserInfo.Instance.IsDied())
				return;
			
			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}
			
			if( true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( true == AsHudDlgMgr.Instance.IsOpenTrade)
				return;

			if( true == AsHudDlgMgr.Instance.IsOpenedSkillshop)
				AsHudDlgMgr.Instance.CloseSkillshop();

			if( true == AsHudDlgMgr.Instance.IsOpenEnchantDlg)
				AsHudDlgMgr.Instance.CloseEnchantDlg();

			if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				AsHudDlgMgr.Instance.ClosePlayerStatus();

			if( true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
				AsHudDlgMgr.Instance.CloseStrengthenDlg();

			if( true == AsHudDlgMgr.Instance.IsOpenedPostBox)
				AsHudDlgMgr.Instance.ClosePostBoxDlg();

			if( true == AsHudDlgMgr.Instance.IsOpenInstantDungeonDlg)
				AsHudDlgMgr.Instance.CloseInstantDungeonDlg();

			if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
				AsPvpDlgManager.Instance.ClosePvpDlg();

			AsSocialManager.Instance.OpenSocailDlg();
			AsSocialManager.Instance.OpenFindFriendDlg();
			
		//	Close();
		}
	}	
	
	private void PStoreSearchBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsPStoreManager.Instance.OpenPStoreSearch();
			
			Close();
		}
	}
	
	private void OnRankBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		if( AsPStoreManager.Instance.storeState != ePStoreState.Closed)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			
			return;
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		AsChatFullPanel.Instance.Close();

		//#21465
		if( true == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Tutorial))
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2078),
								null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		body_CS_RANK_SUMMARY_MYRANK_LOAD rankLoad = new body_CS_RANK_SUMMARY_MYRANK_LOAD( eRANKTYPE.eRANKTYPE_ITEM);
		byte[] data = rankLoad.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		
		Close();
	}	
	
	private void OnEventList( ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPStoreManager.Instance.storeState == ePStoreState.Closed)
				AsHudDlgMgr.Instance.OpenInGameEventListDlg();
			else
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
					AsTableManager.Instance.GetTbl_String(365), AsNotify.MSG_BOX_TYPE.MBT_OK);
			
			Close();
		}
	}
	
	public void OpenSynthesisMainDlg(ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		AsChatFullPanel.Instance.Close();		

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

		if( true == AsHudDlgMgr.Instance.IsOpenedPostBox)
			AsHudDlgMgr.Instance.ClosePostBoxDlg();

		if( true == AsHudDlgMgr.Instance.IsOpenSynthesisDlg )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenSynCosDlg )
			AsHudDlgMgr.Instance.CloseSynCosDlg();
		if( true == AsHudDlgMgr.Instance.IsOpenSynDisDlg )
			AsHudDlgMgr.Instance.CloseSynDisDlg();
		if( true == AsHudDlgMgr.Instance.IsOpenSynEnchantDlg )
			AsHudDlgMgr.Instance.CloseSynEnchantDlg();
		if( true == AsHudDlgMgr.Instance.IsOpenSynOptionDlg )
			AsHudDlgMgr.Instance.CloseSynOptionDlg();

		BonusManager.Instance.CloseLevelUpWindow();

		AsHudDlgMgr.Instance.OpenSynthisisDlg_Coroutine();	
		
		Close();
	}	
	
	public void OpenNoticeDlg(ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		GameObject noticesDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_IntroEvent")) as GameObject;
		AsHudDlgMgr.Instance.NoticeDlg = noticesDlg.GetComponentInChildren<AsNoticesDlg>();
		Debug.Assert( null != AsHudDlgMgr.Instance.NoticeDlg);
		
		AsHudDlgMgr.Instance.NoticeDlg.ReOpen();
		
		Close();
	}		
	
	private void SystemBtnDelegate( ref POINTER_INFO ptr)
	{
		if( POINTER_INFO.INPUT_EVENT.TAP == ptr.evt)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsChatFullPanel.Instance.Close();

			if( true == AsUserInfo.Instance.IsDied())
				return;
			
			if( true == AsHudDlgMgr.Instance._isOpenNpcMenu())
				return;

			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.PopUpDisableByPStore();
				return;
			}

			Debug.Log( "SystemBtnDelegate");

			if( false == AsHudDlgMgr.Instance.IsOpenSystemDlg)
			{
				AsHudDlgMgr.Instance.OpenSystemDlg();
			}
			else
			{
				AsHudDlgMgr.Instance.CloseSystemDlg();
			}
			
			//Close();
		}
	}	
	
	private void OnLobbyBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;
		
		if( false == AsHudDlgMgr.Instance.IsOpenLobiRecDlg)
		{
			AsHudDlgMgr.Instance.OpenLobiRecDlg();
		}
		else
		{
			AsHudDlgMgr.Instance.CloseLobiRecDlg();
		}
	}
	
	private void OnPvpBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		AsChatFullPanel.Instance.Close();

		AsPvpManager.Instance.OnBtnPvpInfo();
		
		Close();
	}
	
	private void OnPetBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		string content = "not set";
		bool indun = TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Indun) == true;
		bool pvp = TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Pvp) == true;
		if(indun == true || pvp == true)
		{
			string title = AsTableManager.Instance.GetTbl_String( 126);

			if(indun == true) content = AsTableManager.Instance.GetTbl_String(2286);
			if(pvp == true) content = AsTableManager.Instance.GetTbl_String(903);

			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(content);
			return;
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsChatFullPanel.Instance.Close();

		AsPetManager.Instance.PetManagerBtnClicked();
		
		Close();
	}
	#endregion
		
		
	// Update is called once per frame
	void Update () {
	
	}
}
