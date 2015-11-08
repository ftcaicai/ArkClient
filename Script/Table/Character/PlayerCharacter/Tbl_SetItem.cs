using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class Tbl_SetItem_Record : AsTableRecord
{
	public class CSetApply
	{
		public int iApply = int.MaxValue;
		public int iStringID = int.MaxValue;
	}
	
	
	int m_Index;	
	int m_iNameID;
	int m_iSkillID;
	int []m_iItemNameIDList = new int[12];
	List<int> []m_iItemIDList = new List<int>[12];
	CSetApply[] m_setApplys = new CSetApply[3];

    public int skillId
    {
        get
        {
            return m_iSkillID;
        }
    }
	
	public int Index
	{
		get
		{
			return m_Index;
		}
	}
		
	public int getNameID
	{
		get
		{
			return m_iNameID;
		}
	}   
	
	public int GetItemNameID( int iIndex )
	{
		if( m_iItemNameIDList.Length <= iIndex )
			return -1;
		
		return m_iItemNameIDList[iIndex];
	}
	
	public bool IsHaveItemID( int iIndex )
	{
		if( m_iItemIDList.Length <= iIndex )
			return false;
		
		foreach( int _itemId in m_iItemIDList[iIndex] )			
		{
			if( ItemMgr.HadItemManagement.Inven.IsHaveEquipItem( _itemId ) || ItemMgr.HadItemManagement.Inven.IsHaveCosEquipItem(_itemId) )
			{
				return true;
			}
		}
		
		return false;
	}
	
	public Tbl_SetItem_Record.CSetApply GetSetApply( int iIndex )
	{
		if( m_setApplys.Length <= iIndex )
			return null;
		
		if( m_setApplys[iIndex].iApply == int.MaxValue )
			return null;
		
		return m_setApplys[iIndex];
	}
		
	
	public Tbl_SetItem_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref m_Index, node, "Set_ID" );
			SetValue( ref m_iNameID, node, "Set_Name_ID" );
			SetValue( ref m_iSkillID, node, "Set_Skill_ID" );		
			
			
			System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();
			for( int i=0; i<12; ++i )				
			{
				sbTemp.Length=0;
				sbTemp.Append( "Set_Item" );
				sbTemp.Append( i+1 );
				sbTemp.Append( "_Name_ID" );
			
				SetValue( ref m_iItemNameIDList[i], node, sbTemp.ToString() );
								
				m_iItemIDList[i] = new List<int>();
				for( int k=0;k<3;++k )
				{
					int _itemId = 0;
					sbTemp.Length=0;
					sbTemp.Append( "Set_Item" );
					sbTemp.Append( i+1 );
					sbTemp.Append( "-" );
					sbTemp.Append( k+1 );
					sbTemp.Append( "_ID" );
					
					SetValue( ref _itemId, node, sbTemp.ToString() );
					
					if( 0 == _itemId || int.MaxValue == _itemId )
						continue;
					
					m_iItemIDList[i].Add( _itemId );				
				}
			}			
			
			m_setApplys[0] = new CSetApply();
			SetValue( ref m_setApplys[0].iApply, node, "Set_Apply1" );
			SetValue( ref m_setApplys[0].iStringID, node, "Set_Apply1_String" );
			
			m_setApplys[1] = new CSetApply();
			SetValue( ref m_setApplys[1].iApply, node, "Set_Apply2" );
			SetValue( ref m_setApplys[1].iStringID, node, "Set_Apply2_String" );
			
			m_setApplys[2] = new CSetApply();
			SetValue( ref m_setApplys[2].iApply, node, "Set_Apply3" );
			SetValue( ref m_setApplys[2].iStringID, node, "Set_Apply3_String" );		
			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SetItem_Record] 'constructor': |" + e + "| error while parsing");
		}
	}
}

public class Tbl_SetItem_Table : AsTableBase {
	
	Dictionary<int, Tbl_SetItem_Record> m_SetItemTable = 
		new Dictionary<int, Tbl_SetItem_Record>();
	
	public Tbl_SetItem_Table(string _path)
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
			Tbl_SetItem_Record record = new Tbl_SetItem_Record((XmlElement)node);
			m_SetItemTable.Add(record.Index, record);
		}
	}
	
	public Tbl_SetItem_Record GetRecord(int _id)
	{
		if(m_SetItemTable.ContainsKey(_id) == true)
			return m_SetItemTable[_id];
		
		Debug.LogWarning("[Tbl_SetItem_Table]GetRecord: there is no " + _id + "record");
		return null;
	}

 

    public List<Tbl_SetItem_Record> GetRecordAll()
    {
        List<Tbl_SetItem_Record> returnList = new List<Tbl_SetItem_Record>();

        foreach (Tbl_SetItem_Record record in m_SetItemTable.Values)
            returnList.Add(record);

        return returnList;
    }
}
