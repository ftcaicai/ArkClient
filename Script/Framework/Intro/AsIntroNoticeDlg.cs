using UnityEngine;
using System.Collections;

public delegate void CloseDelegate();

public class AsIntroNoticeDlg : MonoBehaviour
{
	[SerializeField]private SpriteText title = null;
	[SerializeField]private UIScrollList list = null;
	[SerializeField]private UIButton detailBtn = null;
	[SerializeField]private SimpleSprite[] dots = new SimpleSprite[0];
	[SerializeField]private SimpleSprite curDot = null;
	[SerializeField]private GameObject item = null;
	private CloseDelegate closeDelegate = null;
	public CloseDelegate CloseCallback
	{
		set { closeDelegate = value; }
	}
	private int selIndex = 0;

	void Awake()
	{
		foreach( SimpleSprite dot in dots)
			dot.gameObject.SetActiveRecursively( false);
	}
	
	// Use this for initialization
	void Start()
	{
		title.Text = AssetbundleManager.Instance.GetPatcherString(58);
		detailBtn.Text = AssetbundleManager.Instance.GetPatcherString(59);
		
		list.AddItemSnappedDelegate( ItemChanged);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void InsertNotice( string notice, string detailInfo)
	{
		IUIListObject obj = list.CreateItem( item, 0, notice);
		AsIntroNoticeDlgItem dlgItem = obj.gameObject.GetComponent<AsIntroNoticeDlgItem>();
		Debug.Assert( null != dlgItem);
		dlgItem.data = detailInfo;
		if( string.Empty == detailInfo)
			detailBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		else
			detailBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		
		Debug.Log( "obj.Index : " + obj.Index);
		dots[ list.Count - 1].gameObject.SetActiveRecursively( true);
	}
	
	private void OnCloseBtn()
	{
		closeDelegate();
		GameObject.Destroy( gameObject);
	}
	
	private void OnDetailBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		IUIListObject listObj = list.GetItem( selIndex);
		AsIntroNoticeDlgItem dlgItem = listObj.gameObject.GetComponent<AsIntroNoticeDlgItem>();
		Debug.Assert( null != dlgItem);
		if( string.Empty == dlgItem.data)
			return;
		
		Application.OpenURL( dlgItem.data);
	}
	
	private void ItemChanged( IUIObject obj)
	{
		IUIListObject data = (IUIListObject)obj;
		curDot.transform.position = dots[ data.Index].transform.position;
		Debug.Log( "Item index : " + data.Index);
		selIndex = data.Index;
		
		AsIntroNoticeDlgItem dlgItem = data.gameObject.GetComponent<AsIntroNoticeDlgItem>();
		Debug.Assert( null != dlgItem);
		if( string.Empty == dlgItem.data)
			detailBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		else
			detailBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
	}
}
