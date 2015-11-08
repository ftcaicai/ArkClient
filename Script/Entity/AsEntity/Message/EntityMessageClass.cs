using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#region - type & base -
public enum eMessageType
{
	NONE = 0,
	
	MESSAGE_PARAMETER,
	MODEL_INITIALIZE, 
	ANIMATION_INDICATION, ANIMATION_END, FADETIME_INDICATION,
	ENTERWORLD_DEATH,
	LEVEL_UP,
	NPC_BEGIN_DIALOG,
	
	ZONE_WARP, ZONE_WARP_END, APPLICATION_PAUSE, HIDE_INDICATE,
	CHOICE,
	
	INPUT_MOVE, INPUT_AUTO_MOVE, INPUT_TARGETING, INPUT_ATTACK,
	INPUT_BEGIN_CHARGE, INPUT_SLOT_ACTIVE, INPUT_DRAG_STRAIGHT, INPUT_ARC, INPUT_CIRCLE,
	INPUT_SINGLE_TAB,
	INPUT_DOUBLE_TAB,
	INPUT_DASH, INPUT_SHIELD_BEGIN, INPUT_SHIELD_END, INPUT_RUB, DASH_INDICATION, DASHBACK_INDICATION, WARP_INDICATION, FORCEDMOVE_INDICATION, FORCEDMOVE_USER_INDICATION, JUMP_MSG, JUMP_STOP,
	
	MODEL_LOADED, MODEL_LOADED_DUMMY, MODEL_CHANGE,
	MOVE_INFO, MOVE_AUTO, MOVE_STOP_INDICATION, MOVE_END_INFORM, MOVE_SPEED_REFRESH, FORCED_MOVEMENT_SEARCH, FORCED_MOVEMENT_RESULT,
	COMBAT_BEGIN, COMBAT_END, RELEASE_TENSION,
	CLICKED, 
	MOVE_OTHER_USER_INDICATION,
	
	AUTO_SKILL_BEGIN, PLAYER_SKILL_TARGET_MOVE, PLAYER_SKILL_NONTARGET_MOVE,
	PLAYER_SKILL_READY, PLAYER_SKILL_HIT, PLAYER_SKILL_FINISH, PLAYER_SKILL_LINKACTION, PLAYER_SKILL_LEARN, PLAYER_USE_ACTIONITEM,
	//pet
	PET_APPEAR, PET_SKILL_READY, PET_SKILL_RESULT, PET_SKILL_CHANGE_RESULT, PET_SKILL_GET,
	PET_FEEDING, PET_SCRIPT, PET_SCRIPT_INDICATE, PET_LEVELUP, PET_EVOLUTION, PET_DATA_INDICATE, PET_NAME_CHANGE, PET_DELETE, PET_HATCH, PET_ITEM_VIEW, PET_POSITION_REFRESH, PET_HUNGRY_INDICATE, PET_EFFECT_INDICATE,
	
	CHAR_ATTACK_NPC1, CHAR_ATTACK_NPC2, CHAR_ATTACK_NPC3, OTHER_CHAR_ATTACK_READY, OTHER_CHAR_ATTACK_HIT, OTHER_CHAR_ATTACK_FINISH, OTHER_CHAR_ATTACK_LINKACTION,
	OTHER_CHAR_SKILL_EFFECT, OTHER_CHAR_SKILL_STANCE, CHAR_BUFF_ACCURE, CHAR_SKILL_ADD, CHAR_SKILL_SOULSTONE, CHAR_SKILL_PVP_AGGRO,
	MOVE_NPC_INDICATION, NPC_ATTACK_CHAR1, NPC_ATTACK_CHAR2, NPC_ATTACK_CHAR3, NPC_ATTACK_HIT, NPC_ATTACK_FINISH, NPC_ATTACK_LINKACTION, NPC_SKILL_EFFECT, NPC_STATUS,
	PLAYER_ATTACK_NPC_INDICATION,
	
	HIT_EXECUTION, HIT_POTENCY,
	
	EFFECT_INDICATION, EFFECT_GENERATION, EFFECT_RELEASE,
	
	OBJ_TRAP_MSG, OBJ_BREAK_MSG, OBJ_STEPPING_MSG,
	SKILL_CHARGE_COMPLETE,
	INPUT_CANCEL_CHARGE,
	CHEAT_DEATH,
	MOB_CASTING, MOB_COMBAT_FREE,
	
	REFLECTION_REFRESH,
	
	PLAYER_CLICK, NPC_CLICK, OBJECT_CLICK, COLLECT_CLICK, COLLECT_INFO, COLLECT_START, COLLECT_RESULT, 
	OTHER_USER_CLICK,
	
	//condition
	CONDITION_STUN, RECOVER_CONDITION_STUN,
	CONDITION_SLEEP, RECOVER_CONDITION_SLEEP,
	CONDITION_FREEZE, RECOVER_CONDITION_FREEZE,
	CONDITION_BURNING, RECOVER_CONDITION_BURNING,
	CONDITION_FEAR, RECOVER_CONDITION_FEAR,
	CONDITION_BINDING, RECOVER_CONDITION_BINDING,
	CONDITION_SIZECONTROL, RECOVER_CONDITION_SIZECONTROL,
	CONDITION_FORCEDMOVE, FORCEDMOVE_SYNC,
	CONDITION_BLANK, RECOVER_CONDITION_BLANK,
	
//	CONDITION_BALLOON, RECOVER_CONDITION_BALLOON,
	CONDITION_AIRBONE, RECOVER_CONDITION_AIRBONE,
	CONDITION_TRANSFORM, RECOVER_CONDITION_TRANSFORM,
	CONDITION_GRAY, RECOVER_CONDITION_GRAY,
	
	SHADOW_CONTROL, TRANSFORM,
	
	// resurrection
	CHAR_RESURRECTION,
	
	//getter
	ANIMATION_CLIP_RECEIVER,
	
	//buff
	CHAR_BUFF, CHAR_DEBUFF, CHAR_DEBUFF_RESIST, NPC_BUFF, NPC_DEBUFF, BUFF_REFRESH, BUFF_INCLUSION, DEATH_DEBUFF_CLEAR, DEATH_PENALTY_INDICATE,
	
	//attribute
	ATTACK_SPEED_REFRESH,
	
	//exception
	RECOVER_STATE, DELAY_BATTLE_RUN,
	
	//death by other reason
	DEATH_INDICATION,
	
	//shake
	SHAKE_INDICATION,
	
	//target
	TARGET_INDICATION,
	
	// private shop
	OPEN_PRIVATESHOP, CLOSE_PRIVATESHOP, PREV_STATE_PRIVATESHOP, OPEN_PRIVATESHOP_UI,
	
	// emotion
	EMOTION_INDICATION, EMOTICON_INDICATION, EMOTICON_SEAT_INDICATION, BALLOON,
	
	PRODUCT,
	
	AUTOCOMBAT_ON, AUTOCOMBAT_OFF, AUTOCOMBAT_SEARCH,
}

public abstract class AsIMessage
{
	protected eMessageType m_MessageType;
	public eMessageType MessageType	{ get { return m_MessageType; } }
}
#endregion

#region - system & entering world -
public class Msg_PropertyProcessor : AsIMessage
{
	public Msg_PropertyProcessor()
	{
		m_MessageType = eMessageType.MESSAGE_PARAMETER;
	}
}

public class Msg_ModelInitialize : AsIMessage
{
	public eCreationType creationType_;

	public Msg_ModelInitialize( eCreationType _type)
	{
		m_MessageType = eMessageType.MODEL_INITIALIZE;

		creationType_ = _type;
	}
}

public class Msg_EnterWorld_Death : AsIMessage
{
	public bool alreadyDead_;

	public Msg_EnterWorld_Death( bool _alreadyDead)
	{
		m_MessageType = eMessageType.ENTERWORLD_DEATH;

		alreadyDead_ = _alreadyDead;
	}
}

public class Msg_ZoneWarp : AsIMessage
{
	public Msg_ZoneWarp()
	{
		m_MessageType = eMessageType.ZONE_WARP;
	}
}

public class Msg_ZoneWarpEnd : AsIMessage
{
	public Msg_ZoneWarpEnd()
	{
		m_MessageType = eMessageType.ZONE_WARP_END;
	}
}

public class Msg_ApplicationPause : AsIMessage
{
	public bool pause_;

	public Msg_ApplicationPause( bool _pause)
	{
		m_MessageType = eMessageType.APPLICATION_PAUSE;

		pause_ = _pause;
	}
}

public class Msg_HideIndicate :AsIMessage
{
	public bool hide_;

	public Msg_HideIndicate( bool _hide)
	{
		m_MessageType = eMessageType.HIDE_INDICATE;

		hide_ = _hide;
	}
}
#endregion

#region - animation begin & end -
public class Msg_AnimationIndicate : AsIMessage
{
	public string animString_;
	public bool clampEnd_ = false;
	public float animSpeed_ = 1;
	public float fadeTime_ = 0.1f;
	public float targetTime_ = float.MaxValue;
	public float targetDuration_ = 0;
	Tbl_Action_Animation actionAnimation_ = null;

	public Msg_AnimationIndicate( string _name)
	{
		m_MessageType = eMessageType.ANIMATION_INDICATION;

		animString_ = _name;
	}

	public Msg_AnimationIndicate( string _name, bool _clampEnd)
	{
		m_MessageType = eMessageType.ANIMATION_INDICATION;

		animString_ = _name;
		clampEnd_ = _clampEnd;
	}

	public Msg_AnimationIndicate( string _name, float _fadeTime)
	{
		m_MessageType = eMessageType.ANIMATION_INDICATION;

		animString_ = _name;
		fadeTime_ = _fadeTime;
	}

	public Msg_AnimationIndicate( string _name, float _fadeTime, float _animSpeed)
	{
		m_MessageType = eMessageType.ANIMATION_INDICATION;

		animString_ = _name;
		fadeTime_ = _fadeTime;
		animSpeed_ = _animSpeed;
	}

	public Msg_AnimationIndicate( string _name, float _fadeTime, float _animSpeed, float _targetTime, float _targetDuration)
	{
		m_MessageType = eMessageType.ANIMATION_INDICATION;

		animString_ = _name;
		fadeTime_ = _fadeTime;
		animSpeed_ = _animSpeed;

		targetTime_ = _targetTime;
		targetDuration_ = _targetDuration;
	}

	public void SetActionAnimation( Tbl_Action_Animation _actionAnimation)
	{
		actionAnimation_ = _actionAnimation;
	}

	public WrapMode GetWrapMode()
	{
		if( actionAnimation_ != null)
			return actionAnimation_.wrapMode;
		else
			return WrapMode.Default;
	}

	public bool CheckAnimationLoop()
	{
		if( actionAnimation_ != null &&
			(actionAnimation_.LoopType == eLoopType.Loop ||
			actionAnimation_.LoopType == eLoopType.TimeLoop))
			return true;
		else
			return false;
	}
}

public class Msg_AnimationEnd : AsIMessage
{
	public string animName_;

	public Msg_AnimationEnd( string _name)
	{
		m_MessageType = eMessageType.ANIMATION_END;

		animName_ = _name;
	}
}

public class Msg_FadeTimeIndicate : AsIMessage
{
	public float fadeTime_ = 0.1f;

