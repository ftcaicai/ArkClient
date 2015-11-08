#define AUTOMOVE_EFFECT
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum eEntityType{UNKNOWN, USER, NPC} //{UNKNOWN, PLAYER, NPC, ENEMY, ITEM}
public enum eFsmType{UNKNOWN, PLAYER, OTHER_USER, NPC, MONSTER, OBJECT, COLLECTION, PET }
public enum eSkillNameShoutType {Self, Harm, Benefit}

public class MessageDispatcher
{
	#region - member -
	Dictionary<eMessageType, List<MessageRegistry>> m_dicRegistry = new Dictionary<eMessageType, List<MessageRegistry>>();
	#endregion
	#region - function -
	public void AttachRegistry(MessageRegistry _registry)
	{
		foreach(KeyValuePair<eMessageType, MessageRegistry.MessageFunction> pair in _registry.dicFunction)
		{
			List<MessageRegistry> registries;
			if(m_dicRegistry.ContainsKey(pair.Key) == false)
			{
				registries = new List<MessageRegistry>();
				m_dicRegistry.Add(pair.Key, registries);
			}
			registries = m_dicRegistry[pair.Key];
			registries.Add(_registry);
		}
	}
	
	public void DetachRegistry(MessageRegistry _registry)
	{
		foreach(KeyValuePair<eMessageType, MessageRegistry.MessageFunction> pair in _registry.dicFunction)
		{
			if(m_dicRegistry.ContainsKey(pair.Key) == false)
				continue;
			
			List<MessageRegistry> registries = m_dicRegistry[pair.Key];
			registries.Remove(_registry);
		}
	}
	#endregion
	#region - message -
	public void HandleMessage(AsIMessage _msg)
	{
		if(m_dicRegistry.ContainsKey(_msg.MessageType) == true)
		{
			List<MessageRegistry> registries = m_dicRegistry[_msg.MessageType];
			int count = registries.Count;
			for(int i=0; i<count; ++i)
			{
				registries[i].HandleMessage(_msg);
			}
		}
	}
	#endregion
}

public delegate bool ConditionCheck(eSkillIcon_Enable_Condition _condition);
public delegate bool BuffCheck(int _index);
public delegate eModelType ModelTypeCheck();
public delegate eModelLoadingState ModelLoadingStateCheck();
public delegate bool PartsLoadedCheck();
public delegate bool TargetShopOpening();
public delegate float NavPathDistance(Vector3 _pos1, Vector3 _pos2, bool _show);
public delegate AsBaseEntity GetterTarget();
public delegate float Dlt_CharacterSize();
public delegate float Dlt_AnimationTime();
//public delegate AsBaseEntity GetterTarget();

public abstract class AsBaseEntity : MonoBehaviour
{
	#region - member -
	public static readonly float s_PetRange = 2f;
	
	public abstract eEntityType EntityType{get;}
	eFsmType m_FsmType;public eFsmType FsmType{get{return m_FsmType;}set{m_FsmType = value;}}
	
	public abstract bool isIntangible{get;}
	public abstract bool isTrap{get;}
	
	public abstract bool isSegregate{get;}
	
	Dictionary<eComponentType, AsBaseComponent> m_dicComponent = new Dictionary<eComponentType, AsBaseComponent>();
	AsPropertySet m_PropertySet = new AsPropertySet();
	
	GameObject m_ModelObject;
	public GameObject ModelObject{get{return m_ModelObject;}}
	public void SetModelObject(GameObject _modelObject){m_ModelObject = _modelObject;}
	
	Animation m_ModelObjectAnimation;
	public Animation ModelObjectAnimation{get{return m_ModelObjectAnimation;}}
	public void SetModelObjectAnimation(Animation _animation){m_ModelObjectAnimation = _animation;}
	
	CharacterController m_Controller;
	public CharacterController characterController{get{return m_Controller;}}
	public void SetCharacterController(CharacterController _controller){m_Controller = _controller;}

    [HideInInspector]
    public AsPanel_Name namePanel   = null;
    public AsPanel_Quest questPanel = null;
    public QuestCollectionMarkController collectionMark = null;
	private bool needShadow = true;
	public bool NeedShadow
	{
		get { return needShadow; }
		set { needShadow = value; }
	}
	public Vector2 shadowScale = Vector2.one;
	
	MessageDispatcher m_MessageDispatcher = new MessageDispatcher();
	
	Dictionary<string, Transform> m_dicDummy;
	
//	protected Vector3 m_TableScale = Vector3.one; public Vector3 TableScale{get{return m_TableScale;}}
	
