using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Monster_Record : AsTableRecord
{
	int m_Id;public int Id{get{return m_Id;}}
	string m_Class;public string Class{get{return m_Class;}}
	int m_Monster_Kind_ID; public int Monster_Kind_ID { get { return m_Monster_Kind_ID; } }
	string m_Race;public string Race{get{return m_Race;}}
	int m_ElementalIndex;	public int ElementalIndex	{ get { return m_ElementalIndex; } }
	eMonster_Grade m_Grade;public eMonster_Grade Grade{get{return m_Grade;}}
	eMonster_AttackType m_AttackType;public eMonster_AttackType AttackType{get{return m_AttackType;}}
	eMonster_AttackStyle attackStyle = eMonster_AttackStyle.NONE; public eMonster_AttackStyle AttackStyle	{ get { return attackStyle; } }

    int m_Champion;public int Champion{get{return m_Champion;}}
	int m_SkillGrant;public int SkillGrant{get{return m_SkillGrant;}}
//	int m_SpawnTime;public int SpawnTime{get{return m_SpawnTime;}}
	int m_Level;public int Level{get{return m_Level;}}
	
	int m_HPMax;public int HPMax{get{return m_HPMax;}}
	int m_MPMax;public int MPMax{get{return m_MPMax;}}
	
	int m_PhysicalAttack_Min;public int PhysicalAttack_Min{get{return m_PhysicalAttack_Min;}}
	int m_PhysicalAttack_Max;public int PhysicalAttack_Max{get{return m_PhysicalAttack_Max;}}
	int m_MagicalAttack_Min;public int MagicalAttack_Min{get{return m_MagicalAttack_Min;}}
	int m_MagicalAttack_Max;public int MagicalAttack_Max{get{return m_MagicalAttack_Max;}}

	int m_PhysicalDefense;public int PhysicalDefense{get{return m_PhysicalDefense;}}
	int m_MagicalResist;public int MagicalResist{get{return m_MagicalResist;}}
	
	int m_HPRecovery;public int HPRecovery{get{return m_HPRecovery;}}
	int m_MPRecovery;public int MPRecovery{get{return m_MPRecovery;}}
	
	int m_AccuracyRatio;public int AccuracyRatio{get{return m_AccuracyRatio;}}
	int m_CriticalRatio;public int CriticalRatio{get{return m_CriticalRatio;}}
	int m_DodgeRatio;public int DodgeRatio{get{return m_DodgeRatio;}}
	
#if false
	int m_FireResist;public int FireResist{get{return m_FireResist;}}
	int m_IceResist;public int IceResist{get{return m_IceResist;}}
	int m_LightResist;public int LightResist{get{return m_LightResist;}}
	int m_DarkResist;public int DarkResist{get{return m_DarkResist;}}
	int m_NatureResist;public int NatureResist{get{return m_NatureResist;}}
#endif
	
	int m_WalkSpeed;public int WalkSpeed{get{return m_WalkSpeed;}}
	int m_RunSpeed;public int RunSpeed{get{return m_RunSpeed;}}
	int m_ViewDistance;public int ViewDistance{get{return m_ViewDistance;}}
	int m_ChaseDistance;public int ChaseDistance{get{return m_ChaseDistance;}}
	
	float m_StopTime_Min;public float StopTime_Min{get{return m_StopTime_Min;}}
	float m_StopTime_Max;public float StopTime_Max{get{return m_StopTime_Max;}}
	
	int m_AttackSpeed;public int AttackSpeed{get{return m_AttackSpeed;}}
	
	eMonster_HelpType m_HelpType;public eMonster_HelpType Helptype{get{return m_HelpType;}}
	eMonster_HelpCondition m_HelpCondition;public eMonster_HelpCondition HelpCondition{get{return m_HelpCondition;}}
	int m_HelpValue;public int HelpValue{get{return m_HelpValue;}}
	int m_HelpProb;public int HelpProb{get{return m_HelpProb;}}
	
	eMonster_RunAwayCondition m_RunAwayCondition;public eMonster_RunAwayCondition RunAwayCondition{get{return m_RunAwayCondition;}}
	int m_RunAwayValue;public int RunAwayValue{get{return m_RunAwayValue;}}
	int m_RunAwayProb;public int RunAwayProb{get{return m_RunAwayProb;}}
	eMonster_RunAwayDirection m_RunAwayDirection;public eMonster_RunAwayDirection RunAwayDirection{get{return m_RunAwayDirection;}}
	int m_RunAwayDistance;public int RunAwayDistance{get{return m_RunAwayDistance;}}
	
	int m_AggroDecreaseTime;public int AggroDecreaseTime{get{return m_AggroDecreaseTime;}}
//	int m_AggroResetTime;public int AggroResetTime{get{return m_AggroResetTime;}}
	int m_AggroDecreaseProb;public int AggroDecreaseProb{get{return m_AggroDecreaseProb;}}
	int m_DropExp;public int DropExp{get{return m_DropExp;}}
	int m_DropItem;public int DropItem{get{return m_DropItem;}}
	
	bool m_ViewHold = false;public bool ViewHold{get{return m_ViewHold;}}
	
	public Tbl_Monster_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			#region - prev version -
#if flase
			m_Id = int.Parse(node["ID"].InnerText);
			SetValue(ref m_Id, node, "ID");
			SetValue<eMONSTER_ATTACK_TYPE>(ref m_AttackType, node, "AttackType");
		
			SetValue(ref m_Level, node, "Level");
			SetValue(ref m_Hp, node, "HP");
			SetValue(ref m_Mp, node, "MP");
			
			SetValue(ref m_Defence, node, "Defence");
			SetValue(ref m_MoveSpeed, node, "WalkSpeed");
			SetValue(ref m_MoveSpeed, node, "RunSpeed");
//			SetValue(ref m_ViewAngle, node, "ViewAngle");
			SetValue(ref m_ViewDistance, node, "ViewDistance");
			SetValue(ref m_ChaseDistance, node, "ChaseDistance");
			SetValue(ref m_Attack, node, "Attack");
			SetValue(ref m_AttackCoolTime, node, "AttackCoolTime");
					
			SetValue(ref m_SkillIndex_1, node, "SkillIndex_1");
			SetValue<eSKILL_CONDITION>(ref m_SkillCondition_1, node, "SkillCondition_1");
			SetValue(ref m_ProbValue_1, node, "ProbValue_1");
			SetValue<eSKILL_TARGET>(ref m_SkillTarget_1, node, "SkillTarget_1");
			SetValue(ref m_SkillProb_1, node, "SkillProb_1");
			SetValue(ref m_SkillDistanceMin_1, node, "SkillDistanceMin_1");
			SetValue(ref m_SkillDistanceMax_1, node, "SkillDistanceMax_1");
			
			SetValue(ref m_SkillIndex_2, node, "SkillIndex_2");
			SetValue<eSKILL_CONDITION>(ref m_SkillCondition_2, node, "SkillCondition_2");
			SetValue(ref m_ProbValue_2, node, "ProbValue_2");
			SetValue<eSKILL_TARGET>(ref m_SkillTarget_2, node, "SkillTarget_2");
			SetValue(ref m_SkillProb_2, node, "SkillProb_2");
			SetValue(ref m_SkillDistanceMin_2, node, "SkillDistanceMin_2");
			SetValue(ref m_SkillDistanceMax_2, node, "SkillDistanceMax_2");
			
			SetValue(ref m_SkillIndex_3, node, "SkillIndex_3");
			SetValue<eSKILL_CONDITION>(ref m_SkillCondition_3, node, "SkillCondition_3");
			SetValue(ref m_ProbValue_3, node, "ProbValue_3");
			SetValue<eSKILL_TARGET>(ref m_SkillTarget_3, node, "SkillTarget_3");
			SetValue(ref m_SkillProb_3, node, "SkillProb_3");
			SetValue(ref m_SkillDistanceMin_3, node, "SkillDistanceMin_3");
			SetValue(ref m_SkillDistanceMax_3, node, "SkillDistanceMax_3");
			
			SetValue(ref m_DropExp, node, "DropExp");
			SetValue(ref m_DropItem, node, "DropItem");
#endif
			#endregion
			
			SetValue(ref m_Id, node, "ID");
			SetValue(ref m_Class, node, "Class");
            SetValue(ref m_Monster_Kind_ID, node, "Kind");
			SetValue(ref m_Race, node, "Race");
			SetValue( ref m_ElementalIndex, node, "ElementalIndex");
			SetValue<eMonster_Grade>(ref m_Grade, node, "Grade");
			SetValue<eMonster_AttackType>(ref m_AttackType, node, "AttackType");
			SetValue<eMonster_AttackStyle>( ref attackStyle, node, "AttackStyle");
			SetValue(ref m_Champion, node, "Champion");
			SetValue(ref m_SkillGrant, node, "SkillGrant");
//			SetValue(ref m_SpawnTime, node, "SpawnTime");
			SetValue(ref m_Level, node, "Level");
			
			SetValue(ref m_HPMax, node, "HPMax");
			SetValue(ref m_MPMax, node, "MPMax");
			
			SetValue(ref m_PhysicalAttack_Min, node, "PhysicalAttack_Min");
			SetValue(ref m_PhysicalAttack_Max, node, "PhysicalAttack_Max");
			SetValue(ref m_MagicalAttack_Min, node, "MagicalAttack_Min");
			SetValue(ref m_MagicalAttack_Max, node, "MagicalAttack_Max");
			
			SetValue(ref m_PhysicalDefense, node, "PhysicalDefense");
			SetValue(ref m_MagicalResist, node, "MagicalResist");
			
			SetValue(ref m_HPRecovery, node, "HPRecovery");
			SetValue(ref m_MPRecovery, node, "MPRecovery");
			
			SetValue(ref m_AccuracyRatio, node, "AccuracyRatio");
			SetValue(ref m_CriticalRatio, node, "CriticalRatio");
			SetValue(ref m_DodgeRatio, node, "DodgeRatio");
			
#if false
			SetValue(ref m_FireResist, node, "FireResist");
			SetValue(ref m_IceResist, node, "IceResist");
			SetValue(ref m_LightResist, node, "LightResist");
			SetValue(ref m_DarkResist, node, "DarkResist");
			SetValue(ref m_NatureResist, node, "NatureResist");
#endif
			
			SetValue(ref m_WalkSpeed, node, "WalkSpeed");  if( /*m_WalkSpeed == 0f ||*/ m_WalkSpeed == float.MaxValue) Debug.LogError("Tbl_Monster_Record::constructor: m_WalkSpeed = " + m_WalkSpeed);
			SetValue(ref m_RunSpeed, node, "RunSpeed");  if( /*m_RunSpeed == 0f ||*/ m_RunSpeed == float.MaxValue) Debug.LogError("Tbl_Monster_Record::constructor: m_RunSpeed = " + m_RunSpeed);
			SetValue(ref m_ViewDistance, node, "ViewDistance");
			SetValue(ref m_ChaseDistance, node, "ChaseDistance");
			
			SetValue(ref m_StopTime_Min, node, "StopTime_Min");
			SetValue(ref m_StopTime_Max, node, "StopTime_Max");
			
			SetValue(ref m_AttackSpeed, node, "AttackSpeed");
			
			SetValue<eMonster_HelpType>(ref m_HelpType, node, "HelpType");
			SetValue<eMonster_HelpCondition>(ref m_HelpCondition, node, "HelpCondition");
			SetValue(ref m_HelpValue, node, "HelpValue");
			SetValue(ref m_HelpProb, node, "HelpProb");
			
			SetValue(ref m_RunAwayCondition, node, "RunAwayCondition");
			SetValue(ref m_RunAwayValue, node, "RunAwayValue");
			SetValue(ref m_RunAwayProb, node, "RunAwayProb");
			SetValue(ref m_RunAwayDirection, node, "RunAwayDirection");
			SetValue(ref m_RunAwayDistance, node, "RunAwayDistance");
			
			SetValue(ref m_AggroDecreaseTime, node, "AggroDecreaseTime");
//			SetValue(ref m_AggroResetTime, node, "AggroResetTime");
			SetValue(ref m_AggroDecreaseProb, node, "AggroDecreaseProb");
			SetValue(ref m_DropExp, node, "DropExp");
			SetValue(ref m_DropItem, node, "DropItem");
			
			SetValue(ref m_ViewHold, node, "ViewHold");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
			Debug.Log(_element.Name);
			//Debug.LogError("[Tbl_Monster_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
	
	public Tbl_Monster_Record(BinaryReader br)
	{
		m_Id = br.ReadInt32();
		m_Class = br.ReadString();
		m_Monster_Kind_ID = br.ReadInt32();
		m_Race = br.ReadString();
		m_ElementalIndex = br.ReadInt32();
		m_Grade = (eMonster_Grade)br.ReadInt32();
		m_AttackType = (eMonster_AttackType)br.ReadInt32();
		attackStyle = (eMonster_AttackStyle)br.ReadInt32();
		m_Champion = br.ReadInt32();
		m_SkillGrant = br.ReadInt32();
		m_Level = br.ReadInt32();
		//m_HPMin = br.ReadInt32();
		m_HPMax = br.ReadInt32();
		m_MPMax = br.ReadInt32();
		m_PhysicalAttack_Min = br.ReadInt32();
		m_PhysicalAttack_Max = br.ReadInt32();
		m_MagicalAttack_Min = br.ReadInt32();
		m_MagicalAttack_Max = br.ReadInt32();
		m_PhysicalDefense = br.ReadInt32();
		m_MagicalResist = br.ReadInt32();
		m_HPRecovery = br.ReadInt32();
		m_MPRecovery = br.ReadInt32();
		m_AccuracyRatio = br.ReadInt32();
		m_CriticalRatio = br.ReadInt32();
		m_DodgeRatio = br.ReadInt32();
		m_WalkSpeed = br.ReadInt32();
		m_RunSpeed = br.ReadInt32();
		m_ViewDistance = br.ReadInt32();
		m_ChaseDistance = br.ReadInt32();
		//m_MoveDistance = br.ReadInt32();
		m_StopTime_Min = br.ReadInt32();
		m_StopTime_Max = br.ReadInt32();
		m_AttackSpeed = br.ReadInt32();
		m_HelpType = (eMonster_HelpType)br.ReadInt32();
		m_HelpCondition = (eMonster_HelpCondition)br.ReadInt32();
		m_HelpValue = br.ReadInt32();
		m_HelpProb = br.ReadInt32();
		m_RunAwayCondition = (eMonster_RunAwayCondition)br.ReadInt32();
		m_RunAwayValue = br.ReadInt32();
		m_RunAwayProb = br.ReadInt32();
		m_RunAwayDirection = (eMonster_RunAwayDirection)br.ReadInt32();
		m_RunAwayDistance = br.ReadInt32();
		m_AggroDecreaseTime = br.ReadInt32();
		m_AggroDecreaseProb = br.ReadInt32();
		m_DropExp = br.ReadInt32();
		m_DropItem = br.ReadInt32();
		//m_DropItem_Champ = br.ReadInt32();
		//m_DropGold_Min = br.ReadInt32();
		//m_DropGold_Max = br.ReadInt32();
		//m_ChampDropGold_Min = br.ReadInt32();
		//m_ChampDropGold_Max = br.ReadInt32();
		
		m_ViewHold = br.ReadBoolean();
	}
}

