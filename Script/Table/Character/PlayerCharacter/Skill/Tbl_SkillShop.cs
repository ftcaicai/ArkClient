using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_SkillShop_Record : AsTableRecord
{	
	int m_Index;public int Index{get{return m_Index;}}
	int m_NPC_Index;public int NPC_Index{get{return m_NPC_Index;}}
	int m_SkillBook_Index;public int SkillBook_Index{get{return m_SkillBook_Index;}}
	
	public Tbl_SkillShop_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_NPC_Index, node, "NPC_Index");
			SetValue(ref m_SkillBook_Index, node, "SkillBook_Index");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);//
		}
	}

	public Tbl_SkillShop_Record(BinaryReader br)
	{
		m_Index = br.ReadInt32();
		m_NPC_Index = br.ReadInt32();
		m_SkillBook_Index = br.ReadInt32();
	}
}

public class Tbl_SkillShop_Table : AsTableBase {
	SortedList<int, Tbl_SkillShop_Record> m_ResourceTable = 
		new SortedList<int, Tbl_SkillShop_Record>();
	
	MultiDictionary<int, Tbl_SkillShop_Record> m_mdicNpcIdx = 
		new MultiDictionary<int, Tbl_SkillShop_Record>();
	
	public Tbl_SkillShop_Table(string _path)
	{
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
					Tbl_SkillShop_Record record = new Tbl_SkillShop_Record( br);
					m_ResourceTable.Add(record.Index, record);
					
					m_mdicNpcIdx.Add(record.NPC_Index, record);
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
					Tbl_SkillShop_Record record = new Tbl_SkillShop_Record((XmlElement)node);
					m_ResourceTable.Add(record.Index, record);
					
					m_mdicNpcIdx.Add(record.NPC_Index, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_SkillShop_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_Skill_Table]GetRecord: there is no [" + _id + "] record");
		return null;
	}
	
	public List<Tbl_SkillBook_Record> GetRecordsByClass(int _npcIdx, eCLASS _class)
	{
		if(m_mdicNpcIdx.ContainsKey(_npcIdx) == true)
		{
			List<Tbl_SkillBook_Record> extractedList = new List<Tbl_SkillBook_Record>();
			
			foreach(Tbl_SkillShop_Record shop in m_mdicNpcIdx[_npcIdx])
			{
				Tbl_SkillBook_Record record = AsTableManager.Instance.GetTbl_SkillBook_Record(shop.SkillBook_Index);
				
				if(record.Class == _class)
					extractedList.Add(AsTableManager.Instance.GetTbl_SkillBook_Record(shop.SkillBook_Index));
			}
			
			return extractedList;
		}
		
//		Debug.LogError("[Tbl_Skill_Table]GetRecord: there is no [" + _npcId + "] record");
		return null;
	}
	
	public SortedList<int, Tbl_SkillShop_Record> GetList()
	{
		return m_ResourceTable;
	}
}
