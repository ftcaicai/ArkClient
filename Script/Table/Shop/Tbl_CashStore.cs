using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

//item kind
public class Cash_Store_Item_Categorization
{
	private	Dictionary<eCashStoreMainCategory, Dictionary<eCashStoreSubCategory, List<Store_Item_Info_Table>>> dicData = new Dictionary<eCashStoreMainCategory, Dictionary<eCashStoreSubCategory, List<Store_Item_Info_Table>>>();
	public	Dictionary<eCashStoreMainCategory, Dictionary<eCashStoreSubCategory, List<Store_Item_Info_Table>>> Data { get { return dicData; } }

	public Cash_Store_Item_Categorization()
    {
		dicData = new Dictionary<eCashStoreMainCategory, Dictionary<eCashStoreSubCategory, List<Store_Item_Info_Table>>>();
    }

	public List<Store_Item_Info_Table> GetStoreItem(eCashStoreMainCategory _main, eCashStoreSubCategory _sub)
    {
		if (dicData.ContainsKey(_main))
		{
			if (dicData[_main].ContainsKey(_sub))
				return dicData[_main][_sub];
			else
				return new List<Store_Item_Info_Table>();
		}
		else
			return new List<Store_Item_Info_Table>();
    }

	public List<eCashStoreMainCategory> GetStoreKind()
    {
		List<eCashStoreMainCategory> listKind = new List<eCashStoreMainCategory>();

		foreach (eCashStoreMainCategory kind in dicData.Keys)
            listKind.Add(kind);

        return listKind;
    }

	public void AddStoreItemInfoKind(eCashStoreMainCategory _mainCategory, eCashStoreSubCategory _subCategory,  Store_Item_Info_Table _storeItemElement)
    {
		if (!dicData.ContainsKey(_mainCategory))
			dicData.Add(_mainCategory, new Dictionary<eCashStoreSubCategory, List<Store_Item_Info_Table>>());

		if (!dicData[_mainCategory].ContainsKey(_subCategory))
			dicData[_mainCategory].Add(_subCategory, new List<Store_Item_Info_Table>());

        dicData[_mainCategory][_subCategory].Add(_storeItemElement);
    }

	//public override string ToString()
	//{
	//    string szResult = string.Empty;
	//    foreach (eCashStoreMainCategory kind in dicData.Keys)
	//    {
	//        szResult += kind.ToString();

	//        foreach (Store_Item_Info_Table storeItemInfo in dicData[kind])
	//        {
	//            szResult += " \n";
	//            szResult += storeItemInfo.ToString();
	//        }
	//    }

	//    return szResult;
	//}
}

public class Cash_Store_Info
{
    private Dictionary<int, Store_Item_Info_Table> dicStoreItem;
	private Dictionary<eCLASS, Cash_Store_Item_Categorization> dicStoreItemByClass;

    public Cash_Store_Info()
    {
        dicStoreItem = new Dictionary<int, Store_Item_Info_Table>();

		dicStoreItemByClass = new Dictionary<eCLASS, Cash_Store_Item_Categorization>();
		dicStoreItemByClass.Add(eCLASS.DIVINEKNIGHT, new Cash_Store_Item_Categorization());
		dicStoreItemByClass.Add(eCLASS.MAGICIAN, new Cash_Store_Item_Categorization());
		dicStoreItemByClass.Add(eCLASS.CLERIC, new Cash_Store_Item_Categorization());
		dicStoreItemByClass.Add(eCLASS.HUNTER, new Cash_Store_Item_Categorization());
		dicStoreItemByClass.Add(eCLASS.PET, new Cash_Store_Item_Categorization());
		dicStoreItemByClass.Add(eCLASS.All, new Cash_Store_Item_Categorization());
    }

