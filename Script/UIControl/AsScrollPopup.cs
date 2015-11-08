using UnityEngine;
using System.Collections;

public class AsScrollPopup : MonoBehaviour {
	
	public SpriteText m_topText = null;
	public SpriteText m_centerText = null;
	public SpriteText m_bottomText = null;	
	
	public const int DRAG_SENSITYVITY = 30;	
	//public int m_maxCount = 1000;
	public int m_curIndex = 0;
	private float m_accumDrag = 0.0f;
	private bool m_isHit = false;
	
	public Camera m_uiCamera = null;
	
	private ArrayList m_dataList = new ArrayList();	
	
	
// Use this for initialization
	void Start()
	{
	
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_topText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_centerText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_bottomText);
	
		if(m_uiCamera == null)
			m_uiCamera = AsInputManager.Instance.UICamera;
		
		m_isHit = false;
		UpdateIndex();
	}
	
	private Vector3 prevPos = Vector3.zero;
	void OnMouseDown()
	{
		prevPos = Input.mousePosition;
	}
	
	void OnMouseDrag()
	{	
		Vector2 touchPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
		m_isHit = AsUtil.PtInCollider( m_uiCamera, collider, touchPos );
		if(m_isHit)
		{
			m_accumDrag += ( Input.mousePosition.y - prevPos.y);
			if( 0.0 > m_accumDrag)
			{
				DownScroll();
				m_accumDrag = DRAG_SENSITYVITY + m_accumDrag;
			}
			else if( DRAG_SENSITYVITY < m_accumDrag)
			{
				UpScroll();
				m_accumDrag -= DRAG_SENSITYVITY;
			}
			
			prevPos = Input.mousePosition;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if( 0 == Input.touchCount)
			return;
		
		Touch touch = Input.GetTouch(0);
		switch( touch.phase)
		{
		case TouchPhase.Began:		
			m_isHit = AsUtil.PtInCollider( m_uiCamera, collider, touch.position);
			break;
		case TouchPhase.Moved:
			{
				if( false == m_isHit)
					break;
				
				m_accumDrag += touch.deltaPosition.y;
				if( 0.0 > m_accumDrag)
				{
					DownScroll();
					m_accumDrag = DRAG_SENSITYVITY + m_accumDrag;
				}
				else if( DRAG_SENSITYVITY < m_accumDrag)
				{
					UpScroll();
					m_accumDrag -= DRAG_SENSITYVITY;
				}
			}
			break;
		case TouchPhase.Ended:
			m_isHit = false;
			break;
		}
	}
	
	private void UpScroll()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		m_curIndex++;
		if(  m_dataList.Count <= m_curIndex)
			m_curIndex = 0;
		
		UpdateIndex();
	}
	
	private void DownScroll()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		m_curIndex--;
		if( 0 > m_curIndex)
			m_curIndex = m_dataList.Count-1;
		
		UpdateIndex();
	}
	
	private void UpdateIndex()
	{
		if(m_dataList.Count <= 0 ) return;
		
		int maxCount =  m_dataList.Count-1;
		if(m_curIndex == 0)
		{
			m_topText.Text =  m_dataList[maxCount].ToString();	
		}
		else
		{
			m_topText.Text =  m_dataList[m_curIndex-1].ToString();	
		}
		
		m_centerText.Text =  m_dataList[m_curIndex].ToString();	
		
		
		if(  maxCount <= m_curIndex)
		{
			m_bottomText.Text =  m_dataList[0].ToString();	
		}
		else
		{
			m_bottomText.Text =  m_dataList[m_curIndex+1].ToString();	
		}
		
	
		
	}
	
	
	public void InsertData( string strText )
	{
		m_dataList.Add( strText );
	}
	
	public void SetSelectedByIndex(int index)
	{
		if(index < 0 )
		{
			Debug.LogError("AsScrollPopup::SetSelectedByIndex() Err!!! Index:" + index.ToString() );
			return;
		}
		m_curIndex = index;
		UpdateIndex();
	}
}

