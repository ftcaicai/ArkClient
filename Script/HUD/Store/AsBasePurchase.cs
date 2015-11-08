using UnityEngine;
using System.Collections.Generic;


public enum PurchaseState
{
	NONE,
	REQUEST_P_KEY,
	START_PROCESS_TRANSACTION,
	REQUEST_PRODUCTINFO,
	RESULT_PRODUCTINFO,
	REQUEST_PURCHASE,
	RESULT_PURCHASED,
	REQUEST_CONNECT_IAPA,
	REQUEST_CHECK_RECEIPT,
	REQUEST_INSERT_RECEIPT,
	REQUEST_GET_ITEM,
	REQUEST_CONSUME,
	REQUEST_REGISTER_KEY,
};


public class RequestPurchaseInfo
{
	public string productID;
	public int quantity;
	public int slot;
	public int serverID;
	public uint friendUnique;

	public RequestPurchaseInfo( string _productID, int _quantity, int _slot, int _serverID, uint _friendUnqiueKey)
	{
		productID = _productID;
		quantity = _quantity;
		slot = _slot;
		serverID = _serverID;
		friendUnique = _friendUnqiueKey;
	}
}

public class ProductInfoForPartyTracker
{
	public string itemName;
	public string currencyCode;
	public double price;

	public ProductInfoForPartyTracker() { }

	public ProductInfoForPartyTracker(string _name, string _currencyCode, double _price)
	{
		itemName = _name;
		currencyCode = _currencyCode;
		price = _price;
	}
}


public class WaitTimer
{
	bool stopTimer = false;
	public string displayMsg = string.Empty;
	public float fWaitTime = 0.0f;
	public EndTimerDelegate EndProcess = null;

	public WaitTimer()
	{
		stopTimer = true;
		fWaitTime = 0.0f;
		EndProcess = null;
	}

	public WaitTimer( float _fTime, EndTimerDelegate _EndProcess, string _msg)
	{
		stopTimer = false;
		fWaitTime = _fTime;
		EndProcess = _EndProcess;
		displayMsg = _msg;
	}

	public void StopTimer()
	{
		stopTimer = true;
	}

	public void UpdateTime( float fTimeDelta)
	{
		if( stopTimer == true)
			return;

		fWaitTime -= fTimeDelta;

		if( fWaitTime <= 0.0f)
		{
			stopTimer = true;

			if( EndProcess != null)
			{
				//AsLoadingIndigator.Instance.ShowIndigator( displayMsg);
				Debug.Log( displayMsg);
				EndProcess();
			}
		}
	}
}

public delegate void ProductInfoReceiverDelegate( List<Store_Item_Info_Table> listStoreItemElement);
public delegate void SuccessPurchase( object _purchaseInfo);
public delegate void MessageDelegate( string _szMessage);
public delegate void EndTimerDelegate();
public delegate void voidDelegate();

public class AsBasePurchase : MonoBehaviour
{
	static private AsBasePurchase m_Instance = null;
	static public AsBasePurchase Instance { get { return m_Instance; } }
	
	protected PurchaseState nowPurchaseState = PurchaseState.NONE;
	protected RequestPurchaseInfo reqPurchaseInfo = null;
	protected ArkTransaction nowArkTransaction = null;
	protected SortedDictionary<int, Store_Item_Info_Table> dicStoreItemElement = new SortedDictionary<int, Store_Item_Info_Table>();
	protected List<Store_Item_Info_Table> listStoreItemElement = new List<Store_Item_Info_Table>();
	protected Dictionary<int, string> dicProductName = new Dictionary<int, string>();
	protected Dictionary<int, string> dicProductPrice = new Dictionary<int, string>();
	protected Dictionary<int, string> dicProductID = new Dictionary<int, string>();
	protected List<string> listRequestID = new List<string>();
	protected bool bCanPayments = false;
	protected WaitTimer waitTimer = new WaitTimer();
	protected AsMessageBox progressMessageBox = null;
	protected string progressMessage = string.Empty;
	
	// delegate
	public ProductInfoReceiverDelegate ProductInfoListener;
	public MessageDelegate MessageListener;
	public MessageDelegate MessageListenerForFail;
	public SuccessPurchase SuccessPurchaseListener;
	public voidDelegate FailPurchase;
	public voidDelegate InitSuccessPurcahseListender;
	public voidDelegate ProcessAfterGetProduct;
	public voidDelegate CloseCashStore;
	
	// virtual funcs
	public virtual string GetProductName( int _uniqueKey) { return string.Empty; }
	public virtual string GetProductPrice( int _uniqueKey) { return string.Empty; }
	public virtual bool InitPurchase() { return false;}
	public virtual void RequestProductInfos( string[] _szProductIDs) { }
	public virtual void PurchaseProduct( string _productID, int _count, int _slot, int _serverID, uint _userUnique) { }
	public virtual void PurchaseProduct(string _productID, int _count, uint _usrUniqueKey, int _slot, int _serverID, uint _friendUniqueKey, int _registerKey) { }
	public virtual void RequestItemRegisterKey(uint _usrUnique, uint _charUnique, int _charSlot, string _itemID) { }
	public virtual void RequestItemUnRegister() { }
	public virtual bool ConnectIAPA() { return false; }
	public virtual bool StartGetItemProcess( ArkTransaction _transaction) {return false; }
	public virtual bool StartGetItemProcess() {return false; }
	public virtual void SendCheckReceipt( ArkTransaction _arkTransaction) { }
	public virtual void RequestInsertReceipt() { }
	public virtual void OrganizeTransaction(bool _isSuccessPurchase) { }
	public virtual void RequestGetCashItem() { }
	public virtual void CheckUnProcessedArkTransaction() { }
	
