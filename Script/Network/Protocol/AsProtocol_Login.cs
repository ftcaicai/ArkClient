using UnityEngine;
using System;
using System.Reflection;


enum PROTOCOL_LOGIN : byte
{
	CL_LIVE,
	LC_LIVE_ECHO,
	CL_LOGIN,
	LC_LOGIN_RESULT,
	CL_SERVERLIST,
	LC_SERVERLIST_RESULT,
	#region -WEME Certify
	CL_WEMELOGIN,
	LC_WEMELOGIN_RESULT,
	CL_ACCOUNT_DELETE,
	LC_ACCOUNT_DELETE,
	#endregion
}


public class AS_CL_LOGIN : AsPacketHeader
{
	public byte[] id = new byte[AsGameDefine.MAX_ID_LEN + 1];
	public byte[] pw = new byte[AsGameDefine.MAX_PW_LEN + 1];
	public bool bIsAccountDeleteCancel;

	public AS_CL_LOGIN( string inId, string inPw,bool isAccountDeleteCancel)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CL;
		Protocol = (byte)PROTOCOL_LOGIN.CL_LOGIN;

		byte[] id = System.Text.UTF8Encoding.UTF8.GetBytes( inId.ToCharArray(), 0, inId.Length);
		Buffer.BlockCopy( id, 0, this.id, 0, id.Length);
		byte[] pw = System.Text.UTF8Encoding.UTF8.GetBytes( inPw.ToCharArray(), 0, inPw.Length);
		Buffer.BlockCopy( pw, 0, this.pw, 0, pw.Length);

		bIsAccountDeleteCancel = isAccountDeleteCancel;
	}
};

public class AS_LC_LOGIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nUserUniqKey;
	public UInt32 nUserSessionKey;

	public bool bIOS_Promotion;
	public bool bIOS_Marketing_Banner;
	public bool bIOS_Platform;
	public bool bIOS_Friend_Reward;
	public bool bIOS_Recommend_Reward;
	public bool bIOS_Review_Reward;
	public bool bIOS_Coupon;
	public bool bIOS_Social_Reward;
};

public class AS_CL_SERVERLIST : AsPacketHeader
{
	public AS_CL_SERVERLIST()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CL;
		Protocol = (byte)PROTOCOL_LOGIN.CL_SERVERLIST;
	}
}

public class AS_LC_SERVERLIST_BODY : AsPacketHeader
{
	public byte[] szServerName = new byte[AsNetworkDefine.eSERVERNAME + 1];
	public byte[] szIpaddress = new byte[AsNetworkDefine.eIPADDRESS + 1];
	public UInt16 nPort;
	public Int32 nWorldIdx;
	public Int32 nCurUser;
	public Int32 nMaxUser;
	public bool bPossibleCreate;
	public bool bGMOpen;

	public static int size
	{
		get
		{
			return ( ( sizeof( byte) * ( AsNetworkDefine.eSERVERNAME + 1)) + ( sizeof( byte) * ( AsNetworkDefine.eIPADDRESS + 1)) + sizeof( UInt16)
				+ sizeof( Int32) + sizeof( Int32) + sizeof( Int32) + sizeof( bool) + sizeof( bool));
		}
	}
}

public class AS_LC_SERVERLIST_COUNT : AsPacketHeader
{
	public Int32 nLastWorldIdx;
	public Int32 nServerNewCnt;
	public Int32 nServerCnt;
	public string szIAPAgentIP = string.Empty;
	public string szIAPDomain = string.Empty;
	public UInt16 nIAPAgentPort;

	public AS_LC_SERVERLIST_BODY[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nLastWorldIdx
		byte[] lastWorldIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nLastWorldIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, lastWorldIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( lastWorldIdx, 0));
		index += sizeof( Int32);

