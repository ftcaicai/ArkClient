using UnityEngine;
using System.Collections;

public class AsObjState_PostBox : AsBaseFsmState<AsObjectFsm.eObjectFsmStateType, AsObjectFsm>
{
#if false	
	private bool m_isTempNewMail = false;
#endif
	private GameObject m_goAlam =null;
	
    public AsObjState_PostBox(AsObjectFsm _fsm)
        : base(AsObjectFsm.eObjectFsmStateType.POST_BOX, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
	}

    public override void Enter(AsIMessage _msg)
    {
       
    }
    public override void Update()
    {
#if false
       	if( null != AsUserInfo.Instance && m_isTempNewMail != AsUserInfo.Instance.NewMail )
		{
			if( true == AsUserInfo.Instance.NewMail )
			{
				CreateNewMail( "UI/Post/Prefab/GUI_NewPost" );
			}
			else 				
			{
				DeleteAlam();
			}
			
			m_isTempNewMail = AsUserInfo.Instance.NewMail;
		}
#endif
    }
    public override void Exit()
    {
		DeleteAlam();
    }
	
	void OnNpcClick(AsIMessage _msg)
	{
		AsHudDlgMgr.Instance.OpenPostBoxDlg();		
	}
				
	
	private void DeleteAlam()
	{
		if( null != m_goAlam )
			GameObject.Destroy( m_goAlam );
	}
#if false	
	private void CreateNewMail( string strPath )
	{
		if (null == m_goAlam)
        {
           GameObject goRes = ResourceLoad.LoadGameObject(strPath);
	        if (null == goRes)
	        {
	            Debug.LogError("CreateNewMail()[(null == goRes] path : " + strPath);
	            return;
	        } 
			
			m_goAlam = GameObject.Instantiate(goRes) as GameObject;		
        }
                
        m_goAlam.transform.localPosition = Vector3.zero;
        m_goAlam.transform.localRotation = Quaternion.identity;
        m_goAlam.transform.localScale = Vector3.one;
		
		if( null == m_OwnerFsm.Entity || null == m_OwnerFsm.Entity.ModelObject )
			return;
		

		Transform trs = ResourceLoad.SearchHierarchyTransform( m_OwnerFsm.ObjectEntity.transform, "DummyLeadTop" );
		Vector3 worldPos = m_OwnerFsm.ObjectEntity.ModelObject.transform.position;
		if( null != trs )
		{
			worldPos = trs.position;
		}		
		else
		{
			Debug.LogWarning("CreateNewMail() [ no find Bone_head_02 ]");
		}  
		
		worldPos.y += 1.0f;        
        m_goAlam.transform.position = worldPos;
	}
#endif
}