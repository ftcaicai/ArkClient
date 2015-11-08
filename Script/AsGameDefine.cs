
//============================================
// c/s °øÅë ·ÎÁ÷
//============================================
// °ÔÀÓ¿¡Œ­ »ç¿ëµÇŽÂ °øÅë DEFINE
//============================================
// c++					c#
// char					sbyte 1byte
// byte(unsigned char)	byte 1byte
// wchar_t				char 2byte
// short				short 2byte
// unsigned short(WORD)	ushort 2byte
// int					int 4byte
// unsigned int			uint 4byte
// long					int 4byte
// unsigned long(DWORD, DWORD32)	uint 4byte
// __int64							long 8byte
// unsigned __int64(DWORD64)		ulong 8byte
//============================================

using System;

public class AsGameDefine
{
	public enum WemeRequestType : int
	{
		WemeLinkClick = 1,
		MemberRegistration,
		Inquiry,
		ShowGameInfo,
		FriendList,
		Billing,
		FindPIN,
		EnterKakaoInfo = 100,
		MoveWemeKakaoAccount,
		EMailMemberRegistration = 201,
	};

	public enum PlatformCode : int
	{
		OldWeme = 5,
		Kakao = 7,
		NewWeme = 9,
		Facebook = 10,
		TStore = 11,
		Tweeter = 12,
	};
	
	public const string WEME_DOMAIN = "wemejp";
	public const string BUNDLE_VERSION = "1.9.26";
	public const int GAME_CODE = 300125;
	public const int MAX_LEVEL = 50;
	public const int MAX_LAYER_COUNT = 512;
	public const int MAX_MAP = 128;
	public const int MAX_CHARACTER_SLOT_COUNT = 8;
	public const int MAX_BUFF_COUNT = 30;
	public const int MAX_SKILL_COUNT = 30;
	public const int MAX_INVENTORY_SLOT = 18;
	public const int MAX_WAREHOUSE_SLOT = 18;
	public const int TOTAL_ITEM_SLOT = MAX_INVENTORY_SLOT + MAX_WAREHOUSE_SLOT;
	public const int MAX_WORLD = 64;
	public const int MAX_MAILT_TITLE_LENGTH = 24;
	public const int SERVER_USER_MAX = 3000;
	public const int SERVER_USER_SATURATION = 1000;
	public const int eCHANNELNAME = 16;
	public const int MAX_REC_LEN = 20;
	public const int MAX_ID_LEN = 24;
	public const int MAX_PW_LEN = 16;
	public const int MAX_WORLD_NAME_LEN = 20;
	public const int MAX_CHANNEL_LEN = 20;
	public const int MAX_NAME_LEN = 24;
	public const int MAX_GUILD_NAME_LEN = 24;
	public const int eGUILDNOTICE = 240;
	public const int eGUILDPUBLICIZE = 60;
	public const int MAX_MAP_LEN = 32;
	public const int MAX_CHAT_LEN = 255;
	public const int MAX_ANIMATION_LEN = 32;
	public const int MAX_ACTIVE_SKILL = 5;
	public const int MAX_PASSIVE_SKILL = 5;
	public const int ITEM_SLOT_VIEW_COUNT = 5;
	public const int ITEM_SLOT_COS_VIEW_COUNT = 7;
	public const int eITEM_MAX_ENCHANT_COUNT = 2;
	public const byte eITEM_PRODUCT_SLOT_COUNT = 5;
	public const int ePLATFORMUSERID = 100;
	public const int eAPPID = 100;
	public const int ePLATFORMPWD = 30;
	public const int eOAUTHTOKEN = 100;
	public const int eDEVICEID = 100;
	public const int eEMAIL = 320;
	public const uint INVALID_INDEX = uint.MaxValue;
	public const uint INVALID_SERIAL = uint.MaxValue;
	public const int INVALID_DBKEY = 0;
	public const ushort INVALID_MAPID = ushort.MaxValue;
	public const int INVALID_RECORD = int.MaxValue;
	public const string INVALID_RECID = "*";
	public const int QUICK_SLOT_MAX = 24;
	public const int QUICK_SHOW_SLOT_MAX = 8;
	public const int QUICK_ITEM_SLOT_NUM = 12;
	public const int QUICK_SKILL_SLOT_NUM = 12;
	public const int MAX_SPEECH = 3;
	public const int MAX_VOICE = 3;
	public const int MAX_TALK = 3;
	public const int MAX_NPC_MENU = 5;
	public const int MAX_PARTYNAME_LEN = 36;
	public const int MAX_PARTY_USER = 4;
	public const int ePARTYNOTICE = 40;
	public const int MAX_STORE = 30;
	public static int MAKELONG( int LoWord, int HiWord )
	{
		return ( HiWord << 16 ) | ( LoWord & 0xffff );
	}

