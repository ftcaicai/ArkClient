//#define USE_OLD_COSTUME
#define NEW_DELEGATE_IMAGE
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



//--------------------------------------------------------------------------------------------------
/* Common Packet Define */
//--------------------------------------------------------------------------------------------------

enum PROTOCOL_CS_2 : byte
{
	CS_NOTHING,

	SC_GUILD_LOAD_REUSLT,
	CS_GUILD_INFO,
	SC_GUILD_INFO_RESULT,
	CS_GUILD_INFO_DETAIL,
	SC_GUILD_INFO_DETAIL_RESULT,
	SC_GUILD_SEARCH_RESULT,
	CS_GUILD_CREATE,
	SC_GUILD_CREATE_RESULT,
	CS_GUILD_NOTICE,
	SC_GUILD_NOTICE_RESULT,
	CS_GUILD_INVITE,
	SC_GUILD_INVITE,
	SC_GUILD_INVITE_RESULT,
	CS_GUILD_JOIN,
	SC_GUILD_JOIN_RESULT,
	CS_GUILD_SEARCH_JOIN,
	SC_GUILD_SEARCH_JOIN_RESULT,
	CS_GUILD_EXIT,
	SC_GUILD_EXIT_RESULT,
	CS_GUILD_EXILE,
	SC_GUILD_EXILE,
	SC_GUILD_EXILE_RESULT,
	CS_GUILD_CHANGE_PERMISSION,
	SC_GUILD_CHANGE_PERMISSION_RESULT,
	CS_GUILD_MEMBER_INFO,
	SC_GUILD_MEMBER_INFO_RESULT,
	CS_GUILD_SUPERVISE,
	SC_GUILD_SUPERVISE_RESULT,
//	CS_GUILD_NOT_APPROVE_MEMBER_INFO,
	SC_GUILD_NOT_APPROVE_MEMBER_INFO_RESULT,
	CS_GUILD_PUBLICIZE,
	SC_GUILD_PUBLICIZE_RESULT,
	CS_GUILD_SEARCH_JOIN_APPROVE,
	SC_GUILD_SEARCH_JOIN_APPROVE_RESULT,
	CS_GUILD_LEVEL_UP,
	SC_GUILD_LEVEL_UP_RESULT,
	SC_GUILD_MEMBER_DELETE_RESULT,
	CS_GUILD_MASTER_CHANGE,
	SC_GUILD_MASTER_CHANGE_RESULT,
	//CS_GUILD_MEMBER_INFO_DETAIL,
	//SC_GUILD_MEMBER_INFO_DETAIL_RESULT,
	CS_GUILD_UI_SCROLL,
	SC_GUILD_UI_SCROLL_RESULT,
	SC_GUILD_NAME_NOTIFY,
	SC_GUILD_NOTICE_NOTIFY,
	CS_GUILD_NAME_CHANGE,
	SC_GUILD_NAME_CHANGE,
	SC_GUILD_NAME_NOTIFY_CHANGE,

	CS_PRIVATESHOP_CREATE,
	SC_PRIVATESHOP_CREATE,
	CS_PRIVATESHOP_DESTROY,
	SC_PRIVATESHOP_DESTROY,
	CS_PRIVATESHOP_MODIFY,
	SC_PRIVATESHOP_MODIFY,
	CS_PRIVATESHOP_REGISTRATION_ITEM,
	SC_PRIVATESHOP_REGISTRATION_ITEM,
	CS_PRIVATESHOP_OPEN,
	SC_PRIVATESHOP_OPEN,
	CS_PRIVATESHOP_CLOSE,
	SC_PRIVATESHOP_CLOSE,
	SC_PRIVATESHOP_LIST,
	CS_PRIVATESHOP_ENTER,
	SC_PRIVATESHOP_ENTER,
	CS_PRIVATESHOP_LEAVE,
	SC_PRIVATESHOP_LEAVE,
	SC_PRIVATESHOP_ITEMLIST,
	SC_PRIVATESHOP_OWNER_ITEMLIST,
	CS_PRIVATESHOP_ITEMBUY,
	SC_PRIVATESHOP_ITEMBUY,
	SC_PRIVATESHOP_OWNER_ITEMBUY,
	
	// pstore search
	CS_PRIVATESHOP_SEARCH,
	SC_PRIVATESHOP_SEARCH_RESULT,

	CS_SOCIAL_INFO,
	SC_SOCIAL_INFO,
	SC_SOCIAL_HISTORY,
	CS_SOCIAL_NOTICE,
	SC_SOCIAL_NOTICE,
	CS_SOCIAL_UI_SCROLL,
	SC_SOCIAL_UI_SCROLL,
	CS_SOCIAL_ITEMBUY,
	SC_SOCIAL_ITEMBUY_RESULT,
	CS_SOCIAL_ITEMSELL,
	SC_SOCIAL_ITEMSELL_RESULT,

	CS_SOCIAL_HISTORY_REGISTER,
	SC_SOCIAL_HISTORY_REGISTER_RESULT,

	SC_FRIEND_CONNECT,
	CS_FRIEND_LIST,
	SC_FRIEND_LIST,
	CS_FRIEND_INVITE,
	SC_FRIEND_INVITE,
	SC_FRIEND_INVITE_RESULT,
	CS_FRIEND_JOIN,
	SC_FRIEND_JOIN_RESULT,
	SC_FRIEND_INSERT,
	CS_FRIEND_CLEAR,
	SC_FRIEND_CLEAR_RESULT,
	SC_FRIEND_DELETE,
	CS_FRIEND_HELLO,
	SC_FRIEND_HELLO,
	SC_FRIEND_HELLO_RESULT,
	CS_FRIEND_RECALL,
	SC_FRIEND_RECALL,
	SC_FRIEND_RECALL_RESULT,
	CS_FRIEND_RANDOM,
	SC_FRIEND_RANDOM,
	CS_FRIEND_WARP,
	SC_FRIEND_WARP,
	CS_FRIEND_CONDITION,				//2013.10.30.
	SC_FRIEND_CONDITION_RESULT,			//2013.10.30.

	CS_FRIEND_CONNECT_REQUEST,					
	SC_FRIEND_CONNECT_REQUEST_RESULT,
	
	SC_BLOCKOUT_LIST,
	CS_BLOCKOUT_INSERT,
	SC_BLOCKOUT_INSERT,
	CS_BLOCKOUT_DELETE,
	SC_BLOCKOUT_DELETE,

	CS_GAME_INVITE_LIST,
	SC_GAME_INVITE_LIST_RESULT,
	CS_GAME_INVITE,
	SC_GAME_INVITE_RESULT,
	CS_GAME_INVITE_REWARD,				//2013.10.30.
	SC_GAME_INVITE_REWARD_RESULT,		//2013.10.30.

	CS_SUBTITLE_LIST,
	SC_SUBTITLE_LIST,
	CS_SUBTITLE_SET,
	SC_SUBTITLE_SET_RESULT,
	SC_SUBTITLE_ADD,
	SC_SUBTITLE_ADD_NOTIFY,
	SC_SUBTITLE_CHANGE_NOTIFY,
	CS_SUBTITLE_INDEX_REWARD,
	SC_SUBTITLE_INDEX_REWARD_RESULT,
	CS_SUBTITLE_ACCURE_REWARD,
	SC_SUBTITLE_ACCURE_REWARD_RESULT,

	#region -DelegateImage
	CS_IMAGE_LIST,
	SC_IMAGE_LIST,
	CS_IMAGE_BUY,
	SC_IMAGE_BUY_RESULT,
	CS_IMAGE_SET,
	SC_IMAGE_SET_RESULT,
#if NEW_DELEGATE_IMAGE
	SC_IMAGE_CHANGE_NOTIFY,
#endif
	#endregion

	CS_ITEM_PRODUCT_INFO,
	SC_ITEM_PRODUCT_INFO,
	CS_ITEM_PRODUCT_TECHNIQUE_REGISTER,
	SC_ITEM_PRODUCT_TECHNIQUE_REGISTER,
	CS_ITEM_PRODUCT_PROGRESS,
	SC_ITEM_PRODUCT_PROGRESS,
	CS_ITEM_PRODUCT_REGISTER,
	SC_ITEM_PRODUCT_REGISTER,
	CS_ITEM_PRODUCT_CANCEL,
	SC_ITEM_PRODUCT_CANCEL,
	CS_ITEM_PRODUCT_RECEIVE,
	SC_ITEM_PRODUCT_RECEIVE,
	CS_ITEM_PRODUCT_CASH_DIRECT,
	SC_ITEM_PRODUCT_CASH_DIRECT,
	CS_ITEM_PRODUCT_CASH_SLOT_OPEN,
	SC_ITEM_PRODUCT_CASH_SLOT_OPEN,
	CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN,
	SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN,
	CS_ITEM_PRODUCT_CASH_LEVEL_UP,
	SC_ITEM_PRODUCT_CASH_LEVEL_UP,

	SC_ITEM_PRODUCT_SLOT_INFO,
	SC_ITEM_PRODUCT_TECHNIQUE_INFO,
	SC_ITEM_PRODUCT_STATE,

	CS_COLLECT_REQUEST,
	SC_COLLECT_RESULT,
	SC_COLLECT_INFO,

	CS_ITEM_ENCHANT,
	SC_ITEM_ENCHANT_RESULT,
	CS_ITEM_ENCHANT_OUT,
	SC_ITEM_ENCHANT_OUT_RESULT,

	CS_ITEM_STRENGTHEN,
	SC_ITEM_STRENGTHEN_RESULT,

	CS_ITEM_MIX,
	SC_ITEM_MIX_RESULT,

	CS_COS_ITEM_MIX_UP,	
	SC_COS_ITEM_MIX_UP_RESULT,
	CS_COS_ITEM_MIX_UPGRADE,
	SC_COS_ITEM_MIX_UPGRADE_RESULT,

	SC_ITEM_TIME_EXPIRE,

	SC_BONUS_ATTENDANCE,
	SC_BONUS_RETURN,

	CS_OTHER_INFO,
	SC_OTHER_INFO,

	CS_COSTUME_ONOFF,
	SC_COSTUME_ONOFF,


	CS_RESURRECT_PENALTY_CLEAR,
	SC_RESURRECT_PENALTY_CLEAR,

	CS_COUPON_REGIST,	//
	SC_COUPON_REGIST,
	/* Old Recommend
	CS_RECOMMEND_INFO,
	SC_RECOMMEND_INFO,

	CS_RECOMMEND_RECEIVE,
	SC_RECOMMEND_RECEIVE,
	*/
	CS_EVENT_LIST,
	SC_EVENT_LIST,

	SC_SERVER_EVENT_START,
	SC_SERVER_EVENT_STOP,

	SC_GAME_REVIEW,
	CS_GAME_REVIEW,

	SC_USER_EVENT_NOTIFY,

	SC_WANTED_START,
	SC_WANTED_CLEAR,
	CS_WANTED_COMPLETE,
	SC_WANTED_COMPLETE,

	#region -Condition_Protocol
	SC_CONDITION,
	#endregion

	#region -Ranking
	CS_RANK_MYRANK_LOAD,
	SC_RANK_MYRANK_LOAD_RESULT,
	CS_RANK_ITEM_MYRANK_LOAD,
	SC_RANK_ITEM_MYRANK_LOAD_RESULT,
	CS_RANK_ITEM_TOP_LOAD,
	SC_RANK_ITEM_TOP_LOAD_RESULT,
	CS_RANK_ITEM_MYFRIEND_LOAD,
	SC_RANK_ITEM_MYFRIEND_LOAD_RESULT,
	SC_RANK_CHANGE_MYRANK,
	SC_RANK_CHANGE_RANKPOINT,
	#endregion

	#region -Instance_Dungeon
	CS_INSTANCE_CREATE,
	SC_INSTANCE_CREATE_RESULT,
	SC_INSTANCE_CREATE,
	CS_ENTER_INSTANCE,
	SC_ENTER_INSTANCE_RESULT,
	CS_ENTER_INSTANCE_END,
	CS_EXIT_INSTANCE,
	SC_EXIT_INSTANCE_RESULT,
	CS_EXIT_INSTANCE_END,
	#endregion

	#region -Pvp
	CS_ARENA_MATCHING_INFO,
	SC_ARENA_MATCHING_INFO_RESULT,
	CS_ARENA_MATCHING_REQUEST,
	SC_ARENA_MATCHING_REQUEST_RESULT,
	CS_ARENA_MATCHING_GOINTO,
	SC_ARENA_MATCHING_GOINTO_RESULT,
	SC_ARENA_MATCHING_NOTIFY,
	SC_ARENA_MATCHING_LIMITCOUNT,
	SC_ARENA_INITILIZE,
	SC_ARENA_GOGOGO,
	SC_ARENA_USERINFO_LIST,
	SC_ARENA_ENTERUSER,
	SC_ARENA_REWARDINFO,
	SC_ARENA_READY,
	SC_ARENA_START,
	#endregion

	#region -AccountGender
	CS_USERGENDER_SET,
	SC_USERGENDER_SET_RESULT,
	SC_USERGENDER_NOTIFY,
	#endregion
	
	#region -HackCheck
	CS_HACK_INFO,
	#endregion
}

//$yde
public enum ePRIVATESHOPSEARCHTYPE
{
	ePRIVATESHOPSEARCHTYPE_NOTHING = 0,
	
	ePRIVATESHOPSEARCHTYPE_LEVEL_ASCENDING,
	ePRIVATESHOPSEARCHTYPE_LEVEL_DESCENDING,
	
	ePRIVATESHOPSEARCHTYPE_PRICE_ASCENDING,
	ePRIVATESHOPSEARCHTYPE_PRICE_DESCENDING,
	
	ePRIVATESHOPSEARCHTYPE_GRADE_ASCENDING,
	ePRIVATESHOPSEARCHTYPE_GRADE_DESCENDING,
	
	ePRIVATESHOPSEARCHTYPE_MAX
};

#region -Guild_Struct
public class body_SC_GUILD_LOAD_RESULT : AsPacketHeader
{
	public string szGuildName;
	public string szGuildMaster;
	public eGUILDPERMISSION ePermission;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szGuildName
		byte[] name = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);

		// szGuildMaster
		byte[] master = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, master, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szGuildMaster = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( master));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// ePermission
		byte[] permission = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, permission, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "ePermission", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( permission, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_GUILD_INFO : AsPacketHeader
{
	public bool bGuildList;
	
	public body_CS_GUILD_INFO(bool bRequestGuildList)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_INFO;
		
		bGuildList = bRequestGuildList;
	}
}

