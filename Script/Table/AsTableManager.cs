
//#define _USE_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ArkSphereQuestTool;

public enum eTableType {NONE, ENTITY_TEMPLATE, RACE, STAT_PER_LEVEL, NPC, NORMAL_NPC, MONSTER, STRING, OBJECT, CREATE_CHARACTER, STORE }

public class AsTableManager : MonoBehaviour
{
	public bool s_bTableLoaded = false;
	public static bool s_bTableLoadStarted = false;
	private static bool s_bCheckReadBinary = false;
	private static bool s_bUseReadBinary = false;
	public bool useReadBinary
	{
		get
		{
			if( false == s_bCheckReadBinary)
			{
				s_bCheckReadBinary = true;
				
				GameObject main = GameObject.Find( "GameMain");
				if( null != main)
				{
					AsGameMain asMain = main.GetComponent<AsGameMain>();
					s_bUseReadBinary = asMain.UseReadBinary;
				}
				else // dopamin skill_editor or ActionTool...
				{
					GameObject toolMain = GameObject.Find( "ToolMain");
					if( toolMain != null )
					{
						AsToolMain	asToolMain = toolMain.GetComponent<AsToolMain>();
						s_bUseReadBinary = asToolMain.UseReadBinary;
					}
					else
					{
						return false;
					}
				}
			}
			
			return s_bUseReadBinary;
		}
	}
	
	#region - static -
	public static readonly int sActionEffectCount = 3;
	public static readonly int sSkillPotencyCount = 5;
	public static readonly int sSkillLevelPotencyCount = 5;
	
	public static readonly int sMonsterActionEffectCount = 3;
	public static readonly int sMonsterSkillPotencyCount = 5;
	public static readonly int sMonsterSkillLevelPotencyCount = 5;
	
	public static readonly int sPetResourceCount = 3;
	public static readonly int sPetGroupCount = 12;
	public static readonly int sPetSkillCount = 8;
	public static readonly int sPetSkillLevelCount = 10;
	public static readonly int sPetScriptCount = 5;
	#endregion
	
	public static readonly int sTableLoadCount = 65;
	public int m_nCurTableLoadCount = 8;
	
	#region - singleton -
	static AsTableManager m_instance;
	public static AsTableManager Instance { get { return m_instance; } }
	#endregion
	#region - member -
	//	Tbl_EntityTemplate_Table m_Tbl_EntityTemplate;//
	Tbl_GlobalWeight_Table m_Tbl_GlobalWeight;

//	Tbl_Tribe_Table m_Tbl_Tribe;
	Tbl_Class_Table m_Tbl_Class;
	Tbl_UserLevel_Table m_Tbl_Level;
	Tbl_Npc_Table m_Tbl_Npc;
	Tbl_NormalNpc_Table m_Tbl_NormalNpc;
	Tbl_Monster_Table m_Tbl_Monster;
	Tbl_String_Table m_Tbl_String;
	Tbl_Object_Table m_Tbl_Object;

//	Tbl_SkillPotency_Table m_Tbl_SkillPotency;
	Tbl_SkillPotencyEffect_Table m_Tbl_SkillPotencyEffect;
	Tbl_Recall_Table m_Tbl_Recall;

	Tbl_Skill_Table m_Tbl_Skill;
	Tbl_SkillLevel_Table m_Tbl_SkillLevel;
	Tbl_SkillLevel_Table m_Tbl_PvpSkillLevel;
	Tbl_Action_Table m_Tbl_Action;
//	Tbl_Projectile_Table m_Tbl_Projectile;
	
	Tbl_BuffOverlap_Table m_Tbl_BuffOverlap;

	Tbl_MonsterSkill_Table m_Tbl_MonsterSkill;
	Tbl_MonsterSkillLevel_Table m_Tbl_MonsterSkillLevel;
	Tbl_MonsterAction_Table m_Tbl_MonsterAction;
//	Tbl_MonsterPassive_Table m_Tbl_MonsterPassive;
	Tbl_MonsterChampion_Table m_Tbl_MonsterChampion;
	
	Tbl_CreateCharacter_Table m_Tbl_CreateCharacter;
	Tbl_SetItem_Table m_Tbl_SetItemTable;

	Tbl_QuestTalk_Table m_Tbl_QuestTalk;
	Tbl_QuestReward_Table m_Tbl_QuestReward;
	Tbl_Quest_Table m_Tbl_Quest;
	Tbl_QuestString_Table m_Tbl_QuestString;
	
	Tbl_Store_Table	m_Tbl_Store;
	Tbl_Cash_Store_Table m_Tbl_CachStore;
	Tbl_CashStoreCostume m_Tbl_CashStoreCostume;
	Tbl_SupportEnchantTable m_TblSuppertEnchantTable;
	Tbl_SoulStoneEnchant_Table m_TblSoulStoneEnchantTable;
	Tbl_Strengthen_Table m_StrengthenTable;
	
	Tbl_SkillShop_Table m_Tbl_SkillShop;
	Tbl_SkillBook_Table m_Tbl_SkillBook;
	Tbl_RandomCoolTime_Table m_Tbl_RandomCoolTime;
	Tbl_RandItem_Table m_Tbl_RandItem;
	Tbl_Lottery_Table m_Tbl_Lottery;
	
	Tbl_Emotion_Table m_Tbl_Emotion;
	Tbl_Emoticon_Table m_Tbl_Emoticon;
	
	Tbl_TextFilter_Table m_Tbl_TextFilter;
	Tbl_ZoneMap_Table m_ZoneMapTable;
	Tbl_AreaMapTable m_AreaMapTable;
	Tbl_WarpData_Table m_WarpDataTable;
	
	Tbl_Production_Table m_ProductionTable;
	Tbl_Technic_Table m_TechnicTable;
	Tbl_BuffMinMaxTable_Table m_BuffMinMaxTable;
	
	Tbl_AttendBonus_Table m_AttendBonusTable; public Tbl_AttendBonus_Table AttendBonusTable{get{return m_AttendBonusTable;}}
	Tbl_ReturnBonus_Table m_ReturnBonusTable; public Tbl_ReturnBonus_Table ReturnBonusTable{get{return m_ReturnBonusTable;}}
	Tbl_Collection_Table m_CollectionTable; public Tbl_Collection_Table getColletionTable{get{return m_CollectionTable;}}
	
	Tbl_PreLoad_Table m_PreLoadTable;
	Tbl_InDun_Table m_InDunTable;
	Tbl_InsQuestGroup_Table m_InsQuestGroupTable;
	Tbl_InsDungeonReward_Table m_InsRewardTable;
	Tbl_ItemRankWeightTable m_ItemRankWeightTable;
	Tbl_PvpGrade_Table m_PvpGradeTable;
	
	Tbl_Promotion_Table m_PromotionTable;

	Tbl_QuestMove m_QuestMoveTable;
	
	Tbl_Event_Table m_EventTable;

	Tbl_Charge_Table m_ChargeTable;
	
	
	Tbl_SynMixEnchant_Table m_TblSynMixEnchantTable;
	Tbl_SynOptionChange_Table m_TblSynOptionChangeTable;
	Tbl_SynDisassemble_Table m_TblSynDisassembleTable;
	Tbl_SynCosMix_Table m_TblsynCosMixTable;
	
