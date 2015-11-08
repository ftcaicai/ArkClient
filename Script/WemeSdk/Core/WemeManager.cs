using UnityEngine;
using System.Collections;
using WmWemeSDK.JSON;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class WemeManager : MonoBehaviour {
	
	public enum WemeErrType : long
	{
		WM_ERR_SUCCESS = 0x00000000,					/**< @memberof WmError error code means success.(no error) */
		WM_ERR_AUTH_FAILURE = 0x10000001,				/**< @memberof WmError error code means authorization failed. */
		WM_ERR_INVALID_PARAMETER = 0x10000002,			/**< @memberof WmError error code means invalid parameters. */
		WM_ERR_NOT_SUPPORTED = 0x10000003,				/**< @memberof WmError error code means operation is not supported. */
		WM_ERR_NETWORK_FAILURE = 0x10000004,			/**< @memberof WmError error code means network failure. */
		WM_ERR_TIMEOUT = 0x10000005,					/**< @memberof WmError error code means time out. */
		WM_ERR_CONNECTION_FAILURE = 0x10000006,			/**< @memberof WmError error code means network connection failure. */
		WM_ERR_PARSING_FAILURE = 0x10000007,			/**< @memberof WmError error code means parsing failure. */
		WM_ERR_INVALID_VERSION = 0x10000008,			/**< @memberof WmError error code means client version has mismatched. */
		WM_ERR_UNDER_MAINTENANCE = 0x10000009,			/**< @memberof WmError error code means service is under maintenance. */
		WM_ERR_USER_CANCELED = 0x1000000A,				/**< @memberof WmError error code means user canceled operation. */
		WM_ERR_SERVICE_CLOSED = 0x1000000B,				/**< @memberof WmError error code means service has closed. */
		WM_ERR_IO_ERROR = 0x1000000C,					/**< @memberof WmError error code means I/O error. */
		WM_ERR_INVALID_SERVER_RESPONSE = 0x1000000D,	/**< @memberof WmError error code means server response is invalid. */	
		WM_ERR_NOT_INITIALIZED = 0x1000000E,			/**< @memberof WmError error code means component is not initialized. */
		WM_ERR_OPERATION_IN_PROGRESS = 0x1000000F,		/**< @memberof WmError error code means same operation is already in progress. */
		WM_ERR_RESTRICTED_PLAYER = 0x10000011,           /**< @memberof WmError error code means ban player. */
		WM_ERR_INVALID_PROMOTION = 0x20000001,           /**< @memberof WmError error code means invalid promotion. */
		WM_ERR_UNKNOWN = 0xFFFFFFFF,					/**< @memberof WmError error code means unknown error. */
	}
	
	public enum WmOauthDisplayOrientation
	{
		WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE,
		WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT,
	}
	
	public enum WmPromotionDisplayOrientation
	{
		WM_PROMOTION_DISPLAY_ORIENTATION_LANDSCAPE,
		WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT,
	}

	public enum WmPromotionPopupExposeStyle
	{
		WM_PROMOTION_POPUP_EXPOSE_STYLE_MULTIPLE_POPUP,
		WM_PROMOTION_POPUP_EXPOSE_STYLE_SINGLE_POPUP,
	}
	
	 /**
     * 플랫폼 뷰 타입
     */
    public enum WmPlatformViewType {
        /** 메인 타입 플랫폼 뷰 */
        WM_PLATFORM_VIEW_MAIN,
        /** 이벤트 타입 플랫폼 뷰 */
        WM_PLATFORM_VIEW_EVENT,
    }
	#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void setEnableNotification(string returnObject,string returnMethod);
	
		[DllImport("__Internal")]
		private static extern void removeNotification();
	#endif
	
	
	// Use this for initialization
	private static WemeManager instance_ = null;

	public static WemeManager Instance
    {
        get
        {
            if( null == instance_ )
            {
                instance_ = FindObjectOfType( typeof( WemeManager ) ) as WemeManager;
                if( null == instance_ )
                {
                    Debug.LogError( "Fail to get WemeManager Instance" );
                }
            }
            return instance_;
        }
    }
	
	public const string DOMAIN = "WemeManager";
	
	public delegate void WemeManagerDelegate(string jsonResult);
	
	public static event WemeManagerDelegate OnEventNotify; 
	
	public GameObject wemeObject = null;
	
	private const string JSON_KEY_URI = "uri";
	
	private const string JSON_KEY_PARAMS = "params";
	
	private const string JSON_KEY_AUTHDATA = "authData";
	
	private const string JSON_VALUE_PREFIX_URI = "weme://";
	private const string JSON_PUSH_VALUE_PREFIX_URI = "wemePush://";
	private const string JSON_PROMOTIONUI_VALUE_PREFIX_URI = "wemePromotionUI://";
	private const string JSON_PLATFORMVIEW_VALUE_PREFIX_URI = "wemePlatformView://";
	
	private const string WM_HOST_START = "start";
	private const string WM_HOST_LOGIN = "login";
	private const string WM_HOST_LOGOUT = "logout";
	private const string WM_HOST_WITHDRAW = "withdraw";
	private const string WM_HOST_SUSPEND = "suspend";
	private const string WM_HOST_RESUME = "resume";
	private const string WM_HOST_REQUEST = "request";
	private const string WM_HOST_ISAUTHORIZED = "isAuthorized";
	private const string WM_HOST_GATEINFO = "gateInfo";
	private const string WM_HOST_CONFIGURATION = "configuration";
	private const string WM_HOST_AUTHDATA = "authData";
	private const string WM_HOST_GAMEID = "gameId";
	private const string WM_HOST_PLAYERKEY = "playerKey";
	private const string WM_HOST_SDKVERSION = "sdkVersion";
	private const string WM_PUSH_HOST_REQUESTALLOWPUSHMESSAGE = "requestAllowPushMessage";
	private const string WM_PUSH_HOST_ISALLOWPUSHMESSAGE = "isAllowPushMessage";
	
	private const string WM_HOST_DEBUGMODE = "debugMode";
	
	private const string WM_HOST_PROMOTION = "promotion";
	private const string WM_HOST_REFRESHPROMOTION = "refreshPromotion";
	
	private const string WM_HOST_ENDINGPOPUP_PROMOTION = "endingPopupPromotion";
	private const string WM_HOST_EXPIRE_ENDINGPOPUP_PROMOTION = "expireEndingPopupPromotion";
	
	private const string WM_HOST_SHOWBANDPROMOTION = "showBannerPromotion";
	private const string WM_HOST_SHOWPOPUPPROMOTION	= "showPopupPromotion";
	private const string WM_HOST_SHOWSTATIONPROMOTION	= "showStationPromotion";
	private const string WM_HOST_SHOWENDINGPOPUPPROMOTION	= "showEndingPopupPromotion";
	private const string WM_HOST_HIDEBANDPROMOTION = "hideBannerPromotion";
	private const string WM_HOST_HIDEPOPUPPROMOTION	= "hidePopupPromotion";
	private const string WM_HOST_HIDEENDINGPOPUPPROMOTION	= "hideEndingPopupPromotion";
	
	private const string WM_HOST_PROMOTION_CLOSE = "closePromotion";
	private const string WM_HOST_PROMOTION_SENDBI = "sendPromotionClickedBI";
	private const string WM_HOST_PROMOTION_SEND_EXPOSELOG = "sendPromotionExposeLogs";
	
	private const string WM_HOST_CHECKAGREEMENT = "checkAgreement";
	private const string WM_HOST_SHOWAGREEMENT = "showAgreementView";
	private const string WM_HOST_SHOWTERMS = "showTermsView";
	private const string WM_HOST_SHOWPLATFORM = "showPlatformView";
	private const string WM_HOST_SHOWWEB = "showWebView";
	private const string WM_HOST_SHOWPAYMENTSTATEMENT = "showPaymentstatementView";
	private const string WM_HOST_SHOWINVITEFRIEND = "showInviteFriendView";
	
	private const string WM_HOST_SETAGREEMENT = "setAgreement";
	
	// Update is called once per frame
	
	void Awake(){
		
	}
	
	// wemeManager set ObjectName for Communication to native plugins
	public void initWemeManager(string gameObjectName){
		GameObject gameObj = GameObject.Find(gameObjectName);
		gameObj.AddComponent<WmInterfaceBroker>();
		gameObj.AddComponent<WemeOAuthManager>();
		wemeObject = gameObj;
	}
	public void initWemeManager(GameObject gameObj){
		
		gameObj.AddComponent<WmInterfaceBroker>();
		gameObj.AddComponent<WemeOAuthManager>();
		wemeObject = gameObj;
	}
	/**
	 * Weme sdk start
	 * 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void start(string serverZone,string gameCode,string domain,string gameVersion,string marketCode,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(isEmpty(serverZone)==true||isEmpty(gameCode)==true||
			isEmpty(domain)==true||isEmpty(gameVersion)==true||
			isEmpty(marketCode)==true){
			Debug.LogError("Must not null Application default datas");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_START);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("serverZone",serverZone);
		paramsObject.Add("gameCode",gameCode);
		paramsObject.Add ("domain",domain);
		paramsObject.Add("gameVersion",gameVersion);
		paramsObject.Add ("marketCode",marketCode);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginWeme
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void login(string oAuthString,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(isEmpty(oAuthString)){
			Debug.LogError("Must not null oAuth Data");
			return;
		}
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_LOGIN);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add (JSON_KEY_AUTHDATA,JSONObject.Parse(oAuthString));
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk logoutWemeSdk
	 * 
	 * 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void logout(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_LOGOUT);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk withdrawWemeSdk
	 * 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void withdraw(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_WITHDRAW);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk suspendWemeSdk
	 * 
	 */
	public void suspend(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_SUSPEND);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk resumeWemeSdk
	 * 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void resume(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_RESUME);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	/**
	 * 
	 * Weme sdk requestGameServer
	 * 
	 * 
	 * @param JSONObject sendDataJson
	 */
	public void request(JSONObject jsonParams,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_REQUEST);
		jsonObject.Add(JSON_KEY_PARAMS,jsonParams);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	
	}
	
	/**
	 * Weme sdk isAuthorized
	 * 
	 */
	public string isAuthorized(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_ISAUTHORIZED);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	/**
	 * Weme sdk getWemeGateData
	 * 
	 * @return json string
	 */
	public string gateInfo(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_GATEINFO);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	/**
	 * Weme sdk getWemeConfigurationData
	 * 
	 * @return json string
	 */
	public string configuration(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_CONFIGURATION);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	/**
	 * Weme sdk getWemeAuthData
	 * 
	 * @return json string
	 */
	public string authData(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_AUTHDATA);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	/**
	 * Weme sdk getGameId
	 * 
	 */
	public string gameId(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_GAMEID);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	/**
	 * Weme sdk getWemePlayerKey
	 * 
	 * @return string playerkey ex) 82747828939.kakao
	 */
	public string playerKey(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_PLAYERKEY);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	/**
	 * Weme sdk getGameId
	 * 
	 */
	public string sdkVersion(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_SDKVERSION);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	/**
	 * Weme sdk getWemePushAllow
	 * 
	 * @return bool pushArrow
	 */
	public string isAllowPushMessage(){
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PUSH_VALUE_PREFIX_URI+WM_PUSH_HOST_ISALLOWPUSHMESSAGE);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
		
	}
	
	/**
	 * Weme sdk setWemePushAllow
	 * 
	 * @param bool pushAllow set push allow
	 * @param WmInterfaceBrokerDelegate
	 */
	public void requestAllowPushMessage(bool pushAllow,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PUSH_VALUE_PREFIX_URI+WM_PUSH_HOST_REQUESTALLOWPUSHMESSAGE);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("allow",pushAllow);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	/**
	 * Weme sdk setDebugMode
	 * 
	 */
	public string setDebugMode(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_DEBUGMODE);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	public void promotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_PROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void refreshPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_REFRESHPROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void endingPopupPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_ENDINGPOPUP_PROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void expireEndingPopupPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_EXPIRE_ENDINGPOPUP_PROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void closePromotion(long promotionId,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_PROMOTION_CLOSE);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("promotionId",promotionId);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void sendPromotionClickedBI(long promotionId,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_PROMOTION_SENDBI);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("promotionId",promotionId);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void showBannerPromotion(WmPromotionDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_SHOWBANDPROMOTION);
		JSONObject paramsObject = new JSONObject();
		string orientationString = null;
		if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void showPopUpPromotion(WmPromotionDisplayOrientation orientation,WmPromotionPopupExposeStyle style,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_SHOWPOPUPPROMOTION);
		JSONObject paramsObject = new JSONObject();
		string orientationString = null;
		if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		paramsObject.Add("orientation",orientationString);
		string styleString = null;
		if(WmPromotionPopupExposeStyle.WM_PROMOTION_POPUP_EXPOSE_STYLE_MULTIPLE_POPUP == style){
			styleString = "multiple";
		}else if(WmPromotionPopupExposeStyle.WM_PROMOTION_POPUP_EXPOSE_STYLE_SINGLE_POPUP == style){
			styleString = "single";
		}
		paramsObject.Add("style",styleString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void showEndingPopUpPromotion(WmPromotionDisplayOrientation orientation, WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject paramsObject = new JSONObject();
		string orientationString = null;
		if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		paramsObject.Add("orientation",orientationString);

		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI, JSON_PROMOTIONUI_VALUE_PREFIX_URI + WM_HOST_SHOWENDINGPOPUPPROMOTION);
		jsonObject.Add(JSON_KEY_PARAMS, paramsObject);

		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(), callback);
	}

	public void showStationPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_SHOWSTATIONPROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void hideBannerPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_HIDEBANDPROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void hidePopUpPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_HIDEPOPUPPROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void hideEndingPopUpPromotion(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PROMOTIONUI_VALUE_PREFIX_URI+WM_HOST_HIDEENDINGPOPUPPROMOTION);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public void logExpose(List<long> exposeList, WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_PROMOTION_SEND_EXPOSELOG);
		JSONObject paramsObject = new JSONObject();
		JSONArray array = new JSONArray();
		foreach(long promoid in exposeList){
			array.Add(promoid);	
		}
		paramsObject.Add("promoIds",array);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk checkAgreement
	 */
	public string checkAgreement(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_CHECKAGREEMENT);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());	
	}
	/**
	 * Weme sdk showAgreement
	 */
	public void showAgreementView(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWAGREEMENT);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk showTermsView
	 */
	public void showTermsView(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWTERMS);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	/**
	 * Weme sdk showPlatformView
	 */
	public void showPlatformView(WmPlatformViewType type,string additionalParamString, WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWPLATFORM);
		JSONObject paramsObject = new JSONObject();
		string typeString = null;
		if(WmPlatformViewType.WM_PLATFORM_VIEW_MAIN == type){
			typeString = "main";
		}else if(WmPlatformViewType.WM_PLATFORM_VIEW_EVENT == type){
			typeString = "event";
		}
		paramsObject.Add("type",typeString);
		paramsObject.Add("additionalParams",JSONObject.Parse(additionalParamString));
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	/**
	 * Weme sdk showWebView
	 */
	public void showWebView(string urlString, WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWWEB);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("url",urlString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}

	/**
	 * Weme sdk showPaymentstatementView
	 */
	public void showPaymentstatementView(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWPAYMENTSTATEMENT);
		Debug.Log("storyzero : " + jsonObject.ToString());
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}

	/**
	 * Weme sdk setAgreement
	 */
	public string setAgreement(bool agreement){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SETAGREEMENT);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("agreement",agreement);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}

	/**
	 * Weme sdk showInviteFriendView
	 */
	public void showInviteFriendView(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_PLATFORMVIEW_VALUE_PREFIX_URI+WM_HOST_SHOWINVITEFRIEND);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	/**
	 * Weme sdk removeWemeSdkNotification
	 * 
	 */
	public void removeWemeSdkNotification(){
		
	#if UNITY_EDITOR
		if(RuntimePlatform.OSXEditor==Application.platform||RuntimePlatform.WindowsEditor==Application.platform){
	//		WemeSDKDllImport.removeNotificationEditor();
			/*if(RuntimePlatform.OSXEditor==Application.platform){
				#if UNITY_EDITOR
					removeNotificationEditor();
				#endif
			}else{
			}*/
		}
	
	#elif UNITY_ANDROID
		WmInterfaceBroker.getInstance.broker.CallStatic("removeNotification");
	
	#elif UNITY_IPHONE
		removeNotification();
	#endif
	
	}
	
	
	/**
	 * Weme sdk enableNotification
	 * 
	 * @param WemeGateInfoNotification
	 */
	public void enableNotification(string methodName,WemeManagerDelegate callback){
		OnEventNotify -= callback;
		OnEventNotify += callback;
	#if UNITY_EDITOR
		if(RuntimePlatform.OSXEditor==Application.platform||RuntimePlatform.WindowsEditor==Application.platform){
	//		WemeSDKDllImport.setEnableNotificationEditor(WemeManager.Instance.wemeObject.name,methodName);
			/*
			if(RuntimePlatform.OSXEditor==Application.platform){
				#if UNITY_EDITOR
					setEnableNotificationEditor(WemeManager.Instance.wemeObject.name,methodName);
					
				#endif
			}else{
			}
			*/
		}
	#elif UNITY_ANDROID
		WmInterfaceBroker.getInstance.broker.CallStatic("setEnableNotification",WemeManager.Instance.wemeObject.name,methodName);
	
	#elif UNITY_IPHONE
		setEnableNotification(WemeManager.Instance.wemeObject.name,methodName);
	#endif
	}
	
	/**
	 * Weme sdk reciver WmCoreNotificationRefreshedGateInformation
	 * 
	 * 
	 * @param string  gateinfo jsonString
	 */
	void WemeSDKGateNotification(string resultString){
		OnEventNotify(resultString);
	}
	
	public static bool isBanPlayer(string jsonResult){
		if(isEmpty(jsonResult)){
			return false;	
		}
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			long errorCode = (long)(jsonObj.GetObject("error").GetNumber("code"));
			if(errorCode==(long)WemeManager.WemeErrType.WM_ERR_RESTRICTED_PLAYER){
				return true;
			}else{
				return false;	
			}
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return false;
		}
	}
	
	public static int getMaintenanceRemainTime(string jsonMaintenance){
		if(isEmpty(jsonMaintenance)){
			return 0;	
		}
		try{
			JSONObject maintenance = JSONObject.Parse(jsonMaintenance);
			DateTime end = DateTime.ParseExact(maintenance.GetString("end").ToString(),"yyyyMMddHHmmss",System.Globalization.CultureInfo.InvariantCulture);
			DateTime now = System.DateTime.Now;
			int diff = (end - now).Minutes;
			if(diff > 0)
				return diff;
			else
				return 0;
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return 0;
		}
	}

	/**
	 * common code
	 */
	public static bool isEmpty(string checkString){
		if(checkString==null){
			return true;
		}
		if(checkString.Length<1){
			return true;
		}
		return false;	
	}

	public static bool isSuccess(string jsonResult){
		if(isEmpty(jsonResult)){
			return false;	
		}
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			long errorCode = (long)(jsonObj.GetObject("error").GetNumber("code"));
			if(errorCode==(long)WemeManager.WemeErrType.WM_ERR_SUCCESS){
				return true;
			}else{
				return false;	
			}
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return false;
		}
	}
	
	public static bool getBoolean(string key,string jsonResult,bool defValue){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
			return jsonObj.GetBoolean(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
		}
		return defValue;
	}
	
	public static bool getBoolean(string key,string jsonResult){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return jsonObj.GetBoolean(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return false;
		}
	}

	public static double getNumber(string key,string jsonResult,long defValue){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return jsonObj.GetNumber(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
		}
		return defValue;	
	}
	
	public static double getNumber(string key,string jsonResult){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return jsonObj.GetNumber(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return 0;
		}
		
	}
	
	public static string getString(string key,string jsonResult,string defValue){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return jsonObj.GetString(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			
		}
		return defValue;
	}
	public static string getString(string key,string jsonResult){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return jsonObj.GetString(key);			
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return "";
		}
	}
	public static JSONObject getObject(string key,string jsonResult){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return (JSONObject)jsonObj.GetObject(key);
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return null;
		}
	}
	
	public static JSONArray getArray(string key,string jsonResult){
		try{
			JSONObject jsonObj = JSONObject.Parse(jsonResult);
		
			return (JSONArray)jsonObj.GetArray(key);
		}catch(Exception e){
			Debug.LogError(DOMAIN + "Exception : "+e.Message);
			return null;
		}
	}
	
	public static class WmError{
		public static JSONObject ErrorWithDesc(long code,string desc){
			JSONObject errjson = new JSONObject();
			errjson.Add("code",code);
			errjson.Add("desc",desc);
			return errjson;
		}
		public static JSONObject Complete(){
			JSONObject errjson = new JSONObject();
			errjson.Add("code",0);
			errjson.Add("desc","success");
			return errjson;
		}
	}
}


