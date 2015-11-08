using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Globalization;

#region -Condition-

public class ConditionBase
{
	public int CommonValue	{ get; set; }
	public ConditionBase()	{ CommonValue = 0; }
	protected ConditionBase( ConditionBase _conditon)	{ }
	public virtual bool CheckAccept()	{ return true; }
}

public class ConditionDeath : ConditionBase
{
	public int DeathCount { get; set; }
}

public class ConditionItem : ConditionBase
{
	public ItemConditionType Type { get; set; }
	public int ItemID { get; set; }
	public int ItemCount { get; set; }

	#region -Initializes-
	public ConditionItem() { ItemID = ItemCount = 0; }
	public ConditionItem( int _itemID, int _itemCount)
	{
		ItemID = _itemID;
		ItemCount = _itemCount;
	}

	protected ConditionItem( ConditionItem _conditionItem)
	{
		ItemID = _conditionItem.ItemID;
		ItemCount = _conditionItem.ItemCount;
	}
	#endregion

	public override bool CheckAccept()
	{
		Dictionary<int, int> dicHaveItem = new Dictionary<int, int>();
		InvenSlot[] invens = ItemMgr.HadItemManagement.Inven.invenSlots;
		foreach( InvenSlot inven in invens)
		{
			if( inven != null && inven.realItem != null)
			{
				int itemID = inven.realItem.item.ItemID;

				if( dicHaveItem.ContainsKey( itemID))
					dicHaveItem[itemID] += 1;
				else
					dicHaveItem.Add( itemID, 1);
			}
		}

		// 아이템 소유 & 수량 체크
		if( !dicHaveItem.ContainsKey( ItemID))
			return false;

		int count = dicHaveItem[ItemID];
		if( ItemCount > count)
			return false;

		return true;
	}
}


public class ConditionLevel : ConditionBase
{
	public int MinLevel { get; set; }
	public int MaxLevel { get; set; }

	public ConditionLevel() { MinLevel = MaxLevel = -1; }
	public ConditionLevel( int _minLevel, int _maxLevel) { MinLevel = _minLevel; MaxLevel = _maxLevel; }

	public bool CheckAcceptForDaily()
	{
		int level = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<int>( eComponentProperty.LEVEL);

		if( level >= MinLevel && level <= MaxLevel)
			return true;

		return false;
	}

	public bool CheckAccept( ConditionBase _condition)
	{
		if( _condition.GetType() != GetType())
			return true;

		if( _condition.CommonValue >= MinLevel && _condition.CommonValue <= MaxLevel)
			return true;

		return false;
	}
}


public class ConditionRace : ConditionBase
{
	public eRACE RaceType { get; set; }

	public ConditionRace() { RaceType = eRACE.NONE; }
	public ConditionRace( eRACE _raceType) { RaceType = _raceType; }
}


public class ConditionClass : ConditionBase
{
	public eCLASS ClassType { get; set; }

	public ConditionClass() { ClassType = eCLASS.NONE; }
	public ConditionClass( eCLASS _classType) { ClassType = _classType; }

	public override bool CheckAccept()
	{
		if( AsUserInfo.Instance.SavedCharStat.class_ == ( int)ClassType)
			return true;

		return false;
	}
}

public class ConditionQuestPass : ConditionBase
{
	public int QuestID { get; set; }
	public QuestPassType QuestPassType { get; set; }
	
	public ConditionQuestPass() { QuestID = 0; QuestPassType = QuestPassType.AND; }
	public ConditionQuestPass( int _questID, QuestPassType _passType) { QuestID = _questID; QuestPassType = _passType; }
}

public class ConditionSkill : ConditionBase
{
	public int SkillID { get; set; }
	public SkillConditionType Type { get; set; }

	public ConditionSkill() { Type = SkillConditionType.MAX; SkillID = 0; }
	public ConditionSkill( SkillConditionType _skillType, int _skillID) { SkillID = _skillID; Type = _skillType; }

	public override bool CheckAccept()
	{
		bool bCheck1 = SkillBook.Instance.dicActive.ContainsKey( SkillID);
		bool bCheck2 = SkillBook.Instance.dicFinger.ContainsKey( SkillID);
		bool bCheck3 = SkillBook.Instance.dicPassive.ContainsKey( SkillID);

		if( bCheck1 == false && bCheck2 == false && bCheck3 == false)
			return false;

		return true;
	}
}

public class ConditionMonsterKill : ConditionBase
{
	public int MonsterID { get; set; }
	public int MonsterKillCount { get; set; }
	
	public ConditionMonsterKill() { MonsterID = MonsterKillCount = 0; }
	public ConditionMonsterKill( int _monsterID, int _monsterkillCount) { MonsterID = _monsterID; MonsterKillCount = _monsterkillCount; }
}

public class ConditionProductionMastery : ConditionBase
{
	public eITEM_PRODUCT_TECHNIQUE_TYPE ProductionMasteryType { get; set; }
	public int ProductionMastery { get; set; }
	
	public ConditionProductionMastery() { ProductionMasteryType = eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX; ProductionMastery = 0; }
	public ConditionProductionMastery( eITEM_PRODUCT_TECHNIQUE_TYPE _productionSkillType, int _productionMastery) { ProductionMasteryType = _productionSkillType; ProductionMastery = _productionMastery; }
	
	public override bool CheckAccept()
	{
		if( ProductionMasteryType == eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX)
		{
			int lv = AsUserInfo.Instance.HaveProductTechLvMax();

			if( lv >= ProductionMastery)
				return true;

			return false;
		}
		else
		{
			int lv = AsUserInfo.Instance.GetProductTechniqueLv( ProductionMasteryType);

			if( lv >= ProductionMastery)
				return true;

			return false;
		}
	}
}

public class ConditionDesignation : ConditionBase
{
	public DesignationType type;
	public int data;

	public override bool CheckAccept()
	{
		if( type == DesignationType.DESIGNATION_COUNT)
		{
			if( AsDesignationManager.Instance.GetObtainedDesignationCount() >= data)
				return true;
		}
		else if( type == DesignationType.DESIGNATION_HAVE)
		{
			return AsDesignationManager.Instance.IsObtainedDesignation( data);
		}
		else if( type == DesignationType.DESIGNATION_EQUIP)
		{
			if( AsDesignationManager.Instance.GetCurrentDesignation().id == data)
				return true;
		}
	
		return false;
	}
}

public class ConditionCollectionSkill : ConditionBase
{
	public int CollectionSkillID { get; set; }
	public int ProductionSkillMastery { get; set; }
	
	public ConditionCollectionSkill() { CollectionSkillID = ProductionSkillMastery = 0; }
	public ConditionCollectionSkill( int _collectionSkillID, int _productionSkillMastery) { CollectionSkillID = _collectionSkillID; ProductionSkillMastery = _productionSkillMastery; }
}

public class ConditionMap : ConditionBase
{
	public int MapID { get; set; }
	public int LocationX { get; set; }
	public int LocationZ { get; set; }
	public int LocationRadius { get; set; }

	public ConditionMap()
	{
		MapID = LocationX = LocationZ = LocationRadius = -1;
	}

	public ConditionMap( int _mapID, int _locationX, int _locationZ, int _radius)
	{
		MapID = _mapID;
		LocationX = _locationX;
		LocationZ = _locationZ;
		LocationRadius = _radius;
	}
}


