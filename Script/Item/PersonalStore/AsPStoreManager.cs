using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum ePStoreState {Closed, User_Folded, User_Standby, User_Opening, Another_Opened}
public enum ePStoreEnableSlot {NONE = 0, Ticket_1 = 1, Ticket_2 = 3, Ticket_3 = 6, Ticket_4 = 9, Ticket_5 = 12}

public class AsPStoreManager : MonoBehaviour {
	
	#region - singleton -
	static AsPStoreManager m_instance;
	public static AsPStoreManager Instance{ get{ return m_instance;}}
	#endregion
	#region - member -
	public static ushort s_ShopOpeningSession = ushort.MaxValue;

	public static float s_ShopOpenPossibleDistance = 0;
	
	public static bool s_IsSendCS_PRIVATESHOP_ENTER = true;
	
	[SerializeField] ePStoreState m_StoreState; public ePStoreState storeState{get{return m_StoreState;}}
	ePStoreEnableSlot m_PStoreEnableSlot; public ePStoreEnableSlot PStoreEnableSlot{get{return m_PStoreEnableSlot;}}
	RealItem m_PStoreItemUsedSlot; public RealItem PStoreItemUsedSlot{get{return m_PStoreItemUsedSlot;}}
	
//	[SerializeField] UIPStoreDlg m_PStoreDlg = null;
	
	string m_strTitle = ""; public string strTitle{get{return m_strTitle;}}
	string m_strContent = ""; public string strContent{get{return m_strContent;}}
	
	Dictionary<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> m_dicPlayerShopItem = new Dictionary<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST>();
	public Dictionary<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> dicPlayerShopItem
	{
		get
		{
			foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
			{
				Debug.Log("AsPStoreManager::listPlayerShopItem: element inven slot = " + pair.Value.nInvenSlot);
			}
			
			return m_dicPlayerShopItem;
		}
	}
	
	uint m_CurShopUId; public uint curShopUId{get{return m_CurShopUId;}}
	
	float m_RemainTime; public float RemainTime{get{return m_RemainTime;}}
	
	bool m_ShopModelingLoaded; public bool ShopModelingLoaded{get{return m_ShopModelingLoaded;}}
	GameObject m_OriginShopModeling;
	
	bool m_OpenCoolRemained; public bool OpenCoolRemained{get{return m_OpenCoolRemained;}}
	
	UIPStoreSearchDlg m_SearchDlg;
	#endregion
	
	#region - init & release & update -
	void Awake()
	{
		#region - singleton -
		m_instance = this;
		#endregion
	}

	void Start()
	{
//		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
//		{
//			StartCoroutine(LoadShopModeling());
//		}
	}
	
	public void LoadShopModel_Coroutine()
	{
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			StartCoroutine(LoadShopModeling());
		}
	}
	
	IEnumerator LoadShopModeling()
	{
		m_ShopModelingLoaded = false;
		Debug.Log("AsPStoreManager::LoadShopModeling: pstore model loading started");
		////////////////// < private shop load
		//GameObject model =  Instantiate(ResourceLoad.LoadGameObject("Character/Pc/Town/PrivateShop")) as GameObject;
		// < loading assetbundle
		string strNameLower_privateshop = "privateshop";
		string strAssetbundlePath_privateshop = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strNameLower_privateshop);
		int nVersion = AssetbundleManager.Instance.GetCachedVersion( strNameLower_privateshop);
		
		Debug.Log("AsModel::LoadShopModeling: WWW.LoadFromCacheOrDownload( " + strAssetbundlePath_privateshop + ", " + nVersion + ")");
		WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath_privateshop, nVersion);
		yield return www;
		
		AssetBundle bundle = www.assetBundle;
		AssetBundleRequest request = bundle.LoadAsync( strNameLower_privateshop, typeof( GameObject));
		yield return request;
		
		GameObject obj = request.asset as GameObject;
		if( null == obj)
		{
			Debug.LogError( "AsPStoreManager::LoadShopModeling: There is no [ privateshop.asab ] modeling resource.");
			obj = GameObject.CreatePrimitive( PrimitiveType.Cube);
		}
		
		GameObject model = Instantiate( obj) as GameObject;
		
		bundle.Unload( false);
		
		Debug.Log("AsPStoreManager::LoadShopModeling: CreateMaterial( " + model + ", " + strNameLower_privateshop + ")");
		yield return StartCoroutine( CreateMaterial( model, strNameLower_privateshop));
		////////////////// private shop load >
		
		m_OriginShopModeling = model;
		m_OriginShopModeling.transform.parent = transform;
		
		m_OriginShopModeling.transform.localPosition = new Vector3( 0.0f, -100.0f, 0.0f);//Vector3.zero;
		m_OriginShopModeling.transform.localRotation = Quaternion.identity;
		
		Debug.Log("AsPStoreManager::LoadShopModeling: pstore model loading ends");
		m_ShopModelingLoaded = true;
	}
	
	IEnumerator CreateMaterial( GameObject model, string name)
	{
		AssetbundleHelper helper = model.GetComponent<AssetbundleHelper>();
		if( null == helper)
		{
			Debug.Log( "AsPStoreManager::LoadShopModeling: Not Found Assetbundle Helper: " + name);
			yield break;
		}
		
		Renderer[] rens = model.GetComponentsInChildren<Renderer>();
		foreach( Renderer ren in rens)
		{
			int nIndex = helper.m_listMeshName.IndexOf( ren.name.ToLower());
			if( 0 > nIndex)
			{
				Debug.Log( "AsPStoreManager::LoadShopModeling: InitialNpcCharacter() NoMatchRenderMeshName: " + model.name + ", " + ren.name);
				continue;
			}
			
			if( nIndex > helper.m_listShaderName.Count - 1)
			{
				Debug.Log( "AsPStoreManager::LoadShopModeling: InitialNpcCharacter() index: " + nIndex + ", shader count: " + helper.m_listShaderName.Count);
				continue;
			}
			
			if( nIndex > helper.m_listTextureName.Count - 1)
			{
				Debug.Log( "AsPStoreManager::LoadShopModeling: InitialNpcCharacter() index: " + nIndex + ", texture count: " + helper.m_listTextureName.Count);
				continue;
			}
			
			string strShaderName = helper.m_listShaderName[nIndex];
			string sgtrNameLower = helper.m_listTextureName[nIndex].ToLower();
			string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( sgtrNameLower);
			int nVersion = AssetbundleManager.Instance.GetCachedVersion( sgtrNameLower);
			
			Debug.Log( "AsPStoreManager::LoadShopModeling: Texture path : " + strAssetbundlePath);
			
#if _USE_TEXTURE_MANAGER
			Texture tempTex = AsCharacterTextureManager.Instance.Get( sgtrNameLower);
			if( null == tempTex)
			{
				WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
				yield return www;
				
				AssetBundle bundle = www.assetBundle;
				tempTex = bundle.Load( sgtrNameLower) as Texture;
				
				Debug.Log( "Texture loaded: " + sgtrNameLower + ", " + tempTex);
				
				bundle.Unload( false);
			}
			
			ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tempTex);
#else
			WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
			yield return www;
			
			AssetBundle bundle = www.assetBundle;
			Texture tex = bundle.Load( sgtrNameLower) as Texture;
			
			Debug.Log( "Texture loaded: " + sgtrNameLower + ", " + tex);
			
			bundle.Unload( false);
			
			ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tex);
