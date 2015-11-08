//#define USE_OLD_COSTUME
using UnityEngine;
using System.Collections;

public class PlayerPartsOptionDlg : MonoBehaviour 
{
	public SpriteText textTitle;
	
	public SpriteText weaponTitle;
	public SpriteText[] weaponText;
	
	public SpriteText headTitle;
	public SpriteText[] headText;
	
	public SpriteText ArmorTitle;
	public SpriteText[] ArmorText;
	
	public SpriteText glovesTitle;	
	public SpriteText[] glovesText;
	
	public SpriteText pointTitle;	
	public SpriteText[] pointText;
	
	public SpriteText etcTitle;	
	public SpriteText wingText;	
	public SpriteText fairyText;
	
	public UIButton[] weaponBtn;
	public GameObject[] weaponImg;	
	
	public UIButton[] headBtn;
	public GameObject[] headImg;	
	
	public UIButton[] ArmorBtn;
	public GameObject[] ArmorImg;
	
	
	public UIButton[] glovesBtn;
	public GameObject[] glovesImg;	
	
	public UIButton[] pointBtn;
	public GameObject[] pointImg;
	
	
	public UIButton wingBtn;
	public GameObject wingImg;	
	
	public UIButton fairyBtn;
	public GameObject fairyImg;
	
	
	
	public UIButton okBtn;
	public UIButton closeBtn;
	
	private int m_isCostumeOnOff = 0;
	
	protected void SetUIRadio( GameObject[] radioImg, int index )
	{
		if( radioImg.Length <= index )
		{
			Debug.LogError("PlayerPartsOptionDlg::SetUIRadio()[ radioImg.Length <= index ] " + index );
			return;
		}
		
		for( int i=0; i<radioImg.Length; ++i )
		{
			if( i == index )
			{
				radioImg[i].active = true;
			}
			else
			{
				radioImg[i].active = false;
			}
			
		}
	}
	
	
	
	protected void SetUICheck( GameObject checkImg, bool _isActive )
	{		
		checkImg.active = _isActive;		
	}
	
	protected bool isCheckActive( GameObject checkImg )
	{
		return checkImg.active;
	}

	

	public void Open()
	{	
#if USE_OLD_COSTUME
#else
		AsUserEntity _userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == _userEntity )
			return;		
		
		m_isCostumeOnOff = _userEntity.isCostumeOnOff;
		SetUIRadio( weaponImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Weapon, _userEntity.isCostumeOnOff ) == true ? 1 : 0 );
		SetUIRadio( ArmorImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Armor, _userEntity.isCostumeOnOff ) == true ? 1 : 0 );		
		SetUIRadio( glovesImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Gloves, _userEntity.isCostumeOnOff ) == true ? 1 : 0 );
		SetUIRadio( pointImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Point, _userEntity.isCostumeOnOff ) == true ? 1 : 0 );	
		SetUICheck( wingImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Wing, _userEntity.isCostumeOnOff ) );
		SetUICheck( fairyImg, PartsRoot.IsCostumeOn( Item.eEQUIP.Fairy, _userEntity.isCostumeOnOff ) );
		
		if( PartsRoot.IsCostumeOn( Item.eEQUIP.Head, _userEntity.isCostumeOnOff ) )
		{
			SetUIRadio( headImg, 2 );
		}		
		else if( PartsRoot.IsEquipView( Item.eEQUIP.Head, _userEntity.isCostumeOnOff ) )
		{
			SetUIRadio( headImg, 1 );
		}
		else 
		{
			SetUIRadio( headImg, 0 );
		}
#endif
	}

	
	public void Close()
	{
		
	}
