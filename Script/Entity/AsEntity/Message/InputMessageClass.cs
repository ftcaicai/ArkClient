using UnityEngine;
using System.Collections;

public class Msg_Input
{
	#region - member -
	public eInputType type_;
	public GameObject inputObject_;
	public Vector3 screenPosition_;
	public Vector3 deltaPosition_;
	public Vector3 worldPosition_;
	public float deltaTime_;
	#endregion

	#region - init -
	public Msg_Input( eInputType _type, GameObject _inputObject, Vector3 _screenPosition,
		Vector3 _deltaPosition, Vector3 _worldPosition, float _deltaTime)
	{
		type_ = _type;
		inputObject_ = _inputObject;
		screenPosition_ = _screenPosition;
		deltaPosition_ = _deltaPosition;
		worldPosition_ = _worldPosition;
		deltaTime_ = _deltaTime;
	}
	#endregion
}

public class Msg_Input_Move : AsIMessage
{
	public Vector3 worldPos_;

	public Msg_Input_Move( Vector3 _pos)
	{
		m_MessageType = eMessageType.INPUT_MOVE;

		worldPos_ = _pos;
	}
}

public class Msg_Input_Auto_Move : AsIMessage
{
	public Vector3 worldPos_;

	public Msg_Input_Auto_Move( Vector3 _pos)
	{
		m_MessageType = eMessageType.INPUT_AUTO_MOVE;

		worldPos_ = _pos;
	}
}

public class Msg_Input_Targeting : AsIMessage
{
	public AsBaseEntity target_;

	public Msg_Input_Targeting( AsBaseEntity _entity)
	{
		m_MessageType = eMessageType.INPUT_TARGETING;

		target_ = _entity;
	}
}

public class Msg_Input_Attack : AsIMessage
{
	public AsNpcEntity enemy_;

	public Msg_Input_Attack( AsNpcEntity _enemy)
	{
		m_MessageType = eMessageType.INPUT_ATTACK;

		enemy_ = _enemy;
	}
}

public class Msg_Input_Begin_Charge : AsIMessage
{
	public int skillIdx_;

	public Msg_Input_Begin_Charge( int _skillIdx)
	{
		m_MessageType = eMessageType.INPUT_BEGIN_CHARGE;

		skillIdx_ = _skillIdx;
	}
}

public class Msg_Input_Cancel_Charge : AsIMessage
{
	public Msg_Input_Cancel_Charge()
	{
		m_MessageType = eMessageType.INPUT_CANCEL_CHARGE;
	}
}

public class Msg_Input_Slot_Active : AsIMessage
{
	public int skillIdx_;
	public int step_;

	public Msg_Input_Slot_Active( int _skillIdx, int _step)
	{
		m_MessageType = eMessageType.INPUT_SLOT_ACTIVE;

		skillIdx_ = _skillIdx;
		step_ = _step;
	}
}

public class Msg_Input_DragStraight : AsIMessage
{
	public Vector3 head_;
	public Vector3 tail_;
	public Vector3 center_;
	public Vector3 direction_;

	public Msg_Input_DragStraight( Vector3 _head, Vector3 _center, Vector3 _tail, Vector3 _direction)
	{
		m_MessageType = eMessageType.INPUT_DRAG_STRAIGHT;

		head_ = _head;
		tail_ = _tail;
		center_ = _center;

		direction_ = _direction;
	}
}

public class Msg_Input_Arc : AsIMessage
{
	public Vector3 head_;
	public Vector3 center_;
	public Vector3 tail_;
	public Vector3 direction_;

	public eClockWise cw_;

	public Msg_Input_Arc( Vector3 _head, Vector3 _center, Vector3 _tail, Vector3 _direction, eClockWise _cw)
	{
		m_MessageType = eMessageType.INPUT_ARC;

		head_ = _head;
		tail_ = _tail;
		center_ = _center;

		direction_ = _direction;

		cw_ = _cw;
	}
}

public class Msg_Input_Circle : AsIMessage
{
	public Vector3 head_;
	public Vector3 center_;
	public Vector3 tail_;
	public Vector3 direction_;
	public eClockWise cw_;

	public Msg_Input_Circle( Vector3 _head, Vector3 _tail, Vector3 _center, Vector3 _direction, eClockWise _cw)
	{
		m_MessageType = eMessageType.INPUT_CIRCLE;

		head_ = _head;
		tail_ = _tail;
		center_ = _center;

		direction_ = _direction;

		cw_ = _cw;
	}
}

#region - double tap -
public class Msg_Input_DoubleTab : AsIMessage
{
	public enum eDoubleTabType {Terrain, Player, OtherUser, Monster}
	public eDoubleTabType type_;
	public Msg_Input input_;

