using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

//public class Tbl_MonsterSkillLevel_Potency
//{
//	float m_Potency_Value;public float Potency_Value{get{return m_Potency_Value;}}
//	float m_Potency_Duration;public float Potency_Duration{get{return m_Potency_Duration;}}
//	int m_Potency_EffectIndex;public int Potency_EffectIndex{get{return m_Potency_EffectIndex;}}
//	
//	public Tbl_SkillLevel_Potency(float _value, float _duration, int _effectIndex)
//	{
//		m_Potency_Value = _value;
//		m_Potency_Duration = _duration;
//		m_Potency_EffectIndex = _effectIndex;
//	}
//}

public class Tbl_MonsterSkillLevel_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	string m_Des_SkillName;public string Des_SkillName{get{return m_Des_SkillName;}}
	int m_Skill_GroupIndex;public int Skill_GroupIndex{get{return m_Skill_GroupIndex;}}
	int m_Skill_Level;public int Skill_Level{get{return m_Skill_Level;}}
	int m_Mp_Decrease;public int Mp_Decrease{get{return m_Mp_Decrease;}}
	float m_CoolTime;public float CoolTime{get{return m_CoolTime;}}
	int m_SkillAction_Index;public int SkillAction_Index{get{return m_SkillAction_Index;}}
	
//	int m_Potency1_Index;public int Potency1_Index{get{return m_Potency1_Index;}}
//	int m_Potency1_Value;public int Potency1_Value{get{return m_Potency1_Value;}}
//	float m_Potency1_Duration;public float Potency1_Duration{get{return m_Potency1_Duration;}}
//	int m_Potency1_EffectIndex;public int Potency1_EffectIndex{get{return m_Potency1_EffectIndex;}}
//	
//	int m_Potency2_Index;public int Potency2_Index{get{return m_Potency2_Index;}}
//	int m_Potency2_Value;public int Potency2_Value{get{return m_Potency2_Value;}}
//	float m_Potency2_Duration;public float Potency2_Duration{get{return m_Potency2_Duration;}}
//	int m_Potency2_EffectIndex;public int Potency2_EffectIndex{get{return m_Potency2_EffectIndex;}}
	
	List<Tbl_SkillLevel_Potency> m_listSkillLevelPotency = new List<Tbl_SkillLevel_Potency>();public List<Tbl_SkillLevel_Potency> listSkillLevelPotency{get{return m_listSkillLevelPotency;}}
	
	public Tbl_MonsterSkillLevel_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_Des_SkillName, node, "Des_SkillName");
			SetValue(ref m_Skill_GroupIndex, node, "Skill_GroupIndex");
			SetValue(ref m_Skill_Level, node, "Skill_Level");
			SetValue(ref m_Mp_Decrease, node, "Mp_Decrease");
			SetValue(ref m_CoolTime, node, "CoolTime");
			SetValue(ref m_SkillAction_Index, node, "SkillAction_Index");
			
			for(int i=1; i<=AsTableManager.sMonsterSkillLevelPotencyCount; ++i)
			{
//				if(node["Potency" + i + "_Value"].InnerText == "NONE" &&
//					node["Potency" + i + "_Duration"].InnerText == "NONE")
//					continue;
				
				float _prob = 0;
				float __value = 0;
				float __int_value = 0;
				float duration = 0;
				int effectIndex = 0;
				
				SetValue( ref _prob, node, "Potency" + i + "_Prob");
				SetValue( ref __value, node, "Potency" + i + "_Value");
				SetValue( ref __int_value, node, "Potency" + i + "_IntValue");
				SetValue( ref duration, node, "Potency" + i + "_Duration");
				SetValue( ref effectIndex, node, "Potency" + i + "_EffectIndex");
				
				Tbl_SkillLevel_Potency potency = new Tbl_SkillLevel_Potency( _prob * 0.1f, __value, __int_value, duration, effectIndex);
				m_listSkillLevelPotency.Add(potency);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_MonsterSkillLevel_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
	
	
	public Tbl_MonsterSkillLevel_Record(BinaryReader br)// : base(_element)
	{
		m_Index = br.ReadInt32();
		m_Des_SkillName = br.ReadString();
		m_Skill_GroupIndex = br.ReadInt32();
		m_Skill_Level = br.ReadInt32();
		m_Mp_Decrease = br.ReadInt32();
		m_CoolTime = br.ReadSingle();
		m_SkillAction_Index = br.ReadInt32();		
		
		
		for(int i=1; i<=AsTableManager.sMonsterSkillLevelPotencyCount; ++i)
		{
			float _prob = br.ReadSingle();
			float __value = br.ReadSingle();	
			float __int_value = br.ReadSingle();
			float duration = br.ReadSingle();
			int effectIndex = br.ReadInt32();			
			
			Tbl_SkillLevel_Potency potency = new Tbl_SkillLevel_Potency( _prob * 0.1f, __value, __int_value, duration, effectIndex);
			m_listSkillLevelPotency.Add(potency);
		}
	}
}

