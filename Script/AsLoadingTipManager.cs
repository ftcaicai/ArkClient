using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class LoadingTipData : AsTableRecord
{
	public int index = -1;
	public int minLevel = -1;
	public int maxLevel = -1;
	public eCLASS eClass = eCLASS.NONE;
	public int strIndex = -1;
	
	public LoadingTipData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref index, node, "Index");
			SetValue( ref minLevel, node, "MinLevel");
			SetValue( ref maxLevel, node, "MaxLevel");
			SetValue<eCLASS>( ref eClass, node, "Class");
			SetValue( ref strIndex, node, "StringIndex");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsLoadingTipManager
{
	private List<LoadingTipData> dataList = new List<LoadingTipData>();
	private List<int> suitableTipList = new List<int>();
	
	private static AsLoadingTipManager instance = null;
	public static AsLoadingTipManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsLoadingTipManager();
			
			return instance;
		}
	}
	
	private AsLoadingTipManager()
	{
	}
	
	public void SuitableTipCollect()
	{
		int curLevel = AsUserInfo.Instance.SavedCharStat.level_;
		eCLASS eClass = (eCLASS)AsUserInfo.Instance.SavedCharStat.class_;
		
		suitableTipList.Clear();
		
		foreach( LoadingTipData data in dataList)
		{
			if( ( data.minLevel > curLevel) || ( data.maxLevel < curLevel))
				continue;
			
			if( ( eCLASS.All != data.eClass) && ( data.eClass != eClass))
				continue;
			
			suitableTipList.Add( data.index - 1);
		}
	}
	
	public string GetLoadingTip()
	{
		if( 0 == suitableTipList.Count)
			return "";
		
		int tipIndex = Random.Range( 0, suitableTipList.Count);
		int index = dataList[ suitableTipList[ tipIndex]].strIndex;
		
		return AsTableManager.Instance.GetTbl_String( index);
	}
	
	public string GetLoadingTip_RandAll()
	{
		int nRandIndex = Random.Range( 0, dataList.Count);
		int nIndex = dataList[nRandIndex].strIndex;
		
		return AsTableManager.Instance.GetTbl_String( nIndex);
	}
	
	public void LoadTable()
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( "Table/TipTable");
			XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
			{
				LoadingTipData tipData = new LoadingTipData( (XmlElement)node);
				dataList.Add( tipData);
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsLoadingTipManager:LoadTable");
		}
	}
}
