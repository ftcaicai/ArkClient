#define _USE_PARTY_WARP
#define _STANCE
#define NEW_DELEGATE_IMAGE
//#define USE_OLD_COSTUME
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



//--------------------------------------------------------------------------------------------------
/* Common Packet Define */
//--------------------------------------------------------------------------------------------------

enum PROTOCOL_CS : byte
{
	CS_LIVE,
	SC_LIVE_ECHO,
	CS_BACKGROUND_PROC,

	CS_CHAR_MOVE,
	SC_CHAR_MOVE,
	SC_OTHER_CHAR_APPEAR,
	SC_OTHER_CHAR_DISAPPEAR,
	CS_CHAR_SKILL,
	SC_CHAR_SKILL,
	SC_CHAR_SKILL_EFFECT,
#if _STANCE
	CS_CHAR_SKILL_STANCE,
	SC_CHAR_SKILL_STANCE,
	SC_CHAR_BUFF_ACCURE_INFO,
	SC_CHAR_SKILL_USE_ADD,
#endif
	
	SC_CHAR_SKILL_SOULSTONE,
	SC_CHAR_SKILL_PVP_AGGRO,
	
	SC_NPC_APPEAR,
	SC_NPC_DISAPPEAR,
	SC_NPC_AWAKE,
	SC_NPC_MOVE,
	SC_NPC_SKILL,
	SC_NPC_SKILL_EFFECT,
	SC_NPC_COMBAT_FREE,
	SC_NPC_HELP,
	SC_NPC_STATUS,
	SC_NPC_APPEAR_NOTIFY,

	CS_PICKUP_ITEM,
	SC_PICKUP_ITEM_RESULT,
	SC_DROPITEM_APPEAR,
	SC_DROPITEM_DISAPPEAR,
	SC_DROPITEM_UPDATE,

	CS_ITEM_USE,
	CS_ITEM_MOVE,
	CS_ITEM_REMOVE,
	CS_ITEM_PAGE_SORT,

	SC_ITEM_INVENTORY,
	CS_ITEM_INVENTORY_OPEN,
	SC_ITEM_INVENTORY_OPEN,
	SC_ITEM_SLOT,
	SC_ITEM_VIEW_SLOT,
	SC_ITEM_RESULT,

	CS_STORAGE_LIST,
	SC_STORAGE_LIST,
	CS_STORAGE_COUNT_UP,
	SC_STORAGE_COUNT_UP_RESULT,
	CS_STORAGE_MOVE,
	SC_STORAGE_MOVE_RESULT,
	CS_STORAGE_SORT,
	SC_STORAGE_SLOT,

	SC_QUICKSLOT_LIST,
	CS_QUICKSLOT_CHANGE,

	SC_COOL_TIME_LIST,

	CS_ATTR_TOTAL,
	SC_ATTR_TOTAL,
	SC_ATTR_CHANGE,
	SC_ATTR_HP,
	SC_ATTR_MP,
	SC_ATTR_EXP,
	SC_ATTR_LEVELUP,

	SC_NPC_ATTR_HP,
	SC_NPC_ATTR_CHANGE,

	SC_GOLD_CHANGE,
	SC_SPHERE_CHANGE,
	SC_SOCIALPOINT_CHANGE,
	
	#region -SkillReset
	SC_SKILLBOOK_LIST,
	#endregion
	SC_SKILL_LIST,
	CS_SKILL_LEARN,
	CS_SKILL_RESET,
	SC_SKILL_LEARN_RESULT,
	SC_SKILL_RESET_RESULT,

	SC_CHAR_BUFF,
	SC_CHAR_DEBUFF,
	SC_CHAR_DEBUFF_RESIST,

	SC_NPC_BUFF,
	SC_NPC_DEBUFF,

	CS_WARP,
	SC_WARP_RESULT,
	CS_WARP_END,
	CS_WARP_XZ,
	SC_WARP_XZ_RESULT,
	CS_WARP_XZ_END,
	CS_WARP_CANCEL,

	CS_GOTO_TOWN,
	SC_GOTO_TOWN_RESULT,
	CS_GOTO_TOWN_END,

	CS_RESURRECTION,
	SC_RESURRECTION,

	CS_WAYPOINT,
	SC_WAYPOINT,
	CS_WAYPOINT_ACTIVE,
	SC_WAYPOINT_ACTIVE_RESULT,
	CS_WAYPOINT_LEVELACTIVE,
	SC_WAYPOINT_LEVELACTIVE_RESULT,

//	CS_CHEAT_HP,
//	CS_CHEAT_MP,
//	CS_CHEAT_LEVELUP,
//	CS_CHEAT_DEATH,
//	CS_CHEAT_NPCALL_REGEN,
//	CS_CHEAT_NPCALL_DELETE,

	CS_PARTY_LIST,
	SC_PARTY_LIST,
	CS_PARTY_DETAIL_INFO,
	SC_PARTY_DETAIL_INFO,
	CS_PARTY_CREATE,
	SC_PARTY_CREATE_RESULT,
	CS_PARTY_CAPTAIN,
	SC_PARTY_CAPTAIN,
	CS_PARTY_SETTING,
	SC_PARTY_SETTING_RESULT,
	SC_PARTY_SETTING_INFO,
	CS_PARTY_INVITE,
	SC_PARTY_INVITE_RESULT,
	SC_PARTY_INVITE,
	CS_PARTY_JOIN,
	SC_PARTY_JOIN_RESULT,
	CS_PARTY_JOIN_REQUEST,		    //<-- 2013.11.29.
	SC_PARTY_JOIN_REQUEST_NOTIFY,	//<-- 2013.11.29.
	CS_PARTY_JOIN_REQUEST_ACCEPT,	//<-- 2013.11.29.
	CS_PARTY_EXIT,
	SC_PARTY_EXIT_RESULT,
	CS_PARTY_BANNED_EXIT,
	SC_PARTY_BANNED_EXIT_RESULT,
	CS_PARTY_SEARCH,	    //	<--  2014.01.10.
	SC_PARTY_SEARCH_RESULT,	//	<--  2014.01.10.	
	SC_PARTY_USER_ADD,
	SC_PARTY_USER_DEL,
	SC_PARTY_USER_INFO,
	SC_PARTY_USER_BUFF,
	SC_PARTY_USER_DEBUFF,
	CS_PARTY_USER_POSITION,
	SC_PARTY_USER_POSITION,
	SC_PARTY_DICE_ITEM_INFO,
	CS_PARTY_DICE_SHAKE,
	SC_PARTY_DICE_SHAKE,
	SC_PARTY_DICE_SHAKE_RESULT,
	CS_PARTY_NOTICE_ONOFF,
	SC_PARTY_NOTICE_ONOFF_NOTIFY,
#if _USE_PARTY_WARP
	CS_PARTY_WARP_XZ,		     //  <--2014.01.21.
	SC_PARTY_WARP_XZ_RESULT,	 //  <--2014.01.21.
	CS_PARTY_WARP_XZ_END,		 //  <--2014.01.21.
#endif

	SC_QUEST_RUNNING_LIST,
	SC_QUEST_COMPLETE_LIST,
	CS_QUEST_START,
	SC_QUEST_START_RESULT,
	CS_QUEST_CLEAR,
	SC_QUEST_CLEAR_RESULT,
	CS_QUEST_DROP,
	SC_QUEST_DROP_RESULT,
	CS_QUEST_COMPLETE,
	SC_QUEST_COMPLETE_RESULT,
	SC_QUEST_NPC_KILL_RESULT,
	//SC_QUEST_GET_ITEM_RESULT,
	CS_QUEST_INFO,
	SC_QUEST_INFO,			

	CS_QUEST_CLEAR_TALK,
	SC_QUEST_CLEAR_TALK_RESULT,
	CS_QUEST_CLEAR_ENTER_MAP,
	SC_QUEST_CLEAR_ENTER_MAP_RESULT,
	CS_QUEST_CLEAR_NOW,
	SC_QUEST_CLEAR_NOW_RESULT,

	CS_QUEST_CLEAR_OPEN_UI,
	SC_QUEST_CLEAR_OPEN_UI_RESULT,

	SC_QUEST_FAIL_RESULT,
	CS_QUEST_DAILY_LIST,
	SC_QUEST_DAILY_LIST_RESULT,
	SC_QUEST_GROUP_LIST,
	CS_QUEST_WARP,
	SC_QUEST_WARP_RESULT,

	SC_QUEST_DAILY_RESET_RESULT,
	SC_TUTORIAL_RESULT,

	CS_OBJECT_CATCH,
	SC_OBJECT_CATCH_RESULT,
	SC_OBJECT_CATCH,
	CS_OBJECT_CATCH_TIME,

	CS_OBJECT_JUMP,
	SC_OBJECT_JUMP_RESULT,
	SC_OBJECT_JUMP,

	CS_OBJECT_BREAK,
	SC_OBJECT_BREAK_RESULT,
	SC_OBJECT_BREAK,

	CS_TRADE_REQUEST,
	SC_TRADE_REQUEST,
	CS_TRADE_RESPONSE,
	SC_TRADE_RESPONSE,
	CS_TRADE_REGISTRATION_ITEM,
	SC_TRADE_REGISTRATION_ITEM,
	CS_TRADE_REGISTRATION_GOLD,
	SC_TRADE_REGISTRATION_GOLD,
	CS_TRADE_LOCK,
	SC_TRADE_LOCK,
	CS_TRADE_OK,
	SC_TRADE_OK,
	CS_TRADE_CANCEL,
	SC_TRADE_CANCEL,

	CS_POST_SEND,
	SC_POST_SEND_RESULT,
	SC_POST_NEWPOST,
	CS_POST_LIST,
	CS_POST_PAGE,
	SC_POST_LIST_RESULT,
	CS_POST_ITEM_RECEIVE,
	SC_POST_ITEM_RECEIVE_RESULT,
	CS_POST_GOLD_RECEIVE,
	SC_POST_GOLD_RECEIVE_RESULT,
	CS_POST_READ,
	CS_POST_DELETE,
	//SC_POST_ERROR,
	CS_POST_ADDRESS_BOOK,
	SC_POST_ADDRESS_BOOK,

	CS_CHAT_LOCAL,
	SC_CHAT_LOCAL_RESULT,
	CS_CHAT_PRIVATE,
	SC_CHAT_PRIVATE_RESULT,
	CS_CHAT_PARTY,
	SC_CHAT_PARTY_RESULT,
	CS_CHAT_GUILD,
	SC_CHAT_GUILD_RESULT,
	CS_CHAT_MAP,
	SC_CHAT_MAP_RESULT,
	CS_CHAT_WORLD,
	SC_CHAT_WORLD_RESULT,
	CS_CHAT_EMOTICON,
	SC_CHAT_EMOTICON_RESULT,
	SC_CHAT_SYSTEM_RESULT,

	CS_SHOP_INFO,
	SC_SHOP_INFO,
	CS_SHOP_ITEMBUY,
	SC_SHOP_ITEMBUY,
	CS_SHOP_ITEMSELL,
	SC_SHOP_ITEMSELL,
	CS_SHOP_USELESS_ITEMSELL,
	SC_SHOP_USELESS_ITEMSELL,

	CS_CHARGE_ITEMLIST,
	SC_CHARGE_ITEMLIST,
	CS_CHARGE_BUY,
	SC_CHARGE_BUY,
	CS_CHARGE_GIFT_FIND,
	SC_CHARGE_GIFT_FIND,
	CS_CASHSHOP_ITEMBUY,
	SC_CASHSHOP_ITEMBUY,
	CS_CHARGE_ANDROIDPUBLICKEY,
	SC_CHARGE_ANDROIDPUBLICKEY,
    SC_ATTR_AP,
}


//--------------------------------------------------------------------------------------------------
/* Create Common Packet */
//--------------------------------------------------------------------------------------------------

#region - character -

#region - other char appear & disappear -
public class AS_SC_OTHER_CHAR_APPEAR_1 : AsPacketHeader
{
	public Int32 nCharCnt;
	public AS_SC_OTHER_CHAR_APPEAR_2[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharCnt
		byte[] charCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCharCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, charCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( charCnt, 0));
		index += sizeof( Int32);

		body = new AS_SC_OTHER_CHAR_APPEAR_2[nCharCnt];
		for( int i = 0; i < nCharCnt; i++)
		{
			body[i] = new AS_SC_OTHER_CHAR_APPEAR_2();
	
			byte[] tmpData = new byte[AS_SC_OTHER_CHAR_APPEAR_2.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_SC_OTHER_CHAR_APPEAR_2.size;
		}
	}
}


public class AS_SC_OTHER_CHAR_APPEAR_2 : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public byte[] szGuildName = new byte[AsGameDefine.MAX_GUILD_NAME_LEN + 1];
	public UInt32 nCharUniqKey;//73
#if !NEW_DELEGATE_IMAGE
	public eGENDER eUserGender;
#endif
	public eGENDER eGender;
	public Int32 eRace;
	public Int32 eClass;//85
	public int nHair;
	public int nHairColor;
	#region -Designation
	public Int32 nSubTitleTableIdx;
	public bool bSubTitleHide;
	#endregion
	public Int32 nLevel;//89
	public Int32 nMoveSpeed;
	public Int32 nAtkSpeed;
	public Single fHpMax;
	public Single fHpCur;//97
	public Vector3 sCurPosition;
	public Vector3 sDestPosition;
	public int nMoveType;//125	
#if USE_OLD_COSTUME	
	public bool bCostumeOnOff;
#else
	public int bCostumeOnOff;
#endif	
	public sITEMVIEW sNormalItemVeiw_1;
	public sITEMVIEW sNormalItemVeiw_2;
	public sITEMVIEW sNormalItemVeiw_3;
	public sITEMVIEW sNormalItemVeiw_4;
	public sITEMVIEW sNormalItemVeiw_5;
	public sITEMVIEW sCosItemView_1;
	public sITEMVIEW sCosItemView_2;
	public sITEMVIEW sCosItemView_3;
	public sITEMVIEW sCosItemView_4;
	public sITEMVIEW sCosItemView_5;
	public sITEMVIEW sCosItemView_6;
	public sITEMVIEW sCosItemView_7;
	public bool bProgress;
	public Int32 nCollectIdx;
	public Int32 nPartyIdx;
	public bool bPartyNotice;
	public byte[] szPartyNotice = new byte[AsGameDefine.ePARTYNOTICE+1];

	#region -GMMark
	public bool bIsGM;
	#endregion
	public bool bHide;
	//public Int32 nPvpPoint;
	public UInt32 nYesterdayPvpRank;
	public Int32 nYesterdayPvpPoint;
	public UInt32 nYesterdayPvpRankRate;
