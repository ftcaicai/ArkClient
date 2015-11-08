using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region - classes -
public class DecidedEntities
{
	List<body2_AS_CS_CHAR_ATTACK_NPC> listTarget_ = new List<body2_AS_CS_CHAR_ATTACK_NPC>();
	public List<body2_AS_CS_CHAR_ATTACK_NPC> listTarget{get{return listTarget_;}}
	List<body3_AS_CS_CHAR_ATTACK_NPC> listChar_ = new List<body3_AS_CS_CHAR_ATTACK_NPC>();
	public List<body3_AS_CS_CHAR_ATTACK_NPC> listChar{get{return listChar_;}}
	
	public int TargetCount{get{return listTarget_.Count;}}
	public int CharCount{get{return listChar_.Count;}}
	
//	List<AsNpcEntity> listNpcTarget = new List<AsNpcEntity>(); public int NpcTargetCount{get{return listNpcTarget.Count;}}
//	List<AsUserEntity> listUserTarget = new List<AsUserEntity>(); public int UserTargetCount{get{return listUserTarget.Count;}}
	
	public void AddTarget(body2_AS_CS_CHAR_ATTACK_NPC _target)
	{
		listTarget_.Add(_target);
	}
	
	public void AddChar(body3_AS_CS_CHAR_ATTACK_NPC _char)
	{
		bool notContain = true;
		foreach( body3_AS_CS_CHAR_ATTACK_NPC node in listChar_)
		{
			if( node.nSessionIdx == _char.nSessionIdx && 
				node.nCharUniqKey == _char.nCharUniqKey)
				notContain = false;
		}
		
		if( notContain == true)
			listChar_.Add(_char);
		else
			Debug.Log("DecidedEntities:: AddChar: nSessionIdx = " + _char.nSessionIdx + ", nCharUniqKey = " + _char.nCharUniqKey + ". ignore this info");
	}
	
	public void SetTargets(List<body2_AS_CS_CHAR_ATTACK_NPC> _list)
	{
		listTarget_ = _list;
	}
	
	public void SetChars(List<body3_AS_CS_CHAR_ATTACK_NPC> _list)
	{
		listChar_ = _list;
	}
	
//	public bool CheckNpcTargetContain( int _session)
//	{
//		foreach( AsNpcEntity node in listNpcTarget)
//		{
//			if( node.SessionId == _session)
//			{
//				return true;
//			}
//		}
//		
//		return false;
//	}
//	
//	public bool CheckUserTargetContain( ushort _session, uint _uniqKey)
//	{
//		foreach( AsUserEntity node in listUserTarget)
//		{
//			if( node.SessionId == _session &&
//				node.UniqueId == _uniqKey)
//			{
//				return true;
//			}
//		}
//		
//		return false;
//	}
	
	public void Clear()
	{
		listTarget_.Clear();
		listChar_.Clear();
		
//		listNpcTarget.Clear();
//		listUserTarget.Clear();
	}
}
#endregion