public static class WmIntefaceBrokerKey
{
	// weme interfacebroker
	public readonly static string WmInterfaceBrokerUri = "uri";
	public readonly static string WmInterfaceBrokerError = "error";
	
	public readonly static string WmInterfaceBrokerParams = "params";
	public readonly static string WmInterfaceBrokerAuthData = "authData";
	public readonly static string WmInterfaceBrokerResponse = "response";
	public readonly static string WmInterfaceBrokerAuthorized = "authorized";
	public readonly static string WmInterfaceBrokerGateInfo = "gateInfo";
	public readonly static string WmInterfaceBrokerConfiguration = "configuration";
	public readonly static string WmInterfaceBrokerGameId = "gameId";
	public readonly static string WmInterfaceBrokerPlayerKey = "playerKey";
	public readonly static string WmInterfaceBrokerSdkVersion = "sdkVersion";
	public readonly static string WmInterfaceBrokerAllow = "allow";
	public readonly static string WmInterfaceBrokerPromotion = "promotion";
	
	// weme interfacebroker
	public static class WmErrorKey
	{
		public readonly static string WmErrorCode = "code";
		public readonly static string WmErrorDesc = "desc";
		public readonly static string WmErrorDetail = "detail";
		
		public class WmErrorDetailKey
		{
			public readonly static string WmErrorRestrCode = "restrCode";
			public readonly static string WmErrorRestrReason = "restrReason";
			public readonly static string WmErrorPermitTime = "permitTime";
		}
	}
	
