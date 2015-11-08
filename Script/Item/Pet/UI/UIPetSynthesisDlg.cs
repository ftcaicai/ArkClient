using UnityEngine;
using System.Collections;

public class UIPetSynthesisDlg : MonoBehaviour {

	#region - serialized -
	[SerializeField] GameObject parent_;
	
	// - title -
	[SerializeField] SpriteText text_Title;
	[SerializeField] UIButton btn_Closing;

	// - ui -
	[SerializeField] SpriteText txt_Index;
	[SerializeField] UIPetImg slot0;
	[SerializeField] Animation effect;
	[SerializeField] SpriteText txt_SubDesc;

	[SerializeField] UIProgressBar gauge_Base;
	[SerializeField] UIProgressBar gauge_Add;
	[SerializeField] SpriteText txt_gauge;

	// - slots -
	[SerializeField] UIInvenSlot[] slots;
	[SerializeField] SpriteText[] slotExp;

	// - gauge -
	[SerializeField] UIProgressBar gauge_Normal;

	// - under buttons -
	[SerializeField] UIButton btn_Reset;
	[SerializeField] SpriteText txt_Reset;
	[SerializeField] UIButton btn_Confirm;
	[SerializeField] SpriteText txt_Confirm;
	#endregion
	#region - member -
	bool m_ItemRegistered = false;
	#endregion
	#region - init & release -
	void Awake()
	{
		text_Title.Text = AsTableManager.Instance.GetTbl_String(2752);
		txt_Index.Text = AsTableManager.Instance.GetTbl_String(2771);

		txt_Reset.Text = AsTableManager.Instance.GetTbl_String(2768);
		txt_Confirm.Text = AsTableManager.Instance.GetTbl_String(2772);
	}

	public void Open(bool _recvResult = false)
	{
		if(_recvResult == true)
			slot0.GetComponentInChildren<Animation>().Play();

		m_TotalIncrement = 0;

		btn_Reset.renderer.material.color = Color.white;

		ConfirmButtonProc();

		ReleaseSlots();
		
		PetInfo curInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
		if(curInfo == null)
		{
			Debug.LogError("UIPetSynthesisDlg:: GaugeProc: no pet info is found. AsPetManager.Instance.CurDisplayedPetInfo == null");
			return;
		}
		
		slot0.SetSlotImg(curInfo.GetPetRecord().Icon);
		
		InitInfos();
	}
	
	void Start()
	{
		btn_Closing.SetInputDelegate(Dlt_Close);
		btn_Reset.SetInputDelegate(Dlt_Reset);
		btn_Confirm.SetInputDelegate(Dlt_Confirm);
	}
	#endregion
	#region - public -
	public void Close()
	{
		ReleaseSlots();

		Destroy( parent_);
	}

	public bool SetClickRealItem( RealItem _realItem )
	{
		if( true == AsPetManager.Popup)
			return false;

		if(AsPetManager.Instance.PacketSend == true)
			return false;

		if(m_NowUpgrading == true)
			return false;

		if( null == _realItem )
			return false;

		if( false == IsCheckItemType( _realItem ) )
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2773), 
			                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return false;
		}

		if(CheckItemRegistable(_realItem) == false)
		{
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2779), 
			                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			return false;
		}

		
		if(AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg != null)
		{
			PetInfo petInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
			if(petInfo.nLevel == (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value)
			{
				AsMessageBox msgBox = null;
				msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2760),
				                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				msgBox.gameObject.AddComponent<UIPetPopup>();
				return false;
			}
		}

		for(int i=0; i<slots.Length; ++i)
		{
			UIInvenSlot slot = slots[i];
			if(slot.slotItem == null)
			{
				SetItemInSlot( slot, _realItem );
				slotExp[i].Text = _realItem.item.ItemData.m_AbsorbExp + "EXP";
				break;
			}
		}

		GaugeProc();
		ConfirmButtonProc();
		
		return false;
	}

	public bool IsRect(Ray _ray)
	{
		if(collider != null && collider.bounds.IntersectRay(_ray) == true)
			return true;
//		else if(slot0.collider != null && slot0.collider.bounds.IntersectRay(_ray) == true)
//			return true;
//		else
//		{
//			for(int i=0; i<slots.Length; ++i)
//			{
//				if(slots[i].collider != null && slots[i].collider.bounds.IntersectRay(_ray) == true)
//					return true;
//			}
//		}
		
		return false;
	}
	#endregion
	#region - method -
	bool IsCheckItemType( RealItem _realItem )
	{
//		if( Item.eITEM_TYPE.EquipItem != _realItem.item.ItemData.GetItemType() )
//			return false;
//		
//		if( Item.eUSE_TIME_TYPE.NONE != _realItem.item.ItemData.m_eUseTimeType )
//			return false;
		
		if( _IsSameRealItem( _realItem ) )
		{
			return false;
		}

		if(_realItem.item.ItemData.m_AbsorbExp <= 0)
			return false;
		
		return true;
	}
	bool _IsSameRealItem( RealItem _realItem )
	{
		for(int i=0; i<slots.Length; ++i)
		{
			UIInvenSlot slot = slots[i];
			if(slot.slotItem != null && (slot.slotItem.realItem.getSlot == _realItem.getSlot))
				return true;
		}

		return false;
	}

	void SetItemInSlot( UIInvenSlot _slot, RealItem _realItem )
	{
		if( null == _slot )
			return;
		
		if( null == _realItem )
			return;
		
		_slot.DeleteSlotItem();
		_slot.CreateSlotItem( _realItem, _slot.transform );
		_slot.ResetSlotItemLocalPosition(-0.5f);
		ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _realItem.getSlot, true );
