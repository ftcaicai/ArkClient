using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class AsCharacterCreateGenderData : AsTableRecord
{
	public eGENDER gender = eGENDER.eGENDER_NOTHING;
	public ArrayList hairTypes = new ArrayList();
	public ArrayList hairColors = new ArrayList();
	public ArrayList bodyColors = new ArrayList();
	public ArrayList pointColors = new ArrayList();
	public ArrayList gloveColors = new ArrayList();
	
	public AsCharacterCreateGenderData( XmlElement _element)
	{
		XmlNode node = (XmlElement)_element;
		
		// <Type>
		SetValue( ref gender, node, "Type");
		// <Property>
		XmlNode propertyNode = node[ "Property"];
		// <HairType>
		XmlNode hairTypeNode = propertyNode[ "HairType"];
		XmlNodeList hairTypeList = hairTypeNode.ChildNodes;
		foreach( XmlNode hairType in hairTypeList)
		{
			// <Type>
			int hairId = int.MaxValue;
			SetValue( ref hairId, hairType);
			hairTypes.Add( hairId);
		}
		// <HairColor>
		XmlNode hairColorNode = propertyNode[ "HairColor"];
		XmlNodeList hairColorList = hairColorNode.ChildNodes;
		foreach( XmlNode hairColor in hairColorList)
		{
			// <Color>
			int colorId = int.MaxValue;
			SetValue( ref colorId, hairColor);
			hairColors.Add( colorId);
		}
		// <BodyColor>
		XmlNode bodyColorNode = propertyNode[ "BodyColor"];
		XmlNodeList bodyColorList = bodyColorNode.ChildNodes;
		foreach( XmlNode bodyColor in bodyColorList)
		{
			// <Color>
			int colorId = int.MaxValue;
			SetValue( ref colorId, bodyColor);
			bodyColors.Add( colorId);
		}
		// <PointColor>
		XmlNode pointColorNode = propertyNode[ "PointColor"];
		XmlNodeList pointColorList = pointColorNode.ChildNodes;
		foreach( XmlNode pointColor in pointColorList)
		{
			// <Color>
			int colorId = int.MaxValue;
			SetValue( ref colorId, pointColor);
			pointColors.Add( colorId);
		}
		// <GloveColor>
		XmlNode gloveColorNode = propertyNode[ "GloveColor"];
		XmlNodeList gloveColorList = gloveColorNode.ChildNodes;
		foreach( XmlNode gloveColor in gloveColorList)
		{
			// <Color>
			int colorId = int.MaxValue;
			SetValue( ref colorId, gloveColor);
			gloveColors.Add( colorId);
		}
	}
};

public class AsCharacterCreateItem : AsTableRecord
{
	public int id;
	public int count;
	
	public AsCharacterCreateItem( XmlElement _element)
	{
		XmlNode node = (XmlElement)_element;

		// <Id>
		SetValue( ref id, node, "Id");
		// <Count>
		SetValue( ref count, node, "Count");
	}
};

public class AsCharacterCreateCommonProperty : AsTableRecord
{
	public int weapon = 0;
	public ArrayList defaultItems = new ArrayList();
	public ArrayList defaultSkills = new ArrayList();
	public ArrayList itemQuickSlots = new ArrayList();
	public ArrayList skillQuickSlots = new ArrayList();
	
