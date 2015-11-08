using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AsDelegateImageDlg : MonoBehaviour
{
	[SerializeField]UIScrollList list = null;
	[SerializeField]GameObject listItem = null;
	[SerializeField]GameObject leftArrow = null;
	[SerializeField]GameObject rightArrow = null;
	[SerializeField]UIPanelTab generalTab = null;
	[SerializeField]UIPanelTab rareTab = null;
	[SerializeField]UIButton clearBtn = null;
	[SerializeField]UIButton assignBtn = null;
	private int maxColumn = 6;
	private int curTab = 0;
	
	public void Init()
	{
		if( 0 == curTab)
			InitGeneralTab();
		else
			InitRareTab();
	}
	
	public void UnlockDelegateImage( Int32 id)
	{
		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer container = list.GetItem(i) as UIListItemContainer;
			AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
			dlgItem.UnlockDelegateImage( id);
		}
	}
	
	private void ClearSelectMark()
	{
		for( int i = 0; i < list.Count; i++)
		{
			UIListItemContainer container = list.GetItem(i) as UIListItemContainer;
			AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
			dlgItem.ClearSelectMark();
		}
	}
	
	private void InitGeneralTab()
	{
		int column = 0;
		int row = 0;
		
		list.ClearList( true);
		
		Dictionary<int,DelegateImageData> dicGeneralDelegates = AsDelegateImageManager.Instance.GeneralDelegates;
		
		if( 18 < dicGeneralDelegates.Count)
		{
			float totalRow = dicGeneralDelegates.Count / 3.0f;
			maxColumn = Mathf.CeilToInt( totalRow);
		}
		
		foreach( KeyValuePair<int,DelegateImageData> pair in dicGeneralDelegates)
		{
			if( 0 == row)
			{
				UIListItemContainer container = list.CreateItem( listItem) as UIListItemContainer;
				AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
				dlgItem.parentDlg = this;
				dlgItem.data[ row] = pair.Value;
				dlgItem.Init();
			}
			else
			{
				UIListItemContainer container = list.GetItem( column) as UIListItemContainer;
				AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
				dlgItem.parentDlg = this;
				dlgItem.data[ row] = pair.Value;
				dlgItem.Init();
			}
			
			column++;
			
			if( maxColumn <= column)
			{
				column = 0;
				row++;
			}
		}

		rareTab.spriteText.Color = Color.black;
		generalTab.spriteText.Color = Color.white;
	}
	
	private void InitRareTab()
	{
		int column = 0;
		int row = 0;
		
		list.ClearList( true);
		
		Dictionary<int,DelegateImageData> dicRarelDelegates = AsDelegateImageManager.Instance.RareDelegates;
		
		if( 18 < dicRarelDelegates.Count)
		{
			float totalRow = dicRarelDelegates.Count / 3.0f;
			maxColumn = Mathf.CeilToInt( totalRow);
		}
		
		foreach( KeyValuePair<int,DelegateImageData> pair in dicRarelDelegates)
		{
			if( 0 == row)
			{
				UIListItemContainer container = list.CreateItem( listItem) as UIListItemContainer;
				AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
				dlgItem.parentDlg = this;
				dlgItem.data[ row] = pair.Value;
				dlgItem.Init();
			}
			else
			{
				UIListItemContainer container = list.GetItem( column) as UIListItemContainer;
				AsDelegateImageDlgItem dlgItem = container.gameObject.GetComponent<AsDelegateImageDlgItem>();
				dlgItem.parentDlg = this;
				dlgItem.data[ row] = pair.Value;
				dlgItem.Init();
			}
			
			column++;
			
			if( maxColumn <= column)
			{
				column = 0;
				row++;
			}
		}

		rareTab.spriteText.Color = Color.white;
		generalTab.spriteText.Color = Color.black;
	}
	
	// Use this for initialization
	void Start()
	{
		generalTab.Text = AsTableManager.Instance.GetTbl_String(1174);
		rareTab.Text = AsTableManager.Instance.GetTbl_String(4074);
		clearBtn.Text = AsTableManager.Instance.GetTbl_String(1356);
		assignBtn.Text = AsTableManager.Instance.GetTbl_String(4076);
		
		Init();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( 7 >= list.Count)
		{
			leftArrow.SetActiveRecursively( false);
			rightArrow.SetActiveRecursively( false);
			return;
		}
		
		if( 0.0f >= list.ScrollPosition)
			leftArrow.SetActiveRecursively( false);
		else if( false == leftArrow.active)
			leftArrow.SetActiveRecursively( true);
		
		if( 1.0f <= list.ScrollPosition)
			rightArrow.SetActiveRecursively( false);
		else if( false == rightArrow.active)
			rightArrow.SetActiveRecursively( true);
	}
	
	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		Destroy( gameObject.transform.parent.gameObject);
		
		if( null != AsDelegateImageDlgItem.confirmDlg)
			Destroy( AsDelegateImageDlgItem.confirmDlg);

		if( null != AsDelegateImageDlgItem.statDlg)
			Destroy( AsDelegateImageDlgItem.statDlg);
		
		AsDelegateImageManager.Instance.SelectedImageID = -1;
	}
	
	public void Close()
	{
		Destroy( gameObject.transform.parent.gameObject);
		
		if( null != AsDelegateImageDlgItem.confirmDlg)
			Destroy( AsDelegateImageDlgItem.confirmDlg);
		
		if( null != AsDelegateImageDlgItem.statDlg)
			Destroy( AsDelegateImageDlgItem.statDlg);
		
		AsDelegateImageManager.Instance.SelectedImageID = -1;
	}
	
	void OnTabGeneral()
	{
		if( 0 == curTab)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		InitGeneralTab();
		
		curTab = 0;
	}
	
	void OnTabRare()
	{
		if( 1 == curTab)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6005_EFF_NextPage", Vector3.zero, false);
		
		InitRareTab();
		
		curTab = 1;
	}
	
	void OnClearBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 >= AsDelegateImageManager.Instance.AssignedImageID)
			return;
		
		body_CS_IMAGE_SET imageSet = new body_CS_IMAGE_SET(0);
		byte[] data = imageSet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	void OnAssignBtn()
	{
		DelegateImageData imageData = AsDelegateImageManager.Instance.GetSelectDelegateImage();
		if( null == imageData)
			return;
		
		if( true == imageData.locked)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( 0 >= AsDelegateImageManager.Instance.SelectedImageID)
			return;
		
		body_CS_IMAGE_SET imageSet = new body_CS_IMAGE_SET( imageData.id);
		byte[] data = imageSet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
}
