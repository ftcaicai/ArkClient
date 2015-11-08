using UnityEngine;
using System.Collections;

public class AsPartyMatchingDlg : MonoBehaviour
{
	public SpriteText m_TextTitle = null;
	public UIButton m_SelectAreaBtn;
	public UIButton m_SearchBtn;
	public UIButton m_CloseBtn;
	public SpriteText m_TextSelectTitle1 = null;
	public SpriteText m_TextSelectTitle2 = null;
	public SimpleSprite m_ImgGuide = null;

	private int m_MapIdx;

	// Use this for initialization
	void Start()
	{
		m_SelectAreaBtn.SetInputDelegate( SelectAreaBtnDelegate);
		m_SearchBtn.SetInputDelegate( SearchBtnDelegate);
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSelectTitle1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSelectTitle2);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_SelectAreaBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_SearchBtn.spriteText);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 2125);
		m_TextSelectTitle1.Text = AsTableManager.Instance.GetTbl_String( 2126);
		m_TextSelectTitle2.Text = AsTableManager.Instance.GetTbl_String( 2014);
		//m_SelectAreaBtn.Text
		m_SearchBtn.Text = AsTableManager.Instance.GetTbl_String( 2125);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Open()
	{
		m_MapIdx = TerrainMgr.Instance.GetCurMapID();
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( TerrainMgr.Instance.GetCurMapID());
		if( null == record)
			return;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String( record.getTooltipStrIdx);
		gameObject.SetActiveRecursively( true);
		
		// set texture
		Texture tex = ResourceLoad.Loadtexture( "UIPatchResources/GameGuide/Img_PartyMatchGuide");
		m_ImgGuide.renderer.material.mainTexture = tex;
	}

	public bool ClickBtnMsg( int iMapId, int iWarpIdx)
	{
		Tbl_WarpData_Record warpData = AsTableManager.Instance.GetWarpDataRecord( iWarpIdx);
		if( null == warpData)
			return false;

		if( false == warpData.isActive)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2040));
			return false;
		}

		m_MapIdx = iMapId;
		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( iMapId);
		if( null == record)
			return false;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String( record.getTooltipStrIdx);
		return true;
	}

	private void SelectAreaBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			//맵 선택 화면 표시....
			AsPartyManager.Instance.PartyUI.OpenSelectAreaDlg();
			AsPartyManager.Instance.PartyUI.USING_DLG = AsPartyUI.eUSING_DLG.PARTYMATCHING_DLG;
			AsPartyManager.Instance.PartyUI.SelectAreaDlg.SetFocusZoneMap(m_MapIdx);//#21419
		}
	}

	private void SearchBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartySender.SendPartySearch( m_MapIdx);
			AsPartyManager.Instance.PartyUI.CloseSelectAreaDlg();

			if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.SEARCH_PARTY_MATCHING) != null)
				AsCommonSender.SendClearOpneUI(OpenUIType.SEARCH_PARTY_MATCHING);

			Close();
		}
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartyManager.Instance.PartyUI.CloseSelectAreaDlg();
			AsPartyManager.Instance.PartyUI.OpenPartyList();
			Close();
		}
	}
}
