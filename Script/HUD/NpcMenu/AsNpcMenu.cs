
#define _CBT

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public delegate void dele_Talk( int talkCount);

public class AsNpcMenu : UIMessageBase
{
	public UIScrollList m_questScroll = null;
	public SpriteText m_Talk = null;
	public SpriteText m_NpcName = null;
	public QuestListControll m_Questlist = null;
	public UIButton[] m_MenuBtn;
	public List<QuestTalkInfo> listTalkInfo = new List<QuestTalkInfo>();
	public SimpleSprite m_Pc_Image;
	public SimpleSprite m_Npc_Image;
	public GameObject targetObject = null;
	public QuestHolder questHolder = null;
	public int m_NpcTableID = -1;
	public bool LockInput { get { return m_bInputLock; } set { m_bInputLock = value; } }
	public UIScrollList eventList;
	private float m_GapX;
	private float m_basePositionX;
	private string npcName = string.Empty;
	private dele_Talk deleTalk;
	private int m_CurrentTalkCount = 0;
	private int m_MaximumTalkCount = 0;
	private Tbl_NormalNpc_Record m_NpcRec; 
	private AsNpcEntity m_NpcEntity = null;
	private bool m_bInputLock = false;
	public bool m_bWaitOpenNpcMenu = false;
	private List<IUIListObject> m_eventListItemList = new List<IUIListObject>();
	private float m_fTooltipOpenTime = 0f;
	private IUIListObject m_CurDownListItem = null;
	[SerializeField]private UIButton cashStoreBtn = null;
	
	public int m_TextureSize = 256;
	public int m_TextureCount = 4;
	//SimpleSprite Texture Change
	public void Set_Tex( SimpleSprite Obj_Sprite, Texture2D tex, int index, int size, int count)
	{
		if( index < 0)
		{
			 Debug.LogError( "AsNpcMenu:Set_Tex() index < 0 ");
			index = 0;
		}

		Obj_Sprite.SetTexture( tex);

		int nU, nV;
	
		int uIndex = index % count;
		int vIndex = index / count;
		nU = uIndex * size;
		nV = ( vIndex + 1) * size;
		Obj_Sprite.SetLowerLeftPixel( nU, nV);
	}
	