	public static class WmRequestKey
	{
		public readonly static string WmRequestUri = "uri";
		public readonly static string WmRequestHeader = "header";
		public readonly static string WmRequestBody = "body";
		
	}
	
	public static class WmConfigurationKey
	{
		public readonly static string WmConfigKeyServerZone = "WmCoreConfigKeyServerZone";
		public 	readonly static string  WmCoreConfigKeyGameCode = "WmCoreConfigKeyGameCode";
		public readonly static string  WmCoreConfigKeyGameVersion = "WmCoreConfigKeyGameVersion";
		public readonly static string  WmCoreConfigKeyGameDomain = "WmCoreConfigKeyGameDomain";
		public readonly static string  WmCoreConfigKeyMarketCode = "WmCoreConfigKeyMarketCode";
	}

	// wemeOauth interfacebroker
	public static class WmAuthDataKey
	{
		public static class WmWemeAuthDataKey
		{
			public readonly static string WmWemeAuthAuthProvider = "authProvider";
			public readonly static string WmWemeAuthWemeClientId = "wemeClientId";
			public readonly static string WmWemeAuthWemeClientSecret = "wemeClientSecret";
			public readonly static string WmWemeAuthUuid = "uuid";
			public readonly static string WmWemeAuthAccessToken = "accessToken";
			public readonly static string WmWemeAuthUserId = "userId";
			public readonly static string WmWemeAuthExpires_in = "expires_in";
		}
		
