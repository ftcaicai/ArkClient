using UnityEngine;
using System.Collections;

public class AsIndunRewardDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private int m_nSelect = 0;
	private AsMessageBox m_msgboxIndunRewardCancel = null;
	private body_SC_INDUN_QUEST_PROGRESS_INFO m_nIndunQuestProgressInfoBuff = new body_SC_INDUN_QUEST_PROGRESS_INFO();
	private bool m_bSend_InDun_Reward_GetItem = false;
	private bool m_bBtnOKClick = false;

	public SpriteText m_textTitle = null;
	public SpriteText m_textClearTime = null;
	public SpriteText m_textBonusExp = null;
	public SpriteText m_textBonusGold = null;
	public SpriteText m_textTitleRewardItem = null;
	public SpriteText m_textTitleQuestItem = null;
	public SpriteText m_textBtn = null;
	public SimpleSprite m_imgRandBox_1 = null;
	public SimpleSprite m_imgRandBox_2 = null;
	public SimpleSprite m_imgRandBox_3 = null;
	public SpriteText m_textRewardItem_1 = null;
	public SpriteText m_textRewardItem_2 = null;
	public SpriteText m_textRewardItem_3 = null;
	public SpriteText m_textRewardItem_4 = null;
	public UIRadioBtn m_btnRandBox_1 = null;
	public UIRadioBtn m_btnRandBox_2 = null;
	public UIRadioBtn m_btnRandBox_3 = null;
	public ParticleSystem m_Particle_1;
	public ParticleSystem m_Particle_2;
	public ParticleSystem m_Particle_3;
	public ParticleSystem m_Particle_4;
	public UIInvenSlot m_RewardItem_1;
	public UIInvenSlot m_RewardItem_2;
	public UIInvenSlot m_RewardItem_3;
	public EnchantSlot m_RewardItem_4;
	
	void Start()
	{
		m_msgboxIndunRewardCancel= ( new GameObject( "msgboxIndunRewardCancel")).AddComponent<AsMessageBox>();
		CloseIndunRewardCancelMsgBox();
	}
	
	void Update()
	{
		if( true == gameObject.activeInHierarchy)
		{
			if( true == AsNotify.Instance.IsOpenDeathDlg)
			{
				AsNotify.Instance.CloseDeathDlg();
			}
		}
	}

	public void Open(GameObject goRoot, int nPlayTimeSec, int nExp, int nAddExp, long lGold, long lAddGold, long lExchangeGold, long lExchangeAddGold, body_SC_INDUN_QUEST_PROGRESS_INFO IndunQuestProgressInfo)
	{
		m_goRoot = goRoot;
		gameObject.SetActiveRecursively( true);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textClearTime);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textBonusExp);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textBonusGold);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textTitleRewardItem);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textTitleQuestItem);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textBtn);
		
		m_textTitle.Text = AsTableManager.Instance.GetTbl_String( 2319);
		int nMin = 0;
		int nSec = 0;
		if( nPlayTimeSec > 0)
		{
			nMin = nPlayTimeSec / 60;
			nSec = nPlayTimeSec % 60;
			if( nMin > 99)
			{
				nMin = 99;
				nSec = 59;
			}
		}

		m_textClearTime.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1896), nMin.ToString(), nSec.ToString());
		//m_textBonusExp.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1876), nExp.ToString(), nAddExp.ToString());
		m_textBonusGold.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1877), lGold.ToString(), lAddGold.ToString());
		m_textTitleRewardItem.Text = AsTableManager.Instance.GetTbl_String( 1879);
		m_textTitleQuestItem.Text = AsTableManager.Instance.GetTbl_String( 2197);
		m_textBtn.Text = AsTableManager.Instance.GetTbl_String( 4109);

		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 69);
		if( null != record)
		{
			if( AsUserInfo.Instance.SavedCharStat.level_ >= record.Value)
				m_textBonusExp.Text = string.Format( AsTableManager.Instance.GetTbl_String( 2359), lExchangeGold.ToString(), lExchangeAddGold.ToString());
			else
				m_textBonusExp.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1876), nExp.ToString(), nAddExp.ToString());
		}
		else
			m_textBonusExp.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1876), nExp.ToString(), nAddExp.ToString());

		m_textRewardItem_1.Text = "";
		m_textRewardItem_2.Text = "";
		m_textRewardItem_3.Text = "";
		m_textRewardItem_4.Text = "";
		
		m_nSelect = 0;
		
		if( ( nAddExp > 0 || lExchangeAddGold > 0) && lAddGold > 0)
		{
			//_SetIndunQuestRewardItem( IndunQuestProgressInfo);
			m_nIndunQuestProgressInfoBuff = IndunQuestProgressInfo;
		}
		else
		{
			m_nIndunQuestProgressInfoBuff.nInsQuestGroupTableUniqCode = 0;
		}

		m_bSend_InDun_Reward_GetItem = false;
		m_bBtnOKClick = false;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6143_EFF_Indun_Complete_Popup", Vector3.zero, false);
	}
	
	private void _SetIndunQuestRewardItem( body_SC_INDUN_QUEST_PROGRESS_INFO data)
	{
		if( 0 == data.nInsQuestGroupTableUniqCode)
			return;

		Tbl_InsQuestGroup_Record record = AsTableManager.Instance.GetInsQuestGroupRecord( data.nInsQuestGroupTableUniqCode);
		if( null == record)
		{
			Debug.LogError( "AsIndunRewardDlg::_SetIndunQuestRewardItem(), null == record, data.nInsQuestGroupTableUniqCode: " + data.nInsQuestGroupTableUniqCode);
			return;
		}
		
		eCLASS eCurClass = (eCLASS)( AsUserInfo.Instance.SavedCharStat.class_);
		int nIndunQuestRewardItemID = 0;
		int nIndunQuestRewardItemCount = 0;
		
		switch( eCurClass)
		{
		case eCLASS.DIVINEKNIGHT:
			nIndunQuestRewardItemID = record.Knight_Reward;
			nIndunQuestRewardItemCount = record.Knight_Reward_Count;
			break;
			
		case eCLASS.MAGICIAN:
			nIndunQuestRewardItemID = record.Magician_Reward;
			nIndunQuestRewardItemCount = record.Magician_Reward_Count;
			break;
			
		case eCLASS.CLERIC:
			nIndunQuestRewardItemID = record.Cleric_Reward;
			nIndunQuestRewardItemCount = record.Cleric_Reward_Count;
			break;
			
		case eCLASS.HUNTER:
			nIndunQuestRewardItemID = record.Hunter_Reward;
			nIndunQuestRewardItemCount = record.Hunter_Reward_Count;
			break;
		}
		
		Item item = ItemMgr.ItemManagement.GetItem( nIndunQuestRewardItemID);
		
		if( null == item)
		{
			Debug.LogError( "AsIndunRewardDlg::_SetIndunQuestRewardItem(), null == item, nIndunQuestRewardItemID: " + nIndunQuestRewardItemID);
			return;
		}
		
		m_RewardItem_4.CreateSlotItem( item, gameObject.transform);
		m_RewardItem_4.SetItemCountText( nIndunQuestRewardItemCount);
		m_textRewardItem_4.Text = AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
	}

	public void Close()
	{
		AsInstanceDungeonManager.Instance.InitIndunQuestProgressInfoBuff();

		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	public void OnBtnClose()
	{
		if( 0 == m_nSelect)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			OpenIndunRewardCancelMsgBox();
		}
		else
		{
			Close();
			AsInstanceDungeonManager.Instance.Send_InDun_Exit();
		}
	}
	
	public void OnBtnOk()
	{
		if( true == m_bSend_InDun_Reward_GetItem)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 == m_nSelect)
		{
			AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage( AsTableManager.Instance.GetTbl_String( 2320));
			return;
		}
		
		if( true == m_bBtnOKClick)
			return;
		
		m_bBtnOKClick = true;
		
		//Close();
		AsInstanceDungeonManager.Instance.Send_InDun_Exit();
	}
	
	public void OnRandBox_1()
	{
		if( 0 != m_nSelect)
			return;

		_SelectRandBox( 1);
	}

	public void OnRandBox_2()
	{
		if( 0 != m_nSelect)
			return;

		_SelectRandBox( 2);
	}

	public void OnRandBox_3()
	{
		if( 0 != m_nSelect)
			return;

		_SelectRandBox( 3);
	}

	public void OnBtnTooltip_1()
	{
		_OpenTooltip( m_RewardItem_1);
	}

	public void OnBtnTooltip_2()
	{
		_OpenTooltip( m_RewardItem_2);
	}

	public void OnBtnTooltip_3()
	{
		_OpenTooltip( m_RewardItem_3);
	}

	public void OnBtnTooltip_4()
	{
		_OpenTooltip( m_RewardItem_4);
	}

	private void _OpenTooltip(UIInvenSlot invenslot)
	{
		if( true == TooltipMgr.Instance.IsOpenAny())
		{
			TooltipMgr.Instance.Clear();
			return;
		}

		if( null != invenslot && null != invenslot.slotItem && null != invenslot.slotItem.realItem)
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, invenslot.slotItem.realItem);
		}
	}

	private void _OpenTooltip(EnchantSlot enchantslot)
	{
		if( true == TooltipMgr.Instance.IsOpenAny())
		{
			TooltipMgr.Instance.Clear();
			return;
		}
		
		if( null != enchantslot && null != enchantslot.getItem && 0 < enchantslot.getItem.ItemID)
		{
			TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, enchantslot.getItem.ItemID);
		}
	}
	
	private void _SelectRandBox(int nSelect)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		m_nSelect = nSelect;

		switch( nSelect)
		{
		case 1:
//			m_Particle_1.Play();
			m_btnRandBox_1.controlIsEnabled = true;
			m_btnRandBox_2.controlIsEnabled = false;
			m_btnRandBox_3.controlIsEnabled = false;
//			m_btnRandBox_1.Value = true;
//			m_btnRandBox_2.Value = false;
//			m_btnRandBox_3.Value = false;
			break;
		case 2:
//			m_Particle_2.Play();
			m_btnRandBox_1.controlIsEnabled = false;
			m_btnRandBox_2.controlIsEnabled = true;
			m_btnRandBox_3.controlIsEnabled = false;
//			m_btnRandBox_1.Value = false;
//			m_btnRandBox_2.Value = true;
//			m_btnRandBox_3.Value = false;
			break;
		case 3:
//			m_Particle_3.Play();
			m_btnRandBox_1.controlIsEnabled = false;
			m_btnRandBox_2.controlIsEnabled = false;
			m_btnRandBox_3.controlIsEnabled = true;
//			m_btnRandBox_1.Value = false;
//			m_btnRandBox_2.Value = false;
//			m_btnRandBox_3.Value = true;
			break;
		}

