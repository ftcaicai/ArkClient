using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;

public class AsDelegateImageItemGetDlg : MonoBehaviour 
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText msg = null;
	[SerializeField]SpriteText stat = null;
	[SerializeField]SimpleSprite image = null;
	[SerializeField]UIButton closeBtn = null;
	[SerializeField]UIButton cancelBtn = null;
	[SerializeField]UIButton obtain = null;

	int				m_nSlot = -1;

	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(2710);

		closeBtn.SetInputDelegate( OnCloseBtn );

		cancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);
		cancelBtn.SetInputDelegate( OnCancelBtn );

		obtain.Text = AsTableManager.Instance.GetTbl_String(2713);
		obtain.SetInputDelegate( OnObtainBtn );
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( DelegateImageData imageData , int _nSlot )
	{
		switch( imageData.eSubType)
		{
		case eDelegateImageSubType.Item:
			msg.Text = AsTableManager.Instance.GetTbl_String(2711);
			break;
		}
		
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( imageData.iconName);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		image.SetTexture( tex);
		image.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
		
		stat.Text = AsTableManager.Instance.GetTbl_String( imageData.idEffectDescription );

		m_nSlot = _nSlot;
	}
	
	void OnCloseBtn(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	void OnCancelBtn(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	void OnObtainBtn(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
			return;
		
		Destroy( gameObject);

		AsCommonSender.SendUseItem ( m_nSlot );
	}
}
