using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPetManager : MonoBehaviour {
	
	#region - sinlgeton -
	static AsPetManager m_Instance; public static AsPetManager Instance{get{return m_Instance;}}
	#endregion
	#region - member -
	public static readonly int ePETINVENTORY = 10000;
	public static readonly int s_BaseManageLine = 2;
	public static readonly int s_MaxStarGrade = 5;

	static bool m_Popup; public static bool Popup{get{return m_Popup;}}

	enum eDlgState {NONE = 0, Info, Upgrade, Synthesis}
//	eDlgState m_DlgState = eDlgState.NONE;

	PetInfo m_PetInfo = new PetInfo(); // - pet following player now
	public bool isPetExist{get{return ( m_PetInfo.nPetTableIdx > 0);}}
	public bool isPetEquip{get{return ( m_PetInfo.itemView != null && m_PetInfo.itemView.nItemTableIdx > 0);}}
	public ePET_HUNGRY_STATE PetHungry{get{return m_PetInfo.Hungry;}}
	public sITEMVIEW ItemView{get{return m_PetInfo.itemView;}}

	public PetListElement CurPetListElement{get{return m_CurDisplayingInfoOnPetInfoDlg.Element;}} // - pet current displaying on pet info dlg
	PetInfo m_CurDisplayingInfoOnPetInfoDlg; public PetInfo CurDisplayingInfoOnPetInfoDlg{get{return m_CurDisplayingInfoOnPetInfoDlg;}}

	int m_ExtendLine; public int ExtendLine{get{return m_ExtendLine;}}

	Dictionary<int, PetListElement> m_dicPetList = new Dictionary<int, PetListElement>();
	List<PetListElement> m_listPetList = new List<PetListElement>(); public List<PetListElement> listPetList{get{return m_listPetList;}}
	UIPetManageDlg m_PetManageDlg; public UIPetManageDlg PetManageDlg{get{return m_PetManageDlg;}}

	UIPetInfoDlg m_PetDlg; public UIPetInfoDlg PetInfoDlg{get{return m_PetDlg;}}
	UIPetUpgradeDlg m_PetUpgradeDlg; public UIPetUpgradeDlg PetUpgradeDlg{get{return m_PetUpgradeDlg;}}
	UIPetSynthesisDlg m_PetSynthesisDlg; public UIPetSynthesisDlg PetSynthesisDlg{get{return m_PetSynthesisDlg;}}
	#endregion
	#region - init & update -
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion

//		m_sdicPetList = new SortedDictionary<int, PetListElement>(Dlt_SortPetList);
	}
	
	void OnEnable()
	{
		m_PacketSend = false;
	}
	
	// Use this for initialization
	IEnumerator Start () {
		
		while(true)
		{
			yield return null;
			
			if( AsTableManager.Instance.s_bTableLoaded == true)
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	#endregion
	#region - send -
	bool m_PacketSend = false; public bool PacketSend{get{return m_PacketSend;}}
	// - ui window -
	public void Send_PetInfo(UIPetManageSlot _slot)
	{
		body_CS_PET_INFO info = new body_CS_PET_INFO(_slot.Element.PetSlot);
		AsCommonSender.Send(info.ClassToPacketBytes());
	}

	public void Send_PetCall()
	{
		body_CS_PET_CALL call = new body_CS_PET_CALL(m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot);
		AsCommonSender.Send(call.ClassToPacketBytes());
	}

	public void Send_Release()
	{
		body_CS_PET_RELEASE release = new body_CS_PET_RELEASE();
		AsCommonSender.Send(release.ClassToPacketBytes());
	}

	public void Send_PetDelete()
	{
		body_CS_PET_DELETE delete = new body_CS_PET_DELETE(m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot);
		AsCommonSender.Send( delete);
	}

	// - equip -
	RealItem m_ReservedEquip;
	public void Send_PetEquip(RealItem _slot)
	{
		if(m_PacketSend == true)
			return;

		if(isPetExist == false)
		{
			AsHudDlgMgr.Instance.SetMsgBox(
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2753),
			                             AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
			return;
		}

		m_ReservedEquip = _slot;

		string content = "undefined content";
		if(isPetEquip == true)
		{
			string reservedEquip = AsTableManager.Instance.GetTbl_String(_slot.item.ItemData.nameId);
			content = string.Format(AsTableManager.Instance.GetTbl_String(2754), reservedEquip);
			AsHudDlgMgr.Instance.SetMsgBox(
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), content,
			                             this, "_EquipConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING));
			return;
		}

		string petName = AsUtil.GetRealString(System.Text.UTF8Encoding.UTF8.GetString(m_PetInfo.szPetName));
		content = string.Format(AsTableManager.Instance.GetTbl_String(2755), petName);
		AsHudDlgMgr.Instance.SetMsgBox(
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), content,
		                             this, "_EquipConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_WARNING));
	}
	void _EquipConfirm()
	{
		body_CS_PET_EQUIP_ITEM equip = new body_CS_PET_EQUIP_ITEM(m_ReservedEquip.getSlot);
		AsCommonSender.Send( equip);

		m_PacketSend = true;
	}
  
	public void Send_PetUnEquip()
	{
		int cost = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetAcc_Lift").Value;

		AsMessageBox msgBox = null;
		if(cost > AsUserInfo.Instance.nMiracle)
		{                                           
			msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String(264), this, "_OpenCashShop",
			                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.gameObject.AddComponent<UIPetPopup>();
			return;
		}

		string content = AsTableManager.Instance.GetTbl_String(2756);
		content = string.Format(content, m_CurDisplayingInfoOnPetInfoDlg.GetCurItemName() + Color.white) ;
		msgBox = AsNotify.Instance.CashMessageBox((long)cost, AsTableManager.Instance.GetTbl_String( 126), content, this,
		                                          "_PetUnEquipConfirm", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	void _PetUnEquipConfirm()
	{
		body_CS_PET_UNEQUIP_ITEM equip = new body_CS_PET_UNEQUIP_ITEM(m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot);
		AsCommonSender.Send( equip);
	}

	// - act -
	public void Send_PetNameChange( string _name)
	{
		byte[] name = System.Text.Encoding.UTF8.GetBytes(_name);
		
		body_CS_PET_NAME_CHANGE change = new body_CS_PET_NAME_CHANGE(m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot, name);
		AsCommonSender.Send(change.ClassToPacketBytes());
	}

	OkDelegate m_Dlt_Send_Upgrade;
	public void Send_Upgrade(PetListElement _base, PetListElement _matter, OkDelegate _dlt)
	{
		Debug.Log("AsPetManager:: Send_Upgrade: _base.PetSlot = " + _base.PetSlot + ", _matter.PetSlot = " + _matter.PetSlot);

		body_CS_PET_UPGRADE upgrade = new body_CS_PET_UPGRADE(_base.PetSlot, _matter.PetSlot);
		AsCommonSender.Send(upgrade.ClassToPacketBytes());

		m_Dlt_Send_Upgrade = _dlt;
	}

	OkDelegate m_Dlt_Send_Item_Compose;
	public void Send_Item_Compose(int _petSlot, int _slot1, int _slot2, int _slot3, OkDelegate _dlt)
	{
		Debug.Log("AsPetManager:: Send_Item_Compose: _petSlot = " + _petSlot + ", _slot1 = " + _slot1 + ", _slot2 = " + _slot2 + ", _slot3 = " + _slot3);

		body_CS_PET_ITEM_COMPOSE compose = new body_CS_PET_ITEM_COMPOSE(_petSlot, _slot1, _slot2, _slot3);
		AsCommonSender.Send(compose.ClassToPacketBytes());

		m_Dlt_Send_Item_Compose = _dlt;

		m_PacketSend = true;
	}

	public void Send_SlotExtend(int _line)
	{
		Debug.Log("AsPetManager:: Send_SlotExtend: _line = " + _line);

		body_CS_PET_SLOT_EXTEND extend = new body_CS_PET_SLOT_EXTEND(_line);
		AsCommonSender.Send(extend.ClassToPacketBytes());
	}
	#endregion	
	#region - recv -
	public void Recv_PetCreateResult( byte[] _packet)
	{
		m_PacketSend = false;

		body_SC_PET_CREATE_RESULT create = new body_SC_PET_CREATE_RESULT();
		create.PacketBytesToClass(_packet);

		_HatchPerform(create);
	}
	void _HatchPerform(body_SC_PET_CREATE_RESULT _create)
	{
		UIPetPerform perform = _HatchGeneration();
		perform.Open(_create);
	}
	GameObject m_Obj;
	UIPetPerform _HatchGeneration()
	{
		if(m_Obj != null)
			Destroy(m_Obj);

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6200_EFF_PetUpgrade", Vector3.zero, false);

		m_Obj = Instantiate( ResourceLoad.LoadGameObject( "UI/Optimization/Prefab/MessageBox_Pet") ) as GameObject;
		m_Obj.AddComponent<UIPetPopup>();
		return m_Obj.GetComponent<UIPetPerform>();
	}

	public void Recv_PetSkillUse( byte[] _packet)
	{
		body_SC_PET_SKILL_USE use = new body_SC_PET_SKILL_USE();
		use.PacketBytesToClass( _packet);
		
		Msg_Pet_Skill_Ready ready = new Msg_Pet_Skill_Ready( use);
		AsEntityManager.Instance.DispatchMessageByUniqueKey( use.nCharUniqKey, ready);
	}

	public void Recv_PetLevelUp( byte[] _packet)
	{
		body_SC_PET_LEVEL_UP level = new body_SC_PET_LEVEL_UP();
		level.PacketBytesToClass( _packet);
		
		Msg_Pet_LevelUp lvUp = new Msg_Pet_LevelUp( level);
		AsEntityManager.Instance.DispatchMessageByUniqueKey(level.nCharUniqKey, lvUp);
	}

	public void Recv_PetList(byte[] _packet)
	{
		body1_SC_PET_LIST list = new body1_SC_PET_LIST();
		list.PacketBytesToClass( _packet);

		m_ExtendLine = list.nExtendLine;

		m_listPetList.Clear();
		m_dicPetList.Clear();

		Debug.Log("AsPetManager:: Recv_PetList: m_ExtendLine = " + m_ExtendLine + ", list.nCnt = " + list.nCnt);

		foreach(body2_SC_PET_LIST node in list.body)
		{
			PetListElement element = new PetListElement(node);
			m_listPetList.Add(element);
			m_dicPetList.Add(node.nPetSlot, element);

			Debug.Log("AsPetManager:: Recv_PetList: element.PetTableIdx = " + element.PetTableIdx + ", element.PetSlot = " + element.PetSlot);
			Debug.Log("AsPetManager:: Recv_PetList: element.PetName = " + element.PetName + ", element.Level = " + element.Level);
		}

		m_listPetList.Sort(Dlt_SortPetList);
	}

	public void Recv_PetSlotChange(byte[] _packet)
	{
		body_SC_PET_SLOT_CHANGE slot = new body_SC_PET_SLOT_CHANGE();
		slot.PacketBytesToClass( _packet);

		Debug.Log("AsPetManager:: Recv_PetSlotChange: slot.PetTableIdx = " + slot.nPetTableIdx + ", slot.PetSlot = " + slot.nPetSlot);

		if(m_dicPetList.ContainsKey(slot.nPetSlot) == true)
		{
			PetListElement element = m_dicPetList[slot.nPetSlot];
			element.ChangeElement(slot);

			if(element.PetTableIdx == 0)
			{
				m_listPetList.Remove(element);
				m_dicPetList.Remove(element.PetSlot);
			}
		}
		else 
		{
			PetListElement element = new PetListElement(slot);
			m_listPetList.Add(element);
			m_dicPetList.Add(slot.nPetSlot, element);
		}

		m_listPetList.Sort(Dlt_SortPetList);
		if(m_PetManageDlg != null)
			m_PetManageDlg.Open();
	}

	public void Recv_PetHungry(byte[] _packet)
	{
		body_SC_PET_HUNGRY hungry = new body_SC_PET_HUNGRY();
		hungry.PacketBytesToClass( _packet);

		Debug.Log("AsPetManager:: Recv_PetHungry: hungry.eHungryState = " + hungry.eHungryState);

		m_PetInfo.SetHungry(hungry);

		if(m_PetInfo.Hungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
			SendScriptWord(Tbl_PetScript_Record.eType.Hungry);

		AsEntityManager.Instance.MessageToPlayer(new Msg_PetHungryIndicate());
	}

	public void Recv_PetNameNotify( byte[] _packet)
	{
		body_SC_PET_NAME_NOTIFY nameNotify = new body_SC_PET_NAME_NOTIFY();
		nameNotify.PacketBytesToClass( _packet);

		AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( nameNotify.nCharUniqKey);
		if( user == null)
		{
			Debug.LogWarning( "AsPetManager:: PetNotify: user is not found. id = " + nameNotify.nCharUniqKey);
			return;
		}
		
		Msg_PetNameChange change = new Msg_PetNameChange( nameNotify);
		user.HandleMessage( change);
		
		if(user.FsmType == eFsmType.PLAYER)
		{
			m_PetInfo.szPetName = nameNotify.szPetName;
			
			if( m_PetDlg != null)
				m_PetDlg.ChangeName( nameNotify);
		}
	}

	// - ui window -
	public void Recv_PetInfo( byte[] _packet)
	{
		body_SC_PET_INFO info = new body_SC_PET_INFO();
		info.PacketBytesToClass( _packet);

		if(m_dicPetList.ContainsKey(info.nSlot) == true)
		{
			if(m_PetManageDlg != null && m_CurDisplayingInfoOnPetInfoDlg != null)
				m_PetManageDlg.SetSlotToBase(m_CurDisplayingInfoOnPetInfoDlg.Element);

			m_CurDisplayingInfoOnPetInfoDlg = new PetInfo(info);
			m_CurDisplayingInfoOnPetInfoDlg.Element = m_dicPetList[info.nSlot];

			if(m_PetManageDlg != null)
				m_PetManageDlg.SetSlotToSelect(m_CurDisplayingInfoOnPetInfoDlg.Element);
		}
		else
		{
			Debug.LogError("AsPetManager:: Recv_PetInfo: invalid uniq slot idx. info.nSlot = " + info.nSlot);
			return;
		}

		if(m_PetDlg == null)
		{
			GameObject obj = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_PetInfo") ) as GameObject;
			m_PetDlg = obj.GetComponentInChildren<UIPetInfoDlg>();
		}

		m_PetDlg.Open( info);

//		if(m_PetInfo.nPetUniqSlot == m_CurDisplayingInfoOnPetInfoDlg.nPetUniqSlot)
//		{
////			m_PetInfo.SetHungry(m_CurDisplayingInfoOnPetInfoDlg.Hungry);
//			AsEntityManager.Instance.MessageToPlayer(new Msg_PetHungryIndicate());
//		}
	}

	public void Recv_PetCall( byte[] _packet)
	{
		body_SC_PET_CALL call = new body_SC_PET_CALL();
		call.PacketBytesToClass( _packet);
		
		Debug.Log("AsPetManager:: Recv_PetCall: call.nPetTableIdx = " + call.nPetTableIdx);

		AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( call.nCharUniqKey);
		if( user == null)
		{
			Debug.LogWarning( "AsPetManager:: Recv_PetCall: user is not found. id = " + call.nCharUniqKey);
			return;
		}

		PetAppearData appear = new PetAppearData( user, call);
		user.HandleMessage( new Msg_PetDataIndicate( appear));

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6024_EFF_Bonus", Vector3.zero, false);

		if(user.FsmType == eFsmType.PLAYER)
		{
			m_PetInfo.SetValue( call);

//			if(m_PetInfo.Hungry == ePET_HUNGRY_STATE.ePET_HUNGRY_STATE_HUNGRY)
			AsEntityManager.Instance.MessageToPlayer(new Msg_PetHungryIndicate());

			if(m_PetDlg != null)
				m_PetDlg.RefreshInfoDlg();

			if(m_PetManageDlg != null)
				m_PetManageDlg.Open();
		}
	}

	public void Recv_PetCallResult(byte[] _packet)
	{
		body_SC_PET_CALL_RESULT call = new body_SC_PET_CALL_RESULT();
		call.PacketBytesToClass(_packet);

		if(call.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetCallResult: call.eResult = " + call.eResult);
		}
	}

	public void Recv_PetRelease(byte[] _packet)
	{
		body_SC_PET_RELEASE release = new body_SC_PET_RELEASE();
		release.PacketBytesToClass( _packet);

		AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId(release.nCharUniqKey);
		if( user == null)
		{
			Debug.LogWarning( "AsPetManager:: Recv_PetRelease: user is not found. id = " + release.nCharUniqKey);
			return;
		}

		user.HandleMessage(new Msg_PetDelete());
		if(user.FsmType == eFsmType.PLAYER)
		{
			m_PetInfo.Clear();
			
			if(m_PetDlg != null)
				m_PetDlg.RefreshInfoDlg();

			if(m_PetManageDlg != null)
				m_PetManageDlg.Open();
		}
	}

	public void Recv_PetReleaseResult( byte[] _packet)
	{
		body_SC_PET_RELEASE_RESULT release = new body_SC_PET_RELEASE_RESULT();
		release.PacketBytesToClass( _packet);

		if(release.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetReleaseResult: release.eResult = " + release.eResult);
			return;
		}

		m_PetInfo.Clear();
	}

	public void Recv_PetDeleteResult( byte[] _packet)
	{
		body_SC_PET_DELETE_RESULT delete = new body_SC_PET_DELETE_RESULT();
		delete.PacketBytesToClass( _packet);

		if( delete.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			if(m_dicPetList.ContainsKey(delete.nSlot) == true)
			{
				if(m_CurDisplayingInfoOnPetInfoDlg.Element != null &&
				   (delete.nSlot != m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot))
				{
					Debug.LogError("AsPetManager:: Recv_PetDeleteResult: different slot idx. delete.nSlot = " + delete.nSlot +
					               ", m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot = " + m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot);
				}

				PetListElement slot = m_dicPetList[delete.nSlot];
				m_dicPetList.Remove(delete.nSlot);
				m_listPetList.Remove(slot);

				m_CurDisplayingInfoOnPetInfoDlg.Element = null;
				m_CurDisplayingInfoOnPetInfoDlg = null;
			}

			if( m_PetDlg != null)
				m_PetDlg.Close();
			
			if(m_PetManageDlg != null)
				m_PetManageDlg.Open();
		}

		if(delete.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetDeleteResult: delete.eResult = " + delete.eResult);
			return;
		}
	}

	// - equip -
	public void Recv_PetEquipItemResult(byte[] _packet)
	{
		m_PacketSend = false;

		body_SC_PET_EQUIP_ITEM_RESULT equip = new body_SC_PET_EQUIP_ITEM_RESULT();
		equip.PacketBytesToClass( _packet);

		if(equip.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetEquipItemResult: equip.eResult = " + equip.eResult);
			return;
		}
	}

	public void Recv_PetUnEquipItemResult(byte[] _packet)
	{
		body_SC_PET_UNEQUIP_ITEM_RESULT unequip = new body_SC_PET_UNEQUIP_ITEM_RESULT();
		unequip.PacketBytesToClass( _packet);

		switch(unequip.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			if(m_PetDlg != null) m_PetDlg.ReleaseEquip();
			m_PetInfo.itemView.nItemTableIdx = 0;
//			AsEntityManager.Instance.MessageToPlayer(new Msg_PetItemView(new sITEMVIEW()));
			break;
		case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
			AsHudDlgMgr.Instance.SetMsgBox(
				AsNotify.Instance.MessageBox(
				AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2757),
				AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_ERROR));
			break;
		default:
			Debug.LogError("AsPetManager:: Recv_PetUnEquipItemResult: unequip.eResult = " + unequip.eResult);
			break;
		}
	}

	// - act -
	public void Recv_PetNameChangeResult(byte[] _packet)
	{
		body_SC_PET_NAME_CHANGE_RESULT change = new body_SC_PET_NAME_CHANGE_RESULT();
		change.PacketBytesToClass(_packet);

		if(change.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetNameChangeResult: change.eResult = " + change.eResult);
			return;
		}
	}

	public void Recv_PetUpgradeResult(byte[] _packet)
	{
		body_SC_PET_UPGRADE_RESULT upgrade = new body_SC_PET_UPGRADE_RESULT();
		upgrade.PacketBytesToClass( _packet);

		if(upgrade.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetUpgradeResult: upgrade.eResult = " + upgrade.eResult);
			return;
		}

		if(m_Dlt_Send_Upgrade != null)
			m_Dlt_Send_Upgrade();

		_HatchPerform(upgrade);

		// need refresing upgrade dlg
	}
	void _HatchPerform(body_SC_PET_UPGRADE_RESULT _upgrade)
	{
		UIPetPerform perform = _HatchGeneration();
		perform.Open(_upgrade);
	}

	public void Recv_PetItemComposeResult(byte[] _packet)
	{
		m_PacketSend = false;

		body_SC_PET_ITEM_COMPOSE_RESULT compose = new body_SC_PET_ITEM_COMPOSE_RESULT();
		compose.PacketBytesToClass( _packet);

		if(compose.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetItemComposeResult: compose.eResult = " + compose.eResult);
			return;
		}

		if(m_Dlt_Send_Item_Compose != null)
			m_Dlt_Send_Item_Compose();

		m_CurDisplayingInfoOnPetInfoDlg.nLevel = compose.nLevel;
		m_CurDisplayingInfoOnPetInfoDlg.nExp = compose.nExp;

		if(m_PetSynthesisDlg != null)
			m_PetSynthesisDlg.Open(true);

		if(m_PetManageDlg != null)
			m_PetManageDlg.Open();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6038_OptionChange_Complete", Vector3.zero, false);

		if(m_PetInfo.nPetUniqSlot == m_CurDisplayingInfoOnPetInfoDlg.nPetUniqSlot)
			AsEntityManager.Instance.MessageToPlayer(new Msg_PetEffectIndicate("FX/Effect/COMMON/Fx_Pet_Common"));
	}

	public void Recv_PetSlotExtendResult(byte[] _packet)
	{
		body_SC_PET_EXTEND_SLOT_RESULT extend = new body_SC_PET_EXTEND_SLOT_RESULT();
		extend.PacketBytesToClass( _packet);

		if(extend.eResult != eRESULTCODE.eRESULT_SUCC)
		{
			Debug.LogError("AsPetManager:: Recv_PetSlotExtendResult: extend.eResult = " + extend.eResult);
			return;
		}

		Debug.Log("AsPetManager:: Recv_PetSlotExtendResult: extend.nLine = " + extend.nLine);

		m_ExtendLine = extend.nLine;
		if(m_PetManageDlg != null) m_PetManageDlg.Open();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6000_EFF_Window_Open", Vector3.zero, false);
	}
	#endregion
	#region - public -
	public void UnSelectManageSlot()
	{
		if(m_CurDisplayingInfoOnPetInfoDlg.Element != null && m_PetManageDlg)
		{
			m_PetManageDlg.SetSlotToBase(AsPetManager.Instance.CurPetListElement);
		}
	}

	public void PetManagerBtnClicked()
	{
		GameObject obj = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_PetManagement") ) as GameObject;
		m_PetManageDlg = obj.GetComponentInChildren<UIPetManageDlg>();
		m_PetManageDlg.Open();
	}
	
	bool m_ReserveLoggedIn = false;
	public void Reserve_LoggedIn()
	{
		m_PetInfo.nPetTableIdx = 0;
		
		m_ReserveLoggedIn = true;
	}
	
	public void PlayerLogin()
	{
		if(isPetExist == true && m_ReserveLoggedIn == true)
		{
			SendScriptWord(Tbl_PetScript_Record.eType.Login);
			m_ReserveLoggedIn = false;
		}
	}
	
	public void PlayerDeath()
	{
		SendScriptWord(Tbl_PetScript_Record.eType.Dead);
	}
	
	public void PlayerEnterWorld()
	{
		if(isPetExist == true)
		{
			AsUserEntity player = AsEntityManager.Instance.UserEntity;
			PetAppearData appear = new PetAppearData( player, m_PetInfo);
			player.HandleMessage( new Msg_PetDataIndicate( appear));
//		
//			AsEntityManager.Instance.PetAppear( appear);
		}
	}
	
	public bool CheckPetEquipEnable( RealItem _item)
	{
		if( _item.item.ItemData.needClass != eCLASS.PET)
			return false;
		
		if( m_PetInfo.nPetTableIdx <= 0)
			return false;

		return true;
	}
	
	public bool CheckPetEquipEnable( ItemData _data)
	{
		if( _data.needClass != eCLASS.PET)
			return true;
		
		if( m_PetInfo.nPetTableIdx == 0)
			return true;
		
//		if( m_PetInfo.GetPetRecord().Class != _data.petClass_)
//			return false;
//		else
			return true;
	}
	
	public void SetPetEquip( sITEMVIEW _slot)
	{
		m_PetInfo.itemView.nItemTableIdx = _slot.nItemTableIdx;
		m_PetInfo.itemView.nStrengthenCount = _slot.nStrengthenCount;
		
		sITEMVIEW itemView = new sITEMVIEW();
		itemView.nItemTableIdx = _slot.nItemTableIdx;
		itemView.nStrengthenCount = _slot.nStrengthenCount;
		
		if( m_PetDlg != null)
			m_PetDlg.ChangeEquip( itemView);
	}
	
	public void ReleaseEquip()
	{
		Item item = ItemMgr.ItemManagement.GetItem( m_PetInfo.itemView.nItemTableIdx);
		if( item == null)
			return;
		
		int iTargetSlot = ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot();
		if( -1 != iTargetSlot)
		{
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 365),
										null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}
			
			AsSoundManager.Instance.PlaySound( item.ItemData.m_strUseSound, Vector3.zero, false);
			AsCommonSender.SendMoveItem( AsPetManager.ePETINVENTORY, iTargetSlot);
		}
		else
		{
			AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String( 20),
							null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
		}
		
		m_PetInfo.itemView.nItemTableIdx = 0;
		m_PetInfo.itemView.nStrengthenCount = 0;
		
		sITEMVIEW itemView = new sITEMVIEW();
		itemView.nItemTableIdx = 0;
		itemView.nStrengthenCount = 0;
		
		if( m_PetDlg != null)
			m_PetDlg.ChangeEquip( itemView);
	}
	
	public void ClosePetDlgByOtherWindow()
	{
		if( m_PetDlg != null) m_PetDlg.Close();
		if( m_PetManageDlg != null) m_PetManageDlg.Close();
		if( m_PetUpgradeDlg != null) m_PetUpgradeDlg.Close();
		if( m_PetSynthesisDlg != null) m_PetSynthesisDlg.Close();
	}

	public void OpenPetUpgradeDlg()
	{
		// low level
		int maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value;
		if(m_CurDisplayingInfoOnPetInfoDlg.Element.Level < maxLv)
		{
			AsMessageBox msgBox = null;
			msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2758),
			                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.gameObject.AddComponent<UIPetPopup>();
			return;
		}

		/// max star grade
		if(m_CurDisplayingInfoOnPetInfoDlg.Element.PetRecord.StarGrade == s_MaxStarGrade)
		{
			AsMessageBox msgBox = null;
			msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2759),
			                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.gameObject.AddComponent<UIPetPopup>();
			return;
		}

		GameObject obj = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_PetUpgrade") ) as GameObject;
		m_PetUpgradeDlg = obj.GetComponentInChildren<UIPetUpgradeDlg>();

		if(m_PetManageDlg != null)
		{
			m_PetUpgradeDlg.Open(m_CurDisplayingInfoOnPetInfoDlg.Element);
			m_PetManageDlg.RefreshSlotsOnUpgradeDlgOpen();
		}

		if(m_PetDlg != null)
			m_PetDlg.Close();
	}

	public void PetManageSlotClicked(UIPetManageSlot _slot)
	{
		switch(GetDlgState())
		{
		case eDlgState.NONE:
			Send_PetInfo(_slot);
			break;
		case eDlgState.Info:
			Send_PetInfo(_slot);
			break;
		case eDlgState.Upgrade:
			m_PetUpgradeDlg.ManageSlotClicked(_slot);
			break;
		case eDlgState.Synthesis:
			break;
		}
	}

	public bool CheckInfoViewingPetSummoning()
	{
		if(isPetExist == false)
			return false;

		if(m_PetInfo.nPetUniqSlot == m_CurDisplayingInfoOnPetInfoDlg.Element.PetSlot)
			return true;
		else
			return false;
	}

	public bool CheckPetSummoning(PetListElement _element)
	{
		if(_element == null)
			return false;

		if(isPetExist == false)
			return false;
		
		if(m_PetInfo.nPetUniqSlot == _element.PetSlot)
			return true;
		else
			return false;
	}

	public void UnLockSlot()
	{
		int curLockedPage = (m_ExtendLine) / UIPetManageDlg.s_LineCount + 1;
		int cost = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("Petslot" + curLockedPage + "P_Open").Value;

		AsMessageBox msgBox = null;
		if(cost > AsUserInfo.Instance.nMiracle)
		{
			msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String(264), this, "_OpenCashShop",
			                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.gameObject.AddComponent<UIPetPopup>();
			return;
		}

		msgBox = AsNotify.Instance.CashMessageBox((long)cost, AsTableManager.Instance.GetTbl_String( 126), AsTableManager.Instance.GetTbl_String(2742), this,
		                                          "UnLockSlot_Confirmed", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);

		msgBox.gameObject.AddComponent<UIPetPopup>();
	}
	void _OpenCashShop()
	{
		AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.CHARGE_MIRACLE, eCashStoreSubCategory.NONE, 4);
	}

	void UnLockSlot_Confirmed()
	{
		Send_SlotExtend(m_ExtendLine + 1);
	}

	public static void SetPopup( bool _active)
	{
		m_Popup = _active;
	}

	public void OpenPetSynthesisDlg()
	{
		int maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value;
		if(m_CurDisplayingInfoOnPetInfoDlg.Element.Level >= maxLv)
		{
			AsMessageBox msgBox = null;
			msgBox = AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2760),
			                                      AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			msgBox.gameObject.AddComponent<UIPetPopup>();
			return;
		}

		GameObject obj = Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_PetSynthesis") ) as GameObject;
		m_PetSynthesisDlg = obj.GetComponentInChildren<UIPetSynthesisDlg>();
		m_PetSynthesisDlg.Open();
		
		if(m_PetManageDlg != null)
			m_PetManageDlg.Close();
		
		if(m_PetDlg != null)
			m_PetDlg.Close();

		if(AsHudDlgMgr.Instance.IsOpenInven == false)
			AsHudDlgMgr.Instance.OpenInven();
	}

	public void InventoryClosed()
	{
//		if( m_PetDlg != null) m_PetDlg.Close();
//		if( m_PetManageDlg != null) m_PetManageDlg.Close();
//		if( m_PetUpgradeDlg != null) m_PetUpgradeDlg.Close();
		if( m_PetSynthesisDlg != null) m_PetSynthesisDlg.Close();
	}

	public void ItemProc(Item _item)
	{
		if(Item.CheckItem_PetFood(_item) == true)
		{
			SendScriptWord(Tbl_PetScript_Record.eType.Eat);
			AsEntityManager.Instance.MessageToPlayer(new Msg_Pet_Feeding());
		}
	}

	RealItem m_RealItem;
	public void PetFoodProc(RealItem _realItem)
	{
		m_RealItem = _realItem;

		string title = AsTableManager.Instance.GetTbl_String( 126);
		string content = AsTableManager.Instance.GetTbl_String(2293);
		AsHudDlgMgr.Instance.SetMsgBox(AsNotify.Instance.MessageBox( title, content,
		                                                            this, "_PetFoodConfirmed", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));
	}

	void _PetFoodConfirmed()
	{
		if(m_RealItem == null)
		{
			Debug.LogError("AsPetManager:: _PetFoodConfirmed: no real item is set");
			return;
		}

		AsCommonSender.SendUseItem( m_RealItem.getSlot );
	}
	#endregion
	#region - ui -
