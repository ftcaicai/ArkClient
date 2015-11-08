using UnityEngine;
using System.Collections;
using System.Xml;

public class VersionData : AsTableRecord
{
	public string version = string.Empty;
	
	public VersionData( XmlNode node)
	{
		SetValue( ref version, node, "number");
	}
}

public class VersionManager
{
	static private VersionManager instance = null;
	static public VersionManager Instance
	{
		get
		{
			if( null == instance)
				instance = new VersionManager();
			
			return instance;
		}
	}
	
	static public string version = string.Empty;
	
	private VersionManager()
	{
	}
	
	public void LoadTable()
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( "Table/Version");
			XmlNodeList nodes = root.ChildNodes;
			
			foreach( XmlNode node in nodes)
			{
				VersionData data = new VersionData( node);
				version = string.Format( "{0}.{1}{2}", AsGameDefine.BUNDLE_VERSION, data.version, AsNetworkDefine.ENVIRONMENT);
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}
