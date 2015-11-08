//#define _USE_PATCH_FINISH_BTN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum eAppPlatformType
{
	eAppPlatformType_none = 0,
	eAppPlatformType_IPhonePlayer,
	eAppPlatformType_Android,
	eAppPlatformType_WindowsPlayer
}

public enum ePatchGroup
{
	ePatchGroup_Invalid = 0,
	ePatchGroup_1,
	ePatchGroup_2,
	ePatchGroup_3,
	ePatchGroup_4,
	ePatchGroup_5,
	ePatchGroup_Max
}

public enum eAssetbundleDataType
{
	eAssetbundleDataType_None = 0,
	eAssetbundleDataType_Scene,
	eAssetbundleDataType_ItemPrefab,
	eAssetbundleDataType_ItemTexture,
	eAssetbundleDataType_MonsterPrefab,
	eAssetbundleDataType_MonsterTexture,
	eAssetbundleDataType_NpcPrefab,
	eAssetbundleDataType_NpcTexture,
	eAssetbundleDataType_Pc,
	eAssetbundleDataType_Text,
	eAssetbundleDataType_Sound,
	eAssetbundleDataType_UITexture,
	
	eAssetbundleDataType_Max
}

public class AssetbundleManager : MonoBehaviour
{
	public struct stDownloadData
	{
		public string _strName;
		public int _nVersion;
		public long _lSize;
		
		public stDownloadData(string strName, int nVersion, long lSize)
		{
			_strName = strName;
			_nVersion = nVersion;
			_lSize = lSize;
		}
	}
	
	static AssetbundleManager m_instance;
	public static AssetbundleManager Instance{ get{  return m_instance;}}
	
	public UIProgressBar m_AssetbundleProgress = null;
	public SpriteText m_AssetbundleDownInfoText = null;
	public SimpleSprite m_PatcherBG = null;
	public SimpleSprite m_PatcherImg = null;
	public SpriteText m_PatcherText = null;
	public SpriteText m_LoadingTip = null;
	public UIButton m_btnPatchFinished = null;
	
	public UIButton m_btnWebview = null;
	
	private eAppPlatformType m_eAppFlatformType = eAppPlatformType.eAppPlatformType_none;
	private Dictionary<string, XmlNode> m_dAssetbundleTable = new Dictionary<string, XmlNode>();
	private Dictionary<string, XmlNode> m_dAssetbundleTable_c = new Dictionary<string, XmlNode>();
	private int m_nAssetbundleTableVersion_c = 0;
	private AssetbundleLoader m_AssetbundleLoader = new AssetbundleLoader();
	private AssetbundleParser m_AssetbundleParser;// = new AssetbundleParser(); kij
	private List<stDownloadData> m_listDownLoad = new List<stDownloadData>();
	private bool m_bDownLoaded = false;
	private bool m_bUseAssetbundle = false;
	private bool m_bDownloadStart = false;
	private long m_lDownloadingTotalMaxSize = 0;
	private long m_lDownloadingTotalCurSize = 0;
	private byte[] m_encodedStringBuf;
//	private string m_strVersionFolderName = string.Empty;
	private int MAX_ASYNCDOWNCOUNT = 10;
	private AsMessageBox m_msgboxAppUpdate = null;
	private AsMessageBox m_msgboxPatchChoice = null;
	private AsMessageBox m_msgboxPatchError = null;
	private Dictionary<string, int> m_dAssetbundleVersion = new Dictionary<string, int>();
	private float m_fMsgBoxTimer = 0.0f;
	private Dictionary<string, XmlNode> m_dPatchGroup_2 = new Dictionary<string, XmlNode>();
	private Dictionary<string, XmlNode> m_dPatchGroup_3 = new Dictionary<string, XmlNode>();
	private Dictionary<string, XmlNode> m_dPatchGroup_4 = new Dictionary<string, XmlNode>();
	private Dictionary<string, XmlNode> m_dPatchGroup_5 = new Dictionary<string, XmlNode>();
	private bool m_bUpdatePatchSnapshot = false;
	private float m_fPatchSnapshotTimer = 0.0f;
	private ePatchGroup m_eCurPatchGroup = ePatchGroup.ePatchGroup_Invalid;
	private int m_nCurPatchGroupImgIndex = 0;
	private int m_nCurPatchGroupTextIndex = 0;
	private bool m_bSendWarpCancel = false;
	private bool m_bPatchFinishedBtnOk = false;

	public bool bLoaded { get{  return m_bDownLoaded;}}
	public bool useAssetbundle { get{  return m_bUseAssetbundle;}}
	public bool bPatchFinishedBtnOk { get{ return m_bPatchFinishedBtnOk;}}
	public bool bSendWarpCancel
	{
		set { m_bSendWarpCancel = value;}
		get { return m_bSendWarpCancel;}
	}
	
