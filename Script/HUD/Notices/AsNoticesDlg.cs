using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WmWemeSDK.JSON;

public class AsNoticesDlg : MonoBehaviour
{
	private const int MAX_IMAGE_COUNT = 5;
	public delegate void CloseDelegate();
	private CloseDelegate closeCallback = null;
	public CloseDelegate CloseCallback
	{
		set	{ closeCallback = value; }
	}

	[SerializeField] SpriteText m_Title = null;
	[SerializeField] UIScrollList m_list = null;
	[SerializeField] GameObject m_listItem = null;
	[SerializeField] SimpleSprite[] m_Counts = null;
	[SerializeField] SimpleSprite m_SelectPage = null;
	[SerializeField] UIButton m_BtnClose = null;
	[SerializeField] UIRadioBtn m_DayCloseBtn = null;
	[SerializeField] UIButton btnPrev = null;
	[SerializeField] UIButton btnNext = null;
	
	private int imageCount = 0;
	private int curPage = 0;
	private List<string> responseList = new List<string>();

	// Use this for initialization
	void Start()
	{
		m_Title.Text = AsTableManager.Instance.GetTbl_String(1936);
		m_DayCloseBtn.Text = AsTableManager.Instance.GetTbl_String(1937);
		m_BtnClose.SetInputDelegate( CloseBtnDelegate);
		m_DayCloseBtn.SetInputDelegate( DayCloseBtnDelegate);
		m_list.AddItemSnappedDelegate( ChangedItemDelegate);
		btnPrev.SetInputDelegate( OnPrevBtn);
		btnNext.SetInputDelegate( OnNextBtn);

		StartCoroutine( "InsertNotice");
	}
	
	private void OnSelect()
	{
		Application.OpenURL( responseList[ curPage]);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void DestroyWindow()
	{
		if( null != closeCallback)
			closeCallback();
		
		Destroy( gameObject);
	}
	
	public void ReOpen()
	{
		m_DayCloseBtn.gameObject.SetActiveRecursively( false);
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			DestroyWindow();
		}
	}
	
	public void Close()
	{
		DestroyWindow();
	}
	
	//이미지 파일 처리...
	public IEnumerator InsertNotice()
	{
		StringBuilder sb = new StringBuilder( AsNetworkDefine.ImageServerAddress);
		sb.Append( "event_new.json");

		WWW www = new WWW( sb.ToString());
		yield return www;

		JSONObject topObj = JSONObject.Parse( www.text);
		www.Dispose();
		www = null;

		JSONObject eventObj = topObj.GetObject( "event");
		Debug.Assert( null != eventObj);
		IEnumerator<KeyValuePair<string,JSONValue>> enumerator = eventObj.GetEnumerator();
		while( enumerator.MoveNext())
		{
			if( MAX_IMAGE_COUNT <= imageCount)
				break;
			
			JSONObject pageData = JSONObject.Parse( enumerator.Current.Value.ToString());
			
			responseList.Add( pageData[ "response"].Str);
			
			sb.Remove( 0, sb.Length);
			sb.Append( AsNetworkDefine.ImageServerAddress);
			sb.Append( pageData[ "image"].Str);

			www = new WWW( sb.ToString());
			yield return www;

			UIListItem item = m_list.CreateItem( m_listItem) as UIListItem;
			item.SetTexture( www.texture);
			item.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, www.texture.width, www.texture.height));

			www.Dispose();
			www = null;
			
			imageCount++;
		}

//		m_list.ScrollListTo( 0.0f);
		m_list.UpdateCamera();
		
//		curPage = 0;
		SelectPage(0);

		SetCount();
		
		InvokeRepeating( "Roll", 5.0f, 5.0f);
	}
	
	private void Roll()
	{
		curPage++;
		
		if( imageCount <= curPage)
		{
			curPage = 0;
			m_list.ScrollToItem( curPage, 0.0f);
		}
		else
		{
			m_list.ScrollToItem( curPage, 1.0f, EZAnimation.EASING_TYPE.Linear);
		}
		
		SelectPage( curPage);
	}
	
	public void SetCount()
	{
		for( int i = 0; i < m_Counts.Length; ++i)
		{
			m_Counts[i].gameObject.SetActiveRecursively( false);
		}
		
		for( int i = 0; i < imageCount; ++i)
		{
			m_Counts[i].gameObject.SetActiveRecursively( true);
		}
	}
	
	void SelectPage( int nPageNow)
	{		
		m_SelectPage.transform.position = m_Counts[ nPageNow].transform.position;
		curPage = nPageNow;
		
		if( 0 == curPage)
		{
			btnPrev.Hide( true);
		}
		else
		{
			if( true == btnPrev.IsHidden())
				btnPrev.Hide( false);
		}
		
		if( imageCount - 1 == curPage)
		{
			btnNext.Hide( true);
		}
		else
		{
			if( true == btnNext.IsHidden())
				btnNext.Hide( false);
		}
	}
	
    void ChangedItemDelegate( IUIObject obj)
    {
	    UIListItem data = (UIListItem)obj;
        SelectPage( data.Index);
    }
		
	private void DayCloseBtnDelegate( ref POINTER_INFO ptr)
	{		
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			PlayerPrefs.SetInt( "EventView", System.DateTime.Now.Day);
			DestroyWindow();
		}
	}
	
	private void OnPrevBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( 0 < curPage)
			{
				curPage--;
				m_list.ScrollToItem( curPage, 0.0f);
				
				SelectPage( curPage);
			}
		}
	}
	
	private void OnNextBtn( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( imageCount - 1 > curPage)
			{
				curPage++;
				m_list.ScrollToItem( curPage, 0.0f);
				
				SelectPage( curPage);
			}
		}
	}
}
