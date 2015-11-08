using UnityEngine;
using System.Collections;

public class AsRankUsermenuMgr : MonoBehaviour
{
	private GameObject prevMenu = null;
	
	private static AsRankUsermenuMgr instance = null;
	public static AsRankUsermenuMgr Instance
	{
		get
		{
			if( null == instance)
				instance = FindObjectOfType( typeof( AsRankUsermenuMgr)) as AsRankUsermenuMgr;
			
			if( null == instance)
			{
				GameObject obj = new GameObject( "GUI_Ranking");
				instance = obj.AddComponent( typeof( AsRankUsermenuMgr)) as AsRankUsermenuMgr;
			}
			
			return instance;
		}
	}
	
	void OnDisable()
	{
		Close();
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null == prevMenu)
			return;
		
		Camera cam = UIManager.instance.rayCamera;
		
#if UNITY_EDITOR
		if( true == Input.GetMouseButtonUp(0))
		{
			Vector3 pos = cam.ScreenToWorldPoint( Input.mousePosition);
			if( false == AsUtil.PtInCollider( cam, prevMenu.collider, new Vector2( pos.x, pos.y), true))
				Close();
		}
#else
		if( 0 != Input.touchCount)
		{
			Touch touch = Input.GetTouch(0);
			Vector3 pos = cam.ScreenToWorldPoint( touch.position);
			
			switch( touch.phase)
			{
			case TouchPhase.Began:
				break;
			case TouchPhase.Ended:
				if( false == AsUtil.PtInCollider( cam, prevMenu.collider, new Vector2( pos.x, pos.y), true))
					Close();
				break;
			}
		}
#endif
	}
	
	public void Open( ushort sessionKey)
	{
		Close();
		
		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/UserMenu")) as GameObject;
		Debug.Assert( null != go);
		AsUserMenu menu = go.GetComponent<AsUserMenu>();
		Debug.Assert( null != menu);
#if UNITY_EDITOR
		Vector3 pos = UIManager.instance.rayCamera.ScreenToWorldPoint( Input.mousePosition);
#else
		Touch touch = Input.GetTouch(0);
		Vector3 pos = UIManager.instance.rayCamera.ScreenToWorldPoint( touch.position);
#endif
		menu.transform.position = new Vector3( pos.x, pos.y, gameObject.transform.position.z - 2.0f);
		menu.Open( sessionKey);
		
		prevMenu = go;
	}
	
	public void Close()
	{
		if( null != prevMenu)
			GameObject.Destroy( prevMenu);
	}
}
