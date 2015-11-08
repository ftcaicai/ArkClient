using UnityEngine;
using System.Collections;

public class AsButton : AsCtrl
{
	public AsFrameworkBase receiver = null;
	public Texture normalTexture = null;
	public Texture overTexture = null;
	public Texture pressTexture = null;
	public bool fade = false;
	private bool isClicked = false;
	private float prevClickedTime = 0.0f;
	public Camera uiCamera = null;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		TouchPhase phase = TouchPhase.Canceled;

		if( 0 < Input.touchCount)
			phase = Input.GetTouch( 0 ).phase;

		switch( phase )
		{
		case TouchPhase.Began:
			{
				if( false == AsUtil.PtInCollider( uiCamera, collider, Input.GetTouch( 0 ).position ) )
					return;

				SetPress();
				isClicked = true;
			}
			break;
		case TouchPhase.Moved:
			{
				if( false == AsUtil.PtInCollider( uiCamera, collider, Input.GetTouch( 0 ).position ) )
				{
					SetNormal();
				}
				else
				{
					if( true == isClicked )
						SetPress();
				}
			}
			break;
		case TouchPhase.Ended:
			{
				if( ( true == isClicked ) && ( true == AsUtil.PtInCollider( uiCamera, collider, Input.GetTouch( 0 ).position ) ) )
				{
					SetNormal();

					if( true == fade )
					{
						Color currentColor = renderer.sharedMaterial.GetColor( "_Color" );
						if( 1.0f != currentColor.a )
							return;
					}

					float curClickedTime = Time.time;
					if( 0.4f > ( curClickedTime - prevClickedTime ) )
					{
						NOTIFY_MSG msg = new NOTIFY_MSG();
						msg.id = this.id;
						msg.type = NOTIFY_MSG_TYPE.BM_DBL_CLICKED;
						SendMessageEx( msg );
					}
					else
					{
						NOTIFY_MSG msg = new NOTIFY_MSG();
						msg.id = this.id;
						msg.type = NOTIFY_MSG_TYPE.BM_CLICKED;
						SendMessageEx( msg );
					}

					prevClickedTime = curClickedTime;
				}

				isClicked = false;
			}
			break;
		}
	}

	void OnMouseDown()
	{
		SetPress();
		isClicked = true;
	}

	void OnMouseOver()
	{
		SetOver();
	}

	void OnMouseEnter()
	{
		if( false == isClicked)
			return;

		SetPress();
	}

	void OnMouseExit()
	{
		SetNormal();
	}

	void OnMouseUpAsButton()
	{
		SetNormal();

		isClicked = false;

		if( true == fade)
		{
			Color currentColor = renderer.sharedMaterial.GetColor( "_Color" );
			if( 1.0f != currentColor.a )
				return;
		}

		float curClickedTime = Time.time;
		if( 0.4f > ( curClickedTime - prevClickedTime))
		{
			NOTIFY_MSG msg = new NOTIFY_MSG();
			msg.id = this.id;
			msg.type = NOTIFY_MSG_TYPE.BM_DBL_CLICKED;
			SendMessageEx( msg );
		}
		else
		{
			NOTIFY_MSG msg = new NOTIFY_MSG();
			msg.id = this.id;
			msg.type = NOTIFY_MSG_TYPE.BM_CLICKED;
			SendMessageEx( msg );
		}

		prevClickedTime = curClickedTime;
	}

	private void SetNormal()
	{
		if( null == normalTexture)
			return;

		renderer.material.mainTexture = normalTexture;
	}

	private void SetOver()
	{
		if( true == isClicked)
			return;

		if( null == overTexture)
			return;

		renderer.material.mainTexture = overTexture;
	}

	private void SetPress()
	{
		if( null == pressTexture)
			return;

		renderer.material.mainTexture = pressTexture;
	}

	private void SendMessageEx( NOTIFY_MSG msg)
	{
		if( null == receiver)
			return;

		receiver.SendMessage( "OnNotify", msg);
	}
}
