using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public enum QuestTutorialMsg
{
	NONE,
	QUEST_TUTORIAL_FORCED_START,
	TAP_QUEST_LIST,
	TAP_QUEST_LIST_BTN,
	TAP_SKILL_SHOP_BTN,
	TAP_SKILL_SHOP_BASE,
	TAP_SKILL_SHOP_CHOISE,
	TAP_SKILL_SHOP_LIST_0,
	TAP_MINIMAP_WARP,
	TAP_KAKAO_VIEW,
	TAP_MENU_BTN,
	TAP_QUESTBOOK_DAILYQUEST,
	TAP_INVEN_ITEM,
	TAP_DESIGNATION_BTN,
	TAP_DESIGNATION,
	TAP_SOCIAL_INFO,
	TAP_RANDOM_FRIEND,
	TAP_AUTO_BATTLE,
	OPEN_QUEST_ACCEPT_UI,
	OPEN_NPC_TALK,
	OPEN_INVENTORY,
	OPEN_WORLDMAP,
	OPEN_STRENGTHEN_UI,
	OPEN_FINDFRIEND,
	OPEN_QUESTBOOK,
	OPEN_PRODUCT,
	OPEN_STATUS,
	OPEN_SYSTEM,
	OPEN_SOCIAL,
	OPEN_SOCIAL_STORE,
	OPEN_PARTY_MENU,
	OPEN_SEARCH_PARTY_MENU,
	OPEN_INDUN_DLG,
	SHOW_QUESTLIST,
	PREV_TALK,
	CLOSE_STATUS,
	CLOSE_DESIGNATION_DLG,
	CLOSE_INVENTORY,
	CLOSE_NPC_COMMUNITY,
	CLOSE_QUEST_ACCEPT_UI,
	CLOSE_STRENGTHEN_UI,
	CLOSE_SKILLSHOP,
	CLOSE_WORLDMAP,
	CLOSE_MYINFO,
	CLOSE_FINDFRIEND,
	CLOSE_QUESTBOOK,
	CLOSE_SYSTEM,
	CLOSE_SOCIAL_DLG,
	CLOSE_OPTION_MENU,
	CLOSE_OPTION_MENU_BY_PRODUCT,
	CLOSE_PARTY_DLG,
	CLOSE_INDUN_DLG,
	CHANGE_INVENTORY_PAGE,
	MOVE_INVENTORY_ITEM,
	ACCEPT_QUEST,
	CLEAR_QUEST,
	DROP_QUEST,
	MENU_EXPAND,
	MENU_COLLAPSE,
	USE_ITEM,
}

public enum TutorialConditionType
{
	NONE,
	QUEST_ID,
	SKILL_NOT_HAVE,
	NPC,
	START_STEP_MSG,
	NOT_RESTART_STEP_AFTER_CLOSE,
	NOT_COMPLETE_ACHIEVEMENT,
	NOT_ACTION_ACHIEVEMENT_OVER_COUNT,
	NOT_COMPLETE_ACHIEVEMENT_UPDATE,
}

public enum TutorialStepType
{
	NONE,
	OVER_OBJECT,
	OVER_OBJECT_OUT_HUD,
	OVER_OBJECT_OUT_HUD_UPDATE,
	OVER_OBJECT_UPDATE,
	OVER_QUEST_LIST,
	OVER_LISTITEM_OUT_HUD,
	OVER_INVEN_ITEM,
	OVER_KILLSHOP,
	OVER_DESIGNATION_LIST,
	OVER_SOCIALHISTORY_LIST,
	HIDE,
	LAST_HIDE,
}


public class QuestTutorialMsgInfo
{
	public QuestTutorialMsg message;
	public int value;

	public QuestTutorialMsgInfo()
	{
		message = global::QuestTutorialMsg.NONE;
		value = 0;
	}

	public QuestTutorialMsgInfo( QuestTutorialMsg _message, int _value)
	{
		message = _message;
		value = _value;
	}

	public QuestTutorialMsgInfo( QuestTutorialMsg _message)
	{
		message = _message;
		value = 0;
	}
}


[System.Serializable]
public class TutorialStepInfo
{
	public static string NodeName = "Step";
	public TutorialStepType type;
	public QuestTutorialMsg nextMsg;
	public GameObject objectTarget;
	public string uiPath;
	public TutorialFingerType fingerType;
	public bool doubleTouch;
	public List<int> listOption;

	public void Initilize()
	{
		type = TutorialStepType.NONE;
		nextMsg = QuestTutorialMsg.NONE;
		objectTarget = null;
		uiPath = string.Empty;
		fingerType = TutorialFingerType.TOP;
		doubleTouch = false;
		listOption = new List<int>();
	}

