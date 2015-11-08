using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class AsNetworkIntegratedManager : MonoBehaviour
{
	private static AsNetworkIntegratedManager instance = null;
	private TcpClient m_socket_game = null;
	private TcpClient m_socket_integrated = null;
	private bool isConnected = true;
	public TcpClient socket_game{ get{ return m_socket_game;}}

	public static AsNetworkIntegratedManager Instance
	{
		get { return instance;}
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

	public void InitSocket()
	{
		if( null == m_socket_integrated)
			return;

		m_socket_integrated.Close();
		isConnected = false;
	}

	public bool IsConnected()
	{
		/*
		if( null == m_socket_integrated)
			return false;

		return m_socket_integrated.Connected;
		*/
		return isConnected;
	}

    public void DisConnect()
    {
		if (m_socket_integrated != null)
			m_socket_integrated.Close();
		
		isConnected = false;
		AsNetworkManager.Instance.SwitchServer( m_socket_game);
	}
	
	public void SwitchServer()
	{
		isConnected = false;
		AsNetworkManager.Instance.SwitchServer( m_socket_game);
	}

	public void ConnectToServer( string ip, ushort port)
	{
		if( ( null != m_socket_integrated) && ( true == m_socket_integrated.Connected))
			InitSocket();

		m_socket_integrated = new TcpClient();

		try
		{
			m_socket_integrated.Connect( ip, port);
		}
		catch( Exception e)
		{
			Debug.Log( "Exception : " + e);
			throw;
		}

		if( true == m_socket_integrated.Connected)
		{
			isConnected = m_socket_integrated.Connected;
			
			m_socket_game = AsNetworkManager.Instance.Socket;
			AsNetworkManager.Instance.SwitchServer( m_socket_integrated);
		}
	}

	void Start()
	{
	}
	
	void Update()
	{
	}
}
