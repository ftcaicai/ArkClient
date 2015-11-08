using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsEventManager
{
	static private AsEventManager instance = null;
	static public AsEventManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsEventManager();
			
			return instance;
		}
	}
	
	private Dictionary<Int32,body2_SC_SERVER_EVENT_START> dicEvent = new Dictionary<Int32,body2_SC_SERVER_EVENT_START>();
	public bool conditionEvent = false;
	
	public int GetEventCount()
	{
		return dicEvent.Count;
	}
	
	private AsEventManager()	{}
	
	public void Insert( body2_SC_SERVER_EVENT_START data)
	{
		if( true == dicEvent.ContainsKey( data.nEventKey))
		{
			Debug.LogWarning( "event key is already contained!");
			return;
		}
		
		dicEvent.Add( data.nEventKey, data);
		
		switch( data.eEventType)
		{
		case eEVENT_TYPE.eEVENT_TYPE_TIME_EXP:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_GOLD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_DROP:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION:
			AsMyProperty.Instance.SetConditionEventState( true);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_ATTEND_ADD_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_RETURN_ADD_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_ITEM_STRENGTHEN:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COLLECT:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_PRODUCT:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MIX:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COMMISSION:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MOB_REGEN:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_CONDITION_DROP:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE:
			AsEventUIMgr.Instance.InsertEventAlram( data);
			break;
		}
	}
	
	public body2_SC_SERVER_EVENT_START Get( Int32 key)
	{
		if( false == dicEvent.ContainsKey( key))
		{
			Debug.Log( "Event key is not exist : " + key);
			return null;
		}
		
		return dicEvent[ key];
	}
	
	public body2_SC_SERVER_EVENT_START Get( eEVENT_TYPE type)
	{
		foreach( KeyValuePair<int,body2_SC_SERVER_EVENT_START> pair in dicEvent)
		{
			if( type == pair.Value.eEventType)
				return pair.Value;
		}
		
		return null;
	}
	
	public void Remove( Int32 key)
	{
		if( false == dicEvent.ContainsKey( key))
		{
			Debug.Log( "Event key is not exist : " + key);
			return;
		}
		
		switch( dicEvent[ key].eEventType)
		{
		case eEVENT_TYPE.eEVENT_TYPE_TIME_EXP:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_GOLD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_DROP:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION:
			AsMyProperty.Instance.SetConditionEventState( false);
			break;
		case eEVENT_TYPE.eEVENT_TYPE_ATTEND_ADD_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_RETURN_ADD_REWARD:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_ITEM_STRENGTHEN:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COLLECT:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_PRODUCT:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MIX:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_COMMISSION:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_MOB_REGEN:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_COMBINE_CONDITION_DROP:
			break;
		case eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE:
			break;
		}

		dicEvent.Remove( key);
	}
	
	public void Clear()
	{
		dicEvent.Clear();
	}
}
