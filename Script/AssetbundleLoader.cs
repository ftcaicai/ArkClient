using System;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;

public class AssetbundleLoader
{
	private static Dictionary<string, WWW> m_WWWs = new Dictionary<string, WWW>();
	private static Dictionary<string, WWW> m_SceneWWWs = new Dictionary<string, WWW>();
	
	public bool isDownLoaded(string strName, int nVersion)
	{
		string nameLower = strName.ToLower();

		if( false == m_WWWs.ContainsKey( nameLower))
		{
			if( true == _isVersionCached( strName, nVersion))
				return true;
		}
		
		WWW www = _wwwCache( nameLower, nVersion);
		
		if( null == www)
			return false;
		
		if( null != www.error)
		{
			AssetbundleManager.Instance.OpenPatchErrorMsgBox();
			m_WWWs.Remove( nameLower);
			Debug.LogError( "Assetbundle Download Error: " + www.error + ", filename: " + nameLower);
			return false;
		}
		
		if( false == www.isDone)
			return false;
		
		if( null == www.assetBundle)
		{
			m_WWWs.Remove( nameLower);
			Debug.LogError( "null == www.assetBundle: " + nameLower);
			return false;
		}

		// ================ FT MODIFY
		//System.GC.Collect();
		www.assetBundle.Unload( false);
		m_WWWs.Remove( nameLower);

		//Debug.Log( "DownLoaded Assetbundle: " + strName + ", Version: " + nVersion);
		
		return true;
	}

	public GameObject GetGameObject(string strName, bool bClone)
	{
		Object o = _LoadAsset( strName, typeof(GameObject));

		if( true == bClone)
			return Object.Instantiate( o) as GameObject;
		
		return o as GameObject;
	}
	
	public Material GetMaterial(string strName, bool bClone)
	{
		Object o = _LoadAsset( strName, typeof(Material));

		if( true == bClone)
			return Object.Instantiate( o) as Material;
		
		return o as Material;
	}
	
	public Texture GetTexture(string strName, bool bClone)
	{
		Object o = _LoadAsset( strName, typeof(Texture));

		if( true == bClone)
			return Object.Instantiate( o) as Texture;
		
		return o as Texture;
	}
	
	public TextAsset GetTextAsset(string strName, bool bClone)
	{
		Object o = _LoadAsset( strName, typeof(TextAsset));

		if( true == bClone)
			return Object.Instantiate( o) as TextAsset;
		
		//return o as TextAsset;

		TextAsset textAsset = o as TextAsset;
		WWW www = _wwwCache( strName.ToLower(), AssetbundleManager.Instance.GetCachedVersion( strName));
		
		if( null != www)
		{
			www.assetBundle.Unload( false);
			m_WWWs.Remove( strName.ToLower());
			//System.GC.Collect();
			
//			Debug.Log( "AssetbundleLoader::GetTextAsset(), Remove: " + strName);
		}
		else
		{
			Debug.Log( "AssetbundleLoader::GetTextAsset(), null == www");
		}

		return textAsset;
	}
	
	public AudioClip GetAudioClip(string strName, bool bClone)
	{
		Object o = _LoadAsset( strName, typeof(AudioClip));

		if( true == bClone)
			return Object.Instantiate( o) as AudioClip;
		
		return o as AudioClip;
	}
	
	public float GetCurLoadingProgress(string strName)
	{
		string nameLower = strName.ToLower();
		
		if( true == m_WWWs.ContainsKey( nameLower))
			return m_WWWs[nameLower].progress;
		
		return 0.0f;
	}
	
