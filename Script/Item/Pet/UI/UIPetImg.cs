using UnityEngine;
using System.Collections;
using System.Globalization;

public class UIPetImg : MonoBehaviour
{
	private GameObject m_Object;
	private UISlotItem m_SlotItem;

	enum eState {NONE = 0, From_Path, From_Item, From_Skill}
	eState m_State;

	sPETSKILL m_PetSkill;
	
	public void DeleteSlotImg()
	{
		m_State = eState.NONE;

		if( null == m_Object)
			return;
		
		DestroyImmediate( m_Object );	
	}
	
	public bool SetSlotImg( string path)
	{
		m_State = eState.From_Path;

		GameObject faceObj = ResourceLoad.LoadGameObject( path);
		return _SetSlotImg( faceObj);
	}

	public bool SetSlotImg_Item( sITEM _view)
	{
		m_State = eState.From_Item;

		Item item = ItemMgr.ItemManagement.GetItem( _view.nItemTableIdx);
		if(item != null)
			return _SetSlotImg_Item(item.ItemID);
		else
		{
			DeleteSlotImg();
			return false;
		}
	}

	public bool SetSlotImg_Item( sITEMVIEW _view)
	{
		m_State = eState.From_Item;

		Item item = ItemMgr.ItemManagement.GetItem( _view.nItemTableIdx);
		if(item != null)
			return _SetSlotImg_Item(item.ItemID);
		else
		{
			DeleteSlotImg();
			return false;
		}
	}

	bool _SetSlotImg_Item(int _itemIdx)
	{
		Item item = ItemMgr.ItemManagement.GetItem(_itemIdx);
		if( item == null)
		{
			DestroyImmediate( m_Object );
			return false;
		}
		
		return _SetSlotImg( item.GetIcon());
	}

	public bool SetSlotImg_Skill( int _idx)
	{
		return _SetSlotImg_Skill(_idx);
	}
	public bool SetSlotImg_Skill(sPETSKILL _skill)
	{
		m_PetSkill = _skill;

		return _SetSlotImg_Skill(_skill.nSkillTableIdx);
	}
	bool _SetSlotImg_Skill( int _idx)
	{
		m_State = eState.From_Skill;

		GameObject skillObj = null;
		bool ret = true;
		
		if( _idx > 0)
		{
			Tbl_Skill_Record skillRec = AsTableManager.Instance.GetTbl_Skill_Record( _idx);
			if( skillRec != null)
				skillObj = ResourceLoad.LoadGameObject( skillRec.Skill_Icon);
			else
				ret = false;
		}
		else
			ret = false;
		
		if( ret == false) return false;
		else return _SetSlotImg( skillObj);
	}
	
	bool _SetSlotImg( GameObject _img)
	{
		if( m_Object != null) Destroy( m_Object);
		
		m_Object = GameObject.Instantiate( _img ) as GameObject;
		m_Object.transform.parent = transform;
		m_Object.transform.localPosition = new Vector3( 0, 0, -0.2f);
		m_Object.transform.localRotation = Quaternion.identity;
		m_Object.transform.localScale = Vector3.one;
		
		m_SlotItem = m_Object.GetComponent<UISlotItem>();

		float width = 1f; float height = 1f;
		BoxCollider boxCollider = collider as BoxCollider;
		if(boxCollider != null)
		{
			width = boxCollider.size.x;
			height = boxCollider.size.y;
		}
		else
		{
			width = transform.localScale.x;
			height = transform.localScale.y;
		}
		
		m_SlotItem.iconImg.SetSize( width, height);
		
		return true;
	}

	void OnMouseUpAsButton()
	{
		if(m_State == eState.From_Skill)
			OpenTooltip();
		else
			SendMessageUpwards("UIPetManageSlot_UIPetImg_Clicked", SendMessageOptions.DontRequireReceiver);
	}

	void OpenTooltip()
	{
		if(m_PetSkill == null)
		{
			Debug.LogError("UIPetImg:: OpenTooltip: no pet skill data set");
			return;
		}

		Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(m_PetSkill.nSkillTableIdx);
		Tbl_SkillLevel_Record skillLv = AsTableManager.Instance.GetTbl_SkillLevel_Record(m_PetSkill.nLevel, m_PetSkill.nSkillTableIdx);

		if(skill == null || skillLv == null)
		{
			Debug.LogError("UIPetImg:: OpenTooltip: invalid data. m_PetSkill.nSkillTableIdx = " + m_PetSkill.nSkillTableIdx + ", m_PetSkill.nLevel = " + m_PetSkill.nLevel);
			return;
		}

		if(AsPetManager.Instance.PetInfoDlg != null)
			AsPetManager.Instance.PetInfoDlg.SetTooltip(skill, skillLv);
	}
}

