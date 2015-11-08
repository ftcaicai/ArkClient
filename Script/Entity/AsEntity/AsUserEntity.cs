//#define USE_OLD_COSTUME
#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;

public class AsUserEntity : AsBaseEntity
{
	#region - member -
	public override eEntityType EntityType	{ get	{ return eEntityType.USER; } }
	
	public override bool isIntangible { get { return false; } }
	public override bool isTrap { get { return false; } }
	public override bool isSegregate { get { return false; } }

	//debug
	public int sessionId_;
	public int uniqueId_;
	public int shopUId_;
	public int linkIndex_ = 0;

	ushort m_SessionId = ushort.MaxValue;
	uint m_UniqueId = uint.MaxValue;
	uint m_ShopUId = uint.MaxValue;

	public ushort SessionId	{ get	{ return m_SessionId; } }
	public uint UniqueId	{ get	{ return m_UniqueId; } }
	public uint ShopUId	{ get { return m_ShopUId; } }
	
#if USE_OLD_COSTUME
	private bool m_bCostumeOnOff = true;
#else
	private int m_bCostumeOnOff = 0;
#endif
	private sITEMVIEW[] m_sCharView = null;
	private sITEMVIEW[] m_sCosItemView = null;
	private int m_iHairItemIndex = 0;
	private AsChatBalloonBase prevBalloon = null;
	private StrengthenBalloon m_StrengthenBalloon;
	private AsProductBallon m_ProductBalloon;
	private AsChatBallon_PartyPR m_PartyPRBalloon;
	private bool m_isProductPrgress = false;
	private AsProductBallon m_CollectBalloon;

	private AsItemGetAlarmBallon m_ItemGetAlarmBallon;
	#region -Designation
	private int designationID = -1;
	public int DesignationID
	{
		get	{ return designationID; }
		set	{ designationID = value; }
	}
	#endregion

	#region -AccountGender
	public int nUserDelegateImageIndex;
	private eGENDER userGender;
	public eGENDER UserGender
	{
		get	{ return userGender; }
		set	{ userGender = value; }
	}
	#endregion

	#region -GMMark
	private bool isGM;
	public bool IsGM
	{
		get	{ return isGM; }
		set	{ isGM = value; }
	}
	#endregion

	private int m_nPartyIdx = 0;
	public int PartyIdx
	{
		get { return m_nPartyIdx; }
		set { m_nPartyIdx = value;}
	}

	byte[] m_ShopContent;
	public byte[] shopContent	{ get { return m_ShopContent; } }

	

	public sITEMVIEW[] getCharView
	{
		get	{ return m_sCharView; }
	}

	public sITEMVIEW[] getCosItemView
	{
		get	{ return m_sCosItemView; }
	}

	public bool WeaponEquip
	{
		get	{ return ( 0 == getCharView[0].nItemTableIdx) ? false : true; }
	}
	
#if USE_OLD_COSTUME	
	public bool isCostumeOnOff
	{
		get	{ return m_bCostumeOnOff; }
	}
	
	public void SetCostumeOnOff( bool _bCostumeOnOff)
	{
		m_bCostumeOnOff = _bCostumeOnOff;

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model)
			_Model.SetCostumeOnOff();
	}

	public void SetCostumeOnOff_Coroutine( bool _bCostumeOnOff)
	{
		m_bCostumeOnOff = _bCostumeOnOff;

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model && false == _Model.isKeepDummyObj )
			StartCoroutine( _Model.SetCostumeOnOff_Coroutine());
	}
#else
	public int isCostumeOnOff
	{
		get	{ return m_bCostumeOnOff; }
	}
	
	public void SetCostumeOnOff( int _bCostumeOnOff)
	{
		m_bCostumeOnOff = _bCostumeOnOff;

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model)
			_Model.SetCostumeOnOff();
	}

	public void SetCostumeOnOff_Coroutine( int _bCostumeOnOff)
	{
		m_bCostumeOnOff = _bCostumeOnOff;

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model && false == _Model.isKeepDummyObj )
			StartCoroutine( _Model.SetCostumeOnOff_Coroutine());
	}
