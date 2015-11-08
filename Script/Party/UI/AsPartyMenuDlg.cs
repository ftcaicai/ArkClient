using UnityEngine;
using System.Collections;

public class AsPartyMenuDlg : MonoBehaviour
{
	public SpriteText m_TextTitle = null;
	public UIButton m_PMSearchBtn;
	public UIButton m_PMCreateBtn;
	public UIButton m_PMListBtn;
	public UIButton m_CloseBtn;

	// Use this for initialization
	void Start()
	{
		m_PMSearchBtn.SetInputDelegate( PMSearchBtnDelegate);
		m_PMCreateBtn.SetInputDelegate( PMCreateBtnDelegate);
		m_PMListBtn.SetInputDelegate( PMListBtnDelegate);
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PMSearchBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PMCreateBtn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_PMListBtn.spriteText);
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1117);
		m_PMSearchBtn.Text = AsTableManager.Instance.GetTbl_String(2125);
		m_PMCreateBtn.Text = AsTableManager.Instance.GetTbl_String(1946);
		m_PMListBtn.Text = AsTableManager.Instance.GetTbl_String(1943);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Open()
	{
		gameObject.SetActiveRecursively( true);

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_PARTY_MENU));

		if (AsHudDlgMgr.Instance.IsOpenMainMenuDlg)
			AsHudDlgMgr.Instance.MainMenuDlg.CloseForQuestTutorial();
	}

	private void PMSearchBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			//파티 자동매칭...
			AsPartyManager.Instance.PartyUI.OpenPartyMatchingDlg();
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_SEARCH_PARTY_MENU));
			CloseForQuestTutorial();
		}
	}

	private void PMCreateBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartyManager.Instance.PartyUI.OpenPartyCreateDlg();
			Close();
		}
	}

	private void PMListBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsPartyManager.Instance.PartyUI.OpenPartyListDlg();
			Close();
		}
	}

	public void Close()
	{
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_PARTY_DLG));
		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void CloseForQuestTutorial()
	{
		gameObject.SetActiveRecursively(false);
		gameObject.active = false;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
}
