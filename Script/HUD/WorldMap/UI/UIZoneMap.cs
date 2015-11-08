using UnityEngine;
using System.Collections;

public class UIZoneMap : MonoBehaviour
{
	// image
	public SimpleSprite playerImg;
	public UIZoneImg[] partyImgs;

	// image path
	public string npcImgPath = "UI/WorldMap/Img_npc";
	public string potalImgPath = "UI/WorldMap/Img_portal";
	public string waypointImgPath = "UI/WorldMap/Img_waypoint";
	public string LocalImgPath = "UI/AsGUI/WorldMap/Object_WorldMapLocallImg";

	// layout
	public float npcLayout = -2.5f;
	public float potalLayout = -2f;
	public float mapLayout = - 1.5f;
	public float localLayout = - 1.5f;
	public float playerLayout = -1.1f;
	public float partyUserLayout = -1.4f;
	public Material []alphaMaterials;

	private SimpleSprite m_mapImg = null;
	private GameObject m_ObjectParent;
	private GameObject m_LocalParent;
	private GameObject m_NormalParent;


	// open & close
	public void Open( string strBackImgPath)
	{
		if( null == m_ObjectParent)
		{
			m_ObjectParent = new GameObject();
			m_ObjectParent.transform.parent = transform;
			m_ObjectParent.transform.localPosition = Vector3.zero;
			m_ObjectParent.transform.localRotation = Quaternion.identity;
			m_ObjectParent.transform.localScale = Vector3.one;
		}

		if( null == m_LocalParent)
		{
			m_LocalParent = new GameObject();
			m_LocalParent.transform.parent = transform;
			m_LocalParent.transform.localPosition = Vector3.zero;
			m_LocalParent.transform.localRotation = Quaternion.identity;
			m_LocalParent.transform.localScale = Vector3.one;
		}

		if( null == m_NormalParent)
		{
			m_NormalParent = new GameObject();
			m_NormalParent.transform.parent = transform;
			m_NormalParent.transform.localPosition = Vector3.zero;
			m_NormalParent.transform.localRotation = Quaternion.identity;
			m_NormalParent.transform.localScale = Vector3.one;
		}

		CrateMapImg( strBackImgPath);
	}

	public void Close()
	{
	}

	public void SetViewObject( bool isShow)
	{
		if( true == isShow)
		{
			m_ObjectParent.gameObject.SetActiveRecursively( true);
			if( null != m_LocalParent)
				m_LocalParent.gameObject.SetActiveRecursively( false);
		}
		else
		{
			m_ObjectParent.gameObject.SetActiveRecursively( false);
			if( null != m_LocalParent)
				m_LocalParent.gameObject.SetActiveRecursively( true);
		}
	}

	// clear
	public void ClearPartyPlayer()
	{
		foreach( UIZoneImg spriteData in partyImgs)
		{
			spriteData.gameObject.SetActiveRecursively( false);
		}
	}

	// Get
	public float getMapImgWidth
	{
		get
		{
			return m_mapImg.width;
		}
	}

	public float getMapImgHeight
	{
		get
		{
			return m_mapImg.height;
		}
	}

	public UIZoneImg SetUseItemInMap( Vector3 pos, Vector2 mapMin, Vector2 mapMax)
	{
		pos.z = npcLayout;

		UIZoneImg temp = CreateZoneImg( npcImgPath, pos, m_NormalParent.transform);

		temp.SetIconUV( 16, -0.09f);

		//temp.HideMapName();
		temp.SetMapNameSimple( AsTableManager.Instance.GetTbl_String( 963), new Color( 1.0f, 0.725f, 0.098f));

		float fPosX			= 0.0f;
		float fNameWidth	= temp.mapName.TotalWidth;

		if ( temp.transform.localPosition.x >= 0)
			fPosX = -1.0f * ( fNameWidth - temp.textBackImg.LeftFrameWidth);
		else
			fPosX = ( fNameWidth - temp.textBackImg.RightFrameWidth);

		temp.mapName.transform.localPosition = new Vector3( fPosX, 0.0f, temp.mapName.transform.localPosition.z);

		return temp;
	}

	public void UpdateQuestMark( UIZoneImg _img, Tbl_ZoneMap_Record.NpcData _data)
	{
		//_data.iNpcId
		QuestHolder questHolder = QuestHolderManager.instance.GetQuestHolder( _data.iNpcId);

		if ( questHolder != null)
		{
			switch ( questHolder.NowMarkType)
			{
				case QuestMarkType.HAVE:
				case QuestMarkType.HAVE_NPC_DAILY:
					_img.SetIconUV( 8, -0.06f); // !
					break;
				case QuestMarkType.CLEAR_AND_HAVE:
				case QuestMarkType.CLEAR_NPC_DAILY:
				case QuestMarkType.CLEAR:
					_img.SetIconUV( 9, -0.07f); // ?
					break;
				case QuestMarkType.TALK_HAVE:
				case QuestMarkType.TALK_CLEAR:
				case QuestMarkType.CLEAR_REMAINTALK:
					_img.SetIconUV( 14, -0.08f);
					break;
				default:
					_img.SetIconUV( _data.iImgIdx);
					break;
			}
		}
		else
		{
			_img.SetIconUV( _data.iImgIdx);
		}
	}

