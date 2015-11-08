using UnityEngine;
using System.Collections;
using System.Xml;

public class MapData : AsTableRecord
{
	public enum eAutoUse {NONE, Enable} //$yde
	
	private int m_iID = 0;
	private eMAP_TYPE m_eMapType;
	private int m_iName = 0;
	private int m_iNameTitle = 0;
	private int m_iPatchGroup = 0;
	private string m_strFilePath = null;
	private string m_strBGSoundPath= null;
	private int m_iMapNameStrIdx;
	private float m_fMapNameShowTime = 0.0f;
	private string loadingImage = null;
	private Color rimLightColor = Color.gray;
	private int m_MapLevel = 0;
	private int m_iDiffculty = 1;
	private int m_AdvicePlayer_Min = 0;
	private int m_AdvicePlayer_Max = 0;
	private string m_strInsName = null;

	private Vector2 minSize = Vector2.zero;
	private Vector2 maxSize = Vector2.zero;
	
	private eAutoUse m_AutoUse = eAutoUse.NONE; //$yde

	public int getMapLevel
	{
		get	{ return m_MapLevel; }
	}
	
	public int getDiffculty
	{
		get	{ return m_iDiffculty; }
	}
	public int getAdvicePlayer_Min
	{
		get	{ return m_AdvicePlayer_Min; }
	}
	public int getAdvicePlayer_Max
	{
		get	{ return m_AdvicePlayer_Max; }
	}

	public Vector2 getMinSize
	{
		get	{ return minSize; }
	}

	public Vector2 getMaxSize
	{
		get	{ return maxSize; }
	}

	public string LoadingImage
	{
		get	{ return loadingImage; }
	}

	public Color RimLightColor
	{
		get	{ return rimLightColor; }
	}

	public eMAP_TYPE getMapType
	{
		get	{ return m_eMapType; }
	}
	
	public eAutoUse AutoUse{get{return m_AutoUse;}} //$yde

	public string InsName
	{
		get{return m_strInsName;}
	}

	public MapData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;

			SetValue( ref m_iID, node, "ID" );
			SetValue<eMAP_TYPE>( ref m_eMapType, node, "MapType");
			SetValue( ref m_MapLevel, node, "MapLevel" );
			SetValue( ref m_iName, node, "MapName" );
			SetValue( ref m_iPatchGroup, node, "PatchGroup" );
			SetValue( ref m_strFilePath, node, "FilePath" );
			SetValue( ref m_strBGSoundPath, node, "BGM" );
			SetValue( ref m_iMapNameStrIdx, node, "MapName" );
			SetValue( ref m_fMapNameShowTime, node, "WorldNameShowTime" );
			SetValue( ref loadingImage, node, "LoadingImage");
			
			SetValue( ref m_iNameTitle, node, "MapTitle");
			SetValue( ref m_iDiffculty, node, "Difficulty");
			SetValue( ref m_AdvicePlayer_Min, node, "AdvicePlayer_Min");
			SetValue( ref m_AdvicePlayer_Max, node, "AdvicePlayer_Max");

			XmlNode rimLightColorNode = node.SelectSingleNode( "RimLightColor");
			rimLightColor.r = float.Parse( rimLightColorNode.Attributes[ "r"].Value);
			rimLightColor.g = float.Parse( rimLightColorNode.Attributes[ "g"].Value);
			rimLightColor.b = float.Parse( rimLightColorNode.Attributes[ "b"].Value);
			rimLightColor.a = float.Parse( rimLightColorNode.Attributes[ "a"].Value);


			SetValue( ref minSize.x, node, "MinX" );
			SetValue( ref minSize.y, node, "MinZ" );
			SetValue( ref maxSize.x, node, "MaxX" );
			SetValue( ref maxSize.y, node, "MaxZ" );
			
			int auto = 0;
			SetValue( ref auto, node, "AutoUse" ); if( auto < 0 || auto > 1 ) Debug.LogError("MapData::constructor: [AutoUse] range is wrong : " + auto);
			m_AutoUse = (eAutoUse)auto;

			SetValue( ref m_strInsName, node, "InsName" );
		}
		catch( System.Exception e)
		{
			Debug.LogError( e.ToString());
		}
	}

	protected bool CheckString( string str)
	{
		if( 0 == str.CompareTo( "-1"))
			return false;

		return true;
	}

	protected string ReadString( XmlElement _element, string strfind)
	{
		string strTemp = _element[strfind].InnerText;
		if( false == CheckString( strTemp))
			return null;

		return strTemp;
	}

	public string GetPath()
	{
		return m_strFilePath;
	}

	public string GetName()
	{
		if( null == AsTableManager.Instance )
			return string.Empty;

		return AsTableManager.Instance.GetTbl_String( m_iName );
	}
	
	public string GetNameTitle()
	{
		if( null == AsTableManager.Instance )
			return string.Empty;

		return AsTableManager.Instance.GetTbl_String( m_iNameTitle );
	}

	public int GetID()
	{
		return m_iID;
	}

	public int GetNameStrIdx()
	{
		return m_iMapNameStrIdx;
	}

	public float GetMapNameShowTime()
	{
		return m_fMapNameShowTime;
	}

	public string GetBGSoungPath()
	{
		return m_strBGSoundPath;
	}

	public ePatchGroup GetPatchGroup()
	{
		ePatchGroup PatchGroup = ePatchGroup.ePatchGroup_Invalid;

		if( (int)ePatchGroup.ePatchGroup_Invalid < m_iPatchGroup && (int)ePatchGroup.ePatchGroup_Max > m_iPatchGroup)
		{
			PatchGroup = (ePatchGroup)m_iPatchGroup;
		}

		return PatchGroup;
	}
}
