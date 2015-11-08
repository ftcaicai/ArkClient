using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class Tbl_SoulStoneEnchant_Record : AsTableRecord
{	
	
	public enum eTYPE
	{
		WEAPON,
		DEFEND,
		ACCESSORY,
	}
	
	
	public enum eSTATE
	{
		NONE,
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
		Skill = 100,
	}	
	
	int m_Index;
	eTYPE m_eType;
	bool m_isCostume;
	eSTATE m_eState1;
	int m_iValue1;		
	int m_iRankPoint;
	int m_ExtractionCount;
	
	public bool IsCostume
	{
		get
		{
			return m_isCostume;
		}
	}
	
	public eSTATE getState
	{
		get
		{
			return m_eState1;	
		}
	}
	
	public int getValue
	{
		get
		{
			return m_iValue1;	
		}
	}
		
	public int Index
	{
		get
		{
			return m_Index;
		}
	}
		
	public eTYPE getEquipType
	{
		get
		{
			return m_eType;
		}
		
	}
		
	public int getRankPoint
	{
		get
		{
			return m_iRankPoint;
		}
	}
	
	public int getExtractionCount
	{
		get
		{
			return m_ExtractionCount;
		}
	}
	
	public Tbl_SoulStoneEnchant_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;			
		
			SetValue( ref m_Index, node, "Index" );
			SetValue<eTYPE>( ref m_eType, node, "Type" );
			SetValue( ref m_isCostume, node, "Costume" );
			SetValue<eSTATE>( ref m_eState1, node, "State1" );
			SetValue( ref m_iValue1, node, "State1_Value" );	
			SetValue( ref m_iRankPoint, node, "RankPoint" );	
			SetValue( ref m_ExtractionCount, node, "ExtractionCount" );	
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SoulStoneEnchant_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
}

public class Tbl_SoulStoneEnchant_Table : AsTableBase {
	
	Dictionary<int, Tbl_SoulStoneEnchant_Record> m_SoulStoneEnchantmTable = 
		new Dictionary<int, Tbl_SoulStoneEnchant_Record>();
	
	public Tbl_SoulStoneEnchant_Table(string _path)
	{
		m_TableType = eTableType.STRING;		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		XmlElement root = GetXmlRootElement(_path);
		XmlNodeList nodes = root.ChildNodes;
		
		foreach(XmlNode node in nodes)
		{
			Tbl_SoulStoneEnchant_Record record = new Tbl_SoulStoneEnchant_Record((XmlElement)node);
			m_SoulStoneEnchantmTable.Add(record.Index, record);
		}
	}
	
	public Tbl_SoulStoneEnchant_Record GetRecord(int _id)
	{
		if(m_SoulStoneEnchantmTable.ContainsKey(_id) == true)
			return m_SoulStoneEnchantmTable[_id];
		
		Debug.LogWarning("[Tbl_SoulStoneEnchant_Table]GetRecord: there is no " + _id + "record");
		return null;
	}
}
