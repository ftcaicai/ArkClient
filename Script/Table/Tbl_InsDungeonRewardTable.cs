using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_InsDungeonReward_Record
{
	int m_ID;
	int m_InstanceTableIdx;
	int m_nMaxPlayerCount;
	string m_strGrade;
	int m_nPrecede;
	bool m_bResurrection;
	int m_nFirst_Reward;
	int m_nFirst_Reward_Count;
	int m_Use;
	int m_Normal_Level;
	int m_Hell_Level;
	int m_Common_Reward1;
	int m_Common_Reward2;
	int m_Common_Reward3;
	int m_Common_Reward4;
	int m_Common_Reward5;
	int m_DivineKnight_Reward_Ex1;
	int m_DivineKnight_Reward_Ex2;
	int m_DivineKnight_Reward_Ex3;
	int m_DivineKnight_Reward_Ex4;
	int m_DivineKnight_Reward_Ex5;
	int m_Magician_Reward_Ex1;
	int m_Magician_Reward_Ex2;
	int m_Magician_Reward_Ex3;
	int m_Magician_Reward_Ex4;
	int m_Magician_Reward_Ex5;
	int m_Cleric_Reward_Ex1;
	int m_Cleric_Reward_Ex2;
	int m_Cleric_Reward_Ex3;
	int m_Cleric_Reward_Ex4;
	int m_Cleric_Reward_Ex5;
	int m_Hunter_Reward_Ex1;
	int m_Hunter_Reward_Ex2;
	int m_Hunter_Reward_Ex3;
	int m_Hunter_Reward_Ex4;
	int m_Hunter_Reward_Ex5;

	public int ID{ get{ return m_ID;}}
	public int InstanceTableIdx{ get{ return m_InstanceTableIdx;}}
	public int MaxPlayerCount{ get{ return m_nMaxPlayerCount;}}
	public string Grade{ get{ return m_strGrade;}}
	public int Precede{ get{ return m_nPrecede;}}
	public bool Resurrection{ get{ return m_bResurrection;}}
	public int First_Reward{ get{ return m_nFirst_Reward;}}
	public int First_Reward_Count{ get{ return m_nFirst_Reward_Count;}}
	public int Use{ get{ return m_Use;}}
	public int Normal_Level{ get{ return m_Normal_Level;}}
	public int Hell_Level{ get{ return m_Hell_Level;}}
	public int Common_Reward1{ get{ return m_Common_Reward1;}}
	public int Common_Reward2{ get{ return m_Common_Reward2;}}
	public int Common_Reward3{ get{ return m_Common_Reward3;}}
	public int Common_Reward4{ get{ return m_Common_Reward4;}}
	public int Common_Reward5{ get{ return m_Common_Reward5;}}
	public int DivineKnight_Reward_Ex1{ get{ return m_DivineKnight_Reward_Ex1;}}
	public int DivineKnight_Reward_Ex2{ get{ return m_DivineKnight_Reward_Ex2;}}
	public int DivineKnight_Reward_Ex3{ get{ return m_DivineKnight_Reward_Ex3;}}
	public int DivineKnight_Reward_Ex4{ get{ return m_DivineKnight_Reward_Ex4;}}
	public int DivineKnight_Reward_Ex5{ get{ return m_DivineKnight_Reward_Ex5;}}
	public int Magician_Reward_Ex1{ get{ return m_Magician_Reward_Ex1;}}
	public int Magician_Reward_Ex2{ get{ return m_Magician_Reward_Ex2;}}
	public int Magician_Reward_Ex3{ get{ return m_Magician_Reward_Ex3;}}
	public int Magician_Reward_Ex4{ get{ return m_Magician_Reward_Ex4;}}
	public int Magician_Reward_Ex5{ get{ return m_Magician_Reward_Ex5;}}
	public int Cleric_Reward_Ex1{ get{ return m_Cleric_Reward_Ex1;}}
	public int Cleric_Reward_Ex2{ get{ return m_Cleric_Reward_Ex2;}}
	public int Cleric_Reward_Ex3{ get{ return m_Cleric_Reward_Ex3;}}
	public int Cleric_Reward_Ex4{ get{ return m_Cleric_Reward_Ex4;}}
	public int Cleric_Reward_Ex5{ get{ return m_Cleric_Reward_Ex5;}}
	public int Hunter_Reward_Ex1{ get{ return m_Hunter_Reward_Ex1;}}
	public int Hunter_Reward_Ex2{ get{ return m_Hunter_Reward_Ex2;}}
	public int Hunter_Reward_Ex3{ get{ return m_Hunter_Reward_Ex3;}}
	public int Hunter_Reward_Ex4{ get{ return m_Hunter_Reward_Ex4;}}
	public int Hunter_Reward_Ex5{ get{ return m_Hunter_Reward_Ex5;}}

	public Tbl_InsDungeonReward_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_ID = int.Parse(node["ID"].InnerText);
			m_InstanceTableIdx = int.Parse(node["InDun_ID"].InnerText);
			m_nMaxPlayerCount = int.Parse(node["Ins_MaxPlayerCount"].InnerText);
			m_strGrade = node["Ins_Grade"].InnerText;
			m_nPrecede = int.Parse(node["Ins_Precede"].InnerText);
			m_bResurrection = bool.Parse(node["Ins_Resurrection"].InnerText);
			m_nFirst_Reward = int.Parse(node["First_Reward"].InnerText);
			m_nFirst_Reward_Count = int.Parse(node["First_Reward_Count"].InnerText);
			m_Use = int.Parse(node["Ins_Use"].InnerText);
			m_Normal_Level = int.Parse(node["Ins_Normal_Level"].InnerText);
			m_Hell_Level = int.Parse(node["Ins_Hell_Level"].InnerText);
			m_Common_Reward1 = int.Parse(node["Common_Reward1"].InnerText);
			m_Common_Reward2 = int.Parse(node["Common_Reward2"].InnerText);
			m_Common_Reward3 = int.Parse(node["Common_Reward3"].InnerText);
			m_Common_Reward4 = int.Parse(node["Common_Reward4"].InnerText);
			m_Common_Reward5 = int.Parse(node["Common_Reward5"].InnerText);
			m_DivineKnight_Reward_Ex1 = int.Parse(node["DivineKnight_Reward_Ex1"].InnerText);
			m_DivineKnight_Reward_Ex2 = int.Parse(node["DivineKnight_Reward_Ex2"].InnerText);
			m_DivineKnight_Reward_Ex3 = int.Parse(node["DivineKnight_Reward_Ex3"].InnerText);
			m_DivineKnight_Reward_Ex4 = int.Parse(node["DivineKnight_Reward_Ex4"].InnerText);
			m_DivineKnight_Reward_Ex5 = int.Parse(node["DivineKnight_Reward_Ex5"].InnerText);
			m_Magician_Reward_Ex1 = int.Parse(node["Magician_Reward_Ex1"].InnerText);
			m_Magician_Reward_Ex2 = int.Parse(node["Magician_Reward_Ex2"].InnerText);
			m_Magician_Reward_Ex3 = int.Parse(node["Magician_Reward_Ex3"].InnerText);
			m_Magician_Reward_Ex4 = int.Parse(node["Magician_Reward_Ex4"].InnerText);
			m_Magician_Reward_Ex5 = int.Parse(node["Magician_Reward_Ex5"].InnerText);
			m_Cleric_Reward_Ex1 = int.Parse(node["Cleric_Reward_Ex1"].InnerText);
			m_Cleric_Reward_Ex2 = int.Parse(node["Cleric_Reward_Ex2"].InnerText);
			m_Cleric_Reward_Ex3 = int.Parse(node["Cleric_Reward_Ex3"].InnerText);
			m_Cleric_Reward_Ex4 = int.Parse(node["Cleric_Reward_Ex4"].InnerText);
			m_Cleric_Reward_Ex5 = int.Parse(node["Cleric_Reward_Ex5"].InnerText);
			m_Hunter_Reward_Ex1 = int.Parse(node["Hunter_Reward_Ex1"].InnerText);
			m_Hunter_Reward_Ex2 = int.Parse(node["Hunter_Reward_Ex2"].InnerText);
			m_Hunter_Reward_Ex3 = int.Parse(node["Hunter_Reward_Ex3"].InnerText);
			m_Hunter_Reward_Ex4 = int.Parse(node["Hunter_Reward_Ex4"].InnerText);
			m_Hunter_Reward_Ex5 = int.Parse(node["Hunter_Reward_Ex5"].InnerText);
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_InsDungeonReward_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_InsDungeonReward_Table : AsTableBase
{
	Dictionary<int, Tbl_InsDungeonReward_Record> m_InsDungeonRewardTable = new Dictionary<int, Tbl_InsDungeonReward_Record>();
	
	public Tbl_InsDungeonReward_Table(string _path)
	{
		m_TableType = eTableType.STRING;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		XmlElement root = GetXmlRootElement(_path);
		XmlNodeList nodes = root.ChildNodes;
		
		foreach(XmlNode node in nodes)
		{
			Tbl_InsDungeonReward_Record record = new Tbl_InsDungeonReward_Record((XmlElement)node);
			m_InsDungeonRewardTable.Add(record.ID, record);
		}
	}
	
	public Tbl_InsDungeonReward_Record GetRecord(int _id)
	{
		if(m_InsDungeonRewardTable.ContainsKey(_id) == true)
			return m_InsDungeonRewardTable[_id];
		
//		Debug.LogError("[Tbl_InsDungeonRewardTable_Table]GetRecord: there is no record");
		return null;
	}

	public Dictionary<int, Tbl_InsDungeonReward_Record> GetInsRewardRecordList()
	{
		return m_InsDungeonRewardTable;
	}
}
