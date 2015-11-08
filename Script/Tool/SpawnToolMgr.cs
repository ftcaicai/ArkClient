using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;



public class SpawnToolMgr : MonoBehaviour 
{
	
	private TerrainMgr m_terrainMgr = null;
	private AsTableManager m_tableManager = null;
	private Dictionary<int, SpawnGroupRecord> m_SpawnGroupList = new Dictionary<int, SpawnGroupRecord>();
	private List<SpawnData> m_SpawnDownList = new List<SpawnData>();
	private List<QuestRecallRecord> m_QuestRecallList = new List<QuestRecallRecord>();
	static SpawnToolMgr m_instance;	
	private GameObject spawnDataParent;
	
	private bool m_bSaveLoadXMLEnable = true;
	
	

	public static SpawnToolMgr Instance
	{
		get
		{	
			if (m_instance == null)
		    {
		        m_instance = (SpawnToolMgr)FindObjectOfType(typeof(SpawnToolMgr));
		        if (m_instance == null)
				{
					m_instance = (new GameObject("SpawnToolMgr")).AddComponent<SpawnToolMgr>();
					m_instance.CreateComponent();				
				}
		    }	
			return m_instance;
		}
	}
	
	public List<SpawnData> getSpawnDataList
	{
		get
		{
			return m_SpawnDownList;
		}
	}
	public TerrainMgr getTerrainMgr
	{
		get
		{
			return m_terrainMgr;
		}		
	}
	
	public AsTableManager getTableMgr
	{
		get
		{
			return m_tableManager;
		}			
	}
	
	public void CreateComponent()
	{
		if( null != m_terrainMgr )
		{
			GameObject.DestroyImmediate( m_terrainMgr.gameObject );
		}
		
		if( null != m_tableManager )
		{
			GameObject.DestroyImmediate( m_tableManager.gameObject );
		}
		
		if( null == spawnDataParent )
		{
			spawnDataParent = new GameObject("parent");
			spawnDataParent.transform.parent = m_instance.transform;
			
			//GameObject.DestroyImmediate( spawnDataParent );
		}
		
		
		
		GameObject goTerrainMgr = new GameObject("TerrainMgr");					
		GameObject goTableMgr = new GameObject("AsTableManager");
		goTerrainMgr.transform.parent = m_instance.transform;
		goTableMgr.transform.parent = m_instance.transform;
		
		m_instance.SetTerrainMgr( goTerrainMgr.gameObject.AddComponent<TerrainMgr>() );
		m_instance.LoadSpawnGroupTable("Table/SpawnGroupTable");
		m_instance.LoadQuestRecallTable("Table/QuestRecallTable");
		m_instance.SetTableMgr( goTableMgr.gameObject.AddComponent<AsTableManager>() );
	}
	
	public void Clear()
	{
		if( null != spawnDataParent )
		{		
			GameObject.DestroyImmediate( spawnDataParent );
		}
		
		spawnDataParent = new GameObject("parent");
		spawnDataParent.transform.parent = m_instance.transform;
		m_SpawnDownList.Clear();
	}
		
	
	public void Refresh()
	{
		CreateComponent();
	}
	
	
	public void SetTableMgr( AsTableManager mgr )
	{
		if( null == mgr )
		{
			Debug.LogError("SpawnToolMgr::SetTableMgr()[ null == AsTableManager ] ");
			return;
		}
		m_tableManager = mgr;
		m_tableManager.SpawnToolLoadTable();
		
	}
	
	public SpawnGroupRecord GetSpawnGroupRecord( int iIndex )
	{
		if( false == m_SpawnGroupList.ContainsKey( iIndex ) )
		{			
			return null;
		}
		
		return m_SpawnGroupList[ iIndex ];
	}
	
	
	public void SetTerrainMgr( TerrainMgr mgr )
	{		
		m_terrainMgr = mgr;		
		m_terrainMgr.LoadTable();		
	}
	
