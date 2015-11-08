using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_SkillLevel_Potency
{
	float m_Potency_Prob;	public float Potency_Prob	{ get{ return m_Potency_Prob; } }
	float m_Potency_Value;	public float Potency_Value	{ get{ return m_Potency_Value; } }
	float m_Potency_IntValue;	public float Potency_IntValue	{ get { return m_Potency_IntValue; } }
	float m_Potency_Duration;	public float Potency_Duration	{ get { return m_Potency_Duration; } }
	int m_Potency_EffectIndex;	public int Potency_EffectIndex	{ get { return m_Potency_EffectIndex; } }
	
	public Tbl_SkillLevel_Potency(float _prob, float _value, float _int_value, float _duration, int _effectIndex)
	{
		m_Potency_Prob = _prob;
		m_Potency_Value = _value;
		m_Potency_IntValue = _int_value;
		m_Potency_Duration = _duration;
		m_Potency_EffectIndex = _effectIndex;
	}
	
	public bool CheckValuePositive()
	{
//		return m_Potency_Value > 0 || m_Potency_IntValue > 0;		
				
		if(m_Potency_Value != float.MaxValue && m_Potency_Value > 0)
			return true;
		else if(m_Potency_IntValue != float.MaxValue && m_Potency_IntValue > 0)
			return true;
		
		return false;
	}
}

public class Tbl_SkillLevel_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	int m_Skill_GroupIndex;public int Skill_GroupIndex{get{return m_Skill_GroupIndex;}}
	int m_Skill_Level;public int Skill_Level{get{return m_Skill_Level;}}
	int m_Level_Limit;public int Level_Limit{get{return m_Level_Limit;}}
	int m_Mp_Decrease;public int Mp_Decrease{get{return m_Mp_Decrease;}}
	float m_Usable_Distance;public float Usable_Distance{get{return m_Usable_Distance;}}
	int m_CoolTime;public int CoolTime{get{return m_CoolTime;}}
	
	int m_ChargeDuration;public int ChargeDuration{get{return m_ChargeDuration;}}
	int m_ChargeMaxStep;public int ChargeMaxStep{get{return m_ChargeMaxStep;}}
	int m_ChargeStep;public int ChargeStep{get{return m_ChargeStep;}}
	
	int m_SkillAction_Index;public int SkillAction_Index{get{return m_SkillAction_Index;}}
	int m_SkillActionCCW_Index;public int SkillActionCCW_Index{get{return m_SkillActionCCW_Index;}}
	int m_SkillAction_Index_Female;public int SkillAction_Index_Female{get{return m_SkillAction_Index_Female;}}
	int m_SkillActionCCW_Index_Female;public int SkillActionCCW_Index_Female{get{return m_SkillActionCCW_Index_Female;}}
	eAggro_ValueType m_Aggro_ValueType;public eAggro_ValueType Aggro_ValueType{get{ return m_Aggro_ValueType;}}
	int m_Aggro_Value;public int Aggro_Value{get{return m_Aggro_Value;}}
	
	List<Tbl_SkillLevel_Potency> m_listSkillLevelPotency = new List<Tbl_SkillLevel_Potency>();public List<Tbl_SkillLevel_Potency> listSkillLevelPotency{get{return m_listSkillLevelPotency;}}
	
	public Tbl_SkillLevel_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_Skill_GroupIndex, node, "Skill_GroupIndex");
			SetValue(ref m_Skill_Level, node, "Skill_Level");
			SetValue(ref m_Level_Limit, node, "Level_Limit");
			SetValue(ref m_Mp_Decrease, node, "Mp_Decrease");
			SetValue(ref m_Usable_Distance, node, "Usable_Distance");
			SetValue(ref m_CoolTime, node, "CoolTime");
			
			SetValue(ref m_ChargeDuration, node, "ChargeDuration");
			SetValue(ref m_ChargeMaxStep, node, "ChargeMaxStep");
			SetValue(ref m_ChargeStep, node, "ChargeStep");
			
			SetValue(ref m_SkillAction_Index, node, "SkillAction_Index");
			SetValue(ref m_SkillActionCCW_Index, node, "SkillActionCCW_Index");
			SetValue(ref m_SkillAction_Index_Female, node, "SkillAction_Index_Female");
			SetValue(ref m_SkillActionCCW_Index_Female, node, "SkillActionCCW_Index_Female");
			
			SetValue<eAggro_ValueType>( ref m_Aggro_ValueType, node, "Aggro_ValueType");
			SetValue( ref m_Aggro_Value, node, "Aggro_Value");
			
			for(int i=1; i<=AsTableManager.sSkillLevelPotencyCount; ++i)
			{
//				if(node["Potency" + i + "_Value"].InnerText == "NONE" &&
//					node["Potency" + i + "_Duration"].InnerText == "NONE")
//					continue;//
				
				float prob = 0;
				float __value = 0;
				float __int_value = 0;
				float duration = 0;
				int effectIndex = 0;
				
				SetValue(ref prob, node, "Potency" + i + "_Prob");
				SetValue(ref __value, node, "Potency" + i + "_Value");
				SetValue( ref __int_value, node, "Potency" + i + "_IntValue");
				SetValue(ref duration, node, "Potency" + i + "_Duration");
				SetValue(ref effectIndex, node, "Potency" + i + "_EffectIndex");
				
//				Tbl_SkillLevel_Potency potency = new Tbl_SkillLevel_Potency(__value * 0.01f, duration * 0.001f, effectIndex);
				Tbl_SkillLevel_Potency potency = new Tbl_SkillLevel_Potency( prob * 0.1f, __value, __int_value, duration * 0.001f, effectIndex);
				m_listSkillLevelPotency.Add(potency);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SkillLevel_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
	
	public Tbl_SkillLevel_Record(BinaryReader br)// : base(_element)
	{
		m_Index = br.ReadInt32();
		m_Skill_GroupIndex = br.ReadInt32();
		m_Skill_Level = br.ReadInt32();
		m_Level_Limit = br.ReadInt32();
		m_Mp_Decrease = br.ReadInt32();
		m_Usable_Distance = br.ReadSingle();
		m_CoolTime = br.ReadInt32();
		
		m_ChargeDuration = br.ReadInt32();
		m_ChargeMaxStep = br.ReadInt32();
		m_ChargeStep = br.ReadInt32();
		
		m_SkillAction_Index = br.ReadInt32();
		m_SkillActionCCW_Index = br.ReadInt32();
		m_SkillAction_Index_Female = br.ReadInt32();
		m_SkillActionCCW_Index_Female = br.ReadInt32();
		
		m_Aggro_ValueType = (eAggro_ValueType)br.ReadInt32();
		m_Aggro_Value = br.ReadInt32();
		
		for(int i=1; i<=AsTableManager.sSkillLevelPotencyCount; ++i)
		{
			float prob = br.ReadSingle();
			float __value = br.ReadSingle();
			float __int_value = br.ReadSingle();
			float duration = br.ReadSingle();
			int effectIndex = br.ReadInt32();
			
			Tbl_SkillLevel_Potency potency = new Tbl_SkillLevel_Potency( prob, __value, __int_value, duration, effectIndex);
			m_listSkillLevelPotency.Add(potency);
		}
	}
}

