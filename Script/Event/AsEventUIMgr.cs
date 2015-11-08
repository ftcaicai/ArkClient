using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsEventUIMgr : MonoBehaviour
{
	#region - Recommend -
	bool m_bRecommendEvent = false;
	bool m_bRecommendEventEnd = false;
	int m_nItemTableIdx = 0;
	string m_UserName;

	public void SetRecommend( bool flag, int nItemTableIdx)
	{
		m_bRecommendEvent = flag;
		m_nItemTableIdx = nItemTableIdx;
	}

	public bool RecommendEvent	{ get { return m_bRecommendEvent; } }
	public int RecommendItemTableIdx	{ get { return m_nItemTableIdx; } }
	public bool RecommendEventEnd	{ get { return m_bRecommendEventEnd; } set{m_bRecommendEventEnd = value;} }
	public string UserName	{ get { return m_UserName; } set{m_UserName = value;} }
	#endregion

	private static AsEventUIMgr ms_kIstance = null;
	public static AsEventUIMgr Instance
	{
		get { return ms_kIstance; }
	}

	private List<body2_SC_SERVER_EVENT_START> lstAlarms = new List<body2_SC_SERVER_EVENT_START>();
	private bool m_isRunCoroutine = false;

	void Awake()
	{
		if( ms_kIstance == null)
		{
			ms_kIstance = this;
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void InsertEventAlram( body2_SC_SERVER_EVENT_START data)
	{
		//	if eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE is already exist , this alarm must be skiped.
		if (data.eEventType == eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE && isExistEventType (data.eEventType) == true) 
		{
			Debug.Log( "event type[eEVENT_TYPE.eEVENT_TYPE_GACHA_GRADE_RATE] is already exist skip" );
			return;
		}

		lstAlarms.Add( data);

		if( false == m_isRunCoroutine)
			StartCoroutine( "_EventAlram");
	}

	bool isExistEventType( eEVENT_TYPE _type )
	{
		for (int i=0; i<lstAlarms.Count; i++) 
		{
			body2_SC_SERVER_EVENT_START data = lstAlarms[i];
			if( data.eEventType == _type )
				return true;
		}
		return false;
	}

	IEnumerator _EventAlram()
	{
		m_isRunCoroutine = true;

		while( true)
		{
			if( 0 == lstAlarms.Count)
			{
				m_isRunCoroutine = false;
				break;
			}

			GameObject obj = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_QAEventAlram");
			Debug.Assert( null != obj);
			GameObject go = GameObject.Instantiate( obj) as GameObject;
			Debug.Assert( null != go);
			AsQAEventAlram Alarm = go.GetComponent<AsQAEventAlram>();
			Debug.Assert( null != Alarm);

			body2_SC_SERVER_EVENT_START data = lstAlarms[0];
			Debug.Assert( null != data);
			Alarm.Init( data);

			yield return new WaitForSeconds( 7.0f);

			lstAlarms.RemoveAt( 0);
			GameObject.Destroy( go);
		}
	}

//	void OnGUI()
//	{
//		if( GUI.Button( new Rect( 10, 100, 80, 30), "event") == true)
//		{
//			body2_SC_SERVER_EVENT_START data = new body2_SC_SERVER_EVENT_START();
//			data.eEventType = eEVENT_TYPE.eEVENT_TYPE_TIME_EXP;
//			InsertEventAlram( data);
//		}
//	}
}
