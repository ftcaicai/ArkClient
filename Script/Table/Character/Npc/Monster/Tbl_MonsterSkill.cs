using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

//public class Tbl_Skill_Potency
//{
//	ePotency_UsableCheck_Target m_Potency_UsableCheck_Target;public ePotency_UsableCheck_Target Potency_UsableCheck_Target{get{return m_Potency_UsableCheck_Target;}}
//	ePotency_UsableCheck_Condition m_Potency_UsableCheck_Condition;public ePotency_UsableCheck_Condition Potency_UsableCheck_Condition{get{return m_Potency_UsableCheck_Condition;}}
//	float m_Potency_UsableCheck_ConditionValue;public float Potency_UsableCheck_ConditionValue{get{return m_Potency_UsableCheck_ConditionValue;}}
//	ePotency_Type m_Potency_Type;public ePotency_Type Potency_Type{get{return m_Potency_Type;}}
//	ePotency_Target m_Potency_Target;public ePotency_Target Potency_Target{get{return m_Potency_Target;}}
//	ePotency_DurationType m_Potency_DurationType;public ePotency_DurationType Potency_Duration{get{return m_Potency_DurationType;}}
//	ePotency_Attribute m_Potency_Attribute;public ePotency_Attribute Potency_Attribute{get{return m_Potency_Attribute;}}
//	ePotency_Element m_Potency_Element;public ePotency_Element Potency_Element{get{return m_Potency_Element;}}
//	string m_Potency_BuffIcon;public string Potency_BuffIcon{get{return m_Potency_BuffIcon;}}
//	
//	public Tbl_Skill_Potency(ePotency_UsableCheck_Target _checkTarget, ePotency_UsableCheck_Condition _checkCondition, float _checkConditionValue, ePotency_Type _type,
//		 ePotency_Target _target, ePotency_DurationType _duration, ePotency_Attribute _attribute, ePotency_Element _element, string _buff)
//	{
//		m_Potency_Target = _target;
//		m_Potency_DurationType = _duration;
//		m_Potency_Attribute = _attribute;
//		m_Potency_Element = _element;
//		m_Potency_BuffIcon = _buff;
//	}
//}

