//#define REQUEST_DIMPLE_LOG
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using WmWemeSDK.JSON;

public class AsWebRequest : MonoBehaviour
{
	static AsWebRequest m_instance;
	public static AsWebRequest Instance{ get{ return m_instance;}}

//	private string m_strUrl = "http://dimple.cjp.wemade.com:10080/1.0/record/"; // jpn real
//	private string m_strUrl = "http://beta-dimple.cjp.wemade.com:10080/1.0/record/"; // jpn beta
	private JSONObject m_jsonObject = new JSONObject();

	void Awake()
	{
		m_instance = this;
	}

	public void Request_Indun(int nSubKey, uint nUserUniqID, uint nCharacterID, int nIndunTableID, int nIndunBranchTableID)
	{
#if REQUEST_DIMPLE_LOG
	#if ( ( UNITY_IPHONE || UNITY_ANDROID) && ( !UNITY_EDITOR))
		string strDimpleServiceCode = "dungeonlog_c";

		StringBuilder sb = new StringBuilder( AsNetworkDefine.DimpleLog_url);
		sb.Append( WemeManager.getString( "playerKey", WemeManager.Instance.playerKey(), ""));
		sb.Append( '/');
		sb.Append( WemeManager.getString( "gameId", WemeManager.Instance.gameId(), ""));
		sb.Append( '/');
		sb.Append( strDimpleServiceCode);
		sb.Append( '/');
		sb.Append( nSubKey.ToString());

		m_jsonObject.Add( "UserUniqID", nUserUniqID);
		m_jsonObject.Add( "CharacterID", nCharacterID);
		m_jsonObject.Add( "IndunTableID", nIndunTableID);
		m_jsonObject.Add( "IndunBranchTableID", nIndunBranchTableID);

		_RequestPostJSON( sb.ToString());
	#endif
#endif
	}

	public void Request_Purchase(int nSubKey, RuntimePlatform platform, string data = "", string msg ="")
	{
#if REQUEST_DIMPLE_LOG
	#if ( ( UNITY_IPHONE || UNITY_ANDROID) && ( !UNITY_EDITOR))
		string strDimpleServiceCode = "paylog_c";
		
		StringBuilder sb = new StringBuilder( AsNetworkDefine.DimpleLog_url);
		sb.Append( WemeManager.getString( "playerKey", WemeManager.Instance.playerKey(), ""));
		sb.Append( '/');
		sb.Append( WemeManager.getString( "gameId", WemeManager.Instance.gameId(), ""));
		sb.Append( '/');
		sb.Append( strDimpleServiceCode);
		sb.Append( '/');
		sb.Append( nSubKey.ToString());

        m_jsonObject.Add("platform", platform.ToString());
        m_jsonObject.Add("data", data);
        m_jsonObject.Add("Message", msg);

		_RequestPostJSON( sb.ToString());
	#endif
#endif
	}

	// < private function
	private void _RequestPostJSON(string _url)
	{
		byte[] srcRecord = System.Text.UTF8Encoding.UTF8.GetBytes( m_jsonObject.ToString());
		
		_Request( _url, srcRecord);
		
		m_jsonObject.Clear();
	}

	private void _Request(string _url , byte[] _bytes)
	{
//		HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create( _url);
		HttpRequestCreator httpRequestCreator = new HttpRequestCreator();
		Uri _uri = new Uri( _url);
		HttpWebRequest httpRequest = (HttpWebRequest)httpRequestCreator.Create( _uri);

		httpRequest.ContentLength = _bytes.Length;
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json; charset=UTF-8";
		httpRequest.KeepAlive = false;

		Stream requestStream = httpRequest.GetRequestStream();
		requestStream.Write( _bytes, 0, _bytes.Length);
		requestStream.Close();

		HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
		Stream responseStream = httpResponse.GetResponseStream();
		StreamReader streamReader = new StreamReader( responseStream, Encoding.Default);
		string pageContent = streamReader.ReadToEnd();

		Debug.Log( "AsWebRequest::_Request(), url: " + _url);
		Debug.Log( "AsWebRequest::_Request(), response: " + pageContent);

		streamReader.Close();
		responseStream.Close();

		httpResponse.Close();
	}
	// private function >
}

public class HttpRequestCreator : IWebRequestCreate
{
	public WebRequest Create(Uri uri)
	{
		return new HttpWebRequest(uri);	
	}
}
