
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IosPurchaseManager : AsBasePurchase
{
#if UNITY_IPHONE
    List<StoreKitProduct> productList    = new List<StoreKitProduct>();
	Dictionary<int, StoreKitProduct>  dicProduct = new Dictionary<int, StoreKitProduct>();
    List<string> listRequestedProductID  = new List<string>();
	void OnEnable()
    {
        #region -Attach Event
        // Listens to all the StoreKit events.  All event listeners MUST be removed before this object is disposed!
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
		StoreKitManager.purchaseFailedEvent += purchaseFailed;
		//StoreKitManager.receiptValidationFailedEvent += receiptValidationFailed;
		//StoreKitManager.receiptValidationRawResponseReceivedEvent += receiptValidationRawResponseReceived;
		//StoreKitManager.receiptValidationSuccessfulEvent += receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
        #endregion
    }
	
	void OnDisable()
    {
        #region -Detach event
        // Remove all the event handlers
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
		StoreKitManager.purchaseFailedEvent -= purchaseFailed;
		//StoreKitManager.receiptValidationFailedEvent -= receiptValidationFailed;
		//StoreKitManager.receiptValidationRawResponseReceivedEvent -= receiptValidationRawResponseReceived;
		//StoreKitManager.receiptValidationSuccessfulEvent -= receiptValidationSuccessful;
		StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
		StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailed;
		StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinished;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
        #endregion
    }

    void Update()
    {
        if (bCanPayments && listRequestID.Count > 0)
        {
            StoreKitBinding.requestProductData(listRequestID.ToArray());
            listRequestID.Clear();
        }

        if (waitTimer != null)
            waitTimer.UpdateTime(Time.deltaTime);
    }

	void productListReceivedEvent( List<StoreKitProduct> _productList )
	{
        nowPurchaseState = PurchaseState.RESULT_PRODUCTINFO;
        productList.Clear();
		dicProduct.Clear();
        productList = _productList;
        ResetProductData();

        foreach (StoreKitProduct product in productList)
        {
            int uniqueID = listRequestedProductID.IndexOf(product.productIdentifier);
            Store_Item_Info_Table storeItem = new Store_Item_Info_Table(Store_Item_Type.ChargeItem, uniqueID, product.productIdentifier, 1);
            dicStoreItemElement.Add(uniqueID, storeItem);
            dicProductName.Add(uniqueID, product.title);
            dicProductID.Add(uniqueID, product.productIdentifier);
            dicProductPrice.Add(uniqueID, product.formattedPrice);
			dicProduct.Add(uniqueID, product);
        }

        if (dicStoreItemElement.Count > 0)
        {
            listStoreItemElement.AddRange(dicStoreItemElement.Values);
        }

        if (ProductInfoListener != null)
        {
           // ProductInfoListener(listStoreItemElement);
        }

        AsLoadingIndigator.Instance.HideIndigator();

        CheckUnProcessedArkTransaction();
	}
	
	
	void productListRequestFailed( string error )
	{
        nowPurchaseState = PurchaseState.NONE;

        if (MessageListener != null)
            MessageListener("(IOS)productListRequestFailed:" + error);
        else
            Debug.Log("FailRecieveProductListener is null" + error);

        AsLoadingIndigator.Instance.HideIndigator();
	}

    #region -not use
	//void receiptValidationSuccessful()
	//{
	//    Debug.Log( "receipt validation successful" );
	//}
	
	
	//void receiptValidationFailed( string error )
	//{
	//    Debug.Log( "receipt validation failed with error: " + error );
	//}
	
	
	//void receiptValidationRawResponseReceived( string response )
	//{
	//    Debug.Log( "receipt validation raw response: " + response );
	//}

    void restoreTransactionsFailed(string error)
    {
        if (MessageListener != null)
            MessageListener("restoreTransactionsFailed: " + error);
    }


    void restoreTransactionsFinished()
    {
        if (MessageListener != null)
            MessageListener("restoreTransactionsFinished");
    }
    #endregion

    void purchaseFailed( string error )
	{
        if (MessageListener != null)
            MessageListener("(IOS):" + error);
        else
            Debug.Log("FailPurchaseListener is null" + error);

        AsLoadingIndigator.Instance.HideIndigator();

		if (ProcessAfterGetProduct != null)
			ProcessAfterGetProduct();
	}
	
	
	void purchaseCancelled( string error )
	{
        nowPurchaseState = PurchaseState.NONE;

        AsLoadingIndigator.Instance.HideIndigator();

        if (FailPurchase != null)
            FailPurchase();
	}
	
	
	void productPurchaseAwaitingConfirmationEvent( StoreKitTransaction transaction )
	{
		bool bStartGetItemProcess = dicProduct != null ? dicProduct.Count >= 1 : false;

		SaveTransaction(transaction, bStartGetItemProcess);
	}

	public void SaveTransaction(StoreKitTransaction _transaction, bool _StartGetItemProcess)
	{

		Debug.LogWarning("Save Tran");

		uint chaUniqueKey = 0;
		
		if ( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			chaUniqueKey = AsUserInfo.Instance.GetCurrentUserCharacterInfo().nCharUniqKey;

		int serverID = AsUserInfo.Instance.CurrentServerID;

		int slot = AsTableManager.Instance.GetChargeRecordSlotNumber(_transaction.productIdentifier);

		if (slot == -1)
			return;
		
		ArkTransaction transaction = new ArkTransaction(string.Empty,
		                                                string.Empty, 
		                                                _transaction.productIdentifier,
		                                                _transaction.transactionIdentifier,
		                                                _transaction.base64EncodedTransactionReceipt,
		                                                slot,
		                                                _transaction.quantity,
		                                                chaUniqueKey,
			                                            serverID,
		                                                0,
		                                                0); // register key

		transaction.WriteToFile();
		
		if (_StartGetItemProcess == true)
		{
			StartGetItemProcess(transaction);
			
			nowPurchaseState = PurchaseState.RESULT_PURCHASED;
		}
	}
	
	
	void purchaseSuccessful( StoreKitTransaction transaction )
	{
		Debug.Log( "success" );

       // SaveTransaction(transaction);
	}	
	
	void paymentQueueUpdatedDownloadsEvent( List<StoreKitDownload> downloads )
	{
		foreach( var dl in downloads )
			Debug.Log( dl );
	}

    public override void RequestProductInfos(string[] _szProductIDs)
    {
        nowPurchaseState = PurchaseState.REQUEST_PRODUCTINFO;
        listRequestID.Clear();
        listRequestID.AddRange(_szProductIDs);

        listRequestedProductID.Clear();
        listRequestedProductID.AddRange(_szProductIDs);
    }

	public override void RequestItemRegisterKey(uint _usrUnique, uint _charUnique, int _itemSlot, string _itemID)
	{
		byte[] productID = System.Text.ASCIIEncoding.ASCII.GetBytes(_itemID);

		body_CI_APPLE_IAP_REGISTER data = new body_CI_APPLE_IAP_REGISTER(_usrUnique, _charUnique, _itemSlot, productID);

		if (ConnectIAPA() == true)
		{
			AsCommonSender.SendToIAP(data.ClassToPacketBytes());
            AsWebRequest.Instance.Request_Purchase(190103, Application.platform, "", "Req Register Key");
		}
	}

	public override void RequestItemUnRegister ()
	{
		body_CI_APPLE_IAP_UNREGISTER data = new body_CI_APPLE_IAP_UNREGISTER();
		
		if (ConnectIAPA() == true)
		{
			AsCommonSender.SendToIAP(data.ClassToPacketBytes());
            AsWebRequest.Instance.Request_Purchase(190104, Application.platform, "", "Req UnRegister Key");
		}
	}

    public override void PurchaseProduct(string _productID, int _count, int _slot, int _serverID, uint _friendUnique)
    {
        // fix count 1
		reqPurchaseInfo  = new RequestPurchaseInfo(_productID, 1, _slot, _serverID, _friendUnique);
        nowPurchaseState = PurchaseState.REQUEST_PURCHASE;

		AsCommonSender.SendBackgroundProc(true);

        StoreKitBinding.purchaseProduct(_productID, 1);

        ShowLoadingIndigator("");
    }

    public override bool InitPurchase()
    {
        bCanPayments = StoreKitBinding.canMakePayments();

        return bCanPayments;
    }

    public override string GetProductName(int _uniqueKey) 
    {
        if (dicProductName.ContainsKey(_uniqueKey))
            return dicProductName[_uniqueKey];
        else
            return _uniqueKey + "is not exist";
    }

    public override string GetProductPrice(int _uniqueKey) 
    {
        if (dicProductPrice.ContainsKey(_uniqueKey))
            return dicProductPrice[_uniqueKey];
        else
            return _uniqueKey + "is not exist";
    }

    public override bool ConnectIAPA()
    {
        nowPurchaseState = PurchaseState.REQUEST_CONNECT_IAPA;

		// already connected
		if (AsNetworkIAPManager.Instance.IsConnected() == true)
			return true;

    
#if UNITY_IPHONE || UNITY_ANDROID
        AsWebRequest.Instance.Request_Purchase(190101, Application.platform, "", "Connect IAPA");
#endif
		bool value = AsNetworkIAPManager.Instance.ConnectToServer(AsNetworkDefine.IAPAGENT_SERVER_IP, AsNetworkDefine.IAPAGENT_SERVER_PORT, IAP_SOCKET_STATE.ISS_CONNECT);

		return value;
		

    }

    public override void RequestGetCashItem()
    {
        if (nowArkTransaction != null)
        {
			nowPurchaseState = PurchaseState.REQUEST_GET_ITEM;

            AsCommonSender.SendRequestBuyChargeItem(eCHARGECOMPANYTYPE.eCHARGECOMPANYTYPE_APPLE, nowArkTransaction.slot, nowArkTransaction.productID, nowArkTransaction.itemUniqueID);

            AsWebRequest.Instance.Request_Purchase(190109, Application.platform, nowArkTransaction.productID, "Req Get Cash Item");

            MakeTimerForGetCashItemRequest();
        }
    }

    public override void SendCheckReceipt(ArkTransaction _arkTransaction)
    {
        byte[] productID = System.Text.ASCIIEncoding.ASCII.GetBytes(_arkTransaction.productID);


        uint chracterUniqueKey = 0;
        
        if (AsCashStore.Instance != null)
        {
            eCHARGETYPE type =  AsCashStore.Instance.GetChargeType(_arkTransaction.productID);

            if (type == eCHARGETYPE.eCHARGETYPE_GOLD)
            {
                chracterUniqueKey = _arkTransaction.characterUniqueKey;
    
            }
        }


        body_CI_APPLE_IAP_START data = new body_CI_APPLE_IAP_START(AsUserInfo.Instance.LoginUserUniqueKey,
                                                                   chracterUniqueKey,
                                                                   _arkTransaction.slot,
                                                                   System.Convert.ToUInt64(_arkTransaction.transactionIdentifier),
                                                                   _arkTransaction.quantity,
                                                                   productID,
                                                                   _arkTransaction.GetSize());

		AsCommonSender.SendToIAP(data.ClassToPacketBytes());

        AsWebRequest.Instance.Request_Purchase(190105, Application.platform, "", "Req Check Receipt");

        nowPurchaseState = PurchaseState.REQUEST_CHECK_RECEIPT;

        MakeTimerForCheckReceipt();

  
    }

	public override void OrganizeTransaction(bool _isSuccessPurchase)
    {
        nowPurchaseState = PurchaseState.NONE;

        if (ArkTransaction.DeleteArkTransactionFile(nowArkTransaction.transactionIdentifier))
        {
			Debug.LogWarning("OrganizeTransaction");

			 string message = string.Format(AsTableManager.Instance.GetTbl_String(376), GetProductName(nowArkTransaction.slot));

			 // check friend
             if (nowArkTransaction.friendUniqueKey != 0)
                 message = AsTableManager.Instance.GetTbl_String(1719);

			 // check daily
			 Tbl_ChargeRecord chargeRecord = AsTableManager.Instance.GetChargeRecord(nowArkTransaction.productID);
			 if (chargeRecord != null)
			 {
				 if (chargeRecord.chargeType == eCHARGETYPE.eCHARGETYPE_DAILY)
					 message = AsTableManager.Instance.GetTbl_String(1795);
			 }

			if (_isSuccessPurchase == false)
				message = AsTableManager.Instance.GetTbl_String(2361);

			// pending transaction
			StoreKitBinding.finishPendingTransaction(nowArkTransaction.transactionIdentifier);

        	AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(1538), message, AsNotify.MSG_BOX_TYPE.MBT_OK);

	       	nowPurchaseState = PurchaseState.NONE;

            if (SuccessPurchaseListener != null)
                SuccessPurchaseListener(nowArkTransaction);

            nowArkTransaction = null;
        }
        else
        {
            AsLoadingIndigator.Instance.HideIndigator();
            StartGetItemProcess(nowArkTransaction);
        }
    }

    public override void RequestInsertReceipt()
    {
        if (nowArkTransaction == null)
            return;

        StopTimer();

        nowPurchaseState = PurchaseState.REQUEST_INSERT_RECEIPT;

        List<byte[]> listDataPiece = JasonToByteList(nowArkTransaction.json);

        // send
        foreach (byte[] bytes in listDataPiece)
        {
            body_CI_APPLE_IAP_INFO start = new body_CI_APPLE_IAP_INFO(bytes);
            AsCommonSender.SendToIAP(start.ClassToPacketBytes());
        }
		
		body_CI_APPLE_IAP_END end = null;

		if (dicProduct.ContainsKey(nowArkTransaction.slot))
        {
            byte[] nameArray         = new byte[AsGameDefine.MAX_IAPPRODUCT_NAME         +1];
            byte[] currencyCodeArray = new byte[AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE +1];
            byte[] countryCodeArray  = new byte[AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE  +1];
			
			int uniqueID = nowArkTransaction.slot;
			
            byte[] nameArraySrc = System.Text.UTF8Encoding.UTF8.GetBytes(dicProduct[uniqueID].title);
            Array.Copy(nameArraySrc, nameArray, nameArraySrc.Length);
            nameArray[nameArraySrc.Length] = 0;
			
            byte[] currencyCodeArraySrc = System.Text.UTF8Encoding.UTF8.GetBytes(dicProduct[uniqueID].currencyCode);
            Array.Copy(currencyCodeArraySrc, currencyCodeArray, AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE);
            currencyCodeArray[AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE] = 0;
			
            byte[] countryCodeArraySrc = System.Text.UTF8Encoding.UTF8.GetBytes(dicProduct[uniqueID].countryCode);
            Array.Copy(countryCodeArraySrc, countryCodeArray, AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE);
            countryCodeArray[AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE] = 0;
            
            end = new body_CI_APPLE_IAP_END(true, 0, nameArray, currencyCodeArray, countryCodeArray, System.Convert.ToSingle(dicProduct[uniqueID].price), nowArkTransaction.serverID, nowArkTransaction.friendUniqueKey);

        }
        else
        {
             end = new body_CI_APPLE_IAP_END(true);
        }
		
        AsCommonSender.SendToIAP(end.ClassToPacketBytes());

        AsWebRequest.Instance.Request_Purchase(190107, Application.platform, "", "Request Insert Receipt");

		Debug.Log("************* [5] Request Insert Receipt");

        MakeTimerForInsertReceipt();
    }

    public override bool StartGetItemProcess()
    {
        if (nowArkTransaction != null)
            return StartGetItemProcess(nowArkTransaction);
        else
            return false;
    }

    // start
    public override bool StartGetItemProcess(ArkTransaction _transaction)
    {
        Debug.Log("StartGetItemProcess()");

        nowPurchaseState = PurchaseState.START_PROCESS_TRANSACTION;

        nowArkTransaction = _transaction;

		if (AsNetworkIAPManager.Instance.IsConnected() == true)
		{
			nowArkTransaction.isRetryConnectIAPAServer = false;

			SendCheckReceipt(nowArkTransaction);

			if (nowArkTransaction.isUnHandled == false)
				ShowLoadingIndigator(AsTableManager.Instance.GetTbl_String(375));
		}
        else
        {
			StartCoroutine("ReConnectIAPAServerProcess");
        }

		return true;
    }

	IEnumerator ReConnectIAPAServerProcess()
	{
		yield return new WaitForSeconds(5.0f);

		if (nowArkTransaction.isRetryConnectIAPAServer == false)
		{
			nowArkTransaction.isRetryConnectIAPAServer = true;
			nowArkTransaction.SetRetryConnectServerCount();
			RequestItemRegisterKey(AsUserInfo.Instance.LoginUserUniqueKey, nowArkTransaction.characterUniqueKey, nowArkTransaction.slot, nowArkTransaction.productID);
		}
		else
		{
			nowArkTransaction.DiscountRetryConnectCount();

			if (nowArkTransaction.isRetryConnectIAPAServer == false) // end Connect
				CloseCashStore();
			else
				RequestItemRegisterKey(AsUserInfo.Instance.LoginUserUniqueKey, nowArkTransaction.characterUniqueKey, nowArkTransaction.slot, nowArkTransaction.productID);
		}
	}

    public override void CheckUnProcessedArkTransaction()
    {
        List<ArkTransaction> listRemain = new List<ArkTransaction>();
		
		sCHARVIEWDATA characterData = AsUserInfo.Instance.GetCurrentUserCharacterInfo();
		
		if (characterData != null)
			listRemain = ArkTransaction.GetAllTransaction(characterData.nCharUniqKey);

		if (listRemain.Count >= 1)
		{
			int slotNum = AsTableManager.Instance.GetChargeRecordSlotNumber(listRemain[0].productID);

			if (slotNum == -1)
				NotProcceUnProccedTransaction();
			else
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126),
				                             string.Format(AsTableManager.Instance.GetTbl_String(2360),
				              				 dicProduct[slotNum].title),
				                             AsTableManager.Instance.GetTbl_String(2361),
				                             AsTableManager.Instance.GetTbl_String(1027),
				                             this,
				                             "ProcessUnPorccessedTransaction",
				                             "NotProcceUnProccedTransaction",
				                             AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL,
				                             AsNotify.MSG_BOX_ICON.MBI_QUESTION);                       
			}
		}
		else
			NotProcceUnProccedTransaction();
    }

	void ProcessUnPorccessedTransaction()
	{
		List<ArkTransaction> listRemain = new List<ArkTransaction>();
				
		sCHARVIEWDATA characterData = AsUserInfo.Instance.GetCurrentUserCharacterInfo();
				
		if (characterData != null)
			listRemain = ArkTransaction.GetAllTransaction(characterData.nCharUniqKey);

		if (listRemain.Count >= 1)
		{
			// get character key
			UInt32 charUniqueKey = 0;
			
			if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
				charUniqueKey = AsUserInfo.Instance.GetCurrentUserCharacterInfo().nCharUniqKey;

            AsWebRequest.Instance.Request_Purchase(190110, Application.platform, "", "Un Handled Transaction");
			
			RequestItemRegisterKey(AsUserInfo.Instance.LoginUserUniqueKey, charUniqueKey, listRemain[0].slot, listRemain[0].productID);
			
			nowArkTransaction = listRemain[0];
			nowArkTransaction.isUnHandled = true;
			
		}
		else
		{
			NotProcceUnProccedTransaction();
		}
	}

	void NotProcceUnProccedTransaction()
	{
		HideLoadIndigator();
		nowPurchaseState = PurchaseState.NONE;
		
		if (ProcessAfterGetProduct != null)
			ProcessAfterGetProduct();
	}

	public override ProductInfoForPartyTracker GetItemInfoForPartyTracker (string _productID)
	{
		foreach(StoreKitProduct storekit in productList)
		{
			if (storekit.productIdentifier == _productID)
				return new ProductInfoForPartyTracker(storekit.title, storekit.currencyCode, double.Parse(storekit.price));
		}
		return null;
	}
    
#endif
}