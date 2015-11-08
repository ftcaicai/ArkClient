
#define _USER_SHADOW_
#define _NPC_SHADOW_ 
#define _USE_TEXTURE_MANAGER
#define AUTOMOVE_EFFECT
#define _USE_DUMMY_OBJECT
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum eModelType	{ Normal, Shop }
public enum eModelLoadingState	{ Waiting, Started, Finished, NONE }

public class AsModel : AsBaseComponent
{
	#region - member -
	Dictionary<string, Transform> m_dicDummy = new Dictionary<string, Transform>();
	
	private string m_strShadow01Path = "FX/Effect_Resource/Decal/common_shadow03";
	private GameObject m_ShadowObject = null;
	private int m_Shadow01Eff_Id;
	private GameObject m_DummyObj = null;
	public GameObject DummObj { get{ return m_DummyObj;}}

	PartsRoot m_PartsRoot = null;	
	public PartsRoot partsRoot	{ get { return m_PartsRoot; } }
	
	eModelType m_ModelType = eModelType.Normal;
	eModelLoadingState m_ModelLoadingState = eModelLoadingState.Waiting;
	
	bool m_PartsLoaded = true;
	public bool isKeepDummyObj
	{
		get
		{
			if( null == m_Entity)
				return false;
			else
				return m_Entity.isKeepDummyObj;
		}
	}
	
	readonly float m_HideAlpha = 0.2f;
	#endregion
	
	#region - init & destroy -
	void Awake()
	{
		m_ComponentType = eComponentType.MODEL;
		
		MsgRegistry.RegisterFunction( eMessageType.MODEL_CHANGE, OnModelChange);
		MsgRegistry.RegisterFunction( eMessageType.HIDE_INDICATE, OnHideIndicate);
		MsgRegistry.RegisterFunction( eMessageType.SHADOW_CONTROL, OnShadowControl);
		MsgRegistry.RegisterFunction( eMessageType.TRANSFORM, OnTransform);
	}
	
	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);
		
		_entity.SetModelTypeDelegate( CheckModelTypeDelegate);
		_entity.SetModelLoadingStateDelegate( CheckModelLoadingStateDelegate);
		_entity.SetPartsLoadedDelegate( CheckPartsLoadedDelegate);
		
