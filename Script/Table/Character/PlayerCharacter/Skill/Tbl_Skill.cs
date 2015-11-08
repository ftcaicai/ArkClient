using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public enum eSkillNamePrint {None, Print, Negative}
public enum eSkillReset {NONE, Enable, Disable}
public enum eSkillAutoTarget	{ NONE, Enable, Disable }
public enum eDisableInPvP { NONE, Disable }
public enum eDisableInRaid { NONE, Disable }
public enum eDisableInInDun { NONE, Disable}

public class Tbl_Skill_Potency
{
	ePotency_Enable_Target m_Potency_Enable_Target;public ePotency_Enable_Target Potency_Enable_Target{get{return m_Potency_Enable_Target;}}
	ePotency_Enable_Condition m_Potency_Enable_Condition;public ePotency_Enable_Condition Potency_Enable_Condition{get{return m_Potency_Enable_Condition;}}
	float m_Potency_Enable_ConditionValue;public float Potency_Enable_ConditionValue{get{return m_Potency_Enable_ConditionValue;}}
	ePotency_Enable_Target m_Potency_Enable_Target2;public ePotency_Enable_Target Potency_Enable_Target2{get{return m_Potency_Enable_Target2;}}
	ePotency_Enable_Condition m_Potency_Enable_Condition2;public ePotency_Enable_Condition Potency_Enable_Condition2{get{return m_Potency_Enable_Condition2;}}
	float m_Potency_Enable_ConditionValue2;public float Potency_Enable_ConditionValue2{get{return m_Potency_Enable_ConditionValue2;}}
	ePotency_Type m_Potency_Type;public ePotency_Type Potency_Type{get{return m_Potency_Type;}}
	ePotency_Target m_Potency_Target;public ePotency_Target Potency_Target{get{return m_Potency_Target;}}
	ePotency_DurationType m_Potency_DurationType;public ePotency_DurationType Potency_DurationType{get{return m_Potency_DurationType;}}
	ePotency_Attribute m_Potency_Attribute;public ePotency_Attribute Potency_Attribte{get{return m_Potency_Attribute;}}
//	ePotency_Element m_Potency_Element;public ePotency_Element Potency_Element{get{return m_Potency_Element;}}
	string m_Potency_BuffIcon;public string Potency_BuffIcon{get{return m_Potency_BuffIcon;}}
	int m_Potency_BuffTooltip;public int Potency_BuffTooltip{get{return m_Potency_BuffTooltip;}}
	
	public Tbl_Skill_Potency(ePotency_Enable_Target _checkTarget, ePotency_Enable_Condition _checkCondition, float _checkConditionValue,
		ePotency_Enable_Target _checkTarget2, ePotency_Enable_Condition _checkCondition2, float _checkConditionValue2,
		ePotency_Type _type, ePotency_Target _target, ePotency_DurationType _duration,
		ePotency_Attribute _attribute, ePotency_Element _element, string _buff, int _buffToolIdx)
	{
		m_Potency_Enable_Target = _checkTarget;
		m_Potency_Enable_Condition = _checkCondition;
		m_Potency_Enable_ConditionValue = _checkConditionValue;
		m_Potency_Enable_Target2 = _checkTarget2;
		m_Potency_Enable_Condition2 = _checkCondition2;
		m_Potency_Enable_ConditionValue2 = _checkConditionValue2;
		m_Potency_Type = _type;
		m_Potency_Target = _target;
		m_Potency_DurationType = _duration;
		m_Potency_Attribute = _attribute;
//		m_Potency_Element = _element;
		m_Potency_BuffIcon = _buff;
		m_Potency_BuffTooltip = _buffToolIdx;
	}
	
//	public bool CheckPotencyActivate_Stance(Msg_OtherCharAttackNpc3 _attack)
//	{
//		bool retValue = true;
//		
//		if( _attack.eDamageType != eDAMAGETYPE.eDAMAGETYPE_CRITICAL &&
//			(m_Potency_Enable_Condition == ePotency_Enable_Condition.Critical ||
//			m_Potency_Enable_Condition2 == ePotency_Enable_Condition.Critical) )
//			retValue = false;
//		
//		if( 
//			(m_Potency_Enable_Condition == ePotency_Enable_Condition.HaveBuff ||
//			m_Potency_Enable_Condition2 == ePotency_Enable_Condition.HaveBuff) )
//			retValue = false;
//		
//		return retValue;
//	}
}

