//#define USE_OLD_COSTUME
#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System;
using System.Collections;
using System.Text;

public enum eCreationType {NONE = 0, CHAR_LOAD_RESULT, CHAR_LOAD_RESULT_EMPTY, OTHER_CHAR_APPEAR, NPC_APPEAR, CHARACTER_SELECT, PET_APPEAR}

public abstract class EntityCreationData
{
	public eCreationType creationType_ = eCreationType.NONE;
}

#region - user -
public class UserEntityCreationData : EntityCreationData
{
	public ushort sessionKey_;
	public uint uniqKey_;
	public bool shopOpening_ = false;
	public uint shopUId_ = uint.MaxValue;
}

public class CharacterLoadData : UserEntityCreationData
{
	public string charName_;
	public string guildName_;
	public int race_;
	public int class_;
	public eGENDER gender_;
	public int level_;
	public int totExp_;
	public float hpCur_;
	public float mpCur_;
	public float moveSpeed_;
	public float attackRange_;
	public UInt64 nGold;
	//public UInt64 nSphere = 0;
	public int nHairItemIndex;
	public sCLIENTSTATUS sDefaultStatus;
	public sCLIENTSTATUS sFinalStatus;
#if USE_OLD_COSTUME	
	public bool bCostumeOnOff;
#else
	public int bCostumeOnOff;
#endif
	public sITEMVIEW[] sCharView;
	public sITEMVIEW[] sCosItemView;
	#region -Designation
	public int designationID = -1;
	public bool bSubtitleHide = false;
	#endregion
	
	public CharacterLoadData(){}
	
	public CharacterLoadData( ushort _sessionKey, sCHARVIEWDATA _data)
	{
		creationType_ = eCreationType.CHAR_LOAD_RESULT;
		
		sessionKey_ = _sessionKey;
		uniqKey_ = _data.nCharUniqKey;
		
		if(uniqKey_ == AsUserInfo.Instance.nPrivateShopOpenCharUniqKey && AsUserInfo.Instance.nPrivateShopOpenCharUniqKey != 0)
		{
			shopOpening_ = true;
//			shopUId_ = AsUserInfo.Instance.np
			Debug.Log("CharacterLoadData::ctor: Shop opening(id:" + uniqKey_);
		}
		
		charName_ = AsUtil.GetRealString( Encoding.UTF8.GetString( _data.szCharName));
		guildName_ = "";
		
		race_ = (int)_data.eRace;
		class_ = (int)_data.eClass;
		gender_ = _data.eGender;
		level_ = _data.nLevel;	
		nHairItemIndex = _data.nHair + _data.nHairColor;
		
		bCostumeOnOff = _data.bCostumeOnOff;
		sCharView = new sITEMVIEW[ AsGameDefine.ITEM_SLOT_VIEW_COUNT];
		sCharView[0] = _data.sNormalItemVeiw_1;
		sCharView[1] = _data.sNormalItemVeiw_2;
		sCharView[2] = _data.sNormalItemVeiw_3;
		sCharView[3] = _data.sNormalItemVeiw_4;
		sCharView[4] = _data.sNormalItemVeiw_5;
		
		sCosItemView = new sITEMVIEW[ AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT];
		sCosItemView[0] = _data.sCosItemView_1;
		sCosItemView[1] = _data.sCosItemView_2;
		sCosItemView[2] = _data.sCosItemView_3;
		sCosItemView[3] = _data.sCosItemView_4;
		sCosItemView[4] = _data.sCosItemView_5;
		sCosItemView[5] = _data.sCosItemView_6;
		sCosItemView[6] = _data.sCosItemView_7;
	}
	
//	public CharacterLoadData(ushort _sessionKey, AS_GC_CHAR_LOAD_RESULT_2 _data)
//	{
//		creationType_ = eCreationType.CHAR_LOAD_RESULT;
//		
//		sessionKey_ = _sessionKey;
//		uniqKey_ = _data.nCharUniqKey;
//		charName_ = Encoding.UTF8.GetString(_data.szCharName);
//		
//		
//		race_ = _data.eRace;
//		class_ = _data.eClass;
////		gender_ = _data.gen;
//		level_ = _data.nLevel;
//		totExp_ = _data.nTotExp;
//		
//		hpCur_ = _data.nHpCur;
//		hpMax_ = _data.nHpMax;
//		mpCur_ = _data.nMpCur;
//		mpMax_ = _data.nMpMax;
//		
//		moveSpeed_ = _data.fMoveSpeed;
//		attackRange_ = _data.fAttDistance;
//	}
	
