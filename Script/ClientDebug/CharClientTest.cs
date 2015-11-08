using UnityEngine;
using System.Collections;

public class CharClientTest
{
//	private AS_GC_CHAR_LOAD_RESULT_2 CharLoadData = new AS_GC_CHAR_LOAD_RESULT_2();
	private sCHARVIEWDATA CharLoadData = new sCHARVIEWDATA();
	private AS_GC_ENTER_WORLD_RESULT EnterWorldData = new AS_GC_ENTER_WORLD_RESULT();
	
	
	public bool Loading()	
	{
		
		// char info			
		CharLoadData.nCharUniqKey = 35;
		string str = "PlayerChar";
		for( int i=0; i<str.Length; ++i )
			CharLoadData.szCharName[i] = (byte)str[i];	
	
		CharLoadData.eRace = eRACE.DEMIGOD;
		CharLoadData.eClass = eCLASS.DIVINEKNIGHT;
		CharLoadData.nLevel = 1;
//		CharLoadData.nTotExp = 1;		
//		CharLoadData.nHpCur = 1;
//		CharLoadData.nHpMax = 1;
//		CharLoadData.nMpCur = 1;
//		CharLoadData.nMpMax = 1;		
//		CharLoadData.fMoveSpeed = 300;
//		CharLoadData.fAttDistance = 100;
		
		// position 
		EnterWorldData.sPosition.x = 20.0f;
		EnterWorldData.sPosition.y = 1.0f;
		EnterWorldData.sPosition.z = 30.0f;
		
		
//		AsUserInfo.Instance.SetCurrentUserCharacterInfo(CharLoadData);
//		AsUserInfo.Instance.SetCurrentUserEnterWorldinfo( EnterWorldData );
		
		
		// Create
		AsUserEntity userChar = AsEntityManager.Instance.CreateDummyEntity("PlayerChar") ;
//		CharacterLoadData entityCreationData = new CharacterLoadData(AsUserInfo.Instance.GamerUserSessionIdx, AsUserInfo.Instance.GetCharacter(0));
//		userChar.SetCreationData( entityCreationData );
		CharacterLoadData entityCreationData = new CharacterLoadData(AsUserInfo.Instance.GamerUserSessionIdx, CharLoadData);
		entityCreationData.moveSpeed_ = 5;
		userChar.SetCreationData( entityCreationData );
		userChar.HandleMessage(new Msg_ModelInitialize(eCreationType.CHAR_LOAD_RESULT));
		userChar.SetEnterWorldData( EnterWorldData );
		
		return true ;
		
	}
	
	
}
