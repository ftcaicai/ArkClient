using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

public enum eCashStoreMenuMode
{
	MAIN,
	WEAPON,
	EQUIPMENT,
	COSTUME,
	PET,
	EVENT,
	CONVENIENCE,
	FREE,
	CHARGE_MIRACLE,
	NONE,
}

public class AsCashStore : AsStore
{
	public static AsCashStore Instance { get { return ms_kIstance; } }
	public CashStoreMenu	menuMain;
	public CashStoreMenu	menuMiracle;
	public GameObject[]	    menuObjects;
	public UIRadioBtn[]		menuButtons;
	public int[]			menuNameIDs;
	public UIButton			btnCoupon;
	public UIButton			btnMiracleBuy;
	public UIButton			btnPurchasePolicyInfo;
	public UIButton			btnOpenMiracleInfo;
	public MiracleInfoPopup miracleInfoPopup;
	public CashstoreLoadingIndigator cashstoreLoadingIndigator;
	public SpriteText txtMiracle;
	public SpriteText txtGold;
	public SpriteText txtTitle;
	public GameObject[] objOpenGachaPopupPrefabs;
	public GameObject objFreeGachaMark = null;
	public int titleNameID;
	public int couponNameID;

	private static AsCashStore	ms_kIstance			= null;
	private StoreItemInfoUI		prevStoreItemInfo	= null;
	private FindGiftFriendPopup findGiftFriendPopup	= null;
	private AskSendGiftMsgBox	askSendGiftMsgBox	= null;
	private eCashStoreMenuMode	nowMenuMode			= eCashStoreMenuMode.MAIN;

	// after get miracle info from google or apple server
	private eCashStoreMenuMode afterGotoMenu = eCashStoreMenuMode.MAIN;
	private eCashStoreSubCategory afterSelectCategory = eCashStoreSubCategory.NONE;
	private int afterSelectIdx = 0;

	private uint	foundFriendUniqueID = 0;
	private int		nUseMiracle = 0;
	private float   fPassedTime = 0.0f;

	private Dictionary<eCashStoreMenuMode, List<CashStoreMenu>>	dicMenuList		= new Dictionary<eCashStoreMenuMode,List<CashStoreMenu>>();
	private Dictionary<eCashStoreMenuMode, CashStoreMenu>		dicMainMenuItem	= new Dictionary<eCashStoreMenuMode, CashStoreMenu>();
	private Dictionary<eCashStoreMenuMode, UIRadioBtn>			dicMenuBtn		= new Dictionary<eCashStoreMenuMode,UIRadioBtn>();
	private List<eCashStoreMenuMode>							listMenuMode	= new List<eCashStoreMenuMode>();
	private CashStoreMenu nowCashStoreMenu = null;

	public System.Int64 nMiracle = 0;
	public System.Int64 nFreeMiracle = 0;

	private byte nFreeGachaPoint = 0;
	public byte FreeGachaPoint
	{
		get { return nFreeGachaPoint; }
		set
		{
			nFreeGachaPoint = value;

			if (objFreeGachaMark != null)
				objFreeGachaMark.SetActive(nFreeGachaPoint == 1);
		}
	}

	[HideInInspector]
	public Camera uiCamera = null;

	void Awake() 
	{ 
		ms_kIstance = this; 
	}

	void OnDestory()
	{
		ms_kIstance = null;
	}

	public override void InitilizeStore( int _npcId, eCLASS _eClass)
	{
		Debug.Log(WemeSdkManager.Instance.IsConfirmGuest);
//		if(WemeSdkManager.Instance.IsConfirmGuest)
//			btnCoupon.SetControlState(UIButton.CONTROL_STATE.DISABLED);
 
		nowNpcID		= _npcId;
		nowUserClass	= _eClass;
		nowClass		= _eClass;

		if( btnCoupon != null)
			btnCoupon.AddInputDelegate( InputCupon);

		if (btnMiracleBuy != null)
			btnMiracleBuy.AddInputDelegate( InputMiracleBuy);

		LockInput( true);

		ShowLoadingIndigator();

		InitMenu();

		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(79);

		if (record != null)
			nUseMiracle = (int)record.Value;

		if (nUseMiracle == 1)
			InitPurchase();

		PrepareBuyItemEvent = PrepareBuyItem;
		BuyItemEvent = BuyItem;

		Initilize();

		if( txtTitle != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( txtTitle);
			txtTitle.Text = AsTableManager.Instance.GetTbl_String(titleNameID);
		}

		if( btnCoupon != null && btnCoupon.spriteText != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(btnCoupon.spriteText);
			btnCoupon.spriteText.Text = AsTableManager.Instance.GetTbl_String(couponNameID);
		}

		if (btnMiracleBuy != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(btnMiracleBuy.spriteText);
			btnMiracleBuy.Text = AsTableManager.Instance.GetTbl_String(2727);
		}

		if (btnPurchasePolicyInfo != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(btnPurchasePolicyInfo.spriteText);
			btnPurchasePolicyInfo.spriteText.Text = AsTableManager.Instance.GetTbl_String(2224);
			btnPurchasePolicyInfo.SetInputDelegate(ProcessPurchasePolicyInfo);
		}

		if (btnOpenMiracleInfo != null)
			btnOpenMiracleInfo.SetInputDelegate(ProcessMiracleInfo);

		if (txtGold != null)
			txtGold.Text = AsUserInfo.Instance.SavedCharStat.nGold.ToString( "#,#0", CultureInfo.InvariantCulture);

		UpdateMiracleText();
		UpdateGoldText();

		if (purchaseManager == null)
		{
			nowState = StoreState.NOTOUCH;
			if (afterGotoMenu != eCashStoreMenuMode.NONE)
				ActivateItem(afterGotoMenu, afterSelectCategory, afterSelectIdx);
		}
		else
		{
			//nowState = StoreState.NOTOUCH;
			GoToMenu(eCashStoreMenuMode.MAIN, 0, false, true);
		}


		if( AsReviewConditionManager.Instance.IsReviewCondition(eREVIEW_CONDITION.COUPON) == false )
		{
			btnCoupon.gameObject.SetActive(false);
		}
	}

	public void InitilizeAfterAdLoad()
	{

	}

	void Start()
	{
		if( CameraMgr.Instance != null)
			uiCamera = CameraMgr.Instance.UICamera;
	}

	public void InitMenu()
	{
		if (menuObjects.Length != menuButtons.Length)
		{
			Debug.LogError("menuObjects.Length != menuButtons.Length");
			return;
		}

		foreach (UIRadioBtn button in menuButtons)
		{
			if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME && button != null)
				button.SetInputDelegate(MenuButtonInput);

			if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
				button.controlIsEnabled = false;
		}

