using UnityEngine;
using System.Collections;

public class StrengthenBalloon : MonoBehaviour {

	public float fTime = 2.0f;	
	public UISimpleSpriteAni spriteAni;
	
	private AsUserEntity entity = null;
	
	public AsUserEntity Owner
	{
		set	{ entity = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		StartCoroutine( RemoveBalloon());
		if( null != spriteAni )
			spriteAni.Play();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null == entity )
			return;
		
		if( null == entity.ModelObject || null == entity.characterController )
			return;
		
		Vector3 worldPos = entity.ModelObject.transform.position;
		worldPos.y += entity.characterController.height;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.y += 2.0f;
		vRes.z = 0.0f;
		gameObject.transform.position = vRes;
	}	
	
	private IEnumerator RemoveBalloon()
	{
		while( true)
		{
			yield return new WaitForSeconds( fTime );
			GameObject.DestroyImmediate( gameObject);
		}
	}

}