	public Msg_FadeTimeIndicate( float _fadeTime)
	{
		m_MessageType = eMessageType.FADETIME_INDICATION;

		fadeTime_ = _fadeTime;
	}
}
#endregion

public class Msg_Choice : AsIMessage
{
	public Msg_Choice()
	{
		m_MessageType = eMessageType.CHOICE;
	}
}

#region - user, other user, npc move -
public class Msg_ModelLoaded : AsIMessage
{
	public Msg_ModelLoaded()
	{
		m_MessageType = eMessageType.MODEL_LOADED;
	}
}

public class Msg_ModelLoaded_Dummy : AsIMessage
{
	public Msg_ModelLoaded_Dummy()
	{
		m_MessageType = eMessageType.MODEL_LOADED_DUMMY;
	}
}

public class Msg_ModelChange : AsIMessage
{
	public Msg_ModelChange()
	{
		m_MessageType = eMessageType.MODEL_CHANGE;
	}
}

public class Msg_MoveBase : AsIMessage
{
	public eMoveType moveType_;

	public virtual Vector3 GetTargetPosition( Vector3 _curPos)
	{
		return Vector3.zero;
	}
}

public class Msg_MoveInfo : Msg_MoveBase
{
	public Vector3 targetPosition_;

	public Msg_MoveInfo( Vector3 _targetPosition)
	{
		m_MessageType = eMessageType.MOVE_INFO;

		moveType_ = eMoveType.Normal;

		targetPosition_ = _targetPosition;
	}

	//OVERRIDE
	public override Vector3 GetTargetPosition( Vector3 _curPos)
	{
		return targetPosition_;
	}
}

public class Msg_AutoMove : Msg_MoveBase
{
	public Vector3 curPos_;
	public Vector3 targetPosition_;

	Vector3 direction_;

	public Msg_AutoMove( Vector3 _curPos, Vector3 _targetPosition)
	{
		m_MessageType = eMessageType.MOVE_AUTO;

		moveType_ = eMoveType.Auto;

		curPos_ = _curPos;
		targetPosition_ = _targetPosition;

		direction_ = _targetPosition - _curPos;
	}

	//OVERRIDE
	public override Vector3 GetTargetPosition( Vector3 _curPos)
	{
		return _curPos + direction_;
	}
}

public class Msg_DashIndication : AsIMessage
{
	public float time_;
	public float distance_;
	public Vector3 direction_;
	public Vector3 curPos_;

	public Msg_DashIndication( float _time, float _distance, Vector3 _direction, Vector3 _curPos)
	{
		m_MessageType = eMessageType.DASH_INDICATION;

		time_ = _time;
		distance_ = _distance;
		direction_ = _direction;
		curPos_ = _curPos;
	}
}

public class Msg_DashBackIndication : Msg_DashIndication
{
	public Msg_DashBackIndication( float _time, float _distance, Vector3 _direction, Vector3 _curPos) : base(_time, _distance, _direction, _curPos)
	{
		m_MessageType = eMessageType.DASHBACK_INDICATION;

//		time_ = _time;
//		distance_ = _distance;
//		direction_ = _direction;
//		curPos_ = _curPos;
	}
}

public class Msg_WarpIndication : AsIMessage
{
	public float distance_;
	public Vector3 direction_;
	public Vector3 curPos_;

	public Msg_WarpIndication( float _distance, Vector3 _direction, Vector3 _curPos)
	{
		m_MessageType = eMessageType.WARP_INDICATION;

		distance_ = _distance;
		direction_ = _direction;
		curPos_ = _curPos;
	}
}

public class Msg_ForcedMoveIndication : AsIMessage
{
	public float duration_;
	public Vector3 destination_;

	public Msg_ForcedMoveIndication( float _duration, Vector3 _destination)
	{
		m_MessageType = eMessageType.FORCEDMOVE_INDICATION;

		duration_ = _duration;
		destination_ = _destination;
	}
}

public class Msg_ForcedMoveUserIndication : AsIMessage
{
	public float duration_;
	public Vector3 destination_;

	public Msg_ForcedMoveUserIndication( float _duration, Vector3 _destination)
	{
		m_MessageType = eMessageType.FORCEDMOVE_USER_INDICATION;

		duration_ = _duration;
		destination_ = _destination;
	}
}

public class Msg_Jump : AsIMessage
{
	public Vector3 m_vec3TargetPos;
	public float m_fJumpSpeed;

	public Msg_Jump( Vector3 _vec3TargetPos, float fJumpSpeed)
	{
		m_MessageType = eMessageType.JUMP_MSG;
		m_vec3TargetPos = _vec3TargetPos;
		m_fJumpSpeed = fJumpSpeed;
	}
}

public class Msg_JumpStop : AsIMessage
{
	public Msg_JumpStop()
	{
		m_MessageType = eMessageType.JUMP_STOP;
	}
}

public class Msg_OtherUserMoveIndicate : AsIMessage
{
	public UInt16 sessionIdx_;
	public UInt32 charUniqKey_;
	public Vector3 curPosition_;
	public Vector3 destPosition_;
	public eMoveType moveType_;

	public Msg_OtherUserMoveIndicate( AS_SC_CHAR_MOVE _move)
	{
		m_MessageType = eMessageType.MOVE_OTHER_USER_INDICATION;

		sessionIdx_ = _move.nSessionIdx;
		charUniqKey_ = _move.nCharUniqKey;
	
		curPosition_ = _move.sCurPosition;
		destPosition_ = _move.sDestPosition;

		moveType_ = ( eMoveType)_move.nMoveType;
	}

	public Msg_OtherUserMoveIndicate( UInt16 _sessionIdx, UInt32 _charUniqKey, Vector3 _curPos, Vector3 _destPos, eMoveType _moveType)
	{
		m_MessageType = eMessageType.MOVE_OTHER_USER_INDICATION;

		sessionIdx_ = _sessionIdx;
		charUniqKey_ = _charUniqKey;
	
		curPosition_ = _curPos;
		destPosition_ = _destPos;

		moveType_ = _moveType;
	}
}

public class Msg_NpcMoveIndicate : AsIMessage
{
	public Int32 npcSessionId_;
	public float moveSpeed_;
	public Vector3 targetPosition_;

	public bool combat_;
	public bool forceMove_;
	public bool forceMyself_;

	public Msg_NpcMoveIndicate( AS_SC_NPC_MOVE _move)
	{
		m_MessageType = eMessageType.MOVE_NPC_INDICATION;

		npcSessionId_ = _move.nNpcIdx;
		moveSpeed_ = _move.fMoveSpeed * 0.01f;
		targetPosition_ = _move.sDestPosition;

		combat_ = _move.bCombat;
		forceMove_ = _move.bForceMove;
		forceMyself_ = _move.bForceMyself;
	}

	public Msg_NpcMoveIndicate( Int32 _session, Vector3 _pos)
	{
		m_MessageType = eMessageType.MOVE_NPC_INDICATION;

		npcSessionId_ = _session;
		targetPosition_ = _pos;
	}
}

public class Msg_MoveStopIndication : AsIMessage
{
	public Msg_MoveStopIndication()
	{
		m_MessageType = eMessageType.MOVE_STOP_INDICATION;
	}
}

public class Msg_MoveEndInform : AsIMessage
{
	public Msg_MoveEndInform()
	{
		m_MessageType = eMessageType.MOVE_END_INFORM;
	}
}

public class Msg_MoveSpeedRefresh : AsIMessage
{
	public float moveSpeed_;

	public Msg_MoveSpeedRefresh( float _moveSpeed)
	{
		m_MessageType = eMessageType.MOVE_SPEED_REFRESH;

		moveSpeed_ = _moveSpeed;
	}
}

public class Msg_ForcedMovementSearch : AsIMessage
{
	public enum eSender {User, Npc}
	public eSender sender_;

	public uint senderUserId_;
	public int senderNpcId_;

	public int index_;
	public body2_AS_CS_CHAR_ATTACK_NPC target_;

	public Msg_ForcedMovementSearch( uint _senderId, int _index, ref body2_AS_CS_CHAR_ATTACK_NPC _target)
	{
		m_MessageType = eMessageType.FORCED_MOVEMENT_SEARCH;

		sender_ = eSender.User;

		senderUserId_ = _senderId;
		index_ = _index;
		target_ = _target;
	}

	public Msg_ForcedMovementSearch( int _senderId, int _index, ref body2_AS_CS_CHAR_ATTACK_NPC _target)
	{
		m_MessageType = eMessageType.FORCED_MOVEMENT_SEARCH;

		sender_ = eSender.Npc;

		senderNpcId_ = _senderId;
		index_ = _index;
		target_ = _target;
	}
}

public class Msg_ForcedMovementResult : AsIMessage
{
	public int index_;

	public Msg_ForcedMovementResult( int _index)
	{
		m_MessageType = eMessageType.FORCED_MOVEMENT_RESULT;

		index_ = _index;
	}
}
#endregion

public class Msg_NpcCombatFree : AsIMessage
{
	public Msg_NpcCombatFree()
	{
		m_MessageType = eMessageType.MOB_COMBAT_FREE;
	}
}

public class Msg_ReflectionRefresh : AsIMessage
{
	public AsBaseEntity entity_;

	public Msg_ReflectionRefresh( AsBaseEntity _entity)
	{
		m_MessageType = eMessageType.REFLECTION_REFRESH;

		entity_ = _entity;
	}
}

#region - npc attack & skill -
public class Msg_NpcAttackChar1 : AsIMessage
{
	public Int32 npcId_;
	public UInt16[] sessionId_;
	public UInt32[] charUniqKey_;
	public float hpCur_;
	public AsNpcEntity attacker_;
	public Tbl_MonsterSkill_Record skill_;
	public Tbl_MonsterSkillLevel_Record skillLv_;
	public Tbl_Action_Record action_;
	public bool casting_;
	public Int32 castingMilliSec_;
	public bool ready_;
	public float animSpeed_;
	public Vector3 targetPos_;
	public AsBaseEntity target_;
	public List<Msg_NpcAttackChar2> bodyChar_ = new List<Msg_NpcAttackChar2>();
	public List<Msg_NpcAttackChar3> bodyNpc_ = new List<Msg_NpcAttackChar3>();

	public Msg_NpcAttackChar1( AS_SC_NPC_ATTACK_CHAR_1 _info)
	{
		m_MessageType = eMessageType.NPC_ATTACK_CHAR1;

		npcId_ = _info.nNpcIdx;

		sessionId_ = _info.nSessionIdx;
		charUniqKey_ = _info.nCharUniqKey;

		hpCur_ = _info.nHpCur;

		attacker_ = AsEntityManager.Instance.GetNpcEntityBySessionId( npcId_);
		skillLv_ = AsTableManager.Instance.GetTbl_MonsterSkillLevel_Record( _info.nMonsterSkillLevelTableIdx);
		skill_ = AsTableManager.Instance.GetTbl_MonsterSkill_Record( skillLv_.Skill_GroupIndex);
		action_ = AsTableManager.Instance.GetTbl_MonsterAction_Record( skillLv_.SkillAction_Index);

		casting_ = _info.bCasting;
		castingMilliSec_ = _info.nCastingMilliSec;
		ready_ = _info.bReady;

		if( true == casting_)
			Debug.Log( "true == casting_");

		foreach( AS_SC_NPC_ATTACK_CHAR_2 attack2 in _info.bodyChar)
		{
			bodyChar_.Add( new Msg_NpcAttackChar2( this, attack2));
		}

		foreach( AS_SC_NPC_ATTACK_CHAR_3 attack3 in _info.bodyNpc)
		{
			bodyNpc_.Add( new Msg_NpcAttackChar3( this, attack3));
		}
	}

