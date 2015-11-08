using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



[Serializable]
public class AsEffectInfo 
{	
	

		
	[SerializeField] public GameObject m_effectObject;	
	[SerializeField] public Dummy_Type m_dummyType = Dummy_Type.DummyNone;
	[SerializeField] public float startTime = 0f;	
	[SerializeField] public float endTime = 0f;
	[SerializeField] public bool  indirectPos   = false;
	[SerializeField] public bool  randomAngleY  = false;
	[SerializeField] public bool  randomPos     = false;	
	[SerializeField] public string m_strBundlePath;
	
	
	[HideInInspector] public bool isEnable = false;
	[HideInInspector] public bool isInstantiate = false;
	[HideInInspector] public bool isPlaying = false;
	[HideInInspector] public ParticleSystem[]  m_ParticleSystem;
	[HideInInspector] public Animation[] m_Animation;
	[HideInInspector] public ParticleEmitter[] m_Emitter;
	[HideInInspector] public GameObject m_effectInstance;	
	[HideInInspector] public AsMeshEffect  m_AsMeshEffect;	
	[HideInInspector] public AsTrailEffect  m_AsTrailEffect;	
}

[AddComponentMenu( "ArkSphere/EffetData")]
[ExecuteInEditMode]
public class AsEffectData : MonoBehaviour 
{

	float m_pastTime = 0f;
	public List<AsEffectInfo> m_effectLayer = new List<AsEffectInfo>();	
	
	private List<GameObject> m_removeList = new List<GameObject>();
	Transform m_transform = null;
	Transform m_targetTran = null;
	

	bool m_bLoop = false;
	public bool Loop
	{
		get{return m_bLoop;}		
	}
	
	bool m_bEnalbe = false; 
    public bool Enalbe
    {
        get { return m_bEnalbe; }
        set { m_bEnalbe = value; }
    }
	
	bool m_bAutoDel = false;
	 public bool AutoDel
    {
        get { return m_bAutoDel; }
        set { m_bAutoDel = value; }
    }
	
	float m_StartSize  = 1f;
	public float StartSize
    {
        get { return m_StartSize; }
        set { m_StartSize = value; SetStartSize();  }
    }
	
	AsEffectEntity m_ParentObject = null;
	 public AsEffectEntity ParentObject
    {
        get { return m_ParentObject; }
        set { m_ParentObject = value;  }
    }
	
		
	void Start()
	{
		//ReSet();		
		
	}
	public bool IsPlaying()
    {
		bool bPlaying = false;
		for( int index = 0; index < m_effectLayer.Count; ++index )			
		{
			AsEffectInfo data = m_effectLayer[index];
			if(data.isPlaying)
			{
				bPlaying = true;
				break;
			}
			
		}
		return bPlaying;
	
	}	
	
	 	

	public void RemoveInstantiate()
	{
		m_bEnalbe = false;
		SetVisible();
		for( int i = 0; i < m_removeList.Count; ++i )				// remove particle object
		{
			GameObject go = m_removeList[i];
			//DestroyImmediate( go ); 
			if(go) Destroy( go ); 
		}
	}
	
	public void RemoveDestroyImmediate()
	{
		m_bEnalbe = false;
		SetVisible();
		for( int i = 0; i < m_removeList.Count; ++i )				// remove particle object
		{
			GameObject go = m_removeList[i];
			//DestroyImmediate( go ); 
			if(go) DestroyImmediate( go ); 
		}
	}
	public void OnLoop(bool bLoop)
	{
		m_bLoop = bLoop;
		if(bLoop)
		{
			for( int index = 0; index < m_effectLayer.Count; ++index )			
			{
				AsEffectInfo data = m_effectLayer[index];
				data.isEnable  = true;
			}			
		}
		//m_bLoop = true;		
	}
	
	void ReSet()
	{
		m_pastTime = 0f;
		m_bLoop = false;		
		m_bEnalbe = true;	
		for( int index = 0; index < m_effectLayer.Count; ++index )			
		{
			AsEffectInfo data = m_effectLayer[index];
			data.isPlaying = false;
			data.isEnable  = true;
		}
	}
	
	
	
	public void Enable( bool bLoop , float fStartSize = 1f)
	{
		ReSet();
		m_bLoop = bLoop;	
		m_StartSize	 = fStartSize;
		
	}
	
	public void Enable( Vector3 _pos, bool bLoop, float fStartSize = 1f)
	{
		ReSet();	
		m_bLoop = bLoop;		
		m_StartSize = fStartSize;
	}
	
	public void Enable( Transform _tra, bool bLoop, float fStartSize = 1f )
	{
		ReSet();	
		m_bLoop = bLoop;
		
		
		m_transform  = _tra;
		m_StartSize	 = fStartSize;
	
	}
	
