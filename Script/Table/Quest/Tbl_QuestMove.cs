using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class Tbl_QuestMove_Record 
{
	public int questID;
	public int startMapID;
	public int endMapID;
	public Vector3 startPos;
	public Vector3 endPos;

	public override string ToString()
	{
		return questID.ToString() + " = {" + startMapID + "}[" + startPos.ToString() + "], {" + endMapID + "} [" + endPos.ToString() + "]"; 
	}
}


public class Tbl_QuestMove : AsTableBase
{
	public Dictionary<int, Tbl_QuestMove_Record> dicRecord = new Dictionary<int, Tbl_QuestMove_Record>();

	public Tbl_QuestMove(string _path)
	{
		LoadTable(_path);
	}

	public override void LoadTable(string _path)
	{
		try
		{
			Tbl_QuestMove_Record record = null;

			XmlElement root = GetXmlRootElement(_path);

			XmlNodeList nodes = root.SelectNodes("QuestMove");

			foreach (XmlNode node in nodes)
			{
				record = new Tbl_QuestMove_Record()
				{
					questID		= int.Parse(node.Attributes["Index"].Value),

					startMapID	= int.Parse(node.Attributes["St_Map"].Value),

					startPos	= new Vector3()
					{
						x = float.Parse(node.Attributes["St_X"].Value),
						y = float.Parse(node.Attributes["St_Y"].Value),
						z = float.Parse(node.Attributes["St_Z"].Value)
					},

					endMapID	= int.Parse(node.Attributes["End_Map"].Value),

					endPos		= new Vector3()
					{
						x = float.Parse(node.Attributes["End_X"].Value),
						y = float.Parse(node.Attributes["End_Y"].Value),
						z = float.Parse(node.Attributes["End_Z"].Value)
					}
				};										 


				if (dicRecord != null)
					if (!dicRecord.ContainsKey(record.questID))
						dicRecord.Add(record.questID, record);
			}
		}
		catch (System.Exception e)
		{
			System.Diagnostics.Trace.WriteLine(e);
		}
	}

	public Tbl_QuestMove_Record Get_Tbl_QuestMove_Record(int _questID)
	{
		if (dicRecord.ContainsKey(_questID))
			return dicRecord[_questID];
		else
			return null;
	}
}


