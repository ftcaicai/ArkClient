using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eComponentProperty
{
	NONE = 0,

	NAME,
	GUILD_NAME,
	RESOURCE_ID,

	MOVING,
	MOVE_TYPE,
	ATTACKING,

	RACE, GRADE, CLASS, GENDER, ATTRIBUTE,
	LEVEL, EVOLUTION_LEVEL, TOTAL_EXP, ATTACK_TYPE,
	//npc
	NPC_ID, NPC_TYPE,
	//pet
	PET_ID,
	//normal
	FACE_FILE_NAME, NPC_ACT, NPC_LIVING,
	//monster
//	MONSTER_TYPE,
//	MONSTER_SIZE,
	LIVING, COMBAT, CONDITION,
	SHOP_OPENING,

	MONSTER_ID,
	MONSTER_ATTACK_TYPE,
	MONSTER_ATTACK_STYLE,
	MONSTER_KIND_ID,
	VIEW_DISTANCE, CHASE_DISTANCE, VIEW_ANGLE, ATTACK_COOLTIME,
	DROP_EXP, DROP_ITEM, VIEW_HOLD,

	HP_CUR, HP_MAX,
	MP_CUR, MP_MAX,
	ATTACK, DEFENCE, CRITICAL_CHANCE,

	SKILL_1, SKILL_2, SKILL_3, SKILL_4, SKILL_5,

	MOVE_SPEED, ATTACK_DISTANCE, ATTACK_SPEED,

	GROUP_INDEX, LINK_INDEX,
	OBJ_TYPE, COLLECTOR_IDX, COLLECTOR_TECHNIC_TYPE,

	//slot in selection
	EMPTY_SLOT
}

public class AsProperty : ICloneable
{
	public static readonly float s_BaseMoveSpeedRatio = 0.01f;
	public static readonly float s_BaseAttackSpeedRatio = 0.001f;
	public static readonly float s_BaseMoveSpeed = 5;

	eComponentProperty id_;

	System.Object defaultValue_;
	System.Object currentValue_;
	Type type_;

	public object Clone()
	{
		return this.MemberwiseClone();
	}

	public AsProperty( string id, string type)
	{
		id_ = GetPropertyEnum( id);
		type_ = GetTypeDefinition( type);
	}

	~AsProperty()
	{
		defaultValue_ = null;
		currentValue_ = null;
	}

	public eComponentProperty ID	{ get { return id_; } }
	public eComponentProperty Name	{ get { return id_; } }

	public Type GetTypeInfo()
	{
		return type_;
	}

	public void SetDefaultValue( System.Object v)
	{
		if( GetTypeInfo() != v.GetType())
		{
			Debug.LogError( "[AsProperty]SetDefaultValue:Invalid value");
			return;
		}
		defaultValue_ = v;
		currentValue_ = v;
	}

	public System.Object GetDefaultValue()
	{
		return ( ValueType)defaultValue_;
	}

	public void SetValue( System.Object valueType)
	{
		if( currentValue_ == null)
		{
			Debug.LogError( "[AsProperty]SetDefaultValue:Invalid value");
			return;
		}

		if( !currentValue_.Equals( valueType))
			currentValue_ = valueType;
	}

	public System.Object GetValue()
	{
		return currentValue_;
	}

	static Vector3 FromStringToVector3( string str)
	{
		Vector3 pos;
		string [] split = str.Split( new Char [] {' '});
		float.TryParse( split[0], out pos.x);
		float.TryParse( split[1], out pos.y);
		float.TryParse( split[2], out pos.z);
		return pos;
	}

	public void SetDefaultValueAsString( string str)
	{
		SetPropertyFromString( type_, str, ref defaultValue_);
		SetPropertyFromString( type_, str, ref currentValue_);
	}

	public string GetDefaultValueAsString()
	{
		if( defaultValue_ == null)
		{
			Debug.LogError( "[AsProperty]AsProperty: no default string");
			return null;
		}

		return defaultValue_.ToString();
	}

	public void SetValueAsString( string str)
	{
	}

	public string GetValueAsString()
	{
		if( currentValue_ == null)
			return null;
		else
			return currentValue_.ToString();
	}

