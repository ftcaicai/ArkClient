using UnityEngine;
using System;
using System.Collections;

public class AsPartyCreateDlg : MonoBehaviour
{
	public enum ePARTYPUBLIC
	{
		ePARTYPUBLIC_PUBLIC = 0,
		ePARTYPUBLIC_PRIVATE,

		ePARTYPUBLIC_MAX
	};

	const int MAX_USER_INDEX = 3;

	public UITextField m_PartyName = null;

	public UIButton m_SelectAreaBtn;
	public UIButton m_PartyCreateBtn;
	public UIButton m_ComformBtn;
	public UIButton m_CloseBtn;
	public SpriteText m_TextTitle = null;
	public SpriteText m_TextSelectTitle1 = null;
	public SpriteText m_TextSelectTitle2 = null;
	public SpriteText m_TextSelectTitle3 = null;
	public SpriteText m_TextSelectTitle4 = null;
	public SpriteText m_TextSelectTitle5 = null;

	public UIRadioBtn[] m_PublicRadioBtn;
	bool m_bPublic = false;
	public UIRadioBtn[] m_ItemDivideRadioBtn;
	int m_nItemDivide;

	public UIButton[] m_nMaxUserBtn = new UIButton[MAX_USER_INDEX];
	int m_nMaxUser;

	public SimpleSprite[] m_RadioBtnOn = null;
	public SimpleSprite[] m_RadioBtnOff = null;

	private bool m_IsBack = false;
	public bool IsBack
	{
		get	{ return m_IsBack; }
		set	{ m_IsBack = value; }
	}

