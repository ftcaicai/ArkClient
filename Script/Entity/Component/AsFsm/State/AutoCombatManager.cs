using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoCombatManager : MonoBehaviour
{
	#region - singleton -
	static AutoCombatManager m_Instance; public static AutoCombatManager Instance{get{return m_Instance;}}
	#endregion
	#region - member -
	[SerializeField] UIButton m_Button = null;
	[SerializeField] PackedSprite m_Sprite = null;
	[SerializeField] SimpleSprite m_Img_Off = null;
	[SerializeField] SimpleSprite m_Img_On = null;
	[SerializeField] SimpleSprite m_Img_Disable = null;
	
	bool m_Activate; public bool Activate{get{return m_Activate;}}
	
	AutoCombatPotionSpecifier m_PotionSpecifier = null;
	RealItem m_PotionRealItem = null;
	#endregion
	#region - init & update -
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
	}
	
	void OnEnable()
	{
		TurnOff(true);

		m_dicEvent.Clear();
	}
	
	// Use this for initialization
	void Start ()
	{
		m_PotionSpecifier = Resources.Load("UseScript/AutoCombatPotionSpecifier") as AutoCombatPotionSpecifier; if(m_PotionSpecifier == null) Debug.LogError("there is no asset");
		
		m_Button.SetInputDelegate(OnButton);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	#endregion
	#region - toggle -
	void ToggleActivate()
	{
		if (m_Activate == true)
		{
			TurnOff();
			
			AsEntityManager.Instance.MessageToPlayer(new Msg_AutoCombat_Off(true));

			if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.AUTO_BATTLE) != null)
				AsCommonSender.SendClearOpneUI(OpenUIType.AUTO_BATTLE);
		}
		else
		{
			if (CheckPossibleAutoCombat() == true)
			{
				TurnOn();

				AsEntityManager.Instance.MessageToPlayer(new Msg_AutoCombat_On());

				if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.AUTO_BATTLE) != null)
					AsCommonSender.SendClearOpneUI(OpenUIType.AUTO_BATTLE);
			}
		}
	}

	bool CheckPossibleAutoCombatArea()
	{
		if(TerrainMgr.Instance == null)
			return false;
		
		if(	TerrainMgr.Instance.GetCurrentMap().MapData.AutoUse == MapData.eAutoUse.NONE)
			return false;

		return true;
	}
	
	bool CheckPossibleAutoCombat(bool _showMsg = true)
	{
		if(TerrainMgr.Instance == null)
			return false;

		if(	TerrainMgr.Instance.GetCurrentMap().MapData.AutoUse == MapData.eAutoUse.NONE)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2110);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
			
			return false;
		}
		
		if(CheckEnoughCondition() == false && ConditionEventActivate == false)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2111);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
			
			return false;
		}

		bool lvEnable = true;
		if( CheckPotionInQuickSlot(ref lvEnable) == false)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2112);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
			
			return false;
		}
		else if(lvEnable == false)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2726);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}

			return false;
		}

		if( AsPartyManager.Instance.IsPartying == true)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2113);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
			
			return false;
		}

		if( AsEntityManager.Instance.GetPlayerCharFsm().WeaponEquip == false)
		{
			if(_showMsg == true)
			{
				string str = AsTableManager.Instance.GetTbl_String(2725);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
			
			return false;
		}
		
		return true;
	}
	
	void OnButton(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ToggleActivate();
		}
	}
	#endregion
	#region - on & off -
	void TurnOn()
	{
		m_Activate = true;
		
		m_Sprite.enabled = true;
		m_Sprite.renderer.enabled = true;
		
		m_Img_Off.renderer.enabled = false;
		m_Img_On.renderer.enabled = true;
		
		if( AsSoundManager.Instance != null)
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6125_EFF_AutoOn", Vector3.zero, false);
	}
	
	void TurnOff(bool _onEnable = false)
	{
		m_Activate = false;
		
		m_Sprite.enabled = false;
		m_Sprite.renderer.enabled = false;
		
		m_Img_Off.renderer.enabled = true;
		m_Img_On.renderer.enabled = false;
		
		if( AsSoundManager.Instance != null && _onEnable == false)
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6126_EFF_AutoOff", Vector3.zero, false);
	}
	#endregion
	#region - public -	
	// potion
	public void ItemRemovedFromQuickSlot( int _index, int _value, eQUICKSLOT_TYPE _type)
	{
		if( m_Activate == true && _index == 0 &&
			( _value == 0 || m_PotionSpecifier.CheckValidPotion(_value) == false))
		{
			TurnOff();
			
			AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
		
			string str = AsTableManager.Instance.GetTbl_String(2115);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
		}
	}
	
	// party 
	public void PartyOrganized()
	{
		if(m_Activate == true)
		{
			TurnOff();
			
			AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
			
			string str = AsTableManager.Instance.GetTbl_String(2116);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
		}
	}
	
	// zone warp
	bool m_WarpedInActivated = false;
	public void ZoneWarped()
	{
		if( m_Activate == true)
		{
			m_WarpedInActivated = true;
		}
	}	
	public void ZoneWarpFinished()
	{
		if( m_WarpedInActivated == true)
		{
			m_Activate = false;
			m_Sprite.enabled = false;
			m_Sprite.renderer.enabled = false;
			
			string str = AsTableManager.Instance.GetTbl_String(2116);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
		}
		
		m_WarpedInActivated = false;

		TurnOff(true);

		if(CheckPossibleAutoCombatArea() == false)
			m_Img_Disable.renderer.enabled = true;
		else
			m_Img_Disable.renderer.enabled = false;
	}
	
	// target changed
	public void TargetChangeProcess(AsPlayerFsm _player)
	{
		if(m_Activate == true)
		{
			if(/*_player.Target == null && */ CheckEnoughCondition() == false && ConditionEventActivate == false)
			{
				TurnOff();
				
				AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
				
				string title = AsTableManager.Instance.GetTbl_String(2114);
				string str = AsTableManager.Instance.GetTbl_String(2117);
				AsNotify.Instance.MessageBox( title, str,AsNotify.MSG_BOX_TYPE.MBT_OK);
			}
			
			if(_player.Target != null && _player.Target.FsmType != eFsmType.MONSTER)
			{
				TurnOff();
				
				AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
				
				string str = AsTableManager.Instance.GetTbl_String(2116);
				AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
			}
		}
				
	}
	
	// damaged
	public void DamagedProcess()
	{
		if(m_Activate == false)
			return;
		
		float rate = AsUserInfo.Instance.SavedCharStat.hpCur_ / AsUserInfo.Instance.SavedCharStat.sFinalStatus.fHPMax;
		float recover = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 136).Value * 0.001f;
		if( recover > rate)
		{
			bool release = true;
			
			Item item = null;
			if( AutoCombatManager.Instance.CheckPotionInQuickSlot( out item) == true)
			{
				release = false;
				
				CoolTimeGroup cool = CoolTimeGroupMgr.Instance.GetCoolTimeGroup(item.ItemData.itemSkill, item.ItemData.itemSkillLevel);
				if(cool != null)
					Debug.LogWarning("AutoCombatManager::DamagedProcess: cool.isCoolTimeActive = " + cool.isCoolTimeActive);
				
				if(cool != null && cool.isCoolTimeActive == true)
				{
				}
				else
				{
					SetRealItem_AutoCombatPotion(item.ItemID);
					AsEntityManager.Instance.MessageToPlayer(new Msg_Player_Use_ActionItem(m_PotionRealItem));
				}
			}
			
			if(release == true)
			{
				TurnOff();
				
				AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
				
				string title = AsTableManager.Instance.GetTbl_String(2114);
				string str = AsTableManager.Instance.GetTbl_String(2115);
				AsNotify.Instance.MessageBox( title, str,AsNotify.MSG_BOX_TYPE.MBT_OK);
			}
		}
	}
	
	// death
	public void DeathProcess()
	{
		if(m_Activate == true)
		{
			TurnOff();
			
			AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
			
			string str = AsTableManager.Instance.GetTbl_String(2116);
			AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(str, false);
		}
	}
	
	// condition event
	Dictionary<int, body2_SC_SERVER_EVENT_START> m_dicEvent = new Dictionary<int, body2_SC_SERVER_EVENT_START>();
	bool ConditionEventActivate{get{return m_dicEvent.Count != 0;}}
	public void EventProcess( body1_SC_SERVER_EVENT_START _start)
	{
		foreach( body2_SC_SERVER_EVENT_START node in _start.body)
		{
			if( node.eEventType == eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION)
				m_dicEvent.Add(node.nEventKey, node);
		}
	}
	public void EventProcess( body1_SC_SERVER_EVENT_STOP _stop)
	{
		foreach( body2_SC_SERVER_EVENT_STOP node in _stop.body)
		{
			if( m_dicEvent.ContainsKey( node.nEventKey) == true)
				m_dicEvent.Remove( node.nEventKey);
		}
		
//		if(m_Activate == true)
//		{
//			TurnOff();	
//			AsEntityManager.Instance.MessageToPlayer( new Msg_AutoCombat_Off(false));
//		}
	}

	public void ExitInGame()
	{
		m_dicEvent.Clear();
	}
	#endregion
	#region - method -
	// potion
	bool CheckPotionInQuickSlot( out Item _item)
	{
		bool potionExist = false;
		_item = null;
		
		sQUICKSLOT slot = ItemMgr.HadItemManagement.QuickSlot.getQuickSlots[0];
		if( (eQUICKSLOT_TYPE)slot.eType == eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM &&
			m_PotionSpecifier.CheckValidPotion(slot.nValue) == true)
		{
			_item = ItemMgr.ItemManagement.GetItem(slot.nValue);
			int count = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(_item.ItemID);
			
			if(count > 0 && _item.ItemData.levelLimit <= AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL))
				potionExist = true;
		}
		
		return potionExist;
	}
	
	bool CheckPotionInQuickSlot( ref bool _lvEnable)
	{
		bool potionExist = false;
		
		sQUICKSLOT slot = ItemMgr.HadItemManagement.QuickSlot.getQuickSlots[0];
		if( (eQUICKSLOT_TYPE)slot.eType == eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM &&
			m_PotionSpecifier.CheckValidPotion(slot.nValue) == true)
		{
			Item item = ItemMgr.ItemManagement.GetItem(slot.nValue);
			int count = ItemMgr.HadItemManagement.Inven.GetItemTotalCount(item.ItemID);

			_lvEnable = item.ItemData.levelLimit <= AsEntityManager.Instance.UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
			if(count > 0)
				potionExist = true;
		}
		
		return potionExist;
	}
	
	void SetRealItem_AutoCombatPotion(int _idx)
	{
//		if(m_PotionRealItem == null || m_PotionRealItem.item.ItemID != _idx)
//		{
			m_PotionRealItem = ItemMgr.HadItemManagement.Inven.GetRealItem(_idx);
//		}
	}

	bool CheckEnoughCondition()
	{
		float maxCondition = AsTableManager.Instance.GetTbl_GlobalWeight_Record(11).Value;
		float curCondition = (float)AsUserInfo.Instance.CurConditionValue;

		float condition = curCondition / maxCondition;

		return condition > 0.02f;
	}
	#endregion
}

