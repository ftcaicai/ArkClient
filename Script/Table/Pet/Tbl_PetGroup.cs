using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public enum eGroupType {NONE = 0, Person, Passive, Active, Special}

public class Tbl_PetGroup_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	eGroupType m_GroupType;public eGroupType GroupType{get{return m_GroupType;}}
	
	List<int> m_listRandID = new List<int>();public List<int> listRandID{get{return m_listRandID;}}
	List<int> m_listRandProb = new List<int>();public List<int> listRandProb{get{return m_listRandProb;}}
	
	public Tbl_PetGroup_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "GroupID");
			SetValue<eGroupType>(ref m_GroupType, node, "GroupType");
			
			for(int i=1; i<=AsTableManager.sPetGroupCount; ++i)
			{
				int id = 0;
				SetValue(ref id, node, "RandID_" + i);
				m_listRandID.Add(id);
				
				int prob = 0;
				SetValue(ref prob, node, "RandProb_" + i);
				m_listRandProb.Add(prob);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetGroup_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_GroupType = (eGroupType)br.ReadInt32();
			
		for(int i=1; i<=AsTableManager.sPetGroupCount; ++i)
		{
			int id = br.ReadInt32();
			m_listRandID.Add(id);
			
			int prob = br.ReadInt32();
			m_listRandProb.Add(prob);
		}
	}
}

public class Tbl_PetGroup_Table : AsTableBase {
	
	SortedList<int, Tbl_PetGroup_Record> m_ResourceTable = 
		new SortedList<int, Tbl_PetGroup_Record>();
	
	public Tbl_PetGroup_Table(string _path)
	{
		m_TableType = eTableType.NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try
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
					Tbl_PetGroup_Record record = new Tbl_PetGroup_Record( br);
					m_ResourceTable.Add(record.Index, record);
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
					Tbl_PetGroup_Record record = new Tbl_PetGroup_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}
