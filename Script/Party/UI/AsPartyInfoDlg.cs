using UnityEngine;
using System.Collections;
using System.Text;

public class AsPartyInfoDlg : MonoBehaviour
{
	public SpriteText m_PartyNameText = null;
	public SpriteText m_TitleInfoText1 = null;
	public SpriteText m_TitleInfoText2 = null;
	public SpriteText m_ItemDivideText = null;
	public UIButton m_JoinBtn = null;
	public UIButton m_CloseBtn = null;
	public SpriteText[] m_UserNameText = new SpriteText[AsGameDefine.MAX_PARTY_USER];
	public SpriteText[] m_UserClassText = new SpriteText[AsGameDefine.MAX_PARTY_USER];
	public SpriteText[] m_UserLevelText = new SpriteText[AsGameDefine.MAX_PARTY_USER];

	public int m_nPartyIdx = 0;
	private bool m_bNotice = false;
	public SpriteText m_SelectArea = null;

	// Use this for initialization
	void Start()
	{
		m_TitleInfoText1.Text = AsTableManager.Instance. GetTbl_String(1951);
		m_TitleInfoText2.Text = AsTableManager.Instance. GetTbl_String(1949);

		m_JoinBtn.Text = AsTableManager.Instance. GetTbl_String(1950);

		m_JoinBtn.SetInputDelegate( JoinBtnDelegate);
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetData( AS_SC_PARTY_DETAIL_INFO _info, bool bNotice = false)
	{
		m_bNotice = bNotice;
		m_nPartyIdx = _info.nPartyIdx;
		m_PartyNameText.Text = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( _info.sOption.szPartyName));

		if( _info.sOption.eDivide == (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_FREE)
			m_ItemDivideText.Text = AsTableManager.Instance. GetTbl_String(1953); //Free

		if( _info.sOption.eDivide == (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_ORDER)
			m_ItemDivideText.Text = AsTableManager.Instance. GetTbl_String(1952);//Rotation
		
		if( _info.sOption.eDivide == (int)ePARTYITEMDIVIDE.ePARTYITEMDIVIDE_EQUIP )// #23004
			m_ItemDivideText.Text = AsTableManager.Instance.GetTbl_String(1250);//Equip
		
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( _info.nMapIdx);
		if( null != record)
			m_SelectArea.Text = AsTableManager.Instance. GetTbl_String( record.getTooltipStrIdx);

		for( int i = 0; i < AsGameDefine.MAX_PARTY_USER; ++i)
		{
			if( i < _info.nUserCnt)
			{
				switch( (eCLASS)_info.body[i].eClass)
				{
				case eCLASS.DIVINEKNIGHT:	m_UserClassText[i].Text = AsTableManager.Instance. GetTbl_String(1054);	break;
				case eCLASS.MAGICIAN:	m_UserClassText[i].Text = AsTableManager.Instance. GetTbl_String(1055);	break;
				case eCLASS.CLERIC:	m_UserClassText[i].Text = AsTableManager.Instance. GetTbl_String(1057);	break;
				case eCLASS.HUNTER:	m_UserClassText[i].Text = AsTableManager.Instance. GetTbl_String(1056);	break;
				case eCLASS.ASSASSIN:	m_UserClassText[i].Text = AsTableManager.Instance. GetTbl_String(1058);	break;
				default:	m_UserClassText[i].Text = "Error";	break;
				}

				StringBuilder sb = new StringBuilder();
				sb.Insert( 0,"Lv.");
				sb.Append( _info.body[i].nLevel.ToString());

				m_UserLevelText[i].Text = sb.ToString();

				m_UserNameText[i].Text = AsUtil.GetRealString((System.Text.UTF8Encoding.UTF8.GetString( _info.body[i].szCharName)));
			}
			else
			{
				m_UserClassText[i].Text = "";
				m_UserLevelText[i].Text = "";
				m_UserNameText[i].Text = "";
			}
		}
	}

	private void JoinBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			AsPartySender.SendPartyJoinRequest( m_nPartyIdx);	//#21321
			AsPartyManager.Instance.SendNoticePartyIdx = 0;
			AsPartyManager.Instance.PartyUI.PartyListDlg.Close();//#21346
			AsPartyManager.Instance.PartyUI.CloseSelectAreaDlg(); //#21346
			Destroy( gameObject);
		}
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
			Destroy( gameObject);
		}
	}
}