#if !NEW_DELEGATE_IMAGE
	public Int32 nRankPoint;
#endif
	
	//$yde
	public bool bCriticalChance;
	public bool bDodgeChance;
	public Int32 nPetTableIdx;
	public byte[] szPetName = new byte[AsGameDefine.ePET_NAME+1];
	public sITEMVIEW sPetItem;
//	public Int32 nPetLevel;
#if NEW_DELEGATE_IMAGE
	public Int32 nDelegateImageTableIndex;
#endif
	
	public static int size
	{
		get
		{
			return sizeof( UInt16) // 2
				+ ( sizeof(byte) * ( AsGameDefine.MAX_NAME_LEN + 1)) // 25
				+ ( sizeof(byte) * ( AsGameDefine.MAX_GUILD_NAME_LEN + 1)) //25
				+ sizeof( UInt32)
#if !NEW_DELEGATE_IMAGE
				+ sizeof( eGENDER)
#endif
				+ sizeof( eGENDER) + sizeof( Int32) //12
				+ sizeof( Int32) + sizeof( int) + sizeof( int) + sizeof( Int32) + sizeof( Int32) + sizeof( Int32) //24
				+ sizeof( Int32) + sizeof( Single) + sizeof( Single) + sizeof( float) * 3 + sizeof( float) * 3 // 36
				+ sizeof( int) // 4
				+ sizeof( bool)
				+ ( sITEMVIEW.size * AsGameDefine.ITEM_SLOT_VIEW_COUNT) + ( sITEMVIEW.size * AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT)// + 1/*$yde*/ // 60
				+ 1 + 4 + 1

				+ sizeof( int) + 1 + ( sizeof(byte) * ( AsGameDefine.ePARTYNOTICE+1)) // 46

				+ sizeof( bool)
#if USE_OLD_COSTUME	
				+ 1
#else
				+ sizeof(Int32)
#endif
				+ sizeof( UInt32)
				+ sizeof( Int32)
				+ sizeof( UInt32)
#if !NEW_DELEGATE_IMAGE
				+ sizeof( Int32)
#endif
				//$yde
				+ sizeof( bool)
				+ sizeof( bool)
				+ sizeof( Int32)
				+ AsGameDefine.ePET_NAME + 1
				+ sITEMVIEW.size
//				+ sizeof(Int32)
#if NEW_DELEGATE_IMAGE
				+ sizeof(Int32)
#endif
				;
		}
	}
}


public class AS_SC_OTHER_CHAR_DISAPPEAR_1 : AsPacketHeader
{
	public Int32 nCharCnt;
	public AS_SC_OTHER_CHAR_DISAPPEAR_2[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharCnt
		byte[] charCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCharCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, charCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( charCnt, 0));
		index += sizeof( Int32);

		body = new AS_SC_OTHER_CHAR_DISAPPEAR_2[nCharCnt];
		for( int i = 0; i < nCharCnt; i++)
		{
			body[i] = new AS_SC_OTHER_CHAR_DISAPPEAR_2();
	
			byte[] tmpData = new byte[AS_SC_OTHER_CHAR_DISAPPEAR_2.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_SC_OTHER_CHAR_DISAPPEAR_2.size;
		}
	}
}


public class AS_SC_OTHER_CHAR_DISAPPEAR_2 : AsPacketHeader
{
	public static int size = 6;
	
	public UInt16 nSessionIdx;//2
	public UInt32 nCharUniqKey;
}
#endregion


#region - npc appear & disappear -
public class AS_SC_NPC_APPEAR_1 : AsPacketHeader
{
	public Int32 nNpcCnt;
	public AS_SC_NPC_APPEAR_2[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nNpcCnt
		byte[] npcCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nNpcCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, npcCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( npcCnt, 0));
		index += sizeof( Int32);

		body = new AS_SC_NPC_APPEAR_2[nNpcCnt];
		for( int i = 0; i < nNpcCnt; i++)
		{
			body[i] = new AS_SC_NPC_APPEAR_2();

			byte[] tmpData = new byte[AS_SC_NPC_APPEAR_2.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_SC_NPC_APPEAR_2.size;
		}
	}
}


public class AS_SC_NPC_APPEAR_2 : AsPacketHeader
{
	public static int size = 60 + 4;

	public Int32 nNpcIdx;//4

	public Int32 nNpcTableIdx;
	public Int32 nNpcGroupIdx;
	public Int32 nNpcLinkIdx;//16

	public Int32 nNameHead;//20

	public float nHpMax;
	public float nHpCur;
	public float nMpMax;
	public float nMpCur;//36

	public Int32 nAtkSpeed;//40

	public Vector3 sCurPosition;//52
	public float fCurRotate;//56

	public Int32 nObjectStatus;//( 0:no break, 1:broken)
	public UInt32 nCollector;
}


public class AS_SC_NPC_DISAPPEAR_1 : AsPacketHeader
{
	public Int32 nNpcCnt;
	public AS_SC_NPC_DISAPPEAR_2[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nNpcCnt
		byte[] npcCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nNpcCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, npcCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( npcCnt, 0));
		index += sizeof( Int32);

		body = new AS_SC_NPC_DISAPPEAR_2[nNpcCnt];
		for( int i = 0; i < nNpcCnt; i++)
		{
			body[i] = new AS_SC_NPC_DISAPPEAR_2();

			byte[] tmpData = new byte[AS_SC_NPC_DISAPPEAR_2.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_SC_NPC_DISAPPEAR_2.size;
		}
	}
}


public class AS_SC_NPC_DISAPPEAR_2 : AsPacketHeader
{
	public static int size = 4;
	public Int32 nNpcIdx;//4
}
#endregion


#region - move -
public class AS_CS_CHAR_MOVE : AsPacketHeader
{
	public Vector3 sCurPosition;
	public Vector3 sDestPosition;
	public Int32 nMoveType;

	public AS_CS_CHAR_MOVE( int _moveType, Vector3 _curPosition, Vector3 _destPosition)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHAR_MOVE;

		sCurPosition = _curPosition;
		sDestPosition = _destPosition;

		nMoveType = _moveType;
	}
	
//	public AS_CS_CHAR_MOVE( SavedMoveInfo _info) //$yde
//	{
//		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
//		Protocol = (byte)PROTOCOL_CS.CS_CHAR_MOVE;
//		
//		sCurPosition = _info.sCurPosition;
//		sDestPosition = _info.sDestPosition;
//
//		nMoveType = (int)_info.nMoveType;
//	}
}


public class AS_SC_CHAR_MOVE : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Vector3 sCurPosition;
	public Vector3 sDestPosition;
	public Int32 nMoveType;
}


public class AS_SC_NPC_MOVE : AsPacketHeader
{
	public Int32 nNpcIdx;

	public Int32 fMoveSpeed;

	public Vector3 sCurPosition;
	public Vector3 sDestPosition;

	public bool bCombat;
	public bool bForceMove;
	public bool bForceMyself;
}
#endregion


#region - send player attack -
public class AS_CS_CHAR_ATTACK_NPC : AsPacketHeader
{
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
	public Int32 nActionTableIdx;
	public Int32 nChargeStep;
	public bool bCasting;
	public bool bReady;
	public int nSlot = -1;
	public int eEquipSlot = -1;

//	public Int32 nNpcIdx;//main target( npc)
//	public UInt32 nCharUniqKey;//main target( user)
	
	public Int32[] nNpcIdx = new Int32[TargetDecider.MAX_SKILL_TARGET];
	public UInt32[] nCharUniqKey = new UInt32[TargetDecider.MAX_SKILL_TARGET];
	
	public Vector3 sTargeting;
	public Vector3 sDirection;

	public Int32 nNpcCnt;
	public Int32 nCharCnt;

	public body2_AS_CS_CHAR_ATTACK_NPC[] nNpcIndices;
	public body3_AS_CS_CHAR_ATTACK_NPC[] nCharIndices;

	public AS_CS_CHAR_ATTACK_NPC( Int32 _skillTableIdx, Int32 _skillLevel,
		Int32 _actionTableIdx, Int32 _chargeStep, bool _casting, bool _ready, int _slot,
		Int32[] _npcIdx, UInt32[] _charIdx, Vector3 _target, Vector3 _dir, Int32 _npcCnt, Int32 _charCnt,
		body2_AS_CS_CHAR_ATTACK_NPC[] _npcIndices, body3_AS_CS_CHAR_ATTACK_NPC[] _charIndices)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHAR_SKILL;

		nSkillTableIdx = _skillTableIdx;
		nSkillLevel = _skillLevel;
		nActionTableIdx = _actionTableIdx;
		nChargeStep = _chargeStep;
		bCasting = _casting;
		bReady = _ready;
		nSlot = _slot;

		nNpcIdx = _npcIdx;
		nCharUniqKey = _charIdx;
		sTargeting = _target;
		sDirection = _dir;

		nNpcCnt = _npcCnt;
		nCharCnt = _charCnt;

		nNpcIndices = _npcIndices;
		nCharIndices = _charIndices;
	}
	
	public void SetEquipSlot( int _equip)
	{
		eEquipSlot = _equip;
	}
}


public class body2_AS_CS_CHAR_ATTACK_NPC : AsBaseClass
{
	public Int32 nNpcIdx;

	public body2_AS_CS_CHAR_ATTACK_NPC( Int32 _npcIdx)
	{
		nNpcIdx = _npcIdx;
	}
}


public class body3_AS_CS_CHAR_ATTACK_NPC : AsBaseClass
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public body3_AS_CS_CHAR_ATTACK_NPC( UInt16 _sessionIdx, UInt32 _charUniqKey)
	{
		nSessionIdx = _sessionIdx;
		nCharUniqKey = _charUniqKey;
	}
}
#endregion


#region - receive user attack -
public class AS_SC_CHAR_ATTACK_NPC_1 : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
	public Int32 nActionTableIdx;
	public Int32 nChargeStep;
	public bool bCasting;
	public bool bReady;
	
//	public Int32 nSlot = -1;
//	public Int32 eEquipSlot = -1;

//	public Int32 nNpcIdx;//MAIN TARGET( npc)
//	public UInt32 nCharIdx;//MAIN TARGET( user)
	public Int32[] nNpcIdx = new Int32[TargetDecider.MAX_SKILL_TARGET];
	public UInt32[] nMainCharUniqKey = new UInt32[TargetDecider.MAX_SKILL_TARGET];
	
	public Vector3 sTargeting;
	public Vector3 sDirection;

	public UInt32 nCoolTime;

	public float nHpCur;
	public float nHpHeal;

	public Int32 nNpcCnt;
	public Int32 nCharCnt;
	public AS_SC_CHAR_ATTACK_NPC_2[] bodyNpc = null;
	public AS_SC_CHAR_ATTACK_NPC_3[] bodyChar = null;

	public new void PacketBytesToClass( byte[] data)
	{
		try
		{
			Type infotype = this.GetType();
			FieldInfo headerinfo = null;
	
			int index = ParsePacketHeader( data);

			// nSessionIdx
			byte[] sessionIdx = new byte[ sizeof( UInt16)];
			headerinfo = infotype.GetField( "nSessionIdx", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
			headerinfo.SetValue( this, BitConverter.ToUInt16( sessionIdx, 0));
			index += sizeof( UInt16);

			// nCharUniqKey
			byte[] charUniqKey = new byte[ sizeof( UInt32)];
			headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
			headerinfo.SetValue( this, BitConverter.ToUInt32( charUniqKey, 0));
			index += sizeof( UInt32);

			// nSkillTableIdx
			byte[] skillTableIdx = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nSkillTableIdx", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, skillTableIdx, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( skillTableIdx, 0));
			index += sizeof( Int32);

			// nSkillLevel
			byte[] skillLevel = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nSkillLevel", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, skillLevel, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( skillLevel, 0));
			index += sizeof( Int32);

			// nActionTableIdx
			byte[] actionTableIdx = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nActionTableIdx", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, actionTableIdx, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( actionTableIdx, 0));
			index += sizeof( Int32);

			// nChargeStep
			byte[] chargeStep = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nChargeStep", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, chargeStep, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( chargeStep, 0));
			index += sizeof( Int32);

			// bCasting
			byte[] casting = new byte[ sizeof( bool)];
			headerinfo = infotype.GetField( "bCasting", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, casting, 0, sizeof( bool));
			headerinfo.SetValue( this, BitConverter.ToBoolean( casting, 0));
			index += sizeof( bool);

			// bReady
			byte[] ready = new byte[ sizeof( bool)];
			headerinfo = infotype.GetField( "bReady", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, ready, 0, sizeof( bool));
			headerinfo.SetValue( this, BitConverter.ToBoolean( ready, 0));
			index += sizeof( bool);

			// nNpcIdx
			for( int i=0; i<TargetDecider.MAX_SKILL_TARGET; ++i)
			{
				byte[] npcIdx = new byte[ sizeof( Int32)];
				Buffer.BlockCopy( data, index, npcIdx, 0, sizeof( Int32));
				nNpcIdx[i] = BitConverter.ToInt32( npcIdx, 0);
				index += sizeof( Int32);
			}

			// nCharIdx
			for( int i=0; i<TargetDecider.MAX_SKILL_TARGET; ++i)
			{
				byte[] charIdx = new byte[ sizeof( UInt32)];
				Buffer.BlockCopy( data, index, charIdx, 0, sizeof( UInt32));
				nMainCharUniqKey[i] = BitConverter.ToUInt32( charIdx, 0);
				index += sizeof( UInt32);
			}
			
			// sTargeting
			byte[] targetting = new byte[12];
			headerinfo = infotype.GetField( "sTargeting", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, targetting, 0, 12);
			headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( targetting, 0), BitConverter.ToSingle( targetting, 4), BitConverter.ToSingle( targetting, 8)));
			index += 12;

			// sDirection
			byte[] direction = new byte[12];
			headerinfo = infotype.GetField( "sDirection", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, direction, 0, 12);
			headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( direction, 0), BitConverter.ToSingle( direction, 4), BitConverter.ToSingle( direction, 8)));
			index += 12;

			// nCoolTime
			byte[] coolTime = new byte[ sizeof( UInt32)];
			headerinfo = infotype.GetField( "nCoolTime", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, coolTime, 0, sizeof( UInt32));
			headerinfo.SetValue( this, BitConverter.ToUInt32( coolTime, 0));
			index += sizeof( UInt32);

			// nHpCur
			byte[] hpCur = new byte[ sizeof( Single)];
			headerinfo = infotype.GetField( "nHpCur", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, hpCur, 0, sizeof( Single));
			headerinfo.SetValue( this, BitConverter.ToSingle( hpCur, 0));
			index += sizeof( Single);

			// nHpHeal
			byte[] hpHeal = new byte[ sizeof( Single)];
			headerinfo = infotype.GetField( "nHpHeal", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, hpHeal, 0, sizeof( Single));
			headerinfo.SetValue( this, BitConverter.ToSingle( hpHeal, 0));
			index += sizeof( Single);

			// nNpcCnt
			byte[] npcCnt = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nNpcCnt", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, npcCnt, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( npcCnt, 0));
			index += sizeof( Int32);

			// nCharCnt
			byte[] charCnt = new byte[ sizeof( Int32)];
			headerinfo = infotype.GetField( "nCharCnt", BINDING_FLAGS_PIG);
			Buffer.BlockCopy( data, index, charCnt, 0, sizeof( Int32));
			headerinfo.SetValue( this, BitConverter.ToInt32( charCnt, 0));
			index += sizeof( Int32);

			bodyNpc = new AS_SC_CHAR_ATTACK_NPC_2[nNpcCnt];
			for( int i = 0; i < nNpcCnt; i++)
			{
				bodyNpc[i] = new AS_SC_CHAR_ATTACK_NPC_2();
	
				byte[] tmpData = new byte[AS_SC_CHAR_ATTACK_NPC_2.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				bodyNpc[i].ByteArrayToClass( tmpData);
				index += AS_SC_CHAR_ATTACK_NPC_2.size;
			}

			bodyChar = new AS_SC_CHAR_ATTACK_NPC_3[nCharCnt];
			for( int i = 0; i < nCharCnt; i++)
			{
				bodyChar[i] = new AS_SC_CHAR_ATTACK_NPC_3();
	
				byte[] tmpData = new byte[AS_SC_CHAR_ATTACK_NPC_3.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				bodyChar[i].ByteArrayToClass( tmpData);
				index += AS_SC_CHAR_ATTACK_NPC_3.size;
			}
		}
		catch
		{
			Debug.LogError( "AsProtocol_Commmon::AS_SC_CHAR_ATTACK_NPC_2: error in constructor");
		}
	}
}


