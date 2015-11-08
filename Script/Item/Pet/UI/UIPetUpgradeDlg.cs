using UnityEngine;
using System.Collections;

public class UIPetUpgradeDlg : MonoBehaviour {

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
	[SerializeField] SpriteText txt_Gold;

	// - slots -
	[SerializeField] SpriteText txt_base;
	[SerializeField] SpriteText txt_matter;
	[SerializeField] UIPetUpgradeSlot slot1;
	[SerializeField] UIPetUpgradeSlot slot2;

	// - gauge -
	[SerializeField] UIProgressBar gauge_Normal;
	
	// - under buttons -
	[SerializeField] UIButton btn_Reset;
	[SerializeField] SpriteText txt_Reset;
	[SerializeField] UIButton btn_Confirm;
	[SerializeField] SpriteText txt_Confirm;
	#endregion
	#region - member -
	PetListElement m_BaseElement; public PetListElement BaseElement{get{return m_BaseElement;}}
	PetListElement m_MatterElement; public PetListElement MatterElement{get{return m_MatterElement;}}

	UIPetManageDlg PetManageDlg{get{return AsPetManager.Instance.PetManageDlg;}}
	#endregion
	#region - init & release -
	void Awake()
	{
		text_Title.Text = AsTableManager.Instance.GetTbl_String(2761);
		txt_Index.Text = AsTableManager.Instance.GetTbl_String(2762);

		txt_Reset.Text = AsTableManager.Instance.GetTbl_String(2768);
		txt_Confirm.Text = AsTableManager.Instance.GetTbl_String(2761);

		txt_base.Text = AsTableManager.Instance.GetTbl_String(2763);
		txt_matter.Text = AsTableManager.Instance.GetTbl_String(2764);
	}

	public void Open(PetListElement _element)
	{
		Tbl_Pet_Record nextPet = AsTableManager.Instance.GetPetRecord(_element.PetRecord.UpgradeID);
		slot0.SetSlotImg(nextPet.Icon);
		
		switch(_element.PetRecord.StarGrade)
		{
		case 1:
			txt_SubDesc.Text = AsTableManager.Instance.GetTbl_String(2765);
			break;
		case 2:
			txt_SubDesc.Text = AsTableManager.Instance.GetTbl_String(2766);
			break;
		case 3:
			txt_SubDesc.Text = AsTableManager.Instance.GetTbl_String(2765);
			break;
		case 4:
			txt_SubDesc.Text = AsTableManager.Instance.GetTbl_String(2767);
			break;
		}
		
		m_MatterElement = null;
		slot2.Clear();
		
		m_BaseElement = _element;
		slot1.SetSlot(_element);
		
		if(PetManageDlg == null)
		{
			Debug.LogError("UIPetUpgradeDlg:: Open: there is no PetManageDlg");
			return;
		}
		
		m_NeedGold = (ulong)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMixStar_" + _element.PetRecord.StarGrade).Value;
		txt_Gold.Text = m_NeedGold.ToString();
		
		ConfirmButtonProc();

		gauge_Normal.Value = 0f;
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
		Destroy( parent_);
		
		if(PetManageDlg != null)
			PetManageDlg.RefreshSlotsOnUpgradeDlgClose();
	}

	public void ManageSlotClicked(UIPetManageSlot _manageSlot)
	{
		if(m_NowUpgrading == true)
			return;

		if(m_BaseElement == null)
		{
			Open(_manageSlot.Element);

			if(PetManageDlg != null)
				PetManageDlg.RefreshSlotsOnUpgradeDlgOpen();
		}
		else
			SetMatter(_manageSlot.Element);
	}
	#endregion
	#region - method -
	bool m_Confirmed = false;
	void ConfirmButtonProc()
	{
		if(m_BaseElement != null && m_MatterElement != null &&
		   m_NeedGold <= AsUserInfo.Instance.SavedCharStat.nGold)
		{
			m_Confirmed = true;

			txt_Gold.Text = m_NeedGold.ToString();
			btn_Confirm.renderer.material.color = Color.white;
		}
		else
		{
			m_Confirmed = false;

			btn_Confirm.renderer.material.color = Color.gray;
		}

		if(m_NeedGold > AsUserInfo.Instance.SavedCharStat.nGold)
			txt_Gold.Text = Color.red + m_NeedGold.ToString();
	}

	void SetMatter(PetListElement _element)
	{
		m_MatterElement = _element;
		
		slot2.SetSlot(_element);
		
		if(PetManageDlg == null)
		{
			Debug.LogError("UIPetUpgradeDlg:: Open: there is no PetManageDlg");
			return;
		}
		
		PetManageDlg.SetMatterInUpgrade(_element);
		
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

			Close();
		}
	}

	void Dlt_Reset(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_NowUpgrading == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if(PetManageDlg != null)
				PetManageDlg.SetSlotToPossible(m_MatterElement);

//			m_BaseElement = null;
			m_MatterElement = null;

//			slot0.DeleteSlotImg();
//			slot1.Clear();
			slot2.Clear();

			ConfirmButtonProc();

			if(PetManageDlg != null)
				PetManageDlg.RefreshSlotsOnUpgradeDlgOpen();
		}
	}

	ulong m_NeedGold = 0;
	void Dlt_Confirm(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_Confirmed == false)
				return;

			if(m_NowUpgrading == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

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

		AsPetManager.Instance.Send_Upgrade(m_BaseElement, m_MatterElement, Dlt_SendComplete);

		m_NowUpgrading = false;

		Close();
	}
	void Dlt_SendComplete()
	{
		m_NowUpgrading = false;
	}
	#endregion
}

public delegate void dltUpgradeSlotClicked(UIPetUpgradeSlot _slot);