	public void SetTarget( AsBaseEntity _target)
	{
		target_ = _target;
		if( target_ != null)
			targetPos_ = target_.transform.position;
	}

	public void SetAnimSpeed( float _animSpeed)
	{
		animSpeed_ = _animSpeed;

		if( animSpeed_ == 0f)
		{
			Debug.LogError( "Msg_NpcAttackChar1::constructor: animSpeed_ is set as 0. instead set animSpeed_ 1f");
			animSpeed_ = 1f;
		}
	}
}

public class Msg_NpcAttackChar2 : AsIMessage
{
	public Msg_NpcAttackChar1 parent_;
	public UInt16 sessionId_;
	public UInt32 charUniqKey_;
	public Single hpCur_;
	public Single mpCur_;
	public eDAMAGETYPE eDamageType_; // ilmeda, 20120731
	public Single damage_;
	public Single hpHeal_;
	public Single mpHeal_;
	public float reflection_;
	public float drain_;
	public bool knockBack_ = false;

	//ADDITINAL INFO
//	public AsNpcEntity attacker_;
//	public Tbl_MonsterSkillLevel_Record skillLv_;
//	public Tbl_Action_Record action_;

	public Msg_NpcAttackChar2( Msg_NpcAttackChar1 _parent, AS_SC_NPC_ATTACK_CHAR_2 _info)
	{
		m_MessageType = eMessageType.NPC_ATTACK_CHAR2;

		parent_ = _parent;

		sessionId_ = _info.nSessionIdx;
		charUniqKey_ = _info.nCharUniqKey;

		hpCur_ = _info.fHpCur;
		mpCur_ = _info.fMpCur;

		eDamageType_ = ( eDAMAGETYPE)_info.eDamageType; // ilmeda, 20120731
		damage_ = _info.nDamage;

		hpHeal_ = _info.fHpHeal;
		mpHeal_ = _info.fMpHeal;

		reflection_ = _info.fReflection;
		drain_ = _info.fDrain;

		knockBack_ = _info.bKnockBack;
	}
}

public class Msg_NpcAttackChar3 : AsIMessage
{
	public Msg_NpcAttackChar1 parent_;
	public Int32 npcIdx_;
	public Single hpCur_;
	public Single hpHeal_;
	public Single mpHeal_;

	public Msg_NpcAttackChar3( Msg_NpcAttackChar1 _parent, AS_SC_NPC_ATTACK_CHAR_3 _info)
	{
		m_MessageType = eMessageType.NPC_ATTACK_CHAR3;

		parent_ = _parent;

		npcIdx_ = _info.nNpcIdx;

		hpCur_ = _info.fHpCur;
		hpHeal_ = _info.fHpHeal;
		mpHeal_ = _info.fMpHeal;
	}
}

public class Msg_NpcAttackChar_Hit : AsIMessage
{
	public Msg_NpcAttackChar1 attack_;
	public Tbl_Action_Record action_;
	public Tbl_MonsterSkillLevel_Record skill_;

	public Msg_NpcAttackChar_Hit( Msg_NpcAttackChar1 _attack, Tbl_Action_Record _action, Tbl_MonsterSkillLevel_Record _skill)
	{
		m_MessageType = eMessageType.NPC_ATTACK_HIT;

		attack_ = _attack;
		action_ = _action;
		skill_ = _skill;
	}
}

public class Msg_NpcAttackChar_Finish : AsIMessage
{
	public Msg_NpcAttackChar1 attack_;
	public Tbl_Action_Record action_;
	public Tbl_MonsterSkillLevel_Record skill_;

	public Msg_NpcAttackChar_Finish( Msg_NpcAttackChar1 _attack, Tbl_Action_Record _action, Tbl_MonsterSkillLevel_Record _skill)//, Msg_NpcMoveIndicate _move)
	{
		m_MessageType = eMessageType.NPC_ATTACK_FINISH;

		attack_ = _attack;
		action_ = _action;
		skill_ = _skill;
	}
}

public class Msg_NpcAttackChar_Link : AsIMessage
{
	public Msg_NpcAttackChar1 attack_;

	public Msg_NpcAttackChar_Link( Msg_NpcAttackChar1 _attack)
	{
		m_MessageType = eMessageType.NPC_ATTACK_LINKACTION;

		attack_ = _attack;
	}
}

public class Msg_NpcSkillEffect : AsIMessage
{
	public int npcIdx_;
	public int skillTableIdx_;
	public int skillLevel_;
	public int potencyIdx_;
	public int chargeStep_;

	public Msg_NpcSkillEffect( body_SC_NPC_SKILL_EFFECT _body)
	{
		m_MessageType = eMessageType.NPC_SKILL_EFFECT;

		npcIdx_ = _body.nNpcIdx;

		skillTableIdx_ = _body.nSkillTableIdx;
		skillLevel_ = _body.nSkillLevel;
		potencyIdx_ = _body.nPotencyIdx;
		chargeStep_ = _body.nChargeStep;
	}
}

public class Msg_NpcStatus : AsIMessage
{
	public eNPCSTATUS status_;
	public UInt16 sessionIdx_;

	public Msg_NpcStatus( body_SC_NPC_STATUS _status)
	{
		m_MessageType = eMessageType.NPC_STATUS;

		status_ = _status.eStatus;
		sessionIdx_ = _status.nSessionIdx;
	}
}
#endregion

#region - player skill -
public class Msg_Player_Skill_Target_Move : AsIMessage
{
	Msg_Player_Skill_Ready m_Ready; public Msg_Player_Skill_Ready Ready{get{return m_Ready;}}
	AsPlayerState_BattleRun.eRunType m_RunType = AsPlayerState_BattleRun.eRunType.Target; public AsPlayerState_BattleRun.eRunType RunType{get{return m_RunType;}}

	public Msg_Player_Skill_Target_Move( Msg_Player_Skill_Ready _ready)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_TARGET_MOVE;
		m_Ready = _ready;
	}

	public Msg_Player_Skill_Target_Move( Msg_Player_Skill_Ready _ready, AsPlayerState_BattleRun.eRunType _runType)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_TARGET_MOVE;
		m_Ready = _ready;
		m_RunType = _runType;
	}
}

public class Msg_Player_Skill_Ready : AsIMessage
{
	//table
	Tbl_Skill_Record skillRecord_;public Tbl_Skill_Record skillRecord{get{return skillRecord_;}}
	int skillLv_;public int skillLv{get{return skillLv_;}}
	Tbl_SkillLevel_Record skillLvRecord_;public Tbl_SkillLevel_Record skillLvRecord{get{return skillLvRecord_;}}
	Tbl_Action_Record actionRecord_;public Tbl_Action_Record actionRecord{get{return actionRecord_;}}

	//entity
	AsBaseEntity pickedEntity_;public AsBaseEntity pickedEntity{get{return pickedEntity_;}}

	//geometry
	Vector3 picked_;public Vector3 picked{get{return picked_;}}

	Vector3 head_;public Vector3 head{get{return head_;}}
	Vector3 center_;public Vector3 center{get{return center_;}}
	Vector3 tail_;public Vector3 tail{get{return tail_;}}
	Vector3 direction_;public Vector3 direction{get{return direction_;}}
	eClockWise cw_;public eClockWise cw{get{return cw_;}}
	float animSpeed_ = 1;public float animSpeed{get{return animSpeed_;}}

	bool constructSucceed_ = false;public bool constructSucceed{get{return constructSucceed_;}}

	int itemSlot_ = 0; public int itemSlot{get{return itemSlot_;}}
	bool charging_ = false; public bool charging{get{return charging_;}}
	AsBaseEntity target_ = null; public AsBaseEntity Target{get{return target_;}}

	//constructor
	public Msg_Player_Skill_Ready( eCLASS _class, eSKILL_TYPE _type, eCommand_Type _command, eGENDER _gender,
		AsBaseEntity _pickedEntity, Vector3 _picked,
		Vector3 _head, Vector3 _center, Vector3 _tail,
		Vector3 _dir, eClockWise _cw)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_READY;

		pickedEntity_ = _pickedEntity;
		picked_ = _picked;

		head_ = _head;
		center_ = _center;
		tail_ = _tail;

		direction_ = _dir;

		cw_ = _cw;

		switch( _type)
		{
		case eSKILL_TYPE.Base:
			skillRecord_ = AsTableManager.Instance.GetRandomBaseSkill( _class);
			break;
		case eSKILL_TYPE.Command:
			skillRecord_ = SkillBook.Instance.GetLearnedCommandSkill( _command);
			break;
		default:
			Debug.LogError( "Msg_Player_Skill_Ready::Msg_Player_Skill_Ready(): [class]" + _class + ",[type]" +
				_type + ",[command]" + _command +
				",[head_]" + _head + ",[center_]" + _center + ",[tail_]" + _tail + ",[direction]" + direction_);
			break;
		}

		if( skillRecord_ != null)
		{
			skillLv_ = SkillBook.Instance.GetSkillLevel( skillRecord_);

			skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLv_, skillRecord_.Index);

			int actionIdx = -1;
			if( _gender == eGENDER.eGENDER_MALE)
			{
				if( cw_ == eClockWise.CW)
					actionIdx = skillLvRecord_.SkillAction_Index;
				else
					actionIdx = skillLvRecord_.SkillActionCCW_Index;
			}
			else if( _gender == eGENDER.eGENDER_FEMALE)
			{
				if( cw_ == eClockWise.CW)
					actionIdx = skillLvRecord_.SkillAction_Index_Female;
				else
					actionIdx = skillLvRecord_.SkillActionCCW_Index_Female;
			}

