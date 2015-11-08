using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class AsFacebookItem : MonoBehaviour
{
	public SimpleSprite m_DefaultPortrait = null;
	public SimpleSprite m_Portrait = null;
	public SpriteText desc = null;
	private AsFaceBookFriend m_friendData = null;

	private bool isLoaded = false;
	public bool IsLoaded
	{
		get { return isLoaded; }
		set { isLoaded = value; }
	}
	public UIButton m_InviteMsgBtn;

	private Texture2D m_TexPortrait = null;

	void OnDestory()
	{
		if( m_TexPortrait != null)
			DestroyImmediate( m_TexPortrait);
	}

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_InviteMsgBtn.spriteText);
		m_InviteMsgBtn.SetInputDelegate( m_InviteMsgBtnDelegate);
		m_InviteMsgBtn.Text = AsTableManager.Instance.GetTbl_String( 1263);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetFacebookFriendData( AsFaceBookFriend friend)
	{
		m_friendData = friend;
		desc.Text = friend.name;

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "https://graph.facebook.com/{0}/picture", m_friendData.str_id);

		if( friend.installed)
			m_InviteMsgBtn.gameObject.SetActiveRecursively( false);
		else
			m_InviteMsgBtn.gameObject.SetActiveRecursively( true);

		StartCoroutine( LoadPortraitImage( sb.ToString()));
	}

	private void m_InviteMsgBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Debug.Log( "Facebook InviteMsgBtnDelegate");
			AsCommonSender.SendGameInvite( true, ( int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_FACEBOOK, m_friendData.str_id);
		}
	}

	IEnumerator LoadPortraitImage( string url)
	{
		if( ( null == url) || ( 0 == url.Length))
		{
			isLoaded = true;
			yield break;
		}

		WWW www = new WWW( url);
		yield return www;

		if( www.error == null)
		{
			m_TexPortrait = www.texture;
			if( m_TexPortrait.height == 8)
			{
				m_DefaultPortrait.gameObject.SetActiveRecursively( true);
				m_Portrait.gameObject.SetActiveRecursively( false);
			}
			else
			{
				m_Portrait.SetTexture( m_TexPortrait);
				m_Portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, m_TexPortrait.width, m_TexPortrait.height));

				m_DefaultPortrait.gameObject.SetActiveRecursively( false);
				m_Portrait.gameObject.SetActiveRecursively( true);
			}

			www.Dispose();
			www = null;
		}
		else
		{
			m_DefaultPortrait.gameObject.SetActiveRecursively( true);
			m_Portrait.gameObject.SetActiveRecursively( false);
		}

		isLoaded = true;
	}
}
