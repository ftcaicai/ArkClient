using UnityEngine;
using System.Collections;

public class AsSkillNameShout : MonoBehaviour
{
	private const float MINIMAL_WIDTH = 1.6f;
	public SpriteText text = null;
	string m_SkillName = "";
	
	[SerializeField] SimpleSprite left_ = null;
	[SerializeField] SimpleSprite center_ = null;
	[SerializeField] SimpleSprite right_ = null;

	private AsBaseEntity entity = null;
	public AsBaseEntity Owner
	{
		set	{ entity = value; }
	}
	
	public float TotalWidth
	{
		get	{ return left_.width + center_.width + right_.width; }
	}
	
	Transform m_TrnPos = null;
	
	// Use this for initialization
	void Start()
	{
		Transform dummyLeadTop = entity.GetDummyTransform( "DummyLeadTop");
		if(dummyLeadTop != null)
			m_TrnPos = dummyLeadTop;
//		else
//		{
//			m_TrnPos = entity.transform;
//			Debug.LogError("AsSkillNameShout::Start: DummyLeadTop dummy is not found");
//		}
		
		StartCoroutine( RemoveShout());
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		if( null == entity)
		{
			GameObject.DestroyImmediate( gameObject);
			return;
		}
		
//		Vector3 worldPos = entity.ModelObject.transform.position;
		Vector3 worldPos = Vector3.zero;//m_TrnPos.position;
		if( null != m_TrnPos)
			worldPos = m_TrnPos.position;
		else
		{
			if( true == entity.isKeepDummyObj)
			{
				worldPos = entity.transform.position;
				worldPos.y += entity.characterController.height;
			}
		}
//		worldPos.y += entity.characterController.height;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.y += ( text.GetDisplayLineCount() * 1.0f) + entity.skillNameShout_RevisionY;
		vRes.z = 10.0f;
		gameObject.transform.position = vRes;
	}
	
	public void SetText( string msg)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
		text.Text = m_SkillName = msg;
		
		SetSize(msg);
	}
	
	public void SetColor(string _color)
	{
		string str = _color + m_SkillName;
		text.Text = str;
	}
	
	private IEnumerator RemoveShout()
	{
		float sec = AsTableManager.Instance.GetTbl_GlobalWeight_Record(43).Value * 0.001f;
		yield return new WaitForSeconds( sec);
		GameObject.DestroyImmediate( gameObject);
	}	
	
	void SetSize(string _msg)
	{
		left_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		center_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
		right_.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		
		float width = _msg.Length * 0.7f;
		Vector3 centerPt = center_.transform.position;
		
		center_.width = width;
		
		left_.transform.position = new Vector3( centerPt.x - ( width * 0.5f), 	centerPt.y, centerPt.z);
		right_.transform.position = new Vector3( centerPt.x + ( width * 0.5f), 	centerPt.y, centerPt.z);
		
		left_.CalcSize();
		center_.CalcSize();
		right_.CalcSize();
		
		Vector3 leftOffset = left_.transform.position;
		leftOffset.x -= left_.width;
		left_.transform.position = leftOffset;
		
		Vector3 rightOffset = right_.transform.position;
		rightOffset.x += right_.width;
		right_.transform.position = rightOffset;
	}
}