	public const int MAX_SOCIAL_NOTICE	= 45;
	public const int eGAMEINVITE_KEY = 30;
	public const int CHARGE_PAGEMAX = 50;
	public const int CHARGE_MAXSLOT = 50;
	public const int CHARGE_MAXCOUNT = 10000000;
	public const int MAX_CHARGE_IAPPRODUCTID = 20;
	public const int MAX_IAPPRODUCT_NAME = 100;
	public const int MAX_IAPPRODUCT_CURRENCYCODE = 3;
	public const int MAX_IAPPRODUCT_COUNTRYCODE = 2;
	public const int MAX_TRANSACTION_DATA_SIZE = 2000;
	public const int MAX_CASHSHOP_JAP_ITEMTABLE_COUNT = 11;
	public const int MAX_ANDROID_IAPORDERID = 100;
	public const int MAX_ANDROID_PUBLICKEY_LENGTH = 2000;
	public const int MAX_PUSHKEY = 256;
	public const int MAX_NICKNAME = 24;
	public const int MAX_PRIVATESHOP_CONTENT_LENGTH = 61;
	public const ulong MAX_GOLD = 9999999999;
	public const int MAX_EVENT_TITLE = 60;
	public const int MAX_EVENT_CONTENT = 150;
	public const int eCOUPONKEY	= 16;
	public const int eWEMECHECKAUTH = 1024;
	public const int eWEMERECORD = 256;
	public const int eACCESSTOKEN = 512;
	public const int MAX_GAME_INVITE_REWARD = 5;
	public const int MAX_BAND_GAME_INVITE_REWARD = 6;
	public const int eRECOMMEND_ACCURE_MAX = 6;
	public const int eHACK_INFO = 60;
	
	public const int ePET_NAME = 18;
}

public enum eDiscipline
{
	eDISCIPLINE_NONE,			//Â¡°è»çÇ×Ÿ÷Àœ
	eDISCIPLINE_BLOCK,			//Á¢ŒÓ±ÝÁö
	eDISCIPLINE_MUTE,			//Ã€ÆÃ±ÝÁö
	eDISCIPLINE_MAX
};

//À¯ÀúŒö µî±Þ
//==============
public enum eCrowdGrade
{
	eCROWDGRADE_SMOOTH,		//¿øÈ°ÇÑ »óÅÂ
	eCROWDGRADE_BUSY,		//ºžÅë(60%)
	eCROWDGRADE_FULL,		//È¥Àâ(80%)
	eCROWDGRADE_OVER,		//ŽõÀÌ»ó žøµéŸî°š(100%)
};

//ÀÌµ¿»óÅÂ
//==============
public enum eMoveState
{
	eMOVESTATE_NORMAL,
	eMOVESTATE_SIDESTEP,
	eMOVESTATE_BACK,
	eMOVESTATE_RUNAWAY,
	eMOVESTATE_HELPCALL,
	eMOVESTATE_KEEPMOTION,
	eMOVESTATE_OVERLAPPING,
};

public enum eJumpState
{
	eSTANDING,		//ÁŠÀÚž® Á¡ÇÁ.
	eJUMPING,		//Á¡ÇÁ.
	eFLYING			//³«ÇÏÁß.
};

