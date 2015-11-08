using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public class Tbl_SynMixEnchant_Record : AsTableRecord
{		
	public bool isSkill = false;
	public int enchantLevel_1;
	public Item.eGRADE enchantGrade_1;
	public Tbl_SoulStoneEnchant_Record.eTYPE enchantType_1;
	
	public int enchantLevel_2;
	public Item.eGRADE enchantGrade_2;
	public Tbl_SoulStoneEnchant_Record.eTYPE enchantType_2;
	
	public int cost = 0;
	
	
	public Tbl_SynMixEnchant_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;				
			
			SetValue( ref isSkill, node, "IsSkill" );		
			
			SetValue( ref enchantLevel_1, node, "Enchant1_Lv" );		
			SetValue<Item.eGRADE>(ref enchantGrade_1, node, "Enchant1_Grade");
			SetValue<Tbl_SoulStoneEnchant_Record.eTYPE>(ref enchantType_1, node, "Enchant1_Type");
			
			SetValue( ref enchantLevel_2, node, "Enchant2_Lv" );		
			SetValue<Item.eGRADE>(ref enchantGrade_2, node, "Enchant2_Grade");
			SetValue<Tbl_SoulStoneEnchant_Record.eTYPE>(ref enchantType_2, node, "Enchant2_Type");			
            
			SetValue( ref cost, node, "EnchantMixCost" );			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SynMixEnchant_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}	

	public Tbl_SynMixEnchant_Record(BinaryReader br)
	{
		isSkill = br.ReadBoolean();

		enchantLevel_1 = br.ReadInt32();
		enchantGrade_1 = (Item.eGRADE)br.ReadInt32();
		enchantType_1 = (Tbl_SoulStoneEnchant_Record.eTYPE)br.ReadInt32();

		enchantLevel_2 = br.ReadInt32();
		enchantGrade_2 = (Item.eGRADE)br.ReadInt32();
		enchantType_2 = (Tbl_SoulStoneEnchant_Record.eTYPE)br.ReadInt32();

		cost = br.ReadInt32();
	}

}

public class Tbl_SynMixEnchant_Table : AsTableBase 
{
	List<Tbl_SynMixEnchant_Record> m_recordList = new List<Tbl_SynMixEnchant_Record>();
	
	public Tbl_SynMixEnchant_Table(string _path)
	{
		m_TableType = eTableType.STRING;		
		LoadTable(_path);
	}
	 
	public override void LoadTable(string _path)
	{
		try
		{
			if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || 
			   ( null != AsTableManager.Instance && true == AsTableManager.Instance.useReadBinary ) )
			{		
				// Ready Binary
				TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
				MemoryStream stream = new MemoryStream( textAsset.bytes);
				BinaryReader br = new BinaryReader( stream);
				
				int nCount = br.ReadInt32();
				
				
				for( int i = 0; i < nCount; i++)
				{
					Tbl_SynMixEnchant_Record record = new Tbl_SynMixEnchant_Record(br);
					m_recordList.Add(record);	
				}
				
				br.Close();
				stream.Close();	
			}
			else
			{
				XmlElement root = GetXmlRootElement(_path);
				XmlNodeList nodes = root.ChildNodes;
				
				foreach(XmlNode node in nodes)
				{
					Tbl_SynMixEnchant_Record record = new Tbl_SynMixEnchant_Record((XmlElement)node);			
					m_recordList.Add( record );
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_NormalNpc_Table] LoadTable:|" + e + "| error while parsing");
		}
	}		
	
	public Tbl_SynMixEnchant_Record GetRecord( bool _isSkill, int level_1, Item.eGRADE _eGrade_1, Tbl_SoulStoneEnchant_Record.eTYPE _type_1,
		int level_2, Item.eGRADE _eGrade_2, Tbl_SoulStoneEnchant_Record.eTYPE _type_2 )
	{		
		foreach( Tbl_SynMixEnchant_Record _record in m_recordList )
		{
			if( _record.isSkill != _isSkill )
				continue;
			
			bool b1 = level_1 == _record.enchantLevel_1 && _eGrade_1 == _record.enchantGrade_1 && _type_1 == _record.enchantType_1;			
			bool b2 = level_2 == _record.enchantLevel_2 && _eGrade_2 == _record.enchantGrade_2 && _type_2 == _record.enchantType_2;			
			bool b3 = level_2 == _record.enchantLevel_1 && _eGrade_2 == _record.enchantGrade_1 && _type_2 == _record.enchantType_1;			
			bool b4 = level_1 == _record.enchantLevel_2 && _eGrade_1 == _record.enchantGrade_2 && _type_1 == _record.enchantType_2;
			
			
			if( b1 && b2 )
				return _record;
			
			if( b3 && b4 )
				return _record;		
		}
		
		return null;
	}
	
}