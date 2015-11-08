using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsTradeManager : MonoBehaviour
{
	static AsTradeManager m_instance;
	public static AsTradeManager Instance	{ get { return m_instance; } }
	
	private ushort m_requestSessionID = 0;
	private ushort m_responseSessionID = 0;
	private AsMessageBox m_msgboxRequestWait = null;
	private AsMessageBox m_msgboxRequestChoice = null;
	private AsUserEntity m_userPlayer = null;
	private AsUserEntity m_otherPlayer = null;
	private float TRADE_RANGE = 10.0f;

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
		m_msgboxRequestWait = ( new GameObject( "msgboxRequestWait")).AddComponent<AsMessageBox>();
		m_msgboxRequestChoice = ( new GameObject( "msgboxRequestChoice")).AddComponent<AsMessageBox>();
		m_userPlayer = ( new GameObject( "userPlayer")).AddComponent<AsUserEntity>();
		m_otherPlayer = ( new GameObject( "otherPlayer")).AddComponent<AsUserEntity>();
		
		if( null != m_msgboxRequestWait)
			m_msgboxRequestWait.Destroy_Only();

		if( null != m_msgboxRequestChoice)
			m_msgboxRequestChoice.Destroy_Only();
	}

	void Update()
	{
		if( null != AsHudDlgMgr.Instance)
		{
			if( AsHudDlgMgr.Instance.IsOpenTrade || null != m_msgboxRequestWait || null != m_msgboxRequestChoice)
			{
				if( null == m_userPlayer || null == m_userPlayer.transform || null == m_otherPlayer || null == m_otherPlayer.transform)
				{
					AsHudDlgMgr.Instance.CloseTradeDlg( true);
					Request_Response_Cancel();
				}
				
				float fRange = Vector3.Distance( m_userPlayer.transform.position, m_otherPlayer.transform.position);
				
				if( fRange > TRADE_RANGE || true == _isUserPlayerCombat())
				{
					AsHudDlgMgr.Instance.CloseTradeDlg( true);
					Request_Response_Cancel();
				}
			}
		}
	}
	
	public void SetTradeTargetPlayer(ushort responseSessionID)
	{
		m_userPlayer = _GetUserEntityBySessionId( AsUserInfo.Instance.GamerUserSessionIdx);
		m_otherPlayer = _GetUserEntityBySessionId( responseSessionID);

		if( false == _isUserPlayerTradePossible())
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(25), eCHATTYPE.eCHATTYPE_SYSTEM);
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 25));
			return;
		}
		
		m_responseSessionID = responseSessionID;
		AsCommonSender.SendTradeRequest( responseSessionID);
	}
	
	public bool isOpenRequestWaitMsgBox()
	{
		if( null != m_msgboxRequestWait)
			return true;

		return false;
	}

	public bool isOpenRequestChoiceMsgBox()
	{
		if( null != m_msgboxRequestChoice)
			return true;

		return false;
	}
	
	// < Recive
	public void TradeRequest(ushort requestSessionID)
	{
		if( false == _isUserPlayer( requestSessionID))
		{
			m_userPlayer = _GetUserEntityBySessionId( AsUserInfo.Instance.GamerUserSessionIdx);
			if( false == _isUserPlayerTradePossible())
			{
				AsCommonSender.SendTradeResponse( false, requestSessionID);
				return;
			}
		}

		m_requestSessionID = requestSessionID;
		
		if( _isUserPlayer( requestSessionID))
		{
			string title = AsTableManager.Instance.GetTbl_String(1104);
			//string msgRequest = _GetUserName( m_responseSessionID) + AsTableManager.Instance.GetTbl_String( 21);
			string msgRequest = string.Format( AsTableManager.Instance.GetTbl_String( 21), _GetUserName( m_responseSessionID));
			//m_msgboxRequestWait = AsNotify.Instance.MessageBox( title, msgRequest, this, "OnMsgBox_TradeRequest_Cancel", "OnMsgBox_TradeRequest_Cancel", AsNotify.MSG_BOX_TYPE.MBT_CANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			m_msgboxRequestWait = AsNotify.Instance.MessageBox( title, msgRequest, AsNotify.MSG_BOX_TYPE.MBT_CANCEL, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			m_msgboxRequestWait.SetOkDelegate = OnMsgBox_TradeRequest_Cancel;
			m_msgboxRequestWait.SetCancelDelegate = OnMsgBox_TradeRequest_Cancel;

			m_userPlayer = _GetUserEntityBySessionId( m_requestSessionID);
			m_otherPlayer = _GetUserEntityBySessionId( m_responseSessionID);
		}
		else
		{
			if (AsHudDlgMgr.Instance.IsOpenCashStore || AsHudDlgMgr.Instance.IsOpenNpcStore)
			{
				AsCommonSender.SendTradeResponse(false, m_requestSessionID);
				return;
			}

			string title = AsTableManager.Instance.GetTbl_String(1105);
			//string msgResponse = _GetUserName( m_requestSessionID) + AsTableManager.Instance.GetTbl_String( 22);
			string msgResponse = string.Format( AsTableManager.Instance.GetTbl_String( 22), _GetUserName( m_requestSessionID));
			//m_msgboxRequestChoice = AsNotify.Instance.MessageBox( title, msgResponse, this, "OnMsgBox_TradeResponse_Ok", "OnMsgBox_TradeResponse_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_msgboxRequestChoice = AsNotify.Instance.MessageBox( title, msgResponse, AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
			m_msgboxRequestChoice.SetOkDelegate = OnMsgBox_TradeResponse_Ok;
			m_msgboxRequestChoice.SetCancelDelegate = OnMsgBox_TradeResponse_Cancel;

			m_userPlayer = _GetUserEntityBySessionId( AsUserInfo.Instance.GamerUserSessionIdx);
			m_otherPlayer = _GetUserEntityBySessionId( m_requestSessionID);
		}
	}
	
	public void TradeResponse(bool bAccept)
	{
		if( null != m_msgboxRequestWait)
		{
			m_msgboxRequestWait.Destroy_Only();
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(63), eCHATTYPE.eCHATTYPE_SYSTEM);
//			AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 25));
		}
		
		if( true == bAccept)
		{
			string strUserPlayer = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>( eComponentProperty.NAME);
			string strOtherPlayer = "";
			
			if( true == _isUserPlayer( m_requestSessionID))
				strOtherPlayer = _GetUserName( m_responseSessionID);
			else
				strOtherPlayer = _GetUserName( m_requestSessionID);

			AsHudDlgMgr.Instance.OpenTradeDlg( strUserPlayer, strOtherPlayer);
		}
	}
	
	public void TradeCancel()
	{
		if( null != m_msgboxRequestWait)
			m_msgboxRequestWait.Destroy_Only();
		
		if( null != m_msgboxRequestChoice)
			m_msgboxRequestChoice.Destroy_Only();

		AsHudDlgMgr.Instance.CloseTradeDlg( false);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(26), eCHATTYPE.eCHATTYPE_SYSTEM);
