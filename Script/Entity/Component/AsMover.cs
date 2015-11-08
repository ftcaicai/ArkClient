using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsMover : AsBaseComponent
{
	#region - member -
	private CharacterController m_kController = null;
	[SerializeField] float m_MoveSpeed = 3.0f;
	private AsCharMover m_charMover = null;
	GameObject m_Destination;
	bool m_MoveRecognition = false;

	[SerializeField] eMoveType m_MoveType;
	Vector3 m_Direction;
	Vector3 m_vec3TargetPos;
	bool m_IsJump = false;
	float m_fMaxReadyTime = 0.23f;
	float m_fReadyTime = 2.0f;

	Vector3[] m_WarpPath;

	float forcedMovementSpeed = 20f;

	float m_RemainDistance = 0;
	#endregion

	#region - init -
	void Awake()
	{
		m_ComponentType = eComponentType.MOVER;

		MsgRegistry.RegisterFunction( eMessageType.MODEL_LOADED, OnModelLoaded);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_INFO, OnMoveInfo);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_AUTO, OnMoveAuto);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_STOP_INDICATION, OnMoveStopIndication);
		MsgRegistry.RegisterFunction( eMessageType.DASH_INDICATION, OnDash);
		MsgRegistry.RegisterFunction( eMessageType.DASHBACK_INDICATION, OnDashBack);
		MsgRegistry.RegisterFunction( eMessageType.WARP_INDICATION, OnWarp);
		MsgRegistry.RegisterFunction( eMessageType.FORCEDMOVE_INDICATION, OnForcedMove);
		MsgRegistry.RegisterFunction( eMessageType.JUMP_MSG, OnJump);
		MsgRegistry.RegisterFunction( eMessageType.MOVE_SPEED_REFRESH, OnChangeMoveSpeed);
		MsgRegistry.RegisterFunction( eMessageType.FORCED_MOVEMENT_SEARCH, OnForcedMovementSearch);
		MsgRegistry.RegisterFunction( eMessageType.MODEL_CHANGE, OnModelChange);

		m_charMover = new AsCharMover();
	}

	public override void Init( AsBaseEntity _entity)
	{
		base.Init( _entity);

		_entity.SetNavPathDistance( GetNavPathDistance);

		if( m_kController == null)
		{
			CharacterController attached = gameObject.AddComponent<CharacterController>();
			if( attached == null)
				Debug.LogError( "AsMover::InterInit: attaching controller failed");
			else
			{
				m_kController = attached;

				// default setting, ilmeda
//				if( eEntityType.USER == m_Entity.EntityType)
//				{
					m_kController.center = new Vector3( 0.0f, 1.0f, 0.0f);
					m_kController.height = 2.0f;
//				}
			}
		}

		m_Entity.SetCharacterController( m_kController);
		m_kController.slopeLimit = 70f;
	}

	public override void InterInit( AsBaseEntity _entity)
	{
//		if( m_kController == null)
//		{
//			CharacterController attached = gameObject.AddComponent<CharacterController>();
//			if( attached == null)
//				Debug.LogError( "AsMover::InterInit: attaching controller failed");
//			else
//			{
//				m_kController = attached;
//
//				// default setting, ilmeda
//				if( eEntityType.USER == m_Entity.EntityType)
//				{
//					m_kController.center = new Vector3( 0.0f, 1.0f, 0.0f);
//					m_kController.height = 2.0f;
//				}
//			}
//		}
//
//		m_Entity.SetCharacterController( m_kController);
	}

	void Start()
	{
	}
	
	void OnDestroy()
	{
		Destroy( m_kController);
	}
	#endregion
	#region - update -
	void Update ()
	{
		if( m_MoveRecognition == true)
		{
			if( null != m_charMover.GetPath())
			{
				switch( m_MoveType)
				{
				case eMoveType.Normal:
				case eMoveType.Combat:
					MoveEntity();
					break;
				case eMoveType.Back:
					MoveBackEntity();
					break;
				case eMoveType.Auto:
					AutoMove();
					break;
				case eMoveType.Dash:
					DashMove();
					break;
				case eMoveType.Warp:
					WarpMove();
					break;
				case eMoveType.Jump:
					JumpMove();
					break;
				case eMoveType.ForcedMove:
					MoveEntity();
					break;
				}
			}
		}
	}
	#endregion

	#region - normal move -
	void MoveEntity()
	{
		m_charMover.Update( transform.position, m_MoveSpeed * Time.deltaTime);
		if( true == m_charMover.IsMoving())
		{
			Vector3 vec3Temp = m_charMover.GetMovement();
			vec3Temp.y = -1;
			m_kController.Move( vec3Temp);

			if( m_MoveType != eMoveType.ForcedMove)
			{
				transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( vec3Temp), 20.0f * Time.deltaTime);
				Vector3 rot = transform.rotation.eulerAngles;
				rot.x = 0;
				rot.z = 0;
				transform.rotation = Quaternion.Euler( rot);
			}

			if( m_MoveType == eMoveType.Dash)
			{
				m_RemainDistance -= m_MoveSpeed * Time.deltaTime;

				if( m_RemainDistance < 0)
					MoveStop();
			}
		}
		else
		{
			MoveStop();
		}
	}
	
	void MoveBackEntity()
	{
		m_charMover.Update( transform.position, m_MoveSpeed * Time.deltaTime);
		if( true == m_charMover.IsMoving())
		{
			Vector3 vec3Temp = m_charMover.GetMovement();
			vec3Temp.y = -1;
			m_kController.Move( vec3Temp);
			
			transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( -vec3Temp), 20.0f * Time.deltaTime);
			Vector3 rot = transform.rotation.eulerAngles;
			rot.x = 0;
			rot.z = 0;
			transform.rotation = Quaternion.Euler( rot);

			if( m_MoveType == eMoveType.Dash)
			{
				m_RemainDistance -= m_MoveSpeed * Time.deltaTime;

				if( m_RemainDistance < 0)
					MoveStop();
			}
		}
		else
		{
			MoveStop();
		}
	}

	void MoveStop()
	{
		m_MoveRecognition = false;
		m_Entity.SetProperty( eComponentProperty.MOVING, false);

		m_Entity.HandleMessage( new Msg_MoveEndInform());
	}
	#endregion

	#region - auto move -
	void AutoMove()
	{
		// being kij
		/*if( m_charMover.GetArrival( transform.position, 0.2f) == true)
		{
			m_MoveRecognition = false;
			m_Entity.SetProperty( eComponentProperty.MOVING, false);

			m_Entity.HandleMessage( new Msg_MoveEndInform());

			CancelInvoke( "RefreshAutoMoveDestination");
		}
		else
		{
			MoveEntity();
		}*/
		// end kij

		// begin kij
		m_charMover.Update( transform.position, m_MoveSpeed * Time.deltaTime);
		if( true == m_charMover.IsMoving())
		{
			Vector3 vec3Temp = m_charMover.GetMovement();
			vec3Temp.y = -1;
			m_kController.Move( vec3Temp);

			if( m_MoveType != eMoveType.ForcedMove)
			{
				transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( vec3Temp), 5.0f * Time.deltaTime);
				Vector3 rot = transform.rotation.eulerAngles;
				rot.x = 0;
				rot.z = 0;
				transform.rotation = Quaternion.Euler( rot);
			}
		}
		else
		{
			Vector3[] _newPath = NavMeshFinder.Instance.PathFind( transform.position, transform.position + m_Direction);
			if( null != _newPath)
				m_charMover.SetPath( _newPath);
			else
				MoveStop();

			if( m_charMover.GetArrival( transform.position, 0.2f) == true)
				MoveStop();
		}
		//begin kij
	}
	#endregion

	#region - dash -
	void DashMove()
	{
		MoveEntity();
	}
	#endregion

	#region - warp -
	void WarpMove()
	{
		transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( m_Direction), 20.0f * Time.deltaTime);
		Vector3 rot = transform.rotation.eulerAngles;
		rot.x = 0;
		rot.z = 0;
		transform.rotation = Quaternion.Euler( rot);

		if( null != m_WarpPath && 0<m_WarpPath.Length)
		{
			Vector3 vec3Temp = m_WarpPath[m_WarpPath.Length - 1 ];// m_charMover.GetDestination() - transform.position;

			vec3Temp.y = TerrainMgr.GetTerrainHeight( m_kController, vec3Temp);
			transform.position = vec3Temp;
			//m_kController.Move( vec3Temp);
		}

		//OnMoveEnd
		m_MoveRecognition = false;
		m_Entity.SetProperty( eComponentProperty.MOVING, false);

		m_Entity.HandleMessage( new Msg_MoveEndInform());
	}
	#endregion

	#region - knockback -
	void KnockbackMove()
	{
	}
	#endregion

	//message
	#region - model loaded -
	void OnModelLoaded( AsIMessage _ms)
	{
		CharacterController control = m_Entity.ModelObject.GetComponent<CharacterController>();

		if( ( null == m_kController || null == control))
		{
			if( m_Entity.FsmType != eFsmType.COLLECTION)
			{
				Debug.LogError( "AsMover::LateInit: m_kController = " + m_kController);
				Debug.LogError( "AsMover::LateInit: control  = " + control);
				Debug.LogError( "AsMover::LateInit: no controller is attached on " + m_Entity.name + "(" + m_Entity.FsmType + ")");
			}
		}
		else
		{
			m_kController.center = control.center;
			m_kController.height = control.height;
			m_kController.radius = control.radius;

			DestroyImmediate( control);
		}

		m_Entity.SetRePosition( m_Entity.transform.position);
	}

	void OnModelChange( AsIMessage _msg)
	{
		if( m_kController == null)
		{
			CharacterController attached = gameObject.AddComponent<CharacterController>();
			if( attached == null)
				Debug.LogError( "AsMover::InterInit: attaching controller failed");
			else
				m_kController = attached;
		}

		m_Entity.SetCharacterController( m_kController);
	}
	#endregion

	#region - normal move -
	void OnMoveInfo( AsIMessage _msg)
	{
		Msg_MoveInfo msg = _msg as Msg_MoveInfo;

		m_MoveRecognition = true;
		m_MoveSpeed = m_Entity.GetProperty<float>( eComponentProperty.MOVE_SPEED);// / 100;
		m_Entity.SetProperty( eComponentProperty.MOVING, true);
		m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Normal);
		m_MoveType = eMoveType.Normal;

		if( this.Entity.EntityType == eEntityType.NPC)
		{
			Vector3[] path = new Vector3[1];
			path[0] = msg.targetPosition_;
			m_charMover.SetPath( path);
		}
		else
		{
			Vector3[] _newPath = NavMeshFinder.Instance.PathFind( transform.position, msg.targetPosition_);
			if( null != _newPath)
				m_charMover.SetPath( _newPath);

		}
		CancelInvoke( "RefreshAutoMoveDestination");
	}

	void JumpMove()
	{
		if( false == m_IsJump)
			return;

//		if( m_fMaxReadyTime > Time.realtimeSinceStartup - m_fReadyTime)
		if( m_fMaxReadyTime > Time.time - m_fReadyTime)
			return;

		Vector3 vec3Direction = ( m_vec3TargetPos - transform.position);
		float fSpeed = m_MoveSpeed * Time.deltaTime;
		Vector3 vec3Temp = vec3Direction;
		vec3Temp.y = 0.0f;
		if( fSpeed < vec3Temp.magnitude)
		{
			m_kController.Move( vec3Direction.normalized * fSpeed);
		}
		else
		{
			m_kController.Move( vec3Direction.normalized * vec3Direction.magnitude);
			m_Entity.HandleMessage( new Msg_JumpStop());
			m_IsJump = false;
			Debug.Log( "JumpMove [ end] ");
			return;
		}

		transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( vec3Direction), 20.0f * Time.deltaTime);
		Vector3 rot = transform.rotation.eulerAngles;
		rot.x = 0; rot.z = 0;
		transform.rotation = Quaternion.Euler( rot);
	}
	#endregion

	#region - auto move -
	void OnMoveAuto( AsIMessage _msg)
	{
		if( this.Entity.EntityType != eEntityType.USER)
		{
			Debug.LogError( "AsMover::OnMoveAuto: this entity is npc. invalid message delivery");
		}
		else
		{
			CancelInvoke( "RefreshAutoMoveDestination");

			Msg_AutoMove msg = _msg as Msg_AutoMove;

			m_MoveRecognition = true;

			m_MoveSpeed = m_Entity.GetProperty<float>( eComponentProperty.MOVE_SPEED);
			m_Entity.SetProperty( eComponentProperty.MOVING, true);
			m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Auto);
			m_MoveType = eMoveType.Auto;
			m_Direction = msg.targetPosition_ - msg.curPos_;
			m_Direction.y = 0;
			m_Direction = m_Direction.normalized * m_PreCalcultateRatio;

			Vector3[] _newPath = NavMeshFinder.Instance.PathFind( transform.position, transform.position + m_Direction);
			if( null != _newPath)
				m_charMover.SetPath( _newPath);

			InvokeRepeating( "RefreshAutoMoveDestination", 0, m_AutoMoveInterval);
		}
	}

	float m_PreCalcultateRatio = 7;
	float m_AutoMoveInterval = 0.4f;
	void RefreshAutoMoveDestination()
	{
		Vector3[] _newPath = NavMeshFinder.Instance.PathFind( transform.position, transform.position + m_Direction);
		if( null != _newPath)
			m_charMover.SetPath( _newPath);
	}
	#endregion

	#region - dash move -
	void OnDash( AsIMessage _msg)
	{
		Msg_DashIndication msg = _msg as Msg_DashIndication;

		m_MoveRecognition = true;

		m_Entity.SetProperty( eComponentProperty.MOVING, true);
		m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Dash);
		m_MoveType = eMoveType.Dash;

		m_RemainDistance = msg.distance_;

		Vector3[] _newPath = null;

		switch( m_Entity.EntityType)
		{
		case eEntityType.USER:
			m_Direction = msg.direction_.normalized * msg.distance_;
			m_MoveSpeed = msg.distance_ / msg.time_ * 1000f;
			_newPath = NavMeshFinder.Instance.PathFind_type1( transform.position, transform.position + m_Direction);
//			ShowPoint( transform.position + m_Direction);//$yde
//			ShowPath( _newPath);//$yde
			if( null != _newPath)
			{
				m_charMover.SetPath( _newPath);

//				float dist = GetNavPathDistance( transform.position, transform.position + m_Direction);
//				m_MoveSpeed = dist / msg.time_ * 1000f;
//				m_Direction = msg.direction_.normalized * msg.distance_;
			}
			break;
		case eEntityType.NPC:
			m_Direction = msg.direction_.normalized * msg.distance_;
			m_MoveSpeed = msg.distance_ / msg.time_ * 1000f;
			_newPath = NavMeshFinder.Instance.PathFind_type1( transform.position, transform.position + m_Direction);
			if( null != _newPath)
			{
				m_charMover.SetPath( _newPath);

//				m_MoveSpeed = msg.distance_ / msg.time_ * 1000f;
//				m_Direction = msg.direction_.normalized * msg.distance_;
			}
			break;
		}

		CancelInvoke( "RefreshAutoMoveDestination");
	}
	
	void OnDashBack( AsIMessage _msg)
	{
		Msg_DashBackIndication msg = _msg as Msg_DashBackIndication;

		m_MoveRecognition = true;

		m_Entity.SetProperty( eComponentProperty.MOVING, true);
		m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Back);
		m_MoveType = eMoveType.Back;

		m_RemainDistance = msg.distance_;

		Vector3[] _newPath = null;
		
		m_Direction = msg.direction_.normalized * msg.distance_;
		m_MoveSpeed = msg.distance_ / msg.time_ * 1000f;
		_newPath = NavMeshFinder.Instance.PathFind_type1( transform.position, transform.position + m_Direction);
		
		if( null != _newPath)
			m_charMover.SetPath( _newPath);

		CancelInvoke( "RefreshAutoMoveDestination");
	}

	void OnWarp( AsIMessage _msg)
	{
		if( this.Entity.EntityType != eEntityType.USER)
		{
			Debug.LogError( "AsMover::OnWarp: this entity is npc. invalid message delivery");
		}
		else
		{
			Msg_WarpIndication warp = _msg as Msg_WarpIndication;

			m_MoveRecognition = true;

			m_Entity.SetProperty( eComponentProperty.MOVING, true);
			m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Dash);
			m_MoveType = eMoveType.Warp;

			m_Direction = warp.direction_ * warp.distance_;
			m_Direction.y = 0;

			m_WarpPath = NavMeshFinder.Instance.PathFind( transform.position, transform.position + m_Direction);
			//if( null != m_WarpPath)
			//	m_charMover.SetPath( _newPath);
		}

		CancelInvoke( "RefreshAutoMoveDestination");
	}
	#endregion

	#region - knockback & haul -
	void OnKnockback( AsIMessage _msg)
	{
	}
	#endregion

	#region - end -
	void OnMoveStopIndication( AsIMessage _msg)
	{
		m_MoveRecognition = false;
		m_Entity.SetProperty( eComponentProperty.MOVING, false);

		m_Entity.HandleMessage( new Msg_MoveEndInform());
		CancelInvoke( "RefreshAutoMoveDestination");
	}
	#endregion

	#region - move speed change -
	void OnChangeMoveSpeed( AsIMessage _msg)
	{
		Msg_MoveSpeedRefresh refresh = _msg as Msg_MoveSpeedRefresh;

		m_Entity.SetProperty( eComponentProperty.MOVE_SPEED, refresh.moveSpeed_);
		m_MoveSpeed = refresh.moveSpeed_;
	}
	#endregion

	#region - forced movement search -
	void OnForcedMovementSearch( AsIMessage _msg)
	{
//		Msg_ForcedMovementSearch search = _msg as Msg_ForcedMovementSearch;
//		StartCoroutine( "CheckingSearch", search);
	}

	IEnumerator CheckingSearch( Msg_ForcedMovementSearch _search)
	{
		while( true)
		{
			yield return null;
		}
	}
	#endregion

	#region - forced move -
	void OnForcedMove( AsIMessage _msg)
	{
		Msg_ForcedMoveIndication msg = _msg as Msg_ForcedMoveIndication;

		m_MoveRecognition = true;

		m_Entity.SetProperty( eComponentProperty.MOVING, true);
		m_MoveType = eMoveType.ForcedMove;

		m_MoveSpeed = forcedMovementSpeed;

		Vector3[] path = NavMeshFinder.Instance.PathFind_type1( m_Entity.transform.position, msg.destination_);
//		path[0] = msg.destination_;
		m_charMover.SetPath( path);

		if( path.Length > 0)
			m_Entity.HandleMessage( new Msg_ForcedMove_Sync( path[path.Length - 1]));
	}
	#endregion

	#region - jump -
	void OnJump( AsIMessage _msg)
	{
		if ( eMessageType.JUMP_MSG == _msg.MessageType)
		{
			Msg_Jump msgJump = _msg as Msg_Jump;
			if ( null != msgJump)
			{
				Debug.Log( "OnJump");
				m_Entity.SetProperty( eComponentProperty.MOVING, true);
				m_Entity.SetProperty( eComponentProperty.MOVE_TYPE, eMoveType.Jump);

				m_MoveRecognition = true;

				m_MoveType = eMoveType.Jump;
				m_vec3TargetPos = msgJump.m_vec3TargetPos;

				msgJump.m_fJumpSpeed -= ( m_fMaxReadyTime + 0.23f);
				if( 0.0f == msgJump.m_fJumpSpeed)
					msgJump.m_fJumpSpeed = 1.0f;
				m_MoveSpeed = ( m_vec3TargetPos - transform.position).magnitude / msgJump.m_fJumpSpeed;
				m_IsJump = true;
//				m_fReadyTime = Time.realtimeSinceStartup;
				m_fReadyTime = Time.time;
			}
		}
	}
	#endregion

	#region - gizmos -
	void OnDrawGizmos()
	{
		if( null == m_charMover)
			return;

		if( null != m_charMover.GetPath())
		{
			float i=0;
			foreach( Vector3 data in m_charMover.GetPath())
			{
				Color temp = Color.green;
				temp.a += i;
				Gizmos.color = temp;
				Gizmos.DrawSphere( data, 0.2f);
				i += 0.1f;
			}
		}
	}
	#endregion

	#region - delegate -
	float GetNavPathDistance( Vector3 _pos1, Vector3 _pos2, bool _show)
	{
//		Vector3[] path = NavMeshFinder.Instance.PathFind( _pos1, _pos2);
		Vector3[] path = NavMeshFinder.Instance.PathFind_type1( _pos1, _pos2);

//		if( _show == true)
//			ShowPath( path);

		if( null == path)
			return float.MaxValue;

		float dist = 0;
		if( path.Length > 0)
			dist = Vector3.Distance( m_Entity.transform.position, path[0]);

		for( int i=0; i<path.Length - 1; ++i)
		{
			dist += Vector3.Distance( path[i], path[i+1]);
		}

		return dist;
	}

	void ShowPoint( Vector3 _pos)
	{
		Color color = Color.Lerp( Color.blue, Color.yellow, Random.Range( 0f, 1f));

		GameObject obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
		obj.transform.position = _pos;
		obj.renderer.material.color = color;

		List<GameObject> listObj = new List<GameObject>();
		listObj.Add( obj);

		StartCoroutine( PathLife( listObj));
	}

	void ShowPath( Vector3[] _path)
	{
		List<GameObject> listObj = new List<GameObject>();
		Color color = Color.Lerp( Color.blue, Color.yellow, Random.Range( 0f, 1f));
		foreach( Vector3 pos in _path)
		{
			GameObject obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
			obj.transform.position = pos;
			obj.renderer.material.color = color;
			listObj.Add( obj);
		}

		StartCoroutine( PathLife( listObj));
	}

	IEnumerator PathLife( List<GameObject> _listObj)
	{
		yield return new WaitForSeconds( 5);

		foreach( GameObject obj in _listObj)
		{
			Destroy( obj);
		}

		_listObj.Clear();
	}
	#endregion
}




