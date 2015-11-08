using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ArkSphereQuestTool
{
    public class QuestPreparation
    {
        public List<PrepareItem>                QuestItemList            { get; set; }
        public List<PrepareMonster>             MonsterList              { get; set; }
        public List<PrepareCollection>          CollectionList           { get; set; }
        public List<PrepareBuff>                BuffList                 { get; set; }
        public PrepareTransform                 Transformation           { get; set; }
        public PrepareMoveMap                   MoveMap                  { get; set; }
        public PrepareCharacterSize             CharacterSize            { get; set; }
        public List<PrepareItemDropMonster>     ItemDropMonsterList      { get; set; }
        public List<PrepareItemDropMonsterKind> ItemDromMonsterKindList  { get; set; }
        public List<PrepareItemDropCollection>  CollectionDropList       { get; set; }
		public List<PrepareOpenUI>              OpenUIList               { get; set; }

        public QuestPreparation()
        {
            QuestItemList  = new List<PrepareItem>();
            MonsterList    = new List<PrepareMonster>();
            CollectionList = new List<PrepareCollection>();
            BuffList       = new List<PrepareBuff>();
            Transformation = new PrepareTransform();
            MoveMap        = new PrepareMoveMap();
            CharacterSize  = new PrepareCharacterSize();
            ItemDropMonsterList     = new List<PrepareItemDropMonster>();
            CollectionDropList      = new List<PrepareItemDropCollection>();
            ItemDromMonsterKindList = new List<PrepareItemDropMonsterKind>();
			OpenUIList              = new List<PrepareOpenUI>();

        }

        public void Reset()
        {
             QuestItemList.Clear();      
             MonsterList.Clear();   
             CollectionList.Clear();
             BuffList.Clear();
             ItemDromMonsterKindList.Clear();
             Transformation = new PrepareTransform();
             MoveMap        = new PrepareMoveMap();       
             CharacterSize  = new PrepareCharacterSize(); 
             ItemDropMonsterList.Clear();
             CollectionDropList.Clear();
			 OpenUIList.Clear();
        }

        public static void LoadFromXml(QuestPreparation prepare, XmlNode node)
        {
            //XmlAttribute[] attributes = node.Attributes.Cast<XmlAttribute>().ToArray();

            string name = node.Attributes["Type"].Value;

            try {
                switch (name)
                {
                    case PrepareTypeString.MoveTo:
                        prepare.MoveMap              = new PrepareMoveMap();
                        prepare.MoveMap.MapID        = Convert.ToInt32(node.Attributes["ID"].Value);
                        prepare.MoveMap.MapLocationX = Convert.ToSingle(node.Attributes["X"].Value);
                        prepare.MoveMap.MapLocationZ = Convert.ToSingle(node.Attributes["Z"].Value);
                        break;

                    case PrepareTypeString.MonsterGenerate:
                        {
                            PrepareMonster monster = new PrepareMonster();
                            foreach(XmlAttribute attribute in node.Attributes)
                            {
                                if (attribute.Name == "ID")
                                    monster.MonsterID      = Convert.ToInt32(node.Attributes["ID"].Value);
                                else if (attribute.Name == "Count")
                                    monster.Count          = Convert.ToInt32(node.Attributes["Count"].Value);
                                else if (attribute.Name == "Radius")
                                    monster.Radius         = Convert.ToInt32(node.Attributes["Radius"].Value);
                                else if (attribute.Name == "DelayTime")
                                    monster.DelayTime      = Convert.ToInt32(node.Attributes["DelayTime"].Value);
                                else if (attribute.Name == "LifeTime")
                                    monster.LifeTime       = Convert.ToInt32(node.Attributes["LifeTime"].Value);
                            }
                            prepare.MonsterList.Add(monster);
                        }
                        break;

                    case PrepareTypeString.QuestItem:
                        PrepareItem item = new PrepareItem();
                        item.ItemID = Convert.ToInt32(node.Attributes["ID"].Value);
                        item.ItemCount = Convert.ToInt32(node.Attributes["Count"].Value);
                        //item.ItemGetBack = Convert.ToInt32(node.Attributes["GetBack"].Value) == 1 ? true : false;
                        prepare.QuestItemList.Add(item);
                        break;

                    case PrepareTypeString.MonsterItemDrop:
                        PrepareItemDropMonster dropMonster = new PrepareItemDropMonster();
                        dropMonster.TargetMonsterID = Convert.ToInt32(node.Attributes["MonsterID"].Value);
                        dropMonster.ItemID          = Convert.ToInt32(node.Attributes["ItemID"].Value);
                        dropMonster.ItemDropMin     = Convert.ToInt32(node.Attributes["ItemDropMin"].Value);
                        dropMonster.ItemDropMax     = Convert.ToInt32(node.Attributes["ItemDropMax"].Value);
                        dropMonster.ItemDropRate    = Convert.ToInt32(node.Attributes["ItemDropRate"].Value);
                        dropMonster.Champion        = Convert.ToBoolean(node.Attributes["Champion"].Value);
                        prepare.ItemDropMonsterList.Add(dropMonster);
                        break;

                    case PrepareTypeString.MonsterKindItemDrop:
                        PrepareItemDropMonsterKind dropMonsterKind = new PrepareItemDropMonsterKind();
                        dropMonsterKind.TargetMonsterKindID = Convert.ToInt32(node.Attributes["MonsterKindID"].Value);
                        dropMonsterKind.ItemID              = Convert.ToInt32(node.Attributes["ItemID"].Value);
                        dropMonsterKind.ItemDropMin         = Convert.ToInt32(node.Attributes["ItemDropMin"].Value);
                        dropMonsterKind.ItemDropMax         = Convert.ToInt32(node.Attributes["ItemDropMax"].Value);
                        dropMonsterKind.ItemDropRate        = Convert.ToInt32(node.Attributes["ItemDropRate"].Value);
                        dropMonsterKind.Champion            = Convert.ToBoolean(node.Attributes["Champion"].Value);
                        prepare.ItemDromMonsterKindList.Add(dropMonsterKind);
                        break;

					case PrepareTypeString.OpenUI:
						PrepareOpenUI openUI = new PrepareOpenUI();
						openUI.UIType = (PrepareOpenUIType)Enum.Parse(typeof(PrepareOpenUIType), node.Attributes["UIType"].Value, true);
						prepare.OpenUIList.Add(openUI);
						break;

                    case PrepareTypeString.CollectionItemDrop:
                        break;

                    case PrepareTypeString.CollectionSkill:
                        break;

                    case PrepareTypeString.ProductionSkill:
                        break;

                    case "Buff":
                        break;

                    case PrepareTypeString.Size:
                        prepare.CharacterSize.SizeRate = Convert.ToInt32(node.Attributes["Rate"].Value);
                        prepare.CharacterSize.BuffID   = Convert.ToInt32(node.Attributes["BuffId"].Value);
                        prepare.CharacterSize.BuffTime = Convert.ToInt32(node.Attributes["BuffTime"].Value);
                        break;

                    case PrepareTypeString.Transform:
                        prepare.Transformation.TransformID       = Convert.ToInt32(node.Attributes["TransID"].Value);
                        prepare.Transformation.TransformBuffId   = Convert.ToInt32(node.Attributes["BuffID"].Value);
                        prepare.Transformation.TransformBuffTime = Convert.ToInt32(node.Attributes["BuffTime"].Value);
                        break;
                }
            }
            catch(Exception e)
            {

                Debug.LogWarning(e.ToString());
            }
        }
    }

}
