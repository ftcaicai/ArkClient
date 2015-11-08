using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Xml;
using System.IO;
using System.Text;

public class Tbl_Emotion_Record : AsTableRecord//
{
	int m_Index;public int Index{get{return m_Index;}}
	string m_EmotionName;public string EmotionName{get{return m_EmotionName;}}
	List<string> m_listCommand;public List<string> listCommand{get{return m_listCommand;}}
	
	int m_DivineKnightAction_Male;public int DivineKnightAction_Male{get{return m_DivineKnightAction_Male;}}
	int m_DivineKnightAction_Female;public int DivineKnightAction_Female{get{return m_DivineKnightAction_Female;}}
	
	int m_ClericAction_Male;public int ClericAction_Male{get{return m_ClericAction_Male;}}
	int m_ClericAction_Female;public int ClericAction_Female{get{return m_ClericAction_Female;}}
	
	int m_MagicianAction_Male;public int MagicianAction_Male{get{return m_MagicianAction_Male;}}
	int m_MagicianAction_Female;public int MagicianAction_Female{get{return m_MagicianAction_Female;}}

	int m_HunterAction_Male;public int HunterAction_Male{get{return m_HunterAction_Male;}}
	int m_HunterAction_Female;public int HunterAction_Female{get{return m_HunterAction_Female;}}
	
	public Tbl_Emotion_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_listCommand = new List<string>();
			
			SetValue(ref m_Index, node, "Index");
			SetValue(ref m_EmotionName, node, "EmotionName");
			
			for(int i=1; i<=5; ++i)
			{
				string name = "Command" + i;
				string str = "";
				SetValue(ref str, node, name);
				if(str != "NONE")
					m_listCommand.Add(str);
			}
			
			SetValue(ref m_DivineKnightAction_Male, node, "DivineKnightAction_Male");
			SetValue(ref m_DivineKnightAction_Female, node, "DivineKnightAction_Female");
			
			SetValue(ref m_ClericAction_Male, node, "ClericAction_Male");
			SetValue(ref m_ClericAction_Female, node, "ClericAction_Female");
			
			SetValue(ref m_MagicianAction_Male, node, "MagicianAction_Male");
			SetValue(ref m_MagicianAction_Female, node, "MagicianAction_Female");
			
			SetValue(ref m_HunterAction_Male, node, "HunterAction_Male");
			SetValue(ref m_HunterAction_Female, node, "HunterAction_Female");
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class Tbl_Emotion_Table : AsTableBase {
		
	SortedList<int, Tbl_Emotion_Record> m_ResourceTable = new SortedList<int, Tbl_Emotion_Record>();
	Dictionary<string, Tbl_Emotion_Record> m_dicCommand = new Dictionary<string, Tbl_Emotion_Record>();
	MultiDictionary<char, Tbl_Emotion_Record> m_mddicEmotion = new MultiDictionary<char, Tbl_Emotion_Record>();
	char[] m_arChar = null;
	
	public Tbl_Emotion_Table(string _path)
	{
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			List<char> listChar = new List<char>();
			foreach(XmlNode node in nodes)
			{
				Tbl_Emotion_Record record = new Tbl_Emotion_Record((XmlElement)node);
				m_ResourceTable.Add(record.Index, record);
				
				m_dicCommand.Add(record.EmotionName, record);
				
				foreach(string emotion in record.listCommand)
				{
					m_mddicEmotion.Add(emotion[0], record);
					listChar.Add(emotion[0]);
				}
			}
			
			m_arChar = listChar.ToArray();
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);
		}
	}
	
	public Tbl_Emotion_Record GetValidEmotionRecord(string _str)
	{
		Tbl_Emotion_Record commandRecord = null;
		bool commandContain = false;
		foreach(KeyValuePair<string, Tbl_Emotion_Record> pair in m_dicCommand)
		{
			if(_str.IndexOf(pair.Key) > -1)
			{
				commandRecord = pair.Value;
				break;
			}
		}
		
		if(commandContain == true)
		{
			return commandRecord;
		}
		else
		{
			int index = _str.IndexOfAny(m_arChar);
			if(index < 0)
			{
				Debug.Log("Tbl_Emotion_Record:: GetValidEmotionRecord: not include emotional word.");
				return null;
			}
			else
			{
				char ch = _str[index];
				List<Tbl_Emotion_Record> listRecord = m_mddicEmotion[ch];
				if(listRecord.Count != 0)
				{
					foreach(Tbl_Emotion_Record record in listRecord)
					{
						foreach(string emotion in record.listCommand)
						{
							int contain = _str.IndexOf(emotion);
							if(contain > -1)
								return record;
						}
					}
				}
				else
				{
					Debug.Log("Tbl_Emotion_Record:: GetValidEmotionRecord: invalid word finding. first character is '" + ch + "'. check table or code.");
					return null;
				}
			}
			
			Debug.Log("Tbl_Emotion_Record:: GetValidEmotionRecord: invalid word finding [" + _str + "].");
			return null;
		}		
	}
	
	public List<string> GetCommandList()
	{
		List<string> listString = new List<string>();
		foreach(KeyValuePair<string, Tbl_Emotion_Record> pair in m_dicCommand)
		{
			listString.Add(pair.Key);
		}
		
		return listString;
	}
}