public class ConditionGold : ConditionBase
{
	public ulong gold;

	public ConditionGold( ulong _gold)
	{
		gold = _gold;
	}

	public override bool CheckAccept()
	{
		if( AsUserInfo.Instance.SavedCharStat.nGold >= gold)
			return true;

		return false;
	}
}

public class ConditionFriendship : ConditionBase
{
	public int count;
	public int point;
}
#endregion

#region -Prepare-
public class PrepareItem
{
	public int ItemID { get; set; }
	public int ItemCount { get; set; }
	public bool ItemGetBack { get; set; }
}

public class PrepareMonster
{
	public int MonsterID { get; set; }
	public int Count { get; set; }
	public int DelayTime { get; set; }
	public int LifeTime { get; set; }
	public int Radius { get; set; }
}

public class PrepareCollection
{
	public int CollectionId { get; set; }
}

public class PrepareBuff
{
	public int BuffID { get; set; }
	public int BuffTime { get; set; }
}

public class PrepareTransform
{
	public int TransformID { get; set; }
	public int TransformBuffId { get; set; }
	public int TransformBuffTime { get; set; }
}

public class PrepareMoveMap
{
	public int MapID { get; set; }
	public float MapLocationX { get; set; }
	public float MapLocationZ { get; set; }

	public PrepareMoveMap()
	{
		MapID = -1;
		MapLocationX = MapLocationZ = 0.0f;
	}
}

public class PrepareCharacterSize
{
	public int SizeRate { get; set; }
	public int BuffID { get; set; }
	public int BuffTime { get; set; }
}

public class PrepareItemDropMonster
{
	public int TargetMonsterID { get; set; }
	public int ItemID { get; set; }
	public int ItemDropMin { get; set; }
	public int ItemDropMax { get; set; }
	public int ItemDropRate { get; set; }
    public bool Champion { get; set; }
}

public class PrepareItemDropMonsterKind
{
	public int TargetMonsterKindID { get; set; }
	public int ItemID { get; set; }
	public int ItemDropMin { get; set; }
	public int ItemDropMax { get; set; }
	public int ItemDropRate { get; set; }
    public bool Champion { get; set; }
}

public class PrepareItemDropCollection
{
	public int TargetCollectionID { get; set; }
	public int ItemID { get; set; }
	public int ItemDropMin { get; set; }
	public int ItemDropMax { get; set; }
	public int ItemDropRate { get; set; }
}

public class PrepareOpenUI
{
	public PrepareOpenUIType UIType { get; set; }
}
#endregion

#region-Achievement-
public class AchBase
{
	public bool IsComplete { get; set; }
	public string Achievement { get; set; }
	public int DestCount { get; set; }
	public int CommonCount { get; set; }
	public int CommonCountSub { get; set; }
	public ulong CommonUlongCount { get; set; }
	public float DestFloatCount { get; set; }
	public float CommonFloatCount { get; set; }
	public int AchievementNum { get; set; }
	public int AssignedQuestID { get; set; }
	public bool FromRunning { get; set; }
	public QuestMessages QuestMessageType { get; set; }
    public int Target { get; set; }
    public bool Champion { get; set; }

	public AchBase()
	{
		Achievement = string.Empty;
		IsComplete = false;
		DestCount = 1;
		CommonCount = 0;
		DestFloatCount = 0.0f;
		CommonFloatCount = 0.0f;
		AchievementNum = 0;
		AssignedQuestID = 0;
		CommonUlongCount = 0;
		FromRunning = false;
        Target = -1;
		QuestMessageType = QuestMessages.QM_NONE;
        Champion = false;
	}

	public virtual void SetAchievementCommonCount( int _count)
	{
		CommonCount = _count;
		CommonFloatCount = (float)_count;
		CommonUlongCount = (ulong)_count;
		CheckComplete();
	}

	/// <summary>
	/// 진행 상황을 string로 보여준다.
	/// </summary>
	/// <returns></returns>
	public virtual string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " ");
		sb.Append( GetProgressStringOnly());

		return sb.ToString();
	}

	public virtual string GetProgressStringOnly()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append( "(");
		sb.Append( CommonCount.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( "/");
		sb.Append( DestCount.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( ")");

		return sb.ToString();
	}

	public string GetAchievementString()
	{
		if( Achievement != null)
			return Achievement;

		return "Ach is null";
	}

	public void SetImmediatelyClear()
	{
		IsComplete = true;
	}

	public virtual bool CheckComplete() { IsComplete = ( CommonCount > 0); return IsComplete; }

	public virtual void ProcessMessage( QuestMessages message, Object data) { }

	public virtual void CompleteAchievement()
	{
		Debug.Log( "quest id: " + AssignedQuestID);
		Debug.Log( "ach idx : " + AchievementNum);
	}

	public virtual void SendProgressMessage( string _message)
	{
		if( AsChatManager.Instance == null)
			return;

		if( IsComplete == true)
		{
			StringBuilder sb = new StringBuilder( GetAchievementString());
			sb.Append( " (");
			sb.Append( AsTableManager.Instance.GetTbl_String( 131));
			sb.Append( ")");
		
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage( sb.ToString(), true);
	
			CompleteAchievement();
		}
		else
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage( _message, false);
		}
	}
}

public class AchTalkNPC : AchBase
{
	public int NpcID { get; set; }

	public AchTalkNPC( int _npcID)
	{
		NpcID = _npcID;
	}

	public AchTalkNPC()
	{
		NpcID = 0;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_TALK_WITH_NPC && IsComplete == false)
		{
			AchTalkNPC talkWithNpc = data as AchTalkNPC;

			if( talkWithNpc.NpcID == NpcID)
			{
				CommonCount = 1;
				IsComplete = true;
				SendProgressMessage( GetProgressString());


				if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
				{
					// update npc quest holder
					if (QuestHolderManager.instance != null)
						QuestHolderManager.instance.UpdateQuestHolder(NpcID);

					// update world map npc icon
					if (AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
						AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().UpdateNpcQuestIcon();
				}
			}
		}
	}

	public override string GetProgressString()
	{
		if( IsComplete == true)
		{
			StringBuilder sb = new StringBuilder( GetAchievementString());

			sb.Append( " (");
			sb.Append( AsTableManager.Instance.GetTbl_String( 131));
			sb.Append( ")");

			return sb.ToString();
		}
		else
			return GetAchievementString();
	}
}

public class AchKillMonster : AchBase
{
	public int MonsterID { get; set; }
	public int MonsterKillCount { get; set; }

	public AchKillMonster( int _monsterID, bool _isChampion, int _monsterKillCount)
	{
		Target = MonsterID	= _monsterID;
		DestCount	= MonsterKillCount = _monsterKillCount;
		Champion	= _isChampion;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= MonsterKillCount)
		{
			CommonCount = MonsterKillCount;
			IsComplete = true;

            ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_KILL_MONSTER)
		{
			if( IsComplete == false)
			{
				AchKillMonster killMonster = data as AchKillMonster;

				if( killMonster.MonsterID == MonsterID && 
					killMonster.AchievementNum == AchievementNum &&
					killMonster.AssignedQuestID == AssignedQuestID)
				{
					CommonCount++;
	
					CheckComplete();
	
					SendProgressMessage( GetProgressString());
				}
			}
		}
	}
}

