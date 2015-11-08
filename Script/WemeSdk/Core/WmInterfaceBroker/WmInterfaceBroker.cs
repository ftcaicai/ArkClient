using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WmWemeSDK.JSON;
using System;
using System.Runtime.InteropServices;


public class WmInterfaceBroker : MonoBehaviour {
	// for use unity simulrator
	public bool onStart = false;
	
	public bool onLogin = false;
	
	public bool onPush = false;
	
	private static WmInterfaceBroker instance_ = null;
	
	public static WmInterfaceBroker getInstance
    {
        get
        {
            if( null == instance_ )
            {
                instance_ = FindObjectOfType( typeof( WmInterfaceBroker ) ) as WmInterfaceBroker;
				
                if( null == instance_ )
                {
                    Debug.Log( "Fail to get Applications Instance" );
                }
            }
            return instance_;
        }
    }
	
	/* delegate */
	public delegate void WmInterfaceBrokerDelegate(string jsonResult);
	
	/* delegate event */
	//public static event WmInterfaceBrokerDelegate OnEventNotify; 
	/**
	  * WmInterfaceBrokerErrorDomain
	  * 
	  * The Error For WmInterfaceBroker when Exception into JsonProxy and communication native JsonProxy API
	  * 
	  */
	public const string DOMAIN = "WmInterfaceBroker";
	
	
	/**
	  * WmInterfaceBroker mothod handler event storage dictionary
	  * 
	  * @key WmInterfaceBroker.uri
	  * @value WmInterfaceBrokerDelegate
	  */
	
	public IDictionary eventDictionary = new Dictionary<string,WmInterfaceBrokerDelegate>();
	
	/*
	 * setting native Link
	 */

	void Awake()
	{

		
	}
	
	void OnApplicationQuit() {
		#if UNITY_EDITOR
		
		#endif
	}
	
	/**
	 * requestAsync
	 * 
	 * Request Async Method to JSONProxy 
	 * 
	 * @param[in] jsonRequest request json string for request method [mandatory]
	 * @param[in] callback The Callback hanlder for request [optional]
	 * @see http://wiki.wemade.com/display/TECHDIV/WmInterfaceBroker_JSONProtocols
	 */
	public void   requestAsync(string jsonString, WmInterfaceBrokerDelegate callback ){
		Debug.Log("send " + jsonString);
		JSONObject jsonObject = JSONObject.Parse(jsonString);
		string uriString = jsonObject.GetString("uri");
		
		if(uriString == null){
			Debug.LogError(DOMAIN + "WmInterfaceBroker cannot use without uri");
			onResponse(jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"cannot use without uri",null).ToString());
			return;	
		}
		if(callback!=null){
			addEventHandler(uriString,callback);
		}else{
			Debug.LogError(DOMAIN + "WmInterfaceBroker cannot use without callback");
			onResponse(jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"cannot use without callback",uriString).ToString());
			return;	
		}
	}
	
	/**
	 * requestSync
	 * 
	 * Request sync Method to JSONProxy 
	 * 
	 * @param[in] jsonRequest request json string for request method [mandatory]
	 */
	public string requestSync(string jsonString){	
		JSONObject jsonObject = JSONObject.Parse(jsonString);
		string uriString = jsonObject.GetString("uri");
		if(uriString == null){
			Debug.LogError(DOMAIN + "WmInterfaceBroker cannot use without uri");
			return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"cannot use without uri",null).ToString();
		}
		return null;
	}
