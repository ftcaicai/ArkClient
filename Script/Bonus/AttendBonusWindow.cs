using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttendBonusWindow : MonoBehaviour
{
	#region - singleton -
	static AttendBonusWindow m_Instance;
	public static AttendBonusWindow Instance	{ get { return m_Instance; } }
	#endregion
	
	[SerializeField] UIButton btnClose_;
	[SerializeField] UIButton btnPost_;
	[SerializeField] UIButton btnLoginGaCha_;

	public SpriteText m_TextName;
	public SpriteText m_TextContents;
	
	
	public AttendBonusSlot[]	m_attendBonusSlot;
	
	private int[] m_nDayTitleIndex = { 1303, 1304, 1305, 1306, 1307, 1308, 1309};
	private int[] m_nDayStringIndex = { 1311, 1393, 1394, 1395, 1396, 1397, 1398};
	private bool m_bInitialize = false;
	
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
		
		btnClose_.SetInputDelegate( OnCloseBtnClick);
		btnPost_.SetInputDelegate( OnPostBtnClick);
		btnLoginGaCha_.SetInputDelegate( OnLoginGaChaBtnClick);
	}
	
	void Start()
	{
		btnPost_.Text = AsTableManager.Instance.GetTbl_String(1760);
		btnLoginGaCha_.Text = AsTableManager.Instance.GetTbl_String(2196);;

//		_Initialize();
		
		StartCoroutine( "AutoDestroy");
	}
	
		
	IEnumerator AutoDestroy()
	{
		float time = AsTableManager.Instance.GetTbl_GlobalWeight_Record(77).Value * 0.001f;
		
		yield return new WaitForSeconds( time );
		
		DestroyWindow();
	}


	public void Init(body_SC_BONUS_ATTENDANCE _attend)
	{
		Debug.Log("AttendBonusWindow::Init: day = " + _attend.nDays);
		
		if( _attend.nDays <= 0)
		{
			Debug.LogError("AttendBonusWindow::Init: day = " + _attend.nDays);
			return;
		}
		
		if( m_attendBonusSlot.Length <= 0 )
		{
			Debug.LogError("m_attendBonusSlot.Length is 0");
			return;
		}

		_Initialize(_attend.nDays);
		
		int nWeekDay 	= ((_attend.nDays-1) % m_attendBonusSlot.Length) + 1;
		
		int nStrIndex = m_nDayStringIndex[ nWeekDay - 1];
		string strMsg = AsTableManager.Instance.GetTbl_String( nStrIndex);
		m_TextContents.Text = string.Format( strMsg, nWeekDay);
		
		for( int i = 0; i < nWeekDay ; i++)
		{
			if( i >= m_attendBonusSlot.Length )
				continue;
			
			Color _textColor = Color.white;
			
			bool isFinish = true;
			
			bool isSelect = false;
			
			if( i == nWeekDay-1 )
			{
				isSelect = true;
				isFinish = false;
			}
			
			m_attendBonusSlot[i].SetAttendItemProperty( _textColor , isFinish , isSelect );
		}
	}
	
	void OnCloseBtnClick(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			DestroyWindow();
		}
	}
	
	void OnPostBtnClick(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			DestroyWindow();
			
			AsHudDlgMgr.Instance.OpenPostBoxDlg();
		}
	}

	void OnLoginGaChaBtnClick(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			DestroyWindow();
			
			AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.FREE , 0, 0);			
		}
	}
	
	void DestroyWindow()
	{
		TooltipMgr.Instance.Clear();
		QuestTutorialMgr.Instance.StartQuestTutorial();
		Destroy(gameObject);
	}
	
	private void _Initialize(int nDays)
	{
		if( true == m_bInitialize)
			return;
		
		if( m_attendBonusSlot.Length <= 0 )
			return;
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextContents);

		m_TextName.Text = AsTableManager.Instance.GetTbl_String( 1310);
		
		//	default
		int nDefaultDay = 1;
		int nStrIndex = m_nDayStringIndex[nDefaultDay-1];
		string strMsg = AsTableManager.Instance.GetTbl_String( nStrIndex);
		m_TextContents.Text = string.Format( strMsg, nDefaultDay );
		
		int	nStartDay = m_attendBonusSlot.Length * ((nDays-1) / m_attendBonusSlot.Length) + 1;
		
		eCLASS	_class = AsUserInfo.Instance.GetCurrentUserEntity().GetProperty<eCLASS>(eComponentProperty.CLASS);
		
		
		Tbl_AttendBonus_Table	_attendBonusTable = AsTableManager.Instance.AttendBonusTable;
		
		for( int i = 0; i < m_attendBonusSlot.Length ; i++)
		{
			Tbl_AttendBonus_Record _attendBonusRecord = _attendBonusTable.GetRecord( nStartDay+i );
			if( _attendBonusRecord == null )
				continue;
			
			AttendBonusItem	_attendBonusItem = _attendBonusRecord.GetAttendBonusItem(_class);
			if( _attendBonusItem == null )
				continue;
			
			string 	txtDay = AsTableManager.Instance.GetTbl_String( m_nDayTitleIndex[i]);
			
			Color _textColor = Color.gray;
			
			m_attendBonusSlot[i].SetAttendItem( _attendBonusItem.ItemTableIndex , txtDay , _textColor , false , false , _attendBonusItem.ItemCount );
		}
		
		transform.position += new Vector3(0f, 0f, 0.5f);
		
		m_bInitialize = true;

	}
	
	public void GuiInputClickUp( Ray inputRay)
	{
		for( int i = 0; i < m_attendBonusSlot.Length ; i++)
		{
			if( m_attendBonusSlot[i].GuiInputClickUp(inputRay) == true )
				break;
		}
	}
	
	#region - public -
	public void Destroy()
	{
		TooltipMgr.Instance.Clear();
		QuestTutorialMgr.Instance.StartQuestTutorial();
		Destroy(gameObject);
	}
	#endregion
}