public class AchKillMonsterKind : AchBase
{
	public int MonsterKindID { get; set; }
	public int MonsterKillCount { get; set; }
	public int SkillItemID { get; set; }

	public AchKillMonsterKind(int _monsterKindID, bool _isChampion, int _monsterKillCount, int _skillItemID)
    {
		Champion		= _isChampion;
        Target = MonsterKindID = _monsterKindID;
        DestCount		= MonsterKillCount = _monsterKillCount;
		SkillItemID		= _skillItemID;
    }

	public override bool CheckComplete()
	{
		if( CommonCount >= MonsterKillCount)
		{
			CommonCount = MonsterKillCount;
			IsComplete = true;
             ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_KILL_MONSTER_KIND)
		{
			if( IsComplete == false)
			{
				AchKillMonsterKind killMonsterKind = data as AchKillMonsterKind;

				if( killMonsterKind.MonsterKindID == MonsterKindID && 
					killMonsterKind.AchievementNum == AchievementNum && 
					AssignedQuestID == killMonsterKind.AssignedQuestID)
				{
					CommonCount++;
	
					CheckComplete();
	
					SendProgressMessage( GetProgressString());
				}
			}
		}
	}
}


public class AchGetItem : AchBase
{
	public int ItemID { get; set; }
	public int ItemCount { get; set; }

	public AchGetItem( int _ItemID, int _ItemCount)
	{
		ItemID = _ItemID;
		DestCount = ItemCount = _ItemCount;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
		else
		{
			IsComplete = false;
		}
	
		return IsComplete;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( "ItemID[");
		sb.Append( ItemID);
		sb.Append( "] = ");
		sb.Append( CommonCount);
		return sb.ToString();
	}

	public virtual void ChangeItemCount( AchGetItem _getItem)
	{
		if( _getItem.ItemID == ItemID)
			CommonCount = _getItem.ItemCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_GET_ITEM)
		{
			AchGetItem getItem = data as AchGetItem;
			if( getItem.ItemID == ItemID)
			{
				CommonCount = getItem.ItemCount;
	
				bool bPrintMessage = !IsComplete;
	
				CheckComplete();

                if (IsComplete == true)
                    ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
	
				if( bPrintMessage)
					SendProgressMessage( GetProgressString());
			}
		}
		else if( message == QuestMessages.QM_GET_ITEM_COUNT_CHANGE)
		{
			AchGetItem getItem = data as AchGetItem;

			if( getItem.ItemID != ItemID)
				return;

			bool bNotMessage = ( IsComplete) && ( CommonCount <= getItem.ItemCount);
			bool bPrevComplete = IsComplete;

			ChangeItemCount( getItem);
			CheckComplete();

			if( !bNotMessage)
				SendProgressMessage( GetProgressString());


            if (IsComplete == true && bPrevComplete == false)
                ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);


			if (IsComplete == false && bPrevComplete == true)
			{
                ArkQuestmanager.instance.AddMonInfoForTargetMarkAch(this);

				if (AsHudDlgMgr.Instance != null)
					if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == true)
						AsHudDlgMgr.Instance.questMiniView.UpdateQuetMiniViewMgr();
			}
		}
	}
}

public class AchGetItemFromMonster : AchGetItem
{
	public int MonsterID { get; set; }
	public int ItemDropMin { get; set; }
	public int ItemDropMax { get; set; }
	public int ItemDropRate { get; set; }

	public AchGetItemFromMonster( int _MonsterID, bool _isChampion, int _ItemID, int _ItemCount, int _ItemDropMin, int _ItemDropMax, int _ItemDropRate) : base( _ItemID, _ItemCount)
	{
        Target = MonsterID = _MonsterID;
		Champion = _isChampion;
		ItemDropMin = _ItemDropMin;
		ItemDropMax = _ItemDropMax;
		ItemDropRate = _ItemDropRate;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_GET_ITEM_FROM_MONSTER)
		{
			AchGetItem itemMonster = data as AchGetItem;

			// 아이템을 떨어뜨린 몬스터를 알 수 없기때문에
			//if( itemMonster.MonsterID == MonsterID && itemMonster.ItemID == ItemID)
			if( itemMonster.ItemID == ItemID)
			{
				CommonCount = itemMonster.ItemCount;
	
				bool bPrintMessage = !IsComplete;
	
				CheckComplete();

                if (IsComplete == true)
                    ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
	
				if( bPrintMessage)
					SendProgressMessage( GetProgressString());
			}
		}
		else if( message == QuestMessages.QM_GET_ITEM_COUNT_CHANGE)
		{
			AchGetItem getItem = data as AchGetItem;

			if( getItem.ItemID != ItemID)
				return;

			bool bNotMessage   = ( IsComplete) && ( CommonCount <= getItem.ItemCount);
			bool bPrevComplete = IsComplete;

			ChangeItemCount( getItem);
			CheckComplete();

			if( !bNotMessage)
				SendProgressMessage( GetProgressString());

			if (IsComplete == false && bPrevComplete == true)
			{
                ArkQuestmanager.instance.AddMonInfoForTargetMarkAch(this);

				if (AsHudDlgMgr.Instance != null)
					if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == true)
						AsHudDlgMgr.Instance.questMiniView.UpdateQuetMiniViewMgr();
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
		else
		{
			IsComplete = false;
		}

		return IsComplete;
	}
}

public class AchGetItemFromMonsterKind : AchGetItem
{
	public int MonsterKindID { get; set; }
	public int ItemDropMin { get; set; }
	public int ItemDropMax { get; set; }
	public int ItemDropRate { get; set; }
	public int SkillItemID { get; set; }

	public AchGetItemFromMonsterKind( int _MonsterKindID, bool _isChampion, int _ItemID, int _ItemCount, int _ItemDropMin, int _ItemDropMax, int _ItemDropRate, int _SkillItemID): base( _ItemID, _ItemCount)
	{
        Target = MonsterKindID = _MonsterKindID;
		ItemDropMin = _ItemDropMin;
		ItemDropMax = _ItemDropMax;
		ItemDropRate = _ItemDropRate;
		Champion = _isChampion;
		SkillItemID = _SkillItemID;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_GET_ITEM_FROM_MONSTER_KIND)
		{
			AchGetItem itemMonster = data as AchGetItem;

			if( itemMonster.ItemID == ItemID)
			{
				CommonCount = itemMonster.ItemCount;
				bool bPrintMessage = IsComplete;
				CheckComplete();

                if (IsComplete == true)
                    ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
	
				if( !bPrintMessage)
					SendProgressMessage( GetProgressString());
			}
		}
		else if( message == QuestMessages.QM_GET_ITEM_COUNT_CHANGE)
		{
			AchGetItem getItem = data as AchGetItem;

			if( getItem.ItemID != ItemID)
				return;

			bool bNotMessage   = ( IsComplete) && ( CommonCount <= getItem.ItemCount);
			bool bPrevComplete = IsComplete;

			ChangeItemCount( getItem);
			CheckComplete();

			if( !bNotMessage)
				SendProgressMessage( GetProgressString());


			if (IsComplete == false && bPrevComplete == true)
			{
                ArkQuestmanager.instance.AddMonInfoForTargetMarkAch(this);

				if (AsHudDlgMgr.Instance != null)
					if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == true)
						AsHudDlgMgr.Instance.questMiniView.UpdateQuetMiniViewMgr();
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
		else
		{
			IsComplete = false;
		}
	
		return IsComplete;
	}
}

