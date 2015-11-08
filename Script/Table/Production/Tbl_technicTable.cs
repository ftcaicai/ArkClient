using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public class Tbl_Technic_Record : AsTableRecord
{	
	eITEM_PRODUCT_TECHNIQUE_TYPE m_type;
	int m_iLevel;
	int m_exp;
	int m_Miracle;
	
	public eITEM_PRODUCT_TECHNIQUE_TYPE getType
	{
		get
		{
			return m_type;
		}
	}
	
	public int getLevel
	{
		get
		{
			return m_iLevel;
		}
	}
	
	public int getExp
	{
		get
		{
			return m_exp;
		}
	}
	
	public int getMiracle
	{
		get
		{
			return m_Miracle;
		}
		
	}
	
	public Tbl_Technic_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue<eITEM_PRODUCT_TECHNIQUE_TYPE>(ref m_type, node, "Technic_Type");
			SetValue(ref m_iLevel, node, "Technic_Level");
			SetValue(ref m_exp, node, "Technic_Expertism");		
			SetValue(ref m_Miracle, node, "Technic_Miracle");		
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class Tbl_Technic_Table : AsTableBase 
{	
	Dictionary<eITEM_PRODUCT_TECHNIQUE_TYPE, Dictionary<int,Tbl_Technic_Record> >
		m_recordList = new Dictionary<eITEM_PRODUCT_TECHNIQUE_TYPE, Dictionary<int, Tbl_Technic_Record> >(); // type, level, data
	
	int[] m_levelMaxList = new int[(int)eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX];
	
	public Tbl_Technic_Table(string _path)
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
			Tbl_Technic_Record record = new Tbl_Technic_Record((XmlElement)node);
			if( false == m_recordList.ContainsKey( record.getType ) )
			{
				m_recordList.Add( record.getType, new Dictionary<int,Tbl_Technic_Record>() );				
			}
			
			m_recordList[ record.getType ].Add( record.getLevel, record );	
			
			
			if( m_levelMaxList[(int)record.getType] < record.getLevel )
			{
				m_levelMaxList[(int)record.getType] = record.getLevel;
			}			
		}
	}
	
	public Tbl_Technic_Record GetRecord( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, int iLevel )
	{
		if(false == m_recordList.ContainsKey(_eType))
		{
			//AsUtil.ShutDown("Tbl_technicTable::GetRecord()[ no exist type : " + _eType );
			return null;
		}
		
		if(false == m_recordList[_eType].ContainsKey( iLevel ) )
		{
			//AsUtil.ShutDown("Tbl_technicTable::GetRecord()[ no exist type : " + _eType + " level : " + iLevel );
			return null;		
		}
		
		
		return m_recordList[_eType][iLevel];
	}	
	
	public int GetMaxLevel( int iIndex )
	{
		if( m_levelMaxList.Length <= iIndex )	
		{
			return 0;
		}
		
		return m_levelMaxList[iIndex];
	}
	
}
