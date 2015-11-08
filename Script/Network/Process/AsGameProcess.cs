#define _USE_PARTY_WARP
#define INTEGRATED_ARENA_MATCHING
using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;

public class AsGameProcess : AsProcessBase
{
	private bool m_bLoginSelectResponse = false;

	public override void Process( byte[] _packet)
	{

		AsNetworkManager.Instance.ResetSendLiveTime();
		try
		{
			// 서버와의 데이터오더 문제로 3이어야 하나 2로 해야 Protocol이 얻어짐.
			switch( ( PROTOCOL_GAME)_packet[2])
			{
			case PROTOCOL_GAME.GC_TYPE_TEST_ECHO:
				break;
			case PROTOCOL_GAME.GC_LOGIN_POSSIBLE_RESULT:
				GameLoginPossibleResult( _packet);
				break;
			case PROTOCOL_GAME.GC_LOGIN_RESULT:
				GameLoginResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHAR_LOAD_RESULT:
				CharacterLoadResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHAR_CREATE_RESULT:
				CharacterCreateResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHAR_DELETE_RESULT:
				CharacterDeleteResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHAR_SELECT_RESULT:
				CharacterSelectResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHAR_SLOT_ADD_RESULT:
				CharSlotAddResult( _packet);
				break;
#region -ChannelSelect
			case PROTOCOL_GAME.GC_CHANNEL_AUTO_SELECT:
				ChannelAutoSelect( _packet);
				break;
			case PROTOCOL_GAME.GC_CHANNEL_LIST:
				ChannelListResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHANNEL_SELECT_RESULT:
				ChannelSelectResult( _packet);
				break;
#if _USE_PARTY_WARP	
			case PROTOCOL_GAME.GC_CHANNEL_INFO:
				ChannelInfo( _packet);
				break;		
#endif
#endregion
			case PROTOCOL_GAME.GC_ENTER_WORLD_RESULT:
				EnterWorldResult( _packet);
				break;
#if USE_INSTANCE_SERVER
			case PROTOCOL_GAME.GC_INSTANCE_CREATE_RESULT:
				InstanceCreateResult( _packet);
				break;
#endif
#if USE_INSTANCE_SERVER
			case PROTOCOL_GAME.GC_RETURN_WORLD_RESULT:
				ReturnWorldresult( _packet);
				break;
#endif
			case PROTOCOL_GAME.GC_RETURN_CHARSELECT_RESULT:
				ReturnCharSelectResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHEAT_RESULT:
				CheatResult( _packet);
				break;
			case PROTOCOL_GAME.GC_CHEAT_HIDE_NOTIFY://$yde
				HideProcess(_packet);
				break;
			case PROTOCOL_GAME.GC_ACCOUNT_DUPLICATE:
				AccountDuplicate( _packet);
				break;
				//event
			case PROTOCOL_GAME.GC_RECOMMEND :
				Recommend( _packet);
				break;
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( e);
		}
	}

	void Update()
	{
		if( false == m_bLoginSelectResponse)
			return;

		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( 0 >= AsUserInfo.Instance.CreatedCharacterCount)
			{
				if( true == AssetbundleManager.Instance.isLoadedScene( "CharacterCreate"))
				{
					AsGameMain.createCharacterSlot = 0;
					Application.LoadLevel( "CharacterCreate");
					DDOL_Tracer.BeginTrace();//$ yde
					Resources.UnloadUnusedAssets();
					m_bLoginSelectResponse = false;
				}
			}
			else
			{
				if( true == AssetbundleManager.Instance.isLoadedScene( "CharacterSelect"))
				{
					Application.LoadLevel( "CharacterSelect");
					DDOL_Tracer.BeginTrace();//$ yde
					Resources.UnloadUnusedAssets();
					m_bLoginSelectResponse = false;
				}
			}
		}
		else
		{
			if( 0 >= AsUserInfo.Instance.CreatedCharacterCount)
			{
				AsGameMain.createCharacterSlot = 0;
				Application.LoadLevel( "CharacterCreate");
			}
			else
			{
				Application.LoadLevel( "CharacterSelect");
			}
			DDOL_Tracer.BeginTrace();//$ yde
			Resources.UnloadUnusedAssets();
			m_bLoginSelectResponse = false;
		}
	}

