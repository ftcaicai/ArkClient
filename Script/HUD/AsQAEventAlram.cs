using UnityEngine;
using System.Collections;

public class AsQAEventAlram : MonoBehaviour
{
	public SpriteText m_TextTitle = null;
	public SpriteText m_TextEvent = null;
	
	void Start()
	{
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.BringIn();

		Invoke( "Dismiss", 6.0f);
	}
	
	void Update()
	{
	}
	
	public void Init( body2_SC_SERVER_EVENT_START data)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6035_EFF_RankingAlarm", Vector3.zero, false);
		
		string strUserPlayer = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>( eComponentProperty.NAME);
		m_TextTitle.Text = string.Format( AsTableManager.Instance.GetTbl_String( 970), strUserPlayer);
		m_TextEvent.Text = "";
		
		switch( data.eEventType)
		{
		case eEVENT_TYPE.eEVENT_TYPE_TIME_EXP: break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_GOLD: break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_DROP: break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_REWARD: break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION: break;
		case eEVENT_TYPE.eEVENT_TYPE_ATTEND_ADD_REWARD: break;
		case eEVENT_TYPE.eEVENT_TYPE_RETURN_ADD_REWARD: break;
			
		case eEVENT_TYPE.eEVENT_TYPE_ITEM_STRENGTHEN: m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 976); break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COLLECT: m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 971); break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_PRODUCT: m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 972); break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MIX: m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 973); break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COMMISSION: m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 974); break;

		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MOB_REGEN: 
			if( (eMonster_Grade)(data.nValue1) ==  eMonster_Grade.Treasure )
				m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 2795); 
			else
				m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 975); 
			break;

		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_CONDITION_DROP:
			m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 2796);
			break;

		case eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE:
			m_TextEvent.Text = AsTableManager.Instance.GetTbl_String( 2794); 
			break;
		}
	}

	private void Dismiss()
	{
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.Dismiss();
		
		Invoke( "Destroy", 1.0f);
	}
	
	private void Destroy()
	{
		GameObject.Destroy( gameObject);
	}

	/*
	public SimpleSprite left_top = null;
	public SimpleSprite top = null;
	public SimpleSprite right_top = null;
	public SimpleSprite left = null;
	public SimpleSprite center = null;
	public SimpleSprite right = null;
	public SimpleSprite left_bottom = null;
	public SimpleSprite bottom = null;
	public SimpleSprite right_bottom = null;
	public SimpleSprite symbol = null;
	public SpriteText text = null;

	IEnumerator Start()
	{
		float wait = 5.0f;
		
		yield return new WaitForSeconds( wait);
		
		GameObject.DestroyImmediate( gameObject);
	}
	
	void Update()
	{
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
		
		center.width = text.TotalWidth;
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
		bottom.transform.position = new Vector3( centerPt.x, centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom.width = center.width;
		right_bottom.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		
		symbol.transform.position = new Vector3( centerPt.x, centerPt.y + ( center.height * 0.5f), centerPt.z);
		
		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		bottom.CalcSize();
	}
	*/
}
