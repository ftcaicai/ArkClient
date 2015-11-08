using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TooltipObject : MonoBehaviour 
{	
	public enum ePOSITION
	{
		LEFT,
		RIGHT,
		CENTER,
	}
	private List<TooltipDlg> m_DlgList = new List<TooltipDlg>();
	public ePOSITION dlgPosState = ePOSITION.CENTER;
	
    private float fSavedZ = 0.0f;
	
	private float m_fTime = 0f;
	private bool m_isNeedClear = true;
	
	public bool isOpen
	{
		get
		{
			return 0 < m_DlgList.Count;
		}
	}
	
	public void Clear()
	{
		if( false == m_isNeedClear )
			return;
		
		foreach( TooltipDlg tooltip in m_DlgList )
		{
			Destroy( tooltip.gameObject );
		}
		
		m_DlgList.Clear();
		
		Vector3 tempPos = transform.position;
		tempPos.y = 0.0f;
		transform.position = tempPos;
	}
	
	public void AddTooltipDlg( TooltipDlg _dlg )
	{
		if( null == _dlg )
		{
			Debug.LogError("TooltipObject::AddTooltipDlg()[ null == dlg ]");
			return;
		}
		
//		if( null == _dlg.simpleSprite )
//		{
//			Debug.LogError("TooltipObject::AddTooltipDlg()[ null == dlg.simpleSprite ]");
//			return;
//		}
		
		/*if( null == _dlg.DlgBase)
		{
			Debug.LogError( "TooltipObject::AddTooltipDlg()[ null == _dlg.DlgBase]");
			return;
		}*/
		
		_dlg.transform.parent = transform;
		_dlg.transform.localPosition = Vector3.zero;
		_dlg.transform.localRotation = Quaternion.identity;
		_dlg.transform.localScale = Vector3.one;
		
		if( 0 != m_DlgList.Count )		
		{
			TooltipDlg preDlg =  m_DlgList[ m_DlgList.Count-1 ];
			if( null == preDlg )
			{
				Debug.LogError("TooltipObject::AddTooltipDlg()[ null == preDlg ]");
				return;
			}

//			if( null == preDlg.simpleSprite )
//			{
//				Debug.LogError("TooltipObject::AddTooltipDlg()[ null == preDlg.simpleSprite ]");
//				return;
//			}
			
			/*if( null == preDlg.DlgBase)
			{
				Debug.LogError("TooltipObject::AddTooltipDlg()[ null == preDlg.DlgBase]");
				return;
			}*/
			
//			float fHeight = (preDlg.simpleSprite.height/2.0f) + (_dlg.simpleSprite.height/2.0f); 
			//float fHeight = ( preDlg.DlgBase.TotalHeight * 0.5f) + ( _dlg.DlgBase.TotalHeight * 0.5f);
			float fHeight = ( preDlg.backImgHeight * 0.5f) + ( _dlg.backImgHeight * 0.5f);
			
			
			Vector3 vec3Temp = preDlg.transform.localPosition;
			vec3Temp.y -= fHeight;
			_dlg.transform.localPosition = vec3Temp;
		}
		
		m_DlgList.Add( _dlg );
	}


    public void ResetPosition( TooltipMgr.eOPEN_DLG _openDlgState, float fOffsetZ = 0.0f)
	{
		float fTotalHeight = 0;
		foreach( TooltipDlg _dlg in m_DlgList )
		{
			if( null == _dlg )
			{
				Debug.LogError("TooltipObject::ResetPosition[ null == _dlg ]");
				continue;
			}
//			fTotalHeight += _dlg.simpleSprite.height;
			//fTotalHeight += _dlg.DlgBase.TotalHeight;
			fTotalHeight += _dlg.backImgHeight;
			
		}
		
		Vector3 tempPos = transform.position;
		tempPos.y += (fTotalHeight * 0.3f);
		tempPos.z = fSavedZ + fOffsetZ;
		
		float screenWidth = UIManager.instance.rayCamera.orthographicSize * UIManager.instance.rayCamera.aspect * 2.0f;
		float fdlgWidth = 19.5f;

        switch (_openDlgState)
		{
		case TooltipMgr.eOPEN_DLG.normal:        	
			transform.position = tempPos;
			break;
			
		case TooltipMgr.eOPEN_DLG.right:
			
			if( dlgPosState == ePOSITION.CENTER )
			{				
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth + 16.5f) * 0.5f), tempPos.y, tempPos.z);
			}
			else if( dlgPosState == ePOSITION.LEFT )
			{				
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( screenWidth -(screenWidth * 0.05f)) + ( fdlgWidth * 0.5f), tempPos.y, tempPos.z);
			}
			else if( dlgPosState == ePOSITION.RIGHT )
			{
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( screenWidth-(screenWidth * 0.05f)) + ( fdlgWidth * 1.5f), tempPos.y, tempPos.z);
			}
			break;
			
		case TooltipMgr.eOPEN_DLG.left:	
			if( dlgPosState == ePOSITION.CENTER )
			{				
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.5f), tempPos.y, tempPos.z);
			}
			else if( dlgPosState == ePOSITION.LEFT )
			{				
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.05f) - ( fdlgWidth * 1.5f), tempPos.y, tempPos.z);
			}
			else if( dlgPosState == ePOSITION.RIGHT )
			{
				transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.05f) - ( fdlgWidth * 0.5f), tempPos.y, tempPos.z);
			}
			break;
		}
		
		
		
		m_isNeedClear = false;
		m_fTime = 0f;
	}
	
	// Use this for initialization
	void Start () 
	{
        fSavedZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_isNeedClear )
		{
			m_fTime += Time.deltaTime;
			if( m_fTime > 0.1f )
			{
				m_isNeedClear = true;
				m_fTime = 0f;
			}
		}
	}
}
