using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public enum Store_Item_Type
{
    NormalItem,
    ChargeItem,
	GachaItem,
}

public class Store_Item_Info_Base
{
	public int Key = -1;
	public int ID = -1;
	public int Count = 0;
	public int ItemBuyMax = 0;
	public int TradeItemID = -1;
	public int TradeCount = 0;
	public int ResetTime = 0;
}


public class Store_Item_Info_Table : Store_Item_Info_Base
{
    public Store_Item_Type			Type			= Store_Item_Type.NormalItem;
    public eSHOPITEMHIGHLIGHT		Highlight		= eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE;
	public eCashStoreMainCategory	MainCategory	= eCashStoreMainCategory.NONE;
	public eCashStoreSubCategory    SubCategory		= eCashStoreSubCategory.NONE;
	public string	ID_String	= "-1";
	public int		Price		= 0;
	public int		SetItemID	= 0;
	public int		DescID		= 0;

    public Store_Item_Info_Table()
    {
        Key			= -1;
        ID			= -1;
        Count		= 0;
		SetItemID	= 0;
		DescID		= 0;
        ID_String	= "-1";
        Highlight	= eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE;
        Price		= 0;
		MainCategory	= eCashStoreMainCategory.NONE;
		SubCategory		= eCashStoreSubCategory.NONE;
    }

	public Store_Item_Info_Table(int key, int id, int itemMaxBuy, int tradeItemID, int tradeCount, int resetTime)
	{
		Type  = Store_Item_Type.NormalItem;
		Key   = key;
		ID    = id;
		Count = 1;
		ItemBuyMax	= itemMaxBuy;
		TradeItemID	= tradeItemID;
		TradeCount  = tradeCount;
		ResetTime	= ResetTime;
	}

	public Store_Item_Info_Table(Store_Item_Type _storeItemType, int _key, int _itemID, int _count, int _itemDescID, int _setItemID, eSHOPITEMHIGHLIGHT _highlight, int _price, eCashStoreMainCategory _mainCategory, eCashStoreSubCategory _subCategory)
	{
        Type			= _storeItemType;
        Key				= _key;
		ID				= _itemID;
		Count			= _count;
		DescID			= _itemDescID;
		SetItemID		= _setItemID;
        ID_String		= _itemID.ToString();
        Highlight		= _highlight;
        Price			= _price;
		MainCategory	= _mainCategory;
		SubCategory		= _subCategory;

	}

    public Store_Item_Info_Table(Store_Item_Type type, int key, string id, int count, int _recommendIdx = 0, eSHOPITEMHIGHLIGHT _highlight = eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE)
    {
        Type      = type;
        Key       = key;
        ID_String = id;
        ID        = -1;
        Count     = count;
        Highlight = _highlight;
    }

    public override string ToString()
    {
        return "Item["+Key+"] = (" +ID+") [" + MainCategory + "]["+ SubCategory + "] price = "+ Price;
    }
}


//item kind
public class Npc_Store_Info_Kind
{
    private Dictionary<eStore_ItemKind, List<Store_Item_Info_Table>> dicData = new Dictionary<eStore_ItemKind, List<Store_Item_Info_Table>>();

    public Dictionary<eStore_ItemKind, List<Store_Item_Info_Table>> Data { get { return dicData; } }

    public Npc_Store_Info_Kind()
    {
        dicData = new Dictionary<eStore_ItemKind, List<Store_Item_Info_Table>>();
    }

    public List<Store_Item_Info_Table> GetStoreItemKind(eStore_ItemKind _kind)
    {
        if (dicData.ContainsKey(_kind))
            return dicData[_kind];
        else
            return null;
    }

    public List<eStore_ItemKind> GetStoreKind()
    {
        List<eStore_ItemKind> listKind = new List<eStore_ItemKind>();

        foreach (eStore_ItemKind kind in dicData.Keys)
            listKind.Add(kind);
        
        return listKind;
    }

    public void AddStoreItemInfoKind(eStore_ItemKind _storeItemKind, Store_Item_Info_Table _storeItemElement)
    {
        if (!dicData.ContainsKey(_storeItemKind))
            dicData.Add(_storeItemKind, new List<Store_Item_Info_Table>());
		
        dicData[_storeItemKind].Add(_storeItemElement);
    }

    public override string ToString()
    {
        string szResult = string.Empty;
        foreach (eStore_ItemKind kind in dicData.Keys)
        {
            szResult += kind.ToString();

            foreach (Store_Item_Info_Table storeItemInfo in dicData[kind])
            {
                szResult += " \n";
                szResult += storeItemInfo.ToString();
            }
        }

        return szResult;
    }
}

