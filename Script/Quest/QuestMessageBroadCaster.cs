using UnityEngine;
using System.Collections;
using ArkSphereQuestTool;

public static class QuestMessageBroadCaster
{
	private static GameObject questManagerObj = null;

	public static void Init()
	{
		questManagerObj = GameObject.Find( "QuestManager");
	}

	public static void Init( GameObject _objQuestManager)
	{
		questManagerObj = _objQuestManager;
		Debug.Log( questManagerObj.name);
	}

	public static void BrocastQuest( QuestMessages _messageType, object _data)
	{
		ArkQuestmanager.instance.ProcessMessage( new QuestMessageObject( _messageType, _data));
	}
}
