using UnityEngine;
using System.Collections;

public class AsCharacterSlotDeleteBox : MonoBehaviour
{
	public SpriteText time = null;
	public float remain = 0;
	public AsCharacterSlot slot = null;
	private bool isTouched = false;
	private AsMessageBox msgBox = null;
	
	// Use this for initialization
	void Start()
	{
		// ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( time);
	}
	
	// Update is called once per frame
	void Update()
	{
		if( 0 >= remain)
			return;
		
		remain -= ( Time.deltaTime);
		
		float hr = remain / 3600.0f;
		float min = ( remain % 3600.0f) / 60.0f;
		float sec = ( remain % 3600.0f) % 60.0f;
		
		time.Text = string.Format( "{0:D2}:{1:D2}:{2:D2}", (int)hr, (int)min, (int)sec);

		UpdateTouch();
	}
	
	public void StartDeleteState( int remain)
	{
		gameObject.SetActiveRecursively( 0 != remain);
		this.remain = (float)remain;
	}

	public void Hide()
	{
		gameObject.SetActiveRecursively( false);
		remain = 0;
	}
	
	private void UpdateTouch()
	{
		if( true == AsGameMain.isPopupExist)
			return;
		
		if( ( 0 >= Input.touchCount) || ( 1 != slot.ScreenIndex))
			return;
	
		switch( Input.GetTouch(0).phase)
		{
		case TouchPhase.Began:
			{
				if( false == AsUtil.PtInCollider( Camera.mainCamera, collider, Input.GetTouch(0).position))
					break;
				
				isTouched = true;
			}
			break;
		case TouchPhase.Ended:
			{
				if( ( true == isTouched) && ( true == AsUtil.PtInCollider( Camera.mainCamera, collider, Input.GetTouch(0).position)))
				{
					if( null != msgBox)
						break;
				
					msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), AsTableManager.Instance.GetTbl_String(8), this, "OnCancel",
						AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				}
				
				isTouched = false;
			}
			break;
		}
	}

	void OnMouseUpAsButton()
	{
		if( 1 != slot.ScreenIndex)
			return;
		
		if( null != msgBox)
			return;
		
		msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), AsTableManager.Instance.GetTbl_String(8), this, "OnCancel",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	private void OnCancel()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		slot.DeleteCancel();
	}
	
	private void OnMessageBoxNotify()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		MessageBoxClose();
	}
	
	public void MessageBoxClose()
	{
		if( null != msgBox)
			msgBox.Close();
	}
}