			actionRecord_ = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);

			if( skillLvRecord_ != null || actionRecord_ != null)
			{
				constructSucceed_ = true;
				return;
			}
		}
	}
	
	public Msg_Player_Skill_Ready( Tbl_Skill_Record _record, eGENDER _gender, int _step/* = 1 */,
		AsBaseEntity _pickedEntity, Vector3 _picked,
		Vector3 _head, Vector3 _center, Vector3 _tail, Vector3 _dir, eClockWise _cw)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_READY;

		pickedEntity_ = _pickedEntity;
		picked_ = _picked;

		head_ = _head;
		center_ = _center;
		tail_ = _tail;

		direction_ = _dir;

		skillRecord_ = _record;
		if( skillRecord_ != null)
		{
			skillLv_ = SkillBook.Instance.GetSkillLevel( skillRecord_);

			skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLv_, skillRecord_.Index, _step);

			int actionIdx = -1;
			if( _gender == eGENDER.eGENDER_MALE)
			{
				if( cw_ == eClockWise.CW)
					actionIdx = skillLvRecord_.SkillAction_Index;
				else
					actionIdx = skillLvRecord_.SkillActionCCW_Index;
			}
			else if( _gender == eGENDER.eGENDER_FEMALE)
			{
				if( cw_ == eClockWise.CW)
					actionIdx = skillLvRecord_.SkillAction_Index_Female;
				else
					actionIdx = skillLvRecord_.SkillActionCCW_Index_Female;
			}

			actionRecord_ = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);

			if( skillLvRecord_ != null || actionRecord_ != null)
			{
				constructSucceed_ = true;
				return;
			}
		}
	}

	public Msg_Player_Skill_Ready( Msg_Player_Skill_Ready _ready, Tbl_Action_Record _action)
	{
		skillRecord_ = _ready.skillRecord;
		skillLv_ = _ready.skillLv_;
		skillLvRecord_ = _ready.skillLvRecord;
		actionRecord_ = _action;//IMPORTANT

		pickedEntity_ = _ready.pickedEntity;

		head_ = _ready.head;
		center_ = _ready.center;
		tail_ = _ready.tail;
		direction_ = _ready.direction;
		cw_ = _ready.cw;

		constructSucceed_ = true;
	}

	public Msg_Player_Skill_Ready( Tbl_Skill_Record _record, eGENDER _gender)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_READY;

		head_ = Vector3.zero;
		center_ = Vector3.zero;
		tail_ = Vector3.zero;

		direction_ = Vector3.zero;

		skillRecord_ = _record;

		skillLv_ = SkillBook.Instance.GetSkillLevel( skillRecord_);
		skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLv_, _record.Index);

		int actionIdx = -1;
		if( _gender == eGENDER.eGENDER_MALE)
			actionIdx = skillLvRecord_.SkillAction_Index;
		else if( _gender == eGENDER.eGENDER_FEMALE)
			actionIdx = skillLvRecord_.SkillAction_Index_Female;

		actionRecord_ = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);

		constructSucceed_ = true;
	}
	
	public Msg_Player_Skill_Ready( int _skillIdx, int _skillLv, eGENDER _gender)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_READY;

		head_ = Vector3.zero;
		center_ = Vector3.zero;
		tail_ = Vector3.zero;

		direction_ = Vector3.zero;

		skillRecord_ = AsTableManager.Instance.GetTbl_Skill_Record( _skillIdx);
		skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( _skillLv, _skillIdx);

		int actionIdx = -1;
		if( _gender == eGENDER.eGENDER_MALE)
			actionIdx = skillLvRecord_.SkillAction_Index;
		else if( _gender == eGENDER.eGENDER_FEMALE)
			actionIdx = skillLvRecord_.SkillAction_Index_Female;

		actionRecord_ = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);

		constructSucceed_ = true;
	}

	public Msg_Player_Skill_Ready( ItemData _data, int _slot, eGENDER _gender)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_READY;

		head_ = Vector3.zero;
		center_ = Vector3.zero;
		tail_ = Vector3.zero;

		direction_ = Vector3.zero;

		skillRecord_ = AsTableManager.Instance.GetTbl_Skill_Record( _data.itemSkill);

		skillLv_ = _data.itemSkillLevel;
		skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLv_, _data.itemSkill);

		int actionIdx = -1;
		if( _gender == eGENDER.eGENDER_MALE)
			actionIdx = skillLvRecord_.SkillAction_Index;
		else if( _gender == eGENDER.eGENDER_FEMALE)
			actionIdx = skillLvRecord_.SkillAction_Index_Female;

		actionRecord_ = AsTableManager.Instance.GetTbl_Action_Record( actionIdx);

		if( actionRecord_.HitAnimation != null &&
			actionRecord_.HitAnimation.FileName == "NonAnimation")
			constructSucceed_ = false;
		else
			constructSucceed_ = true;

		itemSlot_ = _slot;
	}

	//set
	public void SetSkillLevel( int _lv)
	{
		skillLv_ = _lv;
	}

	public void SetSkillLevel( Tbl_Skill_Record _record)
	{
		skillLv_ = SkillBook.Instance.GetSkillLevel( _record);
	}

	public void SetAnimSpeed( float _animSpeed)
	{
		animSpeed_ = _animSpeed;

		if( animSpeed_ == 0f)
		{
			Debug.LogError( "Msg_Player_Skill_Ready::constructor: animSpeed_ is set as 0. instead set animSpeed_ 1f");
			animSpeed_ = 1f;
		}
	}

	public void SetCharging()
	{
		charging_ = true;
	}

	public void SetCurrentTarget( AsBaseEntity _target)
	{
		target_ = _target;
	}

	//get
	public bool CheckValidTargeting( AsBaseEntity _target)
	{
		if( ( skillRecord_.Skill_Type == eSKILL_TYPE.SlotBase ||
			skillRecord_.Skill_Type == eSKILL_TYPE.Target) &&
			( _target == null ||
			( skillRecord_.CheckPotencyTypeIncludeResurrection() == false &&
			( _target.ContainProperty( eComponentProperty.LIVING) == true &&
			_target.GetProperty<bool>( eComponentProperty.LIVING) == false)
			)))
		{
			Map map = TerrainMgr.Instance.GetCurrentMap();
			eMAP_TYPE mapType = map.MapData.getMapType;

			if( eMAP_TYPE.Town == mapType)
				AsMyProperty.Instance.AlertSkillInTown();
			else
			{
				Debug.Log( "Msg_Player_Skill_Ready::CheckValidTargeting: AsMyProperty.Instance.AlertInvalidTarget()");
				Debug.Log( "_target = " + _target);
				Debug.Log( "skillRecord_.CheckPotencyTypeIncludeResurrection() = " + skillRecord_.CheckPotencyTypeIncludeResurrection());
				Debug.Log( "_target.ContainProperty( eComponentProperty.LIVING) = " + _target.ContainProperty( eComponentProperty.LIVING));
				Debug.Log( "_target.GetProperty<bool>( eComponentProperty.LIVING) = " + _target.GetProperty<bool>( eComponentProperty.LIVING));
				AsMyProperty.Instance.AlertInvalidTarget();
			}

			return false;
		}

		return true;
	}
//	public bool CheckNonAnimation()
//	{
//		if( actionRecord_ != null &&
//			actionRecord_.HitAnimation != null &&
//			actionRecord_.HitAnimation.FileName == "NonAnimation")
//			constructSucceed_ = true;
//		else
//			constructSucceed_ = false;
//	}
}

public class Msg_Player_Skill_Hit : AsIMessage
{
	public Msg_Player_Skill_Ready ready_;
//	public Tbl_Action_Record action_;
//	public Tbl_SkillLevel_Record skillLv_;

	public int chargeStep_;
	public bool casting_;
//	public int mainTargetNpc_;
//	public uint mainTargetUser_;

	public bool targetReleased_;

	public Msg_Player_Skill_Hit( Msg_Player_Skill_Ready _ready,
		int _chargeStep, bool _casting,
//		int _mainTargetNpc, uint _mainTargetUser,
		bool _targetReleased)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_HIT;

		ready_ = _ready;

		if( _chargeStep == int.MaxValue)
			_chargeStep = 0;

		chargeStep_ = _chargeStep;
		casting_ = _casting;

//		mainTargetNpc_ = _mainTargetNpc;
//		mainTargetUser_ = _mainTargetUser;

		targetReleased_ = _targetReleased;
	}

//	public AsBaseEntity GetMainTarget()
//	{
//		if( mainTargetNpc_ != 0)
//			return AsEntityManager.Instance.GetNpcEntityBySessionId( mainTargetNpc_);
//		else if( mainTargetUser_ != 0)
//			return AsEntityManager.Instance.GetUserEntityByUniqueId( mainTargetUser_);
//		else
//		{
//			Debug.Log( "Msg_Player_Skill_Hit::GetMainTarget: main target is not set.");
//			return null;
//		}
//	}
}

public class Msg_Player_Skill_Finish : AsIMessage
{
	public Msg_Player_Skill_Ready ready_;
//	public Tbl_Action_Record action_;
//	public Tbl_SkillLevel_Record skillLv_;

	public bool targetReleased_;

	public Msg_Player_Skill_Finish( Msg_Player_Skill_Ready _ready,
		bool _targetReleased)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_FINISH;

		ready_ = _ready;

		targetReleased_ = _targetReleased;
	}
}

public class Msg_Player_Skill_Linkaction : AsIMessage
{
	public Msg_Player_Skill_Ready ready_;

	public Msg_Player_Skill_Linkaction( Msg_Player_Skill_Ready _ready)
	{
		m_MessageType = eMessageType.PLAYER_SKILL_LINKACTION;

		ready_ = _ready;
	}
}

public class Msg_Player_Skill_Learn : AsIMessage
{
	public bool bLearnSkill = false;
	public Msg_Player_Skill_Learn( bool _learnSkill)
	{
		bLearnSkill = _learnSkill;
		m_MessageType = eMessageType.PLAYER_SKILL_LEARN;
	}
}

public class Msg_Player_Use_ActionItem : AsIMessage
{
//	public delegate bool CheckingFunc( AsPlayerFsm _fsm);

	RealItem m_RealItem; public RealItem realItem{get{return m_RealItem;}}
//	public CheckingFunc checkingFunc_;

	public Msg_Player_Use_ActionItem( RealItem _item)
	{
		m_MessageType = eMessageType.PLAYER_USE_ACTIONITEM;

		m_RealItem = _item;
	}

//	public Msg_Player_Use_ActionItem( RealItem _item, CheckingFunc _func)
//	{
//		m_MessageType = eMessageType.PLAYER_USE_ACTIONITEM;
//
//		m_RealItem = _item;
//		checkingFunc_ = _func;
//	}
}
#endregion

#region - pet -
//public class Msg_Pet_Appear : AsIMessage
//{
//	public PetAppearData data_;
//	
//	public Msg_Pet_Appear( PetAppearData _data)
//	{
//		m_MessageType = eMessageType.PET_APPEAR;
//		
//		playerPet_ = false;
//		
//		data_ = _data;
//	}
//}

public class Msg_Pet_Skill_Ready : AsIMessage
{
	public bool playerPet_;
	
	public Tbl_Skill_Record skillRecord_;
	public Tbl_SkillLevel_Record skillLvRecord_;
	public Tbl_Action_Record actionRecord_;
	
	public Msg_Pet_Skill_Ready( body_SC_PET_SKILL_USE _use)
	{
		m_MessageType = eMessageType.PET_SKILL_READY;
		
		playerPet_ = false;
		
		skillRecord_ = AsTableManager.Instance.GetTbl_Skill_Record( _use.nSkillTableIdx);
		skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( _use.nSkillLevel, _use.nSkillTableIdx);
		actionRecord_ = AsTableManager.Instance.GetPetActionRecord( skillLvRecord_.SkillAction_Index);
	}
	
	public Msg_Pet_Skill_Ready( sPETSKILL _petSkill)
	{
		m_MessageType = eMessageType.PET_SKILL_READY;
		
		playerPet_ = true;
		
		skillRecord_ = AsTableManager.Instance.GetTbl_Skill_Record( _petSkill.nSkillTableIdx);
		skillLvRecord_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( _petSkill.nLevel, _petSkill.nSkillTableIdx);
		actionRecord_ = AsTableManager.Instance.GetPetActionRecord( skillLvRecord_.SkillAction_Index);
	}
}

