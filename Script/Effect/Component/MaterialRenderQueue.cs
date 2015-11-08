using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/RenderQueue")]
public class MaterialRenderQueue : MonoBehaviour 
{
	public int renderQueue = 0;
	
	// Use this for initialization
	void Start () 
	{
		foreach( Material mtrl in renderer.materials)
			mtrl.renderQueue = renderQueue;
	}
	
	// Update is called once per frame
	void Update ()
	{
//#if UNITY_EDITOR
		foreach( Material mtrl in renderer.materials)
			mtrl.renderQueue = renderQueue;
//#endif
	}
}
