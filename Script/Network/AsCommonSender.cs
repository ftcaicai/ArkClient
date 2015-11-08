//#define USE_OLD_COSTUME
#define _SOCIAL_LOG_
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AsCommonSender
{
	static SOCKET_STATE ms_eSocketState = SOCKET_STATE.SS_DISCONNECT;

	public static bool isSendItemMove = false;
	public static bool isSendItemUse = false;
	public static bool isSendItemRemove = false;
	public static bool isSendPickupDropItem = false;
	public static bool isSendWarp = false;
	public static bool isSendPostList = false;
	public static bool isSendProductProgress = false;
	public static bool isSendCollectRequest = false;
	public static bool isSendItemInventroyOpen = false;
	public static bool isSendGuild = false;
	public static bool isSendLivePack = false;
	public static bool isSendShopUseLessItemSell = false;
	private static int usedSkillIdx = -1;
	public static bool isSendStrengthen = false;
	public static bool isSendEnchant = false;
	public static bool isSendItemMix = false;

	public static bool isSendItemProductRegister = false;


	static public void ResetSendCheck()
	{
		isSendItemMove = false;
		isSendItemUse = false;
		isSendItemRemove = false;
		isSendPickupDropItem = false;
		isSendWarp = false;
		isSendProductProgress = false;
		isSendCollectRequest = false;
		isSendItemInventroyOpen = false;
		isSendLivePack = false;
		isSendShopUseLessItemSell = false;

		isSendItemProductRegister = false;
		isSendStrengthen = false;
		isSendEnchant = false;
		usedSkillIdx = -1;//$yde
		isSendItemMix = false;
	}

	//$yde
	#region - skill using -
	public static bool CheckSkillUseProcessing()
	{
		if (usedSkillIdx != -1)
			return true;
		else
			return false;
	}

	public static void BeginSkillUseProcess(int _idx)
	{
		if (_idx == -1)
			Debug.LogError("AsCommonSender::BeginSkillUseProcess: index is -1. check skill table");

		usedSkillIdx = _idx;
		//		Debug.LogWarning("AsCommonSender::BeginSkillUseProcess: skill using permitted. AsCommonSender.usedSkillIdx = " + usedSkillIdx);
	}

	public static void EndSkillUseProcess()
	{
		usedSkillIdx = -1;
		//		Debug.LogWarning("AsCommonSender::EndSkillUseProcess: skill using end");
	}
	#endregion

	static public void SendLive()
	{
		body_CS_LIVE packetData = new body_CS_LIVE();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		isSendLivePack = true;
		Debug.Log("Send body_CS_LIVE");
	}

	static public void SetSocketState(SOCKET_STATE state)
	{
		ms_eSocketState = state;
		Debug.Log("server state : " + ms_eSocketState);
	}

	static public SOCKET_STATE GetSocketState()
	{
		return ms_eSocketState;
	}

	static public void Send(byte[] data)
	{
#if UNITY_EDITOR
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
		{
			switch ((PACKET_CATEGORY)data[2])
			{
				case PACKET_CATEGORY._CATEGORY_CS:
					if (PROTOCOL_CS.CS_BACKGROUND_PROC != (PROTOCOL_CS)data[3])
						Debug.LogError("AsCommonSender::Send()[ error GAME_STATE.STATE_INGAME ] protocal : " + (PACKET_CATEGORY)data[2] + " packet : " + (PROTOCOL_CS)data[3]);
					break;
				case PACKET_CATEGORY._CATEGORY_CS2:
					Debug.LogError("AsCommonSender::Send()[ error GAME_STATE.STATE_INGAME ] protocal : " + (PACKET_CATEGORY)data[2] + " packet : " + (PROTOCOL_CS_2)data[3]);
					break;

				case PACKET_CATEGORY._CATEGORY_CS3:
					Debug.LogError("AsCommonSender::Send()[ error GAME_STATE.STATE_INGAME ] protocal : " + (PACKET_CATEGORY)data[2] + " packet : " + (PROTOCOL_CS_3)data[3]);
					break;
			}
		}
#endif

		switch (GetSocketState())
		{
			case SOCKET_STATE.SS_LOGIN:
			case SOCKET_STATE.SS_GAMESERVER:
				{
					if (null != AsNetworkManager.Instance)
						AsNetworkManager.Instance.AddSend(data);
				}
				return;
		}
	}
	
	//$yde
	public static void Send( AsPacketHeader _packet)
	{
		byte[] bytes = _packet.ClassToPacketBytes();
		Send( bytes);
	}

	static public void SendToIAP(byte[] data)
	{
		if (AsNetworkIAPManager.Instance != null)
		{
			if (AsNetworkIAPManager.Instance.socketState == IAP_SOCKET_STATE.ISS_CONNECT)
			{
				AsNetworkIAPManager.Instance.AddSend(data);
				return;
			}
		}
	}

	/*
	* Send Packet : Create Dungeon Complete
	* Packet Define : CI_PICKUP_ITEM
   */
	static public void SendPickupItem(Int32 nDropItemIdx, bool isUseSendPack = true)
	{
		if (true == AsGameMain.isBackGroundPause)
			return;

		if (true == isSendPickupDropItem && true == isUseSendPack)
			return;

		AS_body_CS_PICKUP_ITEM packetData = new AS_body_CS_PICKUP_ITEM(nDropItemIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		isSendPickupDropItem = true;

		// Debug.Log( "Send CI_PICKUP_ITEM [ dropItem id : " + nDropItemIdx);
	}

	/*
	* Send Packet : Move
	* Packet Define : CS_CHAR_MOVE
   */

	public static Vector3 selfPosition = Vector3.zero;

	static public void SendMove(SavedMoveInfo _info)//$yde
	{
		SendMove(_info.nMoveType, _info.sCurPosition, _info.sDestPosition);
	}

	static public void SendMove(eMoveType _moveType, Vector3 _curPosition, Vector3 _destPosition)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		if (_destPosition == Vector3.zero)
		{
			Debug.LogWarning("AsCommonSender:: SendMove: _destPosition = " + _destPosition);
			//			return;
		}

		if (false == NavMeshFinder.Instance.IsCell(_curPosition))
		{
			//string strText = "NavMeshFind Failed IsCell : " + _curPosition;
			//AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( "error", strText,
			//						null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR));
			//return;

			Debug.LogError("NavMeshFind Failed IsCell : " + _curPosition);

			if (null != AsUserInfo.Instance.GetCurrentUserEntity())
			{
				AsUserInfo.Instance.GetCurrentUserEntity().SetRePosition(selfPosition);
				AsEntityManager.Instance.GetPlayerCharFsm().SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			}
			_curPosition = selfPosition;
		}

		if (_curPosition == Vector3.zero || _destPosition == Vector3.zero)
			Debug.LogError("AsCommonSender::SendMove: _curPosition = " + _curPosition + ", _destPosition = " + _destPosition);

		selfPosition = _curPosition;
		//Debug.Log( "Send Move [curpos : " + _curPosition + " target pso : " + _destPosition);
		AS_CS_CHAR_MOVE charMove = new AS_CS_CHAR_MOVE((int)_moveType, _curPosition, _destPosition);
		byte[] data = charMove.ClassToPacketBytes();
		Send(data);
	}

	/*
	* Send Packet : mop change
	* Packet Define : CS_WARP
	*/
	static public void SendWarp(Int32 nWarpTableIdx, Int32 nNpcSpawnIdx = 0)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
		{
			Debug.LogError("SendWarp() [GAME_STATE.STATE_INGAME !=  AsGameMain.s_gameState ]");
			return;
		}

		AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_ZoneWarp());

		if (true == AsCommonSender.isSendWarp)
		{
			Debug.Log("true == AsCommonSender.isSendWarp");
			return;
		}

		if (true == AsCommonSender.isSendGuild)
		{
			Debug.LogWarning("true == AsCommonSender.isSendGuild");
			return;
		}

		if (true == AsCommonSender.isSendPostList)
		{
			Debug.LogWarning("true == AsCommonSender.isSendPostList");
			return;
		}

		AsTradeManager.Instance.Request_Response_Cancel();

		AsCommonSender.isSendWarp = true;

		AS_body_CS_WARP packetData = new AS_body_CS_WARP(nWarpTableIdx, nNpcSpawnIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
		AsGameMain.s_gameState = GAME_STATE.STATE_WARP_SEND;

		//$yde
		AsPlayerFsm player = AsEntityManager.Instance.GetPlayerCharFsm();
		if (player != null)
		{
			player.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			player.ClearMovePacket();
		}

		Debug.Log("CS_WARP [ id : " + nWarpTableIdx);
	}
	
	

	static public void SendWarpEnd()
	{
		AS_body_CS_WARP_END packetData = new AS_body_CS_WARP_END();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("CS_WARP_END");
	}

	static public void SendWarpXZEnd(Int32 nMapIdx, Vector3 vPos)
	{
		body_CS_WARP_XZ_END packetData = new body_CS_WARP_XZ_END(nMapIdx, vPos);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendWarpCancel()
	{
		body_CS_WARP_CANCEL packetData = new body_CS_WARP_CANCEL();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendResurrection()
	{
		body_CS_RESURRECTION resurrection = new body_CS_RESURRECTION();
		byte[] data = resurrection.ClassToPacketBytes();
		Send(data);
	}

	static public void SendOtherUserResurrection(UInt16 _sessionIdx)
	{
		body_CS_RESURRECTION resurrection = new body_CS_RESURRECTION(_sessionIdx);
		byte[] data = resurrection.ClassToPacketBytes();
		Send(data);
	}

	static public void SendGotoTown()
	{
		if (true == AsCommonSender.isSendWarp)
		{
			Debug.Log("SendGotoTown[true == AsCommonSender.isSendWarp]");
			return;
		}

		AsCommonSender.isSendWarp = true;

		body_CS_GOTO_TOWN gotoTown = new body_CS_GOTO_TOWN();
		byte[] data = gotoTown.ClassToPacketBytes();
		Send(data);

		AsGameMain.s_gameState = GAME_STATE.STATE_WARP_SEND;
	}

	static public void SendGotoTownEnd()
	{
		body_CS_GOTO_TOWN_END gotoTownEnd = new body_CS_GOTO_TOWN_END();
		byte[] data = gotoTownEnd.ClassToPacketBytes();
		Send(data);
	}

	static public bool SendUseItem(Int32 iSlot, int iTargetNpcTableID = -1)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return false;

		if (true == isSendItemUse)
			return false;

		if (ItemMgr.HadItemManagement.Inven.invenSlots.Length <= iSlot || 0 > iSlot)
			return false;

		if (null == ItemMgr.HadItemManagement.Inven.invenSlots[iSlot])
			return false;

		if (null == ItemMgr.HadItemManagement.Inven.invenSlots[iSlot].realItem)
			return false;

		body_CS_ITEM_USE packetdata = new body_CS_ITEM_USE(iSlot);

		packetdata.nTargetID = ArkQuestmanager.instance.SetUseItemToTarget(iSlot, iTargetNpcTableID);

		// only inven
		if (packetdata.nTargetID == 0 && iTargetNpcTableID == -1)
		{
			int targetID = ArkQuestmanager.instance.SetTargetForQuestSkillItem(iSlot);

			if (targetID != 0)
				packetdata.nTargetID = targetID;
		}

		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		isSendItemUse = true;
		return true;
	}

	static public void SendMoveItem(Int32 iSlot1, Int32 iSlot2)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		if (true == isSendItemMove)
		{
			Debug.Log("AsCommonSender::SendMoveItem()[ true == isSendItemMove ] slot 1 : " + iSlot1 + " slot 2 : " + iSlot2);
			return;
		}

		body_CS_ITEM_MOVE packetdata = new body_CS_ITEM_MOVE(iSlot1, iSlot2);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMove = true;
		Debug.Log("SendMoveItem [ slot1 : " + iSlot1 + " slot2 : " + iSlot2);
	}

	static public void SendRemoveItem(Int32 islot, Int32 icount)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		if (true == isSendItemRemove)
		{
			Debug.Log("AsCommonSender::SendRemoveItem()[ true == isSendItemRemove ] ");
			return;
		}

		body_CS_ITEM_REMOVE packetdata = new body_CS_ITEM_REMOVE(islot, icount);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		isSendItemRemove = true;
		Debug.Log("SendRemoveItem [ slot1 : " + islot + " count : " + icount);
	}

	static public void SendInvenPageSort(Int32 iPage)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PAGE_SORT packetdata = new body_CS_ITEM_PAGE_SORT(iPage);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendInvenPageSort [ page : " + iPage);
	}

	/*
	* Send Packet :
	* Packet Define : CS_QUICKSLOT_CHANGE
	*/
	static public void SendQuickslotChange(Int16 _nslot, Int32 _nValue, eQUICKSLOT_TYPE _eType)
	{
		switch (_nslot)
		{
			case 12:
				AsGameGuideManager.Instance.CheckUp(eGameGuideType.Q_SlotOpen, -1);
				AsQuickSlotManager.Instance.PromprtActivateAlarm(2);
				break;
			case 20:
				AsQuickSlotManager.Instance.PromprtActivateAlarm(3);
				break;
			case 28:
				AsQuickSlotManager.Instance.PromprtActivateAlarm(4);
				break;
		}

		if (_nslot >= AsGameDefine.QUICK_SLOT_MAX)
			Debug.LogError("QuickSlot slot over[ index : " + _nslot);

		body_CS_QUICKSLOT_CHANGE packetdata = new body_CS_QUICKSLOT_CHANGE(_nslot, (int)_eType, _nValue);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendQuickslotChange [ _nslot : " + _nslot + " _nValue : " + _nValue + "eQUICKSLOT_TYPE : " + _eType);
	}

	static public void SendStoragePageSort(Int32 iPage)
	{
		body_CS_STORAGE_SORT packetdata = new body_CS_STORAGE_SORT((short)iPage);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendInvenPageSort [ page : " + iPage);
	}

	static public void SendItemEnchant(Int16 _nItemSlot, Int16 _nSoulStoneSlotIdx, byte _nEnchantPos)
	{
		if (true == isSendEnchant)
			return;

		isSendEnchant = true;

		body_CS_ITEM_ENCHANT packetdata = new body_CS_ITEM_ENCHANT(_nItemSlot, _nSoulStoneSlotIdx, _nEnchantPos);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);


		Debug.Log("SendItemEnchant [ _nItemSlot : " + _nItemSlot + " _nEnchantPos : " + _nEnchantPos + "_nSoulStoneIdx : " + _nSoulStoneSlotIdx);
	}

	static public void SendItemEnchantOut(short _nEquipSlot, byte _nEnchantPos)
	{
		if (true == isSendEnchant)
			return;

		isSendEnchant = true;

		body_CS_ITEM_ENCHANT_OUT packetData = new body_CS_ITEM_ENCHANT_OUT(_nEquipSlot, _nEnchantPos);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendItemEnchantOut[ _nEquipSlot: " + _nEquipSlot + " [ _nEnchantPos : " + _nEnchantPos);
	}

	static public void SendItemStrengthen(eITEM_STRENGTHEN_TYPE _eStrengthenType, Int16 _nInvenSlot, bool isUseCash)
	{
		if (true == isSendStrengthen)
			return;

		body_CS_ITEM_STRENGTHEN packetdata = new body_CS_ITEM_STRENGTHEN(_eStrengthenType, _nInvenSlot, isUseCash);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		isSendStrengthen = true;
		Debug.Log("SendItemStrengthen [ eITEM_STRENGTHEN_TYPE : " + _eStrengthenType + " _nInvenSlot : " + _nInvenSlot);
	}

	static public void SendEnchantItemMix( Int16 _nSlot1, Int16 _nSlot2 )
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_ITEM_MIX packetdata = new body_CS_ITEM_MIX( (int)eITEM_MIX_TYPE.eITEM_MIX_TYPE_SOUL_STONE, _nSlot1, _nSlot2 );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}
	
	static public void SendOptionItemMix( Int16 _nSlot1 )
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_ITEM_MIX packetdata = new body_CS_ITEM_MIX( (int)eITEM_MIX_TYPE.eITEM_MIX_TYPE_EQUIP_OPT, _nSlot1, 0 );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}
	
	static public void SendCosItemMix( Int16 _nSlot1, Int16 _nSlot2)
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_ITEM_MIX packetdata = new body_CS_ITEM_MIX( (int)eITEM_MIX_TYPE.eITEM_MIX_TYPE_COSTUME,  _nSlot1, _nSlot2 );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}

	static public void SendCosItemMixUp( Int16 _targetSlot, Int16 _nSlot1, Int16 _nSlot2 , Int16 _nSlot3)
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_COS_ITEM_MIX_UP packetdata = new body_CS_COS_ITEM_MIX_UP( _targetSlot , _nSlot1 , _nSlot2 , _nSlot3 );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}

	static public void SendCosItemMixUpgrade( Int16 _targetSlot, bool _isCash )
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_COS_ITEM_MIX_UPGRADE packetdata = new body_CS_COS_ITEM_MIX_UPGRADE( _targetSlot , _isCash );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}


	static public void SendDecompositionItemMix( Int16 _nSlot1 )
	{
		if( true == isSendItemMix )
			return;
		
		body_CS_ITEM_MIX packetdata = new body_CS_ITEM_MIX( (int)eITEM_MIX_TYPE.eITEM_MIX_TYPE_DECOMPOSITION,  _nSlot1, 0 );
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
		isSendItemMix = true;
	}


	/*
	* Send Packet : break object action
	* Packet Define : CS_OBJECT_BREAK
	*/
	static public void SendObjectBreak(Int32 iNpcIdx)
	{
		body_CS_OBJECT_BREAK packetdata = new body_CS_OBJECT_BREAK(iNpcIdx);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendObjectBreak [ iNpcIdx : " + iNpcIdx);
	}

	/*
	* Send Packet : Trap object action
	* Packet Define : CS_OBJECT_CATCH
	*/
	static public void SendObjectTrap(Int32 iNpcIdx)
	{
		body_CS_OBJECT_CATCH packetdata = new body_CS_OBJECT_CATCH(iNpcIdx);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("CS_OBJECT_CATCH [ iNpcIdx : " + iNpcIdx);
	}

	/*
	* Send Packet : Stepping object action
	* Packet Define : CS_OBJECT_JUMP
	*/
	static public void SendObjectJump(Int32 iNpcIdx)
	{
		body_CS_OBJECT_JUMP packetdata = new body_CS_OBJECT_JUMP(iNpcIdx);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendObjectJump [ iNpcIdx : " + iNpcIdx);
	}

	// < trade
	static public void SendTradeRequest(UInt16 nResponseSessionID)
	{
		body_CS_TRADE_REQUEST packetdata = new body_CS_TRADE_REQUEST(nResponseSessionID);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeResponse(bool bAccept, UInt16 nRequestSessionID)
	{
		body_CS_TRADE_RESPONSE packetdate = new body_CS_TRADE_RESPONSE(bAccept, nRequestSessionID);
		byte[] data = packetdate.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeCancel()
	{
		body_CS_TRADE_CANCEL packetdate = new body_CS_TRADE_CANCEL();
		byte[] data = packetdate.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeRegistrationItem(bool bAdd, Int32 invenSlot, Int32 tradeSlot, Int16 itemCount)
	{
		body_CS_TRADE_REGISTRATION_ITEM packetdate = new body_CS_TRADE_REGISTRATION_ITEM(bAdd, invenSlot, tradeSlot, itemCount);
		byte[] data = packetdate.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeRegistrationGold(UInt64 nGold)
	{
		body_CS_TRADE_REGISTRATION_GOLD packetdata = new body_CS_TRADE_REGISTRATION_GOLD(nGold);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeLock(bool bLock)
	{
		body_CS_TRADE_LOCK packetdata = new body_CS_TRADE_LOCK(bLock);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}

	static public void SendTradeOk()
	{
		body_CS_TRADE_OK packetdata = new body_CS_TRADE_OK();
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}
	// trade >

	#region -Quest-
	static public void SendAcceptQuest(int nQuestTableIdx)
	{
		body_CS_QUEST_START packet = new body_CS_QUEST_START(nQuestTableIdx);

		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendClearMapInto(int _qustID)
	{
		body_CS_QUEST_CLEAR_ENTER_MAP packet = new body_CS_QUEST_CLEAR_ENTER_MAP(_qustID);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendClearQuest(int nQuestTableIdx)
	{
		body_CS_QUEST_CLEAR packet = new body_CS_QUEST_CLEAR(nQuestTableIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);

		Debug.LogWarning("Send Clear Qeust = " + nQuestTableIdx);
	}

	static public void SendImmediatelyClearQuest(int nQuestTableIdx)
	{
		body_CS_QUEST_CLEAR_NOW packet = new body_CS_QUEST_CLEAR_NOW(nQuestTableIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
		Debug.LogWarning("Send Immediately Clear Qeust = " + nQuestTableIdx);
	}

	static public void SendDropQuest(int nQuestTableIdx)
	{
		body_CS_QUEST_DROP packet = new body_CS_QUEST_DROP(nQuestTableIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendCompleteQuest(int nQuestTableIdx)
	{
		body_CS_QUEST_COMPLETE packet = new body_CS_QUEST_COMPLETE(nQuestTableIdx, 255);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendCompleteQuest(int nQuestTableIdx, byte nSelectItemIdx)
	{
		body_CS_QUEST_COMPLETE packet = new body_CS_QUEST_COMPLETE(nQuestTableIdx, nSelectItemIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendCompleteWantedQuest(int nQuestTableIdx)
	{
		body_CS_WANTED_COMPLETE packet = new body_CS_WANTED_COMPLETE(nQuestTableIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
		Debug.LogWarning("Send Wanted quest complete = " + nQuestTableIdx);
	}

	static public void SendTalkWithNPC(int _npcSpwanIdx)
	{
		body_CS_QUEST_CLEAR_TALK packet = new body_CS_QUEST_CLEAR_TALK(_npcSpwanIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendRequestDailyQuests()
	{
		body_CS_QUEST_DAILY_LIST packet = new body_CS_QUEST_DAILY_LIST();
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendQuestWarp(int _questID, int _mapID, Vector3 _position)
	{
		body_CS_QUEST_WARP packet = new body_CS_QUEST_WARP(_questID, _mapID, _position);

		byte[] data = packet.ClassToPacketBytes();
		Send(data);

		AsCommonSender.isSendWarp = true;
	}

	static public void SendClearOpneUI(OpenUIType _type)
	{
		body_CS_QUEST_CLEAR_OPEN_UI packet = new body_CS_QUEST_CLEAR_OPEN_UI(_type);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendQuestInfo(int _questTableIdx, int _cond)
	{
		body_CS_QUEST_INFO packet = new body_CS_QUEST_INFO(_questTableIdx, _cond);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}
	#endregion

	#region -npc store
	static public void BuyNpcStoreItem(int _npcId, int _shopItemSlot, int _itemCount)
	{
		body_CS_SHOP_ITEMBUY packetdata = new body_CS_SHOP_ITEMBUY(_npcId, _shopItemSlot, _itemCount);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}

	static public void SellInvenItem(int _npcId, int _invelSlot, int _itemCount)
	{
		body_CS_SHOP_ITEMSELL packetdata = new body_CS_SHOP_ITEMSELL(_npcId, _invelSlot, _itemCount);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}

	static public void RequestShopInfo(int _sessionNpcID = 0)
	{
		if (_sessionNpcID == 0)
		{
			if (AsHudDlgMgr.Instance.IsOpenNpcStore == true)
				_sessionNpcID = AsHudDlgMgr.Instance.npcStore.GetNpcSessionID();
			else
				return;
		}

		body_CS_SHOP_INFO packetdata = new body_CS_SHOP_INFO(_sessionNpcID);
		byte[] data = packetdata.ClassToPacketBytes();
		Send(data);
	}
	#endregion

	#region -cash store
	static public void SendRequestChargeItemList()
	{
		body_CS_CHARGE_ITEMLIST packetData = null;

		if (Application.platform == RuntimePlatform.Android)
			packetData = new body_CS_CHARGE_ITEMLIST(eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_ANDROID);
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			packetData = new body_CS_CHARGE_ITEMLIST(eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_APPLE);
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer)
			packetData = new body_CS_CHARGE_ITEMLIST(eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_ANDROID);
		else
		{
			UnityEngine.Debug.LogWarning("[sendRequestChargeItemList] Not support platform = " + Application.platform.ToString());
			return;
			//packetData = new body_CS_CHARGE_ITEMLIST(eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_ANDROID);
		}

		if (packetData != null)
		{
			byte[] data = packetData.ClassToPacketBytes();
			Send(data);
		}
		else
			Debug.LogWarning("sendRequestChargeItemList] packet data is null");
	}

	static public void SendRequestBuyCashItem(int _shopItemSlot, int _itemCount)
	{
		body_CS_CASHSHOP_ITEMBUY packetData = new body_CS_CASHSHOP_ITEMBUY(_shopItemSlot, _itemCount);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendRequestBuyChargeItem(int _shopItemSlot, string _iapProductID)
	{
		// default
		body_CS_CHARGE_BUY packetData = new body_CS_CHARGE_BUY((byte)eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_NOTHING, 0, _shopItemSlot, _iapProductID);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendRequestBuyChargeItem(eCHARGECOMPANYTYPE _companyType, int _shopItemSlot, string _iapProductID, UInt32 _iapUniqueKey)
	{
		// default
		body_CS_CHARGE_BUY packetData = new body_CS_CHARGE_BUY((byte)_companyType, _iapUniqueKey, _shopItemSlot, _iapProductID);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendRequestAndroidPublicKey(bool _mode)
	{
		body_CS_CHARGE_ANDROIDPUBLICKEY packetData = new body_CS_CHARGE_ANDROIDPUBLICKEY(_mode);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public bool SendRequestFindGiftFirend(string _szNickname)
	{
		body_CS_CHARGE_GIFT_FIND packetData = new body_CS_CHARGE_GIFT_FIND();

		if (packetData.Initilize(_szNickname) == false)
			return false;

		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		return true;
	}

	static public void SendRequestBuyCashItemJpn(int _shopItemSlot, int _itemTableIdx, int _itemCount, int _invenSlot)
	{
		body_CS_CASHSHOP_JAP_ITEMBUY packetData = new body_CS_CASHSHOP_JAP_ITEMBUY(_shopItemSlot, _itemTableIdx, _itemCount, _invenSlot);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendRequestCashInfoJpn()
	{
		body_CS_CASHSHOP_JAP_INFO packetData = new body_CS_CASHSHOP_JAP_INFO();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}
	#endregion

	static public void SendWayPoint()
	{
		AsUserInfo.Instance.isReceiveWaypointList = false;
		body_CS_WAYPOINT packet = new body_CS_WAYPOINT();
		byte[] data = packet.ClassToPacketBytes();
		Send(data);
	}

	static public void SendWayPointActive(int iWarpIdx)
	{
		body_CS_WAYPOINT_ACTIVE packet = new body_CS_WAYPOINT_ACTIVE(iWarpIdx);
		byte[] data = packet.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendWayPointActive [ warpIdx : " + iWarpIdx);
	}

	#region -Social
	static public void SendSocialInfo(uint nUserUniqKey)
	{
		body_CS_SOCIAL_INFO packetData = new body_CS_SOCIAL_INFO(nUserUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendSocialInfo  nUserUniqKey: " + nUserUniqKey.ToString());
#endif
	}

	static public void SendSocialHistoryRegister(uint nCharKey, int nSubTitle, int ePlatformType)
	{
		body_CS_SOCIAL_HISTORY_REGISTER packetData = new body_CS_SOCIAL_HISTORY_REGISTER(nCharKey, nSubTitle, ePlatformType);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendSocialHistoryRegister  nCharKey: " + nCharKey.ToString());
#endif
	}

	static public void SendSocialNotice(string strNotice)
	{
		body_CS_SOCIAL_NOTICE packetData = new body_CS_SOCIAL_NOTICE(strNotice);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
#if _SOCIAL_LOG_
		Debug.Log("SendSocialNotice  : " + strNotice);
#endif
	}

	static public void SendSocialUiScroll(uint userUniqKey, eSOCIAL_UI_TYPE type, int page, bool connected)
	{
		body_CS_SOCIAL_UI_SCROLL packetData = new body_CS_SOCIAL_UI_SCROLL(userUniqKey, type, page, connected);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendSocialUiScroll  userUniqKey: " + userUniqKey.ToString() + "eSOCIAL_UI_TYPE : " + type.ToString() + "page : " + page.ToString() + connected.ToString());
#endif
	}

	static public void SendFriendList(uint nUserUniqKey, bool isConnect)
	{
		body_CS_FRIEND_LIST packetData = new body_CS_FRIEND_LIST(nUserUniqKey, isConnect);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendFriendList  nUserUniqKey: " + nUserUniqKey.ToString() + "isConnect:" + isConnect);
#endif
	}

	static public void SendFriendInvite(string name)
	{
		if (null != AsSocialManager.Instance.SocialData.GetBlockOutUserByName(AsUtil.GetRealString(name)))
		{
			//친구 초대 실패( 차단)
			AsChatManager.Instance.InsertChat(AsTableManager.Instance.GetTbl_String(161), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(AsTableManager.Instance.GetTbl_String(161));
			return;
		}
		body_CS_FRIEND_INVITE packetData = new body_CS_FRIEND_INVITE(name);

		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendFriendJoin(string name, uint uniqKey, eFRIEND_JOIN_TYPE joinType)
	{
		body_CS_FRIEND_JOIN packetData = new body_CS_FRIEND_JOIN(name, uniqKey, (int)joinType);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendFriendClear(uint nUserKey)
	{
		body_CS_FRIEND_CLEAR packetData = new body_CS_FRIEND_CLEAR(nUserKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendFriendClear  nUserKey : " + nUserKey);
#endif
	}

	static public void SendFriendHello(uint uniqKey)
	{
		body_CS_FRIEND_HELLO packetData = new body_CS_FRIEND_HELLO(uniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
#if _SOCIAL_LOG_
		Debug.Log("SendFriendHello  nUserUniqKey: " + uniqKey.ToString());
#endif
	}

	static public void SendFriendReCall(uint uniqKey)
	{
		body_CS_FRIEND_RECALL packetData = new body_CS_FRIEND_RECALL(uniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
#if _SOCIAL_LOG_
		Debug.Log("SendFriendReCall  nUserUniqKey: " + uniqKey.ToString());
#endif
	}

	static public void SendFriendRandom()
	{
		body_CS_FRIEND_RANDOM packetData = new body_CS_FRIEND_RANDOM();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
#if _SOCIAL_LOG_
		Debug.Log("SendFriendRandom  ");
#endif
	}

	static public void SendFriendWarp(uint userUniqKey)
	{
		body_CS_FRIEND_WARP packetData = new body_CS_FRIEND_WARP(userUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendFriendWarp userUniqKey: " + userUniqKey.ToString());
#endif
	}

	static public void SendFrinedCondition()
	{
		body_CS_FRIEND_CONDITION packetData = new body_CS_FRIEND_CONDITION();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("body_CS_FRIEND_CONDITION userUniqKey: ");
#endif
	}

	static public void SendBlockOutInsert(string name)
	{
		body_CS_BLOCKOUT_INSERT packetData = new body_CS_BLOCKOUT_INSERT(name);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendBlockOutInsert : " + name);
#endif

	}

	static public void SendBlockOutDelete(uint userUniqKey)
	{
		body_CS_BLOCKOUT_DELETE packetData = new body_CS_BLOCKOUT_DELETE(userUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendBlockOutDelete " + userUniqKey);
#endif
	}

	static public void SendSocialItemBuy(int shopItemSlot, ushort cnt)
	{
		body_CS_SOCIAL_ITEMBUY packetData = new body_CS_SOCIAL_ITEMBUY(shopItemSlot, cnt);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendSocialItemBuy shopItemSlot: " + shopItemSlot.ToString() + " cnt : " + cnt.ToString());
#endif
	}

	static public void SendGameInviteList()
	{
		body_CS_GAME_INVITE_LIST packetData = new body_CS_GAME_INVITE_LIST();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendGameInviteList userUniqKey: ");
#endif
	}

	static public void SendGameInvite(bool check, int nPlatform, string strKeyword)
	{
		body_CS_GAME_INVITE packetData = new body_CS_GAME_INVITE(check, nPlatform, strKeyword);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("check" + check.ToString() + "SendGameInvite nPlatform: " + nPlatform + "strKeyword : " + strKeyword);
#endif
	}

	static public void SendGameInviteReward(int nPlatform, int nId)
	{
		body_CS_GAME_INVITE_REWARD packetData = new body_CS_GAME_INVITE_REWARD(nPlatform, nId);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendGameInviteReward nPlatform: " + nPlatform);
#endif
	}

	static public void SendFriendConnectReuqest(uint userUniqKey)
	{
		body_CS_FRIEND_CONNECT_REUQEST packetData = new body_CS_FRIEND_CONNECT_REUQEST(userUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

#if _SOCIAL_LOG_
		Debug.Log("SendFriendConnectReuqest userUniqKey :  " + userUniqKey);
#endif
	}


	#endregion //-Social

	#region -BackgroundProc
	private static bool backgroundState = false;
	static public void SendBackgroundProc(bool isBackground)
	{
		if (backgroundState == isBackground)
			return;

		body_CS_BACKGROUND_PROC packetData = new body_CS_BACKGROUND_PROC(isBackground);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		backgroundState = isBackground;
	}
	#endregion -BackgroundProc

	#region -ItemProduction
	static public void SendItemProductInfo()
	{
		body_CS_ITEM_PRODUCT_INFO packetData = new body_CS_ITEM_PRODUCT_INFO();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductTechniqueRegister(eITEM_PRODUCT_TECHNIQUE_TYPE eType)
	{
		body_CS_ITEM_PRODUCT_TECHNIQUE_REGISTER packetData = new body_CS_ITEM_PRODUCT_TECHNIQUE_REGISTER((Int32)eType);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductProgress(bool isProgress)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		if (true == isSendProductProgress)
			return;

		body_CS_ITEM_PRODUCT_PROGRESS packetData = new body_CS_ITEM_PRODUCT_PROGRESS(isProgress);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		isSendProductProgress = true;
		Debug.Log("SendItemProductProgress [ progress : " + isProgress);
	}

	static public void SendItemProduct(byte _nProductSlot, int _nRecipeIndex)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		if (true == isSendItemProductRegister)
			return;

		body_CS_ITEM_PRODUCT_REGISTER packetData = new body_CS_ITEM_PRODUCT_REGISTER(_nProductSlot, _nRecipeIndex);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		isSendItemProductRegister = true;
	}

	static public void SendItemProductCancel(byte _nProductSlot)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_CANCEL packetData = new body_CS_ITEM_PRODUCT_CANCEL(_nProductSlot);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductReceive(byte _nProductSlot)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_RECEIVE packetData = new body_CS_ITEM_PRODUCT_RECEIVE(_nProductSlot);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductCashDirect(byte _nProductSlot)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_CASH_DIRECT packetData = new body_CS_ITEM_PRODUCT_CASH_DIRECT(_nProductSlot);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductCashSlotOpen()
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_CASH_SLOT_OPEN packetData = new body_CS_ITEM_PRODUCT_CASH_SLOT_OPEN();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductCashTechniqueOpen(eITEM_PRODUCT_TECHNIQUE_TYPE eType)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN packetData = new body_CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN((int)eType);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendItemProductCashLevelUp(eITEM_PRODUCT_TECHNIQUE_TYPE eType)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_ITEM_PRODUCT_CASH_LEVEL_UP packetData = new body_CS_ITEM_PRODUCT_CASH_LEVEL_UP((int)eType);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}
	#endregion

	#region -Collect
	static public bool SendCollectRequest(Int32 collectNpcIdx, eCOLLECT_STATE eState)
	{
		if (true == isSendCollectRequest)
		{
			return false;
		}

		body_CS_COLLECT_REQUEST packetData = new body_CS_COLLECT_REQUEST(collectNpcIdx, eState);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendCollectRequest [ npc index : " + collectNpcIdx + " state : " + eState);
		isSendCollectRequest = true;
		return true;
	}
	#endregion

	static public void SendItemInventoryOpen()
	{
		if (true == isSendItemInventroyOpen)
		{
			return;
		}

		body_CS_ITEM_INVENTORY_OPEN packetData = new body_CS_ITEM_INVENTORY_OPEN();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendItemInventoryOpen");
		isSendItemInventroyOpen = true;
	}

	static public void SendOtherUserInfo(int _eInfoType, ushort _nSessionIdx, uint _nCharUniqKey, uint _nUserUniqKey)
	{
		body_CS_OTHER_INFO packetData = new body_CS_OTHER_INFO(_eInfoType, _nSessionIdx, _nCharUniqKey, _nUserUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendOtherInfo");
	}


#if USE_OLD_COSTUME
	static public void SendConstumeOnOff( bool _isCostumeOff)
	{
		if( GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_COSTUME_ONOFF  packetData = new body_CS_COSTUME_ONOFF( _isCostumeOff);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		Debug.Log( "SendConstumeOnOff is : " + _isCostumeOff);
	}
#else
	static public void SendConstumeOnOff(int _isCostumeOff)
	{
		if (GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_COSTUME_ONOFF packetData = new body_CS_COSTUME_ONOFF(_isCostumeOff);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendConstumeOnOff is : " + _isCostumeOff);
	}
#endif


	static public void SendGameReviewEvent(bool _review)
	{
		body_CS_GAME_REVIEW packetData = new body_CS_GAME_REVIEW(_review);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendGameReviewEvent is : " + _review);
	}

	static public void SendNpcEventList()
	{
		body_CS_EVENT_LIST packetData = new body_CS_EVENT_LIST();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		Debug.Log("SendNpcEventList ");
	}

	static public void SendLevelActiveWaypoint()
	{
		body_CS_WAYPOINT_LEVELACTIVE packetData = new body_CS_WAYPOINT_LEVELACTIVE();
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}

	static public void SendShopUseLessItemSell(int npcidx)
	{
		if (true == isSendShopUseLessItemSell)
			return;

		body_CS_SHOP_USELESS_ITEMSELL packetData = new body_CS_SHOP_USELESS_ITEMSELL(npcidx);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);

		isSendShopUseLessItemSell = true;
		Debug.Log("SendShopUseLessItemSell [ npc id :" + npcidx);
	}

    static public void SendGetApRankReward()
    {
        body_CS_AP_RANK_REWARD packetData = new body_CS_AP_RANK_REWARD();
        byte[] data = packetData.ClassToPacketBytes();
        Send(data);
    }

	#region In game event
	static public void SendRewardInGameEvent(int _eventID, byte _rewardIdx)
	{
		body_CS_EVENT_QUEST_REWARD packetData = new body_CS_EVENT_QUEST_REWARD(_eventID, _rewardIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send(data);
	}
	#endregion
}

#region - struct -
public struct SavedMoveInfo //$yde
{
	public eMoveType nMoveType;
	public Vector3 sCurPosition;
	public Vector3 sDestPosition;

	public SavedMoveInfo(eMoveType _moveType, Vector3 _curPosition, Vector3 _destPosition)
	{
		sCurPosition = _curPosition;
		sDestPosition = _destPosition;

		nMoveType = _moveType;
	}
}
#endregion

