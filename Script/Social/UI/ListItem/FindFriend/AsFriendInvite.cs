using UnityEngine;
using System.Collections;
using System.Text;


public class AsFriendInvite : MonoBehaviour {
	
	public SpriteText m_NickName  = null; //
	public UIButton   m_FriendAcceptBtn = null;
	public UIButton   m_FriendRefuseBtn = null;
	
	public uint		  m_nUserUniqKey;	
	private string 	  m_szCharName = null;
	
	public SimpleSprite portrait = null;
//	private Texture2D portraitTex = null;
	
	FriendAcceptBtnFunc m_FriendAcceptBtnFunc;
		
	// Use this for initialization
	void Start () {
		m_FriendAcceptBtn.SetInputDelegate(FriendAcceptBtnDelegate);
		m_FriendRefuseBtn.SetInputDelegate(FriendRefuseBtnDelegate);
		
		m_FriendAcceptBtn.Text =  AsTableManager.Instance.GetTbl_String( 37922 );
		m_FriendRefuseBtn.Text =  AsTableManager.Instance.GetTbl_String( 1151 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetFriendData( body2_SC_FRIEND_LIST  Data, FriendAcceptBtnFunc friendAcceptBtnChangeDel)
	{
		m_nUserUniqKey  = Data.nUserUniqKey;	
		m_szCharName    = Data.szCharName;
	
		SetDelegateImage(Data.nImageIndex);
		
		m_NickName.Text    = string.Format( "Lv.{0} {1}", Data.nLevel, Data.szCharName);	
		
		m_FriendAcceptBtnFunc = friendAcceptBtnChangeDel;
	}
	
	private void FriendRefuseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		    AsCommonSender.SendFriendJoin( m_szCharName, m_nUserUniqKey, eFRIEND_JOIN_TYPE.eFRIEND_JOIN_REFUSE );	
			SetDisabledButton();		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
			m_FriendAcceptBtnFunc(this);
		}
	}
	
	private void FriendAcceptBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsCommonSender.SendFriendJoin( m_szCharName, m_nUserUniqKey, eFRIEND_JOIN_TYPE.eFRIEND_JOIN_ACCEPT );	
			SetDisabledButton();				
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
			m_FriendAcceptBtnFunc(this);
			
			
		}
	}
	
	private void SetDisabledButton()
	{
	
		m_FriendAcceptBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);	
		m_FriendAcceptBtn.controlIsEnabled = false;
		m_FriendRefuseBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);	
		m_FriendRefuseBtn.controlIsEnabled = false;
	}
		

	void SetDelegateImage(int nImageTableIdx)
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");

		DelegateImageData delegateImage = AsDelegateImageManager.Instance.GetDelegateImage( nImageTableIdx);
		if( null == delegateImage)
			sb.Append( "Default");
		else
			sb.Append( delegateImage.iconName);
		
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	
}