		for (int i = 0; i < menuButtons.Length; i++)
		{
			// set count
			menuButtons[i].Data = i;

			// set menu string
			AsLanguageManager.Instance.SetFontFromSystemLanguage(menuButtons[i].spriteText);
			menuButtons[i].Text = AsTableManager.Instance.GetTbl_String(menuNameIDs[i]);

			Component[]			components	= menuObjects[i].GetComponents(typeof(CashStoreMenu));
			List<CashStoreMenu>	listMenu	= new List<CashStoreMenu>();
			eCashStoreMenuMode	mainMode	= eCashStoreMenuMode.NONE;
			CashStoreMenu		mainMenu	= null;

			foreach (CashStoreMenu menu in components)
			{
				if (menu.isMainMenu == true)
				{
					mainMode = menu.menuMode;

					mainMenu = menu;

					// make menumode list
					if (!listMenuMode.Contains(menu.menuMode))
						listMenuMode.Add(menu.menuMode);
				}

				listMenu.Add(menu);
			}

			// make dictionary
			dicMainMenuItem.Add(mainMode, mainMenu);
			dicMenuList.Add(mainMode, listMenu);
			dicMenuBtn.Add(mainMode, menuButtons[i]);

			// change txt color for character select
			if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
			{
				menuButtons[i].spriteText.Color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
			}
		}

		// for main menu
		List<CashStoreMenu> listMenuForMain = new List<CashStoreMenu>();
		listMenuForMain.Add(menuMain);
		dicMainMenuItem.Add(eCashStoreMenuMode.MAIN, menuMain);
		dicMenuList.Add(eCashStoreMenuMode.MAIN, listMenuForMain);

		// for miracle menu
		List<CashStoreMenu> listMenuForMiracle = new List<CashStoreMenu>();
		Component[] miracleComponents = menuMiracle.GetComponents(typeof(CashStoreMenu));
		foreach (CashStoreMenu menu in miracleComponents)
			listMenuForMiracle.Add(menu);
		dicMainMenuItem.Add(eCashStoreMenuMode.CHARGE_MIRACLE, menuMiracle);
		dicMenuList.Add(eCashStoreMenuMode.CHARGE_MIRACLE, listMenuForMiracle);

		// off menu object
		foreach (GameObject menuObject in menuObjects)
			menuObject.SetActive(false);


