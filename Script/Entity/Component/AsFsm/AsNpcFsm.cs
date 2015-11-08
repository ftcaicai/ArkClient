using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eNpcFsmStateType{NONE, IDLE, WALK, SKILL}

public class AsNpcFsm : AsBaseComponent {

	public enum eNpcFsmStateType{NONE, IDLE, ACCOST, POPUP, WALK, PORTAL }
	
	#region - member -
	public static float s_DialogEnableDistance = 4.5f;
	public static float s_DialogReleaseDistance = 5f;
	public static float s_CollectEnableDistance = 2.5f;

//    static Object questMarkPrefab = null;
	
	public eNpcFsmStateType state_;
	AsNpcEntity m_NpcEntity;public AsNpcEntity NpcEntity{get{return m_NpcEntity;}}
	
	protected Dictionary<eNpcFsmStateType, AsBaseFsmState<eNpcFsmStateType, AsNpcFsm>> m_dicFsmState = 
		new Dictionary<eNpcFsmStateType, AsBaseFsmState<eNpcFsmStateType, AsNpcFsm>>();
	protected AsBaseFsmState<eNpcFsmStateType, AsNpcFsm> m_CurrentFsmState;
	public eNpcFsmStateType CurrnetFsmStateType{get{return m_CurrentFsmState.FsmStateType;}}
	protected AsBaseFsmState<eNpcFsmStateType, AsNpcFsm> m_OldFsmState;
	public eNpcFsmStateType OldFsmStateType{get{return m_OldFsmState.FsmStateType;}}	
	
	
	AsUserEntity m_Target;public AsUserEntity Target{get{return m_Target;}set{m_Target = value;}}
	#endregion
	#region - init & release -
	void Awake()
	{
		m_ComponentType = eComponentType.FSM_NPC;
		
		m_NpcEntity = GetComponent<AsNpcEntity>();
		if(m_NpcEntity == null) Debug.LogError("AsMonsterFsm::Init: no user entity attached");
		m_NpcEntity.FsmType = eFsmType.NPC;
		
//		MsgRegistry.RegisterFunction(eMessageType.CLICKED, OnClicked);
        MsgRegistry.RegisterFunction(eMessageType.MODEL_LOADED, FishLoadModel);
		MsgRegistry.RegisterFunction(eMessageType.MODEL_LOADED_DUMMY, FishLoadModelDummy);
		MsgRegistry.RegisterFunction(eMessageType.MOVE_NPC_INDICATION, OnMoveIndication);
		MsgRegistry.RegisterFunction(eMessageType.MOVE_END_INFORM, OnMoveEnd);
		MsgRegistry.RegisterFunction(eMessageType.NPC_BEGIN_DIALOG, OnNpcBeginDialog);
		
		m_dicFsmState.Add(eNpcFsmStateType.IDLE, new AsNpcState_Idle(this));
		m_dicFsmState.Add(eNpcFsmStateType.ACCOST, new AsNpcState_Accost(this));
		m_dicFsmState.Add(eNpcFsmStateType.POPUP, new AsNpcState_Popup(this));
		m_dicFsmState.Add(eNpcFsmStateType.WALK, new AsNpcState_Walk(this));
		m_dicFsmState.Add(eNpcFsmStateType.PORTAL, new AsNpcState_Portal(this));
	}
	
	public override void Init(AsBaseEntity _entity)
	{
		base.Init(_entity);
	}
	
	public override void InterInit(AsBaseEntity _entity)
	{
		m_dicFsmState[eNpcFsmStateType.IDLE].Init();
		m_dicFsmState[eNpcFsmStateType.ACCOST].Init();
		m_dicFsmState[eNpcFsmStateType.POPUP].Init();
		m_dicFsmState[eNpcFsmStateType.WALK].Init();
		
		gameObject.layer = LayerMask.NameToLayer("Npc");
	}
	
	public override void LateInit(AsBaseEntity _entity)
	{
		if( null == m_NpcEntity )
		{
			SetNpcFsmState(eNpcFsmStateType.IDLE);
			Debug.LogError("AsNpcFsm::LateInit()[ null == m_NpcEntity ] : " );
			return;
		}
		
		Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record(m_NpcEntity.TableIdx);
		if(npcRec == null)
		{
			SetNpcFsmState(eNpcFsmStateType.IDLE);
			Debug.LogError("AsNpcFsm::LateInit()[ null == npcRec ] : " + m_NpcEntity.TableIdx );
			return;
		}
		
		m_NpcEntity.npcWarpIndex = npcRec.warpIndex;		
		if( m_NpcEntity.isNoWarpIndex )			
		{				
			SetNpcFsmState(eNpcFsmStateType.IDLE);
		}
		else
		{			
			SetNpcFsmState(eNpcFsmStateType.PORTAL);
		}
	}
	
	void Start()
	{
		Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record(m_NpcEntity.TableIdx);
		if(npcRec != null)
		{
			gameObject.name += "<" + npcRec.NpcName + ">";
		}
	}
	
	void OnDisable()
	{
		m_CurrentFsmState.Exit();
	}
	#endregion
	#region - fsm -
	public void SetNpcFsmState(eNpcFsmStateType _type, AsIMessage _msg)
	{
		if(m_CurrentFsmState != null)
		{
			m_CurrentFsmState.Exit();
			m_OldFsmState = m_CurrentFsmState;
		}
		
		if(m_dicFsmState.ContainsKey(_type) == true)
		{
			state_ = _type;
			m_CurrentFsmState = m_dicFsmState[_type];
			m_CurrentFsmState.Enter(_msg);
		}
	}
	
	public void SetNpcFsmState(eNpcFsmStateType _type)
	{
		SetNpcFsmState(_type, null);
	}
	#endregion
	#region - update -
	void Update()
	{
		m_CurrentFsmState.Update();
	}
	#endregion
	#region - msg -
	void OnClicked(AsIMessage _msg)
	{
		
	}

    void FishLoadModel(AsIMessage _msg)
    {
        if (m_Entity.FsmType == eFsmType.NPC)
        {
            QuestHolderManager.instance.MakeQuestHolderNpc(m_NpcEntity.TableIdx);
            QuestHolderManager.instance.UpdateQuestHolder(m_NpcEntity.TableIdx);
			AsInGameEventManager.instance.UpdateByNpcEntity(m_NpcEntity);
        }

		CharacterController ctrl = GetComponent<CharacterController>();
		if(ctrl != null)
		{
			SphereCollider col = gameObject.AddComponent<SphereCollider>();
			col.radius = ctrl.radius;
		}
    }
	
	void FishLoadModelDummy(AsIMessage _msg)
	{
        if (m_Entity.FsmType == eFsmType.NPC)
        {
            QuestHolderManager.instance.MakeQuestHolderNpc(m_NpcEntity.TableIdx);
            QuestHolderManager.instance.UpdateQuestHolder(m_NpcEntity.TableIdx);
        }
	}
	
	void OnMoveIndication(AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess(_msg);
	}
	
	void OnMoveEnd(AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess(_msg);
	}
	
	void OnNpcBeginDialog(AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess(_msg);
	}
	#endregion
	#region - process
	#endregion
}
