using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/12-Grid Dialog")]

public class AsDlgBaseBubble : AsDlgBase
{ 
	//public float width = 10;
	//public float height = 10;
	public float tailPos = 20;
	//public SimpleSprite left_top = null;
	//public SimpleSprite top = null;
	//public SimpleSprite right_top = null;
	//public SimpleSprite left = null;
	//public SimpleSprite center = null;
	//public SimpleSprite right = null;
	//public SimpleSprite left_bottom = null;
	//public SimpleSprite bottom = null;
	//public SimpleSprite right_bottom = null;

	public SimpleSprite bottom_right = null;
	public SimpleSprite bottom_left = null;
	public SimpleSprite tail = null;

	/*public float TotalHeight
	{
		get	{ return top.height + center.height + bottom.height; }
	}

	public float TotalWidth
	{
		get	{ return left.width + center.width + right.width; }
	}*/

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void Assign()
	{
		left_top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT;
		top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER;
		right_top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT;
		left.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		center.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
		right.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		left_bottom.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT;
		right_bottom.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;

		bottom_right.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_CENTER;
		bottom_left.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;
		tail.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_CENTER;

		Vector3 centerPt = center.transform.position;

		center.width = width;
		center.height = height;

		left_top.transform.position = new Vector3( centerPt.x - ( width * 0.5f), 	centerPt.y + ( height * 0.5f), centerPt.z);
		top.transform.position = new Vector3( centerPt.x, centerPt.y + ( height * 0.5f), centerPt.z);
		top.width = width;
		right_top.transform.position = new Vector3( centerPt.x + ( width * 0.5f), centerPt.y + ( height * 0.5f), centerPt.z);
		left.transform.position = new Vector3( centerPt.x - ( width * 0.5f), centerPt.y, centerPt.z);
		left.height = height;
		right.transform.position = new Vector3( centerPt.x + ( width * 0.5f), centerPt.y, centerPt.z);
		right.height = height;
		left_bottom.transform.position = new Vector3( centerPt.x - ( width * 0.5f), centerPt.y - ( height * 0.5f), centerPt.z);
		//bottom.transform.position = new Vector3( centerPt.x, centerPt.y - ( height * 0.5f), centerPt.z);
		//bottom.width = width;
		right_bottom.transform.position = new Vector3( centerPt.x + ( width * 0.5f), centerPt.y - ( height * 0.5f), centerPt.z);

		tail.transform.position = new Vector3( centerPt.x + tailPos, centerPt.y - ( center.height * 0.5f), centerPt.z);

		float fX = left_bottom.transform.position.x;// + ( left_bottom.width * 0.5f);
		float fX_1 = tail.transform.position.x - ( tail.width * 0.5f);
		bottom_left.transform.position = new Vector3( fX, centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_left.width = fX_1 - fX;//( center.width - tail.width) * 0.5f;

		float fX1 = right_bottom.transform.position.x;// + ( left_bottom.width * 0.5f);
		float fX_2 = tail.transform.position.x + ( tail.width * 0.5f);
		bottom_right.transform.position = new Vector3( (fX_2 + (fX1-fX_2)*0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_right.width = fX1 - fX_2;

		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		//bottom.CalcSize();
		bottom_left.CalcSize();
		bottom_right.CalcSize();
	}
}
