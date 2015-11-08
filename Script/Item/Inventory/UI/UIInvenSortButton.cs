using UnityEngine;
using System.Collections;

public class UIInvenSortButton : MonoBehaviour 
{
	public AsIconCooltime coolTime;	
	public UIButton sortBtn;  
	private float m_fMaxCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f;
	
	
	// cool time
	private void SetCoolTime( float fMaxValue, float fRemainCooltime )
	{
		if( null == coolTime ) 
		{
			Debug.LogError("ResetDataButton::SetCoolTime() [ null == coolTime");
			return;
		}
		
		if( true == coolTime.gameObject.active )
		{
			return;
		}
		else
		{
			coolTime.gameObject.active = true;
			if( null != sortBtn )
				sortBtn.controlIsEnabled = false;
		}		
		
		m_fRemainCoolTime = fRemainCooltime;
		m_fMaxCoolTime = fMaxValue;
		
		ResetCoolTime();
	}
	
	
	private void ResetCoolTime()
	{
		if (0.0f > m_fRemainCoolTime)
		{
			m_fRemainCoolTime = 0.0f;
			coolTime.gameObject.active = false;	
			if( null != sortBtn )
				sortBtn.controlIsEnabled = true;
		}

		coolTime.Value = (m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;	
	}
	
	// input
	
//	public void GuiInputUp(Ray inputRay, int iPage)
//	{
//        if (((4+ItemMgr.HadItemManagement.Inven.getExtendPageCount) / 5) < iPage)
//        {
//            return;
//        }
//
//		if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//		{
//			Debug.Log(" true == AsHudDlgMgr.Instance.IsDontMoveState ");
//			return;
//		}
//		if( true == coolTime.gameObject.active )
//		{
//			return;
//		}
//		
//		if( null == collider )
//		{
//			Debug.LogError("UIInvenSortButton::GuiInputUp() [ null == collider ] " );
//			return;
//		}
//		
//		if( true == collider.bounds.IntersectRay( inputRay ) )
//		{
//			AsCommonSender.SendInvenPageSort( iPage );
//			SetCoolTime( 30f, 30f );
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6006_EFF_ItemSort", Vector3.zero, false);
//		}
//	}
	
	private void SortBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( false == AsHudDlgMgr.Instance.IsOpenInven )
				return;
			
			if( AsHudDlgMgr.Instance.IsOpenReNameDlg == true )
				return;
			
			if( null != AsHudDlgMgr.Instance && true == AsHudDlgMgr.Instance.IsOpenRandItemUI && true == AsHudDlgMgr.Instance.randItemUI.isCanUseDlg )
			{
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				return;
			}
			
			if( null != AsHudDlgMgr.Instance && true == AsHudDlgMgr.Instance.isOpenRandomItemDlg )
			{
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				return;
			}
			
			if( null != AsHudDlgMgr.Instance && 
				(true == AsHudDlgMgr.Instance.IsOpenSynCosDlg ||
				 true == AsHudDlgMgr.Instance.IsOpenSynDisDlg ||
				 true == AsHudDlgMgr.Instance.IsOpenSynEnchantDlg ||
				 true == AsHudDlgMgr.Instance.IsOpenSynOptionDlg ) )
			{
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				return;
			}

			if( null != AsPetManager.Instance.PetSynthesisDlg)//$yde
			{
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
				                                                              null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				return;
			}
			
			int iPage = AsHudDlgMgr.Instance.invenDlg.page.curPage;
			if (((4+ItemMgr.HadItemManagement.Inven.getExtendPageCount) / 5) < iPage)
	        {
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
	            return;
	        }
	
			if( true == AsHudDlgMgr.Instance.IsDontMoveState)
			{
				if( true == AsHudDlgMgr.Instance.IsOpenNpcStore )
					AsHudDlgMgr.Instance.npcStore.LockInput( false );
				
				AsHudDlgMgr.Instance.SetMsgBox(  AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(1646),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE) );
				return;
			}
			if( true == coolTime.gameObject.active )
			{
				return;
			}					
			
			AsCommonSender.SendInvenPageSort( iPage );
			SetCoolTime( 30f, 30f );
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6006_EFF_ItemSort", Vector3.zero, false);
						
		}
	}
	
	
	// inven reset button
	public void Init()
	{
		if( null == coolTime ) 
		{
			Debug.LogError("ResetDataButton::Init() [ null == coolTime");
			return;
		}
		coolTime.gameObject.active = false;
		if( null != sortBtn )
			sortBtn.controlIsEnabled = true; 
	}
	
	void Awake()
    {
		if( null != sortBtn )
		{
			sortBtn.SetInputDelegate(SortBtnDelegate);
			sortBtn.spriteText.Text = AsTableManager.Instance.GetTbl_String(1918);	
		}
	}
	// Use this for initialization
	void Start () 
	{	
		Init();
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		if( null == coolTime )
			return;
		
		if( false == coolTime.gameObject.active )
			return;
		
		m_fRemainCoolTime -= Time.deltaTime;
		ResetCoolTime();
	}
}