	private void GameLoginPossibleResult( byte[] _packet)
	{
		Debug.Log( "GameLoginPossibleResult");
		
#if INTEGRATED_ARENA_MATCHING
		if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState)
			return;
#endif

		body_GC_LOGIN_POSSIBLE_RESULT result = new body_GC_LOGIN_POSSIBLE_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsServerListBtn.ConnectionConfirmed();
			break;
		case eRESULTCODE.eRESULT_FAIL:
		default:
			AsServerListBtn.CreateStandbyDlg( result);
			break;
		}
	}

	private void GameLoginResult( byte[] _packet)
	{
		Debug.Log( "GameLoginResult");

		AS_GC_LOGIN_RESULT result = new AS_GC_LOGIN_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_DIS_DBAGENT:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(394), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		case eRESULTCODE.eRESULT_FAIL_VERSION:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(395), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		case eRESULTCODE.eRESULT_FAIL_ACCOUNT_DUPLICATE:
			AsLoadingIndigator.Instance.HideIndigator();
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(206), AsTableManager.Instance.GetTbl_String(1446), this, "_ReLogin", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		case eRESULTCODE.eRESULT_FAIL_BAN_LOGIN:
			AsLoadingIndigator.Instance.HideIndigator();
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1653), this, "_ReLogin", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		case eRESULTCODE.eRESULT_FAIL_GM_EDIT:
			AsLoadingIndigator.Instance.HideIndigator();
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1652), this, "_ReLogin", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		default:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(396), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		//$yde
		AsUserInfo.Instance.GamerUserSessionIdx = result.nSessionIdx;
		#region -AccountGender
		AsUserInfo.Instance.accountGender = result.eUserGender;
		#endregion
		#region -GMMark
		AsUserInfo.Instance.isGM = result.bIsGM;
		#endregion
		//2014.05.16
		WemeSdkManager.Instance.IsConfirmGuest = result.bIsGuest;
		Debug.Log( "result.IsConfirmGuest : " + result.bIsGuest);

		AS_CG_CHAR_LOAD charLoad = new AS_CG_CHAR_LOAD();
		byte[] data = charLoad.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void _ReLogin()
	{
		GameObject go = GameObject.Find( "ServerList");
		if( null == go)
			return;

		AsServerListFramework serverList = go.GetComponentInChildren<AsServerListFramework>();
		if( null != serverList)
			serverList.SendMessage( "_Refresh");
	}

	private void ServerLimitOver()
	{
		m_bLoginSelectResponse = true;
	}

	private void CharacterCreateResult( byte[] _packet)
	{
		Debug.Log( "CharacterCreateResult");

		body_GC_CHAR_CREATE_RESULT result = new body_GC_CHAR_CREATE_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_NAME:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1406), AsTableManager.Instance.GetTbl_String(1407), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		case eRESULTCODE.eRESULT_FAIL_COLLISION_NAME:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1406), AsTableManager.Instance.GetTbl_String(129), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		case eRESULTCODE.eRESULT_FAIL_SERVERLIMITOVER:
			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2037), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			msgBox.SetOkDelegate = ServerLimitOver;
			msgBox.SetCancelDelegate = ServerLimitOver;
			return;
		default:
			Debug.LogError( "CharacterCreateResult : " + result.eResult);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(397), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}
		
		AsUserInfo.Instance.ReplaceSlotData( result.sCharView);

		#region -Recommend-
		AsEventUIMgr.Instance.SetRecommend( result.bRecommendEvent,result.nItemTableIdx);
		if( result.bRecommendEvent)
		{
			AsEventUIMgr.Instance.RecommendEventEnd = false;
			AsEventUIMgr.Instance.UserName = AsUtil.GetRealString( ( System.Text.UTF8Encoding.UTF8.GetString( result.sCharView.szCharName)));
			GameObject go = GameObject.Find( "CharacterCreateFramework");
			if( null == go)
				return;

			AsCharacterCreateFramework characterCreateFramework = go.GetComponentInChildren<AsCharacterCreateFramework>();
			if( null != characterCreateFramework)
				characterCreateFramework.SendMessage( "OpenRecommendDlg", 1439);
		}
		#endregion
		else
		{
			m_bLoginSelectResponse = true;

			if( 1 == AsUserInfo.Instance.CreatedCharacterCount)
			{
				Debug.LogError( "CharacterCreateResult");
				AsLoadingIndigator.Instance.ShowIndigator( "");
				
				AsUserInfo.Instance.SetCurrentUserCharacterInfo(0);
				AsCharacterSlotManager.CharacterSelected = true;
				AsCharacterSlotManager.autoSelected = true;
				AsUserInfo.Instance.latestCharSlot = 0;

				AS_CG_CHAR_SELECT select = new AS_CG_CHAR_SELECT(0);
				byte[] data = select.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
			}
		}
	}

	private void CharacterDeleteResult( byte[] _packet)
	{
		body_GC_CHAR_DELETE_RESULT result = new body_GC_CHAR_DELETE_RESULT();
		result.PacketBytesToClass( _packet);

		Debug.Log( "CharacterDeleteResult : " + result.eResult);

		GameObject go = GameObject.Find( "CharacterSlotManager");
		if( null == go)
			Debug.LogError( "CharacterSlotManager is not found!");

		AsCharacterSlotManager slotMng = go.GetComponent<AsCharacterSlotManager>();
		if( null == slotMng)
			Debug.LogError( "null == slotMng");

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			{
				if( 1 == result.nActionType)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat( AsTableManager.Instance.GetTbl_String(16), result.nMonthDeleteCount);
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), sb.ToString(), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				}

				slotMng.UpdateSlotState( result, true);
			}
			break;
		case eRESULTCODE.eRESULT_FAIL_CHAR_DELETE_GUILDMASTER:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(240), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			slotMng.UpdateSlotState( result, false);
			break;
		case eRESULTCODE.eRESULT_FAIL_CHAR_DELETE_MONTHCOUNTOVER:
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( AsTableManager.Instance.GetTbl_String(15), result.nMonthDeleteCount);
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), sb.ToString(), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				slotMng.UpdateSlotState( result, false);
			}
			break;
		default:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(398), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			slotMng.UpdateSlotState( result, false);
			break;
		}
	}

	private void CharacterLoadResult( byte[] _packet)
	{
		Debug.Log( "CharacterLoadResult");

		body1_GC_CHAR_LOAD_RESULT result = new body1_GC_CHAR_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(399), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		if( 0 > result.nLastSelectCharSlot)
		{
			AsUtil.ShutDown( "Latest char slot : " + result.nLastSelectCharSlot);
			return;
		}

		Debug.Log( "Miracle = " + result.nMiracle);
		AsUserInfo.Instance.nMiracle = result.nMiracle;
		AsUserInfo.Instance.latestCharSlot = result.nLastSelectCharSlot;
		//$yde
		AsUserInfo.Instance.SetPrivateShopInfo( result);
		AsPStoreManager.Instance.SetPrivateShopInfo( result);
		AsCharacterSlotManager.PossibleCharCreate = result.bPossibleCharCreate;

		for( int i = 0; i < result.nEnableSlotCnt; i++)
		{
			bool charGenerated = false;

			for( int j=0; j<result.nCharCnt; j++)
			{
				if( result.sCharView[j].nCharSlot == i)
				{
					AsUserInfo.Instance.AddCharacter( result.sCharView[j]);
					charGenerated = true;
					break;
				}
			}

			if( charGenerated == false)
			{
				AsUserInfo.Instance.AddEmptySlot();
			}
		}

		m_bLoginSelectResponse = true;
	}

	private void CharacterSelectResult( byte[] _packet)
	{
		Debug.Log( "CharacterSelectResult");

		AsPartyManager.Instance.Initilize();
		AsSocialManager.Instance.Initilize();

		body_GC_CHAR_SELECT_RESULT_1 result = new body_GC_CHAR_SELECT_RESULT_1();
		result.PacketBytesToClass( _packet);

		Debug.Log( "CharacterSelectResult: result.nChannel = " + result.nChannel);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_ANOTHERCHAR_OPENED:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(403), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		default:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(400), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		AsUserInfo.Instance.SetCostumeOnOff( result.bCostumeOnOff);
		AsUserInfo.Instance.SetItemViews( result.sNormalItemVeiw);
		AsUserInfo.Instance.SetCosItemView( result.sCosItemView);
		AsUserInfo.Instance.SaveCurCharStat( result);
		AsUserInfo.Instance.CurConditionValue = ( int)result.nCondition;
		//AsUserInfo.Instance.PvpPoint = result.nPvpPoint;
		AsUserInfo.Instance.YesterdayPvpRank = result.nYesterdayPvpRank;
		AsUserInfo.Instance.YesterdayPvpPoint = result.nYesterdayPvpPoint;
		AsUserInfo.Instance.YesterdayPvpRankRate = result.nYesterdayPvpRankRate;
		AsUserInfo.Instance.RankPoint = result.nRankPoint;
		AsUserInfo.Instance.FreeGachaPoint = result.nFreeGachaPoint;
		
		BonusManager.Instance.SetCompleteLevelBonus(result.nLevelComplete);

		#region -Designation
		AsDesignationManager.Instance.CurrentID = result.body.nSubTitleTableIdx;
		AsUserInfo.Instance.SubTitleHide = result.body.bSubTitleHide;
		AsGameMain.SetOptionState( OptionBtnType.OptionBtnType_SubTitleName, result.body.bSubTitleHide);
		AsDesignationManager.Instance.ResetDesignationRewardReceiveFlag();
		#endregion

#if false
		if( 0 == result.nChannel)
		{
			body_CG_CHANNEL_LIST channelList = new body_CG_CHANNEL_LIST( false);
			byte[] data = channelList.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
		else
		{
			body_CG_CHANNEL_SELECT channelSelect = new body_CG_CHANNEL_SELECT( result.nChannel);
			byte[] data = channelSelect.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
#endif

		if( null != result.body)
			AsHudDlgMgr.productRadioClassIndex = ( eCLASS)result.body.eClass;
	}

	private void CharSlotAddResult( byte[] _packet)
	{
		Debug.Log( "CharSlotAddResult");

		body_GC_CHAR_SLOT_ADD_RESULT result = new body_GC_CHAR_SLOT_ADD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1412), AsTableManager.Instance.GetTbl_String(1419), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		AsCharacterSlotManager.UnlockSlot();
	}

#region -ChannelSelect
	private void ChannelAutoSelect( byte[] _packet)
	{
		Debug.Log( "ChannelAutoSelect");

		body_GC_CHANNEL_AUTO_SELECT channelAutoSelect = new body_GC_CHANNEL_AUTO_SELECT();
		channelAutoSelect.PacketBytesToClass( _packet);

		if( 0 == channelAutoSelect.nChannel)
		{
			body_CG_CHANNEL_LIST channelList = new body_CG_CHANNEL_LIST( false);
			byte[] data = channelList.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
		else
		{
			Debug.Log( "Enter channel : " + channelAutoSelect.nChannel);

			AsUserInfo.Instance.currentChannel = channelAutoSelect.nChannel;
			AsUserInfo.Instance.currentChannelName = channelAutoSelect.szChannelName;

			AS_CG_ENTER_WORLD enterWorld = new AS_CG_ENTER_WORLD();
			byte[] data = enterWorld.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);

#if false
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null != userEntity)
			{
				StringBuilder sb = new StringBuilder( "ChannelCooltime_");
				sb.Append( userEntity.UniqueId);

				PlayerPrefs.SetString( sb.ToString(), System.DateTime.Now.Ticks.ToString());
				PlayerPrefs.Save();
			}
#endif
		}
	}

	private void ChannelListResult( byte[] _packet)
	{
		Debug.Log( "ChannelListResult");

		body1_GC_CHANNEL_LIST channelList = new body1_GC_CHANNEL_LIST();
		channelList.PacketBytesToClass( _packet);

		AsChannelListData.Instance.Clear();

		foreach( body2_GC_CHANNEL_LIST channel in channelList.channels)
			AsChannelListData.Instance.InsertData( channel);

		if( false == channelList.bIngame)
		{
			Application.LoadLevel( "ChannelSelect");
		}
		else
		{
			if( null == AsHudDlgMgr.Instance.ChannelSelectDlg)
			{
				GameObject dlg = Instantiate( Resources.Load( "UI/AsGUI/GUI_Channel")) as GameObject;
				AsHudDlgMgr.Instance.ChannelSelectDlg = dlg;
			}
			else
			{
				AsChannelSelectDlg dlg = AsHudDlgMgr.Instance.ChannelSelectDlg.GetComponentInChildren<AsChannelSelectDlg>();
				if( null != dlg)
					dlg.InitData();
			}
		}

		DDOL_Tracer.BeginTrace();//$ yde
	}
	
	
	private void ChannelInfo( byte[] _packet)
	{
		Debug.Log( "ChannelInfo");
		body_GC_CHANNEL_INFO channelInfo = new body_GC_CHANNEL_INFO();
		channelInfo.PacketBytesToClass( _packet);
		
		AsUserInfo.Instance.currentChannel = channelInfo.nChannel;
		if( null != channelInfo)
			AsUserInfo.Instance.currentChannelName = channelInfo.szChannelName;
		else
			AsUserInfo.Instance.currentChannelName = string.Empty;


	}
	private void ChannelSelectResult( byte[] _packet)
	{
		Debug.Log( "ChannelSelectResult");

		body_GC_CHANNEL_SELECT_RESULT selectResult = new body_GC_CHANNEL_SELECT_RESULT();
		selectResult.PacketBytesToClass( _packet);

		switch( selectResult.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_CHANNEL_INVALID:
			AsChannelListBtn.s_ChannelSelected = false;	// #12066
			return;
		case eRESULTCODE.eRESULT_FAIL_CHANNEL_FULL:
			if( AsUserInfo.Instance.PlayerPrivateShopOpened == false)
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(206), AsTableManager.Instance.GetTbl_String(831), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			else // private shop opened
				AsPStoreManager.Instance.OpenChannelDlgInCharacterSelect();
			AsChannelListBtn.s_ChannelSelected = false;	// #12066
			return;
		case eRESULTCODE.eRESULT_FAIL_CHANNEL_COMBAT:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(206), AsTableManager.Instance.GetTbl_String(208), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			AsChannelListBtn.s_ChannelSelected = false;	// #12066
			return;
		case eRESULTCODE.eRESULT_FAIL_CHANNEL_USING_CONTENT:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(206), AsTableManager.Instance.GetTbl_String(832), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			AsChannelListBtn.s_ChannelSelected = false;	// #12066
			return;
		}

		AsUserInfo.Instance.currentChannel = selectResult.nChannel;
//		body2_GC_CHANNEL_LIST channel = AsChannelListData.Instance.GetData( selectResult.nChannel - 1);
		body2_GC_CHANNEL_LIST channel = AsChannelListData.Instance.GetDataByChannelIndex( selectResult.nChannel);
		if( null != channel)
			AsUserInfo.Instance.currentChannelName = channel.szChannelName;
		else
			AsUserInfo.Instance.currentChannelName = string.Empty;

		//$yde
		AsPromotionManager.Instance.Reserve_ChannelChanged();

		AS_CG_ENTER_WORLD enterWorld = new AS_CG_ENTER_WORLD();
		byte[] data = enterWorld.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			StringBuilder sb = new StringBuilder( "ChannelCooltime_");
			sb.Append( userEntity.UniqueId);

			PlayerPrefs.SetString( sb.ToString(), System.DateTime.Now.Ticks.ToString());
			PlayerPrefs.Save();
		}
	}
