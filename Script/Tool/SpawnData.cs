using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.IO;
using System;
using System.Xml;

[ExecuteInEditMode]
public class SpawnData : MonoBehaviour 
{
	public enum ePATH_USE_TYPE
	{
		RAMDOM = 0,
		SEQUENCE
	};
	
	public class CPathData 
	{				
		public Vector3 position;
		public float time = 0.0f;
	}
	
	public static int max_Position_num = 10;
	public static int max_PathGroup_num = 10;
	public static int max_Path_num = 30;
	
	public int groupID;	
	public List<Vector3> positionList = new List<Vector3>();	
	public int linkGroupIndex = 0;
	public int linkIndex = 0;	
	public ePATH_USE_TYPE ePathUseType;	
	public List<List<CPathData>> pathGroupDataList = new List<List<CPathData>>();
	
	
	public float chaseDistance = 0f;
	public float viewDistance = 0f;
	public bool isShowChaseDistance = false;
	public bool isUseViewDistance = false;
	
	
	/*public void SetPositionNum( int iNum )
	{
		if( positionList.Count == iNum )
			return;
		
		if( positionList.Count < iNum )
		{  
			for( int i=positionList.Count; i<iNum; ++i )
			{
				positionList.Add( Vector3.zero );
			}
		}
		else if( 0 >= iNum )
		{
			positionList.Clear();
		}
		else
		{
			positionList.RemoveRange( iNum, positionList.Count - iNum );
		}			
	}*/
	