		// nServerNewCnt
		byte[] serverNewCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nServerNewCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, serverNewCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( serverNewCount, 0));
		index += sizeof( Int32);

		// nServerCnt
		byte[] serverCount = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nServerCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, serverCount, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( serverCount, 0));
		index += sizeof( Int32);

		// iap ip
		byte[] szIAPIP = new byte[AsNetworkDefine.eIPADDRESS + 1];
		headerinfo = infotype.GetField( "szIAPAgentIP", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, szIAPIP, 0, AsNetworkDefine.eIPADDRESS + 1);
		headerinfo.SetValue( this, System.Text.ASCIIEncoding.ASCII.GetString( szIAPIP));
		index += AsNetworkDefine.eIPADDRESS + 1;

		// iap domain
		byte[] szDomain = new byte[AsNetworkDefine.eDOMAIN + 1];
		headerinfo = infotype.GetField("szIAPDomain", BINDING_FLAGS_PIG);
		Buffer.BlockCopy(data, index, szDomain, 0, AsNetworkDefine.eDOMAIN + 1);
		headerinfo.SetValue(this, System.Text.ASCIIEncoding.ASCII.GetString( szDomain));
		index += AsNetworkDefine.eDOMAIN + 1;

		// iap port
		byte[] szIAPPort = new byte[sizeof( UInt16)];
		headerinfo = infotype.GetField( "nIAPAgentPort", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, szIAPPort, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( szIAPPort, 0));
		index += sizeof( UInt16);

		AsNetworkDefine.IAPAGENT_SERVER_IP = szIAPAgentIP;
		AsNetworkDefine.IAPAGENT_DOMAIN = szIAPDomain;
		AsNetworkDefine.IAPAGENT_SERVER_PORT = nIAPAgentPort;

		//$yde
		Debug.Log( "AS_LC_SERVERLIST_COUNT::PacketBytesToClass: AsNetworkDefine.IAPAGENT_SERVER_IP = " + AsNetworkDefine.IAPAGENT_SERVER_IP);
		Debug.Log( "AS_LC_SERVERLIST_COUNT::PacketBytesToClass: AsNetworkDefine.IAPAGENT_DOMAIN = " + AsNetworkDefine.IAPAGENT_DOMAIN);
		Debug.Log( "AS_LC_SERVERLIST_COUNT::PacketBytesToClass: AsNetworkDefine.IAPAGENT_SERVER_PORT = " + AsNetworkDefine.IAPAGENT_SERVER_PORT);

		body = new AS_LC_SERVERLIST_BODY[ nServerCnt];
		for( int i = 0; i < nServerCnt; i++)
		{
			body[i] = new AS_LC_SERVERLIST_BODY();

			byte[] tmpData = new byte[AS_LC_SERVERLIST_BODY.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += AS_LC_SERVERLIST_BODY.size;
		}
	}
}

#region -WEME Certify
public class body_CL_WEMELOGIN : AsPacketHeader
{
	public body_CL_WEMELOGIN()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CL;
		Protocol = (byte)PROTOCOL_LOGIN.CL_WEMELOGIN;
	}

	public int eLoginType = 0;
	public byte[] strCheckAuth = new byte[ AsGameDefine.eWEMECHECKAUTH + 1];
	public byte[] strRecord = new byte[ AsGameDefine.eWEMERECORD + 1];
	public bool bIsGuest;
	public bool bIsAccountDeleteCancel;
}

public class body_LC_WEMELOGIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nUserUniqKey;
	public byte[] strAccessToken = new byte[ AsGameDefine.eACCESSTOKEN + 1];
	public bool bIsGuest;

	public bool bIOS_Promotion;
	public bool bIOS_Marketing_Banner;
	public bool bIOS_Platform;
	public bool bIOS_Friend_Reward;
	public bool bIOS_Recommend_Reward;
	public bool bIOS_Review_Reward;
	public bool bIOS_Coupon;
	public bool bIOS_Social_Reward;
}

public class body_CL_ACCOUNT_DELETE : AsPacketHeader
{
	public body_CL_ACCOUNT_DELETE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CL;
		Protocol = (byte)PROTOCOL_LOGIN.CL_ACCOUNT_DELETE;
	}
}

public class body_LC_ACCOUNT_DELETE : AsPacketHeader
{
	public eRESULTCODE eResult;
}
#endregion