using UnityEngine;
using System.Collections;

public class AsMonsterState_SkillReady : AsBaseFsmState<AsMonsterFsm.eMonsterFsmStateType, AsMonsterFsm> {//
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_NpcAttackChar1 m_AttackMsg;
	
	Tbl_MonsterSkillLevel_Record m_Skill;
	
	Vector3 m_EffectPos;
	
	Tbl_Action_Animation m_ReadyAnimation;
	#endregion
	
	public AsMonsterState_SkillReady(AsMonsterFsm _fsm) : base(AsMonsterFsm.eMonsterFsmStateType.SKILL_READY, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.MOVE_NPC_INDICATION, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.NPC_ATTACK_CHAR1, OnBeginReady);
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OwnerFsm.MonsterEntity.SetProperty(eComponentProperty.COMBAT, true);

		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
//		if(m_OwnerFsm.LastMoveIndication != null && m_OwnerFsm.LastMoveIndication.forceMove_ == false)
		if(m_OwnerFsm.CheckLastMoveIndicated() == true)
		{
			Debug.Log("AsMonsterState_SkillReady::Enter: last move indication = " + m_OwnerFsm.LastMoveIndication.targetPosition_);
			
			m_OwnerFsm.Entity.SetRePosition(m_OwnerFsm.LastMoveIndication.targetPosition_);
			m_OwnerFsm.LastMoveIndicationUsed();
		}
		
		ReadyProcess(_msg);
	}
	public override void Update()
	{
//		CheckTargetLiving();
//		if(CheckTargetLiving() == true)
//			RefreshDirection();
		if(m_Action != null)
		{
			CheckAnimDuration();
		}
	}
	public override void Exit()
	{
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_OwnerFsm.Target == null)
			return ReleaseCombat();
		
		if(m_OwnerFsm.Target.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_OwnerFsm.Target.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_OwnerFsm.Target.transform.position - m_OwnerFsm.Entity.transform.position;
		dir.y = 0;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
	}
	
	void CheckAnimDuration()
	{
		if((m_ReadyAnimation.LoopType & (eLoopType.TimeLoop | eLoopType.TimeLoop | eLoopType.Cast)) != 0)
		{
			if(TimeElapsed() == true)
			{
				ChangeStateToHit();
			}
		}
	}
	#endregion
	
	#region - ready to READY -
	void ReadyProcess(AsIMessage _msg)
	{
		bool succeed = false;
		
		switch(_msg.MessageType)
		{
		case eMessageType.NPC_ATTACK_CHAR1:
			succeed = ReadyMsgDecode(_msg as Msg_NpcAttackChar1);
			break;
		case eMessageType.NPC_ATTACK_LINKACTION:
			succeed = ReadyMsgDecode(_msg as Msg_NpcAttackChar_Link);
			break;
		}
		
		if(succeed == false)
		{
			Debug.LogError("AsMonsterState_SkillReady::ReadyProcess: ready msg decoding is failed.");
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
			return;
		}
		
		if( true == m_AttackMsg.casting_)
			AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_Mob_Cast(m_AttackMsg));
		
		int monsterId = m_OwnerFsm.Entity.GetProperty<int>(eComponentProperty.MONSTER_ID);
		Tbl_Monster_Record monsterRecord = AsTableManager.Instance.GetTbl_Monster_Record(monsterId);
		
		if(monsterRecord != null &&
			(monsterRecord.Grade == eMonster_Grade.DObject || monsterRecord.Grade == eMonster_Grade.QObject))
		{
//			Debug.Log("AsMonsterState_SkillReady:: monster grade = " + monsterRecord.Grade + ". direction will not change.");
		}
		else
		{
			if(m_OwnerFsm.Target != null)
			{
				if( m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.VIEW_HOLD) == false)
				{
					m_OwnerFsm.transform.LookAt(m_OwnerFsm.Target.transform);
					Vector3 rotation = m_OwnerFsm.transform.rotation.eulerAngles;
					rotation.x = 0;
					m_OwnerFsm.transform.rotation = Quaternion.Euler(rotation);
				}
			}
		}
		
		#region - warning skill -
		if(m_AttackMsg.skill_.Skill_Type == eSKILL_TYPE.Warning && m_OwnerFsm.Entity.ModelObject != null)
		{
			Debug.LogWarning("AsMonsterState_SkillReady::ReadyProcess: skill type = warning");
			AsEffectManager.Instance.PlayEffect( "Fx/Effect/COMMON/Fx_Common_Warning", m_OwnerFsm.Entity.ModelObject.transform, false, 0);
		}
		#endregion
		
		m_OwnerFsm.SetAction(m_Action);
		
		if(m_ReadyAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_NpcEntity(m_Action, eActionStep.Ready);
			m_AttackMsg.SetAnimSpeed(animSpeed);
			
			switch(m_ReadyAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_ReadyAnimation.LoopDuration * 0.001f);
				break;
			case eLoopType.Charge:
				if(m_AttackMsg.casting_ == true)
				{
					SetTimer(float.PositiveInfinity);
				}
				else
				{
					ChangeStateToHit();
				}
				break;
			}
			
			Msg_AnimationIndicate anim = null;		
			switch(m_ReadyAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
			case eLoopType.Charge:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_AttackMsg.animSpeed_);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_AttackMsg.animSpeed_,
					m_ReadyAnimation.LoopTargetTime, m_ReadyAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsMonsterState_SkillReady::ReadyProcess: invalid ready animation process. m_ReadyAnimation.LoopType = " + m_ReadyAnimation.LoopType);
				ChangeStateToHit();
				break;
			}
			
			m_CurPlayingAnimName = m_ReadyAnimation.FileName;
			anim.SetActionAnimation(m_ReadyAnimation);
			m_OwnerFsm.MonsterEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Ready)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