public class body_SC_GUILD_INFO_RESULT : AsPacketHeader
{
	public int nGuildIdx;
	public string szGuildName;
	public ushort nLevel;
	public string szMasterName;
	public UInt16 nConnectCnt;
	public UInt16 nMaxMember;
	public byte nNoticeLen;
	public string szGuildNotice;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nGuildIdx
		byte[] idx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nGuildIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, idx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( idx, 0));
		index += sizeof( Int32);

		// szGuildName
		byte[] name = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);

		// nLevel
		byte[] level = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nLevel", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, level, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( level, 0));
		index += sizeof( UInt16);

		// szMasterName
		byte[] masterName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, masterName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szMasterName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( masterName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nConnectCnt
		byte[] count = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nConnectCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( count, 0));
		index += sizeof( UInt16);

		// nMaxMember
		byte[] maxMember = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nMaxMember", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, maxMember, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( maxMember, 0));
		index += sizeof( UInt16);

		// nNoticeLen
		headerinfo = infotype.GetField( "nNoticeLen", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// szGuildNotice
		byte[] notice = new byte[ nNoticeLen];
		Buffer.BlockCopy( data, index, notice, 0, nNoticeLen);
		szGuildNotice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notice));
		index += nNoticeLen;
	}
}

public class body1_SC_GUILD_SEARCH_RESULT : AsPacketHeader
{
	public byte nCnt;
	public Int32 nMaxPage;
	public body2_SC_GUILD_SEARCH_RESULT[] search = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		byte[] maxPage = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, maxPage, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( maxPage, 0));
		index += sizeof( Int32);

		search = new body2_SC_GUILD_SEARCH_RESULT[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			search[i] = new body2_SC_GUILD_SEARCH_RESULT();
			index = search[i].PacketBytesToClass( data, index);
		}
	}

	public void PacketBytesToClass2( byte[] data, int index)
	{
		Type infotype = this.GetType();
		// nCnt
		FieldInfo headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		byte[] maxPage = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, maxPage, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( maxPage, 0));
		index += sizeof( Int32);

		search = new body2_SC_GUILD_SEARCH_RESULT[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			search[i] = new body2_SC_GUILD_SEARCH_RESULT();
			index = search[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_SC_GUILD_SEARCH_RESULT
{
	public int nGuildIdx;
	public string szGuildName;
	public UInt16 nLevel;
	public string szMasterName;
	public byte nPublicizeLen;
	public string szGuildPublicize;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nGuildIdx
		byte[] idx = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( Int32));
		FieldInfo headerinfo = infotype.GetField( "nGuildIdx", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( idx, 0));
		index += sizeof( Int32);

		// szGuildName
		byte[] guildName = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, guildName, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildName));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);

		// nLevel
		byte[] level = new byte[ sizeof( UInt16)];
		Buffer.BlockCopy( data, index, level, 0, sizeof( UInt16));
		headerinfo = infotype.GetField( "nLevel", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt16( level, 0));
		index += sizeof( UInt16);

		// szMasterName
		byte[] masterName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, masterName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szMasterName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( masterName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nPublicizeLen
		headerinfo = infotype.GetField ( "nPublicizeLen", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// szGuildPublicize
		byte[] guildPublicize = new byte[ nPublicizeLen];
		Buffer.BlockCopy( data, index, guildPublicize, 0, nPublicizeLen);
		szGuildPublicize = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildPublicize));
		index += nPublicizeLen;

		return index;
	}
}

public class body_CS_GUILD_INFO_DETAIL : AsPacketHeader
{
	public body_CS_GUILD_INFO_DETAIL()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_INFO_DETAIL;
	}
}

public class body_SC_GUILD_INFO_DETAIL_RESULT : AsPacketHeader
{
	public ushort nCurMaxMember;
	public byte nCurMaxStorage;
	public ushort nNextMaxMember;
	public byte nNextMaxStorage;
	public uint nPrice;
}

public class body_CS_GUILD_MEMBER_INFO : AsPacketHeader
{
	public body_CS_GUILD_MEMBER_INFO()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_MEMBER_INFO;
	}
}

public class body1_SC_GUILD_MEMBER_INFO_RESULT : AsPacketHeader
{
	public byte nCnt;
	public byte nMaxPage;
	public body2_SC_GUILD_MEMBER_INFO_RESULT[] infos = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		infos = new body2_SC_GUILD_MEMBER_INFO_RESULT[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			infos[i] = new body2_SC_GUILD_MEMBER_INFO_RESULT();
			index = infos[i].PacketBytesToClass( data, index);
		}
	}

	public void PacketBytesToClass2( byte[] data, int index)
	{
		Type infotype = this.GetType();
		// nCnt
		FieldInfo headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		infos = new body2_SC_GUILD_MEMBER_INFO_RESULT[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			infos[i] = new body2_SC_GUILD_MEMBER_INFO_RESULT();
			index = infos[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_SC_GUILD_MEMBER_INFO_RESULT
{
	public ushort nSessionIdx;
	public uint nCharUniqKey;
	public string szCharName;
	public uint nLevel;
	public eCLASS eClass;
	public eGUILDPERMISSION ePermission;
	public bool bConnect;
	public Int64 nLastConnectTime;

	public static int size
	{
		get	{ return ( sizeof( ushort) + ( sizeof( uint) * 2) + ( sizeof( byte) * ( AsGameDefine.MAX_NAME_LEN + 1)) + ( sizeof( int) * 2) + sizeof( bool)); }
	}

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nSessionIdx
		byte[] sessionIdx = new byte[ sizeof( UInt16)];
		Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
		FieldInfo headerinfo = infotype.GetField( "nSessionIdx", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToUInt16( sessionIdx, 0));
		index += sizeof( UInt16);

		// nCharUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nLevel
		byte[] level = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, level, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nLevel", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( level, 0));
		index += sizeof( UInt32);

		// eClass
		byte[] cls = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, cls, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eClass", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( cls, 0));
		index += sizeof( Int32);

		// ePermission
		byte[] permission = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, permission, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "ePermission", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( permission, 0));
		index += sizeof( Int32);

		// bConnect
		byte[] connect = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, connect, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bConnect", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( connect, 0));
		index += sizeof( bool);
		
		// nLastConnectTime
		byte[] tempTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nLastConnectTime", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( tempTime, 0));
		index += sizeof( Int64);
		
		return index;
	}
}

public class body_CS_GUILD_CREATE : AsPacketHeader
{
	public byte[] szGuildName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];

	public body_CS_GUILD_CREATE( string guildName)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_CREATE;

		byte[] tempName = UTF8Encoding.UTF8.GetBytes( guildName);
		Buffer.BlockCopy( tempName, 0, szGuildName, 0, tempName.Length);
	}
}

public class body_SC_GUILD_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public int nGuildIdx;
	public string szGuildName;

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

		// nGuildIdx
		byte[] idx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nGuildIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, idx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( idx, 0));
		index += sizeof( Int32);

		// szGuildName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_CS_GUILD_NOTICE : AsPacketHeader
{
	public byte[] szNotice = new byte[ AsGameDefine.eGUILDNOTICE + 1];

	public body_CS_GUILD_NOTICE( string msg)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_NOTICE;

		byte[] notice = UTF8Encoding.UTF8.GetBytes( msg);
		Buffer.BlockCopy( notice, 0, szNotice, 0, notice.Length);
	}
}

public class body_SC_GUILD_NOTICE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szNotice;

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

		// szNotice
		byte[] notice = new byte[ AsGameDefine.eGUILDNOTICE + 1];
		Buffer.BlockCopy( data, index, notice, 0, AsGameDefine.eGUILDNOTICE + 1);
		szNotice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notice));
		index += ( AsGameDefine.eGUILDNOTICE + 1);
	}
}

public class body_CS_GUILD_INVITE : AsPacketHeader
{
	public byte[] szCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];

	public body_CS_GUILD_INVITE( string name)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_INVITE;

		byte[] tempName = UTF8Encoding.UTF8.GetBytes( name);
		Buffer.BlockCopy( tempName, 0, szCharName, 0, tempName.Length);
	}
}

public class body_SC_GUILD_INVITE : AsPacketHeader
{
	public int nGuildIdx;
	public string szGuildName;
	public uint nCharUniqKey;
	public string szCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nGuildIdx
		byte[] guildIdx = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, guildIdx, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nGuildIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( guildIdx, 0));
		index += sizeof( Int32);

		// szGuildName
		byte[] guildName = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, guildName, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildName));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);

		// nCharUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_GUILD_INVITE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szCharName;

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

		// szCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_CS_GUILD_JOIN : AsPacketHeader
{
	public eGUILDJOINTYPE eJoinType;
	public int nGuildIdx;
	public uint nCharUniqKey;

	public body_CS_GUILD_JOIN( eGUILDJOINTYPE type, int index, uint uniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_JOIN;

		eJoinType = type;
		nGuildIdx = index;
		nCharUniqKey = uniqKey;
	}
}

public class body_SC_GUILD_JOIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szCharName;

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

		// szCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_CS_GUILD_SEARCH_JOIN : AsPacketHeader
{
	public int nGuildIdx;

	public body_CS_GUILD_SEARCH_JOIN( int id)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_SEARCH_JOIN;

		nGuildIdx = id;
	}
}

public class body_SC_GUILD_SEARCH_JOIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szGuildName;

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

		// szGuildName
		byte[] name = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);
	}
}

public class body_CS_GUILD_EXIT : AsPacketHeader
{
	public body_CS_GUILD_EXIT()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_EXIT;
	}
}

public class body_SC_GUILD_EXIT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CS_GUILD_EXILE : AsPacketHeader
{
	public UInt32 nCharUniqKey;

	public body_CS_GUILD_EXILE( UInt32 key)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_EXILE;

		nCharUniqKey = key;
	}
}

public class body_SC_GUILD_EXILE : AsPacketHeader
{
	// nothing..
}

public class body_SC_GUILD_EXILE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CS_GUILD_CHANGE_PERMISSION : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public eGUILDPERMISSION ePermission;

	public body_CS_GUILD_CHANGE_PERMISSION( UInt32 uniqKey, eGUILDPERMISSION permission)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_CHANGE_PERMISSION;

		nCharUniqKey = uniqKey;
		ePermission = permission;
	}
}

public class body_SC_GUILD_CHANGE_PERMISSION_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szAdminCharName;
	public string szTargetCharName;
	public eGUILDPERMISSION ePermission;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// szAdminCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szAdminCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// szTargetCharName
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szTargetCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// ePermission
		byte[] permission = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, permission, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "ePermission", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( permission, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_GUILD_SUPERVISE : AsPacketHeader
{
	public body_CS_GUILD_SUPERVISE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_SUPERVISE;
	}
}

public class body_SC_GUILD_SUPERVISE_RESULT : AsPacketHeader
{
	public string szPublicize;

	public new void PacketBytesToClass( byte[] data)
	{
		int index = ParsePacketHeader( data);

		// szPublicize
		byte[] publicize = new byte[ AsGameDefine.eGUILDPUBLICIZE + 1];
		Buffer.BlockCopy( data, index, publicize, 0, AsGameDefine.eGUILDPUBLICIZE + 1);
		szPublicize = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( publicize));
		index += ( AsGameDefine.eGUILDPUBLICIZE + 1);
	}
}

public class body_CS_GUILD_PUBLICIZE : AsPacketHeader
{
	public byte[] szPublicize = new byte[ AsGameDefine.eGUILDPUBLICIZE + 1];

	public body_CS_GUILD_PUBLICIZE( string msg)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_PUBLICIZE;

		byte[] tempPublicize = UTF8Encoding.UTF8.GetBytes( msg);
		Buffer.BlockCopy( tempPublicize, 0, szPublicize, 0, tempPublicize.Length);
	}
}

public class body_SC_GUILD_PUBLICIZE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szPublicize;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// szPublicize
		byte[] publicize = new byte[ AsGameDefine.eGUILDPUBLICIZE + 1];
		Buffer.BlockCopy( data, index, publicize, 0, AsGameDefine.eGUILDPUBLICIZE + 1);
		szPublicize = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( publicize));
		index += ( AsGameDefine.eGUILDPUBLICIZE + 1);
	}
}

public class body_CS_GUILD_SEARCH_JOIN_APPROVE : AsPacketHeader
{
	public uint nCharUniqKey;
	public eGUILDJOINTYPE eJoinType;

	public body_CS_GUILD_SEARCH_JOIN_APPROVE( uint uniqKey, eGUILDJOINTYPE type)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_SEARCH_JOIN_APPROVE;

		nCharUniqKey = uniqKey;
		eJoinType = type;
	}
}

public class body_SC_GUILD_SEARCH_JOIN_APPROVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CS_GUILD_LEVEL_UP : AsPacketHeader
{
	public body_CS_GUILD_LEVEL_UP()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_LEVEL_UP;
	}
}

public class body_SC_GUILD_LEVEL_UP_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public ushort nMaxMember;
	public byte nLevel;
}

public class body_SC_GUILD_MEMBER_DELETE_RESULT : AsPacketHeader
{
	public eGUILDMEMBER_DELETE eDeleteFlag;
	public string szCharName;
	public string szExileName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eDeleteFlag
		byte[] flag = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, flag, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eDeleteFlag", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( flag, 0));
		index += sizeof( Int32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// szExileName
		byte[] exileName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, exileName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szExileName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( exileName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_CS_GUILD_MASTER_CHANGE : AsPacketHeader
{
	public uint nCharUniqKey;

	public body_CS_GUILD_MASTER_CHANGE( uint uniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_MASTER_CHANGE;

		nCharUniqKey = uniqKey;
	}
}

public class body_SC_GUILD_MASTER_CHANGE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szOldMasterName;
	public string szNewMasterName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// szOldMasterName
		byte[] oldName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, oldName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szOldMasterName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( oldName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// szNewMasterName
		byte[] newName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, newName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szNewMasterName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( newName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}
/* dopamin ==>CS_OTHER_INFO;
public class body_CS_GUILD_MEMBER_INFO_DETAIL : AsPacketHeader
{
	public uint nCharUniqKey;

	public body_CS_GUILD_MEMBER_INFO_DETAIL( uint uniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_MEMBER_INFO_DETAIL;

		nCharUniqKey = uniqKey;
	}
}
 */
public class body1_SC_GUILD_MEMBER_INFO_DETAIL_RESULT : AsPacketHeader
{
	public string szCharName;
	public byte nCnt;
	public body2_SC_GUILD_MEMBER_INFO_DETAIL_RESULT[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nCnt
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 >= nCnt)
			return;

		// body
		body = new body2_SC_GUILD_MEMBER_INFO_DETAIL_RESULT[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_GUILD_MEMBER_INFO_DETAIL_RESULT();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_SC_GUILD_MEMBER_INFO_DETAIL_RESULT
{
	public ushort nSlot;
	public sITEM sItem = new sITEM();

	public static int size
	{
		get	{ return ( sizeof( ushort) + sITEM.size); }
	}

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nSlot
		byte[] slot = new byte[ sizeof( UInt16)];
		Buffer.BlockCopy( data, index, slot, 0, sizeof( UInt16));
		FieldInfo headerinfo = infotype.GetField( "nSlot", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt16( slot, 0));
		index += sizeof( UInt16);

		// sItem
		byte[] item = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, item, 0, sITEM.size);
		sItem.ByteArrayToClass( item);
		index += sITEM.size;

		return index;
	}
}

public class body_CS_GUILD_UI_SCROLL : AsPacketHeader
{
	public eGUILD_UI_SCROLL eType;
	public Int32 nPage;
	public bool bConnected;

	public body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL type, Int32 page, bool connected)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_UI_SCROLL;

		eType = type;
		nPage = page;
		bConnected = connected;
	}
}

public class body_SC_GUILD_UI_SCROLL_RESULT : AsPacketHeader
{
	public eGUILD_UI_SCROLL eType;
	public body1_SC_GUILD_SEARCH_RESULT searchResult = null;
	public body1_SC_GUILD_MEMBER_INFO_RESULT memberInfoResult = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eType
		byte[] type = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, type, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eType", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( type, 0));
		index += sizeof( Int32);

		switch( eType)
		{
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD:
			searchResult = new body1_SC_GUILD_SEARCH_RESULT();
			searchResult.PacketBytesToClass2( data, index);
			break;
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_APPROVE_MEMBER:
		case eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_NOT_APPROVE_MEMBER:
			memberInfoResult = new body1_SC_GUILD_MEMBER_INFO_RESULT();
			memberInfoResult.PacketBytesToClass2( data, index);
			break;
		}
	}
}
#endregion

#region - Private Shop -
 // cs
public class body_CS_PRIVATESHOP_CREATE : AsPacketHeader
{
	public byte[] szContent = new byte[AsGameDefine.MAX_PRIVATESHOP_CONTENT_LENGTH];

