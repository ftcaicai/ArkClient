#define AUTOMOVE_EFFECT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsEntityTemplate
{
	public List<string> m_listComponent = new List<string>();
	public AsPropertySet m_PropertySet = new AsPropertySet();

	public AsEntityTemplate(){}

	public AsEntityTemplate( Tbl_EntityTemplate_Record _record)
	{
		m_listComponent = _record.ListComponent;

		foreach( Tbl_EntityTemplate_Record.AsProperty prop in _record.ListProperty)
		{
			AsProperty property = m_PropertySet.InitProperty( prop.name_, prop.varType_);
			property.SetDefaultValueAsString( prop.default_);
		}
	}
}

public class AsEntityManager : MonoBehaviour
{
	#region - member -
	#region - singleton -
	static AsEntityManager m_instance;
	public static AsEntityManager Instance	{ get { return m_instance; } }
	#endregion
	Dictionary<string, AsEntityTemplate > m_dicEntityTemplate = new Dictionary<string, AsEntityTemplate>();
	GameObject m_RootObject;

	AsUserEntity m_kUserEntity = null;
	public AsUserEntity UserEntity	{ get { return m_kUserEntity; } }

	AsPlayerFsm m_PlayerFsm = null;

	//users
	MultiDictionary<ushort, AsUserEntity> m_mdicUserEntity_SessionId = new MultiDictionary<ushort, AsUserEntity>();// session id
	Dictionary<uint, AsUserEntity> m_dicUserEntity_UniqueId = new Dictionary<uint, AsUserEntity>();// unique id( user only)

	//npc
	Dictionary<int, AsNpcEntity> m_dicNpcEntity_SessionId = new Dictionary<int, AsNpcEntity>();// npc session id

	//common
	Dictionary<int, AsBaseEntity> m_dicEntity_InstanceId = new Dictionary<int, AsBaseEntity>();// instance id( GetInstanceID())
	#endregion

	void Awake()
	{
		m_instance = this;
		m_RootObject = new GameObject( "Entities");
		DontDestroyOnLoad( m_RootObject);
		DDOL_Tracer.RegisterDDOL( this, m_RootObject);//$ yde

		//LoadEntityTemplates( "EntityTemplate/EntityTemplate"); // move to AsTableManager.LoadTable()
	}

	void Start()
	{
		//InvokeRepeating( "InvokeUnloadAssets", 3, 3);
	}

	void OnEnable()
	{
		m_queLoadingModel.Clear();
		m_ModelLoading = false;

		if( m_queWaitingEntity.Count != 0)
		{
			Debug.Log( "AsEntityManager::OnEnable: m_queWaitingEntity.Count == " + m_queWaitingEntity.Count + ". StartCoroutine( DestroyingSequence())");
			StartCoroutine( DestroyingSequence());
		}

		Debug.Log( "AsEntityManager::OnEnable: ends");
	}

	void OnDisable()
	{
		Debug.Log( "AsEntityManager::OnDisable:");

		if( m_ModelLoading == true)
			Debug.LogWarning( "AsEntityManager::OnDisable: ModelLoading coroutine is not finished correctly.");
		m_ModelLoading = false;

		if( m_DestroyingEntity == true)
			Debug.LogWarning( "AsEntityManager::OnDisable: Destroying coroutine is not finished correctly.");
		m_DestroyingEntity = false;
	}

	void OnDestroy()
	{
//		Debug.Log( "AsEntityManager::OnDestroy:");
	}

	public void LoadEntityTemplates( string _path)
	{
//		InitByJnXml( _path);
		InitFromTableBase();
	}

	bool InitFromTableBase()
	{
		//Dictionary<string, AsEntityTemplate > dicEntityTemplate = AsTableManager.Instance.GetEntityTemplate();
		Tbl_EntityTemplate_Table m_Tbl_EntityTemplate = new Tbl_EntityTemplate_Table( "Table/EntityTemplate");
		Dictionary<string, AsEntityTemplate > dicEntityTemplate = m_Tbl_EntityTemplate.GetEntityTemplate();

		if( dicEntityTemplate != null)
		{
			m_dicEntityTemplate = dicEntityTemplate;
			return true;
		}
		else
		{
			Debug.LogError( "[AsEntityManager]InitFromTableBase: error while template loading");
			return false;
		}
	}

	#region - obsolete -
	bool InitByJnXml( string _path)
	{
		try
		{
			TextAsset xmlFile = Resources.Load( _path) as TextAsset;
			JnXmlReader reader = JnXmlReader.FromString( xmlFile.text);
			if( reader.Goto( "/EntityTemplate/Entity")) do
			{
				AsEntityTemplate entT = new AsEntityTemplate();
				string entityType = reader.ReadAttrAsString( "Type");

				if( reader.FirstChild( "Component"))
				{
					do
					{
						string componentName = reader.ReadAttrAsString( "Name");
						entT.m_listComponent.Add( componentName);
					}
					while( reader.NextSibling());
					reader.Parent();
				}

				if( reader.FirstChild( "AsProperty"))
				{
					do
					{
						string propertyName = reader.ReadAttrAsString( "Name");
						AsProperty prop = entT.m_PropertySet.InitProperty( propertyName, reader.ReadAttrAsString( "VarType"));
						prop.SetDefaultValueAsString( reader.ReadAttrAsString( "Default"));
					}
					while( reader.NextSibling());
					reader.Parent();
				}

				m_dicEntityTemplate.Add( entityType, entT);
			}
			while( reader.NextSibling());
		}
		catch ( System.Exception e)
		{
			Debug.Log( e.Message);
			return false;
		}

		return true;
	}
	#endregion

