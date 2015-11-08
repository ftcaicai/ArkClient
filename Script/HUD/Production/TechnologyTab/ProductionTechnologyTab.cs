using UnityEngine;
using System.Collections;

public class ProductionTechnologyTab : MonoBehaviour 
{		
	public GameObject goResTechnologyItemDef;
	public GameObject goResTechnologyItemCash;
	public GameObject goResTechnologyItemOpen;
	public UIScrollList techList;
	
	
	public void Open( int iCount, sPRODUCT_INFO[] _dataList)
	{
		if( null == _dataList)
		{
			Debug.LogError( "ProductionTechnologyTab::Open() [ null == body1_SC_ITEM_PRODUCT_INFO ]");
			return;
		}
		
		techList.ClearList( true);
		
		for( int i = 0; i < (int)eITEM_PRODUCT_TECHNIQUE_TYPE.eITEM_PRODUCT_TECHNIQUE_MAX; ++i)
		{
			if( 0 == iCount)
			{
				InsertItemDef( (eITEM_PRODUCT_TECHNIQUE_TYPE)i, null);				
			}
			else
			{
				if( _dataList.Length <= i)
				{
					Debug.LogError( "ProductionTechnologyTab::Open()[ sProductInfo.Length <= i ] sPRODUCT_INFO Count : " 
						+ iCount + " sProductInfo.Length : " + _dataList.Length);
					continue;
				}
				
				if( 1 <= _dataList[i].nLevel)
					InsertItemOpen( (eITEM_PRODUCT_TECHNIQUE_TYPE)i, _dataList[i]);		
				else
					InsertItemCash( (eITEM_PRODUCT_TECHNIQUE_TYPE)i, null);
			}
		}
		
		techList.ScrollToItem( 0, 0.0f);
	}
	
	public void Close()
	{
		techList.ClearList( true);
	}
	
	public void InsertItemDef( eITEM_PRODUCT_TECHNIQUE_TYPE eType, sPRODUCT_INFO _info)
	{		
		UIListItem item = techList.CreateItem( goResTechnologyItemDef) as UIListItem;		
		ProductionTechItemDef _techItem = item.gameObject.GetComponent<ProductionTechItemDef>();	
		_techItem.Open( eType, _info);
	}
	
	public void InsertItemOpen( eITEM_PRODUCT_TECHNIQUE_TYPE eType, sPRODUCT_INFO _info)
	{		
		UIListItem item = techList.CreateItem( goResTechnologyItemOpen) as UIListItem;		
		ProductionTechItemOpen _techItem = item.gameObject.GetComponent<ProductionTechItemOpen>();	
		_techItem.Open( eType, _info);
	}
	
	public void InsertItemCash( eITEM_PRODUCT_TECHNIQUE_TYPE eType, sPRODUCT_INFO _info)
	{		
		UIListItem item = techList.CreateItem( goResTechnologyItemCash) as UIListItem;		
		ProductionTechItemCash _techItem = item.gameObject.GetComponent<ProductionTechItemCash>();	
		_techItem.Open( eType, _info);
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
