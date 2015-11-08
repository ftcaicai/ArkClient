using UnityEngine;
using System.Collections;

public class AsSynthesisUpgradePopup : MonoBehaviour 
{
	[SerializeField] SpriteText 		txtTitle;
	[SerializeField] SpriteText 		txtInfo1;
	[SerializeField] SpriteText 		txtInfo2;

	[SerializeField] UIButton 		btnGold;
	[SerializeField] UIButton 		btnMiracle;
	[SerializeField] UIButton 		btnClose;

	[SerializeField] SpriteText 		txtGold;
	[SerializeField] SpriteText 		txtMiracle;

	AsSynthesisCosDlg 			cosSynthesisDlg;

	int										costGold = 0;
	int										costMiracle = 0;

	// Use this for initialization
	void Start () 
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String (2419);
		txtInfo1.Text = AsTableManager.Instance.GetTbl_String (2425);
		txtInfo2.Text = AsTableManager.Instance.GetTbl_String (2420);

		btnGold.spriteText.Text = AsTableManager.Instance.GetTbl_String (2422);
		btnGold.SetInputDelegate (GoldBtnDelegate);

		btnMiracle.spriteText.Text = AsTableManager.Instance.GetTbl_String (2421);
		btnMiracle.SetInputDelegate (MiracleBtnDelegate);

		btnClose.SetInputDelegate (CloseBtnDelegate);
	}

	public void Open( AsSynthesisCosDlg _dlg , int _gold , int _miracle )
	{
		cosSynthesisDlg = _dlg;

		txtGold.Text 		= _gold.ToString ();
		txtMiracle.Text 	= _miracle.ToString ();

		costGold = _gold;
		costMiracle = _miracle;

		if ( _miracle == 0 ) 
		{
			AsUtil.SetButtonState( btnMiracle , UIButton.CONTROL_STATE.DISABLED );
		}

		if (AsUserInfo.Instance.SavedCharStat.nGold < (ulong)_gold) 
		{
			AsUtil.SetButtonState( btnGold , UIButton.CONTROL_STATE.DISABLED );
		}
	}

	private void GoldBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6117_EFF_GoldBuy_Popup", Vector3.zero, false);

			cosSynthesisDlg.ExcuteUpgrade( costGold , 0 );

			Close ();
		}
	}

	private void MiracleBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6110_EFF_MiracleBuy_Popup", Vector3.zero, false);

			if( AsUserInfo.Instance.nMiracle < costMiracle )
			{
				string title = AsTableManager.Instance.GetTbl_String(1412);
				string content = AsTableManager.Instance.GetTbl_String(368);
				
				//KB
				if (AsGameMain.useCashShop == true)
					AsNotify.Instance.MessageBox(title, content, this, "OpenCashShop", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
				else
					AsNotify.Instance.MessageBox(title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
			}
			else
			{
				cosSynthesisDlg.ExcuteUpgrade( 0 , costMiracle );
			}

			Close ();
		}
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSynthesisCosDlg.isAuthorityButtonClick = true;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Close ();
		}
	}

	private void Close()
	{
		Destroy( transform.parent.gameObject );	
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
		}
		else
		{
			AsCashStore.CreateCashStoreForMiracle();
		}

		AsHudDlgMgr.Instance.CloseSynCosDlg();
	}


	// Update is called once per frame
	void Update () 
	{
	
	}
}
