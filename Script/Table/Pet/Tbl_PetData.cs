using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Pet_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	string m_Name = "";public string Name{get{return m_Name;}}
	int m_Desc;public int Desc{get{return m_Desc;}}
	int m_StarGrade;public int StarGrade{get{return m_StarGrade;}}

	int	m_PersonGroupID;public int PersonGroupID{get{return m_PersonGroupID;}}
	int m_PassiveGroupID = -1;public int PassiveGroupID{get{return m_PassiveGroupID;}}
	int	m_ActiveGroupID;public int ActiveGroupID{get{return m_ActiveGroupID;}}
	int	m_SpecialGroupID;public int SpecialGroupID{get{return m_SpecialGroupID;}}
	int	m_UpgradeID;public int UpgradeID{get{return m_UpgradeID;}}
	string m_Class;public string Class{get{return m_Class;}}

	string m_Icon = "";public string Icon{get{return m_Icon;}}
	string m_Model = "";public string Model{get{return m_Model;}}
	
	public Tbl_Pet_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "PetID");
			SetValue(ref m_Name, node, "Name");
			SetValue(ref m_Desc, node, "Desc");
			SetValue(ref m_StarGrade, node, "StarGrade");

			SetValue(ref m_PersonGroupID, node, "PersonGroupID");
			SetValue(ref m_PassiveGroupID, node, "PassiveGroupID");
			SetValue(ref m_ActiveGroupID, node, "ActiveGroupID");
			SetValue(ref m_SpecialGroupID, node, "SpecialGroupID");
			SetValue(ref m_UpgradeID, node, "UpgradeID");
			SetValue(ref m_Class, node, "Class");

			SetValue(ref m_Icon, node, "Icon");
			SetValue(ref m_Model, node, "Model");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_Pet_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_Name = br.ReadString();
		m_Desc = br.ReadInt32();
		m_StarGrade = br.ReadInt32();

		m_PersonGroupID = br.ReadInt32();
		m_PassiveGroupID = br.ReadInt32();
		m_ActiveGroupID = br.ReadInt32();
		m_SpecialGroupID = br.ReadInt32();
		m_UpgradeID = br.ReadInt32();
		m_Class = br.ReadString();

		m_Icon = br.ReadString();
		m_Model = br.ReadString();
	}

	public int GetNextMaxExp(int _curLv)
	{
		Tbl_PetLevel_Record gradeRec = AsTableManager.Instance.GetPetLevelRecord(m_StarGrade);
		if(gradeRec == null)
		{
			Debug.LogError("Tbl_Pet_Record:: GetNextMaxExp: invalid star grade. grade = " + m_StarGrade);
			return -1;
		}
		
		int maxExp = 0;
		if(gradeRec.listData.ContainsKey(_curLv + 1) == true)
			maxExp = gradeRec.listData[_curLv + 1].Exp;
		else if(_curLv == AsTableManager.sPetSkillLevelCount)
			maxExp = gradeRec.listData[AsTableManager.sPetSkillLevelCount].Exp;

		return maxExp;
	}

	public int GetPrevMaxExp(int _curLv)
	{
		Tbl_PetLevel_Record gradeRec = AsTableManager.Instance.GetPetLevelRecord(m_StarGrade);
		if(gradeRec == null)
		{
			Debug.LogError("Tbl_Pet_Record:: GetNextMaxExp: invalid star grade. grade = " + m_StarGrade);
			return -1;
		}
		
		int maxExp = 0;
		if(gradeRec.listData.ContainsKey(_curLv) == true)
			maxExp = gradeRec.listData[_curLv].Exp;
		else
			Debug.LogError("Tbl_Pet_Record:: GetNextMaxExp: invalid current level. _curLv = " + _curLv);

		if(_curLv == AsTableManager.sPetSkillLevelCount)
			maxExp = gradeRec.listData[AsTableManager.sPetSkillLevelCount - 1].Exp;
		
		return maxExp;
	}
}

public class Tbl_Pet_Table : AsTableBase {
	
	SortedList<int, Tbl_Pet_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Pet_Record>();
	
	public Tbl_Pet_Table(string _path)
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
					Tbl_Pet_Record record = new Tbl_Pet_Record( br);
					m_ResourceTable.Add(record.Index, record);
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
					Tbl_Pet_Record record = new Tbl_Pet_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_Pet_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Pet_Record]GetRecord: there is no record [" + _id + "]");
		return null;
	}
}
