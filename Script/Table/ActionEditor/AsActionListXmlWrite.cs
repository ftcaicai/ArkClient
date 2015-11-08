#define _USE_AREALAYER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
//using UnityEditor;



[Serializable]
public class AsAction_Record  : AsTableRecord
{
		
	public enum eNEXT_ANIMATION
	{
		None,
		Idle,
		BattleIdle,		
	}	
	int seq = 0;
	public int Seq
	{
		get{ return seq;}
		set{ seq = value;}
	}
	public int index = 0;
	
	
	public string actionName;
	
	[HideInInspector] public int classIndex;// = eCLASS_TYPE.DivineKnight;
	[HideInInspector] public string classType;// = eCLASS_TYPE.DivineKnight;
	[HideInInspector] public eGENDER gender = eGENDER.eGENDER_MALE;
	public int linkActionIndex = 0;
	public int aniBlendingDuration  = 0; //ms
	public eNEXT_ANIMATION nextIdleIndex = eNEXT_ANIMATION.None;

	public ReadyAnimationInfo   readyAnimationInfoData = new ReadyAnimationInfo();
	public HitAnimationInfo  	hitAnimationInfoData = new HitAnimationInfo();
	public FinishAnimationInfo  finishAnimationInfoData = new FinishAnimationInfo();
	
	

	 public bool FirstChild( XmlElement _element, string name)
    {
        if (_element == null)
        {
            Console.WriteLine("XmlReader::FirstChild() : current_ is null");
            return false;
        }

        foreach(XmlNode node in _element.ChildNodes)
        {
            if(node.Name == name)
            {
                _element = (XmlElement)node;
                return true;
            }
        }
        return false;
    }

	 public bool NextSibling(XmlElement _element)
    {
        if (_element == null)
        {
            Debug.Log("XmlReader::NextSibling() : current_ is null");
            return false;
        }

        XmlElement originElement = _element;
        string curName = _element.Name;

        //while (true)
        //{
            _element = (XmlElement)_element.NextSibling;

            if (_element == null)
            {
                //Debug.Log("XmlReader::NextSibling() : no next sibling");
                _element = originElement;
                return false;
            }else if (_element.Name != curName)
            {
                //Debug.Log("XmlReader::NextSibling() : no same name Element in the parent");
                _element = originElement;
                return false;
            }

            if (_element.Name == curName)
                return true;
        //}
		return false;
    }
	
	void ReadAttribute<T>(ref T _target, XmlElement _element, string _column,  string _attribute)
	{
		if(typeof(T).IsEnum == false)
		{
			Debug.LogWarning("AsTableRecord::SetEnumValue: target value must be Enum");
			return;
		}
		
		try{		
			_target = (T)Enum.Parse(typeof(T), _element[_column].GetAttribute(_attribute), true);
		}
		catch
		{
			//Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
			
			if(_element[_column] == null)
			{
				Debug.LogWarning("AsTableRecord::SetValue: [" + _column + "] setting default");
				_target = default(T);
			}
			else
			{
				Debug.LogError("AsTableRecord::SetEnumValue: " +
					"invalid enum parsing on xmlnode(node[" + _column + "] | value:" +
					_element[_column].InnerText);
			}
		}
	}
	protected void ReadAttribute(ref int _target,XmlElement _element, string _column,  string _attribute)
	{
		try{
			_target = int.Parse(_element[_column].GetAttribute(_attribute));
		}
		catch
		{
			//Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
			
			if(string.Compare("NONE", _element[_column].InnerText, true) != 0)
			{
				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_element[_column].InnerText);
			}
				
			_target = 0;
		}
	}
	
	protected void ReadAttribute(ref float _target,XmlElement _element, string _column,  string _attribute)
	{
		try{
			_target = float.Parse(_element[_column].GetAttribute(_attribute));
		}
		catch
		{
			//Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
			
			if(string.Compare("NONE", _element[_column].InnerText, true) != 0)
			{
				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_element[_column].InnerText);
			}
				
			_target = 0;
		}
	}
	
	
	protected void ReadAttribute(ref bool _target, XmlElement _element, string _column,  string _attribute, bool _def)
	{
	
		
		try{
			_target = bool.Parse(_element[_column].GetAttribute(_attribute));
		}
		catch
		{
			Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_element[_column].InnerText);

			_target = _def;
		}
	}
	
	public AsAction_Record()// : base(_element)
	{
	}
	
	public void Load_HitInfo( XmlElement _element, HitInfo data )
	{
		string strElement = "HitInfo";
		ReadAttribute<HitInfo.eHIT_TYPE>(ref data.hitType, _element, strElement , "HitType");		
#if _USE_AREALAYER		
		ReadAttribute(ref data.multiTargetCount , _element, strElement, "MultiTargetCount");		
#endif		
		ReadAttribute(ref data.timing , _element, strElement, "Timing");
		
		data.projectileFilePathName  =  _element[strElement].GetAttribute("ProjectileFilePath");
		GameObject obj = ResourceLoad.LoadGameObject(data.projectileFilePathName);
		if(obj != null)
		{
				data.projectileFilePath = obj;
		}
		ReadAttribute(ref data.projectileSpeed , _element, strElement, "ProjectileSpeed");
		ReadAttribute(ref data.projectileAcceleration , _element, strElement, "ProjectileAcceleration");
		ReadAttribute<eProjectilePath>(ref data.projectilePath, _element, strElement , "ProjectilePath");	
		data.projectileHitFilePathName  =  _element[strElement].GetAttribute("ProjectileHitFilePath");
		GameObject hitObj = ResourceLoad.LoadGameObject(data.projectileHitFilePathName);
		if(obj != null)
		{
				data.projectileHitFilePath = hitObj;
		}		
#if !_USE_AREALAYER			
		ReadAttribute<HitInfo.eAREA_SHAPE>(ref data.areaShape, _element, strElement , "AreaShape");	
	
		ReadAttribute(ref data.angle , _element, strElement, "Angle");
		ReadAttribute(ref data.centerDirectionAngle , _element, strElement, "CenterDirectionAngle");	
		ReadAttribute(ref data.minDistance , _element, strElement, "MinDistance");
		ReadAttribute(ref data.maxDistance , _element, strElement, "MaxDistance");
		ReadAttribute(ref data.width , _element, strElement, "Width");
		ReadAttribute(ref data.height, _element, strElement, "Height");
		ReadAttribute(ref data.offsetX , _element, strElement, "OffsetX");
		ReadAttribute(ref data.offsetY , _element, strElement, "OffsetY");
#endif		
		ReadAttribute(ref data.valuePercent, _element, strElement, "ValuePercent");
		ReadAttribute(ref data.valueLookType , _element, strElement, "ValueLookType");
		ReadAttribute(ref data.valueLookDuration, _element, strElement, "ValueLookDuration");
		ReadAttribute(ref data.valueLookCount , _element, strElement, "ValueLookCount");		
		
		
#if _USE_AREALAYER	
		int areaListCount  = int.Parse( _element[strElement].GetAttribute("AreaListCount"));
		if( areaListCount > 0)
		{
			Load_AreaInfo(_element,data.areaLayer, areaListCount);
		}
#endif		
		data.projectileHitSoundPathName  =  _element[strElement].GetAttribute("ProjectileHitSoundPath");
		AudioClip hitSoundObj = ResourceLoad.LoadAudioClip(data.projectileHitSoundPathName);	
		if(hitSoundObj != null)
		{
			data.projectileHitSoundPath = hitSoundObj;
		}
		
		
	}	