	public AsCharacterCreateCommonProperty( XmlElement _element)
	{
		XmlNode node = (XmlElement)_element;
		
		// <Weapon>
		XmlNode weaponNode = node[ "Weapon"];
		SetValue( ref weapon, weaponNode);
		
		// <DefaultItem>
		XmlNode defaultItem = node[ "DefaultItem"];
		XmlNodeList itemList = defaultItem.ChildNodes;
		foreach( XmlNode itemNode in itemList)
		{
			// <Item>
			AsCharacterCreateItem item = new AsCharacterCreateItem( itemNode as XmlElement);
			defaultItems.Add( item);
		}
		
		// <DefaultSkill>
		XmlNode defaultSkill = node[ "DefaultSkill"];
		XmlNodeList skillList = defaultSkill.ChildNodes;
		foreach( XmlNode skillNode in skillList)
		{
			// <Skill>
			int skillId = int.MaxValue;
			SetValue( ref skillId, skillNode);
			defaultSkills.Add( skillId);
		}
		
		// <ItemQuickSlot>
		XmlNode itemQuickSlot = node[ "ItemQuickSlot"];
		XmlNodeList itemQuickSlotList = itemQuickSlot.ChildNodes;
		foreach( XmlNode itemNode in itemQuickSlotList)
		{
			// <Slot>
			int itemId = int.MaxValue;
			SetValue( ref itemId, itemNode);
			itemQuickSlots.Add( itemId);
		}
		
		// <SkillQuickSlot>
		XmlNode skillQuickSlot = node[ "SkillQuickSlot"];
		XmlNodeList skillQuickSlotList = skillQuickSlot.ChildNodes;
		foreach( XmlNode skillNode in skillQuickSlotList)
		{
			// <Slot>
			int skillId = int.MaxValue;
			SetValue( ref skillId, skillNode);
			skillQuickSlots.Add( skillId);
		}
	}
};

public class AsCharacterCreateClassData : AsTableRecord
{
	public Dictionary<eGENDER, AsCharacterCreateGenderData> dicGenderData = new Dictionary<eGENDER, AsCharacterCreateGenderData>();
	
	public eCLASS eClass = eCLASS.NONE;
	public string className;
	public string position;
	public string weapon;
	public int damage;
	public int defense;
	public int support;
	public int startMapID;
	public Vector2 startPoint;
	public float startPointRandRatio;
	public AsCharacterCreateCommonProperty commonProp;
	
	public AsCharacterCreateGenderData GetData( eGENDER _gender)
	{
		if( false == dicGenderData.ContainsKey( _gender))
			return null;
		
		return dicGenderData[ _gender];
	}
	
	public AsCharacterCreateClassData( XmlElement _element)
	{
		XmlNode node = (XmlElement)_element;
		
		// <Type>
		SetValue( ref eClass, node, "Type");
//		SetValue( ref className, node, "Type");
		switch( eClass)
		{
		case eCLASS.DIVINEKNIGHT:	className = AsTableManager.Instance.GetTbl_String(306);	break;
		case eCLASS.MAGICIAN:	className = AsTableManager.Instance.GetTbl_String(307);	break;
		case eCLASS.CLERIC:	className = AsTableManager.Instance.GetTbl_String(308);	break;
		case eCLASS.HUNTER:	className = AsTableManager.Instance.GetTbl_String(309);	break;
		case eCLASS.ASSASSIN:	className = AsTableManager.Instance.GetTbl_String(310);	break;
		}
		// <Position>
		SetValue( ref position, node, "Position");
		// <Weapon>
		SetValue( ref weapon, node, "Weapon");
		// <Damage>
		SetValue( ref damage, node, "Damage");
		// <Defense>
		SetValue( ref defense, node, "Defense");
		// <Support>
		SetValue( ref support, node, "Support");
		// <StartMapID>
		SetValue( ref startMapID, node, "StartMapID");
		// <StartPointX>
		SetValue( ref startPoint.x, node, "StartPointX");
		// <StartPointZ>
		SetValue( ref startPoint.y, node, "StartPointZ");
		// <StartPointRandRatio>
		SetValue( ref startPointRandRatio, node, "StartPointRandRatio");
		// <CommonProperty>
		XmlNode commonProperty = node[ "CommonProperty"];
		commonProp = new AsCharacterCreateCommonProperty( commonProperty as XmlElement);
		// <Genders>
		XmlNode genders = node[ "Genders"];
		XmlNodeList genderList = genders.ChildNodes;
		foreach( XmlNode genderNode in genderList)
		{
			// <Gender>
			AsCharacterCreateGenderData genderData = new AsCharacterCreateGenderData( genderNode as XmlElement);
			dicGenderData.Add( genderData.gender, genderData);
		}
	}
};

