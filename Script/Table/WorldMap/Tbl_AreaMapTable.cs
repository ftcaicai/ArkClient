using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.Xml;


public class Tbl_AreaMap_Record : AsTableRecord
{		
	int m_Index;	
	string m_strImgPath;
	bool m_isActive= false;
	int m_iTitleStrIdx;	
		
	public int Index
	{
		get
		{
			return m_Index;
		}
	}
	
	public string getStrImgPath
	{
		get
		{
			return m_strImgPath;
		}
	}
	
	public bool isActive
	{
		get
		{
			return m_isActive;
		}	
	}
	
	public int getTitleStrIdx	
	{
		get
		{
			return m_iTitleStrIdx;
		}
	}
			
	
	/*public int getTooltipStrIdx
	{
		get	
		{
			return m_iTooltipStrIdx;
		}
	}*/
	
	
	public Tbl_AreaMap_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref m_Index, node, "Index" );
			SetValue( ref m_strImgPath, node, "ImgPath" );
			SetValue( ref m_isActive, node, "Active" );
			SetValue( ref m_iTitleStrIdx, node, "String" );	
			
			//SetValue( ref m_iTooltipStrIdx, node, "TooltipStringIdx" );					
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_StrengthenCost_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
}

public class Tbl_AreaMapTable : AsTableBase 
{	
	Dictionary<int, Tbl_AreaMap_Record> m_RecordList = new Dictionary<int, Tbl_AreaMap_Record>();
	
	
	public Tbl_AreaMapTable(string _path)
	{
		m_TableType = eTableType.STRING;		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		XmlElement root = GetXmlRootElement(_path);
		XmlNodeList nodes = root.ChildNodes;
		
		foreach(XmlNode node in nodes)
		{
			Tbl_AreaMap_Record record = new Tbl_AreaMap_Record((XmlElement)node);
			
			if( true == m_RecordList.ContainsKey( record.Index ) )
			{
				Debug.LogError("Tbl_AreaMapTable::LoadTable()[true == m_RecordList.ContainsKey] index : " + record.Index );
				return;
			}
			m_RecordList.Add( record.Index, record );
		}
	}		
	
	
	public Tbl_AreaMap_Record GetRecord( int iIdx )
	{
		if( false == m_RecordList.ContainsKey( iIdx ) )
		{
			Debug.LogError("Tbl_AreaMapTable::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx );
			return null;
		}
		
		return m_RecordList[iIdx];
	}
}
