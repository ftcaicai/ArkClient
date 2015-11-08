using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AsSlot : MonoBehaviour
{
	public enum SLOT_TYPE
	{
		SLT_INVALID = 0x0000,

		SLT_CT_ITEM = 0x0100,
		SLT_CT_SKILL = 0x0200,
		FILTER_CONTENT = 0xFF00,

		SLT_IT_IMMEDIATE = 0x0001,
		SLT_IT_CHARGE = 0x0002,
		SLT_IT_TARGET = 0x0004,
		FILTER_INVOKE_TYPE = 0x00FF,
		SLT_IT_SHOP = 0x0008,
		SLT_IT_BOOK = 0x000F,
		SLT_IT_POST = 0x001F
	};

	public enum eUSE_TYPE
	{
		NONE,
		QUICK_SLOT,
		SKILL_BOOK,
	}

	public SimpleSprite disableScreen = null;
	public Texture defaultTexture = null;
//	public SLOT_TYPE type = SLOT_TYPE.SLT_CT_SKILL | SLOT_TYPE.SLT_INVALID;
	public SLOT_TYPE type = SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_INVALID;
	private bool isUsable = true;

	public AsIconCooltime screen = null;
	private SpriteText txt = null;
	private bool isClicked = false;
	private bool chargeBegined = false;
	private float ChargeTime = 0.0f;
	private int ChargeStep = 1;
	private bool isDisable = false;
	private bool isShow = true;

	// being kij
	private GameObject m_goIcon;	
	public eUSE_TYPE UseType = eUSE_TYPE.NONE;

	// item
	private RealItem m_RealItem;
	private int m_iItemCount = 0;
	private int m_iItemID;
	private sITEM m_sItem;

	// skill
	private int m_iSkillId;
	private int m_iSkillLevel;

	//	temp
	Tbl_SkillLevel_Record m_curSkillLevelRecord = null;
	
	bool	m_isMouseDown = false;
	
	//$yde
	bool m_UseOnly_Movable = false;
	bool m_DisableSkill_PvP = false;
	bool m_DisableSkill_Raid = false;
    bool m_DisableSkill_Field = false;
	bool m_DisableSkill_Indun = false;

	// get
	public int getSkillID
	{
		get	{ return m_iSkillId; 	}
	}

	public int getSkillLevel
	{
		get	{ return m_iSkillLevel; 	}
	}

	public int getItemID
	{
		get	{ return m_iItemID; }
	}

	public int getItemCount
	{
		get	{ return m_iItemCount; }
	}

	public GameObject getIcon
	{
		get	{ return m_goIcon; }
	}

	public bool IsEnableItem
	{
		get	{ return 0 != m_iItemID && 0 < m_iItemCount && null != m_RealItem; }
	}

	public RealItem getRealItem
	{
		get	{ return m_RealItem; }
	}

	public sITEM getSItem
	{
		get	{ return m_sItem; }
	}

	public bool IsEnableSkill
	{
		get	{ return 0 != m_iSkillId && 0 != m_iSkillLevel; }
	}

	private bool CreateIcon( string strPath)
	{
		GameObject obj = Resources.Load( strPath) as GameObject;
		if( null == obj)
		{
			Debug.LogError( "AsSlot::CreateSkillIcon() [ null == load fail] path : " + strPath);
			return false;
		}

		m_goIcon = GameObject.Instantiate( obj) as GameObject;
		m_goIcon.transform.parent = transform;
		m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
		m_goIcon.transform.localRotation = Quaternion.identity;
		m_goIcon.transform.localScale = Vector3.one;

		return true;
	}

	private void DeleteIcon()
	{
		if( null == m_goIcon)
			return;

		Destroy( m_goIcon);
	}

	protected void SetDisableSkillPvp( Item _item)
	{
		if( null == _item)
			return;

		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)
			return;


		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)		
			m_DisableSkill_PvP = ( skillrecord.DisableInPvP == eDisableInPvP.Disable);			
		
	}
	
	protected void SetDisableSkillIndun( Item _item)
	{
		if( null == _item)
			return;
		
		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)
			return;
		
		
		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)		
			m_DisableSkill_Indun = ( skillrecord.DisableInInDun == eDisableInInDun.Disable);			
		
	}

	protected void SetDisableSkillRaid( Item _item)
	{
		if( null == _item)
			return;

		if( _item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)
			return;


		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill);
		if( null != skillrecord)
		{			
			m_DisableSkill_Raid = ( skillrecord.DisableInRaid == eDisableInRaid.Disable);
            m_DisableSkill_Field = (skillrecord.DisableInField == eDisableInRaid.Disable);
		}
	}


	// item
	public void SetItem( int iItemID)
	{
		ChargeCancel();
		m_iSkillId = 0;
		m_iSkillLevel = 0;

		if( 0 == iItemID)
		{
			DeleteIcon();			
			m_RealItem = null;
			m_sItem = null;
			m_iItemCount = 0;
			m_iItemID = 0;
			SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_INVALID);
			return;
		}

		//$yde
		RealItem realItem = ItemMgr.HadItemManagement.Inven.GetRealItem( iItemID);
		if( realItem != null)
		{
			SetDisableSkillPvp( realItem.item);
			SetDisableSkillRaid(realItem.item);
			SetDisableSkillIndun( realItem.item);
		}

		int iItemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount( iItemID);

		if( iItemID == m_iItemID && null != m_goIcon)
		{
			m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
			if( m_iItemCount != iItemCount)
			{
				m_iItemCount = iItemCount;
				SetEnableIcon();
			}

			if( 0 >= m_iItemCount)
			{				
				m_RealItem = null;
				m_iItemCount = 0;
			}
			else
			{
				m_RealItem = ItemMgr.HadItemManagement.Inven.GetRealItem( m_iItemID);
				if( null == m_RealItem)
				{
					Debug.LogError( "AsSlot::SetItem()[ null == m_RealItem ] item id : " + iItemID + " count : " + iItemCount);
					return;
				}				
				SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_IT_IMMEDIATE);
			}

			return;
		}

		if( 0 == iItemCount)
		{
			m_iItemID = iItemID;
			m_iItemCount = iItemCount;		

			DeleteIcon();
			Item _item = ItemMgr.ItemManagement.GetItem( getItemID);
			if( null == _item)
				return;

			if( false == CreateIcon( _item.ItemData.GetIcon()))
			{
				Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] item id : " + getItemID + " icon path : " + _item.ItemData.GetIcon());
				return;
			}

			SetEnableIcon();
		}
		else
		{
			m_iItemID = iItemID;
			m_iItemCount = iItemCount;
			m_RealItem = ItemMgr.HadItemManagement.Inven.GetRealItem( m_iItemID);
			if( null == m_RealItem)
			{
				Debug.LogError( "AsSlot::SetItem()[ null == m_RealItem ] item id : " + iItemID + " count : " + iItemCount);
				return;
			}

			DeleteIcon();
			if( false == CreateIcon( m_RealItem.item.ItemData.GetIcon()))
			{
				Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] item id : " + iItemID + " icon path : " + m_RealItem.item.ItemData.GetIcon());
				return;
			}
			m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
		
			UISlotItem slotItem = m_goIcon.GetComponent<UISlotItem>();
			if( null != slotItem)
				slotItem.SetItemCountText( m_iItemCount);

			Tbl_SkillLevel_Record skillData = AsTableManager.Instance.GetTbl_SkillLevel_Record( m_RealItem.item.ItemData.itemSkillLevel, m_RealItem.item.ItemData.itemSkill);
			ChargeTime = skillData.ChargeDuration * 0.001f;
			ChargeStep = skillData.ChargeMaxStep;
			SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_IT_IMMEDIATE);
		}
	}

	// item
	public void SetItem( RealItem _realItem, int iItemCount)
	{
		ChargeCancel();
		m_iSkillId = 0;
		m_iSkillLevel = 0;

		if( null == _realItem || 0 >= iItemCount)
		{
			DeleteIcon();			
			m_RealItem = null;
			m_sItem = null;
			m_iItemCount = 0;
			m_iItemID = 0;
			SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_INVALID);
			return;
		}

		m_iItemID = _realItem.item.ItemID;
		m_iItemCount = iItemCount;
		m_RealItem = _realItem;

		DeleteIcon();
		if( false == CreateIcon( m_RealItem.item.ItemData.GetIcon()))
		{
			Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] item id : " + m_iItemID + " icon path : " + m_RealItem.item.ItemData.GetIcon());
			return;
		}
		m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);

		UISlotItem slotItem = m_goIcon.GetComponent<UISlotItem>();
		if( null != slotItem)
			slotItem.SetItemCountText( m_iItemCount);

		SetDisableSkillPvp( _realItem.item);
		SetDisableSkillRaid( _realItem.item);
		SetDisableSkillIndun( _realItem.item);
	}

	public void SetItem( int iItemID, int iItemCount, sITEM _sitem)
	{
		ChargeCancel();
		m_iSkillId = 0;
		m_iSkillLevel = 0;

		Item _item = ItemMgr.ItemManagement.GetItem( iItemID);

		if( null == _item || 0 >= iItemCount)
		{
			DeleteIcon();			
			m_RealItem = null;
			m_sItem = null;
			m_iItemCount = 0;
			m_iItemID = 0;
			SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_INVALID);
			return;
		}

		m_sItem = _sitem;
		m_iItemID = iItemID;
		m_iItemCount = iItemCount;

		DeleteIcon();
		if( false == CreateIcon( _item.ItemData.GetIcon()))
		{
			Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] item id : " + m_iItemID + " icon path : " + _item.ItemData.GetIcon());
			return;
		}
		m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);

		UISlotItem slotItem = m_goIcon.GetComponent<UISlotItem>();
		if( null != slotItem)
			slotItem.SetItemCountText( m_iItemCount);

		//$yde
		SetDisableSkillPvp( _item);
		SetDisableSkillRaid( _item);
		SetDisableSkillIndun( _item);
	}

	public void ResetItem()
	{
		ChargeCancel();
		if( 0 == getItemID)
			return;

		m_iItemCount = ItemMgr.HadItemManagement.Inven.GetItemTotalCount( getItemID);
		if( 0 == m_iItemCount)
		{			
			SetEnableIcon();
			return;
		}

		m_RealItem = ItemMgr.HadItemManagement.Inven.GetRealItem( m_iItemID);
		if( null == m_RealItem)
		{
			Debug.LogError( "AsSlot::SetItem()[ null == m_RealItem ] item id : " + getItemID + " count : " + m_iItemCount);
			return;
		}

		DeleteIcon();

		if( false == CreateIcon( m_RealItem.item.ItemData.GetIcon()))
		{
			Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] item id : " + getItemID + " icon path : " + m_RealItem.item.ItemData.GetIcon());
			return;
		}
		m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);

		
		UISlotItem slotItem = m_goIcon.GetComponent<UISlotItem>();
		if( null != slotItem)
			slotItem.SetItemCountText( m_iItemCount);

		Tbl_SkillLevel_Record skillData = AsTableManager.Instance.GetTbl_SkillLevel_Record( m_RealItem.item.ItemData.itemSkillLevel, m_RealItem.item.ItemData.itemSkill);
		ChargeTime = skillData.ChargeDuration * 0.001f;
		ChargeStep = skillData.ChargeMaxStep;
		SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_IT_IMMEDIATE);
	}

	public void SetEmpty()
	{
		DeleteIcon();
		
		m_iItemCount = 0;
		m_iItemID = 0;
		m_iSkillId = 0;
		m_iSkillLevel = 0;
		m_RealItem = null;
		m_sItem = null;
		SetType( SLOT_TYPE.SLT_CT_ITEM | SLOT_TYPE.SLT_INVALID);
	}

	private void SetEnableIcon()
	{
		if( null == m_goIcon)
			return;

		Transform ImgTrm = m_goIcon.transform.Find( "Img");
		if( null == ImgTrm)
			ImgTrm = m_goIcon.transform.Find( "img");

		if( null != ImgTrm && null != ImgTrm.renderer)
		{
			if( 0 < m_iItemCount)
				ImgTrm.renderer.material.SetColor( "_Color", Color.white);
			else
				ImgTrm.renderer.material.SetColor( "_Color", new Color( 0.4f, 0.4f, 0.4f, 0.5f));
		}

		UISlotItem slotItem = m_goIcon.GetComponent<UISlotItem>();
		if( null != slotItem)
			slotItem.SetItemCountText( m_iItemCount);
	}

	// skill
	public void SetSkill( int iSkill, int iLevel)
	{
		ChargeCancel();
		if( 0 == iSkill || 0 == iLevel)
		{
			m_curSkillLevelRecord = null;
			DeleteIcon();			
			iSkill = 0;
			iLevel = 0;
			SetType( SLOT_TYPE.SLT_CT_SKILL | SLOT_TYPE.SLT_INVALID);
			return;
		}

		/*if( iSkill == m_iSkillId && iLevel == m_iSkillLevel && null != m_goIcon)
		{
			m_goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
			return;
		}*/

		m_iItemID = 0;
		m_RealItem = null;
		m_sItem = null;
		m_iSkillId = iSkill;
		m_iSkillLevel = iLevel;

		Tbl_SkillLevel_Record curSkillLevelData = AsTableManager.Instance.GetTbl_SkillLevel_Record( m_iSkillLevel, m_iSkillId, 0);
		if( null == curSkillLevelData)
		{
			Debug.LogError( "AsSlot::SetSkill() [ null == curSkillLevelData ]");
			return;
		}
		m_curSkillLevelRecord = curSkillLevelData;

		DeleteIcon();
		CreateSkillIcon( iSkill, new Vector3( 0.0f, 0.0f, -0.5f));
		
		ChargeTime = curSkillLevelData.ChargeDuration * 0.001f;
		ChargeStep = curSkillLevelData.ChargeMaxStep;

		Tbl_Skill_Record skillrecord = AsTableManager.Instance.GetTbl_Skill_Record( iSkill);
		if( null != skillrecord)
		{
			SetType( SLOT_TYPE.SLT_CT_SKILL | GetSlotType( skillrecord.Skill_Type));
			m_UseOnly_Movable = skillrecord.CheckSkillUsingOnly_Movable();
			m_DisableSkill_PvP = ( skillrecord.DisableInPvP == eDisableInPvP.Disable);
			m_DisableSkill_Raid = ( skillrecord.DisableInRaid == eDisableInRaid.Disable);
            m_DisableSkill_Field = (skillrecord.DisableInField == eDisableInRaid.Disable);
			m_DisableSkill_Indun = ( skillrecord.DisableInInDun == eDisableInInDun.Disable);

			if( SkillBook.Instance.StanceInfo.StanceSkill == iSkill)
				StanceChanged( SkillBook.Instance.StanceInfo);
		}
	}

	public void SetSkillIcon( int iSkill, int iLevel)
	{
		if( m_iSkillId == iSkill && m_iSkillLevel == iLevel && null != m_goIcon)
			return;

		m_iSkillId = iSkill;
		m_iSkillLevel = iLevel;

		DeleteIcon();
		CreateSkillIcon( iSkill, new Vector3( 0.0f, 0.0f, -0.5f));
	}
	
	public void StanceChanged( StanceInfo _stance)
	{
		int lv = _stance.SkillLevel;
		Tbl_SkillLevel_Record lvRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record(lv, _stance.StanceSkill);
		Tbl_SkillLevel_Potency potency = lvRecord.listSkillLevelPotency[_stance.StancePotency];
		
		DeleteIcon();
		CreateSkillIcon( (int)potency.Potency_Value, new Vector3( 0.0f, 0.0f, -0.5f));
	}

	public AsSlot.SLOT_TYPE GetSlotType( eSKILL_TYPE eSkillType)
	{
		switch( eSkillType)
		{
		case eSKILL_TYPE.Charge:
		case eSKILL_TYPE.TargetCharge:
			return AsSlot.SLOT_TYPE.SLT_IT_CHARGE;
		case eSKILL_TYPE.Target:
		case eSKILL_TYPE.SlotBase:
			return AsSlot.SLOT_TYPE.SLT_IT_TARGET;
		}
	
		return AsSlot.SLOT_TYPE.SLT_IT_IMMEDIATE;
	}

	private void CreateSkillIcon( int iSkillId, Vector3 vecPos)
	{
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( iSkillId);
		if( null == record)
		{
			Debug.LogError( "AsSlot::CreateSkillIcon() [ null == record ] skill id : " + iSkillId);
			return;
		}

		if( false == CreateIcon( record.Skill_Icon))
		{
			Debug.LogError( "AsSlot::SetItem()[ false == CreateIcon() ] skill id : " + iSkillId + " icon path : " + record.Skill_Icon);
			return;
		}

		m_goIcon.transform.localPosition = vecPos;
	}

	public bool Show
	{
		set
		{
			isShow = value;
			collider.enabled = value;
			renderer.enabled = value;

			if( null != screen)
				screen.gameObject.SetActiveRecursively( value);

			if( null != txt)
				txt.gameObject.SetActiveRecursively( value);

			if( null != m_goIcon)
				m_goIcon.SetActiveRecursively( value);
		}
	}

	//end kij
	public bool Disable
	{
		set	{ isDisable = value; }
		get	{ return isDisable; }
	}

	public bool EnableText
	{
		get
		{
			if( null == txt)
				return false;
			return txt.enabled;
		}
		set
		{
			if( null != txt)
			{
				txt.enabled = value;
				txt.renderer.enabled = value;
			}
		}
	}

	private void OnEnable()
	{
//		SetIconEnableState( true);
		if( null != disableScreen)
			disableScreen.gameObject.SetActiveRecursively( false);
	}

	void Awake()
	{
		txt = gameObject.GetComponentInChildren<SpriteText>();
		if( null != txt)
			txt.Text = "";
	}

	// Use this for initialization
	void Start()
	{
		chargeBegined = false;

		// ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt, true);

		if( null != disableScreen)
			disableScreen.gameObject.SetActiveRecursively( false);
	}

	// Update is called once per frame
	void Update()
	{
		UpdateSlotState();

		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
		{
			if( ( 0 < Input.touchCount) && ( AsHudDlgMgr.eDlgPresentState.None == AsHudDlgMgr.dlgPresentState))
			{
				if( 1 < Input.touchCount)
				{
					if( true == chargeBegined)
						OnTouchUpAsButton();
				}
				else
				{
					Touch touch = Input.GetTouch( 0);

					switch( touch.phase)
					{
					case TouchPhase.Began:
						{
							if( false == AsUtil.PtInCollider( UIManager.instance.rayCamera, collider, touch.position))
								break;

							if( true == isClicked)
								break;

							isClicked = true;
							OnTouchDown();
						}
						break;
					case TouchPhase.Moved:
						{
							if( false == isClicked)
								break;
						
							if( false == AsUtil.PtInCollider( UIManager.instance.rayCamera, collider, touch.position))
							{
								isClicked = false;
								OnTouchExit();
							}
						}
						break;
					case TouchPhase.Ended:
						{
							if( false == isClicked)
						 		break;

							isClicked = false;
							OnTouchUpAsButton();
						}
						break;
					}
				}
			}
		}

		//begin kij
		CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup(); 	
		if( null != _cooltimeGroup)
		{
			if( true == _cooltimeGroup.isCoolTimeActive)
			{
				EnableText = true;
				screen.Enable = true;
				screen.Value = _cooltimeGroup.getCoolTimeValue;

				if( 1.0f <= screen.Value)
					AsSlotEffectManager.Instance.ShowUsableEffect( transform.position);

				txt.Text = AsMath.GetCoolTimeRemainTime( _cooltimeGroup.getRemainTime);
			}
			else if( true == EnableText || ( null != screen && true == screen.Enable))
			{
				EnableText = false;
				screen.Enable = false;
			}
		}
		else if( true == EnableText || ( null != screen && true == screen.Enable))
		{
			EnableText = false;
			if( null != screen)
				screen.Enable = false;
		}
		// end kij

		_UpdateChargeCancel(); // ilmeda, 20120817
	}
	
	private CoolTimeGroup GetCoolTimeGroup()
	{
		if( type == SLOT_TYPE.SLT_IT_POST )
			return null;
		
		CoolTimeGroup _cooltimeGroup = null; 
		if( true == IsEnableItem )
		{
			_cooltimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( m_RealItem.item.ItemData.itemSkill, m_RealItem.item.ItemData.itemSkillLevel);
		}
		else if( true == IsEnableSkill )			
		{
			_cooltimeGroup = CoolTimeGroupMgr.Instance.GetCoolTimeGroup( m_iSkillId, m_iSkillLevel);
		}
		
		return _cooltimeGroup;
	}

	public void SetType( SLOT_TYPE _type)
	{
		type = _type;
	}

	public void SetMainTex( Texture tex)
	{
		renderer.material.mainTexture = tex;
	}

	public void OuterInvoke()
	{
		if( true == TargetDecider.IsAvailableSkill( getSkillID, getSkillLevel))
		{
			CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
			
			if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
			{
				AsMyProperty.Instance.AlertCoolTime( COMMAND_SKILL_TYPE._NONE);
				AsSkillCoolTimeAlramDelegatorManager.Instance.AddSkillCoolTimeAlram( getSkillID, _cooltimeGroup.getRemainTime);
				return;
			}
		}
		else if( null != m_RealItem && 0 < m_iItemCount)
		{
			if( m_RealItem.item.ItemData.GetItemType() != Item.eITEM_TYPE.ActionItem)//$yde
				m_RealItem.SendUseItem();
		}
		//end kij

		if( true == isShow)
			AsSlotEffectManager.Instance.ShowUseEffect( transform.position);
	}

	void Invoke()
	{
		// begin kij
		if( 0 == getSkillID || 0 == getSkillLevel)
		{
			if( null != m_RealItem && 0 < m_iItemCount)
			{
				if( m_RealItem.item.ItemData.GetItemType() == Item.eITEM_TYPE.ActionItem )//$yde
				{
					
					CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
					if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
					{
						AsMyProperty.Instance.AlertCoolTime( COMMAND_SKILL_TYPE._NONE);
						AsSkillCoolTimeAlramDelegatorManager.Instance.AddSkillCoolTimeAlram( m_RealItem.item.ItemData.GetIcon(), _cooltimeGroup.getRemainTime);
						return;
					}

					UsingActionItem();
				}
				else
					m_RealItem.SendUseItem();
			}
		}
		else if( true == TargetDecider.IsAvailableSkill( getSkillID, getSkillLevel))
		{
			CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
			
			if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
			{
				AsMyProperty.Instance.AlertCoolTime( COMMAND_SKILL_TYPE._NONE);
				AsSkillCoolTimeAlramDelegatorManager.Instance.AddSkillCoolTimeAlram( getSkillID, _cooltimeGroup.getRemainTime);
				return;
			}

			switch( type)
			{
			case SLOT_TYPE.SLT_IT_SHOP:
				break;
			case SLOT_TYPE.SLT_IT_BOOK:
				break;
			default:
				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Input_Slot_Active( getSkillID, AsSlotEffectManager.Instance.chargeGuage.Step));
				break;
			}
		}
		//end kij

		if( true == isShow)
			AsSlotEffectManager.Instance.ShowUseEffect( transform.position);
	}

	#region - using action item -
	void UsingActionItem()
	{
		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		user.HandleMessage( new Msg_Player_Use_ActionItem( m_RealItem));
	}
	#endregion

	private bool IsSufficientMP()
	{
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( getSkillLevel, getSkillID, 1);
		if( null == skillLevelRecord)
			return false;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		if( curMP < skillLevelRecord.Mp_Decrease)
			return false;

		return true;
	}

	public void Charge()
	{
		CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
		if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
			return;

		if( false == IsSufficientMP())
		{
			AsMyProperty.Instance.AlertMP();
			return;
		}

		AsSlotEffectManager.Instance.ShowChargeEffect( transform.position);
		AsSlotEffectManager.Instance.chargeGuage.ParentSlot = this;
		AsSlotEffectManager.Instance.chargeGuage.Begin( ChargeTime, ChargeStep, getSkillID, getSkillLevel);

		chargeBegined = true;
	}

	public void ChargeCancel()
	{
		chargeBegined = false;
		AsSlotEffectManager.Instance.chargeGuage.Cancel();
		AsSlotEffectManager.Instance.HideChargeEffect();
	}

	public void ChargeComplete( int step)
	{
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( getSkillLevel, getSkillID, step);
		if( null == skillLevelRecord)
			return;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		userEntity.SetProperty( eComponentProperty.MP_CUR, curMP - skillLevelRecord.Mp_Decrease);
	}

	public void StepChanged( int step)
	{
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( getSkillLevel, getSkillID, step);
		if( null == skillLevelRecord)
			return;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		userEntity.SetProperty( eComponentProperty.MP_CUR, curMP - skillLevelRecord.Mp_Decrease);
	}
	
	void OnMouseDown()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
			return;

		m_isMouseDown = true;
		
		if( eUSE_TYPE.QUICK_SLOT == UseType && true == AsQuickSlotManager.Instance.isLock)
			return;

		if( eUSE_TYPE.SKILL_BOOK == UseType)
			return;

		if( false == isUsable)
			return;

		if( true == isDisable)
			return;
		
		CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
		if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
			return;

		if( false == CheckStatus())
			return;

		SLOT_TYPE eInvokeType = type & SLOT_TYPE.FILTER_INVOKE_TYPE;
		switch( eInvokeType)
		{
		case SLOT_TYPE.SLT_IT_CHARGE:
			{
				if( true == IsEnableSkill)
				{
					if( false == TargetDecider.IsAvailableSkill( getSkillID, getSkillLevel, false))
						break;

					if( false == IsSufficientMP())
					{
						AsMyProperty.Instance.AlertMP();
						break;
					}

					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Input_Begin_Charge( getSkillID));
				}
			}
			break;
		}
		
		if( eUSE_TYPE.QUICK_SLOT == UseType )
		{
			AsQuickSlotManager.Instance.CancelCheckReduceQuickSlot();
		}
	}

	void OnTouchDown()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
		{
			m_isMouseDown = true;
			
			if( eUSE_TYPE.QUICK_SLOT == UseType && true == AsQuickSlotManager.Instance.isLock)
				return;

			if( eUSE_TYPE.SKILL_BOOK == UseType)
				return;

			if( false == isUsable)
				return;

			if( true == isDisable)
				return;
			
			CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
			if( null != _cooltimeGroup && true == _cooltimeGroup.isCoolTimeActive)
				return;

			if( false == CheckStatus())
				return;

			SLOT_TYPE eInvokeType = type & SLOT_TYPE.FILTER_INVOKE_TYPE;
			switch( eInvokeType)
			{
			case SLOT_TYPE.SLT_IT_CHARGE:
				{
					if( true == IsEnableSkill)
					{
						if( false == TargetDecider.IsAvailableSkill( getSkillID, getSkillLevel, false))
							break;

						if( false == IsSufficientMP())
						{
							AsMyProperty.Instance.AlertMP();
							break;
						}

						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Input_Begin_Charge( getSkillID));
					}
				}
				break;
			}
			
			if( eUSE_TYPE.QUICK_SLOT == UseType )
			{
				AsQuickSlotManager.Instance.CancelCheckReduceQuickSlot();
			}
		}
	}

	void OnMouseExit()
	{
		if( eUSE_TYPE.QUICK_SLOT == UseType && true == AsQuickSlotManager.Instance.isLock)
			return;

		if( eUSE_TYPE.SKILL_BOOK == UseType)
			return;

		if( false == isUsable)
			return;

		// ilmeda, 20120817
		if( true == chargeBegined)
			_OnMouseUpAsButton();
		
		if( eUSE_TYPE.QUICK_SLOT == UseType && m_isMouseDown == true )
		{
			AsQuickSlotManager.Instance.RefreshReduceQuickSlot();
		}
	}

	void OnTouchExit()
	{
		if( eUSE_TYPE.QUICK_SLOT == UseType && true == AsQuickSlotManager.Instance.isLock)
			return;

		if( eUSE_TYPE.SKILL_BOOK == UseType)
			return;

		if( false == isUsable)
			return;

		// ilmeda, 20120817
		if( true == chargeBegined)
			_OnMouseUpAsButton();
		
		if( eUSE_TYPE.QUICK_SLOT == UseType && m_isMouseDown == true )
		{
			AsQuickSlotManager.Instance.RefreshReduceQuickSlot();
		}
	}

	void OnMouseUpAsButton()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
			return;

		m_isMouseDown = false;
		
		if( false == AsUtil.PtInCollider( UIManager.instance.rayCamera, collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
			return;

		_OnMouseUpAsButton();
	}

	void OnTouchUpAsButton()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
		{
			m_isMouseDown = false;

			_OnMouseUpAsButton();
		}
	}

	private void _OnMouseUpAsButton()
	{
		if( eUSE_TYPE.QUICK_SLOT == UseType && true == AsQuickSlotManager.Instance.isLock)
			return;

		if( eUSE_TYPE.SKILL_BOOK == UseType)
			return;

		if( false == isUsable)
			return;

		if( true == isDisable)
			return;

		if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			return;

		Tbl_Skill_Record skillRecord = null;
		if( 0 < getSkillID)
		{
			skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( getSkillID);
		}

		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null != playerFsm)
		{
			if( false == playerFsm.Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) &&
				true == m_UseOnly_Movable)
			{
				if( CheckActionItem_NeedAnimation() == false)//$yde
				{
					Invoke();
					return;
				}

				AsMyProperty.Instance.AlertState();
				return;
			}

			if( true == playerFsm.Entity.CheckCondition( eSkillIcon_Enable_Condition.Stun) ||
				true == playerFsm.Entity.CheckCondition( eSkillIcon_Enable_Condition.Freeze) ||
				true == playerFsm.Entity.CheckCondition( eSkillIcon_Enable_Condition.Sleep) ||
				true == playerFsm.Entity.CheckCondition( eSkillIcon_Enable_Condition.Fear))
			{
				if( CheckActionItem_NeedAnimation() == false)//$yde
					Invoke();
				else
					AsMyProperty.Instance.AlertState();
				
				return;
			}

			if( true == TargetDecider.CheckCurrentMapIsArena() && true == m_DisableSkill_PvP)
			{				
				AsMyProperty.Instance.AlertNotInPvp();
				return;
			}
				
			if( true == TargetDecider.CheckCurrentMapIsRaid() && true == m_DisableSkill_Raid)
			{					
				AsMyProperty.Instance.AlertNotInRaid();
				return;			
			}

            if (true == TargetDecider.CheckCurrentMapIsField() && true == m_DisableSkill_Field)
            {
                AsMyProperty.Instance.AlertNotInField();
                return;
            }

			if( true == TargetDecider.CheckCurrentMapIsIndun() && true == m_DisableSkill_Indun)
			{
				AsMyProperty.Instance.AlertNotInIndun();
				return;
			}

			if( AsPlayerFsm.ePlayerFsmStateType.DEATH == playerFsm.CurrnetFsmStateType)
				return;
		}

		SLOT_TYPE eInvokeType = type & SLOT_TYPE.FILTER_INVOKE_TYPE;
		switch( eInvokeType)
		{
		case SLOT_TYPE.SLT_IT_CHARGE:
			{
				if( false == chargeBegined)
				{
					AsMyProperty.Instance.AlertCoolTime( COMMAND_SKILL_TYPE._NONE);
					break;
				}

				if( 0 < AsSlotEffectManager.Instance.chargeGuage.Step)
				{
					if( null == skillRecord)
						break;

					if( true == TargetDecider.SkillTargetStateCheck( skillRecord, playerFsm.UserEntity))
						Invoke();
				}
				else 
				{
					if( false == IsEnableItem )
					{
						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Input_Cancel_Charge());
						Tbl_SkillLevel_Record curSkillLevelData = AsTableManager.Instance.GetTbl_SkillLevel_Record( getSkillLevel, getSkillID);
					
						CoolTimeGroup _cooltimeGroup = GetCoolTimeGroup();
						if( null != _cooltimeGroup && false == _cooltimeGroup.isCoolTimeActive && null != curSkillLevelData )
							_cooltimeGroup.RemainCooltime( curSkillLevelData.CoolTime);
					}
					else
					{
						Debug.LogError("AsSlot::_OnMouseUpAsButton()[ true == IsEnableItem ] id : " + getItemID );
					}
				}				

				ChargeCancel();
			}
			break;
		case SLOT_TYPE.SLT_IT_TARGET:
			{
				if( null == skillRecord)
					break;

				TargetDecider.sTargeting_AsSlot( playerFsm, skillRecord);//$yde

				if( null == playerFsm.Target)
				{
					if( true == playerFsm.CheckMap_Village())
						AsMyProperty.Instance.AlertSkillInTown();
					else
						AsMyProperty.Instance.AlertInvalidTarget();
				}
				else
				{
					if( true == TargetDecider.TargetSkillCheck( getSkillID, playerFsm))
						Invoke();
				}
			}
			break;
		case SLOT_TYPE.SLT_IT_IMMEDIATE:
			{
				if( 0 != m_iItemID)
				{
					if( true == IsEnableItem)
						Invoke();
				}
				else
				{
					if( null == skillRecord)
						break;

					if( true == TargetDecider.SkillTargetStateCheck( skillRecord, playerFsm.UserEntity))
						Invoke();
					else if( skillRecord.CheckNonAnimation() == true)
						Invoke();
				}
			}
			break;
		}
		
		if( eUSE_TYPE.QUICK_SLOT == UseType )
		{
			AsQuickSlotManager.Instance.RefreshReduceQuickSlot();
		}
	}

	#region - obsolete -
