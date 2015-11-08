using UnityEngine;
using System.Collections;
using System.Text;

public class AsCharacterSlot : MonoBehaviour
{
	private const int SLOT_0 = 0;
	private const int SLOT_1 = 1;
	private const int SLOT_2 = 2;

	public static int STATE_EXACT_POSITION = -1;
	public static int STATE_LEFT_SHIFT = 0;
	public static int STATE_RIGHT_SHIFT = 1;
	public static float SLOT_WIDTH = 13.5f;

	[SerializeField] UIButton btnCreate = null;
	[SerializeField] UIButton btnJoin = null;
	[SerializeField] UIButton btnUnlock = null;
	[SerializeField] UIButton btnCancel = null;
	[SerializeField] SimpleSprite locked = null;//$yde
	[SerializeField] SpriteText miracleCost = null;

	public GameObject charDummy = null;
	public float rotSpeed = 100.0f;
	private float rotAccum = 0.0f;

	private int index = -1;
	private int screenIndex = -1;
	private int curStatus = STATE_EXACT_POSITION;
	private float shiftStartTime = 0.0f;
	private AsUserEntity userEntity = null;
	private bool isClicked = false;
//	private float prevClickedTime = 0.0f;
	private Camera uiCamera = null;
	private bool isEmpty = true;
	private bool showDeleteBox = false;
	private bool isJoinSend = false;
	private bool isCreateSend = false;
	private Vector2 touchPos = Vector2.zero;
	private bool isExistDeleteDlg = false;
	private bool loadStarted = false;
	private bool isLoadedLoginCreateScene = false;
	[SerializeField]GameObject footPlate = null;
	[SerializeField]GameObject emptyEffect = null;

	public AsCharacterSlotInfo info = null;
	public UIButton deleteBtn = null;
	public AsCharacterSlotDeleteBox deleteBox = null;

	private bool m_isChangeScale = false; //dopamin #17440
	public bool IsChangeScale
	{
		get { return m_isChangeScale; }
		set { m_isChangeScale = value; }
	}
	//$yde
	[SerializeField] AsCharacterSlotShopInfo shopInfo = null;

	public bool IsEmpty
	{
		get { return isEmpty; }
		set { isEmpty = value; }
	}

	public int Index
	{
		get { return index; }
		set { index = value; }
	}

	public int ScreenIndex
	{
		get { return screenIndex; }
		set { screenIndex = value; }
	}

	public AsUserEntity Data
	{
		get { return userEntity; }
		set
		{
			userEntity = value;
//			info.SetData( userEntity, true);
		}
	}

	private bool isLocked = false;
	public bool IsLocked
	{
		set
		{
			isLocked = value;
			info.SetData( null, value);
		}
	}

	// Use this for initialization
	void Start()
	{
		isJoinSend = false;
		isCreateSend = false;
		uiCamera = Camera.mainCamera;

		btnCreate.Text = AsTableManager.Instance.GetTbl_String(1346);
		locked.transform.GetChild(0).GetComponent<SpriteText>().Text = AsTableManager.Instance.GetTbl_String(1531);//$yde
		btnJoin.Text = AsTableManager.Instance.GetTbl_String(1347);
		btnUnlock.Text = AsTableManager.Instance.GetTbl_String(1412);
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		miracleCost.Text = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 36).Value.ToString();