	void Awake()
	{
		m_instance = this;
		// begin kij		
		m_AssetbundleParser = gameObject.AddComponent<AssetbundleParser>(); 
		// end kij

		GameObject main = GameObject.Find( "GameMain");
		AsGameMain asMain = main.GetComponent<AsGameMain>();
		m_bUseAssetbundle = asMain.GetUseAssetbundle();
	}
	
	void Start()
	{

		if( RuntimePlatform.IPhonePlayer == Application.platform)
			m_eAppFlatformType = eAppPlatformType.eAppPlatformType_IPhonePlayer;
		else if( RuntimePlatform.Android == Application.platform)
			m_eAppFlatformType = eAppPlatformType.eAppPlatformType_Android;
		else if( RuntimePlatform.WindowsPlayer == Application.platform)
			m_eAppFlatformType = eAppPlatformType.eAppPlatformType_WindowsPlayer;
		else
			m_eAppFlatformType = eAppPlatformType.eAppPlatformType_WindowsPlayer;


		m_eAppFlatformType = eAppPlatformType.eAppPlatformType_Android;
		
		m_bDownLoaded = false;
		m_bPatchFinishedBtnOk = false;
		m_lDownloadingTotalMaxSize = 0;
		m_lDownloadingTotalCurSize = 0;
		SetActiveAssetbundleInfo( false);

		m_AssetbundleParser.LoadTable();
		_CreateMsgBox();
		m_btnPatchFinished.Text = GetPatcherString( 61);
		#region --webview
		m_btnWebview.Text = GetPatcherString( 62);
		m_btnWebview.SetInputDelegate( OnClickWebview );
		#endregion
		
#if !_USE_PATCH_FINISH_BTN
		_SetActivePatchFinishedButton( false);
		m_bPatchFinishedBtnOk = true;
#endif
	}

	void Update()
	{
		if( false == m_bUseAssetbundle)
			return;
		
		if( false == Caching.ready)
			return;

		if( true == m_bUpdatePatchSnapshot)
			_UpdatePatchSnapshot();

		if( false == m_bDownloadStart)
			return;
		
		int nCount = m_listDownLoad.Count;
		if( 0 == nCount)
			return;
		
		if( null != m_msgboxPatchError)
		{
			if( m_fMsgBoxTimer + 5.0f < Time.realtimeSinceStartup)
				_ClosePatchErrorMsgBox();
		}
		
		for( int i = 0; i < MAX_ASYNCDOWNCOUNT; i++)
		{
			int nIndex = nCount - ( 1 + i);
			
			if( nIndex >= 0)
			{
				stDownloadData stData = m_listDownLoad[nIndex];
				
				if( true == m_AssetbundleLoader.isDownLoaded( stData._strName, stData._nVersion))
				{
					m_listDownLoad.RemoveAt( nIndex);
					m_lDownloadingTotalCurSize += stData._lSize;
				}
			}
		}
		
//		long lPercent = ( m_lDownloadingTotalCurSize * 100) / m_lDownloadingTotalMaxSize;
//		m_AssetbundleDownInfoText.Text = "Update..." + lPercent.ToString() + "%";
//		m_AssetbundleProgress.Value = (float)lPercent / 100.0f;

		long lPercent = ( m_lDownloadingTotalCurSize * 100) / m_lDownloadingTotalMaxSize;
		m_AssetbundleProgress.Value = (float)lPercent / 100.0f;
		StringBuilder sb = new StringBuilder( "Update...");
		sb.Append( lPercent.ToString());
		sb.Append( "%");
		m_AssetbundleDownInfoText.Text = sb.ToString();

		if( 0 == m_listDownLoad.Count)
		{
			m_bDownLoaded = true;
			//_HidePatchSnapshot();
			//SetActivePatchFinishedButton( true);
#if _USE_PATCH_FINISH_BTN
			_SetPatchFinishedButtonState( true);
#else
			OnPatchFinishedBtnOk();
#endif
			
			if( null != m_encodedStringBuf)
			{
				_SaveFile( m_encodedStringBuf, _strAssetbundleTableFileName);
				m_encodedStringBuf = null;
			}
		}
	}

	public GameObject GetAssetbundleGameObject(string strPath)
	{
		string[] strSplit = strPath.Split('/');
		string strName = strSplit[strSplit.Length-1];
		return m_AssetbundleLoader.GetGameObject( strName, false);
	}
	
	public TextAsset GetAssetbundleTextAsset(string strPath)
	{
		string[] strSplit = strPath.Split('/');
		string strName = strSplit[strSplit.Length-1];
		return (TextAsset)m_AssetbundleLoader.GetTextAsset( strName, false);
	}
	
