using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public enum eSkillPosition
{
	Invalid = -1,
	
	SkillBook,
	SkillShop
};


public class AsBaseSkillItem : MonoBehaviour
{
	public AsSlot slot = null;
	public AsSkillTooltip toolTip = null;
	public AsSkillTooltipInBook toolTipInBook = null;
	public SpriteText desc = null;
	[HideInInspector]public int price = 0;
	public eSkillPosition position = eSkillPosition.Invalid;
	public Color defaultColor = Color.black;
	public Color deficientColor = Color.red;
	public Color goldColor = Color.yellow;
	
	private Tbl_Skill_Record skillRecord = null;
	private Tbl_SkillLevel_Record skillLevelRecord = null;
	AsSkillTooltipInBook m_tip = null;
	private SkillView skillInfo = null;
	
	public UIButton 	btnReset;
	public UIButton 	btnResetUp;
	public SpriteText 	textResetDesc;
	public SpriteText 	textResetCost;
	AsMessageBox 		skillResetMessageBox;

	void OnDestroy() 
	{
		if( null != m_tip )
			GameObject.Destroy(m_tip.gameObject);
	}
		
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( SkillView skill, bool isNew )
	{
		skillInfo = skill;
		
		if( eSkillPosition.SkillShop == position)
			slot.gameObject.collider.enabled = false;
		
		slot.SetSkillIcon( skill.nSkillTableIdx, skill.nSkillLevel);
		skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( skill.nSkillTableIdx);
		if( null == skillRecord)
		{
			Debug.LogError( "null == skillRecord");
			return;
		}
		
		skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( skill.nSkillLevel, skill.nSkillTableIdx);
		if( null == skillLevelRecord)
		{
			Debug.LogError( "null == skillLevelRecord");
			return;
		}

		UIListItem item = gameObject.GetComponent<UIListItem>();
		item.spriteText = desc;

		StringBuilder sb = new StringBuilder();
		
		switch( position)
		{
		case eSkillPosition.SkillShop:
			desc.transform.localPosition = new Vector3( 6.92458f, 1.524059f, -0.5f);
			desc.characterSize = 1;
			desc.anchor = SpriteText.Anchor_Pos.Upper_Right;
			desc.alignment = SpriteText.Alignment_Type.Right;

			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			int userLevel = userEntity.GetProperty<int>( eComponentProperty.LEVEL);
			string format = defaultColor.ToString() + "{0} Lv.{1}\n";
			if( userLevel < skillLevelRecord.Level_Limit)
				format += deficientColor.ToString();
			format += AsTableManager.Instance.GetTbl_String(134) + " {2}\n";
			
			if( ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( skill.getSkillRecord.Index))
				|| ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( skill.getSkillRecord.CoupleIndex)))
			{
				format += ( goldColor.ToString() + "0");
				
				sb.AppendFormat( format, AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index),
					skill.nSkillLevel, skillLevelRecord.Level_Limit);
			}
			else
			{
				if( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)price)
					format += ( deficientColor.ToString() + "{3}");
				else
					format += ( goldColor.ToString() + "{3}");
	
				sb.AppendFormat( format, AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index),
					skill.nSkillLevel, skillLevelRecord.Level_Limit, price.ToString( "#,#0", CultureInfo.InvariantCulture));
			}
			break;
			
		case eSkillPosition.SkillBook:
			
			//	skill reset cost button
			Vector3	descPos;
			if( skillRecord.SkillReset == eSkillReset.Enable && skillRecord.SkillResetCost >= 0 )
			{
				btnReset.gameObject.SetActive(true);
				btnReset.SetInputDelegate(ResetBtnDelegate);
				btnResetUp.SetInputDelegate(ResetBtnDelegate);
				textResetDesc.Text = AsTableManager.Instance.GetTbl_String(1221);
				textResetCost.Text = skillRecord.SkillResetCost.ToString();
			}
			else
			{
				btnReset.gameObject.SetActive(false);
			}
			
			descPos = new Vector3( -4.641596f , 0.0f, -0.5f);
			desc.transform.localPosition = descPos;
			desc.characterSize = 2;
			desc.anchor = SpriteText.Anchor_Pos.Middle_Left;
			
			desc.alignment = SpriteText.Alignment_Type.Center;

			sb.AppendFormat( "{0} Lv.{1}", AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index), skill.nSkillLevel);
			break;
		}

		desc.Text = sb.ToString();
		
		UISlotItem slotItem = slot.getIcon.GetComponent<UISlotItem>();
		if( null == slotItem)
		{
			Debug.LogError( "null == slotItem");
			return;
		}
		GameObject.DestroyImmediate( slotItem.coolTime.gameObject);
		GameObject.DestroyImmediate( slotItem.itemCountText.gameObject);
		slotItem.iconImg.renderer.enabled = false;
		item.layers[0] = slotItem.iconImg;
		
		if( isNew )
		{			
			GameObject obj = ResourceLoad.CreateUI( "UI/AsGUI/GUI_NewImg", slotItem.iconImg.transform, new Vector3(0f, 1f, -0.1f ) );
			if( null == obj )
				return;
			SimpleSprite _sprite = obj.GetComponentInChildren<SimpleSprite>();
			if( null!= _sprite )
				item.layers[2] = _sprite;
		}
	}
	
	public void PromptTooltip()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( eSkillPosition.SkillShop == position)
		{
			AsSkillTooltip tip = GameObject.Instantiate( toolTip) as AsSkillTooltip;

			float screenWidth = desc.RenderCamera.orthographicSize * desc.RenderCamera.aspect * 2.0f;
			tip.transform.position = new Vector3( ( ( screenWidth * 0.5f) - 500.0f) - ( ( screenWidth - 16.5f) * 0.5f), 0.0f, -10.0f);
			
			tip.ID = slot.getSkillID;
			tip.Level = slot.getSkillLevel;

			if( ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( skillInfo.getSkillRecord.Index))
				|| ( true == AsUserInfo.Instance.resettedSkills.ContainsKey( skillInfo.getSkillRecord.CoupleIndex)))
				price = 0;
			
			tip.Init( skillRecord, skillLevelRecord, price);
		}
		else
		{
			AsSkillTooltipInBook tip = GameObject.Instantiate( toolTipInBook) as AsSkillTooltipInBook;
			AsDlgBase dlgBase = tip.gameObject.GetComponentInChildren<AsDlgBase>();
			tip.transform.position = new Vector3( -500.0f - ( dlgBase.TotalWidth * 0.5f), 0.0f,  -10.0f);
			
			tip.ID = slot.getSkillID;
			tip.Level = slot.getSkillLevel;
			
			tip.Init( skillRecord, skillLevelRecord);
			m_tip = tip;
		}
	}
	
	private void ResetBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( null != skillRecord )
			{
				if( true == TargetDecider.CheckCurrentMapIsIndun())
				{
					AsMyProperty.Instance.AlertNotInIndun();
					return;
				}

				if( true == TargetDecider.CheckCurrentMapIsArena())
				{
					AsMyProperty.Instance.AlertNotInPvp();
					return;
				}

				if( AsNotify.Instance.IsOpenMessageBox() == false )
				{
					long   lSkillResetMiracle = skillRecord.SkillResetCost;
					string skillName = AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index);
					string title = AsTableManager.Instance.GetTbl_String(126);
				
					string msg = string.Format(AsTableManager.Instance.GetTbl_String(147) , skillName );
			
					skillResetMessageBox = AsNotify.Instance.SkillResetMessageBox( lSkillResetMiracle , title, msg, this, "SkillResetOK", "SkillResetCancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL);
				}
			}
			else
			{
				Debug.LogError("skillRecord is null");
			}
		}
	}	
	
	private void SkillResetOK()
	{
		skillResetMessageBox = null;
		
		long   lSkillResetMiracle = skillRecord.SkillResetCost;
		
		if( AsUserInfo.Instance.nMiracle < lSkillResetMiracle )
		{
			//	cash shop messagebox
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(264), AsHudDlgMgr.Instance , "NotEnoughMiracleProcessInGame", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
		}
		else
		{
			//	send request skill reset
			body_CS_SKILL_RESET packetData = new body_CS_SKILL_RESET( skillRecord.Index );
			byte[] data = packetData.ClassToPacketBytes();
			AsCommonSender.Send(data);
		}
	}
	
	private void SkillResetCancel()
	{
		skillResetMessageBox = null;
	}
}
