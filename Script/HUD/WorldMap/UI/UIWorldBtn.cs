using UnityEngine;
using System.Collections;


[AddComponentMenu( "WorldCtrl/UIWorldBtn" )]
public class UIWorldBtn : MonoBehaviour 
{	
	public int areaIndex;
	private GameObject partyImg;
	public GameObject ImgPos;
	
	public bool IsRect( Ray _inputRay )
	{
		if( null == collider )
			return false;
		
//		return collider.bounds.IntersectRay( _inputRay );
		return AsUtil.PtInCollider( collider, _inputRay);
	}
	
	public void ClearPartyImg()
	{
		if( null != partyImg )
		{
			GameObject.DestroyObject( partyImg );
		}
	}
	
	public void SetPartyImg( string strPath )
	{
		ClearPartyImg();
		Transform trmPar = transform;
		if( null!=ImgPos)
		{
			trmPar = ImgPos.transform;
		}
		partyImg = ResourceLoad.CreateGameObject( strPath, trmPar );
		//if( null == partyImg )
		//	return;
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
