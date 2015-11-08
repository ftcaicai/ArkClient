using UnityEngine;
using System.Collections;

public class AsDlgBalloonMsgBox : MonoBehaviour
{
	public enum TailType
	{
		LEFT,
		CENTER,
		RIGHT,
	};

	protected const float MINIMAL_WIDTH = 3.0f;
	public TailType tailType = TailType.CENTER;
	public GameObject frame;
	public float frameRotation = 0.0f;
	public float visibleTime = 0.0f;
	public bool useTimer = false;
	public SpriteText spriteText;
	public SimpleSprite left_top = null;
	public SimpleSprite top = null;
	public SimpleSprite right_top = null;
	public SimpleSprite left = null;
	public SimpleSprite center = null;
	public SimpleSprite right = null;
	public SimpleSprite left_bottom = null;
	public SimpleSprite bottom_left = null;
	public SimpleSprite tail = null;
	public SimpleSprite bottom_right = null;
	public SimpleSprite right_bottom = null;

	private float remainTime = 0.0f;
	private bool visible = false;
	public bool Visible
	{
		set
		{
			visible = value;
			gameObject.SetActiveRecursively( value);

			if( useTimer == true)
			{
				passedTime = 0.0f;
				remainTime = visibleTime;
			}
		}
		get { return visible; }
	}

	private float passedTime = 0.0f;

	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( visible == false || useTimer == false)
			return;

		passedTime += Time.deltaTime;

		if( passedTime >= 1.0f)
		{
			remainTime -= 1.0f;
	
			if( remainTime <= 0.0f)
				GameObject.DestroyImmediate( gameObject);

			passedTime = 0.0f;
		}
	}
	
	public void SetText( string _msg, string _withoutColorTagMsg)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( spriteText);
		spriteText.Text = _withoutColorTagMsg;
		UpdateShape();
		spriteText.Text = _msg;
		
		Visible = true;
	}
	
	public virtual void UpdateShape()
	{
		Vector3 centerPt = new Vector3( spriteText.transform.position.x, spriteText.transform.position.y, 0.0f);

		// center
		center.transform.position = centerPt;
		center.width = ( MINIMAL_WIDTH > spriteText.TotalWidth) ? MINIMAL_WIDTH : spriteText.TotalWidth;
		center.height = spriteText.lineSpacing * spriteText.GetDisplayLineCount();

		// left
		left.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y, centerPt.z);
		left.height = center.height;
		left_bottom.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);

		// right
		right.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y, centerPt.z);
		right.height = center.height;
		right_bottom.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);

		// top
		left_top.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.transform.position = new Vector3( centerPt.x, centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.width = center.width;
		right_top.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y + ( center.height * 0.5f), centerPt.z);

		// tail left/right size
		bottom_left.width = bottom_right.width = ( center.width - tail.width) * 0.5f;

		// bottom
		switch( tailType)
		{
		case TailType.CENTER:
			tail.transform.position = new Vector3( centerPt.x, centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_left.transform.position = new Vector3( tail.transform.position.x - ( tail.width * 0.5f) - ( bottom_left.width * 0.5f) , centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_right.transform.position = new Vector3( tail.transform.position.x + ( tail.width * 0.5f) + ( bottom_right.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
			break;
		case TailType.LEFT:
			tail.transform.position = new Vector3( left_bottom.transform.position.x + tail.width * 0.5f, centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_left.transform.position = new Vector3( tail.transform.position.x + ( bottom_left.width * 0.5f) + tail.width * 0.5f, centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_right.transform.position = new Vector3( bottom_left.transform.position.x + ( bottom_right.width), centerPt.y - ( center.height * 0.5f), centerPt.z);
			break;
		case TailType.RIGHT:
			tail.transform.position = new Vector3( right_bottom.transform.position.x - tail.width * 0.5f, centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_left.transform.position = new Vector3( tail.transform.position.x - ( bottom_left.width * 0.5f) - tail.width * 0.5f, centerPt.y - ( center.height * 0.5f), centerPt.z);
			bottom_right.transform.position = new Vector3( bottom_left.transform.position.x - ( bottom_right.width), centerPt.y - ( center.height * 0.5f), centerPt.z);
			break;
		}

		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		bottom_left.CalcSize();
		bottom_right.CalcSize();

		if( frame != null)
			frame.transform.rotation = Quaternion.Euler( new Vector3( 0.0f, 0.0f, frameRotation));
		
        //Vector3 vOffset = transform.position - tail.transform.position;
		
        //transform.position += new Vector3( vOffset.x, vOffset.y, 0.0f);

        transform.position += new Vector3((center.width - tail.width * 0.5f) * -0.5f, 0.0f, 0.0f);

        transform.Translate(0.0f, 0.0f, 5.0f);
	}
}
