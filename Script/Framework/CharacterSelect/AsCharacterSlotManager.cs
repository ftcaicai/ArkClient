using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WmWemeSDK.JSON;
using System.Text;

public class AsCharacterSlotManager : MonoBehaviour
{
	public static int MAX_CHARACTER_NUM = 8;
	private static ArrayList slotArray = new ArrayList();

	private Vector2 touchPos = Vector2.zero;
	private bool touchMoved = false;
	public static bool CharacterSelected = false;
	public static int selectCharacterIndex = 1;
	public float DRAG_SENSITYVITY_RATIO = 0.1f;
	[SerializeField]UIButton backBtn = null;
	[SerializeField]SpriteText currentServer = null;
	bool showPromotionDlg = false;

	private bool toBack = false;

	public static int SlotCount
	{
		get { return slotArray.Count; }
	}

	//$yde
	public static bool PossibleCharCreate = true;

	public static AsCharacterSlot GetSlot( int index)
	{
		if( ( 0 > index) || ( slotArray.Count <= index))
			return null;

		return slotArray[ index] as AsCharacterSlot;
	}

	public static void UnlockSlot()
	{
		AsUserInfo.Instance.AddEmptySlot();

		AsCharacterSlot slot = slotArray[ slotArray.Count - 1] as AsCharacterSlot;
		slot.IsEmpty = true;
		slot.IsLocked = false;
		slot.SetDeleteState(0);
		int newSlotIndex = slot.ScreenIndex;

		if( AsGameDefine.MAX_CHARACTER_SLOT_COUNT > slotArray.Count)
		{
			UpdateScreenIndex( newSlotIndex + 1);

			GameObject slotResource = Instantiate( Resources.Load( "UI/GUI_SelectCreat/Prefab/characterslot")) as GameObject;
			Debug.Assert( null != slotResource);

			slot = slotResource.GetComponent<AsCharacterSlot>();
			Debug.Assert( null != slot);

			slot.Index = slotArray.Count;
			slot.ScreenIndex = newSlotIndex + 1;
			float posY = ( 1 == slot.ScreenIndex) ? 1.5f : 0.0f;
			slot.SetPosition( new Vector3( -AsCharacterSlot.SLOT_WIDTH + ( AsCharacterSlot.SLOT_WIDTH * slot.ScreenIndex) - 500.0f, posY, 0.0f));
			slot.IsEmpty = true;
			slot.IsLocked = true;
			slot.SetDeleteState(0);

			slotArray.Add( slot);
		}

		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1412), AsTableManager.Instance.GetTbl_String(1414),AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
	}

	private static void UpdateScreenIndex( int index)
	{
		foreach( AsCharacterSlot slot in slotArray)
		{
			if( slot.ScreenIndex >= index)
			{
				slot.ScreenIndex++;
				slot.SetPosition( new Vector3( -AsCharacterSlot.SLOT_WIDTH + ( AsCharacterSlot.SLOT_WIDTH * slot.ScreenIndex) - 500.0f, 0.0f, 0.0f));
			}
		}
	}

	public static bool IsModelAllLoaded()
	{
		foreach( AsCharacterSlot slot in slotArray)
		{
			if( true == slot.IsEmpty)
				continue;

			if( null == slot.Data)
				continue;

			//dopamin #17440
			if( eModelLoadingState.Finished == slot.Data.CheckModelLoadingState() && !slot.IsChangeScale)
			{
				slot.IsChangeScale = true;
				AsEffectManager.Instance.SetSizeAttachEffect(slot.Data.UniqueId, slot.charDummy.transform.localScale.x);
			}

			if( eModelLoadingState.Finished != slot.Data.CheckModelLoadingState())
				return false;
		}

		return true;
	}

	void OnDestroy()
	{
		AsEntityManager.Instance.RemoveAllEntities();
		AsInputManager.Instance.m_Activate = true;
		slotArray.Clear();
	}

	void OnEnable()
	{
		//$yde
		AsPetManager.Instance.Reserve_LoggedIn();
	}

	// Use this for initialization
	void Start()
	{
		backBtn.Text = AsTableManager.Instance.GetTbl_String(1810);
		StringBuilder sb = new StringBuilder( AsTableManager.Instance.GetTbl_String(980));
		sb.Append( Color.green.ToString());
		sb.Append( AsSocialManager.Instance.ServerName);
		sb.Append( ' ');
		sb.Append( AsTableManager.Instance.GetTbl_String(4011));
		currentServer.Text = sb.ToString();

		selectCharacterIndex = AsUserInfo.Instance.latestCharSlot;
		AsCharacterSlot.SLOT_WIDTH = ( Camera.mainCamera.orthographicSize * Camera.mainCamera.aspect * 2.0f) / 3.0f;
		AsGameMain.s_gameState = GAME_STATE.STATE_CHARACTER_SELECT;

		_CreateSlot();

		AsInputManager.Instance.m_Activate = false;
		CharacterSelected = false;

		AsLoadingIndigator.Instance.HideIndigator();
		toBack = false;

		//$yde
		AsUserInfo.Instance.ClearBuff();
		AsPromotionManager.Instance.Reserve_LoggedIn();
//		AsPetManager.Instance.Reserve_LoggedIn();

		AsUserInfo.Instance.isFirstConnect = false;

		if( AsReviewConditionManager.Instance.IsReviewCondition(eREVIEW_CONDITION.MARKETING_BANNER) == true )
			OpenNoticesDlg();
	}

	// Update is called once per frame
	void Update()
	{
		bool allLoaded = IsModelAllLoaded();
		if( true == allLoaded)
			AsLoadingIndigator.Instance.HideIndigator();
		else
			AsLoadingIndigator.Instance.ShowIndigator("");

		if( true == AsGameMain.isPopupExist)
			return;

		if( true == CharacterSelected)
			return;

		if( true == showPromotionDlg)
			return;

		if( 0 == Input.touchCount)
			return;

		Touch touch = Input.GetTouch(0);
		switch( touch.phase)
		{
		case TouchPhase.Began:
			{
				touchPos = touch.position;
				touchMoved = false;
			}
			break;
		case TouchPhase.Moved:
			{
				touchMoved = true;
			}
			break;
		case TouchPhase.Ended:
			{
				if( false == touchMoved)
					break;

				float dragAmount = touchPos.x - touch.position.x;
				if( dragAmount > ( Screen.width * DRAG_SENSITYVITY_RATIO))
					ShiftLeft();
				else if( dragAmount < -( Screen.width * DRAG_SENSITYVITY_RATIO))
					ShiftRight();
			}
			break;
		}
	}

	public static void ShiftLeft()
	{
		////2014.05.16
		if(/* ( true == WemeSdkManager.Instance.IsGuest) || */( null != AsCashStore.Instance))
			return;

		foreach( AsCharacterSlot slot in slotArray)
			slot.ShiftLeft();
	}

	public static void ShiftRight()
	{
		//2014.05.16
		if( /*( true == WemeSdkManager.Instance.IsGuest) ||*/ ( null != AsCashStore.Instance))
			return;

		foreach( AsCharacterSlot slot in slotArray)
			slot.ShiftRight();
	}

	public void UpdateSlotState( body_GC_CHAR_DELETE_RESULT data, bool setDelete)
	{
		AsCharacterSlot slot = slotArray[ data.nCharSlot] as AsCharacterSlot;

		if( true == setDelete)
		{
			slot.SetDeleteState( data.nRemainTime);

			sCHARVIEWDATA viewData = new sCHARVIEWDATA();
			viewData = AsUserInfo.Instance.GetCharacter( data.nCharSlot) as sCHARVIEWDATA;
			viewData.nRemainTime = data.nRemainTime;
			AsUserInfo.Instance.ReplaceSlotData( viewData);
		}
		else
		{
			slot.SetDeleteState(0);
		}
	}

	public void SetDeleteState( body_GC_CHAR_DELETE_RESULT data)
	{
		AsCharacterSlot slot = slotArray[ data.nCharSlot] as AsCharacterSlot;
		slot.SetDeleteState( data.nRemainTime);

		sCHARVIEWDATA viewData = new sCHARVIEWDATA();
		viewData = AsUserInfo.Instance.GetCharacter( data.nCharSlot) as sCHARVIEWDATA;
		viewData.nRemainTime = data.nRemainTime;
		AsUserInfo.Instance.ReplaceSlotData( viewData);
	}

	private void _CreateSlot()
	{
		int count = AsUserInfo.Instance.CharacterCount;
		for( int i = 0; i < count; i++)
		{
			GameObject slotResource = Instantiate( Resources.Load( "UI/GUI_SelectCreat/Prefab/CharacterSlot")) as GameObject;
			Debug.Assert( null != slotResource);

			AsCharacterSlot slot = slotResource.GetComponent<AsCharacterSlot>();
			Debug.Assert( null != slot);

			AsPacketHeader packet = AsUserInfo.Instance.GetCharacter(i);

			if( packet.GetType() == typeof( sCHARVIEWDATA))
			{
				sCHARVIEWDATA viewData = packet as sCHARVIEWDATA;

				Debug.Log( "AsCharacterSlotManager::_CreateSlot: viewData.nCharUniqKey" + viewData.nCharUniqKey);

				slot.IsEmpty = false;
				slot.SetDeleteState( viewData.nRemainTime);

				if( viewData.nCharUniqKey == AsUserInfo.Instance.nPrivateShopOpenCharUniqKey)
					slot.SetShopRemainTime( AsPStoreManager.Instance.RemainTime);
				else
					slot.ClearShopTime();
			}
			else if( packet.GetType() == typeof( AS_GC_CHAR_LOAD_RESULT_EMPTY))
			{
				slot.IsEmpty = true;
				slot.SetDeleteState(0);
			}

			slot.Index = i;
			int screenIndex = i - ( selectCharacterIndex - 1);
			if( -1 > screenIndex)
			{
				if( AsGameDefine.MAX_CHARACTER_SLOT_COUNT > count)
					screenIndex = count + screenIndex + 1;
				else
					screenIndex = count + screenIndex;
			}

			if( count >= AsGameDefine.MAX_CHARACTER_SLOT_COUNT)
			{
				if( count <= screenIndex)
					screenIndex = 0;
			}
			else
			{
				if( count < screenIndex)
					screenIndex = 0;
			}
			slot.ScreenIndex = screenIndex;
			float posY = ( 1 == screenIndex) ? 1.5f : 0.0f;
			slot.SetPosition( new Vector3( -AsCharacterSlot.SLOT_WIDTH + ( AsCharacterSlot.SLOT_WIDTH * slot.ScreenIndex) - 500.0f, posY, 0.0f));
			slot.IsLocked = false;

			slotArray.Add( slot);
		}

		if( AsGameDefine.MAX_CHARACTER_SLOT_COUNT > count)
		{
			GameObject slotResource = Instantiate( Resources.Load( "UI/GUI_SelectCreat/Prefab/CharacterSlot")) as GameObject;
			Debug.Assert( null != slotResource);

			AsCharacterSlot slot = slotResource.GetComponent<AsCharacterSlot>();
			Debug.Assert( null != slot);

			slot.Index = count;
			int screenIndex = count - ( selectCharacterIndex - 1);
			if( 0 == selectCharacterIndex)
				screenIndex = 0;
			if( -1 > screenIndex)
				screenIndex = count + screenIndex;
			slot.ScreenIndex = screenIndex;
			slot.SetPosition( new Vector3( -AsCharacterSlot.SLOT_WIDTH + ( AsCharacterSlot.SLOT_WIDTH * slot.ScreenIndex) - 500.0f, 0.0f, 0.0f));
			slot.IsEmpty = true;
			slot.IsLocked = true;
			slot.SetDeleteState(0);

			slotArray.Add( slot);
		}
	}

	private void OnBackBtn()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		if( true == CharacterSelected)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//2014.05.16
		//if( true == WemeSdkManager.Instance.IsGuest)
		//	return;

		if( true == toBack)
			return;

		//#15883 dopamin
		if( false == IsModelAllLoaded())
			return;

		toBack = true;
		AsUserInfo.Instance.Clear();
		AsNotify.Instance.CloseAllMessageBox();
		AsCharacterSlotManager.autoSelected = false;

		AsNetworkManager.Instance.ConnectToServer( AsNetworkDefine.LOGIN_SERVER_IP, AsNetworkDefine.LOGIN_SERVER_PORT, SOCKET_STATE.SS_LOGIN);
		if( true == AsNetworkManager.Instance.IsConnected())
		{
			if( true == AsUserInfo.Instance.isWemeCertified)
			{
				WemeRelogin();
			}
			else
			{
				AS_CL_LOGIN login = new AS_CL_LOGIN( AsUserInfo.curID, AsUserInfo.curPass,AsUserInfo.Instance.isAccountDeleteCancel);
				byte[] data = login.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);

				if( true == AssetbundleManager.Instance.useAssetbundle)
					AssetbundleManager.Instance.SceneAssetbundleLoadCache( "ServerSelect");

				AsUserInfo.Instance.isAccountDeleteCancel = false;
			}

			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
				AssetbundleManager.Instance.SceneAssetbundleLoadCache( "Login");
		}
	}

