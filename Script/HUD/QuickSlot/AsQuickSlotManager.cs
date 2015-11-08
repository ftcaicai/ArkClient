using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsQuickSlotManager : MonoBehaviour
{
	private static AsQuickSlotManager instance = null;
	public static AsQuickSlotManager Instance
	{
		get { return instance; }
	}

	public AsQuickSlot[] slotArray = new AsQuickSlot[0];
	public UIStateToggleBtn rbtnLock;
	public UIButton changeBtn;
	private bool m_isLock = false;
	public AsSkillTooltipInBook toolTipInBook = null;
	public SpriteText textEditSlot;
	public GameObject goEditSlotImg;
	public SimpleSprite[] pages = new SimpleSprite[0];

	public float m_battleLockMsgMaxTime = 3f;
	private float m_battleLockMsgTime = 0f;
	private bool m_isBattleLockShow = false;
	
	private AsSkillTooltipInBook m_skillTooltip = null;
	
	private List<AsSlot>		m_arraySlot = new List<AsSlot>();
	private bool				m_isCheckReduceQuickSlot = false;
	private bool				m_isCancelCheckReduceQuickSlot = false;
	private	float				m_fReduceTime = 0;
	private float				m_fReduceTimeDelay 	= 3.0f;
	private	float				m_fForwardZValue	= -2.0f;
	private	float				m_fBackwardZValue	= 7.0f;
	
	public bool isOpenSkillTooltip
	{
		get
		{
			return null != m_skillTooltip;
		}
	}
	
	public void CloseSkillTooltip()
	{
		if( null == m_skillTooltip )
			return;
		
		GameObject.Destroy( m_skillTooltip.gameObject );			
	}

//	[HideInInspector]
//	public float actionStartTime = 0.0f;
//	[HideInInspector]
//	public float actionEndTime = 0.0f;
//	[HideInInspector]
//	public float actionTime = 0.0f;
//	[HideInInspector]
//	public AsSlot waitingSlot = null;

	// item move active time
	public float m_fMaxItemMoveTime = 0.5f;
	private float m_fItemMoveTime = 0.0f;
	private GameObject m_goMoveIcon;
	private AsSlot m_ClickDownSlot;
	private int m_iClickDownQuickSlotIndex = -1;
	private Vector3 m_tipPosition = new Vector3( -500.0f, 0.0f, -10.0f);

	private string strIconUpSound = "Sound/Interface/S6025_EFF_SlotUp";
	private string strIconDownSound = "Sound/Interface/S6026_EFF_SlotDown";
	private string strIconDeleteSounc = "Sound/Interface/S6027_EFF_SlotDel";
	[SerializeField]GameObject activateAlarmBalloon = null;
	[SerializeField]SpriteText activateAlarmText = null;
	[SerializeField]AsSpriteBlinker activateEffect = null;
	private int activatedPage = 0;

	public void PromprtActivateAlarm( int page)
	{
		return;	// not use
		
		if( activatedPage >= page)
			return;

		activatedPage = page;

		if( true == m_isLock)
			return;

		activateAlarmBalloon.SetActiveRecursively( true);
		activateEffect.Play();

		switch( page)
		{
		case 2:	activateAlarmText.Text = AsTableManager.Instance.GetTbl_String(2018);	break;
		case 3:	activateAlarmText.Text = AsTableManager.Instance.GetTbl_String(2019);	break;
		case 4:	activateAlarmText.Text = AsTableManager.Instance.GetTbl_String(2020);	break;
		}
	}

	public void HideActivateAlarm()
	{
		activateAlarmBalloon.SetActiveRecursively( false);
	}

	public void SetEditSlotShow( bool isActive)
	{
		if( null == goEditSlotImg)
			return;

		if( isActive == goEditSlotImg.active)
			return;

		goEditSlotImg.SetActiveRecursively( isActive);
		
		if( isActive == true )
		{
			Vector3 nowPos = goEditSlotImg.transform.position;
			nowPos.z = -2;
			goEditSlotImg.transform.position = nowPos;
			
			StartCoroutine( _CheckDisappearEditSlotImg());
		}
	}
	
	private IEnumerator _CheckDisappearEditSlotImg()
	{
		yield return new WaitForSeconds( 3.0f );
		
		goEditSlotImg.SetActive(false);
	}
	

	public void Init()
	{
		ReduceQuickSlot();
		Backward();
		SetLock(false);
		
		HideActivateAlarm();
//		SetLock( m_isLock);
		SetViewPage(0);
	}

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start()
	{
		m_battleLockMsgTime = 0f;
 		m_isBattleLockShow = false;
		activatedPage = 0;

		if( null != rbtnLock)
			rbtnLock.AddInputDelegate( LockRbtnDelegate);

		if( null != changeBtn)
			changeBtn.AddInputDelegate( ChangeBtnDelegate);
		
		
		foreach( AsQuickSlot quickSlot in slotArray)
		{
			foreach( AsSlot slot in quickSlot.packSlot.slotArray )
			{
				m_arraySlot.Add( slot );
			}
		}
		
		ReduceQuickSlot();
		
		Backward();
		
		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record("QuickSlot_Duration");
		if( null != record)
			m_fReduceTimeDelay = record.Value * 0.001f;
	}
	
	public bool isLock
	{
		get	{ return m_isLock; }
	}

	private void SetLock( bool isLock)
	{
		m_isLock = isLock;

		foreach( AsQuickSlot quickSlot in slotArray)		
			quickSlot.SetLock( m_isLock );
		
		rbtnLock.SetState( m_isLock ? 1 : 0 );
		
		if( m_isLock == true && textEditSlot != null )
			textEditSlot.Text = AsTableManager.Instance.GetTbl_String(1615);
		
		SetEditSlotShow( m_isLock);
	}

	private void LockRbtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			bool _isLock = !m_isLock;
			if( AsUserInfo.Instance.IsBattle() && true == _isLock)
			{
				if( false == m_isBattleLockShow)
				{
					SetEditSlotShow( true);
					m_battleLockMsgTime = 0f;
					m_isBattleLockShow = true;
					if( null != textEditSlot)
						textEditSlot.Text = AsTableManager.Instance.GetTbl_String(1734);
				}
				rbtnLock.SetToggleState( 1);
			}
			else
			{
				SetLock( _isLock);
				
				if( _isLock == true )
					ExpandQuickSlot(true);
				else
					ReduceQuickSlot();
			}
		}
	}

	private void ChangeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_isLock == true )
				return;
			
			if( IsExpandQuickSlot() == false )
				ExpandQuickSlot();
			else
				ReduceQuickSlot();

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			HideActivateAlarm();
		}
	}

	private void SetViewPage( int iValue)
	{
		foreach( SimpleSprite page in pages)
			page.gameObject.SetActive(false);
		
		if( iValue < pages.Length )
			pages[ iValue].gameObject.SetActive( true);
	}

	public void Clear()
	{
		foreach( AsSlot _slot in m_arraySlot )
		{
			_slot.SetEmpty();
		}
	}

	private void SetSlotsInSlot( AsSlot _slot, int iBeginIndex, sQUICKSLOT[] qucikSlots, int i)
	{
		if( iBeginIndex+i >= qucikSlots.Length )
		{
			Debug.LogError("AsQuickSlotManager SetSlotsInSlot array index overflow![" + (iBeginIndex+i).ToString() + "]"); 
			return;
		}
		
		sQUICKSLOT _quickSlot = qucikSlots[ iBeginIndex + i];
		if( (int)eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM == _quickSlot.eType)
		{
			if( _quickSlot.nValue == 0)
				_slot.SetEmpty();
			else
				_slot.SetItem( _quickSlot.nValue);
		}
		else if( (int)eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL == _quickSlot.eType)
		{
			if( _quickSlot.nValue == 0)
			{
				_slot.SetEmpty();
			}
			else if( SkillBook.Instance.dicCurSkill.ContainsKey(_quickSlot.nValue) ) 
			{
				SkillView _skill = SkillBook.Instance.dicCurSkill[_quickSlot.nValue];
				_slot.SetSkill( _skill.nSkillTableIdx, _skill.nSkillLevel);
			}
		}
		else
		{
			_slot.SetEmpty();
		}
	}

	public void SetSlots( sQUICKSLOT[] qucikSlots)
	{
		int iBeginIndex = 0;
		
		int	nItemSeq = 0;
//		foreach( AsSlot _slot in m_arraySlot )
		for(int i=0;i<m_arraySlot.Count;i++)
		{
			AsSlot _slot = m_arraySlot[i];
			SetSlotsInSlot( _slot, iBeginIndex , qucikSlots, nItemSeq++ );
		}
	}

	// Update is called once per frame
	void Update()
	{
		if( true == m_isBattleLockShow)
		{
			m_battleLockMsgTime += Time.deltaTime;
			if( m_battleLockMsgTime >= m_battleLockMsgMaxTime)
			{
				m_battleLockMsgTime = 0f;
 				m_isBattleLockShow = false;
				SetEditSlotShow( false);
			}
		}
	}

	public void BeginCooltime( int skillID)
	{
		foreach( AsQuickSlot quickSlot in slotArray)
			quickSlot.BeginCooltime( skillID);
	}

	public void BeginCharge( int skillID)
	{
		foreach( AsQuickSlot quickSlot in slotArray)
			quickSlot.BeginCharge( skillID);
	}

	public bool SetMoveInvenSlotInItemSlot( Ray inputRay, UIInvenSlot _InvenSlot)
	{
		if( null == _InvenSlot)
			return false;

		if( null == _InvenSlot.slotItem)
			return false;

		bool isUseQuickSlot = false;
		if( Item.eITEM_TYPE.ActionItem == _InvenSlot.slotItem.realItem.item.ItemData.GetItemType())
		{
			isUseQuickSlot = true;
		}

		if( Item.eITEM_TYPE.UseItem == _InvenSlot.slotItem.realItem.item.ItemData.GetItemType())
		{
			Item.eUSE_ITEM subtype = (Item.eUSE_ITEM)_InvenSlot.slotItem.realItem.item.ItemData.GetSubType();
			if( Item.eUSE_ITEM.InfiniteQuest == subtype ||
				Item.eUSE_ITEM.ConsumeQuest == subtype)
				isUseQuickSlot = true;
		}

		if( false == isUseQuickSlot)
			return false;

		short	slotIndex = 0;
		foreach( AsSlot _asslot in m_arraySlot )
		{
			if( false == _asslot.Disable && true == AsUtil.PtInCollider( _asslot.collider, inputRay))
			{
				int iItemID = _InvenSlot.slotItem.realItem.item.ItemID;

				AsCommonSender.SendQuickslotChange( slotIndex, iItemID, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM);
				ItemMgr.HadItemManagement.QuickSlot.SetQuickSlot( slotIndex, iItemID, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM);
				AsSoundManager.Instance.PlaySound( strIconDownSound, Vector3.zero, false);
				return true;
			}
			slotIndex++;
		}

		return false;
	}

	public bool SetMoveInvenSlotInSkill( Ray inputRay, int _skillid, int _skilllevel)
	{
		short	slotIndex = 0;
		foreach( AsSlot _asslot in m_arraySlot )
		{
			if( false == _asslot.Disable && true == AsUtil.PtInCollider( _asslot.collider, inputRay))
			{
				AsCommonSender.SendQuickslotChange( slotIndex, _skillid, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL);
				ItemMgr.HadItemManagement.QuickSlot.SetQuickSlot( slotIndex, _skillid, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL);
				AsSoundManager.Instance.PlaySound( strIconDownSound, Vector3.zero, false);
				return true;
			}
			slotIndex++;
		}
		
		return false;
	}
	
	//$yde
	public void ChangeStance(StanceInfo _stance)
	{
		foreach( AsQuickSlot quickSlot in slotArray)
			quickSlot.ChangeStance(_stance);
	}

	private bool GuiInputUpSlot( AsSlot _slot, Ray inputRay, int i)
	{
		if( true == AsUtil.PtInCollider( _slot.collider, inputRay))
		{
			if( _slot == m_ClickDownSlot)
			{
				m_goMoveIcon.transform.position = m_ClickDownSlot.transform.position;
				m_goMoveIcon.transform.localPosition = new Vector3( 0f, 0f, -0.5f);
			}
			else
			{
				int slot_1 = m_iClickDownQuickSlotIndex;
				int data_1 = 0;//slot.getItemID;
				eQUICKSLOT_TYPE type_1 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_NOTHING;

				int slot_2 = i;
				int data_2 = 0;//m_ClickDownSlot.getItemID;
				eQUICKSLOT_TYPE type_2 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_NOTHING;

				if( _slot.IsEnableItem)
				{
					data_1 = _slot.getItemID;
					type_1 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM;
				}
				else if( _slot.IsEnableSkill)
				{
					data_1 = _slot.getSkillID;
					type_1 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL;
				}

				if( m_ClickDownSlot.IsEnableItem)
				{
					data_2 = m_ClickDownSlot.getItemID;
					type_2 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM;
				}
				else if( m_ClickDownSlot.IsEnableSkill)
				{
					data_2 = m_ClickDownSlot.getSkillID;
					type_2 = eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL;
				}

				AsCommonSender.SendQuickslotChange( (short)slot_1, data_1, type_1);
				AsCommonSender.SendQuickslotChange( (short)slot_2, data_2, type_2);
				ItemMgr.HadItemManagement.QuickSlot.SetQuickSlot( slot_1, data_1, type_1);
				ItemMgr.HadItemManagement.QuickSlot.SetQuickSlot( slot_2, data_2, type_2);

				AsSoundManager.Instance.PlaySound( strIconDownSound, Vector3.zero, false);
			}
			return true;
		}

		return false;
	}

	private bool GuiInputUpItem(Ray inputRay)
	{
		int	nSlotIndex = 0;
		foreach( AsSlot _slot in m_arraySlot )
		{
			if( _slot.Disable == true )
			{
				nSlotIndex++;
				continue;
			}
			
			if( true == GuiInputUpSlot( _slot, inputRay, nSlotIndex++))
				return true;
		}

		return false;
	}

	public void GuiInputDown( Ray inputRay)
	{
		if( AsPStoreManager.Instance.UnableActionByPStore() == true) //$yde
			return;

		if( false == isLock)
			return;

		m_ClickDownSlot = null;
		m_fItemMoveTime = 0.0f;
		m_iClickDownQuickSlotIndex = -1;

		int	nSlotIndex = 0;
		foreach( AsSlot _slot in m_arraySlot )
		{
			if( false == _slot.Disable && true == AsUtil.PtInCollider( _slot.collider, inputRay))
			{
				if( ( null != _slot.getIcon) && ( ( true == _slot.IsEnableItem) || ( true == _slot.IsEnableSkill)))
				{
					m_ClickDownSlot = _slot;
					m_iClickDownQuickSlotIndex = nSlotIndex;
				}

				break;
			}
			
			nSlotIndex++;
		}
	}

	public void GuiInputMove( Ray inputRay)
	{
		if( false == isLock)
			return;

		if( null == m_goMoveIcon && null != m_ClickDownSlot && AsPStoreManager.Instance.UnableActionByPStore() == false) //$yde
		{
			if( m_fMaxItemMoveTime <= m_fItemMoveTime)
			{
				AsSoundManager.Instance.PlaySound( strIconUpSound, Vector3.zero, false);
				m_goMoveIcon = m_ClickDownSlot.getIcon;
				m_ClickDownSlot.Disable = true;
				m_fItemMoveTime = 0.0f;
			}

			m_fItemMoveTime += Time.deltaTime;
		}
	}

	public void GuiInputUp( Ray inputRay)
	{
		if( false == isLock)
			return;

		if( null != m_goMoveIcon && null != m_ClickDownSlot &&
			AsPStoreManager.Instance.UnableActionByPStore() == false) //$yde
		{
			if( false == GuiInputUpItem(inputRay))
			{
				m_goMoveIcon.transform.position = m_ClickDownSlot.transform.position;
				m_goMoveIcon.transform.localPosition = new Vector3( 0f, 0f, -0.5f);
			}

			m_ClickDownSlot.Disable = false;
			m_goMoveIcon = null;
			m_ClickDownSlot = null;
		}
	}

	public void InputUp( Ray inputRay)
	{
		if( ( null != m_ClickDownSlot) && ( AsPStoreManager.Instance.UnableActionByPStore() == false))	//$yde
		{
			if( null != m_goMoveIcon)
			{
				if( true == m_ClickDownSlot.IsEnableItem || true == m_ClickDownSlot.IsEnableSkill)
				{
					m_ClickDownSlot.Disable = false;
					if( m_iClickDownQuickSlotIndex >= 0)
					{
						AsCommonSender.SendQuickslotChange( (short)m_iClickDownQuickSlotIndex, 0, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_NOTHING);
						ItemMgr.HadItemManagement.QuickSlot.SetQuickSlot( (short)m_iClickDownQuickSlotIndex, 0, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_NOTHING);
						AsSoundManager.Instance.PlaySound( strIconDeleteSounc, Vector3.zero, false);
					}
				}
			}
			else
			{
				if( true == AsUtil.PtInCollider( m_ClickDownSlot.collider, inputRay))
				{
					if( m_ClickDownSlot.IsEnableItem)
					{
						TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_ClickDownSlot.getItemID);
					}
					else if( m_ClickDownSlot.IsEnableSkill)
					{
						if( null != toolTipInBook)
						{
							Tbl_Skill_Record skillRecord
								= AsTableManager.Instance.GetTbl_Skill_Record( m_ClickDownSlot.getSkillID);
							Tbl_SkillLevel_Record skillLevelRecord
								= AsTableManager.Instance.GetTbl_SkillLevel_Record( m_ClickDownSlot.getSkillLevel, m_ClickDownSlot.getSkillID);
							if( null != skillRecord && null != skillLevelRecord)
							{
								AsSkillTooltipInBook tip = GameObject.Instantiate( toolTipInBook) as AsSkillTooltipInBook;
								//AsDlgBase dlgBase = tip.gameObject.GetComponentInChildren<AsDlgBase>();
								tip.transform.position = m_tipPosition;//new Vector3( -500.0f/* - ( dlgBase.TotalWidth * 0.5f)*/, 0.0f,  -10.0f);

								tip.ID = m_ClickDownSlot.getSkillID;
								tip.Level = m_ClickDownSlot.getSkillLevel;
								tip.Init( skillRecord, skillLevelRecord);
								m_skillTooltip = tip;
							}
						}
					}
				}
			}
		}
	}

	public void InputMove( Ray inputRay)
	{
		if( null != m_goMoveIcon)
		{
			Vector3 vec3Temp = inputRay.origin;
			vec3Temp.z = -10;
			m_goMoveIcon.transform.position = vec3Temp;
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	public bool IsExpandQuickSlot()
	{
		int 	nIndex = 0;
		int 	nExpandLineIndex = 0;

		foreach( AsQuickSlot quickSlot in slotArray)
		{
			nExpandLineIndex = nIndex/2;
			
			if( nExpandLineIndex > 0 && quickSlot.IsOn() == true )
				return true;
			
			nIndex++;
		}
		
		return false;
	}
	
	public void ExpandQuickSlot(bool isExpandAll = false)
	{
		int 	nIndex = 0;
		int 	nExpandLineIndex = 0;
		bool	isQuickSlotOpen = false;
		
		
		AsQuickSlot	quickSlot = null;
		for(int i=slotArray.Length-1; i>=0; i-- )
		{
			quickSlot = slotArray[i];
			
			nExpandLineIndex = i/2;
			
			bool isHaveItemInQuickSlotLine = ItemMgr.HadItemManagement.QuickSlot.IsHadSlotPage( nExpandLineIndex );
			
			if( nExpandLineIndex > 0 )
			{
				if( isExpandAll == true || isHaveItemInQuickSlotLine == true || isQuickSlotOpen == true )
				{
					quickSlot.On( m_isLock );
					isQuickSlotOpen = true;
				}
				else
				{
					quickSlot.Off();
				}
			}
		}
		
		RefreshReduceQuickSlot();
		
		if (isQuickSlotOpen == true) 
		{
			SetViewPage (1);
		} 
		else 
		{
			AsMyProperty.Instance.AlertNoExpandQuickSlot();
		}
	}
	
	public void ReduceQuickSlot()
	{
		int 	nIndex = 0;
		int 	nExpandLineIndex = 0;

		foreach( AsQuickSlot quickSlot in slotArray)
		{
			nExpandLineIndex = nIndex/2;
			
			if( nExpandLineIndex > 0 )
				quickSlot.Off();
			
			nIndex++;
		}
		
		SetViewPage(0);
	}
	
	public void RefreshReduceQuickSlot()
	{
		m_isCancelCheckReduceQuickSlot = false;
		
		if( m_isCheckReduceQuickSlot == false )
			StartCoroutine( _CheckReduceQuickSlot());
		else
			m_fReduceTime = Time.time;
	}
	
	private IEnumerator _CheckReduceQuickSlot()
	{
		m_isCheckReduceQuickSlot = true;
		
		m_fReduceTime = Time.time;
		
		while( Time.time - m_fReduceTime < m_fReduceTimeDelay )
		{
			yield return null;
		}
		
		if( m_isLock == false && m_isCancelCheckReduceQuickSlot == false )
			ReduceQuickSlot();
		
		m_isCheckReduceQuickSlot = false;
	}
	
	public void CancelCheckReduceQuickSlot()
	{
		m_isCancelCheckReduceQuickSlot = true;
	}
	
	public void	Foward()
	{
		Vector3 nowPos = gameObject.transform.position;
		nowPos.z = m_fForwardZValue;
		gameObject.transform.position = nowPos;
	}
	
	public void	Backward()
	{
		StartCoroutine( _CheckBackward());
	}
	
	private IEnumerator _CheckBackward()
	{
		yield return new WaitForSeconds( 0.5f );

		Vector3 nowPos = gameObject.transform.position;
		nowPos.z = m_fBackwardZValue;
		gameObject.transform.position = nowPos;
	}
	
	public void DeleteQuickSlot( int nSkillTableIndex )
	{
		int	nSlotIndex = 0;
		foreach( AsSlot _slot in m_arraySlot )
		{
			if( _slot.getSkillID > 0 && _slot.getSkillID == nSkillTableIndex )
			{
				_slot.SetEmpty();
			}
		}
	}
}