/*public class AchGetItemFromCollection : AchGetItem
{
 public int CollectionID { get; set; }
 public int ItemDropMin { get; set; }
 public int ItemDropMax { get; set; }
 public int ItemDropRate { get; set; }

 public AchGetItemFromCollection() { }

 public AchGetItemFromCollection( int _CollectionID, int _ItemID, int _ItemCount, int _ItemDropMin, int _ItemDropMax, int _ItemDropRate)
 {
 CollectionID = _CollectionID;
 ItemID = _ItemID;
 ItemCount = _ItemCount;
 ItemDropMin = _ItemDropMin;
 ItemDropMax = _ItemDropMax;
 ItemDropRate = _ItemDropRate;
 }

 public override void ProcessMessage( QuestMessages message, object data)
 {
 if( message == QuestMessages.QM_GET_ITEM_FROM_COLLECTION)
 {
 if( IsComplete == true)
 return;

 AchGetItemFromCollection itemCollection = data as AchGetItemFromCollection;

 if( itemCollection.CollectionID == CollectionID && itemCollection.ItemID == ItemID)
 {
 CommonCount++;
 CheckComplete();
 SendProgressMessage( GetProgressString());
 }
 }
 }

 public override string GetProgressString()
 {
 StringBuilder sb = new StringBuilder( GetAchievementString());
 sb.Append( " (");
 sb.Append( CommonCount);
 sb.Append( "/");
 sb.Append( ItemCount);
 sb.Append( ")");
 return sb.ToString();
 }

 public override bool CheckComplete()
 {
 if( CommonCount >= ItemCount)
 {
 CommonCount = ItemCount;
 IsComplete = true;
 }

 return IsComplete;
 }
}*/

public class AchUseItem : AchBase
{
	public int ItemID { get; set; }
	public int ItemCount { get; set; }

	public AchUseItem( int _ItemID, int _ItemCount)
	{
		ItemID = _ItemID;
		DestCount = ItemCount = _ItemCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_ITEM)
		{
			if( IsComplete == true)
				return;

			AchUseItem useItem = data as AchUseItem;

			if( useItem.ItemID == ItemID)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}
}

public class AchUseItemInMap : AchUseItem
{
	public int QuestID { get; set; }
	public int MapID { get; set; }
	public float MapLocationX { get; set; }
	public float MapLocationZ { get; set; }
	public int MapLocationRadius { get; set; }
	
	public AchUseItemInMap( int _MapID, float _MapLocationX, float _MapLocationZ, int _MapLocationRadius, int _ItemID, int _ItemCount) : base( _ItemID, _ItemCount)
	{
		MapID = _MapID;
		MapLocationX = _MapLocationX;
		MapLocationZ = _MapLocationZ;
		MapLocationRadius = _MapLocationRadius;
	}

	public AchUseItemInMap( int _questID, int _count) : base ( 0, _count)
	{
		QuestID = _questID;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_ITEM_IN_MAP)
		{
			if( IsComplete == true)
				return;

			AchUseItemInMap itemMap = ( AchUseItemInMap)data;

			if( itemMap.ItemID == ItemID && itemMap.AchievementNum == AchievementNum)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());

				if( IsComplete == true)
				{
					if( AsHudDlgMgr.Instance.IsOpenWorldMapDlg == true)
						AsHudDlgMgr.Instance.worldMapDlg.UpdateZoneMapImg();
	
					AsUseItemInMapMarkManager.instance.DeleteMark( QuestID, AchievementNum);
				}
			}
		}
	}
}

public class AchMapInto : AchBase
{
	public int QuestID { get; set; }
	public int MapID { get; set; }
	public float MapLocationX { get; set; }
	public float MapLocationY { get; set; }
	public float MapLocationZ { get; set; }
	public int MapLocationRadius { get; set; }

	public AchMapInto()
	{
		QuestID = 0;
		MapID = MapLocationRadius = -1;
		MapLocationX = MapLocationY = MapLocationZ = 0.0f;
	}

	public AchMapInto( int _questID)
	{
		QuestID = _questID;
	}

