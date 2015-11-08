using UnityEngine;
using System.Collections;

public class AsSlideButton : MonoBehaviour
{
	public SimpleSprite m_thumb = null;
	public SimpleSprite m_centerPos = null;
	public SimpleSprite m_startPos = null;
	
	public SpriteText   m_leftText = null;
	public SpriteText   m_rightText = null;
	
	private bool 		m_clickThumb = false;
	
	public Camera 		m_uiCamera = null;
	
	private bool 		m_isThumbMove = false;	
	
	public bool ThumbMove
	{
		get	{ return m_isThumbMove;  }
		//set	{ m_isThumbMove = value; }
	}
	
	
	private bool m_ActiveThumbMove = true;	
	
	public bool ActiveThumbMove
	{
		get	{ return m_ActiveThumbMove;  }
		set	{ m_ActiveThumbMove = value; }
	}
	
	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{			
		if(m_uiCamera == null)
			m_uiCamera = AsInputManager.Instance.UICamera;
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_leftText );
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_rightText );
	}
	
	void CheckPostion()
	{
		m_clickThumb = false;
		bool bOldThumbMove = m_isThumbMove;
		
		Vector3 pos = new Vector3();
		if(  m_centerPos.transform.position.x  <= m_thumb.transform.position.x)
		{
			pos.x = m_centerPos.transform.position.x * 2.0f - m_startPos.transform.position.x ;	
				
			//event
			m_isThumbMove = true;
		}
		else
		{
			pos.x = m_startPos.transform.position.x;					
			//event
			m_isThumbMove = false;
		}		
		
		if(bOldThumbMove != m_isThumbMove)
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
				
		m_thumb.transform.position = new Vector3( pos.x, m_centerPos.transform.position.y, m_centerPos.transform.position.z - 1.0f);
	}
	
	void CheckMouseDrag()
	{
		
		if( true == Input.GetMouseButton(0))
		{
			Vector2 touchPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y);
			if( true == AsUtil.PtInCollider( m_uiCamera,  m_thumb.collider, touchPos ) )
				m_clickThumb = true;
			else
				CheckPostion();
			
			if( m_clickThumb ) 
			{
				Vector3 center = m_uiCamera.WorldToScreenPoint( m_centerPos.transform.position);
				Vector3 start = m_uiCamera.WorldToScreenPoint(  m_startPos.transform.position);
				Vector3 screenPos = new Vector3( Input.mousePosition.x, start.y, start.z);
			
				if( start.x > screenPos.x)
					screenPos.x = start.x;
			
				if( ( center.x * 2.0f - start.x) <= screenPos.x)
					screenPos.x = center.x * 2.0f - start.x;				
				
				Vector3 worldPos = m_uiCamera.ScreenToWorldPoint( screenPos);
				m_thumb.transform.position = new Vector3( worldPos.x, m_centerPos.transform.position.y, m_centerPos.transform.position.z - 1.0f);
			}
		}
		else if( true == Input.GetMouseButtonUp(0))
		{
			CheckPostion();
		}
	}
	
	void CheckTouchrag()
	{	
		TouchPhase phase = TouchPhase.Canceled;
		
		if( 0 < Input.touchCount)
			phase = Input.GetTouch(0).phase;

		switch( phase)
		{
		case TouchPhase.Began:
			{
				Vector2 touchPos = Input.GetTouch(0).position;

				if( true == AsUtil.PtInCollider( m_uiCamera , m_thumb.collider, touchPos ) )
					m_clickThumb = true;
				else
					CheckPostion();
			}
			break;
		case TouchPhase.Moved:
			{
				Vector2 touchPos = Input.GetTouch(0).position;
				if( false == AsUtil.PtInCollider( m_uiCamera , m_thumb.collider, touchPos ) )
				{
					CheckPostion();
					break;
				}
			
				m_clickThumb = true;

				Vector3 center = m_uiCamera.WorldToScreenPoint( m_centerPos.transform.position);
				Vector3 start = m_uiCamera.WorldToScreenPoint( m_startPos.transform.position);
				Vector3 screenPos = new Vector3( touchPos.x, start.y, start.z);
			
				if( start.x > screenPos.x)
					screenPos.x = start.x;
			
				if( ( center.x * 2.0f - start.x) <= screenPos.x)
					screenPos.x = center.x * 2.0f - start.x;
			
				Vector3 worldPos = m_uiCamera.ScreenToWorldPoint( screenPos);
				m_thumb.transform.position = new Vector3( worldPos.x, m_centerPos.transform.position.y, m_centerPos.transform.position.z - 1.0f);
			}
			break;
		case TouchPhase.Ended:
			{
				CheckPostion();
			}
			break;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( DeviceType.Desktop == SystemInfo.deviceType)
		{	
			if(m_ActiveThumbMove) CheckMouseDrag();
		}
		else
		{
			if(m_ActiveThumbMove) CheckTouchrag();
		}
	}
	
	public void SetThumbMove(bool bValue)
	{
		m_isThumbMove = bValue;
			
		Vector3 pos = new Vector3();
		if(bValue)
			pos.x = m_centerPos.transform.position.x * 2.0f - m_startPos.transform.position.x ;	
		else
			pos.x = m_startPos.transform.position.x;
					
		m_thumb.transform.position = new Vector3( pos.x, m_centerPos.transform.position.y, m_centerPos.transform.position.z - 1.0f);
	}
	
	public void SetLeftText(string strValue)
	{
		m_leftText.Text = strValue;
	}
	
	public void SetRightText(string strValue)
	{
		m_rightText.Text = strValue;
	}
}

