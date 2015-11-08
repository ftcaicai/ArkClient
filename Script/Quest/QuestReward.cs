using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    public class QuestReward
    {
        public int ID { get; set; }
        public int ExperiencePoint { get; set; }
        public int Money { get; set; }
        public int Miracle { get; set; }
        public List<RewardItem> Items { get; set; }
        public List<RewardItem> ItemsSelect { get; set; }
        public List<RewardSkill> skill { get; set; }
        public List<RewardDesignation> designation { get; set; }
        public int ItemClassID { get; set; }

        public QuestReward()
        {
            Reset();
        }

        public void Reset()
        {
            ID = -1;
            ExperiencePoint = 0;
            Money       = 0;
            Items       = new List<RewardItem>();
            ItemsSelect = new List<RewardItem>();
            skill       = new List<RewardSkill>();
            designation = new List<RewardDesignation>();
            ItemClassID = 0;
        }

        public static void LoadFromXml(QuestReward _reward, XmlNode _node)
        {
            string name = _node.Attributes["Type"].Value;

            switch (name)
            {
                case RewardTypeString.ID:
                    _reward.ID = Convert.ToInt32(_node.Attributes["ID"].Value);
                    break;
                case RewardTypeString.Exp:
                    _reward.ExperiencePoint = Convert.ToInt32(_node.Attributes["Exp"].Value);
                    break;

                case RewardTypeString.Money:
                    _reward.Money = Convert.ToInt32(_node.Attributes["Ark"].Value);
                    break;

                case RewardTypeString.Miracle:
                    _reward.Miracle = Convert.ToInt32(_node.Attributes["Miracle"].Value);
                    break;

                case RewardTypeString.Item:
                    {
                        RewardItem item = new RewardItem();

                        if (_node.Attributes["Class"] != null)
                            item.Class = (eCLASS)Enum.Parse(typeof(eCLASS), _node.Attributes["Class"].Value, true);
                        else
                            item.Class = eCLASS.NONE;

                        item.ID = Convert.ToInt32(_node.Attributes["ItemID"].Value);
                        item.Count = Convert.ToInt32(_node.Attributes["ItemCount"].Value);
                        _reward.Items.Add(item);
                    }
                    break;

                case RewardTypeString.ItemSelect:
                    {
                        RewardItem item = new RewardItem();
                        item.Class = eCLASS.All;
                        item.ID    = Convert.ToInt32(_node.Attributes["ItemID"].Value);
                        item.Count = Convert.ToInt32(_node.Attributes["ItemCount"].Value);
                        _reward.ItemsSelect.Add(item);
                    }
                    break;

                case RewardTypeString.Skill:
                    {
                        RewardSkill skill = new RewardSkill();
                        skill.ID = Convert.ToInt32(_node.Attributes["ID"].Value);
                        skill.Lv = Convert.ToInt32(_node.Attributes["Lv"].Value);
                        _reward.skill.Add(skill);
                    }
                    break;

                case RewardTypeString.Designaion:
                    {
                        RewardDesignation designaion = new RewardDesignation();
                        designaion.designationID = Convert.ToInt32(_node.Attributes["SubTitle"].Value);
                        _reward.designation.Add(designaion);
                    }
                    break;
            }

        }

        public static void LoadIDFromXml(QuestReward _reward, XmlNode _node)
        {
            _reward.ID = Convert.ToInt32(_node.Attributes["ID"].Value);
        }
    }
}