		public static class WmFacebookAuthDataKey
		{
			public readonly static string WmWemeAuthAuthProvider = "authProvider";
			public readonly static string WmWemeAuthAccessToken = "accessToken";
			public readonly static string WmWemeAuthFbUserId = "fbUserId";
		}
		
		public static class WmTwitterAuthDataKey
		{
			public readonly static string WmWemeAuthAuthProvider = "authProvider";
			public readonly static string WmWemeAuthConsumerKey = "consumerKey";
			public readonly static string WmWemeAuthConsumerSecret = "consumerSecret";
			public readonly static string WmWemeAuthAccessToken = "accessToken";
			public readonly static string WmWemeAuthAccessTokenSecret = "accessTokenSecret";
			public readonly static string WmWemeAuthTwitterUserId = "twitterUserId";
		}
			
	}
	
	// weme Kakao interfacebroker
	public readonly static string WmKakaoGuestLogin = "guestLogin";
	public readonly static string WmKakaoAuthorized = "authorized";
	public readonly static string WmKakaoLocaluser = "localuser";
	public readonly static string WmKakaoFriends = "friends";
	public readonly static string WmKakaoKKASdkVer = "kKASdkVer";
	public readonly static string WmKakaoOldUser = "oldUser";
		
	public static class WmKakaoAuthDataKey{
		public readonly static string WmWemeAuthAuthProvider = "authProvider";
		public readonly static string WmWemeAuthAccessToken = "accessToken";
		public readonly static string WmWemeAuthKakaoClientId = "kakaoClientId";
		public readonly static string WmWemeAuthKakaoSdkVer = "kakaoSdkVer";
		public readonly static string WmWemeAuthKakaoUserId = "kakaoUserId";
	}
	
}
	