//public class Msg_Pet_Skill_Result : AsIMessage
//{
//	public body_SC_PET_SKILL_USE_RESULT result_;
//	
//	public Msg_Pet_Skill_Result( body_SC_PET_SKILL_USE_RESULT _result)
//	{
//		m_MessageType = eMessageType.PET_SKILL_RESULT;
//		
//		result_ = _result;
//	}
//}

//public class Msg_Pet_Skill_Change_Result : AsIMessage
//{
//	public body_SC_PET_SKILL_CHANGE_RESULT result_;
//	
//	public Msg_Pet_Skill_Change_Result( body_SC_PET_SKILL_CHANGE_RESULT _result)
//	{
//		m_MessageType = eMessageType.PET_SKILL_CHANGE_RESULT;
//		
//		result_ = _result;
//	}
//}

public class Msg_Pet_Skill_Get : AsIMessage
{
	public ePET_SKILL_TYPE type_;
	public sPETSKILL skill_;
	
	public Msg_Pet_Skill_Get( ePET_SKILL_TYPE _type, sPETSKILL _skill)
	{
		m_MessageType = eMessageType.PET_SKILL_GET;
		
		type_ = _type;
		skill_ = _skill;
	}
}

public class Msg_Pet_Feeding : AsIMessage
{
	public float hungry_;
	
	public Msg_Pet_Feeding(float _hungry)
	{
		m_MessageType = eMessageType.PET_FEEDING;
		
		hungry_ = _hungry;
	}

	public Msg_Pet_Feeding()
	{
		m_MessageType = eMessageType.PET_FEEDING;
	}
}

public class Msg_Pet_Script : AsIMessage
{
	public string word_;
	
	public Msg_Pet_Script( string _word)
	{
		m_MessageType = eMessageType.PET_SCRIPT;
		
		word_ = _word;
	}
}

public class Msg_Pet_Script_Indicate : AsIMessage
{
	public Msg_PetDataIndicate data_;

	public Msg_Pet_Script_Indicate(Msg_PetDataIndicate _data)
	{
		m_MessageType = eMessageType.PET_SCRIPT_INDICATE;

		data_ = _data;
	}
}

public class Msg_Pet_LevelUp : AsIMessage
{
	public body_SC_PET_LEVEL_UP levelUp_;
	
	public bool skillUp_ = false;
	
	public Msg_Pet_LevelUp( body_SC_PET_LEVEL_UP _levelUp)
	{
		m_MessageType = eMessageType.PET_LEVELUP;
		
		levelUp_ = _levelUp;
	}
}

public class Msg_Pet_Evolution : AsIMessage
{
	public int curLv_;
	public int nextLv_;
	
//	public PetInfo info_;
	
	public Msg_Pet_Evolution( int _curLv, int _nextLv)
	{
		m_MessageType = eMessageType.PET_EVOLUTION;
		
		curLv_ = _curLv;
		nextLv_ = _nextLv;
	}
	
//	public void SetPetInfo(PetInfo _info)
//	{
//		info_ = _info;
//	}
}

public class Msg_PetDataIndicate : AsIMessage
{
	public PetAppearData data_;
//	public AsNpcEntity pet_;

	public bool display_ = false;
	
	public Msg_PetDataIndicate( PetAppearData _data)//, AsNpcEntity _pet)
	{
		m_MessageType = eMessageType.PET_DATA_INDICATE;
		
		data_ = _data;
//		pet_ = _pet;
	}

	public Msg_PetDataIndicate(PetAppearData _data, bool _display)
	{
		m_MessageType = eMessageType.PET_DATA_INDICATE;

		data_ = _data;
		display_ = _display;
	}
}

public class Msg_PetNameChange : AsIMessage
{
	public byte[] name_;
	
	public Msg_PetNameChange( body_SC_PET_NAME_NOTIFY _notify)
	{
		m_MessageType = eMessageType.PET_NAME_CHANGE;
		
		name_ = _notify.szPetName;
	}

//	public Msg_PetNameChange( body_SC_PET_LOAD _load)
//	{
//		m_MessageType = eMessageType.PET_NAME_CHANGE;
//		
//		name_ = _load.szPetName;
//	}
}

public class Msg_PetDelete : AsIMessage
{
//	public body_SC_PET_NOTIFY notify_;
	
	public Msg_PetDelete()
	{
		m_MessageType = eMessageType.PET_DELETE;
	}
	
//	public Msg_PetDelete( body_SC_PET_NOTIFY _notify)
//	{
//		m_MessageType = eMessageType.PET_DELETE;
//		
//		notify_ = _notify;
//	}
}

public class Msg_PetHatchIndicate : AsIMessage
{
	public Msg_PetHatchIndicate()
	{
		m_MessageType = eMessageType.PET_HATCH;
	}
}

public class Msg_PetItemView : AsIMessage
{
	public sITEMVIEW view_;
	
	public Msg_PetItemView( sITEMVIEW _view)
	{
		m_MessageType = eMessageType.PET_ITEM_VIEW;
		
		view_ = _view;
	}
}

public class Msg_PetPositionRefresh : AsIMessage
{
	public Msg_PetPositionRefresh()
	{
		m_MessageType = eMessageType.PET_POSITION_REFRESH;
	}
}

public class Msg_PetHungryIndicate : AsIMessage
{
	public Msg_PetHungryIndicate()
	{
		m_MessageType = eMessageType.PET_HUNGRY_INDICATE;
	}
}

public class Msg_PetEffectIndicate : AsIMessage
{
	public string path_;

	public Msg_PetEffectIndicate(string _path)
	{
		m_MessageType = eMessageType.PET_EFFECT_INDICATE;

		path_ = _path;
	}
}
#endregion

#region - character attack from server -
public class Msg_OtherCharAttackNpc1 : AsIMessage
{
	public UInt16 sessionId_;
	public UInt32 charUniqKey_;

	public Int32[] npcIdx_;
	public UInt32[] mainCharUniqKey_;
	public Vector3 targeting_;
	public Vector3 direction_;

	public Int32 skillTableIdx_;
	public Int32 skillLevel_;
	public Int32 actionTableIdx_;
	public Int32 chargeStep_;
	public bool casting_;
	public bool ready_;

	public float hpCur_;
	public float hpHeal_;

	//ADDITONAL
	public AsUserEntity attacker_;
	public Tbl_Skill_Record skill_;
	public Tbl_SkillLevel_Record skillLv_;
	public Tbl_Action_Record action_;

	public Int32 npcCnt_;
	public Int32 charCnt_;

	public List<Msg_OtherCharAttackNpc2> npcBody_ = new List<Msg_OtherCharAttackNpc2>();
	public List<Msg_OtherCharAttackNpc3> charBody_ = new List<Msg_OtherCharAttackNpc3>();

//	public bool instantSkill_;

//	public Msg_OtherCharAttackNpc1( AS_SC_CHAR_ATTACK_NPC_1 _info)
//	{
//		Msg_OtherCharAttackNpc1( _info, false);
//	}

//	public Msg_OtherCharAttackNpc1( AS_SC_CHAR_ATTACK_NPC_1 _info, bool _instantSkill)
	public Msg_OtherCharAttackNpc1( AS_SC_CHAR_ATTACK_NPC_1 _info)
	{
		m_MessageType = eMessageType.CHAR_ATTACK_NPC1;

		sessionId_ = _info.nSessionIdx;
		charUniqKey_ = _info.nCharUniqKey;

		npcIdx_ = _info.nNpcIdx;
		mainCharUniqKey_ = _info.nMainCharUniqKey;
		targeting_ = _info.sTargeting;
		direction_ = _info.sDirection;

		skillTableIdx_ = _info.nSkillTableIdx;
		skillLevel_ = _info.nSkillLevel;
		actionTableIdx_ = _info.nActionTableIdx;
		chargeStep_ = _info.nChargeStep;
		casting_ = _info.bCasting;
		ready_ = _info.bReady;

		hpCur_ = _info.nHpCur;
		hpHeal_ = _info.nHpHeal;

		if( _info.nChargeStep == 0)
			skillLv_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( _info.nSkillLevel, _info.nSkillTableIdx);
		else
			skillLv_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( _info.nSkillLevel, _info.nSkillTableIdx, _info.nChargeStep);

		attacker_ = AsEntityManager.Instance.GetUserEntityByUniqueId( charUniqKey_);
		skill_ = AsTableManager.Instance.GetTbl_Skill_Record( skillLv_.Skill_GroupIndex);
		action_ = AsTableManager.Instance.GetTbl_Action_Record( skillLv_.SkillAction_Index);

		npcCnt_ = _info.nNpcCnt;
		charCnt_ = _info.nCharCnt;

		bool IsMine = AsUserInfo.Instance.GetCurrentUserEntity().UniqueId == charUniqKey_ ? true : false;

		foreach( AS_SC_CHAR_ATTACK_NPC_2 attack2 in _info.bodyNpc)
		{
			npcBody_.Add( new Msg_OtherCharAttackNpc2( this, attack2, charUniqKey_));
		}

		foreach( AS_SC_CHAR_ATTACK_NPC_3 attack3 in _info.bodyChar)
		{
			charBody_.Add( new Msg_OtherCharAttackNpc3( this, attack3));
		}

		if( ready_ == false)
		{
			if( IsMine == true)
				QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_USE_SKILL, new AchUseSkill( _info.nSkillTableIdx, 1));

			int count = 0;
			foreach ( AS_SC_CHAR_ATTACK_NPC_2 attack2 in _info.bodyNpc)
			{
				AsNpcEntity monEntity = AsEntityManager.Instance.GetNpcEntityBySessionId( attack2.nNpcIdx);

				if( monEntity != null)
				{
					if( IsMine == true)
					{
						int monId = monEntity.GetProperty<int>( eComponentProperty.MONSTER_ID);
						int monKindID = monEntity.GetProperty<int>( eComponentProperty.MONSTER_KIND_ID);
						QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_USE_SKILL_TO_MONSTER, new AchUseSkillToMonster( _info.nSkillTableIdx, monId, 1));
						QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_USE_SKILL_TO_MONSTER_KIND, new AchUseSkillToMonsterKind( _info.nSkillTableIdx, monKindID, 1));
					}
				}
				else
					Debug.LogWarning( "MonsterEntity is null " + count);

				count++;
			}
		}
	}
	
	public Msg_OtherCharAttackNpc1( Msg_OtherCharSkillStance _stance)
	{
		sessionId_ = _stance.sessionIdx_;
		charUniqKey_ = _stance.charUniqKey_;
		
		npcIdx_ = new int[TargetDecider.MAX_SKILL_TARGET];
		mainCharUniqKey_ = new uint[TargetDecider.MAX_SKILL_TARGET];
		
		skillTableIdx_ = _stance.stanceSkill_;
		skillLevel_ = _stance.stanceLevel_;
		
		ready_ = true;
		
		skillLv_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( skillLevel_, skillTableIdx_);
		skill_ = AsTableManager.Instance.GetTbl_Skill_Record( skillLv_.Skill_GroupIndex);
		action_ = AsTableManager.Instance.GetTbl_Action_Record( skillLv_.SkillAction_Index);
		
		actionTableIdx_ = action_.Index;
	}
}

