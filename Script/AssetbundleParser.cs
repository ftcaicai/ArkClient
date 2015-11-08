using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

public class AssetbundleParser : MonoBehaviour
{
	public struct stPatcherTable
	{
		public int _nID;
		public string _strPath;
		public int _nTime;
		public string _str_1;
		public string _str_2;
		public string _str_3;
		public string _str_4;
		public string _str_5;
		
		public stPatcherTable(int nID, string strPath, int nTime, string str_1, string str_2, string str_3, string str_4, string str_5)
		{
			_nID = nID;
			_strPath = strPath;
			_nTime = nTime;
			_str_1 = str_1;
			_str_2 = str_2;
			_str_3 = str_3;
			_str_4 = str_4;
			_str_5 = str_5;
		}
	}

	private Dictionary<int, string> m_dStringTable = new Dictionary<int, string>();
	private Dictionary<int, stPatcherTable> m_dPatcherTable_1 = new Dictionary<int, stPatcherTable>();
	private Dictionary<int, stPatcherTable> m_dPatcherTable_2 = new Dictionary<int, stPatcherTable>();
	private Dictionary<int, stPatcherTable> m_dPatcherTable_3 = new Dictionary<int, stPatcherTable>();
	private Dictionary<int, stPatcherTable> m_dPatcherTable_4 = new Dictionary<int, stPatcherTable>();
	private Dictionary<int, stPatcherTable> m_dPatcherTable_5 = new Dictionary<int, stPatcherTable>();

	void Start()
	{
	}

	void Update()
	{
	}
	
	public void LoadTable()
	{
		_LoadPatcherStringTable();
		_LoadPatcherTable();
	}
	
	public string GetPatcherString(int nIndex)
	{
		if( 0 == nIndex)
			return "";
		
		int nKey = nIndex;

		/*
		switch( Application.systemLanguage)
		{
		case SystemLanguage.Korean:
			nKey = nIndex;
			break;
			
		case SystemLanguage.Japanese:
			nKey = nIndex + 1000;
			break;
			
		case SystemLanguage.English:
			nKey = nIndex + 2000;
			break;
		}
		*/
		
		if( true == m_dStringTable.ContainsKey( nKey))
		{
			if( null != m_dStringTable[nKey])
				return m_dStringTable[nKey];
		}
		
		return "can not find string: " + nKey;
	}
	
	public string GetPatcherString(stPatcherTable stData, int nStringIndex)
	{
		switch( nStringIndex)
		{
		case 1:
			return stData._str_1;
		case 2:
			return stData._str_2;
		case 3:
			return stData._str_3;
		case 4:
			return stData._str_4;
		case 5:
			return stData._str_5;
		default:
			return "";
		}
	}
	
	public stPatcherTable GetPatcherTableData(ePatchGroup PatchGroup, int nID)
	{
		switch( PatchGroup)
		{
		case ePatchGroup.ePatchGroup_1:
			return m_dPatcherTable_1[nID];
		case ePatchGroup.ePatchGroup_2:
			return m_dPatcherTable_2[nID];
		case ePatchGroup.ePatchGroup_3:
			return m_dPatcherTable_3[nID];
		case ePatchGroup.ePatchGroup_4:
			return m_dPatcherTable_4[nID];
		case ePatchGroup.ePatchGroup_5:
			return m_dPatcherTable_5[nID];
		default:
			Debug.Log( "Error - AssetbundleParser::GetPatcherTableData() - PatchGroup:" + PatchGroup + ", ID: " + nID);
			return m_dPatcherTable_1[1];
		}
	}
	
	public int GetPatcherListCount(ePatchGroup PatchGroup)
	{
		switch( PatchGroup)
		{
		case ePatchGroup.ePatchGroup_1:
			return m_dPatcherTable_1.Count;
		case ePatchGroup.ePatchGroup_2:
			return m_dPatcherTable_2.Count;
		case ePatchGroup.ePatchGroup_3:
			return m_dPatcherTable_3.Count;
		case ePatchGroup.ePatchGroup_4:
			return m_dPatcherTable_4.Count;
		case ePatchGroup.ePatchGroup_5:
			return m_dPatcherTable_5.Count;
		default:
			return 0;
		}
	}

