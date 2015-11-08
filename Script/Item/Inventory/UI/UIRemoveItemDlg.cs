using UnityEngine;
using System.Collections;

public class UIRemoveItemDlg : MonoBehaviour 
{
	
	public enum eDLG_TYPE
	{
		REMOVE,
		POSTBOX,
		STORAGE
	}
	
	
	public GameObject rootGameObject;
	
	public UIButton okButton;
	public UIButton cancelButton;
	public UIButton addButton;
	public UIButton minusButton;
	
	public UIButton addButton_1;
	public UIButton minusButton_1;
	
	public SpriteText titleText;
	public SpriteText contentText;
	public SpriteText inputText;
	public SpriteText textCancel;
	public SpriteText textConfirm;
	
	
	public Color titleTextColor = Color.black;
	public Color contentTextColor = Color.white;
	public Color inputTextColor = Color.black;
	
	
	private int m_iSlotIndex = 0;
	private int m_iRemoveCount = 0;
	private int m_iMaxRemoveCount = 0;
	
	
	private bool m_bNumPadBeginWrite = false;
	
	public void SetTitleText( string str )
	{
		if( null == titleText )
		{
			Debug.LogError("UIRemoveItemDlg::SetTitleText() [ null == titleText ] ");
			return;
		}
		
		titleText.Text = titleTextColor + str; 
	}
	
	public void SetContentText( string str )
	{
		if( null == contentText )
		{
			Debug.LogError("UIRemoveItemDlg::SetContentText() [ null == contentText ] ");
			return;
		}
		
		contentText.Text = contentTextColor + str;
	}
	
	public void SetInputText( string str )
	{
		if( null == inputText )
		{
			Debug.LogError("UIRemoveItemDlg::SetInputText() [ null == inputText ] ");
			return;
		} 
		
		inputText.Text = inputTextColor + str;
	}
	
	public void SetRemoveCount( int iData)
	{
		m_iRemoveCount = iData;
		if( 1 > m_iRemoveCount )
			m_iRemoveCount = 1;
		if( m_iMaxRemoveCount < m_iRemoveCount )
			m_iRemoveCount = m_iMaxRemoveCount;
		
		SetInputText( m_iRemoveCount.ToString() );
	}
	
	public bool IsRect( Ray ray )
	{
		if( false == rootGameObject.active )
			return false;
		
		if( null == rootGameObject.collider )
		{
			Debug.LogError("UIRemoveItemDlg()[ null == rootGameObject.collider ] ");
			return false;
		}
		
//		return rootGameObject.collider.bounds.IntersectRay( ray );
		return AsUtil.PtInCollider( rootGameObject.collider, ray);
	}
	
	public bool IsOpen()
	{
		if( false == rootGameObject )
		{
			Debug.LogError("UIRemoveItemDlg::IsOpen() [ null == rootGameObject ]");
			return false;
		}
		
		return rootGameObject.active;
	}
	
	void Awake()
    {
		textCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
		textConfirm.Text = AsTableManager.Instance.GetTbl_String( 1152);
		
		SetRemoveCount(0);
		
		cancelButton.SetInputDelegate(CancelBtnDelegate);
		addButton.SetInputDelegate(AddDataBtnDelegate);
		minusButton.SetInputDelegate(MinusDataBtnDelegate);	
		
		if( null != addButton_1 )
			addButton_1.SetInputDelegate(AddDataBtnDelegate_1);
		
		if( null != minusButton_1 )
			minusButton_1.SetInputDelegate(MinusDataBtnDelegate_1);	
	}
	
	// Use this for initialization
	void Start () 
	{
		/*AsLanguageManager.Instance.SetFontFromSystemLanguage( titleText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( inputText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textConfirm);*/
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void Open( int iSlotIndex, int iMaxCount, eDLG_TYPE eType )
	{
		switch( eType )
		{
		case eDLG_TYPE.REMOVE:
			SetTitleText( AsTableManager.Instance.GetTbl_String(1099));
			SetContentText( AsTableManager.Instance.GetTbl_String(6));
			okButton.SetInputDelegate(OkBtnRemoveDelegate);
			break;
			
		case eDLG_TYPE.POSTBOX:
			SetTitleText( AsTableManager.Instance.GetTbl_String(107));
			SetContentText( AsTableManager.Instance.GetTbl_String(108));
			okButton.SetInputDelegate(OkBtnPostBoxDelegate);
			break;
			
		case eDLG_TYPE.STORAGE:
			SetTitleText( AsTableManager.Instance.GetTbl_String(1099));
			SetContentText( AsTableManager.Instance.GetTbl_String(6));
			okButton.SetInputDelegate(OkBtnStorageDelegate);
			break;
		}
		m_iMaxRemoveCount = iMaxCount;
		m_iSlotIndex = iSlotIndex;
		SetRemoveCount(m_iMaxRemoveCount);		
		
		if( null == rootGameObject)
		{
			Debug.LogError("UIRemoveItemDlg::Open() [ null == rootGameObject ]");
			return;
		}
		rootGameObject.SetActiveRecursively(true);
	}
	
	
	public void Close()
	{
		if( null == rootGameObject)
		{
			Debug.LogError("UIRemoveItemDlg::Close() [ null == rootGameObject ]");
			return;
		}
		rootGameObject.SetActiveRecursively(false);
	}
	
	private void OkBtnRemoveDelegate( ref POINTER_INFO ptr )
	{
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP )
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
           if( true == AsHudDlgMgr.Instance.IsOpenInven )
			    AsHudDlgMgr.Instance.invenDlg.OpenReallyRemoveItemDlg( m_iSlotIndex, m_iRemoveCount );							
			Close();
		}
	}	
	
	private void OkBtnPostBoxDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( AsHudDlgMgr.Instance.IsOpenedPostBox )
			{
				AsPostBoxDlg postBox = AsHudDlgMgr.Instance.postBoxDlgObj.GetComponentInChildren<AsPostBoxDlg>();
				Debug.Assert( null != postBox);
				postBox.SetUseOpenDlgItemSlot( m_iSlotIndex, m_iRemoveCount);
//				AsHudDlgMgr.Instance.postBoxDlg.SetUseOpenDlgItemSlot( m_iSlotIndex, m_iRemoveCount );
			}
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);							
			Close();
		}
	}
	
	private void OkBtnStorageDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.storageDlg.OpenReallyRemoveItemDlg( m_iSlotIndex, m_iRemoveCount );							
			Close();
		}
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}
	
	private void AddDataBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetRemoveCount( m_iRemoveCount + 1 );
		}
	}
	
	private void MinusDataBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetRemoveCount( m_iRemoveCount - 1 );
		}
	}
	
	
	private void AddDataBtnDelegate_1( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetRemoveCount( m_iRemoveCount + 10 );
		}
	}
	
	private void MinusDataBtnDelegate_1( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetRemoveCount( m_iRemoveCount - 10 );
		}
	}
	
	
	// input
	
	public void GuiInputUp(Ray inputRay)
	{ 
	}
	
	
	public void InputDelegate(int iData)
	{
		
		
		int iRemoveCount = m_iRemoveCount;
		
		if( -1 == iData )
		{
			if( 10 < iRemoveCount )
			{
				SetRemoveCount( iRemoveCount / 10 );
			}
			else
			{
				m_iRemoveCount = 0;
				SetInputText("");
			}
		}
		else
		{
			if( true == m_bNumPadBeginWrite )
			{
				SetRemoveCount( iData );
			}
			else
			{
				SetRemoveCount( (iRemoveCount * 10) + iData );				
			}
		}
		
		m_bNumPadBeginWrite = false;
	}
}
