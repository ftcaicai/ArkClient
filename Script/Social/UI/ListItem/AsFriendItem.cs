using UnityEngine;
using System.Collections;
using System.Text;

public class AsFriendItem : MonoBehaviour
{
	const int MAX_GRADE = 5;

	public AsChargingEffect m_ButtonEffect = null;
	public SpriteText m_NickName = null;
	public SpriteText m_LastConnectTimeText = null;
	public UIButton m_HelloBtn = null;
	public UIButton m_RecallBtn = null;
	public UIButton m_ConnectRequestBtn = null;

	eFRIEND_BUTTON_TYPE m_CommandState;

	public SimpleSprite m_DefaultPortrait = null;
	public SimpleSprite m_Portrait = null;
	public UIProgressBar m_nRateProgress = null;
	public SimpleSprite[] m_Grade_Image;
	public SimpleSprite[] m_Dis_Grade_Image;

	public SimpleSprite portrait = null;

	body2_SC_FRIEND_LIST m_FriendData;
	public body2_SC_FRIEND_LIST FriendData
	{
		get { return m_FriendData; }
		set { m_FriendData = value; }
	}

	private bool isLoaded = false;
	public bool IsLoaded
	{
		get { return isLoaded; }
		set { isLoaded = value; }
	}

	void OnEnable()
	{
		m_DefaultPortrait.gameObject.SetActiveRecursively( true);
		m_Portrait.gameObject.SetActiveRecursively( false);
	}

	void OnDestory()
	{
	}

