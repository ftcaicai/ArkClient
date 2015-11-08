using System;
using System.Collections.Generic;
using System.Text;

public enum DailyQuestLevel
{
    EASY,
    NORMAL,
    HARD,
    SPECIAL,
    MAX,
}

public enum QuestProgressStateNet
{
    QUEST_NOTHING = 0,
    QUEST_START,
    QUEST_DROP,
    QUEST_CLEAR,
    QUEST_CLEAR_IMMEDIATELY,
    QUEST_COMPLETE,
    QUEST_FAIL,
    QUEST_WANTED_NEW,
    QUEST_MAX
};

public enum QuestClearType
{
    NONE,
    GOLD,
    CASH,
    MAX,
};


public enum QuestProgressState
{
    QUEST_PROGRESS_NOTHING,
    QUEST_PROGRESS_IN,
    QUEST_PROGRESS_CLEAR,
    QUEST_PROGRESS_IMMEDIATELY_CLEAR,
    QUEST_PROGRESS_COMPLETE,
    QUEST_PROGRESS_FAIL,
    QUEST_PROGRESS_MAX
}

public enum QuestType
{
    QUEST_NONE,
    QUEST_MAIN,
    QUEST_FIELD,
    QUEST_WANTED,
    QUEST_DAILY,
    QUEST_BOSS,
	QUEST_PVP,
	QUEST_NPC_DAILY,
    QUEST_MAX,
}

public enum QuestGuideDirection : int
{
    NONE = -1,
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    CENTER,
    MAX,
}

public enum ItemConditionType : int
{
    HAVE = 0,
    USE,
    MAX,
}

public enum SkillConditionType : int
{
    HAVE = 0,
    USE,
    MAX,
}

public enum QuestPassType : int
{
    AND = 0,
    OR,
    MAX,
}

public enum CheerType
{
    GET,
    SEND,
    MAX,
}

public enum OpenUIType
{
	PRODUCTION,
	INTENSIFY,
	SOCKET,
	MINIMAP,
	WAYPOINT,
	CHARACTER_INFO,
	OPEN_DAILY_QUEST,
	OPEN_LOCATION_MAP,
	OPEN_INVEN_WEB,
	OPEN_CHANNEL,
	OPEN_EMOTICON,
	OPEN_SOCIAL_STORE,
	OPEN_FRIEND_RANDOM,
	OPEN_REPRESENTATIVE_IMAGE,
	COSTUME_ON_OFF,
	SEARCH_PARTY_MATCHING,
	USE_TWO_TOUCH_MOVE,
	OPEN_FACEBOOK,
	AUTO_BATTLE,
	OPEN_GUIDE,
	OPEN_INDUN,
	MAX,
}

public enum UIExtensionType
{
    //SUCCESS_INTENSIFY,
    FAIL_INTENSIFY,
    INSERT_SOCKET,
    MAX,
}

public enum PostingSnsType
{
	TWITTER_OR_FACEBOOK,
	TWITTER,
	FACEBOOK,
	MAX,
}

public enum ePvpMatchMode
{
	NOTHING,
	RANKING,
	TRAINNING,
	MAX,
}

public struct QuestToolDefine
{
    public const int MaxAchievement = 5;
    public const int Gap            = 5;

    public static string GetMainPath()
    {
        string path = string.Empty;
#if DEBUG
        path = "../../";
#else
        path = "";
#endif

        return path;
    }
}

public struct XmlFileExtension
{
    public const string Extension = ".xml";
}

public struct QuestToolString
{
    public const string QuestTableFileName        = "QuestTable.xml";
    public const string QuestTalkTableFileName    = "QuestTalkTable.xml";
    public const string QuestRewardTalbleFileName = "QuestRewardTable.xml";
    public const string QuestStringTableFileName  = "QuestStringTable.xml";
    public const string QuestDiveideDirectory     = "/DivideTable/";
}

public struct QuestFileNames
{
    public const string QuestMain   = "QuestMainTable_";
    public const string QuestSub    = "QuestSubTable_";
    public const string QuestDaily  = "QuestDailyTable_";
    public const string QuestString = "QuestStringTable_";
    public const string QuestTalk   = "QuestTalkTable_";
}

