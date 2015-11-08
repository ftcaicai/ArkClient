using UnityEngine;
using System.Collections;

public enum eSkillBookTabState
{
	Invalid = -1,
	
	NotSet,
	Active,
	Finger,
	Passive,
	
	Max
};

public class AsSkillbookTab : MonoBehaviour
{
	public AsSkillTab[] panels = null;
	public UIPanelTab[] tabs = null;
	
	private eSkillBookTabState state = eSkillBookTabState.Invalid;
	private int m_iCurSkillTabIndex = 0;
	
	public AsSkillTab GetCurSkillTab()
	{
		return panels[ m_iCurSkillTabIndex ];
	}
	
	public int getCurSkillTabIndex
	{
		get	{ return m_iCurSkillTabIndex; }
	}
	
	// Use this for initialization
	void Start()
	{
		foreach( UIPanelTab tab in tabs)
			tab.SetState(1);
		
		tabs[0].SetState(0);
	}
	
	void OnDisable()
	{
		state = eSkillBookTabState.Invalid;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void OnNotsetTabClick()
	{
		if( eSkillBookTabState.NotSet == state)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		state = eSkillBookTabState.NotSet;
		NotsetTabAction();
	}
	
	public void OnActiveTabClick()
	{
		if( eSkillBookTabState.Active == state)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		state = eSkillBookTabState.Active;
		ActiveTabAction();
	}
	
	public void OnFingerTabClick()
	{
		if( eSkillBookTabState.Finger == state)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		state = eSkillBookTabState.Finger;
		FingerTabAction();
	}
	
	public void OnPassiveTabClick()
	{
		if( eSkillBookTabState.Passive == state)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		state = eSkillBookTabState.Passive;
		PassiveTabAction();
	}
	
	private void NotsetTabAction()
	{
		foreach( AsSkillTab panel in panels)
			panel.gameObject.SetActiveRecursively( false);
		
		panels[0].gameObject.SetActiveRecursively( true);
		panels[0].Init(-1);
		m_iCurSkillTabIndex = 0;
	}
	
	private void ActiveTabAction()
	{
		foreach( AsSkillTab panel in panels)
			panel.gameObject.SetActiveRecursively( false);
		
		panels[1].gameObject.SetActiveRecursively( true);
		panels[1].Init(-1);
		m_iCurSkillTabIndex = 1;
	}
	
	private void FingerTabAction()
	{
		foreach( AsSkillTab panel in panels)
			panel.gameObject.SetActiveRecursively( false);
		
		panels[2].gameObject.SetActiveRecursively( true);
		panels[2].Init(-1);
		m_iCurSkillTabIndex = 2;
	}
	
	private void PassiveTabAction()
	{
		foreach( AsSkillTab panel in panels)
			panel.gameObject.SetActiveRecursively( false);
		
		panels[3].gameObject.SetActiveRecursively( true);
		panels[3].Init(-1);
		m_iCurSkillTabIndex = 3;
	}
	
	public void Init( eSkillBookSelectType type)
	{
		switch( type)
		{
		case eSkillBookSelectType.Type_NotSet:
			TabStateUpdate(0);
			NotsetTabAction();
			break;
		case eSkillBookSelectType.Type_Active:
			TabStateUpdate(1);
			ActiveTabAction();
			break;
		case eSkillBookSelectType.Type_Finger:
			TabStateUpdate(2);
			FingerTabAction();
			break;
		case eSkillBookSelectType.Type_Passive:
			TabStateUpdate(3);
			PassiveTabAction();
			break;
		}
	}
	
	private void TabStateUpdate( int index)
	{
		foreach( UIPanelTab tab in tabs)
			tab.SetState(1);
		
		tabs[ index].SetState(0);
	}
}