public enum eStateFlag
{
	eSTATE_INIT = 0,
	eSTATE_SIT = 1 << 0,		//ŸÉŸÆÀÖŽÂ »óÅÂ
	eSTATE_DIE = 1 << 1,		//Á×Àº »óÅÂ
	eSTATE_PERCEPT = 1 << 2,		//ÀûÀÎÁö »óÅÂ
	eSTATE_ANGER = 1 << 3,		//ÀüÅõ »óÅÂ
	eSTATE_REWIND = 1 << 4,		//¿ø·¡À§Ä¡·Î µÇµ¹ŸÆ°¡ŽÂ »óÅÂ

	eSTATE_CALL_FOR_HELP = 1 << 10,	//Ž©±º°¡¿¡°Ô µµ¿òÀ» Ã»ÇÏŽÂ »óÅÂ
	eSTATE_INTERACT_CASTING = 1 << 11,	//ÀÎÅÍ·ºŒÇ ¿ÀºêÁ§Æ®¿Í ÀÎÅÍ·ºŒÇ Ä³œºÆÃÁßÀÎ »óÅÂ
	eSTATE_NOT_CONTROL = 1 << 12,	//ÄÁÆ®·ÑºÒ°¡ »óÅÂ(À¯Àú Á¶ÀÛ ºÒ°¡)
};

public enum eDUNGEON : int
{
	DUN_INVALID = -1,
	DUN_FOREST,
	DUN_MAGMA,

	NUM_DUNGEON
};


//	dawnsmell
#region Race
public enum eRACE : int
{
	NONE = 0,

	DEMIGOD,
	ELF,
	LUMICLE,
	ARMAN,
	DEMIEVIL,

	MAX
};
#endregion

#region Class
public enum eCLASS : int
{
	NONE = 0,

	DIVINEKNIGHT,
	MAGICIAN,
	CLERIC,
	HUNTER,
	ASSASSIN,

	// Demigod
//	DIVINEKNIGHT,
//	DESTROYER,
//	SWORDMASTER,
//	PALADIN,
//
//	// Elf
//	FIREWIZARD,
//	ICEWIZARD,
//	SUMMONER,
//	ELEMENTALSHAMAN,
	
	PET,
	All,
	MAX
};
#endregion

public enum eATTACK_TYPE : int
{
	NONE = 0,
	Physics,
	Magic
};

public enum eGENDER : int
{
	eGENDER_NOTHING = 0,

	eGENDER_MALE,
	eGENDER_FEMALE,
	All,

	eGENDER_MAX
};

#region - action table -
public enum eReadyMoveType {NONE = 0, Dash, TargetDash, BackTargetDash, Warp, Jump, TabDash, TabWarp}
public enum eHitMoveType {NONE = 0, Dash, TargetDash, BackTargetDash, Warp, Jump, TabDash, TabWarp}
public enum eLoopType {NONE = 0, Once = 1<<0, Loop = 1<<1, TimeLoop = 1<<2,
	TargetLoop = 1<<3, Charge = 1<<4, Cast = 1<<5, ClampForever = 1<<6, Cut = 1<<7, Once_Cycle = 1<<8}
public enum eNextIdleIndex {NONE = 0, Idle, BattleIdle}
public enum eAniCancel {NONE = 0, Disable}
public enum eLinkType {NONE, AttachDummy, HitPosition, HoldPosition, HitHoldPosition, TargetChain, Shooting, ShootingPosition}
public enum eEffPosition {NONE, HitPosition}
public enum eEffLoopType {NONE, Once, Loop, TimeLoop}//temp
public enum eHitType {NONE, Target, NonTarget, PositionTarget, ProjectileTarget, HoldTarget}
public enum eHitTarget {NONE, Self, Enemy}
public enum eHitAreaShape {NONE, Point, Circle, OurParty, OtherParty}//, Quadrangle}
public enum eHitActionCheck {NONE, Enable}
public enum eSkillDisableCheck {NONE, Disable}
public enum eProjectilePath {NONE = 0, Straight, Arc, Through}
public enum eValueLookType {NONE = 0, Moment, Duration}
#endregion

#region - projectile table -
public enum eProjectileEffLoopType {NONE, Once, Always, TimeLoop}
#endregion

public enum eNPCType
{
	NONE = 0,
	NPC,
	Monster,
	Object,
	Collection,
	Portal,
	Pet
}

