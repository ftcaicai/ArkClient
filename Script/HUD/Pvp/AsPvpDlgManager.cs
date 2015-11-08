using UnityEngine;
using System.Collections;

public class AsPvpDlgManager : MonoBehaviour
{
	static AsPvpDlgManager m_instance;
	public static AsPvpDlgManager Instance{ get{  return m_instance;}}

	[HideInInspector] public AsPvpDlg m_PvpDlg = null;
	[HideInInspector] public AsPvpUserInfoDlg m_PvpUserInfoDlg = null;
	[HideInInspector] public AsPvpCountDlg m_PvpCountDlg = null;
	[HideInInspector] public AsPvpFightDlg m_PvpFightDlg = null;
	[HideInInspector] public AsPvpTimeDlg m_PvpTimeDlg = null;
	[HideInInspector] public AsPvpRewardDlg m_PvpRewardDlg = null;

	void Awake()
	{
		m_instance = this;
	}

	void Start()
	{
	}
	
	void Update()
	{
		if( null != m_PvpRewardDlg)
		{
			if( false == m_PvpRewardDlg.gameObject.active)
				m_PvpRewardDlg.UpdateOpenCheck();
		}
	}

	public bool IsOpenPvpDlg
	{
		get
		{
			if( null == m_PvpDlg)
				return false;
			return m_PvpDlg.gameObject.active;
		}
	}

	public bool IsOpenPvpUserInfoDlg
	{
		get
		{
			if( null == m_PvpUserInfoDlg)
				return false;
			return m_PvpUserInfoDlg.gameObject.active;
		}
	}
	
	public bool IsOpenPvpCountDlg
	{
		get
		{
			if( null == m_PvpCountDlg)
				return false;
			return m_PvpCountDlg.gameObject.active;
		}
	}

	public bool IsOpenPvpFightDlg
	{
		get
		{
			if( null == m_PvpFightDlg)
				return false;
			return m_PvpFightDlg.gameObject.active;
		}
	}

	public bool IsOpenPvpTimeDlg
	{
		get
		{
			if( null == m_PvpTimeDlg)
				return false;
			return m_PvpTimeDlg.gameObject.active;
		}
	}
	
	public bool IsOpenPvpRewardDlg
	{
		get
		{
			if( null == m_PvpRewardDlg)
				return false;
			return m_PvpRewardDlg.gameObject.active;
		}
	}

	public IEnumerator OpenPvpDlg()
	{
		if( false == IsOpenPvpDlg)
		{
			if( null == m_PvpDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVP");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpDlg = go.GetComponentInChildren<AsPvpDlg>();

				if( null != m_PvpDlg)
				{
					m_PvpDlg.Open( go);
				}
			}
		}
		else
		{
			ClosePvpDlg();
		}
	}

	public void OpenPvpDlg_Coroutine()
	{
		StartCoroutine( OpenPvpDlg());
	}

	public IEnumerator OpenPvpUserInfoDlg()
	{
		if( false == IsOpenPvpUserInfoDlg)
		{
			if( null == m_PvpUserInfoDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVPEnter");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpUserInfoDlg = go.GetComponentInChildren<AsPvpUserInfoDlg>();

				if( null != m_PvpUserInfoDlg)
				{
					m_PvpUserInfoDlg.Open( go);
				}
			}
		}
		else
		{
			ClosePvpUserInfoDlg();
		}
	}

	public void OpenPvpUserInfoDlg_Coroutine()
	{
		StartCoroutine( OpenPvpUserInfoDlg());
	}
	
	public IEnumerator OpenPvpCountDlg()
	{
		if( false == IsOpenPvpCountDlg)
		{
			if( null == m_PvpCountDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVPcount_Ready");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpCountDlg = go.GetComponentInChildren<AsPvpCountDlg>();

				if( null != m_PvpCountDlg)
				{
					m_PvpCountDlg.Open( go);
				}
			}
		}
		else
		{
			ClosePvpCountDlg();
		}
	}

	public void OpenPvpCountDlg_Coroutine()
	{
		StartCoroutine( OpenPvpCountDlg());
	}

	public IEnumerator OpenPvpFightDlg()
	{
		if( false == IsOpenPvpFightDlg)
		{
			if( null == m_PvpFightDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVPcount_Fight");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpFightDlg = go.GetComponentInChildren<AsPvpFightDlg>();

				if( null != m_PvpFightDlg)
				{
					m_PvpFightDlg.Open( go);
				}
			}
		}
		else
		{
			ClosePvpFightDlg();
		}
	}

	public void OpenPvpFightDlg_Coroutine()
	{
		StartCoroutine( OpenPvpFightDlg());
	}

	public IEnumerator OpenPvpTimeDlg(int nTimeDurationSec)
	{
		if( false == IsOpenPvpTimeDlg)
		{
			if( null == m_PvpTimeDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVPTimeLimitDlg");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpTimeDlg = go.GetComponentInChildren<AsPvpTimeDlg>();

				if( null != m_PvpTimeDlg)
				{
					m_PvpTimeDlg.Open( go, nTimeDurationSec);
				}
			}
		}
		else
		{
			ClosePvpTimeDlg();
		}
	}

