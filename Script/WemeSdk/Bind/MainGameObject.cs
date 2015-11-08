//#define _USE_BAND
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WmWemeSDK.JSON;
using System;
using System.Text;
using System.Runtime.InteropServices;

public class MainGameObject : MonoBehaviour {
	public string logString { get;set; }
		

	private static MainGameObject instance_ = null;
	
	private int loginOrientation = 0;
	
	public static MainGameObject Instance
    {
        get
        {
            if( null == instance_ )
            {
                instance_ = FindObjectOfType( typeof( MainGameObject ) ) as MainGameObject;
                if( null == instance_ )
                {
                    Debug.Log( "Fail to get MainGameObject Instance" );
                }
            }
            return instance_;
        }
    }
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	
	}
	
	/*
	 * 
	 *  WEME SDK CODE
	 * 
	 */
	void OnApplicationPause(bool pauseStatus) {
		
		if(pauseStatus==true){
			//if login suspend
			if(UserData.Instance.isAuthorized==true){
					WemeManager.Instance.suspend(WemeSDKSuspendHandler);	
					
			}
		}else{
			//resume
			#if _USE_BAND	
			   //밴드 이용자 정보 호출  
			 BandSdkManager.Instance.getAliveProfile();
			#endif
			if(UserData.Instance.isAuthorized==true){
				WemeManager.Instance.resume(WemeSDKResumeHandler);	
			}
			
		}
    }
	void WemeAgreementHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			if(WemeManager.getBoolean("agreement",resultString)==true){
			//	setState(GameState.WmSDKStateWait);
			}
		}else{
			//finish();	
		}
	}
	void OnApplicationQuit() {
		//notification remove
		if(UserData.Instance.isAuthorized==true){
			WemeManager.Instance.suspend(WemeSDKSuspendHandler);	
     		WemeManager.Instance.removeWemeSdkNotification();
		}
    }
	
	void WemeSDKResumeHandler(string resultString){
		logString = "WemeSDKResumeHandler : " + " result : "  + resultString;
		Debug.Log( logString );
		#if _USE_BAND	
		if(!WemeManager.isSuccess( resultString))
		{
			JSONObject authDataJson = new JSONObject();
			authDataJson.Add("authProvider","band");
			authDataJson.Add("accessToken",BandSdkManager.Instance.AccessToken);
			authDataJson.Add("bandUserId",BandSdkManager.Instance.BandUserKey);
			authDataJson.Add("bandClientId",BandSdkManager.Instance.BAND_CLIENT_ID);
			authDataJson.Add("bandClientSecret",BandSdkManager.Instance.BAND_CLIENT_SECRET);
			UserData.Instance.authData = authDataJson.ToString();			
			Debug.Log( "BandProfileHandler authDataJson ::" + authDataJson.ToString());
			WemeSdkManager.GetMainGameObject.loginWemeSdk( BandResumeHandler );
		}
		#endif
	}
	void BandResumeHandler(string resultString){
		logString = "BandResumeHandler : " + " result : "  + resultString;
		Debug.Log( logString );
	
	}
	
	void WemeSDKSuspendHandler(string resultString){
		logString = "WemeSDKSuspendHandler : " + " result : "  + resultString;
		Debug.Log( logString );
	
	}
	
	// loading state startButtonClicked
	// init wemeSDK with client data
	// connect gate server,heartbeat server
	// check auto auth login
	// if has been login data than do autologin
	public void startButtonClicked(){
			// serverZone = kr_alpha,kr_beta,kr_real
		// gameCode = auth gameCode ex) 201180,201181
		// domain = weme,kakao
		// gameVersion = game version - mush match with sim admin
		// marketCode = appstore,google,tstore
		// @see wanna detail guide contact techdiv PM
//		string serverZone = WemeSdkManager.Instance.serverZone_;
		string serverZone = AsNetworkDefine.WemeServerZone;
		string gameCode = AsGameDefine.GAME_CODE.ToString();
//		string domain = WemeSdkManager.Instance.domain_;
		string domain = AsGameDefine.WEME_DOMAIN;
//		string gameVersion = WemeSdkManager.Instance.gameVersion_;
		string gameVersion = AsGameDefine.BUNDLE_VERSION;
		Debug.Log("gameVer sinon + "+ gameVersion);
		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		string marketCode = "google";
		#elif UNITY_ANDROID
		string marketCode = "google";
		#elif UNITY_IPHONE
		string marketCode = "appstore";
		#endif

		WemeManager.Instance.start( serverZone,gameCode, domain,gameVersion,marketCode,WemeSDKStartHandler);
		if( "kr_beta" == serverZone)
			WemeManager.Instance.setDebugMode();
			
	}
	/**
	  * start handler
	  * start finish
	  * 
	  * must check WemeManager isAuthorized
	  * true - auto login complete
	  * false - auto login falied
	  **/
	void WemeSDKStartHandler(string resultString){
	

		if( WemeManager.isSuccess( resultString))
		{
			//weme gateinfo first call and notification on
			logString = "GateInfo : " + " result : " + WemeManager.Instance.gateInfo();

			// enable GateInfo Notification
			WemeManager.Instance.enableNotification( "WemeSDKGateNotification",WemeSDKGateNotification);
			string resultAuthorized = WemeManager.Instance.isAuthorized();

			if( WemeManager.isSuccess( resultAuthorized))
			{
				if( WemeManager.getBoolean( "authorized",resultAuthorized))
					UserData.Instance.isAuthorized = true;
			}
		}
		
		#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
			UserData.Instance.isDeviceLogin = JSONObject.Parse(isDeviceLogin()).GetBoolean("deviceLogin");
		#endif
		
		GameObject go = GameObject.Find( "LoginFramework");
		if( null != go)
		{
			AsLoginScene loginScene = go.GetComponentInChildren<AsLoginScene>();
			if( null != loginScene)
				loginScene.StartWeme( resultString);
		}		
	
	}
	
	void processGateInfo(){
		string gateInfoString = WemeManager.Instance.gateInfo();
		if(WemeManager.isSuccess(gateInfoString)){
			JSONObject gateJson = JSONObject.Parse(gateInfoString);
			JSONObject notices =  gateJson.GetObject("gateInfo").GetObject("notice");
			IEnumerator enumerator =  notices.GetEnumerator();
			while (enumerator.MoveNext())
			{
	    		KeyValuePair<string, JSONValue> item = (KeyValuePair<string, JSONValue>)enumerator.Current;
				string result = item.Value.Obj.GetString("contents");
		//		showAlertView(result,XMLStringReader.Instance.GetText("weme_sdk_button_ok"),null,null);	
			}
		}
		
	}
	/**
	  * when success gateinfo result do something
	  * check isAuthorized
	  * true - change state to game
	  * false - change state to wait for login 
	  */
	void StartComplete(int buttonIndex){		
		//weme gateinfo first call and notification on
		logString = "GateInfo : " + " result : "  + WemeManager.Instance.gateInfo();	
		/**
		 * enable GateInfo Notification
		 * 
		 */
		WemeManager.Instance.enableNotification("WemeSDKGateNotification",WemeSDKGateNotification);
		string resultString = WemeManager.Instance.isAuthorized();
		
		if(WemeManager.isSuccess(resultString)){
			if(WemeManager.getBoolean("authorized",resultString)){
				UserData.Instance.isAuthorized = true;
		//		setState(MainGameObject.GameState.WmSDKStateGame);
			}else{
		//		setState(MainGameObject.GameState.WmSDKStateWait);
			}	
		}else{
		//	setState(MainGameObject.GameState.WmSDKStateWait);
		}
		#if !UNITY_EDITOR
			UserData.Instance.isDeviceLogin = JSONObject.Parse(isDeviceLogin()).GetBoolean("deviceLogin");
		#endif
	}
	
	void StartFailed(int buttonIndex){
		
	}
	
	/** 
	 * weme gateinfo notification
	 */
	void WemeSDKGateNotification(string resultString){
		logString = "WemeSDKGateNotification : " + " result : "  + resultString;	
		Debug.Log("WemeSDKGateNotification : " + resultString);
	}
	
	/**
	 * login facebook
	 * 
	 */
	public void loginFacebook( WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthFaceBookHandler, int _orientation = 0){
		
		/**
		 * facebook auth client keys
		 * 
		 * @params permissions permissions   https://developers.facebook.com/docs/reference/login/#permissions
		 */
		string appId = WemeSdkManager.Instance.facebook_appId_;
		string redirectUri = WemeSdkManager.Instance.facebook_redirectUri_;
		string[] permissions = {"read_friendlists","publish_actions","email"};
		if(_orientation==0){
			WemeOAuthManager.Instance.loginFacebook(appId,redirectUri,permissions,_WemeOauthFaceBookHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginFacebook(appId,redirectUri,permissions,orientation,_WemeOauthFaceBookHandler);	
		}
	}
	
	void WemeOauthFaceBookHandler(string resultString){
		
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthFaceBookHandler : " + " result : "  + resultString;
	}
	/*
	public void getFacebookFriends(){
		FacebookReqeustURL facebookSender = getFacebookSender();
		Dictionary<string,string> requestParams = new Dictionary<string, string>();
		requestParams.Add("access_token",JSONObject.Parse(resultString).GetObject("authData").GetString("accessToken"));
		facebookSender.getFriends(requestParams);			
	}
	
	public void getFacebookMe(){
		FacebookReqeustURL facebookSender = getFacebookSender();
		Dictionary<string,string> requestParams = new Dictionary<string, string>();
		requestParams.Add("access_token",JSONObject.Parse(resultString).GetObject("authData").GetString("accessToken"));
		facebookSender.getMe(requestParams);
	}
	
	public void getFacebookMeFeed(){
		FacebookReqeustURL facebookSender = getFacebookSender();
		Dictionary<string,string> requestParams = new Dictionary<string, string>();
		requestParams.Add("access_token",JSONObject.Parse(resultString).GetObject("authData").GetString("accessToken"));
		facebookSender.getMeFeed(requestParams);
	}
	
	FacebookReqeustURL getFacebookSender(){
		if(nowStateObject.GetComponent<FacebookReqeustURL>()!=null){
			return nowStateObject.GetComponent<FacebookReqeustURL>();
		}
		return  nowStateObject.AddComponent<FacebookReqeustURL>();
	}
	*/
	/**
	 *  login twitter
	 */
	public void loginTwitter(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthTwitterHandler, int _orientation = 0){
		
		/**
		 * 
		 *  twitter auth client keys
		 */
		string consumerKey = WemeSdkManager.Instance.twitter_consumerKey_;
		string consumerSecret = WemeSdkManager.Instance.twitter_consumerSecret_;
		string callbackUrl = WemeSdkManager.Instance.twitter_callbackUrl_;
		if(_orientation==0){
			WemeOAuthManager.Instance.loginTwitter(consumerKey,consumerSecret,callbackUrl,_WemeOauthTwitterHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginTwitter(consumerKey,consumerSecret,callbackUrl,orientation,_WemeOauthTwitterHandler);
		}
		
		
	}
	
	void WemeOauthTwitterHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthTwitterHandler : " + " result : "  + resultString;
	}
	
	/**
	 * login weme 
	 */
	public void loginWeme(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthWemeHandler, int _orientation = 0){
		
		/**
		 * weme auth client keys
		 */
		string clientId = WemeSdkManager.Instance.weme_clientId_;
		string clientSecret = WemeSdkManager.Instance.weme_clientSecret_;
		if(_orientation==0){
			WemeOAuthManager.Instance.loginWeme(clientId,clientSecret,_WemeOauthWemeHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginWeme(clientId,clientSecret,orientation,_WemeOauthWemeHandler);
		}
		
		
	}
	
	
	void WemeOauthWemeHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthWemeHandler : " + " result : "  + resultString;
	}
	/**
	 * login Google 
	 */
	public void loginGoogle(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthGoogleHandler, int _orientation = 0){
		
		/**
		 * google auth client keys
		 */
		string clientId = WemeSdkManager.Instance.google_clientId_;
		string clientSecret = WemeSdkManager.Instance.google_clientSecret_;
		string[] scopes = {"email","profile"};
		
		if(_orientation==0){
			WemeOAuthManager.Instance.loginGoogle(clientId,clientSecret,scopes,_WemeOauthGoogleHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginGoogle(clientId,clientSecret,scopes,orientation,_WemeOauthGoogleHandler);
		}
		
		
	}
	
	
	void WemeOauthGoogleHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthGoogleHandler : " + " result : "  + resultString;
	}
	
	/**
	 * login Naver 
	 */
	
	public void loginNaver(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthNaverHandler, int _orientation = 0){
		
		/**
		 * Naver auth client keys
		 */
		string clientId = WemeSdkManager.Instance.naver_clientId_;
		string clientSecret = WemeSdkManager.Instance.naver_clientSecret_;
		string redirectUri = WemeSdkManager.Instance.naver_redirectUri_;
		if(_orientation==0){
			WemeOAuthManager.Instance.loginNaver(clientId,clientSecret,redirectUri,_WemeOauthNaverHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginNaver(clientId,clientSecret,redirectUri,orientation,_WemeOauthNaverHandler);
		}
	}
	
	void WemeOauthNaverHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthGoogleHandler : " + " result : "  + resultString;
	}
	
	public void loginYahoo(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthYahooHandler, int _orientation = 0){
		/**
		 * Yahoo auth client keys
		 */
		string clientId = WemeSdkManager.Instance.yahoo_clientId_;
		string[] scopes = {"email","openid"};
		if(_orientation==0){
			WemeOAuthManager.Instance.loginYahooJp(clientId,scopes ,_WemeOauthYahooHandler);
		}else{
			WemeManager.WmOauthDisplayOrientation orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			if(_orientation==1){
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT;
			}else{
				orientation = WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE;
			}
			WemeOAuthManager.Instance.loginYahooJp(clientId,scopes,orientation ,_WemeOauthYahooHandler);
		}
	}
	
	void WemeOauthYahooHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthGoogleHandler : " + " result : "  + resultString;
	}
	// device login API
	public void loginDevice(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthDeviceHandler){
			WemeOAuthManager.Instance.loginDevice(_WemeOauthDeviceHandler);		
	}
	// Device Login agreement handler
	
	void WemeOauthDeviceHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
		}
		logString = "WemeOauthDeviceHandler : " + " result : "  + resultString;
	}
	
	public string isDeviceLogin(){
		return 	WemeOAuthManager.Instance.isDeviceLogin();
	}
	
	public void requestDeviceChange(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeOauthRequestDeviceChangeHandler){
		WemeOAuthManager.Instance.requestDeviceChange(_WemeOauthRequestDeviceChangeHandler);	
	}
		
	void WemeOauthRequestDeviceChangeHandler(string resultString){
		Debug.Log("WemeOauthRequestDeviceChangeHandler "+resultString);
		logString = "WemeOauthRequestDeviceChangeHandler "+resultString;
		if(WemeManager.isSuccess(resultString)){
			//if success logout and wemeSDKLogin
			string authData = JSONObject.Parse(resultString).GetObject("authData").ToString();
			if(authData.Length>0){
				UserData.Instance.authData = authData;
			//if device change ->>> logut and reLogin
				WemeManager.Instance.logout(WemeDeviceChangeLogoutHandler);
			}
			
		}
		logString = "WemeOauthRequestDeviceChangeHandler : " + " result : "  +  resultString;
	}
	void WemeDeviceChangeLogoutHandler( string resultString){
	
		if(WemeManager.isSuccess(resultString)){
			// if logout success login wemeSDK with Device
		//	setState(GameState.WmSDKStateWait);
			UserData.Instance.isAuthorized = false;
			UserData.Instance.isDeviceLogin=false;
			string authData = UserData.Instance.authData;
			WemeManager.Instance.login(authData,WemeLoginHandler);
		}
		logString = "WemeDeviceChangeLogoutHandler : " + " result : "  + resultString;
		
	}
	/**
	 * login Band
	 */
	public void loginBand(int _orientation){
		JSONObject authDataJson = new JSONObject();
		authDataJson.Add("authProvider","band");
		authDataJson.Add("accessToken","ZQAAAf4j9LsrFHkVjDzmXmfaDMrS-h_hUIHVnLoPXqn03bp8KlAr21bBrdXJE8Y20lk4IrrBU7cw_MPdnDkC0ED_nzM");
		authDataJson.Add("bandUserId","AACa331JqeBBo1EX5xvBdql9");
		authDataJson.Add("bandClientId","102206560");
		authDataJson.Add("bandClientSecret","N1lC8Vgy6McM-JnC20H7hHm75QOAVFH3");
		UserData.Instance.authData = authDataJson.ToString();
	
		Debug.Log("loginBand : " + UserData.Instance.authData);
		logString = "loginBand : " + " result : "  + UserData.Instance.authData;
	}
	/**
	 * weme sdk Login
	 * not auth login just sdk login
	 */
	public void loginWemeSdk(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeLoginHandler){
		
		/**
		 * weme sdk login
		 * using authProvider and authData
		 */
		string authData = UserData.Instance.authData;
		
		WemeManager.Instance.login(authData,_WemeLoginHandler);
	}
	
	//weme Login handler
	void WemeLoginHandler(string resultString){
		if(WemeManager.isSuccess(resultString)){
			UserData.Instance.isAuthorized = true;
	//		setState(GameState.WmSDKStateGame);
		}else{
			if(WemeManager.isBanPlayer(resultString)){
				string restrCode = WemeManager.getObject("error",resultString).GetObject("detail").GetString("restrCode");
				string restrReason = WemeManager.getObject("error",resultString).GetObject("detail").GetString("restrReason");
				double permitTime = WemeManager.getObject("error",resultString).GetObject("detail").GetNumber("permitTime");
				Debug.Log("WemeLoginHandler : ban player code : "+ restrCode + " restrReason : " + restrReason +  " permitTime : "  + permitTime);
			}
	//		setState(GameState.WmSDKStateWait);
			
		}
		logString = "WemeLoginHandler : " + " result : "  + resultString;
		#if !UNITY_EDITOR
			UserData.Instance.isDeviceLogin = JSONObject.Parse(isDeviceLogin()).GetBoolean("deviceLogin");
		#endif
	}
	
	/*
	 * get weme GateInfo 
	 * sync method
	 */
	
	public void getWemeGateData(){
		logString = "getWemeGateData : " + " result : "  + WemeManager.Instance.gateInfo();
		
	}
	/*
	 * logout wemesdk 
	 * weme sdk logout both auth logout
	 */
	public void logoutWemeSdk(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeLogoutHandler){
		WemeManager.Instance.logout(_WemeLogoutHandler);
	}
	void WemeLogoutHandler( string resultString){
	
		if(WemeManager.isSuccess(resultString)){
	//		setState(GameState.WmSDKStateWait);
			UserData.Instance.isAuthorized = false;
			UserData.Instance.authData="";
			UserData.Instance.isDeviceLogin=false;
			WemeSdkManager.Instance.IsGuest = false;
		}
		logString = "WemeLogoutHandler : " + " result : "  + resultString;
		
	}
	/*
	 * 
	 * withdraw wemesdk
	 * weme sdk withdraw both auth withdraw
	 * not disable allow facebook or twitter
	 */
	public void withdrawWemeSdk(WmInterfaceBroker.WmInterfaceBrokerDelegate _WemeWithdrawHandler){
		WemeManager.Instance.withdraw(_WemeWithdrawHandler);	
		
	}
	void WemeWithdrawHandler( string resultString){
		if(WemeManager.isSuccess(resultString)){
	//		setState(GameState.WmSDKStateWait);	
			UserData.Instance.isAuthorized = false;
			UserData.Instance.authData="";
		}
		
		logString = "WemeWithdrawHandler : " + " result : "  + resultString;
	}
	
	/*
	 * push allow setting
	 */
	public void setPushAllow(){
		string resultString = WemeManager.Instance.isAllowPushMessage();		
		if(WemeManager.isSuccess(resultString)){
			bool allow = JSONObject.Parse(resultString).GetBoolean("allow");
			WemeManager.Instance.requestAllowPushMessage(!allow,WemePushAllowHandler);
		}
	}
	void WemePushAllowHandler( string resultString){
		if(WemeManager.isSuccess(resultString)){
			//TODO something
		}
		logString = "WemePushAllowHandler : " + " result : "  + resultString;
	}
	
	/*
	 *  request gameServer
	 */
	public void requestGameServer(){
		/*
		 * @param uri server uri
		 * @param header header
		 * @param body body
		 */

		// FT MODEIFY
		if (1 == 1) {
			return;
		}

		JSONObject requsetJson = new JSONObject();
		JSONObject headerJson = new JSONObject();
		var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		headerJson.Add("starttime",string.Format("{0:0}",(System.DateTime.UtcNow - epochStart).TotalSeconds));
		JSONObject bodyJson = new JSONObject();
		string playerKeyResult = WemeManager.Instance.playerKey();
		if(WemeManager.isSuccess(playerKeyResult)){
			bodyJson.Add("playerKey",JSONObject.Parse(playerKeyResult).GetString("playerKey"));
		}
		string gameIdResult = WemeManager.Instance.gameId();
		if(WemeManager.isSuccess(gameIdResult)){
			bodyJson.Add("game",JSONObject.Parse(gameIdResult).GetString("gameId"));
		}
		bodyJson.Add("size",1);
		
		string requsetUri = "hpost://beta-dimple.weme.wemade.com:10080/1.0/getlog/player/game";
		requsetJson.Add("uri",requsetUri);
		requsetJson.Add("header",headerJson);
		requsetJson.Add ("body",bodyJson);
		WemeManager.Instance.request(requsetJson,WemeRequestGameServerHandler);
	}
	
	void WemeRequestGameServerHandler( string resultString){
		logString = "WemeRequestGameServerHandler : " + " result : "  + resultString;
	}
	/*
	 *  request getPromotionData
	 */
	public void getPromotion(){
		WemeManager.Instance.promotion(WemeGetPromotionHandler);
	}
	void WemeGetPromotionHandler(string resultString){
		UserData.Instance.promotionData = resultString;
		Debug.Log("WemeGetPromotionHandler : " + resultString);
		
		
		JSONArray exposeArray = JSONObject.Parse(resultString).GetObject("promotion").GetArray("exposeList");
		List<long> exposeList = new List<long>();
		
		foreach(JSONValue expose in exposeArray){
			long promoId = (long)expose.Obj.GetNumber("promoId");		
			exposeList.Add(promoId);
		}
		
		WemeManager.Instance.logExpose(exposeList,WemePromotionLogExposeHandler);
		logString = "WemeGetPromotionHandler : " + " result : "  + resultString;
	}
	void WemePromotionLogExposeHandler(string resultString){
		Debug.Log("WemePromotionLogExposeHandler : " + resultString);
	}
	/*
	 *  show bandPromotion use promotionUI
	 */
	public void showBannerPromotion(){
		WemeManager.Instance.showBannerPromotion(WemeManager.WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT,WemeBannerPromotionHandler);
	}
	
	void WemeBannerPromotionHandler(string resultString){
		Debug.Log("WemeBannerPromotionHandler : " + resultString);
		logString = "WemeBannerPromotionHandler : " + " result : "  + resultString;
	}
	/*
	 *  show popupPromotion use promotionUI
	 */
	public void showPopupPromotion(){
		WemeManager.Instance.showPopUpPromotion(WemeManager.WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT,WemeManager.WmPromotionPopupExposeStyle.WM_PROMOTION_POPUP_EXPOSE_STYLE_MULTIPLE_POPUP,WemePopupPromotionHandler);
	}
	
	void WemePopupPromotionHandler(string resultString){
		Debug.Log("WemePopupPromotionHandler : " + resultString);
		logString = "WemePopupPromotionHandler : " + " result : "  + resultString;
	}
	
	public void closePromotion(){
		WemeManager.Instance.closePromotion(1,WemePromotionClosePromotionHandler);	
		
	}
	
	void WemePromotionClosePromotionHandler(string resultString){
		Debug.Log("WemePromotionClosePromotionHandler : " + resultString);
		logString = "WemePromotionClosePromotionHandler : " + " result : "  + resultString;
	}
	public void sendClickBI(){
		WemeManager.Instance.sendPromotionClickedBI(1,WemePromotionSendClickedBIPromotionHandler);	
	}
	void WemePromotionSendClickedBIPromotionHandler(string resultString){
		Debug.Log("WemePromotionSendClickedBIPromotionHandler : " + resultString);
		logString = "WemePromotionSendClickedBIPromotionHandler : " + " result : "  + resultString;
	}
	
	public void showPlatformViewMain(){
		JSONObject additional = new JSONObject();
		// @see platformview guide
		byte[] bytesToEncode = Encoding.UTF8.GetBytes ("nick name");
		string encodedText = Convert.ToBase64String (bytesToEncode);
		additional.Add("nickname",encodedText);		
		WemeManager.Instance.showPlatformView(WemeManager.WmPlatformViewType.WM_PLATFORM_VIEW_MAIN,additional.ToString(),ShowPlatformViewHandler);
	}
	void ShowPlatformViewHandler(string jsonResult){
		Debug.Log("result ; " + jsonResult);
	}
	public void showPlatformViewEvent(){
		JSONObject additional = new JSONObject();
		// @see platformview guide
		byte[] bytesToEncode = Encoding.UTF8.GetBytes ("nick name");
		string encodedText = Convert.ToBase64String (bytesToEncode);
		additional.Add("nickname",encodedText);
		WemeManager.Instance.showPlatformView(WemeManager.WmPlatformViewType.WM_PLATFORM_VIEW_EVENT,additional.ToString(),ShowPlatformViewHandler);
	}
	public void showTermsView(){
		WemeManager.Instance.showTermsView(ShowTermsViewHandler);
	}
	void ShowTermsViewHandler(string jsonResult){
		Debug.Log("result ; " + jsonResult);
	}
	public void showPlatformViewWeb(string url){
		WemeManager.Instance.showWebView(url,ShowWebViewHandler);
	}
	void ShowWebViewHandler(string jsonResult){
		Debug.Log("result ; " + jsonResult);
	}	
	public void showPlatformViewPaymentstatement(){
		WemeManager.Instance.showPaymentstatementView(showPlatformViewPaymentstatementHandler);
	}
	void showPlatformViewPaymentstatementHandler(string jsonResult){
		Debug.Log("result ; " + jsonResult);
	}
}
	
	


