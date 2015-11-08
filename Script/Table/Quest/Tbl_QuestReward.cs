using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ArkSphereQuestTool;


    public class Tbl_QuestReward_Record : AsTableRecord
    {
        QuestReward m_Reward; public QuestReward Reward { get { return m_Reward; } }

        public Tbl_QuestReward_Record(XmlNode mainNode)
        {
            try
            {
                m_Reward = new QuestReward();

                foreach (XmlNode node in mainNode.ChildNodes)
                {
                    QuestReward.LoadFromXml(m_Reward, node);
                }
            }

            catch (System.Exception e)
            {

                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }


    public class Tbl_QuestReward_Table : AsTableBase
    {

        SortedList<int, Tbl_QuestReward_Record> m_ResourceTable = new SortedList<int, Tbl_QuestReward_Record>();
        public Tbl_QuestReward_Table(string _path)
        {
            LoadTable(_path);
        }

        public override void LoadTable(string _path)
        {
            try
            {
                XmlElement root = GetXmlRootElement(_path);

                XmlNodeList nodes = root.ChildNodes;

                foreach (XmlNode node in nodes)
                {
                    Tbl_QuestReward_Record record = new Tbl_QuestReward_Record(node);
                    if (m_ResourceTable.ContainsKey(record.Reward.ID) == false)
                        m_ResourceTable.Add(record.Reward.ID, record);
                    else
                        System.Diagnostics.Trace.WriteLine("Tbl_Npc::LoadTable: There are same id(" + record.Reward.ID + ") xml node.");
                }

                Tbl_QuestReward_Record[] datas = GetRecordAll();
                foreach (Tbl_QuestReward_Record data in datas)
                {
                    System.Diagnostics.Trace.WriteLine(data.Reward.ID);

                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
            }
        }

        public Tbl_QuestReward_Record GetRecord(int _id)
        {
            if (m_ResourceTable.ContainsKey(_id) == true)
            {
                return m_ResourceTable[_id];
            }
            return null;
        }

        public Tbl_QuestReward_Record[] GetRecordAll()
        {
            List<Tbl_QuestReward_Record> list = new List<Tbl_QuestReward_Record>();
            foreach (Tbl_QuestReward_Record record in m_ResourceTable.Values)
                list.Add(record);

            return list.ToArray();
        }
    }