#if _USE_AREALAYER		
	public void Load_AreaInfo( XmlElement _element , List<AreaInfo> areaLayer, int _count )
	{
		
		//AreaInfo		
		for( int index = 0; index < _count; ++index )
		{
			AreaInfo data = new AreaInfo();
			string strElement = "AreaInfo"+ (1+index);		
			
			ReadAttribute<HitInfo.eAREA_SHAPE>(ref data.areaShape, _element, strElement , "AreaShape");	
	
			ReadAttribute(ref data.angle , _element, strElement, "Angle");
			ReadAttribute(ref data.centerDirectionAngle , _element, strElement, "CenterDirectionAngle");	
			ReadAttribute(ref data.minDistance , _element, strElement, "MinDistance");
			ReadAttribute(ref data.maxDistance , _element, strElement, "MaxDistance");
			ReadAttribute(ref data.width , _element, strElement, "Width");
			ReadAttribute(ref data.height, _element, strElement, "Height");
			ReadAttribute(ref data.offsetX , _element, strElement, "OffsetX");
			ReadAttribute(ref data.offsetY , _element, strElement, "OffsetY");
			
			
						
			areaLayer.Add(data);
				
		}
	}	
#endif	
	
	
	public void Load_EffectInfo( XmlElement _element , List<EffectInfo> effectLayer, int _count )
	{
		
		//EffectInfo		
		for( int index = 0; index < _count; ++index )
		{
			EffectInfo data = new EffectInfo();
			string strElement = "EffectInfo"+ (1+index);
			
			
			ReadAttribute(ref data.timing , _element, strElement, "Timing");
			data.filePathName  =  _element[strElement].GetAttribute("FileName");
			GameObject obj = ResourceLoad.LoadGameObject(data.filePathName);
			if(obj != null)
			{
				data.filePath = obj;
			}
			
			ReadAttribute<EffectInfo.eLINK_TYPE>(ref data.linkType, _element, strElement , "LinkType");
			ReadAttribute<EffectInfo.eLOOP_TYPE>(ref data.loopType, _element, strElement , "LoopType");
			ReadAttribute<EffectInfo.eLOOP_TYPE>(ref data.loopType, _element, strElement , "LoopType");
			data.loopDuration =  int.Parse(_element[strElement].GetAttribute("LoopDuration"));
			ReadAttribute(ref data.positionFix , _element, strElement , "PositionFix", false);
		
			data.startSize =  float.Parse(_element[strElement].GetAttribute("StartSize"));	
						
			effectLayer.Add(data);
				
		}
	}	
	
	public void Load_SoundInfo( XmlElement _element ,  List<SoundInfo> soundLayer , int _count )
	{	
		//SoundInfo		
		for( int index = 0; index < _count; ++index )
		{
			SoundInfo data = new SoundInfo();
			string strElement = "SoundInfo"+ (1+index);
		
			ReadAttribute(ref data.timing , _element, strElement, "Timing");	
			data.filePathName  =  _element[strElement].GetAttribute("FileName");
			AudioClip obj = ResourceLoad.LoadAudioClip(data.filePathName);
			if(obj != null)
			{
				data.filePath = (AudioClip)obj;
			}		
			
		//	AudioClip clip = Resources.Load( "Sound/" + _filename ) as AudioClip;
			
			ReadAttribute<SoundInfo.eLOOP_TYPE>(ref data.loopType, _element, strElement , "LoopType");
	
			soundLayer.Add(data);
				
		}
	}	
	public void Load_TrailInfo( XmlElement _element ,  List<TrailInfo> trailLayer , int _count )
	{	
		//TrailInfo		
		for( int index = 0; index < _count; ++index )
		{
			TrailInfo data = new TrailInfo();
			string strElement = "TrailInfo"+ (1+index);
			
		
			data.filePathName  =  _element[strElement].GetAttribute("TextureName");
			Texture obj = ResourceLoad.Loadtexture(data.filePathName);
			if(obj != null)
			{
				data.textureName = obj;
			}		
			ReadAttribute(ref data.startTime , _element, strElement, "StartTime");
			ReadAttribute(ref data.loopDuration , _element, strElement, "LoopDuration");		
		
			trailLayer.Add(data);
				
		}
	}	
	
	public void Load_ActionCancelInfo(  XmlElement _element, List<ActionCancelInfo> actionCancelLayer, int _count)
	{	
		//ActionCancelInfo		
		for( int index = 0; index < _count; ++index )
		{
			ActionCancelInfo data = new ActionCancelInfo();
			string strElement = "ActionCancel"+ (1+index);
			
					
			ReadAttribute(ref data.startTime , _element, strElement, "StartTime");
			ReadAttribute(ref data.endTime , _element, strElement, "EndTime");
		
			actionCancelLayer.Add(data);
				
		}
	}	
	
	public AsAction_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
		
			SetValue(ref index, node, "Index");
			SetValue(ref actionName, node, "ActionName");
			SetValue(ref classType, node, "Class");
			if(node.SelectSingleNode("Gender") != null )
			{	
				SetValue(ref gender, node, "Gender");
			}
			SetValue(ref linkActionIndex, node, "LinkActionIndex");
			SetValue(ref aniBlendingDuration, node, "AniBlendingDuration");
			SetValue<eNEXT_ANIMATION>(ref nextIdleIndex, node, "NextIdleIndex");
			
		
			
			if(node.SelectSingleNode("Ready") != null )
			{	
				
				_element = (XmlElement) node["Ready"];
				if(FirstChild (_element, "ReadyAnimation"))
				{
			
					readyAnimationInfoData.filePathName  = _element["ReadyAnimation"].GetAttribute("FileName");
#if _USE_AREALAYER					
					ReadAttribute(ref readyAnimationInfoData.actionSpeed , _element, "ReadyAnimation", "ActionSpeed");
#endif					
					ReadAttribute<ReadyAnimationInfo.eLOOP_TYPE>(ref readyAnimationInfoData.loopType, _element, "ReadyAnimation", "LoopType");					
					ReadAttribute(ref readyAnimationInfoData.loopTargetTime , _element, "ReadyAnimation", "LoopTargetTime");
					ReadAttribute(ref readyAnimationInfoData.loopDuration , _element, "ReadyAnimation", "LoopDuration");
					ReadAttribute(ref readyAnimationInfoData.animationLength , _element, "ReadyAnimation", "AnimationLength");
					ReadAttribute<HitAnimationInfo.eMOVE_TYPE>(ref readyAnimationInfoData.moveType, _element, "ReadyAnimation", "MoveType");					
					ReadAttribute(ref readyAnimationInfoData.moveDistance , _element, "ReadyAnimation", "MoveDistance");	
			
					
					int effectListCount =  int.Parse(_element["ReadyAnimation"].GetAttribute("EffectListCount"));
					int soundListCount  = int.Parse(_element["ReadyAnimation"].GetAttribute("SoundListCount"));
					int trailListCount  = int.Parse(_element["ReadyAnimation"].GetAttribute("TrailListCount"));
					int actionCancelListCount  = int.Parse(_element["ReadyAnimation"].GetAttribute("ActionCancelListCount"));
					
					if(effectListCount > 0)
					{
						Load_EffectInfo(_element,readyAnimationInfoData.effectLayer, effectListCount);
					}
					if(soundListCount > 0)
					{
						Load_SoundInfo(_element,readyAnimationInfoData.soundLayer, soundListCount);
					}
					if(trailListCount > 0)
					{
						Load_TrailInfo(_element,readyAnimationInfoData.trailLayer, trailListCount);
					}
					if(actionCancelListCount > 0)
					{
						Load_ActionCancelInfo(_element,readyAnimationInfoData.actionCancelDisableLayer, actionCancelListCount);
					}
				}
			}
			if(node.SelectSingleNode("Hit") != null )
			{
				_element = (XmlElement) node["Hit"];
				if(FirstChild (_element, "HitAnimation") )
				{
				
					hitAnimationInfoData.filePathName  = _element["HitAnimation"].GetAttribute("FileName");
#if _USE_AREALAYER
					ReadAttribute(ref hitAnimationInfoData.actionSpeed , _element, "HitAnimation", "ActionSpeed");
#endif					
					ReadAttribute<HitAnimationInfo.eLOOP_TYPE>(ref hitAnimationInfoData.loopType, _element, "HitAnimation", "LoopType");
					ReadAttribute(ref hitAnimationInfoData.loopTargetTime , _element, "HitAnimation", "LoopTargetTime");
					ReadAttribute(ref hitAnimationInfoData.loopDuration , _element, "HitAnimation", "LoopDuration");
					ReadAttribute(ref hitAnimationInfoData.animationLength , _element, "HitAnimation", "AnimationLength");	
					
					ReadAttribute<HitAnimationInfo.eMOVE_TYPE>(ref hitAnimationInfoData.moveType, _element, "HitAnimation", "MoveType");					
					ReadAttribute(ref hitAnimationInfoData.moveDistance , _element, "HitAnimation", "MoveDistance");	
					
				

					
					int effectListCount  = int.Parse(_element["HitAnimation"].GetAttribute("EffectListCount"));
					int soundListCount  = int.Parse(_element["HitAnimation"].GetAttribute("SoundListCount"));
					int trailListCount  = int.Parse(_element["HitAnimation"].GetAttribute("TrailListCount"));
					int actionCancelListCount  = int.Parse(_element["HitAnimation"].GetAttribute("ActionCancelListCount"));
					
					if(effectListCount > 0)
					{
						Load_EffectInfo(_element,hitAnimationInfoData.effectLayer, effectListCount);
					}
					if(soundListCount > 0)
					{
						
						Load_SoundInfo(_element,hitAnimationInfoData.soundLayer, soundListCount);
					}
					if(trailListCount > 0)
					{
						Load_TrailInfo(_element,hitAnimationInfoData.trailLayer, trailListCount);
					}
					if(actionCancelListCount > 0)
					{
						Load_ActionCancelInfo(_element,hitAnimationInfoData.actionCancelDisableLayer, actionCancelListCount);
					}
			
					if( FirstChild (_element, "HitInfo") )
						Load_HitInfo(_element,hitAnimationInfoData.hitInfoData );
				
				}
			}
			
			
			if(node.SelectSingleNode("Finish") != null )
			{
				_element = (XmlElement) node["Finish"];
				if( FirstChild (_element, "FinishAnimation")   )
				{                                                     
				
					finishAnimationInfoData.filePathName  = _element["FinishAnimation"].GetAttribute("FileName");
#if _USE_AREALAYER
					ReadAttribute(ref finishAnimationInfoData.actionSpeed , _element, "FinishAnimation", "ActionSpeed");
#endif
					ReadAttribute<FinishAnimationInfo.eLOOP_TYPE>(ref finishAnimationInfoData.loopType, _element, "FinishAnimation", "LoopType");
					ReadAttribute(ref finishAnimationInfoData.loopTargetTime , _element, "FinishAnimation", "LoopTargetTime");
					ReadAttribute(ref finishAnimationInfoData.loopDuration , _element, "FinishAnimation", "LoopDuration");
					ReadAttribute(ref finishAnimationInfoData.animationLength , _element, "FinishAnimation", "AnimationLength");			
					
					int effectListCount  = int.Parse(_element["FinishAnimation"].GetAttribute("EffectListCount"));
					int soundListCount  = int.Parse(_element["FinishAnimation"].GetAttribute("SoundListCount"));
					int trailListCount  = int.Parse(_element["FinishAnimation"].GetAttribute("TrailListCount"));
					int actionCancelListCount  = int.Parse(_element["FinishAnimation"].GetAttribute("ActionCancelListCount"));
					
					if(effectListCount > 0)
					{
						Load_EffectInfo(_element,finishAnimationInfoData.effectLayer, effectListCount);
					}
					if(soundListCount > 0)
					{
						Load_SoundInfo(_element,finishAnimationInfoData.soundLayer, soundListCount);
					}
					if(trailListCount > 0)
					{
						Load_TrailInfo(_element,finishAnimationInfoData.trailLayer, trailListCount);
					}
					if(actionCancelListCount > 0)
					{
						Load_ActionCancelInfo(_element,finishAnimationInfoData.actionCancelDisableLayer, actionCancelListCount);
					}
				}			
			
			}
		}
		catch(System.Exception e)
		{	
			Debug.LogError(e);
		}
	}
	
};

