//#define _USE_BAND
#define _SOCIAL_LOG_
#define INTEGRATED_ARENA_MATCHING
#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Text;

public class AsCommonProcess_2 : AsProcessBase
{
	public override void Process( byte[] _packet)
	{
		PROTOCOL_CS_2 protocol = (PROTOCOL_CS_2)_packet[2];

		AsNetworkManager.Instance.ResetSendLiveTime();

		try
		{
			switch( protocol)
			{
			#region -Guild
			case PROTOCOL_CS_2.SC_GUILD_LOAD_REUSLT:
				GuildLoadResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_INFO_RESULT:
				GuildInfoResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_INFO_DETAIL_RESULT:
				GuildInfoDetailResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_SEARCH_RESULT:
				GuildSearchResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_CREATE_RESULT:
				GuildCreateResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_NOTICE_RESULT:
				GuildNoticeResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_INVITE:
				GuildInvite( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_INVITE_RESULT:
				GuildInviteResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_JOIN_RESULT:
				GuildJoinResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_SEARCH_JOIN_RESULT:
				GuildSearchJoinResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_EXIT_RESULT:
				GuildExitResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_EXILE:
				GuildExile( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_EXILE_RESULT:
				GuildExileResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_CHANGE_PERMISSION_RESULT:
				GuildChangePermissionResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_MEMBER_INFO_RESULT:
				GuildMemberInfoResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_SUPERVISE_RESULT:
				GuildSuperviseResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_NOT_APPROVE_MEMBER_INFO_RESULT:
				GuildNotApproveMemberInfoResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_PUBLICIZE_RESULT:
				GuildPublicizeResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_SEARCH_JOIN_APPROVE_RESULT:
				GuildSearchJoinApproveResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_LEVEL_UP_RESULT:
				GuildLevelUpResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_MEMBER_DELETE_RESULT:
				GuildMemberDeleteResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_MASTER_CHANGE_RESULT:
				GuildMasterChangeResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_UI_SCROLL_RESULT:
				GuildUIScrollResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_NAME_NOTIFY:
				GuildNameNotify( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_NOTICE_NOTIFY:
				GuildNoticeNotify( _packet);
				break;
			case PROTOCOL_CS_2.SC_GUILD_NAME_CHANGE:
				GuildNameChange( _packet );
				break;
			case PROTOCOL_CS_2.SC_GUILD_NAME_NOTIFY_CHANGE:
				GuildNameNotifyChange( _packet );
				break;
			#endregion
			}

			switch( protocol)
			{
			#region - priate shop -
			case PROTOCOL_CS_2.SC_PRIVATESHOP_CREATE:
				PrivateShop_Create( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_DESTROY:
				PrivateShop_Destroy( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_MODIFY:
				PrivateShop_Modify( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_REGISTRATION_ITEM:
				PrivateShop_Registraion_Item( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_OPEN:
				PrivateShop_Open( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_CLOSE:
				PrivateShop_Close( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_LIST:
				PrivateShop_List( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_ENTER:
				PrivateShop_Enter( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_LEAVE:
				PrivateShop_Leave( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_ITEMLIST:
				PrivateShop_ItemList( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_OWNER_ITEMLIST:
				PrivateShop_Owner_ItemList( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_ITEMBUY:
				PrivateShop_ItemBuy( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_OWNER_ITEMBUY:
				PrivateShop_Owner_ItemBuy( _packet);
				break;
			case PROTOCOL_CS_2.SC_PRIVATESHOP_SEARCH_RESULT:
				PrivateShop_SearchResult( _packet);
				break;
			#endregion
			#region - bonus -
			case PROTOCOL_CS_2.SC_BONUS_ATTENDANCE:
				AttendBonus( _packet);
				break;
			case PROTOCOL_CS_2.SC_BONUS_RETURN:
				ReturnBonus( _packet);
				break;
			#endregion
			}

			switch( protocol)
			{
			#region -Social
			case PROTOCOL_CS_2.SC_SOCIAL_INFO:
				SocialInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_SOCIAL_HISTORY:
				SocialHistory( _packet);
				break;
			case PROTOCOL_CS_2.SC_SOCIAL_HISTORY_REGISTER_RESULT:
				SocialHistoryRegisterResult(_packet);
				break;
			case PROTOCOL_CS_2.SC_SOCIAL_NOTICE:
				SocialNotice( _packet);
				break;
			case PROTOCOL_CS_2.SC_SOCIAL_UI_SCROLL:
				SocialUiScroll( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_CONNECT:
				FriendConnect( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_LIST:
				FriendList( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_INVITE:
				FriendInvite( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_INVITE_RESULT:
				FriendInviteResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_JOIN_RESULT:
				FriendJoinResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_INSERT:
				FriendInsert( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_CLEAR_RESULT:
				FriendClearResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_DELETE:
				FriendDelete( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_HELLO:
				FriendHello( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_HELLO_RESULT:
				FriendHelloResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_RECALL_RESULT:
				FriendRecallResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_RANDOM:
				FriendRandom( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_WARP:
				FriendWarp( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_CONDITION_RESULT:
				FriendConditionResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_FRIEND_CONNECT_REQUEST_RESULT:
				FriendConnectRequestResult( _packet);
				break;	
			case PROTOCOL_CS_2.SC_BLOCKOUT_LIST:
				FriendBlockOutList( _packet);
				break;
			case PROTOCOL_CS_2.SC_BLOCKOUT_INSERT:
				FriendBlockOutInsert( _packet);
				break;
			case PROTOCOL_CS_2.SC_BLOCKOUT_DELETE:
				FriendBlockOutDelete( _packet);
				break;
			case PROTOCOL_CS_2.SC_SOCIAL_ITEMBUY_RESULT:
				SocialItemBuyResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GAME_INVITE_LIST_RESULT:
				GameInviteListResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GAME_INVITE_RESULT:
				GameInviteResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_GAME_INVITE_REWARD_RESULT:
				GameInviteRewardResult( _packet);
				break;
			#endregion
			}

			switch( protocol)
			{
			#region -Designation
			case PROTOCOL_CS_2.SC_SUBTITLE_LIST:
				SubTitleList( _packet);
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_SET_RESULT:
				SubTitleSetResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_ADD:
				SubTitleAdd( _packet);
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_ADD_NOTIFY:
				SubTitleAddNotify( _packet);
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_CHANGE_NOTIFY:
				SubTitleChangeNotify( _packet);
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_INDEX_REWARD_RESULT:
				SubtitleIndexRewardResult( _packet );
				break;
			case PROTOCOL_CS_2.SC_SUBTITLE_ACCURE_REWARD_RESULT:
				SubtitleAccrueRewardResult( _packet );
				break;
			#endregion
			#region -DelegateImage
			case PROTOCOL_CS_2.SC_IMAGE_LIST:
				ImageList( _packet);
				break;
			case PROTOCOL_CS_2.SC_IMAGE_BUY_RESULT:
				ImageBuyResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_IMAGE_SET_RESULT:
				ImageSetResult( _packet);
				break;
#if NEW_DELEGATE_IMAGE
			case PROTOCOL_CS_2.SC_IMAGE_CHANGE_NOTIFY:
				Image_Change_Notify( _packet);
				break;
#endif
			#endregion
			#region -ITEM_PRODUCT
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_INFO:
				ReceiveItemProductInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_TECHNIQUE_REGISTER:
				ReceiveItemProductTechniqueRegister( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_PROGRESS:
				ReceiveItemProductProgress( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_REGISTER:
				ReceiveItemProductRegister( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_CANCEL:
				ReceiveItemProductCancel( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_RECEIVE:
				ReceiveItemProductReceive( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_CASH_DIRECT:
				ReceiveItemProductCashDirect( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_CASH_SLOT_OPEN:
				ReceiveItemProductCashSlotOpen( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN:
				ReceiveItemProductCashTechniqueOpen( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_CASH_LEVEL_UP:
				ReceiveItemProductCashLevelUp( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_SLOT_INFO:
				ReceiveItemProductSlotInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_TECHNIQUE_INFO:
				ReceiveItemProductTechniqueInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_PRODUCT_STATE:
				ReceiveItemProductState( _packet);
				break;
			#endregion
			}

			switch( protocol)
			{
			case PROTOCOL_CS_2.SC_COLLECT_INFO:
				ReceiveCollectInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_COLLECT_RESULT:
				ReceiveCollectResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_ENCHANT_RESULT:
				ReciveEnchantResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_ENCHANT_OUT_RESULT:
				ReciveOutEnchantResult(_packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_STRENGTHEN_RESULT:
				ReciveStrengthenResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_ITEM_MIX_RESULT:
				ReciveItemMixResult( _packet);
				break;

			case PROTOCOL_CS_2.SC_COS_ITEM_MIX_UP_RESULT:
				ReciveCostumeItemMixUpResult( _packet );
				break;
			case PROTOCOL_CS_2.SC_COS_ITEM_MIX_UPGRADE_RESULT:
				ReciveCostumeItemMixUpgradeResult( _packet );
				break;

			case PROTOCOL_CS_2.SC_ITEM_TIME_EXPIRE:
				ReceiveItemTimeExpire( _packet);
				break;
			case PROTOCOL_CS_2.SC_OTHER_INFO:
				ReciveOtherUserInfo( _packet);
				break;

			case PROTOCOL_CS_2.SC_COSTUME_ONOFF:
				ReciveConstumeOnOff( _packet);
				break;

			case PROTOCOL_CS_2.SC_RESURRECT_PENALTY_CLEAR:
				ReceiveResurrectPenaltyClear( _packet);
				break;
			case PROTOCOL_CS_2.SC_COUPON_REGIST:
				ReceiveCouponRegist( _packet);
				break;
			case PROTOCOL_CS_2.SC_EVENT_LIST:
				ReceiveEventList( _packet);
				break;
			case PROTOCOL_CS_2.SC_SERVER_EVENT_START:
				ServerEventStart( _packet);
				break;
			case PROTOCOL_CS_2.SC_SERVER_EVENT_STOP:
				ServerEventStop( _packet);
				break;
			case PROTOCOL_CS_2.SC_GAME_REVIEW:
				ReceiveGameReview( _packet);
				break;
			case PROTOCOL_CS_2.SC_USER_EVENT_NOTIFY:
				ReceiveUserEventNotify( _packet);
				break;
			case PROTOCOL_CS_2.SC_WANTED_START:
				WantedQuestStart( _packet);
				break;
			case PROTOCOL_CS_2.SC_WANTED_CLEAR:
				WantedQuestClear( _packet);
				break;
			case PROTOCOL_CS_2.SC_WANTED_COMPLETE:
				WantedQuestCompleteResult( _packet);
				break;
			#region -Condition_Impl
			case PROTOCOL_CS_2.SC_CONDITION:
				ReceiveCondition( _packet);
				break;
			#endregion
			}

			switch( protocol)
			{
			#region -Ranking_Imple
			case PROTOCOL_CS_2.SC_RANK_MYRANK_LOAD_RESULT:
				RankMyRankLoadResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_RANK_ITEM_MYRANK_LOAD_RESULT:
				RankItemMyRankLoadResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_RANK_ITEM_TOP_LOAD_RESULT:
				RankItemTopLoadResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_RANK_ITEM_MYFRIEND_LOAD_RESULT:
				RankItemMyFriendLoadResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_RANK_CHANGE_MYRANK:
				RankChangeMyRank( _packet);
				break;
			case PROTOCOL_CS_2.SC_RANK_CHANGE_RANKPOINT:
				RankChangeRankPoint( _packet);
				break;
			#endregion
				
			/* Not Used
			#region -Instance_Dungeon
			case PROTOCOL_CS_2.SC_INSTANCE_CREATE_RESULT:
				InDun_Create_Result( _packet);
				break;
			case PROTOCOL_CS_2.SC_INSTANCE_CREATE:
				InDun_Create( _packet);
				break;
			case PROTOCOL_CS_2.SC_ENTER_INSTANCE_RESULT:
				InDun_Enter_Result( _packet);
				break;
			case PROTOCOL_CS_2.SC_EXIT_INSTANCE_RESULT:
				InDun_Exit_Result( _packet);
				break;
			#endregion
			*/

			#region -Pvp
			case PROTOCOL_CS_2.SC_ARENA_MATCHING_INFO_RESULT:
				Pvp_Matching_Info_Result( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_MATCHING_REQUEST_RESULT:
				Pvp_Matching_Request_Result( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_MATCHING_GOINTO_RESULT:
				Pvp_Matching_GoInto_Result( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_MATCHING_NOTIFY:
				Pvp_Matching_Notity( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_MATCHING_LIMITCOUNT:
				Pvp_Matching_LimitCount( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_INITILIZE:
				Pvp_Arena_Initilize( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_GOGOGO:
				Pvp_Gogogo( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_USERINFO_LIST:
				Pvp_UserInfo_List( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_ENTERUSER:
				Pvp_EnterUser( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_REWARDINFO:
				Pvp_RewardInfo( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_READY:
				Pvp_Arena_Ready( _packet);
				break;
			case PROTOCOL_CS_2.SC_ARENA_START:
				Pvp_Arena_Start( _packet);
				break;
			#endregion

			#region -AccountGender
			case PROTOCOL_CS_2.SC_USERGENDER_SET_RESULT:
				UserGenderSetResult( _packet);
				break;
			case PROTOCOL_CS_2.SC_USERGENDER_NOTIFY:
				UserGenderNotify( _packet);
				break;
			#endregion

			default:
				break;
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( "Protocol : " + protocol + " --------> exception : " + e);
		}
	}

	#region -Guild_Impl
	void GuildLoadResult( byte[] _packet)
	{
		body_SC_GUILD_LOAD_RESULT result = new body_SC_GUILD_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		AsUserInfo.Instance.GuildData = result;
		Debug.Log( "GuildLoadResult");

		if( null != AsHudDlgMgr.Instance)
			AsHudDlgMgr.Instance.CloseGuildDlg();
	}

	void GuildInfoResult( byte[] _packet)
	{
		body_SC_GUILD_INFO_RESULT info = new body_SC_GUILD_INFO_RESULT();
		info.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			AsCommonSender.isSendGuild = false;
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.InitInfoTab( info);
		AsCommonSender.isSendGuild = false;
	}

	void GuildInfoDetailResult( byte[] _packet)
	{
		body_SC_GUILD_INFO_DETAIL_RESULT result = new body_SC_GUILD_INFO_DETAIL_RESULT();
		result.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDetailInfoDlg)
		{
			Debug.LogWarning( "Guild detail info dlg is not exist");
			return;
		}

		AsGuildDetailInfo detailInfoDlg = AsHudDlgMgr.Instance.GuildDetailInfoDlg.gameObject.GetComponentInChildren<AsGuildDetailInfo>();
		detailInfoDlg.Init( result);
	}

	void GuildSearchResult( byte[] _packet)
	{
		body1_SC_GUILD_SEARCH_RESULT searchResult = new body1_SC_GUILD_SEARCH_RESULT();
		searchResult.PacketBytesToClass( _packet);
	
		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			AsCommonSender.isSendGuild = false;
			return;
		}
		
		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.InitListTab( searchResult );
		AsCommonSender.isSendGuild = false;
	}

	void GuildMemberInfoResult( byte[] _packet)
	{
		body1_SC_GUILD_MEMBER_INFO_RESULT memberInfo = new body1_SC_GUILD_MEMBER_INFO_RESULT();
		memberInfo.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.InitMemberTab( memberInfo);
	}

	void GuildCreateResult( byte[] _packet)
	{
		body_SC_GUILD_CREATE_RESULT create = new body_SC_GUILD_CREATE_RESULT();
		create.PacketBytesToClass( _packet);

		switch( create.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_GUILD_NAME_EXIST:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(241), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_NEED_GOLD:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(242), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_NEED_LEVEL:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(246), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_SUCC:
			body_SC_GUILD_LOAD_RESULT guildData = new body_SC_GUILD_LOAD_RESULT();
			guildData.szGuildName = create.szGuildName;
			guildData.szGuildMaster = AsUserInfo.Instance.SavedCharStat.charName_;
			guildData.ePermission = eGUILDPERMISSION.eGUILDPERMISSION_ALL;
			AsUserInfo.Instance.GuildData = guildData;
			
			AsHudDlgMgr.Instance.CloseGuildDlg();

			string msg = string.Format( AsTableManager.Instance.GetTbl_String(247), create.szGuildName);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), msg, null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		}
	}

	void GuildNoticeResult( byte[] _packet)
	{
		Debug.Log( "GuildNoticeResult");
		body_SC_GUILD_NOTICE_RESULT result = new body_SC_GUILD_NOTICE_RESULT();
		result.PacketBytesToClass( _packet);
		
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.LogWarning( "GuildNoticeResult : " + result.eResult);
			return;
		}

		Debug.Log( "Notice : " + result.szNotice);
//		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void GuildInvite( byte[] _packet)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_GuildInviteRefuse))
			return;
		
		body_SC_GUILD_INVITE invite = new body_SC_GUILD_INVITE();
		invite.PacketBytesToClass( _packet);

		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildApprovalDlg")) as GameObject;
		AsGuildApprovalDlg approvalDlg = go.GetComponent<AsGuildApprovalDlg>();
		approvalDlg.Init( invite);
	}

	void GuildInviteResult( byte[] _packet)
	{
		body_SC_GUILD_INVITE_RESULT invite = new body_SC_GUILD_INVITE_RESULT();
		invite.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			switch( invite.eResult)
			{
			case eRESULTCODE.eRESULT_FAIL_GUILD_EXIST:	// dopamin
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE:
				AsChatManager.Instance.InsertChat( "GuildInviteResult : " + invite.eResult, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_FULL:
				AsChatManager.Instance.InsertChat( "GuildInviteResult : " + invite.eResult, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_TUTORIAL:
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(423), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_NOT_EXIST:
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(225), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_REFUSE:
				string msg = string.Format( AsTableManager.Instance.GetTbl_String(228), invite.szCharName);
				AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_ALREADY:
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(227), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_PVP:
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(4102), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_SUCC:
				return;
			}
		}
		else
		{
			AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.GetComponentInChildren<AsGuildDlg>();

			switch( invite.eResult)
			{
			case eRESULTCODE.eRESULT_FAIL_GUILD_EXIST:	// dopamin
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE:
				guildDlg.DisplayInviteErrorMsg( "GuildInviteResult : " + invite.eResult);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_FULL:
				guildDlg.DisplayInviteErrorMsg( "GuildInviteResult : " + invite.eResult);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_TUTORIAL:
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(423), eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_NOT_EXIST:
				guildDlg.DisplayInviteErrorMsg( AsTableManager.Instance.GetTbl_String(225));
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_REFUSE:
				string msg = string.Format( AsTableManager.Instance.GetTbl_String(228), invite.szCharName);
				guildDlg.DisplayInviteErrorMsg( msg);
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_ALREADY:
				guildDlg.DisplayInviteErrorMsg( AsTableManager.Instance.GetTbl_String(227));
				return;
			case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_PVP:
				guildDlg.DisplayInviteErrorMsg( AsTableManager.Instance.GetTbl_String(4102));
				return;
			case eRESULTCODE.eRESULT_SUCC:
				guildDlg.CloseInviteDlg();
				return;
			}
		}

		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(232), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void GuildJoinResult( byte[] _packet)
	{
		body_SC_GUILD_JOIN_RESULT join = new body_SC_GUILD_JOIN_RESULT();
		join.PacketBytesToClass( _packet);

		switch( join.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_GUILD_EXIST:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(227), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN:
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_FULL:
			Debug.LogWarning( "GuildJoinResult : " + join.eResult);
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_ALREADY:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4100), AsNotify.MSG_BOX_TYPE.MBT_OK);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_LEVEL:
			Debug.LogWarning( "GuildJoinResult : " + join.eResult);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_INVITE_PVP:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(4102), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(232), join.szCharName);
		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void GuildSearchJoinResult( byte[] _packet)
	{
		Debug.Log( "GuildSearchJoinResult");

		body_SC_GUILD_SEARCH_JOIN_RESULT result = new body_SC_GUILD_SEARCH_JOIN_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_NOT_EXIST:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4085), AsNotify.MSG_BOX_TYPE.MBT_OK);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_ALREADY:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4100), AsNotify.MSG_BOX_TYPE.MBT_OK);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.LogWarning( "result.eResult : " + result.eResult);
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(248), result.szGuildName);
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), sb.ToString());
	}

	void GuildExitResult( byte[] _packet)
	{
		Debug.Log( "GuildExitResult");

		body_SC_GUILD_EXIT_RESULT exit = new body_SC_GUILD_EXIT_RESULT();
		exit.PacketBytesToClass( _packet);

		switch( exit.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_GUILD_EXIT:
			Debug.LogWarning( "GuildExitResult : " + exit.eResult);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_EXIT_MASTER:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(229), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		string msg = string.Format( AsTableManager.Instance.GetTbl_String(157), AsUserInfo.Instance.GuildData.szGuildName);
		AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
		AsUserInfo.Instance.GuildData = null;
		AsHudDlgMgr.Instance.CloseGuildDlg();
	}

	void GuildExile( byte[] _packet)
	{
		body_SC_GUILD_EXILE exile = new body_SC_GUILD_EXILE();
		exile.PacketBytesToClass( _packet);

		AsUserInfo.Instance.GuildData = null;
		Debug.Log( "GuildExile");
		AsHudDlgMgr.Instance.CloseGuildDlg();
	}

	void GuildExileResult( byte[] _packet)
	{
		body_SC_GUILD_EXILE_RESULT exile = new body_SC_GUILD_EXILE_RESULT();
		exile.PacketBytesToClass( _packet);
		
		switch( exile.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.Log( "exile.eResult : " + exile.eResult);
			return;
		}
	}

	void GuildChangePermissionResult( byte[] _packet)
	{
		body_SC_GUILD_CHANGE_PERMISSION_RESULT result = new body_SC_GUILD_CHANGE_PERMISSION_RESULT();
		result.PacketBytesToClass( _packet);

		Debug.Log( "GuildChangePermissionResult : " + result.eResult);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
			return;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		string szName = AsUtil.GetRealString( userEntity.GetProperty<string>( eComponentProperty.NAME));
		if( szName == result.szTargetCharName)
		{
			Debug.Log( "permission : " + result.ePermission);
			AsUserInfo.Instance.GuildData.ePermission = result.ePermission;
		}
	}

	void GuildSuperviseResult( byte[] _packet)
	{
		body_SC_GUILD_SUPERVISE_RESULT result = new body_SC_GUILD_SUPERVISE_RESULT();
		result.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.InitManageTab( result);
	}

	void GuildNotApproveMemberInfoResult( byte[] _packet)
	{
		body1_SC_GUILD_MEMBER_INFO_RESULT memberInfo = new body1_SC_GUILD_MEMBER_INFO_RESULT();
		memberInfo.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.InsertApplicantList( memberInfo);
	}

	void GuildPublicizeResult( byte[] _packet)
	{
		body_SC_GUILD_PUBLICIZE_RESULT result = new body_SC_GUILD_PUBLICIZE_RESULT();
		result.PacketBytesToClass( _packet);
		
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.Log( "GuildPublicizeResult : " + result.eResult);
			return;
		}

//		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void GuildSearchJoinApproveResult( byte[] _packet)
	{
		body_SC_GUILD_SEARCH_JOIN_APPROVE_RESULT result = new body_SC_GUILD_SEARCH_JOIN_APPROVE_RESULT();
		result.PacketBytesToClass( _packet);

		Debug.Log( "GuildSearchJoinApproveResult : " + result.eResult);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_GUILD_JOIN_ALREADY:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(227), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( null == AsHudDlgMgr.Instance.GuildDlg)
		{
			Debug.LogWarning( "Guild dlg is not exist");
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
		guildDlg.RequestCurPageApplicant( true);
	}

	void GuildLevelUpResult( byte[] _packet)
	{
		body_SC_GUILD_LEVEL_UP_RESULT result = new body_SC_GUILD_LEVEL_UP_RESULT();
		result.PacketBytesToClass( _packet);

		StringBuilder sb = new StringBuilder();

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_MAX_LEVEL:
			sb.AppendFormat( "{0} eRESULT_FAIL_GUILD_MAX_LEVEL", AsTableManager.Instance.GetTbl_String(1177));
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_NEED_GOLD:
			sb.AppendFormat( "{0}", AsTableManager.Instance.GetTbl_String(834));
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.LogWarning( "GuildLevelUpResult : " + result.eResult);
			return;
		}

		AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.GetComponentInChildren<AsGuildDlg>();
		if( null != guildDlg)
			guildDlg.UpdateGuildLevel( result);

		sb.AppendFormat( "{0} {1}", AsTableManager.Instance.GetTbl_String(1177), AsTableManager.Instance.GetTbl_String(1279));
		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void GuildMemberDeleteResult( byte[] _packet)
	{
		Debug.Log( "GuildMemberDeleteResult");

		body_SC_GUILD_MEMBER_DELETE_RESULT result = new body_SC_GUILD_MEMBER_DELETE_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eDeleteFlag)
		{
		case eGUILDMEMBER_DELETE.eGUILDMEMBER_DELETE_EXIT:
			{
				string msg = string.Format( AsTableManager.Instance.GetTbl_String(233), result.szCharName);
				AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
			}
			break;
		case eGUILDMEMBER_DELETE.eGUILDMEMBER_DELETE_EXILE:
			{
				string msg = string.Format( AsTableManager.Instance.GetTbl_String(234), result.szCharName, result.szExileName);
				AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
			}
			break;
		}
	}

	void GuildMasterChangeResult( byte[] _packet)
	{
		Debug.Log( "GuildMasterChangeResult");

		body_SC_GUILD_MASTER_CHANGE_RESULT result = new body_SC_GUILD_MASTER_CHANGE_RESULT();
		result.PacketBytesToClass( _packet);
		
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_GUILD_BUSY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1660), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		default:
			Debug.LogWarning( "GuildMasterChangeResult : " + result.eResult);
			return;
		}

		string msg = string.Format( AsTableManager.Instance.GetTbl_String(260), result.szOldMasterName, result.szNewMasterName);
		AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);

		AsHudDlgMgr.Instance.CloseGuildDlg();
	}

	void GuildUIScrollResult( byte[] _packet)
	{
		Debug.Log( "GuildUIScrollResult");

		body_SC_GUILD_UI_SCROLL_RESULT result = new body_SC_GUILD_UI_SCROLL_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eType)
		{
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD:
			{
				if( null == AsHudDlgMgr.Instance.GuildDlg)
				{
					Debug.LogWarning( "Guild dlg is not exist");
					break;
				}
				
				AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.gameObject.GetComponentInChildren<AsGuildDlg>();
				guildDlg.UpdateGuildList( result.searchResult );
			}
			break;
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_APPROVE_MEMBER:
			{
				if( null == AsHudDlgMgr.Instance.GuildDlg)
				{
					Debug.Log( "Guild dlg is not exist");
					break;
				}

				AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.GetComponentInChildren<AsGuildDlg>();
				guildDlg.InsertMemberList( result.memberInfoResult);
			}
			break;
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_NOT_APPROVE_MEMBER:
			{
				if( null == AsHudDlgMgr.Instance.GuildDlg)
				{
					Debug.Log( "Guild dlg is not exist");
					break;
				}

				AsGuildDlg guildDlg = AsHudDlgMgr.Instance.GuildDlg.GetComponentInChildren<AsGuildDlg>();
				guildDlg.InsertApplicantList( result.memberInfoResult);
			}
			break;
		}
	}

	void GuildNameNotify( byte[] _packet)
	{
		Debug.Log( "GuildNameNotify");

		body_SC_GUILD_NAME_NOTIFY result = new body_SC_GUILD_NAME_NOTIFY();
		result.PacketBytesToClass( _packet);

		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( result.nCharUniqKey);
		if( null != userEntity)
		{
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey)
				AsUserInfo.Instance.SavedCharStat.guildName_ = result.szGuildName;
			
			userEntity.SetProperty( eComponentProperty.GUILD_NAME, AsUtil.GetRealString( result.szGuildName));

			if( null != userEntity.namePanel)
			{
				userEntity.namePanel.SetGuildName( result.szGuildName);
				userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
			}

//			#region -AccountGender
//			userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
//			#endregion
		}
	}

	void GuildNoticeNotify( byte[] _packet)
	{
		body_SC_GUILD_NOTICE_NOTIFY notify = new body_SC_GUILD_NOTICE_NOTIFY();
		notify.PacketBytesToClass( _packet);

		if( false == string.IsNullOrEmpty( notify.szGuildNotice))
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(2044), eCHATTYPE.eCHATTYPE_SYSTEM, true);
			AsChatManager.Instance.InsertChat( notify.szGuildNotice, eCHATTYPE.eCHATTYPE_SYSTEM, true);
//			AsEventNotifyMgr.Instance.CenterNotify.AddGuildNotice( notify.szGuildNotice);
		}
	}
	
	void GuildNameChange( byte[] _packet)
	{
		body_SC_GUILD_NAME_CHANGE result = new body_SC_GUILD_NAME_CHANGE();
		result.PacketBytesToClass( _packet);
		
		if( result.eResult == eRESULTCODE.eRESULT_SUCC )
		{
			if( AsUserInfo.Instance.GuildData.szGuildMaster == AsUserInfo.Instance.SavedCharStat.charName_ )
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1882), result.szGuildName);
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage( sb.ToString() );
				
				if( AsHudDlgMgr.Instance.IsOpenReNameDlg == true )
				{
					AsReNameDlg renameDlg = AsHudDlgMgr.Instance.ReNameDlg;
					if( renameDlg.ReNameType == AsReNameDlg.eReNameType.Guild )
					{
						renameDlg.Close();
					}
				}
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1883), AsUserInfo.Instance.GuildData.szGuildMaster , result.szGuildName);
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage( sb.ToString() );
			}
		}
		else if( result.eResult == eRESULTCODE.eRESULT_FAIL_GUILD_NAME_CHANGE_SYSTEM_ERROR )
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1881));
		}
		else if( result.eResult == eRESULTCODE.eRESULT_FAIL_GUILD_NAME_CHANGE_USE_ALREADY )
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1791));
		}
	}
	
	void GuildNameNotifyChange( byte[]_packet )
	{	
		body_SC_GUILD_NAME_NOTIFY_CHANGE result = new body_SC_GUILD_NAME_NOTIFY_CHANGE();
		result.PacketBytesToClass( _packet);
		
		List<AsUserEntity> entities = AsEntityManager.Instance.GetUserEntitybyGuildName( result.szBeforeGuildName );
		foreach( AsUserEntity userEntity in entities )
		{
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == userEntity.UniqueId )
				AsUserInfo.Instance.SavedCharStat.guildName_ = result.szAfterGuildName;
			
			userEntity.SetProperty( eComponentProperty.GUILD_NAME, AsUtil.GetRealString( result.szAfterGuildName));
			
			if( null != userEntity.namePanel)
			{
				userEntity.namePanel.SetGuildName( result.szAfterGuildName);
				userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
			}
		}
	}
	#endregion

	#region - private shop -
	void PrivateShop_Create( byte[] _packet)
	{
		body_SC_PRIVATESHOP_CREATE create = new body_SC_PRIVATESHOP_CREATE();
		create.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Create( create);
	}

	void PrivateShop_Destroy( byte[] _packet)
	{
		body_SC_PRIVATESHOP_DESTROY destroy = new body_SC_PRIVATESHOP_DESTROY();
		destroy.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Destroy( destroy);
	}

	void PrivateShop_Modify( byte[] _packet)
	{
		body_SC_PRIVATESHOP_MODIFY modify = new body_SC_PRIVATESHOP_MODIFY();
		modify.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Modify( modify);
	}

	void PrivateShop_Registraion_Item( byte[] _packet)
	{
		body_SC_PRIVATESHOP_REGISTRATION_ITEM registration = new body_SC_PRIVATESHOP_REGISTRATION_ITEM();
		registration.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Registraion_Item( registration);
	}

	void PrivateShop_Open( byte[] _packet)
	{
		body_SC_PRIVATESHOP_OPEN open = new body_SC_PRIVATESHOP_OPEN();
		open.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Open( open);
	}

	void PrivateShop_Close( byte[] _packet)
	{
		body_SC_PRIVATESHOP_CLOSE close = new body_SC_PRIVATESHOP_CLOSE();
		close.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Close( close);
	}

	void PrivateShop_List( byte[] _packet)
	{
		body1_SC_PRIVATESHOP_LIST list = new body1_SC_PRIVATESHOP_LIST();
		list.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_List( list);
	}

	void PrivateShop_Enter( byte[] _packet)
	{

		body_SC_PRIVATESHOP_ENTER enter = new body_SC_PRIVATESHOP_ENTER();
		enter.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Enter( enter);

		AsPStoreManager.s_IsSendCS_PRIVATESHOP_ENTER = true;
	}

	void PrivateShop_Leave( byte[] _packet)
	{
		body_SC_PRIVATESHOP_LEAVE leave = new body_SC_PRIVATESHOP_LEAVE();
		leave.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Leave( leave);
	}

	void PrivateShop_ItemList( byte[] _packet)
	{
		body1_SC_PRIVATESHOP_ITEMLIST list = new body1_SC_PRIVATESHOP_ITEMLIST();
		list.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_ItemList( list);
	}

	void PrivateShop_Owner_ItemList( byte[] _packet)
	{
		body1_SC_PRIVATESHOP_OWNER_ITEMLIST list = new body1_SC_PRIVATESHOP_OWNER_ITEMLIST();
		list.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Owner_ItemList( list);
	}

	void PrivateShop_ItemBuy( byte[] _packet)
	{
		body_SC_PRIVATESHOP_ITEMBUY buy = new body_SC_PRIVATESHOP_ITEMBUY();
		buy.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_ItemBuy( buy);
	}

	void PrivateShop_Owner_ItemBuy( byte[] _packet)
	{
		body_SC_PRIVATESHOP_OWNER_ITEMBUY buy = new body_SC_PRIVATESHOP_OWNER_ITEMBUY();
		buy.PacketBytesToClass( _packet);

		AsPStoreManager.Instance.Recv_Owner_ItemBuy( buy);
	}
	
	void PrivateShop_SearchResult( byte[] _packet)
	{
		body1_SC_PRIVATESHOP_SEARCH_RESULT search = new body1_SC_PRIVATESHOP_SEARCH_RESULT();
		search.PacketBytesToClass( _packet);
		
		AsPStoreManager.Instance.Recv_Search( search);
	}
	#endregion

	#region -Designation_Impl
	void SubTitleList( byte[] _packet)
	{
		body1_SC_SUBTITLE_LIST subtitleList = new body1_SC_SUBTITLE_LIST();
		subtitleList.PacketBytesToClass( _packet);

		AsDesignationManager.Instance.Clear();
		for( int i = 0; i < subtitleList.nCount; i++)
		{
			AsDesignationManager.Instance.ObtainDesignation( subtitleList.designations[i].nSubTitleTableIdx);
			AsDesignationManager.Instance.SetDesignationRewardReceive( subtitleList.designations[i].nSubTitleTableIdx , subtitleList.designations[i].bReward );
		}

		AsDesignationRankRewardManager.Instance.LastReceiveRewardRankPoint = subtitleList.nAccrueRewardPoint;
	}

	void SubTitleSetResult( byte[] _packet)
	{
		body_SC_SUBTITLE_SET_RESULT result = new body_SC_SUBTITLE_SET_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogWarning( "SubTitleSetResult : " + result.eResult);
			return;
		}

		AsUserInfo.Instance.SavedCharStat.designationID = result.nSubTitleTableIdx;	// #10738
		AsUserInfo.Instance.SavedCharStat.bSubtitleHide = result.bSubTitleHide;
		AsDesignationManager.Instance.CurrentID = result.nSubTitleTableIdx;
		AsHudDlgMgr.Instance.SetDesignationText();

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		userEntity.DesignationID = result.nSubTitleTableIdx;
		userEntity.SubTitleHide = result.bSubTitleHide;
		DesignationData data = AsDesignationManager.Instance.GetCurrentDesignation();
		if( null != data)
			userEntity.namePanel.SetSubTitleName( AsTableManager.Instance.GetTbl_String(data.name));
		else
			userEntity.namePanel.SetSubTitleName( "");

		#region -AccountGender
		userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
		#endregion

		if( eRESULTCODE.eRESULT_SUCC == result.eResult)
		{
			if( data != null)
				QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_DESIGNATION, new AchDesignation( DesignationAchType.DESIGNATION_CHANGE, result.nSubTitleTableIdx));
		}
	}

	void SubTitleAdd( byte[] _packet)
	{
		#region -GameGuide_Title
		AsGameGuideManager.Instance.CheckUp( eGameGuideType.Title, -1);
		#endregion

		body_SC_SUBTITLE_ADD subtitleAdd = new body_SC_SUBTITLE_ADD();
		subtitleAdd.PacketBytesToClass( _packet);

		AsDesignationManager.Instance.ObtainDesignation( subtitleAdd.nSubTitleTableIdx);
		AsDesignationManager.Instance.InsertAlarm( subtitleAdd.nSubTitleTableIdx);
		AsHUDController.Instance.PromptDesignationAlarm( subtitleAdd.nSubTitleTableIdx);

		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_DESIGNATION, new AchDesignation( DesignationAchType.DESIGNATION_GET, subtitleAdd.nSubTitleTableIdx));
		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_DESIGNATION, new AchDesignation( DesignationAchType.DESIGNATION_COUNT,AsDesignationManager.Instance.GetObtainedDesignationCount()));

		ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();
	}

	void SubTitleAddNotify( byte[] _packet)
	{
		body_SC_SUBTITLE_ADD_NOTIFY notify = new body_SC_SUBTITLE_ADD_NOTIFY();
		notify.PacketBytesToClass( _packet);

		DesignationData data = AsDesignationManager.Instance.GetDesignation( notify.nSubTitleTableIdx);
		string designation = AsTableManager.Instance.GetTbl_String(data.name);
		string noticeMsg = AsTableManager.Instance.GetTbl_String(data.notice);
		string msg = string.Format( noticeMsg, notify.szCharName, designation);
		AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void SubTitleChangeNotify( byte[] _packet)
	{
		body_SC_SUBTITLE_CHANGE_NOTIFY notify = new body_SC_SUBTITLE_CHANGE_NOTIFY();
		notify.PacketBytesToClass( _packet);

		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( notify.nCharUniqKey);
		if( null == userEntity)
		{
			Debug.LogWarning( "userEntity : " + notify.nCharUniqKey);
			return;
		}

		DesignationData designationData = AsDesignationManager.Instance.GetDesignation( notify.nSubTitleTableIdx);

		string strSubTileName = "";
		if( null != designationData)
		{
			strSubTileName = AsTableManager.Instance.GetTbl_String(designationData.name);
			userEntity.DesignationID = notify.nSubTitleTableIdx;
		}

		if( null != userEntity.namePanel)
		{
			userEntity.namePanel.SetSubTitleName( strSubTileName);
			userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
		}

//		#region -AccountGender
//		userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
//		#endregion

		ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();
	}

	void SubtitleIndexRewardResult( byte[] _packet)
	{
		body_SC_SUBTITLE_INDEX_REWARD_RESULT result = new body_SC_SUBTITLE_INDEX_REWARD_RESULT();
		result.PacketBytesToClass( _packet);

		AsDesignationRankRewardManager.Instance.Recv_SubtitleIndexRewardResult (result);
	}

	void SubtitleAccrueRewardResult( byte[] _packet)
	{
		body_SC_SUBTITLE_ACCRUE_REWARD_RESULT result = new body_SC_SUBTITLE_ACCRUE_REWARD_RESULT();
		result.PacketBytesToClass( _packet);
		
		AsDesignationRankRewardManager.Instance.Recv_SubtitleAccrueRewardResult (result);
	}
	#endregion

	#region -DelegateImage
	void ImageList( byte[] _packet)
	{
		body1_SC_IMAGE_LIST imageList = new body1_SC_IMAGE_LIST();
		imageList.PacketBytesToClass( _packet);

		AsDelegateImageManager.Instance.Init();

		AsDelegateImageManager.Instance.AssignedImageID = imageList.nCurImageTableIdx;

		foreach( body2_SC_IMAGE_LIST body in imageList.imageList)
		{
			AsDelegateImageManager.Instance.SetUnlockState( body.nImageTableIdx, false);
		}
	}

	void ImageBuyResult( byte[] _packet)
	{
		body_SC_IMAGE_BUY_RESULT buyResult = new body_SC_IMAGE_BUY_RESULT();
		buyResult.PacketBytesToClass( _packet);

		switch( buyResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_IMAGE_BUY_ALREADY:
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(404), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_WARNING));
			return;
		case eRESULTCODE.eRESULT_FAIL_IMAGE_BUY_NEED_CASH:
			{
				if (AsGameMain.useCashShop == true)
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), this, "OpenCashShop", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING));
				else
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), AsNotify.MSG_BOX_TYPE.MBT_OK));
			}
			return;
		case eRESULTCODE.eRESULT_FAIL_IMAGE_BUY_NEED_GOLD:
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1400), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_WARNING));
			return;
		default:
			Debug.LogError( "ImageBuyResult : " + buyResult.eResult);
			return;
		}

		AsDelegateImageManager.Instance.SetUnlockState( buyResult.nImageTableIdx, false);

		GameObject go = AsHudDlgMgr.Instance.DelegateImageDlg;
		if( null != go)
		{
			AsDelegateImageDlg dlg = go.GetComponentInChildren<AsDelegateImageDlg>();
			Debug.Assert( null != dlg);
			dlg.UnlockDelegateImage( buyResult.nImageTableIdx);
		}
	}

	void OpenCashShop()
	{
		AsHudDlgMgr.Instance.CloseDelegateImageDlg();
		AsHudDlgMgr.Instance.CloseRecommendInfoDlg();	// #17479
		AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 4);
	}

	void ImageSetResult( byte[] _packet)
	{
		body_SC_IMAGE_SET_RESULT setResult = new body_SC_IMAGE_SET_RESULT();
		setResult.PacketBytesToClass( _packet);

		switch( setResult.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_IMAGE_SET_NOTHING:
			setResult.nImageTableIdx = 0;
			break;
		case eRESULTCODE.eRESULT_SUCC:
			break;
		default:
			Debug.LogError( "ImageSetResult : " + setResult.eResult);
			return;
		}

		AsDelegateImageManager.Instance.AssignedImageID = setResult.nImageTableIdx;
		if( 0 >= setResult.nImageTableIdx)
			AsMyProperty.Instance.ClearDelegateImage();
		else
			AsMyProperty.Instance.SetDelegateImage();
		//Party MyPortrait
		AsPartyManager.Instance.SetMyPortraitImage();
		GameObject go = AsHudDlgMgr.Instance.DelegateImageDlg;
		if( null != go)
		{
			AsDelegateImageDlg dlg = go.GetComponentInChildren<AsDelegateImageDlg>();
			Debug.Assert( null != dlg);
			dlg.Init();
		}

#if NEW_DELEGATE_IMAGE
		AsUserEntity userEntity = AsEntityManager.Instance.UserEntity;
		if( null != userEntity)
		{
			userEntity.nUserDelegateImageIndex = setResult.nImageTableIdx;
			userEntity.namePanel.UpdateDelegateImage( setResult.nImageTableIdx);
		}
#endif
	}

#if NEW_DELEGATE_IMAGE
	void Image_Change_Notify( byte[] _packet)
	{
		body_SC_IMAGE_CHANGE_NOTIFY setResult = new body_SC_IMAGE_CHANGE_NOTIFY();
		setResult.PacketBytesToClass( _packet);

		AsUserEntity _entity = AsEntityManager.Instance.GetUserEntityByUniqueId( setResult.nCharUniqKey);

		if( null != _entity && null != _entity.namePanel)
		{
			_entity.nUserDelegateImageIndex = setResult.nImageTableIdx;
			_entity.namePanel.UpdateDelegateImage( setResult.nImageTableIdx);
		}
	}
#endif
	#endregion

	#region - bonus -
	void AttendBonus( byte[] _packet)
	{
		body_SC_BONUS_ATTENDANCE attend = new body_SC_BONUS_ATTENDANCE();
		attend.PacketBytesToClass( _packet);

		BonusManager.Instance.Recv_AttendBonus(attend);
	}

	void ReturnBonus( byte[] _packet)
	{
		body_SC_BONUS_RETURN __return = new body_SC_BONUS_RETURN();
		__return.PacketBytesToClass( _packet);

		BonusManager.Instance.Recv_ReturnBonus(__return);
	}
	#endregion

	#region -Social
	void SocialInfo( byte[] _packet)
	{
		body_SC_SOCIAL_INFO socialInfo = new body_SC_SOCIAL_INFO();
		socialInfo.PacketBytesToClass( _packet);

		string notice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( socialInfo.szNotice));

		#if _SOCIAL_LOG_
		Debug.Log( "SocialInfo SocialPoint:" + socialInfo.nSocialPoint.ToString() + "nUserUniqKey: " + socialInfo.nUserUniqKey.ToString() + "notice: " + notice);
		#endif

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		if( socialInfo.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)
		{
			AsSocialManager.Instance.SocialData.SocialInfo = socialInfo;
			AsSocialManager.Instance.SocialUI.SetSocialInfo( socialInfo);
		}
		else
		{//Clone
			AsSocialManager.Instance.SocialUI.SetSocialCloneInfo( socialInfo);
		}
	}

	void SocialHistory( byte[] _packet)
	{
		body1_SC_SOCIAL_HISTORY socialHistory = new body1_SC_SOCIAL_HISTORY();
		socialHistory.PacketBytesToClass( _packet);
		#if _SOCIAL_LOG_
		Debug.Log( "SocialHistory Count : " + socialHistory.nCnt + "nUserUniqKey: " + socialHistory.nUserUniqKey);
		#endif

		if( socialHistory.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)
		{
			if( null == AsSocialManager.Instance.SocialUI.m_SocialDlg)
			{
				Debug.Log( "SocialDlg Friendlist  is not exist");
				return;
			}

			AsSocialManager.Instance.SocialUI.SetHistoryList( socialHistory);
		}
		else
		{//Clone
			if( null == AsSocialManager.Instance.SocialUI.m_ObjectSocialCloneDlg)
			{
				Debug.Log( "SocialDlg Friendlist  is not exist");
				return;
			}

			AsSocialManager.Instance.SocialUI.SetSocialCloneHistoryList( socialHistory);
		}
	}

	void SocialHistoryRegisterResult(byte[] _packet)
	{
		#if _SOCIAL_LOG_
		Debug.Log( "SocialHistoryRegisterResult");
		#endif
		body_SC_SOCIAL_HISTORY_REGISTER_RESULT socialHistoryRegisterResult = new body_SC_SOCIAL_HISTORY_REGISTER_RESULT();
		socialHistoryRegisterResult.PacketBytesToClass(_packet);

		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(1203);

		switch(socialHistoryRegisterResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			//AsCommonSender.SendSocialInfo(AsUserInfo.Instance.LoginUserUniqueKey);
			PostingSnsType postingType = PostingSnsType.TWITTER_OR_FACEBOOK;
			if (socialHistoryRegisterResult.ePlatform == (int)eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_FACEBOOK)
				postingType = PostingSnsType.FACEBOOK;
			else if (socialHistoryRegisterResult.ePlatform == (int)eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_TWITTER)
				postingType = PostingSnsType.TWITTER;

			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_POSTING_SNS, new AchPostingSNS(postingType, 1));

			AsSocialManager.Instance.SocialUI.RequestCurPageHistory();
			break;
		case eRESULTCODE.eRESULT_FAIL_HISTORY_REGISTRY:
			 message = AsTableManager.Instance.GetTbl_String(932);
			break;
		case eRESULTCODE.eRESULT_FAIL_HISTORY_REGISTRY_FULL:
			 message = AsTableManager.Instance.GetTbl_String(933);
			break;
		case eRESULTCODE.eRESULT_FAIL_HISTORY_REGISTRY_ALREADY:
			message = AsTableManager.Instance.GetTbl_String(934);
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}


	void SocialNotice( byte[] _packet)
	{
		body_SC_SOCIAL_NOTICE socialNotice = new body_SC_SOCIAL_NOTICE();
		socialNotice.PacketBytesToClass( _packet);

		AsSocialManager.Instance.SocialData.SocialInfo.szNotice = socialNotice.szNotice;
		string notice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( AsSocialManager.Instance.SocialData.SocialInfo.szNotice));

		AsSocialManager.Instance.SocialUI.SetNotice( notice);
		#if _SOCIAL_LOG_
		Debug.Log( "SocialNotice : " + notice);
		#endif

		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(380), eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void SocialUiScroll( byte[] _packet)
	{
		body_SC_SOCIAL_UI_SCROLL  socialScroll = new body_SC_SOCIAL_UI_SCROLL();
		socialScroll.PacketBytesToClass( _packet);

		eSOCIAL_UI_TYPE eType = (eSOCIAL_UI_TYPE)socialScroll.eType;

		#if _SOCIAL_LOG_
		Debug.Log( "SocialUiScroll  : " + eType.ToString());
		#endif

		switch( eType)
		{
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY:
			{
				if( socialScroll.socialHistory.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)
				{
					if( null == AsSocialManager.Instance.SocialUI.m_SocialDlg)
					{
						Debug.Log( "SocialDlg Friendlist  is not exist nUserUniqKey:" + socialScroll.socialHistory.nUserUniqKey);
						break;
					}

					AsSocialManager.Instance.SocialUI.SetHistoryList( socialScroll.socialHistory);
				}
				else
				{//Clone
					if( null == AsSocialManager.Instance.SocialUI.m_ObjectSocialCloneDlg)
					{
						Debug.Log( "SocialCloneDlg Historylist  is not exist. nUserUniqKey:" + socialScroll.socialHistory.nUserUniqKey);
						break;
					}

					AsSocialManager.Instance.SocialUI.SetSocialCloneHistoryList( socialScroll.socialHistory);
				}
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND:
			{
				if( socialScroll.friendList.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)
				{
					if( null == AsSocialManager.Instance.SocialUI.m_SocialDlg)
					{
						Debug.Log( "SocialDlg Friendlist  is not exist. nUserUniqKey:" + socialScroll.friendList.nUserUniqKey);
						break;
					}

					AsSocialManager.Instance.SocialUI.SetFriendList( socialScroll.friendList);
				}
				else
				{//Clone
					if( null == AsSocialManager.Instance.SocialUI.m_ObjectSocialCloneDlg)
					{
						Debug.Log( "SocialCloneDlg Friendlist  is not exist.nUserUniqKey" + socialScroll.friendList.nUserUniqKey);
						break;
					}

					AsSocialManager.Instance.SocialUI.SetSocialCloneFriendList( socialScroll.friendList);
				}

			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_RANDOM:
			{
				if( null == AsSocialManager.Instance.SocialUI.m_FindFriendDlg)
				{
					Debug.Log( "FindFriendDlg Randomlist  is not exist");
					break;
				}
				AsSocialManager.Instance.SocialUI.SetFriendRandomList( socialScroll.randomList);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_BLOCK:
			{
				if( null == AsSocialManager.Instance.SocialUI.m_SocialDlg)
				{
					Debug.Log( "SocialDlg Block list is not exist");
					break;
				}
				AsSocialManager.Instance.SocialUI.SetBlockOutList( socialScroll.blockOutList);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY:
			{
				if( null == AsSocialManager.Instance.SocialUI.m_FindFriendDlg)
				{
					Debug.Log( "FindFriendDlg FriendApplylist  is not exist");
					break;
				}
				AsSocialManager.Instance.SocialUI.SetFriendApplyList( socialScroll.friendApplyList);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_RECOMMEND:
			{
				if( null == AsSocialManager.Instance.SocialUI.m_FindFriendDlg)
				{
					Debug.Log( "FindFriendDlg Recommedn  is not exist");
					break;
				}
				AsSocialManager.Instance.SocialUI.SetRecommedList( socialScroll.recommendList);
			}
			break;
		}
	}

	void FriendConnect( byte[] _packet)
	{
		if( null == AsChatManager.Instance)
			return;
		
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_FriendOnlineAlarm))
			return;
		
		body_SC_FRIEND_CONNECT connect = new body_SC_FRIEND_CONNECT();
		connect.PacketBytesToClass( _packet);

		string strMsg;
		string userName;

		userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( connect.szCharName));

		if( true == connect.bConnect)
		{
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(185), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		}
		else
		{
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(186), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		}
	}

	void FriendList( byte[] _packet)
	{
		body1_SC_FRIEND_LIST listResult = new body1_SC_FRIEND_LIST();
		listResult.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendList nUserUniqKey : " + listResult.nUserUniqKey.ToString());
		#endif

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		if( listResult.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)
			AsSocialManager.Instance.SocialUI.SetFriendList( listResult);
		else
			AsSocialManager.Instance.SocialUI.SetSocialCloneFriendList( listResult);
	}

	void FriendInvite( byte[] _packet)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_FriendInviteRefuse))
			return;
		
		body_SC_FRIEND_INVITE invite = new body_SC_FRIEND_INVITE();
		invite.PacketBytesToClass( _packet);
		AsSocialManager.Instance.FriendInvite( invite);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendInvite nUserUniqKey: " + invite.nUserUniqKey.ToString());
		#endif
	}

