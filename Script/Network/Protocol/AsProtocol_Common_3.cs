#define INTEGRATED_ARENA_MATCHING
#define INDUN_TIME
#define INDUN_EXCHANGE_GOLD
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



//--------------------------------------------------------------------------------------------------
/* Common Packet Define */
//--------------------------------------------------------------------------------------------------

enum PROTOCOL_CS_3 : byte
{
	CS3_NOTHING,
	SC_EVENT_QUEST_TIME_SYNC,
	CS_EVENT_QUEST_REWARD,
	SC_EVENT_QUEST_REWARD_RESULT,
	
	CS_LEVEL_BONUS,
	SC_LEVEL_BONUS_RESULT,

	SC_RESURRECTION_CNT,
	
	CS_TARGET_MARK,
	SC_TARGET_MARK,
	
	CS_CHAR_NAME_CHANGE,
	SC_CHAR_NAME_CHANGE,
	SC_CHAR_NAME_NOTIFY,

	#region - pet -
	// init & notify & release
//	SC_PET_LOAD,				//소환중인 펫 정보.
	SC_PET_CREATE_RESULT,			//<<알사용 성공.
	SC_PET_SKILL_USE,			//<<스킬 사용.
	SC_PET_LEVEL_UP,			//<<레벨업.
	SC_PET_LIST,				//<<펫 슬롯 리스트.
	SC_PET_SLOT_CHANGE,			//<<펫 슬롯 변경.
	SC_PET_HUNGRY,				//<<펫 배고픔.
	SC_PET_NAME_NOTIFY,

	// ui window
	CS_PET_INFO,				//<<펫 세부정보.
	SC_PET_INFO,
	CS_PET_CALL,				//<<펫 소환.
	SC_PET_CALL,				//<<펫 소환 브로드 캐스팅 ( 본인 포함 )
	SC_PET_CALL_RESULT,			//<<펫 소환 결과 (본인만)

	CS_PET_RELEASE,				//<<펫 해제.
	SC_PET_RELEASE,				//<<펫 해제 브로드 캐스팅 ( 본인 포함 )
	SC_PET_RELEASE_RESULT,			//<<펫 해제 결과 (본인만)
	CS_PET_DELETE,				//<<펫 삭제.
	SC_PET_DELETE_RESULT,			//<<펫 삭제 결과 (본인만) -- 소환중인 펫 삭제시  SC_PET_RELEASE 와 같이 감.

	// equip
	CS_PET_EQUIP_ITEM,			//펫 아이템(악세사리) 장착.
	SC_PET_EQUIP_ITEM_RESULT,
	CS_PET_UNEQUIP_ITEM,			//펫 아이템(악세사리) 해제.
	SC_PET_UNEQUIP_ITEM_RESULT,

	// action
	CS_PET_NAME_CHANGE,			//<<펫 이름 변경 (본인 포함 SC_PET_NOTIFY)
	SC_PET_NAME_CHANGE_RESULT,		//<<펫 이름 변경 결과 (본인)

	CS_PET_UPGRADE,				//<<펫 업그레이드.
	SC_PET_UPGRADE_RESULT,
	CS_PET_ITEM_COMPOSE,			//<<펫 아이템 합성.
	SC_PET_ITEM_COMPOSE_RESULT,

	CS_PET_SLOT_EXTEND,			//<<펫 슬롯 확장.
	SC_PET_SLOT_EXTEND_RESULT,
	#endregion
	#region CashStore JPN
	CS_CASHSHOP_JAP_INFO,
	SC_CASHSHOP_JAP_INFO_RESULT,
	CS_CASHSHOP_JAP_ITEMBUY,
	SC_CASHSHOP_JAP_ITEMBUY_RESULT,
	#endregion

#if INTEGRATED_ARENA_MATCHING
	#region -Integrated Pvp
	CS_INTEGRATED_ARENA_MATCHING_REQUEST,
	SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT,
	CS_INTEGRATED_ARENA_MATCHING_GOINTO,
	SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT,
	SC_INTEGRATED_ARENA_MATCHING_NOTIFY,
	SC_INTEGRATED_ARENA_GOGOGO,

	CS_INTEGRATED_INDUN_LOGIN,
	SC_INTEGRATED_INDUN_LOGIN_RESULT,
	CS_INTEGRATED_INDUN_LOGIN_END,
	CS_INTEGRATED_INDUN_LOGOUT,
	SC_INTEGRATED_INDUN_LOGOUT_RESULT,
	CS_RETURN_WORLD,
	SC_RETURN_WORLD_RESULT,
	CS_RETURN_WORLD_END,
	#endregion
	
