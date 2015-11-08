using UnityEngine;
using System.Collections;

public class AsOptionDlg : MonoBehaviour
{
	public UIScrollList m_scrollList = null;
	public GameObject m_goListItem = null;
	public UIButton m_BtnClose;
	public SpriteText m_TextTitle;
	
	private GameObject m_goRoot = null;
	
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1379);
	}
	
	void Update()
	{
	}
	
	public void Open(GameObject goRoot)
	{
		_Init();
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
	}
	
	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		if( null != m_goRoot)
			Destroy( m_goRoot);
		
		AsGameMain.SaveOption();
	}
	
	// < private
	private void _BtnDelegate_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			Close();
		}
	}

	private void _Init()
	{
		m_BtnClose.SetInputDelegate( _BtnDelegate_Close);

		_CreateOptionBtn();
	}

	private void _CreateOptionBtn()
	{
		int nOptionBtnMaxCount = (int)(OptionBtnType.OptionBtnType_Max);
		
		for( int i = 0; i < nOptionBtnMaxCount; i++)
		{
			UIListItemContainer listItemContainer =  m_scrollList.CreateItem( m_goListItem) as UIListItemContainer;
			AsOptionBtn listBtn = listItemContainer.gameObject.GetComponent<AsOptionBtn>();
			bool bState = AsGameMain.GetOptionState( (OptionBtnType)i);

			listBtn.Init( (OptionBtnType)i, bState);
		}
	}
	// private >
}
