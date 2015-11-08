
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class UIPetInfoDlg : MonoBehaviour
{
	public enum eHungry { Hungry = 0, Normal, Satisfy, MAX}
	
	#region - serialized -
	[SerializeField] GameObject parent_;

	// - title -
	[SerializeField] SpriteText text_Title;
	[SerializeField] UIButton btn_Closing;

	// - basic info -
	[SerializeField] SpriteText txt_1base;

	[SerializeField] UIPetImg img1_;

	[SerializeField] SpriteText txt_Name;
	[SerializeField] SpriteText txt_Character;
	[SerializeField] SpriteText txt_Level;
	[SerializeField] SpriteText txt_Hungry;

	[SerializeField] UIButton btn_Rename;
	[SerializeField] SpriteText text_BtnRename;

	[SerializeField] UIProgressBar gauge_Level;
	[SerializeField] SpriteText text_GaugeLevel;
	[SerializeField] UIProgressBar gauge_Hungry;
	[SerializeField] SpriteText text_GaugeHungry;

	// - accessory -
	[SerializeField] SpriteText txt_2accessory;
	[SerializeField] UIPetImg img2;
	[SerializeField] UIPetSlot slot2;
	[SerializeField] SpriteText txt_5accessory;

	[SerializeField] UIButton btn_Accessory;
	[SerializeField] SpriteText text_Accessory;
	
	// - skill -
	[SerializeField] SpriteText txt_3skill;

	[SerializeField] SpriteText text_IndexTitle1;
	[SerializeField] UIPetImg img3;
	[SerializeField] SpriteText text_Index1;

	[SerializeField] SpriteText text_IndexTitle2;
	[SerializeField] UIPetImg img4;
	[SerializeField] SpriteText text_Index2;

	[SerializeField] SpriteText text_IndexTitle3;
	[SerializeField] UIPetImg img5;
	[SerializeField] SpriteText text_Index3;

	// under buttons
	[SerializeField] SpriteText text_BtnSummon;
	[SerializeField] UIButton btn_Summon;
	[SerializeField] SpriteText text_BtnUpgrade;
	[SerializeField] UIButton btn_Upgrade;
	[SerializeField] SpriteText text_BtnSynthesis;
	[SerializeField] UIButton btn_Synthesis;
	[SerializeField] SpriteText text_BtnFarewell;
	[SerializeField] UIButton btn_Farewell;

	[SerializeField] UIPetSkillTooltip tooltip;
	#endregion
	#region - member -
	string[] m_HungryGrade = new string[ (int)eHungry.MAX];
	float m_HungryMaxTime;
	float[] m_HungryGradeValue = new float[ (int)eHungry.MAX];

	body_SC_PET_INFO m_Info;
	#endregion
	#region - init & release -
	void Awake()
	{
		// title
		text_Title.Text = AsTableManager.Instance.GetTbl_String( 2200);

		// basic info
		txt_1base.Text = AsTableManager.Instance.GetTbl_String(2743);

		txt_Name.Text = AsTableManager.Instance.GetTbl_String( 2201);
		txt_Character.Text = AsTableManager.Instance.GetTbl_String( 2202);
		txt_Level.Text = AsTableManager.Instance.GetTbl_String( 2203);
		txt_Hungry.Text = AsTableManager.Instance.GetTbl_String( 2204);
		
		m_HungryGrade[(int)eHungry.Hungry] = AsTableManager.Instance.GetTbl_String( 2205);
		m_HungryGrade[(int)eHungry.Normal] = AsTableManager.Instance.GetTbl_String( 2206);
		m_HungryGrade[(int)eHungry.Satisfy] = AsTableManager.Instance.GetTbl_String( 2207);

		text_BtnRename.Text = AsTableManager.Instance.GetTbl_String( 2210);

		// accessory
		txt_2accessory.Text = AsTableManager.Instance.GetTbl_String(2208);
		text_Accessory.Text = AsTableManager.Instance.GetTbl_String(2209);

		// skill
		txt_3skill.Text = AsTableManager.Instance.GetTbl_String(2212);
		text_Index1.Text = "";
		text_IndexTitle1.Text = AsTableManager.Instance.GetTbl_String(2744);
		text_Index2.Text = "";
		text_IndexTitle2.Text = AsTableManager.Instance.GetTbl_String(2745);
		text_Index3.Text = "";
		text_IndexTitle3.Text = AsTableManager.Instance.GetTbl_String(2747);

		// under buttons
		text_BtnSummon.Text = AsTableManager.Instance.GetTbl_String( 2749);
		text_BtnUpgrade.Text = AsTableManager.Instance.GetTbl_String( 2751);
		text_BtnSynthesis.Text = AsTableManager.Instance.GetTbl_String( 2752);
		text_BtnFarewell.Text = AsTableManager.Instance.GetTbl_String( 2211);
	}
	
	public void Open( body_SC_PET_INFO _info)
	{
		m_Info = _info;

		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord( _info.nPetTableIdx);
		if( petRec == null) return;

		int maxExp = petRec.GetNextMaxExp(_info.nLevel);
		int exceptExp = petRec.GetPrevMaxExp(_info.nLevel);
		maxExp -= exceptExp;
		int curExp = _info.nExp - exceptExp;
		
		// name & personality
		txt_Name.Text = AsTableManager.Instance.GetTbl_String( 2201) + AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( _info.szPetName));
		Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(_info.nPetPersonality);
		if(scriptRec != null)
			txt_Character.Text = AsTableManager.Instance.GetTbl_String( 2202) + AsTableManager.Instance.GetTbl_String(scriptRec.PersonName);
		
		// level
		txt_Level.Text = AsTableManager.Instance.GetTbl_String( 2203) + _info.nLevel.ToString();
		gauge_Level.Value = (float)curExp / (float)maxExp;
		text_GaugeLevel.Text = curExp + " / " + maxExp;
		
		// hungry
		m_HungryMaxTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "Pet_HungryTime").Value;
		m_HungryGradeValue[(int)eHungry.Normal] = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "Pet_Normal").Value; 
		m_HungryGradeValue[(int)eHungry.Satisfy] = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "Pet_Good").Value;
		
		float degree = (float)_info.nHungryPoint;
		txt_Hungry.Text = AsTableManager.Instance.GetTbl_String( 2204);
		if( degree > m_HungryGradeValue[(int)eHungry.Satisfy])
			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Satisfy];
		else if( degree > m_HungryGradeValue[(int)eHungry.Normal])
			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Normal];
		else
			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Hungry];
		
		gauge_Hungry.Value = degree / m_HungryMaxTime;
		text_GaugeHungry.Text = ((int)(degree / m_HungryMaxTime * 100f)).ToString() + "%";
		
		// icon
		img1_.SetSlotImg(petRec.Icon);
		