#endregion

	private void EnterWorldResult( byte[] _packet)
	{
		Debug.Log( "EnterWorldResult");

		AS_GC_ENTER_WORLD_RESULT result = new AS_GC_ENTER_WORLD_RESULT();
		result.PacketBytesToClass( _packet);

		//$yde
		AsUserInfo.Instance.SetCurrentUserEnterWorldinfo( result);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(401), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		GameObject go = GameObject.Find( "SceneLoader");
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
		sceneLoader.Load( result.nMapIdx, AsSceneLoader.eLoadType.ENTER_WORLD);
		AsCommonSender.SetSocketState( SOCKET_STATE.SS_GAMESERVER);

		#region -Designation
		body_CS_SUBTITLE_LIST subtitleList = new body_CS_SUBTITLE_LIST();
		byte[] data = subtitleList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		#endregion

		#region -DelegateImage
		body_CS_IMAGE_LIST imageList = new body_CS_IMAGE_LIST();
		byte[] dataImageList = imageList.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( dataImageList);
		#endregion
	}

	private void ReturnCharSelectResult( byte[] _packet)
	{
		Debug.Log( "ReturnCharSelectResult");

		GC_RETURN_CHARSELECT_RESULT result = new GC_RETURN_CHARSELECT_RESULT();
		result.PacketBytesToClass( _packet);
		
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			ArkQuestmanager.instance.ResetQuestManager();
			break;
		case eRESULTCODE.eRESULT_FAIL_RETURN_CHARSELECT_WAIT:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1239),
				this, "OnOkBtn_del", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		default:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(402), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();
		ArkQuestmanager.instance.ResetQuestManager();

		if( null != AsChatManager.Instance)
			AsChatManager.Instance.ClearAllChat();
		AsUserInfo.Instance.Init();
		AsHUDController.SetActiveRecursively( false);
		PlayerBuffMgr.Instance.Clear();
		if( null != AsHudDlgMgr.Instance)
		{
			AsHudDlgMgr.Instance.CloseMsgBox();
			AsHudDlgMgr.Instance.invenPageIdx = 0;
		}
		TerrainMgr.Instance.Clear();
		AsCommonSender.ResetSendCheck();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
		AsEntityManager.Instance.RemoveAllEntities();
		StartCoroutine( ReturnCharSelectResult_Coroutine());
		
		AsUserInfo.Instance.NewMail = false;
		AsCharacterSlotManager.autoSelected = false;
	}

	private IEnumerator ReturnCharSelectResult_Coroutine()
	{
		AsLoadingIndigator.Instance.ShowIndigator( "");

		while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
			yield return null;

		AsLoadingIndigator.Instance.HideIndigator();

		Application.LoadLevel( "CharacterSelect");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}