//		AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 26));
	}
	
	public void Request_Response_Cancel()
	{
		if( null != m_msgboxRequestWait)
		{
			m_msgboxRequestWait.Destroy_Only();
			OnMsgBox_TradeRequest_Cancel();
		}
		
		if( null != m_msgboxRequestChoice)
		{
			m_msgboxRequestChoice.Destroy_Only();
			OnMsgBox_TradeResponse_Cancel();
		}
	}
	
	public void TradeRegistrationItem(int nSessionIdx, bool bAddOrDel, int nInvenSlot, int nTradeSlot, sITEM sTradeItem)
	{
		AsHudDlgMgr.Instance.SetTradeItemSlot( _isUserPlayer( (ushort)nSessionIdx), bAddOrDel, nInvenSlot, nTradeSlot, sTradeItem);
	}
	
	public void TradeRegistrationGold(ushort nSessionIdx, long nGold)
	{
		bool isLeft = true;
		
		if( true == _isUserPlayer( nSessionIdx))
			isLeft = false;
		
		AsHudDlgMgr.Instance.SetTradeGold( isLeft, nGold);
	}
	
	public void TradeLock(ushort nSessionIdx, bool bLock)
	{
		bool isLeft = true;
		
		if( true == _isUserPlayer( nSessionIdx))
			isLeft = false;
		
		AsHudDlgMgr.Instance.SetTradeLock( isLeft, bLock);
	}
	
	public void TradeOk()
	{
		AsHudDlgMgr.Instance.CloseTradeDlg( false);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(33), eCHATTYPE.eCHATTYPE_SYSTEM);
