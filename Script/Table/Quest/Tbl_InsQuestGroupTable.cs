using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_InsQuestGroup_Record
{
	int m_Ins_QuestGroup_ID;
	int m_Ins_Quest_ID;
	int m_Ins_Group_Rate;
	int m_Monster_Kind_ID1;
	int m_Monster1_Kill_Count;
	int m_Monster_Kind_ID2;
	int m_Monster2_Kill_Count;
	int m_Monster_Kind_ID3;
	int m_Monster3_Kill_Count;
	int m_Exp_Reward;
	int m_Gold_Reward;
	int m_Knight_Reward;
	int m_Knight_Reward_Count;
	int m_Magician_Reward;
	int m_Magician_Reward_Count;
	int m_Cleric_Reward;
	int m_Cleric_Reward_Count;
	int m_Hunter_Reward;
	int m_Hunter_Reward_Count;

	public int Ins_QuestGroup_ID{ get{ return m_Ins_QuestGroup_ID;}}
	public int Ins_Quest_ID{ get{ return m_Ins_Quest_ID;}}
	public int Ins_Group_Rate{ get{ return m_Ins_Group_Rate;}}
	public int Monster_Kind_ID1{ get{ return m_Monster_Kind_ID1;}}
	public int Monster1_Kill_Count{ get{ return m_Monster1_Kill_Count;}}
	public int Monster_Kind_ID2{ get{ return m_Monster_Kind_ID2;}}
	public int Monster2_Kill_Count{ get{ return m_Monster2_Kill_Count;}}
	public int Monster_Kind_ID3{ get{ return m_Monster_Kind_ID3;}}
	public int Monster3_Kill_Count{ get{ return m_Monster3_Kill_Count;}}
	public int Exp_Reward{ get{ return m_Exp_Reward;}}
	public int Gold_Reward{ get{ return m_Gold_Reward;}}
	public int Knight_Reward{ get{ return m_Knight_Reward;}}
	public int Knight_Reward_Count{ get{ return m_Knight_Reward_Count;}}
	public int Magician_Reward{ get{ return m_Magician_Reward;}}
	public int Magician_Reward_Count{ get{ return m_Magician_Reward_Count;}}
	public int Cleric_Reward{ get{ return m_Cleric_Reward;}}
	public int Cleric_Reward_Count{ get{ return m_Cleric_Reward_Count;}}
	public int Hunter_Reward{ get{ return m_Hunter_Reward;}}
	public int Hunter_Reward_Count{ get{ return m_Hunter_Reward_Count;}}
	
	public Tbl_InsQuestGroup_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_Ins_QuestGroup_ID = int.Parse(node["Ins_QuestGroup_ID"].InnerText);
			m_Ins_Quest_ID = int.Parse(node["Ins_Quest_ID"].InnerText);
			m_Ins_Group_Rate = int.Parse(node["Ins_Group_Rate"].InnerText);
			m_Monster_Kind_ID1 = int.Parse(node["Monster_Kind_ID1"].InnerText);
			m_Monster1_Kill_Count = int.Parse(node["Monster1_Kill_Count"].InnerText);
			m_Monster_Kind_ID2 = int.Parse(node["Monster_Kind_ID2"].InnerText);
			m_Monster2_Kill_Count = int.Parse(node["Monster2_Kill_Count"].InnerText);
			m_Monster_Kind_ID3 = int.Parse(node["Monster_Kind_ID3"].InnerText);
			m_Monster3_Kill_Count = int.Parse(node["Monster3_Kill_Count"].InnerText);
			m_Exp_Reward = int.Parse(node["Exp_Reward"].InnerText);
			m_Gold_Reward = int.Parse(node["Gold_Reward"].InnerText);
			m_Knight_Reward = int.Parse(node["Knight_Reward"].InnerText);
			m_Knight_Reward_Count = int.Parse(node["Knight_Reward_Count"].InnerText);
			m_Magician_Reward = int.Parse(node["Magician_Reward"].InnerText);
			m_Magician_Reward_Count = int.Parse(node["Magician_Reward_Count"].InnerText);
			m_Cleric_Reward = int.Parse(node["Cleric_Reward"].InnerText);
			m_Cleric_Reward_Count = int.Parse(node["Cleric_Reward_Count"].InnerText);
			m_Hunter_Reward = int.Parse(node["Hunter_Reward"].InnerText);
			m_Hunter_Reward_Count = int.Parse(node["Hunter_Reward_Count"].InnerText);
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_InsQuestGroup_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_InsQuestGroup_Table : AsTableBase
{
	Dictionary<int, Tbl_InsQuestGroup_Record> m_InsQuestGroupTable = new Dictionary<int, Tbl_InsQuestGroup_Record>();
	
	public Tbl_InsQuestGroup_Table(string _path)
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
			Tbl_InsQuestGroup_Record record = new Tbl_InsQuestGroup_Record((XmlElement)node);
			m_InsQuestGroupTable.Add(record.Ins_Quest_ID, record);
		}
	}
	
	public Tbl_InsQuestGroup_Record GetRecord(int _id)
	{
		if(m_InsQuestGroupTable.ContainsKey(_id) == true)
			return m_InsQuestGroupTable[_id];
		
		Debug.LogError("[Tbl_InsQuestGroupTable_Table]GetRecord: there is no record");
		return null;
	}
}
