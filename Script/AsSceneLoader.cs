#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System;
using System.Collections;

public class AsSceneLoader : MonoBehaviour
{
	public enum eLoadState
	{
		NONE,
		LOADING,
		LOAD_COMPLETED
	};
	
	public enum eLoadType : int
	{
		INVALID = -1,
		
		ENTER_WORLD,
		WARP,
		WARPXZ,
		GOTO_TOWN,
		INSTANCE_DUNGEON_ENTER,
		ARENA_ENTER,
		INSTANCE_DUNGEON_EXIT,
		PARTY_WARPXZ,
		MAX_LOADTYPE
	};

	public AsLoadingInfo loadInfo = null;
	private GAME_STATE m_eOldGameState;
	public GAME_STATE OldGameState	{ get { return m_eOldGameState; } }
	public eLoadType curLoadType = eLoadType.INVALID;

	private bool	m_bActiveCoroutineMapCheck = false;
	public bool ActiveCoroutineMapCheck
	{
		get{return m_bActiveCoroutineMapCheck;}
	}

	void Start()
	{
		ActivateLoadInfo( false);
	}
	
	private void ActivateLoadInfo( bool flag)
	{
		if( true == flag)
			AsLoadingIndigator.Instance.ShowIndigator( string.Empty);
		else
			AsLoadingIndigator.Instance.HideIndigator();
			
		loadInfo.gameObject.SetActiveRecursively( flag);
	}

	private int m_nMapID = 0;
	private eLoadType m_eType = eLoadType.INVALID;
	public void Load( int iMapID, eLoadType type)
	{
		Debug.Log ("SceneLoad .Load " + iMapID);
		curLoadType = type;
		m_eOldGameState = AsGameMain.s_gameState;
		AsGameMain.s_gameState = GAME_STATE.STATE_LOADING;

		if(PlayerPrefs.HasKey( AsNpcStore.SaveKindKey))
			PlayerPrefs.DeleteKey( AsNpcStore.SaveKindKey);
		
		if( true == AssetbundleManager.Instance.useAssetbundle)
			AssetbundleManager.Instance.DownloadAssets_Group( _GetPatchGroup( iMapID));

		m_nMapID = iMapID;
		m_eType = type;

		StartCoroutine( "MapLoadCheck");
	}
	
	IEnumerator MapLoadCheck()//int iMapID, eLoadType type)
	{
		int iMapID = m_nMapID;
		eLoadType type = m_eType;
		m_bActiveCoroutineMapCheck = true;

		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			while( false == AssetbundleManager.Instance.bLoaded || false == AssetbundleManager.Instance.bPatchFinishedBtnOk)
			{
				if( true == AssetbundleManager.Instance.bSendWarpCancel)
					break;
			
				yield return null;
			}
			
			AssetbundleManager.Instance.HidePatchScene();
			AssetbundleManager.Instance.SetActiveAssetbundleInfo( false);

			if( true == AssetbundleManager.Instance.bSendWarpCancel)
			{
				AssetbundleManager.Instance.bSendWarpCancel = false;
				AsInputManager.Instance.m_Activate = true;
				//AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
				if( GAME_STATE.STATE_CHARACTER_SELECT == m_eOldGameState)
					AsGameMain.s_gameState = m_eOldGameState;
				else
					AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			}
			else
			{
				yield return StartCoroutine( MapLoad( iMapID, type));
			}
		}
		else
		{
			yield return StartCoroutine( MapLoad( iMapID, type));
		}