	#region -Intergrated Indun
	CS_INTEGRATED_INDUN_MATCHING_INFO,
	SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT,
	CS_INTEGRATED_INDUN_MATCHING_REQUEST,
	SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT,
	CS_INTEGRATED_INDUN_MATCHING_GOINTO,
	SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT,
	SC_INTEGRATED_INDUN_MATCHING_NOTIFY,
	SC_INTEGRATED_INDUN_GOGOGO,
	SC_INTEGRATED_INDUN_REWARDINFO,
	CS_INDUN_REWARD_GETITEM,
	SC_INDUN_REWARD_GETITEM_RESULT,
	SC_INDUN_QUEST_PROGRESS_INFO,
	SC_INDUN_ENTER_LIMITCOUNT,
	SC_INDUN_ENTER_HELLLIMITCOUNT,
#if INDUN_TIME
	SC_INDUN_CLEARINFO,
#endif
	SC_INTEGRATED_INDUN_RE_CONNECT_FAIL,
	CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL,
	#endregion
#endif

    SC_AP_RANK_RESULT,
    CS_AP_RANK_REWARD,
    SC_AP_RANK_REWARD_RESULT,
}


public class body_CS_EVENT_QUEST_REWARD : AsPacketHeader
{
	public int nEventIdx;
	public byte nRewardIdx;

	public body_CS_EVENT_QUEST_REWARD(int _eventID, byte _rewardIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_EVENT_QUEST_REWARD;
		nEventIdx = _eventID;
		nRewardIdx = _rewardIdx;
	}
};

public class body_SC_EVENT_QUEST_TIME_SYNC : AsPacketHeader
{
	public long nServerSec;
};

public class body_SC_EVENT_QUEST_REWARD_RESULT: AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_CS_LEVEL_BONUS : AsPacketHeader
{
	public int	nCompleteLevel;
	
	public body_CS_LEVEL_BONUS(int _lv)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_LEVEL_BONUS;
		
		nCompleteLevel = _lv;
	}
};

public class body_SC_LEVEL_BONUS_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public int nCompleteLevel;
};

public class body_SC_RESURRECTION_CNT : AsPacketHeader
{
	public int nResurrectionCnt;
};

public class body_CS_TARGET_MARK : AsPacketHeader
{
	public ushort	nCharTargetIdx = 0;			//session id
	public int	nNpcTargetIdx = 0;			//npc id
	
	public body_CS_TARGET_MARK( ushort _nCharTargetIdx, int _nNpcTargetIdx)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_TARGET_MARK;
		
		nCharTargetIdx = _nCharTargetIdx;
		nNpcTargetIdx = _nNpcTargetIdx;
	}
};

public class body_SC_TARGET_MARK : AsPacketHeader
{
	public ushort	nCharTargetIdx = 0;
	public int	nNpcTargetIdx = 0;
};

class body_CS_CHAR_NAME_CHANGE :AsPacketHeader
{
	public int nSlot = -1;
	public byte[] szCharName = null;
	
	public body_CS_CHAR_NAME_CHANGE( string _szCharName , int _slot )
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_CHAR_NAME_CHANGE;
		
		byte[] byteArray = System.Text.UTF8Encoding.UTF8.GetBytes( _szCharName);
		
		if( byteArray.Length > AsGameDefine.MAX_NAME_LEN)
		{
			return;
		}
		
		szCharName = new byte[AsGameDefine.MAX_NAME_LEN + 1];
		
		Buffer.BlockCopy( byteArray, 0, szCharName, 0, byteArray.Length);
		
		// end of string
		szCharName[byteArray.Length] = 0;
		
		nSlot = _slot;
	}
};


public class body_SC_CHAR_NAME_CHANGE : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt32 nCharUniqKey;
	public string szBeforeCharName;
	public string szAfterCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
		
		// nCharUniqKey
		byte[] userUniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, userUniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( userUniqKey, 0));
		index += sizeof( UInt32);
		
		// szBeforeCharName
		byte[] beforeCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, beforeCharName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szBeforeCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( beforeCharName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
		
		// szAfterCharName
		byte[] afterCharName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, afterCharName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szAfterCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( afterCharName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}


public class body_SC_CHAR_NAME_NOTIFY : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public string szCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] userUniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, userUniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( userUniqKey, 0));
		index += sizeof( UInt32);
		
		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

