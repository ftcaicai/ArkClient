//#define _USE_BAND
using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Text;
using WmWemeSDK.JSON;

public class AsServerListFramework : AsFrameworkBase
{
	//[SerializeField] UIButton btnAccountManage = null;
	[SerializeField] UIScrollList listLeft = null;
	[SerializeField] UIScrollList listRight = null;
	[SerializeField] GameObject listItem = null;
	[SerializeField] SpriteText info = null;
	[SerializeField] SimpleSprite withBand = null;
	[SerializeField] UIButton btnLogout = null;
	
	private bool isConnected = false;
	private GameObject accountDlg = null;
	private GameObject withdrawDlg = null;
	#if UNITY_ANDROID
	const string market = "google";
	#elif UNITY_IPHONE
	const string market = "appstore";
	#elif ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
	const string market = "google";
	#endif
	const string domain = "wemejp";

	// Use this for initialization
	void Start()
	{
		#if _USE_BAND
		withBand.gameObject.SetActiveRecursively( true);	
		#else
		withBand.gameObject.SetActiveRecursively( false);		
		#endif
		
		AsServerListBtn.SetModifier( ModifyServerConnection);

	//	btnAccountManage.Text = AsTableManager.Instance.GetTbl_String(4013);
		info.Text = AsTableManager.Instance.GetTbl_String(2092);

		btnLogout.SetInputDelegate( OnLogoutBtnClick);
//		btnLogout.Text = AsTableManager.Instance.GetTbl_String(809);	// logout
		btnLogout.Text = AsTableManager.Instance.GetTbl_String(1757);	// back

		accountDlg = null;
		withdrawDlg = null;
		isConnected = false;

		StringBuilder sb = new StringBuilder();

		int count = AsServerListData.Instance.Count;
		int i = 0;
		for( i = 0; i < count; i++)
		{
			AS_LC_SERVERLIST_BODY data = AsServerListData.Instance.GetData(i);
			if( null == data)
				continue;

			int degSaturation = -1;
			sb.Remove( 0, sb.Length);
			sb.Insert ( 0, AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( data.szServerName)));
			sb.Append( ' ');
			sb.Append( AsTableManager.Instance.GetTbl_String(4011));
			if( ( 0 > data.nCurUser) || ( false == data.bGMOpen))
			{
				sb.Append( AsTableManager.Instance.GetTbl_String(1095));
				degSaturation = 0;
			}
			else
			{
				if( AsGameDefine.SERVER_USER_MAX <= data.nCurUser)
				{
					sb.AppendFormat( " - {0}{1}", Color.red, AsTableManager.Instance.GetTbl_String(1097));
					degSaturation = 3;
				}
				else if( ( AsGameDefine.SERVER_USER_SATURATION <= data.nCurUser) && ( AsGameDefine.SERVER_USER_MAX > data.nCurUser))
				{
					sb.AppendFormat( " - {0}{1}", Color.yellow, AsTableManager.Instance.GetTbl_String(1098));
					degSaturation = 2;
				}
				else
				{
					sb.AppendFormat( " - {0}{1}", Color.green, AsTableManager.Instance.GetTbl_String(1096));
					degSaturation = 1;
				}
			}

			UIScrollList list = ( 0 == ( i & 0x01)) ? listLeft : listRight;

			UIListItemContainer itemContainer = list.CreateItem( listItem) as UIListItemContainer;
			AsServerListBtn listBtn = itemContainer.gameObject.GetComponent<AsServerListBtn>();
			listBtn.serverData = data;
			listBtn.Saturation = degSaturation;
//			listBtn.SetRecommendState( ( i < 2) ? true : false);
			listBtn.SetRecommendState( ( i < AsServerListData.Instance.serverNewCnt) ? true : false);

//			// ilmeda, 20120821
			AsLanguageManager.Instance.SetFontFromSystemLanguage( itemContainer.spriteText);
			if( false == data.bPossibleCreate)
			{
//				sb.AppendFormat( " {0}({1})", Color.red, AsTableManager.Instance.GetTbl_String(1531));
				listBtn.CharacterCreateEnable( data.bPossibleCreate);
				itemContainer.spriteText.Color = Color.gray;
			}
			itemContainer.Text = sb.ToString();

			if( 0 == degSaturation)
				itemContainer.spriteText.Color = Color.gray;
		}

		if( 0 != ( i & 0x01))
		{
			UIListItemContainer itemContainer = listRight.CreateItem( listItem) as UIListItemContainer;
			AsServerListBtn listBtn = itemContainer.gameObject.GetComponent<AsServerListBtn>();
			listBtn.serverData = null;
			listBtn.Saturation = 0;
			itemContainer.Text = "";
			listBtn.SetRecommendState( false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if( true == listLeft.IsScrolling)
			listRight.ScrollPosition = listLeft.ScrollPosition;
		else if( true == listRight.IsScrolling)
			listLeft.ScrollPosition = listRight.ScrollPosition;
	}

	public override void PrevStep()
	{
	}

	public override void NextStep()
	{
	}

	public override void DeactivePrevStep()
	{
	}

	public override void OnNotify( NOTIFY_MSG msg)
	{
	}

	void OnLogoutBtnClick(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);


/*
			if( AsLogoutManager.Instance.ActiveLogoutCoroutine == true )
				return;
			
			AsLogoutManager.Instance.ProcessLogout ();
*/
			AsLogoutManager.Instance.GotoLoginSceneNotLogout();
		}
	}


	public void OnBack()
	{
#if false
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == isBack)
			return;

		isBack = true;

		CloseAccountDlg();
		CloseWithdrawDlg();

		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
