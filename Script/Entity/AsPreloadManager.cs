using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Dlt_ModelingLoaded(GameObject _obj);

public class AsPreloadManager : MonoBehaviour {

	#region - singleton -
	static AsPreloadManager m_Instance; public static AsPreloadManager Instance{get{return m_Instance;}}
	#endregion

	bool m_UseAssetBundle = false;
	
	[SerializeField] List<string> m_listTransformPath = new List<string>();
	List<GameObject> m_listGameObject = new List<GameObject>();

	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
	}

	// Use this for initialization
	IEnumerator Start () {

		GameObject main = GameObject.Find( "GameMain");
		if( null != main)
		{
			AsGameMain asMain = main.GetComponent<AsGameMain>();
			m_UseAssetBundle = asMain.useAssetbundle;
		}

		if(m_UseAssetBundle == true)
			yield break;

		while(true)
		{
			yield return null;

			if(AssetbundleManager.Instance != null)
				break;
		}

		SetTrnObject();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Dictionary<string, string> m_dicRequestedModelingPath = new Dictionary<string, string>();
	GameObject m_LoadedObject;
	public void LoadModeling(string _path, Dlt_ModelingLoaded _dlt)
	{
		if(m_UseAssetBundle == true)
			StartCoroutine(_LoadModeling_CR(_path, _dlt));
		else
			_dlt(ResourceLoad.CreateGameObject(_path));
	}

	static bool s_ModelLoading = false;
	IEnumerator _LoadModeling_CR(string _path, Dlt_ModelingLoaded _dlt)
	{
		while(true)
		{
			yield return null;

			if(s_ModelLoading == false)
				break;
		}

		s_ModelLoading = true;

		string[] strSplit = _path.Split('/');
		string strName = strSplit[strSplit.Length-1].ToLower();
		string downloadPath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strName);
		int nVersion = AssetbundleManager.Instance.GetCachedVersion( strName);
		
		Debug.Log("PreloadManager::_LoadModeling_CR: WWW.LoadFromCacheOrDownload( " + downloadPath + ", " + nVersion + ")");
		WWW www = WWW.LoadFromCacheOrDownload( downloadPath, nVersion);
		yield return www;
		
		AssetBundle bundle = www.assetBundle;
		AssetBundleRequest request = bundle.LoadAsync( strName, typeof( GameObject));
		yield return request;
		
		GameObject obj = request.asset as GameObject;
		if( null == obj)
		{
			Debug.LogError( "AsPStoreManager::_LoadModeling_CR: There is no [ privateshop.asab ] modeling resource.");
			obj = GameObject.CreatePrimitive( PrimitiveType.Cube);
		}
		
		GameObject model = Instantiate( obj) as GameObject;
		if(model == null)
			Debug.LogError("PreloadManager:: _LoadModeling_CR: invalid path = " + _path);
		
		bundle.Unload( false);
		
		Debug.Log("PreloadManager::_LoadModeling_CR: CreateMaterial( " + model + ", " + strName + ")");
		yield return StartCoroutine( CreateMaterial( model, strName));

		_dlt(model);

		s_ModelLoading = false;
	}

	public void LoadTransformModel()
	{
		StartCoroutine(LoadTransformModel_CR());
	}

	IEnumerator LoadTransformModel_CR()
	{
		foreach(string node in m_listTransformPath)
		{
			string[] strSplit = node.Split('/');
			string strName = strSplit[strSplit.Length-1].ToLower();
			string downloadPath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strName);
			int nVersion = AssetbundleManager.Instance.GetCachedVersion( strName);
			
			Debug.Log("PreloadManager::LoadTransformModel_CR: WWW.LoadFromCacheOrDownload( " + downloadPath + ", " + nVersion + ")");
			WWW www = WWW.LoadFromCacheOrDownload( downloadPath, nVersion);
			yield return www;
			
			AssetBundle bundle = www.assetBundle;
			AssetBundleRequest request = bundle.LoadAsync( strName, typeof( GameObject));
			yield return request;
			
			GameObject obj = request.asset as GameObject;
			if( null == obj)
			{
				Debug.LogError( "AsPStoreManager::LoadTransformModel_CR: There is no [ privateshop.asab ] modeling resource.");
				obj = GameObject.CreatePrimitive( PrimitiveType.Cube);
			}
			
			GameObject model = Instantiate( obj) as GameObject;
			m_listGameObject.Add(model);
			model.transform.parent = transform;
			model.transform.position = new Vector3(99999f, 99999f, 99999f);	
			
			if(model == null)
				Debug.LogError("PreloadManager:: LoadTransformModel_CR: invalid path = " + node);
			
			bundle.Unload( false);
			
			Debug.Log("PreloadManager::LoadTransformModel_CR: CreateMaterial( " + model + ", " + strName + ")");
			yield return StartCoroutine( CreateMaterial( model, strName));
		}
	}

	IEnumerator CreateMaterial( GameObject model, string name)
	{
		AssetbundleHelper helper = model.GetComponent<AssetbundleHelper>();
		if( null == helper)
		{
			Debug.Log( "PreloadManager::CreateMaterial: Not Found Assetbundle Helper: " + name);
			yield break;
		}
		
		Renderer[] rens = model.GetComponentsInChildren<Renderer>();
		foreach( Renderer ren in rens)
		{
			int nIndex = helper.m_listMeshName.IndexOf( ren.name.ToLower());
			if( 0 > nIndex)
			{
				Debug.Log( "PreloadManager::CreateMaterial: InitialNpcCharacter() NoMatchRenderMeshName: " + model.name + ", " + ren.name);
				continue;
			}
			
			if( nIndex > helper.m_listShaderName.Count - 1)
			{
				Debug.Log( "PreloadManager::CreateMaterial: InitialNpcCharacter() index: " + nIndex + ", shader count: " + helper.m_listShaderName.Count);
				continue;
			}
			
			if( nIndex > helper.m_listTextureName.Count - 1)
			{
				Debug.Log( "PreloadManager::CreateMaterial: InitialNpcCharacter() index: " + nIndex + ", texture count: " + helper.m_listTextureName.Count);
				continue;
			}
			
			string strShaderName = helper.m_listShaderName[nIndex];
			string sgtrNameLower = helper.m_listTextureName[nIndex].ToLower();
			string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( sgtrNameLower);
			int nVersion = AssetbundleManager.Instance.GetCachedVersion( sgtrNameLower);
			
			Debug.Log( "PreloadManager::CreateMaterial: Texture path : " + strAssetbundlePath);
			
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

	void SetTrnObject()
	{
		foreach(string node in m_listTransformPath)
		{
			GameObject obj = ResourceLoad.CreateGameObject(node);
			
			m_listGameObject.Add(obj);
			obj.transform.parent = transform;
			obj.transform.position = new Vector3(99999f, 99999f, 99999f);	
			
			if(obj == null)
				Debug.LogError("PreloadManager:: Start: invalid path = " + node);
		}
	}

	public GameObject GetTrnObject(int _idx)
	{
		if(m_listGameObject.Count >= _idx && m_listGameObject[_idx - 1] != null)
			return Instantiate(m_listGameObject[_idx - 1]) as GameObject;
		else
			return null;
	}
}
