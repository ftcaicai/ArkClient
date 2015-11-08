using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/9-Grid Dialog")]

public class AsDlgBase : MonoBehaviour
{
	public float width = 10;
	public float height = 10;
	public SimpleSprite left_top = null;
	public SimpleSprite top = null;
	public SimpleSprite right_top = null;
	public SimpleSprite left = null;
	public SimpleSprite center = null;
	public SimpleSprite right = null;
	public SimpleSprite left_bottom = null;
	public SimpleSprite bottom = null;
	public SimpleSprite right_bottom = null;
	public float widthRatio = 0.0f;
	public float heightRatio = 0.0f;

	public float TotalHeight
	{
		get	{ return top.height + center.height + bottom.height; }
	}

	public float TotalWidth
	{
		get	{ return left.width + center.width + right.width; }
	}

	// Use this for initialization
	void Start()
	{
		if( ( 0.0f != widthRatio) || ( 0.0f != heightRatio))
			AssignRatio();
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void AssignRatio()
	{
		float screenWidth = center.RenderCamera.orthographicSize * center.RenderCamera.aspect * 2.0f * widthRatio;
		float screenHeight = center.RenderCamera.orthographicSize * 2.0f * heightRatio;

		width = screenWidth - left.width - right.width;
		height = screenHeight - top.height - bottom.height;

		Assign();
	}

	public virtual void Assign()
	{
		left_top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT;
		top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER;
		right_top.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT;
		left.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		center.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
		right.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		left_bottom.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT;
		bottom.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_CENTER;
		right_bottom.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;

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
		bottom.transform.position = new Vector3( centerPt.x, centerPt.y - ( height * 0.5f), centerPt.z);
		bottom.width = width;
		right_bottom.transform.position = new Vector3( centerPt.x + ( width * 0.5f), centerPt.y - ( height * 0.5f), centerPt.z);

		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		bottom.CalcSize();
	}

	public void SetColor( Color c)
	{
		left_top.gameObject.renderer.material.SetColor( "_Color", c);
		top.gameObject.renderer.material.SetColor( "_Color", c);
		right_top.gameObject.renderer.material.SetColor( "_Color", c);
		left.gameObject.renderer.material.SetColor( "_Color", c);
		center.gameObject.renderer.material.SetColor( "_Color", c);
		right.gameObject.renderer.material.SetColor( "_Color", c);
		left_bottom.gameObject.renderer.material.SetColor( "_Color", c);
		bottom.gameObject.renderer.material.SetColor( "_Color", c);
		right_bottom.gameObject.renderer.material.SetColor( "_Color", c);
	}

	public void SetAlpha( float _alpha)
	{
		left_top.SetColor( new Color( left_top.Color.r, left_top.Color.g, left_top.Color.b, _alpha));
		top.SetColor( new Color( top.Color.r, top.Color.g, top.Color.b, _alpha));
		right_top.SetColor( new Color( right_top.Color.r, right_top.Color.g, right_top.Color.b, _alpha));
		left.SetColor( new Color( left.Color.r, left.Color.g, left.Color.b, _alpha));
		center.SetColor( new Color( center.Color.r, center.Color.g, center.Color.b, _alpha));
		right.SetColor( new Color( right.Color.r, right.Color.g, right.Color.b, _alpha));
		left_bottom.SetColor( new Color( left_bottom.Color.r, left_bottom.Color.g, left_bottom.Color.b, _alpha));
		bottom.SetColor( new Color( bottom.Color.r, bottom.Color.g, bottom.Color.b, _alpha));
		right_bottom.SetColor( new Color( right_bottom.Color.r, right_bottom.Color.g, right_bottom.Color.b, _alpha));
	}
}
