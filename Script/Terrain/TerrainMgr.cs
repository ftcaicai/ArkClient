
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class TerrainMgr : MonoBehaviour
{
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------
	
	private static TerrainMgr ms_kIstance = null; // Instance
	private Dictionary<int, Map> m_MapList = new Dictionary<int, Map>(); // ID, Map
	private int m_iCurMapID = 0;
	private MapNameShow m_mapNameShow = null;
	private bool m_isNeedTerrainView = false;
	
	//---------------------------------------------------------------------
	/* function */
	//---------------------------------------------------------------------
	
	public bool isNeedTerrainView 
	{
		get
		{
			return m_isNeedTerrainView;
		}
		
		set			
		{
			m_isNeedTerrainView = value;
		}
	}
	public static TerrainMgr Instance
	{
		get	{ return ms_kIstance; }
	}
	
	public void Clear()
	{
		m_iCurMapID = 0;
	}

	public int GetCurMapID()
	{
		return m_iCurMapID;
	}
	
	public void SetCurMapID( int iMapId)
	{
		m_iCurMapID = iMapId;
	}

	public Map GetMap( int iID)
	{
		if( true == m_MapList.ContainsKey( iID))
			return m_MapList[ iID ];

		return null;
	}
	
	public Map GetMap( string strSceneName)
	{
		foreach( KeyValuePair<int, Map> iter in m_MapList)
		{
			if( iter.Value.MapData.GetPath().ToUpper() == strSceneName.ToUpper())
				return iter.Value;
		}
		
		return null;
	}

	public static Map GetMap( string strSceneName, Dictionary<int, Map>  mapList)
	{
		foreach( KeyValuePair<int, Map> iter in mapList)
		{
			if( iter.Value.MapData.GetPath().ToUpper() == strSceneName.ToUpper())
				return iter.Value;
		}
		
		return null;
	}
	
	public Map GetCurrentMap()
	{
		if( false == m_MapList.ContainsKey( m_iCurMapID))
			return null;
		
		return m_MapList[ m_iCurMapID];
	}

	public bool IsLoadedTable()
	{
		return ( 0 < m_MapList.Count);
	}
		
	public bool IsCurMapType( eMAP_TYPE _mapType)
	{
		Map _map = GetCurrentMap();
		if( null == _map)
			return false;

		return _map.MapData.getMapType == _mapType;
	}

	// Load
	public static bool LoadTable( string strPath, Dictionary<int, Map> maplist)
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( strPath);
			XmlNodeList nodes = root.ChildNodes;
	
			foreach ( XmlNode node in nodes)
			{
				Map map = new Map( node as XmlElement);
				
				int id = map.MapData.GetID();
				if( true == maplist.ContainsKey( id))
				{
					Debug.LogError( " m_MapList exist to same ID : " + id);
					return false;
				}
				
				maplist.Add( id, map);
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( e.ToString());
		}
		
		return true;
	}

	public void LoadMap( int iMapIndex)
	{
		NavMeshFinder.Instance.LoadNavMesh( iMapIndex);
		NavMeshFinder.Instance.SetNavMesh( iMapIndex);
		SetCurMapID( iMapIndex);
	}
	
	public void Initialize( int iMapIndex)
	{
		AsHUDController.Instance.Init();
		AsHudDlgMgr.Instance.Init( false);
		AsHUDController.Instance.StartTutorial();
		AsHudDlgMgr.Instance.OpenWorldMapDlg();
		PlayerBuffMgr.Instance.SetShowUI();
		QuestHolderManager.instance.MakeQuestHolder( iMapIndex);
		AsHudDlgMgr.Instance.questCompleteMsgManager.ResetForChangeMap();
	}
	
	private GameObject prevMapName = null;
	public void ShowMapNameShow( int iMapIndex)
	{
		if( null != prevMapName)
			Destroy( prevMapName);
		
		GameObject goMapNameShow = ResourceLoad.CreateGameObject( "UI/AsGUI/MapNameShow", AsHUDController.Instance.gameObject.transform);
		Debug.Assert( null != goMapNameShow);
		prevMapName = goMapNameShow;
		
		Map _curMap = GetMap( iMapIndex);
		m_mapNameShow = goMapNameShow.GetComponent<MapNameShow>();
		if( null!= m_mapNameShow && null != _curMap)
		{
			m_mapNameShow.Open( AsTableManager.Instance.GetTbl_String( _curMap.MapData.GetNameStrIdx()), _curMap.MapData.GetMapNameShowTime());
			_curMap.PlayMapBegin();
		}

		if( eMAP_TYPE.Indun == _curMap.MapData.getMapType)
		{
			AsChatManager.Instance.InsertSystemChat( 
				string.Format( AsTableManager.Instance.GetTbl_String( 1735), AsTableManager.Instance.GetTbl_String( _curMap.MapData.GetNameStrIdx())),
				eCHATTYPE.eCHATTYPE_SYSTEM);
		}
		
		if( _curMap.MapData.getMapType != eMAP_TYPE.Town && 
			_curMap.MapData.getMapType != eMAP_TYPE.Tutorial && 
			_curMap.MapData.getMapType != eMAP_TYPE.Pvp &&
			_curMap.MapData.getMapType != eMAP_TYPE.Summon)
		{
			m_isNeedTerrainView = true;
			AsHudDlgMgr.Instance.PromptRankChangeAlarm();
		}
	}
	
	bool m_isNeedReviewEventOpenDlg = false;
	public void CheckReviewEvent()
	{
		if ( false == m_isNeedReviewEventOpenDlg)
			return;
		
		if( null != m_mapNameShow)
			return;
		
		Map _map = GetCurrentMap();
		if( null == _map)
			return;
		
		if( eMAP_TYPE.Tutorial == _map.MapData.getMapType)
			return;
			
		AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( 	AsTableManager.Instance.GetTbl_String( 1380),
																		AsTableManager.Instance.GetTbl_String( 1381),
																		AsTableManager.Instance.GetTbl_String( 1385),
																		AsTableManager.Instance.GetTbl_String( 1151),
																		this, "OkReviewEvent", "CancelReviewEvent",
																		AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
		m_isNeedReviewEventOpenDlg = false;
	} 
	
	public void OpenReviewEventDlg()
	{
		m_isNeedReviewEventOpenDlg = true;
	}

	public void CancelReviewEvent()
	{
		AsCommonSender.SendGameReviewEvent( false);
	}
	
	public void OkReviewEvent()
	{
		Debug.Log( "reviewUrl : " + WemeGateManager.reviewUrl);
		Application.OpenURL( WemeGateManager.reviewUrl);
		AsCommonSender.SendGameReviewEvent( true);
	}

	// check
	bool CheckInsertMap( int id)
	{
		if( null != GetMap( id))
		{
			Debug.LogError( " m_MapList exist to same ID : " + id);
			return false;
		}

		return true;
	}

	public void LoadTable()
	{
		if( false == LoadTable( "Table/MapTable", m_MapList))
			Debug.LogError( "MapTable Read failed");
	}

	// Get
	public static float GetTerrainHeight( CharacterController characterController, Vector3 _pos)
	{
		if( null == characterController)
		{
			Debug.LogError( "TerrainMgr::GetTerrainHeight() [ null == characterController ]");
			return 0.0f;
		}

		Vector3 vec3Temp = _pos;
		vec3Temp.y = 100.0f;
		
		Ray ray = new Ray( vec3Temp, Vector3.down);
		RaycastHit hit;
		
		if( Physics.Raycast( ray, out hit, 10000f, 1<<LayerMask.NameToLayer( "Terrain")) == true)
		{
			float fCharCtlerHeight = 0.0f;
			float fCenter = -characterController.center.y;
			
			if( characterController.height > ( characterController.radius+characterController.radius))
				fCharCtlerHeight = fCenter + ( characterController.height/2.0f);
			else
				fCharCtlerHeight = fCenter + characterController.radius;

			fCharCtlerHeight *= characterController.transform.localScale.y;
			float fHeight = fCharCtlerHeight +  hit.point.y + 0.02f;

			return fHeight;
		}

		return _pos.y;
	}
	
	public static float GetTerrainHeight( Vector3 _pos)
	{
		Vector3 vec3Temp = _pos;
		vec3Temp.y = 100.0f;
		
		Ray ray = new Ray( vec3Temp, Vector3.down);
		RaycastHit hit;
		
		if( Physics.Raycast( ray, out hit, 10000f, 1<<LayerMask.NameToLayer( "Terrain")) == true)
		{
			float fHeight = hit.point.y + 0.001f;
			return fHeight;
		}
		
		Debug.LogWarning( "Physics.Raycast failed [ collider ]");
		
		return 0.0f;
	}

	// Awake
	void Awake()
	{
		ms_kIstance = this;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		TerrainMgr.Instance.CheckReviewEvent();
	}
}
