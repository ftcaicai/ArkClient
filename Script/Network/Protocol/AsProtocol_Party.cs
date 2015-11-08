#define _USE_PARTY_WARP
using System;
using System.Reflection;
using UnityEngine;
using System.Text;
using System.Diagnostics;


public enum eEQUIPITEM_GRADE
{
	eEQUIPITEM_GRADE_NOTHING = 0,

	eEQUIPITEM_GRADE_NORMAL,
	eEQUIPITEM_GRADE_MAGIC,
	eEQUIPITEM_GRADE_RARE,
	eEQUIPITEM_GRADE_EPIC,
	eEQUIPITEM_GRADE_ARK,

	eEQUIPITEM_GRADE_MAX
};


//------------------------------------------------------------------------------------------------
//party
//------------------------------------------------------------------------------------------------
public enum ePARTYPURPOSE
{
	ePARTYPURPOSE_NOTHING = 0,

	ePARTYPURPOSE_HUNT,
	ePARTYPURPOSE_QUEST,

	ePARTYPURPOSE_MAX
};

public enum ePARTYITEMDIVIDE
{
	ePARTYITEMDIVIDE_NOTHING = 0,

	ePARTYITEMDIVIDE_FREE,
	ePARTYITEMDIVIDE_ORDER,
	ePARTYITEMDIVIDE_EQUIP,

	ePARTYITEMDIVIDE_MAX
};

public enum ePARTYJOINTYPE
{
	ePARTYJOINTYPE_NOTHING = 0,

	ePARTYJOINTYPE_NORMAL, //List Click
	ePARTYJOINTYPE_ACCEPT,

	ePARTYJOINTYPE_REFUSE,
	ePARTYJOINTYPE_PLAYING, //Invitation of other users already in progress

	ePARTYJOINTYPE_MAX
};


public class AS_CS_PARTY_LIST : AsPacketHeader
{
	public Int32 nMapIdx;	//<-- 2014.01.10.

	public AS_CS_PARTY_LIST( int _MapIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_LIST;
		nMapIdx = _MapIdx;
	}
}


public class AS_CS_PARTY_DETAIL_INFO : AsPacketHeader
{
	public Int32 nPartyIdx;

	public AS_CS_PARTY_DETAIL_INFO( Int32 _nPartyIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_DETAIL_INFO;
	
		nPartyIdx = _nPartyIdx;
	}
}


public class AS_CS_PARTY_CREATE : AsPacketHeader
{
	public Int32 nMapIdx;	//<-- 2014.01.10.
	public byte[] szPartyName = new byte[AsGameDefine.MAX_PARTYNAME_LEN + 1];
	public bool bPublic;
	public int ePurpose;
	public int eDivide;
	public int eGrade;
	public int nMaxUser;

	public AS_CS_PARTY_CREATE( int _MapIdx, sPARTYOPTION stOption)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_CREATE;

		nMapIdx = _MapIdx;
		szPartyName = stOption.szPartyName;
		bPublic	 = stOption.bPublic;
		nMaxUser = stOption.nMaxUser;

		ePurpose = (int)stOption.ePurpose;
		eDivide = (int)stOption.eDivide;
		eGrade = (int)stOption.eGrade;
	}
}


public class AS_CS_PARTY_CAPTAIN : AsPacketHeader
{
	public uint nCharUniqKey;

	public AS_CS_PARTY_CAPTAIN( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_CAPTAIN;
	
		nCharUniqKey = _CharUniqKey;
	}
}


public class AS_CS_PARTY_SETTING : AsPacketHeader
{
	public Int32 nMapIdx;//update( n), no update( 0)	//<-- 2014.01.10.
	public byte[] szPartyName = new byte[AsGameDefine.MAX_PARTYNAME_LEN + 1];
	public bool bPublic;
	public int ePurpose;
	public int eDivide;
	public int eGrade;
	public int nMaxUser;

	public AS_CS_PARTY_SETTING( int _MapIdx, sPARTYOPTION stOption)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_SETTING;

		if( AsPartyManager.Instance.PartyMapIdx == AsPartyManager.Instance.BeforeMapIdx)
			nMapIdx = 0;
		else
			nMapIdx = _MapIdx;

		szPartyName = stOption.szPartyName;
		bPublic	 = stOption.bPublic;
		nMaxUser = stOption.nMaxUser;

		ePurpose = (int)stOption.ePurpose;
		eDivide = (int)stOption.eDivide;
		eGrade = (int)stOption.eGrade;
	}
}


public class AS_CS_PARTY_INVITE : AsPacketHeader
{
	public UInt16 nSessionIdx;
	public uint nCharUniqKey;

	public AS_CS_PARTY_INVITE( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_INVITE;
	
		nSessionIdx = _SessionIdx;
		nCharUniqKey = _CharUniqKey;
	}
}


