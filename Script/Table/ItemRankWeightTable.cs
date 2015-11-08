using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


/*
public enum eTABLE_ITEM_OPTION : int
{
	NONE	= 0,

	HP_plus,//eITEM_OPTION_HP_PLUS,
	MP_plus,//eITEM_OPTION_MP_PLUS,

	Atk_hit,// eITEM_OPTION_PATK_DMG_MIN,
	Atk_critical,// eITEM_OPTION_PATK_DMG_MAX,
	Pdodge,// eITEM_OPTION_PATK_DMG,

	Pdef,//eITEM_OPTION_ATK_HIT_PROB,
	Mdef,//eITEM_OPTION_CRITICAL_PROB,

	Patk_dmg_min,//eITEM_OPTION_MATK_DMG_MIN,
	Patk_dmg_max,//eITEM_OPTION_MATK_DMG_MAX,
	Patk_dmg,//eITEM_OPTION_MATK_DMG,

	Matk_dmg_min,//eITEM_OPTION_PDEF,
	Matk_dmg_max,//eITEM_OPTION_MDEF,

	Matk_dmg,//eITEM_OPTION_DODGE_PROB,
	
	Fire_authorize,//eITEM_OPTION_FIRE,
	Ice_authorize,//eITEM_OPTION_ICE,
	Light_authorize,//eITEM_OPTION_LIGHT,
	Dark_authorize,//eITEM_OPTION_DARK,
	Nature_authorize,//eITEM_OPTION_NATURE,

	Atk_critical_dmg,//eITEM_OPTION_CRI_DMG,

	Stun_tolerance,//eITEM_OPTION_STUN_IMMUNE,
	Bleed_tolerance,//eITEM_OPTION_BLEED_IMMUNE,

	Normal_dmg_atk_rate,//eITEM_OPTION_NORAML_DMG_PROB,
	Elite_dmg_atk_rate,//eITEM_OPTION_ELITE_DMG_PROB,
	Boss_dmg_atk_rate,//eITEM_OPTION_BOSS_DMB_PROB,

	Normal_cri_dmg_rate,//eITEM_OPTION_NORMAL_CRI_DMG_PROB,
	Elite_cri_dmg_rate,//eITEM_OPTION_ELITE_CRI_DMG_PROB,
	Boss_cri_dmg_rate,//eITEM_OPTION_BOSS_CRI_DMG_PROB,

	Normal_dmg_def_rate,//eITEM_OPTION_NORM_DEF_PROB,
	Elite_dmg_def_rate,//eITEM_OPTION_ELITE_DEF_PROB,
	Boss_dmg_def_rate,//eITEM_OPTION_BOSS_DEF_PROB,

	Normal_cri_prob_rate,//eITEM_OPTION_NORMAL_CRI_PROB,
	Elite_cri_prob_rate,//eITEM_OPTION_ELITE_CRI_PROB,
	Boss_cri_prob_rate,//eITEM_OPTION_BOSS_CRI_PROB,

	HP_Regen,//eITEM_OPTION_HP_REGEN,
	MP_Regen,//eITEM_OPTION_MP_REGEN,

	Patk_rate,//eITEM_OPTION_PATK_PROB,
	Matk_rate,//eITEM_OPTION_MATK_PROB,

	Bloodsucking,//eITEM_OPTION_HP_DRAIN,
	Bloodsucking_rate,//eITEM_OPTION_HP_DRAIN_PROB,

	HP_plus_rate,//eITEM_OPTION_HP_PROB,
	MP_plus_rate,//eITEM_OPTION_MP_PROB,

	Pdef_rate,//eITEM_OPTION_PDEF_PROB,
	Mdef_rate,//eITEM_OPTION_MDEF_PROB,

	HP_Regen_rate,//eITEM_OPTION_HP_REGEN_PROB,
	MP_Regen_rate,//eITEM_OPTION_MP_REGEN_PROB,
		
	Atk_speed,//eITEM_OPTION_ATK_SPEED_PROB,
	Move_speed,//eITEM_OPTION_MOVE_SPEED_PROB,

	eITEM_OPTION_MAX
};
 */
public class Tbl_ItemRankWeight_Record: AsTableRecord
{
	public int iItemlevel;
	public eITEM_EFFECT eOptionType;
	public Dictionary< eCLASS, float > m_itemClassList = new Dictionary<eCLASS, float>();
	
		
	public Tbl_ItemRankWeight_Record(XmlElement _element)// : base(_element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;	
			SetValue(ref iItemlevel, node, "Level");
			SetValue<eITEM_EFFECT>(ref eOptionType, node, "Type");
			
			float data = 0f;
			SetValue(ref data, node, "All");
			m_itemClassList.Add( eCLASS.All, data );
			
			SetValue(ref data, node, "DivineKnight");
			m_itemClassList.Add( eCLASS.DIVINEKNIGHT, data );			
			
			SetValue(ref data, node, "Cleric");
			m_itemClassList.Add( eCLASS.CLERIC, data );
			
			SetValue(ref data, node, "Magician");
			m_itemClassList.Add( eCLASS.MAGICIAN, data );
			
			SetValue(ref data, node, "Hunter");
			m_itemClassList.Add( eCLASS.HUNTER, data );
			
			SetValue(ref data, node, "Assassin");
			m_itemClassList.Add( eCLASS.ASSASSIN, data );			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_BuffMinMaxTable_Record] : " + e );
		}
	}
	
	
	
	public Tbl_ItemRankWeight_Record(BinaryReader br)
	{
		iItemlevel = br.ReadInt32();
		eOptionType = (eITEM_EFFECT)br.ReadInt32();		
		m_itemClassList.Add( eCLASS.All, br.ReadSingle() );
		m_itemClassList.Add( eCLASS.DIVINEKNIGHT, br.ReadSingle() );
		m_itemClassList.Add( eCLASS.CLERIC, br.ReadSingle() );
		m_itemClassList.Add( eCLASS.MAGICIAN, br.ReadSingle() );
		m_itemClassList.Add( eCLASS.HUNTER, br.ReadSingle() );
		m_itemClassList.Add( eCLASS.ASSASSIN, br.ReadSingle() );
	}
	
	
}

public class Tbl_ItemRankWeightTable : AsTableBase 
{
	
	List<Tbl_ItemRankWeight_Record> m_recordList = new List<Tbl_ItemRankWeight_Record>();

	
	public Tbl_ItemRankWeightTable(string _path)
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
				Tbl_ItemRankWeight_Record record = new Tbl_ItemRankWeight_Record( br);
				m_recordList.Add( record );		
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
				Tbl_ItemRankWeight_Record record = new Tbl_ItemRankWeight_Record((XmlElement)node);			
				m_recordList.Add( record );		
			}
		}
	}
	
	public Tbl_ItemRankWeight_Record GetRecord( int ilevel, eITEM_EFFECT type ) 
	{
		foreach( Tbl_ItemRankWeight_Record record in m_recordList )
		{
			if( record.iItemlevel == ilevel && record.eOptionType == type )
			{
				return record;
			}
		}
		
		Debug.LogError( "ItemRankWeightTable::Getrecord()[ level: " + ilevel + "type :" + type );
		return null;
	}
	
}
