using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPlayerState_SkillReady : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {//
	
	#region - member -
	string m_CurPlayingAnimName;
	
	Msg_Player_Skill_Ready m_ReadyMsg;
	
	float m_BeginTime;
	
	Tbl_Action_Animation m_ReadyAnimation;
	
	bool canceled = false;
	
	AsBaseEntity m_CurTarget;
	
	int[] m_MainNpcIdx = new int[TargetDecider.MAX_SKILL_TARGET ];
	uint[] m_MainUserIdx = new uint[TargetDecider.MAX_SKILL_TARGET];
	#endregion
	
	#region - init -
	public AsPlayerState_SkillReady(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.SKILL_READY, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.ANIMATION_END, OnAnimationEnd);
		
		m_dicMessageTreat.Add(eMessageType.INPUT_MOVE, OnBeginMove);
		m_dicMessageTreat.Add(eMessageType.INPUT_AUTO_MOVE, OnBeginAutoMove);
		m_dicMessageTreat.Add(eMessageType.OTHER_USER_CLICK, OnOtherUserClick);
		m_dicMessageTreat.Add(eMessageType.NPC_CLICK, OnNpcClick);
		m_dicMessageTreat.Add(eMessageType.COLLECT_CLICK, OnCollectClick);
		m_dicMessageTreat.Add(eMessageType.OBJECT_CLICK, OnObjectClick);
		m_dicMessageTreat.Add(eMessageType.INPUT_ATTACK, OnBeginAttack);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_TARGET_MOVE, OnTargetSkillMove);
		m_dicMessageTreat.Add(eMessageType.PLAYER_SKILL_READY, OnSkillReady);
		m_dicMessageTreat.Add(eMessageType.INPUT_CANCEL_CHARGE, OnCancelCharge);
		
		m_dicMessageTreat.Add(eMessageType.INPUT_DASH, OnBeginDash);
		
		m_dicMessageTreat.Add(eMessageType.CHEAT_DEATH, OnCheatDeath);
	}
	#endregion
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		canceled = false;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		//AsCommonSender.SendMove(eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
//		m_OwnerFsm.MovePacketRefresh( eMoveType.Sync_Stop, m_OwnerFsm.transform.position, m_OwnerFsm.transform.position);
		m_OwnerFsm.MovePacketSynchronize();
		
		ReadyProcess(_msg);
		
