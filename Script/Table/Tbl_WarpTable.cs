using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public enum eWARP_TYPE
{
	Portal,
	WayPoint,
	NpcPortal,
}

public class Tbl_WarpData_Record : AsTableRecord
{
	int m_Index;
	int m_iWarpMapId;
	eWARP_TYPE m_eWarpType;
	ulong m_GoldCost;
	int m_iActiveLevel;
	int m_iName;
	public bool isActive = true;

	public string GetName()
	{
		if( null == AsTableManager.Instance)
			return string.Empty;

		return AsTableManager.Instance.GetTbl_String( m_iName);
	}

	public bool IsNameExist()
	{
		if( m_iName == 0 || m_iName == int.MaxValue)
			return false;

		return true;
	}

	public int Index
	{
		get	{ return m_Index; }
	}

	public eWARP_TYPE getWarpType
	{
		get	{ return m_eWarpType; }
	}

	public int getWarpMapId
	{
		get	{ return m_iWarpMapId; }
	}

	public ulong getGoldCost
	{
		get	{ return m_GoldCost; }
	}

	public int getActiveLevel
	{
		get	{ return m_iActiveLevel; }
	}


	public Tbl_WarpData_Record( XmlElement _element)
	{
		try
		{
			XmlNode node = ( XmlElement)_element;

			SetValue( ref m_Index, node, "ID");
			SetValue( ref m_iWarpMapId, node, "WarpMapID");
			SetValue( ref m_iName, node, "WorldName");
			SetValue( ref m_iActiveLevel, node, "ActivateLevel");
			SetValue<eWARP_TYPE>( ref m_eWarpType, node, "WarpType");
			SetValue( ref m_GoldCost, node, "GoldCost");
			SetValue( ref isActive, node, "WarpActivation");

		}
		catch( System.Exception e)
		{
			Debug.LogError( "[Tbl_WarpData_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
}

public class Tbl_WarpData_Table : AsTableBase
{
	Dictionary<int, Tbl_WarpData_Record> m_RecordList = new Dictionary<int, Tbl_WarpData_Record>();
	Dictionary<int, Tbl_WarpData_Record> m_WayPointList = new Dictionary<int, Tbl_WarpData_Record>(); // mapid , data

	public Dictionary<int, Tbl_WarpData_Record> getWaypointList
	{
		get	{ return m_WayPointList; }
	}

	public Tbl_WarpData_Table( string _path)
	{
		m_TableType = eTableType.STRING;
		LoadTable( _path);
	}

	public override void LoadTable( string _path)
	{
		XmlElement root = GetXmlRootElement( _path);
		XmlNodeList nodes = root.ChildNodes;

		foreach( XmlNode node in nodes)
		{
			Tbl_WarpData_Record record = new Tbl_WarpData_Record( ( XmlElement)node);

			if( true == m_RecordList.ContainsKey( record.Index))
			{
				AsUtil.ShutDown( "Tbl_WarpData_Table::LoadTable()[true == m_RecordList.ContainsKey] index : " + record.Index);
				return;
			}
			m_RecordList.Add( record.Index, record);

			if( eWARP_TYPE.WayPoint == record.getWarpType)
			{
				if( true == m_WayPointList.ContainsKey( record.getWarpMapId))
				{
					AsUtil.ShutDown( "Tbl_WarpData_Table::LoadTable()[true == m_WayPointList.ContainsKey] map index : " + record.getWarpMapId);
					return;
				}

				m_WayPointList.Add( record.getWarpMapId, record);
			}
		}
	}

	public Tbl_WarpData_Record GetRecord( int iIdx)
	{
		if( false == m_RecordList.ContainsKey( iIdx))
		{
			Debug.LogWarning( "Tbl_WarpData_Table::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx);
			return null;
		}

		return m_RecordList[iIdx];
	}

	public Tbl_WarpData_Record GetWaypointData( int iMapId)
	{
		if( false == m_WayPointList.ContainsKey( iMapId))
		{
			Debug.LogWarning( "Tbl_WarpData_Table::GetWaypointData()[true == m_RecordList.ContainsKey] map index : " + iMapId);
			return null;
		}

		return m_WayPointList[iMapId];
	}
}
