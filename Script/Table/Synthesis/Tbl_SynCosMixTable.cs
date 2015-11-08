using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class Tbl_SynCosMix_Record : AsTableRecord
{	
	public int					cosMixId;
	public Item.eGRADE		targetItemGrade;
	public Item.eEQUIP		targetItemKind;
	public int					needExp;
	public int					upgradeCostNormal;
	public int					upgradeCostSpecial;

	public Tbl_SynCosMix_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;				
			
			SetValue(ref cosMixId, node, "Cos_Mix_ID");
			SetValue<Item.eGRADE>(ref targetItemGrade, node, "TargetItem_Grade");
			SetValue<Item.eEQUIP>(ref targetItemKind, node, "TargetItem_Kind");
			SetValue(ref needExp, node, "NeedEXP");
			SetValue(ref upgradeCostNormal, node, "UpgradeCost_Normal");
			SetValue(ref upgradeCostSpecial, node, "UpgradeCost_Special");
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SynCosMix_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}	
}

public class Tbl_SynCosMix_Table : AsTableBase 
{
	
	List<Tbl_SynCosMix_Record> m_recordList = new List<Tbl_SynCosMix_Record>();
	
	public Tbl_SynCosMix_Table(string _path)
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
			Tbl_SynCosMix_Record record = new Tbl_SynCosMix_Record((XmlElement)node);			
			m_recordList.Add( record );
		}		
	}		
	
	public Tbl_SynCosMix_Record GetRecord( Item.eGRADE _grade , Item.eEQUIP _equip )
	{		
		foreach( Tbl_SynCosMix_Record _record in m_recordList )
		{		
			if( _record.targetItemGrade == _grade && _record.targetItemKind == _equip )
				return _record;
		}
		
		return null;
	}
	
}