//	private bool SkillTargetStateCheck( Tbl_Skill_Record skillRecord, AsBaseEntity entity)
//	{
//		bool ret = false;
//
//		switch( skillRecord.SkillIcon_Enable_Condition)
//		{
//		case eSkillIcon_Enable_Condition.NONE:
//			return true;
//		case eSkillIcon_Enable_Condition.LowHealth:
//			float curHP = entity.GetProperty<float>( eComponentProperty.HP_CUR);
//			if( curHP < skillRecord.SkillIcon_Enable_ConditionValue)
//				ret = true;
//			break;
//		case eSkillIcon_Enable_Condition.Death:
//			bool isDied = entity.GetProperty<bool>( eComponentProperty.LIVING);
//			ret = !isDied;
//			break;
//		case eSkillIcon_Enable_Condition.Movable:
//			ret = CheckStatus();
//			break;
//		default:
//			ret = entity.CheckCondition( skillRecord.SkillIcon_Enable_Condition);
//			break;
//		}
//
//		if( false == ret)
//			AsMyProperty.Instance.AlertState();
//
//		return ret;
//	}

//	private bool SkillTargetTypeCheck( Tbl_Skill_Record skillRecord, AsPlayerFsm fsm)
//	{
//		switch( skillRecord.SkillIcon_Enable_Target)
//		{
//		case eSkillIcon_Enable_Target.Self:
//			return SkillTargetStateCheck( skillRecord, fsm.UserEntity);
//		case eSkillIcon_Enable_Target.Target:
//			return SkillTargetStateCheck( skillRecord, fsm.Target);
//		}
//
//		return true;
//	}

