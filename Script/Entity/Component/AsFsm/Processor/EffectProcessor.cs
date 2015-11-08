using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eEffectState {Before_Play, After_Play, Finish_Play}

public class EffectElement
{
	Tbl_Action_Effect m_Effect;
	
	AsBaseEntity m_Owner;
		
	string m_EffectName;
	float m_Timing;
	eLoopType m_LoopType;
	eLinkType m_LinkType;
	float m_LoopDuration;
	bool m_PositionFix;
	
	//position cached
	Vector3 m_PositionCached;
	
	float m_SetTime;
	float m_BeganTime;
	Vector3 m_Pos;
	Transform m_TargetTrn;
	
	// calculated
	float m_AnimSpeed;
	int m_EffectIdx;
	
	// control
	eEffectState m_State = eEffectState.Before_Play;
	
	public EffectElement(Tbl_Action_Effect _effect, AsBaseEntity _owner, Vector3 _pos, Transform _targetTrn, float _animSpeed)
	{
		if(_animSpeed == 0f) Debug.LogError("BuffProcessor::EffectElement: _animSpeed = 0");
		
		m_Effect = _effect;
		
		m_Owner = _owner;
		
		m_EffectName = _effect.FileName;
		m_Timing = _effect.Timing;// / _animSpeed;
		m_LoopType = _effect.LoopType;
		m_LinkType = _effect.LinkType;
		m_LoopDuration = _effect.LoopDuration;
		m_PositionFix = _effect.PositionFix;
		
		m_SetTime = Time.time;
		m_Pos = _pos;
		m_TargetTrn = _targetTrn;
		if( m_TargetTrn != null)
			m_PositionCached = m_TargetTrn.position;
		
		m_AnimSpeed = _animSpeed;
	}
	
	public void Update()
	{
		switch(m_State)
		{
		case eEffectState.Before_Play:
			BeforePlay();
			break;
		case eEffectState.After_Play:
			AfterPlay();
			break;
		}
	}
	
	void BeforePlay()
	{
		if(m_Owner.GetAnimationTime() > m_Timing ||
			(m_Owner.GetAnimationTime() == 0f && (Time.time - m_SetTime) * m_AnimSpeed > m_Timing))
		{
			m_State = eEffectState.After_Play;
			
			bool loop;
			switch(m_LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Cut:
				loop = false;
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
				loop = true;
				break;
			default:
				Debug.LogWarning("EffectElement:: Update: loop type is NONE. check action table.");
				loop = false;
				break;
			}
			
			switch(m_LinkType)
			{
			case eLinkType.AttachDummy:
				if( m_Owner.ModelObject != null)
					m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, m_Owner.ModelObject.transform, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
				break;
			case eLinkType.HitPosition:
				if(m_Pos != Vector3.zero)
				{
					m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, m_Pos, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
					AsEffectManager.Instance.SetAngleY(m_EffectIdx, m_Owner.transform.rotation.eulerAngles.y);
				}
				else if(m_TargetTrn != null)
					m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, m_TargetTrn, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
				else
					Debug.LogWarning("EffectProcessor::BeforePlay: [eLinkType.HitPosition] position is (0,0,0) and target transform is null");
				break;
			case eLinkType.HoldPosition:
				m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, m_Owner.transform.position, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
				AsEffectManager.Instance.SetAngleY(m_EffectIdx, m_Owner.transform.rotation.eulerAngles.y);
				break;
			case eLinkType.HitHoldPosition:
				Vector3 pos = Vector3.zero;
				if( m_PositionFix == false)
				{
					if( m_TargetTrn != null)
						pos = m_TargetTrn.position;
					else if(m_Pos != Vector3.zero)
						pos = m_Pos;
				}
				else
				{
					if( m_PositionCached != Vector3.zero)
						pos = m_PositionCached;
				}
				
				if( pos == Vector3.zero)
				{
					Debug.LogError("EffectProcessor::BeforePlay: [eLinkType.HitPosition] position is (0,0,0) and target transform is null");
					break;
				}
				m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, pos, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
				AsEffectManager.Instance.SetAngleY(m_EffectIdx, m_Owner.transform.rotation.eulerAngles.y);
				break;
			case eLinkType.TargetChain:
				if( m_Owner.ModelObject != null && m_TargetTrn != null)
				{
					m_EffectIdx = AsEffectManager.Instance.PlayEffect(m_EffectName, m_Owner.ModelObject.transform, m_TargetTrn, loop, m_LoopDuration * 0.001f, m_Owner.transform.localScale.y * m_Effect.StartSize);
					AsEffectManager.Instance.SetAngleY(m_EffectIdx, m_Owner.ModelObject.transform.rotation.eulerAngles.y);
				}
				else
					Debug.LogWarning("Effectprocessor::BeforePlay: target transform is null. 'TargetChain' effect cannot be played");
				break;
			case eLinkType.Shooting:
				if( m_Owner.ModelObject != null && m_TargetTrn != null && m_Effect.Action.HitAnimation != null)
				{
					AsEffectManager.Instance.ShootingEffect(
						m_Effect.FileName, m_Owner.ModelObject.transform, m_TargetTrn, null,
						m_Effect.Action.HitAnimation.hitInfo.HitProjectileSpeed, m_Effect.Action.HitAnimation.hitInfo.HitProjectileAccel,
						m_Effect.Action.HitAnimation.hitInfo.HitProjectilePath, m_Owner.ModelObject.transform.localScale.y);
				}
				else
					Debug.LogWarning("Effectprocessor::BeforePlay: target transform is null. 'Shooting' effect cannot be played");
				break;
			case eLinkType.ShootingPosition:
				if( m_Owner.ModelObject != null && m_Effect.Action.HitAnimation != null)
				{
					AsEffectManager.Instance.ShootingEffect(
						m_Effect.FileName, m_Owner.ModelObject.transform, m_Pos, null,
						m_Effect.Action.HitAnimation.hitInfo.HitProjectileSpeed, m_Effect.Action.HitAnimation.hitInfo.HitProjectileAccel,
						m_Effect.Action.HitAnimation.hitInfo.HitProjectilePath, m_Owner.ModelObject.transform.localScale.y);
				}
				else
					Debug.LogWarning("Effectprocessor::BeforePlay: m_Effect.Action.HitAnimation == null. 'ShootingPosition' effect cannot be played");
				break;
			}
			
			AsEffectManager.Instance.SetSpeed(m_EffectIdx, m_AnimSpeed);
			
			m_BeganTime = Time.time;
		}
	}
	
