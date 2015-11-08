using UnityEngine;
using System.Collections;


public class AsIAPProcess : AsProcessBase
{
	public override void Process( byte[] _packet)
	{
		PROTOCOL_IAP protocol = (PROTOCOL_IAP)_packet[2];
		
		try
		{
			switch( protocol)
			{
			#region -apple-
			case PROTOCOL_IAP.IC_APPLE_IAP_START_RES:
				StartAppleIAPResult( _packet);
				break;
			case PROTOCOL_IAP.IC_APPLE_IAP_END_RES:
				EndAppleIAPResult( _packet);
				break;
			case PROTOCOL_IAP.IC_APPLE_IAP_REGISTER_RES:
				GetItemRegisterKeyResult( _packet);
				break;
			#endregion

			#region -android-
			case PROTOCOL_IAP.IC_ANDROID_IAP_REGISTER_RES:
				GetItemRegisterKeyResult( _packet);
				break;
			case PROTOCOL_IAP.IC_ANDROID_IAP_START_RES:
				StartAndroidIAPResult( _packet);
				break;
			case PROTOCOL_IAP.IC_ANDROID_IAP_END_RES:
				EndAndroidIAPResult( _packet);
				break;
			#endregion

			#region - coupon -
			case PROTOCOL_IAP.IC_COUPON_RES:
				CouponResult( _packet);
				break;
			#endregion
			}
		}
		catch ( System.Exception e)
		{
			//Debug.LogError( "Protocol : " + protocol + " --------> exception : " + e);
			Debug.LogError("error Protocol iap proc");
		}
	}

