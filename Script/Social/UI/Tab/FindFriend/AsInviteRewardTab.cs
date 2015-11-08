using UnityEngine;
using System.Collections;

public class AsInviteRewardTab : AsSocialTab {
	
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public SpriteText noReward = null;
	
	public SpriteText m_CountMessageText = null;
	
	override public void Init()
	{
	}

	public override UIScrollList getList()
	{
		return null;
	}
	
	void OnEnable()
	{
		noReward.gameObject.SetActiveRecursively( true);
		m_list.ClearList( true);
	}

	// Use this for initialization
	void Start () {

		noReward.Text = AsTableManager.Instance.GetTbl_String(4209);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetRewardList( body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_list.ClearList( true);
		int count = 0;
		if( 1 == data.bLobi_Reward )
		{
			InsertLobiReward(0,data);	
			count++;
		}
		else
		{
			if( data.nLobi_Goal > data.nLobi_Day )
			{
				InsertLobiReward(0,data);	
				count++;
			}
		}
		
		if( 1 == data.bLine_Reward )
		{
			InsertLineReward(0,data);	
			count++;
		}
		else
		{
			if( data.nLine_Goal >  data.nLine_Day )
			{
				InsertLineReward(0,data);	
				count++;
			}
		}
		
	
	/*	for( int i = 0; i < AsGameDefine.MAX_GAME_INVITE_REWARD; ++i)
		{			
			if( 1 == data.bFacebook_Reward[i])
			{
				InsertFacebookReward(i,data);				
				count++;
			}
			else
			{
				if(data.nFacebook_Goal[i] > data.nFacebook_Day )
				{
					InsertFacebookReward(i,data);
					count++;
				}				
			}
			
		}
		
		for( int i = 0; i < AsGameDefine.MAX_GAME_INVITE_REWARD; ++i)
		{			
			if( 1 == data.bSms_Reward[i])
			{
				InsertSmsReward(i,data);
			
				count++;
			}
			else
			{
				if( data.nSms_Goal[i]  > data.nSms_Day )
				{
					InsertSmsReward(i,data);
					count++;
				}				
			}
			
		}
		*/
		
		for( int i = 0; i < AsGameDefine.MAX_GAME_INVITE_REWARD; ++i)
		{			
			if( 1 == data.bTwitter_Reward[i])
			{
				InsertTwitterReward(i,data);
			
				count++;
			}
			else
			{
				if( data.nTwitter_Goal[i]  > data.nTwitter_Day )
				{
					InsertTwitterReward(i,data);
					count++;
				}				
			}
			
		}
		
		m_CountMessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 4285), data.nTwitter_Total);
		noReward.gameObject.SetActiveRecursively( 0 == count);
	}
	
	void InsertFacebookReward(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		UIListItemContainer fbItem = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;
				
		AsInviteRewardItem fbRewardItem = fbItem.gameObject.GetComponent<AsInviteRewardItem>();
		fbRewardItem.SetFbRewardData(index, data);		
		
		fbItem.ScanChildren();
		m_list.ClipItems();			
	
	}
	void InsertSmsReward(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		UIListItemContainer pnItem = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;
				
		AsInviteRewardItem pnRewardItem = pnItem.gameObject.GetComponent<AsInviteRewardItem>();
		pnRewardItem.SetPnRewardData(index, data);		
		
		pnItem.ScanChildren();
		m_list.ClipItems();		

	}
	
	void InsertLobiReward(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		UIListItemContainer pnItem = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;
				
		AsInviteRewardItem lobiRewardItem = pnItem.gameObject.GetComponent<AsInviteRewardItem>();
		lobiRewardItem.SetLobiRewardData(index, data);		
		
		pnItem.ScanChildren();
		m_list.ClipItems();		
	
	}
	
	void InsertLineReward(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		UIListItemContainer pnItem = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;
				
		AsInviteRewardItem lineRewardItem = pnItem.gameObject.GetComponent<AsInviteRewardItem>();
		lineRewardItem.SetLineRewardData(index, data);		
		
		pnItem.ScanChildren();
		m_list.ClipItems();		
	}
	
	void InsertTwitterReward(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		UIListItemContainer pnItem = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;
				
		AsInviteRewardItem pnRewardItem = pnItem.gameObject.GetComponent<AsInviteRewardItem>();
		pnRewardItem.SetTwitterRewardData(index, data);		
		
		pnItem.ScanChildren();
		m_list.ClipItems();		

	}
}