public class Msg_OtherCharAttackNpc2 : AsIMessage
{
	public Msg_OtherCharAttackNpc1 parent_ = null;
	public Int32 npcIdx_;
	public float hpCur_;
	public eDAMAGETYPE eDamageType_;
	public float damage_;
	public float heal_;
	public float reflection_;
	public float drain_;
	#region -Authority
	public bool authorityInMe;
	public bool authorityOwnerIsMe;
	#endregion

//	public Vector3 position_;

//	//ADDITONAL
//	public Tbl_SkillLevel_Record skillLv_;
//	public Tbl_Action_Record action_;

//	public Msg_OtherCharAttackNpc2( AS_SC_CHAR_ATTACK_NPC_2 _info)
//	{
//		m_MessageType = eMessageType.CHAR_ATTACK_NPC2;
//
//		npcIdx_ = _info.nNpcIdx;
//
//		hpCur_ = _info.nHpCur;
//		eDamageType = ( eDAMAGETYPE)_info.eDamageType;
//		damage_ = _info.nDamage;
//
//		reflection_ = _info.fReflection;
//		drain_ = _info.fDrain;
//
//		position_ = _info.vPosition;
//	}

	public Msg_OtherCharAttackNpc2( Msg_OtherCharAttackNpc1 _parent, AS_SC_CHAR_ATTACK_NPC_2 _info, UInt32 charUniqKey)
	{
		m_MessageType = eMessageType.CHAR_ATTACK_NPC2;

		parent_ = _parent;

		npcIdx_ = _info.nNpcIdx;

		hpCur_ = _info.nHpCur;
		eDamageType_ = ( eDAMAGETYPE)_info.eDamageType;
		damage_ = _info.nDamage;
		heal_ = _info.fHpHeal;

		reflection_ = _info.fReflection;
		drain_ = _info.fDrain;
		
		#region -Authority
		authorityOwnerIsMe = ( charUniqKey == AsUserInfo.Instance.GetCurrentUserEntity().UniqueId) ? true : false;
		if( true == authorityOwnerIsMe)
			authorityInMe = _info.bAggroMyself;
		#endregion
//		position_ = _info.vPosition;
	}

//	public void SetSkillLvRecord( Tbl_SkillLevel_Record _skillLv)
//	{
//		skillLv_ = _skillLv;
//	}

//	public void SetPotencyEffect( List<Tbl_SkillPotencyEffect_Record> _listPotency)
//	{
//		potency_ = _listPotency;
//	}
}

public class Msg_OtherCharAttackNpc3 : AsIMessage
{
	public Msg_OtherCharAttackNpc1 parent_ = null;

	public ushort sessionIdx_;
	public uint charUniqKey_;

	public float hpCur_;
	public float mpCur_;

	public eDAMAGETYPE eDamageType;
	public float damage_;
	public float hpHeal_;
	public float mpHeal_;

	public float reflection_;
	public float drain_;
	
	public bool knockBack_;

	public Msg_OtherCharAttackNpc3( Msg_OtherCharAttackNpc1 _parent, AS_SC_CHAR_ATTACK_NPC_3 _info)
	{
		m_MessageType = eMessageType.CHAR_ATTACK_NPC3;

		parent_ = _parent;

		sessionIdx_ = _info.nSessionIdx;
		charUniqKey_ = _info.nCharUniqKey;

		hpCur_ = _info.fHpCur;
		mpCur_ = _info.fMpCur;

		eDamageType = ( eDAMAGETYPE)_info.eDamageType;

		damage_ = _info.fDamage;

		hpHeal_ = _info.fHpHeal;
		mpHeal_ = _info.fMpHeal;

		reflection_ = _info.fReflection;
		drain_ = _info.fDrain;
		
		knockBack_ = _info.bKnockBack;
	}
}

public class Msg_OtherCharAttackNpc_Ready : AsIMessage
{
	public Tbl_Action_Record actionRecord{get{return action_;}}
	public Tbl_Skill_Record skillRecord{get{return skill_;}}
	public Tbl_SkillLevel_Record skillLvRecord{get{return skillLv_;}}
	public Msg_OtherCharAttackNpc1 attackMsg{get{return attack_;}}

	Tbl_Action_Record action_;
	Tbl_Skill_Record skill_;
	Tbl_SkillLevel_Record skillLv_;
	Msg_OtherCharAttackNpc1 attack_;

	float animSpeed_ = 1;public float animSpeed{get{return animSpeed_;}}

	public Msg_OtherCharAttackNpc_Ready( Msg_OtherCharAttackNpc1 _msg)//, float _animSpeed)
	{
		try
		{
			m_MessageType = eMessageType.OTHER_CHAR_ATTACK_READY;

			attack_ = _msg;

			skill_ = AsTableManager.Instance.GetTbl_Skill_Record( attack_.skillTableIdx_);
			skillLv_ = AsTableManager.Instance.GetTbl_SkillLevel_Record( 
				attack_.skillLevel_, attack_.skillTableIdx_, attack_.chargeStep_);
			action_ = AsTableManager.Instance.GetTbl_Action_Record( _msg.actionTableIdx_);

			animSpeed_ = 1f;

			if( animSpeed_ == 0f)
			{
				Debug.LogError( "Msg_OtherCharAttackNpc_Ready::constructor: animSpeed_ is set as 0. instead set animSpeed_ 1f");
				animSpeed_ = 1f;
			}
		}
		catch
		{
			Debug.Log( "Msg_OtherCharAttackNpc_Ready:constructor: error occured");
		}
	}

	public bool CheckNonAnimation()
	{
		if( action_.HitAnimation != null && action_.HitAnimation.FileName == "NonAnimation")
			return true;
		else
			return false;
	}
	
	public void SetAnimSpeed( float _animSpeed)
	{
		animSpeed_ = _animSpeed;

		if( animSpeed_ == 0f)
		{
			Debug.LogError( "Msg_OtherCharAttackNpc_Ready::SetAnimSpeed: animSpeed_ is set as 0. instead set animSpeed_ 1f");
			animSpeed_ = 1f;
		}
	}
}

public class Msg_OtherCharAttackNpc_Hit : AsIMessage
{
	public Msg_OtherCharAttackNpc_Ready ready_;

	public Msg_OtherCharAttackNpc_Hit( Msg_OtherCharAttackNpc_Ready _ready)
	{
		m_MessageType = eMessageType.OTHER_CHAR_ATTACK_HIT;

		ready_ = _ready;
	}
}

public class Msg_OtherCharAttackNpc_Finish : AsIMessage
{
	public Msg_OtherCharAttackNpc_Hit hit_;

	public Msg_OtherCharAttackNpc_Finish( Msg_OtherCharAttackNpc_Hit _hit)
	{
		m_MessageType = eMessageType.OTHER_CHAR_ATTACK_FINISH;

		hit_ = _hit;
	}
}

public class Msg_OtherCharAttackNpc_Link : AsIMessage
{
	public Msg_OtherCharAttackNpc_Ready ready_;

	public Msg_OtherCharAttackNpc_Link( Msg_OtherCharAttackNpc_Ready _ready)
	{
		m_MessageType = eMessageType.OTHER_CHAR_ATTACK_LINKACTION;

		ready_	= _ready;
	}
}

public class Msg_OtherCharSkillEffect : AsIMessage
{
	public ushort sessionIdx_;
	public uint charUniqKey_;

	public int skillTableIdx_;
	public int skillLevel_;
	public int potencyIdx_;
	public int chargeStep_;

	public Msg_OtherCharSkillEffect( body_SC_CHAR_SKILL_EFFECT _body)
	{
		m_MessageType = eMessageType.OTHER_CHAR_SKILL_EFFECT;

		sessionIdx_ = _body.nSessionIdx;
		charUniqKey_ = _body.nCharUniqKey;

		skillTableIdx_ = _body.nSkillTableIdx;
		skillLevel_ = _body.nSkillLevel;
		potencyIdx_ = _body.nPotencyIdx;
		chargeStep_ = _body.nChargeStep;
	}
}

public class Msg_OtherCharSkillStance : AsIMessage
{
	public ushort sessionIdx_;
	public uint charUniqKey_;

	public int stanceSkill_;
	public int stanceLevel_;
	public int stancePotencyIdx_;

	public Msg_OtherCharSkillStance( body_SC_CHAR_SKILL_STANCE _body)
	{
		m_MessageType = eMessageType.OTHER_CHAR_SKILL_STANCE;

		sessionIdx_ = _body.nSessionIdx;
		charUniqKey_ = _body.nCharUniqKey;

		stanceSkill_ = _body.nStanceSkill;
		stanceLevel_ = _body.nStanceSkillLevel;
		stancePotencyIdx_ = _body.nStancePotencyIdx;

		QuestMessageBroadCaster.BrocastQuest(QuestMessages.QM_USE_SKILL, new AchUseSkill(_body.nStanceSkill, 1));
	}
}

public class Msg_CharBuffAccure : AsIMessage // $yde
{
	public body_SC_CHAR_BUFF_ACCURE_INFO info_;
	
	public Msg_CharBuffAccure(body_SC_CHAR_BUFF_ACCURE_INFO _info)
	{
		m_MessageType = eMessageType.CHAR_BUFF_ACCURE;
		
		info_ = _info;
	}
	
	public Msg_CharBuffAccure( bool _critical, bool _dodge)
	{
		m_MessageType = eMessageType.CHAR_BUFF_ACCURE;
		
		info_ = new body_SC_CHAR_BUFF_ACCURE_INFO();
		info_.bCriticalChance = _critical;
		info_.bDodgeChance = _dodge;
	}
}

public class Msg_CharSkillAdd : AsIMessage
{
	public Int32 skillTableIdx_;
	public Int32 skillLevel_;
	
	public Msg_CharSkillAdd(body_SC_CHAR_SKILL_USE_ADD _add)
	{
		m_MessageType = eMessageType.CHAR_SKILL_ADD;
		
		skillTableIdx_ = _add.nSkillTableIdx;
		skillLevel_ = _add.nSkillLevel;
	}
}

public class Msg_CharSkillSoulStone : AsIMessage
{
	public body_SC_CHAR_SKILL_SOULSTONE soul_;
	public AsPlayerFsm player_;
	
	public int actionIdx_;
	
	public Msg_CharSkillSoulStone(body_SC_CHAR_SKILL_SOULSTONE _soul)
	{
		m_MessageType = eMessageType.CHAR_SKILL_SOULSTONE;
		
		soul_ = _soul;
	}
	
	public void SetPlayerFsm( AsPlayerFsm _player)
	{
		player_ = _player;
	}
	
	public void SetActionIdx( int _idx)
	{
		actionIdx_ = _idx;
	}
}

public class Msg_CharSkillPvpAggro : AsIMessage
{
	public ushort sessionIdx_;
	public uint charUniqKey_;
	
	public Msg_CharSkillPvpAggro(body_SC_CHAR_SKILL_PVP_AGGRO _aggro)
	{
		m_MessageType = eMessageType.CHAR_SKILL_PVP_AGGRO;
		
		sessionIdx_ = _aggro.nSessionIdx;
		charUniqKey_ = _aggro.nCharUniqKey;
	}
}
#endregion

#region - combat relaxation -
public class Msg_CombatBegin : AsIMessage
{
	public Msg_CombatBegin()
	{
		m_MessageType = eMessageType.COMBAT_BEGIN;
	}
}

public class Msg_CombatEnd : AsIMessage
{
	public Msg_CombatEnd()
	{
		m_MessageType = eMessageType.COMBAT_END;
	}
}