	// Use this for initialization
	void Start()
	{
		m_HelloBtn.SetInputDelegate( CommandBtnDelegate);
		m_RecallBtn.SetInputDelegate( CommandBtnDelegate);
		m_ConnectRequestBtn.SetInputDelegate( ConnectRequesDelegate);
	
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void ShowButtonEffect( Vector3 pos)
	{
		m_ButtonEffect.Enable = true;
		m_ButtonEffect.transform.position = new Vector3( pos.x, pos.y, pos.z - 2.0f);
	}

	public void HideButtonEffect()
	{
		m_ButtonEffect.Enable = false;
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}
	
	private int  GetOffLineDay(long nTime)
	{		
		System.DateTime lastConnectTime  = new System.DateTime(1970, 1, 1, 9, 0, 0);
		lastConnectTime = lastConnectTime.AddSeconds(nTime);
		System.DateTime curTime = System.DateTime.Now;   
		
		System.TimeSpan span = curTime.Subtract(lastConnectTime);

		if(span.Days > 99) 
			return 99;
		
		return span.Days;
	}
	
	public void SetFriendData( body2_SC_FRIEND_LIST Data, bool isClone = false)
	{
		m_HelloBtn.Text = AsTableManager.Instance.GetTbl_String( 1187); //#21545
		m_RecallBtn.Text = AsTableManager.Instance.GetTbl_String( 1188);
		m_ConnectRequestBtn.Text = AsTableManager.Instance.GetTbl_String( 1907);
		
		m_FriendData = Data;
		SetVisible( m_ConnectRequestBtn.gameObject, true);
		if( m_FriendData.bConnectRequest)
			m_ConnectRequestBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		else
			m_ConnectRequestBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		
		m_LastConnectTimeText.Text =  string.Format(AsTableManager.Instance.GetTbl_String(2038),GetOffLineDay(m_FriendData.nLastConnectTime));		

		m_CommandState = (eFRIEND_BUTTON_TYPE)Data.eButtonType;

		SetDelegateImage( Data.nImageIndex);
		m_NickName.Text = string.Format( "Lv.{0} {1}", Data.nLevel, Data.szCharName);
		if( Data.nSessionIdx == 0)	// 0. offline, !0. online
			m_NickName.Color = Color.gray;

		m_nRateProgress.Value = ( float)( Data.nFriendlyRate * 0.01f);

		for( int i = 0; i < MAX_GRADE; i++)
		{
			SetVisible( m_Grade_Image[i].gameObject, false);
			SetVisible( m_Dis_Grade_Image[i].gameObject, true);
		}

		int nGrade = 0;
		if( Data.nFriendlyLevel > MAX_GRADE)
			nGrade = MAX_GRADE;
		else
			nGrade = Data.nFriendlyLevel;

		for( int i = 0; i < nGrade; i++)
		{
			SetVisible( m_Grade_Image[i].gameObject, true);
			SetVisible( m_Dis_Grade_Image[i].gameObject, false);
		}

		if( isClone)
		{
			SetVisible( m_LastConnectTimeText.gameObject, false);
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
			SetVisible( m_ConnectRequestBtn.gameObject, true);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_ConnectRequestBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			return;
		}

		HideButtonEffect();

		
		
		switch( m_CommandState)
		{
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_NOTHING:
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_ENABLE:
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_ENABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
		SetVisible( m_HelloBtn.gameObject, false);
			SetVisible( m_RecallBtn.gameObject, true);
			m_RecallBtn.Text = AsTableManager.Instance.GetTbl_String( 1187);	
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		//	ShowButtonEffect( m_RecallBtn.gameObject.transform.position);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_DISABLE:
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_DISABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
		//	ShowButtonEffect( m_HelloBtn.gameObject.transform.position);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
			SetVisible( m_HelloBtn.gameObject, false);
			SetVisible( m_RecallBtn.gameObject, true);
			m_RecallBtn.Text = AsTableManager.Instance.GetTbl_String( 1187);	
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
		//	ShowButtonEffect( m_RecallBtn.gameObject.transform.position);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_ENABLE:
			SetVisible( m_HelloBtn.gameObject, false);
			SetVisible( m_RecallBtn.gameObject, true);
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_ENABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
			SetVisible( m_HelloBtn.gameObject, false);
			SetVisible( m_RecallBtn.gameObject, true);
		//	ShowButtonEffect( m_HelloBtn.gameObject.transform.position);
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_DISABLE:
			SetVisible( m_HelloBtn.gameObject, false);
			SetVisible( m_RecallBtn.gameObject, true);
			m_RecallBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			break;
		case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_DISABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
			SetVisible( m_HelloBtn.gameObject, true);
			SetVisible( m_RecallBtn.gameObject, false);
			m_HelloBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		//	ShowButtonEffect( m_HelloBtn.gameObject.transform.position);
			break;
		default:
			AsNotify.Instance.MessageBox( "SetFriendData Error!!!", m_CommandState.ToString());
			break;
		}
	}

	private void CommandBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			switch( m_CommandState)
			{
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_NOTHING:
				break;
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_ENABLE:
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_ENABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
				AsCommonSender.SendFriendHello( m_FriendData.nUserUniqKey);
				break;
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_DISABLE:
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_DISABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
				break;
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
				AsCommonSender.SendFriendHello( m_FriendData.nUserUniqKey);
				break;
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_ENABLE:
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_ENABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
				AsCommonSender.SendFriendReCall( m_FriendData.nUserUniqKey);
				HideButtonEffect();
				break;
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_DISABLE:
			case eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_RECALL_DISABLE|eFRIEND_BUTTON_TYPE.eFRIEND_BUTTON_HELLO_RECV:
				break;
			}

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}


	private void ConnectRequesDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			string msg = string.Format( AsTableManager.Instance.GetTbl_String( 1905),m_FriendData.szCharName);
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String( 1203), msg, this, "OnMsgBox_ConnectReques_Ok", "OnMsgBox_ConnectReques_Cancel", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}

	public void OnMsgBox_ConnectReques_Ok()
	{
		AsCommonSender.SendFriendConnectReuqest( m_FriendData.nUserUniqKey);
		AsSocialManager.Instance.SocialData.ConnectReuqestUserName = m_FriendData.szCharName;
	}

	public void OnMsgBox_ConnectReques_Cancel()
	{
	}

	void SetDelegateImage( int nImageTableIdx)
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");

		DelegateImageData delegateImage = AsDelegateImageManager.Instance.GetDelegateImage( nImageTableIdx);
		if( null == delegateImage)
			sb.Append( "Default");
		else
			sb.Append( delegateImage.iconName);

		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
}