//			if(m_AttackMsg.target_ != null)
//				m_OwnerFsm.PlayElements(eActionStep.Ready, m_AttackMsg.targetPos_, m_OwnerFsm.Target.transform, m_AttackMsg.animSpeed_);
//			else
//				m_OwnerFsm.PlayElements(eActionStep.Ready, m_AttackMsg.animSpeed_);
			
			for( int i=0; i<m_AttackMsg.charUniqKey_.Length; ++i)
			{
				uint node = m_AttackMsg.charUniqKey_[i];
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);

				if( user != null)
					m_OwnerFsm.PlayElements(eActionStep.Ready,  m_AttackMsg.targetPos_, user.transform, m_AttackMsg.animSpeed_);
				else if( i == 0)
					m_OwnerFsm.PlayElements(eActionStep.Ready, m_AttackMsg.targetPos_, null, m_AttackMsg.animSpeed_);
			}
		}
		else
		{
			ChangeStateToHit();
		}
	}
	
	bool ReadyMsgDecode(Msg_NpcAttackChar1 _attack)
	{
		m_AttackMsg = _attack;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_AttackMsg.charUniqKey_[0]);		
		m_AttackMsg.SetTarget(m_OwnerFsm.Target);
		
		m_Skill = m_AttackMsg.skillLv_;
		m_Action = m_AttackMsg.action_;
		
		if(m_Action == null)
		{
			string name = m_OwnerFsm.Entity.GetProperty<string>(eComponentProperty.CLASS);
			Debug.LogError("AsMonsterState_SkillReady::ReadyMsgDecode: such indexed action is not exist on Monster<" + name + ">");
			return false;
		}
		
		m_ReadyAnimation = m_Action.ReadyAnimation;
		
		return true;
	}
	
	bool ReadyMsgDecode(Msg_NpcAttackChar_Link _link)
	{
		m_AttackMsg = _link.attack_;
		m_OwnerFsm.Target = AsEntityManager.Instance.GetUserEntityByUniqueId(m_AttackMsg.charUniqKey_[0]);
		
		m_Skill = m_AttackMsg.skillLv_;
		m_Action = m_AttackMsg.action_;
		
		m_ReadyAnimation = m_Action.ReadyAnimation;
		
		return true;
	}
	#endregion
	
	#region - msg -
	void OnBeginMove(AsIMessage _msg)
	{
//		float attSpeed = m_AttackMsg.animSpeed_;
		
		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
		if(move.forceMyself_ == true)
			m_OwnerFsm.SetLastMoveIndication(_msg as Msg_NpcMoveIndicate);
		else
			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
		
//		Msg_NpcMoveIndicate move = _msg as Msg_NpcMoveIndicate;
//		if(move.forceMove_ == true)
//		{
//			Msg_DashIndication dashIndication = new Msg_DashIndication(
//				m_Action.HitAnimation.LoopDuration / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,
//				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
//			
//			m_OwnerFsm.Entity.HandleMessage(dashIndication);
//		}
//		else
//			m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.RUN, _msg);
	}
	
	void OnBeginReady(AsIMessage _msg)
	{
		ReadyProcess(_msg);
	}
	
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName &&
			(m_ReadyAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToHit();
		}
	}
	#endregion
	
	#region - method -
	void ChangeStateToHit()
	{
		ReleaseTimer();
		Msg_NpcAttackChar_Hit hit = new Msg_NpcAttackChar_Hit(m_AttackMsg, m_Action, m_Skill);
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.SKILL_HIT, hit);
	}
	
	bool ReleaseCombat()
	{
		Debug.Log("Monster::skillReady: Target is released. move to IDLE state.");
		
		m_OwnerFsm.Target = null;
		m_OwnerFsm.SetMonsterFsmState(AsMonsterFsm.eMonsterFsmStateType.IDLE);
		return false;
	}
	#endregion
}