    public void AddCashStoreItem(Store_Item_Info_Table _storeItemElement)
    {
        try
        {
            if (!dicStoreItem.ContainsKey(_storeItemElement.ID))
                dicStoreItem.Add(_storeItemElement.ID, _storeItemElement);

            // 분류 작업
            Item item = ItemMgr.ItemManagement.GetItem(System.Convert.ToInt32(_storeItemElement.ID));

            if (item == null)
            {
                string szMessage = "Cash item isn't exist in item table = " + _storeItemElement.ID;
                AsUtil.ShutDown(szMessage);
            }

			eCashStoreMainCategory	mainCategory	= _storeItemElement.MainCategory;
			eCashStoreSubCategory	subCategory		= _storeItemElement.SubCategory;
            eCLASS	classType	= item.ItemData.needClass;

			dicStoreItemByClass[classType].AddStoreItemInfoKind(mainCategory, subCategory, _storeItemElement);

			//if (_storeItemElement.RecommendIdx != 0)
			//{
			//    dicStoreItemByClass[classType].AddStoreItemInfoKind(eCashStoreMainCategory.RECOMMEND, _storeItemElement);
			//}
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

	public List<Store_Item_Info_Table> GetCashStoreItem(eCLASS _class, eCashStoreMainCategory _mainCetegory, eCashStoreSubCategory _subCategory)
    {
		if (dicStoreItemByClass.ContainsKey(_class))
			return dicStoreItemByClass[_class].GetStoreItem(_mainCetegory, _subCategory);
		else
			return new List<Store_Item_Info_Table>();
    }

    public Store_Item_Info_Table GetCashStoreItem(int _id)
    {
        if (dicStoreItem.ContainsKey(_id))
            return dicStoreItem[_id];
        else
            return null;
    }

	public List<eCashStoreMainCategory> GetHaveKind(eCLASS _class)
    {
        if (dicStoreItemByClass.ContainsKey(_class))
            return dicStoreItemByClass[_class].GetStoreKind();

		return new List<eCashStoreMainCategory>();

    }

    public override string ToString()
    {
        string resultString = string.Empty;

        resultString += "\n list count = " + dicStoreItem.Count;

        return resultString;
    }
}

public class Tbl_Cash_Store_Record : AsTableRecord
{
    private Cash_Store_Info m_StoreItemInfo = new Cash_Store_Info();
    public Cash_Store_Info StoreItemInfo { get { return m_StoreItemInfo; } }

    public Tbl_Cash_Store_Record(XmlElement _element)// : base(_element)
    {
        try
        {
            XmlNode node = (XmlElement)_element;

            m_StoreItemInfo = new Cash_Store_Info();

			XmlNodeList storeItemNodeList = node.SelectNodes("CashStoreItem");

			foreach (XmlNode storeItemNode in storeItemNodeList)
			{
				int key			= Int32.Parse(storeItemNode["ShopItemID"].InnerText);
				int itemID		= Int32.Parse(storeItemNode["ItemTableIdx"].InnerText);
				int itemCount	= Int32.Parse(storeItemNode["ItemCount"].InnerText);
				int price		= Int32.Parse(storeItemNode["BuyAmount"].InnerText);
				int itemDescID	= storeItemNode["ItemDesc"].InnerText == "NONE" ? -1 : Int32.Parse(storeItemNode["ItemDesc"].InnerText); 
				int setItemID	= storeItemNode["SetID"].InnerText == "NONE" ? -1 : Int32.Parse(storeItemNode["SetID"].InnerText); 

				Store_Item_Type storeItemType = Store_Item_Type.NormalItem;

				eCashStoreMainCategory	mainCategory	= (eCashStoreMainCategory)Enum.Parse(typeof(eCashStoreMainCategory), storeItemNode["MainCategory"].InnerText, false);
				eCashStoreSubCategory	subCategory		= (eCashStoreSubCategory)Enum.Parse(typeof(eCashStoreSubCategory), storeItemNode["SubCategory"].InnerText, false);
				eSHOPITEMHIGHLIGHT		highlight		= storeItemNode["Highlight"].InnerText == "NONE" ? eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_NONE : (eSHOPITEMHIGHLIGHT)Enum.Parse(typeof(eSHOPITEMHIGHLIGHT), storeItemNode["Highlight"].InnerText, false);

				if (mainCategory == eCashStoreMainCategory.WEAPON || mainCategory == eCashStoreMainCategory.EQUIPMENT || mainCategory == eCashStoreMainCategory.COSTUME || mainCategory == eCashStoreMainCategory.FREE)
					storeItemType = Store_Item_Type.GachaItem;

				Store_Item_Info_Table itemElement = new Store_Item_Info_Table(storeItemType, key, itemID, itemCount, itemDescID, setItemID, highlight, price, mainCategory, subCategory);

				m_StoreItemInfo.AddCashStoreItem(itemElement);
			}
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}


public class Tbl_Cash_Store_Table : AsTableBase
{
    Cash_Store_Info m_ResourceTable = new Cash_Store_Info();

    public Tbl_Cash_Store_Table(string _path)
    {
        m_TableType = eTableType.STORE;

        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root = GetXmlRootElement(_path);
			//XmlNode node = root.SelectSingleNode("CashStore");

			Tbl_Cash_Store_Record record = new Tbl_Cash_Store_Record((XmlElement)root);
           	m_ResourceTable = record.StoreItemInfo;
		}
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }


	public List<Store_Item_Info_Table> GetCashStoreItemInfo(eCLASS _eClass, eCashStoreMainCategory _mainCategory, eCashStoreSubCategory _subCategory)
    {
        List<Store_Item_Info_Table> returnList = new List<Store_Item_Info_Table>();

		List<Store_Item_Info_Table> listStoreItemInfo = m_ResourceTable.GetCashStoreItem(_eClass, _mainCategory, _subCategory);
        List<Store_Item_Info_Table> listStoreItemInfoAll = new List<Store_Item_Info_Table>();
        
        if (_eClass != eCLASS.All)
			listStoreItemInfoAll = m_ResourceTable.GetCashStoreItem(eCLASS.All, _mainCategory, _subCategory);

        if (listStoreItemInfo != null)
            returnList.AddRange(listStoreItemInfo);

        if (listStoreItemInfoAll != null)
            returnList.AddRange(listStoreItemInfoAll);

        return returnList;
    }

    public List<Store_Item_Info_Table> GetCashStoreItemInfo(params int[] _storeItemIDs)
    {
        List<Store_Item_Info_Table> resultList = new List<Store_Item_Info_Table>();

        foreach (int id in _storeItemIDs)
        {
            Store_Item_Info_Table storeItemInfo = m_ResourceTable.GetCashStoreItem(id);

            if (storeItemInfo != null)
                resultList.Add(storeItemInfo);
        }

        return resultList;
    }

	public List<eCashStoreMainCategory> GetHaveItemKind(eCLASS _class)
    {
		List<eCashStoreMainCategory> listKindByType	= m_ResourceTable.GetHaveKind(_class);
		List<eCashStoreMainCategory> listKindAll	= m_ResourceTable.GetHaveKind(eCLASS.All);
		List<eCashStoreMainCategory> listKeys		= new List<eCashStoreMainCategory>();

        // recharge is bagic
		listKeys.Add(eCashStoreMainCategory.RECHARGE_GOLD);
		listKeys.Add(eCashStoreMainCategory.RECHARGE_MIRACLE);
        listKeys.AddRange(listKindByType);
        listKeys.AddRange(listKindAll);

        return listKeys;
    }
}
