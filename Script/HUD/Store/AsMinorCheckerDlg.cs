using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class AsMinorCheckInfo
{
	
	//private string fileName = "/MinorCheckInfo.dat";
	private bool bLoaded = false;
	public string version = string.Empty;
	public int year;
	public int month;
	public int day;

	public bool SaveFile()
	{
		PlayerPrefs.SetString("MinorVersion", VersionManager.version);
		PlayerPrefs.SetInt("MinorYear", DateTime.Now.Year);
		PlayerPrefs.SetInt("MinorMonth", DateTime.Now.Month);
		PlayerPrefs.SetInt("MinorDay", DateTime.Now.Day);

		return true;
	}

	public bool LoadFile()
	{
		this.version= PlayerPrefs.GetString("MinorVersion") == null ? "0" : PlayerPrefs.GetString("MinorVersion");
		this.year	= PlayerPrefs.GetInt("MinorYear") == null ? 0 : PlayerPrefs.GetInt("MinorYear");
		this.month	= PlayerPrefs.GetInt("MinorMonth") == null ? 0 : PlayerPrefs.GetInt("MinorMonth");
		this.day	= PlayerPrefs.GetInt("MinorDay") == null ? 0 : PlayerPrefs.GetInt("MinorDay");

		if (version == "0" || year == 0 || month == 0 || day == 0)
			return false;

		bLoaded = true;

		return true;
	}

	//public void DeleteFile()
	//{
	//    File.Delete(Application.persistentDataPath + fileName);
	//}

	public bool CheckMinorInfo()
	{
		if (bLoaded == false)
		{
			Debug.LogWarning("Not Loaded");
			return false;
		}

		if (VersionManager.version != version || DateTime.Now.Year != year ||
			DateTime.Now.Month != month || DateTime.Now.Day != day)
		{
			return false;
		}

		return true;
	}
}

public enum eMinorChckDlgState
{
	STEP1,
	STEP2,
	STEP3,
}

public class AsMinorCheckerDlg : MonoBehaviour {

	public SpriteText	txtTitle;
	public SpriteText	txtContents;
	public UIButton		btnConfirm;
	public UIButton		btnCancel;
	public UIButton		btnClose;
	public UIRadioBtn   btnNoMoreExpose;
	public eMinorChckDlgState nowState = eMinorChckDlgState.STEP1;

	private bool isNormalOpen = true;
	private bool isChecked = true;
	private bool toMiracle = false;
	private eCLASS usrClass = eCLASS.NONE;
	private eCashStoreMenuMode menuMode = eCashStoreMenuMode.NONE;
	private eCashStoreSubCategory category = eCashStoreSubCategory.NONE;
	private int idx = 0;

	void Start () 
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtContents);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnConfirm.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnCancel.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(btnNoMoreExpose.spriteText);

		txtTitle.Text = AsTableManager.Instance.GetTbl_String(2327);

		btnConfirm.SetInputDelegate(Confirm);
		btnCancel.SetInputDelegate(Cancel);
		btnClose.SetInputDelegate(Close);
		btnNoMoreExpose.Value = true;

	}

	public void Show(bool _normalOpen, eCLASS _class, eCashStoreMenuMode _mode, eCashStoreSubCategory _category, int _idx, bool _toMiralce = false)
	{
		usrClass = _class;
		isNormalOpen = _normalOpen;
		menuMode = _mode;
		category = _category;
		idx = _idx;
		toMiracle = _toMiralce;

		Step1();
	}

	void Step1()
	{
		nowState = eMinorChckDlgState.STEP1;

		btnConfirm.gameObject.SetActive(true);
		btnCancel.gameObject.SetActive(true);
		btnNoMoreExpose.gameObject.SetActive(true);

		txtContents.Text = AsTableManager.Instance.GetTbl_String(2328);
		btnConfirm.Text = AsTableManager.Instance.GetTbl_String(2333);
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(2334);
		btnNoMoreExpose.Text = AsTableManager.Instance.GetTbl_String(2332);
	}

	void Step2()
	{
		nowState = eMinorChckDlgState.STEP2;

		txtContents.Text = AsTableManager.Instance.GetTbl_String(2329);
		
		btnConfirm.Text = AsTableManager.Instance.GetTbl_String(2333);
		btnNoMoreExpose.gameObject.SetActive(false);
	}

	void Step3()
	{
		nowState = eMinorChckDlgState.STEP3;

		txtContents.Text = AsTableManager.Instance.GetTbl_String(2330);
		btnConfirm.Text = AsTableManager.Instance.GetTbl_String(1152);

		btnNoMoreExpose.gameObject.SetActive(false);
		btnCancel.gameObject.SetActive(false);
		btnConfirm.transform.localPosition = new Vector3(txtContents.transform.localPosition.x, btnConfirm.transform.localPosition.y, btnConfirm.transform.localPosition.z);
	}

	void OnCheck()
	{
		if (isChecked == true)
			btnNoMoreExpose.Value = isChecked = false;
		else
			btnNoMoreExpose.Value = isChecked = true;
	}

	void Confirm(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;

		if (nowState == eMinorChckDlgState.STEP1)
		{
			if (btnNoMoreExpose.Value == true)
			{
				AsMinorCheckInfo checker = new AsMinorCheckInfo();
				bool saved = checker.SaveFile();
			}

			OpenCashShop();
			CloseCore();
		}
		else if (nowState == eMinorChckDlgState.STEP2)
		{
			OpenCashShop();
			CloseCore();
		}
		else if (nowState == eMinorChckDlgState.STEP3)
		{
			AsDeathDlg.MiracleShopClosed();//$yde
			CloseCore();
		}

	}

	void Cancel(ref POINTER_INFO ptr)
	{
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.TAP)
			return;


		if (nowState == eMinorChckDlgState.STEP1)
		{
			Step2();
		}
		else if (nowState == eMinorChckDlgState.STEP2)
		{
			Step3();
		}
	}

	void Close(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsDeathDlg.MiracleShopClosed();//$yde
			CloseCore();
		}
	}

	void CloseCore()
	{
		if (AsHudDlgMgr.Instance != null)
			if (AsHudDlgMgr.Instance.IsOpenCashStore == true)
				AsHudDlgMgr.Instance.cashStore.SetNotTouch();

		GameObject.Destroy(gameObject);
	}

	void OpenCashShop()
	{
		if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
		{
			if (AsHudDlgMgr.Instance.IsOpenCashStore == false)
				AsHudDlgMgr.Instance.OpenCashStore(0, usrClass, menuMode, category, idx, true);
			else
			{

				if (AsHudDlgMgr.Instance.cashStore != null)
					AsHudDlgMgr.Instance.cashStore.GoToMenu(eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, true, false, true);
			}
		}
		else if (AsGameMain.s_gameState == GAME_STATE.STATE_CHARACTER_SELECT)
		{
			AsCashStore.CreateCashStoreForMiracle();
		}
	}

}
