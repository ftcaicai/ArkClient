using UnityEngine;
using System.Collections;

public class UIPetManageSlot : MonoBehaviour {

	#region - serialized -
	// - top text -
	[SerializeField] SpriteText txt_Name;
	[SerializeField] SpriteText txt_Level;

	// - icon -
	[SerializeField] UIPetImg trnIcon;

	// - selection -
	[SerializeField] GameObject img_Selected;

	// - bg status -
	[SerializeField] GameObject img_SlotDisable;
	[SerializeField] GameObject img_SlotSummon;
	[SerializeField] GameObject img_SlotBase;
	[SerializeField] GameObject img_SlotEmpty;
	[SerializeField] GameObject img_SlotLocked;
	[SerializeField] GameObject img_SlotTarget;
	[SerializeField] GameObject img_SlotPossible;
	#endregion
	#region - member -
	public enum eState {NONE = 0, Selected, Disable, Base, Empty, Locked, Target, Possible}
	eState m_State = eState.NONE;

	PetListElement m_Element; public PetListElement Element{get{return m_Element;}}

	dltManageSlotClicked m_DltClick;
	#endregion
	#region - init & release -
	void Awake()
	{
	}

	System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
	public void SetPet(PetListElement _element, dltManageSlotClicked _dlt)
	{
		img_SlotEmpty.SetActive(false);

		m_Element = _element;
		img_SlotBase.SetActive(true);

		m_State = eState.Base;
		img_Selected.SetActive(false);

		if(AsPetManager.Instance.PetInfoDlg != null)
		{
			PetListElement curElement = AsPetManager.Instance.CurPetListElement;
			if(curElement != null && curElement.PetSlot == m_Element.PetSlot)
			{
				m_State = eState.Selected;
				img_Selected.SetActive(true);
			}
		}

		if(AsPetManager.Instance.CheckPetSummoning(m_Element) == true)
		{
			img_SlotBase.SetActive(false);
			img_SlotSummon.SetActive(true);
		}

		trnIcon.SetSlotImg(_element.PetRecord.Icon);
		txt_Name.Text = _element.PetName;

		int maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value;
		strBuilder.Remove(0, strBuilder.Length);
		strBuilder.Append("LV. ");
		strBuilder.Append(_element.Level);
		strBuilder.Append(" / ");
		strBuilder.Append(maxLv);
		txt_Level.Text = strBuilder.ToString();
//		if(_element.Level == maxLv) strBuilder.Append(" ");

		m_DltClick = _dlt;
	}
	
	void Start()
	{
		#region - basic -
//		btn_Closing.SetInputDelegate( Dlt_Close);
//		btn_Left.SetInputDelegate( Dlt_Left);
//		btn_Right.SetInputDelegate( Dlt_Right);
//		
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_Title);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_SubDesc);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_page);
		#endregion
		
//		AsHudDlgMgr.Instance.PetDlgOpened();
	}
	#endregion
	#region - call back -
	void OnMouseUpAsButton()
	{
		switch(m_State)
		{
		case eState.Base:
		case eState.Possible:
			m_DltClick(this);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			break;
		case eState.Locked:
			AsPetManager.Instance.UnLockSlot();
			break;
		default:
			Debug.Log("UIPetManageSlot:: OnMouseUpAsButton: input is ignored. m_State = " + m_State);
			break;
		}
	}

	void UIPetManageSlot_UIPetImg_Clicked()
	{
		OnMouseUpAsButton();
	}
	#endregion
	#region - method -
	void DeactivateOtherImg(eState _state)
	{
		switch(_state)
		{
		case eState.Disable:
			img_SlotBase.SetActive(false); img_SlotEmpty.SetActive(false); img_SlotLocked.SetActive(false); img_SlotTarget.SetActive(false); img_SlotPossible.SetActive(false);
			break;
		case eState.Base:
			img_SlotDisable.SetActive(false); img_SlotEmpty.SetActive(false); img_SlotLocked.SetActive(false); img_SlotTarget.SetActive(false); img_SlotPossible.SetActive(false);
			break;
		case eState.Empty:
			img_SlotDisable.SetActive(false); img_SlotBase.SetActive(false); img_SlotLocked.SetActive(false); img_SlotTarget.SetActive(false); img_SlotPossible.SetActive(false);
			break;
		case eState.Locked:
			img_SlotDisable.SetActive(false); img_SlotBase.SetActive(false); img_SlotEmpty.SetActive(false); img_SlotTarget.SetActive(false); img_SlotPossible.SetActive(false);
			break;
		case eState.Target:
			img_SlotDisable.SetActive(false); img_SlotBase.SetActive(false); img_SlotEmpty.SetActive(false); img_SlotLocked.SetActive(false); img_SlotPossible.SetActive(false);
			break;
		case eState.Possible:
			img_SlotDisable.SetActive(false); img_SlotBase.SetActive(false); img_SlotEmpty.SetActive(false); img_SlotLocked.SetActive(false); img_SlotTarget.SetActive(false);
			break;
		}
	}
	#endregion
	#region - delegate -