#region -CashStore JPN-
public class  body_CS_CASHSHOP_JAP_INFO : AsPacketHeader
{
	public body_CS_CASHSHOP_JAP_INFO()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_CASHSHOP_JAP_INFO;
	}
};

public class body_CS_CASHSHOP_JAP_ITEMBUY : AsPacketHeader
{
	public int nShopItemSlot;
    public int nItemTableIdx;
	public int nItemCount; 
	public int nInvenSlot;

	public body_CS_CASHSHOP_JAP_ITEMBUY(int _slot, int _itemTableIdx, int _count, int _invenSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_CASHSHOP_JAP_ITEMBUY;

		nShopItemSlot	= _slot;
        nItemTableIdx   = _itemTableIdx;
		nItemCount		= _count;
		nInvenSlot		= _invenSlot;
	}
};

public class body_SC_CASHSHOP_JAP_INFO_RESULT : AsPacketHeader
{
	public Int64 nCash;
	public Int64 nFreeCash;
	public Byte nGachaPoint;
};

public class body_SC_CASHSHOP_JAP_ITEMBUY_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int64	nCash;					
	public Int64	nFreeCash;				
	public Byte	nGachaPoint;
	public int[]   nOpenGachaItemTable = new Int32[AsGameDefine.MAX_CASHSHOP_JAP_ITEMTABLE_COUNT];
    public byte[] nOpenGachaItemStrengTable = new byte[AsGameDefine.MAX_CASHSHOP_JAP_ITEMTABLE_COUNT];

	public override string ToString()
	{
		string szResult = "[" + eResult + "] c = " + nCash + "  fc = " + nFreeCash + "  gp = " + nGachaPoint +"\n";

		foreach (int id in nOpenGachaItemTable)
		{
			szResult += " id[" + id + "] ";
		}

		return szResult;
	}
};



#endregion

#region -AP Point
// popup
public class body_SC_AP_RANK_RESULT : AsPacketHeader
{
    public int nRank;
    public int nRewardGroup;
};

public class body_CS_AP_RANK_REWARD : AsPacketHeader
{
    public body_CS_AP_RANK_REWARD()
    {
        Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
        Protocol = (byte)PROTOCOL_CS_3.SC_AP_RANK_RESULT;
    }
};

public class body_SC_AP_RANK_REWARD_RESULT : AsPacketHeader
{
    public eRESULTCODE eResult;
};


#endregion

// - pet
#region - init & notify & release -
//public class body_SC_PET_LOAD : AsPacketHeader
//{
//	public Int32			nSlot;
//	public Int32			nPetTableIdx;				//<< TableIdx가 0 이면 펫을 해제 했을경우!!
//	public Int32			nPersonality;
//	public byte[]			szPetName = new byte[AsGameDefine.ePET_NAME + 1];
//	public sITEMVIEW		sViewItem;
//	public ePET_HUNGRY_STATE	eHungryState;
//};

public class body_SC_PET_CREATE_RESULT : AsPacketHeader
{
	public eRESULTCODE		eResult;

	public Int32			nPetTableIdx;
	public Int32			nPetPersonality;			//랜덤성격.
}

public class body_SC_PET_NAME_NOTIFY : AsPacketHeader
{
	public UInt32 nCharUniqKey;			//펫 오너.
	public byte[] szPetName = new byte[AsGameDefine.ePET_NAME+1];		//펫 이름.
}

public class body_SC_PET_SKILL_USE : AsPacketHeader
{
	public UInt32		nCharUniqKey;
	public Int32		nSkillTableIdx;
	public Int32		nSkillLevel;
};

public class body_SC_PET_LEVEL_UP : AsPacketHeader
{
	public UInt32				nCharUniqKey;
};

public class body1_SC_PET_LIST : AsPacketHeader
{
	public Int32			nExtendLine;				//확장된 슬롯 라인수.
	public Int32			nCnt;					//body2 갯수.
	
	public body2_SC_PET_LIST[] body = new body2_SC_PET_LIST[0];
	
	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;
		
		int index = ParsePacketHeader( data);

		nExtendLine = BitConverter.ToInt32( ParseValueToByte(data, ref index, sizeof(Int32)), 0);
		nCnt = BitConverter.ToInt32( ParseValueToByte(data, ref index, sizeof(Int32)), 0);
		
		if( nCnt > 0) body = ParsePacketToBody<body2_SC_PET_LIST>(data, ref index, nCnt);
	}
};