	void AfterPlay()
	{
		if(m_LoopType == eLoopType.TimeLoop)
		{
			if(Time.time > m_BeganTime + m_LoopDuration)
			{
				m_State = eEffectState.Finish_Play;
				AsEffectManager.Instance.RemoveEffectEntity(m_EffectIdx);
			}
		}
	}
	
	public void Release(bool _canceled)
	{
		switch(m_LoopType)
		{
		case eLoopType.Cast:
		case eLoopType.Charge:
		case eLoopType.Loop:
		case eLoopType.TargetLoop:
		case eLoopType.TimeLoop:
		case eLoopType.ClampForever:
			AsEffectManager.Instance.RemoveEffectEntity(m_EffectIdx);
			break;
		case eLoopType.Cut:
			if(_canceled == true)
				AsEffectManager.Instance.RemoveEffectEntity(m_EffectIdx);
			break;
		case eLoopType.Once:
//			AsEffectManager.Instance.RemoveEffectEntity(m_EffectIdx);
			break;
		}
	}
	
	public void Cancel()
	{
		AsEffectManager.Instance.RemoveEffectEntity(m_EffectIdx);
	}
}

public class EffectProcessor
{
	List<EffectElement> m_listEffect = new List<EffectElement>();
	
	AsBaseEntity m_Owner;public AsBaseEntity Owner{get{return m_Owner;}}
	
	public EffectProcessor(AsBaseEntity _entity)
	{
		m_Owner = _entity;
	}

	public void AddEffect(List<Tbl_Action_Effect> _list)
	{
		AddEffect(_list, Vector3.zero, null, 1, true);
	}
	
	public void AddEffect(List<Tbl_Action_Effect> _list, Vector3 _pos, Transform _targetTrn, float _animSpeed, bool _refresh)
	{
//		if(m_Owner.ModelObject != null)
//		{	
			
			if( _refresh == true)
				m_listEffect.Clear();
			
			foreach(Tbl_Action_Effect effect in _list)
			{
				m_listEffect.Add(new EffectElement(effect, m_Owner, _pos, _targetTrn, _animSpeed));
			}
//		}
	}
	
	public void Update()
	{
		for(int i=0; i<m_listEffect.Count; ++i)
		{
			m_listEffect[i].Update();
		}
	}
	
	public void Release(bool _canceled)
	{
		foreach(EffectElement element in m_listEffect)
		{
			element.Release(_canceled);
		}
		
		m_listEffect.Clear();
	}
	
	public void Cancel()
	{
		foreach(EffectElement element in m_listEffect)
		{
			element.Cancel();
		}
		
		m_listEffect.Clear();
	}
}