[Serializable]
public class ActionRecordInfo 
{		
	public enum eCLASS_TYPE
	{
		DivineKnight,
		Magician,		
	}
	
	public enum eNEXT_ANIMATION
	{
		None,
		Idle,
		BattleIdle,		
	}	
	int seq = 0;
	public int Seq
	{
		get{ return seq;}
		set{ seq = value;}
	}
	public int index = 0;
	
	
	public string actionName;
	public eCLASS_TYPE classType = eCLASS_TYPE.DivineKnight;
	public eGENDER gender = eGENDER.eGENDER_MALE;
	public int linkActionIndex = 0;
	public int AniBlendingDuration  = 0; //ms
	public eNEXT_ANIMATION NextIdleIndex = eNEXT_ANIMATION.None;
}


[Serializable]
public class EffectInfo 
{		
	public enum eLINK_TYPE
	{
		AttachDummy,
		HoldPosition,
		HitPosition,
		TargetChain,
		Shooting,
		ShootingPosition,
		HitHoldPosition,

	}	
	public enum eLOOP_TYPE
	{
		Once,
		Loop,
		TimeLoop,	
		Cut,
	}
	[SerializeField]public int  timing  = 0;	
	[SerializeField]public GameObject filePath = null;
	[HideInInspector]public string filePathName = null;
	[SerializeField]public eLINK_TYPE linkType = eLINK_TYPE.AttachDummy;	
	[SerializeField]public eLOOP_TYPE loopType = eLOOP_TYPE.Once;	
	[SerializeField]public int loopDuration = 0;	
	[SerializeField]public bool positionFix = false;	
	[SerializeField]public float startSize  = 1.0f;	
	
