using UnityEngine;
using System.Collections;

public class AsTargetGradeManager : MonoBehaviour
{
	public GameObject[] gradeFrame = new GameObject[0];
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Display( AsBaseEntity entity)
	{
		gameObject.SetActive(true);
		
		foreach( GameObject obj in gradeFrame)
			obj.SetActiveRecursively( false);
		
		eMonster_Grade eGrade = entity.GetProperty<eMonster_Grade>( eComponentProperty.GRADE);
		switch( eGrade)
		{
		case eMonster_Grade.Normal:
			gradeFrame[0].SetActiveRecursively( true);
			break;
		case eMonster_Grade.Elite:
			gradeFrame[1].SetActiveRecursively( true);
			break;
		case eMonster_Grade.Champion:
			gradeFrame[2].SetActiveRecursively( true);
			break;
		case eMonster_Grade.Boss:
			gradeFrame[3].SetActiveRecursively( true);
			break;
		case eMonster_Grade.DObject:
		case eMonster_Grade.QObject:
			Debug.LogWarning( "Target grade icon is not added: " + eGrade);
			break;
		case eMonster_Grade.Named:
			gradeFrame[4].SetActiveRecursively( true);
			break;
		case eMonster_Grade.Trap:
			break;
		default:
			Debug.LogError( "Invalid target grade : " + eGrade);
			break;
		}
		
//		eMonster_AttackType eAttackType = entity.GetProperty<eMonster_AttackType>( eComponentProperty.MONSTER_ATTACK_TYPE);
//		switch( eAttackType)
//		{
//		case eMonster_AttackType.Fool:
//			break;
//		case eMonster_AttackType.Peaceful:
//			break;
//		case eMonster_AttackType.Offensive:
//			break;
//		default:
//			Debug.LogError( "Invalid target attack type : " + eAttackType);
//		}
	}
}
