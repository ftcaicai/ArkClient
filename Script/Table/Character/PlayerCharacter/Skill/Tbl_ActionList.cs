using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

#region - sub element (effect, sound, trail, cancel) -
public enum eElementSequence {Effect = 0, Sound, Trail, Cancel}

public class Tbl_Action_Effect
{
	Tbl_Action_Record m_Action;public Tbl_Action_Record Action{get{return m_Action;}}
	
	float m_Timing;public float Timing{get{return m_Timing;}}
	string m_FileName;public string FileName{get{return m_FileName;}}
	eLinkType m_LinkType;public eLinkType LinkType{get{return m_LinkType;}}
	eLoopType m_LoopType;public eLoopType LoopType{get{return m_LoopType;}}
	float m_LoopDuration;public float LoopDuration{get{return m_LoopDuration;}}
	bool m_PositionFix;public bool PositionFix{get{return m_PositionFix;}}
	float m_StartSize;public float StartSize{get{return m_StartSize;}}
	
//	public Tbl_Action_Effect(float _timing, string _fileName, eLinkType _linkType, eLoopType _loopType, float _loopDuration, float _startSize)
//	{
//		m_Timing = _timing * 0.001f;
//		m_FileName = _fileName;
//		m_LinkType = _linkType;
//		m_LoopType = _loopType;
//		m_LoopDuration = _loopDuration;
//		m_StartSize = _startSize;
//	}
	
	public Tbl_Action_Effect(Tbl_Action_Record _action, float _timing, string _fileName, eLinkType _linkType, eLoopType _loopType, float _loopDuration, bool _positionFix, float _startSize)
	{
		m_Action = _action;
		
		m_Timing = _timing * 0.001f;
		m_FileName = _fileName;
		m_LinkType = _linkType;
		m_LoopType = _loopType;
		m_LoopDuration = _loopDuration;
		m_PositionFix = _positionFix;
		m_StartSize = _startSize;
	}
}
public class Tbl_Action_Sound
{
	float m_Timing;public float Timing{get{return m_Timing;}}
	string m_FileName;public string FileName{get{return m_FileName;}}
	eLoopType m_LoopType;public eLoopType LoopType{get{return m_LoopType;}}
	
	public Tbl_Action_Sound(float _timing, string _fileName, eLoopType _loopType)
	{
		m_Timing = _timing * 0.001f;
		m_FileName = _fileName;
		m_LoopType = _loopType;
	}
}
public class Tbl_Action_Trail
{
	string m_Texture;public string Texture{get{return m_Texture;}}
	float m_StartTime;public float StartTime{get{return m_StartTime;}}
	float m_EndTime;public float EndTime{get{return m_EndTime;}}
	float m_LoopDuration;public float LoopDuration{get{return m_LoopDuration;}}
	
	public Tbl_Action_Trail(string _texture, float _startTime, float _endTime, float _loopDuration)
	{
		m_Texture = _texture;
		m_StartTime = _startTime * 0.001f;
		m_EndTime = _endTime * 0.001f;
		
		m_LoopDuration = _loopDuration;
	}
}
public class Tbl_Action_Cancel
{
	float m_StartTime;public float StartTime{get{return m_StartTime;}}
	float m_EndTime;public float EndTime{get{return m_EndTime;}}
	
	public Tbl_Action_Cancel(float _startTime, float _endTime)
	{
		m_StartTime = _startTime * 0.001f;
		m_EndTime = _endTime * 0.001f;
	}
}
#endregion
#region - anim & hit info -
public class Tbl_Action_Animation
{
	public WrapMode wrapMode{
		get{
			switch(m_LoopType)
			{
			case eLoopType.Cast:
			case eLoopType.Charge:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
				return WrapMode.Loop;
			case eLoopType.ClampForever:
				return WrapMode.ClampForever;
			case eLoopType.NONE:
				return WrapMode.Default;
			case eLoopType.Once:
			case eLoopType.TargetLoop:
			default:	
				return WrapMode.Once;
			}
			
		}
	}
	
	protected string m_FileName;public string FileName{get{return m_FileName;}}
	protected float m_ActionSpeed;public float ActionSpeed{get{return m_ActionSpeed;}}
	protected eLoopType m_LoopType;public eLoopType LoopType{get{return m_LoopType;}}
	protected float m_LoopTargetTime;public float LoopTargetTime{get{return m_LoopTargetTime;}}
	protected float m_LoopDuration;public float LoopDuration{get{return m_LoopDuration;}}
	protected float m_AnimationLength;public float AnimationLength{get{return m_AnimationLength;}}
	
