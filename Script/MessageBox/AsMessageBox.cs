using UnityEngine;
using System.Collections;

public delegate void OkDelegate();
public delegate void CancelDelegate();

public class AsMessageBox : MonoBehaviour
{
	[SerializeField]UIButton ok = null;
	[SerializeField]UIButton cancel = null;
	[SerializeField]UIButton center = null;
	[SerializeField]SpriteText text = null;
	[SerializeField]SpriteText text_title = null;
	[HideInInspector] public MonoBehaviour script = null;
	[HideInInspector] public string method = "";
	[HideInInspector] public string method_cancel = "";
	[HideInInspector] public bool m_bUseBtnSoundOk = true;
	[HideInInspector] public bool m_bUseBtnSoundCancel = true;
	private OkDelegate okDelegate = null;
	public OkDelegate SetOkDelegate	{ set { okDelegate = value; } }
	private CancelDelegate cancelDelegate = null;
	public CancelDelegate SetCancelDelegate	{ set { cancelDelegate = value; } }

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		// ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_title);
		AsGameMain.isPopupExist = true;
	}
	
	void OnDestroy()
	{
		AsGameMain.isPopupExist = false;
	}

	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetOkText( string text)
	{
		ok.Text = text;
	}
	
	public void SetCancelText( string text)
	{
		cancel.Text = text;
	}
	
	public void SetCenterText( string text)
	{
		center.Text = text;
	}
	
	public void Close()
	{
		if( null != cancelDelegate)
			cancelDelegate();

		if( null != script)
		{
			if( method_cancel.Length > 0)
				script.Invoke( method_cancel, 0);
			else
				script.Invoke( "OnMessageBoxNotify", 0);
		}
		
#if UNITY_EDITOR
		DestroyImmediate( gameObject);
#else
		Destroy( gameObject);
#endif
	}
	
	public void Destroy_Only()
	{
#if UNITY_EDITOR
		DestroyImmediate( gameObject);
#else
		Destroy( gameObject);
#endif
	}
	
	public virtual void OnOK()
	{
		if(true == m_bUseBtnSoundOk)
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != script)
			script.Invoke( method, 0);

		if( null != okDelegate)
			okDelegate();

		Destroy( gameObject);
	}
	
	public void OnCancel()
	{
		if(true == m_bUseBtnSoundCancel)
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != script)
		{
			if( method_cancel.Length > 0)
				script.Invoke( method_cancel, 0);
			else
				script.Invoke( "OnMessageBoxNotify", 0);
		}

		if( null != cancelDelegate)
			cancelDelegate();
		
		Destroy( gameObject);
	}
	
	public void SetMessage( string msg)
	{
		text.Text = msg;
	}
	
	public void SetTitle( string title)
	{
		text_title.Text = title;
	}

	public void SetStyle( AsNotify.MSG_BOX_TYPE type, AsNotify.MSG_BOX_ICON icon)
	{
		switch( type)
		{
		case AsNotify.MSG_BOX_TYPE.MBT_OK:
			{
				ok.gameObject.SetActiveRecursively( false);
				cancel.gameObject.SetActiveRecursively( false);
				center.gameObject.SetActiveRecursively( true);
			}
			break;
		case AsNotify.MSG_BOX_TYPE.MBT_CANCEL:
			{
				ok.gameObject.SetActiveRecursively( false);
				cancel.gameObject.SetActiveRecursively( false);
				center.gameObject.SetActiveRecursively( true);
			}
			break;
		case AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL:
			center.gameObject.SetActiveRecursively( false);
			break;
		case AsNotify.MSG_BOX_TYPE.MBT_NOTHING:
			{
				cancel.gameObject.SetActiveRecursively( false);
				ok.gameObject.SetActiveRecursively( false);
				center.gameObject.SetActiveRecursively( false);
			}
			break;
		default:
			Debug.Log( "Invalid messagebox type");
			break;
		}
	}

	public void SetFullscreenCollider()
	{
		if( null == gameObject.collider)
			return;

		BoxCollider col = gameObject.collider as BoxCollider;
		col.size = new Vector3( 80.0f, 60.0f, 0.0f);
	}
}
