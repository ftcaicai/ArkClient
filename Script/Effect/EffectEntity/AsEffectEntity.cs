using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class AsEffectEntity : MonoBehaviour {	
	
	public enum eEffect_Type
    {
        PositionEffect = 0,
        AttachEffect,
        ShootingEffect,
		LaserBimEffect,
    }
		
    eEffect_Type m_EffectType;
	public eEffect_Type EffectType
	{
		get{return m_EffectType;}	
		set{m_EffectType = value;}
	}
	
	eProjectilePath m_eProjectlie_Path;
	AsEffectData m_EffectPrefab;
	public AsEffectData EffectPrefab//Effect Prefab
	{
		get{return m_EffectPrefab;}	
		set{m_EffectPrefab = value;}
	}
	GameObject m_EntityObject;
	public GameObject EntityObject{
		get{return m_EntityObject;}
		set{m_EntityObject = value;}
	}
	int m_Id = 0;
	public int Id{
		get{return m_Id;}
		set{m_Id = value;}
	}
	float m_pastTime = 0f;
	float m_fLifeTime = 0f;
	
	float m_fPlaybackSpeed = 1.0f;
	public float PlaybackSpeed{
		get{return m_fPlaybackSpeed;}
		set{ m_fPlaybackSpeed = value;}
	}
		
	bool m_bLoop = false;
	
	private uint m_nCharUniqKey;
		//dopamin #17440
	public uint CharUniqKey
	{
		get{ return m_nCharUniqKey; }
		set{ m_nCharUniqKey = value;}
	}
		
		
	public bool Loop	{ get { return m_bLoop; } 	}
  
	public void OnLoop(bool bLoop)
	{
		m_bLoop = bLoop;
		if(m_EffectPrefab != null)
		{
			m_EffectPrefab.OnLoop(bLoop);
		}
	}	
	
	
	float m_shootDelTime = 0f;
	bool  m_IsHit		  = false;
	bool  m_IsShootDelete = false;
	bool  m_IsHold   	  = false;
	HitResultFunc m_HitResult;
	string m_ProjectileHitSoundName = null;
    Transform m_targetTran;
	Vector3 m_target_position = Vector3.zero;
	Vector3 m_startPos = Vector3.zero;

    float m_fSpeed = 8.0f;
	float m_acce   = 0.0f;
	float m_fHeight= 2.0f;
	
	public float Speed{
		get{return m_fSpeed;}
		set{ m_fSpeed = value;}
	}
	
	public float Acce_Speed{
		get{return m_acce;}
		set{ m_acce = value;}
	}
	void InitTime(float fLifeTime)
	{
		m_pastTime = 0f;
		m_fLifeTime = fLifeTime;
	}
	bool InitEffectPrefab(AsEffectData effectData,float fLifeTime )
	{
		InitTime(fLifeTime);
		m_EffectPrefab = effectData;
		if(m_EffectPrefab == null)
		{
			Debug.Log("InitEffectPrefab(EffectDataComponent) Not found Resource :" );    
			return false;
		}	
		
		m_EffectPrefab.AutoDel = true;
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity>();
		return true;
	}
	public bool CreateEffect(AsEffectData effectData, Vector3 _pos, bool bLoop, float fLifeTime, float fStartSize = 1f )
	{
		if(!InitEffectPrefab(effectData,fLifeTime)) return false;
		m_EffectType = eEffect_Type.PositionEffect;
		transform.position = _pos;
		m_EffectPrefab.Enable( _pos, bLoop,fStartSize);			
		
		
		return true;
	}
	public bool CreateEffect(AsEffectData effectData,Transform _sourceTran ,Transform _targetTran, bool bLoop, float fLifeTime, float fStartSize = 1f )
	{
		if(!InitEffectPrefab(effectData,fLifeTime)) return false;
		m_EffectType = eEffect_Type.LaserBimEffect;	
		m_EffectPrefab.Enable( _sourceTran,_targetTran, bLoop,fStartSize);			
		return true;
	}
	/*public bool CreateEffect(AsEffectData effectData, Vector3 _pos,  Transform _tra, bool bLoop, float fLifeTime )
	{
		if(!InitEffectPrefab(effectData)) return false;
		transform.position = _pos;
		m_EffectPrefab.Enable( _pos,  _tra, bLoop);			
	
		
		return true;
	}
	*/
	public bool CreateEffect(AsEffectData effectData, Transform _tra, bool bLoop, float fLifeTime, float fStartSize = 1f  )
	{	
		if(!InitEffectPrefab(effectData,fLifeTime)) return false;
		m_EffectType = eEffect_Type.AttachEffect;

		m_EffectPrefab.Enable( _tra, bLoop,fStartSize);	

		return true;
	}
	
	
	//$yde	shooting TargetTransForm
	public bool CreateEffect (AsEffectData effectData, Transform _startTran, Transform _targetTran,  HitResultFunc _HitResult , float _speed, float _acce, eProjectilePath ePath, float fStartSize = 1f, string projectileHitSoundName = null, bool bHold =false)
	{
		m_pastTime = 0f;
		
		if( null == effectData)
		{
			Debug.Log ("CreateEffect(Shooting) Not found Resource :");
			return false;
		}
		
		if(ePath == eProjectilePath.NONE)return false;
		
		m_EffectType = eEffect_Type.ShootingEffect;
		
		m_ProjectileHitSoundName = projectileHitSoundName;
//		m_startPos   = transform.position  = _startPos;
		if (_targetTran != null) 	
		{
			m_targetTran = ResourceLoad.SearchHierarchyTransform (_targetTran, "DummyCharacterCenter");
			//m_fDistance = Vector3.Distance (m_targetTran.position, transform.position);
			if(m_targetTran == null)
			{
				m_targetTran = _targetTran;
			}
		}
		
		m_EffectPrefab = effectData;
		m_IsShootDelete =  false;
		m_IsHit         =  false;
		m_IsHold        =  bHold;
		if(m_IsHold) m_target_position = m_targetTran.position;	
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity> ();
		Transform trn = null;
		foreach(AsEffectInfo info in m_EffectPrefab.m_effectLayer)
		{
			trn = ResourceLoad.SearchHierarchyTransform(_startTran, info.m_dummyType.ToString() );
			Debug.Log(info.m_dummyType.ToString());
		}
		
		if(trn != null)
		{
			m_startPos   = transform.position  = trn.position;
		}
		else
		{
			Vector3 pos = _startTran.position; pos.y += 0.5f;
			m_startPos = pos;

			Debug.LogWarning("AsEffectEntity::CreateEffect: no transform is found");
		}
		
		m_fHeight = Vector3.Distance (m_startPos, m_targetTran.position ) * 0.25f;	
			
		m_EffectPrefab.Enable(false,fStartSize);
	
	    m_HitResult = _HitResult;
		if(_speed != 0f)	m_fSpeed = _speed;
		m_acce = _acce; 
	
		m_eProjectlie_Path = ePath;
		return true;
	}
	
	//shooting TargetPostion
	public bool CreateEffect (AsEffectData effectData, Transform _startTran, Vector3 _targetPos,  HitResultFunc _HitResult , float _speed, float _acce, eProjectilePath ePath, float fStartSize = 1f, string projectileHitSoundName = null )
	{
		m_pastTime = 0f;
		
		if( null == effectData)
		{
			Debug.Log ("CreateEffect(Shooting) Not found Resource :");
			return false;
		}
		
		if(ePath == eProjectilePath.NONE)return false;
		
		m_EffectType = eEffect_Type.ShootingEffect;
		
		m_ProjectileHitSoundName = projectileHitSoundName;
//		m_startPos   = transform.position  = _startPos;
		
		
		m_targetTran    =  null;
		m_IsHold        =  true;			
		
		m_EffectPrefab = effectData;
		m_IsShootDelete =  false;
		m_IsHit         =  false;
	
		if(m_IsHold) m_target_position = _targetPos;	
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity> ();
		Transform trn = null;
		foreach(AsEffectInfo info in m_EffectPrefab.m_effectLayer)
		{
			trn = ResourceLoad.SearchHierarchyTransform(_startTran, info.m_dummyType.ToString() );
			Debug.Log(info.m_dummyType.ToString());
		}
		
		if(trn != null)
		{
			m_startPos   = transform.position  = trn.position;
		}
		else
			Debug.LogError("AsEffectEntity::CreateEffect: no transform is found");
	
		m_fHeight = Vector3.Distance (m_startPos, m_target_position ) * 0.25f;	
			
		m_EffectPrefab.Enable(false,fStartSize);
	
	    m_HitResult = _HitResult;
		if(_speed != 0f)	m_fSpeed = _speed;
		m_acce = _acce; 
	
		m_eProjectlie_Path = ePath;
		return true;
	}
	
	//tool
	public bool CreateEffect (AsEffectData effectData, Vector3 _startPos, Transform _targetTran,  HitResultFunc _HitResult , float _speed, float _acce, eProjectilePath ePath, float fStartSize = 1f)
	{
		m_pastTime = 0f;
		
		if( null == effectData)
		{
			Debug.Log ("CreateEffect(Shooting) Not found Resource :");
			return false;
		}
		
		if(ePath == eProjectilePath.NONE)return false;
		
		m_EffectType = eEffect_Type.ShootingEffect;
		m_startPos   = transform.position  = _startPos;
		if (_targetTran != null) 	
		{
			m_targetTran = ResourceLoad.SearchHierarchyTransform (_targetTran, "DummyCharacterCenter");
			//m_fDistance = Vector3.Distance (m_targetTran.position, transform.position);
			if(m_targetTran == null)
			{
				m_targetTran = _targetTran;
			}
		}
		
		
	
		
		m_EffectPrefab = effectData;
		m_IsShootDelete =  false;
		m_IsHit         =  false;
	
		m_fHeight = Vector3.Distance (m_startPos, m_targetTran.position ) * 0.25f;	
		
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity> ();
		m_EffectPrefab.Enable (false,fStartSize);
	
	    m_HitResult = _HitResult;
		if(_speed != 0f)	m_fSpeed = _speed;
		m_acce = _acce; 
			
		m_eProjectlie_Path = ePath;
		return true;
	}
	
	
	public void ChangeAttach(Transform _tra)
	{
		EffectPrefab.transform.parent = _tra;
	}
	public void SetPosition( Vector3  _pos )
	{
		EntityObject.transform.position = _pos;  
	}
	public void SetAngleY( float angle, bool bAngleReset )
	{
		if( true == bAngleReset)
			EntityObject.transform.rotation = Quaternion.identity;
		
		EntityObject.transform.Rotate(new Vector3(0, angle, 0));  
	}
	
	// ilmeda 20120516
	public void SetAngleY( float angle)
	{
		SetAngleY( angle, false);
	}
	
/*	public void SetDecalSize( float _size )
	{
		EffectPrefab.SetDecalSize(_size);		
	}
 */
	public void SetSize( float _size )
	{
		EffectPrefab.SetSize(_size);		
	}
	
	public void Delete()
	{		
		if(EffectPrefab != null)
		{
			EffectPrefab.RemoveInstantiate();
			Destroy(EffectPrefab);			
			//DestroyImmediate(EffectPrefab);			
		}
		
	
	}
	// Use this for initialization
	void Start () {
	
	}
	
	
	void Update_ShootinEffect()
	{
		Vector3 lineDirection = Vector3.zero;	
		float fDist = 0f;
		if(!EffectPrefab.IsPlaying())return;
		if(m_targetTran == null && !m_IsHold )
		{	
			AsEffectManager.Instance.RemoveEffectEntity(m_Id);
			return;
		}		
		Vector3 pos = Vector3.zero;			
		
		switch(m_eProjectlie_Path)
		{	
			case eProjectilePath.Arc:			
			{
				if (!m_IsHit )
				{		
					Vector3 vecPosition, vecTarget_position;
					if(!m_IsHold)
						vecTarget_position = m_target_position = m_targetTran.position;	
					else
						vecTarget_position = m_target_position;
				
					vecTarget_position.y = 0;
					vecPosition = gameObject.transform.position;
					vecPosition.y = 0;
				
				
					lineDirection = Vector3.Normalize(vecTarget_position - vecPosition);
				    float fSpeed =  (m_fSpeed *0.01f * Time.deltaTime) + (0.5f*m_acce* 0.01f *m_pastTime*m_pastTime);
								
					pos = (fSpeed * lineDirection ) + transform.position;
				
					//포물선 계산(Sin곡선 이용).
				    Vector3 vecStartPos = m_startPos;
					vecStartPos.y = 0;
					float processDist = Vector3.Distance (vecStartPos, vecPosition );		
					float remainDist = Vector3.Distance (vecStartPos,  vecTarget_position );	
		
					float processRatio = processDist / remainDist;					
					pos.y = m_startPos.y + ( Mathf.Sin(processRatio * 180 * Mathf.Deg2Rad) * m_fHeight );
				
					if(processRatio > 1.0f)
						pos.y =  m_target_position.y;
					
					transform.position = pos ;	
					transform.LookAt(m_target_position);	
					
					fDist = Vector3.Distance (m_target_position, transform.position);					
					if (fDist < 0.1f  )	m_IsHit = true;	
				
					if(Mathf.Abs(m_target_position.x - transform.position.x ) < 0.1f 
					&& Mathf.Abs(m_target_position.z - transform.position.z ) < 0.1f
					&& Mathf.Abs(m_target_position.y - transform.position.y ) < 0.1f)
					{
						m_IsHit = true;
					}	
				
					if (Vector3.Distance (m_target_position, m_startPos)  < Vector3.Distance (vecPosition, m_startPos) )
					{
						m_IsHit = true;	
					//	Debug.Log("BeforeDistance:" + m_fBeforeDistance);    
					//	Debug.Log("fDist:" + fDist);    
					//	Debug.Log("m_fSpeed:" + (m_fSpeed *0.01f * Time.deltaTime));    
					}
				}
		
			}
			break;
			case eProjectilePath.NONE:
			case eProjectilePath.Straight:
			case eProjectilePath.Through:
			{			
				if (!m_IsHit )
				{		
					if(!m_IsHold)
						m_target_position = m_targetTran.position;				
						
					lineDirection = Vector3.Normalize(m_target_position-transform.position);
				    float fSpeed =  (m_fSpeed *0.01f * Time.deltaTime) + (0.5f*m_acce* 0.01f *m_pastTime*m_pastTime);
				
					transform.position = (fSpeed * lineDirection ) + transform.position;
					
						
					fDist = Vector3.Distance (m_target_position, transform.position);
					 
			        transform.LookAt(m_target_position);
					
					if(Mathf.Abs(m_target_position.x - transform.position.x ) < 0.1f && Mathf.Abs(m_target_position.z - transform.position.z ) < 0.1f)
					{
						m_IsHit = true;
					}
					
					if (fDist < 0.1f  )	m_IsHit = true;		
				
					if (Vector3.Distance (m_target_position, m_startPos)  < Vector3.Distance (transform.position, m_startPos) )
					{
						m_IsHit = true;	
//						Debug.Log("BeforeDistance:" + m_fBeforeDistance);    
//						Debug.Log("fDist:" + fDist);    
//						Debug.Log("m_fSpeed:" + (m_fSpeed *0.01f * Time.deltaTime));    
					}
				    
				}
				else
				{					
						
					lineDirection = Vector3.Normalize(m_target_position-transform.position);
				    float fSpeed =  (m_fSpeed *0.01f * Time.deltaTime) + (0.5f*m_acce* 0.01f *m_pastTime*m_pastTime);
				
					transform.position = (fSpeed * lineDirection ) + transform.position;
					
				}
				
			}
			break;			
				
		}
		
		
		
		//HitResult funtion
		if (m_IsHit && m_shootDelTime == 0f)
		{
			//ProjectileHitSound Play			
			if(null != m_ProjectileHitSoundName  )						
				AsSoundManager.Instance.PlaySound( m_ProjectileHitSoundName, m_target_position, false);	
			else if(m_HitResult != null)	m_HitResult(this);		
		}
	
		//Delete Effect
		switch(m_eProjectlie_Path)
		{
				
			case eProjectilePath.NONE:
			case eProjectilePath.Arc:
			case eProjectilePath.Straight:
			{
				if(m_IsHit)	m_IsShootDelete = true;
			}
			break;
			case eProjectilePath.Through:
			{
			
				if(m_IsHit && m_shootDelTime == 0f)
					m_target_position +=  lineDirection * 10f; 
			
				if(m_shootDelTime > 0.5f )	m_IsShootDelete = true;
			}
			break;			
				
		}
		
		if(m_IsHit)	m_shootDelTime += Time.deltaTime; 	
		if(m_IsShootDelete) 	AsEffectManager.Instance.RemoveEffectEntity(m_Id);		
	
	}
	// Update is called once per frame
	void Update ()
	{
		m_pastTime += Time.deltaTime; 
	
		if( m_fLifeTime != 0f &&  m_fLifeTime <= m_pastTime )
		{			
			//AsEffectManager.Instance.Remove_EffectList(this);
			//Delete();
			AsEffectManager.Instance.RemoveEffectEntity(m_Id);
			return;
		}
		
		
			
		
		if (m_EffectType == eEffect_Type.ShootingEffect )
		{			
			Update_ShootinEffect();
		}
	}

	public void PlayOnce( )
	{
		m_EffectPrefab.Enable(false);
	}
		
   
	public bool IsPlaying()
    {
      return m_EffectPrefab.IsPlaying();
    }

	
    public void StopEffect()
    {
      m_EffectPrefab.SetVisible();
    }
	 #region - SkillEditor -
    public void Play()
    {
        m_EffectPrefab.PlayEffect();
    }
	public void UpdateEffect()
	{
		m_EffectPrefab.RemoveInstantiate();
		m_EffectPrefab.UpdateEffect();
	}
	
	bool InitTool_EffectPrefab(AsEffectData effectData)
	{
		m_EffectPrefab = effectData;
		if(m_EffectPrefab == null)
		{
			Debug.Log("CreateEffect(EffectDataComponent) Not found Resource :" );    
			return false;
		}	
		m_EffectPrefab.AutoDel = false;
		m_EffectPrefab.ParentObject =  this.GetComponent<AsEffectEntity>();
		return true;
	}
	
	public bool Tool_CreateEffect(AsEffectData effectData, Vector3 _pos, bool bLoop, float fStartSize = 1f )
	{		
		if(!InitTool_EffectPrefab(effectData)) return false;
		m_EffectType = eEffect_Type.PositionEffect;
		m_EffectPrefab.Enable( _pos, bLoop, fStartSize);			
		transform.position = _pos;
		return true;
	}
	
	public bool Tool_CreateEffect(AsEffectData effectData, Transform _tra, bool bLoop, float fStartSize = 1f )
	{	
		
		if(!InitTool_EffectPrefab(effectData)) return false;
		m_EffectType = eEffect_Type.AttachEffect;
		m_EffectPrefab.Enable( _tra, bLoop,fStartSize);	

		return true;
	}
	
	 public bool Tool_CreateEffect(AsEffectData effectData, Vector3 _startPos, Transform _targetTran, float fStartSize = 1f )
	{
		m_pastTime = 0f;
		
		if( null == effectData)
		{
			Debug.Log ("CreateEffect(Shooting) Not found Resource :");
			return false;
		}
		
		m_EffectType = eEffect_Type.ShootingEffect;
		transform.position  = _startPos;
	
		if (_targetTran != null) 	
		{
			m_targetTran = ResourceLoad.SearchHierarchyTransform (_targetTran, "DummyCharacterCenter");
			//m_fDistance = Vector3.Distance (m_targetTran.position, transform.position);
		}
	
		
		m_EffectPrefab = effectData;
		
		m_EffectPrefab.AutoDel = false;
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity> ();
		m_EffectPrefab.Enable (false);
	
	  	
	
		return true;
	}
	
    #endregion
	#region Will be deleted....	
	
	 public bool CreateEffect (AsEffectData effectData, Vector3 _startPos, Transform _targetTran,  HitResultFunc _HitResult , float _speed)
	{
		m_pastTime = 0f;
		
		if( null == effectData)
		{
			Debug.Log ("CreateEffect(Shooting) Not found Resource :");
			return false;
		}
		
		m_EffectType = eEffect_Type.ShootingEffect;
		transform.position  = _startPos;
	
		if (_targetTran != null) 	
		{
			m_targetTran = ResourceLoad.SearchHierarchyTransform (_targetTran, "DummyCharacterCenter");
			//m_fDistance = Vector3.Distance (m_targetTran.position, transform.position);
		}
	
		
		m_EffectPrefab = effectData;
		
		m_EffectPrefab.AutoDel = true;
		m_EffectPrefab.ParentObject = this.GetComponent<AsEffectEntity> ();
		m_EffectPrefab.Enable (false);
	
	    m_HitResult = _HitResult;
		if(_speed != 0f)	m_fSpeed = _speed;		
		return true;
	}
	
	 #endregion
}
