using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsInstantDungeonDlg_new : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private int m_nCurInDunID = 0;
	private List<Tbl_InDun_Record> m_listIndun = new List<Tbl_InDun_Record>();
	private int m_nPlayerCountType = 0;
	private int m_nIndunGrade = 0;
	private AsIndunItemPreviewDlg m_IndunItemPreviewDlg = null;

	public UIScrollList m_scrollList = null;
	public GameObject m_goListItem = null;
	
	public UIButton m_BtnClose;
	public UIButton m_BtnIndunEnter_1;
	public UIButton m_BtnIndunEnter_4;
	public UIButton m_BtnReward;
	public SimpleSprite m_InDunImg;
	public SpriteText m_TextTitle;
	public SpriteText m_TextDungeonName;
	public SpriteText m_TextLimitCount;
	public SpriteText m_TextDungeonInfo;
	public SimpleSprite m_MoreList_L;
	public SimpleSprite m_MoreList_R;
	public UIRadioBtn m_btnNormal = null;
	public UIRadioBtn m_btnHard = null;
	public UIRadioBtn m_btnHell = null;
	public SpriteText m_TextNormal;
	public SpriteText m_TextHard;
	public SpriteText m_TextHell;
	public SpriteText m_TextHellCount;
	public SpriteText m_TextHellLevel;
	public ParticleSystem m_HellModeEffect;
	public ParticleSystem m_NormalSelectEff;
	public ParticleSystem m_HardSelectEff;
	public ParticleSystem m_HellSelectEff;

	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDungeonName, true);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextLimitCount, true);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextDungeonInfo);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnIndunEnter_1.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnIndunEnter_4.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnReward.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextNormal);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextHard);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextHell);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextHellCount);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextHellLevel);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1681);
		m_TextNormal.Text = AsTableManager.Instance.GetTbl_String( 1873);
		m_TextHard.Text = AsTableManager.Instance.GetTbl_String( 1874);
		m_TextHell.Text = AsTableManager.Instance.GetTbl_String( 1897);
		m_BtnReward.Text = AsTableManager.Instance.GetTbl_String( 37918);
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot, int nIndunBranchTable_LastEnter)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		Tbl_InsDungeonReward_Record record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTable_LastEnter);
		if( null == record)
			m_nCurInDunID = 0;
		else
			m_nCurInDunID = record.InstanceTableIdx;

		if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_INDUN) != null)
			AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_INDUN);
		
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_INDUN_DLG));
		
		_Init();
	}
	
	public void Close()
	{
		CloseIndunItemPreviewDlg();

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
			Debug.LogError( "AsInstantDungeonDlg_new::Set_InDunData(): Not Found Data, ID: " + nID);
			return;
		}
		
		Texture tex = ResourceLoad.Loadtexture( record.Icon);
		m_InDunImg.renderer.material.mainTexture = tex;
		
		m_TextDungeonInfo.Text = AsTableManager.Instance.GetTbl_String( record.Description);

		int nHellLimitCount_Total = _GetHellLimitCount_Total();
		if( nHellLimitCount_Total > 0)
			m_TextLimitCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 2717), AsInstanceDungeonManager.Instance.LimitCount, nHellLimitCount_Total);
		else
			m_TextLimitCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 2716), AsInstanceDungeonManager.Instance.LimitCount);
		
		m_nPlayerCountType = 0;
		m_nIndunGrade = 0;
		m_nCurInDunID = nID;

		for( int i = 0; i < m_scrollList.Count; i++)
		{
			UIListItemContainer listItemContainer = m_scrollList.GetItem( i) as UIListItemContainer;
			AsInstantDungeonListItem item = listItemContainer.gameObject.GetComponent<AsInstantDungeonListItem>();
			if( item.InDunID == nID)
			{
				item.SetActiveSelect( true);
				m_scrollList.ScrollToItem( i, 1.0f);
				StartCoroutine( _UpdateMoreListMark_Coroutine());
			}
			else
				item.SetActiveSelect( false);
		}

		// level check
		_SetIndunEnterBtnState( false);
		m_btnNormal.SetState( 2); // disable
		m_btnHard.SetState( 2); // disable
		m_btnHell.SetState( 2); // disable
		m_btnNormal.controlIsEnabled = false;
		m_btnHard.controlIsEnabled = false;
		m_btnHell.controlIsEnabled = false;

		bool bGradeFirstActive = true;

		// grade btn setting
		if( true == _isIndunGradeUse( nID, "normal"))
		{
			m_btnNormal.controlIsEnabled = true;
			if( true == bGradeFirstActive)
			{
				m_NormalSelectEff.Play();
				bGradeFirstActive = false;
				m_btnNormal.SetState( 0);
				m_nIndunGrade = 0;
			}
		}
		if( true == _isIndunGradeUse( nID, "hard"))
		{
			m_btnHard.controlIsEnabled = true;
			if( true == bGradeFirstActive)
			{
				if( 1 != m_nIndunGrade)
					m_HardSelectEff.Play();
				bGradeFirstActive = false;
				m_btnHard.SetState( 0);
				m_nIndunGrade = 1;
			}
			else
				m_btnHard.SetState( 1);
		}
		if( true == _isIndunGradeUse( nID, "hell"))
		{
			m_TextHellCount.gameObject.SetActive( true);
			m_TextHellLevel.gameObject.SetActive( true);
			m_TextHellCount.Text = AsInstanceDungeonManager.Instance.GetHellLimitCount( nID).ToString();
			m_TextHellLevel.Text = _GetHellLevel( nID);
			m_HellModeEffect.Play();

			m_btnHell.controlIsEnabled = true;
			if( true == bGradeFirstActive)
			{
				if( 2 != m_nIndunGrade)
					m_HellSelectEff.Play();
				bGradeFirstActive = false;
				m_btnHell.SetState( 0);
				m_nIndunGrade = 2;
			}
			else
				m_btnHell.SetState( 1);

			if( false == AsInstanceDungeonManager.Instance.isClearPrevMode( _GetIndunBranchTableIndex( nID, "hell", 1))
			   && false == AsInstanceDungeonManager.Instance.isClearPrevMode( _GetIndunBranchTableIndex( nID, "hell", 4)))
			{
				m_TextHellCount.gameObject.SetActive( false);
				m_HellModeEffect.Stop();
				m_HellModeEffect.Clear();
			}
		}
		else
		{
			m_TextHellCount.gameObject.SetActive( false);
			m_TextHellLevel.gameObject.SetActive( false);
			m_HellModeEffect.Stop();
			m_HellModeEffect.Clear();
		}

		m_TextDungeonName.Text = _GetIndunNameWithGrade( nID, m_nIndunGrade);

		// player count btn setting
		_ModeClearCheckAndSetBtnState();
	}

	public void OnBtnNormal()
	{
		if( 0 != m_nIndunGrade)
			m_NormalSelectEff.Play();

		m_nIndunGrade = 0;
		m_TextDungeonName.Text = _GetIndunNameWithGrade( m_nCurInDunID, m_nIndunGrade);
		_ModeClearCheckAndSetBtnState();
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}
	
	public void OnBtnHard()
	{
		if( 1 != m_nIndunGrade)
			m_HardSelectEff.Play();

		m_nIndunGrade = 1;
		m_TextDungeonName.Text = _GetIndunNameWithGrade( m_nCurInDunID, m_nIndunGrade);
		_ModeClearCheckAndSetBtnState();
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}

	public void OnBtnHell()
	{
		if( 2 != m_nIndunGrade)
			m_HellSelectEff.Play();

		m_nIndunGrade = 2;
		m_TextDungeonName.Text = _GetIndunNameWithGrade( m_nCurInDunID, m_nIndunGrade);
		_ModeClearCheckAndSetBtnState();
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}

	public void OnBtnReward()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6107_EFF_Jewel_TabButton", Vector3.zero, false);
		StartCoroutine( OpenIndunItemPreviewDlg());
	}

	public IEnumerator OpenIndunItemPreviewDlg()
	{
		if( null == m_IndunItemPreviewDlg)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_IndeonPopup_LineupFree");
			yield return obj;
			
			GameObject go = GameObject.Instantiate( obj) as GameObject;
			m_IndunItemPreviewDlg = go.GetComponentInChildren<AsIndunItemPreviewDlg>();
			
			if( null != m_IndunItemPreviewDlg)
			{
				m_IndunItemPreviewDlg.Open( go, m_nCurInDunID , m_nIndunGrade);
			}
		}
	}
	
	public void CloseIndunItemPreviewDlg()
	{
		if( null == m_IndunItemPreviewDlg)
			return;
		
		m_IndunItemPreviewDlg.Close();
	}

	// < private
	private void _Init()
	{
		m_BtnClose.SetInputDelegate( _BtnDelegate_Close);
		m_BtnIndunEnter_1.SetInputDelegate( _BtnDelegate_IndunEnter_1);
		m_BtnIndunEnter_4.SetInputDelegate( _BtnDelegate_IndunEnter_4);
		m_scrollList.AddItemSnappedDelegate( _Delegate_PageChange);
		
//		m_nCurInDunID = 0;
		
		m_BtnIndunEnter_1.Text = AsTableManager.Instance.GetTbl_String( 2355);
		m_BtnIndunEnter_4.Text = AsTableManager.Instance.GetTbl_String( 2356);

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
				Debug.Log( "AsInstantDungeonDlg_new::_CreateDungeonList(), Tbl_InDun_Record record == null, nInDunID: " + nInDunID);
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

	private void _BtnDelegate_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseInstantDungeonDlg();
		}
	}
	
	private void _BtnDelegate_IndunEnter_1(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			m_nPlayerCountType = 0;
			_BtnDelegate_IndunEnter();
		}
	}
	
	private void _BtnDelegate_IndunEnter_4(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			m_nPlayerCountType = 1;
			_BtnDelegate_IndunEnter();
		}
	}

	private void _BtnDelegate_IndunEnter()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		int nIndunBranchTableIndex = _GetIndunBranchTableIndex();
		AsInstanceDungeonManager.Instance.Send_InDun_Create( m_nCurInDunID, nIndunBranchTableIndex);
		Debug.Log( "AsInstantDungeonDlg_new::_BtnDelegate_IndunEnter(), nCurInDunID: " + m_nCurInDunID + ", nIndunBranchTableIndex: " + nIndunBranchTableIndex);
	}

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

	private IEnumerator _UpdateMoreListMark_Coroutine()
	{
		yield return new WaitForSeconds( 1.0f);
		m_scrollList.LateUpdate();
		_Update_MoreListMark();
	}

	private void _ModeClearCheckAndSetBtnState()
	{
		m_nPlayerCountType = 0;
		_ModeClearCheckAndSetBtnState( true);
		m_nPlayerCountType = 1;
		_ModeClearCheckAndSetBtnState( false);
	}
	
	private void _ModeClearCheckAndSetBtnState(bool isOnePlayer)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nCurInDunID);
		
		if( null == record)
		{
			Debug.LogError( "AsInstantDungeonDlg_new::_ModeClearCheckAndSetBtnState(): Not Found Data, ID: " + m_nCurInDunID);
			return;
		}
		
		int nIndunBranchTableIndex = _GetIndunBranchTableIndex();
		Tbl_InsDungeonReward_Record record2 = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);
		if( null == record2)
		{
			Debug.LogError( "AsInstantDungeonDlg_new::_ModeClearCheckAndSetBtnState(): Not Found Data, ID: " + nIndunBranchTableIndex);
			return;
		}

		if( record.MinLv <= AsUserInfo.Instance.SavedCharStat.level_)
		{
			if( 1 == record2.Use && true == AsInstanceDungeonManager.Instance.isClearPrevMode( nIndunBranchTableIndex))
			{
				_SetIndunEnterBtnState( isOnePlayer, true);
			}
			else
			{
				if( 1 == record2.Use)
					_SetIndunEnterBtnState( isOnePlayer, false);
				else
					_SetIndunEnterBtnState( isOnePlayer, false, true);
			}
		}
		else
		{
			if( 1 == record2.Use)
				_SetIndunEnterBtnState( isOnePlayer, false);
			else
				_SetIndunEnterBtnState( isOnePlayer, false, true);
		}
	}

	private void _SetIndunEnterBtnState(bool bActive)
	{
		_SetIndunEnterBtnState( true, bActive);
		_SetIndunEnterBtnState( false, bActive);
	}

	private void _SetIndunEnterBtnState(bool isOnePlayer, bool bActive, bool bDiable = false)
	{
		if( true == isOnePlayer)
		{
			if( true == bActive)
			{
				m_BtnIndunEnter_1.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				m_BtnIndunEnter_1.spriteText.SetColor( Color.black);
			}
			else
			{
				m_BtnIndunEnter_1.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				m_BtnIndunEnter_1.spriteText.SetColor( Color.gray);
			}

			if( true == bDiable)
				m_BtnIndunEnter_1.SetState( 3);
			else
				m_BtnIndunEnter_1.SetState( 0);
		}
		else
		{
			if( true == bActive)
			{
				m_BtnIndunEnter_4.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				m_BtnIndunEnter_4.spriteText.SetColor( Color.black);
			}
			else
			{
				m_BtnIndunEnter_4.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				m_BtnIndunEnter_4.spriteText.SetColor( Color.gray);
			}

			if( true == bDiable)
				m_BtnIndunEnter_4.SetState( 3);
			else
				m_BtnIndunEnter_4.SetState( 0);
		}
	}

	private bool _isIndunGradeUse(int nIndunID, string strGradeLower)
	{
		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( nIndunID == pair.Value.InstanceTableIdx && pair.Value.Grade.ToLower().Contains( strGradeLower) && 1 == pair.Value.MaxPlayerCount)
			{
				if( 0 == pair.Value.Use)
				{
					foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair2 in dic)
					{
						if( nIndunID == pair2.Value.InstanceTableIdx && pair2.Value.Grade.ToLower().Contains( strGradeLower) && 4 == pair2.Value.MaxPlayerCount)
						{
							if( 0 == pair2.Value.Use)
							{
								return false;
							}
							else
							{
								return true;
							}
						}
					}
				}
				else
				{
					return true;
				}
			}
		}

		return true;
	}

	private bool _isIndunPlayerCountUse(int nIndunID, string strGradeLower, int nPlayerCount)
	{
		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( nIndunID == pair.Value.InstanceTableIdx && pair.Value.Grade.ToLower().Contains( strGradeLower) && nPlayerCount == pair.Value.MaxPlayerCount)
			{
				if( 0 == pair.Value.Use)
					return false;
				else
					return true;
			}
		}

		return true;
	}

	private string _GetHellLevel(int nIndunID)
	{
		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( nIndunID == pair.Value.InstanceTableIdx && pair.Value.Grade.ToLower().Contains( "hell") && 1 == pair.Value.MaxPlayerCount)
				return "Lv" + pair.Value.Hell_Level.ToString();
		}
		
		return "";
	}

	private int _GetHellLimitCount_Total()
	{
		int nCount = 0;
		Dictionary<int, int> dic = AsInstanceDungeonManager.Instance.GetHellLimitCountList();

		foreach( KeyValuePair<int, int> pair in dic)
		{
			int nIndunID = pair.Key;

			if( true == _isIndunGradeUse( nIndunID, "hell"))
			{
				int nIndunBranchTableIndex = _GetIndunBranchTableIndex( nIndunID, "hell", 1);
				Tbl_InsDungeonReward_Record record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);
				if( null != record)
				{
					if( record.Hell_Level <= AsUserInfo.Instance.SavedCharStat.level_)
					{
						if( 1 == record.Use && true == AsInstanceDungeonManager.Instance.isClearPrevMode( nIndunBranchTableIndex))
						{
							nCount += AsInstanceDungeonManager.Instance.GetHellLimitCount( nIndunID);
						}
						else
						{
							nIndunBranchTableIndex = _GetIndunBranchTableIndex( nIndunID, "hell", 4);
							record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);
							if( null != record)
							{
								if( record.Hell_Level <= AsUserInfo.Instance.SavedCharStat.level_)
								{
									if( 1 == record.Use && true == AsInstanceDungeonManager.Instance.isClearPrevMode( nIndunBranchTableIndex))
									{
										nCount += AsInstanceDungeonManager.Instance.GetHellLimitCount( nIndunID);
									}
								}
							}
						}
					}
				}
			}
		}

		return nCount;
	}

	private int _GetIndunBranchTableIndex(int nIndunID, string strGrade, int nPlayerCount)
	{
		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( nIndunID == pair.Value.InstanceTableIdx)
			{
				if( nPlayerCount == pair.Value.MaxPlayerCount && strGrade.Contains( pair.Value.Grade.ToLower()))
					return pair.Value.ID;
			}
		}

		return 0;
	}

	private string _GetIndunNameWithGrade(int nIndunID, int nGrade) // nGrade: 0 normal, 1 hard, 2 hell
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_nCurInDunID);
		
		if( null != record)
		{
			int nGradeString = 0;
			switch( nGrade)
			{
			case 0: nGradeString = 1873; break;
			case 1: nGradeString = 1874; break;
			case 2: nGradeString = 1897; break;
			}

			StringBuilder sb = new StringBuilder( AsTableManager.Instance.GetTbl_String( record.Name));
			sb.Append( "(");
			sb.Append( AsTableManager.Instance.GetTbl_String( nGradeString));
			sb.Append( ")");

			return sb.ToString();
		}

		return "";
	}
	// private >
}