	protected List<Tbl_Action_Effect> m_listEffect = new List<Tbl_Action_Effect>();public List<Tbl_Action_Effect> listEffect{get{return m_listEffect;}}
	protected List<Tbl_Action_Sound> m_listSound = new List<Tbl_Action_Sound>();public List<Tbl_Action_Sound> listSound{get{return m_listSound;}}
	protected List<Tbl_Action_Trail> m_listTrail = new List<Tbl_Action_Trail>();public List<Tbl_Action_Trail> listTrail{get{return m_listTrail;}}
	protected List<Tbl_Action_Cancel> m_listCancel = new List<Tbl_Action_Cancel>();public List<Tbl_Action_Cancel> listCancel{get{return m_listCancel;}}
	
	public Tbl_Action_Animation(string _fileName, float _speed, eLoopType _loop, float _loopTargetTime, float _loopDuration, float _animationLength)
//		List<Tbl_Action_Effect> _effects, List<Tbl_Action_Sound> _sounds, List<Tbl_Action_Trail> _trails, List<Tbl_Action_Cancel> _cancels)
	{
		m_FileName = _fileName;
		m_ActionSpeed = _speed;
		m_LoopType = _loop;
		m_LoopTargetTime = _loopTargetTime;
		m_LoopDuration = _loopDuration;
		m_AnimationLength = _animationLength;
		
//		m_listEffect = _effects;
//		m_listSound = _sounds;
//		m_listTrail = _trails;
//		m_listCancel = _cancels;
	}
	
	public void SetEffect(List<Tbl_Action_Effect> _effects)
	{
		m_listEffect = _effects;
	}
	public void SetSound(List<Tbl_Action_Sound> _sounds)
	{
		m_listSound = _sounds;
	}
	public void SetTrail(List<Tbl_Action_Trail> _trails)
	{
		m_listTrail = _trails;
	}
	public void SetCancel(List<Tbl_Action_Cancel> _cancels)
	{
		m_listCancel = _cancels;
	}
}

public class Tbl_Action_ReadyAnimation : Tbl_Action_Animation
{
	eReadyMoveType m_MoveType;public eReadyMoveType MoveType{get{return m_MoveType;}}
	float m_MoveDistance;public float MoveDistance{get{return m_MoveDistance;}}
	
	public Tbl_Action_ReadyAnimation(string _fileName, float _speed, eLoopType _loop, float _loopTargetTime, float _loopDuration, float _animationLength, eReadyMoveType _moveType, float _moveDistance) :
		base(_fileName, _speed, _loop, _loopTargetTime, _loopDuration, _animationLength)
	{
		m_MoveType = _moveType;
		m_MoveDistance = _moveDistance;
	}
}

public class Tbl_Action_HitAnimation : Tbl_Action_Animation
{
	eHitMoveType m_MoveType;public eHitMoveType MoveType{get{return m_MoveType;}}
	float m_MoveDistance;public float MoveDistance{get{return m_MoveDistance;}}
	
	Tbl_Action_HitInfo m_hitInfo = null;public Tbl_Action_HitInfo hitInfo{
		get{
			if(m_hitInfo == null)
				Debug.LogWarning("Tbl_Action_HitInfo:: this is not hit info. null value will be returned");
			
			return m_hitInfo;
		}
	}
	
	public Tbl_Action_HitAnimation(string _fileName, float _speed, eLoopType _loop, float _loopTargetTime, float _loopDuration, float _animationLength, eHitMoveType _moveType, float _moveDistance) :
		base(_fileName, _speed, _loop, _loopTargetTime, _loopDuration, _animationLength)
	{
		m_MoveType = _moveType;
		m_MoveDistance = _moveDistance;
	}
	
//	public void SetMoveInfo(eHitMoveType _moveType, float _moveDistance)
//	{
//		m_MoveType = _moveType;
//		m_MoveDistance = _moveDistance;
//	}
	
	public void SetHitInfo(Tbl_Action_HitInfo _hitInfo)
	{
		m_hitInfo = _hitInfo;
	}
}

