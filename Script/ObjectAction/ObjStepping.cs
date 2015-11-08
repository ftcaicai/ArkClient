using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjStepping
{
    public enum eSTEPPIG_STATE
    {
        IDLE,
		JUMP_START,
		LANDING,
        LEAF_IDLE,        
    }

    private int m_iGroupIndex = 0;
    private eSTEPPIG_STATE m_eSteppingState = eSTEPPIG_STATE.IDLE;
    private List<AsNpcEntity> m_ObjEntityList = new List<AsNpcEntity>();
	
		
	
    public ObjStepping(int iGroupIndex, int iLinkIndex, AsNpcEntity npcEntity )
    {
        m_iGroupIndex = iGroupIndex;
        AddObjEntity(iLinkIndex, npcEntity);
    }
	
	
    public int GetBegintLinkIndex()
    {		
		if( 0 >= m_ObjEntityList.Count )
		{
			Debug.LogError("ObjStepping:GetBegintLinkIndex() [0 >= m_ObjEntityList.Count]");
			return 0;
		}
		if( null == m_ObjEntityList[0] )
		{
			Debug.LogError("ObjStepping:GetBegintLinkIndex() [null == m_ObjEntityList[0]]");
			return 0;
		}
		
        return m_ObjEntityList[0].GetProperty<int>(eComponentProperty.LINK_INDEX);
    }

    public int GetEndLinkIndex()
    {
		if( 0 >= m_ObjEntityList.Count )
		{
			Debug.LogError("ObjStepping:GetEndLinkIndex() [0 >= m_ObjEntityList.Count]");
			return 0;
		}
		
		int iLinkIndex = m_ObjEntityList[m_ObjEntityList.Count - 1].GetProperty<int>(eComponentProperty.LINK_INDEX);
        return iLinkIndex;
    }	

    public int GetGroupIndex()
    {
        return m_iGroupIndex;
    }

    public eSTEPPIG_STATE GetSteppingState()
    {
        return m_eSteppingState;
    }

    public void SetSteppingState(eSTEPPIG_STATE eState)
    {
        m_eSteppingState = eState;
    }
	
	public void RemoveNpc(int nNpcIdx)
	{
		foreach( AsNpcEntity data in m_ObjEntityList )
		{
			if( data.sessionId_ == nNpcIdx )
			{				
				m_ObjEntityList.Remove(data);
				return;
			}
		}
	}
	
	public bool IsEmpty()
	{
		return m_ObjEntityList.Count <= 0;
	}
		
	public void AddObjEntity( int iLinkIndex, AsNpcEntity asObjEntity )
	{
        if (null == asObjEntity)
		{
            Debug.LogError("null == asObjEntity");
			return;
		}
		
		if( 0 == m_ObjEntityList.Count )
		{		
			//Debug.Log("ttttnormal [ " + iLinkIndex + " , " + m_ObjEntityList.Count );
        	m_ObjEntityList.Add(asObjEntity);	
			
		}
		else
		{     		
			
			if( iLinkIndex > m_ObjEntityList[m_ObjEntityList.Count-1].GetProperty<int>(eComponentProperty.LINK_INDEX) )
			{
				//Debug.Log("normal [ " + iLinkIndex + " , " + m_ObjEntityList.Count );
				m_ObjEntityList.Add(asObjEntity);
			}
			else
			{
			
				//Debug.Log("iLinkIndex > m_ObjEntityList.Count [ " + iLinkIndex + " , " + m_ObjEntityList.Count );
				
				List<AsNpcEntity> tempList = m_ObjEntityList;
				m_ObjEntityList = new List<AsNpcEntity>();
				
				
				bool bAddSucc = false;
				foreach( AsNpcEntity temp in tempList )
				{
					if( iLinkIndex < temp.GetProperty<int>(eComponentProperty.LINK_INDEX) && bAddSucc == false )
					{
						m_ObjEntityList.Add(asObjEntity);
						bAddSucc = true;
					}
					m_ObjEntityList.Add(temp);				
				}	
			}
		}		
	}
	
	
	
	public void ClickObject( AsNpcEntity npcObject, Vector3 vec3Target )
	{
		if (3 > m_ObjEntityList.Count)
        {
            Debug.LogError("3 > m_ObjEntityList.Count");
            return;
        }
		
		switch( GetSteppingState() )
		{
		case eSTEPPIG_STATE.IDLE:
			IdleClickObject( npcObject, vec3Target );
			break;
			
		case eSTEPPIG_STATE.JUMP_START:
			break;
			
		case eSTEPPIG_STATE.LANDING:			
			break;
			
		case eSTEPPIG_STATE.LEAF_IDLE:
			NextClickObject( npcObject, vec3Target );
			break;
		}
	}
	
	public void NextClickObject( AsNpcEntity npcObject, Vector3 vec3Target )
	{
		AsCommonSender.SendObjectJump(npcObject.SessionId);
	}
	
	
	public void IdleClickObject( AsNpcEntity npcObject, Vector3 vec3Target )
	{
		Vector3 vec3CharPos = AsEntityManager.Instance.UserEntity.transform.position;
		int iListEndIndex = m_ObjEntityList.Count-1;
		
		Collider coliderBegin = m_ObjEntityList[0].GetComponentInChildren<Collider>();
		Collider coliderEnd = m_ObjEntityList[iListEndIndex].GetComponentInChildren<Collider>();
		
		if( m_ObjEntityList[0] == npcObject || m_ObjEntityList[iListEndIndex] == npcObject ) 
		{
			AsEntityManager.Instance.BroadcastMessageToAllEntities(
	                	new Msg_Input_Move(vec3Target));
		}		
		else if( coliderBegin.bounds.Contains(vec3CharPos ) )
		{
			AsCommonSender.SendObjectJump(m_ObjEntityList[1].SessionId);			
		}
		else if( coliderEnd.bounds.Contains(vec3CharPos ) )
		{
			AsCommonSender.SendObjectJump(m_ObjEntityList[iListEndIndex-1].SessionId);		
			
		}
		else
		{
			float fFirstLengh = (m_ObjEntityList[0].transform.position - vec3CharPos).magnitude;
        	float fEndLengh = (m_ObjEntityList[iListEndIndex].transform.position - vec3CharPos).magnitude;
			
			AsNpcEntity RailNpcEntity = m_ObjEntityList[iListEndIndex];
	        if (fFirstLengh < fEndLengh)
	        {
	            RailNpcEntity = m_ObjEntityList[0];
	        }			
			
			AsEntityManager.Instance.BroadcastMessageToAllEntities(
	                	new Msg_Input_Move(RailNpcEntity.transform.position));
		}
		
		
		
	}	
}
