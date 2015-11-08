using UnityEngine;
using System.Collections;

public class AsServerListData
{
	static private AsServerListData instance = null;
	private ArrayList dataList = new ArrayList();
	public int serverNewCnt = 0;
	
	public int Count
	{
		get { return dataList.Count; }
	}
	
	private AsServerListData()
	{
	}
	
	static public AsServerListData Instance
	{
		get
		{
			if( null == instance)
				instance = new AsServerListData();
			
			return instance;
		}
	}
	
	public void InsertData( AS_LC_SERVERLIST_BODY data)
	{
		dataList.Add( data);
	}
	
	public AS_LC_SERVERLIST_BODY GetData( int index)
	{
		if( index >= dataList.Count)
			return null;
		
		return dataList[ index] as AS_LC_SERVERLIST_BODY;
	}
	
	public void Clear()
	{
		dataList.Clear();
	}
}
