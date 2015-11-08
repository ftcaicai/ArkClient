using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReturnBonusWindow : MonoBehaviour
{
	#region - singleton -
	static ReturnBonusWindow m_Instance;
	public static ReturnBonusWindow Instance	{ get { return m_Instance; } }
	#endregion
	
	[SerializeField] UIButton btnClose_;
	[SerializeField] UIButton btnPost_;
	List<GameObject> m_listObj = new List<GameObject>();
	
	public SpriteText m_TextName;
	public SpriteText m_TextSubName;
	public SpriteText m_TextContents;
	
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
		
		btnClose_.SetInputDelegate( OnCloseBtnClick);
		btnPost_.SetInputDelegate( OnPostBtnClick);
	}
	
	void Start()
	{
		btnPost_.Text = AsTableManager.Instance.GetTbl_String( 1760);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextSubName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextContents);

		m_TextName.Text = AsTableManager.Instance.GetTbl_String( 1313);
		m_TextSubName.Text = AsTableManager.Instance.GetTbl_String( 1314);
		m_TextContents.Text = AsTableManager.Instance.GetTbl_String( 1315);
		
		float time = AsTableManager.Instance.GetTbl_GlobalWeight_Record(77).Value * 0.001f;
		Destroy(gameObject, time);
	}

	public void Init( body_SC_BONUS_RETURN _return)
	{
		Debug.Log( "AttendBonusWindow::Init: day = " + _return.nDays);
		
////		string text = AsTableManager.Instance.GetTbl_String( 255);
////		string.Format( text, 
//		
//		if( _attend.nDays > 1)
//		{
//			for( int i=1; i<_attend.nDays; ++i)
//			{
//				GameObject obj = Instantiate( days_[0].gameObject) as GameObject;
//				m_listObj.Add( obj);
//				obj.transform.position = days_[i].transform.position;
//			}
//		}
	}
	
	void OnCloseBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			DestroyWindow();
		}
	}
	
	void OnPostBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			DestroyWindow();
			
			AsHudDlgMgr.Instance.OpenPostBoxDlg();
		}
	}
	
	void OnEventBtnClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			DestroyWindow();
		}
	}
	
	void DestroyWindow()
	{
		Destroy( gameObject);
		
		foreach( GameObject obj in m_listObj)
		{
			Destroy( obj);
		}
	}
	
	#region - public -
	public void Destroy()
	{
		QuestTutorialMgr.Instance.StartQuestTutorial();
		Destroy( gameObject);
	}
	#endregion
}