	void FriendInviteResult( byte[] _packet)
	{
		body_SC_FRIEND_INVITE_RESULT inviteResult = new body_SC_FRIEND_INVITE_RESULT();
		inviteResult.PacketBytesToClass( _packet);

		string strMsg;
		string userName;

		userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( inviteResult.szCharName));

		switch( inviteResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(164), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			//친구추가 퀘스트 체크.
			QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_CALL_ADD_FRIEND, new AchFriendCall( 1));
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_FULL:
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_RECV_FULL_OTHER:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(160), null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(160), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(167), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_NOTHING_USER:	//친구 초대 실패( 없는 유저).
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(159), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_FULL_OTHER: //친구 초대 실패( 더이상 초대 불과. 대상).
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(348), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_ALREADY:	//친구 초대 실패( 이미 친구 관계).
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(176), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_ALREADY_BLOCKOUT: //친구 초대 실패( 차단).
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(161), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(161));
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_TOTURIALZONE:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(423), null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(423), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;	
		case eRESULTCODE.eRESULT_FAIL_FRIEND_INVITE_PVP:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4102), null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(4102), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;		
		}

		#if _SOCIAL_LOG_
		Debug.Log( "FriendInviteResult: " + inviteResult.eResult+ "userName:" + userName);
		#endif
	}

	void FriendJoinResult( byte[] _packet)
	{
		body_SC_FRIEND_JOIN_RESULT joinResult = new body_SC_FRIEND_JOIN_RESULT();
		joinResult.PacketBytesToClass( _packet);

		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);

		switch( joinResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_REFUSE:	//친구 요청 거절.
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(162), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(162));
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN:	//친구 수락 실패.
			message = AsTableManager.Instance.GetTbl_String(935);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_NOT_EXIST_INVITE:	//요청이 없음.
			message = AsTableManager.Instance.GetTbl_String(936);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRINND_JOIN_ALREADY:	//이미 수락한 대상.
			message = AsTableManager.Instance.GetTbl_String(937);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_FULL:	//더이상 수락할수 없음.
			message =  AsTableManager.Instance.GetTbl_String(160);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_FULL_OTHER://대상이 더이상 수락을 못함.
			message = AsTableManager.Instance.GetTbl_String(939);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_ALREADY_BLOCKOUT:	//차단목록에 있는 대상입니다.
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(4094), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(4094));
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_JOIN_ALREADY_BLOCKOUT_OTHER:	//대상의 차단목록에 자신이 존재함.
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(4094), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(4094));
			break;
		default:
			message = joinResult.eResult.ToString();
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		#if _SOCIAL_LOG_
		Debug.Log( "FriendJoinResult");
		#endif
	}

	void FriendInsert( byte[] _packet)
	{
		body_SC_FRIEND_INSERT friendInsert = new body_SC_FRIEND_INSERT();
		friendInsert.PacketBytesToClass( _packet);

		string strMsg;
		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( friendInsert.szCharName));

		#if _SOCIAL_LOG_
		Debug.Log( "FriendInsert userName :  " + userName);
		#endif

		strMsg = string.Format( AsTableManager.Instance.GetTbl_String(163), userName);
		AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
		AsSocialManager.Instance.SocialUI.RequestFirstPageByFriendList();
	}

	void FriendDelete( byte[] _packet)
	{
		body_SC_FRIEND_DELETE friendDelete = new body_SC_FRIEND_DELETE();
		friendDelete.PacketBytesToClass( _packet);
		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( friendDelete.szCharName));
		#if _SOCIAL_LOG_
		Debug.Log( "FriendDelete userName:  "+  userName);
		#endif

		AsSocialManager.Instance.SocialUI.RequestFirstPageByFriendList();
		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(168), userName);
		AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
	}

	void FriendClearResult( byte[] _packet)
	{
		body_SC_FRIEND_CLEAR_RESULT clearResult = new body_SC_FRIEND_CLEAR_RESULT();
		clearResult.PacketBytesToClass( _packet);
		#if _SOCIAL_LOG_
		Debug.Log( "FriendClearResult : " + clearResult.eResult.ToString());
		#endif

		switch( clearResult.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_FRIEND_DELETE:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(167), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		}
	}

	void FriendHello( byte[] _packet)
	{
		body_SC_FRIEND_HELLO hello = new body_SC_FRIEND_HELLO();
		hello.PacketBytesToClass( _packet);

		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_CHEER, new AchCheer( CheerType.GET, 1));

		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( hello.szCharName));

		string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(172), userName);
		AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(strMsg);
		AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();

		if( hello.bDual)
		{
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(173), userName);
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
		}

		#if _SOCIAL_LOG_
		Debug.Log( "FriendHello : " + userName);
		#endif
	}

	void FriendHelloResult( byte[] _packet)
	{
		body_SC_FRIEND_HELLO_RESULT helloResult = new body_SC_FRIEND_HELLO_RESULT();
		helloResult.PacketBytesToClass( _packet);

		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_CHEER, new AchCheer( CheerType.SEND, 1));

		#if _SOCIAL_LOG_
		Debug.Log( "FriendHelloResult : " + helloResult.eResult.ToString());
		#endif
		string userName, strMsg;

		userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( helloResult.szCharName));

		switch( helloResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			{
				strMsg = string.Format( AsTableManager.Instance.GetTbl_String(171), userName);
				AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
				AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();

				if( helloResult.bDual)
				{
					strMsg = string.Format( AsTableManager.Instance.GetTbl_String(173), userName);
					AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
				}
			}
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_HELLO_MAX_COUNT:
			{
				strMsg = string.Format( AsTableManager.Instance.GetTbl_String(181), userName);
				AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);

				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),AsTableManager.Instance.GetTbl_String(181));
			}
			break;
		}
	}

	void FriendRecallResult( byte[] _packet)
	{
		body_SC_FRIEND_RECALL_RESULT recallResult = new body_SC_FRIEND_RECALL_RESULT();
		recallResult.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendRecallResult : " + recallResult.eResult.ToString());
		#endif

		string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( recallResult.szCharName));

		switch( recallResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			{

				if(recallResult.nRecallCount == 0)
				{
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(4097),  userName);
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),strMsg);
					AsChatManager.Instance.InsertChat(strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
					AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();
				}
				else
				{
					string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(4096), userName ,recallResult.nRecallCount, recallResult.nMaxRecallCount);
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),strMsg);
					AsChatManager.Instance.InsertChat(strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
					AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();
				}
			}
			break;
		}
	}

	void FriendRandom( byte[] _packet)
	{
		body1_SC_FRIEND_RANDOM friendRandomList = new body1_SC_FRIEND_RANDOM();
		friendRandomList.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendRandom : " + friendRandomList.nCnt);
		#endif

		AsSocialManager.Instance.SocialUI.FindFriendDlg.SetFriendRandomList( friendRandomList);
	}

	void FriendWarp( byte[] _packet)
	{
		body_SC_FRIEND_WARP friendWarp = new body_SC_FRIEND_WARP();
		friendWarp.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendWarp : " + friendWarp.eResult.ToString());
		#endif
		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		switch( friendWarp.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_WARP:
				message = AsTableManager.Instance.GetTbl_String(940);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_WARP_BUSY:
				message = AsTableManager.Instance.GetTbl_String(941);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_WARP_COOLTIME:
				message = AsTableManager.Instance.GetTbl_String(942);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_WARP_NOT_ACTIVE:
				message = AsTableManager.Instance.GetTbl_String(943);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_WARP_TUTORIALZONE:
				message = AsTableManager.Instance.GetTbl_String(424);
		break;	
		default:
			message = friendWarp.eResult.ToString();
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	void FriendConditionResult( byte[] _packet)
	{
		body_SC_FRIEND_CONDITION_RESULT friendCondition = new body_SC_FRIEND_CONDITION_RESULT();
		friendCondition.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendConditionResult : " + friendCondition.eResult.ToString());
		#endif

		switch( friendCondition.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(4087));
			AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_CONDITION:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),AsTableManager.Instance.GetTbl_String(4088));
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_CONDITION_FULL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),AsTableManager.Instance.GetTbl_String(4088));
			break;
		}
	}
	
	void FriendConnectRequestResult( byte[] _packet)
	{
		body_SC_FRIEND_CONNECT_REQUEST_RESULT friendConnectRequest = new body_SC_FRIEND_CONNECT_REQUEST_RESULT();
		friendConnectRequest.PacketBytesToClass( _packet);

		#if _SOCIAL_LOG_
		Debug.Log( "FriendConnectRequestResult : " + friendConnectRequest.eResult.ToString());
		#endif
		
		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		
		switch( friendConnectRequest.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
				message = string.Format(AsTableManager.Instance.GetTbl_String(2043),AsSocialManager.Instance.SocialData.ConnectReuqestUserName);
				AsSocialManager.Instance.SocialUI.RequestCurrentPageByFriendList();
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_CONNECT_REQUEST:
				message = AsTableManager.Instance.GetTbl_String(2041);
			break;
		case eRESULTCODE.eRESULT_FAIL_FRIEND_CONNECT_REQUEST_ALREADY_CONNECT:
				message = AsTableManager.Instance.GetTbl_String(2042);
			break;	
		case eRESULTCODE.eRESULT_FAIL_FRIEND_CONNECT_REQUEST_COOLTIME:
				message = AsTableManager.Instance.GetTbl_String(2042);
			break;	
		default:
			message = friendConnectRequest.eResult.ToString();
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	

	void FriendBlockOutList( byte[] _packet)
	{
		body1_SC_BLOCKOUT_LIST blockOutList = new body1_SC_BLOCKOUT_LIST();
		blockOutList.PacketBytesToClass( _packet);
		#if _SOCIAL_LOG_
		Debug.Log( "FriendBlockOutList nCnt : " + blockOutList.nCnt.ToString());
		#endif
		AsSocialManager.Instance.SocialData.ReceiveBlockOutList( blockOutList.body);
		AsSocialManager.Instance.SocialUI.SetBlockOutList();
	}

	void FriendBlockOutInsert( byte[] _packet)
	{
		body_SC_BLOCKOUT_INSERT blockOutInsert = new body_SC_BLOCKOUT_INSERT();
		blockOutInsert.PacketBytesToClass( _packet);
		#if _SOCIAL_LOG_
		Debug.Log( "FriendBlockOutInsert nUserUniqKey : " + blockOutInsert.nUserUniqKey.ToString());
		#endif

		switch( blockOutInsert.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsSocialManager.Instance.SocialData.ReceiveBlockOutInsert( blockOutInsert);
			AsSocialManager.Instance.SocialUI.SetBlockOutList();
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_INSERT:	//차단 추가 실패.
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(295), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_INSERT_NOTHING_USER:	//차단 추가 실패( 없는 유저 / 오프라인 유저).
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(296), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_INSERT_ARLEADY:	//차단 추가 실패( 이미 차단중인 유저).
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(297), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_INSERT_ARLEADY_FRIEND:	//차단 추가 실패( 이미 차단중인 유저).
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(363), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		}
	}

	void FriendBlockOutDelete( byte[] _packet)
	{
		body_SC_BLOCKOUT_DELETE blockOutDelete = new body_SC_BLOCKOUT_DELETE();
		blockOutDelete.PacketBytesToClass( _packet);
		#if _SOCIAL_LOG_
		Debug.Log( "FriendBlockOutDelete nUserUniqKey : " + blockOutDelete.nUserUniqKey.ToString());
		#endif

		switch( blockOutDelete.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
//			string userName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( blockOutDelete.szUserId));
			AsSocialManager.Instance.SocialData.ReceiveBlockOutDelete( blockOutDelete);
			AsSocialManager.Instance.SocialUI.SetBlockOutList();
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_DELETE:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(298), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_BLOCKOUT_DELETE_NOTHING_USER:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(299), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		}
	}

	void SocialItemBuyResult( byte[] _packet)
	{
		body_SC_SOCIAL_ITEMBUY_RESULT itemBuyResult = new body_SC_SOCIAL_ITEMBUY_RESULT();
		itemBuyResult.PacketBytesToClass( _packet);

		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		switch( itemBuyResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			message = string.Empty;
			break;
		case eRESULTCODE.eRESULT_FAIL_SHOP_NOTGOLD:
			message = AsTableManager.Instance.GetTbl_String(97);
			break;
		case eRESULTCODE.eRESULT_FAIL_SHOP_NOTSOCIALPOINT:
			message = AsTableManager.Instance.GetTbl_String(1919);
			break;
		case eRESULTCODE.eRESULT_FAIL_SHOP_INVENTORYFULL:
			message = AsTableManager.Instance.GetTbl_String(1399);
			break;
		case eRESULTCODE.eRESULT_FAIL_SHOP_OVERGOLD:
			message =  AsTableManager.Instance.GetTbl_String(205);
			break;
		case eRESULTCODE.eRESULT_FAIL_SHOP_IMPOSSIBLE_ITEM:
			message = AsTableManager.Instance.GetTbl_String(1415);
			break;
		default:
			message = itemBuyResult.eResult.ToString();
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);

		AsSocialManager.Instance.SocialUI.SetSocialPoint( itemBuyResult.nSocialPoint);
	}

	void GameInviteListResult(byte[] _packet)
	{
		body_SC_GAME_INVITE_LIST_RESULT gameInviteListResult = new body_SC_GAME_INVITE_LIST_RESULT();
		gameInviteListResult.PacketBytesToClass( _packet);

		AsSocialManager.Instance.SocialUI.SetGameInviteRewardList(gameInviteListResult);
	}

	void GameInviteResult(byte[] _packet)
	{
		body_SC_GAME_INVITE_RESULT gameInviteResult = new body_SC_GAME_INVITE_RESULT();
		gameInviteResult.PacketBytesToClass( _packet);
		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		if( gameInviteResult.bCheck)
		{
			switch( gameInviteResult.eResult)
			{
			case eRESULTCODE.eRESULT_SUCC:
				AsSocialManager.Instance.GameInviteResult( gameInviteResult);
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE:
				message = AsTableManager.Instance.GetTbl_String(944);
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_OVER:
				message = AsTableManager.Instance.GetTbl_String(4089);
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_COOLTIME:
#if _USE_BAND
				string user_Key = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( gameInviteResult.szKeyword));
				message = 	string.Format( AsTableManager.Instance.GetTbl_String(4253),AsSocialManager.Instance.SocialData.GetBandMemberName(user_Key));				
#else
				message = AsTableManager.Instance.GetTbl_String(4090);
#endif			
				break;
			default:
				message = gameInviteResult.eResult.ToString();
				break;
			}

			if( message != string.Empty)
				AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else // 등록.
		{
			switch( gameInviteResult.eResult)
			{
			case eRESULTCODE.eRESULT_SUCC:
				#if _SOCIAL_LOG_
				Debug.Log( "GameInviteResult eRESULT_SUCC: ");
				#endif
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE:
				message = AsTableManager.Instance.GetTbl_String(944);
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_OVER:
				message = AsTableManager.Instance.GetTbl_String(4089);
				break;
			case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_COOLTIME:
#if _USE_BAND
				string user_Key = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( gameInviteResult.szKeyword));
				message = 	string.Format( AsTableManager.Instance.GetTbl_String(4253),AsSocialManager.Instance.SocialData.GetBandMemberName(user_Key));				
#else
				message = AsTableManager.Instance.GetTbl_String(4090);
#endif	
				break;	
			default:
				message = gameInviteResult.eResult.ToString();
				break;
			}

			if( message != string.Empty)
				AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
	}

	void GameInviteRewardResult(byte[] _packet)
	{
		body_SC_GAME_INVITE_REWARD_RESULT gameInviteRewardResult = new body_SC_GAME_INVITE_REWARD_RESULT();
		gameInviteRewardResult.PacketBytesToClass( _packet);

		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		switch( gameInviteRewardResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			#if _SOCIAL_LOG_
			Debug.Log( "GameInviteRewardResult eRESULT_SUCC: ");
			#endif
			message = AsTableManager.Instance.GetTbl_String(4067);
			break;
		case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_REWARD_ALREADY:
			message = AsTableManager.Instance.GetTbl_String(4091);
			break;
		case eRESULTCODE.eRESULT_FAIL_GAME_INVITE_NOT_REWARD:
			message = AsTableManager.Instance.GetTbl_String(4092);
			break;
		default:
			message = gameInviteRewardResult.eResult.ToString();
			break;
		}

		if( message != string.Empty)
			AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	#endregion

	#region -ItemProduct
	private void ReceiveItemProductInfo( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductInfo");

		body1_SC_ITEM_PRODUCT_INFO data = new body1_SC_ITEM_PRODUCT_INFO();
		data.PacketBytesToClass( _packet);

		AsUserInfo.Instance.SetProductionProgress( data.bProgress);
		AsUserInfo.Instance.SetProductionInfo( data);
		if( null != AsHudDlgMgr.Instance)
			AsHudDlgMgr.Instance.OpenProductionDlg( data);
	}

	private void ReceiveItemProductTechniqueRegister( byte[] _packet)
	{
		body_SC_ITEM_PRODUCT_TECHNIQUE_REGISTER data = new body_SC_ITEM_PRODUCT_TECHNIQUE_REGISTER();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_TECHNIQUE_REGISTER [ : " + data.eResult);
	}

	private void ReceiveItemProductProgress( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductProgress");

		body_SC_ITEM_PRODUCT_PROGRESS data = new body_SC_ITEM_PRODUCT_PROGRESS();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_PROGRESS [ : " + data.eResult);

		AsCommonSender.isSendProductProgress = false;
	}

	private void ReceiveItemProductState( byte[] _packet)
	{
		body_SC_ITEM_PRODUCT_STATE data = new body_SC_ITEM_PRODUCT_STATE();
		data.PacketBytesToClass( _packet);

		Debug.Log( "ReceiveItemProductState");

		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey)
		{
			if( AsHudDlgMgr.Instance.IsOpenProductionDlg)
				AsHudDlgMgr.Instance.productionDlg.ReceiveProgress( data.bProgress);

			AsUserInfo.Instance.SetProductionProgress( data.bProgress);
			if( true == data.bProgress)
				AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_PRODUCT( data.bProgress));
		}
		else
		{
			AsUserEntity _entity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey);
			if( null != _entity)
				_entity.ShowProductImg( data.bProgress);
		}
	}

	private void ReceiveItemProductRegister( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductRegister");
		
		AsCommonSender.isSendItemProductRegister = false;
		body_SC_ITEM_PRODUCT_REGISTER data = new body_SC_ITEM_PRODUCT_REGISTER();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_REGISTER [ : " + data.eResult);
		else
		{
			string strTitle = AsTableManager.Instance.GetTbl_String(126);
			string strText = AsTableManager.Instance.GetTbl_String(266) + "\n" + Color.red.ToString() + AsTableManager.Instance.GetTbl_String(1336);
			
			int iItemid = 0;
			if( false == AsHudDlgMgr.Instance.productionRandItem )
			{
				iItemid = AsHudDlgMgr.Instance.productionItemId;
			}
			AsHudDlgMgr.Instance.SetMsgBox(
								AsNotify.Instance.ItemViewMessageBox( iItemid, string.Empty, strTitle, strText, 
				null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true) );
		}
	}

	private void ReceiveItemProductCancel( byte[] _packet)
	{
		body_SC_ITEM_PRODUCT_CANCEL data = new body_SC_ITEM_PRODUCT_CANCEL();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_CANCEL [ : " + data.eResult);
	}

	private void ReceiveItemProductReceive( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductReceive");

		body_SC_ITEM_PRODUCT_RECEIVE_RESULT data = new body_SC_ITEM_PRODUCT_RECEIVE_RESULT();
		data.PacketBytesToClass( _packet);		
		
		if( eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL == data.eResult)
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(272), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_RECEIVE_RESULT [ : " + data.eResult);

		QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_PRODUCE_ITEM, new AchProduceItem( data.nProductTableIdx, 1));
	}

	private void ReceiveItemProductCashDirect( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductCashDirect");

		body_SC_ITEM_PRODUCT_CASH_DIRECT data = new body_SC_ITEM_PRODUCT_CASH_DIRECT();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_CASH_DIRECT [ : " + data.eResult);
	}

	private void ReceiveItemProductCashSlotOpen( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductCashSlotOpen");

		body_SC_ITEM_PRODUCT_CASH_SLOT_OPEN data = new body_SC_ITEM_PRODUCT_CASH_SLOT_OPEN();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_CASH_SLOT_OPEN [ : " + data.eResult);

		if( AsHudDlgMgr.Instance.IsOpenProductionDlg)
			AsHudDlgMgr.Instance.productionDlg.ReceiveCashSlotOpen( data.nSlot);
	}

	private void ReceiveItemProductCashTechniqueOpen( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductCashTechniqueOpen");

		body_SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN data = new body_SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN [ : " + data.eResult);
	}

	private void ReceiveItemProductCashLevelUp( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductCashLevelUp");

		body_SC_ITEM_PRODUCT_CASH_LEVEL_UP data = new body_SC_ITEM_PRODUCT_CASH_LEVEL_UP();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
			Debug.LogError( "failed body_SC_ITEM_PRODUCT_CASH_LEVEL_UP [ : " + data.eResult);
	}

	private void ReceiveItemProductSlotInfo( byte[] _packet)
	{
		body_SC_ITEM_PRODUCT_SLOT_INFO data = new body_SC_ITEM_PRODUCT_SLOT_INFO();
		data.PacketBytesToClass( _packet);

		Debug.Log( "ReceiveItemProductSlotInfo[ nProductSlot : " + data.nProductSlot + " [ nContents : " + data.nContents +
			" [ nRecipeIndex : " + data.sSlotInfo.nRecipeIndex + " [ nProductTime : " + data.sSlotInfo.nProductTime);

		AsUserInfo.Instance.SetProductSlotInfo( data);

		if( AsHudDlgMgr.Instance.IsOpenProductionDlg)
			AsHudDlgMgr.Instance.productionDlg.ReceiveAddProgInfoData( data.nProductSlot, data.sSlotInfo);
	}

	private void ReceiveItemProductTechniqueInfo( byte[] _packet)
	{
		Debug.Log( "ReceiveItemProductTechniqueInfo");

		body_SC_ITEM_PRODUCT_TECHNIQUE_INFO data = new body_SC_ITEM_PRODUCT_TECHNIQUE_INFO();
		data.PacketBytesToClass( _packet);

		int nExp = AsUserInfo.Instance.GetTechInfoDiffValue( data);
		AsUserInfo.Instance.SetProductTechnique( data);

		if( AsHudDlgMgr.Instance.IsOpenProductionDlg)
			AsHudDlgMgr.Instance.productionDlg.ReceiveTechniqueRegister( data);



		StartCoroutine( ExpShow( nExp, AsPanelManager.eCustomFontType.eCustomFontType_EXP2));
	}
	#endregion

	IEnumerator ExpShow( int nExp, AsPanelManager.eCustomFontType _type)
	{
		yield return new WaitForSeconds( 0.5f);

		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			hud.panelManager.ShowNumberPanel( nExp, _type);
		}
	}

	private void ReceiveCollectInfo( byte[] _packet)
	{
		body_SC_COLLECT_INFO data = new body_SC_COLLECT_INFO();
		data.PacketBytesToClass( _packet);

		eCOLLECT_STATE state = (eCOLLECT_STATE)data.eCollectState;
		Debug.Log( "ReceiveCollectInfo [ npc id : " + data.nCollectNpcIdx + "[ char key : " + data.nCollectorUniqKey
			+ "[ state : " + state);

		AsEntityManager.Instance.DispatchMessageByNpcSessionId( data.nCollectNpcIdx, new Msg_CollectInfo( data));

		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCollectorUniqKey)
		{
			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_CollectResult( state));
			if( eCOLLECT_STATE.eCOLLECT_STATE_CANCEL == state || eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE == state)
				AsHUDController.Instance.targetInfo.EmptyCollectEntity();
		}
		else
		{
			AsEntityManager.Instance.DispatchMessageByUniqueKey( data.nCollectorUniqKey, new Msg_CollectInfo( data));
		}
	}

	private void ReceiveCollectResult( byte[] _packet)
	{
		AsCommonSender.isSendCollectRequest = false;

		body_SC_COLLECT_RESULT data = new body_SC_COLLECT_RESULT();
		data.PacketBytesToClass( _packet);

		eCOLLECT_STATE state = ( eCOLLECT_STATE)data.eCollectState;
		Debug.Log( "ReceiveCollectResult [ result : " + data.eResult + " [ state :" + state);

		if( eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL == data.eResult)
		{
			AsHUDController.Instance.targetInfo.EmptyCollectEntity();
			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_CollectResult( eCOLLECT_STATE.eCOLLECT_STATE_CANCEL));
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(283), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( eRESULTCODE.eRESULT_SUCC != data.eResult)
		{
			Debug.Log( "failed body_SC_COLLECT_RESULT [ : " + data.eResult);
			AsHUDController.Instance.targetInfo.EmptyCollectEntity();
			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_CollectResult( eCOLLECT_STATE.eCOLLECT_STATE_CANCEL));
			return;
		}

		if( eCOLLECT_STATE.eCOLLECT_STATE_CANCEL == state || eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE == state)
		{
			if( state == eCOLLECT_STATE.eCOLLECT_STATE_COMPLETE)
			{
				int collectionID = AsHUDController.Instance.targetInfo.getCurTargetEntity.GetProperty<int>( eComponentProperty.NPC_ID);
				QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_TRYCOLLECTING, new AchGetCollection( collectionID, 1));
			}

			AsHUDController.Instance.targetInfo.EmptyCollectEntity();
		}
	}

	private void ReciveEnchantResult( byte[] _packet)
	{
		Debug.Log( "ReciveEnchantResult");
		
		AsCommonSender.isSendEnchant = false;

		body_SC_ITEM_ENCHANT_RESULT data = new body_SC_ITEM_ENCHANT_RESULT();
		data.PacketBytesToClass( _packet);

		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_ITEM_ENCHANT_SUCCESS:
			AsHudDlgMgr.Instance.enchantDlg.ReciveSuccResult();
			return;
		}

		Debug.LogError( "ReciveEnchantResult error : " + data.eResult);
	}

	private void ReciveOutEnchantResult( byte[] _packet)
	{
		Debug.Log( "ReciveOutEnchantResult");
		
		AsCommonSender.isSendEnchant = false;
		
		body_SC_ITEM_ENCHANT_OUT_RESULT data = new body_SC_ITEM_ENCHANT_OUT_RESULT();
		data.PacketBytesToClass( _packet);

		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_ITEM_ENCHANT_SUCCESS:
			AsHudDlgMgr.Instance.enchantDlg.ReciveOutSuccResult();
			return;
		}

		Debug.LogError( "ReciveOutEnchantResult error : " + data.eResult);
	}

	private void ReciveStrengthenResult( byte[] _packet)
	{
		body_SC_ITEM_STRENGTHEN_RESULT data = new body_SC_ITEM_STRENGTHEN_RESULT();
		data.PacketBytesToClass( _packet);		

		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == data.nCharUniqKey)
		{
			AsCommonSender.isSendStrengthen = false;
			
			if( true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg)
				AsHudDlgMgr.Instance.strengthenDlg.ReciveResult( data);
		}

		AsUserEntity _userentity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey);
		if( null == _userentity ||  null == _userentity.ModelObject)
			return;

		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL:
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_StrengthenBreak", _userentity.transform, false, 0f, 1.0f);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6012_EFF_StrengthenBreak", _userentity.transform.position, false);
			break;
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL_PROTECT:
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_StrengthenFail", _userentity.transform, false, 0f, 1.0f);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6012_EFF_StrengthenBreak_Protection", _userentity.transform.position, false);
			break;
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_SUCCESS:
			AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_StrengthenSuccess", _userentity.transform, false, 0f, 1.0f);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6012_EFF_StrengthenSuccess", _userentity.transform.position, false);
			break;
		default:
			Debug.LogError( "ReciveStrengthenResult()[ error : " + data.eResult);
			break;
		}

		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			hud.panelManager.ShowStrengthenPanel( _userentity.transform.gameObject, data.eResult, data.nCharUniqKey);
		}
	}

	private void ReciveItemMixResult( byte[] _packet)
	{
		body_SC_ITEM_MIX_RESULT data = new body_SC_ITEM_MIX_RESULT();
		data.PacketBytesToClass( _packet);
		
		AsCommonSender.isSendItemMix = false;
		if( data.eResult != eRESULTCODE.eRESULT_SUCC && data.eResult != eRESULTCODE.eRESULT_ITEM_MIX_COS_FAIL )
		{
			if( data.eResult == eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL )
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
			else
			{
				Debug.LogError("ReciveItemMixResult()[ data.eResult != eRESULTCODE.eRESULT_SUCC ] : + " + data.eResult );
			}
			return;
		}
		
		switch ( (eITEM_MIX_TYPE)data.eMixType )
		{
		case eITEM_MIX_TYPE.eITEM_MIX_TYPE_DECOMPOSITION:
			if( AsHudDlgMgr.Instance.IsOpenSynDisDlg )
				AsHudDlgMgr.Instance.m_SynDisDlg.ReceivePacket( data );
			break;
		case eITEM_MIX_TYPE.eITEM_MIX_TYPE_EQUIP_OPT:
			if( AsHudDlgMgr.Instance.IsOpenSynOptionDlg )
				AsHudDlgMgr.Instance.m_SynOptionDlg.ReceivePacket( data );
			break;
		case eITEM_MIX_TYPE.eITEM_MIX_TYPE_SOUL_STONE:
			if( AsHudDlgMgr.Instance.IsOpenSynEnchantDlg )
				AsHudDlgMgr.Instance.m_SynEnchantDlg.ReceivePacket( data );
			break;
		case eITEM_MIX_TYPE.eITEM_MIX_TYPE_COSTUME:
			if( AsHudDlgMgr.Instance.IsOpenSynCosDlg )
				AsHudDlgMgr.Instance.m_SynCosDlg.ReceivePacket( data );
			break;
		}
	}

	private void ReciveCostumeItemMixUpResult( byte[] _packet)
	{
		body_SC_COS_ITEM_MIX_UP_RESULT data = new body_SC_COS_ITEM_MIX_UP_RESULT();
		data.PacketBytesToClass( _packet);
		
		AsCommonSender.isSendItemMix = false;
		AsSynthesisCosDlg.isProcessing = false;
		if( data.eResult == eRESULTCODE.eRESULT_SUCC )
		{
			if( AsHudDlgMgr.Instance.IsOpenSynCosDlg )
				AsHudDlgMgr.Instance.m_SynCosDlg.ReceivePacket_CosItemMixResult( data );
		}
		else if( data.eResult == eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL )
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
			                             null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else
		{
			Debug.LogError("ReceiveCostumeItemMixUpResult()[ data.eResult != eRESULTCODE.eRESULT_SUCC ] : + " + data.eResult );
		}
		AsSynthesisCosDlg.isAuthorityButtonClick = true;
	}

	private void ReciveCostumeItemMixUpgradeResult( byte[] _packet)
	{
		body_SC_COS_ITEM_MIX_UPGRADE_RESULT data = new body_SC_COS_ITEM_MIX_UPGRADE_RESULT();
		data.PacketBytesToClass( _packet);
		
		AsCommonSender.isSendItemMix = false;
		AsSynthesisCosDlg.isProcessing = false;
		if( data.eResult == eRESULTCODE.eRESULT_SUCC )
		{
			if( AsHudDlgMgr.Instance.IsOpenSynCosDlg )
				AsHudDlgMgr.Instance.m_SynCosDlg.ReceivePacket_CosItemMixUpgradeResult( data );
		}
		else if( data.eResult == eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL )
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
			                             null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else
		{
			Debug.LogError("ReceiveCostumeItemMixUpgradeResult()[ data.eResult != eRESULTCODE.eRESULT_SUCC ] : + " + data.eResult );
		}
		AsSynthesisCosDlg.isAuthorityButtonClick = true;
	}

	private void ReceiveItemTimeExpire( byte[] _packet)
	{
		body1_SC_ITEM_TIME_EXPIRE data = new body1_SC_ITEM_TIME_EXPIRE();
		data.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance)
		{
			Debug.LogError( "AsCommonProcess_2::ReceiveItemTimeExpire() [ null == AsHudDlgMgr.Instance ] ");
			return;
		}

		AsHudDlgMgr.Instance.ReceiveItemTimeExpire( data);

		AsPStoreManager.Instance.ExpireProcess( data);//$yde
	}

	private void ReciveOtherUserInfo( byte[] _packet)
	{
		Debug.Log( "GuildMemberInfoDetailResult");

		body1_SC_OTHER_INFO result = new body1_SC_OTHER_INFO();
		result.PacketBytesToClass( _packet);

		AsHudDlgMgr.Instance.OpenOtherUserStatusDlg( result, -8f);
	}


	private void ReciveConstumeOnOff( byte[] _packet)
	{
		Debug.Log( "ReciveConstumeOnOff");

		body_SC_COSTUME_ONOFF result = new body_SC_COSTUME_ONOFF();
		result.PacketBytesToClass( _packet);

		Debug.Log( "ReciveConstumeOnOff key :" + result.nCharUniqKey + " [is: " + result.bCostumeOnOff);

		if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState)
		{
			AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( result.nCharUniqKey);
			if( null != userEntity)
			{
				userEntity.SetHairItemIndex( result.nHair + result.nHairColor );
				if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
					 userEntity.SetCostumeOnOff_Coroutine( result.bCostumeOnOff);
				else
					userEntity.SetCostumeOnOff( result.bCostumeOnOff);
			}
		}

		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey)
		{
			AsUserInfo.Instance.SetCostumeOnOff( result.bCostumeOnOff);
			AsUserInfo.Instance.SavedCharStat.nHairItemIndex = result.nHair + result.nHairColor;
			if( AsHudDlgMgr.Instance.IsOpenPlayerStatus)
				AsHudDlgMgr.Instance.playerStatusDlg.ReceiveConstumeOnOff();
		}
	}


	private void ReceiveResurrectPenaltyClear( byte[] _packet)
	{
		body_SC_RESURRECT_PENALTY_CLEAR clear = new body_SC_RESURRECT_PENALTY_CLEAR();
		clear.PacketBytesToClass( _packet);

		AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_DeathDebuffClear());

		Debug.Log( "ReceiveResurrectPenaltyClear: result = " + clear.eResult_);
	}

	//$yde
	private void ReceiveCouponRegist( byte[] _packet)
	{
		body_SC_COUPON_REGIST regist = new body_SC_COUPON_REGIST();
		regist.PacketBytesToClass( _packet);

		switch( regist.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			Debug.Log( "ReceiveCouponRegist: result = " + regist.eResult);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1631), regist.CouponReward);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), sb.ToString());
			break;
		default:
			Debug.Log( "ReceiveCouponRegist: unknown result = " + regist.eResult);
			break;
		}
	}

	private void ReceiveEventList( byte[] _packet)
	{
		body1_SC_EVENT_LIST result = new body1_SC_EVENT_LIST();
		result.PacketBytesToClass( _packet);

		if( null != AsHUDController.Instance.m_NpcMenu)
			AsHUDController.Instance.m_NpcMenu.ReceiveEventNpcList( result);

		Debug.Log( "ReceiveEventList");
	}

	#region -EventStart
	private void ServerEventStart( byte[] _packet)
	{
		body1_SC_SERVER_EVENT_START packet = new body1_SC_SERVER_EVENT_START();
		packet.PacketBytesToClass( _packet);

		if( null == packet.body)
			return;

		foreach( body2_SC_SERVER_EVENT_START data in packet.body)
			AsEventManager.Instance.Insert( data);
		
		AutoCombatManager.Instance.EventProcess( packet);//$yde
	}

	private void ServerEventStop( byte[] _packet)
	{
		body1_SC_SERVER_EVENT_STOP packet = new body1_SC_SERVER_EVENT_STOP();
		packet.PacketBytesToClass( _packet);

		foreach( body2_SC_SERVER_EVENT_STOP data in packet.body)
			AsEventManager.Instance.Remove( data.nEventKey);
		
		AutoCombatManager.Instance.EventProcess( packet);//$yde
	}
	#endregion

	private void ReceiveGameReview( byte[] _packet)
	{
		TerrainMgr.Instance.OpenReviewEventDlg();
		Debug.Log( "ReceiveGameReview");
	}

	private void ReceiveUserEventNotify( byte[] _packet)
	{
		Debug.Log( "ReceiveUserEventNotify");
		body_SC_USER_EVENT_NOTIFY notify = new body_SC_USER_EVENT_NOTIFY();
		notify.PacketBytesToClass( _packet);
		
		/*
		AsEmotionManager.Instance.Event_Condition_GetRareItem(notify);//$yde

		if( (eUSEREVENT_ITEM_GETTYPE)notify.nValue_1 == eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_RULLET)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null != userEntity)
			{
				string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
				if( userName.CompareTo( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName))) == 0)
				{
					AsEventNotifyMgr.Instance.SetRandItemNotify( notify);
					return;
				}
			}

			AsEventNotifyMgr.Instance.ReceiveUserEventNotify( notify);

			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ != notify.nValue_2)
			{
				AsUserEntity _entity = AsEntityManager.Instance.GetUserEntityByUniqueId( (uint)notify.nValue_2);
				if( null != _entity)
				{
					Debug.Log("other user");
				}
			}
			else
			{
				Debug.Log("Player");
			}
		}
		else
		{
			AsEventNotifyMgr.Instance.ReceiveUserEventNotify( notify);
		}
		*/
		
		if( eUSEREVENTTYPE.eUSEREVENTTYPE_ITEM == (eUSEREVENTTYPE)notify.eType)
		{
			AsEmotionManager.Instance.Event_Condition_GetRareItem(notify);//$yde
	
			if( (eUSEREVENT_ITEM_GETTYPE)notify.nValue_1 == eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_RULLET)
			{
				AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != userEntity)
				{
					string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
					if( userName.CompareTo( AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName))) == 0)
					{
						AsEventNotifyMgr.Instance.SetRandItemNotify( notify);
						return;
					}
				}
	
				AsEventNotifyMgr.Instance.ReceiveUserEventNotify( notify);
	
				if( AsUserInfo.Instance.SavedCharStat.uniqKey_ != notify.nValue_2)
				{
					AsUserEntity _entity = AsEntityManager.Instance.GetUserEntityByUniqueId( (uint)notify.nValue_2);
					if( null != _entity)
					{
						Debug.Log("other user");
					}
				}
				else
				{
					Debug.Log("Player");
				}
			}
			else
			{
				AsEventNotifyMgr.Instance.ReceiveUserEventNotify( notify);
			}
		}
		else if( eUSEREVENTTYPE.eUSEREVENTTYPE_ARENA == (eUSEREVENTTYPE)notify.eType)
		{
			if( eUSEREVENT_ARENATYPE.eUSEREVENT_ARENATYPE_LEAVE == (eUSEREVENT_ARENATYPE)notify.nValue_1)
			{
				//string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 2109), AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName)));
				//AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
				AsPvpManager.Instance.ShowKillMessage( notify.nValue_2, 0);
				
				if( notify.nValue_2 == AsUserInfo.Instance.SavedCharStat.uniqKey_)
				{
					AsPvpManager.Instance.SetBackGroundDelayTime( notify.nServerTime);
				}
			}
			else if( eUSEREVENT_ARENATYPE.eUSEREVENT_ARENATYPE_DEATH == (eUSEREVENT_ARENATYPE)notify.nValue_1)
			{
				AsPvpManager.Instance.ShowKillMessage( notify.nValue_2, notify.nValue_3);
			}
		}
	}

	#region -quest wanted-
	private void WantedQuestStart( byte[] _packet)
	{
		body_SC_WANTED_START data = new body_SC_WANTED_START();
		data.PacketBytesToClass( _packet);

		Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord( data.nQuestTableIdx);

		questRecord.QuestDataInfo.NowQuestProgressState = ArkQuestmanager.ConvertQuestProgressNetToClient( ( QuestProgressStateNet)data.eStatus);

		ArkQuestmanager.instance.AcceptQuest( data.nQuestTableIdx, true);

		Debug.LogWarning( "Wanted quest start = [" + data.nQuestTableIdx +"]["+data.eStatus +"]");

		// 버튼 U.I를 보여줘야 한다.
		if( AsHudDlgMgr.Instance.wantedQuestBtn != null)
		{
			AsHudDlgMgr.Instance.wantedQuestBtn.wantedState = (QuestProgressStateNet)data.eStatus;
			AsHudDlgMgr.Instance.wantedQuestBtn.gameObject.SetActiveRecursively( true);
			AsHudDlgMgr.Instance.wantedQuestBtn.CheckVisible();

			if( questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR)
				AsHudDlgMgr.Instance.wantedQuestBtn.Clear = true;
			else
				AsHudDlgMgr.Instance.wantedQuestBtn.Clear = false;
		}
	}

	private void WantedQuestClear( byte[] _packet)
	{
		ArkQuest wantedQuest = ArkQuestmanager.instance.GetWantedQuest();
		if( wantedQuest != null)
		{
			Debug.LogWarning( "Wanted Quest Clear");
			ArkQuestmanager.instance.ClearQuest( wantedQuest.GetQuestData().Info.ID);
			AsHudDlgMgr.Instance.wantedQuestBtn.Clear = true;
		}
	}

	private void WantedQuestCompleteResult( byte[] _packet)
	{
		body_SC_WANTED_COMPLETE data = new body_SC_WANTED_COMPLETE();
		data.PacketBytesToClass( _packet);

		if( data.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			ArkQuest wantedQuest = ArkQuestmanager.instance.GetWantedQuest();
			if( wantedQuest != null)
			{
				ArkQuestmanager.instance.CompleteQuest( wantedQuest.GetQuestData().Info.ID);
				AsHudDlgMgr.Instance.CloseQuestAccept();
			}
		}
		else
		{
			if (data.eResult == eRESULTCODE.eRESULT_FAIL_REWARD_EMPTYITEMSLOT)
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(811));
		}
	}
	#endregion

	#region -Condition
	private void ReceiveCondition( byte[] _packet)
	{
		body_SC_CONDITION condition = new body_SC_CONDITION();
		condition.PacketBytesToClass( _packet);

		AsUserInfo.Instance.CurConditionValue = (int)condition.nCondition;
	}
	#endregion

	#region -Ranking
	private void RankMyRankLoadResult( byte[] _packet)
	{
		body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT result = new body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogError( "RankMyRankLoadResult : " + result.eResult);
			return;
		}

		AsHudDlgMgr.Instance.OpenRankingDlg( result);
	}

	private void RankItemMyRankLoadResult( byte[] _packet)
	{
		body_SC_RANK_MYRANK_LOAD_RESULT result = new body_SC_RANK_MYRANK_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogError( "RankItemMyRankLoadResult : " + result.eResult);
			return;
		}

		AsHudDlgMgr.Instance.InsertWorldRankData( result);
	}

	private void RankItemTopLoadResult( byte[] _packet)
	{
		body_SC_RANK_TOP_LOAD_RESULT result = new body_SC_RANK_TOP_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogError( "RankItemTopLoadResult : " + result.eResult);
			return;
		}

		AsHudDlgMgr.Instance.InsertWorldRankData( result);
	}

	private void RankItemMyFriendLoadResult( byte[] _packet)
	{
		body_SC_RANK_MYFRIEND_LOAD_RESULT result = new body_SC_RANK_MYFRIEND_LOAD_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			Debug.LogWarning( "RankItemMyFriendLoadResult : " + result.eResult);
			return;
		}

		AsHudDlgMgr.Instance.InsertFriendRankData( result);
	}

	private void RankChangeMyRank( byte[] _packet)
	{
		body_SC_RANK_CHANGE_MYRANK changeRank = new body_SC_RANK_CHANGE_MYRANK();
		changeRank.PacketBytesToClass( _packet);

		AsHudDlgMgr.Instance.PromptRankChangeAlarm( changeRank);
	}
	
	private void RankChangeRankPoint( byte[] _packet)
	{
		body_SC_RANK_CHANGE_RANKPOINT result = new body_SC_RANK_CHANGE_RANKPOINT();
		result.PacketBytesToClass( _packet);
		
		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( result.nCharUniqKey);
		if( null == userEntity)
		{
			Debug.LogWarning( "RankChangeRankPoint(), userEntity : " + result.nCharUniqKey);
			return;
		}
		
		if( eRANKTYPE.eRANKTYPE_ITEM == result.eRankType)
		{
			if (AsUserInfo.Instance.GetCurrentUserEntity().UniqueId == result.nCharUniqKey)
			{
				AsUserInfo.Instance.RankPoint = result.nRankPoint;

				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_RANK_POINT, new AchGetRankPoint(result.nRankPoint));
			}
			else
				userEntity.RankPoint = result.nRankPoint;
	
			if( null != userEntity.namePanel)
				userEntity.namePanel.SetRankMark();
		}
	}
	#endregion
	
	/* Not Used
	#region -Instance_Dungeon
	private void InDun_Create_Result( byte[] _packet)
	{
		body_SC_INSTANCE_CREATE_RESULT result = new body_SC_INSTANCE_CREATE_RESULT();
		result.PacketBytesToClass( _packet);

		// InDun create failed
		AsInstanceDungeonManager.Instance.Recv_InDun_Create_Result( result);
	}

	private void InDun_Create( byte[] _packet)
	{
		body_SC_INSTANCE_CREATE result = new body_SC_INSTANCE_CREATE();
		result.PacketBytesToClass( _packet);

		// InDun create succeeded
		AsInstanceDungeonManager.Instance.Recv_InDun_Create( result);
	}

	private void InDun_Enter_Result( byte[] _packet)
	{
		body_SC_ENTER_INSTANCE_RESULT result = new body_SC_ENTER_INSTANCE_RESULT();
		result.PacketBytesToClass( _packet);

		// map load( field -> InDun)
		AsInstanceDungeonManager.Instance.Recv_InDun_Enter( result);
	}

	private void InDun_Exit_Result( byte[] _packet)
	{
		body_SC_EXIT_INSTANCE_RESULT result = new body_SC_EXIT_INSTANCE_RESULT();
		result.PacketBytesToClass( _packet);

		// map load( InDun -> field)
		AsInstanceDungeonManager.Instance.Recv_InDun_Exit( result);
	}
	#endregion
	*/

	#region -Pvp
	private void Pvp_Matching_Info_Result( byte[] _packet)
	{
		body_SC_ARENA_MATCHING_INFO_RESULT result = new body_SC_ARENA_MATCHING_INFO_RESULT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Matching_Info_Result( result);
	}

	private void Pvp_Matching_Request_Result( byte[] _packet)
	{
#if !INTEGRATED_ARENA_MATCHING
		body_SC_ARENA_MATCHING_REQUEST_RESULT result = new body_SC_ARENA_MATCHING_REQUEST_RESULT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Matching_Request_Result( result);
#endif
	}

	private void Pvp_Matching_GoInto_Result( byte[] _packet)
	{
#if !INTEGRATED_ARENA_MATCHING
		body_SC_ARENA_MATCHING_GOINTO_RESULT result = new body_SC_ARENA_MATCHING_GOINTO_RESULT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Matching_GoInto_Result( result);
#endif
	}

	private void Pvp_Matching_Notity( byte[] _packet)
	{
#if !INTEGRATED_ARENA_MATCHING
		body_SC_ARENA_MATCHING_NOTIFY result = new body_SC_ARENA_MATCHING_NOTIFY();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Matching_Notity( result);
#endif
	}

	private void Pvp_Matching_LimitCount( byte[] _packet)
	{
		body_SC_ARENA_MATCHING_LIMITCOUNT result = new body_SC_ARENA_MATCHING_LIMITCOUNT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Matching_LimitCount( result);
	}
	
	private void Pvp_Arena_Initilize( byte[] _packet)
	{
		body_SC_ARENA_INITILIZE result = new body_SC_ARENA_INITILIZE();
		result.PacketBytesToClass( _packet);
		
		AsPartyManager.Instance.Initilize();
	}

	private void Pvp_Gogogo( byte[] _packet)
	{
#if !INTEGRATED_ARENA_MATCHING
		body_SC_ARENA_GOGOGO result = new body_SC_ARENA_GOGOGO();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Gogogo( result);
#endif
	}

	private void Pvp_UserInfo_List( byte[] _packet)
	{
		body_SC_ARENA_USERINFO_LIST result = new body_SC_ARENA_USERINFO_LIST();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_UserInfo_List( result);
	}

	private void Pvp_EnterUser( byte[] _packet)
	{
		body_SC_ARENA_ENTERUSER result = new body_SC_ARENA_ENTERUSER();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_EnterUser( result);
	}

	private void Pvp_RewardInfo( byte[] _packet)
	{
		body_SC_ARENA_REWARDINFO result = new body_SC_ARENA_REWARDINFO();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_RewardInfo( result);
	}

	private void Pvp_Arena_Ready( byte[] _packet)
	{
		body_SC_ARENA_READY result = new body_SC_ARENA_READY();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Arena_Ready( result);
	}

	private void Pvp_Arena_Start( byte[] _packet)
	{
		body_SC_ARENA_START result = new body_SC_ARENA_START();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Recv_Pvp_Arena_Start( result);
	}
	#endregion

	#region -AccountGender
	private void UserGenderSetResult( byte[] _packet)
	{
		body_SC_USERGENDER_SET_RESULT result = new body_SC_USERGENDER_SET_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
			Debug.LogError( "resultCode : " + result.eResult);

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		Debug.Assert( null != userEntity);
		userEntity.UserGender = result.eGender;
		userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
		AsUserInfo.Instance.accountGender = result.eGender;
	}

	private void UserGenderNotify( byte[] _packet)
	{
		body_SC_USERGENDER_NOTIFY notify = new body_SC_USERGENDER_NOTIFY();
		notify.PacketBytesToClass( _packet);

		List<AsUserEntity> userEntityList = AsEntityManager.Instance.GetUserEntityBySessionId( notify.nSessionIdx);
		if( null == userEntityList)
			return;

		foreach( AsUserEntity userEntity in userEntityList)
		{
			userEntity.UserGender = notify.eGender;
			userEntity.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userEntity);
		}
	}
	#endregion
}