//		if( AsPetManager.Instance.isPetEquip == true)
//		{
//			img2.SetSlotImg_Item(_info.sItem);
//			slot2.SetSlotInfo(_info.sItem);
//		}

		// accessory
		if(_info.sItem.nItemTableIdx > 0)
		{
			img2.SetSlotImg_Item(_info.sItem);
			slot2.SetSlotInfo(_info.sItem);

			txt_5accessory.Text = _info.sItem.GetNameWithStrengthCount();
			btn_Accessory.renderer.material.color = Color.white;
		}
		else
		{
			ReleaseEquip();
		}
		
		// passive skill
		sPETSKILL passive = _info.sSkill[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_PASSIVE];
		if(passive.nSkillTableIdx > 0)
		{
			img3.SetSlotImg_Skill(passive);
			
			Tbl_Skill_Record skillRec = AsTableManager.Instance.GetTbl_Skill_Record(passive.nSkillTableIdx);
			if( skillRec != null)
			{
				string title = AsTableManager.Instance.GetTbl_String( skillRec.SkillName_Index);
				text_IndexTitle1.Text = title;
				text_Index1.Text = "LV " + passive.nLevel;
//				string desc = AsTableManager.Instance.GetTbl_String( skillRec.Description_Index);
//				desc = AsUtil.ModifyDescriptionInTooltip( desc, passive.nSkillTableIdx, passive.nLevel, 0);
//				text_Index1.Text = desc;
			}
			else
				Debug.LogError(" UIPetInfoDlg:: Open: there is no PASSIVE Tbl_Skill_Record. id = " + _info.sSkill[0].nSkillTableIdx);
		}
		else
		{
			img3.DeleteSlotImg();
			text_IndexTitle1.Text = "";
			text_Index1.Text = "";
		}

		// active skill
		sPETSKILL active = _info.sSkill[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_ACTIVE];
		if(active.nSkillTableIdx > 0)
		{
			img4.SetSlotImg_Skill(active);
			
			Tbl_Skill_Record skillRec = AsTableManager.Instance.GetTbl_Skill_Record(active.nSkillTableIdx);
			if( skillRec != null)
			{
				string title = AsTableManager.Instance.GetTbl_String( skillRec.SkillName_Index);
				text_IndexTitle2.Text = title;
				text_Index2.Text = "LV " + active.nLevel;
//				string desc = AsTableManager.Instance.GetTbl_String( skillRec.Description_Index);
//				desc = AsUtil.ModifyDescriptionInTooltip( desc, active.nSkillTableIdx, active.nLevel, 0);
//				text_Index2.Text = desc;
			}
			else
				Debug.LogError(" UIPetInfoDlg:: Open: there is no ACTIVE Tbl_Skill_Record. id = " + _info.sSkill[1].nSkillTableIdx);
		}
		else
		{
			img4.DeleteSlotImg();
			text_IndexTitle2.Text = "";
			text_Index2.Text = AsTableManager.Instance.GetTbl_String(2746);
		}

		// special skill
		sPETSKILL special = _info.sSkill[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_SPECIAL];
		if(special.nSkillTableIdx > 0)
		{
			img5.SetSlotImg_Skill(special);
			
			Tbl_Skill_Record skillRec = AsTableManager.Instance.GetTbl_Skill_Record(special.nSkillTableIdx);
			if( skillRec != null)
			{
				string title = AsTableManager.Instance.GetTbl_String( skillRec.SkillName_Index);
				text_IndexTitle3.Text = title;
				text_Index3.Text = "LV " + special.nLevel;
//				string desc = AsTableManager.Instance.GetTbl_String( skillRec.Description_Index);
//				desc = AsUtil.ModifyDescriptionInTooltip( desc, special.nSkillTableIdx, special.nLevel, 0);
//				text_Index3.Text = desc;
			}
			else
				Debug.LogError(" UIPetInfoDlg:: Open: there is no SPECIAL Tbl_Skill_Record. id = " + _info.sSkill[1].nSkillTableIdx);
		}
		else
		{
			img5.DeleteSlotImg();
			text_IndexTitle3.Text = "";
			text_Index3.Text = AsTableManager.Instance.GetTbl_String(2748);
		}

		// button
		if(AsPetManager.Instance.CheckInfoViewingPetSummoning() == true)
			text_BtnSummon.Text = AsTableManager.Instance.GetTbl_String( 2750);
		else
			text_BtnSummon.Text = AsTableManager.Instance.GetTbl_String( 2749);
	}
	
	void Start()
	{
		#region - basic -
		btn_Closing.SetInputDelegate(Dlt_Close);
		btn_Rename.SetInputDelegate(Dlt_Rename);
		btn_Accessory.SetInputDelegate(Dlt_ReleaseAccessory);
		btn_Summon.SetInputDelegate(Dlt_Call);
		btn_Upgrade.SetInputDelegate(Dlt_Upgrade);
		btn_Synthesis.SetInputDelegate(Dlt_Synthesis);
		btn_Farewell.SetInputDelegate(Dlt_Farewell);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_Title);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_Index1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_Index2);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_IndexTitle1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text_IndexTitle2);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Name);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Character);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Level);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Hungry);
		#endregion
		
		AsHudDlgMgr.Instance.PetDlgOpened();
	}
	
	void Update()
	{
	}
	#endregion
	#region - method -
	#endregion
	#region - delegate -
	void Dlt_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			Close();
		}
	}

	void Dlt_Rename(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			GameObject obj = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_ReName")) as GameObject;
			AsReNameDlg rename = obj.GetComponentInChildren<AsReNameDlg>();
			rename.gameObject.AddComponent<UIPetPopup>();
			rename.Open( obj, 0, AsReNameDlg.eReNameType.Pet);
		}
	}

	void Dlt_ReleaseAccessory(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;

			if(m_Info.sItem.nItemTableIdx <= 0)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			AsPetManager.Instance.Send_PetUnEquip();
		}
	}

	void Dlt_Call(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if(AsPetManager.Instance.CheckInfoViewingPetSummoning() == true)
				AsPetManager.Instance.Send_Release();
			else
				AsPetManager.Instance.Send_PetCall();
		}
	}

	void Dlt_Upgrade(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsPetManager.Instance.OpenPetUpgradeDlg();
		}
	}

	void Dlt_Synthesis(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			AsPetManager.Instance.OpenPetSynthesisDlg();
		}
	}
	
	void Dlt_Farewell(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPetManager.Popup == true)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			string title = AsTableManager.Instance.GetTbl_String( 126);
			string content = "not defined";
			if(AsPetManager.Instance.CurDisplayingInfoOnPetInfoDlg.itemView.nItemTableIdx > 0)
			{
				content = AsTableManager.Instance.GetTbl_String(2322);
				AsNotify.Instance.MessageBox( title, content, this, "_FarewellConfirm",
				                             AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION).gameObject.AddComponent<UIPetPopup>();
			}
			else
			{
				content = AsTableManager.Instance.GetTbl_String(2214);
				AsNotify.Instance.MessageBox( title, content, this, "_FarewellConfirm",
				                             AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION).gameObject.AddComponent<UIPetPopup>();
			}
		}
	}
	void _FarewellConfirm()
	{
		AsPetManager.Instance.Send_PetDelete();
	}
	#endregion
	#region - public -
	public void Close()
	{
		Destroy( parent_);

		if(AsPetManager.Instance.PetUpgradeDlg == null)
			AsPetManager.Instance.UnSelectManageSlot();
	}
	
	public void ChangeName( body_SC_PET_NAME_NOTIFY _load)
	{
		txt_Name.Text = AsTableManager.Instance.GetTbl_String( 2201) + AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( _load.szPetName));
	}
	
	public void ChangeEquip( sITEMVIEW _item)
	{
		if(_item.nItemTableIdx <= 0)
		{
			ReleaseEquip();
		}
		else
		{
			img2.SetSlotImg_Item(_item);

			Item item = ItemMgr.ItemManagement.GetItem(_item.nItemTableIdx);
			if(item == null)
			{
				txt_5accessory.Text = "undefined index(" + _item.nItemTableIdx + ")";
				return;
			}
			
			string str = item.ItemData.GetGradeColor() + AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
			if(_item.nStrengthenCount > 0)
				str = Color.white + "+" + _item.nStrengthenCount + " " + str;

			txt_5accessory.Text = str;
		}
	}
	