public class Tbl_MonsterSkillLevel_Table : AsTableBase {
	SortedList<int, Tbl_MonsterSkillLevel_Record> m_ResourceTable = 
		new SortedList<int, Tbl_MonsterSkillLevel_Record>();
	
	//<level, original skill index, record>
	Dictionary<int, Dictionary<int, Tbl_MonsterSkillLevel_Record>> m_ddicTable = 
		new Dictionary<int, Dictionary<int, Tbl_MonsterSkillLevel_Record>>();

	public Tbl_MonsterSkillLevel_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || true == AsTableManager.Instance.useReadBinary )
		{
			TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
			MemoryStream stream = new MemoryStream( textAsset.bytes);
			BinaryReader br = new BinaryReader( stream);
			
			int nCount = br.ReadInt32();		
	
			for( int i = 0; i < nCount; i++)
			{
				Tbl_MonsterSkillLevel_Record record = new Tbl_MonsterSkillLevel_Record(br);
				m_ResourceTable.Add(record.Index, record);			
			}
			
			br.Close();
			stream.Close();
			
			foreach(KeyValuePair<int, Tbl_MonsterSkillLevel_Record> pair in m_ResourceTable)
			{
			
				if(m_ddicTable.ContainsKey(pair.Value.Skill_Level) == false)
					m_ddicTable.Add(pair.Value.Skill_Level, new Dictionary<int, Tbl_MonsterSkillLevel_Record>());
				
				if( false == m_ddicTable[pair.Value.Skill_Level].ContainsKey( pair.Value.Skill_GroupIndex))
					m_ddicTable[pair.Value.Skill_Level].Add(pair.Value.Skill_GroupIndex, pair.Value);
//				else
//					Debug.LogWarning("Tbl_MonsterSkillLevel_Table::LoadTable: same skill group index and skill level. [index:" +
//						pair.Value.Skill_GroupIndex + "][level:" + pair.Value.Skill_Level + "]");
			}
		}
		else
		{	

			try
			{
				XmlElement root = GetXmlRootElement(_path);
				XmlNodeList nodes = root.ChildNodes;
				
				foreach(XmlNode node in nodes)
				{
					Tbl_MonsterSkillLevel_Record record = new Tbl_MonsterSkillLevel_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
				}
				
	//			List<Tbl_MonsterSkillLevel_Record> chargeableSkillLv = new List<Tbl_MonsterSkillLevel_Record>();
				foreach(KeyValuePair<int, Tbl_MonsterSkillLevel_Record> pair in m_ResourceTable)
				{
				
					if(m_ddicTable.ContainsKey(pair.Value.Skill_Level) == false)
						m_ddicTable.Add(pair.Value.Skill_Level, new Dictionary<int, Tbl_MonsterSkillLevel_Record>());
					
					if( false == m_ddicTable[pair.Value.Skill_Level].ContainsKey( pair.Value.Skill_GroupIndex))
						m_ddicTable[pair.Value.Skill_Level].Add(pair.Value.Skill_GroupIndex, pair.Value);
				}
			}
			catch(System.Exception e)
			{
				Debug.LogError("[Tbl_MonsterSkillLevel_Table] LoadTable:|" + e + "| error while parsing ");
			}
		}
	}
	
	public Tbl_MonsterSkillLevel_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_MonsterSkillLevel_Table]GetRecord: there is no record. id:" + _id);
		return null;
	}
	
	public Tbl_MonsterSkillLevel_Record GetRecord(int _skillLv, int _skillIdx)
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
	
//	public SortedList<int, Tbl_MonsterSkill_Record> GetList()
//	{
//		return m_ResourceTable;
//	}
}
