//#define TOWN_DEF_PARTS
//#define USE_OLD_COSTUME
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PartsRoot : MonoBehaviour
{
	//---------------------------------------------------------------------
    /* Variable*/
    //---------------------------------------------------------------------	       
    private GameObject m_goBone;
	private Transform[] m_BoneTransforms;
    private SkinnedMeshRenderer m_SkinnedMeshRenderer;	
    private List<CombineInstance> m_CombineInstances = new List<CombineInstance>();
    private List<Material> m_Materials = new List<Material>();
    private List<Transform> m_bones = new List<Transform>();    
	
	private eCLASS m_CurClass;
	private eGENDER m_CurGender;	
	private uint m_nCharUniqKey;
	private Dictionary<Item.eEQUIP, PartsElement> m_PartsList = new Dictionary<Item.eEQUIP, PartsElement>();
	private List<Item.eEQUIP> m_LoadList = new List<Item.eEQUIP>();
	private byte m_nStrengthenCount = 0;
	
	public void SetStrengthenCount( byte _nStrengthenCount )
	{
		m_nStrengthenCount = _nStrengthenCount;
	}
	
	public byte getStrengthenCount
	{
		get
		{
			return m_nStrengthenCount;
		}
	}
	//dopamin #17440
	public uint getOwnerUniqKey
	{
		get 
		{
			return m_nCharUniqKey;
		}
	}
	
	
	
	
    //---------------------------------------------------------------------
    /* functon */
    //---------------------------------------------------------------------
	
#if USE_OLD_COSTUME
	public static void SetParts( bool isCostumeOnOff, PartsRoot _partsRoot, Item.eEQUIP equip, sITEMVIEW norParts, sITEMVIEW cosParts )
	{		
		if( null == cosParts )
		{
			if( equip != Item.eEQUIP.Fairy && equip != Item.eEQUIP.Wing )
				_partsRoot.SetParts( equip, norParts );
			return;				
		}
		
		if( Item.eEQUIP.Wing == equip || Item.eEQUIP.Fairy == equip )
		{			
			_partsRoot.SetParts( equip, cosParts ); 
			return;
		}
		
		if( true == isCostumeOnOff )
		{				
			if( 0 == cosParts.nItemTableIdx || int.MaxValue == cosParts.nItemTableIdx )
			{
				_partsRoot.SetParts( equip, norParts );
				
			}
			else
			{
				_partsRoot.SetParts( equip, cosParts ); 	
			}					
		}	
		else
		{
			_partsRoot.SetParts( equip, norParts );
			/*if( 0 != norParts.nItemTableIdx && int.MaxValue != norParts.nItemTableIdx )
			{
				_partsRoot.SetParts( equip, norParts );					
			}
			else
			{
				_partsRoot.SetParts( equip, cosParts ); 
			}	*/		
		}
	}
	
	public static bool IsCostumeOn( Item.eEQUIP _equip, bool isCostumeOnOff )	
	{
		return isCostumeOnOff;
	}
	
	public static bool GetCosOnDef()
	{
		return true;
	}
	public static bool IsEquipView( Item.eEQUIP _equip, bool isCostumeOnOff )	
	{
		return true;
	}

#else
	
	public static int GetCosOnDef()
	{
		return 253;
	}
	public static bool IsCostumeOn( Item.eEQUIP _equip, int isCostumeOnOff )	
	{
		switch( _equip )
		{
		case Item.eEQUIP.Weapon:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Weapon); 	
			
		case Item.eEQUIP.Head:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_costume);		
			
		case Item.eEQUIP.Armor:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Armor);
			
		case Item.eEQUIP.Gloves:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Gloves);
			
		case Item.eEQUIP.Point:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Point);
			
		case Item.eEQUIP.Wing:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Wing);
			
		case Item.eEQUIP.Fairy:
			return 0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Fairy);
		}	
		return false;
	}
	
	public static bool IsEquipView( Item.eEQUIP _equip, int isCostumeOnOff )	
	{
		switch( _equip )
		{
		case Item.eEQUIP.Head:
			return (0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_costume)) || (0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Head_normal));
			
		case Item.eEQUIP.Wing:
			return (0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Wing));
			
		case Item.eEQUIP.Fairy:
			return (0 != (isCostumeOnOff & (int)eITEM_PARTS_VIEW.Fairy));
		}
		
		return true;
	}
	
	public static void SetParts( int isCostumeOnOff, PartsRoot _partsRoot, Item.eEQUIP equip, sITEMVIEW norParts, sITEMVIEW cosParts )
	{		
		if( false == IsEquipView( equip, isCostumeOnOff ) )	
		{
			_partsRoot.SetEmpty(equip);
			return;
		}
		if( null == cosParts )
		{
			if( equip != Item.eEQUIP.Fairy && equip != Item.eEQUIP.Wing )
				_partsRoot.SetParts( equip, norParts );
			return;				
		}
		
		if( Item.eEQUIP.Wing == equip || Item.eEQUIP.Fairy == equip )
		{			
			_partsRoot.SetParts( equip, cosParts ); 
			return;
		}		
		 
		
		if( true == IsCostumeOn(equip, isCostumeOnOff) )
		{				
			if( 0 == cosParts.nItemTableIdx || int.MaxValue == cosParts.nItemTableIdx )
			{
				_partsRoot.SetParts( equip, norParts );
				
			}
			else
			{
				_partsRoot.SetParts( equip, cosParts ); 	
			}					
		}	
		else
		{
			_partsRoot.SetParts( equip, norParts );					
		}
	}	
