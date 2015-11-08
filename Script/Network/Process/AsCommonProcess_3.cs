#define INTEGRATED_ARENA_MATCHING
#define INDUN_TIME
using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Text;

public class AsCommonProcess_3 : AsProcessBase
{
	public bool showLog_ = false;

	public override void Process(byte[] _packet)
	{
		PROTOCOL_CS_3 protocol = (PROTOCOL_CS_3)_packet[2];

		AsNetworkManager.Instance.ResetSendLiveTime();

		try
		{
			// 서버와의 데이터오더 문제로 3이어야 하나 2로 해야 Protocol이 얻어짐.
			switch (protocol)
			{
				case PROTOCOL_CS_3.SC_EVENT_QUEST_TIME_SYNC:
					ReceiveServerTime(_packet);
					break;

				case PROTOCOL_CS_3.SC_EVENT_QUEST_REWARD_RESULT:
					ReceiveEventRewardResult(_packet);
					break;
				
				case PROTOCOL_CS_3.SC_LEVEL_BONUS_RESULT:
					BonusManager.Instance.Recv_LevelBonus(_packet);
					break;

				case PROTOCOL_CS_3.SC_RESURRECTION_CNT:
					AsDeathDlg.SetResurrectCnt(_packet);
					break;

				case PROTOCOL_CS_3.SC_TARGET_MARK:
					ReceiveTargetMark(_packet);
					break;
				
				case PROTOCOL_CS_3.SC_CHAR_NAME_CHANGE:
					ReceiveCharcterNameChange( _packet );
					break;
				
				case PROTOCOL_CS_3.SC_CHAR_NAME_NOTIFY:
					ReceiveCharacterNameNotify( _packet );	
					break;
				
				#region - pet -
				// - init & notify & release -
//				case PROTOCOL_CS_3.SC_PET_LOAD:
//					AsPetManager.Instance.Recv_PetLoad(_packet);
//					break;
				case PROTOCOL_CS_3.SC_PET_CREATE_RESULT:
					AsPetManager.Instance.Recv_PetCreateResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_SKILL_USE:
					AsPetManager.Instance.Recv_PetSkillUse(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_LEVEL_UP:
					AsPetManager.Instance.Recv_PetLevelUp(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_LIST:
					AsPetManager.Instance.Recv_PetList(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_SLOT_CHANGE:
					AsPetManager.Instance.Recv_PetSlotChange(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_HUNGRY:
					AsPetManager.Instance.Recv_PetHungry(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_NAME_NOTIFY:
					AsPetManager.Instance.Recv_PetNameNotify(_packet);
					break;

				// - ui window -
				case PROTOCOL_CS_3.SC_PET_INFO:
					AsPetManager.Instance.Recv_PetInfo(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_CALL:
					AsPetManager.Instance.Recv_PetCall(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_CALL_RESULT:
					AsPetManager.Instance.Recv_PetCallResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_RELEASE:
					AsPetManager.Instance.Recv_PetRelease(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_RELEASE_RESULT:
					AsPetManager.Instance.Recv_PetReleaseResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_DELETE_RESULT:
					AsPetManager.Instance.Recv_PetDeleteResult(_packet);
					break;

				// - equip -
				case PROTOCOL_CS_3.SC_PET_EQUIP_ITEM_RESULT:
					AsPetManager.Instance.Recv_PetEquipItemResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_UNEQUIP_ITEM_RESULT:
					AsPetManager.Instance.Recv_PetUnEquipItemResult(_packet);
					break;

				// - act -
				case PROTOCOL_CS_3.SC_PET_NAME_CHANGE_RESULT:
					AsPetManager.Instance.Recv_PetNameChangeResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_UPGRADE_RESULT:
					AsPetManager.Instance.Recv_PetUpgradeResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_ITEM_COMPOSE_RESULT:
					AsPetManager.Instance.Recv_PetItemComposeResult(_packet);
					break;
				case PROTOCOL_CS_3.SC_PET_SLOT_EXTEND_RESULT:
					AsPetManager.Instance.Recv_PetSlotExtendResult(_packet);
					break;
				#endregion
			
				
#if INTEGRATED_ARENA_MATCHING
				#region -Integrated Pvp
				case PROTOCOL_CS_3.SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT:
					Receive_Integrated_Arena_Matching_Request_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT:
					Receive_Integrated_Arena_Matching_GoInto_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_ARENA_MATCHING_NOTIFY:
					Receive_Integrated_Arena_Matching_Notify( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_ARENA_GOGOGO:
					Receive_Integrated_Arena_Gogogo( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_LOGIN_RESULT:
					Receive_Integrated_Indun_LogIn_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_LOGOUT_RESULT:
					Receive_Integrated_Indun_LogOut_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_RETURN_WORLD_RESULT:
					Receive_Return_World_Result( _packet);
					break;
				#endregion
				
				#region -Integrated Indun
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT:
					Receive_Integrated_Indun_Matching_Info_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT:
					Receive_Integrated_Indun_Matching_Request_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT:
					Receive_Integrated_Indun_Matching_GoInto_Result( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_MATCHING_NOTIFY:
					Receive_Integrated_Indun_Matching_Notify( _packet);
					break;
				
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_GOGOGO:
					Receive_Integrated_Indun_Gogogo( _packet);
					break;
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_REWARDINFO:
					Receive_Integrated_Indun_RewardInfo( _packet);
					break;
				case PROTOCOL_CS_3.SC_INDUN_REWARD_GETITEM_RESULT:
					Receive_Integrated_Indun_GetItem_Result( _packet);
					break;
				case PROTOCOL_CS_3.SC_INDUN_QUEST_PROGRESS_INFO:
					Receive_Integrated_Indun_Quest_Progress_Info( _packet);
					break;
				case PROTOCOL_CS_3.SC_INDUN_ENTER_LIMITCOUNT:
					Receive_Integrated_Indun_Enter_LimitCount( _packet);
					break;
				case PROTOCOL_CS_3.SC_INDUN_ENTER_HELLLIMITCOUNT:
					Receive_integrated_Indun_Enter_HellLimitCount( _packet);
					break;
#if INDUN_TIME
				case PROTOCOL_CS_3.SC_INDUN_CLEARINFO:
					Receive_Integrated_Indun_ClearInfo( _packet);
					break;
#endif
				case PROTOCOL_CS_3.SC_INTEGRATED_INDUN_RE_CONNECT_FAIL:
				Recive_Integrated_Indun_Re_Connect_Fail( _packet);
					break;
				#endregion
#endif
			}

			#region -Cash Store JPN-
			switch (protocol)
			{
				case PROTOCOL_CS_3.SC_CASHSHOP_JAP_INFO_RESULT:
					RecieveCashShopInfoJpn(_packet);
					break;

				case PROTOCOL_CS_3.SC_CASHSHOP_JAP_ITEMBUY_RESULT:
					ReciveChashAndChargeResultJpn(_packet);
					break;
			}
			#endregion

            #region -Ap ranking-
            switch (protocol)
            {
                case PROTOCOL_CS_3.SC_AP_RANK_RESULT:
                    {
                        if (AsHudDlgMgr.Instance != null)
                        {
                            body_SC_AP_RANK_RESULT data = new body_SC_AP_RANK_RESULT();
                            data.PacketBytesToClass(_packet);
                            AsHudDlgMgr.Instance.OpenApRankRewardDlg(data);
                        }
                    }
                    break;
            }
            #endregion

        }
		catch (Exception e)
		{
			Debug.LogError("Protocol3 : " + protocol + " --------> exception : " + e);
		}
	}

	void ReceiveServerTime(byte[] _packet)
	{
		body_SC_EVENT_QUEST_TIME_SYNC result = new body_SC_EVENT_QUEST_TIME_SYNC();
		result.PacketBytesToClass(_packet);

		DateTime serverDate = new System.DateTime(1970, 1, 1, 9, 0, 0);
		serverDate = serverDate.AddSeconds(result.nServerSec);

		AsGameMain.serverTickGap = serverDate.Ticks - System.DateTime.Now.Ticks;

		Debug.LogWarning("server tick gap = " + AsGameMain.serverTickGap);
		Debug.LogWarning("server Date = " + serverDate);
		Debug.LogWarning("Set Client Date = " + System.DateTime.Now.AddTicks(AsGameMain.serverTickGap));
	}

	void ReceiveEventRewardResult(byte[] _packet)
	{
		body_SC_EVENT_QUEST_REWARD_RESULT result = new body_SC_EVENT_QUEST_REWARD_RESULT();
		result.PacketBytesToClass(_packet);

		if (result.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsSoundManager.Instance.PlaySound(AsSoundPath.QuestComplete, Vector3.zero, false, false);
			AsHudDlgMgr.Instance.UpdateInGameEventAchievementDlg();
		}
		else if (result.eResult == eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL)
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(811));
		}
		else
			Debug.LogWarning("event result = " + result.eResult);

	}
	
	void ReceiveTargetMark( byte[] _packet)
	{
		body_SC_TARGET_MARK mark = new body_SC_TARGET_MARK();
		mark.PacketBytesToClass( _packet);
		AsPartyManager.Instance.Recv_TargetMark( mark);
	}
	
	void ReceiveCharcterNameChange( byte[] _packet)
	{
		body_SC_CHAR_NAME_CHANGE result = new body_SC_CHAR_NAME_CHANGE();
		result.PacketBytesToClass( _packet);
		
		if( result.eResult == eRESULTCODE.eRESULT_SUCC )
		{
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey )
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1885), result.szAfterCharName);
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage( sb.ToString() );
				
				AsUserInfo.Instance.ChangeCharacterName( result.nCharUniqKey , result.szAfterCharName );
			}
			else
			{
				if( AsPartyManager.Instance.IsPartying == true )
				{
					AsPartyManager.Instance.PartyMemberNameChange( result.nCharUniqKey , result.szAfterCharName );
				}
				
				if( AsUserInfo.Instance.GuildData != null && AsUserInfo.Instance.GuildData.szGuildMaster == result.szBeforeCharName )
				{
					AsUserInfo.Instance.GuildData.szGuildMaster = result.szAfterCharName;
				}
				
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( AsTableManager.Instance.GetTbl_String(1886), result.szBeforeCharName ,result.szAfterCharName);
				AsEventNotifyMgr.Instance.CenterNotify.AddMessage( sb.ToString() );
			}
		}
		else if( result.eResult == eRESULTCODE.eRESULT_FAIL_CHARACTER_NAME_CHANGE_SYSTEM_ERROR )
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1881));
		}
		else if( result.eResult == eRESULTCODE.eRESULT_FAIL_CHARACTER_NAME_CHANGE_USE_ALREADY )
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1791));
		}
	}
	
	void ReceiveCharacterNameNotify( byte[] _packet)
	{
		body_SC_CHAR_NAME_NOTIFY result = new body_SC_CHAR_NAME_NOTIFY();
		result.PacketBytesToClass( _packet );
		
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey )
		{
			AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
			if(user != null)
			{
				user.namePanel.SetUserName( result.szCharName );
				user.SetProperty( eComponentProperty.NAME, result.szCharName );
			}
			
			if( AsUserInfo.Instance.GuildData != null && AsUserInfo.Instance.GuildData.szGuildMaster == AsUserInfo.Instance.SavedCharStat.charName_ )
			{
				AsUserInfo.Instance.GuildData.szGuildMaster = result.szCharName;
			}
			
			AsUserInfo.Instance.SavedCharStat.charName_ = result.szCharName;
			
			AsMyProperty.Instance.SetName( result.szCharName );
			
			if( AsHudDlgMgr.Instance.IsOpenReNameDlg == true )
			{
				AsReNameDlg renameDlg = AsHudDlgMgr.Instance.ReNameDlg;
				if( renameDlg.ReNameType == AsReNameDlg.eReNameType.Character )
				{
					renameDlg.Close();
				}
			}
		}
		else
		{
			AsUserEntity other = AsEntityManager.Instance.GetUserEntityByUniqueId(result.nCharUniqKey);
			if( other != null )
			{
				other.namePanel.SetUserName( result.szCharName );
				other.SetProperty( eComponentProperty.NAME, result.szCharName );
			}
			
			AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
			if( playerFsm != null )
			{
				AsUserEntity target = playerFsm.Target as AsUserEntity;
				
				if( target == other )
				{
					AsHUDController.Instance.SetTargetInfo( other );
				}
			}
		}
	}
		
