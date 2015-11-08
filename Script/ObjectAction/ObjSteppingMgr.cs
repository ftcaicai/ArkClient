using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjSteppingMgr : MonoBehaviour 
{
    private static ObjSteppingMgr ms_kIstance = null;
    private Dictionary<int, ObjStepping> m_ObjSteppingList = new Dictionary<int, ObjStepping>();


    public static ObjSteppingMgr Instance
    {
        get
        {
            return ms_kIstance;
        }

    }

    public void InsertObjStepping(int iGroupIndex, int iLinkIndex, AsNpcEntity npcEntity)
    {
        if (true == m_ObjSteppingList.ContainsKey(iGroupIndex))
        {
            m_ObjSteppingList[iGroupIndex].AddObjEntity(iLinkIndex, npcEntity);
        }
        else
        {
            m_ObjSteppingList.Add(iGroupIndex, new ObjStepping(iGroupIndex, iLinkIndex, npcEntity));
        }
    }
	
	public ObjStepping GetObjStepping( int iGroupIndex )
	{
		if( false == m_ObjSteppingList.ContainsKey(iGroupIndex) )
			return null;
		return m_ObjSteppingList[iGroupIndex];
	}


    public void Clear()
    {
        m_ObjSteppingList.Clear();
    }
	
	public void Remove( int iGroupIndex )
	{
		if (true == m_ObjSteppingList.ContainsKey(iGroupIndex))
        {
			m_ObjSteppingList.Remove(iGroupIndex);            
        }
	}
	
	public void RemoveNpcIdx( int nNpcIdx )
	{		
		foreach( KeyValuePair<int, ObjStepping> data in m_ObjSteppingList )
		{
			data.Value.RemoveNpc( nNpcIdx );
			if( true == data.Value.IsEmpty() )
			{
				Debug.Log("Remove [ Npc Idx : " + nNpcIdx + " Group idx : " + data.Key );
				Remove( data.Key );
				return;
			}
		}
	}
	
	public void ClickObject( int iGroupIndex, AsNpcEntity npcObject, Vector3 vec3Target )
	{
		ObjStepping objStepping = GetObjStepping( iGroupIndex );
		if( null == objStepping )
		{
			Debug.LogError("null == objStepping");
			return;
		}
		
		objStepping.ClickObject( npcObject, vec3Target );
	}
	
	/*public bool IsEndObject( int iGroupIndex, AsNpcEntity npcObject )
	{
	}*/
	
	public bool IsEndObject( ObjStepping objstepping, AsNpcEntity npcObject )
	{
        int iLinkIndex = npcObject.GetProperty<int>(eComponentProperty.LINK_INDEX);

        return objstepping.GetBegintLinkIndex() == iLinkIndex || objstepping.GetEndLinkIndex() == iLinkIndex;
	}


    // Awake
    void Awake()
    {
        ms_kIstance = this;
    }

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
