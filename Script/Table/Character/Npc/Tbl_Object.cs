using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;



public class Tbl_Object_Record : AsTableRecord
{
    int m_Id; public int Id { get { return m_Id; } }
	string m_Name;public string NpcName{get{return m_Name;}}
    eOBJECT_PROP m_pPropType; public eOBJECT_PROP PropType { get { return m_pPropType; } }
    string m_StartActionAni; public string StartActionAni { get { return m_StartActionAni; } }
    string m_DoActionAni; public string DoActionAni { get { return m_DoActionAni; } }
    string m_EndActionAni; public string EndActionAni { get { return m_EndActionAni; } }

    public Tbl_Object_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_Id = int.Parse(node["ID"].InnerText);
			m_Name = node["Name"].InnerText;            

            SetValue<eOBJECT_PROP>(ref m_pPropType, node, "Kind");
            m_StartActionAni = node["StartActionAni"].InnerText;
            m_DoActionAni = node["DoActionAni"].InnerText;
            m_EndActionAni = node["EndActionAni"].InnerText;
		}
		catch(System.Exception e)
		{
            Debug.LogError(e);
		}
	}
}


public class Tbl_Object_Table : AsTableBase
{
    SortedList<int, Tbl_Object_Record> m_ResourceTable = new SortedList<int, Tbl_Object_Record>();

    public Tbl_Object_Table(string _path)
    {
        m_TableType = eTableType.OBJECT;
        LoadTable(_path);
    }

    public override void LoadTable(string _path)
    {
        try
        {
            XmlElement root = GetXmlRootElement(_path);
            XmlNodeList nodes = root.ChildNodes;

            foreach (XmlNode node in nodes)
            {
                Tbl_Object_Record record = new Tbl_Object_Record((XmlElement)node);
                m_ResourceTable.Add(record.Id, record);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Tbl_Npc_Table] LoadTable:|" + e + "| error while parsing");
        }
    }

    public Tbl_Object_Record GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
        {
            return m_ResourceTable[_id];
        }

        Debug.LogError("[Tbl_Npc_Table]GetRecord: there is no record");
        return null;
    }  
}