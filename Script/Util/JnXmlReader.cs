using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

class JnXmlReader
{
	protected XmlDocument doc_;
	protected XmlElement current_;
	
	private JnXmlReader() { }

	protected bool CheckEncoding( string encoding)
	{
		string decl = "";
		decl = doc_.FirstChild.InnerText;
		if( decl.LastIndexOf( encoding) != 0)
		{
			current_ = doc_.DocumentElement;
			return true;
		}

		return false;
	}

	public static JnXmlReader FromString( string str)
	{
		JnXmlReader reader = new JnXmlReader();
		reader.doc_ = null;
		reader.doc_ = new XmlDocument();

		try
		{
			reader.doc_.LoadXml( str);
		}
		catch
		{
			Console.WriteLine( "XmlReader's constructor : Failed to parse xml");
		}

		if( reader.doc_ == null)
			Console.WriteLine( "XmlReader::FromString() : Failed to parse xml");
		if( reader.CheckEncoding( "UTF-8") == false)
			Console.WriteLine( "XmlReader::FromString() : Failed to parse xml");

		reader.current_ = reader.doc_.DocumentElement;

		return reader;
	}

	public JnXmlReader( string fileName)
	{
		doc_ = new XmlDocument();
		try
		{
			doc_.Load( fileName);
		}
		catch
		{
			Console.WriteLine( "XmlReader's constructor : Failed to parse xml");
		}

		if( doc_ == null)
			Console.WriteLine( "XmlReader's constructor : Failed to parse xml");

		if( CheckEncoding( "UTF-8") == false)
			Console.WriteLine( "XmlReader's constructor : xml encoding is not UTF-8");
	}

