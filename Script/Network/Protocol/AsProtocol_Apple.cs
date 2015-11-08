using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


enum PROTOCOL_IAP
{
	CI_APPLE_IAP_REGISTER = 1,
    IC_APPLE_IAP_REGISTER_RES,
	CI_APPLE_IAP_UNREGISTER,
	IC_APPLE_IAP_UNREGISTER_RES,  
	CI_APPLE_IAP_START,
	IC_APPLE_IAP_START_RES,
	CI_APPLE_IAP_INFO,
	CI_APPLE_IAP_END,
	IC_APPLE_IAP_END_RES,
	CI_ANDROID_IAP_REGISTER,
	IC_ANDROID_IAP_REGISTER_RES,
	CI_ANDROID_IAP_UNREGISTER,
	IC_ANDROID_IAP_UNREGISTER_RES,
	CI_ANDROID_IAP_START,
	IC_ANDROID_IAP_START_RES,
	CI_ANDROID_IAP_INFO,
	CI_ANDROID_IAP_END,
	IC_ANDROID_IAP_END_RES,
	CI_COUPON_REQ,
	IC_COUPON_RES,
}

//$yde
public enum eCOUPON_TYPE
{
	eCOUPON_TYPE_NOTHING,

	eCOUPON_TYPE_M010,
	eCOUPON_TYPE_M100,

	eCOUPON_TYPE_G010	= 101,
	eCOUPON_TYPE_G100,

	eCOUPON_TYPE_MAX,
};

public class body_CI_APPLE_IAP_REGISTER : AsPacketHeader
{
	public UInt32	nUserUniqKey;
	public UInt32	nCharUniqKey;
	public Int32	nChargeSlot;
	public Byte[]	strIAPProductID = new Byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1];
	
	public body_CI_APPLE_IAP_REGISTER(UInt32 _nUserUniqKey, UInt32 _nCharUniqKey, Int32 _nChargeSlot, Byte[] _strItemID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_REGISTER;
		
		nUserUniqKey = _nUserUniqKey;
		nCharUniqKey = _nCharUniqKey;
		nChargeSlot	= _nChargeSlot;
		
		Array.Copy(_strItemID, strIAPProductID, _strItemID.Length);
	}
}

public class body_CI_APPLE_IAP_UNREGISTER : AsPacketHeader
{
	public body_CI_APPLE_IAP_UNREGISTER()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_UNREGISTER;
	}
}

public class body_CI_APPLE_IAP_START : AsPacketHeader
{
	public UInt32 nUserUniqKey;
	public UInt32 nCharUniqKey;
	public Int32 nChargeSlot;
	public UInt64 nTransactionID;
	public Int32 nQuantity;
	public Byte[] strIAPProductID = new Byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID +1];
	public Int32 nAllReceiptSize;

	public body_CI_APPLE_IAP_START( UInt32 _nUserUniqKey, UInt32 _nCharUniqKey, Int32 _nChargeSlot, UInt64 _nTransactionID, Int32 _nQuantity, Byte[] _strIAPProductID, Int32 _nAllReceiptSize)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_START;

		nUserUniqKey = _nUserUniqKey;
		nCharUniqKey = _nCharUniqKey;
		nChargeSlot = _nChargeSlot;
		nTransactionID = _nTransactionID;
		nQuantity = _nQuantity;
		Array.Copy( _strIAPProductID, strIAPProductID, _strIAPProductID.Length);
	
		nAllReceiptSize = _nAllReceiptSize;
	}
};

public class body_CI_APPLE_IAP_INFO : AsPacketHeader
{
	public Int32 nReceiptSize = 0;
	public Byte[] strReceipt = new Byte[AsGameDefine.MAX_TRANSACTION_DATA_SIZE + 1];

	public body_CI_APPLE_IAP_INFO( Byte[] _strReceipt)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_INFO;

		nReceiptSize = Mathf.Clamp( _strReceipt.Length, 0, AsGameDefine.MAX_TRANSACTION_DATA_SIZE);

		System.Array.Copy( _strReceipt, strReceipt, _strReceipt.Length);
	
		strReceipt[_strReceipt.Length] = 0;
	}
};

public class body_CI_APPLE_IAP_END : AsPacketHeader
{
	public bool bIsTest; //true : test mode, false : real mode

	public byte nPayTool;								// 결제 수단( 일단 필드만 추가)
	public byte[] szItemName;	// 아이템 상품명
	public byte[] szCurrencyCode;	// 화폐코드
	public byte[] szPayCountry;	// 국가
	public float fAmount;
	public int nServerID;
	public uint nUserUniqueID;

	public body_CI_APPLE_IAP_END( bool _isTest)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_END;
		bIsTest = _isTest;
		nPayTool = 0;
		szItemName = new byte[AsGameDefine.MAX_IAPPRODUCT_NAME + 1];
		szCurrencyCode = new byte[AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE + 1];	// 화폐코드
		szPayCountry = new byte[AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE + 1];	// 국가
		fAmount = 0.0f;
	}

	public body_CI_APPLE_IAP_END( bool _isTest, byte _payTool, byte[] _itemName, byte[] _currencyCode, byte[] _payCountry, float _amount, int _serverID, uint _usrUniqueID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_APPLE_IAP_END;
		bIsTest = _isTest;
		nPayTool = _payTool; // 결제 수단( 일단 필드만 추가)
		nServerID = _serverID;
		nUserUniqueID = _usrUniqueID;
		szItemName = new byte[AsGameDefine.MAX_IAPPRODUCT_NAME + 1];
		szCurrencyCode = new byte[AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE + 1];	// 화폐코드
		szPayCountry = new byte[AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE + 1];	// 국가
	
		Array.Copy( _itemName, szItemName, AsGameDefine.MAX_IAPPRODUCT_NAME + 1);
		Array.Copy( _currencyCode, szCurrencyCode, AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE + 1);
		Array.Copy( _payCountry, szPayCountry, AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE + 1);
	
		fAmount = _amount;
	}
};

public class body_IC_APPLE_IAP_START_RES : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public UInt32 nIAPUniqKey = 0;
};

public class body_IC_APPLE_IAP_END_RES : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public UInt32 nIAPUniqKey = 0;
};

public class body_IC_APPLE_IAP_REGISTER_RES : AsPacketHeader
{

};
