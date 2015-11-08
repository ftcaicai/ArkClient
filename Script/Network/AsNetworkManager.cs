#define _USE_CRYPT

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum SOCKET_STATE
{
	SS_DISCONNECT = 0,
	SS_LOGIN,
	SS_GAMESERVER,
	SS_SOCKET_NULL
}

public class AsNetworkManager : MonoBehaviour
{
	private static AsNetworkManager instance = null;
	private TcpClient socket = null;
	private int sentCount = 0;
	private bool isConnected = true;
#if _USE_CRYPT
	private AsCrypt m_AsCrypt = new AsCrypt();
#endif
	MemoryStream recvBufferStream = new MemoryStream();

	public delegate void Begin_Process( AsyncCallback end_process);

	public SOCKET_STATE socketState = SOCKET_STATE.SS_DISCONNECT;

	private float m_fSendLiveTime = 0f;
	public void ResetSendLiveTime()
	{
		m_fSendLiveTime = Time.realtimeSinceStartup;
	}

	public static AsNetworkManager Instance
	{
		get { return instance; }
	}
	
	public TcpClient Socket{ get{ return socket;}}

	void Awake()
	{
		instance = this;
	}

	public void OnApplicationQuit()
	{
		InitSocket();
		instance = null;
	}

	// Use this for initialization
//	[SerializeField] float repeatRate = 3f;
	void Start()
	{
		InvokeRepeating( "CheckPacketSize", 0, 1f);
		StartCoroutine( "SendAlive");
	}

	private void ForceQuit()
	{
#if UNITY_EDITOR
//		EditorApplication.isPlaying = false;
//		EditorApplication.isPaused = false;

		ReturnToLogin();//$yde
#else
//		Application.Quit();

		ReturnToLogin();//$yde
#endif

		AsNotify.Instance.CloseAllMessageBox();
	}

	private void ReturnToLogin()
	{
		StartCoroutine(ReturnToLogin_CR());
	}
	
	IEnumerator ReturnToLogin_CR()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseSystemDlg();

		AsChatManager.Instance.ClearAllChat();
		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();
		AsHudDlgMgr.Instance.CloseMsgBox();
		TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
		ArkQuestmanager.instance.ResetQuestManager();
		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();
		AsHudDlgMgr.Instance.invenPageIdx = 0;

		if( null != AsEntityManager.Instance)
		{
			AsEntityManager.Instance.RemoveAllEntities();

			while( true == AsEntityManager.Instance.ModelLoading || true == AsEntityManager.Instance.DestroyingEntity)
				yield return null;
		}
		
		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator SendAlive()
	{
		while( true)
		{
			yield return new WaitForSeconds( 10.0f);

			if( true == AsCommonSender.isSendLivePack &&
				Time.realtimeSinceStartup - m_fSendLiveTime > 40f &&
				SOCKET_STATE.SS_DISCONNECT != AsCommonSender.GetSocketState() &&
				true == isConnected)
			{
				InitSocket();
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(127),
					this, "ForceQuit", "ForceQuit", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
				
				if( null != AsPvpManager.Instance)
					AsPvpManager.Instance.isMatching = false;
				
				if( null != AsInstanceDungeonManager.Instance)
					AsInstanceDungeonManager.Instance.isMatching = false;
			}
			else if( false == AsGameMain.isBackGroundPause && SOCKET_STATE.SS_DISCONNECT != AsCommonSender.GetSocketState() && true == isConnected)
			{
				if( false == AsCommonSender.isSendLivePack)
					m_fSendLiveTime = Time.realtimeSinceStartup;

				AsCommonSender.SendLive();
			}
		}
	}

	public void Start_SendAlive_GameServer()
	{
		StartCoroutine( "SendAlive_GameServer");
	}
	
	public void Stop_SendAlive_GameServer()
	{
		StopCoroutine( "SendAlive_GameServer");
	}
	
	private IEnumerator SendAlive_GameServer()
	{
		while( true)
		{
			yield return new WaitForSeconds( 10.0f);
			
			body_CS_LIVE packetData = new body_CS_LIVE();
			byte[] data = packetData.ClassToPacketBytes();
			AddSend_GameServer( data);
		}
	}

	IEnumerator _AutoQuit()
	{
		yield return new WaitForSeconds( 5.0f);

		if( GAME_STATE.STATE_LOGIN != AsGameMain.s_gameState)
			ForceQuit();
	}