	[HideInInspector]public bool m_bPlay = false;
}


[Serializable]
public class SoundInfo 
{	
		
	public enum eLOOP_TYPE
	{
		Once,
		Loop,		
		Once_Cycle,
	}
	[SerializeField]public int  timing  = 0;	
	[SerializeField]public AudioClip filePath  = null;
	[HideInInspector]public string filePathName = null;
	[SerializeField]public eLOOP_TYPE loopType = eLOOP_TYPE.Once;		
	
	[HideInInspector]public bool m_bPlay = false;
}

[Serializable]
public class TrailInfo 
{	
	[SerializeField] public Texture textureName = null;
	[HideInInspector]public string filePathName = null;
	[SerializeField] public int  startTime  = 0;	
	[SerializeField] public int  loopDuration    = 0;	
	
	[HideInInspector]public bool m_bPlay = false;
	
}

[Serializable]
public class ActionCancelInfo 
{	
	[SerializeField] public int  startTime  = 0;	
	[SerializeField] public int  endTime    = 0;		
}

#if _USE_AREALAYER
[Serializable]
public class AreaInfo 
{	
	[SerializeField] public HitInfo.eAREA_SHAPE areaShape = HitInfo.eAREA_SHAPE.Point;
	
	[SerializeField] public int angle = 0;
	[SerializeField] public int centerDirectionAngle = 0;
	[SerializeField] public int minDistance = 0;
	[SerializeField] public int maxDistance = 0;
	[SerializeField] public int width = 0;
	[SerializeField] public int height = 0;
	[SerializeField] public int offsetX = 0;
	[SerializeField] public int offsetY = 0;	
}
#endif
	
	
[Serializable]
public class ReadyAnimationInfo
{
	public enum eLOOP_TYPE
	{
		Once,
		Loop,
		TimeLoop,
		TargetLoop,
		Charge,
		Cast,
		ClampForever,		
	}
	
	

	
	[HideInInspector]public string filePathName;
#if _USE_AREALAYER	
	public int actionSpeed = 1000;
#endif	
	public eLOOP_TYPE loopType = eLOOP_TYPE.Once;
	public int loopTargetTime = 0;
	public int loopDuration = 0;
	public int animationLength = 0;
	public HitAnimationInfo.eMOVE_TYPE moveType = HitAnimationInfo.eMOVE_TYPE.None;	
	public int moveDistance = 0;
	
	public List<EffectInfo> effectLayer = new List<EffectInfo>();	
	public List<SoundInfo> soundLayer = new List<SoundInfo>();	
	public List<TrailInfo> trailLayer = new List<TrailInfo>();	
	public List<ActionCancelInfo> actionCancelDisableLayer = new List<ActionCancelInfo>();	
	
};


[Serializable]
public class HitAnimationInfo
{
	public enum eLOOP_TYPE
	{
		Once,
		Loop,
		TimeLoop,
		TargetLoop,
		
	}
	
