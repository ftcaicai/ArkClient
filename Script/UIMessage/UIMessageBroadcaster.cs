using UnityEngine;
using System.Collections.Generic;

public enum UIMessageType
{
    UI_MESSAGE_QUESTLIST_SHOW,
    UI_MESSAGE_QUESTLIST_UPDATE,
    UI_MESSAGE_QUESTLIST_NONESELECT,
    UI_MESSAGE_QUESTLIST_CLOSE,
    UI_MESSAGE_TALK_RESET,
    UI_MESSAGE_TALK_MENUBUTTON_UPDATE,
    UI_MESSAGE_TALK_CLOSE,
    UI_MESSAGE_QUESTACCEPT_UPDATE_CASHDONE,
}

public class UIMessageObject
{
    public UIMessageType messageType;
    public object        data;

    public UIMessageObject(UIMessageType _messageType, object _data)
    {
        messageType = _messageType;
        data = _data;
    }

    public UIMessageObject(UIMessageType _messageType)
    {
        messageType = _messageType;
        data        = new Object();
    }
}

public class UIMessageBroadcaster : MonoBehaviour
{
    private static List<UIMessageBase> listMessageUI = new List<UIMessageBase>();
    private static UIMessageBroadcaster s_Instance = null;
    public static UIMessageBroadcaster instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = FindObjectOfType(typeof(UIMessageBroadcaster)) as UIMessageBroadcaster;

            if (s_Instance == null)
            {
               
                GameObject obj = new GameObject("UIMessageBroadcaster");
                s_Instance = obj.AddComponent(typeof(UIMessageBroadcaster)) as UIMessageBroadcaster;
                Debug.Log("Could not locate an UIMessageBroadcaster object.\n UIMessageBroadcaster was Generated Automaticly.");

                listMessageUI = new List<UIMessageBase>();
                listMessageUI.Add(AsHUDController.Instance.m_questList);
                listMessageUI.Add(AsHUDController.Instance.m_NpcMenu);
               // listMessageUI.Add(AsHudDlgMgr.Instance.questAccept);
                listMessageUI.Add(AsHUDController.Instance.m_WorldMap);

            }
            return s_Instance;
        }
    }

    void Awake()
    {
//        DontDestroyOnLoad(this);
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
    }

    public void SendUIMessage(UIMessageType _messageType)
    {
        UIMessageObject messageObj = new UIMessageObject(_messageType);

        foreach (UIMessageBase ui in listMessageUI)
        {
            if (ui != null)
                ui.ProcessUIMessage(messageObj);
        }
    }


    public void SendUIMessage(UIMessageObject _messageObject)
    {
        foreach (UIMessageBase ui in listMessageUI)
        {
            if (ui != null)
                ui.ProcessUIMessage(_messageObject);
        }
    }
    
}
