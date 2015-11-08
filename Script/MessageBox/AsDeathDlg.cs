using UnityEngine;
using System.Collections;

public class AsDeathDlg : MonoBehaviour
{
//	public SpriteText title = null; // ilmeda, 20120824
	public SimpleSprite img = null;
	public SpriteText textTitle = null;
	public SpriteText text = null;
//	public SpriteText haveCashTitle;
//	public SpriteText haveCash;
	public SpriteText remainTime;
	public SpriteText remainTimeTitle;
	public UIButton btnTown = null;
	public UIButton btnResurrection = null;
	public UIButton btnShop = null;
	public float displayTime = 60;
	public SpriteText btnMiracle_Text_Miracle = null;

	static AsDeathDlg instance;
	static bool s_MiracleShopOpened = false;

	static int s_ResurrectCnt = 1;
	
	eCashStoreMenuMode m_CashSotreMenu = eCashStoreMenuMode.CONVENIENCE;

	// Use this for initialization
	void Start()
	{
		instance = this;
		
		textTitle.Text = AsTableManager.Instance.GetTbl_String(3);
		
		// < ilmeda, 20120824
//		title.Text = "";
		text.Text = "";
		AsCommonSender.ResetSendCheck();
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnMiracle_Text_Miracle);//$yde
//		title.Text = AsTableManager.Instance.GetTbl_String(3);
		// ilmeda, 20120824 >
		if( true == TargetDecider.CheckCurrentMapIsIndun())
			btnTown.Text = AsTableManager.Instance.GetTbl_String(1865);
		else
			btnTown.Text = AsTableManager.Instance.GetTbl_String(1368);
		
		btnResurrection.Text = AsTableManager.Instance.GetTbl_String(1367);
		btnShop.spriteText.Text = AsTableManager.Instance.GetTbl_String(1769);

		//$yde
//		btnMiracle_Text_Miracle.Text = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 37).Value.ToString();
		AsUserEntity entity = AsUserInfo.Instance.GetCurrentUserEntity();
		eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
		int level = entity.GetProperty<int>( eComponentProperty.LEVEL);
//		btnMiracle_Text_Miracle.Text = AsTableManager.Instance.GetTbl_Level_Record( __class, level).Resurrection_Cost.ToString();
		btnMiracle_Text_Miracle.Text = GetResurrectMiracle().ToString();

		s_MiracleShopOpened = false;
		
		if( true == TargetDecider.CheckCurrentMapIsIndun())
			text.Text = AsTableManager.Instance.GetTbl_String(1864);
		else
			text.Text = AsTableManager.Instance.GetTbl_String(5);
		
//		haveCashTitle.Text = AsTableManager.Instance.GetTbl_String(859);
//		haveCash.Text = AsUserInfo.Instance.nMiracle.ToString();
		remainTimeTitle.Text = AsTableManager.Instance.GetTbl_String(861);

		BoxCollider col = gameObject.collider as BoxCollider;
		col.size = new Vector3( 80.0f, 60.0f, 0.0f);
		
		btnTown.SetInputDelegate( OnTownClicked);
		btnResurrection.SetInputDelegate( OnResurrectionClicked);
		btnShop.SetInputDelegate( OnShopBtnClicked);
		
		int idx = Random.Range( 1, 5);
		switch( idx)
		{

		case 1: m_CashSotreMenu = eCashStoreMenuMode.WEAPON; break;
		case 2: m_CashSotreMenu = eCashStoreMenuMode.PET; break;
		case 3: m_CashSotreMenu = eCashStoreMenuMode.COSTUME; break;
		case 4: m_CashSotreMenu = eCashStoreMenuMode.CONVENIENCE; break;

		}
		string imgStr = "UIPatchResources/DeathDlgImage/DeathDlgImg00" + idx.ToString();
		
		img.renderer.material.mainTexture = ResourceLoad.Loadtexture( imgStr);

