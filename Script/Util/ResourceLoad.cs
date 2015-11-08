using UnityEngine;
using System.Collections;

public class ResourceLoad
{
	public static GameObject CreateGameObject( string strPath, Transform _trn)
	{
		if( null== _trn)
		{
			AsUtil.ShutDown( "ResourceLoad::CreateGameObject()[ transfrom == null ] path : " + strPath);
			return null;
		}

		GameObject goRes = LoadGameObject( strPath);
		if( null == goRes)
		{
			AsUtil.ShutDown( "ResourceLoad::CreateGameObject()[ goRes == null ] path : " + strPath);
			return null;
		}

		GameObject goIns = GameObject.Instantiate( goRes) as GameObject;

		goIns.transform.parent = _trn;
		goIns.transform.localPosition = Vector3.zero;
		goIns.transform.localRotation = Quaternion.identity;
		goIns.transform.localScale = Vector3.one;

		return goIns;
	}

	public static GameObject CreateUI( string strPath, Transform _trn, Vector3 _pos)
	{
		if( null== _trn)
		{
			AsUtil.ShutDown( "ResourceLoad::CreateGameObject()[ transfrom == null ] path : " + strPath);
			return null;
		}

		GameObject goRes = LoadGameObject( strPath);
		if( null == goRes)
		{
			AsUtil.ShutDown( "ResourceLoad::CreateGameObject()[ goRes == null ] path : " + strPath);
			return null;
		}

		GameObject goIns = GameObject.Instantiate( goRes) as GameObject;

		goIns.transform.parent = _trn;
		goIns.transform.localPosition = _pos;
		goIns.transform.localRotation = Quaternion.identity;
		goIns.transform.localScale = Vector3.one;

		return goIns;
	}

	public static GameObject CreateGameObject( string strPath)
	{
		GameObject goRes = LoadGameObject( strPath);
		if( null == goRes)
		{
			AsUtil.ShutDown( "ResourceLoad::CreateGameObject()[ goRes == null ] path : " + strPath);
			return null;
		}

		GameObject goIns = GameObject.Instantiate( goRes) as GameObject;

		return goIns;
	}

	static public UISlotItem CreateItemIcon( GameObject goItemIcon, SimpleSprite iconImgPos, Vector3 pos, Vector2 _size, bool isNeedCollider = true)
	{
		if( null == goItemIcon)
			return null;

		GameObject obj = GameObject.Instantiate( goItemIcon) as GameObject;
		UISlotItem _SlotItem = obj.GetComponent<UISlotItem>();
		if( null != _SlotItem)
		{
			_SlotItem.transform.parent = iconImgPos.transform;
			_SlotItem.transform.localPosition = pos;
			_SlotItem.transform.localRotation = Quaternion.identity;
			_SlotItem.transform.localScale = Vector3.one;
			_SlotItem.iconImg.width = iconImgPos.width - _size.x;
			_SlotItem.iconImg.height = iconImgPos.height - _size.y;
			if( true == isNeedCollider)
				_SlotItem.iconImg.gameObject.AddComponent<BoxCollider>();
		}

		return _SlotItem;
	}


	// Load Texture
	public static Texture Loadtexture( string strPath)
	{
		if( null == strPath || 0 == strPath.Length)
			return null;

		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == strPath.Contains( "Character/") || true == strPath.Contains( "UIPatchResources/"))
				return AssetbundleManager.Instance.GetAssetbundleTexture( strPath);
			else
				return Resources.Load( strPath) as Texture;
		}
		else
			return Resources.Load( strPath) as Texture;
	}


	// Load GameObject
	public static GameObject LoadGameObject( string strPath)
	{
		if( null == strPath || 0 == strPath.Length)
			return null;

		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == strPath.Contains( "Character/"))
				return AssetbundleManager.Instance.GetAssetbundleGameObject( strPath);
//			else if( true == strPath.Contains( "FX/Effect_Resource/"))
//				return AssetbundleManager.Instance.GetAssetbundleGameObject( strPath);
			else
				return Resources.Load( strPath) as GameObject;
		}
		else
			return Resources.Load( strPath) as GameObject;
	}

	public static TextAsset LoadTextAsset( string strPath)
	{
		if( null == strPath || 0 == strPath.Length)
			return null;

		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == strPath.Contains( "Table/"))
				return AssetbundleManager.Instance.GetAssetbundleTextAsset( strPath);
			else
				return Resources.Load( strPath) as TextAsset;
		}
		else
			return Resources.Load( strPath) as TextAsset;
	}

	public static AudioClip LoadAudioClip( string strPath, bool bLoadDirect = false)
	{
		if( null == strPath || 0 == strPath.Length)
			return null;

		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle && false == bLoadDirect)
		{
//			if( true == strPath.Contains( "Sound/"))
				return AssetbundleManager.Instance.GetAssetbundleAudioClip( strPath);
//			else
//				return Resources.Load( strPath) as AudioClip;
		}
		else
			return Resources.Load( strPath) as AudioClip;
	}

	public static Transform SearchHierarchyTransform( Transform _trn, string _name)
	{
		if( _trn.name == _name)
			return _trn;

		for( int i = 0; i < _trn.GetChildCount(); ++i)
		{
			Transform child = SearchHierarchyTransform( _trn.GetChild( i), _name);
	
			if( child != null)
				return child;
		}

		return null;
	}

	public static void SetActive( Transform _trn, bool bActive)
	{
		if( null == _trn)
			return;

		_trn.gameObject.active = bActive;

		for( int i = 0; i < _trn.GetChildCount(); ++i)
		{
			SetActive( _trn.GetChild( i), bActive);
		}
	}

	public static void SetLayerHierArchy( Transform _trn, int _layer)
	{
		_trn.gameObject.layer = _layer;

		for( int i=0; i<_trn.GetChildCount(); ++i)
		{
			SetLayerHierArchy( _trn.GetChild( i), _layer);
		}
	}

	public static GameObject CreateSkillIcon( int iSkillId)
	{
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( iSkillId);
		if( null == record)
		{
			Debug.LogError( "ResourceLoad::CreateSkillIcon() [ null == record ] skill id : " + iSkillId);
			return null;
		}

		GameObject obj = Resources.Load( record.Skill_Icon) as GameObject;
		if( null == obj)
		{
			Debug.LogError( "AsSlot::CreateSkillIcon() [ null == load fail] path : " +  record.Skill_Icon);
			return null;
		}

		GameObject icon = GameObject.Instantiate( obj) as GameObject;
		icon.transform.localPosition = Vector3.zero;
		icon.transform.localRotation = Quaternion.identity;
		icon.transform.localScale = Vector3.one;

		return icon;
	}
}