	public AchMapInto( int _questID, int _mapID, float _locationX, float _locationY, float _locationZ)
	{
		MapLocationX = _locationX;
		MapLocationY = _locationY;
		MapLocationZ = _locationZ;
		AssignedQuestID = _questID;
		MapID = _mapID;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_MAP_INTO)
		{
			if( IsComplete == true)
				return;

			AchMapInto mapInto = data as AchMapInto;

			if( QuestID == mapInto.QuestID && AchievementNum == mapInto.AchievementNum)
			{
				CommonCount = 1;
				IsComplete = true;
				SendProgressMessage( GetProgressString());
	
				AsIntoMapTriggerManager.instance.DeleteTrigger( QuestID, mapInto.AchievementNum);
			}
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchEquipItem : AchBase
{
	public int ItemID { get; set; }
	public AchEquipItem() { ItemID = 0; }
	public AchEquipItem( int _itemID) { ItemID = _itemID; }

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_EQUIP_ITEM || message == QuestMessages.QM_UNEQUIP_ITEM)
		{
			AchEquipItem equipItem = data as AchEquipItem;

			if (ItemID != equipItem.ItemID)
				return;

			if (message == QuestMessages.QM_EQUIP_ITEM)
			{
				if (IsComplete == true)
					return;

				CommonCount = 1;
				IsComplete = true;
			}
			else if (message == QuestMessages.QM_UNEQUIP_ITEM)
			{
				// no record equip item
				if (IsComplete == false)
					return;

				CommonCount = 0;
				IsComplete = false;
			}

			SendProgressMessage(GetProgressString());
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchGetSkill : AchBase
{
	public int SkillID { get; set; }
	public AchGetSkill() { SkillID = 0;}
	public AchGetSkill( int _skillID) { SkillID = _skillID; }

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_GET_SKILL)
			return;

		if( IsComplete == true)
			return;

		AchGetSkill getSkill = data as AchGetSkill;

		if( getSkill.SkillID == 0)
		{
			CommonCount++;
			IsComplete = true;
			SendProgressMessage( GetProgressString());
		}
		else if( SkillID == getSkill.SkillID)
		{
			CommonCount++;
			IsComplete = true;
			SendProgressMessage( GetProgressString());
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchUseSkill : AchBase
{
	public int SkillID { get; set; }
	public int SkillUseCount { get; set; }
	
	public AchUseSkill( int _SkillID, int _SkillUseCount)
	{
		SkillID = _SkillID;
		DestCount = SkillUseCount = _SkillUseCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_SKILL)
		{
			if( IsComplete == true)
				return;

			AchUseSkill useSkill = data as AchUseSkill;
			if( useSkill.SkillID == SkillID)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= SkillUseCount)
		{
			CommonCount = SkillUseCount;
			IsComplete = true;
		}

		return IsComplete;
	}
}

public class AchUseSkillToMonster : AchUseSkill
{
	public int MonsterID { get; set; }

	public AchUseSkillToMonster( int _SkillID, int _MonsterID, int _SkillCount) : base ( _SkillID, _SkillCount)
	{
		MonsterID = _MonsterID;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= SkillUseCount)
		{
			CommonCount = SkillUseCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_USE_SKILL_TO_MONSTER)
			return;

		if( IsComplete == true)
			return;

		AchUseSkillToMonster useSkillToMon = data as AchUseSkillToMonster;
		if( useSkillToMon.MonsterID == MonsterID && useSkillToMon.SkillID == SkillID)
		{
			CommonCount++;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}
}

public class AchUseSkillToMonsterKind : AchUseSkill
{
	public int MonsterKindID { get; set; }

	public AchUseSkillToMonsterKind( int _SkillID, int _MonsterKindID, int _SkillCount) : base( _SkillID, _SkillCount)
	{
		SkillID = _SkillID;
		MonsterKindID = _MonsterKindID;
		DestCount = SkillUseCount = _SkillCount;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= SkillUseCount)
		{
			CommonCount = SkillUseCount;
			IsComplete = true;
		}

		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_USE_SKILL_TO_MONSTER_KIND)
			return;

		if( IsComplete == true)
			return;

		AchUseSkillToMonsterKind useSkillToMonKind = data as AchUseSkillToMonsterKind;
		if( useSkillToMonKind.MonsterKindID == MonsterKindID && useSkillToMonKind.SkillID == SkillID)
		{
			CommonCount++;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}
}

public class AchProductionMastery : AchBase
{
	public eITEM_PRODUCT_TECHNIQUE_TYPE ProductionType { get; set; }
	public int ProductionMastery { get; set; }

	public AchProductionMastery( eITEM_PRODUCT_TECHNIQUE_TYPE _ProductionType, int _ProductionMastery)
	{
		ProductionType = _ProductionType;
		DestCount = ProductionMastery = _ProductionMastery;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_PRODUCE_MASTERY)
			return;

		if( IsComplete == true)
			return;

		AchProductionMastery mastery = data as AchProductionMastery;

		if( ProductionType == eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX)
		{
			if( mastery.ProductionMastery > CommonCount)
			{
				CommonCount = mastery.ProductionMastery;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
		else if( mastery.ProductionType == ProductionType)
		{
			CommonCount = mastery.ProductionMastery;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= ProductionMastery;

		if( CommonCount >= ProductionMastery)
			CommonCount = ProductionMastery;
	
		return IsComplete;
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
	
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/");
		sb.Append( ProductionMastery.ToString());
		sb.Append( ")");
	
		return sb.ToString();
	}
}


public class AchProduceItem : AchBase
{
	public int ProduceItemID { get; set; }
	public int ProduceItemCount { get; set; }

	public AchProduceItem( int _ProduceItemID, int _ProduceItemCount)
	{
		ProduceItemID = _ProduceItemID;
		DestCount = ProduceItemCount = _ProduceItemCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_PRODUCE_ITEM)
			return;

		if( IsComplete == true)
			return;

		AchProduceItem item = data as AchProduceItem;

		if( item.ProduceItemID == ProduceItemID)
		{
			CommonCount += item.ProduceItemCount;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= ProduceItemCount;
	
		return IsComplete;
	}
}

public class AchCollectionMastery : AchBase
{
	public int CollectionSkillID { get; set; }
	public int CollectionMastery { get; set; }

	public AchCollectionMastery( int _CollectionSkillID, int _CollectionMastery)
	{
		CollectionSkillID = _CollectionSkillID;
		DestCount = CollectionMastery = _CollectionMastery;
	}
	
	public override void ProcessMessage( QuestMessages message, object data)
	{
	}
}

public class AchGetLevel : AchBase
{
	public int Level { get; set; }
	
	public AchGetLevel( int level) { Level = level; }

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_LEVEL_UP)
		{
			if( IsComplete == true)
				return;

			AchGetLevel level = data as AchGetLevel;

			if( level.Level >= Level)
			{
				CommonCount = 1;
				IsComplete = true;
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchTime : AchBase
{
	public int Hour { get; set; }
	public int Min { get; set; }
	public int Sec { get; set; }
	public bool IsEnd { get; set; }
	public bool Start { get; set; }
	public int TotSec = 0;

	public AchTime()
	{
		Hour = Min = Sec = TotSec = CommonCount = 0;
		IsEnd = false;
		Start = false;
	}

	public AchTime( int _Hour, int _Min, int _Sec)
	{
		Hour = _Hour;
		Min = _Min;
		Sec = _Sec;
		TotSec = Sec + Min * 60 + Hour * 3600;
		CommonFloatCount = (float)TotSec;
		IsEnd = false;
		Start = false;
		DestCount = TotSec;
	}

	public float DiscountTime( float fTimeDelta)
	{
		if( IsEnd == false && Start == true)
		{
			CommonFloatCount -= fTimeDelta;

			CommonCount = (int)CommonFloatCount;

			if( CommonFloatCount <= 0.0f)
			{
				CommonFloatCount = 0.0f;
				IsEnd = true;
				return CommonFloatCount;
			}
		}

		return CommonFloatCount;
	}

	public void ResetTime()
	{
		IsEnd = false;
		Start = false;
		IsComplete = false;
		CommonFloatCount = (float)TotSec;
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( string.Format( " ({0:F0} /", CommonFloatCount));
		sb.Append( TotSec);
		sb.Append( ")");
	
		return sb.ToString();
	}

	public bool IsHasData { get { return !( Hour == 0 && Min == 0 && Sec == 0); } }
}

public class AchTimeSurvival : AchTime
{
	public AchTimeSurvival() : base() { }

	public AchTimeSurvival( int _Hour, int _Min, int _Sec) : base( _Hour, _Min, _Sec) { }

	public override bool CheckComplete()
	{
		if( CommonFloatCount > 0.0f)
			return false;
		else
			return true;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_TIME_SURVIVAL)
			IsComplete = CheckComplete();
	}
}

public class AchTimeLimit : AchTime
{
	public AchTimeLimit() : base() { }
	
	public AchTimeLimit( int _Hour, int _Min, int _Sec) : base( _Hour, _Min, _Sec) { }

	public override bool CheckComplete()
	{
		if( CommonFloatCount > 0.0f)
			return true;
		else
			return false;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_TIME_LIMIT)
			IsComplete = CheckComplete();
	}
}

public class AchCheer : AchBase
{
	public CheerType type { get; set; }
	public int Count { get; set; }

	public AchCheer( CheerType _type, int _count)
	{
		type = _type;
		DestCount = Count = _count;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= Count)
		{
			CommonCount = Count;
			IsComplete = true;
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_CHEER)
			return;

		if( IsComplete == false)
		{
			AchCheer cheer = data as AchCheer;

			if( cheer.type == type)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}
}

public class AchSocialPoint : AchBase
{
	public int point { get; set; }

	public AchSocialPoint( int _point)
	{
		DestCount = point = _point;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= point)
		{
			CommonCount = point;
			IsComplete = true;
		}
		else
		{
			IsComplete = false;
		}
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_SOCIAL_POINT)
			return;

		AchSocialPoint socialPoint = data as AchSocialPoint;

		bool prevState = IsComplete;
		bool notSamePrev = CommonCount != socialPoint.point;

		CommonCount = socialPoint.point;
		CheckComplete();

		if( !( prevState == true && IsComplete == true) && notSamePrev)
			SendProgressMessage( GetProgressString());
	}
}

public class AchAddFirends : AchBase
{
	public int count { get; set; }

	public AchAddFirends( int _count)
	{
		DestCount = count = _count;
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= count)
		{
			CommonCount = count;
			IsComplete = true;
		}
		else
			IsComplete = false;
	
		return IsComplete;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message != QuestMessages.QM_ADD_FRIEND)
			return;

		AchAddFirends addFirend = data as AchAddFirends;

		bool prevState = IsComplete;
		bool notSamePrev = CommonCount != addFirend.count;

		CommonCount = addFirend.count;
		CheckComplete();

		if( !( prevState == true && IsComplete == true) && notSamePrev)
			SendProgressMessage( GetProgressString());
	}
}

