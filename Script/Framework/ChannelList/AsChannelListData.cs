using UnityEngine;
using System.Collections;

public class AsChannelListData
{
	static private AsChannelListData instance = null;
	private ArrayList dataList = new ArrayList();
	
	public int Count
	{
		get { return dataList.Count; }
	}
	
	private AsChannelListData()
	{
	}
	
	static public AsChannelListData Instance
	{
		get
		{
			if( null == instance)
				instance = new AsChannelListData();
			
			return instance;
		}
	}
	
	public void InsertData( body2_GC_CHANNEL_LIST data)
	{
		dataList.Add( data);
	}
	
	public body2_GC_CHANNEL_LIST GetData( int index)
	{
		if( index >= dataList.Count)
			return null;
		
		return dataList[ index] as body2_GC_CHANNEL_LIST;
	}

	public body2_GC_CHANNEL_LIST GetDataByChannelIndex( int channelIndex)
	{
		foreach( body2_GC_CHANNEL_LIST channel in dataList)
		{
			if( channel.nChannel == channelIndex)
				return channel;
		}

		return null;
	}
	
	public void Clear()
	{
		dataList.Clear();
	}
}