	#region - process -
	public AsUserEntity CreateUserEntity( string _typeName, UserEntityCreationData _data, bool needShadow=true, bool needAutoEffect = false, float _fsize = float.MaxValue )
	{
		if( false == m_dicUserEntity_UniqueId.ContainsKey( _data.uniqKey_) || 0 == string.Compare( "EmptySlot", _typeName))
		{
			AsUserEntity entity;

			if( true == m_dicEntityTemplate.ContainsKey( _typeName))
			{
				GameObject obj = new GameObject( _typeName);
				entity = obj.AddComponent<AsUserEntity>();
				entity.AttachComponent( m_dicEntityTemplate[ _typeName]);
				entity.InitComponents();
				obj.transform.parent = m_RootObject.transform;
				if( float.MaxValue != _fsize )
					obj.transform.localScale = new Vector3( _fsize, _fsize, _fsize);
			}
			else
			{
				Debug.LogError( "[AsEntityManager]CreateEntity: invalid entity name");
				return null;
			}
#if AUTOMOVE_EFFECT
			entity.SetNeedAutoMoveEffect( needAutoEffect);
#endif

			entity.NeedShadow = needShadow;
			entity.SetCreationData( _data);
			entity.InterInitComponents();
			entity.LateInitComponents();
			entity.LastInitComponents();

			entity.SetEnterWorldData( entity.transform.position);

			if( "PlayerChar" == _typeName)
			{
				m_kUserEntity = entity;
				m_PlayerFsm = m_kUserEntity.GetComponent<AsPlayerFsm>();
			}

			return entity;
		}
		else
		{
			AsUserEntity entity = m_dicUserEntity_UniqueId[_data.uniqKey_];
			if( entity.CheckShopOpening() == true)
			{
				entity.SetCreationData( _data);
				entity.InterInitComponents();
				entity.LateInitComponents();
				entity.LastInitComponents();
			}

			//Debug.LogError( "[AsEntityManager]CreateEntity: [" + AsGameMain.s_gameState + "] type:" + _data.creationType_ + " is already registered. invalid id[" + _data.uniqKey_ + "]");
			return entity;
		}
	}

	public AsNpcEntity CreateNpcEntity( string _typeName, NpcEntityCreationData _data)
	{
		if( m_dicNpcEntity_SessionId.ContainsKey( _data.npcIdx_) == false)
		{
			AsNpcEntity entity;

			if( m_dicEntityTemplate.ContainsKey( _typeName) == true)
			{
				GameObject obj = new GameObject( _typeName);
				entity = obj.AddComponent<AsNpcEntity>();
				entity.AttachComponent( m_dicEntityTemplate[_typeName]);
				entity.InitComponents();
				obj.transform.parent = m_RootObject.transform;
			}
			else
			{
				Debug.LogError( "[AsEntityManager]CreateEntity: invalid entity name");
				return null;
			}

			entity.SetCreationData( _data);
//			entity.HandleMessage( new Msg_ModelInitialize( eCreationType.NPC_APPEAR));

			entity.InterInitComponents();
			entity.LateInitComponents();
			entity.LastInitComponents();

			// npc name panel
			//AsHUDController.Instance.panelManager.CreateNamePanel_Npc( entity, ( ( NpcAppearData)_data).charName_, ( ( NpcAppearData)_data).tableIdx_);

			return entity;
		}
		else
		{
			Debug.LogError( "[AsEntityManager]CreateEntity: id is already registered. invalid id");
			return null;
		}
	}

	public AsUserEntity CreateDummyEntity( string _typeName)
	{
		AsUserEntity entity;

		if( m_dicEntityTemplate.ContainsKey( _typeName) == true)
		{
			GameObject obj = new GameObject( _typeName);
			entity = obj.AddComponent<AsUserEntity>();
			entity.AttachComponent( m_dicEntityTemplate[_typeName]);
			entity.InitComponents();
			obj.transform.parent = m_RootObject.transform;

//			m_dicUserEntity_SessionId.Add( entity.SessionId, entity);
//			m_dicUserEntity_UniqueId.Add( entity.UniqueId, entity);
//			m_dicEntity_InstanceId.Add( entity.GetInstanceID(), entity);

			return entity;
		}
		else
		{
			Debug.LogError( "[AsEntityManager]CreateEntity: invalid entity name");
			return null;
		}
	}

	public void RemoveEntity( AsUserEntity _entity)
	{
		if (m_mdicUserEntity_SessionId.ContainsKey(_entity.SessionId))
			m_mdicUserEntity_SessionId.Remove( _entity.SessionId, _entity);
		
		if (m_dicUserEntity_UniqueId.ContainsKey( _entity.UniqueId))
			m_dicUserEntity_UniqueId.Remove( _entity.UniqueId);

		if (m_dicEntity_InstanceId.ContainsKey(_entity.gameObject.GetInstanceID()))
			m_dicEntity_InstanceId.Remove( _entity.gameObject.GetInstanceID());

		RequestDestroy( _entity);
//		Destroy( _entity.gameObject);

		UnloadAssetsForMemory();
	}

	public void RemoveEntity( AsNpcEntity _entity)
	{
		m_dicNpcEntity_SessionId.Remove( _entity.SessionId);
		m_dicEntity_InstanceId.Remove( _entity.gameObject.GetInstanceID());

		AsUseItemToTargetPanelManager.instance.DeletePanel( _entity);
		
		if( _entity.isSegregate == false)
		{
			RequestDestroy( _entity);
	
			UnloadAssetsForMemory();
		}
		else
			Destroy( _entity.gameObject);
	}

	public void RemovePlayer()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();

		m_mdicUserEntity_SessionId.Remove( user.SessionId, user);
		m_dicUserEntity_UniqueId.Remove( user.UniqueId);
		m_dicEntity_InstanceId.Remove( user.gameObject.GetInstanceID());

		RequestDestroy( user);
//		Destroy( user.gameObject);

