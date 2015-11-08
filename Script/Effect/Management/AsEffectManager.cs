using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public delegate void HitResultFunc( System.Object _obj);

public class AsEffectManager : MonoBehaviour
{
	static AsEffectManager m_instance;

	public static AsEffectManager Instance
	{
		get	{ return m_instance; }
	}
	
	private Dictionary<int, AsEffectEntity> m_EffectsList = new Dictionary<int, AsEffectEntity>();

	GameObject m_RootObject;
	
	void Awake()
	{
		if( m_instance == null)
		{
			m_instance = this;
			m_RootObject = new GameObject( "Effects");
			DontDestroyOnLoad( m_RootObject);
			DDOL_Tracer.RegisterDDOL( this, m_RootObject);//$ yde
		}
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	AsEffectEntity LoadEntity( string sourcePath)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_EffectShow))
			return null;
	
		GameObject obj = ResourceLoad.LoadGameObject( sourcePath);
		if( obj == null)
			return null;
		
		GameObject go = GameObject.Instantiate( obj) as GameObject;
		if( go == null)
			return null;
		
		AsEffectEntity entity =  go.AddComponent<AsEffectEntity>();
		
		int hash = entity.GetHashCode();
		StringBuilder sb = new StringBuilder();
		sb.Append( entity.name);
		sb.Append( "_key:");
		sb.Append( hash);
//		string entityname = entity.name + "_key:" + hash;
		entity.name = sb.ToString();
		entity.Id = hash;
		entity.EntityObject = go;
		entity.gameObject.layer = LayerMask.NameToLayer( "Effect");
