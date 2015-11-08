using UnityEngine;
using System.Collections;

public class AsMonsterState_Death : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm>
{
	private string m_strAniName = "Death";
	
	public AsMonsterState_Death(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.DEATH, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	public override void Init ()
	{
		string __class = m_OwnerFsm.MonsterEntity.GetProperty<string>(eComponentProperty.CLASS);
		m_Action = AsTableManager.Instance.GetTbl_MonsterAction_Record(__class, m_strAniName);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.LIVING, false);
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		if(_msg.GetType() == typeof(Msg_EnterWorld_Death))
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death", true));
		else
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death"));
		
		CharacterController controller = m_OwnerFsm.GetComponent<CharacterController>();
		if(controller !=null)
			controller.enabled = false;
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire, 1);
		
		AsModel _Model = m_OwnerFsm.Entity.GetComponent( eComponentType.MODEL ) as AsModel;
		if(_Model)
			_Model.ShadowRemove();
		
		if(m_OwnerFsm.Renderers != null)
		{
			foreach( Renderer renderer in m_OwnerFsm.Renderers)
			{
				if(renderer != null)
					renderer.material.renderQueue = 3000;	// Transparent
			}
		}		
#if !DROP_TYPE_DIRECT
		if( 0 < m_OwnerFsm.MonsterEntity.getDropItems.Count )
		{
			foreach( AS_body2_SC_DROPITEM_APPEAR data in m_OwnerFsm.MonsterEntity.getDropItems )
			{	
				ItemMgr.DropItemManagement.InsertDropItem( data );
			}
		}
#endif
	}
	
	public override void Update()
	{
	}
	
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		m_OwnerFsm.DeathFade();
	}
}
