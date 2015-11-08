using UnityEngine;
using System.Collections;
using System.Text;

public class AsChannelListFramework : MonoBehaviour
{
	[SerializeField] UIScrollList listLeft = null;
	[SerializeField] UIScrollList listRight = null;
	[SerializeField] GameObject item = null;
	private UIRadioBtn prevSelect = null;
	private float prevClickTime = 0.0f;

	// Use this for initialization
	void Start()
	{
		StringBuilder sb = new StringBuilder();

		int i = 0;
		int count = AsChannelListData.Instance.Count;
		for( i = 0; i < count; i++)
		{
			body2_GC_CHANNEL_LIST channel = AsChannelListData.Instance.GetData(i);
			if( null == channel)
				continue;

			sb.Remove( 0, sb.Length);
			if( ( 0 == channel.nMinLevel) && ( 0 == channel.nMaxLevel))
				sb.Append( channel.szChannelName);
			else
				sb.AppendFormat( "{0} (Lv {1} ~ {2})", channel.szChannelName, channel.nMinLevel, channel.nMaxLevel);

			switch( channel.eConfusion)
			{
			case eCONFUSIONTYPE.eCONFUSION_NORMAL:
				sb.AppendFormat( " RGBA(0.0, 1.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1096));
				break;
			case eCONFUSIONTYPE.eCONFUSION_BUSY:
				sb.AppendFormat( " RGBA(1.0, 1.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1098));
				break;
			case eCONFUSIONTYPE.eCONFUSION_SATURATION:
				sb.AppendFormat( " RGBA(1.0, 0.0, 0.0, 1.0)[{0}]", AsTableManager.Instance.GetTbl_String(1097));
				break;
			default:
				Debug.Log( "Channel confusion : " + channel.eConfusion);
				break;
			}

			UIScrollList list = ( 0 == ( i & 0x01)) ? listLeft : listRight;

			UIListItemContainer itemContainer = list.CreateItem( item) as UIListItemContainer;
			AsChannelListBtn listBtn = itemContainer.gameObject.GetComponent<AsChannelListBtn>();
			listBtn.channelData = channel;

			// ilmeda, 20120821
			AsLanguageManager.Instance.SetFontFromSystemLanguage( itemContainer.spriteText);
			itemContainer.Text = sb.ToString();

			if( eCONFUSIONTYPE.eCONFUSION_INSPECTION == channel.eConfusion)
			{
				itemContainer.spriteText.Color = Color.gray;

				UIRadioBtn btn = itemContainer.gameObject.GetComponentInChildren<UIRadioBtn>();
				if( null != btn)
					btn.controlIsEnabled = false;
			}
		}

		if( 0 != ( i & 0x01))
		{
			UIListItemContainer itemContainer = listRight.CreateItem( item) as UIListItemContainer;
			AsChannelListBtn listBtn = itemContainer.gameObject.GetComponent<AsChannelListBtn>();
			listBtn.channelData = null;
			itemContainer.Text = "";
		}

		AsLoadingIndigator.Instance.HideIndigator();
	}

	// Update is called once per frame
	void Update()
	{
		if( true == listLeft.IsScrolling)
			listRight.ScrollPosition = listLeft.ScrollPosition;
		else if( true == listRight.IsScrolling)
			listLeft.ScrollPosition = listRight.ScrollPosition;
	}

	private void OnSelectLeft()
	{
		UIRadioBtn clickItem = listLeft.LastClickedControl as UIRadioBtn;
		UIListItemContainer itemContainer = clickItem.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsChannelListBtn listBtn = itemContainer.gameObject.GetComponent<AsChannelListBtn>();

		if( eCONFUSIONTYPE.eCONFUSION_INSPECTION != listBtn.channelData.eConfusion)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( null != prevSelect)
				prevSelect.Value = false;

			clickItem.Value = true;

			float curClickTime = Time.time;
			if( 0.4f >= ( curClickTime - prevClickTime) && ( prevSelect == clickItem))
			{
				listBtn.EnterChannel();
			}

			prevSelect = clickItem;
			prevClickTime = curClickTime;
		}
	}

	private void OnSelectRight()
	{
		UIRadioBtn clickItem = listRight.LastClickedControl as UIRadioBtn;
		UIListItemContainer itemContainer = clickItem.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsChannelListBtn listBtn = itemContainer.gameObject.GetComponent<AsChannelListBtn>();

		if( eCONFUSIONTYPE.eCONFUSION_INSPECTION != listBtn.channelData.eConfusion)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( null != prevSelect)
				prevSelect.Value = false;

			clickItem.Value = true;

			float curClickTime = Time.time;
			if( 0.4f >= ( curClickTime - prevClickTime) && ( prevSelect == clickItem))
				listBtn.EnterChannel();

			prevSelect = clickItem;
			prevClickTime = curClickTime;
		}
	}

	private void OnBackBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AS_CG_RETURN_CHARSELECT returnCharSelect = new AS_CG_RETURN_CHARSELECT();
		byte[] data = returnCharSelect.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}

	private void OnNextBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( null == prevSelect)
			return;

		UIListItemContainer itemContainer = prevSelect.transform.parent.gameObject.GetComponent<UIListItemContainer>();
		AsChannelListBtn listBtn = itemContainer.gameObject.GetComponent<AsChannelListBtn>();
		listBtn.EnterChannel();
	}
}