#endif
	// Get
	public GameObject getBone
	{
		get
		{
			return m_goBone;
		}
	}

	public eGENDER getGender
	{
		get
		{
			return m_CurGender;	
		}
	}
	
	public eCLASS getClass
	{
		get
		{
			return m_CurClass;
		}
	}
	
	int GetDefPartsItemID( Item.eEQUIP equip )
	{	
		switch( getClass )
		{
		case eCLASS.DIVINEKNIGHT:			
			switch( equip )
			{
			case Item.eEQUIP.Armor:					
				return 20000;
			case Item.eEQUIP.Hair:
				return 10000;
			case Item.eEQUIP.Gloves:
				return 30000;
			}			
			break;
		case eCLASS.MAGICIAN:
			switch( equip )
			{
			case Item.eEQUIP.Armor:					
				return 22000;
			case Item.eEQUIP.Hair:
				return 12000;
			case Item.eEQUIP.Gloves:
				return 32000;
			}			
			break;
		case eCLASS.CLERIC:
			switch( equip )
			{
			case Item.eEQUIP.Armor:					
				return 26000;
			case Item.eEQUIP.Hair:
				return 16000;
			case Item.eEQUIP.Gloves:
				return 36000;
			}			
			break;
		
		case eCLASS.HUNTER:
			switch( equip )
			{
			case Item.eEQUIP.Armor:					
				return 24000;
			case Item.eEQUIP.Hair:
				return 14000;
			case Item.eEQUIP.Gloves:
				return 34000;
			}			
			break;
		case eCLASS.ASSASSIN:			
			Debug.LogError("GetDefPartsItemID()[ASSASSIN default parts empty]");
			break;	
			
		default:
			Debug.LogError("GetDefPartsItemID()[default]");
			break;
		}
		
		return 0;
	}
	
	static public int GetDefWeaponItemID( eCLASS _class )
	{	
		switch( _class )
		{
		case eCLASS.DIVINEKNIGHT:			
			return 154101;
		case eCLASS.MAGICIAN:
			return 154102;
		case eCLASS.CLERIC:
			return 6400;
		
		case eCLASS.HUNTER:
			return 4000;
		case eCLASS.ASSASSIN:			
			Debug.LogError("GetDefPartsItemID()[ASSASSIN default parts empty]");
			break;	
			
		default:
			Debug.LogError("GetDefPartsItemID()[default]");
			break;
		}
		
		return 0;
	}
	
	public bool IsCheckNoParts(Item.eEQUIP eEquip)
	{
		switch( eEquip )
		{
		case Item.eEQUIP.Earring:
		case Item.eEQUIP.Necklace:
		case Item.eEQUIP.Ring:
		case Item.eEQUIP.SubAcc:
			return true;			
		}
		
		return false;
	}
	
	private bool IsHadPartsItem( Item.eEQUIP eEquip )
	{
		if( true == m_PartsList.ContainsKey( eEquip ) )
		{
			if( true == m_PartsList[eEquip].isHavePartsItem )
				return true;
		}
		
		return false;
	}
	
	
	protected bool IsNeedGenerateParts()
	{
		if( false == IsHadPartsItem( Item.eEQUIP.Armor ) || 
			false == IsHadPartsItem( Item.eEQUIP.Gloves) ||
			( false == IsHadPartsItem(Item.eEQUIP.Hair) && false == IsHadPartsItem(Item.eEQUIP.Head) ) )
		{
			return true;
		}
		
		foreach (KeyValuePair<Item.eEQUIP, PartsElement> ele in m_PartsList)
		{
			if( true == ele.Value.isNeedResetParts )
				return true;
		}
		
		return false;
	}	
		
	
	
	public void Clear()
	{
		if( null != m_goBone )
		{
			GameObject.DestroyObject( m_goBone );
			m_goBone = null;
		}
		
		m_PartsList.Clear();
	}
	
		
	// create	
    public bool Create( string strBonePath, eGENDER eGender, eCLASS _class, uint _nCharUniqKey )
    {
		float tempTime = 0;//$yde
		
		if( null != m_goBone )
		{
			Debug.LogError("null != m_goBone");
			return false;			
		}
		
		m_CurGender = eGender;
		m_nCharUniqKey = _nCharUniqKey;
		m_CurClass = _class;
					
        // load boneRoot
		tempTime = Time.realtimeSinceStartup;//$yde
		GameObject goBone = ResourceLoad.LoadGameObject(strBonePath);
        if( null == goBone )
        {
            Debug.LogError("PartsRoot::Create() [ null == goBone ] filepath : " + strBonePath );
            return false;
        }
		Debug.Log("PartsRoot::Create: ResourceLoad.LoadGameObject(strBonePath)'s elapsed time = " + (Time.realtimeSinceStartup - tempTime));//$yde
		
		// create boneRoot
		tempTime = Time.realtimeSinceStartup;//$yde
        m_goBone = GameObject.Instantiate(goBone, Vector3.zero, Quaternion.identity) as GameObject;
        if (null == m_goBone)
        {
            Debug.LogError("PartsRoot::Create() [ null == m_goBone ]");
            return false;
        }
		Debug.Log("PartsRoot::Create: GameObject.Instantiate(goBone, Vector3.zero, Quaternion.identity) as GameObject's  elapsed time = " + (Time.realtimeSinceStartup - tempTime));//$yde
			
		// attach boneRoot
      	m_goBone.transform.parent = transform; 
		m_goBone.transform.localScale = Vector3.one;
		m_goBone.transform.localPosition = Vector3.zero;
		m_goBone.transform.localRotation = Quaternion.identity;
		
			
		// bones
		tempTime = Time.realtimeSinceStartup;//$yde
        m_BoneTransforms = m_goBone.GetComponentsInChildren<Transform>();
		if( null == m_BoneTransforms )
		{
			Debug.LogError("PartsRoot::Create() [ null == m_BoneTransforms ]");
            return false;
		}
		Debug.Log("PartsRoot::Create: m_goBone.GetComponentsInChildren<Transform>()'s  elapsed time = " + (Time.realtimeSinceStartup - tempTime));//$yde
        
		// skinnedMeshRenderer
		tempTime = Time.realtimeSinceStartup;//$yde
		m_SkinnedMeshRenderer =m_goBone.GetComponentInChildren<SkinnedMeshRenderer>();
		if( null == m_SkinnedMeshRenderer )
        	m_SkinnedMeshRenderer = m_goBone.AddComponent<SkinnedMeshRenderer>();
		//m_SkinnedMeshRenderer.updateWhenOffscreen = true;
		//m_SkinnedMeshRenderer.sharedMesh = new Mesh();
		Debug.Log("PartsRoot::Create: m_goBone.GetComponentsInChildren<Transform>()'s  elapsed time = " + (Time.realtimeSinceStartup - tempTime));//$yde
		
		
		// weapon attach
		tempTime = Time.realtimeSinceStartup;//$yde
		m_PartsList.Add( Item.eEQUIP.Weapon, new WeaponElement( m_goBone.transform, "DummyCharacterWeapon_R", this) );	
		m_PartsList.Add( Item.eEQUIP.Wing, new WingElement( m_goBone.transform, "DummyCharacterChest") );
		m_PartsList.Add( Item.eEQUIP.Fairy, new FairyElement() );
		Debug.Log("PartsRoot::Create: m_goBone.GetComponentsInChildren<Transform>()'s  elapsed time = " + (Time.realtimeSinceStartup - tempTime));//$yde
		
			
		m_LoadList.Clear();
		m_LoadList.Add( Item.eEQUIP.Head );
		m_LoadList.Add( Item.eEQUIP.Armor );
		m_LoadList.Add( Item.eEQUIP.Gloves );
		m_LoadList.Add( Item.eEQUIP.Point );
		//m_LoadList.Add( Item.eEQUIP.Earring );
		//m_LoadList.Add( Item.eEQUIP.Necklace );
		//m_LoadList.Add( Item.eEQUIP.Ring );
		//m_LoadList.Add( Item.eEQUIP.SubAcc );
		m_LoadList.Add( Item.eEQUIP.Hair );
		m_LoadList.Add( Item.eEQUIP.Face );
		m_LoadList.Add( Item.eEQUIP.Fairy );
		m_LoadList.Add( Item.eEQUIP.Wing );
		m_LoadList.Add( Item.eEQUIP.Weapon );
		
       	return true;
    }
	
	public void GenerateParts()
    {
		if( false == IsNeedGenerateParts() )
			return;
		
        m_CombineInstances.Clear();
        m_Materials.Clear();
        m_bones.Clear(); 		
		
		CheckDefaultParts( Item.eEQUIP.Hair );
		CheckDefaultParts( Item.eEQUIP.Armor );
		CheckDefaultParts( Item.eEQUIP.Gloves );

        //foreach (KeyValuePair<Item.eEQUIP, PartsElement> ele in m_PartsList)
		foreach( Item.eEQUIP _equip in m_LoadList )
        {
			
			if( false == m_PartsList.ContainsKey( _equip ) ) 
				continue;
			
			PartsElement eleValue = m_PartsList[ _equip ];
			
			
			MeshElement meshEle = eleValue as MeshElement;
			if( null == meshEle )
				continue;
			
			if( false == meshEle.isHavePartsItem )
				continue;		
			
			if( Item.eEQUIP.Hair == _equip && true == IsHadPartsItem(Item.eEQUIP.Head) )
			{
				continue;
			}		
			
			eleValue.CreateData( getGender );           
			m_Materials.AddRange( meshEle.GetMaterials() );
			
			CombineInstance combineInstance = new CombineInstance();
       	 	combineInstance.mesh = meshEle.GetResMesh();
        	combineInstance.subMeshIndex = 0;
			
			m_CombineInstances.Add(combineInstance);		
		     
            foreach (string smrBone in meshEle.GetBones() )
            {
                foreach (Transform tr in m_BoneTransforms)
                {
                    if (tr.name != smrBone)
                        continue;

                    m_bones.Add(tr);
                    break;
                }
            }
		}

        if( null == m_SkinnedMeshRenderer.sharedMesh )
			 m_SkinnedMeshRenderer.sharedMesh = new Mesh();
        m_SkinnedMeshRenderer.sharedMesh.CombineMeshes(m_CombineInstances.ToArray(), false, false);
        m_SkinnedMeshRenderer.bones = m_bones.ToArray();
        m_SkinnedMeshRenderer.materials = m_Materials.ToArray();
    }
	
	//public void GenerateParts()
	public IEnumerator GenerateParts_Assetbundle()
	{
		if( true == IsNeedGenerateParts() )
		{
			Debug.Log( "yield return StartCoroutine( LoadParts);");
			yield return StartCoroutine( "LoadParts");
		}
	}
	
	
	static public bool s_isLoading = false;
	
	IEnumerator LoadParts()
    {
		while( true == s_isLoading )
			yield return null;
//			yield return new WaitForSeconds( 0.1f);
		
		s_isLoading = true;
		
        m_CombineInstances.Clear();
        m_Materials.Clear();
        m_bones.Clear(); 		
		
		CheckDefaultParts( Item.eEQUIP.Hair );
		CheckDefaultParts( Item.eEQUIP.Armor );
		CheckDefaultParts( Item.eEQUIP.Gloves );

		foreach( Item.eEQUIP _equip in m_LoadList )
        {
			if( false == m_PartsList.ContainsKey( _equip ) ) 
				continue;
			
			PartsElement eleValue = m_PartsList[ _equip ];
			
			if( null == eleValue )
				continue;
			
			if( false == eleValue.isHavePartsItem )
				continue;
			
			if( false == (eleValue.getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || eleValue.getItem.ItemData.getGender == getGender ) )
			{
				Debug.Log(" PartsRoot::LoadParts() [getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || getItem.ItemData.getGender == eGender");
				continue;
			}
			
			if( Item.eEQUIP.Hair == _equip && true == IsHadPartsItem(Item.eEQUIP.Head) )
				continue;
			
			string path_model;
			string path_texture;
			if( eGENDER.eGENDER_MALE == getGender )
			{
				path_model = eleValue.getItem.ItemData.GetPartsItem_M();
				path_texture = eleValue.getItem.ItemData.GetPartsItemDiff_M();
			}
			else
			{
				path_model = eleValue.getItem.ItemData.GetPartsItem_W();
				path_texture = eleValue.getItem.ItemData.GetPartsItemDiff_W();
			}
			
			// only effect 
			if( Item.eEQUIP.Fairy == _equip )
			{
				FairyElement fairyEle = eleValue as FairyElement;
				if( null == fairyEle )
					continue;
				
				if( -1 != fairyEle.getEffectIdx )
				{
					AsEffectManager.Instance.RemoveEffectEntity( fairyEle.getEffectIdx );
				}
				
				float fScale = 1f;				
				fScale = transform.localScale.x;	
		       	fairyEle.SetEffectIdx( AsEffectManager.Instance.PlayEffect( path_model, transform, true, 0f, fScale) );	
				AsEffectManager.Instance.AddAttachEffectUser(fairyEle.getEffectIdx, getOwnerUniqKey );
				continue;
			}
			
			GameObject goPartsItem = null;	
			Texture tex = null;
			
			if( true == eleValue.isNeedResetParts )
			{			
				// model	
				string[] strSplit = path_model.Split('/');
				string strLower_model = strSplit[strSplit.Length-1].ToLower();
				string strAssetbundlePath_model = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strLower_model );
				int nVersion = AssetbundleManager.Instance.GetCachedVersion( strLower_model );
	
				WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath_model, nVersion);
				yield return www;
				
				AssetBundle bundle = www.assetBundle;
				AssetBundleRequest request = bundle.LoadAsync( strLower_model, typeof( GameObject));
				yield return request;
				
				GameObject obj = request.asset as GameObject;
				if( null == obj)
				{
					Debug.LogError( "PartsRoot::LoadParts: There is no [" + strLower_model + "] modeling resource.");
					obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
				}
				
				goPartsItem = Instantiate( obj) as GameObject;				
				bundle.Unload( false);
				
				// texture
				strSplit = path_texture.Split('/');
				string strRealLower_tex = strSplit[strSplit.Length-1].ToLower();
				string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strRealLower_tex);
				nVersion = AssetbundleManager.Instance.GetCachedVersion( strRealLower_tex);
				
				Debug.Log( "Texture path : " + strAssetbundlePath);
				www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
				yield return www;
				
				bundle = www.assetBundle;
