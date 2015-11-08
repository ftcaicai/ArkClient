#define _USE_PARTY_WARP
#define _STANCE
using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Text;

public class AsCommonProcess : AsProcessBase
{
	public bool showLog_ = false;

	public override void Process(byte[] _packet)
	{
		PROTOCOL_CS protocol = (PROTOCOL_CS)_packet[2];
		//bool packetProcessed = true;

		AsNetworkManager.Instance.ResetSendLiveTime();

		try
		{
			// 서버와의 데이터오더 문제로 3이어야 하나 2로 해야 Protocol이 얻어짐.
			switch( protocol)
			{
			case PROTOCOL_CS.SC_LIVE_ECHO:
				ReceiveLiveEcho();
				break;

			case PROTOCOL_CS.SC_CHAR_MOVE:
				CharMove(_packet);
				break;
			case PROTOCOL_CS.SC_OTHER_CHAR_APPEAR:
				OtherCharAppear(_packet);
				break;
			case PROTOCOL_CS.SC_OTHER_CHAR_DISAPPEAR:
				OtherCharDisappear(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_SKILL:
				CharAttackNpc(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_SKILL_EFFECT:
				CharSkillEffect(_packet);
				break;
#if _STANCE
			case PROTOCOL_CS.SC_CHAR_SKILL_STANCE:
				CharSkillStance(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_BUFF_ACCURE_INFO:
				CharBuffAccureInfo(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_SKILL_USE_ADD:
				CharSkillAdd(_packet);
				break;
#endif
				
			case PROTOCOL_CS.SC_CHAR_SKILL_SOULSTONE:
				CharSkillSoulStone(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_SKILL_PVP_AGGRO:
				CharSkillPvpAggro(_packet);
				break;	
				
			case PROTOCOL_CS.SC_NPC_APPEAR:
				NpcAppear(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_DISAPPEAR:
				NpcDisappear(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_AWAKE:
				break;
			case PROTOCOL_CS.SC_NPC_MOVE:
				NpcMove(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_SKILL:
				NpcAttackChar(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_SKILL_EFFECT:
				NpcSkillEffect(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_COMBAT_FREE:
				NpcCombatFree( _packet);
				break;
			case PROTOCOL_CS.SC_NPC_STATUS:
				NpcStatus(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_APPEAR_NOTIFY:
				NpcAppearNotify(_packet);
				break;

			case PROTOCOL_CS.SC_PICKUP_ITEM_RESULT:
				PickupItemResult(_packet);
				break;
			case PROTOCOL_CS.SC_DROPITEM_APPEAR:
				DropItemAppear(_packet);
				break;
			case PROTOCOL_CS.SC_DROPITEM_DISAPPEAR:
				DropItemDisappear(_packet);
				break;

			case PROTOCOL_CS.SC_ITEM_INVENTORY:
				ReciveItemInventory(_packet);
				break;
			case PROTOCOL_CS.SC_ITEM_INVENTORY_OPEN:
				ReceiveInventoryOpen(_packet);
				break;
			case PROTOCOL_CS.SC_ITEM_SLOT:
				ReciveItemSlot(_packet);
				break;
			case PROTOCOL_CS.SC_ITEM_RESULT:
				ReciveItemResult(_packet);
				break;

			}
			switch( protocol)
			{
			//$ yde - 20130118
			case PROTOCOL_CS.SC_STORAGE_LIST:
				ReceiveItemStorage(_packet);
				break;
			case PROTOCOL_CS.SC_STORAGE_SLOT:
				ReceiveStorageSlot(_packet);
				break;
			case PROTOCOL_CS.SC_STORAGE_MOVE_RESULT:
				ReceiveStorageMoveResult(_packet);
				break;
			case PROTOCOL_CS.SC_STORAGE_COUNT_UP_RESULT:
				ReceiveStorageCountUpResult(_packet);
				break;

			case PROTOCOL_CS.SC_QUICKSLOT_LIST:
				ReciveQuickSlotList(_packet);
				break;
			case PROTOCOL_CS.SC_ITEM_VIEW_SLOT:
				ReciveItemViewSlot(_packet);
				break;
			case PROTOCOL_CS.SC_ATTR_CHANGE:
				ReciveAttrChange(_packet);
				break;
			case PROTOCOL_CS.SC_ATTR_TOTAL:
				ReciveAttrTotal(_packet);
				break;
			case PROTOCOL_CS.SC_ATTR_HP:
				ReciveHp(_packet);
				break;
			case PROTOCOL_CS.SC_ATTR_MP:
				ReciveMP(_packet);
				break;
			case PROTOCOL_CS.SC_ATTR_EXP:
				RecieveExp( _packet);
				break;
            case PROTOCOL_CS.SC_ATTR_AP:
                RecieveAP(_packet);
                break;
			case PROTOCOL_CS.SC_ATTR_LEVELUP:
				RecieveLevelUp( _packet);
				break;
			case PROTOCOL_CS.SC_NPC_ATTR_HP:
				NpcAttr_Hp(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_ATTR_CHANGE:
				NpcAttr_Change(_packet);
				break;
			case PROTOCOL_CS.SC_GOLD_CHANGE:
				ReciveGoldChange(_packet);
				break;
			case PROTOCOL_CS.SC_SPHERE_CHANGE:
				ReciveSphereChange(_packet);
				break;
			case PROTOCOL_CS.SC_SOCIALPOINT_CHANGE:
				ReciveSocialPointChange(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_BUFF:
				AddCharBuff(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_DEBUFF:
				DelCharBuff(_packet);
				break;
			case PROTOCOL_CS.SC_CHAR_DEBUFF_RESIST:
				CharDebuffResist(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_BUFF:
				AddNpcBuff(_packet);
				break;
			case PROTOCOL_CS.SC_NPC_DEBUFF:
				DelNpcBuff(_packet);
				break;

			case PROTOCOL_CS.SC_WARP_RESULT:
				WarpResult(_packet);
				break;
			case PROTOCOL_CS.SC_WARP_XZ_RESULT:
				WarpXZResult(_packet);
				break;

			case PROTOCOL_CS.SC_GOTO_TOWN_RESULT:
				GotoTownResult( _packet);
				break;
			case PROTOCOL_CS.SC_RESURRECTION:
				ResurrectionResult( _packet);
				break;

			case PROTOCOL_CS.SC_WAYPOINT:
				ReciveWaypoint(_packet );
				break;

			case PROTOCOL_CS.SC_WAYPOINT_ACTIVE_RESULT:
				ReciveWaypointActive(_packet);
				break;

			//jump object
			case PROTOCOL_CS.SC_OBJECT_JUMP_RESULT:
				ObjectJumpResult(_packet);
				break;

			case PROTOCOL_CS.SC_OBJECT_JUMP:
				ObjectJump(_packet);
				break;

			// break object
			case PROTOCOL_CS.SC_OBJECT_BREAK_RESULT:
				ObjectBreakResult(_packet);
				break;
			case PROTOCOL_CS.SC_OBJECT_BREAK:
				ObjectBreak(_packet);
				break;

			// trap object
			case PROTOCOL_CS.SC_OBJECT_CATCH_RESULT:
				ObjectTrapResult(_packet);
				break;
			case PROTOCOL_CS.SC_OBJECT_CATCH:
				ObjectTrap(_packet);
				break;
			}

			#region -Trade
			switch( protocol)
			{
			case PROTOCOL_CS.SC_TRADE_REQUEST:
				Trade_Request( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_RESPONSE:
				Trade_Response( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_REGISTRATION_ITEM:
				Trade_Registration_Item( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_REGISTRATION_GOLD:
				Trade_Registration_Gold( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_LOCK:
				Trade_Lock( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_OK:
				Trade_Ok( _packet);
				break;
			case PROTOCOL_CS.SC_TRADE_CANCEL:
				Trade_Cancel( _packet);
				break;
			default:
				//packetProcessed = false;
				break;
			}
			#endregion -Trade

			switch( protocol)
			{
			#region -PostBox
			case PROTOCOL_CS.SC_POST_SEND_RESULT:
				PostSendResult( _packet);
				break;
			case PROTOCOL_CS.SC_POST_NEWPOST:
				PostNewPost( _packet);
				break;
			case PROTOCOL_CS.SC_POST_LIST_RESULT:
				PostListResult( _packet);
				break;
			case PROTOCOL_CS.SC_POST_ITEM_RECEIVE_RESULT:
				PostItemRecieveResult( _packet);
				break;
			case PROTOCOL_CS.SC_POST_GOLD_RECEIVE_RESULT:
				PostGoldRecieveResult( _packet);
				break;
			case PROTOCOL_CS.SC_POST_ADDRESS_BOOK:
				PostAddressBook( _packet);
				break;
			#endregion _PostBox

			#region -party
			case PROTOCOL_CS.SC_PARTY_LIST:
				PartyList(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_DETAIL_INFO:
				PartyDetailInfo(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_CREATE_RESULT:
				PartyCreateResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_CAPTAIN:
				PartyCaptain(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_SETTING_RESULT:
				PartySettingResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_SETTING_INFO:
				PartySettingInfo(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_INVITE_RESULT:
				PartyInviteResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_INVITE:
				PartyInvite(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_JOIN_RESULT:
				PartyJoinResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_JOIN_REQUEST_NOTIFY:
				PartyJoinRequestNotify(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_EXIT_RESULT:
				PartyExitResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_BANNED_EXIT_RESULT:
				PartyBannedExitResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_SEARCH_RESULT:
				PartySearchResult(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_USER_ADD:
				PartyUserAdd(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_USER_DEL:
				PartyUserDel(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_USER_INFO:
				PartyUserInfo(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_USER_BUFF:
				PartyUserBuff(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_USER_DEBUFF:
				PartyUserDeBuff(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_DICE_ITEM_INFO:
				PartyDiceItemInfo(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_DICE_SHAKE:
				PartyDiceShake(_packet);
				break;
			case PROTOCOL_CS.SC_PARTY_DICE_SHAKE_RESULT:
				PartyDiceShakeResult(_packet);
				break;

			case PROTOCOL_CS.SC_PARTY_NOTICE_ONOFF_NOTIFY:
				PartyNoticeOnOffNotify(_packet);
				break;
#if _USE_PARTY_WARP
			case PROTOCOL_CS.SC_PARTY_WARP_XZ_RESULT:
				PartyWarpXZResult(_packet);
				break;
#endif
			case PROTOCOL_CS.SC_PARTY_USER_POSITION:
				RecivePartyUserPosition(_packet);
				break;
			#endregion -party
			}

			switch( protocol)
			{
			#region -quest
			case PROTOCOL_CS.SC_QUEST_START_RESULT:
				Quest_StartResult(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_DROP_RESULT:
				Quest_DropResult(_packet);
				break;

			//case PROTOCOL_CS.SC_QUEST_GET_ITEM_RESULT:
			//	Quest_Get_Item_Result(_packet);
			//	break;

			case PROTOCOL_CS.SC_QUEST_RUNNING_LIST:
				Quest_RunningList(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_COMPLETE_LIST:
				Quest_CompleteList(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_CLEAR_RESULT:
				Quest_ClearResult(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_CLEAR_NOW_RESULT:
				Quest_ImmediatelyClearResult(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_COMPLETE_RESULT:
				Quest_CompleteResult(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_CLEAR_ENTER_MAP_RESULT:
				Quest_Clear_EnterMap_Result(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_DAILY_LIST_RESULT:
				Quest_Daily_List_Result(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_GROUP_LIST:
				Quest_Npc_Daily_List(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_DAILY_RESET_RESULT:
				Quest_Daily_List_Reset();
				break;
			case PROTOCOL_CS.SC_QUEST_FAIL_RESULT:
				Quest_Fail(_packet);
				break;
			case PROTOCOL_CS.SC_QUEST_CLEAR_TALK_RESULT:
				Quest_TalkWithNPC_Result(_packet);
				break;

			case PROTOCOL_CS.SC_QUEST_NPC_KILL_RESULT:
				Quest_KillMonster_Result(_packet);
				break;
//#if KB
			case PROTOCOL_CS.SC_QUEST_CLEAR_OPEN_UI_RESULT:
				Quest_ClearOpenUI_Result(_packet);
				break;

			case PROTOCOL_CS.SC_QUEST_INFO:
				Quest_Info(_packet);
				break;

			case PROTOCOL_CS.SC_QUEST_WARP_RESULT: 
				Quest_MoveResult(_packet);
				break;
//#endif
			#endregion
			#region -SkillReset
			case PROTOCOL_CS.SC_SKILLBOOK_LIST:
				SkillBookList( _packet);
				break;
			#endregion
			#region - skill list & shop -
			case PROTOCOL_CS.SC_SKILL_LIST:
				SkillList_Receive(_packet);
				break;
			case PROTOCOL_CS.SC_SKILL_LEARN_RESULT:
				SkillLearn_Result( _packet);
				break;
			case PROTOCOL_CS.SC_SKILL_RESET_RESULT:
				SkillReset_Result(_packet);
				break;
			#endregion

			#region -Chat
			case PROTOCOL_CS.SC_CHAT_LOCAL_RESULT:
				ChatLocalResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_PRIVATE_RESULT:
				ChatPrivateResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_PARTY_RESULT:
				ChatPartyResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_GUILD_RESULT:
				ChatGuildResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_MAP_RESULT:
				ChatMapResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_WORLD_RESULT:
				ChatWorldResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_SYSTEM_RESULT:
				ChatSystemResult( _packet);
				break;
			case PROTOCOL_CS.SC_CHAT_EMOTICON_RESULT:
				ChatEmoticonResult( _packet);
				break;
			#endregion -Chat
			}


			switch( protocol)
			{
			#region -tutorial
			case PROTOCOL_CS.SC_TUTORIAL_RESULT:
				FirstConnect();
				break;
			#endregion

			#region -npc store
			case PROTOCOL_CS.SC_SHOP_ITEMBUY:
				ItemBuyResult(_packet);
				break;
			case PROTOCOL_CS.SC_SHOP_ITEMSELL:
				ItemSellResult(_packet);
				break;
			case PROTOCOL_CS.SC_SHOP_USELESS_ITEMSELL:
				ItemLessSellResult(_packet);
				break;

			case PROTOCOL_CS.SC_SHOP_INFO:
				NpcShopInfoResult(_packet);
				break;
			#endregion

			#region -cash store
			case PROTOCOL_CS.SC_CHARGE_ITEMLIST:
				ReciveChargeItemList(_packet);
				break;

			case PROTOCOL_CS.SC_CASHSHOP_ITEMBUY:
				ReciveChashAndChargeResult(_packet, protocol);
				break;

			case PROTOCOL_CS.SC_CHARGE_BUY:
				ReciveChashAndChargeResult(_packet, protocol);
				break;

			case PROTOCOL_CS.SC_CHARGE_ANDROIDPUBLICKEY:
				ReciveAndroidPublicKey(_packet);
				break;

			case PROTOCOL_CS.SC_CHARGE_GIFT_FIND:
				ReceiveFindGiftFriend(_packet);
				break;
			#endregion

			case PROTOCOL_CS.SC_COOL_TIME_LIST:
				ReciveCoolTimeList( _packet);
				break;

			case PROTOCOL_CS.SC_WAYPOINT_LEVELACTIVE_RESULT:
				ReceiveWaypointLevelResult( _packet);
				break;

			default:
				//if(packetProcessed == false)
				//	Debug.Log( "AsCommonProcess::Process: Protocol : " + protocol + " is not processed");

				break;
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( "Protocol : " + protocol + " --------> exception : " + e);
		}
	}


    //--------------------------------------------------------------------------------------------------
    /* Create Common Packet */
    //--------------------------------------------------------------------------------------------------



	void ReceiveLiveEcho()
	{
		//Debug.Log( "ReceiveLiveEcho" );
		AsCommonSender.isSendLivePack = false;
		AsNetworkManager.Instance.ResetSendLiveTime();
	}

    //$yde
    void CharMove(byte[] _packet)
    {
 //       AsNotify.Log("CharMove");

        AS_SC_CHAR_MOVE charMove = new AS_SC_CHAR_MOVE();
        charMove.PacketBytesToClass(_packet);
        //Msg_MoveInfo moveInfo = new Msg_MoveInfo(charMove.sDestPosition);
		Msg_OtherUserMoveIndicate moveIndication =
			new Msg_OtherUserMoveIndicate(charMove);
		if( null == AsEntityManager.Instance )
		{
			Debug.LogError("null == AsEntityManager.Instance");
			return;
		}
        AsEntityManager.Instance.DispatchMessageByUniqueKey(moveIndication.charUniqKey_, moveIndication);
//		Debug.Log("Char Move = Uniqkey:" + moveIndication.charUniqKey_ + ", info:" + moveIndication.moveType_);
    }


    void OtherCharAppear(byte[] _packet)
    {
//        AsNotify.Log("OtherCharAppear");

        AS_SC_OTHER_CHAR_APPEAR_1 appear = new AS_SC_OTHER_CHAR_APPEAR_1();
        appear.PacketBytesToClass(_packet);

        AsEntityManager.Instance.OtherCharAppear(appear);
    }

    void OtherCharDisappear(byte[] _packet)
    {
//        AsNotify.Log("OtherCharDisappear");

        AS_SC_OTHER_CHAR_DISAPPEAR_1 disappear = new AS_SC_OTHER_CHAR_DISAPPEAR_1();
        disappear.PacketBytesToClass(_packet);

        AsEntityManager.Instance.OtherCharDisappear(disappear);
    }

	void CharAttackNpc(byte[] _packet)
	{
		AS_SC_CHAR_ATTACK_NPC_1 attack = new AS_SC_CHAR_ATTACK_NPC_1();
	    attack.PacketBytesToClass(_packet);
		
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == attack.nCharUniqKey )
		{
			CoolTimeGroupMgr.Instance.SetSkillUseResult( attack );
		}

		AsEntityManager.Instance.OtherCharAttack(attack);
	}

	void CharSkillEffect(byte[] _packet)
	{
		body_SC_CHAR_SKILL_EFFECT skill = new body_SC_CHAR_SKILL_EFFECT();
		skill.PacketBytesToClass(_packet);

		Msg_OtherCharSkillEffect msg = new Msg_OtherCharSkillEffect(skill);

		AsEntityManager.Instance.DispatchMessageByUniqueKey(msg.charUniqKey_, msg);
	}
	
	void CharSkillSoulStone(byte[] _packet)
	{
		body_SC_CHAR_SKILL_SOULSTONE soul = new body_SC_CHAR_SKILL_SOULSTONE();
		soul.PacketBytesToClass(_packet);
		
		AsEntityManager.Instance.MessageToPlayer( new Msg_CharSkillSoulStone( soul));
	}
	
	void CharSkillPvpAggro(byte[] _packet)
	{
		body_SC_CHAR_SKILL_PVP_AGGRO aggro = new body_SC_CHAR_SKILL_PVP_AGGRO();
		aggro.PacketBytesToClass(_packet);
		
		AsEntityManager.Instance.MessageToPlayer( new Msg_CharSkillPvpAggro(aggro));
	}
	
	void CharSkillStance(byte[] _packet)
	{
		body_SC_CHAR_SKILL_STANCE stance = new body_SC_CHAR_SKILL_STANCE();
		stance.PacketBytesToClass(_packet);
		
		AsEntityManager.Instance.DispatchMessageByUniqueKey(stance.nCharUniqKey, new Msg_OtherCharSkillStance(stance));
		
		if(stance.nCharUniqKey == AsUserInfo.Instance.GetCurrentUserEntity().UniqueId)
		{
			SkillBook.Instance.StanceChanged(stance);
		}
	}
	
	void CharBuffAccureInfo(byte[] _packet) //$yde
	{
//		Debug.Log("AsCommonProcess::CharBuffAccureInfo: pakcet receive");
		
		body_SC_CHAR_BUFF_ACCURE_INFO accure = new body_SC_CHAR_BUFF_ACCURE_INFO();
		accure.PacketBytesToClass(_packet);
		
		AsEntityManager.Instance.DispatchMessageByUniqueKey(accure.nCharUniqKey, new Msg_CharBuffAccure(accure));
	}
	
	void CharSkillAdd(byte[] _packet)
	{
		body_SC_CHAR_SKILL_USE_ADD __add = new body_SC_CHAR_SKILL_USE_ADD();
		__add.PacketBytesToClass(_packet);
		
		if(AsEntityManager.Instance.UserEntity != null)
		{
			AsEntityManager.Instance.UserEntity.HandleMessage(new Msg_CharSkillAdd(__add));
		}
	}

	void NpcAppear(byte[] _packet)
    {
//		AsNotify.Log( "NpcAppear");

        AS_SC_NPC_APPEAR_1 appear = new AS_SC_NPC_APPEAR_1();
        appear.PacketBytesToClass(_packet);

        AsEntityManager.Instance.NpcAppear(appear);
    }

    void NpcDisappear(byte[] _packet)
    {
//		AsNotify.Log( "NpcDisappear");

        AS_SC_NPC_DISAPPEAR_1 disappear = new AS_SC_NPC_DISAPPEAR_1();
        disappear.PacketBytesToClass(_packet);

        AsEntityManager.Instance.NpcDisappear(disappear);
    }

	void NpcMove(byte[] _packet)
	{
//		Debug.Log("NpcMove");

		AS_SC_NPC_MOVE move = new AS_SC_NPC_MOVE();
        move.PacketBytesToClass(_packet);
		//Msg_NpcMoveIndicate moveNpc = new Msg_NpcMoveIndicate(move.nNpcIdx, move.sDestPosition);
		Msg_NpcMoveIndicate moveNpc = new Msg_NpcMoveIndicate(move);

        AsEntityManager.Instance.DispatchMessageByNpcSessionId(moveNpc.npcSessionId_, moveNpc);
	}

	void NpcAttackChar(byte[] _packet)
	{
//		Debug.Log("NpcAttackChar");

//		try{
		AS_SC_NPC_ATTACK_CHAR_1 attack = new AS_SC_NPC_ATTACK_CHAR_1();
		attack.PacketBytesToClass(_packet);
		Msg_NpcAttackChar1 msg = new Msg_NpcAttackChar1(attack);
		AsEntityManager.Instance.DispatchMessageByNpcSessionId(msg.npcId_, msg);
//		}
//		catch
//		{
//			Debug.Log("Error while npc_attack_char packet process.");
//
//			AS_SC_NPC_ATTACK_CHAR_1 attack = new AS_SC_NPC_ATTACK_CHAR_1();
//			attack.PacketBytesToClass(_packet);
//			Msg_NpcAttackChar1 msg = new Msg_NpcAttackChar1(attack);
//			AsEntityManager.Instance.DispatchMessageByNpcSessionId(msg.npcId_, msg);
//		}

//		foreach(AS_SC_NPC_ATTACK_CHAR_2 attack2 in attack.body)
//		{
//			NpcAttackCharMessage msg = new NpcAttackCharMessage(attack2);
//			AsEntityManager.Instance.DispatchMessageByUniqueKey(msg.charUniqKey_, msg);
//		}
	}
	//~$yde

	void NpcSkillEffect(byte[] _packet)
	{
		body_SC_NPC_SKILL_EFFECT skill = new body_SC_NPC_SKILL_EFFECT();
		skill.PacketBytesToClass(_packet);

		Msg_NpcSkillEffect msg = new Msg_NpcSkillEffect(skill);

		AsEntityManager.Instance.DispatchMessageByNpcSessionId(msg.npcIdx_, msg);
	}

	void NpcCombatFree( byte[] _packet)
	{
		body_SC_NPC_COMBAT_FREE data = new body_SC_NPC_COMBAT_FREE();
		data.PacketBytesToClass( _packet);

		Msg_NpcCombatFree msg = new Msg_NpcCombatFree();
		AsEntityManager.Instance.DispatchMessageByNpcSessionId( data.nNpcIdx, msg);
	}

	void NpcStatus(byte[] _packet)
	{
		body_SC_NPC_STATUS data = new body_SC_NPC_STATUS();
		data.PacketBytesToClass(_packet);

		Msg_NpcStatus status = new Msg_NpcStatus(data);
		AsEntityManager.Instance.DispatchMessageByNpcSessionId( data.nNpcIdx, status);
	}

	void NpcAppearNotify(byte[] _packet)
	{
		body_SC_NPC_APPEAR_NOTIFY data = new body_SC_NPC_APPEAR_NOTIFY();
		data.PacketBytesToClass(_packet);

		Tbl_Npc_Record npc = AsTableManager.Instance.GetTbl_Npc_Record(data.nNpcTableIdx);
		if(npc != null)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(AsTableManager.Instance.GetTbl_String(npc.RegenString), false);
//			GameObject obj = Instantiate(Resources.Load("UI/AsGUI/MapMonsterShow")) as GameObject;
//			if(obj != null)
//			{
//				MapMonsterShow show = obj.GetComponent<MapMonsterShow>();
//				if(show != null)
//				{
//					string str = AsTableManager.Instance.GetTbl_String(npc.RegenString);
//					show.Init(str);
//				}
//			}
		}
		else
			Debug.LogError("AsCommonProcess::NpcAppearNotify: AsTableManager.Instance.GetTbl_String(npc.RegenString) = " + AsTableManager.Instance.GetTbl_String(npc.RegenString));
	}

    //--------------------------------------------------------------------------------------------------
    /* Drop Item Packet */
    //--------------------------------------------------------------------------------------------------


    /*
      * Packet Define: IC_PICKUP_ITEM_RESULT
    */
    private void PickupItemResult(byte[] _packet)
    {
        AS_body_SC_PICKUP_ITEM_RESULT result = new AS_body_SC_PICKUP_ITEM_RESULT();
        result.PacketBytesToClass(_packet);

		AsCommonSender.isSendPickupDropItem = false;

#if FULL_INVEN_UNIFIED
        if( eRESULTCODE.eRESULT_FAIL_PICKUP_IVNENTORY_FULL == result.eResult )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(10), eCHATTYPE.eCHATTYPE_SYSTEM);
            return;
		}
#else
		if( eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL == result.eResult )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(20), eCHATTYPE.eCHATTYPE_SYSTEM);
            return;
		}
#endif
		else if( eRESULTCODE.eRESULT_FAIL == result.eResult )
		{
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String(11), AsMessageManager.MSG_TYPE.ERROR);
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(11), eCHATTYPE.eCHATTYPE_SYSTEM);
            return;
		}
		else if( eRESULTCODE.eRESULT_SUCC == result.eResult )
		{
           // AchGetItem getItem = new AchGetItem();
            //getItem.ItemID = result.nDropItemIdx;
           // QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM, getItem);
			//Debug.Log("PickupItemResult [ DropItem Idx : " + result.nDropItemIdx);
		}
		else
		{
			string strError = "PickupItemResult[" + result.eResult;
			//AsMessageManager.Instance.InsertMessage( strError, AsMessageManager.MSG_TYPE.ERROR);
			Debug.Log( strError);
			return;
		}
    }


    /*
     * Packet Define: IC_DROPITEM_APPEAR
    */
    private void DropItemAppear( byte[] _packet)
    {
        AS_body1_SC_DROPITEM_APPEAR result = new AS_body1_SC_DROPITEM_APPEAR();
        result.PacketBytesToClass( _packet);

		ItemMgr.DropItemManagement.ReceiveInsertDropItems( result);
#if !UNITY_EDITOR
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( false == DropItemManagement.IsStarted)
				StartCoroutine( ItemMgr.DropItemManagement.Visualize());
		}
#endif
  	}


    /*
     * Packet Define: SC_DROPITEM_DISAPPEAR
    */
    private void DropItemDisappear(byte[] _packet)
    {
        AS_body1_SC_DROPITEM_DISAPPEAR result = new AS_body1_SC_DROPITEM_DISAPPEAR();
        result.PacketBytesToClass(_packet);

//        Debug.Log("DropItemDisappear [ count : " + result.nDropItemCnt);
        foreach (AS_body2_SC_DROPITEM_DISAPPEAR data in result.body)
        {
            ItemMgr.DropItemManagement.ReceiveRemoveDropItem(data.nDropItemIdx);
        }
    }


    /*
     * Packet Define: SC_ATTRINFO_HP
    */
    private void ReciveHp(byte[] _packet)
    {
        AS_body_SC_ATTRINFO_HP result = new AS_body_SC_ATTRINFO_HP();
        result.PacketBytesToClass(_packet);

		AsUserEntity other = AsEntityManager.Instance.GetUserEntityByUniqueId(result.nCharUniqKey);
		if( null == other)
			return;

		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
			hud = go.GetComponent<AsHUDController>();

		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if(other.UniqueId == user.UniqueId)
		{
			AsUserInfo.Instance.SavedCharStat.hpCur_ = result.fHpCur;

			if( null != hud)
			{
				int nDamage = (int)( user.GetProperty<float>( eComponentProperty.HP_CUR) - result.fHpCur);

				if( result.eContents == eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_ITEMMOVE
				|| user.GetProperty<float>( eComponentProperty.HP_MAX) >= result.fHpMax)
					nDamage = 0;

				if( nDamage > 0)
					hud.panelManager.ShowNumberPanel( user.gameObject, nDamage, eDAMAGETYPE.eDAMAGETYPE_NORMAL, false);
				else
					hud.panelManager.ShowNumberPanel( user, (int)result.fRecovery, result.eContents, AsPanelManager.eCustomFontType.eCustomFontType_HP);
			}
		}
		else
		{
			if( null != AsPartyManager.Instance.GetPartyMember( result.nCharUniqKey))
				hud.panelManager.ShowNumberPanel( other, (int)result.fRecovery, result.eContents, AsPanelManager.eCustomFontType.eCustomFontType_HP);

			// < pvp, user dot damage
			if( true == TargetDecider.CheckCurrentMapIsArena())
			{
				if( eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_DOT_DAMAGE == result.eContents)
				{
					int nDamage = (int)(result.fRecovery);
					if( nDamage < 0)
					{
						nDamage *= -1;
						if( null != AsPartyManager.Instance.GetPartyMember( result.nCharUniqKey))
							AsHUDController.Instance.panelManager.ShowNumberPanel( other.transform.gameObject, nDamage, false);
						else
							AsHUDController.Instance.panelManager.ShowNumberPanel( other.transform.gameObject, nDamage, true);
					}
				}
			}
			// pvp, user dot damage >
		}

		other.SetProperty( eComponentProperty.HP_CUR, result.fHpCur);
		if(result.fHpCur < 1)
		{

			other.HandleMessage(new Msg_DeathIndication());
			if( null != AsHudDlgMgr.Instance )
			{
				AsHudDlgMgr.Instance.DeathPlayerUIState();
			}
		}

		if( null != hud)
		{
			if( null != hud.targetInfo.getCurTargetEntity)
			{
				if( hud.targetInfo.getCurTargetEntity.GetInstanceID() == other.GetInstanceID())
					hud.targetInfo.UpdateTargetSimpleInfo( other);
			}
		}

		if(showLog_ == true)
			Debug.Log("ReciveHp amount : " + result.fHpCur + " recovery : " + result.fRecovery);
	}

    /*
     * Packet Define: SC_ATTRINFO_MP
    */
    private void ReciveMP(byte[] _packet)
    {
        AS_body_SC_ATTRINFO_MP result = new AS_body_SC_ATTRINFO_MP();
        result.PacketBytesToClass(_packet);

		AsUserEntity other = AsEntityManager.Instance.GetUserEntityByUniqueId(result.nCharUniqKey);
		if( null == other)
			return;

		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if(other.UniqueId == user.UniqueId)
		{
			AsUserInfo.Instance.SavedCharStat.mpCur_ = result.fMpCur;

			AsHUDController hud = null;
			GameObject go = AsHUDController.Instance.gameObject;
			if( null != go)
			{
				hud = go.GetComponent<AsHUDController>();
				hud.panelManager.ShowNumberPanel( user, (int)result.fRecovery, result.eContents, AsPanelManager.eCustomFontType.eCustomFontType_MP);
			}
		}

		other.SetProperty( eComponentProperty.MP_CUR, result.fMpCur);

        if(showLog_ == true) Debug.Log("ReciveMP cur : " + result.fMpCur + " recovery : " + result.fRecovery);
    }

    private void RecieveAP(byte[] _packet)
    {
        body_SC_ATTR_AP data = new body_SC_ATTR_AP();
        data.PacketBytesToClass(_packet);

        AsHUDController hud = null;
        GameObject go = AsHUDController.Instance.gameObject;

        if (null != go)
        {
            hud = go.GetComponent<AsHUDController>();
            
            if (hud != null)
                hud.panelManager.ShowNumberPanel(data.nAddApp, AsPanelManager.eCustomFontType.eCustomFontType_ArkPoint);
        }
    }

	private void RecieveExp( byte[] _packet)
	{
		body_SC_ATTR_EXP data = new body_SC_ATTR_EXP();
		data.PacketBytesToClass( _packet);

        StartCoroutine("_ExpProcess", data);
	}

	IEnumerator _ExpProcess(body_SC_ATTR_EXP data)
	{
		AsNpcEntity monster = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);

		while(true)
		{
			yield return null;

			if(monster == null)
				break;

			if(monster.GetProperty<bool>(eComponentProperty.LIVING) == false)
				break;
		}

		AsUserEntity other = AsEntityManager.Instance.GetUserEntityByUniqueId(data.nCharUniqKey);
		if(/*null == monster ||*/ null == other)
		{
			Debug.Log("AsCommonProcess::ReceiveExp: entity received exp is not found");
		}
		else
		{
			AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
			if(other.UniqueId == user.UniqueId)
			{
				AsUserInfo.Instance.SavedCharStat.totExp_ = data.nAddExp + data.nTotExp;

				AsHUDController hud = null;
				GameObject go = AsHUDController.Instance.gameObject;
				if( null != go)
				{
					hud = go.GetComponent<AsHUDController>();
					hud.panelManager.ShowNumberPanel( data.nAddExp, AsPanelManager.eCustomFontType.eCustomFontType_EXP1);
				}
			}

			other.SetProperty( eComponentProperty.LEVEL, data.nLevel);
			other.SetProperty( eComponentProperty.TOTAL_EXP, data.nTotExp);

			#region - hud -
			if( null != AsHudDlgMgr.Instance )
			{
				if( true == AsHudDlgMgr.Instance.IsOpenPlayerStatus )
					AsHudDlgMgr.Instance.playerStatusDlg.ResetPageText();
			}
			#endregion

			if(showLog_ == true) Debug.Log( "Level : " + data.nLevel + "\n" +
				"TotExp : " + data.nTotExp + "\n" +
				"AddExp : " + data.nAddExp);
		}
	}

	private void RecieveLevelUp( byte[] _packet)
	{
		body_SC_ATTR_LEVELUP data = new body_SC_ATTR_LEVELUP();
		data.PacketBytesToClass( _packet);

        // quest level up
        if( data.nSessionIdx == AsEntityManager.Instance.UserEntity.sessionId_)
        {
            AchGetLevel levelUp = new AchGetLevel( data.nLevel);
            QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_LEVEL_UP, levelUp);

			#region -GameGuide_Level
			AsGameGuideManager.Instance.CheckUp( eGameGuideType.Level, data.nLevel);
			#endregion

			if( 8 == data.nLevel)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(4123)); 
				AsHudDlgMgr.Instance.InstantDungeonBtn.gameObject.SetActive( true);
			}

			Tbl_InDun_Record		indunRecord = AsTableManager.Instance.GetInDunTable().GetRecord_SearchByMinLevel( data.nLevel );
			if( indunRecord != null )
			{
				string strMsg = string.Format( AsTableManager.Instance.GetTbl_String( 4118),AsTableManager.Instance.GetTbl_String( indunRecord.Name ) );
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);
			}
        }

		StartCoroutine( "_LevelUpProcess", data);
	}

	IEnumerator _LevelUpProcess( body_SC_ATTR_LEVELUP data)
	{
		AsNpcEntity monster = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);

		while( true)
		{
			yield return null;

			if( monster == null)
				break;

			if( monster.GetProperty<bool>( eComponentProperty.LIVING) == false)
				break;
		}

		AsUserEntity other = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey);
		if( ( null == other) || ( ( 0 != data.nNpcIdx) && ( null == monster)))
			Debug.LogWarning( "AsCommonProcess::ReceiveLevelUp: level up entity is not found");
		else
			other.HandleMessage( new Msg_Level_Up(data));
	}

	void NpcAttr_Hp(byte[] _packet)
	{
		body_SC_NPC_ATTR_HP hp = new body_SC_NPC_ATTR_HP();
		hp.PacketBytesToClass(_packet);

		AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId(hp.nNpcIdx);
		npc.SetProperty(eComponentProperty.HP_CUR, hp.fHpCur);
		if(hp.fHpCur < 1)
			npc.HandleMessage(new Msg_DeathIndication());

		#region - target compare -
		AsBaseEntity target = AsEntityManager.Instance.GetPlayerCharFsm().Target;
		if(target != null && target.FsmType == eFsmType.MONSTER)
		{
			AsNpcEntity targetNpc = target as AsNpcEntity;
			if(targetNpc.SessionId == npc.SessionId)
				AsHUDController.Instance.UpdateTargetSimpleInfo(npc);
		}
		#endregion

//		AsHUDController.Instance.UpdateTargetSimpleInfo(npc);

		if( eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_DOT_DAMAGE == hp.eContents)
		{
			int nDamage = (int)(hp.fRecovery);
			if( nDamage < 0)
			{
				nDamage *= -1;
				AsHUDController.Instance.panelManager.ShowNumberPanel( npc.gameObject, nDamage, true);
			}
		}
	}

	void NpcAttr_Change(byte[] _packet)
	{
		body_SC_NPC_ATTR_CHANGE change = new body_SC_NPC_ATTR_CHANGE();
		change.PacketBytesToClass(_packet);

		AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId(change.nNpcIdx);
		if(npc != null)
			npc.AttributeChange(change);
	}

	//--------------------------------------------------------------------------------------------------
    /* SC_GOLD_CHANGE Packet */
    //--------------------------------------------------------------------------------------------------
	private void ReciveGoldChange(byte[] _packet)
	{
		body_SC_GOLD_CHANGE result = new body_SC_GOLD_CHANGE();
        result.PacketBytesToClass(_packet);

		Debug.Log("SC_GOLD_CHANGE [ change :" + result.nChangeGold + " total : " + result.nTotalGold );

        if (ArkQuestmanager.instance.HaveGoldAchieve())
        {
			if (AsUserInfo.Instance.SavedCharStat.nGold > result.nTotalGold)
			{
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_GOLD, new AchGold(GoldAchType.GOLD_USE, AsUserInfo.Instance.SavedCharStat.nGold - result.nTotalGold));
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_GOLD, new AchGold(GoldAchType.GOLD_GET, result.nTotalGold));
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_BACK_GOLD, new AchGold(GoldAchType.GOLD_GET_BACK, result.nTotalGold));
			}
			else
			{
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_GOLD, new AchGold(GoldAchType.GOLD_GET, result.nTotalGold));
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_BACK_GOLD, new AchGold(GoldAchType.GOLD_GET_BACK, result.nTotalGold));
			}
        }

		AsUserInfo.Instance.SavedCharStat.nGold = result.nTotalGold;

		// < ilmeda 20120904
		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			hud.panelManager.ShowNumberPanel( result.nChangeGold, AsPanelManager.eCustomFontType.eCustomFontType_GOLD);
		}

        if (AsCashStore.Instance != null)
            AsCashStore.Instance.UpdateGoldText();



		// ilmeda 20120904 >
	}


	private void ReciveSphereChange(byte[] _packet)
	{
		body_SC_SPHERE_CHANGE result = new body_SC_SPHERE_CHANGE();
        result.PacketBytesToClass(_packet);

		Debug.Log("body_SC_SPHERE_CHANGE [ change :" + result.nChangeSphere + " total : " + result.nTotalSphere );

        AsUserInfo.Instance.nMiracle = result.nTotalSphere;

        if (AsHudDlgMgr.Instance != null)
            AsHudDlgMgr.Instance.cashShopBtn.GetComponent<CashShopBtnController>().SetMiracleTxt(AsUserInfo.Instance.nMiracle);

        GameObject go = AsHUDController.Instance.gameObject;
        AsHUDController hud = null;
        if (null != go)
        {
            hud = go.GetComponent<AsHUDController>();
            hud.panelManager.ShowNumberPanel(result.nChangeSphere, AsPanelManager.eCustomFontType.eCustomFontType_MIRACLE);
        }

        if (result.nChangeSphere > 0)
            AsSoundManager.Instance.PlaySound(AsSoundPath.itemBuyResultMiracle, Vector3.zero, false);

        if (AsCashStore.Instance != null)
            AsCashStore.Instance.UpdateMiracleText();
	}


	private void ReciveSocialPointChange(byte[] _packet)
	{
		body_SC_SOCIALPOINT_CHANGE result = new body_SC_SOCIALPOINT_CHANGE();
        result.PacketBytesToClass(_packet);

		// use SocialPoint
		if (AsSocialManager.Instance.SocialData.SocialPoint > result.nTotalSocialPoint)
		{
			int usePoint = AsSocialManager.Instance.SocialData.SocialPoint - result.nTotalSocialPoint;
			QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_SOCIAL_POINT, new AchUseSocialPoint(usePoint));
		}

		Debug.Log("body_SC_SOCIALPOINT_CHANGE [ change :" + result.nChangeSocialPoint + " total : " + result.nTotalSocialPoint );

       AsSocialManager.Instance.SocialData.SocialPoint =  result.nTotalSocialPoint;
	   AsSocialManager.Instance.SocialUI.SetSocialPoint( result.nTotalSocialPoint); //#19791
       QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_SOCIAL_POINT, new AchSocialPoint(result.nTotalSocialPoint));
	}
	//--------------------------------------------------------------------------------------------------
    /* WARP Packet */
    //--------------------------------------------------------------------------------------------------
	private void WarpResult(byte[] _packet)
    {
        AS_body_SC_WARP_RESULT result = new AS_body_SC_WARP_RESULT();
        result.PacketBytesToClass(_packet);

		AsCommonSender.isSendWarp = false;

		if( eRESULTCODE.eRESULT_SUCC != result.eResult )
		{
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = true;
			Debug.Log("WarpResult() [ eRESULTCODE.eRESULT_SUCC != result.eResult ] err : " + result.eResult );
			return;
		}

        Debug.Log("SC_WARP_RESULT [ map id :" + result.nMapIdx );

		//$yde
//		AsInputManager.Instance.m_Activate = true;
		AsUserInfo.Instance.SetCurrentUserCharacterPosition(result.sPosition);
		AsPromotionManager.Instance.Reserve_ZoneMoved();

		GameObject go = GameObject.Find("SceneLoader");
	    AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;

		if( TerrainMgr.Instance.GetCurMapID() != result.nMapIdx )
		{
	        sceneLoader.Load(result.nMapIdx, AsSceneLoader.eLoadType.WARP);

		}
		else
		{
			sceneLoader.LoadSameMap( result.nMapIdx, AsSceneLoader.eLoadType.WARP );
		}


		Debug.Log("AsCommonProcess:: WarpResult: scene loading started. execute [AsCommonSender.SendWarpEnd()]");

//		AsCommonSender.SendWarpEnd();
    }

	private void WarpXZResult(byte[] _packet)
	{
		body_SC_WARP_XZ_RESULT result = new body_SC_WARP_XZ_RESULT();
		result.PacketBytesToClass(_packet);

		AsCommonSender.isSendWarp = false;

		if( eRESULTCODE.eRESULT_SUCC != result.eResult )
		{
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = true;
			Debug.Log("WarpResult() [ eRESULTCODE.eRESULT_SUCC != result.eResult ] err : " + result.eResult );
			return;
		}

		AsUserInfo.Instance.SetCurrentUserCharacterPosition( result.sPosition);

		GameObject go = GameObject.Find("SceneLoader");
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;


		if( TerrainMgr.Instance.GetCurMapID() != result.nMapIdx )
		{
			sceneLoader.Load(result.nMapIdx, AsSceneLoader.eLoadType.WARPXZ);
		}
		else
		{
			sceneLoader.LoadSameMap(result.nMapIdx, AsSceneLoader.eLoadType.WARPXZ);
		}
	}

	private void GotoTownResult( byte[] _packet)
	{
		body_SC_GOTO_TOWN_RESULT result = new body_SC_GOTO_TOWN_RESULT();
		result.PacketBytesToClass( _packet);

		AsCommonSender.isSendWarp = false;

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = true;
			Debug.LogError( "SC_GOTO_TOWN_RESULT");
			return;
		}

		AsUserInfo.Instance.SetCurrentUserCharacterPosition( result.sPosition);

		GameObject go = GameObject.Find( "SceneLoader" );
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
		sceneLoader.Load( result.nMapIdx, AsSceneLoader.eLoadType.GOTO_TOWN);
//		AsCommonSender.SendGotoTownEnd();
	}

	private void ResurrectionResult( byte[] _packet)
	{
		body_SC_RESURRECTION result = new body_SC_RESURRECTION();
		result.PacketBytesToClass( _packet);

		if(result.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsEntityManager.Instance.DispatchMessageByUniqueKey( result.nCharUniqKey,
				new Msg_Resurrection( result.nResurrectCharUniqKey));
		}
		else
			Debug.LogError("AsCommonProcess::ResurrectionResult: result = " + result.eResult);
	}


	private void ReciveWaypoint( byte[] _packet)
	{
		Debug.Log("ReciveWaypoint");
		body1_SC_WAYPOINT result = new body1_SC_WAYPOINT();
		result.PacketBytesToClass( _packet);

		AsUserInfo.Instance.ReciveWaypointList( result.body );

		if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
		{
			AsHudDlgMgr.Instance.worldMapDlg.ReceiveWaypointList();
		}
	}


	private void ReciveWaypointActive( byte[] _packet)
	{
		Debug.Log("ReciveWaypointActive");

		body_SC_WAYPOINT_ACTIVE_RESULT result = new body_SC_WAYPOINT_ACTIVE_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == result.eResult )
		{
			AsUserInfo.Instance.InsertWapointList( result.nWarpTableIdx );

			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
			{
				AsHudDlgMgr.Instance.worldMapDlg.ReceiveWaypointActive();
			}

			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(803), eCHATTYPE.eCHATTYPE_SYSTEM);

		}
		else if( eRESULTCODE.eRESULT_FAIL_WAYPOINT_ALREADY_ACTIVE == result.eResult )
		{
			Debug.LogWarning("eRESULTCODE.eRESULT_FAIL_WAYPOINT_ALREADY_ACTIVE");
		}
		else if( eRESULTCODE.eRESULT_FAIL_WAYPOINT_BAD == result.eResult )
		{
			Debug.LogError("eRESULTCODE.eRESULT_FAIL_WAYPOINT_BAD");
		}
		else if( eRESULTCODE.eRESULT_FAIL_WAYPOINT_LEVEL == result.eResult )
		{
			Debug.LogError("eRESULTCODE.eRESULT_FAIL_WAYPOINT_LEVEL");
		}
	}




    //--------------------------------------------------------------------------------------------------
    /* Buff Packet */
    //--------------------------------------------------------------------------------------------------

    /*
     * Packet Define: SC_CHAR_BUFF
     * 설명 : 캐릭터 버프가 발생했을때
    */
    private void AddCharBuff(byte[] _packet)
    {
        body1_SC_CHAR_BUFF data = new body1_SC_CHAR_BUFF();
        data.PacketBytesToClass(_packet);
		
		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
		if( null != userEntity )
		{
			// broadcast buff
			userEntity.HandleMessage(new Msg_CharBuff(data));
		}

		if( AsEntityManager.Instance.UserEntity.SessionId == data.nSessionIdx )
		{
			PlayerBuffMgr.Instance.ReciveBuff( data );
		}
	}

    /*
     * Packet Define: SC_CHAR_DEBUFF
     * 설명 : 캐릭터 버프가 삭제 될때
    */
    private void DelCharBuff(byte[] _packet)
    {
        Debug.Log("DelCharBuff");

        body_SC_CHAR_DEBUFF data = new body_SC_CHAR_DEBUFF();
		data.PacketBytesToClass(_packet);

		if( AsEntityManager.Instance.UserEntity.SessionId == data.nSessionIdx )
		{
			PlayerBuffMgr.Instance.DeleteBuff( data );
		}

		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
		if( null != userEntity )
		{
//			userEntity.SetProperty(eComponentProperty.MOVE_SPEED, data.fMoveSpeed );
//			userEntity.HandleMessage(new Msg_MoveSpeedRefresh(data.fMoveSpeed));
//			Debug.Log("DelCharBuff [ buff Speed : " + data.fMoveSpeed + "char ID : " + data.nCharUniqKey );

			// delete buff
			userEntity.HandleMessage(new Msg_CharDeBuff(data));
		}
    }
	
	private void CharDebuffResist(byte[] _packet)
	{
		Debug.Log("CharDebuffResist");

        body_SC_CHAR_DEBUFF_RESIST data = new body_SC_CHAR_DEBUFF_RESIST();
		data.PacketBytesToClass(_packet);

		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
		if( null != userEntity )
		{
			userEntity.HandleMessage(new Msg_CharDeBuffResist(data));
		}
	}

    /*
     * Packet Define: SC_NPC_BUFF
     * 설명 : Npc 버프가 발생했을때
    */
    private void AddNpcBuff(byte[] _packet)
    {
//        Debug.Log("AddNpcBuff");

//        body_SC_NPC_BUFF data = new body_SC_NPC_BUFF();
		body1_SC_NPC_BUFF data = new body1_SC_NPC_BUFF();
        data.PacketBytesToClass(_packet);

		AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( data.nNpcIdx );
		if( null != npcEntity )
		{
			npcEntity.HandleMessage(new Msg_NpcBuff(data));
		}
    }

    /*
     * Packet Define: SC_NPC_DEBUFF
     * 설명 : Npc 버프가 삭제 될때
    */
    private void DelNpcBuff(byte[] _packet)
    {
        Debug.Log("DelNpcBuff");

        body_SC_NPC_DEBUFF data = new body_SC_NPC_DEBUFF();
        data.PacketBytesToClass(_packet);

		AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( data.nNpcIdx );
		if( null != npcEntity )
		{
			npcEntity.HandleMessage(new Msg_NpcDeBuff(data));
		}
    }


    //--------------------------------------------------------------------------------------------------
    /* item Packet */
    //--------------------------------------------------------------------------------------------------

    private void ReciveItemInventory(byte[] _packet)
    {
		Debug.Log("ReciveItemInventory");
        body_SC_ITEM_INVENTORY data = new body_SC_ITEM_INVENTORY();
        data.PacketBytesToClass(_packet);

		if( null == ItemMgr.HadItemManagement )
		{
			Debug.Log( "AsCommonProcess::ReciveItemInventory() [  null == ItemMgr.HadItemManagement ] " );
		}

		if( null == ItemMgr.HadItemManagement.Inven )
		{
			Debug.Log( "AsCommonProcess::ReciveItemInventory() [  null == ItemMgr.HadItemManagement.Inven ] " );
		}

		ItemMgr.HadItemManagement.Inven.SetExtendPageCount( data.nExtendPageCount );
        ItemMgr.HadItemManagement.Inven.ReceiveItemList(data.body);
    }


	private void ReceiveInventoryOpen(byte[] _packet)
    {
		Debug.Log("ReceiveInventoryOpen");
        body_SC_ITEM_INVENTORY_OPEN data = new body_SC_ITEM_INVENTORY_OPEN();
        data.PacketBytesToClass(_packet);

		AsCommonSender.isSendItemInventroyOpen = false;

		if( eRESULTCODE.eRESULT_SUCC != data.eResult )
		{
			Debug.LogError("body_SC_ITEM_INVENTORY_OPEN receive [ result : " + data.eResult );
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(327), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		ItemMgr.HadItemManagement.Inven.SetExtendPageCount( data.nExtendPageCount );

		if( AsHudDlgMgr.Instance.IsOpenInven )
		{
			AsHudDlgMgr.Instance.invenDlg.InvenOpen();
		}

		//AsNotify.Instance.MessageBox( "", AsTableManager.Instance.GetTbl_String(328), null, "", "",
		//		AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE );
    }




	private void ReciveItemSlot(byte[] _packet)
	{
		//Debug.Log("ReciveItemSlot");

        body_SC_ITEM_SLOT data = new body_SC_ITEM_SLOT();
        data.PacketBytesToClass(_packet);

        ItemMgr.HadItemManagement.Inven.ReceiveItem(data);
	}

	private void ReciveItemResult(byte[] _packet)
	{
        body_SC_ITEM_RESULT data = new body_SC_ITEM_RESULT();
        data.PacketBytesToClass(_packet);

		Debug.Log("ReciveItemResult [ result : " + data.eResult + "][ commond : " + (PROTOCOL_CS)data.nCommond );

		if( eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL == data.eResult )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(118), eCHATTYPE.eCHATTYPE_SYSTEM);
		}

		switch( (PROTOCOL_CS)data.nCommond )
		{
		case PROTOCOL_CS.CS_ITEM_USE:
			if( eRESULTCODE.eRESULT_SUCC == data.eResult )
			{
				//$yde
				AsPStoreManager.Instance.ItemUsedResult(data);

				//kij
				if( 0 != data.nItemTableIdx )
				{
					Item _item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx );
					if( null != _item )
					{
                        AsSoundManager.Instance.PlaySound(_item.ItemData.m_strUseSound, Vector3.zero, false);

						/*Tbl_Lottery_Record _randomRecord = AsTableManager.Instance.GetTbl_Lottery_Record( _item.ItemData.m_iItem_Rand_ID );
						if( null != _randomRecord && true == _randomRecord.needEffect )
						{
						}
						else
						{
							AsUserInfo.Instance.GetCurrentUserEntity().PlayUseItemEffect( _item );
						}*/

                        // get quest item
                        if (_item.ItemData.m_eItemType == Item.eITEM_TYPE.UseItem)
                        {
                            Item.eUSE_ITEM subType = (Item.eUSE_ITEM)_item.ItemData.m_iSubType;

                            if (subType == Item.eUSE_ITEM.GetQuest)
                            {
                                ArkSphereQuestTool.QuestData questData = AsTableManager.Instance.GetTbl_QuestData(_item.ItemData.m_iItem_Rand_ID);

                                if (questData != null)
                                {
                                    ArkQuest.SetQuestStrings(questData);

                                    AsHudDlgMgr.Instance.OpenQuestAcceptUI(questData, true);
                                }
                            }
							else if (subType == Item.eUSE_ITEM.InfiniteQuest || subType == Item.eUSE_ITEM.ConsumeQuest)
							{
								CoolTimeGroup coolTimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( _item.ItemData.itemSkill, _item.ItemData.itemSkillLevel );
								if( null != coolTimeGroup )
									coolTimeGroup.ActionCoolTime();
							}
                        }

						//$yde
						AsPetManager.Instance.ItemProc(_item);
					}
					else
					{
						Debug.LogError("ReciveItemResult()[ not find item [ id : " + data.nItemTableIdx );
					}
				}
			}

			AsCommonSender.isSendItemUse = false;
			break;

		case PROTOCOL_CS.CS_ITEM_MOVE:
			AsCommonSender.isSendItemMove = false;
			break;

        case PROTOCOL_CS.CS_ITEM_REMOVE:
            {
                AsCommonSender.isSendItemRemove = false;
                ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();
            }
            break;
		}

        if (eRESULTCODE.eRESULT_FAIL_ITEM_USE == data.eResult      || eRESULTCODE.eRESULT_FAIL_ITEM_USE_SKILLRESET_DEFAULT == data.eResult ||
            eRESULTCODE.eRESULT_FAIL_ITEM_USE_AREA == data.eResult || eRESULTCODE.eRESULT_FAIL_ITEM_USE_BAD_TARGET == data.eResult ||
            eRESULTCODE.eRESULT_FAIL_ITEM_USE_NOT_EXIST_TARGET == data.eResult || eRESULTCODE.eRESULT_FAIL_ITEM_USE_ALREADY_AREA == data.eResult ||
            eRESULTCODE.eRESULT_FAIL_ITEM_USE_ALREADY_TARGET == data.eResult)
        {
            String msg = string.Empty;
			
			switch( data.eResult )
			{
			case eRESULTCODE.eRESULT_FAIL_ITEM_USE:
				msg = AsTableManager.Instance.GetTbl_String(13);
				AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str13_Cannot_Use_Item);
				break;
				
			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_SKILLRESET_DEFAULT:
				msg = AsTableManager.Instance.GetTbl_String(120);
				break;

			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_AREA:
				msg = AsTableManager.Instance.GetTbl_String(924);
				break;

			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_BAD_TARGET:
				msg = AsTableManager.Instance.GetTbl_String(923);
				break;

			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_NOT_EXIST_TARGET:
				msg = AsTableManager.Instance.GetTbl_String(922);
				break;

			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_ALREADY_AREA:
				msg = AsTableManager.Instance.GetTbl_String(953);
				break;

			case eRESULTCODE.eRESULT_FAIL_ITEM_USE_ALREADY_TARGET:
				msg = AsTableManager.Instance.GetTbl_String(923);
				break;
			}
			
            AsChatManager.Instance.InsertChat(msg, eCHATTYPE.eCHATTYPE_SYSTEM);
            AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(msg, true);
        }
		else if (eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_MANY == data.eResult)
        {
            string msg = AsTableManager.Instance.GetTbl_String(945);

            AsChatManager.Instance.InsertChat(msg, eCHATTYPE.eCHATTYPE_SYSTEM);
            AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(msg, true);
        }
		else if( eRESULTCODE.eRESULT_FAIL_ARENAMATCHING_OTHER_CONTENTS == data.eResult)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 894));
		}
		else if( eRESULTCODE.eRESULT_FAIL_ARENA_OTHER_CONTENTS == data.eResult)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 903));
		}
		else if( eRESULTCODE.eRESULT_FAIL_PET_NOT_EXSIT == data.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2778),
			                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else if( eRESULTCODE.eRESULT_FAIL_PET_FULL_SLOT == data.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2737),
			                             null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else if( eRESULTCODE.eRESULT_FAIL_PET_USE_ITEM_COMBAT == data.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2738),
			                             null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else if( eRESULTCODE.eRESULT_FAIL_PET_USE_ITEM_ZONE == data.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2739),
			                             null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else if( eRESULTCODE.eRESULT_FAIL_PET_HUNGRY_FULL == data.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2217),
			                             null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		// default
        else if (eRESULTCODE.eRESULT_SUCC != data.eResult)
        {
            Debug.LogError("error ReciveItemResult()[ result : " + data.eResult);
        }

        // update quest item count
        ArkQuestmanager.instance.UpdateQuestItem();
	}

	//$yde - storage
	private void ReceiveItemStorage(byte[] _packet)
	{
		Debug.Log("ReceiveItemStorage");
		body1_SC_STORAGE_LIST data = new body1_SC_STORAGE_LIST();
        data.PacketBytesToClass(_packet);

        ItemMgr.HadItemManagement.Storage.ReceiveItemList(data);
	}

	private void ReceiveStorageSlot(byte[] _packet)
	{
		Debug.Log("ReceiveStorageSlot");

        body_SC_STORAGE_SLOT data = new body_SC_STORAGE_SLOT();
        data.PacketBytesToClass(_packet);

        ItemMgr.HadItemManagement.Storage.ReceiveItem(data);
	}

	private void ReceiveStorageMoveResult(byte[] _packet)
	{
		body_SC_STORAGE_MOVE_RESULT data = new body_SC_STORAGE_MOVE_RESULT();
        data.PacketBytesToClass(_packet);

		ItemMgr.HadItemManagement.Storage.ResultProcess(data);

		Debug.Log("ReceiveStorageMoveResult: result = " + data.eResult);
	}

	private void ReceiveStorageCountUpResult(byte[] _packet)
	{
		body_SC_STORAGE_COUNT_UP_RESULT data = new body_SC_STORAGE_COUNT_UP_RESULT();
        data.PacketBytesToClass(_packet);

		ItemMgr.HadItemManagement.Storage.ResultProcess(data);

		Debug.Log("ReceiveStorageCountUpResult: result = " + data.eResult);
	}

	private void ReciveQuickSlotList(byte[] _packet)
	{
		Debug.Log("ReciveQuickSlotList");

		body_SC_QUICKSLOT_LIST data = new body_SC_QUICKSLOT_LIST();
        data.PacketBytesToClass(_packet);

		/*sQUICKSLOT[] slots = new sQUICKSLOT[data.body.Length];
		for(int i=0; i<data.body.Length; ++i)
		{
			slots[i] = new sQUICKSLOT();
			slots[i] = data.body[i];
		}*/

		ItemMgr.HadItemManagement.QuickSlot.SetQuickSlotList(data.body);
	}

	private void ReciveItemViewSlot(byte[] _packet)
	{
		body_SC_ITEM_VIEW_SLOT data = new body_SC_ITEM_VIEW_SLOT();
		data.PacketBytesToClass(_packet);

		Debug.Log( "ReciveItemViewSlot [ char key : " + data.nCharUniqKey + " slot : " + data.nSlot + " item id : " + data.sViewItem.nItemTableIdx );

		if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState )
		{
			AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
			#region - condition -
//			AsEmotionManager.Instance.Event_Condition_GetRareItem(data);
			#endregion
			if( null != userEntity )
			{
				userEntity.ReceiveItemViewSlot( data.nSlot, data.sViewItem );
			}
			else
			{
				Debug.LogError("AsCommonProcess::ReciveItemViewSlot() [ null == userEntity ] id : " + data.nCharUniqKey);
			}
		}
	}

	private void ReciveAttrChange(byte[] _packet)
	{
		Debug.Log("ReciveAttrChange");

        body_SC_ATTR_CHANGE data = new body_SC_ATTR_CHANGE();
        data.PacketBytesToClass(_packet);

//		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
//		Debug.Log("ReciveAttrChange [ type : " + data.eChangeType + "[ value : " + data.nChangeValue );


		AsUserInfo.Instance.SaveCurCharStatChange( data );

		if( eCHANGE_INFO_TYPE.eCHANGE_INFO_HP_MAX == data.eChangeType)
		{
			AsBaseEntity baseEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey) as AsBaseEntity;
			if( null != baseEntity)
			{
				baseEntity.SetProperty( eComponentProperty.HP_MAX, data.nChangeValue);
			}
			else
			{
				baseEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( data.nSessionIdx) as AsBaseEntity;
				if( null != baseEntity)
				{
					baseEntity.SetProperty( eComponentProperty.HP_MAX, data.nChangeValue);
				}
			}
		}
	}

	private void ReciveAttrTotal(byte[] _packet)
	{
		Debug.Log("ReciveAttrTotal");

        body_SC_ATTR_TOTAL data = new body_SC_ATTR_TOTAL();
        data.PacketBytesToClass(_packet);

		AsUserInfo.Instance.SaveCurCharStatTotal( data );
	}

    //--------------------------------------------------------------------------------------------------
    /* Object Trap */
    //--------------------------------------------------------------------------------------------------

    private void ObjectTrapResult(byte[] _packet)
    {
        /*Debug.Log("SC_OBJECT_CATCH_RESULT");

        body_SC_OBJECT_CATCH_RESULT data = new body_SC_OBJECT_CATCH_RESULT();
        data.PacketBytesToClass(_packet);

        if (eRESULTCODE.eRESULT_SUCC != data.eResult)
        {
            //AsNotify.Instance.MessageBox("Object trap failed", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
            return;
        }

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx);
            return;
        }

        npcEntity.HandleMessage(new Msg_ObjTrap(null));*/
    }

    private void ObjectTrap(byte[] _packet)
    {
        /*Debug.Log("SC_OBJECT_CATCH");

        body_SC_OBJECT_CATCH data = new body_SC_OBJECT_CATCH();
        data.PacketBytesToClass(_packet);

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx);
            return;
        }

        npcEntity.HandleMessage(new Msg_ObjTrap(null));

		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( data.nCharUniqKey );
		if( null != userEntity )
		{
			userEntity.HandleMessage(new Msg_ObjTrap(null));
		}*/
    }



    //--------------------------------------------------------------------------------------------------
    /* Object break */
    //--------------------------------------------------------------------------------------------------

    private void ObjectBreakResult(byte[] _packet)
    {
        Debug.Log("SC_OBJECT_BREAK_RESULT");

        body_SC_OBJECT_BREAK_RESULT data = new body_SC_OBJECT_BREAK_RESULT();
        data.PacketBytesToClass(_packet);

        if (eRESULTCODE.eRESULT_SUCC != data.eResult)
        {
            //AsNotify.Instance.MessageBox("Object break failed", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
            return;
        }

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx );
            return;
        }

        npcEntity.HandleMessage(new Msg_ObjBreak(npcEntity.TableIdx));
    }

    private void ObjectBreak(byte[] _packet)
    {
        Debug.Log("SC_OBJECT_BREAK");

        body_SC_OBJECT_BREAK data = new body_SC_OBJECT_BREAK();
        data.PacketBytesToClass(_packet);

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx);
            return;
        }

        npcEntity.HandleMessage(new Msg_ObjBreak(npcEntity.TableIdx));
    }


    //--------------------------------------------------------------------------------------------------
    /* Object Jump */
    //--------------------------------------------------------------------------------------------------




    private void ObjectJumpResult(byte[] _packet)
    {
        body_SC_OBJECT_JUMP_RESULT data = new body_SC_OBJECT_JUMP_RESULT();
        data.PacketBytesToClass(_packet);

        if (eRESULTCODE.eRESULT_SUCC != data.eResult)
        {
            //AsNotify.Instance.MessageBox("Object jump failed", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
            return;
        }

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx);
            return;
        }

