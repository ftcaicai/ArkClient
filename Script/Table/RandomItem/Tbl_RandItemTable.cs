using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;


public class Tbl_RandItem_Record : AsTableRecord
{		
	public int Index = 0;
	public List<int> idlist = new List<int>();
	public List<int> quentities = new List<int>();
	
	public Tbl_RandItem_Record(XmlElement _element)
	{
		
		System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();
		System.Text.StringBuilder sbQuentity = new System.Text.StringBuilder();
		
		try
		{
			XmlNode node = (XmlElement)_element;
		
			SetValue( ref Index, node, "Rand_Index" );
			for( int i=1; i<=50; ++i )
			{
				int _iIndex = 0;
				sbTemp.Length = 0;
				sbTemp.Append( "Rand_Item_ID" );
				sbTemp.Append( i );
				SetValue( ref _iIndex, node, sbTemp.ToString() );

				int _iQuantity = 0;
				sbQuentity.Length = 0;
				sbQuentity.Append("Rand_Item_Set");
				sbQuentity.Append(i);
				SetValue(ref _iQuantity, node, sbQuentity.ToString());
				
				if( 0 != _iIndex && int.MaxValue != _iIndex )
				{
					idlist.Add( _iIndex ); 
					quentities.Add(_iQuantity);
				}
				else 
					break;
			}	
			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_RandItem_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
	
	
	public Tbl_RandItem_Record(BinaryReader br)
	{
		Index = br.ReadInt32();
		
		int icount = br.ReadInt32();
		for( int i=0; i<icount; ++i )
		{			
			idlist.Add( br.ReadInt32() );
 			quentities.Add( br.ReadInt32());
		}
	}
}

public class Tbl_RandItem_Table : AsTableBase 
{	
	Dictionary<int, Tbl_RandItem_Record> m_RecordList = new Dictionary<int, Tbl_RandItem_Record>();
	
		
	
	public Tbl_RandItem_Table(string _path)
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
				Tbl_RandItem_Record record = new Tbl_RandItem_Record( br);
				m_RecordList.Add( record.Index, record );		
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
				Tbl_RandItem_Record record = new Tbl_RandItem_Record((XmlElement)node);
				
				if( true == m_RecordList.ContainsKey( record.Index ) )
				{
					AsUtil.ShutDown("Tbl_RandItem_Table::LoadTable()[true == m_RecordList.ContainsKey] index : " + record.Index );
					continue;
				}
				m_RecordList.Add( record.Index, record );			
			}
		}
	}		
	
	
	public Tbl_RandItem_Record GetRecord( int iIdx )
	{
		if( false == m_RecordList.ContainsKey( iIdx ) )
		{
			Debug.LogWarning("Tbl_RandItem_Table::GetRecord()[true == m_RecordList.ContainsKey] index : " + iIdx );
			return null;
		}
		
		return m_RecordList[iIdx];
	}	
}
