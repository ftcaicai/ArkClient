using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsEmotionManager : MonoBehaviour
{
	public enum eCHAT_FILTER {eCHAT_FILTER_LOCAL = 0, eCHAT_FILTER_PARTY, eCHAT_FILTER_GUILD}

	#region - singleton -
	private static AsEmotionManager instance = null;
	public static AsEmotionManager Instance
	{
		get	{ return instance; }
	}
	#endregion

	AsEmoticonPanel m_EmoticonPanel = null;
	
	[SerializeField] float m_RefreshRate = 0.5f;
//	Dictionary<eAutoCondition, float> m_dicConditionCool = new Dictionary<eAutoCondition, float>();
	float[] m_ConditionCool = new float[(int)eAutoCondition.NONE];

//	eEmoticonType m_CurTab = eEmoticonType.Normal; public eEmoticonType curTab{get{return m_CurTab;}}

	void Awake()
	{
		#region - singleton -
		instance = this;
		#endregion
		
//		foreach(float node in m_ConditionCool)
//		{
//			node = 0f;
//		}
		
		for(int i=0; i<m_ConditionCool.Length; ++i)
		{
			m_ConditionCool[i] = 0;
		}
	}
	
	void OnEnable()
	{
		StartCoroutine(Event_pCondition_PartyHP40_CR());
//		StartCoroutine(Event_pCondition_PositionRunOut_CR);
//		StartCoroutine(Event_Condition_FirstAttack_CR);
//		StartCoroutine(Event_Condition_PartyReject_CR);
		StartCoroutine(Event_pCondition_PartyMP10_CR());
//		StartCoroutine(Event_pCondition_BuffCancelALL_CR);
//		StartCoroutine(Event_Condition_Death_CR);
//		StartCoroutine(Event_Condition_GetRareItem_CR);
//		StartCoroutine(Event_Condition_LevelUp_CR);
//		StartCoroutine(Event_Condition_Trade_CR);
//		StartCoroutine(Event_Condition_GetBuff_CR);
	}
	
	#region - condition check -
	bool CheckEnableConditon(eAutoCondition _condition)
	{
//		if(m_dicConditionCool.ContainsKey(_condition) == false)
//			return true;
//		else if(m_dicConditionCool[_condition] <= 0f)
//			return true;
//		else
//			return false;
				
		if(m_ConditionCool[(int)_condition] <= 0f)
			return true;
		else
			return false;
	}
	
	void ConditionCoolProcess(Tbl_Emoticon_Record _record, eAutoCondition _condition)
	{
//		StartCoroutine(ConditionCoolProcess_CR(_record, _condition));
		
//		if(m_dicConditionCool.ContainsKey(_condition) == false)
//			m_dicConditionCool.Add(_condition, _record.GetConditionCool(_condition));
//		else
//			m_dicConditionCool[_condition] = _record.GetConditionCool(_condition);
		
		m_ConditionCool[(int)_condition] = _record.GetConditionCool(_condition) * 0.001f;
	}
	
//	IEnumerator ConditionCoolProcess_CR(Tbl_Emoticon_Record _record, eAutoCondition _condition)
//	{
//		if(m_dicConditionCool.ContainsKey(_condition) == false)
//			m_dicConditionCool.Add(_condition, _record.GetConditionCool(_condition));
//		else
//			m_dicConditionCool[_condition] = _record.GetConditionCool(_condition);
//		
//		yield return new WaitForSeconds(_record.GetConditionCool(_condition) * 0.001f);
//		
//		m_dicConditionCool[_condition] = true;
//	}
	#endregion
	
	#region - real time check -
	IEnumerator Event_pCondition_PartyHP40_CR()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_RefreshRate);
			
			if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
				continue;
			
			if(AsPartyManager.Instance.IsPartying == false)
				continue;
			
			float totalCurHp = 0;
			float totalMaxHp = 0;
			foreach(KeyValuePair<uint, AS_PARTY_USER> pair in AsPartyManager.Instance.GetPartyMemberList())
			{
				totalCurHp += pair.Value.fHpCur;
				totalMaxHp += pair.Value.fHpMax;
			}
			
			totalCurHp += AsEntityManager.Instance.UserEntity.GetProperty<float>(eComponentProperty.HP_CUR);
			totalMaxHp += AsEntityManager.Instance.UserEntity.GetProperty<float>(eComponentProperty.HP_MAX);
			
			if(totalCurHp / totalMaxHp < 0.4f)
			{
				Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.pCondition_PartyHP40);
				if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.pCondition_PartyHP40))
				{
					if(CheckEnableConditon(eAutoCondition.pCondition_PartyHP40) == true)
					{
						Event_pCondition_PartyHP40();
					}
				}
			}
		}
	}
	IEnumerator Event_pCondition_PartyMP10_CR()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_RefreshRate);
			
			if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
				continue;
			
			if(AsPartyManager.Instance.IsPartying == false)
				continue;
			
			float curMp = AsEntityManager.Instance.UserEntity.GetProperty<float>(eComponentProperty.MP_CUR);
			float maxMp = AsEntityManager.Instance.UserEntity.GetProperty<float>(eComponentProperty.MP_MAX);
			
			if(curMp / maxMp < 0.1f)
			{
				Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.pCondition_PartyMP10);
				if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.pCondition_PartyMP10))
				{
					if(CheckEnableConditon(eAutoCondition.pCondition_PartyHP40) == true)
					{
						Event_pCondition_PartyMP10();
					}
				}
			}
		}
	}
	#endregion

	void Start()
	{
	}

	void Update()
	{
//		foreach(KeyValuePair<eAutoCondition, float> pair in m_dicConditionCool)
//		{
//			if(pair.Value >= 0)
//				m_dicConditionCool[pair.Key] -= Time.deltaTime;
////				pair.Value =  Time.deltaTime;
//		}
		
//		foreach(float node in m_ConditionCool)
//		{
//			if(node > 0)
//				node -= Time.deltaTime;
//		}
		
		for(int i=0; i<m_ConditionCool.Length; ++i)
		{
			if(m_ConditionCool[i] > 0f)
				m_ConditionCool[i] -= Time.deltaTime;
		}
	}
	
	void OnDisable()
	{
		
	}

	public void SystemProcess( uint uniqIdx, string _str)
	{
//		Debug.Log( "AsEmotionManager::SystemProcess: / is input.");

		AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
		if( user.UniqueId == uniqIdx && _str.Contains( "/?") == true)
		{
//			Debug.Log( "AsEmotionManager::SystemProcess: /? is processing.");

			if( user.GetProperty<bool>( eComponentProperty.COMBAT) == false)
			{
				List<string> listString = AsTableManager.Instance.GetCommandList();
				foreach( string command in listString)
				{
					AsChatManager.Instance.InsertChat( command, eCHATTYPE.eCHATTYPE_SYSTEM);
				}
			}
			else
			{
				string str = AsTableManager.Instance.GetTbl_String(345);
				AsChatManager.Instance.InsertChat( str, eCHATTYPE.eCHATTYPE_SYSTEM);
			}
		}
	}

	public void EmotionProcess( body_SC_CHAT_WITH_BALLOON_RESULT _result)
	{
//		_EmotionProcess( _result.nCharUniqKey, _result.kMessage.szMsg);
	}

	void _EmotionProcess( uint _key, string _str)
	{
		Tbl_Emotion_Record record = AsTableManager.Instance.GetValidEmotionRecord( _str);
		if( record != null)
		{
			Msg_EmotionIndicate emotion = new Msg_EmotionIndicate( record);
			AsEntityManager.Instance.DispatchMessageByUniqueKey( _key, emotion);
		}

//		if( _str.Contains( "gangnam") == true)//$ e.e
//		{
//			AsUserEntity user = AsEntityManager.Instance.GetUserEntityByUniqueId( _key);
//			if( user != null && user.GetProperty<bool>( eComponentProperty.COMBAT) == false)
//			{
//				eCLASS __class = user.GetProperty<eCLASS>( eComponentProperty.CLASS);
//				eGENDER gender = user.GetProperty<eGENDER>( eComponentProperty.GENDER);
//				if( __class == eCLASS.DIVINEKNIGHT && gender == eGENDER.eGENDER_FEMALE)
//				{
//					AsEntityManager.Instance.DispatchMessageByUniqueKey( _key, new Msg_AnimationIndicate( "gangnam"));
//				}
//			}
//		}
	}

	public void ToggleEmoticonPanel()
	{
		if( m_EmoticonPanel != null)
			CloseEmoticonPanel();
		else
			OpenEmoticonPanel();
	}

	void OpenEmoticonPanel()
	{
		if( m_EmoticonPanel != null)
			return;

		if( m_Block == true)
			return;

		GameObject obj = Instantiate( Resources.Load( "UI/AsGUI/GUI_ChatMacro_New")) as GameObject;
		m_EmoticonPanel = obj.GetComponentInChildren<AsEmoticonPanel>();

		if( ArkQuestmanager.instance.CheckHaveOpenUIType( OpenUIType.OPEN_EMOTICON) != null)
			AsCommonSender.SendClearOpneUI( OpenUIType.OPEN_EMOTICON);

//		switch( m_CurTab)
//		{
//		case eEmoticonType.Hunt:
//			m_EmoticonPanel.ActivateBtnHunt();
//			m_CurTab = eEmoticonType.Hunt;
//			break;
//		case eEmoticonType.Normal:
//			m_EmoticonPanel.ActivateBtnNormal();
//			m_CurTab = eEmoticonType.Normal;
//			break;
//		}
	}

	public void CloseEmoticonPanel()
	{
//		Debug.Log( "AsEmoticonManager::CloseEmoticonPanel: " + m_EmoticonPanel);
//		Debug.Log( "AsEmoticonManager::CloseEmoticonPanel: " + m_EmoticonPanel.transform);
//		Debug.Log( "AsEmoticonManager::CloseEmoticonPanel: " + m_EmoticonPanel.transform.parent);
//		Debug.Log( "AsEmoticonManager::CloseEmoticonPanel: " + m_EmoticonPanel.transform.parent.gameObject);

		if( m_EmoticonPanel != null)
			Destroy( m_EmoticonPanel.transform.parent.gameObject);

		m_EmoticonPanel = null;
	}
	
	public bool IsOpenEmotionPanel()
	{
		if( null == m_EmoticonPanel)
			return false;
		return true;
	}

	bool m_Block = false;
	public void BlockPanel()
	{
		m_Block = true;
	}

	public void ReleaseBlock()
	{
		m_Block = false;
	}

	public void Recv_Emoticon( body_SC_CHAT_EMOTICON_RESULT _result)
	{
		if( _result.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			if( AsSocialManager.Instance.SocialData.GetBlockOut( _result.nUserUniqKey) != null)
			{
				Debug.Log( "AsEmotionManager::Recv_Emoticon: blocked out user[" + _result.nUserUniqKey + "]");
				return;
			}

//			Msg_EmoticonIndicate emoticon = new Msg_EmoticonIndicate( _result);
//			AsEntityManager.Instance.DispatchMessageByUniqueKey( _result.nCharUniqKey, emoticon);

			AsUserEntity entity = AsEntityManager.Instance.GetUserEntityByUniqueId( _result.nCharUniqKey);
			eGENDER gender = entity.GetProperty<eGENDER>( eComponentProperty.GENDER);

			Tbl_Emoticon_Record record = AsTableManager.Instance.GetTbl_Emoticon_Record( _result.nIndex);
			string chat = AsTableManager.Instance.GetTbl_String(record.GetRandomString( gender));
			
//			Debug.LogWarning("AsEmotionManager::Recv_Emoticon: _result.nIndex = " + _result.nIndex);
//			Debug.LogWarning("AsEmotionManager::Recv_Emoticon: chat = " + chat);

			string name = "[" + AsUtil.GetRealString( System.Text.Encoding.UTF8.GetString( _result.szCharName)) + "]:";// "[Unknown_Player]:";
//			if( entity != null) name = "[" + entity.GetProperty<string>( eComponentProperty.NAME) + "]:";

			switch( _result.eFilter)
			{
			case AsEmotionManager.eCHAT_FILTER.eCHAT_FILTER_LOCAL:
				AsChatManager.Instance.ShowChatBalloon( _result.nCharUniqKey, chat, eCHATTYPE.eCHATTYPE_PUBLIC);
				AsChatManager.Instance.InsertChat( name + chat, eCHATTYPE.eCHATTYPE_PUBLIC);
				break;
			case AsEmotionManager.eCHAT_FILTER.eCHAT_FILTER_PARTY:
				AsChatManager.Instance.ShowChatBalloon( _result.nCharUniqKey, chat, eCHATTYPE.eCHATTYPE_PARTY);
				AsChatManager.Instance.InsertChat( name + chat, eCHATTYPE.eCHATTYPE_PARTY);
				break;
			case AsEmotionManager.eCHAT_FILTER.eCHAT_FILTER_GUILD:
				AsChatManager.Instance.ShowChatBalloon( _result.nCharUniqKey, chat, eCHATTYPE.eCHATTYPE_GUILD);
				AsChatManager.Instance.InsertChat( name + chat, eCHATTYPE.eCHATTYPE_GUILD);
				break;
			}

			#region - exceptional case -
			Debug.Log( "AsEmotionManager::Recv_Emoticon: record.Index = " + record.Index);

			switch( record.Index)
			{
			case 21:
				entity.HandleMessage( new Msg_Emoticon_Seat_Indicate( record));
				break;
			}
			#endregion

			QuestMessageBroadCaster.BrocastQuest( QuestMessages.QM_USE_CHAT_MACRO, new AchUseEmoticon( _result.nIndex, 1));
			
			
		}
		else
		{
			if( eRESULTCODE.eRESULT_FAIL_CHAT_NOTHING_PARTY == _result.eResult)
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1477), eCHATTYPE.eCHATTYPE_SYSTEM);
			else if( eRESULTCODE.eRESULT_FAIL_CHAT_NOTHING_GUILD == _result.eResult)
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(1478), eCHATTYPE.eCHATTYPE_SYSTEM);
			else
				Debug.LogWarning( "AsEmotionManager::EmoticonProcess: result is " + _result.eResult);
		}
	}

	public void Request_Emoticon( int _idx)
	{
		#region - exceptional case -
		switch( _idx + 1)
		{
		case 21:
			AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
			if( user.GetProperty<bool>( eComponentProperty.COMBAT) == true)
			{
				AsMyProperty.Instance.AlertEmoticon_Seat();
				return;
			}
			break;
		default:
			break;
		}
		#endregion

		eCHAT_FILTER filterType = eCHAT_FILTER.eCHAT_FILTER_LOCAL;
		#region - adjust filter type -
		switch( AsChatFullPanel.Instance.filterType)
		{
		case CHAT_FILTER_TYPE.General:
			break;
		case CHAT_FILTER_TYPE.Party:
			filterType = eCHAT_FILTER.eCHAT_FILTER_PARTY;
			break;
		case CHAT_FILTER_TYPE.Guild:
			filterType = eCHAT_FILTER.eCHAT_FILTER_GUILD;
			break;
		default:
			Debug.LogWarning( "AsEmotionManager::Request_Emoticon: invalid type requested = " + AsChatFullPanel.Instance.filterType);
			break;
		}
		#endregion
		body_CS_CHAT_EMOTICON emoticon = new body_CS_CHAT_EMOTICON( filterType, _idx + 1);
		AsCommonSender.Send( emoticon.ClassToPacketBytes());
		
//		Debug.LogWarning("AsEmotioncManager::Request_Emoticon: _idx + 1 = " + (_idx + 1));
	}

