using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.Xml;
using System.IO;
using System.Text;


public class Tbl_BuffMinMaxTable_Record: AsTableRecord
{
	public enum eBuffTYPE
	{
		PhysicalAttackMinMax,
		MagicalAttackMinMax,
		PhysicalDefenseMinMax,
		MagicalResistMinMax,
		AttackSpeedMinMax,
		MoveSpeedMinMax,
		ReceiveDamageMinMax,
		ManaSpendMinMax,
		CoolTimeMinMax,
		AccuracyMinMax,
		DodgeMinMax,
		DebuffResistMinMax,
	}

	public eBuffTYPE bufftype;
	public int max;
	public int min;
	
		
	public Tbl_BuffMinMaxTable_Record(XmlElement _element)// : base(_element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue<eBuffTYPE>(ref bufftype, node, "MinMaxLimitType");
			SetValue(ref max, node, "OverlapMax");
			SetValue(ref min, node, "OverlapMin");
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_BuffMinMaxTable_Record] : " + e );
		}
	}
}

public class Tbl_BuffMinMaxTable_Table : AsTableBase 
{
	
	Dictionary<Tbl_BuffMinMaxTable_Record.eBuffTYPE, Tbl_BuffMinMaxTable_Record> m_list = 
					new Dictionary<Tbl_BuffMinMaxTable_Record.eBuffTYPE, Tbl_BuffMinMaxTable_Record>();

	
	public Tbl_BuffMinMaxTable_Table(string _path)
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
			Tbl_BuffMinMaxTable_Record record = new Tbl_BuffMinMaxTable_Record((XmlElement)node);
			
			if( true == m_list.ContainsKey( record.bufftype ) )
			{
				Debug.LogError("Tbl_BuffMinMaxTable_Table::LoadTable()[ bufftype : " + record.bufftype );
				continue;
			}
			m_list.Add( record.bufftype, record );		
		}
	}
	
	public Tbl_BuffMinMaxTable_Record GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE _bufftype ) 
	{
		if(m_list.ContainsKey(_bufftype) == true)
			return m_list[_bufftype];
		
		Debug.LogError("[Tbl_BuffMinMaxTable_Table]GetRecord: there is no record : " + _bufftype );
		return null;
	}
	
}