	public enum eMOVE_TYPE
	{
		None,
		Dash,		
		Warp,
		Jump,
		TargetDash,
		TabDash,
		TabWarp,
		BackTargetDash,
	}
	
	
	//public AnimationClip filePath= null;
	[HideInInspector]public string filePathName;
#if _USE_AREALAYER	
	public int actionSpeed = 1000;
#endif	
	public eLOOP_TYPE loopType = eLOOP_TYPE.Once;	
	public int loopTargetTime = 0;
	public int loopDuration = 0;
	public int animationLength = 0;
	public eMOVE_TYPE moveType = eMOVE_TYPE.None;	
	public int moveDistance = 0;
	
	
	public List<EffectInfo> effectLayer = new List<EffectInfo>();	
	public List<SoundInfo> soundLayer = new List<SoundInfo>();	
	public List<TrailInfo> trailLayer = new List<TrailInfo>();	
	public List<ActionCancelInfo> actionCancelDisableLayer = new List<ActionCancelInfo>();	
	public HitInfo  			hitInfoData = new HitInfo();
};



[Serializable]
public class FinishAnimationInfo
{
	public enum eLOOP_TYPE
	{
		Once,
		Loop,
		TimeLoop,
		TargetLoop,		
	}
	

	
//	public AnimationClip filePath= null;
	[HideInInspector]public string filePathName;
#if _USE_AREALAYER	
	public int actionSpeed = 1000;
#endif	
	public eLOOP_TYPE loopType = eLOOP_TYPE.Once;
	public int loopTargetTime = 0;
	public int loopDuration = 0;
	public int animationLength = 0;

	public List<EffectInfo> effectLayer = new List<EffectInfo>();	
	public List<SoundInfo> soundLayer = new List<SoundInfo>();	
	public List<TrailInfo> trailLayer = new List<TrailInfo>();	
	public List<ActionCancelInfo> actionCancelDisableLayer = new List<ActionCancelInfo>();	
};

[Serializable]
public class HitInfo
{
	public enum eHIT_TYPE
	{
		None,
		Target,
		NonTarget,
		PositionTarget,
		ProjectileTarget,	
		HoldTarget,
	}
	
	public enum ePROJECTILE_PATH
	{
		None,
		Straight,
		Arc,
		Through,		
	}	
	
	public enum eAREA_SHAPE
	{		
		Point,
		Circle,
		Quadrangle,
		OurParty,
		OtherParty
	}
	

	public enum eVALUELOOK_TYPE
	{		
		None,
		Moment,
		Duration,		
	}
	
	
	public eHIT_TYPE hitType = eHIT_TYPE.None;	
#if _USE_AREALAYER	
	public int  multiTargetCount = 0;            //2013.12.31 추가.
#endif	
	public int  timing  = 0;	
	[SerializeField]public GameObject projectileFilePath = null;
	[HideInInspector]public string projectileFilePathName = null;
	
	//public string projectileFilePath;
	public int projectileSpeed = 0;
	public int projectileAcceleration = 0;
	public eProjectilePath projectilePath = eProjectilePath.NONE;
	[SerializeField]public GameObject projectileHitFilePath = null;
	[HideInInspector]public string projectileHitFilePathName = null;
#if _USE_AREALAYER
	public List<AreaInfo> areaLayer = new List<AreaInfo>();	
#else	
	public eAREA_SHAPE areaShape = eAREA_SHAPE.Point;
	
	public int angle = 0;
	public int centerDirectionAngle = 0;
	public int minDistance = 0;
	public int maxDistance = 0;
	public int width = 0;
	public int height = 0;
	public int offsetX = 0;
	public int offsetY = 0;
#endif
	
	public int valuePercent  = 0;
	public eVALUELOOK_TYPE valueLookType = eVALUELOOK_TYPE.None;
	
	public int valueLookDuration = 0;
	public int valueLookCount = 0;	
	
	[HideInInspector]public bool m_bPlay = false;
	[HideInInspector]public bool m_bPontencyPlay = false;
	
	[SerializeField]public AudioClip projectileHitSoundPath = null;
	[HideInInspector]public string projectileHitSoundPathName = null;
	
	
};