	//condition
	ConditionCheck m_ConditionCheck = null;
	//buff
	BuffCheck m_BuffCheck = null;
	//model
	ModelTypeCheck m_ModelTypeCheck = null;
	ModelLoadingStateCheck m_ModelLoadingStateCheck = null;
	PartsLoadedCheck m_PartsLoadedCheck = null;
	
	TargetShopOpening m_TargetShopOpening = null;
	
	NavPathDistance m_NavPathDistance = null;
	
	//skill name shouting
	AsSkillNameShout m_SkillNameShout = null;
	
	//get target
	GetterTarget m_GetterTarget = null;
	
	//chactacter size
	Dlt_CharacterSize m_Character_Size = null;
	
	//animation time
	Dlt_AnimationTime m_AnimationTime = null;
#if AUTOMOVE_EFFECT
	private bool m_isNeedAutoMoveEffect = false;
	
	public void SetNeedAutoMoveEffect( bool isNeed )
	{
		m_isNeedAutoMoveEffect = isNeed;
	}
	
	public bool isNeedAutoMoveEffect
	{
		get
		{
			return m_isNeedAutoMoveEffect;
		}
	}
#endif
	
	public bool isKeepDummyObj = false;
	
	//animation
	public bool AnimEnableViaShop
	{
		get
		{
			bool shop = false;
			if( ContainProperty( eComponentProperty.SHOP_OPENING) == true)
				shop = GetProperty<bool>( eComponentProperty.SHOP_OPENING);
			
			if( ( true == shop) && ( ( GAME_STATE.STATE_INGAME == AsGameMain.s_gameState) || ( GAME_STATE.STATE_LOADING == AsGameMain.s_gameState)))
			{
				Debug.LogWarning("AsBaseEntity::AnimEnable = false: eComponentProperty.SHOP_OPENING = " + shop + ", AsGameMain.s_gameState = " + AsGameMain.s_gameState);
				return false;
			}
			
			return true;
		}
	}
	
	bool m_Destroyed = false; public bool Destroyed{get{return m_Destroyed;}}
	public void DestroyThis(){m_Destroyed = true;}
	#endregion
	
	#region - init -
	public void AttachComponent(AsEntityTemplate _entityTemplate)
	{
		foreach(string type in _entityTemplate.m_listComponent)
		{
			AsBaseComponent component = gameObject.AddComponent(type) as AsBaseComponent;
			
			if(component != null)
			{
				m_dicComponent.Add(component.ComponentType, component);
				m_MessageDispatcher.AttachRegistry(component.MsgRegistry);
			}
			else
				Debug.LogError("[AsBaseEntity]Init: It's not valid component name");	
		}
		
		m_PropertySet = _entityTemplate.m_PropertySet.Clone();
	}
	
	public void InitComponents()
	{
		foreach(KeyValuePair<eComponentType, AsBaseComponent> pair in m_dicComponent)
		{
			pair.Value.Init(this);
		}
	}
	
	public void InterInitComponents()
	{
		foreach(KeyValuePair<eComponentType, AsBaseComponent> pair in m_dicComponent)
		{
			pair.Value.InterInit(this);
		}
	}
	
	public void LateInitComponents()
	{
		foreach(KeyValuePair<eComponentType, AsBaseComponent> pair in m_dicComponent)
		{
			pair.Value.LateInit(this);
		}
	}
	
	public void LastInitComponents()
	{
		foreach(KeyValuePair<eComponentType, AsBaseComponent> pair in m_dicComponent)
		{
			pair.Value.LastInit(this);
		}
	}

    public AsBaseComponent GetComponent(eComponentType etype)
    {
        if (false == m_dicComponent.ContainsKey(etype))
            return null;

        return m_dicComponent[etype];
    }
	
	public void SetEnterWorldData(AS_GC_ENTER_WORLD_RESULT _data)
	{
		transform.position = _data.sPosition;
	}
	
	public void SetEnterWorldData(Vector3 _pos)
	{			
		
		transform.position = _pos;
	}
	
	public void SetRePosition( Vector3 vec3Pos )
	{
//		StartCoroutine("RePosition", vec3Pos);
		
		vec3Pos.y = TerrainMgr.GetTerrainHeight(characterController, vec3Pos);
		transform.position = vec3Pos;
	}
	