	public WWW SceneAssetbundleLoadCache(string strSceneName)
	{
		string nameLower = strSceneName.ToLower();

		if( true == nameLower.Contains( "login"))
			return null;

		if( false == m_SceneWWWs.ContainsKey( nameLower))
		{
			string strPath = _GetAssetbundlePath( nameLower);
			
			if( 0 == strPath.Length)
				return null;

			WWW downloadCache = WWW.LoadFromCacheOrDownload( strPath, AssetbundleManager.Instance.GetCachedVersion( strSceneName));
			m_SceneWWWs.Add( nameLower, downloadCache);
		}
		
		return m_SceneWWWs[nameLower];
	}
	
	public bool isDownLoadedScene(string strSceneName)
	{
		string nameLower = strSceneName.ToLower();

		if( true == nameLower.Contains( "login"))
			return true;

		if( false == m_SceneWWWs.ContainsKey( nameLower))
		{
			Debug.LogError( "AssetbundleLoader::isDownLoadedScene: " + strSceneName);
			return false;
		}
		
		return m_SceneWWWs[nameLower].isDone;
	}
	
	public void ClearAssetbundle()
	{
//		System.GC.Collect();
//		//m_WWWs.Clear();
//		//m_ABRequest.Clear();
//		m_SceneWWWs.Clear();
//		Resources.UnloadUnusedAssets();
	}
	
	// < private
	private WWW _wwwCache(string strName, int nVersion)
	{
		string nameLower = strName.ToLower();
		
		if( false == m_WWWs.ContainsKey( nameLower))
		{
			string strPath = _GetAssetbundlePath( nameLower);

			if( 0 == strPath.Length)
				return null;
			
//			bool bCached = _isVersionCached( strName, nVersion);
//			Debug.Log( "_wwwCache(); " + strName + ", bCached: " + bCached + ", Version: " + nVersion);

			WWW downloadCache = WWW.LoadFromCacheOrDownload( strPath, nVersion);
			m_WWWs.Add( nameLower, downloadCache);
		}
		
		return m_WWWs[nameLower];
	}
	
	public bool _isVersionCached(string strName, int nVersion)
	{
		string strPath = _GetAssetbundlePath( strName.ToLower());
		if( true == Caching.IsVersionCached( strPath, nVersion))
		{
			//Debug.Log( "Caching true: " + strName.ToLower());
			return true;
		}

		//Debug.Log( "Caching false: " + strName.ToLower());
		return false;
	}
	
	public Object _LoadAsset(string strName, Type type)
	{
		string nameLower = strName.ToLower();
		WWW www = _wwwCache( nameLower, AssetbundleManager.Instance.GetCachedVersion( strName));
		
		if( null == www)
			Debug.LogError( "_LoadAsset(), null == www: " + strName);
		if( null == www.assetBundle)
			Debug.LogError( "_LoadAsset(), null == www.assetBundle: " + strName);
		
		AssetBundle bundle = www.assetBundle;
		
		if( null == bundle.mainAsset)
			Debug.LogError( "_LoadAsset(), null == bundle.mainAsset: " + strName);
		
		//return bundle.Load( nameLower);
		return bundle.mainAsset;
	}
	
	/*
	private static Dictionary<string, AssetBundleRequest> m_ABRequest = new Dictionary<string, AssetBundleRequest>();
	public Object _LoadAsset(string strName, Type type)
	{
		string nameLower = strName.ToLower();
		WWW www = _wwwCache( nameLower, AssetbundleManager.Instance.GetCachedVersion( strName));
		
		if( null == www)
			Debug.LogError( "_LoadAsset(), null == www: " + strName);
		if( null == www.assetBundle)
			Debug.LogError( "_LoadAsset(), null == www.assetBundle: " + strName);
		
		if( false == m_ABRequest.ContainsKey( nameLower))
			m_ABRequest.Add( nameLower, www.assetBundle.LoadAsync( nameLower, type));
		
		return (Object)(m_ABRequest[nameLower].asset);
	}
	*/
	
	private string _GetAssetbundlePath(string strFileNameLower)
	{
		return AssetbundleManager.Instance.GetAssetbundleDownloadPath( strFileNameLower);
	}
	// private >
}