//	private bool TargetSkillCheck( AsPlayerFsm fsm)
//	{
//		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( getSkillID);
//		if( null == skillRecord)
//			return false;
//
//		switch( skillRecord.Skill_TargetType)
//		{
//		case eSkill_TargetType.All:
//			{
//				if( eFsmType.NPC == fsm.Target.FsmType)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//
//				return SkillTargetTypeCheck( skillRecord, fsm);
//			}
//			break;
//		case eSkill_TargetType.Alliance:
//			{
//				if( eEntityType.NPC == fsm.Target.EntityType)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//
//				return SkillTargetTypeCheck( skillRecord, fsm);
//			}
//			break;
//		case eSkill_TargetType.Enemy:
//			{
//				if( eFsmType.MONSTER != fsm.Target.FsmType)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//
//				return SkillTargetTypeCheck( skillRecord, fsm);
//			}
//			break;
//		case eSkill_TargetType.Party:
//			{
//				if( eFsmType.OTHER_USER != fsm.Target.FsmType)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//
//				if( false == AsPartyManager.Instance.IsPartying)
//					return false;
//
//				AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( ( ( AsUserEntity)fsm.Target).UniqueId);
//				if( null == partyUser)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//			}
//			break;
//		case eSkill_TargetType.Self:
//			{
//				if( eFsmType.PLAYER != fsm.Target.FsmType)
//				{
//					AsMyProperty.Instance.AlertTarget();
//					return false;
//				}
//
//				return SkillTargetTypeCheck( skillRecord, fsm);
//			}
//			break;
//		}
//
//		return true;
//	}