	public body_CS_PRIVATESHOP_CREATE( byte[] _content)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_CREATE;

		for( int i=0; i<_content.Length; ++i)
		{
			szContent[i] = _content[i];
		}
	}
}

public class body_CS_PRIVATESHOP_DESTROY : AsPacketHeader
{
	public body_CS_PRIVATESHOP_DESTROY()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_DESTROY;
	}
}

public class body_CS_PRIVATESHOP_MODIFY : AsPacketHeader
{
	public byte[] szContent = new byte[AsGameDefine.MAX_PRIVATESHOP_CONTENT_LENGTH];

	public body_CS_PRIVATESHOP_MODIFY( byte[] _content)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_MODIFY;

		for( int i=0; i<_content.Length; ++i)
		{
			szContent[i] = _content[i];
		}
	}
}

public class body_CS_PRIVATESHOP_REGISTRATION_ITEM : AsPacketHeader
{
	public bool bAddOrDel;
	public Int32 nInvenSlot;
	public byte nPrivateShopSlot;
	public Int16 nItemCount;
	public UInt64 nItemSellGold;

	public body_CS_PRIVATESHOP_REGISTRATION_ITEM( bool _addOrDel, Int32 _invenSlot, byte _privateShopSlot, Int16 _itemCount, UInt64 _itemSellGold)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_REGISTRATION_ITEM;

		bAddOrDel = _addOrDel;
		nInvenSlot = _invenSlot;
		nPrivateShopSlot = _privateShopSlot;
		nItemCount = _itemCount;
		nItemSellGold = _itemSellGold;
	}
}

public class body_CS_PRIVATESHOP_OPEN : AsPacketHeader
{
	public body_CS_PRIVATESHOP_OPEN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_OPEN;
	}
}

public class body_CS_PRIVATESHOP_CLOSE : AsPacketHeader
{
	public body_CS_PRIVATESHOP_CLOSE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_CLOSE;
	}
}

public class body_CS_PRIVATESHOP_ENTER : AsPacketHeader
{
	public UInt32 nPrivateShopUID; //UID.

	public body_CS_PRIVATESHOP_ENTER( UInt32 _shopUID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_ENTER;

		nPrivateShopUID = _shopUID;
	}
}

public class body_CS_PRIVATESHOP_LEAVE : AsPacketHeader
{
	public UInt32 nPrivateShopUID; //UID.

	public body_CS_PRIVATESHOP_LEAVE( UInt32 _shopUID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_LEAVE;

		nPrivateShopUID = _shopUID;
	}
}

public class body_CS_PRIVATESHOP_ITEMBUY : AsPacketHeader
{
	public UInt32 nPrivateShopUID;	// UID.
	public byte nPrivateShopSlot;	//
	public Int16 nItemCount;			//

	public body_CS_PRIVATESHOP_ITEMBUY( UInt32 _shopUID, byte _slot, Int16 _count)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_ITEMBUY;

		nPrivateShopUID = _shopUID;
		nPrivateShopSlot = _slot;
		nItemCount = _count;
	}
}

 // sc
public class body_SC_PRIVATESHOP_CREATE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nPrivateShopSlotMaxCount;
};

public class body_SC_PRIVATESHOP_DESTROY : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_SC_PRIVATESHOP_MODIFY : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_SC_PRIVATESHOP_REGISTRATION_ITEM : AsPacketHeader
{
	public eRESULTCODE eResult;

	public bool bAddOrDel;			//true:add, false:del
	public Int32 nInvenSlot;			//
	public byte nPrivateShopSlot;	//
	public sITEM sPrivateShopItem;	//
	public UInt64 nItemSellGold;
};

public class body_SC_PRIVATESHOP_OPEN : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nPrivateShopUID;
	public UInt32 nCharUniqKey;
	public byte[] strName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public byte[] strContent = new byte[AsGameDefine.MAX_PRIVATESHOP_CONTENT_LENGTH];//256
	public Int64 nPrivateShopRemainingTime;
	public Int32 nOpenItemInvenSlot;
	public Int32 nOpenItemTableIdx;
	public Vector3 sCurPosition;
	
//	
//	public Int32 nChannel;
//	public byte[] szChannelName = new byte[ AsGameDefine.eCHANNELNAME + 1];
//	public Int32 nMinLevel;
//	public Int32 nMaxLevel;
//	public eCONFUSIONTYPE eConfusion;
//	public bool bIsPrivateShop; // true: ChargeReduction
//	public Int32 nPrivateStore_Charge;
//	public Int32 nPrivateStore_ChargeReduction;
}

public class body_SC_PRIVATESHOP_CLOSE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nPrivateShopUID;
	public UInt32 nCharUniqKey;
};

public class body1_SC_PRIVATESHOP_LIST : AsPacketHeader
{
	public Int32 nShopCnt;
	public body2_SC_PRIVATESHOP_LIST[] body = new body2_SC_PRIVATESHOP_LIST[0];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nShopCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nShopCnt)
			return;

		body = new body2_SC_PRIVATESHOP_LIST[ nShopCnt];
		for( int i = 0; i < nShopCnt; i++)
		{
			body[i] = new body2_SC_PRIVATESHOP_LIST();
			byte[] tmpData = new byte[ body2_SC_PRIVATESHOP_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_PRIVATESHOP_LIST.size;
		}
	}
};

public class body2_SC_PRIVATESHOP_LIST : AsPacketHeader
{
	public static readonly int size = 4 + 4 + AsGameDefine.MAX_NAME_LEN + 1 + AsGameDefine.MAX_PRIVATESHOP_CONTENT_LENGTH + 12 + 4;

	public UInt32 nPrivateShopUID;
	public UInt32 nCharUniqKey;
	public byte[] strName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public byte[] strContent = new byte[AsGameDefine.MAX_PRIVATESHOP_CONTENT_LENGTH];//61
	public Vector3 sCurPosition;
	public Int32 nItemTableIdx;
};

public class body_SC_PRIVATESHOP_ENTER : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_SC_PRIVATESHOP_LEAVE : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body1_SC_PRIVATESHOP_ITEMLIST : AsPacketHeader
{
	public Int32 nItemCnt;
	public body2_SC_PRIVATESHOP_ITEMLIST[] body = new body2_SC_PRIVATESHOP_ITEMLIST[0];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nItemCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nItemCnt)
			return;

		body = new body2_SC_PRIVATESHOP_ITEMLIST[ nItemCnt];
		for( int i = 0; i < nItemCnt; i++)
		{
			body[i] = new body2_SC_PRIVATESHOP_ITEMLIST();
			byte[] tmpData = new byte[ body2_SC_PRIVATESHOP_ITEMLIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_PRIVATESHOP_ITEMLIST.size;
		}
	}
};

public class body2_SC_PRIVATESHOP_ITEMLIST : AsPacketHeader
{
	public static readonly int size = 13 + sITEM.size;

	public byte nPrivateShopSlot;
	public Int32 nMaxOverlapped;
	public UInt64 nItemGold;
	public sITEM sItem;
};

public class body1_SC_PRIVATESHOP_OWNER_ITEMLIST : AsPacketHeader
{
	public Int32 nItemCnt;
	public body2_SC_PRIVATESHOP_OWNER_ITEMLIST[] body = new body2_SC_PRIVATESHOP_OWNER_ITEMLIST[0];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nItemCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nItemCnt)
			return;

		body = new body2_SC_PRIVATESHOP_OWNER_ITEMLIST[ nItemCnt];
		for( int i = 0; i < nItemCnt; i++)
		{
			body[i] = new body2_SC_PRIVATESHOP_OWNER_ITEMLIST();
			byte[] tmpData = new byte[ body2_SC_PRIVATESHOP_OWNER_ITEMLIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_PRIVATESHOP_OWNER_ITEMLIST.size;
		}
	}
};

public class body2_SC_PRIVATESHOP_OWNER_ITEMLIST : AsPacketHeader
{
	public static readonly int size = 17 + sITEM.size;

	public Int32 nInvenSlot;
	public byte nPrivateShopSlot;
	public Int32 nMaxOverlapped;
	public UInt64 nItemGold;
	public sITEM sItem;
};

public class body_SC_PRIVATESHOP_ITEMBUY : AsPacketHeader
{
	public eRESULTCODE eResult;

	public Int32 nPrivateShopUID;
	public byte nPrivateShopSlot;
	public Int16 nItemCount;
};

public class body_SC_PRIVATESHOP_OWNER_ITEMBUY : AsPacketHeader
{
	public eRESULTCODE eResult;

	public Int32 nPrivateShopUID;
	public Int32 nInvenSlot;
	public byte nPrivateShopSlot;
	public Int16 nItemCount;
	
	public Int32 nPrivateStore_Charge;
};

public class body_CS_PRIVATESHOP_SEARCH : AsPacketHeader
{
	public Int32 nPageIdx;

	public Int32 nItemTableIdx;
	public eCLASS eClass;
	public Int32 nCategory1;       //eITEMTYPE
	public Int32 nCategory2;       //eEQUIPITEM_TYPE, eETCITEM_TYPE, eUSEITEM_TYPE, eACTIONITEM_TYPE
	public Int32 nLevelMin;
	public Int32 nLevelMax;
	public eEQUIPITEM_GRADE eGrade;
	public Int32 nMapIdx;
	
	public ePRIVATESHOPSEARCHTYPE eSearchType;
	
	public body_CS_PRIVATESHOP_SEARCH( Int32 _pageIdx, Int32 _itemTableIdx, eCLASS _class,
		Int32 _category1, Int32 _category2, Int32 _levelMin, Int32 _levelMax,
		eEQUIPITEM_GRADE _grade, Int32 _mapIdx, ePRIVATESHOPSEARCHTYPE _searchType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_PRIVATESHOP_SEARCH;
		
		nPageIdx = _pageIdx;
		
		nItemTableIdx = _itemTableIdx;
		eClass = _class;
		nCategory1 = _category1;
		nCategory2 = _category2;
		nLevelMin = _levelMin;
		nLevelMax = _levelMax;
		eGrade = _grade;
		nMapIdx = _mapIdx;
		
		eSearchType = _searchType;
	}
};

public class body1_SC_PRIVATESHOP_SEARCH_RESULT : AsPacketHeader
{
	public Int32 nItemCnt;
	public Int32 nMaxPageCount;
	
	public body2_SC_PRIVATESHOP_SEARCH_RESULT[] body = new body2_SC_PRIVATESHOP_SEARCH_RESULT[0];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nItemCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nItemCnt)
			return;
		
		count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMaxPageCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		body = new body2_SC_PRIVATESHOP_SEARCH_RESULT[ nItemCnt];
		for( int i = 0; i < nItemCnt; i++)
		{
			body[i] = new body2_SC_PRIVATESHOP_SEARCH_RESULT();
			byte[] tmpData = new byte[ body2_SC_PRIVATESHOP_SEARCH_RESULT.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_PRIVATESHOP_SEARCH_RESULT.size;
		}
	}
};

public class body2_SC_PRIVATESHOP_SEARCH_RESULT : AsPacketHeader
{
	public static readonly int size = 17 + sITEM.size;
	
	public UInt32 nPrivateShopUID;
	public byte nPrivateShopSlot;           //개인 상점 슬롯 번호.
	public Int32 nMaxOverlapped;           //아이템 중첩 최대 개수.
	public Int64 nItemGold;                   //가격.
	public sITEM sItem;                         //sItem.nOverlapped은 팔 수 있는 아이템 개수이다. 0이면 다 팔린 상태임.
};
#endregion

#region -Designation_Struct
public class body_CS_SUBTITLE_LIST : AsPacketHeader
{
	public body_CS_SUBTITLE_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SUBTITLE_LIST;
	}
}

public class body_CS_SUBTITLE_SET : AsPacketHeader
{
	public Int32 nSubTitleTableIdx;
	public bool bSubTitleHide;

	public body_CS_SUBTITLE_SET( Int32 id, bool bHide)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SUBTITLE_SET;

		nSubTitleTableIdx = id;
		bSubTitleHide = bHide;
	}
}

public class body1_SC_SUBTITLE_LIST : AsPacketHeader
{
	public Int32 nAccrueRewardPoint;
	public Int32 nCount;
	public body2_SC_SUBTITLE_LIST[] designations = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nAccrueRewardPoint
		byte[] accrueRewardPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nAccrueRewardPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, accrueRewardPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( accrueRewardPoint, 0));
		index += sizeof( Int32);

		// nCount
		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nCount)
			return;

		designations = new body2_SC_SUBTITLE_LIST[ nCount];
		for( int i = 0; i < nCount; i++)
		{
			designations[i] = new body2_SC_SUBTITLE_LIST();
			byte[] tmpData = new byte[ body2_SC_SUBTITLE_LIST.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			designations[i].ByteArrayToClass( tmpData);
			index += body2_SC_SUBTITLE_LIST.size;
		}
	}
}

public class body2_SC_SUBTITLE_LIST : AsPacketHeader
{
	public Int32 	nSubTitleTableIdx;
	public bool 		bReward;

	public static Int32 size
	{
		get	{ return sizeof( Int32 ) + sizeof(bool); }
	}
}

public class body_SC_SUBTITLE_SET_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nSubTitleTableIdx;
	public bool bSubTitleHide;
}

public class body_SC_SUBTITLE_ADD : AsPacketHeader
{
	public Int32 nSubTitleTableIdx;
}

public class body_SC_SUBTITLE_ADD_NOTIFY : AsPacketHeader
{
	public Int32 nSubTitleTableIdx;
	public string szCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nSubTitleTableIdx
		byte[] tableIdx = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, tableIdx, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nSubTitleTableIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( tableIdx, 0));
		index += sizeof( Int32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_CS_SUBTITLE_INDEX_REWARD : AsPacketHeader
{
	public Int32 nSubTitleTableIdx;

	public body_CS_SUBTITLE_INDEX_REWARD( Int32 _subTitleTableIdx )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SUBTITLE_INDEX_REWARD;

		nSubTitleTableIdx = _subTitleTableIdx;
	}
}

