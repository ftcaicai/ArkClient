
//#define _TRACE_DDOL

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DDOL_Tracer : MonoBehaviour
{
	static Dictionary<MonoBehaviour, GameObject> s_dicObj = new Dictionary<MonoBehaviour, GameObject>();

	public static void RegisterDDOL( MonoBehaviour _mono, Mesh mesh)
	{
	}
	
	[System.Diagnostics.Conditional( "_TRACE_DDOL")]
	public static void RegisterDDOL( MonoBehaviour _mono, GameObject _obj)
	{
		if( s_dicObj.ContainsKey( _mono) == false)
			s_dicObj.Add( _mono, _obj);
		else
			Debug.LogWarning( _mono + " is already registered.");
	}
	
	[System.Diagnostics.Conditional( "_TRACE_DDOL")]
	public static void BeginTrace()
	{
		foreach( KeyValuePair<MonoBehaviour, GameObject> pair in s_dicObj)
		{
			TraceRecursively( pair.Key, pair.Value);
		}
	}
	
	[System.Diagnostics.Conditional( "_TRACE_DDOL")]
	static void TraceRecursively( MonoBehaviour _mono, GameObject _obj)
	{
		if( null == _obj)
			return;
		
		Transform parent = _obj.transform.parent;
		string log = "{R}" + _mono.GetType();
		
		while( true)
		{
			if( parent == null)
				break;
			
			bool concur = false;
			foreach( KeyValuePair<MonoBehaviour, GameObject> pair in s_dicObj)
			{
				if( pair.Value == parent.gameObject)
				{
					concur = true;
					break;
				}
			}
			
			if( concur == false)
				log = "< [ INVALID ] " + parent.name + " >/" + log;
			else
				log = "(" + parent.name + ")/" + log;

			parent = parent.parent;
		}
	}
}
