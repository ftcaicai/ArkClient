using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class Tbl_InDun_Record
{
	int m_ID;
	int m_InstanceTableIndex;
	int m_IndunType;
	int m_ArenaType;
	int m_InsGroup;
	int m_nName;
	int m_nDescription;
	int m_nStartMsg;
	int m_nEndMsg;
	string m_strIcon;
	int m_nMinLv;
	int m_nMaxLv;
//	int m_nMaxPlayerCount;
//	string m_strGrade;
//	string m_strStartDay;
//	string m_strFinishDay;
//	int m_nStartHour;
//	int m_nFinishHour;
//	int m_nStartDayText;
//	int m_nFinishDayText;
//	int m_nComplete_1;
//	int m_nCompleteValue_1;
//	int m_nComplete_2;
//	int m_nCompleteValue_2;
//	int m_nComplete_3;
//	int m_nCompleteValue_3;
//	int m_nPvPLimitCount;
	int m_nRankPvP;
	int m_nPracticePvP;
	int m_nPvPDuration;
	int m_nIns_RaidIcon;
	
	public int ID{ get{ return m_ID;}}
	public int InstanceTableIndex{ get{ return m_InstanceTableIndex;}}
	public int IndunType{ get{ return m_IndunType;}}
	public int ArenaType{ get{ return m_ArenaType;}}
	public int InsGroup{ get{ return m_InsGroup;}}
	public int Name{ get{ return m_nName;}}
	public int Description{ get{ return m_nDescription;}}
	public int StartMsg{ get{ return m_nStartMsg;}}
	public int EndMsg{ get{ return m_nEndMsg;}}
	public string Icon{ get{ return m_strIcon;}}
	public int MinLv{ get{ return m_nMinLv;}}
	public int MaxLv{ get{ return m_nMaxLv;}}
//	public int MaxPlayerCount{ get{ return m_nMaxPlayerCount;}}
//	public string Grade{ get{ return m_strGrade;}}
//	public string StartDay{ get{ return m_strStartDay;}}
//	public string FinishDay{ get{ return m_strFinishDay;}}
//	public int StartHour{ get{ return m_nStartHour;}}
//	public int FinishHour{ get{ return m_nFinishHour;}}
//	public int StartDayText{ get{ return m_nStartDayText;}}
//	public int FinishDayText{ get{ return m_nFinishDayText;}}
//	public int Complete_1{ get{ return m_nComplete_1;}}
//	public int CompleteValue_1{ get{ return m_nCompleteValue_1;}}
//	public int Complete_2{ get{ return m_nComplete_2;}}
//	public int CompleteValue_2{ get{ return m_nCompleteValue_2;}}
//	public int Complete_3{ get{ return m_nComplete_3;}}
//	public int CompleteValue_3{ get{ return m_nCompleteValue_3;}}
//	public int PvPLimitCount{ get{ return m_nPvPLimitCount;}}
	public int RankPvP{ get{ return m_nRankPvP;}}
	public int PracticePvP{ get{ return m_nPracticePvP;}}
	public int PvPDuration{ get{ return m_nPvPDuration;}}
	public int RaidIcon{ get{ return m_nIns_RaidIcon;}}
	
	public Tbl_InDun_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_ID = int.Parse(node["ID"].InnerText);
			m_InstanceTableIndex = int.Parse(node["InstanceTableIdx"].InnerText);
			m_IndunType = int.Parse(node["IndunType"].InnerText);
			m_ArenaType = int.Parse(node["ArenaType"].InnerText);
			m_InsGroup = int.Parse(node["insGuorp"].InnerText);
			m_nName = int.Parse(node["Ins_Name"].InnerText);
			m_nDescription = int.Parse(node["Ins_Description"].InnerText);
			m_nStartMsg = int.Parse(node["Ins_StartMessage"].InnerText);
			m_nEndMsg = int.Parse(node["Ins_EndMessage"].InnerText);
			m_strIcon = node["Ins_Icon"].InnerText;
			m_nMinLv = int.Parse(node["Ins_Min_Lv"].InnerText);
			m_nMaxLv = int.Parse(node["Ins_Max_Lv"].InnerText);
//			m_nMaxPlayerCount = int.Parse(node["Ins_MaxPlayerCount"].InnerText);
//			m_strGrade = node["Grade"].InnerText;
//			m_strStartDay = node["Ins_StartDay"].InnerText;
//			m_strFinishDay = node["Ins_FinishDay"].InnerText;
//			m_nStartHour = int.Parse(node["Ins_StartHour"].InnerText);
//			m_nFinishHour = int.Parse(node["Ins_FinishHour"].InnerText);
//			m_nStartDayText = int.Parse(node["Ins_StartDayText"].InnerText);
//			m_nFinishDayText = int.Parse(node["Ins_FinishDayText"].InnerText);
//			m_nComplete_1 = int.Parse(node["Ins_Complete1"].InnerText);
//			m_nCompleteValue_1 = int.Parse(node["Complete_Value1"].InnerText);
//			m_nComplete_2 = int.Parse(node["Ins_Complete2"].InnerText);
//			m_nCompleteValue_2 = int.Parse(node["Complete_Value2"].InnerText);
//			m_nComplete_3 = int.Parse(node["Ins_Complete3"].InnerText);
//			m_nCompleteValue_3 = int.Parse(node["Complete_Value3"].InnerText);
//			m_nPvPLimitCount = int.Parse(node["PvPLimitCount"].InnerText);
			m_nRankPvP = int.Parse(node["RankPvP"].InnerText);
			m_nPracticePvP = int.Parse(node["PracticePvP"].InnerText);
			m_nPvPDuration = int.Parse(node["PvPDuration"].InnerText);
			m_nIns_RaidIcon = int.Parse(node["Ins_RaidIcon"].InnerText);
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_InDun_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_InDun_Table : AsTableBase
{
	Dictionary<int, Tbl_InDun_Record> m_InDunTable = new Dictionary<int, Tbl_InDun_Record>();
	
	public Tbl_InDun_Table(string _path)
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
			Tbl_InDun_Record record = new Tbl_InDun_Record((XmlElement)node);
			m_InDunTable.Add(record.ID, record);
		}
	}
	
	public Tbl_InDun_Record GetRecord(int _id)
	{
		if(m_InDunTable.ContainsKey(_id) == true)
			return m_InDunTable[_id];
		
		Debug.LogError("[Tbl_InDunTable_Table]GetRecord: there is no record");
		return null;
	}
	
	public int GetCount()
	{
		return m_InDunTable.Count;
	}

	public Tbl_InDun_Record GetRecord_SearchByMinLevel(int nLevel)
	{
		Dictionary<int, Tbl_InDun_Record>.Enumerator	ir = m_InDunTable.GetEnumerator();
		while (ir.MoveNext()) 
		{
			KeyValuePair<int, Tbl_InDun_Record> pair = 	ir.Current;

			if( nLevel == pair.Value.MinLv )
				return pair.Value;
		}

		return null;
	}
}