//	public void SetCurTab( eEmoticonType _type)
//	{
//		m_CurTab = _type;
//	}
	
	#region - condition -
	GameObject m_Button = null;
	Tbl_Emoticon_Record m_ReservedRecord = null;
	void ConditionProcess(Tbl_Emoticon_Record _record, eAutoCondition _condition)
	{
		if(AsGameMain.GetOptionState(OptionBtnType.OptionBtnType_AutoChat) == false)
			return;
		
		if(_record == null)
			return;
		
		if(CheckEnableConditon(_condition) == true)
			ConditionCoolProcess(_record, _condition);
		else
			return;
		
		switch(_record.GetConditionActivation(_condition))
		{
		case eActivationType.NONE:
			Debug.LogWarning("AsEmoticonManager::ConditionProcess: cannot be process in this condition = " + eActivationType.NONE);
			break;
		case eActivationType.Auto:
			Request_Emoticon(_record.Index - 1);
			break;
		case eActivationType.Choice:
			m_ReservedRecord = _record;
			
			// button init
			m_Button = Instantiate(ResourceLoad.LoadGameObject("UI/Optimization/Prefab/GUI_Balloon_ChatMacro")) as GameObject;
			AsChatMacroContainer container = m_Button.GetComponent<AsChatMacroContainer>();
			container.ButtonActivate(_record.Index - 1);//, OnIconClicked);
			
//			string path = "Btn_";
//			if(m_ReservedRecord.Section == eEmoticonType.Normal)
//				path += "Normal_" + (m_ReservedRecord.Index + AsEmoticonPanel.s_BtnCount - 1);
//			else
//				path += "Hunting_" + (m_ReservedRecord.Index - 1);
			
//			Transform buttons = m_Button.transform.Find("Buttons");
//			Transform child = buttons.transform.Find(path);
//			child.parent = m_Button.transform;
//			Destroy(buttons.gameObject);
//			Destroy(m_Button, 3f);
			
			AsSkillCoolTimeAlramDelegatorManager.Instance.DestroyAlarm();
			
//			UIButton btn = child.GetComponent<UIButton>();
//			btn.SetInputDelegate(OnIconClicked);
			
			break;
		}
	}
	
	public void ButtonClicked()
	{
		Request_Emoticon(m_ReservedRecord.Index - 1);
	}
	
	// EVENT
	void Event_pCondition_PartyHP40()
	{
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.pCondition_PartyHP40);
		ConditionProcess(record, eAutoCondition.pCondition_PartyHP40);
	}
	public void Event_pCondition_PositionRunOut()
	{
	}
	public void Event_Condition_FirstAttack()
	{
	}
	public void Event_Condition_PartyReject()
	{
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_PartyReject);
		ConditionProcess(record, eAutoCondition.Condition_PartyReject);
	}
	void Event_pCondition_PartyMP10()
	{
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.pCondition_PartyMP10);
		ConditionProcess(record, eAutoCondition.pCondition_PartyMP10);
	}
	void Event_pCondition_BuffCancelALL()
	{
		if(AsPartyManager.Instance.IsPartying == false)
			return;
		
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.pCondition_BuffCancelALL);
		if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.pCondition_BuffCancelALL))
		{
			if(CheckEnableConditon(eAutoCondition.pCondition_BuffCancelALL) == true)
			{
				ConditionProcess(record, eAutoCondition.pCondition_BuffCancelALL);
			}
		}
	}
	public void Event_Condition_Death()
	{
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_Death);
		if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.Condition_Death))
		{
			if(CheckEnableConditon(eAutoCondition.Condition_Death) == true)
			{
				ConditionProcess(record, eAutoCondition.Condition_Death);
			}
		}
	}
	public void Event_Condition_GetRareItem(body_SC_USER_EVENT_NOTIFY _data)
	{
		if(AsEntityManager.Instance.UserEntity.UniqueId == _data.nValue_2)
			return;
		
		if(AsEntityManager.Instance.GetUserEntityByUniqueId((uint)_data.nValue_2) == false)
			return;
		
		Item item = ItemMgr.ItemManagement.GetItem(_data.sItem.nItemTableIdx);
		if(item.ItemData.grade == Item.eGRADE.Rare ||
			item.ItemData.grade == Item.eGRADE.Epic ||
			item.ItemData.grade == Item.eGRADE.Ark)
		{
			Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_GetRareItem);
			if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.Condition_GetRareItem))
			{
				if(CheckEnableConditon(eAutoCondition.Condition_GetRareItem) == true)
				{
					ConditionProcess(record, eAutoCondition.Condition_GetRareItem);
				}
			}
		}
	}
	public void Event_Condition_LevelUp()
	{
//		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_LevelUp);
//		if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.Condition_LevelUp))
//		{
//			if(CheckEnableConditon(eAutoCondition.Condition_LevelUp) == true)
//			{
//				ConditionProcess(record, eAutoCondition.Condition_LevelUp);
//			}
//		}
	}
	public void Event_Condition_Trade()
	{
		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_Trade);
		if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.Condition_Trade))
		{
			if(CheckEnableConditon(eAutoCondition.Condition_Trade) == true)
			{
				ConditionProcess(record, eAutoCondition.Condition_Trade);
			}
		}
	}
	public void Event_Condition_GetBuff()
	{
//		Tbl_Emoticon_Record record = AsTableManager.Instance.GetEmoticonByCondition(eAutoCondition.Condition_GetBuff);
//		if(record != null && Random.Range(0, 1001) <= record.GetConditionProb(eAutoCondition.Condition_GetBuff))
//		{
//			if(CheckEnableConditon(eAutoCondition.Condition_GetBuff) == true)
//			{
//				ConditionProcess(record, eAutoCondition.Condition_GetBuff);
//			}
//		}
	}
	#endregion
	#region - public -
	int m_BenefitBuffCount = 0;
	public void IncreaseBenefitBuffCount()
	{
		m_BenefitBuffCount++;
	}
	
	public void DecreaseBenefitBuffCount()
	{
		m_BenefitBuffCount--;
		if(m_BenefitBuffCount == 0)
		{
			AsUserEntity player = AsEntityManager.Instance.UserEntity;
			if(player != null)
			{
				if(player.GetProperty<bool>(eComponentProperty.LIVING) == true)
					Event_pCondition_BuffCancelALL();
			}
		}
	}
	
	public void SkillCoolTimeIconGenerated()
	{
		Destroy(m_Button);
	}
	#endregion
}
