using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsCenterNotify : MonoBehaviour
{
	#region - m_CenterNotifyItem -
	public enum eCENTER_NOTIFY
	{
		NONE = 0,
		GM_Message,	//게임내 공지.
		ItemPutOn,	//아이템 장착.
		ItemStrengthen,	//아이템 강화.
		ItemGet,	//아이템 획득.
		QuestMessage,	//퀘스트 진행 상항.
		MAX
	}

	public AsCenterNotifyItem m_CenterNotifyItem = null;
	public Color rankUpColor = new Color( 0.9f, 0.3f, 0f, 1f);
	public Color rankDownColor = new Color( 0.1f, 0.7f, 1f, 1f);

	private List<AsCenterNotifyItem> m_listNotifyItem = new List<AsCenterNotifyItem>();
	private List<AsCenterNotifyItem> m_listRemoveNotifyItem = new List<AsCenterNotifyItem>();
	private float m_fStartTime = 0.0f;
	private float m_fDestoryTime = 0.5f;
	private float m_NextWaitTime = 0.0f;
	private int m_ShowTotalLine = 0;
	#endregion

	// Use this for initialization
	void Start()
	{
		m_ShowTotalLine = 0;
	}

	// Update is called once per frame
	public void UpdateData()
	{
		int line = 0;
		foreach( AsCenterNotifyItem stData in m_listNotifyItem)
		{
			if( stData == null)
			{
				m_ShowTotalLine = 0;
				m_listNotifyItem.Clear();
				break;
			}

			if( !stData.ShowCommand)
			{
				if( ( Time.time - m_fStartTime) > m_NextWaitTime && ( stData.Line < 5))
				{
					m_fStartTime = Time.time;
					m_ShowTotalLine++;
					stData.Show();
					stData.Line = 0;
				}
			}
		}

		foreach( AsCenterNotifyItem stData in m_listNotifyItem)
		{
			if( stData == null)
			{
				m_ShowTotalLine = 0;
				m_listNotifyItem.Clear();
				break;
			}

			if( stData.ShowCommand)
			{
				stData.Line = m_ShowTotalLine - line;

				if( ( Time.time - stData.StartFadeInTime) > m_fDestoryTime  || ( stData.Line > 4))
				{
					m_ShowTotalLine--;
					stData.Hide();
					m_listRemoveNotifyItem.Add( stData);
				}
				else
				{
					line++;
				}
			}
		}

		if( 0 < m_listRemoveNotifyItem.Count)
		{
			foreach( AsCenterNotifyItem stRemoveData in m_listRemoveNotifyItem)
			{
				DestroyImmediate( stRemoveData.ParentObject);
				m_listNotifyItem.Remove( stRemoveData);
			}

			m_listRemoveNotifyItem.Clear();
		}
	}

	//게임내 공지.
	public void AddGMMessage( string _msg)
	{
		StringBuilder sb_msg = new StringBuilder();
		sb_msg.Insert( 0, AsEventNotifyMgr.Instance.m_GMMssageColor.ToString());
		sb_msg.AppendFormat( "{0}", _msg);

		AddMessageItemPanel( sb_msg.ToString(), eCENTER_NOTIFY.GM_Message);
	}

	//아이템 장착.
	
	public void AddItemPutOnMessage( RealItem _equipItem, int nItemIdx, byte nStrengthenCount, sITEM _sitem)
	{
		Item item = ItemMgr.ItemManagement.GetItem( nItemIdx);
		if( null == item)
			return;

		StringBuilder sbStrengthen = new StringBuilder();
		if( 0 < nStrengthenCount)
			sbStrengthen.AppendFormat( "{0}+{1}", AsEventNotifyMgr.Instance.m_StrengthenNum.ToString(), nStrengthenCount.ToString());

		sbStrengthen.AppendFormat( "{0}{1}", item.ItemData.GetGradeColor().ToString(), AsTableManager.Instance.GetTbl_String( item.ItemData.nameId));

		int iRankRealPoint = ItemMgr.GetRealRankPoint( _sitem, item);

		StringBuilder sb = new StringBuilder();
		/*string strItemRank;
		if( 0 < iRankRealPoint)
			strItemRank = string.Format( "+{0}", iRankRealPoint);
		else
			strItemRank = iRankRealPoint.ToString();*/

		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(849), sbStrengthen.ToString(), iRankRealPoint);

		if( null == _equipItem)
		{
			if( 0 < iRankRealPoint)
				sb.AppendFormat( " {0}({1} +{2})", rankUpColor, AsTableManager.Instance.GetTbl_String(1666), iRankRealPoint);
			else
				sb.AppendFormat( " {0}({1} {2})", rankDownColor, AsTableManager.Instance.GetTbl_String(1666), iRankRealPoint);
		}
		else
		{
			int iEquipRankRealPoint = ItemMgr.GetRealRankPoint( _equipItem.sItem, _equipItem.item);
			int iDiffPoint = iRankRealPoint - iEquipRankRealPoint;
			if( 0 < iDiffPoint)
				sb.AppendFormat( " {0}({1} +{2})", rankUpColor, AsTableManager.Instance.GetTbl_String(1666), iDiffPoint);
			else
				sb.AppendFormat( " {0}({1} {2})", rankDownColor, AsTableManager.Instance.GetTbl_String(1666), iDiffPoint);
		}

		AsChatManager.Instance.InsertSystemChat( sb.ToString(), eCHATTYPE.eCHATTYPE_SYSTEM_ITEM);

		AddMessageItemPanel( sb.ToString(), eCENTER_NOTIFY.ItemPutOn);
	}

	//아이템 강화.
	public void AddItemStrengthenOnMessage( body_SC_USER_EVENT_NOTIFY notify)
	{
		Item item = ItemMgr.ItemManagement.GetItem( notify.sItem.nItemTableIdx);
		if( null == item)
			return;

		StringBuilder sbStrengthenCount = new StringBuilder();
		sbStrengthenCount.AppendFormat( "+{0}", notify.sItem.nStrengthenCount.ToString());

		StringBuilder sbItemName = new StringBuilder();
		sbItemName.Insert( 0, item.ItemData.GetGradeColor().ToString());
		sbItemName.AppendFormat( "{0}", AsTableManager.Instance.GetTbl_String( item.ItemData.nameId));

		string chatMsg = string.Format( AsTableManager.Instance.GetTbl_String(850),
			AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( notify.szCharName)), sbStrengthenCount.ToString(), sbItemName.ToString());
		AsChatManager.Instance.InsertSystemChat( chatMsg, eCHATTYPE.eCHATTYPE_SYSTEM_ITEM);

		AddMessageItemPanel( chatMsg, eCENTER_NOTIFY.ItemStrengthen);
	}

	//아이템 획득.
	public void AddItemGetMessage( string text)
	{
		AddMessageItemPanel( text, eCENTER_NOTIFY.ItemGet);
	}

	public void AddMessage(string text)
	{
		AddMessageItemPanel(text, eCENTER_NOTIFY.QuestMessage);
	}

	public void AddQuestMessage( string _msg, bool bComplete)
	{
		StringBuilder sb_text = new StringBuilder();

		if( true == bComplete)
		{
			sb_text.Insert( 0, AsHudDlgMgr.Instance.questCompleteMsgManager.colorCompleteQuest.ToString());
			sb_text.AppendFormat( "{0}", _msg);
			AddMessageItemPanel( sb_text.ToString(), eCENTER_NOTIFY.QuestMessage);
		}
		else
		{
			sb_text.Insert( 0, AsHudDlgMgr.Instance.questCompleteMsgManager.colorProgresssionQuest.ToString());
			sb_text.AppendFormat( "{0}", _msg);
			AddMessageItemPanel( sb_text.ToString(), eCENTER_NOTIFY.QuestMessage);
		}
	}

	public void AddQuestMessageColorTag( string _msg)
	{
		AddMessageItemPanel( _msg, eCENTER_NOTIFY.QuestMessage);
	}

	public void AddTradeMessage( string _msg)
	{
		StringBuilder sb_msg = new StringBuilder();
		sb_msg.Insert( 0, AsEventNotifyMgr.Instance.m_ItemPutOn.ToString());
		sb_msg.AppendFormat( "{0}", _msg);

		AddMessageItemPanel( sb_msg.ToString(), eCENTER_NOTIFY.ItemPutOn);
	}

	private void  AddMessageItemPanel( string text, eCENTER_NOTIFY eCENTER_NOTIFY_Type)
	{
		m_fDestoryTime = ( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 50).Value / 1000.0f) +
		( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 16).Value / 1000.0f) +
		( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 51).Value / 1000.0f);

		m_NextWaitTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 52).Value / 1000.0f;

		GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_CenterNotify");
		GameObject centerNotifyObject = GameObject.Instantiate( obj)  as GameObject;
		AsCenterNotifyItem m_CenterNotifyItem = centerNotifyObject.GetComponentInChildren<AsCenterNotifyItem>();
		m_CenterNotifyItem.SetText( text, centerNotifyObject);
		m_listNotifyItem.Add( m_CenterNotifyItem);
	}

	public void AddResetAlarmMessage( string msg)
	{
		StringBuilder sb = new StringBuilder( AsEventNotifyMgr.Instance.m_ResetAlarmColor.ToString());
		sb.Append( msg);
		AddMessageItemPanel( sb.ToString(), eCENTER_NOTIFY.ItemGet);
	}

	public void AddGuildNotice( string notice)
	{
		StringBuilder sb = new StringBuilder( AsEventNotifyMgr.Instance.m_GuildNoticeColor.ToString());
		sb.Append( notice);
		AddMessageItemPanel( sb.ToString(), eCENTER_NOTIFY.ItemGet);
	}
	
	public void AddPartyMoveNotice( string notice)
	{
		StringBuilder sb = new StringBuilder( AsEventNotifyMgr.Instance.m_GuildNoticeColor.ToString());
		sb.Append( notice);
		AddMessageItemPanel( sb.ToString(), eCENTER_NOTIFY.ItemGet);
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
	}	
}