public class Tbl_Skill_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	int m_SkillName_Index = -1;public int SkillName_Index{get{return m_SkillName_Index;}}
	int m_Description_Index = -1;public int Description_Index{get{return m_Description_Index;}}
	eSkillNamePrint m_SkillName_Print = eSkillNamePrint.None;public eSkillNamePrint SkillName_Print{get{return m_SkillName_Print;}}
	eSkillReset m_SkillReset;public eSkillReset SkillReset{get{return m_SkillReset;}}
	int			m_nSkillResetCost;public int SkillResetCost{get{return m_nSkillResetCost;}}
	eSkillAutoTarget m_SkillAutoTarget; public eSkillAutoTarget SkillAutoTarget	{ get { return m_SkillAutoTarget; } }
	string m_Skill_Icon;public string Skill_Icon{get{return m_Skill_Icon;}}
	
	eCLASS m_Class;public eCLASS Class{get{return m_Class;}}
	eSKILL_TYPE m_Skill_Type;public eSKILL_TYPE Skill_Type{get{return m_Skill_Type;}}
	eSkill_TargetType m_Skill_TargetType;public eSkill_TargetType Skill_TargetType{get{return m_Skill_TargetType;}}
	eSkillIcon_Enable_Target m_SkillIcon_Enable_Target;public eSkillIcon_Enable_Target SkillIcon_Enable_Target{get{return m_SkillIcon_Enable_Target;}}
	eSkillIcon_Enable_Condition m_SkillIcon_Enable_Condition;public eSkillIcon_Enable_Condition SkillIcon_Enable_Condition{get{return m_SkillIcon_Enable_Condition;}}
	int m_SkillIcon_Enable_ConditionValue;public int SkillIcon_Enable_ConditionValue{get{return m_SkillIcon_Enable_ConditionValue;}}
	
	eCommand_Type m_Command_Type;public eCommand_Type Command_Type{get{return m_Command_Type;}}
	eCommandPicking_Type m_CommandPicking_Type;public eCommandPicking_Type CommandPicking_Type{get{return m_CommandPicking_Type;}}	
	eATTACK_DIRECTION m_Attack_Direction;public eATTACK_DIRECTION Attack_Direction{get{return m_Attack_Direction;}}
	int m_CoolTimeGroup; public int getCoolTimeGroup { get { return m_CoolTimeGroup; } }
	
	eDisableInPvP m_DisableInPvP; public eDisableInPvP DisableInPvP{get{return m_DisableInPvP;}}
	eDisableInRaid m_DisableInRaid; public eDisableInRaid DisableInRaid{get{return m_DisableInRaid;}}
    eDisableInRaid m_DisableInField; public eDisableInRaid DisableInField { get { return m_DisableInField; } }
	eDisableInInDun m_DisableInInDun; public eDisableInInDun DisableInInDun{ get{ return m_DisableInInDun;}}
	
	List<Tbl_Skill_Potency> m_listSkillPotency = new List<Tbl_Skill_Potency>();public List<Tbl_Skill_Potency> listSkillPotency{get{return m_listSkillPotency;}}
	
	public Tbl_Skill_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_SkillName_Index, node, "SkillName_Index");
			SetValue(ref m_Description_Index, node, "Description_Index");
			int print = 0; SetValue(ref print, node, "SkillName_Print");
			switch(print)
			{
			case 0: m_SkillName_Print = eSkillNamePrint.None; break;
			case 1: m_SkillName_Print = eSkillNamePrint.Print; break;
			case 2: m_SkillName_Print = eSkillNamePrint.Negative; break;
			}
			SetValue(ref m_SkillReset, node, "SkillReset");
			
			string strSkillResetCost = ""; SetValue(ref strSkillResetCost, node, "SkillResetCost");
			if( strSkillResetCost == "NONE" )	m_nSkillResetCost = -1;
			else								m_nSkillResetCost = int.Parse( strSkillResetCost );
			
			SetValue( ref m_SkillAutoTarget, node, "AutoTarget");
			SetValue(ref m_Skill_Icon, node, "Skill_Icon");
			SetValue<eCLASS>(ref m_Class, node, "Class");
			SetValue<eSKILL_TYPE>(ref m_Skill_Type, node, "Skill_Type");
			SetValue<eSkill_TargetType>(ref m_Skill_TargetType, node, "Skill_TargetType");
			SetValue<eSkillIcon_Enable_Target>(ref m_SkillIcon_Enable_Target, node, "SkillIcon_Enable_Target");
			SetValue<eSkillIcon_Enable_Condition>(ref m_SkillIcon_Enable_Condition, node, "SkillIcon_Enable_Condition");
			SetValue(ref m_SkillIcon_Enable_ConditionValue, node, "SkillIcon_Enable_ConditionValue");
			
			SetValue<eCommand_Type>(ref m_Command_Type, node, "Command_Type");
			SetValue<eCommandPicking_Type>(ref m_CommandPicking_Type, node, "CommandPicking_Type");
			SetValue<eATTACK_DIRECTION>(ref m_Attack_Direction, node, "Attack_Direction");
			SetValue(ref m_CoolTimeGroup, node, "CoolTimeGroup");
			
			SetValue<eDisableInPvP>(ref m_DisableInPvP, node, "DisableInPvP");
			SetValue<eDisableInRaid>(ref m_DisableInRaid, node, "DisableInRaid");
            SetValue<eDisableInRaid>(ref m_DisableInField, node, "DisableInField");
			SetValue<eDisableInInDun>(ref m_DisableInInDun, node, "DisableInInDun");
			
			
			for(int i=1; i<=AsTableManager.sSkillPotencyCount; ++i)
			{
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
			Debug.LogError(e);//
		}
	}
	
	public Tbl_Skill_Record(BinaryReader br)// : base(_element)
	{
		m_Index = br.ReadInt32();
		m_SkillName_Index = br.ReadInt32();
		m_Description_Index = br.ReadInt32();
		m_SkillName_Print = (eSkillNamePrint)br.ReadInt32();
		m_SkillReset = (eSkillReset)br.ReadInt32();
		m_nSkillResetCost = br.ReadInt32();
		m_SkillAutoTarget = (eSkillAutoTarget)br.ReadInt32();
		m_Skill_Icon = br.ReadString();
		
		m_Class = (eCLASS)br.ReadInt32();
		m_Skill_Type = (eSKILL_TYPE)br.ReadInt32();
		m_Skill_TargetType = (eSkill_TargetType)br.ReadInt32();		
		m_SkillIcon_Enable_Target = (eSkillIcon_Enable_Target)br.ReadInt32();
		m_SkillIcon_Enable_Condition = (eSkillIcon_Enable_Condition)br.ReadInt32();
		m_SkillIcon_Enable_ConditionValue = br.ReadInt32();
		
		m_Command_Type = (eCommand_Type)br.ReadInt32();
		m_CommandPicking_Type = (eCommandPicking_Type)br.ReadInt32();
		m_Attack_Direction = (eATTACK_DIRECTION)br.ReadInt32();
		m_CoolTimeGroup = br.ReadInt32();
		m_DisableInPvP = (eDisableInPvP)br.ReadInt32();
		m_DisableInRaid = (eDisableInRaid)br.ReadInt32();
        m_DisableInField = (eDisableInRaid)br.ReadInt32();
		m_DisableInInDun = (eDisableInInDun)br.ReadInt32();
		
		for(int i=1; i<=AsTableManager.sSkillLevelPotencyCount; ++i)
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
			ePotency_Element element = ePotency_Element.NONE;
			string buff = br.ReadString();
			int buffToolTipIdx = br.ReadInt32();
			
			Tbl_Skill_Potency potency = 
				new Tbl_Skill_Potency(check_target, check_condition, check_conditionValue,
					check_target2, check_condition2, check_conditionValue2,
					type, target, duration, attribute, element, buff, buffToolTipIdx);
			
			m_listSkillPotency.Add(potency);
		}
	}
	
	public bool CheckPotencyTargetTypeIncludeEnemy()
	{
		foreach(Tbl_Skill_Potency potency in listSkillPotency)
		{
			if(potency.Potency_Target == ePotency_Target.Enemy)
				return true;
		}
		
		return false;
	}
	
	public bool CheckPotencyTypeIncludeHeal()
	{
		foreach(Tbl_Skill_Potency potency in listSkillPotency)
		{
			if(potency.Potency_Type == ePotency_Type.Heal)
				return true;
		}
		
		return false;
	}
	
	public bool CheckPotencyTypeIncludeResurrection()
	{
		foreach(Tbl_Skill_Potency potency in listSkillPotency)
		{
			if(potency.Potency_Type == ePotency_Type.Resurrection)
				return true;
		}
		
		return false;
	}
	
	public bool CheckPotencyTargetTypeIncludeSelf()
	{
		foreach(Tbl_Skill_Potency potency in listSkillPotency)
		{
			if(potency.Potency_Target == ePotency_Target.Self)
				return true;
		}
		
		return false;
	}
	
	public bool CheckNonAnimation()
	{
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record(1, m_Index);
		if(skillLevelRecord != null)
		{
			Tbl_Action_Record actionRecord = AsTableManager.Instance.GetTbl_Action_Record(skillLevelRecord.SkillAction_Index);
			if(actionRecord != null)
			{
				if(actionRecord.HitAnimation != null)
				{
					if(actionRecord.HitAnimation.FileName == "NonAnimation")
						return true;
				}
			}
		}
		
		return false;
	}
	
	public bool CheckSkillUsingOnly_Movable()
	{
		if(m_SkillIcon_Enable_Target == eSkillIcon_Enable_Target.Self)
		{
			if(m_SkillIcon_Enable_Condition == eSkillIcon_Enable_Condition.Movable)
			{
				return true;
			}
		}
		
		return false;
	}
	