//	public void ChangeHungry()
//	{
//		txt_Hungry.Text = AsTableManager.Instance.GetTbl_String( 2204);
//		
//		float degree = AsPetManager.Instance.CurHungry;
//		if( degree > m_HungryGradeValue[(int)eHungry.Satisfy])
//			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Satisfy];
//		else if( degree > m_HungryGradeValue[(int)eHungry.Normal])
//			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Normal];
//		else
//			txt_Hungry.Text += m_HungryGrade[(int)eHungry.Hungry];
//	}

	public void RefreshInfoDlg()
	{
		if(AsPetManager.Instance.CheckInfoViewingPetSummoning() == true)
			text_BtnSummon.Text = AsTableManager.Instance.GetTbl_String( 2750);
		else
			text_BtnSummon.Text = AsTableManager.Instance.GetTbl_String( 2749);
	}

	public void ReleaseEquip()
	{
		m_Info.sItem.nItemTableIdx = 0;
		
		img2.DeleteSlotImg();
		slot2.ReleaseSlot();

		txt_5accessory.Text = "";

		btn_Accessory.renderer.material.color = Color.gray;
	}

	public void SetTooltip(Tbl_Skill_Record _skill, Tbl_SkillLevel_Record _skillLv)
	{
		tooltip.Init(_skill, _skillLv);
	}
	#endregion
}
