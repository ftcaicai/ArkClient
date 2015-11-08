#define _USE_PARTY_WARP
//#define USE_OLD_COSTUME
using System;
using System.Reflection;
using UnityEngine;
using System.Text;

enum PROTOCOL_GAME : byte
{
	CG_TYPE_TEST,// = _CATEGORY_CG,
	GC_TYPE_TEST_ECHO,

	CG_LOGIN_POSSIBLE,
	GC_LOGIN_POSSIBLE_RESULT,

	CG_LOGIN,
	CG_WEMELOGIN,
	GC_LOGIN_RESULT,
	GC_ACCOUNT_DUPLICATE,

	CG_CHAR_LOAD,
	GC_CHAR_LOAD_RESULT,
	CG_CHAR_CREATE,
	GC_CHAR_CREATE_RESULT,
	CG_CHAR_DELETE,
	GC_CHAR_DELETE_RESULT,
	CG_CHAR_SELECT,
	GC_CHAR_SELECT_RESULT,
	CG_CHAR_SLOT_ADD,
	GC_CHAR_SLOT_ADD_RESULT,

	GC_CHANNEL_AUTO_SELECT,
	CG_CHANNEL_LIST,
	GC_CHANNEL_LIST,
	CG_CHANNEL_SELECT,
	GC_CHANNEL_SELECT_RESULT,
#if _USE_PARTY_WARP	
	GC_CHANNEL_INFO,
#endif	
	CG_ENTER_WORLD,
	GC_ENTER_WORLD_RESULT,
	CG_ENTER_WORLD_END,

	CG_RETURN_CHARSELECT,
	GC_RETURN_CHARSELECT_RESULT,

	CG_CHEAT,
	GC_CHEAT_RESULT,
	GC_CHEAT_HIDE_NOTIFY,

	CG_RECOMMEND,
	GC_RECOMMEND,
}

public class body_CG_LOGIN_POSSIBLE : AsPacketHeader
{
	public body_CG_LOGIN_POSSIBLE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_LOGIN_POSSIBLE;
	}
};

public class body_GC_LOGIN_POSSIBLE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public int nOrder;//대기열 순서
	public int nWaitSec;//대기 시간
};

public class AS_CG_LOGIN : AsPacketHeader
{
	public UInt32 nUserUniqKey;	// �������� �־����� ����ũ�� Ű ��
	public UInt32 nUserSessionKey;	// �α��μ����� �α��ν� ������ ����Ű
	public UInt32 nVersion;

	public AS_CG_LOGIN( UInt32 uniqueKey, UInt32 sessionKey, UInt32 version)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_LOGIN;
		nUserUniqKey = uniqueKey;
		nUserSessionKey = sessionKey;
		nVersion = version;
	}
}

public class AS_GC_LOGIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIdx;
	public bool bIsGuest;
	#region -AccountGender
	public eGENDER eUserGender;
	#endregion
	#region -GMMark
	public bool bIsGM;
	#endregion
}

public class AS_CG_CHAR_LOAD : AsPacketHeader
{
	public AS_CG_CHAR_LOAD()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_LOAD;
	}
}

public class body1_GC_CHAR_LOAD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public bool bPossibleCharCreate;
	public Int32 nEnableSlotCnt;
	public Int32 nLastSelectCharSlot;
	public Int64 nMiracle;
	public UInt32 nPrivateShopOpenCharUniqKey;
