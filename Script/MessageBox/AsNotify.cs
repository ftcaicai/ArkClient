using UnityEngine;
using System.Collections;
using System.Globalization;


public class AsNotify : MonoBehaviour
{
	public enum MSG_BOX_TYPE
	{
		MBT_INVALID = -1,
		MBT_OK,
		MBT_CANCEL,
		MBT_OKCANCEL,
		MBT_NOTHING,
		NUM_MSG_BOX_TYPE
	};

	public enum MSG_BOX_ICON
	{
		MBI_INVALID = -1,
		MBI_NOTICE,
		MBI_QUESTION,
		MBI_WARNING,
		MBI_ERROR,

		NUM_MSG_BOX_ICON
	};

	GameObject m_RootObject;

	public GameObject msgBoxSrc = null;
	public GameObject msgCashBoxSrc = null;
	public GameObject msgGoldBoxSrc = null;
	public GameObject msgItemViewBoxSrc = null;
	public GameObject msgBoxSrc_Patch = null;
	public GameObject msgSkillResetSrc = null;
	static private AsNotify instance = null;
	static public AsNotify Instance
	{
		get	{ return instance; }
	}

	void Awake()
	{
		instance = this;
		m_RootObject = new GameObject( "MessageBox");
		DontDestroyOnLoad( m_RootObject);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public AsMessageBox MessageBox( string title, string msg, string ok, string cancel, MonoBehaviour script=null, string method=null, MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool bUseBtnSoundOk=true, bool bUseBtnSoundCancel = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.SetOkText( ok);
		msgBox.SetCancelText( cancel);
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundCancel;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( ok);	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( cancel);	break;
		}

		return msgBox;
	}

	public AsMessageBox MessageBox( string title, string msg, string ok, string cancel, MonoBehaviour script = null, string method_ok = null, string method_cancel = null, MSG_BOX_TYPE type = MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon = MSG_BOX_ICON.MBI_NOTICE, bool bUseBtnSoundOk = true, bool bUseBtnSoundCancel = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method_ok;
		msgBox.method_cancel = method_cancel;
		msgBox.SetOkText( ok);
		msgBox.SetCancelText( cancel);
		msgBox.transform.position = new Vector3( -500.0f, 0, -43.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundCancel;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( ok);	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( cancel);	break;
		}

		return msgBox;
	}

