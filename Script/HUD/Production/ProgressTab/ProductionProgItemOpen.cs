using UnityEngine;
using System.Collections;

public class ProductionProgItemOpen : ProductionProgressItem 
{
	public SpriteText textShow;
		
	public void Open( byte _iIndex )
	{
		SetSlot( _iIndex );
	}
	
	// Use this for initialization
	void Start () 
	{
		textShow.Text = AsTableManager.Instance.GetTbl_String(1293);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
