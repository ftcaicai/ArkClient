using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsPvpDlg : MonoBehaviour
{
	public enum ePvpMode
	{
		ePvpMode_1 = 0,
		ePvpMode_2,
		ePvpMode_3,
		ePvpMode_4,
		ePvpMode_Max
	}
	
	private GameObject m_goRoot = null;
	private int[] m_aPvpTableIndex = new int[(int)ePvpMode.ePvpMode_Max];
	private Vector3[] m_aModeBtnOrgPos = new Vector3[(int)ePvpMode.ePvpMode_Max];
	private Color m_ActiveTextColor = new Color( 0.92f, 0.27f, 0.02f, 1.0f);
	private ePvpMode m_eCurPvpMode;
	private bool m_bTrainningMode;
	
	#region -private member value
	public UIButton m_BtnPvp1;
	public UIButton m_BtnPvp2;
	public UIButton m_BtnPvp3;
	public UIButton m_BtnPvp4;
	public UIButton m_BtnStart;
//	public UIButton m_BtnCash;
	public UIPanelTab m_BtnRanking;
	public UIPanelTab m_BtnTrainning;
	
	public SpriteText m_TextTitle;
//	public SpriteText m_TextModeInfo;
	public SpriteText m_TextModeDesc;
	public SpriteText m_TextRank;
	public SpriteText m_TextName;
	public SpriteText m_TextResultInfo;
	
	public SimpleSprite m_ImgRank;
	public SimpleSprite m_ImgMode;
	#endregion

	void Start()
	{
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		m_bTrainningMode = false;
		
		_ParsePvpData();
		
		_Init();
		m_eCurPvpMode = _SortModeBtn( false);

		_SetPvpInfo( m_eCurPvpMode);
		_SetUserInfo();
		_SetResultInfo();
		_SetImage_Mode( m_eCurPvpMode);
		
		AsPvpManager.Instance.m_bSendMatching = false;
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public int GetCurIndunTableIndex()
	{
		return m_aPvpTableIndex[(int)m_eCurPvpMode];
	}
	
	#region -button msg
	void OnClose()
	{
		Close();
	}
	
	void OnMode1vs1()
	{
		m_eCurPvpMode = ePvpMode.ePvpMode_1;

		_SetModeStringColor( ePvpMode.ePvpMode_1);
		_SetPvpInfo( ePvpMode.ePvpMode_1);
		_SetImage_Mode( ePvpMode.ePvpMode_1);
	}

	void OnMode2vs2()
	{
		m_eCurPvpMode = ePvpMode.ePvpMode_2;

		_SetModeStringColor( ePvpMode.ePvpMode_2);
		_SetPvpInfo( ePvpMode.ePvpMode_2);
		_SetImage_Mode( ePvpMode.ePvpMode_2);
	}

	void OnMode3vs3()
	{
		m_eCurPvpMode = ePvpMode.ePvpMode_3;

		_SetModeStringColor( ePvpMode.ePvpMode_3);
		_SetPvpInfo( ePvpMode.ePvpMode_3);
		_SetImage_Mode( ePvpMode.ePvpMode_3);
	}

	void OnMode4vs4()
	{
		m_eCurPvpMode = ePvpMode.ePvpMode_4;

		_SetModeStringColor( ePvpMode.ePvpMode_4);
		_SetPvpInfo( ePvpMode.ePvpMode_4);
		_SetImage_Mode( ePvpMode.ePvpMode_4);
	}
	
	void OnCash()
	{
		if( true == AsPvpDlgManager.Instance.IsOpenPvpDlg)
			AsPvpDlgManager.Instance.ClosePvpDlg();
		
		// open cash shop
		AsHudDlgMgr.Instance.NotEnoughMiracleProcessInGame();
	}
	
	void OnModeRanking()
	{
		m_bTrainningMode = false;
		m_eCurPvpMode = _SortModeBtn( false);
		_SetPvpInfo( m_eCurPvpMode);
		_SetImage_Mode( m_eCurPvpMode);
	}
	
	void OnModeTrainning()
	{
		m_bTrainningMode = true;
		m_eCurPvpMode = _SortModeBtn( true);
		_SetPvpInfo( m_eCurPvpMode);
		_SetImage_Mode( m_eCurPvpMode);
	}
	
	void OnStart()
	{
		int nIndunTableIndex = m_aPvpTableIndex[(int)m_eCurPvpMode];
		
		if( true == _CheckPvpStart() && false == AsPvpManager.Instance.m_bSendMatching)
		{
			AsPvpManager.Instance.Send_Pvp_Matching_Request( true, nIndunTableIndex, m_bTrainningMode);
			AsPvpManager.Instance.m_bSendMatching = true;
		}
	}
	#endregion

	#region -private function
	private void _Init()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnPvp1.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnPvp2.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnPvp3.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnPvp4.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnRanking.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnTrainning.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnStart.spriteText);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnCash.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
//		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextModeInfo);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextModeDesc);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextRank);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextResultInfo);
		
		m_BtnPvp1.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_1);
		m_BtnPvp2.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_2);
		m_BtnPvp3.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_3);
		m_BtnPvp4.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_4);
		m_BtnRanking.Text = AsTableManager.Instance.GetTbl_String( 947);
		m_BtnTrainning.Text = AsTableManager.Instance.GetTbl_String( 948);
		m_BtnStart.Text = AsTableManager.Instance.GetTbl_String( 949);
