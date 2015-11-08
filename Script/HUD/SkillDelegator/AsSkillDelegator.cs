using UnityEngine;
using System.Collections;

public class AsSkillDelegator : MonoBehaviour
{
	public static float SIZE = 3.2f;
	//private float coolTime = 0.0f;
	//private float remain = 0.0f;
	private COMMAND_SKILL_TYPE type;
	private GameObject icon = null;
	private CoolTimeGroup m_coolTimeGroup;

	public AsIconCooltime screen = null;
	public SpriteText txt = null;
	[HideInInspector]
	public AsSkillDelegatorManager manager = null;
	
	private bool m_isCoolStart = false;
	
	public GameObject Icon
	{
		get { return icon; }
		set
		{
			icon = value;
			icon.transform.parent = transform;
			icon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.1f);
			icon.transform.localRotation = Quaternion.identity;
			icon.transform.localScale = Vector3.one;
		}
	}

	public int nCoolTimeGroupID
	{
		get
		{
			return m_coolTimeGroup.getCoolTimeGroupID;
		}
	}
	
	/*public float CoolTime
	{
		get	{ return coolTime; }
		set
		{
			coolTime = value * 0.001f;
			remain = coolTime;
		}
	}*/
	
	
	public void SetCoolTime( int iSkill, int iSkillLevel )
	{		
		txt.Text = string.Empty;
		m_coolTimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( iSkill, iSkillLevel );		
		m_isCoolStart = true;
	}
	
	
	public float RemainTime
	{
		get 
		{ 
			if( null == m_coolTimeGroup)
				return 0f;
			return m_coolTimeGroup.getRemainTime; 
		}
	}
	
	public COMMAND_SKILL_TYPE Type
	{
		get { return type; }
		set { type = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		// ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt, true);
	}
	
	// Update is called once per frame
	void Update()
	{
		
		if( null != m_coolTimeGroup )
		{
			if( true == m_coolTimeGroup.isCoolTimeActive )
			{
				m_isCoolStart = true;
				screen.Enable = true;
				screen.Value = m_coolTimeGroup.getCoolTimeValue; 	
				txt.Text = AsMath.GetCoolTimeRemainTime( m_coolTimeGroup.getRemainTime );
		
				/*if( 1.0f > m_coolTimeGroup.getRemainTime )
					txt.Text = string.Format( "{0:F1}", m_coolTimeGroup.getRemainTime);
				else
					txt.Text = string.Format( "{0:D}", (int)m_coolTimeGroup.getRemainTime);*/
			}
			else
			{
				if( true == m_isCoolStart)
				{
					m_isCoolStart = false;
					screen.Enable = false;
					manager.SendMessage( "RemoveDelegator", this);
					
				}
			}			
		}
		else
		{
			if( true == m_isCoolStart)
			{
				m_isCoolStart = false;
				screen.Enable = false;
				manager.SendMessage( "RemoveDelegator", this);
			}
		}
		
		/*if( 0.0f < remain )
		{
			screen.Enable = true;

			remain -= Time.deltaTime;
			if( 0.0f > remain)
				remain = 0.0f;
			
			screen.Value = ( coolTime - remain ) / coolTime;
			
			if( 1.0f > remain)
				txt.Text = string.Format( "{0:F1}", remain);
			else
				txt.Text = string.Format( "{0:D}", (int)remain);
		}
		else
		{
			screen.Enable = false;
			manager.SendMessage( "RemoveDelegator", this);
		}*/
	}
	
	public void AlignScreenPos()
	{
		screen.AlignScreenPos();
	}
}