public struct AchievementTypeString
{
    public const string GetItem               = "GetItem";
    public const string GetItemFromMonster    = "GetItemFromMonster";
    public const string GetItemFromMonsterKind= "GetItemFromMonsterKind";
    public const string GetLevel              = "GetLevel";
    public const string IntoMap               = "MoveMap";
    public const string KillMonster           = "KillMonster";
    public const string KillMonsterKind       = "KillMonsterKind";
    public const string TalkWithNPC           = "TalkWithNPC";
    public const string TimeSurvival          = "TimeSurvival";
    public const string TimeLimit             = "TimeLimit";
    public const string UseItem               = "UseItem";
    public const string UseItemInMap          = "UseItemInMap";
    public const string UseSkill              = "UseSkill";
    public const string UseSkillToMonster     = "UseSkillToMonster";
    public const string EquipItem             = "EquipItem";
    public const string GetSkill              = "GetSkill";
    public const string UseSkillToMonsterKind = "UseSkillToMonsterKind";
    public const string Cheer                 = "Cheer";
    public const string GetSocialPoint        = "GetSocialPoint";
    public const string AddFirends            = "AddFriends";
    public const string OpenUI                = "OpenUI";
    public const string Extenstion            = "Extenstion";
	public const string PostingSNS            = "PostingSNS";
    public const string ProduceItem           = "ProduceItem";
    public const string ProductionMastery     = "ProductionMastery";
    public const string GetCollect            = "GetCollect";
    public const string Waypoint              = "Waypoint";
    public const string MixItem               = "MixItem";
    public const string Designation           = "Designation";
    public const string Gold                  = "Gold";
    public const string Friendship            = "Friendship";
    public const string FriendCall            = "FriendCall";
    public const string MetaQuest             = "MetaQuest";
    public const string AddParty              = "AddParty";
    public const string DailyQuest            = "DailyQuest";
    public const string UseItemToTarget       = "UseItemToTarget";
    public const string UseEmoticon           = "UseEmoticon";
    public const string GetQuestCollection    = "GetQuestCollection";
    public const string StrengthenItem        = "StrengthenItem";
    public const string InsertSeal            = "InsertSeal";
	public const string GetPvpPoint           = "GetPvpPoint";
	public const string PvpPlay				  =	"PvpPlay";
	public const string PvpWin				  =	"PvpWin";
	public const string GetRankPoint	= "GetRankPoint";
	public const string CompleteQuest	= "CompleteQuest";
	public const string UseSocialPoint = "UseSocialPoint";
	public const string GetItemFromShop = "GetItemFromShop";
}

public struct ConditionTypeString
{
    public const string ItemHave           = "ItemHave";
    public const string ItemUse            = "ItemUse";
    public const string Level              = "Level";
    public const string CollectionSkill    = "CollectionSkill";
    public const string ProductionMastery  = "ProductionMastery";
    public const string MoveMap            = "MoveMap";
    public const string Buff               = "Buff";
    public const string QuestPass          = "QuestPass";
    public const string Size               = "Size";
    public const string Transform          = "Transform";
    public const string Race               = "Race";
    public const string Class              = "Class";
    public const string SkillHave          = "SkillHave";
    public const string SkillUse           = "SkillUse";
    public const string Designation        = "Designation";
    public const string Gold               = "Gold";
    public const string Friendship         = "Friendship";
}

public struct PrepareTypeString
{
    public const string CollectionSkill    = "CollectionSkill";
    public const string ProductionSkill    = "ProductionSkill";
    public const string CollectionItemDrop = "CollectionItemDrop";
    public const string MonsterItemDrop    = "MonsterItemDrop";
    public const string MonsterKindItemDrop= "MonsterKindItemDrop";
    public const string MonsterGenerate    = "MonsterGenerate";
    public const string QuestItem          = "QuestItem";
    public const string MoveTo             = "MoveMap";
    public const string OpenUI             = "OpenUI";
    public const string Size               = "Size";
    public const string Transform          = "Transform";
}

public struct RewardTypeString
{
    public const string ID          = "ID";
    public const string Exp         = "Experience";
    public const string Item        = "Item";
    public const string ItemSelect  = "ItemSelect";
    public const string Money       = "Money";
    public const string Miracle     = "Miracle";
    public const string ByClassType = "ByClassType";
    public const string Skill       = "Skill";
    public const string Designaion  = "SubTitle";
}

public enum DesignationType
{
    DESIGNATION_COUNT,
    DESIGNATION_HAVE,
    DESIGNATION_EQUIP,
    MAX,
}

public enum DesignationAchType
{
    DESIGNATION_COUNT,
    DESIGNATION_GET,
    DESIGNATION_CHANGE,
    MAX,
    DESIGNATION_UNEQUIP,
    DESIGNATION_ALREADY_EQUIP,
}

public enum GoldAchType
{
    GOLD_GET,
    GOLD_USE,
	GOLD_GET_BACK,
    MAX,
}

public enum FriendshipConditionType
{
    FRIENDSHIP_COUNT,
    FRIENDSHIP_POINT,
    MAX
}

public enum FriendshipAchType
{
    FRIENDSHIP_COUNT,
    FRIENDSHIP_POINT,
    FRIENDSHIP_CALL,
    MAX
}

public enum PrepareOpenUIType
{
	QUEST_BOOK_BTN,
	INVENTORY,
	CHANNEL,
	WARP,
	EMOTICON,
	PVP,
	AUTO_COMBAT,
	MAX,
}

public enum UseItemToTargetType
{
    MONSTER,
    NPC,
    COLLECTION,
    MAX
}

public enum eInsertSealGrade
{
    None,
    Normal,
    Magic,
    Rare,
    Epic,
    Ark,
    Max,
}

public enum eInsertSealKind
{
    None,
    Weapon,
    Defend,
    Accessory,
    Max,
}

