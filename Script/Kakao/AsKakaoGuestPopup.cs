using UnityEngine;
using System.Collections;

public class AsKakaoGuestPopup : MonoBehaviour
{
/*	public SpriteText title = null;
	public SpriteText msg = null;
	public UIButton kakaoLogin = null;
	public UIButton cancel = null;
	[HideInInspector]
	public bool guestRestriction = false;
	
	// Use this for initialization
	void Start()
	{
		kakaoLogin.Text = AsTableManager.Instance.GetTbl_String(1409);
		cancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		title.Text = AsTableManager.Instance.GetTbl_String(4024);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetMessage( string msg)
	{
		this.msg.Text = msg;
	}
	
	private void OnKakaoLoginBtn()
	{
		KakaoManager.Instance.authDeviceKakaoUnregister();
		KakaoManager.Instance.authDeviceKakaoSignOff();
		StartCoroutine( "KakaoLogout");
	}
	
	private void OnCancelBtn()
	{
		if( true == guestRestriction)
		{
			KakaoManager.Instance.authDeviceKakaoUnregister();
			KakaoManager.Instance.authDeviceKakaoSignOff();
			StartCoroutine( "KakaoLogoutOnly");
			return;
		}
		
		Close();
	}
	
	private void Close()
	{
		GameObject.DestroyImmediate( gameObject.transform.parent.gameObject);
	}
	
	IEnumerator KakaoLogoutOnly()
	{
		while( true)
		{
			yield return new WaitForSeconds( 0.5f);
			
			if( false == KakaoManager.isKakaoLogin)
			{
				Close();
				ToLoginScene();
				break;
			}
		}
	}
		
	IEnumerator KakaoLogout()
	{
		while( true)
		{
			yield return new WaitForSeconds( 0.5f);
			
			if( false == KakaoManager.isKakaoLogin)
			{
				KakaoManager.Instance.authDeviceKakakoSignInOrSignUp();
				StartCoroutine( "KakaoLogin");
				break;
			}
		}
	}
	
	IEnumerator KakaoLogin()
	{
		while( true)
		{
			yield return new WaitForSeconds( 0.5f);
			
			if( true == KakaoManager.isKakaoLogin)
			{
				Close();
				ToLoginScene();
				break;
			}
		}
	}
	
	private void ToLoginScene()
	{
		AsNetworkManager.Instance.InitSocket();
		
		AsChatManager.Instance.ClearAllChat();
		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
		AsServerListData.Instance.Clear();
		AsNetworkManager.Instance.InitSocket();
		AsHUDController.SetActiveRecursively( false);
		AsCommonSender.ResetSendCheck();
		PlayerBuffMgr.Instance.Clear();
		AsHudDlgMgr.Instance.CloseMsgBox();
		TerrainMgr.Instance.Clear();
		ItemMgr.HadItemManagement.Inven.ClearInvenItems();
		ItemMgr.HadItemManagement.Storage.ClearStorageItems();//$yde
		SkillBook.Instance.InitSkillBook();
		CoolTimeGroupMgr.Instance.Clear();
        ArkQuestmanager.instance.ResetQuestManager();
		AsPartyManager.Instance.PartyDiceRemoveAll();//#11954
		AsPartyManager.Instance.PartyUserRemoveAll();
		AsUserInfo.Instance.Clear();
		AsHudDlgMgr.Instance.invenPageIdx = 0;
		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}
	*/
}