#endif
	}

	public void OnNext()
	{
#if false
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == isConnected)
			return;

		if( null == prevSelect)
			return;

		UIListItemContainer itemContainer = prevSelect.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsServerListBtn listBtn = itemContainer.gameObject.GetComponent<AsServerListBtn>();
		if( 0 == listBtn.Saturation)
			return;

		if( true == listBtn.ConnectServer())
		{
			CloseAccountDlg();
			CloseWithdrawDlg();
			isConnected = true;
			//dopamin #15829
			if( true == AssetbundleManager.Instance.useAssetbundle)
			{
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterSelect");
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterCreate");
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ChannelSelect");
			}
		}
#endif
	}

	public void OnSelectLeft()
	{
#if ( !UNITY_EDITOR && ( UNITY_ANDROID || UNITY_IPHONE))
		StartCoroutine( "CheckMaintenance", true);
#else
		SelectLeft();
#endif
	}

	public void OnSelectRight()
	{
#if ( !UNITY_EDITOR && ( UNITY_ANDROID || UNITY_IPHONE))
		StartCoroutine( "CheckMaintenance", false);
#else
		SelectRight();
#endif
	}

/* dopamin 2014.09.22
	private void OnAccountManageBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null != accountDlg)
			return;

		accountDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/KAKAO/Prefab/GUI_KAKAOedit_Popup")) as GameObject;
		AsAccountDlg dlg = accountDlg.GetComponentInChildren<AsAccountDlg>();
		dlg.ParentObject = gameObject;
	}

	private void CloseAccountDlg()
	{
		if( null == accountDlg)
			return;

		GameObject.DestroyImmediate( accountDlg);
		accountDlg = null;
	}

	private void OpenWithdrawDlg()
	{
		if( null != withdrawDlg)
			return;

		withdrawDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/KAKAO/Prefab/GUI_KAKAOedit_Withdraw")) as GameObject;
		AsWithdrawDlg dlg = withdrawDlg.GetComponentInChildren<AsWithdrawDlg>();
		dlg.ParentObject = gameObject;
	}

	private void CloseWithdrawDlg()
	{
		if( null == withdrawDlg)
			return;

		GameObject.DestroyImmediate( withdrawDlg);
		withdrawDlg = null;
	}
	 */
	private void _Refresh()
	{
		isConnected = false;
	}

	void ModifyServerConnection( bool _connect)
	{
		isConnected = _connect;
	}
	
	public bool IsOpenServerListFrameworkDlg()
	{
		if( null != accountDlg || null != withdrawDlg)
			return true;
		
		if( null != AsServerListBtn.s_ServerListStandbyDlg)
			return true;
		
		return false;
	}
	
	public void CloseServerListFrameworkDlg()
	{
	//	CloseAccountDlg();
	//	CloseWithdrawDlg();
		
		if( null != AsServerListBtn.s_ServerListStandbyDlg)
			AsServerListBtn.s_ServerListStandbyDlg.Cancel();
	}
	
	IEnumerator CheckMaintenance( bool isLeft)
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0}{1}&domain={2}&game={3}&version={4}", AsNetworkDefine.WemeGateBaseUrl, market, domain, AsGameDefine.GAME_CODE, AsGameDefine.BUNDLE_VERSION);
		
		WWW www = new WWW( sb.ToString());
		yield return www;

		if( null != www.error)
		{
			Debug.Log( "WemeGateManager ShutDown");
			AsUtil.ShutDown( string.Empty);
			yield break;
		}
		
		JSONObject topObj = JSONObject.Parse( www.text);
		www.Dispose();
		www = null;
		
		JSONObject maintenance = topObj.GetObject( "maintenance");
		if( null != maintenance)
		{		
			UInt64 curTime = UInt64.Parse( topObj[ "timestamp"].Str);
			string maintenanceBegin = maintenance[ "begin"].Str;
			string maintenanceEnd = maintenance[ "end"].Str;
			UInt64 beginTime = UInt64.Parse( maintenanceBegin);
			UInt64 endTime = UInt64.Parse( maintenanceEnd);
			
			if( ( curTime >= beginTime) && ( curTime <= endTime))
				yield break;
		}
		
		if( true == isLeft)
			SelectLeft();
		else
			SelectRight();
	}
	
	void SelectLeft()
	{
		if( true == isConnected)
			return;
		
		UIRadioBtn clickItem = listLeft.LastClickedControl as UIRadioBtn;
		UIListItemContainer itemContainer = clickItem.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsServerListBtn listBtn = itemContainer.gameObject.GetComponent<AsServerListBtn>();
		
		if( 0 != listBtn.Saturation)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			clickItem.Value = true;
			
			if( true == listBtn.ConnectServer())
			{
				isConnected = true;
				
				if( true == AssetbundleManager.Instance.useAssetbundle)
				{
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterSelect");
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterCreate");
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ChannelSelect");
				}
			}
		}
	}
	
	void SelectRight()
	{
		if( true == isConnected)
			return;
		
		UIRadioBtn clickItem = listRight.LastClickedControl as UIRadioBtn;
		UIListItemContainer itemContainer = clickItem.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsServerListBtn listBtn = itemContainer.gameObject.GetComponent<AsServerListBtn>();
		
		if( 0 != listBtn.Saturation)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			clickItem.Value = true;
			
			if( true == listBtn.ConnectServer())
			{
				isConnected = true;
				
				if( true == AssetbundleManager.Instance.useAssetbundle)
				{
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterSelect");
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "CharacterCreate");
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ChannelSelect");
				}
			}
		}
	}
}
