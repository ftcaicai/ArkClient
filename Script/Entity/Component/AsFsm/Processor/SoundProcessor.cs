using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eSoundState {Before_Play, After_Play, Finish_Play}

public class SoundElement
{
	AnimationState m_AnimState;
	Tbl_Action_Sound m_ActionSound;
	Transform m_EntityTrn;
	float m_BeginTime;
	float m_Timing;
	float m_Length = 1f;
	float m_SoundBegan;
	AudioSource m_Sound;
	eSoundState m_State = eSoundState.Before_Play;
	
	public SoundElement( AnimationState _state, Tbl_Action_Sound _Sound, Transform _trn, Vector3 _pos, float _animSpeed)
	{
		if( _state == null)
		{
			Debug.LogWarning("SoundElement::constructor: _state is null");
//			return;
		}
		
		m_AnimState = _state;
		m_ActionSound = _Sound;
		
		if( _animSpeed == 0f)
			Debug.LogError( "SoundProcessor::SoundElement: _animSpeed = 0");
		m_Timing = m_ActionSound.Timing / _animSpeed;
		m_BeginTime = Time.time;
		if( m_AnimState != null)
			m_Length = m_AnimState.length / _animSpeed;
		m_SoundBegan = 0f;
		m_EntityTrn = _trn;
	}
	
	public void Update()
	{
		switch( m_State)
		{
		case eSoundState.Before_Play:
			BeforePlay();
			break;
		case eSoundState.After_Play:
			AfterPlay();
			break;
		case eSoundState.Finish_Play:
			FinishPlay();
			break;
		}
		
		if( m_EntityTrn != null)
			AsSoundManager.Instance.UpdatePosition( m_Sound, m_EntityTrn.position);
	}
	
	void BeforePlay()
	{
		float time = GetProceedTime();
		
		if( time > m_Timing)
		{
			switch( m_ActionSound.LoopType)
			{
			case eLoopType.Once:
				m_Sound = AsSoundManager.Instance.PlaySound( m_ActionSound.FileName, m_EntityTrn.position, false);
				break;
			case eLoopType.Once_Cycle:
				if(m_Sound == null)
				{
					m_Sound = AsSoundManager.Instance.PlaySound( m_ActionSound.FileName, m_EntityTrn.position, false);
					if(m_Sound != null)
						GameObject.Destroy( m_Sound.GetComponent<AsSoundObject>());
//					else
//						Debug.LogError("SoundProcessor::BeforePlay: '" + m_ActionSound.FileName + "' is not found");
				}
				else
				{
					m_Sound.Play();
				}
				break;
			case eLoopType.Loop:
				m_Sound = AsSoundManager.Instance.PlaySound( m_ActionSound.FileName, m_EntityTrn.position, true);
				break;
			}
			
			m_State = eSoundState.After_Play;
		}
	}
	
	void AfterPlay()
	{
		switch( m_ActionSound.LoopType)
		{
		case eLoopType.Once_Cycle:
			AfterPlay_Once_Cycle();
			break;
		}
	}
	
	void FinishPlay()
	{
		switch( m_ActionSound.LoopType)
		{
		case eLoopType.Once_Cycle:
			FinishPlay_Once_Cycle();
			break;
		}
	}
	
	public void Release()
	{
		switch( m_ActionSound.LoopType)
		{
		case eLoopType.Once:
			break;
		case eLoopType.Once_Cycle:
			if(m_Sound != null)
				GameObject.Destroy(m_Sound.gameObject);
			break;
		default:
			AsSoundManager.Instance.StopSound( m_Sound);
			break;
		}
	}
	
	float GetProceedTime()
	{
		float time = float.MaxValue;
		time = Time.time - m_BeginTime;
		
		return time;
	}
	
	void AfterPlay_Once_Cycle()
	{
//		m_SoundBegan += Time.time;
//		
//		if( m_SoundBegan > m_Sound.clip.length)
//		{
//			m_SoundBegan = 0f;
//			Debug.Log("AfterPlay_Once_Cycle: " + m_Sound.name + " : " + m_SoundBegan);
//			
//			m_Sound.mute = true;
//			m_State = eSoundState.Finish_Play;
//		}
		
//		if( m_Sound.isPlaying == false)
//		{
			m_State = eSoundState.Finish_Play;
//		}
	}
	
	void FinishPlay_Once_Cycle()
	{
		if( Time.time - m_BeginTime > m_Length)
		{
			m_BeginTime = Time.time;
			m_State = eSoundState.Before_Play;
		}
	}
}

public class SoundProcessor
{
	List<SoundElement> m_listSound = new List<SoundElement>();
	
	AsBaseEntity m_Owner;public AsBaseEntity Owner{get{return m_Owner;}}
	
	public SoundProcessor( AsBaseEntity _entity)
	{
		m_Owner = _entity;
	}
	
	public void AddSound( Tbl_Action_Animation _action, float _animSpeed)
	{
		if( m_Owner.ModelObject != null)
		{
			if( m_Owner.ModelObjectAnimation == null)
				return;
			
			AnimationState state = m_Owner.ModelObjectAnimation[_action.FileName];
			if( state == null)
				Debug.LogWarning( "SoundProcessor::AddSound: no animation state is found[" + _action.FileName + "].");
			
			foreach( Tbl_Action_Sound sound in _action.listSound)
			{
				m_listSound.Add( new SoundElement( state, sound, m_Owner.ModelObject.transform, Vector3.zero, _animSpeed));
			}
		}
	}

//	public void AddSound( List<Tbl_Action_Sound> _list, float _animSpeed)
//	{
//		if( m_Owner.ModelObject != null)
//		{
//			foreach( Tbl_Action_Sound sound in _list)
//			{
//				m_listSound.Add( new SoundElement( sound, m_Owner.ModelObject.transform, Vector3.zero, _animSpeed));
//			}
//		}
//	}

	public void Update()
	{
		for(int i=0; i<m_listSound.Count; ++i)
		{
			m_listSound[i].Update();
		}
	}
	
	public void Release()
	{
		foreach( SoundElement element in m_listSound)
		{
			element.Release();
		}
		
		m_listSound.Clear();
	}
}
