using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsSkillDelegatorManager : MonoBehaviour
{
	public AsSkillDelegator tempDel = null;
	public AsSpriteBlinker coolTimeWanring = null;

	private ArrayList delegatorList = new ArrayList();
	private float actionTime = 0.0f;
	private List<Tbl_Action_Cancel> cancelList = new List<Tbl_Action_Cancel>();

	private static AsSkillDelegatorManager instance = null;
	public static AsSkillDelegatorManager Instance
	{
		get	{ return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	public void Clear()
	{
		foreach( AsSkillDelegator delegator in delegatorList)
		{
			DestroyImmediate( delegator.gameObject);
		}
		delegatorList.Clear();
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		int index = 0;

		foreach( AsSkillDelegator delegator in delegatorList)
		{
			delegator.transform.position = new Vector3( transform.position.x - ( AsSkillDelegator.SIZE * ( ( delegatorList.Count - 1) - index)), transform.position.y, transform.position.z);
			index++;

			delegator.AlignScreenPos();
		}
	}

	public void RemoveDelegator( AsSkillDelegator del)
	{
		if( 0.0f >= del.RemainTime)
			AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_Skill_Charge_Complete( del.Type));

		delegatorList.Remove( del);
		DestroyImmediate( del.gameObject);
	}

	public void AddDelegator( COMMAND_SKILL_TYPE type)
	{
		//AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		//eCLASS _class = userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);

		Tbl_Skill_Record skillRecord = null;
		switch( type)
		{
//		case COMMAND_SKILL_TYPE._DOUBLE_TAP_TERRAIN:
//			skillRecord = AsTableManager.Instance.GetTbl_Skill_RecordByPickingType( _class, eCommandPicking_Type.Terrain);
//			break;
		case COMMAND_SKILL_TYPE._DOUBLE_TAP_PLAYER:
//			skillRecord = AsTableManager.Instance.GetTbl_Skill_RecordByPickingType( _class, eCommandPicking_Type.Self);
			skillRecord = SkillBook.Instance.GetLearnedDoubleTapSkill(eCommandPicking_Type.Self);
			break;
		case COMMAND_SKILL_TYPE._DOUBLE_TAP_MONSTER:
//			skillRecord = AsTableManager.Instance.GetTbl_Skill_RecordByPickingType( _class, eCommandPicking_Type.FingerPoint);
			skillRecord = SkillBook.Instance.GetLearnedDoubleTapSkill(eCommandPicking_Type.FingerPoint);
			break;
//		case COMMAND_SKILL_TYPE._DOUBLE_TAP_OTHERUSER:
		default:
			skillRecord = SkillBook.Instance.GetLearnedCommandSkill((eCommand_Type)( type + 1));
			break;
		}

		if( null == skillRecord)
		{
			Debug.LogError( "null == skillRecord");
			return;
		}

		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skillRecord.Index);
		if( null == skillLevelRecord)
		{
			Debug.LogError( "null == skillLevelRecord");
			return;
		}

		AsSkillDelegator del = GameObject.Instantiate( tempDel) as AsSkillDelegator;
		del.Type = type;
		//del.CoolTime = skillLevelRecord.CoolTime;
		del.SetCoolTime( skillRecord.Index, 1 );

		GameObject icon = Resources.Load( skillRecord.Skill_Icon) as GameObject;
		del.Icon = GameObject.Instantiate( icon) as GameObject;
		Debug.Log( del.Icon);

		AddDelegator( del);

		_SetCommandSkillActionCancelTime( skillLevelRecord); // ilmeda, 20120817
	}

	public void AddDelegator( AsSkillDelegator del)
	{
		// contains check
		foreach( AsSkillDelegator delegator in delegatorList)
		{
			if( delegator.nCoolTimeGroupID == del.nCoolTimeGroupID)
			{
				RemoveDelegator( del);
				return;
			}
		}

		if( 6 <= delegatorList.Count)
		{
			AsSkillDelegator tempDel = delegatorList[0] as AsSkillDelegator;
			RemoveDelegator( tempDel);
		}

		del.transform.parent = transform;
		del.transform.position = transform.position;
		del.manager = this;
		delegatorList.Add( del);
	}

	// < ilmeda, 20120817
	private void _SetCommandSkillActionCancelTime(Tbl_SkillLevel_Record curSkillLevelData)
	{
		if( null == curSkillLevelData)
			return;

		Tbl_Action_Record actionRecord = null;
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		eGENDER gender = userEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		switch( gender)
		{
		case eGENDER.eGENDER_FEMALE:
			actionRecord = AsTableManager.Instance.GetTbl_Action_Record( curSkillLevelData.SkillAction_Index_Female);
			break;
		case eGENDER.eGENDER_MALE:
			actionRecord = AsTableManager.Instance.GetTbl_Action_Record( curSkillLevelData.SkillAction_Index);
			break;
		default:
			Debug.LogError( "Invalid gender");
			break;
		}

		if( null == actionRecord)
			Debug.LogError( "null == actionRecord");

		cancelList. Clear();
		if( ( null != actionRecord.HitAnimation) && ( 0 < actionRecord.HitAnimation.listCancel.Count))
		{
			foreach( Tbl_Action_Cancel cancel in actionRecord.HitAnimation.listCancel)
				cancelList.Add( cancel);
		}
		actionTime = Time.time;
	}
	// ilmeda, 20120817 >

	public bool IsActionCancelTime( float time)
	{
		//$yde
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		float attSpeed = player.GetProperty<float>(eComponentProperty.ATTACK_SPEED);

		foreach( Tbl_Action_Cancel cancel in cancelList)
		{
			if( ( cancel.StartTime / attSpeed < ( time - actionTime)) && ( cancel.EndTime / attSpeed >= ( time - actionTime)))
				return false;
		}

		return true;
	}

	public void AddWarning( COMMAND_SKILL_TYPE type)
	{
		foreach( AsSkillDelegator del in delegatorList)
		{
			if( type == del.Type)
			{
				AsSpriteBlinker blinker = del.gameObject.GetComponentInChildren< AsSpriteBlinker>();
				if( null != blinker)
				{
					if( false == blinker.IsPlaying)
						blinker.Play();

					continue;
				}

				AsSpriteBlinker warn = GameObject.Instantiate( coolTimeWanring) as AsSpriteBlinker;
				warn.transform.parent = del.transform;
				warn.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
				warn.Play();
			}
		}
	}
}