	// < private function
	private void _LoadPatcherStringTable()
	{
		TextAsset xmlText = Resources.Load( "UseScript/PatcherStringTable") as TextAsset;
		byte[] encodedString = Encoding.UTF8.GetBytes( xmlText.text);
		MemoryStream memoryStream = new MemoryStream(encodedString);
		StreamReader streamReader = new StreamReader(memoryStream);
		StringReader stringReader = new StringReader(streamReader.ReadToEnd());
		string str = stringReader.ReadToEnd();
		
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(str);

		XmlNodeList xnList = xmlDoc.SelectNodes( "StringTable/String");
		
		foreach( XmlNode xn in xnList)
		{
			int nIndex = int.Parse( xn["Index"].InnerText);
			string strText = xn["String"].InnerText;
			
			m_dStringTable.Add( nIndex, strText);
		}
		
		memoryStream.Close();
		streamReader.Close();
	}

	private void _LoadPatcherTable()
	{
		TextAsset xmlText = Resources.Load( "UseScript/PatcherTable") as TextAsset;
		byte[] encodedString = Encoding.UTF8.GetBytes( xmlText.text);
		MemoryStream memoryStream = new MemoryStream(encodedString);
		StreamReader streamReader = new StreamReader(memoryStream);
		StringReader stringReader = new StringReader(streamReader.ReadToEnd());
		string str = stringReader.ReadToEnd();
		
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(str);

		XmlNodeList xnList = xmlDoc.SelectNodes( "PatcherTable/Patcher");
		
		foreach( XmlNode xn in xnList)
		{
			int nID = int.Parse( xn["ID"].InnerText);
			
			string strPath = "";
			if( false == xn["FilePath"].InnerText.ToLower().Contains( "none"))
				strPath = xn["FilePath"].InnerText;
			
			int nTime = int.Parse( xn["Time"].InnerText);
			
			int nStr_1 = 0;
			if( false == xn["String_1"].InnerText.ToLower().Contains( "none"))
				nStr_1 = int.Parse( xn["String_1"].InnerText);
			
			int nStr_2 = 0;
			if( false == xn["String_2"].InnerText.ToLower().Contains( "none"))
				nStr_2 = int.Parse( xn["String_2"].InnerText);
			
			int nStr_3 = 0;
			if( false == xn["String_3"].InnerText.ToLower().Contains( "none"))
				nStr_3 = int.Parse( xn["String_3"].InnerText);
			
			int nStr_4 = 0;
			if( false == xn["String_4"].InnerText.ToLower().Contains( "none"))
				nStr_4 = int.Parse( xn["String_4"].InnerText);
			
			int nStr_5 = 0;
			if( false == xn["String_5"].InnerText.ToLower().Contains( "none"))
				nStr_5 = int.Parse( xn["String_5"].InnerText);
			
			stPatcherTable stData = new stPatcherTable( nID, strPath, nTime,
				GetPatcherString( nStr_1), GetPatcherString( nStr_2), GetPatcherString( nStr_3), GetPatcherString( nStr_4), GetPatcherString( nStr_5));
			
			int nRes = nID / 100;
			int nResID = nID % 100;
			if( 1 == nRes)
				m_dPatcherTable_1.Add( nResID, stData);
			else if( 2 == nRes)
				m_dPatcherTable_2.Add( nResID, stData);
			else if( 3 == nRes)
				m_dPatcherTable_3.Add( nResID, stData);
			else if( 4 == nRes)
				m_dPatcherTable_4.Add( nResID, stData);
			else if( 5 == nRes)
				m_dPatcherTable_5.Add( nResID, stData);
		}
		
		memoryStream.Close();
		streamReader.Close();
	}
	// private function >
}
