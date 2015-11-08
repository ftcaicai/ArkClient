using UnityEngine;
using System.Collections;

public class ObjBreakAction : MonoBehaviour 
{
    private int m_iSessionID = -1;	
	private bool m_bAction = false;
    //// Use this for initialization
    void Start () 
    {
        AsNpcEntity npcEntity = transform.parent.GetComponent<AsNpcEntity>();
        if (null == npcEntity)
        {
            Debug.LogError("null == npcEntity");
            return;
        }

        if (eFsmType.OBJECT != npcEntity.FsmType)
        {
            Debug.LogError("eFsmType.OBJECT != npcEntity.FsmType");
            return;
        }

        m_iSessionID = npcEntity.sessionId_;

    }
	 
	// Update is called once per frame
	void Update () 
    {
	
	}
	void OnTriggerStay(Collider collisionInfo)
	{
		DoAction(collisionInfo);
	}
	
	
	void DoAction(Collider collisionInfo)
	{
		if( true == m_bAction )
			return;
		
		AsUserEntity userEntity = AsEntityManager.Instance.GetEntityByInstanceId(collisionInfo.gameObject.GetInstanceID()) as AsUserEntity;
        if (null == userEntity)
            return;

        if( eFsmType.PLAYER != userEntity.FsmType )
            return;

        AsPlayerFsm objFsm = userEntity.GetComponent( eComponentType.FSM_PLAYER ) as AsPlayerFsm;
        if( null == objFsm )
            return;
	

       	if( AsPlayerFsm.ePlayerFsmStateType.DASH == objFsm.CurrnetFsmStateType )
		{
            if (-1 != m_iSessionID)
			{
				AsCommonSender.SendObjectBreak(m_iSessionID);
				m_bAction = true;
			}
            else
                Debug.LogError("don't Send CS_OBJECT_BREAK");
		}
	}
	
	void OnTriggerExit(Collider collisionInfo)
	{
		DoAction(collisionInfo);
	}

   

    void OnTriggerEnter(Collider collisionInfo)
    {		
		DoAction(collisionInfo);
    }
}
