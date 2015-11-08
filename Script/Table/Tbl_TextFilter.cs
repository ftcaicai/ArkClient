using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public enum eTextFilterType {Name, Chat}

public class Tbl_TextFilter_Record : AsTableRecord
{
	eTextFilterType m_FilterType;
	public eTextFilterType FilterType	{ get { return m_FilterType; } }
	string m_FilterText;
	public string FilterText	{ get { return m_FilterText; } }

	public Tbl_TextFilter_Record( XmlElement _element)
	{
		try
		{
			XmlNode node = ( XmlElement)_element;

			SetValue( ref m_FilterType, node, "Type");
			SetValue( ref m_FilterText, node, "FilterText");
		}
		catch( System.Exception e)
		{
			Debug.LogError( e);
		}
	}
}

public class Tbl_TextFilter_Table : AsTableBase
{
	int m_MaxFilterTextLength = 1;

	Dictionary<string, Tbl_TextFilter_Record> m_dicName = new Dictionary<string, Tbl_TextFilter_Record>();
	Dictionary<string, Tbl_TextFilter_Record> m_dicChat = new Dictionary<string, Tbl_TextFilter_Record>();

	public Tbl_TextFilter_Table( string _path)
	{
		LoadTable( _path);
	}

	public override void LoadTable( string _path)
	{
		try
		{
			XmlElement root = GetXmlRootElement( _path);
			XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
			{
				Tbl_TextFilter_Record record = new Tbl_TextFilter_Record( ( XmlElement)node);

				if( record.FilterText.Length > m_MaxFilterTextLength)
					m_MaxFilterTextLength = record.FilterText.Length;

				string strFilterText = record.FilterText.ToLower();

				switch( record.FilterType)
				{
				case eTextFilterType.Name:
					if( m_dicName.ContainsKey( strFilterText ) == false )
						m_dicName.Add( strFilterText , record);
					break;

				case eTextFilterType.Chat:
					if( m_dicName.ContainsKey( strFilterText ) == false )
						m_dicName.Add( strFilterText , record);

					if( m_dicChat.ContainsKey( strFilterText ) == false )
						m_dicChat.Add( strFilterText , record);
					break;
				}
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError( e);
		}
	}

	public bool CheckFilter_PStoreContent( string _content)
	{
		string content = _content.ToLower();
		int length = content.Length;

		String substr;

		for( int k = 0; k < length; k++)
		{
			for( int i = 1; i <= length && i < m_MaxFilterTextLength && k + i <= length; i++)
			{
				substr = content.Substring( k, i);

				if( m_dicName.ContainsKey( substr) == true)
					return false;
			}
		}

		return true;
	}

	public bool CheckFilter_Name( string _name)
	{
		string content = _name.ToLower();
		int length = content.Length;

		String substr;

		for( int k = 0; k < length; k++)
		{
			for( int i = 1; i <= length && i <= m_MaxFilterTextLength && k + i <= length; i++)
			{
				substr = content.Substring( k, i);

				if( m_dicName.ContainsKey( substr) == true)
					return false;
			}
		}

		return true;
	}

	public bool CheckFilter_Post( string str)
	{
		string strCheck = str.ToLower();
		int length = strCheck.Length;

		String substr;

		for( int k = 0; k < length; k++)
		{
			for( int i = 1; i <= length && i <= m_MaxFilterTextLength && k + i <= length; i++)
			{
				substr = strCheck.Substring( k, i);

				if( m_dicName.ContainsKey( substr) == true)
					return false;
			}
		}

		return true;
	}

	public bool CheckFilter_Guild( string str)
	{
		string strCheck = str.ToLower();
		int length = strCheck.Length;

		String substr;

		for( int k = 0; k < length; k++)
		{
			for( int i = 1; i <= length && i <= m_MaxFilterTextLength && k + i <= length; i++)
			{
				substr = strCheck.Substring( k, i);

				if( true == m_dicName.ContainsKey( substr))
					return false;

				if( true == m_dicChat.ContainsKey( substr))
					return false;
			}
		}

		return true;
	}

	public string TextFiltering_Chat( string _chat)
	{
		if( false == _isEnableFiltering())
			return _chat;

		string extracted = "";
		string content = _chat;

		int cutIdx = _chat.IndexOf( ':');
		if( _chat.IndexOf( '[') < _chat.IndexOf( ']') && _chat.IndexOf( ']') < cutIdx)
		{
			// ilmeda
			extracted = _chat.Substring( 0, cutIdx + 2);
			content = _chat.Remove( 0, cutIdx + 2);
		}

		return extracted + TextFiltering( content, m_dicChat);
	}

	public string TextFiltering_Balloon( string _chat)
	{
		if( false == _isEnableFiltering())
			return _chat;

		return TextFiltering( _chat, m_dicChat);
	}

	readonly char[] separator = new char[] {',', ';', ':', '/', '\'', ',', '|', '"', '\n', '\r', '.', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '|', '-', '=', '`', '[', ']', '}', ' '};
	string TextFiltering( string _text, Dictionary<string, Tbl_TextFilter_Record> _filter)
	{
		string content = _text.ToLower();
		string[] words = content.Split( separator);
		int nCurCheckChar = 0; // ilmeda

		foreach( string str in words)
		{
			int length = str.Length;

			String substr;

			for( int k = 0; k < length; k++)
			{
				for( int i = 1; i <= length && i < m_MaxFilterTextLength && k + i <= length; i++)
				{
					substr = str.Substring( k, i);

					if( _filter.ContainsKey( substr) == true)
					{
						string alternate = "";
						for( int j = 0; j < substr.Length; j++)
						{
							alternate += "*";
						}

						// < ilmeda
						_text = _text.Remove( k + nCurCheckChar, i);
						_text = _text.Insert( k + nCurCheckChar, alternate);
						// ilmeda >

						Debug.Log( "Tbl_TextFilter::TextFiltering: Replaced text is " + content);
					}
				}
			}

			nCurCheckChar += ( str.Length + 1); // ilmeda
		}

		return _text;
	}

	private bool _isEnableFiltering()
	{
		return AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_Filtering);
	}
}