//	public Int64 nPrivateShopMaxOpenTime;
//	public Int64 nPrivateShopOpenTime;
	public Int64 nPrivateShopRemainingTime;
	public Int32 nPrivateShopCreateItemSlot;

	public Int32 nCharCnt;
	public sCHARVIEWDATA[] sCharView = null;

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

		// bPossibleCharCreate
		byte[] create = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bPossibleCharCreate", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, create, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( create, 0));
		index += sizeof( bool);

		// nEnableSlotCnt
		byte[] enableSlotCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nEnableSlotCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, enableSlotCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( enableSlotCnt, 0));
		index += sizeof( Int32);

		// nLastSelectCharSlot
		byte[] latestCharSlot = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLastSelectCharSlot", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, latestCharSlot, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( latestCharSlot, 0));
		index += sizeof( Int32);

		// nMiracle
		byte[] sphere = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nMiracle", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, sphere, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( sphere, 0));
		index += sizeof( Int64);

		// nPrivateShopOpenCharUniqKey
		byte[] tempNewMember = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nPrivateShopOpenCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempNewMember, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( tempNewMember, 0));
		index += sizeof( UInt32);

		// nPrivateShopRemainingTime
		tempNewMember = new byte[ sizeof( Int64)];
		headerinfo = infotype.GetField( "nPrivateShopRemainingTime", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempNewMember, 0, sizeof( Int64));
		headerinfo.SetValue( this, BitConverter.ToInt64( tempNewMember, 0));
		index += sizeof( Int64);

		// nPrivateShopCreateItemSlot
		tempNewMember = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPrivateShopCreateItemSlot", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, tempNewMember, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( tempNewMember, 0));
		index += sizeof( Int32);

		// nCharCnt
		byte[] characterCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCharCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, characterCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( characterCount, 0));
		index += sizeof( Int32);

		if( 0 < nCharCnt)
		{
			sCharView = new sCHARVIEWDATA[ nCharCnt];
			for( int i = 0; i < nCharCnt; i++)
			{
				sCharView[i] = new sCHARVIEWDATA();

				byte[] tmpData = new byte[ sCHARVIEWDATA.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				sCharView[i].ByteArrayToClass( tmpData);
				index += sCHARVIEWDATA.size;
			}
		}
	}
}

//customize
public class AS_CG_CHAR_CREATE : AsPacketHeader
{
	public AS_CG_CHAR_CREATE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_CREATE;
	}
}

public class AS_GC_CHAR_CREATE_RESULT : AsPacketHeader
{
	public AS_GC_CHAR_CREATE_RESULT()
	{
	}
}

public class AS_CG_CHAR_DELETE : AsPacketHeader
{
	public AS_CG_CHAR_DELETE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_DELETE;
	}
}

public class AS_GC_CHAR_DELETE_RESULT : AsPacketHeader
{
	public AS_GC_CHAR_DELETE_RESULT()
	{
	}
}
//~customize

public class sQUICKSLOT : AsPacketHeader
{
	public static int size = 8;
	public Int32 eType;
	public Int32 nValue;
}

public class body_GC_CHAR_SELECT_RESULT_2 : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];//29
	public byte[] szGuildName = new byte[AsGameDefine.MAX_GUILD_NAME_LEN + 1];
	public eGENDER eGender;
	public Int32 eRace;
	public Int32 eClass;
	public Int32 nHair;
	public Int32 nHairColor;//49
	public Int32 nSubTitleTableIdx;
	public bool bSubTitleHide;
	public Int32 nLevel;
	public Int32 nTotExp;//57
	public UInt64 nGold;//65
	public Int32 nSocialPoint;
	//public UInt64 nSphere;
	public Single nHpCur;
	public Single nMpCur;//73
	public Int32 fAttDistance;//77
	public sCLIENTSTATUS sDefaultStatus;
	public sCLIENTSTATUS sFinalStatus;//277

	public static int size
	{
		get
		{
			return ( sizeof( UInt32) + ( sizeof( byte) * ( AsGameDefine.MAX_NAME_LEN + 1)) + ( sizeof( byte) * ( AsGameDefine.MAX_GUILD_NAME_LEN + 1)) + sizeof( eGENDER)
				+ sizeof( bool) + ( sizeof( Int32) * 8) + ( sizeof( UInt64)*1) + sizeof( Int32) + ( sizeof( Single) * 2) + ( sCLIENTSTATUS.size * 2));
		}
	}
}

