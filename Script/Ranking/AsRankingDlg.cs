using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public enum eRankViewType : int
{
	Invalid = -1,
	
	World,
	Friend,
	PvpWorld,
	PvpFriend,
	Week,
	Max
};

public class AsRankingDlg : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]UIPanelTab worldTab = null;
	[SerializeField]UIPanelTab friendTab = null;
	[SerializeField]UIPanelTab pvpworldTab = null;
	[SerializeField]UIPanelTab pvpfriendTab = null;
	[SerializeField]SpriteText nameText = null;
	[SerializeField]SpriteText rankText = null;
	[SerializeField]SpriteText fluctuationText = null;
	[SerializeField]SpriteText rankPointText = null;
	[SerializeField]SpriteText renewTime = null;
	[SerializeField]UIButton prevPage = null;
	[SerializeField]UIButton nextPage = null;
	[SerializeField]SpriteText pageText = null;
	[SerializeField]UIScrollList rankList = null;
	[SerializeField]UIButton toggleBtn = null;
	[SerializeField]GameObject listItem = null;
    [SerializeField]UIButton btnRewardLineup = null;

	private eRankViewType eType = eRankViewType.Invalid;
	private Int16 curPage = 0;
	// MyInfo
	[SerializeField]private AsMyRankInfo myInfo = null;
	private Int16 WorldMaxPage = 1;
	private Int16 FriendMaxPage = 1;
	private Int16 PvpWorldMaxPage = 1;
	private Int16 PvpFriendMaxPage = 1;
    private Int16 WeekMaxPage = 1;
	private const Int16 ItemsPerPage = 7;
	private bool isMyInfo = true;
	private body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT baseDate = null;
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( nameText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rankText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( fluctuationText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rankPointText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( worldTab.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( friendTab.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( pvpworldTab.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( pvpfriendTab.spriteText);
		
//		nameText.Text = AsTableManager.Instance.GetTbl_String(1767);
//		rankText.Text = AsTableManager.Instance.GetTbl_String(1662);
//		fluctuationText.Text = AsTableManager.Instance.GetTbl_String(1764);
//		rankPointText.Text = AsTableManager.Instance.GetTbl_String(1768);
		_SetString_SubTitle( eType);
		
		title.Text = AsTableManager.Instance.GetTbl_String(1662);
		worldTab.Text = AsTableManager.Instance.GetTbl_String(1663);
		friendTab.Text = AsTableManager.Instance.GetTbl_String(1664);
		pvpworldTab.Text = AsTableManager.Instance.GetTbl_String(905);
		pvpfriendTab.Text = AsTableManager.Instance.GetTbl_String(910);

        btnRewardLineup.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body_SC_RANK_SUMMARY_MYRANK_LOAD_RESULT data)
	{
		eType = eRankViewType.World;
		
		baseDate = data;
		
		//InvokeRepeating( "UpdateRenewalTime", 0.0f, 60.0f);
		renewTime.Hide( true); // ilmeda
		
		myInfo.Init( data);
		
		body_CS_RANK_MYRANK_LOAD myRank = new body_CS_RANK_MYRANK_LOAD( eRANKTYPE.eRANKTYPE_ITEM);
		byte[] sendData = myRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);

#if false
		WorldMaxPage = (Int16)( data.nWorldItemRankMaxCount / ItemsPerPage);
		if( 0 != ( data.nWorldItemRankMaxCount % ItemsPerPage))
			WorldMaxPage++;
		
		FriendMaxPage = (Int16)( data.nFriendItemRankMaxCount / ItemsPerPage);
		if( 0 != ( data.nFriendItemRankMaxCount % ItemsPerPage))
			FriendMaxPage++;
#endif

		_SetString_SubTitle( eType);

		//toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1670);
		pageText.Text = "1/1";
		
		prevPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		nextPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
	}
	
	void UpdateRenewalTime()
	{
		TimeSpan ts = baseDate.tRenewalTime - DateTime.Now;
		string format = AsTableManager.Instance.GetTbl_String(1669);
		if( ( 0 >= ts.Hours) && ( 0 >= ts.Minutes))
			renewTime.Text = string.Format( format, 0, 0);
		else
			renewTime.Text = string.Format( format, ts.Hours, ts.Minutes);
	}
	
	private void OnWorldTab()
	{
		if( eRankViewType.World == eType)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eType = eRankViewType.World;
		curPage = 0;
		
		toggleBtn.spriteText.color = Color.black;
		_SetString_SubTitle( eType);

        btnRewardLineup.gameObject.SetActive(false);
		
		pageText.Text = "1/1";
		prevPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		nextPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		toggleBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		
		body_CS_RANK_MYRANK_LOAD myRank = new body_CS_RANK_MYRANK_LOAD( eRANKTYPE.eRANKTYPE_ITEM);
		byte[] sendData = myRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}
	
	private void OnFriendTab()
	{
		if( eRankViewType.Friend == eType)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eType = eRankViewType.Friend;
		curPage = 0;
		
		toggleBtn.spriteText.color = Color.gray;
		_SetString_SubTitle( eType);

        btnRewardLineup.gameObject.SetActive(false);
		
//		pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
		prevPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		nextPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		//toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1670);
		toggleBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		isMyInfo = true;
		
		body_CS_RANK_MYFRIEND_LOAD friendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ITEM, curPage);
		byte[] sendData = friendRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}
	
	private void OnPvpWorldTab()
	{
		if( eRankViewType.PvpWorld == eType)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eType = eRankViewType.PvpWorld;
		curPage = 0;
		
		toggleBtn.spriteText.color = Color.black;
		_SetString_SubTitle( eType);

        btnRewardLineup.gameObject.SetActive(false);

		pageText.Text = "1/1";
		prevPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		nextPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		toggleBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		
		body_CS_RANK_MYRANK_LOAD myRank = new body_CS_RANK_MYRANK_LOAD( eRANKTYPE.eRANKTYPE_ARENA);
		byte[] sendData = myRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}
	
	private void OnPvpFriendTab()
	{
		if( eRankViewType.PvpFriend == eType)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eType = eRankViewType.PvpFriend;
		curPage = 0;
		
		toggleBtn.spriteText.color = Color.gray;
		_SetString_SubTitle( eType);

        btnRewardLineup.gameObject.SetActive(false);
//		pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
		prevPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		nextPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		//toggleBtn.Text = AsTableManager.Instance.GetTbl_String(909);
		toggleBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		isMyInfo = true;
		
		body_CS_RANK_MYFRIEND_LOAD friendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ARENA, curPage);
		byte[] sendData = friendRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
	}

    private void OnWeekTab()
    {
        if( eRankViewType.Week == eType)
			return;
		
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		eType = eRankViewType.Week;
		curPage = 0;
		
		toggleBtn.spriteText.color = Color.black;
		_SetString_SubTitle( eType);

        btnRewardLineup.gameObject.SetActive(true);

//		pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
		prevPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		nextPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		//toggleBtn.Text = AsTableManager.Instance.GetTbl_String(909);
		toggleBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		isMyInfo = false;

        body_CS_RANK_TOP_LOAD myRank = new body_CS_RANK_TOP_LOAD(eRANKTYPE.eRANKTYPE_AP, curPage);
        byte[] sendData = myRank.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( sendData);
    }
	
	private void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsHudDlgMgr.Instance.CloseRankingDlg();
	}
	
	private void OnPrevBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage--;
		
		if( 0 > curPage)
		{
			curPage = 0;
			return;
		}
		
		if( eRankViewType.World == eType)
		{
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, WorldMaxPage);
			
			body_CS_RANK_TOP_LOAD worldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ITEM, curPage);
			byte[] sendData = worldRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		else if( eRankViewType.Friend == eType)
		{
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
			
			body_CS_RANK_MYFRIEND_LOAD friendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ITEM, curPage);
			byte[] sendData = friendRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		else if( eRankViewType.PvpWorld == eType)
		{
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpWorldMaxPage);
			
			body_CS_RANK_TOP_LOAD PvpworldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ARENA, curPage);
			byte[] sendData = PvpworldRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		else if( eRankViewType.PvpFriend == eType)
		{
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpFriendMaxPage);
			
			body_CS_RANK_MYFRIEND_LOAD PvpfriendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ARENA, curPage);
			byte[] sendData = PvpfriendRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
        else if (eRankViewType.Week == eType)
        {
            pageText.Text = string.Format("{0}/{1}", curPage + 1, WeekMaxPage);

            body_CS_RANK_TOP_LOAD weekRank = new body_CS_RANK_TOP_LOAD(eRANKTYPE.eRANKTYPE_AP, curPage);
            byte[] sendData = weekRank.ClassToPacketBytes();
            AsNetworkMessageHandler.Instance.Send(sendData);
        }
	}
	
	private void OnNextBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		curPage++;
		
		if( eRankViewType.World == eType)
		{
			if( WorldMaxPage <= curPage)
			{
				curPage = (short)( WorldMaxPage - 1);
				return;
			}
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, WorldMaxPage);
			
			body_CS_RANK_TOP_LOAD worldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ITEM, curPage);
			byte[] sendData = worldRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
        else if (eRankViewType.Week == eType)
        {
            if (WeekMaxPage <= curPage)
            {
                curPage = (short)(WeekMaxPage - 1);
                return;
            }

            pageText.Text = string.Format("{0}/{1}", curPage + 1, WeekMaxPage);

            body_CS_RANK_TOP_LOAD weekRank = new body_CS_RANK_TOP_LOAD(eRANKTYPE.eRANKTYPE_AP, curPage);
            byte[] sendData = weekRank.ClassToPacketBytes();
            AsNetworkMessageHandler.Instance.Send(sendData);
        }
		else if( eRankViewType.Friend == eType)
		{
			if( FriendMaxPage <= curPage)
			{
				curPage = (short)( FriendMaxPage - 1);
				return;
			}
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
			
			body_CS_RANK_MYFRIEND_LOAD friendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ITEM, curPage);
			byte[] sendData = friendRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		else if( eRankViewType.PvpWorld == eType)
		{
			if( PvpWorldMaxPage <= curPage)
			{
				curPage = (short)( PvpWorldMaxPage - 1);
				return;
			}
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpWorldMaxPage);
			
			body_CS_RANK_TOP_LOAD PvpworldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ARENA, curPage);
			byte[] sendData = PvpworldRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
		else if( eRankViewType.PvpFriend == eType)
		{
			if( PvpFriendMaxPage <= curPage)
			{
				curPage = (short)( PvpFriendMaxPage - 1);
				return;
			}
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpFriendMaxPage);
			
			body_CS_RANK_MYFRIEND_LOAD PvpfriendRank = new body_CS_RANK_MYFRIEND_LOAD( eRANKTYPE.eRANKTYPE_ARENA, curPage);
			byte[] sendData = PvpfriendRank.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( sendData);
		}
	}
	
	private void OnToggleBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( true == isMyInfo)
		{
			if( eRankViewType.World == eType || eRankViewType.Friend == eType)
			{
				curPage = 0;
				toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1665);
				pageText.Text = string.Format( "1/{0}", WorldMaxPage);
				
				body_CS_RANK_TOP_LOAD worldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ITEM, 0);
				byte[] sendData = worldRank.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( sendData);
				
				prevPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				nextPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			}
            else if (eRankViewType.Week == eType)
            {
                curPage = 0;
                toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1665);
                pageText.Text = string.Format("1/{0}", WeekMaxPage);

                body_CS_RANK_TOP_LOAD weekRank = new body_CS_RANK_TOP_LOAD(eRANKTYPE.eRANKTYPE_AP, 0);
                byte[] sendData = weekRank.ClassToPacketBytes();
                AsNetworkMessageHandler.Instance.Send(sendData);

                prevPage.SetControlState(UIButton.CONTROL_STATE.NORMAL);
                nextPage.SetControlState(UIButton.CONTROL_STATE.NORMAL);
            }
			else
			{
				curPage = 0;
				toggleBtn.Text = AsTableManager.Instance.GetTbl_String(908);
				pageText.Text = string.Format( "1/{0}", PvpWorldMaxPage);
				
				body_CS_RANK_TOP_LOAD PvpworldRank = new body_CS_RANK_TOP_LOAD( eRANKTYPE.eRANKTYPE_ARENA, 0);
				byte[] sendData = PvpworldRank.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( sendData);
				
				prevPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
				nextPage.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			}
		}
		else
		{
			if( eRankViewType.World == eType || eRankViewType.Friend == eType)
			{
				toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1670);
				pageText.Text = "1/1";
				
				body_CS_RANK_MYRANK_LOAD myRank = new body_CS_RANK_MYRANK_LOAD( eRANKTYPE.eRANKTYPE_ITEM);
				byte[] sendData = myRank.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( sendData);
				
				prevPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
				nextPage.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			}
            else if (eRankViewType.Week == eType)
            {
                toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1670);
                pageText.Text = "1/1";

                body_CS_RANK_MYRANK_LOAD myRank = new body_CS_RANK_MYRANK_LOAD(eRANKTYPE.eRANKTYPE_AP);
                byte[] sendData = myRank.ClassToPacketBytes();
                AsNetworkMessageHandler.Instance.Send(sendData);

                prevPage.SetControlState(UIButton.CONTROL_STATE.DISABLED);
                nextPage.SetControlState(UIButton.CONTROL_STATE.DISABLED);
            }
            else
            {
                toggleBtn.Text = AsTableManager.Instance.GetTbl_String(909);
                pageText.Text = "1/1";

                body_CS_RANK_MYRANK_LOAD PvpmyRank = new body_CS_RANK_MYRANK_LOAD(eRANKTYPE.eRANKTYPE_ARENA);
                byte[] sendData = PvpmyRank.ClassToPacketBytes();
                AsNetworkMessageHandler.Instance.Send(sendData);

                prevPage.SetControlState(UIButton.CONTROL_STATE.DISABLED);
                nextPage.SetControlState(UIButton.CONTROL_STATE.DISABLED);
            }
		}
		
		isMyInfo = !isMyInfo;
	}
	
	public void InsertWorldData( body_SC_RANK_TOP_LOAD_RESULT data)
	{
		if( eRANKTYPE.eRANKTYPE_ITEM == data.eRankType)
		{
			WorldMaxPage = (Int16)( data.nWorldItemRankMaxCount / ItemsPerPage);
			if( 0 != ( data.nWorldItemRankMaxCount % ItemsPerPage))
				WorldMaxPage++;
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, WorldMaxPage);
		}
		else if( eRANKTYPE.eRANKTYPE_ARENA == data.eRankType)
		{
			PvpWorldMaxPage = (Int16)( data.nWorldItemRankMaxCount / ItemsPerPage);
			if( 0 != ( data.nWorldItemRankMaxCount % ItemsPerPage))
				PvpWorldMaxPage++;
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpWorldMaxPage);
		}
        else if (eRANKTYPE.eRANKTYPE_AP == data.eRankType)
        {
            WeekMaxPage = (Int16)(data.nWorldItemRankMaxCount / ItemsPerPage);
            if (0 != (data.nWorldItemRankMaxCount % ItemsPerPage))
                WeekMaxPage++;

            pageText.Text = string.Format("{0}/{1}", curPage + 1, WeekMaxPage);
        }
        else
        {
            Debug.Log("AsRankingDlg::InsertWorldData(), eRANKTYPE: " + data.eRankType);
            return;
        }
		
		rankList.ClearList( true);
		
		foreach( sRANKINFO info in data.sRankInfo)
		{
			if( eCLASS.NONE == info.eClass)
				continue;
			
			UIListButton listBtn = rankList.CreateItem( listItem) as UIListButton;
			AsRankListItem item = listBtn.gameObject.GetComponent<AsRankListItem>();
			Debug.Assert( null != item);
			item.Init( info);

            if (data.eRankType == eRANKTYPE.eRANKTYPE_AP)
            {
                item.DisableFluctuation();
                item.SetApRewardInfo(AsTableManager.Instance.GetTbl_ApRewardInfoList(info.eClass, info.nRank, data.nRewardGroup));
            }
		}
	}
	
	public void InsertWorldData( body_SC_RANK_MYRANK_LOAD_RESULT data)
	{
		rankList.ClearList( true);
		
		foreach( sRANKINFO info in data.sMyWorldRankInfo)
		{
			if( eCLASS.NONE == info.eClass)
				continue;
			
			UIListButton listBtn = rankList.CreateItem( listItem) as UIListButton;
			AsRankListItem item = listBtn.gameObject.GetComponent<AsRankListItem>();
			Debug.Assert( null != item);
			item.Init( info);

			if( info.nCharUniqKey == AsUserInfo.Instance.SavedCharStat.uniqKey_)
				myInfo.SetMyInfo( eType, info.nRankPoint);

            if (data.eRankType == eRANKTYPE.eRANKTYPE_AP)
            {
                item.DisableFluctuation();
                item.SetApRewardInfo(AsTableManager.Instance.GetTbl_ApRewardInfoList(info.eClass, info.nRank, data.nRewardGroup));
            }
		}
	}
	
	public void InsertFriendData( body_SC_RANK_MYFRIEND_LOAD_RESULT data)
	{
		if( eRANKTYPE.eRANKTYPE_ITEM == data.eRankType)
		{
			FriendMaxPage = (Int16)( data.nFriendItemRankMaxCount / ItemsPerPage);
			if( 0 != ( data.nFriendItemRankMaxCount % ItemsPerPage))
				FriendMaxPage++;
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, FriendMaxPage);
		}
		else if( eRANKTYPE.eRANKTYPE_ARENA == data.eRankType)
		{
			PvpFriendMaxPage = (Int16)( data.nFriendItemRankMaxCount / ItemsPerPage);
			if( 0 != ( data.nFriendItemRankMaxCount % ItemsPerPage))
				PvpFriendMaxPage++;
			
			pageText.Text = string.Format( "{0}/{1}", curPage + 1, PvpFriendMaxPage);
		}
		else
		{
			Debug.Log( "AsRankingDlg::InsertFriendData(), eRANKTYPE: " + data.eRankType);
			return;
		}
		
		rankList.ClearList( true);
		
		foreach( sRANKINFO info in data.sRankInfo)
		{
			if( eCLASS.NONE == info.eClass)
				continue;
			
			UIListButton listBtn = rankList.CreateItem( listItem) as UIListButton;
			AsRankListItem item = listBtn.gameObject.GetComponent<AsRankListItem>();
			Debug.Assert( null != item);
			item.Init( info);

			if( info.nCharUniqKey == AsUserInfo.Instance.SavedCharStat.uniqKey_)
				myInfo.SetMyInfo( eType, info.nRankPoint);
		}
	}
	
	private void _SetString_SubTitle(eRankViewType type)
	{
		if( eRankViewType.PvpWorld == type || eRankViewType.PvpFriend == type)
		{
			nameText.Text = AsTableManager.Instance.GetTbl_String(1767);
			rankText.Text = AsTableManager.Instance.GetTbl_String(2105);
			fluctuationText.Text = AsTableManager.Instance.GetTbl_String(2104);
			rankPointText.Text = AsTableManager.Instance.GetTbl_String(2106);
			toggleBtn.Text = AsTableManager.Instance.GetTbl_String(909);
		}
        else if (eRankViewType.Week == type)
        {
            nameText.Text = AsTableManager.Instance.GetTbl_String(1767);
            rankText.Text = AsTableManager.Instance.GetTbl_String(2105);
            fluctuationText.Text = AsTableManager.Instance.GetTbl_String(2104);
            rankPointText.Text = AsTableManager.Instance.GetTbl_String(2106);
            toggleBtn.Text = AsTableManager.Instance.GetTbl_String(909);
        }
        else
        {
            nameText.Text = AsTableManager.Instance.GetTbl_String(1767);
            rankText.Text = AsTableManager.Instance.GetTbl_String(1662);
            fluctuationText.Text = AsTableManager.Instance.GetTbl_String(1764);
            rankPointText.Text = AsTableManager.Instance.GetTbl_String(1768);
            toggleBtn.Text = AsTableManager.Instance.GetTbl_String(1670);
        }
	}

    public void GuiInputUp(Ray _ray)
    {
        if (eRankViewType.Week != eType)
            return;

        List<IUIListObject> listItems = rankList.GetItems();

        foreach (IUIListObject listObj in listItems)
        {
            AsRankListItem rankListItem = listObj.gameObject.GetComponent<AsRankListItem>();

            if (rankListItem != null)
                rankListItem.ShowTooltip(_ray);
        }
    }
}
