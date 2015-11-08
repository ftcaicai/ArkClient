
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class Tbl_CashStoreCostume_Record
{
	public int setID;
	public int nameID;
	public eCashStoreMenuMode type;
	public int[] itemIDs;


}

public class Tbl_CashStoreCostume : AsTableBase {

//	static int setItemCosStartIdx = 1000;

	//List<Tbl_CashStoreCostume_Record> m_CashStoreCostumeTable = new  List<Tbl_CashStoreCostume_Record>();
	Dictionary<int, Tbl_CashStoreCostume_Record> dicCashStoreCostumeTable = new Dictionary<int, Tbl_CashStoreCostume_Record>();


	public Tbl_CashStoreCostume(string _path)
	{
		LoadTable(_path);
	}

	public override void LoadTable(string _path)
	{
		XmlElement root = GetXmlRootElement(_path);
		
		XmlNodeList setNodes = root.SelectNodes("SetTable");

		List<int> listItem = new List<int>();

		foreach (XmlNode setNode in setNodes)
		{
			Tbl_CashStoreCostume_Record record = new Tbl_CashStoreCostume_Record();

			foreach (XmlNode node in setNode.ChildNodes)
			{
				if (node.Name == "SetID")
					record.setID = int.Parse(node.InnerText);
				//else if (node.Name == "CostumeType")
				//    record.type = (eCashStoreMenuMode)System.Enum.Parse(typeof(eCashStoreMenuMode), node.Attributes["Type"].Value, false);
				else if (node.Name == "SetName")
					record.nameID = int.Parse(node.InnerText);
				else if (node.Name.Contains("ItemID_"))
				{
					if (node.InnerText != "NONE")
						listItem.Add(int.Parse(node.InnerText));
				}
			}

			record.itemIDs = listItem.ToArray();
			
			if (!dicCashStoreCostumeTable.ContainsKey(record.setID))
				dicCashStoreCostumeTable.Add(record.setID, record);
			
			//m_CashStoreCostumeTable.Add(record);
			listItem.Clear();
		}
	}

	public Tbl_CashStoreCostume_Record[] GetCashStoreCostumeRecordAll()
	{
		return new List<Tbl_CashStoreCostume_Record>(dicCashStoreCostumeTable.Values).ToArray();
	}
	
	public Tbl_CashStoreCostume_Record GetCashStoreCostumeRecord(int _setID)
	{
		if (dicCashStoreCostumeTable.ContainsKey(_setID))
			return dicCashStoreCostumeTable[_setID];
		else
			return null;
	}
	
	//public Tbl_SetItem_Record GetRecord(int _id)
	//{
	//    if(m_SetItemTable.ContainsKey(_id) == true)
	//        return m_SetItemTable[_id];
		
	//    Debug.LogWarning("[Tbl_SetItem_Table]GetRecord: there is no " + _id + "record");
	//    return null;
	//}

	//public int[] GetRowItems(int _setID)
	//{
	//    return m_SetItemTable[_setID].getRowItemIDs();
	//}

	//public List<Tbl_SetItem_Record> GetRecordAll()
	//{
	//    List<Tbl_SetItem_Record> returnList = new List<Tbl_SetItem_Record>();

	//    foreach (Tbl_SetItem_Record record in m_SetItemTable.Values)
	//        returnList.Add(record);

	//    return returnList;
	//}
}