	public Msg_Input_DoubleTab( Msg_Input _input, eDoubleTabType _type)
	{
		m_MessageType = eMessageType.INPUT_DOUBLE_TAB;

		type_ = _type;
		input_ = _input;
	}
}
#endregion

public class Msg_Input_Dash : AsIMessage
{
	public Vector3 direction_;

	public Msg_Input_Dash( Vector3 _direction)
	{
		m_MessageType = eMessageType.INPUT_DASH;

		direction_ = _direction;
	}
}

public class Msg_Input_Rub : AsIMessage
{
	public Msg_Input_Rub()
	{
		m_MessageType = eMessageType.INPUT_RUB;
	}
}


public class Msg_Skill_Charge_Complete : AsIMessage
{
	public COMMAND_SKILL_TYPE type;

	public Msg_Skill_Charge_Complete( COMMAND_SKILL_TYPE type)
	{
		m_MessageType = eMessageType.SKILL_CHARGE_COMPLETE;
		this.type = type;
	}
}

public class Msg_Skill_Shield_Start : AsIMessage
{
	public Msg_Skill_Shield_Start()
	{
		m_MessageType = eMessageType.INPUT_SHIELD_BEGIN;
	}
}

public class Msg_Skill_Shield_End : AsIMessage
{
	public Msg_Skill_Shield_End()
	{
		m_MessageType = eMessageType.INPUT_SHIELD_END;
	}
}

public class  Msg_Resurrection : AsIMessage
{
	public uint reviver_;

	public Msg_Resurrection()
	{
		m_MessageType = eMessageType.CHAR_RESURRECTION;
	}

	public Msg_Resurrection( uint _charUniqKey)
	{
		m_MessageType = eMessageType.CHAR_RESURRECTION;

		reviver_ = _charUniqKey;
	}
}

// CHEAT
public class Msg_Cheat_Death : AsIMessage
{
	public Msg_Cheat_Death()
	{
		m_MessageType = eMessageType.CHEAT_DEATH;
	}
}

// casting
public class Msg_Mob_Cast : AsIMessage
{
	public Msg_NpcAttackChar1 castInfo;

	public Msg_Mob_Cast( Msg_NpcAttackChar1 info)
	{
		m_MessageType = eMessageType.MOB_CASTING;
		castInfo = info;
	}
}

public class Msg_PlayerClick : AsIMessage
{
	public Msg_PlayerClick()
	{
		m_MessageType = eMessageType.PLAYER_CLICK;
	}
}

//npc
public class Msg_NpcClick : AsIMessage
{
	public int idx_;

	public Msg_NpcClick( int _idx)
	{
		m_MessageType = eMessageType.NPC_CLICK;

		idx_ = _idx;
	}
}

public class Msg_StartCollect : AsIMessage
{
	public AsPlayerFsm playerFsm;

	public Msg_StartCollect( AsPlayerFsm _playerFsm)
	{
		m_MessageType = eMessageType.COLLECT_START;

		playerFsm = _playerFsm;
	}
}

public class Msg_CollectResult : AsIMessage
{
	public eCOLLECT_STATE eCollectState;

	public Msg_CollectResult( eCOLLECT_STATE _collectState)
	{
		m_MessageType = eMessageType.COLLECT_RESULT;

		eCollectState = _collectState;
	}
}

public class Msg_CollectInfo : AsIMessage
{
	public body_SC_COLLECT_INFO collectInfo;

	public Msg_CollectInfo( body_SC_COLLECT_INFO _collectInfo)
	{
		m_MessageType = eMessageType.COLLECT_INFO;

		collectInfo = _collectInfo;
	}
}

public class Msg_CollectClick :AsIMessage
{
	public int idx_;

	public Msg_CollectClick( int _idx)
	{
		m_MessageType = eMessageType.COLLECT_CLICK;

		idx_ = _idx;
	}
}

public class Msg_ObjectClick : AsIMessage
{
	public Msg_ObjectClick()
	{
		m_MessageType = eMessageType.OBJECT_CLICK;
	}
}

// Other User
public class Msg_OtherUserClick : AsIMessage
{
	public uint idx_;

	public Msg_OtherUserClick( uint _idx)
	{
		m_MessageType = eMessageType.OTHER_USER_CLICK;

		idx_ = _idx;
	}
}

public class Msg_PRODUCT : AsIMessage
{
	public bool m_isProduct = false;

	public Msg_PRODUCT( bool _isProduct)
	{
		m_MessageType = eMessageType.PRODUCT;
		m_isProduct = _isProduct;
	}
}

