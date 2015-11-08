using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public class AsDelegateImageConfirmDlg : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText msg = null;
	[SerializeField]SpriteText stat = null;
	[SerializeField]SimpleSprite image = null;
	[SerializeField]UIButton cancelBtn = null;
	[SerializeField]UIButton buyBtn = null;
//	private AsDelegateImageDlgItem parentItem = null;

	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1412);
		cancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);
		buyBtn.Text = AsTableManager.Instance.GetTbl_String(1386);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( AsDelegateImageDlgItem parent, DelegateImageData imageData)
	{
//		parentItem = parent;
		
		switch( imageData.eSubType)
		{
		case eDelegateImageSubType.Gold:
			msg.Text = string.Format( AsTableManager.Instance.GetTbl_String(4080), imageData.unlockCost.ToString( "#,#0", CultureInfo.InvariantCulture));
			break;
		case eDelegateImageSubType.Miracle:
			msg.Text = string.Format( AsTableManager.Instance.GetTbl_String(4079), imageData.unlockCost.ToString( "#,#0", CultureInfo.InvariantCulture));
			break;

		default:
			msg.Text = "";
			break;
		}
		
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( imageData.iconName);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		image.SetTexture( tex);
		image.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
		
		stat.Text = AsTableManager.Instance.GetTbl_String( imageData.idEffectDescription );
	}
	
	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	void OnCancelBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	void OnBuyBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsInstanceDungeonManager.Instance.CheckInIndun() || true == AsPvpManager.Instance.CheckInArena())
			return;

		Destroy( gameObject);
		
		body_CS_IMAGE_BUY imageBuy = new body_CS_IMAGE_BUY( AsDelegateImageManager.Instance.SelectedImageID);
		byte[] data = imageBuy.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
}
