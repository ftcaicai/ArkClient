using UnityEngine;
using System.Collections;
using WmWemeSDK.JSON;
using System.Text;
public class WemeOAuthManager : MonoBehaviour {

	// Use this for initialization
	private static WemeOAuthManager instance_ = null;

	public static WemeOAuthManager Instance
    {
        get
        {
            if( null == instance_ )
            {
                instance_ = FindObjectOfType( typeof( WemeOAuthManager ) ) as WemeOAuthManager;
                if( null == instance_ )
                {
                    Debug.Log( "Fail to get WemeOAuthManager Instance" );
                }
            }
            return instance_;
        }
    }
	
	private const string JSON_KEY_URI = "uri";
	
	private const string JSON_KEY_PARAMS = "params";
	
	private const string JSON_VALUE_PREFIX_URI = "wemeOAuth://";
	private const string JSON_DEVICE_OAUTH_VALUE_PREFIX_URI = "wemeDeviceAuth://";

	private const string WM_HOST_WEME = "loginWeme";
	private const string WM_HOST_FACEBOOK = "loginFacebook";
	private const string WM_HOST_TWITTER = "loginTwitter";
	private const string WM_HOST_GOOGLE = "loginGoogle";
	private const string WM_HOST_NAVER = "loginNaver";
	private const string WM_HOST_YAHOOJP = "loginYahooJp";
	private const string WM_HOST_LOGIN_DEVICE = "loginDevice";
	private const string WM_HOST_IS_DEVICE_LOGIN = "isDeviceLogin";
	private const string WM_HOST_DEVICE_CHANGE = "requestDeviceChange";
	
	/**
	 * Weme sdk loginWeme
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginWeme(string clientId,string clientSecret,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_WEME);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginWeme(string clientId,string clientSecret,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_WEME);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginFacebook
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginFacebook(string appId,string redirectUri,string[] permissions,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(appId)==true||WemeManager.isEmpty(redirectUri)==true){
			Debug.Log("must not null appId or redirectUri");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_FACEBOOK);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("appId",appId);
		paramsObject.Add("redirectUri",redirectUri);
		string permissionsString = ConvertStringArrayToString(permissions);
		paramsObject.Add("permissions",JSONArray.Parse(permissionsString));
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginFacebook(string appId,string redirectUri,string[] permissions,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(appId)==true||WemeManager.isEmpty(redirectUri)==true){
			Debug.Log("must not null appId or redirectUri");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_FACEBOOK);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("appId",appId);
		paramsObject.Add("redirectUri",redirectUri);
		string permissionsString = ConvertStringArrayToString(permissions);
		paramsObject.Add("permissions",JSONArray.Parse(permissionsString));
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginTwitter
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginTwitter(string consumerKey,string consumerSecret,string callbackUrl,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(consumerKey)==true||WemeManager.isEmpty(consumerSecret)==true){
			Debug.Log("must not null consumerKey or consumerSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_TWITTER);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("consumerKey",consumerKey);
		paramsObject.Add("consumerSecret",consumerSecret);
		paramsObject.Add("callbackUrl",callbackUrl);
		
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginTwitter(string consumerKey,string consumerSecret,string callbackUrl,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(consumerKey)==true||WemeManager.isEmpty(consumerSecret)==true){
			Debug.Log("must not null consumerKey or consumerSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_TWITTER);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("consumerKey",consumerKey);
		paramsObject.Add("consumerSecret",consumerSecret);
		paramsObject.Add("callbackUrl",callbackUrl);
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginGoogle
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginGoogle(string clientId,string clientSecret,string[] scopes,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_GOOGLE);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		string scopesString = ConvertStringArrayToString(scopes);
		paramsObject.Add("scopes",JSONArray.Parse(scopesString));
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginGoogle(string clientId,string clientSecret,string[] scopes,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_GOOGLE);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		string scopesString = ConvertStringArrayToString(scopes);
		paramsObject.Add("scopes",JSONArray.Parse(scopesString));
		
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginNaver
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginNaver(string clientId,string clientSecret,string redirectUri,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true||WemeManager.isEmpty(redirectUri)==true){
			Debug.Log("must not null clientId or clientSecret or redirectUrl");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_NAVER);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		paramsObject.Add("redirectUri",redirectUri);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginNaver(string clientId,string clientSecret,string redirectUri,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true||WemeManager.isEmpty(clientSecret)==true||WemeManager.isEmpty(redirectUri)==true){
			Debug.Log("must not null clientId or clientSecret or redirectUrl");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_NAVER);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		paramsObject.Add("clientSecret",clientSecret);
		paramsObject.Add("redirectUri",redirectUri);
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	/**
	 * Weme sdk loginYahooConnect
	 * 
	 * @param oAuthString - 
	 * @param WmInterfaceBrokerDelegate
	 */
	public void loginYahooJp(string clientId,string[] scopes,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_YAHOOJP);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		string scopesString = ConvertStringArrayToString(scopes);
		paramsObject.Add("scopes",JSONArray.Parse(scopesString));
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public void loginYahooJp(string clientId,string[] scopes,WemeManager.WmOauthDisplayOrientation orientation,WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		if(WemeManager.isEmpty(clientId)==true){
			Debug.Log("must not null clientId or clientSecret");
			return;
		}
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_VALUE_PREFIX_URI+WM_HOST_YAHOOJP);
		JSONObject paramsObject = new JSONObject();
		paramsObject.Add("clientId",clientId);
		string scopesString = ConvertStringArrayToString(scopes);
		paramsObject.Add("scopes",JSONArray.Parse(scopesString));
		
		string orientationString = null;
		if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_LANDSCAPE == orientation){
			orientationString = "landscape";
		}else if(WemeManager.WmOauthDisplayOrientation.WM_OAUTH_DISPLAY_ORIENTATION_PORTRAIT == orientation){
			orientationString = "portrait";
		}
		if(orientationString!=null)
			paramsObject.Add("orientation",orientationString);
		jsonObject.Add(JSON_KEY_PARAMS,paramsObject);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	// Device OAuth
	public void loginDevice(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_DEVICE_OAUTH_VALUE_PREFIX_URI+WM_HOST_LOGIN_DEVICE);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	
	public string isDeviceLogin(){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_DEVICE_OAUTH_VALUE_PREFIX_URI+WM_HOST_IS_DEVICE_LOGIN);
		return WmInterfaceBroker.getInstance.requestSync(jsonObject.ToString());
	}
	
	public void requestDeviceChange(WmInterfaceBroker.WmInterfaceBrokerDelegate callback){
		JSONObject jsonObject = new JSONObject();
		jsonObject.Add(JSON_KEY_URI,JSON_DEVICE_OAUTH_VALUE_PREFIX_URI+WM_HOST_DEVICE_CHANGE);
		WmInterfaceBroker.getInstance.requestAsync(jsonObject.ToString(),callback);
	}
	public static string ConvertStringArrayToString(string[] array)
    {
		//
		// Concatenate all the elements into a StringBuilder.
		//
		StringBuilder builder = new StringBuilder();
		builder.Append("[");
		bool start = true;
		foreach (string value in array)
		{
			if(start==false){
				builder.Append(',');	
			}
			start=false;
		    builder.Append("\"" +value +"\"");
	    	
		}
		builder.Append("]");
		return builder.ToString();
    }
}
