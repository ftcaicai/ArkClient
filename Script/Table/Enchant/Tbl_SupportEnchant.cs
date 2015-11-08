using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class Tbl_SupportEnchantTable : AsTableRecord 
{
	private enum eENCHANT_CASH_KIND
	{
		RATIOUP = 0,
		PROTECT
	}
	
	
	private List<int> ratioupList = new List<int>();
	private List<int> protectList = new List<int>();
	
	private int iIndex  = 0;
	private eENCHANT_CASH_KIND ekind; 
	
	public Tbl_SupportEnchantTable(string _path)
	{				
		LoadTable(_path);
	}
	
	public void LoadTable(string _path)
	{				
		try
		{	
			XmlElement root = AsTableBase.GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;	
			
			foreach(XmlNode node in nodes)
			{								
				SetValue( ref iIndex, node, "Index" );
				SetValue< eENCHANT_CASH_KIND>( ref ekind, node, "SupportType" );
				if( eENCHANT_CASH_KIND.RATIOUP == ekind )
				{
					ratioupList.Add( iIndex );
				}
				else
				{
					protectList.Add( iIndex );
				}
				
			}
			
		}
		catch(System.Exception e)
		{
			Debug.LogError("[LoadTable] 'constructor': |" + e.ToString() + "| error while parsing");
		}
	}
	
	public bool IsHaveRatioup( int iIndex )
	{
		foreach( int idata in ratioupList )
		{
			if( iIndex == idata )
				return true;
		}
		
		return false;
	}
	
	
	public bool IsHaveProtect( int iIndex )
	{
		foreach( int idata in protectList )
		{
			if( iIndex == idata )
				return true;
		}
		
		return false;
	}
	
}
