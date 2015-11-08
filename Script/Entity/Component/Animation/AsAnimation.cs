using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region - public class AnimationTypeDefine -
//public class AnimationTypeDefine{
//	public class Info
//	{
//		public string m_ClipName;
//		public int m_Layer;
//
//		public Info( string _clipName, int _layer)
//		{
//			m_ClipName = _clipName;
//			m_Layer = _layer;
//		}
//	}
//
//	static Dictionary<eAnimationType, Info> s_dicAnimationType;
//	public static Dictionary<eAnimationType, Info> sDicAnimationType
//	{
//		get{
//			if( s_dicAnimationType == null)
//			{
//				s_dicAnimationType = new Dictionary<eAnimationType, Info>();
//				AnimationTypeDefineAsset asset =
//					Resources.Load( "Character/AnimationTypeList") as AnimationTypeDefineAsset;
//
//				foreach( AnimationTypeInfo info in asset.data_)
//				{
//					s_dicAnimationType.Add( info.type_, new Info( info.clipName_, info.layer_));
//				}
//			}
//			return s_dicAnimationType;
//		}
//	}
//}
#endregion

public delegate void ClipFunc( AnimationClip _clip);

public class AsAnimation : AsBaseComponent
{
	#region - member -
	[SerializeField] float m_CurAnimSpeed = 0;
	[SerializeField] string m_PlayingAnimation = "Idle";
	public string PlayingAnimation{get{return m_PlayingAnimation;}}

	bool m_AnimEnded = false;
	AnimationState m_AnimState;

	float m_NextFadeTime = 0.1f;
	float m_PlayTime = 0;

	//target loop
	float m_TargetLoopTime = float.MaxValue;
	float m_TargetLoopDuration = 0;
	enum eTargetLoopState {NONE, Before, TargetLoop, After}
	eTargetLoopState m_TargetLoopState = eTargetLoopState.NONE;

	readonly float m_ErrorRecoveryTime = 3;

	static AnimationEventAsset m_EventAsset = null;
	List<AnimationEventInfo> m_listEvent = new List<AnimationEventInfo>();
	List<AnimEventReceiver_Base> m_listReceiver = new List<AnimEventReceiver_Base>();

	Animation m_AnimComponent = null;
	#endregion