public class AS_SC_CHAR_ATTACK_NPC_2 : AsPacketHeader
{
//	public static int size = 28;
	public static int size = ( sizeof( Int32) * 2) + ( sizeof( float) * 5) + sizeof( bool);

	public Int32 nNpcIdx;

	public float nHpCur;
	public Int32 eDamageType;
	public float nDamage;
	public float fHpHeal;

	public float fReflection;
	public float fDrain;
	
	#region -Authority
	public bool bAggroMyself;
	#endregion
}


public class AS_SC_CHAR_ATTACK_NPC_3 : AsPacketHeader
{
	public static int size = 50 + sizeof( bool);

	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public Single fHpCur;
	public Single fMpCur;

	public Int32 eDamageType;

	public Single fDamage;

	public Single fHpHeal;
	public Single fMpHeal;

	public Single fReflection;
	public Single fDrain;

	public Vector3 vPosition;
	public bool bKnockBack;
}


public class body_SC_CHAR_SKILL_EFFECT : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
	public Int32 nPotencyIdx;
	public Int32 nChargeStep;
}

public class body_CS_CHAR_SKILL_STANCE : AsPacketHeader
{
	public body_CS_CHAR_SKILL_STANCE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
#if _STANCE
		Protocol = (byte)PROTOCOL_CS.CS_CHAR_SKILL_STANCE;
#endif
	}
}

public class body_SC_CHAR_SKILL_STANCE : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public Int32 nStanceSkill;
	public Int32 nStanceSkillLevel;
	public Int32 nStancePotencyIdx;
}

//$yde
public class body_SC_CHAR_BUFF_ACCURE_INFO : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	
	public bool bCriticalChance;
	public bool bDodgeChance;
}

public class body_SC_CHAR_SKILL_USE_ADD : AsPacketHeader
{
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
};

public class body_SC_CHAR_SKILL_SOULSTONE : AsPacketHeader
{
	public Int32		eEquipSlot;
	public Int32		nSkillTableIdx;
	public Int32		nSkillLevel;
}

public class body_SC_CHAR_SKILL_PVP_AGGRO : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
}
#endregion


#region - receive npc attack -
public class AS_SC_NPC_ATTACK_CHAR_1 : AsPacketHeader
{
	public Int32 nNpcIdx;
	
//	public UInt16 nSessionIdx;
//	public UInt32 nCharUniqKey;
	public UInt16[] nSessionIdx = new UInt16[TargetDecider.MAX_SKILL_TARGET];
	public UInt32[] nCharUniqKey = new UInt32[TargetDecider.MAX_SKILL_TARGET];
	
	public Single nHpCur;
	public Int32 nMonsterSkillLevelTableIdx;
	public bool bCasting;
	public Int32 nCastingMilliSec;
	public bool bReady;
	public Int32 nCharCnt;
	public Int32 nNpcCnt;
	public AS_SC_NPC_ATTACK_CHAR_2[] bodyChar = null;
	public AS_SC_NPC_ATTACK_CHAR_3[] bodyNpc = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nNpcIdx
		byte[] npcIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nNpcIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, npcIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( npcIdx, 0));
		index += sizeof( Int32);

//		// nSessionIdx
//		byte[] sessionIdx = new byte[ sizeof( UInt16)];
//		headerinfo = infotype.GetField( "nSessionIdx", BINDING_FLAGS_PIG);
//		Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
//		headerinfo.SetValue( this, BitConverter.ToUInt16( sessionIdx, 0));
//		index += sizeof( UInt16);
//
//		// nCharUniqKey
//		byte[] charUniqKey = new byte[ sizeof( UInt32)];
//		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
//		Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
//		headerinfo.SetValue( this, BitConverter.ToUInt32( charUniqKey, 0));
//		index += sizeof( UInt32);
		
		// nSessionIdx
		for( int i=0; i<TargetDecider.MAX_SKILL_TARGET; ++i)
		{
			byte[] sessionIdx = new byte[ sizeof( UInt16)];
			Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
			nSessionIdx[i] = BitConverter.ToUInt16( sessionIdx, 0);
			index += sizeof( UInt16);
		}

		// nCharUniqKey
		for( int i=0; i<TargetDecider.MAX_SKILL_TARGET; ++i)
		{
			byte[] charUniqKey = new byte[ sizeof( UInt32)];
			Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
			nCharUniqKey[i] = BitConverter.ToUInt32( charUniqKey, 0);
			index += sizeof( UInt32);
		}

		// nHpCur
		byte[] hpCur = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "nHpCur", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, hpCur, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( hpCur, 0));
		index += sizeof( Single);

		// nMonsterSkillLevelTableIdx
		byte[] monsterSkillLevelTableIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMonsterSkillLevelTableIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, monsterSkillLevelTableIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( monsterSkillLevelTableIdx, 0));
		index += sizeof( Int32);

		// bCasting
		byte[] casting = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bCasting", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, casting, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( casting, 0));
		index += sizeof( bool);

		// nCastingMilliSec
		byte[] castingMilliSec = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCastingMilliSec", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, castingMilliSec, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( castingMilliSec, 0));
		index += sizeof( Int32);

		// bReady
		byte[] ready = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bReady", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ready, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( ready, 0));
		index += sizeof( bool);

		// nCharCnt
		byte[] charCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCharCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, charCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( charCnt, 0));
		index += sizeof( Int32);

		// nNpcCnt
		byte[] npcCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nNpcCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, npcCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( npcCnt, 0));
		index += sizeof( Int32);

		bodyChar = new AS_SC_NPC_ATTACK_CHAR_2[nCharCnt];
		for( int i = 0; i < nCharCnt; i++)
		{
			bodyChar[i] = new AS_SC_NPC_ATTACK_CHAR_2();
	
			byte[] tmpData = new byte[AS_SC_NPC_ATTACK_CHAR_2.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			bodyChar[i].ByteArrayToClass( tmpData);
			index += AS_SC_NPC_ATTACK_CHAR_2.size;
		}

		bodyNpc = new AS_SC_NPC_ATTACK_CHAR_3[nNpcCnt];
		for( int i = 0; i < nNpcCnt; i++)
		{
			bodyNpc[i] = new AS_SC_NPC_ATTACK_CHAR_3();
	
			byte[] tmpData = new byte[AS_SC_NPC_ATTACK_CHAR_3.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			bodyNpc[i].ByteArrayToClass( tmpData);
			index += AS_SC_NPC_ATTACK_CHAR_3.size;
		}
	}
}


public class AS_SC_NPC_ATTACK_CHAR_2 : AsPacketHeader
{
	public static int size = 38 + sizeof( bool);

	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Single fHpCur;
	public Single fMpCur;
	public Int32 eDamageType; // ilmeda, 20120731
	public Single nDamage;
	public Single fHpHeal;
	public Single fMpHeal;
	public Single fReflection;
	public Single fDrain;
	public bool bKnockBack;
}


public class AS_SC_NPC_ATTACK_CHAR_3 : AsPacketHeader
{
	public static int size = 16;

	public Int32 nNpcIdx;
	public Single fHpCur;
	public Single fHpHeal;
	public Single fMpHeal;
}


public class body_SC_NPC_SKILL_EFFECT : AsPacketHeader
{
	public Int32 nNpcIdx;

	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
	public Int32 nPotencyIdx;
	public Int32 nChargeStep;
}
#endregion
#endregion -- character -


public class body_SC_NPC_COMBAT_FREE : AsPacketHeader
{
	public Int32 nNpcIdx;
}


public enum eNPCSTATUS
{
	eNPCSTATUS_NOTHING = 0,

	eNPCSTATUS_FINDENEMY,
	eNPCSTATUS_RUNAWAY_START,
	eNPCSTATUS_RUNAWAY_END,
	eNPCSTATUS_RESIST_DEBUFF,

	eNPCSTATUS_MAX
}


public class body_SC_NPC_STATUS : AsPacketHeader
{
	public Int32 nNpcIdx;
	public eNPCSTATUS eStatus;
	public UInt16 nSessionIdx;
}


public class body_SC_NPC_APPEAR_NOTIFY : AsPacketHeader
{
	public Int32 nNpcIdx;
	public Int32 nNpcTableIdx;
	public Int32 nChannel;
	public Int32 nMapIdx;
}


//--------------------------------------------------------------------------------------------------
/* Live */
//--------------------------------------------------------------------------------------------------
public class body_CS_LIVE : AsPacketHeader
{
	public Int64 nClientSec;
	static private DateTime dt = new DateTime( 1970, 1, 1);
	
	public body_CS_LIVE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_LIVE;
		
		TimeSpan ts = DateTime.Now - dt;
		
		nClientSec = (Int64)( ts.TotalSeconds);
	}
}


public class body_CS_BACKGROUND_PROC : AsPacketHeader
{
	public bool bBackground;
	public body_CS_BACKGROUND_PROC( bool isBackground)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_BACKGROUND_PROC;
		bBackground = isBackground;
	}
}


//--------------------------------------------------------------------------------------------------
/* Drop Item ( Use Server IC and GC)*/
//--------------------------------------------------------------------------------------------------
/*
 * Send Packet : Drop Item Pick up
*/
public class AS_body_CS_PICKUP_ITEM : AsPacketHeader
{
	public Int32 nDropItemIdx = 0;
	public AS_body_CS_PICKUP_ITEM( Int32 _nDropItemIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PICKUP_ITEM;
		nDropItemIdx = _nDropItemIdx;
	}
}


/*
 * Receve Packet : Drop Item Pick up Result
 * Packet Define : SC_PICKUP_ITEM_RESULT
*/
public class AS_body_SC_PICKUP_ITEM_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nDropItemIdx;
}


/*
 * Receve Packet : Drop Item Appear Header
 * Packet Define : SC_DROPITEM_APPEAR
*/
public class AS_body1_SC_DROPITEM_APPEAR : AsPacketHeader
{
	public Int32 nDropItemCnt = 0;
	public AS_body2_SC_DROPITEM_APPEAR[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nDropItemCnt
		byte[] dropItemCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nDropItemCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, dropItemCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( dropItemCnt, 0));
		index += sizeof( Int32);

		body = new AS_body2_SC_DROPITEM_APPEAR[nDropItemCnt];
		for( int i = 0; i < nDropItemCnt; i++)
		{
			body[i] = new AS_body2_SC_DROPITEM_APPEAR();
	
			byte[] tmpData = new byte[AS_body2_SC_DROPITEM_APPEAR.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_body2_SC_DROPITEM_APPEAR.size;
		}
	}
}


/*
 * Receve Packet : Drop Item Appear Body
 * Packet Define : SC_DROPITEM_APPEAR
*/
public class AS_body2_SC_DROPITEM_APPEAR : AsPacketHeader
{
	public static int size = 4 + 4 + 12 + 4 + 4 + 4;
	public Int32 nDropItemIdx = 0;
	public Int32 nItemTableIdx = 0;
	public Int32 nNpcIdx = 0;
	public Int32 nOwner = 0;
	public Int32 nPartyIdx = 0;
	public Vector3 sCurPosition;
}


/*
 * Receve Packet : Drop Item disappear
 * Packet Define : IC_DROPITEM_APPEAR
*/
public class AS_body1_SC_DROPITEM_DISAPPEAR : AsPacketHeader
{
	public Int32 nDropItemCnt = 0;
	public AS_body2_SC_DROPITEM_DISAPPEAR[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nDropItemCnt
		byte[] dropItemCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nDropItemCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, dropItemCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( dropItemCnt, 0));
		index += sizeof( Int32);

