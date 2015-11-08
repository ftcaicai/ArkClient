using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    [System.Serializable]
    public class QuestData
    {
        public  QuestProgressState   NowQuestProgressState   { get; set; }
        public  QuestInfo            Info                    { get; set; }
        public  QuestCondition       Condition               { get; set; }
        public  QuestAchievement     Achievement             { get; set; }
        public  QuestPreparation     Preparation             { get; set; }
        public  QuestTalk            Talk                    { get; set; }
        Dictionary<QuestProgressState, List<QuestTalkInfo>> dicQuestTalks = new Dictionary<QuestProgressState, List<QuestTalkInfo>>();

        public QuestData()
        {
            Reset();
        }

        public void Reset()
        {
            NowQuestProgressState = QuestProgressState.QUEST_PROGRESS_NOTHING;
            Info                  = new QuestInfo();
            Condition             = new QuestCondition();
            Preparation           = new QuestPreparation();
            Talk                  = new QuestTalk();
            Achievement           = new QuestAchievement();
            dicQuestTalks.Clear();
        }

        public void SetQuestTalks(Dictionary<QuestProgressState, List<QuestTalkInfo>> _questTalkDics)
        {
            dicQuestTalks = _questTalkDics;
        }

        public void IncreaseQuestCompleteCount()
        {
            // skip infinite
            if (Info.Repeat == byte.MaxValue)
                return;

            Info.RepeatCurrent++;

            if (Info.RepeatCurrent > Info.Repeat)
                Info.RepeatCurrent = Info.Repeat;
        }

        public bool CanRepeat()
        {
            // infinite
            if (Info.Repeat == byte.MaxValue)
                return true;

            if (Info.Repeat > Info.RepeatCurrent)
                return true;
            else
                return false;
        }

        public string GetRepeatString()
        {
            if (Info.Repeat == byte.MaxValue)
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append(AsTableManager.Instance.GetTbl_String(1995));
                sb.Append("]");
                return sb.ToString();
            }
           
            if (Info.Repeat > 1)
            {
                StringBuilder sb = new StringBuilder("[");
                sb.Append((Info.RepeatCurrent + 1).ToString());
                sb.Append("/");
                sb.Append(Info.Repeat.ToString());
                sb.Append("]");
                return sb.ToString();
            }
            else
                return string.Empty;
        }

        public List<QuestTalkInfo> GetNowQuestTalk()
        {
            QuestProgressState nowProgress = NowQuestProgressState;

            if (nowProgress == QuestProgressState.QUEST_PROGRESS_IMMEDIATELY_CLEAR)
                nowProgress = QuestProgressState.QUEST_PROGRESS_CLEAR;

            if (dicQuestTalks.ContainsKey(nowProgress))
                return dicQuestTalks[nowProgress];

            else
                return new List<QuestTalkInfo>();
        }

        /// <summary>
        /// 노드 리스트에서 QuestData를 얻는다.
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public static QuestData GetQuestDataFromXml(XmlNodeList nodeList)
        {
            QuestData data = new QuestData();
            try
            {
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "QuestInfo")
                        QuestInfo.LoadFromXml(data.Info, node);
                    else if (node.Name == "Condition")
                        QuestCondition.LoadFromXml(data.Condition, node);
                    else if (node.Name == "Prepare")
                        QuestPreparation.LoadFromXml(data.Preparation, node);
                    else if (node.Name == "Achievement")
                        QuestAchievement.LoadFromXml(data.Info.ID, data.Achievement, node);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                System.Diagnostics.Trace.WriteLine(e.Source);
            }

            return data;
        }


        /// <summary>
        /// xml 파일로 부터 QuestData정보를 넘겨준다.
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static  List<QuestData> GetQuestDatasFromXmlFile(string _path)
        {
            List<QuestData> listQuestData = new List<QuestData>();

            XmlDocument doc = new XmlDocument();

            doc.Load(_path);

            XmlNodeList nodeQuests = doc.SelectNodes("Quests/Quest");

            foreach (XmlNode quest in nodeQuests)
            {
                QuestData data = GetQuestDataFromXml(quest.ChildNodes);
                listQuestData.Add(data);
            }

            return listQuestData;
        }
    }
}