//		AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 33));
		
		#region - condition -
		AsEmotionManager.Instance.Event_Condition_Trade();
		#endregion
	}
	// Recive >
	
	// < MessageBox
	public void OnMsgBox_TradeRequest_Cancel()
	{
		AsCommonSender.SendTradeCancel();
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(26), eCHATTYPE.eCHATTYPE_SYSTEM);
//		AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 26));
	}
	
	public void OnMsgBox_TradeResponse_Ok()
	{
		AsCommonSender.SendTradeResponse( true, m_requestSessionID);
	}
	
	public void OnMsgBox_TradeResponse_Cancel()
	{
		AsCommonSender.SendTradeResponse( false, m_requestSessionID);
		AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(26), eCHATTYPE.eCHATTYPE_SYSTEM);
//		AsMessageManager.Instance.InsertMessage( AsTableManager.Instance.GetTbl_String( 26));
	}
	// MessageBox >
	
	// < private
	private string _GetUserName(ushort sessionID)
	{
		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( sessionID);
		
		if( null == entity)
			return "";
		
		return entity[0].GetProperty<string>( eComponentProperty.NAME);
	}
	
	private bool _isUserPlayer(ushort sessionID)
	{
		return ( AsUserInfo.Instance.GamerUserSessionIdx == sessionID);
	}
	
	private AsUserEntity _GetUserEntityBySessionId(ushort sessionIdx)
	{
		List<AsUserEntity> entity = AsEntityManager.Instance.GetUserEntityBySessionId( sessionIdx);
		return entity[0];
	}
	
	private bool _isUserPlayerTradePossible()
	{
		if( true == _isUserPlayerCombat() || true == _isUserPlayerOpenNpcMenu()
			|| true == _isUserPlayerOpenStrengthen() || true == _isUserPlayerOpenEnchant()
			|| true == _isUserPlayerOpenQuest() || _isUserPlayerOpenSkillShop()
			|| true == _isuserPlayerOpenSynthesis()
		    || true == _isUserPlayerOpenLevelUpBonus()
		   || true == _isuserPlayerOpenSynthesisEnchant() 
		   || true == _isuserPlayerOpenSynthesisOption() 
		   || true == _isuserPlayerOpenSynthesisDisassemble() 
		   || true == _isuserPlayerOpenSynthesisCos() )
		{
			return false;
		}
		
		return true;
	}
	
	private bool _isUserPlayerCombat()
	{
		return m_userPlayer.GetProperty<bool>(eComponentProperty.COMBAT);
	}
	
	private bool _isUserPlayerOpenNpcMenu()
	{
		if( null != AsHUDController.Instance.m_NpcMenu)
			return AsHUDController.Instance.m_NpcMenu.gameObject.active;

		return false;
	}
	
	private bool _isUserPlayerOpenStrengthen()
	{
		return AsHudDlgMgr.Instance.IsOpenStrengthenDlg;
	}
	
	private bool _isUserPlayerOpenEnchant()
	{
		return AsHudDlgMgr.Instance.IsOpenEnchantDlg;
	}
	
	private bool _isUserPlayerOpenQuest()
	{
		return AsHUDController.Instance.m_questList.Visible;
	}
	
	private bool _isUserPlayerOpenSkillShop()
	{
		return AsHudDlgMgr.Instance.IsOpenedSkillshop;
	}
	
	private bool _isuserPlayerOpenSynthesis()
	{
		return AsHudDlgMgr.Instance.IsOpenSynthesisDlg;
	}

	private bool _isUserPlayerOpenLevelUpBonus()
	{
		return BonusManager.Instance.CheckLevelUpOpened();
	}

	private bool _isuserPlayerOpenSynthesisEnchant()
	{
		return AsHudDlgMgr.Instance.IsOpenSynEnchantDlg;
	}

	private bool _isuserPlayerOpenSynthesisOption()
	{
		return AsHudDlgMgr.Instance.IsOpenSynOptionDlg;
	}

	private bool _isuserPlayerOpenSynthesisDisassemble()
	{
		return AsHudDlgMgr.Instance.IsOpenSynDisDlg;
	}

	private bool _isuserPlayerOpenSynthesisCos()
	{
		return AsHudDlgMgr.Instance.IsOpenSynCosDlg;
	}
	// private >
}