public class AS_GC_CHAR_LOAD_RESULT_EMPTY : AsPacketHeader
{
	public AS_GC_CHAR_LOAD_RESULT_EMPTY()
	{
	}
}

public class body_GC_CHAR_SELECT_RESULT_1 : AsPacketHeader
{
	public eRESULTCODE eResult;

	public body_GC_CHAR_SELECT_RESULT_2 body;
#if USE_OLD_COSTUME
	public bool bCostumeOnOff;
#else 
	public Int32 bCostumeOnOff;
#endif
	public sITEMVIEW[] sNormalItemVeiw = null;
	public sITEMVIEW[] sCosItemView = null;

	public Int32 nChannel;
	public UInt32 nCondition;
	//public Int32 nPvpPoint;
	public UInt32 nYesterdayPvpRank;
	public Int32 nYesterdayPvpPoint;
	public UInt32 nYesterdayPvpRankRate;
	public Int32 nRankPoint;
	
	public Int32 nLevelComplete;

	public byte nFreeGachaPoint;

	public Int32 nResurrectionCnt;

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

		body = new body_GC_CHAR_SELECT_RESULT_2();
		byte[] tmpData = new byte[body_GC_CHAR_SELECT_RESULT_2.size];
		Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
		index += body_GC_CHAR_SELECT_RESULT_2.size;
		body.ByteArrayToClass( tmpData);

#if USE_OLD_COSTUME
		// bCostumeOnOff
		byte[] ingame = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bCostumeOnOff", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ingame, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( ingame, 0));
		index += sizeof( bool);
#else
		// bCostumeOnOff
		byte[] ingame = new byte[ sizeof(Int32)];
		headerinfo = infotype.GetField( "bCostumeOnOff", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ingame, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToInt32( ingame, 0));
		index += sizeof( Int32);
#endif

		sNormalItemVeiw = new sITEMVIEW[AsGameDefine.ITEM_SLOT_VIEW_COUNT];
		for ( int i = 0; i < AsGameDefine.ITEM_SLOT_VIEW_COUNT; i++)
		{
			sNormalItemVeiw[i] = new sITEMVIEW();
			
			tmpData = new byte[sITEMVIEW.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			sNormalItemVeiw[i].ByteArrayToClass( tmpData);
			index += sITEMVIEW.size;
		}

		sCosItemView = new sITEMVIEW[AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT];
		for ( int i = 0; i < AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT; i++)
		{
			sCosItemView[i] = new sITEMVIEW();

			tmpData = new byte[sITEMVIEW.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			sCosItemView[i].ByteArrayToClass( tmpData);
			index += sITEMVIEW.size;
		}

		byte[] channel = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nChannel", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, channel, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( channel, 0));
		index += sizeof( Int32);

		byte[] condition = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCondition", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, condition, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( condition, 0));
		index += sizeof( UInt32);

//		byte[] PvpPoint = new byte[ sizeof( Int32)];
//		headerinfo = infotype.GetField( "nPvpPoint", BINDING_FLAGS_PIG);
//		Buffer.BlockCopy( data, index, PvpPoint, 0, sizeof( Int32));
//		headerinfo.SetValue( this, BitConverter.ToInt32( PvpPoint, 0));
//		index += sizeof( Int32);

		byte[] YesterdayPvpRank = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nYesterdayPvpRank", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpRank, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( YesterdayPvpRank, 0));
		index += sizeof( UInt32);

		byte[] YesterdayPvpPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nYesterdayPvpPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( YesterdayPvpPoint, 0));
		index += sizeof( Int32);

		byte[] YesterdayPvpRankRate = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nYesterdayPvpRankRate", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, YesterdayPvpRankRate, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( YesterdayPvpRankRate, 0));
		index += sizeof( UInt32);

		byte[] RankPoint = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nRankPoint", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, RankPoint, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( RankPoint, 0));
		index += sizeof( Int32);
		
		byte[] LevelComplete = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLevelComplete", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, LevelComplete, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( LevelComplete, 0));
		index += sizeof( Int32);

		byte freeGacha = 0;
		headerinfo = infotype.GetField("nFreeGachaPoint", BINDING_FLAGS_PIG);
		freeGacha = data[index];
		headerinfo.SetValue(this, freeGacha);
		index += sizeof(byte);

		byte[] ResurrectionCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nResurrectionCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ResurrectionCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( ResurrectionCnt, 0));
		index += sizeof( Int32);
	}
}