public class AsActionListXmlWrite 
{	
	void Save_HitInfo( XmlWriter writer, HitInfo data)
	{
		if(data.hitType == HitInfo.eHIT_TYPE.None ) return;	
	
		writer.WriteStartElement ("HitInfo");	
		
		writer.WriteStartAttribute ("HitType");	
		writer.WriteValue ( data.hitType.ToString() );
#if _USE_AREALAYER		
		writer.WriteStartAttribute ("MultiTargetCount");	
		writer.WriteValue ( data.multiTargetCount.ToString() );		
#endif	
		writer.WriteStartAttribute ("Timing");	
		writer.WriteValue ( data.timing.ToString() );
		
		writer.WriteStartAttribute ("ProjectileFilePath");	
		//writer.WriteValue ( data.projectileFilePath );
		if(data.projectileFilePath == null)
		{
			writer.WriteValue ("None" );
		}
		else
		{			
			writer.WriteValue ( data.projectileFilePathName );
		}
			
		
		writer.WriteStartAttribute ("ProjectileSpeed");	
		writer.WriteValue ( data.projectileSpeed.ToString() );
		
		writer.WriteStartAttribute ("ProjectileAcceleration");	
		writer.WriteValue ( data.projectileAcceleration.ToString() );
		
		writer.WriteStartAttribute ("ProjectilePath");	
		writer.WriteValue ( data.projectilePath.ToString() );
		
		writer.WriteStartAttribute ("ProjectileHitFilePath");	
		
		if(data.projectileHitFilePath == null)
		{
			writer.WriteValue ("None" );
		}
		else
		{			
			writer.WriteValue ( data.projectileHitFilePathName );
		}

#if _USE_AREALAYER
		// AreaList Count
		writer.WriteStartAttribute ("AreaListCount");	
		writer.WriteValue ( data.areaLayer.Count.ToString() );
#else		
		writer.WriteStartAttribute ("AreaShape");	
		writer.WriteValue ( data.areaShape.ToString() );
		
		writer.WriteStartAttribute ("Angle");	
		writer.WriteValue ( data.angle.ToString() );
		
		writer.WriteStartAttribute ("CenterDirectionAngle");	
		writer.WriteValue ( data.centerDirectionAngle.ToString() );		
	
		
		writer.WriteStartAttribute ("MinDistance");	
		writer.WriteValue ( data.minDistance.ToString() );
		
		writer.WriteStartAttribute ("MaxDistance");	
		writer.WriteValue ( data.maxDistance.ToString() );
		
		writer.WriteStartAttribute ("Width");	
		writer.WriteValue ( data.width.ToString() );
		
		writer.WriteStartAttribute ("Height");	
		writer.WriteValue ( data.height.ToString() );
		
		writer.WriteStartAttribute ("OffsetX");	
		writer.WriteValue ( data.offsetX.ToString() );
				
		writer.WriteStartAttribute ("OffsetY");	
		writer.WriteValue ( data.offsetY.ToString() );
#endif		
		
		writer.WriteStartAttribute ("ValuePercent");	
		writer.WriteValue ( data.valuePercent.ToString() );
		
		writer.WriteStartAttribute ("ValueLookType");	
		writer.WriteValue ( data.valueLookType.ToString() );
				
		writer.WriteStartAttribute ("ValueLookDuration");	
		writer.WriteValue ( data.valueLookDuration.ToString() );
		// #22596
		if(data.valueLookCount > 10)
			data.valueLookCount = 10;
		writer.WriteStartAttribute ("ValueLookCount");	
		writer.WriteValue ( data.valueLookCount.ToString() );
		
		
		writer.WriteStartAttribute ("ProjectileHitSoundPath");	
		
		if(data.projectileHitSoundPath == null)
		{
			writer.WriteValue ("None" );
		}
		else
		{			
			writer.WriteValue ( data.projectileHitSoundPathName );
		}

		writer.WriteEndAttribute();
		writer.WriteEndElement();//HitInfo		
	
				
#if _USE_AREALAYER			
		Save_AreaInfo(writer,data.areaLayer);
#endif
		
	}
	
#if _USE_AREALAYER
	void Save_AreaInfo( XmlWriter writer, List<AreaInfo> areaLayer )
	{
			//AreaInfoo		
		for( int index = 0; index < areaLayer.Count; ++index )
		{
			AreaInfo data = areaLayer[index];					
			writer.WriteStartElement ("AreaInfo"+(1+index).ToString());	
		
			writer.WriteStartAttribute ("AreaShape");	
			writer.WriteValue ( data.areaShape.ToString() );
		
			writer.WriteStartAttribute ("Angle");	
			writer.WriteValue ( data.angle.ToString() );
		
			writer.WriteStartAttribute ("CenterDirectionAngle");	
			writer.WriteValue ( data.centerDirectionAngle.ToString() );		
		
		
			writer.WriteStartAttribute ("MinDistance");	
			writer.WriteValue ( data.minDistance.ToString() );
			
			writer.WriteStartAttribute ("MaxDistance");	
			writer.WriteValue ( data.maxDistance.ToString() );
		
			writer.WriteStartAttribute ("Width");	
			writer.WriteValue ( data.width.ToString() );
		
			writer.WriteStartAttribute ("Height");	
			writer.WriteValue ( data.height.ToString() );
		
			writer.WriteStartAttribute ("OffsetX");	
			writer.WriteValue ( data.offsetX.ToString() );
				
			writer.WriteStartAttribute ("OffsetY");	
			writer.WriteValue ( data.offsetY.ToString() );		
			
			writer.WriteEndAttribute();			
			writer.WriteEndElement ();	//AreaInfoo		
		}
	}
#endif		
	
	void Save_EffectInfo( XmlWriter writer, List<EffectInfo> effectLayer )
	{		
		//EffectInfo		
		for( int index = 0; index < effectLayer.Count; ++index )
		{
			EffectInfo data = effectLayer[index];		
			writer.WriteStartElement ("EffectInfo"+(1+index).ToString());	
		
			writer.WriteStartAttribute ("Timing");	
			writer.WriteValue ( data.timing.ToString() );
			
			
			writer.WriteStartAttribute ("FileName");
			if(data.filePath == null)
			{
				writer.WriteValue ("None" );
			}
			else
			{
				writer.WriteValue ( data.filePathName );
			}
			
			writer.WriteStartAttribute ("LinkType");	
			writer.WriteValue ( data.linkType.ToString() );
			
			
			writer.WriteStartAttribute ("LoopType");	
			writer.WriteValue ( data.loopType.ToString() );
		
			
			writer.WriteStartAttribute ("LoopDuration");	
			writer.WriteValue ( data.loopDuration.ToString() );
		
			
			writer.WriteStartAttribute ("PositionFix");	
			writer.WriteValue ( data.positionFix.ToString() );
				
			writer.WriteStartAttribute ("StartSize");	
			writer.WriteValue ( data.startSize.ToString() );
			
			writer.WriteEndAttribute();
			
			writer.WriteEndElement ();	//EffectInfo		
		}
	}	
	
	public void Save_SoundInfo( XmlWriter writer, List<SoundInfo> soundLayer )
	{
		//SoundInfo		
		for( int index = 0; index < soundLayer.Count; ++index )
		{
			SoundInfo data = soundLayer[index];		
			writer.WriteStartElement ("SoundInfo"+(1+index).ToString());	
		
			writer.WriteStartAttribute ("Timing");	
			writer.WriteValue ( data.timing.ToString() );
		
			writer.WriteStartAttribute ("FileName");	
			if(data.filePath == null)
			{
				writer.WriteValue ("None" );
			}
			else
			{
				writer.WriteValue ( data.filePathName );
			}		
	
		
			writer.WriteStartAttribute ("LoopType");	
			writer.WriteValue ( data.loopType.ToString() );
		
			writer.WriteEndAttribute();
			writer.WriteEndElement ();	//SoundInfo		
		}			
	}
	