//#if _USE_DUMMY_OBJECT
//		CreateDummyObj( _entity.FsmType);
//#endif
	}

	public override void InterInit( AsBaseEntity _entity)
	{
		if( _entity.FsmType == eFsmType.PLAYER)
			Debug.Log( "AsModel:: InterInit: shop opening = " + _entity.GetProperty<bool>( eComponentProperty.SHOP_OPENING));
		
#if _USE_DUMMY_OBJECT
		CreateDummyObj( _entity.FsmType);
#endif

		AsEntityManager.Instance.RequestModelLoading( this);
	}
	
	public IEnumerator LoadingModel( AsBaseEntity _entity, int _idx)
	{
		m_ModelLoadingState = eModelLoadingState.Started;
		_entity.isKeepDummyObj = false;
		
		if( null != m_Entity.ModelObject)
		{
			Destroy( m_Entity.ModelObject);
			
			if( null != m_PartsRoot)
				m_PartsRoot.Clear();
		}
		
		if( Entity.ContainProperty( eComponentProperty.SHOP_OPENING) == true)
		{
			if( Entity.AnimEnableViaShop == false)
				m_ModelType = eModelType.Shop;
			else
				m_ModelType = eModelType.Normal;
		}
		
		if( eEntityType.USER == _entity.EntityType)
		{
			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			{
				Debug.Log( "InitialUserCharacter( 1): " + AssetbundleManager.Instance);
				yield return StartCoroutine( InitialUserCharacter_Assetbundle());
			}
			else
			{
				Debug.Log( "InitialUserCharacter( 2): " + AssetbundleManager.Instance);
				
				InitialUserCharacter();
			}
			
#if AUTOMOVE_EFFECT
			if( Entity.isNeedAutoMoveEffect)
				AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_AutoMove", Entity.transform, false, 0f, 1.0f);
			if( Entity.isNeedAutoMoveEffect && null != AsSoundManager.Instance)
				AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_AutoMove", Entity.transform.position, false);
#endif
		}
		else if( eEntityType.NPC == _entity.EntityType)
		{
			m_dicDummy.Clear();
			
			string modelingPath = null;
			string[] strSplit = null;
			bool dataFound = true;
			
			if( Entity.ContainProperty( eComponentProperty.NPC_ID) == true)
			{
				int id = Entity.GetProperty<int>( eComponentProperty.NPC_ID);
				Tbl_Npc_Record npcrecord = AsTableManager.Instance.GetTbl_Npc_Record( id);
				
				modelingPath = npcrecord.ModelingPath;
				strSplit = npcrecord.ModelingPath.Split( '/');
			}
			else if( Entity.ContainProperty( eComponentProperty.PET_ID) == true)
			{
				int id = Entity.GetProperty<int>( eComponentProperty.PET_ID);
				Tbl_Pet_Record petRecord = AsTableManager.Instance.GetPetRecord( id);

				modelingPath = petRecord.Model;
				strSplit = petRecord.Model.Split( '/');
			}
			else
			{
				Debug.LogError( "AsModel::InitialNpcCharacter: no data table record. CHECK NPC ID");
				dataFound = false;
			}
			
			if( dataFound == true)
			{	
				if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
				{
					string strNameLower = strSplit[strSplit.Length-1].ToLower();
					string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strNameLower);
					int nVersion = AssetbundleManager.Instance.GetCachedVersion( strNameLower);
					
					WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
					yield return www;
					
					AssetBundle bundle = www.assetBundle;
					AssetBundleRequest request = bundle.LoadAsync( strNameLower, typeof( GameObject));
					yield return request;
					
					GameObject obj = request.asset as GameObject;
					if( null == obj)
					{
						Debug.LogError( "AsModel::InitialNpcCharacter: There is no [" + modelingPath + "] modeling resource.");
						obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
					}
					
					GameObject model = Instantiate( obj) as GameObject;
					
					bundle.Unload( false);
	
					yield return StartCoroutine( CreateMaterial( model, strNameLower));
					
					Entity.SetModelObject( model);
					InitMonsterDummy( Entity.ModelObject.transform);
					Entity.ModelObject.name = "Model";
					Entity.ModelObject.transform.parent = transform;
//					Entity.ModelObject.transform.localPosition = Vector3.zero;
					Entity.ModelObject.transform.localRotation = Quaternion.identity;
				}
				else
				{
					GameObject obj = ResourceLoad.LoadGameObject( modelingPath);
					if( null == obj)
					{
						Debug.LogError( "AsModel::InitialNpcCharacter: There is no [" + modelingPath + "] modeling resource.");
						obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
					}

					GameObject model = Instantiate( obj) as GameObject;

					Entity.SetModelObject( model);
					InitMonsterDummy( Entity.ModelObject.transform);
					Entity.ModelObject.name = "Model";
					Entity.ModelObject.transform.parent = transform;
//					Entity.ModelObject.transform.localPosition = Vector3.zero;
					Entity.ModelObject.transform.localRotation = Quaternion.identity;
				}
			}
			else
			{
				Debug.LogError( "AsModel::InitialNpcCharacter: no data table record. CHECK NPC ID");
			}
		}
		
		_entity.SetDummyTransform( m_dicDummy);
 
		Entity.ModelObject.transform.localPosition = Vector3.zero;
		m_Entity.ModelObject.transform.localScale = Vector3.one;
		
		if( _entity.isSegregate == false)
			_CreateNamePanel( _entity);
		
		if( eFsmType.COLLECTION != _entity.FsmType )
			MakeShadow();
		
		_CreatePvpPointEffect( _entity);
		
		m_ModelLoadingState = eModelLoadingState.Finished;
		
		InitHide();
		
 		m_Entity.HandleMessage( new Msg_ModelLoaded());
		if( m_Entity.FsmType == eFsmType.PLAYER)
			Debug.Log( "AsModel::OnModelChange: Msg_ModelLoaded is send. [" + m_Entity.FsmType + "] model type = " + m_ModelType);

		DeleteDummyObj();
	}
	
	public IEnumerator LoadingModel_Dummy( AsBaseEntity _entity, int _idx)
	{
		m_ModelLoadingState = eModelLoadingState.Started;
		_entity.isKeepDummyObj = true;
		
		yield return null;
		
		if( Entity.ContainProperty( eComponentProperty.SHOP_OPENING) == true)
		{
			if( Entity.AnimEnableViaShop == false)
				m_ModelType = eModelType.Shop;
			else
			{
				m_ModelType = eModelType.Normal;
				if( null != m_Entity.ModelObject)
				{
					Destroy( m_Entity.ModelObject);
					
					if( null != m_PartsRoot)
						m_PartsRoot.Clear();
				}
			}
		}
		else
		{
			if( null != m_Entity.ModelObject)
			{
				Destroy( m_Entity.ModelObject);
				
				if( null != m_PartsRoot)
					m_PartsRoot.Clear();
			}
		}
		
		if( eEntityType.USER == _entity.EntityType)
		{
#if AUTOMOVE_EFFECT
			if( Entity.isNeedAutoMoveEffect)
				AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_AutoMove", Entity.transform, false, 0f, 1.0f);
			if( Entity.isNeedAutoMoveEffect && null != AsSoundManager.Instance)
				AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_AutoMove", Entity.transform.position, false);
#endif
		}
		
//		_entity.SetDummyTransform( m_dicDummy);

		m_Entity.SetModelObject( m_DummyObj);
		m_Entity.ModelObject.name = "Model";
		
		if( _entity.isSegregate == false)
			_CreateNamePanel( _entity);
		
		if( eFsmType.COLLECTION != _entity.FsmType )
			MakeShadow();
		
		_CreatePvpPointEffect( _entity);
		
		m_ModelLoadingState = eModelLoadingState.Finished;
		
		InitHide();
		
		m_Entity.HandleMessage( new Msg_ModelLoaded_Dummy());
		
		if( m_Entity.FsmType == eFsmType.PLAYER)
			Debug.Log( "AsModel::OnModelChange: Msg_ModelLoaded is send. [" + m_Entity.FsmType + "] model type = " + m_ModelType);

//		DeleteDummyObj();
	}
	
	void DispatchModelLoaded()
	{
		m_Entity.HandleMessage( new Msg_ModelLoaded());
		if( m_Entity.FsmType == eFsmType.PLAYER)
			Debug.Log( "AsModel::DispatchModelLoaded: Msg_ModelLoaded is send. [" + m_Entity.FsmType + "] model type = " + m_ModelType);
	}
	
	IEnumerator CreateMaterial( GameObject model, string name)
	{
		AssetbundleHelper helper = model.GetComponent<AssetbundleHelper>();
		if( null == helper)
		{
			Debug.Log( "Not Found Assetbundle Helper: " + name);
			yield break;
		}
		
		Renderer[] rens = model.GetComponentsInChildren<Renderer>();
		foreach( Renderer ren in rens)
		{
			int nIndex = helper.m_listMeshName.IndexOf( ren.name.ToLower());
			if( 0 > nIndex)
			{
				Debug.Log( "InitialNpcCharacter() NoMatchRenderMeshName: " + model.name + ", " + ren.name);
				continue;
			}
			
			if( nIndex > helper.m_listShaderName.Count - 1)
			{
				Debug.Log( "InitialNpcCharacter() index: " + nIndex + ", shader count: " + helper.m_listShaderName.Count);
				continue;
			}
			
			if( nIndex > helper.m_listTextureName.Count - 1)
			{
				Debug.Log( "InitialNpcCharacter() index: " + nIndex + ", texture count: " + helper.m_listTextureName.Count);
				continue;
			}
			
			string strShaderName = helper.m_listShaderName[nIndex];
			string sgtrNameLower = helper.m_listTextureName[nIndex].ToLower();
			string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( sgtrNameLower);
			int nVersion = AssetbundleManager.Instance.GetCachedVersion( sgtrNameLower);
			
			Debug.Log( "Texture path : " + strAssetbundlePath);
			
#if _USE_TEXTURE_MANAGER
			Texture tempTex = AsCharacterTextureManager.Instance.Get( sgtrNameLower);
			if( null == tempTex)
			{
				WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
				yield return www;
				
				AssetBundle bundle = www.assetBundle;
				tempTex = bundle.Load( sgtrNameLower) as Texture;
				
				Debug.Log( "Texture loaded: " + sgtrNameLower + ", " + tempTex);
				
				bundle.Unload( false);
			}
			
			ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tempTex);
#else
			WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
			yield return www;
			
			AssetBundle bundle = www.assetBundle;
			Texture tex = bundle.Load( sgtrNameLower) as Texture;
			
			Debug.Log( "Texture loaded: " + sgtrNameLower + ", " + tex);
			
			bundle.Unload( false);
			
			ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tex);