public static class WmGateKey
{
	public readonly static string WmGateErrorDomain = "WmGateErrorDomain";	
	
	public static class WmGateClientKey
	{
		public readonly static string WmGateClientKeyGameCode = "game";
		public readonly static string WmGateClientKeyGameDomain = "domain";
		public readonly static string WmGateClientKeyMarket = "market";
		public readonly static string WmGateClientKeyVersion = "version";
		public readonly static string WmGateClientKeyStatus = "clientStatus";
		public readonly static string WmGateClientKeyOSType = "osType";
		public readonly static string WmGateClientKeyRegDate = "regDate";
		public readonly static string WmGateClientKeyRegUser = "regUser";
		public readonly static string WmGateClientKeyGameServerInfo = "gameServerInfo";
		public readonly static string WmGateClientKeyPatchServerInfo = "patchServerInfo";
		public readonly static string WmGateClientKeyImageServerInfo = "imageServerInfo";
	}
	
	public static class WmGateDomainKey
	{
		public readonly static string WmGateDomainKeyDomain = "domain";
		public readonly static string WmGateDomainKeyCode = "code";
		public readonly static string WmGateDomainKeyDescription = "description";
		public readonly static string WmGateDomainKeyRegDate = "regDate";
		public readonly static string WmGateDomainKeyRegUser = "regUser";
	}
	
