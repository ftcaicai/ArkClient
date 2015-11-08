//#define _USE_CONDITION_
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsFriendTab : AsSocialTab
{
	public SpriteText m_HelloCount;
	public AsSocialMenu m_SocialMenu;
	public UIButton m_FindFriendBtn = null;
	public UIRadioBtn m_OnlineFilterBtn = null;
	private bool m_bOnlineMember = false;
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public SpriteText noFriend = null;
	public UIButton m_ConditionBtn = null;
	public UIProgressBar m_nRateProgress = null;
	public SpriteText m_ConditionText = null;

	public UIButton m_PrevPageBtn = null;
	public UIButton m_NextPageBtn = null;
	public int DRAG_SENSITIVITY = 5;
	public SpriteText m_PageText = null;
	private int m_CurPage = 0;
	private int m_MaxPage = 0;
	private int m_OldPage = -1;

	public bool OnlineMember
	{
		get { return m_bOnlineMember; }
	}

	public override UIScrollList getList()
	{
		return m_list;
	}

	void SetLanguageText()
	{
		if( null != m_OnlineFilterBtn)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_OnlineFilterBtn.spriteText);
			m_OnlineFilterBtn.Text = AsTableManager.Instance.GetTbl_String( 1190);
		}

		if( null != m_FindFriendBtn)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_FindFriendBtn.spriteText);
			m_FindFriendBtn.Text = AsTableManager.Instance.GetTbl_String( 1189);
		}
		#if _USE_CONDITION_
		if( null != m_ConditionBtn)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_ConditionBtn.spriteText);
			m_ConditionBtn.Text = AsTableManager.Instance.GetTbl_String( 1908);
		}
		#endif		
	}

	// Use this for initialization
	void Start()
	{
		noFriend.Text = AsTableManager.Instance.GetTbl_String( 1470);
		SetLanguageText();
		m_CurPage = 0;
		if( m_FindFriendBtn != null)
			m_FindFriendBtn.SetInputDelegate( FindFriendBtnDelegate);
		m_PrevPageBtn.SetInputDelegate( PrevPageDelegate);
		m_NextPageBtn.SetInputDelegate( NextPageDelegate);
		#if _USE_CONDITION_
		m_ConditionBtn.SetInputDelegate( ConditionBtnDelegate);
		#endif
	}

	void OnEnable()
	{		
		m_OldPage = 0;
		m_CurPage = 0;
		noFriend.gameObject.SetActiveRecursively( 0 == m_list.Count);
		SetVisibleSocialMenu( false);
		
		if(IsClone)
		{
		//	m_OnlineFilterBtn.SetControlState( UIRadioBtn.CONTROL_STATE.DISABLED );
			m_OnlineFilterBtn.controlIsEnabled = false;
			
			m_FindFriendBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED );
			m_FindFriendBtn.controlIsEnabled = false;
			
		}
		#if _USE_CONDITION_	
		m_ConditionBtn.gameObject.SetActiveRecursively( true);
		m_ConditionText.gameObject.SetActiveRecursively(true);
		m_nRateProgress.gameObject.SetActiveRecursively(true);
		#else
		m_ConditionBtn.gameObject.SetActiveRecursively(false);
		m_ConditionText.gameObject.SetActiveRecursively(false);
		m_nRateProgress.gameObject.SetActiveRecursively(false);
		#endif
	}


	// Update is called once per frame
	void Update()
	{
	}

	public void SetVisibleSocialMenu( bool bVisible)
	{
		if( m_SocialMenu == null)
			return;

		if( bVisible)
			m_SocialMenu.Open();
		else
			m_SocialMenu.Close();
	}

	override public void Init()
	{
		if( m_SocialMenu != null)
			m_SocialMenu.Close();
	}

	public void OnSelect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		if( m_SocialMenu == null)
			return;

		SetVisibleSocialMenu( true);
		UIListButton item = m_list.LastClickedControl as UIListButton;
		AsFriendItem baseFriendItem = item.gameObject.GetComponent<AsFriendItem>();

		m_SocialMenu.SetFriendItem( baseFriendItem.FriendData);
	}



	private void ConditionBtnDelegate( ref POINTER_INFO ptr)
	{
		if(IsClone)
		 return;
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(4206), this,
				"SendFrinedCondition_Ok", "SendFrinedCondition_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);	//#21456	
		}
	}
	
	void SendFrinedCondition_Ok()
	{
		AsCommonSender.SendFrinedCondition( );
	}
	
	void SendFrinedCondition_Cancel()
	{
		
	}
		
	private void FindFriendBtnDelegate( ref POINTER_INFO ptr)
	{
		if(IsClone)
		 return;
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsSocialManager.Instance.OpenFindFriendDlg();
		}
	}

	private void OnOnlineFilterBtnDelegate( ref POINTER_INFO ptr)
	{
		if(IsClone)
		 return;
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Debug.Log( "OnOnlineFilterBtn");

			m_bOnlineMember = !m_bOnlineMember;
			m_OnlineFilterBtn.Value = m_bOnlineMember;
			m_CurPage = 0;
			m_OldPage = -1;
			RequestCurPageMember();
		}
	}

	private void OnOnlineFilterBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Debug.Log( "OnOnlineFilterBtn");

		m_bOnlineMember = !m_bOnlineMember;
		m_OnlineFilterBtn.Value = m_bOnlineMember;
		m_CurPage = 0;
		m_OldPage = -1;
		RequestCurPageMember();
	}

	private void ConditionProcess(body1_SC_FRIEND_LIST data)
	{
		m_HelloCount.Text = string.Format( AsTableManager.Instance.GetTbl_String(1388), data.nHelloCount, data.nMaxHelloCount);
		#if _USE_CONDITION_	
		string conditionCount = string.Format(":{0}/{1}",data.nChargeConditionCount ,data.nMaxChargeConditionCount);
		m_ConditionText.Text = string.Format( AsTableManager.Instance.GetTbl_String(1906), conditionCount);
		m_nRateProgress.Value = (float)( data.nChargeConditionPointRate * 0.01f);

		if(data.nChargeConditionCount == 0)
		{
			AsUtil.SetButtonState( m_ConditionBtn , UIButton.CONTROL_STATE.DISABLED );
		}
		else
		{
			AsUtil.SetButtonState( m_ConditionBtn , UIButton.CONTROL_STATE.NORMAL );
		}
		#endif
	}
	public void SetFriendList( body1_SC_FRIEND_LIST list)
	{
		ConditionProcess(list);

		m_MaxPage =  list.nMaxPage;
		if( m_MaxPage < m_CurPage + 1)
			m_CurPage = m_MaxPage - 1; //OnlineFilter

		m_PageText.Text = string.Format( "{0}/{1}", m_CurPage + 1, m_MaxPage);

		//public SpriteText m_HelloCount;

		m_list.ClearList( true);
		if( list.body == null)
			return;

		foreach( body2_SC_FRIEND_LIST data in list.body)
		{
			UIListItem item = m_list.CreateItem( m_objChoiceItem) as UIListItem;

			AsFriendItem friendItem = item.gameObject.GetComponent<AsFriendItem>();
			friendItem.SetFriendData( data, IsClone);
		}

		noFriend.gameObject.SetActiveRecursively( 0 == list.nCnt);
	}

	public void RequestFirstPage()
	{
		m_CurPage = 0;
		m_OldPage = -1;
		RequestCurPageMember();
	}

	public void RequestCurrentPage()
	{
		if( !gameObject.active)
			return;

		if( IsClone)
			AsCommonSender.SendSocialUiScroll( AsSocialManager.Instance.SocialData.FriendItem.nUserUniqKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND, m_CurPage, m_OnlineFilterBtn.Value);	//2013.02.26
		else
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND, m_CurPage, m_OnlineFilterBtn.Value);	//2013.02.26

		m_OldPage = m_CurPage;
	}

	private void PrevPageDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Debug.Log( "OnOnlineFilterBtn");

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
			Debug.Log( "OnNextPageBtn");

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

		if( IsClone)
			AsCommonSender.SendSocialUiScroll( AsSocialManager.Instance.SocialData.FriendItem.nUserUniqKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND, m_CurPage, false);	//2013.02.26
		else
			AsCommonSender.SendSocialUiScroll( AsUserInfo.Instance.LoginUserUniqueKey, eSOCIAL_UI_TYPE.eSOCIAL_UI_FRIEND, m_CurPage, m_OnlineFilterBtn.Value);	//2013.02.26

		m_OldPage = m_CurPage;
	}
}
