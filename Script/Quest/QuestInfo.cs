using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
	public class QuestInfo
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Explain { get; set; }
		public string Achievement { get; set; }
		public byte Repeat { get; set; }
		public byte RepeatCurrent { get; set; }
		public int QuestSuggestNpcID { get; set; }
		public int QuestCompleteNpcID { get; set; }
		public int QuestMapID { get; set; }
		public QuestType QuestType { get; set; }
		public DailyQuestLevel DailyLevel { get; set; }
		public int NpcDailyGroupID { get; set; }
		public QuestClearType QuestClearType { get; set; }
		public int ClearPrice { get; set; }
		public int QuestLocationNameID { get; set; }
		public QuestGuideDirection QuestGuideDirect { get; set; }
		public bool PartyQuest { get; set; }



		public QuestInfo()
		{
			ID = 0;
			Name = String.Empty;
			Explain = String.Empty;
			Achievement = String.Empty;
			RepeatCurrent = 0;
			QuestSuggestNpcID = 0;
			QuestCompleteNpcID = 0;
			QuestMapID = 0;
			QuestType = QuestType.QUEST_NONE;
			DailyLevel = DailyQuestLevel.EASY;
			QuestClearType = QuestClearType.NONE;
			ClearPrice = 0;
			QuestLocationNameID = 66000;
			QuestGuideDirect = QuestGuideDirection.NONE;
			PartyQuest = false;
		}

		public string GetQuestTypeString()
		{
			string szReturn = string.Empty;

			switch (QuestType)
			{
				case QuestType.QUEST_MAIN:
					szReturn = AsTableManager.Instance.GetTbl_String(2127);
					break;
				case QuestType.QUEST_FIELD:
					szReturn = AsTableManager.Instance.GetTbl_String(2128);
					break;
				case QuestType.QUEST_DAILY:
					szReturn = AsTableManager.Instance.GetTbl_String(2129);
					break;
				case QuestType.QUEST_BOSS:
					szReturn = AsTableManager.Instance.GetTbl_String(2130);
					break;
				case QuestType.QUEST_PVP:
					szReturn = AsTableManager.Instance.GetTbl_String(2131);
					break;
			}

			return szReturn;
		}

		public static void LoadFromXml(QuestInfo info, XmlNode node)
		{
			foreach (XmlAttribute attribute in node.Attributes)
			{
				switch (attribute.Name)
				{
					case "ID":
						info.ID = Convert.ToInt32(attribute.Value);
						break;

					case "Type":
						info.QuestType = (QuestType)Convert.ToInt32(attribute.Value);
						break;

					case "DailyLevel":
						info.DailyLevel = (DailyQuestLevel)Convert.ToInt32(attribute.Value);
						break;

					case "Repeat":
						info.Repeat = Convert.ToByte(attribute.Value);
						break;

					case "SuggestNPC":
						info.QuestSuggestNpcID = Convert.ToInt32(attribute.Value);
						break;

					case "CompleteNPC":
						info.QuestCompleteNpcID = Convert.ToInt32(attribute.Value);
						break;

					case "MapID":
						info.QuestMapID = Convert.ToInt32(attribute.Value);
						break;

					case "ClearType":
						info.QuestClearType = (QuestClearType)Convert.ToInt32(attribute.Value);
						break;

					case "ClearPrice":
						info.ClearPrice = Convert.ToInt32(attribute.Value);
						break;

					case "QuestLocationNameID":
						info.QuestLocationNameID = Convert.ToInt32(attribute.Value);
						break;

					case "QuestGuideDirection":
						info.QuestGuideDirect = (QuestGuideDirection)Enum.Parse(typeof(QuestGuideDirection), attribute.Value, true);
						break;

					case "Party":
						info.PartyQuest = bool.Parse(attribute.Value);
						break;

					case "NpcDailyGroupID":
						info.NpcDailyGroupID = Int32.Parse(attribute.Value);
						break;
				}
			}
		}
	}
}
