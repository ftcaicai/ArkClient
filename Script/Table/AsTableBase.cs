//#define SHOW_EXCEPTION

using UnityEngine;
using System.Collections;

using System;
using System.Xml;
using System.IO;
using System.Text;

public abstract class AsTableRecord
{
	protected string m_CurrentColumn;
	
	#region - int -
	protected void SetValue(ref int _target, XmlNode _node, string _column, int _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(int) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = int.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
//				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
//					_node[_column].InnerText);
			}
				
			_target = _def;
		}
	}
	
	protected void SetValue(ref ulong _target, XmlNode _node, string _column, ulong _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(ulong) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = ulong.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
//				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
//					_node[_column].InnerText);
			}
				
			_target = _def;
		}
	}
	
	protected void SetValue(ref long _target, XmlNode _node, string _column, long _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(long) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = long.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
			}
				
			_target = _def;
		}
	}
	
	protected void SetValue( ref int _target, XmlNode _node)
	{
		try
		{
			_target = int.Parse( _node.InnerText);
		}
		catch
		{
			_target = int.MaxValue;
		}
	}
	
	protected void SetValue(ref int _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, int.MaxValue);
	}
	
	
	protected void SetValue(ref ulong _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, ulong.MaxValue);
	}
	
	protected void SetValue(ref long _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, long.MaxValue);
	}
	
	#endregion
	#region - float -
	protected void SetValue(ref float _target, XmlNode _node, string _column, float _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(float) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = float.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
//				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
//					_node[_column].InnerText);
			}
			
			_target = _def;
		}
	}
	
	protected void SetValue(ref float _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, float.MaxValue);
	}
	#endregion
	#region - uint -
	protected void SetValue(ref uint _target, XmlNode _node, string _column, uint _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(uint) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = uint.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION			
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
//				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
//					_node[_column].InnerText);
			}
			
			_target = _def;
		}
	}
	
	protected void SetValue(ref uint _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, uint.MaxValue);
	}
	#endregion
	
	
	
	#region - bool -
	protected void SetValue(ref bool _target, XmlNode _node, string _column, bool _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(bool) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = bool.Parse(_node[_column].InnerText);
		}
		catch
		{
			ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
#if SHOW_EXCEPTION			
			Debug.LogError("AsTableRecord::SetValue: invalid parsing on bool(node[" + _column + "] | value:" +
				_node[_column].InnerText);
#endif
			_target = _def;
		}
	}
	
	protected void SetValue(ref bool _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, false);
	}
	#endregion
	#region - string -
	protected void SetValue(ref string _target, XmlNode _node, string _column, string _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(string) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = _node[_column].InnerText;
//			char[] separator = new char[1];separator[0] = '@';
//			string[] splits = _target.Split(separator);
//			if(splits.Length >= 2)
//				_target = splits[splits.Length - 1];
		}
		catch
		{
			ShutdownTable("AsTableRecord::SetValue: invalid parsing on string(node[" + _column + "] | value:" +
				_node[_column].InnerText);
#if SHOW_EXCEPTION
			Debug.LogError("AsTableRecord::SetValue: invalid parsing on string(node[" + _column + "] | value:" +
				_node[_column].InnerText);
#endif
			_target = _def;
		}
	}
	
	protected void SetValue(ref string _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, "NONE");
	}
	
	protected void SetStringFullPath(ref string _target, XmlNode _node, string _column)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetStringFullPath(string) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = _node[_column].InnerText;
		}
		catch
		{
			ShutdownTable("AsTableRecord::SetStringFullPath: invalid parsing on string(node[" + _column + "] | value:" +
				_node[_column].InnerText);
#if SHOW_EXCEPTION			
			Debug.LogError("AsTableRecord::SetStringFullPath: invalid parsing on string(node[" + _column + "] | value:" +
				_node[_column].InnerText);
#endif
			_target = "NONE";
		}
	}
	#endregion
	#region - enum -
	protected void SetValue<T>( ref T _target, XmlNode _node, string _column)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetEnumValue<" + typeof(T).ToString() + "> - column[" + _column + "] not exist");
			return;
		}
		
		if( false == typeof(T).IsEnum)
		{
			Debug.LogWarning( "AsTableRecord::SetEnumValue: target value must be Enum");
			return;
		}
		
		try
		{
//			if( null == _node[ _column])
//			{
//				Debug.LogWarning( "AsTableRecord::SetEnumValue<" + typeof(T).ToString() + "> - column[" + _column + "] is not exist");
//				return;
//			}
			
			_target = (T)Enum.Parse(typeof(T), _node[_column].InnerText, true);
		}
		catch
		{
#if SHOW_EXCEPTION			
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			if(_node[_column] == null)
			{
#if SHOW_EXCEPTION				
				Debug.LogWarning("AsTableRecord::SetValue: [" + _column + "] setting default");
				_target = default(T);
#endif
			}
			else
			{
				ShutdownTable("AsTableRecord::SetEnumValue: " +
					"invalid enum parsing on xmlnode(node[" + _column + "] | value:" +
					_node[_column].InnerText);
#if SHOW_EXCEPTION				
				Debug.LogError("AsTableRecord::SetEnumValue: " +
					"invalid enum parsing on xmlnode(node[" + _column + "] | value:" +
					_node[_column].InnerText);
#endif
			}
		}
	}
	#endregion
	
	#region - sbyte -
	protected void SetValue(ref sbyte _target, XmlNode _node, string _column)
	{
		SetValue(ref _target, _node, _column, sbyte.MaxValue);
	}
	
	protected void SetValue(ref sbyte _target, XmlNode _node, string _column, sbyte _def)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetValue(uint) - column[" + _column + "] not exist");
			return;
		}
		
		try{
			_target = sbyte.Parse(_node[_column].InnerText);
		}
		catch
		{
#if SHOW_EXCEPTION			
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _column + "]");
#endif
			if(string.Compare("NONE", _node[_column].InnerText, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
					_node[_column].InnerText);
//				Debug.LogError("AsTableRecord::SetValue: invalid parsing on int(node[" + _column + "] | value:" +
//					_node[_column].InnerText);
			}
			
			_target = _def;
		}
	}
	#endregion
	
	#region - set string from string table -
	protected void SetString(ref string _target, XmlNode _node, string _column)
	{
		m_CurrentColumn = _column;
		
		if( null == _node[ _column])
		{
			Debug.LogWarning( "AsTableRecord::SetString - column[" + _column + "] not exist");
			return;
		}
		
		try{
			int index = int.Parse(_node[_column].InnerText);
			if( null == AsTableManager.Instance )
			{
#if SHOW_EXCEPTION				
				Debug.LogWarning("null == AsTableManager.Instance");
#endif
				_target = _node[_column].InnerText;
			}
			else
			{
				_target = AsTableManager.Instance.GetTbl_String_Record(index).String;
			}
		}
		catch
		{
			if(string.Compare(_node[_column].InnerText, "NONE", true) == 0)
			{
				_target = "NONE";
			}
			else
			{
				ShutdownTable("AsTableRecord::SetValue: invalid parsing on bool(node[" + _column + "] | value:" +
					_node[_column].InnerText);
#if SHOW_EXCEPTION				
				Debug.LogError("AsTableRecord::SetValue: invalid parsing on bool(node[" + _column + "] | value:" +
					_node[_column].InnerText);
#endif
				_target = "INVALID_STRING";
			}
		}
	}
	#endregion
	
	//attr
	#region - int -
	protected void SetAttribute(ref int _target, XmlNode _node, string _attr, int _def)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[_attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute(int): column[" + _attr + "] not exist");
			return;
		}
		
		try{
			_target = int.Parse(_node.Attributes[_attr].Value);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#endif
			
			if(string.Compare("NONE", _node.Attributes[_attr].Value, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetAttribute: invalid parsing on int(node[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
//				Debug.LogError("AsTableRecord::SetAttribute: invalid parsing on int(node[" + _attr + "] | value:" +
//					_node.Attributes[_attr].Value);
			}
				
			_target = _def;
		}
	}
	
	protected void SetAttribute(ref int _target, XmlNode _node, string _attr)
	{
		SetAttribute(ref _target, _node, _attr, int.MaxValue);
	}
	#endregion
	#region - float -
	protected void SetAttribute(ref float _target, XmlNode _node, string _attr, float _def)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[_attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute(float): column[" + _attr + "] not exist");
			return;
		}
		
		try{
			_target = float.Parse(_node.Attributes[_attr].Value);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#endif
			
			if(string.Compare("NONE", _node.Attributes[_attr].Value, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetAttribute: invalid parsing on float(node[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
//				Debug.LogError("AsTableRecord::SetAttribute: invalid parsing on float(node[" + _attr + "] | value:" +
//					_node.Attributes[_attr].Value);
			}
				
			_target = _def;
		}
	}
	
	protected void SetAttribute(ref float _target, XmlNode _node, string _attr)
	{
		SetAttribute(ref _target, _node, _attr, float.MaxValue);
	}
	#endregion
	#region - uint -
	protected void SetAttribute(ref uint _target, XmlNode _node, string _attr, uint _def)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[_attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute(uint): column[" + _attr + "] not exist");
			return;
		}
		
		try{
			_target = uint.Parse(_node.Attributes[_attr].Value);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#endif
			
			if(string.Compare("NONE", _node.Attributes[_attr].Value, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetAttribute: invalid parsing on uint(node[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
//				Debug.LogError("AsTableRecord::SetAttribute: invalid parsing on uint(node[" + _attr + "] | value:" +
//					_node.Attributes[_attr].Value);
			}
				
			_target = _def;
		}
	}
	
	protected void SetAttribute(ref uint _target, XmlNode _node, string _attr)
	{
		SetAttribute(ref _target, _node, _attr, uint.MaxValue);
	}
	#endregion
	#region - bool -
	protected void SetAttribute(ref bool _target, XmlNode _node, string _attr, bool _def)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[_attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute(bool): column[" + _attr + "] not exist");
			return;
		}
		
		try{
			_target = bool.Parse(_node.Attributes[_attr].Value);
		}
		catch
		{
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#endif
			
			if(string.Compare("NONE", _node.Attributes[_attr].Value, true) != 0)
			{
				ShutdownTable("AsTableRecord::SetAttribute: invalid parsing on bool(node[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
//				Debug.LogError("AsTableRecord::SetAttribute: invalid parsing on bool(node[" + _attr + "] | value:" +
//					_node.Attributes[_attr].Value);
			}
				
			_target = _def;
		}
	}
	
	protected void SetAttribute(ref bool _target, XmlNode _node, string _attr)
	{
		SetAttribute(ref _target, _node, _attr, false);
	}
	#endregion
	#region - string -
	protected void SetAttribute(ref string _target, XmlNode _node, string _attr, string _def)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[_attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute(string): column[" + _attr + "] not exist");
			return;
		}
		
		try{
			_target = _node.Attributes[_attr].Value;
		}
		catch
		{
			ShutdownTable("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#if SHOW_EXCEPTION
			Debug.LogWarning("AsTableRecord::SetValue: special case(node[" + _attr + "]");
#endif
			_target = _def;
		}
	}
	
	protected void SetAttribute(ref string _target, XmlNode _node, string _attr)
	{
		SetAttribute(ref _target, _node, _attr, "NONE");
	}
	#endregion
	#region - enum -
	protected void SetAttribute<T>( ref T _target, XmlNode _node, string _attr)
	{
		m_CurrentColumn = _attr;
		
		if( null == _node.Attributes[ _attr])
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute<" + typeof(T) + "> - column[" + _attr + "] not exist");
			return;
		}
		
		if( false == typeof(T).IsEnum)
		{
			Debug.LogWarning( "AsTableRecord::SetAttribute: target value must be Enum");
			return;
		}
		
		try
		{
			_target = (T)Enum.Parse( typeof(T), _node.Attributes[_attr].Value, true);
		}
		catch
		{
#if SHOW_EXCEPTION			
			Debug.LogWarning("AsTableRecord::SetAttribute: special case(node[" + _attr + "]");
#endif
			if(_node[_attr] == null)
			{
#if SHOW_EXCEPTION				
				Debug.LogWarning("AsTableRecord::SetAttribute: [" + _attr + "] setting default");
				_target = default(T);
#endif
			}
			else
			{
				ShutdownTable("AsTableRecord::SetAttribute: " +
					"invalid enum parsing on xmlnode(_node.Attributes[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
#if SHOW_EXCEPTION				
				Debug.LogError("AsTableRecord::SetAttribute: " +
					"invalid enum parsing on xmlnode(_node.Attributes[" + _attr + "] | value:" +
					_node.Attributes[_attr].Value);
#endif
			}
		}
	}
	#endregion
	
	#region - method -
	void ShutdownTable(string _log)
	{
		if(AsTableManager.Instance != null)
			AsTableManager.Instance.Shutdown(_log);
	}
	#endregion
}

public abstract class AsTableBase
{
	protected eTableType m_TableType;

	public abstract void LoadTable( string _path);
	
	public static XmlElement  GetXmlRootElement( string _path)
	{
		XmlDocument xmlDoc = new XmlDocument();
		try
		{
			TextAsset xmlText = ResourceLoad.LoadTextAsset( _path);

			byte[] encodedString = Encoding.UTF8.GetBytes( xmlText.text);
			MemoryStream memoryStream = new MemoryStream( encodedString);

			StreamReader streamReader = new StreamReader( memoryStream );
			
			StringReader stringReader = new StringReader( streamReader.ReadToEnd());
			string str = stringReader.ReadToEnd();
			
			xmlDoc.LoadXml( str);
			
			return xmlDoc.DocumentElement;
		}
		catch( SystemException e)
		{
			Debug.LogError(e);
			AsTableManager.Instance.Shutdown( "[Tbl_Table_Base]LoadTable: error while load xml [ path : " + _path);
			return null;
		}
	}
}
