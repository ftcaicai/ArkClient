//#define USE_OLD_COSTUME
using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public class PlayerStatusDlg : MonoBehaviour
{
	public UIInvenSlot[] slots;
	public SimpleSprite focusImg;
	public UIInvenSlot moveItemSlot;
	public GameObject slotItemParent;
	public SpriteText subTitle;
	public SpriteText textTitle;
	public UIButton btnClose;
	public UIButton btnChangeSlot;
	public float dragPageMoveDistance = 3.0f;
	public SpriteText[] texts;
	public SpriteText[] textTitles;
	private int m_iCurSlotPage = 0;
	private UIInvenSlot m_ClickDownItemSlot;
	public SimpleSprite normalEquipImg;
	public SimpleSprite costumeEquipImg;

	private StringBuilder m_strbuilderTemp = new StringBuilder();
	private StringBuilder m_strbuilderTemp_1 = new StringBuilder();
	
	public bool isCostumeEquipSlot		
	{
		get
		{
			return m_iCurSlotPage == 1;
		}
	}
	#region -DesignationDlg
	private GameObject designationDlg = null;
	public GameObject DesignationDlg
	{
		get	{ return designationDlg; }
	}
	#endregion

	// item move active time
	public float m_fMaxItemMoveTime = 0.5f;
	private float m_fItemMoveTime = 0.0f;
	private bool m_bNeedClose = false;

	#region -cooltime
#if USE_OLD_COSTUME
	public UIStateToggleBtn btnCostume;
#else
	public UIButton btnCostume;
#endif	
	public AsIconCooltime coolTime;
	public float useCoolTime = 3f;
	private float m_fMaxCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f;


	private void SetCoolTime( float fMaxValue, float fRemainCooltime)
	{
		if( null == coolTime)
		{
			Debug.LogError( "ResetDataButton::SetCoolTime() [ null == coolTime");
			return;
		}

		if( true == coolTime.gameObject.active)
			return;
		else
			coolTime.gameObject.active = true;

		btnCostume.controlIsEnabled = false;
		m_fRemainCoolTime = fRemainCooltime;
		m_fMaxCoolTime = fMaxValue;

		ResetCoolTime();
	}

	private void ResetCoolTime()
	{
		if( 0.0f > m_fRemainCoolTime)
		{
			m_fRemainCoolTime = 0.0f;
			coolTime.gameObject.active = false;
			btnCostume.controlIsEnabled = true;
		}

		coolTime.Value = ( m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;
	}

	void CoolUpdate()
	{
		if( null == coolTime)
			return;

		if( false == coolTime.gameObject.active)
			return;

		m_fRemainCoolTime -= Time.deltaTime;
		ResetCoolTime();
	}

	public void InitCoolTime()
	{
		m_fMaxCoolTime = 0.0f;
		m_fRemainCoolTime = 0.0f;
		coolTime.gameObject.active = false;
	}

	// inven reset button
	public void StartCoolTime()
	{
		if( null != coolTime)
		{
			coolTime.gameObject.active = false;			
		}
		btnCostume.SetInputDelegate( ChangeViewPartsBtnDelegate);
		ResetCostumeOnOffBtn();
	}

	private void ResetCostumeOnOffBtn()
	{
#if USE_OLD_COSTUME
		if( null == btnCostume || false == btnCostume.gameObject.active)
			return;

		if( AsUserInfo.Instance.isCostumeOnOff)
			btnCostume.SetToggleState( 0);
		else
			btnCostume.SetToggleState( 1);
#endif
	}

	private void RetainCostumeOnOffBtn()
	{
#if USE_OLD_COSTUME
		if( null == btnCostume || false == btnCostume.gameObject.active)
			return;

		if( AsUserInfo.Instance.isCostumeOnOff)
			btnCostume.SetToggleState( 1);
		else
			btnCostume.SetToggleState( 0);		
#endif
	}

	private void ChangeViewPartsBtnDelegate( ref POINTER_INFO ptr)
	{
#if USE_OLD_COSTUME
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				RetainCostumeOnOffBtn();
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
					null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}

			ResetCostumeOnOffBtn();
			if( true == coolTime.gameObject.active)
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetCoolTime( useCoolTime, useCoolTime);
			AsCommonSender.SendConstumeOnOff( !AsUserInfo.Instance.isCostumeOnOff);

			if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.COSTUME_ON_OFF) != null)
				AsCommonSender.SendClearOpneUI( OpenUIType.COSTUME_ON_OFF);
		}		
		
