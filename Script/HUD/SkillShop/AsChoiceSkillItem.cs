using UnityEngine;
using System.Collections;

public class AsChoiceSkillItem : MonoBehaviour 
{
	public AsSlot leftSlot;
	public AsSlot rightSlot;
	public SimpleSprite disable = null;
	public AsSkillTooltip toolTip = null;
	public SpriteText info = null;
	[HideInInspector]public int price = 0;
	public Color defaultColor = Color.black;
	public Color deficientColor = Color.red;
	public Color goldColor = Color.yellow;
	public Color infoColor = Color.cyan;
	private Tbl_SkillBook_Record skillBookRecord = null;
	public Tbl_SkillBook_Record SkillBookRecord
	{
		set	{ skillBookRecord = value; }
	}

	private bool isUsable = false;
	public bool IsUsable
	{
		get	{ return isUsable; }
		set	{ isUsable = value; }
	}
	
	void OnEnable()
	{
		disable.gameObject.SetActiveRecursively( !isUsable);
	}
	
	// Use this for initialization
	void Start () 
	{
		disable.gameObject.SetActiveRecursively( !isUsable);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	public void PromptTooltip()
	{
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_SKILL_SHOP_LIST_0));
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( true == AsGameMain.isPopupExist)
			return;
		
		float screenWidth = info.RenderCamera.orthographicSize * info.RenderCamera.aspect * 2.0f;

		//	LeftSkill
		AsSkillTooltip tip1 = GameObject.Instantiate( toolTip) as AsSkillTooltip;
		AsDlgBase dlgBase1 = tip1.gameObject.GetComponentInChildren<AsDlgBase>();
		tip1.transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.05f) - ( dlgBase1.TotalWidth * 1.5f), 0.0f, -10.0f);
		tip1.ID = leftSlot.getSkillID;
		tip1.Level = leftSlot.getSkillLevel;

		Tbl_Skill_Record skillRecord1 = AsTableManager.Instance.GetTbl_Skill_Record( leftSlot.getSkillID);
		if( null == skillRecord1)
		{
			Debug.LogError( "null == skillRecord1");
			return;
		}
		
		Tbl_SkillLevel_Record skillLevelRecord1 = AsTableManager.Instance.GetTbl_SkillLevel_Record( leftSlot.getSkillLevel, leftSlot.getSkillID);
		if( null == skillLevelRecord1)
		{
			Debug.LogError( "null == skillLevelRecord1");
			return;
		}
		
		if( true == AsUserInfo.Instance.resettedSkills.ContainsKey( skillBookRecord.Index))
			price = 0;
		
		tip1.Init( skillRecord1, skillLevelRecord1, price);
		
		//	RightSkill
		AsSkillTooltip tip2 = GameObject.Instantiate( toolTip) as AsSkillTooltip;
		AsDlgBase dlgBase2 = tip2.gameObject.GetComponentInChildren<AsDlgBase>();
		tip2.transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.05f) - ( dlgBase2.TotalWidth * 0.5f), 0.0f, -10.0f);
		tip2.ID = rightSlot.getSkillID;
		tip2.Level = rightSlot.getSkillLevel;

		Tbl_Skill_Record skillRecord2 = AsTableManager.Instance.GetTbl_Skill_Record( rightSlot.getSkillID);
		if( null == skillRecord2)
		{
			Debug.LogError( "null == skillRecord2");
			return;
		}
		
		Tbl_SkillLevel_Record skillLevelRecord2 = AsTableManager.Instance.GetTbl_SkillLevel_Record( rightSlot.getSkillLevel, rightSlot.getSkillID);
		if( null == skillLevelRecord2)
		{
			Debug.LogError( "null == skillLevelRecord2");
			return;
		}
		
		tip2.Init( skillRecord2, skillLevelRecord2, price);

		tip1.Sibling = tip2;
		tip2.Sibling = tip1;
	}
}