	public void LoadFromXml( XmlNode _node)
	{
		// init
		Initilize();

		foreach ( XmlAttribute attribute in _node.Attributes)
		{
			switch ( attribute.Name)
			{
			case "Type":
				type = ( TutorialStepType)Enum.Parse( typeof( TutorialStepType), attribute.Value, true);
				break;
			case "Path":
				uiPath = attribute.Value;
				break;
			case "NextMsg":
				nextMsg = ( QuestTutorialMsg)Enum.Parse( typeof( QuestTutorialMsg), attribute.Value, true);
				break;
			case "PtType":
				fingerType = ( TutorialFingerType)Enum.Parse( typeof( TutorialFingerType), attribute.Value, true);
				break;
			case "DoublePt":
				doubleTouch = System.Convert.ToInt32( attribute.Value) != 0 ? true : false;
				break;
			case "Value0":
			case "Value1":
			case "Value2":
				listOption.Add( System.Convert.ToInt32( attribute.Value));
				break;
			}
		}
	}
}


[System.Serializable]
public class TutorialConditionBase
{
	public static string NodeName = "Cond";
	public TutorialConditionType type;

	virtual public void Initilize()
	{
		type = TutorialConditionType.NONE;
	}

	virtual public void LoadFromXml( XmlNode _node)
	{
		if( _node.Attributes["Type"] != null)
			type = ( TutorialConditionType)Enum.Parse( typeof( TutorialConditionType), _node.Attributes["Type"].Value, true);
	}

	virtual public bool CanProgress( QuestTutorialMsgInfo _msgInfo)
	{
		return true;
	}
}


[System.Serializable]
public class TutorialConditionUseInt: TutorialConditionBase
{
	public int value;

	public override void Initilize()
	{
		base.Initilize();
		value = 0;
	}

	public override void LoadFromXml( XmlNode _node)
	{
		base.LoadFromXml( _node);
	
		if( _node.Attributes["Value"] != null)
			value = System.Convert.ToInt32( _node.Attributes["Value"].Value);
	}

	public override bool CanProgress( QuestTutorialMsgInfo _msgInfo)
	{
		if( type == TutorialConditionType.QUEST_ID)
		{
			if( value <= 0)
			{
				int realValue = value * -1;
				return !ArkQuestmanager.instance.IsProgressQuest( realValue) && !ArkQuestmanager.instance.IsCompleteQuest( realValue);
			}
			else
				return ArkQuestmanager.instance.IsProgressQuest( value);
		}
		else if( type == TutorialConditionType.SKILL_NOT_HAVE)
		{
			return !SkillBook.Instance.ContainSkillTableID( value);
		}
		else if( type == TutorialConditionType.NPC)
		{
			if( _msgInfo.message == QuestTutorialMsg.ACCEPT_QUEST)
				return true;

			if( AsHUDController.Instance.targetInfo != null)
			{
				if( AsHUDController.Instance.targetInfo.getCurTargetEntity != null)
				{
					if( AsHUDController.Instance.targetInfo.getCurTargetEntity.FsmType == eFsmType.NPC)
					{
						int npcId = AsHUDController.Instance.targetInfo.getCurTargetEntity.GetProperty<int>( eComponentProperty.NPC_ID);

						if( npcId == value)
							return true;
					}
				}
			}

			return false;
		}

		return true;
	}
}


[System.Serializable]
public class TutorialConditionStartStep : TutorialConditionBase
{
	public QuestTutorialMsg msg;

	public override void Initilize()
	{
		base.Initilize();
		msg = QuestTutorialMsg.NONE;
	}

	public override void LoadFromXml( XmlNode _node)
	{
		base.LoadFromXml( _node);
	
		if( _node.Attributes["Value"] != null)
			msg = ( QuestTutorialMsg)Enum.Parse( typeof( QuestTutorialMsg), _node.Attributes["Value"].Value, true);
	}
}


[System.Serializable]
public class QuestTutorial
{
	public List<TutorialConditionBase> listCondition = new List<TutorialConditionBase>();
	public List<TutorialConditionUseInt> listConditionAchievement = new List<TutorialConditionUseInt>();
	public List<TutorialConditionUseInt> listConditionAchievementCount = new List<TutorialConditionUseInt>();
	public List<TutorialConditionUseInt> listConditionAchievementUpdate = new List<TutorialConditionUseInt>();
	public List<TutorialStepInfo> listStep = new List<TutorialStepInfo>();
	public List<QuestTutorialMsg> listCloseMsg = new List<QuestTutorialMsg>();
	public List<QuestTutorialMsg> listUpdateMsg = new List<QuestTutorialMsg>();
	public List<QuestTutorialMsg> listNextStepMsg = new List<QuestTutorialMsg>();
	public List<TutorialConditionStartStep> listStartStepMsg = new List<TutorialConditionStartStep>();
	public List<int> listQuestID = new List<int>();
	public List<int> listTapInvenItemID = new List<int>();
	public List<int> listUseItemID = new List<int>();
	public List<int> listSelectDesgination = new List<int>();
	public bool notRestartAfterClose = false;
	public bool notProcessMsg = false;
	public int nowStepIdx = -1;
	public TutorialStepInfo nowStep = null;