//	bool IsAvailableSkill( bool checkMP=true)
//	{
//		if( false == IsEnableSkill)
//			return false;
//
//		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//		if( false == userEntity.WeaponEquip)
//		{
//			AsMyProperty.Instance.AlertNoWeapon();
//			return false;
//		}
//
//		if( false == AsSkillDelegatorManager.Instance.IsActionCancelTime( Time.time))
//			return false;
//
//		if( true == checkMP)
//		{
//			float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
//			Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( getSkillLevel, getSkillID);
//			if( curMP < skillLevelRecord.Mp_Decrease)
//			{
//				AsMyProperty.Instance.AlertMP();
//				return false;
//			}
//		}
//
//		if( true == userEntity.CheckTargetShopOpening())
//		{
//			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( getSkillID);
//
//			if( skillRecord.Skill_Type == eSKILL_TYPE.Target)
//			{
//				AsMyProperty.Instance.AlertTarget();
//				return false;
//			}
//		}
//
//		return true;
//	}
	#endregion

	// < ilmeda, 20120817
	private void _UpdateChargeCancel()  
	{
		if( false == isShow)
			return;

		if( true == isDisable)
			return;

		if( false == chargeBegined)
			return;

		if( false == CheckStatus())
			ChargeCancel();
	}
	// ilmeda, 20120817 >

	private bool CheckMana()
	{
		if( null == m_curSkillLevelRecord)
			return false;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return false;

		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);

		return ( curMP < m_curSkillLevelRecord.Mp_Decrease) ? false : true;
	}
	
	private bool CheckStatus()
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		Debug.Assert( null != playerFsm);
		AsBaseEntity baseEntity = playerFsm.Entity;

		if( AsPlayerFsm.ePlayerFsmStateType.DEATH == playerFsm.CurrnetFsmStateType)
			return false;

		//$yde
		if( (true == TargetDecider.CheckCurrentMapIsArena() &&
			true == m_DisableSkill_PvP) ||
			(true == TargetDecider.CheckCurrentMapIsRaid() &&
            true == m_DisableSkill_Raid) ||
            (true == TargetDecider.CheckCurrentMapIsField() &&
            true == m_DisableSkill_Field) ||
		    (true == TargetDecider.CheckCurrentMapIsIndun() &&
		    true == m_DisableSkill_Indun))
		{
			return false;
		}
		
		//$yde
		if( false == baseEntity.CheckCondition( eSkillIcon_Enable_Condition.Movable) &&
			true == m_UseOnly_Movable)
			return false;

		//$yde
		if( true == baseEntity.CheckCondition( eSkillIcon_Enable_Condition.Stun) ||
			true == baseEntity.CheckCondition( eSkillIcon_Enable_Condition.Freeze) ||
			true == baseEntity.CheckCondition( eSkillIcon_Enable_Condition.Sleep) ||
			true == baseEntity.CheckCondition( eSkillIcon_Enable_Condition.Fear))
		{
			if( CheckActionItem_NeedAnimation() == false)
				return true;
			else
				return false;
		}

		//$yde
		if( CheckEnableSkillOnTarget() == false)
			return false;

		return true;
	}

	private bool CheckActionItem_NeedAnimation()//$yde
	{
		SLOT_TYPE invoke = type & SLOT_TYPE.FILTER_CONTENT;
		if( invoke == SLOT_TYPE.SLT_CT_ITEM)
		{
			if( null == m_RealItem)
				return true;

			Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record( m_RealItem.item.ItemData.itemSkill);

			if( null == skill)
				return true;

			if( skill.CheckNonAnimation() == true)
			{
				return false;
			}
		}

		return true;
	}

	void SetIconEnableState( bool flag)
	{
		if( null == m_goIcon)
			return;

		Transform imgTrm = m_goIcon.transform.Find( "Img");
		if( null == imgTrm)
			imgTrm = m_goIcon.transform.Find( "img");

		if( null != imgTrm && null != imgTrm.renderer)
		{
			if( SLOT_TYPE.SLT_CT_ITEM == ( type & SLOT_TYPE.FILTER_CONTENT))
			{
				if( 0 < m_iItemCount)
					imgTrm.renderer.material.SetColor( "_Color", Color.white);
				else
					imgTrm.renderer.material.SetColor( "_Color", new Color( 0.4f, 0.4f, 0.4f, 0.5f));
			}
			else
			{
				if( true == flag)
					imgTrm.renderer.material.SetColor( "_Color", Color.white);
				else
					imgTrm.renderer.material.SetColor( "_Color", new Color( 1.0f, 0.5f, 0.5f, 1.0f));
			}
		}
	}

	void SetActionItemState( bool flag)
	{
		if( null == m_goIcon)
			return;

		Transform imgTrm = m_goIcon.transform.Find( "Img");
		if( null == imgTrm)
			imgTrm = m_goIcon.transform.Find( "img");

		if( null != imgTrm && null != imgTrm.renderer)
		{
			if( true == flag)
			{
				if( 0 < m_iItemCount)
					imgTrm.renderer.material.SetColor( "_Color", Color.white);
				else
					imgTrm.renderer.material.SetColor( "_Color", new Color( 0.4f, 0.4f, 0.4f, 0.5f));
			}
			else
				imgTrm.renderer.material.SetColor( "_Color", new Color( 1.0f, 0.5f, 0.5f, 1.0f));
		}
	}

	private void UpdateSlotState()
	{
		if( false == isShow)
		{
			if( null != disableScreen && false != disableScreen.gameObject.active)
				disableScreen.gameObject.SetActiveRecursively( false);
			return;
		}

		bool bActive = CheckMana() && CheckStatus();

		switch( type & SLOT_TYPE.FILTER_CONTENT)
		{
		case SLOT_TYPE.SLT_CT_ITEM:
			if( null != disableScreen)
			{
				if( null == getRealItem )				
					disableScreen.gameObject.SetActiveRecursively( false);
				else if( CheckStatus() == false)//$yde
					disableScreen.gameObject.SetActiveRecursively( true);
				else if( CheckPlayerDeath() == true)
					disableScreen.gameObject.SetActiveRecursively( true);
				else
					disableScreen.gameObject.SetActiveRecursively( false);
			}
			break;
		case SLOT_TYPE.SLT_CT_SKILL:
			if( 0 == m_iSkillId )
			{
				disableScreen.gameObject.SetActiveRecursively( false);
			}
			else
			{				
				switch( type & SLOT_TYPE.FILTER_INVOKE_TYPE)
				{
				case SLOT_TYPE.SLT_IT_IMMEDIATE:
				case SLOT_TYPE.SLT_IT_CHARGE:
				case SLOT_TYPE.SLT_IT_TARGET:
					if( null != disableScreen && bActive == disableScreen.gameObject.active)
						disableScreen.gameObject.SetActiveRecursively( !bActive);
					break;
				case SLOT_TYPE.SLT_INVALID:
					if( null != disableScreen && false != disableScreen.gameObject.active)
						disableScreen.gameObject.SetActiveRecursively( false);
					break;
				}
			}
			break;
		}
	}

	bool CheckPlayerDeath()//$yde
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		Debug.Assert( null != playerFsm);
		AsBaseEntity baseEntity = playerFsm.Entity;

		if( AsPlayerFsm.ePlayerFsmStateType.DEATH == playerFsm.CurrnetFsmStateType)
			return true;
		else
			return false;
	}

	bool CheckEnableSkillOnTarget()
	{
		Tbl_Skill_Record skill = null;

		switch( type & SLOT_TYPE.FILTER_CONTENT)
		{
		case SLOT_TYPE.SLT_CT_ITEM:
			if( m_RealItem != null)
				skill = AsTableManager.Instance.GetTbl_Skill_Record( m_RealItem.item.ItemData.itemSkill);
			break;
		case SLOT_TYPE.SLT_CT_SKILL:
			if( 0 != m_iSkillId && int.MaxValue != m_iSkillId )
				skill = AsTableManager.Instance.GetTbl_Skill_Record( m_iSkillId);
			break;
		}

		if( skill == null)
			return true;

		if( skill.SkillIcon_Enable_Target == eSkillIcon_Enable_Target.Target)
		{
			AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
			Debug.Assert( null != playerFsm);

			if( playerFsm.Target == null)
				return true;

			return TargetDecider.SkillTargetStateCheck( skill, playerFsm.Target, false);
		}

		return true;
	}
}