	public CharacterLoadData( ushort _sessionKey, body_GC_CHAR_SELECT_RESULT_2 _data)
	{
		creationType_ = eCreationType.CHAR_LOAD_RESULT;
		sessionKey_ = _sessionKey;
		uniqKey_ = _data.nCharUniqKey;
		if(uniqKey_ == AsUserInfo.Instance.nPrivateShopOpenCharUniqKey)
		{
			shopOpening_ = true;
			Debug.Log("CharacterLoadData::ctor: Shop opening(id:" + uniqKey_ + ")");
		}
		charName_ = AsUtil.GetRealString( Encoding.UTF8.GetString( _data.szCharName));
		guildName_ = AsUtil.GetRealString( Encoding.UTF8.GetString( _data.szGuildName));
		gender_ = _data.eGender;
		race_ = _data.eRace;
		class_ = _data.eClass;
		gender_ = _data.eGender;
		level_ = _data.nLevel;
		totExp_ = _data.nTotExp;
		hpCur_ = _data.nHpCur;
		mpCur_ = _data.nMpCur;
		moveSpeed_ = _data.sFinalStatus.nMoveSpeed * 0.01f;
		attackRange_ = _data.fAttDistance * 0.01f;
		sDefaultStatus = _data.sDefaultStatus;
		sFinalStatus = _data.sFinalStatus;
		nHairItemIndex = _data.nHair + _data.nHairColor;
		bCostumeOnOff = AsUserInfo.Instance.isCostumeOnOff;
		sCharView = AsUserInfo.Instance.getItemViews;
		sCosItemView = AsUserInfo.Instance.getCosItemView;
		nGold = _data.nGold;
		//nSphere = _data.nSphere;
		#region -Designation
		designationID = _data.nSubTitleTableIdx;
		bSubtitleHide = _data.bSubTitleHide;
		#endregion
	}
	
	public CharacterLoadData(ushort _sessionKey, AS_GC_CHAR_LOAD_RESULT_EMPTY _data)
	{
		creationType_ = eCreationType.CHAR_LOAD_RESULT_EMPTY;
	}
}

public class OtherCharacterAppearData : UserEntityCreationData
{
	public string charName_ = "char name is not set";
	public string guildName_ = "";

	public int race_;
	public int class_;
	public eGENDER userGender;
	public eGENDER gender_;
	#region -GMMark
	public bool isGM;
	#endregion
	
	public int hair_;
	public int hairColor_;
	
	public int level_;
	public int totExp_;
	
	public float hpCur_;
	public float hpMax_;
//	public float mpCur_;
	//public float mpMax_;
	public int designationID;
	
	public float moveSpeed_;
	public int atkSpeed_;
	
	public Vector3 curPosition_;
	public Vector3 destPosition_;
	
	public AS_SC_OTHER_CHAR_APPEAR_2 surverData;
	
	public byte[] shopContent_;
	
	public bool hide_ = false;
	
	//public int nPvpPoint_;
	public UInt32 nYesterdayPvpRank;
	public Int32 nYesterdayPvpPoint;
	public UInt32 nYesterdayPvpRankRate;
	public int nRankPoint_;
	public bool bSubTitleHide;
	
	public bool criticalChance = false;
	public bool dodgeChance = false;
	public Int32 nPetTableIdx_ = 0;
	public byte[] szPetName_ = new byte[0];
	public sITEMVIEW sPetItem_;
	public Int32 nPetLevel = 1;
	public bool notRegisterMgr = false;
#if NEW_DELEGATE_IMAGE
	public int nDelegateImageTableIndex;
#endif
	
	public OtherCharacterAppearData(body2_SC_PRIVATESHOP_LIST _data)
	{
		creationType_ = eCreationType.OTHER_CHAR_APPEAR;
		
		charName_ = "";
		
		sessionKey_ = AsPStoreManager.s_ShopOpeningSession;
		uniqKey_ = _data.nCharUniqKey;
		curPosition_ = _data.sCurPosition;
		destPosition_ = _data.sCurPosition;
//		Debug.Log("OtherCharacterAppearData::ctor: pos = " + _data.sCurPosition);
		shopContent_ = _data.strContent;
		
		shopOpening_ = true;
		shopUId_ = _data.nPrivateShopUID;
		
		Debug.Log("OtherCharacterAppearData::ctor: [log off]Shop opening(id:" + uniqKey_);
	}
	
