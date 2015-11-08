using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPetManageDlg : MonoBehaviour {

	#region - serialized -
	[SerializeField] GameObject parent_;
	
	// - title -
	[SerializeField] SpriteText text_Title;
	[SerializeField] UIButton btn_Closing;
	[SerializeField] SpriteText txt_SubDesc;

	[SerializeField] UIPetManageSlot[] arSlot = new UIPetManageSlot[]{}; 

	// - page -
	[SerializeField] SpriteText txt_page;
	[SerializeField] UIButton btn_Left;
	[SerializeField] UIButton btn_Right;
	#endregion
	#region - member -
	static bool m_Popup; public static bool Popup{get{return m_Popup;}}

	List<PetListElement> listPetList{get{return AsPetManager.Instance.listPetList;}}
	Dictionary<int, UIPetManageSlot> m_dicPetManageSlot = new Dictionary<int, UIPetManageSlot>();

	public static readonly int s_MaxSlotCount = 9;
	public static readonly int s_LineSlotCount = 3;
	public static readonly int s_LineCount = 3;
	public static readonly int s_MinPage = 1;
	public static readonly int s_MaxPage = 3;
	int m_CurPage = 1;

	UIPetUpgradeDlg PetUpgradeDlg{get{return AsPetManager.Instance.PetUpgradeDlg;}}
	#endregion
	#region - init & release -
	void Awake()
	{
		// title
		text_Title.Text = AsTableManager.Instance.GetTbl_String(2740);
		if(txt_SubDesc != null) txt_SubDesc.Text = AsTableManager.Instance.GetTbl_String(2741);
	}

	public void Open()
	{
		SetSlots();

		txt_page.Text = m_CurPage.ToString() + " / " + s_MaxPage;
	}

	void Start()
	{
		#region - basic -
		btn_Closing.SetInputDelegate( Dlt_Close);
		btn_Left.SetInputDelegate( Dlt_Left);
		btn_Right.SetInputDelegate( Dlt_Right);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_Title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_SubDesc);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_page);
		#endregion

		AsHudDlgMgr.Instance.PetDlgOpened();
	}
	#endregion
	#region - method -
	void SetSlots()
	{
		m_dicPetManageSlot.Clear();
		for(int i=0; i<s_MaxSlotCount; ++i)
		{
			arSlot[i].Set_Empty();
		}

		int beginIdx = GetBeginIndexOnCurPage();
		int count = listPetList.Count - beginIdx;
		if(count > s_MaxSlotCount) count = s_MaxSlotCount;
		for(int i=0; i<count; ++i)
		{
			arSlot[i].SetPet(listPetList[i + beginIdx], Dlt_SlotClicked);
			m_dicPetManageSlot.Add(arSlot[i].Element.PetSlot, arSlot[i]);
		}

		RefreshLockedSlots();

		if(PetUpgradeDlg != null)
			RefreshSlotsOnUpgradeDlgOpen();
	}

	int GetBeginIndexOnCurPage()
	{
		return (m_CurPage - 1) * s_MaxSlotCount;
	}

	PetListElement GetUpgradeBaseElement()
	{
		PetListElement element = null;
		if(PetUpgradeDlg != null && PetUpgradeDlg.BaseElement != null)
			element = PetUpgradeDlg.BaseElement;

		return element;
	}

	PetListElement GetUpgradeMatterElement()
	{
		PetListElement element = null;
		if(PetUpgradeDlg != null && PetUpgradeDlg.MatterElement != null)
			element = PetUpgradeDlg.MatterElement;
		
		return element;
	}

	UIPetManageSlot GetManageSlot(PetListElement _element)
	{
		if(_element == null)
			return null;

		if(m_dicPetManageSlot.ContainsKey(_element.PetSlot) == true)
			return m_dicPetManageSlot[_element.PetSlot];
		else
			return null;
	}
	#endregion
	#region - delegate -
	void Dlt_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_Popup == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Close();
		}
	}

	void Dlt_Left(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			--m_CurPage;
			m_CurPage = Mathf.Clamp(m_CurPage, s_MinPage, s_MaxPage);
			txt_page.Text = m_CurPage.ToString() + " / " + s_MaxPage;

			SetSlots();
		}
	}

	void Dlt_Right(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			++m_CurPage;
			m_CurPage = Mathf.Clamp(m_CurPage, s_MinPage, s_MaxPage);
			txt_page.Text = m_CurPage.ToString() + " / " + s_MaxPage;

			SetSlots();
		}
	}

	void Dlt_SlotClicked(UIPetManageSlot _slot)
	{
		AsPetManager.Instance.PetManageSlotClicked(_slot);
	}
	#endregion
	#region - public -
	public void Close()
	{
		if(AsPetManager.Instance.PetInfoDlg != null)
			AsPetManager.Instance.PetInfoDlg.Close();

		if(PetUpgradeDlg != null)
			PetUpgradeDlg.Close();

		Destroy( parent_);
	}

	public void SetSlotToPossible(PetListElement _element)
	{
		if(_element == null)
			return;
		
		if(m_dicPetManageSlot.ContainsKey(_element.PetSlot) == true)
			m_dicPetManageSlot[_element.PetSlot].Set_Possible();
	}

	public void SetSlotToBase(PetListElement _element)
	{
		if(_element == null)
			return;

		if(m_dicPetManageSlot.ContainsKey(_element.PetSlot) == true)
			m_dicPetManageSlot[_element.PetSlot].Set_UnSelected();
	}

	public void SetSlotToSelect(PetListElement _element)
	{
		if(m_dicPetManageSlot.ContainsKey(_element.PetSlot) == true)
			m_dicPetManageSlot[_element.PetSlot].Set_Selected();
		else
			Debug.LogWarning("UIPetManagerDlg:: SetSlotToSelect: slot is not found. _element.PetSlot = " + _element.PetSlot);
	}

	public void RefreshLockedSlots()
	{
		int openedLineInCurPage = AsPetManager.Instance.ExtendLine - (m_CurPage - 1) * s_LineCount;
		openedLineInCurPage = Mathf.Clamp(openedLineInCurPage, 0, s_LineCount);
		Debug.Log("UIPetManageDlg:: SetSlots: openedLineInCurPage = " + openedLineInCurPage);
		for(int i=s_LineCount - 1; i>openedLineInCurPage - 1; --i)
		{
			for(int j=0; j<s_LineSlotCount; ++j)
			{
				arSlot[i * s_LineCount + j].Set_Lock();
			}
		}
	}

	public void RefreshSlotsOnUpgradeDlgOpen()
	{
		PetListElement originSlot = null;
		PetListElement matterSlot = null;
		if(PetUpgradeDlg != null)
		{
			originSlot = PetUpgradeDlg.BaseElement;
			matterSlot = PetUpgradeDlg.MatterElement;
		}

		if(originSlot == null)
		{
			Debug.Log("UIPetManageDlg:: RefreshSlotsOnUpgradeDlgOpen : base element is not set. PetUpgradeDlg.BaseElement = " + PetUpgradeDlg.BaseElement);
			return;
		}

		for(int i=0; i<s_MaxSlotCount; ++i)
		{
			arSlot[i].UpgradeDlgOpened(originSlot, matterSlot);
		}
	}

	public void RefreshSlotsOnUpgradeDlgClose()
	{
		for(int i=0; i<s_MaxSlotCount; ++i)
		{
			arSlot[i].UpgradeDlgClosed();
		}
	}

	public void RefreshSlotsOnUpgradeDlgReset()
	{
		for(int i=0; i<s_MaxSlotCount; ++i)
		{
			arSlot[i].UpgradeDlgReset();
		}
	}

	public void SetMatterInUpgrade(PetListElement _element)
	{
		if(GetManageSlot(_element) != null)
		{
			foreach(UIPetManageSlot node in arSlot)
			{
				node.OtherSlotSelectedAsMatter(GetManageSlot(_element));
			}
		}
		else
			Debug.LogError("UIPetManageDlg:: SetMatterInUpgrade: m_dicPetManageSlot does not contain element. _element.PetSlot = " + _element.PetSlot);
	}
	#endregion
}

public delegate void dltManageSlotClicked(UIPetManageSlot _slot);