public class body_CS_SUBTITLE_ACCRUE_REWARD : AsPacketHeader
{
	public Int32 nTableIdx;

	public body_CS_SUBTITLE_ACCRUE_REWARD( Int32 _tableIdx )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SUBTITLE_ACCURE_REWARD;
		
		nTableIdx = _tableIdx;
	}

}

public class body_SC_SUBTITLE_INDEX_REWARD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nSubTitleTableIdx;
}

public class body_SC_SUBTITLE_ACCRUE_REWARD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nAccrueRewardPoint;
}

#region -DelegateImage
public class body_CS_IMAGE_LIST : AsPacketHeader
{
	public body_CS_IMAGE_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_IMAGE_LIST;
	}
}

public class body_CS_IMAGE_BUY : AsPacketHeader
{
	public Int32 nImageTableIdx;

	public body_CS_IMAGE_BUY( Int32 idx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_IMAGE_BUY;

		nImageTableIdx = idx;
	}
}

public class body_CS_IMAGE_SET : AsPacketHeader
{
	public Int32 nImageTableIdx;

	public body_CS_IMAGE_SET( Int32 idx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_IMAGE_SET;

		nImageTableIdx = idx;
	}
}

public class body1_SC_IMAGE_LIST : AsPacketHeader
{
	public Int32 nCurImageTableIdx;
	public Int32 nCount;
	public body2_SC_IMAGE_LIST[] imageList = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCurImageTableIdx
		byte[] tempInt32 = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, tempInt32, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nCurImageTableIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( tempInt32, 0));
		index += sizeof( Int32);

		// nCount
		Buffer.BlockCopy( data, index, tempInt32, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( tempInt32, 0));
		index += sizeof( Int32);

		// imageList
		imageList = new body2_SC_IMAGE_LIST[ nCount];
		for( int i = 0; i < nCount; i++)
		{
			imageList[i] = new body2_SC_IMAGE_LIST();
			index = imageList[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_SC_IMAGE_LIST : AsPacketHeader
{
	public Int32 nImageTableIdx;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nImageTableIdx
		byte[] idx = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( Int32));
		FieldInfo headerinfo = infotype.GetField( "nImageTableIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( idx, 0));
		index += sizeof( Int32);

		return index;
	}
}

public class body_SC_IMAGE_BUY_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nImageTableIdx;
}

public class body_SC_IMAGE_SET_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nImageTableIdx;
}

#if NEW_DELEGATE_IMAGE
public class body_SC_IMAGE_CHANGE_NOTIFY : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nImageTableIdx;
}
#endif
#endregion

public class body_SC_GUILD_NAME_NOTIFY : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public string szGuildName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		// szGuildName
		byte[] guildName = new byte[ AsGameDefine.MAX_GUILD_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, guildName, 0, AsGameDefine.MAX_GUILD_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildName));
		index += ( AsGameDefine.MAX_GUILD_NAME_LEN + 1);
	}
}

public class body_SC_GUILD_NOTICE_NOTIFY : AsPacketHeader
{
	public string szGuildNotice;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szGuildNotice
		byte[] guildNotice = new byte[ AsGameDefine.eGUILDNOTICE + 1];
		Buffer.BlockCopy( data, index, guildNotice, 0, AsGameDefine.eGUILDNOTICE + 1);
		szGuildNotice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildNotice));
		index += ( AsGameDefine.eGUILDNOTICE + 1);
	}
}

class body_CS_GUILD_NAME_CHANGE :AsPacketHeader
{
	public int nSlot = -1;
	public byte[] szGuildName = null;
	
	public body_CS_GUILD_NAME_CHANGE( string _szGuildName , int _slot )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GUILD_NAME_CHANGE;
		
		byte[] byteArray = System.Text.UTF8Encoding.UTF8.GetBytes( _szGuildName);
		
		if( byteArray.Length > AsGameDefine.MAX_NAME_LEN)
		{
			return;
		}
		
		szGuildName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
		
		Buffer.BlockCopy( byteArray, 0, szGuildName, 0, byteArray.Length);
		
		// end of string
		szGuildName[byteArray.Length] = 0;
		
		nSlot = _slot;
	}
};

public class body_SC_GUILD_NAME_CHANGE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public string szGuildName;

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
		
		// szGuildName
		byte[] guildName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, guildName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( guildName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_GUILD_NAME_NOTIFY_CHANGE : AsPacketHeader
{
	public string szBeforeGuildName;
	public string szAfterGuildName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szBeforeGuildName
		byte[] beforeGuildName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, beforeGuildName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szBeforeGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( beforeGuildName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
		
		// szAfterGuildName
		byte[] afterGuildName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, afterGuildName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szAfterGuildName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( afterGuildName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_SUBTITLE_CHANGE_NOTIFY : AsPacketHeader
{
	public ushort nSessionIdx;
	public UInt32 nCharUniqKey;
	public Int32 nSubTitleTableIdx;
	public bool bSubTitleHide;
}
#endregion




#region -Production_Struct

// send
public class body_CS_ITEM_PRODUCT_INFO : AsPacketHeader
{
	public body_CS_ITEM_PRODUCT_INFO()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_INFO;
	}
}


public class body_CS_ITEM_PRODUCT_TECHNIQUE_REGISTER : AsPacketHeader
{
	public Int32 eProductType;

	public body_CS_ITEM_PRODUCT_TECHNIQUE_REGISTER( int _iType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_TECHNIQUE_REGISTER;

		eProductType = _iType;
	}
}


public class body_CS_ITEM_PRODUCT_PROGRESS : AsPacketHeader
{
	public bool bProgress;

	public body_CS_ITEM_PRODUCT_PROGRESS( bool _bProgress)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_PROGRESS;

		bProgress = _bProgress;
	}
}


public class body_CS_ITEM_PRODUCT_REGISTER : AsPacketHeader
{
	public Byte nProductSlot;
	public Int32 nRecipeIndex;

	public body_CS_ITEM_PRODUCT_REGISTER( byte _nProductSlot, Int32 _nRecipeIndex)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_REGISTER;

		nProductSlot = _nProductSlot;
		nRecipeIndex = _nRecipeIndex;
	}
}

public class body_CS_ITEM_PRODUCT_CANCEL : AsPacketHeader
{
	public Byte nProductSlot;

	public body_CS_ITEM_PRODUCT_CANCEL( byte _nProductSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_CANCEL;

		nProductSlot = _nProductSlot;
	}
}

public class body_CS_ITEM_PRODUCT_RECEIVE : AsPacketHeader
{
	public Byte nProductSlot;

	public body_CS_ITEM_PRODUCT_RECEIVE( byte _nProductSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_RECEIVE;

		nProductSlot = _nProductSlot;
	}
}


public class body_CS_ITEM_PRODUCT_CASH_DIRECT : AsPacketHeader
{
	public Byte nProductSlot;

	public body_CS_ITEM_PRODUCT_CASH_DIRECT( byte _nProductSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_CASH_DIRECT;

		nProductSlot = _nProductSlot;
	}
}

public class body_CS_ITEM_PRODUCT_CASH_SLOT_OPEN : AsPacketHeader
{
	public body_CS_ITEM_PRODUCT_CASH_SLOT_OPEN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_CASH_SLOT_OPEN;
	}
}

public class body_CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN : AsPacketHeader
{
	public Int32 eProductType;

	public body_CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN( Int32 _eProductType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN;

		eProductType = _eProductType;
	}
}

public class body_CS_ITEM_PRODUCT_CASH_LEVEL_UP : AsPacketHeader
{
	public Int32 eProductType;

	public body_CS_ITEM_PRODUCT_CASH_LEVEL_UP( Int32 _eProductType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_PRODUCT_CASH_LEVEL_UP;

		eProductType = _eProductType;
	}
}


// Receive
public class sPRODUCT_SLOT : AsPacketHeader
{
	public Int32 nRecipeIndex;
	public Int64 nProductTime;

	public static Int32 size
	{
		get	{ return 4 + 8; }
	}
}

public class sPRODUCT_INFO : AsPacketHeader
{
	public Int32 nLevel;
	public Int32 nTotExp;

	public static Int32 size
	{
		get	{ return 4 + 4; }
	}
};


public class body2_SC_ITEM_PRODUCT_INFO : AsPacketHeader
{
	public Byte nProductSlot;
	public sPRODUCT_SLOT sSlotInfo;

	public static Int32 size
	{
		get	{ return 1 + sPRODUCT_SLOT.size; }
	}
}

public class body1_SC_ITEM_PRODUCT_INFO : AsPacketHeader
{
	public bool bProgress;
	public sPRODUCT_INFO[] sProductInfo;
	public Byte nBody2Cnt;
	public body2_SC_ITEM_PRODUCT_INFO[] body;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// bConnect
		byte[] connect = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, connect, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bProgress", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( connect, 0));
		index += sizeof( bool);

		int iProductInfoCount = (int)eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX;
		sProductInfo = new sPRODUCT_INFO[ iProductInfoCount ];
		for( int i = 0; i < iProductInfoCount; i++)
		{
			sProductInfo[i] = new sPRODUCT_INFO();
			byte[] tmpData = new byte[sPRODUCT_INFO.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			sProductInfo[i].ByteArrayToClass( tmpData);
			index += sPRODUCT_INFO.size;
		}

		headerinfo = infotype.GetField( "nBody2Cnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[index++]);

		if( 0 >= nBody2Cnt)
			return;

		body = new body2_SC_ITEM_PRODUCT_INFO[ nBody2Cnt ];
		for( int i = 0; i < nBody2Cnt; i++)
		{
			body[i] = new body2_SC_ITEM_PRODUCT_INFO();
			byte[] tmpData = new byte[ body2_SC_ITEM_PRODUCT_INFO.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_ITEM_PRODUCT_INFO.size;
		}
	}
}


public class body_SC_ITEM_PRODUCT_TECHNIQUE_REGISTER : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_PROGRESS : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_STATE : AsPacketHeader
{
	public UInt16 nSession;
	public UInt32 nCharUniqKey;
	public bool bProgress;
}

public class body_SC_ITEM_PRODUCT_REGISTER : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_CANCEL : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_RECEIVE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nProductTableIdx;
}


public class body_SC_ITEM_PRODUCT_SLOT_INFO : AsPacketHeader
{
	public Byte nProductSlot;
	public sPRODUCT_SLOT sSlotInfo;
	public Int32 nContents;
}

public class body_SC_ITEM_PRODUCT_TECHNIQUE_INFO : AsPacketHeader
{
	public Int32 eProductType;
	public sPRODUCT_INFO sProductInfo;
	public Int32 nContents;
}

public class body_SC_ITEM_PRODUCT_CASH_DIRECT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_CASH_SLOT_OPEN : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Byte nSlot;
}

public class body_SC_ITEM_PRODUCT_CASH_TECHNIQUE_OPEN : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_ITEM_PRODUCT_CASH_LEVEL_UP : AsPacketHeader
{
	public eRESULTCODE eResult;
}
#endregion

#region - bonus -
public class body_SC_BONUS_ATTENDANCE : AsPacketHeader
{
	public byte nDays;
}

public class body_SC_BONUS_RETURN : AsPacketHeader
{
	public UInt16 nDays;
}
#endregion

#region -Socail
public class body_CS_SOCIAL_INFO : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_SOCIAL_INFO( uint userUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SOCIAL_INFO;

		nUserUniqKey = userUniqKey;
	}
}

public class body_SC_SOCIAL_INFO : AsPacketHeader
{
	public uint nUserUniqKey; //2013.02.26

	public int nSocialPoint;
	public int nMaxSocialPoint;
	public byte[] szNotice = new byte[AsGameDefine.MAX_SOCIAL_NOTICE + 1];

	public Int16 nHelloCount;				//new
	public Int16 nMaxHelloCount;				//new
}

public class body_CS_SOCIAL_NOTICE : AsPacketHeader
{
	public byte[] szNotice = new byte[AsGameDefine.MAX_SOCIAL_NOTICE + 1];

	public body_CS_SOCIAL_NOTICE( string strNotice)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SOCIAL_NOTICE;

		byte[] tempNotice = UTF8Encoding.UTF8.GetBytes( strNotice);
		Buffer.BlockCopy( tempNotice, 0, szNotice, 0, tempNotice.Length);
	}
}

public class body_SC_SOCIAL_NOTICE : AsPacketHeader
{
	public byte[] szNotice = new byte[AsGameDefine.MAX_SOCIAL_NOTICE + 1];
}

public class body2_SC_SOCIAL_HISTORY : AsPacketHeader
{
	public int nSubTitleIdx; //if zero is Friand Add Message
	public Int64 nTime;
	public int ePlatform ;	//eSOCIAL_HISTORY_PLATFORM
	public UInt32 nCharUniqKey;
	public byte nLen;

	public string szUserId;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		byte[] idx = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( Int32));
		FieldInfo headerinfo = infotype.GetField( "nSubTitleIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( idx, 0));
		index += sizeof( Int32);

		byte[] tempNewMember = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nTime", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempNewMember, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( tempNewMember, 0));
		index += sizeof( Int64);

		// ePlatform
		byte[] platform = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, platform, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "ePlatform", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( platform, 0));
		index += sizeof( Int32);

		byte[] nCharKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, nCharKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( nCharKey, 0));
		index += sizeof( UInt32);

		headerinfo = infotype.GetField ( "nLen", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( nLen > 0)
		{
			byte[] szCharName = new byte[ nLen + 1];
			Buffer.BlockCopy( data, index, szCharName, 0, nLen);
			szUserId = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( szCharName));
			index += nLen;
		}

		Debug.Log( "nSubTitleIdx"+ nSubTitleIdx + "nTime"+ nTime + "ePlatform"+ ePlatform +"nCharUniqKey"+nCharUniqKey + "nLen" + nLen + "szUserId"+szUserId);
		return index;
	}
}

public class body1_SC_SOCIAL_HISTORY : AsPacketHeader
{
	public uint	nUserUniqKey;//2013.02.27
	public int nMaxPage;
	public byte nCnt;
	public body2_SC_SOCIAL_HISTORY[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		byte[] idx = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( idx, 0));
		index += sizeof( UInt32);
		
	//	headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
	//	headerinfo.SetValue( this, data[ index++]);

		byte[] maxPage = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, maxPage, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( maxPage, 0));
		index += sizeof( Int32);
		
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_SOCIAL_HISTORY[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_SOCIAL_HISTORY();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}

	public void PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo;

		byte[] idx = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( idx, 0));
		index += sizeof( UInt32);
		
	//	headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
	//	headerinfo.SetValue( this, data[ index++]);

		byte[] maxPage = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, maxPage, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( maxPage, 0));
		index += sizeof( Int32);
		
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_SOCIAL_HISTORY[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_SOCIAL_HISTORY();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}
}

public class body_CS_SOCIAL_HISTORY_REGISTER : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public int nSubTitleIdx; //if zero is Friand Add Message
	public int ePlatform ;

	public body_CS_SOCIAL_HISTORY_REGISTER( uint	nCharKey, int nSubTitle, int ePlatformType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SOCIAL_HISTORY_REGISTER;

		nCharUniqKey = nCharKey;
		nSubTitleIdx = nSubTitle;
		ePlatform = ePlatformType;
	}
}