public class Msg_ReleaseTension : AsIMessage
{
	public Msg_ReleaseTension()
	{
		m_MessageType = eMessageType.RELEASE_TENSION;
	}
}
#endregion

#region - buff -
public class Msg_CharBuff : AsIMessage
{
	public ushort sessionIdx_;
	public uint charUniqKey_;

	public bool effect_;//true:perform effect, false:don't

	public float moveSpeed_;

	public List<Msg_CharBuff_Body> listBuff_ = new List<Msg_CharBuff_Body>();

	public Msg_CharBuff( body1_SC_CHAR_BUFF _buff)
	{
		m_MessageType = eMessageType.CHAR_BUFF;

		sessionIdx_ = _buff.nSessionIdx;
		charUniqKey_ = _buff.nCharUniqKey;

		effect_ = _buff.bEffect;

		for( int i=0; i<_buff.nBuffCnt; ++i)
		{
			listBuff_.Add( new Msg_CharBuff_Body(this, _buff.body[i]));
		}
	}
}

public class Msg_CharBuff_Body
{
	public Msg_CharBuff parent_;
	
	public uint charUniqKey_;
	
	public int skillTableIdx_;
	public int skillLevelTableIdx_;
	public int skillLevel_;
	public int chargeStep_;
	public int potencyIdx_;
	public eBUFFTYPE type_;
	public int duration_;
	public body2_SC_CHAR_BUFF serverData;

	public Msg_CharBuff_Body(Msg_CharBuff _parent,  body2_SC_CHAR_BUFF _buff)
	{
		parent_ = _parent;
		
		charUniqKey_ = _buff.nCharUniqKey;
		
		skillTableIdx_ = _buff.nSkillTableIdx;
		skillLevelTableIdx_ = _buff.nSkillLevelTableIdx;
		skillLevel_ = _buff.nSkillLevel;
		chargeStep_ = _buff.nChargeStep;
		potencyIdx_ = _buff.nPotencyIdx;
		type_ = _buff.eType;
		duration_ = _buff.nDuration;
		serverData = _buff;
	}
	
	public Msg_CharBuff_Body( int _floatTime)
	{
		duration_ = _floatTime;
	}
}

public class Msg_CharDeBuff : AsIMessage
{
	public ushort sessionIdx_;
	public uint charUniqKey_;
	public int skillTableIdx_;
	public int skillLevelTableIdx_;
	public int skillLevel_;
	public int chargeStep_;
	public int potencyIdx_;
	public eBUFFTYPE type_;
	public Single moveSpeed_;
	public body_SC_CHAR_DEBUFF serverData;

	public Msg_CharDeBuff( body_SC_CHAR_DEBUFF _debuff)
	{
		m_MessageType = eMessageType.CHAR_DEBUFF;

		sessionIdx_ = _debuff.nSessionIdx;
		charUniqKey_ = _debuff.nCharUniqKey;

		skillTableIdx_ = _debuff.nSkillTableIdx;
		skillLevelTableIdx_ = _debuff.nSkillLevelTableIdx;

		potencyIdx_ = _debuff.nSkillLevel;
		chargeStep_ = _debuff.nChargeStep;
		potencyIdx_ = _debuff.nPotencyIdx;
		type_ = _debuff.eType;

		serverData = _debuff;
	}
}

public class Msg_CharDeBuffResist : AsIMessage
{
	public body_SC_CHAR_DEBUFF_RESIST data_;

	public Msg_CharDeBuffResist( body_SC_CHAR_DEBUFF_RESIST _resist)
	{
		m_MessageType = eMessageType.CHAR_DEBUFF_RESIST;

		data_ = _resist;
	}
}

public class Msg_NpcBuff : AsIMessage
{
	public int npcIdx_;

	public bool effect_;

	public List<Msg_NpcBuff_Body> body_ = new List<Msg_NpcBuff_Body>();

	public Msg_NpcBuff( body1_SC_NPC_BUFF _buff)
	{
		m_MessageType = eMessageType.NPC_BUFF;

		npcIdx_ = _buff.nNpcIdx;

		effect_ = _buff.bEffect;

		foreach( body2_SC_NPC_BUFF node in _buff.body)
		{
			Msg_NpcBuff_Body body = new Msg_NpcBuff_Body();

			body.update_ = node.bUpdate;
			body.skillTableIdx_ = node.nSkillTableIdx;
			body.skillLevelTableIdx_ = node.nSkillLevelTableIdx;
			body.skillLevel_ = node.nSkillLevel;
			body.chargeStep_ = node.nChargeStep;
			body.potencyIdx_ = node.nPotencyIdx;
			body.type_ = node.eType;
			body.duration_ = node.nDuration;
			body.serverData = node;
			body_.Add( body);
		}
	}
}

public class Msg_NpcBuff_Body
{
	public bool update_;
	public int skillTableIdx_;
	public int skillLevelTableIdx_;
	public int skillLevel_;
	public int chargeStep_;
	public int potencyIdx_;
	public eBUFFTYPE type_;
	public int duration_;

	public body2_SC_NPC_BUFF serverData;
}

public class Msg_NpcDeBuff : AsIMessage
{
	public int npcIdx_;

	public int skillTableIdx_;
	public int skillLevelTableIdx_;
	public int skillLevel_;
	public int chargeStep_;
	public int potencyIdx_;
	public eBUFFTYPE type_;
	public body_SC_NPC_DEBUFF serverData;

	public Msg_NpcDeBuff( body_SC_NPC_DEBUFF _deBuff)
	{
		m_MessageType = eMessageType.NPC_DEBUFF;

		npcIdx_ = _deBuff.nNpcIdx;

		skillTableIdx_ = _deBuff.nSkillTableIdx;
		skillLevelTableIdx_ = _deBuff.nSkillLevelTableIdx;
		skillLevel_ = _deBuff.nSkillLevel;
		chargeStep_ = _deBuff.nChargeStep;
		potencyIdx_ = _deBuff.nPotencyIdx;
		type_ = _deBuff.eType;

		serverData = _deBuff;
	}
}

public class Msg_BuffRefresh : AsIMessage
{
	public List<body2_SC_CHAR_BUFF> list_ = null;
	
	public Msg_BuffRefresh()
	{
		m_MessageType = eMessageType.BUFF_REFRESH;
	}
	
	public Msg_BuffRefresh( List<body2_SC_CHAR_BUFF> _list)
	{
		m_MessageType = eMessageType.BUFF_REFRESH;

		list_ = _list;
	}
}

public class Msg_BuffInclusion : AsIMessage
{
	public AsBaseEntity sender_;

	public int skillIdx_;

	public Msg_BuffInclusion( AsBaseEntity _sender, int _skillIdx)//, int _potencyIdx)
	{
		m_MessageType = eMessageType.BUFF_INCLUSION;

		sender_ = _sender;

		skillIdx_ = _skillIdx;
	}
}

public class Msg_DeathDebuffClear : AsIMessage
{
	public Msg_DeathDebuffClear()
	{
		m_MessageType = eMessageType.DEATH_DEBUFF_CLEAR;
	}
}

public class Msg_DeathPenaltyIndicate :AsIMessage
{
	public bool penalty_;
	
	public Msg_DeathPenaltyIndicate( bool _penalty)
	{
		m_MessageType = eMessageType.DEATH_PENALTY_INDICATE;
		
		penalty_ = _penalty;
	}
}
#endregion

#region - effect -
public class Msg_EffectIndication : AsIMessage
{
	public int index_;

	public Msg_EffectIndication( int _index)
	{
		m_MessageType = eMessageType.EFFECT_INDICATION;

		index_ = _index;
	}
}

public class Msg_EffectGeneration : AsIMessage
{
	public string path_;

	public eLinkType linkType_;
	public string dummy_;
	public eProjectileEffLoopType loopType_;
	public float loopDuration_;
	public float speed_;

	public Transform owner_;
	public Transform target_;
	public HitResultFunc func_;

//	public Msg_EffectGeneration( string _path, string _fileName, eLinkType _linkType,
//		string _dummy, eEffLoopType _loopType, float _loopDuration)
//	{
//		m_MessageType = eMessageType.EFFECT_GENERATION;
//
//		fileName_ = _fileName;
//		linkType_ = _linkType;
//		dummy_ = _dummy;
//		loopType_ = _loopType;
//		loopDuration_ = _loopDuration;
//
//		string[] strings = _fileName.Split( new char[]{'@'});
//		foreach( string str in strings)
//		{
//			Debug.Log( str);
//		}
//
//		string __class = strings[0];
//		string skill = strings[1];
//
//		__class = __class.Replace( "Fx_", "");
//
//
//		if( __class == "Common")
//			__class = "COMMON";
//
//		path_ = _path + __class + "/" + skill;
//
//		path_ = _path + _fileName;
//	}

	public Msg_EffectGeneration( string _path, float _loopDuration, float _speed,
		Transform _owner, Transform _target, HitResultFunc _func)
	{
		m_MessageType = eMessageType.EFFECT_GENERATION;

		path_ = _path;

		loopDuration_ = _loopDuration;
		speed_ = _speed;

		owner_ = _owner;
		target_ = _target;
		func_ = _func;
	}

	public static string[] GetEffectSplitted( string _fullFileName)
	{
		string[] strings = _fullFileName.Split( new char[]{'@'});
		strings[0] = strings[0].Replace( "Fx_", "");

		return strings;
	}
}

public class Msg_EffectRelease : AsIMessage
{
	public string fileName_;

	public Msg_EffectRelease( string _fileName)
	{
		m_MessageType = eMessageType.EFFECT_RELEASE;

		fileName_ = _fileName;
	}
}
#endregion

#region - hit impact( sync) -
public class Msg_HitExecution : AsIMessage
{
	public Msg_HitExecution()
	{
		m_MessageType = eMessageType.HIT_EXECUTION;
	}
}

public class Msg_HitPotency : AsIMessage
{
	public List<Tbl_SkillPotencyEffect_Record> potency{get{return potency_;}}List<Tbl_SkillPotencyEffect_Record> potency_;

	public Msg_HitPotency( List<Tbl_SkillPotencyEffect_Record> _potency)
	{
		m_MessageType = eMessageType.HIT_POTENCY;

		potency_ = _potency;
	}
}
#endregion

#region - attribute -
public class Msg_AttackSpeedRefresh : AsIMessage
{
	public float attackSpeed_;

	public Msg_AttackSpeedRefresh( float _speed)
	{
		m_MessageType = eMessageType.ATTACK_SPEED_REFRESH;

		attackSpeed_ = _speed;
	}
}

public class Msg_Level_Up : AsIMessage
{
	public body_SC_ATTR_LEVELUP data_;

	public Msg_Level_Up( body_SC_ATTR_LEVELUP _data)
	{
		m_MessageType = eMessageType.LEVEL_UP;

		data_ = _data;
	}
}
#endregion

#region - condition -
public class Msg_ConditionRecover_Stun : AsIMessage
{
	public Msg_ConditionRecover_Stun()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_STUN;
	}
}

public class Msg_ConditionRecover_Freeze : AsIMessage
{
	public Msg_ConditionRecover_Freeze()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_FREEZE;
	}
}

public class Msg_ConditionRecover_Sleep : AsIMessage
{
	public Msg_ConditionRecover_Sleep()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_SLEEP;
	}
}

