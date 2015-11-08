using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsActiveSkillTab : AsSkillTab
{
	public UIScrollList list = null;
	public GameObject objSkillItem = null;
	public SpriteText noSkill = null;
	
	public override UIScrollList getList()
	{
		return list;
	}
	
	// Use this for initialization
	void Start()
	{
		noSkill.Text = AsTableManager.Instance.GetTbl_String(1466);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	override public void Init( int npcID, bool flag)
	{
		list.ClearList( true);
		
		foreach(KeyValuePair<int, SkillView> pair in SkillBook.Instance.dicActive)
		{
			InsertSkillInfoData( pair.Value);
		}
		
		list.ScrollListTo( 0.0f);
		noSkill.gameObject.SetActiveRecursively( 0 == list.Count);
	}
	
	public void InsertSkillInfoData( SkillView _skill)
	{
		UIListItem item = list.CreateItem( objSkillItem) as UIListItem;
		
		AsBaseSkillItem baseSkillItem = item.gameObject.GetComponent<AsBaseSkillItem>();
		if( null == baseSkillItem)
			return;
		
		baseSkillItem.position = eSkillPosition.SkillBook;
		baseSkillItem.price = 0;
//		baseSkillItem.List = list;
		baseSkillItem.Init( _skill, SkillBook.Instance.newdicActive.ContainsKey( _skill.nSkillTableIdx ) );		
	}
	
	override public void PromptTooltipBySkillID( int id, int level)
	{
		for( int i = 0; i < list.Count; i++)
		{
			UIListItem listItem = list.GetItem(i) as UIListItem;
			
			AsBaseSkillItem baseSkillItem = listItem.GetComponent<AsBaseSkillItem>();
			if( ( id == baseSkillItem.slot.getSkillID) && ( level == baseSkillItem.slot.getSkillLevel))
				baseSkillItem.PromptTooltip();
		}
	}
}
