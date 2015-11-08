using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


public class Tbl_QuestStringRecord : AsTableRecord
{
    QuestStringInfo m_Info; public QuestStringInfo Info { get { return m_Info; } set { m_Info = value; } }

    public Tbl_QuestStringRecord(XmlElement _element)
    {
        m_Info = new QuestStringInfo();
        try
        {
            XmlNode node = (XmlElement)_element;

            int id             = 0;
            string title       = string.Empty;
            string explain     = string.Empty;
            string achievement = string.Empty;

            SetValue(ref id, node, "ID");
            SetValue(ref title, node, "Title");
            SetValue(ref explain, node, "Explain");
            SetValue(ref achievement, node, "Achievement");

            m_Info = new QuestStringInfo(id, title, explain, achievement);
        }
        catch (System.Exception e)
        {
			Debug.LogError( e);
        }
    }
}


public class Tbl_QuestString_Table : AsTableBase
{

    SortedList<int, Tbl_QuestStringRecord> m_ResourceTable = new SortedList<int, Tbl_QuestStringRecord>();
    public Tbl_QuestString_Table(string _path)
    {
        // m_TableType = eTableType.NPC;

        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root = GetXmlRootElement(_path);

            // 파일이 없다
            if (root == null)
                throw new Exception("File is not exist = " + _path);


            XmlNodeList nodes = root.ChildNodes;

            foreach (XmlNode node in nodes)
            {
                Tbl_QuestStringRecord record = new Tbl_QuestStringRecord((XmlElement)node);
                if (m_ResourceTable.ContainsKey(record.Info.ID) == false)
                    m_ResourceTable.Add(record.Info.ID, record);
                else
                    System.Diagnostics.Trace.WriteLine("Tbl_Npc::LoadTable: There are same id(" + record.Info.ID + ") xml node.");
            }
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e);
        }
    }

    public Tbl_QuestStringRecord GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
        {
            return m_ResourceTable[_id];
        }

        System.Diagnostics.Trace.WriteLine("[Tbl_Npc_Table]GetRecord: there is no record. id = " + _id);

        return null;
    }

    public void SetTblQuestStringRecord(QuestStringInfo _info)
    {
        if (m_ResourceTable.ContainsKey(_info.ID))
            m_ResourceTable[_info.ID].Info = _info;
    }

    public Tbl_QuestStringRecord[] GetRecordAll()
    {
        List<Tbl_QuestStringRecord> list = new List<Tbl_QuestStringRecord>();
        foreach (Tbl_QuestStringRecord record in m_ResourceTable.Values)
            list.Add(record);

        return list.ToArray();
    }
}


