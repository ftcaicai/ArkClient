using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public class Tbl_Production_Record : AsTableRecord
{	
	int m_iIndex;
	eITEM_PRODUCT_TECHNIQUE_TYPE m_type;
	int m_iLevel;
	int m_iItemID;
	int m_iItemCount;
	public float itemTime;
	public int iExpertism;
	public int iExp;
	public ulong iGold;
	public int iBaseID;
	public int iBaseCount;
	public int iSubID_1;
	public int iSubCount_1;
	public int iSubID_2;
	public int iSubCount_2;
	public int iOpID;
	public int iOpCount;
	public int miracle;
	
	
	public bool IsRandItemType()
	{
		return m_type == eITEM_PRODUCT_TECHNIQUE_TYPE.RING || 
				m_type == eITEM_PRODUCT_TECHNIQUE_TYPE.NECKLACE ||
				m_type == eITEM_PRODUCT_TECHNIQUE_TYPE.EARRING;
	}
	
	public int getIndex
	{
		get
		{
			return m_iIndex;
		}
	}
	
	public eITEM_PRODUCT_TECHNIQUE_TYPE getType
	{
		get
		{
			return m_type;
		}
	}
	
	public int getLevel
	{
		get
		{
			return m_iLevel;
		}
	}
	
	public int getItemID
	{
		get
		{
			return m_iItemID;
		}
	}
	public int getItemCount
	{
		get
		{
			return m_iItemCount;
		}
	}
	
	
	
	public Tbl_Production_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_iIndex, node, "Index");	
			SetValue<eITEM_PRODUCT_TECHNIQUE_TYPE>(ref m_type, node, "Production_Technic");
			
			SetValue(ref m_iLevel, node, "Production_Level");	
			SetValue(ref m_iItemID, node, "Production_Item_ID1");	
			SetValue(ref m_iItemCount, node, "Production_Item_Count1");	
			SetValue(ref itemTime, node, "Production_Item_Time");	
			itemTime *= 0.001f;
			SetValue(ref iExpertism , node, "Production_Item_Expertism");	
			SetValue(ref iExp , node, "Production_Item_EXP");	
			SetValue(ref iGold , node, "Production_Item_Gold");	
			SetValue(ref iBaseID , node, "Production_Item_Base");	
			SetValue(ref iBaseCount , node, "Production_Item_BaseCount");	
			SetValue(ref iSubID_1 , node, "Production_Item_Sub");	
			SetValue(ref iSubCount_1 , node, "Production_Item_SubCount");	
			SetValue(ref iSubID_2 , node, "Production_Item_Sub2");	
			SetValue(ref iSubCount_2 , node, "Production_Item_Sub2Count");	
			SetValue(ref iOpID , node, "Production_Item_Op");	
			SetValue(ref iOpCount , node, "Production_Item_OpCount");	
			SetValue(ref miracle, node, "Production_Item_Miracle");
			
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class Tbl_Production_Table : AsTableBase 
{		
	Dictionary<eITEM_PRODUCT_TECHNIQUE_TYPE, Dictionary<int, List<Tbl_Production_Record>>> 
		m_recordList = new Dictionary<eITEM_PRODUCT_TECHNIQUE_TYPE, Dictionary<int, List<Tbl_Production_Record>>>();
	
	
	Dictionary<int, Tbl_Production_Record> m_dataList = new Dictionary<int, Tbl_Production_Record>();
	
	public Tbl_Production_Table(string _path)
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
			
			Tbl_Production_Record record = new Tbl_Production_Record((XmlElement)node);
			
			if( true == m_dataList.ContainsKey( record.getIndex ) )
			{
				AsUtil.ShutDown("Tbl_ProductionTable::LoadTable()[ same index : " + record.getIndex );
				continue;
			}
			m_dataList.Add( record.getIndex, record );
			
			
			if( false == m_recordList.ContainsKey( record.getType ) )
			{
				Dictionary<int,List<Tbl_Production_Record>> levellist = new Dictionary<int, List<Tbl_Production_Record>>();
				levellist.Add( record.getLevel, new List<Tbl_Production_Record>() );
				
				m_recordList.Add( record.getType, levellist );				
			}
			
			if( false == m_recordList[ record.getType ].ContainsKey( record.getLevel ) )
			{
				m_recordList[ record.getType ].Add( record.getLevel, new List<Tbl_Production_Record>() );
			}
			
			
			m_recordList[ record.getType ][record.getLevel].Add( record );
				
		}
	}
	
	public Tbl_Production_Record GetRecord(int _idx)
	{
		if(m_dataList.ContainsKey(_idx) == true)
			return m_dataList[_idx];
		
		//Debug.LogWarning("[Tbl_Production_Table]GetRecord: there is no record [ id : " + _idx );
		return null;
	}	
	
	public List<Tbl_Production_Record> GetRecordList( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, int iLevel )
	{
		if(false == m_recordList.ContainsKey(_eType))
		{
			//AsUtil.ShutDown("Tbl_Production_Table::GetRecord()[ no exist type : " + _eType );
			return null;
		}
		
		if(false == m_recordList[_eType].ContainsKey( iLevel ) )
		{
			//AsUtil.ShutDown("Tbl_Production_Table::GetRecord()[ no exist type : " + _eType + " level : " + iLevel );
			return null;		
		}
		
		
		return m_recordList[_eType][iLevel];
	}	
	
	
}