	public bool ReadXml( XmlElement _element )
	{
		groupID = int.Parse(_element["GroupID"].InnerText);
		
		
		gameObject.name = _element["name"].InnerText;		
		
		
		positionList.Clear();
		
		int iPositionCount = int.Parse(_element["PositionCount"].InnerText);
		
		for( int i=0; i<iPositionCount; ++i )
		{
			Vector3 vec3Position = Vector3.zero;
			string strPosEle = "Position_" + i;
			
			if( 0 == i )
			{
				vec3Position.x = float.Parse( _element[strPosEle].GetAttribute("x") );
	            vec3Position.y = float.Parse( _element[strPosEle].GetAttribute("y") );
	            vec3Position.z = float.Parse( _element[strPosEle].GetAttribute("z") );
				transform.position = vec3Position;
			}
			else
			{				
	            vec3Position.x = float.Parse( _element[strPosEle].GetAttribute("x") );
	            vec3Position.y = float.Parse( _element[strPosEle].GetAttribute("y") );
	            vec3Position.z = float.Parse( _element[strPosEle].GetAttribute("z") );
				positionList.Add( vec3Position );
			}
		}
		
		SetRotate( float.Parse( _element["Rotate"].InnerText ) );		
		 
				
		if( null == _element["LinkGroupIndex"] )
		{
			linkGroupIndex = 0;
			linkIndex = 0;
		}
		else
		{
			linkGroupIndex = int.Parse( _element["LinkGroupIndex"].InnerText );
			linkIndex = int.Parse( _element["LinkIndex"].InnerText );
		}
				
		
		ePathUseType = (ePATH_USE_TYPE)Enum.Parse(typeof(ePATH_USE_TYPE), _element["PathUseType"].InnerText, true);
		
		
		pathGroupDataList.Clear();
		int iPathGroupCount = int.Parse(_element["PathGroupCount"].InnerText);
		for(  int i=0; i<iPathGroupCount; ++i )
		{
			List<CPathData> _pathData = new List<CPathData>();
			pathGroupDataList.Add( _pathData );
			
			string strPathCount = "PathCount_" + (i+1);
			int iPathCount = int.Parse(_element[strPathCount].InnerText);
			for( int k=0; k<iPathCount; ++k )
			{
				string strPathPosEle = "PathPos_" + (k+1);
				Vector3 vec3Position = Vector3.zero;
				vec3Position.x = float.Parse( _element[strPathPosEle].GetAttribute("x") );
	            vec3Position.y = float.Parse( _element[strPathPosEle].GetAttribute("y") );
	            vec3Position.z = float.Parse( _element[strPathPosEle].GetAttribute("z") );				
				float fTime = float.Parse( _element[strPathPosEle].GetAttribute("time") );
				
				
				CPathData _data = new CPathData();	
				_data.position = vec3Position;
				_data.time = fTime;
				_pathData.Add(_data);
			}			
		}
		
		return true;
	}
	
	
	public bool WriteXml( XmlWriter writer )
	{
		if( null == writer )
			return false;
		
		writer.WriteStartElement ("SpawnData");
		
		// Map ID
		writer.WriteStartElement ("GroupID");	
		writer.WriteValue ( groupID.ToString() );
		writer.WriteEndElement ();
		
		// name ID
		writer.WriteStartElement ("name");	
		writer.WriteValue ( gameObject.name );
		writer.WriteEndElement ();
		
		//position
		writer.WriteStartElement ("PositionCount");	
		writer.WriteValue ( (positionList.Count+1).ToString() );
		writer.WriteEndElement ();
		
		
		writer.WriteStartElement ("Position_0");
		writer.WriteStartAttribute("x");
		writer.WriteValue ( transform.position.x );
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("y");
		writer.WriteValue ( transform.position.y );
        writer.WriteEndAttribute();

        writer.WriteStartAttribute("z");
		writer.WriteValue ( transform.position.z );
        writer.WriteEndAttribute();		
		writer.WriteEndElement ();			
		
		for( int k=0; k<positionList.Count; ++k )
		{
			Vector3 vec3PathData = (Vector3)positionList[k];					
			writer.WriteStartElement ("Position_" + (k+1));

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
		
		
		// Rotate
		writer.WriteStartElement ("Rotate");	
		writer.WriteValue ( getRotate.ToString() );
		writer.WriteEndElement ();
		
		
		writer.WriteStartElement ("LinkGroupIndex");	
		writer.WriteValue ( linkGroupIndex.ToString() );
		writer.WriteEndElement ();
		
		writer.WriteStartElement ("LinkIndex");	
		writer.WriteValue ( linkIndex.ToString() );
		writer.WriteEndElement ();
		
		
		// path use type
		writer.WriteStartElement ("PathUseType");	
		writer.WriteValue ( ePathUseType.ToString() );
		writer.WriteEndElement ();
		
		
		int iPathGroupDataCount = pathGroupDataList.Count;
		
		if( 0 < iPathGroupDataCount )
		{
			List<CPathData> temp = pathGroupDataList[0];
			if( 0 >= temp.Count )
			{
				iPathGroupDataCount = 0;
			}
		}
		
		// path data 		
		writer.WriteStartElement ("PathGroupCount");	
		writer.WriteValue ( iPathGroupDataCount.ToString() );
		writer.WriteEndElement ();
		
		for( int i=0; i<iPathGroupDataCount; ++i )
		{
			List<CPathData> temp = pathGroupDataList[i];
			writer.WriteStartElement ( "PathCount_" + (i+1) );	
			writer.WriteValue ( temp.Count.ToString() );
			writer.WriteEndElement ();
			
			for( int k=0; k<temp.Count; ++k )
			{				
				Vector3 vec3PathData = (Vector3)temp[k].position;				
				writer.WriteStartElement ("PathPos_" + (k+1));
	
	            writer.WriteStartAttribute("x");
				writer.WriteValue ( vec3PathData.x.ToString() );
	            writer.WriteEndAttribute();
	
	            writer.WriteStartAttribute("y");
				writer.WriteValue ( vec3PathData.y.ToString() );
	            writer.WriteEndAttribute();
	
	            writer.WriteStartAttribute("z");
				writer.WriteValue ( vec3PathData.z.ToString() );
	            writer.WriteEndAttribute();
				
				writer.WriteStartAttribute("time");
				writer.WriteValue ( temp[k].time );
	            writer.WriteEndAttribute();
				
				writer.WriteEndElement ();
			}			
		}
				
		
		writer.WriteEndElement ();
		
		return true;
	}
	 
	public bool Create()
	{
		pathGroupDataList.Add( new List<SpawnData.CPathData>() );
		
		return true;
	}
	
	public void SetRotate( float fRot )
	{	
		Quaternion rotmat = Quaternion.identity;
		rotmat.eulerAngles = new Vector3( 0.0f, fRot, 0.0f );			
		transform.localRotation = rotmat;		
	}
	
	public float getRotate
	{
		get
		{
			return transform.localRotation.eulerAngles.y;
		}
		
	}
	
	void Awake()
    {
		
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	void OnDrawGizmos() 
	{       
		if( true == isShowChaseDistance )
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere( transform.position, chaseDistance );	
		}
		
		if( true == isUseViewDistance )
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere( transform.position, viewDistance );
		}
	}
	
	
}