public class Tbl_Action_HitInfo_
{
	eHitType m_HitType;public eHitType HitType{get{return m_HitType;}}
	float m_HitTiming;public float HitTiming{get{return m_HitTiming;}}
	string m_HitProjectileName;public string HitProjectileName{get{return m_HitProjectileName;}}
	string m_HitProjectileHitFileName;public string HitProjectileHitFileName{get{return m_HitProjectileHitFileName;}}
	float m_HitProjectileSpeed;public float HitProjectileSpeed{get{return m_HitProjectileSpeed;}}
	float m_HitProjectileAccel;public float HitProjectileAccel{get{return m_HitProjectileAccel;}}
	eProjectilePath m_HitProjectilePath;public eProjectilePath HitProjectilePath{get{return m_HitProjectilePath;}}
	eHitAreaShape m_HitAreaShape;public eHitAreaShape AreaShape{get{return m_HitAreaShape;}}
	float m_HitAngle;public float HitAngle{get{return m_HitAngle;}}
	float m_HitCenterDirectionAngle;public float HitCenterDirectionAngle{get{return m_HitCenterDirectionAngle;}}
	float m_HitMinDistance;public float HitMinDistance{get{return m_HitMinDistance;}}
	float m_HitMaxDistance;public float HitMaxDistance{get{return m_HitMaxDistance;}}
	float m_HitWidth;public float HitWidth{get{return m_HitWidth;}}
	float m_HitHeight;public float HitHeight{get{return m_HitHeight;}}
	float m_HitOffsetX;public float HitOffsetX{get{return m_HitOffsetX;}}
	float m_HitOffsetY;public float HitOffsetY{get{return m_HitOffsetY;}}
	float m_HitValuePercent;public float HitValuePercent{get{return m_HitValuePercent;}}
	eValueLookType m_HitValueLookType;public eValueLookType HitValueLookType{get{return m_HitValueLookType;}}
	float m_HitValueLookDuration;public float HitValueLookDuration{get{return m_HitValueLookDuration;}}
	int m_HitValueLookCount;public int HitValueLookCount{get{return m_HitValueLookCount;}}
	string m_HitProjectileHitSoundPath;public string HitProjectileHitSoundPath{get{return m_HitProjectileHitSoundPath;}}
	
	public Tbl_Action_HitInfo_(XmlNode _node)
	{
		try{
			m_HitType = (eHitType)Enum.Parse(typeof(eHitType), _node.Attributes["HitType"].Value, true);
			m_HitTiming = float.Parse(_node.Attributes["Timing"].Value);
			m_HitProjectileName = _node.Attributes["ProjectileFilePath"].Value;
			m_HitProjectileHitFileName = _node.Attributes["ProjectileHitFilePath"].Value;
			m_HitProjectileSpeed = float.Parse(_node.Attributes["ProjectileSpeed"].Value);
			m_HitProjectileAccel = float.Parse(_node.Attributes["ProjectileAcceleration"].Value);
			m_HitProjectilePath = (eProjectilePath)Enum.Parse(typeof(eProjectilePath), _node.Attributes["ProjectilePath"].Value, true);
			m_HitAreaShape = (eHitAreaShape)Enum.Parse(typeof(eHitAreaShape), _node.Attributes["AreaShape"].Value, true);
			m_HitAngle = float.Parse(_node.Attributes["Angle"].Value);
			m_HitCenterDirectionAngle = float.Parse(_node.Attributes["CenterDirectionAngle"].Value);
			m_HitMinDistance = float.Parse(_node.Attributes["MinDistance"].Value);
			m_HitMaxDistance = float.Parse(_node.Attributes["MaxDistance"].Value);
			m_HitWidth = float.Parse(_node.Attributes["Width"].Value);
			m_HitHeight = float.Parse(_node.Attributes["Height"].Value);
			m_HitOffsetX = float.Parse(_node.Attributes["OffsetX"].Value);
			m_HitOffsetY = float.Parse(_node.Attributes["OffsetY"].Value);
			m_HitValuePercent = float.Parse(_node.Attributes["ValuePercent"].Value);
			m_HitValueLookType = (eValueLookType)Enum.Parse(typeof(eValueLookType), _node.Attributes["ValueLookType"].Value, true);
			m_HitValueLookDuration = float.Parse(_node.Attributes["ValueLookDuration"].Value);
			m_HitValueLookCount = int.Parse(_node.Attributes["ValueLookCount"].Value);
			m_HitProjectileHitSoundPath = _node.Attributes["ProjectileHitSoundPath"].Value;
		}
		catch(Exception e){
			Debug.LogError(e);
		}
	}
}

public class Tbl_Action_HitInfo
{
	eHitType m_HitType;public eHitType HitType{get{return m_HitType;}}
	int m_HitMultiTargetCount;public int HitMultiTargetCount{get{return m_HitMultiTargetCount;}}
	float m_HitTiming;public float HitTiming{get{return m_HitTiming;}}
	string m_HitProjectileName;public string HitProjectileName{get{return m_HitProjectileName;}}
	string m_HitProjectileHitFileName;public string HitProjectileHitFileName{get{return m_HitProjectileHitFileName;}}
	
	float m_HitProjectileSpeed;public float HitProjectileSpeed{get{return m_HitProjectileSpeed;}}
	float m_HitProjectileAccel;public float HitProjectileAccel{get{return m_HitProjectileAccel;}}
	eProjectilePath m_HitProjectilePath;public eProjectilePath HitProjectilePath{get{return m_HitProjectilePath;}}
	float m_HitValuePercent;public float HitValuePercent{get{return m_HitValuePercent;}}
	eValueLookType m_HitValueLookType;public eValueLookType HitValueLookType{get{return m_HitValueLookType;}}
	