	Tbl_Pet_Table m_PetTable;
	Tbl_PetGroup_Table m_PetGroupTable;
	Tbl_PetLevel_Table m_PetLevelTable;
	Tbl_PetScript_Table m_PetScriptTable;
//	Tbl_PetSkill_Table m_PetSkillTable;
	
	Tbl_PetAction_Table m_PetActionTable;

    Tbl_ApRewardTable m_ApRewardTable;
	#endregion
	
	public void ResetZonMapTable()
	{
		m_ZoneMapTable = new Tbl_ZoneMap_Table( "Table/ZoneMapTable");
	}
	
	#region - init -
	void Awake()
	{
		m_instance = this;
		s_bTableLoaded = false;
		s_bTableLoadStarted = false;
	}

	void Start()
	{
		if( ( null == AssetbundleManager.Instance) || ( false == AssetbundleManager.Instance.useAssetbundle))
		{
			if( false == s_bTableLoadStarted)
			{
#if _USE_ASSETBUNDLE
				StartCoroutine( LoadTable());
#else
				LoadTable();
#endif
				s_bTableLoadStarted = true;
//				s_bTableLoaded = false;
			}
		}
	}
	
#if _USE_ASSETBUNDLE
	public IEnumerator LoadTable()
	{
		float fStartTime = Time.realtimeSinceStartup;
		
		#region -Version
		VersionManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
		#endregion
		
		m_Tbl_GlobalWeight = new Tbl_GlobalWeight_Table( "Table/GlobalWeightTable");
		yield return null; m_nCurTableLoadCount++;

		// < ilmeda, 20120824
		GameObject main = GameObject.Find( "GameMain");
		if( null == main)	// dopamin skill_editor use
		{
			m_Tbl_String = new Tbl_String_Table( "Table/StringTable_kor");
		}
		else
		{
			AsGameMain asMain = main.GetComponent<AsGameMain>();
		
			GAME_LANGUAGE gameLanguage = asMain.GetGameLanguage();
			switch( gameLanguage)
			{
			case GAME_LANGUAGE.LANGUAGE_KOR:
				m_Tbl_String = new Tbl_String_Table( "Table/StringTable_kor");
				m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable");
				m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable");
				break;
			case GAME_LANGUAGE.LANGUAGE_JAP:
				m_Tbl_String = new Tbl_String_Table( "Table/StringTable_jap");
				m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable_Jap");
				m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable_Jap");
				break;
			}
		}
		yield return null; m_nCurTableLoadCount++;
		// ilmeda, 20120824 >

		#region -LoadingTip
		AsLoadingTipManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
		#endregion

		m_Tbl_Class = new Tbl_Class_Table( "Table/ClassTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Level = new Tbl_UserLevel_Table( "Table/UserLevelTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Npc = new Tbl_Npc_Table( "Table/NPCTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_NormalNpc = new Tbl_NormalNpc_Table( "Table/NormalNpcTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Monster = new Tbl_Monster_Table( "Table/MonsterTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Object = new Tbl_Object_Table( "Table/ObjectTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_SetItemTable = new Tbl_SetItem_Table( "Table/SetItemTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_SkillPotencyEffect = new Tbl_SkillPotencyEffect_Table( "Table/SkillPotencyEffectTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Recall = new Tbl_Recall_Table( "Table/RecallTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Skill = new Tbl_Skill_Table( "Table/SkillTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_SkillLevel = new Tbl_SkillLevel_Table( "Table/SkillLevelTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_PvpSkillLevel = new Tbl_SkillLevel_Table( "Table/PvPSkillLevelTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Action = new Tbl_Action_Table( "Table/ActionDataList");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_BuffOverlap = new Tbl_BuffOverlap_Table( "Table/BuffOverlapTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_MonsterSkill = new Tbl_MonsterSkill_Table( "Table/MonsterSkillTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_MonsterSkillLevel = new Tbl_MonsterSkillLevel_Table( "Table/MonsterSkillLevelTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_MonsterAction = new Tbl_MonsterAction_Table( "Table/MonsterActionDataList");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_MonsterChampion = new Tbl_MonsterChampion_Table( "Table/MonsterChampionTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_CreateCharacter = new Tbl_CreateCharacter_Table( "Table/CreateCharacterTable");	yield return null; m_nCurTableLoadCount++;
		//m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_QuestReward = new Tbl_QuestReward_Table( "Table/QuestRewardTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Quest = new Tbl_Quest_Table ( "Table/QuestTable");	yield return null; m_nCurTableLoadCount++;
		m_TblSuppertEnchantTable = new Tbl_SupportEnchantTable( "Table/SupportItemTable");	yield return null; m_nCurTableLoadCount++;
		m_TblSoulStoneEnchantTable = new Tbl_SoulStoneEnchant_Table( "Table/SoulStoneEnchantTable");	yield return null; m_nCurTableLoadCount++;
		m_StrengthenTable = new Tbl_Strengthen_Table( "Table/StrengthenTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_SkillShop = new Tbl_SkillShop_Table( "Table/SkillShopTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_SkillBook = new Tbl_SkillBook_Table( "Table/SkillBookTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_RandomCoolTime = new Tbl_RandomCoolTime_Table( "Table/LotteryTypeTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_RandItem = new Tbl_RandItem_Table( "Table/RandItemTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Lottery = new Tbl_Lottery_Table( "Table/LotteryTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Emotion = new Tbl_Emotion_Table( "Table/EmotionTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_Emoticon = new Tbl_Emoticon_Table( "Table/EmoticonTable");	yield return null; m_nCurTableLoadCount++;
		//m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable");	yield return null; m_nCurTableLoadCount++;
		m_Tbl_CashStoreCostume = new Tbl_CashStoreCostume("Table/CashStoreSetTable_jap"); yield return null; m_nCurTableLoadCount++;
		m_QuestMoveTable = new Tbl_QuestMove("Table/QuestMoveTable"); yield return null; m_nCurTableLoadCount++;
		m_EventTable = new Tbl_Event_Table("Table/EventTable"); yield return null; m_nCurTableLoadCount++;

#if UNITY_IPHONE
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableIOS_Jap"); yield return null; m_nCurTableLoadCount++;
#elif UNITY_ANDROID
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableAOS_Jap"); yield return null; m_nCurTableLoadCount++;
#else
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableAOS_Jap"); yield return null; m_nCurTableLoadCount++;
#endif


		m_TblSynMixEnchantTable = new Tbl_SynMixEnchant_Table("Table/MixEnchantTable"); yield return null; m_nCurTableLoadCount++;
		m_TblSynOptionChangeTable = new Tbl_SynOptionChange_Table("Table/OptionChangeTable"); yield return null; m_nCurTableLoadCount++;
		m_TblSynDisassembleTable = new Tbl_SynDisassemble_Table("Table/DisassembleTable"); yield return null; m_nCurTableLoadCount++;
		m_TblsynCosMixTable = new Tbl_SynCosMix_Table("Table/CosMixTable"); yield return null; m_nCurTableLoadCount++;
		
		if( null != TerrainMgr.Instance)
		{
			TerrainMgr.Instance.LoadTable();
			yield return null; m_nCurTableLoadCount++;
		}

		if( null != AsEntityManager.Instance)
		{
			AsEntityManager.Instance.LoadEntityTemplates( "EntityTemplate/EntityTemplate");
			yield return null; m_nCurTableLoadCount++;
		}

		if( null != ItemMgr.Instance)
		{
			ItemMgr.ItemManagement.LoadTable( "Table/ItemTable");	yield return null; m_nCurTableLoadCount++;
			m_Tbl_Store = new Tbl_Store_Table( "Table/NpcStoreTable");	yield return null; m_nCurTableLoadCount++;
			m_Tbl_CachStore = new Tbl_Cash_Store_Table( "Table/CashStoreTable_jap");	yield return null; m_nCurTableLoadCount++;
		}

		m_Tbl_TextFilter = new Tbl_TextFilter_Table( "Table/TextFilterTable");	yield return null; m_nCurTableLoadCount++;
		m_ZoneMapTable = new Tbl_ZoneMap_Table( "Table/ZoneMapTable");	yield return null; m_nCurTableLoadCount++;
		m_AreaMapTable = new Tbl_AreaMapTable( "Table/AreaMapTable");	yield return null; m_nCurTableLoadCount++;
		m_WarpDataTable = new Tbl_WarpData_Table( "Table/WarpTable");	yield return null; m_nCurTableLoadCount++;
		m_ProductionTable = new Tbl_Production_Table( "Table/ProductionTable");	yield return null; m_nCurTableLoadCount++;
		m_TechnicTable = new Tbl_Technic_Table( "Table/TechnicTable");	yield return null; m_nCurTableLoadCount++;
		m_BuffMinMaxTable = new Tbl_BuffMinMaxTable_Table( "Table/BuffMinMaxTable");	yield return null; m_nCurTableLoadCount++;
	#region -Designation
		AsDesignationManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
		AsDesignationRankRewardManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
	#endregion
		
		m_AttendBonusTable = new Tbl_AttendBonus_Table( "Table/AttendanceBonusTable");	yield return null; m_nCurTableLoadCount++;
		m_ReturnBonusTable = new Tbl_ReturnBonus_Table( "Table/ReturnBonusTable");	yield return null; m_nCurTableLoadCount++;
		m_CollectionTable = new Tbl_Collection_Table( "Table/CollectionTable");	yield return null; m_nCurTableLoadCount++;
		
	#region -GameGuide
		AsGameGuideManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
	#endregion
		
	#region -MonsterLines
		AsMonsterLineManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
	#endregion
		
	#region -DelegateImage
		AsDelegateImageManager.Instance.LoadTable();	yield return null; m_nCurTableLoadCount++;
	#endregion

		QuestTutorialMgr.Instance.LoadTutorial( "Table/QuestTutorial");
		
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			m_PreLoadTable = new Tbl_PreLoad_Table( "Table/PreLoadTable");
			yield return null; m_nCurTableLoadCount++;
		}
		
		m_InDunTable = new Tbl_InDun_Table( "Table/InDunTable"); yield return null; m_nCurTableLoadCount++;
		m_InsQuestGroupTable = new Tbl_InsQuestGroup_Table( "Table/InsQuestGroupTable"); yield return null; m_nCurTableLoadCount++;
		m_InsRewardTable = new Tbl_InsDungeonReward_Table( "Table/InDunRewardTable"); yield return null; m_nCurTableLoadCount++;
		
		m_ItemRankWeightTable = new Tbl_ItemRankWeightTable( "Table/ItemRankWeightTable"); yield return null; m_nCurTableLoadCount++;
		
		m_PvpGradeTable = new Tbl_PvpGrade_Table( "Table/PvpGradeTable"); yield return null; m_nCurTableLoadCount++;
		
		m_PromotionTable = new Tbl_Promotion_Table( "Table/PromotionTable");  yield return null; m_nCurTableLoadCount++;
		
		//pet
		m_PetTable = new Tbl_Pet_Table( "Table/PetDataTable");  yield return null; m_nCurTableLoadCount++;
		m_PetGroupTable = new Tbl_PetGroup_Table( "Table/PetGroupTable");  yield return null; m_nCurTableLoadCount++;;
		m_PetLevelTable = new Tbl_PetLevel_Table( "Table/PetLevelTable");  yield return null; m_nCurTableLoadCount++;;
		m_PetScriptTable = new Tbl_PetScript_Table( "Table/PetScriptTable");  yield return null; m_nCurTableLoadCount++;;
//		m_PetSkillTable = new Tbl_PetSkill_Table( "Table/PetSkillTable");  yield return null; m_nCurTableLoadCount++;;
		m_PetActionTable = new Tbl_PetAction_Table( "Table/PetActionDataList");  yield return null; m_nCurTableLoadCount++;;
		/////////////////////////////////////// load complete
		
		m_Tbl_Skill.InitBasicSkill();//$yde
		
		s_bTableLoaded = true;
		
		float fTime = Time.realtimeSinceStartup - fStartTime;
		Debug.Log( "TableManager::LoadTable(): " + fTime);

		// ================ FT MODIFY
		System.GC.Collect ();
	}
#else
	public void LoadTable()
	{
		float fStartTime = Time.realtimeSinceStartup;
		
		#region -Version
		VersionManager.Instance.LoadTable();
		#endregion
		
		m_Tbl_GlobalWeight = new Tbl_GlobalWeight_Table( "Table/GlobalWeightTable");

		// < ilmeda, 20120824
		GameObject main = GameObject.Find( "GameMain");
		if( null == main)	// dopamin skill_editor use
		{
			#region -skill_editor use
			m_Tbl_String = new Tbl_String_Table( "Table/StringTable_kor");
			m_Tbl_Class = new Tbl_Class_Table( "Table/ClassTable");
			m_Tbl_Npc = new Tbl_Npc_Table( "Table/NPCTable");
			m_Tbl_Monster = new Tbl_Monster_Table( "Table/MonsterTable");
			m_Tbl_CreateCharacter = new Tbl_CreateCharacter_Table( "Table/CreateCharacterTable");
			m_Tbl_SkillPotencyEffect = new Tbl_SkillPotencyEffect_Table( "Table/SkillPotencyEffectTable");
			if( null != ItemMgr.Instance)
			{
				ItemMgr.ItemManagement.LoadTable( "Table/ItemTable");
			}
			return;
			#endregion
		}
		else
		{
			AsGameMain asMain = main.GetComponent<AsGameMain>();
		
			GAME_LANGUAGE gameLanguage = asMain.GetGameLanguage();
			switch( gameLanguage)
			{
			case GAME_LANGUAGE.LANGUAGE_KOR:
				m_Tbl_String = new Tbl_String_Table( "Table/StringTable_kor");
				m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable");
				m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable");
				break;
			case GAME_LANGUAGE.LANGUAGE_JAP:
				m_Tbl_String = new Tbl_String_Table( "Table/StringTable_jap");
				m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable_Jap");
				m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable_Jap");
				break;
			}
		}
		// ilmeda, 20120824 >

		#region -LoadingTip
		AsLoadingTipManager.Instance.LoadTable();
		#endregion

		m_Tbl_Class = new Tbl_Class_Table( "Table/ClassTable");
		m_Tbl_Level = new Tbl_UserLevel_Table( "Table/UserLevelTable");
		m_Tbl_Npc = new Tbl_Npc_Table( "Table/NPCTable");
		m_Tbl_NormalNpc = new Tbl_NormalNpc_Table( "Table/NormalNpcTable");
		m_Tbl_Monster = new Tbl_Monster_Table( "Table/MonsterTable");
		m_Tbl_Object = new Tbl_Object_Table( "Table/ObjectTable");
		m_Tbl_SetItemTable = new Tbl_SetItem_Table( "Table/SetItemTable");
		m_Tbl_SkillPotencyEffect = new Tbl_SkillPotencyEffect_Table( "Table/SkillPotencyEffectTable");
		m_Tbl_Recall = new Tbl_Recall_Table( "Table/RecallTable");
		m_Tbl_Skill = new Tbl_Skill_Table( "Table/SkillTable");
		m_Tbl_SkillLevel = new Tbl_SkillLevel_Table( "Table/SkillLevelTable");
		m_Tbl_PvpSkillLevel = new Tbl_SkillLevel_Table( "Table/PvPSkillLevelTable");
		m_Tbl_Action = new Tbl_Action_Table( "Table/ActionDataList");
		m_Tbl_BuffOverlap = new Tbl_BuffOverlap_Table( "Table/BuffOverlapTable");
		m_Tbl_MonsterSkill = new Tbl_MonsterSkill_Table( "Table/MonsterSkillTable");
		m_Tbl_MonsterSkillLevel = new Tbl_MonsterSkillLevel_Table( "Table/MonsterSkillLevelTable");
		m_Tbl_MonsterAction = new Tbl_MonsterAction_Table( "Table/MonsterActionDataList");
		m_Tbl_MonsterChampion = new Tbl_MonsterChampion_Table( "Table/MonsterChampionTable");
		m_Tbl_CreateCharacter = new Tbl_CreateCharacter_Table( "Table/CreateCharacterTable");
		//m_Tbl_QuestTalk = new Tbl_QuestTalk_Table( "Table/QuestTalkTable");
		m_Tbl_QuestReward = new Tbl_QuestReward_Table( "Table/QuestRewardTable");
		m_Tbl_Quest = new Tbl_Quest_Table ( "Table/QuestTable");
		m_TblSuppertEnchantTable = new Tbl_SupportEnchantTable( "Table/SupportItemTable");
		m_TblSoulStoneEnchantTable = new Tbl_SoulStoneEnchant_Table( "Table/SoulStoneEnchantTable");
		m_StrengthenTable = new Tbl_Strengthen_Table( "Table/StrengthenTable");
		m_Tbl_SkillShop = new Tbl_SkillShop_Table( "Table/SkillShopTable");
		m_Tbl_SkillBook = new Tbl_SkillBook_Table( "Table/SkillBookTable");
		m_Tbl_RandomCoolTime = new Tbl_RandomCoolTime_Table( "Table/LotteryTypeTable");
		m_Tbl_RandItem = new Tbl_RandItem_Table( "Table/RandItemTable");
		m_Tbl_Lottery = new Tbl_Lottery_Table( "Table/LotteryTable");
		m_Tbl_Emotion = new Tbl_Emotion_Table( "Table/EmotionTable");
		m_Tbl_Emoticon = new Tbl_Emoticon_Table( "Table/EmoticonTable");
		//m_Tbl_QuestString = new Tbl_QuestString_Table( "Table/QuestStringTable");
		m_Tbl_CashStoreCostume = new Tbl_CashStoreCostume("Table/CashStoreSetTable_jap");
		m_QuestMoveTable = new Tbl_QuestMove("Table/QuestMoveTable");
		m_EventTable = new Tbl_Event_Table("Table/EventTable");
        m_ApRewardTable = new Tbl_ApRewardTable("Table/APRewardTable");
		
#if UNITY_IPHONE
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableIOS_Jap");
#elif UNITY_ANDROID
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableAOS_Jap");
#else
		m_ChargeTable = new Tbl_Charge_Table("Table/ChargeTableAOS_Jap");
#endif


		m_TblSynMixEnchantTable = new Tbl_SynMixEnchant_Table("Table/MixEnchantTable");
		m_TblSynOptionChangeTable = new Tbl_SynOptionChange_Table("Table/OptionChangeTable");
		m_TblSynDisassembleTable = new Tbl_SynDisassemble_Table("Table/DisassembleTable");
		m_TblsynCosMixTable = new Tbl_SynCosMix_Table("Table/CosMixTable");
		
		if( null != TerrainMgr.Instance)
			TerrainMgr.Instance.LoadTable();

		if( null != AsEntityManager.Instance)
			AsEntityManager.Instance.LoadEntityTemplates( "EntityTemplate/EntityTemplate");

		if( null != ItemMgr.Instance)
		{
			ItemMgr.ItemManagement.LoadTable( "Table/ItemTable");
			m_Tbl_Store = new Tbl_Store_Table( "Table/NpcStoreTable");
			m_Tbl_CachStore = new Tbl_Cash_Store_Table( "Table/CashStoreTable_jap");
		}

		m_Tbl_TextFilter = new Tbl_TextFilter_Table( "Table/TextFilterTable");
		m_ZoneMapTable = new Tbl_ZoneMap_Table( "Table/ZoneMapTable");
		m_AreaMapTable = new Tbl_AreaMapTable( "Table/AreaMapTable");
		m_WarpDataTable = new Tbl_WarpData_Table( "Table/WarpTable");
		m_ProductionTable = new Tbl_Production_Table( "Table/ProductionTable");
		m_TechnicTable = new Tbl_Technic_Table( "Table/TechnicTable");
		m_BuffMinMaxTable = new Tbl_BuffMinMaxTable_Table( "Table/BuffMinMaxTable");
		#region -Designation
		AsDesignationManager.Instance.LoadTable();
		AsDesignationRankRewardManager.Instance.LoadTable();
		#endregion
		m_AttendBonusTable = new Tbl_AttendBonus_Table( "Table/AttendanceBonusTable");
		m_ReturnBonusTable = new Tbl_ReturnBonus_Table( "Table/ReturnBonusTable");
		m_CollectionTable = new Tbl_Collection_Table( "Table/CollectionTable");
		#region -GameGuide
		AsGameGuideManager.Instance.LoadTable();
		#endregion
		#region -MonsterLines
		AsMonsterLineManager.Instance.LoadTable();
		#endregion
		#region -DelegateImage
		AsDelegateImageManager.Instance.LoadTable();
		#endregion

		QuestTutorialMgr.Instance.LoadTutorial( "Table/QuestTutorial");
		
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			m_PreLoadTable = new Tbl_PreLoad_Table( "Table/PreLoadTable");
		
		m_InDunTable = new Tbl_InDun_Table( "Table/InDunTable");
		m_InsQuestGroupTable = new Tbl_InsQuestGroup_Table( "Table/InsQuestGroupTable");
		m_InsRewardTable = new Tbl_InsDungeonReward_Table( "Table/InDunRewardTable");
		
		m_ItemRankWeightTable = new Tbl_ItemRankWeightTable( "Table/ItemRankWeightTable");
		
		m_PvpGradeTable = new Tbl_PvpGrade_Table( "Table/PvpGradeTable");
		
		m_PromotionTable = new Tbl_Promotion_Table( "Table/PromotionTable");
		
		//pet
		m_PetTable = new Tbl_Pet_Table( "Table/PetDataTable");
		m_PetGroupTable = new Tbl_PetGroup_Table( "Table/PetGroupTable");
		m_PetLevelTable = new Tbl_PetLevel_Table( "Table/PetLevelTable");
		m_PetScriptTable = new Tbl_PetScript_Table( "Table/PetScriptTable");
//		m_PetSkillTable = new Tbl_PetSkill_Table( "Table/PetSkillTable");
		m_PetActionTable = new Tbl_PetAction_Table( "Table/PetActionDataList");
		/////////////////////////////////////// load complete
		
		m_Tbl_Skill.InitBasicSkill();//$yde
		
		s_bTableLoaded = true;
		
		float fTime = Time.realtimeSinceStartup - fStartTime;
		Debug.Log( "TableManager::LoadTable(): " + fTime);
	}
#endif
	#endregion
	
	public void SpawnToolLoadTable()
	{
		m_Tbl_Npc = new Tbl_Npc_Table( "Table/NPCTable");
		m_Tbl_NormalNpc = new Tbl_NormalNpc_Table( "Table/NormalNpcTable");
		m_Tbl_Monster = new Tbl_Monster_Table( "Table/MonsterTable");
		m_Tbl_Object = new Tbl_Object_Table( "Table/ObjectTable"); 
		m_Tbl_Recall = new Tbl_Recall_Table( "Table/RecallTable");
	}

	#region - get record -
//		public Dictionary<string, AsEntityTemplate> GetEntityTemplate()
//		{
//			return m_Tbl_EntityTemplate.GetEntityTemplate();
//		}
//		public Tbl_EntityTemplate_Record GetEntityTemplateRecord( string _type)
//		{
//			return m_Tbl_EntityTemplate.GetRecord( _type);
//		}
	#region  - global -
	public Tbl_GlobalWeight_Record GetTbl_GlobalWeight_Record( int _id)
	{
		return m_Tbl_GlobalWeight.GetRecord( _id);
	}
	
	public Tbl_GlobalWeight_Record GetTbl_GlobalWeight_Record( string _id)
	{
		return m_Tbl_GlobalWeight.GetRecord( _id);
	}

	public Tbl_String_Record GetTbl_String_Record( int _id)
	{
		return m_Tbl_String.GetRecord( _id);
	}
	
	public string GetTbl_String( int _id)
	{
		Tbl_String_Record record = m_Tbl_String.GetRecord( _id);
		if( null == record)
			return "can't find string id : " + _id;
		
		return record.String;
	}
	#endregion

	#region - base -
//	public Tbl_Tribe_Record GetTbl_Tribe_Record( int _id)
//	{
//		return m_Tbl_Tribe.GetRecord( _id);
//	}
//	public Tbl_Tribe_Record GetTbl_Tribe_Record( eRACE _tribe, eCLASS _class)
//	{
//		return m_Tbl_Tribe.GetRecordByTribeAndClass( _tribe, _class);
//	}
	
	public Tbl_Class_Record GetTbl_Class_Record( int _id)
	{
		return m_Tbl_Class.GetRecord( _id);
	}
	
	public Tbl_Class_Record GetTbl_Class_Record( eRACE _race, eCLASS _class)
	{
		return m_Tbl_Class.GetRecordByRaceAndClass( _race, _class);
	}
	
	public Tbl_Class_Record GetTbl_Class_Record( eCLASS _class)
	{
		return m_Tbl_Class.GetRecordByRaceAndClass( _class);
	}
	
	public Tbl_UserLevel_Record GetTbl_Level_Record( eCLASS _class, int _level)
	{
		return m_Tbl_Level.GetRecord( _class, _level);
	}
	
	public Tbl_Npc_Record GetTbl_Npc_Record( int _id)
	{
		return m_Tbl_Npc.GetRecord( _id);
	}
	
	public Tbl_NormalNpc_Record GetTbl_NormalNpc_Record( int _id)
	{
		return m_Tbl_NormalNpc.GetRecord( _id);
	}
	
	public Tbl_Monster_Record GetTbl_Monster_Record( int _id)
	{
		return m_Tbl_Monster.GetRecord( _id);
	}
	
	public int GetNpcIdByMonsterKindId( int nMonsterKindId)
	{
		return m_Tbl_Monster.GetNpcIdByMonsterKindId( nMonsterKindId);
	}
	
	public Tbl_MonsterChampion_Record GetTbl_MonsterChampion_Record( int _id)
	{
		return m_Tbl_MonsterChampion.GetRecord( _id);
	}
	
	public Tbl_Object_Record GetTbl_Object_Record( int _id)
	{
		return m_Tbl_Object.GetRecord( _id);
	}
	
	public Tbl_SetItem_Record GetTbl_SetItem_Record( int _id)
	{
		return m_Tbl_SetItemTable.GetRecord( _id);
	}

	public List<Tbl_SetItem_Record> GetTbl_SetItem_Record_All()
	{
		return m_Tbl_SetItemTable.GetRecordAll();
	}

	public Tbl_CashStoreCostume_Record[] GetTbl_CashstoreCostume_Record_All()
	{
		return m_Tbl_CashStoreCostume.GetCashStoreCostumeRecordAll();
	}	
	
	public Tbl_CashStoreCostume_Record GetTbl_CashstoreCostume_Record(int _setID)
	{
		return m_Tbl_CashStoreCostume.GetCashStoreCostumeRecord(_setID);
	}
	#endregion

	#region - skill potency -
//	public Tbl_SkillPotency_Record GetTbl_SkillPotency_Record( int _id)
//	{
//		return m_Tbl_SkillPotency.GetRecord( _id);
//	}

	public Tbl_SkillPotencyEffect_Table GetTbl_SkillPotencyEffect_Table()
	{
		return m_Tbl_SkillPotencyEffect;
	}
	
	public Tbl_SkillPotencyEffect_Record GetTbl_SkillPotencyEffect_Record( int _id)
	{
		return m_Tbl_SkillPotencyEffect.GetRecord( _id);
	}
	
	public Tbl_Recall_Record GetTbl_Recall_Record( int _id)
	{
		return m_Tbl_Recall.GetRecord( _id);
	}

	public SortedList<int, Tbl_Recall_Record> GetRecallList()
	{
		return m_Tbl_Recall.GetList();
	}

	public Tbl_BuffOverlap_Record GetTbl_BuffOverlap_Record( eBUFFTYPE _type)
	{
		return m_Tbl_BuffOverlap.GetRecord( _type);
	}
	#endregion

	#region - player skill -
	public Tbl_Skill_Record GetTbl_Skill_Record( int _id)
	{
		return m_Tbl_Skill.GetRecord( _id);
	}
	
	public Tbl_Skill_Record GetTbl_Skill_Record( eCLASS _class, eSKILL_TYPE _type, eCommand_Type _inputType)
	{
		return m_Tbl_Skill.GetSkillByType( _class, _type, _inputType);
	}
	
	public List<Tbl_Skill_Record> GetTbl_Skill_RecordsByClass( eCLASS _class)
	{
		return m_Tbl_Skill.GetSkillsByClass( _class);
	}
	
	public Tbl_Skill_Record GetRandomBaseSkill( eCLASS _class)
	{
		return m_Tbl_Skill.GetRandomBaseSkill( _class);
	}
	
	public SuitableBasicSkill GetSuitableBasicSkill( eCLASS _class, float _dist)
	{
		return m_Tbl_Skill.GetSuitableBasicSkill( _class, _dist);
	}
	
//	public Tbl_Skill_Record GetTbl_Skill_RecordByPickingType( eCLASS _class, eCommandPicking_Type _type)
//	{
//		return m_Tbl_Skill.GetDoubleTabSkillByPickingType( _class, _type);
//	}

	public Tbl_SkillLevel_Record GetTbl_SkillLevel_Record( int _id)
	{
		if( true == TargetDecider.CheckCurrentMapIsArena())
			return m_Tbl_PvpSkillLevel.GetRecord( _id);
		
		return m_Tbl_SkillLevel.GetRecord( _id);
	}
	
	public Tbl_SkillLevel_Record GetTbl_SkillLevel_Record( int _skillLv, int _skillIdx)
	{
		if( true == TargetDecider.CheckCurrentMapIsArena())
			return m_Tbl_PvpSkillLevel.GetRecord( _skillLv, _skillIdx);
		
		return m_Tbl_SkillLevel.GetRecord( _skillLv, _skillIdx);
	}
	
	public Tbl_SkillLevel_Record GetTbl_SkillLevel_Record( int _skillLv, int _skillIdx, int _chargeStep)
	{
		if( true == TargetDecider.CheckCurrentMapIsArena())
			return m_Tbl_PvpSkillLevel.GetRecord( _skillLv, _skillIdx, _chargeStep);
		
		return m_Tbl_SkillLevel.GetRecord( _skillLv, _skillIdx, _chargeStep);
	}
	
	public Tbl_SkillLevel_Table GetTbl_SkillLevel_Table()
	{
		if( true == TargetDecider.CheckCurrentMapIsArena())
			return m_Tbl_PvpSkillLevel;
		
		return m_Tbl_SkillLevel;
	}
	
	public Tbl_Action_Record GetTbl_Action_Record( int _id)
	{
		return m_Tbl_Action.GetRecord( _id);
	}
	
	public Tbl_Action_Record GetTbl_Action_Record( eCLASS _class, eGENDER _gender, string _act)
	{
		return m_Tbl_Action.GetRecord( _class, _gender, _act);
	}
	
	public Tbl_Action_Animation GetTbl_Action_Animation( eCLASS _class, eGENDER _gender, string _animName)
	{
		return m_Tbl_Action.GetActionAnimation( _class, _gender, _animName);
	}
	
//	public Tbl_Projectile_Record GetTbl_Projectile_Record( int _id)
//	{
//		return m_Tbl_Projectile.GetRecord( _id);
//	}
	#endregion

	#region - monster skill -
	public Tbl_MonsterSkill_Record GetTbl_MonsterSkill_Record( int _id)
	{
		return m_Tbl_MonsterSkill.GetRecord( _id);
	}
	
	public Tbl_MonsterSkillLevel_Record GetTbl_MonsterSkillLevel_Record( int _id)
	{
		return m_Tbl_MonsterSkillLevel.GetRecord( _id);
	}
	
	public Tbl_MonsterSkillLevel_Record GetTbl_MonsterSkillLevel_Record( int _skillLv, int _id)
	{
		return m_Tbl_MonsterSkillLevel.GetRecord( _skillLv, _id);
	}
	
	public Tbl_Action_Record GetTbl_MonsterAction_Record( int _id)
	{
		return m_Tbl_MonsterAction.GetRecord( _id);
	}
	
	public Tbl_Action_Record GetTbl_MonsterAction_Record( string _class, string _act)
	{
		return m_Tbl_MonsterAction.GetRecord( _class, _act);
	}
	
	public Tbl_Action_Animation GetTbl_MonsterAction_Animation( string _class, string _animName)
	{
		return m_Tbl_MonsterAction.GetActionAnimation( _class, _animName);
	}
	
//	public Tbl_MonsterPassive_Record GetTbl_MonsterPassive_Record( int _id)
//	{
//		return m_Tbl_MonsterPassive.GetRecord( _id);
//	}
	#endregion

	#region - create character -
//	public Tbl_CreateCharacter_Record GetTbl_CreateCharacter_Record( int _id)
//	{
//		return m_Tbl_CreateCharacter.GetRecord( _id);
//	}
	public AsCharacterCreateGenderData GetTbl_CreateCharacter_GenderData( eRACE _race, eCLASS _class, eGENDER _gender)
	{
		return m_Tbl_CreateCharacter.GetGenderData( _race, _class, _gender);
	}
	
	public AsCharacterCreateClassData GetTbl_CreateCharacter_ClassData( eRACE _race, eCLASS _class)
	{
		return m_Tbl_CreateCharacter.GetClassData( _race, _class);
	}
	
	public AsCharacterCreateData GetTbl_CreateCharacter_RaceData( eRACE _race)
	{
		return m_Tbl_CreateCharacter.GetRaceData( _race);
	}
	
//	public Tbl_CreateCharacter_Record GetTbl_CreateCharacter_Record( eRACE _tribe, eCLASS _class)
//	{
//		return m_Tbl_CreateCharacter.GetRecordByTribeAndClass( _tribe, _class);
//	}
	#endregion

	#region - SkillEditor
	/// dopamin begin
	public SortedList<int, Tbl_Class_Record> GetClassList()
	{
		return m_Tbl_Class.GetList();
	}

	public SortedList<int, Tbl_Npc_Record> GetNpcList()
	{
		return m_Tbl_Npc.GetList();
	}
	/// dopamin end
	#endregion// - SkillEditor

	#region - quest -
	public Tbl_QuestReward_Record GetTbl_QuestRewardRecord( int _id)
	{
		return m_Tbl_QuestReward.GetRecord( _id);
	}

	public Tbl_QuestReward_Record[] GetTbl_QuestRewardRecords( int _id)
	{
		return m_Tbl_QuestReward.GetRecordAll();
	}

	public Tbl_QuestTalk_Record GetTbl_QuestTalk( int _id)
	{
		return m_Tbl_QuestTalk.GetRecord( _id);
	}

	public Tbl_QuestTalk_Record[] GetTbl_QuestTalks()
	{
		return m_Tbl_QuestTalk.GetRecordAll();
	}

	public void SetTbl_QuesrRecordState( int _id, QuestProgressState _state, int _repeat = -1)
	{
		m_Tbl_Quest.SetQuestRecordState( _id, _state, _repeat);
	}

	public void ResetTbl_QuesrRecord( int _id)
	{
		m_Tbl_Quest.ResetQuestRecord( _id);
	}

	public void ResetTbl_Quests()
	{
		m_Tbl_Quest.ResetQuests();
	}

	public Tbl_Quest_Record GetTbl_QuestRecord( int _id)
	{
		return m_Tbl_Quest.GetRecord( _id);
	}

	public Dictionary<int, PrepareOpenUIType> GetTbl_PrepareOpenUI(eCLASS _eClass)
	{
		return m_Tbl_Quest.GetPrepareOpenUI(_eClass);
	}

	public void Reset_Complete_Daily_Quests()
	{
		m_Tbl_Quest.ResetCompleteDailyRecords();
	}

	public Tbl_Quest_Record[] GetTbl_QuestRecordsByNpc( int _id, QuestTableType _tableType)
	{
		return m_Tbl_Quest.GetRecordByID( _id, _tableType);
	}

	public List<int> GetTbl_QuestIDByNpcList( int _id, QuestTableType _tableType)
	{
		List<int> list = new List<int>();

		Tbl_Quest_Record[] records = GetTbl_QuestRecordsByNpc( _id, _tableType);

		if( records != null)
		{
			foreach (Tbl_Quest_Record record in records)
			{
				if (record.QuestDataInfo.Info.QuestType == QuestType.QUEST_NPC_DAILY)
					if (record.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_NOTHING)
						continue;

				list.Add(record.QuestDataInfo.Info.ID);
			}
		}

		return list;
	}

	public Tbl_Quest_Record[] GetTbl_QuestRecordsByNpc( int _id)
	{
		return m_Tbl_Quest.GetRecordByID( _id);
	}

	public Tbl_Quest_Record[] GetTbl_QuestRecords()
	{
		return m_Tbl_Quest.GetRecordAll();
	}

	public QuestData[] GetTbl_QuestDatas()
	{
		List<QuestData> listQuests = new List<QuestData>();
		Tbl_Quest_Record[] questRecords = m_Tbl_Quest.GetRecordAll();
		foreach ( Tbl_Quest_Record record in questRecords)
			listQuests.Add( record.QuestDataInfo);

		return listQuests.ToArray();
	}

	public QuestData GetTbl_QuestData( int _questID)
	{
		Tbl_Quest_Record questRecord = m_Tbl_Quest.GetRecord( _questID);

		if ( questRecord != null)
			return questRecord.QuestDataInfo;
		else
			return null;
	}

	public Tbl_QuestStringRecord[] GetTbl_QuestString_RecordAll()
	{
		if( m_Tbl_QuestString == null)
			return null;
		else
			return m_Tbl_QuestString.GetRecordAll();
	}

	public Tbl_QuestStringRecord GetTbl_QuestString_Record( int _id)
	{
		if( m_Tbl_QuestString == null)
			return null;
		else
			return m_Tbl_QuestString.GetRecord( _id);
	}

	public void SetTbl_QuestString( QuestStringInfo _info)
	{
		m_Tbl_QuestString.SetTblQuestStringRecord( _info);
	}

	public bool IsUseItemToTargetEntity( int _npcTableID)
	{
		return m_Tbl_Quest.IsContainUseItemToTargetID( _npcTableID);
	}

	public Tbl_QuestMove_Record GetTbl_QuestMove_Record(int _questID)
	{
		return m_QuestMoveTable.Get_Tbl_QuestMove_Record(_questID);
	}
	#endregion
	
	#region -in game event-
	public List<Tbl_Event_Record> GetTbl_Event(int _npcID, System.DateTime _date)
	{
		return m_EventTable.GetRecord(_npcID, _date);	
	}
	
	public List<Tbl_Event_Record> GetTbl_Event(System.DateTime date, bool _checkNpcView = false)
	{
		return m_EventTable.GetRecordAll(date, _checkNpcView);	
	}

	public List<Tbl_Event_Record> GetTbl_EventCanNotProgress(System.DateTime date, bool _checkNpcView = false)
	{
		return m_EventTable.GetRecordAllCanNotProgress(date, _checkNpcView);
	}

	public Tbl_Event_Record GetTbl_Event(int _eventID)
	{
		return m_EventTable.GetRecord(_eventID);
	}
	
	public bool CheckViewEventNpc(int _npcTableID)
	{
		return m_EventTable.CheckEventViewNpc(_npcTableID);
	}

	public bool CheckViewEventNpcAppear(int _npcTableID, System.DateTime _date)
	{
		return m_EventTable.CheckEventViewNpcAppear(_npcTableID, _date);
	}

	#endregion
	
	#region  - store -
	public List<Store_Item_Info_Table> GetStoreItems( eCLASS _eClass, eStore_ItemKind _eStoreItemKind, int _npcID)
	{
		return m_Tbl_Store.GetStoreItemInfo( _eClass, _eStoreItemKind, _npcID);
	}

	public List<eStore_ItemKind> GetHaveItemKind( int _npcID, eCLASS _class)
	{
		return m_Tbl_Store.GetHaveItemKind( _npcID, _class);
	}

	public Store_Item_Info_Table GetStoreItem(int _npcID, int _itemID)
	{
		return m_Tbl_Store.GetStoreInfo(_npcID, _itemID);
	}
	#endregion

	#region -cash store-
	public List<Store_Item_Info_Table> GetCashStoreItems(eCLASS _eClass, eCashStoreMainCategory _mainCategory, eCashStoreSubCategory _subCategory)
	{
		return m_Tbl_CachStore.GetCashStoreItemInfo(_eClass, _mainCategory, _subCategory);
	}

	public List<Store_Item_Info_Table> GetCashStoreItems( params int[] _storeItemIDs)
	{
		return m_Tbl_CachStore.GetCashStoreItemInfo( _storeItemIDs);
	}

	public List<eCashStoreMainCategory> GetHaveCashItemKind(eCLASS _class)
	{
		return m_Tbl_CachStore.GetHaveItemKind( _class);
	}

	public Tbl_ChargeRecord GetChargeRecord(string _id)
	{
		return m_ChargeTable.GetRecord(_id);
	}

	public Tbl_ChargeRecord[] GetChargeRecords()
	{
		return m_ChargeTable.GetRecordAll();
	}

	public int GetChargeRecordSlotNumber(string _id)
	{
		Tbl_ChargeRecord[] records = m_ChargeTable.GetRecordAll();

		int count = 0;

		foreach(Tbl_ChargeRecord record in records)
		{
			if (record.itemID == _id)
				return count;

			count++;
		}

		return -1;
	}
	#endregion
	
	public Tbl_SupportEnchantTable GetSupportEnchantTable()
	{
		return m_TblSuppertEnchantTable;
	}
	
	public Tbl_SoulStoneEnchant_Record GetSoulStoneEnchantTable( int id)
	{
		return m_TblSoulStoneEnchantTable.GetRecord( id);
	}
		
	public Tbl_Strengthen_Table GetStrengthenTable()
	{
		return m_StrengthenTable;
	}
	
	public Tbl_SynMixEnchant_Record GetSynMixEnchantRecord( bool _isSkill, int level_1, Item.eGRADE _eGrade_1, Tbl_SoulStoneEnchant_Record.eTYPE _type_1,
		int level_2, Item.eGRADE _eGrade_2, Tbl_SoulStoneEnchant_Record.eTYPE _type_2)
	{
		return m_TblSynMixEnchantTable.GetRecord( _isSkill, level_1, _eGrade_1, _type_1, level_2, _eGrade_2, _type_2 );
	}
	
	
	public Tbl_SynOptionChange_Record GetSynOptionChangeRecord( int level, Item.eGRADE _eGrade, Item.eEQUIP _equip )
	{
		return m_TblSynOptionChangeTable.GetRecord( level, _eGrade, _equip );
	}
	
	
	public Tbl_SynDisassemble_Record GetSynDisassembleRecord( int level, Item.eGRADE _eGrade, Item.eEQUIP _equip, int _step, eItem_Disassemble _type )
	{
		return m_TblSynDisassembleTable.GetRecord( level, _eGrade, _equip, _step, _type );
	}
	
	public Tbl_SynCosMix_Record GetSynCosMixRecord( Item.eGRADE _grade , Item.eEQUIP _equip )
	{
		return m_TblsynCosMixTable.GetRecord( _grade, _equip );
	}
	
	#region - skill shop & book -
	public Tbl_SkillShop_Record GetTbl_SkillShop_Record( int _id)
	{
		return m_Tbl_SkillShop.GetRecord( _id);
	}
	
	public Tbl_SkillBook_Record GetTbl_SkillBook_Record( int _id)
	{
		return m_Tbl_SkillBook.GetRecord( _id);
	}
	
	public Tbl_SkillBook_Record GetTbl_SkillBook_Record( eCLASS _class, int _skillIdx, int _skillLv)
	{
		return m_Tbl_SkillBook.GetRecord( _class, _skillIdx, _skillLv);
	}
	
	public Dictionary<int, Tbl_SkillBook_Record> GetTbl_SkillBook_RecordsByClass( eCLASS _class)
	{
		return m_Tbl_SkillBook.GetRecordsByClass( _class);
	}
	
	public Tbl_RandomCoolTime_Record GetTbl_RandomCoolTime_Record( int _iItemId)
	{
		return m_Tbl_RandomCoolTime.GetRecord( _iItemId);
	}
	
	public Tbl_RandItem_Record GetTbl_RandItem_Record( int _iIndex)
	{
		return m_Tbl_RandItem.GetRecord( _iIndex);
	}
	
	public Tbl_Lottery_Record GetTbl_Lottery_Record( int _iIndex)
	{
		return m_Tbl_Lottery.GetRecord( _iIndex);
	}
	
	public List<Tbl_SkillBook_Record> GetTbl_SkillBook_RecordsByClass( int _npcIdx, eCLASS _class)
	{
		return m_Tbl_SkillShop.GetRecordsByClass( _npcIdx, _class);
	}
	#endregion

    #region -Ap Reward-
    
    public List<ApRewardInfoLineup> GetTbl_ApRewardInfoLineup(eCLASS _class, int _group)
    {
        return m_ApRewardTable.GetAllRewardInfoLineup(_class, _group);
    }

    public List<ApRewardInfo> GetTbl_ApRewardInfoList(eCLASS _class, uint _rank, int _group)
    {
        return m_ApRewardTable.GetRewardInfoList(_class, _rank, _group);
    }

    #endregion

    #region - emotion -
    public Tbl_Emotion_Record GetValidEmotionRecord( string _str)
	{
		return m_Tbl_Emotion.GetValidEmotionRecord( _str);
	}
	
	public List<string> GetCommandList()
	{
		return m_Tbl_Emotion.GetCommandList();
	}
	
	public Tbl_Emoticon_Record GetTbl_Emoticon_Record( int _idx)
	{
		return m_Tbl_Emoticon.GetRecord( _idx);
	}
	
	public List<Tbl_Emoticon_Record> GetHuntEmoticons()
	{
		return m_Tbl_Emoticon.listHunt;
	}
	
	public List<Tbl_Emoticon_Record> GetNormalEmoticons()
	{
		return m_Tbl_Emoticon.listNormal;
	}
	
	public Tbl_Emoticon_Record GetEmoticonByCondition(eAutoCondition _condition)
	{
		return m_Tbl_Emoticon.GetRecordByCondition(_condition);
	}
	#endregion
	
	#region - pet -
	public Tbl_Pet_Record GetPetRecord( int _idx)
	{
		return m_PetTable.GetRecord( _idx);
	}
	
	public Tbl_PetLevel_Record GetPetLevelRecord( int _lv)
	{
		return m_PetLevelTable.GetRecord( _lv);
	}
	
	public Tbl_PetScript_Record GetPetScriptRecord( int _idx)
	{
		return m_PetScriptTable.GetRecord( _idx);
	}
	
	public Tbl_Action_Record GetPetActionRecord( int _idx)
	{
		return m_PetActionTable.GetRecord( _idx);
	}
	
	public Tbl_Action_Record GetPetActionRecord( string _class, string _act)
	{
		return m_PetActionTable.GetRecord( _class, _act);
	}
	
	public Tbl_Action_Animation GetPetActionAnimation( string _class, string _animName)
	{
		return m_PetActionTable.GetActionAnimation( _class, _animName);
	}
	#endregion
	#endregion

	#region - public method -
	public void Shutdown( string _log)
	{
		AsUtil.ShutDown( _log);
	}
	
	public bool TextFiltering_PStoreContent( string _name)
	{
		return m_Tbl_TextFilter.CheckFilter_PStoreContent( _name);
	}
	
	public bool TextFiltering_Name( string _name)
	{
		return m_Tbl_TextFilter.CheckFilter_Name( _name);
	}
	
	public string TextFiltering_Chat( string _chat)
	{
		return m_Tbl_TextFilter.TextFiltering_Chat( _chat);
	}
	
	public string TextFiltering_Balloon( string _chat)
	{
		return m_Tbl_TextFilter.TextFiltering_Balloon( _chat);
	}
	
	public bool TextFiltering_Post( string strPost)
	{
		return m_Tbl_TextFilter.CheckFilter_Post( strPost);
	}
	
	public bool TextFiltering_Guild( string text)
	{
		return m_Tbl_TextFilter.CheckFilter_Guild( text);
	}
	
	public Tbl_ZoneMap_Record GetZoneMapRecord( int iIdx)
	{
		return m_ZoneMapTable.GetRecord( iIdx);
	}
	
	public Tbl_AreaMap_Record GetAreaMapRecord( int iIdx)
	{
		return m_AreaMapTable.GetRecord( iIdx);
	}
	
	public Tbl_WarpData_Record GetWarpDataRecord( int iIdx)
	{
		return m_WarpDataTable.GetRecord( iIdx);
	}
	
	public Tbl_WarpData_Record GetWarpDataIdxToUseMapID( int iMapId)
	{
		return m_WarpDataTable.GetWaypointData( iMapId);
	}
	
	public Tbl_WarpData_Table GetWarpDataTable()
	{
		return m_WarpDataTable;
	}
	
	public Tbl_Production_Table GetProductionTable()
	{
		return m_ProductionTable;
	}
	
	public Tbl_BuffMinMaxTable_Table GetBuffMinMaxTable()
	{
		return m_BuffMinMaxTable;
	}
	
	public Tbl_Technic_Table GetTechnicTable()
	{
		return m_TechnicTable;
	}
	
	public Tbl_PreLoad_Record GetPreLoadRecord( int iMapID)
	{
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			return m_PreLoadTable.GetRecord( iMapID);
		return null;
	}
	
	public Tbl_InDun_Record GetInDunRecord( int nID)
	{
		return m_InDunTable.GetRecord( nID);
	}
	
	public Tbl_InDun_Table GetInDunTable()
	{
		return m_InDunTable;
	}
	
	public Tbl_InsQuestGroup_Record GetInsQuestGroupRecord( int nID)
	{
		return m_InsQuestGroupTable.GetRecord( nID);
	}
	
	public Tbl_InsDungeonReward_Record GetInsRewardRecord( int nID)
	{
		return m_InsRewardTable.GetRecord( nID);
	}

	public Dictionary<int, Tbl_InsDungeonReward_Record> GetInsRewardRecordList()
	{
		return m_InsRewardTable.GetInsRewardRecordList();
	}
	
	public Tbl_PvpGrade_Record GetPvpGradeRecord( int nID)
	{
		return m_PvpGradeTable.GetRecord( nID);
	}
	
	public Tbl_PvpGrade_Table GetPvpGradeTable()
	{
		return m_PvpGradeTable;
	}
	
	public Tbl_ItemRankWeight_Record GetTblItemRankWeightRecord( int ilevel, eITEM_EFFECT type)	
	{
		return m_ItemRankWeightTable.GetRecord( ilevel, type);
	}
	
	public Tbl_Promotion_Table GetTbl_PromotionTable()
	{
		return m_PromotionTable;
	}
	#endregion
}
