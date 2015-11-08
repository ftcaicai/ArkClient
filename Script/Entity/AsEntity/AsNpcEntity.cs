using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsNpcEntity : AsBaseEntity
{
	#region - memeber -
	public override eEntityType EntityType	{ get { return eEntityType.NPC; } }
	bool m_IsIntangible = false; public override bool isIntangible	{ get { return m_IsIntangible; } }
	bool m_IsTrap = false; public override bool isTrap	{ get { return m_IsTrap; } }
	public override bool isSegregate { get { return m_IsIntangible || m_IsTrap; } }

	//debug
	public int sessionId_;
	int m_SessionId = int.MaxValue;
	public int SessionId	{ get { return m_SessionId; } }
	int m_iTableIdx = 0;
	public int TableIdx	{ get { return m_iTableIdx; } }
	public float fCollisionRadius = 0.0f;
	private AsChatBalloonBase prevBalloon = null;

	private List<AS_body2_SC_DROPITEM_APPEAR> m_ReciveDropItem = new List<AS_body2_SC_DROPITEM_APPEAR>();
	private bool m_isEvent_npc = false;
    public  bool m_isChampion = false;
	
	int m_warpIndex = 0; 
	public int npcWarpIndex
	{
		get
		{
			return m_warpIndex;
		}
		set
		{
			m_warpIndex = value;
		}
	}
	
	public bool isNoWarpIndex	
	{
		get
		{
			return m_warpIndex == 0 || m_warpIndex == int.MaxValue;
		}
	}
	#endregion

	public void AddDropItem( AS_body2_SC_DROPITEM_APPEAR dropItem)
	{
		m_ReciveDropItem.Add( dropItem);
	}

	public List<AS_body2_SC_DROPITEM_APPEAR> getDropItems
	{
		get	{ return m_ReciveDropItem; }
	}

	#region - set creation data -
	public void SetCreationData( EntityCreationData _data)
	{
		switch( _data.creationType_)
		{
		case eCreationType.NPC_APPEAR:
			SetCreationData( _data as NpcAppearData);
			break;
		case eCreationType.PET_APPEAR:
			SetCreationData( _data as PetAppearData);
			break;
		}
	}

	public void SetCreationData( NpcAppearData _data)
	{
		m_SessionId = _data.npcIdx_;
		sessionId_ = _data.npcIdx_;

		Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record( _data.tableIdx_);
		if( npcRec == null)
		{
			Debug.LogError( "AsNpcEntity::SetCreatioinData: NO npc table NPC RECORD");
			return;
		}

		m_iTableIdx = _data.tableIdx_;

		string nameHead = "";
		if( _data.nameHead_ != 0)
			nameHead = AsTableManager.Instance.GetTbl_String( _data.nameHead_);

		SetProperty( eComponentProperty.NAME, nameHead + " " + npcRec.NpcName);
		SetProperty( eComponentProperty.NPC_ID, npcRec.Id);
		SetProperty( eComponentProperty.NPC_TYPE, npcRec.NpcType);
		fCollisionRadius = npcRec.getCollisionRadius;

		switch( _data.npcType_)
		{
		case eNPCType.Monster:
			Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record( _data.tableIdx_);

			SetProperty( eComponentProperty.CLASS, monster.Class);
			SetProperty( eComponentProperty.ATTRIBUTE, monster.ElementalIndex);
			SetProperty( eComponentProperty.GRADE, monster.Grade);
			SetProperty( eComponentProperty.MONSTER_ID, monster.Id);
			SetProperty( eComponentProperty.MONSTER_ATTACK_TYPE, monster.AttackType);
			SetProperty( eComponentProperty.MONSTER_ATTACK_STYLE, monster.AttackStyle);
			SetProperty( eComponentProperty.MONSTER_KIND_ID, monster.Monster_Kind_ID);
			SetProperty( eComponentProperty.LEVEL, monster.Level);
			SetProperty( eComponentProperty.HP_CUR, _data.hpCur_);//follow data
			SetProperty( eComponentProperty.MP_CUR, _data.mpCur_);// "
//			SetProperty( eComponentProperty.HP_MAX, ( float)monster.HPMax);
//			SetProperty( eComponentProperty.MP_MAX, ( float)monster.MPMax);
			SetProperty( eComponentProperty.HP_MAX, _data.hpMax_);
			SetProperty( eComponentProperty.MP_MAX, _data.mpMax_);
//			SetProperty( eComponentProperty.ATTACK, monster.phy);
//			SetProperty( eComponentProperty.DEFENCE, monster.Defence);
//			SetProperty( eComponentProperty.MOVE_SPEED, monster.MoveSpeed);
			SetProperty( eComponentProperty.ATTACK_SPEED, _data.atkSpeed_);
			SetProperty( eComponentProperty.VIEW_DISTANCE, monster.ViewDistance * 0.01f);
			SetProperty( eComponentProperty.CHASE_DISTANCE, monster.ChaseDistance * 0.01f);
			SetProperty( eComponentProperty.DROP_EXP, monster.DropExp);
			SetProperty( eComponentProperty.DROP_ITEM, monster.DropItem);
			SetProperty( eComponentProperty.VIEW_HOLD, monster.ViewHold);
			break;
		case eNPCType.NPC:
			Tbl_NormalNpc_Record normal = AsTableManager.Instance.GetTbl_NormalNpc_Record( _data.tableIdx_);
			SetProperty( eComponentProperty.MOVE_SPEED, normal.MoveSpeed);
			SetProperty( eComponentProperty.NPC_LIVING, normal.LivingType);

			Tbl_NormalNpc_Record normalNpc = AsTableManager.Instance.GetTbl_NormalNpc_Record( _data.tableIdx_);
			if( null != normalNpc)
			{
				if( true == normalNpc.IsHaveMenuBtn( eNPCMenu.Event_npc))
					m_isEvent_npc = true;
			}
			break;
		case eNPCType.Object:
			Tbl_Object_Record data = AsTableManager.Instance.GetTbl_Object_Record( _data.tableIdx_);
			SetProperty( eComponentProperty.OBJ_TYPE, data.PropType);
			break;
		case eNPCType.Collection:
			Tbl_Collection_Record record = AsTableManager.Instance.getColletionTable.GetRecord( _data.tableIdx_);
			if( null != record)
			{
				SetProperty( eComponentProperty.LEVEL, record.level);
				SetProperty( eComponentProperty.HP_CUR, record.time);
				SetProperty( eComponentProperty.HP_MAX, record.time);

				switch( record.technic)
				{
				case eCOLLECTION_TECHNIC.MINERAL:
					SetProperty( eComponentProperty.COLLECTOR_TECHNIC_TYPE, ( int)eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL);
					break;
				case eCOLLECTION_TECHNIC.PLANTS:
					SetProperty( eComponentProperty.COLLECTOR_TECHNIC_TYPE, ( int)eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS);
					break;
				case eCOLLECTION_TECHNIC.SPIRIT:
					SetProperty( eComponentProperty.COLLECTOR_TECHNIC_TYPE, ( int)eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT);
					break;
				case eCOLLECTION_TECHNIC.QUEST:
					SetProperty( eComponentProperty.COLLECTOR_TECHNIC_TYPE, ( int)eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST);
					break;
				}
			}
			else
			{
				Debug.LogError( "AsNpcEntity::SetCreationData()[ null == Tbl_Collection_Record ] id : " + _data.tableIdx_);
			}
			break;
		default:
			Debug.Log( "Invalid npc type");
			break;
		}

//		Vector3 pos = _data.curPosition_; pos.y += 0.1f;
//		pos.y = TerrainMgr.GetTerrainHeight(characterController, _data.curPosition_);
//		transform.position = pos;

		SetRePosition(_data.curPosition_);
		transform.rotation = Quaternion.AngleAxis( _data.curRotate, Vector3.up);

		if( _data.nameHead_ != 0)// case of CHAMPION
		{
			Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record( _data.tableIdx_);
			Tbl_MonsterChampion_Record champion = AsTableManager.Instance.GetTbl_MonsterChampion_Record( monster.Champion);
			transform.localScale = new Vector3( champion.Scale, champion.Scale, champion.Scale);

			SetProperty( eComponentProperty.GRADE, eMonster_Grade.Champion);

            m_isChampion = true;
		}
		else
		{
			transform.localScale = new Vector3( npcRec.Scale, npcRec.Scale, npcRec.Scale); // ilmeda, 20120816
            m_isChampion = false;
		}

		AsEntityManager.Instance.RegisterNpcCharacter( this);
		
//		bool intangible = false;
		// destroy components
		if( _data.npcType_ == eNPCType.Monster)
		{
			Tbl_Monster_Record monster = AsTableManager.Instance.GetTbl_Monster_Record( _data.tableIdx_);
			if( monster.Grade == eMonster_Grade.Intangible)
			{
				DetachComponent( typeof( AsAnimation));
				DetachComponent( typeof( AsModel));
				DetachComponent( typeof( AsMover));
				
				m_IsIntangible = true;
//				intangible = true;
			}
			
			if( monster.Grade == eMonster_Grade.Trap)
			{
//				DetachComponent( typeof( AsMover));
				
				m_IsTrap = true;
				
//				namePanel.gameObject.SetActive( false);
			}
		}
		
//		if( intangible == false)
//			AsEntityManager.Instance.RegisterNpcCharacter( this);
	}
	
	public void SetCreationData( PetAppearData _data)
	{
		m_SessionId = -1;
		sessionId_ = -1;

		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord( _data.nPetTableIdx_);
		if( petRec == null)
		{
			Debug.LogError( "AsNpcEntity::SetCreatioinData: NO pet table PET RECORD");
			return;
		}

		m_iTableIdx = _data.nPetTableIdx_;
		
		SetProperty( eComponentProperty.CLASS, petRec.Class);
		
		string name = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( _data.szPetName_));
		SetProperty( eComponentProperty.NAME, name);
		SetProperty( eComponentProperty.PET_ID, petRec.Index);
		SetProperty( eComponentProperty.LEVEL, _data.nLevel_);
//		Tbl_PetLevel_Record lvRec = AsTableManager.Instance.GetPetLevelRecord( _data.nLevel_);
//		SetProperty( eComponentProperty.EVOLUTION_LEVEL, lvRec.EvolutionLv);
		
//		SetProperty( eComponentProperty.MOVING, false);
//		SetProperty( eComponentProperty.MOVE_TYPE, false);
//		SetProperty( eComponentProperty.MOVE_SPEED, false);
		
		Vector3 pos = _data.owner_.GetRandomValidPosisionInRange( AsBaseEntity.s_PetRange);
		SetRePosition( pos);
		transform.rotation = _data.owner_.transform.rotation;
	}
	#endregion

	#region - attribute -
	public void AttributeChange( body_SC_NPC_ATTR_CHANGE _change)
	{
		switch( _change.eChangeType)
		{
		case eCHANGE_INFO_TYPE.eCHANGE_INFO_ATTACK_SPEED:
			SetProperty( eComponentProperty.ATTACK_SPEED, _change.nChangeValue * 0.001f);
			break;
		}
	}
	#endregion

	public AsChatBalloonBase AddChatBalloon( AsChatBalloonBase chatBalloon, string msg)
	{
		if( null == ModelObject)
			return null;

		DeleteChatBalloon();

		AsChatBalloonBase balloon = GameObject.Instantiate( chatBalloon) as AsChatBalloonBase;
		balloon.gameObject.transform.localPosition = Vector3.zero;
		balloon.Owner = this;
		balloon.SetText( msg);
		balloon.isNeedDelete = false;
		prevBalloon = balloon;

		Vector3 worldPos = ModelObject.transform.position;
		worldPos.y += characterController.height;

		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.y += 2.0f;
		vRes.z = 0.0f;
		balloon.gameObject.transform.position = vRes;
		
		return balloon;
	}

	public void DeleteChatBalloon()
	{
		if( null == ModelObject)
			return;

		if( null != prevBalloon)
		{
			GameObject.DestroyImmediate( prevBalloon.gameObject);
			prevBalloon = null;
		}
	}

	void Update()
	{
		if( true == m_isEvent_npc && null != AsEventManager.Instance)
		{
			int iCount = AsEventManager.Instance.GetEventCount();
			if( iCount > 0 && null == prevBalloon && null != AsChatFullPanel.Instance)
				AddChatBalloon( AsChatFullPanel.Instance.chatBalloon, AsTableManager.Instance.GetTbl_String( 1994));
			else if( iCount <= 0)
				DeleteChatBalloon();
		}
	}

	#region - getter -
	public bool GetClickable()
	{
		Tbl_NormalNpc_Record npcRec = AsTableManager.Instance.GetTbl_NormalNpc_Record( m_iTableIdx);
		if( npcRec != null)
			return npcRec.Touchable;
		else
		{
			Debug.LogError( "AsNpcEntity::GetClickable: no normal npc record [" + m_iTableIdx + "]");
			return false;
		}
	}
	#endregion
	
	#region - public -
	public void SetPetItemView( Msg_PetItemView _view)
	{
		if( _view.view_.nItemTableIdx > 0)
		{
			Item item = ItemMgr.ItemManagement.GetItem( _view.view_.nItemTableIdx);
			SetPetItemView( item);
		}
		else
		{
			_ReleasePetItemView();
		}
	}
	public void SetPetItemView( int _idx)
	{
		if( _idx > 0)
		{
			Item item = ItemMgr.ItemManagement.GetItem( _idx);
			SetPetItemView( item);
		}
		else
		{
			_ReleasePetItemView();
		}
	}
	GameObject m_AccObj;
	void SetPetItemView( Item _item)
	{
		if( _item != null)
		{
			StartCoroutine(SetPetItemView_CR(_item));
		}
	}

	public bool IsAccLoadFinish { get { return m_AccObj != null; } }
	bool m_AccLoadFinish = true;
	IEnumerator SetPetItemView_CR(Item _item)
	{
		m_AccLoadFinish = false;

		while(true)
		{
			yield return null;
			
			if(CheckModelLoadingState() == eModelLoadingState.Finished)
				break;
		}

		AsPreloadManager.Instance.LoadModeling(_item.ItemData.GetPartsItem_M(), OnAccLoaded);

		m_AccLoadFinish = true;
	}
	void OnAccLoaded(GameObject _acc)
	{
		if(m_AccObj != null) DestroyImmediate(m_AccObj);

		GameObject obj = _acc;
		Transform accTrn = SearchHierarchyTransform( obj.transform, "Dummy_Acc_Attach");
		Transform modelTrn = GetDummyTransform( "Dummy_Acc");
		if(modelTrn == null)
		{
			Debug.LogWarning("AsNpcEntity:: OnAccLoaded: Dummy_Acc transform is not found");
			Destroy(_acc);
			return;
		}
		
		Transform root = FindRoot( accTrn);
		
		accTrn.parent = modelTrn;
		accTrn.transform.localPosition = Vector3.zero;
		//		accTrn.transform.localScale = Vector3.one;
		accTrn.transform.localRotation = Quaternion.identity;
		
		Destroy( root.gameObject);

//		accTrn.gameObject.AddComponent<DestroyDetector>();
		m_AccObj = accTrn.gameObject;
	}

	void _ReleasePetItemView()
	{
		StartCoroutine(ReleasePetItemView_CR());
	}
	IEnumerator ReleasePetItemView_CR()
	{
		while(true)
		{
			if(m_AccLoadFinish == true)
				break;

			yield return null;
		}

		if( m_AccObj != null)
			Destroy( m_AccObj);
	}
	#endregion
	
	#region - method -
	Transform SearchHierarchyTransform( Transform _trn, string _name)
	{
		if( _trn.name == _name)
			return _trn;
		
//		for( int i = 0; i < _trn.GetChildCount(); ++i)
		foreach(Transform trn in _trn)
		{
//			Transform child = SearchHierarchyTransform( _trn.GetChild(i), _name);
			Transform child = SearchHierarchyTransform( trn, _name);
			if( child != null)
				return child;
		}
		
		return null;
	}
	
	Transform FindRoot( Transform _trn)
	{
		Transform curTrn = _trn;
		
		while(true)
		{
			if( curTrn.parent == null)
				break;
			else
				curTrn = curTrn.parent;
		}
		
		return curTrn;
	}
	#endregion
}