public class AchOpenUI : AchBase
{
	public OpenUIType type { get; set; }
	public int mapID { get; set; }

	public AchOpenUI( OpenUIType _type, int _mapID = 0)
	{
		type = _type;
		mapID = _mapID;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_OPEN_UI && IsComplete == false)
		{
			AchOpenUI uiOpen = data as AchOpenUI;

			if( uiOpen.type == type)
			{
				CommonCount = 1;
				IsComplete = true;
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchStrengthenItem : AchBase
{
	public Item.eGRADE ItemGrade;
	public int StrenthenCount;

	public AchStrengthenItem( Item.eGRADE _itemGrade, int _strengthenCount)
	{
		ItemGrade = _itemGrade;
		DestCount = StrenthenCount = _strengthenCount;
	}

	public AchStrengthenItem( int _strengthenCount)
	{
		StrenthenCount = _strengthenCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_STRENGTHEN_ITEM && IsComplete != true)
		{
			AchStrengthenItem strengthen = data as AchStrengthenItem;

			if( AssignedQuestID != strengthen.AssignedQuestID)
				return;
	
			CommonCount = strengthen.StrenthenCount;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= StrenthenCount)
		{
			CommonCount = StrenthenCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}
}

public class AchGetQuestCollection : AchGetItem
{
	public int CollectionID { get; set; }
	public int ItemMin { get; set; }
	public int ItemMax { get; set; }
	public int ItemRatio { get; set; }
	public int SkillItemID { get; set; }

	public AchGetQuestCollection( int _collectionID, int _itemID, int _itemCount, int _min, int _max, int _ratio, int _skillItemID) :base( _itemID, _itemCount)
	{
		CollectionID = _collectionID;
		DestCount = ItemCount = _itemCount;
		ItemMin = _min;
		ItemMax = _max;
		ItemRatio = _ratio;
		SkillItemID = _skillItemID;
	}

	public AchGetQuestCollection( int _itemID, int _itemCount) : base( _itemID, _itemCount)
	{
		CollectionID = 0;
		DestCount = ItemCount = _itemCount;
		ItemMin = 0;
		ItemMax = 0;
		ItemRatio = 0;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_QUEST_COLLECTION)
		{
			AchGetItem item = data as AchGetItem;

			if( item.ItemID == ItemID)
			{
				CommonCount = item.ItemCount;

				bool bPrintMessage = !IsComplete;
				bool bPrevComplete = IsComplete;

				CheckComplete();

				if( bPrintMessage)
					SendProgressMessage( GetProgressString());

                if (IsComplete == true)
                    ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);


				if (bPrevComplete == false && IsComplete == true)
					ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();

			}
		}
		else if( message == QuestMessages.QM_GET_ITEM_COUNT_CHANGE)
		{
			Debug.LogWarning( "quest collection item change");
			AchGetItem getItem = data as AchGetItem;

			if( getItem.ItemID != ItemID)
				return;

			bool bNotMessage = ( IsComplete) && ( CommonCount <= getItem.ItemCount);
			bool bPrevComplete = IsComplete;

			ChangeItemCount( getItem);
			CheckComplete();

			if( !bNotMessage)
				SendProgressMessage( GetProgressString());


			if (bPrevComplete == true && IsComplete == false)
			{
				ArkQuestmanager.instance.CheckQuestMarkAllNpcAndCollecion();

                ArkQuestmanager.instance.AddMonInfoForTargetMarkAch(this);

				if (AsHudDlgMgr.Instance != null)
					if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == true)
						AsHudDlgMgr.Instance.questMiniView.UpdateQuetMiniViewMgr();
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= ItemCount)
		{
			CommonCount = ItemCount;
			IsComplete = true;
		}
		else
		{
			IsComplete = false;
		}

		return IsComplete;
	}
}

public class AchExtension : AchBase
{
	public UIExtensionType type { get; set; }

	public AchExtension( UIExtensionType _type)
	{
		type = _type;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_EXTENSTION && IsComplete == false)
		{
			AchExtension extension = data as AchExtension;

			if( extension.type == type)
			{
				CommonCount = 1;
				IsComplete = true;
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/1)");
		return sb.ToString();
	}
}

public class AchPostingSNS : AchBase
{
	public PostingSnsType type { get; set; }
	public int count;

	public AchPostingSNS( PostingSnsType _type, int _count)
	{
		type = _type;
		DestCount = count = _count;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_POSTING_SNS && IsComplete == false)
		{
			AchPostingSNS posting = data as AchPostingSNS;

			if( posting.type == type || type == PostingSnsType.TWITTER_OR_FACEBOOK)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonCount.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( "/");
		sb.Append( DestCount.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( ")");
		return sb.ToString();
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= count)
		{
			CommonCount = count;
			IsComplete = true;
		}
		else
			IsComplete = false;

		return IsComplete;
	}
}

public class AchGetCollection : AchBase
{
	public int CollectionID { get; set; }
	public int CollectionCount { get; set; }

	public AchGetCollection( int _CollectionID, int _CollectionCount)
	{
		CollectionID = _CollectionID;
		DestCount = CollectionCount = _CollectionCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_TRYCOLLECTING)
		{
			if( IsComplete == true)
				return;

			AchGetCollection getCollection = data as AchGetCollection;

			if( getCollection.CollectionID == CollectionID)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= CollectionCount)
		{
			CommonCount = CollectionCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}
}

public class AchMixItem : AchBase
{
	public int MixCount { get; set; }

	public AchMixItem( int _mixCount)
	{
		DestCount = MixCount = _mixCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_MIX_ITEM)
		{
			if( IsComplete == true)
				return;
	
			CommonCount++;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if( CommonCount > DestCount)
			CommonCount = DestCount;
	
		return IsComplete;
	}
}

public class AchAddParty : AchBase
{
	public int count;

	public AchAddParty( int _partyCount)
	{
		DestCount = count = _partyCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_ADD_PARTY)
		{
			if( IsComplete == true)
				return;
	
			AchAddParty addPary = data as AchAddParty;
	
			CommonCount = addPary.count;
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if( CommonCount > DestCount)
			CommonCount = DestCount;
	
		return IsComplete;
	}
}

public class AchCompleteDailyQuest : AchBase
{
	public int mapID;
	public int count;

	public AchCompleteDailyQuest( int _mapID, int _count)
	{
		mapID = _mapID;
		DestCount = count = _count;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_COMPLETE_DAILY_QUEST)
		{
			if( IsComplete == true)
				return;

			AchCompleteDailyQuest dailyQuest = data as AchCompleteDailyQuest;

			if( mapID != 0)
			{
				if( dailyQuest.mapID == mapID)
					CommonCount++;
				else
					return;
			}
			else
				CommonCount++;
	
			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if( CommonCount > DestCount)
			CommonCount = DestCount;
	
		return IsComplete;
	}
}

public class AchMetaQuest : AchBase
{
	public int questID;