public class AS_CS_PARTY_JOIN : AsPacketHeader
{
	public int eJoinType; //ePARTYJOINTYPE
	public Int32 nPartyIdx;
	public uint nCharUniqKey; //Invitation Info 0:empty

	public AS_CS_PARTY_JOIN( int _nJoinType, Int32 _nPartyIdx, uint _nCharUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_JOIN;

		eJoinType = _nJoinType;
		nPartyIdx = _nPartyIdx;
		nCharUniqKey = _nCharUniqKey;
	}
}


public class AS_CS_PARTY_EXIT : AsPacketHeader
{
	public AS_CS_PARTY_EXIT()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_EXIT;
	}
}


public class AS_CS_PARTY_BANNED_EXIT : AsPacketHeader
{
	public uint nCharUniqKey;

	public AS_CS_PARTY_BANNED_EXIT( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_BANNED_EXIT;

		nCharUniqKey = _CharUniqKey;
	}
}


public class AS_CS_PARTY_DICE_SHAKE : AsPacketHeader
{
	public bool bAccept;//Dice Participation( true: Participation, false:Give Up)
	public int nDropItemIdx;

	public AS_CS_PARTY_DICE_SHAKE( bool _bAccept, int _nDropItemIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_DICE_SHAKE;

		bAccept = _bAccept;
		nDropItemIdx = _nDropItemIdx;
	}
}


public class sPARTYOPTION : AsPacketHeader
{
	public byte[] szPartyName = new byte[AsGameDefine.MAX_PARTYNAME_LEN + 1];
	public bool bPublic;
	public int ePurpose;
	public int eDivide;
	public int eGrade;
	public int nMaxUser;

	public static int size
	{
		get
		{
			return ( sizeof( byte) * ( AsGameDefine.MAX_PARTYNAME_LEN + 1)) + sizeof( bool) + sizeof( ePARTYPURPOSE) + sizeof( ePARTYITEMDIVIDE)
					+ sizeof( eEQUIPITEM_GRADE) + sizeof (int) ;
		}
	}
};


public class sPARTYLIST : AsPacketHeader
{
	public Int32 nPartyIdx;
	public Int32 nMapIdx;	//<-- 2014.01.10.
	public sPARTYOPTION sOption;
	public Int32 nLevel;
	public Int32 nUserCnt;

	public static int size
	{
		get
		{
			return sizeof( Int32) + sizeof( Int32) + sPARTYOPTION.size + sizeof( Int32) + sizeof( Int32);
		}
	}
}


public class AS_SC_PARTY_LIST : AsPacketHeader
{
	public Int32 nCnt;
	public sPARTYLIST[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCnt
		byte[] nCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, nCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( nCount, 0));
		index += sizeof( Int32);

		if( 0 < nCnt)
		{
			body = new sPARTYLIST[nCnt];
			for( int i = 0; i < nCnt; i++)
			{
				body[i] = new sPARTYLIST();
	
				byte[] tmpData = new byte[sPARTYLIST.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += sPARTYLIST.size;
			}
		}
	}
}


public class sPARTYDETAILINFO : AsPacketHeader
{
	public bool bConnect;//( true : log In , false : log out)
	public uint nCharUniqKey;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public Int32 eRace;
	public Int32 eClass;
	public Int32 nLevel;
	public Int32 nImageTableIdx;

	public static int size
	{
		get
		{
			return sizeof( bool) + sizeof( uint) + ( sizeof( byte) * ( AsGameDefine.MAX_NAME_LEN + 1))
					+ sizeof( Int32) + sizeof( Int32) + sizeof( Int32) + sizeof( Int32);
		}
	}
}


public class AS_SC_PARTY_DETAIL_INFO : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nPartyIdx;
	public Int32 nMapIdx;	//<-- 2014.01.10.
	public sPARTYOPTION sOption;
	public Int32 nUserCnt;
	public sPARTYDETAILINFO[] body = null;

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

		// nPartyIdx
		byte[] partyIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPartyIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, partyIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( partyIdx, 0));
		index += sizeof( Int32);

		// nMapIdx
		byte[] mapIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMapIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, mapIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( mapIdx, 0));
		index += sizeof( Int32);

		sOption = new sPARTYOPTION();
		byte[] tmpData = new byte[sPARTYOPTION.size];
		Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
		sOption.ByteArrayToClass( tmpData);
		index += sPARTYOPTION.size;

		// nUserCnt
		byte[] userCnt = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nUserCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, userCnt, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( userCnt, 0));
		index += sizeof( Int32);

		if( 0 < nUserCnt)
		{
			body = new sPARTYDETAILINFO[nUserCnt];
			for( int i = 0; i < nUserCnt; i++)
			{
				body[i] = new sPARTYDETAILINFO();
	
				tmpData = new byte[sPARTYDETAILINFO.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += sPARTYDETAILINFO.size;
			}
		}
	}
}