	public void MakeTimerForGetPKey( EndTimerDelegate _eventDelegate) { waitTimer = new WaitTimer( 60, _eventDelegate, "GetPKey"); Debug.Log( "Get P Key");}
	public void MakeTimerForCheckReceipt() { waitTimer = new WaitTimer( 50, SendCheckReceipt , "Re Send Check Receipt"); Debug.Log( "timer check Receipt"); }
	public void MakeTimerForInsertReceipt() { waitTimer = new WaitTimer( 50, RequestInsertReceipt, "Re Rquest Insert Receipt"); Debug.Log( "timer insert Receipt"); }
	public void MakeTimerForGetCashItemRequest(){ waitTimer = new WaitTimer( 50, RequestGetCashItem , "Re Request Get Cash Item"); Debug.Log( "timer GetItemRequest"); }
	public void StopTimer() { if( waitTimer != null) waitTimer.StopTimer(); }
	public bool IsUnHandledTransaction()
	{
		if (nowArkTransaction == null)
			return false;

		return nowArkTransaction.isUnHandled;
	}

	public bool IsRetryConnectIAPAServer()
	{
		if (nowArkTransaction == null)
			return false;

		return nowArkTransaction.isRetryConnectIAPAServer;
	}

	void Awake()
	{
		m_Instance = this;
	}

	void OnDestroy()
	{
		m_Instance = null;
	}

	public void SetItemUniqueID( uint _itemUniqueID)
	{
		if( nowArkTransaction != null)
			nowArkTransaction.itemUniqueID = _itemUniqueID;
		else
			Debug.Log( "nowArkTransaction is null");
	}

	public void ResetProductData()
	{
		listStoreItemElement.Clear();
		dicStoreItemElement.Clear();
		dicProductName.Clear();
		dicProductPrice.Clear();
		dicProductID.Clear();
	}

	public List<Store_Item_Info_Table> GetStoreItemList()
	{
		if (AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			return listStoreItemElement;
		else
		{
			List<Store_Item_Info_Table> returnList = new List<Store_Item_Info_Table>();

			foreach (Store_Item_Info_Table info in listStoreItemElement)
			{
				Tbl_ChargeRecord record =  AsTableManager.Instance.GetChargeRecord(info.ID_String);

				if (record.chargeType == eCHARGETYPE.eCHARGETYPE_DAILY)
					continue;

				returnList.Add(info);
			}

			return returnList;
		}

		return listStoreItemElement;
	}

	public List<byte[]> JasonToByteList( string _json)
	{
		List<byte[]> listDataPiece = new List<byte[]>();

		try
		{
			byte[] data = System.Text.ASCIIEncoding.Default.GetBytes( _json);
			int totalCount = data.Length;
	
			if( totalCount <= 0)
				return listDataPiece;

			if( totalCount <= 2000)
			{
				listDataPiece.Add( data);
			}
			else
			{
				int idx = 0;
				int remainCount = totalCount;

				while( remainCount != 0)
				{
					int length = System.Math.Min( 2000, remainCount);
					byte[] dataPiece = new byte[length];
					System.Array.Copy( data, idx, dataPiece, 0, length);
					idx += length;
					remainCount -= length;
	
					listDataPiece.Add( dataPiece);
				}
			}
		}
		catch( System.Exception e)
		{
			Debug.Log( e.Message);
		}
	
		return listDataPiece;
	}
	
	/*
	// dont't used
	public void MakeMessageBox()
	{
		if( progressMessageBox == null)
		{
			progressMessageBox = AsNotify.Instance.MessageBox( string.Empty, string.Empty, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			progressMessageBox.gameObject.SetActiveRecursively( false);
		}
	}

	public void SetProgressMessageBox( string _msg)
	{
		if( progressMessageBox != null)
		{
			progressMessageBox.gameObject.SetActiveRecursively( true);
			progressMessageBox.SetStyle( AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
	
			Debug.Log( _msg);
			progressMessageBox.SetMessage( _msg);
		}
		else
			Debug.Log( "No init MessageBox:" + _msg);
	}

	public void CloseProgressMessageBox()
	{
		if( progressMessageBox != null)
			progressMessageBox.gameObject.SetActiveRecursively( false);
	}
	*/

	public void SendCheckReceipt()
	{
		SendCheckReceipt( nowArkTransaction);
	}

	public void DisconnectIAPA()
	{
#if UNITY_IPHONE || UNITY_ANDROID
        AsWebRequest.Instance.Request_Purchase(190102, Application.platform, "", "Dis connect IAPA");
#endif
		AsNetworkIAPManager.Instance.DisConnect();
	}

	public void ShowLoadingIndigator( string _msg)
	{
		if( AsCashStore.Instance != null)
			AsCashStore.Instance.ShowLoadingIndigator( _msg);
	}

	public void HideLoadIndigator()
	{
		if( AsCashStore.Instance != null)
			AsCashStore.Instance.HideLoadingIndigator();
	}

	public virtual ProductInfoForPartyTracker GetItemInfoForPartyTracker(string _productID)
	{
		return new ProductInfoForPartyTracker();
	}
}
