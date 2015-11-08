using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
public class WemeSDKDllImportMacOSX : MonoBehaviour {

	[DllImport("WemeSDKPluginPC")]
	private static extern string  test();
	
	[DllImport("WemeSDKPluginPC")]
	private static extern string requestSyncEditor(string jsonObjectString);

	[DllImport("WemeSDKPluginPC")]
	private static extern void requestASyncEditor(string jsonObjectString,string resultObject,string resultMethod);
	
	[DllImport("WemeSDKPluginPC")]
	static extern void setEnableNotificationEditor(string resultObject,string resultMethod);
	
	[DllImport("WemeSDKPluginPC")]
	static extern void removeNotificationEditor();
	
	public static string e_test(){
		return test();
	}
	
	public static void e_requestASyncEditor(string requestString,string objectName,string methodName)
	{
		requestASyncEditor(requestString,objectName,methodName);
	}
	public static string e_requestSyncEditor(string requestString)
	{
		return requestSyncEditor(requestString);
	}
	
	public static void e_setEnableNotificationEditor(string resultObject,string resultMethod)
	{
		setEnableNotificationEditor(resultObject,resultMethod);
	}
	
	public static void e_removeNotificationEditor()
	{
		removeNotificationEditor();
	}
}
#endif
