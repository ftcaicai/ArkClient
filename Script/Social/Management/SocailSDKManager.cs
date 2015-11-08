using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using WmWemeSDK.JSON;
using System.Text;

public class SocailSDKManager : MonoBehaviour
{
	private static SocailSDKManager instance_ = null;
	private const string RETURN_OBJECT_NAME = "SocialManager";
	public static SocailSDKManager Instance
	{
		get
		{
			if( null == instance_)
			{
				instance_ = FindObjectOfType( typeof( SocailSDKManager)) as SocailSDKManager;
				if( null == instance_)
					Debug.Log( "Fail to get SocailSDKManager Instance");
			}

			return instance_;
		}
	}
	
	
	#if UNITY_IPHONE
	[DllImport( "__Internal")]
	private static extern void sendAppData( string resultObject, string title, string msg, string installurl, string executeurl);
	#endif

	#if UNITY_ANDROID
	private AndroidJavaClass KakaoAndroid = null;
	private AndroidJavaClass SMSAndroid = null;
	private string m_strID;
	private bool m_IsTablet = false;
	public bool IsTabletPc 
	{
		get {return m_IsTablet;}
		set { m_IsTablet = value;}
	}
	#endif

	// Use this for initialization
	void Start()
	{
		#if ( UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJNI.AttachCurrentThread();
		SMSAndroid = new AndroidJavaClass( "com.wemadecreative.arksphere.SendSMSManager");
		KakaoAndroid = new AndroidJavaClass( "com.wemadecreative.arksphere.KakaoLinkManager");
		#endif
	}

	public void SendSMS( string phonenumber, string msg)
	{
		#if UNITY_ANDROID
		m_IsTablet = false;
		m_strID = phonenumber;
		SMSAndroid.CallStatic( "SendSMS", RETURN_OBJECT_NAME, phonenumber, msg);
		#endif
	}

	public void getContactList()
	{
		#if UNITY_ANDROID
		SMSAndroid.CallStatic( "getContactList", RETURN_OBJECT_NAME);
		#endif
	}

	public void ContactList( string str)
	{

	}

	public void SendKakaoMessage( string title, string msg, string installurl, string executeurl)
	{
		#if UNITY_ANDROID
		//KakaoAndroid.CallStatic( "sendAppData", RETURN_OBJECT_NAME, title, msg, installurl, executeurl);
		#endif
		
		#if UNITY_IPHONE
		sendAppData( RETURN_OBJECT_NAME, title, msg, installurl, executeurl);
		#endif
	}

	void receiveKakaoLink( string str)
	{
		Debug.Log( str);
		#if UNITY_IPHONE || UNITY_ANDROID
		if( str == "Success")
		{
			AsCommonSender.SendGameInvite( false, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_KAKAOTALK, "12345");
			AsCommonSender.SendGameInviteList();
		}
		else
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 4059),
				AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		#endif
	}

	
	public void IsTablet( string str)
	{
		Debug.Log( str);
		#if  UNITY_ANDROID
		if( str == "Tablet")
		{
			m_IsTablet = true;
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
				AsTableManager.Instance.GetTbl_String(4101), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	
		}	
		else
		{
			AsCommonSender.SendGameInvite( false, (int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_SMS, m_strID);
			AsCommonSender.SendGameInviteList(); //#19733
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
				AsTableManager.Instance.GetTbl_String(4208), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);//#21544
		}
		#endif		
		
	}

	
	// Update is called once per frame
	void Update()
	{
	}
}