	public float HitAngle_{get{return m_AreaInfo[0].HitAngle;}}
	public float HitMinDistance_{get{return m_AreaInfo[0].HitMinDistance;}}
	public float HitMaxDistance_{get{return m_AreaInfo[0].HitMaxDistance;}}
	public float HitOffsetX_{get{return m_AreaInfo[0].HitOffsetX;}}
	public float HitOffsetY_{get{return m_AreaInfo[0].HitOffsetY;}}
	
	float m_HitValueLookDuration;public float HitValueLookDuration{get{return m_HitValueLookDuration;}}
	int m_HitValueLookCount;public int HitValueLookCount{get{return m_HitValueLookCount;}}
	string m_HitProjectileHitSoundPath;public string HitProjectileHitSoundPath{get{return m_HitProjectileHitSoundPath;}}
	
	List<Tbl_Action_AreaInfo> m_AreaInfo = new List<Tbl_Action_AreaInfo>(); public List<Tbl_Action_AreaInfo> AreaInfo{get{return m_AreaInfo;}}
	
	public Tbl_Action_HitInfo(XmlNode _node)
	{
		try{
			m_HitType = (eHitType)Enum.Parse(typeof(eHitType), _node.Attributes["HitType"].Value, true);
			m_HitMultiTargetCount = int.Parse(_node.Attributes["MultiTargetCount"].Value);
			m_HitTiming = float.Parse(_node.Attributes["Timing"].Value);
			m_HitProjectileName = _node.Attributes["ProjectileFilePath"].Value;
			m_HitProjectileHitFileName = _node.Attributes["ProjectileHitFilePath"].Value;
			
			m_HitProjectileSpeed = float.Parse(_node.Attributes["ProjectileSpeed"].Value);		
			m_HitProjectileAccel = float.Parse(_node.Attributes["ProjectileAcceleration"].Value);
			m_HitProjectilePath = (eProjectilePath)Enum.Parse(typeof(eProjectilePath), _node.Attributes["ProjectilePath"].Value, true);
			m_HitValuePercent = float.Parse(_node.Attributes["ValuePercent"].Value);
			m_HitValueLookType = (eValueLookType)Enum.Parse(typeof(eValueLookType), _node.Attributes["ValueLookType"].Value, true);
			
			m_HitValueLookDuration = float.Parse(_node.Attributes["ValueLookDuration"].Value);
			m_HitValueLookCount = int.Parse(_node.Attributes["ValueLookCount"].Value);
			m_HitProjectileHitSoundPath = _node.Attributes["ProjectileHitSoundPath"].Value;
			
			XmlNode nodeArea = _node.NextSibling;
			while(true)
			{
				if(nodeArea == null)
					break;
				
				Tbl_Action_AreaInfo areaInfo = new Tbl_Action_AreaInfo(nodeArea);
				m_AreaInfo.Add(areaInfo);
				
				nodeArea = nodeArea.NextSibling;
			}
		}
		catch(Exception e){
			Debug.LogError(e);
		}
	}
}

public class Tbl_Action_AreaInfo
{
	eHitAreaShape m_HitAreaShape;public eHitAreaShape AreaShape{get{return m_HitAreaShape;}}
	float m_HitAngle;public float HitAngle{get{return m_HitAngle;}}
	float m_HitCenterDirectionAngle;public float HitCenterDirectionAngle{get{return m_HitCenterDirectionAngle;}}
	float m_HitMinDistance;public float HitMinDistance{get{return m_HitMinDistance;}}
	float m_HitMaxDistance;public float HitMaxDistance{get{return m_HitMaxDistance;}}
	
	float m_HitWidth;public float HitWidth{get{return m_HitWidth;}}
	float m_HitHeight;public float HitHeight{get{return m_HitHeight;}}
	float m_HitOffsetX;public float HitOffsetX{get{return m_HitOffsetX;}}
	float m_HitOffsetY;public float HitOffsetY{get{return m_HitOffsetY;}}
	
	public Tbl_Action_AreaInfo(XmlNode _node)
	{
		try{
			m_HitAreaShape = (eHitAreaShape)Enum.Parse(typeof(eHitAreaShape), _node.Attributes["AreaShape"].Value, true);
			m_HitAngle = float.Parse(_node.Attributes["Angle"].Value);
			m_HitCenterDirectionAngle = float.Parse(_node.Attributes["CenterDirectionAngle"].Value);
			m_HitMinDistance = float.Parse(_node.Attributes["MinDistance"].Value);
			m_HitMaxDistance = float.Parse(_node.Attributes["MaxDistance"].Value);
			
			m_HitWidth = float.Parse(_node.Attributes["Width"].Value);
			m_HitHeight = float.Parse(_node.Attributes["Height"].Value);
			m_HitOffsetX = float.Parse(_node.Attributes["OffsetX"].Value);
			m_HitOffsetY = float.Parse(_node.Attributes["OffsetY"].Value);
		}
		catch(Exception e){
			Debug.LogError(e);
		}
	}
}
#endregion

