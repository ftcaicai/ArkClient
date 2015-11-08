using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_MonsterAction_Table : AsTableBase {
	SortedList<int, Tbl_Action_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Action_Record>();
	
//	MultiDictionary<eCLASS, Dictionary<string, Tbl_Action_Record>> m_mddicAction = 
//		new MultiDictionary<eCLASS, Dictionary<string, Tbl_Action_Record>>();
	
	Dictionary<string, Dictionary<string, Tbl_Action_Record>> m_ddicAction = 
		new Dictionary<string, Dictionary<string, Tbl_Action_Record>>();
	
	Dictionary<string, Dictionary<string, Tbl_Action_Animation>> m_ddicActionAnim = 
		new Dictionary<string, Dictionary<string, Tbl_Action_Animation>>();
	
	public Tbl_MonsterAction_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			Tbl_Action_Record record = null;
			
			foreach(XmlNode node in nodes)
			{
				record = new Tbl_Action_Record((XmlElement)node, Tbl_Action_Record.eType.Monster);
				
				if( true == m_ResourceTable.ContainsKey( record.Index))
					Debug.LogError( "Duplicated index(" + record.Index + ")");
				m_ResourceTable.Add(record.Index, record);
				
				string _class = record.ClassName;

				if(m_ddicAction.ContainsKey(_class) == false)
					m_ddicAction.Add(_class, new Dictionary<string, Tbl_Action_Record>());
				
				if( true == m_ddicAction[ _class].ContainsKey( record.ActionName))
				{
					Debug.LogError( "Duplicated key : record.Index = " + record.Index + ", record.ActionName = " + record.ActionName);
					continue;
				}
				
				m_ddicAction[_class].Add(record.ActionName, record);
//				Debug.Log("Loading ActionListList table : Class:" + _class + ", action:" + act);
				
				if(m_ddicActionAnim.ContainsKey(_class) == false)
					m_ddicActionAnim.Add(_class, new Dictionary<string, Tbl_Action_Animation>());
				
//				try{
				
				if(record.ReadyAnimation != null && m_ddicActionAnim[_class].ContainsKey(record.ReadyAnimation.FileName) == false)
					m_ddicActionAnim[_class].Add(record.ReadyAnimation.FileName, record.ReadyAnimation);
				if(record.HitAnimation != null && m_ddicActionAnim[_class].ContainsKey(record.HitAnimation.FileName) == false)
					m_ddicActionAnim[_class].Add(record.HitAnimation.FileName, record.HitAnimation);
				if(record.FinishAnimation != null && m_ddicActionAnim[_class].ContainsKey(record.FinishAnimation.FileName) == false)
					m_ddicActionAnim[_class].Add(record.FinishAnimation.FileName, record.FinishAnimation);
				
//				}
//				catch
//				{
//					if(record.ReadyAnimation != null)
//						Debug.LogError("record.ReadyAnimation=" + record.ReadyAnimation.FileName);
//					if(record.HitAnimation != null)
//						Debug.LogError("record.HitAnimation=" + record.HitAnimation.FileName);
//					if(record.FinishAnimation != null)
//						Debug.LogError("record.FinishAnimation=" + record.FinishAnimation.FileName);
//				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_Action_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_ActionList_Table]GetRecord: there is no record = " + _id);
		return null;
	}
	
	public Tbl_Action_Record GetRecord(string _class, string _act)
	{
		if(m_ddicAction.ContainsKey(_class) == true)
		{
			if(m_ddicAction[_class].ContainsKey(_act) == true)
			{
				return m_ddicAction[_class][_act];
			}
		}
		
		Debug.LogWarning("[Tbl_ActionList_Table]GetRecord: there is no record = " + _class + ", " + _act);
		return null;
	}
	
	public Tbl_Action_Animation GetActionAnimation(string _class, string _animName)
	{
		if(m_ddicActionAnim.ContainsKey(_class) == true)
		{
			if(m_ddicActionAnim[_class].ContainsKey(_animName) == true)
			{
				return m_ddicActionAnim[_class][_animName];
			}
		}
		
//		Debug.LogError("[Tbl_MonsterActionList_Table]GetRecord: " + _class + " doesnt have " + _animName + " animation");
		return null;
	}
	
	public SortedList<int, Tbl_Action_Record> GetTable()
	{
		return m_ResourceTable;
	}
}