	#region - type translation -
	static Type GetTypeDefinition( string _type)
	{
		Type type;

		switch( _type)
		{
		case "ushort": type = typeof( ushort);	break;
		case "uint": type = typeof( uint);	break;
		case "ulong": type = typeof( ulong);	break;
		case "int": type = typeof( int);	break;
		case "float": type = typeof( float);	break;
		case "bool": type = typeof( bool);	break;
		case "Vector3": type = typeof( Vector3);	break;
		case "string": type = typeof( string);	break;
		//enum
		case "eGENDER": type = typeof( eGENDER);	break;
		case "eCLASS": type = typeof( eCLASS);	break;
		case "eRACE": type = typeof( eRACE);	break;
		case "eATTACK_TYPE": type = typeof( eATTACK_TYPE);	break;
		case "eMoveType": type = typeof( eMoveType);	break;
		case "eMonster_Grade": type = typeof( eMonster_Grade);	break;
		case "eCHATTYPE": type = typeof( eCHATTYPE); break;
		case "ePotency_Enable_Condition": type = typeof( ePotency_Enable_Condition);	break;
		case "eLivingType": type = typeof( eLivingType);	break;
		case "eOSTYPE":	type = typeof( eOSTYPE);	break;
		case "eCONDITION_TYPE":	type = typeof( eCONDITION_TYPE);	break;
		case "eEVENT_TYPE":	type = typeof( eEVENT_TYPE);	break;
		case "ePOST_ADDRESS_BOOK_TYPE":	type = typeof( ePOST_ADDRESS_BOOK_TYPE);	break;
		default:
			type = null;
			Debug.LogError( "[AsProperty]AsProperty: Invalid type[" + type + "]");
			break;
		}

		return type;
	}

	static void SetPropertyFromString( Type _type, string str, ref System.Object data)
	{
		switch( _type.ToString())
		{
		case "System.UInt16" : data = ushort.Parse( str);	break;
		case "System.UInt32" : data = uint.Parse( str);	break;
		case "System.UInt64" : data = ulong.Parse( str);	break;
		case "System.Int32": data = int.Parse( str);	break;
		case "System.Single": data = float.Parse( str);	break;
		case "System.Boolean": data = bool.Parse( str);	break;
		case "UnityEngine.Vector3": data = FromStringToVector3( str);	break;
		case "System.String": data = str;	break;
		//enum
		case "eGENDER": data = ( eGENDER)Enum.Parse( typeof( eGENDER), str, true);	break;
		case "eCLASS": data = ( eCLASS)Enum.Parse( typeof( eCLASS), str, true);	break;
		case "eRACE": data = ( eRACE)Enum.Parse( typeof( eRACE), str, true);	break;
		case "eATTACK_TYPE": data = ( eATTACK_TYPE)Enum.Parse( typeof( eATTACK_TYPE), str, true);	break;
		case "eMoveType": data = ( eMoveType)Enum.Parse( typeof( eMoveType), str, true);	break;
		case "eMonster_Grade": data = ( eMonster_Grade)Enum.Parse( typeof( eMonster_Grade), str, true);	break;
		case "eCHATTYPE": data = ( eCHATTYPE)Enum.Parse( typeof( eCHATTYPE), str, true);	break;
		case "ePotency_Enable_Condition": data = ( ePotency_Enable_Condition)Enum.Parse( typeof( ePotency_Enable_Condition), str, true);	break;
		case "eLivingType": data = ( eLivingType)Enum.Parse( typeof( eLivingType), str, true);	break;
		case "eOSTYPE":	data = ( eOSTYPE)Enum.Parse( typeof( eOSTYPE), str, true);	break;
		case "eCONDITION_TYPE":	data = ( eCONDITION_TYPE)Enum.Parse( typeof( eCONDITION_TYPE), str, true);	break;
		case "eEVENT_TYPE":	data = ( eEVENT_TYPE)Enum.Parse( typeof( eEVENT_TYPE), str, true);	break;
		case "ePOST_ADDRESS_BOOK_TYPE":	data = ( ePOST_ADDRESS_BOOK_TYPE)Enum.Parse( typeof( ePOST_ADDRESS_BOOK_TYPE), str, true);	break;
		default:
			Debug.LogError( "[AsProperty]AsProperty: Invalid type");
			break;
		}
	}
	#endregion

	#region - change type between eneum and string( properties will be added) -
	public static eComponentProperty GetPropertyEnum( string _type)
	{
		eComponentProperty eProperty;

		try
		{
			eProperty = ( eComponentProperty)System.Enum.Parse( typeof( eComponentProperty), _type, true);
		}
		catch
		{
			Debug.LogError( "[AsProperty]GetPropertyEnum: invalid property name");
			eProperty = eComponentProperty.NONE;
		}

		return eProperty;
	}

	public static string GetPropertyString( eComponentProperty _type)
	{
		return _type.ToString();
	}
	#endregion
}