		if( AsCharacterSlotManager.PossibleCharCreate == true)
		{
			locked.gameObject.SetActiveRecursively( false);
		}
		else
		{
			btnCreate.gameObject.SetActiveRecursively( false);
			btnJoin.gameObject.SetActiveRecursively( false);
			btnUnlock.gameObject.SetActiveRecursively( false);
			btnCancel.gameObject.SetActiveRecursively( false);
			miracleCost.gameObject.SetActiveRecursively( false);
		}

#if false
		if( 0 == AsTableManager.Instance.GetTbl_GlobalWeight_Record(79).Value)
			btnJoin.SetControlState( UIButton.CONTROL_STATE.DISABLED);
#endif
	}

	public void SetPosition( Vector3 pos)
	{
		transform.position = pos;
	}

	private void UpdateInput()
	{
		if( false == _isLoadedLoginCreateScene())
			return;

		if( true == AsGameMain.isPopupExist)
			return;

		if( 0 >= Input.touchCount)
			return;

		Touch touch = Input.GetTouch(0);

		switch( touch.phase)
		{
		case TouchPhase.Began:
			{
				if( false == AsUtil.PtInCollider( uiCamera, collider, touch.position))
					return;

				isClicked = true;
				touchPos = touch.position;
			}
			break;
		case TouchPhase.Moved:
			{
			}
			break;
		case TouchPhase.Ended:
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

				if( true == AsCharacterSlotManager.CharacterSelected)
					return;

				if( ( true == isClicked) && ( true == AsUtil.PtInCollider( uiCamera, collider, touch.position)))
				{
					switch( screenIndex)
					{
					case 0:
						AsCharacterSlotManager.ShiftRight();
						break;
					case 2:
						AsCharacterSlotManager.ShiftLeft();
						break;
					case 1:
						if( Mathf.Abs( touchPos.x - touch.position.x) >= ( Screen.width * 0.05f))
							break;

						if( null != userEntity)
						{
							if( ( true == AsCharacterSlotManager.PossibleCharCreate) && ( true == isEmpty))
								LoadCreateScene();
							else
								SendCharacterSelectInfo();
						}
						else
						{
							UnlockSlotProcess();
						}
						break;
					}
#if false
					float curClickedTime = Time.time;
					if( 0.4f > ( curClickedTime - prevClickedTime))
					{
						switch( screenIndex)
						{
						case 0:
							AsCharacterSlotManager.ShiftRight();
							break;
						case 2:
							AsCharacterSlotManager.ShiftLeft();
							break;
						case 1:
							SendCharacterSelectInfo();
							break;
						}
					}
					else
					{
						if( 1 == screenIndex)
						{
							AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

							if( Mathf.Abs( touchPos.x - touch.position.x) < ( Screen.width * 0.05f))
							{
								if( null == userEntity)
									UnlockSlotProcess();
								else
									LoadCreateScene();
							}
						}
					}

					prevClickedTime = curClickedTime;
#endif
				}

				isClicked = false;
			}
			break;
		}
	}

	private void _rotateEntity()
	{
		if( true == isEmpty)
			return;

		if( null == userEntity)
			return;

		if( eModelLoadingState.Finished != userEntity.CheckModelLoadingState())
			return;

		if( 1 != screenIndex)
		{
			rotAccum = 0.0f;
			userEntity.transform.localRotation = Quaternion.identity;
			footPlate.transform.localRotation = Quaternion.identity;
		}
		else
		{
			rotAccum += ( Time.deltaTime * rotSpeed);
			userEntity.transform.localRotation = Quaternion.AngleAxis( rotAccum, Vector3.up);
			footPlate.transform.localRotation = Quaternion.AngleAxis( rotAccum, Vector3.up);
		}

		userEntity.transform.localPosition = Vector3.zero;
	}

	private void UpdateBtnState()
	{
		if( ( true == isEmpty) || ( 1 != screenIndex) || ( true == showDeleteBox) || ( true == isExistDeleteDlg))
		{
			deleteBtn.gameObject.SetActive( false);
			btnJoin.Hide( true);
			
			if( 1 != screenIndex)
			{
				btnCreate.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				btnCreate.spriteText.Color = Color.gray;
				btnUnlock.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				btnUnlock.spriteText.Color = Color.gray;
			}
			else
			{
				btnCreate.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				btnCreate.spriteText.Color = Color.black;
				btnUnlock.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				btnUnlock.spriteText.Color = Color.black;
			}

			btnCreate.gameObject.SetActive( ( true == isEmpty) && ( false == isLocked) && ( true == AsCharacterSlotManager.PossibleCharCreate));
			btnUnlock.gameObject.SetActive( isLocked);
			locked.gameObject.SetActive( ( null != userEntity) && ( false == AsCharacterSlotManager.PossibleCharCreate) && ( true == isEmpty) && ( ( false == showDeleteBox) || ( false == isExistDeleteDlg)));
		}
		else
		{
			deleteBtn.gameObject.SetActive( true);
			btnCreate.gameObject.SetActive( false);
			btnJoin.Hide( false);
			btnUnlock.gameObject.SetActive( false);
			locked.gameObject.SetActive( false);
		}

		btnCancel.gameObject.SetActive( ( false == isEmpty) && ( null != userEntity) && ( true == showDeleteBox));
	}

	private void CharacterLoad()
	{
		if( true == loadStarted)
			return;

		if( ( 0 > screenIndex) || ( 2 < screenIndex))
			return;

		AsPacketHeader packet = AsUserInfo.Instance.GetCharacter( index);
		if( null == packet)
			return;

		CharacterLoadData entityCreationData = null;

		if( packet.GetType() == typeof( sCHARVIEWDATA))
		{
			sCHARVIEWDATA viewData = packet as sCHARVIEWDATA;

			Debug.Log( "AsCharacterSlotManager::_CreateCharacter: viewData.nCharUniqKey" + viewData.nCharUniqKey);

			entityCreationData = new CharacterLoadData( AsUserInfo.Instance.GamerUserSessionIdx, viewData);
			userEntity = AsEntityManager.Instance.CreateUserEntity( "PlayerChar", entityCreationData, false);
			userEntity.transform.parent = charDummy.transform;
			userEntity.transform.localPosition = Vector3.zero;
			userEntity.transform.localScale = Vector3.one;
			userEntity.transform.localRotation = Quaternion.identity;
			userEntity.HandleMessage( new Msg_AnimationIndicate( "Idle"));

			emptyEffect.gameObject.SetActiveRecursively( false);
		}
		else if( packet.GetType() == typeof( AS_GC_CHAR_LOAD_RESULT_EMPTY))
		{
			entityCreationData = new CharacterLoadData( AsUserInfo.Instance.GamerUserSessionIdx, packet as AS_GC_CHAR_LOAD_RESULT_EMPTY);
			userEntity = AsEntityManager.Instance.CreateUserEntity( "EmptySlot", entityCreationData);
			Debug.Assert( null != userEntity);
		}

		info.SetData( userEntity, isLocked);

		loadStarted = true;
	}

	void UpdateDeletedCharacter()
	{
		if( false == showDeleteBox)
			return;

		if( 0 < deleteBox.remain)
			return;

		info.SetData( null, false);

		AsEntityManager.Instance.RemoveEntity( userEntity);
		userEntity = null;
		showDeleteBox = false;
		deleteBox.Hide();
		deleteBtn.gameObject.SetActiveRecursively( false);
		btnCreate.gameObject.SetActiveRecursively( true);
	}

	// Update is called once per frame
	void Update()
	{
		if( false == _isLoadedLoginCreateScene())
			return;

		CharacterLoad();
		UpdateInput();
		UpdateBtnState();

		float deltaTime = ( Time.time - shiftStartTime) * 2.0f;
		deltaTime = Mathf.Clamp01( deltaTime);

		_rotateEntity();

		if( STATE_EXACT_POSITION == curStatus)
			return;

		switch( curStatus)
		{
		case 0://STATE_LEFT_SHIFT:
			{
				if( ( 3 >= AsCharacterSlotManager.SlotCount) && ( 0 == screenIndex))
				{
					Vector3 curPos = new Vector3( -SLOT_WIDTH - 500.0f, 0.0f, 0.0f);
					Vector3 destPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * 2) - 500.0f, 0.0f, 0.0f);
					transform.position = Vector3.Lerp( curPos, destPos, deltaTime);
					if( true == AsUtil.IsEqual( deltaTime, 1.0f))
					{
						transform.position = destPos;
						curStatus = STATE_EXACT_POSITION;
						screenIndex = 2;
					}
				}
				else
				{
					if( screenIndex <= -1)
						screenIndex = AsCharacterSlotManager.SlotCount - 1;

					Vector3 curPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * screenIndex) - 500.0f, ( ( 1 == screenIndex) ? 1.5f : 0.0f), 0.0f);
					Vector3 destPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * ( screenIndex - 1)) - 500.0f, ( ( 2 == screenIndex) ? 1.5f : 0.0f), 0.0f);
					transform.position = Vector3.Lerp( curPos, destPos, deltaTime);
					if( true == AsUtil.IsEqual( deltaTime, 1.0f))
					{
						transform.position = destPos;
						curStatus = STATE_EXACT_POSITION;
						screenIndex--;
						PlaySelectSound();
					}
				}
			}
			break;
		case 1://STATE_RIGHT_SHIFT:
			{
				if( ( 3 >= AsCharacterSlotManager.SlotCount) && ( 2 == screenIndex))
				{
					Vector3 curPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * screenIndex) - 500.0f, 0.0f, 0.0f);
					Vector3 destPos = new Vector3( -SLOT_WIDTH - 500.0f, 0.0f, 0.0f);
					transform.position = Vector3.Lerp( curPos, destPos, deltaTime);
					if( true == AsUtil.IsEqual( deltaTime, 1.0f))
					{
						transform.position = destPos;
						curStatus = STATE_EXACT_POSITION;
						screenIndex = 0;
					}
				}
				else
				{
					if( screenIndex >= AsCharacterSlotManager.SlotCount - 1)
						screenIndex = -1;

					Vector3 curPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * screenIndex) - 500.0f, ( ( 1 == screenIndex) ? 1.5f : 0.0f), 0.0f);
					Vector3 destPos = new Vector3( -SLOT_WIDTH + ( SLOT_WIDTH * ( screenIndex + 1)) - 500.0f, ( ( 0 == screenIndex) ? 1.5f : 0.0f), 0.0f);
					transform.position = Vector3.Lerp( curPos, destPos, deltaTime);
					if( true == AsUtil.IsEqual( deltaTime, 1.0f))
					{
						transform.position = destPos;
						curStatus = STATE_EXACT_POSITION;
						screenIndex++;
						PlaySelectSound();
					}
				}
			}
			break;
		}
	}

	public void ShiftLeft()
	{
		if( STATE_EXACT_POSITION != curStatus)
			return;

		curStatus = STATE_LEFT_SHIFT;
		shiftStartTime = Time.time;
	}

	public void ShiftRight()
	{
		if( STATE_EXACT_POSITION != curStatus)
			return;

		curStatus = STATE_RIGHT_SHIFT;
		shiftStartTime = Time.time;
	}

	void OnMouseUpAsButton()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( false == _isLoadedLoginCreateScene())
			return;

		if( true == AsCharacterSlotManager.CharacterSelected)
			return;

		if( true == AsGameMain.isPopupExist)
			return;

		switch( screenIndex)
		{
		case 0:
			AsCharacterSlotManager.ShiftRight();
			break;
		case 2:
			AsCharacterSlotManager.ShiftLeft();
			break;
		case 1:
			if( null != userEntity)
			{
				if( ( true == AsCharacterSlotManager.PossibleCharCreate) && ( true == isEmpty))
					LoadCreateScene();
				else
					SendCharacterSelectInfo();
			}
			else
			{
				UnlockSlotProcess();
			}
			break;
		}