        AsEntityManager.Instance.UserEntity.linkIndex_ = npcEntity.GetProperty<int>(eComponentProperty.LINK_INDEX);
		Msg_ObjStepping msg = new Msg_ObjStepping(ObjStepping.eSTEPPIG_STATE.JUMP_START);
		msg.m_AsObjectEntity = npcEntity;
		AsEntityManager.Instance.UserEntity.HandleMessage( msg );
    }

    private void ObjectJump(byte[] _packet)
    {


        body_SC_OBJECT_JUMP data = new body_SC_OBJECT_JUMP();
        data.PacketBytesToClass(_packet);

        AsNpcEntity npcEntity = AsEntityManager.Instance.GetNpcEntityBySessionId(data.nNpcIdx);
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity [ nNpcIdx : " + data.nNpcIdx);
            return;
        }

       	AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId(data.nCharUniqKey);
		if( null == userEntity )
		{
			Debug.LogError("null == userEntity [ nCharUniqKey : " + data.nCharUniqKey);
			return;
		}

		Msg_ObjStepping msg = new Msg_ObjStepping( ObjStepping.eSTEPPIG_STATE.JUMP_START );
		msg.m_AsObjectEntity = npcEntity;
		userEntity.HandleMessage(msg);

		Debug.Log("SC_OBJECT_JUMP [ npcIdx : " + data.nNpcIdx + " nCharUniqKey : " + data.nCharUniqKey );
    }

	// < trade
	private void Trade_Request(byte[] _packet)
	{
		body_SC_TRADE_REQUEST data = new body_SC_TRADE_REQUEST();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeRequest( data.nRequestSessionIdx);
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Request Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Response(byte[] _packet)
	{
		body_SC_TRADE_RESPONSE data = new body_SC_TRADE_RESPONSE();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeResponse( true);
		else if( eRESULTCODE.eRESULT_FAIL_TRADE_REFUSE == data.eResult)
			AsTradeManager.Instance.TradeResponse( false);
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Response Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Registration_Item(byte[] _packet)
	{
		body_SC_TRADE_REGISTRATION_ITEM data = new body_SC_TRADE_REGISTRATION_ITEM();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeRegistrationItem( data.nSessionIdx, data.bAddOrDel, data.nInvenSlot, data.nTradeSlot, data.sTradeItem);
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Registration_Item Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Registration_Gold(byte[] _packet)
	{
		body_SC_TRADE_REGISTRATION_GOLD data = new body_SC_TRADE_REGISTRATION_GOLD();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeRegistrationGold( data.nSessionIdx, data.nTotalGold);
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Registration_Gold Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Lock(byte[] _packet)
	{
		body_SC_TRADE_LOCK data = new body_SC_TRADE_LOCK();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeLock( data.nSessionIdx, data.bLock);
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Lock Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Ok(byte[] _packet)
	{
		body_SC_TRADE_OK data = new body_SC_TRADE_OK();
		data.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC == data.eResult)
			AsTradeManager.Instance.TradeOk();
		else if( eRESULTCODE.eRESULT_FAIL == data.eResult)
			Debug.Log( "Trade_Ok Failed: " + data.eResult);
		else
			Trade_PrintErrSystemMsg( data.eResult);
	}

	private void Trade_Cancel(byte[] _packet)
	{
		AsTradeManager.Instance.TradeCancel();
	}

	#region -PostBox
	private void PostSendResult( byte[] _packet)
	{
		body_SC_POST_SEND_RESULT result = new body_SC_POST_SEND_RESULT();
		result.PacketBytesToClass( _packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(110), AsTableManager.Instance.GetTbl_String(111),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		case eRESULTCODE.eRESULT_FAIL_POST_SEND_BLOCKOUT:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(300), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsHudDlgMgr.Instance.ClosePostBoxDlg();
			break;
		case eRESULTCODE.eRESULT_FAIL_POST_RECEIVER:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 109), AsTableManager.Instance.GetTbl_String( 92),
				null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		default:

            ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
			if( true == AsHudDlgMgr.Instance.IsOpenInven)
			{
				AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
			}
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(109), AsTableManager.Instance.GetTbl_String(1441), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			//AsHudDlgMgr.Instance.ClosePostBoxDlg();
			break;
		}
	}

	private void PostNewPost( byte[] _packet)
	{
		body_SC_POST_NEWPOST newPost = new body_SC_POST_NEWPOST();
		newPost.PacketBytesToClass( _packet);

		AsUserInfo.Instance.NewMail = newPost.bHave;

		if( null != AsHudDlgMgr.Instance )
		{
			AsHudDlgMgr.Instance.CheckNewMenuImg();
			AsHudDlgMgr.Instance.SetNewPostImg(AsUserInfo.Instance.NewMail);
		}
	}

	private void PostListResult( byte[] _packet)
	{
		body1_SC_POST_LIST_RESULT result = new body1_SC_POST_LIST_RESULT();
		result.PacketBytesToClass( _packet);

		if( null == AsHudDlgMgr.Instance.postBoxDlgObj)
			return;

		AsPostBoxDlg postBoxDlg = AsHudDlgMgr.Instance.postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
		Debug.Assert( null != postBoxDlg);

		postBoxDlg.ClearMailList();
		
		postBoxDlg.InsertMailInfo( result.body);

		AsCommonSender.isSendPostList = false;
	}

	private void PostItemRecieveResult( byte[] _packet)
	{
		body_SC_POST_ITEM_RECIEVE_RESULT result = new body_SC_POST_ITEM_RECIEVE_RESULT();
		result.PacketBytesToClass( _packet);
		
		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1576), AsTableManager.Instance.GetTbl_String(105), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(39), AsTableManager.Instance.GetTbl_String(103), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		default:
			Debug.LogError( "Post item recieve failed!!");
			return;
		}

		if( null == AsHudDlgMgr.Instance.postBoxDlgObj)
			return;

		AsPostBoxDlg postBoxDlg = AsHudDlgMgr.Instance.postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
		Debug.Assert( null != postBoxDlg);
		postBoxDlg.RequestNextPageMailList();
	}

	private void PostGoldRecieveResult( byte[] _packet)
	{
		body_SC_POST_GOLD_RECIEVE_RESULT result = new body_SC_POST_GOLD_RECIEVE_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
			Debug.LogError( "Post gold recieve failed!!");
	}

	private void PostAddressBook( byte[] _packet)
	{
		body1_SC_POST_ADDRESS_BOOK addressBook = new body1_SC_POST_ADDRESS_BOOK();
		addressBook.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != addressBook.eResult)
		{
			Debug.LogWarning( "PostAddressBook : " + addressBook.eResult);
			AsHudDlgMgr.Instance.postBoxDlg.ClearAddress();
			return;
		}

		AsHudDlgMgr.Instance.postBoxDlg.InsertAddress( addressBook);
	}
	#endregion

	private void Trade_PrintErrSystemMsg(eRESULTCODE eResult)
	{
		switch( eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_TRADE_NOT:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(62), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_BADUSER:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(25), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_REFUSE:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(63), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_GAP:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(32), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_IMPOSSIBLE_ITEM:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(29), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_ALREADY_ITEM:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(64), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_EMPTY_ITEM:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(65), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_OVERGOLD:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(66), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_NOTREADY:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(67), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_INVENTORYFULL:
			AsHudDlgMgr.Instance.OpenTradeErrorMsgBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(31));
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_OTHERNOTOK:
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(68));
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_LOCK:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(69), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_TRADE_ALREADY_CANCEL:
			AsHudDlgMgr.Instance.OpenTradeErrorMsgBox( AsTableManager.Instance.GetTbl_String(1106), AsTableManager.Instance.GetTbl_String(26), true);
			break;
		}
	}

	#region -party-
	void PartyList(byte[] _packet)
    {
		#if _PARTY_LOG_
		Debug.Log( "PartyList");
		#endif
        AS_SC_PARTY_LIST partyList = new AS_SC_PARTY_LIST();
        partyList.PacketBytesToClass(_packet);

        AsPartyManager.Instance.ReceivePartyList(partyList.body);
    }

	void PartyDetailInfo(byte[] _packet)
    {
		AS_SC_PARTY_DETAIL_INFO partyDetailInfo = new AS_SC_PARTY_DETAIL_INFO();
        partyDetailInfo.PacketBytesToClass(_packet);

		#if _PARTY_LOG_
		Debug.Log( "PartyDetailInfo");
		Debug.Log( "PartyOptionInfo" + partyDetailInfo.sOption.bPublic.ToString() + partyDetailInfo.sOption.ePurpose.ToString() + "Divide:"+ partyDetailInfo.sOption.eDivide.ToString() + "eGrade" + partyDetailInfo.sOption.eGrade.ToString()  );

		#endif

		if( eRESULTCODE.eRESULT_SUCC != partyDetailInfo.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(390), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}
        AsPartyManager.Instance.ReceivePartyDetailInfo(partyDetailInfo);
	}

	private void PartyCreateResult( byte[] _packet)
	{
		AS_SC_PARTY_CREATE_RESULT result = new AS_SC_PARTY_CREATE_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(391), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			AsPartyManager.Instance.IsPartying = false;
			return;
		}
		AsPartyManager.Instance.PartyIdx   = result.nPartyIdx;
		AsPartyManager.Instance.IsPartying = true;


		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;
		//2013.03.04
		AsPartyManager.Instance.PartyMyAdd();
		AsPartyManager.Instance.SetCaptain(userEntity.UniqueId);


		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(877), eCHATTYPE.eCHATTYPE_SYSTEM);
		#if _PARTY_LOG_
		Debug.Log( "PartyCreateResult nPartyIdx : " + result.nPartyIdx);
		#endif
	}

	private void PartyCaptain( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyCaptain");
		#endif
		AS_SC_PARTY_CAPTAIN result = new AS_SC_PARTY_CAPTAIN();
		result.PacketBytesToClass( _packet);

		AsPartyManager.Instance.SetCaptain(result.nCharUniqKey);
		AsPartyManager.Instance.SetCaptainCenterNotify(result.nCharUniqKey);
	}
	
	int nDivide, nGrade;
	private void PartySettingResult( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartySettingResult");
		#endif
		AS_SC_PARTY_SETTING_RESULT result = new AS_SC_PARTY_SETTING_RESULT();
		result.PacketBytesToClass( _packet);

		if( eRESULTCODE.eRESULT_SUCC != result.eResult)
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(392), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}
		else
		{
			if(AsPartyManager.Instance.IsDivideItemChange)
			{
				switch(AsPartyManager.Instance.PartyOption.eDivide)
				{
					case 1:	nDivide = 1127; break;
					case 2:	nDivide = 1126; break;
					case 3:	nDivide = 1250; break;
				}
				
				string	strMsg = string.Format( AsTableManager.Instance.GetTbl_String(48), AsTableManager.Instance.GetTbl_String(nDivide));
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);//##22832.
				AsPartyManager.Instance.IsDivideItemChange = false;
			}
		}
	}
	
	private void PartySettingInfo( byte[] _packet)
	{
		AS_SC_PARTY_SETTING_INFO settingInfo = new AS_SC_PARTY_SETTING_INFO();
		settingInfo.PacketBytesToClass( _packet);
		#if _PARTY_LOG_
		Debug.Log( "PartySettingInfo" + settingInfo.sOption.bPublic.ToString() + settingInfo.sOption.ePurpose.ToString() + "Divide:"+ settingInfo.sOption.eDivide.ToString() + "eGrade" + settingInfo.sOption.eGrade.ToString()  );
		#endif

		bool bChange = false;
		if(AsPartyManager.Instance.PartyOption.eDivide  != settingInfo.sOption.eDivide)
		{
			bChange = true;
		}

		if(AsPartyManager.Instance.PartyOption.eGrade  != settingInfo.sOption.eGrade)
		{
			bChange = true;
		}

		AsPartyManager.Instance.PartyMapIdx = settingInfo.nMapIdx;
		AsPartyManager.Instance.PartyOption = settingInfo.sOption;

		switch(settingInfo.sOption.eDivide)
		{
			case 1:	nDivide = 1127; break;
			case 2:	nDivide = 1126; break;
			case 3:	nDivide = 1250; break;
		}

		switch(settingInfo.sOption.eGrade)
		{
			case 1:	nGrade = 1131; break;
			case 2:	nGrade = 1130; break;
			case 3:	nGrade = 1129; break;
			case 4:	nGrade = 1128; break;
		}
		if(bChange && !AsPartyManager.Instance.IsCaptain)
		{
			string	strMsg = string.Format( AsTableManager.Instance.GetTbl_String(48), AsTableManager.Instance.GetTbl_String(nDivide)/*, AsTableManager.Instance.GetTbl_String(nGrade)*/ );
	//		AsMessageManager.Instance.InsertMessage( strMsg );
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( strMsg);//##22832.
		}

		AsPartyManager.Instance.AutoDelPartyNotice();

	}

	private void PartyInviteResult( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyInviteResult");
		#endif
		AS_SC_PARTY_INVITE_RESULT result = new AS_SC_PARTY_INVITE_RESULT();
		result.PacketBytesToClass( _packet);
		string strMsg;
		string userName = AsUtil.GetRealString(System.Text.UTF8Encoding.UTF8.GetString( result.szCharName ));

		switch(result.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_PARTY_INVITE_ALREADY:
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String(44));
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(44), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;

		case eRESULTCODE.eRESULT_FAIL_PARTY_INVITE_REFUSE:
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(46), userName );
//			AsMessageManager.Instance.InsertMessage( strMsg );
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			break;

		case eRESULTCODE.eRESULT_FAIL_PARTY_INVITE_PLAYING:
			strMsg = string.Format( AsTableManager.Instance.GetTbl_String(47), userName );