//	void Dlt_Close(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//		}
//	}
//	
//	void Dlt_Left(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//		}
//	}
//	
//	void Dlt_Right(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//		}
//	}
	#endregion
	#region - public -
	public void Set_Selected()
	{
		m_State = eState.Selected;

		img_Selected.SetActive(true);
	}

	public void Set_UnSelected()
	{
		m_State = eState.Base;
		
		img_Selected.SetActive(false);
	}

	public void Set_Disable()
	{
		m_State = eState.Disable;

		img_SlotDisable.SetActive(true);

		img_Selected.SetActive(false);

		img_SlotSummon.SetActive(false);

		DeactivateOtherImg(m_State);
	}

	public void Recover_Base()
	{
		m_State = eState.Base;

		if(AsPetManager.Instance.CheckPetSummoning(m_Element) == true)
			img_SlotSummon.SetActive(true);
		else
			img_SlotBase.SetActive(true);

		img_Selected.SetActive(false);

		DeactivateOtherImg(m_State);
	}

	public void Set_Lock()
	{
		m_State = eState.Locked;

		trnIcon.DeleteSlotImg();
		txt_Name.Text = "";
		txt_Level.Text = "";

		img_SlotLocked.SetActive(true);

		img_Selected.SetActive(false);

		DeactivateOtherImg(m_State);
	}

	public void Set_Empty()
	{
		m_State = eState.Empty;

		m_Element = null;

		trnIcon.DeleteSlotImg();
		txt_Name.Text = "";
		txt_Level.Text = "";
		
		img_SlotEmpty.SetActive(true);

		img_Selected.SetActive(false);

		img_SlotSummon.SetActive(false);

		DeactivateOtherImg(m_State);
	}

	public void Set_Target()
	{
		m_State = eState.Target;

		img_SlotTarget.SetActive(true);

		DeactivateOtherImg(m_State);
	}

	public void Set_Possible()
	{
		m_State = eState.Possible;

		img_SlotPossible.SetActive(true);

		img_Selected.SetActive(false);

		DeactivateOtherImg(m_State);
	}

	public void Clear()
	{
		m_State = eState.NONE;

		trnIcon.DeleteSlotImg();
		txt_Name.Text = "";
		txt_Level.Text = "";
		
		img_SlotDisable.SetActive(false);
		img_SlotBase.SetActive(false);
		img_SlotEmpty.SetActive(false);
		img_SlotLocked.SetActive(false);
		img_SlotTarget.SetActive(false);
		img_SlotPossible.SetActive(false);
	}

	public void UpgradeDlgOpened(PetListElement _baseSlot, PetListElement _matterSlot)
	{
		if(m_Element == null)
			return;

		if(_baseSlot.PetSlot == m_Element.PetSlot)
			Set_Selected();
		else if(m_State == eState.Base || m_State == eState.Possible)
		{
			if(_baseSlot.CheckUpgradable(m_Element) == true)
			{
				if(AsPetManager.Instance.CheckPetSummoning(m_Element) == true)
					Set_Disable();
				else
					Set_Possible();
			}
			else
				Set_Disable();
		}

		if(_matterSlot != null)
		{
			if(_matterSlot.PetSlot == m_Element.PetSlot)
				Set_Target();
		}
	}

	public void UpgradeDlgClosed()
	{
		switch(m_State)
		{
		case eState.Selected:
		case eState.Possible:
		case eState.Disable:
		case eState.Target:
			Recover_Base();
			break;
		}
	}

	public void UpgradeDlgReset()
	{
		if(m_Element == null)
			return;

		if(m_Element.Level == (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value)
			Set_Possible();
	}

	public void OtherSlotSelectedAsMatter(UIPetManageSlot _slot)
	{
		if(m_Element == null)
			return;

		if(_slot.Element.PetSlot == m_Element.PetSlot)
			Set_Target();
		else if(m_State == eState.Target)
			Set_Disable();
	}
	#endregion
}