//		_HideRandBox();
		AsInstanceDungeonManager.Instance.Send_InDun_Reward_GetItem();
		m_bSend_InDun_Reward_GetItem = true;

		m_btnRandBox_1.collider.enabled = false;
		m_btnRandBox_2.collider.enabled = false;
		m_btnRandBox_3.collider.enabled = false;
	}
	
	private void _HideRandBox()
	{
		m_imgRandBox_1.gameObject.SetActive( false);
		m_imgRandBox_2.gameObject.SetActive( false);
		m_imgRandBox_3.gameObject.SetActive( false);
	}
	
	public void Recv_Integrated_Indun_GetItem_Result(sITEM item_get, sITEM item1, sITEM item2)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6144_EFF_Indun_RewardItem", Vector3.zero, false);

		_HideRandBox();
		RealItem _realItem = null;
		
		switch( m_nSelect)
		{
		case 1:
			m_Particle_1.Play();
			_realItem = new RealItem( item_get, 0);
			m_RewardItem_1.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item1, 0);
			m_RewardItem_2.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item2, 0);
			m_RewardItem_3.CreateSlotItem( _realItem, gameObject.transform);
			break;

		case 2:
			m_Particle_2.Play();
			_realItem = new RealItem( item1, 0);
			m_RewardItem_1.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item_get, 0);
			m_RewardItem_2.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item2, 0);
			m_RewardItem_3.CreateSlotItem( _realItem, gameObject.transform);
			break;
		
		case 3:
			m_Particle_3.Play();
			_realItem = new RealItem( item1, 0);
			m_RewardItem_1.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item2, 0);
			m_RewardItem_2.CreateSlotItem( _realItem, gameObject.transform);
			_realItem = new RealItem( item_get, 0);
			m_RewardItem_3.CreateSlotItem( _realItem, gameObject.transform);
			break;
		}
		m_textRewardItem_1.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem_1.slotItem.realItem.item.ItemData.nameId);
		m_textRewardItem_2.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem_2.slotItem.realItem.item.ItemData.nameId);
		m_textRewardItem_3.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem_3.slotItem.realItem.item.ItemData.nameId);

		_SetIndunQuestRewardItem( m_nIndunQuestProgressInfoBuff);

		m_bSend_InDun_Reward_GetItem = false;
	}

	public void OpenIndunRewardCancelMsgBox()
	{
		if( null != m_msgboxIndunRewardCancel)
			return;
		
		string strTitle = AsTableManager.Instance.GetTbl_String( 126);
		string strMsg = AsTableManager.Instance.GetTbl_String( 2321);
		m_msgboxIndunRewardCancel = AsNotify.Instance.MessageBox( strTitle, strMsg, this, "OnMsgBox_IndunRewardCancel_Ok", "OnMsgBox_IndunRewardCancel_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	}
	
	public void CloseIndunRewardCancelMsgBox()
	{
		if( null != m_msgboxIndunRewardCancel)
			m_msgboxIndunRewardCancel.Close();
	}

	public void OnMsgBox_IndunRewardCancel_Ok()
	{
		CloseIndunRewardCancelMsgBox();

		Close();
		AsInstanceDungeonManager.Instance.Send_InDun_Exit();
	}

	public void OnMsgBox_IndunRewardCancel_Cancel()
	{
		CloseIndunRewardCancelMsgBox();
	}
}