public class AS_CG_CHAR_SELECT : AsPacketHeader
{
	public Int32 nCharSlot;

	public AS_CG_CHAR_SELECT( Int32 slot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_SELECT;
		nCharSlot = slot;
	}
}

public class body_CG_CHAR_SLOT_ADD : AsPacketHeader
{
	public body_CG_CHAR_SLOT_ADD()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_SLOT_ADD;
	}
}

public class body_GC_CHAR_SLOT_ADD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nEnableSlotCnt;
}

#region -ChannelList_Struct
public class body1_GC_CHANNEL_LIST : AsPacketHeader
{
	public bool bIngame;
	public Int32 nCnt;
	public body2_GC_CHANNEL_LIST[] channels = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// bIngame
		byte[] ingame = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bIngame", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ingame, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( ingame, 0));
		index += sizeof( bool);

		// nCnt
		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);

		if( 0 >= nCnt)
			AsUtil.ShutDown( "body1_GC_CHANNEL_LIST");

		channels = new body2_GC_CHANNEL_LIST[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			channels[i] = new body2_GC_CHANNEL_LIST();
//			tmpData = new byte[ body2_GC_CHANNEL_LIST.size];
//			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
//			channels[i].ByteArrayToClass( tmpData);
//			index += body2_GC_CHANNEL_LIST.size;
			index = channels[i].PacketBytesToClass( data, index);
		}
	}
}

public class body2_GC_CHANNEL_LIST : AsPacketHeader
{
	public Int32 nChannel;
	public string szChannelName;
	public Int32 nMinLevel;
	public Int32 nMaxLevel;
	public eCONFUSIONTYPE eConfusion;
	public bool bIsPrivateShop; // true: ChargeReduction
	public Int32 nPrivateStore_Charge;
	public Int32 nPrivateStore_ChargeReduction;

	public static int size		{ get	{ return ( 3 * sizeof( Int32)) + ( AsGameDefine.eCHANNELNAME + 1); } }

	public int PacketBytesToClass( byte[] data, int index)
	{
		Type infotype = this.GetType();

		// nChannel
		byte[] channel = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, channel, 0, sizeof( Int32));
		FieldInfo headerinfo = infotype.GetField( "nChannel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( channel, 0));
		index += sizeof( Int32);

		// szChannelName
		byte[] name = new byte[ AsGameDefine.eCHANNELNAME + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.eCHANNELNAME + 1);
		szChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.eCHANNELNAME + 1);

		// nMinLevel
		byte[] minLevel = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, minLevel, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nMinLevel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( minLevel, 0));
		index += sizeof( int);

		// nMaxLevel
		byte[] maxLevel = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, maxLevel, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nMaxLevel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( maxLevel, 0));
		index += sizeof( int);

		// eConfusion
		byte[] confusion = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, confusion, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "eConfusion", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( confusion, 0));
		index += sizeof( int);

		// bIsPrivateShop
		byte[] IsPrivateShop = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bIsPrivateShop", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IsPrivateShop, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( IsPrivateShop, 0));
		index += sizeof( bool);

		// nPrivateStore_Charge
		byte[] PrivateStore_Charge = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, PrivateStore_Charge, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nPrivateStore_Charge", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( PrivateStore_Charge, 0));
		index += sizeof( int);

		// nPrivateStore_ChargeReduction
		byte[] PrivateStore_ChargeReduction = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, PrivateStore_ChargeReduction, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nPrivateStore_ChargeReduction", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( PrivateStore_ChargeReduction, 0));
		index += sizeof( int);

		return index;
	}
}

