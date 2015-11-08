using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class Tbl_PvpGrade_Record : AsTableRecord
{
	int m_ID;
	int m_GradeNameID;
	int m_GradePoint;
	int m_GradeProp;
	int m_GradeImageType;
	string m_strGradeTownEffect;
	string m_strGradeColor;
	
	public int ID{ get{ return m_ID;}}
	public int GradeNameID{ get{ return m_GradeNameID;}}
	public int GradePoint{ get{ return m_GradePoint;}}
	public int GradeProp{ get{ return m_GradeProp;}}
	public int GradeImageType{ get{ return m_GradeImageType;}}
	public string GradeTownEffect{ get{ return m_strGradeTownEffect;}}
	public string GradeColor{ get{ return m_strGradeColor;}}
	
	public Tbl_PvpGrade_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_ID, node, "ID");
			SetValue(ref m_GradeNameID, node, "GradeNameID");
			SetValue(ref m_GradePoint, node, "GradePoint");
			SetValue(ref m_GradeProp, node, "GradeProp");
			SetValue(ref m_GradeImageType, node, "GradeImageType");
			SetValue(ref m_strGradeTownEffect, node, "GradeTownEffect");
			SetValue(ref m_strGradeColor, node, "GradeColor");
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_PvpGrade_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_PvpGrade_Table : AsTableBase
{
	Dictionary<int, Tbl_PvpGrade_Record> m_PvpGradeTable = new Dictionary<int, Tbl_PvpGrade_Record>();
	
	public Tbl_PvpGrade_Table(string _path)
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
			Tbl_PvpGrade_Record record = new Tbl_PvpGrade_Record((XmlElement)node);
			m_PvpGradeTable.Add(record.ID, record);
		}
	}
	
	public Tbl_PvpGrade_Record GetRecord(int _id)
	{
		if(m_PvpGradeTable.ContainsKey(_id) == true)
			return m_PvpGradeTable[_id];
		
		Debug.LogError("[Tbl_PvpGradeTable_Table]GetRecord: there is no record");
		return null;
	}
	
	public int GetCount()
	{
		return m_PvpGradeTable.Count;
	}
}