public class AS_SC_PARTY_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nPartyIdx;
}


public class AS_SC_PARTY_CAPTAIN : AsPacketHeader
{
	public uint nCharUniqKey;
}


public class AS_SC_PARTY_SETTING_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class AS_SC_PARTY_SETTING_INFO : AsPacketHeader
{
	public Int32 nMapIdx;	//<-- 2014.01.10.
	public sPARTYOPTION sOption;
}


public class AS_SC_PARTY_INVITE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
}


public class AS_SC_PARTY_INVITE : AsPacketHeader
{
	public Int32 nPartyIdx;
	public byte[] szPartyName = new byte[AsGameDefine.MAX_PARTYNAME_LEN + 1];
	public uint nCharUniqKey; //Invitation Info 0:empty
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public bool onOff;
	public byte[] szPartyNotice = new byte[ AsGameDefine.ePARTYNOTICE + 1];
}


public class AS_SC_PARTY_JOIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nPartyIdx;
}


public class AS_SC_PARTY_EXIT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class AS_SC_PARTY_BANNED_EXIT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}


public class AS_SC_PARTY_USER_ADD : AsPacketHeader
{
	public Int32 nPartyIdx;
	public UInt16 nSessionIdx;//0 : the abnormal termination of the user.
	public uint nCharUniqKey;
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public Int32 eRace;
	public Int32 eClass;
	public Int32 nLevel;
	public Int32 nImageTableIdx;
	public float fHpCur;
	public float fHpMax;
	public UInt32 nCondition;//2013.10.31.
	public byte[] szChannelName= new byte[AsGameDefine.eCHANNELNAME + 1]; //2014.01.14.
}


public class AS_SC_PARTY_USER_DEL : AsPacketHeader
{
	public bool bDisconnect;//Abnormal termination( true : Abnormal termination, false : General terminates)
	public uint nCharUniqKey;
	public bool bBanned;
}


public class AS_SC_PARTY_USER_INFO : AsPacketHeader
{
	public uint nCharUniqKey;
	public Int32 nLevel;
	public Int32 nImageTableIdx;
	public float fHpCur;
	public float fHpMax;
	public UInt32 nCondition;//2013.10.31.
	public byte[] szChannelName= new byte[AsGameDefine.eCHANNELNAME + 1]; //2014.01.14 .
}


public class sPARTYUSERBUFF : AsPacketHeader
{
	public bool bUpdate;//Time Update( true : Time Update, false : Insert)
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
	public Int32 nDuration;

	public static int size
	{
		get
		{
			return sizeof( bool) + sizeof( Int32) + sizeof( Int32) + sizeof( Int32) + sizeof( Int32) + sizeof( eBUFFTYPE) + sizeof( Int32);
		}
	}
}


public class AS_SC_PARTY_USER_BUFF : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public bool bEffect;//true:Play
	public Int32 nBuffCnt;
	public body2_SC_PARTY_USER_BUFF[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

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

		if( 0 < nBuffCnt)
		{
			body = new body2_SC_PARTY_USER_BUFF[nBuffCnt];
			for( int i = 0; i < nBuffCnt; i++)
			{
				body[i] = new body2_SC_PARTY_USER_BUFF();
	
				byte[] tmpData = new byte[body2_SC_PARTY_USER_BUFF.size];
				Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
				body[i].ByteArrayToClass( tmpData);
				index += body2_SC_PARTY_USER_BUFF.size;
			}
		}
	}
}

public class body2_SC_PARTY_USER_BUFF : AsPacketHeader// AsBaseClass
{
	public static int size = 1+28;

	public bool bUpdate;
	
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
	public Int32 nDuration;
}


public class AS_SC_PARTY_USER_DEBUFF : AsPacketHeader
{
	public uint nCharUniqKey;
	public Int32 nSkillTableIdx;
	public Int32 nSkillLevelTableIdx;// use only npc
	public Int32 nSkillLevel;
	public Int32 nChargeStep;
	public Int32 nPotencyIdx;
	public eBUFFTYPE eType;
}


// CS_PARTY_USER_POSITION
public class body_CS_PARTY_USER_POSITION : AsPacketHeader
{
	public body_CS_PARTY_USER_POSITION()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_USER_POSITION;
	}
}


public class body2_SC_PARTY_USER_POSITION : AsPacketHeader
{
	public static int size
	{
		get
		{
			return 4 + 4 + ( 4 * 3);
		}
	}

	public UInt32 nCharUniqKey;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}


// SC_PARTY_USER_POSITION
public class body1_SC_PARTY_USER_POSITION : AsPacketHeader
{
	public Int32 nCharCnt;
	public body2_SC_PARTY_USER_POSITION[] body = null;

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

