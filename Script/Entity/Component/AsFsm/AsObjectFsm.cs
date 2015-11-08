using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsObjectFsm : AsBaseComponent
{


    //---------------------------------------------------------------------
    /* enum */
    //---------------------------------------------------------------------
    public enum eObjectFsmStateType 
    { 
        NONE,  
        IDLE,	    
        STEPPING_LEAF,
        BREAK,
		POST_BOX,
		STORAGE,
		WAYPOINT,
    }


    //---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------
    public eObjectFsmStateType state_;
    protected AsNpcEntity m_ObjectEntity;
    protected Dictionary<eObjectFsmStateType, AsBaseFsmState<eObjectFsmStateType, AsObjectFsm>> m_dicFsmState =
        new Dictionary<eObjectFsmStateType, AsBaseFsmState<eObjectFsmStateType, AsObjectFsm>>();
    protected AsBaseFsmState<eObjectFsmStateType, AsObjectFsm> m_CurrentFsmState;
    protected AsBaseFsmState<eObjectFsmStateType, AsObjectFsm> m_OldFsmState;
    protected AsUserEntity m_Target;


    public AsNpcEntity ObjectEntity 
    { 
        get 
        {
            return m_ObjectEntity; 
        } 
    }

    public eObjectFsmStateType CurrnetFsmStateType 
    { 
        get 
        { 
            return m_CurrentFsmState.FsmStateType; 
        } 
    }
       
    
    public eObjectFsmStateType OldFsmStateType 
    { 
        get 
        { 
            return m_OldFsmState.FsmStateType; 
        } 
    }

    public AsUserEntity Target 
    { 
        get 
        { 
            return m_Target; 
        } 
        set 
        { 
            m_Target = value; 
        } 
    }


    public override void Init(AsBaseEntity _entity)
    {
        base.Init(_entity);

        
    }
	
	public override void InterInit(AsBaseEntity _entity)
	{
		m_dicFsmState.Add(eObjectFsmStateType.IDLE, new AsObjState_Idle(this));
        m_dicFsmState.Add(eObjectFsmStateType.BREAK, new AsObjState_Break(this));       
		m_dicFsmState.Add(eObjectFsmStateType.STEPPING_LEAF, new AsObjState_Stepping(this));
		m_dicFsmState.Add(eObjectFsmStateType.POST_BOX, new AsObjState_PostBox(this));
		m_dicFsmState.Add(eObjectFsmStateType.STORAGE, new AsObjState_Storage(this));
		m_dicFsmState.Add(eObjectFsmStateType.WAYPOINT, new AsObjState_Waypoint(this));
		
		
		switch( m_ObjectEntity.GetProperty<eOBJECT_PROP>(eComponentProperty.OBJ_TYPE) )
		{			
		case eOBJECT_PROP.STEPPING_LEAF:
            SetObjectFsmState(eObjectFsmStateType.STEPPING_LEAF);
			break;
			
		case eOBJECT_PROP.BROKEN_PROP:
			SetObjectFsmState(eObjectFsmStateType.BREAK);
			break;	
		case eOBJECT_PROP.POST_BOX:
			SetObjectFsmState(eObjectFsmStateType.POST_BOX);
			break;
		case eOBJECT_PROP.STORAGE:
			SetObjectFsmState(eObjectFsmStateType.STORAGE);
			break;
			
		case eOBJECT_PROP.WAYPOINT:
			SetObjectFsmState(eObjectFsmStateType.WAYPOINT);
			break;
			
		}
        
        gameObject.layer = LayerMask.NameToLayer("Object");
		Transform trs = gameObject.transform.FindChild( "Model" );
		if( null != trs )
		{
			trs.gameObject.layer = LayerMask.NameToLayer("Object");
		}
	}


     // Awake
    void Awake()
    {
        m_ComponentType = eComponentType.FSM_OBJECT;
		
		m_ObjectEntity = GetComponent<AsNpcEntity>();
        if (m_ObjectEntity == null) Debug.LogError("AsObjectFsm::Init: no user entity attached");
		m_ObjectEntity.FsmType = eFsmType.OBJECT;
        
        MsgRegistry.RegisterFunction(eMessageType.ANIMATION_END, OnAnimationEnd);			
        MsgRegistry.RegisterFunction(eMessageType.OBJ_BREAK_MSG, OnObjectBreak);      
		MsgRegistry.RegisterFunction(eMessageType.OBJ_TRAP_MSG, OnObjectTrap);
        MsgRegistry.RegisterFunction(eMessageType.OBJ_STEPPING_MSG, OnObjectStepping);
		MsgRegistry.RegisterFunction(eMessageType.NPC_CLICK, OnNpcClick);
		MsgRegistry.RegisterFunction(eMessageType.NPC_BEGIN_DIALOG, OnNpcBeginDialog);
    }

	// Use this for initialization
	void Start () 
    {
        
	}
	
	void OnDisable()
	{
		m_CurrentFsmState.Exit();
	}
	
	// Update is called once per frame
	void Update () 
    {
        m_CurrentFsmState.Update();
	}


    void FixedUpdate()
    {
    }


    public void SetObjectFsmState(eObjectFsmStateType _type, AsIMessage _msg)
    {
        if (m_CurrentFsmState != null)
        {
            m_CurrentFsmState.Exit();
            m_OldFsmState = m_CurrentFsmState;
        }
        //		else
        //			Debug.LogWarning("[AsBaseFsm]SetFsmState: current state");

        if (m_dicFsmState.ContainsKey(_type) == true)
        {
            state_ = _type;
            m_CurrentFsmState = m_dicFsmState[_type];
            m_CurrentFsmState.Enter(_msg);
        }
    }


    public void SetObjectFsmState(eObjectFsmStateType _type)
    {
        SetObjectFsmState(_type, null);
    }

    void OnAnimationEnd(AsIMessage _msg)
    {
        m_CurrentFsmState.MessageProcess(_msg);
    }

    void OnObjectBreak(AsIMessage _msg)
    {
        m_CurrentFsmState.MessageProcess(_msg);
    }
	
	void OnObjectTrap(AsIMessage _msg)
    {
        m_CurrentFsmState.MessageProcess(_msg);
    }

    void OnObjectStepping(AsIMessage _msg)
    {
        m_CurrentFsmState.MessageProcess(_msg);
    }
	
	void OnNpcClick(AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
//		if( null != AsHudDlgMgr.Instance && (true == AsHudDlgMgr.Instance.IsOpenTrade || true == AsHudDlgMgr.Instance.IsOpenEnchantDlg || true == AsHudDlgMgr.Instance.IsOpenStrengthenDlg) )
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}
	
	void OnNpcBeginDialog(AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}
}
