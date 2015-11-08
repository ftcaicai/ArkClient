using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class EventAchievement
{
	public int itemID;
	public int itemCount;
	public int buffID;
	public int exp;
	public int lotteryID;

	public EventAchievement() { }

	public EventAchievement(int _itemID, int _itemCount)//, int _buffID, int _exp, int _lottery)
	{
		itemID		= _itemID;
		itemCount	= _itemCount;
		//buffID		= _buffID;
		//exp			= _exp;
		//lotteryID	= _lottery;
	}

	public override string ToString()
	{
		return "[EventAchieve] ItemID = " + itemID + " , Item Count = " + itemCount;
	}
}

public class Tbl_Event_Record : AsTableRecord
{
	public int npcID;
	public int eventIdx;
	public int titleID;
	public int txtID;
	public bool viewNpc;
	public bool viewList;
	public System.DateTime startDate;
	public System.DateTime endDate;
	public System.DateTime onTime;
	public System.DateTime offTime;
	public List<EventAchievement> listEventAchievement = new List<EventAchievement>();

	public bool noneOnOffTime = false;

	public override string ToString()
	{
		string szTemp = "[event " + eventIdx + "] =" + "npc = " + npcID + ", " + titleID + ", " + txtID +
					    "start = " + startDate.ToString() + " end = " + endDate.ToString() + "\n";

		foreach(EventAchievement ach in listEventAchievement)
		{
			szTemp += ach.ToString();
			szTemp +="\n";
		}

		return szTemp;

	}

	public bool CanStartEvent(System.DateTime _date)
	{
		if (noneOnOffTime == false)
		{
			bool date = startDate <= _date && endDate > _date;

			System.DateTime nowTime		= new System.DateTime(1970, 1, 1, _date.Hour, _date.Minute, 0);
			System.DateTime startTime	= new System.DateTime(1970, 1, 1, onTime.Hour, onTime.Minute, 0);
			System.DateTime endTime		= new System.DateTime(1970, 1, 1, offTime.Hour, offTime.Minute, 0);

			bool time = nowTime >= startTime && nowTime <= endTime;

			return date & time;
		}
		else
		{
			return startDate <= _date && endDate > _date;
		}

	}

	public Tbl_Event_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;

			npcID		= int.Parse(node["NPC_ID"].InnerText);
			eventIdx	= int.Parse(node["Index"].InnerText);
			titleID		= int.Parse(node["Title_ID"].InnerText);
			txtID		= int.Parse(node["Txt_ID"].InnerText);
			viewNpc     = bool.Parse(node["NPC_View"].InnerText);
			viewList	= bool.Parse(node["List_View"].InnerText);

			string date	= node["Start_Date"].InnerText;
			int year	= System.Convert.ToInt32(date.Substring(0 , 4));
			int month	= System.Convert.ToInt32(date.Substring(4 , 2));
			int day		= System.Convert.ToInt32(date.Substring(6 , 2));
			int hour	= System.Convert.ToInt32(date.Substring(8 , 2));
			int minute	= System.Convert.ToInt32(date.Substring(10, 2));

			startDate = new System.DateTime(year, month, day, hour, minute, 0);

			date	= node["End_Date"].InnerText;
			year	= System.Convert.ToInt32(date.Substring(0 , 4));
			month	= System.Convert.ToInt32(date.Substring(4 , 2));
			day		= System.Convert.ToInt32(date.Substring(6 , 2));
			hour	= System.Convert.ToInt32(date.Substring(8 , 2));
			minute	= System.Convert.ToInt32(date.Substring(10, 2));

			endDate = new System.DateTime(year, month, day, hour, minute, 0);
			
			int onHour	  = 0;
			int onMinute  = 0;
			int offHour   = 0;
			int offMinute = 0;

			string tempOnTime  = node["On_Time"].InnerText;
			string tempOffTime = node["Off_Time"].InnerText;

			if (tempOnTime != "NONE")
			{
				onHour = System.Convert.ToInt32(tempOnTime.Substring(0, 2));
				onMinute = System.Convert.ToInt32(tempOnTime.Substring(2, 2));

				onTime = new System.DateTime(1970, 1, 1, onHour, onMinute, 0);
			}
			else
			{
				noneOnOffTime = true;
			}

			if (tempOffTime != "NONE")
			{
				offHour = System.Convert.ToInt32(tempOffTime.Substring(0, 2));
				offMinute = System.Convert.ToInt32(tempOffTime.Substring(2, 2));

				offTime = new System.DateTime(1970, 1, 1, offHour, offMinute, 0);
			}
			else
			{
				noneOnOffTime = true;
			}

			listEventAchievement = new List<EventAchievement>();

