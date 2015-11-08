using UnityEngine;
using System.Collections;

public class AsExpireItemView : MonoBehaviour 
{
	
	public UIButton btnOk;
	public UIButton btnClose;
	public SpriteText textTitle;
	public UIScrollList listItems;
	public SpriteText textContent;
	
	public GameObject goResItem;
	
	public void Open( body1_SC_ITEM_TIME_EXPIRE dataRoot )
	{
		if( null == dataRoot )
		{
			Debug.LogError("AsExpireItemView::Open()[ body1_SC_ITEM_TIME_EXPIRE == datas ]");
			return;
		}
		
		listItems.ClearList( true);
		
		AddList(dataRoot);
		
		listItems.ScrollToItem( 0, 0.0f);
	}
	
	public void AddList( body1_SC_ITEM_TIME_EXPIRE dataRoot )
	{
		foreach( sITEM _data in dataRoot.datas )
		{
			if( null == goResItem )
			{
				Debug.LogError("AsExpireItemView::Open()[ null == goResItem ] ");
				break;
			}
			UIListItem item = listItems.CreateItem( goResItem ) as UIListItem;		
			AsExpireItem _script  = item.gameObject.GetComponent<AsExpireItem>();
			if( null == _script )
			{
				Debug.LogError("AsExpireItemView::Open()[ null == AsExpireItem ] " );
				continue;
			}
			_script.Open(_data);
		}
		
		listItems.ScrollToItem( 0, 0.0f);
	}
	
	
	public void Close()
	{
		
	}

	
	void Awake()
	{
		if( null != btnOk.spriteText )
		{
			btnOk.spriteText.Text = AsTableManager.Instance.GetTbl_String(1152);
		}
		
		if( null != textTitle )
		{
			textTitle.Text = AsTableManager.Instance.GetTbl_String(126);
		}
		
		if( null != textContent )
		{
			textContent.Text = AsTableManager.Instance.GetTbl_String(1638);
		}
		
		btnOk.SetInputDelegate(CloseBtnDelegate);
		btnClose.SetInputDelegate(CloseBtnDelegate);
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{					
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
			AsHudDlgMgr.Instance.CloseExpireItemViewDlg(); 
		}
	}
	
	
	
}