		// set input delegate only main
		foreach (List<CashStoreMenu> list in dicMenuList.Values)
		{
			foreach (CashStoreMenu menu in list)
			{
				menu.InitText();

				if (menu.isMainMenu != true)
					continue;

				if (menu.mainScrollList != null)
					menu.mainScrollList.SetInputDelegate(InputDelegate);

				menu.addItemToList = AddStoreItems;
			}
		}
	}

	public void InitPurchase()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer)
			purchaseManager = gameObject.AddComponent<IosPurchaseManager>();

		if( purchaseManager != null)
		{

			if( purchaseManager.InitPurchase())
			{
				Debug.Log( "InitPurchase success");
	
				#if UNITY_IPHONE
				InitSuccessListener();
				#endif
			}
			else
			{
				#if UNITY_IPHONE
				MessageListener( "InitPurchase fail");
				#endif
			}

			purchaseManager.ProductInfoListener = AddStoreItems;
			purchaseManager.MessageListener = MessageListener;
			purchaseManager.InitSuccessPurcahseListender = InitSuccessListener;
			purchaseManager.ProcessAfterGetProduct = ProcessAfterReceiveProductInfo;
			purchaseManager.SuccessPurchaseListener = SuccessPurchase;
			purchaseManager.FailPurchase = FailPurchase;
			purchaseManager.CloseCashStore = Close;

		}
		else
		{
			Debug.Log( "purchaseManager is null");
		}
	}

	void Update()
	{
		ProcessNotTouch();

		ProcessUpClick();

		ProcessTouchInput();

		UpdateRemainTime();
	}

	void UpdateRemainTime()
	{
		if (dicChargeItemInfo == null)
			return;

		if (dicChargeItemInfo.Count <= 0)
			return;

		// check time
		if (fPassedTime >= 1.0f)
		{
			fPassedTime = 0.0f;
		}
		else
		{
			fPassedTime += Time.deltaTime;
			return;
		}
			
		foreach (KeyValuePair<string,body2_SC_CHARGE_ITEMLIST> pair in dicChargeItemInfo)
		{
			if (pair.Value.eChargeType != eCHARGETYPE.eCHARGETYPE_DAILY)
				continue;

			if (pair.Value.nRemainTime > 0)
				pair.Value.nRemainTime--;
		}
	}

	public void CashItemBuySuccessForJpn(body_SC_CASHSHOP_JAP_ITEMBUY_RESULT _data)
	{
		if (prevStoreItemInfo == null)
			return;

		nFreeMiracle	= _data.nFreeCash;
		nMiracle		= _data.nCash;
		nFreeGachaPoint = _data.nGachaPoint;
		AsUserInfo.Instance.FreeGachaPoint = _data.nGachaPoint;

		if (prevStoreItemInfo.item_Type == Store_Item_Type.GachaItem)
		{

			Item boughtItem = ItemMgr.ItemManagement.GetItem(int.Parse(prevStoreItemInfo.itemID));

			// collect not 0 id
			List<int> listSelected = new List<int>();
            List<byte> listStrengthen = new List<byte>();

			for(int i= 0 ; i < _data.nOpenGachaItemTable.Length ; i++)
            {
                if (_data.nOpenGachaItemTable[i] != 0)
                {
                    listSelected.Add(_data.nOpenGachaItemTable[i]);
                    listStrengthen.Add(_data.nOpenGachaItemStrengTable[i]);
                }
            }

			List<int> listUseItem = new List<int>();

			for (int i = 0; i < listSelected.Count; i++)
				listUseItem.Add(int.Parse(prevStoreItemInfo.itemID));

			GameObject gachaPrefab = null;

			// select gacha popup prefab
			if (prevStoreItemInfo.itemBunch == 1)
				gachaPrefab = objOpenGachaPopupPrefabs[0];
			else if (prevStoreItemInfo.itemBunch == 5)
				gachaPrefab = objOpenGachaPopupPrefabs[1];
			else if (prevStoreItemInfo.itemBunch == 11)
				gachaPrefab = objOpenGachaPopupPrefabs[2];

			if (gachaPrefab != null)
			{
				GameObject gachaPopup = GameObject.Instantiate(gachaPrefab) as GameObject;

				gachaPopup.transform.parent = transform;
				gachaPopup.transform.localPosition = new Vector3(0.0f, 0.0f, -15.0f);

				GachaOpenPopup gachaOpenPopup = gachaPopup.GetComponent<GachaOpenPopup>();

				if (gachaOpenPopup != null)
					gachaOpenPopup.Open(listUseItem, listSelected, listStrengthen, Cancel);
			}
		}
		else if (prevStoreItemInfo.itemName != string.Empty)
		{
			string message = string.Format(AsTableManager.Instance.GetTbl_String(376), prevStoreItemInfo.itemName);

			PlayItemSound(prevStoreItemInfo);

			AsMessageBox msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(1538), message, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.SetOkDelegate = Cancel;
		}

		UpdateFreeGachaPoint();
	}

	public void UpdateFreeGachaPoint()
	{
		if (nowCashStoreMenu != null)
		{
			nowCashStoreMenu.UpdateFreeGachaPoint();

			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.SetCashStoreBtnFreeMark(nFreeGachaPoint == 1);

			if (objFreeGachaMark != null)
				objFreeGachaMark.SetActive(nFreeGachaPoint == 1);
		}
	}

	public void CashItemBuySuccess()
	{
		if (prevStoreItemInfo == null)
			return;

		 if (prevStoreItemInfo.itemName != string.Empty)
		{
			string message = string.Format(AsTableManager.Instance.GetTbl_String(376), prevStoreItemInfo.itemName);

			PlayItemSound(prevStoreItemInfo);

			AsMessageBox msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(1538), message, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.SetOkDelegate = Cancel;
		}
	}

	public void MessageListener( string _msg)
	{
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), _msg, AsNotify.MSG_BOX_TYPE.MBT_OK);
	}

	public void FailQueryInventory(string _msg)
	{
		AsMessageBox msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), _msg, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
		msgBox.SetOkDelegate = Cancel;
	}

	void InitSuccessListener()
	{
		AsCommonSender.SendRequestCashInfoJpn();
	}

	public void ProcessAfterReceiveProductInfo()
	{
		if (afterGotoMenu != eCashStoreMenuMode.NONE)
			ActivateItem(afterGotoMenu, afterSelectCategory, afterSelectIdx);
		else
			nowState = StoreState.NOTOUCH;
	}

	public void SuccessPurchase( object _purchaseInfo)
	{
		HideLoadingIndigator();

		nowState = StoreState.NOTOUCH;

		string productID = string.Empty;


		#if UNITY_IPHONE
		ArkTransaction transaction = _purchaseInfo as ArkTransaction;
		productID = transaction.productID;
		#endif
		
		if (dicChargeItemInfo.ContainsKey(productID) == true)
		{
			Tbl_ChargeRecord chargeRecord = AsTableManager.Instance.GetChargeRecord(productID);

			if (chargeRecord != null)
				dicChargeItemInfo[productID].nRemainTime = chargeRecord.useTime;
			else
				Debug.LogWarning(productID + " chargeRecord is null");
		}

		AsPartyTrackManager.Instance.SetEvent(AsPartyTrackManager.ePartyTrackEvent.eFirst_Payment);

		if( productID.Contains( "jp.wemade.ark.id.15") || productID.Contains( "jp.wemade.ark.ad15"))
			AsPartyTrackManager.Instance.SetEvent( AsPartyTrackManager.ePartyTrackEvent.ePayment_Item_900);

		if (purchaseManager != null)
		{
			ProductInfoForPartyTracker info = purchaseManager.GetItemInfoForPartyTracker(productID);
			if (info != null)
				AsPartyTrackManager.Instance.SendPayment(info.itemName, 1, info.currencyCode, info.price);
		}
	}

	public void UpdateMiracleText()
	{
		if (txtMiracle != null)
			txtMiracle.Text = AsUserInfo.Instance.nMiracle.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void UpdateGoldText()
	{
		if (txtGold != null)
			txtGold.Text = AsUserInfo.Instance.SavedCharStat.nGold.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void FailPurchase()
	{
		HideLoadingIndigator();

		if (purchaseManager != null)
			purchaseManager.RequestItemUnRegister();

		AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(1539), AsTableManager.Instance.GetTbl_String(372), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		msgBox.SetOkDelegate = Cancel;
		msgBox.SetCancelDelegate = Cancel;

	}

	public void MenuButtonInput( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && IsLockInput == false)
		{
			if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
				GoToMenu(listMenuMode[(int)ptr.targetObj.Data], 0);
		}
	}

	public CashStoreMenu OnMenu(eCashStoreMenuMode _mode)
	{
		//if (nowMenu != null)
		//{
		//    if (nowMenu.mainScrollList != null)
		//        nowMenu.mainScrollList.ClearListSync(true);
		//}
		nowCashStoreMenu = menuMain;

		nowMenuMode = _mode;

		HideMiracleInfo();

		List<CashStoreMenu> listNowCashStoreMenu = dicMenuList[nowMenuMode];

		// clear menu
		foreach (CashStoreMenu cashStoreMenu in listNowCashStoreMenu)
			cashStoreMenu.Reset();

		if (nowMenuMode != eCashStoreMenuMode.COSTUME && AsHudDlgMgr.Instance != null)
		{
			if (null != AsHudDlgMgr.Instance.cashShopEntity)
			{
				AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
				AsHudDlgMgr.Instance.cashShopEntity = null;
			}
		}

		//if (nowMenuMode != eCashStoreMenuMode.PET && AsHudDlgMgr.Instance != null)
		//{
		//    if (null != AsHudDlgMgr.Instance.cashShopPetEntity)
		//    {
		//        AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopPetEntity);
		//        AsHudDlgMgr.Instance.cashShopPetEntity = null;
		//    }
		//}

		if (nowMenuMode == eCashStoreMenuMode.MAIN)
		{
			menuMain.gameObject.SetActive(true);
			nowCashStoreMenu = menuMain;
		}
		else if (nowMenuMode == eCashStoreMenuMode.CHARGE_MIRACLE)
		{
			menuMain.gameObject.SetActive(false);

			foreach (GameObject menuObject in menuObjects)
				menuObject.SetActive(false);

			txtTitle.Text = AsTableManager.Instance.GetTbl_String(818);

			menuMiracle.gameObject.SetActive(true);
			nowCashStoreMenu = menuMiracle;
		}
		else
		{
			foreach (GameObject menuObject in menuObjects)
				menuObject.SetActive(false);

			nowCashStoreMenu = dicMainMenuItem[nowMenuMode];

			nowCashStoreMenu.gameObject.SetActive(true);

			txtTitle.Text = dicMenuBtn[_mode].Text;

			menuMain.gameObject.SetActive(false);
			menuMiracle.gameObject.SetActive(false);
		}

		scrollList = nowCashStoreMenu.mainScrollList;
		scrollCategory = nowCashStoreMenu.listCategory;

		if (nowCashStoreMenu.mainScrollList != null)
		{
			if (nowCashStoreMenu.mainListItem != null)
				listItemPrefab = nowCashStoreMenu.mainListItem.gameObject;
		}

		// initilize menu
		foreach (CashStoreMenu cashStoreMenu in listNowCashStoreMenu)
			cashStoreMenu.InitMenu(nowUserClass);

		return nowCashStoreMenu;
	}

	public void ActivateItem( eCashStoreMenuMode _mode, eCashStoreSubCategory _eSubCategory, int _idx)
	{
		CashStoreMenu menu = OnMenu( _mode);

		if (dicMenuBtn.ContainsKey(_mode) == true)
			dicMenuBtn[_mode].Value = true;

		if( null != menu)
			menu.SetCategory(_eSubCategory);

		menu.ActivateItem( _idx);
	
		if (TooltipMgr.Instance != null)
			TooltipMgr.Instance.Clear();
	
		nowState = StoreState.NOTOUCH;
	}

	public void ToMiracleStep()
	{
		ActivateItem(eCashStoreMenuMode.CHARGE_MIRACLE, 0, 0);
	}

	public void GotoMiracleMenu()
	{
		GoToMenu(eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE);
	}

	public void SetNotTouch()
	{
		nowState = StoreState.NOTOUCH;
	}

	public void GoToMenu( eCashStoreMenuMode _mode, eCashStoreSubCategory _selectCategory, bool _playSound = true, bool _initFirst = false, bool _notCheckMinor = false)
	{
		if (!dicMenuBtn.ContainsKey(_mode))
		{
			if (_mode != eCashStoreMenuMode.MAIN && _mode != eCashStoreMenuMode.CHARGE_MIRACLE) 
			{
				Debug.LogWarning(_mode + "is not contain(GoToMenu)");
				return;
			}
		}

		if (_mode == eCashStoreMenuMode.CHARGE_MIRACLE && _notCheckMinor == false)
		{
			AsMinorCheckInfo checker = new AsMinorCheckInfo();

			bool loadFile = checker.LoadFile();

			bool canOpen = checker.CheckMinorInfo();

			if (loadFile == false || canOpen == false)
			{
				LockInput(true);

				GameObject obj = ResourceLoad.CreateGameObject("UI/AsGUI/GUI_MinorCheck");
				AsMinorCheckerDlg dlg = obj.GetComponent<AsMinorCheckerDlg>();
				dlg.Show(true, nowUserClass, _mode, eCashStoreSubCategory.NONE, 0, false);

				return;
			}
		}


		CashStoreMenu menu = OnMenu( _mode);

		// menu btn on
		if (dicMenuBtn.ContainsKey(_mode))
		{
			dicMenuBtn[_mode].SetState(0);
			dicMenuBtn[_mode].Value = true;
		}

		if( menu != null)
			menu.SetCategory(_selectCategory);

		if( _playSound == true)
			PlayMenuSound( _mode);

		if (_initFirst == false && _mode != eCashStoreMenuMode.COSTUME)
			nowState = StoreState.NOTOUCH;
	}

	protected override void CloseDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( nowMenuMode == eCashStoreMenuMode.COSTUME && AsHudDlgMgr.Instance != null)
			{
				if (AsHudDlgMgr.Instance.cashShopEntity != null)
					if( AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState() != eModelLoadingState.Finished)
						return;
			}

			if( findGiftFriendPopup != null)
			{
				GameObject.DestroyImmediate( findGiftFriendPopup.gameObject);
				findGiftFriendPopup = null;
			}

			if( askSendGiftMsgBox != null)
			{
				GameObject.DestroyImmediate( askSendGiftMsgBox.gameObject);
				askSendGiftMsgBox = null;
			}

			Close();
		}
	}

	GameObject m_CouponDlg = null;
	public void InputCupon( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && IsLockInput == false)
		{
			if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			{
				AsSoundManager.Instance.PlaySound(AsSoundPath.CashshopCoupon, Vector3.zero, false);

				if (m_CouponDlg == null)
					m_CouponDlg = Instantiate(Resources.Load("UI/AsGUI/GUI_Coupon")) as GameObject;

				LockInput(true);
			}
		}
	}


	public void InputMiracleBuy( ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && IsLockInput == false)
		{
			if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
				return;

			AsSoundManager.Instance.PlaySound(AsSoundPath.CashshopMenu_Miracle, Vector3.zero, false);
			GoToMenu(eCashStoreMenuMode.CHARGE_MIRACLE, 0);
		}
	}

	public void Close()
	{
		if( purchaseManager != null)
		{
			//purchaseManager.CloseProgressMessageBox();
			purchaseManager.DisconnectIAPA();
		}

		AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);

		HideLoadingIndigator();

		AsDeathDlg.MiracleShopClosed();//$yde

		if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			AsHudDlgMgr.Instance.CloseCashStore();
		else
		{
			gameObject.SetActive( false);
			GameObject.Destroy( gameObject.transform.parent.gameObject);
		}

		if( m_CouponDlg != null)
			Destroy( m_CouponDlg.gameObject);

		if( null != AsHudDlgMgr.Instance && null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			AsEntityManager.Instance.RemoveEntity( AsHudDlgMgr.Instance.cashShopEntity);
			AsHudDlgMgr.Instance.cashShopEntity = null;
		}
	}

	void ProcessTouchInput()
	{
		if (AsGameMain.s_gameState != GAME_STATE.STATE_CHARACTER_SELECT)
		{
			// for in game
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				if (Input.touchCount <= 0)
					return;

				Touch touch = Input.GetTouch(0);

				if (touch.phase == TouchPhase.Ended)
				{
					Ray ray = uiCamera.ScreenPointToRay(touch.position);
					CheckClickCashStore(ray);
				}
			}
			else
			{
				if (!Input.GetMouseButtonUp(0))
					return;

				Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);

				CheckClickCashStore(ray);
			}
			return;
		}

		if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			if( Input.touchCount <= 0)
				return;

			Touch touch = Input.GetTouch( 0);

			if( touch.phase == TouchPhase.Ended)
			{
				Ray ray = uiCamera.ScreenPointToRay( touch.position);
				InputUp( ray);
				CheckClickCashStore(ray);
			}
		}
		else
		{
			if( !Input.GetMouseButtonUp( 0))
				return;

			Ray ray = uiCamera.ScreenPointToRay( Input.mousePosition);

			InputUp( ray);
			CheckClickCashStore(ray);
		}
	}

	public override void AddStoreItemLine( bool _isCharge = false)
	{
		if( scrollList == null)
			return;

		IUIListObject item = null;

		// for armor
		if (nowCashStoreMenu.mainScrollList != null)
		{
			if (nowCashStoreMenu.mainListItem != null)
				listItemPrefab = nowCashStoreMenu.mainListItem.gameObject;
		}

		item = scrollList.CreateItem( listItemPrefab);

		item.gameObject.name = scrollList.Count.ToString();
		item.gameObject.layer = listItemPrefab.layer;
		scrollList.UpdateCamera();
	}

	protected override void AddStoreItem( int _line, params Store_Item_Info_Table[] _storeItem)
	{
		IUIListObject item = scrollList.GetItem( _line);

		StoreItemController storeItemCon = null;
		StoreItemController storeItemConNormal = item.gameObject.GetComponent<StoreItemController>();
		StoreItemControllerBuyButton storeItemConBuyBtn = item.gameObject.GetComponent<StoreItemControllerBuyButton>();

		storeItemCon = storeItemConNormal == null ? storeItemConBuyBtn : storeItemConNormal;


		GameObject itemIcon = null;
		
		string itemID = string.Empty;
		string itemName = string.Empty;
		string itemPrice = string.Empty;
		int buyAmount = 0;
		int buyLimit = 0;
		
		int count = 0;

		foreach( Store_Item_Info_Table info in _storeItem)
		{
			if( info.Type == Store_Item_Type.ChargeItem)
			{
				itemID = info.ID_String;

				itemIcon = GetItemIconCharge( itemID);

				if( purchaseManager != null)
				{
					itemName = purchaseManager.GetProductName( info.Key);
					itemPrice = purchaseManager.GetProductPrice( info.Key);

					string currencySymboleOrginal = AsTableManager.Instance.GetTbl_String(926);
					string currencySymboleChange = AsTableManager.Instance.GetTbl_String(927);

					if( itemPrice.Contains( currencySymboleOrginal))
					{
						if( AsLanguageManager.Instance.NowLanguageType == AsLanguageManager.LanguageType.LanguageType_Korea)
							itemPrice = itemPrice.Replace( currencySymboleOrginal, currencySymboleChange);
					}
					else
						Debug.Log( "not......");
				}
				else // test
				{
					itemName = info.ID_String;
					itemPrice = "\\9,999";
				}
			}
			else if (info.Type == Store_Item_Type.GachaItem)
			{
				buyAmount = info.Price;

				itemID = info.ID.ToString();

				if (info.Count > 1)
					storeItemCon.items[count].itemCountTxt.Text = "x" + info.Count;
				else
					storeItemCon.items[count].itemCountTxt.Text = string.Empty;
			}
			else if (info.Type == Store_Item_Type.NormalItem)
			{
				ItemData itemData = null;
				itemID = info.ID.ToString();
				itemData = ItemMgr.ItemManagement.GetItem(info.ID).ItemData;
				itemName = AsTableManager.Instance.GetTbl_String(itemData.nameId);

				if (info.Price == 0)
					itemPrice = (itemData.buyAmount * info.Count).ToString();
				else
					itemPrice = info.Price.ToString();

				buyAmount = info.Price;

				// exception gold
				if (itemData.GetItemType() == Item.eITEM_TYPE.EtcItem)
				{
					if (itemData.GetSubType() == (int)Item.eEtcItem.Gold)
						itemIcon = GetItemIcon(info.ID.ToString(), 1);
					else
						itemIcon = GetItemIcon(info.ID.ToString(), info.Count);
				}
				else
				{
					itemIcon = GetItemIcon(info.ID.ToString(), info.Count);
				}

				buyLimit = itemData.m_iItem_Buy_Limit;
			}

			// set hilight & Limit && bonus & hide miracle img
			if( info != null)
			{
				if( info.Type == Store_Item_Type.ChargeItem)
				{
					eSHOPITEMHIGHLIGHT highlight = dicChargeItemInfo[itemID].eShopItemHighLight;
					
					if( highlight == eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_BONUS)
						storeItemCon.items[count].OnHighlightBonus( dicChargeItemInfo[itemID].nBonusCount);
					else
						storeItemCon.items[count].OnHighlight( highlight);

					// hide miracle Img
					if (storeItemCon.items[count].imgMiracle.gameObject != null)
					{
						storeItemCon.items[count].imgMiracle.gameObject.SetActive(false);
					}
				}
				else
				{
					//if( info.Highlight == eSHOPITEMHIGHLIGHT.eSHOPITEMHIGHLIGHT_BONUS)
					//    storeItemCon.items[count].OnHighlightBonus( info.Bonus);
					//else
						storeItemCon.items[count].OnHighlight( info.Highlight);
				}

				// limit
				storeItemCon.items[count].OnBuyLimitMark( buyLimit);
			}

			if( info != null)
			{
				if (itemIcon != null)
				{
					itemIcon.transform.parent = storeItemCon.items[count].itemSlotSprite.transform;
					itemIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f);
					itemIcon.transform.localScale = Vector3.one;
					storeItemCon.items[count].itemIconObject = itemIcon;
				}

				if (storeItemCon.items[count].itemName != null)
					storeItemCon.items[count].itemName.Text = itemName;

				if( info.Type == Store_Item_Type.ChargeItem)
					storeItemCon.items[count].itemPrice.Text = itemPrice;
				else
					storeItemCon.items[count].itemPrice.Text = buyAmount.ToString();

				if (storeItemCon.items[count].itemDescription != null)
				{
					AsLanguageManager.Instance.SetFontFromSystemLanguage(storeItemCon.items[count].itemDescription);

					if (info.Type == Store_Item_Type.ChargeItem)
					{
						Tbl_ChargeRecord chargeRecord = AsTableManager.Instance.GetChargeRecord(info.ID_String);
						storeItemCon.items[count].itemDescription.Text = info.DescID == -1 ? string.Empty : AsTableManager.Instance.GetTbl_String(chargeRecord.descriptionID);
					}
					else
						storeItemCon.items[count].itemDescription.Text = info.DescID == -1 ? string.Empty : AsTableManager.Instance.GetTbl_String(info.DescID);

				}
			
				//data
				storeItemCon.items[count].itemUIInfoData = new StoreItemInfoUI( info.Type,
					itemID, buyAmount.ToString(), info.Count, info.Key, itemName);
			}


			// set input buy button
			if (storeItemConBuyBtn != null)
				if (storeItemConBuyBtn.btnBuy != null)
					storeItemConBuyBtn.btnBuy.SetInputDelegate(BuyItemBtnInputDelegate);

			storeItemCon.initilized = true;
			UIListItemContainer con = item.gameObject.GetComponent<UIListItemContainer>();
			con.ScanChildren();
			scrollList.ClipItems();

			count++;
		}
	}

	public override void AddStoreItems( List<Store_Item_Info_Table> _storeItemList)
	{
		if (scrollList == null)
			return;

		scrollList.ScrollWheel( 0);
		scrollList.ClearListSync( true);

		List<Store_Item_Info_Table> filteredList = new List<Store_Item_Info_Table>();

		eCashStoreMainCategory nowMenuKind = dicMainMenuItem[nowMenuMode].mainItemKind;// nowMenu.mainItemKind;

		if (nowMenuKind == eCashStoreMainCategory.RECHARGE_GOLD || nowMenuKind == eCashStoreMainCategory.RECHARGE_MIRACLE)
		{
			foreach( Store_Item_Info_Table info in _storeItemList)
			{
				if (nowMenuKind == eCashStoreMainCategory.RECHARGE_GOLD)
					filteredList.Add( info);
				else if (nowMenuKind == eCashStoreMainCategory.RECHARGE_MIRACLE)
					filteredList.Add( info);
			}
		}
		else
		{
			filteredList = _storeItemList;
		}

		if (nowCashStoreMenu.mainScrollList != null)
		{
			if (nowCashStoreMenu.mainListItem != null)
				listItemPrefab = nowCashStoreMenu.mainListItem.gameObject;
		}

		StoreItemController storeItemController = listItemPrefab.GetComponent<StoreItemController>();

		int line = 0;
		bool addLine = false;
		int totLine = 0;
		int itemCount = storeItemController.items.Length;
		int remainItem = 0;

		// 1 line list
		line = filteredList.Count / itemCount;
		remainItem = filteredList.Count % itemCount;
		addLine = remainItem == 0 ? false : true;
		totLine = line;

		if( addLine == true)
			totLine++;

		for( int i = 0; i < totLine; i++)
			AddStoreItemLine();

		List<Store_Item_Info_Table> listAddTable = new List<Store_Item_Info_Table>();

		for( int lineCount = 0; lineCount < line; lineCount++)
		{
			int addNumForOverTwoLine = 0;
			listAddTable.Clear();

			for( int j = 0; j < itemCount; j++)
			{
				int idx = lineCount + addNumForOverTwoLine;

				if( idx < filteredList.Count)
					listAddTable.Add( filteredList[idx]);

				addNumForOverTwoLine += totLine;
			}

			AddStoreItem( lineCount, listAddTable.ToArray());
		}

		if( addLine == true)
		{
			listAddTable.Clear();
			
			for( int i = 0; i < remainItem; i++)
				listAddTable.Add( filteredList[totLine * (i + 1) - 1]);

			AddStoreItem( totLine - 1, listAddTable.ToArray());
			IUIListObject listItem = scrollList.GetItem( totLine - 1);
			StoreItemController con = listItem.gameObject.GetComponent<StoreItemController>();
			con.UpdateEnvisibleSlot();
		}
	}

	protected override void BuyItemBtnInputDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && IsLockInput == false && nowState != StoreState.NOTOUCH)
		{
			Transform tm = ptr.targetObj.transform.parent.parent;

			StoreItemControllerBuyButton itemController = tm.gameObject.GetComponent<StoreItemControllerBuyButton>();

			// check remain Time
			StoreItemInfoUI info = itemController.GetStoreItemInfo(StoreTouchType.STORE_TOUCH_LEFT);

			if (info != null && info.item_Type == Store_Item_Type.ChargeItem)
			{
				if (dicChargeItemInfo.ContainsKey(info.itemID))
				{
					int remainTime = dicChargeItemInfo[info.itemID].nRemainTime;

					if (remainTime > 0)
					{
						ShowItemRemainTimeMsgBox(remainTime);
						return;
					}
				}
			}

			if (itemController != null)
			{
				StoreTouchInfo dummyTouchInfo = new StoreTouchInfo();
				dummyTouchInfo.type = StoreTouchType.STORE_TOUCH_LEFT;
				dummyTouchInfo.idx = int.Parse(tm.gameObject.name);
				dummyTouchInfo.storeItemController = itemController;
				dummyTouchInfo.touchObject = null;
				PrepareBuyNowTouchItem(dummyTouchInfo);
			}

			HideMiracleInfo();
		}
	}

	public override void ProcessDClickUp(){ /*not process*/ }

	public override void ProcessUpClick()
	{
		if (scrollList == null || nowState == StoreState.NOTOUCH)
		{
			isClicked = false;
			return;
		}

		if (scrollList.IsAutoScrolling == true || scrollList.IsScrolling == true)
		{
			isClicked = false;
			return;
		}

		if (IsLockInput == true)
		{
			isClicked = false;
			return;
		}

		if (nowState == StoreState.NORMAL || nowState == StoreState.SELECT)
		{
			if (isClicked == false)
				return;

			fClickTimePassed += Time.deltaTime;

			if (fClickTimePassed >= fDClickTime)
			{
				nowTouchInfo = OneClickTouchInfo;
				SelectStoreItem(nowTouchInfo);
				prevTouchInfo = nowTouchInfo;
				fStationaryPassTime = 0.0f;
				nowState = StoreState.SELECT;

				StoreItemInfoUI storeItemInfo = nowTouchInfo.storeItemController.GetStoreItemInfo(nowTouchInfo.type);

				PlayItemSound(storeItemInfo);

				HideMiracleInfo();

				if (storeItemInfo == null)
				{
					fClickTimePassed = 0.0f;
					isClicked = false;
					return;
				}

				// exception miracle
				if (storeItemInfo.item_Type == Store_Item_Type.ChargeItem)
				{
					fClickTimePassed = 0.0f;
					isClicked = false;
					//PrepareBuyNowTouchItem(nowTouchInfo);
					return;
				}
				else
				{
					if (nowCashStoreMenu != null)
						nowCashStoreMenu.ClickMainListItemProcess(storeItemInfo);
				}

				fClickTimePassed = 0.0f;
				isClicked = false;
			}
		}
	}

	public AsBasePurchase GetPurchaseManager()
	{
		return purchaseManager;
	}

	public void AddItemToListForEditor()
	{
		List<string> listItemName = new System.Collections.Generic.List<string>();
	
		listItemName.AddRange( dicChargeItemInfo.Keys);
	
		List<Store_Item_Info_Table> testList = new List<Store_Item_Info_Table>();
		int count = 0;
		foreach( string id in listItemName)
			testList.Add( new Store_Item_Info_Table( Store_Item_Type.ChargeItem, count++, id, 1));

		AddStoreItems( testList);

		nowState = StoreState.NOTOUCH;
	}

	public void RequestProductInfoToPurchaseServer( body2_SC_CHARGE_ITEMLIST[] _chargeItems)
	{
		dicChargeItemInfo.Clear();
		
		List<string> listItemName = new System.Collections.Generic.List<string>();
		
		dicChargeItemInfo = ConvertChargeItemListToDictionary( _chargeItems);
		
		listItemName.AddRange( dicChargeItemInfo.Keys);

		if( purchaseManager != null)
		{
			purchaseManager.RequestProductInfos( listItemName.ToArray());
		}
		else
		{
			#region -test-
#if UNITY_EDITOR
			List<Store_Item_Info_Table> testList = new List<Store_Item_Info_Table>();
			int count = 0;

			foreach (body2_SC_CHARGE_ITEMLIST item in _chargeItems)
				Debug.LogWarning(item.ToString());

			foreach( string id in listItemName)
				testList.Add( new Store_Item_Info_Table( Store_Item_Type.ChargeItem, count++, id, 1));

			ProcessAfterReceiveProductInfo();

			nowState = StoreState.NOTOUCH;
#endif
			#endregion
		}
	}

	public void ShowLoadingIndigator( string _msg = "")
	{
		if( cashstoreLoadingIndigator != null)
			cashstoreLoadingIndigator.ShowIndigator( _msg);
	}

	public void HideLoadingIndigator()
	{
		if( cashstoreLoadingIndigator != null)
			cashstoreLoadingIndigator.HideIndigator();
	}

	public static Dictionary<string, body2_SC_CHARGE_ITEMLIST> ConvertChargeItemListToDictionary( body2_SC_CHARGE_ITEMLIST[] _chargeItems)
	{
		Dictionary<string, body2_SC_CHARGE_ITEMLIST> dicItemName = new Dictionary<string, body2_SC_CHARGE_ITEMLIST>();

		foreach( body2_SC_CHARGE_ITEMLIST product in _chargeItems)
		{
			string id = System.Text.ASCIIEncoding.Default.GetString( product.strIAPProductID);
	
			int lastIdx = id.IndexOf( '\0');
			string productId = id.Substring( 0, lastIdx);
	
			if( !dicItemName.ContainsKey( productId))
				dicItemName.Add( productId, product);
		}

		return dicItemName;
	}

	public override void InputMove( Ray _ray) { }

	public void PrepareBuyItem( StoreItemInfoUI _storeItemInfo)
	{
		base.LockInput(true);

		if( _storeItemInfo.item_Type == Store_Item_Type.ChargeItem)
		{
			#region -for japan-
			/*GameObject objMsgBoxPrefab = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_MiraclePopup");

			GameObject msgBox = GameObject.Instantiate( objMsgBoxPrefab) as GameObject;

			msgBox.transform.parent = transform;
			msgBox.transform.localPosition = Vector3.zero;

			askSendGiftMsgBox = msgBox.GetComponentInChildren<AskSendGiftMsgBox>();

			foundFriendUniqueID = 0;

			askSendGiftMsgBox.Initilize( this, "Deal", "FindFriend", "Cancel");*/
			#endregion
			Deal();
		}
		else
			ShowAskDealMessageBox( AsTableManager.Instance.GetTbl_String(126), string.Format( AsTableManager.Instance.GetTbl_String(1411), _storeItemInfo.itemName, _storeItemInfo.itemPrice), "Deal");
	}

	public override AsMessageBox ShowAskDealMessageBox( string _title, string _message, string _dealFuncName)
	{
		return AsNotify.Instance.MessageBox( _title, _message, AsTableManager.Instance.GetTbl_String(1152), AsTableManager.Instance.GetTbl_String(1151),
			this, _dealFuncName, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION,false);
	}

	void FindFriend()
	{
		LockInput(true);
		
		if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
			GameObject objMsgBoxPrefab = ResourceLoad.LoadGameObject("UI/AsGUI/GUI_MiraclePopup_Gift");

			GameObject msgBox = GameObject.Instantiate(objMsgBoxPrefab) as GameObject;

			msgBox.transform.parent = transform;
			msgBox.transform.localPosition = Vector3.zero;

			FindGiftFriendPopup findGiftFriend = msgBox.GetComponentInChildren<FindGiftFriendPopup>();

			if (findGiftFriend != null)
				findGiftFriendPopup = findGiftFriend;

			findGiftFriend.Initilize(this, "Deal", "Cancel");
		}
		else if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1520), this, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
	}

	protected override void Deal()
	{
		if( /*nowState == StoreState.QUANTITY_BUY &&*/ BuyItemEvent != null)
		{
			BuyItemEvent();
		}
	}

	protected override void Cancel()
	{
		Debug.LogWarning( "cancel");
		nowState = StoreState.NOTOUCH;
		foundFriendUniqueID = 0;
	}

	public void ReceiveGiftFriendInfo( uint _giftFriendUniqueID, int _level, int _class)
	{
		if( findGiftFriendPopup != null)
		{
			findGiftFriendPopup.ReceiveFriendInfo( _giftFriendUniqueID, _level, _class);
			foundFriendUniqueID = _giftFriendUniqueID;
		}
		else
			foundFriendUniqueID = 0;
	}

	public void BuyItem()
	{
		StoreItemInfoUI storeItemUIInfo = nowTouchInfo.storeItemController.GetStoreItemInfo(nowTouchInfo.type);

		if (storeItemUIInfo.item_Type == Store_Item_Type.ChargeItem)
		{
			if (purchaseManager != null)
			{
				uint characterUniqueID = 0;

				if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
					characterUniqueID = AsUserInfo.Instance.GetCurrentUserCharacterInfo().nCharUniqKey;

				ShowLoadingIndigator();

				prevStoreItemInfo = storeItemUIInfo;

				LockInput(true);

				purchaseManager.RequestItemRegisterKey(AsUserInfo.Instance.LoginUserUniqueKey, characterUniqueID, storeItemUIInfo.itemSlot, storeItemUIInfo.itemID);
			}
		}
		else
		{
			prevStoreItemInfo = storeItemUIInfo;

			isLockInput = true;

			#region -kor ver-
			if (false)
				AsCommonSender.SendRequestBuyCashItem(storeItemUIInfo.itemSlot, 1);
			#endregion
			AsCommonSender.SendRequestBuyCashItemJpn(storeItemUIInfo.itemSlot,int.Parse(storeItemUIInfo.itemID), 1, 0);
		}
	}

	public void BuyNowItem(int _registerKey)
	{
		if (purchaseManager != null)
		{
			if (Application.platform == RuntimePlatform.Android)
				purchaseManager.PurchaseProduct(prevStoreItemInfo.itemID, 1, AsUserInfo.Instance.LoginUserUniqueKey, prevStoreItemInfo.itemSlot, AsUserInfo.Instance.CurrentServerID, foundFriendUniqueID, _registerKey);
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				purchaseManager.PurchaseProduct(prevStoreItemInfo.itemID, 1, prevStoreItemInfo.itemSlot, AsUserInfo.Instance.CurrentServerID, foundFriendUniqueID);
			}
		}
	}

	public void SetGotoMenuAfterInit(eCashStoreMenuMode _mode, eCashStoreSubCategory _category, int _idx)
	{
		if (afterGotoMenu == eCashStoreMenuMode.NONE)
			return;

		afterGotoMenu = _mode;
		afterSelectCategory = _category;
		afterSelectIdx = _idx;
	}

	public GameObject GetItemIconCharge( string _itemID)
	{
		Object objIcon = null;

		Tbl_ChargeRecord chargeRecord = AsTableManager.Instance.GetChargeRecord(_itemID);

		objIcon = Resources.Load(chargeRecord.iconPath);

		GameObject objIconInstantiate = GameObject.Instantiate( objIcon) as GameObject;

		return objIconInstantiate;
	}

	void TestIAPAServer()
	{
		string transID = "12999763169054705758.1362860355674459";
		TextAsset textAsset = Resources.Load( "json") as TextAsset;
		string receipt = textAsset.text.Replace( " ", "");
	
		//purchaseManager = gameObject.AddComponent<GooglePurchaseManager>();
	
		//purchaseManager.MakeMessageBox();
	
		// 저장 공간이 부족할 경우 대비 해야 한다.
		//string characterName = AsUserInfo.Instance.GetCharacterName();
		//ArkTransaction transaction = new ArkTransaction( string.Empty, characterName, "test3_gold_1000", transID, receipt, 0, 1, AsUserInfo.Instance.GetCurrentUserCharacterInfo().nCharUniqKey, AsUserInfo.Instance.CurrentServerID, 0);
		//purchaseManager.StartGetItemProcess( transaction);
	}

	void PlayMenuSound( eCashStoreMenuMode _mode)
	{
		string soundPath = string.Empty;

		switch ( _mode)
		{
			case eCashStoreMenuMode.MAIN:
			case eCashStoreMenuMode.PET:
			case eCashStoreMenuMode.CONVENIENCE:
			case eCashStoreMenuMode.EVENT:
				soundPath = AsSoundPath.CashshopMenu_Package;
				break;

			//case eCashStoreMenuMode.CHARGE_MIRACLE:
			case eCashStoreMenuMode.WEAPON:
			case eCashStoreMenuMode.EQUIPMENT:
			case eCashStoreMenuMode.FREE:
			case eCashStoreMenuMode.COSTUME:
				soundPath = AsSoundPath.CashshopMenu_Miracle;
				break;
			
		}

		if( soundPath != string.Empty)
			AsSoundManager.Instance.PlaySound( soundPath, Vector3.zero, false);
	}

	public static void CreateCashStoreForMiracle()
	{
		GameObject cashStoreObj = GameObject.Instantiate(Resources.Load("UI/MiracleShop/GUI_MiracleShop")) as GameObject;
		AsCashStore cashStore = null;

		if( cashStoreObj == null)
			return;

		cashStore = cashStoreObj.GetComponentInChildren<AsCashStore>();

		cashStore.SetGotoMenuAfterInit(eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);

		if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
			cashStore.InitilizeStore(0, (eCLASS)AsUserInfo.Instance.SavedCharStat.class_);
			AsHudDlgMgr.Instance.SetCashStoreObj(cashStoreObj);
		}
		else
		{
			cashStore.InitilizeStore(0, eCLASS.NONE);
			ms_kIstance = cashStore;
			AsNotify.Instance.CashStoreRef = cashStore;
		}

		cashStoreObj.transform.localPosition = new Vector3( 0.0f, 0.0f, -20.0f);
	}

	public override void LockInput( bool _value)
	{
		if( _value == true)
			fNotTouchPassTime = 0.0f;

		base.LockInput( _value);

		foreach (UIRadioBtn radio in menuButtons)
		{
			if (radio != null)
				radio.useLockInput = _value;
		}
	}

	public override void OpenTooltip( RealItem _reamItem, Item _itemData, MonoBehaviour _script, string method, int _price = -1, int _key = -1)
	{	
		base.OpenTooltip( _reamItem, _itemData, _script, method, System.Convert.ToInt32( nowTouchInfo.storeItemController.GetStoreItemInfo(nowTouchInfo.type).itemPrice), _key);
	}

	void ProcessPurchasePolicyInfo(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{

		}
	}

	void ProcessMiracleInfo(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && miracleInfoPopup.gameObject.activeSelf == false)
		{
			miracleInfoPopup.gameObject.SetActive(true);
			miracleInfoPopup.Show(nMiracle, nFreeMiracle);
		}
	}

	void CheckClickCashStore(Ray _ray)
	{
		if (AsUtil.PtInCollider(gameObject.collider, _ray) == true)
		{
			if (miracleInfoPopup != null)
				miracleInfoPopup.gameObject.SetActive(false);
		}
	}

	void HideMiracleInfo()
	{
		if (miracleInfoPopup != null)
			miracleInfoPopup.gameObject.SetActive(false);
	}

	public void SetPrevStoreItemInfoUI(StoreItemInfoUI _info)
	{
		prevStoreItemInfo = _info;
	}

	void ShowItemRemainTimeMsgBox(int _remainTime)
	{
		string form = string.Empty;

		int remainHour = _remainTime / 3600;

		int resultValue = 0;

		if (remainHour >= 24)
		{
			resultValue = remainHour / 24;
			form = AsTableManager.Instance.GetTbl_String(448);
		}
		else if (remainHour < 24 && remainHour >= 1)
		{
			resultValue = remainHour;
			form = AsTableManager.Instance.GetTbl_String(449);
		}
		else
		{
			resultValue = _remainTime / 60;
			form = AsTableManager.Instance.GetTbl_String(450);
		}

		string resultString = string.Format(form, resultValue);

		LockInput(true);

		AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), resultString, this, "Cancel", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
}