		body = new AS_body2_SC_DROPITEM_DISAPPEAR[nDropItemCnt];
		for( int i = 0; i < nDropItemCnt; i++)
		{
			body[i] = new AS_body2_SC_DROPITEM_DISAPPEAR();

			byte[] tmpData = new byte[AS_body2_SC_DROPITEM_DISAPPEAR.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_body2_SC_DROPITEM_DISAPPEAR.size;
		}
	}
}


public class AS_body2_SC_DROPITEM_DISAPPEAR : AsPacketHeader
{
	public static int size = 4;
	public Int32 nDropItemIdx = 0;
}


//--------------------------------------------------------------------------------------------------
/* HP, MP */
//--------------------------------------------------------------------------------------------------
/*
 * Receve Packet : HP
 * Packet Define : SC_ATTRINFO_HP
*/
public enum eATTRCHANGECONTENTS
{
	eATTRCHANGECONTENTS_NOTHING,
	eATTRCHANGECONTENTS_DOT_DAMAGE,
	eATTRCHANGECONTENTS_KILLNPC,
	eATTRCHANGECONTENTS_RECOVERY,
	eATTRCHANGECONTENTS_RESURRECTION,
	eATTRCHANGECONTENTS_USING_SKILL,
	eATTRCHANGECONTENTS_CHEAT,
	eATTRCHANGECONTENTS_REWARD,
	eATTRCHANGECONTENTS_STATUS_CHANGE,
	eATTRCHANGECONTENTS_LEVELUP,
	eATTRCHANGECONTENTS_ITEMMOVE,
	eATTRCHANGECONTENTS_HP_INSTEAD_MP,
	eATTRCHANGECONTENTS_MAX
}


public class AS_body_SC_ATTRINFO_HP : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Single fHpMax;
	public Single fHpCur;
	public eATTRCHANGECONTENTS eContents;
	public Single fRecovery;
}


/*
 * Receve Packet : MP
 * Packet Define : SC_ATTRINFO_MP
*/
public class AS_body_SC_ATTRINFO_MP : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Single fMpMax;
	public Single fMpCur;
	public eATTRCHANGECONTENTS eContents;
	public Single fRecovery;
}


public class body_SC_GOLD_CHANGE : AsPacketHeader
{
	public static int size = 20;

	public Int64 nChangeGold;
	public UInt64 nTotalGold;
}


public class body_SC_SPHERE_CHANGE : AsPacketHeader
{
	public static int size = 16;

	public Int64 nChangeSphere;
	public Int64 nTotalSphere;
};


public class body_SC_SOCIALPOINT_CHANGE : AsPacketHeader
{
	public static int size = 8;

	public Int32 nChangeSocialPoint;
	public Int32 nTotalSocialPoint;
};


public class sSKILL : AsPacketHeader
{
	public static int size = 8;

	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;

	public sSKILL(){}
	public sSKILL( int _idx, int _lv)
	{
		nSkillTableIdx = _idx;
		nSkillLevel = _lv;
	}
}

#region -SkillReset
public class body1_SC_SKILLBOOK_LIST : AsPacketHeader
{
	public Int32 nSkillBookCnt;
	public body2_SC_SKILLBOOK_LIST[] body = null;
	
	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);
		
		// nSkillBookCnt
		byte[] skillBookCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nSkillBookCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, skillBookCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( skillBookCnt, 0));
		index += sizeof( Int32);
		
		AsUserInfo.Instance.resettedSkills.Clear();
		
		// body
		body = new body2_SC_SKILLBOOK_LIST[ nSkillBookCnt];
		for( int i = 0; i < nSkillBookCnt; i++)
		{
			body[i] = new body2_SC_SKILLBOOK_LIST();
			byte[] tmpData = new byte[ body2_SC_SKILLBOOK_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_SKILLBOOK_LIST.size;
			
			AsUserInfo.Instance.resettedSkills.Add( body[i].nSkillBookTableIdx, body[i]);
		}
	}
}

public class body2_SC_SKILLBOOK_LIST : AsPacketHeader
{
	public static readonly int size = 4;
	
	public Int32 nSkillBookTableIdx;
}
#endregion

public class body1_SC_SKILL_LIST : AsPacketHeader
{
#if _STANCE
	public Int32 nStanceSkill;
	public Int32 nStanceSkillLevel;
	public Int32 nStancePotencyIdx;
#endif
	public Int32 nSkillCnt = 0;
	public sSKILL[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

#if _STANCE
		// nStanceSkill
		byte[] stanceSkill = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nStanceSkill", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, stanceSkill, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( stanceSkill, 0));
		index += sizeof( Int32);
		// nStanceSkillLevel
		byte[] stanceSkillLevel = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nStanceSkillLevel", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, stanceSkillLevel, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( stanceSkillLevel, 0));
		index += sizeof( Int32);
		// nStancePotencyIdx
		byte[] stancePotencyIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nStancePotencyIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, stancePotencyIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( stancePotencyIdx, 0));
		index += sizeof( Int32);
#endif
		
		// nSkillCnt
		byte[] skillCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nSkillCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, skillCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( skillCnt, 0));
		index += sizeof( Int32);

		body = new sSKILL[nSkillCnt];
		for( int i = 0; i < nSkillCnt; i++)
		{
			body[i] = new sSKILL();

			byte[] tmpData = new byte[sSKILL.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += sSKILL.size;
		}
	}
}


public class body_CS_SKILL_LEARN : AsPacketHeader
{
	public Int32 nNpcIdx;
	public Int32 nSkillBookIdx;
	public sSKILL sSkill;

	public body_CS_SKILL_LEARN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SKILL_LEARN;
	}
}


public class body_SC_SKILL_LEARN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public sSKILL sSkill;
	public int nSkillBookIdx;
}

public class body_CS_SKILL_RESET : AsPacketHeader
{
	public Int32 nSkillTableIdx;

	public body_CS_SKILL_RESET(Int32 _skillTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SKILL_RESET;
		
		nSkillTableIdx = _skillTableIdx;
	}
}

public class body_SC_SKILL_RESET_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nSkillTableIdx;
}


public class body_SC_ATTR_EXP : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nLevel;
	public Int32 nTotExp;
	public Int32 nAddExp;
	public Int32 nNpcIdx;//$yde
}


public class body_SC_ATTR_LEVELUP : AsPacketHeader
{
	public static int size = 212;

	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nLevel;
	public float fHpCur;
	public float fHpMax;
	public float fMpCur;
	public float fMpMax;
	public sCLIENTSTATUS sDefaultStatus;
	public sCLIENTSTATUS sFinalStatus;
	public Int32 nNpcIdx;//$yde
}


public class body_SC_NPC_ATTR_HP : AsPacketHeader
{
	public Int32 nNpcIdx;
	public float fHpCur;
	public eATTRCHANGECONTENTS eContents;
	public float fRecovery;
}


public class body_SC_NPC_ATTR_CHANGE : AsPacketHeader
{
	public Int32 nNpcIdx;
	public eCHANGE_INFO_TYPE eChangeType;
	public Single nChangeValue;
}


//--------------------------------------------------------------------------------------------------
/* Warp Packet : 인던( 마을) 안에서 맵 이동 */
//--------------------------------------------------------------------------------------------------
public class AS_body_CS_WARP : AsPacketHeader
{
	public Int32 nWarpTableIdx = 0;
	public Int32 nNpcSpawnIdx = 0;

	public AS_body_CS_WARP( Int32 _nWarpTableIdx, Int32 _nNpcSpawnIdx )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WARP;
		nWarpTableIdx = _nWarpTableIdx;
		nNpcSpawnIdx = _nNpcSpawnIdx;
	}
}


public class AS_body_SC_WARP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}


public class AS_body_CS_WARP_END : AsPacketHeader
{
	public AS_body_CS_WARP_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WARP_END;
	}
}


public class body_CS_WARP_XZ_END : AsPacketHeader
{
	public Int32 nMapIdx;
	public Vector3 sPosition;

	public body_CS_WARP_XZ_END( Int32 mapIdx, Vector3 vPos)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WARP_XZ_END;
		nMapIdx = mapIdx;
		sPosition = vPos;
	}
}


public class body_SC_WARP_XZ_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}


public class body_CS_WARP_CANCEL : AsPacketHeader
{
	public body_CS_WARP_CANCEL()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WARP_CANCEL;
	}
}


//--------------------------------------------------------------------------------------------------
/* Buff Packet : */
//--------------------------------------------------------------------------------------------------
// Character buff insert.
public class body1_SC_CHAR_BUFF : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public bool bEffect;//true:perform effect, false:don't
	public Int32 nBuffCnt;
	public body2_SC_CHAR_BUFF[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nSessionIdx
		byte[] sessionIdx = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nSessionIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( sessionIdx, 0));
		index += sizeof( UInt16);

		// nCharUniqKey
		byte[] charUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( charUniqKey, 0));
		index += sizeof( UInt32);

		// bEffect
		byte[] effect = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bEffect", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, effect, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( effect, 0));
		index += sizeof( bool);

		// nBuffCnt
		byte[] buffCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nBuffCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, buffCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( buffCnt, 0));
		index += sizeof( Int32);

		body = new body2_SC_CHAR_BUFF[nBuffCnt];
		for( int i = 0; i < nBuffCnt; i++)
		{
			body[i] = new body2_SC_CHAR_BUFF();
			
			byte[] tmpData = new byte[body2_SC_CHAR_BUFF.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_CHAR_BUFF.size;
		}
	}
}


public class body2_SC_CHAR_BUFF : AsPacketHeader// AsBaseClass
{
	public static int size = 1+32;

	public bool bUpdate;
	
	public UInt32 nCharUniqKey;
	
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
	public Int32 nDuration;
	
	public body2_SC_CHAR_BUFF(){}
	
	public body2_SC_CHAR_BUFF(body2_SC_PARTY_USER_BUFF _party)
	{
		bUpdate = _party.bUpdate;
		
		nSkillTableIdx = _party.nSkillTableIdx;
		nSkillLevelTableIdx = _party.nSkillLevelTableIdx;
		nSkillLevel = _party.nSkillLevel;
		nChargeStep = _party.nChargeStep;
		nPotencyIdx = _party.nPotencyIdx;
		eType = _party.eType;
		nDuration = _party.nDuration;
	}
}


// Character buff delete.
public class body_SC_CHAR_DEBUFF : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
}

public class body_SC_CHAR_DEBUFF_RESIST : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
}

// Npc buff insert.
public class body_SC_NPC_BUFF : AsPacketHeader
{
	public Int32 nNpcIdx;
	public bool bUpdate;
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
	public Int32 nDuration;
}


// Npc buff insert.
public class body1_SC_NPC_BUFF : AsPacketHeader
{
	public Int32 nNpcIdx;
	public bool bEffect;
	public Int32 nBuffCnt;
	public body2_SC_NPC_BUFF[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nNpcIdx
		byte[] npcIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nNpcIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, npcIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( npcIdx, 0));
		index += sizeof( Int32);

		// bEffect
		byte[] effect = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bEffect", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, effect, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( effect, 0));
		index += sizeof( bool);

		// nBuffCnt
		byte[] buffCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nBuffCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, buffCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( buffCnt, 0));
		index += sizeof( Int32);

		body = new body2_SC_NPC_BUFF[nBuffCnt];
		for( int i = 0; i < nBuffCnt; i++)
		{
			body[i] = new body2_SC_NPC_BUFF();

			byte[] tmpData = new byte[body2_SC_NPC_BUFF.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_NPC_BUFF.size;
		}
	}
}


// Npc buff insert.
public class body2_SC_NPC_BUFF : AsPacketHeader
{
	public static readonly int size = 29;

	public bool bUpdate;
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
	public Int32 nDuration;
}


// Npc buff delete.
public class body_SC_NPC_DEBUFF : AsPacketHeader
{
	public Int32 nNpcIdx;
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
}


public class body_CS_GOTO_TOWN : AsPacketHeader
{
	public body_CS_GOTO_TOWN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_GOTO_TOWN;
	}
}


public class body_SC_GOTO_TOWN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}


public class body_CS_GOTO_TOWN_END : AsPacketHeader
{
	public body_CS_GOTO_TOWN_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_GOTO_TOWN_END;
	}
}


public class body_CS_RESURRECTION : AsPacketHeader
{
	public UInt16 nSessionIdx;

	public body_CS_RESURRECTION()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_RESURRECTION;

		nSessionIdx = 0;
	}

	public body_CS_RESURRECTION( UInt16 _sessionIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_RESURRECTION;

		nSessionIdx = _sessionIdx;
	}
}


public class body_SC_RESURRECTION : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public UInt32 nResurrectCharUniqKey;
}


//WAYPOINT
public class body_CS_WAYPOINT : AsPacketHeader
{
	public body_CS_WAYPOINT()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WAYPOINT;
	}
}


public class body_CS_WAYPOINT_ACTIVE : AsPacketHeader
{
	public Int32 nWarpTableIdx;

	public body_CS_WAYPOINT_ACTIVE( Int32 _nWarpTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WAYPOINT_ACTIVE;
		nWarpTableIdx = _nWarpTableIdx;
	}
};


public class body1_SC_WAYPOINT : AsPacketHeader
{
	public Int32 nCnt;
	public body2_SC_WAYPOINT[] body;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		byte[] cnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, cnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( cnt, 0));
		index += sizeof( Int32);

		if( 0 < nCnt)
		{
			body = new body2_SC_WAYPOINT[nCnt];
			for( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_WAYPOINT();
	
				byte[] tmpData = new byte[body2_SC_WAYPOINT.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += body2_SC_WAYPOINT.size;
			}
		}
	}
};


public class body2_SC_WAYPOINT : AsPacketHeader
{
	public static int size = 4;
	public Int32 nWarpTableIdx;
};


public class body_SC_WAYPOINT_ACTIVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nWarpTableIdx;
};


