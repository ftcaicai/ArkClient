using UnityEngine;
using System.Collections;

public class AsSynthesisMainDlg : MonoBehaviour 
{
	
	public GameObject disassembleObject;
	public GameObject cosObject;
	public GameObject enchantObject;
	public GameObject optionObject;
	

	public SpriteText titleTxt;
	public UIButton strengthenBtn;
	public UIButton enchantBtn;
	public UIButton optionBtn;
	public UIButton cosBtn;
	public UIButton disassembleBtn;
	

	
	public void OpenStrengthenDlg()
	{
		if( AsHudDlgMgr.Instance.IsOpenStrengthenDlg == true )
			return;
		
		AsHudDlgMgr.Instance.OpenStrengthenDlg();
	}
	
	public void OpenEnchantDlg()
	{
		if( null == enchantObject )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenSynEnchantDlg )
			return;
		
		AsHudDlgMgr.Instance.CloseSynthesisDlg();
		
		GameObject go = GameObject.Instantiate( enchantObject ) as GameObject;
		AsHudDlgMgr.Instance.m_SynEnchantDlg = go.GetComponentInChildren<AsSynthesisEnchantDlg>();	
		AsHudDlgMgr.Instance.m_SynEnchantDlg.Open();	
		
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.OpenInven();
	}
	
	
	public void OpenOptionDlg()
	{
		if( null == optionObject )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenOptionDlg )
			return;
		
		AsHudDlgMgr.Instance.CloseSynthesisDlg();
		
		GameObject go = GameObject.Instantiate( optionObject ) as GameObject;
		AsHudDlgMgr.Instance.m_SynOptionDlg = go.GetComponentInChildren<AsSynthesisOptionDlg>();	
		AsHudDlgMgr.Instance.m_SynOptionDlg.Open();	
		
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.OpenInven();
	}
	
	public void OpenCosDlg()
	{
		if( null == cosObject )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenSynCosDlg )
			return;
		
		AsHudDlgMgr.Instance.CloseSynthesisDlg();
		
		GameObject go = GameObject.Instantiate( cosObject ) as GameObject;
		AsHudDlgMgr.Instance.m_SynCosDlg = go.GetComponentInChildren<AsSynthesisCosDlg>();	
		AsHudDlgMgr.Instance.m_SynCosDlg.Open();
		
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.OpenInven();
	}
	
	public void OpenDisassembleDlg()
	{
		if( null == disassembleObject )
			return;
		
		if( true == AsHudDlgMgr.Instance.IsOpenSynDisDlg )
			return;
		
		AsHudDlgMgr.Instance.CloseSynthesisDlg();
		
		GameObject go = GameObject.Instantiate( disassembleObject ) as GameObject;
		AsHudDlgMgr.Instance.m_SynDisDlg = go.GetComponentInChildren<AsDisassembleDlg>();	
		AsHudDlgMgr.Instance.m_SynDisDlg.Open();
		
		if( false == AsHudDlgMgr.Instance.IsOpenInven)
			AsHudDlgMgr.Instance.OpenInven();
	}
	
	public void Close()
	{
		
	}
	
	
	public void BtnClose()
	{
		AsHudDlgMgr.Instance.CloseSynthesisDlg();
	}

	private void StrengthenBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OpenStrengthenDlg();
			
		}
	}
	
	private void EnchantBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OpenEnchantDlg();
		}
	}
	
	private void OptionDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			OpenOptionDlg();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void CosBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			OpenCosDlg();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	private void DisBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			OpenDisassembleDlg();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	
	
	// Use this for initialization
	void Start () 
	{
		titleTxt.Text = AsTableManager.Instance.GetTbl_String(1220);
	
		strengthenBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(1348);
		enchantBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(2150);
		optionBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(2144);
		cosBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(1788);
		disassembleBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(1846);
		
		
		strengthenBtn.SetInputDelegate(StrengthenBtnDelegate);
		enchantBtn.SetInputDelegate(EnchantBtnDelegate);
		optionBtn.SetInputDelegate(OptionDelegate);
		cosBtn.SetInputDelegate(CosBtnDelegate);
		disassembleBtn.SetInputDelegate(DisBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void InputDown( Ray inputRay)
	{		
		
	}
	
	public void InputUp(Ray inputRay)
	{
	}
	
	public void InputMove(Ray inputRay)
	{
	}
	
	public void GuiInputDown( Ray inputRay)
	{		
	}	

	public void GuiInputMove(Ray inputRay)
	{
		
	}

	public void GuiInputUp(Ray inputRay)
	{
		
	}
	
	public void GuiInputDClickUp(Ray inputRay)
	{
		
	}
}
