using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    public class QuestRecoder
    {
        Dictionary<int, QuestData> dicQuest = new Dictionary<int, QuestData>();
       
        private QuestRecoder()
        {
           
        }

        static QuestRecoder m_Instance;

        public static QuestRecoder Instance 
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new QuestRecoder();
                    return m_Instance;
                }
                else
                    return m_Instance;
            }
        }


        public void Load(string path)
        {
            dicQuest.Clear();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNodeList nodeQuests = doc.SelectNodes("Quests/Quest");

               // nodeQuests[0].ChildNodes[0].RemoveAll();

                foreach (XmlNode quest in nodeQuests)
                {
                    QuestData data = LoadCore(quest.ChildNodes);
                    dicQuest.Add(data.Info.ID, data);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.WriteLine(exception.Message);
            }

        }

        public QuestData[] GetAllQuest()
        {
            List<QuestData> list = new List<QuestData>();

            foreach (QuestData data in dicQuest.Values)
                list.Add(data);

            return list.ToArray();
        }

        public QuestData GetQuestData(int _id)
        {
            if (dicQuest.ContainsKey(_id))
                return dicQuest[_id];
            else
                return null;
        }

        QuestData LoadCore(XmlNodeList nodeList)
        {
            return QuestData.GetQuestDataFromXml(nodeList);

        }

    }
}