	public void LoadFromXml( XmlNode _node)
	{
		XmlNodeList childNodes = _node.ChildNodes;

		foreach ( XmlNode node in childNodes)
		{
			if( node.Name == TutorialConditionBase.NodeName)
			{
				TutorialConditionBase condBase = new TutorialConditionBase();
				condBase.LoadFromXml( node);

				if( condBase.type == TutorialConditionType.NPC || condBase.type == TutorialConditionType.QUEST_ID || condBase.type == TutorialConditionType.SKILL_NOT_HAVE)
				{
					TutorialConditionUseInt cond = new TutorialConditionUseInt();
					cond.LoadFromXml( node);
					listCondition.Add( cond);
	
					if( cond.type == TutorialConditionType.QUEST_ID)
						listQuestID.Add( cond.value);
				}
				else if( condBase.type == TutorialConditionType.NOT_COMPLETE_ACHIEVEMENT)
				{
					TutorialConditionUseInt cond = new TutorialConditionUseInt();
					cond.LoadFromXml( node);
					listConditionAchievement.Add( cond);
				}
				else if( condBase.type == TutorialConditionType.NOT_ACTION_ACHIEVEMENT_OVER_COUNT)
				{
					TutorialConditionUseInt cond = new TutorialConditionUseInt();
					cond.LoadFromXml( node);
					listConditionAchievementCount.Add( cond);
				}
				else if( condBase.type == TutorialConditionType.NOT_COMPLETE_ACHIEVEMENT_UPDATE)
				{
					TutorialConditionUseInt conditionAchievementUpdate = new TutorialConditionUseInt();
					conditionAchievementUpdate.LoadFromXml( node);
	
					listConditionAchievementUpdate.Add( conditionAchievementUpdate);
				}
				else if( condBase.type == TutorialConditionType.START_STEP_MSG)
				{
					TutorialConditionStartStep startStepCon = new TutorialConditionStartStep();
					startStepCon.LoadFromXml( node);
					listStartStepMsg.Add( startStepCon);
				}
				else if( condBase.type == TutorialConditionType.NOT_RESTART_STEP_AFTER_CLOSE)
				{
					notRestartAfterClose = true;
				}
			}
			else if( node.Name == TutorialStepInfo.NodeName)
			{
				TutorialStepInfo step = new TutorialStepInfo();
				step.LoadFromXml( node);

				// tap inven item
				if( step.nextMsg == QuestTutorialMsg.TAP_INVEN_ITEM)
				{
					foreach ( int id in step.listOption)
					{
						if( id != 0)
							listTapInvenItemID.Add( id);
					}
				}

				if( step.nextMsg == QuestTutorialMsg.USE_ITEM)
				{
					foreach ( int id in step.listOption)
					{
						if( id != 0)
							listUseItemID.Add( id);
					}
				}

				if( step.nextMsg == QuestTutorialMsg.TAP_DESIGNATION)
				{
					foreach ( int id in step.listOption)
					{
						if( id != 0)
							listSelectDesgination.Add( id);
					}
				}

				if( !listNextStepMsg.Contains( step.nextMsg))
					listNextStepMsg.Add( step.nextMsg);

				listStep.Add( step);
			}
			else if( node.Name == "Close")
			{
				if( node.Attributes["Msg"] != null)
					listCloseMsg.Add( ( QuestTutorialMsg)Enum.Parse( typeof( QuestTutorialMsg), node.Attributes["Msg"].Value, true));
			}
			else if( node.Name == "Update")
			{
				if( node.Attributes["Msg"] != null)
					listUpdateMsg.Add( ( QuestTutorialMsg)Enum.Parse( typeof( QuestTutorialMsg), node.Attributes["Msg"].Value, true));
			}
		}
	}

