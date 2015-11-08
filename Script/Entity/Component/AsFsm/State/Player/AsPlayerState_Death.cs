using UnityEngine;
using System.Collections;

public class AsPlayerState_Death : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm>
{
	#region - member -
	float m_Timer;
	float m_UnitTime = 5;
	
	#endregion
	#region - init -
	public AsPlayerState_Death(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.DEATH, _fsm)
	{
		m_dicMessageTreat.Add( eMessageType.CHAR_RESURRECTION, OnResurrection);
		m_dicMessageTreat.Add( eMessageType.APPLICATION_PAUSE, OnApplicationPause);
		m_dicMessageTreat.Add( eMessageType.ENTERWORLD_DEATH, OnEnterWorld_Death);
	}
	
	public override void Init ()
	{
		eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		eGENDER gender = m_OwnerFsm.Entity.GetProperty<eGENDER>(eComponentProperty.GENDER);
		m_Action = AsTableManager.Instance.GetTbl_Action_Record(__class, gender, "Death");
	}
	#endregion
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.Target = null;
		
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.LIVING, false);
		
		if(_msg != null && _msg.GetType() == typeof(Msg_EnterWorld_Death))
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death", true));
		else
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death"));
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		m_Timer = Time.time;
		
		m_OwnerFsm.SetAction(m_Action);
		m_OwnerFsm.PlayElements(eActionStep.Entire);
		
//		eCLASS eClass = m_OwnerFsm.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
//		eGENDER eGender = m_OwnerFsm.UserEntity.GetProperty<eGENDER>( eComponentProperty.GENDER);
//		Tbl_Action_Record actionRecord = AsTableManager.Instance.GetTbl_Action_Record( eClass, eGender, "Death");
//		Tbl_Action_Animation readyAnim = actionRecord.ReadyAnimation;
//		foreach( Tbl_Action_Sound sound in readyAnim.listSound)
//		{
//			AsSoundManager.Instance.PlaySound( sound.FileName, m_OwnerFsm.UserEntity.ModelObject.transform.position, false);
//		}
	}
	
	public override void Update()
	{
	}
	
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - msg -
	void OnResurrection( AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.LIVING, true);
		m_OwnerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.IDLE);
		m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate( "Idle"));
		
		Msg_Resurrection resurrect = _msg as Msg_Resurrection;
		if(m_OwnerFsm.UserEntity.UniqueId != resurrect.reviver_)
		{
			AsNotify.Instance.CloseDeathDlg();
			
			AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId(resurrect.reviver_);
			if(user != null)
			{
				string str = AsTableManager.Instance.GetTbl_String(370);
				str = string.Format(str, user.GetProperty<string>(eComponentProperty.NAME));
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), str);
				
				m_OwnerFsm.Entity.HandleMessage( new Msg_BuffRefresh( PlayerBuffMgr.Instance.CharBuffDataList));
			}			
		}
	}
	
	void OnApplicationPause(AsIMessage _msg)
	{
		Msg_ApplicationPause pause = _msg as Msg_ApplicationPause;
		if(pause.pause_ == false)
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death", true));
	}
	
	void OnEnterWorld_Death(AsIMessage _msg)
	{
		if(_msg.GetType() == typeof(Msg_EnterWorld_Death))
			m_OwnerFsm.Entity.HandleMessage(new Msg_AnimationIndicate("Death", true));
	}
	#endregion
	
	#region - method -
	void Timer()
	{
		if(Time.time - m_Timer > m_UnitTime)
		{
			AsEntityManager.Instance.RemoveEntity(m_OwnerFsm.UserEntity);
		}
	}
	#endregion
}
