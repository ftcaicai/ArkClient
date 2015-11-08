#define _USE_AREALAYER

using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;
using System.Reflection;
using System.Reflection.Emit;


using System.Xml;
using System.Text;



public class AsActionSaveData :  MonoBehaviour {
	
	public AsAction_Record m_AsAction_Record;
	
	SortedList<int, AsAction_Record> m_ResourceTable = 
		new SortedList<int, AsAction_Record>();


	int m_wMaxKey;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public SortedList<int, AsAction_Record> GetList()
	{
		return m_ResourceTable;
	}	
	
	
	public AsAction_Record AddRecord()
	{
		AsAction_Record record = new AsAction_Record();
		if(m_ResourceTable.Count != 0 )
			record.index  = m_ResourceTable.Values[m_ResourceTable.Count-1].index+1;
		else
			record.index  =	1;
		m_ResourceTable.Add(record.index, record);
		m_AsAction_Record = record; 
		record.Seq = m_ResourceTable.IndexOfKey(record.index ); 
		return m_AsAction_Record;
		
	}
	
	public void RefreshRecord()
	{	
		m_ResourceTable[m_AsAction_Record.index] = m_AsAction_Record;
	}
	
	public void RemoveRecord(int id)
	{	
		m_ResourceTable.Remove(id);
	}
	
	public AsAction_Record GetRecord(int id)
	{
		if(m_ResourceTable.ContainsKey(id) == true)
			return m_ResourceTable[id];		
		else 
			return null;
	}
		// Save xml file
	public void SaveXmlFile(string filename)
	{	

		AsActionListXmlWrite write = new AsActionListXmlWrite();
		if( false == write.Save( filename , this ) )	
		{
				Debug.LogWarning( "not found DataExport GameObject" );
		}
		
		
	}
	

	public static Enum CreateDynamicEnum(List<string> _list)
    {
        // Get the current application domain for the current thread.
        AppDomain currentDomain = AppDomain.CurrentDomain;

        // Create a dynamic assembly in the current application domain, 
        // and allow it to be executed and saved to disk.
        AssemblyName aName = new AssemblyName("TempAssembly");
        AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(
            aName, AssemblyBuilderAccess.RunAndSave);

        // Define a dynamic module in "TempAssembly" assembly. For a single-
        // module assembly, the module has the same name as the assembly.
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

        // Define a public enumeration with the name "Elevation" and an 
        // underlying type of Integer.
        EnumBuilder eb = mb.DefineEnum("Elevation", TypeAttributes.Public, typeof(int));

        // Define two members, "High" and "Low".
        //eb.DefineLiteral("Low", 0);
        //eb.DefineLiteral("High", 1);

        int i = 0;
        foreach (string item in _list)
        {
            eb.DefineLiteral(item, i);
            i++;
        }

        // Create the type and save the assembly.
        return (Enum)Activator.CreateInstance(eb.CreateType());
        //ab.Save(aName.Name + ".dll");


    }