public class body_GC_CHANNEL_SELECT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nChannel;
}

public class body_GC_CHANNEL_INFO : AsPacketHeader
{	
	public Int32 nChannel;
	public string szChannelName;
	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nChannel
		byte[] channel = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, channel, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nChannel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( channel, 0));
		index += sizeof( Int32);

		// szChannelName
		byte[] name = new byte[ AsGameDefine.eCHANNELNAME + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.eCHANNELNAME + 1);
		szChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.eCHANNELNAME + 1);

	}
}
#endregion

public class body_CG_CHAR_CREATE : AsPacketHeader
{
	public Int32 nSlot;
	public byte[] szCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
	public eRACE eRace;
	public eCLASS eClass;
	public eGENDER eGender;
	public Int32 nHairIndex;
	public Int32 nHairColor;
	public Int32 nBodyIndex;
	public Int32 nPointIndex;
	public Int32 nGloveIndex;

	public body_CG_CHAR_CREATE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_CREATE;
	}
}

public class sCHARVIEWDATA : AsPacketHeader
{
	public Int32 nCharSlot;
	public UInt32 nCharUniqKey;
	public byte[] szCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
	public eGENDER eGender;
	public eRACE eRace;
	public eCLASS eClass;
	public Int32 nHair;
	public Int32 nHairColor;
	public Int32 nLevel;
	//public sITEMVIEW[] sNormalItemVeiw = new sITEMVIEW[AsGameDefine.ITEM_SLOT_VIEW_COUNT];
	//public sITEMVIEW[] sCosItemView = new sITEMVIEW[AsGameDefine.ITEM_SLOT_VIEW_COUNT];

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


	public bool bDelete;
	public Int32 nRemainTime;

	public static int size
	{
		get
		{
			return sizeof( Int32) + sizeof( UInt32) + ( sizeof( byte) * ( AsGameDefine.MAX_NAME_LEN + 1)) + sizeof( eGENDER) + sizeof( eRACE)
				+ sizeof( eCLASS) + sizeof( Int32) + sizeof( Int32) + sizeof( Int32) + ( sITEMVIEW.size * AsGameDefine.ITEM_SLOT_VIEW_COUNT)
					+ ( sITEMVIEW.size * AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT)
					+ sizeof( bool) + sizeof( Int32)
#if USE_OLD_COSTUME
					+ 1
#else
					+sizeof( UInt32)
#endif					
					;
		}
	}

	public void ShowInfo()
	{
		Debug.Log( "ApplyInGameDataOnSelectInfo: nCharUniqKey = " + nCharUniqKey);
		Debug.Log( "ApplyInGameDataOnSelectInfo: sNormalItemVeiw_1 = " + sNormalItemVeiw_1.nItemTableIdx +
			", sNormalItemVeiw_2 = " + sNormalItemVeiw_2.nItemTableIdx +
			", sNormalItemVeiw_3 = " + sNormalItemVeiw_3.nItemTableIdx +
			", sNormalItemVeiw_4 = " + sNormalItemVeiw_4.nItemTableIdx +
			", sNormalItemVeiw_5 = " + sNormalItemVeiw_5.nItemTableIdx);
		Debug.Log( "ApplyInGameDataOnSelectInfo: data.sCosItemView_1 = " + sCosItemView_1.nItemTableIdx +
			", sCosItemView_2 = " + sCosItemView_2.nItemTableIdx +
			", sCosItemView_3 = " + sCosItemView_3.nItemTableIdx +
			", sCosItemView_4 = " + sCosItemView_4.nItemTableIdx +
			", sCosItemView_5 = " + sCosItemView_5.nItemTableIdx);
	}
}

public class sCLIENTSTATUS : AsPacketHeader
{
	public static int size = 60 + ( sizeof( Single) * 2);