public class Tbl_Action_Record : AsTableRecord
{
	public enum eType {User, Monster, Pet}
	
	int					m_Index;public int Index{get{return m_Index;}}
	string				m_ActionName;public string ActionName{get{return m_ActionName;}}
	eCLASS				m_Class;public eCLASS Class{get{return m_Class;}}
	string				m_ClassName;public string ClassName{get{return m_ClassName;}}
	eGENDER				m_Gender = eGENDER.eGENDER_MALE;public eGENDER Gender{get{return m_Gender;}}
	int					m_LinkActionIndex;public int LinkActionIndex{get{return m_LinkActionIndex;}}
	float				m_AniBlendingDuration;public float AniBlendingDuration{get{return m_AniBlendingDuration;}}
	eActionStep			m_AniBlendStep;public eActionStep AniBlendStep{get{return m_AniBlendStep;}}
	eNextIdleIndex		m_NextIdleIndex;public eNextIdleIndex NextIdleIndex{get{return m_NextIdleIndex;}}
//	float				m_ActionSpeed = 1000f;public float ActionSpeed{get{return m_ActionSpeed;}}
	
	Tbl_Action_ReadyAnimation	m_ReadyAnimation;public Tbl_Action_ReadyAnimation ReadyAnimation{get{return m_ReadyAnimation;}}
	Tbl_Action_HitAnimation	m_HitAnimation;public Tbl_Action_HitAnimation HitAnimation{get{return m_HitAnimation;}}
	Tbl_Action_Animation	m_FinishAnimation;public Tbl_Action_Animation FinishAnimation{get{return m_FinishAnimation;}}
	
	public Tbl_Action_Record(XmlElement _element, eType _type)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_ActionName, node, "ActionName");
			if(_type == eType.User)
			{
				SetValue<eCLASS>(ref m_Class, node, "Class");
				SetValue<eGENDER>(ref m_Gender, node, "Gender");
			}
			else
			{
				SetValue(ref m_ClassName, node, "Class");
			}
				
			SetValue(ref m_LinkActionIndex, node, "LinkActionIndex");
			SetValue(ref m_AniBlendingDuration, node, "AniBlendingDuration");
			SetValue<eNextIdleIndex>(ref m_NextIdleIndex, node, "NextIdleIndex");