public class AsCharacterCreateData : AsTableRecord
{
	public Dictionary<eCLASS, AsCharacterCreateClassData> dicJob = new Dictionary<eCLASS, AsCharacterCreateClassData>();
	
	public eRACE race = eRACE.NONE;
	public int raceName;
	public string history;
	public ArrayList classes = new ArrayList();
	
	public eCLASS cls = eCLASS.NONE;
	public eGENDER gender = eGENDER.eGENDER_NOTHING;
	public int mapID = -1;
	public Vector2 startPoint = Vector2.zero;
	public float randRatio = 0.0f;
	public ArrayList hairTypes = new ArrayList();
	public ArrayList bodyList = new ArrayList();
	public ArrayList pointList = new ArrayList();
	public ArrayList handList = new ArrayList();
	
	public AsCharacterCreateClassData GetData( eCLASS _class)
	{
		if( false == dicJob.ContainsKey( _class))
			return null;
		
		return dicJob[ _class];
	}
	
	public AsCharacterCreateData( XmlElement _element)
	{
		XmlNode node = (XmlElement)_element;

		// <Name>
		SetValue( ref raceName, node, "Name");
		// <Type>
		SetValue( ref race, node, "Type");
		// <History>
		SetValue( ref history, node, "History");
		// <AvailableClasses>
		XmlNode availableClasses = node[ "AvailableClasses"];
		XmlNodeList availableClassList = availableClasses.ChildNodes;
		foreach( XmlNode classNode in availableClassList)
		{
			// <Class>
			string _class = classNode.InnerText;
			classes.Add( _class);
		}
		
		// <Classes>
		XmlNode classesNode = node[ "Classes"];
		XmlNodeList classList = classesNode.ChildNodes;
		foreach( XmlNode classNode in classList)
		{
			// <Class>
			AsCharacterCreateClassData classData = new AsCharacterCreateClassData( classNode as XmlElement);
			if( true == dicJob.ContainsKey( classData.eClass))
				continue;
				
			dicJob.Add( classData.eClass, classData);
		}
	}
}

//public class AsCharacterCreateDataManager
//{
//	public Dictionary<eRACE, AsCharacterCreateData> dicRace = new Dictionary<eRACE, AsCharacterCreateData>();
//	
//	public AsCharacterCreateDataManager( string fileName)
//	{
//		try
//		{
//            XmlElement root = AsTableBase.GetXmlRootElement( fileName);
//            XmlNodeList nodes = root.ChildNodes;
//
//            foreach( XmlNode node in nodes)
//            {
//				// <Race>
//				AsCharacterCreateData createData = new AsCharacterCreateData( node as XmlElement);
//				dicRace.Add( createData.race, createData);
//            }
//		}
//		catch( Exception e)
//		{
//			Debug.LogError(e);
//		}
//	}
//	
//	public AsCharacterCreateGenderData GetGenderData( eRACE _race, eCLASS _class, eGENDER _gender)
//	{
//		if( false == dicRace.ContainsKey( _race))
//			return null;
//		
//		AsCharacterCreateData data = dicRace[ _race];
//		AsCharacterCreateClassData classData = data.GetData( _class);
//		if( null == classData)
//			return null;
//		
//		AsCharacterCreateGenderData genderData = classData.GetData( _gender);
//		if( null == genderData)
//			return null;
//		
//		return genderData;
//	}
//	
//	public AsCharacterCreateClassData GetClassData( eRACE _race, eCLASS _class)
//	{
//		if( false == dicRace.ContainsKey( _race))
//			return null;
//		
//		AsCharacterCreateData data = dicRace[ _race];
//		AsCharacterCreateClassData classData = data.GetData( _class);
//		if( null == classData)
//			return null;
//		
//		return classData;
//	}
//	
//	public AsCharacterCreateData GetRaceData( eRACE race)
//	{
//		if( false == dicRace.ContainsKey( race))
//			return null;
//		
//		return dicRace[ race];
//	}
//}
