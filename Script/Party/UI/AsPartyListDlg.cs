using UnityEngine;
using System.Collections;

public class AsPartyListDlg : MonoBehaviour
{
	public enum ePARTYLISTSORT
	{
		ePARTYLISTSORT_NOTHING = -1,

		ePARTYLISTSORT_LEVEL,
		ePARTYLISTSORT_MAXUSER,

		ePARTYLISTSORT_MAX
	};

	public UIButton m_PartyRefresh;
	public UIButton m_CloseBtn;
	public UIButton m_SortBtn;
	public UIRadioBtn[] m_SortRadioBtn;
	public GameObject m_SortRadioPanel;
	public UIButton m_SelectAreaBtn;

	ePARTYLISTSORT 	 m_eSortType = ePARTYLISTSORT.ePARTYLISTSORT_LEVEL;
	public ePARTYLISTSORT eSortType
	{
		set	{ m_eSortType = value; }
		get	{ return m_eSortType; }
	}

	public SpriteText m_TextTitle = null;
	public SpriteText noParty = null;
	private float m_fSendTime = 0f;
	private int m_PartyIndex = 0;
	public UIScrollList m_list = null;
	public GameObject m_listItem = null;

	private UIListButton m_ClickBtn = null;
	private float m_ClickTime = 0.0f;
	private float m_DClickTime = 0.4f;
	public int Count
	{
		get { return m_list.Count; }
	}

	void Clear()
	{
	}

	public void PartyCreateDlgClose()
	{
		AsPartyManager.Instance.PartyUI.PartyCreateDlg.Close();
	}

	public void Close()
	{
		Clear();
		AsPartyManager.Instance.ClosePartyInfoDlg();
		AsPartyManager.Instance.SendDetailPartyIdx = 0;
		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void Open()
	{
		Clear();
		gameObject.active = true;
		AsPartyManager.Instance.PartyListUIMapIdx = TerrainMgr.Instance.GetCurMapID();		
		AsPartySender.SendPartyList( AsPartyManager.Instance.PartyListUIMapIdx);

		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( AsPartyManager.Instance.PartyListUIMapIdx);
		if( null == record)
			return;

		m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String(record.getTooltipStrIdx);
	
		m_fSendTime = 0.0f;//#21354
		gameObject.SetActiveRecursively( true);
		SetVisibleSortBtn( ePARTYLISTSORT.ePARTYLISTSORT_LEVEL);
		SetVisible( m_SortRadioPanel, false);
	}

	public bool ClickBtnMsg( int iMapId, int iWarpIdx)
	{
		Tbl_WarpData_Record warpData = AsTableManager.Instance.GetWarpDataRecord( iWarpIdx);
		if( null == warpData)
			return false;

		if( false == warpData.isActive)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(2040));
			return false;
		}

