using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    public class QuestCondition
    {
        private List<ConditionBase>            listCondition = null;

        public void AddCondition(ConditionBase _condition)
        {
            listCondition.Add(_condition);
        }
         
        public List<T> GetCondition<T>() where T : class
        {
            List<T> list = new List<T>();

            foreach (ConditionBase condition in listCondition)
            {

                if (condition.GetType() == typeof(T))
                    list.Add(condition as T);
            }
            return list;
        }

        public int GetconditionCount(ConditionBase _condition)
        {
            int count = 0;
            foreach (ConditionBase condition in listCondition)
            {
                if (condition.GetType() == _condition.GetType())
                    count++;
            }

            return count;
        }

        public List<ConditionBase> GetConditions()
        {
            return listCondition;
        }

        public void Reset()
        {
            listCondition.Clear();
        }

        public QuestCondition()
        {
            listCondition = new List<ConditionBase>();
        }

        public bool HaveConditionItem(int _itemID)
        {
            List<ConditionItem> list = GetCondition<ConditionItem>();

            foreach (ConditionItem item in list)
            {
                if (item.ItemID == _itemID)
                    return true;
            }

            return false;
        }

        public bool CanAccept()
        {
            foreach (ConditionBase condition in listCondition)
            {
                if (condition.CheckAccept() == false)
                    return false;
            }

            return true;
        }

        public static void LoadFromXml(QuestCondition condition, XmlNode node)
        {
            string name = node.Attributes["Type"].Value;

            switch (name)
            {

                case ConditionTypeString.Level:
                    condition.AddCondition(new ConditionLevel(Convert.ToInt32(node.Attributes["Min"].Value),
                                                              Convert.ToInt32(node.Attributes["Max"].Value)));
                    break;

                case ConditionTypeString.ItemHave:
                    {
                        ConditionItem item = new ConditionItem();
                        item.ItemID = Convert.ToInt32(node.Attributes["ID"].Value);
                        item.ItemCount = Convert.ToInt32(node.Attributes["Count"].Value);
                        item.Type = ItemConditionType.HAVE;
                        condition.AddCondition(item);
                    }
                    break;

                case ConditionTypeString.ItemUse:
                    {
                        ConditionItem item = new ConditionItem();
                        item.ItemID = Convert.ToInt32(node.Attributes["ID"].Value);
                        item.ItemCount = Convert.ToInt32(node.Attributes["Count"].Value);
                        item.Type = ItemConditionType.USE;
                        condition.AddCondition(item);
                    }
                    break;

                case ConditionTypeString.Class:
                    condition.AddCondition(new ConditionClass((eCLASS)Convert.ToInt32(node.Attributes["Class"].Value)));
                    break;

                case ConditionTypeString.Race:
                    condition.AddCondition(new ConditionRace((eRACE)Convert.ToInt32(node.Attributes["Race"].Value)));
                    break;

                case ConditionTypeString.SkillHave:
                    {
                        ConditionSkill skill = new ConditionSkill();
                        skill.SkillID = Convert.ToInt32(node.Attributes["ID"].Value);
                        skill.Type = SkillConditionType.HAVE;
                        condition.AddCondition(skill);
                    }
                    break;

                case ConditionTypeString.SkillUse:
                    {
                        ConditionSkill skill = new ConditionSkill();
                        skill.SkillID = Convert.ToInt32(node.Attributes["ID"].Value);
                        skill.Type = SkillConditionType.USE;
                        condition.AddCondition(skill);
                    }
                    break;

                case ConditionTypeString.MoveMap:
                    ConditionMap map = new ConditionMap();
                    map.MapID = Convert.ToInt32(node.Attributes["ID"].Value);
                    map.LocationX = Convert.ToInt32(node.Attributes["X"].Value);
                    map.LocationZ = Convert.ToInt32(node.Attributes["Z"].Value);
                    map.LocationRadius = Convert.ToInt32(node.Attributes["Radius"].Value);
                    condition.AddCondition(map);
                    break;

                case ConditionTypeString.ProductionMastery:
                    ConditionProductionMastery productionSkill = new ConditionProductionMastery();
                    productionSkill.ProductionMasteryType = (eITEM_PRODUCT_TECHNIQUE_TYPE)Convert.ToInt32(node.Attributes["ProductionMasteryType"].Value);
                    productionSkill.ProductionMastery     = Convert.ToInt32(node.Attributes["Mastery"].Value);
                    condition.AddCondition(productionSkill);
                    break;

                case ConditionTypeString.QuestPass:
                    ConditionQuestPass questPass = new ConditionQuestPass();
                    questPass.QuestID = Convert.ToInt32(node.Attributes["ID"].Value);

                    if (node.Attributes["QuestPassType"] != null)
                        questPass.QuestPassType = (QuestPassType)Convert.ToInt32(node.Attributes["QuestPassType"].Value);
                    else
                        questPass.QuestPassType = QuestPassType.AND;

                    condition.AddCondition(questPass);
                    break;

                case ConditionTypeString.Designation:
                    ConditionDesignation designation = new ConditionDesignation();
                    designation.type = (DesignationType)Enum.Parse(typeof(DesignationType), node.Attributes["DesignationType"].Value, true);
                    designation.data = Convert.ToInt32(node.Attributes["Data"].Value);
                    condition.AddCondition(designation);
                    break;

                case ConditionTypeString.Gold:
                    ConditionGold gold = new ConditionGold(Convert.ToUInt64(node.Attributes["Value"].Value));
                    condition.AddCondition(gold);
                    break;

                case ConditionTypeString.Friendship:
                    ConditionFriendship friendship = new ConditionFriendship();
                    friendship.count = Convert.ToInt32(node.Attributes["Count"].Value);
                    friendship.point = Convert.ToInt32(node.Attributes["Point"].Value);
                    condition.AddCondition(friendship);
                    break;

            }
        }
    }
}
