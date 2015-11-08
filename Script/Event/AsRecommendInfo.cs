using UnityEngine;
using System;
using System.Collections;

public class AsRecommendInfo : MonoBehaviour
{
	public UIButton m_BtnClose = null;
	public UIButton m_BtnCount = null;
	public UIButton m_BtnItemIndex = null;
	public UIButton m_BtnReceive = null;
	public SpriteText m_TextTitle = null;
	public SpriteText m_TextEventTime = null;
	public SpriteText m_TextCount = null;
	public UIScrollList m_list = null;
	public GameObject m_itemPrefab = null;
	
	public void Close()
	{
		DestroyWindow();
	}
	
	void DestroyWindow()
	{
		AsHudDlgMgr.Instance.recommendInfoDlg = null;
		Destroy(gameObject);
	}
	
	/*Old Recommand
	// Use this for initialization
	void Start () {
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_BtnCount.spriteText);		
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_BtnItemIndex.spriteText);	
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_BtnReceive.spriteText);	
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextTitle);		
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextEventTime);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(m_TextCount);
		
		m_BtnCount.Text = AsTableManager.Instance.GetTbl_String( 1591);
		m_BtnItemIndex.Text = AsTableManager.Instance.GetTbl_String( 1592);
		m_BtnReceive.Text = AsTableManager.Instance.GetTbl_String( 1593);
		
		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 1597);		
	
		
		m_BtnClose.SetInputDelegate( CloseBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			DestroyWindow();
		}
	}
	

	
	public void SetRecommendInfo(body1_SC_RECOMMEND_INFO list)
	{
		
		m_TextEventTime.Text = "";
		m_TextCount.Text = "";
		m_TextEventTime.Text =  string.Format( AsTableManager.Instance.GetTbl_String( 1589), GetTime (list.tRecommendTerm));	
		m_TextCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1590), list.nRecommendCount);	
		
		m_list.ClearList( true);
		if(list.body == null) return;
		int index = 1;
		foreach( body2_SC_RECOMMEND_INFO data in list.body )
		{
			UIListItemContainer item = m_list.CreateItem( m_itemPrefab) as UIListItemContainer;
			
			AsRecommendListItem listItem = item.gameObject.GetComponent<AsRecommendListItem>();
			listItem.SetData(data, list.nRecommendCount, index);	
			index++;
			
		}
		m_list.ScrollListTo( 0.0f);
	}
	
	private string GetTime(long nTime)
	{		
		System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
		dt = dt.AddSeconds(nTime);
		return string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(dt.ToString())); 
	}
	*/
}
