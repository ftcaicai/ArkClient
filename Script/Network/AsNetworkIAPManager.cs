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

public enum IAP_SOCKET_STATE
{
    ISS_CONNECT,
    ISS_DISCONNECT,
}

public class AsNetworkIAPManager : MonoBehaviour
{
    private static AsNetworkIAPManager instance = null;
	private TcpClient socket = null;
	private int sentCount = 0;
	private bool isConnected = false;
	MemoryStream recvBufferStream = new MemoryStream();
	#if _USE_CRYPT
	private AsCrypt m_AsCrypt = new AsCrypt();
	#endif	
	public delegate void Begin_Process( AsyncCallback end_process);

    public IAP_SOCKET_STATE socketState = IAP_SOCKET_STATE.ISS_DISCONNECT;

    public static AsNetworkIAPManager Instance
	{
		get { return instance; }
	}

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
		InvokeRepeating("CheckPacketSize", 0, 1f);
		StartCoroutine( "SendAlive");
	}
	
	private IEnumerator SendAlive()
	{
		while( true)
		{
			yield return new WaitForSeconds( 15.0f);
			
            if( SOCKET_STATE.SS_DISCONNECT != AsCommonSender.GetSocketState() && isConnected == true)
            {
                body_CS_LIVE packetData = new body_CS_LIVE();
                byte[] data = packetData.ClassToPacketBytes();
                AsCommonSender.SendToIAP( data);
            }
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		EncodingPackNPass();
//		AsCommonSender.ActionUpdate();
	}

	public void InitSocket()
	{
		if( null == socket)
			return;

		socket.Close();
        socketState = IAP_SOCKET_STATE.ISS_DISCONNECT;
		isConnected = false;
	}

	public Boolean IsConnected()
	{
		if( null == socket)
			return false;

		return socket.Connected;
	}

    public void DisConnect()
    {
        if (socket != null)
            socket.Close();

        socketState = IAP_SOCKET_STATE.ISS_DISCONNECT;
    }

	public bool ConnectToServer( string ip, ushort port, IAP_SOCKET_STATE state)
	{
		if( ( null != socket) && ( true == socket.Connected))
			InitSocket();

		try
		{
            socket = AsTimeoutSocket.Connect(ip, port, 5000);
		}
		catch( Exception e)
		{
			Debug.Log( "Exception : asNetworkIAPMg");

            if (state == IAP_SOCKET_STATE.ISS_CONNECT)
            {
                if (socket != null)
                    socket.Close();

                isConnected = false;

                return false;
            }
		}

        if (true == socket.Connected)
        {
            isConnected = socket.Connected;
            socketState = state;
            Recv();
            return true;
        }
        else
            return false;
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
		
	   listProtocol.Add((PROTOCOL_IAP)data[2]);
		++packetCount;
		packetTotal += data.Length;		
		
		sentCount = data.Length;
		byte[] encryptData = m_AsCrypt.Encrypt(data);
		Buffer.BlockCopy(encryptData, 0, data, 0, sentCount);
		socket.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);	
#else
		sentCount = data.Length;
		socket.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
//        AsCommonSender.ActionSender();
		//$yde
//		byte[] type = new byte[2];
//		type[0] = data[0];
//		type[1] = data[1];
        listProtocol.Add((PROTOCOL_IAP)data[2]);
		++packetCount;
		packetTotal += data.Length;
#endif
		
		
	
	}
	
	//$yde
	[SerializeField] int stdPacketCount = 5;
	[SerializeField] int stdPacketTotal = 150;
	int packetCount = 0;
	int packetTotal = 0;
    System.Collections.Generic.List<PROTOCOL_IAP> listProtocol = new System.Collections.Generic.List<PROTOCOL_IAP>();
	void CheckPacketSize()
	{
		if(packetCount >= stdPacketCount || packetTotal >= stdPacketTotal)
		{
			string protocols = "";
            foreach (PROTOCOL_IAP protocol in listProtocol)
			{
				protocols += "/" + protocol;//  + " (or " + (PROTOCOL_CS_2)protocol + ")";
			}
            // IAP packet size is over 3000 bytes
			//Debug.LogWarning("ptorocols are " + protocols);
            //Debug.LogWarning("packet count is " + packetCount + ", " + "size:" + packetTotal);
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
				
				byte[] buffer = br.ReadBytes(size);
				
				//$yde
				try
				{	
					#if _USE_CRYPT	
						 byte[] decryptData = m_AsCrypt.Decrypt(buffer);				
						 //AsNetworkMessageHandler.Instance.MessageToCategory( decryptData );	
						
						  Buffer.BlockCopy(decryptData, 0, buffer, 0, size);
					      AsNetworkMessageHandler.Instance.MessageToCategory( buffer );	
					#else
					
						AsNetworkMessageHandler.Instance.MessageToCategory( buffer );	
					#endif
					
					
				}
				catch
				{
					//$yde
					PACKET_CATEGORY tempPackCategory = (PACKET_CATEGORY)buffer[3];


                    if (PACKET_CATEGORY._CATEGORY_CI == tempPackCategory)
                    {
                        Debug.LogError("Invalid packet process:");//+ (PROTOCOL_IAP)buffer[2]);
                    }

					
					if( 0 == buffer.Length)
						break;
					
					//Debug.LogError("AsIAPNetworkManager::EncodingPackNPass: thread error"  );
					for( int i=2; i<4; ++i )					
					{
						if( i >= buffer.Length )
						{
							//Debug.LogError("i >= buffer.Length");
							continue;
						}
						
						//Debug.LogError( buffer[i] ); 
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
					//Debug.Log( "Remain : " + ( bufferLength - index));
					for( int i = 0; i < bufferLength - index; i++)
					{
						if( i >= tmp.Length )
						{
						//	Debug.LogError("i >= tmp.Length");
							continue;
						}
						//Debug.Log( "RemainData [" + i + "]:" + tmp[i]);
					}
				}
			
				recvBufferStream.SetLength(0);
				recvBufferStream.Write( tmp, 0, tmp.Length );
			}
			else
			{
				recvBufferStream.SetLength(0);
			}
		}
	}
}