		UnloadAssetsForMemory();
	}
	
	public void RemovePet( AsNpcEntity _pet)
	{
		RequestDestroy( _pet);
	}

	public void RemoveAllOtherUser()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == user)
			return;

		List<AsUserEntity> listEntity = new List<AsUserEntity>();

		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			if( m_dicEntity_InstanceId.ContainsKey( pair.Value.gameObject.GetInstanceID()) == true &&
				user != pair.Value)
			{
				listEntity.Add( pair.Value);
			}
		}

		foreach( AsUserEntity node in listEntity)
		{
			m_dicEntity_InstanceId.Remove( node.gameObject.GetInstanceID());
			RequestDestroy( node);
//			Destroy( node.gameObject);
		}

		m_mdicUserEntity_SessionId.Clear();
		m_dicUserEntity_UniqueId.Clear();

		m_mdicUserEntity_SessionId.Add( user.SessionId, user);
		m_dicUserEntity_UniqueId.Add( user.UniqueId, user);

		UnloadAssetsForMemory();
	}

	public void RemoveAllNpc()
	{
		List<AsNpcEntity> listEntity = new List<AsNpcEntity>();

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.FsmType == eFsmType.NPC ||
				pair.Value.FsmType == eFsmType.OBJECT ||
				pair.Value.FsmType == eFsmType.COLLECTION)
			{
				listEntity.Add( pair.Value);
//				m_dicNpcEntity_SessionId.Remove( pair.Value.SessionId);
//				m_dicEntity_InstanceId.Remove( pair.Value.gameObject.GetInstanceID());
//				Destroy( pair.Value.gameObject);

				if (pair.Value.FsmType == eFsmType.NPC)
				    AsUseItemToTargetPanelManager.instance.DeletePanel(pair.Value);
			}
		}

		foreach( AsNpcEntity node in listEntity)
		{
			m_dicNpcEntity_SessionId.Remove( node.SessionId);
			m_dicEntity_InstanceId.Remove( node.gameObject.GetInstanceID());
			RequestDestroy( node);
//			Destroy( node.gameObject);
		}

		UnloadAssetsForMemory();
	}

	public void RemoveAllMonster()
	{
		List<AsNpcEntity> listEntity = new List<AsNpcEntity>();

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.FsmType == eFsmType.MONSTER)
			{
				listEntity.Add( pair.Value);
//				m_dicNpcEntity_SessionId.Remove( pair.Value.SessionId);
//				m_dicEntity_InstanceId.Remove( pair.Value.gameObject.GetInstanceID());
//				Destroy( pair.Value.gameObject);
			}
		}

		foreach( AsNpcEntity node in listEntity)
		{
			m_dicNpcEntity_SessionId.Remove( node.SessionId);
			m_dicEntity_InstanceId.Remove( node.gameObject.GetInstanceID());
			RequestDestroy( node);
//			Destroy( node.gameObject);
		}

		UnloadAssetsForMemory();
	}

	public void RemoveAllEntities()
	{
		m_mdicUserEntity_SessionId.Clear();
		m_dicUserEntity_UniqueId.Clear();

		m_dicNpcEntity_SessionId.Clear();

		foreach( KeyValuePair<int, AsBaseEntity> pair in m_dicEntity_InstanceId)
		{
			if( pair.Value != null)
			{
				if( pair.Value.FsmType == eFsmType.PLAYER)
				{
					AsUserEntity player = pair.Value as AsUserEntity;
					Debug.Log( "AsEntityManager::RemoveAllEntities: [" + AsGameMain.s_gameState + "] destroyed player.UniqueId:" + player.UniqueId);
				}
				RequestDestroy( pair.Value);
//				Destroy( pair.Value.gameObject);
			}
		}
		m_dicEntity_InstanceId.Clear();

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.RemoveCashShopModel();

		UnloadAssetsForMemory();
	}

	bool m_RemoveAllEntitiesAsyncFinished = false;public bool RemoveAllEntitiesAsyncFinished{get{return m_RemoveAllEntitiesAsyncFinished;}}
	public void RemoveAllEntitiesAsync()
	{
		StartCoroutine( RemoveAllEntitiesAsync_CR());
	}

	IEnumerator RemoveAllEntitiesAsync_CR()
	{
		Debug.Log( "AsEntityManager::RemoveAllEntitiesAsync: RemoveAllEntitiesAsync_CR() began");

		m_RemoveAllEntitiesAsyncFinished = false;

		m_mdicUserEntity_SessionId.Clear();
		m_dicUserEntity_UniqueId.Clear();

		m_dicNpcEntity_SessionId.Clear();

		foreach( KeyValuePair<int, AsBaseEntity> pair in m_dicEntity_InstanceId)
		{
			if( pair.Value != null)
			{
				if( pair.Value.FsmType == eFsmType.PLAYER)
				{
					AsUserEntity player = pair.Value as AsUserEntity;
					Debug.Log( "AsEntityManager::RemoveAllEntitiesAsync: [" + AsGameMain.s_gameState + "] destroyed player.UniqueId:" + player.UniqueId);
				}
//				RequestDestroy( pair.Value);
				Debug.Log( "AsEntityManager::RemoveAllEntitiesAsync: inserted entity InstanceID:" + pair.Value.GetInstanceID());
//				Destroy( pair.Value.gameObject);
				m_queWaitingEntity.Enqueue( pair.Value);
			}
		}
		m_dicEntity_InstanceId.Clear();

		if( m_DestroyingEntity == false && gameObject.active == true)
			StartCoroutine( DestroyingSequence());

		yield return null;

		UnloadAssetsForMemory();
	}

	int m_UnusedAssetCount = 0;
	public int maxAssetCount_ = 40;
	public bool unloadAssets_ = true;
	void UnloadAssetsForMemory()
	{
		++m_UnusedAssetCount;

//		if( unloadAssets_ == true && m_UnusedAssetCount > maxAssetCount_)
//		{
//			m_UnusedAssetCount = 0;
//			Resources.UnloadUnusedAssets();
//		}
	}

	void InvokeUnloadAssets()
	{
		if( unloadAssets_ == true && m_UnusedAssetCount > maxAssetCount_)
		{
			m_UnusedAssetCount = 0;
			Resources.UnloadUnusedAssets();
		}
	}
	#endregion

	#region - register -
	public void RegisterUserCharacter( AsUserEntity _entity)
	{
		m_mdicUserEntity_SessionId.Add( _entity.SessionId, _entity);

		if( m_dicUserEntity_UniqueId.ContainsKey( _entity.UniqueId) == true)
		{
			if( _entity.CheckShopOpening() == true)
			{
			}
			else
			{
				Debug.LogError( "AsEntityManager::RegisterUserCharacter: entity( unique=" + _entity.UniqueId + ") does not open shop. invalid registering.");
			}
		}
		else
		{
			m_dicUserEntity_UniqueId.Add( _entity.UniqueId, _entity);
		}

		if( m_dicEntity_InstanceId.ContainsKey( _entity.gameObject.GetInstanceID()) == true)
		{
			if( _entity.CheckShopOpening() == true)
			{
			}
			else
			{
				Debug.LogError( "AsEntityManager::RegisterUserCharacter: entity( instance=" + _entity.gameObject.GetInstanceID() + ") does not open shop. invalid registering.");
			}
		}
		else
		{
			m_dicEntity_InstanceId.Add( _entity.gameObject.GetInstanceID(), _entity);
		}
	}
	public void RegisterNpcCharacter( AsNpcEntity _entity)
	{
		m_dicNpcEntity_SessionId.Add( _entity.SessionId, _entity);
		m_dicEntity_InstanceId.Add( _entity.gameObject.GetInstanceID(), _entity);
	}
	#endregion

	#region - message -
	public void DispatchMessageByUniqueKey( uint _id, AsIMessage _msg)//user unique key
	{
		if( m_dicUserEntity_UniqueId.ContainsKey( _id) == true)
		{
			m_dicUserEntity_UniqueId[_id].HandleMessage( _msg);
		}
		else
			Debug.LogWarning( "[AsEntityManager]DispatchMessageByUniqueKey: id:" + _id + " doesn't exist");
	}

	public void DispatchMessageByNpcSessionId( int _id, AsIMessage _msg)// npc session key
	{
		if( m_dicNpcEntity_SessionId.ContainsKey( _id) == true)
		{
			m_dicNpcEntity_SessionId[_id].HandleMessage( _msg);
		}
		else
			Debug.LogWarning( "[AsEntityManager]DispatchMessageByNpcSessionId: id:" + _id + " doesn't exist");
	}

	public void DispatchMessageByInstanceId( int _instanceId, AsIMessage _msg)
	{
		if( m_dicEntity_InstanceId.ContainsKey( _instanceId) == true)
		{
			m_dicEntity_InstanceId[_instanceId].HandleMessage( _msg);
		}
		else
			Debug.LogWarning( "[AsEntityManager]DispatchMessageByInstanceId: id:" + _instanceId + " doesn't exist");
	}

	public void BroadcastMessageToAllEntities( AsIMessage _msg)
	{
//		foreach( KeyValuePair<uint, AsBaseEntity> pair in m_dicEntity_UniqueId)
//		{
//			pair.Value.HandleMessage( _msg);
//		}
		foreach( KeyValuePair<int, AsBaseEntity> pair in m_dicEntity_InstanceId)
		{
			pair.Value.HandleMessage( _msg);
		}
	}

	public void MessageToPlayer( AsIMessage _msg)
	{
		if( m_kUserEntity != null)
			m_kUserEntity.HandleMessage( _msg);
		else
			Debug.LogWarning( "AsEntityManager::MessageToPlayer: player entity not exist. game state =" + AsGameMain.s_gameState);
	}
	#endregion

	#region - network process -
	public void OtherCharAppear( AS_SC_OTHER_CHAR_APPEAR_1 _appear)//game process
	{
		for( int i = 0; i < _appear.nCharCnt; ++i)
		{
			AS_SC_OTHER_CHAR_APPEAR_2 appear = _appear.body[i];

			if( false == m_dicUserEntity_UniqueId.ContainsKey( appear.nCharUniqKey))
			{
				//create
				OtherCharacterAppearData creationData = new OtherCharacterAppearData( appear);
				AsUserEntity entity = AsEntityManager.Instance.CreateUserEntity( "OtherUser", creationData, true, true);
				entity.SetRePosition( entity.transform.position);
				
				_PetProc( entity, creationData);

				Msg_OtherUserMoveIndicate moveInfo = new Msg_OtherUserMoveIndicate( creationData.sessionKey_,
					creationData.uniqKey_, creationData.curPosition_, creationData.destPosition_, eMoveType.Normal);
				entity.HandleMessage( moveInfo);

				if( appear.bProgress)
				{
					entity.ShowProductImg( appear.bProgress);
				}
				else if( 0 != appear.nCollectIdx)
				{
					body_SC_COLLECT_INFO info = new body_SC_COLLECT_INFO();
					info.nCollectNpcIdx = appear.nCollectIdx;
					info.nCollectorUniqKey = appear.nCharUniqKey;
					info.nCollectorSession = appear.nSessionIdx;
					info.eCollectState = ( int)eCOLLECT_STATE.eCOLLECT_STATE_START;
					entity.HandleMessage( new Msg_CollectInfo( info));
				}

				//party
				entity.PartyIdx = appear.nPartyIdx;
				entity.ShowPartyPR( appear.bPartyNotice, System.Text.UTF8Encoding.UTF8.GetString( appear.szPartyNotice));
			}
			else
			{
				AsUserEntity entity = m_dicUserEntity_UniqueId[appear.nCharUniqKey];

				if( entity.CheckShopOpening() == true)
				{
					OtherCharacterAppearData creationData = new OtherCharacterAppearData( appear);
					entity.SetCreationData( creationData);
					entity.SetRePosition( entity.transform.position);
					entity.ShowProductImg( appear.bProgress);

					if( 0 != appear.nCollectIdx)
					{
						body_SC_COLLECT_INFO info = new body_SC_COLLECT_INFO();
						info.nCollectNpcIdx = appear.nCollectIdx;
						info.nCollectorUniqKey = appear.nCharUniqKey;
						info.nCollectorSession = appear.nSessionIdx;
						info.eCollectState = ( int)eCOLLECT_STATE.eCOLLECT_STATE_START;
						entity.HandleMessage( new Msg_CollectInfo( info));
					}
				}
				else
				{
					Debug.LogError( "[AsEntityManager]OtherCharAppear: already created character. id = " + appear.nCharUniqKey);
					continue;
				}
			}
		}
	}
	
	void _PetProc( AsUserEntity _user, OtherCharacterAppearData _data)
	{
		if( _data.nPetTableIdx_ > 0)
		{
			PetAppearData appear = new PetAppearData(_user, _data);
			_user.HandleMessage( new Msg_PetDataIndicate( appear));
//			PetAppear( appear);
		}
	}

	public void OtherCharDisappear( AS_SC_OTHER_CHAR_DISAPPEAR_1 _appear)//game process
	{
		for( int i=0; i<_appear.nCharCnt; ++i)
		{
			AS_SC_OTHER_CHAR_DISAPPEAR_2 appear = _appear.body[i];

			if( m_dicUserEntity_UniqueId.ContainsKey( appear.nCharUniqKey) == true)
			{
				if( m_dicUserEntity_UniqueId[appear.nCharUniqKey].CheckShopOpening() == true)
				{
				}
				else
				{
					AsEntityManager.Instance.RemoveEntity( m_dicUserEntity_UniqueId[appear.nCharUniqKey]);
				}
			}
			else
			{
				Debug.LogError( "[AsEntityManager]OtherCharDisappear: no character, CharUniqKey: " + appear.nCharUniqKey);
				continue;
			}
		}

		UnloadAssetsForMemory();
	}

	public void NpcAppearForEvent(AS_SC_NPC_APPEAR_2 _appear)
	{
		if (true == m_dicNpcEntity_SessionId.ContainsKey(_appear.nNpcIdx))
		{
			Debug.LogError("[AsEntityManager]NpcAppear: already created character. id = " + _appear.nNpcIdx);
			return;
		}

		NpcAppearData creationData = new NpcAppearData(_appear);

		if (creationData.npcType_ == eNPCType.NPC)
			AsEntityManager.Instance.CreateNpcEntity( "Npc", creationData);
	}

	public void NpcAppear( AS_SC_NPC_APPEAR_1 _appear)
	{
		List<AS_SC_NPC_APPEAR_2> listAppear = new List<AS_SC_NPC_APPEAR_2>();
		listAppear.AddRange( _appear.body);
		listAppear.Sort( AsUtil.DistanceCompare);

		foreach( AS_SC_NPC_APPEAR_2 apr in listAppear)
		{
			AS_SC_NPC_APPEAR_2 appear = apr;
			if( true == m_dicNpcEntity_SessionId.ContainsKey( appear.nNpcIdx))
			{
				Debug.LogError( "[AsEntityManager]NpcAppear: already created character. id = " + appear.nNpcIdx);
				continue;
			}

			// check in game event 
			if (AsInGameEventManager.instance.CheckEventViewNpc(apr.nNpcTableIdx))
			{
				if (AsInGameEventManager.instance.CheckRunningEventviewNpc(apr.nNpcTableIdx))
				{
					AsInGameEventManager.instance.AddNpcAppear(apr);
					continue;
				}
			}

			//create
			NpcAppearData creationData = new NpcAppearData( appear);

			switch( creationData.npcType_)
			{
			case eNPCType.Monster:
				AsEntityManager.Instance.CreateNpcEntity( "Monster", creationData);
				break;
			case eNPCType.NPC:
			case eNPCType.Portal:
				AsEntityManager.Instance.CreateNpcEntity( "Npc", creationData);
				break;
			case eNPCType.Object:
				{
					if( 0 == appear.nObjectStatus)
					{
						AsNpcEntity npcEntity = AsEntityManager.Instance.CreateNpcEntity( "Object", creationData);
						npcEntity.SetProperty( eComponentProperty.GROUP_INDEX, appear.nNpcGroupIdx);
						npcEntity.SetProperty( eComponentProperty.LINK_INDEX, appear.nNpcLinkIdx);

						if( ( 0 < appear.nNpcGroupIdx) && ( null != npcEntity))
							ObjSteppingMgr.Instance.InsertObjStepping( appear.nNpcGroupIdx, appear.nNpcLinkIdx, npcEntity);
					}
				}
				break;
			case eNPCType.Collection:
				{
					AsNpcEntity npcEntity = AsEntityManager.Instance.CreateNpcEntity( "Collection", creationData);
					npcEntity.SetProperty( eComponentProperty.COLLECTOR_IDX, appear.nCollector);
				}
				break;
			}
		}
	}

	public void NpcDisappear( AS_SC_NPC_DISAPPEAR_1 _appear)//game process
	{
		for( int i=0; i<_appear.nNpcCnt; ++i)
		{
			AS_SC_NPC_DISAPPEAR_2 appear = _appear.body[i];

			AsInGameEventManager.instance.DelNpcAppear(appear);

			if( m_dicNpcEntity_SessionId.ContainsKey( appear.nNpcIdx) == true)
			{
				AsNpcEntity entity = AsEntityManager.Instance.GetNpcEntityBySessionId( appear.nNpcIdx);
				if( eFsmType.OBJECT == entity.FsmType && eOBJECT_PROP.STEPPING_LEAF == entity.GetProperty<eOBJECT_PROP>( eComponentProperty.OBJ_TYPE))
				{
					ObjSteppingMgr.Instance.RemoveNpcIdx( appear.nNpcIdx);
				}
				AsEntityManager.Instance.RemoveEntity( m_dicNpcEntity_SessionId[appear.nNpcIdx]);
			}
			else
			{
				Debug.LogWarning( "[AsEntityManager]NpcDisappear: there is no [id:" + appear.nNpcIdx + "] character");
				continue;
			}
		}

		UnloadAssetsForMemory();
	}
	
	public AsNpcEntity PetAppear( PetAppearData _appear)
	{
		AsNpcEntity entity = null;
	
		GameObject obj = new GameObject( "Pet");
		entity = obj.AddComponent<AsNpcEntity>();
		entity.AttachComponent( m_dicEntityTemplate["Pet"]);
		entity.InitComponents();
		obj.transform.parent = m_RootObject.transform;

		entity.SetCreationData( _appear);
		
		entity.InterInitComponents();
		entity.LateInitComponents();
		entity.LastInitComponents();
		
		return entity;
	}
	
