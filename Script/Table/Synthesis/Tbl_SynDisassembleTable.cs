using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_SynDisassemble_Record : AsTableRecord
{		
	
	public int level;
	public Item.eGRADE grade;
	public Item.eEQUIP equip;
	public int step;
	public eItem_Disassemble type;
		
	public int cost = 0;
	
	
	public Tbl_SynDisassemble_Record(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;				
			
			SetValue( ref level, node, "Level" );		
			SetValue<Item.eGRADE>(ref grade, node, "Grade");
			SetValue<Item.eEQUIP>(ref equip, node, "Part");
			SetValue( ref step, node, "Step" );		
			SetValue<eItem_Disassemble>(ref type, node, "Type");
			         
			SetValue( ref cost, node, "Cost" );			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_SynDisassemble_Record] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}	

	public Tbl_SynDisassemble_Record(BinaryReader br)
	{
		level = br.ReadInt32();
		grade =(Item.eGRADE) br.ReadInt32();
		equip = (Item.eEQUIP)br.ReadInt32();
		step = br.ReadInt32();
		type = (eItem_Disassemble)br.ReadInt32();
		cost = br.ReadInt32();
	}
}

public class Tbl_SynDisassemble_Table : AsTableBase 
{
	
	List<Tbl_SynDisassemble_Record> m_recordList = new List<Tbl_SynDisassemble_Record>();
	
	public Tbl_SynDisassemble_Table(string _path)
	{
		m_TableType = eTableType.STRING;		
		LoadTable(_path);
	}
	 
	public override void LoadTable(string _path)
	{		
		if ((null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || true == AsTableManager.Instance.useReadBinary) 
		{
			// Ready Binary
			TextAsset textAsset = ResourceLoad.LoadTextAsset (_path);
			MemoryStream stream = new MemoryStream (textAsset.bytes);
			BinaryReader br = new BinaryReader (stream);

			int nCount = br.ReadInt32 ();

			for (int i = 0; i < nCount; i++) {
				Tbl_SynDisassemble_Record record = new Tbl_SynDisassemble_Record (br);
					m_recordList.Add (record);		
			}

			br.Close ();
			stream.Close ();				
		} 
		else 
		{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_SynDisassemble_Record record = new Tbl_SynDisassemble_Record((XmlElement)node);			
				m_recordList.Add( record );
			}		
		}
	}		 
	
	public Tbl_SynDisassemble_Record GetRecord( int _level, Item.eGRADE _eGrade, Item.eEQUIP _equip, int _step, eItem_Disassemble _type )
	{		
		foreach( Tbl_SynDisassemble_Record _record in m_recordList )
		{		
			if( _record.level == _level && _record.grade == _eGrade && _record.equip == _equip && _record.step == _step && _record.type == _type )
				return _record;
		}
		
		return null;
	}
	
}