	public void LoadSpawnGroupTable( string strPath )
	{
		m_SpawnGroupList.Clear();
		
		try
		{
	        XmlElement root = AsTableBase.GetXmlRootElement( strPath );
	        XmlNodeList nodes = root.ChildNodes;	        
	
	        foreach (XmlNode node in nodes)
	        {              
	            SpawnGroupRecord data = new SpawnGroupRecord( node as XmlElement );  
				
				int id = data.getIndex;
				if( true == m_SpawnGroupList.ContainsKey( id ) )
				{
					Debug.LogError( " m_SpawnGroupList exist to same ID : " + id ); 
					continue;
				}
				
				m_SpawnGroupList.Add( id, data );				
	        }
		
		}
		catch(System.Exception e)
		{
			Debug.LogError( e.ToString() );
		}
	}
	
	
	
	public SpawnData CreateNpcObject( int iGroupID, Vector3 vec3Pos, float fRot  )
	{				
		if( false == m_SpawnGroupList.ContainsKey( iGroupID ) )
		{
			Debug.LogError("SpawnToolMgr::CreateNpcObject() [ false == m_SpawnGroupList.ContainsKey( iGroupID ) ]");
			return null;
		}
			
		SpawnGroupRecord record = m_SpawnGroupList[iGroupID];
		
		if( 0 >= record.getData.Count )
		{
			Debug.LogError("SpawnToolMgr::CreateNpcObject() [ 0 >= record.getData.Count ]");
			return null;
		}
				
		int iFirsetNpcID = record.getData[0].iNpcID;
		Tbl_Npc_Record tabledata = m_tableManager.GetTbl_Npc_Record( iFirsetNpcID );
		if( null == tabledata )
		{
			Debug.LogError( "not find Tbl_Npc_Record in Npc table [ npc id : " + iFirsetNpcID );	
			return null;
		}		
		
		
			
		string strPath = tabledata.ModelingPath;
		GameObject goObj = Resources.Load( strPath ) as GameObject;
		if( null != goObj )
		{		
			GameObject goCopy = GameObject.Instantiate( goObj ) as GameObject;
			goCopy.transform.parent = spawnDataParent.transform;
			goCopy.transform.localPosition = Vector3.zero;
			goCopy.transform.localScale = Vector3.one;
			goCopy.transform.localRotation = Quaternion.identity;
			
			goCopy.name = record.getData[0].iNpcID.ToString();
			SpawnData data = goCopy.AddComponent<SpawnData>();	
			data.groupID = iGroupID;
			data.SetRotate( fRot );		
			data.Create();
			
			goCopy.transform.position = vec3Pos;			
			goCopy.transform.localScale = new Vector3( tabledata.Scale, tabledata.Scale, tabledata.Scale );
			m_SpawnDownList.Add( data );
			
			Tbl_Npc_Record npcRecord = m_tableManager.GetTbl_Npc_Record( iFirsetNpcID );
			if( null != npcRecord )				
			{
				if( eNPCType.Monster ==  npcRecord.NpcType )
				{
					Tbl_Monster_Record mobRecourd = m_tableManager.GetTbl_Monster_Record( iFirsetNpcID );
					if( null != mobRecourd )
					{
						data.chaseDistance = mobRecourd.ChaseDistance  * 0.01f;
						data.viewDistance = mobRecourd.ViewDistance  * 0.01f ;
					}
				}
			}
			else
			{
				Debug.LogError("SpawnToolMgr::CreateNpcObject()[null == Tbl_Npc_Record] id : " + iFirsetNpcID );
			}
			
			return data;
			
		}
		
		Debug.LogError( "Load failed [ path : "  + tabledata.ModelingPath );		
		return null;
	}

	public int GetSpawnGroupId( int _monsterId )
	{
		foreach (KeyValuePair<int, SpawnGroupRecord> pair in m_SpawnGroupList) 
		{
			SpawnGroupRecord	_record = pair.Value;
			if( _record.IsThereSpawnGroup( _monsterId ) == true )
				return _record.getIndex;
		}
		return 0;
	}

	public void LoadQuestRecallTable( string strPath )
	{
		m_QuestRecallList.Clear();
		
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( strPath );
			XmlNodeList nodes = root.ChildNodes;	        
			
			foreach (XmlNode node in nodes)
			{              
				QuestRecallRecord data = new QuestRecallRecord( node as XmlElement );  
				m_QuestRecallList.Add(data);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError( e.ToString() );
		}
	}

