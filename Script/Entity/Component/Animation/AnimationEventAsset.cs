using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eAnimEvent {NONE = 0, Eye_Blink = 10}

[System.Serializable]
public class AnimationEventInfo
{
//	public string race_;
	public eCLASS class_;
	public eGENDER gender_;
	public string name_;
	
	public eAnimEvent event_;
	public float timing_;
	
	public AnimationEventInfo(eCLASS _class, eGENDER _gender, string _name, eAnimEvent _event, float _timing)
	{
//		race_ = _race;
		class_ = _class;
		gender_ = _gender;
		name_ = _name;
		
		event_ = _event;
		timing_ = _timing;
	}
}

public class AnimationEventAsset : ScriptableObject
{
	[SerializeField]
	public List<AnimationEventInfo> eventInfo_ = new List<AnimationEventInfo>();
	
	Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, List<AnimationEventInfo>>>> m_dicEvents = null;
	
	public void SetEventDictionary()
	{
		m_dicEvents = new Dictionary<eCLASS, Dictionary<eGENDER, Dictionary<string, List<AnimationEventInfo>>>>();
		AnimationEventInfo info = null;
		for(int i=0; i<eventInfo_.Count; ++i)
		{
			info = eventInfo_[i];

			if(m_dicEvents.ContainsKey(info.class_) == false)
				m_dicEvents.Add(info.class_, new Dictionary<eGENDER,Dictionary<string, List<AnimationEventInfo>>>());
			
			if(m_dicEvents[info.class_].ContainsKey(info.gender_) == false)
				m_dicEvents[info.class_].Add(info.gender_, new Dictionary <string, List<AnimationEventInfo>>());
			
			if(m_dicEvents[info.class_][info.gender_].ContainsKey(info.name_) == false)
				m_dicEvents[info.class_][info.gender_].Add(info.name_, new List<AnimationEventInfo>());
			
			m_dicEvents[info.class_][info.gender_][info.name_].Add(info);
		}
	}
	
	public Dictionary<string, List<AnimationEventInfo>> GetEventDic(eCLASS _class, eGENDER _gender)
	{
		if(m_dicEvents.ContainsKey(_class) == true)
		{
			if(m_dicEvents[_class].ContainsKey(_gender) == true)
			{
				return m_dicEvents[_class][_gender];
			}
		}
		
//		Debug.LogError("AnimationEventAsset::GetEventList: event not registered is requested. [" + _class + "][" + _gender + "]");
		return null;
	}
	
	public List<AnimationEventInfo> GetEventList(eCLASS _class, eGENDER _gender, string _name)
	{
//		if(m_dicEvents == null)
//			SetEventDictionary();
		
		if(m_dicEvents.ContainsKey(_class) == true)
		{
			if(m_dicEvents[_class].ContainsKey(_gender) == true)
			{
				if(m_dicEvents[_class][_gender].ContainsKey(_name) == true)
				{
					if(m_dicEvents[_class][_gender][_name] != null)
						return m_dicEvents[_class][_gender][_name];
				}
			}
		}
		
//		Debug.LogError("AnimationEventAsset::GetEventList: event not registered is requested. [" + _class + "][" + _gender + "][" + _name + "]");
		return null;
	}
}