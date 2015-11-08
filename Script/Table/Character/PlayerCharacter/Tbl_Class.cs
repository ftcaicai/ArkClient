using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Class_Record : AsTableRecord//
{
	int m_Index;public int Index{get{return m_Index;}}
	eRACE m_RaceId;public eRACE Race{get{return m_RaceId;}}
	eCLASS m_Class;public eCLASS Class{get{return m_Class;}}
	eATTACK_TYPE m_AttackType;public eATTACK_TYPE attackType{get{return m_AttackType;}}
	
//	eGENDER m_Gender;public eGENDER Gender{get{return m_Gender;}}
	
	string m_ModelingPath_Male;public string ModelingPath_Male{get{return m_ModelingPath_Male;}}
	string m_ModelingPath_Female;public string ModelingPath_Female{get{return m_ModelingPath_Female;}}
	
	int m_Portrait_Male;public int Portrait_Male{get{return m_Portrait_Male;}}
	int m_Portrait_Female;public int Portrait_Female{get{return m_Portrait_Female;}}
	
	
	string m_JobIcon;public string JobIcon{get{return m_JobIcon;}}
	
	int m_JobStep;public int JobStep{get{return m_JobStep;}}
	int m_BaseJob;public int BaseJob{get{return m_BaseJob;}}
//	int m_ParentJob;public int ParentJob{get{return m_ParentJob;}}
	
	float m_HpRecovery;public float HpRecovery{get{return m_HpRecovery;}}
	float m_MpRecovery;public float MpRecovery{get{return m_MpRecovery;}}
	float m_HpRecoveryBattle;public float HpRecoveryBattle{get{return m_HpRecoveryBattle;}}
	float m_MpRecoveryBattle;public float MpRecoveryBattle{get{return m_MpRecoveryBattle;}}
	
	float m_MoveSpeed;public float MoveSpeed{get{return m_MoveSpeed;}}
	float m_ViewDistance;public float ViewDistance{get{return m_ViewDistance;}}	
	float m_AccuracyRatio;public float AccuracyRatio{get{return m_AccuracyRatio;}}
	float m_DodgeRatio;public float DodgeRatio{get{return m_DodgeRatio;}}	
	float m_AttackSpeedRatio;public float AttackSpeedRatio{get{return m_AttackSpeedRatio;}}
	float m_BaseAttackCycle;public float BaseAttackCycle{get{return m_BaseAttackCycle;}}
	float m_CriticalRatio;public float CriticalRatio{get{return m_CriticalRatio;}}
	
#if false
	int m_FireAttack;public int FireAttack{get{return m_FireAttack;}}
	int m_IceAttack;public int IceAttack{get{return m_IceAttack;}}
	int m_LightAttack;public int LightAttack{get{return m_LightAttack;}}
	int m_DarkAttack;public int DarkAttack{get{return m_DarkAttack;}}
	int m_NatureAttack;public int NatureAttack{get{return m_NatureAttack;}}
	
	int m_FireResist;public int FireResist{get{return m_FireResist;}}
	int m_IceResist;public int IceResist{get{return m_IceResist;}}
	int m_LightResist;public int LightResist{get{return m_LightResist;}}
	int m_DarkResist;public int DarkResist{get{return m_DarkResist;}}
	int m_NatureResist;public int NatureResist{get{return m_NatureResist;}}
#endif
	
//	Dictionary<eSKILL_INPUT_TYPE , int > m_dicSkillInputType = new Dictionary<eSKILL_INPUT_TYPE, int>();
	
	public Tbl_Class_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			
			SetValue(ref m_Index, node, "Index");
			SetValue<eRACE>(ref m_RaceId, node, "Race");
			SetValue<eCLASS>(ref m_Class, node, "Class");
//			SetValue<eGENDER>(ref m_Gender, node, "Gender");
			
			SetValue(ref m_ModelingPath_Male, node, "ModelingPath_Male");
			SetValue(ref m_ModelingPath_Female, node, "ModelingPath_Female");
			
			SetValue(ref m_Portrait_Male, node, "ChatImage_Male");
			SetValue(ref m_Portrait_Female, node, "ChatImage_Female");
			
			SetValue(ref m_JobIcon, node, "JobIcon");
			SetValue<eATTACK_TYPE>(ref m_AttackType, node, "AttackType");
			
			
			SetValue(ref m_JobStep, node, "JobStep");
			SetValue(ref m_BaseJob, node, "BaseJob");
//			SetValue(ref m_ParentJob, node, "ParentJob");
			
			SetValue(ref m_HpRecovery, node, "HPRecovery");
			SetValue(ref m_MpRecovery, node, "MPRecovery");
			SetValue(ref m_HpRecoveryBattle, node, "HPRecoveryBattle");
			SetValue(ref m_MpRecoveryBattle, node, "MPRecoveryBattle");
			
			SetValue(ref m_MoveSpeed, node, "MoveSpeed"); if(m_MoveSpeed == 0f || m_MoveSpeed == float.MaxValue) Debug.LogError("Tbl_Class_Record::constructor: m_MoveSpeed = " + m_MoveSpeed);
			SetValue(ref m_ViewDistance, node, "ViewDistance");
			SetValue(ref m_AccuracyRatio, node, "AccuracyRatio");
			SetValue(ref m_DodgeRatio, node, "DodgeRatio");
			SetValue(ref m_AttackSpeedRatio, node, "AttackSpeedRatio");
			SetValue(ref m_BaseAttackCycle, node, "BaseAttackCycle");
			SetValue(ref m_CriticalRatio, node, "CriticalRatio");
			
			
#if false
			SetValue(ref m_FireAttack, node, "FireAttack");
			SetValue(ref m_IceAttack, node, "IceAttack");
			SetValue(ref m_LightAttack, node, "LightAttack");
			SetValue(ref m_DarkAttack, node, "DarkAttack");
			SetValue(ref m_NatureAttack, node, "NatureAttack");
			
			SetValue(ref m_FireResist, node, "FireResist");
			SetValue(ref m_IceResist, node, "IceResist");
			SetValue(ref m_LightResist, node, "LightResist");
			SetValue(ref m_DarkResist, node, "DarkResist");
			SetValue(ref m_NatureResist, node, "NatureResist");
#endif
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
//	public int GetSkillIndexByInputType(eSKILL_INPUT_TYPE _type)
//	{
//		if(m_dicSkillInputType.ContainsKey(_type) == true)
//			return m_dicSkillInputType[_type];
//		else
//		{
//			Debug.LogError("Tbl_Tribe::GetSkillIndexByInputType : Invalid Type Access = " + _type);
//			return int.MaxValue;
//		}
//	}
	
	// bad code
//	public eSKILL_INPUT_TYPE GetInputTypeBySkillIndex(int _skillIdx)
//	{
//		if(m_dicSkillInputType.ContainsValue(_skillIdx) == true)
//		{
//			foreach(KeyValuePair<eSKILL_INPUT_TYPE, int> pair in m_dicSkillInputType)
//			{
//				if(pair.Value == _skillIdx)
//					return pair.Key;
//			}
//		}
//		
//		Debug.LogError("Tbl_Tribe::GetInputTypeBySkillIndex : Invalid index Access = " + _skillIdx);
//		return eSKILL_INPUT_TYPE.NONE;
//	}
}