//		entity.isStatic = true;
		AddEffectEntity( entity);
		
		return entity;
	}
	
	public int PlayEffect( string sourcePath, Vector3 _pos, bool bLoop = false, float fLifeTime = 0f, float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.EntityObject.transform.parent = m_RootObject.transform;
			entity.CreateEffect( effectData, _pos, bLoop, fLifeTime, fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	
	public int PlayEffect( string sourcePath, Transform _attach, bool bLoop = false, float fLifeTime = 0f, float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.EntityObject.transform.parent = m_RootObject.transform;
			entity.CreateEffect( effectData, _attach, bLoop, fLifeTime, fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	
	public int PlayEffect( string sourcePath, Transform _sourceTran ,Transform _targetTran, bool bLoop = false, float fLifeTime = 0f , float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.EntityObject.transform.parent = m_RootObject.transform;
			entity.CreateEffect( effectData, _sourceTran,_targetTran, bLoop, fLifeTime, fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	
	//$yde
	public int ShootingEffect( string sourcePath, Transform _startTran, Transform _targetTran,  HitResultFunc _HitResult, float _speed, float acce, eProjectilePath ePath , float fStartSize = 1f, string projectileHitSoundName = null, bool bHold = false)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.CreateEffect( effectData, _startTran, _targetTran, _HitResult, _speed, acce, ePath, fStartSize, projectileHitSoundName, bHold);
			return entity.Id;
		}
		
		return -1;
	}
	
	//$yde
	public int ShootingEffect( string sourcePath,  Transform _startTran, Vector3 _targetPos,  HitResultFunc _HitResult, float _speed, float acce, eProjectilePath ePath , float fStartSize = 1f, string projectileHitSoundName = null)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.CreateEffect( effectData, _startTran, _targetPos, _HitResult, _speed, acce, ePath, fStartSize, projectileHitSoundName );
			return entity.Id;
		}
		
		return -1;
	}
	
	public int GetEffectCount()
	{
		return m_EffectsList.Count;
	}
		
	public void SetSpeed( int _id, float _speed)
	{
		AsEffectEntity _entity = GetEffectEntity( _id);
		if( _entity != null)
			_entity.PlaybackSpeed = _speed;
	}
	
	public void StopEffect( int _id)
	{
		AsEffectEntity _entity = GetEffectEntity( _id);
		if( _entity != null)
			_entity.StopEffect();
	}
	
	public void SetAngleY( int _id, float _angle, bool bAngleReset)
	{
		AsEffectEntity _entity = GetEffectEntity( _id);
		if( _entity != null)
			_entity.SetAngleY( _angle, bAngleReset);
	}
	
	// ilmeda 20120516
	public void SetAngleY( int _id, float _angle)
	{
		SetAngleY( _id, _angle, false);
	}
	
	public void SetPosition( int _id, Vector3  _pos)
	{
		AsEffectEntity _entity = GetEffectEntity( _id);
		if( _entity != null)
			_entity.SetPosition( _pos);
	}
	
	public void SetSize( int _id, float _size)
	{
		AsEffectEntity _entity = GetEffectEntity( _id);
		if( _entity != null)
			_entity.SetSize( _size);
	}
	
	public bool AddEffectEntity( AsEffectEntity _entity)
	{
		if( !m_EffectsList.ContainsKey( _entity.Id))
		{
			m_EffectsList.Add( _entity.Id,  _entity);
			return true;
		}
		else
		{
			Debug.Log( "AddEffectEntity() Key Crash!!!");
			return false;
		}
	}
	
	public AsEffectEntity GetEffectEntity( int _id)
	{
		if( m_EffectsList.ContainsKey( _id) == true)
			return m_EffectsList[_id];
		else
			return null;
	}

	/*public void Remove_EffectList( AsEffectEntity _entity)
	{
		m_EffectsList.Remove( _entity.Id);		
	}
	*/
	public void RemoveEffectEntity( int _id)
	{
		AsEffectEntity entity = AsEffectManager.Instance.GetEffectEntity( _id);
		if( entity == null)
			return;
		
		m_EffectsList.Remove( _id);
		entity.Delete();
		Destroy( entity.gameObject);
	}
	
	public void RemoveAllEntities()
	{
		foreach( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
			{
				//entity.Delete();
				if( entity.EffectPrefab != null)
				{
					entity.EffectPrefab.RemoveDestroyImmediate();
					DestroyImmediate( entity.EffectPrefab);
				}
				DestroyImmediate( entity.gameObject);
			}
		}
		
		m_EffectsList.Clear();
	}
	
	public void StopAllEffect()
	{
		foreach ( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
				entity.StopEffect();
		}
	}
	
	#region -Entity AttachEffect	
	public bool AddAttachEffectUser( int id, uint charUniqKey)
	{
		AsEffectEntity entity = GetEffectEntity( id);
		if( entity == null)
			return false;
		
		entity.CharUniqKey = charUniqKey;
		return true;
	}
	
	public void SetSizeAttachEffect( uint charUniqKey, float size = 1f)
	{
		foreach( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
			{
				if( charUniqKey == entity.CharUniqKey)
					SetSize( pair.Key, size);
			}
		}
	}
	#endregion
	
	#region - SkillEditor -
	public void UpdateEffect()
	{
		foreach ( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
				entity.UpdateEffect();
		}
		
	}
	
	public bool IsPlayingAllEffect()
	{
		foreach ( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
			{
				if( entity.IsPlaying())
					return true;
			}
		}
		
		return false;
	}
	
	public bool PlayOnceAllEffect()
	{
		bool bRe = false;
		foreach ( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
			{
				entity.OnLoop( false);
				entity.PlayOnce();
				bRe = true;
			}
		}
		
		return bRe;
	}
	
	public void PlayAllEffect()
	{
		foreach ( KeyValuePair<int, AsEffectEntity> pair in m_EffectsList)
		{
			AsEffectEntity entity = pair.Value;
			if( entity != null)
				entity.Play();
		}
	}
	
	public int Tool_PlayEffect( string sourcePath, Vector3 _pos, bool bLoop, float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.Tool_CreateEffect( effectData, _pos, bLoop, fStartSize);
		}
		
		return -1;
	}
	
	public int Tool_PlayEffect( string sourcePath, Transform _attach, bool bLoop, float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.Tool_CreateEffect( effectData, _attach, bLoop,fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	
	public int Tool_PlayEffect( string sourcePath,  Transform _sourceTran, Transform _targetTran, float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.Tool_CreateEffect( effectData, _sourceTran, _targetTran, fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	
	public int Tool_ShootingEffect( string sourcePath, Vector3 _startPos, Transform _targetTran,  HitResultFunc _HitResult, float _speed, float acce, eProjectilePath ePath , float fStartSize = 1f)
	{
		AsEffectEntity entity = LoadEntity( sourcePath);
		if( entity != null)
		{
			AsEffectData effectData = entity.GetComponentInChildren<AsEffectData>();
			entity.CreateEffect( effectData, _startPos, _targetTran, _HitResult, _speed, acce, ePath, fStartSize);
			return entity.Id;
		}
		
		return -1;
	}
	#endregion
}