	public void OpenPvpTimeDlg_Coroutine(int nTimeDurationSec)
	{
		StartCoroutine( OpenPvpTimeDlg( nTimeDurationSec));
	}
	
	public IEnumerator OpenPvpRewardDlg(body_SC_ARENA_REWARDINFO data, float fOpenDelayTimeSec)
	{
		if( false == IsOpenPvpRewardDlg)
		{
			if( null == m_PvpRewardDlg)
			{
				GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/PVP/GUI_PVP_ResultPopup");
				yield return obj;

				GameObject go = GameObject.Instantiate( obj) as GameObject;
				m_PvpRewardDlg = go.GetComponentInChildren<AsPvpRewardDlg>();

				if( null != m_PvpRewardDlg)
				{
					string strPrefab = "";
					switch( (ePVPRESULT)data.nPvpResult)
					{
					case ePVPRESULT.ePVPRESULT_WIN: strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Win"; break;
					case ePVPRESULT.ePVPRESULT_LOSE: strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Lose"; break;
					case ePVPRESULT.ePVPRESULT_DRAW: strPrefab = "UI/AsGUI/PVP/GUI_PVP_Result_Draw"; break;
					}

					GameObject obj2 = ResourceLoad.LoadGameObject( strPrefab);
					yield return obj2;

					m_PvpRewardDlg.Open( go, obj2, data, fOpenDelayTimeSec);
				}
			}
		}
		else
		{
			ClosePvpRewardDlg();
		}
	}

	public void OpenPvpRewardDlg_Coroutine(body_SC_ARENA_REWARDINFO data, float fOpenDelayTimeSec)
	{
		StartCoroutine( OpenPvpRewardDlg( data, fOpenDelayTimeSec));
	}

	public void ClosePvpDlg()
	{
		if( null == m_PvpDlg)
			return;

		m_PvpDlg.Close();
	}

	public void ClosePvpUserInfoDlg()
	{
		if( null == m_PvpUserInfoDlg)
			return;

		m_PvpUserInfoDlg.Close();
	}

	public void ClosePvpCountDlg()
	{
		if( null == m_PvpCountDlg)
			return;

		m_PvpCountDlg.Close();
	}

	public void ClosePvpFightDlg()
	{
		if( null == m_PvpFightDlg)
			return;

		m_PvpFightDlg.Close();
	}

	public void ClosePvpTimeDlg()
	{
		if( null == m_PvpTimeDlg)
			return;

		m_PvpTimeDlg.Close();
	}

	public void ClosePvpRewardDlg()
	{
		if( null == m_PvpRewardDlg)
			return;

		m_PvpRewardDlg.Close();
	}
	
//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(50, 100, 80, 30), "ready") == true)
//			OpenPvpCountDlg_Coroutine();
//		if(GUI.Button(new Rect(50, 131, 80, 30), "fight") == true)
//			OpenPvpFightDlg_Coroutine();
//		if(GUI.Button(new Rect(50, 162, 80, 30), "time") == true)
//			OpenPvpTimeDlg_Coroutine( 40);
//		if(GUI.Button(new Rect(50, 193, 80, 30), "reward 1") == true)
//		{
//			body_SC_ARENA_REWARDINFO data = new body_SC_ARENA_REWARDINFO();
//			data.nPvpResult = 1;
//			data.nAddPvpPoint = 100;
//			data.nPvpPoint = 1000;
//			data.nAddExp = 10;
//			data.nExp = 100;
//			OpenPvpRewardDlg_Coroutine( data, 3.0f);
//		}
//		if(GUI.Button(new Rect(50, 224, 80, 30), "reward 2") == true)
//		{
//			body_SC_ARENA_REWARDINFO data = new body_SC_ARENA_REWARDINFO();
//			data.nPvpResult = 2;
//			data.nAddPvpPoint = 100;
//			data.nPvpPoint = 1000;
//			data.nAddExp = 10;
//			data.nExp = 100;
//			OpenPvpRewardDlg_Coroutine( data, 3.0f);
//		}
//		if(GUI.Button(new Rect(50, 255, 80, 30), "reward 3") == true)
//		{
//			body_SC_ARENA_REWARDINFO data = new body_SC_ARENA_REWARDINFO();
//			data.nPvpResult = 3;
//			data.nAddPvpPoint = 100;
//			data.nPvpPoint = 1000;
//			data.nAddExp = 10;
//			data.nExp = 100;
//			OpenPvpRewardDlg_Coroutine( data, 3.0f);
//		}
//		if(GUI.Button(new Rect(50, 286, 80, 30), "info") == true)
//		{
//			sARENAUSERINFO info = new sARENAUSERINFO();
//			
//			info.nCharUniqKey = 1;
//			info.szCharName = "test_1";
//			info.nImageTableIdx = 1;
//			info.eClass = eCLASS.DIVINEKNIGHT;
//			info.nTeamType = 0;
//			info.nSlotIndex = 0;
//			info.nLevel = 10;
//			info.nRate = 50;
//			info.nPvpPoint = 1000;
//			info.nEnterState = 2;
//			
//			AsPvpManager.Instance.GetUserInfoList.Add( info.nCharUniqKey, info);
//
//			OpenPvpUserInfoDlg_Coroutine();
//		}
//	}
}
