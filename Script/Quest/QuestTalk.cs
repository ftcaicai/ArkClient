using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    public class QuestTalk
    {
        private static char splitCharacter = '\\';
        public  int ID;
        private Dictionary<QuestProgressState, string> dicQuestTalk;
        private Dictionary<QuestProgressState, List<QuestTalkInfo>> dicQuestTalkInfo;
     
        public QuestTalk()
        {
            dicQuestTalk     = new Dictionary<QuestProgressState, string>();
            dicQuestTalkInfo = new Dictionary<QuestProgressState, List<QuestTalkInfo>>();
        }

        public void AddTalk(QuestProgressState _talkState, string _talk)
        {
            if (!dicQuestTalk.ContainsKey(_talkState))
            {
                dicQuestTalk.Add(_talkState, _talk);
                dicQuestTalkInfo.Add(_talkState, MakeTalkInfo(_talk));
            }
        }

        public Dictionary<QuestProgressState, List<QuestTalkInfo>> GetTalkStringAll()
        {
            return dicQuestTalkInfo;
        }

        public List<QuestTalkInfo> GetTalkInfo(QuestProgressState _talkState)
        {
            if (dicQuestTalkInfo.ContainsKey(_talkState))
                return dicQuestTalkInfo[_talkState];
            else
                return null;
        }

        public string GetTalkString(QuestProgressState _talkState)
        {
            if (dicQuestTalk.ContainsKey(_talkState))
                return dicQuestTalk[_talkState];
            else
                return string.Empty;
        }
        

        //로드 부분
		public static void LoadFromXml(QuestTalk _talk, XmlNode _node)
		{
			_talk.ID = Convert.ToInt32(_node["ID"].InnerText);
			_talk.AddTalk(QuestProgressState.QUEST_PROGRESS_NOTHING, GetInnerText(_node, "Start"));
			_talk.AddTalk(QuestProgressState.QUEST_PROGRESS_IN, GetInnerText(_node, "Progress"));
			_talk.AddTalk(QuestProgressState.QUEST_PROGRESS_CLEAR, GetInnerText(_node, "Clear"));
		}

		public static string GetInnerText(XmlNode _node, string _key)
		{
			if (_node[_key] == null)
			{
				Debug.LogError(_node["ID"].InnerText + "'s "+ _key + "is null");
				return string.Empty;
			}
			else
				return _node[_key].InnerText;
		}


        // string을 talkinfo로 나눈다.
        public static List<QuestTalkInfo> MakeTalkInfo(string _content)
        {
            List<QuestTalkInfo> listTalkInfo = new List<QuestTalkInfo>();
            if (_content != string.Empty)
                listTalkInfo = DivideTalk(_content);

            return listTalkInfo;

        }

        public static List<QuestTalkInfo> DivideTalk(string _text)
        {
            List<QuestTalkInfo> listTalkInfo = new List<QuestTalkInfo>();
            string[] talks = _text.Split(splitCharacter);

            foreach (string talkSplit in talks)
            {
                string sz = talkSplit.Trim();
                int n = -1;

                if (sz == string.Empty)
                    continue;

                if (sz[0] == '[' && sz[2] == ']')
                {
                    n = System.Convert.ToInt32(sz[1].ToString());
                    sz = sz.Remove(0, 3);
                }

                listTalkInfo.Add(new QuestTalkInfo(n, sz));
            }

            listTalkInfo.Add(new QuestTalkInfo(0, ""));

            return listTalkInfo;

        }
    }

}
