using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


//https://github.com/kayac/Lobi/wiki/Lobi-OfficialCommunity
public class AsLobiSDKManager : MonoBehaviour {

	private static AsLobiSDKManager instance_ = null;
	private const string RETURN_OBJECT_NAME = "LobiManager";
	public static AsLobiSDKManager Instance
	{
		get
		{
			if( null == instance_)
			{
				instance_ = FindObjectOfType( typeof(AsLobiSDKManager)) as AsLobiSDKManager;
				if( null == instance_)
					Debug.Log( "Fail to get AsLobiSDKManager Instance");
			}

			return instance_;
		}
	}

	private float m_fX = 100.0f;
	private float m_fY = 100.0f;
	private float m_fSize = 100.0f;

		private bool isCapturing = false; 
	public bool IsCapturing
	{
		get { return isCapturing; }
		set	{ isCapturing = value; }
	}

	private bool isInCamera = false; 
	public bool IsInCamera
	{
		get { return isInCamera; }
		set	{ isInCamera = value; }
	}
	

	// Use this for initialization
	void Start()
	{
	
	}

	
	private void OpenOfficialCommunity_android() {

	}

	public void OpenLobiApplication( )
	{

	}

	public void SetWipePosition(float x, float y, float size) {
		m_fX = x * 0.5f;
		m_fY = y * 0.5f;
		m_fSize = size * 0.5f;
	}

	public void StartCapturing( )
	{
		isCapturing = true;
		#if UNITY_IPHONE
		QualitySettings.antiAliasing = 0; //Disabled
		#endif
	}

	public void StopCapturing( )
	{
		isCapturing = false;
		#if UNITY_IPHONE
		QualitySettings.antiAliasing = 2; //Set AntiAliasing to use 2x Multisampling
		#endif
	}

	public void PresentLobiPost( )
	{

	}

	public void PresentLobiPlay( )
	{

	}

	public void Snap( )
	{

	}

	public void SnapFace( )
	{

	}

	public void IsMicEnabled( )
	{

	}

	public bool IsSupported( )
	{
		return false;
	}


	void SnapCallback(string message)
	{
		Debug.Log("SnapCallback");
		Debug.Log(message);
	}
	
	void SnapFaceCallback(string message)
	{
		Debug.Log("SnapFaceCallback");
		Debug.Log(message);
	}
	
	void IsMicEnabledCallback(string message)
	{
		Debug.Log("IsMicEnabledCallback");
		Debug.Log(message);
	}
	
	void MicEnableErrorCallback(string message)
	{
		Debug.Log("micEnableError");
		Debug.Log(message);
	}
	
	void DismissingPostVideoViewCallback(string message)
	{
		Debug.Log("DismissingPostVideoViewCallback");
		Debug.Log(message);
	}
/*	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0, 0, 100, 100), "OpenLobiApplication") == true)
		{
			OpenLobiApplication();
		}

		if (GUI.Button(new Rect(50, 150, 200, 50), "StartCapturing")){
			StartCapturing();
		}
		if (GUI.Button(new Rect(50, 350, 200, 50), "StopCapturing")){
			StopCapturing();
		}
		if (GUI.Button(new Rect(50, 450, 200, 50), "PresentLobiPost")){
			PresentLobiPost();
		}
		if (GUI.Button(new Rect(50, 550, 200, 50), "PresentLobiPlay")){
			PresentLobiPlay()
		}
		if (GUI.Button(new Rect(50, 650, 200, 50), "Snap")){
			Snap();
		}
		if (GUI.Button(new Rect(50, 750, 200, 50), "SnapFace")){
			SnapFace();
		}
		if (GUI.Button(new Rect(50, 850, 200, 50), "IsMicEnabled")){
			IsMicEnabled();
		}
		if (GUI.Button(new Rect(50, 950, 200, 50), "IsSupported")){
			IsSupported();
		}
	}
*/	
}
