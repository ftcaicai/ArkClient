using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public enum eChoiceType {Base, Choice}

public class Tbl_SkillBook_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	int coupleIndex;	public int CoupleIndex	{ get { return coupleIndex; } }
	eCLASS m_Class;public eCLASS Class{get{return m_Class;}}
	int m_ClassLevel;public int ClassLevel{get{return m_ClassLevel;}}
	int m_SkillShop_BuyAmount;public int SkillShop_BuyAmount{get{return m_SkillShop_BuyAmount;}}
	eChoiceType m_ChoiceType;public eChoiceType ChoiceType{get{return m_ChoiceType;}}
	int m_Skill1_Index;public int Skill1_Index{get{return m_Skill1_Index;}}
	int m_Skill1_Level;public int Skill1_Level{get{return m_Skill1_Level;}}
	int m_Skill2_Index;public int Skill2_Index{get{return m_Skill2_Index;}}
	int m_Skill2_Level;public int Skill2_Level{get{return m_Skill2_Level;}}
	
	public Tbl_SkillBook_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue( ref coupleIndex, node, "CoupleIndex");
			SetValue<eCLASS>(ref m_Class, node, "Class");
			SetValue(ref m_ClassLevel, node, "ClassLevel");
			SetValue(ref m_SkillShop_BuyAmount, node, "SkillShop_BuyAmount");
			SetValue<eChoiceType>(ref m_ChoiceType, node, "ChoiceType");
			SetValue(ref m_Skill1_Index, node, "Skill1_Index");
			SetValue(ref m_Skill1_Level, node, "Skill1_Level");
			SetValue(ref m_Skill2_Index, node, "Skill2_Index");
			SetValue(ref m_Skill2_Level, node, "Skill2_Level");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);//
		}
	}
	
	
	public Tbl_SkillBook_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		coupleIndex = br.ReadInt32();
		m_Class = (eCLASS)br.ReadInt32();
		m_ClassLevel = br.ReadInt32();
		m_SkillShop_BuyAmount = br.ReadInt32();
		m_ChoiceType = (eChoiceType)br.ReadInt32();
		m_Skill1_Index = br.ReadInt32();
		m_Skill1_Level = br.ReadInt32();		
		m_Skill2_Index = br.ReadInt32();	
		m_Skill2_Level = br.ReadInt32();			
	}

}

public class Tbl_SkillBook_Table : AsTableBase {
	SortedList<int, Tbl_SkillBook_Record> m_ResourceTable = 
		new SortedList<int, Tbl_SkillBook_Record>();
	
	Dictionary<eCLASS, Dictionary<int, Dictionary<int, Tbl_SkillBook_Record>>> m_dddicClass = 
		new Dictionary<eCLASS, Dictionary<int, Dictionary<int, Tbl_SkillBook_Record>>>();
	
	Dictionary<eCLASS, Dictionary<int, Tbl_SkillBook_Record>> m_ddicClass =
		new Dictionary<eCLASS, Dictionary<int, Tbl_SkillBook_Record>>();
			
	
	public Tbl_SkillBook_Table(string _path)
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
					Tbl_SkillBook_Record record = new Tbl_SkillBook_Record(br);
					m_ResourceTable.Add(record.Index, record);
					
					if(m_dddicClass.ContainsKey(record.Class) == false)
					{
						m_dddicClass.Add(record.Class, new Dictionary<int, Dictionary<int, Tbl_SkillBook_Record>>());
						m_ddicClass.Add(record.Class, new Dictionary<int, Tbl_SkillBook_Record>());
					}
					m_ddicClass[record.Class].Add(record.Index, record);
					
					if(m_dddicClass[record.Class].ContainsKey(record.Skill1_Index) == false)
						m_dddicClass[record.Class].Add(record.Skill1_Index, new Dictionary<int, Tbl_SkillBook_Record>());
					
					if(m_dddicClass[record.Class][record.Skill1_Index].ContainsKey(record.Skill1_Level) == false)
						m_dddicClass[record.Class][record.Skill1_Index].Add(record.Skill1_Level, record);
					if(record.ChoiceType == eChoiceType.Choice)
					{
						if(m_dddicClass[record.Class].ContainsKey(record.Skill2_Index) == false)
							m_dddicClass[record.Class].Add(record.Skill2_Index, new Dictionary<int, Tbl_SkillBook_Record>());
						
						if(m_dddicClass[record.Class][record.Skill2_Index].ContainsKey(record.Skill2_Level) == false)
							m_dddicClass[record.Class][record.Skill2_Index].Add(record.Skill2_Level, record);
					}
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
					Tbl_SkillBook_Record record = new Tbl_SkillBook_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
					
