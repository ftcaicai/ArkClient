using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class AsChoiceSkillTab : AsSkillTab
{
	public UIScrollList list = null;
	public GameObject objChoiceSkillItem = null;
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

	private void InsertSkillInfoData( Tbl_SkillBook_Record _record, bool flag)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		int userLevel = userEntity.GetProperty<int>( eComponentProperty.LEVEL);
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( _record.Skill1_Level, _record.Skill1_Index);
		if( ( true == flag) && ( ( userLevel < skillLevelRecord.Level_Limit) || ( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)_record.SkillShop_BuyAmount)))
			return;
		
		UIListItem item = list.CreateItem( objChoiceSkillItem) as UIListItem;		
		
		AsChoiceSkillItem _asChoiceSkillItem = item.gameObject.GetComponent<AsChoiceSkillItem>();
		Debug.Assert( null != _asChoiceSkillItem);
		_asChoiceSkillItem.SkillBookRecord = _record;

		StringBuilder sb = new StringBuilder();

		_asChoiceSkillItem.price = _record.SkillShop_BuyAmount;
		
		bool isResettedSkill = false;
		if( true == AsUserInfo.Instance.resettedSkills.ContainsKey( _record.Index))
		{
			sb.AppendFormat( "{0}{1}\n\n{2}0", _asChoiceSkillItem.infoColor.ToString(),
				AsTableManager.Instance.GetTbl_String(2036), _asChoiceSkillItem.goldColor.ToString());
			
			isResettedSkill = true;
		}
		else
		{
			if( skillLevelRecord.Level_Limit <= userLevel)
			{
				if( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)_record.SkillShop_BuyAmount)
				{
					sb.AppendFormat( "\n\n{0}{1}", _asChoiceSkillItem.deficientColor.ToString(),
						_record.SkillShop_BuyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));
				}
				else
				{
					sb.AppendFormat( "{0}{1}\n\n{2}{3}", _asChoiceSkillItem.infoColor.ToString(),
						AsTableManager.Instance.GetTbl_String(2036), _asChoiceSkillItem.goldColor.ToString(),
						_record.SkillShop_BuyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));
				}
			}
			else
			{
				if( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)_record.SkillShop_BuyAmount)
				{
					sb.AppendFormat( "{0}{1} {2}\n\n{3}", _asChoiceSkillItem.deficientColor.ToString(),
						AsTableManager.Instance.GetTbl_String(134), skillLevelRecord.Level_Limit,
						_record.SkillShop_BuyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));
				}
				else
				{
					sb.AppendFormat( "{0}{1} {2}\n\n{3}{4}", _asChoiceSkillItem.deficientColor.ToString(),
						AsTableManager.Instance.GetTbl_String(134), skillLevelRecord.Level_Limit, _asChoiceSkillItem.goldColor.ToString(),
						_record.SkillShop_BuyAmount.ToString( "#,#0", CultureInfo.InvariantCulture));
				}
			}
		}

		_asChoiceSkillItem.info.Text = sb.ToString();

		if( null != _record && null != _asChoiceSkillItem)
		{
			_asChoiceSkillItem.leftSlot.SetSkillIcon( _record.Skill1_Index, _record.Skill1_Level);
			_asChoiceSkillItem.leftSlot.SetType( AsSlot.SLOT_TYPE.SLT_IT_SHOP);//$yde
			UISlotItem leftItemImg = _asChoiceSkillItem.leftSlot.getIcon.GetComponent<UISlotItem>();
			GameObject.DestroyImmediate( leftItemImg.coolTime.gameObject);
			GameObject.DestroyImmediate( leftItemImg.itemCountText.gameObject);
			leftItemImg.iconImg.renderer.enabled = false;
			item.layers[0] = leftItemImg.iconImg;

			_asChoiceSkillItem.rightSlot.SetSkillIcon( _record.Skill2_Index, _record.Skill2_Level);
			_asChoiceSkillItem.rightSlot.SetType( AsSlot.SLOT_TYPE.SLT_IT_SHOP);
			UISlotItem rightItemImg = _asChoiceSkillItem.rightSlot.getIcon.GetComponent<UISlotItem>();
			GameObject.DestroyImmediate( rightItemImg.coolTime.gameObject);
			GameObject.DestroyImmediate( rightItemImg.itemCountText.gameObject);
			rightItemImg.iconImg.renderer.enabled = false;
			item.layers[1] = rightItemImg.iconImg;

			_asChoiceSkillItem.IsUsable = ( userLevel >= skillLevelRecord.Level_Limit) && ( AsUserInfo.Instance.SavedCharStat.nGold >= (ulong)_record.SkillShop_BuyAmount);
			
			if( isResettedSkill == true )
				_asChoiceSkillItem.IsUsable = true;
		}
	}
	
	override public void Init( int npcID, bool flag)
	{
		list.ClearList( true);

		SkillShop.Instance.RefreshSkillShop( npcID);
		
		Dictionary<int, Tbl_SkillBook_Record> records = SkillShop.Instance.ChoiceTypeDic;
		foreach( KeyValuePair<int, Tbl_SkillBook_Record> record in records)
		{
			InsertSkillInfoData( record.Value, flag);
		}
		
		list.ScrollListTo( 0.0f);
		
		noSkill.gameObject.SetActiveRecursively( 0 == list.Count);
	}

	override public void PromptTooltipBySkillID( int id, int level)
	{
	}
}
