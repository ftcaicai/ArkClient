using UnityEngine;
using System.Collections;

public class UIStoragePage : MonoBehaviour 
{
	public Transform []pagePositions;
	public SimpleSprite pageImg;
	public Collider nextPageRect;
	public Collider prePageRect;
	
	
	private int m_iCurPage = 0;
	
	
	// page index 
	
	public int curPage
	{
		get
		{
			return m_iCurPage;
		}
	}
	
	public void SetPage( int iPage )
	{
		if( null == pagePositions )
		{
			Debug.LogError("UIStoragePage::SetPage() [null == pagePositions]");	
			return;
		}
		
		if( pagePositions.Length <= iPage )
		{
			Debug.LogError("UIStoragePage::SetPage() [pagePositions.Length <= iPage]");	
			return;
		}
		
		m_iCurPage = iPage;
			
		if( null != pageImg )
		{
			Vector3 vec3Temp = pagePositions[m_iCurPage].transform.position;			
			pageImg.transform.position = vec3Temp;
		}		
	}
	
	public bool NextPage()
	{
		if( null == pagePositions )
		{
			Debug.LogError("UIStoragePage::NextPage() [null == pagePositions]");	
			return false;
		}		
		
		if( pagePositions.Length <= m_iCurPage + 1 )
			SetPage(0);
		else			
			SetPage( m_iCurPage + 1 );	
		
		return true;
	}
	
	public bool PrePage()
	{			
		if( 0 > m_iCurPage - 1 )
			SetPage( pagePositions.Length - 1 );
		else		
			SetPage( m_iCurPage - 1 );
		
		return true;
	}
	
	
	//collider 
	
	public bool IsPageeRectIntersect( Ray ray )
	{
		if( null == collider )
		{
			Debug.LogError("UIStoragePage::IsPageeRectIntersect() [null == collider]");
			return false;
		}
		
//		return collider.bounds.IntersectRay(ray);
		return AsUtil.PtInCollider( collider, ray);
	}
	
	public bool IsPageNextRectIntersect( Ray ray )
	{
		if( null == nextPageRect )
		{
			Debug.LogError("UIStoragePage::IsPageNextRectIntersect() [null == nextPageRect]");
			return false;
		}
		
//		return nextPageRect.bounds.IntersectRay(ray);
		return AsUtil.PtInCollider( nextPageRect, ray);
	}
	
	public bool IsPagePreRectIntersect( Ray ray )
	{
		if( null == prePageRect )
		{
			Debug.LogError("UIStoragePage::IsPagePreRectIntersect() [null == prePageRect]");
			return false;
		}
		
//		return prePageRect.bounds.IntersectRay(ray);
		return AsUtil.PtInCollider( prePageRect, ray);
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		SetPage(0);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