	#region -Apple-
	private void StartAppleIAPResult( byte[] _packet)
	{
		body_IC_APPLE_IAP_START_RES data = new body_IC_APPLE_IAP_START_RES();
		data.PacketBytesToClass( _packet);

		if( AsBasePurchase.Instance == null)
			return;

		AsBasePurchase.Instance.StopTimer();

		// stop timer ---> startIAP timer
		Debug.Log( "Start IAP Result = " + data.eResult);

        AsWebRequest.Instance.Request_Purchase(190106, Application.platform, data.eResult.ToString(), "Start IAP Result");

		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_IAP_NOINSERT:
			AsBasePurchase.Instance.RequestInsertReceipt();
			break;
		case eRESULTCODE.eRESULT_IAP_NOUSE:
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.SetItemUniqueID( data.nIAPUniqKey);
			AsBasePurchase.Instance.RequestGetCashItem();
			break;
		case eRESULTCODE.eRESULT_IAP_COMPLATE:
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.OrganizeTransaction(false);
			return;
		}
	

	}

	private void EndAppleIAPResult( byte[] _packet)
	{
		body_IC_APPLE_IAP_END_RES data = new body_IC_APPLE_IAP_END_RES();
		data.PacketBytesToClass( _packet);

		if( AsBasePurchase.Instance == null)
			return;

		AsBasePurchase.Instance.StopTimer();

        AsWebRequest.Instance.Request_Purchase(190108, Application.platform, data.eResult.ToString(), "End IAP Result");

		if( data.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.SetItemUniqueID( data.nIAPUniqKey);
			AsBasePurchase.Instance.RequestGetCashItem();
		}
	}
	#endregion

	#region -Android-
	private void StartAndroidIAPResult( byte[] _packet)
	{
		body_IC_ANDROID_IAP_START_RES data = new body_IC_ANDROID_IAP_START_RES();
		data.PacketBytesToClass( _packet);
	
		if( AsBasePurchase.Instance == null)
			return;

		AsBasePurchase.Instance.StopTimer();

        AsWebRequest.Instance.Request_Purchase(190106, Application.platform, data.eResult.ToString(), "StartAndroidIAPResult");

		switch( data.eResult)
		{
		case eRESULTCODE.eRESULT_IAP_NOINSERT:
			AsBasePurchase.Instance.RequestInsertReceipt();
			break;
		case eRESULTCODE.eRESULT_IAP_NOUSE:
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.SetItemUniqueID( data.nIAPUniqKey);
			AsBasePurchase.Instance.RequestGetCashItem();
			break;
		case eRESULTCODE.eRESULT_IAP_COMPLATE:
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.OrganizeTransaction(false);
			break;
		default:
			AsBasePurchase.Instance.DisconnectIAPA();
			Debug.LogWarning( data.eResult.ToString());
			break;
		}
	}

	private void EndAndroidIAPResult( byte[] _packet)
	{
		body_IC_ANDROID_IAP_END_RES data = new body_IC_ANDROID_IAP_END_RES();
		data.PacketBytesToClass( _packet);

		if( AsBasePurchase.Instance == null)
			return;

		AsBasePurchase.Instance.StopTimer();

        AsWebRequest.Instance.Request_Purchase(190108, Application.platform, data.eResult.ToString(), "End IAPA Result");

		if( data.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsBasePurchase.Instance.DisconnectIAPA();
			AsBasePurchase.Instance.SetItemUniqueID( data.nIAPUniqKey);
			AsBasePurchase.Instance.RequestGetCashItem();
		}
		else
		{
			Debug.Log( "fail End IAP Result = ");// + data.eResult);
		}
	}

	private void ProcessGetItemRegisterKeyResult(int _key)
	{
		if (AsBasePurchase.Instance == null)
			return;
		
		AsBasePurchase.Instance.StopTimer();
		AsBasePurchase purchaseMgr = null;
		
		if (AsCashStore.Instance == null)
			return;
		
		purchaseMgr = AsCashStore.Instance.GetPurchaseManager();
		
		if (purchaseMgr == null)
			return;

		bool isUnhandledTransaction = purchaseMgr.IsUnHandledTransaction();
		bool isRetryConnectIapServer = purchaseMgr.IsRetryConnectIAPAServer();
		
		if (isUnhandledTransaction == true || isRetryConnectIapServer == true)
		{
			purchaseMgr.StartGetItemProcess();
		}
		else
		{
			// purchase now item
			if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			{
				if (AsHudDlgMgr.Instance.IsOpenCashStore == true)
					AsHudDlgMgr.Instance.cashStore.BuyNowItem(_key);
			}
			else if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
			{
				if (AsCashStore.Instance != null)
					AsCashStore.Instance.BuyNowItem(_key);
			}
		}

	}

	private void GetItemRegisterKeyResult(byte[] _packet)
	{
		int key = 0;
		if (Application.platform == RuntimePlatform.Android)
		{
			body_IC_ANDROID_IAP_REGISTER_RES data = new body_IC_ANDROID_IAP_REGISTER_RES();
			data.PacketBytesToClass(_packet);
			key = data.nRegisterKey;
			ProcessGetItemRegisterKeyResult(key);
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			ProcessGetItemRegisterKeyResult(key);
		}
		else
		{
			Debug.LogWarning("[Get Item Register]not support platform = " + Application.platform.ToString());
		}
	}
	#endregion

	#region - coupon -
	void CouponResult( byte[] _packet)
	{
		AsNetworkIAPManager.Instance.DisConnect();

		body_IC_COUPON_RES result = new body_IC_COUPON_RES();
		result.PacketBytesToClass( _packet);

		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		switch( result.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			body_CS_COUPON_REGIST regist = new body_CS_COUPON_REGIST();
			regist.szCouponKey = AsCouponDlg.CouponKey;
			AsCommonSender.Send( regist.ClassToPacketBytes());
			return;
		case eRESULTCODE.eRESULT_FAILE_COUPON_DELAY:
			sb.Append( AsTableManager.Instance.GetTbl_String(1626));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_DUPLICATE:
			sb.Append( AsTableManager.Instance.GetTbl_String(1627));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_DAILYOVER:
			sb.Append( AsTableManager.Instance.GetTbl_String(1628));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_EXPIRATION:
			sb.Append( AsTableManager.Instance.GetTbl_String(1656));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_DELETED:
			sb.Append( AsTableManager.Instance.GetTbl_String(1657));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_COUNTOVER:
			sb.Append( AsTableManager.Instance.GetTbl_String(1629));
			break;
		case eRESULTCODE.eRESULT_FAILE_COUPON_INVALID:
			sb.Append( AsTableManager.Instance.GetTbl_String(1658));
			break;
		default:
			Debug.LogError( "AsIAPProcess::CouponResult: unknown result [" + result.eResult + "]");
			return;
		}

		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), sb.ToString());
	}
	#endregion
}