	public void Enable( Transform _sourceTran, Transform _targetTran, bool bLoop, float fStartSize = 1f )
	{
		ReSet();	
		m_bLoop = bLoop;		
		m_transform  = _sourceTran;
		m_targetTran = _targetTran;
		
		m_StartSize	 = fStartSize;
	
	}
	
	public void Enable( Vector3 _pos, Transform _tra, bool bLoop, float fStartSize = 1f)
	{
		ReSet();	
		m_bLoop = bLoop;		
		m_transform     = _tra;
		m_StartSize	 = fStartSize;
	}	
	
	public void LoadComponents(AsEffectInfo data, GameObject go)
	{      
		data.m_Emitter = go.GetComponentsInChildren<ParticleEmitter>();		
		data.m_ParticleSystem = go.GetComponentsInChildren<ParticleSystem>();			
		data.m_Animation = go.GetComponentsInChildren<Animation>(); 
		data.m_AsMeshEffect = go.GetComponentInChildren<AsMeshEffect>(); 
		data.m_AsTrailEffect = go.GetComponentInChildren<AsTrailEffect>(); 	
		
	}
	
	public void SetSize( float _size )
	{	
		m_StartSize = _size;	
		SetStartSize();	
	}
	
	void SetStartSize()
	{
		for( int index = 0; index < m_effectLayer.Count; ++index )				// startTime check
		{		
			AsEffectInfo data = m_effectLayer[index];
			SetStartSize(data);
		}
	}
	
	void SetStartSize(AsEffectInfo data)
	{
		if(m_StartSize > 0.0f)
		{			
			for (int i = 0; i < data.m_ParticleSystem.Length; ++i)
			{
				if(null != data.m_ParticleSystem[i]  )
					data.m_ParticleSystem[i].startSize = data.m_ParticleSystem[i].startSize  * m_StartSize;
			}	
		
			if(null != data.m_AsMeshEffect  )
			{
				 data.m_AsMeshEffect.StartSize =  m_StartSize;
			}
			
		}

	}
	
	void SetIndirectPos(GameObject go)
	{	
	//	Vector3 pos =  go.transform;
		SetParent(go);	
	//	go.transform = tran;
	}
	
	void SetRandomAngleY(GameObject go)
	{
		float angle = go.transform.rotation.y;
		angle += UnityEngine.Random.Range(0f, 30F);
		go.transform.Rotate(new Vector3(0, angle, 0)); 
	}
	
	void SetRandomPos(GameObject go)
	{	
		Vector3 pos = go.transform.localPosition;
		pos.x += UnityEngine.Random.Range(-2f, 2F);
		pos.z += UnityEngine.Random.Range(-2f, 2F);
		go.transform.localPosition = pos;
	}
	
	
	void SetParent(GameObject go)
	{
		 go.transform.parent = m_ParentObject.EntityObject.transform;
		 go.transform.localPosition = go.transform.position;
        
         go.transform.localRotation =  go.transform.rotation;
		
		Vector3 vecScale = Vector3.one;
		vecScale.x = go.transform.localScale.x * m_StartSize;
		vecScale.y = go.transform.localScale.y * m_StartSize;
		vecScale.z = go.transform.localScale.z * m_StartSize;
		go.transform.localScale = vecScale;
	
	}	
	
	void SetAttach(AsEffectInfo data)
	{
		Transform attach = null;
		if(data.m_dummyType != Dummy_Type.DummyNone)
		{			
			if(m_transform != null)
	            attach = ResourceLoad.SearchHierarchyTransform(m_transform, data.m_dummyType.ToString() );
			
			if(attach != null)
			{						
				data.m_effectInstance.transform.parent =  attach;
				data.m_effectInstance.transform.localPosition = data.m_effectObject.gameObject.transform.position + m_ParentObject.EntityObject.transform.position;
				data.m_effectInstance.transform.localRotation = data.m_effectInstance.transform.rotation;		
				Vector3 vecScale = Vector3.one;
				vecScale.x = data.m_effectInstance.transform.localScale.x * m_StartSize;
				vecScale.y = data.m_effectInstance.transform.localScale.y * m_StartSize;
				vecScale.z = data.m_effectInstance.transform.localScale.z * m_StartSize;
				data.m_effectInstance.transform.localScale = vecScale;
			}
			else
	        {
	           SetParent(data.m_effectInstance);
	        }
			
		
		}
		else
		{
			SetParent(data.m_effectInstance);
		}
	
					
		if( !data.m_effectInstance )
		{
			Debug.Log("AsEffectData GameObject Null!!! "+ data.m_effectObject.name);		
		}
		
		if(data.indirectPos) SetIndirectPos(data.m_effectInstance); 
		if(data.randomAngleY) SetRandomAngleY(data.m_effectInstance);
		if(data.randomPos) SetRandomPos(data.m_effectInstance);
		
		LoadComponents(data,data.m_effectInstance);
		m_removeList.Add( data.m_effectInstance );
		data.isInstantiate = true;	
		
		SetStartSize(data);
		/////////////////////////////////////////////////////////////////////
		
		AsLaser _Laser = data.m_effectInstance.GetComponentInChildren<AsLaser>();
		if(_Laser) 
		{			
			Transform targetAttach = null;
			targetAttach = ResourceLoad.SearchHierarchyTransform (m_targetTran, "DummyCharacterCenter");
			if(targetAttach != null)	_Laser.m_target = targetAttach;
			else 	_Laser.m_target = m_targetTran;
			if(attach != null)			_Laser.m_start  = attach;
			else  	_Laser.m_start  = m_transform;
		}
		/////////////////////////////////////////////////////////////////////
	}	
		
