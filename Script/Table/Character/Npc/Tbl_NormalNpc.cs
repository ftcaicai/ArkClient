using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public enum eLivingType {Living, UnLiving}

public class Tbl_NormalNpc_Record : AsTableRecord
{
	
	
	int m_Id;public int Id{get{return m_Id;}}
	int m_FaceId;public int  FaceId{get{return m_FaceId;}}
	
	eLivingType m_LivingType;public eLivingType LivingType{get{return m_LivingType;}}
	float m_MoveSpeed;public float MoveSpeed{get{return m_MoveSpeed;}}
	

	public int[]  m_Speech; 
	int    m_SpeechCount = 0; public int SpeechCount{get{return m_SpeechCount;}} 
	//Npc Voice
	public string[]  m_Voice; 
	int    m_VoiceCount = 0; public int VoiceCount{get{return m_VoiceCount;}} 
	
	public int[]  m_Talk; 
	public eNpcMenuImage[]  m_DisplayType; 
	int    m_TalkCount 	 = 0; public int TalkCount{get{return m_TalkCount;}} 
	
	int    m_QuestGroupId; public int QuestGroupId{get{return m_QuestGroupId;}}
	
	public eNPCMenu[]    m_NpcMenuBtn; 
	
	int    m_MenuBtnCount = 0; public int MenuBtnCount{get{return m_MenuBtnCount;}} 
//	int m_NpcAct;public int NpcAct{get{return m_NpcAct;}}
	
	int m_NpcLV;
	public int NpcLV { get{ return m_NpcLV;}}
	int m_NpcHP;
	public int NpcHP { get{ return m_NpcHP;}}
	
	bool m_Touchable; public bool Touchable{get{return m_Touchable;}}
	bool m_EnablePStore; public bool EnablePStore{get{return m_EnablePStore;}}
	
	public int Speech(int index)
	{
		if(index < 0 || index > m_SpeechCount)
		{
			Debug.LogError("Tbl_NormalNpc_Record::Speech() index err!!! " );
			index = 0;
		}
		return m_Speech[index];
	}
	
	public string Voice(int index)
	{
		if(index < 0 || index > m_VoiceCount)
		{
			Debug.LogError("Tbl_NormalNpc_Record::Voice() index err!!! " );
			index = 0;
		}
		return m_Voice[index];
	}
	
	public int Talk(int index)
	{
		if(index < 0 || index > m_TalkCount)
		{
			Debug.LogError("Tbl_NormalNpc_Record::Talk() index err!!! " );
			index = 0;
		}
		return m_Talk[index];
	}
	
	public eNpcMenuImage DisplayType(int index)
	{
		if(index < 0 || index > m_TalkCount)
		{
			Debug.LogError("Tbl_NormalNpc_Record::DisplayType() index err!!! " );
			index = 0;
		}
		return m_DisplayType[index];
	}
	
	public	eNPCMenu NpcMenuBtn(int index)
	{
		if(index < 0 )
		{
			Debug.LogError("Tbl_NormalNpc_Record::NpcMenuBtn() index err!!! " );
			index = 0;
		}
		return m_NpcMenuBtn[index];
	}