	public AudioClip GetAssetbundleAudioClip(string strPath)
	{
		string[] strSplit = strPath.Split('/');
		string strName = strSplit[strSplit.Length-1];
		
		if( false == m_dAssetbundleTable.ContainsKey( strName.ToLower()))
		{
			Debug.LogError( "Can't find Assetbundle file from AssetbundleTable: " + strName);
			return null;
		}
		
		return (AudioClip)m_AssetbundleLoader.GetAudioClip( strName, false);
	}

	public Material GetAssetbundleMaterial(string strPath)
	{
		string[] strSplit = strPath.Split('/');
		string strName = strSplit[strSplit.Length-1];
		return m_AssetbundleLoader.GetMaterial( strName, false);
	}
	
	public Texture GetAssetbundleTexture(string strPath)
	{
		string[] strSplit = strPath.Split('/');
		string strName = strSplit[strSplit.Length-1];
		return m_AssetbundleLoader.GetTexture( strName, false);
	}
	
	public WWW SceneAssetbundleLoadCache(string strSceneName)
	{
		return m_AssetbundleLoader.SceneAssetbundleLoadCache( strSceneName);
	}
	
	public bool isLoadedScene(string strSceneName)
	{
		return m_AssetbundleLoader.isDownLoadedScene( strSceneName);
	}
	
	public int GetCachedVersion(string strName)
	{
		string strKey = strName.ToLower();
		if( true == m_dAssetbundleVersion.ContainsKey( strKey))
			return m_dAssetbundleVersion[strKey];

		Debug.Log( "GetCachedVersion(): not found key string: " + strKey);
		return 1;
	}
	
	public bool IsCachedVersion(string strName)
	{
		string strKey = strName.ToLower();
		if( true == m_dAssetbundleVersion.ContainsKey( strKey))
			return true;
		
		return false;
	}
	
	public void SetActiveAssetbundleInfo(bool bActive, Color textColor)
	{
		SetActiveAssetbundleInfo( bActive);
		m_AssetbundleDownInfoText.Color = textColor;
	}
	
	public void SetActiveAssetbundleInfo(bool bActive)
	{
		m_AssetbundleProgress.gameObject.SetActiveRecursively( bActive);
		m_AssetbundleDownInfoText.gameObject.SetActiveRecursively( bActive);
	}
	
	public void SetAssetbundleInfoText(string strText)
	{
		m_AssetbundleDownInfoText.Text = strText;
	}
	
	public void SetAssetbundleInfoProgress(float fProgress)
	{
		m_AssetbundleProgress.Value = fProgress;
	}
	
	public void SetActiveLoadingTip(bool bActive)
	{
		m_LoadingTip.gameObject.SetActiveRecursively( bActive);
	}
	
	public void SetLoadingText(string strText, Color textColor)
	{
		m_LoadingTip.Text = strText;
		m_LoadingTip.Color = textColor;
	}
	
	public void OnPatchFinishedBtnOk()
	{
		m_bPatchFinishedBtnOk = true;
		if( ePatchGroup.ePatchGroup_1 == m_eCurPatchGroup)
		{
			_HidePatchSnapshot();
			_SetActivePatchFinishedButton( false);
		}
	}
	
	public void HidePatchScene()
	{
		_HidePatchSnapshot();
		_SetActivePatchFinishedButton( false);
	}
	
	public string GetPatcherString(int nIndex)
	{
		return m_AssetbundleParser.GetPatcherString( nIndex);
	}

	#region assetbundle path
	public string strAssetbundleDownloadURL
	{
		get
		{
			StringBuilder sb = new StringBuilder( AsNetworkDefine.PATCH_SERVER_ADDRESS);

#if UNITY_IPHONE
			sb.Append( "ios/");
#elif UNITY_ANDROID
			sb.Append( "aos/");
#else
			//sb.Append( "ios/");
#endif
			
			return sb.ToString();
		}
	}
	
	public string strAssetbundleExtension
	{
		get { return ".asab";}
	}
	
	public string GetAssetbundleDownloadPath(string strFileNameLower)
	{
		StringBuilder sb = new StringBuilder( strAssetbundleDownloadURL);
		sb.Append( strFileNameLower);
		sb.Append( strAssetbundleExtension);
		
		return sb.ToString();
	}
	#endregion assetbundle path

	public void DownloadAssets()
	{
		m_AssetbundleDownInfoText.gameObject.SetActiveRecursively( true);
		m_AssetbundleDownInfoText.Color = Color.white;
		m_AssetbundleDownInfoText.Text = "Checking Update...";

		m_eCurPatchGroup = ePatchGroup.ePatchGroup_1;
		_CheckPatchList();
	}
	