public enum eNpcMenuImage
{
	NONE = -1,
	Npc = 0,
	Pc,
	Both,
}
public enum eNPCMenu
{
	NONE = -1,
	Ok = 0,
	Next,
	Pre,
	Skip,
	Close,
	Cure,
	Quest,
	S_Shop,
	I_Shop,
	Upgrade,
	Make,
	Storage,
	Synthesis,
	Event_npc,
	Recommend,
	InGameEvent,
	MaxNpcMenu
}

public enum eMONSTER_ATTACK_TYPE : int
{
	NONE = 0,

	Mild,
	Tyrannical,
	LowHp,
	HighHp,
	GreatItem,
	LowLevel,
	HighLevel,
	Elf
}

public enum eSKILL_CONDITION
{
	NONE = -1,

	Timer = 0,
	HPProb,
	MPProb,
	FrontSkill
}

//public enum eSKILL_TARGET
//{
//	NONE = -1,
//
//	//ProtoPlayer = 1
//	//$yde
//	Self = 1,
//	Enemy
//}

//$yde
public enum eMoveType
{
	Normal = 0,
	Auto = 1,
	Dash,
	Warp,
	Jump,
	ForcedMove,
	Sync_Move,
	Sync_Stop,
	Combat,
	Back
}

#region - skill & potency -
public enum eSKILL_TYPE
{
	NONE = 0,
	Base = 1,
	Command,
	Active,
	Target,
	Charge,
	Case,
	Passive,
	SlotBase,
	Warning,
	Stance,
	TargetCharge
}

public enum eSkill_TargetType
{
	NONE = 0,
	Self,
	Enemy,
	Party,
	Alliance,
	All
}

public enum COMMAND_SKILL_TYPE
{
	_NONE = -1,

	_STRAIGHT,
	_ARC_CW,
	_ARC_CCW,
	_CIRCLE_CW,
	_CIRCLE_CCW,
	_DOUBLE_TAP_TERRAIN,
	_DOUBLE_TAP_PLAYER,
	_DOUBLE_TAP_OTHERUSER,
	_DOUBLE_TAP_MONSTER,

	MAX_SKILL_TYPE
};

public enum eSkillIcon_Enable_Target
{
	NONE = 0,
	Self,
	Target
}

public enum eSkillIcon_Enable_Condition
{
	NONE = 0,
	ShieldDefense,
	Stun,
	Bleeding,
	Burning,
	Freeze,
	Critical,
	LowHealth,
	Hit,
	Movable,
	Poison,
	Bind,
	Blind,
	Sleep,
	Death,
	Fear
}

//public enum eSKILL_INPUT_TYPE
//{
//	NONE = 0,
//	Skill_1 = 1, Skill_2, Skill_3, Skill_4,
//	Straight, Arc, Circle, DoubleTab
//}

public enum eCommand_Type
{
	NONE = 0,
	Straight,
	ArcCW,
	ArcCCW,
	CircleCW,
	CircleCCW,
	DoubleTab
}

public enum eCommandPicking_Type
{
	NONE = 0,
	Self,
	Enemy,
	Party,
	Alliance,
	All,
	Terrain,
	FingerPoint
}

public enum ePotency_Enable_Target
{
	NONE = 0,
	Self,
	Target
}

public enum ePotency_Enable_Condition
{
	NONE = 0,
	ShieldDefense,
	Stun,
	Bleeding,
	Critical,
	LowHealth,
	Movable,
	Poison,
	Burning,
	Bind,
	Blind,
	Freeze,
	Sleep,
	Kill,
	HaveSkill,
	PvP,
	HaveBuff,
	Slow,
	Dodge
}