    public bool IsHaveMenuBtn(eNPCMenu type)
    {
        if( null == m_NpcMenuBtn )
            return false;

        foreach (eNPCMenu data in m_NpcMenuBtn)
        {
            if (data == type)
                return true;
        }

        return false;
    }
	
		
	public Tbl_NormalNpc_Record(XmlElement _element)// : base(_element)
	{
		try{
			XmlNode node = (XmlElement)_element;
			
			m_Id = int.Parse(node["ID"].InnerText);
		//	m_FaceId = int.Parse(node["NPCFaceFileName"].InnerText);
			SetValue(ref m_FaceId, node, "NPCFaceFileName");
			SetValue(ref m_LivingType, node, "Living");
			SetValue(ref m_MoveSpeed, node, "MoveSpeed");
			SetValue(ref m_QuestGroupId, node, "QuestGroup");
			SetValue( ref m_NpcLV, node, "LV");
			SetValue( ref m_NpcHP, node, "HP");
			SetValue( ref m_Touchable, node, "Touchable");
			SetValue( ref m_EnablePStore, node, "EnablePStore");
			
			string strColumn = "";			
			m_Speech = new int[AsGameDefine.MAX_SPEECH];
			for(int i = 0; i < AsGameDefine.MAX_SPEECH; ++i)
			{		
				strColumn = "Speech_"+ (i + 1);
				SetValue(ref m_Speech[i], node, strColumn);
				if(m_Speech[i] != int.MaxValue)
					m_SpeechCount++;
				
				
			}
			m_Voice = new string[AsGameDefine.MAX_VOICE];
			for(int i = 0; i < AsGameDefine.MAX_VOICE; ++i)
			{		
				strColumn = "Voice_"+ (i + 1);
				SetValue(ref m_Voice[i], node, strColumn);
				if(m_Voice[i].Length != 0 &&  m_Voice[i] != "NONE")
					m_VoiceCount++;			
			}
			
			m_Talk = new int[AsGameDefine.MAX_TALK];
			m_DisplayType = new eNpcMenuImage[AsGameDefine.MAX_TALK];
			
			for(int i = 0; i < AsGameDefine.MAX_TALK; ++i)
			{				
				strColumn = "Talk_"+ (i + 1);
				SetValue(ref m_Talk[i], node, strColumn);
				if(m_Talk[i] != int.MaxValue)
				{
					m_TalkCount++;
					 m_DisplayType[i] = eNpcMenuImage.Npc;
				}
				strColumn = "DisplayType_"+ (i + 1);
				SetValue<eNpcMenuImage>(ref m_DisplayType[i], node, strColumn);
			}	
			
			
			m_NpcMenuBtn = new eNPCMenu[AsGameDefine.MAX_NPC_MENU];
			
			
			for(int i = 0; i < AsGameDefine.MAX_NPC_MENU; ++i)
			{				
				strColumn = "Button_"+ (i + 1);
				SetValue<eNPCMenu>(ref m_NpcMenuBtn[i], node, strColumn);
				
				if(m_NpcMenuBtn[i] != eNPCMenu.NONE)
					m_MenuBtnCount++;
			}
		
//			m_NpcAct = int.Parse(node["NPCAct"].InnerText);
		}
		catch(System.Exception e)
		{		
			Debug.LogError("[Tbl_NormalNpc_Record] 'constructor':|" + e + "| error while parsing");
		}
	}
	
	
	public Tbl_NormalNpc_Record(BinaryReader br)// : base(_element)
	{					
		m_Id = br.ReadInt32();
		m_FaceId = br.ReadInt32();
		m_LivingType = (eLivingType)br.ReadInt32();
		m_MoveSpeed = br.ReadSingle();
		m_QuestGroupId = br.ReadInt32();
		m_NpcLV = br.ReadInt32();
		m_NpcHP = br.ReadInt32();
		m_Touchable = br.ReadBoolean();
		m_EnablePStore = br.ReadBoolean();
		
//		string strColumn = "";			
		m_Speech = new int[AsGameDefine.MAX_SPEECH];
		for(int i = 0; i < AsGameDefine.MAX_SPEECH; ++i)
		{				
			m_Speech[i] = br.ReadInt32();			
			if(m_Speech[i] != int.MaxValue)
				m_SpeechCount++;
		}
		
		m_Voice = new string[AsGameDefine.MAX_VOICE];
		for(int i = 0; i < AsGameDefine.MAX_VOICE; ++i)
		{		
			
			m_Voice[i] = br.ReadString();					
			if(m_Voice[i].Length != 0 &&  m_Voice[i] != "NONE")
				m_VoiceCount++;				
		}
		
		m_Talk = new int[AsGameDefine.MAX_TALK];
		m_DisplayType = new eNpcMenuImage[AsGameDefine.MAX_TALK];
		
		for(int i = 0; i < AsGameDefine.MAX_TALK; ++i)
		{				
			m_Talk[i] = br.ReadInt32();			
			if(m_Talk[i] != int.MaxValue)
			{
				m_TalkCount++;
				 m_DisplayType[i] = eNpcMenuImage.Npc;
			}
			
			m_DisplayType[i] = (eNpcMenuImage)br.ReadInt32();			
		}	
		
		
		m_NpcMenuBtn = new eNPCMenu[AsGameDefine.MAX_NPC_MENU];
		
		
		for(int i = 0; i < AsGameDefine.MAX_NPC_MENU; ++i)
		{					
			m_NpcMenuBtn[i] = (eNPCMenu)br.ReadInt32();	
			
			if(m_NpcMenuBtn[i] != eNPCMenu.NONE)
				m_MenuBtnCount++;
		}		
	}
}

public class Tbl_NormalNpc_Table : AsTableBase {
	SortedList<int, Tbl_NormalNpc_Record> m_ResourceTable = 
		new SortedList<int, Tbl_NormalNpc_Record>();
	
	public Tbl_NormalNpc_Table(string _path)
	{
//		m_TableName = "CharacterResource";
		m_TableType = eTableType.NORMAL_NPC;
		
		LoadTable(_path);
	}
	
	public override void LoadTable(string _path)
	{
		try
		{
			if( (null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle) || 
				( null != AsTableManager.Instance && true == AsTableManager.Instance.useReadBinary ) )
			{		
				// Ready Binary
				TextAsset textAsset = ResourceLoad.LoadTextAsset( _path);
				MemoryStream stream = new MemoryStream( textAsset.bytes);
				BinaryReader br = new BinaryReader( stream);
				
				int nCount = br.ReadInt32();
					
		
				for( int i = 0; i < nCount; i++)
				{
					Tbl_NormalNpc_Record record = new Tbl_NormalNpc_Record(br);
					m_ResourceTable.Add(record.Id, record);	
				}
				
				br.Close();
				stream.Close();	
			}
			else
			{
			
				XmlElement root = GetXmlRootElement(_path);
				XmlNodeList nodes = root.ChildNodes;
				
				foreach(XmlNode node in nodes)
				{
					Tbl_NormalNpc_Record record = new Tbl_NormalNpc_Record((XmlElement)node);
					m_ResourceTable.Add(record.Id, record);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_NormalNpc_Table] LoadTable:|" + e + "| error while parsing");
		}
	}
	
	public Tbl_NormalNpc_Record GetRecord(int _id)
	{
		if(m_ResourceTable.ContainsKey(_id) == true)
		{
			return m_ResourceTable[_id];
		}
		
		Debug.LogError("[Tbl_NormalNpc_Table]GetRecord: there is no record : " + _id );
		return null;
	}
	
//	public Tbl_NormalNpc_Record GetRecordByTribeAndClass(int _tribe, int _class)
//	{
//		foreach(KeyValuePair<int, Tbl_NormalNpc_Record> pair in m_ResourceTable)
//		{
//			if(pair.Value.Race == _tribe)
//			{
//				if(pair.Value.Class == _class)
//				{
//					return pair.Value;
//				}
//			}
//		}
//		
//		Debug.LogError("Tbl_NormalNpc_Table::GetRecordByTribeAndClass: no record");
//		return null;
//	}
}
