using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class AsNetworkMessageHandler : MonoBehaviour
{
	private static AsNetworkMessageHandler instance = null;
	private Dictionary<PACKET_CATEGORY, AsProcessBase> categoryList = new Dictionary<PACKET_CATEGORY, AsProcessBase>();

	public static AsNetworkMessageHandler Instance
	{
		get { return instance; }
	}

	public void OnApplicationQuit()
	{
		instance = null;
	}

	void Awake()
	{
		instance = this;
	
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CL, GetComponent<AsLoginProcess>() as AsProcessBase);
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CS, GetComponent<AsCommonProcess>() as AsProcessBase);
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CG, GetComponent<AsGameProcess>() as AsProcessBase);
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CS2, GetComponent<AsCommonProcess_2>() as AsProcessBase);
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CS3, GetComponent<AsCommonProcess_3>() as AsProcessBase);
		categoryList.Add( PACKET_CATEGORY._CATEGORY_CI, GetComponent<AsIAPProcess>() as AsProcessBase);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Send( byte[] data)
	{

		_Debug (data, "Send ");

		AsNetworkManager.Instance.AddSend( data);
	}

	public void MessageToCategory( byte[] packet)
	{
		_Debug (packet, "Recv ");
		// 서버와의 데이터오더 문제로 2이어야 하나 3으로 해야 Category가 얻어짐
		categoryList[(PACKET_CATEGORY)packet[3]].Process( packet );
	}

	private void _Debug (byte[] data, string f) {
		byte Protocol = data [2];
		PACKET_CATEGORY Cat = (PACKET_CATEGORY)data [3];
		string sPro = "";
		switch (Cat) {
		case PACKET_CATEGORY._CATEGORY_CL:
		{
			sPro = ((PROTOCOL_LOGIN)Protocol).ToString();
		}
			break;
		case PACKET_CATEGORY._CATEGORY_CS:
		{
			sPro = ((PROTOCOL_CS)Protocol).ToString();
		}
			break;
		case PACKET_CATEGORY._CATEGORY_CG:
		{
			sPro = ((PROTOCOL_GAME)Protocol).ToString();
		}
			break;
		case PACKET_CATEGORY._CATEGORY_CS2:
		{
			sPro = ((PROTOCOL_CS_2)Protocol).ToString();
		}
			break;
		case PACKET_CATEGORY._CATEGORY_CS3:
		{
			sPro = ((PROTOCOL_CS_3)Protocol).ToString();
		}
			break;
		case PACKET_CATEGORY._CATEGORY_CI:
		{
			sPro = ((PROTOCOL_IAP)Protocol).ToString();
		}
			break;
		}
		
		Debug.LogWarning (string.Format("==================== NET WORK {2}:  {0}:{1}", sPro, Cat.ToString(), f));
	}
}
