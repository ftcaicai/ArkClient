using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;


public enum eITEM_EFFECT
{
	NONE	= 0,	
	HP_plus,
	MP_plus,
	Patk_dmg_min,
	Patk_dmg_max,
	Patk_dmg,
	Atk_hit,
	Atk_critical,
	Matk_dmg_min,
	Matk_dmg_max,
	Matk_dmg,
	Pdef,
	Mdef,
	Pdodge,
	Fire_authorize,
	Ice_authorize,
	Light_authorize,
	Dark_authorize,
	Nature_authorize,
	Atk_critical_dmg,
	Stun_tolerance,
	Bleed_tolerance,
	Normal_dmg_atk_rate,
	Elite_dmg_atk_rate,
	Boss_dmg_atk_rate,
	Normal_cri_dmg_rate,
	Elite_cri_dmg_rate,
	Boss_cri_dmg_rate,
	Normal_dmg_def_rate,
	Elite_dmg_def_rate,
	Boss_dmg_def_rate,
	Normal_cri_prob_rate,
	Elite_cri_prob_rate,
	Boss_cri_prob_rate,
	HP_Regen,
	MP_Regen,
	Patk_rate,
	Matk_rate,
	Bloodsucking,
	Bloodsucking_rate,
	HP_plus_rate,
	MP_plus_rate,
	Pdef_rate,
	Mdef_rate,
	HP_Regen_rate,
	MP_Regen_rate,
	Atk_speed,
	Move_speed,
};


public class Tbl_Strengthen_Record : AsTableRecord
{		
	public int m_Index;	
	public Item.eITEM_TYPE m_itemType;
	public Item.eGRADE m_ItemQuality;
	public Item.eEQUIP m_equip;
	public int m_ItemLevellimit;
	public int m_iStep;
	public int m_iCost;	
	public int m_StuffCount;
	public int m_StrengthenRating;
	public long m_iMiracle;
	public int m_iBlessing;

		
			
	public int Index
	{
		get
		{
			return m_Index;
		}
	}

    public Item.eITEM_TYPE getItemType
    {
        get
        {
            return m_itemType;
        }
    }
     
	
	public int getStep	
	{
		get
		{
			return m_iStep;
		}
	}
		
	
	public Item.eGRADE getItemQuality
	{
		get	
		{
			return m_ItemQuality;
		}
	}
		
	public int getItemLevelLimit
	{
		get
		{
			return m_ItemLevellimit;
		}
	}
	
	public int getStrengthenRatiog
	{
		get
		{
			return m_StrengthenRating;
		}
	}	
	
	
	public int getStrengthenCost
	{
		get
		{
			return m_iCost;
		}
	}
	
	public int getStrengthenStuffCount
	{
		get
		{
			return m_StuffCount;
		}
	}
	
	public long getMiracle
	{
		get
		{
			return m_iMiracle;
		}
	}
	
	public Item.eEQUIP getEquip
	{
		get
		{
			return m_equip;
		}
	}
	
	
	static public eITEM_OPTION GetOptionType( eITEM_EFFECT eItemEffect )
	{
		int iTemp = (int)eItemEffect;
		
		return (eITEM_OPTION)iTemp;	
	}
	
	
	public Tbl_Strengthen_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;	
			
			SetValue( ref m_Index, node, "ID" );
            SetValue<Item.eITEM_TYPE>(ref m_itemType, node, "ItemType");			
			SetValue<Item.eGRADE>( ref m_ItemQuality, node, "ItemGrade" );
			SetValue<Item.eEQUIP>( ref m_equip, node, "ItemPart" );			
			SetValue( ref m_ItemLevellimit, node, "ItemLevel" );				
			SetValue( ref m_iStep, node, "Step" );			
			SetValue( ref m_iCost, node, "Cost" );			
			SetValue( ref m_StuffCount, node, "StuffCount" );
			SetValue( ref m_StrengthenRating, node, "IncreaseRatio" );			
			SetValue( ref m_iMiracle, node, "Miracle" );
			SetValue( ref m_iBlessing, node, "Blessing" );			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_StrengthenState_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
	
	public Tbl_Strengthen_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_itemType = (Item.eITEM_TYPE)br.ReadInt32();
		m_ItemQuality = (Item.eGRADE)br.ReadInt32();
		m_equip = (Item.eEQUIP)br.ReadInt32();
		m_ItemLevellimit = br.ReadInt32();
		m_iStep = br.ReadInt32();		
		m_iCost = br.ReadInt32();	
		m_StuffCount = br.ReadInt32();	
		m_StrengthenRating = br.ReadInt32();
		m_iMiracle = br.ReadInt64();
		m_iBlessing = br.ReadInt32();	
	}
}

public class Tbl_Strengthen_Table : AsTableBase 
{
	
	List<Tbl_Strengthen_Record> m_StrengthenStateTable = new List<Tbl_Strengthen_Record>();
	
	public Tbl_Strengthen_Table(string _path)
	{
		m_TableType = eTableType.STRING;		
		LoadTable(_path);
	}
	 
	public override void LoadTable(string _path)
	{
		if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || true == AsTableManager.Instance.useReadBinary )
		{	
			// Ready Binary
			TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
			MemoryStream stream = new MemoryStream( textAsset.bytes);
			BinaryReader br = new BinaryReader( stream);
			
			int nCount = br.ReadInt32();
				
	
			for( int i = 0; i < nCount; i++)
			{
				Tbl_Strengthen_Record record = new Tbl_Strengthen_Record( br);
				m_StrengthenStateTable.Add(record);			
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
				Tbl_Strengthen_Record record = new Tbl_Strengthen_Record((XmlElement)node);			
				m_StrengthenStateTable.Add( record );
			}
			
		}
	}	
	
	public bool IsEnableParts(Item.eEQUIP _equip)
	{
		if( _equip == Item.eEQUIP.Weapon || _equip == Item.eEQUIP.Armor ||
			_equip == Item.eEQUIP.Gloves || _equip == Item.eEQUIP.Head ||
			_equip == Item.eEQUIP.Point )
			return true;
		
		return false;
	}
	
	public Tbl_Strengthen_Record GetStrengthenRecord( Item.eITEM_TYPE _eItemtype, Item.eEQUIP _equip, Item.eGRADE _eGrade, int _iLevel, int _iStep )
	{
		if( false == IsEnableParts( _equip ) )
			return null;
			
		foreach( Tbl_Strengthen_Record _record in m_StrengthenStateTable )
		{
			if( _record.getItemQuality == _eGrade && _record.getItemType == _eItemtype &&
				_record.getItemLevelLimit == _iLevel && _record.getStep == _iStep && _record.getEquip == _equip )
			{
				return _record;
			}
		}
		
		return null;
	}
	
}
