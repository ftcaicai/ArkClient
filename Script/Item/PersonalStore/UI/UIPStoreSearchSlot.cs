using UnityEngine;
using System.Collections;
using System.Globalization;

public class UIPStoreSearchSlot : MonoBehaviour
{
	#region - member -
	[SerializeField] UIListItemContainer container;
	[SerializeField] UIListButton listBtn; public UIListButton ListBtn{get{return listBtn;}}
	
	[SerializeField] GameObject objSlot;
	[SerializeField] SpriteText txt_Grade;
	[SerializeField] SpriteText txt_Level;
	[SerializeField] SpriteText txt_Name; public string TextName{get{return txt_Name.Text;}}
	string m_SimpleName; public string SimpleName{get{return m_SimpleName;}}
	[SerializeField] SpriteText txt_Price;
	
	SpriteText count_;
	
	body2_SC_PRIVATESHOP_SEARCH_RESULT m_SearchInfo; public body2_SC_PRIVATESHOP_SEARCH_RESULT SearchInfo{get{return m_SearchInfo;}}
	
	delListBtnClicked m_Del;
	
	static bool s_TooltipOpened = false;
	#endregion

	#region - init & update -
	void Awake()
	{
	}

	public void Init( UIScrollList _list, body2_SC_PRIVATESHOP_SEARCH_RESULT _item, delListBtnClicked _del)
	{
		m_SearchInfo = _item;
		m_Del = _del;
		
		listBtn.SetInputDelegate( OnListButton);
		_list.AddItem( container);
		
		Item item = ItemMgr.ItemManagement.GetItem( _item.sItem.nItemTableIdx);
		GameObject objIcon = item.GetIcon();
		objIcon = Instantiate( objIcon) as GameObject;
		objIcon.transform.parent = objSlot.transform;
		objIcon.transform.localPosition = Vector3.zero;
		
		count_ = objIcon.GetComponentInChildren<SpriteText>();
		if( count_ != null && m_SearchInfo.sItem.nOverlapped > 1)
			count_.Text = m_SearchInfo.sItem.nOverlapped.ToString();
		
		// item number must be added
		
		txt_Grade.Text = item.GetStrGrade();
		txt_Level.Text = item.ItemData.levelLimit.ToString();
		
		string str = m_SimpleName = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
		if( m_SearchInfo.sItem.nStrengthenCount > 0)
			str = Color.white + "+" + m_SearchInfo.sItem.nStrengthenCount + " " + m_SimpleName;
		txt_Name.Text = str;

//		txt_Price.Text = _item.nItemGold.ToString();
		txt_Price.Text = _item.nItemGold.ToString("#,#0", CultureInfo.InvariantCulture);
		
		container.ScanChildren();
		container.GetScrollList().ClipItems();
	}
	
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Grade);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Level);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Name);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txt_Price);
	}
	
	void Update()
	{
	}

	void OnEnable()
	{
	}
	#endregion
	
	#region - delegate -
	void OnListButton( ref POINTER_INFO _ptr)
	{
		if( _ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
//			if( s_TooltipOpened == false)
//			{
				Debug.Log("item is clicked");
				
				m_Del( this);
//			}
		}
	}
	#endregion
	#region - public -
	public static void TooltipActivate( bool _active)
	{
//		s_TooltipOpened = _active;
	}
	
	public void BuyProc( body_SC_PRIVATESHOP_ITEMBUY _buy)
	{
		Item item = ItemMgr.ItemManagement.GetItem( m_SearchInfo.sItem.nItemTableIdx);
		string name = AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
		string header = AsTableManager.Instance.GetTbl_String(1145);
		string body = string.Format(AsTableManager.Instance.GetTbl_String(376), name);
		AsNotify.Instance.MessageBox(header, body);
		
		m_SearchInfo.sItem.nOverlapped -= _buy.nItemCount;
		if( count_ != null)
		{
			if( m_SearchInfo.sItem.nOverlapped > 1)
				count_.Text = m_SearchInfo.sItem.nOverlapped.ToString();
			else if( m_SearchInfo.sItem.nOverlapped == 1)
				count_.Text = "";
		}
		
//		if( m_SearchInfo.sItem.nOverlapped < 1)
//			container.GetScrollList().RemoveItem( container, true);
	}
	#endregion
}