#if false
		float curClickedTime = Time.time;
		if( 0.4f > ( curClickedTime - prevClickedTime))
		{
			switch( screenIndex)
			{
			case 0:
				AsCharacterSlotManager.ShiftRight();
				break;
			case 2:
				AsCharacterSlotManager.ShiftLeft();
				break;
			case 1:
				SendCharacterSelectInfo();
				break;
			}
		}
		else
		{
			if( 1 == screenIndex)
			{
				if( null == userEntity)
					UnlockSlotProcess();//$yde
				else if( AsCharacterSlotManager.PossibleCharCreate == true)
					LoadCreateScene();
			}
		}

		prevClickedTime = curClickedTime;
#endif
	}

	public void OnDelete()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		if( true == isExistDeleteDlg)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		
		AsNotify.Instance.CloseAllMessageBox();
		
		if( ( 0 != AsUserInfo.Instance.nPrivateShopOpenCharUniqKey) && ( userEntity.UniqueId == AsUserInfo.Instance.nPrivateShopOpenCharUniqKey))
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1604), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return;
		}

		NotHaveUnhandledTransaction();
	}

	void OnDeleteCancel()
	{
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), AsTableManager.Instance.GetTbl_String(8), this, "DeleteCancel",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}

	void NotHaveUnhandledTransaction()
	{
		if( 1 != screenIndex)
			return;

		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(366), AsTableManager.Instance.GetTbl_String(7), this, "CharacterDelete",
			AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);

		isExistDeleteDlg = true;
		AsGameMain.isPopupExist = true;
	}

	void Unusual()
	{
		AsNotify.Instance.MessageBox( "Error", "can't check unhandled transaction", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR);
	}

	private void OnMessageBoxNotify()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		isExistDeleteDlg = false;
