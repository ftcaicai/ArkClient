using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class Tbl_SynOptionChange_Record : AsTableRecord
{		
	
	public int level;
	public Item.eGRADE grade;
	public Item.eEQUIP equip;
	
	//public int stuffID = 0;
	public int stuffCount = 0;
	public int cost = 0;
	
	
	public Tbl_SynOptionChange_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;				
			
			SetValue( ref level, node, "ItemLevel" );		
			SetValue<Item.eGRADE>(ref grade, node, "ItemGrade");
			SetValue<Item.eEQUIP>(ref equip, node, "ItemPart");
			
			//SetValue( ref stuffID, node, "StuffID" );      
			SetValue( ref stuffCount, node, "StuffCount" );         
			SetValue( ref cost, node, "CostGold" );			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SynOptionChange_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}	
}

public class Tbl_SynOptionChange_Table : AsTableBase 
{
	
	List<Tbl_SynOptionChange_Record> m_recordList = new List<Tbl_SynOptionChange_Record>();
	
	public Tbl_SynOptionChange_Table(string _path)
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
			Tbl_SynOptionChange_Record record = new Tbl_SynOptionChange_Record((XmlElement)node);			
			m_recordList.Add( record );
		}		
	}		
	
	public Tbl_SynOptionChange_Record GetRecord( int _level, Item.eGRADE _eGrade, Item.eEQUIP _equip )
	{		
		foreach( Tbl_SynOptionChange_Record _record in m_recordList )
		{		
			if( _record.level == _level && _record.grade == _eGrade && _record.equip == _equip )
				return _record;
		}
		
		return null;
	}
	
}
