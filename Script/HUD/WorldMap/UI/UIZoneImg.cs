using UnityEngine;
using System.Collections;

public class UIZoneImg : MonoBehaviour
{
	public SpriteText mapName;
	public SimpleSprite backImg;
	public AsNameBackImg textBackImg;

	private string m_strMapName = string.Empty;
	private Color m_TempColor = Color.white;
	private Vector2 m_vOrgBackImg = Vector2.one;
	private float m_fOrgZ = 0f;
	
	private void Awake()
	{
		if ( backImg != null)
			m_vOrgBackImg = backImg.lowerLeftPixel;
		
		m_fOrgZ = transform.localPosition.z;
	}

	public void SetIconUV( int nIndex, float fZ = 0f)
	{
		if( null == backImg)
		{
			Debug.LogError( "UIZoneImg::SetIconUV()[ null == backIMg ]");
			return;
		}

		int nLow = nIndex / 5;
		int nCol = nIndex % 5;

		int nX = (int)( m_vOrgBackImg.x) + 50 * nCol;
		int nY = (int)( m_vOrgBackImg.y) + 50 * nLow;

		backImg.SetLowerLeftPixel( nX, nY);

		Vector3 temp = transform.localPosition;
		temp.z = m_fOrgZ + fZ ;
		transform.localPosition = temp;
	}

	public void SetAlpahMap( float _fAlpha)
	{
		if( null == mapName)
			return;

		m_TempColor.a = _fAlpha;
		mapName.Text = m_TempColor.ToString() + m_strMapName;
	}

	public void HideMapName()
	{
		mapName.gameObject.SetActiveRecursively( false);
		textBackImg.gameObject.SetActiveRecursively( false);
	}

	public void SetMapNameSimple( string _strName, Color _color)
	{
		m_strMapName = _strName;
		m_TempColor = _color;

		mapName.Text = m_TempColor.ToString() + _strName;
		RsetSize();
	}

	public void SetMapName( string strName, Vector2 mapMin, Vector2 mapMax, Vector2 uiPos)
	{
		if( null == mapName)
			return;

		mapName.Text = m_strMapName = strName;

		Vector3 mapNamePosition = mapName.transform.localPosition;
		Vector2 backImgPos = Vector2.zero;
		float backImgHeight = 0f;
		if( null != backImg)
		{
			backImgPos.x = backImg.gameObject.transform.localPosition.x;
			backImgPos.y = backImg.gameObject.transform.localPosition.y;
			backImgHeight = backImg.height;
		}

		// x axis
		float fHelfWidth = mapName.TotalWidth * 0.5f;
		float mapNameMinPosX = backImgPos.x + mapNamePosition.x - fHelfWidth;
		float mapNameMaxPosX = backImgPos.x + mapNamePosition.x + fHelfWidth;
		if( mapNameMinPosX < mapMin.x)
		{
			mapNamePosition.x += ( mapMin.x - mapNameMinPosX) + uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}
		else if( mapNameMaxPosX > mapMax.x)
		{
			mapNamePosition.x -= ( mapNameMaxPosX - mapMax.x)  + uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}
		else
		{
			mapNamePosition.x += uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}

		// y axis
		float fHelfHeight = mapName.BaseHeight * 0.5f;
		float mapNameMaxPosY = backImgPos.y + mapNamePosition.y + fHelfHeight;
		if( mapNameMaxPosY < mapMax.y)
		{
			mapNamePosition.y = backImgHeight * 0.5f  + uiPos.y;
			mapName.transform.localPosition = mapNamePosition;
		}
		else
		{
			mapNamePosition.y += uiPos.y;
			mapName.transform.localPosition = mapNamePosition;
		}

		RsetSize();
	}

	public void SetPartyMapName( string strName, Vector2 mapMin, Vector2 mapMax, Vector2 uiPos)
	{
		if( null == mapName)
			return;

		mapName.Text = m_strMapName = strName;

		Vector3 mapNamePosition = Vector3.zero;
		Vector2 backImgPos = Vector2.zero;
		float backImgHeight = 0f;
		if( null != backImg)
		{
			backImgPos.x = backImg.gameObject.transform.localPosition.x;
			backImgPos.y = backImg.gameObject.transform.localPosition.y;
			backImgHeight = backImg.height;
		}

		// x axis
		float fHelfWidth = ( mapName.TotalWidth * 0.5f)+0.5f;
		float mapNameMinPosX = backImgPos.x + mapNamePosition.x - fHelfWidth;
		float mapNameMaxPosX = backImgPos.x + mapNamePosition.x + fHelfWidth;
		if( mapNameMinPosX < mapMin.x)
		{
			mapNamePosition.x = ( mapMin.x - mapNameMinPosX) + uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}
		else if( mapNameMaxPosX > mapMax.x)
		{
			mapNamePosition.x = ( mapMax.x - mapNameMaxPosX)  + uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}
		else
		{
			mapNamePosition.x = uiPos.x;
			mapName.transform.localPosition = mapNamePosition;
		}

		// y axis
		float fHelfHeight = mapName.BaseHeight * 0.5f;
		float mapNameMaxPosY = backImgPos.y + mapNamePosition.y + fHelfHeight;
		if( mapNameMaxPosY < mapMax.y)
		{
			mapNamePosition.y = backImgHeight * 0.5f  + uiPos.y;
			mapName.transform.localPosition = mapNamePosition;
		}
		else
		{
			mapNamePosition.y = uiPos.y;
			mapName.transform.localPosition = mapNamePosition;
		}

		RsetSize();
	}

	private void RsetSize()
	{
		if( null == mapName || null == textBackImg)
			return;

		//textBackImg.width = mapName.TotalWidth;
		textBackImg.SetText( mapName);
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
