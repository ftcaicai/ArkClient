using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_Main : AsGuildNewPanel_Base 
{
	public SpriteText 		m_MainTitle = null;

	public UIButton		m_btnMakeGuild = null;
	public UIButton		m_btnShowGuildList = null;

	private GameObject m_createDlg = null;

	// Use this for initialization
	void Awake()
	{

	}

	void Start () 
	{
		m_MainTitle.Text = AsTableManager.Instance.GetTbl_String(18002);

		m_btnMakeGuild.Text = AsTableManager.Instance.GetTbl_String(1241);
		m_btnShowGuildList.Text = AsTableManager.Instance.GetTbl_String(1004);
	}

	override public void Init(System.Object data)
	{
	}

	void OnDisable()
	{
		CloseCreateDlg();
	}	

	public void CloseCreateDlg()
	{
		GameObject.DestroyImmediate( m_createDlg);
		m_createDlg = null;
	}

	void OnBtnCreate()
	{
		if( null != AsUserInfo.Instance.GuildData )
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != m_createDlg)
			return;
		
		m_createDlg = Instantiate( Resources.Load( "UI/AsGUI/Guild_new/Popup_GuildCreate")) as GameObject;
		AsGuildNewCreateDlg dlg = m_createDlg.GetComponentInChildren<AsGuildNewCreateDlg>();
		dlg.Parent = gameObject;
	}

	void OnBtnList()
	{
		body_CS_GUILD_UI_SCROLL scroll = new body_CS_GUILD_UI_SCROLL( eGUILD_UI_SCROLL.eGUILD_UI_SCROLL_GUILD, 0 , false);
		byte[] packet = scroll.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
