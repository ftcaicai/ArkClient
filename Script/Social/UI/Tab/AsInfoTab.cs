using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class AsInfoTab : AsSocialTab
{
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public UIButton m_SocialStoreBtn;
	public SpriteText m_NickName;
	public SpriteText m_Level;
	public SpriteText m_CharName;
	public UITextField m_SocialNoticeEdit;
	public SpriteText m_SocialNotice;
	public SpriteText m_SocialPoint;
	public SimpleSprite m_SocialPointImage = null;
	public UIButton m_PrevPageBtn = null;
	public UIButton m_NextPageBtn = null;
	public SpriteText m_PageText = null;

	private int m_CurPage = 0;
	private int m_MaxPage = 0;
	private int m_OldPage = -1;
	private string prevNotice = string.Empty;

	public override UIScrollList getList()
	{
		return m_list;
	}
	
#if false
	void OnDisable()
	{
		m_list.ClearList( true);
	}
#endif

	void OnEnable()
	{
		m_OldPage = 0;
		m_CurPage = 0;

		if( true == IsClone)
		{
			m_SocialNoticeEdit.controlIsEnabled = false;
			if( null != m_SocialPoint)
				m_SocialPoint.gameObject.SetActiveRecursively( false);

			if( null != m_SocialStoreBtn)
				m_SocialStoreBtn.gameObject.SetActiveRecursively( false);
		}
		else
		{
			m_SocialNoticeEdit.controlIsEnabled = true;
		}
	}

	void Clear()
	{
		m_list.ClearList( true);
		if( null != m_NickName)
			m_NickName.Text = string.Empty;
		m_Level.Text = string.Empty;
		m_CharName.Text = string.Empty;
		m_SocialNoticeEdit.Text = string.Empty;
		m_SocialNotice.Text = string.Empty;
		prevNotice = string.Empty;
		m_SocialPoint.Text = string.Empty;
		m_PageText.Text = "1/1";
	}

	void SetLanguageText()
	{
		if( false == IsClone)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_SocialStoreBtn.spriteText);
			m_SocialStoreBtn.Text = AsTableManager.Instance.GetTbl_String( 1201);
		}
		else
		{
			if( null != m_SocialStoreBtn)
				m_SocialStoreBtn.gameObject.SetActiveRecursively( false);
		}
	}

	// Use this for initialization
	void Start()
	{
		SetLanguageText();
		Clear();
		m_CurPage = 0;

		if( false == IsClone)
		{
			if( null != m_SocialStoreBtn)
				m_SocialStoreBtn.SetInputDelegate( SocialStoreBtnDelegate);
		}
		else
		{
			if( null != m_SocialStoreBtn)
				m_SocialStoreBtn.gameObject.SetActiveRecursively( false);
		}

		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
		
		m_SocialNoticeEdit.SetValidationDelegate( SocialNoticeValidator);


		if (AsReviewConditionManager.Instance.IsReviewCondition (eREVIEW_CONDITION.SOCIAL_REWARD) == false) 
		{
			m_SocialStoreBtn.gameObject.SetActive(false);
			m_SocialPoint.gameObject.SetActive(false);
			m_SocialPointImage.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void ClearList()
	{
		m_list.ClearList( true);
	}

	public void OnSelect()
	{
	}

	override public void Init()
	{
	}

	private void SocialStoreBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsSocialManager.Instance.OpenSocialStoreDlg();
		}
	}

	public void ChangeNotice()
	{
		if( 0 >= m_SocialNotice.Text.Length || m_SocialNotice.Text.Equals( prevNotice))		
			return;
		
		prevNotice = m_SocialNotice.Text;

		if( 0 != m_SocialNotice.Text.Length)
			AsCommonSender.SendSocialNotice( m_SocialNotice.Text);
		
	}
	
	public void SetSocialInfo( body_SC_SOCIAL_INFO socialInfo)
	{
		if( socialInfo.nUserUniqKey == AsUserInfo.Instance.LoginUserUniqueKey)//My
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == userEntity)
				return;

			if( null != m_Level)
				m_Level.Text = string.Format( "Lv.{0}", userEntity.GetProperty<int>( eComponentProperty.LEVEL).ToString());

			m_CharName.Text = userEntity.GetProperty<string>( eComponentProperty.NAME);

			m_SocialPoint.Text = AsTableManager.Instance.GetTbl_String( 1186) + ": " + socialInfo.nSocialPoint.ToString() + "/" + socialInfo.nMaxSocialPoint.ToString();
			AsSocialManager.Instance.SocialData.SocialPoint = socialInfo.nSocialPoint;
			AsSocialManager.Instance.SocialData.MaxSocialPoint = socialInfo.nMaxSocialPoint;

		//	m_HelloCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1388), socialInfo.nHelloCount, socialInfo.nMaxHelloCount);

			AsSocialManager.Instance.SocialData.HelloCount = socialInfo.nHelloCount;
			AsSocialManager.Instance.SocialData.MaxHelloCount = socialInfo.nMaxHelloCount;

			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SOCIAL_INFO));
		}
		else//clone
		{
			if( null != m_Level)
				m_Level.Text = string.Format( "Lv.{0}", AsSocialManager.Instance.SocialData.FriendItem.nLevel.ToString());

			m_CharName.Text = AsSocialManager.Instance.SocialData.FriendItem.szCharName;
		}

		string notice = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( socialInfo.szNotice));
		if( 0 == notice.Length)
			m_SocialNotice.Text = AsTableManager.Instance.GetTbl_String( 1338);
		else
			m_SocialNotice.Text = notice;

		prevNotice = m_SocialNotice.Text;
	}

	public void SetNotice( string notice)
	{
		m_SocialNotice.Text = notice;
		prevNotice = notice;
	}

	public void SetSocialPoint( int nPoint)
	{
		if( false == IsClone)
		{
			m_SocialPoint.Text = AsTableManager.Instance.GetTbl_String( 1186) + ": " + nPoint.ToString() + "/" + AsSocialManager.Instance.SocialData.MaxSocialPoint.ToString();
			AsSocialManager.Instance.SocialData.SocialPoint = nPoint;
		}
	}

	public void SetHistoryList( body1_SC_SOCIAL_HISTORY list)
	{
		if( false == gameObject.active)
			return;

		m_MaxPage = list.nMaxPage;
		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		m_list.ClearList( true);
		if( null == list.body)
			return;

		foreach( body2_SC_SOCIAL_HISTORY data in list.body)
		{
			UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;
			AsGameHistoryItem historyItem = item.gameObject.GetComponent<AsGameHistoryItem>();
			historyItem.SetHistoryData( data, IsClone);
		}

		if ( AsSocialManager.Instance.SocialUI.IsOpenSocailStoreDlg == false)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SOCIAL_INFO));
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			m_CurPage--;
			if( 0 > m_CurPage)
				m_CurPage = 0;

			RequestCurPageMember();
		}
	}

	private void NextPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			m_CurPage++;
			if( m_MaxPage <= m_CurPage + 1)
				m_CurPage = m_MaxPage - 1;

			RequestCurPageMember();
		}
	}

	private void RequestCurPageMember()
	{
		if( m_OldPage == m_CurPage)
			return;

		if( true == IsClone)
			AsCommonSender.SendSocialUiScroll( AsSocialManager.Instance.SocialData.FriendItem.nUserUniqKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY, m_CurPage, false);	//2013.02.22
		else
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY, m_CurPage, false);	//2013.02.22

		m_OldPage = m_CurPage;
	}

	public void RequestCurPageHistory()
	{
		if( true == IsClone)
			AsCommonSender.SendSocialUiScroll( AsSocialManager.Instance.SocialData.FriendItem.nUserUniqKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY, m_CurPage, false);	//2013.02.22
		else
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_HISTORY, m_CurPage, false);	//2013.02.22

		m_OldPage = m_CurPage;
	}
	
	string SocialNoticeValidator( UITextField field, string text, ref int insPos)
	{		
		// #22671 #27131
		int byteCountMax = 45;
		int charCountMax = 24;
		
		text = Regex.Replace( text, "\n", "");
		
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= byteCountMax) && ( charCount <= charCountMax))
				break;
			
			text = text.Remove( text.Length - 1);
		}
		
		return text;	
	}
}