	public AchMetaQuest( int _questID)
	{
		questID = _questID;
		DestCount = 1;
	}

	public AchMetaQuest(int _questID, bool _fromRunning)
	{
		questID = _questID;
		DestCount = 1;
		FromRunning = _fromRunning;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_META_QUEST)
		{
			if( IsComplete == true)
				return;

			AchMetaQuest metaQuest = data as AchMetaQuest;

			if( questID == metaQuest.questID)
			{
				IsComplete = true;
				CommonCount = 1;
			}
			else
				return;
	
			if (FromRunning == false)
				SendProgressMessage( GetProgressString());
		}
	}
}

public class AchFriendCall : AchBase
{
	public int callCount;

	public AchFriendCall( int _callCount)
	{
		DestCount = callCount = _callCount;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_CALL_ADD_FRIEND)
		{
			if( IsComplete == true)
				return;
	
			CommonCount++;
	
			CheckComplete();
	
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= callCount)
		{
			CommonCount = callCount;
			IsComplete = true;
		}
	
		return IsComplete;
	}
}

public class AchWaypoint : AchBase
{
	public int MapID { get; set; }

	public AchWaypoint( int _mapID)
	{
		MapID = _mapID;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_WAY_POINT)
		{
			if( IsComplete == true)
				return;

			AchWaypoint wayPoint = data as AchWaypoint;

			IsComplete = MapID == wayPoint.MapID;

			if( IsComplete == true)
				CommonCount = 1;
	
			SendProgressMessage( GetProgressString());
		}
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
	
		sb.Append( " (");
		sb.Append( CommonCount);
		sb.Append( "/");
		sb.Append( "1");
		sb.Append( ")");
	
		return sb.ToString();
	}
}


public class AchFriendship : AchBase
{
	public int count;
	public int point;

	public AchFriendship( int _count, int _point)
	{
		DestCount = count = _count;
		point = _point;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_FRIENDSHIP)
		{
			AchFriendship friendship = data as AchFriendship;

			CommonCount = friendship.count;

			if( CommonCount >= DestCount)
			{
				CommonCount = DestCount;
				IsComplete = true;
			}
			else
				IsComplete = false;
	
			SendProgressMessage( GetProgressString());
		}
	}
}

public class AchUseEmoticon : AchBase
{
	public int emoticonID;
	public int count;