//		m_BtnCash.Text = AsTableManager.Instance.GetTbl_String( 1402);
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 883);
		
		m_aModeBtnOrgPos[(int)ePvpMode.ePvpMode_1] = m_BtnPvp1.transform.position;
		m_aModeBtnOrgPos[(int)ePvpMode.ePvpMode_2] = m_BtnPvp2.transform.position;
		m_aModeBtnOrgPos[(int)ePvpMode.ePvpMode_3] = m_BtnPvp3.transform.position;
		m_aModeBtnOrgPos[(int)ePvpMode.ePvpMode_4] = m_BtnPvp4.transform.position;
	}
	
	private void _ParsePvpData()
	{
		int nCount = AsTableManager.Instance.GetInDunTable().GetCount();
		
		for( int i = 0; i < nCount; i++)
		{
			int nPvpID = i + 1;
			Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( nPvpID);
			
			if( null == record)
			{
				Debug.Log( "AsPvpDlg::_ParsePvpData(), Tbl_InDun_Record record == null, nPvpID: " + nPvpID);
				continue;
			}
			
			if( 1 == record.IndunType) // 0: indun, 1: pvp
			{
				switch( record.ArenaType)
				{
				case ((int)ePvpMode.ePvpMode_1 + 1): m_aPvpTableIndex[(int)ePvpMode.ePvpMode_1] = nPvpID; break;
				case ((int)ePvpMode.ePvpMode_2 + 1): m_aPvpTableIndex[(int)ePvpMode.ePvpMode_2] = nPvpID; break;
				case ((int)ePvpMode.ePvpMode_3 + 1): m_aPvpTableIndex[(int)ePvpMode.ePvpMode_3] = nPvpID; break;
				case ((int)ePvpMode.ePvpMode_4 + 1): m_aPvpTableIndex[(int)ePvpMode.ePvpMode_4] = nPvpID; break;
				}
			}
		}
	}
	
	private string _GetModeNameString(ePvpMode pvpMode)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_aPvpTableIndex[(int)pvpMode]);
		
		if( null != record)
			return AsTableManager.Instance.GetTbl_String( record.Name);
		
		return string.Empty;
	}
	
	private void _SetModeStringColor(ePvpMode curMode)
	{
		m_BtnPvp1.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_1);
		m_BtnPvp2.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_2);
		m_BtnPvp3.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_3);
		m_BtnPvp4.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_4);

		if( ePvpMode.ePvpMode_1 == curMode)
			m_BtnPvp1.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_1);
		else if( ePvpMode.ePvpMode_2 == curMode)
			m_BtnPvp2.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_2);
		else if( ePvpMode.ePvpMode_3 == curMode)
			m_BtnPvp3.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_3);
		else if( ePvpMode.ePvpMode_4 == curMode)
			m_BtnPvp4.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_4);
	}
	
	private void _SetPvpInfo(ePvpMode pvpMode)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_aPvpTableIndex[(int)pvpMode]);
		if( null == record)
			return;
		
//		int nCurCount = 0;
//		int nMaxCount = record.PvPLimitCount;
//		
//		if( ePvpMode.ePvpMode_1 == pvpMode)
//			nCurCount = AsPvpManager.Instance.m_nLimitCount1;
//		else if( ePvpMode.ePvpMode_2 == pvpMode)
//			nCurCount = AsPvpManager.Instance.m_nLimitCount2;
//		else if( ePvpMode.ePvpMode_3 == pvpMode)
//			nCurCount = AsPvpManager.Instance.m_nLimitCount3;
//		else if( ePvpMode.ePvpMode_4 == pvpMode)
//			nCurCount = AsPvpManager.Instance.m_nLimitCount4;
		
		m_TextModeDesc.Hide( false);