#else
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{				
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
					null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}
			
			if( true == coolTime.gameObject.active)
				return;
			
			if( false == AsHudDlgMgr.Instance.IsPlayerPartsOptionDlg )
			{
				SetCoolTime( useCoolTime, useCoolTime);
				AsHudDlgMgr.Instance.OpenPlayerPartsOptionDlg();
			}
			
			if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.COSTUME_ON_OFF) != null)
				AsCommonSender.SendClearOpneUI( OpenUIType.COSTUME_ON_OFF);
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}		
#endif
	}
	#endregion

	public void ReceiveConstumeOnOff()
	{
		if( true== m_isOtherUser)
			return;

		ResetCostumeOnOffBtn();
	}

	public bool isOtherUser
	{
		get
		{
			return m_isOtherUser;
		}
	}

	private bool m_isOtherUser = false;
	private body1_SC_OTHER_INFO m_OtherUserData = null;

	public bool IsRect( Ray inputRay)
	{
		if( null == collider)
			return false;

		return collider.bounds.IntersectRay( inputRay);
	}

	//-------------------------
	public void Open( float fz)
	{
		InitCoolTime();
		m_isOtherUser = false;
		ResetPageText();
		ResetSlotItmes();
		DetachFocusImg();
		#region -Designation_Text
		SetDesignationText();
		#endregion
		btnCostume.gameObject.SetActiveRecursively( true);
		ResetCostumeOnOffBtn();

		if( null != textTitle)
			textTitle.Text = AsTableManager.Instance.GetTbl_String( 1330);

		SetBtnChange( m_iCurSlotPage);

		if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.CHARACTER_INFO) != null)
			AsCommonSender.SendClearOpneUI( OpenUIType.CHARACTER_INFO);

		Vector3 temp = transform.position;
		temp.z = fz;
		transform.position = temp;

		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.OPEN_STATUS));
	}

	//dopamin
	public void OpenOtherUser( body1_SC_OTHER_INFO _data, float fz)
	{
		InitCoolTime();
		btnCostume.gameObject.SetActiveRecursively( false);
		m_isOtherUser = true;
		m_OtherUserData = _data;
		foreach( SpriteText _text in texts)
		{
			_text.Text = string.Empty;
		}

		foreach ( SpriteText _text in textTitles)
		{
			_text.Text = string.Empty;
		}

		DetachFocusImg();
		if( null == slotItemParent)
		{
			Debug.LogError( "PlayerStatusDlg::OpenOtherUser() [ null == slotItemParent ]");
			return;
		}

		if( null == slots)
		{
			Debug.LogError( "PlayerStatusDlg::OpenOtherUser() [ null == slots ]");
			return;
		}

		int iStartIndex = m_iCurSlotPage * 10;
		int iIndex = iStartIndex;

		foreach( UIInvenSlot slot in slots)
		{
			slot.SetSlotIndex( iIndex);
			slot.DeleteSlotItem();

			RealItem realItem = GetOtherRealItem( iIndex);
			if( null == realItem)
			{
				++ iIndex;
				continue;
			}

			slot.CreateSlotItem( realItem, slotItemParent.transform);
			++ iIndex;
		}

		SetOtherUserDesignationText( _data.nSubTitleTableIdx);
		SetOtherUserPageText_1( _data);

		if( null != textTitle)
		{
			//textTitle.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1331), _data.szCharName);
			m_strbuilderTemp.Length = 0;
			m_strbuilderTemp.Append( "Lv.");
			m_strbuilderTemp.Append( _data.nLevel);
			m_strbuilderTemp.Append( " ");
			m_strbuilderTemp.Append( _data.szCharName);

			textTitle.Text = m_strbuilderTemp.ToString();
		}
		SetBtnChange( m_iCurSlotPage);

		Vector3 temp = transform.position;
		temp.z = fz;
		transform.position = temp;
	}

	private RealItem GetOtherRealItem( int iIndex)
	{
		if( null == m_OtherUserData || null == m_OtherUserData.body)
			return null;

		for( int i = 0; i < m_OtherUserData.body.Length; ++i)
		{
			if( iIndex == m_OtherUserData.body[i].nSlot && 0 != m_OtherUserData.body[i].sItem.nItemTableIdx)
				return new RealItem( m_OtherUserData.body[i].sItem,m_OtherUserData.body[i].nSlot);
		}

		return null;
	}

	public void Close()
	{
		ClearSlot();
		DetachFocusImg();
		#region -DesignationDlg
		CloseDesignationDlg();
		#endregion
		moveItemSlot.DeleteSlotItem();
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_MYINFO));
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_STATUS));
	}

	public void ResetPageText()
	{
		if( true == m_isOtherUser)
			return;

		foreach( SpriteText _text in texts)
		{
			_text.Text = string.Empty;
		}

		foreach ( SpriteText _text in textTitles)
		{
			_text.Text = string.Empty;
		}

		if( 6 > texts.Length)
		{
			Debug.LogError( "PlayerStatusDlg::ResetPageText() [ 6 > texts.Length ] ");
			return;
		}

		CharacterLoadData savedCharStat = AsUserInfo.Instance.SavedCharStat;
		SetPageText_1( savedCharStat);
	}

	private string GetFinalStatusString( float fFinalStatus, float fDefaultStatus)
	{
		float fPrint = fFinalStatus - fDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0.0f == fPrint)
		{
			m_strbuilderTemp.AppendFormat( "{0:0}", fFinalStatus);
		}
		else if( 0.0f > fPrint)
		{
			m_strbuilderTemp.AppendFormat( "{0}{1:0}( {2:0})", Color.red, fFinalStatus, fPrint);
		}
		else
		{
			m_strbuilderTemp.AppendFormat( "{0}{1:0}( +{2:0})", Color.green, fFinalStatus, fPrint);
		}

		return m_strbuilderTemp.ToString();
	}

