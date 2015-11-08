#define _USE_PARTY_WARP
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class AsPartySender
{
	static public void Send( byte[] data)
	{
		switch ( AsCommonSender.GetSocketState())
		{
		case SOCKET_STATE.SS_LOGIN:
		case SOCKET_STATE.SS_GAMESERVER:
			{
				if( null != AsNetworkManager.Instance)
					AsNetworkManager.Instance.AddSend( data);
			}
			return;
		}

		Debug.LogError( " AsCommonSender :: Send [ SOCKET_STATE == none ] ");
	}

	static public void SendPartyList( int _MapIdx)
	{
		AS_CS_PARTY_LIST packetData = new AS_CS_PARTY_LIST( _MapIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_LIST");
		#endif
	}

	static public void SendPartyDetailInfo( Int32 _nPartyIdx)
	{
		AS_CS_PARTY_DETAIL_INFO packetData = new AS_CS_PARTY_DETAIL_INFO( _nPartyIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_DETAIL_INFO");
		#endif
	}

	static public void SendPartyCreate( int _MapIdx, sPARTYOPTION stOption)
	{
		if( AsPartyManager.Instance.IsPartying)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00001]");
			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00001]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_CREATE packetData = new AS_CS_PARTY_CREATE( _MapIdx, stOption);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_CREATE ");
		#endif
	}

	static public void SendPartyCaptain( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		if( !AsPartyManager.Instance.IsCaptain)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00002]");
			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00002]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( AsPartyManager.Instance.GetPartyMember( _CharUniqKey) == null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00003]");
			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00003]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_CAPTAIN packetData = new AS_CS_PARTY_CAPTAIN( _SessionIdx, _CharUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		AsPartyManager.Instance.IsCaptain = false;

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_CAPTAIN _SessionIdx :" + _SessionIdx + " CharUniqKey :" + _CharUniqKey);
		#endif
	}

	static public void SendPartySetting( int _MapIdx, sPARTYOPTION stOption)
	{
		if( !AsPartyManager.Instance.IsCaptain)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00004]");

		//	string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00004]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_SETTING packetData = new AS_CS_PARTY_SETTING( _MapIdx, stOption);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_SETTING" + packetData.bPublic.ToString() + packetData.ePurpose.ToString() + "Divide:"+ packetData.eDivide.ToString() + "eGrade" + packetData.eGrade.ToString());
		#endif
	}

	static public void SendPartyInvite( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		if( 0 >= _SessionIdx)
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String( 159), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( false == AsPartyManager.Instance.IsPartying)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == userEntity)
				return;

			StringBuilder sb = new StringBuilder();
			sb.Append( userEntity.GetProperty<string>( eComponentProperty.NAME));
			sb.Append( AsTableManager.Instance.GetTbl_String( 1357));
			AsPartyManager.Instance.SelfPartyCreate( sb.ToString());
		}

		AS_CS_PARTY_INVITE packetData = new AS_CS_PARTY_INVITE( _SessionIdx, _CharUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_INVITE _SessionIdx : " + _SessionIdx + " CharUniqKey : " + _CharUniqKey);
		#endif
	}

	static public void SendPartyJoin( int _nJoinType, Int32 _nPartyIdx, uint nCharUniqKey)
	{
		if( AsPartyManager.Instance.IsPartying)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00005]");
			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00005]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_JOIN packetData = new AS_CS_PARTY_JOIN( _nJoinType, _nPartyIdx, nCharUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		if( _nJoinType == ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_NORMAL || _nJoinType == ( int)ePARTYJOINTYPE.ePARTYJOINTYPE_ACCEPT)
			AsPartyManager.Instance.IsPartying = true;

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_JOIN JoinType : " + _nJoinType + " PartyIdx : " + _nPartyIdx);
		#endif
	}

	static public void SendPartyExit()
	{
		if( !AsPartyManager.Instance.IsPartying)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00006]");
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_EXIT packetData = new AS_CS_PARTY_EXIT();
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_EXIT");
		#endif
	}

	static public void SendPartyBannedExit( UInt16 _SessionIdx, uint _CharUniqKey)
	{
		if( !AsPartyManager.Instance.IsCaptain)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00007]");
			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00007]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		if( AsPartyManager.Instance.GetPartyMember( _CharUniqKey) == null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( AsTableManager.Instance.GetTbl_String( 117));
			sb.Append( "[P00008]");

			//string strMsg = AsTableManager.Instance.GetTbl_String( 117) + "[P00008]";
			AsChatManager.Instance.InsertChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}

		AS_CS_PARTY_BANNED_EXIT packetData = new AS_CS_PARTY_BANNED_EXIT( _SessionIdx, _CharUniqKey);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_BANNED_EXIT _SessionIdx :" + _SessionIdx + " CharUniqKey :" + _CharUniqKey);
		#endif
	}

	static public void SendPartyDiceShake( bool _bAccept, int _nDropItemIdx)
	{
		AS_CS_PARTY_DICE_SHAKE packetData = new AS_CS_PARTY_DICE_SHAKE( _bAccept, _nDropItemIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send AS_CS_PARTY_DICE_SHAKE bAccept :" + _bAccept.ToString() + " nDropItemIdx :" + _nDropItemIdx.ToString());
		#endif
	}

	static public void SendPartyUserPosition()
	{
		if( GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return;

		body_CS_PARTY_USER_POSITION packetData = new body_CS_PARTY_USER_POSITION();
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "Send CS_PARTY_USER_POSITION");
		#endif
	}

	static public void SendPartyNoticeOnOff( bool _onOff, string _szPartyNotice)
	{
		body_CS_PARTY_NOTICE_ONOFF packetData = new body_CS_PARTY_NOTICE_ONOFF( _onOff, _szPartyNotice);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "SendPartyNoticeOnOff :" + __onOff.ToString() + "PartyNotice :" + _szPartyNotice);
		#endif
	}

	static public void SendPartyJoinRequest( Int32 _nPartyIdx)
	{
		body_CS_PARTY_JOIN_REQUEST packetData = new body_CS_PARTY_JOIN_REQUEST( _nPartyIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "SendPartyJoinRequest :" + _nPartyIdx.ToString());
		#endif
	}

	static public void SendPartyJoinRequestAccept( uint _CharUniqKey, bool bApccept)
	{
		body_CS_PARTY_JOIN_REQUEST_ACCEPT packetData = new body_CS_PARTY_JOIN_REQUEST_ACCEPT( _CharUniqKey, bApccept);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "SendPartyJoinRequestAccept :" + _CharUniqKey.ToString() + bApccept.ToString());
		#endif
	}

	static public void SendPartySearch( int _MapIdx)
	{
		body_CS_PARTY_SEARCH packetData = new body_CS_PARTY_SEARCH( _MapIdx);
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);

		#if _PARTY_LOG_
		Debug.Log( "SendPartySearch :" + _MapIdx.ToString());
		#endif
	}

	static public void SendWarpXZ(uint _CharUniqKey)
	{
		#if _USE_PARTY_WARP	
		body_CS_PARTY_WARP_XZ packetData = new body_CS_PARTY_WARP_XZ( _CharUniqKey );
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);
		#endif	
		
		#if _PARTY_LOG_
		Debug.Log( "SendWarpXZ");
		#endif
	}
	
	static public void SendWarpXZEnd()
	{
		#if _USE_PARTY_WARP	
		body_CS_PARTY_WARP_XZ_END packetData = new body_CS_PARTY_WARP_XZ_END();
		byte[] data = packetData.ClassToPacketBytes();
		Send( data);
		#endif	
		
		#if _PARTY_LOG_
		Debug.Log( "SendWarpXZEnd");
		#endif
	}

}