public class Tbl_Class_Table : AsTableBase {
	SortedList<int, Tbl_Class_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Class_Record>();
	
	public Tbl_Class_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.RACE;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_Class_Record record = new Tbl_Class_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Class_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_Class_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Class_Table]GetRecord: there is no record");
		return null;
	}
	
	public Tbl_Class_Record GetRecordByRaceAndClass(eRACE _race, eCLASS _class)
	{
		foreach(KeyValuePair<int, Tbl_Class_Record> pair in m_ResourceTable)
		{
			if(pair.Value.Race == _race)
			{
				if(pair.Value.Class == _class)
				{
					return pair.Value;
				}
			}
		}
		
		Debug.LogError("Tbl_Class_Table::GetRecordByTribeAndClass: there is no record. race:" + _race + ", class:" + _class);
		return null;
	}
	
	public Tbl_Class_Record GetRecordByRaceAndClass(eCLASS _class)
	{
		foreach(KeyValuePair<int, Tbl_Class_Record> pair in m_ResourceTable)
		{			
			if(pair.Value.Class == _class)
			{
				return pair.Value;
			}			
		}
		
		Debug.LogError("Tbl_Class_Table::GetRecordByTribeAndClass: there is no record. class:" + _class);
		return null;
	}
	
	   /// dopamin begin
    public SortedList<int, Tbl_Class_Record> GetList()
    {
        return m_ResourceTable;
    }
    /// dopamin end 
}
