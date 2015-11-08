using UnityEngine;
using System.Collections;

public enum eSkillBookSelectType
{
	Type_NotSet,
	Type_Active,
	Type_Finger,
	Type_Passive
}

public class AsSkillbookDlg : MonoBehaviour
{
	public delegate void CloseDelegate();
	public CloseDelegate closeDel;
	
	public AsSkillbookTab tab = null;
	
	
	public SimpleSprite newActiveTab;
	public SimpleSprite newFingerTab;
	public SimpleSprite newPassiveTab;
	
	
	// begin kij move icon
	public float m_fMaxItemMoveTime = 0.5f;
	public SpriteText m_TextTitle;
	public SpriteText m_TextActive;
	public SpriteText m_TextFinger;
	public SpriteText m_TextNotset;
	public SpriteText m_TextPassive;
	//public SpriteText m_TextSetting;
	private float m_fItemMoveTime = 0.0f;	
	private GameObject m_goMoveIcon;
	private AsSlot m_ClickDownSlot;
	// end kij
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextActive);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextFinger);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextNotset);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextPassive);
		//AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSetting);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 354);
		m_TextActive.Text = AsTableManager.Instance.GetTbl_String( 356);
		m_TextFinger.Text = AsTableManager.Instance.GetTbl_String( 357);
		m_TextNotset.Text = AsTableManager.Instance.GetTbl_String( 355);
		m_TextPassive.Text = AsTableManager.Instance.GetTbl_String( 358);
		//m_TextSetting.Text = AsTableManager.Instance.GetTbl_String( 359);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Open()
	{
		gameObject.SetActiveRecursively( true);
		SkillBook.Instance.ResetDicNotSettingData();
		tab.Init( eSkillBookSelectType.Type_NotSet);
		
		SetNewActiveImg( SkillBook.Instance.newdicActive.Count > 0 );
		SetNewFingerTab( SkillBook.Instance.newdicFinger.Count > 0 );
		SetNewPassiveTab( SkillBook.Instance.newdicPassive.Count > 0 );
	}

	
	public void SetNewActiveImg( bool _isActive )
	{
		if( null == newActiveTab )
			return;
		
		if( newActiveTab.gameObject.active == _isActive )
			return;
		
		newActiveTab.gameObject.active = _isActive;
	}
	
	public void SetNewFingerTab( bool _isActive )
	{
		if( null == newFingerTab )
			return;
		
		if( newFingerTab.gameObject.active == _isActive )
			return;
		
		newFingerTab.gameObject.active = _isActive;
	}
	
	public void SetNewPassiveTab( bool _isActive )
	{
		if( null == newPassiveTab )
			return;
		
		if( newPassiveTab.gameObject.active == _isActive )
			return;
		
		newPassiveTab.gameObject.active = _isActive;
	}
	
	public void OnClickClose()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}
	
	public void Close()
	{
		closeDel();
		
		SkillBook.Instance.SetNewSkillInit();
	}
	  
	// begin kij move icon
	public void GuiInputDown( Ray inputRay)
	{
		m_ClickDownSlot = null;
		m_fItemMoveTime = 0.0f;	
		
		AsSkillTab _skillTab = tab.GetCurSkillTab();
		if( null == _skillTab)
			return;
		
		for( int i=0; i<_skillTab.getList().Count; ++i)			
		{
			IUIListObject obj = _skillTab.getList().GetItem(i);
			if( null == obj)
				continue;
			AsSlot _slot = obj.gameObject.GetComponentInChildren<AsSlot>();
			if( null == _slot)
				continue;
			
			if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
			{
				m_ClickDownSlot = _slot;
				break;
			}		
		}
	}
	
	public void GuiInputMove( Ray inputRay)
	{		
		if( null == m_goMoveIcon && null != m_ClickDownSlot)
		{
			if( m_fMaxItemMoveTime <= m_fItemMoveTime)
			{				
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6025_EFF_SlotUp", Vector3.zero, false);
				m_goMoveIcon = ResourceLoad.CreateSkillIcon( m_ClickDownSlot.getSkillID);							
				m_fItemMoveTime = 0.0f;	
				
				AsQuickSlotManager.Instance.Foward();
			}
			
			m_fItemMoveTime += Time.deltaTime;
		}		
	}

	
	public void GuiInputUp( Ray inputRay)
	{
		if( null != m_ClickDownSlot)
		{
			if( null != m_goMoveIcon)			
			{		
				//if( false == AsHudDlgMgr.Instance.skillSettingDlg.SetSkillIcon( inputRay, m_ClickDownSlot.getSkillID, m_ClickDownSlot.getSkillLevel))
				//{	
					if( true == AsQuickSlotManager.Instance.SetMoveInvenSlotInSkill( inputRay, m_ClickDownSlot.getSkillID, m_ClickDownSlot.getSkillLevel))
					{						
						/*if( true == AsHudDlgMgr.Instance.IsOpenedSkillSettingDlg)
						{
							AsHudDlgMgr.Instance.skillSettingDlg.ResetSlots();
						}*/
					}
				//}
				
				GameObject.Destroy( m_goMoveIcon);
			}
			else
			{
				AsSkillTab _skillTab = tab.GetCurSkillTab();
				_skillTab.PromptTooltipBySkillID( m_ClickDownSlot.getSkillID, m_ClickDownSlot.getSkillLevel);
			}

			m_ClickDownSlot = null;
			
			AsQuickSlotManager.Instance.Backward();
		}
	}
	
	public void InputUp( Ray inputRay)
	{
		if( null != m_ClickDownSlot && null != m_goMoveIcon)
		{				
			GameObject.Destroy( m_goMoveIcon);
			m_ClickDownSlot = null;				
		}
	}
	
	public void InputMove(Ray inputRay)
	{	
		if( null != m_goMoveIcon)
		{
			Vector3 vec3Temp = inputRay.origin;			
			vec3Temp.z = -10;		
			
			m_goMoveIcon.transform.position = vec3Temp;
		}				
	}
	
	//$yde
	public void RefreshSkillBookDlg()
	{
		AsSkillTab _skillTab = tab.GetCurSkillTab();
		_skillTab.Init(-1);
	}
	
	//end kij move icon
}