/*	
	public static void onResult(string resultObject,string resultMethod,string resultString){
		WemeLoom.QueueOnMainThread(()=>{
			GameObject obj = GameObject.Find(resultObject);
			if (obj != null)
				obj.SendMessage(resultMethod, resultString);
			//onResponse(resultString);
		});
		
	}*/
	
	void onResponse(string jsonResult){
		Debug.Log("result log " + jsonResult);
		try{
			JSONObject requestJSON  = JSONObject.Parse(jsonResult);
			string uri = requestJSON.GetString("uri");
			if(string.IsNullOrEmpty(uri)==true){
				Debug.LogError("WmInterfaceBroker must have uri param onResonse result :  "+jsonResult);
				return;	
			}	
			WmInterfaceBrokerDelegate callback =getValueDictionaryForKey(uri);
			if(callback==null){
				Debug.LogError("WmInterfaceBroker callback storage error :  "+jsonResult);
				return;
			}
			WmInterfaceBrokerDelegate eventDelegate = new WmInterfaceBrokerDelegate(callback);
			eventDelegate(jsonResult);
			
		}catch(Exception e){
			Debug.LogError(DOMAIN + " UncaughtException : "+e.Message);
		}
		
	}
	// for use unity simulrator
	// requset parse
	private JSONObject getResultForRequest(string scheme,string host, JSONObject jsonObject){
		scheme = scheme.ToLower();
		switch(scheme){
			case "weme":
				return getResultForWeme(scheme,host,jsonObject);
			case "wemeoauth":
				return getResultForWemeOAuth(scheme,host,jsonObject);
			case "wemepush":
				return getResultForWemePush(scheme,host,jsonObject);
		}
		return jsonObject;
	}
	// weme urlscheme parse
	private JSONObject getResultForWeme(string scheme,string host,JSONObject jsonObject){
		host = host.ToLower();
		switch(host){
			case "start":
				if(jsonObject.ContainsKey("serverZone")&&jsonObject.ContainsKey("gameCode")&&jsonObject.ContainsKey("domain")&&jsonObject.ContainsKey("gameVersion")&&jsonObject.ContainsKey("marketCode")){
					JSONObject jsonResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					onStart=true;
					return jsonResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}
			case "gateinfo":
				JSONObject gateResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return gateResult;
			
			case "isauthorized":
				JSONObject authResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				authResult.Remove("authorized");
				authResult.Add("authorized",onLogin);
				return authResult;
			
			case "login":
				if(jsonObject.ContainsKey("authData")){
					JSONObject jsonResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					onLogin=true;
					return jsonResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}
			
			case "playerkey":
				JSONObject playerKeyResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return playerKeyResult;
			case "logout":
				JSONObject logoutResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return logoutResult;
			case "withdraw":
				JSONObject withdrawResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return withdrawResult;
			
			case "gameid":
				JSONObject gameidResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return gameidResult;
			case "request":
				if(jsonObject.ContainsKey("uri")){
					JSONObject jsonResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					onLogin=true;
					return jsonResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}
			
			case "configuration":
				JSONObject configurationResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return configurationResult;
			
			case "authdata":
				JSONObject authdataResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return authdataResult;
			
			case "sdkversion":
				JSONObject sdkversionResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return sdkversionResult;
			
		}
		return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
	}
	// wemeOauth urlscheme parse
	private JSONObject getResultForWemeOAuth(string scheme,string host,JSONObject jsonObject){
		host = host.ToLower();
		switch(host){
		case "loginfacebook":
			if(jsonObject.ContainsKey("appId")&&jsonObject.ContainsKey("redirectUri")&&jsonObject.ContainsKey("permissions")){
				JSONObject facebookResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				return facebookResult;
			}else{
				return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
			}
			case "logintwitter":
				if(jsonObject.ContainsKey("consumerKey")&&jsonObject.ContainsKey("consumerSecret")&&jsonObject.ContainsKey("callbackUrl")){
					JSONObject twitterResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					return twitterResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}

			case "loginweme":
				if(jsonObject.ContainsKey("clientId")&&jsonObject.ContainsKey("clientSecret")){
					JSONObject wemeResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					return wemeResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}

		default:
			break;
		}
		return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
	}
	// wemePush urlscheme parse
	public JSONObject getResultForWemePush(string scheme,string host,JSONObject jsonObject){
		host = host.ToLower();
		switch(host){
			case "isallowpushmessage":
				JSONObject pushallowResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
				pushallowResult.Remove("allow");
				pushallowResult.Add("allow",onPush);
				return pushallowResult;

			case "requestallowpushmessage":
				if(jsonObject.ContainsKey("allow")){
					JSONObject requestPushResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/"+scheme+"/"+host)).text);
					onPush = jsonObject.GetBoolean("allow");
					requestPushResult.Remove("allow");
					requestPushResult.Add("allow",onPush);
					return requestPushResult;
				}else{
					return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
				}
		}
		return jsonResultForError("WmInterfaceBroker",(long)WemeManager.WemeErrType.WM_ERR_INVALID_PARAMETER,"invalid_parameter",scheme+"://"+host);
	}
	
	private WmInterfaceBrokerDelegate getValueDictionaryForKey(string uri){
		if(eventDictionary.Contains(uri)){
			return (WmInterfaceBrokerDelegate)eventDictionary[uri];	
		}
		return null;
	}
	
	
	private void addEventHandler(string uri,WmInterfaceBrokerDelegate callback){
		if(eventDictionary.Contains(uri)){
			eventDictionary.Remove(uri);	
		}
		eventDictionary.Add(uri,callback);
	}
	
	private JSONObject jsonResultForError(String domain,long errorCode,string desc,string uri){
		JSONObject resultObject = new JSONObject();
		if(isEmpty(uri)==false){
			resultObject.Add("uri",uri);
		}		
		JSONObject errObject = new JSONObject();
		errObject.Add("code",errorCode);
		errObject.Add("domain",domain);
		if(isEmpty(desc)==false){
			errObject.Add("desc",desc);
		}
		return resultObject;
	}
	
	private static bool isEmpty(string checkString){
		if(checkString==null){
			return true;
		}
		if(checkString.Length<1){
			return true;
		}
		return false;	
	}
}

