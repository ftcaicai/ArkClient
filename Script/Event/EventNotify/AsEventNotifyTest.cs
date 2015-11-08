using UnityEngine;
using System.Collections;
using System.Text;

public class AsEventNotifyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	 void OnGUI()
    {
		
        GUILayout.BeginArea( new Rect( 1, 1, 200, 100));
        if( true == GUILayout.Button( "EventNotify: " ))
		{
			body_SC_USER_EVENT_NOTIFY notify = new body_SC_USER_EVENT_NOTIFY();
			notify.eType = (int)eUSEREVENTTYPE.eUSEREVENTTYPE_ITEM;
			notify.szCharName = System.Text.UTF8Encoding.UTF8.GetBytes("mmma");
			notify.sItem = new sITEM();
			notify.sItem.nItemTableIdx = 175041; 
			notify.nValue_1 = (int)eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_HUNTING;
			//notify.nValue_1 = 175041;
			//notify.nValue_2 = (int)eUSEREVENT_ITEM_GETTYPE.eUSEREVENT_ITEM_GETTYPE_HUNTING;
			AsEventNotifyMgr.Instance.ReceiveUserEventNotify(notify);
		
			sPARTYLIST[] pList = new sPARTYLIST[20];
		for(int i = 0; i < 20; i++)
		{
		   pList[i] = new sPARTYLIST();
			
			pList[i].sOption = new sPARTYOPTION();
			pList[i].sOption.bPublic = true;
			
			
			pList[i].nPartyIdx = i+1;
			pList[i].nLevel = i % 10;
			pList[i].nUserCnt = i % 4;
			
			pList[i].sOption.szPartyName = 	 System.Text.UTF8Encoding.UTF8.GetBytes( pList[i].nLevel.ToString().ToCharArray(), 0, pList[i].nLevel.ToString().Length ); 
			if(pList[i].nUserCnt==0)
				pList[i].nUserCnt = 1;
		}
	    pList[0].sOption.nMaxUser = 4;
		pList[1].sOption.nMaxUser = 4;
		pList[2].sOption.nMaxUser = 4;
		pList[3].sOption.nMaxUser = 4;  
		pList[4].sOption.nMaxUser = 4;
	
		pList[5].sOption.nMaxUser = 3;
		pList[6].sOption.nMaxUser = 3;
		pList[7].sOption.nMaxUser = 3;
		pList[8].sOption.nMaxUser = 3;  
		pList[9].sOption.nMaxUser = 3;
	
	    pList[10].sOption.nMaxUser = 2;
		pList[11].sOption.nMaxUser = 2;
		pList[12].sOption.nMaxUser = 2;
		pList[13].sOption.nMaxUser = 2;  
		pList[14].sOption.nMaxUser = 2;
	
		pList[15].sOption.nMaxUser = 4;
		pList[16].sOption.nMaxUser = 3;
		pList[17].sOption.nMaxUser = 2;
		pList[18].sOption.nMaxUser = 4;  
		pList[19].sOption.nMaxUser = 3;
		
	//	AsPartyManager.Instance.ReceivePartyList(pList);
	//		AS_SC_PARTY_DICE_ITEM_INFO data = new AS_SC_PARTY_DICE_ITEM_INFO();
	//		data.nDropItemIdx = 170002;
			//data.sItem = new sITEM();
			
	//	AsPartyManager.Instance.PartyDiceItemInfo(data);
		/*	AS_SC_PARTY_DICE_ITEM_INFO dice = new AS_SC_PARTY_DICE_ITEM_INFO();
			dice.nDropItemIdx = 170002;
			dice.sItem = new sITEM();
			dice.sItem.nItemTableIdx = 170002;
	//		dice.sItem
			AsPartyManager.Instance.PartyDiceItemInfo(dice);
		*/
			
			/*
			 * public eRESULTCODE eResult;
	public UInt32 nUserUniqKey; //Check BlockList 	2013.03.05
	public UInt32 nCharUniqKey;
	public body_SC_CHAT_NAME kName = new body_SC_CHAT_NAME();
	public body_SC_CHAT_MESSAGE kMessage = new body_SC_CHAT_MESSAGE();
			 */
		
		//sEventNotifyMgr.Instance.CenterNotify.AddGMMessage("123456789");
		   
		//	AsChatManager.Instance.InsertGMChat( "1234567890" );
			
		}

        GUILayout.EndArea();
    }	
}
