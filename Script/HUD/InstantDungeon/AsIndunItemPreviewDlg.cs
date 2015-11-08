using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsIndunItemPreviewDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;

	public GameObject listItemPrefab;
	public SimpleSprite upArrowSprite;
	public SimpleSprite downArrowSprite;
	public UIScrollList scrollList;
	public SpriteText txtTitle;
	public SpriteText txtIcon;
	public SpriteText txtGrade;
	public SpriteText txtLevel;
	public SpriteText txtItemName;
	
	public UIRadioBtn	btnNormal;
	public UIRadioBtn	btnHard;
	public UIRadioBtn	btnHell;
	public SpriteText 	txtInfo;
	
	List<GachaFreeLineupListItemController> listLineupListItemCon = new List<GachaFreeLineupListItemController>();
	
	int						_selectedIndunIndex;

	void Start()
	{
		scrollList.SetInputDelegate(InputDelegate);
		
		txtTitle.Text	= AsTableManager.Instance.GetTbl_String(2357);
		txtIcon.Text	= AsTableManager.Instance.GetTbl_String(2169);
		txtGrade.Text	= AsTableManager.Instance.GetTbl_String(2168);
		txtLevel.Text	= AsTableManager.Instance.GetTbl_String(1724);
		txtItemName.Text= AsTableManager.Instance.GetTbl_String(2273);
		txtInfo.Text = AsTableManager.Instance.GetTbl_String(2715);
		
		btnNormal.Text = AsTableManager.Instance.GetTbl_String(1873);
		btnHard.Text = AsTableManager.Instance.GetTbl_String(1874);
		btnHell.Text = AsTableManager.Instance.GetTbl_String(1897);
		
		btnNormal.SetInputDelegate (NormalBtnDelegate);
		btnHard.SetInputDelegate (HardBtnDelegate);
		btnHell.SetInputDelegate (HellBtnDelegate);

	}

	// nGrade: 0 normal, 1 hard, 2 hell
	public void Open(GameObject goRoot,int nIndunIndex, int nGrade)
	{
		m_goRoot = goRoot;


		switch (nGrade) 
		{
		case 0:
			//btnNormal.Value = true;
			btnNormal.SetState( 0);
			break;
			
		case 1:
			//btnHard.Value = true;
			btnHard.SetState( 0);
			break;
			
		case 2:
			//btnHell.Value = true;
			btnHell.SetState( 0);
			break;
		}
		
		_selectedIndunIndex = nIndunIndex;
		
		IndunRepresentRewardInfoSet ( nIndunIndex , nGrade );

		_SetupTabButton( nIndunIndex);
	}

	Tbl_InsDungeonReward_Record GetIndunRewardRecord( int _indunIndex , int nGrade )
	{
		string strGrade = "normal";

		//	convert grade int => string
		switch (nGrade) 
		{
		case 0:	strGrade = "normal";	break;
		case 1:	strGrade = "hard";	break;
		case 2:	strGrade = "hell";	break;
		}

		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( pair.Value.InstanceTableIdx == _indunIndex &&  strGrade.Contains( pair.Value.Grade.ToLower() )  )
			{
				return pair.Value;
			}
		}
		
		return null;
	}
	
	
	void IndunRepresentRewardInfoSet( int _indunIndex , int nGrade )
	{
		Tbl_InsDungeonReward_Record _record = GetIndunRewardRecord (_indunIndex, nGrade);
		
		if (_record == null) 
		{
			Debug.LogError("Tbl_InsDungeonReward_Record is null [" + _indunIndex + "]" );
			return;
		}
		
		List<int> listRewardItem = new List<int> ();
		
		eCLASS _class = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS> (eComponentProperty.CLASS);
		
		//	1. first class reward set
		switch (_class) 
		{
		case eCLASS.DIVINEKNIGHT:
			listRewardItem.Add ( _record.DivineKnight_Reward_Ex1 );
			listRewardItem.Add ( _record.DivineKnight_Reward_Ex2 );
			listRewardItem.Add ( _record.DivineKnight_Reward_Ex3 );
			listRewardItem.Add ( _record.DivineKnight_Reward_Ex4 );
			listRewardItem.Add ( _record.DivineKnight_Reward_Ex5 );
			break;
			
		case eCLASS.MAGICIAN:
			listRewardItem.Add ( _record.Magician_Reward_Ex1 );
			listRewardItem.Add ( _record.Magician_Reward_Ex2 );
			listRewardItem.Add ( _record.Magician_Reward_Ex3 );
			listRewardItem.Add ( _record.Magician_Reward_Ex4 );
			listRewardItem.Add ( _record.Magician_Reward_Ex5 );
			break;
			
		case eCLASS.CLERIC:
			listRewardItem.Add ( _record.Cleric_Reward_Ex1 );
			listRewardItem.Add ( _record.Cleric_Reward_Ex2 );
			listRewardItem.Add ( _record.Cleric_Reward_Ex3 );
			listRewardItem.Add ( _record.Cleric_Reward_Ex4 );
			listRewardItem.Add ( _record.Cleric_Reward_Ex5 );
			break;
			
		case eCLASS.HUNTER:
			listRewardItem.Add ( _record.Hunter_Reward_Ex1 );
			listRewardItem.Add ( _record.Hunter_Reward_Ex2 );
			listRewardItem.Add ( _record.Hunter_Reward_Ex3 );
			listRewardItem.Add ( _record.Hunter_Reward_Ex4 );
			listRewardItem.Add ( _record.Hunter_Reward_Ex5 );
			break;
		}
		
		//	2. second common reward set
		listRewardItem.Add ( _record.Common_Reward1 );
		listRewardItem.Add ( _record.Common_Reward2 );
		listRewardItem.Add ( _record.Common_Reward3 );
		listRewardItem.Add ( _record.Common_Reward4 );
		listRewardItem.Add ( _record.Common_Reward5 );
		
		ShowLineUp (listRewardItem);
	}
	
	
	void ShowLineUp(List<int> listItem)
	{
		listLineupListItemCon.Clear();
		
		scrollList.ClearList (true);
		
		foreach (int itemID in listItem)
		{
			Item newItem = ItemMgr.ItemManagement.GetItem(itemID);
			
			if (newItem == null)
				continue;

			IUIObject newListItem = scrollList.CreateItem(listItemPrefab);
			
			GachaFreeLineupListItemController newListItemController = newListItem.gameObject.GetComponent<GachaFreeLineupListItemController>();
			
			if (newListItemController == null)
				continue;

			// add list
			listLineupListItemCon.Add(newListItemController);
			newListItemController.itemID = itemID;
			
			GameObject objIcon = AsCashStore.GetItemIcon(newItem.ItemID.ToString(),  1);
			
			newListItemController.SetInfo(objIcon, newItem.GetStrGrade(), newItem.ItemData.levelLimit, AsTableManager.Instance.GetTbl_String(newItem.ItemData.nameId));
		}

		scrollList.UpdateCamera();
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		
		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	public void OnBtnClose()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}

	void NormalBtnDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			IndunRepresentRewardInfoSet ( _selectedIndunIndex , 0 );
		}
	}
	
	void HardBtnDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			IndunRepresentRewardInfoSet ( _selectedIndunIndex , 1 );
		}
	}
	
	void HellBtnDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			IndunRepresentRewardInfoSet ( _selectedIndunIndex , 2 );
		}
	}
	
	void CheckMorelistMark()
	{
		if (scrollList == null)
			return;
		
		if (scrollList.Count > 0 && scrollList.gameObject.activeSelf)
		{
			if (!scrollList.IsShowingLastItem())
			{
				if (downArrowSprite.IsHidden() == true)
					downArrowSprite.Hide(false);
			}
			else
			{
				if (downArrowSprite.IsHidden() == false)
					downArrowSprite.Hide(true);
			}
			
			if (!scrollList.IsShowingFirstItem())
			{
				if (upArrowSprite.IsHidden() == true)
					upArrowSprite.Hide(false);
			}
			else
			{
				if (upArrowSprite.IsHidden() == false)
					upArrowSprite.Hide(true);
			}
		}
		else
		{
			if (downArrowSprite.IsHidden() == false)
				downArrowSprite.Hide(true);
			
			if (upArrowSprite.IsHidden() == false)
				upArrowSprite.Hide(true);
		}
	}
	
	public void InputDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			foreach (GachaFreeLineupListItemController con in listLineupListItemCon)
			{
				con.boxCollider.enabled = true;
				if (AsUtil.PtInCollider(con.boxCollider, ptr.ray))
				{
					ShowTooltip(con.itemID);
					con.boxCollider.enabled = false;
					break;
				}
				con.boxCollider.enabled = false;
			}
		}
	}
	
	void ShowTooltip(int _itemID)
	{
		AsSoundManager.Instance.PlaySound(AsSoundPath.ButtonClick, Vector3.zero, false);
		
		Item itemData = ItemMgr.ItemManagement.GetItem(_itemID);
		
		if (itemData != null)
		{
			RealItem haveitem = null;
			
			if (Item.eITEM_TYPE.EquipItem == itemData.ItemData.GetItemType())
				haveitem = ItemMgr.HadItemManagement.Inven.GetEquipItem(itemData.ItemData.GetSubType());
			else if (Item.eITEM_TYPE.CosEquipItem == itemData.ItemData.GetItemType())
				haveitem = ItemMgr.HadItemManagement.Inven.GetCosEquipItem(itemData.ItemData.GetSubType());
			
			if (TooltipMgr.Instance != null)
				TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.left, _itemID);
		}
	}

	void Update()
	{
		if (scrollList != null)
			if (scrollList.gameObject.activeSelf)
				CheckMorelistMark();
	}

	private void _SetupTabButton(int nIndunID)
	{
		if( false == _isIndunGradeUse( nIndunID, "normal"))
		{
			btnNormal.SetState( 2);
			btnNormal.controlIsEnabled = false;
		}

		if( false == _isIndunGradeUse( nIndunID, "hard"))
		{
			btnHard.SetState( 2);
			btnHard.controlIsEnabled = false;
		}

		if( false == _isIndunGradeUse( nIndunID, "hell"))
		{
			btnHell.SetState( 2);
			btnHell.controlIsEnabled = false;
		}
	}

	private bool _isIndunGradeUse(int nIndunID, string strGradeLower)
	{
		Dictionary<int, Tbl_InsDungeonReward_Record> dic = AsTableManager.Instance.GetInsRewardRecordList();
		
		foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair in dic)
		{
			if( nIndunID == pair.Value.InstanceTableIdx && pair.Value.Grade.ToLower().Contains( strGradeLower) && 1 == pair.Value.MaxPlayerCount)
			{
				if( 0 == pair.Value.Use)
				{
					foreach( KeyValuePair<int, Tbl_InsDungeonReward_Record> pair2 in dic)
					{
						if( nIndunID == pair2.Value.InstanceTableIdx && pair2.Value.Grade.ToLower().Contains( strGradeLower) && 4 == pair2.Value.MaxPlayerCount)
						{
							if( 0 == pair2.Value.Use)
							{
								return false;
							}
							else
							{
								return true;
							}
						}
					}
				}
				else
				{
					return true;
				}
			}
		}
		
		return true;
	}
}