#if USE_INSTANCE_SERVER
	// in dungeon
	private void InstanceCreateResult( byte[] _packet)
	{
		Debug.Log( "InstanceCreateResult");

		AS_GC_INSTANCE_CREATE_RESULT result = new AS_GC_INSTANCE_CREATE_RESULT();
		result.PacketBytesToClass( _packet);

		if( false == AsDungeonSender.CreateInDunSender())
			return;

		string ip = System.Text.ASCIIEncoding.ASCII.GetString( result.szIpaddress);

		AsDungeonSender.Instance.ConnectToServer( ip, result.nPort, SOCKET_STATE.SS_DUNGEON);
		if( false == AsNetworkManager.Instance.IsConnected())
			AsNotify.Instance.MessageBox( "Error", "Dungeon server connect\nfailed...!", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);

		Debug.Log( "success dungeon Server ");
		Debug.Log( "In dungeon Server IP : " + ip);
		Debug.Log( "In dungeon Server Prot : " + result.nPort);

		AsUserInfo.Instance.SetCurrentInstanceInfo( result);
		AsDungeonSender.Instance.SendEnterInstance( result.nRoomIdx, result.nEnterKey, AsUserInfo.Instance.GetCurrentUserCharacterInfo().nCharUniqKey);
	}
#endif

	// out Dungeon
	private void ReturnWorldresult( byte[] _packet)
	{
		Debug.Log( "ReturnWorldresult");

		AS_GC_ENTER_WORLD_RESULT result = new AS_GC_ENTER_WORLD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox( "Error", "Return world failed...!", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		GameObject go = GameObject.Find( "SceneLoader");
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
		sceneLoader.Load( result.nMapIdx, AsSceneLoader.eLoadType.ENTER_WORLD);
		AsCommonSender.SetSocketState( SOCKET_STATE.SS_GAMESERVER);
	}

	private void CheatResult( byte[] _packet)
	{
		body_GC_CHEAT_RESULT result = new body_GC_CHEAT_RESULT();
		result.PacketBytesToClass( _packet);

		string strBuf = "";
		if( eRESULTCODE.eRESULT_SUCC == result.eResult)
		{
			strBuf = "cheat succeeded";
			
			if( AsCheatManager.eCheatType.CT_Tel == (AsCheatManager.eCheatType)result.nType)
				AsCommonSender.SendWarp( result.nValue_1);

//			if( 26 == result.nType && 1 == result.nValue_1)
//			{
//				AsPStoreManager.Instance.Request_Create();//cheat
////				AsHudDlgMgr.Instance.OpenPStore();
//			}
			if( AsCheatManager.eCheatType.CT_InventoryExtendMax == (AsCheatManager.eCheatType)result.nType)
			{
				//ItemMgr.HadItemManagement.Inven.SetExtendPageCount( ( byte)result.nValue_1);
				ItemMgr.HadItemManagement.Inven.SetExtendPageCount( ( byte)20 );
				if( AsHudDlgMgr.Instance.IsOpenInven)
					AsHudDlgMgr.Instance.invenDlg.InvenOpen();
			}
		}
		else
			strBuf = "cheat failed";

		AsChatManager.Instance.InsertChat( strBuf, eCHATTYPE.eCHATTYPE_SYSTEM);
	}
	
	void HideProcess(byte[] _packet)
	{
		body_GC_CHEAT_HIDE_NOTIFY hide = new body_GC_CHEAT_HIDE_NOTIFY();
		hide.PacketBytesToClass(_packet);
		
		System.Collections.Generic.List<AsUserEntity> users = AsEntityManager.Instance.GetUserEntityBySessionId(hide.nSessionIdx);
		if(users.Count != 1)
		{
			Debug.LogError("AsGameProcess::HideProcess: such sessioned character is plural. count = " + users.Count);
		}
		
		users[0].HandleMessage(new Msg_HideIndicate(hide.bHide));
	}

	private void AccountDuplicate( byte[] _packet)
	{
		Debug.LogWarning( "AccountDuplicate");

		AsChatManager.Instance.ClearAllChat();
		AsHudDlgMgr.Instance.invenPageIdx = 0;
		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();
		TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
		ArkQuestmanager.instance.ResetQuestManager();
		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();

//		Application.LoadLevel( "Login");
//		DDOL_Tracer.BeginTrace();//$ yde
//		Resources.UnloadUnusedAssets();
		StartCoroutine( AccountDuplicate_Coroutine());
	}

	private IEnumerator AccountDuplicate_Coroutine()
	{
		AsLoadingIndigator.Instance.ShowIndigator( "");

		while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
			yield return null;

		AsLoadingIndigator.Instance.HideIndigator();

		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}

	#region - input -
	void OnOkBtn_del()
	{
		UIManager.instance.blockInput = false;
	}
	#endregion

	private void Recommend( byte[] _packet)
	{
		body_GC_RECOMMEND result = new body_GC_RECOMMEND();
		result.PacketBytesToClass( _packet);

		Debug.Log( "Recommend" + result.eResult.ToString());
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			{
				AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1434), AsTableManager.Instance.GetTbl_String(1436), this, "OnMsgBox_Recommend_Ok", "OnMsgBox_Recommend_Ok",
											AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				msgBox.SetFullscreenCollider();

				//clear;
				AsEventUIMgr.Instance.SetRecommend( false, 0);
				AsEventUIMgr.Instance.RecommendEventEnd = true;
				AsEventUIMgr.Instance.UserName = string.Empty;
			}
			break;

		case eRESULTCODE.eRESULT_FAIL_RECOMMEND_NOT_EXIST_CHARACTER://해당 유저 없어서 등록 실패
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1434), AsTableManager.Instance.GetTbl_String(1439),
											AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);

				GameObject go = GameObject.Find( "CharacterCreateFramework");
				if( null == go)
					return;

				AsCharacterCreateFramework characterCreateFramework = go.GetComponentInChildren<AsCharacterCreateFramework>();
				if( null != characterCreateFramework)
					characterCreateFramework.SendMessage( "OpenRecommendDlg", 1439);
			}
			break;
		default:
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), result.eResult.ToString(), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);

				GameObject go = GameObject.Find( "CharacterCreateFramework");
				if( null == go)
					return;

				AsCharacterCreateFramework characterCreateFramework = go.GetComponentInChildren<AsCharacterCreateFramework>();
				if( null != characterCreateFramework)
					characterCreateFramework.SendMessage( "OpenRecommendDlg", 1439);
			}
			break;
		}
	}

	public void OnMsgBox_Recommend_Ok()
	{
//		if( ( 1 == AsUserInfo.Instance.CreatedCharacterCount) && ( 0 != AsTableManager.Instance.GetTbl_GlobalWeight_Record(79).Value))
		if( 1 == AsUserInfo.Instance.CreatedCharacterCount)
		{
			AsLoadingIndigator.Instance.ShowIndigator( "");

			AS_CG_CHAR_SELECT select = new AS_CG_CHAR_SELECT( 0);
			byte[] data = select.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
			//$yde
			AsUserInfo.Instance.SetCurrentUserCharacterInfo( 0);

#if false
			GameObject main = GameObject.Find( "GameMain");
			AsIntroSound introSound = main.GetComponentInChildren<AsIntroSound>();
			introSound.StopSound();
#endif

			AsCharacterSlotManager.CharacterSelected = true;
			AsUserInfo.Instance.latestCharSlot = 0;

			m_bLoginSelectResponse = false;
		}
		else
		{
			m_bLoginSelectResponse = true;
		}
	}
}