public enum ePotency_Type
{
	NONE = 0,
	PhysicalAttackControlRatio,
	PhysicalAttackControl,
	MinPhysicalAttackControlRatio,
	MinPhysicalAttackControl,
	MaxPhysicalAttackControlRatio,
	MaxPhysicalAttackControl,
	MagicalAttackControlRatio,
	MagicalAttackControl,
	MinMagicalAttackControlRatio,
	MinMagicalAttackControl,
	MaxMagicalAttackControlRatio,
	MaxMagicalAttackControl,
	PhysicalReceiveDamageControlRatio,
	PhysicalReceiveDamageControl,
	MagicalReceiveDamageControlRatio,
	MagicalReceiveDamageControl,
	TotalReceiveDamageControlRatio,
	TotalReceiveDamageControl,
	PhysicalDefenseControlRatio,
	PhysicalDefenseControl,
	MagicalResistControlRatio,
	MagicalResistControl,
	FireResistControlRatio,
	FireResistControl,
	IceResistControlRatio,
	IceResistControl,
	LightResistControlRatio,
	LightResistControl,
	DarkResistControlRatio,
	DarkResistControl,
	NatureResistControlRatio,
	NatureResistControl,
	HPControlRatio,
	HPControl,
	MPControlRatio,
	MPControl,
	TotalHPControlRatio,
	TotalHPControl,
	TotalMPControlRatio,
	TotalMPControl,
	HPChargeControlRatio,
	HPChargeControl,
	MPChargeControlRatio,
	MPChargeControl,
	AccuracyControlRatio,
	MoveSpeedControlRatio,
	AttackSpeedControlRatio,
	PhysicalReflectionRatio,
	MagicalReflectionRatio,
	Reflection,
	DebuffTimeControlRatio,
	DebuffTimeControl,
	AggroSetup,
	AggroAccrueControlRatio,
	AggroAccrueControl,
	AutoShieldRatio,
	AutoShield,
	Stun,
	HPDrainRatio,
	HPDrain,
	BleedingRatio,
	Bleeding,
	Move,
	Recall,
	PoisonRatio,
	Poison,
	DodgeControlRatio,
	CriticalControlRatio,
	CriticalDamageControlRatio,
	ShieldDefenseRatio,
	ShieldDefense,
	BurningRatio,
	Burning,
	BurningCancel,
	DebuffCancel,
	Bind,
	ManaSpendControlRatio,
	ManaSpendControl,
	CoolTimeControlRatio,
	CoolTimeControl,
	FireAttackControlRatio,
	FireAttackControl,
	IceAttackControlRatio,
	IceAttackControl,
	LightAttackControlRatio,
	LightAttackControl,
	DarkAttackControlRatio,
	DarkAttackControl,
	NatureAttackControlRatio,
	NatureAttackControl,
	HealRatio,
	Heal,
	HealDivideRatio,
	LimitShieldRatio,
	LimitShield,
	Blind,
	RecieveDamageTargetChange,
	AggroTargetChange,
	RecieveDamageToMP,
	HPControlRatioDuration,
	HPControlDuration,
	AddMagicDamageRatio,
	AddMagicDamage,
	AggroControlRatio,
	AggroControl,
	DebuffResist,
	BuffCancel,
	Freeze,
	Sleep,
	RunningCoolTimeControlRatio,
	RunningCoolTimeControl,
	CurrentCoolTimeControlRatio,
	CurrentCoolTimeControl,
	FreezeCancel,
	ExpControlRatio,
	BattleExpControlRatio,
	Slow,
	PhysicalAttackPenaltyRatio,
	MagicalAttackPenaltyRatio,
	PhysicalDefensePenaltyRatio,
	MagicalResistPenaltyRatio,
	MoveSpeedPenaltyRatio,
	BattleGoldControlRatio,
	BattleItemDropControlRatio,
	Resurrection,
	RecieveHealIncreaseRatio,
	ConditionControl,//$yde 20130704
	ConditionBattleExpControlRatio,//$yde 20130709
	SizeControl,//$yde 20130830
	HPInsteadMP,
	HPControl2,
	MPControl2,
	CountdownHPControl,
	SkillDistanceControl,
	Fear,
	SkillLearn,
	SkillRemove,
	AttackStackToCritical,
	DamageStackToDodge,
	GoalAttackStackControl,
	GoalDamageStackControl,
//	AttackStackControl,
//	DamageStackControl,
//	AttackStackAdd,
	PointMove,
	SkillUse,
	Stance,
	Blank,
	Balloon,
	AirBone,
	Transform,
	Gray,
	StunResist,
	BurningResist,
	FreezeResist,
	BleedingResist,
	BlindResist,
	HoldResist,
	PoisonResist,
	SleepResist,
	SlowResist,
	MoveResist,
	FearResist,
	BlankResist,
	AirBoneResist,
	TransformResist,
	GrayResist,
	NormalHardPlayCount,
	HellPlayCount,
	HellPlayCountAll,