//		if( ( null != AsQuickSlotManager.Instance) && ( null != AsQuickSlotManager.Instance.waitingSlot))
//			AsQuickSlotManager.Instance.waitingSlot.OuterInvoke();
	}
	public override void Update()
	{
		CheckTargetLiving();
//		if(CheckTargetLiving() == true)
//			RefreshDirection();

		CheckAnimDuration();
	}
	public override void Exit()
	{
		if(canceled == true)
			AsCommonSender.EndSkillUseProcess();
		
		m_OwnerFsm.ReleaseElements(canceled);
	}
	#endregion
	
	#region - update -
	bool CheckTargetLiving()
	{
		if(m_CurTarget == null)
			return ReleaseCombat();
		
		if(m_CurTarget.ContainProperty(eComponentProperty.LIVING) == true)
		{
			if(m_CurTarget.GetProperty<bool>(eComponentProperty.LIVING) == false)
			{
				return ReleaseCombat();
			}
		}
		
		return true;
	}
	
	void RefreshDirection()
	{
		Vector3 dir = m_CurTarget.transform.position - m_OwnerFsm.Entity.transform.position;
		
		if( Vector3.zero == dir)
			return;
		
		m_OwnerFsm.Entity.transform.rotation = 
			Quaternion.Slerp(m_OwnerFsm.Entity.transform.rotation, 
				Quaternion.LookRotation(dir), 
				20.0f * Time.deltaTime);
	}
	
	void CheckAnimDuration()
	{
		if((m_ReadyMsg.actionRecord.ReadyAnimation.LoopType & (eLoopType.TimeLoop | 
				eLoopType.TimeLoop | eLoopType.Cast)) != 0)
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
		#region - prepare -
		bool succeed = false;
		m_BeginTime = Time.time;
		
		switch(_msg.MessageType)
		{
		case eMessageType.PLAYER_SKILL_READY:
			succeed = ReadyMessageDecode(_msg as Msg_Player_Skill_Ready);
			break;
		case eMessageType.PLAYER_SKILL_LINKACTION:
			succeed = ReadyMessageDecode(_msg as Msg_Player_Skill_Linkaction);
			break;
		default:
			Debug.LogError("AsPlayerState_SkillReady::ReadyProcess: invalid message typoe - " + _msg.MessageType);
			break;
		}
		
		if(succeed == false)
		{
			Debug.LogError("AsPlayerState_SkillReady::ReadyProcess: ready msg decoding is failed.");
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			return;
		}
		
		m_CurTarget = m_OwnerFsm.Target;
		m_ReadyMsg.SetCurrentTarget(m_CurTarget);
		
		m_MainNpcIdx = GetTargetSessioinIdNpc();
		m_MainUserIdx = GetTargetUniqIdUser();
		
		float animSpeed = GetAnimationSpeedByAttack_UserEntity(m_Action, eActionStep.Ready);
		m_ReadyMsg.SetAnimSpeed(animSpeed);
		#endregion
		#region - hud -
		if( System.Int32.MaxValue == m_ReadyMsg.skillLvRecord.ChargeMaxStep)
		{
//			if( null != AsQuickSlotManager.Instance)
//				AsQuickSlotManager.Instance.BeginCooltime( m_ReadyMsg.skillRecord.Index);
		}
		else
		{
			if( System.Int32.MaxValue == m_ReadyMsg.skillLvRecord.ChargeStep)
			{
				if( null != AsQuickSlotManager.Instance)
					AsQuickSlotManager.Instance.BeginCharge( m_ReadyMsg.skillRecord.Index);
			}
		}
		#endregion
		#region - cooltime -
		if(AsCommonSender.CheckSkillUseProcessing() == false && (
			m_ReadyMsg.skillRecord.Skill_Type != eSKILL_TYPE.Base &&
			m_ReadyMsg.skillRecord.Skill_Type != eSKILL_TYPE.SlotBase) &&
//			m_ReadyMsg.skillRecord.Skill_Type != eSKILL_TYPE.Stance) &&
			m_ReadyMsg.charging == false)
		{
			AsCommonSender.BeginSkillUseProcess( m_ReadyMsg.skillRecord.Index);
			
		}
		
		if(m_ReadyMsg.skillRecord.Skill_Type == eSKILL_TYPE.Base ||
			m_ReadyMsg.skillRecord.Skill_Type == eSKILL_TYPE.SlotBase)
		{
			eRACE race = m_OwnerFsm.Entity.GetProperty<eRACE>(eComponentProperty.RACE);
			eCLASS __class = m_OwnerFsm.Entity.GetProperty<eCLASS>(eComponentProperty.CLASS);
			Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record(race, __class);
			
			m_OwnerFsm.ElapsedCoolTime_BaseAttack = record.BaseAttackCycle * 0.001f / m_ReadyMsg.animSpeed;
		}
		#endregion
		#region - direction -
		switch(m_ReadyMsg.skillRecord.Attack_Direction)
		{
		case eATTACK_DIRECTION.Camera:
			m_OwnerFsm.transform.LookAt(Camera.mainCamera.transform);
			break;
		case eATTACK_DIRECTION.FingerPoint:
		case eATTACK_DIRECTION.LineEnd:
			m_OwnerFsm.transform.LookAt(GetWorldPositionFromCamera(m_ReadyMsg.tail));
			break;
		case eATTACK_DIRECTION.LineMiddle:
			m_OwnerFsm.transform.LookAt(GetWorldPositionFromCamera(m_ReadyMsg.direction));
			break;
		case eATTACK_DIRECTION.LineStart:
			m_OwnerFsm.transform.LookAt(GetWorldPositionFromCamera(m_ReadyMsg.head));
			break;
		case eATTACK_DIRECTION.Target:
			if(m_CurTarget != null)
				m_OwnerFsm.transform.LookAt(m_CurTarget.transform);
			break;
		default:
			break;
		}
		
		Vector3 angle = m_OwnerFsm.transform.eulerAngles;angle.x = 0f;
		m_OwnerFsm.transform.eulerAngles = angle;
		#endregion
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_CombatBegin());
		
		m_OwnerFsm.SetAction(m_Action);
		SendingPacket();
		
		#region - anim & fx & switch to hit -
		if(m_ReadyAnimation != null)
		{		
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
				SetTimer(float.PositiveInfinity);
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
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_ReadyMsg.animSpeed);
				break;
			case eLoopType.TargetLoop:
				anim = new Msg_AnimationIndicate(m_ReadyAnimation.FileName, 0.1f, m_ReadyMsg.animSpeed,
					m_ReadyAnimation.LoopTargetTime, m_ReadyAnimation.LoopDuration);
				break;
			default:
				Debug.LogError("AsPlayerState_SkillReady::ReadyProcess: invalid ready animation process. m_ReadyAnimation.LoopType = " + m_ReadyAnimation.LoopType);
				ChangeStateToHit();
				return;
			}
			
			m_CurPlayingAnimName = m_ReadyMsg.actionRecord.ReadyAnimation.FileName;
			m_OwnerFsm.UserEntity.HandleMessage(anim);			
			
			if(m_Action.AniBlendStep == eActionStep.Ready)
				m_OwnerFsm.Entity.HandleMessage(new Msg_FadeTimeIndicate(m_Action.AniBlendingDuration * 0.001f));
			
			if(m_CurTarget != null)
				m_OwnerFsm.PlayElements(eActionStep.Ready, m_ReadyMsg.picked, m_CurTarget.transform, m_ReadyMsg.animSpeed);
			else
				m_OwnerFsm.PlayElements(eActionStep.Ready, m_ReadyMsg.picked, null, m_ReadyMsg.animSpeed);
		}
		else
		{
			ChangeStateToHit();
		}
		#endregion
		
		ActionMove();
	}
	
	void ActionMove()
	{
		if(m_Action.ReadyAnimation == null)
			return;
		
		Msg_DashIndication dashIndication = null;
		
		float attSpeed = m_ReadyMsg.animSpeed;// m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			
		switch(m_Action.ReadyAnimation.MoveType)
		{
		case eReadyMoveType.Dash:
			dashIndication = new Msg_DashIndication(
				m_Action.ReadyAnimation.LoopDuration / attSpeed, m_Action.ReadyAnimation.MoveDistance * 0.01f,// * m_OwnerFsm.Character_Size,
				m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
			break;
		case eReadyMoveType.TargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_CurTarget != null)
			{
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.ReadyAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashIndication(
						animState.length * 1000 / attSpeed, m_OwnerFsm.targetDistance - 1,
						m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eReadyMoveType.BackTargetDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			if(m_CurTarget != null)
			{
				AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.HitAnimation.FileName];
				if(animState != null)
				{
					dashIndication = new Msg_DashBackIndication(
						animState.length * 1000 / attSpeed, m_Action.ReadyAnimation.MoveDistance * 0.01f,// * m_OwnerFsm.Character_Size,
						-m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				}
			}
			break;
		case eReadyMoveType.TabDash:
			if(m_OwnerFsm.Entity.ModelObjectAnimation == null)
				return;
			
			float dist = Vector3.Distance(m_OwnerFsm.transform.position, m_ReadyMsg.picked);			
			AnimationState animState = m_OwnerFsm.Entity.ModelObjectAnimation[m_Action.ReadyAnimation.FileName];
			Debug.Log("dist:" + dist + ", animState:" + animState);
			if(animState != null)
			{
				dashIndication = new Msg_DashIndication(
					animState.length * 1000 / attSpeed, dist - 1,
					m_OwnerFsm.transform.forward, m_OwnerFsm.transform.position);
				Debug.Log("dist:" + dist + ", animState.length * 1000 / attSpeed:" + animState.length * 1000 / attSpeed);
			}
			break;
		case eReadyMoveType.TabWarp:
			dist = Vector3.Distance(m_OwnerFsm.transform.position, m_ReadyMsg.picked);
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
	
//	int m_Layer = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("OtherUser") | 1 << LayerMask.NameToLayer("Monster"));
//	int m_Layer = (1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Default"));
	Vector3 GetWorldPositionFromCamera(Vector3 _screenPos)
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay(_screenPos);
		Vector3 screenPos = ray.origin;
		Vector3 dirVec = ray.direction;		
		Vector3 normalVec = Vector3.up;
		Vector3 planePos = m_OwnerFsm.transform.position;
		
		Vector3 resultVec  = planePos - screenPos;
	    float resultScalar = Vector3.Dot(resultVec, normalVec);
		float ratio = Vector3.Dot(dirVec, normalVec);
		if(ratio == 0f)
			return m_OwnerFsm.transform.position;
	    float resultRatio = resultScalar / ratio;
//	    Vector3 resultPos = screenPos + dirVec * resultRatio;
		return screenPos + dirVec * resultRatio;
		
//		Ray ray = Camera.mainCamera.ScreenPointToRay(_screenPos);
//		RaycastHit hit = new RaycastHit();
//		if(Physics.Raycast(ray, out hit, float.MaxValue, m_Layer) == true)
////		if(Physics.Raycast(ray, out hit) == true)
//		{
//			return hit.point;
//		}
//		
//		return Vector3.zero;
	}
	
	bool ReadyMessageDecode(Msg_Player_Skill_Ready _attack)
	{
		if(_attack.constructSucceed == false)
		{
			Debug.LogError("AsPlayerState_SkillReady::ReadyMessageDecode: invalid ready message. change state to IDLE");
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			return false;
		}
		
		m_ReadyMsg = _attack;
		m_Action = m_ReadyMsg.actionRecord;
		m_ReadyAnimation = m_Action.ReadyAnimation;
		
		return true;
	}
	
	bool ReadyMessageDecode(Msg_Player_Skill_Linkaction _link)
	{
//		Debug.Log("ReadyMessageDecode: link action [" + _link.ready_.actionRecord.ActionName + "] is played");
		
		if(_link.ready_.constructSucceed == false)
		{
			Debug.LogError("AsPlayerState_SkillReady::ReadyMessageDecode: invalid link message. change state to IDLE");
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE);
			return false;
		}
		
		m_ReadyMsg = _link.ready_;
		m_Action = m_ReadyMsg.actionRecord;
		m_ReadyAnimation = m_Action.ReadyAnimation;
		
		return true;
	}
	
	void SendingPacket()
	{
		AS_CS_CHAR_ATTACK_NPC attack = new AS_CS_CHAR_ATTACK_NPC(m_ReadyMsg.skillLvRecord.Skill_GroupIndex,
			m_ReadyMsg.skillLvRecord.Skill_Level, m_ReadyMsg.actionRecord.Index,
			0, false, true, m_ReadyMsg.itemSlot,
			m_MainNpcIdx, m_MainUserIdx, m_ReadyMsg.picked, m_OwnerFsm.transform.forward,
			0, 0, new body2_AS_CS_CHAR_ATTACK_NPC[0], new body3_AS_CS_CHAR_ATTACK_NPC[0]);
		
		if(m_OwnerFsm.showSendingPacket == true)
		{
			Debug.Log("AsPlayerState_SkillReady::SendingPacket: skill index=" + m_ReadyMsg.skillLvRecord.Skill_GroupIndex +
				" skill level=" + m_ReadyMsg.skillLvRecord.Skill_Level + " action index=" + m_ReadyMsg.actionRecord.Index +
				" charge step=" + attack.nChargeStep + " casting=" + attack.bCasting +
				" main target id=" + attack.nNpcIdx + " skill pos=" + attack.sTargeting + " m_OwnerFsm.transform.forward=" + m_OwnerFsm.transform.forward);
		}
	
		if( m_ReadyMsg.skillRecord.Skill_Type != eSKILL_TYPE.Stance)
		{
			byte[] bytes = attack.ClassToPacketBytes();
			AsCommonSender.Send(bytes);
		}
		else
		{
			body_CS_CHAR_SKILL_STANCE stance = new body_CS_CHAR_SKILL_STANCE();
			byte[] bytes = stance.ClassToPacketBytes();
			AsCommonSender.Send(bytes);
			
//			AsCommonSender.BeginSkillUseProcess( record.Index);
		}
	}
	#endregion
	
	#region - msg -
	void OnAnimationEnd(AsIMessage _msg)
	{
		Msg_AnimationEnd end = _msg as Msg_AnimationEnd;
		if(end.animName_ == m_CurPlayingAnimName &&
			(m_ReadyMsg.actionRecord.ReadyAnimation.LoopType & (eLoopType.NONE | eLoopType.Once | eLoopType.TargetLoop)) != 0)
		{
			ChangeStateToHit();
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
		
		if(CancelEnable() == true)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.RUN_NPC, _msg);
		}
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
			&& attack.enemy_ != m_CurTarget)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE, _msg);
		}
	}
	
	void OnTargetSkillMove(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			return;
		
		if(CancelEnable() == true)
//			&& attack.enemy_ != m_CurTarget)
		{
			canceled = true;
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN, _msg);
		}
	}
	
	void OnSkillReady(AsIMessage _msg)
	{
		switch(_msg.MessageType)
		{
		case eMessageType.PLAYER_SKILL_READY:
			
			if( AsCommonSender.CheckSkillUseProcessing() == true)
				return;
			
			bool permit = false;
			
			if(m_ReadyMsg.skillRecord.Skill_Type == eSKILL_TYPE.Charge)
				permit = true;// CancelEnableOnCharge();
			else
				permit = CancelEnable();
			
			if(permit == true)
			{
				m_OwnerFsm.ReleaseElements();
				ReadyProcess(_msg);
			}
			break;
		case eMessageType.INPUT_CANCEL_CHARGE:
			if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING);
			else
				m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
			break;
		}
	}
	
	void OnCancelCharge( AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.CheckCondition(eSkillIcon_Enable_Condition.Bind) == true)
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.CONDITION_BINDING);
		else
			m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
		
		m_OwnerFsm.ReleaseElements();
		AS_CS_CHAR_ATTACK_NPC cancelCharge = new AS_CS_CHAR_ATTACK_NPC(
			m_ReadyMsg.skillLvRecord.Skill_GroupIndex, m_ReadyMsg.skillLv,
			m_ReadyMsg.actionRecord.Index, -1, false, true, 0,
			new int[TargetDecider.MAX_SKILL_TARGET], new uint[TargetDecider.MAX_SKILL_TARGET],
			Vector3.zero, Vector3.zero,
			0, 0, new body2_AS_CS_CHAR_ATTACK_NPC[0], new body3_AS_CS_CHAR_ATTACK_NPC[0]);
		AsCommonSender.Send(cancelCharge.ClassToPacketBytes());		
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
	#endregion
	
	#region - method -
	bool ReleaseCombat()
	{
		return false;
	}
	
	bool CancelEnableOnCharge()
	{
		if(m_Action != null && m_Action.GetCancelEnable(eActionStep.Ready, Time.time - m_BeginTime) == true)
			return true;
		else
			return false;
	}
	
	bool CancelEnable()
	{
		if(m_Action != null)
		{
			float attSpeed = m_OwnerFsm.Entity.GetProperty<float>(eComponentProperty.ATTACK_SPEED);
			if(m_Action.GetCancelEnable(eActionStep.Ready, (Time.time - m_BeginTime) * attSpeed) == true)
			{
				return true;
			}
		}
		
		return false;
	}
	
	int[] GetTargetSessioinIdNpc()
	{
		int[] mainTargetIdx = new int[TargetDecider.MAX_SKILL_TARGET];
		
		if(m_ReadyMsg.actionRecord.HitAnimation != null && m_ReadyMsg.actionRecord.HitAnimation.hitInfo != null)
		{
			switch(m_ReadyMsg.actionRecord.HitAnimation.hitInfo.HitType)
			{
			case eHitType.Target:
			case eHitType.ProjectileTarget:
				if(m_CurTarget == null)
				{
					Debug.LogWarning("Target is null");
				}
				else if(m_CurTarget.FsmType == eFsmType.MONSTER)
				{
					AsNpcEntity monster = m_CurTarget as AsNpcEntity;
					mainTargetIdx[0] = monster.SessionId;
				}
				break;
			default:
				break;
			}
		}
		
		return mainTargetIdx;
	}
	
	uint[] GetTargetUniqIdUser()
	{
		uint[] mainTargetIdx = new uint[TargetDecider.MAX_SKILL_TARGET];
		
		if(m_ReadyMsg.actionRecord.HitAnimation != null && m_ReadyMsg.actionRecord.HitAnimation.hitInfo != null)
		{
			switch(m_ReadyMsg.actionRecord.HitAnimation.hitInfo.HitType)
			{
			case eHitType.Target:
			case eHitType.ProjectileTarget:
				if(m_CurTarget == null)
				{
					Debug.LogWarning("Target is null");
				}
				else if(m_CurTarget.FsmType == eFsmType.OTHER_USER ||
					m_CurTarget.FsmType == eFsmType.NPC)
				{
					AsUserEntity user = m_CurTarget as AsUserEntity;
					mainTargetIdx[0] = user.UniqueId;
				}
				break;
			default:
				break;
			}
		}
		
		return mainTargetIdx;
	}
	
	void ChangeStateToHit()
	{
		ReleaseTimer();
		
		if(m_Action.ReadyAnimation != null &&
			m_Action.ReadyAnimation.MoveType != eReadyMoveType.NONE)
		{
			m_OwnerFsm.MovePacketSynchronize_Skill();
		}
		
		Msg_Player_Skill_Hit hit = new Msg_Player_Skill_Hit(m_ReadyMsg, 
			m_ReadyMsg.skillLvRecord.ChargeStep, false,
//			m_MainNpcIdx, m_MainUserIdx,
			false);
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.SKILL_HIT, hit);
	}
	#endregion
}