public class body2_SC_PET_LIST : AsPacketHeader, IContainSize						//펫 관리 창 정보.
{
	public Int32			nPetTableIdx;
	public Int32			nPetSlot;				//펫 고유키.
	public byte[]			szPetName = new byte[AsGameDefine.ePET_NAME + 1];
	public Int32			nLevel;
	
	//	public static int size = sizeof(Int32) + sizeof(Int32) + (AsGameDefine.ePET_NAME + 1) + sizeof(Int32);
	
	public int Size()
	{
		return sizeof(Int32) + sizeof(Int32) + (AsGameDefine.ePET_NAME + 1) + sizeof(Int32);
	}
};

public class  body_SC_PET_SLOT_CHANGE : AsPacketHeader
{
	public Int32			nPetTableIdx;
	public Int32			nPetSlot;				//펫 고유키.
	public byte[]			szPetName = new byte[AsGameDefine.ePET_NAME + 1];
	public Int32			nLevel;
};

public class body_SC_PET_HUNGRY : AsPacketHeader
{
	public ePET_HUNGRY_STATE	eHungryState;
};
#endregion
#region - ui window -
public class body_CS_PET_INFO : AsPacketHeader
{
	public Int32		nSlot;

	public body_CS_PET_INFO(int _nSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_INFO;

		nSlot = _nSlot;
	}
};

public class body_SC_PET_INFO : AsPacketHeader
{
	public Int32		nSlot;
	public Int32		nPetTableIdx;
	public Int32		nPetPersonality;			//성격.
	public byte[]		szPetName = new byte[AsGameDefine.ePET_NAME + 1];
	public Int32		nLevel;
	public Int32		nExp;
	public Int32		nHungryPoint;
	public sPETSKILL[]	sSkill = new sPETSKILL[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX];
	public sITEM		sItem;
};

public class body_CS_PET_CALL : AsPacketHeader
{
	public Int32			nSlot;

	public body_CS_PET_CALL(int _nSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_CALL;

		nSlot = _nSlot;
	}
};

public class body_SC_PET_CALL : AsPacketHeader
{
	public UInt32				nCharUniqKey;				//call 한 character
	
	public Int32				nSlot;
	public Int32				nPetTableIdx;
	public Int32				nPersonality;
	public byte[]				szPetName = new byte[AsGameDefine.ePET_NAME+1];
	public sITEMVIEW			sViewItem;
	public ePET_HUNGRY_STATE		eHungryState;
};

public class body_SC_PET_CALL_RESULT : AsPacketHeader		//소환 실패시에만 전송
{
	public eRESULTCODE		eResult;
};

public class body_CS_PET_RELEASE : AsPacketHeader
{
	public body_CS_PET_RELEASE()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_RELEASE;
	}
};

public class body_SC_PET_RELEASE : AsPacketHeader
{
	public UInt32				nCharUniqKey;				//release한 character
};

public class body_SC_PET_RELEASE_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_CS_PET_DELETE : AsPacketHeader
{
	public Int32			nSlot;

	public body_CS_PET_DELETE(int _slot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_DELETE;

		nSlot = _slot;
	}
}

public class body_SC_PET_DELETE_RESULT : AsPacketHeader
{
	public eRESULTCODE	eResult;
	public Int32		nSlot;
};
#endregion
#region - equip -
public class body_CS_PET_EQUIP_ITEM : AsPacketHeader
{
	public Int32				nSlot;

	public body_CS_PET_EQUIP_ITEM(int _nSlot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_EQUIP_ITEM;
		
		nSlot = _nSlot;
	}
};

public class body_SC_PET_EQUIP_ITEM_RESULT : AsPacketHeader
{
	public eRESULTCODE			eResult;
};

public class body_CS_PET_UNEQUIP_ITEM : AsPacketHeader
{
	public Int32 nSlot;

	public body_CS_PET_UNEQUIP_ITEM(int _slot)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_UNEQUIP_ITEM;

		nSlot = _slot;
	}
};

public class body_SC_PET_UNEQUIP_ITEM_RESULT : AsPacketHeader
{
	public eRESULTCODE			eResult;
};
#endregion
#region - act -
public class body_CS_PET_NAME_CHANGE : AsPacketHeader
{
	public Int32 nSlot;
	public byte[] szPetName = new byte[AsGameDefine.ePET_NAME+1];
	