	public Single fHPMax;
	public Single fMPMax;//8

	public Int32 nPhysicDmg_Min;
	public Int32 nPhysicDmg_Max;//16

	public Int32 nMagicDmg_Min;
	public Int32 nMagicDmg_Max;//24

	public Int32 nPhysic_Def;
	public Single fPhysic_Dmg_Dec;
	public Int32 nMagic_Def;//32
	public Single fMagic_Dmg_Dec;

	public Int32 nCriticalProb;
	public Int32 nAccuracyProb;
	public Int32 nDodgeProb;//44

	public Int32 nMoveSpeed;
	public Int32 nAtkSpeed;//52

	public Single nHPRegen;
	public Single nMPRegen;//60

	//public Int32[]				nAttr_Dmg = new Int32[( int)eATTRIBUTE_TYPE.eATTRIBUTE_MAX];
	//public Int32[]				nAttr_Resist = new Int32[( int)eATTRIBUTE_TYPE.eATTRIBUTE_MAX];//100
}

public class body_GC_CHAR_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public sCHARVIEWDATA sCharView = new sCHARVIEWDATA();

	public bool bRecommendEvent;// 추천인 이벤트.
	public Int32 nItemTableIdx;

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

		byte[] tmpData = new byte[ sCHARVIEWDATA.size];
		Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
		sCharView.ByteArrayToClass( tmpData);
		index += sCHARVIEWDATA.size;

		// bRecommendEvent
		byte[] recommendEvent = new byte[ sizeof( bool)];
		Buffer.BlockCopy( data, index, recommendEvent, 0, sizeof( bool));
		headerinfo = infotype.GetField( "bRecommendEvent", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToBoolean( recommendEvent, 0));
		index += sizeof( bool);

		// nItemTableIdx
		byte[] itemTableIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nItemTableIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, itemTableIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( itemTableIdx, 0));
		index += sizeof( Int32);
	}
}

public class body_CG_CHAR_DELETE : AsPacketHeader
{
	public byte nActionType;
	public Int32 nCharSlot;

	public body_CG_CHAR_DELETE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHAR_DELETE;
	}
}

public class body_GC_CHAR_DELETE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte nActionType;
	public Int32 nCharSlot;
	public bool bDelete;
	public Int32 nRemainTime;
	public Int32 nMonthDeleteCount;
}

public class body_GC_CHANNEL_AUTO_SELECT : AsPacketHeader
{
	public Int32 nChannel;
	public string szChannelName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nChannel
		byte[] channel = new byte[ sizeof( Int32)];
		Buffer.BlockCopy( data, index, channel, 0, sizeof( Int32));
		headerinfo = infotype.GetField( "nChannel", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToInt32( channel, 0));
		index += sizeof( Int32);

		// szChannelName
		byte[] name = new byte[ AsGameDefine.eCHANNELNAME + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.eCHANNELNAME + 1);
		szChannelName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.eCHANNELNAME + 1);
	}
}

public class body_CG_CHANNEL_LIST : AsPacketHeader
{
	public bool bIngame;

	public body_CG_CHANNEL_LIST( bool inGame)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHANNEL_LIST;

		bIngame = inGame;
	}
}

public class body_CG_CHANNEL_SELECT : AsPacketHeader
{
	public Int32 nChannel;

	public body_CG_CHANNEL_SELECT( Int32 channel)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHANNEL_SELECT;

		nChannel = channel;
	}
}

public class AS_CG_ENTER_WORLD : AsPacketHeader
{
	public AS_CG_ENTER_WORLD()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_ENTER_WORLD;
	}
}

public class AS_CG_ENTER_WORLD_END : AsPacketHeader
{
	public AS_CG_ENTER_WORLD_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_ENTER_WORLD_END;
	}
}

public class AS_GC_ENTER_WORLD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}