	public static class WmGateGameKey
	{
		public readonly static string WmGateGameKeyCode = "game";
		public readonly static string WmGateGameKeyName = "name";
		public readonly static string WmGateGameKeyDescription = "description";
		public readonly static string WmGateGameKeyRegUser = "regUser";
		public readonly static string WmGateGameKeyRegDate = "regDate";
	}
	
	public static class WmGateGameDomainKey
	{
		public readonly static string WmGateGameDomainKeyGameCode = "game";
		public readonly static string WmGateGameDomainKeyGameId = "gameId";
		public readonly static string WmGateGameDomainKeyGameName = "gameName";
		public readonly static string WmGateGameDomainKeyGameDomain = "domain";
		public readonly static string WmGateGameDomainKeyRegDate = "regDate";
		public readonly static string WmGateGameDomainKeyRegUser = "regUser";
		public readonly static string WmGateGameDomainKeyUseAppStore = "useAppStore";
		public readonly static string WmGateGameDomainKeyAppStoreURL = "urlOfAppStore";
		public readonly static string WmGateGameDomainKeyUseTStore = "useTStore";
		public readonly static string WmGateGameDomainKeyTStoreURL = "urlOfTStore";
		public readonly static string WmGateGameDomainKeyUseGoogleStore = "useGoogleStore";
		public readonly static string WmGateGameDomainKeyGoogleStoreURL = "urlOfGoogleStore";
		public readonly static string WmGateGameDomainKeyURLScheme = "customURLScheme";
		public readonly static string WmGateGameDomainKeyAuthByWeme = "idpWeme";
		public readonly static string WmGateGameDomainKeyAuthByKakao = "idpKakao";
		public readonly static string WmGateGameDomainKeyAuthByLine = "idpLine";
		public readonly static string WmGateGameDomainKeyAuthByTwitter = "idpTwitter";
		public readonly static string WmGateGameDomainKeyAuthByFacebook = "idpFaceBook";
		public readonly static string WmGateGameDomainKeyWebGameCode = "webGameCode";
		public readonly static string WmGateGameDomainKeyGamePageURL = "urlOfGamePage";
		public readonly static string WmGateGameDomainKeyServiceStatus = "serviceStatus";

