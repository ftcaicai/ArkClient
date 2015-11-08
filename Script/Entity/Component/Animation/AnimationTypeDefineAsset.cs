using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region - common -
public enum eCommonAnimType
{
	NONE = 0,
	//select
	CHOICE,
	//village
	IDLE, WALK, IDLE_ACTION,
	//att 1
	BATTLE_IDLE_01,
	RUN_01,
	DASH_READY_01, DASH_SLIDE_01, DASH_END_01,
	JUMP_READY_01, JUMP_AIR_01, JUMP_LANDING_01,
	RECOVER_01,
	WAKE_01,
	HIT_01,
	CONDITION_STUN_01,
	DEATH_FALL_01, DEATH_STOP_01,
	LEVEL_UP,
	BASIC_ATTACK_01,
	//COMMAND_SKILL1_01,
	//COMMAND_SKILL2_01,
	//ACTIVE_SKILL1_01,
	//ACTIVE_SKILL2_CHARGE_01,
	//ACTIVE_SKILL2_CHARGETACTION_01,
	//CHARACTERISTIC_SKILL
	CHANGE_01, CHANGE_02,
	//att 2
	BATTLE_IDLE_02,
	RUN_02,
	DASH_READY_02, DASH_SLIDE_02, DASH_END_02,
	JUMP_READY_02, JUMP_AIR_02, JUMP_LANDING_02,
	RECOVER_02,
	WAKE_02,
	HIT_02,
	CONDITION_STUN_02,
	DEATH_FALL_02, DEATH_STOP_02,
	BASIC_ATTACK_02,
	//COMMAND_SKILL1_02,
	//COMMAND_SKILL2_02,
	//ACTIVE_SKILL1_02,
	//ACTIVE_SKILL2_CHARGE_02,
	//ACTIVE_SKILL2_CHARGEACTION_02,
	//CHARACTERISTIC_SKILL
	
	#region - divine knight -
	SKILL_3_COMBO = 200,
	SKILL_SWORD_WAVE,
	SKILL_SWORD_SMASH,
	SKILL_SWORD_QUAKE,
	SKILL_ARCHANGEL_CHARGE,
	SKILL_ARCHANGEL_1_STEP,
	SKILL_ARCHANGEL_2_STEP,
	SKILL_ARCHANGEL_3_STEP,
	SKILL_ARCHANGEL_4_STEP,
	SKILL_SHIELD_START_01, SKILL_SHIELD_ON_01, SKILL_SHIELD_END_01,
	
	SKILL_2_COMBO,
	SKILL_SHIELD_DASH_READY, SKILL_SHIELD_DASH_SLIDE, SKILL_SHIELD_DASH_END,
	SKILL_SHIELD_CRASH,
	SKILL_REVELATION,
	SKILL_GUARDIAN,
	SKILL_GUARDIAN_CHARGE,
	SKILL_SHIELD_START_02,  SKILL_SHIELD_ON_02, SKILL_SHIELD_END_02,
	#endregion
	#region - magician -
	SKILL_FIRE_BALL = 300,
	SKILL_FIRE_WAVE,
	SKILL_FIRE_FILLER,
	SKILL_INFERNO,
	SKILL_METEOR_CHARGE,
	SKILL_METEOR_1_STEP,
	SKILL_METEOR_2_STEP,
	SKILL_METEOR_3_STEP,
	SKILL_METEOR_4_STEP,
	//characteristic
	#endregion
	#region - ark technical -
	SKILL_DOUBLE_BEAM = 400,
	SKILL_BEAM_GATLING,
	SKILL_HALF_BEAM,
	SKILL_HOMING_BALL,
	SKILL_BEAM_STORM,
	SKILL_BEAM_STORM_CHARGE,
	//characteristic
	#endregion
	#region - monster -
	
	MONSTER_SKILL,
	#region - forest -
	
	#region - agripe -
	AGRIPE_HIT_BOOM,
	AGRIPE_DRAW_BOOM_READY,
	AGRIPE_DRAW_BOOM_ACTION,
	#endregion
	#region - giblin -
	GIBLIN_SWORD_SWING,
	GIBLIN_JUMP_IMPRINT,
	#endregion
	#region - harpy -
	HARPY_WING_ATTACK,
	HARPY_TORNADO,
	#endregion
	#region - terastone -
	TERASTONE_LEFT_SWING_PUNCH_READY,
	TERASTONE_LEFT_SWING_PUNCH_ACTION,
	TERASTONE_LEFT_SWING_PUNCH_END,
	TERASTONE_RIGHT_SWING_PUNCH_READY,
	TERASTONE_RIGHT_SWING_PUNCH_ACTION,
	TERASTONE_ROCKET_PUNCH_READY,
	TERASTONE_ROCKET_PUNCH_ACTION,
	TERASTONE_ROCKET_PUNCH_END,
	TERASTONE_LASER_BEAM_READY,
	TERASTONE_LASER_BEAM_ACTION,
	#endregion
	#region - tretanus -
	TRETANUS_XCLAW,
	TRETANUS_TENTACLE_SPURT_READY,
	TRETANUS_TENTACLE_SPURT_ACTION,
	TRETANUS_TENTACLE_SPURT_END,
	TRETANUS_BUG_RECALL_READY,
	TRETANUS_BUG_RECALL_ACTION,
	TRETANUS_POISON_WAVE_READY,
	TRETANUS_POISON_WAVE_ACTION,
	#endregion
	#region - claw insect -
	CLAWINSECT_BITE,
	#endregion
	