		if( Time.time - m_fSendTime > 10)
		{
			m_fSendTime = Time.time;
			Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( iMapId);
			if( null == record)
				return false;

			m_SelectAreaBtn.Text = AsTableManager.Instance.GetTbl_String(record.getTooltipStrIdx);
			AsPartyManager.Instance.PartyListUIMapIdx = iMapId;
			AsPartySender.SendPartyList( AsPartyManager.Instance.PartyListUIMapIdx);
			return true;
		}
		else
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String(58));
			return false;
		}
		
		return true;
	}

	public void ClearPartyList()
	{
		m_list.ClearList( true);
		if(gameObject.active)// #21586
			noParty.gameObject.SetActiveRecursively( true);
	}

	public void SetPartyList()
	{
		if(!gameObject.active)// #21586
			return;
		
		ClearPartyList();

		foreach( AsSortPartyList data in AsPartyManager.Instance.GetSortPartyList())
		{
			if( data.m_sPARTYLIST.nUserCnt == AsPartyManager.Instance.PartyOption.nMaxUser) //#17259
				continue;

			if( data.m_sPARTYLIST.sOption.bPublic)
			{
				UIListItemContainer itemContainer = m_list.CreateItem( m_listItem) as UIListItemContainer;

				AsPartyListBtn listItem = itemContainer.gameObject.GetComponent<AsPartyListBtn>();
				listItem.SetData( data.m_sPARTYLIST);
			}
		}
		if(gameObject.active)// #21586
			noParty.gameObject.SetActiveRecursively( 0 == Count);
	}

	// Use this for initialization
	void Start()
	{
		noParty.Text = AsTableManager.Instance.GetTbl_String(1465);
		noParty.gameObject.SetActiveRecursively( false);
		m_PartyRefresh.Text = AsTableManager.Instance.GetTbl_String(1964);
		m_PartyRefresh.SetInputDelegate( RefreshBtnDelegate);

		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1117);

		m_SelectAreaBtn.SetInputDelegate( SelectAreaBtnDelegate);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_SelectAreaBtn.spriteText);

		m_SortBtn.SetInputDelegate( SortBtnDelegate);

		m_SortBtn.Text = AsTableManager.Instance.GetTbl_String(1944);

		m_SortRadioBtn[(int)ePARTYLISTSORT.ePARTYLISTSORT_LEVEL ].SetInputDelegate(LevelSortBtnDelegate);
		m_SortRadioBtn[(int)ePARTYLISTSORT.ePARTYLISTSORT_MAXUSER ].SetInputDelegate(MemberSortBtnDelegate);

		m_SortRadioBtn[(int)ePARTYLISTSORT.ePARTYLISTSORT_LEVEL ].Text = AsTableManager.Instance.GetTbl_String(1944);
		m_SortRadioBtn[(int)ePARTYLISTSORT.ePARTYLISTSORT_MAXUSER ].Text = AsTableManager.Instance.GetTbl_String(1945);
	}

	// Update is called once per frame
	void Update()
	{
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	private void SelectAreaBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			//맵 선택 화면 표시....
			AsPartyManager.Instance.PartyUI.OpenSelectAreaDlg();
			AsPartyManager.Instance.PartyUI.USING_DLG = AsPartyUI.eUSING_DLG.PARTYLIST_DLG;
			AsPartyManager.Instance.PartyUI.SelectAreaDlg.SetFocusZoneMap(AsPartyManager.Instance.PartyListUIMapIdx);//#21419
		}
	}

	private void SetVisibleSortBtn( ePARTYLISTSORT eType)
	{
		m_eSortType = eType;
		foreach( UIRadioBtn radioBtn in m_SortRadioBtn)
			radioBtn.SetState( 1);

		m_SortRadioBtn[(int)m_eSortType].SetState(0);
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

	private void SortBtnDelegate( ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound("Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetVisible(m_SortRadioPanel, true);
			
			m_SortBtn.gameObject.SetActive(false);
		}
	}

	private void LevelSortBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_eSortType = ePARTYLISTSORT.ePARTYLISTSORT_LEVEL;
			AsPartyManager.Instance.LevelSortList();
			AsPartyManager.Instance.PartyUI.PartyListDlg.SetPartyList();

			m_SortBtn.Text = AsTableManager.Instance.GetTbl_String(1944);
			SetVisible( m_SortRadioPanel, false);
			
			m_SortBtn.gameObject.SetActive(true);
		}
	}

	private void MemberSortBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_eSortType = ePARTYLISTSORT.ePARTYLISTSORT_MAXUSER;
			AsPartyManager.Instance.MaxUserSortList();
			AsPartyManager.Instance.PartyUI.PartyListDlg.SetPartyList();
			m_SortBtn.Text = AsTableManager.Instance.GetTbl_String(1945);
			SetVisible( m_SortRadioPanel, false);
			
			m_SortBtn.gameObject.SetActive(true);
		}
	}

	private void RefreshBtnDelegate( ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(Time.time - m_fSendTime > 10)
			{
				m_fSendTime = Time.time;
				AsPartySender.SendPartyList( AsPartyManager.Instance.PartyListUIMapIdx);
			}
			else
			{
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(AsTableManager.Instance.GetTbl_String(58));
			}
		}
	}

	public void OnSelect()
	{
		if(!AsPartyManager.Instance.IsPartying)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			UIListButton clickBtn = m_list.LastClickedControl as UIListButton;
			AsPartyListBtn listBtn = clickBtn.transform.parent.gameObject.GetComponent<AsPartyListBtn>();

			if( m_PartyIndex == listBtn.m_partyListData.nPartyIdx && ( Time.time - m_ClickTime) < m_DClickTime)
			{
				PartyJoin( listBtn);
			}
			else
			{
				m_PartyIndex = listBtn.m_partyListData.nPartyIdx;
				m_ClickTime = Time.time;
				PartyInfo( listBtn);
			}
		}
	}

	void PartyInfo( AsPartyListBtn listBtn)
	{
		listBtn.PartyInfo();
	}

	void PartyJoin( AsPartyListBtn listBtn)
	{
		listBtn.PartyJoin();
		m_ClickBtn = null;
		m_ClickTime = 0.0f;
	}
}
