using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class AttendBonusItem
{
	int		nItemTableIndex;	public int ItemTableIndex{get{return nItemTableIndex;}}
	int		nItemCount;			public int ItemCount{get{return nItemCount;}}
	
	public AttendBonusItem( int nIndex , int nCount )
	{
		nItemTableIndex = nIndex;
		nItemCount = nCount;
	}
}

public class Tbl_AttendBonus_Record : AsTableRecord//
{
	int m_Index;public int Index{get{return m_Index;}}
	int m_Level;public int Level{get{return m_Level;}}
	
	Dictionary<eCLASS, AttendBonusItem> m_dicAttendanceItem = new Dictionary<eCLASS, AttendBonusItem>();
	
	public Tbl_AttendBonus_Record(XmlElement _element)// : base(_element)
	{
		int					nItemTableIndex = 0;
		int					nItemCount = 0;
		AttendBonusItem		_attendBonusItem;
		
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index			, node, "AttendanceBonus_ID");
			SetValue(ref m_Level			, node, "Attendance_Lv");
			
			//	eCLASS.DIVINEKNIGHT
			SetValue(ref nItemTableIndex	, node, "Knight_Reward");			
			SetValue(ref nItemCount			, node, "Knight_RewardCount");
			_attendBonusItem = new AttendBonusItem(nItemTableIndex,nItemCount);
			m_dicAttendanceItem.Add(eCLASS.DIVINEKNIGHT, _attendBonusItem);
			
			//	eCLASS.MAGICIAN
			SetValue(ref nItemTableIndex	, node, "Magician_Reward");			
			SetValue(ref nItemCount			, node, "Magician_RewardCount");
			_attendBonusItem = new AttendBonusItem(nItemTableIndex,nItemCount);
			m_dicAttendanceItem.Add(eCLASS.MAGICIAN, _attendBonusItem);
			
			//	eCLASS.CLERIC
			SetValue(ref nItemTableIndex	, node, "Cleric_Reward");			
			SetValue(ref nItemCount			, node, "Cleric_RewardCount");
			_attendBonusItem = new AttendBonusItem(nItemTableIndex,nItemCount);
			m_dicAttendanceItem.Add(eCLASS.CLERIC, _attendBonusItem);
			
			//	eCLASS.HUNTER
			SetValue(ref nItemTableIndex	, node, "Hunter_Reward");			
			SetValue(ref nItemCount			, node, "Hunter_RewardCount");
			_attendBonusItem = new AttendBonusItem(nItemTableIndex,nItemCount);
			m_dicAttendanceItem.Add(eCLASS.HUNTER, _attendBonusItem);
			
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public AttendBonusItem	GetAttendBonusItem( eCLASS	_class )
	{
		if( m_dicAttendanceItem.ContainsKey( _class ) == false )
			return null;
		
		return m_dicAttendanceItem[_class];
	}
}

public class Tbl_AttendBonus_Table : AsTableBase {
	SortedList<int, Tbl_AttendBonus_Record> m_ResourceTable = 
		new SortedList<int, Tbl_AttendBonus_Record>();
	
	public Tbl_AttendBonus_Table(string _path)
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
				Tbl_AttendBonus_Record record = new Tbl_AttendBonus_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_AttendBonus_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_AttendBonus_Table]GetRecord: there is no record");
		return null;
	}
}
