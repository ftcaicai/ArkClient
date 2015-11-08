using UnityEngine;
using System.Collections;
using System.Xml;

public class Map
{
	private MapData m_MapData = null;


	public Map( XmlElement _element)
	{
		m_MapData = new MapData( _element);
	}

	public MapData MapData
	{
		get	{ return m_MapData; }
	}

	public void PlayMapBegin()
	{
		AsSoundManager.Instance.StopBGM();

		// begin BG sound play
		string strBgPath = MapData.GetBGSoungPath();
		if( null == strBgPath)
			Debug.LogWarning( "back ground sound no have [ Map ID : " + MapData.GetID());
		else
			AsSoundManager.Instance.PlayBGM( strBgPath);
		// end BG sound play
	}
}
