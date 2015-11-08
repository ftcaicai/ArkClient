using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;




public class ItemManagement
{
    //---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------	

    private Dictionary<int, Item> m_ItemList = new Dictionary<int, Item>(); // ID, Item
	
	//$yde
	MultiDictionary<string, Item> m_mdicItemByName = new MultiDictionary<string, Item>(); public MultiDictionary<string, Item> mdicItemByName{get{return m_mdicItemByName;}}


    //---------------------------------------------------------------------
    // function
    //---------------------------------------------------------------------

    // Get
    public Item GetItem(int itemID)
    {
        if (false == IsHaveItem(itemID))
            return null;

        return m_ItemList[itemID];
    }

    public bool IsHaveItem(int itemID)
    {
        return m_ItemList.ContainsKey(itemID);
    }

    /// dopamin begin
    public Dictionary<int, Item> GetList()
    {
        return m_ItemList;
    }
    /// dopamin end 
    // Load
    public bool LoadTable(string strPath)
    {
		bool isUseReadBinaryItemTable = false;
		GameObject toolMain = GameObject.Find( "ToolMain");
		if( toolMain != null )
		{
			AsToolMain	asToolMain = toolMain.GetComponent<AsToolMain>();
			isUseReadBinaryItemTable = asToolMain.UseReadBinaryItemTable;
		}


		if((null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || true == AsTableManager.Instance.useReadBinary || true == isUseReadBinaryItemTable )
		{
			// Ready Binary
			TextAsset textAsset = ResourceLoad.LoadTextAsset( strPath);
			MemoryStream stream = new MemoryStream( textAsset.bytes);
			BinaryReader br = new BinaryReader( stream);
			
			int nCount = br.ReadInt32();
			
			for( int i = 0; i < nCount; i++)
			{
				Item item = new Item( br);
				InsertItem( item);
			}
			
			br.Close();
			stream.Close();
		}
		else
		{
	        try
	        {
	            XmlElement root = AsTableBase.GetXmlRootElement(strPath);
	            XmlNodeList nodes = root.ChildNodes;
	
	            foreach (XmlNode node in nodes)
	            {
	                Item item = new Item(node as XmlElement);
	                InsertItem(item);                
	            }
	
	        }
	        catch (System.Exception e)
	        {
	            Debug.LogError(e.ToString());
	        }
		}

        return true;
    }

#if USE_ITEM_RES
    public void AllLoadItemRes()
    {
        foreach (var item in m_ItemList)
        {
            item.Value.LoadGoRes(ItemRes.eRES_GO.ICON);
            item.Value.LoadGoRes(ItemRes.eRES_GO.PARTS_W);
			item.Value.LoadGoRes(ItemRes.eRES_GO.PARTS_M);
            item.Value.LoadGoRes(ItemRes.eRES_GO.DROP);
            item.Value.LoadTexRes(ItemRes.eRES_TEX.DIFF_W);
			item.Value.LoadTexRes(ItemRes.eRES_TEX.DIFF_M);
            //item.Value.LoadTexRes(ItemRes.eRES_TEX.SPEC);
        }
    }

    public void AllLoadItemTexRes(ItemRes.eRES_TEX eImgType)
    {
        foreach (var item in m_ItemList)
        {
            item.Value.LoadTexRes(eImgType);
        }
    }

    public void AllLoadItemGoRes(ItemRes.eRES_GO eImgType)
    {
        foreach (var item in m_ItemList)
        {
            item.Value.LoadGoRes(eImgType);
        }
    }

#endif

    // Insert item ( key : item id, value : item )
    protected void InsertItem(Item item)
    {
        if (false == CheckInsertItem(item.ItemID))
            return;

        m_ItemList.Add(item.ItemID, item);
		
		//$yde
		if( item.ItemData.isTradeLimit == false)
		{
			string name = AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
			m_mdicItemByName.Add( name, item);
		}
    }

    // Check
    protected bool CheckInsertItem(int iID)
    {
        if (true == m_ItemList.ContainsKey(iID))
        {
            Debug.LogError(" m_ItemList exist to same ID : " + iID);
            return false;
        }
        return true;
    }   
	
	public List<Item> GetItemListByName( string _name)
	{
		List<Item> listItem = new List<Item>();
		
		foreach( string key in m_mdicItemByName.Keys)
		{
			if( key.Contains( _name) == true)
			{
				Debug.Log(key);
				listItem.AddRange( m_mdicItemByName[key]);
				
//				foreach( Item node in  m_mdicItemByName[key])
//				{
//					listItem.Add( node);
//				}
			}
		}
		
		return listItem;
	}
}
