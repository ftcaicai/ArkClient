using UnityEngine;
using System.Collections;

public class AsNameBackImg : MonoBehaviour 
{
	private const float MINIMAL_WIDTH = 1.6f;
	
	[SerializeField] SimpleSprite left_ = null;
	[SerializeField] SimpleSprite center_ = null;
	[SerializeField] SimpleSprite right_ = null;


	public float LeftFrameWidth  { get { return left_.width; } }
	public float RightFrameWidth { get { return right_.width; } }
	
	
	// Use this for initialization
	void Start()
	{
		
	}
	
	public void SetText( SpriteText _text )
	{		
		left_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		center_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
		right_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		
		float width = _text.TotalWidth;
		Vector3 centerPt = center_.transform.localPosition;
		
		center_.width = width;
		
		left_.transform.localPosition = new Vector3( centerPt.x - ( width * 0.5f), 	centerPt.y, centerPt.z);
		right_.transform.localPosition = new Vector3( centerPt.x + ( width * 0.5f), 	centerPt.y, centerPt.z);
		
		left_.CalcSize();
		center_.CalcSize();
		right_.CalcSize();
	}
}