public class NPC_Store_Info
{
    public int npcID;
    private Dictionary<int , Store_Item_Info_Table> dicStoreItem;
    private Dictionary<eCLASS, Npc_Store_Info_Kind> dicStoreItemByClass;

    public NPC_Store_Info()
    {
        npcID = -1;
		dicStoreItem = new Dictionary<int, Store_Item_Info_Table>();

        dicStoreItemByClass = new Dictionary<eCLASS, Npc_Store_Info_Kind>();
        dicStoreItemByClass.Add(eCLASS.DIVINEKNIGHT, new Npc_Store_Info_Kind());
        dicStoreItemByClass.Add(eCLASS.MAGICIAN, new Npc_Store_Info_Kind());
        dicStoreItemByClass.Add(eCLASS.CLERIC, new Npc_Store_Info_Kind());
		dicStoreItemByClass.Add(eCLASS.HUNTER, new Npc_Store_Info_Kind());
        dicStoreItemByClass.Add(eCLASS.All, new Npc_Store_Info_Kind());
    }

    public void AddNpcStoreItem(Store_Item_Info_Table _storeItemElement)
    {
        try
        {
			if (!dicStoreItem.ContainsKey(_storeItemElement.ID))
				dicStoreItem.Add(_storeItemElement.ID, _storeItemElement);
			else
			{
				string szMessage = "same id already exist in item table = " + _storeItemElement.ID;
				AsUtil.ShutDown(szMessage);
			}



            // 분류 작업
            Item item = ItemMgr.ItemManagement.GetItem(_storeItemElement.ID);

            if (item == null)
            {
                string szMessage = "item isn't exist in item table = " + _storeItemElement.ID;
                AsUtil.ShutDown(szMessage);
            }

            eStore_ItemKind storeKind = GetItemKind(item.ItemData);
            eCLASS classType = item.ItemData.needClass;

			if (classType == eCLASS.PET)
				classType = eCLASS.All;

            dicStoreItemByClass[classType].AddStoreItemInfoKind(storeKind, _storeItemElement);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public List<Store_Item_Info_Table> GetNpcStoreItems(eCLASS _class, eStore_ItemKind _eKind)
    {
		if (!dicStoreItemByClass.ContainsKey(_class))
			return new List<Store_Item_Info_Table>();

        return dicStoreItemByClass[_class].GetStoreItemKind(_eKind);
    }

	public Store_Item_Info_Table GetNpcStoreItem(int _itemID)
	{
		if (!dicStoreItem.ContainsKey(_itemID))
			return null;
		else
			return dicStoreItem[_itemID];
	}

    public List<eStore_ItemKind> GetHaveKind(eCLASS _class)
    {
        if (dicStoreItemByClass.ContainsKey(_class))
            return dicStoreItemByClass[_class].GetStoreKind();

        return new List<eStore_ItemKind>();

    }

    public eStore_ItemKind GetItemKind(ItemData _itemData)
    {
        eStore_ItemKind storeItemKind = eStore_ItemKind.NONE;

        Item.eITEM_TYPE type = _itemData.GetItemType();

        if (type == Item.eITEM_TYPE.EquipItem)
        {
            Item.eEQUIP equipType = (Item.eEQUIP)_itemData.GetSubType();

            if (equipType == Item.eEQUIP.Weapon)
                storeItemKind = eStore_ItemKind.WEAPON;
            else if (equipType == Item.eEQUIP.Armor || equipType == Item.eEQUIP.Gloves || equipType == Item.eEQUIP.Head || equipType == Item.eEQUIP.Point)
                storeItemKind = eStore_ItemKind.ARMOR;
            else if (equipType == Item.eEQUIP.Ring || equipType == Item.eEQUIP.Necklace || equipType == Item.eEQUIP.Earring)
                storeItemKind = eStore_ItemKind.ACCESSORY;
            else
                storeItemKind = eStore_ItemKind.ETC;
        }
        else if (type == Item.eITEM_TYPE.EtcItem)
        {
            Item.eEtcItem etcType = (Item.eEtcItem)_itemData.GetSubType();

            if (etcType == Item.eEtcItem.Material)
                storeItemKind = eStore_ItemKind.MATERIAL;
            else
                storeItemKind = eStore_ItemKind.ETC;
        }
        else if (type == Item.eITEM_TYPE.ActionItem)
        {
            storeItemKind = eStore_ItemKind.CONSUME;
        }
        else if (type == Item.eITEM_TYPE.UseItem || type == Item.eITEM_TYPE.CosEquipItem)
        {
            storeItemKind = eStore_ItemKind.ETC;
        }

        return storeItemKind;
    }

    public override string ToString()
    {
        string resultString = "NPC ID = " + npcID;

        resultString += "\n list count = " + dicStoreItem.Count;

        return resultString;
    }
}

public class Tbl_Store_Record : AsTableRecord {

    private NPC_Store_Info m_StoreItemInfo = new NPC_Store_Info();
    public  NPC_Store_Info StoreItemInfo { get { return m_StoreItemInfo; } }

	public Tbl_Store_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;

            m_StoreItemInfo = new NPC_Store_Info();

            m_StoreItemInfo.npcID = System.Convert.ToInt32(node.Attributes["NpcTableIdx"].Value);

            XmlNodeList storeItemNodeList = node.SelectNodes("Store_Item_Unit");

            int keyCount = 0;

            foreach (XmlNode storeItemNode in storeItemNodeList)
            {
                int itemID		= System.Convert.ToInt32(storeItemNode.Attributes["ItemTableIdx"].Value);
				int itemBuyMax	= -1;
				int tradeItem	= -1;
				int tradeCount  = 0;
				int resetTime	= -1;

				if (storeItemNode.Attributes["ItemCount"].Value != "NONE")
					itemBuyMax = System.Convert.ToInt32(storeItemNode.Attributes["ItemCount"].Value);

				if (storeItemNode.Attributes["TradeItem"].Value != "NONE")
					tradeItem = System.Convert.ToInt32(storeItemNode.Attributes["TradeItem"].Value);

				if (storeItemNode.Attributes["TradeItemCount"].Value != "NONE")
					tradeCount = System.Convert.ToInt32(storeItemNode.Attributes["TradeItemCount"].Value);

				if (storeItemNode.Attributes["ResetTime"].Value != "NONE")
					resetTime = System.Convert.ToInt32(storeItemNode.Attributes["ResetTime"].Value);

                Store_Item_Info_Table itemElement = new Store_Item_Info_Table(keyCount++, itemID, itemBuyMax, tradeItem, tradeCount, resetTime);

                m_StoreItemInfo.AddNpcStoreItem(itemElement);
            }
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}


public class Tbl_Store_Table : AsTableBase
{
    Dictionary<int, NPC_Store_Info> m_ResourceTable = new Dictionary<int, NPC_Store_Info>();

    public Tbl_Store_Table(string _path)
    {
        m_TableType = eTableType.STORE;

        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root   = GetXmlRootElement(_path);
            XmlNodeList nodes = root.SelectNodes("Store");

            foreach (XmlNode node in nodes)
            {
                Tbl_Store_Record record = new Tbl_Store_Record((XmlElement)node);
                int             npcID   = record.StoreItemInfo.npcID;

                if (!m_ResourceTable.ContainsKey(npcID))
                    m_ResourceTable.Add(npcID, record.StoreItemInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    #region -Get
    public List<Store_Item_Info_Table> GetStoreItemInfo(eCLASS _eClass, eStore_ItemKind _eKind, int _npcID)
    {
        List<Store_Item_Info_Table> returnList = new List<Store_Item_Info_Table>();

        if (!m_ResourceTable.ContainsKey(_npcID))
            return returnList;

        List<Store_Item_Info_Table> listStoreItemInfo    = m_ResourceTable[_npcID].GetNpcStoreItems(_eClass, _eKind);
        List<Store_Item_Info_Table> listStoreItemInfoAll = m_ResourceTable[_npcID].GetNpcStoreItems(eCLASS.All, _eKind);


        if (listStoreItemInfo != null)
            returnList.AddRange(listStoreItemInfo);

        if (listStoreItemInfoAll != null)
            returnList.AddRange(listStoreItemInfoAll);

        return returnList;
    }
	
	public List<eStore_ItemKind> GetHaveItemKind(int _npcID, eCLASS _class)
	{
        List<eStore_ItemKind> listResult = new List<eStore_ItemKind>();
		
		if (m_ResourceTable.ContainsKey(_npcID) == false)
            return listResult;
		else
		{
		    List<eStore_ItemKind> listKindByType = m_ResourceTable[_npcID].GetHaveKind(_class);
            List<eStore_ItemKind> listKindAll    = m_ResourceTable[_npcID].GetHaveKind(eCLASS.All);

            List<eStore_ItemKind> listKeys = new List<eStore_ItemKind>();

            listKeys.AddRange(listKindByType);
            listKeys.AddRange(listKindAll);

            return listKeys;
		}
    }

	public Store_Item_Info_Table GetStoreInfo(int _npcID, int _itemID)
	{
		if (m_ResourceTable.ContainsKey(_npcID) == true)
			return m_ResourceTable[_npcID].GetNpcStoreItem(_itemID);
		else
			return null;
	}
    #endregion
}