//				request = bundle.LoadAsync( strRealLower_tex, typeof( Texture));
//				yield return request;
//				
//				tex = request.asset as Texture;
				tex = bundle.Load( strRealLower_tex, typeof( Texture)) as Texture;
				
				Debug.Log( "Texture loaded: " + strRealLower_tex + ", " + tex);
				
				bundle.Unload( false);				
			}
			
			if( Item.eEQUIP.Weapon == _equip )
			{
				if( true == eleValue.isNeedResetParts )	
				{
					WeaponElement weaponEle = eleValue as WeaponElement;
					if( null == weaponEle )
						continue;
					
					weaponEle.SetGameObject( goPartsItem, tex, getClass, this );	
					weaponEle.SetNeedResetParts(false);
				}
			}
			else if( Item.eEQUIP.Wing == _equip )
			{
				
				if( true == eleValue.isNeedResetParts )
				{
					WingElement wingEle = eleValue as WingElement;
					if( null == wingEle )
						continue;
					
					wingEle.SetGameObject( goPartsItem, tex );	
					wingEle.SetNeedResetParts(false);
				}
			}			
			else
			{
				MeshElement meshEle = eleValue as MeshElement;
				if( null == meshEle )
					continue;
				
				if( true == eleValue.isNeedResetParts && null != goPartsItem && null != tex)
				{
					SkinnedMeshRenderer smr = goPartsItem.GetComponentInChildren<SkinnedMeshRenderer>();
			        if (null == smr)
			        {
			            Debug.LogError(" MeshElement::CreateData()[ null == smr] itemid : " + meshEle.getItem.ItemID + " itemtype : " + (Item.eEQUIP)meshEle.getItem.ItemData.GetSubType() );
						GameObject.Destroy(goPartsItem);
			            continue;
			        }
					
					if( 0 >= smr.materials.Length )
					{
						Debug.LogError(" MeshElement::CreateData()[ smr.materials empty ] itemid : " + meshEle.getItem.ItemID + " itemtype : " + (Item.eEQUIP)meshEle.getItem.ItemData.GetSubType());
						continue;
					}			
					
					meshEle.SetResMesh( smr.sharedMesh );
//					CombineInstance _CombineInstance = new CombineInstance();
//	       	 		_CombineInstance.mesh = smr.sharedMesh;
//	        		_CombineInstance.subMeshIndex = 0;
//					meshEle.SetCombineInstance( _CombineInstance );
					
					List<string> _gobones = new List<string>();
					
					foreach( Transform tr in smr.bones )
					{
						_gobones.Add( tr.name );
					}
					
					meshEle.SetBones( _gobones );				
					
					Shader shader = AsShaderManager.Instance.Get( "newark_shader");
					
					Material[] mat = new Material[1];
					mat[0] = new Material( shader );
					mat[0].SetTexture( "_maintex", tex);
					meshEle.SetMaterials( mat );			
					
					meshEle.SetNeedResetParts(false);				
					GameObject.DestroyObject(goPartsItem);
				}
				
				CombineInstance _CombineInstance = new CombineInstance();
       	 		_CombineInstance.mesh = meshEle.GetResMesh();
        		_CombineInstance.subMeshIndex = 0;
				
				
				m_Materials.AddRange( meshEle.GetMaterials() );
				m_CombineInstances.Add(_CombineInstance);		
		     
	            foreach (string smrBone in meshEle.GetBones() )
	            {
	                foreach (Transform tr in m_BoneTransforms)
	                {
	                    if (tr.name != smrBone)
	                        continue;
	
	                    m_bones.Add(tr);
	                    break;
	                }
	            }				
			}	
		}
        	
		if( null == m_SkinnedMeshRenderer.sharedMesh )
			 m_SkinnedMeshRenderer.sharedMesh = new Mesh();
		
        m_SkinnedMeshRenderer.sharedMesh.CombineMeshes(m_CombineInstances.ToArray(), false, false);
        m_SkinnedMeshRenderer.bones = m_bones.ToArray();
        m_SkinnedMeshRenderer.materials = m_Materials.ToArray();
		
		s_isLoading = false;
    }

	protected void CheckDefaultParts( Item.eEQUIP eEquip )
	{
		if( true == m_PartsList.ContainsKey( eEquip ) )
		{
			if( false == m_PartsList[eEquip].isHavePartsItem )
			{			
				SetParts( eEquip, GetDefPartsItemID( eEquip ), 0 );				
			}			
		}
		else
		{			
			m_PartsList.Add( eEquip, new MeshElement( eEquip, GetDefPartsItemID( eEquip ), 0, getGender, getClass ) );
		}
	}
	
	
	
	
	// Set
	protected void SetEmpty( Item.eEQUIP eParts )
	{
		if( true == IsCheckNoParts(eParts) )
			return;
		
		if (true == m_PartsList.ContainsKey(eParts))
        {
            m_PartsList[eParts].SetEmpty();			
        }		
	}
	
	
	public void SetParts(Item.eEQUIP eParts, sITEMVIEW itemView )
    {
		if( null == itemView )
		{
			Debug.LogError("PartsRoot::SetParts()[ null == itemView ]" );
			return;
		}
		
		SetParts( eParts, itemView.nItemTableIdx, itemView.nStrengthenCount );
    }
	
	protected bool IsTownNoParts()
	{
		if( false == TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Town ) )
			return false;
		
		if( null == AsEntityManager.Instance.UserEntity )
			return false;
		
