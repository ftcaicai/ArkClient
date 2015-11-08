using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Npc_Record : AsTableRecord
{
	int m_Id;public int Id{get{return m_Id;}}
	public int m_iNpcNameId = 0;
	public string NpcName
	{
		get
		{
			if( int.MaxValue == m_iNpcNameId )
				return "NONE";
			
			if( null == AsTableManager.Instance )
			{			
				return string.Empty;
			}
			
			return AsTableManager.Instance.GetTbl_String(m_iNpcNameId);
		}
	}	
	public int m_iNpcGNameId = 0;
	public string NpcGName
	{
		get
		{			
			if( int.MaxValue == m_iNpcGNameId )
				return "NONE";
			
			return AsTableManager.Instance.GetTbl_String(m_iNpcGNameId);
		}
	}	
	//string m_NpcName;public string NpcName{get{return m_NpcName;}}
	//string m_NpcGName;public string NpcGName{get{return m_NpcGName;}}
	eNPCType m_NpcType;public eNPCType NpcType{get{return m_NpcType;}}
	bool m_SpawnCheck;public bool SpawnCheck{get{return m_SpawnCheck;}}
	int m_SpawnTimeMin;public int SpawnTimeMin{get{return m_SpawnTimeMin;}}
	int m_SpawnTimeMax;public int SpawnTimeMax{get{return m_SpawnTimeMax;}}
	int m_LineIndex; public int LineIndex	{ get { return m_LineIndex; } }
	string m_ModelingPath;public string ModelingPath{get{return m_ModelingPath;}}
	int m_NpcIcon;public int NpcIcon{get{return m_NpcIcon;}}
	float m_fScale;public float Scale{ get{ return m_fScale;}}
	float m_fPointScale;public float PointScale{ get{ return m_fPointScale;}}
	float m_fCollisionRadius; public float getCollisionRadius{ get { return m_fCollisionRadius; } }
	bool m_UseCheck;public bool UserCheck{get{return m_UseCheck;}}
	float m_fOrgSize;public float OrgSize{ get{ return m_fOrgSize;}}
	int m_RegenString;public int RegenString{get{return m_RegenString;}}
	string m_strNickColor; public string NickColor{ get{ return m_strNickColor;}}
	int m_warpIndex = 0; public int warpIndex{ get{ return m_warpIndex;}}
	
	public Tbl_Npc_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref m_Id, node, "ID");			
			SetValue(ref m_iNpcNameId, node, "NpcName");
			SetValue(ref m_iNpcGNameId, node, "NpcGName");
			SetValue<eNPCType>(ref m_NpcType, node, "NpcType");
			SetValue(ref m_SpawnCheck, node, "SpawnCheck");
			SetValue(ref m_SpawnTimeMin, node, "SpawnTime_Min");
			SetValue(ref m_SpawnTimeMax, node, "SpawnTime_Max");
			SetValue( ref m_LineIndex, node, "ChatBalloonIndex");
			SetValue(ref m_ModelingPath, node, "ModelingPath");
			SetValue(ref m_NpcIcon, node, "NpcIcon");
			SetValue(ref m_fScale, node, "Scale");
			SetValue(ref m_fPointScale, node, "PointScale");
			SetValue(ref m_fCollisionRadius, node, "CollisionRadius");			
			SetValue(ref m_UseCheck, node, "UseCheck");
			SetValue(ref m_fOrgSize, node, "OrgSize");
			SetValue(ref m_RegenString, node, "RegenString");
			SetValue(ref m_strNickColor, node, "NickColor");
			SetValue(ref m_warpIndex, node, "WarpIndex");			
			
			
			m_fCollisionRadius = m_fCollisionRadius/ 100.0f;
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}	
	
	public Tbl_Npc_Record(BinaryReader br)
	{
		m_Id = br.ReadInt32();
		m_iNpcNameId = br.ReadInt32();
		m_iNpcGNameId = br.ReadInt32();
		m_NpcType = (eNPCType)br.ReadInt32();
		m_SpawnCheck = br.ReadBoolean();
		m_SpawnTimeMin = br.ReadInt32();		
		m_SpawnTimeMax = br.ReadInt32();	
		m_LineIndex = br.ReadInt32();	
		m_ModelingPath = br.ReadString();
		m_NpcIcon = br.ReadInt32();
		m_fScale = br.ReadSingle();	
		m_fPointScale = br.ReadSingle();
		m_fCollisionRadius = br.ReadSingle();
		m_UseCheck = br.ReadBoolean();
		m_fOrgSize = br.ReadSingle();
		m_RegenString = br.ReadInt32();
		m_strNickColor = br.ReadString();
		m_warpIndex = br.ReadInt32();
		
		m_fCollisionRadius = m_fCollisionRadius*0.01f;// / 100.0f;
	}
}

public class Tbl_Npc_Table : AsTableBase
{
    SortedList<int, Tbl_Npc_Record> m_ResourceTable =
        new SortedList<int, Tbl_Npc_Record>();

    public Tbl_Npc_Table(string _path)
    {
        //		m_TableName = "CharacterResource";
        m_TableType = eTableType.NPC;

        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try  
        {
			if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || 
				( null != AsTableManager.Instance && true == AsTableManager.Instance.useReadBinary ) )
			{	
				// Ready Binary
				TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
				MemoryStream stream = new MemoryStream( textAsset.bytes);
				BinaryReader br = new BinaryReader( stream);
				
				int nCount = br.ReadInt32();
					
		
				for( int i = 0; i < nCount; i++)
				{
					Tbl_Npc_Record record = new Tbl_Npc_Record( br);
					if(m_ResourceTable.ContainsKey(record.Id) == false)
	                	m_ResourceTable.Add(record.Id, record);
					else
						Debug.LogWarning("Tbl_Npc::LoadTable: There are same id(" + record.Id + ") xml node.");	
				}
				
				br.Close();
				stream.Close();	
			}
			else
			{
				
	            XmlElement root = GetXmlRootElement(_path);
	            XmlNodeList nodes = root.ChildNodes;
	
	            foreach (XmlNode node in nodes)
	            {
	                Tbl_Npc_Record record = new Tbl_Npc_Record((XmlElement)node);
					if(m_ResourceTable.ContainsKey(record.Id) == false)
	                	m_ResourceTable.Add(record.Id, record);
					else
						Debug.LogWarning("Tbl_Npc::LoadTable: There are same id(" + record.Id + ") xml node.");
	            }
			}
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public Tbl_Npc_Record GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
        {
            return m_ResourceTable[_id];
        }

        Debug.LogError("[Tbl_Npc_Table]GetRecord: there is no record. id = " + _id);
        return null;
    }

    public SortedList<int, Tbl_Npc_Record> GetList()
    {
        return m_ResourceTable;
    }

    //	public Tbl_Npc_Record GetRecordByTribeAndClass(int _tribe, int _class)
    //	{
    //		foreach(KeyValuePair<int, Tbl_Npc_Record> pair in m_ResourceTable)
    //		{
    //			if(pair.Value.Race == _tribe)
    //			{
    //				if(pair.Value.Class == _class)
    //				{
    //					return pair.Value;
    //				}
    //			}
    //		}
    //		
    //		Debug.LogError("Tbl_Npc_Table::GetRecordByTribeAndClass: no record");
    //		return null;
    //	}


}