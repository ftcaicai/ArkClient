using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Projectile_Record : AsTableRecord
{
	int					m_Index;public int Index{get{return m_Index;}}
	string				m_Projectile_FileName;public string Projectile_FileName{get{return m_Projectile_FileName;}}
	float				m_Projectile_Speed;public float Projectile_Speed{get{return m_Projectile_Speed;}}
	float				m_Projectile_Acceleration;public float Projectile_Acceleration{get{return m_Projectile_Acceleration;}}
	eProjectilePath	m_Projectile_Path;public eProjectilePath Projectile_Path{get{return m_Projectile_Path;}}

	public Tbl_Projectile_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetStringFullPath(ref m_Projectile_FileName, node, "Projectile_FileName");
			SetValue(ref m_Projectile_Speed, node, "Projectile_Speed");
			
			SetValue(ref m_Projectile_Acceleration, node, "Projectile_Acceleration");
			SetValue<eProjectilePath>(ref m_Projectile_Path, node, "Projectile_Path");
		}
		catch(System.Exception e)
		{
			//Debug.LogError("[Tbl_Action_Record] 'constructor':|" + e + "| error while parsing");
			Debug.LogError(e);
		}
	}
}

public class Tbl_Projectile_Table : AsTableBase {
	SortedList<int, Tbl_Projectile_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Projectile_Record>();
	
	public Tbl_Projectile_Table(string _path)
	{
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_Projectile_Record record = new Tbl_Projectile_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Action_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_Projectile_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Projectile_Table]GetRecord: there is no record = " + _id);
		return null;
	}
	
	public SortedList<int, Tbl_Projectile_Record> GetTable()
	{
		return m_ResourceTable;
	}
}