#endif
		}
	}
	
	void Update()
	{
		if(m_RemainTime > 0)
			m_RemainTime -= Time.deltaTime;
		
//		if(Input.GetMouseButtonDown(0) == true)
//			StartCoroutine(CheckScrollListPos());
	}
	
	IEnumerator CheckScrollListPos()
	{
		if(AsHudDlgMgr.Instance != null &&
			AsHudDlgMgr.Instance.pstoreDlg != null &&
			AsHudDlgMgr.Instance.IsOpenPStore == true)
		{
			UIScrollList list = AsHudDlgMgr.Instance.pstoreDlg.ScrollList;
			
			while(true)
			{
				if(Input.GetMouseButton(0) == false)
					break;
				
				yield return new WaitForSeconds(0.2f);
				
				Debug.Log("AsPStoreManager::CheckScrollListPos: scroll positionn = " + list.ScrollPosition);
			}
		}
	}
	
//	bool m_TimeShow = false;
	float m_PausedTime = 0;
//	float m_SecondPausedTime = 0;
	float m_RevisionTime = 0;
	void OnApplicationPause(bool pause)
	{
		switch(pause)
		{
		case true:
			switch(m_StoreState)
			{
			case ePStoreState.User_Opening:
//				m_PausedTime = System.DateTime.Now.Ticks / 100000000f;
//				Debug.LogWarning("AsPStoreManager::OnApplicationPause(" + pause + "): m_PausedTime = " + m_PausedTime);
				m_PausedTime = Time.realtimeSinceStartup;
				Debug.LogWarning("AsPStoreManager::OnApplicationPause(" + pause + "): m_PausedTime = " + m_PausedTime);
				AsHudDlgMgr.Instance.ClosePStore();
				break;
			}
			break;
		case false:
			switch(m_StoreState)
			{
			case ePStoreState.User_Folded:
//				m_SecondPausedTime = System.DateTime.Now.Ticks / 100000000f;
//				m_RevisionTime = m_SecondPausedTime - m_PausedTime;
//				Debug.LogWarning("AsPStoreManager::OnApplicationPause(" + pause + "): m_SecondPausedTime = " + m_SecondPausedTime);
//				Debug.LogWarning("AsPStoreManager::OnApplicationPause(" + pause + "): m_RevisionTime = " + m_RevisionTime);
				m_RevisionTime = Time.realtimeSinceStartup - m_PausedTime;
				Debug.LogWarning("AsPStoreManager::OnApplicationPause(" + pause + "): m_RevisionTime = " + m_RevisionTime);
				m_RemainTime -= m_RevisionTime;
//				StartCoroutine(TimeShowing());
				break;
			}
			break;
		}
	}
	
	IEnumerator TimeShowing()
	{
//		m_TimeShow = true;
		
		yield return new WaitForSeconds(10);
		
//		m_TimeShow = false;
	}
	
	void OnEnable()
	{
		m_OpenCoolRemained = false;
	}
	
	void OnDisable()
	{
	}
	#endregion
	#region - open & close -
	public void ReadyPStore()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if(user.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == false)
		{
			body_CG_CHEAT cheat = new body_CG_CHEAT(26, "", 1, 0, 0, 0, 0, 0);
			AsCommonSender.Send(cheat.ClassToPacketBytes());
		}
		else
		{
			m_StoreState = ePStoreState.User_Opening;
			
			Debug.LogWarning("Bug Searching::: AsPStoreManager:: ReadyPStore: private shop dialog is instantiated");
			AsHudDlgMgr.Instance.OpenPStore();
		}
	}
	
//	public void PStoreDlgOpened(UIPStoreDlg _pstoreDlg)
//	{
////		m_PStoreDlg = _pstoreDlg;
//	}
 	
	public void Close()
	{
		Debug.LogWarning("AsPStoreManager::Close: store state = " + m_StoreState);
		
//		m_PStoreDlg = null;
		
		switch(m_StoreState)
		{
		case ePStoreState.User_Standby:
			Request_Destroy();
			m_StoreState = ePStoreState.Closed;
			break;
		case ePStoreState.User_Opening:
			m_StoreState = ePStoreState.User_Folded;
			break;
		case ePStoreState.Another_Opened:
			Request_Leave();
			m_StoreState = ePStoreState.Closed;
			break;
		}
	}
	#endregion
	#region - send to server -
	public void Request_Create()
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		playerFsm.ReleaseTarget();
		