		body = new body2_SC_PARTY_USER_POSITION[nCharCnt];
		for( int i = 0; i < nCharCnt; i++)
		{
			body[i] = new body2_SC_PARTY_USER_POSITION();
			
			byte[] tmpData = new byte[body2_SC_PARTY_USER_POSITION.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_PARTY_USER_POSITION.size;
		}
	}
}


public class AS_SC_PARTY_DICE_ITEM_INFO : AsPacketHeader
{
	public Int32 nDropItemIdx;
	public sITEM sItem;
}


public class AS_SC_PARTY_DICE_SHAKE : AsPacketHeader
{
	public uint nCharUniqKey;
	public Int32 nDropItemIdx;
	public Int32 nDiceNumber;//-1:Give up.
}


public class AS_SC_PARTY_DICE_SHAKE_RESULT : AsPacketHeader
{
	public Int32 nDropItemIdx;
	public Int32 nDiceNumber;//-1:Give up.
	public Int16 nSessionIdx;
	public uint nCharUniqKey;
}



public class body_CS_PARTY_NOTICE_ONOFF : AsPacketHeader
{
	public bool onOff;
	public byte[] szPartyNotice = new byte[ AsGameDefine.ePARTYNOTICE + 1];

	public body_CS_PARTY_NOTICE_ONOFF( bool _onOff, string _szPartyNotice)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_NOTICE_ONOFF;

		onOff = _onOff;
		byte[] tempName = UTF8Encoding.UTF8.GetBytes( _szPartyNotice);
		Buffer.BlockCopy( tempName, 0, szPartyNotice, 0, tempName.Length);
	}
}

public class body_SC_PARTY_NOTICE_ONOFF_NOTIFY : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public Int32 nPartyIdx;
	public bool onOff;
	public string szPartyNotice;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] charUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, charUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( charUniqKey, 0));
		index += sizeof( UInt32);

		// nPartyIdx
		byte[] partyIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nPartyIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, partyIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( partyIdx, 0));
		index += sizeof( UInt32);

		// onOff
		byte[] onOff = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "onOff", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, onOff, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( onOff, 0));
		index += sizeof( bool);

		// szPartyNotice
		int nSendNameLength = AsGameDefine.ePARTYNOTICE + 1;
		byte[] tempSenderName = new byte[ nSendNameLength];
		Buffer.BlockCopy( data, index, tempSenderName, 0, nSendNameLength);
		szPartyNotice = System.Text.UTF8Encoding.UTF8.GetString( tempSenderName);
		index += nSendNameLength;
	}
}


public class body_CS_PARTY_JOIN_REQUEST : AsPacketHeader
{
	public Int32 nPartyIdx;
	public body_CS_PARTY_JOIN_REQUEST( int _nPartyIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_JOIN_REQUEST;
	
		nPartyIdx = _nPartyIdx;
	}
}


public class body_CS_PARTY_JOIN_REQUEST_ACCEPT : AsPacketHeader
{
	public UInt32 nCharUniqKey;	//Information of the applicant.
	public bool bAccept;//true : Accept.

	public body_CS_PARTY_JOIN_REQUEST_ACCEPT( uint _nCharUniqKey, bool _bAccept)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_JOIN_REQUEST_ACCEPT;
	
		nCharUniqKey = _nCharUniqKey;
		bAccept = _bAccept;
	}
}


public class body_SC_PARTY_JOIN_REQUEST_NOTIFY : AsPacketHeader
{
	public UInt32 nCharUniqKey;	//Join the party requester's information.
	public byte[] szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
	public Int32 eClass;
	public Int32 nLevel;
	public Int32 nCurItemRankPoint;
}
//<-- 2014.01.10.
public class body_CS_PARTY_SEARCH : AsPacketHeader
{
	public Int32 nMapIdx;
	public body_CS_PARTY_SEARCH( int _MapIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_SEARCH;

		nMapIdx = _MapIdx;
	}
}


public class body_SC_PARTY_SEARCH_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
}
//<-- 2014.01.21.
#if _USE_PARTY_WARP
public class body_CS_PARTY_WARP_XZ : AsPacketHeader
{
	public UInt32	nCharUniqKey;
	
	//nothing
	public body_CS_PARTY_WARP_XZ( uint _nCharUniqKey )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_WARP_XZ;
		
		nCharUniqKey = _nCharUniqKey;
	}
}

public class body_CS_PARTY_WARP_XZ_END : AsPacketHeader
{
	public body_CS_PARTY_WARP_XZ_END()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS;
		Protocol = (byte)PROTOCOL_CS.CS_PARTY_WARP_XZ_END;	
	}
}

public class body_SC_PARTY_WARP_XZ_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nMapIdx;
	public Vector3 sPosition;
}
#endif

