using UnityEngine;
using System.Collections;
using System.Text;

public class AsCharacterCreateFramework : MonoBehaviour
{
	public enum PANEL_TYPE
	{
		PANEL_INVALID = -1,

		PANEL_RACE,
//		PANEL_CLASS,
		PANEL_CUSTOMIZING,

		MAX_PANEL
	};

	[SerializeField]UIButton prevBtn = null;
	[SerializeField]UIButton nextBtn = null;

	public AsCharacterCreatePanel[] panels = new AsCharacterCreatePanel[0];
	private int curPanel = (int)PANEL_TYPE.PANEL_RACE;
	public UITextField nameField = null;
	public AsCharacterRepresentative rep = null;
	private AsMessageBox msgBox = null;

	// character information - begin
	public static int curRace = (int)eRACE.DEMIGOD;
	public static int curJob = (int)eCLASS.DIVINEKNIGHT;
	public static int curGender = (int)eGENDER.eGENDER_MALE;
	public static int curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_1;
	public static int curHairColor = (int)CHARACTER_COLOR.COLOR_1;
	public static int curBodyColor = (int)CHARACTER_COLOR.COLOR_1;
	public static int curPointColor = (int)CHARACTER_COLOR.COLOR_1;
	public static int curGloveColor = (int)CHARACTER_COLOR.COLOR_1;
	public static string curName = "";
	// character information - end

#if false
	private string m_strEngChar = "abcdefghijklmnopqrstuvwxyz";
#endif
	private AsMessageBox m_CharNameErrDlg = null;


	private GameObject m_RecommendDlg = null;

	// Use this for initialization
	void Start()
	{
		prevBtn.Text = AsTableManager.Instance.GetTbl_String(2095);
		nextBtn.Text = AsTableManager.Instance.GetTbl_String(2096);

		foreach( AsCharacterCreatePanel panel in panels)
			panel.prtFramework = this;

		AsInputManager.Instance.m_Activate = false;
		AsGameMain.s_gameState = GAME_STATE.STATE_CHARACTER_CREATE;
		ChangePanel( PANEL_TYPE.PANEL_RACE);
		AsEventUIMgr.Instance.RecommendEventEnd = false;
		AsLoadingIndigator.Instance.HideIndigator();
	}

	void OnDestroy()
	{
		AsInputManager.Instance.m_Activate = true;
	}

	private void CloseAllAlertDlg()
	{
		AsNotify.Instance.CloseAllMessageBox();
		msgBox = null;
		m_CharNameErrDlg = null;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnBack()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( AsEventUIMgr.Instance.RecommendEvent)
			return; //#18182.
		
		if( true == PartsRoot.s_isLoading )
			return;
		
		switch( (PANEL_TYPE)curPanel)
		{
		case PANEL_TYPE.PANEL_RACE:
			Application.LoadLevel( "CharacterSelect");
			DDOL_Tracer.BeginTrace();//$ yde
			Resources.UnloadUnusedAssets();
			break;
		case PANEL_TYPE.PANEL_CUSTOMIZING:
			prevBtn.Text = AsTableManager.Instance.GetTbl_String(2095);
			nextBtn.Text = AsTableManager.Instance.GetTbl_String(2096);
			ChangePanel( PANEL_TYPE.PANEL_RACE);
			break;
		}
	}

	public void OnNext()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( AsEventUIMgr.Instance.RecommendEvent)
			return; //#18182.

		if( true == PartsRoot.s_isLoading )
			return;
		
		if( AsEventUIMgr.Instance.RecommendEventEnd)
		{
			CloseAllAlertDlg();
			
			if( true == AssetbundleManager.Instance.useAssetbundle)
			{
				if( true == AssetbundleManager.Instance.isLoadedScene( "CharacterSelect"))
				{
					Application.LoadLevel( "CharacterSelect");
					DDOL_Tracer.BeginTrace();//$ yde
					Resources.UnloadUnusedAssets();
				}
			}
			else
			{
				Application.LoadLevel( "CharacterSelect");
				DDOL_Tracer.BeginTrace();//$ yde
				Resources.UnloadUnusedAssets();
			}
		}
		else
		{
			switch( (PANEL_TYPE)curPanel)
			{
			case PANEL_TYPE.PANEL_RACE:
				prevBtn.Text = AsTableManager.Instance.GetTbl_String(2097);
				nextBtn.Text = AsTableManager.Instance.GetTbl_String(2098);
				ChangePanel( PANEL_TYPE.PANEL_CUSTOMIZING);
				break;
			case PANEL_TYPE.PANEL_CUSTOMIZING:
				SendCharacterCreate();
				break;
			}
		}
	}

	private void ChangePanel( PANEL_TYPE type)
	{
		//$yde
		if((PANEL_TYPE)curPanel == PANEL_TYPE.PANEL_CUSTOMIZING)
		{
			if(AsEntityManager.Instance.ModelLoading == true)
				return;
		}

		if( (int)type == curPanel)
			return;

		CloseAllAlertDlg();

		rep.Enable = ( PANEL_TYPE.PANEL_CUSTOMIZING == type) ? false : true;

		panels[ curPanel].Hide( true);
		panels[ (int)type].Hide( false);
		curPanel = (int)type;
	}

	void OnEnable()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	void OnDisable()
	{
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}

	private void SendCharacterCreate()
	{
		//$yde
		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		CloseAllAlertDlg();

		string name = nameField.Text;

		if( 0 == name.Length)
		{
			if( null == msgBox)
				msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1406), AsTableManager.Instance.GetTbl_String(1407), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
			return;
		}

		byte[] bytesName = Encoding.UTF8.GetBytes( name);

		if( false == AsUtil.CheckCharacterName( name))
		{
			string title = AsTableManager.Instance.GetTbl_String(128);
			string msg = AsTableManager.Instance.GetTbl_String(129);
			if( null == m_CharNameErrDlg)
				m_CharNameErrDlg = AsNotify.Instance.MessageBox( title, msg, null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		body_CG_CHAR_CREATE charCreate = new body_CG_CHAR_CREATE();
		charCreate.nSlot = AsGameMain.createCharacterSlot;
		System.Buffer.BlockCopy( bytesName, 0, charCreate.szCharName, 0, bytesName.Length);
		charCreate.eRace = (eRACE)curRace;
		charCreate.eClass = (eCLASS)curJob;
		charCreate.eGender = (eGENDER)curGender;
		charCreate.nHairIndex = curHairStyle;
		charCreate.nHairColor = curHairColor;
		charCreate.nBodyIndex = curBodyColor;
		charCreate.nPointIndex = curPointColor;
		charCreate.nGloveIndex = curGloveColor;

		byte[] data = charCreate.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void OpenRecommendDlg()
	{
		if( null == m_RecommendDlg)
			m_RecommendDlg = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Recommend")) as GameObject;

		AsRecommendDlg dlg = m_RecommendDlg.GetComponentInChildren<AsRecommendDlg>();
		dlg.ParentObject = gameObject;
	}

	private void CloseRecommendDlg()
	{
		if( null == m_RecommendDlg)
			return;

		GameObject.DestroyImmediate( m_RecommendDlg);
		m_RecommendDlg = null;
	}
	
	public bool IsOpenRecommendDlg()
	{
		if( null == m_RecommendDlg)
			return false;
		return true;
	}
	
	public void CancelRecommendDlg()
	{
		AsRecommendDlg dlg = m_RecommendDlg.GetComponentInChildren<AsRecommendDlg>();
		dlg.Close();
	}
}
