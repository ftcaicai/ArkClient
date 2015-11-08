using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class AsLevelUpMsg
{
	[SerializeField] public SpriteText m_Text = null;
	[SerializeField] public AsDlgBase m_BGDlg = null;
}

public class AsLevelUpNotifyItem : MonoBehaviour
{
	public enum eLEVELUP_NOTIFY
	{
		NONE = 0,
		SLIDE,
		DURATION,
		FADEOUT,
		MAX
	}

	public Color m_SkillNameColor;
	public Color m_WayPointNameColor;

	public AsLevelUpMsg[] m_LevelUpNotifyList = null;
	private Color m_curColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f);

	private eLEVELUP_NOTIFY m_eState = eLEVELUP_NOTIFY.NONE;

	private float m_fAlphaDecreaseSpeed = 2.0f; // default: 2.0f
	private float m_fMoveSpeedX = 5.0f; // default: 5.0f

	private float m_fStartSlideInTime = 0.0f;
	private float m_fStartDurationTime = 0.0f;
	private float m_fStartFadeOutTime = 0.0f;

	private float m_SlideInTime = 0.1f;
	private float m_DurationTime = 0.1f;
	private float m_FadeOutTime = 0.1f;

	GameObject m_ParentObj;
	public GameObject ParentObject
	{
		get { return m_ParentObj; }
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
		case eLEVELUP_NOTIFY.SLIDE:
			{
				//Slide
				Vector3 tmpPos	= transform.position;
				tmpPos.x = tmpPos.x - ( m_fMoveSpeedX * Time.deltaTime);
				transform.position = tmpPos;

				if( ( Time.time - m_fStartSlideInTime) > m_SlideInTime)
				{
					m_fStartDurationTime = Time.time;
					m_eState = eLEVELUP_NOTIFY.DURATION;
				}
			}
			break;
		case eLEVELUP_NOTIFY.DURATION:
			{
				if( ( Time.time - m_fStartDurationTime) > m_DurationTime)
				{
					m_fStartFadeOutTime = Time.time;
					m_eState = eLEVELUP_NOTIFY.FADEOUT;
				}
			}
			break;
		case eLEVELUP_NOTIFY.FADEOUT:
			{
				if( ( Time.time - m_fStartFadeOutTime) > m_FadeOutTime)
				{
					Remove();
					return;
				}

				m_curColor.a -= ( Time.deltaTime * m_fAlphaDecreaseSpeed);
				SetColor( m_curColor);
			}
			break;
		}
	}

	void SetText( string[] textList, GameObject parentObj)
	{
		//skill:hp 리사이런스 학습 가능!.
		//waypoint:달빛 광산 공간 이동의 돌 활성화!.
		m_ParentObj = parentObj;
		foreach( AsLevelUpMsg data in m_LevelUpNotifyList)
		{
			data.m_Text.Text = "";
			data.m_BGDlg.gameObject.SetActiveRecursively( false);
		}

		int index = 0;
		foreach( string str in textList)
		{
			if( ( m_LevelUpNotifyList.Length - 1) < index)
				break;

			string[] vals = str.Split( ':');
			if( vals[0].CompareTo( "skill") == 0)
				m_LevelUpNotifyList[index].m_Text.SetColor( m_SkillNameColor);

			if( vals[0].CompareTo( "waypoint") == 0)
				m_LevelUpNotifyList[index].m_Text.SetColor( m_WayPointNameColor);

			m_LevelUpNotifyList[index].m_Text.Text = vals[1];
			m_LevelUpNotifyList[index].m_BGDlg.gameObject.SetActiveRecursively( true);
			index++;
		}

		Show();
	}

	public void SetText( List<string> textList, GameObject parentObj)
	{
		//skill:hp 리사이런스 학습 가능!.
		//waypoint:달빛 광산 공간 이동의 돌 활성화!.
		m_ParentObj = parentObj;
		foreach( AsLevelUpMsg data in m_LevelUpNotifyList)
		{
			data.m_Text.gameObject.SetActiveRecursively( false);
			data.m_BGDlg.gameObject.SetActiveRecursively( false);
		}

		int index = 0;
		foreach( string str in textList)
		{
			if( ( m_LevelUpNotifyList.Length - 1) < index)
				break;

			string[] vals = str.Split( ':');
			if( vals[0].CompareTo( "skill") == 0)
				m_LevelUpNotifyList[index].m_Text.SetColor( m_SkillNameColor);
			if( vals[0].CompareTo( "waypoint") == 0)
				m_LevelUpNotifyList[index].m_Text.SetColor( m_WayPointNameColor);

			m_LevelUpNotifyList[index].m_Text.Text = vals[1];
			m_LevelUpNotifyList[index].m_Text.gameObject.SetActiveRecursively( true);
			m_LevelUpNotifyList[index].m_BGDlg.gameObject.SetActiveRecursively( true);
			index++;
		}

		Show();
	}

	void SetColor( Color color)
	{
		foreach( AsLevelUpMsg data in m_LevelUpNotifyList)
		{
			Color textColor = data.m_Text.Color;
			textColor.a = color.a;
			data.m_Text.SetColor( textColor);
			data.m_BGDlg.SetColor( color);
		}
	}

	float CalculateWidth()
	{
		float width = 0.0f, max_Width = 0.0f;
		foreach( AsLevelUpMsg data in m_LevelUpNotifyList)
		{
			width = data.m_Text.GetWidth( data.m_Text.Text);
			if( max_Width < width)
				max_Width = width;

			data.m_BGDlg.width = width;
			data.m_BGDlg.Assign();
			Vector3 pos = data.m_BGDlg.transform.localPosition;
			pos.x = -( width / 2);
			data.m_BGDlg.transform.localPosition = pos;
		}

		return max_Width;
	}

	public void Show()
	{
		m_SlideInTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 53).Value / 1000.0f ;
		m_DurationTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 54).Value / 1000.0f;
		m_FadeOutTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 55).Value / 1000.0f;
		m_fAlphaDecreaseSpeed = ( 1.0f / m_FadeOutTime);

		m_eState = eLEVELUP_NOTIFY.SLIDE;
		m_fStartSlideInTime = Time.time;

		float maxWidth = CalculateWidth();
		m_fMoveSpeedX = maxWidth / m_SlideInTime;


		Vector3 tmpPos	= transform.position;
		tmpPos.x = tmpPos.x + maxWidth;
		transform.position = tmpPos;
	}

	public void Remove()
	{
		if( m_ParentObj != null)
			Destroy( m_ParentObj);
	}
}
