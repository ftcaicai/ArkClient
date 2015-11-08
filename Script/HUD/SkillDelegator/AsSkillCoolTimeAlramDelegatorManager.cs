using UnityEngine;
using System.Collections;

public class AsSkillCoolTimeAlramDelegatorManager : MonoBehaviour
{
	private static AsSkillCoolTimeAlramDelegatorManager instance = null;
	public static AsSkillCoolTimeAlramDelegatorManager Instance { get{ return instance;}}

	public AsSkillCoolTimeAlramDelegator skillCoolTimeAlram = null;
	private AsSkillCoolTimeAlramDelegator m_SkillCoolTimeAlramBuf = null;
	private float m_StartShowTime = 0.0f;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
	}
	
	void Update()
	{
		if( null != m_SkillCoolTimeAlramBuf)
		{
			if( m_StartShowTime + 1.0f < Time.realtimeSinceStartup)
			{
				_DestroyAlram();
			}
		}
	}

	public void AddSkillCoolTimeAlram(COMMAND_SKILL_TYPE type)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SkillCoolAlram))
			return;
		
		if( COMMAND_SKILL_TYPE._NONE == type)
			return;
		
		_DestroyAlram();
		
		m_StartShowTime = Time.realtimeSinceStartup;

		Tbl_Skill_Record skillRecord = null;
		
		switch( type)
		{
		case COMMAND_SKILL_TYPE._DOUBLE_TAP_PLAYER:
			skillRecord = SkillBook.Instance.GetLearnedDoubleTapSkill(eCommandPicking_Type.Self);
			break;
		case COMMAND_SKILL_TYPE._DOUBLE_TAP_MONSTER:
			skillRecord = SkillBook.Instance.GetLearnedDoubleTapSkill(eCommandPicking_Type.FingerPoint);
			break;
		default:
			skillRecord = SkillBook.Instance.GetLearnedCommandSkill((eCommand_Type)( type + 1));
			break;
		}

		if( null == skillRecord)
		{
			Debug.LogError( "null == skillRecord");
			return;
		}

//		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skillRecord.Index);
//		if( null == skillLevelRecord)
//		{
//			Debug.LogError( "null == skillLevelRecord");
//			return;
//		}

		AsSkillCoolTimeAlramDelegator alram = GameObject.Instantiate( skillCoolTimeAlram) as AsSkillCoolTimeAlramDelegator;
		GameObject icon = Resources.Load( skillRecord.Skill_Icon) as GameObject;
		alram.Icon = GameObject.Instantiate( icon) as GameObject;

		CoolTimeGroup coolTimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( skillRecord.Index, 1);
		
		float fRemainCoolTime = 0.0f;
		
		if( null != coolTimeGroup)
			fRemainCoolTime = coolTimeGroup.getRemainTime;

		alram.textCoolTime.Text = AsMath.GetCoolTimeRemainTime( fRemainCoolTime);

		alram.transform.parent = transform;
		alram.transform.position = transform.position;
		
		m_SkillCoolTimeAlramBuf = alram;
		
		AsEmotionManager.Instance.SkillCoolTimeIconGenerated();//$yde
	}

	public void AddSkillCoolTimeAlram(string strIconPath, float fRemainTime)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SkillCoolAlram))
			return;

		_DestroyAlram();
		
		m_StartShowTime = Time.realtimeSinceStartup;

		AsSkillCoolTimeAlramDelegator alram = GameObject.Instantiate( skillCoolTimeAlram) as AsSkillCoolTimeAlramDelegator;
		
		GameObject obj = Resources.Load( strIconPath) as GameObject;
		if( null == obj)
		{
			Debug.LogError( "AddSkillCoolTimeAlram(): fail to load icon: " +  strIconPath);
			return;
		}
		
		alram.Icon = GameObject.Instantiate( obj) as GameObject;
		
		alram.textCoolTime.Text = AsMath.GetCoolTimeRemainTime( fRemainTime);
		alram.transform.parent = transform;
		alram.transform.position = transform.position;
		
		m_SkillCoolTimeAlramBuf = alram;
		
		AsEmotionManager.Instance.SkillCoolTimeIconGenerated();//$yde
	}
	
	public void AddSkillCoolTimeAlram(int nSkillID, float fRemainTime)
	{
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( nSkillID);
		
		if( null == record)
		{
			Debug.LogError( "AddSkillCoolTimeAlram(): null == record, skill id : " + nSkillID);
			return;
		}
		
		AddSkillCoolTimeAlram( record.Skill_Icon, fRemainTime);
	}
	
	private void _DestroyAlram()
	{
		if( null != m_SkillCoolTimeAlramBuf)
		{
			DestroyImmediate( m_SkillCoolTimeAlramBuf.gameObject);
			m_SkillCoolTimeAlramBuf = null;
		}
	}
	
	public void DestroyAlarm()//$yde
	{
		_DestroyAlram();
	}
}