// Cheat
/*
public class body_CS_CHEAT_HP : AsPacketHeader
{
	public body_CS_CHEAT_HP()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_HP;
	}
}

public class body_CS_CHEAT_MP : AsPacketHeader
{
	public body_CS_CHEAT_MP()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_MP;
	}
}

public class body_CS_CHEAT_LEVELUP : AsPacketHeader
{
	public body_CS_CHEAT_LEVELUP()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_LEVELUP;
	}
}

public class body_CS_CHEAT_DEATH : AsPacketHeader
{
	public body_CS_CHEAT_DEATH()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_DEATH;
	}
}

public class body_CS_CHEAT_NPCALL_REGEN : AsPacketHeader
{
	public body_CS_CHEAT_NPCALL_REGEN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_NPCALL_REGEN;
	}
}

public class body_CS_CHEAT_NPCALL_DELETE : AsPacketHeader
{
	public body_CS_CHEAT_NPCALL_DELETE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHEAT_NPCALL_DELETE;
	}
}
*/


//--------------------------------------------------------------------------------------------------
/* inven */
//--------------------------------------------------------------------------------------------------


/*
 * sITEM
*/
public class body_CS_ITEM_USE : AsPacketHeader
{
	public Int32 nTargetID;
	public Int32 nSlot;
	
	public body_CS_ITEM_USE( Int32 slotindex)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_ITEM_USE;
		nSlot = slotindex;
	}
}


/*
 * sITEM
*/
public class sITEMVIEW : AsPacketHeader
{
	public static int size	{ get { return ( sizeof( Int32) + sizeof( byte)); } }

	public Int32 nItemTableIdx;
	public byte nStrengthenCount;
}


public class sITEM : AsPacketHeader
{
	public static int size
	{
		get	{ return ( ( sizeof( Int32) * ( 6 + AsGameDefine.eITEM_MAX_ENCHANT_COUNT)) + sizeof( byte) + sizeof( sbyte) + sizeof( Int64) + sizeof( Int32) ); }
	}

	public Int32 nItemTableIdx;
	public Int32 nOverlapped;
	public byte	nStrengthenCount;
	public Int32 nOptID_1;
	public Int32 nOptValue_1;
	public Int32 nOptID_2;
	public Int32 nOptValue_2;
	public Int32[] nEnchantInfo = new Int32[ AsGameDefine.eITEM_MAX_ENCHANT_COUNT];
	public sbyte nTradeCount;
	public Int64 nExpireTime;
	public Int32 nAccreCount;

	public bool IsTradeEnable()
	{
		return 0 != nTradeCount;
	}

	//$yde
	public string GetNameWithStrengthCount()
	{
		Item item = ItemMgr.ItemManagement.GetItem(nItemTableIdx);
		if(item == null)
			return "undefined index(" + nItemTableIdx + ")";

		string str = item.ItemData.GetGradeColor() + AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
		if(nStrengthenCount > 0)
			str = Color.white + "+" + nStrengthenCount + " " + str;

		return str;
	}
}


public class body2_SC_ITEM_INVENTORY : AsPacketHeader
{
	public static int size	{ get { return ( sizeof( Int32) + sITEM.size); } }
	public Int32 nSlot;
	public sITEM sItem;
};

public class body_CS_ITEM_MOVE : AsPacketHeader
{
	public Int32 nSlot1;
	public Int32 nSlot2;

	public body_CS_ITEM_MOVE( Int32 _nSlot1, Int32 _nSlot2)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_ITEM_MOVE;
		nSlot1 = _nSlot1;
		nSlot2 = _nSlot2;
	}
}


public class body_SC_ITEM_SLOT : AsPacketHeader
{
	public static int size = 4 + sITEM.size;
	public Int32 nContents;
	public Int32 nSlot;
	public sITEM sItem;
}


public class body_SC_ITEM_INVENTORY : AsPacketHeader
{
	public byte nExtendPageCount;
	public body2_SC_ITEM_INVENTORY[] body = null;
	public Int16 nCount = 0;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nExtendPageCount
		headerinfo = infotype.GetField( "nExtendPageCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[index++]);

		//Debug.Log( "ITEM_INVENTORY nExtendPageCount: " + nExtendPageCount);

		// nCount
		byte[] count = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof(Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( count, 0));
		index += sizeof( Int16);

		//Debug.Log( "ITEM_INVENTORY nCount: " + nCount);

 		if( 0 < nCount)
		{
			body = new body2_SC_ITEM_INVENTORY[nCount];
			for( int i = 0; i < nCount; i++)
			{
				body[i] = new body2_SC_ITEM_INVENTORY();
				
				byte[] tmpData = new byte[body2_SC_ITEM_INVENTORY.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += body2_SC_ITEM_INVENTORY.size;

				//Debug.Log( "ITEM_INVENTORY idx: " + i + " id : " + body[i].sItem.nItemTableIdx + " slot : " + body[i].nSlot);
			}
		}

		//Debug.Log( "ITEM_INVENTORY end : " + index);
	}
}


public class body_SC_ITEM_INVENTORY_OPEN : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nExtendPageCount;
};


public class body_CS_ITEM_INVENTORY_OPEN : AsPacketHeader
{
	public body_CS_ITEM_INVENTORY_OPEN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_ITEM_INVENTORY_OPEN;
	}
}


/*
 * Receve Packet : CS_ITEM_REMOVE
*/
public class body_CS_ITEM_REMOVE : AsPacketHeader
{
	public Int32 nSlot;
	public Int32 nCnt;

	public body_CS_ITEM_REMOVE( Int32 slotindex, Int32 count)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_ITEM_REMOVE;
		nSlot = slotindex;
		nCnt = count;
	}
}


/*
 * Receve Packet : CS_ITEM_PAGE_SORT
*/
public class body_CS_ITEM_PAGE_SORT : AsPacketHeader
{
	public Int32 nPage;

	public body_CS_ITEM_PAGE_SORT( Int32 _nPage)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_ITEM_PAGE_SORT;
		nPage = _nPage;
	}
}


public class body_SC_ITEM_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nCommond;
	public Int32 nItemTableIdx;
};


public class body_SC_ITEM_VIEW_SLOT : AsPacketHeader
{
	public static int size = 2 + 4 + 4 + sITEMVIEW.size;

	public Int16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nSlot;
	public sITEMVIEW sViewItem;
};


public class body_SC_QUICKSLOT_LIST : AsPacketHeader
{
	public static int size = sQUICKSLOT.size * AsGameDefine.QUICK_SLOT_MAX;

	public sQUICKSLOT[] body = new sQUICKSLOT[AsGameDefine.QUICK_SLOT_MAX];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		for( int i = 0; i < AsGameDefine.QUICK_SLOT_MAX; i++)
		{
			body[i] = new sQUICKSLOT();
			byte[] tmpData = new byte[sQUICKSLOT.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += sQUICKSLOT.size;
		}
	}
}


public class body_CS_STORAGE_LIST : AsPacketHeader
{
	public body_CS_STORAGE_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_STORAGE_LIST;
	}
}


public class body_CS_STORAGE_MOVE : AsPacketHeader
{
	public eSTORAGE_MOVE_TYPE eMoveType;
	public Int16 nStartSlot;
	public Int32 nStartCount;
	public Int16 nDestSlot;

	public body_CS_STORAGE_MOVE( eSTORAGE_MOVE_TYPE _moveType, Int16 _startSlot, Int32 _startCount, Int16 _destSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_STORAGE_MOVE;

		eMoveType = _moveType;
		nStartSlot = _startSlot;
		nStartCount = _startCount;
		nDestSlot = _destSlot;
	}
}


public class body_CS_STORAGE_COUNT_UP : AsPacketHeader
{
	public body_CS_STORAGE_COUNT_UP()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_STORAGE_COUNT_UP;
	}
}


public class body_CS_STORAGE_SORT : AsPacketHeader
{
	public Int16 nStoragePage;

	public body_CS_STORAGE_SORT( Int16 _storagePage)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_STORAGE_SORT;

		nStoragePage = _storagePage;
	}
}


public class body1_SC_STORAGE_LIST : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nExtendCount;
	public Int16 nCount;

	public body2_SC_STORAGE_LIST[] body = new body2_SC_STORAGE_LIST[0];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nExtendCount
		headerinfo = infotype.GetField( "nExtendCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nCount
		byte[] count = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( count, 0));
		index += sizeof( Int16);

		if( 0 < nCount)
		{
			body = new body2_SC_STORAGE_LIST[nCount];
			for( int i = 0; i < nCount; i++)
			{
				body[i] = new body2_SC_STORAGE_LIST();
				
				byte[] tmpData = new byte[body2_SC_STORAGE_LIST.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += body2_SC_STORAGE_LIST.size;
			}
		}
	}
}


public class body2_SC_STORAGE_LIST : AsPacketHeader
{
	public static int size = 4 + sITEM.size;

	public Int32 nSlot;
	public sITEM sItem;
}


public class body_SC_STORAGE_MOVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class body_SC_STORAGE_SLOT : AsPacketHeader
{
	public Int32 nSlot;
	public sITEM sItem;
}


public class body_SC_STORAGE_COUNT_UP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nExtendCount;
}


/*
 * Receve Packet : SC_COOL_TIME_LIST
*/
public class body2_SC_COOL_TIME_LIST : AsPacketHeader
{
	public Int32 nCoolGroup;
	public Int32 nRemainTime;

	public static int size
	{
		get	{ return sizeof( Int32) * 2; }
	}
};


public class body1_SC_COOL_TIME_LIST : AsPacketHeader
{
	public Int16 nCoolTimeCount;

	public body2_SC_COOL_TIME_LIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCoolTimeCount
		byte[] coolTimeCount = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCoolTimeCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, coolTimeCount, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( coolTimeCount, 0));
		index += sizeof( Int16);

		if( 0 >= nCoolTimeCount)
			return;

		body = new body2_SC_COOL_TIME_LIST[ nCoolTimeCount];
		for( int i = 0; i < nCoolTimeCount; i++)
		{
			body[i] = new body2_SC_COOL_TIME_LIST();
			byte[] tmpData = new byte[body2_SC_COOL_TIME_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_COOL_TIME_LIST.size;
		}
	}
};


/*
 * Receve Packet : CS_QUICKSLOT_CHANGE
*/
public class body_CS_QUICKSLOT_CHANGE : AsPacketHeader
{
	public Int16 nSlot;
	public Int32 eType;
	public Int32 nValue;

	public body_CS_QUICKSLOT_CHANGE( Int16 _nslot, Int32 _eType, Int32 _nValue)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUICKSLOT_CHANGE;
		eType = _eType;
		nSlot = _nslot;
		nValue = _nValue;
	}
}


//--------------------------------------------------------------------------------------------------
/* Attribute Total */
//--------------------------------------------------------------------------------------------------
public class body_SC_ATTR_TOTAL : AsPacketHeader
{
	public static int size = sCLIENTSTATUS.size * 2 + sizeof( float) * 2;

	public sCLIENTSTATUS sDefaultStatus;
	public sCLIENTSTATUS sFinalSatus;
	public float fHpCur;
	public float fMpCur;
}


//--------------------------------------------------------------------------------------------------
/* Attribute Change */
//--------------------------------------------------------------------------------------------------
public class body_SC_ATTR_CHANGE : AsPacketHeader
{
	public static int size = 14;

	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public eCHANGE_INFO_TYPE eChangeType;
	public Single nChangeValue;
}


//--------------------------------------------------------------------------------------------------
/* Object Action */
//--------------------------------------------------------------------------------------------------
/*
 * Send Packet : CS_OBJECT_CATCH
 *
*/
public class body_CS_OBJECT_CATCH : AsPacketHeader
{
	public Int32 nNpcIdx;

	public body_CS_OBJECT_CATCH( Int32 _nNpcIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_OBJECT_CATCH;
		nNpcIdx = _nNpcIdx;
	}
}


/*
 * Recive Packet : SC_OBJECT_CATCH_RESULT
 *
*/
public class body_SC_OBJECT_CATCH_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nNpcIdx;
}


/*
 * Recive Packet : SC_OBJECT_CATCH
 *
*/
public class body_SC_OBJECT_CATCH : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nNpcIdx;
}


/*
 * Send Packet : CS_OBJECT_BREAK
 * break object action
*/
public class body_CS_OBJECT_BREAK : AsPacketHeader
{
	public Int32 nNpcIdx;
	
	public body_CS_OBJECT_BREAK( Int32 _nNpcIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_OBJECT_BREAK;
		nNpcIdx = _nNpcIdx;
	}
}


/*
 * Recive Packet : SC_OBJECT_BREAK_RESULT
 *
*/
public class body_SC_OBJECT_BREAK_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nNpcIdx;
}


/*
 * Recive Packet : SC_OBJECT_BREAK
 *
*/
public class body_SC_OBJECT_BREAK : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nNpcIdx;
}


/*
 * Send Packet : CS_OBJECT_JUMP
 * jump object action
*/
public class body_CS_OBJECT_JUMP : AsPacketHeader
{
	public Int32 nNpcIdx;

	public body_CS_OBJECT_JUMP( Int32 _nNpcIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_OBJECT_JUMP;
		nNpcIdx = _nNpcIdx;
	}
}


public class body_SC_OBJECT_JUMP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nNpcIdx;
}


public class body_SC_OBJECT_JUMP : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nNpcIdx;
}


#region -퀘스트-
class body_CS_QUEST_START : AsPacketHeader
{
	public Int32 nQuestTableIdx;

	public body_CS_QUEST_START( int _questTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_START;
		nQuestTableIdx = _questTableIdx;
	}
};


class body_CS_QUEST_START_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public Int32 nQuestTableIdx = 0;
};


class body_SC_QUEST_GET_ITEM_RESULT : AsPacketHeader
{
	public Int32 nItemTableIdx = 0;
	public Int16 nItemOverlapped = 0;
};


class body_CS_QUEST_CLEAR : AsPacketHeader
{
	public Int32 nQuestTableIdx;

	public body_CS_QUEST_CLEAR( int _questTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_CLEAR;
		nQuestTableIdx = _questTableIdx;
	}
};


class body_CS_QUEST_CLEAR_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public Int32 nQuestTableIdx = 0;
};


class body_CS_QUEST_DROP : AsPacketHeader
{
	public Int32 nQuestTableIdx;

