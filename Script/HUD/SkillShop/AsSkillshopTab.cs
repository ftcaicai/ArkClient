using UnityEngine;
using System.Collections;

public enum eShopTabState
{
	Invalid = -1,

	ChoiceTab,
	BaseTab,
	
	Max
};

public class AsSkillshopTab : MonoBehaviour
{
	public AsSkillTab[] panels = null;
	public UIPanelTab choiceTab = null;
	public UIPanelTab baseTab = null;
	public UIRadioBtn filterBtn = null;
	
	private bool filtering = false;
	private eShopTabState state = eShopTabState.Invalid;
	private int selectNpcID = -1;
	
	// Use this for initialization
	void Start()
	{
		state = eShopTabState.ChoiceTab;
		choiceTab.Value = true;
		baseTab.Value = false;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void OnDisable()
	{
		state = eShopTabState.Invalid;
	}
	
	public void Restructure()
	{
		switch( state)
		{
		case eShopTabState.ChoiceTab:
			ChoiceTabAction();
			break;
		case eShopTabState.BaseTab:
			BaseTabAction();
			break;
		}
	}
	
	public void OnChoiceTabClicked()
	{
		if( eShopTabState.ChoiceTab == state)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		state = eShopTabState.ChoiceTab;
		ChoiceTabAction();
	}
	
	public void OnBaseTabClicked()
	{
		if( eShopTabState.BaseTab == state)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		state = eShopTabState.BaseTab;
		BaseTabAction();
	}
	
	private void ChoiceTabAction()
	{
		panels[ (int)eShopTabState.ChoiceTab].gameObject.SetActiveRecursively( true);
		panels[ (int)eShopTabState.BaseTab].gameObject.SetActiveRecursively( false);
		panels[ (int)eShopTabState.ChoiceTab].Init( selectNpcID, filtering);
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SKILL_SHOP_CHOISE));
	}
	
	private void BaseTabAction()
	{
		panels[ (int)eShopTabState.ChoiceTab].gameObject.SetActiveRecursively( false);
		panels[ (int)eShopTabState.BaseTab].gameObject.SetActiveRecursively( true);
		panels[ (int)eShopTabState.BaseTab].Init( selectNpcID, filtering);
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SKILL_SHOP_BASE));
	}
	
	public void Filtering()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		filtering = !filtering;
		filterBtn.Value = filtering;
		
		switch( state)
		{
		case eShopTabState.ChoiceTab:
			ChoiceTabAction();
			break;
		case eShopTabState.BaseTab:
			BaseTabAction();
			break;
		}
	}
	
	public void Init( int npcID, eSkillSelectType type)
	{
		selectNpcID = npcID;
		
		switch( type)
		{
		case eSkillSelectType.Type_Choice:
			choiceTab.Value = true;
			baseTab.Value = false;
			ChoiceTabAction();
			state = eShopTabState.ChoiceTab;
			break;
		case eSkillSelectType.Type_Base:
			choiceTab.Value = false;
			baseTab.Value = true;
			BaseTabAction();
			state = eShopTabState.BaseTab;
			break;
		}
	}
}
