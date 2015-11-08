using UnityEngine;
using System.Collections;

public class AsProductBallon : MonoBehaviour {

	public Vector3 localPos;	
	public UISimpleSpriteAni spriteAni;	
	private AsUserEntity entity = null;
	
	public AsUserEntity Owner
	{
		set	{ entity = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		
		if( null != spriteAni )
			spriteAni.Play();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null == entity )
		{
			if( null != gameObject )
				GameObject.Destroy( gameObject );
			
			return;
		}
		
		if( null == entity.ModelObject )
			return;
		
		Vector3 worldPos = entity.ModelObject.transform.position;
		worldPos.y += entity.characterController.height;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.x += localPos.x;
		vRes.y += localPos.y;
		vRes.z = localPos.z;
		gameObject.transform.position = vRes;
	}		
}
