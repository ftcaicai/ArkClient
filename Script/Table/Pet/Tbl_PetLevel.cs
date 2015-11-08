using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class PetLevelData
{
	int m_Level;public int Level{get{return m_Level;}}
	int m_Exp;public int Exp{get{return m_Exp;}}
	int m_PassiveSkillLv;public int PassiveSkillLv{get{return m_PassiveSkillLv;}}
	int m_ActiveSkillLv;public int ActiveSkillLv{get{return m_ActiveSkillLv;}}
	int m_SpecialSkillLv;public int SpecialSkillLv{get{return m_SpecialSkillLv;}}

	public PetLevelData(int _lv, int _exp, int _passive, int _active, int _special)
	{
		m_Level = _lv;
	    m_Exp = _exp; 
	    m_PassiveSkillLv = _passive;
	    m_ActiveSkillLv = _active;
	    m_SpecialSkillLv = _special;
	}
}

public class Tbl_PetLevel_Record : AsTableRecord
{
	int m_StarGrade; public int StarGrade{get{return m_StarGrade;}}
	SortedList<int, PetLevelData> m_listData = new SortedList<int, PetLevelData>(); public SortedList<int, PetLevelData> listData{get{return m_listData;}}
	
	public Tbl_PetLevel_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			SetAttribute(ref m_StarGrade, node, "StarGrade");

			XmlNodeList nodes = node.ChildNodes;
			foreach(XmlNode node1 in nodes)
			{
				int lv = 1; int exp = 0; int passive = 0; int active = 0; int special = 0;

				SetAttribute(ref lv, node1, "Level");
				SetAttribute(ref exp, node1, "Exp");
				SetAttribute(ref passive, node1, "PassiveSkillLv");
				SetAttribute(ref active, node1, "ActiveSkillLv");
				SetAttribute(ref special, node1, "SpecialSkillLv");

				m_listData.Add(lv, new PetLevelData(lv, exp, passive, active, special));
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetLevel_Record(BinaryReader br)
	{
		m_StarGrade = br.ReadInt32();
		
		for(int i=1; i<=AsTableManager.sPetSkillLevelCount; ++i)
		{
			int lv = br.ReadInt32();
			int exp = br.ReadInt32(); 
			int passive = br.ReadInt32();
			int active = br.ReadInt32();
			int special = br.ReadInt32();

			m_listData.Add(lv, new PetLevelData(lv, exp, passive, active, special));
		}
	}
}

public class Tbl_PetLevel_Table : AsTableBase {
	
	SortedList<int, Tbl_PetLevel_Record> m_ResourceTable = 
		new SortedList<int, Tbl_PetLevel_Record>();
	
	public Tbl_PetLevel_Table(string _path)
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
					Tbl_PetLevel_Record record = new Tbl_PetLevel_Record( br);
					m_ResourceTable.Add(record.StarGrade, record);
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
					Tbl_PetLevel_Record record = new Tbl_PetLevel_Record((XmlElement)node);
					m_ResourceTable.Add(record.StarGrade, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetLevel_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		if( _id == m_ResourceTable.Count + 1)
			Debug.LogWarning("[Tbl_PetLevel_Record]GetRecord: maximum level = " + _id);
		else
			Debug.LogError("[Tbl_PetLevel_Record]GetRecord: there is no record [" + _id + "]");
		
		return null;
	}
}
