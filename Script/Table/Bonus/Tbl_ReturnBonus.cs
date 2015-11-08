using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_ReturnBonus_Record : AsTableRecord//
{
	int m_Index;public int Index{get{return m_Index;}}
	int m_BonusItem_1;public int BonusItem_1{get{return m_BonusItem_1;}}
	int m_BonusItem_2;public int BonusItem_2{get{return m_BonusItem_2;}}
	int m_BonusItem_3;public int BonusItem_3{get{return m_BonusItem_3;}}
	
	public Tbl_ReturnBonus_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
						
			SetValue(ref m_Index, node, "ReturnBonus_ID");
			SetValue(ref m_BonusItem_1, node, "ReturnBonusItem1");
			SetValue(ref m_BonusItem_2, node, "ReturnBonusItem2");
			SetValue(ref m_BonusItem_3, node, "ReturnBonusItem3");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class Tbl_ReturnBonus_Table : AsTableBase {
	SortedList<int, Tbl_ReturnBonus_Record> m_ResourceTable = 
		new SortedList<int, Tbl_ReturnBonus_Record>();
	
	public Tbl_ReturnBonus_Table(string _path)
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
				Tbl_ReturnBonus_Record record = new Tbl_ReturnBonus_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_ReturnBonus_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_ReturnBonus_Record]GetRecord: there is no record");
		return null;
	}
}
