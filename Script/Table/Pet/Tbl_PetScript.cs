using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_PetScript_Record : AsTableRecord
{
	public enum eType {Login, Eat, Dead, Idle, Battle, Hungry}

	int m_GroupID;public int GroupID{get{return m_GroupID;}}
	int m_PersonName;public int PersonName{get{return m_PersonName;}}

	List<int> m_listLoginString = new List<int>(); public List<int> listLoginString{get{return m_listLoginString;}}
	List<int> m_listEatString = new List<int>(); public List<int> listEatString{get{return m_listEatString;}}
	List<int> m_listDeadString = new List<int>(); public List<int> listDeadString{get{return m_listDeadString;}}
	List<int> m_listIdleString = new List<int>(); public List<int> listIdleString{get{return m_listIdleString;}}
	List<int> m_listBattleString = new List<int>(); public List<int> listBattleString{get{return m_listBattleString;}}
	List<int> m_listHungryString = new List<int>(); public List<int> listHungryString{get{return m_listHungryString;}}

	int m_HungryPush;public int HungryPush{get{return m_HungryPush;}}
	
	public Tbl_PetScript_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;

			SetValue(ref m_GroupID, node, "GroupID");
			SetValue(ref m_PersonName, node, "PersonName");

			int temp = 0;
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "LoginString" + i);
				if(temp != int.MaxValue) m_listLoginString.Add(temp);
			}
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "EatString" + i);
				if(temp != int.MaxValue) m_listEatString.Add(temp);
			}
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "DeadString" + i);
				if(temp != int.MaxValue) m_listDeadString.Add(temp);
			}
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "IdleString" + i);
				if(temp != int.MaxValue) m_listIdleString.Add(temp);
			}
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "BattleString" + i);
				if(temp != int.MaxValue) m_listBattleString.Add(temp);
			}
			for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
			{
				SetValue(ref temp, node, "HungryString" + i);
				if(temp != int.MaxValue) m_listHungryString.Add(temp);
			}

			SetValue(ref m_HungryPush, node, "HungryPush");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetScript_Record(BinaryReader br)
	{
		m_GroupID = br.ReadInt32();
		m_PersonName = br.ReadInt32();

		int temp = 0;
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listLoginString.Add(temp);
		}
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listEatString.Add(temp);
		}
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listDeadString.Add(temp);
		}
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listIdleString.Add(temp);
		}
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listBattleString.Add(temp);
		}
		for(int i=1; i<=AsTableManager.sPetScriptCount; ++i)
		{
			temp = br.ReadInt32(); if(temp != int.MaxValue) m_listHungryString.Add(temp);
		}

		m_HungryPush = br.ReadInt32();
	}

	public string GetScriptString(eType _type)
	{
		int idx = 0;
		switch(_type)
		{
		case eType.Login:
			idx = m_listLoginString.GetRandomValue();
			break;
		case eType.Eat:
			idx = m_listEatString.GetRandomValue();
			break;
		case eType.Dead:
			idx = m_listDeadString.GetRandomValue();
			break;
		case eType.Idle:
			idx = m_listIdleString.GetRandomValue();
			break;
		case eType.Battle:
			idx = m_listBattleString.GetRandomValue();
			break;
		case eType.Hungry:
			idx = m_listHungryString.GetRandomValue();
			break;
		}

		return AsTableManager.Instance.GetTbl_String(idx);
	}
}

public class Tbl_PetScript_Table : AsTableBase {
	
	SortedList<int, Tbl_PetScript_Record> m_ResourceTable = 
		new SortedList<int, Tbl_PetScript_Record>();
	
	public Tbl_PetScript_Table(string _path)
	{
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try
		{
			if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || true == AsTableManager.Instance.useReadBinary )
			{
				// Ready Binary
				TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
				MemoryStream stream = new MemoryStream( textAsset.bytes);
				BinaryReader br = new BinaryReader( stream);
				
				int nCount = br.ReadInt32();
				
				for( int i = 0; i < nCount; i++)
				{
					Tbl_PetScript_Record record = new Tbl_PetScript_Record( br);
					m_ResourceTable.Add(record.GroupID, record);
				}
				
				br.Close();
				stream.Close();
			}
			else
			{
				XmlElement root = GetXmlRootElement(_path);
				XmlNodeList nodes = root.ChildNodes;
				
				foreach(XmlNode node in nodes)
				{
					Tbl_PetScript_Record record = new Tbl_PetScript_Record((XmlElement)node);
					m_ResourceTable.Add(record.GroupID, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetScript_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_PetScript_Record]GetRecord: there is no record [" + _id + "]");
		return null;
	}
}
