using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class Tbl_EntityTemplate_Record// : AsTableRecord
{
	public class AsProperty
	{
		public string name_;
		public string varType_;
		public string default_;
	}

	string m_EntityType;public string EntityType{get{return m_EntityType;}}
	List<string> m_listComponent = new List<string>();public List<string> ListComponent{get{return m_listComponent;}}
	List<AsProperty> m_listProperty = new List<AsProperty>();public List<AsProperty> ListProperty{get{return m_listProperty;}}
	
	public Tbl_EntityTemplate_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode nodes = (XmlElement)_element;
			
			m_EntityType = nodes.Attributes["Type"].Value;
			
			foreach(XmlNode node in nodes)
			{
				if(node.Name == "Component")
				{
					m_listComponent.Add(node.Attributes["Name"].Value);
				}
				if(node.Name == "AsProperty")
				{
					AsProperty prop = new AsProperty();
					prop.name_ = node.Attributes["Name"].Value;	
					prop.varType_ = node.Attributes["VarType"].Value;
					prop.default_ = node.Attributes["Default"].Value;

					m_listProperty.Add(prop);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_EntityTemplate_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
}

public class Tbl_EntityTemplate_Table : AsTableBase {

	SortedList<string, Tbl_EntityTemplate_Record> EntityTemplateTable = 
		new SortedList<string, Tbl_EntityTemplate_Record>();
	
	Dictionary<string, AsEntityTemplate> m_dicEntityTemplate;
	
	public Tbl_EntityTemplate_Table(string _path)
	{
		m_TableType = eTableType.ENTITY_TEMPLATE;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try{
			XmlElement root = GetXmlRootElement(_path);
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				Tbl_EntityTemplate_Record record = new Tbl_EntityTemplate_Record((XmlElement)node);
				EntityTemplateTable.Add(record.EntityType, record);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_EntityTemplate_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_EntityTemplate_Record GetRecord(string _type)
	{
		if(EntityTemplateTable.ContainsKey(_type) == true)
		{
			return EntityTemplateTable[_type];
		}
		
		Debug.LogError("[Tbl_EntityTemplate_Table]GetRecord: there is no record");
		return null;
	}
	
	public Dictionary<string, AsEntityTemplate> GetEntityTemplate()
	{
		if(m_dicEntityTemplate == null)
		{
			m_dicEntityTemplate = new Dictionary<string, AsEntityTemplate>();
			foreach(KeyValuePair<string, Tbl_EntityTemplate_Record> pair in EntityTemplateTable)
			{
				AsEntityTemplate entityTemplate = new AsEntityTemplate(pair.Value);
				m_dicEntityTemplate.Add(pair.Key, entityTemplate);
			}
		}
		
		return m_dicEntityTemplate;
	}
}
