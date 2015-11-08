

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class BuffBaseMgr : MonoBehaviour 
{
	public static readonly int s_MonsterSkillIndexRange_Min = 30001;
	public static readonly int s_MonsterSkillIndexRange_Max = 100000;
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------		
	public UIBuffSlot[] uiBuffSlots;  
	public SpriteText txtEtc;
	
	
	//---------------------------------------------------------------------
	/* Function */
	//---------------------------------------------------------------------	
	
	protected void SetActiveEtc( bool isActive )
	{
		if( null == txtEtc )
			return;
		
		if( false == isActive )
		{
			txtEtc.Text = string.Empty;
		}
		else
		{
			txtEtc.Text = "...";
		}
	}
	
	protected bool IsCheckHaveMobSkillIcon( int iSkillIndex, int iPotencyIndex )
	{
		Tbl_MonsterSkill_Record skillRecord = AsTableManager.Instance.GetTbl_MonsterSkill_Record(iSkillIndex);
		if( null == skillRecord )
			return false;
		
		if( skillRecord.listSkillPotency.Count <= iPotencyIndex )
			return false;
		
		if(string.Compare("NONE", skillRecord.listSkillPotency[iPotencyIndex].Potency_BuffIcon, true) != 0)
			return true;
		
		return false;
	}
	
	
	
	public static bool IsMonsterSkillIndex( int iIndex )
	{
		if(s_MonsterSkillIndexRange_Min <= iIndex && iIndex <= s_MonsterSkillIndexRange_Max)
		{
			return true;
		}		
		
		return false;		
	}	
	
	public void SetHideUI()
	{
		for (int i = 0; i < uiBuffSlots.Length; ++i)
        {            
			 uiBuffSlots[i].OffBuffSlot();           
        }		
	}
	
	
	public bool IsCheckHaveIcon( int iSkillIndex, int iPotencyIndex )
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record(iSkillIndex);
		if( null == skillRecord )
			return false;
		
		if( skillRecord.listSkillPotency.Count <= iPotencyIndex )
			return false;
		
		if(string.Compare("NONE", skillRecord.listSkillPotency[iPotencyIndex].Potency_BuffIcon, true) != 0)
			return true;
		
		return false;
	}
	
	
	public void InputUp()
	{ 	
		for (int i = 0; i < uiBuffSlots.Length; ++i)
        {            
			 uiBuffSlots[i].CloseTooltip();        
        }
	}
	
	public void GuiInputDown( Ray inputRay )
	{		
		
		for (int i = 0; i < uiBuffSlots.Length; ++i)
        {            
			 uiBuffSlots[i].GuiInputDown(inputRay);         
        }
	}
		
	
	public void InputMove(Ray inputRay)
	{
		for (int i = 0; i < uiBuffSlots.Length; ++i)
        {            
			 uiBuffSlots[i].InputMove(inputRay);         
        }
	}	
	
	public virtual void UpdateLogic()
	{
		if( GAME_STATE.STATE_LOADING == AsGameMain.s_gameState)
			return;
			
		
		if( null == UIManager.instance )
			return;
		
		Camera uicamera = UIManager.instance.rayCamera;
		if( null == uicamera )
			return;
		
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
			TouchUpdate(uicamera);
		else
			MouseUpdate(uicamera);
	}
	
	
	void Update()
	{
		UpdateLogic();
	}
	
	void MouseUpdate( Camera uicamera )
	{		
		if(Input.GetMouseButtonUp(0) == true)
		{			
			InputUp();	
		}	
		
		
		//if(Input.GetMouseButtonDown(0) == true)
		if(Input.GetMouseButtonUp(0) == true)
		{			
			Ray ray = uicamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) == true)
			{
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GUI"))
				{
					GuiInputDown(ray);
				}
			}
		}
		else if(Input.GetMouseButton(0) == true)
		{
			Ray ray = uicamera.ScreenPointToRay(Input.mousePosition);			
			InputMove(ray);				
		}
		/*else if(Input.GetMouseButtonUp(0) == true)
		{			
			InputUp();	
		}	*/
	}
	
	void TouchUpdate( Camera uicamera )
	{		
		if( 0 < Input.touchCount)
		{
			
			if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{						
				InputUp();	
			}	
						
			//if(Input.GetTouch(0).phase == TouchPhase.Began)
			if( Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled )
			{	
				Ray ray = uicamera.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit) == true)
				{			
					if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GUI"))
					{
						GuiInputDown( ray );	
					}
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Ray ray = uicamera.ScreenPointToRay(Input.mousePosition);	
				InputMove( ray );	
			}
			/*else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{						
				InputUp();	
			}*/			
			
		}	
	}
	
	
	
	
}
