using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour {

	static UserData instance_ = null;
    static public UserData Instance
	{
		get { return instance_; }
	}
	void Awake()
	{
		instance_ = this;
	}
	// auth data jsonString
	public string authData = "";
	
	public bool isAuthorized = false;
	
	public string playerKey = "";
	
	public string promotionData = "";
	
	public bool isDeviceLogin = false;
}