#if INTEGRATED_ARENA_MATCHING
	#region -Integrated Pvp
	void Receive_Integrated_Arena_Matching_Request_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT result = new body_SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Receive_Integrated_Arena_Matching_Request_Result( result);
	}
	
	void Receive_Integrated_Arena_Matching_GoInto_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT result = new body_SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Receive_Integrated_Arena_Matching_GoInto_Result( result);
	}
	
	void Receive_Integrated_Arena_Matching_Notify( byte[] _packet)
	{
		body_SC_INTEGRATED_ARENA_MATCHING_NOTIFY result = new body_SC_INTEGRATED_ARENA_MATCHING_NOTIFY();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Receive_Integrated_Arena_Matching_Notify( result);
	}
	
	void Receive_Integrated_Arena_Gogogo( byte[] _packet)
	{
		body_SC_INTEGRATED_ARENA_GOGOGO result = new body_SC_INTEGRATED_ARENA_GOGOGO();
		result.PacketBytesToClass( _packet);

		AsPvpManager.Instance.Receive_Integrated_Arena_Gogogo( result);
	}
	
	void Receive_Integrated_Indun_LogIn_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_LOGIN_RESULT result = new body_SC_INTEGRATED_INDUN_LOGIN_RESULT();
		result.PacketBytesToClass( _packet);
		
		// map load( field -> InDun)
		body_SC_ENTER_INSTANCE_RESULT data = new body_SC_ENTER_INSTANCE_RESULT();

		data.eResult = result.eResult;
		data.nMapIndex = result.nMapID;
		data.vPosition = result.vPosition;
		
		Debug.Log( "Receive_Integrated_Indun_LogIn_Result(), result: " + result.eResult + ", Index: " + result.nIndunTableIndex);
		
		PlayerBuffMgr.Instance.Clear();
		AsUserInfo.Instance.ClearBuff();
		AsPartyManager.Instance.PartyDiceRemoveAll();
		AsPartyManager.Instance.PartyUserRemoveAll();
		AsInstanceDungeonManager.Instance.Switch_GameUserSessionIndex( result.nSessionIndex);
		AsInstanceDungeonManager.Instance.Recv_InDun_Enter( data, result.nIndunTableIndex, result.bReconnect);
	}
	
	void Receive_Integrated_Indun_LogOut_Result( byte[] _packet)
	{
		/* Not Used
		// disconnect integrated pvp server
		AsNetworkIntegratedManager.Instance.DisConnect();
		
		if( false == AsNetworkIntegratedManager.Instance.IsConnected())
		{
			body_CS_RETURN_WORLD data = new body_CS_RETURN_WORLD();
			byte[] sendData = data.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		*/
	}
	
	void Receive_Return_World_Result( byte[] _packet)
	{
		body_SC_RETURN_WORLD_RESULT result = new body_SC_RETURN_WORLD_RESULT();
		result.PacketBytesToClass( _packet);
		
		body_SC_EXIT_INSTANCE_RESULT data = new body_SC_EXIT_INSTANCE_RESULT();
		
		data.eResult = result.eResult;
		data.nMapIndex = result.nIndunTableIndex;
		data.vPosition = result.vPosition;

		// map load( InDun -> field)
		PlayerBuffMgr.Instance.Clear();
		AsUserInfo.Instance.ClearBuff();
		AsPartyManager.Instance.PartyDiceRemoveAll();
		AsPartyManager.Instance.PartyUserRemoveAll();
		if( ( AsHudDlgMgr.Instance != null) && ( AsHudDlgMgr.Instance.partyAndQuestToggleMgr != null))
			AsHudDlgMgr.Instance.partyAndQuestToggleMgr.NothingQuestProcess();
		AsInstanceDungeonManager.Instance.Switch_GameUserSessionIndex();
		AsInstanceDungeonManager.Instance.Recv_InDun_Exit( data);
	}
	#endregion

	#region -Integrated Indun
	void Receive_Integrated_Indun_Matching_Info_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT result = new body_SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Matching_Info_Result( result);
	}
	
	void Receive_Integrated_Indun_Matching_Request_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT result = new body_SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Matching_Request_Result( result);
	}
	
	void Receive_Integrated_Indun_Matching_GoInto_Result( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT result = new body_SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Matching_GoInto_Result( result);
	}
	
	void Receive_Integrated_Indun_Matching_Notify( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_MATCHING_NOTIFY result = new body_SC_INTEGRATED_INDUN_MATCHING_NOTIFY();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Matching_Notify( result);
	}
	
	void Receive_Integrated_Indun_Gogogo( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_GOGOGO result = new body_SC_INTEGRATED_INDUN_GOGOGO();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Gogogo( result);
	}
	
	void Receive_Integrated_Indun_RewardInfo( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_REWARDINFO result = new body_SC_INTEGRATED_INDUN_REWARDINFO();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_RewardInfo( result);
	}
	
	void Receive_Integrated_Indun_GetItem_Result( byte[] _packet)
	{
		body_SC_INDUN_REWARD_GETITEM_RESULT result = new body_SC_INDUN_REWARD_GETITEM_RESULT();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_GetItem_Result( result);
	}

	void Receive_Integrated_Indun_Quest_Progress_Info( byte[] _packet)
	{
		body_SC_INDUN_QUEST_PROGRESS_INFO result = new body_SC_INDUN_QUEST_PROGRESS_INFO();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Quest_Progress_Info( result);
	}

	void Receive_Integrated_Indun_Enter_LimitCount( byte[] _packet)
	{
		body_SC_INDUN_ENTER_LIMITCOUNT result = new body_SC_INDUN_ENTER_LIMITCOUNT();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Enter_LimitCount( result);
	}

	void Receive_integrated_Indun_Enter_HellLimitCount( byte[] _packet)
	{
		body_SC_INDUN_ENTER_HELLLIMITCOUNT result = new body_SC_INDUN_ENTER_HELLLIMITCOUNT();
		result.PacketBytesToClass( _packet);

		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_Enter_HellModeCount( result);
	}