	public body_CS_PET_NAME_CHANGE(int _slot, byte[] _petName)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_NAME_CHANGE;

		nSlot = _slot;

		for( int i=0; i<_petName.Length; ++i)
		{
			szPetName[i] = _petName[i];
		}
	}
}

public class body_SC_PET_NAME_CHANGE_RESULT : AsPacketHeader
{
	public eRESULTCODE			eResult;
}

public class body_CS_PET_UPGRADE : AsPacketHeader
{
	public Int32				nSlot1;			//베이스 펫 슬롯.
	public Int32				nSlot2;			//재료 펫 슬롯.

	public body_CS_PET_UPGRADE(int _nSlot1, int _nSlot2)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_UPGRADE;
		
		nSlot1 = _nSlot1;
		nSlot2 = _nSlot2;
	}
};

public class body_SC_PET_UPGRADE_RESULT : AsPacketHeader
{
	public eRESULTCODE			eResult;
	
	public Int32				nTableIdx;
	public Int32				nPersonality;
	public byte[]				szPetName = new byte[AsGameDefine.ePET_NAME+1];
};

public class body_CS_PET_ITEM_COMPOSE : AsPacketHeader
{
	public Int32				nPetSlot;		//베이스 펫 슬롯.
	public Int32				nItemSlot1;		//재료 아이템 슬롯1.
	public Int32				nItemSlot2;		//재료 아이템 슬롯2.
	public Int32				nItemSlot3;		//재료 아이템 슬롯3.

	public body_CS_PET_ITEM_COMPOSE(int _nPetSlot, int _nItemSlot1, int _nItemSlot2, int _nItemSlot3)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_ITEM_COMPOSE;
		
		nPetSlot = _nPetSlot;
		nItemSlot1 = _nItemSlot1;
		nItemSlot2 = _nItemSlot2;
		nItemSlot3 = _nItemSlot3;
	}
};

public class body_SC_PET_ITEM_COMPOSE_RESULT : AsPacketHeader
{
	public eRESULTCODE			eResult;
	public Int32				nLevel;
	public Int32				nExp;		
};

public class  body_CS_PET_SLOT_EXTEND : AsPacketHeader
{
	public Int32			nLine;

	public body_CS_PET_SLOT_EXTEND(int _line)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_PET_SLOT_EXTEND;

		nLine = _line;
	}
};

public class  body_SC_PET_EXTEND_SLOT_RESULT : AsPacketHeader
{
	public eRESULTCODE		eResult;
	public Int32			nLine;
};
#endregion
#region - struct -
public class sPETSKILL : AsPacketHeader
{
	public static int size = 12;
	
	public Int32		nSkillTableIdx;
	public Int32		nLevel;
	public UInt32		nCoolTime;
};

public enum ePET_SKILL_TYPE
{
	ePET_SKILL_TYPE_PASSIVE	= 0,	
	ePET_SKILL_TYPE_ACTIVE,
	ePET_SKILL_TYPE_SPECIAL,

	ePET_SKILL_TYPE_MAX,
};

public enum ePET_HUNGRY_STATE
{
	ePET_HUNGRY_STATE_NOTHING = 0,
	
	ePET_HUNGRY_STATE_GOOD,
	ePET_HUNGRY_STATE_NORMAL,
	ePET_HUNGRY_STATE_HUNGRY,

	ePET_HUNGRY_STATE_ENABLE_POPUP,
	
	ePET_HUNGRY_STATE_MAX
};
#endregion

#if INTEGRATED_ARENA_MATCHING
#region -Integrated Pvp
public class body_CS_INTEGRATED_ARENA_MATCHING_REQUEST : AsPacketHeader
{
	public bool bMatching;
	public Int32 nIndunTableIndex;
	public bool bTestMode;

	public body_CS_INTEGRATED_ARENA_MATCHING_REQUEST( bool matching, Int32 indunTableIndex, bool testMode)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_INTEGRATED_ARENA_MATCHING_REQUEST;

		bMatching = matching;
		nIndunTableIndex = indunTableIndex;
		bTestMode = testMode;
	}
}

public class body_SC_INTEGRATED_ARENA_MATCHING_REQUEST_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_INTEGRATED_ARENA_MATCHING_GOINTO : AsPacketHeader
{
	public bool bEnter;

	public body_CS_INTEGRATED_ARENA_MATCHING_GOINTO( bool enter)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_INTEGRATED_ARENA_MATCHING_GOINTO;

		bEnter = enter;
	}
}