	public body_CS_QUEST_DROP( int _questTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_DROP;
		nQuestTableIdx = _questTableIdx;
	}
};


class body_CS_QUEST_DROP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public Int32 nQuestTableIdx = 0;
};


class body_CS_QUEST_COMPLETE : AsPacketHeader
{
	public Int32 nQuestTableIdx;
	public byte nRewardSelectItemIdx;

	public body_CS_QUEST_COMPLETE( int _questTableIdx, byte _selectItemIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_COMPLETE;
		nQuestTableIdx = _questTableIdx;
		nRewardSelectItemIdx = _selectItemIdx;
	}
};


class body_CS_QUEST_COMPLETE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public Int32 nQuestTableIdx = 0;
};


class body1_SC_QUEST_RUNNING_LIST : AsPacketHeader
{
	public Int16 nCount = 0;
	public body2_SC_QUEST_RUNNING_LIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCount
		byte[] count = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( count, 0));
		index += sizeof( Int16);

		body = new body2_SC_QUEST_RUNNING_LIST[nCount];
		for( int i = 0; i < nCount; i++)
		{
			body[i] = new body2_SC_QUEST_RUNNING_LIST();
			body[i].nValue = new short[body2_SC_QUEST_RUNNING_LIST.nValueMax];
	
			byte[] tmpData = new byte[body2_SC_QUEST_RUNNING_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_QUEST_RUNNING_LIST.size;
		}
	}
};


class body2_SC_QUEST_RUNNING_LIST : AsPacketHeader
{
	public static int size = 4 + 2 + 1 + 10;
	public static int nValueMax = 5;
	
	public Int32 nQuestIndex = 0;
	public Int16 nStatus = 0;
	public Byte nRepeat = 0;
	public Int16[] nValue = null;
};


class body1_SC_QUEST_FAILLIST_RESULT : AsPacketHeader
{
	public Int16 nCount = 0;
	public body2_SC_QUEST_FAILLIST_RESULT[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCount
		byte[] count = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( count, 0));
		index += sizeof( Int16);

		body = new body2_SC_QUEST_FAILLIST_RESULT[nCount];
		for( int i = 0; i < nCount; i++)
		{
			body[i] = new body2_SC_QUEST_FAILLIST_RESULT();

			byte[] tmpData = new byte[body2_SC_QUEST_FAILLIST_RESULT.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_QUEST_FAILLIST_RESULT.size;
		}
	}
};


class body_SC_QUEST_NPC_KILL_RESULT : AsPacketHeader
{
	public Int32 nNpcTableIdx = 0;
	public bool  bChampion;
};


class body2_SC_QUEST_FAILLIST_RESULT : AsPacketHeader
{
	public static int size = 4;
	public Int32 nQuestTableIdx = 0;
};


class body1_SC_QUEST_COMPLETE_LIST : AsPacketHeader
{
	public Int16 nCount = 0;
	public body2_SC_QUEST_COMPLETE_LIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCount
		byte[] count = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( count, 0));
		index += sizeof( Int16);

		body = new body2_SC_QUEST_COMPLETE_LIST[nCount];
		for( int i = 0; i < nCount; i++)
		{
			body[i] = new body2_SC_QUEST_COMPLETE_LIST();
	
			byte[] tmpData = new byte[body2_SC_QUEST_COMPLETE_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_QUEST_COMPLETE_LIST.size;
		}
	}
};


class body2_SC_QUEST_COMPLETE_LIST : AsPacketHeader
{
	public static int size = 1 + 4;
	public Int32 nQuestIndex = 0;
	public Byte nRepeat = 0;
};


class body_CS_QUEST_CLEAR_ENTER_MAP : AsPacketHeader
{
	public Int32 nQuestTableIdx;

	public body_CS_QUEST_CLEAR_ENTER_MAP( int _questTableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_CLEAR_ENTER_MAP;
		nQuestTableIdx = _questTableIdx;
	}
};


class body_SC_QUEST_CLEAR_ENTER_MAP_RESULT : AsPacketHeader
{
	public Int32 nQuestTableIdx = 0;
	public Int32 nCond = 0;
};


class body_SC_QUEST_PROC_ITEM_USE_ENTER_MAP : AsPacketHeader
{
	public Int32 nQuestTableIdx = 0;
	public Int16 nCond = 0;
};


class body_CS_QUEST_CLEAR_TALK : AsPacketHeader
{
	public Int32 nNpcSpwanIdx;

	public body_CS_QUEST_CLEAR_TALK( int _npcSpwanIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_CLEAR_TALK;
		nNpcSpwanIdx = _npcSpwanIdx;
	}
};


class body_SC_QUEST_CLEAR_TALK_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_NOTHING;
	
	public Int32 nNPCTableID = 0;
};


class body_SC_QUEST_DAILY_LIST_RESULT : AsPacketHeader
{
	public Int32[] nQuestTables = new Int32[15];
};


class body_SC_QUEST_DAILY_RESET_RESULT : AsPacketHeader
{
	//clear 된 녀석에서 daily 만 골라 리셋
};


class body1_SC_QUEST_DAILY_LIST_RESULT : AsPacketHeader
{
	public Int32 nCnt;

	public body2_SC_QUEST_DAILY_LIST_RESULT[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		byte[] cnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, cnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( cnt, 0));
		index += sizeof( Int32);

		body = new body2_SC_QUEST_DAILY_LIST_RESULT[nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_QUEST_DAILY_LIST_RESULT();

			byte[] tmpData = new byte[sizeof( Int32)];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += sizeof( Int32);
		}
	}
};


class body2_SC_QUEST_DAILY_LIST_RESULT : AsPacketHeader
{
	public Int32 nQuestTableIdx;
};


class body1_SC_QUEST_GROUP_LIST : AsPacketHeader
{
	public Int32 nCnt;

	public body2_SC_QUEST_GROUP_LIST[] body = null;

	public new void PacketBytesToClass(byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader(data);

		// nCnt
		byte[] cnt = new byte[sizeof(Int32)];
		headerinfo = infotype.GetField("nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy(data, index, cnt, 0, sizeof(Int32));
		headerinfo.SetValue(this, BitConverter.ToInt32(cnt, 0));
		index += sizeof(Int32);

		body = new body2_SC_QUEST_GROUP_LIST[nCnt];
		for (int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_QUEST_GROUP_LIST();

			byte[] tmpData = new byte[sizeof(Int32)];
			Buffer.BlockCopy(data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass(tmpData);
			index += sizeof(Int32);
		}
	}
};

class body2_SC_QUEST_GROUP_LIST : AsPacketHeader
{
	public Int32 nQuestTableIdx;
};


class body_CS_QUEST_DAILY_LIST : AsPacketHeader
{
	public body_CS_QUEST_DAILY_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_DAILY_LIST;
	}
};


class body_CS_QUEST_WARP : AsPacketHeader
{
	public Int32 nQuestTableIdx;
	public Int32 nMapIdx;
	public Vector3 vPosition;

	public body_CS_QUEST_WARP( int _qustID, int _mapID, Vector3 _vPos)
	{
		nQuestTableIdx = _qustID;
		nMapIdx = _mapID;
		vPosition = _vPos;
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_WARP;
	}
};


public class body_CS_QUEST_CLEAR_NOW : AsPacketHeader
{
	public Int32 nQuestTableIdx;

	public body_CS_QUEST_CLEAR_NOW( int _questTableIdx)
	{
		nQuestTableIdx = _questTableIdx;
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_CLEAR_NOW;
	}
};


public class body_SC_QUEST_CLEAR_NOW_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	
	public Int32 nQuestTableIdx = 0;
};


//#if KB
public class body_CS_QUEST_CLEAR_OPEN_UI : AsPacketHeader
{
	public int nUIType;

	public body_CS_QUEST_CLEAR_OPEN_UI( OpenUIType _uiType)
	{
		nUIType = (int)_uiType;
	
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_CLEAR_OPEN_UI;
	}
};


public class body_SC_QUEST_CLEAR_OPEN_UI_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public int nUIType;
};


public class body_SC_QUEST_INFO : AsPacketHeader
{
	public Int32 nQuestTableIdx;
	public Int32 nCond;
	public Int32 nValue;
};


public class body_CS_QUEST_INFO : AsPacketHeader
{
	public Int32 nQuestTableIdx;
	public Int32 nCond;

	public body_CS_QUEST_INFO( Int32 _questTableIdx, Int32 _cond)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_QUEST_INFO;
	
		nQuestTableIdx = _questTableIdx;
		nCond = _cond;
	}
};

public class body_SC_QUEST_WARP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
};
#endregion


// < trade
public class body_CS_TRADE_REQUEST : AsPacketHeader
{
	public UInt16 nResponseSessionIdx;

	public body_CS_TRADE_REQUEST( UInt16 nResponseSessionID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_REQUEST;
		nResponseSessionIdx = nResponseSessionID;
	}
}


public class body_SC_TRADE_REQUEST : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nRequestSessionIdx;
}


public class body_CS_TRADE_RESPONSE : AsPacketHeader
{
	public bool bAccept;
	public UInt16 nRequestSessionIdx;

	public body_CS_TRADE_RESPONSE( bool bAcc, UInt16 nRequestSessionID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_RESPONSE;
		bAccept = bAcc;
		nRequestSessionIdx = nRequestSessionID;
	}
}


public class body_SC_TRADE_RESPONSE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nResponseSessionIdx;
}


public class body_CS_TRADE_REGISTRATION_ITEM : AsPacketHeader
{
	public bool bAddOrDel; //true:add, false:del
	public Int32 nInvenSlot;
	public Int32 nTradeSlot;
	public Int16 nItemCount;

	public body_CS_TRADE_REGISTRATION_ITEM( bool bAdd, Int32 invenSlot, Int32 tradeSlot, Int16 itemCount)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_REGISTRATION_ITEM;
		bAddOrDel = bAdd;
		nInvenSlot = invenSlot;
		nTradeSlot = tradeSlot;
		nItemCount = itemCount;
	}
}


public class body_SC_TRADE_REGISTRATION_ITEM : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIdx;
	public bool bAddOrDel; //true:add, false:del
	public Int32 nInvenSlot;
	public Int32 nTradeSlot;
	public sITEM sTradeItem;
}


public class body_CS_TRADE_REGISTRATION_GOLD : AsPacketHeader
{
	public UInt64 nAddTotalGold;

	public body_CS_TRADE_REGISTRATION_GOLD( UInt64 nGold)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_REGISTRATION_GOLD;
		nAddTotalGold = nGold;
	}
}


public class body_SC_TRADE_REGISTRATION_GOLD : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIdx;
	public Int64 nTotalGold;
}


public class body_CS_TRADE_LOCK : AsPacketHeader
{
	public bool bLock;

	public body_CS_TRADE_LOCK( bool _bLock)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_LOCK;
		bLock = _bLock;
	}
}


public class body_SC_TRADE_LOCK : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIdx;
	public bool bLock; //true : ready
}


public class body_CS_TRADE_OK : AsPacketHeader
{
	public body_CS_TRADE_OK()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_OK;
	}
}


public class body_SC_TRADE_OK : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class body_CS_TRADE_CANCEL : AsPacketHeader
{
	public body_CS_TRADE_CANCEL()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_TRADE_CANCEL;
	}
}


public class body_SC_TRADE_CANCEL : AsPacketHeader
{
}
// trade >


public class body_SC_ATTR_AP : AsPacketHeader
{
    public int nAddApp;
}


#region -PostBox
public class sPOSTITEM
{
	public Int32 nInentorySlot;
	public Int32 nOverlapped;

	public sPOSTITEM()
	{
		nInentorySlot = -1;
		nOverlapped = 0;
	}
}


public class body_CS_POST_SEND : AsPacketHeader
{
	public UInt64 nGold;
	public sPOSTITEM sSendPostItem1 = new sPOSTITEM();
	public sPOSTITEM sSendPostItem2 = new sPOSTITEM();
	public sPOSTITEM sSendPostItem3 = new sPOSTITEM();
	public sPOSTITEM sSendPostItem4 = new sPOSTITEM();
	public Int16 nReciverNameLength;
	public Int16 nTitleLength;
	public Int16 nContentLength;
	public byte[] szReciever = null;
	public byte[] szTitle = null;
	public byte[] szContent = null;

	public body_CS_POST_SEND( string sender, string title, string content, UInt64 gold, List<sPOSTITEM> items)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_SEND;

		nGold = gold;
		sSendPostItem1.nInentorySlot = items[0].nInentorySlot;
		sSendPostItem1.nOverlapped = items[0].nOverlapped;
		sSendPostItem2.nInentorySlot = items[1].nInentorySlot;
		sSendPostItem2.nOverlapped = items[1].nOverlapped;
		sSendPostItem3.nInentorySlot = items[2].nInentorySlot;
		sSendPostItem3.nOverlapped = items[2].nOverlapped;
		sSendPostItem4.nInentorySlot = items[3].nInentorySlot;
		sSendPostItem4.nOverlapped = items[3].nOverlapped;

		szReciever = System.Text.UTF8Encoding.UTF8.GetBytes( sender);
		szTitle = System.Text.UTF8Encoding.UTF8.GetBytes( title);
		szContent = System.Text.UTF8Encoding.UTF8.GetBytes( content);

		nReciverNameLength = (Int16)szReciever.Length;
		nTitleLength = (Int16)szTitle.Length;
		nContentLength = (Int16)szContent.Length;
	}
}


public class body_SC_POST_SEND_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class body_CS_POST_LIST : AsPacketHeader
{
	public body_CS_POST_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_LIST;
	}
}


