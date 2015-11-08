using UnityEngine;
using System.Collections;

[RequireComponent ( typeof( UIScrollList))]
public class AsScrollListAreaSetter : MonoBehaviour
{
	public float sizeX = 1.0f;
	public float sizeY = 1.0f;
	
	[SerializeField] Camera uiCamera = null;
	[SerializeField] UIScrollList scrollList = null;
	
	void Start()
	{
#if !UNITY_EDITOR
		if( scrollList != null)
		{
			scrollList.viewableArea.x = ( scrollList.renderCamera.orthographicSize * scrollList.renderCamera.aspect * 2.0f) * sizeX;
			scrollList.viewableArea.y = scrollList.renderCamera.orthographicSize * 2.0f * sizeY;
			
			scrollList.UpdateCamera();
		}
		else
			Debug.LogError( "AsScrollListAreaSetter::Start: scroll list is not set");
#endif
	}
	
	void Update()
	{
#if UNITY_EDITOR
		if( scrollList != null)
		{
			scrollList.viewableArea.x = ( scrollList.renderCamera.orthographicSize * scrollList.renderCamera.aspect * 2.0f) * sizeX;
			scrollList.viewableArea.y = scrollList.renderCamera.orthographicSize * 2.0f * sizeY;
			
			scrollList.UpdateCamera();
		}
		else
			Debug.LogError("AsScrollListAreaSetter::Start: scroll list is not set");
#endif
	}
}
