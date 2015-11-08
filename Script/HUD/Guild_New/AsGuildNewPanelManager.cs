using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eGuildNewPanelState
{
	Invalid = -1,
	Main,
	GuildList,
	Info,
	BuffSetup,
	MemberList,
	AcceptList,
	Ranking,
}

public class AsGuildNewPanelManager : MonoBehaviour 
{
	public AsGuildNewPanel_Main 			m_panelMain = null;
	public AsGuildNewPanel_GuildList 	m_panelGuildList = null;
	public AsGuildNewPanel_Info 			m_panelInfo = null;
	public AsGuildNewPanel_BuffSetup 	m_panelBuffSetup = null;
	public AsGuildNewPanel_MemberList m_panelMemberList = null;
	public AsGuildNewPanel_AcceptList 	m_panelAcceptList = null;

	private eGuildNewPanelState prevState = eGuildNewPanelState.Invalid;

	private List<AsGuildNewPanel_Base>	m_listPanel = new List<AsGuildNewPanel_Base>();

	AsGuildNewPanel_Base						m_curPanel = null;

	void Awake()
	{
		m_listPanel.Add (m_panelMain);
		m_listPanel.Add (m_panelGuildList);
		m_listPanel.Add (m_panelInfo);
		m_listPanel.Add (m_panelBuffSetup);
		m_listPanel.Add (m_panelMemberList);
		m_listPanel.Add (m_panelAcceptList);

		m_panelMain.PanelState = eGuildNewPanelState.Main;
		m_panelGuildList.PanelState = eGuildNewPanelState.GuildList;
		m_panelInfo.PanelState = eGuildNewPanelState.Info;
		m_panelBuffSetup.PanelState = eGuildNewPanelState.BuffSetup;
		m_panelMemberList.PanelState = eGuildNewPanelState.MemberList;
		m_panelAcceptList.PanelState = eGuildNewPanelState.AcceptList;
	}

	// Use this for initialization
	void Start () 
	{
		int iii = 0;
	}

	public void Init( eGuildNewPanelState state, System.Object data)
	{
		if (prevState == state) 
		{
			if( m_curPanel != null )
				m_curPanel.UpdateData( data );
			return;
		}
		
		prevState = state;

		for (int i=0; i<m_listPanel.Count; i++) 
		{
			if( m_listPanel[i].PanelState == state )
			{
				m_listPanel[i].gameObject.SetActive(true);
				m_listPanel[i].Init(data);
				m_curPanel = m_listPanel[i];
			}
			else
			{
				m_listPanel[i].gameObject.SetActive(false);	
			}
		}
	}

	public void RequestCurrentPage(eGuildNewPanelState	state)
	{
		for (int i=0; i<m_listPanel.Count; i++) 
		{
			if( m_listPanel[i].PanelState == state )
			{
				m_listPanel[i].RequestCurrentPage();
				break;
			}
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