public class AS_CG_RETURN_CHARSELECT : AsPacketHeader
{
	public AS_CG_RETURN_CHARSELECT()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_RETURN_CHARSELECT;
	}
}

public class GC_RETURN_CHARSELECT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}

public class body_CG_CHEAT : AsPacketHeader
{
	public Int32 _nType;
	public byte[] _szCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
	public Int32 _nValue_1;
	public Int32 _nValue_2;
	public Int32 _nValue_3;
	public float _fValue_1;
	public float _fValue_2;
	public float _fValue_3;

	public body_CG_CHEAT( Int32 nType, string strCharName, Int32 n1, Int32 n2, Int32 n3, float f1, float f2, float f3)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_CHEAT;

		_nType = nType;

		byte[] ChatNameBytes = Encoding.UTF8.GetBytes( strCharName);
		if( ChatNameBytes.Length <= ( AsGameDefine.MAX_NAME_LEN + 1))
		{
			for( int i = 0; i < ChatNameBytes.Length; i++)
				_szCharName[i] = ChatNameBytes[i];
		}
		
		_nValue_1 = n1;
		_nValue_2 = n2;
		_nValue_3 = n3;
		_fValue_1 = f1;
		_fValue_2 = f2;
		_fValue_3 = f3;
	}
}

public class body_GC_CHEAT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nType;
	public Int32 nValue_1;
}

public class body_GC_CHEAT_HIDE_NOTIFY :AsPacketHeader
{
	public UInt16		nSessionIdx;
	public bool			bHide;
}

#region -WEME Certify
public class body_CG_WEMELOGIN : AsPacketHeader
{
	public UInt32 nUserUniqKey;
	public byte[] strAccessToken = new byte[ AsGameDefine.eACCESSTOKEN + 1];
	public UInt32 nVersion;
	public bool bIsGuest;

	public body_CG_WEMELOGIN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_WEMELOGIN;
	}
}
#endregion

// In Dungeon

#if USE_INSTANCE_SERVER
public class AS_CG_INSTANCE_CREATE : AsPacketHeader
{
	public Int32 nInstanceTableIdx = 0;
	public Int32 nDifficulty = 0;
	public AS_CG_INSTANCE_CREATE( Int32 _nInstanceTableIdx, Int32 _nDifficulty)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_INSTANCE_CREATE;
	
		nInstanceTableIdx = _nInstanceTableIdx;
		nDifficulty = _nDifficulty;
	}
}


public class AS_GC_INSTANCE_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nRoomIdx;
	public Int32 nEnterKey;

	public byte[] szIpaddress = new byte[AsNetworkDefine.eIPADDRESS + 1];
	public UInt16 nPort;
}

// out Dungeon


public class AS_CG_RETURN_WORLD : AsPacketHeader
{
	public AS_CG_RETURN_WORLD()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_RETURN_WORLD;
	}
}
#endif


public class body_CG_RECOMMEND : AsPacketHeader
{
	public bool bRecommend;					// 추천을 할거냐 말꺼냐.
	public byte[] szRecommendCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];// eCHARNAME = 24.


	//public UInt32 nUserUniqKey; Old Recommend
	public body_CG_RECOMMEND( bool flag, string strCharName)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CG;
		Protocol = (byte)PROTOCOL_GAME.CG_RECOMMEND;

		bRecommend = flag;

		byte[] szName = System.Text.UTF8Encoding.UTF8.GetBytes( strCharName.ToCharArray(), 0, strCharName.Length);
		Array.Clear( szRecommendCharName, 0, ( int)AsGameDefine.MAX_NAME_LEN + 1);

		int length = ( AsGameDefine.MAX_NAME_LEN <szName.Length) ? AsGameDefine.MAX_NAME_LEN : szName.Length;
		System.Buffer.BlockCopy( szName, 0,szRecommendCharName, 0, length);
	}
}

public class body_GC_RECOMMEND : AsPacketHeader
{
	public eRESULTCODE eResult;
}