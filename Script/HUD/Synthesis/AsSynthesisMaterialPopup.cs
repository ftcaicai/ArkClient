using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum eSynthesisMaterialAskMode
{
	Grade,
	AlreadyProgress,
};

public class AsSynthesisMaterialPopup : MonoBehaviour 
{
	[SerializeField] SpriteText 		txtTitle;

	[SerializeField] SimpleSprite 	spriteItem1;
	[SerializeField] SimpleSprite 	spriteItem2;
	[SerializeField] SimpleSprite 	spriteItem3;

	[SerializeField] SpriteText 		txtInfo;

	[SerializeField] UIButton 	btnOK;
	[SerializeField] UIButton 	btnCancel;

	AsSynthesisCosDlg 			cosSynthesisDlg;

	eSynthesisMaterialAskMode	askMode = eSynthesisMaterialAskMode.Grade;

	float									fDistItem = 0.0f;
	Vector3								vCenterItemPos;
	List<SimpleSprite>				listAlignItem = new List<SimpleSprite>();
	List<SimpleSprite>				listSimpleSprite = new List<SimpleSprite>();

	void Awake()
	{
		fDistItem = spriteItem2.transform.localPosition.x - spriteItem1.transform.localPosition.x;
		vCenterItemPos = new Vector3( spriteItem2.transform.localPosition.x , spriteItem2.transform.localPosition.y , spriteItem2.transform.localPosition.z );

		spriteItem1.name = "empty";
		spriteItem2.name = "empty";
		spriteItem3.name = "empty";
		
		listSimpleSprite.Add (spriteItem1);
		listSimpleSprite.Add (spriteItem2);
		listSimpleSprite.Add (spriteItem3);
		
		foreach (SimpleSprite sp in listSimpleSprite) 
		{
			sp.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
		}
	}

	// Use this for initialization
	void Start () 
	{
		txtTitle.Text = AsTableManager.Instance.GetTbl_String (126);

		btnOK.spriteText.Text = AsTableManager.Instance.GetTbl_String (1152);
		btnOK.SetInputDelegate (OKBtnDelegate);

		btnCancel.spriteText.Text = AsTableManager.Instance.GetTbl_String (1151);
		btnCancel.SetInputDelegate (CancelBtnDelegate);
	}

	private void AddAlignList( eSynthesisMaterialAskMode _mode , int _slotIndex )
	{
		Vector2 minusItemSize = Vector2.zero;
		InvenSlot[]	slots = ItemMgr.HadItemManagement.Inven.invenSlots;

		if (_slotIndex <= 0 || _slotIndex >= slots.Length)
			return;

		Item _item = null;
		UISlotItem _SlotItem = null;

		InvenSlot	_invenSlot = slots[_slotIndex];
		_item = ItemMgr.ItemManagement.GetItem( _invenSlot.realItem.item.ItemID );


		if (askMode == eSynthesisMaterialAskMode.Grade) 
		{
			if( _item.ItemData.grade < Item.eGRADE.Epic )
				return;
		} 
		else if (askMode == eSynthesisMaterialAskMode.AlreadyProgress) 
		{
			if( _invenSlot.realItem.sItem.nAccreCount <= 0 )
				return;
		}


		SimpleSprite _sp = GetEmptySimpleSprite ();

		if (_sp == null)
			return;

		_SlotItem = ResourceLoad.CreateItemIcon(_item.GetIcon(), _sp, Vector3.back, minusItemSize, false);

		_sp.name = "filled";

		listAlignItem.Add (_sp);
	}

	private SimpleSprite	GetEmptySimpleSprite()
	{
		foreach (SimpleSprite sp in listSimpleSprite) 
		{
			if( sp.name == "empty" )
				return sp;
		}
		return null;
	}

	private void AlignItem()
	{
		float 	fStartPos = 0;
		float fReviseValue = 0;

		if (listAlignItem.Count % 2 != 0)
			fReviseValue = fDistItem / 2.0f;

		fStartPos = vCenterItemPos.x - ((float)(listAlignItem.Count/2) * fDistItem) - fReviseValue + fDistItem/2.0f;

		foreach (SimpleSprite sp in listAlignItem) 
		{
			sp.transform.localPosition = new Vector3( fStartPos , vCenterItemPos.y , vCenterItemPos.z );

			fStartPos += fDistItem;
		}
	}

	public void Open( AsSynthesisCosDlg _dlg , eSynthesisMaterialAskMode _mode , int _slot1 , int _slot2 , int _slot3 )
	{
		cosSynthesisDlg = _dlg;

		askMode = _mode;

		if (askMode == eSynthesisMaterialAskMode.Grade) 
		{
			txtInfo.Text = AsTableManager.Instance.GetTbl_String (2415);
		} 
		else if (askMode == eSynthesisMaterialAskMode.AlreadyProgress) 
		{
			txtInfo.Text = AsTableManager.Instance.GetTbl_String (2416);
		}

		AddAlignList( _mode , _slot1 );
		AddAlignList( _mode , _slot2 );
		AddAlignList( _mode , _slot3 );

		AlignItem ();
	}

	private void OKBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( askMode == eSynthesisMaterialAskMode.Grade )
				cosSynthesisDlg.AskAlreadyProgress();
			else if( askMode == eSynthesisMaterialAskMode.AlreadyProgress )
				cosSynthesisDlg.ExcuteSynthesis();

			Destroy( transform.parent.gameObject );
		}
	}

	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			AsSynthesisCosDlg.isAuthorityButtonClick = true;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			Destroy( transform.parent.gameObject );
		}
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