public class body1_SC_POST_LIST_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nPostCount;
	public body2_SC_POST_LIST_RESULT[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nPostCount
		headerinfo = infotype.GetField( "nPostCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		body = new body2_SC_POST_LIST_RESULT[ nPostCount];
		for( int i = 0; i < nPostCount; i++)
		{
			body[i] = new body2_SC_POST_LIST_RESULT();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
}


public enum ePOSTTYPE
{
	ePOSTTYPE_NOTHING = 0,
	ePOSTTYPE_PRIVATE_SHOP,
	ePOSTTYPE_ATTENDANCE,
	ePOSTTYPE_RETURN,
	ePOSTTYPE_INVITE_FRIEND,
	ePOSTTYPE_GUILD_EXPULSION,
	ePOSTTYPE_HELLO_SEND,
	ePOSTTYPE_HELLO_RECV,
	ePOSTTYPE_HELLO_SEND_RECV,
	ePOSTTYPE_RECALL_SEND,
	ePOSTTYPE_RECALL_RECV,
	ePOSTTYPE_CONNECT_EVENT,
	ePOSTTYPE_RECOMMEND_1,
	ePOSTTYPE_RECOMMEND_2,
	ePOSTTYPE_RECOMMEND_REWARD_1,
	ePOSTTYPE_RECOMMEND_REWARD_2,
	ePOSTTYPE_RECOMMEND_ACCRUE,
	ePOSTTYPE_CASH_GIFT,
	ePOSTTYPE_COUPON,
	ePOSTTYPE_LEVEL_COMPLETE,
	ePOSTTYPE_GM_SEND,
	ePOSTTYPE_WEME_EVENT,
	ePOSTTYPE_WEME_COMPENSATION,
	ePOSTTYPE_DAILY_REWARD,
	ePOSTTYPE_INDUN_FIRST_REWARD,
	ePOSTTYPE_MAX
}


public class body2_SC_POST_LIST_RESULT : AsPacketHeader
{
	public static int size = 1 + 8 + 1 + 4 + 8 + 8 + 8 + 8 + ( sITEM.size * 4) + 4 + 2 + 2 + 2;

	public bool bRead;
	public UInt64 nPostSerial;
	public bool bAccount;
	public Int32 nPostType;
	public Int64 nPostSubValue1;
	public Int64 nPostSubValue2;
	public Int64 nPostSubValue3;
	public UInt64 nGold;
	public sITEM sRecievItem1;
	public sITEM sRecievItem2;
	public sITEM sRecievItem3;
	public sITEM sRecievItem4;
	public UInt32 nDeleteTime;
	public Int16 nSendNameLength;
	public Int16 nTitleLength;
	public Int16 nContentLength;
	public string senderName;
	public string title;
	public string content;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// bRead
		byte[] tempRead = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, tempRead, 0, sizeof( bool));
		FieldInfo headerinfo = infotype.GetField( "bRead", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( tempRead, 0));
		index += sizeof( bool);

		// nPostSerial
		byte[] tempPostSerial = new byte[ sizeof( UInt64)];
		Buffer.BlockCopy( data, index, tempPostSerial, 0, sizeof( UInt64));
		headerinfo = infotype.GetField( "nPostSerial", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt64( tempPostSerial, 0));
		index += sizeof( UInt64);

		// bAccount
		byte[] tempAccount = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, tempAccount, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bAccount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( tempAccount, 0));
		index += sizeof( bool);

		// nPostType
		byte[] tempPostType = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, tempPostType, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nPostType", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( tempPostType, 0));
		index += sizeof( Int32);

		// nPostSubValue1
		byte[] tempPostSubValue1 = new byte[ sizeof( Int64)];
		Buffer.BlockCopy( data, index, tempPostSubValue1, 0, sizeof( Int64));
		headerinfo = infotype.GetField( "nPostSubValue1", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt64( tempPostSubValue1, 0));
		index += sizeof( Int64);

		// nPostSubValue2
		byte[] tempPostSubValue2 = new byte[ sizeof( Int64)];
		Buffer.BlockCopy( data, index, tempPostSubValue2, 0, sizeof( Int64));
		headerinfo = infotype.GetField( "nPostSubValue2", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt64( tempPostSubValue2, 0));
		index += sizeof( Int64);

		// nPostSubValue3
		byte[] tempPostSubValue3 = new byte[ sizeof( Int64)];
		Buffer.BlockCopy( data, index, tempPostSubValue3, 0, sizeof( Int64));
		headerinfo = infotype.GetField( "nPostSubValue3", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt64( tempPostSubValue3, 0));
		index += sizeof( Int64);

		// nGold
		byte[] tempGold = new byte[ sizeof( UInt64)];
		Buffer.BlockCopy( data, index, tempGold, 0, sizeof( UInt64));
		headerinfo = infotype.GetField( "nGold", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt64( tempGold, 0));
		index += sizeof( UInt64);

		sRecievItem1 = new sITEM();
		byte[] tempRecievItem1 = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, tempRecievItem1, 0, sITEM.size);
		sRecievItem1.ByteArrayToClass( tempRecievItem1);
		index += sITEM.size;

		sRecievItem2 = new sITEM();
		byte[] tempRecievItem2 = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, tempRecievItem2, 0, sITEM.size);
		sRecievItem2.ByteArrayToClass( tempRecievItem2);
		index += sITEM.size;

		sRecievItem3 = new sITEM();
		byte[] tempRecievItem3 = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, tempRecievItem3, 0, sITEM.size);
		sRecievItem3.ByteArrayToClass( tempRecievItem3);
		index += sITEM.size;

		sRecievItem4 = new sITEM();
		byte[] tempRecievItem4 = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, tempRecievItem4, 0, sITEM.size);
		sRecievItem4.ByteArrayToClass( tempRecievItem4);
		index += sITEM.size;

		// nDeleteTime
		byte[] tempDeleteTime = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, tempDeleteTime, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nDeleteTime", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempDeleteTime, 0));
		index += sizeof( UInt32);

		// nSendNameLength
		byte[] tempSendNameLength = new byte[ sizeof(Int16)];
		Buffer.BlockCopy( data, index, tempSendNameLength, 0, sizeof(Int16));
		headerinfo = infotype.GetField( "nSendNameLength", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt16( tempSendNameLength, 0));
		index += sizeof(Int16);

		// nTitleLength
		byte[] tempTitleLength = new byte[ sizeof(Int16)];
		Buffer.BlockCopy( data, index, tempTitleLength, 0, sizeof(Int16));
		headerinfo = infotype.GetField( "nTitleLength", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt16( tempTitleLength, 0));
		index += sizeof(Int16);

		// nContentLength
		byte[] tempContentLength = new byte[ sizeof(Int16)];
		Buffer.BlockCopy( data, index, tempContentLength, 0, sizeof(Int16));
		headerinfo = infotype.GetField( "nContentLength", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt16( tempContentLength, 0));
		index += sizeof(Int16);

		byte[] tempSenderName = new byte[ nSendNameLength];
		Buffer.BlockCopy( data, index, tempSenderName, 0, nSendNameLength);
		senderName = System.Text.UTF8Encoding.UTF8.GetString( tempSenderName);
		index += nSendNameLength;

		byte[] tempTitle = new byte[ nTitleLength];
		Buffer.BlockCopy( data, index, tempTitle, 0, nTitleLength);
		title = System.Text.UTF8Encoding.UTF8.GetString( tempTitle);
		index += nTitleLength;

		byte[] tempContent = new byte[ nContentLength];
		Buffer.BlockCopy( data, index, tempContent, 0, nContentLength);
		content = System.Text.UTF8Encoding.UTF8.GetString( tempContent);
		index += nContentLength;

		return index;
	}
}

public class body2_CS_POST_DELETE : AsPacketHeader
{
	public UInt64 nPostSerial;
	
	public body2_CS_POST_DELETE( UInt64 serial)
	{
		nPostSerial = serial;
	}
}

public class body1_CS_POST_DELETE : AsPacketHeader
{
	public Int32 nPostPage;
	public Int32 nBody2Cnt;
	public body2_CS_POST_DELETE[] bodies;
	
	public body1_CS_POST_DELETE( Int32 page, body2_CS_POST_DELETE[] bodies)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_DELETE;
		
		nPostPage = page;
		nBody2Cnt = bodies.Length;
		this.bodies = bodies;
	}
}

#if false
public class body_CS_POST_DELETE : AsPacketHeader
{
	public UInt64 nPostSerial;
	public Int32 nPostPage;

	public body_CS_POST_DELETE( UInt64 serial, Int32 page)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_DELETE;

		nPostSerial = serial;
		nPostPage = page;
	}
}
#endif

public class body_CS_POST_READ : AsPacketHeader
{
	public UInt64 nPostSerial;

	public body_CS_POST_READ( UInt64 serial)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte) PROTOCOL_CS.CS_POST_READ;

		nPostSerial = serial;
	}
}

public class body_CS_POST_GOLD_RECIEVE : AsPacketHeader
{
	public UInt64 nPostSerial;

	public body_CS_POST_GOLD_RECIEVE( UInt64 serial)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_GOLD_RECEIVE;

		nPostSerial = serial;
	}
}


public class body_SC_POST_GOLD_RECIEVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class body2_CS_POST_ITEM_RECEIVE : AsPacketHeader
{
	public UInt64 nPostSerial;
	
	public body2_CS_POST_ITEM_RECEIVE( UInt64 serial)
	{
		nPostSerial = serial;
	}
}

public class body1_CS_POST_ITEM_RECEIVE : AsPacketHeader
{
	public Byte nPostItemSlot;
	public Int32 nBody2Cnt;
	public body2_CS_POST_ITEM_RECEIVE[] items;
	
	public body1_CS_POST_ITEM_RECEIVE( Byte itemSlot, body2_CS_POST_ITEM_RECEIVE[] items)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_ITEM_RECEIVE;
		
		nPostItemSlot = itemSlot;
		nBody2Cnt = items.Length;
		this.items = items;
	}
}

#if false
public class body_CS_POST_ITEM_RECIEVE : AsPacketHeader
{
	public UInt64 nPostSerial;
	public byte nPostItemSlot;

	public body_CS_POST_ITEM_RECIEVE( UInt64 serial, byte slot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_ITEM_RECEIVE;

		nPostSerial = serial;
		nPostItemSlot = slot;
	}
}
#endif

public class body_SC_POST_ITEM_RECIEVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class body_CS_POST_PAGE : AsPacketHeader
{
	public Int32 nPage;

	public body_CS_POST_PAGE( Int32 page)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_PAGE;

		nPage = page;
	}
}


public class body_SC_POST_NEWPOST : AsPacketHeader
{
	public bool bHave;
}


public enum ePOST_ADDRESS_BOOK_TYPE : int
{
	ePOST_ADDRESS_BOOK_TYPE_NOTHING = 0,

	ePOST_ADDRESS_BOOK_TYPE_FRIEND,
	ePOST_ADDRESS_BOOK_TYPE_GUILD,

	ePOST_ADDRESS_BOOK_TYPE_MAX
};


public class body_CS_POST_ADDRESS_BOOK : AsPacketHeader
{
	public ePOST_ADDRESS_BOOK_TYPE eType;
	public byte nPage;
	public bool bConnect;

	public body_CS_POST_ADDRESS_BOOK( ePOST_ADDRESS_BOOK_TYPE type, byte page, bool connect)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_POST_ADDRESS_BOOK;

		eType = type;
		nPage = page;
		bConnect = connect;
	}
}


public class body2_SC_POST_ADDRESS_BOOK : AsPacketHeader
{
	public string szCharName;
	public Int32 nLevel;
	public eCLASS eClass;
	public bool bConnect;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nLevel
		byte[] level = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, level, 0, sizeof( Int32));
		FieldInfo headerinfo = infotype.GetField( "nLevel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( level, 0));
		index += sizeof( Int32);

		// eClass
		byte[] cls = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, cls, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eClass", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( cls, 0));
		index += sizeof( Int32);

		// bConnect
		byte[] connect = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, connect, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bConnect", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( connect, 0));
		index += sizeof( bool);

		return index;
	}
}


public class body1_SC_POST_ADDRESS_BOOK : AsPacketHeader
{
	public eRESULTCODE eResult;
	public ePOST_ADDRESS_BOOK_TYPE eType;
	public byte nMaxPage;
	public byte nCnt;
	public body2_SC_POST_ADDRESS_BOOK[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;
		
		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// eType
		byte[] type = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, type, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( type, 0));
		index += sizeof( Int32);

		// nMaxPage
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nCnt
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// body
		body = new body2_SC_POST_ADDRESS_BOOK[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_POST_ADDRESS_BOOK();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
}
#endregion -PostBox


#region -Chatting
public class body_CS_CHAT_MESSAGE : AsPacketHeader
{
	public Int32 nSlotIndex;
	public bool  bUseFiltering;
	public Int16 nMsgLen;
	public byte[] pMsg = null;

	public body_CS_CHAT_MESSAGE(string chat, eCHATTYPE type, bool useFiltering = true, int slotIndex = 0)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		switch( type)
		{
		case eCHATTYPE.eCHATTYPE_MAP:
			Protocol = (byte)PROTOCOL_CS.CS_CHAT_MAP;
			break;
		case eCHATTYPE.eCHATTYPE_PUBLIC:
			Protocol = (byte)PROTOCOL_CS.CS_CHAT_LOCAL;
			break;
		case eCHATTYPE.eCHATTYPE_SERVER:
			Protocol = (byte)PROTOCOL_CS.CS_CHAT_WORLD;
			break;
		case eCHATTYPE.eCHATTYPE_PARTY:
			Protocol = (byte)PROTOCOL_CS.CS_CHAT_PARTY;
			break;
		case eCHATTYPE.eCHATTYPE_GUILD:
			Protocol = (byte)PROTOCOL_CS.CS_CHAT_GUILD;
			break;
		}
		bUseFiltering = useFiltering;
		pMsg = System.Text.UTF8Encoding.UTF8.GetBytes( chat);
		nMsgLen = (Int16)pMsg.Length;
		nSlotIndex = slotIndex;
	}
}


public class body_SC_CHAT_NAME : AsPacketHeader
{
	public Int16 nLength;
	public string szName;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nLength
		byte[] length = new byte[ sizeof( Int16)];
		FieldInfo headerinfo = infotype.GetField( "nLength", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, length, 0, sizeof(Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( length, 0));
		index += sizeof(Int16);

		// szName
		byte[] tempName = new byte[ nLength];
		Buffer.BlockCopy( data, index, tempName, 0, nLength);
		index += nLength;
		szName = System.Text.UTF8Encoding.UTF8.GetString( tempName);

		return index;
	}
}


public class body_SC_CHAT_MESSAGE : AsPacketHeader
{
	public bool bUseFiltering;
	public Int16 nLength;
	public string szMsg;

