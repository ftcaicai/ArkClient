using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsInstantDungeonDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private int m_nCurInDunID = 0;
	private List<Tbl_InDun_Record> m_listIndun = new List<Tbl_InDun_Record>();
	private int m_nPlayerCountType = 0;
	private int m_nIndunGrade = 0;
	
	public UIScrollList m_scrollList = null;
	public GameObject m_goListItem = null;
	
	public UIButton m_BtnClose;
	public UIButton m_BtnInstantDungeon;
	public SimpleSprite m_InDunImg;
	public SpriteText m_TextTitle;
	public SpriteText m_TextDungeonName;
	public SpriteText m_TextLevel;
	public SpriteText m_TextLimitCount;
	public SpriteText m_TextDungeonInfo;
	public SpriteText m_TextPlayerCount;
	public SpriteText m_TextIndunGrade;
	public SimpleSprite m_MoreList_L;
	public SimpleSprite m_MoreList_R;
	
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDungeonName, true);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextLimitCount, true);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDungeonInfo);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnInstantDungeon.spriteText);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1681);
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;

		if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_INDUN) != null)
			AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_INDUN);

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_INDUN_DLG));

		_Init();
	}

	public void Close()
	{
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.CLOSE_INDUN_DLG));

		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public void ResetIndunListSelect()
	{
		for( int i = 0; i < m_scrollList.Count; i++)
		{
			UIListItemContainer listItemContainer = m_scrollList.GetItem( i) as UIListItemContainer;
			AsInstantDungeonListItem item = listItemContainer.gameObject.GetComponent<AsInstantDungeonListItem>();
			item.SetActiveSelect( false);
		}
	}
	
	public void Set_InDunData(int nID)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nID);
		
		if( null == record)
		{
			Debug.LogError( "AsInstantDungeonDlg::Set_InDunData(): Not Found Data, ID: " + nID);
			return;
		}
		
		Texture tex = ResourceLoad.Loadtexture( record.Icon);
		m_InDunImg.renderer.material.mainTexture = tex;

		m_TextDungeonName.Text = AsTableManager.Instance.GetTbl_String( record.Name);
		m_TextLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1702), record.MinLv, record.MaxLv);
		m_TextLimitCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1682), AsInstanceDungeonManager.Instance.LimitCount);
		m_TextDungeonInfo.Text = AsTableManager.Instance.GetTbl_String( record.Description);
		
		m_nPlayerCountType = 0;
		m_nIndunGrade = 0;
		_ApplyPlayerCountTypeTextAndIndunGradeText();
		m_nCurInDunID = nID;

		// level check
		int nUserLevel = AsUserInfo.Instance.SavedCharStat.level_;
		if( nUserLevel < record.MinLv || nUserLevel > record.MaxLv)
		{
			m_BtnInstantDungeon.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_BtnInstantDungeon.spriteText.SetColor( Color.gray);
		}
		else
		{
			m_BtnInstantDungeon.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_BtnInstantDungeon.spriteText.SetColor( Color.black);
		}
	}

	public void OnBtnPlayerCountLeft()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		if( m_nPlayerCountType > 0)
		{
			m_nPlayerCountType--;
			_ApplyPlayerCountTypeTextAndIndunGradeText();
			_ModeClearCheckAndSetBtnState();
		}
	}

	public void OnBtnPlayerCountRight()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		if( m_nPlayerCountType < 1)
		{
			m_nPlayerCountType++;
			_ApplyPlayerCountTypeTextAndIndunGradeText();
			_ModeClearCheckAndSetBtnState();
		}
	}

	public void OnBtnIndunGradeLeft()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		if( m_nIndunGrade > 0)
		{
			m_nIndunGrade--;
			_ApplyPlayerCountTypeTextAndIndunGradeText();
			_ModeClearCheckAndSetBtnState();
		}
	}

	public void OnBtnIndunGradeRight()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		if( m_nIndunGrade < 2)
		{
			m_nIndunGrade++;
			_ApplyPlayerCountTypeTextAndIndunGradeText();
			_ModeClearCheckAndSetBtnState();
		}
	}

	// < private
	private void _Init()
	{
		m_BtnClose.SetInputDelegate( _BtnDelegate_Close);
		m_BtnInstantDungeon.SetInputDelegate( _BtnDelegate_InstantDungeon);
		m_scrollList.AddItemSnappedDelegate( _Delegate_PageChange);
		
		m_nCurInDunID = 0;
		
		m_BtnInstantDungeon.Text = AsTableManager.Instance.GetTbl_String( 1685);

		m_listIndun.Clear();
		_CreateDungeonList();
		
		Set_InDunData( m_nCurInDunID);
		
		_Update_MoreListMark();
	}
		
	private void _CreateDungeonList()
	{
		int nCount = AsTableManager.Instance.GetInDunTable().GetCount();
		
		for( int i = 0; i < nCount; i++)
		{
			int nInDunID = i + 1;
			Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nInDunID);
			
			if( null == record)
			{
				Debug.Log( "AsInstantDungeonDlg::_CreateDungeonList(), Tbl_InDun_Record record == null, nInDunID: " + nInDunID);
				continue;
			}
			
			if( true == _isAddScrollList( record))
			{
				_AddDungeonListData( nInDunID);
			}
		}
	}
	
	private void _AddDungeonListData(int nInDunID)
	{
		UIListItemContainer listItemContainer =  m_scrollList.CreateItem( m_goListItem) as UIListItemContainer;
		AsInstantDungeonListItem item = listItemContainer.gameObject.GetComponent<AsInstantDungeonListItem>();
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nInDunID);
		
		item.Init( nInDunID, record);
		
		if( 0 == m_nCurInDunID)
		{
			m_nCurInDunID = nInDunID;
			item.SetActiveSelect( true);
		}
	}
	
	private bool _isAddScrollList(Tbl_InDun_Record record)
	{
		if( 0 != record.IndunType)
			return false;
		
		foreach( Tbl_InDun_Record data in m_listIndun)
		{
			if( data.InsGroup == record.InsGroup)
			{
				m_listIndun.Add( record);
				return false;
			}
		}
		
		m_listIndun.Add( record);
		
		return true;
	}
	
	private void _ApplyPlayerCountTypeTextAndIndunGradeText()
	{
		switch( m_nPlayerCountType)
		{
		case 0: m_TextPlayerCount.Text = AsTableManager.Instance.GetTbl_String( 1870); break;
		case 1: m_TextPlayerCount.Text = AsTableManager.Instance.GetTbl_String( 1871); break;
		}
		
		switch( m_nIndunGrade)
		{
		case 0: m_TextIndunGrade.Text = AsTableManager.Instance.GetTbl_String( 1873); break;
		case 1: m_TextIndunGrade.Text = AsTableManager.Instance.GetTbl_String( 1874); break;
		case 2: m_TextIndunGrade.Text = AsTableManager.Instance.GetTbl_String( 1897); break;
		}
	}
		
	private void _BtnDelegate_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseInstantDungeonDlg();
		}
	}

	private void _BtnDelegate_InstantDungeon(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
//			int nCurIndunTableIndex = _GetIndunTableIndex();
//			AsInstanceDungeonManager.Instance.Send_InDun_Create( nCurIndunTableIndex);
//			Debug.Log( "AsInstantDungeonDlg::_BtnDelegate_InstantDungeon(), nCurIndunTableIndex: " + nCurIndunTableIndex);
			
			int nIndunBranchTableIndex = _GetIndunBranchTableIndex();
			AsInstanceDungeonManager.Instance.Send_InDun_Create( m_nCurInDunID, nIndunBranchTableIndex);
			Debug.Log( "AsInstantDungeonDlg::_BtnDelegate_InstantDungeon(), nCurInDunID: " + m_nCurInDunID + ", nIndunBranchTableIndex: " + nIndunBranchTableIndex);
		}
	}
	
	/*
	private int _GetIndunTableIndex()
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nCurInDunID);
		
		if( null == record)
		{
			Debug.LogError( "AsInstantDungeonDlg::_GetIndunTableIndex(): Not Found Data, ID: " + m_nCurInDunID);
			return -1;
		}

		foreach( Tbl_InDun_Record data in m_listIndun)
		{
			if( data.InsGroup == record.InsGroup)
			{
				int nPlayerCount = 0;
				string strGrade = "";
				
				if( 0 == m_nPlayerCountType)
					nPlayerCount = 1;
				else if( 1 == m_nPlayerCountType)
					nPlayerCount = 4;
				
				if( 0 == m_nIndunGrade)
					strGrade = "normal";
				else if( 1 == m_nIndunGrade)
					strGrade = "hard";
				
				if( nPlayerCount == data.MaxPlayerCount && strGrade.Contains( data.Grade.ToLower()))
					return data.ID;
			}
		}
		
		return -1;
	}
	*/

	private int _GetIndunBranchTableIndex()
	{
		int nPlayerCount = 0;
		string strGrade = "";
		
		if( 0 == m_nPlayerCountType)
			nPlayerCount = 1;
		else if( 1 == m_nPlayerCountType)
			nPlayerCount = 4;
		
		if( 0 == m_nIndunGrade)
			strGrade = "normal";
		else if( 1 == m_nIndunGrade)
			strGrade = "hard";
		else if( 2 == m_nIndunGrade)
			strGrade = "hell";

		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();

		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( m_nCurInDunID == pair.Value.InstanceTableIdx)
			{
				if( nPlayerCount == pair.Value.MaxPlayerCount && strGrade.Contains( pair.Value.Grade.ToLower()))
					return pair.Value.ID;
			}
		}
		
		return -1;
	}

	private void _Delegate_PageChange( IUIObject obj)
	{
		IUIListObject data = (IUIListObject)obj;
		AsInstantDungeonListItem item = data.gameObject.GetComponent<AsInstantDungeonListItem>();

		_Update_MoreListMark();
	}
	
	private void _Update_MoreListMark()
	{
		UIListItemContainer listItemContainer_l = m_scrollList.GetItem( 0) as UIListItemContainer;
		AsInstantDungeonListItem item_l = listItemContainer_l.gameObject.GetComponent<AsInstantDungeonListItem>();
		m_MoreList_L.gameObject.SetActiveRecursively( !( item_l.gameObject.activeInHierarchy));
		
		UIListItemContainer listItemContainer_r = m_scrollList.GetItem( m_scrollList.Count - 1) as UIListItemContainer;
		AsInstantDungeonListItem item_r = listItemContainer_r.gameObject.GetComponent<AsInstantDungeonListItem>();
		m_MoreList_R.gameObject.SetActiveRecursively( !( item_r.gameObject.activeInHierarchy));
	}

	private void _ModeClearCheckAndSetBtnState()
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nCurInDunID);
		
		if( null == record)
		{
			Debug.LogError( "AsInstantDungeonDlg::_ModeClearCheckAndSetBtnState(): Not Found Data, ID: " + m_nCurInDunID);
			return;
		}
		
		int nUserLevel = AsUserInfo.Instance.SavedCharStat.level_;

		if( record.MinLv <= nUserLevel && nUserLevel <= record.MaxLv)
		{
			int nIndunBranchTableIndex = _GetIndunBranchTableIndex();
			
			if( true == AsInstanceDungeonManager.Instance.isClearPrevMode( nIndunBranchTableIndex))
			{
				m_BtnInstantDungeon.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				m_BtnInstantDungeon.spriteText.SetColor( Color.black);
			}
			else
			{
				m_BtnInstantDungeon.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				m_BtnInstantDungeon.spriteText.SetColor( Color.gray);
			}
		}
	}
	// private >
}