	// Update is called once per frame
	void Update()
	{
		if( ( true == isConnected) && ( null != socket) && ( false == socket.Connected) && ( false == AsInstanceDungeonManager.Instance.bIndunLogoutCoroutine))
		{
			Debug.LogWarning( "AsGameMain.s_gameState : " + AsGameMain.s_gameState);

			if( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState)
			{
				isConnected = false;
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(127),
					this, "ForceQuit", "ForceQuit", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);

				StartCoroutine( "_AutoQuit");
				
				if( null != AsPvpManager.Instance)
					AsPvpManager.Instance.isMatching = false;
				
				if( null != AsInstanceDungeonManager.Instance)
					AsInstanceDungeonManager.Instance.isMatching = false;
			}
		}

		EncodingPackNPass();
	}

	public void InitSocket()
	{
		if( null == socket)
			return;

		socket.Close();
		socketState = SOCKET_STATE.SS_DISCONNECT;
		isConnected = false;
	}

	public Boolean IsConnected()
	{
		if( null == socket)
			return false;

		return socket.Connected;
	}

	public void ConnectToServer( string ip, ushort port, SOCKET_STATE state)
	{
		if( ( null != socket) && ( true == socket.Connected))
			InitSocket();

		socket = new TcpClient();

		try
		{
			socket.Connect( ip, port);
		}
		catch( Exception e)
		{
			Debug.Log( "Exception : " + e);

			switch( state)
			{
			case SOCKET_STATE.SS_LOGIN:
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1446), this, "LoginConnectFailed",
						AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
				}
				return;
			case SOCKET_STATE.SS_GAMESERVER:
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1446), this, "GameConnectFailed",
						AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
				}
				return;
			}

			throw;
		}

		if( true == socket.Connected)
		{
			isConnected = socket.Connected;
			AsCommonSender.isSendLivePack = false;
			socketState = state;
			Recv();
		}
	}
	
	public void SwitchServer(TcpClient newServer)
	{
		socket = newServer;
		Recv();
	}

	private void LoginConnectFailed()
	{
		AsUtil.ShutDown( "Login server connect failed...!");
	}

	private void GameConnectFailed()
	{
		AsUtil.ShutDown( "Game server connect failed...!");
	}

	private void Recv()
	{
		byte[] recv_buffers = new byte[ (int)AsBaseClass.ePackingLength.PL_MAX];

		Begin_Process begin_proc = delegate( AsyncCallback end_process)
		{
			socket.GetStream().BeginRead( recv_buffers, 0, recv_buffers.Length, end_process, null);
		};

		AsyncCallback end_proc = delegate( IAsyncResult ar)
		{
			try
			{
				int recv_count = socket.GetStream().EndRead( ar);
				if( 0 >= recv_count)
					return;

				lock( recvBufferStream)
				{
					recvBufferStream.Write( recv_buffers, 0, recv_count);
				}

				Recv();
			}
			catch
			{
				throw new Exception();
			}
		};

		AsyncProcess( begin_proc, end_proc);
	}

	private void AsyncProcess( Begin_Process begin_process, AsyncCallback end_process)
	{
		begin_process( end_process);
	}

	public void AddSend( byte[] data)
	{
		if( false == socket.Connected)
			return;

#if _USE_CRYPT
		listProtocol.Add( (PROTOCOL_CS)data[2]);
		++packetCount;
		packetTotal += data.Length;

		sentCount = data.Length;
		byte[] encryptData = m_AsCrypt.Encrypt( data);
		Buffer.BlockCopy( encryptData, 0, data, 0, sentCount);
		socket.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
#else
		sentCount = data.Length;
		socket.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
		//$yde
		listProtocol.Add((PROTOCOL_CS)data[2]);
		++packetCount;
		packetTotal += data.Length;
#endif
	}

	public void AddSend_GameServer( byte[] data)
	{
		if( null == AsNetworkIntegratedManager.Instance.socket_game || false == AsNetworkIntegratedManager.Instance.socket_game.Connected)
			return;
		
		#if _USE_CRYPT
		listProtocol.Add( (PROTOCOL_CS)data[2]);
		++packetCount;
		packetTotal += data.Length;
		
		sentCount = data.Length;
		byte[] encryptData = m_AsCrypt.Encrypt( data);
		Buffer.BlockCopy( encryptData, 0, data, 0, sentCount);
		AsNetworkIntegratedManager.Instance.socket_game.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
		#else
		sentCount = data.Length;
		AsNetworkIntegratedManager.Instance.socket_game.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
		//$yde
		listProtocol.Add((PROTOCOL_CS)data[2]);
		++packetCount;
		packetTotal += data.Length;
		#endif
	}

	//$yde
	[SerializeField] int stdPacketCount = 5;
	[SerializeField] int stdPacketTotal = 150;
	int packetCount = 0;
	int packetTotal = 0;
	System.Collections.Generic.List<PROTOCOL_CS> listProtocol = new System.Collections.Generic.List<PROTOCOL_CS>();
	void CheckPacketSize()
	{
		if(packetCount >= stdPacketCount || packetTotal >= stdPacketTotal)
		{
			string protocols = "";
			foreach( PROTOCOL_CS protocol in listProtocol)
			{
				protocols += "/" + protocol;//  + " (or " + (PROTOCOL_CS_2)protocol + ")";
			}
			Debug.LogError( "ptorocols are " + protocols);
			Debug.LogError( "packet count is " + packetCount + ", " + "size:" + packetTotal);
		}

		packetCount = 0;
		packetTotal = 0;
		listProtocol.Clear();
	}//~$yde

	private void SendCallback( IAsyncResult ar)
	{
		socket.GetStream().EndWrite( ar);
	}

	private void EncodingPackNPass()
	{
		lock( recvBufferStream)
		{
			if( 4 > recvBufferStream.Length)
				return;

			recvBufferStream.Position = 0;
			BinaryReader br = new BinaryReader( recvBufferStream);

			byte[] tmp;
			int index = 0;
			int bufferLength = (int)recvBufferStream.Length;
			ushort size = 0;

			while( true)
			{
				size = br.ReadUInt16();
				size += 4;
				recvBufferStream.Position = index;

				if( ( 4 > size) || ( size > bufferLength - index))
					break;

				index += size;

				byte[] buffer = br.ReadBytes( size);

				//$yde
				try
				{
					#if _USE_CRYPT
					byte[] decryptData = m_AsCrypt.Decrypt( buffer);
					//AsNetworkMessageHandler.Instance.MessageToCategory( decryptData);
					
					Buffer.BlockCopy( decryptData, 0, buffer, 0, size);
					AsNetworkMessageHandler.Instance.MessageToCategory( buffer);
					#else
					AsNetworkMessageHandler.Instance.MessageToCategory( buffer);
					#endif
				}
				catch
				{
					//$yde
					PACKET_CATEGORY tempPackCategory = (PACKET_CATEGORY)buffer[3];

					switch( tempPackCategory)
					{
					case PACKET_CATEGORY._CATEGORY_CG:
						Debug.LogError("Invalid packet process: " + (PROTOCOL_GAME)buffer[2]);
						break;
					case PACKET_CATEGORY._CATEGORY_CL:
						Debug.LogError("Invalid packet process: " + (PROTOCOL_LOGIN)buffer[2]);
						break;
					case PACKET_CATEGORY._CATEGORY_CS:
						Debug.LogError("Invalid packet process: " + (PROTOCOL_CS)buffer[2]);
						break;
					case PACKET_CATEGORY._CATEGORY_CS2:
						Debug.LogError("Invalid packet process: " + (PROTOCOL_CS_2)buffer[2]);
						break;
					case PACKET_CATEGORY._CATEGORY_CS3:
						Debug.LogError("Invalid packet process: " + (PROTOCOL_CS_3)buffer[2]);
						break;
					default:
						Debug.LogError("Invalid packet process: " + buffer[2]);
						break;
					}

					if( 0 == buffer.Length)
						break;

					Debug.LogError("AsNetworkManager::EncodingPackNPass: thread error");
					Debug.LogError("Invalid packet size: " + size);
					for( int i = 2; i < 4; ++i)
					{
						if( i >= buffer.Length)
						{
							Debug.LogError("i >= buffer.Length");
							continue;
						}

						Debug.LogError( buffer[i]);
					}

					break;
				}

				if( 4 > bufferLength - index)
					break;
			}

			if( 0 < bufferLength - index)
			{
				tmp = br.ReadBytes( bufferLength - index);
				if( 0 != tmp.Length)
				{
					Debug.Log( "Remain : " + ( bufferLength - index));
					for( int i = 0; i < bufferLength - index; i++)
					{
						if( i >= tmp.Length)
						{
							Debug.LogError("i >= tmp.Length");
							continue;
						}
						Debug.Log( "RemainData [" + i + "]:" + tmp[i]);
					}
				}

				recvBufferStream.SetLength(0);
				recvBufferStream.Write( tmp, 0, tmp.Length);
			}
			else
			{
				recvBufferStream.SetLength(0);
			}
		}
	}
}
