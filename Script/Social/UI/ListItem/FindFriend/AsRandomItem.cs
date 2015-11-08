using UnityEngine;
using System.Collections;
using System.Text;
public class AsRandomItem : MonoBehaviour {
	
	public SpriteText m_NickName  = null; //
	public UIButton   m_FriendInviteBtn = null;
	public uint		  m_nUserUniqKey;
	public SpriteText m_FriendInvitBtnText = null;
	private string 	  m_szCharName = null;
	
	public SimpleSprite portrait = null;
//	private Texture2D portraitTex = null;
 	// Use this for initialization
	void Start () {
		m_FriendInviteBtn.SetInputDelegate(FriendInviteBtnDelegate);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_FriendInvitBtnText);	
		m_FriendInvitBtnText.Text =  AsTableManager.Instance.GetTbl_String( 1164 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetRandomFriendData( body2_SC_FRIEND_RANDOM  Data)
	{
		m_nUserUniqKey     = Data.nUserUniqKey;	
		m_szCharName       = Data.szCharName;
		m_NickName.Text    = string.Format( "Lv.{0} {1}", Data.nLevel, Data.szCharName);	
		SetDelegateImage(Data.nImageIndex);		
	}
	
	private void FriendInviteBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsCommonSender.SendFriendInvite(m_szCharName);
			m_FriendInviteBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);			
			m_FriendInvitBtnText.Text = AsTableManager.Instance.GetTbl_String( 1164 );
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
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
