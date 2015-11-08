using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using ArkSphereQuestTool;


public class Tbl_QuestTalk_Record : AsTableRecord
{

    QuestTalk m_QuestTalk; public QuestTalk questTalk { get { return m_QuestTalk; } }

    public Tbl_QuestTalk_Record(XmlElement _element)
    {
        try
        {
            XmlNode node = (XmlElement)_element;

            m_QuestTalk = new QuestTalk();

            QuestTalk.LoadFromXml(m_QuestTalk, node);
        }
        catch (System.Exception e)
        {
			Debug.LogError(e);//$yde

        }
    }
}


public class Tbl_QuestTalk_Table : AsTableBase
{

    SortedList<int, Tbl_QuestTalk_Record> m_ResourceTable = new SortedList<int, Tbl_QuestTalk_Record>();
    public Tbl_QuestTalk_Table(string _path)
    {
        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root = GetXmlRootElement(_path);


            // 파일이 없다
            if (root == null)
            {
                //  새로 생성
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
                XmlElement rootElement = document.CreateElement("QuestTalks");
                rootElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                document.AppendChild(rootElement);
                document.Save(_path);

                // 다시 로드
                root = GetXmlRootElement(_path);
            }

            XmlNodeList nodes = root.ChildNodes;

            foreach (XmlNode node in nodes)
            {
                Tbl_QuestTalk_Record record = new Tbl_QuestTalk_Record((XmlElement)node);
                if (m_ResourceTable.ContainsKey(record.questTalk.ID) == false)
                    m_ResourceTable.Add(record.questTalk.ID, record);
                else
                    System.Diagnostics.Trace.WriteLine("Tbl_Npc::LoadTable: There are same id(" + record.questTalk.ID + ") xml node.");
            }
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e);
        }
    }

    public Tbl_QuestTalk_Record GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
            return m_ResourceTable[_id];
 
        System.Diagnostics.Trace.WriteLine("[Tbl_Npc_Table]GetRecord: there is no record. id = " + _id);

        return null;
    }

    public Tbl_QuestTalk_Record[] GetRecordAll()
    {
        List<Tbl_QuestTalk_Record> list = new List<Tbl_QuestTalk_Record>();
        foreach (Tbl_QuestTalk_Record record in m_ResourceTable.Values)
            list.Add(record);

        return list.ToArray();
    }
}

