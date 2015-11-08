
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
		TimeoutObject.Reset(); // �̺�Ʈ ���¸� �ʱ�ȭ

		socketexception = null; // ���ܹ߻�

		string serverip = ip; // IP

		int serverport = port;

		TcpClient tcpclient = new TcpClient();

		// �񵿱� ����, CallBackMethod

		tcpclient.BeginConnect( serverip, serverport, new AsyncCallback( CallBackMethod), tcpclient);

		if ( TimeoutObject.WaitOne( timeoutMSec, false)) // ����ȭ��Ų��. timeoutMSec ���� ��ٸ���.
		{
			if ( IsConnectionSuccessful) // ���ӵǴ� ������
				return tcpclient;
			else
				throw socketexception; // �ȵǸ�, �ȵ� ���ܸ�
		}
		else // �ð��� �ʰ��Ǹ�
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
