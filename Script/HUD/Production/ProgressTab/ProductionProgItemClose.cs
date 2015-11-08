using UnityEngine;
using System.Collections;

public class ProductionProgItemClose : ProductionProgressItem 
{	
	public SpriteText textClose;
	
	public void Open( byte _iIndex )
	{
		SetSlot(_iIndex);
	}

	// Use this for initialization
	void Start () 
	{
		textClose.Text = AsTableManager.Instance.GetTbl_String(1294);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