	public void LoadTable(string _path)
	{
		try{
			XmlElement root ;
			
			XmlDocument xmlDoc = new XmlDocument();
			try{
			TextAsset xmlText = Resources.Load(_path) as TextAsset;
			byte[] encodedString = Encoding.UTF8.GetBytes(xmlText.text);
			MemoryStream memoryStream = new MemoryStream(encodedString);
			
			StreamReader streamReader = new StreamReader(memoryStream);
			
			StringReader stringReader = new StringReader(streamReader.ReadToEnd());
			string str = stringReader.ReadToEnd();
			
			xmlDoc.LoadXml(str);
			
			root =  xmlDoc.DocumentElement;
			}
			catch{
				Debug.LogError(_path + ": LoadTable error while load xml");
				return ;
			}
			
			
			XmlNodeList nodes = root.ChildNodes;
			
			foreach(XmlNode node in nodes)
			{
				AsAction_Record record = new AsAction_Record((XmlElement)node);
				m_ResourceTable.Add(record.index, record);
				m_AsAction_Record = record; 
		
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("[Tbl_Action_Table] LoadTable:|" + e + "| error while parsing");
		}
	}	
	

	public void OnDrawGizmos()
	{
		if(ActionEditorBrokerMgr.Instance.ActionState != eACTION_STATE.HitAction) return;
		if(m_AsAction_Record == null) return;
		if (m_AsAction_Record.hitAnimationInfoData.hitInfoData == null) return;
		
		HitInfo tblHitInfo = m_AsAction_Record.hitAnimationInfoData.hitInfoData;
		
		if( HitInfo.eHIT_TYPE.None == tblHitInfo.hitType) return;

		if(HitInfo.eHIT_TYPE.ProjectileTarget == tblHitInfo.hitType)
		{
			if(ActionEditorBrokerMgr.Instance.ProjectileDrawGizmosTime  == 0) return;
			if(ActionEditorBrokerMgr.Instance.ProjectileDrawGizmosTime+ 1f <= Time.time) return;			
		}
		else
		{
			if(tblHitInfo.timing * 0.001f > ActionEditorBrokerMgr.Instance.AnimationTime) return;
			if(tblHitInfo.timing * 0.001f + 1f <= ActionEditorBrokerMgr.Instance.AnimationTime) return;		
		}

		
		#if _USE_AREALAYER
		OnAreaDrawGizmos(tblHitInfo);		
		#else
		if( HitInfo.eAREA_SHAPE.Point != tblHitInfo.areaShape)
		{		
			float maxDist = tblHitInfo.maxDistance * 0.01f;
			float minDist = tblHitInfo.minDistance * 0.01f;
			float angle = tblHitInfo.angle;
			float centerAngle = tblHitInfo.centerDirectionAngle;
			Vector3 pos = m_CurCharacter.transform.position;
			if(HitInfo.eHIT_TYPE.ProjectileTarget == tblHitInfo.hitType)
			{
				Vector3 startPosition;
				
				if(ChangeShooting_Position)					
					startPosition = m_CurCharacter.transform.position;
				else			
				{
					GameObject goStartPosition = GameObject.Find( "StartPosition"  );	
					startPosition = goStartPosition.transform.position;
				}
				
				
				pos = startPosition;
			}
			
			Vector3 offset = new Vector3(tblHitInfo.offsetX * 0.01f, 0, tblHitInfo.offsetY * 0.01f);				
			offset = Quaternion.AngleAxis( m_CurCharacter.transform.rotation.eulerAngles.y, Vector3.up) * offset;
			pos += offset;
			
			Vector3 dir = m_CurCharacter.transform.forward * maxDist;
			dir = Quaternion.AngleAxis( centerAngle, Vector3.up) * dir;
			Vector3 rot1 = Quaternion.AngleAxis( angle * 0.5f, Vector3.up) * dir;
			Vector3 rot2 = Quaternion.AngleAxis( -angle * 0.5f, Vector3.up) * dir;
			
			
			
			
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere( pos, minDist);
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere( pos, maxDist);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine( pos, pos + ( dir * 2.0f));
			
			if( 0.0f < angle)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawLine( pos, pos + rot1);
				Gizmos.DrawLine( pos, pos+ rot2);
			}
		}		
		#endif
	}

	void OnAreaDrawGizmos(HitInfo tblHitInfo )
	{
		//AreaInfoo		
		for( int index = 0; index < tblHitInfo.areaLayer.Count; ++index )
		{
			AreaInfo data = tblHitInfo.areaLayer[index];	
			if( HitInfo.eAREA_SHAPE.Point != data.areaShape)
			{		
				float maxDist = data.maxDistance * 0.01f;
				float minDist = data.minDistance * 0.01f;
				float angle = data.angle;
				float centerAngle = data.centerDirectionAngle;
				Vector3 pos = gameObject.transform.position;

				switch(tblHitInfo.hitType)
				{
				case HitInfo.eHIT_TYPE.ProjectileTarget:
					Vector3 startPosition;

					if(ActionEditorBrokerMgr.Instance.ChangeShooting_Position)					
						startPosition = gameObject.transform.position;
					else			
					{
						GameObject goStartPosition = GameObject.Find( "StartPosition"  );	
						startPosition = goStartPosition.transform.position;
					}			
					pos = startPosition;
					break;
				case HitInfo.eHIT_TYPE.PositionTarget:
					if( ActionEditorBrokerMgr.Instance.EnemyList != null )
					{
						GameObject goStartPosition = ActionEditorBrokerMgr.Instance.EnemyList.GetEnemy(index);
						pos = goStartPosition.transform.position;;
					}
					break;
				}

				Vector3 offset = new Vector3(data.offsetX * 0.01f, 0, data.offsetY * 0.01f);				
				offset = Quaternion.AngleAxis( gameObject.transform.rotation.eulerAngles.y, Vector3.up) * offset;
				pos += offset;
				
				Vector3 dir = gameObject.transform.forward * maxDist;
				dir = Quaternion.AngleAxis( centerAngle, Vector3.up) * dir;
				Vector3 rot1 = Quaternion.AngleAxis( angle * 0.5f, Vector3.up) * dir;
				Vector3 rot2 = Quaternion.AngleAxis( -angle * 0.5f, Vector3.up) * dir;

				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere( pos, minDist);
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere( pos, maxDist);
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine( pos, pos + ( dir * 2.0f));
				
				if( 0.0f < angle)
				{
					Gizmos.color = Color.black;
					Gizmos.DrawLine( pos, pos + rot1);
					Gizmos.DrawLine( pos, pos+ rot2);
				}
			}		
		}
	}
}
