using UnityEngine;
using System.Collections;

public enum eBalloonOwnerType : int
{
	Invalid = -1,
	
	Player,
	Monster,
	
	Max
};

public class AsChatBalloonBase : MonoBehaviour
{
	private const float MINIMAL_WIDTH = 1.6f;
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
	public SpriteText text = null;
	private eBalloonOwnerType ownerType = eBalloonOwnerType.Player;
	public eBalloonOwnerType OwnerType	{ set { ownerType = value; } }
	public bool isNeedDelete = true;
	
	private Transform origin = null;
	
	private AsBaseEntity entity = null;
	public AsBaseEntity Owner
	{
		set
		{
			entity = value;
			
			if( null != entity)
				origin = entity.GetDummyTransform( "DummyLeadTop");
		}
	}
	
	// Use this for initialization
	IEnumerator Start()
	{
		float wait = ( eBalloonOwnerType.Player == ownerType) ? 4.0f : 1.5f;
		m_BeginTime = Time.time;
		
		yield return new WaitForSeconds( wait);
		
		if( true == isNeedDelete )
			GameObject.DestroyImmediate( gameObject);
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		/*
		if( ( null == entity) || ( null == origin))
		{
			GameObject.DestroyImmediate( gameObject);
			Debug.Log("AsChatBalloonBase::Update: entity = " + entity);
			Debug.Log("AsChatBalloonBase::Update: origin = " + origin);
			return;
		}
		*/
		if( null == entity)
		{
			GameObject.DestroyImmediate( gameObject);
			Debug.Log("AsChatBalloonBase::Update: entity = " + entity);
			return;
		}
		
		Vector3 worldPos = Vector3.zero;
		
		if( null == origin)
		{
			if( true == entity.isKeepDummyObj)
			{
				worldPos = entity.transform.position;
				worldPos.y += entity.characterController.height;
			}
			else
			{
				GameObject.DestroyImmediate( gameObject);
				Debug.Log("AsChatBalloonBase::Update: origin = " + origin);
				return;
			}
		}
		else
			worldPos = origin.position;
		
		//Vector3 worldPos = origin.position;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		if( eBalloonOwnerType.Monster == ownerType)
			vRes.y += ( center.height + 2.5f);
		else if( false == isNeedDelete )
		{
			vRes.y += ( center.height * 0.5f) + 2f;
		}
		else
		{
			vRes.y += ( center.height * 0.5f);
		}
		vRes.z = 10.0f;
		gameObject.transform.position = vRes;
	}
	
	float m_BeginTime;
	void OnDestroy()
	{
//		Debug.Log("AsChatBalloonBase::OnDestroy: elapsed time = " + (Time.time - m_BeginTime));
	}
	
	public void SetText( string msg)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
		text.Text = msg;

		UpdateShape();
	}
	
	private void UpdateShape()
	{
		Vector3 centerPt = center.transform.position;
		
		center.width = ( MINIMAL_WIDTH > text.TotalWidth) ? MINIMAL_WIDTH : text.TotalWidth;
		center.height = text.lineSpacing * text.GetDisplayLineCount();
		
		left_top.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.transform.position = new Vector3( centerPt.x, centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.width = center.width;
		right_top.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y + ( center.height * 0.5f), centerPt.z);
		left.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y, centerPt.z);
		left.height = center.height;
		right.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y, centerPt.z);
		right.height = center.height;
		left_bottom.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);

		tail.transform.position = new Vector3( centerPt.x, centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_left.transform.position = new Vector3( tail.transform.position.x - ( tail.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_left.width = ( center.width - tail.width) * 0.5f;
		bottom_right.transform.position = new Vector3( tail.transform.position.x + ( tail.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_right.width = ( center.width - tail.width) * 0.5f;
		
		right_bottom.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		
		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		bottom_left.CalcSize();
		bottom_right.CalcSize();
	}
}
