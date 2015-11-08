using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;




public class Tbl_Lottery_Record : AsTableRecord
{		
	public int Index = 0;
	bool m_NeedEffect; public bool needEffect{get{return m_NeedEffect;}}
	public List<int> idlist = new List<int>();
	
	
	
	public Tbl_Lottery_Record(XmlElement _element)
	{
		
		System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();
		
		try
		{
			XmlNode node = (XmlElement)_element;
		
			SetValue( ref Index, node, "Index" );
			SetValue( ref m_NeedEffect, node, "Eff_type" );			
			for( int i=1; i<=10; ++i )
			{
				int _iIndex = 0;
				sbTemp.Length = 0;
				sbTemp.Append( "Rand_ID" );
				sbTemp.Append( i );
				SetValue( ref _iIndex, node, sbTemp.ToString() );
				
				if( 0 != _iIndex && int.MaxValue != _iIndex )
				{
					idlist.Add( _iIndex ); 
				}
				else 
					break;
			}		
					
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Lottery_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
}

public class Tbl_Lottery_Table : AsTableBase 
{	
	Dictionary<int, Tbl_Lottery_Record> m_RecordList = new Dictionary<int, Tbl_Lottery_Record>();
	
		
	
	public Tbl_Lottery_Table(string _path)
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
			Tbl_Lottery_Record record = new Tbl_Lottery_Record((XmlElement)node);
			
			if( true == m_RecordList.ContainsKey( record.Index ) )
			{
				AsUtil.ShutDown("Tbl_Lottery_Table::LoadTable()[true == m_RecordList.ContainsKey] index : " + record.Index );
				continue;
			}
			m_RecordList.Add( record.Index, record );			
		}
	}		
	
	
	public Tbl_Lottery_Record GetRecord( int iIdx )
	{
		if( false == m_RecordList.ContainsKey( iIdx ) )
		{
			Debug.LogWarning("Tbl_Lottery_Table::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx );
			return null;
		}
		
		return m_RecordList[iIdx];
	}	
}