//	private void OnNextBtn()
//	{
//		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//
//		if( ( 0 > selectCharacterIndex) || ( 2 < selectCharacterIndex))
//			return;
//
//		AsCharacterSlot slot = slotArray[ selectCharacterIndex] as AsCharacterSlot;
//		slot.SendMessage( "OnBtnJoin");
//	}

	#region - private shop -
	public static void ShopDestroyed()
	{
		foreach(AsCharacterSlot slot in slotArray)
		{
			if( slot.GetType() == typeof(AsCharacterSlot))
			{
				slot.ClearShopTime();
			}
		}
	}
	#endregion

	private void WemeRelogin()
	{
		AsLoadingIndigator.Instance.ShowIndigator("");
		string strAuthData = WemeManager.Instance.authData();
		if( null == strAuthData)
		{
			Debug.LogError( "WemeRelogin()"+ strAuthData);
			return;
		}
		JSONObject jsonObject = JSONObject.Parse(strAuthData);
		JSONObject authDataJosn = jsonObject.GetObject("authData");
		if( null == authDataJosn)
		{
			Debug.Log( "WemeLogin()"+ jsonObject.ToString());
			return;
		}

		body_CL_WEMELOGIN wemeLogin = new body_CL_WEMELOGIN();
		JSONObject checkAuthJson = new JSONObject();
		JSONObject recordJson = new JSONObject();
		#if UNITY_EDITOR
			recordJson.Add("os", "window");
		#elif UNITY_ANDROID
			recordJson.Add("os", "android");
		#elif UNITY_IPHONE
			recordJson.Add("os", "ios");
		#endif
		//2014.05.16
		//if( WemeSdkManager.Instance.IsGuest)
		//	recordJson.Add("platform", "guest");
		//else
			recordJson.Add("platform", AsGameDefine.WEME_DOMAIN);

		if( authDataJosn["authProvider"].Str.CompareTo( "weme") == 0)
		{
			wemeLogin.eLoginType = (int)eLOGINTYPE.eLOGINTYPE_WEME;
			checkAuthJson.Add("wemeMemberNo", authDataJosn["wemeMemberNo"].Str);
			checkAuthJson.Add("accessToken", authDataJosn["accessToken"].Str);

			recordJson.Add("platform", AsGameDefine.WEME_DOMAIN);
			recordJson.Add("device", authDataJosn["uuid"].Str);
		}
		else if( authDataJosn["authProvider"].Str.CompareTo("facebook") == 0)
		{
			wemeLogin.eLoginType = (int)eLOGINTYPE.eLOGINTYPE_FACEBOOK;
			checkAuthJson.Add("fbUserId", authDataJosn["fbUserId"].Str);
			checkAuthJson.Add("accessToken", authDataJosn["accessToken"].Str);

			recordJson.Add("platform", "facebook");
			recordJson.Add("device", "");
		}
		else if( authDataJosn["authProvider"].Str.CompareTo( "google") == 0)
		{
			wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_GOOGLEPLUS;
			checkAuthJson.Add( "googleUserId", authDataJosn["googleUserId"].Str);
			checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);
			
			recordJson.Add( "platform", "google");
			recordJson.Add( "device", "");
		}
		else if( authDataJosn["authProvider"].Str.CompareTo( "naver") == 0)
		{
			wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_NAVER;
			checkAuthJson.Add( "naverUserId", authDataJosn["naverUserId"].Str);
			checkAuthJson.Add( "naverAliasId", "");
			checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);						
			
			recordJson.Add( "platform", "naver");
			recordJson.Add( "device", "");
		}
		else if( authDataJosn["authProvider"].Str.CompareTo( "band") == 0)
		{
			wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_NAVER_BAND;
			checkAuthJson.Add( "bandUserId",  authDataJosn["bandUserId"].Str);	
			checkAuthJson.Add( "accessToken", authDataJosn["accessToken"].Str);
		
			
			recordJson.Add( "platform", "band");
			recordJson.Add( "device", "");
		}	
		else if( authDataJosn["authProvider"].Str.CompareTo( "device") == 0)
		{
			wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_DEVICE;
			checkAuthJson.Add( "deviceId",  authDataJosn["deviceId"].Str);	
			checkAuthJson.Add( "deviceSeq", authDataJosn["deviceSeq"].Str);
		
			
			recordJson.Add( "platform", "device");
			recordJson.Add( "device", "");
		}	
		else if( authDataJosn["authProvider"].Str.CompareTo( "yahoojp") == 0)
		{
			wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_YAHOOJP;
			checkAuthJson.Add( "yahoojpUserId",  authDataJosn["yahoojpUserId"].Str);	
			checkAuthJson.Add( "yahoojpIdToken", authDataJosn["yahoojpIdToken"].Str);
			checkAuthJson.Add( "yahoojpNonce",   authDataJosn["yahoojpNonce"].Str);
			
			recordJson.Add( "platform", "yahoojp");
			recordJson.Add( "device", "");
		}	
		else if( authDataJosn["authProvider"].Str.CompareTo( "twitter") == 0)
			{
				wemeLogin.eLoginType = ( int)eLOGINTYPE.eLOGINTYPE_TWITTER;
				checkAuthJson.Add( "twitterUserId", 	  authDataJosn["twitterUserId"].Str);	
				checkAuthJson.Add( "accessToken",   	  authDataJosn["accessToken"].Str);
				checkAuthJson.Add( "accessTokenSecret",   authDataJosn["accessTokenSecret"].Str);
				checkAuthJson.Add( "consumerKey",  	      authDataJosn["consumerKey"].Str);
				checkAuthJson.Add( "consumerSecret",  	  authDataJosn["consumerSecret"].Str);
				
				recordJson.Add( "platform", "twitter");
				recordJson.Add( "device", "");
			}	
		
		byte[] srcAuth = System.Text.UTF8Encoding.UTF8.GetBytes(checkAuthJson.ToString());
		System.Buffer.BlockCopy(srcAuth, 0, wemeLogin.strCheckAuth, 0, srcAuth.Length);
		byte[] srcRecord = System.Text.UTF8Encoding.UTF8.GetBytes(recordJson.ToString());
		System.Buffer.BlockCopy(srcRecord, 0, wemeLogin.strRecord, 0, srcRecord.Length);

		Debug.Log( "strCheckAuth()" + checkAuthJson.ToString() + "srcRecord" +  recordJson.ToString());
		wemeLogin.bIsGuest = false;

		wemeLogin.bIsAccountDeleteCancel = AsUserInfo.Instance.isAccountDeleteCancel;
		AsUserInfo.Instance.isAccountDeleteCancel = false;

		byte[] packet = wemeLogin.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send(packet);

		WemeSdkManager.Instance.IsWemeLogin  = true;		
	}
	
	static public bool autoSelected = false;
	private void OpenNoticesDlg()
	{
		if( true == autoSelected)
			return;
		
		int savedDay = PlayerPrefs.GetInt( "EventView");
		if( savedDay == System.DateTime.Now.Day)
			return;

		GameObject go = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_IntroEvent");
		Debug.Assert( null != go);
		AsNoticesDlg noticeDlg = go.GetComponent<AsNoticesDlg>();
		Debug.Assert( null != noticeDlg);
		noticeDlg.CloseCallback = PostNoticeProc;
		showPromotionDlg = true;
	}

	private void PostNoticeProc()
	{
		showPromotionDlg = false;
	}
}