#if INDUN_TIME
	void Receive_Integrated_Indun_ClearInfo( byte[] _packet)
	{
		body1_SC_INDUN_CLEARINFO result = new body1_SC_INDUN_CLEARINFO();
		result.PacketBytesToClass( _packet);
		
		AsInstanceDungeonManager.Instance.Recv_Integrated_Indun_ClearInfo( result);
	}
#endif

	void Recive_Integrated_Indun_Re_Connect_Fail( byte[] _packet)
	{
		body_SC_INTEGRATED_INDUN_RE_CONNECT_FAIL result = new body_SC_INTEGRATED_INDUN_RE_CONNECT_FAIL();
		result.PacketBytesToClass( _packet);

		AsInstanceDungeonManager.Instance.Recive_Integrated_Indun_Re_Connect_Fail( result);
	}
	#endregion
#endif

	#region -CashShop JPN-
	void RecieveCashShopInfoJpn(byte[] _packet)
	{
		body_SC_CASHSHOP_JAP_INFO_RESULT result = new body_SC_CASHSHOP_JAP_INFO_RESULT();
		result.PacketBytesToClass(_packet);

		if (AsCashStore.Instance != null)
		{
			AsCashStore.Instance.nMiracle = result.nCash;
			AsCashStore.Instance.nFreeMiracle = result.nFreeCash;
			AsCashStore.Instance.FreeGachaPoint = result.nGachaPoint;
			
			if (AsUserInfo.Instance != null)
				AsUserInfo.Instance.FreeGachaPoint = result.nGachaPoint;

			AsCashStore.Instance.UpdateFreeGachaPoint();
		}
		else
		{
			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.SetCashStoreBtnFreeMark(result.nGachaPoint == 1);
		}

		AsCommonSender.SendRequestChargeItemList();
	}

	void ReciveChashAndChargeResultJpn(byte[] _packet)
	{
		body_SC_CASHSHOP_JAP_ITEMBUY_RESULT result = new body_SC_CASHSHOP_JAP_ITEMBUY_RESULT();
		result.PacketBytesToClass(_packet);
		
		eRESULTCODE eResult = result.eResult;

		if (eResult == eRESULTCODE.eRESULT_SUCC)
		{
			if (AsCashStore.Instance != null)
				AsCashStore.Instance.CashItemBuySuccessForJpn(result);
		}
		else
		{
			string title = AsTableManager.Instance.GetTbl_String(126);
			if (eResult == eRESULTCODE.eRESULT_FAIL_SHOP_NOTCASH)
			{
				if (AsCashStore.Instance != null)
					AsNotify.Instance.MessageBox(title, AsTableManager.Instance.GetTbl_String(264), AsCashStore.Instance,
					                             "GotoMiracleMenu", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
			}
			else if (eResult == eRESULTCODE.eRESULT_FAIL_SHOP_INVENTORYFULL)
			{
				AsNotify.Instance.MessageBox(title, AsTableManager.Instance.GetTbl_String(1399), AsCashStore.Instance, "Cancel", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
			}
			else if (eResult == eRESULTCODE.eRESULT_FAIL_SHOP_MAXLIMITOVER)
			{
				AsNotify.Instance.MessageBox(title, AsTableManager.Instance.GetTbl_String(1643), AsCashStore.Instance, "Cancel", "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
			}
			else if (eResult == eRESULTCODE.eRESULT_FAIL_SHOP_NOGACHA)
			{
				Debug.LogWarning(eResult.ToString());
			}
			else
				Debug.Log(eResult);
		}
	}
	#endregion
}