					if(m_dddicClass.ContainsKey(record.Class) == false)
					{
						m_dddicClass.Add(record.Class, new Dictionary<int, Dictionary<int, Tbl_SkillBook_Record>>());
						m_ddicClass.Add(record.Class, new Dictionary<int, Tbl_SkillBook_Record>());
					}
					m_ddicClass[record.Class].Add(record.Index, record);
					
					if(m_dddicClass[record.Class].ContainsKey(record.Skill1_Index) == false)
						m_dddicClass[record.Class].Add(record.Skill1_Index, new Dictionary<int, Tbl_SkillBook_Record>());
					
					if(m_dddicClass[record.Class][record.Skill1_Index].ContainsKey(record.Skill1_Level) == false)
						m_dddicClass[record.Class][record.Skill1_Index].Add(record.Skill1_Level, record);
					if(record.ChoiceType == eChoiceType.Choice)
					{
						if(m_dddicClass[record.Class].ContainsKey(record.Skill2_Index) == false)
							m_dddicClass[record.Class].Add(record.Skill2_Index, new Dictionary<int, Tbl_SkillBook_Record>());
						
						if(m_dddicClass[record.Class][record.Skill2_Index].ContainsKey(record.Skill2_Level) == false)
							m_dddicClass[record.Class][record.Skill2_Index].Add(record.Skill2_Level, record);
					}
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_SkillBook_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Skill_Table]GetRecord: there is no [" + _id + "] record");
		return null;
	}
	
//	public List<Tbl_SkillBook_Record> GetRecordsByClass(eCLASS _class)
//	{
//		if(m_mdicClass.ContainsKey(_class) == true)
//		{
//			return m_mdicClass[_class];
//		}
//		else
//		{
//			Debug.LogError("[Tbl_Skill_Table]GetRecordsByClass: there is no [" + _class + "] record");
//			return null;
//		}
//	}
//	
//	public MultiDictionary<int, Tbl_SkillBook_Record> GetRecordsByClass(eCLASS _class)
//	{
////		if(m_dmdicClass.ContainsKey(_class) == true)
////		{
//////			MultiDictionary<int, Tbl_SkillBook_Record> __value = m_dmdicClass[_class];
//////			return __value;
//////			
//////			return m_dmdicClass[_class];
////		}
////		else
//		{
//			Debug.LogError("[Tbl_Skill_Table]GetRecordsByClass: there is no [" + _class + "] record");
//			return null;
//		}
//	}
	
	public Tbl_SkillBook_Record GetRecord(eCLASS _class, int _skillIdx, int _skillLv)
	{
		if(m_dddicClass.ContainsKey(_class) == true)
		{
			if(m_dddicClass[_class].ContainsKey(_skillIdx) == true)
			{
				if(m_dddicClass[_class][_skillIdx].ContainsKey(_skillLv) == true)
				{
					return m_dddicClass[_class][_skillIdx][_skillLv];
				}
			}
			
			return null;
		}
		else
		{
			Debug.LogError("[Tbl_Skill_Table]GetRecordsByClass: there is no [" + _class + "] record");
			return null;
		}
	}
	
	public Dictionary<int, Tbl_SkillBook_Record> GetRecordsByClass(eCLASS _class)
	{
		if(m_ddicClass.ContainsKey(_class) == true)
			return m_ddicClass[_class];
		else
			return null;
	}
	
	public SortedList<int, Tbl_SkillBook_Record> GetList()
	{
		return m_ResourceTable;
	}
}

public class SortByClassLevel : IComparer //IEqualityComparer
{
	int IComparer.Compare(object _record1, object _record2)
	{
		Tbl_SkillBook_Record r1 = _record1 as Tbl_SkillBook_Record;
		Tbl_SkillBook_Record r2 = _record2 as Tbl_SkillBook_Record;
		
		if(r1.ClassLevel > r2.ClassLevel)
			return 1;
		else if(r1.ClassLevel < r2.ClassLevel)
			return -1;
		else
			return 0;
	}
	
//	public new bool Equals(object _record1, object _record2)
//	{
//		Tbl_SkillBook_Record r1 = _record1;
//		Tbl_SkillBook_Record r2 = _record2;
//		
//		if(r1.ClassLevel > r2.ClassLevel)
//			return true;
//		else if(r1.ClassLevel < r2.ClassLevel)
//			return false;
//		else
//			return false;
//	}
}