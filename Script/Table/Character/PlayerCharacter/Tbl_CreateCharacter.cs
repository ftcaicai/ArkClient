using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_CreateCharacter_Record : AsTableRecord//
{
	int m_Index;public int Index{get{return m_Index;}}
	eRACE m_RaceID;public eRACE Race{get{return m_RaceID;}}
	eCLASS m_ClassID;public eCLASS ClassID{get{return m_ClassID;}}
	
	int m_StartMapId;public int StartMapId{get{return m_StartMapId;}}
	float m_StartPointX;public float StartPointX{get{return m_StartPointX;}}
	float m_StartPointZ;public float StartPointZ{get{return m_StartPointZ;}}
	float m_StartPointRandRatio;public float StartPointRandRatio{get{return m_StartPointRandRatio;}}
	
	int m_Hair1_1;public int Hair1_1{get{return m_Hair1_1;}}
	int m_Hair1_2;public int Hair1_2{get{return m_Hair1_2;}}
	int m_Hair1_3;public int Hair1_3{get{return m_Hair1_3;}}
	int m_Hair1_4;public int Hair1_4{get{return m_Hair1_4;}}
	int m_Hair2_1;public int Hair2_1{get{return m_Hair2_1;}}
	int m_Hair2_2;public int Hair2_2{get{return m_Hair2_2;}}
	int m_Hair2_3;public int Hair2_3{get{return m_Hair2_3;}}
	int m_Hair2_4;public int Hair2_4{get{return m_Hair2_4;}}
	int m_Hair3_1;public int Hair3_1{get{return m_Hair3_1;}}
	int m_Hair3_2;public int Hair3_2{get{return m_Hair3_2;}}
	int m_Hair3_3;public int Hair3_3{get{return m_Hair3_3;}}
	int m_Hair3_4;public int Hair3_4{get{return m_Hair3_4;}}
	
	int m_Body1;public int Body1{get{return m_Body1;}}
	int m_Body2;public int Body2{get{return m_Body2;}}
	int m_Body3;public int Body3{get{return m_Body3;}}
	int m_Body4;public int Body4{get{return m_Body4;}}
	
	int m_Point1;public int Point1{get{return m_Point1;}}
	int m_Point2;public int Point2{get{return m_Point2;}}
	int m_Point3;public int Point3{get{return m_Point3;}}
	int m_Point4;public int Point4{get{return m_Point4;}}
	
	int m_Hand1;public int Hand1{get{return m_Hand1;}}
	int m_Hand2;public int Hand2{get{return m_Hand2;}}
	int m_Hand3;public int Hand3{get{return m_Hand3;}}
	int m_Hand4;public int Hand4{get{return m_Hand4;}}
	
	int m_Weapon;public int Weapon{get{return m_Weapon;}}
	
	int m_DefaultItem1;public int DefaultItem1{get{return m_DefaultItem1;}}
	int m_DefaultItem1Number;public int DefaultItem1Number{get{return m_DefaultItem1Number;}}
	int m_DefaultItem2;public int DefaultItem2{get{return m_DefaultItem2;}}
	int m_DefaultItem2Number;public int DefaultItem2Number{get{return m_DefaultItem2Number;}}
	int m_DefaultItem3;public int DefaultItem3{get{return m_DefaultItem3;}}
	int m_DefaultItem3Number;public int DefaultItem3Number{get{return m_DefaultItem3Number;}}
	int m_DefaultItem4;public int DefaultItem4{get{return m_DefaultItem4;}}
	int m_DefaultItem4Number;public int DefaultItem4Number{get{return m_DefaultItem4Number;}}
	int m_DefaultItem5;public int DefaultItem5{get{return m_DefaultItem5;}}
	int m_DefaultItem5Number;public int DefaultItem5Number{get{return m_DefaultItem5Number;}}
	
	int m_ItemQuickSlot1;public int ItemQuickSlot1{get{return m_ItemQuickSlot1;}}
	int m_ItemQuickSlot2;public int ItemQuickSlot2{get{return m_ItemQuickSlot2;}}
	int m_ItemQuickSlot3;public int ItemQuickSlot3{get{return m_ItemQuickSlot3;}}
	
	int m_SkillQuickSlot1;public int SkillQuickSlot1{get{return m_SkillQuickSlot1;}}
	int m_SkillQuickSlot2;public int SkillQuickSlot2{get{return m_SkillQuickSlot2;}}
	int m_SkillQuickSlot3;public int SkillQuickSlot3{get{return m_SkillQuickSlot3;}}
	
	public Tbl_CreateCharacter_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_Index = int.Parse(node["Index"].InnerText);
			m_RaceID = (eRACE)Enum.Parse(typeof(eRACE), node["Race"].InnerText, true);
			m_ClassID = (eCLASS)Enum.Parse(typeof(eCLASS), node["ClassID"].InnerText, true);
			
			SetValue(ref m_StartMapId, node, "StartMapID");
			SetValue(ref m_StartPointX, node, "StartPointX");
			SetValue(ref m_StartPointZ, node, "StartPointZ");
			SetValue(ref m_StartPointRandRatio, node, "StartPointRandRatio");
			
			SetValue(ref m_Hair1_1, node, "Hair1_1");
			SetValue(ref m_Hair1_2, node, "Hair1_2");
			SetValue(ref m_Hair1_3, node, "Hair1_3");
			SetValue(ref m_Hair1_4, node, "Hair1_4");
			SetValue(ref m_Hair2_1, node, "Hair2_1");
			SetValue(ref m_Hair2_2, node, "Hair2_2");
			SetValue(ref m_Hair2_3, node, "Hair2_3");
			SetValue(ref m_Hair2_4, node, "Hair2_4");
			SetValue(ref m_Hair3_1, node, "Hair3_1");
			SetValue(ref m_Hair3_2, node, "Hair3_2");
			SetValue(ref m_Hair3_3, node, "Hair3_3");
			SetValue(ref m_Hair3_4, node, "Hair3_4");
			
			SetValue(ref m_Body1, node, "Body1");
			SetValue(ref m_Body2, node, "Body2");
			SetValue(ref m_Body3, node, "Body3");
			SetValue(ref m_Body4, node, "Body4");
			
			SetValue(ref m_Point1, node, "Point1");
			SetValue(ref m_Point2, node, "Point2");
			SetValue(ref m_Point3, node, "Point3");
			SetValue(ref m_Point4, node, "Point4");
			
			SetValue(ref m_Hand1, node, "Hand1");
			SetValue(ref m_Hand2, node, "Hand2");
			SetValue(ref m_Hand3, node, "Hand3");
			SetValue(ref m_Hand4, node, "Hand4");
			
			SetValue(ref m_Weapon, node, "Weapon");
			
			SetValue(ref m_DefaultItem1, node, "DefaultItem1");
			SetValue(ref m_DefaultItem1Number, node, "DefaultItem1Number");
			SetValue(ref m_DefaultItem2, node, "DefaultItem2");
			SetValue(ref m_DefaultItem2Number, node, "DefaultItem2Number");
			SetValue(ref m_DefaultItem3, node, "DefaultItem3");
			SetValue(ref m_DefaultItem3Number, node, "DefaultItem3Number");
			SetValue(ref m_DefaultItem4, node, "DefaultItem4");
			SetValue(ref m_DefaultItem4Number, node, "DefaultItem4Number");
			SetValue(ref m_DefaultItem5, node, "DefaultItem5");
			SetValue(ref m_DefaultItem5Number, node, "DefaultItem5Number");
			
			SetValue(ref m_ItemQuickSlot1, node, "ItemQuickSlot1");
			SetValue(ref m_ItemQuickSlot2, node, "ItemQuickSlot2");
			SetValue(ref m_ItemQuickSlot3, node, "ItemQuickSlot3");
			
			SetValue(ref m_SkillQuickSlot1, node, "SkillQuickSlot1");
			SetValue(ref m_SkillQuickSlot2, node, "SkillQuickSlot2");
			SetValue(ref m_SkillQuickSlot3, node, "SkillQuickSlot3");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class Tbl_CreateCharacter_Table : AsTableBase
{
	public Dictionary<eRACE, AsCharacterCreateData> dicRace = new Dictionary<eRACE, AsCharacterCreateData>();
//	SortedList<int, Tbl_CreateCharacter_Record> m_ResourceTable = new SortedList<int, Tbl_CreateCharacter_Record>();
	
	public Tbl_CreateCharacter_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.CREATE_CHARACTER;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
//		try
//		{
//			XmlElement root = GetXmlRootElement(_path);
//			XmlNodeList nodes = root.ChildNodes;
//			
//			foreach(XmlNode node in nodes)
//			{
//				Tbl_CreateCharacter_Record record = new Tbl_CreateCharacter_Record((XmlElement)node);
//				m_ResourceTable.Add(record.Index, record);
//			}
//		}
		try
		{
            XmlElement root = AsTableBase.GetXmlRootElement( _path);
            XmlNodeList nodes = root.ChildNodes;

            foreach( XmlNode node in nodes)
            {
				// <Race>
				AsCharacterCreateData createData = new AsCharacterCreateData( node as XmlElement);
				dicRace.Add( createData.race, createData);
            }
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public AsCharacterCreateGenderData GetGenderData( eRACE _race, eCLASS _class, eGENDER _gender)
	{
		if( false == dicRace.ContainsKey( _race))
			return null;
		
		AsCharacterCreateData data = dicRace[ _race];
		AsCharacterCreateClassData classData = data.GetData( _class);
		if( null == classData)
			return null;
		
		AsCharacterCreateGenderData genderData = classData.GetData( _gender);
		if( null == genderData)
			return null;
		
		return genderData;
	}
	
	public AsCharacterCreateClassData GetClassData( eRACE _race, eCLASS _class)
	{
		if( false == dicRace.ContainsKey( _race))
			return null;
		
		AsCharacterCreateData data = dicRace[ _race];
		AsCharacterCreateClassData classData = data.GetData( _class);
		if( null == classData)
			return null;
		
		return classData;
	}
	
	public AsCharacterCreateData GetRaceData( eRACE race)
	{
		if( false == dicRace.ContainsKey( race))
			return null;
		
		return dicRace[ race];
	}
	
//	public Tbl_CreateCharacter_Record GetRecord(int _id)
//	{
//		if(m_ResourceTable.ContainsKey(_id) == true)
//		{
//			return m_ResourceTable[_id];
//		}
//		
//		Debug.LogError("[Tbl_CreateCharacter_Table]GetRecord: there is no " + _id + " record");
//		return null;
//	}
	
	//public Tbl_CharacterResource_Record GetRecordByTribeAndClass(int _tribe, int _class)
//	public Tbl_CreateCharacter_Record GetRecordByTribeAndClass(eRACE _tribe, eCLASS _class)
//	{
//		foreach(KeyValuePair<int, Tbl_CreateCharacter_Record> pair in m_ResourceTable)
//		{
//			if(pair.Value.Race == _tribe)
//			{
//				if(pair.Value.ClassID == _class)
//				{
//					return pair.Value;
//				}
//			}
//		}
//		
//		Debug.LogError("Tbl_CharacterResource_Table::GetRecordByTribeAndClass: no record");
//		return null;
//	}
//	
//	   /// dopamin begin
//    public SortedList<int, Tbl_CreateCharacter_Record> GetList()
//    {
//        return m_ResourceTable;
//    }
    /// dopamin end 
}
