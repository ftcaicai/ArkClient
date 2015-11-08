using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_PetSkill_Record : AsTableRecord
{
	int m_Index;public int Index{get{return m_Index;}}
	
	List<int> m_listSkillID = new List<int>();public List<int> listSkillID{get{return m_listSkillID;}}
	List<int> m_listRandRatio = new List<int>();public List<int> listRandRatio{get{return m_listRandRatio;}}
	
	public Tbl_PetSkill_Record(XmlElement _element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "GroupID");
			
			for(int i=1; i<=AsTableManager.sPetSkillCount; ++i)
			{
				int id = 0;
				SetValue(ref id, node, "SkillID_" + i);
				m_listSkillID.Add(id);
				
				int prob = 0;
				SetValue(ref prob, node, "RandRatio_" + i);
				m_listRandRatio.Add(prob);
			};
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_PetSkill_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
			
		for(int i=1; i<=AsTableManager.sPetSkillCount; ++i)
		{
			int id = br.ReadInt32();
			m_listSkillID.Add(id);
			
			int prob = br.ReadInt32();
			m_listRandRatio.Add(prob);
		}
	}
}

public class Tbl_PetSkill_Table : AsTableBase {
	
	SortedList<int, Tbl_PetSkill_Record> m_ResourceTable = 
		new SortedList<int, Tbl_PetSkill_Record>();
	
	public Tbl_PetSkill_Table(string _path)
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
					Tbl_PetSkill_Record record = new Tbl_PetSkill_Record( br);
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
					Tbl_PetSkill_Record record = new Tbl_PetSkill_Record((XmlElement)node);
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
