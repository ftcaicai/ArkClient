using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_String_Record
{
	int m_Index;
	public int Index { get { return m_Index; } }
	string m_String;
	public string String { get { return m_String; } }

	public Tbl_String_Record( XmlNode node)
	{
		try
		{
			m_Index = int.Parse( node["Index"].InnerText);
			m_String = node["String"].InnerText;
		}
		catch( System.Exception e)
		{
			Debug.LogError( "[Tbl_String_Record] 'constructor': |" + e + "| error while parsing");
		}
	}

	public Tbl_String_Record( BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_String = br.ReadString();
	}
}

public class Tbl_String_Table : AsTableBase
{
	Dictionary<int, Tbl_String_Record> m_StringTable = new Dictionary<int, Tbl_String_Record>();

	public Tbl_String_Table( string _path)
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
			Tbl_String_Record record = new Tbl_String_Record( node);
			m_StringTable.Add( record.Index, record);
		}
	}

	public Tbl_String_Record GetRecord( int _id)
	{
		if( m_StringTable.ContainsKey( _id) == true)
			return m_StringTable[_id];

		Debug.LogWarning( "[Tbl_String_Base_Table]GetRecord: there is no [" + _id + "] record");
		return null;
	}
}
