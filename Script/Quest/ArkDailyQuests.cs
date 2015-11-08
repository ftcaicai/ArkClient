using UnityEngine;
using System.Collections;

public class ArkDailyQuests : MonoBehaviour {

    private static ArkDailyQuests s_Instance = null;
    public static ArkDailyQuests instance
    {
        get
        {
            if (s_Instance == null)
                s_Instance = FindObjectOfType(typeof(ArkDailyQuests)) as ArkDailyQuests;

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("DailyQuests");
                s_Instance = obj.AddComponent(typeof(ArkDailyQuests)) as ArkDailyQuests;
                QuestMessageBroadCaster.Init(obj);
                Debug.Log("Could not locate an DailyQuests object.\n QuestManager was Generated Automaticly.");
            }
            return s_Instance;
        }
    }

    public void Start()
    {
//        DontDestroyOnLoad(this);
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
    }
}