public class body_SC_SOCIAL_HISTORY_REGISTER_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nCharUniqKey;
	public int nSubTitleIdx;
	public int ePlatform;
}

public class body_CS_SOCIAL_UI_SCROLL : AsPacketHeader
{
	public uint nUserUniqKey;//2013.02.26.
	public int eType ;
	public int nPage; //2014.04.16.
	public bool bConnect;

	public body_CS_SOCIAL_UI_SCROLL( uint userUniqKey, eSOCIAL_UI_TYPE type, int page, bool connected)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_SOCIAL_UI_SCROLL;

		nUserUniqKey = userUniqKey;
		eType = (int)type;
		nPage = page;
		bConnect = connected;
	}
}

public class body_SC_SOCIAL_UI_SCROLL : AsPacketHeader
{
	public int eType ; //eSOCIAL_UI_TYPE
	public body1_SC_SOCIAL_HISTORY socialHistory = null;
	public body1_SC_FRIEND_LIST friendList = null;
	public body1_SC_FRIEND_RANDOM randomList = null;
	public body1_SC_BLOCKOUT_LIST blockOutList = null;
	public body1_SC_FRIEND_LIST friendApplyList = null;
	public body_SC_SOCIAL_RECOMMEND recommendList = null;

	public new void PacketBytesToClass( byte[] data)
	{
		FieldInfo headerinfo = null;
		Type infotype = this.GetType();
		int index = ParsePacketHeader( data);

		// eType
		byte[] type = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, type, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eType", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( type, 0));
		index += sizeof( Int32);

		switch( ( eSOCIAL_UI_TYPE)eType)
		{
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY:
			{
				socialHistory = new body1_SC_SOCIAL_HISTORY();
				socialHistory.PacketBytesToClass( data, index);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND:
			{
				friendList = new body1_SC_FRIEND_LIST();
				friendList.PacketBytesToClass( data, index);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_RANDOM:
			{
				randomList = new body1_SC_FRIEND_RANDOM();
				randomList.PacketBytesToClass( data, index);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_BLOCK:
			{
				blockOutList = new body1_SC_BLOCKOUT_LIST();
				blockOutList.PacketBytesToClass( data, index);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND_APPLY:
			{
				friendApplyList = new body1_SC_FRIEND_LIST();
				friendApplyList.PacketBytesToClass( data, index);
			}
			break;
		case eSOCIAL_UI_TYPE.eSOCIAL_UI_RECOMMEND:
			{
				recommendList = new body_SC_SOCIAL_RECOMMEND();
				recommendList.PacketBytesToClass( data, index);
			}
			break;
		}
	}
}

public class body_SC_FRIEND_CONNECT : AsPacketHeader
{
	public uint nUserUniqKey;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public bool bConnect;				// ( false=disconnect. true=connect).
}

public class body_CS_FRIEND_LIST : AsPacketHeader
{
	public uint nUserUniqKey; //2013.02.26.
	public bool bConnect;				// ( false=disconnect. true=connect).

	public body_CS_FRIEND_LIST( uint userUniqKey, bool isConnect)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_FRIEND_LIST;
		nUserUniqKey = userUniqKey;
		bConnect = isConnect;
	}
}

public class body2_SC_FRIEND_LIST : AsPacketHeader
{
	public UInt16 nSessionIdx;			// 0. offline, !0. online
	public uint nUserUniqKey;
	public uint nCharUniqKey;
	public string szCharName;
	public int nImageIndex;
	public int nLevel;
	public byte nFriendlyLevel; //( 0~99)
	public byte nFriendlyRate;
	public int eButtonType; //eFRIEND_BUTTON_TYPE
	public bool bConnectRequest;
	public Int64 nLastConnectTime;
	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nSessionIdx
		byte[] sessionIdx = new byte[ sizeof( UInt16)];
		Buffer.BlockCopy( data, index, sessionIdx, 0, sizeof( UInt16));
		FieldInfo headerinfo = infotype.GetField( "nSessionIdx", BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToUInt16( sessionIdx, 0));
		index += sizeof( UInt16);

		byte[] idx = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, idx, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( idx, 0));
		index += sizeof( UInt32);

		// nCharUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		byte[] szUserName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, szUserName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( szUserName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

			// nImageIndex
		byte[] nImage = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, nImage, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nImageIndex", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( nImage, 0));
		index += sizeof( Int32);

			// nLevel
		byte[] level = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, level, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nLevel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( level, 0));
		index += sizeof( Int32);

		headerinfo = infotype.GetField ( "nFriendlyLevel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nFriendlyRate", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		byte[] nButtonType = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, nButtonType, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eButtonType", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( nButtonType, 0));
		index += sizeof( Int32);
		
			// bConnectRequest
		byte[] connectRequest = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, connectRequest, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bConnectRequest", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( connectRequest, 0));
		index += sizeof( bool);

		
		byte[] tempTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nLastConnectTime", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( tempTime, 0));
		index += sizeof( Int64);
		
		return index;
	}
}

public class body1_SC_FRIEND_LIST : AsPacketHeader
{
	public uint nUserUniqKey;//2013.02.26.
	public byte nCnt;
	public byte nMaxPage;
	public Int16 nHelloCount;
	public Int16 nMaxHelloCount;
	public byte nChargeConditionPointRate;
	public byte nChargeConditionCount;
	public byte nMaxChargeConditionCount;
	
	public body2_SC_FRIEND_LIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nUserUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		byte[] helloCount = new byte[ sizeof( Int16)];
		Buffer.BlockCopy( data, index, helloCount, 0, sizeof( Int16));
		headerinfo = infotype.GetField( "nHelloCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToInt16( helloCount, 0));
		index += sizeof( Int16);

		byte[] maxHelloCount = new byte[ sizeof( Int16)];
		Buffer.BlockCopy( data, index, maxHelloCount, 0, sizeof( Int16));
		headerinfo = infotype.GetField( "nMaxHelloCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToInt16( maxHelloCount, 0));
		index += sizeof( Int16);
		
		headerinfo = infotype.GetField ( "nChargeConditionPointRate", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nChargeConditionCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nMaxChargeConditionCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		
		if( 0 < nCnt)
		{
			body = new body2_SC_FRIEND_LIST[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_FRIEND_LIST();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}

	public void PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo;

		// nUserUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		byte[] helloCount = new byte[ sizeof( Int16)];
		Buffer.BlockCopy( data, index, helloCount, 0, sizeof( Int16));
		headerinfo = infotype.GetField( "nHelloCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToInt16( helloCount, 0));
		index += sizeof( Int16);

		byte[] maxHelloCount = new byte[ sizeof( Int16)];
		Buffer.BlockCopy( data, index, maxHelloCount, 0, sizeof( Int16));
		headerinfo = infotype.GetField( "nMaxHelloCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue ( this, BitConverter.ToInt16( maxHelloCount, 0));
		index += sizeof( Int16);
		
		headerinfo = infotype.GetField ( "nChargeConditionPointRate", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nChargeConditionCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		headerinfo = infotype.GetField ( "nMaxChargeConditionCount", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_FRIEND_LIST[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_FRIEND_LIST();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}
}

public class body_CS_FRIEND_INVITE : AsPacketHeader
{
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1]; //2013.02.25

	public body_CS_FRIEND_INVITE( string name)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_FRIEND_INVITE;

		byte[] tempName = UTF8Encoding.UTF8.GetBytes( name);
		Buffer.BlockCopy( tempName, 0, szCharName, 0, tempName.Length);
	}
}

public class body_SC_FRIEND_INVITE : AsPacketHeader
{
	public uint nUserUniqKey;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_SC_FRIEND_INVITE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_FRIEND_JOIN : AsPacketHeader
{
	public int eJoinType; //eFRIEND_JOIN_TYPE
	public uint nUserUniqKey; ////invite UserUniqKey
	public byte[] szIviteUserId = new byte[ AsGameDefine.MAX_NAME_LEN + 1];

	public body_CS_FRIEND_JOIN( string name, uint userUniqKey, int joinType)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_FRIEND_JOIN;

		byte[] tempName = UTF8Encoding.UTF8.GetBytes( name);
		Buffer.BlockCopy( tempName, 0, szIviteUserId, 0, tempName.Length);
		nUserUniqKey = userUniqKey;
		eJoinType = joinType;
	}
}

public class body_SC_FRIEND_JOIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_FRIEND_INSERT : AsPacketHeader
{
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_FRIEND_CLEAR : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_FRIEND_CLEAR( uint nUserKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_FRIEND_CLEAR;

		nUserUniqKey = nUserKey;
	}
}

public class body_SC_FRIEND_CLEAR_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_SC_FRIEND_DELETE : AsPacketHeader
{
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_FRIEND_HELLO : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_FRIEND_HELLO( uint userUniqKey)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_HELLO;

		nUserUniqKey = userUniqKey;
	}
}

public class body_SC_FRIEND_HELLO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public bool bDual;						//
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_SC_FRIEND_HELLO : AsPacketHeader
{
	public bool bDual;						//
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_FRIEND_RECALL : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_FRIEND_RECALL( uint userUniqKey)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_RECALL;

		nUserUniqKey = userUniqKey;
	}
}

public class body_SC_FRIEND_RECALL_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public byte nRecallCount;
	public byte nMaxRecallCount;
}

public class body_CS_FRIEND_RANDOM : AsPacketHeader
{
	//nothing...
	public body_CS_FRIEND_RANDOM()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_RANDOM;
	}
}

public class body2_SC_FRIEND_RANDOM : AsPacketHeader
{
	public uint nUserUniqKey;
	public string szCharName;
	public int nImageIndex;
	public int nLevel;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nUserUniqKey
		byte[] idx = new byte[ sizeof( UInt32)];
		FieldInfo headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, idx, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( idx, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] szName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, szName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( szName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nImageIndex
		byte[] nImage = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nImageIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, nImage, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( nImage, 0));
		index += sizeof( Int32);

		// nLevel
		byte[] level = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLevel", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, level, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( level, 0));
		index += sizeof( Int32);

		return index;
	}
}

public class body1_SC_FRIEND_RANDOM : AsPacketHeader
{
	public byte nCnt;
	public byte nMaxPage;
	public body2_SC_FRIEND_RANDOM[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_FRIEND_RANDOM[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_FRIEND_RANDOM();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}

	public void PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo;

		// nCnt
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_FRIEND_RANDOM[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_FRIEND_RANDOM();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}
}

public class body2_SC_BLOCKOUT_LIST : AsPacketHeader
{
	public uint nUserUniqKey;
	public string szUserId;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nUserUniqKey
		byte[] userUniqKey = new byte[ sizeof( UInt32)];
		FieldInfo headerinfo = infotype.GetField( "nUserUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, userUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( userUniqKey, 0));
		index += sizeof( UInt32);

		// szUserId
		byte[] userId = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, userId, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szUserId =AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( userId));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		return index;
	}
}

public class body1_SC_BLOCKOUT_LIST : AsPacketHeader
{
	public byte nCnt;
	public byte nMaxPage;
	public body2_SC_BLOCKOUT_LIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// nMaxPage
		headerinfo = infotype.GetField ( "nMaxPage", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_BLOCKOUT_LIST[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_BLOCKOUT_LIST();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}

	public void PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo;

		headerinfo = infotype.GetField ( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 < nCnt)
		{
			body = new body2_SC_BLOCKOUT_LIST[nCnt];
			for ( int i = 0; i < nCnt; i++)
			{
				body[i] = new body2_SC_BLOCKOUT_LIST();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}
}

public class body_CS_BLOCKOUT_INSERT : AsPacketHeader
{
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];

	public body_CS_BLOCKOUT_INSERT( string name)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_BLOCKOUT_INSERT;

		byte[] tempName = UTF8Encoding.UTF8.GetBytes( name);
		Buffer.BlockCopy( tempName, 0, szCharName, 0, tempName.Length);
	}
}

public class body_SC_BLOCKOUT_INSERT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public uint nUserUniqKey;
	public byte[] szUserId = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_BLOCKOUT_DELETE : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_BLOCKOUT_DELETE( uint nUserKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_BLOCKOUT_DELETE;
		nUserUniqKey = nUserKey;
	}
}

public class body_SC_BLOCKOUT_DELETE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public uint nUserUniqKey;
	public byte[] szUserId = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}

public class body_CS_SOCIAL_ITEMBUY : AsPacketHeader
{
	public int nShopItemSlot;
	public ushort nCount;

	public body_CS_SOCIAL_ITEMBUY( int shopItemSlot, ushort cnt)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_SOCIAL_ITEMBUY;

		nShopItemSlot = shopItemSlot;
		nCount = cnt;
	}
}

public class body_SC_SOCIAL_ITEMBUY_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public int nSocialPoint;
}

public class body_CS_FRIEND_WARP : AsPacketHeader
{
	public uint nUserUniqKey;

	public body_CS_FRIEND_WARP( uint userUniqKey)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_WARP;

		nUserUniqKey = userUniqKey;
	}
}

public class body_SC_FRIEND_WARP : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CS_FRIEND_CONDITION : AsPacketHeader
{
	//nothing..
	public body_CS_FRIEND_CONDITION( )
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_CONDITION;

		
	}
}

public class body_SC_FRIEND_CONDITION_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CS_GAME_INVITE_LIST : AsPacketHeader
{
	//nothing...
	public body_CS_GAME_INVITE_LIST()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_GAME_INVITE_LIST;
	}
}

