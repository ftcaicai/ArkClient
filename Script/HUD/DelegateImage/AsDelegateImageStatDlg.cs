using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public class AsDelegateImageStatDlg : MonoBehaviour 
{

	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText msg = null;
	[SerializeField]SpriteText stat = null;
	[SerializeField]SimpleSprite image = null;
	[SerializeField]UIButton confirmBtn = null;
	
	// Use this for initialization
	void Start () 
	{
		title.Text = AsTableManager.Instance.GetTbl_String(4104);
		confirmBtn.Text = AsTableManager.Instance.GetTbl_String(4201);
	}
	
	
	public void Init( DelegateImageData imageData)
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( imageData.iconName);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		image.SetTexture( tex);
		image.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
		
		switch( imageData.eSubType)
		{
		case eDelegateImageSubType.Gold:
			msg.Text = string.Format( AsTableManager.Instance.GetTbl_String(4080), imageData.unlockCost.ToString( "#,#0", CultureInfo.InvariantCulture));
			break;
		case eDelegateImageSubType.Miracle:
			msg.Text = string.Format( AsTableManager.Instance.GetTbl_String(4079), imageData.unlockCost.ToString( "#,#0", CultureInfo.InvariantCulture));
			break;

		default:
			msg.Text = AsTableManager.Instance.GetTbl_String(imageData.idDescription);
			break;
		}
		
		stat.Text = AsTableManager.Instance.GetTbl_String( imageData.idEffectDescription );
	}
	
	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	void OnConfirmBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Destroy( gameObject);
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