	#region - init -
	void Awake()
	{
		m_ComponentType = eComponentType.ANIMATION;

		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_INDICATION, OnAnimation);
		MsgRegistry.RegisterFunction( eMessageType.ANIMATION_CLIP_RECEIVER, OnAnimClipReceiver);
		MsgRegistry.RegisterFunction( eMessageType.FADETIME_INDICATION, OnFadeTime);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_SPEED_REFRESH, OnChangeMoveSpeed);
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);

		_entity.SetAnimationTime( GetAnimationTimeDelegate);
	}

	public override void InterInit( AsBaseEntity _entity)
	{
	}

	void Start()
	{
	}
	#endregion

	#region - update -
	void Update()
	{
		CheckAnimationEnd();
		TargetLoopProcess();
	}

	void CheckAnimationEnd()
	{
		if( m_AnimEnded == false)
		{
			if( m_AnimState != null)
			{
				switch( m_AnimState.wrapMode)
				{
				case WrapMode.Once:
				case WrapMode.Default:
					if( m_AnimComponent.isPlaying == false)
					{
						m_AnimEnded = true;
						m_Entity.HandleMessage( new Msg_AnimationEnd( m_AnimState.name));
					}
					break;
				case WrapMode.ClampForever:
					if( m_AnimState.time >= m_AnimState.length)
					{
						m_AnimEnded = true;
						m_Entity.HandleMessage( new Msg_AnimationEnd( m_AnimState.name));
					}
					break;
				case WrapMode.Loop:
				case WrapMode.PingPong:
					break;
				}
			}
		}
	}

	void TargetLoopProcess()
	{
		if( m_TargetLoopState == eTargetLoopState.Before && m_AnimState.time > m_TargetLoopTime)
		{
			m_AnimState.speed = 0;
			StartCoroutine( "WaitForTargetLoop");
		}
	}

	IEnumerator WaitForTargetLoop()
	{
		m_TargetLoopState = eTargetLoopState.TargetLoop;

		yield return new WaitForSeconds( m_TargetLoopDuration);

		m_AnimState.speed = m_CurAnimSpeed;
		m_TargetLoopState = eTargetLoopState.After;
	}

	#region - legend -
	void EventProcess()
	{
		if( m_listEvent.Count == 0)
			return;

		AnimationEventInfo tempInfo = null;
		foreach( AnimationEventInfo info in m_listEvent)
		{
			if( m_AnimState.time > info.timing_)
			{
				AnimEvent_Base __event = null;

				switch( info.event_)
				{
				case eAnimEvent.Eye_Blink:
					__event = new AnimEvent_EyeBlink( info, m_AnimState.speed);
					break;
				}

				foreach( AnimEventReceiver_Base receiver in m_listReceiver)
				{
					receiver.ReceiveEvent( __event);
				}

				tempInfo = info;
				break;
			}
		}

		if( tempInfo != null)
			m_listEvent.Remove( tempInfo);
	}
	#endregion
	#endregion

	#region - msg -
	void OnModelLoaded( AsIMessage _msg)
	{
		if( Entity.ModelObject != null)
		{
			bool moving = false;
			bool combat = false;

			m_AnimComponent = Entity.ModelObject.animation;
			if( m_AnimComponent == null) m_AnimComponent = Entity.ModelObject.transform.GetChild( 0).animation;
			if( m_AnimComponent == null) AsUtil.ShutDown( "AsAnimation::OnModelLoaded: animation component is not attached on <" + gameObject.name + ">");
			
			if( false == TargetDecider.CheckCurrentMapIsArena())
				m_AnimComponent.cullingType = AnimationCullingType.BasedOnRenderers;
			
			m_Entity.SetModelObjectAnimation( m_AnimComponent);

			if( m_Entity.ContainProperty( eComponentProperty.MOVING) == true)
				moving = m_Entity.GetProperty<bool>( eComponentProperty.MOVING);

			if( m_Entity.ContainProperty( eComponentProperty.COMBAT) == true)
				combat = m_Entity.GetProperty<bool>( eComponentProperty.COMBAT);

			if( m_Entity.EntityType == eEntityType.USER)
			{
				eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
				eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);

				#region - event asset -
				if( m_EventAsset == null)
				{
					m_EventAsset = Resources.Load( "UseScript/AnimationEventAsset") as AnimationEventAsset;
					m_EventAsset.SetEventDictionary();
				}

				foreach( AnimEventReceiver_Base receiver in GetComponentsInChildren<AnimEventReceiver_Base>())
				{
					m_listReceiver.Add( receiver);
				}
				#endregion

				#region - set anim state -
				foreach( AnimationState state in m_AnimComponent)
				{
//					if(state.name == "BattleIdle2")
//						Debug.Log("BattleIdle2");
					
					Tbl_Action_Animation animInfo = AsTableManager.Instance.GetTbl_Action_Animation( __class, gender, state.name);
					if( animInfo != null)
						state.wrapMode = animInfo.wrapMode;
				}
				#endregion

				if( m_Entity.AnimEnableViaShop == false)
					return;

				#region - play cur state anim -
				switch( moving)
				{
				case true:
					switch( combat)
					{
					case true:
						m_PlayingAnimation = "BattleRun";
						break;
					case false:
						m_PlayingAnimation = "Run";
						break;
					}
					break;
				case false:
					switch( combat)
					{
					case true:
						m_PlayingAnimation = "BattleIdle";
						break;
					case false:
						m_PlayingAnimation = "Idle";
						break;
					}
					break;
				}

				bool living = m_Entity.GetProperty<bool>( eComponentProperty.LIVING);
				if( false == living)
					m_PlayingAnimation = "Death";

				m_AnimComponent.CrossFade( m_PlayingAnimation, 0.1f);
				#endregion
			}
			else if( m_Entity.FsmType == eFsmType.MONSTER)
			{
				#region - set anim state -
				string strClass = m_Entity.GetProperty<string>( eComponentProperty.CLASS);
				foreach( AnimationState state in m_AnimComponent)
				{
					Tbl_Action_Animation animInfo = AsTableManager.Instance.GetTbl_MonsterAction_Animation( strClass, state.name);
					if( animInfo != null)
						state.wrapMode = animInfo.wrapMode;
				}

				if( m_AnimComponent.GetClip( "Idle") == null)
				{
					AnimationClip clip = m_AnimComponent.GetClip( "BattleIdle");
					if( clip != null)
						m_AnimComponent.AddClip( m_AnimComponent.GetClip( "BattleIdle"), "Idle");
				}
				if( m_AnimComponent.GetClip( "Walk") == null)
				{
					AnimationClip clip = m_AnimComponent.GetClip( "Run");
					if( clip != null)
						m_AnimComponent.AddClip( m_AnimComponent.GetClip( "Run"), "Walk");
				}
				#endregion

				#region - play cur state anim -
				switch( moving)
				{
				case true:
					switch( combat)
					{
					case true:
						m_PlayingAnimation = "Run";
						break;
					case false:
						m_PlayingAnimation = "Walk";
						break;
					}
					break;
				case false:
					switch( combat)
					{
					case true:
						m_PlayingAnimation = "BattleIdle";
						break;
					case false:
						m_PlayingAnimation = "Idle";
						break;
					}
					break;
				}

				m_AnimComponent.CrossFade( m_PlayingAnimation, 0.1f);
				#endregion
			}
			else if( m_Entity.FsmType == eFsmType.NPC)
			{
				#region - set anim state -
				foreach( AnimationState state in m_AnimComponent)
				{
					AnimationTypeInfo info = AnimationTypeDefine.Instance.GetNpcAnimInfo( state.name);
					if( info != null)
					{
						state.layer = info.layer_;
						state.wrapMode = info.wrap_;
					}
				}
				#endregion

				#region - play cur state anim -
				AnimationClip clip = m_AnimComponent.GetClip( "Idle");
				if( clip != null)
					m_AnimComponent.CrossFade( "Idle", 0.1f);
				#endregion
			}
			else if( m_Entity.FsmType == eFsmType.PET)
			{
				#region - set anim state -
				string strClass = m_Entity.GetProperty<string>( eComponentProperty.CLASS);
				foreach( AnimationState state in m_AnimComponent)
				{
					Tbl_Action_Animation animInfo = AsTableManager.Instance.GetPetActionAnimation( strClass, state.name);
					if( animInfo != null)
						state.wrapMode = animInfo.wrapMode;
				}
				#endregion

				#region - play cur state anim -
				AnimationClip clip = m_AnimComponent.GetClip( "Idle");
				if( clip != null)
					m_AnimComponent.CrossFade( "Idle", 0.1f);
				#endregion
			}
		}
		else
			Debug.LogError( "AsAnimation::OnAnimation: MODEL OBJECT is not initialized correctly");
	}

	void OnAnimation( AsIMessage _msg)
	{
		if( null == _msg)
		{
			Debug.LogError( "AsAnimation::OnAnimation()[ null == _msg ] ");
			return;
		}

		Msg_AnimationIndicate msg = _msg as Msg_AnimationIndicate;

		if( null == msg)
		{
			Debug.LogError( "AsAnimation::OnAnimation()[ null == msg ]");
			return;
		}

		#region - block at specific state -
		if( m_Entity.AnimEnableViaShop == false)
			return;

		if( m_PlayingAnimation != "Idle" && msg.animString_ == "IdleAction")
		{
			Debug.LogWarning( "AsAnimation::OnAnimation: animation[" + msg.animString_ + "] is ignored by playing none Idle animation");
			return;
		}

		if( m_AnimComponent != null)
		{
			if( m_Entity.FsmType == eFsmType.MONSTER)
			{
				switch( msg.animString_)
				{
				case "BattleIdle":
					if( m_AnimComponent.GetClip( "BattleIdle") == null)
						msg.animString_ = "Idle";
					break;
				case "IdleAction":
					if( m_AnimComponent.GetClip( "IdleAction") == null)
						return;
					break;
				case "Walk":
				case "Run":
				case "Stun":
					if( m_AnimComponent.GetClip( msg.animString_) == null)
						return;
					break;
				}
				
				if( msg.animString_.Contains("Skill") == true)
					if( m_AnimComponent.GetClip( msg.animString_) == null)
						return;
			}
		}
		#endregion

		if( Entity.ModelObject != null && m_AnimComponent != null)
		{
			m_AnimEnded = false;
			m_TargetLoopState = eTargetLoopState.NONE;
			StopCoroutine( "WaitForTargetLoop");

			try
			{
				m_AnimState = m_AnimComponent[msg.animString_];
			}
			catch
			{
				AsUtil.ShutDown( "AsAnimation::OnAnimation: <" + gameObject.name + "> Cannot find [ " + msg.animString_ + " ] clip");

				StartCoroutine( "ErrorRecovery");
				return;
			}

			if( m_AnimState == null)
			{
				Debug.LogWarning( "AsAnimation::OnAnimation: animation is not found = " + msg.animString_);
				return;
			}

			WrapMode wrapMode = msg.GetWrapMode();
			if( wrapMode != WrapMode.Default) m_AnimState.wrapMode = wrapMode;

			if( m_PlayingAnimation != msg.animString_)
			{
				m_AnimState.time = 0;
				m_PlayingAnimation = msg.animString_;
				m_AnimComponent.CrossFade( m_PlayingAnimation, m_NextFadeTime);
			}
			else
			{
				if(m_AnimState.wrapMode == WrapMode.Loop)
				{
				}
				else if(msg.CheckAnimationLoop() == false)
				{
					m_AnimState.time = 0;
					m_AnimComponent.CrossFade( m_PlayingAnimation, m_NextFadeTime);
				}
			}

			if( msg.clampEnd_ == true)
				m_AnimState.time = m_AnimState.length;

			m_AnimState.speed = m_CurAnimSpeed = msg.animSpeed_;

			if( msg.animSpeed_ != 0f)
				m_PlayTime = m_AnimState.length / msg.animSpeed_;// - msg.fadeTime_;
			else
				m_PlayTime = float.MaxValue;

			m_NextFadeTime = msg.fadeTime_;

			#region - target loop -
			if( msg.targetTime_ != float.MaxValue)
			{
				m_TargetLoopState = eTargetLoopState.Before;
				m_PlayTime += msg.targetDuration_ * 0.001f;

				if( msg.animSpeed_ != 0f)
				{
					m_TargetLoopTime = ( ( msg.targetTime_/* + ( msg.fadeTime_ * 500f)*/) / msg.animSpeed_ * 0.001f);
					m_TargetLoopDuration = msg.targetDuration_ / msg.animSpeed_ * 0.001f;
				}
				else
				{
					m_TargetLoopTime = float.MaxValue;
					m_TargetLoopDuration = float.MaxValue;
				}
			}
			#endregion

			#region - event -
			if( m_Entity.EntityType == eEntityType.USER)
			{
				eCLASS __class = m_Entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
				eGENDER gender = m_Entity.GetProperty<eGENDER>( eComponentProperty.GENDER);

				m_listEvent.Clear();
				List<AnimationEventInfo> infos = m_EventAsset.GetEventList( __class, gender, m_PlayingAnimation);
				if( infos != null)
				{
					foreach( AnimationEventInfo info in infos)
					{
						m_listEvent.Add( info);
					}
				}
			}
			#endregion
		}
		else
		{
			m_AnimEnded = true;
			m_Entity.HandleMessage( new Msg_AnimationEnd( msg.animString_));
		}
	}

	IEnumerator ErrorRecovery()
	{
		yield return new WaitForSeconds( m_ErrorRecoveryTime);

		m_AnimEnded = true;
		m_Entity.HandleMessage( new Msg_RecoverState());
	}

	void OnAnimClipReceiver( AsIMessage _msg)
	{
		Msg_AnimationClipReceiver recv = _msg as Msg_AnimationClipReceiver;

		recv.clipFunc_( m_AnimComponent[recv.animName_].clip);
	}

	void OnFadeTime( AsIMessage _msg)
	{
		Msg_FadeTimeIndicate fade = _msg as Msg_FadeTimeIndicate;

		m_NextFadeTime = fade.fadeTime_;
		m_PlayTime -= fade.fadeTime_;
	}

	void OnChangeMoveSpeed( AsIMessage _msg)
	{
		Msg_MoveSpeedRefresh refresh = _msg as Msg_MoveSpeedRefresh;

		if( m_PlayingAnimation == "Walk" || m_PlayingAnimation == "Run" || m_PlayingAnimation == "BattleRun")
		{
			m_AnimState.speed = refresh.moveSpeed_ / AsProperty.s_BaseMoveSpeed;
			m_CurAnimSpeed = refresh.moveSpeed_ / AsProperty.s_BaseMoveSpeed;
		}
	}
	#endregion

	#region - delegate -
	float GetAnimationTimeDelegate()
	{
		if( m_AnimState != null)
			return m_AnimState.time;
		else
		{
			Debug.LogWarning( "AsAnimation::GetAnimationTimeDelegate: m_AnimState = null");
			return 0;
		}
	}
	#endregion

	#region - test -
//	void OnGUI()
//	{
//		if( GUI.Button( new Rect( 50, 50, 100, 30), "idle") == true)
//		{
//			m_AnimComponent.CrossFade( "Idle", 0.3f);
//		}
//		if( GUI.Button( new Rect( 50, 80, 100, 30), "walk") == true)
//		{
//			m_AnimComponent.CrossFade( "Walk", 0.3f);
//		}
//		if( GUI.Button( new Rect( 50, 110, 100, 30), "attack") == true)
//		{
//			m_AnimComponent.CrossFade( "Attack01", 0.3f);
//		}
//	}
	#endregion
}
