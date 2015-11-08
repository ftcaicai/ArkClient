using UnityEngine;
using System.Collections.Generic;
using System;


public class body_CI_ANDROID_IAP_REGISTER : AsPacketHeader
{
	public UInt32	nUserUniqKey;
	public UInt32	nCharUniqKey;
	public Int32	nChargeSlot;
	public Byte[]	strIAPProductID = new Byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID + 1];

	public body_CI_ANDROID_IAP_REGISTER(UInt32 _nUserUniqKey, UInt32 _nCharUniqKey, Int32 _nChargeSlot, Byte[] _strItemID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_REGISTER;

		nUserUniqKey = _nUserUniqKey;
		nCharUniqKey = _nCharUniqKey;
		nChargeSlot	= _nChargeSlot;

		Array.Copy(_strItemID, strIAPProductID, _strItemID.Length);
	}
}

public class body_CI_ANDROID_IAP_UNREGISTER : AsPacketHeader
{
	public body_CI_ANDROID_IAP_UNREGISTER()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_UNREGISTER;
	}
};


public class body_CI_ANDROID_IAP_START : AsPacketHeader
{
	public UInt32 nUserUniqKey = 0;
	public UInt32 nCharUniqKey = 0;
	public Int32 nChargeSlot = 0;
	public Byte[] strOrderID = new Byte[AsGameDefine.MAX_ANDROID_IAPORDERID+1];
	public Int32 nQuantity = 0;
	public Byte[] strIAPProductID = new Byte[AsGameDefine.MAX_CHARGE_IAPPRODUCTID+1];
	public Int32 nAllReceiptSize = 0;

	public body_CI_ANDROID_IAP_START( UInt32 _nUserUniqKey, UInt32 _nCharUniqKey, Int32 _nChargeSlot, Byte[] _strOrderID, Int32 _nQuantity, Byte[] _strIAPProductID, Int32 _nAllReceiptSize)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_START;

		nUserUniqKey = _nUserUniqKey;
		nCharUniqKey = _nCharUniqKey;
		nChargeSlot = _nChargeSlot;
		nQuantity = _nQuantity;

		Array.Copy( _strOrderID, strOrderID, _strOrderID.Length);
		Array.Copy( _strIAPProductID, strIAPProductID, _strIAPProductID.Length);

		nAllReceiptSize = _nAllReceiptSize;
	}
};


public class body_CI_ANDROID_IAP_INFO : AsPacketHeader
{
	public Int32 nReceiptSize = 0;
	public Byte[] strReceipt = new Byte[AsGameDefine.MAX_TRANSACTION_DATA_SIZE + 1];

	public body_CI_ANDROID_IAP_INFO( Byte[] _strReceipt)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_INFO;
	
		nReceiptSize = Mathf.Clamp( _strReceipt.Length, 0, AsGameDefine.MAX_TRANSACTION_DATA_SIZE);
	
		System.Array.Copy( _strReceipt, strReceipt, _strReceipt.Length);
	
		strReceipt[_strReceipt.Length] = 0;
	}
};


public class body_CI_ANDROID_IAP_END : AsPacketHeader
{
	public bool bIsTest;
	public byte nPayTool;	// 결제 수단( 일단 필드만 추가)
	public byte[] szItemName;	// 아이템 상품명
	public byte[] szCurrencyCode;	// 화폐코드
	public byte[] szPayCountry;	// 국가
	public float fAmount;
	public int nServerID;
	public uint nUserUniqueID;

	public body_CI_ANDROID_IAP_END( bool _isTest, byte _payTool, byte[] _itemName, byte[] _currencyCode, byte[] _payCountry, float _amount, int _serverID, uint _usrUniqueID)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_END;
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

	public body_CI_ANDROID_IAP_END( bool _isTest)
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_ANDROID_IAP_END;
		bIsTest = _isTest;
		nPayTool = 0;
		szItemName = new byte[AsGameDefine.MAX_IAPPRODUCT_NAME + 1];
		szCurrencyCode = new byte[AsGameDefine.MAX_IAPPRODUCT_CURRENCYCODE + 1];	// 화폐코드
		szPayCountry = new byte[AsGameDefine.MAX_IAPPRODUCT_COUNTRYCODE + 1];	// 국가
		fAmount = 0.0f;
		nServerID = 0;
		nUserUniqueID = 0;
	}
};


public class body_IC_ANDROID_IAP_START_RES : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public UInt32 nIAPUniqKey = 0;
};


public class body_IC_ANDROID_IAP_END_RES : AsPacketHeader
{
	public eRESULTCODE eResult = eRESULTCODE.eRESULT_FAIL;
	public UInt32 nIAPUniqKey = 0;
};


public class body_CI_COUPON_REQ : AsPacketHeader
{
	public UInt32 nUserUniqKey;
	public Int32 nGameCode;
	public byte[] strCouponKey = new byte[AsGameDefine.eCOUPONKEY + 1];
	
	public body_CI_COUPON_REQ()
	{
		Category = (byte)PACKET_CATEGORY._CATEGORY_CI;
		Protocol = (byte)PROTOCOL_IAP.CI_COUPON_REQ;
	}
};


public class body_IC_COUPON_RES : AsPacketHeader
{
	public eRESULTCODE eResult;
};

public class body_IC_ANDROID_IAP_REGISTER_RES : AsPacketHeader
{
	public Int32 nRegisterKey;
};

public class body_IC_ANDROID_IAP_UNREGISTER_RES : AsPacketHeader
{
	public eRESULTCODE eResult;
};