#if USE_OLD_COSTUME
#else
	void Awake()
	{	
		weaponTitle.Text = AsTableManager.Instance.GetTbl_String(1821);	
		headTitle.Text = AsTableManager.Instance.GetTbl_String(1822);	
		ArmorTitle.Text = AsTableManager.Instance.GetTbl_String(1823);	
		glovesTitle.Text = AsTableManager.Instance.GetTbl_String(1824);	
		pointTitle.Text = AsTableManager.Instance.GetTbl_String(1825);	
		etcTitle.Text = AsTableManager.Instance.GetTbl_String(1826);
		
		
		
		textTitle.Text = AsTableManager.Instance.GetTbl_String(1820);
		weaponBtn[0].SetInputDelegate( weaponBtnDelegate_1 );
		weaponBtn[1].SetInputDelegate( weaponBtnDelegate_2 );		
		weaponText[0].Text = AsTableManager.Instance.GetTbl_String(1827);
		weaponText[1].Text = AsTableManager.Instance.GetTbl_String(1828);
		
		
		headBtn[0].SetInputDelegate( headBtnDelegate_1 );
		headBtn[1].SetInputDelegate( headBtnDelegate_2 );
		headBtn[2].SetInputDelegate( headBtnDelegate_3 );	
		headText[0].Text = AsTableManager.Instance.GetTbl_String(1829);
		headText[1].Text = AsTableManager.Instance.GetTbl_String(1827);
		headText[2].Text = AsTableManager.Instance.GetTbl_String(1828);
	
		
		ArmorBtn[0].SetInputDelegate( ArmorBtnDelegate_1 );
		ArmorBtn[1].SetInputDelegate( ArmorBtnDelegate_2 );	
		ArmorText[0].Text = AsTableManager.Instance.GetTbl_String(1827);
		ArmorText[1].Text = AsTableManager.Instance.GetTbl_String(1828);
		
		glovesBtn[0].SetInputDelegate( glovesBtnDelegate_1 );
		glovesBtn[1].SetInputDelegate( glovesBtnDelegate_2 );	
		glovesText[0].Text = AsTableManager.Instance.GetTbl_String(1827);
		glovesText[1].Text = AsTableManager.Instance.GetTbl_String(1828);
		
		pointBtn[0].SetInputDelegate( pointBtnDelegate_1 );
		pointBtn[1].SetInputDelegate( pointBtnDelegate_2 );	
		pointText[0].Text = AsTableManager.Instance.GetTbl_String(1827);
		pointText[1].Text = AsTableManager.Instance.GetTbl_String(1828);
		
		wingBtn.SetInputDelegate( wingBtnDelegate );
		fairyBtn.SetInputDelegate( fairyBtnDelegate );
		wingText.Text = AsTableManager.Instance.GetTbl_String(1830);
		fairyText.Text = AsTableManager.Instance.GetTbl_String(1831);
		
		okBtn.SetInputDelegate( okBtnDelegate );		
		closeBtn.SetInputDelegate( closeBtnDelegate );	
		
		okBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(4201);
	}
	public void okBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsUserEntity _userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == _userEntity )
				return;			
			
			if( m_isCostumeOnOff != _userEntity.isCostumeOnOff )
			{	
				AsCommonSender.SendConstumeOnOff( m_isCostumeOnOff );				
			}
			
			AsHudDlgMgr.Instance.ClosePlayerPartsOptionDlg();
		}
	}
	public void closeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsHudDlgMgr.Instance.ClosePlayerPartsOptionDlg();
		}
	}
	
	
	public void weaponBtnDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Weapon, m_isCostumeOnOff ) )
			{
				SetUIRadio( weaponImg, 0 );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Weapon;
			}
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void weaponBtnDelegate_2( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{			
			if( false == PartsRoot.IsCostumeOn( Item.eEQUIP.Weapon, m_isCostumeOnOff ) )
			{
				SetUIRadio( weaponImg, 1 );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Weapon;
			}
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	
	public void headBtnDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( PartsRoot.IsEquipView( Item.eEQUIP.Head, m_isCostumeOnOff ) )
			{
				SetUIRadio( headImg, 0 );
				
				if( 0 != (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_costume) )
				{					
					m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Head_costume;
				}
				if( 0 != (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_normal) )
				{
					m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Head_normal;
				}
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void headBtnDelegate_2( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			SetUIRadio( headImg, 1 );
				
			if( 0 != (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_costume) )
			{					
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Head_costume;
			}
			if( 0 == (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_normal) )
			{
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Head_normal;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void headBtnDelegate_3( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			SetUIRadio( headImg, 2 );
				
			if( 0 == (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_costume) )
			{					
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Head_costume;
			}
			if( 0 != (m_isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_normal) )
			{
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Head_normal;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void ArmorBtnDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Armor, m_isCostumeOnOff ) )
			{
				SetUIRadio( ArmorImg, 0 );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Armor;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void ArmorBtnDelegate_2( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false == PartsRoot.IsCostumeOn( Item.eEQUIP.Armor, m_isCostumeOnOff ) )
			{
				SetUIRadio( ArmorImg, 1 );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Armor;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void glovesBtnDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Gloves, m_isCostumeOnOff ) )
			{
				SetUIRadio( glovesImg, 0 );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Gloves;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void glovesBtnDelegate_2( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false == PartsRoot.IsCostumeOn( Item.eEQUIP.Gloves, m_isCostumeOnOff ) )
			{
				SetUIRadio( glovesImg, 1 );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Gloves;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void pointBtnDelegate_1( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Point, m_isCostumeOnOff ) )
			{
				SetUIRadio( pointImg, 0 );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Point;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	
	public void pointBtnDelegate_2( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false == PartsRoot.IsCostumeOn( Item.eEQUIP.Point, m_isCostumeOnOff ) )
			{
				SetUIRadio( pointImg, 1 );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Point;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void wingBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Wing, m_isCostumeOnOff ) )
			{
				SetUICheck( wingImg, false );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Wing;
			}
			else
			{
				SetUICheck( wingImg, true );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Wing;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	public void fairyBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == PartsRoot.IsCostumeOn( Item.eEQUIP.Fairy, m_isCostumeOnOff ) )
			{
				SetUICheck( fairyImg, false );
				m_isCostumeOnOff -= (int)eITEM_PARTS_VIEW.Fairy;
			}
			else
			{
				SetUICheck( fairyImg, true );
				m_isCostumeOnOff += (int)eITEM_PARTS_VIEW.Fairy;
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
#endif
}