//		showDeleteBox = false;
		AsGameMain.isPopupExist = false;
	}

	private void CharacterDelete()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		body_CG_CHAR_DELETE charDel = new body_CG_CHAR_DELETE();
		charDel.nActionType = 1;
		charDel.nCharSlot = index;
		byte[] data = charDel.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		deleteBtn.gameObject.SetActiveRecursively( false);

		AsGameMain.isPopupExist = false;
		isExistDeleteDlg = false;
	}

	public void DeleteCancel()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", new Vector3( -500.0f, 0.0f, -50.0f), false);
		body_CG_CHAR_DELETE charDel = new body_CG_CHAR_DELETE();
		charDel.nActionType = 0;
		charDel.nCharSlot = index;
		byte[] data = charDel.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);

		showDeleteBox = false;
		deleteBtn.gameObject.SetActiveRecursively( true);

		AsGameMain.isPopupExist = false;
	}

	private void LoadCreateScene()
	{
		if( ( null == userEntity) || ( false == isEmpty) || false == AsCharacterSlotManager.IsModelAllLoaded())//$yde
			return;

		AsGameMain.createCharacterSlot = index;
		Application.LoadLevel( "CharacterCreate");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}

	private void PlaySelectSound()
	{
#if false
		if( 1 != screenIndex)
			return;

		if( null == userEntity)
			return;

		if( true == isEmpty)
			return;

		eCLASS type = userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		switch( type)
		{
		case eCLASS.DIVINEKNIGHT:
		case eCLASS.DESTROYER:
		case eCLASS.SWORDMASTER:
		case eCLASS.PALADIN:
			AsSoundManager.Instance.PlaySound( "S0021_EFF_SkillBaseAttack", Camera.mainCamera.transform.position, false);
			break;
		case eCLASS.FIREWIZARD:
		case eCLASS.ICEWIZARD:
		case eCLASS.SUMMONER:
		case eCLASS.ELEMENTALSHAMAN:
			AsSoundManager.Instance.PlaySound( "S0014_EFF_Recover", Camera.mainCamera.transform.position, false);
			break;
		}

		userEntity.HandleMessage( new Msg_Choice());
#endif
	}

	private bool SendCharacterSelectInfo()
	{
#if false
		if( 0 == AsTableManager.Instance.GetTbl_GlobalWeight_Record(79).Value)
			return false;
#endif

		if( true == AsCharacterSlotManager.CharacterSelected)
			return false;

		if( true == isEmpty)
			return false;

		if( true == showDeleteBox)
			return false;

		if( false == AsCharacterSlotManager.IsModelAllLoaded())
			return false;

		if( AsUserInfo.Instance.nPrivateShopOpenCharUniqKey != 0 && userEntity.UniqueId != AsUserInfo.Instance.nPrivateShopOpenCharUniqKey)
		{
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(258), this, "OkBtnWhileShopOpening_", "CancelBtnWhileShopOpening_", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING);
			return false;
		}
		else
			Send();

		return true;
	}

	private void Send()
	{
		AsLoadingIndigator.Instance.ShowIndigator( "");

		AS_CG_CHAR_SELECT select = new AS_CG_CHAR_SELECT( index);
		byte[] data = select.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
		//$yde
		AsUserInfo.Instance.SetCurrentUserCharacterInfo( index);

#if false
		GameObject main = GameObject.Find( "GameMain");
		AsIntroSound introSound = main.GetComponentInChildren<AsIntroSound>();
		introSound.StopSound();
#endif

		AsCharacterSlotManager.CharacterSelected = true;
		AsUserInfo.Instance.latestCharSlot = index;
	}

	//$yde
	private void OkBtnWhileShopOpening_()
	{
		AsUserInfo.Instance.ClosePrivateShop();
		AsPStoreManager.Instance.PStoreClosedByOtherSelectedCharacter();

		body_CS_PRIVATESHOP_DESTROY destroy = new body_CS_PRIVATESHOP_DESTROY();
		AsCommonSender.Send(destroy.ClassToPacketBytes());

		Send();
	}

	private void CancelBtnWhileShopOpening_()
	{
		isJoinSend = false;
	}

	AsMessageBox __popup = null;
	private void UnlockSlotProcess()//$yde
	{
	//  2014.05.16
	//	if( true == AsUtil.CheckGuestUser())
	//		return;

		if( __popup == null)
		{
			long require = ( long)AsTableManager.Instance.GetTbl_GlobalWeight_Record( 36).Value;
			string title = AsTableManager.Instance.GetTbl_String(1412);
			string content = AsTableManager.Instance.GetTbl_String(1413);

			__popup = AsNotify.Instance.CashMessageBox( require, title, content, this, "CashProcess", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE, true);
		}
	}

	private void CashProcess()//$yde
	{
		long require = ( long)AsTableManager.Instance.GetTbl_GlobalWeight_Record( 36).Value;

		if( AsUserInfo.Instance.nMiracle < require)
		{
			string title = AsTableManager.Instance.GetTbl_String(1412);
			string content = AsTableManager.Instance.GetTbl_String(368);

			//KB
			if (AsGameMain.useCashShop == true)
				__popup = AsNotify.Instance.MessageBox(title, content, this, "OpenCashShop", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			else
				__popup = AsNotify.Instance.MessageBox(title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
		}
		else
		{
			SendUnlockSlot();
		}
	}

	private void OpenCashShop()//$yde
	{
		AsMinorCheckInfo checker = new AsMinorCheckInfo();

		bool loadFile = checker.LoadFile();

		bool canOpen = checker.CheckMinorInfo();

		if (loadFile == false || canOpen == false)
		{
			GameObject obj = ResourceLoad.CreateGameObject("UI/AsGUI/GUI_MinorCheck");

			AsMinorCheckerDlg dlg = obj.GetComponent<AsMinorCheckerDlg>();
			dlg.Show(true, eCLASS.NONE, eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);
			return;
		}
		else
		{
			AsCashStore.CreateCashStoreForMiracle();
		}
	}

	private void SendUnlockSlot()//$yde
	{
		body_CG_CHAR_SLOT_ADD slotAdd = new body_CG_CHAR_SLOT_ADD();
		byte[] data = slotAdd.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	public void SetDeleteState( int remainTime)
	{
		showDeleteBox = ( 0 != remainTime) ? true : false;
		btnCancel.gameObject.SetActiveRecursively( showDeleteBox);
		deleteBox.StartDeleteState( remainTime);
	}

	private bool _isLoadedLoginCreateScene()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			//return ( AssetbundleManager.Instance.isLoadedScene( "CharacterCreate") && AssetbundleManager.Instance.isLoadedScene( "ChannelSelect"));
			if( false == isLoadedLoginCreateScene)
			{
				isLoadedLoginCreateScene = ( AssetbundleManager.Instance.isLoadedScene( "CharacterCreate") & AssetbundleManager.Instance.isLoadedScene( "ChannelSelect"));
			}

			return isLoadedLoginCreateScene;
		}

		return true;
	}

	#region - shop info -
	bool shopOpened = false;
	public void SetShopRemainTime( float _remain)
	{
		shopInfo.BeginTimeCount( _remain);//, _total);
		shopOpened = true;
	}

	public void ClearShopTime()
	{
		shopInfo.Clear();

		if( shopOpened == true)
			userEntity.HandleMessage( new Msg_ClosePrivateShop());
	}
	#endregion

	private void OnBtnCreate()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == isCreateSend || AsCharacterSlotManager.PossibleCharCreate == false)
			return;

		AsNotify.Instance.CloseAllMessageBox();

		Debug.Log( "OnBtnCreate");
		LoadCreateScene();

		isCreateSend = true;
	}

	private void OnBtnJoin()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == isJoinSend)
			return;

		AsNotify.Instance.CloseAllMessageBox();

		Debug.Log( "OnBtnJoin");
		//##15889 dopamin
		if(SendCharacterSelectInfo())
			isJoinSend = true;
	}

	private void OnBtnUnlock()
	{
		if( true == AssetbundleManager.Instance.useAssetbundle)
		{
			if( true == AssetbundleManager.Instance.isOpenPatchChoiceMsgBox())
				return;
		}

		int slotCount = AsCharacterSlotManager.SlotCount;
		for( int i = 0; i < slotCount; i++)
		{
			AsCharacterSlot slot = AsCharacterSlotManager.GetSlot( i);
			if( null == slot)
				continue;

			if( true == slot.isExistDeleteDlg)
				slot.OnMessageBoxNotify();

			if( true == slot.showDeleteBox)
				slot.deleteBox.MessageBoxClose();
		}

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		UnlockSlotProcess();//$yde
	}
}