//		if( AsHudDlgMgr.Instance.IsOpenInven )
//		{
//			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
//		}
	}

	int m_TotalIncrement;
	void GaugeProc()
	{
		PetInfo curInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
		if(curInfo == null)
		{
			Debug.LogError("UIPetSynthesisDlg:: GaugeProc: no pet info is found. AsPetManager.Instance.CurDisplayedPetInfo == null");
			return;
		}

		int curExp = curInfo.nExp;

		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord(curInfo.nPetTableIdx);
		int maxExp = petRec.GetNextMaxExp(curInfo.nLevel);
		int exceptExp = petRec.GetPrevMaxExp(curInfo.nLevel);

		curExp -= exceptExp;
		maxExp -= exceptExp;

		int increment = GetItemIncrement();
		m_TotalIncrement = curExp + increment;

		gauge_Base.Value = (float)curExp / (float)maxExp;
		gauge_Add.Value = (float)(curExp + increment) / (float)maxExp;

		string incText = "";
		if(increment != 0)
			incText = "(" + "+" + increment + ")";

		txt_gauge.Text = curExp + incText + " / " + maxExp;
	}

	void ConfirmButtonProc()
	{
		m_ItemRegistered = false;

		for(int i=0; i<slots.Length; ++i)
		{
			UIInvenSlot slot = slots[i];
			if(slot.slotItem != null)
			{
				m_ItemRegistered = true;
				break;
			}
		}

		if(m_ItemRegistered == true)
			btn_Confirm.renderer.material.color = Color.white;
		else
			btn_Confirm.renderer.material.color = Color.gray;
	}

	bool CheckItemRegistable(RealItem _realItem)
	{
		PetInfo curInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
		if(curInfo == null)
		{
			Debug.LogError("UIPetSynthesisDlg:: GaugeProc: no pet info is found. AsPetManager.Instance.CurDisplayedPetInfo == null");
			return false;
		}

		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord(curInfo.nPetTableIdx);
		int maxExp = petRec.GetNextMaxExp(curInfo.nLevel);
		int exceptExp = petRec.GetPrevMaxExp(curInfo.nLevel);

		maxExp -= exceptExp;

		int maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value;

		bool firstRegistable = !(curInfo.nLevel == maxLv - 1 && m_TotalIncrement > maxExp);
		if(firstRegistable == true)
		{
			if(curInfo.nExp + GetItemIncrement() <= petRec.GetNextMaxExp(maxLv))
				return true;
		}

		return false;
	}

	int GetItemIncrement()
	{
		int increment = 0;
		for(int i=0; i<slots.Length; ++i)
		{
			UIInvenSlot slot = slots[i];
			if(slot.slotItem != null)
			{
				increment += slot.slotItem.realItem.item.ItemData.m_AbsorbExp;
			}
		}

		return increment;
	}

	void ReleaseSlots()
	{
		for(int i=0; i<slots.Length; ++i)
		{
			UIInvenSlot slot = slots[i];
			if(slot.slotItem != null)
			{
				ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(slot.slotItem.realItem.getSlot, false);
				slot.DeleteSlotItem();
			}

			slotExp[i].Text = "";
		}

		if( AsHudDlgMgr.Instance.IsOpenInven ) AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
	}

	void InitInfos()
	{
		GaugeProc();
		ConfirmButtonProc();
	}
	#endregion
	#region - delegate -
	void Dlt_Close(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_NowUpgrading == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			ReleaseSlots();

			Close();
		}
	}
	
	void Dlt_Reset(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_NowUpgrading == true)
				return;

			m_TotalIncrement = 0;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			ReleaseSlots();
			InitInfos();
		}
	}
	
	void Dlt_Confirm(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_NowUpgrading == true)
				return;

			if(AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg != null)
			{
				PetInfo petInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
				if(petInfo.nLevel == (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value)
				{
					AsMessageBox msgBox = null;
					msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2760),
					                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					msgBox.gameObject.AddComponent<UIPetPopup>();
					return;
				}
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if(m_ItemRegistered == true)
				StartCoroutine(CR_Confirm());
		}
	}
	bool m_NowUpgrading = false;
	IEnumerator CR_Confirm()
	{
		m_NowUpgrading = true;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6012_EFF_StrengthenProgress", Vector3.zero, false);
		
		btn_Reset.renderer.material.color = Color.gray;
		btn_Confirm.renderer.material.color = Color.gray;
		
		float totalTime = 2f;
		float ratio = 0f;
		while(true)
		{
			yield return new WaitForEndOfFrame();
			
			ratio += Time.deltaTime;
			gauge_Normal.Value = ratio / totalTime;
			
			if(ratio > totalTime)
			{
				gauge_Normal.Value = 0f;
				break;
			}
		}
		
		PetInfo curInfo = AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg;
		if(curInfo == null)
		{
			Debug.LogError("UIPetSynthesisDlg:: GaugeProc: no pet info is found. AsPetManager.Instance.CurDisplayedPetInfo == null");
			yield break;
		}
		
		int[] indices = new int[3]{-1, -1, -1};
		for(int i=0; i<3; ++i)
		{
			if(slots[i].slotItem != null && slots[i].slotItem.realItem != null)
				indices[i] = slots[i].slotItem.realItem.getSlot;
		}
		
		AsPetManager.Instance.Send_Item_Compose(curInfo.nPetUniqSlot,
		                                        indices[0], indices[1], indices[2], Dlt_SendComplete);

		m_NowUpgrading = false;
	}
	void Dlt_SendComplete()
	{
		m_NowUpgrading = false;
	}
	#endregion
}
