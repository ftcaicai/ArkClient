using UnityEngine;
using System.Collections;
using System.Text;
public class AsPartyMemberDlg : MonoBehaviour
{
	public SpriteText m_UserName = null;
	public SpriteText m_UserLevel = null;
	public SpriteText m_Channel = null;
	public SimpleSprite[] m_Class_Image;
	public SimpleSprite m_Captain_Image;
	public SimpleSprite m_Offline_Image;
	public SimpleSprite m_Target_Image;
	public UIProgressBar m_hpProgress = null;
	public UIProgressBar conditionProgress = null;
	public SpriteText conditionDigit = null;
	public UIButton m_SelectBtn;
	public float m_maxHP = 0.0f;
	public float m_curHP = 0.0f;
	public ushort m_nSessionIdx;///Zero is OffLine User
	
	public GameObject goChild;

	private uint m_nCharUniqKey;
	public uint CharUniqKey
	{
		get { return m_nCharUniqKey; }
		set { m_nCharUniqKey = value; }
	}

	public ushort m_nBackupSessionIdx;
	public uint m_nBackupCharUniqKey;

	public int m_eRace;
	public int m_eClass;

	public AsPartyBuffUI m_PartyBuffUI;

	private bool m_IsCaptain = false;
	public bool IsCaptain
	{
		get { return m_IsCaptain; }
		set { m_IsCaptain = value; }
	}

	private bool m_IsOffLine = false;
	public bool IsOffLine
	{
		get { return m_IsOffLine; }
		set { m_IsOffLine = value; }
	}

	private bool m_IsTarget = false;
	public bool IsTarget
	{
		get { return m_IsTarget; }
		set { m_IsTarget = value; }
	}

	public SimpleSprite portrait = null;

	void Clear()
	{
		m_nCharUniqKey = 0;
	}

	public void PartyBuffClear()
	{
		m_PartyBuffUI.Clear();
	}
	
	public void SetChildEnable( bool _bool )
	{
		if( goChild.active == _bool )
			return;
		
		goChild.SetActiveRecursively(_bool);
	}
	public void Close()
	{
		Clear();
		m_PartyBuffUI.SetShowUI();
		gameObject.SetActiveRecursively( false);

		Debug.Log( "AsPartyMemberDlg.Close");
	}

	public bool Open()
	{
		gameObject.SetActiveRecursively( true);
		gameObject.active = true;

		SetCaptain( false);
		SetTarget( false);

		return true;
	}