	// Set
	public UIZoneImg SetNpc( Vector3 pos, string strName, Vector2 mapMin, Vector2 mapMax, Tbl_ZoneMap_Record.NpcData _data)
	{
		pos.z = npcLayout;

		UIZoneImg temp = CreateZoneImg( npcImgPath, pos, m_ObjectParent.transform);

		UpdateQuestMark( temp, _data);

		temp.SetMapName( strName, mapMin, mapMax, _data.uiPos);

		return temp;
	}

	public UIZoneImg SetPotal( Vector3 pos, string strName, Vector2 mapMin, Vector2 mapMax, Tbl_ZoneMap_Record.PotalData _data)
	{
		pos.z = potalLayout;
		UIZoneImg temp = CreateZoneImg( potalImgPath, pos, m_NormalParent.transform);
		if( null == temp)
			return null;

		temp.SetMapName( strName, mapMin, mapMax, _data.uiPos);
		return temp;
	}

	public UIZoneImg SetWayPoint( Vector3 pos, string strName, Vector2 mapMin, Vector2 mapMax, Tbl_ZoneMap_Record.WayPointData _data)
	{
		pos.z = potalLayout;

		UIZoneImg temp = CreateZoneImg( waypointImgPath, pos, m_NormalParent.transform);
		if( null == temp)
			return null;

		temp.SetMapName( strName, mapMin, mapMax, _data.uiPos);

		return temp;
	}

	public void SetLocal( Vector3 pos, string strName, Vector2 mapMin, Vector2 mapMax, Tbl_ZoneMap_Record.WayPointData _data)
	{
		pos.z = potalLayout;

		UIZoneImg temp = CreateZoneImg( LocalImgPath, pos, m_LocalParent.transform);
		if( null == temp)
			return;

		temp.SetMapName( strName, mapMin, mapMax, _data.uiPos);
	}


	public void SetPartyPlayer( string strName, int iIndex, Vector3 pos, Vector2 mapMin, Vector2 mapMax)
	{
		pos.z = partyUserLayout;

		if( partyImgs.Length <= iIndex)
		{
			AsUtil.ShutDown( "UIZoneMap::SetPartyPlayer()[ partyImgs.Length <= iIndex ] index : " + iIndex);
			return;
		}

		if( false == partyImgs[iIndex].gameObject.active)
		{
			partyImgs[iIndex].gameObject.SetActiveRecursively( true);
		}

		partyImgs[iIndex].transform.localPosition = pos;
		partyImgs[iIndex].SetPartyMapName( strName, mapMin, mapMax, new Vector2( 0f, 1.5f));
	}

	public void SetPlayerPosition( Vector3 pos)
	{
		if( null == playerImg)
			return;

		if( false == playerImg.gameObject.active)
			return;

		pos.z = playerLayout;
		playerImg.transform.localPosition = pos;
	}

	public void SetShowPlayerImg( bool isShow)
	{
		playerImg.gameObject.active = isShow;
	}

	// Create
	private void CrateMapImg( string strPath)
	{
		Vector3 pos = Vector3.zero;
		pos.z = mapLayout;

		GameObject goRes = ResourceLoad.LoadGameObject( strPath);
		if( null == goRes)
		{
			AsUtil.ShutDown( "UIZoneMap::CrateMapImg()[null == gameobject] path : " + strPath);
			return;
		}

		GameObject goInstance = GameObject.Instantiate( goRes) as GameObject;
		goInstance.transform.parent = transform;
		goInstance.transform.localPosition = Vector3.zero;
		goInstance.transform.localRotation = Quaternion.identity;
		goInstance.transform.localScale = Vector3.one;

		m_mapImg = goInstance.GetComponent<SimpleSprite>();
		if( null == m_mapImg)
		{
			AsUtil.ShutDown( "UIZoneMap::CrateBackImg()[ no have SimpleSpritet] path : " + strPath);
			return;
		}

		m_mapImg.transform.localPosition = pos;
	}

	private UIZoneImg CreateZoneImg( string strPath, Vector3 pos, Transform _trn)
	{
		GameObject goRes = ResourceLoad.LoadGameObject( strPath);
		if( null == goRes)
		{
			AsUtil.ShutDown( "UIZoneMap::CreateZoneImg()[null == gameobject] path : " + strPath);
			return null;
		}

		GameObject goInstance = GameObject.Instantiate( goRes) as GameObject;
		goInstance.transform.parent = _trn;
		goInstance.transform.localPosition = Vector3.zero;
		goInstance.transform.localRotation = Quaternion.identity;
		goInstance.transform.localScale = Vector3.one;

		UIZoneImg tempScript = goInstance.GetComponent< UIZoneImg >();
		if( null == tempScript)
		{
			AsUtil.ShutDown( "UIZoneMap::CreateZoneImg()[no find UIZoneImg] path : " + strPath);
			GameObject.DestroyObject( goInstance);
			return null;
		}

		goInstance.transform.localPosition = pos;
		return tempScript;
	}


	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}
