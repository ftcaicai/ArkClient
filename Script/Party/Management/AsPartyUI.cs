using UnityEngine;
using System.Collections;

public class AsPartyUI : MonoBehaviour
{
	public enum eUSING_DLG
	{
		PARTYCREATE_DLG,
		PARTYMATCHING_DLG,
		PARTYLIST_DLG,
		MAX,
	}

	private AsPartyMemberMgr m_PartyMemberUI = null;

	public GameObject m_GUIPartyCreatObject = null;
	public GameObject m_GUIPartyEditObject = null;
	public GameObject m_GUIPartyListObject = null;
	public GameObject m_GUIPartyMemberObject = null;
	public GameObject m_GUIPartyPRObject = null;
	public GameObject m_GUIPartyMenuObject = null;
	public GameObject m_GUIPartyMatchingObject = null;
	public GameObject m_GUISelectAreaObject = null;

	public UIButton btnLeft;
	public UIButton btnRight;
	public UIButton btnUp;
	public UIButton btnDown;

	private AsPartyCreateDlg m_PartyCreateDlg = null;
	private AsPartyListDlg m_PartyListDlg = null;
	private AsPartyEditDlg m_PartyEditDlg = null;
	private AsPartyPRDlg m_PartyPRDlg = null;
	private AsPartyMenuDlg m_PartyMenuDlg = null;
	private AsPartyMatchingDlg m_PartyMatchingDlg = null;
	private AsSelectAreaDlg m_SelectAreaDlg = null;

	eUSING_DLG m_eUsingDlg = eUSING_DLG.PARTYMATCHING_DLG;

	public bool IsOpenPartyDlg
	{
		get
		{
			if (m_PartyMenuDlg == null)
				return false;

			if (m_PartyMenuDlg.gameObject.active == false)
				return false;

			return true;
		}
	}

	public bool IsOpenPartyMatching
	{
		get
		{
			if (m_PartyMatchingDlg == null)
				return false;

			if (m_PartyMatchingDlg.gameObject.active == false)
				return false;

			return true;
		}
	}

	public bool IsOpenPartyCreate
	{
		get
		{
			if( null == m_PartyCreateDlg)
				return false;
			
			if( false == m_PartyCreateDlg.gameObject.active)
				return false;
			
			return true;
		}
	}
	
	public bool IsOpenPartyPRDlg
	{
		get
		{
			if( null == m_PartyPRDlg)
				return false;
			
			return m_PartyPRDlg.gameObject.active;
		}
	}

	public eUSING_DLG USING_DLG
	{
		get	{ return m_eUsingDlg; }
		set	{ m_eUsingDlg = value; }
	}

	public AsPartyCreateDlg PartyCreateDlg
	{
		get
		{
			if(null == m_PartyCreateDlg)
				LoadPartyCreatDlg();
			return m_PartyCreateDlg;
		}
	}

	public AsPartyEditDlg PartyEditDlg
	{
		get
		{
			if(null == m_PartyEditDlg)
				LoadPartyEditDlg();
			return m_PartyEditDlg;
		}
	}

	public AsPartyListDlg PartyListDlg
	{
		get
		{
			if(null == m_PartyListDlg)
				LoadPartyListDlg();
			return m_PartyListDlg;
		}
	}

	public AsPartyMemberMgr PartyMemberUI
	{
		get
		{
			if(null == m_PartyMemberUI)
				LoadPartyMemberDlg();
			return m_PartyMemberUI;
		}
	}

	public AsPartyPRDlg PartyPRDlg
	{
		get
		{
			if(null == m_PartyPRDlg)
				LoadPartyPRDlg();
			return m_PartyPRDlg;
		}
	}

	public AsSelectAreaDlg SelectAreaDlg
	{
		get
		{
			if(null == m_SelectAreaDlg)
				LoadSelectAreaDlg();
			return m_SelectAreaDlg;
		}
	}

	public AsPartyMenuDlg PartyMenuDlg
	{
		get
		{
			if(null == m_PartyMenuDlg)
				LoadPartyMenuDlg();
			return m_PartyMenuDlg;
		}
	}

	public AsPartyMatchingDlg PartyMatchingDlg
	{
		get
		{
			if(null == m_PartyMatchingDlg)
				LoadPartyMatchingDlg();
			return m_PartyMatchingDlg;
		}
	}

	#region - PartyUI
	public void LoadPartyDlg()
	{
		LoadPartyMemberDlg();
		LoadPartyCreatDlg();
		LoadPartyEditDlg();
		LoadPartyListDlg();
	}

	public void LoadPartyMemberDlg()
	{
		if(null == m_GUIPartyMemberObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject("UI/AsGUI/PartyMatching/GUI_PartyMember");

			m_GUIPartyMemberObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyMemberUI = m_GUIPartyMemberObject.GetComponentInChildren<AsPartyMemberMgr>();
			m_PartyMemberUI.ClosePartyMember();

			m_GUIPartyMemberObject.transform.parent = gameObject.transform;
		}
	}

	public void LoadPartyPRDlg()
	{
		if(null == m_GUIPartyPRObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_Party_PRpopup");

			m_GUIPartyPRObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyPRDlg = m_GUIPartyPRObject.GetComponentInChildren<AsPartyPRDlg>();
			m_GUIPartyPRObject.transform.parent = gameObject.transform;
		}
	}

	public void LoadPartyMenuDlg()
	{
		if(null == m_GUIPartyMenuObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_Party");

			m_GUIPartyMenuObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyMenuDlg = m_GUIPartyMenuObject.GetComponentInChildren<AsPartyMenuDlg>();
			m_GUIPartyMenuObject.transform.parent = gameObject.transform;
		}
	}

