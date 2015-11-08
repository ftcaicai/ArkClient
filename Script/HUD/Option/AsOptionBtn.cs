#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;
using WmWemeSDK.JSON;
public class AsOptionBtn : MonoBehaviour
{
	public UIRadioBtn m_OptionBtn = null;
	
	private bool m_bBtnState = false;
	private OptionBtnType m_eOptionBtnType;
	
	void Start()
	{
	}

	void Update()
	{
	}
	
	public void OptionBtnClick()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		m_bBtnState = !m_bBtnState;
		m_OptionBtn.Value = m_bBtnState;
		
		AsGameMain.SetOptionState( m_eOptionBtnType, m_bBtnState);
		_ApplyOption();
	}
	
	public void Init(OptionBtnType eOptionBtnType, bool bState)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_OptionBtn.spriteText);
		m_OptionBtn.Text = _GetBtnName( eOptionBtnType);
		m_eOptionBtnType = eOptionBtnType;
		m_bBtnState = bState;
		m_OptionBtn.Value = bState;
	}
	
	// < private
	private string _GetBtnName(OptionBtnType eOptionBtnType)
	{
		int nIndex = -1;
		
		switch( eOptionBtnType)
		{
		case OptionBtnType.OptionBtnType_SoundBG: nIndex = 1281; break;
		case OptionBtnType.OptionBtnType_SoundEff: nIndex = 1282; break;
//		case OptionBtnType.OptionBtnType_CharacterQuality: nIndex = 1283; break;
//		case OptionBtnType.OptionBtnType_EffectQuality: nIndex = 1284; break;
		case OptionBtnType.OptionBtnType_MonsterName: nIndex = 1285; break;
		case OptionBtnType.OptionBtnType_NpcName: nIndex = 1286; break;
//		case OptionBtnType.OptionBtnType_HelmView: nIndex = 1287; break;
//		case OptionBtnType.OptionBtnType_PushNotification: nIndex = 1288; break;
		case OptionBtnType.OptionBtnType_Chat: nIndex = 1997; break;
		case OptionBtnType.OptionBtnType_Filtering: nIndex = 1289; break;
		case OptionBtnType.OptionBtnType_SkillCoolAlram: nIndex = 4203; break;
#if !NEW_DELEGATE_IMAGE
		case OptionBtnType.OptionBtnType_RankMark: nIndex = 4205; break;
		case OptionBtnType.OptionBtnType_PvpGrade: nIndex = 995; break;
#endif
		case OptionBtnType.OptionBtnType_Push: nIndex = 1288; break;
		case OptionBtnType.OptionBtnType_QuestPartyMatching: nIndex = 2100; break;
		case OptionBtnType.OptionBtnType_AutoChat: nIndex = 998; break;
		case OptionBtnType.OptionBtnType_Vibrate: nIndex = 2108; break;
		case OptionBtnType.OptionBtnType_SubTitleName: nIndex = 2141; break;
		case OptionBtnType.OptionBtnType_EffectShow: nIndex = 802; break;
		case OptionBtnType.OptionBtnType_ResourceShow: nIndex = 801; break;
		case OptionBtnType.OptionBtnType_FriendOnlineAlarm: nIndex = 1778; break;
		case OptionBtnType.OptionBtnType_FriendInviteRefuse: nIndex = 1779; break;
		case OptionBtnType.OptionBtnType_PartyInviteRefuse: nIndex = 1780; break;
		case OptionBtnType.OptionBtnType_GuildInviteRefuse: nIndex = 1781; break;
		case OptionBtnType.OptionBtnType_VoiceBattle: nIndex = 2135; break; //$yde
		case OptionBtnType.OptionBtnType_TargetChange: nIndex = 2158; break; //$yde
		}
		
		return AsTableManager.Instance.GetTbl_String( nIndex);
	}
	
	private void _ApplyOption()
	{
		switch( m_eOptionBtnType)
		{
		case OptionBtnType.OptionBtnType_SoundBG:
			{
				if( true == m_bBtnState)
					AsSoundManager.Instance.PlayBGMBuf();
				else
					AsSoundManager.Instance.StopBGM();
			}
			break;
		case OptionBtnType.OptionBtnType_Chat:
			{
				AsChatManager.Instance.ToggleWide( m_bBtnState);
			}
			break;
		case OptionBtnType.OptionBtnType_Push:				
			{
				string resultString = WemeManager.Instance.isAllowPushMessage();	
				if( true == m_bBtnState)
				{
					if(WemeManager.isSuccess(resultString)){
							bool allow = JSONObject.Parse(resultString).GetBoolean("allow");
							if(!allow)
								WemeSdkManager.GetMainGameObject.setPushAllow();
						}	
				}
				else
				{
					if(WemeManager.isSuccess(resultString)){
							bool allow = JSONObject.Parse(resultString).GetBoolean("allow");
							if(allow)
								WemeSdkManager.GetMainGameObject.setPushAllow();
						}
				}				
			}
			break;	
		case OptionBtnType.OptionBtnType_EffectShow:				
			{
				if( false == m_bBtnState)
					AsEffectManager.Instance.RemoveAllEntities();
			}
			break;
			
		case OptionBtnType.OptionBtnType_SubTitleName:
			{
			bool bSubTitleHide = m_bBtnState;
				body_CS_SUBTITLE_SET subtitleSet = new body_CS_SUBTITLE_SET( AsDesignationManager.Instance.CurrentID, bSubTitleHide);
				byte[] data = subtitleSet.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
			}
			break;

        case OptionBtnType.OptionBtnType_MonsterName:
            {
                if (AsHUDController.Instance != null)
                    if (AsHUDController.Instance.panelManager != null)
                        AsHUDController.Instance.panelManager.UpdateMonsterNamePanel();
            }
            break;
		}
	}
	// private >
}