//	void Dlt_PetInfoBtn(ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			Send_PetInfo();
//		}
//	}
	#endregion
	#region - delegate -
	int Dlt_SortPetList(PetListElement _left, PetListElement _right) 
	{
		if(_left.PetRecord.StarGrade > _right.PetRecord.StarGrade)
			return -1;
		else if(_left.PetRecord.StarGrade < _right.PetRecord.StarGrade)
			return 1;
		else
		{
			if(_left.Level > _right.Level)
				return -1;
			else if(_left.Level < _right.Level)
				return 1;
			else
			{
				if(_left.PetSlot > _right.PetSlot)
					return -1;
				else if(_left.PetSlot < _right.PetSlot)
					return 1;
			}
		}

		return 0;
 	}
	#endregion
	#region - method -
	void SendScriptWord(Tbl_PetScript_Record.eType _type)
	{
		if(isPetExist == true)
		{
			Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(m_PetInfo.nPersonality);
			string word = scriptRec.GetScriptString(_type);
			AsEntityManager.Instance.MessageToPlayer(new Msg_Pet_Script( word));
		}
	}

	eDlgState GetDlgState()
	{
		if(m_PetDlg != null)
		{
			if(m_PetUpgradeDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetDlg is instantiated, but m_PetUpgradeDlg is also instantiated");
			if(m_PetSynthesisDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetDlg is instantiated, but m_PetSynthesisDlg is also instantiated");

			return eDlgState.Info;
		}

		if(m_PetUpgradeDlg != null)
		{
			if(m_PetDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetUpgradeDlg is instantiated, but m_PetDlg is also instantiated");
			if(m_PetSynthesisDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetUpgradeDlg is instantiated, but m_PetSynthesisDlg is also instantiated");
			
			return eDlgState.Upgrade;
		}

		if(m_PetSynthesisDlg != null)
		{
			if(m_PetDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetSynthesisDlg is instantiated, but m_PetDlg is also instantiated");
			if(m_PetUpgradeDlg != null)
				Debug.LogError("AsPetManager:: GetDlgStatte: m_PetSynthesisDlg is instantiated, but m_PetUpgradeDlg is also instantiated");
			
			return eDlgState.Synthesis;
		}

//		Debug.Log("AsPetManager:: GetDlgStatte: no dlg is instantiated");
		return eDlgState.NONE;
	}

	PetListElement GetPetListElement(int _idx)
	{
		if(m_dicPetList.ContainsKey(_idx) == true)
			return m_dicPetList[_idx];
		else
		{
			Debug.LogError("AsPetManager:: PetListElement: inavalid index. _idx = " + _idx);
			return null;
		}
	}
	#endregion
	#region - test -
//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(900, 500, 100, 40), "pet list") == true)
//		{
//			PetManagerBtnClicked();
//		}
//	}
	#endregion
}

public class PetInfo
{
	public int		nPetUniqSlot;
	public int		nPetTableIdx;
	public int		nPersonality;
	public byte[]		szPetName = new byte[AsGameDefine.ePET_NAME + 1];
	ePET_HUNGRY_STATE m_Hungry; public ePET_HUNGRY_STATE Hungry{get{return m_Hungry;}}
	public sPETSKILL[]	sSkill = new sPETSKILL[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX];
	public sITEMVIEW itemView = new sITEMVIEW();

	public PetListElement Element = new PetListElement();

	public PetInfo(){}

	public void SetValue( body_SC_PET_CALL _load)
	{
		nPetUniqSlot = _load.nSlot;
		nPetTableIdx = _load.nPetTableIdx;
		nPersonality = _load.nPersonality;
		szPetName = _load.szPetName;

		m_Hungry = _load.eHungryState;
		itemView.nItemTableIdx = _load.sViewItem.nItemTableIdx;
		itemView.nStrengthenCount = _load.sViewItem.nStrengthenCount;
		
		m_PetRecord = AsTableManager.Instance.GetPetRecord( nPetTableIdx);
	}

	public int nLevel;
	public int nExp;
	public PetInfo( body_SC_PET_INFO _info)
	{
		nPetUniqSlot = _info.nSlot;
		nPetTableIdx = _info.nPetTableIdx;
		nPersonality = _info.nPetPersonality;
		szPetName = _info.szPetName;
		nLevel = _info.nLevel;
		nExp = _info.nExp;
//		m_Hungry = _info.nHungryPoint;

		sSkill = new sPETSKILL[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX];
		itemView.nItemTableIdx = _info.sItem.nItemTableIdx;
		itemView.nStrengthenCount = _info.sItem.nStrengthenCount;
		
		m_PetRecord = AsTableManager.Instance.GetPetRecord( nPetTableIdx);
		if( m_PetRecord == null) Debug.LogError( "PetInfo:: SetValue: there is no pet record. id = " + nPetTableIdx);
	}
	
	Tbl_Pet_Record m_PetRecord;
	public Tbl_Pet_Record GetPetRecord()
	{
		return m_PetRecord;
	}

	public string GetCurItemName()
	{
		Item item = ItemMgr.ItemManagement.GetItem(itemView.nItemTableIdx);
		if(item == null)
			return "undefined index(" + itemView.nItemTableIdx + ")";
		
		string str = item.ItemData.GetGradeColor() + AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);
		if(itemView.nStrengthenCount > 0)
			str = Color.white + "+" + itemView.nStrengthenCount + " " + str;
		
		return str;
	}

	public void SetHungry(body_SC_PET_HUNGRY _hungry)
	{
		m_Hungry = _hungry.eHungryState;
	}

	public void SetHungry(ePET_HUNGRY_STATE _hungry)
	{
		m_Hungry = _hungry;
	}

	public void ClearItem()
	{
		itemView.nItemTableIdx = 0;
		itemView.nStrengthenCount = 0;
	}

	public void Clear()
	{
		nPetUniqSlot = 0;
		nPetTableIdx = 0;
		nPersonality = 0;
		szPetName = new byte[0];
		nLevel = 0;
		nExp = 0;
		
		sSkill = new sPETSKILL[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX];
		itemView.nItemTableIdx = 0;
		itemView.nStrengthenCount = 0;
		
		m_PetRecord = null;
	}
};

public class PetListElement
{
	// - base -
	int m_PetTableIdx; public int PetTableIdx{get{return m_PetTableIdx;}}
	int m_PetSlot; public int PetSlot{get{return m_PetSlot;}}
	string m_PetName = "undefined"; public string PetName{get{return m_PetName;}}
	int m_Level; public int Level{get{return m_Level;}}

	// - extra -
	Tbl_Pet_Record m_PetRecord; public Tbl_Pet_Record PetRecord{get{return m_PetRecord;}}

	public PetListElement(){}

	public PetListElement(body2_SC_PET_LIST _list)
	{
		m_PetTableIdx = _list.nPetTableIdx;
		m_PetSlot = _list.nPetSlot;
		m_PetName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString(_list.szPetName));
		m_Level = _list.nLevel;

		if(m_PetTableIdx > 0)
			m_PetRecord = AsTableManager.Instance.GetPetRecord(m_PetTableIdx);
	}

	public PetListElement(body_SC_PET_SLOT_CHANGE _change)
	{
		ChangeElement(_change);
	}

	public void ChangeElement(body_SC_PET_SLOT_CHANGE _change)
	{
		m_PetTableIdx = _change.nPetTableIdx;
		m_PetSlot = _change.nPetSlot;
		m_PetName = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString(_change.szPetName));
		m_Level = _change.nLevel;

		if(m_PetTableIdx > 0)
			m_PetRecord = AsTableManager.Instance.GetPetRecord(m_PetTableIdx);
	}

	public bool CheckUpgradable(PetListElement _element)
	{
		if(m_Level == _element.Level &&
		   m_PetRecord.StarGrade == _element.PetRecord.StarGrade &&
		   m_Level == (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("PetMaxLevel").Value)
			return true;
		else
			return false;
	}
}