public class body_SC_GAME_INVITE_LIST_RESULT : AsPacketHeader
{
	public short nFacebook_Day;
	public short nFacebook_Day_Max;
	public Int32 nFacebook_Total;
	public Int16[] nFacebook_Goal = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Byte[] bFacebook_Reward = new Byte[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int32[] nFacebook_Reward_ItemIdx = new Int32[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int16[] nFacebook_Reward_ItemCount = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];
	
	public short nKakaotalk_Day;
	public short nKakaotalk_Day_Max;
	
	public short nSms_Day;
	public short nSms_Day_Max;
	public Int32 nSms_Total;
	public Int16[] nSms_Goal = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Byte[] bSms_Reward = new Byte[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int32[] nSms_Reward_ItemIdx = new Int32[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int16[] nSms_Reward_ItemCount = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];
	
	
	public short nBand_Day;
	public short nBand_Day_Max;
	public Int32 nBand_Total;
	public Int16[] nBand_Goal = new short[AsGameDefine.MAX_BAND_GAME_INVITE_REWARD];
	public Byte[] bBand_Reward = new Byte[AsGameDefine.MAX_BAND_GAME_INVITE_REWARD];
	public Int32[] nBand_Reward_ItemIdx = new Int32[AsGameDefine.MAX_BAND_GAME_INVITE_REWARD];
	public Int16[] nBand_Reward_ItemCount = new short[AsGameDefine.MAX_BAND_GAME_INVITE_REWARD];	
	
	public short	nLine_Day;
	public short	nLine_Day_Max;	
	public Int16	nLine_Goal;
	public Byte		bLine_Reward;
	public Int32	nLine_Reward_ItemIdx;
	public Int16	nLine_Reward_ItemCount;

	public short	nLobi_Day;
	public short	nLobi_Day_Max;	
	public Int16	nLobi_Goal;
	public Byte		bLobi_Reward;
	public Int32	nLobi_Reward_ItemIdx;
	public Int16	nLobi_Reward_ItemCount;

	public short nTwitter_Day;
	public short nTwitter_Day_Max;
	public Int32 nTwitter_Total;
	public Int16[] nTwitter_Goal = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Byte[] bTwitter_Reward = new Byte[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int32[] nTwitter_Reward_ItemIdx = new Int32[AsGameDefine.MAX_GAME_INVITE_REWARD];
	public Int16[] nTwitter_Reward_ItemCount = new short[AsGameDefine.MAX_GAME_INVITE_REWARD];	
	
}

public class body_SC_SOCIAL_RECOMMEND : AsPacketHeader
{
	public Int32 nRecommendCompleteCount;
	public Int32[] arrItemIndex = new Int32[AsGameDefine.eRECOMMEND_ACCURE_MAX];
	public Int32[] arrItemCount = new Int32[AsGameDefine.eRECOMMEND_ACCURE_MAX];
	public Int32[] arrAccrueCount = new Int32[AsGameDefine.eRECOMMEND_ACCURE_MAX];

	public void PacketBytesToClass( byte[] data, int index)
	{
		byte[] packetBody = new byte[ data.Length - index];
		Buffer.BlockCopy( data, index, packetBody, 0, packetBody.Length);
		ByteArrayToClass( packetBody);
	}
}

public class body_CS_GAME_INVITE : AsPacketHeader
{
	public bool bCheck; //( true:check false :register).
	public int ePlatform; //eGAME_INVITE_PLATFORM
	public byte[] szKeyword = new byte[AsGameDefine.eGAMEINVITE_KEY + 1];

	public body_CS_GAME_INVITE( bool check,int nPlatform, string strKeyword)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GAME_INVITE;
		bCheck = check;
		ePlatform = nPlatform;
		byte[] tempKeyword = UTF8Encoding.UTF8.GetBytes( strKeyword);
		Buffer.BlockCopy( tempKeyword, 0, szKeyword, 0, tempKeyword.Length);
	}
}

public class body_SC_GAME_INVITE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public bool bCheck; //( true:check false :register).
	public int ePlatform; //eGAME_INVITE_PLATFORM
	public byte[] szKeyword = new byte[AsGameDefine.eGAMEINVITE_KEY + 1];
}

public class body_CS_GAME_INVITE_REWARD : AsPacketHeader
{
	public int ePlatform; //eGAME_INVITE_PLATFORM
	public int nIndex;
	public body_CS_GAME_INVITE_REWARD( int nPlatform, int nId)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_GAME_INVITE_REWARD;

		ePlatform = nPlatform;
		nIndex = nId;
	}
}

public class body_SC_GAME_INVITE_REWARD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public int ePlatform;
}

public class body_CS_FRIEND_CONNECT_REUQEST : AsPacketHeader
{
	public uint nUserUniqKey;
	public body_CS_FRIEND_CONNECT_REUQEST( uint _nUserUniqKey )
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_FRIEND_CONNECT_REQUEST;

		nUserUniqKey = _nUserUniqKey;
	}
}

public class body_SC_FRIEND_CONNECT_REQUEST_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

#endregion

#region - Collect -
public class body_CS_COLLECT_REQUEST : AsPacketHeader
{
	public Int32 nCollectNpcIdx;
	public Int32 eCollectState;

	public body_CS_COLLECT_REQUEST( Int32 collectNpcIdx, eCOLLECT_STATE eState)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte) PROTOCOL_CS_2.CS_COLLECT_REQUEST;

		nCollectNpcIdx = collectNpcIdx;
		eCollectState = ( Int32)eState;
	}
}

public class body_SC_COLLECT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 eCollectState;
}

public class body_SC_COLLECT_INFO : AsPacketHeader
{
	public UInt16 nCollectorSession;
	public UInt32 nCollectorUniqKey;
	public Int32 nCollectNpcIdx;
	public Int32 eCollectState;
};
#endregion

public class body_CS_ITEM_STRENGTHEN : AsPacketHeader
{
	public Int32 eStrengthenType; //eITEM_STRENGTHEN_TYPE
	public Int16 nSlot;
	public bool isUseCash;

	public body_CS_ITEM_STRENGTHEN( eITEM_STRENGTHEN_TYPE _eStrengthenType, Int16 _nSlot, bool _isUseCash)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_STRENGTHEN;

		eStrengthenType = (int)_eStrengthenType;
		nSlot = _nSlot;
		isUseCash = _isUseCash;
	}
};

public class body_CS_ITEM_ENCHANT : AsPacketHeader
{
	public Int16 nSlot1;
	public Int16 nStoneSlot;
	public byte nEnchantPos;

	public body_CS_ITEM_ENCHANT( Int16 _nItemSlot, Int16 _nSoulStoneIdx, byte _nEnchantPos)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_ENCHANT;

		nSlot1 = _nItemSlot;
		nEnchantPos = _nEnchantPos;
		nStoneSlot = _nSoulStoneIdx;
	}
};

public class body_CS_ITEM_ENCHANT_OUT : AsPacketHeader
{
	public Int16 nEquipSlot;
	public byte nEnchantPos;

	public body_CS_ITEM_ENCHANT_OUT( Int16 _nEquipSlot, byte _nEnchantPos)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_ENCHANT_OUT;

		nEquipSlot = _nEquipSlot;
		nEnchantPos = _nEnchantPos;
	}
};

public class body_SC_ITEM_ENCHANT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_SC_ITEM_ENCHANT_OUT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_SC_ITEM_STRENGTHEN_RESULT : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public UInt32 nCharUniqKey;

	public eITEM_STRENGTHEN_TYPE eStrengthenType;
	public eRESULTCODE eResult;
}


public class body_CS_ITEM_MIX : AsPacketHeader
{
	public int eMixType = 0;

	public Int16 nSlot1 = 0;
	public Int16 nSlot2 = 0;	
	  
	public body_CS_ITEM_MIX( int _eMixType,  Int16 _nSlot1, Int16 _nSlot2 )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ITEM_MIX;
		
		eMixType = _eMixType;
	
		nSlot1 = _nSlot1;
		nSlot2 = _nSlot2;		
	}
}

public class body_SC_ITEM_MIX_RESULT : AsPacketHeader
{
	public int eMixType;
	public eRESULTCODE eResult;
	public Int16 nResultSlot1;
	public Int16 nResultSlot2;
	public Int16 nResultSlot3;
}

public class body_CS_COS_ITEM_MIX_UP : AsPacketHeader
{
	public Int16				nTargetSlot;

	public Int16				nRawSlot1;
	public Int16				nRawSlot2;
	public Int16				nRawSlot3;

	public body_CS_COS_ITEM_MIX_UP( Int16 _targetSlot , Int16 _slot1 , Int16 _slot2 , Int16 _slot3  )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_COS_ITEM_MIX_UP;
		nTargetSlot 	= _targetSlot;
		nRawSlot1 		= _slot1;
		nRawSlot2 		= _slot2;
		nRawSlot3 		= _slot3;
	}
}

public class body_SC_COS_ITEM_MIX_UP_RESULT : AsPacketHeader
{
	public eRESULTCODE 	eResult;
	public Int16 				nResultSlot;
	public bool 					bGreat;
}

public class body_CS_COS_ITEM_MIX_UPGRADE : AsPacketHeader
{
	public Int16				nTargetSlot;
	public bool 				bCash;
	
	public body_CS_COS_ITEM_MIX_UPGRADE( Int16 _targetSlot , bool _isCash )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_COS_ITEM_MIX_UPGRADE;
		nTargetSlot 	= _targetSlot;
		bCash 			= _isCash;
	}
}

public class body_SC_COS_ITEM_MIX_UPGRADE_RESULT : AsPacketHeader
{
	public eRESULTCODE 	eResult;
	public Int16 				nResultSlot;
}


public class body_CS_OTHER_INFO : AsPacketHeader
{
	public int eInfoType; //eOTHER_INFO_TYPE
	public ushort nSessionIdx;
	public uint nCharUniqKey;
	public uint nUserUniqKey;

	public body_CS_OTHER_INFO( int _eInfoType, ushort _nSessionIdx, uint _nCharUniqKey, uint _nUserUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_OTHER_INFO;
		eInfoType = _eInfoType;
		nSessionIdx = _nSessionIdx;
		nCharUniqKey = _nCharUniqKey;
		nUserUniqKey = _nUserUniqKey;
	}
}

public class body1_SC_OTHER_INFO : AsPacketHeader
{
	public string szCharName;
	public Int32 nSubTitleTableIdx;
	public Int32 nLevel;
	public Int32 nClass;
	public Single fDef_Physic_Dmg_Dec;
	public Single fDef_Magic_Dmg_Dec;
	public Single fHPMax;
	public Single fMPMax;
	public Int32 nPhysicDmg_Min;
	public Int32 nPhysicDmg_Max;
	public Int32 nMagicDmg_Min;
	public Int32 nMagicDmg_Max;
	public Int32 nPhysic_Def;
	public Single fPhysic_Dmg_Dec;
	public Int32 nMagic_Def;
	public Single fMagic_Dmg_Dec;
	public Int32 nCriticalProb;
	public Int32 nAccuracyProb;
	public Int32 nDodgeProb;
	public Int32 nMoveSpeed;
	public Int32 nAtkSpeed;
	public Single nHPRegen;
	public Single nMPRegen;
	public Int32 nTotExp;
	public byte nCnt;
	public body2_SC_OTHER_INFO[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		//nSubTitleTableIdx
		byte[] subTitleTableIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nSubTitleTableIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, subTitleTableIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( subTitleTableIdx, 0));
		index += sizeof( Int32);

		//nLevel
		byte[] level = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLevel", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, level, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( level, 0));
		index += sizeof( Int32);

		//class
		byte[] cls = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nClass", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, cls, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( cls, 0));
		index += sizeof( Int32);

		//fDef_Physic_Dmg_Dec
		byte[] defPhysicDmgDec = new byte[sizeof( Single)];
		headerinfo = infotype.GetField( "fDef_Physic_Dmg_Dec", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, defPhysicDmgDec, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( defPhysicDmgDec, 0));
		index += sizeof( Single);

		//fDef_Magic_Dmg_Dec
		byte[] defMagicDmgDec = new byte[sizeof( Single)];
		headerinfo = infotype.GetField( "fDef_Magic_Dmg_Dec", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, defMagicDmgDec, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( defMagicDmgDec, 0));
		index += sizeof( Single);

		//fHPMax
		byte[] hpMax = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "fHPMax", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, hpMax, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( hpMax, 0));
		index += sizeof( Single);

		//fMPMax
		byte[] mpMax = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "fMPMax", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, mpMax, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( mpMax, 0));
		index += sizeof( Single);

		//nPhysicDmg_Min
		byte[] physicDmgMin = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPhysicDmg_Min", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, physicDmgMin, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( physicDmgMin, 0));
		index += sizeof( Int32);

		//nPhysicDmg_Max
		byte[] physicDmgMax = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPhysicDmg_Max", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, physicDmgMax, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( physicDmgMax, 0));
		index += sizeof( Int32);

		//nMagicDmg_Min
		byte[] magicDmgMin = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMagicDmg_Min", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, magicDmgMin, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( magicDmgMin, 0));
		index += sizeof( Int32);

		//nMagicDmg_Max
		byte[] magicDmgMax = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMagicDmg_Max", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, magicDmgMax, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( magicDmgMax, 0));
		index += sizeof( Int32);

		//nPhysic_Def
		byte[] physicDef = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPhysic_Def", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, physicDef, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( physicDef, 0));
		index += sizeof( Int32);

		//fPhysic_Dmg_Dec
		byte[] physicDmgDec = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "fPhysic_Dmg_Dec", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, physicDmgDec, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( physicDmgDec, 0));
		index += sizeof( Single);

		//nMagic_Def
		byte[] magicDef = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMagic_Def", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, magicDef, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( magicDef, 0));
		index += sizeof( Int32);

		//fMagic_Dmg_Dec
		byte[] magicDmgDec = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "fMagic_Dmg_Dec", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, magicDmgDec, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( magicDmgDec, 0));
		index += sizeof( Single);

		//nCriticalProb
		byte[] criticalProb = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCriticalProb", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, criticalProb, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( criticalProb, 0));
		index += sizeof( Int32);

		//nAccuracyProb
		byte[] accuracyProb = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nAccuracyProb", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, accuracyProb, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( accuracyProb, 0));
		index += sizeof( Int32);

		//nDodgeProb
		byte[] dodgeProb = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nDodgeProb", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, dodgeProb, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( dodgeProb, 0));
		index += sizeof( Int32);

		//nMoveSpeed
		byte[] moveSpeed = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMoveSpeed", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, moveSpeed, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( moveSpeed, 0));
		index += sizeof( Int32);

		//nAtkSpeed
		byte[] atkSpeed = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nAtkSpeed", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, atkSpeed, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( atkSpeed, 0));
		index += sizeof( Int32);

		//nHPRegen
		byte[] hpRegen = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "nHPRegen", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, hpRegen, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( hpRegen, 0));
		index += sizeof( Single);

		//nMPRegen
		byte[] mpRegen = new byte[ sizeof( Single)];
		headerinfo = infotype.GetField( "nMPRegen", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, mpRegen, 0, sizeof( Single));
		headerinfo.SetValue( this, BitConverter.ToSingle( mpRegen, 0));
		index += sizeof( Single);

		//nTotExp
		byte[] totExp = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nTotExp", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, totExp, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( totExp, 0));
		index += sizeof( Int32);

		// nCnt
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		if( 0 >= nCnt)
			return;

		// body
		body = new body2_SC_OTHER_INFO[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_OTHER_INFO();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_SC_OTHER_INFO
{
	public ushort nSlot;
	public sITEM sItem = new sITEM();

	public static int size
	{
		get	{ return ( sizeof( ushort) + sITEM.size); }
	}

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nSlot
		byte[] slot = new byte[ sizeof( UInt16)];
		FieldInfo headerinfo = infotype.GetField( "nSlot", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, slot, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( slot, 0));
		index += sizeof( UInt16);

		// sItem
		byte[] item = new byte[ sITEM.size];
		Buffer.BlockCopy( data, index, item, 0, sITEM.size);
		sItem.ByteArrayToClass( item);
		index += sITEM.size;

		return index;
	}
}



public class body_SC_COSTUME_ONOFF : AsPacketHeader
{
	public UInt16 nUserSession;
	public UInt32 nCharUniqKey;
#if USE_OLD_COSTUME
	public bool bCostumeOnOff;
#else
	public Int32 bCostumeOnOff;
	public Int32 nHair;
	public Int32 nHairColor;
#endif
};