public class body_SC_INTEGRATED_ARENA_MATCHING_GOINTO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
	}
}

public class body_SC_INTEGRATED_ARENA_MATCHING_NOTIFY : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunTableIndex;
	public UInt32 nCharUniqKey;
	public string szCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// nCharUniqKey
		byte[] CharUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, CharUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( CharUniqKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] name = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, name, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( name));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_INTEGRATED_ARENA_GOGOGO : AsPacketHeader
{
	public string szIpAddress;
	public UInt16 nPort;
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// szIpAddress
		byte[] IpAddress = new byte[ 17];
		Buffer.BlockCopy( data, index, IpAddress, 0, 17);
		szIpAddress = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( IpAddress));
		index += 17;

		// nPort
		byte[] Port = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nPort", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Port, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( Port, 0));
		index += sizeof( UInt16);

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// nIndunDuplicationIndex
		byte[] IndunDuplicationIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunDuplicationIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunDuplicationIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunDuplicationIndex, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_INTEGRATED_INDUN_LOGIN : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;
	public bool bReconnect;
	
	public body_CS_INTEGRATED_INDUN_LOGIN( UInt32 CharUniqKey, Int32 indunTableIndex, Int32 indunDuplicationIndex, bool reconnect)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_INTEGRATED_INDUN_LOGIN;
		
		nCharUniqKey = CharUniqKey;
		nIndunTableIndex = indunTableIndex;
		nIndunDuplicationIndex = indunDuplicationIndex;
		bReconnect = reconnect;
	}
}

public class body_SC_INTEGRATED_INDUN_LOGIN_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public UInt16 nSessionIndex;
	public Int32 nIndunTableIndex;
	public Int32 nMapID;
	public Vector3 vPosition;
	public bool bReconnect;
	
	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;
		
		int index = ParsePacketHeader( data);
		
		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
		
		// nSessionIndex
		byte[] SessionIndex = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nSessionIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, SessionIndex, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( SessionIndex, 0));
		index += sizeof( UInt16);
		
		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);
		
		// nMapID
		byte[] MapID = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nMapID", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, MapID, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( MapID, 0));
		index += sizeof( Int32);
		
		// vPosition
		byte[] position = new byte[12];
		headerinfo = infotype.GetField( "vPosition", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, position, 0, 12);
		headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( position, 0), BitConverter.ToSingle( position, 4), BitConverter.ToSingle( position, 8)));
		index += 12;
		
		// bReconnect
		byte[] Reconnect = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bReconnect", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Reconnect, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( Reconnect, 0));
		index += sizeof( bool);
	}
}

public class body_CS_INTEGRATED_INDUN_LOGIN_END : AsPacketHeader
{
	public bool bReconnect;
	
	// nothing...
	public body_CS_INTEGRATED_INDUN_LOGIN_END(bool reconnect)
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_INTEGRATED_INDUN_LOGIN_END;
		
		bReconnect = reconnect;
	}
}

public class body_CS_INTEGRATED_INDUN_LOGOUT : AsPacketHeader
{
	// nothing...
	public body_CS_INTEGRATED_INDUN_LOGOUT()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_INTEGRATED_INDUN_LOGOUT;
	}
}

public class body_SC_INTEGRATED_INDUN_LOGOUT_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_RETURN_WORLD : AsPacketHeader
{
	// nothing...
	public body_CS_RETURN_WORLD()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_RETURN_WORLD;
	}
}

public class body_SC_RETURN_WORLD_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunTableIndex;
	public Vector3 vPosition;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// vPosition
		byte[] position = new byte[12];
		headerinfo = infotype.GetField( "vPosition", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, position, 0, 12);
		headerinfo.SetValue( this, new Vector3( BitConverter.ToSingle( position, 0), BitConverter.ToSingle( position, 4), BitConverter.ToSingle( position, 8)));
		index += 12;
	}
}

public class body_CS_RETURN_WORLD_END : AsPacketHeader
{
	// nothing...
	public body_CS_RETURN_WORLD_END()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_RETURN_WORLD_END;
	}
}
#endregion

#region -Intergrated Indun
public class body_CS_INTEGRATED_INDUN_MATCHING_INFO : AsPacketHeader
{
	// nothing...
	public body_CS_INTEGRATED_INDUN_MATCHING_INFO()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_INTEGRATED_INDUN_MATCHING_INFO;
	}
}