	public void Save_TrailInfo( XmlWriter writer, List<TrailInfo> trailLayer  )
	{
		//TrailInfo		
		for( int index = 0; index < trailLayer.Count; ++index )
		{
			TrailInfo data = trailLayer[index];		
			writer.WriteStartElement ("TrailInfo"+(1+index).ToString());	
		
			writer.WriteStartAttribute ("TextureName");	
			//////////////////////////////////////////////////////////////////////////////////////////
		
			if(data.textureName == null)
			{
				writer.WriteValue ("None" );
			}
			else
			{			
				writer.WriteValue (data.filePathName );
			}
			
								
			writer.WriteStartAttribute ("StartTime");	
			writer.WriteValue ( data.startTime.ToString() );
				
			writer.WriteStartAttribute ("LoopDuration");	
			writer.WriteValue ( data.loopDuration.ToString() );
		
			writer.WriteEndAttribute();
			
			writer.WriteEndElement ();	//TrailInfo		
		}		
	}
	
	public void Save_ActionCancelInfo( XmlWriter writer, List<ActionCancelInfo> actionCancelLayer )
	{
		//ActionCancelInfo		
		for( int index = 0; index < actionCancelLayer.Count; ++index )
		{
			ActionCancelInfo data = actionCancelLayer[index];		
			writer.WriteStartElement ( "ActionCancel"+(1+index).ToString());	
		
			writer.WriteStartAttribute ("StartTime");	
			writer.WriteValue ( data.startTime.ToString() );		
			
			writer.WriteStartAttribute ("EndTime");	
			writer.WriteValue ( data.endTime.ToString() );
		
			writer.WriteEndAttribute();
			writer.WriteEndElement ();	//ActionCancel		
		}
	}
	public void Save_ReadyAnimationInfo( XmlWriter writer, ReadyAnimationInfo readyInfo )
	{
		if(readyInfo.filePathName == null ) return;
		if( readyInfo.filePathName.CompareTo("None") == 0) return;
		if(readyInfo.filePathName.Length == 0) 	return;
		writer.WriteStartElement ("Ready");
		writer.WriteStartElement ("ReadyAnimation");
		 
		// FileName
		writer.WriteStartAttribute ("FileName");	
		writer.WriteValue ( readyInfo.filePathName );
#if _USE_AREALAYER		
		writer.WriteStartAttribute ("ActionSpeed");	
		writer.WriteValue ( readyInfo.actionSpeed.ToString() );	
#endif		
		writer.WriteStartAttribute ("LoopType");	
		writer.WriteValue ( readyInfo.loopType.ToString() );	
		
		
		writer.WriteStartAttribute ("LoopTargetTime");	
		writer.WriteValue ( readyInfo.loopTargetTime.ToString() );
		
		writer.WriteStartAttribute ("LoopDuration");	
		writer.WriteValue ( readyInfo.loopDuration.ToString() );
		
		writer.WriteStartAttribute ("AnimationLength");	
		writer.WriteValue ( readyInfo.animationLength.ToString() );
	
		writer.WriteStartAttribute ("MoveType");	
		writer.WriteValue ( readyInfo.moveType.ToString() );
		
		writer.WriteStartAttribute ("MoveDistance");	
		writer.WriteValue ( readyInfo.moveDistance.ToString() );
	
		
		// EffectList Count
		writer.WriteStartAttribute ("EffectListCount");	
		writer.WriteValue ( readyInfo.effectLayer.Count.ToString() );			
		
		
		// SoundList Count
		writer.WriteStartAttribute ("SoundListCount");	
		writer.WriteValue ( readyInfo.soundLayer.Count.ToString() );		
		
		
		// TrailList Count
		writer.WriteStartAttribute ("TrailListCount");	
		writer.WriteValue ( readyInfo.trailLayer.Count.ToString() );			
		
		
		// actionCanceList Count
		writer.WriteStartAttribute ("ActionCancelListCount");	
		writer.WriteValue ( readyInfo.actionCancelDisableLayer.Count.ToString() );			
		
		
		
		writer.WriteEndAttribute();
		writer.WriteEndElement ();//ReadyAnimation
		
		Save_EffectInfo(writer,readyInfo.effectLayer);
		Save_SoundInfo(writer,readyInfo.soundLayer);
		Save_TrailInfo(writer,readyInfo.trailLayer);
		Save_ActionCancelInfo(writer,readyInfo.actionCancelDisableLayer);
		
		writer.WriteEndElement ();//Ready
	}
	
	public void Save_HitAnimationInfo( XmlWriter writer, HitAnimationInfo hitAniInfo )
	{
		if(hitAniInfo.filePathName == null) return;
		if(hitAniInfo.filePathName.CompareTo("None") == 0) return;
		if(hitAniInfo.filePathName.Length == 0) return;
		writer.WriteStartElement ("Hit");
		writer.WriteStartElement ("HitAnimation");
		 
		// FileName
		writer.WriteStartAttribute ("FileName");	
		writer.WriteValue ( hitAniInfo.filePathName );
#if _USE_AREALAYER		
		writer.WriteStartAttribute ("ActionSpeed");	
		writer.WriteValue ( hitAniInfo.actionSpeed.ToString() );	
#endif		
		writer.WriteStartAttribute ("LoopType");	
		writer.WriteValue ( hitAniInfo.loopType.ToString() );	
		
		
		writer.WriteStartAttribute ("LoopTargetTime");	
		writer.WriteValue ( hitAniInfo.loopTargetTime.ToString() );
		
		writer.WriteStartAttribute ("LoopDuration");	
		writer.WriteValue ( hitAniInfo.loopDuration.ToString() );
		
		writer.WriteStartAttribute ("AnimationLength");	
		writer.WriteValue ( hitAniInfo.animationLength.ToString() );
	
		
		writer.WriteStartAttribute ("MoveType");	
		writer.WriteValue ( hitAniInfo.moveType.ToString() );
		
		writer.WriteStartAttribute ("MoveDistance");	
		writer.WriteValue ( hitAniInfo.moveDistance.ToString() );
	

		
		
		// EffectList Count
		writer.WriteStartAttribute ("EffectListCount");	
		writer.WriteValue ( hitAniInfo.effectLayer.Count.ToString() );
		
		// SoundList Count
		writer.WriteStartAttribute ("SoundListCount");	
		writer.WriteValue ( hitAniInfo.soundLayer.Count.ToString() );		
		
		
		// TrailList Count
		writer.WriteStartAttribute ("TrailListCount");	
		writer.WriteValue ( hitAniInfo.trailLayer.Count.ToString() );
			
		
		
		// actionCanceList Count
		writer.WriteStartAttribute ("ActionCancelListCount");	
		writer.WriteValue ( hitAniInfo.actionCancelDisableLayer.Count.ToString() );
		
		
		writer.WriteEndAttribute();
		writer.WriteEndElement ();//HitAnimation		
		
		Save_EffectInfo(writer,hitAniInfo.effectLayer);
		Save_SoundInfo(writer,hitAniInfo.soundLayer);
		Save_TrailInfo(writer,hitAniInfo.trailLayer);
		Save_ActionCancelInfo(writer,hitAniInfo.actionCancelDisableLayer);
		//HitInfo
		Save_HitInfo(writer,hitAniInfo.hitInfoData );
		
		writer.WriteEndElement ();//Hit
		
	}
	