	ePOTENCYTYPE_MAX
}

public enum ePotency_Target
{
	NONE = 0,
	Self,
	Enemy,
	Party,
	Alliance,
	All,
	OurParty,
	OtherParty
}

public enum ePotency_DurationType
{
	NONE = 0,
	Moment,
	Duration,
	Dot,
	Always
}

public enum ePotency_Attribute
{
	NONE = 0,
	Physics,
	Magic
}

public enum ePotency_Element
{
	NONE = 0,
	Fire,
	Ice,
	Light,
	Dark,
	Nature
}

public enum eAggro_ValueType
{
	NONE = 0,
	Value,
	DamageRatio,
	HealRatio,
	DamageHealRatio
}
#endregion

public enum eATTACK_DIRECTION
{
	NONE = 0,
	Current =1,
	Target,
	LineStart,
	LineMiddle,
	LineEnd,
	FingerPoint,
	Camera
}

public enum eClockWise
{
	CW = 0,
	CCW
}

public enum eEFFECT_TIMING
{
	Moment = 1,
	Duration
}

public enum eEFFECT_PLAYTIME
{
	Once = 1,
	Loop
}

public enum eSOUND_TIMING
{
	NONE,
	Moment = 1,
	Duration
}

public enum eRECALL_DEATH
{
	NONE = 0,
	Death
}

public enum eOBJECT_PROP
{
	NONE = 0,
	TRAP_PROP,
	STEPPING_LEAF,
	BROKEN_PROP,
	POST_BOX,
	STORAGE,
	WAYPOINT,
}

public enum eDAMAGETYPE
{
	eDAMAGETYPE_NOTHING = 0,

	eDAMAGETYPE_MISS,
	eDAMAGETYPE_DODGE,
	eDAMAGETYPE_REGIST,
	eDAMAGETYPE_NORMAL,
	eDAMAGETYPE_CRITICAL,

	eDAMAGETYPE_MAX
}

enum eITEMATTRIBUTE : sbyte
{
	eITEMATTRIBUTE_STRENGTHEN	= 0x01,
	eITEMATTRIBUTE_TRADE_LIMIT	= 0x02,
	eITEMATTRIBUTE_STORAGE_LIMIT= 0x04,
	eITEMATTRIBUTE_SEAL			= 0x08,
	eITEMATTRIBUTE_DUMP			= 0x10,
	eITEMATTRIBUTE_USERSELL		= 0x20,
};


public enum eITEM_PARTS_VIEW : int
{
	Weapon = 0x00000001,
	Head_normal	= 0x00000001 << 1,
	Head_costume = 0x00000001 << 2,
	Armor = 0x00000001 << 3,
	Gloves = 0x00000001 << 4,
	Point = 0x00000001 << 5,
	Wing = 0x00000001 << 6,
	Fairy = 0x00000001 << 7,	
};




#region - monster -
public enum eMonster_Race
{
	NONE = 0,
	Human,
	Bug,
	Dragon,
	Magic,
	Plant,
	Daemon,
	Fielder,
	Undead,
	Giant,
	Elemental
}

public enum eMonster_Grade
{
	NONE = 0,
	Normal,
	Elite,
	Champion,
	Boss,
	QObject,
	DObject,
	Named,
	Treasure,
	Intangible,
	Trap,
}

public enum eMonster_AttackType
{
	NONE = 0,
	Fool,
	Peaceful,
	Offensive
}

public enum eMonster_AttackStyle
{
	NONE = 0,
	Physical,
	Magical
}

public enum eMonster_HelpType
{
	NONE = 0,
	Race,
	All
}

public enum eMonster_HelpCondition
{
	NONE = 0,
	HP,
	HP_Always
}

public enum eMonster_RunAwayCondition
{
	NONE = 0,
	HP,
	HP_Always
}

public enum eMonster_RunAwayDirection
{
	NONE = 0,
	Aggro,
	Monster,
	Race
}
#endregion