private void PrintText( string strString, SpriteText uiText, SpriteText uiTitleText, float fFinalStatus, float fDefaultStatus/*, float fMin = float.MinValue, float fMax = float.MaxValue*/)
	{
		/*if( fMin > fFinalStatus)
		{
			fFinalStatus = fMin;
		}

		if( fMin > fDefaultStatus)
		{
			fDefaultStatus = fMin;
		}

		if( fMax < fFinalStatus)
		{
			fFinalStatus = fMax;
		}
		if( fMax < fDefaultStatus)
		{
			fDefaultStatus = fMax;
		}*/

		float fPrint = fFinalStatus - fDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0.0f == fPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1:0}", strString, fFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0:0}", fFinalStatus);
		}
		else if( 0.0f > fPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0}( {3:0})", strString, Color.red, fFinalStatus, fPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0}( {2:0})", Color.red, fFinalStatus, fPrint);
		}
		else
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0}( +{3:0})", strString, Color.green, fFinalStatus, fPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0}( +{2:0})", Color.green, fFinalStatus, fPrint);
		}

		uiTitleText.Text = strString;
		uiText.Text = m_strbuilderTemp.ToString();
	}

	private void PrintIntText( string strString, SpriteText uiText, SpriteText uiTitleText, float fFinalStatus, float fDefaultStatus)
	{
		float iFinalStatus = fFinalStatus;
		float iDefaultStatus = fDefaultStatus;

		float iPrint = iFinalStatus - iDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( -0.1f >= iPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}( {3:0.0})", strString, Color.red, fFinalStatus, iPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.0}( {2:0.0})", Color.red, fFinalStatus, iPrint);
		}
		else if( 0.1f <= iPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}( +{3:0.0})", strString, Color.green, fFinalStatus, iPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.0}( +{2:0.0})", Color.green, fFinalStatus, iPrint);
		}
		else
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1:0.0}", strString, fFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0:0.0}", fFinalStatus);
		}
		uiTitleText.Text = strString;
		uiText.Text = m_strbuilderTemp.ToString();
	}

	private void PercentPrintText( string strString, SpriteText uiText, float fFinalStatus, float fDefaultStatus)
	{
		int iFinalStatus = ( int)fFinalStatus / 10;
		int iDefaultStatus = ( int)fDefaultStatus / 10;

		int iPrint = iFinalStatus - iDefaultStatus;
		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0 == iPrint)
			m_strbuilderTemp.AppendFormat( "{0} {1:0}%", strString, fFinalStatus);
		else if( 0 > iPrint)
			m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}( {3:0.0}%)", strString, Color.red, fFinalStatus, iPrint);
		else
			m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}( +{3:0.0}%)", strString, Color.green, fFinalStatus, iPrint);

		uiText.Text = m_strbuilderTemp.ToString();
	}

	private void PercentPrintText_type1( string strString, SpriteText uiText, float fFinalStatus, float fDefaultStatus, float fMin = float.MinValue, float fMax = float.MaxValue)
	{
		float iFinalStatus = fFinalStatus * 0.1f;
		float iDefaultStatus = fDefaultStatus * 0.1f;

		if( fMin > iFinalStatus)
			iFinalStatus = fMin;

		if( fMax < iFinalStatus)
			iFinalStatus = fMax;

		if( fMin > iDefaultStatus)
			iDefaultStatus = fMin;

		if( fMax < iDefaultStatus)
			iDefaultStatus = fMax;

		float iPrint = iFinalStatus - iDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0 == iPrint)
			m_strbuilderTemp.AppendFormat( "{0} {1:0.0}%", strString, iFinalStatus);
		else if( 0 > iPrint)
			m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}%( {3:0.0}%)", strString, Color.red, iFinalStatus, iPrint);
		else
			m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}%( +{3:0.0}%)", strString, Color.green, iFinalStatus, iPrint);

		uiText.Text = m_strbuilderTemp.ToString();
	}

	private void PercentPrintText_type2( string strString, SpriteText uiText, SpriteText uiTitleText, float fFinalStatus, float fDefaultStatus, float fMin = float.MinValue, float fMax = float.MaxValue)
	{
		float iFinalStatus = fFinalStatus * 0.1f;
		float iDefaultStatus = fDefaultStatus * 0.1f;

		if( fMin > iFinalStatus)
			iFinalStatus = fMin;

		if( fMax < iFinalStatus)
			iFinalStatus = fMax;

		if( fMin > iDefaultStatus)
			iDefaultStatus = fMin;

		if( fMax < iDefaultStatus)
			iDefaultStatus = fMax;

		float iPrint = iFinalStatus - iDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0 == iPrint)
		{
			// m_strbuilderTemp.AppendFormat( "{0} {1:0.0}%", strString, iFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0:0.0}%", iFinalStatus);
		}
		else if( 0 > iPrint)
		{
			// m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}%", strString, Color.red, iFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.0}%", Color.red, iFinalStatus);
		}
		else
		{
			// m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.0}%", strString, Color.green, iFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.0}%", Color.green, iFinalStatus);
		}

		uiTitleText.Text = strString;
		uiText.Text = m_strbuilderTemp.ToString();
	}

	private void PercentPrintText_move( string strString, SpriteText uiText, SpriteText uiTitleText, float fFinalStatus, float fDefaultStatus
		, float fMin = float.MinValue, float fMax = float.MaxValue)
	{
		float iFinalStatus = fFinalStatus * 0.01f;
		float iDefaultStatus = fDefaultStatus * 0.01f;

		fMin = fDefaultStatus * ( fMin * 0.001f) * 0.01f;
		fMax = fDefaultStatus * ( fMax * 0.001f) * 0.01f;

		if( fMin > iFinalStatus)
			iFinalStatus = fMin;

		if( fMax < iFinalStatus)
			iFinalStatus = fMax;

		if( fMin > iDefaultStatus)
			iDefaultStatus = fMin;

		if( fMax < iDefaultStatus)
			iDefaultStatus = fMax;

		float iPrint = iFinalStatus - iDefaultStatus;

		m_strbuilderTemp.Remove( 0, m_strbuilderTemp.Length);
		if( 0 == iPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1:0.00}m/s", strString, iFinalStatus);
			m_strbuilderTemp.AppendFormat( "{0:0.00}", iFinalStatus);
		}
		else if( 0 > iPrint)
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.00}m/s", strString, Color.red, iFinalStatus, iPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.00}", Color.red, iFinalStatus, iPrint);
		}
		else
		{
			//m_strbuilderTemp.AppendFormat( "{0} {1}{2:0.00}m/s", strString, Color.green, iFinalStatus, iPrint);
			m_strbuilderTemp.AppendFormat( "{0}{1:0.00}", Color.green, iFinalStatus, iPrint);
		}

		uiTitleText.Text = strString;
		uiText.Text = m_strbuilderTemp.ToString();
	}

	private float GetRealDataPercent( float fData, float fDef, float fMin, float fMax)
	{
		float fRealData = fData;
		fMin = fDef * ( fMin*0.001f);
		fMax = fDef * ( fMax*0.001f);

		if( fRealData < fMin)
			fRealData = fMin;

		if( fRealData > fMax)
			fRealData = fMax;

		return fRealData;
	}
	
	private float GetRealDataPercent_1( float fData, float fDef, float fMin, float fMax)
	{
		float fRealData = fData;
		//fMin = fDef * ( fMin*0.001f);
		//fMax = fDef * ( fMax*0.001f);

		//if( fRealData < fMin)
		//	fRealData = fMin;

		//if( fRealData > fMax)
		//	fRealData = fMax;

		return fRealData;
	}
	
	

	private void SetOtherUserPageText_1( body1_SC_OTHER_INFO _data)
	{
		if( _data.nClass == 0f)
		{
			//texts[0].Text = AsTableManager.Instance.GetTbl_String( 4070);
			textTitles[0].Text = AsTableManager.Instance.GetTbl_String( 4070);
			return;
		}

		Tbl_UserLevel_Record levelRecord = AsTableManager.Instance.GetTbl_Level_Record( ( eCLASS)_data.nClass, _data.nLevel);
		Tbl_Class_Record classRecord = AsTableManager.Instance.GetTbl_Class_Record( ( eCLASS)_data.nClass);

		if( null == levelRecord)
			return;

		if( null == classRecord)
			return;

		// max hp
		int iIndex = 0;
		PrintText( AsTableManager.Instance.GetTbl_String( 1066), texts[iIndex], textTitles[iIndex], _data.fHPMax, levelRecord.HPMax);
		++iIndex;

		// max mp
		PrintText( AsTableManager.Instance.GetTbl_String( 1067), texts[iIndex], textTitles[iIndex], _data.fMPMax, levelRecord.MPMax);
		++iIndex;

		//HP Regen
		float fHpRegen = levelRecord.HPMax * classRecord.HpRecovery * 0.001f * classRecord.HpRecoveryBattle * 0.001f * 4f;
		PrintIntText( AsTableManager.Instance.GetTbl_String( 1079), texts[iIndex], textTitles[iIndex],_data.nHPRegen, fHpRegen);
		++iIndex;

		//MP Regen
		float fMpRegen = levelRecord.MPMax * classRecord.MpRecovery * 0.001f * classRecord.MpRecoveryBattle * 0.001f * 4f;
		PrintIntText( AsTableManager.Instance.GetTbl_String( 1080), texts[iIndex], textTitles[iIndex],_data.nMPRegen, fMpRegen);
		++iIndex;

		//pamin
		eATTACK_TYPE attackType = classRecord.attackType;

		Tbl_BuffMinMaxTable_Record record = null;
		float fPhsicalMin = float.MinValue;
		float fPhsicalMax = float.MaxValue;

		if( eATTACK_TYPE.Physics == attackType)
		{
			record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.PhysicalAttackMinMax);
			if( null != record)
			{
				fPhsicalMin = record.min;
				fPhsicalMax = record.max;
			}

			m_strbuilderTemp_1.Length = 0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1092));
			//m_strbuilderTemp_1.Append( " ");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( _data.nPhysicDmg_Min, levelRecord.PhysicalAttack_Min, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( levelRecord.PhysicalAttack_Min, levelRecord.PhysicalAttack_Min, fPhsicalMin, fPhsicalMax)));

			m_strbuilderTemp_1.Append( Color.white);
			m_strbuilderTemp_1.Append( "~");

			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( _data.nPhysicDmg_Max, levelRecord.PhysicalAttack_Max, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( levelRecord.PhysicalAttack_Max, levelRecord.PhysicalAttack_Max, fPhsicalMin, fPhsicalMax)));

			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1092);
			++iIndex;

			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1093));
			m_strbuilderTemp_1.Append( "0");
			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1093);

			++iIndex;
		}
		else
		{
			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1092));
			m_strbuilderTemp_1.Append( "0");
			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1092);
			++iIndex;

			record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MagicalAttackMinMax);
			if( null != record)
			{
				fPhsicalMin = record.min;
				fPhsicalMax = record.max;
			}

			m_strbuilderTemp_1.Length = 0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1093));
			//m_strbuilderTemp_1.Append( " ");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( _data.nMagicDmg_Min, levelRecord.MagicalAttack_Min, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( levelRecord.MagicalAttack_Min, levelRecord.MagicalAttack_Min, fPhsicalMin, fPhsicalMax)));

			m_strbuilderTemp_1.Append( Color.white);
			m_strbuilderTemp_1.Append( "~");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( _data.nMagicDmg_Max, levelRecord.MagicalAttack_Max, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( levelRecord.MagicalAttack_Max, levelRecord.MagicalAttack_Max, fPhsicalMin, fPhsicalMax)));

			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1093);
			++iIndex;
		}

		//pd
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.PhysicalDefenseMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min;
			fPhsicalMax = record.max;
		}
		PrintText( AsTableManager.Instance.GetTbl_String( 1072), texts[iIndex], textTitles[iIndex],
			GetRealDataPercent_1( _data.nPhysic_Def, levelRecord.PhysicalDefense, fPhsicalMin, fPhsicalMax),
			GetRealDataPercent_1( levelRecord.PhysicalDefense, levelRecord.PhysicalDefense, fPhsicalMin, fPhsicalMax));
		++iIndex;


		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1218), texts[iIndex], textTitles[iIndex],
			_data.fPhysic_Dmg_Dec, _data.fDef_Physic_Dmg_Dec);
		++iIndex;
		//md
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MagicalResistMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min;
			fPhsicalMax = record.max;
		}
		PrintText( AsTableManager.Instance.GetTbl_String( 1073), texts[iIndex], textTitles[iIndex],
			GetRealDataPercent_1( _data.nMagic_Def, levelRecord.MagicalResist, fPhsicalMin, fPhsicalMax),
			GetRealDataPercent_1( levelRecord.MagicalResist, levelRecord.MagicalResist, fPhsicalMin, fPhsicalMax));
		++iIndex;

		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1219), texts[iIndex], textTitles[iIndex],
			_data.fMagic_Dmg_Dec, _data.fDef_Magic_Dmg_Dec);
		++iIndex;

		//Critical
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1074), texts[iIndex], textTitles[iIndex],_data.nCriticalProb, classRecord.CriticalRatio);//
		++iIndex;

		//Accuracy
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.AccuracyMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min* 0.1f;
			fPhsicalMax = record.max* 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1076), texts[iIndex], textTitles[iIndex],
			_data.nAccuracyProb, classRecord.AccuracyRatio, fPhsicalMin, fPhsicalMax);
		++iIndex;

		//AttackSpeed
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.AttackSpeedMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min* 0.1f;
			fPhsicalMax = record.max* 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1075), texts[iIndex], textTitles[iIndex],
			_data.nAtkSpeed, classRecord.AttackSpeedRatio, fPhsicalMin, fPhsicalMax);
		++iIndex;

		//Dodg
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.DodgeMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min * 0.1f;
			fPhsicalMax = record.max * 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1077), texts[iIndex], textTitles[iIndex],
			_data.nDodgeProb, classRecord.DodgeRatio, fPhsicalMin, fPhsicalMax);//
		++iIndex;

		//move speed
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MoveSpeedMinMax);
		if( null != record)
		{
			fPhsicalMin = ( float)record.min;
			fPhsicalMax = ( float)record.max;
		}

		PercentPrintText_move( AsTableManager.Instance.GetTbl_String( 1078), texts[iIndex], textTitles[iIndex],
			_data.nMoveSpeed, classRecord.MoveSpeed, fPhsicalMin, fPhsicalMax);
		++iIndex;

		//exp
		bool isMaxLevel = false;
		Tbl_GlobalWeight_Record levelrecord = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 69);
		if( null != levelrecord)
		{
			int iCurLevel = _data.nLevel;
			if( iCurLevel >= ( int)levelrecord.Value)
				isMaxLevel = true;
		}

		if( true == isMaxLevel)
		{
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1721);
		}
		else
		{
			m_strbuilderTemp_1.Remove( 0, m_strbuilderTemp_1.Length);

			int level = _data.nLevel;
			eCLASS cls = ( eCLASS)_data.nClass;
			int curExp = _data.nTotExp;
			Tbl_UserLevel_Record curRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level);

			if( AsGameDefine.MAX_LEVEL > level)
			{
				Tbl_UserLevel_Record nextRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level + 1);
				if( null != nextRecord)
				{
					//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1081));
					//m_strbuilderTemp_1.Append( " ");
					m_strbuilderTemp_1.Append( curExp.ToString( "#,#0", CultureInfo.InvariantCulture));
					m_strbuilderTemp_1.Append( "/");
					m_strbuilderTemp_1.Append( nextRecord.TotalEXP.ToString( "#,#0", CultureInfo.InvariantCulture));
					texts[iIndex].Text = m_strbuilderTemp_1.ToString();
					textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1081);
				}
			}
			else
			{
				//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1081));
				//m_strbuilderTemp_1.Append( " ");
				m_strbuilderTemp_1.Append( curExp.ToString( "#,#0", CultureInfo.InvariantCulture));
				m_strbuilderTemp_1.Append( "/");
				m_strbuilderTemp_1.Append( curRecord.TotalEXP.ToString( "#,#0", CultureInfo.InvariantCulture));
				texts[iIndex].Text = m_strbuilderTemp_1.ToString();
				textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1081);
			}
		}
	}

	private void SetPageText_1( CharacterLoadData savedCharStat)
	{
		// max hp
		int iIndex = 0;
		PrintText( AsTableManager.Instance.GetTbl_String( 1066), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.fHPMax, savedCharStat.sDefaultStatus.fHPMax);
		iIndex++;

		// max mp
		PrintText( AsTableManager.Instance.GetTbl_String( 1067), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.fMPMax, savedCharStat.sDefaultStatus.fMPMax);
		iIndex++;

		//HP Regen
		PrintIntText( AsTableManager.Instance.GetTbl_String( 1079), texts[iIndex], textTitles[iIndex], savedCharStat.sFinalStatus.nHPRegen, savedCharStat.sDefaultStatus.nHPRegen);
		iIndex++;

		//MP Regen
		PrintIntText( AsTableManager.Instance.GetTbl_String( 1080), texts[iIndex], textTitles[iIndex], savedCharStat.sFinalStatus.nMPRegen, savedCharStat.sDefaultStatus.nMPRegen);
		iIndex++;

		//pamin
		eATTACK_TYPE attackType = AsEntityManager.Instance.UserEntity.GetProperty<eATTACK_TYPE>( eComponentProperty.ATTACK_TYPE);
		//m_strbuilderTemp_1.Remove( 0, m_strbuilderTemp_1.Length);

		Tbl_BuffMinMaxTable_Record record = null;
		float fPhsicalMin = float.MinValue;
		float fPhsicalMax = float.MaxValue;

		if( eATTACK_TYPE.Physics == attackType)
		{
			record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.PhysicalAttackMinMax);
			if( null != record)
			{
				fPhsicalMin = record.min;
				fPhsicalMax = record.max;
			}

			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1092));
			//m_strbuilderTemp_1.Append( " ");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( savedCharStat.sFinalStatus.nPhysicDmg_Min, savedCharStat.sDefaultStatus.nPhysicDmg_Min, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( savedCharStat.sDefaultStatus.nPhysicDmg_Min, savedCharStat.sDefaultStatus.nPhysicDmg_Min, fPhsicalMin, fPhsicalMax)));

			m_strbuilderTemp_1.Append( Color.white);
			m_strbuilderTemp_1.Append( "~");

			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( savedCharStat.sFinalStatus.nPhysicDmg_Max, savedCharStat.sDefaultStatus.nPhysicDmg_Max, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( savedCharStat.sDefaultStatus.nPhysicDmg_Max, savedCharStat.sDefaultStatus.nPhysicDmg_Max, fPhsicalMin, fPhsicalMax)));

			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1092);
			iIndex++;

			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1093));
			m_strbuilderTemp_1.Append( "0");
			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1093);
			iIndex++;
		}
		else
		{
			record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MagicalAttackMinMax);
			if( null != record)
			{
				fPhsicalMin = record.min;
				fPhsicalMax = record.max;
			}

			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1092));
			m_strbuilderTemp_1.Append( "0");
			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1092);
			iIndex++;

			m_strbuilderTemp_1.Length =0;
			//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1093));
			//m_strbuilderTemp_1.Append( " ");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( savedCharStat.sFinalStatus.nMagicDmg_Min, savedCharStat.sDefaultStatus.nMagicDmg_Min, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( savedCharStat.sDefaultStatus.nMagicDmg_Min, savedCharStat.sDefaultStatus.nMagicDmg_Min, fPhsicalMin, fPhsicalMax)));

			m_strbuilderTemp_1.Append( Color.white);
			m_strbuilderTemp_1.Append( "~");
			m_strbuilderTemp_1.Append( GetFinalStatusString( 
				GetRealDataPercent( savedCharStat.sFinalStatus.nMagicDmg_Max, savedCharStat.sDefaultStatus.nMagicDmg_Max, fPhsicalMin, fPhsicalMax),
				GetRealDataPercent( savedCharStat.sDefaultStatus.nMagicDmg_Max, savedCharStat.sDefaultStatus.nMagicDmg_Max, fPhsicalMin, fPhsicalMax)));

			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1093);
			texts[iIndex].Text = m_strbuilderTemp_1.ToString();
			iIndex++;
		}

		//pd
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.PhysicalDefenseMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min;
			fPhsicalMax = record.max;
		}
		PrintText( AsTableManager.Instance.GetTbl_String( 1072), texts[iIndex], textTitles[iIndex],
			GetRealDataPercent_1( savedCharStat.sFinalStatus.nPhysic_Def, savedCharStat.sDefaultStatus.nPhysic_Def, fPhsicalMin, fPhsicalMax),
			GetRealDataPercent_1( savedCharStat.sDefaultStatus.nPhysic_Def, savedCharStat.sDefaultStatus.nPhysic_Def, fPhsicalMin, fPhsicalMax));
		iIndex++;

		//pd%
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1218), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.fPhysic_Dmg_Dec, savedCharStat.sDefaultStatus.fPhysic_Dmg_Dec);
		iIndex++;

		//md
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MagicalResistMinMax);
		if( null != record)
		{
			fPhsicalMin = record.min;
			fPhsicalMax = record.max;
		}
		PrintText( AsTableManager.Instance.GetTbl_String( 1073), texts[iIndex], textTitles[iIndex],
			GetRealDataPercent_1( savedCharStat.sFinalStatus.nMagic_Def, savedCharStat.sDefaultStatus.nMagic_Def, fPhsicalMin, fPhsicalMax),
			GetRealDataPercent_1( savedCharStat.sDefaultStatus.nMagic_Def, savedCharStat.sDefaultStatus.nMagic_Def, fPhsicalMin, fPhsicalMax));
		iIndex++;


		//md%
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1219), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.fMagic_Dmg_Dec, savedCharStat.sDefaultStatus.fMagic_Dmg_Dec);
		iIndex++;

		//Critical
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1074), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.nCriticalProb, savedCharStat.sDefaultStatus.nCriticalProb);//
		iIndex++;

		//Accuracy
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.AccuracyMinMax);
		if( null != record)
		{
			fPhsicalMin = ( float)record.min* 0.1f;
			fPhsicalMax = ( float)record.max* 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1076), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.nAccuracyProb, savedCharStat.sDefaultStatus.nAccuracyProb, fPhsicalMin, fPhsicalMax);
		iIndex++;

		//AttackSpeed
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.AttackSpeedMinMax);
		if( null != record)
		{
			fPhsicalMin = ( float)record.min* 0.1f;
			fPhsicalMax = ( float)record.max* 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1075), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.nAtkSpeed, savedCharStat.sDefaultStatus.nAtkSpeed, fPhsicalMin, fPhsicalMax);
		iIndex++;

		//Dodg
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.DodgeMinMax);
		if( null != record)
		{
			fPhsicalMin = ( float)record.min * 0.1f;
			fPhsicalMax = ( float)record.max * 0.1f;
		}
		PercentPrintText_type2( AsTableManager.Instance.GetTbl_String( 1077), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.nDodgeProb, savedCharStat.sDefaultStatus.nDodgeProb, fPhsicalMin, fPhsicalMax);//
		iIndex++;

		//move speed
		record = AsTableManager.Instance.GetBuffMinMaxTable().GetRecord( Tbl_BuffMinMaxTable_Record.eBuffTYPE.MoveSpeedMinMax);
		if( null != record)
		{
			fPhsicalMin = ( float)record.min;
			fPhsicalMax = ( float)record.max;
		}

		PercentPrintText_move( AsTableManager.Instance.GetTbl_String( 1078), texts[iIndex], textTitles[iIndex],
			savedCharStat.sFinalStatus.nMoveSpeed, savedCharStat.sDefaultStatus.nMoveSpeed, fPhsicalMin, fPhsicalMax);
		iIndex++;

		//exp
		bool isMaxLevel = false;
		Tbl_GlobalWeight_Record levelrecord = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 69);
		if( null != levelrecord && null != AsEntityManager.Instance.GetPlayerCharFsm())
		{
			int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>( eComponentProperty.LEVEL);
			if( iCurLevel >= ( int)levelrecord.Value)
				isMaxLevel = true;
		}

		if( true == isMaxLevel)
		{
			//texts[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1721);
			textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1721);
		}
		else
		{
			if( null != AsEntityManager.Instance.UserEntity)
			{
				m_strbuilderTemp_1.Remove( 0, m_strbuilderTemp_1.Length);

				int level = AsEntityManager.Instance.UserEntity.GetProperty<int>( eComponentProperty.LEVEL);
				eCLASS cls = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
				int curExp = AsEntityManager.Instance.UserEntity.GetProperty<int>( eComponentProperty.TOTAL_EXP);
				Tbl_UserLevel_Record curRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level);

				if( AsGameDefine.MAX_LEVEL > level)
				{
					Tbl_UserLevel_Record nextRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level + 1);
					if( null != nextRecord)
					{
						//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1081));
						//m_strbuilderTemp_1.Append( " ");
						m_strbuilderTemp_1.Append( curExp.ToString( "#,#0", CultureInfo.InvariantCulture));
						m_strbuilderTemp_1.Append( "/");
						m_strbuilderTemp_1.Append( nextRecord.TotalEXP.ToString( "#,#0", CultureInfo.InvariantCulture));
						texts[iIndex].Text = m_strbuilderTemp_1.ToString();
						textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1081);
					}
				}
				else
				{
					//m_strbuilderTemp_1.Append( AsTableManager.Instance.GetTbl_String( 1081));
					//m_strbuilderTemp_1.Append( " ");
					m_strbuilderTemp_1.Append( curExp.ToString( "#,#0", CultureInfo.InvariantCulture));
					m_strbuilderTemp_1.Append( "/");
					m_strbuilderTemp_1.Append( curRecord.TotalEXP.ToString( "#,#0", CultureInfo.InvariantCulture));
					texts[iIndex].Text = m_strbuilderTemp_1.ToString();
					textTitles[iIndex].Text = AsTableManager.Instance.GetTbl_String( 1081);
				}
			}
		}
	}

	// focus img
	public void AttachFocusImg( UIInvenSlot invenslot)
	{
		if( null == focusImg)
		{
			Debug.LogError( "PlayerStatusDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}

		if( null == invenslot)
			return;

		focusImg.gameObject.active = true;
		Vector3 vec3SlotPos = invenslot.transform.position;
		vec3SlotPos.z = -1;
		focusImg.transform.position = vec3SlotPos;
	}

	public void DetachFocusImg()
	{
		if( null == focusImg)
		{
			Debug.LogError( "PlayerStatusDlg::AttachFocusImg() [ null == focusImg ]");
			return;
		}

		focusImg.gameObject.active = false;
	}

	// slot
	public void ResetSlotItmes()
	{
		if( null == ItemMgr.HadItemManagement)
		{
			Debug.LogError( "PlayerStatusDlg::ResetSlotItmes() [ null == ItemMgr.HadItemManagement ] ");
			return;
		}

		if( null == slotItemParent)
		{
			Debug.LogError( "PlayerStatusDlg::ResetSlotItmes() [ null == slotItemParent ]");
			return;
		}

		if( null == slots)
		{
			Debug.LogError( "PlayerStatusDlg::ResetSlotItmes() [ null == slots ]");
			return;
		}

		int iStartIndex = m_iCurSlotPage * 10;
		int iIndex = iStartIndex;

		if( true == m_isOtherUser)
		{
			foreach( UIInvenSlot slot in slots)
			{
				slot.SetSlotIndex( iIndex);
				slot.DeleteSlotItem();

				RealItem realItem = GetOtherRealItem( iIndex);
				if( null == realItem)
				{
					++ iIndex;
					continue;
				}

				slot.CreateSlotItem( realItem, slotItemParent.transform);
				++ iIndex;
			}
		}
		else
		{
			foreach( UIInvenSlot slot in slots)
			{
				slot.SetSlotIndex( iIndex);
				slot.DeleteSlotItem();

				RealItem realItem = ItemMgr.HadItemManagement.Inven.GetIvenItem( iIndex);
				if( null == realItem)
				{
					++ iIndex;
					continue;
				}

				slot.CreateSlotItem( ItemMgr.HadItemManagement.Inven.GetIvenItem( iIndex), slotItemParent.transform);
				++ iIndex;
			}
		}
	}

	private void ClearSlot()
	{
		if( null == slots)
		{
			Debug.LogError( "PlayerStatusDlg::ClearSlot() [ null == slots ]");
			return;
		}

		foreach( UIInvenSlot slot in slots)
		{
			slot.DeleteSlotItem();
		}
	}

	public bool IsCheckItemSubtype( int iSlot, RealItem _item)
	{
		if( 0 == m_iCurSlotPage)
		{
			if( Item.eITEM_TYPE.EquipItem != _item.item.ItemData.GetItemType())
				return false;
		}
		else
		{
			if( Item.eITEM_TYPE.CosEquipItem != _item.item.ItemData.GetItemType())
				return false;
		}

		if( iSlot != _item.item.ItemData.GetSubType()-1)
			return false;

		return true;
	}

	void Awake()
	{
		// < ilmeda, 20120814
		//foreach( SpriteText _text in texts)
		//	AsLanguageManager.Instance.SetFontFromSystemLanguage( _text);

		//AsLanguageManager.Instance.SetFontFromSystemLanguage( subTitle);
		// ilmeda, 20120814 >
		
		if( null != btnCostume.spriteText )
		{
			btnCostume.spriteText.Text = AsTableManager.Instance.GetTbl_String(1820);
		}
	}

	// Use this for initialization
	void Start()
	{
		btnChangeSlot.SetInputDelegate( ChangeSlotBtnDelegate);
		btnClose.SetInputDelegate( CloseBtnDelegate);
		StartCoolTime();
	}

	// Update is called once per frame
	void Update()
	{
		CoolUpdate();

		if( true == m_bNeedClose)
		{
			AsHudDlgMgr.Instance.ClosePlayerStatus();
			m_bNeedClose =false;
		}

		if( false == btnChangeSlot.controlIsEnabled && null == moveItemSlot.slotItem)
			btnChangeSlot.controlIsEnabled = true;
	}

	public void SetBtnChange( int iSlotPage)
	{
		if( iSlotPage == 0)
		{
			normalEquipImg.gameObject.active = true;
			costumeEquipImg.gameObject.active = false;
			if( null != btnChangeSlot.spriteText)
				btnChangeSlot.spriteText.Text = AsTableManager.Instance.GetTbl_String( 1174);

		}
		else if( iSlotPage == 1)
		{
			normalEquipImg.gameObject.active = false;
			costumeEquipImg.gameObject.active = true;
			if( null != btnChangeSlot.spriteText)
				btnChangeSlot.spriteText.Text = AsTableManager.Instance.GetTbl_String( 1547);
		}
	}

	private void ChangeSlotBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( m_iCurSlotPage == 0)
			{
				m_iCurSlotPage = 1;
				SetBtnChange( m_iCurSlotPage);
			}
			else if( m_iCurSlotPage == 1)
			{
				m_iCurSlotPage = 0;
				SetBtnChange( m_iCurSlotPage);
			}

			ResetSlotItmes();
		}
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_bNeedClose = true;
		}
	}

	private bool IsUseInput()
	{
		if( false == gameObject.active || null != designationDlg) // ilmeda, 20120914
			return false;

		return true;
	}

	// move slot
	private void SetMoveSlot( UIInvenSlot slot)
	{
		moveItemSlot.SetSlotItem( slot.slotItem);
		moveItemSlot.SetSlotIndex( slot.slotIndex);
	}

	private void ResetMoveSlot()
	{
		moveItemSlot.SetSlotItem( null);
		moveItemSlot.SetSlotIndex( 0);
	}

	private void SetClickDownSlot( UIInvenSlot slot)
	{
		m_ClickDownItemSlot = slot;
	}

	public void SetRestoreSlot()
	{
		if( null == m_ClickDownItemSlot || null == moveItemSlot.slotItem)
			return;

		m_ClickDownItemSlot.SetSlotItem( moveItemSlot.slotItem);
		m_ClickDownItemSlot.ResetSlotItemPosition();
		ResetMoveSlot();
		DetachFocusImg();
	}

	public void SetSlotItem( int iSlotIndex, RealItem realItem)
	{
		if( true == m_isOtherUser)
			return;

		int iMaxIndex = ( m_iCurSlotPage * 10) + 9;
		int iMinIndex = m_iCurSlotPage * 10;

		int iIndex = iSlotIndex;
		if( iSlotIndex >= 10)
			iIndex = iSlotIndex % 10;

		if( iMaxIndex < iSlotIndex || iSlotIndex < iMinIndex)
			return;

		slots[iIndex].DeleteSlotItem();
		DetachFocusImg();
		if( null != realItem)
			slots[iIndex].CreateSlotItem( realItem, slotItemParent.transform);
	}

	private void OpenTooltip()
	{
		if( null == m_ClickDownItemSlot || null == m_ClickDownItemSlot.slotItem)
			return;

		if( true == m_isOtherUser)
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem, TooltipMgr.eCommonState.Sell, true);
			return;
		}

		bool isHaveEnchant = TooltipMgr.IsEnableEnchant( m_ClickDownItemSlot.slotItem.realItem.sItem.nEnchantInfo,
			m_ClickDownItemSlot.slotItem.realItem.sItem.nStrengthenCount);
		bool isStrength = StrengthenDlg.IsCanStrengthenItem( m_ClickDownItemSlot.slotItem.realItem);

		if( isHaveEnchant && isStrength)
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem, true, TooltipMgr.eCommonState.Socket_Strength);
		else if( isHaveEnchant)
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem, true, TooltipMgr.eCommonState.Socket);
		else if( isStrength)
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem, true, TooltipMgr.eCommonState.Strength);
		else
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.left, m_ClickDownItemSlot.slotItem.realItem, TooltipMgr.eCommonState.Equip, true);
	}

	public void InputDown( Ray inputRay)
	{
	}

	public void InputUp( Ray inputRay)
	{
		if( null != moveItemSlot.slotItem)
		{
			RaycastHit hit;
			if( Physics.Raycast( inputRay, out hit, 10000f, 1<<LayerMask.NameToLayer( "GUI")) == false)
				SetRestoreSlot();
		}
	}

	public void InputMove( Ray inputRay)
	{
		if( null != moveItemSlot.slotItem)
		{
			Vector3 pt = inputRay.origin;
			pt.z = moveItemSlot.gameObject.transform.position.z - 10.0f;
			moveItemSlot.SetSlotItemPosition( pt);
		}
	}

	public void GuiInputDown( Ray inputRay)
	{
		if( false == IsUseInput())
			return;

		SetRestoreSlot();
		SetClickDownSlot( null);
		m_fItemMoveTime = 0.0f;

		foreach( UIInvenSlot slot in slots)
		{
			if( null != slot.slotItem && true == slot.IsIntersect( inputRay))
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				SetClickDownSlot( slot);
				break;
			}
		}
	}

	public void SetEquipEffect( int iTarget)
	{
		if( 1 == m_iCurSlotPage && 10 <= iTarget)
			iTarget -= 10;

		if( 0 > iTarget || iTarget >= slots.Length)
			return;

		if( null == slots[iTarget])
			return;

		ResourceLoad.CreateUI( "UI/AsGUI/GUI_EquipEffect", slots[iTarget].transform, Vector3.zero);
	}

	public void GuiInputMove( Ray inputRay)
	{
		if( false == IsUseInput())
			return;

		if( true == m_isOtherUser)
			return;

		// exist move item
		if( null == moveItemSlot.slotItem)
		{
			if( null != m_ClickDownItemSlot)
			{
				if( true == m_ClickDownItemSlot.IsIntersect( inputRay))
				{
					if( m_fMaxItemMoveTime <= m_fItemMoveTime && false == AsCommonSender.isSendItemMove)
					{
						SetMoveSlot( m_ClickDownItemSlot);
						m_ClickDownItemSlot.SetSlotItem( null);
						AttachFocusImg( m_ClickDownItemSlot);
						m_fItemMoveTime = 0.0f;
						btnChangeSlot.controlIsEnabled = false;
					}

					m_fItemMoveTime += Time.deltaTime;
				}
			}
		}
		else
		{
			TooltipMgr.Instance.Clear();
		}
	}

	public int GetRingItemSlotIndex( Ray inputRay)
	{
		int iSlotIndex = int.MaxValue;

		if( slots.Length > 9)
		{
			if( true == slots[7].IsIntersect( inputRay))
			{
				iSlotIndex = 7;
			}
			else if( true == slots[8].IsIntersect( inputRay))
			{
				iSlotIndex = 8;
			}
		}

		return iSlotIndex;
	}

	public void GuiInputUp( Ray inputRay)
	{
		if( false == IsUseInput())
			return;

		if( null != moveItemSlot.slotItem && null != m_ClickDownItemSlot)
		{
			foreach( UIInvenSlot _equipSlot in slots)
			{
				if( true == _equipSlot.IsIntersect( inputRay))
				{
					if( ( int)Item.eEQUIP.Ring == moveItemSlot.slotItem.realItem.item.ItemData.GetSubType() &&
						( _equipSlot.slotIndex == 7 || _equipSlot.slotIndex == 8))
					{
						if( moveItemSlot.slotItem.realItem.getSlot == _equipSlot.slotIndex)
						{
							SetRestoreSlot();
							return;
						}

						if( AsPStoreManager.Instance.UnableActionByPStore() == true)
						{
							AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
													null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
							SetRestoreSlot();
							return;
						}

						AsSoundManager.Instance.PlaySound( moveItemSlot.slotItem.realItem.item.ItemData.m_strUseSound, Vector3.zero, false);
						AsCommonSender.SendMoveItem( moveItemSlot.slotItem.realItem.getSlot, _equipSlot.slotIndex);
						moveItemSlot.DeleteSlotItem();
					}
					else
					{
						SetRestoreSlot();
					}

					return;
				}
			}

			if( AsHudDlgMgr.Instance.IsOpenInven)
			{
				int iSlot = AsHudDlgMgr.Instance.invenDlg.GetInvenSlot( inputRay, moveItemSlot.slotItem.realItem);
				if( -1 != iSlot && true == AsEntityManager.Instance.UserEntity.IsCheckEquipEnable( moveItemSlot.slotItem.realItem))
				{
					if( 0 == moveItemSlot.slotItem.realItem.getSlot && false == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 10))
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 1563),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);

						SetRestoreSlot();
					}
					else if( true == AsEntityManager.Instance.UserEntity.IsBattleEnable( moveItemSlot.slotItem.realItem))
					{
						if( AsPStoreManager.Instance.UnableActionByPStore() == true)
						{
							AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
													null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
							SetRestoreSlot();
							return;
						}

						AsSoundManager.Instance.PlaySound( moveItemSlot.slotItem.realItem.item.ItemData.m_strUseSound, Vector3.zero, false);
						AsCommonSender.SendMoveItem( moveItemSlot.slotItem.realItem.getSlot, iSlot);
						moveItemSlot.DeleteSlotItem();
					}
					else
					{
						SetRestoreSlot();
					}
				}
				else
				{
					SetRestoreSlot();
				}
			}
			else
			{
				SetRestoreSlot();
			}
		}
		else if( null != m_ClickDownItemSlot)
		{
			if( m_ClickDownItemSlot.IsIntersect( inputRay))
			{
				OpenTooltip();
			}
		}
	}

	public void SetEquipToInven( RealItem _realItem)
	{
		if( null == _realItem)
			return;

		if( true == AsEntityManager.Instance.UserEntity.IsBattleEnable( _realItem))
		{
			int iTargetSlot = ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot();
			if( -1 != iTargetSlot)
			{
				if( 0 == _realItem.getSlot && false == ItemMgr.HadItemManagement.Inven.IsSlotEmpty( 10))
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 1563),
									null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return;
				}

				if( AsPStoreManager.Instance.UnableActionByPStore() == true)
				{
					AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
											null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
					return;
				}

				AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.m_strUseSound, Vector3.zero, false);
				AsCommonSender.SendMoveItem( _realItem.getSlot, iTargetSlot);

				if( AsHudDlgMgr.Instance.IsOpenInven)
					AsHudDlgMgr.Instance.invenDlg.SetCurEquipEffect( iTargetSlot);
			}
			else
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 20),
								null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			}
		}
	}

	public void GuiInputDClickUp( Ray inputRay)
	{
		if( false == IsUseInput())
			return;

		if( true == m_isOtherUser)
			return;

		if( null != moveItemSlot.slotItem)
		{
			SetRestoreSlot();
		}
		else if( null != m_ClickDownItemSlot && m_ClickDownItemSlot.IsIntersect( inputRay))
		{
			if( null != m_ClickDownItemSlot.slotItem)
				SetEquipToInven( m_ClickDownItemSlot.slotItem.realItem);
		}
	}

	#region -DesignationDlg_Impl
	private void OnDesignationBtn()
	{
		if( true == m_isOtherUser)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//Debug.Log ( "OnDesignationBtn");

		if( null == designationDlg)
		{
			designationDlg = Instantiate( Resources.Load( "UI/AsGUI/GUI_Designation_new")) as GameObject;
		}
		else
		{
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_DESIGNATION_DLG));
			CloseDesignationDlg();
		}
	}

	private void CloseDesignationDlg()
	{
		if( null == designationDlg)
			return;

		GameObject.DestroyImmediate( designationDlg);
		designationDlg = null;
	}

	public void SetDesignationText()
	{
		if( 0 >= AsDesignationManager.Instance.CurrentID)
		{
			if( null != subTitle)
				subTitle.Text = AsTableManager.Instance.GetTbl_String( 1355);
			return;
		}

		int currentID = AsDesignationManager.Instance.CurrentID;
		DesignationData data = AsDesignationManager.Instance.GetDesignation( currentID);
		if( null == data)
		{
			Debug.LogError( "Designation data is null : " + currentID);
			return;
		}

		if( null != subTitle)
			subTitle.Text = AsTableManager.Instance.GetTbl_String( data.name);
	}

	public void SetOtherUserDesignationText( int iIdx)
	{
		if( 0 >= iIdx)
		{
			if( null != subTitle)
				subTitle.Text = AsTableManager.Instance.GetTbl_String( 1355);
			return;
		}

		int currentID = iIdx;
		DesignationData data = AsDesignationManager.Instance.GetDesignation( currentID);
		if( null == data)
		{
			Debug.LogError( "Designation data is null : " + currentID);
			return;
		}

		if( null != subTitle)
			subTitle.Text = AsTableManager.Instance.GetTbl_String( data.name);
	}
	#endregion
}
