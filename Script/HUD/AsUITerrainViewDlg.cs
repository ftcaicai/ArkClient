using UnityEngine;
using System.Collections;

public class AsUITerrainViewDlg : MonoBehaviour 
{
	public SpriteText txtAreaTitle;
	public SpriteText txtAreaText;
	
	public SpriteText txtDiffTitle;
	public SpriteText txtDiffText;
	
	public SpriteText txtUserNumTitle;
	public SpriteText txtUserNumText;
	
	public SimpleSprite[] imgDiffCount;
	
	
	public void Open()
	{
		Map _map = TerrainMgr.Instance.GetCurrentMap();
		if( null == _map )
		{
			return;
		}
		
		SetDiff( _map.MapData.getDiffculty );
		txtAreaText.Text = _map.MapData.GetNameTitle();
		
		System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();
		
		if( int.MaxValue != _map.MapData.getAdvicePlayer_Min && int.MaxValue != _map.MapData.getAdvicePlayer_Max )
		{
			sbTemp.Append( _map.MapData.getAdvicePlayer_Min );
			sbTemp.Append( "~" );
			sbTemp.Append( _map.MapData.getAdvicePlayer_Max );
			sbTemp.Append( AsTableManager.Instance.GetTbl_String(337) );
		}
		else if( int.MaxValue != _map.MapData.getAdvicePlayer_Min )
		{
			sbTemp.Append( _map.MapData.getAdvicePlayer_Min );
			sbTemp.Append( AsTableManager.Instance.GetTbl_String(337) );
		}
		else if( int.MaxValue != _map.MapData.getAdvicePlayer_Max )
		{
			sbTemp.Append( _map.MapData.getAdvicePlayer_Max );
			sbTemp.Append( AsTableManager.Instance.GetTbl_String(337) );
		}
		
		txtUserNumText.Text = sbTemp.ToString();
	}
	
	
	
	public void SetDiff( int iDiff )
	{
		switch ( iDiff )
		{
		case 1:
			txtDiffText.Text = AsTableManager.Instance.GetTbl_String(2087);
			break;
		case 2:
			txtDiffText.Text = AsTableManager.Instance.GetTbl_String(2088);
			break;
		case 3:
			txtDiffText.Text = AsTableManager.Instance.GetTbl_String(2089);
			break;
		case 4:
			txtDiffText.Text = AsTableManager.Instance.GetTbl_String(2090);
			break;
		case 5:
			txtDiffText.Text = AsTableManager.Instance.GetTbl_String(2091);
			break;
		}
		
		for( int i=0; i<imgDiffCount.Length; ++i )
		{
			if( iDiff > i )
			{
				imgDiffCount[i].gameObject.active = true;
			}
			else
			{
				imgDiffCount[i].gameObject.active = false;
			}
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		txtAreaTitle.Text = AsTableManager.Instance.GetTbl_String(2085);
		txtDiffTitle.Text = AsTableManager.Instance.GetTbl_String(2086);
        txtUserNumTitle.Text = AsTableManager.Instance.GetTbl_String(2099);
		
	
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.BringIn();

		Invoke( "Dismiss", 6.0f);	
	}
	
	private void Dismiss()
	{
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.Dismiss();
		
		Invoke( "Destroy", 1.0f);
	}
	
	private void Destroy()
	{
		GameObject.Destroy( gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
