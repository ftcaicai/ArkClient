using UnityEngine;
using System.Collections;

public delegate void SetReceiverDelegate( string str);

public class AsPostReceiverCandidateDlg : MonoBehaviour
{
	[SerializeField] UIRadioBtn friendRadio = null;
	[SerializeField] UIRadioBtn guildRadio = null;
	[SerializeField] UIButton nameBtn = null;
	[SerializeField] UIButton levelBtn = null;
	[SerializeField] UIButton classBtn = null;
	[SerializeField] UIScrollList list = null;
	[SerializeField] GameObject listItem = null;
	[SerializeField] UIButton prevBtn = null;
	[SerializeField] SpriteText page = null;
	[SerializeField] UIButton nextBtn = null;
	[SerializeField] UIRadioBtn online = null;
	private bool isFriendTab = true;
	private int curPage = 0;
	private int maxPage = 0;
	private bool isOnline = false;
	private SetReceiverDelegate setReceiverDelegate = null;
	public SetReceiverDelegate SetReceiverDelegate	{ set { setReceiverDelegate = value; } }

	// Use this for initialization
	void Start()
	{
		friendRadio.Text = AsTableManager.Instance.GetTbl_String(1181);
		guildRadio.Text = AsTableManager.Instance.GetTbl_String(1240);
		nameBtn.Text = AsTableManager.Instance.GetTbl_String(1168);
		levelBtn.Text = AsTableManager.Instance.GetTbl_String(1249);
		classBtn.Text = AsTableManager.Instance.GetTbl_String(1250);
		online.Text = AsTableManager.Instance.GetTbl_String(1251);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void Init()
	{
		isFriendTab = true;
		curPage = 0;
		friendRadio.Value = isFriendTab;
		guildRadio.Value = !isFriendTab;
		isOnline = false;
		online.Value = isOnline;
	}

	void OnEnable()
	{
		Init();
	}

	public void Show()
	{
		gameObject.SetActiveRecursively( true);
		Init();
	}

	public void Hide()
	{
		gameObject.SetActiveRecursively( false);
	}

	void OnFriendTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == isFriendTab)
			return;

		curPage = 0;

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_FRIEND, (byte)curPage, isOnline);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);

		isFriendTab = true;
		friendRadio.Value = isFriendTab;
		guildRadio.Value = !isFriendTab;
	}

	void OnGuildTab()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( false == isFriendTab)
			return;

		curPage = 0;

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_GUILD, (byte)curPage, isOnline);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);

		isFriendTab = false;
		friendRadio.Value = isFriendTab;
		guildRadio.Value = !isFriendTab;
	}

	void OnPrevPageBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		curPage--;

		if( 0 > curPage)
		{
			curPage = 0;
			return;
		}

		ePOST_ADDRESS_BOOK_TYPE type = ( true == isFriendTab) ? ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_FRIEND : ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_GUILD;

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( type, (byte)curPage, isOnline);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	void OnNextPageBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		curPage++;

		if( maxPage <= curPage)
		{
			curPage = (byte)( maxPage - 1);
			return;
		}

		ePOST_ADDRESS_BOOK_TYPE type = ( true == isFriendTab) ? ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_FRIEND : ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_GUILD;

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( type, (byte)curPage, isOnline);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	void OnSelect()
	{
		UIListButton curSel = list.LastClickedControl as UIListButton;
		AsSearchedReceiverItem listItem = curSel.gameObject.GetComponent<AsSearchedReceiverItem>();
		setReceiverDelegate( listItem.Name);

		Hide();
	}

	void OnOnlineFilterBtn()
	{
		isOnline = !isOnline;
		online.Value = isOnline;
		curPage = 0;

		ePOST_ADDRESS_BOOK_TYPE type = ( true == isFriendTab) ? ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_FRIEND : ePOST_ADDRESS_BOOK_TYPE.ePOST_ADDRESS_BOOK_TYPE_GUILD;

		body_CS_POST_ADDRESS_BOOK addressBook = new body_CS_POST_ADDRESS_BOOK( type, (byte)curPage, isOnline);
		byte[] packet = addressBook.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	public void InsertAddress( body1_SC_POST_ADDRESS_BOOK addresses)
	{
		maxPage = addresses.nMaxPage;
		UpdatePageText();

		list.ClearList( true);

		foreach( body2_SC_POST_ADDRESS_BOOK address in addresses.body)
		{
			if( 0 == string.Compare( AsUserInfo.Instance.SavedCharStat.charName_, address.szCharName))
				continue;

			UIListItem item = list.CreateItem( listItem) as UIListItem;

			AsSearchedReceiverItem searchedItem = item.GetComponent<AsSearchedReceiverItem>();
			searchedItem.Init( address);
		}
	}

	public void ClearAddress()
	{
		maxPage = 1;
		UpdatePageText();
		list.ClearList( true);
	}

	private void UpdatePageText()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendFormat( "{0}/{1}", curPage + 1, maxPage);
		page.Text = sb.ToString();
	}
}
