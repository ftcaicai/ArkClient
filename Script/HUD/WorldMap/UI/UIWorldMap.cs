using UnityEngine;
using System.Collections;

public class UIWorldMap : MonoBehaviour 
{
	
	public UIWorldBtn[] worldBtnList;
	public SpriteText textTitle;
	
	protected string mePath = "UI/AsGUI/Worldmap/Object_me";
	protected string party1Path = "UI/AsGUI/Worldmap/Object_party1";
	protected string party2Path = "UI/AsGUI/Worldmap/Object_party2";
	protected string party1WithMePath = "UI/AsGUI/Worldmap/Object_party2withme";
	protected string party3Path = "UI/AsGUI/Worldmap/Object_party3";
	protected string party2WithMePath = "UI/AsGUI/Worldmap/Object_party3withme";
	protected string party3WithMePath = "UI/AsGUI/Worldmap/Object_party4";
	
	
	public void Open(/* int iCurAreaMap */)
	{
		
	}
	
	public void Close()
	{
		
	}
	
	public int GetColliderIndex( Ray inputRay )
	{
		for( int i=0; i<worldBtnList.Length; ++i )
		{
			if( worldBtnList[i].IsRect( inputRay ) )
			{
				return worldBtnList[i].areaIndex;
			}
		}
		
		return 0;
	}
	
	public void ClearPartyImg()
	{
		foreach( UIWorldBtn btn in worldBtnList )
		{
			btn.ClearPartyImg();
		}		
	}
	
	public void SetNoPartyImg( int iPlayerAreaIdx )
	{
		foreach( UIWorldBtn btn in worldBtnList )
		{
			if( btn.areaIndex == iPlayerAreaIdx )
			{	
				btn.SetPartyImg( mePath );
			}
		}
	}
	
	public void SetPartyImg( int iAreaIdx, int iPartyUserCount, int iPlayerAreaIdx )
	{
		foreach( UIWorldBtn btn in worldBtnList )
		{			
			if( btn.areaIndex != iAreaIdx )
				continue;
			
			
			string strPath = null;				
			
			if( iPlayerAreaIdx == iAreaIdx )
			{					
				switch( iPartyUserCount )
				{
				case 1:
					strPath = mePath;
					break;						
				case 2:
					strPath = party1WithMePath;
					break;						
				case 3:
					strPath = party2WithMePath;
					break;						
				case 4:
					strPath = party3WithMePath;
					break;
				}				
			}
			else
			{
				switch( iPartyUserCount )
				{						
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
			
			if( null != strPath )				
				btn.SetPartyImg( strPath );
			break;			
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		textTitle.Text = AsTableManager.Instance.GetTbl_String(65024);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