public class Tbl_SkillLevel_Table : AsTableBase {
	SortedList<int, Tbl_SkillLevel_Record> m_ResourceTable = 
		new SortedList<int, Tbl_SkillLevel_Record>();
	
	//<level, original skill index, record>
	Dictionary<int, Dictionary<int, Tbl_SkillLevel_Record>> m_ddicTable = 
		new Dictionary<int, Dictionary<int, Tbl_SkillLevel_Record>>();
	
	//<level, original skill index, charge step, record>
	Dictionary<int, Dictionary<int, Dictionary<int, Tbl_SkillLevel_Record>>> m_dddicCharge =
		new Dictionary<int, Dictionary<int, Dictionary<int, Tbl_SkillLevel_Record>>>();
	
	public Tbl_SkillLevel_Table(string _path)
	{
		m_TableType = eTableType.NPC;
			
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		int idx = 0;
		int idx2 = 0;
		
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
					Tbl_SkillLevel_Record record = new Tbl_SkillLevel_Record( br);
					m_ResourceTable.Add(record.Index, record);
					
					idx2++;
				}
				
				br.Close();
				stream.Close();
			}
			else
			{
				XmlElement root = GetXmlRootElement(_path);
				XmlNodeList nodes = root.ChildNodes;
				
				//debug
				idx2++;
				
				foreach(XmlNode node in nodes)
				{
					Tbl_SkillLevel_Record record = new Tbl_SkillLevel_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
					
					idx2++;
				}
			}
			
			//debug
			idx2++;
			
			List<Tbl_SkillLevel_Record> chargeableSkillLv = new List<Tbl_SkillLevel_Record>();
			foreach(KeyValuePair<int, Tbl_SkillLevel_Record> pair in m_ResourceTable)
			{
				if(pair.Value.ChargeStep != int.MaxValue)// && pair.Value.ChargeStep != 0)
				{
					chargeableSkillLv.Add(pair.Value);
				}
				else
				{
					if(m_ddicTable.ContainsKey(pair.Value.Skill_Level) == false)
						m_ddicTable.Add(pair.Value.Skill_Level, new Dictionary<int, Tbl_SkillLevel_Record>());
					
//					if(m_ddicTable[pair.Value.Skill_Level].ContainsKey(pair.Value.Skill_GroupIndex) == false)
					m_ddicTable[pair.Value.Skill_Level].Add(pair.Value.Skill_GroupIndex, pair.Value);
				}
			}
			
			//debug
			idx2++;
			
			foreach(Tbl_SkillLevel_Record record in chargeableSkillLv)
			{
				if(m_dddicCharge.ContainsKey(record.Skill_Level) == false)
					m_dddicCharge.Add(record.Skill_Level, new Dictionary<int, Dictionary<int, Tbl_SkillLevel_Record>>());
				
				if(m_dddicCharge[record.Skill_Level].ContainsKey(record.Skill_GroupIndex) == false)
					m_dddicCharge[record.Skill_Level].Add(record.Skill_GroupIndex, new Dictionary<int, Tbl_SkillLevel_Record>());
				
				m_dddicCharge[record.Skill_Level][record.Skill_GroupIndex].Add(record.ChargeStep, record);
			}
			
			idx2 = 0;
			idx++;
		}
		catch(System.Exception e)
		{
			Debug.LogError("Tbl_SkillLevel::LoadTable: exception occurred at " + idx + "th data.");
			Debug.LogError("Tbl_SkillLevel::LoadTable: exception occurred at " + idx2 + "th part.");
			Debug.LogError(e);
		}
	}
	
	public Tbl_SkillLevel_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_SkillLevel_Table]GetRecord: there is no record");
		return null;
	}
	
	public Tbl_SkillLevel_Record GetRecord(int _skillLv, int _skillIdx)
	{
		if(m_ddicTable.ContainsKey(_skillLv) == true)
		{
			if(m_ddicTable[_skillLv].ContainsKey(_skillIdx) == true)
			{
				return m_ddicTable[_skillLv][_skillIdx];
			}
		}
		
		Debug.LogError("Tbl_SkillLevel::GetRecord: invalid request = skill lv:" +
			_skillLv + ", skill index:" + _skillIdx);
		return null;
	}
	
	public Tbl_SkillLevel_Record GetRecord(int _skillLv, int _skillIdx, int _chargeLv)
	{
//		if(_chargeLv == 0)
//		{
//			if(m_ddicTable.ContainsKey(_skillLv) == true)
//			{
//				if(m_ddicTable[_skillLv].ContainsKey(_skillIdx) == true)
//				{
//					return m_ddicTable[_skillLv][_skillIdx];
//				}
//			}
//		}
		
//		if(m_ddicTable.ContainsKey(_skillLv) == true)
//		{
//			if(m_ddicTable[_skillLv].ContainsKey(_skillIdx) == true)
//			{
//				if(//m_ddicTable[_skillLv][_skillIdx].ChargeDuration != float.MaxValue &&
//					m_ddicTable[_skillLv][_skillIdx].ChargeStep == int.MaxValue)
//					return m_ddicTable[_skillLv][_skillIdx];
//			}
//		}
		
		if(m_dddicCharge.ContainsKey(_skillLv) == true)
		{
			if(m_dddicCharge[_skillLv].ContainsKey(_skillIdx) == true)
			{
				if(m_dddicCharge[_skillLv][_skillIdx].ContainsKey(_chargeLv) == true)
				{
					return m_dddicCharge[_skillLv][_skillIdx][_chargeLv];
				}
			}
		}
		
		if(m_ddicTable.ContainsKey(_skillLv) == true)
		{
			if(m_ddicTable[_skillLv].ContainsKey(_skillIdx) == true)
			{
				if(//m_ddicTable[_skillLv][_skillIdx].ChargeDuration != float.MaxValue &&
					m_ddicTable[_skillLv][_skillIdx].ChargeStep == 0 ||
					m_ddicTable[_skillLv][_skillIdx].ChargeStep == int.MaxValue)
					return m_ddicTable[_skillLv][_skillIdx];
			}
		}
		
		Debug.LogError("Tbl_SkillLevel::GetRecord: invalid request = skill lv:" +
			_skillLv + ", skill index:" + _skillIdx + ", charge lv:" + _chargeLv);
		return null;
	}
	
	public SortedList<int, Tbl_SkillLevel_Record> GetList()
	{
		return m_ResourceTable;
	}
}