			for (int i = 1; i < 4; i++)
			{
				string id = node["Item_ID" + i.ToString()].InnerText;

				if (id == "NONE")
					continue;

				string itemCount = node["Item_Count" + i.ToString()].InnerText;

				#region --
				//string buffID	 = node["Buff_ID" + i.ToString()].InnerText;
				//string Exp		 = node["Exp" + i.ToString()].InnerText;
				//string lotteryID = node["Lottery_ID" + i.ToString()].InnerText;


				//data = new EventData(int.Parse(id),
				//                     int.Parse(itemCount),
				//                     buffID		!= "NONE" ? int.Parse(buffID) : -1,
				//                     Exp		!= "NONE" ? int.Parse(Exp) : -1,
				//                     lotteryID	!= "NONE" ? int.Parse(lotteryID) : -1);
				#endregion

				EventAchievement achievement = new EventAchievement(int.Parse(id), int.Parse(itemCount));

				listEventAchievement.Add(achievement);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Event_Table] : " + e + "erroe while parsing");
		}
	}
}

public class Tbl_Event_Table : AsTableBase 
{
	Dictionary<int, List<Tbl_Event_Record>> m_ResourceTableNpc = new Dictionary<int, List<Tbl_Event_Record>>();
	Dictionary<int, Tbl_Event_Record> m_ResourceTableAll = new Dictionary<int, Tbl_Event_Record>();

	public Tbl_Event_Table(string _path)
	{
		LoadTable(_path);
	}

	public override void LoadTable(string _path)
	{
		try
		{
			XmlElement root		= GetXmlRootElement(_path);
			XmlNodeList nodes	= root.SelectNodes("Event");
			m_ResourceTableNpc = new Dictionary<int, List<Tbl_Event_Record>>();

			foreach (XmlNode node in nodes)
			{
				Tbl_Event_Record record = new Tbl_Event_Record((XmlElement)node);

				if (m_ResourceTableNpc.ContainsKey(record.npcID))
					m_ResourceTableNpc[record.npcID].Add(record);
				else
				{
					m_ResourceTableNpc.Add(record.npcID, new List<Tbl_Event_Record>());
					m_ResourceTableNpc[record.npcID].Add(record);
				}
				
				if (!m_ResourceTableAll.ContainsKey(record.eventIdx))
					m_ResourceTableAll.Add(record.eventIdx, record);

			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("[Tbl_Event_Table] LoadTable:|" + e + "| error while parsing");
		}
	}

	public List<Tbl_Event_Record> GetRecord(int _npcID, System.DateTime _date)
	{
		List<Tbl_Event_Record> returnList = new List<Tbl_Event_Record>();

		if (m_ResourceTableNpc == null)
			return returnList;

		if (m_ResourceTableNpc.ContainsKey(_npcID) == true)
		{
			List<Tbl_Event_Record> listRecord = m_ResourceTableNpc[_npcID];

			foreach (Tbl_Event_Record record in listRecord)
			{
				if (record.CanStartEvent(_date))
					returnList.Add(record);
			}
		}

		return returnList;
	}

	public bool CheckEventViewNpcAppear(int _npcID, System.DateTime _date)
	{
		List<Tbl_Event_Record> listViewNpc = new List<Tbl_Event_Record>();

		if (m_ResourceTableNpc.ContainsKey(_npcID) == true)
		{
			List<Tbl_Event_Record> listRecord = m_ResourceTableNpc[_npcID];

			foreach (Tbl_Event_Record record in listRecord)
			{
				if (record.viewNpc == true)
					listViewNpc.Add(record);
			}

			// not connected Event View Npc
			if (listViewNpc.Count <= 0)
				return false;

			// check Event view npc
			foreach (Tbl_Event_Record eventRecord in listViewNpc)
			{
				if (eventRecord.CanStartEvent(_date) == true)
					return false;
			}

			return true;
		}

		return false;
	}

	public bool CheckEventViewNpc(int _npcID)
	{
		if (m_ResourceTableNpc.ContainsKey(_npcID) == true)
		{
			List<Tbl_Event_Record> listRecord = m_ResourceTableNpc[_npcID];

			foreach (Tbl_Event_Record eventRecord in listRecord)
			{
				if (eventRecord.viewNpc == true)
					return true;
			}
		}
		
		return false;
	}
	
	public Tbl_Event_Record GetRecord(int _eventID)
	{
		if (m_ResourceTableAll.ContainsKey(_eventID))
			return m_ResourceTableAll[_eventID];
		else
			return null;
	}

	public List<Tbl_Event_Record> GetRecordAllCanNotProgress(System.DateTime _date, bool _checkNpcView = false)
	{
		List<Tbl_Event_Record> returnList = new List<Tbl_Event_Record>();

		foreach (Tbl_Event_Record record in m_ResourceTableAll.Values)
		{
			if (!record.CanStartEvent(_date))
			{
				if (_checkNpcView == true)
				{
					if (record.viewNpc == true)
						returnList.Add(record);
				}
				else
					returnList.Add(record);
			}
		}

		return returnList;
	}

	public List<Tbl_Event_Record> GetRecordAll(System.DateTime _date, bool _checkNpcView = false)
	{
		List<Tbl_Event_Record> returnList = new List<Tbl_Event_Record>();

		foreach (List<Tbl_Event_Record> listRecord in m_ResourceTableNpc.Values)
		{
			foreach (Tbl_Event_Record record in listRecord)
			{
				if (record.CanStartEvent(_date))
				{
					if (_checkNpcView == true)
					{
						if (record.viewNpc == true)
							returnList.Add(record);
					}
					else
						returnList.Add(record);
				}
			}
		}

		return returnList;
	}
}
