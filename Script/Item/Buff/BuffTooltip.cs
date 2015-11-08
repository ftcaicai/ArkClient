using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffTooltip : MonoBehaviour 
{
	public SpriteText txtText;	
	public SimpleSprite iconParent;
	public Vector2 vec2Img = Vector2.zero;
	private UISlotItem m_Icon;
	
	
	protected void CreateIcon(string tex)
    {		
      	GameObject go =  Resources.Load( tex ) as GameObject;
		if( null == go )
		{
			Debug.LogError("GameObject load failed [ name : " + tex );
			return;
		}
		
		DeleteIcon();	
		
		m_Icon = ResourceLoad.CreateItemIcon( go, iconParent, Vector3.back, vec2Img, false);	
		m_Icon.slotType = UISlotItem.eSLOT_TYPE.BUFF_ICON;
		m_Icon.isUseRealItem = false;
    }
	
	protected void DeleteIcon()
	{
		if( null != m_Icon )
		{
			GameObject.DestroyImmediate( m_Icon.gameObject );	
			m_Icon = null;
		}
	}
	
	public void Open(BuffBaseData _data )
	{		
		List<Tbl_Skill_Potency> pntencyList = null;
		bool isMonsterSkill = false;
		if( true == BuffBaseMgr.IsMonsterSkillIndex( _data.GetSkillIdx() ) )
		{
			Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record( _data.GetSkillIdx() );
			if( null == skillRecord )
			{
				Debug.LogError("BuffTooltip::Open()[ null == Tbl_MonsterSkill_Record ] id : " + _data.GetSkillIdx() );
				return;
			}
			
			pntencyList = skillRecord.listSkillPotency;
			isMonsterSkill = true;
		}
		else
		{
			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _data.GetSkillIdx() );
			if( null == skillRecord )
			{
				Debug.LogError("BuffTooltip::Open()[ null == Tbl_Skill_Record ] id : " + _data.GetSkillIdx() );
				return;
			}
			
			pntencyList = skillRecord.listSkillPotency;
			isMonsterSkill = false;
		}		
		
		if( null == pntencyList )
			return;
		
		
		if( pntencyList.Count <= _data.GetPotencyIdx() )
		{
			Debug.LogError("BuffTooltip::Open()[skillRecord.listSkillPotency.Count <= _data.GetPotencyIdx()] count : " + _data.GetPotencyIdx() );
			return;
		}
		
		Tbl_Skill_Potency skillPotency = pntencyList[_data.GetPotencyIdx()];
		
		string szDesc = AsTableManager.Instance.GetTbl_String( skillPotency.Potency_BuffTooltip);
		if( true == isMonsterSkill )
		{
			txtText.Text = AsUtil.ModifyMonsterDescriptionInTooltip( szDesc, _data.GetSkillIdx(), _data.GetSkillLevel(), _data.GetPotencyIdx() );
		}
		else
		{
			txtText.Text = AsUtil.ModifyDescriptionInTooltip( szDesc, _data.GetSkillIdx(), _data.GetSkillLevel(), _data.GetPotencyIdx() );
		}
		
		
		
		CreateIcon( skillPotency.Potency_BuffIcon );
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