	#endregion
	
	#endregion
}
#endregion

[System.Serializable]
public class AnimationTypeInfo
{
	//public eAnimationType type_;
	public string clipName_;
	public int layer_;
	
	public WrapMode wrap_;
	
//	public AnimationTypeInfo(string _name, int _layer)
//	{
//		clipName_ = _name;
//		layer_ = _layer;
//	}
	
	public AnimationTypeInfo(string _name, int _layer, WrapMode _wrap)
	{
		clipName_ = _name;
		layer_ = _layer;
		wrap_ = _wrap;
	}
}

public class AnimationTypeDefineAsset : ScriptableObject
{
	public AnimationTypeInfo[] character_;
	public AnimationTypeInfo[] monster_;
	public AnimationTypeInfo[] npc_;
}

public class AnimationTypeDefine
{
	#region - singleton -
	static AnimationTypeDefine m_Instance = new AnimationTypeDefine();
	public static AnimationTypeDefine Instance	{ get { return m_Instance; } }
	#endregion
	
//	List<AnimationTypeInfo> m_listAnimInfo = new List<AnimationTypeInfo>();
//	List<AnimationTypeInfo> m_listMonsterAnimInfo = new List<AnimationTypeInfo>();
//	
//	Dictionary<string, AnimationTypeInfo> m_dicAnimInfo = new Dictionary<string, AnimationTypeInfo>();
//	Dictionary<string, AnimationTypeInfo> m_dicMonstersAnimInfo = new Dictionary<string, AnimationTypeInfo>();
	
	Dictionary<string, AnimationTypeInfo> m_dicNpcAnimInfo = new Dictionary<string, AnimationTypeInfo>();
	
	AnimationTypeDefine()
	{
		AnimationTypeDefineAsset asset = Resources.Load("UseScript/AnimationTypeList") as AnimationTypeDefineAsset;
		AnimationTypeInfo info = null;
		for(int i=0; i<asset.npc_.Length; ++i)
		{
			info = asset.npc_[i];
			m_dicNpcAnimInfo.Add(info.clipName_, info);
		}

//		foreach(AnimationTypeInfo info in asset.character_)
//		{
//			m_listAnimInfo.Add(info);
//			m_dicAnimInfo.Add(info.clipName_, info);
////			if(m_dicAnimInfo.ContainsKey(info.clipName_) == true)
////				Debug.LogError("m_dicAnimInfo already has " + info.clipName_);
////			else
////				m_dicAnimInfo.Add(info.clipName_, info);
//		}
//		
//		foreach(AnimationTypeInfo info in asset.npc_)
//		{
//			m_listMonsterAnimInfo.Add(info);
//			m_dicMonstersAnimInfo.Add(info.clipName_, info);
//		}
	}
	
	#region - set anim info -
//	public bool SetAnimInfo(Animation _animation)
//	{
//		bool result = true;
//		
//		foreach(AnimationTypeInfo info in m_listAnimInfo)
//		{
//			AnimationState state = _animation[info.clipName_];
//			if(state != null)
//			{
//				state.layer = info.layer_;
//				state.wrapMode = info.wrap_;
//			}
//			else
//			{
////					Debug.LogError("AnimationTypeDefine::SetAnimInfo: Invalid animation CLIP NAME - " + 
////						pair.Value.clipName_.ToString(, WrapMode.Loop));
//				result = false;
//			}
//		}
//
//		return result;
//	}
//	
//	public bool SetNpcAnimInfo(Animation _animation)
//	{
//		bool result = true;
//		
//		foreach(AnimationTypeInfo info in m_listMonsterAnimInfo)
//		{
//			AnimationState state = _animation[info.clipName_];
//			if(state != null)
//			{
//				state.layer = info.layer_;
//				state.clip.wrapMode = info.wrap_;
//			}
//			else
//			{
////					Debug.LogError("AnimationTypeDefine::SetAnimInfo: Invalid animation CLIP NAME - " + 
////						pair.Value.clipName_.ToString(, WrapMode.Loop));
//				result = false;
//			}
//		}
//		
//		return result;
//	}
	#endregion
	#region - check anim -
//	public bool CheckBasicAnimation(string _act)
//	{
//		if(m_dicAnimInfo.ContainsKey(_act) == true)
//			return true;
//		else
//			return false;
//	}
//	
//	public bool CheckBasicNpcAnimation(string _act)
//	{
//		if(m_dicMonstersAnimInfo.ContainsKey(_act) == true)
//			return true;
//		else
//			return false;
//	}
	#endregion
	#region - getter -
	public AnimationTypeInfo GetNpcAnimInfo(string _anim)
	{
		if(m_dicNpcAnimInfo.ContainsKey(_anim) == true)
		{
			return m_dicNpcAnimInfo[_anim];
		}
		else
		{
//			Debug.LogWarning("AnimationTypeDefine::GetNpcAnimInfo: '" + _anim + "' animation type is not defined");
			return null;
		}
	}
	#endregion
}