	public void Save_FinishAnimationInfo( XmlWriter writer, FinishAnimationInfo finishInfo )
	{
		if(finishInfo.filePathName == null ) return;
		if(finishInfo.filePathName.CompareTo("None") == 0) return;	
		if(finishInfo.filePathName.Length == 0) return;
		
		writer.WriteStartElement ("Finish");
		writer.WriteStartElement ("FinishAnimation");
		 
			// FileName
		writer.WriteStartAttribute ("FileName");	
		writer.WriteValue ( finishInfo.filePathName );
#if _USE_AREALAYER		
		writer.WriteStartAttribute ("ActionSpeed");	
		writer.WriteValue ( finishInfo.actionSpeed.ToString() );	
#endif		
		writer.WriteStartAttribute ("LoopType");	
		writer.WriteValue ( finishInfo.loopType.ToString() );	
		
		
		writer.WriteStartAttribute ("LoopTargetTime");	
		writer.WriteValue ( finishInfo.loopTargetTime.ToString() );
		
		writer.WriteStartAttribute ("LoopDuration");	
		writer.WriteValue ( finishInfo.loopDuration.ToString() );
		
		writer.WriteStartAttribute ("AnimationLength");	
		writer.WriteValue ( finishInfo.animationLength.ToString() );
	
		// EffectList Count		
		writer.WriteStartAttribute ("EffectListCount");	
		writer.WriteValue ( finishInfo.effectLayer.Count.ToString() );
	
		
		// SoundList Count
		writer.WriteStartAttribute ("SoundListCount");	
		writer.WriteValue ( finishInfo.soundLayer.Count.ToString() );
		
		
		// TrailList Count
		writer.WriteStartAttribute ("TrailListCount");	
		writer.WriteValue ( finishInfo.trailLayer.Count.ToString() );
		
		
		// actionCanceList Count
		writer.WriteStartAttribute ("ActionCancelListCount");	
		writer.WriteValue ( finishInfo.actionCancelDisableLayer.Count.ToString() );
		
		writer.WriteEndAttribute();
		writer.WriteEndElement ();//FinishAnimation
		
		Save_EffectInfo(writer,finishInfo.effectLayer);
		Save_SoundInfo(writer,finishInfo.soundLayer);
		Save_TrailInfo(writer,finishInfo.trailLayer);
		Save_ActionCancelInfo(writer,finishInfo.actionCancelDisableLayer);	
		
		writer.WriteEndElement ();//Finish
		
		
	}
	
	public bool Save( string filename, AsActionSaveData actionSaveData )
	{
		XmlWriterSettings wrapperSettings = new XmlWriterSettings();
		wrapperSettings.Indent = true;			
	
		string wrappername = Application.dataPath + "/Resources/Table/" + filename;
		
		
		using ( XmlWriter writer = XmlWriter.Create (wrappername, wrapperSettings) ) 
		{
			writer.WriteStartDocument();
            writer.WriteStartElement ("ActionList");
			
			foreach (KeyValuePair<int, AsAction_Record> tmp in actionSaveData.GetList())
			{				
           		writer.WriteStartElement ("Action");
				AsAction_Record data = (AsAction_Record)tmp.Value;
				
					
				// Index
				writer.WriteStartElement ("Index");	
				writer.WriteValue ( tmp.Value.index );
				writer.WriteEndElement ();
				
				// Action Name
				writer.WriteStartElement ("ActionName");	
				writer.WriteValue ( data.actionName);
				writer.WriteEndElement ();	
				
				//Class
				writer.WriteStartElement ("Class");	
				writer.WriteValue ( data.classType.ToString());
				writer.WriteEndElement ();	
			
#if _USE_AREALAYER	
				if( filename.CompareTo("ActionDataList.xml") == 0)
				{
					//Gender
					writer.WriteStartElement ("Gender");	
					writer.WriteValue ( data.gender.ToString());
					writer.WriteEndElement ();	
				}
#else
				if( filename.CompareTo("ActionDataList.xml") == 0)
				{
					//Gender
					writer.WriteStartElement ("Gender");	
					writer.WriteValue ( data.gender.ToString());
					writer.WriteEndElement ();	
				}
				
#endif
				
				//LinkActionIndex
				writer.WriteStartElement ("LinkActionIndex");	
				writer.WriteValue ( data.linkActionIndex);
				writer.WriteEndElement ();	
			
				//AniBlendingDuration
				writer.WriteStartElement ("AniBlendingDuration");	
				writer.WriteValue ( data.aniBlendingDuration);
				writer.WriteEndElement ();	
				
				
				//NextIdleIndex
				writer.WriteStartElement ("NextIdleIndex");	
				writer.WriteValue ( data.nextIdleIndex.ToString());
				writer.WriteEndElement ();	
	

				Save_ReadyAnimationInfo(writer,data.readyAnimationInfoData);
				Save_HitAnimationInfo(writer,data.hitAnimationInfoData);
				Save_FinishAnimationInfo(writer,data.finishAnimationInfoData);
				
				
				writer.WriteEndElement ();//Action
			}
			writer.WriteEndElement ();//ActionList
			       
            writer.Close ();
		}
		return true;
	}
	
	
}
