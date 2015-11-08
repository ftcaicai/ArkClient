using UnityEngine;
using System.Collections;

public class AsCenterNotifyItem : MonoBehaviour
{
	public enum eCENTER_NOTIFY
	{
		NONE = 0,
		FADEIN,
		DURATION,
		FADEOUT,
		MAX
	}

	public bool ShowCommand
	{
		get	{ return m_bShowCommand; }
		set	{ m_bShowCommand = value; }
	}
	private bool m_bShowCommand = false;
	public SpriteText m_CenterNotifyText = null;
	public AsDlgBase m_BGDlg = null;
	private Color m_curColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f);
	private eCENTER_NOTIFY m_eState = eCENTER_NOTIFY.NONE;
	private float m_Base_Y;
	private float m_fAlphaIncreaseSpeed = 2.0f; // default: 2.0f
	private float m_fAlphaDecreaseSpeed = 2.0f; // default: 2.0f
	private float m_fStartFadeInTime = 0.0f;
	private float m_fStartDurationTime = 0.0f;
	private float m_fStartFadeOutTime = 0.0f;
	private float m_FadeInTime = 0.1f;
	private float m_DurationTime = 0.1f;
	private float m_FadeOutTime = 0.1f;
	private int m_Line;
	public int Line
	{
		get { return m_Line; }
		set
		{
			m_Line = value;
			Vector3 pos = gameObject.transform.localPosition;
			pos.y = m_Base_Y + m_Line * 1.4f ;
			gameObject.transform.localPosition = pos;
		}
	}

	public float StartFadeInTime
	{
		get	{ return m_fStartFadeInTime; }
		set	{ m_fStartFadeInTime = value; }
	}

	GameObject m_ParentObj;
	public GameObject ParentObject
	{
		get	{ return m_ParentObj; }
		//set { m_ParentObj = value;}
	}
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		switch( m_eState)
		{
		case eCENTER_NOTIFY.FADEIN:
			{
				m_curColor.a += ( Time.deltaTime * m_fAlphaIncreaseSpeed);
				SetAlpha( m_curColor.a);
				if( ( Time.time - m_fStartFadeInTime) > m_FadeInTime)
				{
					m_fStartDurationTime = Time.time;
					m_eState = eCENTER_NOTIFY.DURATION;
				}
			}
			break;
		case eCENTER_NOTIFY.DURATION:
			{
				if( ( Time.time - m_fStartDurationTime) > m_DurationTime)
				{
					m_fStartFadeOutTime = Time.time;
					m_eState = eCENTER_NOTIFY.FADEOUT;
					m_curColor.a = 1.0f;
				}
			}
			break;
		case eCENTER_NOTIFY.FADEOUT:
			{
				if( ( Time.time - m_fStartFadeOutTime) > m_FadeOutTime)
					return;

				m_curColor.a -= ( Time.deltaTime * m_fAlphaDecreaseSpeed);
				SetAlpha( m_curColor.a);
			}
			break;
		}
	}

	public void OnDisable()
	{
		if ( true == m_bShowCommand)
			transform.position = Vector3.zero;
	}

	public void SetText( string text,GameObject obj)
	{
		m_CenterNotifyText.Text = text;
		CalculateWidth( text);

		m_BGDlg.transform.localScale = new Vector3( 1.0f, (float)( m_CenterNotifyText.GetDisplayLineCount()), 1.0f);

		m_ParentObj = obj;

		m_bShowCommand = false;
		gameObject.SetActiveRecursively( m_bShowCommand);
	}

	void SetAlpha( float alpha)
	{
		Color textColor = m_CenterNotifyText.Color;
		textColor.a = alpha;
		m_CenterNotifyText.SetColor( textColor);
		m_BGDlg.SetColor( textColor);
	}

	float CalculateWidth( string str)
	{
		float max_Width = 0.0f;
		m_BGDlg.width = max_Width = m_CenterNotifyText.TotalWidth;
		m_BGDlg.Assign();
		return max_Width;
	}

	public void Show()
	{
		m_FadeInTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 50).Value / 1000.0f;
		m_DurationTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 16).Value / 1000.0f;
		m_FadeOutTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 51).Value / 1000.0f;
//		m_NextWaitTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 52).Value / 1000.0f;

		m_fAlphaIncreaseSpeed = ( 1.0f / m_FadeInTime);
		m_fAlphaDecreaseSpeed = ( 1.0f / m_FadeOutTime);

		m_Base_Y = gameObject.transform.localPosition.y;

		m_bShowCommand = true;
		gameObject.SetActiveRecursively( m_bShowCommand);

		m_curColor.a = 0.0f;
		m_eState = eCENTER_NOTIFY.FADEIN;
		m_fStartFadeInTime = Time.time;
		SetAlpha( m_curColor.a);
	}

	public void Hide()
	{
		m_bShowCommand = false;
		gameObject.SetActiveRecursively( m_bShowCommand);
	}

	public void Remove()
	{
		DestroyImmediate( m_ParentObj);
	}
}