	void Clear()
	{
		m_NpcRec = null;
		m_Talk.Text = "";
		m_NpcName.Text = "";
		
		m_CurrentTalkCount = 0;
		
		m_MenuBtn[(int)eNPCMenu.Next].SetControlState( UIButton.CONTROL_STATE.NORMAL);
		m_MenuBtn[(int)eNPCMenu.Next].controlIsEnabled = true;
		m_MenuBtn[(int)eNPCMenu.Skip].SetControlState( UIButton.CONTROL_STATE.NORMAL);
		m_MenuBtn[(int)eNPCMenu.Skip].controlIsEnabled = true;

		eventList.ClearListSync( true);
		m_eventListItemList.Clear();

		if (m_Questlist != null)
			m_Questlist.DeleteAllQuest();
	}
	
/*	
	public void ShowNpcSelectEffect( AsBaseEntity target)
	{
		GameObject goTarget = target.gameObject;
		
		if( null != AsEffectManager.Instance.GetEffectEntity( m_nNpcSelectEff))
			RemoveNpcSelectEffect();

		m_nNpcSelectEff = AsEffectManager.Instance.PlayEffect( m_strNpcSelectEffPath, goTarget.transform, true, 0.0f);
		
		int nNpcID = target.GetProperty<int>( eComponentProperty.NPC_ID);
		Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record( nNpcID);
		float fScale = 1.0f;
		if( 1 == npcRec.PointScale) fScale = 1.0f;
		else if( 2 == npcRec.PointScale) fScale = 1.2f;
		else if( 3 == npcRec.PointScale) fScale = 1.5f;
		else if( 4 == npcRec.PointScale) fScale = 2.0f;
		else if( 5 == npcRec.PointScale) fScale = 2.5f;
		AsEffectManager.Instance.SetSize( m_nNpcSelectEff, fScale);
	}
	
	public void RemoveNpcSelectEffect()
	{
		if( null == AsEffectManager.Instance)
			return;
		
		AsEffectManager.Instance.RemoveEffectEntity( m_nNpcSelectEff);
	}
*/
	public void Close()
	{
		Clear();
		
		gameObject.SetActive( false);
		
		if( m_NpcEntity != null)
		{
			AsNpcFsm objFsm = m_NpcEntity.GetComponent( eComponentType.FSM_NPC) as AsNpcFsm;
			if( null == objFsm)
				return;
			
			objFsm.SetNpcFsmState( AsNpcFsm.eNpcFsmStateType.IDLE);
		}

		targetObject = null;
		
		questHolder = null;
		
		AsHudDlgMgr.dlgPresentState &= ~AsHudDlgMgr.eDlgPresentState.NpcMenu;

		if( m_Questlist != null)
			m_Questlist.gameObject.SetActive( false);

		AsLoadingIndigator.Instance.HideIndigator();
		
		m_bInputLock = false;
		
		if( AsGameMain.s_gameState == GAME_STATE.STATE_INGAME)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_NPC_COMMUNITY, 0));
	}

	public void OpenAfterTalkClear()
	{
		if( !Open( m_NpcEntity, true))
			Debug.Log( "m_NpcEntity is null");
	}
		
	public bool Open( AsBaseEntity target , bool _callFromQuestClear = false)
	{
		AsChatFullPanel.Instance.Close();

		if( target == null)
			return false;

		targetObject = target.gameObject;
		
		Clear();
	
		if( eNPCType.NPC == target.GetProperty<eNPCType>( eComponentProperty.NPC_TYPE))
		{
			m_NpcEntity = target as AsNpcEntity;
			m_NpcRec = AsTableManager.Instance.GetTbl_NormalNpc_Record( m_NpcEntity.TableIdx);
			
			//npc name
			Tbl_Npc_Record record = AsTableManager.Instance.GetTbl_Npc_Record( m_NpcEntity.TableIdx);
			npcName = m_NpcName.Text = record.NpcName;
			m_NpcTableID = m_NpcEntity.TableIdx;
		}
	
		if( null == m_NpcRec)
		{
			Debug.LogError( "null == m_NpcRec");
			return false;
		}
		
		if( m_NpcRec.FaceId != int.MaxValue)//none
		{
			int textNum = 1 + ( m_NpcRec.FaceId / ( m_TextureCount*m_TextureCount));

			string strImageFileName = "UI/Optimization/Texture/img_NPC_" + textNum;
			Texture2D npcImage = ResourceLoad.Loadtexture( strImageFileName) as Texture2D;
			Set_Tex( m_Npc_Image, npcImage, m_NpcRec.FaceId, m_TextureSize, m_TextureCount);
		}

		questHolder = QuestHolderManager.instance.GetQuestHolder( m_NpcTableID);
		
		//Pc Image Setting
		AsUserEntity entity = AsEntityManager.Instance.UserEntity;

		if( null != entity)
		{
			int faceImageId = 0;
			eCLASS __class = entity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			eGENDER gender = entity.GetProperty<eGENDER>( eComponentProperty.GENDER);
			eRACE tribe = entity.GetProperty<eRACE>( eComponentProperty.RACE);
			Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( tribe, __class);

			switch( gender)
			{
			case eGENDER.eGENDER_MALE:
				faceImageId = record.Portrait_Male;
				break;
			case eGENDER.eGENDER_FEMALE:
				faceImageId = record.Portrait_Female;
				break;
			}	

			int textureNum = 1 + ( faceImageId / ( m_TextureCount*m_TextureCount));
			string strPcImageFileName = "UI/Optimization/Texture/img_PC_" + textureNum;
			Texture2D pcImage = ResourceLoad.Loadtexture( strPcImageFileName) as Texture2D;
			
			Set_Tex( m_Pc_Image, pcImage, faceImageId, m_TextureSize, m_TextureCount);
		}

		// for talk clear
		if( ( false == _callFromQuestClear) && ( true == CheckTalkComplete()))
			return true;
	
		gameObject.SetActiveRecursively( true);
		gameObject.active = true;
		
	
		if(0 !=  m_NpcRec.VoiceCount)
		{
			int VoiceIndex = UnityEngine.Random.Range( 0, m_NpcRec.VoiceCount);
			AsSoundManager.Instance.PlaySound( m_NpcRec.Voice(VoiceIndex), Vector3.zero, false);////npc Voice
		}
		
		int SpeechIndex = UnityEngine.Random.Range( 0, m_NpcRec.SpeechCount);
		SetTalk( m_NpcRec.Speech( SpeechIndex));////npc Speech
		
		if( m_NpcRec.FaceId == int.MaxValue)//none
			SetVisible( m_Npc_Image.gameObject, false);

		SetVisible( m_Pc_Image.gameObject, false);
		
		UpdateMenuButton();

		// check clear quest
		m_Questlist.FindQuestFromObject( m_NpcEntity.gameObject);

		ArkSphereQuestTool.QuestData questData = m_Questlist.GetClearQuest();
		
		if( questData != null)
	        AsHudDlgMgr.Instance.OpenQuestAcceptUI(questData, true);
		
		m_Questlist.Visible = false;
		
		AsHudDlgMgr.dlgPresentState |= AsHudDlgMgr.eDlgPresentState.NpcMenu;

		if (questHolder != null)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_NPC_TALK));
		//{
		//    QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.OPEN_NPC_TALK));

		//    if (questHolder.NowMarkType == QuestMarkType.HAVE_EVENT  || questHolder.nowQuetMarkType == QuestMarkType.HAVE_EVENT_AND_PROGRESS)
		//        if (m_NpcEntity != null)
		//            questHolder.UpdateQuestMark(m_NpcEntity.TableIdx);
		//}
		
		return true;
	}

	bool CheckTalkComplete()
	{
		if( m_NpcEntity != null)
		{
			m_NpcTableID = m_NpcEntity.GetProperty<int>( eComponentProperty.NPC_ID);
			
			QuestHolder questHolder = QuestHolderManager.instance.GetQuestHolder( m_NpcTableID);
			
			if( ArkQuestmanager.instance.ContainTalkWithNPC( m_NpcTableID))
			{
				Debug.Log( "Send talk");
				AsCommonSender.SendTalkWithNPC( m_NpcEntity.SessionId);
			}
			
			if( questHolder != null)
			{
				QuestMarkType markType = questHolder.GetQuestMarkType();
				if( markType == QuestMarkType.CLEAR_REMAINTALK || markType == QuestMarkType.TALK_CLEAR)
				{
					AsLoadingIndigator.Instance.ShowIndigator( string.Empty);
					m_bInputLock = true;
					m_bWaitOpenNpcMenu = true;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				Debug.LogWarning( "QuestHoder is null");
			}
		}
		
		return false;
	}


	void SetTalk( int talkId)
	{
		m_Talk.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Talk);
			Tbl_String_Record record = AsTableManager.Instance.GetTbl_String_Record( talkId);
		if( null == record)
		{
			Debug.LogError( "AsNpcMenu::SetTalk() [ null == string record ] id : " + talkId);
			return ;
		}
		m_Talk.Text = record.String;
	}

	void SetQuestTalk( int talkCount)
	{
		float toNormalize = 1.0f / 255.0f;

		string[] szColorTags = {"[h]", "[j]"};
		
		m_Talk.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Talk);
		
		string talk = listTalkInfo[talkCount].contents;
		
		StringBuilder sbTalk = new StringBuilder( talk);

		foreach (string colorTag in szColorTags)
			sbTalk.Replace(colorTag.ToUpper(), colorTag.ToLower());

		String characterName = AsEntityManager.Instance.UserEntity.GetProperty<String>( eComponentProperty.NAME);
		int lastIdx = characterName.IndexOf( '\0');
		if( lastIdx != -1)
			characterName = characterName.Substring( 0, lastIdx);
		
		String jobName = AsUserInfo.Instance.GetCurrentUserClassName();
		lastIdx = jobName.IndexOf( '\0');
		if( lastIdx != -1)
			characterName = jobName.Substring( 0, lastIdx);
		
		sbTalk = sbTalk.Replace( "[h]", String.Format( "RGBA( {0:F5},{1:F5},{2:F5},{3:F5}){4}RGBA( 1.0, 1.0, 1.0, 1.0)", 109.0f * toNormalize, 251.0f * toNormalize, 81.0f * toNormalize, 1.0f, characterName));
		sbTalk = sbTalk.Replace( "[j]", String.Format( "RGBA( {0:F5},{1:F5},{2:F5},{3:F5}){4}RGBA( 1.0, 1.0, 1.0, 1.0)", 34.0f * toNormalize, 173.0f * toNormalize, 255.0f * toNormalize, 1.0f, jobName));
		
		m_Talk.Text = sbTalk.ToString();
	}

	void SetQuestUpperLevelTalk( int _level)
	{
		m_Talk.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Talk);
		m_Talk.Text = string.Format( AsTableManager.Instance.GetTbl_String(139), _level);
	}
	
	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActive( visible);
		//obj.active = visible;
	}
	
	void SetPosition( GameObject obj, Vector3 vec)
	{
		obj.transform.localPosition = vec;
	}
	
	void SetSequence( GameObject obj, int order)
	{
		Vector3 vec = obj.transform.localPosition ;
		vec.x = m_basePositionX - ( order * m_GapX);
		SetPosition( obj, vec);
	}
	
	void AllHideButton()
	{
		for( int index = 0; index < m_MenuBtn.Length; ++index)
			SetVisible( m_MenuBtn[index].gameObject, false);
	}
	
	public void UpdateMenuButton()
	{
		AllHideButton();
		int seq = 0;
		
		SetVisible( m_MenuBtn[(int)eNPCMenu.Close].gameObject, true);
		
		for ( int index = 0; index < AsGameDefine.MAX_NPC_MENU; ++index)
		{
			if( m_NpcRec.NpcMenuBtn( index) == eNPCMenu.NONE)
				continue;
			
			if( m_NpcRec.NpcMenuBtn( index) == eNPCMenu.Cure)//$yde
			{
				AsPlayerFsm userFsm = AsEntityManager.Instance.GetPlayerCharFsm();
				if( userFsm.DeathPenaltyApplied == false)
					continue;
			}

			if (m_NpcRec.NpcMenuBtn(index) == eNPCMenu.InGameEvent)
			{
				List<Tbl_Event_Record> listEvent = AsTableManager.Instance.GetTbl_Event(m_NpcEntity.TableIdx, DateTime.Now.AddTicks(AsGameMain.serverTickGap));
				List<Tbl_Event_Record> listFiltered = new List<Tbl_Event_Record>();

				foreach (Tbl_Event_Record record in listEvent)
				{
					if (record.viewList == true)
						continue;

					listFiltered.Add(record);
				}


				if (listFiltered.Count > 0)
				{
					SetVisible(m_MenuBtn[(int)m_NpcRec.NpcMenuBtn(index)].gameObject, true);
					SetSequence(m_MenuBtn[(int)m_NpcRec.NpcMenuBtn(index)].gameObject, seq++);
				}
				else
					SetVisible(m_MenuBtn[(int)m_NpcRec.NpcMenuBtn(index)].gameObject, false);
			}
			else if( m_NpcRec.NpcMenuBtn( index) == eNPCMenu.Quest)
			{
				if( questHolder != null)
				{
					QuestMarkType questMarkType = questHolder.GetQuestMarkType();
					
					if( questMarkType != QuestMarkType.NOTHING && questMarkType != QuestMarkType.TALK_CLEAR && questMarkType != QuestMarkType.TALK_HAVE && questMarkType != QuestMarkType.HAVE_EVENT)
					{
						SetVisible( m_MenuBtn[(int)m_NpcRec.NpcMenuBtn( index)].gameObject, true);
						SetSequence( m_MenuBtn[(int)m_NpcRec.NpcMenuBtn( index)].gameObject, seq++);
						
						// visible active
						switch( questMarkType)
						{
						case QuestMarkType.HAVE:
						case QuestMarkType.CLEAR:
						case QuestMarkType.CLEAR_AND_HAVE:
						case QuestMarkType.CLEAR_REMAINTALK:
						case QuestMarkType.LOWERLEVEL_AND_HAVE_EVENT:
							m_MenuBtn[(int)m_NpcRec.NpcMenuBtn( index)].transform.GetChild( 0).gameObject.SetActive( true);
							break;


						case QuestMarkType.HAVE_EVENT_AND_PROGRESS:
						case QuestMarkType.PROGRESS:
							m_MenuBtn[(int)m_NpcRec.NpcMenuBtn(index)].transform.GetChild(0).gameObject.SetActive(false);
							break;
						case QuestMarkType.UPPERLEVEL:
							m_MenuBtn[(int)m_NpcRec.NpcMenuBtn(index)].transform.GetChild(0).gameObject.SetActive(true);
							break;
						}
					}
				}
			}
			else
			{
				SetVisible( m_MenuBtn[(int)m_NpcRec.NpcMenuBtn( index)].gameObject, true);
				SetSequence( m_MenuBtn[(int)m_NpcRec.NpcMenuBtn( index)].gameObject, seq++);
			}
		}
	}
	
	void SetPortraitCore( eNpcMenuImage _eImage)
	{
		SetVisible( m_Npc_Image.gameObject, false);
		SetVisible( m_Pc_Image.gameObject , false);
		
		switch( _eImage)
		{
		case eNpcMenuImage.Npc:
			SetVisible( m_Npc_Image.gameObject, true);
			break;
		case eNpcMenuImage.Pc:
			SetVisible( m_Pc_Image.gameObject, true);
			break;
		case eNpcMenuImage.Both:
			SetVisible( m_Npc_Image.gameObject, true);
			SetVisible( m_Pc_Image.gameObject, true);
			break;
		}
	}
	
	//Image Process
	void SetPortrait( int count)
	{
		SetPortraitCore( m_NpcRec.DisplayType( count));
	}
	
		//Image Process
	void SetPortraitForQuest( int count)
	{
		if( count > 2)
			count = 0;
		
		eNpcMenuImage nowImage = ( eNpcMenuImage)count;
		
		SetPortraitCore( nowImage);
	}
	
	
	void OnSkipTalk()
	{
		m_MenuBtn[(int)eNPCMenu.Skip].SetControlState( UIButton.CONTROL_STATE.ACTIVE);
		m_MenuBtn[(int)eNPCMenu.Skip].controlIsEnabled = false;
		deleTalk( m_MaximumTalkCount - 1);
	}

	void OnTalkCore( int talkCount, int maxTalkCount, int imageCount, bool bIsCallQuestTalk)
	{
		m_CurrentTalkCount = talkCount;
		m_MaximumTalkCount = maxTalkCount;
		
		if( false == bIsCallQuestTalk)
			SetPortrait( imageCount);
		
		if( m_CurrentTalkCount > 0)
		{
			m_MenuBtn[(int)eNPCMenu.Pre].SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_MenuBtn[(int)eNPCMenu.Pre].controlIsEnabled = true;
		}
		else if( bIsCallQuestTalk == false)
		{
			m_CurrentTalkCount = 0;
			m_MenuBtn[(int)eNPCMenu.Pre].SetControlState( UIButton.CONTROL_STATE.ACTIVE);
			m_MenuBtn[(int)eNPCMenu.Pre].controlIsEnabled = false;
		}
		
		if( m_CurrentTalkCount >= m_MaximumTalkCount - 1)
		{
			m_CurrentTalkCount = m_MaximumTalkCount - 1;
			
			m_MenuBtn[(int)eNPCMenu.Next].SetControlState( UIButton.CONTROL_STATE.ACTIVE);
			m_MenuBtn[(int)eNPCMenu.Next].controlIsEnabled = false;
			
			m_MenuBtn[(int)eNPCMenu.Skip].SetControlState( UIButton.CONTROL_STATE.ACTIVE);
			m_MenuBtn[(int)eNPCMenu.Skip].controlIsEnabled = false;
			
			if( deleTalk == OnQuestTalk)
			{
				ArkSphereQuestTool.QuestData questData = m_Questlist.GetClickQuestData();
				
				if( questData != null)
					AsHudDlgMgr.Instance.OpenQuestAcceptUI( questData, true);
			}
		}
		else
		{
			m_MenuBtn[(int)eNPCMenu.Next].SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_MenuBtn[(int)eNPCMenu.Next].controlIsEnabled = true;
			
			m_MenuBtn[(int)eNPCMenu.Skip].SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_MenuBtn[(int)eNPCMenu.Skip].controlIsEnabled = true;
		}
	}

	void OnQuestTalk( int talkCount)
	{
		ArkSphereQuestTool.QuestData nowQuestData = m_Questlist.GetClickQuestData();
		
		if( nowQuestData != null)
		{
			// fail
			if( nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_FAIL)
			{
				AsHudDlgMgr.Instance.OpenQuestAcceptUI( nowQuestData, true);
				return;
			}
			
			if( m_Questlist.IsUpperLevelQuest( nowQuestData))
			{
				List<ConditionLevel> listConLevel = nowQuestData.Condition.GetCondition<ConditionLevel>();
				
				if( listConLevel.Count > 0)
				{
					SetQuestUpperLevelTalk( listConLevel[0].MinLevel);
					AllHideButton();
					SetVisible( m_MenuBtn[(int)eNPCMenu.Close].gameObject, true);
					return;
				}
			}
			
			listTalkInfo = nowQuestData.GetNowQuestTalk();
			
			if( listTalkInfo.Count >= 1) // has talk
			{
				if( talkCount <= -1)
				{
					talkCount = 0;
					ShowQuestList();
				}
				else
				{
					OnTalkCore( talkCount, listTalkInfo.Count, listTalkInfo[talkCount].npcTexIdx, true);
					SetQuestTalk( m_CurrentTalkCount);
				}
			}
			else
			{
				if( listTalkInfo.Count == 0)
					Debug.LogWarning( "Quest( "+ nowQuestData.Info.ID + " is not exist");
			}
		}
	}

	void OnTalk( int talkCount)
	{
		OnTalkCore( talkCount, m_NpcRec.TalkCount, talkCount, false);
		
		int talkId = m_NpcRec.Talk( m_CurrentTalkCount);
		SetTalk( talkId);
	}

	void Awake()
	{
//#if _CBT
        if (AsGameMain.useCashShop == false)
		    cashStoreBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
//#endif
		m_GapX = m_MenuBtn[0].gameObject.transform.localPosition.x - m_MenuBtn[1].gameObject.transform.localPosition.x;
		m_basePositionX = m_MenuBtn[0].gameObject.transform.localPosition.x;
		
		m_MenuBtn[(int)eNPCMenu.Ok ].SetInputDelegate( OkBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Next ].SetInputDelegate( NextBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Pre ].SetInputDelegate( PreBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Skip ].SetInputDelegate( SkipBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Close ].SetInputDelegate( CloseBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Cure ].SetInputDelegate( CureBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Quest ].SetInputDelegate( QuestBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.S_Shop ].AddInputDelegate( ShopBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.I_Shop ].AddInputDelegate( ShopBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Upgrade ].SetInputDelegate( UpgradeBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Make ].SetInputDelegate( MakeBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Storage ].SetInputDelegate( WayPointBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Synthesis].SetInputDelegate( SynthesisBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Event_npc].SetInputDelegate( EventNpcBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.Recommend].SetInputDelegate( RecommendBtnDelegate);
		m_MenuBtn[(int)eNPCMenu.InGameEvent].SetInputDelegate( InGameEventBtnDelegate);
	}
	
	// Use this for initialization
	void Start ()
	{
        if (AsGameMain.useCashShop == false)
		    cashStoreBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Talk);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_NpcName);
		
		eventList.SetValueChangedDelegate( SelectEventNpcDelegate);
		
		for( int i = 0; i < (int)eNPCMenu.MaxNpcMenu; i++)
			AsLanguageManager.Instance.SetFontFromSystemLanguage( m_MenuBtn[i].spriteText);
		
		m_MenuBtn[(int)eNPCMenu.Ok].Text = AsTableManager.Instance.GetTbl_String(37910);
		m_MenuBtn[(int)eNPCMenu.Next].Text = AsTableManager.Instance.GetTbl_String(37909);
		m_MenuBtn[(int)eNPCMenu.Pre].Text = AsTableManager.Instance.GetTbl_String(37901);
		m_MenuBtn[(int)eNPCMenu.Skip].Text = AsTableManager.Instance.GetTbl_String(37904);
		m_MenuBtn[(int)eNPCMenu.Cure].Text = AsTableManager.Instance.GetTbl_String(829);
		m_MenuBtn[(int)eNPCMenu.Quest].Text = AsTableManager.Instance.GetTbl_String(37906);
		m_MenuBtn[(int)eNPCMenu.S_Shop].Text = AsTableManager.Instance.GetTbl_String(37912);
		m_MenuBtn[(int)eNPCMenu.I_Shop].Text = AsTableManager.Instance.GetTbl_String(37908);
		m_MenuBtn[(int)eNPCMenu.Upgrade].Text = AsTableManager.Instance.GetTbl_String(37905);
		m_MenuBtn[(int)eNPCMenu.Make].Text = AsTableManager.Instance.GetTbl_String(37907);
		m_MenuBtn[(int)eNPCMenu.Storage].Text = AsTableManager.Instance.GetTbl_String(37925);
		m_MenuBtn[(int)eNPCMenu.Synthesis].Text = AsTableManager.Instance.GetTbl_String(37902);
		m_MenuBtn[(int)eNPCMenu.Event_npc].Text = AsTableManager.Instance.GetTbl_String(1587);
		m_MenuBtn[(int)eNPCMenu.Recommend].Text = AsTableManager.Instance.GetTbl_String(1596);
		m_MenuBtn[(int)eNPCMenu.InGameEvent].Text = AsTableManager.Instance.GetTbl_String(1587);
	}
	
	private void OkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			Close();
	}
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			Close();
			m_Questlist.Visible = false;
		}
	}
	
	private void TalkBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			deleTalk = OnTalk;
			ShowTalkButtons();
			deleTalk( 0);
		}
	}

	private void CureBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			CureBtnClicked();
	}

	private void InGameEventBtnDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			AsHudDlgMgr.Instance.OpenInGameEventListDlg(m_NpcTableID);
	}


	private void ShowTalkButtons()
	{
		AllHideButton();
		
		SetVisible( m_MenuBtn[(int)eNPCMenu.Close].gameObject, true);
		SetVisible( m_MenuBtn[(int)eNPCMenu.Next].gameObject, true);
		SetSequence( m_MenuBtn[(int)eNPCMenu.Next].gameObject, 0);
		SetVisible( m_MenuBtn[(int)eNPCMenu.Skip].gameObject, true);
		SetSequence( m_MenuBtn[(int)eNPCMenu.Skip].gameObject, 1);
		SetVisible( m_MenuBtn[(int)eNPCMenu.Pre].gameObject, true);
		SetSequence( m_MenuBtn[(int)eNPCMenu.Pre].gameObject, 7);
	}
	
	private void PreBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.PREV_TALK, 0));
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			deleTalk( --m_CurrentTalkCount);
		}
	}
	
	private void NextBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			deleTalk( ++m_CurrentTalkCount);
		}
	}
	
	private void SkipBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			OnSkipTalk();
		}
	}
	
	private void QuestBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( AsSoundPath.ButtonClick, Vector3.zero, false);
			ShowQuestList();
		}
	}

	private void ShowQuestList()
	{
		m_Talk.Text = string.Empty;
		m_NpcName.Text = AsTableManager.Instance.GetTbl_String(48000);
		m_Questlist.Visible = !m_Questlist.Visible;
		
		AllHideButton();
		
		SetVisible( m_MenuBtn[(int)eNPCMenu.Close].gameObject, true);
		
		if( m_Questlist.Visible == true)
		{
			if( targetObject != null)
			{
				m_Questlist.FindQuestFromObject( targetObject);
				m_Questlist.Click_Event = QuestTalkStart;
				m_Questlist.Close_Event = QuestClose;
			}
			else
			{
				Debug.LogWarning( "targetObject is null");
			}
		}
	}

	private void QuestTalkStart()
	{
		m_NpcName.Text = npcName;
		
		deleTalk = OnQuestTalk;
		
		ShowTalkButtons();
		
		deleTalk( 0);
		
		m_Questlist.Visible = false;
		
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.TAP_QUEST_LIST, 0));
		
		ArkSphereQuestTool.QuestData nowQuestData = m_Questlist.GetClickQuestData();
		
		if( nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
			ArkQuestmanager.instance.AskWarp( nowQuestData);
	}

	private void QuestClose()
	{
		m_Questlist.Visible = false;
		ResetTalk();
	}

	private void ResetTalk()
	{
		SetVisible( m_Pc_Image.gameObject, false);
		int SpeechIndex = UnityEngine.Random.Range( 0, m_NpcRec.SpeechCount);
		SetTalk( m_NpcRec.Speech( SpeechIndex));
	}

	private void ShopBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( ptr.targetObj == m_MenuBtn[(int)eNPCMenu.S_Shop])
			{
				Close();
				AsHudDlgMgr.Instance.OpenSkillshop( m_NpcEntity.TableIdx);
			}
			else
			{
				Close();
				AsHudDlgMgr.Instance.OpenNpcStore(m_NpcEntity.SessionId, m_NpcTableID, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS));
			}
		}
	}
	
	private void UpgradeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
            Close();
			AsHudDlgMgr.Instance.OpenStrengthenDlg();
			AsHudDlgMgr.Instance.OpenInven();
		}
	}
	
	private void MakeBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			Close();
	}
	
	private void WayPointBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Close();
			AsHudDlgMgr.Instance.OpenStorage();
		}
	}

	private void SynthesisBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Close();
			AsHudDlgMgr.Instance.OpenSynthisisDlg_Coroutine();
		}
	}
	
	private void RecommendBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
		}
	}
	
	private void EventNpcBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AllHideButton();
			SetVisible( m_MenuBtn[(int)eNPCMenu.Close].gameObject, true);
			m_Talk.Text = string.Empty;
			AsCommonSender.SendNpcEventList();
		}
	}
	
	public void SelectEventNpcDelegate( IUIObject obj)
	{
		if( obj == eventList && eventList.SelectedItem != null)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_CurDownListItem = eventList.SelectedItem;
		}
	}
	
	private void EventNpcUpdateCheck()
	{
		if( null == m_CurDownListItem)
			return;
		
		m_fTooltipOpenTime += Time.deltaTime;
		
		if( 0.1f > m_fTooltipOpenTime)
			return;
		
		for( int i=0; i<m_eventListItemList.Count; ++i)
		{
			if( m_CurDownListItem == m_eventListItemList[i])
			{
				if( m_EventListData.body.Length <= i)
				{
					Debug.LogError( "AsNpcMenu::EventNpcUpdateCheck()[ over index : " + i);
					break;
				}
				
				TooltipMgr.Instance.OpenEventDlg( m_EventListData.body[i]);
				break;
			}
		}
		
		m_fTooltipOpenTime = 0f;
		m_CurDownListItem = null;
	}
	
	
	body1_SC_EVENT_LIST m_EventListData = null;

	public void ReceiveEventNpcList( body1_SC_EVENT_LIST _dataList)
	{
		if( null == _dataList)
			return;
		
		eventList.ClearList( true);
		m_eventListItemList.Clear();
		
		if( null == _dataList.body)
		{
			m_Talk.Text = AsTableManager.Instance.GetTbl_String(1609);
			return;
		}

		m_EventListData = _dataList;

		for ( int i=0; i < m_EventListData.body.Length; ++i)
		{
			body2_SC_EVENT_LIST _data = m_EventListData.body[i];
			IUIListObject _itemObject = eventList.CreateItem(m_Questlist.questPrefab, Color.black.ToString() + _data.szTitle);
			if( null != _itemObject)
			{
				QuestListIconController ctrl = _itemObject.gameObject.GetComponent<QuestListIconController>();
				if( null != ctrl)
					ctrl.SetUseEvent();
				
				m_eventListItemList.Add( _itemObject);
			}
		}
	}

	public override void ProcessUIMessage( UIMessageObject message)
	{
		if( gameObject.active == false)
			return;
		
		if( message.messageType == UIMessageType.UI_MESSAGE_QUESTLIST_SHOW)
			ShowQuestList();
		
		if( message.messageType == UIMessageType.UI_MESSAGE_TALK_RESET)
			ResetTalk();
		
		if( message.messageType == UIMessageType.UI_MESSAGE_TALK_MENUBUTTON_UPDATE)
			UpdateMenuButton();
		
		if( message.messageType == UIMessageType.UI_MESSAGE_TALK_CLOSE)
			Close();
	}
	
	#region - cure -
	void CureBtnClicked()
	{
		Close();
		
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		
		ulong gold = ( ulong)( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 39).Value * player.GetProperty<int>( eComponentProperty.LEVEL));
		string title = AsTableManager.Instance.GetTbl_String(1428);
		string content = AsTableManager.Instance.GetTbl_String(1429);
		AsNotify.Instance.GoldMessageBox( gold, title, content, this, "OnCureConfirmed", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
	}
	
	void OnCureConfirmed()
	{
		AsUserEntity player = AsUserInfo.Instance.GetCurrentUserEntity();
		int deathDebuffIdx = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record( 38).Value;
		if( player.CheckBuff( deathDebuffIdx) == false)
		{
			string title = AsTableManager.Instance.GetTbl_String(1428);
			string content = AsTableManager.Instance.GetTbl_String(1430);
			AsNotify.Instance.MessageBox( title, content);
		}
		else
		{
			ulong gold = ( ulong)( AsTableManager.Instance.GetTbl_GlobalWeight_Record( 39).Value * player.GetProperty<int>( eComponentProperty.LEVEL));
			
			if( AsUserInfo.Instance.SavedCharStat.nGold < gold)
			{
				string title = AsTableManager.Instance.GetTbl_String(1428);
				string content = AsTableManager.Instance.GetTbl_String(97);
				AsNotify.Instance.MessageBox( title, content);
			}
			else
			{
				// succeed
				body_CS_RESURRECT_PENALTY_CLEAR clear = new body_CS_RESURRECT_PENALTY_CLEAR();
				byte[] data = clear.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
			}
		}
	}
	#endregion
	
	void Update()
	{
		EventNpcUpdateCheck();

#if UNITY_EDITOR
		if( Input.GetKeyDown( KeyCode.U))
			QuestHolderManager.instance.UpdateQuestHolder();
#endif
	}
	
//	void OnGUI()
//	{
//		if( GUI.Button( new Rect( 600, 300, 50, 20), "cure") == true)
//		{
//			OnCureConfirmed();
//		}
//	}
}