public class body_SC_INTEGRATED_INDUN_MATCHING_INFO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunBranchTable_LastEnter;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nIndunBranchTable_LastEnter
		byte[] IndunBranchTable_LastEnter = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunBranchTable_LastEnter", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunBranchTable_LastEnter, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunBranchTable_LastEnter, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_INTEGRATED_INDUN_MATCHING_REQUEST : AsPacketHeader
{
	public bool bEnter;
	public Int32 nIndunTableIndex;
	public Int32 nIndunBranchTableIndex;

	public body_CS_INTEGRATED_INDUN_MATCHING_REQUEST( bool enter, Int32 indunTableIndex, Int32 indunBranchTableIndex)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_INTEGRATED_INDUN_MATCHING_REQUEST;

		bEnter = enter;
		nIndunTableIndex = indunTableIndex;
		nIndunBranchTableIndex = indunBranchTableIndex;
	}
}

public class body_SC_INTEGRATED_INDUN_MATCHING_REQUEST_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
	}
}

public class body_CS_INTEGRATED_INDUN_MATCHING_GOINTO : AsPacketHeader
{
	public bool bEnter;

	public body_CS_INTEGRATED_INDUN_MATCHING_GOINTO( bool enter)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte)PROTOCOL_CS_3.CS_INTEGRATED_INDUN_MATCHING_GOINTO;

		bEnter = enter;
	}
}

public class body_SC_INTEGRATED_INDUN_MATCHING_GOINTO_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);
	}
}

public class body_SC_INTEGRATED_INDUN_MATCHING_NOTIFY : AsPacketHeader
{
	public eRESULTCODE eResult;
	public Int32 nIndunTableIndex;
	public Int32 nIndunBranchTableIndex;
	public UInt32 nCharUniqKey;
	public string szCharName;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;

		int index = ParsePacketHeader( data);

		// eResult
		byte[] result = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "eResult", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, result, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( result, 0));
		index += sizeof( Int32);

		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);

		// nIndunBranchTableIndex
		byte[] IndunBranchTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunBranchTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunBranchTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunBranchTableIndex, 0));
		index += sizeof( Int32);

		// nCharUniqKey
		byte[] CharUniqKey = new byte[ sizeof( UInt32)];
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, CharUniqKey, 0, sizeof( UInt32));
		headerinfo.SetValue( this, BitConverter.ToUInt32( CharUniqKey, 0));
		index += sizeof( UInt32);

		// szCharName
		byte[] charName = new byte[ AsGameDefine.MAX_NAME_LEN + 1];
		Buffer.BlockCopy( data, index, charName, 0, AsGameDefine.MAX_NAME_LEN + 1);
		szCharName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( charName));
		index += ( AsGameDefine.MAX_NAME_LEN + 1);
	}
}

public class body_SC_INTEGRATED_INDUN_GOGOGO : AsPacketHeader
{
	public string szIpAddress;
	public UInt16 nPort;
	public Int32 nIndunTableIndex;
	public Int32 nIndunDuplicationIndex;
	public bool bReconnect;
	public Int32 nReconnectBranchTableIdx;
	
	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;
		
		int index = ParsePacketHeader( data);
		
		// szIpAddress
		byte[] IpAddress = new byte[ 17];
		Buffer.BlockCopy( data, index, IpAddress, 0, 17);
		szIpAddress = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( IpAddress));
		index += 17;
		
		// nPort
		byte[] Port = new byte[ sizeof( UInt16)];
		headerinfo = infotype.GetField( "nPort", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Port, 0, sizeof( UInt16));
		headerinfo.SetValue( this, BitConverter.ToUInt16( Port, 0));
		index += sizeof( UInt16);
		
		// nIndunTableIndex
		byte[] IndunTableIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunTableIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunTableIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunTableIndex, 0));
		index += sizeof( Int32);
		
		// nIndunDuplicationIndex
		byte[] IndunDuplicationIndex = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nIndunDuplicationIndex", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, IndunDuplicationIndex, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( IndunDuplicationIndex, 0));
		index += sizeof( Int32);
		
		// bReconnect
		byte[] Reconnect = new byte[ sizeof( bool)];
		headerinfo = infotype.GetField( "bReconnect", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, Reconnect, 0, sizeof( bool));
		headerinfo.SetValue( this, BitConverter.ToBoolean( Reconnect, 0));
		index += sizeof( bool);
		
		// nReconnectBranchTableIdx
		byte[] ReconnectBranchTableIdx = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nReconnectBranchTableIdx", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, ReconnectBranchTableIdx, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( ReconnectBranchTableIdx, 0));
		index += sizeof( Int32);
	}
}

