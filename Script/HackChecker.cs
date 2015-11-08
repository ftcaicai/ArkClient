using UnityEngine;
using System.Collections;

public class HackChecker : MonoBehaviour
{
	private bool isExistHack = false;
	private string hackName = string.Empty;
	
	// Use this for initialization
	void Start()
	{
		Check();
	}
	
	// Update is called once per frame
	void Update()
	{
		if( ( GAME_STATE.STATE_INGAME != AsGameMain.s_gameState) && ( GAME_STATE.STATE_CHARACTER_SELECT != AsGameMain.s_gameState))
			return;
		
		if( false == isExistHack)
			return;
		
		AsNotify.Instance.MessageBox( "Warning", hackName + " detected!!!", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_WARNING);
		
		body_CS_HACK_INFO hackInfo = new body_CS_HACK_INFO( hackName);
		byte[] packet = hackInfo.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		isExistHack = false;
	}
	
	public void Check()
	{
#if !UNITY_EDITOR
	#if UNITY_ANDROID
		System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
		for( int i = 0; i < processes.Length; i++)
		{
			string processName = processes[i].ProcessName;
			if( true == string.IsNullOrEmpty( processName))
				continue;
				
			Debug.Log( "Process name : " + processName);
			
			if( true == processName.Contains( "gamehack"))	// GameHacker
			{
				hackName = "GameHacker";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "gamespeed"))	// GameSpeeder
			{
				hackName = "GameSpeeder";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "cn.mc1.sq"))	// GameKiller
			{
				hackName = "GameKiller";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "azg.new.eight"))	// Appzzang guardian
			{
				hackName = "Appzzang guardian";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "kakao.cafe.coffee"))	// Game guardian
			{
				hackName = "Game guardian";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "com.cnn.g"))	// Game guardian
			{
				hackName = "Game guardian";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "freedom"))	// Freedom
			{
				hackName = "Freedom";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "com.prohiro.macro"))	// hiro macro
			{
				hackName = "hiro macro";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "com.hexview.android.memspectorpro"))	// MemSpector
			{
				hackName = "MemSpector";
				isExistHack = true;
				break;
			}
			
			if( true == processName.Contains( "com.cih.game_cih"))	// GameCIH
			{
				hackName = "GameCIH";
				isExistHack = true;
				break;
			}
		}
	#endif
#endif
	}
}