#endif

	public void ReceiveItemViewSlot( int iSlot, sITEMVIEW view)
	{
		switch( iSlot)
		{
		case 0:
			SetCharView( 0, view);
			break;
		case 1:
			SetCharView( 1, view);
			break;
		case 2:
			SetCharView( 2, view);
			break;
		case 3:
			SetCharView( 3, view);
			break;
		case 4:
			SetCharView( 4, view);
			break;
		case 10:
			SetCharCosItemView( 0, view);
			break;
		case 11:
			SetCharCosItemView( 1, view);
			break;
		case 12:
			SetCharCosItemView( 2, view);
			break;
		case 13:
			SetCharCosItemView( 3, view);
			break;
		case 14:
			SetCharCosItemView( 4, view);
			break;
		case 15:
			SetCharCosItemView( Inventory.wingEquipSlotIdx, view);
			break;
		case 16:
			SetCharCosItemView( Inventory.fairyEquipSlotIdx, view);
			break;
		case 10000://$yde
			SetPetItemView( view);
			if(FsmType == eFsmType.PLAYER)
				AsPetManager.Instance.SetPetEquip(view);

//			HandleMessage(new Msg_PetItemView(view));
			break;
		default:
			Debug.LogError( "AsUserEntity::ReceiveItemViewSlot()[ error ] slot: " + iSlot +" [ item idx : " + view.nItemTableIdx);
			break;
		}
	}

	protected void SetCharView( int iViewIndex, sITEMVIEW view)
	{
		if( m_sCharView.Length <= iViewIndex)
		{
			Debug.LogError( "AsUserEntity::SetCharView() m_sCharView.Length <= iViewIndex " + iViewIndex);
			return;
		}

		m_sCharView[ iViewIndex ]  = view;
		
		Item.eEQUIP curEquip = AsModel.GetPartsType( iViewIndex);
		if( false == PartsRoot.IsEquipView( curEquip, isCostumeOnOff ) )
		{
			return;
		}

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model && false == _Model.isKeepDummyObj)
		{
			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			{
				StartCoroutine( _Model.SetEqupParts_Coroutine( iViewIndex,view));
			}
			else
			{
				_Model.SetEqupParts( iViewIndex, view);
			}
		}
	}

	protected void SetCharCosItemView( int iViewIndex, sITEMVIEW view)
	{
		if( m_sCosItemView.Length <= iViewIndex)
		{
			Debug.LogError( "AsUserEntity::SetCharCosItemView() m_sCosItemView.Length <= iViewIndex " + iViewIndex);
			return;
		}

		m_sCosItemView[ iViewIndex ]  = view;
		
		Item.eEQUIP curEquip = AsModel.GetPartsType( iViewIndex);
		if( false == PartsRoot.IsEquipView( curEquip, isCostumeOnOff ) )
		{
			return;
		}

		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null != _Model && false == _Model.isKeepDummyObj)
		{
			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
				StartCoroutine( _Model.SetCosEqupParts_Coroutine( iViewIndex, view));
			else
				_Model.SetCosEqupParts( iViewIndex, view);
		}
	}
	
	protected void SetPetItemView( sITEMVIEW _view)
	{
		Msg_PetItemView view = new Msg_PetItemView( _view);
		HandleMessage( view);
	}

	public int getHairItemIndex
	{
		get	{ return m_iHairItemIndex; }
	}
	
	public void SetHairItemIndex( int _iIndex )
	{
		m_iHairItemIndex = _iIndex;
	}

