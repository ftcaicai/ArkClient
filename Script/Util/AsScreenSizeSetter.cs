using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRoot))]
public class AsScreenSizeSetter : MonoBehaviour
{
	public float sizeX = 1.0f;
	public float sizeY = 1.0f;
	
	private SpriteRoot sprite = null;
	
	// Use this for initialization
	void Start()
	{
		sprite = gameObject.GetComponent<SpriteRoot>();
		
#if !UNITY_EDITOR
		sprite.width = ( sprite.RenderCamera.orthographicSize * sprite.RenderCamera.aspect * 2.0f) * sizeX;
		sprite.height = sprite.RenderCamera.orthographicSize * 2.0f * sizeY;
		sprite.CalcSize();
#endif
	}
	
	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		sprite.width = ( sprite.RenderCamera.orthographicSize * sprite.RenderCamera.aspect * 2.0f) * sizeX;
		sprite.height = sprite.RenderCamera.orthographicSize * 2.0f * sizeY;
		sprite.CalcSize();
#endif
	}
}