	// Update is called once per frame
	void Update()  
	{
		int  nDeleteCount = 0;
		bool bIsPlaying = false;
		bool bIsInstantiate = false;
		m_pastTime += Time.deltaTime; 
	
		for( int index = 0; index < m_effectLayer.Count; ++index )				// startTime check
		{
		
			AsEffectInfo data = m_effectLayer[index];
			if(!bIsInstantiate)
			bIsInstantiate = data.isInstantiate;
			
			if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
			{
				if(data.m_effectObject == null)
					data.m_effectObject = ResourceLoad.LoadGameObject( data.m_strBundlePath);
			}
			
			if(data.m_effectObject == null  ) 
			{
				Debug.Log("Effect Layer Element Empty!!![" + m_ParentObject.name +"]");
				AsEffectManager.Instance.RemoveEffectEntity( m_ParentObject.Id );
				continue;
			}
			if(!data.isEnable ) continue;
			
			float startTime = 0.0f, endTime = 0.0f;
			 if(m_ParentObject.PlaybackSpeed != 0.0f)
			 {
				startTime = ( data.startTime / m_ParentObject.PlaybackSpeed);
				endTime   = ( data.endTime / m_ParentObject.PlaybackSpeed);
			 }
			 else
			 {
				startTime = data.startTime;
				endTime  = data.endTime;
			 }
			
			if( startTime <= m_pastTime )
			{
				if(!data.isInstantiate)
				{										
					data.m_effectInstance = GameObject.Instantiate( data.m_effectObject ) as GameObject;
					
					SetAttach(data);
				}				
				if(!data.isPlaying)	Play(data);						
			}
			else
			{
				bIsPlaying = true;
			}
			
			if(data.isInstantiate  ) 
			{
				if( data.m_effectInstance)
				{
					if( m_bLoop)
						PlayLooping(data);
					else
					{
						if( endTime != 0 &&  endTime <= m_pastTime  )						// endTime check
							Stop(data);			
					}
				
					if(  m_ParentObject != null )
					{
						if( IsPlaying(data)) 
						{
							bIsPlaying = true;
						}
					}
				}
				else
				{
					nDeleteCount++;
				}
			}
			
				
			
		}
	
		////////////////////////////delete///////////////////////////////////////////
		if(m_effectLayer.Count == nDeleteCount)	
		{
			AsEffectManager.Instance.RemoveEffectEntity( m_ParentObject.Id );
			return;
		}
		if(AutoDel)
		{
			if(!m_bLoop && !bIsPlaying && bIsInstantiate)
			{
				AsEffectManager.Instance.RemoveEffectEntity( m_ParentObject.Id );
				return;
			}
		}
		///////////////////////////////////////////////////////////////////////////
		
		
	}
	
	public void SetVisible()
    {
		if(m_bEnalbe == false) return;
		for( int index = 0; index < m_effectLayer.Count; ++index )				// startTime check
		{			
			AsEffectInfo data = m_effectLayer[index];
			if(data.m_effectInstance)
			{
	     		data.m_effectInstance.SetActiveRecursively(false);		    
				
			}
			data.isPlaying = false;
			data.isEnable = false;
		}		
		
    }
	
