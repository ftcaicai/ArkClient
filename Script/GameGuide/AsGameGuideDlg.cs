using UnityEngine;
using System.Collections;
using System.Text;

public class AsGameGuideDlg : MonoBehaviour
{
	[SerializeField]private SpriteText title = null;
	[SerializeField]private SimpleSprite image = null;
	[SerializeField]private GameObject root = null;
	
	// Use this for initialization
//	IEnumerator Start()
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1992);
		
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.BringIn();
		
//		yield return new WaitForSeconds( 5.0f);
//		
//		GameObject.DestroyImmediate( gameObject.transform.parent.gameObject);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( GameGuideData data)
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/GameGuide/");
		sb.Append( data.imagePath);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		image.SetTexture( tex);
		image.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//		GameObject.Destroy( gameObject.transform.parent.gameObject);
		GameObject.Destroy( root);
	}
	
	public void Close()
	{
//		GameObject.Destroy( gameObject.transform.parent.gameObject);
		GameObject.Destroy( root);
	}
}