	public AchUseEmoticon( int _emoticonID, int _count)
	{
		emoticonID = _emoticonID;
		DestCount = count = _count;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_CHAT_MACRO)
		{
			if( IsComplete == true)
				return;

			AchUseEmoticon useEmoticon = data as AchUseEmoticon;

			if( useEmoticon.emoticonID == emoticonID)
			{
				CommonCount++;
				CheckComplete();
				SendProgressMessage( GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		if( CommonCount >= count)
		{
			CommonCount = count;
			IsComplete = true;
		}
	
		return IsComplete;
	}
}

public class AchDesignation : AchBase
{
	public DesignationAchType type;
	public int data;

	public AchDesignation( DesignationAchType _type, int _data)
	{
		type = _type;
		data = _data;
	}

	public override void ProcessMessage( QuestMessages message, object _data)
	{
		if( message == QuestMessages.QM_DESIGNATION && !IsComplete)
		{
			AchDesignation designation = _data as AchDesignation;
	
			if( designation.type == DesignationAchType.DESIGNATION_GET)
			{
				if( designation.type != type)
					return;
	
				if( designation.data != data)
					return;
	
				if( IsComplete == true)
					return;
	
				IsComplete = true;
				CommonCount = 1;
			}
			else if( designation.type == DesignationAchType.DESIGNATION_COUNT)
			{
				if( designation.type != type)
					return;
	
				if( IsComplete == true)
					return;
	
				CommonCount += designation.data;
			}
			else if( designation.type == DesignationAchType.DESIGNATION_CHANGE)
			{
				if( designation.type != type)
					return;
	
				if( data == 0)
				{
					if( IsComplete == true)
						return;
	
					CommonCount = 1;
					IsComplete = true;
				}
				else if( designation.data == data)
				{
					CommonCount = 1;
					IsComplete = true;
				}
				else
					return;
			}
			else if( designation.type == DesignationAchType.DESIGNATION_ALREADY_EQUIP)
			{
				if( data != designation.data)
					return;
	
				if( type != DesignationAchType.DESIGNATION_CHANGE)
					return;
	
				if( IsComplete == true)
					return;
	
				CommonCount = 1;
				IsComplete = true;
			}
//			else if( designation.type == DesignationAchType.DESIGNATION_UNEQUIP)
//			{
//				if( data == 0)
//					return;
//
//				if( type == DesignationAchType.DESIGNATION_GET)
//					return;
//
//				if( designation.data == data && IsComplete == true)
//				{
//					CommonCount = 0;
//					IsComplete = false;
//				}
//				else
//					return;
//			}

			CheckComplete();
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		if( type == DesignationAchType.DESIGNATION_COUNT)
		{
			IsComplete = CommonCount >= data;

			if( IsComplete)
				CommonCount = data;
		}
	
		return IsComplete;
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());

		if( type == DesignationAchType.DESIGNATION_GET || type == DesignationAchType.DESIGNATION_CHANGE)
		{
			sb.Append( " (");
			sb.Append( CommonCount);
			sb.Append( "/");
			sb.Append( "1");
			sb.Append( ")");
		}
		else if( type == DesignationAchType.DESIGNATION_COUNT)
		{
			sb.Append( " (");
			sb.Append( CommonCount.ToString( "#,#0", CultureInfo.InvariantCulture));
			sb.Append( "/");
			sb.Append( DestCount.ToString( "#,#0", CultureInfo.InvariantCulture));
			sb.Append( ")");
		}
	
		return sb.ToString();
	}
}

public class AchGold : AchBase
{
	public GoldAchType type;
	public ulong gold;

	public AchGold( GoldAchType _type, ulong _count)
	{
		type = _type;
		gold = _count;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_GET_GOLD || message == QuestMessages.QM_GET_BACK_GOLD)
		{
			AchGold achGold = data as AchGold;

			if( achGold.type != type)
				return;

			bool prevComplete = IsComplete;

			CommonUlongCount = achGold.gold;

			CheckComplete();

			if( prevComplete == true && IsComplete == true)
				return;

			if (message == QuestMessages.QM_GET_BACK_GOLD)
			{
				if (IsComplete == true)
					SendProgressMessage(GetProgressString());
			}
			else
				SendProgressMessage( GetProgressString());
		}
		else if( message == QuestMessages.QM_USE_GOLD)
		{
			if( IsComplete == true)
				return;

			AchGold achGold = data as AchGold;

			if( achGold.type != type)
				return;
	
			CommonUlongCount += achGold.gold;
	
			CheckComplete();
	
			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonUlongCount >= gold;

		if( IsComplete == true)
			CommonUlongCount = gold;

		if( CommonUlongCount < 0)
			CommonUlongCount = 0;
	
		return IsComplete;
	}

	public override string GetProgressString()
	{
		StringBuilder sb = new StringBuilder( GetAchievementString());
		sb.Append( " (");
		sb.Append( CommonUlongCount.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( "/");
		sb.Append( gold.ToString( "#,#0", CultureInfo.InvariantCulture));
		sb.Append( ")");
		return sb.ToString();
	}
}

public class AchUseItemToTarget : AchUseItem
{
	public UseItemToTargetType targetType;
	public int targetID;
	public int successRate;

	public AchUseItemToTarget( UseItemToTargetType _type, int _targetID, bool _isChampion, int _itemID, int _itemCount, int _successRate) : base( _itemID, _itemCount)
	{
		targetType = _type;
		Champion = _isChampion;
		Target   = targetID = _targetID;
		successRate = _successRate;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_USE_ITEM_TO_TARGET && !IsComplete)
		{
			AchUseItemToTarget achUseItemToTarget = data as AchUseItemToTarget;

			if (targetID == achUseItemToTarget.targetID && ItemID == achUseItemToTarget.ItemID && AssignedQuestID == achUseItemToTarget.AssignedQuestID)
				CommonCount = achUseItemToTarget.CommonCount;
			else
				return;

			if( CheckComplete() == true)
			{
				AsUseItemToTargetPanelManager.instance.DeletePanel( AssignedQuestID, AchievementNum);
				if( targetType == UseItemToTargetType.MONSTER)
					AsUseItemToMonTriggerManager.instance.CheckCompleteTrigger();
			}

			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= ItemCount;

        if (IsComplete == true)
        {
            CommonCount = ItemCount;

            ArkQuestmanager.instance.RemoveMonInfoForTargetMarkAch(this);
        }
	
		return IsComplete;
	}
}

public class AchInsertSeal : AchBase
{
	public eInsertSealGrade sealGrade;
	public eInsertSealKind sealKind;
	public int count;

	public AchInsertSeal( eInsertSealGrade _eSealGrade, eInsertSealKind _eSealKind, int _count)
	{
		sealGrade = _eSealGrade;
		sealKind = _eSealKind;
		DestCount = count = _count;
	}

	public override void ProcessMessage( QuestMessages message, object data)
	{
		if( message == QuestMessages.QM_INSERT_SEAL && !IsComplete)
		{
			AchInsertSeal achInsertSeal = data as AchInsertSeal;

			if( achInsertSeal.sealGrade != sealGrade)
				return;

			if( achInsertSeal.sealKind != sealKind)
				return;

			CommonCount = achInsertSeal.count;

			CheckComplete();

			SendProgressMessage( GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= count;

		if( IsComplete == true)
			CommonCount = count;

		return IsComplete;
	}
}

public class AchGetPvpPoint : AchBase
{
	public int pvpPoint;

	public AchGetPvpPoint(int _pvpPoint)
	{
		DestCount = pvpPoint = _pvpPoint;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_GET_PVP_POINT && !IsComplete)
		{
			AchGetPvpPoint achGetPvpPoint = data as AchGetPvpPoint;

			if (achGetPvpPoint.AssignedQuestID == AssignedQuestID && achGetPvpPoint.AchievementNum == AchievementNum)
			{
				CommonCount = achGetPvpPoint.pvpPoint;

				CheckComplete();

				SendProgressMessage(GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}
}

public class AchPvpPlay : AchBase
{
	public ePvpMatchMode matchMode;
	public AsPvpDlg.ePvpMode pvpMode;

	public int value;

	public AchPvpPlay(ePvpMatchMode _matchMode, AsPvpDlg.ePvpMode _mode, int _value)
	{
		pvpMode = _mode;
		matchMode = _matchMode;
		DestCount = value = _value;
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_PLAY_PVP && !IsComplete)
		{
			AchPvpPlay ach = data as AchPvpPlay;

			if (ach.AssignedQuestID == AssignedQuestID && ach.AchievementNum == AchievementNum)
			{
				CommonCount = ach.value;

				CheckComplete();

				SendProgressMessage(GetProgressString());
			}
		}
	}
}

public class AchPvpWin : AchBase
{
	public ePvpMatchMode matchMode;
	public AsPvpDlg.ePvpMode pvpMode;

	public int value;

	public AchPvpWin(ePvpMatchMode _matchMode, AsPvpDlg.ePvpMode _mode, int _value)
	{
		pvpMode	  = _mode;
		matchMode = _matchMode;
		DestCount = value = _value;
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_WIN_PVP && !IsComplete)
		{
			AchPvpWin ach = data as AchPvpWin;

			if (ach.AssignedQuestID == AssignedQuestID && ach.AchievementNum == AchievementNum)
			{
				CommonCount = ach.value;

				CheckComplete();

				SendProgressMessage(GetProgressString());
			}
		}
	}
}

public class AchGetRankPoint : AchBase
{
	public int rankPoint;

	public AchGetRankPoint(int _rankPoint)
	{
		DestCount = rankPoint = _rankPoint;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_GET_RANK_POINT && !IsComplete)
		{
			AchGetRankPoint ach = data as AchGetRankPoint;

			CommonCount = ach.rankPoint;

			CheckComplete();

			SendProgressMessage(GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}
}

public class AchCompleteQuest : AchBase
{
	public QuestType questType;
	public int mapID;
	public int count;

	public AchCompleteQuest(QuestType _questType, int _mapID, int _count)
	{
		questType = _questType;
		mapID = _mapID;
		DestCount = count = _count;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_COMPLETE_QUEST && !IsComplete)
		{
			AchCompleteQuest completeQuest = data as AchCompleteQuest;

			if (questType == completeQuest.questType)
			{
				if (questType == QuestType.QUEST_DAILY)
				{
					if (mapID == completeQuest.mapID)
						CommonCount++;
					else
						return;
				}
				else
				{
					CommonCount++;
				}

				CheckComplete();

				SendProgressMessage(GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}
}

public class AchGetItemFromShop : AchGetItem
{
	public AchGetItemFromShop(int _itemID, int _count)
		: base(_itemID, _count)
	{

	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_GET_ITEM_FROM_SHOP && !IsComplete)
		{
			AchGetItem getItem = data as AchGetItem;

			if (getItem.AssignedQuestID != AssignedQuestID || getItem.AchievementNum != AchievementNum)
				return;

			if (getItem.ItemID == ItemID)
			{
				CommonCount = getItem.ItemCount;

				CheckComplete();

				SendProgressMessage(GetProgressString());
			}
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}
}

public class AchUseSocialPoint : AchBase
{
	public int point;

	public AchUseSocialPoint(int _socialPoint)
	{
		DestCount = point = _socialPoint;
	}

	public override void ProcessMessage(QuestMessages message, object data)
	{
		if (message == QuestMessages.QM_USE_SOCIAL_POINT && !IsComplete)
		{
			AchUseSocialPoint ach = data as AchUseSocialPoint;

			CommonCount += ach.point;

			CheckComplete();

			SendProgressMessage(GetProgressString());
		}
	}

	public override bool CheckComplete()
	{
		IsComplete = CommonCount >= DestCount;

		if (IsComplete == true)
			CommonCount = DestCount;

		return IsComplete;
	}
}

#endregion

#region -Reward-
public class RewardItem
{
	public eCLASS Class { get; set; }
	public int ID { get; set; }
	public int Count { get; set; }
}

public class RewardSkill
{
	public int ID;
	public int Lv;
}

public class RewardDesignation
{
	public int designationID;
}
#endregion