	public OtherCharacterAppearData(body_SC_PRIVATESHOP_OPEN _data)
	{
		creationType_ = eCreationType.OTHER_CHAR_APPEAR;
		
		charName_ = "";
		
		sessionKey_ = AsPStoreManager.s_ShopOpeningSession;
		uniqKey_ = _data.nCharUniqKey;
		shopContent_ = _data.strContent;
		
		shopOpening_ = true;
		shopUId_ = _data.nPrivateShopUID;
		
		Debug.Log("OtherCharacterAppearData::ctor: [log off]Shop opening(id:" + uniqKey_);
	}
	
	public OtherCharacterAppearData(AS_SC_OTHER_CHAR_APPEAR_2 _data)
	{
		creationType_ = eCreationType.OTHER_CHAR_APPEAR;
		
		sessionKey_ = _data.nSessionIdx;
		uniqKey_ = _data.nCharUniqKey;
		charName_ = Encoding.UTF8.GetString(_data.szCharName);
		guildName_ = Encoding.UTF8.GetString( _data.szGuildName);
		
		race_ = _data.eRace;
		class_ = _data.eClass;
#if !NEW_DELEGATE_IMAGE
		userGender = _data.eUserGender;
#endif
		gender_ = _data.eGender;
		#region -GMMark
		isGM = _data.bIsGM;
		#endregion
		hair_ = _data.nHair;
		hairColor_ = _data.nHairColor;
		
		level_ = _data.nLevel;
//		totExp_ = _data.nTotExp;
		
		hpCur_ = _data.fHpCur;
		hpMax_ = _data.fHpMax;
//		mpCur_ = _data.nMpCur;
//		mpMax_ = _data.nMpMax;
		designationID = _data.nSubTitleTableIdx;
		
		moveSpeed_ = _data.nMoveSpeed * 0.01f;
		atkSpeed_ = _data.nAtkSpeed;
		
		curPosition_ = _data.sCurPosition;
		destPosition_ = _data.sDestPosition;
//		attDistance_ = _data.fAttDistance;
		
		surverData = _data;
		
//		shopOpening_ = _data.bIsPrivateShop;
		if(shopOpening_ == true)
			Debug.Log("OtherCharacterAppearData::ctor: Shop opening(id:" + uniqKey_);
		
		hide_ = _data.bHide;
		//nPvpPoint_ = _data.nPvpPoint;
		nYesterdayPvpRank = _data.nYesterdayPvpRank;
		nYesterdayPvpPoint = _data.nYesterdayPvpPoint;
		nYesterdayPvpRankRate = _data.nYesterdayPvpRankRate;
#if !NEW_DELEGATE_IMAGE
		nRankPoint_ = _data.nRankPoint;
#endif
		bSubTitleHide = _data.bSubTitleHide;
		
		criticalChance = _data.bCriticalChance;
		dodgeChance = _data.bDodgeChance;
		nPetTableIdx_ = _data.nPetTableIdx;
		szPetName_ = _data.szPetName;
		sPetItem_ = _data.sPetItem;
//		nPetLevel = _data.nPetLevel;

#if NEW_DELEGATE_IMAGE
		nDelegateImageTableIndex = _data.nDelegateImageTableIndex;
#endif
	}
}

public class CharacterSelectEntityData : UserEntityCreationData
{
	public string charName_;

	public int race_;
	public int class_;
	public int level_;
	
	public float moveSpeed_;
	
	public Vector3 curPosition_;
	public Vector3 destPosition_;
	
	public CharacterSelectEntityData(AS_SC_OTHER_CHAR_APPEAR_2 _data)
	{
		creationType_ = eCreationType.OTHER_CHAR_APPEAR;
		
		sessionKey_ = _data.nSessionIdx;
		uniqKey_ = _data.nCharUniqKey;
		charName_ = Encoding.UTF8.GetString(_data.szCharName);
		
		race_ = _data.eRace;
		class_ = _data.eClass;
//		level_ = _data.nLevel;
//		totExp_ = _data.nTotExp;
		
//		hpCur_ = _data.nHpCur;
//		hpMax_ = _data.nHpMax;
//		mpCur_ = _data.nMpCur;
//		mpMax_ = _data.nMpMax;
		
//		moveSpeed_ = _data.fMoveSpeed;
		
//		curPosition_ = _data.sCurPosition;
//		destPosition_ = _data.sDestPosition;
//		attDistance_ = _data.fAttDistance;
	}
}
#endregion

#region - npc -
public abstract class NpcEntityCreationData : EntityCreationData
{
	public Int32 npcIdx_;
	public eNPCType npcType_;
}

public class NpcAppearData : NpcEntityCreationData
{
	public int nameHead_;
	public string charName_;
	
	public int tableIdx_;
	
	public float hpMax_;
	public float hpCur_;
	public float mpMax_;
	public float mpCur_;
	
	public float atkSpeed_;
	
