using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsCollectionFsm : AsBaseComponent
{
	public enum eCollectionFsmStateType
	{
		IDLE,
		ING,
		SUCC,
		FAILED
	}

	public eCollectionFsmStateType state_;

	AsNpcEntity m_CollectionEntity;
	public AsNpcEntity CollectionEntity	{ get { return m_CollectionEntity; } }

	protected Dictionary<eCollectionFsmStateType, AsBaseFsmState<eCollectionFsmStateType, AsCollectionFsm>> m_dicFsmState =
		new Dictionary<eCollectionFsmStateType, AsBaseFsmState<eCollectionFsmStateType, AsCollectionFsm>>();
	protected AsBaseFsmState<eCollectionFsmStateType, AsCollectionFsm> m_CurrentFsmState;
	public eCollectionFsmStateType CurrnetFsmStateType	{ get { return m_CurrentFsmState.FsmStateType; } }
	protected AsBaseFsmState<eCollectionFsmStateType, AsCollectionFsm> m_OldFsmState;
	public eCollectionFsmStateType OldFsmStateType	{ get { return m_OldFsmState.FsmStateType; } }

	void Awake()
	{
		m_ComponentType = eComponentType.FSM_COLLECTION;

		m_CollectionEntity = GetComponent<AsNpcEntity>();
		if( m_CollectionEntity == null)
			Debug.LogError( "AsCollectionFsm::Init: no other user entity attached");
		m_CollectionEntity.FsmType = eFsmType.COLLECTION;

		MsgRegistry.RegisterFunction( eMessageType.NPC_CLICK, OnNpcClick);
		MsgRegistry.RegisterFunction( eMessageType.COLLECT_START, OnCollectStart);
		MsgRegistry.RegisterFunction( eMessageType.COLLECT_INFO, OnCollectInfo);
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, FishLoadModel);
		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED_DUMMY, FishLoadModelDummy);

		m_dicFsmState.Add( eCollectionFsmStateType.IDLE, new AsCollectionState_Idle( this));
		m_dicFsmState.Add( eCollectionFsmStateType.ING, new AsCollectionState_Ing( this));
		m_dicFsmState.Add( eCollectionFsmStateType.SUCC, new AsCollectionState_Succ( this));
		m_dicFsmState.Add( eCollectionFsmStateType.FAILED, new AsCollectionState_Failed( this));
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);

		gameObject.layer = LayerMask.NameToLayer( "Collection");
	}

	public override void InterInit( AsBaseEntity _entity)
	{
		SetCollectionFsmState( eCollectionFsmStateType.IDLE);
	}

	public override void LateInit( AsBaseEntity _entity)
	{
	}

	void Start()
	{
	}

	void OnDestroy()
	{
		if( m_CurrentFsmState != null)
			m_CurrentFsmState.Exit();
	}

	public void SetCollectionFsmState( eCollectionFsmStateType _type, AsIMessage _msg)
	{
		if( m_CurrentFsmState != null)
		{
			if( m_CurrentFsmState.FsmStateType == _type)
			{
				Debug.LogWarning( "AsOtherUserFsm::SetOtherUserFsmState: same state = " + _type);
				return;
			}

			m_CurrentFsmState.Exit();
			m_OldFsmState = m_CurrentFsmState;
		}

		if( m_dicFsmState.ContainsKey( _type) == true)
		{
			state_ = _type;
			m_CurrentFsmState = m_dicFsmState[_type];
			m_CurrentFsmState.Enter( _msg);
		}
	}

	public void SetCollectionFsmState( eCollectionFsmStateType _type)
	{
		SetCollectionFsmState( _type, null);
	}

	void Update()
	{
		if( null != m_CurrentFsmState)
			m_CurrentFsmState.Update();
	}

	void OnNpcClick( AsIMessage _msg)
	{
		if( ( null != AsHudDlgMgr.Instance) && ( true == AsHudDlgMgr.Instance.IsDontMoveState))
			return;

		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCollectStart( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void OnCollectInfo( AsIMessage _msg)
	{
		m_CurrentFsmState.MessageProcess( _msg);
	}

	void FishLoadModel( AsIMessage _msg)
	{
		if ( m_CollectionEntity.FsmType != eFsmType.COLLECTION)
			return;

		AsNpcEntity npcEntity = ( AsNpcEntity)m_Entity;

		Tbl_Collection_Record record = AsTableManager.Instance.getColletionTable.GetRecord( npcEntity.TableIdx);

		if ( record != null && record.technic == eCOLLECTION_TECHNIC.QUEST)
		{
			//check
			npcEntity.collectionMark = m_Entity.gameObject.AddComponent<QuestCollectionMarkController>();
			npcEntity.collectionMark.Init( npcEntity.TableIdx, npcEntity.namePanel, (AsBaseEntity)npcEntity);
			npcEntity.collectionMark.UpdateQuestCollectionMark();
		}
	}

	void FishLoadModelDummy( AsIMessage _msg)
	{
		if ( m_CollectionEntity.FsmType != eFsmType.COLLECTION)
			return;

		AsNpcEntity npcEntity = ( AsNpcEntity)m_Entity;

		Tbl_Collection_Record record = AsTableManager.Instance.getColletionTable.GetRecord( npcEntity.TableIdx);

		if ( record != null && record.technic == eCOLLECTION_TECHNIC.QUEST)
		{
			//check
			npcEntity.collectionMark = m_Entity.gameObject.AddComponent<QuestCollectionMarkController>();
			npcEntity.collectionMark.Init( npcEntity.TableIdx, npcEntity.namePanel, (AsBaseEntity)npcEntity);
			npcEntity.collectionMark.UpdateQuestCollectionMark();
		}
	}
}