//	public int GetNextPotencyIndex(int _idx)
//	{
//		if(m_listSkillPotency.Contains(_idx + 1) == true)
//		{
//			if(m_listSkillPotency[_idx+1].Potency_Type != ePotency_Type.NONE)
//				return _idx + 1;
//			else
//				return 0;
//		}
//		else
//		{
//			return 0;
//		}
//	}
}

public class Tbl_Skill_Table : AsTableBase {
	SortedList<int, Tbl_Skill_Record> m_ResourceTable = 
		new SortedList<int, Tbl_Skill_Record>();
	
	MultiDictionary<eCLASS, Tbl_Skill_Record> m_ClassTypeTable =
		new MultiDictionary<eCLASS, Tbl_Skill_Record>();
	
	MultiDictionary<eCLASS, Tbl_Skill_Record> m_mdicBaseSkill =
		new MultiDictionary<eCLASS, Tbl_Skill_Record>();
	
	Dictionary<eCLASS, MultiDictionary<float, Tbl_Skill_Record>> m_dmdicBaseSkill_Dist =
		new Dictionary<eCLASS, MultiDictionary<float, Tbl_Skill_Record>>();
	
//	Dictionary<eCLASS, Dictionary<eCommandPicking_Type, Tbl_Skill_Record>> m_ddicDoubleTabSkill =
//		new Dictionary<eCLASS, Dictionary<eCommandPicking_Type, Tbl_Skill_Record>>();
	