//	public void PetDisappear()
//	{
//	}
	
	public void OtherCharAttack( AS_SC_CHAR_ATTACK_NPC_1 _attack)
	{
//		Debug.Log( "OtherCharAttack : " + _attack.nCharUniqKey);

		AsUserEntity userEntity = GetUserEntityByUniqueId( _attack.nCharUniqKey);
		if( userEntity != null)// && userEntity.UniqueId != AsUserInfo.Instance.GetCurrentUserEntity().UniqueId)
		{
			userEntity.HandleMessage( new Msg_OtherCharAttackNpc1( _attack));
		}
		else
			Debug.LogWarning( "AsEntityManager::OtherCharAttack: no id character");
	}

//	public void NpcMove( AS_SC_NPC_MOVE _move)
//	{
//
//	}
	#endregion

	#region - getter -
	public List<AsUserEntity> GetUserEntityBySessionId( ushort _id)
	{
		if( m_mdicUserEntity_SessionId.ContainsKey( _id) == true)
			return m_mdicUserEntity_SessionId[_id];
		else
			return null;
	}
	public AsUserEntity GetUserEntityByUniqueId( uint _id)
	{
		if( m_dicUserEntity_UniqueId.ContainsKey( _id) == true)
			return m_dicUserEntity_UniqueId[_id];
		else
			return null;
	}
	public AsNpcEntity GetNpcEntityBySessionId( int _id)
	{
		if( m_dicNpcEntity_SessionId.ContainsKey( _id) == true)
			return m_dicNpcEntity_SessionId[_id];
		else
			return null;
	}
	public AsBaseEntity GetEntityByInstanceId( int _instanceId)
	{
		if( m_dicEntity_InstanceId.ContainsKey( _instanceId) == true)
			return m_dicEntity_InstanceId[_instanceId];
		else
			return null;
	}

	public float GetNearestMonster( Vector3 _pos, out AsNpcEntity _npc)
	{
		_npc = null;
		float nearestDistance = float.MaxValue;

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				pair.Value.isSegregate == false)
			{
				float dist = Vector3.SqrMagnitude( pair.Value.transform.position - _pos);
				if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "Monster") && dist < nearestDistance)
				{
					nearestDistance = dist;
					_npc = pair.Value;
				}
			}
		}

		return Mathf.Sqrt( nearestDistance);
	}

	public float GetNearestMonsterExceptObject( Vector3 _pos, out AsNpcEntity _npc)
	{
		_npc = null;
		float nearestDistance = float.MaxValue;

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				pair.Value.isSegregate == false)
			{
				if( pair.Value.CheckObjectMonster() == true)
					continue;

				float dist = Vector3.SqrMagnitude( pair.Value.transform.position - _pos);
				if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "Monster") && dist < nearestDistance)
				{
					nearestDistance = dist;
					_npc = pair.Value;
				}
			}
		}

		return Mathf.Sqrt( nearestDistance);
	}

	float GetNearestHostileUser( Vector3 _pos, out AsUserEntity _npc)
	{
		_npc = null;
		float nearestDistance = float.MaxValue;

		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				TargetDecider.CheckOtherUserIsEnemy( pair.Value) == true &&
				pair.Value.isSegregate == false)
			{
				float dist = Vector3.SqrMagnitude( pair.Value.transform.position - _pos);
				if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "OtherUser") && dist < nearestDistance)
				{
					nearestDistance = dist;
					_npc = pair.Value;
				}
			}
		}

		return Mathf.Sqrt( nearestDistance);
	}

	public float GetNearestHostileEntity( Vector3 _pos, out AsBaseEntity _entity)
	{
		_entity = null;

		AsNpcEntity mob = null;
		AsUserEntity user = null;
		float distNpc = float.MaxValue;
		float distUser = float.MaxValue;

		distNpc = GetNearestMonsterExceptObject( _pos, out mob);
		distUser = GetNearestHostileUser( _pos, out user);

		if( distNpc < distUser)
		{
			_entity = mob;
			return distNpc;
		}
		else
		{
			_entity = user;
			return distUser;
		}
	}

	public float GetNearestNpcForPStore( Vector3 _pos, out AsNpcEntity _npc)
	{
		_npc = null;
		float nearestDistance = float.MaxValue;

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			Tbl_NormalNpc_Record record = AsTableManager.Instance.GetTbl_NormalNpc_Record( pair.Value.TableIdx);

			if( record.EnablePStore == true)
				continue;

			float dist = Vector3.Distance( pair.Value.transform.position, _pos);
			if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "Npc") && dist < nearestDistance)
			{
				nearestDistance = dist;
				_npc = pair.Value;
			}
		}

		return nearestDistance;
	}

	public float GetNearestPrivateShop( Vector3 _pos, out AsUserEntity _user)
	{
		_user = null;
		float nearestDistance = float.MaxValue;

		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			float dist = Vector3.Distance( pair.Value.transform.position, _pos);
			if( pair.Value.GetProperty<bool>( eComponentProperty.SHOP_OPENING) == true && dist < nearestDistance)
			{
				nearestDistance = dist;
				_user = pair.Value;
			}
		}

		return nearestDistance;
	}

	public List<AsNpcEntity> GetMonsterInRange( Vector3 _pos, float _minDist, float _maxDist)
	{
		List<AsNpcEntity> enemies = new List<AsNpcEntity>();

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true &&
				pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				pair.Value.isSegregate == false)
			{
				float dist = Vector3.Distance( pair.Value.transform.position, _pos);
				if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "Monster") &&
					_minDist <= dist && dist <= _maxDist)
				{
					enemies.Add( pair.Value);
				}
			}
		}

		return enemies;
	}

	public List<AsNpcEntity> GetEntityInRange( UseItemToTargetType _useItemToTargetType, Vector3 _pos, float _minDist, float _maxDist, bool _checkDist = true)
	{
		List<AsNpcEntity> enemies = new List<AsNpcEntity>();

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			string layerName = string.Empty;

			if( _useItemToTargetType == UseItemToTargetType.MONSTER)
			{
				if( pair.Value.ContainProperty( eComponentProperty.LIVING) == false)
					continue;

				if( pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == false)
					continue;
				
				if( pair.Value.isSegregate == true)
					continue;

				layerName = "Monster";
			}
			else if( _useItemToTargetType == UseItemToTargetType.COLLECTION)
			{
				layerName = "Collection";
			}
			else if( _useItemToTargetType == UseItemToTargetType.NPC)
			{
				layerName = "Npc";
			}

			float dist = Vector3.Distance( pair.Value.transform.position, _pos);

			if( pair.Value.gameObject.layer == LayerMask.NameToLayer( layerName))
			{
				if( _checkDist == true)
				{
					if( _minDist <= dist && dist <= _maxDist)
						enemies.Add( pair.Value);
				}
				else
				{
					enemies.Add( pair.Value);
				}
			}
		}

		return enemies;
	}

	public List<AsUserEntity> GetUserEntityInRange( Vector3 _pos, float _minDist, float _maxDist, bool _all = true)
	{
		List<AsUserEntity> entities = new List<AsUserEntity>();

		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			float dist = Vector3.Distance( pair.Value.transform.position, _pos);
			if( pair.Value.gameObject.layer == LayerMask.NameToLayer( "OtherUser") &&
				_minDist <= dist && dist <= _maxDist)
			{
				if( _all == true)
				{
					entities.Add( pair.Value);
				}
				else if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true)
				{
					entities.Add( pair.Value);
				}
			}
		}

		return entities;
	}

	public List<AsUserEntity> GetPartyInRange( Vector3 _pos, float _minDist, float _maxDist)
	{
		List<AsUserEntity> entities = new List<AsUserEntity>();

		foreach( KeyValuePair<uint, AS_PARTY_USER> pair in AsPartyManager.Instance.GetPartyMemberList())
		{
			AsUserEntity member = GetUserEntityByUniqueId( pair.Value.nCharUniqKey);

			if( member == null)
				continue;

			float dist = Vector3.Distance( member.transform.position, _pos);
			if( member.gameObject.layer == LayerMask.NameToLayer( "OtherUser") &&
				_minDist <= dist && dist <= _maxDist)
			{
				entities.Add( member);
			}
		}

		return entities;
	}
	
	public List<AsUserEntity> GetUserEntitybyGuildName( string szGuildName )
	{
		List<AsUserEntity> entities = new List<AsUserEntity>();

		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			string strGuildName = pair.Value.GetProperty<string>( eComponentProperty.GUILD_NAME);
			if( strGuildName.Length > 0 && strGuildName == szGuildName )
			{
				entities.Add( pair.Value);
			}
		}
		return entities;
	}
	
	public AsPlayerFsm GetPlayerCharFsm()
	{
//		if( null == m_RootObject)
//			return null;
//
//		AsPlayerFsm player = m_RootObject.GetComponentInChildren<AsPlayerFsm>();
//		return player;

		return m_PlayerFsm;
	}

	public List<AsNpcEntity> GetEntityListByFsmType( eFsmType _fsmType)
	{
		List<AsNpcEntity> listNpc = new List<AsNpcEntity>();

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.FsmType == _fsmType)
				listNpc.Add( pair.Value);
		}

		return listNpc;
	}


	public AsNpcEntity GetNPCEntityByTableID( int _npcID)
	{
		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.FsmType == eFsmType.NPC)
			{
				if( pair.Value.GetProperty<int>( eComponentProperty.NPC_ID) == _npcID)
					return pair.Value;
			}
		}
	
		return null;
	}

	public AsNpcEntity GetMonsterByTableID( int id)
	{
		foreach( KeyValuePair<int,AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( eFsmType.MONSTER == pair.Value.FsmType)
			{
				if( id == pair.Value.GetProperty<int>( eComponentProperty.NPC_ID))
					return pair.Value;
			}
		}
	
		return null;
	}
	
	public List<AsBaseEntity> GetHostileEntityInRange( Vector3 _pos, float _minDist, float _maxDist)
	{
		float sqrMinDist = _minDist * _minDist;
		float sqrMaxDist = _maxDist * _maxDist;
		
		List<AsBaseEntity> listEntities = new List<AsBaseEntity>();
		
		foreach( KeyValuePair<uint, AsUserEntity> pair in m_dicUserEntity_UniqueId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				pair.Value.gameObject.layer == LayerMask.NameToLayer( "OtherUser") &&
				TargetDecider.CheckOtherUserIsEnemy( pair.Value) == true &&
				pair.Value.isSegregate == false)
			{
				float sqrDist = Vector3.SqrMagnitude( pair.Value.transform.position - _pos);
				if( sqrMinDist < sqrDist && sqrDist < sqrMaxDist)
				{
					listEntities.Add( pair.Value);
				}
			}
		}

		foreach( KeyValuePair<int, AsNpcEntity> pair in m_dicNpcEntity_SessionId)
		{
			if( pair.Value.ContainProperty( eComponentProperty.LIVING) == true && pair.Value.GetProperty<bool>( eComponentProperty.LIVING) == true &&
				pair.Value.gameObject.layer == LayerMask.NameToLayer( "Monster")  &&
				pair.Value.isSegregate == false)
			{
				float sqrDist = Vector3.SqrMagnitude( pair.Value.transform.position - _pos);
				if( sqrMinDist < sqrDist && sqrDist < sqrMaxDist)
				{
					listEntities.Add( pair.Value);
				}
			}
		}

		return listEntities;
	}
	#endregion

	#region - Model Loading -
	bool m_ModelLoading = false;public bool ModelLoading{get{return m_ModelLoading;}}
	Queue<AsModel> m_queLoadingModel = new Queue<AsModel>();
	public void RequestModelLoading( AsModel _model)
	{
//		Debug.Log( "AsEntityManager::RequestModelLoading: _model.Entity.GetInstanceID(). entity = " + _model.name + ", _model.Entity.GetInstanceID() = " + _model.Entity.GetInstanceID());
		m_queLoadingModel.Enqueue( _model);

		if( m_ModelLoading == false)
			StartCoroutine( "LoadingSequence");
	}

	IEnumerator LoadingSequence()
	{
		m_ModelLoading = true;
		int idx = 0;

		while( true)
		{
			if( m_queLoadingModel.Count == 0)
			{
				m_ModelLoading = false;
				break;
			}

			AsModel model = m_queLoadingModel.Dequeue();
			if( null == model)
			{
//				Debug.LogWarning( "AsEntityManager::LoadingSequence: model is null. process is passed.");

//				Debug.LogWarning( "AsEntityManager::LoadingSequence: model[" + model.name + "] is null. process is passed. entity = " + model.Entity +
//					", model.Entity.GetInstanceID() = " + model.Entity.GetInstanceID());
				continue;
			}

			if( model.Entity.Destroyed == true)
			{
//				Debug.LogWarning( "AsEntityManager::LoadingSequence: already destroyed entity[" + model.Entity.GetInstanceID() + "]. continue.");
				continue;
			}

//			Debug.Log( "AsEntityManager::LoadingSequence: loading began. model.Entity = " + model.Entity + ", model.Entity.GetInstanceID() = " + model.Entity.GetInstanceID());
//			Debug.Log( "AsEntityManager::LoadingSequence: loading began. model.Entity.CheckModelLoadingState() = " + model.Entity.CheckModelLoadingState() + ", model.Entity.CheckPartsLoaded() = " + model.Entity.CheckPartsLoaded());
			//yield return StartCoroutine( model.LoadingModel( model.Entity, idx));
			bool isCashShopModel = false;
			if( model.Entity.EntityType == eEntityType.USER )
			{
				AsUserEntity _userEntity = model.Entity as AsUserEntity;
				if( null != _userEntity )
				{
					isCashShopModel = _userEntity.UniqueId == uint.MaxValue;
				}
			}
			
			if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME
				&& false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_ResourceShow)
				&& ( eFsmType.PLAYER != model.Entity.FsmType && false == isCashShopModel )
				&& null != model.DummObj)
			{
				yield return StartCoroutine( model.LoadingModel_Dummy( model.Entity, idx));
			}
			else
			{
				yield return StartCoroutine( model.LoadingModel( model.Entity, idx));
			}
			