	void Stop(AsEffectInfo data)
    {
		if(m_bEnalbe == false) return;
        for (int i = 0; i < data.m_ParticleSystem.Length; ++i)
		{
            data.m_ParticleSystem[i].Stop();			
		}
		
		for (int i = 0; i < data.m_Animation.Length; ++i)
		{
			if(data.m_Animation[i] != null)
			{
				data.m_Animation[i].Stop();  
            	data.m_Animation[i].Rewind();
			}
			else
			{
				Debug.LogError("Effect Data Animation Not found(NULL)");
			}
		}
		
		for (int i = 0; i < data.m_Emitter.Length; ++i)
		{
            data.m_Emitter[i].ClearParticles();
            data.m_Emitter[i].emit = false;			
		}
	
		if(data.m_AsMeshEffect != null && data.m_AsMeshEffect.m_Play)
			 data.m_AsMeshEffect.m_Play = false;
		
		if(data.m_AsTrailEffect != null)
			data.m_AsTrailEffect.Stop( );
		
		
		
		data.m_effectInstance.SetActiveRecursively(false);		
		data.isPlaying = false;
		data.isEnable = false;

		
    }

    void Play(AsEffectInfo data)
    {
		if(m_bEnalbe == false) return;
		data.m_effectInstance.SetActiveRecursively(true);		
        for (int i = 0; i < data.m_ParticleSystem.Length; ++i)
		{
           data.m_ParticleSystem[i].Play();		
		   data.m_ParticleSystem[i].playbackSpeed = m_ParentObject.PlaybackSpeed ;
		}
		
		for (int i = 0; i < data.m_Animation.Length; ++i)
		{
			if(data.m_Animation[i] == null) 
			{
				Debug.LogError("Effect Data Animation Not found(NULL)");
				continue;
			}		
			Animation ani = data.m_Animation[i];
			if(null != data.m_Animation[i].clip)
			{
				AnimationState ani_state = ani[data.m_Animation[i].clip.name];		
				if(null != ani_state)
					ani_state.speed = m_ParentObject.PlaybackSpeed;
			}
			if(!data.m_Animation[i].isPlaying)
				data.m_Animation[i].Play();	
		}
			
		for (int i = 0; i < data.m_Emitter.Length; ++i)
		{
			data.m_Emitter[i].Emit();					
		}
		
		if(data.m_AsMeshEffect != null )
			 data.m_AsMeshEffect.m_Play = true;
		
		if(data.m_AsTrailEffect != null)
			data.m_AsTrailEffect.Play( m_transform, m_ParentObject.PlaybackSpeed );
		
		data.isPlaying = true;

		
    }
	
	bool IsPlaying(AsEffectInfo data)
	{
		if(m_bEnalbe == false) return false;
		bool bIsPlaying = false;
		for (int i = 0; i < data.m_ParticleSystem.Length; ++i)
		{
			if(data.m_ParticleSystem[i])
			{
				if(data.m_ParticleSystem[i].isPlaying)
					bIsPlaying = true;
			}
		}
		
		for (int i = 0; i < data.m_Animation.Length; ++i)
		{
			if(data.m_Animation[i] != null) 
			{
				if(data.m_Animation[i].isPlaying)
					bIsPlaying  = true; 
			}
		}	
			
		for (int i = 0; i < data.m_Emitter.Length; ++i)
		{
			if(data.m_Emitter[i] && data.m_Emitter[i].particleCount != 0  )
				 bIsPlaying = true;
		}
		
		if(data.m_AsMeshEffect != null && data.m_AsMeshEffect.m_Play)
			 bIsPlaying = true;
		return bIsPlaying;
	}
	
	void PlayLooping(AsEffectInfo data)
	{
		if(m_bEnalbe == false) return;
		for (int i = 0; i < data.m_ParticleSystem.Length; ++i)
		{
			if(data.m_ParticleSystem[i])
			{
				if(!data.m_ParticleSystem[i].isPlaying)
					data.m_ParticleSystem[i].Play();				
			}
		}
				
		
		for (int i = 0; i < data.m_Emitter.Length; ++i)
		{
			if(data.m_Emitter[i] && data.m_Emitter[i].particleCount == 0  )
				data.m_Emitter[i].Emit();
		}
		
		if(data.m_AsMeshEffect != null && data.m_AsMeshEffect.m_Play)
			 data.m_AsMeshEffect.m_Play = true;
		
	}
	
	
	public void StopEffect()
    {
		for( int index = 0; index < m_effectLayer.Count; ++index )			
		{
			AsEffectInfo data = m_effectLayer[index];
			Stop(data);
		}
	}
	
	public void PlayEffect()
    {
		for( int index = 0; index < m_effectLayer.Count; ++index )			
		{
			AsEffectInfo data = m_effectLayer[index];
			Play(data);
		}
	}
	
	public void UpdateEffect(  )
	{
		m_pastTime = 0f;
		for( int index = 0; index < m_effectLayer.Count; ++index )				
		{
			AsEffectInfo data = m_effectLayer[index];
			data.isPlaying = false;
			data.isInstantiate = false;
		}
		
	}
}