		if( true == TargetDecider.CheckCurrentMapIsIndun())
		{
			btnShop.gameObject.SetActive( false);

			if( AsUserInfo.Instance.nMiracle < GetResurrectMiracle())
			{
				btnResurrection.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				btnResurrection.spriteText.SetColor( Color.gray);
			}
		}

//		displayTime = 60;
//		StartCoroutine( TimeProcess());
	}

	IEnumerator TimeProcess()
	{
		while( true)
		{
//			displayTime -= Time.deltaTime;
			displayTime -= 1;
			int minute = (int)displayTime / 60;
			int second = (int)( displayTime % 60.0f);

			string strBuf = AsTableManager.Instance.GetTbl_String(5);
			text.Text = string.Format( strBuf + "\n{0:D2}:{1:D2}", minute, second);

			yield return new WaitForSeconds( 1);

			if( 0.0f >= displayTime)
			{
				GotoTown();
				break;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if( 0.0f >= displayTime)
		{
			GotoTown();
			return;
		}

		displayTime -= Time.deltaTime;
		int minute = (int)displayTime / 60;
		int second = (int)( displayTime % 60.0f);

		//text.Text = string.Format( "Remaining time\n{0:D2}:{1:D2}", minute, second);
		//text.Text = string.Format( strBuf + "\n{0:D2}:{1:D2}", minute, second);

		remainTime.Text = string.Format( "{0:D2}:{1:D2}", minute, second);
	}

	float m_PausedTime;
	void OnApplicationPause( bool _pause)
	{
		if( _pause == true)
		{
			m_PausedTime = Time.realtimeSinceStartup;
		}
		else
		{
			float pausedTime = Time.realtimeSinceStartup - m_PausedTime;
			displayTime -= pausedTime;
		}
	}

	void OnResurrectionClicked(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Resurrect();
		}
	}
	
	void Resurrect()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

//		AsUserEntity entity = AsUserInfo.Instance.GetCurrentUserEntity();
//		eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//		int level = entity.GetProperty<int>( eComponentProperty.LEVEL);
//		long require = (long)AsTableManager.Instance.GetTbl_Level_Record( __class, level).Resurrection_Cost;
		long require = GetResurrectMiracle();

		if( AsUserInfo.Instance.nMiracle < require)
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(339);
			if( AsGameMain.useCashShop == true)
				AsNotify.Instance.MessageBox( title, content, this, "OnMiracleShop", "OnCanceled", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			else
				AsNotify.Instance.MessageBox( title, content, this, "OnCanceled", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else
		{
			string title = AsTableManager.Instance.GetTbl_String(1102);
			string content = AsTableManager.Instance.GetTbl_String(340);
			content = string.Format( content, require);
			AsNotify.Instance.MessageBox( title, content, this, "OnResurrectionConfirmed", "OnCanceled", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}

		for( int i = 0; i < transform.childCount; ++i)
		{
			transform.GetChild( i).gameObject.SetActiveRecursively( false);
		}
	}

	void OnTownClicked(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			GotoTown();
		}
	}
	
	void GotoTown()
	{
		AsPromotionManager.Instance.Reserve_Revive();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsCommonSender.SendGotoTown();
		AsUserInfo.Instance.SavedCharStat.hpCur_ = 1;

		Destroy( gameObject.transform.parent.gameObject);
		
		AsNotify.Instance.CloseAllMessageBox();
	}

	void OnMiracleShop()
	{
		if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
		{
			gameObject.SetActiveRecursively( true);
			return;
		}

		if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
		{
			gameObject.SetActiveRecursively( true);
			return;
		}

		// cash store required
		AsHudDlgMgr.Instance.OpenCashStore(0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 0);

		s_MiracleShopOpened = true;
		gameObject.SetActiveRecursively( false);
	}

	void OnCanceled()
	{
		gameObject.SetActiveRecursively( true);
	}

	void OnResurrectionConfirmed()
	{
		AsCommonSender.SendResurrection();
		Destroy( gameObject.transform.parent.gameObject);
		
		if( null != AsEntityManager.Instance.UserEntity)
			AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_BuffRefresh( PlayerBuffMgr.Instance.CharBuffDataList));
	}

	public static void MiracleShopClosed()
	{
		if( s_MiracleShopOpened == true)
		{
			s_MiracleShopOpened = false;
			if(instance != null)
				instance.gameObject.SetActiveRecursively( true);
		}
	}

	public static void SetResurrectCnt(byte[] _packet)
	{
		body_SC_RESURRECTION_CNT cnt = new body_SC_RESURRECTION_CNT();
		cnt.PacketBytesToClass(_packet);

		s_ResurrectCnt = cnt.nResurrectionCnt;

//		Debug.Log("s_ResurrectCnt = " + s_ResurrectCnt);
	}

	public static void SetResurrectCnt(int _cnt)
	{
		s_ResurrectCnt = _cnt;

//		Debug.Log("s_ResurrectCnt = " + s_ResurrectCnt);
	}

	public long GetResurrectMiracle()
	{
		int cnt = s_ResurrectCnt + 1;
		string str = "Revival_Miracle_Cost";
		if(cnt > 10) cnt = 10;
		str += cnt.ToString();

		return (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(str).Value;
	}

	void OnShopBtnClicked(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( true == AsPvpManager.Instance.CheckMatching() || true == AsPvpManager.Instance.CheckInArena())
				return;

			if( true == AsInstanceDungeonManager.Instance.CheckMatching() || true == AsInstanceDungeonManager.Instance.CheckInIndun())
				return;

			AsHudDlgMgr.Instance.OpenCashStore(0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS), m_CashSotreMenu, eCashStoreSubCategory.POTION, 0);
			s_MiracleShopOpened = true;
			gameObject.SetActiveRecursively( false);
		}
	}
}