//			SetValue(ref m_ActionSpeed, node, "ActionSpeed");
			
			XmlNode ready = node.SelectSingleNode("Ready");
			XmlNode hit = node.SelectSingleNode("Hit");
			XmlNode finish = node.SelectSingleNode("Finish");
			
			#region - ready & hit & finish -
			if(ready != null)
			{
				int[] counts = new int[4];
				m_ReadyAnimation = CreateActionReadyAnimation(ref ready, ref counts);
				
				List<Tbl_Action_Effect> listEffect = GetEffectElements(ref ready, counts[(int)eElementSequence.Effect], _type);
				List<Tbl_Action_Sound> listSound = GetSoundElements(ref ready, counts[(int)eElementSequence.Sound]);
				List<Tbl_Action_Trail> listTrail = GetTrailElements(ref ready, counts[(int)eElementSequence.Trail]);
				List<Tbl_Action_Cancel> listCancel = GetCancelElements(ref ready, counts[(int)eElementSequence.Cancel]);
				
				m_ReadyAnimation.SetEffect(listEffect);
				m_ReadyAnimation.SetSound(listSound);
				m_ReadyAnimation.SetTrail(listTrail);
				m_ReadyAnimation.SetCancel(listCancel);
			}
			
			if(hit != null)
			{
				int[] counts = new int[4];
				m_HitAnimation = CreateActionHitAnimation(ref hit, ref counts);
				
//				m_HitAnimation.SetMoveInfo(
				
				List<Tbl_Action_Effect> listEffect = GetEffectElements(ref hit, counts[(int)eElementSequence.Effect], _type);
				List<Tbl_Action_Sound> listSound = GetSoundElements(ref hit, counts[(int)eElementSequence.Sound]);
				List<Tbl_Action_Trail> listTrail = GetTrailElements(ref hit, counts[(int)eElementSequence.Trail]);
				List<Tbl_Action_Cancel> listCancel = GetCancelElements(ref hit, counts[(int)eElementSequence.Cancel]);
				
				m_HitAnimation.SetEffect(listEffect);
				m_HitAnimation.SetSound(listSound);
				m_HitAnimation.SetTrail(listTrail);
				m_HitAnimation.SetCancel(listCancel);
				
				if(hit.NextSibling != null)
					m_HitAnimation.SetHitInfo(new Tbl_Action_HitInfo(hit.NextSibling));
			}
			
			if(finish != null)
			{
				int[] counts = new int[4];
				m_FinishAnimation = CreateActionAnimation(ref finish, ref counts);
				
				List<Tbl_Action_Effect> listEffect = GetEffectElements(ref finish, counts[(int)eElementSequence.Effect], _type);
				List<Tbl_Action_Sound> listSound = GetSoundElements(ref finish, counts[(int)eElementSequence.Sound]);
				List<Tbl_Action_Trail> listTrail = GetTrailElements(ref finish, counts[(int)eElementSequence.Trail]);
				List<Tbl_Action_Cancel> listCancel = GetCancelElements(ref finish, counts[(int)eElementSequence.Cancel]);
				
				m_FinishAnimation.SetEffect(listEffect);
				m_FinishAnimation.SetSound(listSound);
				m_FinishAnimation.SetTrail(listTrail);
				m_FinishAnimation.SetCancel(listCancel);
			}
			#endregion
			
			DecideBlendingStep();
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	#region - generate anim instance -
	Tbl_Action_Animation CreateActionAnimation(ref XmlNode _node, ref int[] _counts)
	{
		_node = _node.FirstChild;
		
		string fileName = "NONE";
		float actionSpeed = 1000f;
		eLoopType loopType = eLoopType.NONE;
		float loopTargetTime = 0;
		float loopDuration = 0;
		float animationLength = 0;
		
		SetAttribute(ref fileName, _node, "FileName");
		SetAttribute(ref actionSpeed, _node, "ActionSpeed");
		SetAttribute<eLoopType>(ref loopType, _node, "LoopType");
		SetAttribute(ref loopTargetTime, _node, "LoopTargetTime");
		SetAttribute(ref loopDuration, _node, "LoopDuration");
		SetAttribute(ref animationLength, _node, "AnimationLength");
		
		SetAttribute(ref _counts[(int)eElementSequence.Effect], _node, "EffectListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Sound], _node, "SoundListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Trail], _node, "TrailListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Cancel], _node, "ActionCancelListCount");
		
		return new Tbl_Action_Animation(fileName, actionSpeed, loopType, loopTargetTime, loopDuration, animationLength);
	}
	Tbl_Action_ReadyAnimation CreateActionReadyAnimation(ref XmlNode _node, ref int[] _counts)
	{
		_node = _node.FirstChild;
		
		string fileName = "NONE";
		float actionSpeed = 1000f;
		eLoopType loopType = eLoopType.NONE;
		float loopTargetTime = 0;
		float loopDuration = 0;
		float animationLength = 0;
		
		eReadyMoveType moveType = eReadyMoveType.NONE;
		float moveDistance = 0;
		
		SetAttribute(ref fileName, _node, "FileName");
		SetAttribute(ref actionSpeed, _node, "ActionSpeed");
		SetAttribute<eLoopType>(ref loopType, _node, "LoopType");
		SetAttribute(ref loopTargetTime, _node, "LoopTargetTime");
		SetAttribute(ref loopDuration, _node, "LoopDuration");
		SetAttribute(ref animationLength, _node, "AnimationLength");
		
		SetAttribute<eReadyMoveType>(ref moveType, _node, "MoveType");
		SetAttribute(ref moveDistance, _node, "MoveDistance");
		
		SetAttribute(ref _counts[(int)eElementSequence.Effect], _node, "EffectListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Sound], _node, "SoundListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Trail], _node, "TrailListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Cancel], _node, "ActionCancelListCount");
		
		return new Tbl_Action_ReadyAnimation(fileName, actionSpeed, loopType, loopTargetTime, loopDuration, animationLength, moveType, moveDistance);//
	}
	Tbl_Action_HitAnimation CreateActionHitAnimation(ref XmlNode _node, ref int[] _counts)
	{
		_node = _node.FirstChild;
		
		string fileName = "NONE";
		float actionSpeed = 1000f;
		eLoopType loopType = eLoopType.NONE;
		float loopTargetTime = 0;
		float loopDuration = 0;
		float animationLength = 0;
		
		eHitMoveType moveType = eHitMoveType.NONE;
		float moveDistance = 0;
		
		SetAttribute(ref fileName, _node, "FileName");
		SetAttribute(ref actionSpeed, _node, "ActionSpeed");
		SetAttribute<eLoopType>(ref loopType, _node, "LoopType");
		SetAttribute(ref loopTargetTime, _node, "LoopTargetTime");
		SetAttribute(ref loopDuration, _node, "LoopDuration");
		SetAttribute(ref animationLength, _node, "AnimationLength");
		
		SetAttribute<eHitMoveType>(ref moveType, _node, "MoveType");
		SetAttribute(ref moveDistance, _node, "MoveDistance");
		
		SetAttribute(ref _counts[(int)eElementSequence.Effect], _node, "EffectListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Sound], _node, "SoundListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Trail], _node, "TrailListCount");
		SetAttribute(ref _counts[(int)eElementSequence.Cancel], _node, "ActionCancelListCount");
		
		return new Tbl_Action_HitAnimation(fileName, actionSpeed, loopType, loopTargetTime, loopDuration, animationLength, moveType, moveDistance);//
	}
	#endregion
	#region - getting elements -
	List<Tbl_Action_Effect> GetEffectElements(ref XmlNode _next, int _count, eType _type)
	{
		List<Tbl_Action_Effect> listEffect = new List<Tbl_Action_Effect>();
		
		for(int i=0; i<_count; ++i)
		{
			_next = _next.NextSibling;
			
			float fxTiming = 0;
			string fxName = "NONE";
			eLinkType fxLink = eLinkType.NONE;
			eLoopType fxLoop = eLoopType.NONE;
			float fxDuration = 0;
			bool positionFix = false;
			float startSize = 1;
			
			SetAttribute(ref fxTiming, _next, "Timing");
			SetAttribute(ref fxName, _next, "FileName");
			SetAttribute<eLinkType>(ref fxLink, _next, "LinkType");
			SetAttribute<eLoopType>(ref fxLoop, _next, "LoopType");
			SetAttribute(ref fxDuration, _next, "LoopDuration");
			SetAttribute(ref positionFix, _next, "PositionFix");
//			if(_type == eType.User)
				SetAttribute(ref startSize, _next, "StartSize");
			
			listEffect.Add(new Tbl_Action_Effect(this, fxTiming, fxName, fxLink, fxLoop, fxDuration, positionFix, startSize));
		}
		
		return listEffect;
	}
	List<Tbl_Action_Effect> GetEffectElements(ref XmlNode _next, int _count)
	{
		return GetEffectElements(ref _next, _count);
	}
	List<Tbl_Action_Sound> GetSoundElements(ref XmlNode _next, int _count)
	{
		List<Tbl_Action_Sound> listSound = new List<Tbl_Action_Sound>();
		
		for(int i=0; i<_count; ++i)
		{
			_next = _next.NextSibling;
			
			float sndTiming = 0;
			string sndName = "NONE";
			eLoopType sndLoop = eLoopType.NONE;
			
			SetAttribute(ref sndTiming, _next, "Timing");
			SetAttribute(ref sndName, _next, "FileName");
			SetAttribute<eLoopType>(ref sndLoop, _next, "LoopType");
			
			listSound.Add(new Tbl_Action_Sound(sndTiming, sndName, sndLoop));
		}
		
		return listSound;
	}
	List<Tbl_Action_Trail> GetTrailElements(ref XmlNode _next, int _count)
	{
		List<Tbl_Action_Trail> listTrail = new List<Tbl_Action_Trail>();
		
		for(int i=0; i<_count; ++i)
		{
			_next = _next.NextSibling;
			
			string trlName = "NONE";
			float trlStart = 0;
			float trlEnd = 0;
			float trlLoopduration = 0;
			
			SetAttribute(ref trlName, _next, "TextureName");
			SetAttribute(ref trlStart, _next, "StartTime");
//			SetAttribute(ref trlEnd, _next, "EndTime");
			SetAttribute(ref trlLoopduration, _next, "LoopDuration");
			
			listTrail.Add(new Tbl_Action_Trail(trlName, trlStart, trlEnd, trlLoopduration));
		}
		
		return listTrail;
	}
	List<Tbl_Action_Cancel> GetCancelElements(ref XmlNode _next, int _count)
	{
		List<Tbl_Action_Cancel> listCancel = new List<Tbl_Action_Cancel>();
		
		for(int i=0; i<_count; ++i)
		{
			_next = _next.NextSibling;
			
			float cancelStart = 0;
			float cancelEnd = 0;
			
			SetAttribute(ref cancelStart, _next, "StartTime");
			SetAttribute(ref cancelEnd, _next, "EndTime");
			
			listCancel.Add(new Tbl_Action_Cancel(cancelStart, cancelEnd));
		}
		
		return listCancel;
	}
	#endregion
	#region - method -
	public bool GetCancelEnable(eActionStep _step, float _elapsedTime)
	{
		List<Tbl_Action_Cancel> list = null;
		bool ret = true;
		
		switch(_step)
		{
		case eActionStep.Ready:
			list = m_ReadyAnimation.listCancel;
			break;
		case eActionStep.Hit:
			list = m_HitAnimation.listCancel;
			break;
		case eActionStep.Finish:
			list = m_FinishAnimation.listCancel;
			break;
		default:
			Debug.LogError("Tbl_Action_Recrod::GetCancelEnable: invalid action step[" + _step + "]");
			return false;
		}
		
		foreach(Tbl_Action_Cancel cancel in list)
		{
			if(cancel.StartTime < _elapsedTime && _elapsedTime < cancel.EndTime)
			{
				ret = false;
				break;
			}
		}
		
		return ret;
	}
	
	void DecideBlendingStep()
	{
		if(m_FinishAnimation != null)
			m_AniBlendStep = eActionStep.Finish;
		else if(m_HitAnimation != null)
			m_AniBlendStep = eActionStep.Hit;
		else if(m_ReadyAnimation != null)
			m_AniBlendStep = eActionStep.Ready;
	}
	#endregion
	#region - getter -
	public bool CheckNonAnimation()
	{
		if(m_HitAnimation != null &&
			m_HitAnimation.FileName == "NonAnimation")
			return true;
		else
			return false;
	}
	#endregion
}

