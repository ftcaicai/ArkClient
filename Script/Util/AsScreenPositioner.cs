using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/ScreenPositioner")]

public class AsScreenPositioner : MonoBehaviour
{
	public enum AlignType
	{
		LeftTop,
		Top,
		RightTop,
		LeftMiddle,
		Center,
		RightMiddle,
		LeftBottom,
		Bottom,
		RightBottom,
	};
	
	[SerializeField]Camera uiCamera = null;
	public AlignType align = AlignType.LeftTop;
	
	// Use this for initialization
	void Start()
	{
		if( null == uiCamera)
			uiCamera = UIManager.instance.rayCamera;
			
		Vector3 screenPos = Vector3.zero;
		switch( align)
		{
		case AlignType.LeftTop:	screenPos = new Vector3( 0.0f, Screen.height, 0.0f);	break;
		case AlignType.Top:	screenPos = new Vector3( Screen.width * 0.5f, Screen.height, 0.0f);	break;
		case AlignType.RightTop:	screenPos = new Vector3( Screen.width, Screen.height, 0.0f);	break;
		case AlignType.LeftMiddle:	screenPos = new Vector3( 0.0f, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.Center:	screenPos = new Vector3( Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.RightMiddle:	screenPos = new Vector3( Screen.width, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.LeftBottom:	screenPos = new Vector3( 0.0f, 0.0f, 0.0f);	break;
		case AlignType.Bottom:	screenPos = new Vector3( Screen.width * 0.5f, 0.0f, 0.0f);	break;
		case AlignType.RightBottom:	screenPos = new Vector3( Screen.width, 0.0f, 0.0f);	break;
		}
		
		Vector3 worldPosition = uiCamera.ScreenToWorldPoint( screenPos);
		worldPosition.z = gameObject.transform.position.z;
		gameObject.transform.position = worldPosition;
		
		gameObject.BroadcastMessage( "AlignScreenPos", SendMessageOptions.DontRequireReceiver);
	}
	
	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		Vector3 screenPos = Vector3.zero;
		switch( align)
		{
		case AlignType.LeftTop:	screenPos = new Vector3( 0.0f, Screen.height, 0.0f);	break;
		case AlignType.Top:	screenPos = new Vector3( Screen.width * 0.5f, Screen.height, 0.0f);	break;
		case AlignType.RightTop:	screenPos = new Vector3( Screen.width, Screen.height, 0.0f);	break;
		case AlignType.LeftMiddle:	screenPos = new Vector3( 0.0f, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.Center:	screenPos = new Vector3( Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.RightMiddle:	screenPos = new Vector3( Screen.width, Screen.height * 0.5f, 0.0f);	break;
		case AlignType.LeftBottom:	screenPos = new Vector3( 0.0f, 0.0f, 0.0f);	break;
		case AlignType.Bottom:	screenPos = new Vector3( Screen.width * 0.5f, 0.0f, 0.0f);	break;
		case AlignType.RightBottom:	screenPos = new Vector3( Screen.width, 0.0f, 0.0f);	break;
		}
		
		Vector3 worldPosition = uiCamera.ScreenToWorldPoint( screenPos);
		worldPosition.z = gameObject.transform.position.z;
		gameObject.transform.position = worldPosition;
#endif
	}
}
