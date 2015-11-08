
//#define LOCAL_TEST

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class Tbl_ChargeRecord : AsTableRecord
{
	public string itemID = string.Empty;
	public eCHARGETYPE chargeType = eCHARGETYPE.eCHARGETYPE_NOTHING;
	public int descriptionID = 0;
	public int useTime = 0;
	public string iconPath = string.Empty;

	public Tbl_ChargeRecord(XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;

			foreach (XmlNode attribute in node.Attributes)
			{
				if (attribute.Name == "ProductID")
					itemID = attribute.Value;
				else if (attribute.Name == "Type")
				{
					if (attribute.Value == "SPHERE")
						chargeType = eCHARGETYPE.eCHARGETYPE_MIRACLE;
					else if (attribute.Value == "DAILY")
						chargeType = eCHARGETYPE.eCHARGETYPE_DAILY;
					else
						chargeType = eCHARGETYPE.eCHARGETYPE_NOTHING;
				}
				else if (attribute.Name == "DescriptionID")
					descriptionID = int.Parse(attribute.Value);
				else if (attribute.Name == "Icon")
					iconPath = attribute.Value;
				else if (attribute.Name == "UseTime")
					useTime = attribute.Value == "NONE" ? 0 : int.Parse(attribute.Value);
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}


public class Tbl_Charge_Table : AsTableBase
{

	Dictionary<string, Tbl_ChargeRecord> m_ResourceTable = new Dictionary<string, Tbl_ChargeRecord>();

	public Tbl_Charge_Table(string _path)
	{
		LoadTable(_path);
	}

	public override void LoadTable(string _path)
	{
		try
		{
			XmlElement root = null;

#if LOCAL_TEST
			XmlDocument xmlDoc = new XmlDocument();
			TextAsset xmlText = Resources.Load(_path) as TextAsset;
			byte[] encodedString = Encoding.UTF8.GetBytes(xmlText.text);
			MemoryStream memoryStream = new MemoryStream(encodedString);

			StreamReader streamReader = new StreamReader(memoryStream);

			StringReader stringReader = new StringReader(streamReader.ReadToEnd());
			string str = stringReader.ReadToEnd();

			xmlDoc.LoadXml(str);

			root = xmlDoc.DocumentElement;
#else
			root = GetXmlRootElement(_path);
#endif

			// 파일이 없다
			if (root == null)
				throw new Exception("File is not exist = " + _path);

			XmlNode topNode = root.SelectSingleNode("Charge");

			XmlNodeList nodes = topNode.ChildNodes;

			foreach (XmlNode node in nodes)
			{
				Tbl_ChargeRecord record = new Tbl_ChargeRecord((XmlElement)node);
				if (m_ResourceTable.ContainsKey(record.itemID) == false)
				{
					m_ResourceTable.Add(record.itemID, record);
					Debug.LogWarning("Add = " + record.itemID);
				}
			}
		}
		catch (System.Exception e)
		{
			System.Diagnostics.Trace.WriteLine(e);
		}
	}

	public Tbl_ChargeRecord GetRecord(string _id)
	{
		if (m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}

		System.Diagnostics.Trace.WriteLine("[Tbl_ChargeRecord]GetRecord: there is no record. id = " + _id);

		return null;
	}


	public Tbl_ChargeRecord[] GetRecordAll()
	{
		List<Tbl_ChargeRecord> list = new List<Tbl_ChargeRecord>();
		foreach (Tbl_ChargeRecord record in m_ResourceTable.Values)
			list.Add(record);

		return list.ToArray();
	}
}