	public bool IsThereQuestRecall( int _monsterId )
	{
		foreach (QuestRecallRecord _record in m_QuestRecallList) 
		{
			if( _record.getNpcIndex == _monsterId )
				return true;
		}
		return false;
	}

	
	public bool SaveXML( string strPath )
	{
		if( false == m_bSaveLoadXMLEnable )
			return false;
		
		XmlWriterSettings wrapperSettings = new XmlWriterSettings();
		wrapperSettings.Indent = true;		
	
		string wrappername = Application.dataPath + "/Resources/" + strPath;
		
		
		
		SpawnData[] npcList =  spawnDataParent.GetComponentsInChildren<SpawnData>();
		
		using ( XmlWriter writer = XmlWriter.Create (wrappername, wrapperSettings) ) 
		{
			writer.WriteStartDocument();
            writer.WriteStartElement ("SpawnDataExport");
			
			foreach( SpawnData _data in npcList )
			{				
				_data.WriteXml( writer );
			}
			
			writer.WriteEndElement ();			       
            writer.Close ();        
		}
		
		
		m_bSaveLoadXMLEnable = true;
		
		return true;
	}
	
	public bool LoadXML( string strPath )
	{
		this.Clear();
		
		if( false == m_bSaveLoadXMLEnable )
			return false;
		
		XmlElement root = AsTableBase.GetXmlRootElement( strPath );
		if( null == root ) 
		{
			Debug.LogError("can not found Table/LoadXML" );			
			return false;
		}
	    XmlNodeList nodes = root.ChildNodes;
		
		foreach (XmlNode node in nodes)
	    { 
			XmlElement _element = (XmlElement)node;
			int groupID = int.Parse(_element["GroupID"].InnerText);
			
			SpawnData _spawnData = CreateNpcObject( groupID, Vector3.zero, 0.0f );
			_spawnData.ReadXml( _element );			
		}
			
			
		m_bSaveLoadXMLEnable = true;
		
		return true;
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}




//---------------------------------------------------------------------------
/* SpawnGroupRecord */
//---------------------------------------------------------------------------
public class SpawnGroupRecord : AsTableRecord
{
	
	public class CData
	{
		public int iNpcID;
		public int iPercent;
	}
	//---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------	
	private int m_iID = 0;
	private List<CData> m_DataList = new List<CData>();
	
	
	public int getIndex
	{
		get
		{
			return m_iID;
		}
	}
	
	public List<CData> getData
	{
		get
		{
			return m_DataList;
		}
	}

	
	//---------------------------------------------------------------------
    /* Function */
    //---------------------------------------------------------------------
	// Read
	public SpawnGroupRecord(XmlElement _element)
    {
        try
        {				
			XmlNode node = (XmlElement)_element;	
			
			SetValue( ref m_iID, node, "Index" );			
			for( int i=0; i<5; ++i )
			{
				CData data = new CData();
				SetValue( ref data.iNpcID , node, "MonsterID_" + (i+1) );
				SetValue( ref data.iPercent, node, "Prob_" + (i+1) );
				
				if( 0 == data.iNpcID || int.MaxValue == data.iNpcID )
					continue;
				
				m_DataList.Add( data );
			}			
        } 
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

	public bool IsThereSpawnGroup( int _monsterId )
	{
		foreach( CData _data in m_DataList )
		{
			if( _data.iNpcID == _monsterId )
				return true;
		}

		return false;
	}

}




//---------------------------------------------------------------------------
/* QuestRecallRecord */
//---------------------------------------------------------------------------
public class QuestRecallRecord : AsTableRecord
{
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------	
	private int m_iID = 0;
	private int m_iNpcID = 0;

	public int getIndex
	{
		get{return m_iID;}
	}

	public int getNpcIndex
	{
		get{return m_iNpcID;}
	}

	//---------------------------------------------------------------------
	/* Function */
	//---------------------------------------------------------------------
	// Read
	public QuestRecallRecord(XmlElement _element)
	{
		try
		{				
			XmlNode node = (XmlElement)_element;	
			
			SetValue( ref m_iID, node, "Index" );			
			SetValue( ref m_iNpcID, node, "NPCIndex" );
		} 
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}
}