	public Tbl_Skill_Table(string _path)
	{
//		m_TableName = "CharacterResource";
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
				
				//debug
				idx2++;

				for( int i = 0; i < nCount; i++)
				{
					Tbl_Skill_Record record = new Tbl_Skill_Record( br);
					m_ResourceTable.Add(record.Index, record);
					
					idx++;
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
					Tbl_Skill_Record record = new Tbl_Skill_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
					
					idx++;
				}
			}
			
			//debug
			idx2++;
			
			foreach(KeyValuePair<int, Tbl_Skill_Record> pair in m_ResourceTable)
			{
				m_ClassTypeTable.Add(pair.Value.Class, pair.Value);
			}
			
			//debug
			idx2++;
			
			foreach(KeyValuePair<int, Tbl_Skill_Record> pair in m_ResourceTable)
			{
				Tbl_Skill_Record record = pair.Value;
				if(record.Skill_Type == eSKILL_TYPE.Base)
				{
					m_mdicBaseSkill.Add(record.Class, record);
				}
			}
			
			idx2 = 0;
			idx++;
			
//			foreach(KeyValuePair<int, Tbl_Skill_Record> pair in m_ResourceTable)
//			{
//				Tbl_Skill_Record record = pair.Value;
//				
//				if(record.Command_Type == eCommand_Type.DoubleTab)
//				{
//					if(m_ddicDoubleTabSkill.ContainsKey(record.Class) == false)
//						m_ddicDoubleTabSkill.Add(record.Class, new Dictionary<eCommandPicking_Type, Tbl_Skill_Record>());
//					
//					m_ddicDoubleTabSkill[record.Class].Add(record.CommandPicking_Type, record);
//				}
//			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("Tbl_Skill::LoadTable: exception occurred at " + idx + "th data.");
			Debug.LogError("Tbl_Skill::LoadTable: exception occurred at " + idx2 + "th part.");
			Debug.LogError(e);
		}
	}
	
	public void InitBasicSkill()
	{
		foreach(eCLASS node in m_mdicBaseSkill.Keys)
		{
			m_dmdicBaseSkill_Dist.Add(node, new MultiDictionary<float, Tbl_Skill_Record>());
			List<Tbl_Skill_Record> listRecord = m_mdicBaseSkill[node];
			
			listRecord.Sort(delegate(Tbl_Skill_Record x, Tbl_Skill_Record y)
			{
				Tbl_SkillLevel_Record lvX = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, x.Index);
				Tbl_SkillLevel_Record lvY = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, y.Index);
				
				return 1;
				if(lvX.Usable_Distance < lvY.Usable_Distance)
					return -1;
				else if(lvX.Usable_Distance == lvY.Usable_Distance)
					return 0;
				else
					return 1;
				
			});
			
			foreach(Tbl_Skill_Record node2 in listRecord)
			{
				Tbl_SkillLevel_Record lv = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, node2.Index);
				m_dmdicBaseSkill_Dist[node].Add(lv.Usable_Distance, node2);
			}
		}
	}
	
	public Tbl_Skill_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Skill_Table]GetRecord: there is no [" + _id + "] record");
		return null;
	}
	
	public SortedList<int, Tbl_Skill_Record> GetList()
	{
		return m_ResourceTable;
	}
	
	// bad code
	public Tbl_Skill_Record GetSkillByType(eCLASS _class, eSKILL_TYPE _type, eCommand_Type _commandType)
	{
		if(_commandType == eCommand_Type.DoubleTab)
		{
			Debug.LogWarning("Tbl_Skill_Table::GetSkillByType: invalid usage eCommand_Type.DoubleTab. " +
				"DoubleTab skills does not exist alone, So use [GetDoubleTabSkillByPickingType()] function");
			return null;
		}
		
		foreach(Tbl_Skill_Record record in m_ClassTypeTable[_class])
		{
			if(record.Skill_Type == _type)
			{
				if(_commandType == eCommand_Type.NONE)
					return record;
				else if(_commandType == record.Command_Type)
					return record;
			}
		}
		
		Debug.LogWarning( "[Tbl_Skill_Table]GetSkillByType: there is no record. class=" +
			_class + ", type = " + _type + ", command type = " + _commandType);
		return null;
	}
	
	public List<Tbl_Skill_Record> GetSkillsByClass(eCLASS _class)
	{
		return m_ClassTypeTable[_class];
	}
	
	public Tbl_Skill_Record GetRandomBaseSkill(eCLASS _class)
	{
		if(m_mdicBaseSkill.ContainsKey(_class) == true)
		{
			int count = m_mdicBaseSkill[_class].Count;
			
			return m_mdicBaseSkill[_class][UnityEngine.Random.Range(0, count)];
		}
		else
		{
			Debug.Log("GetRandomBaseSkill: Class[" + _class + "] has no base skill");
			return null;
		}
	}
	
	
	public SuitableBasicSkill GetSuitableBasicSkill(eCLASS _class, float _dist)
	{
//		if(m_mdicBaseSkill.ContainsKey(_class) == false)
//		{
//			Debug.LogError("Tbl_Skill::GetSuitableBasicSkill: unknown class = " + _class);
//			return null;
//		}
//		
//		float curDist = float.MaxValue;
//		
//		foreach(Tbl_Skill_Record node in m_mdicBaseSkill[_class])
//		{
//			Tbl_Skill_Record skill = node;
//			Tbl_SkillLevel_Record skillLevel = AsTableManager.Instance.GetTbl_SkillLevel_Record( 1, skill.Index);
//			
//			float usableDistance = skillLevel.Usable_Distance * 0.01f;
//		}
//		
//		return null;
		
		_dist *= 100f;
		
		if(m_dmdicBaseSkill_Dist.ContainsKey(_class) == false)
		{
			Debug.LogError("Tbl_Skill::GetSuitableBasicSkill: unknown class = " + _class);
			return null;
		}
		
		List<Tbl_Skill_Record> listSuitable = null;
		int count = 0;
		
		foreach(float dist in m_dmdicBaseSkill_Dist[_class].Keys)
		{
			listSuitable = m_dmdicBaseSkill_Dist[_class][dist];
			if(_dist < dist)
			{
				count = listSuitable.Count;
				return new SuitableBasicSkill(true, listSuitable[UnityEngine.Random.Range(0, count)]);
			}
		}
	
		count = listSuitable.Count;
		return new SuitableBasicSkill(false, listSuitable[UnityEngine.Random.Range(0, count)]);
	}
	
//	public Tbl_Skill_Record GetDoubleTabSkillByPickingType(eCLASS _class, eCommandPicking_Type _type)
//	{
//		if(m_ddicDoubleTabSkill.ContainsKey(_class) == true)
//		{
//			if(m_ddicDoubleTabSkill[_class].ContainsKey(_type) == true)
//			{
//				return m_ddicDoubleTabSkill[_class][_type];
//			}
//		}
//		
//		Debug.LogError("Tbl_Skill_Table::GetDoubleTabSkillByPickingType: [" + _class + "][" + _type + "]");
//		return null;
//	}
	
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
