
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class AsTimeoutSocket
{
	private static bool IsConnectionSuccessful = false;
	private static Exception socketexception;
	private static ManualResetEvent TimeoutObject = new ManualResetEvent( false);

	public static TcpClient Connect( string ip, ushort port, int timeoutMSec)
	{
		TimeoutObject.Reset(); // 이벤트 상태를 초기화

		socketexception = null; // 예외발생

		string serverip = ip; // IP

		int serverport = port;

		TcpClient tcpclient = new TcpClient();

		// 비동기 접속, CallBackMethod

		tcpclient.BeginConnect( serverip, serverport, new AsyncCallback( CallBackMethod), tcpclient);

		if ( TimeoutObject.WaitOne( timeoutMSec, false)) // 동기화시킨다. timeoutMSec 동안 기다린다.
		{
			if ( IsConnectionSuccessful) // 접속되는 소켓을
				return tcpclient;
			else
				throw socketexception; // 안되면, 안된 예외를
		}
		else // 시간이 초과되면
		{
			tcpclient.Close();
			throw new TimeoutException( "TimeOut Exception");
		}
	}

	private static void CallBackMethod( IAsyncResult asyncresult)
	{
		try
		{
			IsConnectionSuccessful = false;
			TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

			if ( tcpclient.Client != null)
			{
				tcpclient.EndConnect( asyncresult);
				IsConnectionSuccessful = true;
			}
		}
		catch ( Exception ex)
		{
			IsConnectionSuccessful = false;
			socketexception = ex;
		}
		finally
		{
			TimeoutObject.Set();
		}
	}
}
