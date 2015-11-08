using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;


public class AsNetWork : MonoBehaviour
{
	private int sentCount = 0;
	private TcpClient socket = null;
	private MemoryStream recvBufferStream = new MemoryStream();
	private SOCKET_STATE socketState = SOCKET_STATE.SS_DISCONNECT;

	public delegate void Begin_Process( AsyncCallback end_process);

	public SOCKET_STATE GetSocketState()
	{
		return socketState;
	}

	public void InitSocket()
	{
		if( null == socket)
			return;

		socket.Close();
		socketState = SOCKET_STATE.SS_DISCONNECT;
	}

	public Boolean IsConnected()
	{
		if( null == socket)
			return false;
	
		return socket.Connected;
	}

	private void Recv()
	{
		byte[] recv_buffers = new byte[( int)AsBaseClass.ePackingLength.PL_MAX];

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

		sentCount = data.Length;
		socket.GetStream().BeginWrite( data, 0, sentCount, new AsyncCallback( SendCallback), null);
	}

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
			int bufferLength = ( int)recvBufferStream.Length;
			ushort size = 0;

			while( true)
			{
				size = br.ReadUInt16();
				size += 4;
				recvBufferStream.Position = index;

				if( ( 4 > size) || ( size > bufferLength - index))
					break;

				index += size;

				AsNetworkMessageHandler.Instance.MessageToCategory( br.ReadBytes( size));

				if( 4 > bufferLength - index)
					break;
			}

			if( 0 < bufferLength - index)
			{
				tmp = br.ReadBytes( bufferLength - index);
				Debug.Log( "Remain : " + ( bufferLength - index));
				for ( int i = 0; i < bufferLength - index; i++)
				{
					Debug.Log( "RemainData [" + i + "]:" + tmp[i]);
				}
				recvBufferStream.SetLength( 0);
				recvBufferStream.Write( tmp, 0, tmp.Length);
			}
			else
			{
				recvBufferStream.SetLength( 0);
			}
		}
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
		catch
		{
			switch( state)
			{
			case SOCKET_STATE.SS_LOGIN:
				AsNotify.Instance.MessageBox( "Error", "Login server connect\nfailed...!", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
				break;
			case SOCKET_STATE.SS_GAMESERVER:
				AsNotify.Instance.MessageBox( "Error", "Game server connect\nfailed...!", null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
				break;
			}
		}

		if( true == socket.Connected)
		{
			socketState = state;
			Recv();
		}
	}

	void Update()
	{
		EncodingPackNPass();
	}
}
