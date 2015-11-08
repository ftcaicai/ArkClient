using UnityEngine;
using System.Collections;

public class AsInviteTab : AsSocialTab
{
	public AsFacebookTab m_FaceBookPanels = null;
	public AsSMSTab m_SMSPanels = null;
	public UIButton m_FaceBookBtn = null;
	public UIButton m_KakaoApiBtn = null;
	public UIButton m_PhoneBtn = null;
	public SpriteText m_FacebookTitleText = null;
	public SpriteText m_FacebookMessgaeText = null;
	public SpriteText m_FacebookCountText = null;
	public SpriteText m_KakaoTitleText = null;
	public SpriteText m_KakaoMessgaeText = null;
	public SpriteText m_KakaoCountText = null;
	public SimpleSprite m_PhoneImage = null;
	public SpriteText m_PhoneTitleText = null;
	public SpriteText m_PhoneMessgaeText = null;
	public SpriteText m_PhoneCountText = null;

	public GameObject	m_goLobby;
	public GameObject 	m_goLine;
	public GameObject	m_goTwitter;


	// Use this for initialization
	void Start()
	{
		m_FaceBookPanels.gameObject.SetActiveRecursively( false);
		m_SMSPanels.gameObject.SetActiveRecursively( false);

		m_FaceBookBtn.SetInputDelegate( FaceBookBtnDelegate);
		m_KakaoApiBtn.SetInputDelegate( KakaoApiBtnDelegate);
		m_PhoneBtn.SetInputDelegate( PhoneBtnDelegate);

		m_FaceBookBtn.Text = AsTableManager.Instance.GetTbl_String(1263);
		m_FacebookTitleText.Text = AsTableManager.Instance.GetTbl_String(4113);
		m_FacebookMessgaeText.Text = AsTableManager.Instance.GetTbl_String(4114);
		m_FacebookCountText.Text = AsTableManager.Instance.GetTbl_String(4055);

		m_KakaoApiBtn.Text = AsTableManager.Instance.GetTbl_String(1263);
		m_KakaoTitleText.Text = AsTableManager.Instance.GetTbl_String(4115);
		m_KakaoMessgaeText.Text = AsTableManager.Instance.GetTbl_String(4116);
		m_KakaoCountText.Text = AsTableManager.Instance.GetTbl_String(4055);

		m_PhoneBtn.Text = AsTableManager.Instance.GetTbl_String(1263);
		m_PhoneTitleText.Text = AsTableManager.Instance.GetTbl_String(4283);
		m_PhoneMessgaeText.Text = AsTableManager.Instance.GetTbl_String(4284);
		m_PhoneCountText.Text = AsTableManager.Instance.GetTbl_String(4055);


		if (AsReviewConditionManager.Instance.IsReviewCondition (eREVIEW_CONDITION.FRIEND_REWARD) == false) 
		{
			Vector3	vPosLine = m_goLobby.transform.localPosition;

			m_goLobby.SetActive(false);
			m_goLine.SetActive(false);

			m_goTwitter.transform.localPosition = vPosLine;
		}
	}

	void OnEnable()
	{
	/*	#if UNITY_IPHONE
		m_PhoneBtn.gameObject.SetActiveRecursively( false);
		m_PhoneTitleText.gameObject.SetActiveRecursively( false);
		m_PhoneMessgaeText.gameObject.SetActiveRecursively( false);
		m_PhoneCountText.gameObject.SetActiveRecursively( false);
		m_PhoneImage.gameObject.SetActiveRecursively( false);
		#endif
	*/	
	}

	public void SetGameInviteRewardList( body_SC_GAME_INVITE_LIST_RESULT data)
	{
		/*string CountText = string.Format( "{0}/{1}", data.nFacebook_Day, data.nFacebook_Day_Max);
		m_FacebookCountText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4055), CountText);
		CountText = string.Format( "{0}/{1}", data.nKakaotalk_Day, data.nKakaotalk_Day_Max);
		m_KakaoCountText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4055), CountText);
		*/
		string CountText = string.Format( "{0}/{1}", data.nLobi_Day, data.nLobi_Day_Max);
		m_FacebookCountText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4055), CountText);
	
		CountText = string.Format( "{0}/{1}", data.nLine_Day, data.nLine_Day_Max);
		m_KakaoCountText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4055), CountText);
		
		CountText = string.Format( "{0}/{1}", data.nTwitter_Day, data.nTwitter_Day_Max);
		m_PhoneCountText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4055), CountText);
	}

	// Update is called once per frame
	void Update()
	{
	}

	override public void Init()
	{
	}

	public override UIScrollList getList()
	{
		return null;
	}

	private void FaceBookBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		//	AsSocialManager.Instance.GetFaceBookFriends();
		//	gameObject.SetActive( false);
			AsCommonSender.SendGameInvite( true, ( int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LOBI, "12345");
		}
	}

	private void KakaoApiBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsCommonSender.SendGameInvite( true, ( int)eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LINE, "12345");
		}
	}

	private void PhoneBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			if(!AsSocialManager.Instance.IsLoginTwitter())
			{
				AsSocialManager.Instance.LoginTwitter();
				return;
			}
			AsSocialManager.Instance.GetTwitterFollowers();
		//	AsSocialManager.Instance.GetSMSFriends()
			//gameObject.SetActiveRecursively( false); //#24744
			gameObject.SetActive( false);
		}
	}
}
