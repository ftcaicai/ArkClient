using UnityEngine;
using System.Collections;
using System.Text;

public class AsPartyEditSolt : MonoBehaviour
{
	public UIButton m_MemberBtn;
	public SimpleSprite m_Captain_Image;
	public SimpleSprite m_Offline_Image;
	public SpriteText m_UserName = null;
	public SpriteText m_UserLevel = null;
	public SimpleSprite[] m_Class_Image;
	public SimpleSprite portrait = null;
	public ushort m_nSessionIdx;
	
	
	public UIButton				m_PartyEditMenuBtn;
	private AsPartyEditMenu		m_PartyEditMenu;
	public IEventListener		m_eventListener;
	

	private uint m_nCharUniqKey;
	public uint CharUniqKey
	{
		get { return m_nCharUniqKey; }
		set { m_nCharUniqKey = value; }
	}

	private bool m_IsCaptain = false;
	public bool IsCaptain
	{
		get { return m_IsCaptain; }
		set { m_IsCaptain = value; }
	}

	private bool m_IsOffLine = false;
	public bool IsOffLine
	{
		get { return m_IsOffLine; }
		set { m_IsOffLine = value; }
	}

	private bool m_IsOpne = false;
	public bool IsOpne
	{
		get { return m_IsOpne; }
		set { m_IsOpne = value; }
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	public void SetCaptainImage( bool bIs)
	{
		IsCaptain = bIs;
		SetVisible( m_Captain_Image.gameObject, bIs);
	}

	public void SetOffLine( bool bIs)
	{
		IsOffLine = bIs;
		SetVisible( m_Offline_Image.gameObject, bIs);
	}

	public void SetMemberBtn( bool bIs)
	{
		SetVisible( m_MemberBtn.gameObject, bIs);
	}

	public void SetEditMenuBtn( bool bIs)
	{
		SetVisible( m_PartyEditMenuBtn.gameObject , bIs);
		
		if( bIs == true && m_PartyEditMenu != null )
		{
			m_PartyEditMenu.Close();
			m_PartyEditMenu.CharUniqKey = m_nCharUniqKey;
			m_PartyEditMenu.IsOffLine 	= m_IsOffLine;
			m_PartyEditMenu.SessionIdx 	= m_nSessionIdx;
		}
	}
	
	public void MenuButtonClose()
	{
		if( m_PartyEditMenu != null )
		{
			m_PartyEditMenu.Close();
		}
	}
	
	public void SetPortrait( bool bIs)
	{
		SetVisible( portrait.gameObject, bIs);
	}
	
	public void SetUserLevel( bool bIs)
	{
		SetVisible( m_UserLevel.gameObject, bIs);
	}
	
	public void SetDisableMemberBtn( bool bIs)
	{
		if( bIs)
		{
			m_MemberBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_UserName.Color = Color.gray;
		}
		else
		{
			m_MemberBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_UserName.Color = Color.black;
		}
		
		m_MemberBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
	}

	public void CloseAllClassImage()
	{
		for( int i = 0; i < m_Class_Image.Length; ++i)
		{
			if( m_Class_Image[i] != null)
				SetVisible( m_Class_Image[i].gameObject, false);
		}
	}

	public void SetClassImage( int index)
	{
		CloseAllClassImage();
		if( m_Class_Image[index] != null)
			SetVisible( m_Class_Image[index].gameObject, true);
	}

	public void SetDelegateImage( int nImageTableIdx)
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

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
	}

	public void Open()
	{
		gameObject.SetActiveRecursively( true);
	}
	
	void Awake()
	{
		m_PartyEditMenu = m_PartyEditMenuBtn.gameObject.GetComponent<AsPartyEditMenu>();
		
		if( m_PartyEditMenu == null )
		{
			Debug.LogError("m_PartyEditMenu == null");
		}
	}

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_UserName);
		
		m_MemberBtn.SetInputDelegate( MemberBtnDelegate);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PartyEditMenuBtn.spriteText );
		m_PartyEditMenuBtn.SetInputDelegate( EditMenuBtnDelegate);
		m_PartyEditMenuBtn.Text = AsTableManager.Instance.GetTbl_String(2142);
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void MemberBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_eventListener != null )
			{
				m_eventListener.ListenEvent( new AsEventHeader(eLISTEN_EVENT.partyMenuBtnClose) );
			}
		}
	}
	
	private void EditMenuBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_PartyEditMenu != null )
			{
				if( m_PartyEditMenu.MenuActive == false )
				{
					if( m_eventListener != null )
					{
						m_eventListener.ListenEvent( new AsEventHeader(eLISTEN_EVENT.partyMenuBtnClose) );
					}
					
					m_PartyEditMenu.Open( AsPartyManager.Instance.IsCaptain );
				}
				else 
				{
					m_PartyEditMenu.Close();
				}
			}
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
}