	public void PacketBytesToClass( byte[] data, int index)
	{
		// use filtering
		Type infotype	= this.GetType();
		byte useFilter	= data[index];
		bUseFiltering	= Convert.ToBoolean(useFilter);
		index += 1;

		// nLength
		byte[] length = new byte[ sizeof( Int16)];
		FieldInfo headerinfo = infotype.GetField( "nLength", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, length, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( length, 0));
		index += sizeof( Int16);

		// szMsg
		byte[] tempMsg = new byte[ nLength];
		Buffer.BlockCopy( data, index, tempMsg, 0, nLength);
		index += nLength;

		szMsg = System.Text.UTF8Encoding.UTF8.GetString( tempMsg);
	}
}


public class body_SC_CHAT_WITH_BALLOON_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nUserUniqKey; //Check BlockList 	2013.03.05
	public UInt32 nCharUniqKey;
	public body_SC_CHAT_NAME kName = new body_SC_CHAT_NAME();
	public body_SC_CHAT_MESSAGE kMessage = new body_SC_CHAT_MESSAGE();

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nUserUniqKey
		byte[] charUniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( charUniqKey, 0));
		index += sizeof( UInt32);

		// nCharUniqKey
		byte[] userUniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, userUniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( userUniqKey, 0));
		index += sizeof( UInt32);

		index = kName.PacketBytesToClass( data, index);
		kMessage.PacketBytesToClass( data, index);
	}
}


public class body_CS_CHAT_PRIVATE : AsPacketHeader
{
	public Int16 nNameLength;
	public byte[] pName = null;
	public Int32 nSlotIndex;
	public bool bUseFiltering;
	public Int16 nMsgLength;
	public byte[] pMsg = null;

	public body_CS_CHAT_PRIVATE( string receiver, string chat)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHAT_PRIVATE;

		pName = System.Text.UTF8Encoding.UTF8.GetBytes( receiver);
		nSlotIndex = 0;
		nNameLength = (Int16)pName.Length;
		bUseFiltering = true;
		pMsg = System.Text.UTF8Encoding.UTF8.GetBytes( chat);
		nMsgLength = (Int16)pMsg.Length;
	}
}


public class body_CS_CHAT_EMOTICON : AsPacketHeader
{
	public AsEmotionManager.eCHAT_FILTER eFilter;
	public int nIndex;

	public body_CS_CHAT_EMOTICON( AsEmotionManager.eCHAT_FILTER _filter, int _index)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHAT_EMOTICON;

		eFilter = _filter;
		nIndex = _index;
	}
}


public class body_SC_CHAT_EMOTICON_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public uint nUserUniqKey;
	public uint nCharUniqKey;
	public AsEmotionManager.eCHAT_FILTER eFilter;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public int nIndex;
}
#endregion -Chatting


#region -tutorial
public class body_SC_TUTORIAL_RESULT : AsPacketHeader
{
}
#endregion


#region -npc store
public class body_SC_SHOP_ITEMBUY : AsPacketHeader
{
	public eRESULTCODE eResult;
};


public class body_SC_SHOP_ITEMSELL : AsPacketHeader
{
	public eRESULTCODE eResult;
	//public UInt64 nRepurchaseItemIdx; //Repurchase Item Idx
};


public class body_CS_SHOP_USELESS_ITEMSELL : AsPacketHeader
{
	public Int32 nNpcIdx;

	public body_CS_SHOP_USELESS_ITEMSELL( Int32 _npcIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SHOP_USELESS_ITEMSELL;
		nNpcIdx = _npcIdx;
	}
};


public class body_SC_SHOP_USELESS_ITEMSELL : AsPacketHeader
{
	public eRESULTCODE eResult;
};


public class body_CS_SHOP_ITEMBUY : AsPacketHeader
{
	public Int32 nNpcIdx;
	public Int32 nShopItemSlot; //아이템 테이블 idx
	public Int32 nItemCount;
	
	public body_CS_SHOP_ITEMBUY( Int32 _npcIdx, Int32 _nShopItemSlot, Int32 _itemCount)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SHOP_ITEMBUY;
		nNpcIdx = _npcIdx;
		nShopItemSlot = _nShopItemSlot;
		nItemCount = _itemCount;
	}
};


public class body_CS_SHOP_ITEMSELL : AsPacketHeader
{
	public Int32 nNpcIdx;
	public Int32 nInvenSlot;
	public Int32 nItemCount;
	
	public body_CS_SHOP_ITEMSELL( Int32 _npcIdx, Int32 _invenSlot, Int32 _itemCount)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SHOP_ITEMSELL;
		nNpcIdx = _npcIdx;
		nInvenSlot = _invenSlot;
		nItemCount = _itemCount;
	}
};

public class body_CS_SHOP_INFO : AsPacketHeader
{
	public Int32 nNpcIdx;

	public body_CS_SHOP_INFO(int _npcIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_SHOP_INFO;
		nNpcIdx = _npcIdx;
	}
};

public class body1_SC_SHOP_INFO : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nBody2Cnt;
	public body2_SC_SHOP_INFO[] body = null;

	public new void PacketBytesToClass(byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader(data);

		// eResult
		byte[] itemCount = new byte[sizeof(Int32)];
		headerinfo = infotype.GetField("eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy(data, index, itemCount, 0, sizeof(Int32));
		headerinfo.SetValue(this, BitConverter.ToInt32(itemCount, 0));
		index += sizeof(Int32);

		// nBody2Cnt
		byte[] itemCnt = new byte[sizeof(Int32)];
		headerinfo = infotype.GetField("nBody2Cnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy(data, index, itemCnt, 0, sizeof(Int32));
		headerinfo.SetValue(this, BitConverter.ToInt32(itemCnt, 0));
		index += sizeof(Int32);

		body = new body2_SC_SHOP_INFO[nBody2Cnt];

		for (int i = 0; i < nBody2Cnt; i++)
		{
			body[i] = new body2_SC_SHOP_INFO();

			byte[] tmpData = new byte[body2_SC_SHOP_INFO.size];
			Buffer.BlockCopy(data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass(tmpData);
			index += body2_SC_SHOP_INFO.size;
		}
	}
};

public class body2_SC_SHOP_INFO : AsPacketHeader
{
	public static int size = sizeof(Int32) + sizeof(Int32) + sizeof(Int64);
	public Int32 nShopItemSlot;
	public Int32 nItemCount;
	public Int64 nResetTime;

	public override string ToString()
	{
		return "slot[" + nShopItemSlot + "] itemCount[" + nItemCount + "] ResetTime = " + nResetTime;
	}
};
#endregion


#region -cash-
class body_CS_CHARGE_ITEMLIST: AsPacketHeader
{
	public byte eCompanyType;
	//nothing
	public body_CS_CHARGE_ITEMLIST(eCHARGECOMPANYTYPE _companyType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHARGE_ITEMLIST;
		eCompanyType = (byte)_companyType;
	}
};


class body_CS_CHARGE_BUY : AsPacketHeader
{
	public byte nCompanyType = 0;
	public Int32 nChargeSlot = 0;
	public byte[] strIAPProductID = new byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1];
	public UInt32 nIAPUniqKey = 0;

	public body_CS_CHARGE_BUY( byte _nCompayType, UInt32 _nIAPUniquKey, int _nChargeSlot, string _strIAPProductID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHARGE_BUY;

		nCompanyType = _nCompayType;
		nChargeSlot = _nChargeSlot;
		nIAPUniqKey = _nIAPUniquKey;

		byte[] temp = System.Text.ASCIIEncoding.ASCII.GetBytes( _strIAPProductID);

		int nLength = temp.Length;

		if( temp.Length > AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1)
			nLength = AsGameDefine.MAX_CHARGE_IAPPRODUCTID +1;

		Buffer.BlockCopy( temp, 0, strIAPProductID, 0, nLength);
	}
};


class body_CS_CASHSHOP_ITEMBUY: AsPacketHeader
{
	public Int32 nShopItemSlot = 0;
	public Int32 nItemCount = 0;

	public body_CS_CASHSHOP_ITEMBUY( int _shopItemSlot, int _itemCount)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CASHSHOP_ITEMBUY;
	
		nShopItemSlot = _shopItemSlot;
		nItemCount = _itemCount;
	}
};


class body1_SC_CHARGE_ITEMLIST : AsPacketHeader
{
	public eCHARGECOMPANYTYPE eCompany;
	public UInt16 nCnt = 0;
	public body2_SC_CHARGE_ITEMLIST[] products = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eCompany
		byte[] companyType = new byte[1];
		headerinfo = infotype.GetField("eCompany", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, companyType, 0, sizeof(byte));
		headerinfo.SetValue( this, (eCHARGECOMPANYTYPE)companyType[0]);
		index += sizeof(byte);

		// nCnt
		byte[] itemCount = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, itemCount, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( itemCount, 0));
		index += sizeof( UInt16);

		products = new body2_SC_CHARGE_ITEMLIST[nCnt];
	
		for( int i = 0; i < nCnt; i++)
		{
			products[i] = new body2_SC_CHARGE_ITEMLIST();
			byte[] tmpData = new byte[body2_SC_CHARGE_ITEMLIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			products[i].ByteArrayToClass( tmpData);
			index += body2_SC_CHARGE_ITEMLIST.size;
		}
	}
};


public class body2_SC_CHARGE_ITEMLIST : AsPacketHeader
{
	public static int size = sizeof(Int32) + AsGameDefine.MAX_CHARGE_IAPPRODUCTID + sizeof(byte) + sizeof(eCHARGETYPE) + sizeof(UInt64) + sizeof(UInt64) + sizeof(Int32) + sizeof(Int32);
	public Int32 nChargeSlot = 0;
	public byte[] strIAPProductID = new byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1];
	public eCHARGETYPE eChargeType = eCHARGETYPE.eCHARGETYPE_NOTHING;
	public UInt64 nChargeCount = 0;
	public UInt64 nBonusCount = 0;
	public eSHOPITEMHIGHLIGHT eShopItemHighLight = eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE;
	public Int32 nRemainTime;

	public new int ByteArrayToClass(byte[] data)
	{
		System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
		System.IO.BinaryReader br = new System.IO.BinaryReader(ms);

		nChargeSlot = br.ReadInt32();
		strIAPProductID = br.ReadBytes(AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1);
		eChargeType = (eCHARGETYPE)br.ReadInt32();
		nChargeCount = br.ReadUInt64();
		nBonusCount = br.ReadUInt64();
		eShopItemHighLight = (eSHOPITEMHIGHLIGHT)br.ReadInt32();
		nRemainTime = br.ReadInt32();

		br.Close();
		ms.Close();

		return 0;
	}

	public override string ToString()
	{
		string iapID = System.Text.ASCIIEncoding.ASCII.GetString(strIAPProductID);
		return "[slot = " + nChargeSlot + "] [c = " + nChargeCount + "] [ " + eChargeType.ToString() + "] [ remain = " + nRemainTime + "]" + iapID;
	}
};


class body_SC_CHARGE_BUY : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
};


class body_SC_CASHSHOP_ITEMBUY : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
};


class body_CS_CHARGE_GIFT_FIND :AsPacketHeader
{
	public byte[] szGiftCharName = null;
	
	public bool Initilize( string _szNickname)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHARGE_GIFT_FIND;
		
		byte[] byteArray = System.Text.UTF8Encoding.UTF8.GetBytes( _szNickname);
		
		if( byteArray.Length > AsGameDefine.MAX_NAME_LEN)
		return false;
		
		szGiftCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
		
		Buffer.BlockCopy( byteArray, 0, szGiftCharName, 0, byteArray.Length);
		
		// end of string
		szGiftCharName[byteArray.Length] = 0;
		
		return true;
	}
};


class body_SC_CHARGE_GIFT_FIND : AsPacketHeader
{
	public uint nGiftAccount; // user id
	public int nLevel;
	public int eClass;
};


class body_CS_CHARGE_ANDROIDPUBLICKEY : AsPacketHeader
{
	public bool bIsTest; //true : test mode, false : real mode

	public body_CS_CHARGE_ANDROIDPUBLICKEY( bool _mode)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_CHARGE_ANDROIDPUBLICKEY;

		bIsTest = _mode;
	}
};


class body_SC_CHARGE_ANDROIDPUBLICKEY : AsPacketHeader
{
	public Byte[] strPublicKey = new byte[AsGameDefine.MAX_ANDROID_PUBLICKEY_LENGTH + 1];
};
#endregion


public class body_CS_WAYPOINT_LEVELACTIVE : AsPacketHeader
{
	public body_CS_WAYPOINT_LEVELACTIVE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_WAYPOINT_LEVELACTIVE;
	}
}


public class body2_SC_WAYPOINT_LEVELACTIVE_RESULT : AsPacketHeader
{
	public static int size = sizeof( Int32);

	public Int32 nWarpTableIdx = 0;
}


public class body1_SC_WAYPOINT_LEVELACTIVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public Int32 nCnt = 0;

	public body2_SC_WAYPOINT_LEVELACTIVE_RESULT[] datas = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] itemCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, itemCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( itemCount, 0));
		index += sizeof( Int32);

		// nCnt
		itemCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, itemCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( itemCount, 0));
		index += sizeof( Int32);

		datas = new body2_SC_WAYPOINT_LEVELACTIVE_RESULT[nCnt];
	
		for( int i = 0; i < nCnt; i++)
		{
			datas[i] = new body2_SC_WAYPOINT_LEVELACTIVE_RESULT();
			byte[] tmpData = new byte[body2_SC_WAYPOINT_LEVELACTIVE_RESULT.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			datas[i].ByteArrayToClass( tmpData);
			index += body2_SC_WAYPOINT_LEVELACTIVE_RESULT.size;
		}
	}
}


public class body1_SC_ITEM_TIME_EXPIRE : AsPacketHeader
{
	public Int32 nCnt = 0;

	public sITEM[] datas = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		byte[] itemCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, itemCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( itemCount, 0));
		index += sizeof( Int32);

		datas = new sITEM[nCnt];
	
		for( int i = 0; i < nCnt; i++)
		{
			datas[i] = new sITEM();
			byte[] tmpData = new byte[sITEM.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			datas[i].ByteArrayToClass( tmpData);
			index += sITEM.size;
		}
	}
}