	public string GetCurrentNodeName()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::GetCurrentNodeName() : current_ is null");
			return "";
		}

		return current_.Name;
	}

	public bool Goto( string path)
	{
		if( path == null || path == "")
			return false;

		XmlNode node;
	
		char[] seperator = new char[1];
		seperator[0] = '/';
		string[] str = path.Split( seperator);
	
		if( path[0] == '/')
		{
			node = ( XmlNode)doc_.DocumentElement;
			List<string> list = str.ToList();
			list.RemoveAt( 0); list.RemoveAt( 0);
			str = list.ToArray();
		}
		else
		{
			if( current_ == null)
			{
				Console.WriteLine( "XmlReader::Goto( string path) : current_ is null");
				return false;
			}
			node = ( XmlNode)current_;
		}
	
		if( str == null)
		{
			Console.WriteLine( "XmlReader::Goto( string path) : invalid path");
			return false;
		}

		foreach ( string data in str)
		{
			if( data == "." || data == "")
			{
			}
			else if( data == "..")
			{
				try
				{
					node = node.ParentNode;
					if( node == ( XmlNode)doc_.DocumentElement)
					{
						Console.WriteLine( "XmlReader::Goto( string path) : invalid path");
						return false;
					}
				}
				catch
				{
					Console.WriteLine( "XmlReader::Goto( string path) : invalid path");
					return false;
				}
			}
			else
			{
				try
				{
					node = node.SelectSingleNode( data);
				}
				catch
				{
					Console.WriteLine( "XmlReader::Goto( string path) : invalid path");
					return false;
				}
			}
		}

		if( node == null)
			Console.WriteLine( "XmlReader::Goto( string path) : invalid path");

		current_ = ( XmlElement)node;

		return true;
	}

	public bool FirstChild( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::FirstChild() : current_ is null");
			return false;
		}

		foreach( XmlNode node in current_.ChildNodes)
		{
			if( node.Name == name)
			{
				current_ = ( XmlElement)node;
				return true;
			}
		}

		return false;
	}

	public bool NextSibling()
	{
		if( current_ == null)
		{
			Debug.Log( "XmlReader::NextSibling() : current_ is null");
			return false;
		}

		XmlElement originElement = current_;
		string curName = current_.Name;

		current_ = ( XmlElement)current_.NextSibling;

		if( current_ == null)
		{
			//Debug.Log( "XmlReader::NextSibling() : no next sibling");
			current_ = originElement;
			return false;
		}
		else if( current_.Name != curName)
		{
			//Debug.Log( "XmlReader::NextSibling() : no same name Element in the parent");
			current_ = originElement;
			return false;
		}

		if( current_.Name == curName)
			return true;

		return false;
	}

	public bool Parent()
	{
		if( current_ == null)
		{
			Debug.Log( "XmlReader::Parent() : current_ is null");
			return false;
		}

		XmlElement parent = ( XmlElement)current_.ParentNode;
		if( parent != doc_.DocumentElement)
		{
			current_ = parent;
			return true;
		}

		Debug.Log( "XmlReader::Parent() : current_'s parent node is null");
		return false;
	}

	public bool ReadAttribute( string name, out string value)
	{
		value = "";

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);
		if( attr != null)
		{
			value = attr;
			return true;
		}

		return false;
	}

	public bool ReadAttribute( string name, out int value)
	{
		value = 0;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			value = int.Parse( attr);
			return true;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : invalid value( value is not int)");
			return false;
		}
	}

	public bool ReadAttribute( string name, out float value)
	{
		value = 0.0f;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			value = float.Parse( attr);
			return true;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : invalid value( value is not float)");
			return false;
		}
	}

	public bool ReadAttribute( string name, out bool value)
	{
		value = false;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			value = bool.Parse( attr);
			return true;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : invalid value( value is not bool)");
			return false;
		}
	}

	public bool ReadAttribute( string name, out Vector3 value)
	{
		value = new Vector3();

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			char[] arr = new char[1];
			arr[0] = ',';
			string[] strArr = new string[3];
			strArr = attr.Split( arr);
			value.x = float.Parse( strArr[0]);
			value.y = float.Parse( strArr[1]);
			value.z = float.Parse( strArr[2]);
	
			return true;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : invalid value( value is not Vector3)");
			return false;
		}
	}

	public string ReadAttrAsString( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return "";
		}

		string attr = current_.GetAttribute( name);
		if( attr != null)
			return attr;

		Console.WriteLine( "XmlReader::ReadAttrAsString : Failed to read attribute");
		return "";
	}

	public int ReadAttrAsInt( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttrAsInt() : current_ is null");
			return 0;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			return int.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttrAsInt() : invalid value( value is not int)");
			return 0;
		}
	}

	public float ReadAttrAsFloat( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttrAsFloat() : current_ is null");
			return 0.0f;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			return float.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttrAsFloat() : invalid value( value is not float)");
			return 0.0f;
		}
	}

	public bool ReadAttrAsBool( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttrAsBool() : current_ is null");
			return false;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			return bool.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttrAsBool() : invalid value( value is not bool)");
			return false;
		}
	}

	public Vector3 ReadAttrAsVector3( string name)
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : current_ is null");
			return Vector3.zero;
		}

		string attr = current_.GetAttribute( name);

		try
		{
			Vector3 value = new Vector3();
	
			char[] arr = new char[1];
			arr[0] = ',';
			string[] strArr = new string[3];
			strArr = attr.Split( arr);
			value.x = float.Parse( strArr[0]);
			value.y = float.Parse( strArr[1]);
			value.z = float.Parse( strArr[2]);
	
			return value;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAttribute() : invalid value( value is not Vector3)");
			return Vector3.zero;
		}
	}

	public bool Read( out string value)
	{
		value = "";

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::Read( string value) : current_ is null");
			return false;
		}

		//string text = current_.Value;
		string text = current_.InnerText;

		if( text != null)
		{
			value = text;
			return true;
		}

		return false;
	}

	public bool Read( out int value)
	{
		value = 0;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::Read( out int value) : current_ is null");
			return false;
		}

		//string text = current_.Value;
		string text = current_.InnerText;

		if( text != null)
		{
			value = int.Parse( text);
			return true;
		}

		return false;
	}

	public bool Read( out float value)
	{
		value = 0.0f;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::Read( float value) : current_ is null");
			return false;
		}

		//string text = current_.Value;
		string text = current_.InnerText;

		if( text != null)
		{
			value = float.Parse( text);
			return true;
		}

		return false;
	}

	public bool Read( out bool value)
	{
		value = false;

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::Read( bool value) : current_ is null");
			return false;
		}

		//string text = current_.Value;
		string text = current_.InnerText;

		if( text != null)
		{
			value = bool.Parse( text);
			return true;
		}

		return false;
	}

	public bool Read( out Vector3 value)
	{
		value = new Vector3();

		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::Read( out Vector3 value) : current_ is null");
			return false;
		}

		string attr = current_.Value;

		try
		{
			char[] arr = new char[1];
			arr[0] = ',';
			string[] strArr = new string[3];
			strArr = attr.Split( arr);
			value.x = float.Parse( strArr[0]);
			value.y = float.Parse( strArr[1]);
			value.z = float.Parse( strArr[2]);
	
			return true;
		}
		catch
		{
			Console.WriteLine( "XmlReader::Read( out Vector3 value) : invalid value( value is not Vector3)");
			return false;
		}
	}

	public string ReadAsString()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAsString() : current_ is null");
			return "";
		}

		//string text = current_.Value;
		string text = current_.InnerText;

		if( text != null)
			return text;

		return "";
	}

	public int ReadAsInt()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAsInt() : current_ is null");
			return 0;
		}

		//string attr = current_.Value;
		string attr = current_.InnerText;

		try
		{
			return int.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAsInt() : invalid value( value is not int)");
			return 0;
		}
	}

	public float ReadAsFloat()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAsFloat() : current_ is null");
			return 0.0f;
		}

		//string attr = current_.Value;
		string attr = current_.InnerText;

		try
		{
			return float.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAsFloat() : invalid value( value is not int)");
			return 0.0f;
		}
	}

	public bool ReadAsBool()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAsBool() : current_ is null");
			return false;
		}

		//string attr = current_.Value;
		string attr = current_.InnerText;

		try
		{
			return bool.Parse( attr);
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAsBool() : invalid value( value is not int)");
			return false;
		}
	}

	public Vector3 ReadAsVector3()
	{
		if( current_ == null)
		{
			Console.WriteLine( "XmlReader::ReadAsVector3() : current_ is null");
			return Vector3.zero;
		}

		//string attr = current_.Value;
		string attr = current_.InnerText;

		try
		{
			Vector3 value = new Vector3();
	
			char[] arr = new char[1];
			arr[0] = ',';
			string[] strArr = new string[3];
			strArr = attr.Split( arr);
			value.x = float.Parse( strArr[0]);
			value.y = float.Parse( strArr[1]);
			value.z = float.Parse( strArr[2]);
	
			return value;
		}
		catch
		{
			Console.WriteLine( "XmlReader::ReadAsVector3() : invalid value( value is not Vector3)");
			return Vector3.zero;
		}
	}
}