	public TutorialStepInfo ProcessMessage( QuestTutorialMsgInfo _msgInfo)
	{
		// 시작 메시지를 찾는다
		if( listStartStepMsg.Count > 0)
		{
			if( _msgInfo.message == listStartStepMsg[0].msg)
			{
				nowStep = listStep[0];
				nowStepIdx = 0;
				return nowStep;
			}
		}

		// check next msg
		int count = 0;
		foreach ( TutorialStepInfo step in listStep)
		{
			if( _msgInfo.message == step.nextMsg)
			{
				if( _msgInfo.message == QuestTutorialMsg.TAP_INVEN_ITEM)
				{
					if( !listTapInvenItemID.Contains( _msgInfo.value))
						return null;
				}
				else if( _msgInfo.message == QuestTutorialMsg.USE_ITEM)
				{
					if( !listUseItemID.Contains( _msgInfo.value))
						return null;
				}
				else if( _msgInfo.message == QuestTutorialMsg.TAP_DESIGNATION)
				{
					if( !( listSelectDesgination.Contains( _msgInfo.value)))
						return null;
				}

				nowStep = listStep[count + 1];
				nowStepIdx = count + 1;
				return nowStep;
			}
			count++;
		}

		return null;
	}

	public TutorialStepInfo ProcessUpdateMessage()
	{
		return nowStep;
	}

	public TutorialStepInfo ForcedStartStepZero()
	{
		// check have Start Step Msg
		if( listStartStepMsg.Count >= 1)
			return null;

		nowStep = listStep[0];
		nowStepIdx = 0;

		return nowStep;
	}

	public TutorialStepInfo ForcedProgressStep( int _stepIdx)
	{
		// check achievement count
		if( CheckActionAchievementCount() == false)
			return null;

		if( CheckCompleteAchievement() == false)
			return null;

		nowStep = listStep[_stepIdx];
		nowStepIdx = _stepIdx;

		return nowStep;
	}

	public bool CheckActionAchievementCount()
	{
		if( listConditionAchievementCount.Count <= 0)
			return true;

		// check achievement count
		foreach ( TutorialConditionUseInt cond in listConditionAchievementCount)
		{
			ArkQuest quest = ArkQuestmanager.instance.GetQuest( listQuestID[0]);

			if( quest == null)
				return false;

			ArkSphereQuestTool.QuestData questData = quest.GetQuestData();

			if( questData == null)
				return false;

			if( questData.Achievement.GetAchievement( 0).CommonCount >= cond.value)
				return false;
		}

		return true;
	}

	public bool CheckCompleteAchievementUpdate()
	{
		if( listConditionAchievementUpdate.Count <= 0)
			return false;

		foreach ( int questID in listQuestID)
		{
			// 나중에 꼭 바꿔야함 이 조건의 중복처리를 하지 않음
			if( ArkQuestmanager.instance.IsCompleteAchievement( questID, listConditionAchievementUpdate[0].value) == true)
			return true;
		}

		return false;
	}

	public bool CheckCompleteAchievement()
	{
		if( listConditionAchievement.Count <= 0)
			return true;

		// not complete achivement
		foreach ( TutorialConditionUseInt cond in listConditionAchievement)
		{
			foreach ( int questID in listQuestID)
			{
				ArkQuest quest = ArkQuestmanager.instance.GetQuest( questID);

				if( quest != null)
				{
					ArkSphereQuestTool.QuestData questData = quest.GetQuestData();
					AchBase achBase = questData.Achievement.GetAchievement( cond.value);
	
					if( achBase.IsComplete == false)
						return true;
				}
			}
		}
	
		return false;
	}

	public bool CanProcess( QuestTutorialMsgInfo _msgInfo)
	{
		List<int> listQuestID = new List<int>();
		bool bCheckQuestID = false;
		foreach ( TutorialConditionBase condition in listCondition)
		{
			if( condition.type != TutorialConditionType.QUEST_ID)
				continue;

			TutorialConditionUseInt conQuestID = condition as TutorialConditionUseInt;

			listQuestID.Add( conQuestID.value);

			if( conQuestID.value < 0)
			{
				if( !ArkQuestmanager.instance.IsProgressQuest( conQuestID.value * -1) && !ArkQuestmanager.instance.IsCompleteQuest( conQuestID.value * -1) && !ArkQuestmanager.instance.IsClearQuest( conQuestID.value * -1))
				{
					bCheckQuestID = true;
					break;
				}
			}
			else
			{
				if( ArkQuestmanager.instance.IsProgressQuest( conQuestID.value) == true)
				{
					bCheckQuestID = true;
					break;
				}
			}
		}

		if( bCheckQuestID == false)
			return false;

		// not complete achivement
		if( CheckCompleteAchievement() == false)
			return false;

		if( CheckActionAchievementCount() == false)
			return false;

		foreach ( TutorialConditionBase condition in listCondition)
		{
			if( condition.type == TutorialConditionType.QUEST_ID)
				continue;

			if( condition.CanProgress( _msgInfo) == false)
				return false;
		}
	
		return true;
	}
}
