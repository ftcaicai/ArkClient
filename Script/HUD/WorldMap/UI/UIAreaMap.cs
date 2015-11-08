using UnityEngine;
using System.Collections;

public class UIAreaMap : MonoBehaviour 
{	
	public UIAreaBtn[] btnMapList;
	public SimpleSprite focusImg;
	public float focusImgLayout = 0.1f;
	
	protected string mePath = "UI/AsGUI/Worldmap/Object_me";
	protected string party1Path = "UI/AsGUI/Worldmap/Object_party1";
	protected string party2Path = "UI/AsGUI/Worldmap/Object_party2";
	protected string party1WithMePath = "UI/AsGUI/Worldmap/Object_party2withme";
	protected string party3Path = "UI/AsGUI/Worldmap/Object_party3";
	protected string party2WithMePath = "UI/AsGUI/Worldmap/Object_party3withme";
	protected string party3WithMePath = "UI/AsGUI/Worldmap/Object_party4";
	
	
	public void Open( int iZoneMapIdx )
	{
		SetFocusZoneMap( iZoneMapIdx );
	}
	
	public void Close()
	{
	}
	
	public void AllCloseBtn()
	{
		foreach( UIAreaBtn _btn in btnMapList )
		{
			_btn.CloseBtn();	
		}
	}
	
	public void SetAreaMapClickDelegate(AreaMapBtnClickDelegate callback)
	{
		foreach( UIAreaBtn _btn in btnMapList )
		{
			_btn.SetAreaMapBtnClickDelegate(callback);
		}
	}
	
	// Set
	public void SetFocusZoneMap( int iMapIdx )
	{
		foreach( UIAreaBtn _btn in btnMapList )
		{
			if( _btn.mapIdx == iMapIdx )
			{
				if( null != focusImg )
				{
					Vector3 tempPos = _btn.transform.position;
					tempPos.z -= focusImgLayout;
					focusImg.gameObject.active = true;
					focusImg.transform.position = tempPos;
				}
				return;
			}		
		}	
		
		focusImg.gameObject.active = false;
	}	
	
	public void ClearPartyImg()
	{
		foreach( UIAreaBtn btn in btnMapList )
		{
			btn.ClearPartyImg();
		}		
	}
	
	public void SetNoPartyImg( int iPlayerMapId )
	{
		foreach( UIAreaBtn btn in btnMapList )
		{			
			if( btn.mapIdx == iPlayerMapId )
			{				
				btn.SetPartyImg( mePath );				
			}
		}
	}
	
	public void SetPartyImg( int iMapId, int iPartyUserCount, int iPlayerMapId )
	{
		foreach( UIAreaBtn btn in btnMapList )
		{
			if( btn.mapIdx == iMapId )
			{
				string strPath = "none";
				
				if( iPlayerMapId == iMapId )
				{					
					switch( iPartyUserCount )
					{
					case 0:
						strPath = mePath;
						break;						
					case 1:
						strPath = party1WithMePath;
						break;						
					case 2:
						strPath = party2WithMePath;
						break;						
					case 3:
						strPath = party3WithMePath;
						break;
					}				
				}
				else
				{
					switch( iPartyUserCount )
					{
					case 0:
						strPath = mePath;
						break;						
					case 1:
						strPath = party1Path;
						break;						
					case 2:
						strPath = party2Path;
						break;						
					case 3:
						strPath = party3Path;
						break;
					}
				}
				btn.SetPartyImg( strPath );
				
			}
			else if( btn.mapIdx == iPlayerMapId )
			{
				btn.SetPartyImg(mePath);			
			}
		}
	}
	
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