		m_bActiveCoroutineMapCheck = false;
	}
	
	IEnumerator MapLoad( int iMapID, eLoadType type)
	{
		AsGameMain.SetBackGroundProc( true);
		
		ActivateLoadInfo( true);
		loadInfo.Init( iMapID);
		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();	// kij
		
		yield return null;
		
		AsNotify.Instance.Hidden();
		AsHUDController.SetActiveRecursively( false);
		ObjSteppingMgr.Instance.Clear();
		AsEntityManager.Instance.RemoveAllEntitiesAsync();
		while(true)
		{
			if(AsEntityManager.Instance.RemoveAllEntitiesAsyncFinished == true)
			{
				Debug.Log("AsSceneLoader::MapLoad: << RemoveAllEntitiesAsync process ends >> ");
				break;
			}
			
			yield return null;
		}
		AsEntityManager.Instance.RemoveAllEntities();
		ItemMgr.DropItemManagement.Clear();
		
		yield return null;
		
		if( true == AssetbundleManager.Instance.useAssetbundle)
			AssetbundleManager.Instance.ClearAssetbundle();
		
		Map map = TerrainMgr.Instance.GetMap( iMapID);
		Debug.Assert( null != map);
		AsHUDController.Instance.SetMapFogColor( map.MapData.RimLightColor);
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			WWW downloadCache = AssetbundleManager.Instance.SceneAssetbundleLoadCache( map.MapData.GetPath());
			yield return downloadCache;
		}

		AsyncOperation async = Application.LoadLevelAsync( map.MapData.GetPath());
		yield return async;
		
		TerrainMgr.Instance.LoadMap( iMapID);
		
		yield return null;

		//$yde
		AsUserEntity userChar = AsEntityManager.Instance.CreateUserEntity( "PlayerChar", AsUserInfo.Instance.SavedCharStat, true, true);
		userChar.SetRePosition( AsUserInfo.Instance.GetCurrentUserCharacterPosition());
		userChar.HandleMessage( new Msg_BuffRefresh( PlayerBuffMgr.Instance.CharBuffDataList));

		if( true == AsUserInfo.Instance.isProductionProgress)
			userChar.HandleMessage( new Msg_PRODUCT( true));
		
		userChar.gameObject.AddComponent<AudioListener>();
		
		AsUserInfo.Instance.ApplyPlayerStat();
		AsPetManager.Instance.PlayerEnterWorld();
		//~$yde
		
		yield return null;
		
		TerrainMgr.Instance.Initialize( iMapID);
		
		yield return null;

		bool ret = CameraMgr.Instance.AttachPlayerCamera( userChar.transform, map.MapData.GetMapNameShowTime());
		Debug.Assert( false != ret);
		
		while( eModelLoadingState.Finished != userChar.CheckModelLoadingState())
			yield return null;

		#region -AccountGender
		userChar.UserGender = AsUserInfo.Instance.accountGender;
		#region -GMMark
		userChar.IsGM = AsUserInfo.Instance.isGM;
		#endregion
#if NEW_DELEGATE_IMAGE
		if( null != userChar.namePanel)
		{
			DelegateImageData DelegateImageData = AsDelegateImageManager.Instance.GetSelectDelegateImage();
			if( null == DelegateImageData)
			{
				DelegateImageData = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
				if( null != DelegateImageData)
					userChar.nUserDelegateImageIndex = DelegateImageData.id;
			}
			else
				userChar.nUserDelegateImageIndex = DelegateImageData.id;

			userChar.namePanel.SetDelegateImage( AsPanel_Name.eNamePanelType.eNamePanelType_User, userChar);
		}
#else
		if( null != userChar.namePanel)
			userChar.namePanel.SetGenderMark( AsPanel_Name.eNamePanelType.eNamePanelType_User, userChar);
#endif
		#endregion
		
		AsCommonSender.ResetSendCheck();
		AsGameMain.SetBackGroundProc( false);
		switch( type)
		{
		case eLoadType.ENTER_WORLD:
			{
				AS_CG_ENTER_WORLD_END enterWorld = new AS_CG_ENTER_WORLD_END();
				byte[] data = enterWorld.ClassToPacketBytes();
				ArkQuestmanager.instance.CheckVisibleTimeUI();
				AsNetworkMessageHandler.Instance.Send( data);
				AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			
				if( null != AsEntityManager.Instance && null != AsEntityManager.Instance.GetPlayerCharFsm() &&
					null != AsChatManager.Instance)
				{
					Tbl_GlobalWeight_Record levelrecord = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 69);
					if( null != levelrecord)
					{
						int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>( eComponentProperty.LEVEL);
						if( iCurLevel >= (int)levelrecord.Value)
							AsChatManager.Instance.InsertSystemChat( AsTableManager.Instance.GetTbl_String(4083), eCHATTYPE.eCHATTYPE_SYSTEM);
					}
				}

                ArkQuestmanager.instance.ShowProgressInQuestMsgBox();
			
				AsCommonSender.SendWayPoint();
			}
			break;
		case eLoadType.WARP:
			AsCommonSender.SendWarpEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.WARPXZ:
			AsCommonSender.SendWarpXZEnd( iMapID, AsUserInfo.Instance.GetCurrentUserCharacterPosition());
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.PARTY_WARPXZ:
			AsPartySender.SendWarpXZEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;		
		case eLoadType.GOTO_TOWN:
			AsCommonSender.SendGotoTownEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			#region -GameGuide_Death
			AsGameGuideManager.Instance.CheckUp( eGameGuideType.Death, AsUserInfo.Instance.SavedCharStat.level_);
			#endregion
			break;
		case eLoadType.INSTANCE_DUNGEON_ENTER:
			AsInstanceDungeonManager.Instance.Send_InDun_Enter_End();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.ARENA_ENTER:
			AsInstanceDungeonManager.Instance.Send_InDun_Enter_End();
			AsPvpManager.Instance.EnterArena();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = false;
			break;
		case eLoadType.INSTANCE_DUNGEON_EXIT:
			AsInstanceDungeonManager.Instance.Send_InDun_Exit_End();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		}
		
		//$yde
		if( eLoadType.ARENA_ENTER != type)
			AsInputManager.Instance.m_Activate = true;
		CoolTimeGroupMgr.Instance.CheckLoadMap();
		
		yield return null;
		
		AsIntoMapTriggerManager.instance.UpdateIntoMapTrigger( iMapID);
		AsUseItemInMapMarkManager.instance.UpdateUseItemInMapMark( iMapID);
		ArkQuestmanager.instance.CheckVisibleTimeUI();
		AsHudDlgMgr.Instance.UpdateQuestBookBtn();
		QuestHolderManager.instance.UpdateQuestHolder();
		AsInGameEventManager.instance.ForcedUpdate();

		if( AsHudDlgMgr.Instance != null)
		{
			AsHudDlgMgr.Instance.cashShopBtn.GetComponent<CashShopBtnController>().SetMiracleTxt(AsUserInfo.Instance.nMiracle);
			AsHudDlgMgr.Instance.SetCashStoreBtnFreeMark(AsUserInfo.Instance.FreeGachaPoint == 1);
			AsHudDlgMgr.Instance.partyAndQuestToggleMgr.Initilize();
			AsHudDlgMgr.Instance.CheckNewMenuImg();
		}
		
		yield return null;
		
		AsCharacterTextureManager.Instance.Clear();
		
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
		
		ActivateLoadInfo( false);
		
		if( true == IsInvoking( "ConditionAlarm"))
			CancelInvoke( "ConditionAlarm");
		
		InvokeRepeating( "ConditionAlarm", 0.0f, 1800.0f);
		
		#region -GameGuide_Map
		AsGameGuideManager.Instance.CheckUp( eGameGuideType.Map, iMapID);
		#endregion
		TerrainMgr.Instance.ShowMapNameShow(iMapID);

		#region -AccountGender