	IEnumerator RePosition(Vector3 _pos)
	{
		while(true)
		{
			yield return new WaitForSeconds(1);
			
			if(characterController != null)
			{
				_pos.y = TerrainMgr.GetTerrainHeight(characterController, _pos);
				transform.position = _pos;
				
				break;
			}
		}
	}
	
	public void DetachComponent( Type _type)
	{
		AsBaseComponent component = GetComponent( _type) as AsBaseComponent;
		m_MessageDispatcher.DetachRegistry( component.MsgRegistry);
		DestroyImmediate( component);
		
		m_dicComponent.Remove( component.ComponentType);
	}
	#endregion
	
	#region - update & message -
	public void HandleMessage(AsIMessage _msg)
	{
		m_MessageDispatcher.HandleMessage(_msg);
		
//		if(m_dicComponent.ContainsKey(_msg.ComponentType) == true)
//		{
//			m_dicComponent[_msg.ComponentType].HandleMessage(_msg);
//		}		
//		foreach(KeyValuePair<eComponentType, AsBaseComponent> component in m_dicComponent)
//		{
//			component.Value.HandleMessage(_msg);
//		}
	}
	#endregion
	
	#region - property -
	public bool ContainProperty(eComponentProperty _property)
	{
		return m_PropertySet.ContainProperty(_property);
	}
	
//	public void SetValue(string _id, System.Object _value)
	public void SetProperty(eComponentProperty _id, System.Object _value)
	{
		m_PropertySet.SetValue(_id, _value);
	}
	
//	public T GetValue<T>(string id)
//	{
//		return (T)m_PropertySet.GetValue(id);
//	}
	public T GetProperty<T>(eComponentProperty id)
	{
		try
		{
			T temp = (T)m_PropertySet.GetValue(id);
			return temp;
		}
		catch( Exception e)
		{
			Debug.LogError( e);
			Debug.LogError("type:" + typeof(T) + "[" + id + "] property does not exist");
		}
		
		return default(T);
		//return (T)m_PropertySet.GetValue(id);
	}
	#endregion
	
	#region - getter -
	public void SetDummyTransform(Dictionary<string, Transform> _dic)
	{
		m_dicDummy = _dic;
	}
	
	public Transform GetDummyTransform(string _name)
	{
		if(m_dicDummy == null)
			return null;
		
		if( false == m_dicDummy.ContainsKey(_name ) )
		{
			Debug.LogError("AsBaseEntity::GetDummyTransform()[ not find : " + _name );
			return null;
		}

		if(m_dicDummy[_name] == null)
			Debug.LogWarning("AsBaseEntity:: GetDummyTransform: " + _name + " is initialized as null");

		return m_dicDummy[_name];
	}
	
	public void SetConditionCheckDelegate(ConditionCheck _check)
	{
		m_ConditionCheck = _check;
	}
	
	public bool CheckCondition(eSkillIcon_Enable_Condition _condition)
	{
		if(m_ConditionCheck != null)
			return m_ConditionCheck(_condition);
		else
			return false;
	}
	
	public void SetBuffCheckDelegate(BuffCheck _check)
	{
		m_BuffCheck = _check;
	}
	
	public bool CheckBuff(int _index)
	{
		if(m_BuffCheck != null)
			return m_BuffCheck(_index);
		else
			return false;
	}
	
	public bool CheckShopOpening()
	{
		if(ContainProperty(eComponentProperty.SHOP_OPENING) == true)
		{
			return GetProperty<bool>(eComponentProperty.SHOP_OPENING);
		}
		else 
			return false;
	}
	
	public void SetModelTypeDelegate(ModelTypeCheck _check)
	{
		m_ModelTypeCheck = _check;
	}
	
	public eModelType CheckModelType()
	{
		if(m_ModelTypeCheck == null)
			return eModelType.Normal;
		else
			return m_ModelTypeCheck();
	}
	
	public void SetModelLoadingStateDelegate(ModelLoadingStateCheck _check)
	{
		m_ModelLoadingStateCheck = _check;
	}
	
	public eModelLoadingState CheckModelLoadingState()
	{
		if(m_ModelLoadingStateCheck == null)
		{
			Debug.LogError("AsBaseEntity:CheckModelLoadingState: m_ModelLoadingStateCheck == null");
			return eModelLoadingState.NONE;
		}
		else
			return m_ModelLoadingStateCheck();
	}
	
	public void SetPartsLoadedDelegate(PartsLoadedCheck _check)
	{
		m_PartsLoadedCheck = _check;
	}
	
	public bool CheckPartsLoaded()
	{
		if( m_PartsLoadedCheck == null)
			return false;
		else
			return m_PartsLoadedCheck();
	}
	
