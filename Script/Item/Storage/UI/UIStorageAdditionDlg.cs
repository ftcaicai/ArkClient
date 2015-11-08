using UnityEngine;
using System.Collections;

public class UIStorageAdditionDlg : MonoBehaviour 
{
	public GameObject rootGameObject;
	
	public UIButton okButton;
	public UIButton cancelButton;
	
	public SpriteText titleText;
	public SpriteText contentText;
	

	public Color titleTextColor = Color.white;
	public Color contentTextColor = Color.white;
	
	public SpriteText m_TextCancel;
	public SpriteText m_TextConfirm;
		
	
//	private bool m_bNumPadBeginWrite = false;
	
	public void SetTitleText( string str )
	{
		if( null == titleText )
		{
			Debug.LogError("UIStorageAdditionDlg::SetTitleText() [ null == titleText ] ");
			return;
		}
		
		titleText.Text = titleTextColor + str; 
	}
	
	public void SetContentText( string str )
	{
		if( null == contentText )
		{
			Debug.LogError("UIStorageAdditionDlg::SetContentText() [ null == contentText ] ");
			return;
		}
		
		contentText.Text = contentTextColor + str;
	}
	
	public bool IsRect( Ray ray )
	{
		if( false == gameObject.active )
			return false;
		
		if( null == collider )
		{
			Debug.LogError("UIStorageAdditionDlg()[ null == rootGameObject.collider ] ");
			return false;
		}
		
//		return collider.bounds.IntersectRay( ray );		
		return AsUtil.PtInCollider( collider, ray);
	}
	
	public bool IsOpen()
	{
		return rootGameObject.active;
	}
	
	
	// Use this for initialization
	void Start () 
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( titleText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextCancel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextConfirm);
		
		SetTitleText( AsTableManager.Instance.GetTbl_String(1390));
		m_TextCancel.Text = AsTableManager.Instance.GetTbl_String( 1151);
		m_TextConfirm.Text = AsTableManager.Instance.GetTbl_String( 1152);
		
		okButton.SetInputDelegate(OkBtnDelegate);
		cancelButton.SetInputDelegate(CancelBtnDelegate);
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void Open(int _gold)
	{
		string text = AsTableManager.Instance.GetTbl_String(209);
		text = string.Format(text, Color.yellow.ToString() + _gold + "G" + Color.white.ToString());
		SetContentText(text);
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		gameObject.SetActiveRecursively(true);
	}
	
	
	public void Close()
	{
		gameObject.SetActiveRecursively(false);
	}
	
	private void OkBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			body_CS_STORAGE_COUNT_UP data = new body_CS_STORAGE_COUNT_UP();
			AsCommonSender.Send(data.ClassToPacketBytes());
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
	
	
	// input
	
	public void GuiInputUp(Ray inputRay)
	{ 
	}
}