#endif
		}
	}

	public override void LateInit( AsBaseEntity _entity)
	{
//		m_Entity.HandleMessage( new Msg_ModelLoaded());
	}
	
	public override void LastInit( AsBaseEntity _entity)
	{
	}
	
	#region - Shadow -
	void OnDestroy()
	{
		ShadowRemove();
		DeleteDummyObj();
		
		if( m_ModelLoadingState == eModelLoadingState.Started)
		{
			Debug.LogError( "AsModel::OnDestroy: m_ModelLoadingState started. modeling was not loaded before OnDestroy [" + name + "]( instanceId:" + gameObject.GetInstanceID() + ")");
		}
	}
	 
	public void ShadowRemove()
	{
		DestroyImmediate( m_ShadowObject);
		//AsEffectManager.Instance.RemoveEffectEntity( m_Shadow01Eff_Id);
	}
	#endregion
	#endregion

	#region - update & message -
	void Update()
	{
	}
	
	void OnModelChange( AsIMessage _msg)
	{
		Debug.Log( "AsModel::OnModelChange: FsmType = " + m_Entity.FsmType);
		AsEntityManager.Instance.RequestModelLoading( this);
	}
	
	void OnHideIndicate(AsIMessage _msg)
	{
		AsUserEntity user = m_Entity as AsUserEntity;
		if(user == null)
			return;
		
		Msg_HideIndicate hide = _msg as Msg_HideIndicate;
		user.SetHide(hide.hide_);
		
		float alpha = 0;
		if(user.Hide == true)
		{
			if(m_Entity.FsmType == eFsmType.PLAYER)
				alpha = m_HideAlpha;
			
			m_Entity.gameObject.layer = LayerMask.NameToLayer("Hide");
			
			m_ShadowObject.SetActiveRecursively(false);
			m_Entity.namePanel.gameObject.SetActiveRecursively(false);
		}
		else
		{
			alpha = 1f;
			
			switch(m_Entity.FsmType)
			{
			case eFsmType.PLAYER:
				m_Entity.gameObject.layer = LayerMask.NameToLayer("Player");
				break;
			case eFsmType.OTHER_USER:
				m_Entity.gameObject.layer = LayerMask.NameToLayer("OtherUser");
				break;
			}
			
			m_ShadowObject.SetActiveRecursively(true);
			m_Entity.namePanel.gameObject.SetActiveRecursively(true);
		}
		
		StartCoroutine(SetAlpha_CR(alpha));
	}
	
	IEnumerator SetAlpha_CR(float _alpha)
	{
		while(true)
		{
			yield return null;
			
			if(m_ModelLoadingState == eModelLoadingState.Finished)
			{
				SetAlpha(_alpha);
				break;
			}
		}
	}
	
	void OnShadowControl( AsIMessage _msg)
	{
		if( m_ShadowObject != null)
		{
			Msg_ShadowControl control = _msg as Msg_ShadowControl;
			m_ShadowObject.SetActive( control.active_);
		}
	}
	
	void OnTransform( AsIMessage _msg)
	{
	}
	#endregion
	
	#region - process -
	void MakeShadow( float _scale=1.0f)
	{
		if( ( true == m_Entity.NeedShadow) && ( true == m_dicDummy.ContainsKey( "DummyLead")))
		{
			m_ShadowObject = Instantiate( ResourceLoad.LoadGameObject( m_strShadow01Path)) as GameObject;
			Transform trn = m_dicDummy[ "DummyLeadBottom"];
			m_ShadowObject.transform.parent = trn;
			m_ShadowObject.transform.localPosition = Vector3.zero;
			m_ShadowObject.transform.localRotation = Quaternion.identity;
			
			float fScale = AsUtil.CharacterDecalSize( Entity.ModelObject);
			Vector3 vecSize = m_ShadowObject.transform.localScale;
			vecSize.x *= ( fScale * _scale * m_Entity.shadowScale.x);
			vecSize.z *= ( fScale * _scale * m_Entity.shadowScale.y);
			m_ShadowObject.transform.localScale = vecSize;
		}
	}
	
	void CreateDummyObj( eFsmType type)
	{
//		Debug.Log( "FsmType : " + type);
		
		switch( type)
		{
		case eFsmType.COLLECTION:
			m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_01")) as GameObject;
			break;
		case eFsmType.MONSTER:
			AsNpcEntity npc = m_Entity as AsNpcEntity;
			Tbl_Monster_Record record = AsTableManager.Instance.GetTbl_Monster_Record( npc.TableIdx);
			if( null != record)
			{
				if( eMonster_Grade.DObject == record.Grade || eMonster_Grade.Treasure == record.Grade)
				{
					m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_02")) as GameObject;
				}
				else
				{
					if( eMonster_AttackType.Offensive == record.AttackType)
						m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_05")) as GameObject;
					else
						m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_04")) as GameObject;
				}
			}
			else
			{
				m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_04")) as GameObject;
			}
			break;
		case eFsmType.NPC:
			m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_03")) as GameObject;
			break;
		case eFsmType.OBJECT:
			m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_02")) as GameObject;
			break;
		case eFsmType.OTHER_USER:
			m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_03")) as GameObject;
			break;
		case eFsmType.PLAYER:
			return;
		default:
			m_DummyObj = Instantiate( Resources.Load( "UseScript/LoadingDummy/B_Common_LoadD_01_03")) as GameObject;
			break;
		}

		if( null == m_DummyObj)
		{
			Debug.Log( "CreateDummyObj(): failed");
			return;
		}
		
		m_DummyObj.transform.parent = Entity.transform;
		m_DummyObj.transform.localPosition = Vector3.zero;
		m_DummyObj.transform.localRotation = Quaternion.Euler( -90.0f, 0.0f, 0.0f);
		//m_DummyObj.transform.localScale = Vector3.one;
		
		float fDummyObjLocalScale = 2.0f;
		
		if( eEntityType.NPC == m_Entity.EntityType && m_Entity.FsmType != eFsmType.PET)
		{
			AsNpcEntity npc = m_Entity as AsNpcEntity;
			Tbl_Npc_Record record = AsTableManager.Instance.GetTbl_Npc_Record( npc.TableIdx);
			if( null != record && record.OrgSize > 0.0f)
				fDummyObjLocalScale = record.OrgSize;
		}
		
		m_DummyObj.transform.localScale = new Vector3( fDummyObjLocalScale, fDummyObjLocalScale, fDummyObjLocalScale);
	}
	
	public void DeleteDummyObj()
	{
		if( null != m_DummyObj)
		{
			Destroy( m_DummyObj);
			m_DummyObj = null;
		}
	}
	
	#region - parts -
	float startedTime = 0;
	void InitialParts( AsUserEntity entity)
	{
		startedTime = Time.realtimeSinceStartup;
		Debug.Log( "AsModel::InitialParts: begin time checking");
		
		if( null == entity)
		{
			Debug.LogError( "AsModel::InitalParts() [ null == entity ]");
			return;
		}
		
		if( null == entity.getCharView || null == entity.getCosItemView)
		{
			Debug.LogError( "AsModel::InitalParts() [ null == entity.getCharView || null == entity.getCosItemView ]");
			return;
		}
		
		if( AsGameDefine.ITEM_SLOT_VIEW_COUNT != entity.getCharView.Length || AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT != entity.getCosItemView.Length) 
		{
			Debug.LogError( "AsModel::InitialParts() [ !( AsGameDefine.ITEM_SLOT_VIEW_COUNT == _partsData.Length) ] length : " + entity.getCharView.Length);
			return;
		}

		eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		string strBasePath = GetBaseBonPath();
		
		if( true == m_PartsRoot.Create( strBasePath, entity.GetProperty<eGENDER>( eComponentProperty.GENDER), __class, entity.UniqueId))
		{
			Entity.SetModelObject( m_PartsRoot.getBone);
			Debug.Log( "AsModel::InitialParts: begin init character, weapon dummy"); startedTime = Time.realtimeSinceStartup;// time check
			InitCharacterDummy( Entity.ModelObject.transform);
			InitWeaponDummy( Entity.ModelObject.transform);
			Debug.Log( "AsModel::InitialParts: end init character dummy. time = " + ( Time.realtimeSinceStartup - startedTime));// time check
			Entity.ModelObject.name = "Model";
			Entity.ModelObject.transform.parent = transform;
			Entity.ModelObject.transform.localPosition = Vector3.zero;
			Entity.ModelObject.transform.localRotation = Quaternion.identity;

			sITEMVIEW[] partsData = entity.getCharView;	
			sITEMVIEW[] cosPartsData = entity.getCosItemView;				
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Weapon, partsData[0], cosPartsData[0]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Head, partsData[1], cosPartsData[1]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Armor, partsData[2], cosPartsData[2]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Gloves, partsData[3], cosPartsData[3]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Point, partsData[4], cosPartsData[4]);
			
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Fairy, null, cosPartsData[Inventory.fairyEquipSlotIdx]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Wing, null, cosPartsData[Inventory.wingEquipSlotIdx]);
			SetPartsUseItemId( Item.eEQUIP.Hair, entity.getHairItemIndex);
			
			Debug.Log( "AsModel::InitialParts: begin GenerateParts dummy"); startedTime = Time.realtimeSinceStartup;// time check
			m_PartsRoot.GenerateParts();
			Debug.Log( "AsModel::InitialParts: end GenerateParts. time = " + ( Time.realtimeSinceStartup - startedTime));// time check
		}
	}
	
	
	// SetCostumeOnOff_Coroutine
	public IEnumerator SetCostumeOnOff_Coroutine()
	{
		m_PartsLoaded = false;

		while( eModelLoadingState.Finished != m_ModelLoadingState)
		{
			yield return null;
		}
		
		AsUserEntity entity = Entity as AsUserEntity;
		if( ( null != entity) && ( null != m_PartsRoot))
		{
			
			sITEMVIEW[] partsData = entity.getCharView;	
			sITEMVIEW[] cosPartsData = entity.getCosItemView;
			
			m_PartsRoot.SetStrengthenCount( partsData[0].nStrengthenCount);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Weapon, partsData[0], cosPartsData[0]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Head, partsData[1], cosPartsData[1]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Armor, partsData[2], cosPartsData[2]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Gloves, partsData[3], cosPartsData[3]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Point, partsData[4], cosPartsData[4]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Fairy, null, cosPartsData[Inventory.fairyEquipSlotIdx]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Wing, null, cosPartsData[Inventory.wingEquipSlotIdx]);
			m_PartsRoot.SetPartsUseItemId( Item.eEQUIP.Hair, entity.getHairItemIndex );	
			
			yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
		}
		else
		{
			Debug.LogError( "AsModel::SetCostumeOnOff_Coroutine()[ null != entity && null != m_PartsRoot ]");
		}
		
		m_PartsLoaded = true;
	}

	
	// SetEqupParts_Coroutine
	public IEnumerator SetEqupParts_Coroutine( int iSlot, sITEMVIEW _view)
	{
		m_PartsLoaded = false;

		while( eModelLoadingState.Finished != m_ModelLoadingState)
		{
			yield return null;
		}
		
		AsUserEntity entity = Entity as AsUserEntity;
		if( ( null != entity) && ( null != m_PartsRoot))
		{
			sITEMVIEW _cosItem = entity.getCosItemView[iSlot];			
			Item.eEQUIP curEquip = GetPartsType( iSlot);	
			
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			
			if( ( null != _cosItem) && ( true == PartsRoot.IsCostumeOn( curEquip, entity.isCostumeOnOff) ) )
			{
				if( 0 == _cosItem.nItemTableIdx || int.MaxValue == _cosItem.nItemTableIdx)
				{
					m_PartsRoot.SetParts( curEquip, _view);
					yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
				}
				else if( Item.eEQUIP.Weapon == curEquip)
				{
					m_PartsRoot.SetParts( curEquip, _cosItem);
					yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
				}
			}
			else
			{
				m_PartsRoot.SetParts( curEquip, _view);
				yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
			}
		}
		else
		{
			Debug.LogError( "AsModel::SetEqupParts_Coroutine()[ null != entity && null != m_PartsRoot ]");
		}
		
		m_PartsLoaded = true;
	}
	
	public IEnumerator SetCosEqupParts_Coroutine( int iSlot, sITEMVIEW _view)
	{
		m_PartsLoaded = false;

		while( eModelLoadingState.Finished != m_ModelLoadingState)
		{
			yield return null;
		}
		
		AsUserEntity entity = Entity as AsUserEntity;
		if( null != entity && null != m_PartsRoot)
		{		
			Item.eEQUIP curEquip = GetPartsType( iSlot);
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			if( Item.eEQUIP.Fairy == curEquip || Item.eEQUIP.Wing == curEquip)
			{
				m_PartsRoot.SetParts( curEquip, _view);
				yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
				
			}
			else if( true == PartsRoot.IsCostumeOn( curEquip, entity.isCostumeOnOff) )
			{
				sITEMVIEW _norItem = entity.getCharView[iSlot];
				bool bExistEquip = !( 0 == _norItem.nItemTableIdx || int.MaxValue == _norItem.nItemTableIdx);
				bool bCurCosExist = !( 0 == _view.nItemTableIdx || int.MaxValue == _view.nItemTableIdx);
				
				if( false == bCurCosExist && true == bExistEquip)
				{
					m_PartsRoot.SetParts( curEquip, _norItem);
					yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
				}
				else
				{
					m_PartsRoot.SetParts( curEquip, _view);
					yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
				}
			}
		}
		else
		{
			Debug.LogError( "AsModel::SetCosEqupParts_Coroutine()[ null != entity && null != m_PartsRoot ]");
		}
		
		m_PartsLoaded = true;
	}
	
	
	// SetCostumeOnOff
	public void SetCostumeOnOff()
	{
		AsUserEntity entity = Entity as AsUserEntity;
		if( ( null != entity) && ( null != m_PartsRoot))
		{			
			sITEMVIEW[] partsData = entity.getCharView;
			sITEMVIEW[] cosPartsData = entity.getCosItemView;
			
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Weapon, partsData[0], cosPartsData[0]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Head, partsData[1], cosPartsData[1]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Armor, partsData[2], cosPartsData[2]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Gloves, partsData[3], cosPartsData[3]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Point, partsData[4], cosPartsData[4]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Fairy, null, cosPartsData[Inventory.fairyEquipSlotIdx]);
			PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Wing, null, cosPartsData[Inventory.wingEquipSlotIdx]);
			m_PartsRoot.SetPartsUseItemId( Item.eEQUIP.Hair, entity.getHairItemIndex );
			
			m_PartsRoot.GenerateParts();
		}
		else
		{
			Debug.LogError( "AsModel::SetCostumeOnOff()[ null != entity && null == m_PartsRoot ]");
		}
	}
	
	public void SetEqupParts( int iSlot, sITEMVIEW _view)
	{
		AsUserEntity entity = Entity as AsUserEntity;
		if( ( null != entity) && ( null != m_PartsRoot))
		{
			sITEMVIEW _cosItem = entity.getCosItemView[iSlot];			
			Item.eEQUIP curEquip = GetPartsType( iSlot);
			
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			if( ( true == PartsRoot.IsCostumeOn( curEquip, entity.isCostumeOnOff) ) && ( null != _cosItem))
			{
				if( 0 == _cosItem.nItemTableIdx || int.MaxValue == _cosItem.nItemTableIdx)
				{
					m_PartsRoot.SetParts( curEquip, _view);
					m_PartsRoot.GenerateParts();
				}
				else if( Item.eEQUIP.Weapon == curEquip)
				{
					m_PartsRoot.SetParts( curEquip, _cosItem);
					m_PartsRoot.GenerateParts();
				}
			}
			else
			{
				m_PartsRoot.SetParts( curEquip, _view);
				m_PartsRoot.GenerateParts();
			}
		}
		else
		{
			Debug.LogError( "AsModel::SetEqupParts()[ null != entity && null != m_PartsRoot ]");
		}
	}
	
	
	// SetCosEqupParts
	public void SetCosEqupParts( int iSlot, sITEMVIEW _view)
	{
		AsUserEntity entity = Entity as AsUserEntity;
		if( ( null != entity) && ( null != m_PartsRoot))
		{			
			Item.eEQUIP curEquip = GetPartsType( iSlot);
			
			m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
			
			if( Item.eEQUIP.Fairy == curEquip || Item.eEQUIP.Wing == curEquip)
			{
				m_PartsRoot.SetParts( curEquip, _view);
				m_PartsRoot.GenerateParts();
			}
			else if( true == PartsRoot.IsCostumeOn( curEquip, entity.isCostumeOnOff) )
			{
				sITEMVIEW _norItem = entity.getCharView[iSlot];
				bool bExistEquip = !( 0 == _norItem.nItemTableIdx || int.MaxValue == _norItem.nItemTableIdx);
				bool bCurCosExist = !( 0 == _view.nItemTableIdx || int.MaxValue == _view.nItemTableIdx);
				
				if( false == bCurCosExist && true == bExistEquip)
				{
					m_PartsRoot.SetParts( curEquip, _norItem);
					m_PartsRoot.GenerateParts();
				}
				else
				{
					m_PartsRoot.SetParts( curEquip, _view);
					m_PartsRoot.GenerateParts();
				}
			}
		}
		else
		{
			Debug.LogError( "AsModel::SetCosEqupParts()[ null != entity && null != m_PartsRoot ]");
		}
	}
	
	public static Item.eEQUIP GetPartsType( int iIndex)
	{
		switch( iIndex)
		{
		case 0:	return Item.eEQUIP.Weapon;
		case 1:	return Item.eEQUIP.Head;
		case 2:	return Item.eEQUIP.Armor;
		case 3:	return Item.eEQUIP.Gloves;
		case 4:	return Item.eEQUIP.Point;
		}
		
		if( iIndex == Inventory.fairyEquipSlotIdx )
			return Item.eEQUIP.Fairy;
		
		if( iIndex == Inventory.wingEquipSlotIdx )
			return Item.eEQUIP.Wing;
		
		Debug.LogError( "AsModel::GetPartsType()[ no index : " + iIndex);
		return Item.eEQUIP.Armor;
	}
	
	string GetBaseBonPath()
	{
		AsUserEntity entity = Entity as AsUserEntity;
		if( null == entity)
			return "";
		
		eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		eGENDER gender = entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
		eRACE tribe = entity.GetProperty<eRACE>( eComponentProperty.RACE);
		Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( tribe, __class);
		switch( gender)
		{
		case eGENDER.eGENDER_MALE:
			return record.ModelingPath_Male;
		case eGENDER.eGENDER_FEMALE:
			return record.ModelingPath_Female;
		}
		
		return "";
	}
	
	/*public void SetParts( Item.eEQUIP equip, sITEMVIEW _itemview )
	{		
		m_PartsRoot.SetParts( equip, _itemview); 	
	}	*/
	
	public void SetPartsUseItemId( Item.eEQUIP equip, int iItemID )
	{
		m_PartsRoot.SetPartsUseItemId( equip, iItemID);
	}
	#endregion
	
	void InitialUserCharacter()
	{
		m_dicDummy.Clear();
		
		if( m_ModelType == eModelType.Normal)
		{
			#region - normal -
			Tbl_Class_Record record;
		
			if( Entity.ContainProperty( eComponentProperty.RACE) == true && Entity.ContainProperty( eComponentProperty.CLASS) == true)
			{
				eRACE tribe = Entity.GetProperty<eRACE>( eComponentProperty.RACE);
				eCLASS __class = Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
				record = AsTableManager.Instance.GetTbl_Class_Record( tribe, __class);
			}
			else
			{
				Debug.LogError( "[AsModel]Init: Invalid property");
				return;
			}
			
			if( record != null)
			{
				m_PartsRoot = GetComponentInChildren<PartsRoot>();
				if( null == m_PartsRoot)
					m_PartsRoot = gameObject.AddComponent<PartsRoot>();
				
				if( eEntityType.USER == Entity.EntityType)
				{
					InitialParts( Entity as AsUserEntity);
				}
			}
			else
			{
				Debug.LogError( "AsModel::InitialUserCharacter: no data table record. CHECK tribe or class");
			}
			#endregion
		}
		else
		{
			#region - shop opening -
			GameObject model = Instantiate( ResourceLoad.LoadGameObject( "Character/Pc/Town/PrivateShop")) as GameObject;
			
			Entity.SetModelObject( model);
			Entity.ModelObject.name = "Model";
			Entity.ModelObject.transform.parent = transform;
			Entity.ModelObject.transform.localPosition = Vector3.zero;
			Entity.ModelObject.transform.localRotation = Quaternion.identity;
			
	#if _USER_SHADOW_
			m_dicDummy.Add( "DummyLead", SearchHierarchyTransform( Entity.ModelObject.transform, "DummyLead"));
			m_dicDummy.Add( "DummyLeadTop", SearchHierarchyTransform( Entity.ModelObject.transform, "DummyLeadTop"));
			m_dicDummy.Add( "DummyLeadBottom", SearchHierarchyTransform( Entity.ModelObject.transform, "DummyLeadBottom"));
			
			MakeShadow( 1.3f);
	#endif
			#endregion
		}
	}
	
	IEnumerator InitialUserCharacter_Assetbundle()
	{
		m_dicDummy.Clear();
		
		if( m_ModelType == eModelType.Normal)
		{
			Tbl_Class_Record record = null;
		
			if( Entity.ContainProperty( eComponentProperty.RACE) == true && Entity.ContainProperty( eComponentProperty.CLASS) == true)
			{
				eRACE tribe = Entity.GetProperty<eRACE>( eComponentProperty.RACE);
				eCLASS __class = Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
				record = AsTableManager.Instance.GetTbl_Class_Record( tribe, __class);
			}
			else
			{
				Debug.LogError( "[AsModel]Init: Invalid property");
			}
			
			if( record != null)
			{
				m_PartsRoot = GetComponentInChildren<PartsRoot>();
				if( null == m_PartsRoot)
					m_PartsRoot = gameObject.AddComponent<PartsRoot>();
				
				if( eEntityType.USER == Entity.EntityType)
				{
					AsUserEntity entity = Entity as AsUserEntity;
					startedTime = Time.realtimeSinceStartup;
					Debug.Log( "AsModel::InitialParts: begin time checking");// time check
					
					if( null == entity)
						Debug.LogError( "AsModel::InitalParts() [ null == entity ]");
					
					if( null == entity.getCharView || null == entity.getCosItemView)
						Debug.LogError( "AsModel::InitalParts() [ null == entity.getCharView || null == entity.getCosItemView ]");
					
					if( AsGameDefine.ITEM_SLOT_VIEW_COUNT != entity.getCharView.Length || AsGameDefine.ITEM_SLOT_COS_VIEW_COUNT != entity.getCosItemView.Length) 
						Debug.LogError( "AsModel::InitialParts() [ !( AsGameDefine.ITEM_SLOT_VIEW_COUNT == _partsData.Length) ] length : " + entity.getCharView.Length);
					
					eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);		
					string strBasePath = GetBaseBonPath();
					
					if( true == m_PartsRoot.Create( strBasePath, entity.GetProperty<eGENDER>( eComponentProperty.GENDER), __class, entity.UniqueId))
					{
						Entity.SetModelObject( m_PartsRoot.getBone);
						InitCharacterDummy( Entity.ModelObject.transform);
						InitWeaponDummy( Entity.ModelObject.transform);
						Entity.ModelObject.name = "Model";
						Entity.ModelObject.transform.parent = transform;
						Entity.ModelObject.transform.localPosition = Vector3.zero;
						Entity.ModelObject.transform.localRotation = Quaternion.identity;
						
						sITEMVIEW[] partsData = entity.getCharView;	
						sITEMVIEW[] cosPartsData = entity.getCosItemView;					
						
						m_PartsRoot.SetStrengthenCount( entity.getCharView[0].nStrengthenCount);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Weapon, partsData[0], cosPartsData[0]);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Head, partsData[1], cosPartsData[1]);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Armor, partsData[2], cosPartsData[2]);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Gloves, partsData[3], cosPartsData[3]);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Point, partsData[4], cosPartsData[4]);
						
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Wing, null, cosPartsData[Inventory.wingEquipSlotIdx]);
						PartsRoot.SetParts( entity.isCostumeOnOff, m_PartsRoot, Item.eEQUIP.Fairy, null, cosPartsData[Inventory.fairyEquipSlotIdx]);
						SetPartsUseItemId( Item.eEQUIP.Hair, entity.getHairItemIndex);
						
						yield return StartCoroutine( m_PartsRoot.GenerateParts_Assetbundle());
					}
				}
			}
			else
			{
				Debug.LogError( "AsModel::InitialUserCharacter: no data table record. CHECK tribe or class");
			}
		}
		else
		{
			yield return StartCoroutine( AsPStoreManager.Instance.RequestShopModeling( m_Entity));
			
			Debug.Log( "AsModel::InitialUserCharacter_Assetbundle: m_Entity.ModelObject is set [" + m_Entity.ModelObject + "]");
			m_Entity.ModelObject.name = "Model";
			m_Entity.ModelObject.transform.parent = transform;
			m_Entity.ModelObject.transform.localPosition = Vector3.zero;
			
			Debug.Log( "AsModel::InitialUserCharacter_Assetbundle: dummy setting began.");
			m_dicDummy.Add( "DummyLead", SearchHierarchyTransform( m_Entity.ModelObject.transform, "DummyLead"));
			m_dicDummy.Add( "DummyLeadTop", SearchHierarchyTransform( m_Entity.ModelObject.transform, "DummyLeadTop"));
			m_dicDummy.Add( "DummyLeadBottom", SearchHierarchyTransform( m_Entity.ModelObject.transform, "DummyLeadBottom"));
		}
	}

	void InitialNpcCharacter()
	{
		m_dicDummy.Clear();
		
		Tbl_Npc_Record record;
		
		if( Entity.ContainProperty( eComponentProperty.NPC_ID) == true)
		{
			int id = Entity.GetProperty<int>( eComponentProperty.NPC_ID);
			record = AsTableManager.Instance.GetTbl_Npc_Record( id);
		}
		else
		{
			Debug.LogError( "[AsModel]Init: Invalid property");
			return;
		}
		
		if( null == record)
		{
			Debug.LogError( "AsModel::InitialNpcCharacter: no data table record. CHECK NPC ID");
			return;
		}
	
		string path = record.ModelingPath;
		UnityEngine.Object obj = ResourceLoad.LoadGameObject( path);
		if( obj == null)
		{
			Debug.LogError( "AsModel::InitialNpcCharacter: There is no [" + path + "] modeling resource.");
			obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
		}
		
		GameObject model = Instantiate( obj) as GameObject;
		
		Entity.SetModelObject( model);
		InitMonsterDummy( Entity.ModelObject.transform);
		Entity.ModelObject.name = "Model";
		Entity.ModelObject.transform.parent = transform;
		Entity.ModelObject.transform.localPosition = Vector3.zero;
		Entity.ModelObject.transform.localRotation = Quaternion.identity;
		
#if _NPC_SHADOW_
		MakeShadow();
#endif
	}
	
	#region - init dummy -
	void InitCharacterDummy( Transform _trn)
	{
		m_dicDummy.Add( "DummyLead", SearchHierarchyTransform( _trn, "DummyLead"));
		m_dicDummy.Add( "CharacterBone", SearchHierarchyTransform( _trn, "CharacterBone"));
		
		m_dicDummy.Add( "DummyLeadTop", SearchHierarchyTransform( _trn, "DummyLeadTop"));
		m_dicDummy.Add( "DummyLeadBottom", SearchHierarchyTransform( _trn, "DummyLeadBottom"));
		m_dicDummy.Add( "DummyLeadHit", SearchHierarchyTransform( _trn, "DummyLeadHit"));
		
		m_dicDummy.Add( "DummyCharacterTop", SearchHierarchyTransform( _trn, "DummyCharacterTop"));
		m_dicDummy.Add( "DummyCharacterCenter", SearchHierarchyTransform( _trn, "DummyCharacterCenter"));
		m_dicDummy.Add( "DummyCharacterBottom", SearchHierarchyTransform( _trn, "DummyCharacterBottom"));
		m_dicDummy.Add( "DummyCharacterChest", SearchHierarchyTransform( _trn, "DummyCharacterChest"));
		m_dicDummy.Add( "DummyCharacterBack", SearchHierarchyTransform( _trn, "DummyCharacterBack"));
		m_dicDummy.Add( "DummyCharacterHand_R", SearchHierarchyTransform( _trn, "DummyCharacterHand_R"));
		m_dicDummy.Add( "DummyCharacterHand_L", SearchHierarchyTransform( _trn, "DummyCharacterHand_L"));
		m_dicDummy.Add( "DummyCharacterWeapon_R", SearchHierarchyTransform( _trn, "DummyCharacterWeapon_R"));
		m_dicDummy.Add( "DummyCharacterWeapon_L", SearchHierarchyTransform( _trn, "DummyCharacterWeapon_L"));
		
		//mob
		m_dicDummy.Add( "Bip01 R Hand", SearchHierarchyTransform( _trn, "Bip01 R Hand"));
		m_dicDummy.Add( "Bip01 L Hand", SearchHierarchyTransform( _trn, "Bip01 L Hand"));
	}
	
	void InitWeaponDummy( Transform _trn)
	{
		m_dicDummy.Add( "DummyWeaponHandle", SearchHierarchyTransform( _trn, "DummyWeaponHandle"));
		m_dicDummy.Add( "DummyWeaponCenter", SearchHierarchyTransform( _trn, "DummyWeaponCenter"));
		m_dicDummy.Add( "DummyWeaponLauncher", SearchHierarchyTransform( _trn, "DummyWeaponLauncher"));
		m_dicDummy.Add( "DummyTrailUp", SearchHierarchyTransform( _trn, "DummyTrailUp"));
		m_dicDummy.Add( "DummyTrailDown", SearchHierarchyTransform( _trn, "DummyTrailDown"));
		m_dicDummy.Add( "DummyShield", SearchHierarchyTransform( _trn, "DummyShield"));
		m_dicDummy.Add( "DummyProjectile", SearchHierarchyTransform( _trn, "DummyProjectile"));
	}
	#endregion
	
	#region - init monster dummy -
	void InitMonsterDummy( Transform _trn)
	{
		m_dicDummy.Add( "DummyLead", SearchHierarchyTransform( _trn, "DummyLead"));
		
		m_dicDummy.Add( "DummyLeadTop", SearchHierarchyTransform( _trn, "DummyLeadTop"));
		m_dicDummy.Add( "DummyLeadBottom", SearchHierarchyTransform( _trn, "DummyLeadBottom"));
		m_dicDummy.Add( "DummyLeadHit", SearchHierarchyTransform( _trn, "DummyLeadHit"));
		
		m_dicDummy.Add( "DummyCharacterTop", SearchHierarchyTransform( _trn, "DummyCharacterTop"));
		m_dicDummy.Add( "DummyCharacterCenter", SearchHierarchyTransform( _trn, "DummyCharacterCenter"));
		m_dicDummy.Add( "DummyCharacterBottom", SearchHierarchyTransform( _trn, "DummyCharacterBottom"));
		m_dicDummy.Add( "DummyCharacterChest", SearchHierarchyTransform( _trn, "DummyCharacterChest"));
		
		m_dicDummy.Add( "DummyCharacterHand_R", SearchHierarchyTransform( _trn, "DummyCharacterHand_R"));
		m_dicDummy.Add( "DummyCharacterHand_L", SearchHierarchyTransform( _trn, "DummyCharacterHand_L"));
		m_dicDummy.Add( "DummyCharacterBack", SearchHierarchyTransform( _trn, "DummyCharacterBack"));
		m_dicDummy.Add( "DummyCharacterMouth", SearchHierarchyTransform( _trn, "DummyCharacterMouth"));
		
		m_dicDummy.Add( "DummyWeaponHandle", SearchHierarchyTransform( _trn, "DummyWeaponHandle"));
		m_dicDummy.Add( "DummyWeaponCenter", SearchHierarchyTransform( _trn, "DummyWeaponCenter"));
		m_dicDummy.Add( "DummyWeaponLauncher", SearchHierarchyTransform( _trn, "DummyWeaponLauncher"));
		
		//pet
		m_dicDummy.Add( "Dummy_Acc", SearchHierarchyTransform( _trn, "Dummy_Acc"));
	}
	#endregion
	
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
	#endregion
	
	#region - delegate -
	eModelType CheckModelTypeDelegate()
	{
		return m_ModelType;
	}
	
	eModelLoadingState CheckModelLoadingStateDelegate()
	{
		return m_ModelLoadingState;
	}
	
	bool CheckPartsLoadedDelegate()
	{
		return m_PartsLoaded;
	}
	#endregion
	
	private void _CreateNamePanel( AsBaseEntity baseEntity)
	{
		string strName = baseEntity.GetProperty<string>( eComponentProperty.NAME);
		switch( baseEntity.EntityType)
		{
		case eEntityType.USER:
			AsUserEntity entity = baseEntity as AsUserEntity;
			if( entity.namePanel != null)
				Destroy( entity.namePanel);
			
			if( m_ModelType == eModelType.Normal)
				AsHUDController.Instance.panelManager.CreateNamePanel_User( baseEntity, strName, ((AsUserEntity)baseEntity).UniqueId);
			break;
		case eEntityType.NPC:
			{
				if (strName != string.Empty)
					AsHUDController.Instance.panelManager.CreateNamePanel_Npc(baseEntity, strName, ((AsNpcEntity)baseEntity).TableIdx);
				
				if (baseEntity.namePanel != null)
					AsHUDController.Instance.panelManager.CreateQuestPanel_Npc(baseEntity, baseEntity.namePanel.NameText.BaseHeight);
				
				AsHUDController.Instance.panelManager.CreateUseItemToTargetPanel(baseEntity);
			}
			break;
		}
		
		baseEntity.TargetMarkProc();
	}
	
	private void _CreatePvpPointEffect( AsBaseEntity baseEntity)
	{
		Map map = TerrainMgr.Instance.GetCurrentMap();
		if( null == map)
			return;
		
		if( eMAP_TYPE.Town != map.MapData.getMapType)
			return;
		
		if( eEntityType.USER == baseEntity.EntityType)
		{
			AsUserEntity userEntity = baseEntity as AsUserEntity;
			
			int nYesterdayPvpPoint = userEntity.YesterdayPvpPoint;
			uint nYesterdayPvpRankRate = userEntity.YesterdayPvpRankRate;
			
			if( userEntity.UniqueId == AsUserInfo.Instance.SavedCharStat.uniqKey_)
			{
				nYesterdayPvpPoint = AsUserInfo.Instance.YesterdayPvpPoint;
				nYesterdayPvpRankRate = AsUserInfo.Instance.YesterdayPvpRankRate;
			}

			//int nID = AsPvpManager.Instance.FindPvpGradeID( userEntity.PvpPoint);
			int nID = AsPvpManager.Instance.FindPvpGradeID( nYesterdayPvpPoint, nYesterdayPvpRankRate);
			Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( nID);
			
			if( null == record)
			{
				Debug.Log( "AsModel::_CreatePvpPointEffect(): record == null, nID: " + nID);
				return;
			}
			
			if( record.GradeTownEffect.Length > 0)
				AsEffectManager.Instance.PlayEffect( record.GradeTownEffect, userEntity.transform, false, 0f, 1.0f);
		}
	}

	private void SetShadowLayer( int layer)
	{
		if( null == m_ShadowObject)
			return;
		
		m_ShadowObject.gameObject.layer = layer;
	}
	
	#region - method -
	void InitHide()
	{
		AsUserEntity user = m_Entity as AsUserEntity;
		if(user == null)
			return;
		
		if(user.Hide == true)
		{
			float alpha = 0;
			if(m_Entity.FsmType == eFsmType.PLAYER)
				alpha = m_HideAlpha;
			
			SetAlpha(alpha);
			
			m_Entity.gameObject.layer = LayerMask.NameToLayer("Hide");
			
			m_ShadowObject.SetActiveRecursively(false);
			
			if( m_Entity.namePanel != null)
				m_Entity.namePanel.gameObject.SetActiveRecursively(false);
		}
	}
		
	void SetAlpha(float _alpha)
	{
		if(m_Entity.ModelObject == null)
			return;
		
		Renderer[] renderers = m_Entity.ModelObject.GetComponentsInChildren<Renderer>();
		foreach( Renderer ren in renderers)
		{
			if( null == ren)
				continue;
			
			foreach(Material node in ren.materials)
			{
				node.renderQueue = 3000;
				node.SetFloat("_Alpha", _alpha);
			}
		}
	}
	#endregion
}
