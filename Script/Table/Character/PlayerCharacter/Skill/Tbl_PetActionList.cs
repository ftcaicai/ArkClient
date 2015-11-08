using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_PetAction_Table : AsTableBase {
	SortedList<int, Tbl_Action_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Action_Record>();
	
	Dictionary<string, Dictionary<string, Tbl_Action_Record>> m_ddicAction = 
		new Dictionary<string, Dictionary<string, Tbl_Action_Record>>();
	
	Dictionary<string, Dictionary<string, Tbl_Action_Animation>> m_ddicAnimation = 
		new Dictionary<string, Dictionary<string, Tbl_Action_Animation>>();
	
	public Tbl_PetAction_Table(string _path)
	{
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
//		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_Action_Record record = new Tbl_Action_Record((XmlElement)node, Tbl_Action_Record.eType.Pet);
				m_ResourceTable.Add(record.Index, record);
				
				string _class = record.ClassName;
				
				// act
				if(m_ddicAction.ContainsKey(_class) == false)
					m_ddicAction.Add(_class, new Dictionary<string, Tbl_Action_Record>());
				
				m_ddicAction[_class].Add(record.ActionName, record);
				
				// anim
				if(m_ddicAnimation.ContainsKey(_class) == false)
					m_ddicAnimation.Add(_class, new Dictionary<string, Tbl_Action_Animation>());
				
				if( record.ReadyAnimation != null)
				{
					if( m_ddicAnimation[_class].ContainsKey(record.ReadyAnimation.FileName) == false)
						m_ddicAnimation[_class].Add(record.ReadyAnimation.FileName, record.ReadyAnimation);
				}
				if( record.HitAnimation != null)
				{
					if( m_ddicAnimation[_class].ContainsKey(record.HitAnimation.FileName) == false)
						m_ddicAnimation[_class].Add(record.HitAnimation.FileName, record.HitAnimation);
				}
				if( record.FinishAnimation != null)
				{
					if( m_ddicAnimation[_class].ContainsKey(record.FinishAnimation.FileName) == false)
						m_ddicAnimation[_class].Add(record.FinishAnimation.FileName, record.FinishAnimation);
				}
			}
//		}
//		catch(System.Exception e)
//		{
//			Debug.LogError(e);
//		}
	}
	
	public Tbl_Action_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogWarning("[Tbl_PetAction_Table]GetRecord: there is no record = " + _id);
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
		
		Debug.LogError("[Tbl_PetAction_Table]GetRecord: there is no record = " + _class + ", " + _act);
		return null;
	}
	
	public Tbl_Action_Animation GetActionAnimation(string _class, string _animName)
	{
		if(m_ddicAnimation.ContainsKey(_class) == true)
		{
			if(m_ddicAnimation[_class].ContainsKey(_animName) == true)
			{
				return m_ddicAnimation[_class][_animName];
			}
		}
		
		Debug.LogError("[Tbl_PetAction_Table]GetActionAnimation: there is no record = " + _class + ", " + _animName);
		return null;
	}
}
