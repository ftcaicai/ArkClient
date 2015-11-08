using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Partytrack {
	public static string UUID = "uuid";
	public static string UDID = "udid";
	public static string ClientID = "client_id";
#if ( UNITY_IPHONE && !UNITY_EDITOR)
	[DllImport("__Internal")]
	private static extern void start_(int app_id, string app_key);
	
	[DllImport("__Internal")]
	private static extern void openDebugInfo_();
	
	[DllImport("__Internal")]
	private static extern void setConfigure_(string name, string svalue);
	
	[DllImport("__Internal")]
	private static extern void sendEventWithId_(int event_id);
	
	[DllImport("__Internal")]
	private static extern void sendEventWithName_(string event_name);

	[DllImport("__Internal")]
	private static extern void sendPayment_(string item_name, int item_num, string item_price_currency, double item_price);
  
	[DllImport("__Internal")]
	private static extern void disableApplicationTracking_();
  
#endif

#if ( UNITY_ANDROID && !UNITY_EDITOR)
	private static AndroidJavaClass getTrack()
	{
		return new AndroidJavaClass("it.partytrack.sdk.Track");
	}

	private static AndroidJavaObject getActivity()
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	}
#endif
	public static void start(int app_id, string app_key) {
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		start_(app_id, app_key);
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("start", getActivity(), app_id, app_key);
#endif
    }
	
	public static void openDebugInfo(){
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		openDebugInfo_();
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("setDebugMode", true);
#endif
	}
	
	
	public static void setConfigure(string name, string svalue){
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		setConfigure_(name, svalue);
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("setOptionalparam", name, svalue);
#endif
	}
	
	
	public static void sendEvent(int event_id){
	Debug.Log("sendEvent" + event_id.ToString() );
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		sendEventWithId_(event_id);
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("event", event_id);
#endif
	}
	
	
	public static void sendEvent(string event_name){
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		sendEventWithName_(event_name);
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("event", event_name);
#endif
	}	
	
	
	public static void sendPayment(string item_name, int item_num, string item_price_currency, double item_price){
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		sendPayment_(item_name, item_num, item_price_currency, item_price);
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
		float item_price_f = (float)item_price;
        getTrack().CallStatic("payment", item_name, item_price_f, item_price_currency, item_num);
#endif
	}

	public static void disableAdvertisementOptimize(){
#if ( UNITY_IPHONE && !UNITY_EDITOR)
		//ios
    disableApplicationTracking_();
#endif
#if ( UNITY_ANDROID && !UNITY_EDITOR)
        getTrack().CallStatic("disableAdvertisementOptimize");
#endif
	}
}