public class body_CS_COSTUME_ONOFF : AsPacketHeader
{
#if USE_OLD_COSTUME
	public bool bCostumeOnOff;
	public body_CS_COSTUME_ONOFF( bool _isCostumeOff)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_COSTUME_ONOFF;
		bCostumeOnOff = _isCostumeOff;
	}
#else
	public Int32				bCostumeOnOff;
	public body_CS_COSTUME_ONOFF( Int32 _isCostumeOff)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_COSTUME_ONOFF;
		bCostumeOnOff = _isCostumeOff;
	}
#endif
	
};


public class body_SC_RESURRECT_PENALTY_CLEAR :AsPacketHeader
{
	public eRESULTCODE eResult_;
}

public class body_CS_RESURRECT_PENALTY_CLEAR :AsPacketHeader
{
	public body_CS_RESURRECT_PENALTY_CLEAR()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_RESURRECT_PENALTY_CLEAR;
	}
}

public class body_CS_COUPON_REGIST : AsPacketHeader
{
	public byte[] szCouponKey = new byte[AsGameDefine.eAPPID];

	public body_CS_COUPON_REGIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_COUPON_REGIST;
	}
};

public class body_SC_COUPON_REGIST : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nProductID;

	public string CouponReward
	{
		get
		{
////			1634	골드 1만골
////1635	골드 10만골
////1636	미라클 10개
////1637	미라클 100개
//
//			switch( nCouponType)
//			{
//			case eCOUPON_TYPE.eCOUPON_TYPE_M010:
//				return AsTableManager.Instance.GetTbl_String( 1636);
//			case eCOUPON_TYPE.eCOUPON_TYPE_M100:
//				return AsTableManager.Instance.GetTbl_String( 1637);
//			case eCOUPON_TYPE.eCOUPON_TYPE_G010:
//				return AsTableManager.Instance.GetTbl_String( 1634);
//			case eCOUPON_TYPE.eCOUPON_TYPE_G100:
//				return AsTableManager.Instance.GetTbl_String( 1635);
//			}
			Item item = ItemMgr.ItemManagement.GetItem( nProductID);
			if( item != null)
				return AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
			else
			{
				Debug.LogError( "body_SC_COUPON_REGIST::CouponValue: nProductID = [" + nProductID + "]");
				return "unknown";
			}
		}
	}

//	eCOUPON_TYPE_M010,		// 미라클
//	eCOUPON_TYPE_M100,
//
//	eCOUPON_TYPE_G010,		// 골드( *1000)
//	eCOUPON_TYPE_G100,
};

public class body_CS_EVENT_LIST :AsPacketHeader
{
	public body_CS_EVENT_LIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_EVENT_LIST;
	}
};

#region -EventStart
public class body1_SC_SERVER_EVENT_START : AsPacketHeader
{
	public Int32 nBody2Cnt;
	public body2_SC_SERVER_EVENT_START[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nBody2Cnt
		byte[] cnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nBody2Cnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, cnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( cnt, 0));
		index += sizeof( Int32);

		if( 0 >= nBody2Cnt)
			return;

		// body
		body = new body2_SC_SERVER_EVENT_START[ nBody2Cnt];
		for( int i = 0; i < nBody2Cnt; i++)
		{
			body[i] = new body2_SC_SERVER_EVENT_START();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
};

public class body2_SC_SERVER_EVENT_START
{
	public Int32 nEventKey;
	public eEVENT_TYPE eEventType;
	public Int32 nValue1;
	public Int32 nValue2;
	public Int32 nValue3;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nEventKey
		byte[] eventKey = new byte[ sizeof( Int32)];
		FieldInfo headerinfo = infotype.GetField( "nEventKey", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, eventKey, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( eventKey, 0));
		index += sizeof( Int32);

		// eEventType
		byte[] eventType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eEventType", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, eventType, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( eventType, 0));
		index += sizeof( Int32);

		// nValue1
		byte[] value1 = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nValue1", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, value1, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( value1, 0));
		index += sizeof( Int32);

		// nValue2
		byte[] value2 = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nValue2", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, value2, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( value2, 0));
		index += sizeof( Int32);

		// nValue3
		byte[] value3 = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nValue3", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, value3, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( value3, 0));
		index += sizeof( Int32);

		return index;
	}
};

public class body1_SC_SERVER_EVENT_STOP : AsPacketHeader
{
	public Int32 nBody2Cnt;
	public body2_SC_SERVER_EVENT_STOP[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nBody2Cnt
		byte[] cnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nBody2Cnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, cnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( cnt, 0));
		index += sizeof( Int32);

		// body
		body = new body2_SC_SERVER_EVENT_STOP[ nBody2Cnt];
		for( int i = 0; i < nBody2Cnt; i++)
		{
			body[i] = new body2_SC_SERVER_EVENT_STOP();
			index = body[i].PacketBytesToClass( data, index);
		}
	}
};

public class body2_SC_SERVER_EVENT_STOP
{
	public Int32 nEventKey;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nEventKey
		byte[] eventKey = new byte[ sizeof( Int32)];
		FieldInfo headerinfo = infotype.GetField( "nEventKey", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, eventKey, 0, sizeof( Int32));
		headerinfo.SetValue ( this, BitConverter.ToInt32( eventKey, 0));
		index += sizeof( Int32);

		return index;
	}
};
#endregion

public class body_CS_GAME_REVIEW :AsPacketHeader
{
	public bool bReview;

	public body_CS_GAME_REVIEW( bool _bReview)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_GAME_REVIEW;

		bReview = _bReview;
	}
};

public class body2_SC_EVENT_LIST
{
	public Int64 tStartTime;
	public Int64 tEndTime;
	public string szTitle;
	public string szContent;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		// tStartTime
		byte[] startTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "tStartTime", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, startTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( startTime, 0));
		index += sizeof( Int64);

		// tEndTime
		byte[] endTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "tEndTime", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, endTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( endTime, 0));
		index += sizeof( Int64);

		// szTitle
		byte[] title = new byte[ AsGameDefine.MAX_EVENT_TITLE + 1];
		Buffer.BlockCopy( data, index, title, 0, AsGameDefine.MAX_EVENT_TITLE + 1);
		szTitle = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( title));
		index += ( AsGameDefine.MAX_EVENT_TITLE + 1);

		// szContent
		byte[] content = new byte[ AsGameDefine.MAX_EVENT_CONTENT + 1];
		Buffer.BlockCopy( data, index, content, 0, AsGameDefine.MAX_EVENT_CONTENT + 1);
		szContent = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( content));
		index += ( AsGameDefine.MAX_EVENT_CONTENT + 1);

		return index;
	}
}

public class body1_SC_EVENT_LIST : AsPacketHeader
{
	public Int32 nBody2Cnt;
	public body2_SC_EVENT_LIST[] body;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nBody2Cnt
		byte[] nCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nBody2Cnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, nCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( nCnt, 0));
		index += sizeof( Int32);

		if( 0 < nBody2Cnt)
		{
			body = new body2_SC_EVENT_LIST[ nBody2Cnt];
			for( int i = 0; i < nBody2Cnt; i++)
			{
				body[i] = new body2_SC_EVENT_LIST();
				index = body[i].PacketBytesToClass( data, index);
			}
		}
	}
};

public class body_SC_GAME_REWIVEW : AsPacketHeader
{
	//nothing
};

public class body_SC_USER_EVENT_NOTIFY : AsPacketHeader
{
	public int eType; //eUSEREVENTTYPE.

	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public int nValue_1;//eUSEREVENTTYPE_ITEM 일 경우 아이템 테이블 인덱스. -> eUSEREVENTTYPE_ITEM 일 경우 획득 방법( eUSEREVENT_ITEM_GETTYPE 참조).
	public int nValue_2; //eUSEREVENTTYPE_ITEM 일 경우 획득 방법( eUSEREVENT_ITEM_GETTYPE 참조). -> entity idx
	public int nValue_3; //eUSEREVENTTYPE_ITEM 이면서 nValue가 eUSEREVENT_ITEM_GETTYPE_STRENGTHEN 일 경우 강화 수. - > no use
	public Int64 nServerTime;
	public sITEM sItem;
}

public class body_SC_WANTED_START : AsPacketHeader
{
	public int nQuestTableIdx;
	public int eStatus;
};

public class body_SC_WANTED_CLEAR : AsPacketHeader
{
	//noting..
};

public class body_CS_WANTED_COMPLETE : AsPacketHeader
{
	public int nQuestTableIdx;

	public body_CS_WANTED_COMPLETE( int _tableIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_WANTED_COMPLETE;
		nQuestTableIdx = _tableIdx;
	}
};

public class body_SC_WANTED_COMPLETE : AsPacketHeader
{
	public eRESULTCODE eResult;
};

#region -Condition
public class body_SC_CONDITION : AsPacketHeader
{
	public eCONDITION_TYPE eType;
	public UInt32 nCondition;
};
#endregion

#region -Ranking
public enum eRANKVALUE : int
{
	eRANKVALUE_MAXCOUNT_PER_PAGE = 7,
};

public class sRANKINFO
{
	public UInt32 nRank;
	public ushort nSessionIdx;
	public UInt32 nUserUniqKey;
	public UInt32 nCharUniqKey;
	public string szCharName;
	public eCLASS eClass;
	public Int32 nRankPoint;
	public Int32 nDiffRank;
	public Int32 nImageTableIdx;
	public Int32 nLevel;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		// nRank
		byte[] tempRank = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nRank", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempRank, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempRank, 0));
		index += sizeof( UInt32);

		// nSessionIdx
		byte[] tempSessionIdx = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nSessionIdx", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempSessionIdx, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( tempSessionIdx, 0));
		index += sizeof( UInt16);

		// nUserUniqKey;
		byte[] tempUserKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nUserUniqKey", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempUserKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempUserKey, 0));
		index += sizeof( UInt32);

		// nCharUniqKey
		byte[] tempCharKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempCharKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempCharKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// eClass
		byte[] cls = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, cls, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eClass", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( cls, 0));
		index += sizeof( Int32);

		// nRankPoint
		byte[] rankPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nRankPoint", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankPoint, 0));
		index += sizeof( Int32);

		// nDiffRank
		byte[] diffRank = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nDiffRank", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, diffRank, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( diffRank, 0));
		index += sizeof( Int32);

		// nImageTableIdx
		byte[] imageIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nImageTableIdx", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, imageIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( imageIdx, 0));
		index += sizeof( Int32);

		// nLevel
		byte[] level = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLevel", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, level, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( level, 0));
		index += sizeof( Int32);

		return index;
	}
};

public class sMYRANKINFO
{
	public UInt32 nRank;
	public Int32 nDiffRank;
	public Int32 nRankPoint;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		// nRank
		byte[] tempRank = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nRank", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempRank, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempRank, 0));
		index += sizeof( UInt32);

		// nDiffRank
		byte[] tempDiffRank = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nDiffRank", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempDiffRank, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( tempDiffRank, 0));
		index += sizeof( Int32);

		// nRankPoint
		byte[] tempRankPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nRankPoint", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempRankPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( tempRankPoint, 0));
		index += sizeof( Int32);

		return index;
	}
};

public enum eRANKTYPE
{
	eRANKTYPE_NOTHING = 0,
	eRANKTYPE_ITEM,
	eRANKTYPE_ARENA,
    eRANKTYPE_AP,
	eRANKTYPE_MAX
}

public class body_CS_RANK_SUMMARY_MYRANK_LOAD : AsPacketHeader
{
	public int eRankType;
	
	public body_CS_RANK_SUMMARY_MYRANK_LOAD(eRANKTYPE type)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_RANK_MYRANK_LOAD;
		eRankType = (int)type;
	}
};

public class body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public eRANKTYPE eRankType;
	public DateTime tRenewalTime;
	public Int32 nCurItemRankPoint;
	public sMYRANKINFO sWorldItemMyRank = new sMYRANKINFO();
	public sMYRANKINFO sFriendItemMyRank = new sMYRANKINFO();

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

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// tRenewwalTime
		byte[] renewTime = new byte[ sizeof( Int64)];
		Buffer.BlockCopy( data, index, renewTime, 0, sizeof( Int64));
		Int64 tempTime = BitConverter.ToInt64( renewTime, 0);
		index += sizeof( Int64);

		DateTime dt = new DateTime( 1970, 1, 1).AddSeconds( tempTime);
		tRenewalTime = TimeZone.CurrentTimeZone.ToLocalTime( dt);

		// nCurItemRankPoint
		byte[] rankPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCurItemRankPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankPoint, 0));
		index += sizeof( Int32);

		// sWorldItemMyRank
		index = sWorldItemMyRank.PacketBytesToClass( data, index);

		// sFriendItemMyRank
		index = sFriendItemMyRank.PacketBytesToClass( data, index);
	}
};

public class body_CS_RANK_MYRANK_LOAD : AsPacketHeader
{
	public int eRankType;
	
	public body_CS_RANK_MYRANK_LOAD(eRANKTYPE type)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_RANK_ITEM_MYRANK_LOAD;
		eRankType = (int)type;
	}
};

public class body_SC_RANK_MYRANK_LOAD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public eRANKTYPE eRankType;
	public sRANKINFO[] sMyWorldRankInfo = new sRANKINFO[ (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE];
    public int nRewardGroup;

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

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// sMyWorldRankInfo
		for( int i = 0; i < (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE; i++)
		{
			sMyWorldRankInfo[i] = new sRANKINFO();
			index = sMyWorldRankInfo[i].PacketBytesToClass( data, index);
		}
        
        // reward group
        byte[]  rewardGroup = new byte[ sizeof( Int32)];
        headerinfo = infotype.GetField("nRewardGroup", BINDING_FLAGS_PIG);
        Buffer.BlockCopy(data, index, rewardGroup, 0, sizeof(Int32));
        headerinfo.SetValue(this, BitConverter.ToInt32(rewardGroup, 0));
        index += sizeof(Int32);
	}
};

public class body_CS_RANK_TOP_LOAD : AsPacketHeader
{
	public int eRankType;
	public Int16 nPage;

	public body_CS_RANK_TOP_LOAD( eRANKTYPE type, Int16 page)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_RANK_ITEM_TOP_LOAD;
		eRankType = (int)type;
		nPage = page;
	}
};

public class body_SC_RANK_TOP_LOAD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public eRANKTYPE eRankType;
	public UInt32 nWorldItemRankMaxCount;
	public sRANKINFO[] sRankInfo = new sRANKINFO[ (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE];
    public Int32 nRewardGroup;

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

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// nWorldItemRankMaxCount
		byte[] worldRankMax = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nWorldItemRankMaxCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, worldRankMax, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( worldRankMax, 0));
		index += sizeof( UInt32);

		// sRankInfo
		for( int i = 0; i < (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE; i++)
		{
			sRankInfo[i] = new sRANKINFO();
			index = sRankInfo[i].PacketBytesToClass( data, index);
		}

        // reward group
        byte[] rewardGroup = new byte[sizeof(Int32)];
        headerinfo = infotype.GetField("nRewardGroup", BINDING_FLAGS_PIG);
        Buffer.BlockCopy(data, index, rewardGroup, 0, sizeof(Int32));
        headerinfo.SetValue(this, BitConverter.ToInt32(rewardGroup, 0));
        index += sizeof(Int32);
	}
};

