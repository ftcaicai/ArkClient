using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using System.Text;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
using WmWemeSDK.JSON;


public class WemeGateManager : AsSceneBase
{
#if UNITY_ANDROID
	const string market = "google";
#elif UNITY_IPHONE
	const string market = "appstore";
#elif ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
	const string market = "google";
#endif
	const string domain = "wemejp";

	private JSONObject topObj = null;
	private UInt64 curTime = 0;
	public static string urlStore = string.Empty;
	public static string reviewUrl = string.Empty;

	public AsIntroFramework parentFramework = null;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void Enter()
	{
		gameObject.SetActiveRecursively( true);
		StartCoroutine( StartWemeGate());
	}

	public override void Exit()
	{
		StopCoroutine( "StartWemeGate");
		gameObject.SetActiveRecursively( false);
	}

	IEnumerator StartWemeGate()
	{
		yield return StartCoroutine( ConnectWemeGate());

		GetDomainInfo();
		CheckClientState();
	}

	IEnumerator ConnectWemeGate()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}{1}&domain={2}&game={3}&version={4}", AsNetworkDefine.WemeGateBaseUrl, market, domain, AsGameDefine.GAME_CODE, AsGameDefine.BUNDLE_VERSION);

		WWW www = new WWW( sb.ToString());
		yield return www;

		if( null != www.error)
			{
			Debug.Log("WemeGateManager ShutDown");
			AsUtil.ShutDown( string.Empty);
		}

		topObj = JSONObject.Parse( www.text);
		www.Dispose();
		www = null;

		curTime = UInt64.Parse( topObj[ "timestamp"].Str);
	}

	private void GetDomainInfo()
	{
		JSONObject domain = topObj.GetObject( "game_domain");
		if( null == domain)
			return;

#if UNITY_ANDROID
		urlStore = domain[ "urlOfGoogleStore"].Str;
		reviewUrl = domain[ "reviewUrlOfGoogleStore"].Str;
#elif UNITY_IPHONE
		urlStore = domain[ "urlOfAppStore"].Str;
		reviewUrl = domain[ "reviewUrlOfAppStore"].Str;
#endif
	}

	private void CheckClientState()
	{
		JSONObject client = topObj.GetObject( "client");
		if( null == client)
		{
			CheckMaintenance();
			return;
		}

		AsNetworkDefine.LOGIN_SERVER_IP = client[ "gameServerInfo"].Str;
		//AsNetworkDefine.PATCH_SERVER_ADDRESS = client[ "patchServerInfo"].Str;
		AsNetworkDefine.ImageServerAddress = client[ "imageServerInfo"].Str;

#if ( UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
		CheckNotice();
		return;
#endif

		string state = client[ "clientStatus"].Str;
		switch( state)
		{
		case "prepare":
			AsNotify.Instance.MessageBox( AssetbundleManager.Instance.GetPatcherString(50), "Prepare",
				AssetbundleManager.Instance.GetPatcherString(5), AssetbundleManager.Instance.GetPatcherString(5), this,
				"ClientState_Prepare", "ClientState_PrepareClose", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		case "test":
			AsNetworkDefine.GAME_MODE_REVIEW = true;
			CheckMaintenance();
			break;
		case "service":
			CheckMaintenance();
			break;
		case "upgraderecommend":
			AsNotify.Instance.MessageBox( AssetbundleManager.Instance.GetPatcherString(50), AssetbundleManager.Instance.GetPatcherString(51),
				AssetbundleManager.Instance.GetPatcherString(53), AssetbundleManager.Instance.GetPatcherString(52), this,
				"ClientState_Upgrade", "ClientState_Upgraderecommend_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		case "upgradeneed":
			AsNotify.Instance.MessageBox( AssetbundleManager.Instance.GetPatcherString(50), AssetbundleManager.Instance.GetPatcherString(51),
				AssetbundleManager.Instance.GetPatcherString(53), AssetbundleManager.Instance.GetPatcherString(53), this,
				"ClientState_Upgrade", "Quit", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		case "finish":
			break;
		default:
			break;
		}
	}

	private void ClientState_Prepare()
	{
		Quit();
	}

	private void ClientState_PrepareClose()
	{
		Quit();
	}

	private void ClientState_Test()
	{
		Quit();
	}

	private void ClientState_TestClose()
	{
	}

	private void ClientState_Upgrade()
	{
		Debug.Log( "url : " + urlStore);
		Application.OpenURL( urlStore);

		Quit();
	}

	private void ClientState_Upgraderecommend_Cancel()
	{
		CheckMaintenance();
	}

	private void CheckMaintenance()
	{
		JSONObject maintenance = topObj.GetObject( "maintenance");
		if( null == maintenance)
		{
			CheckNotice();
			return;
		}

		string maintenanceBegin = maintenance[ "begin"].Str;
		string maintenanceEnd = maintenance[ "end"].Str;
		UInt64 beginTime = UInt64.Parse( maintenanceBegin);
		UInt64 endTime = UInt64.Parse( maintenanceEnd);

		if( ( curTime < beginTime) || ( curTime > endTime))
		{
			CheckNotice();
			return;
		}

		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/GUI_PeriodicInspection/Prefab/PeriodicInspectionFramework")) as GameObject;
		Debug.Assert( null != go);
		AsInspectionDlg inspectionDlg = go.GetComponent<AsInspectionDlg>();
		Debug.Assert( null != inspectionDlg);

//		string domain = maintenance[ "domain"].Str;
//		string game = maintenance[ "game"].Str;
		string reason = maintenance[ "reason"].Str;
		string style = maintenance[ "maintenanceStyle"].Str;

		inspectionDlg.SetType( style);
		inspectionDlg.SetPeriod( maintenanceBegin, maintenanceEnd);
		inspectionDlg.SetReason( reason);
	}

	private void PostMaintenanceProc()
	{
		AsUtil.Quit();
	}

	private void CheckNotice()
	{
		JSONObject notices = topObj.GetObject( "notice");
		if( null == notices)
		{
			PostNoticeProc();
			return;
		}

		bool existNotice = false;
		IEnumerator<KeyValuePair<string, JSONValue>> testEnum = notices.GetEnumerator();
		while( testEnum.MoveNext())
		{
			JSONObject noticeData = JSONObject.Parse( testEnum.Current.Value.ToString());

			string id = noticeData["id"].Str;
			string showType = noticeData["showType"].Str;
			if( "once" == showType)
			{
				int isShowed = PlayerPrefs.GetInt( "notice" + id);
				existNotice |= ( 0 != isShowed) ? false : true;
			}
			else
			{
				existNotice = true;
			}
		}

		if( false == existNotice)
		{
			PostNoticeProc();
			return;
		}

		bool msgBoxPresented = false;

		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_IntroNotice")) as GameObject;
		Debug.Assert( null != go);
		AsIntroNoticeDlg noticeDlg = go.GetComponent<AsIntroNoticeDlg>();
		Debug.Assert( null != noticeDlg);
		noticeDlg.CloseCallback = PostNoticeProc;

		IEnumerator<KeyValuePair<string, JSONValue>> e = notices.GetEnumerator();
		while( e.MoveNext())
		{
			JSONObject noticeData = JSONObject.Parse( e.Current.Value.ToString());

			bool isShowed = false;
			string id = noticeData["id"].Str;
			string showType = noticeData["showType"].Str;
			if( "once" == showType)
			{
				int tempValue = PlayerPrefs.GetInt( "notice" + id);
				if( 0 != tempValue)
				{
					isShowed = true;
				}
				else
				{
					isShowed = false;
					PlayerPrefs.SetInt( "notice" + id, 1);
					PlayerPrefs.Save();
				}
			}

			if( false == isShowed)
			{
				string noticeBegin = noticeData["begin"].Str;
				string noticeEnd = noticeData["end"].Str;
				UInt64 beginTime = UInt64.Parse( noticeBegin);
				UInt64 endTime = UInt64.Parse( noticeEnd);

				if( ( curTime < beginTime) || ( curTime > endTime))
					continue;

				msgBoxPresented = true;

				noticeDlg.InsertNotice( noticeData["contents"].Str, noticeData["detailLink"].Str);
			}
		}

		if( false == msgBoxPresented)
			PostNoticeProc();
	}

	private void PostNoticeProc()
	{
		parentFramework.NextStep();
	}

	private void Quit()
	{
		AsUtil.Quit();
	}
}