//		string name = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>(eComponentProperty.NAME);
		m_strContent = "";// string.Format(AsTableManager.Instance.GetTbl_String(1231), name);
		
		Debug.Log("AsPStoreManager::Request_Create: m_strContent = " + m_strContent);
		
		byte[] content = System.Text.Encoding.UTF8.GetBytes(m_strContent);
		
		body_CS_PRIVATESHOP_CREATE create = new body_CS_PRIVATESHOP_CREATE(content);
		AsCommonSender.Send(create.ClassToPacketBytes());
		Debug.Log("AsPStoreManager::Request_Create: " + m_strContent);
	}	
	public void Request_Destroy()
	{
		body_CS_PRIVATESHOP_DESTROY destroy = new body_CS_PRIVATESHOP_DESTROY();
		AsCommonSender.Send(destroy.ClassToPacketBytes());
		
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
			Debug.LogError("AsPStoreManager::Request_Destroy: game state = " + AsGameMain.s_gameState);
	}	
	public void Request_Modify()
	{
		byte[] title = System.Text.Encoding.UTF8.GetBytes(m_strContent);
		
		body_CS_PRIVATESHOP_MODIFY mofidy = new body_CS_PRIVATESHOP_MODIFY(title);
		AsCommonSender.Send(mofidy.ClassToPacketBytes());
		
		Debug.Log("AsPStoreManager::Request_Modify: " + m_strContent);
	}
	public void Request_Registraion_Item(body_CS_PRIVATESHOP_REGISTRATION_ITEM _registration)
	{
		AsCommonSender.Send(_registration.ClassToPacketBytes());
		
		Debug.Log("ItemReleasedOnPStoreDlg: packet send. start slot index = " + _registration.nInvenSlot +
			", dest slot index = " + _registration.nPrivateShopSlot + ", count = " + _registration.nItemCount);
	}
	public void Request_Open()
	{
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		eModelLoadingState modelLoading = player.CheckModelLoadingState();
		bool partsLoaded = player.CheckPartsLoaded();
		
		if(modelLoading == eModelLoadingState.Started || partsLoaded == false)
		{
			Debug.Log("AsPStoreManager::Request_Open: waiting for loading completion.");
			return;
		}
		else
		{	
			body_CS_PRIVATESHOP_OPEN open = new body_CS_PRIVATESHOP_OPEN();
			AsCommonSender.Send(open.ClassToPacketBytes());
			
			Debug.Log("AsPStoreManager::Request_Open: send [" + open + "]");
		}
	}	
	public void Request_Close()
	{
//		if(m_Closing == true)
//			return;
//		
//		StartCoroutine(WaitForClosing());
		
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		eModelLoadingState modelLoading = player.CheckModelLoadingState();
		bool partsLoaded = player.CheckPartsLoaded();
		
		if(modelLoading == eModelLoadingState.Started || partsLoaded == false)
		{
			Debug.Log("AsPStoreManager::Request_Close: waiting for loading completion.");
			return;
		}
		else
		{
			body_CS_PRIVATESHOP_CLOSE close  = new body_CS_PRIVATESHOP_CLOSE();
			AsCommonSender.Send(close.ClassToPacketBytes());
			
			Debug.Log("AsPStoreManager::Request_Close: " + close);
		}
	}
	public void Request_List(body1_SC_PRIVATESHOP_LIST _list)
	{
	}
	public void Request_Enter(UInt32 _shopUId, string _name, byte[] _content)
	{
		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		if( false == s_IsSendCS_PRIVATESHOP_ENTER )
			return;
		
		body_CS_PRIVATESHOP_ENTER enter = new body_CS_PRIVATESHOP_ENTER(_shopUId);
		AsCommonSender.Send(enter.ClassToPacketBytes());
		s_IsSendCS_PRIVATESHOP_ENTER = false;
		
		m_CurShopUId = _shopUId;
		m_strTitle = string.Format(AsTableManager.Instance.GetTbl_String(1231), _name);
		SetContent(_content);
		
		Debug.Log("AsPStoreManager::Request_Enter: shop uid = " + enter.nPrivateShopUID);
		Debug.Log("AsPStoreManager::Request_Enter: m_strContent = " + m_strContent);
	}
	public void Request_Leave()
	{
		body_CS_PRIVATESHOP_LEAVE leave = new body_CS_PRIVATESHOP_LEAVE(m_CurShopUId);
		AsCommonSender.Send(leave.ClassToPacketBytes());
		
		Debug.Log("AsPStoreManager::Request_Leave: shop uid = " + leave.nPrivateShopUID);
	}
