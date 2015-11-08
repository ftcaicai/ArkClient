using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public enum eCOLLECTION_TECHNIC
{
	MINERAL,
	PLANTS,
	SPIRIT,
    QUEST,
}

public class Tbl_Collection_Record : AsTableRecord
{
   	private int npcIndex; public int getNpcIndex{ get{ return npcIndex; } }
	public eCOLLECTION_TECHNIC technic;
	public int level;
	public float time;
	public int expertism;
	public int exp;
	

    public Tbl_Collection_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			SetValue(ref npcIndex, node, "NPC_Index");
			SetValue<eCOLLECTION_TECHNIC>(ref technic, node, "Collection_Technic");
			
			SetValue(ref level, node, "Collection_Level");
			int iTime = 0;
			SetValue(ref iTime, node, "Collection_Time");
			time  = (float)iTime * 0.001f;
			SetValue(ref expertism, node, "Collection_Expertism");
			SetValue(ref exp, node, "Collection_EXP");	
		}
		catch(System.Exception e)
		{
            Debug.LogError(e);
		}
	}
}


public class Tbl_Collection_Table : AsTableBase
{
    SortedList<int, Tbl_Collection_Record> m_ResourceTable = new SortedList<int, Tbl_Collection_Record>();

    public Tbl_Collection_Table(string _path)
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
                Tbl_Collection_Record record = new Tbl_Collection_Record((XmlElement)node);
				if( true == m_ResourceTable.ContainsKey( record.getNpcIndex ) )
				{
					AsUtil.ShutDown("Tbl_Collection_Table::LoadTable()[ same index ] ");
					continue;
				}
                m_ResourceTable.Add(record.getNpcIndex, record);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Tbl_Collection_Table] LoadTable:|" + e + "| error while parsing");
        }
    }

    public Tbl_Collection_Record GetRecord(int _id)
    {
        if (m_ResourceTable.ContainsKey(_id) == true)
        {
            return m_ResourceTable[_id];
        }

        Debug.LogError("[Tbl_Collection_Table]GetRecord: there is no record ] id : " + _id + "[listcount: " + m_ResourceTable.Count );
        return null;
    }  
}