//			AsMessageManager.Instance.InsertMessage( strMsg );
			AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_INVITE_BLOCKOUT:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(183), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_TUTORIALZONE:
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(423), null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);		
			break;	
			
		}
	}

	private void PartyInvite( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyInvite");
		#endif
		
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_PartyInviteRefuse))
			return;
		
		AS_SC_PARTY_INVITE invite = new AS_SC_PARTY_INVITE();
		invite.PacketBytesToClass( _packet);

		AsPartyManager.Instance.PartyInvite(invite);

	}

	private void PartyJoinResult( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyJoinResult");
		#endif
		AS_SC_PARTY_JOIN_RESULT joinResult = new AS_SC_PARTY_JOIN_RESULT();
		joinResult.PacketBytesToClass( _packet);

		switch(joinResult.eResult)
		{
		case  eRESULTCODE.eRESULT_FAIL_PARTY_JOIN:
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(393), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_JOIN_FULL :
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(54), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_JOIN_CLOSED :
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(955), eCHATTYPE.eCHATTYPE_SYSTEM);//#20314.
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_JOIN_BLOCKOUT :
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(184), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		case eRESULTCODE.eRESULT_FAIL_PARTY_NOT_EXIST:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(55), eCHATTYPE.eCHATTYPE_SYSTEM);
			break;
		}

		AsPartyManager.Instance.IsPartyJoin = false;

		if( eRESULTCODE.eRESULT_SUCC == joinResult.eResult)
		{
			AsPartyManager.Instance.PartyIdx   = joinResult.nPartyIdx;
			AsPartyManager.Instance.IsPartying = true;
		}
		else
		{
			AsPartyManager.Instance.IsPartying = false;
		}

	}

	private void PartyJoinRequestNotify( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyJoinRequestNotify");
		#endif
		body_SC_PARTY_JOIN_REQUEST_NOTIFY joinRequestNotify = new body_SC_PARTY_JOIN_REQUEST_NOTIFY();
		joinRequestNotify.PacketBytesToClass( _packet);

		AsPartyManager.Instance.SetJoinRequestNotify(joinRequestNotify);

	}


	private void PartyExitResult( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyExitResult");
		#endif
		AS_SC_PARTY_EXIT_RESULT exitResult = new AS_SC_PARTY_EXIT_RESULT();
		exitResult.PacketBytesToClass( _packet);

		AsPartyManager.Instance.PartyDiceRemoveAll();
		AsPartyManager.Instance.PartyUserRemoveAll();

	}




	private void PartyBannedExitResult( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log( "PartyBannedExitResult");
		#endif
		AS_SC_PARTY_BANNED_EXIT_RESULT bannedExitResult = new AS_SC_PARTY_BANNED_EXIT_RESULT();
		bannedExitResult.PacketBytesToClass( _packet);

	}
	private void PartySearchResult( byte[] _packet)
	{
		body_SC_PARTY_SEARCH_RESULT seartResult = new body_SC_PARTY_SEARCH_RESULT();
		seartResult.PacketBytesToClass(_packet);
		
		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		
		switch(seartResult.eResult)
		{
		case  eRESULTCODE.eRESULT_SUCC:
			message =  AsTableManager.Instance.GetTbl_String(2030);
			break;
		case  eRESULTCODE.eRESULT_FAIL_PARTY_SEARCH:
			message =  AsTableManager.Instance.GetTbl_String(2007);
			break;
		default:
			message = seartResult.eResult.ToString();
			break;				
		}
		
		if( message != string.Empty)
			AsPartyManager.Instance.MsgBox_PartySearch =
				AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}

	private void PartyUserAdd( byte[] _packet)
	{

		AS_SC_PARTY_USER_ADD userAdd = new AS_SC_PARTY_USER_ADD();
		userAdd.PacketBytesToClass( _packet);

		#if _PARTY_LOG_
		Debug.Log( "PartyUserAdd");
		Debug.Log( userAdd.nCharUniqKey.ToString() + "PartyUserAdd" + userAdd.nPartyIdx.ToString() );
		#endif
		AsPartyManager.Instance.PartyUserAdd(userAdd);


		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			if(userAdd.nCharUniqKey == userEntity.UniqueId)
				AsPartySender.SendPartyDetailInfo(userAdd.nPartyIdx);
			else
			{
				string userName = System.Text.UTF8Encoding.UTF8.GetString( userAdd.szCharName );
				string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(45),  AsUtil.GetRealString(userName)  );
				AsChatManager.Instance.InsertChat( strMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			}
		}

	}

	private void PartyUserDel( byte[] _packet)
	{

		AS_SC_PARTY_USER_DEL userDel = new AS_SC_PARTY_USER_DEL();
		userDel.PacketBytesToClass( _packet);
		AsPartyManager.Instance.PartyUserDel(userDel);
		#if _PARTY_LOG_
		Debug.Log( "PartyUserDel" + userDel.nCharUniqKey.ToString() );
		#endif
	}
	private void PartyUserInfo( byte[] _packet)
	{

		AS_SC_PARTY_USER_INFO userInfo = new AS_SC_PARTY_USER_INFO();
		userInfo.PacketBytesToClass( _packet);
		AsPartyManager.Instance.PartyUserInfo(userInfo);
		#if _PARTY_LOG_
	 	Debug.Log( "PartyUserInfo CharUniqKey : " + userInfo.nCharUniqKey + "HpCur"+(int)userInfo.fHpCur);
		#endif
	}

	private void PartyUserBuff( byte[] _packet)
	{
	//	#if _PARTY_LOG_
		Debug.Log( "PartyUserBuff");
	//	#endif
		AS_SC_PARTY_USER_BUFF userBuff = new AS_SC_PARTY_USER_BUFF();
		userBuff.PacketBytesToClass( _packet);
		AsPartyManager.Instance.PartyUserBuff(userBuff);
	}
	private void PartyUserDeBuff( byte[] _packet)
	{
	//	#if _PARTY_LOG_
		Debug.Log( "PartyUserDeBuff");
	//	#endif
		AS_SC_PARTY_USER_DEBUFF userDeBuff = new AS_SC_PARTY_USER_DEBUFF();
		userDeBuff.PacketBytesToClass( _packet);

		body_SC_CHAR_DEBUFF charDeBuff = new body_SC_CHAR_DEBUFF();
		charDeBuff.nCharUniqKey =  userDeBuff.nCharUniqKey;
		charDeBuff.nSkillTableIdx =  userDeBuff.nSkillTableIdx;
		charDeBuff.nSkillLevel =  userDeBuff.nSkillLevel;
		charDeBuff.nChargeStep =  userDeBuff.nChargeStep;
		charDeBuff.nPotencyIdx =  userDeBuff.nPotencyIdx;
		charDeBuff.eType =  userDeBuff.eType;

		AsPartyManager.Instance.PartyUserDeBuff(charDeBuff);

	}

	private void  PartyDiceItemInfo( byte[] _packet)
	{

		AS_SC_PARTY_DICE_ITEM_INFO  DiceItemInfo = new AS_SC_PARTY_DICE_ITEM_INFO();
		DiceItemInfo.PacketBytesToClass(_packet);
		AsPartyManager.Instance.PartyDiceItemInfo(DiceItemInfo);
	//	#if _PARTY_LOG_
		Debug.Log("PartyDiceItemInfo DropItemIdx:" + DiceItemInfo.nDropItemIdx);
	//	#endif
	}

	private void  PartyDiceShake( byte[] _packet)
	{
		#if _PARTY_LOG_
		Debug.Log("PartyDiceShake");
		#endif
		AS_SC_PARTY_DICE_SHAKE  DiceShake = new AS_SC_PARTY_DICE_SHAKE();
		DiceShake.PacketBytesToClass(_packet);
		AsPartyManager.Instance.PartyDiceShake(DiceShake);
	}

	private void  PartyDiceShakeResult( byte[] _packet)
	{
	//	#if _PARTY_LOG_
		Debug.Log("PartyDiceShakeResult");
	//	#endif
		AS_SC_PARTY_DICE_SHAKE_RESULT  DiceShakeResult = new AS_SC_PARTY_DICE_SHAKE_RESULT();
		DiceShakeResult.PacketBytesToClass(_packet);
		AsPartyManager.Instance.PartyDiceShakeResult(DiceShakeResult);
	}


	private void PartyNoticeOnOffNotify( byte[] _packet)
	{
		body_SC_PARTY_NOTICE_ONOFF_NOTIFY  data = new body_SC_PARTY_NOTICE_ONOFF_NOTIFY();
		data.PacketBytesToClass(_packet);
		Debug.Log("PartyNoticeOnOffNotify[ " + data.onOff + " [ str : " + data.szPartyNotice );
		//유저머리 위에 파티광고판 설정..
		AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId(data.nCharUniqKey);
		if( null != userEntity)
		{
			userEntity.PartyIdx = data.nPartyIdx;
			userEntity.ShowPartyPR(data.onOff, data.szPartyNotice);
			if(data.nCharUniqKey == AsUserInfo.Instance.GetCurrentUserEntity().UniqueId)
			{
				AsPartyManager.Instance.IsPartyNotice = data.onOff;
				AsPartyManager.Instance.PartyNotice = data.szPartyNotice;
			}
		}
	}
	
	private void PartyWarpXZResult( byte[] _packet)
	{
#if _USE_PARTY_WARP
		body_SC_PARTY_WARP_XZ_RESULT  result = new body_SC_PARTY_WARP_XZ_RESULT();
		result.PacketBytesToClass(_packet);
		Debug.Log("PartyWarpXZResult nMapIdx : " + result.nMapIdx  + result.sPosition);		
	
		AsCommonSender.isSendWarp = false;
		
		string message = string.Empty;
		string title = AsTableManager.Instance.GetTbl_String(126);
		
		switch(result.eResult)
		{
		case  eRESULTCODE.eRESULT_SUCC:		
			AsUserInfo.Instance.SetCurrentUserCharacterPosition( result.sPosition);
			GameObject go = GameObject.Find("SceneLoader");
			AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
	
	
			if( TerrainMgr.Instance.GetCurMapID() != result.nMapIdx )
			{
				sceneLoader.Load( result.nMapIdx, AsSceneLoader.eLoadType.PARTY_WARPXZ);
			}
			else
			{
				sceneLoader.LoadSameMap( result.nMapIdx, AsSceneLoader.eLoadType.PARTY_WARPXZ);
			}
			break;	
		case eRESULTCODE.eRESULT_FAIL:	
		case eRESULTCODE.eRESULT_FAIL_WARP:
			message	= AsTableManager.Instance.GetTbl_String(941);//#21459
			break;
		case  eRESULTCODE.eRESULT_FAIL_WAYPOINT_BAD_GOLD:
			message	= AsTableManager.Instance.GetTbl_String(252);//#21463
			break;
		case  eRESULTCODE.eRESULT_FAIL_PARTY_NOT_EXIST:
			message	= string.Empty;//파티 없음/ 강퇴당했다는 메시지가 뜨기 때문에 팝업 없음...
			break;	
		case  eRESULTCODE.eRESULT_FAIL_PARTY_WARP_CAPTAIN_MYSELF:
			message	= string.Empty;//본인이 파장/ 본인이 파티장으로 위임됐다는 메시지가 있기 때문에 팝업없음.
			break;	
		case  eRESULTCODE.eRESULT_FAIL_PARTY_WARP_CHANNEL_FULL:
			message	= AsTableManager.Instance.GetTbl_String(831);//#21466
			break;		
		default:
			message = result.eResult.ToString();
			break;				
		}
		
		if( message != string.Empty)		
				AsNotify.Instance.MessageBox( title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
#endif		
	}
			
	private void RecivePartyUserPosition( byte[] _packet)
	{
		body1_SC_PARTY_USER_POSITION _result = new body1_SC_PARTY_USER_POSITION();
		_result.PacketBytesToClass(_packet);

		AsPartyManager.Instance.RecivePartyPositionInfo( _result );
		if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
		{
			AsHudDlgMgr.Instance.worldMapDlg.RecivePartyPositionInfo( _result );
		}
		#if _PARTY_LOG_
		Debug.Log("RecivePartyUserPosition [ count : " + _result.nCharCnt );
		#endif
	}


	#endregion  -party-



    #region -quest-
    private void Quest_StartResult(byte[] _packet)
    {
		Debug.LogWarning("Start Result.........");

        body_CS_QUEST_START_RESULT data = new body_CS_QUEST_START_RESULT();
        data.PacketBytesToClass(_packet);

        if (data.eResult == eRESULTCODE.eRESULT_SUCC)
            ArkQuestmanager.instance.AcceptQuest(data.nQuestTableIdx, false);
        else if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_USE_ITEM)
            AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(812));
        else if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_NEED_INVENTORYSLOT)
            AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(882));
        else if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_TIMELIMIT)
            AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(142));
        else if (AsHudDlgMgr.Instance.IsOpenQuestAccept == true)
        {
            if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_FULL)
                AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(841), this, "AfterProcessQuestFull", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
            else
                Debug.LogWarning("Quest Accept Result = " + data.eResult);
        }
        else
            Debug.LogWarning("Quest Accept Result = " + data.eResult);
    }

    private void AfterProcessQuestFull()
    {
        AsHudDlgMgr.Instance.CloseQuestAccept();
        UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_SHOW);
        UIMessageBroadcaster.instance.SendUIMessage(UIMessageType.UI_MESSAGE_QUESTLIST_UPDATE);
    }

    private void Quest_Get_Item_Result(byte[] _packet)
    {
        //body_SC_QUEST_GET_ITEM_RESULT data = new body_SC_QUEST_GET_ITEM_RESULT();
        //data.PacketBytesToClass(_packet);
        //ArkQuestmanager.instance.ChangeQuestItemCount(data.nItemTableIdx, data.nItemOverlapped);
    }

    private void Quest_DropResult(byte[] _packet)
    {
        body_CS_QUEST_DROP_RESULT data = new body_CS_QUEST_DROP_RESULT();
        data.PacketBytesToClass(_packet);

        if (data.eResult == eRESULTCODE.eRESULT_SUCC)
            ArkQuestmanager.instance.DropQuest(data.nQuestTableIdx);
        else
            Debug.LogWarning("Quest Drop Result = " + data.eResult);
    }

    private void Quest_ClearResult(byte[] _packet)
    {
        body_CS_QUEST_CLEAR_RESULT data = new body_CS_QUEST_CLEAR_RESULT();
        data.PacketBytesToClass(_packet);

        if (data.eResult == eRESULTCODE.eRESULT_SUCC)
            ArkQuestmanager.instance.ClearQuest(data.nQuestTableIdx);
        else
        {
			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.UpdateQuestBookBtn();
        }

        Debug.LogWarning("Quest Clear Result = " + data.eResult);
    }

    private void Quest_ImmediatelyClearResult(byte[] _packet)
    {
        body_SC_QUEST_CLEAR_NOW_RESULT data = new body_SC_QUEST_CLEAR_NOW_RESULT();
        data.PacketBytesToClass(_packet);

        AsLoadingIndigator.Instance.HideIndigator();

        if (AsHudDlgMgr.Instance.IsOpenQuestAccept)
            AsHudDlgMgr.Instance.questAccept.LockInput = false;

        if (data.eResult == eRESULTCODE.eRESULT_SUCC)
        {
            ArkQuestmanager.instance.ImmediatelyClearQuest(data.nQuestTableIdx);

			if (AsHudDlgMgr.Instance != null)
				if (AsHudDlgMgr.Instance.IsOpenQuestAccept == true)
					AsHudDlgMgr.Instance.questAccept.UpdateCashDone();
        }
        else
        {
            if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_CLEAR_CASH)
            {
                if (AsGameMain.useCashShop == true)
					AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), AsHudDlgMgr.Instance, "NotEnoughMiracleProcessInGame", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
                else
                    AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), AsNotify.MSG_BOX_TYPE.MBT_OK);
            }
            else if (data.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_CLEAR_GOLD)
                AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(137));
            else
                Debug.LogWarning("Quest Accept Result = " + data.eResult);
        }
    }

    private void Quest_CompleteResult(byte[] _packet)
    {
        body_CS_QUEST_COMPLETE_RESULT data = new body_CS_QUEST_COMPLETE_RESULT();
        data.PacketBytesToClass(_packet);

        if (data.eResult == eRESULTCODE.eRESULT_SUCC)
            ArkQuestmanager.instance.CompleteQuest(data.nQuestTableIdx);
        else
        {
            if (data.eResult == eRESULTCODE.eRESULT_FAIL_REWARD_EMPTYITEMSLOT)
                AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(811));
            else if (data.eResult == eRESULTCODE.eRESULT_FAIL_REWARD_NO_CONDITION)
                AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(930));

            Debug.LogWarning("Quest Complete Result = " + data.eResult);
        }
    }

    private void Quest_Clear_EnterMap_Result(byte[] _packet)
    {
        body_SC_QUEST_CLEAR_ENTER_MAP_RESULT data = new body_SC_QUEST_CLEAR_ENTER_MAP_RESULT();

        Debug.Log("Clear enter map");

        data.PacketBytesToClass(_packet);
        AchMapInto achMapInto = new AchMapInto(data.nQuestTableIdx);
        achMapInto.AchievementNum = data.nCond;
        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_MAP_INTO, achMapInto);
    }

    private void Quest_KillMonster_Result(byte[] _packet)
    {
        body_SC_QUEST_NPC_KILL_RESULT data = new body_SC_QUEST_NPC_KILL_RESULT();
        data.PacketBytesToClass(_packet);

        Debug.Log("kill mon = " + data.nNpcTableIdx);

        AchKillMonster killMon         = new AchKillMonster(data.nNpcTableIdx, data.bChampion, 1);
        AchKillMonsterKind killMonKind = new AchKillMonsterKind(AsTableManager.Instance.GetTbl_Monster_Record(data.nNpcTableIdx).Monster_Kind_ID, data.bChampion, 1, 0);
        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_KILL_MONSTER, killMon);
        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_KILL_MONSTER_KIND, killMonKind);
    }

	private void Quest_Fail(byte[] _packet)
	{
        body1_SC_QUEST_FAILLIST_RESULT data = new body1_SC_QUEST_FAILLIST_RESULT();

		data.PacketBytesToClass(_packet);

        foreach (body2_SC_QUEST_FAILLIST_RESULT questFail in data.body)
        {
            ArkQuestmanager.instance.FailQuest(questFail.nQuestTableIdx);
            Debug.LogWarning("QuestFail = " + questFail.nQuestTableIdx);
        }

		// process fail quest
	}

    private void Quest_Daily_List_Result(byte[] _packet)
    {
        body1_SC_QUEST_DAILY_LIST_RESULT data = new body1_SC_QUEST_DAILY_LIST_RESULT();
        data.PacketBytesToClass(_packet);

        System.Collections.Generic.List<int> listDailyQuest = new System.Collections.Generic.List<int>();
        foreach (body2_SC_QUEST_DAILY_LIST_RESULT body2 in data.body)
            listDailyQuest.Add(body2.nQuestTableIdx);

        ArkQuestmanager.instance.AddDailyQuestIdx(listDailyQuest.ToArray());
    }

	private void Quest_Npc_Daily_List(byte[] _packet)
	{
		body1_SC_QUEST_GROUP_LIST data = new body1_SC_QUEST_GROUP_LIST();
		data.PacketBytesToClass(_packet);

		System.Collections.Generic.List<int> listNpcDailyQuest = new System.Collections.Generic.List<int>();
		foreach(body2_SC_QUEST_GROUP_LIST body2 in data.body)
		{
			Debug.LogWarning("Npc Daily Quest = " + body2.nQuestTableIdx);
			listNpcDailyQuest.Add(body2.nQuestTableIdx);
		}

		ArkQuestmanager.instance.SetNpcDailyQuest(listNpcDailyQuest);
	}

    private void Quest_Daily_List_Reset()
    {
        AsTableManager.Instance.Reset_Complete_Daily_Quests();

        if (AsHudDlgMgr.Instance.IsOpenQuestBook)
            AsHudDlgMgr.Instance.questBook.UpdateDailyList();
    }

    private void Quest_RunningList(byte[] _packet)
    {
        body1_SC_QUEST_RUNNING_LIST runningList = new body1_SC_QUEST_RUNNING_LIST();

        runningList.PacketBytesToClass(_packet);

        foreach (body2_SC_QUEST_RUNNING_LIST list in runningList.body)
        {
#if UNITY_EDITOR
            Debug.Log("Quest ID = " + list.nQuestIndex + " (status) = " + list.nStatus + "(Repeat) = " + list.nRepeat);
#endif

            QuestProgressStateNet state = (QuestProgressStateNet)list.nStatus;

            QuestProgressState questState = ArkQuestmanager.ConvertQuestProgressNetToClient(state);

            AsTableManager.Instance.SetTbl_QuesrRecordState(list.nQuestIndex, questState, list.nRepeat);

            Tbl_Quest_Record questRecord = AsTableManager.Instance.GetTbl_QuestRecord(list.nQuestIndex);

            // add Repeat
            questRecord.QuestDataInfo.Info.RepeatCurrent = list.nRepeat;

            System.Collections.Generic.List<AchBase> listAchieve = questRecord.QuestDataInfo.Achievement.GetDatas();

            int nCount = 0;
            int nAchCount = listAchieve.Count;

			// 퀘스트 추가
			ArkQuestmanager.instance.AcceptQuest(list.nQuestIndex, true);

			if (state != QuestProgressStateNet.QUEST_FAIL)
			{
				foreach (short value in list.nValue)
				{
					if (nCount < nAchCount)
					{
						listAchieve[nCount].SetAchievementCommonCount(value);
#if UNITY_EDITOR
						Debug.Log("value[" + nCount + "] =" + value);
#endif

					}
					nCount++;
				}
			}
			else
			{
				foreach (short value in list.nValue)
				{
					if (nCount < nAchCount)
						listAchieve[nCount].SetAchievementCommonCount(0);

					nCount++;
				}
			}

            if (questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_CLEAR ||
                questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
            {
                questRecord.QuestDataInfo.Achievement.SetAllComplete();
            }

           
            // 퀘스트 완료 체크
            if (questRecord.QuestDataInfo.Achievement.IsComplete() && questRecord.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                AsCommonSender.SendClearQuest(questRecord.QuestDataInfo.Info.ID);
        }

		ArkQuestmanager.instance.UpdateSortQuestList();
    }

    private void Quest_CompleteList(byte[] _packet)
    {
        body1_SC_QUEST_COMPLETE_LIST completeList = new body1_SC_QUEST_COMPLETE_LIST();

        completeList.PacketBytesToClass(_packet);

		foreach (body2_SC_QUEST_COMPLETE_LIST list in completeList.body)
		{
			AsTableManager.Instance.SetTbl_QuesrRecordState(list.nQuestIndex, QuestProgressState.QUEST_PROGRESS_COMPLETE, list.nRepeat);

			ArkSphereQuestTool.QuestData questData = AsTableManager.Instance.GetTbl_QuestData(list.nQuestIndex);

			// can repeat
			if (questData.CanRepeat() == true)
				AsTableManager.Instance.SetTbl_QuesrRecordState(list.nQuestIndex, QuestProgressState.QUEST_PROGRESS_NOTHING);

			ArkQuestmanager.instance.SetCompleteQuest(list.nQuestIndex, questData);

			if (questData.CanRepeat() == false)
				ArkQuestmanager.instance.SetCompleteMetaQuest(list.nQuestIndex);

#if UNITY_EDITOR
            Debug.Log("Complete = " + list.nQuestIndex + " Repeat = " + list.nRepeat);
#endif
        }

		ArkQuestmanager.instance.UpdateSortQuestList();
    }

    private void Quest_TalkWithNPC_Result(byte[] _packet)
    {
        body_SC_QUEST_CLEAR_TALK_RESULT talkWithNPCResult = new body_SC_QUEST_CLEAR_TALK_RESULT();
       // Debug.LogWarning("result talk with npc = " + talkWithNPCResult.eResult + "    " + talkWithNPCResult.nNPCTableID);
        talkWithNPCResult.PacketBytesToClass(_packet);

        if (talkWithNPCResult.eResult == eRESULTCODE.eRESULT_SUCC)
        {
            AchTalkNPC talkWithNPC = new AchTalkNPC(talkWithNPCResult.nNPCTableID);
            QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_TALK_WITH_NPC, talkWithNPC);
            ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();
        }
    }

    private void Quest_ClearOpenUI_Result(byte[] _packet)
    {
        body_SC_QUEST_CLEAR_OPEN_UI_RESULT open = new body_SC_QUEST_CLEAR_OPEN_UI_RESULT();
        open.PacketBytesToClass(_packet);

        if (open.eResult == eRESULTCODE.eRESULT_SUCC)
            QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_OPEN_UI, new AchOpenUI((OpenUIType)open.nUIType));
        else
        {
            Debug.LogWarning("quest clear open UI fail = " + open.eResult);
        }
    }

	private void Quest_MoveResult(byte[] _packet)
	{
		body_SC_QUEST_WARP_RESULT questWarpResult = new body_SC_QUEST_WARP_RESULT();
		questWarpResult.PacketBytesToClass(_packet);

		string message = string.Empty;

		if (questWarpResult.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_WARP_COMBAT)
			message = AsTableManager.Instance.GetTbl_String(4257);
		else if (questWarpResult.eResult == eRESULTCODE.eRESULT_FAIL_QUEST_WARP_PVP_ARENA)
			message = AsTableManager.Instance.GetTbl_String(4258);
		else
		{
			Debug.LogWarning(message);
			return;
		}

		AsEventNotifyMgr.Instance.CenterNotify.AddMessage(message);
		AsCommonSender.isSendWarp = false; //$yde
	}


    private void Quest_Info(byte[] _packet)
    {
        body_SC_QUEST_INFO questInfo = new body_SC_QUEST_INFO();
        questInfo.PacketBytesToClass(_packet);

        ArkQuest quest = ArkQuestmanager.instance.GetQuest(questInfo.nQuestTableIdx);

        ArkSphereQuestTool.QuestData questData = quest.GetQuestData();

        AchBase achieve = questData.Achievement.GetData(questInfo.nCond);

        if (achieve == null)
        {
            Debug.LogWarning("Quest[" + questInfo.nQuestTableIdx + "] = " + questInfo.nCond + " is not found");
            return;
        }

        switch (achieve.QuestMessageType)
        {
			case QuestMessages.QM_KILL_MONSTER:
				AchKillMonster killMon = achieve as AchKillMonster;
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_KILL_MONSTER, killMon);
				break;

			case QuestMessages.QM_KILL_MONSTER_KIND:
				AchKillMonsterKind killMonKind = achieve as AchKillMonsterKind;
				QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_KILL_MONSTER_KIND, killMonKind);
				break;

            case QuestMessages.QM_USE_ITEM:
                AchUseItem useItem = achieve as AchUseItem;
                QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_ITEM, new AchUseItem(useItem.ItemID, 1));
                break;

            case QuestMessages.QM_USE_WAY_POINT:
                 QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_WAY_POINT, achieve as AchWaypoint);
                 break;

            case QuestMessages.QM_FRIENDSHIP:
                 {
                     if (questInfo.nValue != 0)
                        QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_FRIENDSHIP, new AchFriendship(questInfo.nValue, 0));
                 }
                 break;

            case QuestMessages.QM_ADD_FRIEND:
                     QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_ADD_FRIEND, new AchAddFirends(questInfo.nValue));
                 break;

            case QuestMessages.QM_USE_ITEM_IN_MAP:
                 QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_ITEM_IN_MAP, achieve as AchUseItemInMap);
                 break;

            case QuestMessages.QM_INSERT_SEAL:
                 {
                     AchInsertSeal orgInsertSeal = achieve as AchInsertSeal;
                     AchInsertSeal insertSeal = new AchInsertSeal(orgInsertSeal.sealGrade, orgInsertSeal.sealKind, questInfo.nValue);
                     QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_INSERT_SEAL, insertSeal);
                 }
                 break;

			case QuestMessages.QM_USE_ITEM_TO_TARGET:
				 {
					 AchUseItemToTarget selectAchieve  = achieve as AchUseItemToTarget;
					 AchUseItemToTarget achUse = new AchUseItemToTarget(selectAchieve.targetType, selectAchieve.targetID, selectAchieve.Champion, selectAchieve.ItemID, selectAchieve.ItemCount, 0);
					 achUse.AssignedQuestID = selectAchieve.AssignedQuestID;
					 achUse.AchievementNum	= selectAchieve.AchievementNum;
					 achUse.CommonCount		= questInfo.nValue;
					 QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_ITEM_TO_TARGET, achUse);
				 }
				 break;

            case QuestMessages.QM_STRENGTHEN_ITEM:
                 {
                     AchStrengthenItem ach = new AchStrengthenItem(questInfo.nValue);
                     ach.AssignedQuestID = questInfo.nQuestTableIdx;
                     QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_STRENGTHEN_ITEM, ach);
                 }
				 break;
			case QuestMessages.QM_GET_ITEM_FROM_SHOP:
				 {
					 AchGetItemFromShop ach = achieve as AchGetItemFromShop;
					 AchGetItemFromShop getItem = new AchGetItemFromShop(ach.ItemID, questInfo.nValue);
					 getItem.AssignedQuestID	= ach.AssignedQuestID;
					 getItem.AchievementNum		= ach.AchievementNum;
					 QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_ITEM_FROM_SHOP, getItem);
				 }
				 break;
			#region _PVP
			case QuestMessages.QM_GET_PVP_POINT:
				{
					AchGetPvpPoint ach = new AchGetPvpPoint(questInfo.nValue);
					ach.AssignedQuestID = achieve.AssignedQuestID;
					ach.AchievementNum = achieve.AchievementNum;
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_GET_PVP_POINT, ach);
				}
				break;

			case QuestMessages.QM_PLAY_PVP:
				{
					AchPvpPlay ach = new AchPvpPlay(ePvpMatchMode.NOTHING, AsPvpDlg.ePvpMode.ePvpMode_Max, questInfo.nValue);
					ach.AssignedQuestID = achieve.AssignedQuestID;
					ach.AchievementNum = achieve.AchievementNum;
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_PLAY_PVP, ach);
				}
				break;

			case QuestMessages.QM_WIN_PVP:
				{
					AchPvpWin ach = new AchPvpWin(ePvpMatchMode.NOTHING, AsPvpDlg.ePvpMode.ePvpMode_Max, questInfo.nValue);
					ach.AssignedQuestID = achieve.AssignedQuestID;
					ach.AchievementNum = achieve.AchievementNum;
					QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_WIN_PVP, ach);
				}
				break;
			#endregion
        }
    }
    #endregion
	
	#region -SkillReset
	void SkillBookList( byte[] _packet)
	{
		body1_SC_SKILLBOOK_LIST skillBookList = new body1_SC_SKILLBOOK_LIST();
		skillBookList.PacketBytesToClass( _packet);
	}
	#endregion
	
	#region - skill list & shop -
	void SkillList_Receive(byte[] _packet)
	{
		body1_SC_SKILL_LIST skillList = new body1_SC_SKILL_LIST();
		skillList.PacketBytesToClass(_packet);

		StartCoroutine(Wait_Init_CR(skillList));
//		SkillBook.Instance.SetCurrentSkill(skillList);
	}
	
	IEnumerator Wait_Init_CR(body1_SC_SKILL_LIST _skillList)
	{
		while(true)
		{
			yield return null;
			
			if(AsQuickSlotManager.Instance != null && AsEntityManager.Instance.UserEntity != null)
			{
				SkillBook.Instance.SetCurrentSkill(_skillList);
				break;
			}
		}
	}

	void RestructureSkillshop()
	{
//		GameObject goSkillshop = GameObject.Find( "GUI_skillshop");
//		if( null == goSkillshop)
//			return;
//
//		AsSkillshopDlg skillShopDlg = goSkillshop.GetComponent<AsSkillshopDlg>();
//		skillShopDlg.Restructure();

		GameObject goSkillshop = GameObject.Find( "SkillShopDlg");
		if( null == goSkillshop)
			return;

		AsSkillshopDlg skillShopDlg = goSkillshop.GetComponentInChildren<AsSkillshopDlg>();
		skillShopDlg.Restructure();
	}

	void SkillLearn_Result(byte[] _packet)
	{
		body_SC_SKILL_LEARN_RESULT result = new body_SC_SKILL_LEARN_RESULT();
		result.PacketBytesToClass(_packet);

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			Debug.Log( "skill learn result: id=" + result.sSkill.nSkillTableIdx + ", lv=" + result.sSkill.nSkillLevel);
            QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_GET_SKILL, new AchGetSkill( result.nSkillBookIdx));
			SkillBook.Instance.AddLearnSkill( result);

            if (result.nSkillBookIdx == 0) // quest
            {
                AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Player_Skill_Learn(false));
				if( null != AsUserInfo.Instance.GetCurrentUserEntity() && null != AsUserInfo.Instance.GetCurrentUserEntity().ModelObject )
                	AsSoundManager.Instance.PlaySound("Sound/PC/Common/Se_Common_ArkSkillGet_Eff",
						AsUserInfo.Instance.GetCurrentUserEntity().ModelObject.transform.position, false);

               Tbl_Skill_Record skillRecord =  AsTableManager.Instance.GetTbl_Skill_Record(result.sSkill.nSkillTableIdx);

                string msg = string.Format(AsTableManager.Instance.GetTbl_String(879), AsTableManager.Instance.GetTbl_String(skillRecord.SkillName_Index));

                AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessageColorTag(msg);
            }
            else
            {
                AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Player_Skill_Learn(true));
				if( null != AsUserInfo.Instance.GetCurrentUserEntity() && null != AsUserInfo.Instance.GetCurrentUserEntity().ModelObject )
                	AsSoundManager.Instance.PlaySound("Sound/PC/Common/Se_Common_SkillLearn_Eff",
						AsUserInfo.Instance.GetCurrentUserEntity().ModelObject.transform.position, false);
            }
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_NOT_EXIST:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(81), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_LEARN_ALREADY:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(82), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_LEARN_OTHER:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(83), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_NEED_LEVEL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(84), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_NEED_SKILLLEVEL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(85), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_NEED_GOLD:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(86), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		case eRESULTCODE.eRESULT_FAIL_SKILLLEARN_NEED_CLASS:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(4086), AsTableManager.Instance.GetTbl_String(87), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			break;
		default:
			Debug.LogError( "Invalid SkillLeanResult");
			break;
		}

		RestructureSkillshop();
	}

	void SkillReset_Result(byte[] _packet)
	{
		body_SC_SKILL_RESET_RESULT result = new body_SC_SKILL_RESET_RESULT();
		result.PacketBytesToClass(_packet);

		if(result.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			if( result.nSkillTableIdx == 0 )	// reset all skill
			{
				SkillBook.Instance.ResetSkillBook();
	
				/// refresh skill book HUD
				AsHudDlgMgr.Instance.RecvSkillReset();
			}
			else // reset one skill
			{
				SkillBook.Instance.ResetOneSkill( result.nSkillTableIdx );
				
				AsQuickSlotManager.Instance.DeleteQuickSlot( result.nSkillTableIdx );
				
				if(AsHudDlgMgr.Instance.IsOpenedSkillBook == true)
				{
					SkillBook.Instance.ResetDicNotSettingData();
					AsHudDlgMgr.Instance.skillBookDlg.RefreshSkillBookDlg();			
				}
				
				if(AsHudDlgMgr.Instance.IsOpenedSkillshop == true)
				{
					AsSkillshopDlg dlg = AsHudDlgMgr.Instance.skillShopObj.GetComponentInChildren<AsSkillshopDlg>();
					Debug.Assert( null != dlg);
					dlg.Restructure();
				}
				
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(123), AsTableManager.Instance.GetTbl_String(122), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			}
		}
		else
		{
			Debug.LogError("AsCommonProcess::SkillReset_Result: skill reset is failed");
		}
	}
	#endregion

	#region -Chatting
	void ChatPartyResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if( null != AsSocialManager.Instance.SocialData.GetBlockOutUser( result.nUserUniqKey))
			return;

		result.kName.szName = result.kName.szName.Remove( result.kName.szName.Length - 1);

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "[{0}]: {1}", result.kName.szName, result.kMessage.szMsg);

		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_PARTY);
		AsChatManager.Instance.ShowChatBalloon( result.nCharUniqKey, result.kMessage.szMsg, eCHATTYPE.eCHATTYPE_PARTY);
		AsEmotionManager.Instance.EmotionProcess(result); //$ yde
	}

	void ChatGuildResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if( null != AsSocialManager.Instance.SocialData.GetBlockOutUser( result.nUserUniqKey))
			return;

		result.kName.szName = result.kName.szName.Remove( result.kName.szName.Length - 1);

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "[{0}]: {1}", result.kName.szName, result.kMessage.szMsg);

		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_GUILD);
		AsChatManager.Instance.ShowChatBalloon( result.nCharUniqKey, result.kMessage.szMsg, eCHATTYPE.eCHATTYPE_GUILD);
		AsEmotionManager.Instance.EmotionProcess(result); //$ yde
	}

	void ChatLocalResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if (null != AsSocialManager.Instance.SocialData.GetBlockOutUser(result.nUserUniqKey))
			return;

		AsChatManager.Instance.InsertChatRawData( result);
	}

	void ChatPrivateResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if( null != AsSocialManager.Instance.SocialData.GetBlockOutUser( result.nUserUniqKey))
			return;

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_FAIL_CHAT_MYSELF:
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(112), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		case eRESULTCODE.eRESULT_FAIL_CHAT_NOTHING_CHAR:
			string format = AsTableManager.Instance.GetTbl_String(113);
			string sysMsg = string.Format( format, result.kName.szName);
			AsChatManager.Instance.InsertChat( sysMsg, eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		StringBuilder sb = new StringBuilder();

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( ( null == userEntity) || ( result.nCharUniqKey != userEntity.UniqueId))
			sb.AppendFormat( "From {0}: {1}", result.kName.szName, result.kMessage.szMsg);
		else
			sb.AppendFormat( "To {0}: {1}", result.kName.szName, result.kMessage.szMsg);

		string receiver = AsUtil.GetRealString( result.kName.szName);
		PlayerPrefs.SetString( "LatestWhisper", receiver);
		PlayerPrefs.Save();

		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_PRIVATE);

		AsChatManager.Instance.ShowChatBalloon( result.nCharUniqKey, result.kMessage.szMsg, eCHATTYPE.eCHATTYPE_PRIVATE);
		AsEmotionManager.Instance.EmotionProcess( result); //$ yde
	}

	void ChatMapResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if( null != AsSocialManager.Instance.SocialData.GetBlockOutUser( result.nUserUniqKey))
			return;

		result.kName.szName = result.kName.szName.Remove( result.kName.szName.Length - 1);

		StringBuilder sb = new StringBuilder();
		//sb.AppendFormat( "[{0}]: {1}", result.kName.szName, result.kMessage.szMsg);
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String( 2122), result.kName.szName, result.kMessage.szMsg);

		bool isMe = ( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey) ? true : false;
		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_MAP, isMe);
		AsChatManager.Instance.ShowChatBalloon( result.nCharUniqKey, result.kMessage.szMsg, eCHATTYPE.eCHATTYPE_MAP);
		AsEmotionManager.Instance.EmotionProcess( result); //$ yde
	}

	void ChatWorldResult( byte[] _packet)
	{
		//Debug.Log( "ChatWorldResult");
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		//Check BlockUser
		if( null != AsSocialManager.Instance.SocialData.GetBlockOutUser( result.nUserUniqKey))
			return;

		result.kName.szName = result.kName.szName.Remove( result.kName.szName.Length - 1);

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String( 2123), result.kName.szName, result.kMessage.szMsg);

		bool isMe = ( AsUserInfo.Instance.SavedCharStat.uniqKey_ == result.nCharUniqKey) ? true : false;
		AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SERVER, isMe);
		AsChatManager.Instance.ShowChatBalloon( result.nCharUniqKey, result.kMessage.szMsg, eCHATTYPE.eCHATTYPE_SERVER);
		AsEmotionManager.Instance.EmotionProcess( result); //$ yde
	}

	void ChatSystemResult( byte[] _packet)
	{
		body_SC_CHAT_WITH_BALLOON_RESULT result = new body_SC_CHAT_WITH_BALLOON_RESULT();
		result.PacketBytesToClass( _packet);

		AsChatManager.Instance.InsertGMChat( result.kMessage.szMsg);
	}

	void ChatEmoticonResult( byte[] _packet)
	{
		Debug.Log( "AsCommonProcess::ChatEmoticonResult: began");

		body_SC_CHAT_EMOTICON_RESULT result = new body_SC_CHAT_EMOTICON_RESULT();
		result.PacketBytesToClass( _packet);

		AsEmotionManager.Instance.Recv_Emoticon(result);
	}
	#endregion -Chatting

    #region -tutorial
    void FirstConnect()
    {
        Debug.LogWarning("First Connect");
        QuestTutorialMgr.Instance.firstConnect = true;
    }
    #endregion

    #region -npc store
    void ItemBuyResult(byte[] _packet)
    {
        body_SC_SHOP_ITEMBUY result = new body_SC_SHOP_ITEMBUY();
        result.PacketBytesToClass(_packet);

        if (AsBasePurchase.Instance != null)
            AsBasePurchase.Instance.StopTimer();

        ItemBuyResultMessage(result.eResult, false);
    }

    void ItemSellResult(byte[] _packet)
    {
        body_SC_SHOP_ITEMSELL result = new body_SC_SHOP_ITEMSELL();
        result.PacketBytesToClass(_packet);

        ItemBuyResultMessage(result.eResult, true);
    }

	void ItemLessSellResult(byte[] _packet)
	{
		body_SC_SHOP_USELESS_ITEMSELL result = new body_SC_SHOP_USELESS_ITEMSELL();
        result.PacketBytesToClass(_packet);

		AsCommonSender.isSendShopUseLessItemSell = false;
	}

	void NpcShopInfoResult(byte[] _packet)
	{
		body1_SC_SHOP_INFO result = new body1_SC_SHOP_INFO();
		result.PacketBytesToClass(_packet);

		if (result.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			if (AsHudDlgMgr.Instance.IsOpenNpcStore == true)
				AsHudDlgMgr.Instance.npcStore.ProcessNpcShopInfo(result.body);
		}
	}

    //소유하고 있는 gold보다 아이템이 비쌉니다.
   	//더이상 골드을 추가할 수 없습니다.
    void ItemBuyResultMessage(eRESULTCODE _result, bool _isSell)
    {
        string message = string.Empty;
        string title   = AsTableManager.Instance.GetTbl_String(126);
        switch (_result)
        {
            case eRESULTCODE.eRESULT_SUCC:
                {
                    if (_isSell == true)
                    {
                        AsSoundManager.Instance.PlaySound(AsSoundPath.ItemBuyResultGold, Vector3.zero, false);
                    }
                    else
                    {
						if (AsHudDlgMgr.Instance.npcStore != null)
						{
							AsHudDlgMgr.Instance.npcStore.RequestShopInfo();
							AsHudDlgMgr.Instance.npcStore.PlayDealSound();
						}
                    }
                }
                break;
            case eRESULTCODE.eRESULT_FAIL_SHOP_NOTGOLD:
                message = AsTableManager.Instance.GetTbl_String(1400);
                break;

            case eRESULTCODE.eRESULT_FAIL_SHOP_NOTCASH:
                message = AsTableManager.Instance.GetTbl_String(264);
                break;

            case eRESULTCODE.eRESULT_FAIL_SHOP_INVENTORYFULL:
                message = AsTableManager.Instance.GetTbl_String(1399);
                break;

            case eRESULTCODE.eRESULT_FAIL_SHOP_OVERGOLD:
                message = AsTableManager.Instance.GetTbl_String(205);
                break;

            case eRESULTCODE.eRESULT_FAIL_SHOP_IMPOSSIBLE_ITEM:
                message = AsTableManager.Instance.GetTbl_String(1415);
                break;

			case eRESULTCODE.eRESULT_FAIL_SHOP_HAVE_NOTITEM:
				message = AsTableManager.Instance.GetTbl_String(2193);
				break;
			case eRESULTCODE.eRESULT_FAIL_SHOP_BUY_LIMIT:
				message = AsTableManager.Instance.GetTbl_String(27);
				if (AsHudDlgMgr.Instance.IsOpenNpcStore)
					AsHudDlgMgr.Instance.npcStore.RequestShopInfo();
				break;

            default:
                message = _result.ToString();
                break;
        }

        if (message != string.Empty)
        {
            if (_result == eRESULTCODE.eRESULT_FAIL_SHOP_NOTCASH )
            {
                if (AsGameMain.useCashShop == true)
                    AsNotify.Instance.MessageBox(title, message, AsHudDlgMgr.Instance, "NotEnoughMiracleProcessInGame", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
                else
                    AsNotify.Instance.MessageBox(title, message, AsNotify.MSG_BOX_TYPE.MBT_OK);
            }
            else
                AsNotify.Instance.MessageBox(title, message, null, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
        }
    }
    #endregion

    #region -cash store
    void ReciveAndroidPublicKey(byte[] _packet)
    {

    }

    void ReceiveFindGiftFriend(byte[] _packet)
    {
        body_SC_CHARGE_GIFT_FIND friend = new body_SC_CHARGE_GIFT_FIND();

        friend.PacketBytesToClass(_packet);

        if (AsHudDlgMgr.Instance != null)
        {
            if (AsHudDlgMgr.Instance.IsOpenCashStore)
                if (AsHudDlgMgr.Instance.cashStore != null)
                    AsHudDlgMgr.Instance.cashStore.ReceiveGiftFriendInfo(friend.nGiftAccount, friend.nLevel, friend.eClass);
        }
        else
        {
            AsCashStore.Instance.ReceiveGiftFriendInfo(friend.nGiftAccount, friend.nLevel, friend.eClass);
        }
    }

    void ReciveChargeItemList(byte[] _packet)
    {
        body1_SC_CHARGE_ITEMLIST cashItemList = new body1_SC_CHARGE_ITEMLIST();
        cashItemList.PacketBytesToClass(_packet);

        if (AsCashStore.Instance != null)
            AsCashStore.Instance.RequestProductInfoToPurchaseServer(cashItemList.products);
    }

    void ReciveChashAndChargeResult(byte[] _packet, PROTOCOL_CS _protocol)
    {
        eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;

        if (_protocol == PROTOCOL_CS.SC_CASHSHOP_ITEMBUY)
        {
            body_SC_CASHSHOP_ITEMBUY result = new body_SC_CASHSHOP_ITEMBUY();
            result.PacketBytesToClass(_packet);
            eResult = result.eResult;

            if (eResult == eRESULTCODE.eRESULT_SUCC)
            {
                if (AsCashStore.Instance != null)
                    AsCashStore.Instance.CashItemBuySuccess();
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
                    AsNotify.Instance.MessageBox(title, AsTableManager.Instance.GetTbl_String(1399), AsCashStore.Instance, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
                }
                else if (eResult == eRESULTCODE.eRESULT_FAIL_SHOP_MAXLIMITOVER)
                {
                    AsNotify.Instance.MessageBox(title, AsTableManager.Instance.GetTbl_String(1643), AsCashStore.Instance, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
                }
                else
                    Debug.Log(eResult);
            }
        }
        else if (_protocol == PROTOCOL_CS.SC_CHARGE_BUY)
        {
            body_SC_CHARGE_BUY result = new body_SC_CHARGE_BUY();
            result.PacketBytesToClass(_packet);
            eResult = result.eResult;

            if (eResult == eRESULTCODE.eRESULT_SUCC && AsCashStore.Instance != null)
            {
                Debug.LogWarning("charge success");

                if (AsBasePurchase.Instance != null)
                    AsBasePurchase.Instance.OrganizeTransaction(true);
            }
            else
                Debug.Log("Recv CashChargeResult = " + result.eResult);
        }
    }
    #endregion

    void ReciveCoolTimeList( byte[] _packet)
	{
		if( true == TargetDecider.CheckCurrentMapIsIndun())
			return;
		
		body1_SC_COOL_TIME_LIST result = new body1_SC_COOL_TIME_LIST();
		result.PacketBytesToClass( _packet);		

		if( null == CoolTimeGroupMgr.Instance )
		{
			Debug.LogError("ReciveCoolTimeList()[ null == CoolTimeGroupMgr.Instance ]");
			return;
		}

        CoolTimeGroupMgr.Instance.CoolTimeClear();
		
		if( 0 >= result.nCoolTimeCount)
			return;
		
		if( null == result.body )
		{
			return;
		}

		for( int i = 0; i < result.body.Length; i++)
		{
			CoolTimeGroupMgr.Instance.SetCoolTimeGroup( result.body[i].nCoolGroup, result.body[i].nRemainTime );
		}

		Debug.Log( "ReciveCoolTimeList");
	}


	void ReceiveWaypointLevelResult( byte[] _packet)
	{
		body1_SC_WAYPOINT_LEVELACTIVE_RESULT result = new body1_SC_WAYPOINT_LEVELACTIVE_RESULT();
		result.PacketBytesToClass( _packet);


		if( eRESULTCODE.eRESULT_SUCC == result.eResult )
		{
			if( null == result.datas )
			{
				Debug.LogError("SC_WAYPOINT_LEVELACTIVE_RESULT (eRESULT_SUCC) [ null == result.datas ]");
				return;
			}

			foreach( body2_SC_WAYPOINT_LEVELACTIVE_RESULT _data in result.datas )
			{
				AsUserInfo.Instance.InsertWapointList( _data.nWarpTableIdx );
			}


			if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg )
			{
				AsHudDlgMgr.Instance.worldMapDlg.ReceiveWaypointActive();
			}

			//AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(803), eCHATTYPE.eCHATTYPE_SYSTEM);

		}
		else
		{
			Debug.LogError("SC_WAYPOINT_LEVELACTIVE_RESULT error : " + result.eResult );
		}

	}


}