	public AsMessageBox MessageBox( string title, string msg, MSG_BOX_TYPE type = MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon = MSG_BOX_ICON.MBI_NOTICE, bool bUseBtnSoundOk = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = null;
		msgBox.method = null;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundOk;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox MessageBox( string title, string msg, MonoBehaviour script, string method, MSG_BOX_TYPE type, MSG_BOX_ICON icon, bool bUseBtnSoundOk = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundOk;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox MessageBox( string title, string msg, MonoBehaviour script, string method_ok, string method_cancel, MSG_BOX_TYPE type, MSG_BOX_ICON icon, bool bUseBtnSoundOk = true, bool bUseBtnSoundCancel = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method_ok;
		msgBox.method_cancel = method_cancel;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundCancel;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox CashMessageBox( long cashCost, string title, string msg, MonoBehaviour script = null, string method = null, MSG_BOX_TYPE type = MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon = MSG_BOX_ICON.MBI_NOTICE, bool lackAlarm = false)
	{
		return CashMessageBox( cashCost, title, msg, script, method, string.Empty, type, icon, lackAlarm);
	}

	public AsMessageBox CashMessageBox( long cashCost, string title, string msg, MonoBehaviour script=null, string method=null, string methodCancel = null, MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool lackAlarm=false)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgCashBoxSrc) as GameObject;
		AsCashMessageBox msgBox = clone.GetComponent<AsCashMessageBox>() as AsCashMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.method_cancel = methodCancel;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		#region - lack alarm - //$yde
		string cost = cashCost.ToString();
		if( lackAlarm == true && AsUserInfo.Instance.nMiracle < cashCost)
			cost = Color.red.ToString() + cost;
		#endregion
		msgBox.SetCashText( cashCost);
		msgBox.SetHaveCashText( AsUserInfo.Instance.nMiracle);
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		msgBox.text_HaveCashTitle.Text = AsTableManager.Instance.GetTbl_String(859);
		msgBox.text_CashTitle.Text     = AsTableManager.Instance.GetTbl_String(858);

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox ItemViewMessageBox( int iItemID, string strPreItemID, string title, string msg, MonoBehaviour script=null, string method=null,
		MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool isShowTooltip = false)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgItemViewBoxSrc) as GameObject;
		AsItemViewMessageBox msgBox = clone.GetComponent<AsItemViewMessageBox>() as AsItemViewMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -7.0f);
		msgBox.SetReciveItem( iItemID, strPreItemID, isShowTooltip);
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox ItemViewMessageBox( int iItemID, string strPreItemID, string title, string msg, AsItemViewMessageBox.eventFunction _eventFunction,
		MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool isShowTooltip = false)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgItemViewBoxSrc) as GameObject;
		AsItemViewMessageBox msgBox = clone.GetComponent<AsItemViewMessageBox>() as AsItemViewMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = null;
		msgBox.method = null;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.m_EventFunction = _eventFunction;
		msgBox.transform.position = new Vector3( -500.0f, 0, -7.0f);
		msgBox.SetReciveItem( iItemID, strPreItemID, isShowTooltip);
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox GoldMessageBox( ulong goldCost, string title, string msg, MonoBehaviour script=null, string method=null, MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool lackAlarm=false)
	{
		if( null == msgGoldBoxSrc)
			return null;

		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgGoldBoxSrc) as GameObject;
		AsGoldMessageBox msgBox = clone.GetComponent<AsGoldMessageBox>() as AsGoldMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		#region - lack alarm - //$yde
		string cost = goldCost.ToString( "#,#0", CultureInfo.InvariantCulture);
		if( lackAlarm == true && AsUserInfo.Instance.nMiracle < (long)goldCost)
			cost = Color.red.ToString() + cost;
		#endregion
		msgBox.SetCashText( cost);
		msgBox.gameObject.transform.parent = m_RootObject.transform;
		
		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}

		return msgBox;
	}

	public AsMessageBox MessageBox_Patch( string title, string msg, string ok, string cancel, MonoBehaviour script = null, string method_ok = null, string method_cancel = null, MSG_BOX_TYPE type = MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon = MSG_BOX_ICON.MBI_NOTICE, bool bUseBtnSoundOk = true, bool bUseBtnSoundCancel = true)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( msgBoxSrc_Patch) as GameObject;
		AsMessageBox msgBox = clone.GetComponent<AsMessageBox>() as AsMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method_ok;
		msgBox.method_cancel = method_cancel;
		msgBox.SetOkText( ok);
		msgBox.SetCancelText( cancel);
		msgBox.transform.position = new Vector3( -500.0f, 0, -43.0f);
		msgBox.m_bUseBtnSoundOk = bUseBtnSoundOk;
		msgBox.m_bUseBtnSoundCancel = bUseBtnSoundCancel;
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( ok);	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( cancel);	break;
		}
	
		return msgBox;
	}
	
	public AsMessageBox SkillResetMessageBox( long cashCost, string title, string msg, MonoBehaviour script=null, string method=null, string methodCancel = null, MSG_BOX_TYPE type=MSG_BOX_TYPE.MBT_OK, MSG_BOX_ICON icon=MSG_BOX_ICON.MBI_NOTICE, bool lackAlarm=false)
	{
		CloseAllMessageBox();
		
		GameObject clone = GameObject.Instantiate( msgSkillResetSrc) as GameObject;
		AsSkillResetMessageBox msgBox = clone.GetComponent<AsSkillResetMessageBox>() as AsSkillResetMessageBox;
		msgBox.SetStyle( type, icon);
		msgBox.SetTitle( title);
		msgBox.SetMessage( msg);
		msgBox.script = script;
		msgBox.method = method;
		msgBox.method_cancel = methodCancel;
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		msgBox.transform.position = new Vector3( -500.0f, 0, -40.0f);
		#region - lack alarm - //$yde
		string cost = cashCost.ToString();
		if( lackAlarm == true && AsUserInfo.Instance.nMiracle < cashCost)
			cost = Color.red.ToString() + cost;
		#endregion
		msgBox.SetCashText( cashCost);
		msgBox.SetHaveCashText( AsUserInfo.Instance.nMiracle);
		msgBox.gameObject.transform.parent = m_RootObject.transform;

		msgBox.text_HaveCashTitle.Text = AsTableManager.Instance.GetTbl_String(859);
		msgBox.text_CashTitle.Text     = AsTableManager.Instance.GetTbl_String(858);
		
		switch( type)
		{
		case MSG_BOX_TYPE.MBT_OK:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1152));	break;
		case MSG_BOX_TYPE.MBT_CANCEL:	msgBox.SetCenterText( AsTableManager.Instance.GetTbl_String(1151));	break;
		}
		
		return msgBox;
	}	
	
	public void CloseAllMessageBox()
	{
		m_RootObject.BroadcastMessage( "Close", SendMessageOptions.DontRequireReceiver);
	}
	
	public bool IsOpenMessageBox()
	{
		if( null != m_RootObject)
		{
			if( m_RootObject.transform.childCount > 0)
				return true;
		}
		
		return false;
	}

	public void DeathDlg()
	{
		if( deathDlg == null)
		{
			CloseAllMessageBox();

			GameObject clone = GameObject.Instantiate( Resources.Load( "UI/Optimization/Prefab/DeathDlg_Cash")) as GameObject;
//			clone.transform.position = new Vector3( -500.0f, 0.0f, -35.0f);
//			clone.transform.position = new Vector3( -500.0f, 0.0f, -20.0f);

			deathDlg = clone;//$yde
		}
		else
			Debug.Log( "AsNotify::DeathDlg: death dialog is already instantiated");
	}

	public void DeathDlgFriend( AsUserEntity _user)
	{
		CloseAllMessageBox();

		GameObject clone = GameObject.Instantiate( Resources.Load( "UI/Optimization/Prefab/DeathDlgFriend")) as GameObject;
		clone.GetComponent<AsDeathDlgFriend>().Init( _user);
		clone.transform.position = new Vector3( -500.0f, 0.0f, -35.0f);
		
		deathDlgFriend = clone;
	}

	GameObject deathDlg = null;
	GameObject deathDlgFriend = null;
	public void CloseDeathDlg()
	{
		if( null == deathDlg)
			return;

		Destroy( deathDlg);
	}

	public void Hidden()
	{
		//	m_RootObject must be active 
		CloseAllMessageBox();
		return;
		
		m_RootObject.SetActiveRecursively( false);
	}
	
	AsCashStore CashStore = null;
	public AsCashStore CashStoreRef
	{
		set { CashStore = value;}
		get { return CashStore;}
	}
	
	public bool IsOpenDeathDlg
	{
		get
		{
			if( null == deathDlg)
				return false;
			
			return true;
		}
	}
	
	public bool IsOpenDeathDlgFriend
	{
		get
		{
			if( null == deathDlgFriend)
				return false;
			return true;
		}
	}
	
	public void CloseDeathDlgFriend()
	{
		if( null == deathDlgFriend)
			return;

		Destroy( deathDlgFriend);
	}
}