//	public void SetShopUId( uint _uid)
//	{
//		m_ShopUId = _uid;
//	}

	public void SetShopData( body2_SC_PRIVATESHOP_LIST _info)
	{
		m_ShopUId = _info.nPrivateShopUID;
		m_ShopContent = _info.strContent;

		string name = System.Text.Encoding.UTF8.GetString( _info.strName);
		SetProperty( eComponentProperty.NAME, name.Trim( '\0'));

		SetProperty( eComponentProperty.SHOP_OPENING, true);

//		Debug.Log( "AsUserEntity::SetShopData: [" + FsmType + "] m_ShopTitle = " + new string( m_ShopTitle));
	}

	public void SetShopData( body_SC_PRIVATESHOP_OPEN _open)
	{
		m_ShopUId = _open.nPrivateShopUID;
		m_ShopContent = _open.strContent;

		string name = System.Text.Encoding.UTF8.GetString( _open.strName);
		SetProperty( eComponentProperty.NAME, name.Trim( '\0'));

		SetProperty( eComponentProperty.SHOP_OPENING, true);

		SetRePosition( _open.sCurPosition);

//		Debug.Log( "AsUserEntity::SetShopData: [" + FsmType + "] m_ShopTitle = " + new string( m_ShopTitle));
	}
	
	bool m_Hide = false; public bool Hide{get{return m_Hide;}}
	public void SetHide(bool _hide){m_Hide = _hide;}
	
	//int m_nPvpPoint = 0; public int PvpPoint{ get{ return m_nPvpPoint;}}
	uint m_nYesterdayPvpRank = 0; public uint YesterdayPvpRank{ get{ return m_nYesterdayPvpRank;}}
	int m_nYesterdayPvpPoint = 0; public int YesterdayPvpPoint{ get{ return m_nYesterdayPvpPoint;}}
	uint m_nYesterdayPvpRankRate = 0; public uint YesterdayPvpRankRate{ get{ return m_nYesterdayPvpRankRate;}}
	int m_nRankPoint = 0; public int RankPoint{ get{ return m_nRankPoint;} set{ m_nRankPoint = value;}}
	bool m_bSubTitleHide = false; public bool SubTitleHide{ get{ return m_bSubTitleHide;} set{ m_bSubTitleHide = value;}}
	
	string m_StanceNumber = ""; public string StanceNumber{ get{ return m_StanceNumber;}}
	public void SetStance(StanceInfo _info)
	{
		if(_info.StancePotency == 0)
			m_StanceNumber = "";
		else
			m_StanceNumber = (_info.StancePotency + 1).ToString();
	}
	
	public Msg_CharBuffAccure buffAccure_;
	
	public bool m_PetOwned; public bool PetOwned{get{return m_PetOwned;}}
	#endregion

	#region - set creation data -
	public void SetCreationData( EntityCreationData _data)
	{
		switch( _data.creationType_)
		{
		case eCreationType.CHAR_LOAD_RESULT:
			SetCreationData( _data as CharacterLoadData);
			break;
		case eCreationType.CHAR_LOAD_RESULT_EMPTY:
			SetEmptySlotData();
			break;
		case eCreationType.OTHER_CHAR_APPEAR:
			SetCreationData( _data as OtherCharacterAppearData);
			break;
		case eCreationType.CHARACTER_SELECT:
			SetCreationData( _data as CharacterSelectEntityData);
			break;
		default:
			Debug.LogError( "AsUserEntity::SetCreationData: unknown creation type( " + _data.creationType_ + ")");
			break;
		}
	}

	public void SetCreationData( CharacterLoadData _data)
	{
		m_bCostumeOnOff = _data.bCostumeOnOff;
		m_sCharView = _data.sCharView;
		m_sCosItemView = _data.sCosItemView;
		m_iHairItemIndex = _data.nHairItemIndex;
		#region -Designation
		designationID = _data.designationID;
		#endregion

		Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( (eRACE)_data.race_, (eCLASS)_data.class_);

		sessionId_ = (int)_data.sessionKey_;
		uniqueId_ = (int)_data.uniqKey_;

		m_SessionId = _data.sessionKey_;
		m_UniqueId = _data.uniqKey_;

		m_ShopUId = AsUserInfo.Instance.LoginUserUniqueKey;

		SetProperty( eComponentProperty.NAME, AsUtil.GetRealString( _data.charName_));
		SetProperty( eComponentProperty.GUILD_NAME, AsUtil.GetRealString( _data.guildName_));
		SetProperty( eComponentProperty.RACE, _data.race_);
		SetProperty( eComponentProperty.CLASS, _data.class_);
		SetProperty( eComponentProperty.GENDER, _data.gender_);
		SetProperty( eComponentProperty.LEVEL, _data.level_);
		SetProperty( eComponentProperty.TOTAL_EXP, _data.totExp_);
		SetProperty( eComponentProperty.ATTACK_TYPE, record.attackType);

		SetProperty( eComponentProperty.SHOP_OPENING, _data.shopOpening_);
//		Debug.Log( "AsUserEntity::SetCreationData: eComponentProperty.SHOP_OPENING = " + _data.shopOpening_);

		SetProperty( eComponentProperty.HP_CUR, _data.hpCur_);
		if( null != _data.sFinalStatus)
		{
			SetProperty( eComponentProperty.HP_MAX, _data.sFinalStatus.fHPMax);
			SetProperty( eComponentProperty.MP_MAX, _data.sFinalStatus.fMPMax);
		}
		SetProperty( eComponentProperty.MP_CUR, _data.mpCur_);

		if( _data.sFinalStatus == null)
			SetProperty( eComponentProperty.MOVE_SPEED , _data.moveSpeed_);
		else
			SetProperty( eComponentProperty.MOVE_SPEED , (float)_data.sFinalStatus.nMoveSpeed * 0.01f);

		SetProperty( eComponentProperty.VIEW_DISTANCE , record.ViewDistance * 0.01f);

		Tbl_Skill_Record skill = AsTableManager.Instance.GetRandomBaseSkill( (eCLASS)_data.class_);
		Tbl_SkillLevel_Record skillLevel = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skill.Index);

		SetProperty( eComponentProperty.ATTACK_DISTANCE, skillLevel.Usable_Distance * 0.01f);

		transform.rotation = Quaternion.AngleAxis( 180, Vector3.up);

		AsEntityManager.Instance.RegisterUserCharacter( this);
	}

	public void SetEmptySlotData()
	{
		SetProperty( eComponentProperty.EMPTY_SLOT, true);
	}

	public void SetCreationData( OtherCharacterAppearData _data)
	{
		sessionId_ = (int)_data.sessionKey_;
		uniqueId_ = (int)_data.uniqKey_;
		m_SessionId = _data.sessionKey_;
		m_UniqueId = _data.uniqKey_;
		if( m_ShopUId == uint.MaxValue && _data.shopUId_ != uint.MaxValue)
			m_ShopUId = _data.shopUId_;

		if( m_ShopContent == null)
		{
			string content = "null";
			if( _data.shopContent_ != null) content = System.Text.UTF8Encoding.UTF8.GetString( _data.shopContent_);
			Debug.Log( "AsUserEntity::SetCreationData: previous shop content is null. set by OtherCharacterAppearData.shopContent[" + content + "]");
			m_ShopContent = _data.shopContent_;
		}
		else
		{
			Debug.Log( "AsUserEntity::SetCreationData: previous shop content is " +
				System.Text.UTF8Encoding.UTF8.GetString( m_ShopContent) + ". ignore shop content.");
		}

		#region -Designation
		designationID = _data.designationID;
		#endregion

		#region -AccountGender
#if !NEW_DELEGATE_IMAGE
		userGender = _data.userGender;
#endif
		#endregion

		#region -GMMark
		isGM = _data.isGM;
		#endregion

		SetProperty( eComponentProperty.NAME, AsUtil.GetRealString( _data.charName_));
		SetProperty( eComponentProperty.GUILD_NAME, AsUtil.GetRealString( _data.guildName_));
		SetProperty( eComponentProperty.RACE, _data.race_);
		SetProperty( eComponentProperty.CLASS, _data.class_);
		SetProperty( eComponentProperty.GENDER, _data.gender_);
		SetProperty( eComponentProperty.LEVEL, _data.level_);

		if( GetProperty<bool>( eComponentProperty.SHOP_OPENING) == false)
		{
			SetProperty( eComponentProperty.SHOP_OPENING, _data.shopOpening_);
//			Debug.Log( "AsUserEntity::SetCreationData: [" + _data.uniqKey_ + "]eComponentProperty.SHOP_OPENING = " + _data.shopOpening_);
		}

		Vector3 pos = _data.curPosition_; pos.y += 0.1f;
		transform.position = pos;
		transform.rotation = Quaternion.AngleAxis( 180, Vector3.up);

		SetProperty( eComponentProperty.HP_CUR, _data.hpCur_);
		SetProperty( eComponentProperty.HP_MAX, _data.hpMax_);

		SetProperty( eComponentProperty.MOVE_SPEED , _data.moveSpeed_);

		if (_data.notRegisterMgr == false)
			AsEntityManager.Instance.RegisterUserCharacter( this);

		m_iHairItemIndex = _data.hair_ + _data.hairColor_;

		m_sCharView = new sITEMVIEW[AsGameDefine.ITEM_SLOT_VIEW_COUNT];
		if( _data.surverData != null)
		{
			m_bCostumeOnOff = _data.surverData.bCostumeOnOff;

			m_sCharView[0] = _data.surverData.sNormalItemVeiw_1;
			m_sCharView[1] = _data.surverData.sNormalItemVeiw_2;
			m_sCharView[2] = _data.surverData.sNormalItemVeiw_3;
			m_sCharView[3] = _data.surverData.sNormalItemVeiw_4;
			m_sCharView[4] = _data.surverData.sNormalItemVeiw_5;
		}

		m_sCosItemView = new sITEMVIEW[AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT];
		if( _data.surverData != null)
		{
			m_sCosItemView[0] = _data.surverData.sCosItemView_1;
			m_sCosItemView[1] = _data.surverData.sCosItemView_2;
			m_sCosItemView[2] = _data.surverData.sCosItemView_3;
			m_sCosItemView[3] = _data.surverData.sCosItemView_4;
			m_sCosItemView[4] = _data.surverData.sCosItemView_5;
			m_sCosItemView[5] = _data.surverData.sCosItemView_6;
			m_sCosItemView[6] = _data.surverData.sCosItemView_7;
		}
		
		m_Hide = _data.hide_;
		//m_nPvpPoint = _data.nPvpPoint_;
		m_nYesterdayPvpRank = _data.nYesterdayPvpRank;
		m_nYesterdayPvpPoint = _data.nYesterdayPvpPoint;
		m_nYesterdayPvpRankRate = _data.nYesterdayPvpRankRate;
#if NEW_DELEGATE_IMAGE
		nUserDelegateImageIndex = _data.nDelegateImageTableIndex;
#else
		m_nRankPoint = _data.nRankPoint_;
#endif
		m_bSubTitleHide = _data.bSubTitleHide;
		
		buffAccure_ = new Msg_CharBuffAccure( _data.criticalChance, _data.dodgeChance);
		HandleMessage( buffAccure_);
	}

	public void SetCreationData( CharacterSelectEntityData _data)
	{
		sessionId_ = (int)_data.sessionKey_;
		uniqueId_ = (int)_data.uniqKey_;
		m_SessionId = _data.sessionKey_;
		m_UniqueId = _data.uniqKey_;

		SetProperty( eComponentProperty.NAME, _data.charName_);
		SetProperty( eComponentProperty.RACE, _data.race_);
		SetProperty( eComponentProperty.CLASS, _data.class_);
		SetProperty( eComponentProperty.LEVEL, _data.level_);

		AsEntityManager.Instance.RegisterUserCharacter( this);
	}
	#endregion

	#region - get creation data -
	/*public CharacterLoadData GetCreationData()
	{
		CharacterLoadData data = new CharacterLoadData();

		data.sessionKey_ = m_SessionId;
		data.uniqKey_ = m_UniqueId;

		data.charName_ = GetProperty<string>( eComponentProperty.NAME);

		data.race_ = (int)GetProperty<eRACE>( eComponentProperty.RACE);
		data.class_ = (int)GetProperty<eCLASS>( eComponentProperty.CLASS);
		data.level_ = GetProperty<int>( eComponentProperty.LEVEL);
		data.totExp_ = GetProperty<int>( eComponentProperty.TOTAL_EXP);

		data.hpCur_ = GetProperty<float>( eComponentProperty.HP_CUR);
		data.hpMax_ = GetProperty<float>( eComponentProperty.HP_MAX);
		data.mpCur_ = GetProperty<float>( eComponentProperty.MP_CUR);
		data.mpMax_ = GetProperty<float>( eComponentProperty.MP_MAX);

		data.moveSpeed_ = GetProperty<float>( eComponentProperty.MOVE_SPEED);
		data.attackRange_ = GetProperty<float>( eComponentProperty.ATTACK_DISTANCE);

		return data;
	}*/
	#endregion

	#region - check -
	public bool IsCheckEquipEnable( RealItem _curItem)
	{
		if( null == _curItem)
			return false;

		eCLASS __class = GetProperty<eCLASS>( eComponentProperty.CLASS);

		if( __class != _curItem.item.ItemData.needClass && eCLASS.All != _curItem.item.ItemData.needClass)
		{
//			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(9), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(9));
			return false;
		}

		int iCurLevel = GetProperty<int>( eComponentProperty.LEVEL);
		if( _curItem.item.ItemData.levelLimit > iCurLevel)
		{
//			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(9), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(1650));
			return false;
		}

		eGENDER gender = GetProperty<eGENDER>( eComponentProperty.GENDER);
		if( gender != _curItem.item.ItemData.getGender && eGENDER.eGENDER_NOTHING != _curItem.item.ItemData.getGender)
		{
			//AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(9), null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String(9), AsMessageManager.MSG_TYPE.WARNING);
//			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(9), eCHATTYPE.eCHATTYPE_SYSTEM);
			return false;
		}

		/*if( false == AsUserInfo.Instance.IsEquipChaingEnable())
		{
			AsMessageManager.Instance.InsertMessage( "can't change equip in battle");
			return false;
		}*/

		return true;
	}

	public bool IsCheckEquipEnable( Item _item)
	{
		if( null == _item)
			return false;

		eCLASS __class = GetProperty<eCLASS>( eComponentProperty.CLASS);

		if( __class != _item.ItemData.needClass && eCLASS.All != _item.ItemData.needClass)
			return false;

		int iCurLevel = GetProperty<int>( eComponentProperty.LEVEL);
		if( _item.ItemData.levelLimit > iCurLevel)
			return false;

		eGENDER gender = GetProperty<eGENDER>( eComponentProperty.GENDER);
		if( gender != _item.ItemData.getGender && eGENDER.eGENDER_NOTHING != _item.ItemData.getGender)
			return false;

		return true;
	}

	public bool IsBattleEnable( RealItem _realItem)
	{
		if( null == _realItem)
			return false;

		if( _realItem.item.ItemData.GetSubType() != (int)Item.eEQUIP.Weapon)
			return true;

		if( false == AsUserInfo.Instance.IsEquipChaingEnable())
		{
//			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(12), eCHATTYPE.eCHATTYPE_SYSTEM);
			AsEventNotifyMgr.Instance.CenterNotify.AddTradeMessage( AsTableManager.Instance.GetTbl_String(12));
			return false;
		}

		return true;
	}
	#endregion

	#region - parts -
	public void AttachParts( Item.eEQUIP equip, int iItemID)
	{
		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null == _Model)
		{
			Debug.LogError( "AsUserEntity::AttachParts() [ null == _Model ] char id : " + GetInstanceID());
			return;
		}

		if( null == _Model.partsRoot)
		{
			//Debug.LogError( "AsUserEntity::AttachParts() [  null == _Model.partsRoot ] char id : " + GetInstanceID());
			return;
		}

		_Model.partsRoot.AttachPartsUseItemId( equip, iItemID);
	}

	public void SetParts( Item.eEQUIP equip, int iItemID)
	{
		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null == _Model)
		{
			Debug.LogError( "AsUserEntity::GenerateParts() [ null == _Model ] char id : " + GetInstanceID());
			return;
		}

		if( null == _Model.partsRoot)
		{
			Debug.LogError( "AsUserEntity::AttachParts() [  null == _Model.partsRoot ] char id : " + GetInstanceID());
			return;
		}

		_Model.partsRoot.SetPartsUseItemId( equip, iItemID);
	}

	public void GenerateParts()
	{
		AsModel _Model = GetComponent( eComponentType.MODEL) as AsModel;
		if( null == _Model)
		{
			Debug.LogError( "AsUserEntity::GenerateParts() [ null == _Model ] char id : " + GetInstanceID());
			return;
		}

		_Model.partsRoot.GenerateParts();
	}
	#endregion

	#region - chat -
	public void AddChatBalloon( AsChatBalloonBase chatBalloon, string msg)
	{
		if( null == ModelObject)
			return;

		if( null != prevBalloon)
		{
			GameObject.DestroyImmediate( prevBalloon.gameObject);
			prevBalloon = null;
		}

		AsChatBalloonBase balloon = GameObject.Instantiate( chatBalloon) as AsChatBalloonBase;
		balloon.gameObject.transform.localPosition = Vector3.zero;
		balloon.Owner = this;
		balloon.SetText( msg);
		prevBalloon = balloon;

		Vector3 worldPos = ModelObject.transform.position;
		worldPos.y += characterController.height;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.y += 2.0f;
		vRes.z = 0.0f;
		balloon.gameObject.transform.position = vRes;
	}
	#endregion

	#region - Show Balloon -
	/*public void ShowStrengthenProcessImg( eITEM_STRENGTHEN_TYPE _eItemStrengthenType, eRESULTCODE _eResultCode)
    {
        if( null != m_StrengthenBalloon)
        {
            GameObject.Destroy( m_StrengthenBalloon.gameObject);
        }

        string strPath = null;

        switch ( _eItemStrengthenType)
        {
        case eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_START:
            strPath = "UI/Strengthen/strengthen_ing";
            break;
        case eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_END:
            if( eRESULTCODE.eRESULT_ITEM_STRENGTHEN_SUCCESS == _eResultCode)
            {
                strPath = "UI/Strengthen/strengthen_success";
            }
            else
            {
                strPath = "UI/Strengthen/strengthen_fail";
            }
            break;
        }

        if( null == strPath)
        {
            Debug.LogError( "ShowStrengthenProcessImg()[null == strPath] eITEM_STRENGTHEN_TYPE : " + _eItemStrengthenType + " eRESULTCODE : " + _eResultCode);
            return;
        }

        GameObject goRes = ResourceLoad.LoadGameObject( strPath);
        if( null == goRes)
        {
            Debug.LogError( "ShowStrengthenProcessImg()[( null == goRes] path : " + strPath);
            return;
        }

		GameObject goInstance = GameObject.Instantiate( goRes) as GameObject;
        m_StrengthenBalloon = goInstance.GetComponentInChildren<StrengthenBalloon>();
        if( null == m_StrengthenBalloon)
        {
            Debug.LogError( "ShowStrengthenProcessImg()[( null == StrengthenBalloon] path : " + strPath);
            return;
        }

		if( eITEM_STRENGTHEN_TYPE.eITEM_STRENGTHEN_TYPE_START == _eItemStrengthenType)
		{
			m_StrengthenBalloon.fTime = 100f;
		}
        m_StrengthenBalloon.Owner = this;
        m_StrengthenBalloon.gameObject.transform.localPosition = Vector3.zero;
        m_StrengthenBalloon.gameObject.transform.localRotation = Quaternion.identity;
        m_StrengthenBalloon.gameObject.transform.localScale = Vector3.one;

        Vector3 worldPos = ModelObject.transform.position;
        worldPos.y += characterController.height;
        Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
        Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
        vRes.y += 2.0f;
        vRes.z = 0.0f;
        m_StrengthenBalloon.gameObject.transform.position = vRes;
    }*/

	public void ShowPartyPR( bool _isActive, string szPartyNotice)
	{
		if( false == _isActive)
		{
			if( null != m_PartyPRBalloon)
				GameObject.Destroy( m_PartyPRBalloon.gameObject);
			return;
		}

		if( null != m_PartyPRBalloon && true == _isActive)
			return;

		string strPath = "UI/Optimization/Prefab/GUI_Balloon_Party";

		GameObject goInstance = ResourceLoad.CreateGameObject( strPath);
		if( null == goInstance)
		{
			Debug.LogError( "ShowPartyPR()[( null == goRes] path : " + strPath);
			return;
		}

		m_PartyPRBalloon = goInstance.GetComponentInChildren<AsChatBallon_PartyPR>();
		if( null == m_PartyPRBalloon)
		{
			GameObject.Destroy( goInstance);
			Debug.LogError( "ShowPartyPR()[( null == ChatBallon_PartyPR] path : " + strPath);
			return;
		}

		m_PartyPRBalloon.Owner = this;
		m_PartyPRBalloon.SetTitleAndContent( GetProperty<string>( eComponentProperty.NAME), AsUtil.GetRealString( szPartyNotice), m_nPartyIdx);
	}

	public void ShowItemGetAlarmBallonImg( sITEM _sitem, bool _isActive)
	{
		if( false == _isActive)
		{
			if( null != m_ItemGetAlarmBallon)
				GameObject.Destroy( m_ItemGetAlarmBallon.gameObject);
			return;
		}

		if( null == _sitem)
			return;
		
		if( null != m_ItemGetAlarmBallon)
			GameObject.Destroy( m_ItemGetAlarmBallon.gameObject);

		string strPath = "UI/AsGUI/GUI_ItemGetAlram_PC";

		GameObject goInstance = ResourceLoad.CreateGameObject( strPath);
		if( null == goInstance)
		{
			Debug.LogError( "ShowProductImg()[( null == goRes] path : " + strPath);
			return;
		}

		m_ItemGetAlarmBallon = goInstance.GetComponentInChildren<AsItemGetAlarmBallon>();
		if( null == m_ItemGetAlarmBallon)
		{
			GameObject.Destroy( goInstance);
			Debug.LogError( "ShowItemGetAlarmBallonImg()[( null == AsItemGetAlarmBallon] path : " + strPath);
			return;
		}
		m_ItemGetAlarmBallon.Owner = this;
		m_ItemGetAlarmBallon.Open( _sitem, this );
	}

	public void ShowProductImg( bool _isActive)
	{
		if( false == _isActive)
		{
			if( null != m_ProductBalloon)
				GameObject.Destroy( m_ProductBalloon.gameObject);
			m_isProductPrgress = _isActive;
			return;
		}

		if( true == m_isProductPrgress && true == _isActive)
			return;

		m_isProductPrgress = _isActive;

		string strPath = "UI/AsGUI/Product/Effect_Product_ing";

		GameObject goInstance = ResourceLoad.CreateGameObject( strPath);
		if( null == goInstance)
		{
			Debug.LogError( "ShowProductImg()[( null == goRes] path : " + strPath);
			return;
		}

		m_ProductBalloon = goInstance.GetComponentInChildren<AsProductBallon>();
		if( null == m_ProductBalloon)
		{
			GameObject.Destroy( goInstance);
			Debug.LogError( "ShowProductImg()[( null == StrengthenBalloon] path : " + strPath);
			return;
		}
		m_ProductBalloon.Owner = this;
	}

	public void ShowCollectImg( bool _isActive)
	{
		if( false == _isActive)
		{
			if( null != m_CollectBalloon)
				GameObject.Destroy( m_CollectBalloon.gameObject);
			return;
		}

		if( null != m_CollectBalloon && true == _isActive)
			return;

		string strPath = "UI/AsGUI/Product/Effect_Gathering_ing";

		GameObject goInstance = ResourceLoad.CreateGameObject( strPath);
		if( null == goInstance)
		{
			Debug.LogError( "ShowProductImg()[( null == goRes] path : " + strPath);
			return;
		}

		m_CollectBalloon = goInstance.GetComponentInChildren<AsProductBallon>();
		if( null == m_CollectBalloon)
		{
			GameObject.Destroy( goInstance);
			Debug.LogError( "ShowProductImg()[( null == StrengthenBalloon] path : " + strPath);
			return;
		}
		m_CollectBalloon.Owner = this;
	}
	#endregion
	
	#region - public -
	public Vector3 GetRandomValidPosisionInRange( float _range)
	{
		Vector3 pos = transform.position + Random.onUnitSphere * _range;

		float y = TerrainMgr.GetTerrainHeight(pos);
		if(y != 0.0f)
			return pos;
		else
			return transform.position;
	}
	
	public void OwnPet( bool _own)
	{
		m_PetOwned = _own;
	}
	#endregion
	/*public void PlayUseItemEffect( Item _item)
	{
		if( null == _item)
			return;

		if( false == _item.IsNeedSkillEffect())
			return;

		Tbl_SkillLevel_Record record = AsTableManager.Instance.GetTbl_SkillLevel_Record( _item.ItemData.itemSkillLevel, _item.ItemData.itemSkill);
		if( null == record)
			return;

		if( 0 >= record.listSkillLevelPotency.Count)
			return;


		Tbl_SkillPotencyEffect_Record effect = AsTableManager.Instance.GetTbl_SkillPotencyEffect_Record( record.listSkillLevelPotency[0].Potency_EffectIndex);
		if( effect == null)
			return;

		AsEffectManager.Instance.PlayEffect( effect.PotencyEffect_FileName, transform, false, 0);
	}*/
}