#region - Store -
public enum eStore_ItemKind
{
	NONE = 0,
	WEAPON,
	ARMOR,
	ACCESSORY,
	CONSUME,
	ETC,
	MATERIAL,
	MAX,
}

public enum eCashStoreSubCategory
{
	NONE,
	EQUIPHIGH,
	EQUIPLOW,
	JEWELHIGH,
	JEWELLOW,
	HAIR,
	BOOSTER,
	POTION,
	PRIVATE,
	SCROLL,
	ETC,
	PETEGG,
	PETACC,
	PETFOOD,
	PETPOTION,
}

public enum eCashStoreMainCategory
{
	NONE,
	RECOMMEND,
	RECHARGE_GOLD,
	RECHARGE_MIRACLE,
	PACKAGE,

	COSTUME,

	WEAPONE_RANDOM,
	DEFEND_RANDOM,
	ACC_RANDOM,
	ENCHANT_RANDOM,
	BOOSTER_EXP,
	BOOSTER_GOLD,
	BOOSTER_DROP,
	HP_AND_MP,
	BUFF,
	CONDITION,
	RUBY,
	DIAMOND,
	PRIVATE_SHOP,
	ETC,
	GOLD,

	EVENT,

	COSTUME_ETC,
	SAPPHIRE,
	EMERALD,


	WEAPON,
	EQUIPMENT,
	PET,
	CONVENIENCE,
	FREE,
	MIRACLE,


}
#endregion


public enum ePOST_COMMON
{
	ePOST_MAX_ITEM = 4,
	ePOST_MAX_CLIENT_LIST = 5,
	ePOST_MAX_TITLE = 24,
	ePOST_MAX_LIST = 50,
	ePOST_MAX_CONTENT = 255,
	ePOST_CHECK_TIME = 3000,
}

#region -Dummy_Type
public enum Dummy_Type
{
	DummyNone = 0, //in Chaaracter
	DummyLead,
	DummyLeadBottom,
	DummyLeadTop,

	DummyCharacterTop,
	DummyCharacterCenter,
	DummyCharacterBottom,
	DummyCharacterChest,
	DummyCharacterBack,
	DummyCharacterHand_R,
	DummyCharacterHand_L,

	DummyWeaponHandle_R,
	DummyWeaponHandle_L,
	DummyWeaponCenter_R,
	DummyWeaponCenter_L,
	DummyWeaponLauncher_R,
	DummyWeaponLauncher_L,
	DummyTrailUp_R,
	DummyTrailUp_L,
	DummyTrailDown_R,
	DummyTrailDown_L,

	DummyShield, //in Shield Model
	DummyProjectile, //in Projectile Model
	DummyLeadHit,

	DummyCharacterMouth,

	DummyCharacterWeapon_R,
	DummyCharacterWeapon_L,
}
#endregion

public enum eMAP_TYPE
{
	Town,
	Field,
	Indun,
	Tutorial,
	Pvp,
	Raid,
	Summon,

	Invalid
}

public enum eCHARGETYPE
{
	eCHARGETYPE_NOTHING = 0,
	eCHARGETYPE_GOLD,
	eCHARGETYPE_MIRACLE,
	eCHARGETYPE_DAILY,
	eCHARGETYPE_MAX
};

public enum eCHARGECOMPANYTYPE : byte
{
	eCHARGECOMPANYTYPE_NOTHING,
	eCHARGECOMPANYTYPE_APPLE,
	eCHARGECOMPANYTYPE_ANDROID,
	eCHARGECOMPANYTYPE_MAX
};

public enum eSHOPITEMHIGHLIGHT
{
	eSHOPITEMHIGHLIGHT_NOTHING = 0,
	
	eSHOPITEMHIGHLIGHT_NONE,
	eSHOPITEMHIGHLIGHT_HOT,
	eSHOPITEMHIGHLIGHT_NEW,
	eSHOPITEMHIGHLIGHT_SALE,
	eSHOPITEMHIGHLIGHT_BONUS,
	
	eSHOPITEMHIGHLIGHT_MAX
};

public enum eLOCALPUSHTYPE
{
	eLOCALPUSH_NONE = 0,
	eLOCALPUSH_ITEMMAKE = 100000,
	eLOCALPUSH_MAX
}
