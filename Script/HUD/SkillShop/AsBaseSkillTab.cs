using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsBaseSkillTab : AsSkillTab
{
	public UIScrollList list = null;
	public GameObject objBaseSkillItem = null;
	public SpriteText noSkill = null;
	
	public override UIScrollList getList()
	{
		return list;
	}
	
	// Use this for initialization
	void Start()
	{
		noSkill.Text = AsTableManager.Instance.GetTbl_String(1464);
		noSkill.gameObject.SetActiveRecursively( false);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void InsertSkillInfoData( SkillView _skill, bool flag)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		int userLevel = userEntity.GetProperty<int>( eComponentProperty.LEVEL);
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( _skill.nSkillLevel, _skill.nSkillTableIdx);
		if( ( true == flag) && ( ( userLevel < skillLevelRecord.Level_Limit) || ( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)_skill.getSkillRecord.SkillShop_BuyAmount)))
			return;
		
		UIListItem item = list.CreateItem( objBaseSkillItem) as UIListItem;
		
		AsBaseSkillItem baseSkillItem = item.gameObject.GetComponent<AsBaseSkillItem>();
		if( null == baseSkillItem)
			return;
		
		baseSkillItem.position = eSkillPosition.SkillShop;
		baseSkillItem.price = _skill.getSkillRecord.SkillShop_BuyAmount;
		baseSkillItem.Init( _skill, false);
	}
	
	override public void Init( int npcID, bool flag)
	{
		list.ClearList( true);
		
		SkillShop.Instance.RefreshSkillShop( npcID);
		
		Dictionary<int, SkillView> skills = SkillShop.Instance.BaseTypeDic;
		foreach(KeyValuePair<int, SkillView> skill in skills)
		{
			InsertSkillInfoData( skill.Value, flag);
		}
		
		list.ScrollListTo( 0.0f);
		
		noSkill.gameObject.SetActiveRecursively( 0 == list.Count);
	}
	
	override public void PromptTooltipBySkillID( int id, int level)
	{
	}
}