#if !NEW_DELEGATE_IMAGE
		PromptGenderDialog();
#endif
		#endregion
		
		BonusManager.Instance.SceneLoaded();//$yde
		AsEmotionManager.Instance.ReleaseBlock();//$yde
		

		AsUseItemToMonTriggerManager.instance.CreateAllTrigger();
		AsUseItemToMonTriggerManager.instance.RepoisitionTrigger();
		ArkQuestmanager.instance.UpdateSortQuestList();
		AsHudDlgMgr.Instance.UpdateQuestMiniView();
		AsPromotionManager.Instance.SceneLoaded();
		AutoCombatManager.Instance.ZoneWarpFinished();
		
		Debug.LogWarning("map Load end");
	}

	#region -AccountGender
	void PromptGenderDialog()
	{
		if( eGENDER.eGENDER_NOTHING != AsUserInfo.Instance.accountGender)
			return;

		GameObject genderMsgBox = ResourceLoad.CreateGameObject( "UI/Optimization/Prefab/MessageBox_Gender");
		AsHudDlgMgr.Instance.GenderDlg = genderMsgBox.GetComponentInChildren<AsMessageBoxGender>();
	}
	#endregion
	
	void ConditionAlarm()
	{
		if( 0 != AsUserInfo.Instance.CurConditionValue)
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(862), eCHATTYPE.eCHATTYPE_SYSTEM);
		else
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(863), eCHATTYPE.eCHATTYPE_SYSTEM);
	}
	//$yde
	
	public void LoadSameMap( int iMapID, eLoadType type)
	{
		ActivateLoadInfo( true);
		
		AsEntityManager.Instance.RemoveAllMonster();
		AsEntityManager.Instance.RemoveAllNpc();
		AsEntityManager.Instance.RemoveAllOtherUser();
		
		AsEntityManager.Instance.UserEntity.SetRePosition(AsUserInfo.Instance.GetCurrentUserCharacterPosition());
		AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_MoveStopIndication());
		AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_ZoneWarpEnd());
		
		if( null != ItemMgr.DropItemManagement )
			ItemMgr.DropItemManagement.Clear();
		
		switch( type)
		{
		case eLoadType.ENTER_WORLD:
			{
				AS_CG_ENTER_WORLD_END enterWorld = new AS_CG_ENTER_WORLD_END();
				byte[] data = enterWorld.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
				
				AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			}
			break;
		case eLoadType.WARP:
			AsCommonSender.SendWarpEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.WARPXZ:
			AsCommonSender.SendWarpXZEnd( iMapID, AsUserInfo.Instance.GetCurrentUserCharacterPosition());
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.PARTY_WARPXZ:
			AsPartySender.SendWarpXZEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;	
		case eLoadType.GOTO_TOWN:
			AsCommonSender.SendGotoTownEnd();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			#region -GameGuide_Death
			AsGameGuideManager.Instance.CheckUp( eGameGuideType.Death, AsUserInfo.Instance.SavedCharStat.level_);
			#endregion
			break;
		case eLoadType.INSTANCE_DUNGEON_ENTER:
			AsInstanceDungeonManager.Instance.Send_InDun_Enter_End();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		case eLoadType.ARENA_ENTER:
			AsInstanceDungeonManager.Instance.Send_InDun_Enter_End();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			AsInputManager.Instance.m_Activate = false;
			break;
		case eLoadType.INSTANCE_DUNGEON_EXIT:
			AsInstanceDungeonManager.Instance.Send_InDun_Exit_End();
			AsGameMain.s_gameState = GAME_STATE.STATE_INGAME;
			break;
		}
		
		ActivateLoadInfo( false);
		
		if( eLoadType.ARENA_ENTER != type)
			AsInputManager.Instance.m_Activate = true;
		
		AsIntoMapTriggerManager.instance.UpdateIntoMapTrigger(iMapID);
		AsUseItemInMapMarkManager.instance.UpdateUseItemInMapMark( iMapID);
		
		AsEntityManager.Instance.MessageToPlayer( new Msg_PetPositionRefresh());
		AsEmotionManager.Instance.ReleaseBlock();//$yde
		
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
	}

	void PreLoad_Model(int nMapID)
	{
		if( null == AssetbundleManager.Instance || false == AssetbundleManager.Instance.useAssetbundle)
			return;

//		PreLoad_Pc();
		PreLoad_Npc( nMapID);
	}
	
	void PreLoad_Pc()
	{
		for( int i = 1; i <= 3; i++)
		{
			Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record(i);
			if( null != record)
			{
				AssetbundleManager.Instance.GetAssetbundleGameObject( record.ModelingPath_Male);
				AssetbundleManager.Instance.GetAssetbundleGameObject( record.ModelingPath_Female);
				Debug.Log( "PreLoadModel(): " + record.ModelingPath_Male);
				Debug.Log( "PreLoadModel(): " + record.ModelingPath_Female);
			}
		}
		
		PreLoad_PcItem( 10000);
		PreLoad_PcItem( 20000);
		PreLoad_PcItem( 30000);
		PreLoad_PcItem( 12000);
		PreLoad_PcItem( 22000);
		PreLoad_PcItem( 32000);
		PreLoad_PcItem( 16000);
		PreLoad_PcItem( 26000);
		PreLoad_PcItem( 36000);
	}
	
	void PreLoad_PcItem(int nItemID)
	{
		Item item = ItemMgr.ItemManagement.GetItem( nItemID);
		if( null == item)
			return;

		AssetbundleManager.Instance.GetAssetbundleGameObject( item.ItemData.GetPartsItem_M());
		AssetbundleManager.Instance.GetAssetbundleGameObject( item.ItemData.GetPartsItem_W());
		AssetbundleManager.Instance.GetAssetbundleGameObject( item.ItemData.GetPartsItemDiff_M());
		AssetbundleManager.Instance.GetAssetbundleGameObject( item.ItemData.GetPartsItemDiff_W());
		
		Debug.Log( "PreLoadModel(): " + item.ItemData.GetPartsItem_M());
		Debug.Log( "PreLoadModel(): " + item.ItemData.GetPartsItem_W());
		Debug.Log( "PreLoadModel(): " + item.ItemData.GetPartsItemDiff_M());
		Debug.Log( "PreLoadModel(): " + item.ItemData.GetPartsItemDiff_W());
	}

	void PreLoad_Npc(int nMapID)
	{
		Tbl_PreLoad_Record record = AsTableManager.Instance.GetPreLoadRecord( nMapID);
		
		if( null == record)
			return;

		if( record.Data.Length <= 0)
			return;
		
		string[] strNpcID = record.Data.Split( '/');
		
		foreach( string str in strNpcID)
		{
			if( true == _isDigit( str))
				PreLoad_NpcFormID( Convert.ToInt32( str));
		}
	}
	
	void PreLoad_NpcFormID(int id)
	{
		Tbl_Npc_Record record = AsTableManager.Instance.GetTbl_Npc_Record( id);
		
		if( null == record)
			return;

		string strPath = record.ModelingPath;
		AssetbundleManager.Instance.GetAssetbundleGameObject( strPath);
		
		Debug.Log( "PreLoadModel(): " + strPath);
	}

	private bool _isDigit(string str)
	{
		if( str == null)
			return false;

		return System.Text.RegularExpressions.Regex.IsMatch( str, "^\\d+$");
	}

	private ePatchGroup _GetPatchGroup(int nMapID)
	{
		Map map = TerrainMgr.Instance.GetMap( nMapID);
		if( null == map)
		{
			Debug.Log( "AsSceneLoader::_GetPatchGroup(): null == map, MapID: " + nMapID);
			return ePatchGroup.ePatchGroup_Invalid;
		}
		
		return map.MapData.GetPatchGroup();
	}
}
