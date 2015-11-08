using UnityEngine;
using System.Collections;


using System.IO;
using System;
using System.Xml;


public class AsNpcDataXml
{
	public int _iMapID;
	public int _iNpcID;
	public Vector3 _vec3Pos;
	public float _fRot;
	public int _iSpawnType;
	public int _iGroupIndex;
	public int _iLinkIndex;
	public ArrayList _MovePathList = null;
	
	public int GetListCount()
	{
		if( null == _MovePathList )
			return 0;
		return _MovePathList.Count;
	}
};


public class AsNpcXmlWrite 
{		
	
	public bool Open( ArrayList npcDataList )
	{		
		
		//----------------------------------------------
		XmlWriterSettings wrapperSettings = new XmlWriterSettings();
		wrapperSettings.Indent = true;			
	
		string wrappername = Application.dataPath + "/Resources/Table/NpcDataExport.xml";
		
		
		using ( XmlWriter writer = XmlWriter.Create (wrappername, wrapperSettings) ) 
		{
			writer.WriteStartDocument();
            writer.WriteStartElement ("NpcDataExport");
			
			for( int i=0; i<npcDataList.Count; ++i )
			{				
	            writer.WriteStartElement ("NpcData");
				
				AsNpcDataXml data = (AsNpcDataXml)npcDataList[i];
				
				// Map ID
				writer.WriteStartElement ("MapID");	
				writer.WriteValue ( data._iMapID.ToString() );
				writer.WriteEndElement ();
				
				
				// Npc Type
				writer.WriteStartElement ("NpcID");	
				writer.WriteValue ( data._iNpcID.ToString() );
				writer.WriteEndElement ();
				
				// Position
				writer.WriteStartElement ("Position");

                writer.WriteStartAttribute("x");
				writer.WriteValue ( data._vec3Pos.x.ToString() );
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("y");
				writer.WriteValue ( data._vec3Pos.y.ToString() );
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("z");
				writer.WriteValue ( data._vec3Pos.z.ToString() );
                writer.WriteEndAttribute();
				
				writer.WriteEndElement ();
				
				// Rotate
				writer.WriteStartElement ("Rotate");	
				writer.WriteValue ( data._fRot.ToString() );
				writer.WriteEndElement ();
				
				// Spawn ID
				writer.WriteStartElement ("SpawnType");	
				writer.WriteValue ( data._iSpawnType.ToString() );
				writer.WriteEndElement ();
				
				// Group Index
				writer.WriteStartElement ("GroupIndex");	
				writer.WriteValue ( data._iGroupIndex.ToString() );
				writer.WriteEndElement ();
				
				// Link Index
				writer.WriteStartElement ("LinkIndex");	
				writer.WriteValue ( data._iLinkIndex.ToString() );
				writer.WriteEndElement ();
				
				// Move Path Count
				writer.WriteStartElement ("MovePathCount");	
				writer.WriteValue ( data.GetListCount().ToString() );
				writer.WriteEndElement ();
				
				// Move Path 
				for( int k=0; k<data.GetListCount(); ++k )
				{
					Vector3 vec3PathData = (Vector3)data._MovePathList[k];					
					writer.WriteStartElement ("MovePath_" + k);

                    writer.WriteStartAttribute("x");
					writer.WriteValue ( vec3PathData.x.ToString() );
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("y");
					writer.WriteValue ( vec3PathData.y.ToString() );
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("z");
					writer.WriteValue ( vec3PathData.z.ToString() );
                    writer.WriteEndAttribute();
					
					writer.WriteEndElement ();					
				}
				
				writer.WriteEndElement ();
			}	
			
			writer.WriteEndElement ();
			       
            writer.Close ();
        }
		
		
		return true;
		
	}
		
		
	
	
	public void Save()
	{
		
	}	
	
}