public class body_SC_INTEGRATED_INDUN_REWARDINFO : AsPacketHeader
{
	public Int64 nCompleteQuest_Gold;
	public Int32 nCompleteQuest_Exp;
	public Int64 nSubQuest_Gold;
	public Int32 nSubQuest_Exp;
#if INDUN_EXCHANGE_GOLD
	public Int64 nCompleteQuest_ExchangeGold;
	public Int64 nSubQuest_ExchangeGold;
#endif
#if INDUN_TIME
	public Int64 nIndunStartTime;
	public Int64 nIndunClearTime;
#endif
}

public class body_CS_INDUN_REWARD_GETITEM : AsPacketHeader
{
	// nothing...
	public body_CS_INDUN_REWARD_GETITEM()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_INDUN_REWARD_GETITEM;
	}
}

public class body_SC_INDUN_REWARD_GETITEM_RESULT : AsPacketHeader
{
	public eRESULTCODE eResult;
	public sITEM sIndunRewardItem_0; // 0: get item
	public sITEM sIndunRewardItem_1;
	public sITEM sIndunRewardItem_2;
	public sITEM sIndunRewardItem_Bonus;
}

public class body_SC_INDUN_QUEST_PROGRESS_INFO : AsPacketHeader
{
	// nDataType: 1:int32, 2:bool
	public Int32 nInsQuestGroupTableUniqCode;
	public Int32 nDataType_1;
	public Int32 nDataType_2;
	public Int32 nDataType_3;
	public Int32 nQuestProgress_1;
	public Int32 nQuestProgress_2;
	public Int32 nQuestProgress_3;
}

public class body_SC_INDUN_ENTER_LIMITCOUNT : AsPacketHeader
{
	public Int16 nLimitCount;
}

public class body_SC_INDUN_ENTER_HELLLIMITCOUNT : AsPacketHeader
{
	public Int32 nIndunTableIndex;
	public Int16 nLimitCount;
}

#if INDUN_TIME
public class body1_SC_INDUN_CLEARINFO : AsPacketHeader
{
	public UInt32 nCharUniqKey;
	public Int32 nCnt;
	public body2_SC_INDUN_CLEARINFO[] body = null;

	public new void PacketBytesToClass( byte[] data)
	{
		Type infotype = this.GetType();
		FieldInfo headerinfo = null;
		
		int index = ParsePacketHeader( data);

		// nCharUniqKey
		byte[] userUniqKey = new byte[ sizeof( UInt32)];
		Buffer.BlockCopy( data, index, userUniqKey, 0, sizeof( UInt32));
		headerinfo = infotype.GetField( "nCharUniqKey", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt32( userUniqKey, 0));
		index += sizeof( UInt32);

		// nCnt
		byte[] count = new byte[ sizeof( Int32)];
		headerinfo = infotype.GetField( "nCnt", BINDING_FLAGS_PIG);
		Buffer.BlockCopy( data, index, count, 0, sizeof( Int32));
		headerinfo.SetValue( this, BitConverter.ToInt32( count, 0));
		index += sizeof( Int32);
		
		if( 0 >= nCnt)
			return;
		
		// body
		body = new body2_SC_INDUN_CLEARINFO[ nCnt];
		for( int i = 0; i < nCnt; i++)
		{
			body[i] = new body2_SC_INDUN_CLEARINFO();
			byte[] tmpData = new byte[ body2_SC_INDUN_CLEARINFO.size];
			Buffer.BlockCopy( data, index, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			index += body2_SC_INDUN_CLEARINFO.size;
		}
	}
}

public class body2_SC_INDUN_CLEARINFO : AsPacketHeader
{
	public static readonly int size = 24;

	public Int32 nIndunTableIndex;
	public Int32 nIndunBranchTableIndex;
	public Int64 nIndunStartTime;
	public Int64 nIndunClearTime;
}
#endif

public class body_SC_INTEGRATED_INDUN_RE_CONNECT_FAIL : AsPacketHeader
{
}

public class body_CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL : AsPacketHeader
{
	// nothing...
	public body_CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL()
	{
		Category = (byte) PACKET_CATEGORY._CATEGORY_CS3;
		Protocol = (byte) PROTOCOL_CS_3.CS_INTEGRATED_INDUN_RE_CONNECT_CANCEL;
	}
}
#endregion

#endif