	// Awake
	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		m_SelectBtn.SetInputDelegate( SelectBtnDelegate);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_UserName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_UserLevel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Channel);
	}

	// Update is called once per frame
	void Update()
	{
	}

	void SetVisible( GameObject obj, bool visible)
	{
		obj.SetActiveRecursively( visible);
		obj.active = visible;
	}

	public void SetCaptain( bool bIs)
	{
		IsCaptain = bIs;
		SetVisible( m_Captain_Image.gameObject, bIs);
	}

	public void SetOffLine( bool bIs, bool setImg = true)
	{
		IsOffLine = bIs;
	//	CloseAllClassImage();
		if (setImg == true)
			SetVisible( m_Offline_Image.gameObject, bIs);

		if( IsOffLine)
			PartyBuffClear();
	}

	public void SetTarget( bool bIs, bool setImg = true)
	{
		m_IsTarget = bIs;
		
		if (setImg == true)
			SetVisible( m_Target_Image.gameObject, bIs);
	}

	public void CloseAllClassImage()
	{
		for( int i = 0; i < m_Class_Image.Length; ++i)
		{
			if( m_Class_Image[i] != null)
				SetVisible( m_Class_Image[i].gameObject, false);
		}
	}

	public void SetClassImage( int index)
	{
		CloseAllClassImage();
		if( m_Class_Image[index] != null)
			SetVisible( m_Class_Image[index].gameObject, true);
	}

	public void PartyUserAdd( AS_PARTY_USER data)
	{
		m_UserName.Text = data.strCharName;

		StringBuilder sb = new StringBuilder();
		sb.Append( "Lv.");
		sb.Append( data.nLevel.ToString());
		m_UserLevel.Text =sb.ToString();

		m_Channel.Text = data.strChannelName;//20952

		m_nSessionIdx = ( ushort)data.nSessionIdx;
		m_nCharUniqKey = data.nCharUniqKey;

		m_curHP = data.fHpCur;
		m_maxHP = data.fHpMax;
		SetHP( m_curHP / m_maxHP);

		SetClassImage( data.eClass-1);
		SetDelegateImage( data.nImageTableIdx);
		Open();
	}

	public void PartyUserInfo( AS_PARTY_USER data)
	{
		m_UserLevel.Text = "Lv." + data.nLevel.ToString();
		m_nCharUniqKey = data.nCharUniqKey;
		m_curHP = data.fHpCur;
		m_maxHP = data.fHpMax;
		m_Channel.Text = data.strChannelName;//20952

		SetHP( m_curHP / m_maxHP);
		if( 0.0f >= m_curHP)//Die
			SetTarget( false);
		SetCondition( data.nCondition);
		SetDelegateImage( data.nImageTableIdx);
	}

	public void SetCondition( uint nCondition)
	{
		if( 0 >= nCondition)
			m_UserName.Color = Color.black;
		else
			m_UserName.Color = Color.white;
	}

	public void SetHP( float hp)
	{
		if( hp == m_hpProgress.Value)
			return;
		m_hpProgress.Value = hp;
	}

	public void AlertHP()
	{
		AsSpriteBlinker effect = m_hpProgress.GetComponentInChildren<AsSpriteBlinker>();
		if( null != effect)
			effect.Play();
	}

	public void PartyUserBuff( body1_SC_CHAR_BUFF data)
	{		
		m_PartyBuffUI.ReciveBuff( data, goChild.active );
	}

	public void PartyUserDeBuff( body_SC_CHAR_DEBUFF data)
	{	
		m_PartyBuffUI.DeleteBuff( data, goChild.active);
	}

	private void SelectBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == userEntity)
				return;

			AsPlayerFsm playerFsm = userEntity.GetComponent<AsPlayerFsm>();
			if( null == playerFsm)
				return;

			AsUserEntity otherUser = AsEntityManager.Instance.GetUserEntityByUniqueId( m_nCharUniqKey) as AsUserEntity;
			if( null != otherUser)
			{
				if( Vector3.Distance( userEntity.transform.position, otherUser.transform.position) > playerFsm.UserReleaseDistance)
				{
					//파티원이 멀어서 선택되지 않습니다.
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String( 284), eCHATTYPE.eCHATTYPE_SYSTEM);
					return;
				}
				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_OtherUserClick( otherUser.UniqueId));
			}
			else
			{
				if( IsOffLine)
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String( 324), eCHATTYPE.eCHATTYPE_SYSTEM);//파티원이 로그아웃 상태입니다
				else
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String( 284), eCHATTYPE.eCHATTYPE_SYSTEM);//파티원이 멀어서 선택되지 않습니다.
			}
		}
	}

	public void SetConditionValue( uint nCondition)
	{
		if( null == AsEventManager.Instance.Get( eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION))
		{
			SetConditionEventState( false);
		}
		else
		{
			float maxCondition = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 11).Value;
			float curCondition = ( float)nCondition;

			float condition = curCondition / maxCondition;
			if( null != conditionDigit)
				conditionDigit.Text = string.Format( "{0:D}%", ( int)( condition * 100.0f));

			if( condition == conditionProgress.Value)
				return;

			conditionProgress.Value = condition;
		}
	}

	public void SetConditionEventState( bool flag)
	{
		conditionProgress.gameObject.SetActiveRecursively( !flag);
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
	
	public void SetName( string strName )
	{
		m_UserName.Text = strName;
	}	
	
}