public class Tbl_Action_Table : AsTableBase {
	SortedList<int, Tbl_Action_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Action_Record>();

	Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, Tbl_Action_Record>>> m_dddicAction = 
		new Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, Tbl_Action_Record>>>();
	
	Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, Tbl_Action_Animation>>> m_dddicActionAnim =
		new Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, Tbl_Action_Animation>>>();
	
	public Tbl_Action_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		string className = "class is not set";
		string actionName = "action is not set";

		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_Action_Record record = new Tbl_Action_Record((XmlElement)node, Tbl_Action_Record.eType.User);
				m_ResourceTable.Add(record.Index, record);
				
				eCLASS _class = record.Class;

				if(m_dddicAction.ContainsKey(_class) == false)
					m_dddicAction.Add(_class, new Dictionary<eGENDER, Dictionary<string, Tbl_Action_Record>>());
				if(m_dddicAction[_class].ContainsKey(record.Gender) == false)
					m_dddicAction[_class].Add(record.Gender, new Dictionary<string, Tbl_Action_Record>());

				className = _class.ToString();
				actionName = record.ActionName;

				m_dddicAction[_class][record.Gender].Add(record.ActionName, record);
//				Debug.Log("Loading ActionListList table : Class:" + _class + ", action:" + act);

				if(m_dddicActionAnim.ContainsKey(_class) == false)
					m_dddicActionAnim.Add(_class, new Dictionary<eGENDER, Dictionary<string, Tbl_Action_Animation>>());
				if(m_dddicActionAnim[_class].ContainsKey(record.Gender) == false)
					m_dddicActionAnim[_class].Add(record.Gender, new Dictionary<string, Tbl_Action_Animation>());
				
