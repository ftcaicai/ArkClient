using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

//public enum ePlayerClass {DEVINE_KNIGHT = 1, ARK_TECHNICAL, MAGICIAN, MAX}

public class Tbl_UserLevel_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	eCLASS m_Class;public eCLASS Class{get{return m_Class;}}
	int m_Level;public int Level{get{return m_Level;}}
	int m_TotalEXP;public int TotalEXP{get{return m_TotalEXP;}}
	float m_HPMax;public float HPMax{get{return m_HPMax;}}
	float m_MPMax;public float MPMax{get{return m_MPMax;}}
	float m_PhysicalAttack_Min;public float PhysicalAttack_Min{get{return m_PhysicalAttack_Min;}}
	float m_PhysicalAttack_Max;public float PhysicalAttack_Max{get{return m_PhysicalAttack_Max;}}
	float m_PhysicalDefense;public float PhysicalDefense{get{return m_PhysicalDefense;}}
	float m_MagicalAttack_Min;public float MagicalAttack_Min{get{return m_MagicalAttack_Min;}}
	float m_MagicalAttack_Max;public float MagicalAttack_Max{get{return m_MagicalAttack_Max;}}
	float m_MagicalResist;public float MagicalResist{get{return m_MagicalResist;}}
	int m_Resurrection_Cost;public int Resurrection_Cost{get{return m_Resurrection_Cost;}}
	
	int m_Lv_Bonus; public int Lv_Bonus{get{return m_Lv_Bonus;}}
	int m_Lv_BonusCount; public int Lv_BonusCount{get{return m_Lv_BonusCount;}}
	
	public Tbl_UserLevel_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue<eCLASS>(ref m_Class, node, "Class");
			SetValue(ref m_Level, node, "Level");
			SetValue(ref m_TotalEXP, node, "TotalEXP");
			SetValue(ref m_HPMax, node, "HPMax");
			SetValue(ref m_MPMax, node, "MPMax");
			SetValue(ref m_PhysicalAttack_Min, node, "PhysicalAttack_Min");
			SetValue(ref m_PhysicalAttack_Max, node, "PhysicalAttack_Max");
			SetValue(ref m_PhysicalDefense, node, "PhysicalDefense");
			SetValue(ref m_MagicalAttack_Min, node, "MagicalAttack_Min");
			SetValue(ref m_MagicalAttack_Max, node, "MagicalAttack_Max");
			SetValue(ref m_MagicalResist, node, "MagicalResist");
			SetValue(ref m_Resurrection_Cost, node, "Resurrection_Cost");
			
			SetValue(ref m_Lv_Bonus, node, "Lv_Bonus");
			SetValue(ref m_Lv_BonusCount, node, "Lv_BonusCount");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}

	public Tbl_UserLevel_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_Class = (eCLASS)br.ReadInt32();
		m_Level = br.ReadInt32();
		m_TotalEXP = br.ReadInt32();
		m_HPMax = br.ReadSingle();
		m_MPMax = br.ReadSingle();
		m_PhysicalAttack_Min = br.ReadSingle();
		m_PhysicalAttack_Max = br.ReadSingle();
		m_PhysicalDefense = br.ReadSingle();
		m_MagicalAttack_Min = br.ReadSingle();
		m_MagicalAttack_Max = br.ReadSingle();
		m_MagicalResist = br.ReadSingle();
		m_Resurrection_Cost = br.ReadInt32();
		
		m_Lv_Bonus = br.ReadInt32();
		m_Lv_BonusCount = br.ReadInt32();
	}
}

public class Tbl_UserLevel_Table : AsTableBase {
	
	static int s_FirstLevelUpRewardLv = 0; public static int FirstLevelUpRewardLv{get{return s_FirstLevelUpRewardLv;}}
	static int s_LastLevelUpRewardLv = 0; public static int LastLevelUpRewardLv{get{return s_LastLevelUpRewardLv;}}
	
	Dictionary<eCLASS, SortedList<int, Tbl_UserLevel_Record>> m_StatTable = 
		new Dictionary<eCLASS, SortedList<int, Tbl_UserLevel_Record>>();
	
	public Tbl_UserLevel_Table(string _path)
	{
//		m_TableName = "Tbl_Level";
		m_TableType = eTableType.STAT_PER_LEVEL;
		
		for(int i=0; i<(int)eCLASS.MAX; ++i)
		{
			m_StatTable.Add((eCLASS)i, new SortedList<int, Tbl_UserLevel_Record>());
		}
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
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
				Tbl_UserLevel_Record record = new Tbl_UserLevel_Record( br);
				m_StatTable[record.Class].Add(record.Level, record);
				
				if(record.Lv_Bonus != int.MaxValue && s_FirstLevelUpRewardLv == 0)
					s_FirstLevelUpRewardLv = record.Level - 1;

				if(s_FirstLevelUpRewardLv != 0 && s_LastLevelUpRewardLv == 0 &&
					record.Lv_Bonus == int.MaxValue && record.Level > s_FirstLevelUpRewardLv)
					s_LastLevelUpRewardLv = record.Level - 1;
			}

			if(s_LastLevelUpRewardLv == 0)
				s_LastLevelUpRewardLv = nCount;

			br.Close();
			stream.Close();	
		}
		else
		{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_UserLevel_Record record = new Tbl_UserLevel_Record((XmlElement)node);
				m_StatTable[record.Class].Add(record.Level, record);
				
				if(record.Lv_Bonus != int.MaxValue && s_FirstLevelUpRewardLv == 0)
					s_FirstLevelUpRewardLv = record.Level - 1;

				if(s_FirstLevelUpRewardLv != 0 && s_LastLevelUpRewardLv == 0 &&
				   record.Lv_Bonus == int.MaxValue && record.Level > s_FirstLevelUpRewardLv)
					s_LastLevelUpRewardLv = record.Level - 1;
			}

			if(s_LastLevelUpRewardLv == 0)
			{
				Debug.Log("Tbl_UserLevel:: LoadTable: nodes.Count = " + nodes.Count);
				s_LastLevelUpRewardLv = nodes.Count;
			}
		}
	}
	
	public Tbl_UserLevel_Record GetRecord(eCLASS _class, int _level)
	{
		if(m_StatTable.ContainsKey(_class) == true)
		{
			if(m_StatTable[_class].ContainsKey(_level) == true)
				return m_StatTable[_class][_level];
		}
		
		Debug.LogWarning("[Tbl_UserLevel_Table]GetRecord: there is no record");
		return null;
	}
}