	public void SetTargetShopOpening(TargetShopOpening _open)
	{
		m_TargetShopOpening = _open;
	}
	
	public bool CheckTargetShopOpening()
	{
		if(m_TargetShopOpening == null)
			return false;
		else
			return m_TargetShopOpening();
	}
	
	public void SetNavPathDistance(NavPathDistance _nav)
	{
		m_NavPathDistance = _nav;
	}
	
	public float GetNavPathDistance(Vector3 _pos1, Vector3 _pos2, bool _show = false)
	{
		if(m_NavPathDistance == null)
			return float.MaxValue;
		else
			return m_NavPathDistance(_pos1, _pos2, _show);
	}
	
	public void SetGetterTarget(GetterTarget _get)
	{
		m_GetterTarget = _get;
	}
	
	public AsBaseEntity GetGetterTarget()
	{
		if(m_GetterTarget == null)
			return null;
		
		return m_GetterTarget();
	}
	
	public bool CheckObjectMonster()
	{
		if(ContainProperty(eComponentProperty.MONSTER_ID) == true)
		{
			int monsterId = GetProperty<int>(eComponentProperty.MONSTER_ID);
			Tbl_Monster_Record monsterRecord = AsTableManager.Instance.GetTbl_Monster_Record(monsterId);
			
			if(monsterRecord != null &&
				(monsterRecord.Grade == eMonster_Grade.DObject || monsterRecord.Grade == eMonster_Grade.QObject))
				return true;
			else
				return false;
		}
		else
			return false;
	}
	
	public void SetCharacterSize(Dlt_CharacterSize _get)
	{
		m_Character_Size = _get;
	}
	
	public float GetCharacterSize()
	{
		if(m_Character_Size != null)
			return m_Character_Size();
		else
			return 1f;
	}
	
	public void SetAnimationTime(Dlt_AnimationTime _get)
	{
		m_AnimationTime = _get;
	}
	
	public float GetAnimationTime()
	{
		if(m_AnimationTime != null)
			return m_AnimationTime();
		else
			return 0f;
	}
	#endregion
	
	#region - skill name shout -
	public void SkillNameShout(eSkillNameShoutType _type, string msg)
	{
		if(ModelObject == null)
			return;
		
		if( null != m_SkillNameShout)
		{
			Destroy( m_SkillNameShout.gameObject);
			m_SkillNameShout = null;
		}

		GameObject obj = Instantiate( Resources.Load("UI/AsGUI/GUI_Skillname")) as GameObject;
		AsSkillNameShout skillNameShout = obj.GetComponent<AsSkillNameShout>();
		skillNameShout.gameObject.transform.localPosition = Vector3.zero;
		skillNameShout.Owner = this;
		
		skillNameShout.SetText( msg + AsTableManager.Instance.GetTbl_String(1447));
		
		string color = "";
		switch(_type)
		{
		case eSkillNameShoutType.Self:
			break;
		case eSkillNameShoutType.Harm:
			color = Color.red.ToString();// + msg;
			break;
		case eSkillNameShoutType.Benefit:
			color = Color.cyan.ToString();// + msg;
			break;
		}
		
		skillNameShout.SetColor(color);
		
		m_SkillNameShout = skillNameShout;

//		Vector3 worldPos = ModelObject.transform.position;
//		worldPos.y += characterController.height;
//		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
//		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
//		vRes.y += skillNameShout_RevisionY;
//		vRes.z = 0.0f;
//		skillNameShout.gameObject.transform.position = vRes;
	}
	
//	public float skillNameShout_RevisionY = -0.8f;
	public float skillNameShout_RevisionY = -2;
	#endregion
	
	#region - public -
	public void TargetMarkProc()
	{
		if( m_FsmType == eFsmType.MONSTER &&
			AsPartyManager.Instance.NpcTargetIdx != 0 &&
			AsPartyManager.Instance.NpcTargetIdx == (this as AsNpcEntity).SessionId)
			namePanel.SetTargetMark( true);
		else if( m_FsmType == eFsmType.OTHER_USER &&
			AsPartyManager.Instance.CharTargetIdx != 0 &&
			AsPartyManager.Instance.CharTargetIdx == (this as AsUserEntity).SessionId)
			namePanel.SetTargetMark( true);
	}
	
	public void Remove_Ani_Model()
	{
		DestroyImmediate(GetComponent<AsAnimation>());
		DestroyImmediate(GetComponent<AsModel>());
	}
	#endregion
}
	