//			if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
//				yield return null;
//			else
//				yield return StartCoroutine( model.LoadingModel( model.Entity, idx));
		}
	}
	#endregion

	#region - destroying entity -
	bool m_DestroyingEntity = false;public bool DestroyingEntity{get{return m_DestroyingEntity;}}
	Queue<AsBaseEntity> m_queWaitingEntity = new Queue<AsBaseEntity>();
	void RequestDestroy( AsBaseEntity _entity)
	{
		if( ( eModelLoadingState.Started == _entity.CheckModelLoadingState()) || ( false == _entity.CheckPartsLoaded()))
		{

//			Debug.Log( "AsEntityManager::RequestDestroy: insert destruction. entity = " + _entity + ", _entity.GetInstanceID() = " + _entity.GetInstanceID());
//			Debug.Log( "AsEntityManager::RequestDestroy: _entity.CheckModelLoadingState() = " + _entity.CheckModelLoadingState() + ", _entity.CheckPartsLoaded() = " + _entity.CheckPartsLoaded());
			m_queWaitingEntity.Enqueue( _entity);
		}
		else
		{
//			Debug.Log( "AsEntityManager::RequestDestroy: inserted entity's model is already loaded. Destroy( _entity.GetInstanceID()) = " + _entity.GetInstanceID());
			_entity.DestroyThis();
#if AUTOMOVE_EFFECT
			if( _entity.isNeedAutoMoveEffect)
				AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_AutoMove", _entity.transform.position, false, 0f, 1.0f);
			//if( _entity.isNeedAutoMoveEffect && null != AsSoundManager.Instance)
			//	AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_AutoMove", _entity.transform.position, false);
#endif
			Destroy( _entity.gameObject);
			return;
		}

		if( m_DestroyingEntity == false && gameObject.active == true)
			StartCoroutine( DestroyingSequence());
	}

	IEnumerator DestroyingSequence()
	{
//		Debug.Log( "AsEntityManager::DestroyingSequence: DestroyingSequence is began. m_queWaitingEntity.Count = " + m_queWaitingEntity.Count);

		m_DestroyingEntity = true;

		while( true)
		{
//			Debug.Log( "AsEntityManager::DestroyingSequence: determine m_queWaitingEntity's count = " + m_queWaitingEntity.Count);
			if( m_queWaitingEntity.Count == 0)
			{
				m_DestroyingEntity = false;
				m_RemoveAllEntitiesAsyncFinished = true;

//				Debug.Log( "AsEntityManager::DestroyingSequence: count is 0. m_DestroyingEntity = false. then break");
				break;
			}

//			Debug.Log( "AsEntityManager::DestroyingSequence: m_queWaitingEntity.Dequeue()");
			AsBaseEntity entity = m_queWaitingEntity.Dequeue();

			while( true)
			{
				if( entity == null)
				{
//					Debug.Log( "AsEntityManager::DestroyingSequence: entity is null. remove this and break.");
					break;
				}
				else
				{
//					Debug.Log( "AsEntityManager::DestroyingSequence: entity == " + entity + "[" + entity.GetInstanceID() + "], entity.CheckModelLoadingState() == " + entity.CheckModelLoadingState() + ", entity.CheckPartsLoaded() == " + entity.CheckPartsLoaded());

					if( entity.CheckModelLoadingState() != eModelLoadingState.Started && entity.CheckPartsLoaded() == true)
					{
//						Debug.Log( "AsEntityManager::DestroyingSequence: Destroy( entity.gameObject)");
#if AUTOMOVE_EFFECT
						if( entity.isNeedAutoMoveEffect)
							AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_AutoMove", entity.transform.position, false, 0f, 1.0f);
						//if( entity.isNeedAutoMoveEffect && null != AsSoundManager.Instance)
						//	AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_AutoMove", entity.transform.position, false);
#endif
//						DestroyImmediate( entity.gameObject);
						Destroy( entity.gameObject);
//						Debug.Log( "AsEntityManager::DestroyingSequence: entity is removed, then break");
						break;
					}
					else
					{
//						Debug.Log( "AsEntityManager::DestroyingSequence: waiting for model loading completion.");
					}
				}

				yield return null;
			}
		}

//		Debug.Log( "AsEntityManager::DestroyingSequence: ends");
#if UNITY_IPHONE
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
#endif
	}
	#endregion
}
