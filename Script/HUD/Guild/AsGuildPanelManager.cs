using UnityEngine;
using System.Collections;

public class AsGuildPanelManager : MonoBehaviour
{
	public enum eGuildPanelState
	{
		Invalid = -1,
		Information,
		MemberInfo,
		Managing,
		List,
	}
	
	public AsGuildPanel_Info infoPanel = null;
	public AsGuildPanel_Member memberPanel = null;
	public AsGuildPanel_Manage managePanel = null;
	public AsGuildPanel_List	listPanel = null;
	
//	private eGuildPanelState state = eGuildPanelState.Invalid;
	private eGuildPanelState prevState = eGuildPanelState.Invalid;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( eGuildPanelState state, System.Object data)
	{
		if( prevState == state)
			return;
		
		if( null != data)
			prevState = state;
		
		switch( state)
		{
		case eGuildPanelState.Information:
			infoPanel.gameObject.SetActiveRecursively( true);
			infoPanel.Init( data);
			memberPanel.gameObject.SetActiveRecursively( false);
			managePanel.gameObject.SetActiveRecursively( false);
			listPanel.gameObject.SetActiveRecursively( false);
			break;
		case eGuildPanelState.MemberInfo:
			infoPanel.gameObject.SetActiveRecursively( false);
			memberPanel.gameObject.SetActiveRecursively( true);
			memberPanel.Init( data);
			managePanel.gameObject.SetActiveRecursively( false);
			listPanel.gameObject.SetActiveRecursively( false);
			break;
		case eGuildPanelState.Managing:
			infoPanel.gameObject.SetActiveRecursively( false);
			memberPanel.gameObject.SetActiveRecursively( false);
			managePanel.gameObject.SetActiveRecursively( true);
			managePanel.Init( data);
			listPanel.gameObject.SetActiveRecursively( false);
			break;
		case eGuildPanelState.List:
			infoPanel.gameObject.SetActiveRecursively( false);
			memberPanel.gameObject.SetActiveRecursively( false);
			managePanel.gameObject.SetActiveRecursively( false);
			listPanel.gameObject.SetActiveRecursively( true);
			listPanel.Init( data );
			break;
		}
	}
	
	public void DisplayInviteErrorMsg( string msg)
	{
		infoPanel.DisplayInviteErrorMsg( msg);
	}
	
	public void CloseInviteDlg()
	{
		infoPanel.CloseInviteDlg();
	}
	
	public void InsertApplicantList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		managePanel.InsertApplicantList( data);
	}
	
	public void InsertMemberList( body1_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		memberPanel.InsertMemberList( data);
	}
	
	public void UpdateGuildLevel( body_SC_GUILD_LEVEL_UP_RESULT data)
	{
		infoPanel.UpdateGuildLevel( data);
	}
	
	public void UpdateGuildList( body1_SC_GUILD_SEARCH_RESULT data )
	{
		listPanel.UpdateList( data );
	}
	
	public void RequestCurPageApplicant( bool isForced)
	{
		managePanel.RequestCurPageApplicant( isForced);
	}
}