public class AsPlayerState_SkillHit : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Player_Skill_Hit m_HitMsg;
	
	bool m_TargetReleased = false;
	
	TargetDecider m_TargetDecider = new TargetDecider();
	
	float m_BeginTime;
	
	Tbl_Action_Animation m_HitAnimation;
	
	bool canceled = false;
	bool packetSend = false;
	
	AsBaseEntity m_PrevTarget = null;
	
	AsTimer m_SkillTimer = null;
	AsTimer m_SkillProjTimer = null;
	#endregion
	
	#region - init -
	public AsPlayerState_SkillHit(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.SKILL_HIT, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginAutoMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
		m_dicMessageTreat.Add( eMessageType.INPUT_SHIELD_END, OnSkillShieldEnd);
		m_dicMessageTreat.Add( eMessageType.FORCED_MOVEMENT_RESULT, OnForcedMovementResult);// maybe place this at playerfsm
	}
	#endregion
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_SkillTimer = null;
		m_SkillProjTimer = null;
		
		canceled = false;
		packetSend = false;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		HitProcess(_msg);
	}
	
	public override void Update()
	{
		CheckTargetLiving();
//		if(CheckTargetLiving() == true)
//			RefreshDirection();
		
		if(m_HitMsg.ready_.actionRecord != null)
		{
			CheckAnimDuration();
		}
	}
	
	public override void Exit()
	{
		if(canceled == true)
		{
			if(m_SkillTimer != null) m_SkillTimer.Release();
			if(m_SkillProjTimer != null) m_SkillProjTimer.Release();
		}
		
		if(packetSend == false)
		{
			AsCommonSender.EndSkillUseProcess();
			Debug.LogWarning("AsPlayer_SkillHit::Exit: packet is not send. skill using is reverted(AsCommonSender.usedSkillIdx = -1)");
		}
		
		m_OwnerFsm.ReleaseElements(canceled);
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_PrevTarget == null)
			return ReleaseCombat();
		
		if(m_PrevTarget.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_PrevTarget.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_PrevTarget.transform.position - m_OwnerFsm.Entity.transform.position;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				7.0f * Time.deltaTime);
	}
	
	void CheckAnimDuration()
	{
		switch(m_HitMsg.ready_.actionRecord.HitAnimation.LoopType)
		{
		case eLoopType.Loop:
		case eLoopType.TimeLoop:
		case eLoopType.Cast:
			if(TimeElapsed() == true)
				ChangeStateToFinish();
			break;
		}
	}
	#endregion
	
	#region - ready to HIT -
	void HitProcess(AsIMessage _msg)
	{
		m_HitMsg = _msg as Msg_Player_Skill_Hit;
		m_Action = m_HitMsg.ready_.actionRecord;
		
		m_PrevTarget = m_HitMsg.ready_.Target;
		
		m_BeginTime = Time.time;
		m_HitAnimation = m_Action.HitAnimation;
		
		if(m_HitAnimation != null)
		{
			float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Hit);
			m_HitMsg.ready_.SetAnimSpeed( animSpeed);
			
			switch(m_HitAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.TargetLoop:
			case eLoopType.Charge:
				break;
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
				SetTimer(m_HitAnimation.LoopDuration * 0.001f / m_HitMsg.ready_.animSpeed);
				break;
			}
			
			Msg_AnimationIndicate anim = null;			
			switch(m_HitAnimation.LoopType)
			{
			case eLoopType.Once:
			case eLoopType.Loop:
			case eLoopType.TimeLoop:
			case eLoopType.Cast:
			case eLoopType.Charge:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.ready_.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_HitAnimation.FileName, 0.1f, m_HitMsg.ready_.animSpeed,
					m_HitAnimation.LoopTargetTime, m_HitAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPlayerState_SkillHit:: HitProcess: invalid hit animation process. m_HitAnimation.LoopType = " + m_HitAnimation.LoopType);
				ChangeStateToFinish();
				return;
			}
			
			m_CurPlayingAnimName = m_HitAnimation.FileName;
			m_OwnerFsm.UserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Hit)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			ActionMove();
		
			if(m_TargetDecider.SetTarget(m_OwnerFsm, m_HitMsg.ready_) == true)
			{
				for( int i=0; i<m_TargetDecider.listTargets_Npc.Length; ++i)
				{
					int node = m_TargetDecider.listTargets_Npc[i];
					AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId( node);

					if( npc != null)
						m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.picked, npc.transform, m_HitMsg.ready_.animSpeed);
					else if( i == 0)
						m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.picked, null, m_HitMsg.ready_.animSpeed);
				}

				for( uint i =0; i<m_TargetDecider.listTargets_User.Length; ++i)
				{
					uint node = m_TargetDecider.listTargets_User[i];
					AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);

					if( user != null)
						m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.picked, user.transform, m_HitMsg.ready_.animSpeed);
					else if( i == 0)
						m_OwnerFsm.PlayElements(eActionStep.Hit, m_HitMsg.ready_.picked, null, m_HitMsg.ready_.animSpeed);
				}
				
				Tbl_Action_HitInfo hitInfo = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo;
				if(hitInfo.HitType == eHitType.ProjectileTarget && m_PrevTarget != null)
				{
					GenerateMissile();
				}
				else
				{
					PacketDelay();
				}
			}
			else
			{
				canceled = true;
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			}
		}
		else
		{
			ChangeStateToFinish();
		}
	}
	
	void ActionMove()
	{
		Msg_DashIndication dashIndication = null;
		
		float attSpeed = m_HitMsg.ready_.animSpeed;// m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			
		switch(m_Action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
			dashIndication = new Msg_DashIndication(
				m_Action.HitAnimation.LoopDuration / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,// * m_OwnerFsm.Character_Size,
				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			break;
		case eHitMoveType.TargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_PrevTarget != null)
			{
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashIndication(
						animState.length * 1000 / attSpeed, m_OwnerFsm.targetDistance - 1,
						m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eHitMoveType.BackTargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_PrevTarget != null)
			{
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashBackIndication(
						animState.length * 1000 / attSpeed, m_Action.HitAnimation.MoveDistance * 0.01f,// * m_OwnerFsm.Character_Size,
						-m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eHitMoveType.TabDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			float dist = Vector3.Distance(m_OwnerFsm.transform.position, m_HitMsg.ready_.picked);			
			AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
			Debug.Log("dist:" + dist + ", animState:" + animState);
			if(animState != null)
			{
				dashIndication = new Msg_DashIndication(
					animState.length * 1000 / attSpeed, dist - 1,
					m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				Debug.Log("dist:" + dist + ", animState.length * 1000 / attSpeed:" + animState.length * 1000 / attSpeed);
			}
			break;
		case eHitMoveType.TabWarp:
			dist = Vector3.Distance(m_OwnerFsm.transform.position, m_HitMsg.ready_.picked);
			Msg_WarpIndication warp = new Msg_WarpIndication(dist, m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			m_OwnerFsm.UserEntity.HandleMessage(warp);
			break;
		default:
			return;
		}
		
		if(dashIndication != null)
		{
			m_OwnerFsm.UserEntity.HandleMessage(dashIndication);
//			m_OwnerFsm.MovePacketSynchronize_Skill();
		}
	}
	
	void PacketDelay()
	{
		float delay = 0.1f;
		float time = m_Action.HitAnimation.hitInfo.HitTiming * 0.001f;// - delay;
		
		if(time < 0)
			time = 0;
		
		AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC(m_HitMsg.ready_.skillLvRecord.Skill_GroupIndex, 
			m_HitMsg.ready_.skillLvRecord.Skill_Level, m_HitMsg.ready_.actionRecord.Index,
			m_HitMsg.chargeStep_, m_HitMsg.casting_, false, m_HitMsg.ready_.itemSlot,
//			m_HitMsg.mainTargetNpc_, m_HitMsg.mainTargetUser_,
			m_TargetDecider.listTargets_Npc, m_TargetDecider.listTargets_User,
			m_HitMsg.ready_.picked, m_OwnerFsm.transform.forward,
			m_TargetDecider.decidedEntities.TargetCount, m_TargetDecider.decidedEntities.CharCount,
			m_TargetDecider.decidedEntities.listTarget.ToArray(), m_TargetDecider.decidedEntities.listChar.ToArray());
		
		float attSpeed = m_HitMsg.ready_.animSpeed;
		m_SkillTimer = AsTimerManager.Instance.SetTimer(time / attSpeed - delay, Timer_SendingPacket, attack);
	}
	
	void SendingPacket(AS_CS_CHAR_ATTACK_NPC _attack)
	{
		AS_CS_CHAR_ATTACK_NPC attack = _attack;
		
		if(m_OwnerFsm.showSendingPacket == true)
		{
			Tbl_Action_Record action = AsTableManager.Instance.GetTbl_Action_Record(attack.nActionTableIdx);
			
			Debug.Log("AsPlayerState_SkillHit::SendingPacket: action name=" + action.ActionName +
				" skill index=" + attack.nSkillTableIdx +
				" skill level=" + attack.nSkillLevel + " action index=" + attack.nActionTableIdx +
				" charge step=" + attack.nChargeStep + " casting=" + attack.bCasting +
				" main target id=" + attack.nNpcIdx[0] + " skill pos=" + attack.sTargeting +
				" m_OwnerFsm.transform.forward=" + m_OwnerFsm.transform.forward);
		}
	
		if( m_HitMsg.ready_.skillRecord.Skill_Type != eSKILL_TYPE.Stance)
		{
			byte[] bytes = attack.ClassToPacketBytes();
			AsCommonSender.Send(bytes);
		}
		
		packetSend = true;
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName &&
			(m_HitMsg.ready_.actionRecord.HitAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToFinish();
		}
	}
	
	void OnBeginMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
		}
	}
	
	void OnBeginAutoMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN, _msg);
		}
	}
	
	void OnOtherUserClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
