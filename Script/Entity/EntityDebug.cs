using UnityEngine;
using System.Collections;

public class EntityDebug : MonoBehaviour 
{
	private bool m_bNeedLoadEntity = false;
	public sCHARVIEWDATA CharLoadData = new sCHARVIEWDATA();
	public AS_GC_ENTER_WORLD_RESULT EnterWorldData = new AS_GC_ENTER_WORLD_RESULT();
	
	void Awake()
	{
		if( null == AsEntityManager.Instance )
		{
			gameObject.AddComponent<AsEntityManager>();
			gameObject.AddComponent<AsTableManager>();		
	
			m_bNeedLoadEntity = true;		
			
			// char info			
			CharLoadData.nCharUniqKey = 35;
			string str = "PlayerChar";
			for( int i=0; i<str.Length; ++i )
				CharLoadData.szCharName[i] = (byte)str[i];	
			CharLoadData.eRace = eRACE.DEMIGOD;
			CharLoadData.eClass = eCLASS.DIVINEKNIGHT;
			CharLoadData.nLevel = 1;
//			CharLoadData.nTotExp = 1;		
//			CharLoadData.nHpCur = 1;
//			CharLoadData.nHpMax = 1;
//			CharLoadData.nMpCur = 1;
//			CharLoadData.nMpMax = 1;		
//			CharLoadData.fMoveSpeed = 300;
//			CharLoadData.fAttDistance = 100;
			
			// position 
			EnterWorldData.sPosition.x = -19.0f;
			EnterWorldData.sPosition.y = 11.0f;
			EnterWorldData.sPosition.z = -19.0f;
		}
	}
	// Use this for initialization
	void Start () 
	{		
		if( true == m_bNeedLoadEntity )
		{
			AsUserEntity userChar = AsEntityManager.Instance.CreateDummyEntity("PlayerChar");
//			CharacterLoadData entityCreationData = //
//				new CharacterLoadData(AsUserInfo.Instance.GamerUserSessionIdx, AsUserInfo.Instance.GetCurrentUserCharacterInfo());
			CharacterLoadData entityCreationData = new CharacterLoadData(AsUserInfo.Instance.GamerUserSessionIdx, CharLoadData);
			entityCreationData.moveSpeed_ = 5;
			userChar.SetCreationData( entityCreationData );
			userChar.SetEnterWorldData( EnterWorldData );
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}