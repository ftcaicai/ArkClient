using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public enum eBuffOverlapType {Buff, Debuff, Positive, Negative, Penalty}

public class Tbl_BuffOverlap_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	
	eBUFFTYPE m_BuffPotencyType = eBUFFTYPE.eBUFFTYPE_MAX;public eBUFFTYPE BuffPotencyType{get{return m_BuffPotencyType;}}
	int m_OverlapCount = -1;public int OverlapCount{get{return m_OverlapCount;}}
	int m_OverlapMax = -1;public int OverlapMax{get{return m_OverlapMax;}}
	int m_OverlapMin = -1;public int OverlapMin{get{return m_OverlapMin;}}
	eBuffOverlapType m_BuffOverlapType = eBuffOverlapType.Buff;public eBuffOverlapType BuffOverlapType{get{return m_BuffOverlapType;}}
	
	public Tbl_BuffOverlap_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			
			m_BuffPotencyType = (eBUFFTYPE)m_Index;
//			SetValue<eBUFFTYPE>(ref m_BuffPotencyType, node, "BuffPotencyType");
			SetValue(ref m_OverlapCount, node, "OverlapCount");
			SetValue(ref m_OverlapMax, node, "OverlapMax");
			SetValue(ref m_OverlapMin, node, "OverlapMin");
			SetValue<eBuffOverlapType>(ref m_BuffOverlapType, node, "BuffType");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);//
		}
	}
}

public class Tbl_BuffOverlap_Table : AsTableBase {
	SortedList<int, Tbl_BuffOverlap_Record> m_ResourceTable = 
		new SortedList<int, Tbl_BuffOverlap_Record>();
	
	Dictionary<eBUFFTYPE, Tbl_BuffOverlap_Record> m_dicBuffType =
		new Dictionary<eBUFFTYPE, Tbl_BuffOverlap_Record>();
	
	public Tbl_BuffOverlap_Table(string _path)
	{		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try
		{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_BuffOverlap_Record record = new Tbl_BuffOverlap_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
				
				m_dicBuffType.Add((eBUFFTYPE)record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_BuffOverlap_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_BuffOverlap_Record]GetRecord: there is no [" + _id + "] record");
		return null;
	}
	
	public Tbl_BuffOverlap_Record GetRecord(eBUFFTYPE _type)
	{
		if(m_dicBuffType.ContainsKey(_type) == true)
		{
			return m_dicBuffType[_type];
		}
		
		Debug.LogError("[Tbl_BuffOverlap_Record]GetRecord: there is no [" + _type + "] record");
		return null;
	}
}
