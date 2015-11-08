using UnityEngine;
using System.Collections;

public class AsSlotPackChanger : MonoBehaviour
{
	public SpriteText topText = null;
	public SpriteText centerText = null;
	public SpriteText bottomText = null;
	public int DRAG_SENSITYVITY = 30;
	public int maxCount = 0;
	public int curIndex = -1;
	private Camera uiCamera = null;
	private float accumDrag = 0.0f;
	private bool isHit = false;
	
	// Use this for initialization
	void Start()
	{
		// < ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( topText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( centerText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( bottomText);
		// ilmeda, 20120822 >
		
		GameObject camObj = GameObject.Find( "UICamera");
		uiCamera = camObj.GetComponent<Camera>();
		
		isHit = false;
		UpdateIndex();
	}
	
	private Vector3 prevPos = Vector3.zero;
	void OnMouseDown()
	{
		prevPos = Input.mousePosition;
	}
	
	void OnMouseDrag()
	{
		accumDrag += ( Input.mousePosition.y - prevPos.y);
		if( 0.0 > accumDrag)
		{
			DownScroll();
			accumDrag = DRAG_SENSITYVITY + accumDrag;
		}
		else if( DRAG_SENSITYVITY < accumDrag)
		{
			UpScroll();
			accumDrag -= DRAG_SENSITYVITY;
		}
		
		prevPos = Input.mousePosition;
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
			isHit = AsUtil.PtInCollider( uiCamera, collider, touch.position);
			break;
		case TouchPhase.Moved:
			{
				if( false == isHit)
					break;
				
				accumDrag += touch.deltaPosition.y;
				if( 0.0 > accumDrag)
				{
					DownScroll();
					accumDrag = DRAG_SENSITYVITY + accumDrag;
				}
				else if( DRAG_SENSITYVITY < accumDrag)
				{
					UpScroll();
					accumDrag -= DRAG_SENSITYVITY;
				}
			}
			break;
		case TouchPhase.Ended:
			isHit = false;
			break;
		}
	}
	
	private void UpScroll()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		curIndex++;
		if( maxCount <= curIndex)
			curIndex = 0;
		
		UpdateIndex();
	}
	
	private void DownScroll()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		curIndex--;
		if( 0 > curIndex)
			curIndex = maxCount - 1;
		
		UpdateIndex();
	}
	
	private void UpdateIndex()
	{
		if( 0 == curIndex)
			topText.Text = maxCount.ToString();
		else
			topText.Text = curIndex.ToString();
		
		centerText.Text = ( curIndex + 1).ToString();
		
		if( curIndex >= maxCount - 1)
			bottomText.Text = "1";
		else
			bottomText.Text = ( curIndex + 2).ToString();
	}
}
