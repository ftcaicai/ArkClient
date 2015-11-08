using UnityEngine;
using System.Collections;

public class QuestIntoMapTrigger : MonoBehaviour {

    public int questTableID;
    public int radius;
	public AchMapInto mapInfo;

	// Use this for initialization
	void Start () {

////        DontDestroyOnLoad(this);
//        DontDestroyOnLoad( gameObject);
//        DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, (float)radius);
    }


    void OnTriggerEnter(Collider other)
    {
        AsPlayerFsm asPlayerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
        if (other.gameObject == asPlayerFsm.gameObject)
        {
           Tbl_Quest_Record record =  AsTableManager.Instance.GetTbl_QuestRecord(questTableID);

           if (record != null)
           {
               if (record.QuestDataInfo != null)
               {
                   if (record.QuestDataInfo.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                   {
                       AsCommonSender.SendClearMapInto(questTableID);
                       Debug.LogWarning("Send Clear Map Into = " + questTableID);
                   }
               }
           }
        }
    }
}
