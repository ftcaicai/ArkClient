using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsRankChangeAlarmManager
{
	private List<body_SC_RANK_CHANGE_MYRANK> lstAlarms = new List<body_SC_RANK_CHANGE_MYRANK>();
	
	private static AsRankChangeAlarmManager instance = null;
	public static AsRankChangeAlarmManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsRankChangeAlarmManager();
			
			return instance;
		}
	}
	
	public int Count
	{
		get	{ return lstAlarms.Count; }
	}
	
	public void Insert( body_SC_RANK_CHANGE_MYRANK data)
	{
		lstAlarms.Add( data);
	}
	
	public void Remove( int index)
	{
		if( index >= lstAlarms.Count)
			return;
		
		lstAlarms.RemoveAt( index);
	}
	
	public body_SC_RANK_CHANGE_MYRANK Get( int index)
	{
		if( index >= lstAlarms.Count)
			return null;
		
		return lstAlarms[ index];
	}
}
