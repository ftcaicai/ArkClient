using UnityEngine;
using System.Collections;

public class UIInvenPage : MonoBehaviour 
{
	
	public Transform []pagePositions;
	public SimpleSprite pageImg;
	public Collider nextPageRect;
	public Collider prePageRect;
	public Collider[] pageList;
	
	
	private int m_iCurPage = 0;
	
	
	public int IsPageIntersect( Ray ray )
	{
		if( null == pageList )
		{
			Debug.LogError("UIInvenPage::IsPageeRectIntersect() [null == pageList]");
			return -1;
		}
		
		for( int i=0; i<pageList.Length; ++i )
		{
			Collider _collider = pageList[i];
				
			if( null == _collider )
			{
				Debug.LogError("UIInvenPage::IsPageeRectIntersect() [null == collider]");
				continue;
			}		
			
			if( true == AsUtil.PtInCollider( _collider, ray, false ) )
				return i;				
		}
		
		return -1;
	}
	
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
			Debug.LogError("UIInvenPage::SetPage() [null == pagePositions]");	
			return;
		}
		
		if( pagePositions.Length <= iPage )
		{
			Debug.LogError("UIInvenPage::SetPage() [pagePositions.Length <= iPage]");	
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
			Debug.LogError("UIInvenPage::NextPage() [null == pagePositions]");	
			return false;
		}		
		
		if( pagePositions.Length <= m_iCurPage + 1 )			
		{
			SetPage( 0 );	
		}
		else
		{
			SetPage( m_iCurPage+1 );
		}
		
		return true;
	}
	
	public bool PrePage()
	{			
		if( 0 > m_iCurPage - 1 )			
		{
			SetPage( pagePositions.Length-1 );
		}
		else		
		{
			SetPage( m_iCurPage-1 );
		}
		
		return true;
	}
	
	
	//collider 
	
	public bool IsPageeRectIntersect( Ray ray )
	{
		if( null == collider )
		{
			Debug.LogError("UIInvenPage::IsPageeRectIntersect() [null == collider]");
			return false;
		}		
		
		return AsUtil.PtInCollider( collider, ray, false );		
	}
	
	public bool IsPageNextRectIntersect( Ray ray )
	{
		if( null == nextPageRect )
		{
			Debug.LogError("UIInvenPage::IsPageNextRectIntersect() [null == nextPageRect]");
			return false;
		}	
		
		return AsUtil.PtInCollider( nextPageRect, ray, false );		
	}
	
	public bool IsPagePreRectIntersect( Ray ray )
	{
		if( null == prePageRect )
		{
			Debug.LogError("UIInvenPage::IsPagePreRectIntersect() [null == prePageRect]");
			return false;
		}		
		
		
		return AsUtil.PtInCollider( prePageRect, ray, false );		
	}
	
	public bool IsCurPageExist( int iSlot )
	{
		
		return true;
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