	public void DownloadAssets_Group(ePatchGroup PatchGroup)
	{
		if( PatchGroup <= ePatchGroup.ePatchGroup_1 || PatchGroup >= ePatchGroup.ePatchGroup_Max)
		{
			Debug.Log( "DownloadAssets_Group() - Can't Patch Group: " + PatchGroup);
			return;
		}

		m_AssetbundleDownInfoText.gameObject.SetActiveRecursively( true);
		m_AssetbundleDownInfoText.Color = Color.black;//Color.gray;
		m_AssetbundleDownInfoText.Text = "Checking Update...";
		
		m_eCurPatchGroup = PatchGroup;
		
		m_lDownloadingTotalCurSize = 0;
		m_lDownloadingTotalMaxSize = 0;
		
		if( ePatchGroup.ePatchGroup_2 == PatchGroup)
		{
			foreach( KeyValuePair<string, XmlNode> pair in m_dPatchGroup_2)
				_AddAssetbundleDownLoadCache_Group( pair.Value);
		}
		else if( ePatchGroup.ePatchGroup_3 == PatchGroup)
		{
			foreach( KeyValuePair<string, XmlNode> pair in m_dPatchGroup_3)
				_AddAssetbundleDownLoadCache_Group( pair.Value);
		}
		else if( ePatchGroup.ePatchGroup_4 == PatchGroup)
		{
			foreach( KeyValuePair<string, XmlNode> pair in m_dPatchGroup_4)
				_AddAssetbundleDownLoadCache_Group( pair.Value);
		}
		else if( ePatchGroup.ePatchGroup_5 == PatchGroup)
		{
			foreach( KeyValuePair<string, XmlNode> pair in m_dPatchGroup_5)
				_AddAssetbundleDownLoadCache_Group( pair.Value);
		}

		Debug.Log( "DownLoad Count - " + PatchGroup + ": " + m_listDownLoad.Count);
		
		if( m_listDownLoad.Count > 0)
		{
			m_bDownloadStart = false;
			m_bDownLoaded = false;
#if _USE_PATCH_FINISH_BTN
			m_bPatchFinishedBtnOk = false;
#endif
			_OpenPatchChoiceMsgBox( true);
		}
		else
		{
			m_bDownLoaded = true;
			m_bPatchFinishedBtnOk = true;
			_HidePatchSnapshot();
		}
	}
	
	public void ClearAssetbundle()
	{
		m_AssetbundleLoader.ClearAssetbundle();
	}
	
	#region Patch MessageBox
	public void OnMsgBox_AppUpdate_Ok()
	{
		_CloseAppUpdateMsgBox();
	}
	
	public void OnMsgBox_PatchChoice_Ok()
	{
		m_bDownloadStart = true;
		m_AssetbundleProgress.gameObject.SetActiveRecursively( true);
		m_AssetbundleDownInfoText.Color = Color.black;//Color.white;
		//_ClosePatchChoiceMsgBox();

		_ShowPatchSnapshot( m_eCurPatchGroup);
#if _USE_PATCH_FINISH_BTN
		_SetActivePatchFinishedButton( true);
		_SetPatchFinishedButtonState( false);
#endif
		AsSoundManager.Instance.StopBGM();
		AsSoundManager.Instance.PlayBGM_Patcher();
	}
	
	public void OnMsgBox_PatchChoice_Cancel()
	{
		//_ClosePatchChoiceMsgBox();
		
		if( ePatchGroup.ePatchGroup_1 == m_eCurPatchGroup)
		{
			_ForceQuit();
		}
		else
		{
			_SendWarpCancel();
		}
	}
	
	public void OnMsgBox_PatchError_Ok()
	{
		//_ClosePatchErrorMsgBox();
	}
	
	public void OnMsgBox_PatchError_Cancel()
	{
		//_ClosePatchErrorMsgBox();
		
		if( ePatchGroup.ePatchGroup_1 == m_eCurPatchGroup)
		{
			_ForceQuit();
		}
		else
		{
			_SendWarpCancel();
		}
	}
	
	public bool isOpenPatchChoiceMsgBox()
	{
		if( null != m_msgboxPatchChoice)
			return true;
		return false;
	}
	
	private void _SendWarpCancel()
	{
		GameObject go = GameObject.Find( "SceneLoader");
		AsSceneLoader sceneLoader = go.GetComponent<AsSceneLoader>() as AsSceneLoader;
		
		if( GAME_STATE.STATE_CHARACTER_SELECT == sceneLoader.OldGameState)
		{
			AS_CG_RETURN_CHARSELECT retCharSelect = new AS_CG_RETURN_CHARSELECT();
			byte[] data = retCharSelect.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data );
		}
		else
		{
//			AsCommonSender.SendWarpCancel();

			if( AsSceneLoader.eLoadType.INSTANCE_DUNGEON_ENTER == sceneLoader.curLoadType)
			{
				AsInstanceDungeonManager.Instance.Send_InDun_Exit();
				sceneLoader.curLoadType = AsSceneLoader.eLoadType.WARP;
			}
			else
				AsCommonSender.SendWarpCancel();
		}
		
