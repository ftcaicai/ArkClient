using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum eGameGuideType : int
{
	Invalid = -1,
	
	QuestAgree,
	QuestComplete,
	Death,
	Title,
	Level,
	Monster,
	Item,
	Map,
	Condition,
	Upgrade,
	Q_SlotOpen,
	
	Max
};

public class GameGuideData : AsTableRecord
{
	public int index = -1;
	public int nameIdx = -1;
	public eGameGuideType eType = eGameGuideType.Invalid;
	public int guideValue = 0;
	public string imagePath;

	public GameGuideData()
	{
	}

	public GameGuideData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref index, node, "Index");
			SetValue( ref nameIdx, node, "Name");
			SetValue( ref eType, node, "GuideType");
			SetValue( ref guideValue, node, "GuideValue");
			SetValue( ref imagePath, node, "ImagePath");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsGameGuideManager
{
	private Dictionary<eGameGuideType,List<GameGuideData>> dicGuide = new Dictionary<eGameGuideType,List<GameGuideData>>();
	private AsGameGuideDlg curGuideDlg = null;
	private AsGuideListDlg m_GuideListDlg = null;

	static private AsGameGuideManager instance = null;
	static public AsGameGuideManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsGameGuideManager();
			
			return instance;
		}
	}

	public Dictionary<eGameGuideType,List<GameGuideData>> DataContainer
	{
		get { return dicGuide; }
	}
	
	private AsGameGuideManager()
	{
	}

	public bool CheckUp( eGameGuideType type, int guideValue)
	{
		if( GAME_STATE.STATE_INGAME != AsGameMain.s_gameState)
			return false;
		
		if( false == dicGuide.ContainsKey( type))
			return false;
		
		List<GameGuideData> lstGuide = dicGuide[ type];
		if( 0 == lstGuide.Count)
			return false;
		
		foreach( GameGuideData guideData in lstGuide)
		{
			if( ( guideValue == guideData.guideValue) || ( -1 == guideValue))
			{
				string szKey = string.Format( "GameGuideKey_{0}_{1}", AsUserInfo.Instance.SavedCharStat.charName_, guideData.index);
				if( true == PlayerPrefs.HasKey( szKey))
					return false;
				
				PlayerPrefs.SetInt( szKey, 1);
				PlayerPrefs.Save();
				DisplayGuide( guideData);
				
				return true;
			}
		}
		
		return false;
	}
	
	public void DisplayGuide( GameGuideData guideData)
	{
		if( true == AsUserInfo.Instance.IsDied())
			return;

		if( true == TargetDecider.CheckCurrentMapIsIndun() && eGameGuideType.Death == guideData.eType)
			return;
		
		if( null != curGuideDlg)
			curGuideDlg.Close();

		//$yde
		if(guideData.nameIdx == 0)
		{
			_FingerProc(guideData);
			return;
		}
		
		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_GameGuide")) as GameObject;
		Debug.Assert( null != go);
		AsGameGuideDlg guideDlg = go.GetComponentInChildren<AsGameGuideDlg>();
		Debug.Assert( null != guideDlg);
		guideDlg.Init( guideData);
		curGuideDlg = guideDlg;
	}
	
	public void LoadTable()
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( "Table/GameGuide");
			XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
			{
				GameGuideData guideData = new GameGuideData( (XmlElement)node);
				
				if( true == dicGuide.ContainsKey( guideData.eType))
				{
					List<GameGuideData> lstGuide = dicGuide[ guideData.eType];
					lstGuide.Add( guideData);
				}
				else
				{
					List<GameGuideData> lstGuide = new List<GameGuideData>();
					lstGuide.Add( guideData);
					dicGuide.Add( guideData.eType, lstGuide);
				}
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsLoadingTipManager:LoadTable");
		}
	}
	
	public bool IsOpenGameGuide()
	{
		if( null != curGuideDlg)
			return true;
		
		return false;
	}
	
	public void CloseGameGuide()
	{
		if( null != curGuideDlg)
			curGuideDlg.Close();
	}
	
	public void SetGuideList(GameObject go)
	{
		m_GuideListDlg = go.GetComponentInChildren<AsGuideListDlg>();
	}
	
	public bool IsOpenGameGuideListDlg()
	{
		if( null != m_GuideListDlg)
			return true;
		
		return false;
	}
	
	public void CloseGameGuideListDlg()
	{
		if( null != m_GuideListDlg)
			m_GuideListDlg.Close();
	}

	//$yde
	void _FingerProc(GameGuideData _data)
	{
		if(_data == null)
		{
			Debug.LogError("AsGameGuideManager:: _FingerProc: _data is null");
			return;
		}

		string str = "UI/Optimization/FingerMove/Prefabs/";
		str = str + _data.imagePath;
		Object o = ResourceLoad.LoadGameObject(str);
		if(o == null)
		{
			Debug.LogError("AsGameGuideManager:: _FingerProc: prefab is not found. _data.index" + _data.index + ", _data.imagePath = " + _data.imagePath);
			return;
		}

		GameObject go = GameObject.Instantiate( ResourceLoad.LoadGameObject(str)) as GameObject;
		GameObject.Destroy(go, 3f);
	}
}
