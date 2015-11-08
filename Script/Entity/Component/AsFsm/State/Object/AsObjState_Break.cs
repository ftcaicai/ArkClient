using UnityEngine;
using System.Collections;

public class AsObjState_Break : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{
    public enum eBREAK_STATE
    {
        LIFE,
        BREAK
    }

    private eBREAK_STATE m_eObjState = eBREAK_STATE.LIFE;

    public void SetState(eBREAK_STATE state)
    {
        m_eObjState = state;
    }

    public AsObjState_Break(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.BREAK, _fsm)
	{
        SetState(eBREAK_STATE.LIFE);
		
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.OBJ_BREAK_MSG, OnObjectBreak);
	}

    public override void Enter(AsIMessage _msg)
    {
    }
	
    public override void Update()
    {
        
    }
    public override void Exit()
    {
    }   

	void OnAnimationEnd(AsIMessage _msg)
	{
		if (eBREAK_STATE.BREAK == m_eObjState)
		{
            AsNpcEntity deleteEntity = m_OwnerFsm.Entity as AsNpcEntity;
            if (null != deleteEntity)
                AsEntityManager.Instance.RemoveEntity(deleteEntity);
            else
                Debug.LogError("null == deleteEntity");

		}
	}
	
	void OnObjectBreak(AsIMessage _msg)
	{
		if( null == m_OwnerFsm.Entity || null == m_OwnerFsm.Entity.ModelObject )
			return;
		
		Msg_ObjBreak msgObjBreak = _msg as Msg_ObjBreak;
        if (null == msgObjBreak)
        {
            Debug.LogError("null == msgObjBreak");
            return;
        }

        Tbl_Object_Record objRecord = AsTableManager.Instance.GetTbl_Object_Record(msgObjBreak.m_iObjTableID);
        if (null == objRecord)
        {
            Debug.LogError("null == objRecord");
            return;
        }

        SetState(eBREAK_STATE.BREAK);
        m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate(objRecord.DoActionAni));            
		AsSoundManager.Instance.PlaySound( "S0063_EFF_Break", m_OwnerFsm.Entity.ModelObject.transform.position, false);
	}
}