public class Tbl_Monster_Table : AsTableBase {
	SortedList<int, Tbl_Monster_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Monster_Record>();
	
	public Tbl_Monster_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.MONSTER;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try
		{
			if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || 
				( null != AsTableManager.Instance && true == AsTableManager.Instance.useReadBinary ) )
			{	
				// Ready Binary
				TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
				MemoryStream stream = new MemoryStream( textAsset.bytes);
				BinaryReader br = new BinaryReader( stream);
				
				int nCount = br.ReadInt32();
				
				for( int i = 0; i < nCount; i++)
				{
					Tbl_Monster_Record record = new Tbl_Monster_Record( br);
					m_ResourceTable.Add(record.Id, record);
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
					Tbl_Monster_Record record = new Tbl_Monster_Record((XmlElement)node);
					m_ResourceTable.Add(record.Id, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Monster_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_Monster_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Monster_Table]GetRecord: there is no id[" + _id + "] record");
		return null;
	}
	
	public int GetNpcIdByMonsterKindId(int nKindId)
	{
		foreach(KeyValuePair<int, Tbl_Monster_Record> pair in m_ResourceTable)
		{
			if( pair.Value.Monster_Kind_ID == nKindId)
			{
				return pair.Value.Id;
			}
		}
		
		Debug.LogError("Tbl_Monster_Table::GetNpcIdByMonsterKindId: no record");
		return 0;
	}

#if false
	public Tbl_Monster_Record GetRecordByTribeAndClass(int _tribe, int _class)
	{
		foreach(KeyValuePair<int, Tbl_Monster_Record> pair in m_ResourceTable)
		{
			if(pair.Value.Race == _tribe)
			{
				if(pair.Value.Class == _class)
				{
					return pair.Value;
				}
			}
		}
		
		Debug.LogError("Tbl_Monster_Table::GetRecordByTribeAndClass: no record");
		return null;
	}
#endif
}
