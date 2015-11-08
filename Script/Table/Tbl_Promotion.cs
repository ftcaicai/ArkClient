using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Def_Promotion
{
	public enum eCondition {NONE, Revival, Zone, Channel, Login, Condition}
//	public enum eMiracle_Page {MAIN, CHARGE_MIRACLE, CHARGE_GOLD, COSTUME, USEITEM, BOOSTER, JEWEL, RANDOM, PACKAGE, ETC}
}

public class Tbl_Promotion_Record : AsTableRecord
{
	int m_Index; public int Index	{ get { return m_Index; } }
	int m_Priority; public int Priority	{ get { return m_Priority; } }
	Def_Promotion.eCondition m_Condition; public Def_Promotion.eCondition Condition	{ get { return m_Condition; } }
	eCLASS m_Class; public eCLASS Class	{ get { return m_Class; } }
	int m_Level_Min; public int Level_Min	{ get { return m_Level_Min; } }
	int m_Level_Max; public int Level_Max	{ get { return m_Level_Max; } }
	int m_Item_Index; public int Item_Index	{ get { return m_Item_Index; } }
	int m_String_Index; public int String_Index	{ get { return m_String_Index; } }
	eCashStoreMenuMode m_Miracle_Page; public eCashStoreMenuMode Miracle_Page	{ get { return m_Miracle_Page; } }
	float m_PromotionTime; public float PromotionTime	{ get { return m_PromotionTime; } }
	float m_Probability; public float Probability	{ get { return m_Probability; } }
	eCashStoreSubCategory m_SubCategory; public eCashStoreSubCategory Sub_Category{get{return m_SubCategory;}}

	public Tbl_Promotion_Record( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;

			SetValue( ref m_Index, node, "Index");
			SetValue( ref m_Priority, node, "Priority");
			SetValue<Def_Promotion.eCondition>( ref m_Condition, node, "Condition");
			SetValue<eCLASS>( ref m_Class, node, "Class");

			SetValue( ref m_Level_Min, node, "Level_Min");
			SetValue( ref m_Level_Max, node, "Level_Max");
			SetValue( ref m_Item_Index, node, "Item_Index");
			SetValue( ref m_String_Index, node, "String_Index");

			SetValue<eCashStoreMenuMode>( ref m_Miracle_Page, node, "Miracle_Page");

			SetValue( ref m_PromotionTime, node, "PromotionTime");
			SetValue( ref m_Probability, node, "Probability");
			SetValue( ref m_SubCategory, node, "Sub_Category");
			
		}
		catch( System.Exception e)
		{
			Debug.LogError( e);
		}
	}

	public bool CheckValidLevel( int _lv)
	{
		if( m_Level_Min <= _lv && _lv <= m_Level_Max)
			return true;

		return false;
	}
}

public class Tbl_Promotion_Table : AsTableBase
{
	SortedList<int, Tbl_Promotion_Record> m_ResourceTable = new SortedList<int, Tbl_Promotion_Record>();
	Dictionary<eCLASS, MultiDictionary<Def_Promotion.eCondition, Tbl_Promotion_Record>> m_dmdicCondition = new Dictionary<eCLASS, MultiDictionary<Def_Promotion.eCondition, Tbl_Promotion_Record>>();

	public Tbl_Promotion_Table( string _path)
	{
		LoadTable( _path);
	}

	public override void LoadTable( string _path)
	{
		try
		{
			XmlElement root = GetXmlRootElement( _path);
			XmlNodeList nodes = root.ChildNodes;

//			List<char> listChar = new List<char>();
			foreach( XmlNode node in nodes)
			{
				Tbl_Promotion_Record record = new Tbl_Promotion_Record( (XmlElement)node);
				m_ResourceTable.Add( record.Index, record);

				if( m_dmdicCondition.ContainsKey( record.Class) == false)
					m_dmdicCondition.Add( record.Class, new MultiDictionary<Def_Promotion.eCondition, Tbl_Promotion_Record>());

				m_dmdicCondition[record.Class].Add( record.Condition, record);
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( e);
		}
	}

	public MultiDictionary<Def_Promotion.eCondition, Tbl_Promotion_Record> GetRecordsByClass( eCLASS _class)
	{
		if( m_dmdicCondition.ContainsKey( _class) == true)
			return m_dmdicCondition[_class];
		else
		{
			Debug.LogError( "Tbl_Promotion_Table::GetRecordsByClass: no records. _class = " + _class);
			return null;
		}
	}
}
