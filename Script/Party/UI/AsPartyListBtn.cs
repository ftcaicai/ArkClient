using UnityEngine;
using System.Collections;
using System.Text;
public class AsPartyListBtn : MonoBehaviour
{
	public SpriteText m_PartyName = null;
	public SpriteText m_UserCnt = null;
	public SpriteText m_SelectArea = null;
	public sPARTYLIST m_partyListData;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetData( sPARTYLIST partyListData)
	{
		m_partyListData = partyListData;

		Tbl_ZoneMap_Record record = AsTableManager.Instance.GetZoneMapRecord( m_partyListData.nMapIdx);
		if( null != record)
			m_SelectArea.Text = AsTableManager.Instance.GetTbl_String( record.getTooltipStrIdx);

		StringBuilder sb = new StringBuilder();
		sb.Insert( 0,m_partyListData.nUserCnt.ToString());
		sb.Append( "/");
		sb.Append( m_partyListData.sOption.nMaxUser);

		m_UserCnt.Text = sb.ToString();
		string partyName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( m_partyListData.sOption.szPartyName));
		m_PartyName.Text = partyName;
	}

	public void PartyInfo()
	{
		//#21322
		if( null == AsPartyManager.Instance.PartyInfoDlg || AsPartyManager.Instance.SendDetailPartyIdx != m_partyListData.nPartyIdx)
		{
			AsPartyManager.Instance.SendDetailPartyIdx = m_partyListData.nPartyIdx;
			AsPartySender.SendPartyDetailInfo( m_partyListData.nPartyIdx);
		}
	}

	public void PartyJoin()
	{
		AsPartyManager.Instance.SendDetailPartyIdx = 0;
		AsPartyManager.Instance.SendNoticePartyIdx = 0;
		AsPartyManager.Instance.PartyUI.PartyListDlg.Close();
		AsPartyManager.Instance.ClosePartyInfoDlg();	
		AsPartyManager.Instance.PartyUI.CloseSelectAreaDlg(); //#21346
		AsPartySender.SendPartyJoinRequest( m_partyListData.nPartyIdx);	//2014.01.16
	}
}
