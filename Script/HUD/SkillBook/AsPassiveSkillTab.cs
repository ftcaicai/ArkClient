using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPassiveSkillTab : AsSkillTab
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
		
		foreach( KeyValuePair<int,SkillView> pair in SkillBook.Instance.dicPassive)
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
		
		baseSkillItem.slot.collider.enabled = false;
		baseSkillItem.position = eSkillPosition.SkillBook;
		baseSkillItem.price = 0;
//		baseSkillItem.List = list;
		baseSkillItem.Init( _skill, SkillBook.Instance.newdicPassive.ContainsKey(_skill.nSkillTableIdx) );
	}
}