//		if(CancelEnable() == true)
//		{
//			canceled = true;
//			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OTHERUSER, _msg);
//		}
	}
	
	void OnNpcClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
//		if(CancelEnable() == true)
//		{
//			canceled = true;
//			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
//		}
	}
	
	void OnCollectClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_COLLECT, _msg);		
		}
	}
	
	void OnObjectClick(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_OBJECT, _msg);
		}
	}
	
	void OnBeginAttack(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		Msg_Input_Attack attack = _msg as Msg_Input_Attack;
		
		if(CancelEnable() == true
			&& attack.enemy_ != m_PrevTarget)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
		}
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.ReleaseElements();
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _msg);
		}
	}
	
	void OnBeginDash(AsIMessage _msg)
	{
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.DASH, _msg);
		}
	}
	
	void OnCheatDeath( AsIMessage _msg)
	{
		m_OwnerFsm.CheatDeath( _msg);
	}
	
	void OnSkillShieldEnd(AsIMessage _msg)
	{
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
	}
	
	List<bool> m_ForcedMovementResult = new List<bool>();
	void OnForcedMovementResult(AsIMessage _msg)
	{
		Msg_ForcedMovementResult result = _msg as Msg_ForcedMovementResult;
		
		m_ForcedMovementResult[result.index_] = true;
		
		bool complete = true;
		foreach(bool element in m_ForcedMovementResult)
		{
			if(element == false)
			{
				complete = false;
				break;
			}
		}
		
		if(complete == true)
		{
			m_ForcedMovementResult.Clear();
//			SendingPacket();
		}
	}
	#endregion

	#region - method -
	void ChangeStateToFinish()
	{
		ReleaseTimer();
		Msg_Player_Skill_Finish finish = new Msg_Player_Skill_Finish(m_HitMsg.ready_, m_TargetReleased);
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_FINISH, finish);
		
		switch(m_Action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
		case eHitMoveType.TargetDash:
		case eHitMoveType.TabDash:
		case eHitMoveType.TabWarp:
			m_OwnerFsm.MovePacketRefresh( eMoveType.Normal, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);//stop
			break;
		}
	}
	
	bool ReleaseCombat()
	{
		m_TargetReleased = true;
		return false;
	}
	
	bool CancelEnable()
	{
		if(m_Action != null)
		{
			float attSpeed = m_HitMsg.ready_.animSpeed;
			if(m_Action.GetCancelEnable(eActionStep.Hit, (Time.time - m_BeginTime) * attSpeed) == true)
				return true;
		}
		
		return false;
	}
	#endregion
	
	#region - gizmo -
	public override void OnDrawGizmos()
	{
		/*
		if(m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.AreaShape != eHitAreaShape.Circle)
			return;
		
		float hitAngle = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitAngle;
		float centerAngle = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitCenterDirectionAngle;
		float minDist = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitMinDistance * 0.01f * m_OwnerFsm.Character_Size;
		float maxDist = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitMaxDistance * 0.01f * m_OwnerFsm.Character_Size;
		Vector3 misc = new Vector3(m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitOffsetX * 0.01f,
			0, m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitOffsetY * 0.01f);
		
		Vector3 pos = m_OwnerFsm.transform.position;
		if(m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo.HitType == eHitType.PositionTarget)
			pos = m_HitMsg.ready_.picked;
		Vector3 revisedDir = Quaternion.AngleAxis( centerAngle, Vector3.up) * m_OwnerFsm.transform.forward;
		Vector3 revisedPos = pos + Quaternion.LookRotation(revisedDir) * misc * m_OwnerFsm.Character_Size;
		
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(revisedPos, minDist);
		Gizmos.DrawWireSphere(revisedPos, maxDist);
		
		Vector3 dir = revisedDir * maxDist;
		Vector3 rot1 = Quaternion.AngleAxis( hitAngle / 2, Vector3.up) * dir;
		Vector3 rot2 = Quaternion.AngleAxis( - hitAngle / 2, Vector3.up) * dir;
		
		Gizmos.color = Color.black;
		Gizmos.DrawLine(revisedPos, revisedPos + rot1);
		Gizmos.DrawLine(revisedPos, revisedPos + rot2);
		*/
	}
	#endregion
	
	#region - timer -
	void Timer_SendingPacket(System.Object _obj)
	{
		
		switch(m_OwnerFsm.CurrnetFsmStateType)
		{
		case AsPlayerFsm.ePlayerFsmStateType.CONDITION_STUN:
			break;
		default:
			SendingPacket(_obj as AS_CS_CHAR_ATTACK_NPC);
			SkillNameShout();
			break;
		}		
	}
	
	void Timer_GenerateMissile(System.Object _obj)
	{
		Msg_Player_Skill_Hit hit = _obj as Msg_Player_Skill_Hit;
		if(hit == null)
		{
			Debug.LogError( "m_HitMsg ================= null");
			return;
		}
		
		if( null == m_OwnerFsm)
		{
			Debug.Log( "AsPlayerState_SkillHit::Timer_GenerateMissile(), m_OwnerFsm == null");
			return;
		}

		if(m_PrevTarget == null)
		{
			Debug.Log( "AsPlayerState_SkillHit::Timer_GenerateMissile(), m_PrevTarget == null");
			return;
		}
		
		Tbl_Action_HitInfo hitInfo = hit.ready_.actionRecord.HitAnimation.hitInfo;
		
		float delay = 0.3f;
		float dist = Vector3.Distance(m_OwnerFsm.transform.position, m_PrevTarget.transform.position);
		float expectedTime = dist / (hitInfo.HitProjectileSpeed * 0.01f);
		float moveTime = expectedTime - delay + (expectedTime * expectedTime * 0.4f);
				
		AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC(hit.ready_.skillLvRecord.Skill_GroupIndex, 
			hit.ready_.skillLvRecord.Skill_Level, hit.ready_.actionRecord.Index,
			hit.chargeStep_, hit.casting_, false, hit.ready_.itemSlot,
//			hit.mainTargetNpc_, hit.mainTargetUser_,
			m_TargetDecider.listTargets_Npc, m_TargetDecider.listTargets_User,
			hit.ready_.picked, m_OwnerFsm.transform.forward,
			m_TargetDecider.decidedEntities.TargetCount, m_TargetDecider.decidedEntities.CharCount,
			m_TargetDecider.decidedEntities.listTarget.ToArray(), m_TargetDecider.decidedEntities.listChar.ToArray());
		
		AsTimerManager.Instance.SetTimer(moveTime, Timer_SendingPacket, attack);
		
		if(m_OwnerFsm.Entity != null && m_OwnerFsm.Entity.ModelObject != null)// &&
//			m_PrevTarget != null)
		{
//			if(m_PrevTarget.ModelObject != null)
//			{
//				AsEffectManager.Instance.ShootingEffect(
//					hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, m_PrevTarget.ModelObject.transform, Missile_HitExecution,
//					hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel, hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
//				
//				
//			}
//			else
//			{
//				AsEffectManager.Instance.ShootingEffect(
//					hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, m_PrevTarget.transform.position, Missile_HitExecution,
//					hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel, hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
//			}
			
			
			foreach(int node in m_TargetDecider.listTargets_Npc)
			{
				AsNpcEntity npc = AsEntityManager.Instance.GetNpcEntityBySessionId( node);
				if( npc != null)
				{
					if( npc.ModelObject != null)
					{
						AsEffectManager.Instance.ShootingEffect(
							hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, npc.ModelObject.transform, Missile_HitExecution,
							hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
							hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
					}
					else
					{
						AsEffectManager.Instance.ShootingEffect(
							hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, npc.transform.position, Missile_HitExecution,
							hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
							hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
					}
				}
			}
			
			foreach(uint node in m_TargetDecider.listTargets_User)
			{
				AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( node);
				if( user != null)
				{
					if( user.ModelObject != null)
					{
						AsEffectManager.Instance.ShootingEffect(
							hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, user.ModelObject.transform, Missile_HitExecution,
							hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
							hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
					}
					else
					{
						AsEffectManager.Instance.ShootingEffect(
							hitInfo.HitProjectileName, m_OwnerFsm.Entity.ModelObject.transform, user.transform.position, Missile_HitExecution,
							hitInfo.HitProjectileSpeed, hitInfo.HitProjectileAccel,
							hitInfo.HitProjectilePath, m_OwnerFsm.Entity.transform.localScale.y, hitInfo.HitProjectileHitSoundPath);
					}
				}
			}
		}
	}
	
	//missile
	void GenerateMissile()
	{	
		// projectile target
//		Tbl_Action_HitInfo hitInfo = m_HitMsg.ready_.actionRecord.HitAnimation.hitInfo;
		
//		if(hitInfo.HitType == eHitType.ProjectileTarget)
//		{
			float time = m_Action.HitAnimation.hitInfo.HitTiming * 0.001f;// - delay;
			float attSpeed = m_HitMsg.ready_.animSpeed;
			m_SkillProjTimer = AsTimerManager.Instance.SetTimer(time / attSpeed, Timer_GenerateMissile, m_HitMsg);
			
//			Debug.Log("AsPlayerState_SkillHit::GenerateMissile: (time:" + System.DateTime.Now + ") projectile target =  " + hitInfo.HitProjectileHitFileName);
//		}
	}
	
	void Missile_HitExecution(System.Object _obj)
	{
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_HitExecution());
	}
	#endregion
	
	#region - skill name shout -
	void SkillNameShout()
	{
		if(m_HitMsg.ready_.skillRecord.SkillName_Print != eSkillNamePrint.None)
		{
			string skillName = AsTableManager.Instance.GetTbl_String(m_HitMsg.ready_.skillRecord.SkillName_Index);
			m_OwnerFsm.Entity.SkillNameShout(eSkillNameShoutType.Self, skillName);
		}
	}
	#endregion
}