//	public void Request_ItemList(body1_CS_PRIVATESHOP_ITEMLIST _list)
//	{
//	}
	public void Request_ItemBuy(body_CS_PRIVATESHOP_ITEMBUY _buy)
	{
		AsCommonSender.Send(_buy.ClassToPacketBytes());
		
		Debug.Log("Request_ItemBuy: packet send. shop uid = " + _buy.nPrivateShopUID +
			", dest slot index = " + _buy.nPrivateShopSlot + ", count = " + _buy.nItemCount);
	}
	#endregion
	#region - recv from server -
	public void Recv_Create(body_SC_PRIVATESHOP_CREATE _create)
	{
		if(_create.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			m_dicPlayerShopItem.Clear();
			
			m_StoreState = ePStoreState.User_Standby;
			
			Debug.LogWarning("Bug Searching::: AsPStoreManager:: Recv_Create: private shop dialog is instantiated");
			AsHudDlgMgr.Instance.OpenPStore();
		}
		
//		AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_MoveStopIndication());
	}
	public void Recv_Destroy(body_SC_PRIVATESHOP_DESTROY _destroy)
	{
		Debug.Log("AsPStoreManager::Recv_Destroy: AsGameMain.isBackGroundPause = " + AsGameMain.isBackGroundPause);
		Debug.Log("AsPStoreManager::Recv_Destroy: AsGameMain.s_gameState = " + AsGameMain.s_gameState);
		
		m_StoreState = ePStoreState.Closed;
		
		switch(AsGameMain.s_gameState)
		{
		case GAME_STATE.STATE_CHARACTER_SELECT:
			AsCharacterSlotManager.ShopDestroyed();
			AsUserInfo.Instance.ClosePrivateShop();
			break;
		case GAME_STATE.STATE_LOADING:
//		case GAME_STATE.STATE_LOAD_END:
		case GAME_STATE.STATE_INGAME:
			ClearRegisteredShopItem();
			AsUserInfo.Instance.ClosePrivateShop();
			
			AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
			Debug.Log("AsPStoreManager::Recv_Destroy: AsUserInfo.Instance.GetCurrentUserEntity() = " + player);
			if(player != null)
			{
				player.HandleMessage(new Msg_ClosePrivateShop());
				Debug.Log("AsPStoreManager::Recv_Destroy: player.GetProperty<bool>(eComponentProperty.SHOP_OPENING) = " +
					player.GetProperty<bool>(eComponentProperty.SHOP_OPENING));
			}
			
			Debug.Log("AsPStoreManager::Recv_Destroy: AsHudDlgMgr.Instance.IsOpenPStore = " + AsHudDlgMgr.Instance.IsOpenPStore);
			if(AsHudDlgMgr.Instance.IsOpenPStore == true)
				AsHudDlgMgr.Instance.ClosePStore();
			break;
		}
		
		foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
		{
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(pair.Value.nInvenSlot, false);
		}
		m_dicPlayerShopItem.Clear();
		
		Debug.Log("AsPStoreManager::Recv_Destroy: " + _destroy.eResult + "(GAME_STATE = " + AsGameMain.s_gameState + ")");
	}
	public void Recv_Modify(body_SC_PRIVATESHOP_MODIFY _modify)
	{
		Debug.Log("AsPStoreManager::Recv_Modify: " + _modify.eResult);
	}
	public void Recv_Registraion_Item(body_SC_PRIVATESHOP_REGISTRATION_ITEM _registration)
	{
		if(_registration.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.Log(_registration.eResult);
			return;
		}
		
		/*Debug.Log("Recv_Registration_Item: " + 
			_registration.bAddOrDel + ", " + _registration.nInvenSlot + ", " + _registration.nPrivateShopSlot + ", " +
			_registration.sPrivateShopItem.nItemTableIdx + ", " + _registration.sPrivateShopItem.nAttribute);*/
		
		if(_registration.bAddOrDel == true)
		{
			AsHudDlgMgr.Instance.pstoreDlg.SetPStoreItem(_registration);         
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(_registration.nInvenSlot, true);
			
			body2_SC_PRIVATESHOP_OWNER_ITEMLIST item = new body2_SC_PRIVATESHOP_OWNER_ITEMLIST();
			item.nInvenSlot = _registration.nInvenSlot;
			item.nItemGold = _registration.nItemSellGold;
			item.nPrivateShopSlot = _registration.nPrivateShopSlot;
			item.sItem = _registration.sPrivateShopItem;
			item.nMaxOverlapped = _registration.sPrivateShopItem.nOverlapped;
			
			if(m_dicPlayerShopItem.ContainsKey(item.nPrivateShopSlot) == true)
				m_dicPlayerShopItem.Remove(item.nPrivateShopSlot);
			
			m_dicPlayerShopItem.Add(item.nPrivateShopSlot, item);
		}
		else
		{
			AsHudDlgMgr.Instance.pstoreDlg.RemovePStoreItem(_registration);
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(_registration.nInvenSlot, false);
			
			foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
			{
				if(pair.Value.nPrivateShopSlot == _registration.nPrivateShopSlot)
				{
					Debug.Log("AsPStoreManager::Recv_Registration_Item: item[nPrivateShopSlot(" + _registration.nPrivateShopSlot + ")] in current shop list is deleted.");
					m_dicPlayerShopItem.Remove(pair.Key);
					break;
				}
			}
		}
		
		if( AsHudDlgMgr.Instance.IsOpenInven )
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
	}
	public void Recv_Open(body_SC_PRIVATESHOP_OPEN _open)
	{
		Debug.Log("AsPStoreManager::Recv_Open: " + _open.eResult);
		
		if(_open.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId(_open.nCharUniqKey);
			if(entity == null)
			{
				entity = AsEntityManager.Instance.CreateUserEntity("OtherUser", new OtherCharacterAppearData(_open));
//				entity.SetRePosition(_open.sCurPosition);
//				Debug.LogError("Recv_Open: _open.nCharUniqKey = " + _open.nCharUniqKey);
			}
			
			if(entity.FsmType == eFsmType.PLAYER)
			{
				// cool time
				StartCoroutine(OpenCoolProcess());
				string notify = AsTableManager.Instance.GetTbl_String(417);
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(notify);
				
				m_CurShopUId = _open.nPrivateShopUID;
				m_StoreState = ePStoreState.User_Opening;
				
				AsUserInfo.Instance.SetPrivateShopInfo(_open);
				m_RemainTime = (float)_open.nPrivateShopRemainingTime;
				
				m_PStoreItemUsedSlot = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot(_open.nOpenItemInvenSlot);
				if(m_PStoreItemUsedSlot == null)
					Debug.LogWarning("AsPStoreManager::Recv_Open: _open.nOpenItemInvenSlot = " + _open.nOpenItemInvenSlot +
						". check whether open item was expired.");
					
				SetContent(_open.strContent);
				entity.SetShopData(_open);
				AsHudDlgMgr.Instance.pstoreDlg.BeginTimeProcess();
				if(AsHudDlgMgr.Instance.IsOpenPStore == true)
				{
					AsHudDlgMgr.Instance.pstoreDlg.SetBtnByState();
					AsHudDlgMgr.Instance.pstoreDlg.BeginTimeProcess();
				}
				
				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_OpenPrivateShop(_open.nOpenItemTableIdx));
				
//				m_OpenInfo = _open;
			}
			else
			{
				entity.SetShopData(_open);
				entity.HandleMessage(new Msg_OpenPrivateShop(_open.nOpenItemTableIdx));
			}
			
			entity.SetRePosition(_open.sCurPosition);
			
			Debug.Log("AsPStoreManager::Recv_Open: _open.nCharUniqKey = " + _open.nCharUniqKey);
			Debug.Log("AsPStoreManager::Recv_Open: _open.sCurPosition = " + _open.sCurPosition);
		}
	}
	IEnumerator OpenCoolProcess()
	{
		float cool = 10f;
		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record("PrivateStoreCoolTime");
		if(record != null) cool = record.Value;
		
		m_OpenCoolRemained = true;
		
		yield return new WaitForSeconds(cool);
		
		m_OpenCoolRemained = false;
		
		if(AsHudDlgMgr.Instance.IsOpenPStore == true)
		{
			AsHudDlgMgr.Instance.pstoreDlg.OpenCoolFinished();
		}
	}
	public void Recv_Close(body_SC_PRIVATESHOP_CLOSE _close)
	{
		Debug.Log("AsPStoreManager::Recv_Close: " + _close.eResult);
		Debug.Log("Recv_Close: m_CurShopUId = " + m_CurShopUId + ", _close.nPrivateShopUID = " + _close.nPrivateShopUID);
		
		if(_close.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId(_close.nCharUniqKey);
			if(entity == null)
			{
				Debug.LogError("Recv_Close: _close.nCharUniqKey = " + _close.nCharUniqKey);
				return;
			}
			
//			Debug.Log("Recv_Close: m_CurShopUId = " + m_CurShopUId + ", _close.nPrivateShopUID = " + _close.nPrivateShopUID);
//			Debug.Log("Recv_Close: _close.bIsDelete = " + _close.bIsDelete);
			
			if(entity.FsmType == eFsmType.PLAYER)
			{
				AsUserInfo.Instance.ClosePrivateShop();
				
				m_StoreState = ePStoreState.User_Standby;
				if(AsHudDlgMgr.Instance.IsOpenPStore == true)
				{
					AsHudDlgMgr.Instance.pstoreDlg.SetBtnByState();
					AsHudDlgMgr.Instance.pstoreDlg.StopTimeProcess();
					AsHudDlgMgr.Instance.pstoreDlg.SetDefaultTimeProcess();
				}
				
				AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
				player.HandleMessage(new Msg_ClosePrivateShop());
			}
			else if(AsHudDlgMgr.Instance.IsOpenPStore == true && _close.nPrivateShopUID == m_CurShopUId)
			{
				AsHudDlgMgr.Instance.ClosePStore();
				m_CurShopUId = uint.MaxValue;
			}
			
			if(entity.FsmType != eFsmType.PLAYER)
				AsEntityManager.Instance.RemoveEntity(entity);
		}
	}
	public void Recv_List(body1_SC_PRIVATESHOP_LIST _list)
	{
		Debug.Log("AsPStoreManager::Recv_List: _list.body.Length = " + _list.body.Length);
		
		foreach(body2_SC_PRIVATESHOP_LIST shop in _list.body)
		{
			AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId(shop.nCharUniqKey);
			
			if(entity != null)
			{
				entity.SetShopData(shop);
				entity.HandleMessage(new Msg_OpenPrivateShop(shop.nItemTableIdx));
				
				if(m_StoreState == ePStoreState.User_Opening && entity.FsmType == eFsmType.PLAYER)
				{
//					InvenSlot invenSlot = ItemMgr.HadItemManagement.Inven.GetInvenSlotItem(AsUserInfo.Instance.nPrivateShopCreateItemSlot);
//					if(invenSlot != null)
//						SetPStoreOpenItem(invenSlot.realItem.sItem.nItemTableIdx);
//					else
//						Debug.LogWarning("AsPStoreManager::Recv_List: open item is expired.");
					SetPStoreOpenItem(shop.nItemTableIdx);
					
//					Debug.LogWarning("Bug Searching::: AsPStoreManager:: Recv_List: private shop dialog is instantiated");
					AsHudDlgMgr.Instance.OpenPStore();
					
					SetContent(shop.strContent);
					
//					Debug.Log("AsPStoreManager::Recv_List: m_strContent = " + m_strContent);
				}
			}
			else
			{
				AsUserEntity added = AsEntityManager.Instance.CreateUserEntity("OtherUser", new OtherCharacterAppearData(shop));
				added.SetShopData(shop);
				added.HandleMessage(new Msg_OpenPrivateShop(shop.nItemTableIdx));
			}
		}
	}
	public void Recv_Enter(body_SC_PRIVATESHOP_ENTER _enter)
	{
		if(_enter.eResult == eRESULTCODE.eRESULT_SUCC)
		{
//			AsHudDlgMgr.Instance.OpenPStore();
		}
		
		Debug.Log("AsPStoreManager::Recv_Enter: " + _enter.eResult);
	}
	public void Recv_Leave(body_SC_PRIVATESHOP_LEAVE _leave)
	{
		Debug.Log("AsPStoreManager::Recv_Leave: ");
		
//		if(_leave.eResult == eRESULTCODE.eRESULT_SUCC)
//		{
//			m_CurShopUId = uint.MaxValue;
//		}
		
		m_StoreState = ePStoreState.Closed;
	}
	public void Recv_ItemList(body1_SC_PRIVATESHOP_ITEMLIST _list)
	{
		m_StoreState = ePStoreState.Another_Opened;
	
		Debug.LogWarning("Bug Searching::: AsPStoreManager:: Recv_ItemList: private shop dialog is instantiated");
		
		if( m_SearchDlg == null)
		{
			AsHudDlgMgr.Instance.OpenPStore();
			AsHudDlgMgr.Instance.pstoreDlg.SetOtherPStoreItem(_list);
		}
	}
	public void Recv_Owner_ItemList(body1_SC_PRIVATESHOP_OWNER_ITEMLIST _list)
	{
		m_StoreState = ePStoreState.User_Opening;
		
		//must be added
//		m_PStoreItemUsedSlot = _list.
	
		m_dicPlayerShopItem.Clear();
		foreach(body2_SC_PRIVATESHOP_OWNER_ITEMLIST item in _list.body)
		{
			m_dicPlayerShopItem.Add(item.nPrivateShopSlot, item);
			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(item.nInvenSlot, true);
		}
	
		Debug.Log("AsPStoreManager::Recv_Owner_ItemList: state = " + m_StoreState);
		bool shop = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<bool>(eComponentProperty.SHOP_OPENING);
		if(shop == false)
			Debug.LogError("AsPStoreManager::Recv_Owner_ItemList: shop opening data is not set, but Owner_ItemList is send. check server routine.");
		
		if( AsHudDlgMgr.Instance.IsOpenInven )
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		
//		if(AsHudDlgMgr.Instance.IsOpenPStore == true)
//		{
//			LockRegisteredShopItem();
//		}
	}
	public void Recv_ItemBuy(body_SC_PRIVATESHOP_ITEMBUY _buy)
	{
		switch(_buy.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			if(_buy.nPrivateShopUID == m_CurShopUId && AsHudDlgMgr.Instance.IsOpenPStore == true)
				AsHudDlgMgr.Instance.pstoreDlg.ItemSold(_buy);

			if( m_SearchDlg != null)
				m_SearchDlg.PurchaseComplete( _buy);
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_NOTITEM:
			AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(329), AsNotify.MSG_BOX_TYPE.MBT_OK);
			if( m_SearchDlg != null)
				m_SearchDlg.RequestSearch();
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_LESS:
			AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(329), AsNotify.MSG_BOX_TYPE.MBT_OK);
			if( m_SearchDlg != null)
				m_SearchDlg.RequestSearch();
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_NOTENTER:
			AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(329), AsNotify.MSG_BOX_TYPE.MBT_OK);
			if( m_SearchDlg != null)
				m_SearchDlg.RequestSearch();
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_NOTGOLD:
			AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(28), AsNotify.MSG_BOX_TYPE.MBT_OK);
			break;
		case eRESULTCODE.eRESULT_FAIL_PRIVATESHOP_INVENTORYFULL:
			AsNotify.Instance.MessageBox("", AsTableManager.Instance.GetTbl_String(1399), AsNotify.MSG_BOX_TYPE.MBT_OK);
			break;
		}
		
		Debug.Log("AsPStoreManager::Recv_ItemBuy: " + _buy.eResult);
	}
	public void Recv_Owner_ItemBuy(body_SC_PRIVATESHOP_OWNER_ITEMBUY _buy)
	{
		if(AsHudDlgMgr.Instance.IsOpenPStore == true)
			AsHudDlgMgr.Instance.pstoreDlg.ItemSold(_buy);
		
		if(AsHudDlgMgr.Instance.IsOpenInven == true)
			AsHudDlgMgr.Instance.invenDlg.ItemSoldByPStore(_buy);
		
		foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
		{
			if(pair.Value.nPrivateShopSlot == _buy.nPrivateShopSlot)
			{
				InvenSlot invenSlot = ItemMgr.HadItemManagement.Inven.GetInvenSlotItem( pair.Value.nInvenSlot );
				if( null != invenSlot)
				{
//					string name = AsTableManager.Instance.GetTbl_String(invenSlot.realItem.item.ItemData.nameId);
//					string str = string.Format(AsTableManager.Instance.GetTbl_String(200), name, _buy.nItemCount, (int)_buy.nItemCount * (int)item.nItemGold );
//					AsChatManager.Instance.InsertChat(str,eCHATTYPE.eCHATTYPE_SYSTEM);
					
					string name = AsTableManager.Instance.GetTbl_String(invenSlot.realItem.item.ItemData.nameId);
					double earn = (double)_buy.nItemCount * (double)pair.Value.nItemGold;
//					int commission = (int)(earn * AsTableManager.Instance.GetTbl_GlobalWeight_Record(14).Value * 0.01f + 0.5f);
//					string header = AsTableManager.Instance.GetTbl_String(126);
//					string body = string.Format(AsTableManager.Instance.GetTbl_String(214), name, _buy.nItemCount, earn -  commission, commission);
//					AsNotify.Instance.MessageBox(header, body);
					
					double commission = (earn * AsTableManager.Instance.GetTbl_GlobalWeight_Record(14).Value * 0.01d + 0.5d);
					
//					int nCurrentChannel = AsUserInfo.Instance.currentChannel;
//					body2_GC_CHANNEL_LIST channel = AsChannelListData.Instance.GetDataByChannelIndex( nCurrentChannel);
//					
//					if( null != channel)
//					{
//						if( true == channel.bIsPrivateShop)
//							commission = (earn * (channel.nPrivateStore_Charge - channel.nPrivateStore_ChargeReduction) * 0.01d + 0.5d);
//						else
//							commission = (earn * channel.nPrivateStore_Charge * 0.01d + 0.5d);
//					}
//					else
//						Debug.LogError("AsPStoreManager::Recv_Owner_ItemBuy: current channel is not found. nCurrentChannel = " + nCurrentChannel);
					
					commission = (earn * _buy.nPrivateStore_Charge * 0.01d + 0.5d);
					
					string header = AsTableManager.Instance.GetTbl_String(126);
					string body = string.Format(AsTableManager.Instance.GetTbl_String(214), name, _buy.nItemCount, (ulong)earn -  (ulong)commission, (ulong)commission);
					AsNotify.Instance.MessageBox(header, body);
				}
				
				pair.Value.sItem.nOverlapped -= _buy.nItemCount;
				
				if(pair.Value.sItem.nOverlapped < 1)
				{
//					AsHudDlgMgr.Instance.invenDlg.SetSlotMoveLock(pair.Value.nInvenSlot, false);
					ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(pair.Value.nInvenSlot, false);
					if(AsHudDlgMgr.Instance.IsOpenInven == true)
						AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
					
					m_dicPlayerShopItem.Remove(pair.Key);
				}
								
				break;
			}
		}
		
		Debug.Log("AsPStoreManager::Recv_Owner_ItemBuy: m_dicPlayerShopItem.Count == " + m_dicPlayerShopItem.Count);
		if(m_dicPlayerShopItem.Count == 0)
		{
			Debug.Log("AsPStoreManager::Recv_Owner_ItemBuy: m_dicPlayerShopItem.Count == 0");
			
			string str = AsTableManager.Instance.GetTbl_String(201);
			AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
			
			m_StoreState = ePStoreState.Closed;
			AsHudDlgMgr.Instance.ClosePStore();
		}
		
		Debug.Log("AsPStoreManager::Recv_Owner_ItemBuy: " + _buy.eResult);
	}
	public void Recv_Search( body1_SC_PRIVATESHOP_SEARCH_RESULT _result)
	{
		Debug.Log("AsPStoreManager::Recv_Search: " + _result);
		
		if( m_SearchDlg != null)
		{
			m_SearchDlg.Recv_Search( _result);
		}
	}
	#endregion
	#region - public -
	public void PStoreClosedByOtherSelectedCharacter()
	{
		m_dicPlayerShopItem.Clear();
	}
	
	public int GetFirstFilledSlotIndex()
	{
		int curIdx = -1;
		for(int i=0; i< AsHudDlgMgr.Instance.pstoreDlg.slotCount; ++i)
		{
			if(m_dicPlayerShopItem.ContainsKey(i) == true &&
				m_dicPlayerShopItem[i].sItem.nOverlapped > 0)
			{
				curIdx = i;
				break;
			}
		}
		
		return curIdx;
	}
	
	public int GetProperBlankSlotIndex()
	{
		int curIdx = 0;
		
		for(curIdx=0; curIdx< AsHudDlgMgr.Instance.pstoreDlg.slotCount; ++curIdx)
		{
			if(m_dicPlayerShopItem.ContainsKey(curIdx) == false)
				break;
			else if(m_dicPlayerShopItem[curIdx].sItem.nOverlapped < 1)
				break;
		}
		
		if(curIdx >= (int)m_PStoreEnableSlot)
			return -1;
		else
			return curIdx;
	}
	
	public void ItemUsed(UIInvenSlot _slot)
	{
		if(_slot == null)
		{
			Debug.LogError("AsPStoreManager::ItemUsed: _slot = null");
			return;
		}
		
		ItemUsed(_slot.slotItem.realItem);
	}
	
	public void ItemUsed(RealItem _item)
	{
		if(_item == null)
		{
			Debug.LogError("AsPStoreManager::ItemUsed: _item = null");
			return;
		}
		
		//dopamin
		if(AsPartyManager.Instance.IsPartyNotice)
		{		
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(AsTableManager.Instance.GetTbl_String(1728));
			return;
		}
		
		//product
		if(AsUserInfo.Instance.isProductionProgress == true)
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(379),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}
		
		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
			return;

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
			return;

		Map map = TerrainMgr.Instance.GetCurrentMap();
		eMAP_TYPE mapType = map.MapData.getMapType;
		switch(mapType)
		{
		case eMAP_TYPE.Field:
		case eMAP_TYPE.Tutorial:
		case eMAP_TYPE.Indun:
		case eMAP_TYPE.Pvp:
		case eMAP_TYPE.Raid:
		case eMAP_TYPE.Summon:
//			string header = AsTableManager.Instance.GetTbl_String(126);
			string str = AsTableManager.Instance.GetTbl_String(294);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
//			AsNotify.Instance.MessageBox(header, str);
//			AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
//			Debug.LogWarning("ItemUsed: shop can not be opened out of town.");
			return;
		case eMAP_TYPE.Town:
			break;
		}
		
