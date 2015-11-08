using UnityEngine;
using System.Collections;

public class PartsTest : MonoBehaviour 
{
	[System.Serializable]
	public enum eCHAR_TYPE
	{
		Divinknight = 0,
		MAGICIAN,
		CLERIC,
		HUNTER,
		ASSASSIN,
	}
	
	[System.Serializable]
	public enum eGENDER_TYPE
	{
		MALE = 0,
		FEMALE
	}
	
	
	
	public eCHAR_TYPE charType; 
	public eGENDER_TYPE genderType;
	public int createCount = 1;
	//private PartsRoot m_partsRoot;
	private bool m_bNeed= true;
	// Use this for initialization
	void Start () 
	{
		gameObject.AddComponent<AsTableManager>();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( false == m_bNeed)
			return; 
		
		string path = "";
		eCLASS eTempClass = eCLASS.DIVINEKNIGHT;		
		Tbl_Class_Record record = null;
		
		switch( charType )
		{
		case eCHAR_TYPE.Divinknight:
			record = AsTableManager.Instance.GetTbl_Class_Record(eRACE.DEMIGOD, eCLASS.DIVINEKNIGHT);
			if( eGENDER_TYPE.MALE == genderType )
			{
				path = record.ModelingPath_Male;			
			}			
			else
			{
				path = record.ModelingPath_Female;		
			}
			
			eTempClass = eCLASS.DIVINEKNIGHT;
			break;
			
		case eCHAR_TYPE.CLERIC:
			record = AsTableManager.Instance.GetTbl_Class_Record(eRACE.LUMICLE, eCLASS.CLERIC);
			if( eGENDER_TYPE.MALE == genderType )
			{
				path = record.ModelingPath_Male;			
			}			
			else
			{
				path = record.ModelingPath_Female;		
			}	
			eTempClass = eCLASS.CLERIC;
			break;
		}
				
		for( int i=0; i<createCount; ++i )
		{
			GameObject goCreate = new GameObject();
			PartsRoot _partsRoot = goCreate.AddComponent<PartsRoot>();
			eGENDER eGender = eGENDER.eGENDER_NOTHING;
			if( eGENDER_TYPE.MALE == genderType )
			{
				eGender = eGENDER.eGENDER_MALE;
			}
			else
			{
				eGender = eGENDER.eGENDER_FEMALE;
			}
			
			
			if( false == _partsRoot.Create( path, eGender, eTempClass, 0 ) )
			{
				Debug.LogError("PartsTest::Start() [ false == m_partsRoot.Create( path ) ]");
				return;
			}	
			
			_partsRoot.GenerateParts();
			
		}
		
		m_bNeed = false;
	}
}