	public Vector3 curPosition_;
	public float curRotate;
	
	public NpcAppearData(AS_SC_NPC_APPEAR_2 _data)
	{
		creationType_ = eCreationType.NPC_APPEAR;
		
		//SUPER
		npcIdx_ = _data.nNpcIdx;
		Tbl_Npc_Record record = AsTableManager.Instance.GetTbl_Npc_Record(_data.nNpcTableIdx);
		npcType_ = record.NpcType;
		
		//CUR
		nameHead_ = _data.nNameHead;
		charName_ = record.NpcName;
		tableIdx_ = _data.nNpcTableIdx;
		
		hpMax_ = _data.nHpMax;
		hpCur_ = _data.nHpCur;
		mpMax_ = _data.nMpMax;
		mpCur_ = _data.nMpCur;
		
		atkSpeed_ = _data.nAtkSpeed * 0.001f;

		curPosition_ = _data.sCurPosition;
		curRotate = _data.fCurRotate;
	}
}

public class PetAppearData : EntityCreationData
{
	public AsUserEntity owner_;
	
	public bool initial_ = false;
	
	public int		nPetTableIdx_;
	public int		nPetPersonality_;
	public byte[]		szPetName_;
	public int		nLevel_;
	public int		nExp_;
	public sPETSKILL[]	sSkill_ = new sPETSKILL[ (int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX]{ new sPETSKILL(), new sPETSKILL(), new sPETSKILL()};
	public int		itemIdx_;
	
//	public PetAppearData( AsUserEntity _owner, body_SC_PET_LOAD _load)
//	{
//		creationType_ = eCreationType.PET_APPEAR;
//		
//		initial_ = true;
//
//		owner_ = _owner;
//		
//		nPetTableIdx_ = _load.nPetTableIdx;
//		szPetName_ = _load.szPetName;
//
//		if(_load.sViewItem != null)
//			itemIdx_ = _load.sViewItem.nItemTableIdx;
//	}
	
	public PetAppearData( AsUserEntity _owner, body_SC_PET_CALL _call)
	{
		creationType_ = eCreationType.PET_APPEAR;
		
		initial_ = true;
		
		owner_ = _owner;
		
		nPetTableIdx_ = _call.nPetTableIdx;
		nPetPersonality_ = _call.nPersonality;
		szPetName_ = _call.szPetName;

		if(_call.sViewItem != null)
			itemIdx_ = _call.sViewItem.nItemTableIdx;
	}
	
	public PetAppearData( AsUserEntity _owner, OtherCharacterAppearData _appear)
	{
		creationType_ = eCreationType.PET_APPEAR;

		initial_ = false;

		owner_ = _owner;
		
		nPetTableIdx_ = _appear.nPetTableIdx_;
		szPetName_ = _appear.szPetName_;
		nLevel_ = _appear.nPetLevel;
		nExp_ = 1;

		if(_appear.sPetItem_ != null)
			itemIdx_ = _appear.sPetItem_.nItemTableIdx;
	}
	
	public PetAppearData( AsUserEntity _owner, PetInfo _info)
	{
		creationType_ = eCreationType.PET_APPEAR;

		initial_ = false;
		
		owner_ = _owner;
		
		nPetTableIdx_ = _info.nPetTableIdx;
		nPetPersonality_ = _info.nPersonality;
		szPetName_ = _info.szPetName;
		nLevel_ = _info.nLevel;
		nExp_ = _info.nExp;
		sSkill_ = _info.sSkill;
		
		itemIdx_ = _info.itemView.nItemTableIdx;
	}
}
#endregion

#region - cahracter select -
//public class CharSelectEntityCreationData : EntityCreationData
//{
//	public Int32 sessionKey_;
//}
//
//public class NpcAppearData : NpcEntityCreationData
//{
//	public string charName_;
//	
//	public int tableIdx_;
//
//	public int race_;
//	public int class_;
//	public int level_;
//	
//	public float moveSpeed_;
//	
//	public Vector3 curPosition_;
//	public Vector3 destPosition_;
//	
//	public NpcAppearData(AS_SC_NPC_APPEAR_2 _data)
//	{
//		creationType_ = eCreationType.NPC_APPEAR;
//		
//		sessionKey_ = _data.nNpcIdx;
//		tableIdx_ = _data.nNpcTableIdx;
//		
////		race_ = _data.eRace;
////		class_ = _data.eClass;
////		level_ = _data.nLevel;
//
////		moveSpeed_ = _data.fMoveSpeed;
//		
//		curPosition_ = _data.sCurPosition;
////		destPosition_ = _data.sDestPosition;
//	}
//}
#endregion