		public readonly static string WmGateGameDomainKeyAppstoreReviewKey = "reviewUrlOfAppStore";
		public readonly static string WmGateGameDomainKeyGooglestoreReviewKey = "reviewUrlOfGoogleStore";
		public readonly static string WmGateGameDomainKeyTsotreReviewKey = "reviewUrlOfTStore";
	
	}
	
	public static class WmGateInfoKey
	{
		public readonly static string WmGateInfoKeyCommon = "common";
		public readonly static string WmGateInfoKeyDomain = "domain";
		public readonly static string WmGateInfoKeyGame = "game";
		public readonly static string WmGateInfoKeyGameDomain = "game_domain";
		public readonly static string WmGateInfoKeyClient = "client";
		public readonly static string WmGateInfoKeyNotice = "notice";
		public readonly static string WmGateInfoKeyMaintenance = "maintenance";
		public readonly static string WmGateInfoKeyTimeStamp = "timestamp";
	}
	
	public static class WmGateMaintenanceKey
	{
		public readonly static string WmGateMaintenanceKeyId = "id";
		public readonly static string WmGateMaintenanceKeyReason = "reason";
		public readonly static string WmGateMaintenanceKeyDomain = "domain";
		public readonly static string WmGateMaintenanceKeyGameCode = "game";
		public readonly static string WmGateMaintenanceKeyBeginDate = "begin";
		public readonly static string WmGateMaintenanceKeyEndDate = "end";
		public readonly static string WmGateMaintenanceKeyType = "maintenanceStyle";
		public readonly static string WmGateMaintenanceKeyRange = "maintenanceType";
		public readonly static string WmGateMaintenanceKeyRegDate = "regDate";
		public readonly static string WmGateMaintenanceKeyRegUser = "regUser";
	}
	
	public static class WmGateNoticeKey
	{
		public readonly static string WmGateNoticeKeyId = "id";
		public readonly static string WmGateNoticeKeyContents = "contents";
		public readonly static string WmGateNoticeKeyDetailLink = "detailLink";
		public readonly static string WmGateNoticeKeyDomain = "domain";
		public readonly static string WmGateNoticeKeyGameCode = "game";
		public readonly static string WmGateNoticeKeyBeginDate = "begin";
		public readonly static string WmGateNoticeKeyEndDate = "end";
		public readonly static string WmGateNoticeKeyType = "showType";
		public readonly static string WmGateNoticeKeyRange = "noticeType";
		public readonly static string WmGateNoticeKeyRegDate = "regDate";
		public readonly static string WmGateNoticeKeyRegUser = "regUser";
	}
}