	public void LoadPartyCreatDlg()
	{
		if(null == m_GUIPartyCreatObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyCreat");

			m_GUIPartyCreatObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyCreateDlg = m_GUIPartyCreatObject.GetComponentInChildren<AsPartyCreateDlg>();
			m_GUIPartyCreatObject.transform.parent = gameObject.transform;

		}
	}

	public void LoadPartyEditDlg()
	{
		if(null == m_GUIPartyEditObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyEdit");

			m_GUIPartyEditObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyEditDlg = m_GUIPartyEditObject.GetComponentInChildren<AsPartyEditDlg>();
			m_GUIPartyEditObject.transform.parent = gameObject.transform;

		}
	}

	public void LoadPartyListDlg()
	{
		if(null == m_GUIPartyListObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyList");

			m_GUIPartyListObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyListDlg = m_GUIPartyListObject.GetComponentInChildren<AsPartyListDlg>();
			m_GUIPartyListObject.transform.parent = gameObject.transform;
		}
	}

	public void LoadPartyMatchingDlg()
	{
		if(null == m_GUIPartyMatchingObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartySearch");

			m_GUIPartyMatchingObject = GameObject.Instantiate( obj) as GameObject;
			m_PartyMatchingDlg = m_GUIPartyMatchingObject.GetComponentInChildren<AsPartyMatchingDlg>();
			m_GUIPartyMatchingObject.transform.parent = gameObject.transform;
		}
	}

	public void LoadSelectAreaDlg()
	{
		if(null == m_GUISelectAreaObject)
		{
			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PartyMatching/GUI_PartyMap");

			m_GUISelectAreaObject = GameObject.Instantiate( obj) as GameObject;
			m_SelectAreaDlg = m_GUISelectAreaObject.GetComponentInChildren<AsSelectAreaDlg>();
			m_GUISelectAreaObject.transform.parent = gameObject.transform;
		}
	}
	#endregion

	public bool IsOpenPartyList
	{
		get
		{
			if(null == m_PartyListDlg)
				return false;

			if(null == m_PartyEditDlg)
				return false;

			bool returnValue = false;
			if(m_PartyEditDlg.gameObject.active || m_PartyListDlg.gameObject.active)
				returnValue = true;

			return returnValue;
		}
	}

	public void ClosePartyList()
	{		
		if(null != m_PartyEditDlg)
			m_PartyEditDlg.Close();	
		if(null != m_PartyListDlg)
			m_PartyListDlg.Close();		
		if(null != m_PartyMatchingDlg)
			m_PartyMatchingDlg.Close();
		if(null != m_SelectAreaDlg)
			m_SelectAreaDlg.Close();		
		if( null != m_PartyMenuDlg)
			m_PartyMenuDlg.Close();
		if( null != m_PartyPRDlg)
			m_PartyPRDlg.Close();
		AsPartyManager.Instance.PartyUI.PartyCreateDlg.Close();
	}

	public void CloseSelectAreaDlg()
	{
		if(null == m_SelectAreaDlg)
			return;

		m_SelectAreaDlg.Close();
	}

	public bool OpenPartyList()
	{
	
		if(AsPartyManager.Instance.IsPartying)
		{
			if(null == m_PartyEditDlg)
				LoadPartyEditDlg();
			m_PartyEditDlg.Open();
		}
		else
		{
			if(null == m_PartyMenuDlg)
				LoadPartyMenuDlg();
			m_PartyMenuDlg.Open();
		}

		return true;
	}

	public bool OpenPartyListDlg()
	{
		ClosePartyList();
		if(null == m_PartyListDlg)
			LoadPartyListDlg();
		m_PartyListDlg.Open();

		return true;
	}

	public bool OpenPartyCreateDlg()
	{
		ClosePartyList();
		if(null == m_PartyCreateDlg)
			LoadPartyCreatDlg();
		m_PartyCreateDlg.Open();

		return true;
	}

	public bool EditPartyCreateDlg()
	{		
		if(null == m_PartyCreateDlg)
			LoadPartyCreatDlg();
		m_PartyCreateDlg.OpenEdit();

		return true;
	}

	public bool OpenPartyMatchingDlg()
	{
		ClosePartyList();
		if(null == m_PartyMatchingDlg)
			LoadPartyMatchingDlg();
		m_PartyMatchingDlg.Open();

		return true;
	}

	public bool OpenSelectAreaDlg()
	{
		if(null == m_SelectAreaDlg)
			LoadSelectAreaDlg();
		m_SelectAreaDlg.Open();

		return true;
	}

	public void AreaMapBtnClick(int iMapId, int iWarpIdx)
	{	
		bool reValue = false;
		switch( m_eUsingDlg)
		{
		case eUSING_DLG.PARTYCREATE_DLG:
			Debug.Log( "PARTYCREATE_DLG iMapId:" + iMapId + "iWarpIdx:" +iWarpIdx);
			reValue = m_PartyCreateDlg.ClickBtnMsg( iMapId, iWarpIdx);
			break;
		case eUSING_DLG.PARTYLIST_DLG:
			Debug.Log( "PARTYLIST_DLG iMapId:" + iMapId + "iWarpIdx:" +iWarpIdx);
			reValue = m_PartyListDlg.ClickBtnMsg( iMapId, iWarpIdx);
			break;
		case eUSING_DLG.PARTYMATCHING_DLG:
			Debug.Log( "PARTYMATCHING_DLG iMapId:" + iMapId + "iWarpIdx:" +iWarpIdx);
			reValue = m_PartyMatchingDlg.ClickBtnMsg( iMapId, iWarpIdx);
			break;
		}

		if(reValue && null != m_SelectAreaDlg && true == m_SelectAreaDlg.gameObject.active)
			m_SelectAreaDlg.SetFocusZoneMap(iMapId);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}
