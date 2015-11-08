using UnityEngine;
using System.Collections;
using System;

public class TooltipEventDlg : MonoBehaviour 
{
	public SpriteText textTitle;
	public SpriteText textOpenDay;
	public SpriteText textText;
	public UIButton btnEventWeb;
	
	public void Open( body2_SC_EVENT_LIST _data )
	{
		textTitle.Text = _data.szTitle;
		textText.Text = _data.szContent;
		textOpenDay.Text = GetTime(_data.tStartTime) + "~" + GetTime(_data.tEndTime);
		btnEventWeb.SetInputDelegate( EventBtnDelegate);
	}
	
	private string GetTime(long nTime)
	{		
		System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
		dt = dt.AddSeconds(nTime);
		return string.Format("{0:yyyy.MM.dd}", Convert.ToDateTime(dt.ToString())); 
	}
	
	
	private void EventBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			string url = AsTableManager.Instance.GetTbl_String(2701);
			Application.OpenURL(url);
/*
			#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
			Application.OpenURL(url);
			#else	
			WemeSdkManager.GetMainGameObject.showPlatformViewWeb(url);
			#endif
*/			
		}
	}

	// Use this for initialization
	void Start () 
	{
		btnEventWeb.spriteText.Text = AsTableManager.Instance.GetTbl_String(1588);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