public class Tbl_MonsterSkill_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	string m_Des_SkillName;public string Des_SkillName{get{return m_Des_SkillName;}}
	int m_SkillName_Index = -1;public int SkillName_Index{get{return m_SkillName_Index;}}
	eSkillNamePrint m_SkillName_Print = eSkillNamePrint.None;public eSkillNamePrint SkillName_Print{get{return m_SkillName_Print;}}
	string m_Class;public string Class{get{return m_Class;}}
	eSKILL_TYPE m_Skill_Type;public eSKILL_TYPE Skill_Type{get{return m_Skill_Type;}}
	
	List<Tbl_Skill_Potency> m_listSkillPotency = new List<Tbl_Skill_Potency>();public List<Tbl_Skill_Potency> listSkillPotency{get{return m_listSkillPotency;}}
	
	public Tbl_MonsterSkill_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_Des_SkillName, node, "Des_SkillName");
			SetValue(ref m_SkillName_Index, node, "SkillName_Index");
			int print = 0; SetValue(ref print, node, "SkillName_Print");
			switch(print)
			{
			case 0: m_SkillName_Print = eSkillNamePrint.None; break;
			case 1: m_SkillName_Print = eSkillNamePrint.Print; break;
			case 2: m_SkillName_Print = eSkillNamePrint.Negative; break;
			}
			
			SetValue(ref m_Class, node, "Class");
			SetValue<eSKILL_TYPE>(ref m_Skill_Type, node, "Skill_Type");
			
			for(int i=1; i<=AsTableManager.sMonsterSkillPotencyCount; ++i)
			{
				if(node["Potency" + i + "_Type"].InnerText == "NONE")
					continue;
				
				ePotency_Enable_Target check_target = ePotency_Enable_Target.NONE;
				ePotency_Enable_Condition check_condition = ePotency_Enable_Condition.NONE;
				float check_conditionValue = 0;
				ePotency_Enable_Target check_target2 = ePotency_Enable_Target.NONE;
				ePotency_Enable_Condition check_condition2 = ePotency_Enable_Condition.NONE;
				float check_conditionValue2 = 0;
				ePotency_Type type = ePotency_Type.NONE;
				ePotency_Target target = ePotency_Target.NONE;
				ePotency_DurationType duration = ePotency_DurationType.NONE;
				ePotency_Attribute attribute = ePotency_Attribute.NONE;
				ePotency_Element element = ePotency_Element.NONE;
				string buff = "";
				int iBuffTooltipIdx = 0;
				
				SetValue<ePotency_Enable_Target>(ref check_target, node, "Potency" + i + "_Enable_Target");
				SetValue<ePotency_Enable_Condition>(ref check_condition, node, "Potency" + i + "_Enable_Condition");
				SetValue(ref check_conditionValue, node, "Potency" + i + "_Enable_ConditionValue");
				SetValue<ePotency_Enable_Target>(ref check_target2, node, "Potency" + i + "_Enable_Target2");
				SetValue<ePotency_Enable_Condition>(ref check_condition2, node, "Potency" + i + "_Enable_Condition2");
				SetValue(ref check_conditionValue2, node, "Potency" + i + "_Enable_ConditionValue2");
				SetValue<ePotency_Type>(ref type, node, "Potency" + i + "_Type");
				SetValue<ePotency_Target>(ref target, node, "Potency" + i + "_Target");
				SetValue<ePotency_DurationType>(ref duration, node, "Potency" + i + "_DurationType");
				SetValue<ePotency_Attribute>(ref attribute, node, "Potency" + i + "_Attribute");
//				SetValue<ePotency_Element>(ref element, node, "Potency" + i + "_Element");
				SetValue(ref buff, node, "Potency" + i + "_BuffIcon");
				SetValue(ref iBuffTooltipIdx, node, "BuffTooltip" + i + "_Index");
				
				Tbl_Skill_Potency potency = new Tbl_Skill_Potency(check_target, check_condition, check_conditionValue,
					check_target2, check_condition2, check_conditionValue2,
					type, target, duration, attribute, element, buff, iBuffTooltipIdx);
				
				m_listSkillPotency.Add(potency);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("Error while [" + m_CurrentColumn + "] parsing" + "\n" + e);
		}
	}
	
	
	public Tbl_MonsterSkill_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_Des_SkillName = br.ReadString();
		m_SkillName_Index = br.ReadInt32();		
		
		int print = 0; 
		print = br.ReadInt32();
		switch(print)
		{
		case 0: m_SkillName_Print = eSkillNamePrint.None; break;
		case 1: m_SkillName_Print = eSkillNamePrint.Print; break;
		case 2: m_SkillName_Print = eSkillNamePrint.Negative; break;
		}		
		
		m_Class = br.ReadString();
		m_Skill_Type = (eSKILL_TYPE)br.ReadInt32();
		
		int icount = br.ReadInt32();
		for(int i=0; i<icount; ++i)
		{
			ePotency_Enable_Target check_target = (ePotency_Enable_Target)br.ReadInt32();
			ePotency_Enable_Condition check_condition = (ePotency_Enable_Condition)br.ReadInt32();
			float check_conditionValue = br.ReadSingle();
			ePotency_Enable_Target check_target2 = (ePotency_Enable_Target)br.ReadInt32();
			ePotency_Enable_Condition check_condition2 = (ePotency_Enable_Condition)br.ReadInt32();
			float check_conditionValue2 = br.ReadSingle();
			ePotency_Type type = (ePotency_Type)br.ReadInt32();
			ePotency_Target target = (ePotency_Target)br.ReadInt32();
			ePotency_DurationType duration = (ePotency_DurationType)br.ReadInt32();
			ePotency_Attribute attribute = (ePotency_Attribute)br.ReadInt32();
			
			string buff = br.ReadString();
			int iBuffTooltipIdx = br.ReadInt32();
			
			Tbl_Skill_Potency potency = new Tbl_Skill_Potency(check_target, check_condition, check_conditionValue,
				check_target2, check_condition2, check_conditionValue2,
				type, target, duration, attribute, ePotency_Element.NONE, buff, iBuffTooltipIdx);
				
			m_listSkillPotency.Add(potency);
		}
	}
}

public class Tbl_MonsterSkill_Table : AsTableBase {
	SortedList<int, Tbl_MonsterSkill_Record> m_ResourceTable = 
		new SortedList<int, Tbl_MonsterSkill_Record>();
	
	public Tbl_MonsterSkill_Table(string _path)
	{
//		m_TableName = "CharacterResource";
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
					Tbl_MonsterSkill_Record record = new Tbl_MonsterSkill_Record( br);
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
					Tbl_MonsterSkill_Record record = new Tbl_MonsterSkill_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
				}
			}
			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_MonsterSkill_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_MonsterSkill_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_MonsterSkill_Table]GetRecord: there is no record");
		return null;
	}
	
	public SortedList<int, Tbl_MonsterSkill_Record> GetList()
	{
		return m_ResourceTable;
	}
	
//	public Tbl_Npc_Record GetRecordByTribeAndClass(int _tribe, int _class)
//	{
//		foreach(KeyValuePair<int, Tbl_Npc_Record> pair in m_ResourceTable)
//		{
//			if(pair.Value.Race == _tribe)
//			{
//				if(pair.Value.Class == _class)
//				{
//					return pair.Value;
//				}
//			}
//		}
//		
//		Debug.LogError("Tbl_Npc_Table::GetRecordByTribeAndClass: no record");
//		return null;
//	}
}