#if TOWN_DEF_PARTS
		if(  AsEntityManager.Instance.UserEntity.UniqueId != m_nCharUniqKey )
		{
			return true;
		}
#endif
			
		return false;
	}
	
	public void SetParts(Item.eEQUIP eParts, int _nItemTableIdx, byte _nStrengthenCount )
    {		
		if( true == IsTownNoParts() )
			return;
		
		if( 0 == _nItemTableIdx || int.MaxValue == _nItemTableIdx )
		{
			SetEmpty( eParts );
			return;
		}	
		
		
		if( true == IsCheckNoParts(eParts) )
		{
			Debug.LogError("PartsRoot::SetParts()[ true == IsCheckNoParts(eParts) ]  parts : " + eParts );
			return;
		}
			
        if (true == m_PartsList.ContainsKey(eParts))
        {
            m_PartsList[eParts].SetPartsItem( _nItemTableIdx, _nStrengthenCount, getGender, getClass, this );
			return;
        }		
	
        m_PartsList.Add(eParts, new MeshElement( eParts, _nItemTableIdx, _nStrengthenCount, getGender, getClass ) );
    }
	
	
	public void AttachPartsUseItemId( Item.eEQUIP equip, int iItemTable )
	{
		if( 0 == iItemTable )
			return;	
		
		SetParts( equip, iItemTable, 0 );
		
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			StartCoroutine( LoadParts());
		else
			GenerateParts();
	}
	
	public void SetPartsUseItemId( Item.eEQUIP equip, int iItemTable )
	{
		if( 0 == iItemTable )
			return;	
		
		SetParts( equip, iItemTable, 0 );		
	}	
}
