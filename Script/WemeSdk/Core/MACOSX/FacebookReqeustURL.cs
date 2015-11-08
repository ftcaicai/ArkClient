using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class FacebookReqeustURL : MonoBehaviour {
	// http://developers.facebook.com/docs/reference/api/
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private static string FACEBOOK_API = "https://graph.facebook.com";
	
	public  WWW getFriends(Dictionary<string,string> parameters){
		WWW data = sendGetUrl("/me/friends",parameters);
		
		return data;
	}
	
	public  WWW  getMe(Dictionary<string,string> parameters){
		return sendGetUrl("/me",parameters);
	}
	public WWW getMeFeed(Dictionary<string,string> parameters){
		return sendGetUrl("/me/feed",parameters);
	}
		
	public  WWW sendGetUrl(string url,Dictionary<string,string> parameters ){
		
		url = FACEBOOK_API + url;
		if( parameters != null )
			{
				if(parameters.Count>0){
					url = url+"?";
				}
				foreach( var kv in parameters  )
				{
					url = url + kv.Key+"="+kv.Value+"&";
				}
				if(parameters.Count>0){			
					if(url.EndsWith("&")){
						url = url.Substring(0,url.Length-1);
					}
				}
			}
		
        WWW www = new WWW(url);
		
		StartCoroutine(WaitForRequest(www));
		Debug.Log("return 1");
		return www;
	}
	public  WWW sendPostUrl(string url,Dictionary<string,string> parameters ){
		
		url = FACEBOOK_API + url;
		
		WWWForm form = new WWWForm();
		if( parameters != null )
			{
				foreach( var kv in parameters  )
				{
					form.AddField(kv.Key, kv.Value);
					
				}
			}
		
        WWW www = new WWW(url,form);
		StartCoroutine(WaitForRequest(www));
		return www;
	}
	private IEnumerator WaitForRequest(WWW www)
    {
        
 		yield return www;
		
		// check for errors
		if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        } else {
            Debug.Log("WWW Error: "+ www.error);
        }    
           
		
    }
	public void sendGetUrl(string url,Dictionary<string,string> parameters,Action<string> onComplete)
	{
		url = FACEBOOK_API + url;
		if( parameters != null )
			{
				if(parameters.Count>0){
					url = url+"?";
				}
				foreach( var kv in parameters  )
				{
					url = url + kv.Key+"="+kv.Value+"&";
				}
				if(parameters.Count>0){			
					if(url.EndsWith("&")){
						url = url.Substring(0,url.Length-1);
					}
				}
			}
		
        WWW www = new WWW(url);
		
		StartCoroutine(WaitForRequest(www,onComplete));
	}
	private IEnumerator WaitForRequest(WWW www,Action<string> onComplete)
    {
        yield return www;
		
        if (www.error == null)
        {
         	onComplete(www.text);
        } else {
            onComplete(www.error);
        }    
           
		
    }
}
