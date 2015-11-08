using UnityEngine;
using System.Collections;

public class AsPvpRewardDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private float m_fOpenStartTimeSec = 0.0f;
	private float m_fOpenDelayTimeSec = 0.0f;
	private body_SC_ARENA_REWARDINFO m_Info = new body_SC_ARENA_REWARDINFO();
	private float m_fUpdateTimeBuf = 0.0f;
	private int m_nCloseDelayTimeSec = 30;
	private GameObject m_goPvpResultAction = null;
	
	public GameObject m_goWin = null;
	public GameObject m_goLose = null;
	public GameObject m_goDraw = null;
	public SpriteText m_textPoint = null;
	public SpriteText m_textExp = null;
	public SpriteText m_textBtn = null;
	
	void Start()
	{
	}
	
	void Update()
	{
		if( null != m_goRoot && true == gameObject.active)
		{
			float fCurTime = Time.realtimeSinceStartup;
			
			if( fCurTime - m_fUpdateTimeBuf > 1.0f)
			{
				m_fUpdateTimeBuf = fCurTime;
				m_nCloseDelayTimeSec--;
				m_textBtn.Text = string.Format( AsTableManager.Instance.GetTbl_String( 986), m_nCloseDelayTimeSec.ToString());
				
				if( m_nCloseDelayTimeSec <= 0)
				{
					OnBtnClose();
				}
			}
		}
	}
	
	public void UpdateOpenCheck()
	{
		if( null != m_goRoot && false == gameObject.active)
		{
			float fCurTime = Time.realtimeSinceStartup;
			
			if( m_fOpenStartTimeSec + m_fOpenDelayTimeSec < fCurTime)
			{
				// open reward dlg
				gameObject.SetActiveRecursively( true);
				_UpdatePvpResult( (ePVPRESULT)m_Info.nPvpResult);
				m_fUpdateTimeBuf = fCurTime;
				
				UIPanel panel = gameObject.GetComponent<UIPanel>();
				panel.BringIn();

				if( null != m_goPvpResultAction)
					GameObject.DestroyImmediate( m_goPvpResultAction);
			}
		}
	}

	public void Open(GameObject goRoot, GameObject goWinLoseDraw, body_SC_ARENA_REWARDINFO data, float fOpenDelayTimeSec)
	{
		m_goRoot = goRoot;
		gameObject.SetActiveRecursively( false);
		
		m_Info = data;
		m_fOpenStartTimeSec = Time.realtimeSinceStartup;
		m_fOpenDelayTimeSec = fOpenDelayTimeSec;
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textPoint);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textExp);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_textBtn);
		
		//m_textPoint.Text = string.Format( AsTableManager.Instance.GetTbl_String( 984), data.nAddPvpPoint, data.nPvpPoint);
		
		if( data.nAddPvpPoint >= 0)
			m_textPoint.Text = string.Format( AsTableManager.Instance.GetTbl_String( 984), data.nAddPvpPoint, data.nPvpPoint);
		else
		{
			int nPvpPoint = data.nAddPvpPoint * -1;
			m_textPoint.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1000), nPvpPoint, data.nPvpPoint);
		}
		
		m_textExp.Text = string.Format( AsTableManager.Instance.GetTbl_String( 985), data.nAddExp, data.nExp);
		m_textBtn.Text = string.Format( AsTableManager.Instance.GetTbl_String( 986), m_nCloseDelayTimeSec.ToString());
		
		_PlayPvpResultAction( goWinLoseDraw, (ePVPRESULT)data.nPvpResult);
		
		AsPvpDlgManager.Instance.ClosePvpTimeDlg();
		AsPvpDlgManager.Instance.ClosePvpCountDlg();
		AsPvpDlgManager.Instance.ClosePvpUserInfoDlg();

		if( null != TooltipMgr.Instance && true == TooltipMgr.Instance.IsOpenAny())
			TooltipMgr.Instance.Clear();
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public void OnBtnClose()
	{
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();

		if( player != null)
		{
			float fHP = player.GetProperty<float>( eComponentProperty.HP_MAX);
			player.SetProperty( eComponentProperty.HP_CUR, fHP);
			AsUserInfo.Instance.SavedCharStat.hpCur_ = fHP;
		}

		Close();
		AsInstanceDungeonManager.Instance.Send_InDun_Exit();
	}
	
	#region -private
	private void _UpdatePvpResult(ePVPRESULT ePvpResult)
	{
		m_goWin.SetActiveRecursively( false);
		m_goLose.SetActiveRecursively( false);
		m_goDraw.SetActiveRecursively( false);
		
		switch( ePvpResult)
		{
		case ePVPRESULT.ePVPRESULT_WIN: m_goWin.SetActiveRecursively( true); break;
		case ePVPRESULT.ePVPRESULT_LOSE: m_goLose.SetActiveRecursively( true); break;
		case ePVPRESULT.ePVPRESULT_DRAW: m_goDraw.SetActiveRecursively( true); break;
		}
	}
	
	private void _PlayPvpResultAction(GameObject goWinLoseDraw, ePVPRESULT ePvpResult)
	{
		//string strPrefab = "";
		string strSnd = "";

		switch( ePvpResult)
		{
		case ePVPRESULT.ePVPRESULT_WIN:
			//strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Win";
			strSnd = "Sound/PC/Common/Se_Common_PvPVictory";
			break;
		case ePVPRESULT.ePVPRESULT_LOSE:
			//strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Lose";
			strSnd = "Sound/PC/Common/Se_Common_PvPLose";
			break;
		case ePVPRESULT.ePVPRESULT_DRAW:
			//strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Draw";
			strSnd = "Sound/PC/Common/Se_Common_PvPDraw";
			break;
		}
		
		//m_goPvpResultAction = Instantiate( Resources.Load( strPrefab)) as GameObject;
		m_goPvpResultAction = Instantiate( goWinLoseDraw) as GameObject;

		UIPanel panel = m_goPvpResultAction.GetComponent<UIPanel>();
		panel.BringIn();

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
			AsSoundManager.Instance.PlaySound( strSnd, userEntity.ModelObject.transform.position, false);
		else
			Debug.Log( "AsPvpRewardDlg::Open(), null == userEntity");
	}
	#endregion -private
}
