using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class AsPartyEditMenu : MonoBehaviour 
{

	public enum ePartyEditMenu
	{
		NONE = -1,
		Captain   = 0,
		Kick,
		Move,

		MaxPartyEditMenu
	}	
	
	
	public 	UIButton[] 		m_MenuBtn;
	
	private List<UIButton>	m_listActiveMenuBtn = new List<UIButton>();
	
	private bool			m_isMenuActive = false;
	public bool MenuActive
	{
		get{return m_isMenuActive;}
	}
	
	
	private uint m_nCharUniqKey;
	public uint CharUniqKey
	{
		get { return m_nCharUniqKey; }
		set { m_nCharUniqKey = value; }
	}
	
	private bool m_IsOffLine = false;
	public bool IsOffLine
	{
		get { return m_IsOffLine; }
		set { m_IsOffLine = value; }
	}
	
	private ushort m_nSessionIdx;
	public ushort SessionIdx
	{
		get{return m_nSessionIdx;}
		set{m_nSessionIdx = value;}
	}
	
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () 
	{
		foreach( UIButton btn in m_MenuBtn )
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( btn.spriteText );	
		}
		
		m_MenuBtn[(int)ePartyEditMenu.Captain].spriteText.Text 		= AsTableManager.Instance.GetTbl_String(1959);
		m_MenuBtn[(int)ePartyEditMenu.Kick].spriteText.Text 		= AsTableManager.Instance.GetTbl_String(1137);
		m_MenuBtn[(int)ePartyEditMenu.Move].spriteText.Text 		= AsTableManager.Instance.GetTbl_String(2143);

		m_MenuBtn[(int)ePartyEditMenu.Captain].SetInputDelegate( CaptainBtnDelegate );
		m_MenuBtn[(int)ePartyEditMenu.Kick].SetInputDelegate( KickBtnDelegate );
		m_MenuBtn[(int)ePartyEditMenu.Move].SetInputDelegate( MoveBtnDelegate );
	}
	
	private void Clear()
	{
		m_listActiveMenuBtn.Clear();
	}
	
	private void RefreshActiveButtonState()
	{
		float	fPosY 		= 0;
		float	fYPosGap 	= -2.0f;
		foreach( UIButton	btn in m_listActiveMenuBtn )
		{
			fPosY += fYPosGap;
			
			btn.gameObject.SetActiveRecursively(true);
			btn.gameObject.transform.localPosition = new Vector3( 0 , fPosY , -2.0f );
		}
	}
	
	public void Close()
	{
		foreach( UIButton btn in m_MenuBtn )
		{
			btn.gameObject.SetActiveRecursively(false);
		}
		
		m_isMenuActive = false;
	}
	
	public void Open( bool isCaptain )
	{
		Clear();
		
		if( isCaptain == true )
		{
			m_listActiveMenuBtn.Add( m_MenuBtn[(int)ePartyEditMenu.Captain] );
			m_listActiveMenuBtn.Add( m_MenuBtn[(int)ePartyEditMenu.Kick] );
		}
		
		m_listActiveMenuBtn.Add( m_MenuBtn[(int)ePartyEditMenu.Move] );
		
		RefreshActiveButtonState();
		
		m_isMenuActive = true;
	}
	
	private void BtnSound()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}
	
	private void CaptainBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( IsOffLine)
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(826), AsTableManager.Instance.GetTbl_String(324),
					AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return;
			}

			if( AsPartyManager.Instance.IsPartyNotice)
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(1728));
				return;
			}

			BtnSound();
			AsPartySender.SendPartyCaptain( m_nSessionIdx,m_nCharUniqKey);
			
			Close();
		}
	}

	private void KickBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartyManager.Instance.BannedExitMsgBox( m_nCharUniqKey);
			BtnSound();
			Close();
		}
	}

	private void MoveBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartyManager.Instance.TargetCharUniqKey = m_nCharUniqKey;
			
			AsPartySender.SendPartyUserPosition();
			AsPartyManager.Instance.IsPartyWarpXZMsgBox = true;
			
			BtnSound();
			Close();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