//		AsChatManager.Instance.InsertChat("Private shop is under construction.", eCHATTYPE.eCHATTYPE_SYSTEM, true);
		
		if(AsCommonSender.isSendWarp == true)
		{
			Debug.Log("AsPStoreManager::ItemUsed: AsCommonSender.isSendWarp == true. ignore private shop.");
			return;
		}
		
		if(CheckPStoreOpenItem(_item.sItem.nItemTableIdx) == false)
		{
			Debug.LogWarning("AsPStoreManager::ItemUsed: CheckPStoreOpenItem(_item.sItem.nItemTableIdx) == false. ignore private shop.");
			return;
		}
		
//		AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_MoveStopIndication());
		
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if(user.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == false)
		{
			AsNpcEntity npc = null;
			float minNpc = AsEntityManager.Instance.GetNearestNpcForPStore(user.transform.position, out npc);
			s_ShopOpenPossibleDistance = AsTableManager.Instance.GetTbl_GlobalWeight_Record(18).Value;
			
			if(minNpc < s_ShopOpenPossibleDistance)
			{
//				Debug.Log("AsPStoreManager::ItemUsed: npc is too close. find proper place.");
//				string header = AsTableManager.Instance.GetTbl_String(126);
				string str = AsTableManager.Instance.GetTbl_String(286);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
//				AsNotify.Instance.MessageBox(header, str);
//				AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			}
			
			AsUserEntity shop = null;
			float minShop = AsEntityManager.Instance.GetNearestPrivateShop(user.transform.position, out shop);
			float possibleDistance_Pc = AsTableManager.Instance.GetTbl_GlobalWeight_Record(17).Value;
			
			if(minShop < possibleDistance_Pc)
			{
//				Debug.Log("AsPStoreManager::ItemUsed: private shop is too close. find proper place.");
//				string header = AsTableManager.Instance.GetTbl_String(126);
				string str = AsTableManager.Instance.GetTbl_String(377);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
//				AsNotify.Instance.MessageBox(header, str);
//				AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			}
			
			float minPortal = AsPortalTrigger.ClosestPortalDistance();
			float possibleDistance_Portal = AsTableManager.Instance.GetTbl_GlobalWeight_Record(68).Value;
			
			if(minPortal < possibleDistance_Portal)
			{
//				Debug.Log("AsPStoreManager::ItemUsed: portal is too close. find proper place.");
//				string header = AsTableManager.Instance.GetTbl_String(126);
				string str = AsTableManager.Instance.GetTbl_String(378);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
//				AsNotify.Instance.MessageBox(header, str);
//				AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
				return;
			}
			
			m_PStoreItemUsedSlot = _item;
			
			_item.SendUseItem();
		}
		else
		{
			m_PStoreItemUsedSlot = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot(
				AsUserInfo.Instance.nPrivateShopCreateItemSlot);
			
			if(m_PStoreItemUsedSlot != null &&
				m_PStoreItemUsedSlot.sItem.nItemTableIdx == _item.sItem.nItemTableIdx)
			{
				m_StoreState = ePStoreState.User_Opening;
				
				Debug.LogWarning("Bug Searching::: AsPStoreManager:: ItemUsed: private shop dialog is instantiated");
				AsHudDlgMgr.Instance.OpenPStore();
			}
		}
	}			
	
	public void ItemUsedResult(body_SC_ITEM_RESULT _item)
	{
		//*	
		if(SetPStoreOpenItem(_item.nItemTableIdx) == true)
		{
			Debug.Log("AsPStoreManager::ItemUsedResult: pstore item used[" + m_PStoreEnableSlot + "(" + _item.nItemTableIdx + ")]");
			Request_Create();
		}// dopamin70 */
	}
	
	public void PlayerLoaded()
	{
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
			return;
		
		Debug.Log("AsPStoreManager::PlayerLoaded: state = " + m_StoreState);
	}
	
	public void SetContentText(string _str)
	{
		m_strContent = _str;
		
		Debug.Log("AsPStoreManager::SetContentText: m_strContent = " + m_strContent);
	}
	
	public void SetPrivateShopInfo(body1_GC_CHAR_LOAD_RESULT _result)
	{
		m_RemainTime = (float)_result.nPrivateShopRemainingTime;
	}
	
	//private RealItem m_PStoreUseRealItem = null;
	public bool IsCheckPStorUseItem( RealItem _realItem )
	{
		if( null == _realItem )
			return false;
		
//		if( false == AsUserInfo.Instance.isProductionProgress )
//			return false;
		
		//if( false == CheckPStoreOpenItem( _realItem.item.ItemID ) )		
		//	return false;

        if ( Item.eITEM_TYPE.UseItem == _realItem.item.ItemData.GetItemType() )
        {
            Item.eUSE_ITEM _useitem = (Item.eUSE_ITEM)_realItem.item.ItemData.GetSubType();

            if (Item.eUSE_ITEM.PrivateStore1 == _useitem || Item.eUSE_ITEM.PrivateStore2 == _useitem ||
                Item.eUSE_ITEM.PrivateStore3 == _useitem || Item.eUSE_ITEM.PrivateStore4 == _useitem ||
                Item.eUSE_ITEM.PrivateStore5 == _useitem)
            {
				Map map = TerrainMgr.Instance.GetCurrentMap();
				eMAP_TYPE mapType = map.MapData.getMapType;
				switch(mapType)
				{
				case eMAP_TYPE.Field:
				case eMAP_TYPE.Tutorial:
				case eMAP_TYPE.Indun:
				case eMAP_TYPE.Pvp:
				case eMAP_TYPE.Raid:
				case eMAP_TYPE.Summon:
		//			string header = AsTableManager.Instance.GetTbl_String(126);
					string str = AsTableManager.Instance.GetTbl_String(294);
					AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
		//			AsNotify.Instance.MessageBox(header, str);
		//			AsChatManager.Instance.InsertChat(str, eCHATTYPE.eCHATTYPE_SYSTEM);
		//			Debug.LogWarning("ItemUsed: shop can not be opened out of town.");
					return true;
				case eMAP_TYPE.Town:
					break;
				}
				
                return false;
            }
            else
            {
				
            }
        }
		
		
//		AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(379),
//									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
		
		
		//m_PStoreUseRealItem = _realItem;
		//AsHudDlgMgr.Instance.SetMsgBox( 
		//	AsNotify.Instance.MessageBox( 	string.Empty, 
		//									AsTableManager.Instance.GetTbl_String(379), 
		//									this, 
		//									"OpenPStoreUseItem", 
		//									AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) );
		
		return false;
	}
	
	/*public void OpenPStoreUseItem()
	{
		if( null == m_PStoreUseRealItem )
			return;
		
		if( false == AsUserInfo.Instance.isProductionProgress )
			return;
		
		AsCommonSender.SendItemProductProgress(false);
		m_PStoreUseRealItem.SendUseItem();
	}*/
	
//	public void ApplicationBG()
//	{
//		switch(m_StoreState)
//		{
//		case ePStoreState.User_Opening:
//			m_PausedTime = System.DateTime.Now.Ticks / 100000000f;
//			Debug.LogWarning("AsPStoreManager::ApplicationBG: m_PausedTime = " + m_PausedTime);
////				m_PausedTime = Time.realtimeSinceStartup;
//			AsHudDlgMgr.Instance.ClosePStore();
//			break;
//		}
//	}
//	
//	public void ApplicationRecover()
//	{
//		switch(m_StoreState)
//		{
//		case ePStoreState.User_Opening:
//			m_SecondPausedTime = System.DateTime.Now.Ticks / 100000000f;
//			m_RevisionTime = m_SecondPausedTime - m_PausedTime;
//			Debug.LogWarning("AsPStoreManager::ApplicationRecover: m_SecondPausedTime = " + m_SecondPausedTime);
//			Debug.LogWarning("AsPStoreManager::ApplicationRecover: m_RevisionTime = " + m_RevisionTime);
////				float pausedTime = Time.realtimeSinceStartup - m_PausedTime;
//			m_RemainTime -= m_RevisionTime;
//			StartCoroutine(TimeShowing());
//			break;
//		}
//	}
	
	public void ApplicationPause_GameMain(bool _pause)
	{
		if(_pause == true)
		{
			switch(m_StoreState)
			{
			case ePStoreState.Another_Opened:
				AsHudDlgMgr.Instance.ClosePStore();
				break;
			}
		}
	}
	
	public IEnumerator RequestShopModeling(AsBaseEntity _entity)
	{
		_entity.SetModelObject(null);
		
		while(true)
		{
			if(m_ShopModelingLoaded == true)
			{
				Debug.Log("AsPStoreManager::RequestShopModeling: m_ShopModelingLoaded = " + m_ShopModelingLoaded + ". copy shop object.");
				_entity.SetModelObject(Instantiate(m_OriginShopModeling) as GameObject);
				break;
			}
			
			yield return null;
		}
	}
	
	public void GameReset()
	{
		m_StoreState = ePStoreState.Closed;
		m_PStoreEnableSlot = ePStoreEnableSlot.NONE;
		m_PStoreItemUsedSlot = null;
		
		m_strTitle = "";
		m_strContent = "";
		
		m_dicPlayerShopItem.Clear();
		
		m_CurShopUId = 0;
		m_RemainTime = 0;
	}
	
	public void OpenFoldedUserPrivateShop()
	{
		m_StoreState = ePStoreState.User_Opening;
		AsHudDlgMgr.Instance.OpenPStore();
	}
	
	public void OpenChannelDlgInCharacterSelect()
	{
		string title = AsTableManager.Instance.GetTbl_String(126);
		string content = AsTableManager.Instance.GetTbl_String(1991);
		string okBtn = AsTableManager.Instance.GetTbl_String(1990);
		string cancelBtn = AsTableManager.Instance.GetTbl_String(1151);
		
		AsNotify.Instance.MessageBox( title, content,
			okBtn, cancelBtn,
			this, "FinishPrivateShopInCharacterSelect",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	void FinishPrivateShopInCharacterSelect()
	{
		body_CS_PRIVATESHOP_DESTROY destroy = new body_CS_PRIVATESHOP_DESTROY();
		AsCommonSender.Send(destroy.ClassToPacketBytes());
		
		AsCharacterSlotManager.CharacterSelected = false;
	}
	void OnMessageBoxNotify()
	{
		AsCharacterSlotManager.CharacterSelected = false;
	}
	
	public void ExpireProcess(body1_SC_ITEM_TIME_EXPIRE _expire)
	{
		if( null == _expire || null == _expire.datas)		
			return;				
		
		if( null == m_PStoreItemUsedSlot || null == m_PStoreItemUsedSlot.sItem )
			return;		
		
		foreach(sITEM item in _expire.datas)
		{
			
			if(m_PStoreItemUsedSlot.sItem.nItemTableIdx == item.nItemTableIdx)
			{
				switch(m_StoreState)
				{
				case ePStoreState.User_Standby:
					m_PStoreItemUsedSlot = null;
					AsHudDlgMgr.Instance.ClosePStore();
					return;
//					break;
				}
			}
		}
	}
	
	public void OpenPStoreSearch()
	{
		if( TerrainMgr.Instance.IsCurMapType( eMAP_TYPE.Town) == false)
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2185));
			return;
		}
		
		if( m_StoreState == ePStoreState.Closed)
		{
			GameObject obj = Instantiate(Resources.Load("UI/AsGUI/GUI_SearchStore")) as GameObject;
			m_SearchDlg = obj.GetComponentInChildren<UIPStoreSearchDlg>();
			if( m_SearchDlg == null)
				Debug.LogError(" AsPStoreManager:: OpenPStoreSearch: UIPStoreSearchDlg is not found");
		}
		else
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126),
				AsTableManager.Instance.GetTbl_String(365), AsNotify.MSG_BOX_TYPE.MBT_OK);
	}
	
	public void ClosePStoreSearch()
	{
		if( m_SearchDlg != null)
			Destroy( m_SearchDlg.gameObject);
//			Destroy( m_SearchDlg.transform.parent.gameObject);
	}
	#endregion
	#region - private -
	bool CheckPStoreOpenItem(int _tableIdx)
	{
		return true;
		
//		switch(_tableIdx)
//		{
//		case 160020:
//		case 160021:
//		case 160022:
//		case 160023:
//		case 160024:
//			return true;
//		default:
//			return false;
//		}
	}
	
	bool SetPStoreOpenItem(int _tableIdx)
	{
		Item data = ItemMgr.ItemManagement.GetItem(_tableIdx);
		if(data.ItemData.GetItemType() != Item.eITEM_TYPE.UseItem)
			return false;
		
		int itemKind = data.ItemData.GetSubType();
		
		bool pstoreItemUsed = true;
		m_PStoreEnableSlot = ePStoreEnableSlot.NONE;
		switch((Item.eUSE_ITEM)itemKind)
		{
		case Item.eUSE_ITEM.PrivateStore1:
			m_PStoreEnableSlot = ePStoreEnableSlot.Ticket_1;
			break;
		case Item.eUSE_ITEM.PrivateStore2:
			m_PStoreEnableSlot = ePStoreEnableSlot.Ticket_2;
			break;
		case Item.eUSE_ITEM.PrivateStore3:
			m_PStoreEnableSlot = ePStoreEnableSlot.Ticket_3;
			break;
		case Item.eUSE_ITEM.PrivateStore4:
			m_PStoreEnableSlot = ePStoreEnableSlot.Ticket_4;
			break;
		case Item.eUSE_ITEM.PrivateStore5:
			m_PStoreEnableSlot = ePStoreEnableSlot.Ticket_5;
			break;
		default:
			pstoreItemUsed = false;
			Debug.Log("AsPStoreManager::SetPStoreOpenItem: item is not found.[_tableIdx = " + _tableIdx + "]");
			break;
		}
		
		return pstoreItemUsed;
	}
	
	void SetContent(byte[] _ch)
	{		
		m_strContent = System.Text.UTF8Encoding.UTF8.GetString(_ch);

        int idx = m_strContent.IndexOf('\0');

        m_strContent = m_strContent.Substring(0, idx);
		
		Debug.Log("AsPStoreManager::SetContent: m_strContent = " + m_strContent);
		Debug.Log("AsPStoreManager::SetContent: m_strContent.Length = " + m_strContent.Length);
	}
	
	void LockRegisteredShopItem()
	{
//		foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
//		{
//			ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(pair.Value.nInvenSlot, true);
//		}
		
		if( AsHudDlgMgr.Instance.IsOpenInven )
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
	}
	
	void ClearRegisteredShopItem()
	{
		if(AsHudDlgMgr.Instance != null && AsHudDlgMgr.Instance.invenDlg != null)
		{
			foreach(KeyValuePair<int, body2_SC_PRIVATESHOP_OWNER_ITEMLIST> pair in m_dicPlayerShopItem)
			{	
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(pair.Value.nInvenSlot, false);
			}
			
			m_PStoreItemUsedSlot = null;
			
			m_dicPlayerShopItem.Clear();
			if( AsHudDlgMgr.Instance.IsOpenInven )
				AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		}
	}
	
//	bool m_Closing = false;
//	[SerializeField] float m_ClosingTime = 1f;
//	IEnumerator WaitForClosing()
//	{
//		m_Closing = true;
//		
//		yield return new WaitForSeconds(m_ClosingTime);
//		
//		m_Closing = false;
//	}
	#endregion
	#region - getter -
	public bool UnableActionByPStore()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if(user != null)
		{
			bool shopOpening = user.GetProperty<bool>(eComponentProperty.SHOP_OPENING);
//			Debug.Log("AsPStoreManager::UnableActionByPStore: current state = " + m_StoreState + ", user shop opening = " + shopOpening);
			
			return m_StoreState != ePStoreState.Closed || shopOpening == true;
		}
		else
		{
			Debug.Log("AsPStoreManager::UnableActionByPStore: user character is not exist.");
			return false;
		}
	}
	#endregion
	
//	void OnGUI()
//	{
//		if(m_TimeShow == true)
//		{
//			GUI.Label(new Rect(200, 100, 200, 80), "m_PausedTime = " + m_PausedTime + "\n" +
//				"m_SecondPausedTime = " + m_SecondPausedTime + "\n" +
//				"m_RevisionTime = " + m_RevisionTime);
//		}
//	}
}
	
