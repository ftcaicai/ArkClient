using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetDecider {
	
	public static readonly int MAX_SKILL_TARGET = 5;
	
	AsPlayerFsm m_PlayerFsm = null;
	Msg_Player_Skill_Ready m_SkillReady = null;
	
//	int m_MainTarget_Npc; public int mainTarget_Npc{get{return m_MainTarget_Npc;}}
	int[] m_listTargets_Npc = new int[MAX_SKILL_TARGET]; public int[] listTargets_Npc{get{return m_listTargets_Npc;}}
//	uint m_MainTarget_User; public uint mainTarget_User{get{return m_MainTarget_User;}}
	uint[] m_listTargets_User = new uint[MAX_SKILL_TARGET]; public uint[] listTargets_User{get{return m_listTargets_User;}}
	DecidedEntities m_DecidedEntities = new DecidedEntities(); public DecidedEntities decidedEntities{get{return m_DecidedEntities;}}
	
	#region - main target -
	int GetTargetSessionIdNpc()
	{
		int mainTargetIdx = 0;
		
		if(m_SkillReady.actionRecord.HitAnimation == null || m_SkillReady.actionRecord.HitAnimation.hitInfo == null)
			return mainTargetIdx;
		
		switch(m_SkillReady.actionRecord.HitAnimation.hitInfo.HitType)
		{
		case eHitType.Target:
		case eHitType.ProjectileTarget:
			if(m_SkillReady.Target == null)
			{
				Debug.LogWarning("TargetDecider::GetTargetSession: Target is null");
				return int.MaxValue;
			}
			else if(m_SkillReady.Target.FsmType == eFsmType.MONSTER)
			{
				AsNpcEntity monster = m_SkillReady.Target as AsNpcEntity;
				mainTargetIdx = monster.SessionId;
			}
			break;
		default:
			break;
		}
		
		return mainTargetIdx;
	}
	uint GetTargetUniqIdUser()
	{
		uint mainTargetIdx = 0;
		
		if(m_SkillReady.actionRecord.HitAnimation == null || m_SkillReady.actionRecord.HitAnimation.hitInfo == null)
			return mainTargetIdx;
		
		switch(m_SkillReady.actionRecord.HitAnimation.hitInfo.HitType)
		{
		case eHitType.Target:
		case eHitType.ProjectileTarget:
			if(m_SkillReady.Target == null)
			{
				Debug.LogWarning("TargetDecider::GetTargetUniqIdUser: Target is null");
				return uint.MaxValue;
			}
			else if(m_SkillReady.Target.FsmType == eFsmType.OTHER_USER ||
				m_SkillReady.Target.FsmType == eFsmType.NPC)
			{
				AsUserEntity user = m_SkillReady.Target as AsUserEntity;
				mainTargetIdx = user.UniqueId;
			}
			break;
		default:
			break;
		}
		
		return mainTargetIdx;
	}
	#endregion
	#region - hit decision -
	public bool SetTarget(AsPlayerFsm _playerFsm, Msg_Player_Skill_Ready _ready)
	{
		m_PlayerFsm = _playerFsm;
		m_SkillReady = _ready;
		
		#region - main target -
//		m_MainTarget_Npc = GetTargetSessionIdNpc();
//		if( m_MainTarget_Npc != 0)
//			m_listTargets_Npc.Add( m_MainTarget_Npc);
		
//		m_MainTarget_User = GetTargetUniqIdUser();
//		if( m_MainTarget_User != 0)
//			m_listTargets_User.Add( m_MainTarget_User);
		#endregion
		
		m_listTargets_Npc = new int[MAX_SKILL_TARGET];
		m_listTargets_User = new uint[MAX_SKILL_TARGET];
		m_DecidedEntities.Clear();
		
//		Tbl_Action_HitInfo hitInfo = m_SkillReady.actionRecord.HitAnimation.hitInfo;
//		Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(m_SkillReady.skillLvRecord.Skill_GroupIndex);
		
		return SetEachTarget();
		/*
		switch(skill.Skill_Type)
		{
		#region - eSKILL_TYPE.Base -
		case eSKILL_TYPE.Base:
		case eSKILL_TYPE.SlotBase:
			switch(hitInfo.HitType)
			{
			case eHitType.Target:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			case eHitType.ProjectileTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			default:
				Debug.LogError("HitDecision: eSKILL_TYPE.Base doesnt treat hit type[" + hitInfo.HitType + "]");
				break;
			}
			break;
		#endregion
		#region - eSKILL_TYPE.Active -
		case eSKILL_TYPE.Active:
			switch(hitInfo.HitType)
			{
			case eHitType.NonTarget://self
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision("self");
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_PlayerFsm.transform.position);
					break;
				}
				break;
			default:
				Debug.LogError("HitDecision: eSKILL_TYPE.Active doesnt treat hit type[" + hitInfo.HitType + "]");
				break;
			}
			break;
		#endregion
		#region - eSKILL_TYPE.Command -
		case eSKILL_TYPE.Command:
			switch(hitInfo.HitType)
			{
			case eHitType.NonTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision("self");
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_PlayerFsm.transform.position);
					break;
				}
				break;
			case eHitType.PositionTarget:
				switch(hitInfo.AreaShape)
				{
//				case eHitAreaShape.Point:
//					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.picked);
					break;
				}
				break;
			default:
				Debug.LogError("HitDecision: eSKILL_TYPE.Command cannot treat hit type[" + hitInfo.HitType + "]");
				break;
			}
			break;
		#endregion
		#region - eSKILL_TYPE.Target -
		case eSKILL_TYPE.Target:
			switch(hitInfo.HitType)
			{
			case eHitType.Target:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			case eHitType.NonTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision("self");
					break;
//				case eHitAreaShape.Circle:
//					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
//					break;
				}
				break;
			case eHitType.PositionTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			case eHitType.ProjectileTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			default:
				Debug.LogError("HitDecision: eSKILL_TYPE.Target doesnt treat hit type[" + hitInfo.HitType + "]");
				break;
			}
			break;
		#endregion
		#region - eSKILL_TYPE.Charge -
		case eSKILL_TYPE.Charge:
		case eSKILL_TYPE.TargetCharge:
			switch(hitInfo.HitType)
			{
			case eHitType.Target:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			case eHitType.NonTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision("self");
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_PlayerFsm.transform.position);
					break;
				}
				break;
			case eHitType.PositionTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			case eHitType.ProjectileTarget:
				switch(hitInfo.AreaShape)
				{
				case eHitAreaShape.Point:
					SingleTargetDecision();
					break;
				case eHitAreaShape.Circle:
					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
					break;
				}
				break;
			default:
				Debug.LogError("HitDecision: eSKILL_TYPE.Charge doesnt treat hit type[" + hitInfo.HitType + "]");
				break;
			}
			break;
		#endregion
		#region - etc -
		case eSKILL_TYPE.Stance:
			break;
		#endregion
		default:
			Debug.LogError("TargetDecider::SetTarget: invalid eSKILL_TYPE [" + skill.Skill_Type + "]");
			break;
		}
		//*/
		return true;
	}
	//*
	bool SetEachTarget()
	{
		Tbl_Action_HitInfo hitInfo = m_SkillReady.actionRecord.HitAnimation.hitInfo;
		Tbl_Skill_Record skill = AsTableManager.Instance.GetTbl_Skill_Record(m_SkillReady.skillLvRecord.Skill_GroupIndex);
		
		#region - getting specific multiple target -
//		float minDist = hitInfo.HitMinDistance_ * 0.01f * m_PlayerFsm.Character_Size;
//		float maxDist = hitInfo.HitMaxDistance_ * 0.01f * m_PlayerFsm.Character_Size;
		
		List<AsNpcEntity> listNpc = new List<AsNpcEntity>();
		
		listNpc.AddRange( AsEntityManager.Instance.GetMonsterInRange( m_PlayerFsm.transform.position, 0, m_SkillReady.skillLvRecord.Usable_Distance * 0.01f));
		listNpc.Sort( delegate( AsNpcEntity _a, AsNpcEntity _b) {
			
			float aDist = ( m_PlayerFsm.transform.position - _a.transform.position).sqrMagnitude;
			float bDist = ( m_PlayerFsm.transform.position - _b.transform.position).sqrMagnitude;
			
			if( aDist < bDist)
				return -1;
			else
				return 1;
			
		});
		
		if( m_SkillReady.Target != null && m_SkillReady.Target.EntityType == eEntityType.NPC)
			listNpc.Insert( 0, m_SkillReady.Target as AsNpcEntity);
		
		
		
		List<AsUserEntity> listUser = new List<AsUserEntity>();
		
		listUser.AddRange( AsEntityManager.Instance.GetUserEntityInRange( m_PlayerFsm.transform.position, 0, m_SkillReady.skillLvRecord.Usable_Distance * 0.01f));
		listUser.Sort( delegate( AsUserEntity _a, AsUserEntity _b) {
			
			float aDist = ( m_PlayerFsm.transform.position - _a.transform.position).sqrMagnitude;
			float bDist = ( m_PlayerFsm.transform.position - _b.transform.position).sqrMagnitude;
			
			if( aDist < bDist)
				return -1;
			else
				return 1;
			
		});
		
		if( m_SkillReady.Target != null && m_SkillReady.Target.EntityType == eEntityType.USER)
			listUser.Insert( 0, m_SkillReady.Target as AsUserEntity);
		#endregion
		
		for( int i=0; i<hitInfo.HitMultiTargetCount; ++i)
		{
			Vector3 curPos;
			
			switch(skill.Skill_Type)
			{
			#region - eSKILL_TYPE.Base -
			case eSKILL_TYPE.Base:
			case eSKILL_TYPE.SlotBase:
				switch(hitInfo.HitType)
				{
				case eHitType.Target:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				case eHitType.ProjectileTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				default:
					Debug.LogError("HitDecision: eSKILL_TYPE.Base doesnt treat hit type[" + hitInfo.HitType + "]");
					break;
				}
				break;
			#endregion
			#region - eSKILL_TYPE.Active -
			case eSKILL_TYPE.Active:
				switch(hitInfo.HitType)
				{
				case eHitType.NonTarget://self
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi("self");
						break;
					case eHitAreaShape.Circle:
						MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, m_PlayerFsm.transform.position);
						break;
					}
					break;
				default:
					Debug.LogError("HitDecision: eSKILL_TYPE.Active doesnt treat hit type[" + hitInfo.HitType + "]");
					break;
				}
				break;
			#endregion
			#region - eSKILL_TYPE.Command -
			case eSKILL_TYPE.Command:
				switch(hitInfo.HitType)
				{
				case eHitType.NonTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi("self");
						break;
					case eHitAreaShape.Circle:
						MultiTargetHitDecision_Multi(  i, hitInfo.AreaInfo, m_PlayerFsm.transform.position);
						break;
					}
					break;
				case eHitType.PositionTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Circle:
						MultiTargetHitDecision_Multi(  i, hitInfo.AreaInfo, m_SkillReady.picked);
						break;
					}
					break;
				default:
					Debug.LogError("HitDecision: eSKILL_TYPE.Command cannot treat hit type[" + hitInfo.HitType + "]");
					break;
				}
				break;
			#endregion
			#region - eSKILL_TYPE.Target -
			case eSKILL_TYPE.Target:
				switch(hitInfo.HitType)
				{
				case eHitType.Target:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				case eHitType.NonTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi("self");
						break;
	//				case eHitAreaShape.Circle:
	//					MultiTargetHitDecision(m_SkillReady.Target.transform.position);
	//					break;
					}
					break;
				case eHitType.PositionTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				case eHitType.ProjectileTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				default:
					Debug.LogError("HitDecision: eSKILL_TYPE.Target doesnt treat hit type[" + hitInfo.HitType + "]");
					break;
				}
				break;
			#endregion
			#region - eSKILL_TYPE.Charge -
			case eSKILL_TYPE.Charge:
			case eSKILL_TYPE.TargetCharge:
				switch(hitInfo.HitType)
				{
				case eHitType.Target:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				case eHitType.NonTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi("self");
						break;
					case eHitAreaShape.Circle:
						MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, m_PlayerFsm.transform.position);
						break;
					}
					break;
				case eHitType.PositionTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				case eHitType.ProjectileTarget:
					switch(hitInfo.AreaInfo[i].AreaShape)
					{
					case eHitAreaShape.Point:
						SingleTargetDecision_Multi( i, hitInfo.AreaInfo, listNpc, listUser);
						break;
					case eHitAreaShape.Circle:
						if( GetCurRotatedTargetPos( i, listNpc, listUser, out curPos) == true)
							MultiTargetHitDecision_Multi( i, hitInfo.AreaInfo, curPos);
						break;
					}
					break;
				default:
					Debug.LogError("HitDecision: eSKILL_TYPE.Charge doesnt treat hit type[" + hitInfo.HitType + "]");
					break;
				}
				break;
			#endregion
			#region - etc -
			case eSKILL_TYPE.Stance:
				break;
			#endregion
			default:
				Debug.LogError("TargetDecider::SetTarget: invalid eSKILL_TYPE [" + skill.Skill_Type + "]");
				break;
			}
		}
		
		return true;
	}
	/*/
	void SingleTargetDecision(string _self)
	{
		if(_self == "self")
		{
			AsUserEntity user = m_PlayerFsm.UserEntity;
			m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(
				user.SessionId, user.UniqueId));
		}
		else
			SingleTargetDecision();
	}
	
	void SingleTargetDecision()
	{
		if( null == m_SkillReady.Target)
			return;
		
		switch(m_SkillReady.Target.EntityType)
		{
		case eEntityType.USER:
			AsUserEntity user = m_SkillReady.Target as AsUserEntity;
			if( m_DecidedEntities.NpcTargetCount == 0)
			m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(user.SessionId, user.UniqueId));
			break;
		case eEntityType.NPC:
			AsNpcEntity monster = m_SkillReady.Target as AsNpcEntity;
			m_DecidedEntities.AddTarget(new body2_AS_CS_CHAR_ATTACK_NPC(monster.SessionId));
			break;
		}
		
		if(m_SkillReady.skillRecord.CheckPotencyTargetTypeIncludeSelf() == true)
			m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(m_PlayerFsm.UserEntity.SessionId, m_PlayerFsm.UserEntity.UniqueId));
	}
	
	void MultiTargetHitDecision(Vector3 _pos)
	{
		switch(m_SkillReady.actionRecord.HitAnimation.hitInfo.AreaShape)
		{
		case eHitAreaShape.Circle:
			CircleAreaHitDecision(_pos);
			break;
		}
		
		Debug.Log("Targeted monster entity count = " + m_DecidedEntities.TargetCount);
	}	
	
	void CircleAreaHitDecision(Vector3 _pos)
	{
		Tbl_Action_Record action = m_SkillReady.actionRecord;
		
		float hitAngle = action.HitAnimation.hitInfo.HitAngle;
		float centerAngle = action.HitAnimation.hitInfo.HitCenterDirectionAngle;
		float minDist = action.HitAnimation.hitInfo.HitMinDistance * 0.01f * m_PlayerFsm.Character_Size;
		float maxDist = action.HitAnimation.hitInfo.HitMaxDistance * 0.01f * m_PlayerFsm.Character_Size;
		
		// type is move
		switch(action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
		case eHitMoveType.TargetDash:
//		case eHitMoveType.TabDash:
//			float moveDist = action.HitAnimation.MoveDistance * 0.01f;
//			Vector3 dest = m_PlayerFsm.transform.forward * moveDist + m_PlayerFsm.transform.position;
//			float movableDist = m_PlayerFsm.Entity.GetNavPathDistance(m_PlayerFsm.transform.position, dest);
//			if(movableDist == 0f)
//			{
//				Debug.LogError("AsPlayerState_SkillHit::CircleAreaHitDecision: movableDist is 0. will be replaced by float.MaxValue.");
//				movableDist = float.MaxValue;
//			}
//			float decidedDist = maxDist - minDist;
//			float ratioDist = moveDist * decidedDist / movableDist;
//			maxDist = minDist + ratioDist;
//			break;
			
			
			float moveDist = action.HitAnimation.MoveDistance * 0.01f;
			Vector3 dest = m_PlayerFsm.transform.forward * moveDist + m_PlayerFsm.transform.position;
			Vector3 realDest = GetExpectedDest(m_PlayerFsm.transform.position, dest, moveDist);
			float realDist = Vector3.Dot(realDest - m_PlayerFsm.transform.position, m_PlayerFsm.transform.forward);
			maxDist = ((maxDist - minDist) - action.HitAnimation.MoveDistance * 0.01f) + minDist + realDist;
//			maxDist -= action.HitAnimation.MoveDistance * 0.01f;
				
			break;
		}
		
		Vector3 dir = m_PlayerFsm.transform.forward;
		Vector3 pos = _pos;
		
		Vector3 misc = new Vector3(action.HitAnimation.hitInfo.HitOffsetX * 0.01f * m_PlayerFsm.Character_Size, 0,
			action.HitAnimation.hitInfo.HitOffsetY * 0.01f * m_PlayerFsm.Character_Size);
		Vector3 revisedDir = Quaternion.AngleAxis( centerAngle, Vector3.up) * dir;
		Vector3 revisedPos = pos + Quaternion.LookRotation(revisedDir) * misc;
		
		

		List<AsNpcEntity> listMonster = AsEntityManager.Instance.GetMonsterInRange(
			revisedPos, minDist, maxDist);
		List<AsUserEntity> listChar = AsEntityManager.Instance.GetUserEntityInRange(
			revisedPos, minDist, maxDist, true);	
		
//		Debug.Log("revisedDir = " + revisedDir + ", current dir = " + m_PlayerFsm.transform.forward);
//		Debug.Log("revisedPos = " + revisedPos + ", current pos = " + m_PlayerFsm.transform.position);
		
		if(hitAngle != 360f)
		{
			listMonster = GetEntityInAngleRanged(revisedDir, revisedPos, hitAngle, listMonster);
			listChar = GetEntityInAngleRanged(revisedDir, revisedPos, hitAngle, listChar);
		}
		
		foreach(AsNpcEntity monster in listMonster)
		{
			m_DecidedEntities.AddTarget(new body2_AS_CS_CHAR_ATTACK_NPC(monster.SessionId));//, monster.transform.position));
		}
		foreach(AsUserEntity user in listChar)
		{
			if(user.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == false)
				m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(user.SessionId, user.UniqueId));
		}
		
		m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(m_PlayerFsm.UserEntity.SessionId, m_PlayerFsm.UserEntity.UniqueId));
		
		Targeting_NonTarget(listMonster, listChar);
	}
	//*/
	
	void SingleTargetDecision_Multi(string _self)
	{
		if(_self == "self")
		{
			AsUserEntity user = m_PlayerFsm.UserEntity;
			m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(
				user.SessionId, user.UniqueId));
		}
	}
	
	void SingleTargetDecision_Multi( int _idx, List<Tbl_Action_AreaInfo> _info, List<AsNpcEntity> _listNpc, List<AsUserEntity> _listUser)
	{
		if( m_SkillReady.Target == null)
			return;
		
		switch(m_SkillReady.Target.EntityType)
		{
		case eEntityType.NPC:
			if( _idx < _listNpc.Count)
			{
				m_listTargets_Npc[_idx] = _listNpc[_idx].SessionId;
				m_DecidedEntities.AddTarget( new body2_AS_CS_CHAR_ATTACK_NPC(_listNpc[_idx].SessionId)); 
			}
			break;
		case eEntityType.USER:
			if( _idx < _listUser.Count)
			{
				m_listTargets_User[_idx] = _listUser[_idx].UniqueId;
				m_DecidedEntities.AddChar( new body3_AS_CS_CHAR_ATTACK_NPC( _listUser[_idx].SessionId ,_listUser[_idx].UniqueId)); 
			}
			break;
		}
		
		if(m_SkillReady.skillRecord.CheckPotencyTargetTypeIncludeSelf() == true)
			m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(m_PlayerFsm.UserEntity.SessionId, m_PlayerFsm.UserEntity.UniqueId));
	}
	
	void MultiTargetHitDecision_Multi( int _idx, List<Tbl_Action_AreaInfo> _info, Vector3 _pos)
	{
		switch(_info[_idx].AreaShape)
		{
		case eHitAreaShape.Circle:
			CircleAreaHitDecision_Multi( _idx, _info, _pos);
			break;
		}
	}
	void CircleAreaHitDecision_Multi( int _idx, List<Tbl_Action_AreaInfo> _info, Vector3 _pos)
	{
		Tbl_Action_Record action = m_SkillReady.actionRecord;
		Tbl_Action_AreaInfo area = _info[_idx];
		
		Vector3 pos = _pos;
		
		float hitAngle = area.HitAngle;
		float centerAngle = area.HitCenterDirectionAngle;
		float minDist = area.HitMinDistance * 0.01f * m_PlayerFsm.Character_Size;
		float maxDist = area.HitMaxDistance * 0.01f * m_PlayerFsm.Character_Size;
		float hitOffsetX = area.HitOffsetX;
		float hitOffsetY = area.HitOffsetY;
		
		// type is move
		switch(action.HitAnimation.MoveType)
		{
		case eHitMoveType.Dash:
		case eHitMoveType.TargetDash:
			float moveDist = action.HitAnimation.MoveDistance * 0.01f;
			Vector3 dest = m_PlayerFsm.transform.forward * moveDist + m_PlayerFsm.transform.position;
			Vector3 realDest = GetExpectedDest(m_PlayerFsm.transform.position, dest, moveDist);
			float realDist = Vector3.Dot(realDest - m_PlayerFsm.transform.position, m_PlayerFsm.transform.forward);
			maxDist = ((maxDist - minDist) - action.HitAnimation.MoveDistance * 0.01f) + minDist + realDist;
			
			break;
		}
		
		Vector3 dir = m_PlayerFsm.transform.forward;
		
		Vector3 misc = new Vector3( hitOffsetX * 0.01f * m_PlayerFsm.Character_Size, 0,
			hitOffsetY * 0.01f * m_PlayerFsm.Character_Size);
		Vector3 revisedDir = Quaternion.AngleAxis( centerAngle, Vector3.up) * dir;
		Vector3 revisedPos = pos + Quaternion.LookRotation(revisedDir) * misc;
		
		

		List<AsNpcEntity> listMonster = AsEntityManager.Instance.GetMonsterInRange(
			revisedPos, minDist, maxDist);
		List<AsUserEntity> listChar = AsEntityManager.Instance.GetUserEntityInRange(
			revisedPos, minDist, maxDist, true);	
		
//		Debug.Log("revisedDir = " + revisedDir + ", current dir = " + m_PlayerFsm.transform.forward);
//		Debug.Log("revisedPos = " + revisedPos + ", current pos = " + m_PlayerFsm.transform.position);
		
		if(hitAngle != 360f)
		{
			listMonster = GetEntityInAngleRanged(revisedDir, revisedPos, hitAngle, listMonster);
			listChar = GetEntityInAngleRanged(revisedDir, revisedPos, hitAngle, listChar);
		}
		
		foreach(AsNpcEntity monster in listMonster)
		{
			m_DecidedEntities.AddTarget(new body2_AS_CS_CHAR_ATTACK_NPC(monster.SessionId));//, monster.transform.position));
		}
		foreach(AsUserEntity user in listChar)
		{
			if(user.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == false)
				m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(user.SessionId, user.UniqueId));
		}
		
		m_DecidedEntities.AddChar(new body3_AS_CS_CHAR_ATTACK_NPC(m_PlayerFsm.UserEntity.SessionId, m_PlayerFsm.UserEntity.UniqueId));
		
		Targeting_NonTarget(listMonster, listChar);
	}
	
	bool GetCurRotatedTargetPos( int _idx, List<AsNpcEntity> _listNpc, List<AsUserEntity> _listUser, out Vector3 _pos)
	{
		_pos = Vector3.zero;
		
		if( m_SkillReady.Target == null)
			return false;
		
		switch(m_SkillReady.Target.EntityType)
		{
		case eEntityType.NPC:
			if( _idx < MAX_SKILL_TARGET && _idx < _listNpc.Count)
			{
				m_listTargets_Npc[_idx] = _listNpc[_idx].SessionId;
				_pos = _listNpc[_idx].transform.position;
				return true;
			}
			break;
		case eEntityType.USER:
			if( _idx < MAX_SKILL_TARGET && _idx < _listUser.Count)
			{
				m_listTargets_User[_idx] = _listUser[_idx].UniqueId;
				_pos = _listUser[_idx].transform.position;
				return true;
			}
			break;
		}
		
		return false;
	}
	
	Vector3 GetExpectedDest(Vector3 _pos1, Vector3 _pos2, float _dest)
	{
		Vector3[] path = NavMeshFinder.Instance.PathFind_type1(_pos1, _pos2);
		
		if( null == path )
			return _pos1;
		
		float dist = 0;
//		if(path.Length > 0)
//			dist = Vector3.Distance(_pos1, path[0]);
		
		for(int i=0; i<path.Length - 1; ++i)
		{
			dist += Vector3.Distance(path[i], path[i+1]);
			if(dist > _dest)
				return path[i+1];
		}
		
		return path[path.Length - 1];
	}
	
	void Targeting_NonTarget(List<AsNpcEntity> _listMonster, List<AsUserEntity> _listChar)
	{
//		if(TerrainMgr.Instance.IsCurMapType(eMAP_TYPE.Pvp) == true)
//			return;
		
		Tbl_Action_HitInfo hitInfo = m_SkillReady.actionRecord.HitAnimation.hitInfo;
		if((hitInfo.HitType == eHitType.NonTarget || hitInfo.HitType == eHitType.PositionTarget) &&
		(m_SkillReady.Target == null ||
		m_SkillReady.Target != null &&
		((m_SkillReady.Target.FsmType == eFsmType.NPC || m_SkillReady.Target.FsmType == eFsmType.COLLECTION) ||
		(m_SkillReady.Target.FsmType == eFsmType.OTHER_USER && !CheckTargetUserIsEnemy(m_PlayerFsm)
			&& m_SkillReady.skillRecord.SkillAutoTarget != eSkillAutoTarget.Disable))))
		{
			if(m_SkillReady.skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true)
			{
				float curEnemyDist = float.MaxValue;
				float curObjDist = float.MaxValue;
				AsBaseEntity enemy = null;
				AsNpcEntity objNpc = null;
				
				foreach(AsNpcEntity npc in _listMonster)
				{
					float dist = Vector3.Distance(m_PlayerFsm.transform.position, npc.transform.position);
					if(npc.CheckObjectMonster() == false)
					{
						if(dist < curEnemyDist)
						{
							curEnemyDist = dist;
							enemy = npc;
						}
					}
					else
					{
						if(dist < curObjDist)
						{
							curObjDist = dist;
							objNpc = npc;
						}
					}
				}
				
				foreach(AsUserEntity user in _listChar)
				{
					if(CheckOtherUserIsEnemy(user) == true && user.GetProperty<bool>(eComponentProperty.LIVING) == true)
					{
						float dist = Vector3.Distance(m_PlayerFsm.transform.position, user.transform.position);
						if(dist < curEnemyDist)
						{
							curEnemyDist = dist;
							enemy = user;
						}
					}
				}
				
				if(enemy == null)
					m_PlayerFsm.Target = objNpc;
				else
					m_PlayerFsm.Target = enemy;
			}
		}
	}
	
	#region - get entities by angle -
	List<AsNpcEntity> GetEntityInAngleRanged(Vector3 _revisedDir, Vector3 _revisedPos, float _angle, List<AsNpcEntity> _entities)
	{
		List<AsNpcEntity> listTarget = new List<AsNpcEntity>();
		
		foreach(AsNpcEntity entity in _entities)
		{
			Vector3 targetDir = entity.transform.position - _revisedPos;
			
			float angle = Vector3.Angle(_revisedDir, targetDir);
			if(_angle * 0.5f > angle)
			{
				listTarget.Add(entity);
			}
		}
		
		return listTarget;
	}
	List<AsUserEntity> GetEntityInAngleRanged(Vector3 _revisedDir, Vector3 _revisedPos, float _angle, List<AsUserEntity> _entities)
	{
		List<AsUserEntity> listTarget = new List<AsUserEntity>();
		
		foreach(AsUserEntity entity in _entities)
		{
			Vector3 targetDir = entity.transform.position - _revisedPos;
			
			float angle = Vector3.Angle(_revisedDir, targetDir);
			if(_angle * 0.5f> angle)
			{
				listTarget.Add(entity);
			}
		}
		
		return listTarget;
	}
	static SortedDictionary<float, AsBaseEntity> GetEntityDicInAngleRanged(Vector3 _revisedDir, Vector3 _revisedPos, float _angle, List<AsBaseEntity> _entities)
	{
		SortedDictionary<float, AsBaseEntity> dicTarget = new SortedDictionary<float, AsBaseEntity>();
		
		foreach(AsBaseEntity entity in _entities)
		{
			Vector3 targetDir = entity.transform.position - _revisedPos;
			
			float angle = Vector3.Angle(_revisedDir, targetDir);
			
			if(_angle * 0.5f > angle)
			{
				Vector3 cross = Vector3.Cross(_revisedDir, targetDir);
				float crossDot = Vector3.Dot( cross, Vector3.up);
				
				if( crossDot < 0)
					angle = 360f - angle;
				
				dicTarget.Add( angle, entity);
			}
		}
		
		return dicTarget;
	}
	#endregion
	#endregion
	#region - target -
	public static void sTargeting_AsSlot(AsPlayerFsm _playerFsm, Tbl_Skill_Record _skillRecord)// use targetting slot only 
	{
		if(_playerFsm.CheckMap_Village() == true)
			return;
		
		if( null == _playerFsm.Target) // no entity is targeted
		{
			if(_skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true)
			{
				AsBaseEntity target = _playerFsm.HostileEntityExistInRange( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 30).Value * 0.01f);
				_playerFsm.Target = target;
			}
		}
		else // taget is exist
		{
			if( _playerFsm.Target.FsmType == eFsmType.NPC || _playerFsm.Target.FsmType == eFsmType.COLLECTION)
			{
				if(_skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true)
				{
					AsBaseEntity target = _playerFsm.MonsterExistInRange( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 30).Value * 0.01f);
					if( target != null && target.CheckObjectMonster() == false)
						_playerFsm.Target = target;
				}
			}
			else if(_playerFsm.Target.FsmType == eFsmType.OTHER_USER)
			{
				if(CheckOtherUserIsEnemy(_playerFsm.Target) == false)
				{
					if( ( null != _skillRecord) && ( eSkillAutoTarget.Disable != _skillRecord.SkillAutoTarget))
					{
						if(_skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true &&
							_skillRecord.CheckPotencyTypeIncludeHeal() == false)
						{
							AsUserEntity targetUser = _playerFsm.Target as AsUserEntity;
							AsBaseEntity targetUserTarget = targetUser.GetGetterTarget();
							if( targetUserTarget != null && targetUserTarget.FsmType == eFsmType.MONSTER)
								_playerFsm.Target = targetUserTarget;
							else
								_playerFsm.Target = _playerFsm.MonsterExistInRange( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 30).Value * 0.01f);
						}
					}
				}
			}
			else if( _playerFsm.Target.FsmType == eFsmType.MONSTER)
			{
				if( _skillRecord.Skill_Type == eSKILL_TYPE.SlotBase && AsGameMain.GetOptionState(OptionBtnType.OptionBtnType_TargetChange) == true) // target already chose & slot base touched
				{
					float maxDist = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 138).Value * 0.01f;
					float angle = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 139).Value;
					List<AsBaseEntity> listEntities = AsEntityManager.Instance.GetHostileEntityInRange( _playerFsm.transform.position, 0.01f, maxDist);
					Debug.Log("TargetDecider::sTargeting_AsSlot: listEntities.Count = " + listEntities.Count);
					SortedDictionary<float, AsBaseEntity> angledEntities = GetEntityDicInAngleRanged( _playerFsm.transform.forward, _playerFsm.transform.position, angle, listEntities);
					Debug.Log("TargetDecider::sTargeting_AsSlot: angledEntities.Count = " + angledEntities.Count);
					
					foreach(KeyValuePair<float, AsBaseEntity> pair in angledEntities)
					{
						Debug.Log("TargetDecider::sTargeting_AsSlot: angledEntities angle = " + pair.Key);
					}
					
					int count = 0;
					AsBaseEntity selectedEntity = null;
					foreach(KeyValuePair<float, AsBaseEntity> pair in angledEntities)
					{
						if( _playerFsm.Target == pair.Value)
							continue;
						
						if( (count == 0 && pair.Key > 0) ||
							count > 0)
						{
							selectedEntity = pair.Value;
							break;
						}
						
						count++;
					}
					
					if( selectedEntity != null)
					{
						_playerFsm.Target = selectedEntity;
						AsMyProperty.Instance.AlertTargetChanged();
					}
				}
			}
		}
	}
	
	public static bool sSkillUsable_TargetCharge(AsPlayerFsm _playerFsm, Msg_Player_Skill_Ready _ready)// use targetting slot only 
	{
		float _range = _ready.skillLvRecord.Usable_Distance * 0.01f;// * _playerFsm.Character_Size;
		
		if( null == _playerFsm.Target) // no entity is targeted
		{
			if( _ready.skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true)
			{
				AsBaseEntity target = _playerFsm.HostileEntityExistInRange( _range);
				_playerFsm.Target = target;
			}
		}
		else // taget is exist
		{
			if( _playerFsm.Target.FsmType == eFsmType.NPC || _playerFsm.Target.FsmType == eFsmType.COLLECTION)
			{
				if(_ready.skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true)
				{
					AsBaseEntity target = _playerFsm.MonsterExistInRange( _range);
					if( target != null && target.CheckObjectMonster() == false)
						_playerFsm.Target = target;
				}
			}
			else if(_playerFsm.Target.FsmType == eFsmType.OTHER_USER)
			{
				if(CheckOtherUserIsEnemy(_playerFsm.Target) == false)
				{
					if( ( null != _ready.skillRecord) && ( eSkillAutoTarget.Disable != _ready.skillRecord.SkillAutoTarget))
					{
						if(_ready.skillRecord.CheckPotencyTargetTypeIncludeEnemy() == true &&
							_ready.skillRecord.CheckPotencyTypeIncludeHeal() == false)
						{
							AsUserEntity targetUser = _playerFsm.Target as AsUserEntity;
							AsBaseEntity targetUserTarget = targetUser.GetGetterTarget();
							if( targetUserTarget != null &&
								targetUserTarget.FsmType == eFsmType.MONSTER &&
								Vector3.Distance(_playerFsm.transform.position, targetUser.transform.position) < _range)
								_playerFsm.Target = targetUserTarget;
							else
								_playerFsm.Target = _playerFsm.MonsterExistInRange( _range);
						}
					}
				}
			}
		}
		
		if( null == _playerFsm.Target)
		{
			AsMyProperty.Instance.AlertInvalidTarget();
			return false;
		}
		else
			return TargetSkillCheck( _ready.skillRecord.Index, _playerFsm);
	}
	
	public static bool CheckValidSkill(AsPlayerFsm _playerFsm, Tbl_Skill_Record _record)// use targetting slot only 
	{
		if( ( _record.Skill_Type == eSKILL_TYPE.Target) || ( _record.Skill_Type == eSKILL_TYPE.SlotBase))
		{
			if( null == _playerFsm.Target)
			{
				if( true == _playerFsm.CheckMap_Village())
					AsMyProperty.Instance.AlertSkillInTown();
				else
					AsMyProperty.Instance.AlertInvalidTarget();
				return false;
			}
			else
				return true;
		}

		return true;
	}
	
	public static bool SkillTargetStateCheck( Tbl_Skill_Record skillRecord, AsBaseEntity entity, bool _showAlert = true)
	{
		bool ret = false;
		
		switch( skillRecord.SkillIcon_Enable_Condition)
		{
		case eSkillIcon_Enable_Condition.NONE:
			return true;
		case eSkillIcon_Enable_Condition.LowHealth:
			if(entity.ContainProperty(eComponentProperty.HP_CUR) == true)
			{
				float curHP = entity.GetProperty<float>( eComponentProperty.HP_CUR);
				if( curHP < skillRecord.SkillIcon_Enable_ConditionValue)
					ret = true;
			}
			break;
		case eSkillIcon_Enable_Condition.Death:
			if(entity.ContainProperty(eComponentProperty.LIVING) == true)
			{
				bool isDied = entity.GetProperty<bool>( eComponentProperty.LIVING);
				ret = !isDied;
			}
			break;
		case eSkillIcon_Enable_Condition.Movable:
			AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
			ret = player.CheckCondition( eSkillIcon_Enable_Condition.Movable) &&
				player.GetProperty<bool>(eComponentProperty.LIVING);
			break;
		default:
			ret = entity.CheckCondition( skillRecord.SkillIcon_Enable_Condition);
			break;
		}
		
		if( false == ret && _showAlert == true)
			AsMyProperty.Instance.AlertState();
		
		return ret;
	}
	
	static bool SkillTargetTypeCheck( Tbl_Skill_Record skillRecord, AsPlayerFsm fsm)
	{
		switch( skillRecord.SkillIcon_Enable_Target)
		{
		case eSkillIcon_Enable_Target.Self:
			return SkillTargetStateCheck( skillRecord, fsm.UserEntity);
		case eSkillIcon_Enable_Target.Target:
			return SkillTargetStateCheck( skillRecord, fsm.Target);
		}
		
		return true;
	}
	
	public static bool TargetSkillCheck(int _skillIdx, AsPlayerFsm fsm)
	{
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _skillIdx);
		if( null == skillRecord)
			return false;
	
		switch( skillRecord.Skill_TargetType)
		{
		case eSkill_TargetType.All:
			{
				if( eFsmType.NPC == fsm.Target.FsmType)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}

				return SkillTargetTypeCheck( skillRecord, fsm);
			}
			break;
		case eSkill_TargetType.Alliance:
			{
				if( eEntityType.NPC == fsm.Target.EntityType)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}
				else if( eFsmType.OTHER_USER == fsm.Target.FsmType)
				{
					if(CheckOtherUserIsEnemy(fsm.Target) == true)
					{
						AsMyProperty.Instance.AlertTarget();
						return false;
					}
				}

				return SkillTargetTypeCheck( skillRecord, fsm);
			}
			break;
		case eSkill_TargetType.Enemy:
			{
				if( eFsmType.OTHER_USER == fsm.Target.FsmType)
				{
					if(CheckOtherUserIsEnemy(fsm.Target) == false)
					{
						AsMyProperty.Instance.AlertTarget();
						return false;
					}
				}
				else if( eFsmType.MONSTER != fsm.Target.FsmType)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}

				return SkillTargetTypeCheck( skillRecord, fsm);
			}
			break;
		case eSkill_TargetType.Party:
			{
				if( eFsmType.OTHER_USER != fsm.Target.FsmType)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}
			
				if( false == AsPartyManager.Instance.IsPartying)
					return false;
				
				AS_PARTY_USER partyUser = AsPartyManager.Instance.GetPartyMember( ( ( AsUserEntity)fsm.Target).UniqueId);
				if( null == partyUser)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}
			}
			break;
		case eSkill_TargetType.Self:
			{
				if( eFsmType.PLAYER != fsm.Target.FsmType)
				{
					AsMyProperty.Instance.AlertTarget();
					return false;
				}

				return SkillTargetTypeCheck( skillRecord, fsm);
			}
		default:
			break;
		}
		
		return true;
	}

	// ex. mana
	public static bool IsAvailableSkill(int _skillIdx, int _skillLv, bool checkMP=true)
	{
		if( 0 == _skillIdx || 0 == _skillLv)
			return false;
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( false == userEntity.WeaponEquip)
		{
			AsMyProperty.Instance.AlertNoWeapon();
			return false;
		}

		if( false == AsSkillDelegatorManager.Instance.IsActionCancelTime( Time.time))
			return false;
		
		if( true == checkMP)
		{
			float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
			Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( _skillLv, _skillIdx);
			if( curMP < skillLevelRecord.Mp_Decrease)
			{
				AsMyProperty.Instance.AlertMP();
				return false;
			}
		}
		
		if( true == userEntity.CheckTargetShopOpening())
		{
			Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( _skillIdx);
			
			if( skillRecord.Skill_Type == eSKILL_TYPE.Target)
			{
				AsMyProperty.Instance.AlertTarget();
				return false;
			}
		}
		
		return true;
	}
	#endregion
	#region - method -
	static bool CheckTargetUserIsEnemy(AsPlayerFsm _player)
	{
//#if _PVP
//		return true;
		
		AsUserEntity user = _player.Target as AsUserEntity;
		if(user != null)
			return AsPvpManager.Instance.CheckUsersAreHostile(_player.UserEntity.UniqueId, user.UniqueId);
		return false;
//#else
//		return false;
//#endif
	}
	
	public static bool CheckOtherUserIsEnemy(AsBaseEntity _user)
	{
//#if _PVP
//		return true;
		
		if(CheckCurrentMapIsArena() == false)
			return false;
		
		AsUserEntity user = _user as AsUserEntity;
		if(user == null)
			return false;
		
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		if(player == null)
			return false;
		
		return AsPvpManager.Instance.CheckUsersAreHostile(player.UniqueId, user.UniqueId);
//#else
//		return false;
//#endif
	}
	
	public static bool CheckAffectingUserIsEnemy(AsBaseEntity _main, uint _affectorIdx)
	{
//#if _PVP
//		return true;
		
		AsUserEntity main = _main as AsUserEntity;
		AsUserEntity affector = AsEntityManager.Instance.GetUserEntityByUniqueId(_affectorIdx);
		
		if(main == null || affector == null)
			return false;
		
		return AsPvpManager.Instance.CheckUsersAreHostile(main.UniqueId, affector.UniqueId);
//#else
//		return false;
//#endif
	}
	
	static public bool CheckCurrentMapIsArena()
	{
//		Map map = TerrainMgr.Instance.GetCurrentMap();
//		if(TerrainMgr.Instance != null)
//			return map.MapData.getMapType == eMAP_TYPE.Pvp;
//		else			
//		{
//			Debug.LogError("TargetDecider::CheckCurrentMapIsArena: TerrainMgr.Instance is null");
//			return false;
//		}
		
		if( null == TerrainMgr.Instance)
		{
			Debug.Log("TargetDecider::CheckCurrentMapIsArena: TerrainMgr.Instance is null");
			return false;
		}

		Map map = TerrainMgr.Instance.GetCurrentMap();
		
		if( null == map)
		{
			if(AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
				Debug.LogWarning("TargetDecider::CheckCurrentMapIsArena: CurrentMap is null: " + TerrainMgr.Instance.GetCurMapID());

			return false;
		}
		
		return map.MapData.getMapType == eMAP_TYPE.Pvp;
	}
	
	static public bool CheckCurrentMapIsIndun()
	{
		if( null == TerrainMgr.Instance)
		{
			Debug.Log("TargetDecider::CheckCurrentMapIsIndun: TerrainMgr.Instance is null");
			return false;
		}

		Map map = TerrainMgr.Instance.GetCurrentMap();
		
		if( null == map)
		{
//			Debug.Log("TargetDecider::CheckCurrentMapIsIndun: CurrentMap is null: " + TerrainMgr.Instance.GetCurMapID());
			return false;
		}
		
		return map.MapData.getMapType == eMAP_TYPE.Indun;
	}

	static public bool CheckCurrentMapIsRaid()
	{		
		if( null == TerrainMgr.Instance)
		{
			Debug.LogError("TargetDecider::CheckCurrentMapIsArena: TerrainMgr.Instance is null");
			return false;
		}

		Map map = TerrainMgr.Instance.GetCurrentMap();
		
		if( null == map)
		{
			Debug.LogError("TargetDecider::CheckCurrentMapIsArena: CurrentMap is null: " + TerrainMgr.Instance.GetCurMapID());
			return false;
		}
		
		return map.MapData.getMapType == eMAP_TYPE.Raid;
	}

    static public bool CheckCurrentMapIsField()
	{		
		if( null == TerrainMgr.Instance)
		{
            Debug.LogError("TargetDecider::CheckCurrentMapIsField: TerrainMgr.Instance is null");
			return false;
		}

		Map map = TerrainMgr.Instance.GetCurrentMap();
		
		if( null == map)
		{
            Debug.LogError("TargetDecider::CheckCurrentMapIsField: CurrentMap is null: " + TerrainMgr.Instance.GetCurMapID());
			return false;
		}
		
		return 	map.MapData.getMapType == eMAP_TYPE.Field || 
				map.MapData.getMapType == eMAP_TYPE.Town || 
				map.MapData.getMapType == eMAP_TYPE.Tutorial ||
				map.MapData.getMapType == eMAP_TYPE.Summon;
	}
    
	public static bool CheckDisableSkillByMap(Tbl_Skill_Record _skillrecord)
	{
		if( null != _skillrecord)
		{
			if(CheckCurrentMapIsArena() == true && _skillrecord.DisableInPvP == eDisableInPvP.Disable)
			{
				AsMyProperty.Instance.AlertNotInPvp();
				return true;
			}
			if(CheckCurrentMapIsRaid() == true && _skillrecord.DisableInRaid == eDisableInRaid.Disable)
			{
				AsMyProperty.Instance.AlertNotInRaid();
				return true;
			}
			if(CheckCurrentMapIsField() == true && _skillrecord.DisableInField == eDisableInRaid.Disable)
			{
				AsMyProperty.Instance.AlertNotInField();
				return true;
			}
			if( CheckCurrentMapIsIndun() == true && _skillrecord.DisableInInDun == eDisableInInDun.Disable)
			{
				AsMyProperty.Instance.AlertNotInIndun();
				return true;
			}
		}
		
		return false;
	}
	
	public static bool CheckDisableSkillByCondition(AsPlayerFsm _player, Tbl_Skill_Record _record) // for auto combat
	{
		bool movable = _record.CheckSkillUsingOnly_Movable();
		
		if( false == _player.Entity.CheckCondition( eSkillIcon_Enable_Condition.Movable) &&
			true == movable)
		{
			AsMyProperty.Instance.AlertState();
			return false;
		}

		if( true == _player.Entity.CheckCondition( eSkillIcon_Enable_Condition.Stun) ||
			true == _player.Entity.CheckCondition( eSkillIcon_Enable_Condition.Freeze) ||
			true == _player.Entity.CheckCondition( eSkillIcon_Enable_Condition.Sleep) ||
			true == _player.Entity.CheckCondition( eSkillIcon_Enable_Condition.Fear))
		{
			AsMyProperty.Instance.AlertState();			
			return false;
		}
		
		return true;
	}
	
//	public static 
	
//	public static bool CheckDisableSkill_Arena(Tbl_Skill_Record _skillRecord)
//	{
//		if(CheckCurrentMapIsArena() == true && _skillRecord.DisableInPvP == eDisableInPvP.Disable)
//			return true;
//		else
//			return false;
//	}
//	
//	public static bool CheckDisableSkill_Raid(Tbl_Skill_Record _skillRecord)
//	{
//		if(CheckCurrentMapIsRaid() == true && _skillRecord.DisableInRaid == eDisableInRaid.Disable)
//			return true;
//		else
//			return false;
//	}
//	
//	public static bool CheckDisableSkill_Field(Tbl_Skill_Record _skillRecord)
//	{
//		if(CheckCurrentMapIsField() == true && _skillRecord.DisableInField == eDisableInRaid.Disable)
//			return true;
//		else
//			return false;
//	}
	#endregion
}