public class body_CS_RANK_MYFRIEND_LOAD : AsPacketHeader
{
	public int eRankType;
	public Int16 nPage;

	public body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE type, Int16 page)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_RANK_ITEM_MYFRIEND_LOAD;
		eRankType = (int)type;
		nPage = page;
	}
};

public class body_SC_RANK_MYFRIEND_LOAD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public eRANKTYPE eRankType;
	public UInt32 nFriendItemRankMaxCount;
	public sRANKINFO[] sRankInfo = new sRANKINFO[ (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE];

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

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// nFriendItemRankMaxCount
		byte[] friendRankMax = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nFriendItemRankMaxCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, friendRankMax, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( friendRankMax, 0));
		index += sizeof( UInt32);

		// sRankInfo
		for( int i = 0; i < (int)eRANKVALUE.eRANKVALUE_MAXCOUNT_PER_PAGE; i++)
		{
			sRankInfo[i] = new sRANKINFO();
			index = sRankInfo[i].PacketBytesToClass( data, index);
		}
	}
};

public class body_SC_RANK_CHANGE_MYRANK : AsPacketHeader
{
	public eRANKTYPE eRankType;
	public sMYRANKINFO sMyRankInfo = new sMYRANKINFO();

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// sMyRankInfo
		index = sMyRankInfo.PacketBytesToClass( data, index);
	}
};

public class body_SC_RANK_CHANGE_RANKPOINT : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public eRANKTYPE eRankType;
	public Int32 nRankPoint;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] uniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, uniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( uniqKey, 0));
		index += sizeof( UInt32);

		// eRankType
		byte[] rankType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eRankType", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, rankType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( rankType, 0));
		index += sizeof( Int32);

		// nRankPoint
		byte[] RankPoint = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, RankPoint, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nRankPoint", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( RankPoint, 0));
		index += sizeof( Int32);
	}
}
#endregion

#region -Instance_Dungeon
public class body_CS_INSTANCE_CREATE : AsPacketHeader
{
	public Int32 nIndex;

	public body_CS_INSTANCE_CREATE( Int32 nIndunTableIndex)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_INSTANCE_CREATE;

		nIndex = nIndunTableIndex;
	}
}

public class body_SC_INSTANCE_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunTableIndex;

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

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);
	}
}

public class body_SC_INSTANCE_CREATE : AsPacketHeader
{
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nIndunTableIndex
		byte[] nIndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, nIndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( nIndunTableIndex, 0));
		index += sizeof( Int32);

		// nIndunDuplicationIndex
		byte[] nIndunDuplicationIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunDuplicationIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, nIndunDuplicationIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( nIndunDuplicationIndex, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_ENTER_INSTANCE : AsPacketHeader
{
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;

	public body_CS_ENTER_INSTANCE( Int32 indunTableIndex, Int32 indunDuplicationIndex)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ENTER_INSTANCE;

		nIndunTableIndex = indunTableIndex;
		nIndunDuplicationIndex = indunDuplicationIndex;
	}
}

public class body_SC_ENTER_INSTANCE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIndex;
	public Vector3 vPosition;

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

		// nMapIndex
		byte[] MapIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMapIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, MapIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( MapIndex, 0));
		index += sizeof( Int32);

		// vPosition
		byte[] position = new byte[12];
		headerinfo = infotype.GetField( "vPosition", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, position, 0, 12);
		headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( position, 0), BitConverter.ToSingle( position, 4), BitConverter.ToSingle( position, 8)));
		index += 12;
	}
}

public class body_CS_ENTER_INSTANCE_END : AsPacketHeader
{
	public body_CS_ENTER_INSTANCE_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ENTER_INSTANCE_END;
	}
}

public class body_CS_EXIT_INSTANCE : AsPacketHeader
{
	public body_CS_EXIT_INSTANCE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_EXIT_INSTANCE;
	}
}

public class body_SC_EXIT_INSTANCE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIndex;
	public Vector3 vPosition;

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

		// nMapIndex
		byte[] MapIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMapIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, MapIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( MapIndex, 0));
		index += sizeof( Int32);

		// vPosition
		byte[] position = new byte[12];
		headerinfo = infotype.GetField( "vPosition", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, position, 0, 12);
		headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( position, 0), BitConverter.ToSingle( position, 4), BitConverter.ToSingle( position, 8)));
		index += 12;
	}
}

public class body_CS_EXIT_INSTANCE_END : AsPacketHeader
{
	public body_CS_EXIT_INSTANCE_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_EXIT_INSTANCE_END;
	}
}
#endregion

#region -Pvp
public class body_CS_ARENA_MATCHING_INFO : AsPacketHeader
{
	public body_CS_ARENA_MATCHING_INFO()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ARENA_MATCHING_INFO;
	}
}

public class body_SC_ARENA_MATCHING_INFO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int16 nLimitCount1;
	public Int16 nLimitCount2;
	public Int16 nLimitCount3;
	public Int16 nLimitCount4;
	public Int32 nPvpPoint;
	public UInt32 nBattleAllCount;
	public UInt32 nVictoryCount;
	public UInt32 nLoseCount;
	public UInt32 nDrawCount;

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

		// nLimitCount1
		byte[] LimitCount1 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCount1", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCount1, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCount1, 0));
		index += sizeof( Int16);

		// nLimitCount2
		byte[] LimitCount2 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCount2", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCount2, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCount2, 0));
		index += sizeof( Int16);

		// nLimitCount3
		byte[] LimitCount3 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCount3", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCount3, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCount3, 0));
		index += sizeof( Int16);

		// nLimitCount4
		byte[] LimitCount4 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCount4", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCount4, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCount4, 0));
		index += sizeof( Int16);

		// nPvpPoint
		byte[] PvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPvpPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, PvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( PvpPoint, 0));
		index += sizeof( Int32);

		// nBattleAllCount
		byte[] BattleAllCount = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nBattleAllCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, BattleAllCount, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( BattleAllCount, 0));
		index += sizeof( UInt32);

		// nVictoryCount
		byte[] VictoryCount = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nVictoryCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, VictoryCount, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( VictoryCount, 0));
		index += sizeof( UInt32);

		// nLoseCount
		byte[] LoseCount = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nLoseCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LoseCount, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( LoseCount, 0));
		index += sizeof( UInt32);

		// nDrawCount
		byte[] DrawCount = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nDrawCount", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, DrawCount, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( DrawCount, 0));
		index += sizeof( UInt32);
	}
}

public class body_CS_ARENA_MATCHING_REQUEST : AsPacketHeader
{
	public bool bMatching;
	public Int32 nIndunTableIndex;
	public bool bTestMode;

	public body_CS_ARENA_MATCHING_REQUEST( bool matching, Int32 indunTableIndex, bool testMode)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ARENA_MATCHING_REQUEST;

		bMatching = matching;
		nIndunTableIndex = indunTableIndex;
		bTestMode = testMode;
	}
}

public class body_SC_ARENA_MATCHING_REQUEST_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

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
	}
}

public class body_CS_ARENA_MATCHING_GOINTO : AsPacketHeader
{
	public bool bEnter;

	public body_CS_ARENA_MATCHING_GOINTO( bool enter)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_ARENA_MATCHING_GOINTO;

		bEnter = enter;
	}
}

public class body_SC_ARENA_MATCHING_GOINTO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

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
	}
}

public class body_SC_ARENA_MATCHING_NOTIFY : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunTableIndex;
	public UInt32 nCharUniqKey;
	public string szCharName;

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

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// nCharUniqKey
		byte[] CharUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, CharUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( CharUniqKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_ARENA_MATCHING_LIMITCOUNT : AsPacketHeader
{
	public Int16 nLimitCnt1;
	public Int16 nLimitCnt2;
	public Int16 nLimitCnt3;
	public Int16 nLimitCnt4;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nLimitCnt1
		byte[] LimitCnt1 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCnt1", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCnt1, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCnt1, 0));
		index += sizeof( Int16);

		// nLimitCnt2
		byte[] LimitCnt2 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCnt2", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCnt2, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCnt2, 0));
		index += sizeof( Int16);

		// nLimitCnt3
		byte[] LimitCnt3 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCnt3", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCnt3, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCnt3, 0));
		index += sizeof( Int16);

		// nLimitCnt4
		byte[] LimitCnt4 = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nLimitCnt4", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LimitCnt4, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( LimitCnt4, 0));
		index += sizeof( Int16);
	}
}

public class body_SC_ARENA_INITILIZE : AsPacketHeader
{
	// nothing
}

public class body_SC_ARENA_GOGOGO : AsPacketHeader
{
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// nIndunDuplicationIndex
		byte[] IndunDuplicationIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunDuplicationIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunDuplicationIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunDuplicationIndex, 0));
		index += sizeof( Int32);
	}
}

public enum eARENAUSER_ENTERSTATE
{
	eARENAUSER_ENTERSTATE_NOTHING = 0,
	eARENAUSER_ENTERSTATE_LOADING,
	eARENAUSER_ENTERSTATE_ENTERED,
	eARENAUSER_ENTERSTATE_LOGOUT,
	eARENAUSER_ENTERSTATE_NOTENTERING,
	eARENAUSER_ENTERSTATE_MAX
}

public class sARENAUSERINFO
{
	public UInt32 nCharUniqKey;
	public string szCharName;
	public Int32 nImageTableIdx;
	public eCLASS eClass;
	public Int32 nTeamType;
	public Int16 nSlotIndex;
	public Int32 nLevel;
	public Byte nRate;
	public Int32 nPvpPoint;
	public UInt32 nYesterdayPvpRank;
	public Int32 nYesterdayPvpPoint;
	public UInt32 nYesterdayPvpRankRate;
	public Int32 nEnterState;

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		// nCharUniqKey
		byte[] tempCharKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempCharKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempCharKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);

		// nImageTableIdx
		byte[] ImageTableIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nImageTableIdx", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ImageTableIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( ImageTableIdx, 0));
		index += sizeof( Int32);

		// eClass
		byte[] cls = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, cls, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eClass", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( cls, 0));
		index += sizeof( Int32);

		// nTeamType
		byte[] TeamType = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nTeamType", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, TeamType, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( TeamType, 0));
		index += sizeof( Int32);

		// nSlotIndex
		byte[] SlotIndex = new byte[ sizeof( Int16)];
		headerinfo = infotype.GetField( "nSlotIndex", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, SlotIndex, 0, sizeof( Int16));
		headerinfo.SetValue( this, BitConverter.ToInt16( SlotIndex, 0));
		index += sizeof( Int16);

		// nLevel
		byte[] Level = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLevel", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Level, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( Level, 0));
		index += sizeof( Int32);

		// nRate
		headerinfo = infotype.GetField( "nRate", AsPacketHeader.BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[index++]);

		// nPvpPoint
		byte[] PvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPvpPoint", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, PvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( PvpPoint, 0));
		index += sizeof( Int32);

		// nYesterdayPvpRank
		byte[] YesterdayPvpRank = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nYesterdayPvpRank", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpRank, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( YesterdayPvpRank, 0));
		index += sizeof( UInt32);

		// nYesterdayPvpPoint
		byte[] YesterdayPvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nYesterdayPvpPoint", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( YesterdayPvpPoint, 0));
		index += sizeof( Int32);

		// nYesterdayPvpRankRate
		byte[] YesterdayPvpRankRate = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nYesterdayPvpRankRate", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpRankRate, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( YesterdayPvpRankRate, 0));
		index += sizeof( UInt32);

		// nEnterState
		byte[] EnterState = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nEnterState", AsPacketHeader.BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, EnterState, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( EnterState, 0));
		index += sizeof( Int32);

		return index;
	}
};

public class body_SC_ARENA_USERINFO_LIST : AsPacketHeader
{
	public sARENAUSERINFO[] sInfo = new sARENAUSERINFO[8];

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// sArenaUserInfo
		for( int i = 0; i < 8; i++)
		{
			sInfo[i] = new sARENAUSERINFO();
			index = sInfo[i].PacketBytesToClass( data, index);
		}
	}
}

public class body_SC_ARENA_ENTERUSER : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public Int32 nEnterState;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] CharUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, CharUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( CharUniqKey, 0));
		index += sizeof( UInt32);

		// nEnterState
		byte[] EnterState = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nEnterState", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, EnterState, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( EnterState, 0));
		index += sizeof( Int32);
	}
}

public enum ePVPRESULT
{
	ePVPRESULT_NOTHING = 0,
	ePVPRESULT_WIN,
	ePVPRESULT_LOSE,
	ePVPRESULT_DRAW,
	ePVPRESULT_ENTERSTATE_MAX
}

public class body_SC_ARENA_REWARDINFO : AsPacketHeader
{
	public Int32 nPvpResult;
	public Int32 nPvpPoint;
	public Int32 nAddPvpPoint;
	public Int32 nExp;
	public Int32 nAddExp;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nPvpResult
		byte[] PvpResult = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPvpResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, PvpResult, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( PvpResult, 0));
		index += sizeof( Int32);

		// nPvpPoint
		byte[] PvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPvpPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, PvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( PvpPoint, 0));
		index += sizeof( Int32);

		// nAddPvpPoint
		byte[] AddPvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nAddPvpPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, AddPvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( AddPvpPoint, 0));
		index += sizeof( Int32);

		// nExp
		byte[] Exp = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nExp", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Exp, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( Exp, 0));
		index += sizeof( Int32);

		// nAddExp
		byte[] AddExp = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nAddExp", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, AddExp, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( AddExp, 0));
		index += sizeof( Int32);
	}
}

public class body_SC_ARENA_READY : AsPacketHeader
{
	public Int64 nReadyTime;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nReadyTime
		byte[] readyTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nReadyTime", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, readyTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( readyTime, 0));
		index += sizeof( Int64);
	}
}

public class body_SC_ARENA_START : AsPacketHeader
{
	public Int64 nStartTime;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nStartTime
		byte[] startTime = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nStartTime", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, startTime, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( startTime, 0));
		index += sizeof( Int64);
	}
}
#endregion

#region -AccountGender
public class body_CS_USERGENDER_SET : AsPacketHeader
{
	public eGENDER eGender;

	public body_CS_USERGENDER_SET( eGENDER gender)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_USERGENDER_SET;

		eGender = gender;
	}
}

public class body_SC_USERGENDER_SET_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public eGENDER eGender;
}

public class body_SC_USERGENDER_NOTIFY : AsPacketHeader
{
	public ushort nSessionIdx;
	public eGENDER eGender;
}
#endregion

#region -HackCheck
public class body_CS_HACK_INFO : AsPacketHeader
{
	public byte[] strHackInfo = new byte[ AsGameDefine.eHACK_INFO + 1];
	
	public body_CS_HACK_INFO( string hackName)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS2;
		Protocol = (byte)PROTOCOL_CS_2.CS_HACK_INFO;
		
		byte[] tempName = UTF8Encoding.UTF8.GetBytes( hackName);
		Buffer.BlockCopy( tempName, 0, strHackInfo, 0, tempName.Length);
	}
}
#endregion