//		m_BtnCash.Hide( false);
		
//		m_TextModeInfo.Text = string.Format( AsTableManager.Instance.GetTbl_String( 920),
//			AsTableManager.Instance.GetTbl_String( record.Name), nCurCount.ToString(), nMaxCount.ToString());
		
//		m_TextModeDesc.Text = AsTableManager.Instance.GetTbl_String( record.Description);
		
		if( true == m_bTrainningMode)
		{
//			m_TextModeInfo.Text = AsTableManager.Instance.GetTbl_String( 893);
//			m_TextModeDesc.Hide( true);
//			m_BtnCash.Hide( true);
			m_TextModeDesc.Text = AsTableManager.Instance.GetTbl_String( 893);
		}
		else
			m_TextModeDesc.Text = AsTableManager.Instance.GetTbl_String( record.Description);
	}
	
	private void _SetUserInfo()
	{
		//int nID = AsPvpManager.Instance.FindPvpGradeID( AsPvpManager.Instance.m_nPvpPoint);
		int nID = AsPvpManager.Instance.FindPvpGradeID( AsUserInfo.Instance.YesterdayPvpPoint, AsUserInfo.Instance.YesterdayPvpRankRate);
		Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( nID);
		if( null == record)
		{
			Debug.Log( "_SetUserInfo(): record == null, nID: " + nID);
			return;
		}
		
		m_TextRank.Text = AsTableManager.Instance.GetTbl_String( record.GradeNameID);
		m_TextName.Text = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<string>( eComponentProperty.NAME);

//		Texture tex = ResourceLoad.Loadtexture( record.GradeImagePath);
//		m_ImgRank.renderer.material.mainTexture = tex;
		
		// DelegateImage
		DelegateImageData data = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
		string strIconName = "";
		if( null == data)
			strIconName = "Default";
		else
			strIconName = data.iconName;

		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( strIconName);
		Texture tex = ResourceLoad.Loadtexture( sb.ToString());
		m_ImgRank.renderer.material.mainTexture = tex;
	}
	
	private void _SetResultInfo()
	{
		int nPoint = AsPvpManager.Instance.m_nPvpPoint;
		uint nTotal = AsPvpManager.Instance.m_nBattleAllCount;
		uint nWin = AsPvpManager.Instance.m_nVictoryCount;
		uint nDraw = AsPvpManager.Instance.m_nDrawCount;
		uint nLose = AsPvpManager.Instance.m_nLoseCount;
		uint nOdds = 0;
		
		if( nTotal > 0)
		{
			double res = ( (double)nWin / (double)nTotal) * 100;
			nOdds = (uint)res;
		}
		
		//uint nRank = AsPvpManager.Instance.m_nRank;
		
		m_TextResultInfo.Text = string.Format( AsTableManager.Instance.GetTbl_String( 887),
			nPoint.ToString(), nTotal.ToString(), nWin.ToString(), nDraw.ToString(), nLose.ToString(), nOdds.ToString());//, nRank.ToString());
	}
	
	private void _SetImage_Mode(ePvpMode pvpMode)
	{
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_aPvpTableIndex[(int)pvpMode]);
		if( null == record)
			return;
		
		Texture tex = ResourceLoad.Loadtexture( record.Icon);
		m_ImgMode.renderer.material.mainTexture = tex;
	}
	
	private bool _CheckPvpStart()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if( null == userEntity)
		{
			Debug.LogError( "AsPvpDlg::_CheckPvpStart(): null == userEntity");
			return false;
		}
		
		Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_aPvpTableIndex[(int)m_eCurPvpMode]);
		if( null == record)
		{
			Debug.LogError( "AsPvpDlg::_CheckPvpStart(): null == Tbl_InDun_Record");
			return false;
		}
		
		// party & level check
		AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( userEntity.UniqueId);
		
		if( null != partyUser)
		{
			if( true == partyUser.isCaptain)
			{
				// party member count check
				int nPartyMemberCount = AsPartyManager.Instance.GetPartyMemberList().Count;
				
				if( ePvpMode.ePvpMode_1 == m_eCurPvpMode && 1 == nPartyMemberCount)
					return _CheckPartyMemberConnection();
				else if( ePvpMode.ePvpMode_2 == m_eCurPvpMode && 2 >= nPartyMemberCount)
					return _CheckPartyMemberConnection();
				else if( ePvpMode.ePvpMode_3 == m_eCurPvpMode && 3 >= nPartyMemberCount)
					return _CheckPartyMemberConnection();
				else if( ePvpMode.ePvpMode_4 == m_eCurPvpMode && 4 >= nPartyMemberCount)
					return _CheckPartyMemberConnection();
				else
				{
					AsPvpManager.Instance.OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 892));
					return false;
				}

				// level check
				foreach( KeyValuePair<uint, AS_PARTY_USER> member in AsPartyManager.Instance.GetPartyMemberList())
				{
					if( member.Value.nLevel < record.MinLv)
					{
						AsPvpManager.Instance.OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 891));
						return false;
					}
				}
			}
			else
			{
				AsPvpManager.Instance.OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 890));
				return false;
			}
		}

		return true;
	}
	
	private bool _CheckPartyMemberConnection()
	{
		foreach( KeyValuePair<uint, AS_PARTY_USER> pair in AsPartyManager.Instance.GetPartyMemberList())
		{
			if( 0 == pair.Value.nSessionIdx) // nSesstionIdx: 0( logout)
			{
				AsPvpManager.Instance.OpenPvpMatchingErrorMsgBox( AsTableManager.Instance.GetTbl_String( 960));
				return false;
			}
		}
		
		return true;
	}
	
	private ePvpMode _SortModeBtn(bool bTrainningMode)
	{
		int nSortIndex = 0;
		bool bFirstActivate = true;
		ePvpMode eMode = ePvpMode.ePvpMode_1;
		
		for( int i = 0; i < (int)ePvpMode.ePvpMode_Max; i++)
		{
			Tbl_InDun_Record record = AsTableManager.Instance.GetInDunRecord( m_aPvpTableIndex[i]);
	
			if( null != record)
			{
				if( true == bTrainningMode)
				{
					if( 1 == record.PracticePvP)
					{
						_SetModeBtn( false, bFirstActivate, (ePvpMode)i, m_aModeBtnOrgPos[ nSortIndex]);
						nSortIndex++;
						if( true == bFirstActivate)
						{
							bFirstActivate = false;
							eMode = (ePvpMode)i;
						}
					}
					else
					{
						_SetModeBtn( true, false, (ePvpMode)i, Vector3.zero);
					}
				}
				else
				{
					if( 1 == record.RankPvP)
					{
						_SetModeBtn( false, bFirstActivate, (ePvpMode)i, m_aModeBtnOrgPos[ nSortIndex]);
						nSortIndex++;
						if( true == bFirstActivate)
						{
							bFirstActivate = false;
							eMode = (ePvpMode)i;
						}
					}
					else
					{
						_SetModeBtn( true, false, (ePvpMode)i, Vector3.zero);
					}
				}
			}
		}
		
		return eMode;
	}
	
	private void _SetModeBtn(bool bHide, bool bFirstActivate, ePvpMode eMode, Vector3 vPos)
	{
		switch( eMode)
		{
		case ePvpMode.ePvpMode_1:
			m_BtnPvp1.Hide( bHide);
			if( false == bHide)
			{
				m_BtnPvp1.transform.position = vPos;
				if( true == bFirstActivate)
					m_BtnPvp1.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_1);
				else
					m_BtnPvp1.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_1);
			}
			break;
			
		case ePvpMode.ePvpMode_2:
			m_BtnPvp2.Hide( bHide);
			if( false == bHide)
			{
				m_BtnPvp2.transform.position = vPos;
				if( true == bFirstActivate)
					m_BtnPvp2.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_2);
				else
					m_BtnPvp2.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_2);
			}
			break;
			
		case ePvpMode.ePvpMode_3:
			m_BtnPvp3.Hide( bHide);
			if( false == bHide)
			{
				m_BtnPvp3.transform.position = vPos;
				if( true == bFirstActivate)
					m_BtnPvp3.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_3);
				else
					m_BtnPvp3.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_3);
			}
			break;
			
		case ePvpMode.ePvpMode_4:
			m_BtnPvp4.Hide( bHide);
			if( false == bHide)
			{
				m_BtnPvp4.transform.position = vPos;
				if( true == bFirstActivate)
					m_BtnPvp4.Text = m_ActiveTextColor + _GetModeNameString( ePvpMode.ePvpMode_4);
				else
					m_BtnPvp4.Text = Color.black + _GetModeNameString( ePvpMode.ePvpMode_4);
			}
			break;
		}
	}
	#endregion
}