				if(record.ReadyAnimation != null && m_dddicActionAnim[_class][record.Gender].ContainsKey(record.ReadyAnimation.FileName) == false)
					m_dddicActionAnim[_class][record.Gender].Add(record.ReadyAnimation.FileName, record.ReadyAnimation);
				if(record.HitAnimation != null && m_dddicActionAnim[_class][record.Gender].ContainsKey(record.HitAnimation.FileName) == false)
					m_dddicActionAnim[_class][record.Gender].Add(record.HitAnimation.FileName, record.HitAnimation);
				if(record.FinishAnimation != null && m_dddicActionAnim[_class][record.Gender].ContainsKey(record.FinishAnimation.FileName) == false)
					m_dddicActionAnim[_class][record.Gender].Add(record.FinishAnimation.FileName, record.FinishAnimation);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("Tbl_ActionList_Table::LoadTable: same named data is found. className = " + className + ", actionName = " + actionName);
		}
	}
	
	public Tbl_Action_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogWarning("[Tbl_ActionList_Table]GetRecord: there is no record = " + _id);
		return null;
	}
	
	public Tbl_Action_Record GetRecord(eCLASS _class, eGENDER _gender, string _act)
	{
		if(m_dddicAction.ContainsKey(_class) == true)
		{
			if(m_dddicAction[_class].ContainsKey(_gender) == true)
			{
				if(m_dddicAction[_class][_gender].ContainsKey(_act) == true)
				{
					return m_dddicAction[_class][_gender][_act];
				}
			}
		}
		
		Debug.LogError("[Tbl_ActionList_Table]GetRecord: there is no record = " + _class + ", " + _gender + ", " + _act);
		return null;
	}
	
	public SortedList<int, Tbl_Action_Record> GetTable()
	{
		return m_ResourceTable;
	}
	
	public Tbl_Action_Animation GetActionAnimation(eCLASS _class, eGENDER _gender, string _animName)
	{
		if(m_dddicActionAnim.ContainsKey(_class) == true)
		{
			if(m_dddicActionAnim[_class].ContainsKey(_gender) == true)
			{
				if(m_dddicActionAnim[_class][_gender].ContainsKey(_animName) == true)
				{
					return m_dddicActionAnim[_class][_gender][_animName];
				}
			}
		}
		
//		Debug.LogError("[Tbl_ActionList_Table]GetActionAnimInfo: " + _class + "(" + _gender + ") doesnt have " + _animName + " animation");
		return null;
	}
}
