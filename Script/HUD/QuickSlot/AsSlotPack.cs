using UnityEngine;
using System.Collections;

public class AsSlotPack : MonoBehaviour
{
	public AsSlot[] slotArray = new AsSlot[0];
	
	public bool Show
	{
		set
		{
			foreach( AsSlot slot in slotArray)
				slot.Show = value;
		}
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void DisableAllSlot( bool flag)
	{
		foreach( AsSlot slot in slotArray)
			slot.Disable = flag;
	}
	
	public void BeginCooltime( int skillID )
	{
		foreach( AsSlot slot in slotArray)
		{
			if( ( 0 != slot.getSkillID ) && ( skillID == slot.getSkillID))
				slot.OuterInvoke();
		}
	}
	
	public void BeginCharge( int skillID)
	{
		foreach( AsSlot slot in slotArray)
		{
			if( ( 0 != slot.getSkillID) && ( skillID == slot.getSkillID))
			{
				if( IsTouchedSlot(slot) == true )
				{
					slot.Charge();
					break;
				}
			}
		}
	}

	bool IsTouchedSlot( AsSlot _slot )
	{
		Vector3 _touchPos;

		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) 
		{
			_touchPos = Input.GetTouch (0).position;
		}
		else 
		{
			if( Input.touchCount <= 0 )
				return false;

			_touchPos = Input.mousePosition;
		}

		Ray ray =  AsInputManager.Instance.UICamera.ScreenPointToRay(_touchPos);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit/*, LayerMask.NameToLayer("GUI")*/) == true)
		{
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GUI"))
			{
				AsSlot _touchSlot = hit.collider.gameObject.GetComponent<AsSlot>();
				
				if( _touchSlot == _slot )
					return true;
			}
		}

		return false;
	}

	///$yde
	public void ChangeStance(StanceInfo _stance)
	{
		foreach( AsSlot slot in slotArray)
		{
			if( ( 0 != slot.getSkillID ) && ( _stance.StanceSkill == slot.getSkillID))
			{
				slot.StanceChanged(_stance);
//				int lv = _stance.SkillLevel;
//				Tbl_SkillLevel_Record lvRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record(lv, _stance.StanceSkill);
//				Tbl_SkillLevel_Potency potency = lvRecord.listSkillLevelPotency[_stance.StancePotency];
//				
//				slot.SetSkillIcon((int)potency.Potency_Value, (int)potency.Potency_IntValue);
			}
		}
	}
}
