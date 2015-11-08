using UnityEngine;
using System.Collections;

public class AsServerListStandbyDlg : MonoBehaviour {
	
	static bool s_Exist = false; public static bool Exist{get{return s_Exist;}}
	
	[SerializeField] SpriteText m_TextTitle;
	[SerializeField] SpriteText m_TextContent;
	[SerializeField] SpriteText m_TextNumber;
	[SerializeField] SpriteText m_TextTime;
	[SerializeField] UIButton m_BtnCenter;
	[SerializeField] UIButton m_BtnClose;
	
	int m_BeginOrder;
	int m_BeginWaitSec;
	
	int m_CurOrder = int.MaxValue;
	int m_CurWaitSec;
	
	void Awake()
	{
		m_BtnCenter.SetInputDelegate(OnCanceled);
		m_BtnClose.SetInputDelegate(OnCanceled);
	}
	
	public void Init(body_GC_LOGIN_POSSIBLE_RESULT _result)
	{
		m_BeginOrder = _result.nOrder;
		if(m_CurOrder >= _result.nOrder)
			m_CurOrder = _result.nOrder;
//		m_CurOrder = m_BeginOrder = _result.nOrder;
		m_CurWaitSec = m_BeginWaitSec = _result.nWaitSec;
		
		m_BtnCenter.Text = AsTableManager.Instance.GetTbl_String(1151);
		
		s_Exist = true;
		
		//title
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String(1533);
		//content
		string content = AsTableManager.Instance.GetTbl_String(1534);
		string serverName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( AsServerListBtn.selectedServerData.szServerName));
		m_TextContent.Text = string.Format(content, serverName);
		
		SetOrderAndTime();		
	}

	void Start()
	{
		InvokeRepeating("TickProcess", 0, 1);
		InvokeRepeating("PossibleProcess", 30, 30);
	}
	
	void Update()
	{
	
	}
	
	void OnDestroy()
	{
		s_Exist = false;
	}
	
	void OnCanceled(ref POINTER_INFO _ptr)
	{
		if(_ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			AsServerListBtn.CancelStandbyDlg();
			Destroy(gameObject);
		}
	}
	
	public void Cancel()
	{
		AsServerListBtn.CancelStandbyDlg();
		Destroy(gameObject);
	}
	
	void TickProcess()
	{
		if(m_CurWaitSec > 0)
			--m_CurWaitSec;
		
		int curPos = m_BeginOrder * m_CurWaitSec / m_BeginWaitSec;
		if(curPos < m_CurOrder)
			m_CurOrder = curPos;
		
		SetOrderAndTime();
	}
	
	void PossibleProcess()
	{
		body_CG_LOGIN_POSSIBLE possible = new body_CG_LOGIN_POSSIBLE();
		byte[] data = possible.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data );
		
		Debug.Log("body_CG_LOGIN_POSSIBLE is send");
	}
	
	void SetOrderAndTime()
	{
		//number
		string number = AsTableManager.Instance.GetTbl_String(1535);
		m_TextNumber.Text = "";// string.Format(number, m_CurOrder);
		//time
//		string time = AsTableManager.Instance.GetTbl_String(111111111);
		
		int hour = m_CurWaitSec / 3600;
		int minute = (m_CurWaitSec - hour * 3600) / 60;
		int sec = m_CurWaitSec % 60;
		
//		m_TextTime.Text = string.Format(time, hour, minute, sec);
		
		m_TextTime.Text = string.Format(number, m_CurOrder, hour, minute, sec);
	}
}
