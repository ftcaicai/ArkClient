using UnityEngine;
using System.Collections;
using WmWemeSDK.JSON;

public class AsWithdrawDlg : MonoBehaviour
{
	public SpriteText txtTitle = null;
	public SpriteText txtInfo1 = null;
	public SpriteText txtInfo2 = null;
	public UITextField txtConfirm = null;
	public UIButton btnCancel = null;
	public UIButton btnOk = null;
	
	private GameObject goParent = null;
	public GameObject ParentObject
	{
		set	{ goParent = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String(4016);
		txtInfo1.Text = AsTableManager.Instance.GetTbl_String(4019);
		txtInfo2.Text = AsTableManager.Instance.GetTbl_String(4020);
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		btnOk.Text = AsTableManager.Instance.GetTbl_String(1152);
		txtConfirm.Text = "";
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		goParent.BroadcastMessage( "CloseWithdrawDlg", SendMessageOptions.DontRequireReceiver);
	}
	
	private void OnCancelBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		goParent.BroadcastMessage( "CloseWithdrawDlg", SendMessageOptions.DontRequireReceiver);
	}
	
	private void OnConfirmBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( "ArkSphere" == txtConfirm.Text)
		{
			WemeReLogin();	//#19418.		
			AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eGame_Withdrawal);
		}
		else
		{
			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4022), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		}
			
		goParent.BroadcastMessage( "CloseWithdrawDlg", SendMessageOptions.DontRequireReceiver);
	}
	
	public void WemeReLogin()
	{		
		//JSONObject authdataResult = JSONObject.Parse(((TextAsset)Resources.Load("JSONRequests/weme/authData")).text);
		
		Debug.Log( "Connect to login server... " + AsNetworkDefine.LOGIN_SERVER_IP + " " + AsNetworkDefine.LOGIN_SERVER_PORT);	
		AsNetworkManager.Instance.ConnectToServer( AsNetworkDefine.LOGIN_SERVER_IP, AsNetworkDefine.LOGIN_SERVER_PORT, SOCKET_STATE.SS_LOGIN);
	
        if (true == AsNetworkManager.Instance.IsConnected())
        {
           
			AsLoadingIndigator.Instance.ShowIndigator("");
            string strAuthData = WemeManager.Instance.authData();
            if (null == strAuthData)    
                return;
           
            JSONObject jsonObject = JSONObject.Parse(strAuthData);
			JSONObject authDataJosn = jsonObject.GetObject("authData");
			if( null == authDataJosn)
				return;
			
			
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
	      //  if (WemeSdkManager.Instance.IsGuest)
	       // {
	       //     recordJson.Add("platform", "guest");
	       // }
	       // else
	       // {
	            recordJson.Add("platform", AsGameDefine.WEME_DOMAIN);
	      //  }
	    	
			
			if(authDataJosn["authProvider"].Str.CompareTo( "weme") == 0)
			{
				wemeLogin.eLoginType = (int)eLOGINTYPE.eLOGINTYPE_WEME;
				checkAuthJson.Add("wemeMemberNo", authDataJosn["wemeMemberNo"].Str);
	        	checkAuthJson.Add("accessToken", authDataJosn["accessToken"].Str);	
				
				 recordJson.Add("platform", AsGameDefine.WEME_DOMAIN);
				 recordJson.Add("device", authDataJosn["uuid"].Str );
			}
			else if(authDataJosn["authProvider"].Str.CompareTo("facebook") == 0) 
			{
				wemeLogin.eLoginType = (int)eLOGINTYPE.eLOGINTYPE_FACEBOOK;
				checkAuthJson.Add("fbUserId", authDataJosn["fbUserId"].Str);
	        	checkAuthJson.Add("accessToken", authDataJosn["accessToken"].Str);	
				
				 recordJson.Add("platform", "facebook");
				 recordJson.Add("device", "" );				
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
			
			WemeSdkManager.Instance.IsWithdrawLogin  = true;			
        }
	}
}
