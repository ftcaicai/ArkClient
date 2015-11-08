using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.Xml;


public class Tbl_ZoneMap_Record : AsTableRecord
{		
	public class PotalData
	{
		public Vector3 position;
		public int iStrIdx;
		public Vector2 uiPos;
	}
	
	public class NpcData
	{
		public int iNpcId;
		public Vector3 position;
		public int iImgIdx;
		public int iStrIdx;
		public Vector2 uiPos;
	}
	
	public class WayPointData
	{
		public Vector3 position;
		public int iStrIdx;
		public Vector2 uiPos;
	}
	
	
	int m_Index;	
	string m_strImgPath;
	int m_iTooltipStrIdx;
	int m_iAreaMapIdx;
	List<PotalData> m_PotalList = new List<PotalData>();
	List<NpcData> m_NpcList = new List<NpcData>();
	List<WayPointData> m_WayPointList = new List<WayPointData>();
	List<WayPointData> m_LocalNameList = new List<WayPointData>();
		
	public int Index
	{
		get
		{
			return m_Index;
		}
	}
	
	public string getStrImgPath
	{
		get
		{
			return m_strImgPath;
		}
	}
		
	public int getAreaMapIdx
	{
		get
		{
			return m_iAreaMapIdx;
		}
	}
	
	public int getTooltipStrIdx
	{
		get	
		{
			return m_iTooltipStrIdx;
		}
	}
	
	public List<PotalData> getPotalList
	{
		get
		{
			return m_PotalList;
		}
	}
	
	
	public List<NpcData> getNpcList
	{
		get
		{
			return m_NpcList;
		}
	}
	
	
	public List<WayPointData> getWayPointList
	{
		get
		{
			return m_WayPointList;
		}
	}
	
	
	public List<WayPointData> getLocalNameList
	{
		get
		{
			return m_LocalNameList;
		}
	}
	
	
	
	
	
	public Tbl_ZoneMap_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref m_Index, node, "Index" );
			SetValue( ref m_iAreaMapIdx, node, "AreaMapIdx" );			
			SetValue( ref m_strImgPath, node, "ImgPath" );
			SetValue( ref m_iTooltipStrIdx, node, "TooltipStringIdx" );			
			
			// potal
			int iPotalListCount = 0;
			SetValue( ref iPotalListCount, node, "PotalCount" );
			for( int i=0; i<iPotalListCount; ++i )
			{
				string strElement = "Potal_"+(i+1);
				PotalData data = new PotalData();				
				data.position.x = float.Parse( _element[strElement].GetAttribute("x") );
				data.position.y = float.Parse( _element[strElement].GetAttribute("y") );  
				data.position.z = float.Parse( _element[strElement].GetAttribute("z") );  
				data.iStrIdx = int.Parse( _element[strElement].GetAttribute("strIdx") );
				data.uiPos.x = float.Parse( _element[strElement].GetAttribute("ui_x") );
				data.uiPos.y = float.Parse( _element[strElement].GetAttribute("ui_y") );  
				
                m_PotalList.Add( data );
			}
			
			// npc
			int iNpcListCount = 0;
			SetValue( ref iNpcListCount, node, "NpcCount" );
			for( int i=0; i<iNpcListCount; ++i )
			{
				string strElement = "Npc_"+(i+1);
				NpcData _data = new NpcData();
				_data.iNpcId = int.Parse( _element[strElement].GetAttribute("id") );
				
				_data.position.x = float.Parse( _element[strElement].GetAttribute("x") );
				_data.position.y = float.Parse( _element[strElement].GetAttribute("y") );  
				_data.position.z = float.Parse( _element[strElement].GetAttribute("z") );  
				
				_data.iImgIdx = int.Parse( _element[strElement].GetAttribute("imgIdx") );  
				_data.iStrIdx = int.Parse( _element[strElement].GetAttribute("strIdx") );  
				
				_data.uiPos.x = float.Parse( _element[strElement].GetAttribute("ui_x") );
				_data.uiPos.y = float.Parse( _element[strElement].GetAttribute("ui_y") );  
	
				
                m_NpcList.Add( _data );
			}
			
			//waypoint
			int iWayPointList = 0;
			SetValue( ref iWayPointList, node, "WayPointList" );
			for( int i=0; i<iWayPointList; ++i )
			{
				string strElement = "WayPoint_"+(i+1);
				WayPointData data = new WayPointData();				
				data.position.x = float.Parse( _element[strElement].GetAttribute("x") );
				data.position.y = float.Parse( _element[strElement].GetAttribute("y") );  
				data.position.z = float.Parse( _element[strElement].GetAttribute("z") );  
				data.iStrIdx = int.Parse( _element[strElement].GetAttribute("strIdx") ); 
				data.uiPos.x = float.Parse( _element[strElement].GetAttribute("ui_x") );
				data.uiPos.y = float.Parse( _element[strElement].GetAttribute("ui_y") );  
				
                m_WayPointList.Add( data );
			}
			
			
			//local name
			int iLocalNameList = 0;
			SetValue( ref iLocalNameList, node, "LocalNameCount" );
			for( int i=0; i<iLocalNameList; ++i )
			{
				string strElement = "LocalName_"+(i+1);
				WayPointData data = new WayPointData();				
				data.position.x = float.Parse( _element[strElement].GetAttribute("x") );
				data.position.y = float.Parse( _element[strElement].GetAttribute("y") );  
				data.position.z = float.Parse( _element[strElement].GetAttribute("z") );  
				data.iStrIdx = int.Parse( _element[strElement].GetAttribute("strIdx") ); 
				data.uiPos.x = float.Parse( _element[strElement].GetAttribute("ui_x") );
				data.uiPos.y = float.Parse( _element[strElement].GetAttribute("ui_y") );  
				
                m_LocalNameList.Add( data );
			}
					
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_ZoneMap_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
}

public class Tbl_ZoneMap_Table : AsTableBase 
{	
	Dictionary<int, Tbl_ZoneMap_Record> m_RecordList = new Dictionary<int, Tbl_ZoneMap_Record>();
	
	
	public Tbl_ZoneMap_Table(string _path)
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
			Tbl_ZoneMap_Record record = new Tbl_ZoneMap_Record((XmlElement)node);
			
			if( true == m_RecordList.ContainsKey( record.Index ) )
			{
				AsUtil.ShutDown("Tbl_ZoneMapTable::LoadTable()[true == m_RecordList.ContainsKey] index : " + record.Index );
				return;
			}
			m_RecordList.Add( record.Index, record );
		}
	}		
	
	
	public Tbl_ZoneMap_Record GetRecord( int iIdx )
	{
		if( false == m_RecordList.ContainsKey( iIdx ) )
		{
			//AsUtil.ShutDown("Tbl_ZoneMapTable::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx );
            Debug.Log("Tbl_ZoneMapTable::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx);
			return null;
		}
		
		return m_RecordList[iIdx];
	}
}