public class Msg_ConditionRecover_Burning : AsIMessage
{
	public Msg_ConditionRecover_Burning()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_BURNING;
	}
}

public class Msg_ConditionIndicate_Stun : AsIMessage
{
	public string effectName_;
	public float time_;

	public Msg_ConditionIndicate_Stun()
	{
		m_MessageType = eMessageType.CONDITION_STUN;
	}
}

public class Msg_ConditionIndicate_Freeze : AsIMessage
{
	public string effectName_;
	public float time_;

	public Msg_ConditionIndicate_Freeze()
	{
		m_MessageType = eMessageType.CONDITION_FREEZE;
	}
}

public class Msg_ConditionIndicate_Sleep : AsIMessage
{
	public string effectName_;
	public float time_;

	public Msg_ConditionIndicate_Sleep()
	{
		m_MessageType = eMessageType.CONDITION_SLEEP;
	}
}

public class Msg_ConditionIndicate_Burning : AsIMessage
{
	public string effectName_;
	public float time_;

	public Msg_ConditionIndicate_Burning()
	{
		m_MessageType = eMessageType.CONDITION_BURNING;
	}
}

public class Msg_ConditionIndicate_ForcedMove : AsIMessage
{
	public Vector3 destination_;
	public float distance_;
	public float duration_;
	
	public bool fromBind_ = false;

	public Msg_ConditionIndicate_ForcedMove( Vector3 _destination, float _distance, float _duration)
	{
		m_MessageType = eMessageType.CONDITION_FORCEDMOVE;

		destination_ = _destination;
		distance_ = _distance;
		duration_ = _duration;
	}
	
	public void PrevState_Bind()
	{
		fromBind_ = true;
	}
}

public class Msg_ConditionIndicate_Fear : AsIMessage
{
	public Msg_ConditionIndicate_Fear()
	{
		m_MessageType = eMessageType.CONDITION_FEAR;
	}
}

public class Msg_ConditionRecover_Fear : AsIMessage
{
	public Msg_ConditionRecover_Fear()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_FEAR;
	}
}

public class Msg_ConditionIndicate_SizeControl : AsIMessage
{
//	public Tbl_SkillLevel_Potency potency_;
	public float size_;

//	public Msg_ConditionIndicate_SizeControl( Tbl_SkillLevel_Potency _potency)
	public Msg_ConditionIndicate_SizeControl( float _size)
	{
		m_MessageType = eMessageType.CONDITION_SIZECONTROL;

		size_ = _size;
//		potency_ = _potency;
	}
}

public class Msg_ConditionRecover_SizeControl : AsIMessage
{
	public Tbl_SkillLevel_Potency potency_;

	public Msg_ConditionRecover_SizeControl( Tbl_SkillLevel_Potency _potency)
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_SIZECONTROL;

		potency_ = _potency;
	}
}

public class Msg_ConditionIndicate_Bind : AsIMessage
{
	public Msg_ConditionIndicate_Bind()
	{
		m_MessageType = eMessageType.CONDITION_BINDING;
	}
}

public class Msg_ConditionRecover_Bind : AsIMessage
{
	public Msg_ConditionRecover_Bind()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_BINDING;
	}
}

public class Msg_ForcedMove_Sync : AsIMessage
{
	public Vector3 destination_;

	public Msg_ForcedMove_Sync( Vector3 _destination)
	{
		m_MessageType = eMessageType.FORCEDMOVE_SYNC;

		destination_ = _destination;
	}
}

public class Msg_ConditionIndicate_Blank : AsIMessage
{
	public Msg_ConditionIndicate_Blank()
	{
		m_MessageType = eMessageType.CONDITION_BLANK;
	}
}

public class Msg_ConditionRecover_Blank : AsIMessage
{
	public Msg_ConditionRecover_Blank()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_BLANK;
	}
}

public class Msg_ConditionIndicate_AirBone : AsIMessage
{
	public Msg_CharBuff_Body body_;
	
	public Msg_ConditionIndicate_AirBone( Msg_CharBuff_Body _body)
	{
		m_MessageType = eMessageType.CONDITION_AIRBONE;
		
		body_ = _body;
	}
}

public class Msg_ConditionRecover_AirBone : AsIMessage
{
	public Msg_ConditionRecover_AirBone()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_AIRBONE;
	}
}

public class Msg_ConditionIndicate_Transform : AsIMessage
{
	public int idx_;

	public Msg_ConditionIndicate_Transform(int _idx)
	{
		m_MessageType = eMessageType.CONDITION_TRANSFORM;

		idx_ = _idx;
	}
}

public class Msg_ConditionRecover_Transform : AsIMessage
{
	public Msg_ConditionRecover_Transform()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_TRANSFORM;
	}
}

public class Msg_ConditionIndicate_Gray : AsIMessage
{
	public Msg_ConditionIndicate_Gray()
	{
		m_MessageType = eMessageType.CONDITION_GRAY;
	}
}

public class Msg_ConditionRecover_Gray : AsIMessage
{
	public Msg_ConditionRecover_Gray()
	{
		m_MessageType = eMessageType.RECOVER_CONDITION_GRAY;
	}
}

public class Msg_ShadowControl : AsIMessage
{
	public bool active_;
	
	public Msg_ShadowControl( bool _active)
	{
		m_MessageType = eMessageType.SHADOW_CONTROL;
		
		active_ = _active;
	}
}

public class Msg_Transform :AsIMessage
{
	public Msg_Transform()
	{
		m_MessageType = eMessageType.TRANSFORM;
	}
}
#endregion

#region - npc communication -
public class Msg_NpcBeginDialog : AsIMessage
{
	public AsUserEntity user_;

	public Msg_NpcBeginDialog( AsUserEntity _user)
	{
		m_MessageType = eMessageType.NPC_BEGIN_DIALOG;

		user_ = _user;
	}
}
#endregion

#region - obj -
public class Msg_ObjBreak : AsIMessage
{
	public int m_iObjTableID;

	public Msg_ObjBreak( int iObjTableID)
	{
		m_MessageType = eMessageType.OBJ_BREAK_MSG;
		m_iObjTableID = iObjTableID;
	}
}

public class Msg_ObjStepping : AsIMessage
{
	public AsNpcEntity m_AsObjectEntity;
	public ObjStepping.eSTEPPIG_STATE m_eSteppingState;

	public Msg_ObjStepping( ObjStepping.eSTEPPIG_STATE eState)
	{
		m_MessageType = eMessageType.OBJ_STEPPING_MSG;
		m_eSteppingState = eState;
	}
}
#endregion

#region - getter -
public class Msg_AnimationClipReceiver : AsIMessage
{
	public string animName_;
	public ClipFunc clipFunc_;

	public Msg_AnimationClipReceiver( string _name, ClipFunc _func)
	{
		m_MessageType = eMessageType.ANIMATION_CLIP_RECEIVER;

		animName_ = _name;
		clipFunc_ = _func;
	}
}
#endregion

#region - state -
public class Msg_RecoverState : AsIMessage
{
	public Msg_RecoverState()
	{
		m_MessageType = eMessageType.RECOVER_STATE;
	}
}

public class Msg_DeathIndication : AsIMessage
{
	public Msg_DeathIndication()
	{
		m_MessageType = eMessageType.DEATH_INDICATION;
	}
}

public class Msg_Delay_BattleRun : AsIMessage
{
	public Msg_Delay_BattleRun()
	{
		m_MessageType = eMessageType.DELAY_BATTLE_RUN;
	}
}
#endregion

#region - shake -
public class Msg_ShakeIndication : AsIMessage
{
	public float time_;
	public float dist_;

	public Msg_ShakeIndication( float _time, float _dist)
	{
		m_MessageType = eMessageType.SHAKE_INDICATION;

		time_ = _time;
		dist_ = _dist;
	}
}
#endregion

#region - target -
public class Msg_TargetIndication : AsIMessage
{
	public AsBaseEntity sender_;
	public eMessageType typeCalled_;

	public Msg_TargetIndication( AsBaseEntity _sender, eMessageType _typeCalled)
	{
		m_MessageType = eMessageType.TARGET_INDICATION;

		sender_ = _sender;
		typeCalled_ = _typeCalled;
	}
}
#endregion

#region - private shop -
public class Msg_OpenPrivateShop : AsIMessage
{
	public Item item_;

	public Msg_OpenPrivateShop( int _idx)
	{
		m_MessageType = eMessageType.OPEN_PRIVATESHOP;

		item_ = ItemMgr.ItemManagement.GetItem( _idx);
	}
}

public class Msg_ClosePrivateShop : AsIMessage
{
	public Msg_ClosePrivateShop()
	{
		m_MessageType = eMessageType.CLOSE_PRIVATESHOP;
	}
}

public class Msg_PrevState_PrivateShop : AsIMessage
{
	public Msg_PrevState_PrivateShop()
	{
		m_MessageType = eMessageType.PREV_STATE_PRIVATESHOP;
	}
}

public class Msg_OpenPrivateShopUI : AsIMessage
{
	public Msg_OpenPrivateShopUI()
	{
		m_MessageType = eMessageType.OPEN_PRIVATESHOP_UI;
	}
}
#endregion

#region - emotion & emoticon -
public class Msg_EmotionIndicate : AsIMessage
{
	public Tbl_Emotion_Record record_;

	public Msg_EmotionIndicate( Tbl_Emotion_Record _record)
	{
		m_MessageType = eMessageType.EMOTION_INDICATION;

		record_ = _record;
	}
}

public class Msg_EmoticonIndicate : AsIMessage
{
	public int index_;

	public Msg_EmoticonIndicate( body_SC_CHAT_EMOTICON_RESULT _result)
	{
		m_MessageType = eMessageType.EMOTICON_INDICATION;

		index_ = _result.nIndex;
	}
}

public class Msg_Emoticon_Seat_Indicate : AsIMessage
{
	public Msg_Emoticon_Seat_Indicate( Tbl_Emoticon_Record _record)
	{
		m_MessageType = eMessageType.EMOTICON_SEAT_INDICATION;
	}
}

public class Msg_BalloonIndicate : AsIMessage
{
	public Tbl_SkillLevel_Potency potency_;
	
	public Msg_BalloonIndicate( Tbl_SkillLevel_Potency _potency)
	{
		m_MessageType = eMessageType.BALLOON;
		
		potency_ = _potency;
	}
}
#endregion

#region - auto combat -
public class Msg_AutoCombat_On : AsIMessage
{
	public Msg_AutoCombat_On()
	{
		m_MessageType = eMessageType.AUTOCOMBAT_ON;
	}
}

public class Msg_AutoCombat_Off : AsIMessage
{
	public bool release_;
	
	public Msg_AutoCombat_Off(bool _release)
	{
		m_MessageType = eMessageType.AUTOCOMBAT_OFF;
		
		release_ = _release;
	}
}

public class Msg_AutoCombat_Search : AsIMessage
{
	public int skillIdx_;
	public int step_;

	public Msg_AutoCombat_Search( int _skillIdx, int _step)
	{
		m_MessageType = eMessageType.AUTOCOMBAT_SEARCH;

		skillIdx_ = _skillIdx;
		step_ = _step;
	}
	
//	public Msg_AutoCombat_Search()
//	{
//		m_MessageType = eMessageType.AUTOCOMBAT_SEARCH;
//	}
}
#endregion