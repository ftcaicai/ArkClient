using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum eDelegateImageType : int
{
	None = -1,
	
	Normal,
	Rare,
	
	Max
};

public enum eDelegateImageSubType : int
{
	None = -1,
	
	Gold,
	Miracle,
	Level,
	Quest,
	Designation,
	Item,
	
	Max
};

public class DelegateImageData : AsTableRecord
{
	public int id = -1;
	public eDelegateImageType eType = eDelegateImageType.None;
	public eDelegateImageSubType eSubType = eDelegateImageSubType.None;
	public int unlockCost = -1;
	public string iconName = string.Empty;
	public bool locked = true;
	public int idDescription = -1;
	public int idEffect = -1;
	public int idEffectDescription = -1;
	
	public DelegateImageData( XmlElement _element)
	{
		try
		{
			XmlNode node = (XmlElement)_element;
			
			SetValue( ref id, node, "Portrait_ID");
			SetValue( ref eType, node, "Portrait_Type");
			SetValue( ref eSubType, node, "Portrait_Kind");
			SetValue( ref unlockCost, node, "Portrait_Cost");
			SetValue( ref iconName, node, "Portrait_Icon");
			SetValue( ref idDescription, node, "Portrait_Description");
			SetValue( ref idEffect, node, "Portrait_Effect");
			SetValue( ref idEffectDescription, node, "Portrait_Effect_Description");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsDelegateImageManager
{
	private Dictionary<int,DelegateImageData> dicGeneralDelegate = new Dictionary<int,DelegateImageData>();
	public Dictionary<int,DelegateImageData> GeneralDelegates
	{
		get	{ return dicGeneralDelegate; }
	}
	private Dictionary<int,DelegateImageData> dicRareDelegate = new Dictionary<int,DelegateImageData>();
	public Dictionary<int,DelegateImageData> RareDelegates
	{
		get	{ return dicRareDelegate; }
	}
	
	private int assignedImageID = -1;
	public int AssignedImageID
	{
		set	{ assignedImageID = value; }
		get { return assignedImageID; }
	}
	private int selectedImageID = -1;
	public int SelectedImageID
	{
		set	{ selectedImageID = value; }
		get	{ return selectedImageID; }
	}
	
	static private AsDelegateImageManager instance = null;
	static public AsDelegateImageManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsDelegateImageManager();
			
			return instance;
		}
	}
	
	private AsDelegateImageManager()
	{
	}
	
	public void Init()
	{
		assignedImageID = -1;
		selectedImageID = -1;
		
		foreach( KeyValuePair<int,DelegateImageData> data in dicGeneralDelegate)
		{
			data.Value.locked = ( eDelegateImageSubType.None != data.Value.eSubType) ? true : false;
		}
		
		foreach( KeyValuePair<int,DelegateImageData> data in dicRareDelegate)
		{
			data.Value.locked = true;
		}
	}
	
	public DelegateImageData GetSelectDelegateImage()
	{
		if( -1 == selectedImageID)
			return null;
		
		if( true == dicGeneralDelegate.ContainsKey( selectedImageID))
			return dicGeneralDelegate[ selectedImageID];
		
		if( true == dicRareDelegate.ContainsKey( selectedImageID))
			return dicRareDelegate[ selectedImageID];
		
		return null;
	}
	
	public DelegateImageData GetAssignedDelegateImage()
	{
		if( -1 == assignedImageID)
			return null;
		
		if( true == dicGeneralDelegate.ContainsKey( assignedImageID))
			return dicGeneralDelegate[ assignedImageID];
		
		if( true == dicRareDelegate.ContainsKey( assignedImageID))
			return dicRareDelegate[ assignedImageID];
		
		return null;
	}
	
	public DelegateImageData GetDelegateImage( int id)
	{
		if( true == dicGeneralDelegate.ContainsKey( id))
			return dicGeneralDelegate[ id];
		
		if( true == dicRareDelegate.ContainsKey( id))
			return dicRareDelegate[ id];
		
		return null;
	}

	public void SetUnlockState( int id, bool state)
	{
		if( true == dicGeneralDelegate.ContainsKey( id))
			dicGeneralDelegate[id].locked = state;
		
		if( true == dicRareDelegate.ContainsKey( id))
			dicRareDelegate[id].locked = state;
	}

	public bool UnlockBySubType( eDelegateImageSubType type, int value)
	{
		foreach( KeyValuePair<int,DelegateImageData> data in dicGeneralDelegate)
		{
			if( type == data.Value.eSubType)
			{
				if( value == data.Value.unlockCost)
				{
					data.Value.locked = false;
					return true;
				}
			}
		}

		foreach( KeyValuePair<int,DelegateImageData> data in dicRareDelegate)
		{
			if( type == data.Value.eSubType)
			{
				if( value == data.Value.unlockCost)
				{
					data.Value.locked = false;
					return true;
				}
			}
		}

		return false;
	}

	public void LoadTable()
	{
		try
		{
			XmlElement root = AsTableBase.GetXmlRootElement( "Table/ImageTable");
			XmlNodeList nodes = root.ChildNodes;

			foreach( XmlNode node in nodes)
			{
				DelegateImageData imageData = new DelegateImageData( (XmlElement)node);
				
				switch( imageData.eType)
				{
				case eDelegateImageType.Normal:
					{
						if( true == dicGeneralDelegate.ContainsKey( imageData.id))
						{
							Debug.LogError( "already exist : " + imageData.id);
							continue;
						}
						
						imageData.locked = ( eDelegateImageSubType.None == imageData.eSubType) ? false : true;
						dicGeneralDelegate.Add( imageData.id, imageData);
					}
					break;
				case eDelegateImageType.Rare:
					{
						if( true == dicRareDelegate.ContainsKey( imageData.id))
						{
							Debug.LogError( "already exist : " + imageData.id);
							continue;
						}
						
						dicRareDelegate.Add( imageData.id, imageData);
					}
					break;
				default:
					Debug.LogError( "Invalid image type : " + imageData.eType);
					break;
				}
			}
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsDelegateImageManager:LoadTable");
		}
	}

	public void UseDelegateGetItem( int _nSlot )
	{
		RealItem _realItem = ItemMgr.HadItemManagement.Inven.GetRealItemInSlot( _nSlot );
		if( null == _realItem)
		{
			Debug.LogError("ReceiveImageGetCheckResult() [ null == realItem ] slot : " + _nSlot );
			return;
		}
		
		ItemData _itemData = _realItem.item.ItemData;
		if( Item.eITEM_TYPE.UseItem == _itemData.GetItemType() && Item.eUSE_ITEM.ImageGet == (Item.eUSE_ITEM)_itemData.GetSubType() )
		{
			DelegateImageData	_imageData = AsDelegateImageManager.Instance.GetDelegateImage( _itemData.m_iItem_Rand_ID );

			if( _imageData.locked == true )
			{
				GameObject itemGetDlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/Optimization/Prefab/DelegateImageGet")) as GameObject;
				Debug.Assert( null != itemGetDlg);
				if( itemGetDlg != null )
				{
					AsDelegateImageItemGetDlg dlg = itemGetDlg.GetComponent<AsDelegateImageItemGetDlg>();
					Debug.Assert( null != dlg);
					dlg.Init( _imageData , _nSlot );
				}
			}
			else
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126),  AsTableManager.Instance.GetTbl_String(2714), AsNotify.MSG_BOX_TYPE.MBT_OK);
			}
		}

	}

}