		bSendWarpCancel = true;
		_PatchCancel();
	}

	private void _CreateMsgBox()
	{
		m_msgboxPatchChoice = ( new GameObject( "msgboxPatchChoice")).AddComponent<AsMessageBox>();
		_ClosePatchChoiceMsgBox();
		
		m_msgboxPatchError = ( new GameObject( "msgboxPatchError")).AddComponent<AsMessageBox>();
		_ClosePatchErrorMsgBox();
		
		m_msgboxAppUpdate = ( new GameObject( "msgboxAppUpdate")).AddComponent<AsMessageBox>();
		_CloseAppUpdateMsgBox();
	}
	
	private void _OpenAppUpdateMsgBox()
	{
		if( null != m_msgboxAppUpdate)
			return;
		
		string strTitle = m_AssetbundleParser.GetPatcherString( 1);
		string strMsg = m_AssetbundleParser.GetPatcherString( 2);
		string strOk = m_AssetbundleParser.GetPatcherString( 5);
		string strCancel = m_AssetbundleParser.GetPatcherString( 6);
		m_msgboxAppUpdate = AsNotify.Instance.MessageBox( strTitle, strMsg, strOk, strCancel, this, "OnMsgBox_AppUpdate_Ok", "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE, false, false);
	}
	
	private void _CloseAppUpdateMsgBox()
	{
		if( null != m_msgboxAppUpdate)
			m_msgboxAppUpdate.Close();
	}
	
	private void _OpenPatchChoiceMsgBox(bool bUseBtnSound)
	{
		if( null != m_msgboxPatchChoice)
			return;
		#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		WemeSdkManager.GetMainGameObject.startButtonClicked();
		#endif
		string strTitle = m_AssetbundleParser.GetPatcherString( 1);
		//string strMsg = m_AssetbundleParser.GetPatcherString( 4);
		string strMsg = "";
		int nTotalSize = _GetDownloadTotalSize();
		if( nTotalSize < 0)
			strMsg = m_AssetbundleParser.GetPatcherString( 7);
		else
			strMsg = string.Format( m_AssetbundleParser.GetPatcherString( 4), nTotalSize.ToString());
		string strOk = m_AssetbundleParser.GetPatcherString( 5);
		string strCancel = m_AssetbundleParser.GetPatcherString( 6);
		m_msgboxPatchChoice = AsNotify.Instance.MessageBox_Patch( strTitle, strMsg, strOk, strCancel, this, "OnMsgBox_PatchChoice_Ok", "OnMsgBox_PatchChoice_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION, bUseBtnSound, bUseBtnSound);
	}
	
	private void _ClosePatchChoiceMsgBox()
	{
		if( null != m_msgboxPatchChoice)
			m_msgboxPatchChoice.Close();
	}
	
	public void OpenPatchErrorMsgBox()
	{
		if( null != m_msgboxPatchError)
			return;
		
		string strTitle = m_AssetbundleParser.GetPatcherString( 1);
		string strMsg = m_AssetbundleParser.GetPatcherString( 3);
		string strOk = m_AssetbundleParser.GetPatcherString( 5);
		string strCancel = m_AssetbundleParser.GetPatcherString( 6);
		m_msgboxPatchError = AsNotify.Instance.MessageBox( strTitle, strMsg, strOk, strCancel, this, "OnMsgBox_PatchError_Ok", "OnMsgBox_PatchError_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION, false, false);
		
		m_fMsgBoxTimer = Time.realtimeSinceStartup;
	}
	
	private void _ClosePatchErrorMsgBox()
	{
		if( null != m_msgboxPatchError)
			//m_msgboxPatchError.Close();
			m_msgboxPatchError.Destroy_Only();
	}
	#endregion Patch MessageBox

	// < private
	private string _strAssetbundleTableFileName
	{
		get
		{
			if( eAppPlatformType.eAppPlatformType_IPhonePlayer == m_eAppFlatformType)
				return "AssetbundleTable_ios.asab";
			else if( eAppPlatformType.eAppPlatformType_Android == m_eAppFlatformType)
				return "AssetbundleTable_aos.asab";
			else if( eAppPlatformType.eAppPlatformType_WindowsPlayer == m_eAppFlatformType)
				return "AssetbundleTable_win.asab";
			else
			{
				Debug.LogError( "Failed: _strAssetbundleTableFileName is null, " + "Platform: " + Application.platform);
				return null;
			}
		}
	}
	
	private void _AddAssetbundleDownLoadCache(string strName, int nVersion, long lSize)
	{
		if( 0 == strName.Length || true == strName.ToLower().Contains( "none"))
			return;
		
		stDownloadData data = new stDownloadData( strName.ToLower(), nVersion, lSize);
		m_listDownLoad.Add( data);
		m_lDownloadingTotalMaxSize += lSize;
		
		//Debug.Log( "_AddAssetbundleDownLoadCache(): " + strName);
	}
	
	private void _AddAssetbundleDownLoadCache_Group(XmlNode xn)
	{
		string strName = xn["Name"].InnerText;
		int nVersion = Convert.ToInt32( xn["Version"].InnerText);
		long lSize = Convert.ToInt64( xn["Size"].InnerText);
		string strPath = GetAssetbundleDownloadPath( strName);

		if( false == Caching.IsVersionCached( strPath, nVersion))
		{
			_AddAssetbundleDownLoadCache( strName, nVersion, lSize);
		}
	}
	
	static int s_UnityUpdatePatch = 0;

	private void _CheckPatchList()
	{
		// load local patch list
		System.String[] res = System.IO.Directory.GetFiles( AsUtil.strSaveDataPath, _strAssetbundleTableFileName, SearchOption.AllDirectories);

		if( res.Length > 0)
		//if (1 == 1)
		{

			//var text = AssetbundleManager.Instance.GetAssetbundleTextAsset("AssetbundleTable_aos");
			//string str = text.text;

			string strPath = res[0];//AsUtil.strSaveDataPath + "/" + _strAssetbundleTableFileName;
			FileStream fs = new FileStream( strPath, FileMode.Open);

			byte[] buffer = new byte[fs.Length];
			fs.Read( buffer, 0, (int)(fs.Length));
			//string str = System.Text.Encoding.UTF8.GetString(buffer);
			MemoryStream memoryStream = new MemoryStream( buffer);
			StreamReader streamReader = new StreamReader( memoryStream);
			StringReader stringReader = new StringReader( streamReader.ReadToEnd());
			string str = stringReader.ReadToEnd();


			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml( str);
			
			string strVer = xmlDoc.DocumentElement.GetAttribute( "TableVersion");
			m_nAssetbundleTableVersion_c = Convert.ToInt32( strVer);

//			XmlNodeList xnList = xmlDoc.SelectNodes( "AssetbundleTable/Assetbundle");
//			
//			foreach( XmlNode xn in xnList)
//				m_dAssetbundleTable_c.Add( xn["Name"].InnerText, xn);

			s_UnityUpdatePatch = PlayerPrefs.GetInt( "unityupdatepatch", 0);
			if( 0 == s_UnityUpdatePatch)
			{
				Caching.CleanCache();
				m_nAssetbundleTableVersion_c = 0;
				Debug.Log( "m_nAssetbundleTableVersion_c < 1406021224: CleanCache");
			}
			else
			{
				XmlNodeList xnList = xmlDoc.SelectNodes( "AssetbundleTable/Assetbundle");
				
				foreach( XmlNode xn in xnList)
					m_dAssetbundleTable_c.Add( xn["Name"].InnerText, xn);
			}

			memoryStream.Close();
			streamReader.Close();
			fs.Close();
		}

		PlayerPrefs.SetInt( "unityupdatepatch", 1);
		
		StartCoroutine( _DownloadPatchListAndCreatePatchList());
	}
	
	private IEnumerator _DownloadPatchListAndCreatePatchList()
	{
		string strPath2 = strAssetbundleDownloadURL + _strAssetbundleTableFileName;
		WWW www = new WWW( strPath2);
		
		yield return www;
		
		if( null == www.assetBundle)
		{
			Debug.LogError( "_CreatePatchList(): null == www.assetBundle, " + strPath2);
			yield return null;
		}
		
		AssetBundle bundle = www.assetBundle;
		TextAsset xmlText = bundle.mainAsset as TextAsset;

		m_encodedStringBuf = Encoding.UTF8.GetBytes( xmlText.text);
		MemoryStream memoryStream2 = new MemoryStream( m_encodedStringBuf);
		StreamReader streamReader2 = new StreamReader( memoryStream2);
		StringReader stringReader2 = new StringReader( streamReader2.ReadToEnd());
		string str2 = stringReader2.ReadToEnd();
		
		XmlDocument xmlDoc2 = new XmlDocument();
		xmlDoc2.LoadXml( str2);
		
		bool bCheckTable = false;
		string strVer2 = xmlDoc2.DocumentElement.GetAttribute( "TableVersion");
		int nVersion = Convert.ToInt32( strVer2);

		if( m_nAssetbundleTableVersion_c > nVersion)
		{
			string strErrorMsg = "The AssetbundleTableVersion in the local higher, Local: " + m_nAssetbundleTableVersion_c + ", Server: " + nVersion;
			AsNotify.Instance.MessageBox( "Error", strErrorMsg, "Ok", "Cancel", null, null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR, false, false);
		}
		
		if( m_nAssetbundleTableVersion_c < nVersion)
		{
//			m_strVersionFolderName = nVersion.ToString();
			bCheckTable = true;
		}
//		else
//			m_strVersionFolderName = m_nAssetbundleTableVersion_c.ToString();
		
		//Debug.Log( "TablePath: " + strPath2);
		Debug.Log( "PatchList Vertion, Local: " + m_nAssetbundleTableVersion_c + ", Server: " + nVersion);
		
		XmlNodeList xnList = xmlDoc2.SelectNodes( "AssetbundleTable/Assetbundle");
		
		// load server patch list
		if( true == bCheckTable)
		{
			foreach( XmlNode xn in xnList)
			{
				string strName_s = xn["Name"].InnerText;
				int nVersion_s = Convert.ToInt32( xn["Version"].InnerText);
				long lSize = Convert.ToInt64( xn["Size"].InnerText);
				int nPatchGroupNumber = Convert.ToInt32( xn["PatchGroup"].InnerText);
				
				if( true == m_dAssetbundleTable_c.ContainsKey( strName_s))
				{
					XmlNode xn_c = m_dAssetbundleTable_c[strName_s];
					int nVersion_c = Convert.ToInt32( xn_c["Version"].InnerText);

					if( nVersion_s > nVersion_c)
					{
						if( 1 == nPatchGroupNumber)
							_AddAssetbundleDownLoadCache( strName_s, nVersion_s, lSize);
						else if( 2 == nPatchGroupNumber)
							m_dPatchGroup_2.Add( strName_s, xn);
						else if( 3 == nPatchGroupNumber)
							m_dPatchGroup_3.Add( strName_s, xn);
						else if( 4 == nPatchGroupNumber)
							m_dPatchGroup_4.Add( strName_s, xn);
						else if( 5 == nPatchGroupNumber)
							m_dPatchGroup_5.Add( strName_s, xn);
					}
				}
				else
				{
					if( 1 == nPatchGroupNumber)
						_AddAssetbundleDownLoadCache( strName_s, nVersion_s, lSize);
					else if( 2 == nPatchGroupNumber)
						m_dPatchGroup_2.Add( strName_s, xn);
					else if( 3 == nPatchGroupNumber)
						m_dPatchGroup_3.Add( strName_s, xn);
					else if( 4 == nPatchGroupNumber)
						m_dPatchGroup_4.Add( strName_s, xn);
					else if( 5 == nPatchGroupNumber)
						m_dPatchGroup_5.Add( strName_s, xn);
				}
				
				m_dAssetbundleTable.Add( strName_s, xn);
				m_dAssetbundleVersion.Add( strName_s, nVersion_s);
			}
		}
		else
		{
			foreach( XmlNode xn in xnList)
			{
				string strName_s2 = xn["Name"].InnerText;
				m_dAssetbundleTable.Add( strName_s2, xn);
				m_dAssetbundleVersion.Add( strName_s2, Convert.ToInt32( xn["Version"].InnerText));
				
				int nPatchGroupNumber2 = Convert.ToInt32( xn["PatchGroup"].InnerText);
				if( 2 == nPatchGroupNumber2)
					m_dPatchGroup_2.Add( strName_s2, xn);
				else if( 3 == nPatchGroupNumber2)
					m_dPatchGroup_3.Add( strName_s2, xn);
				else if( 4 == nPatchGroupNumber2)
					m_dPatchGroup_4.Add( strName_s2, xn);
				else if( 5 == nPatchGroupNumber2)
					m_dPatchGroup_5.Add( strName_s2, xn);
			}
			m_bDownLoaded = true;
			m_bPatchFinishedBtnOk = true;
			m_encodedStringBuf = null;
			_HidePatchSnapshot();
		}
		
		Debug.Log( "DownLoad Count: " + m_listDownLoad.Count);
		
		memoryStream2.Close();
		streamReader2.Close();
		m_dAssetbundleTable_c.Clear();
		
		if( m_listDownLoad.Count > 0)
			_OpenPatchChoiceMsgBox( false);
		else
		{
			m_bDownLoaded = true;
			m_bPatchFinishedBtnOk = true;
		}
	}
	
	private void _SaveFile(byte[] byteData, string strFileName)
	{
#if !UNITY_WEBPLAYER
		FileInfo fileInfo = new FileInfo( AsUtil.strSaveDataPath + strFileName);
		if( true == fileInfo.Exists)
			fileInfo.Delete(); // do not UNITY_WEBPLAYER
#endif
		string strPath = AsUtil.strSaveDataPath + "/" + strFileName;
		FileStream fs = new FileStream( strPath, FileMode.Create);
		fs.Seek( 0, SeekOrigin.Begin);
		fs.Write( byteData, 0, byteData.Length);
		fs.Close();

		Debug.Log( "_SaveFile(): " + strFileName);
	}

	private void _ForceQuit()
	{
		AsUtil.Quit();
	}
	
	private void _PatchCancel()
	{
		m_bDownloadStart = false;
		m_bDownLoaded = true;
		m_bPatchFinishedBtnOk = true;
		_HidePatchSnapshot();
		
		m_listDownLoad.Clear();
	}
	
	private void _ShowPatchSnapshot(ePatchGroup PatchGroup)
	{
		if( PatchGroup <= ePatchGroup.ePatchGroup_Invalid || PatchGroup >= ePatchGroup.ePatchGroup_Max)
		{
			Debug.Log( "Error: _ShowPatchSnapshot(): " + PatchGroup);
			return;
		}
		
		m_nCurPatchGroupImgIndex = 1;
		m_nCurPatchGroupTextIndex = 1;
		
		AssetbundleParser.stPatcherTable stData = m_AssetbundleParser.GetPatcherTableData( PatchGroup, m_nCurPatchGroupImgIndex);
		Texture tex = ResourceLoad.Loadtexture( stData._strPath);
		m_PatcherImg.renderer.material.mainTexture = tex;
		m_PatcherText.Text = m_AssetbundleParser.GetPatcherString( stData, m_nCurPatchGroupTextIndex);

		//m_eCurPatchGroup = PatchGroup;
		m_fPatchSnapshotTimer = Time.realtimeSinceStartup;
		m_bUpdatePatchSnapshot = true;
		
		m_PatcherBG.gameObject.SetActiveRecursively( true);
		m_PatcherImg.gameObject.SetActiveRecursively( true);
		m_PatcherText.gameObject.SetActiveRecursively( true);
		m_btnWebview.gameObject.SetActiveRecursively(true);
	}
	
	private void _HidePatchSnapshot()
	{
		m_bUpdatePatchSnapshot = false;
		
		m_PatcherBG.gameObject.SetActiveRecursively( false);
		m_PatcherImg.gameObject.SetActiveRecursively( false);
		m_PatcherText.gameObject.SetActiveRecursively( false);
		m_btnWebview.gameObject.SetActiveRecursively(false);
	}
	
	private void _UpdatePatchSnapshot()
	{
		AssetbundleParser.stPatcherTable stData = m_AssetbundleParser.GetPatcherTableData( m_eCurPatchGroup, m_nCurPatchGroupImgIndex);
		
		if( m_fPatchSnapshotTimer + stData._nTime < Time.realtimeSinceStartup)
		{
			m_nCurPatchGroupTextIndex++;
			m_fPatchSnapshotTimer = Time.realtimeSinceStartup;
			
			string strText = m_AssetbundleParser.GetPatcherString( stData, m_nCurPatchGroupTextIndex);
			if( 0 == strText.Length)
			{
				m_nCurPatchGroupTextIndex = 1;
				m_nCurPatchGroupImgIndex++;

				if( m_AssetbundleParser.GetPatcherListCount( m_eCurPatchGroup) <= m_nCurPatchGroupImgIndex)
					m_nCurPatchGroupImgIndex = 1;
				
				stData = m_AssetbundleParser.GetPatcherTableData( m_eCurPatchGroup, m_nCurPatchGroupImgIndex);
				
				Texture tex = ResourceLoad.Loadtexture( stData._strPath);
				m_PatcherImg.renderer.material.mainTexture = tex;
			}
			
			m_PatcherText.Text = m_AssetbundleParser.GetPatcherString( stData, m_nCurPatchGroupTextIndex);
		}
	}

	private void _SetActivePatchFinishedButton(bool bActive)
	{
		m_btnPatchFinished.gameObject.SetActiveRecursively( bActive);
	}
	
	private void _SetPatchFinishedButtonState(bool bEnable)
	{
		if( true == bEnable)
		{
			m_btnPatchFinished.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_btnPatchFinished.spriteText.SetColor( Color.white);
		}
		else
		{
			m_btnPatchFinished.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_btnPatchFinished.spriteText.SetColor( Color.gray);
		}
	}
	
	private int _GetDownloadTotalSize()
	{
		if( m_lDownloadingTotalMaxSize < 1000000)
			return -1;
		
		int nMB = (int)( m_lDownloadingTotalMaxSize * 0.000001f);
		if( nMB < 1)
			return -1;
		
		return nMB;
	}
	// private >
	private void OnClickWebview( ref POINTER_INFO ptr)
	{

		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "OnClickWebview()");
			#if !UNITY_EDITOR
				WemeSdkManager.GetMainGameObject.showPlatformViewWeb(GetPatcherString(63));
			#endif
		}

	}
	
}