	private int m_MapIdx;

	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PartyName);

		m_SelectAreaBtn.SetInputDelegate( SelectAreaBtnDelegate);
		m_PartyCreateBtn.SetInputDelegate( CreateBtnDelegate);
		m_ComformBtn.SetInputDelegate( ConformBtnDelegate);
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_FREE -1].SetInputDelegate( FreeBtnDelegate);
		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER -1 ].SetInputDelegate( OrderBtnDelegate);
		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP -1 ].SetInputDelegate( EquipBtnDelegate);
		
		m_PublicRadioBtn[(int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC ].SetInputDelegate( PublicBtnDelegate);
		m_PublicRadioBtn[(int)ePARTYPUBLIC.ePARTYPUBLIC_PRIVATE].SetInputDelegate( PrivateBtnDelegate);

		m_nMaxUserBtn[0].SetInputDelegate( MemberLimit2BtnDelegate);
		m_nMaxUserBtn[1].SetInputDelegate( MemberLimit3BtnDelegate);
		m_nMaxUserBtn[2].SetInputDelegate( MemberLimit4BtnDelegate);

		SetVisibleMaxUserBtn( 4);
		//SetVisibleItemDividBtn( (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER -1);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_FREE -1].spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER -1 ].spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSelectTitle1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSelectTitle2);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSelectTitle3);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PartyCreateBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_ComformBtn.spriteText);

		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_FREE -1].Text = AsTableManager.Instance.GetTbl_String(1953); //Free
		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER -1].Text = AsTableManager.Instance.GetTbl_String(1952);//Rotation
		m_ItemDivideRadioBtn[(int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP -1].Text = AsTableManager.Instance.GetTbl_String(1250);//Equip
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1117);

		m_TextSelectTitle1.Text = AsTableManager.Instance.GetTbl_String(1118);
		m_TextSelectTitle2.Text = AsTableManager.Instance.GetTbl_String(2014);
		m_TextSelectTitle3.Text = AsTableManager.Instance.GetTbl_String(1122);
		m_TextSelectTitle4.Text = AsTableManager.Instance.GetTbl_String(1121);
		m_TextSelectTitle5.Text = AsTableManager.Instance.GetTbl_String(1949);
		m_nMaxUserBtn[0].Text = AsTableManager.Instance.GetTbl_String(1956);
		m_nMaxUserBtn[1].Text = AsTableManager.Instance.GetTbl_String(1955);
		m_nMaxUserBtn[2].Text = AsTableManager.Instance.GetTbl_String(1954);

		m_PartyCreateBtn.Text = AsTableManager.Instance.GetTbl_String(1133);
		m_ComformBtn.Text = AsTableManager.Instance.GetTbl_String(1134);

		m_PublicRadioBtn[(int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC ].Text = AsTableManager.Instance.GetTbl_String(1119); //공개.
		m_PublicRadioBtn[(int)ePARTYPUBLIC.ePARTYPUBLIC_PRIVATE].Text = AsTableManager.Instance.GetTbl_String(1120);//비공개.

		m_PartyName.SetValidationDelegate( OnValidateName);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_SelectAreaBtn.spriteText);
	}

	public void SetPartyOption()
	{
		if( !gameObject.active)
			return;

		m_MapIdx = AsPartyManager.Instance.PartyMapIdx;
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( m_MapIdx);
		if( null == record)
			return;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String(record.getTooltipStrIdx);

		if( AsPartyManager.Instance.IsCaptain)
		{
			m_ComformBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_ComformBtn.controlIsEnabled = true;

			m_PartyName.controlIsEnabled = true;

			foreach( UIRadioBtn radioBtn in m_ItemDivideRadioBtn)
			{
				radioBtn.controlIsEnabled = true;
				radioBtn.spriteText.Color = Color.black;
			}

			foreach( UIRadioBtn radioBtn in m_PublicRadioBtn)
			{
				radioBtn.controlIsEnabled = true;
				radioBtn.spriteText.Color = Color.black;
			}

			foreach( UIButton maxUserBtn in m_nMaxUserBtn)
				maxUserBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);

			m_SelectAreaBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_SelectAreaBtn.spriteText.Color = Color.black;
			m_SelectAreaBtn.controlIsEnabled = true;
		}
		else
		{
			m_ComformBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_PartyName.controlIsEnabled = false;

			foreach( UIRadioBtn radioBtn in m_ItemDivideRadioBtn)
			{
				radioBtn.controlIsEnabled = false;
				radioBtn.spriteText.Color = Color.gray;
			}

			foreach( UIRadioBtn radioBtn in m_PublicRadioBtn)
			{
				radioBtn.controlIsEnabled = false;
				radioBtn.spriteText.Color = Color.gray;
			}

			foreach( UIButton maxUserBtn in m_nMaxUserBtn)
				maxUserBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);

			m_SelectAreaBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_SelectAreaBtn.spriteText.Color = Color.gray;
			m_SelectAreaBtn.controlIsEnabled = false;
		}

		string strPartyName = AsUtil.GetRealString( ( System.Text.UTF8Encoding.UTF8.GetString( AsPartyManager.Instance.PartyOption.szPartyName)));
		m_PartyName.Text = strPartyName;

		SetVisibleItemDividBtn( (int)AsPartyManager.Instance.PartyOption.eDivide - 1);
		SetVisibleMaxUserBtn( AsPartyManager.Instance.PartyOption.nMaxUser);
		if( AsPartyManager.Instance.PartyOption.bPublic)
			SetVisiblePublicBtn( (int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC);
		else
			SetVisiblePublicBtn( (int)ePARTYPUBLIC.ePARTYPUBLIC_PRIVATE);
	}

	// Update is called once per frame
	void Update()
	{
	}

	void Clear()
	{
		m_IsBack = false;
		foreach( SimpleSprite spr in m_RadioBtnOff)
		{
			spr.gameObject.SetActiveRecursively( false);
		}

		foreach( SimpleSprite spr in m_RadioBtnOn)
		{
			spr.gameObject.SetActiveRecursively( false);
		}
	}

	public void Close()
	{
		Clear();
		AsPartyManager.Instance.PartyUI.CloseSelectAreaDlg();

		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void Open()
	{
		Clear();
		gameObject.SetActiveRecursively( true);
		gameObject.active = true;
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		m_MapIdx = TerrainMgr.Instance.GetCurMapID();
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( TerrainMgr.Instance.GetCurMapID());
		if( null == record)
			return;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String(record.getTooltipStrIdx);

		m_PartyName.controlIsEnabled = true;

		foreach( UIRadioBtn radioBtn in m_ItemDivideRadioBtn)
		{
			radioBtn.controlIsEnabled = true;
			radioBtn.spriteText.Color = Color.black;
		}

		foreach( UIButton maxUserBtn in m_nMaxUserBtn)
			maxUserBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);

		string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
		m_PartyName.Text = userName + AsTableManager.Instance.GetTbl_String(1357);

		SetVisible( m_ComformBtn.gameObject, false);

		SetVisibleMaxUserBtn( 4);
		SetVisibleItemDividBtn( (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP -1);
		SetVisiblePublicBtn( (int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC);
	}

	public void OpenEdit()
	{
		Clear();

		gameObject.SetActiveRecursively( true);
		gameObject.active = true;
		SetVisible( m_PartyCreateBtn.gameObject, false);
		SetPartyOption();

		m_IsBack = true;
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	public bool ConvertToPartyOption()
	{
		if( m_PartyName.Text.Length == 0)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == userEntity)
				return false;

			string userName = userEntity.GetProperty<string>( eComponentProperty.NAME);
			m_PartyName.Text = userName + AsTableManager.Instance.GetTbl_String(1357);
		}
		AsPartyManager.Instance.IsDivideItemChange = false;
		if(AsPartyManager.Instance.PartyOption.eDivide != m_nItemDivide + 1)
			AsPartyManager.Instance.IsDivideItemChange = true;
		
		AsPartyManager.Instance.PartyOption.eDivide = m_nItemDivide + 1;

		//파티 인원..
		if( m_nMaxUser < AsPartyManager.Instance.GetPartyMemberList().Count)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117),AsTableManager.Instance.GetTbl_String(875),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(875), eCHATTYPE.eCHATTYPE_SYSTEM);
			return false;
		}
		else
		{
			AsPartyManager.Instance.PartyOption.nMaxUser = m_nMaxUser;
		}

		AsPartyManager.Instance.PartyMapIdx = m_MapIdx;
		AsPartyManager.Instance.PartyOption.bPublic = m_bPublic;

		if( AsTableManager.Instance.TextFiltering_Name( m_PartyName.Text) == true)
		{
			byte[] szPartyName = System.Text.UTF8Encoding.UTF8.GetBytes( m_PartyName.Text.ToCharArray(), 0, m_PartyName.Text.Length);
			Array.Clear( AsPartyManager.Instance.PartyOption.szPartyName, 0, (int)AsGameDefine.MAX_PARTYNAME_LEN + 1);

			int length = ( AsGameDefine.MAX_PARTYNAME_LEN <szPartyName.Length) ? AsGameDefine.MAX_PARTYNAME_LEN : szPartyName.Length;
			System.Buffer.BlockCopy( szPartyName, 0, AsPartyManager.Instance.PartyOption.szPartyName, 0, length);
			AsPartyManager.Instance.PartyName = AsUtil.GetRealString( ( System.Text.UTF8Encoding.UTF8.GetString( AsPartyManager.Instance.PartyOption.szPartyName)));	// m_PartyName.Text;

			return true;
		}
		else
		{
			string strPartyName = AsUtil.GetRealString( ( System.Text.UTF8Encoding.UTF8.GetString( AsPartyManager.Instance.PartyOption.szPartyName)));
			m_PartyName.Text = strPartyName;

			AsNotify.Instance.MessageBox( "", AsTableManager.Instance.GetTbl_String(364), this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
		}

		return false;
	}

	private void CreateBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if( ConvertToPartyOption())
			{
				Close();
				AsPartyManager.Instance.PartyCreate();
			}
		}
	}

	private void ConformBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( AsPartyManager.Instance.IsCaptain)
			{
				if( m_nMaxUser < AsPartyManager.Instance.GetPartyMemberList().Count)
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117), AsTableManager.Instance.GetTbl_String(875), this, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
					return ;
				}

				if( ConvertToPartyOption())
				{
					AsPartySender.SendPartySetting( AsPartyManager.Instance.PartyMapIdx, AsPartyManager.Instance.PartyOption);
					Close();
				}
			}
		}
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			//실제로 "뒤로가기" 버튼 역활.
			if( m_IsBack)//Edit Mode
			{
				if( CheckChangeOption())
				{
					AsPartyManager.Instance.PartyOptionMsgBox();
				}
				else
				{
					AsPartyManager.Instance.PartyUI.OpenPartyList();
					Close();
				}
			}
			else
			{
				if( CheckChangeOption())
					AsPartyManager.Instance.PartyOptionMsgBox();
				else
				{
					AsPartyManager.Instance.PartyUI.OpenPartyList();
					Close();
				}
			}
		}
	}

	private bool CheckChangeOption()
	{
		if( !AsPartyManager.Instance.IsCaptain)
			return false;

		bool bChange = false;
		string strPartyName = AsUtil.GetRealString( AsPartyManager.Instance.PartyName);
		if( strPartyName.CompareTo( m_PartyName.Text) == 0)
			bChange = false;
		else
			bChange = true;

		if( AsPartyManager.Instance.PartyMapIdx != m_MapIdx)
			bChange = true;
		if( AsPartyManager.Instance.PartyOption.eDivide != m_nItemDivide + 1)
			bChange = true;
		if( AsPartyManager.Instance.PartyOption.nMaxUser != m_nMaxUser)
			bChange = true;
		if( AsPartyManager.Instance.PartyOption.bPublic != m_bPublic)
			bChange = true;

		return bChange;
	}

	private void SetVisibleItemDividBtn( int index)
	{
		foreach( UIRadioBtn radioBtn in m_ItemDivideRadioBtn)
			radioBtn.SetState( 1);

		m_ItemDivideRadioBtn[index].SetState( 0);
		m_nItemDivide = index;
	}

	private void SetVisiblePublicBtn( int index)
	{
		foreach( UIRadioBtn radioBtn in m_PublicRadioBtn)
			radioBtn.SetState( 1);

		m_PublicRadioBtn[index].SetState( 0);
		if( index == (int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC)
			m_bPublic = true;
		else
			m_bPublic = false;
	}

	private void SetVisibleMaxUserBtn( int count)
	{
		int index = count - 2;
		if( count < AsPartyManager.Instance.GetPartyMemberList().Count)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1117),AsTableManager.Instance.GetTbl_String(875),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(875), eCHATTYPE.eCHATTYPE_SYSTEM);
			index = m_nMaxUser - 2;
		}
		else
			m_nMaxUser = count;

		foreach( UIButton radioBtn in m_nMaxUserBtn)
		{
			radioBtn.SetState( 1);
		}

		foreach( SimpleSprite spr in m_RadioBtnOff)
		{
			spr.gameObject.SetActiveRecursively( true);
		}

		foreach( SimpleSprite spr in m_RadioBtnOn)
		{
			spr.gameObject.SetActiveRecursively( false);
		}

		m_nMaxUserBtn[index].SetState( 0);

		m_RadioBtnOff[index].gameObject.SetActiveRecursively( false);
		m_RadioBtnOn[index].gameObject.SetActiveRecursively( true);
	}

	private void PublicBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisiblePublicBtn( (int)ePARTYPUBLIC.ePARTYPUBLIC_PUBLIC);
		}
	}

	private void PrivateBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisiblePublicBtn( (int)ePARTYPUBLIC.ePARTYPUBLIC_PRIVATE);
		}
	}

	private void OrderBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleItemDividBtn( (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER -1);
		}
	}

	private void FreeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleItemDividBtn( (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_FREE -1);
		}
	}
	
	private void EquipBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleItemDividBtn( (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP -1);
		}
	}

	
	private void MemberLimit2BtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleMaxUserBtn( 2);
		}
	}

	private void MemberLimit3BtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleMaxUserBtn( 3);
		}
	}

	private void MemberLimit4BtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			SetVisibleMaxUserBtn( 4);
		}
	}

	//#20502
	private string OnValidateName( UITextField field, string text, ref int insPos)
	{
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			if( byteCount <= AsGameDefine.MAX_PARTYNAME_LEN)
				break;

			text = text.Remove( text.Length - 1);
		}
		
		// #22671
		int index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);			
		
		return text;
	}

	public bool	ClickBtnMsg( int iMapId, int iWarpIdx)
	{
		Tbl_WarpData_Record warpData = AsTableManager.Instance.GetWarpDataRecord( iWarpIdx);
		if( null == warpData)
			return false;

		if( false == warpData.isActive)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(2040));
			return false;
		}

		m_MapIdx = iMapId;
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( iMapId);
		if( null == record)
			return false;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String(record.getTooltipStrIdx);
		
		return true;
	}

	private void SelectAreaBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			//맵 선택 화면 표시....
			AsPartyManager.Instance.PartyUI.OpenSelectAreaDlg();
			AsPartyManager.Instance.PartyUI.USING_DLG = AsPartyUI.eUSING_DLG.PARTYCREATE_DLG;
			AsPartyManager.Instance.PartyUI.SelectAreaDlg.SetFocusZoneMap(m_MapIdx);//#21419
		}
	}
}
