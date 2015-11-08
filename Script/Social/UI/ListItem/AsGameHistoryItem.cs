using UnityEngine;
using System.Collections;

public class AsGameHistoryItem : MonoBehaviour
{
	public UIButton m_FaceBookBtn;
	public UIButton m_TwitterBtn;
	public SpriteText m_Time = null;
	public SpriteText m_GameHistory = null;

	string m_strArticle = null;
	body2_SC_SOCIAL_HISTORY m_data;

	// Use this for initialization
	void Start()
	{
		m_FaceBookBtn.SetInputDelegate( FaceBookBtnDelegate);
		m_TwitterBtn.SetInputDelegate( TwitterBtnDelegate);
	}

	private bool m_isClone = false;

	public bool IsClone
	{
		get { return m_isClone; }
		set { m_isClone = value; }
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void FaceBookBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "FaceBookBtnDelegate");
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.OPEN_FACEBOOK) != null)
				AsCommonSender.SendClearOpneUI(OpenUIType.OPEN_FACEBOOK);

			if( AsSocialManager.Instance.IsLoginFacebook())
			{
				if( AsSocialManager.Instance.getSessionPermissions())
					AsSocialManager.Instance.PostMessageFacebook( m_strArticle, m_data.nCharUniqKey, m_data.nSubTitleIdx);
				else
					AsSocialManager.Instance.reauthorizeWithPublishPermissions();
			//	AsCommonSender.SendSocialHistoryRegister( m_data.nCharUniqKey, m_data.nSubTitleIdx, ( int)eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_FACEBOOK);
			}
			else
			{
				Debug.Log( "FaceBookBtnDelegate::LoginFacebook()");
				AsSocialManager.Instance.LoginFacebook();
			}
		}
	}

	private void TwitterBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log( "TwitterBtnDelegate");
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if( AsSocialManager.Instance.IsLoginTwitter())
			{
				AsSocialManager.Instance.PostMessageTwitter( m_strArticle);
				AsCommonSender.SendSocialHistoryRegister( m_data.nCharUniqKey, m_data.nSubTitleIdx, ( int)eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_TWITTER);
				
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), 
					AsTableManager.Instance.GetTbl_String(4051), null, null, AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);//#21741
				
			}
			else
			{
				AsSocialManager.Instance.LoginTwitter();
			}
		}
	}

	void OnEnable()
	{
		SetButtonVisible();
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	public void SetButtonVisible()
	{
		if( IsClone)
		{
			SetVisible( m_FaceBookBtn.gameObject, true);
			SetVisible( m_TwitterBtn.gameObject, true);
			m_FaceBookBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_FaceBookBtn.controlIsEnabled = false;
			m_TwitterBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_TwitterBtn.controlIsEnabled = false;
		}
		else
		{
			SetVisible( m_FaceBookBtn.gameObject, true);
			SetVisible( m_TwitterBtn.gameObject, true);
		}
	}

	private void SetButtonState()
	{
		if( IsClone)
			return;

	
		m_TwitterBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		m_TwitterBtn.controlIsEnabled = true;
	//	 #22819
	//	m_FaceBookBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
	//	m_FaceBookBtn.controlIsEnabled = true;

	//	if( eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_FACEBOOK == ( eSOCIAL_HISTORY_PLATFORM)m_data.ePlatform)
	//	{
	//		m_FaceBookBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
	//		m_FaceBookBtn.controlIsEnabled = false;
	//	}
		
		m_FaceBookBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		m_FaceBookBtn.controlIsEnabled = false;
		
		if( eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_TWITTER == ( eSOCIAL_HISTORY_PLATFORM)m_data.ePlatform)
		{
			m_TwitterBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_TwitterBtn.controlIsEnabled = false;
		}

		if( eSOCIAL_HISTORY_PLATFORM.eSOCIAL_HISTORY_PLATFORM_ALL == ( eSOCIAL_HISTORY_PLATFORM)m_data.ePlatform)
		{
			m_FaceBookBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_FaceBookBtn.controlIsEnabled = false;

			m_TwitterBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_TwitterBtn.controlIsEnabled = false;
		}
	}

	public void SetHistoryData( body2_SC_SOCIAL_HISTORY Data, bool isClone)
	{
		m_data = Data;
		IsClone = isClone;
		SetButtonVisible();
		SetButtonState();

		if( Data.nSubTitleIdx == 0) // Friand Add Message
		{
			m_GameHistory.Text = string.Format( AsTableManager.Instance.GetTbl_String( 153), Data.szUserId);
			m_Time.Text = GetTime( Data.nTime);
		}
		else //Title Add Message
		{
			DesignationData designationData = AsDesignationManager.Instance.GetDesignation( Data.nSubTitleIdx);
			if( null == designationData)
			{
				Debug.LogWarning( "SetHistoryData nSubTitleIdx Err!!! : ");
				return;
			}
			m_GameHistory.Text = string.Format( AsTableManager.Instance.GetTbl_String( 275),Data.szUserId, AsTableManager.Instance.GetTbl_String( designationData.name));
			m_Time.Text = GetTime( Data.nTime);

			m_strArticle = string.Format( AsTableManager.Instance.GetTbl_String( 415),Data.szUserId, AsTableManager.Instance.GetTbl_String( designationData.name));
		}
	}

	private string GetTime( long nTime)
	{
		System.DateTime dt = new System.DateTime( 1970, 1, 1, 9, 0, 0);
		dt = dt.AddSeconds( nTime);
		return dt.ToString( "g");
	}
}
