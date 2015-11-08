using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class AsDesignationDlg : MonoBehaviour
{
	const int ITEMS_PER_PAGE = 7;
	
	[SerializeField] SpriteText title = null;
	[SerializeField] UIButton closeBtn = null;
	[SerializeField] AsMyDesignationInfo curDesignationInfo = null;
	[SerializeField] UIButton assignBtn = null;
	[SerializeField] UIButton releaseBtn = null;
	[SerializeField] UIPanelTab[] tabs = new UIPanelTab[0];
	[SerializeField] AsCategoryAchieveInfo achieveInfo = null;
	[SerializeField] UIRadioBtn nameRdo = null;
	[SerializeField] SpriteText requireText = null;
	[SerializeField] SpriteText effectText = null;
	[SerializeField] UIRadioBtn rankRdo = null;
	[SerializeField] UIScrollList designationList = null;
	[SerializeField] SpriteText rewardText = null;
	[SerializeField] UIButton prevBtn = null;
	[SerializeField] SpriteText pageText = null;
	[SerializeField] UIButton nextBtn = null;
	[SerializeField] GameObject listItem = null;
	[SerializeField] Color releaseBtnColor = Color.red;
	[SerializeField] Color assignBtnColor = Color.blue;
	
	private StringBuilder sb = new StringBuilder();
	private int curPage = 0;
	private int curCategoryDesignationCount = 0;
	private eDesignationCategory curCategory = eDesignationCategory.Invalid;
	private IUIObject selectedItem = null;
	private bool isAscendingByName = false;
	private bool isAscendingByRank = false;

	private eDesignationCategory lastSelectCategory = eDesignationCategory.Invalid;
	private int lastSelectPage = 0;

	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(2172);
		closeBtn.SetInputDelegate( _closeBtnDelegate);
		DesignationData data = AsDesignationManager.Instance.GetCurrentDesignation();
		curDesignationInfo.Init( data);
		
		assignBtn.spriteText.Color = Color.gray;
		assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		assignBtn.Text = AsTableManager.Instance.GetTbl_String(4076);
		assignBtn.SetInputDelegate( _assignBtnDelegate);
		if( 0 >= AsDesignationManager.Instance.CurrentID)
		{
			releaseBtn.spriteText.Color = Color.gray;
			releaseBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
		releaseBtn.Text = AsTableManager.Instance.GetTbl_String(1356);
		releaseBtn.SetInputDelegate( _releaseBtnDelegate);
		
		tabs[ (int)eDesignationCategory.Invalid].Text = AsTableManager.Instance.GetTbl_String(2176);
		tabs[ (int)eDesignationCategory.Invalid].SetInputDelegate( _obtainedTabDelegate);
		tabs[ (int)eDesignationCategory.Character].Text = AsTableManager.Instance.GetTbl_String(2177);
		tabs[ (int)eDesignationCategory.Character].SetInputDelegate( _characterTabDelegate);
		tabs[ (int)eDesignationCategory.Monster].Text = AsTableManager.Instance.GetTbl_String(2178);
		tabs[ (int)eDesignationCategory.Monster].SetInputDelegate( _monsterTabDelegate);
		tabs[ (int)eDesignationCategory.Area].Text = AsTableManager.Instance.GetTbl_String(2161);
		tabs[ (int)eDesignationCategory.Area].SetInputDelegate( _areaTabDelegate);
		tabs[ (int)eDesignationCategory.Item].Text = AsTableManager.Instance.GetTbl_String(2179);
		tabs[ (int)eDesignationCategory.Item].SetInputDelegate( _itemTabDelegate);
		tabs[ (int)eDesignationCategory.Quest].Text = AsTableManager.Instance.GetTbl_String(19004);
		tabs[ (int)eDesignationCategory.Quest].SetInputDelegate( _questTabDelegate);
		tabs[ (int)eDesignationCategory.Friends].Text = AsTableManager.Instance.GetTbl_String(2180);
		tabs[ (int)eDesignationCategory.Friends].SetInputDelegate( _friendsTabDelegate);
		tabs[ (int)eDesignationCategory.Unique].Text = AsTableManager.Instance.GetTbl_String(2181);
		tabs[ (int)eDesignationCategory.Unique].SetInputDelegate( _uniqueTabDelegate);
		tabs[ (int)eDesignationCategory.Etc].Text = AsTableManager.Instance.GetTbl_String(1548);
		tabs[ (int)eDesignationCategory.Etc].SetInputDelegate( _etcTabDelegate);

		nameRdo.Text = AsTableManager.Instance.GetTbl_String(2173);
		requireText.Text = AsTableManager.Instance.GetTbl_String(2174);
		effectText.Text = AsTableManager.Instance.GetTbl_String(2175);
		rankRdo.Text = AsTableManager.Instance.GetTbl_String(1666);
		rewardText.Text = AsTableManager.Instance.GetTbl_String(2731);
		
		curCategoryDesignationCount = AsDesignationManager.Instance.ObtainedCount;
		designationList.SetValueChangedDelegate( _selectDesignationDelegate);
		_initDesignationList( eDesignationCategory.Invalid);
		
		prevBtn.SetInputDelegate( _prevBtnDelegate);
		nextBtn.SetInputDelegate( _nextBtnDelegate);

		//	name sorting radio button dont 
		nameRdo.gameObject.renderer.enabled = false;
		nameRdo.gameObject.collider.enabled = false;
	}
	
	public void _selectDesignationDelegate( IUIObject obj)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( designationList != obj)
			return;
		
		selectedItem = designationList.LastClickedControl;
		
		AsDesignationItem designationData = selectedItem.gameObject.GetComponent<AsDesignationItem>();
		Debug.Assert( null != designationData);

		//	release focus others and focus selected
		_releaseFocusItem ();
		designationData.Focus ();
		
		if( true == designationData.IsPossessed)
		{
			assignBtn.spriteText.Color = assignBtnColor;
			assignBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		}
		else
		{
			assignBtn.spriteText.Color = Color.gray;
			assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
		
        QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_DESIGNATION, designationData.Data.id));
	}
	
	void _initPageText( eDesignationCategory category)
	{
		sb.Remove( 0, sb.Length);
		
		if( 0 >= curCategoryDesignationCount)
		{
			sb.Append( "0/0");
		}
		else
		{
			int maxPage = curCategoryDesignationCount / ITEMS_PER_PAGE;
			if( 0 == ( curCategoryDesignationCount % ITEMS_PER_PAGE))
				maxPage--;
			
			sb.AppendFormat( "{0}/{1}", curPage + 1, maxPage + 1);
		}
		
		pageText.Text = sb.ToString();
	}
	
	void _initDesignationList( eDesignationCategory category)
	{
		selectedItem = null;
		
		achieveInfo.Init( category);
		_sortRdoInit( category);
		
		switch( category)
		{
		case eDesignationCategory.Invalid:	_initObtainedList();	break;
		case eDesignationCategory.Character:	_initCharacterCategoryList();	break;
		case eDesignationCategory.Monster:	_initMonsterCategoryList();	break;
		case eDesignationCategory.Area:	_initAreaCategoryList();	break;
		case eDesignationCategory.Item:	_initItemCategoryList();	break;
		case eDesignationCategory.Quest:	_initQuestCategoryList();	break;
		case eDesignationCategory.Friends:	_initFriendsCategoryList();	break;
		case eDesignationCategory.Unique:	_initUniqueCategoryList();	break;
		case eDesignationCategory.Etc:	_initEtcCategoryList();	break;
		default:
			Debug.LogError( "AsDesignationDlg:_initDesignationList -> Invalid category");
			break;
		}
		
		_initPageText( category);

//		if (lastSelectCategory != category || lastSelectPage != curPage) 
		{
			//	assign button disable
			assignBtn.spriteText.Color = Color.gray;
			assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}

		lastSelectCategory = category;
		lastSelectPage = curPage;
	}
	
	void _sortRdoInit( eDesignationCategory category)
	{
		eDesignationSortState sortState = AsDesignationManager.Instance.GetSortState( category);
		switch( sortState)
		{
		case eDesignationSortState.Invalid:
			nameRdo.Value = false;
			rankRdo.Value = false;
			isAscendingByName = false;
			isAscendingByRank = false;
			break;
		case eDesignationSortState.Name_Ascending:
			nameRdo.Value = true;
			rankRdo.Value = false;
			isAscendingByName = true;
			isAscendingByRank = false;
			break;
		case eDesignationSortState.Name_Descending:
			nameRdo.Value = false;
			rankRdo.Value = false;
			isAscendingByName = false;
			isAscendingByRank = false;
			break;
		case eDesignationSortState.Rank_Ascending:
			nameRdo.Value = false;
			rankRdo.Value = true;
			isAscendingByName = false;
			isAscendingByRank = true;
			break;
		case eDesignationSortState.Rank_Descending:
			nameRdo.Value = false;
			rankRdo.Value = false;
			isAscendingByName = false;
			isAscendingByRank = false;
			break;
		}
	}
	
	void _initObtainedList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.ObtainedCount)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetObtainedDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initCharacterCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByCharacter)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetCharacterDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initMonsterCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByMonster)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetMonsterDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initAreaCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByArea)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetAreaDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initItemCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByItem)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetItemDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initQuestCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByQuest)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetQuestDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initFriendsCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByFriends)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetFriendsDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initUniqueCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByUnique)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetUniqueDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _initEtcCategoryList()
	{
		designationList.ClearListSync( true);
		
		if( 0 > curPage)
			return;
		
		for( int i = ( curPage * ITEMS_PER_PAGE); i < ( ( curPage + 1) * ITEMS_PER_PAGE); i++)
		{
			if( i >= AsDesignationManager.Instance.CountByEtc)
				break;
			
			DesignationData data = AsDesignationManager.Instance.GetDesignation( AsDesignationManager.Instance.GetEtcDesignation(i));
			Debug.Assert( null != data);

			UIListButton listBtn = designationList.CreateItem( listItem) as UIListButton;
			AsDesignationItem designationItem = listBtn.gameObject.GetComponent<AsDesignationItem>();
			designationItem.Init( data);
		}
	}
	
	void _nameRdoDelegate()
	{
		//	not use name sorting.
		return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		isAscendingByName = !isAscendingByName;
		nameRdo.Value = isAscendingByName;
		rankRdo.Value = false;
		
		if( true == nameRdo.Value)
		{
			AsDesignationManager.Instance.AscendingSortByName( curCategory);
			AsDesignationManager.Instance.SetSortState( curCategory, eDesignationSortState.Name_Ascending);
		}
		else
		{
			AsDesignationManager.Instance.DescendingSortByName( curCategory);
			AsDesignationManager.Instance.SetSortState( curCategory, eDesignationSortState.Name_Descending);
		}
		
		_initDesignationList( curCategory);
	}
	
	void _rankRdoDelegate()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		isAscendingByRank = !isAscendingByRank;
		rankRdo.Value = isAscendingByRank;
		nameRdo.Value = false;
		
		if( true == rankRdo.Value)
		{
			AsDesignationManager.Instance.AscendingSortByRank( curCategory);
			AsDesignationManager.Instance.SetSortState( curCategory, eDesignationSortState.Rank_Ascending);
		}
		else
		{
			AsDesignationManager.Instance.DescendingSortByRank( curCategory);
			AsDesignationManager.Instance.SetSortState( curCategory, eDesignationSortState.Rank_Descending);
		}
		
		_initDesignationList( curCategory);
	}
	
	void _prevBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage--;
			
			if( 0 > curPage)
				curPage = 0;

			_initDesignationList( curCategory);
		}
	}
	
	void _nextBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage++;
			
			int maxPage = curCategoryDesignationCount / ITEMS_PER_PAGE;
			if( 0 == ( curCategoryDesignationCount % ITEMS_PER_PAGE))
				maxPage--;
			
			if( maxPage <= curPage)
				curPage = maxPage;
			
			_initDesignationList( curCategory);
		}
	}

	void _closeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			GameObject.Destroy( gameObject.transform.parent.gameObject);
		}
	}
	
	void _releaseAssignedItem()
	{
		for( int i = 0; i < designationList.Count; i++)
		{
			IUIObject uiObj = designationList.GetItem(i) as IUIObject;
			AsDesignationItem designationItem = uiObj.gameObject.GetComponent<AsDesignationItem>();
			if( AsDesignationManager.Instance.CurrentID == designationItem.Data.id)
				designationItem.ReleaseAssign();
		}
	}

	void _releaseFocusItem()
	{
		for( int i = 0; i < designationList.Count; i++)
		{
			IUIObject uiObj = designationList.GetItem(i) as IUIObject;
			AsDesignationItem designationItem = uiObj.gameObject.GetComponent<AsDesignationItem>();
			designationItem.ReleaseFocus();
		}
	}

	void _assignBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			if( null == selectedItem)
				return;
			
			_releaseAssignedItem();
			
			AsDesignationItem designationData = selectedItem.gameObject.GetComponent<AsDesignationItem>();
			Debug.Assert( null != designationData);
			designationData.Assign();
			
			curDesignationInfo.Init( designationData.Data);
			
			bool bSubTitleHide = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SubTitleName);
			
			body_CS_SUBTITLE_SET subtitleSet = new body_CS_SUBTITLE_SET( designationData.Data.id, bSubTitleHide);
			byte[] data = subtitleSet.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
			
			releaseBtn.spriteText.Color = releaseBtnColor;
			releaseBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			
			assignBtn.spriteText.Color = Color.gray;
			assignBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);

			_releaseFocusItem ();
		}
	}

	void _releaseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			_releaseAssignedItem();
			
			curDesignationInfo.Init( null);
			
			bool bSubTitleHide = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SubTitleName);
			
			body_CS_SUBTITLE_SET subtitleSet = new body_CS_SUBTITLE_SET( 0, bSubTitleHide);
			byte[] data = subtitleSet.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
			
			releaseBtn.spriteText.Color = Color.gray;
			releaseBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
	}
			
	void _obtainedTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.ObtainedCount;
			curCategory = eDesignationCategory.Invalid;
			_initDesignationList( curCategory);
		}
	}
	
	void _characterTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByCharacter;
			curCategory = eDesignationCategory.Character;
			_initDesignationList( curCategory);
		}
	}
	
	void _monsterTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByMonster;
			curCategory = eDesignationCategory.Monster;
			_initDesignationList( curCategory);
		}
	}
	
	void _areaTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByArea;
			curCategory = eDesignationCategory.Area;
			_initDesignationList( curCategory);
		}
	}
	
	void _itemTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByItem;
			curCategory = eDesignationCategory.Item;
			_initDesignationList( curCategory);
		}
	}
	
	void _questTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByQuest;
			curCategory = eDesignationCategory.Quest;
			_initDesignationList( curCategory);
		}
	}
	
	void _friendsTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByFriends;
			curCategory = eDesignationCategory.Friends;
			_initDesignationList( curCategory);
		}
	}
	
	void _uniqueTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByUnique;
			curCategory = eDesignationCategory.Unique;
			_initDesignationList( curCategory);
		}
	}
	
	void _etcTabDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			curPage = 0;
			curCategoryDesignationCount = AsDesignationManager.Instance.CountByEtc;
			curCategory = eDesignationCategory.Etc;
			_initDesignationList( curCategory);
		}
	}

	public void SetDesignationIndexReward( int _designationID , bool _isReceiveSuccess )
	{
		if (_isReceiveSuccess == false)
			return;

		List<IUIListObject> list = designationList.GetItems();

		foreach (UIListItem listItem in list)
		{
			AsDesignationItem designationItem = listItem.gameObject.GetComponentInChildren<AsDesignationItem>();
			if( designationItem.Data.isRewardReceived == false && designationItem.Data.id == _designationID )
			{
				designationItem.Data.isRewardReceived = true;
				designationItem.SetRewardItem();
				break;
			}
		}
	}

	public void SetDesignationAccrueReward()
	{
		achieveInfo.Init( eDesignationCategory.Invalid );
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
