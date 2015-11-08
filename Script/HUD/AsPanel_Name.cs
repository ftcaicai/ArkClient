#define NEW_DELEGATE_IMAGE
using UnityEngine;
using System.Collections;
using System.Text;

public class AsPanel_Name : MonoBehaviour
{
	public enum eNamePanelType { eNamePanelType_None = 0, eNamePanelType_User, eNamePanelType_Npc, eNamePanelType_Monster, eNamePanelType_Collect}
	
	public SpriteText NameText = null;
	public SimpleSprite NameImage_bg = null;
	public SimpleSprite NameImage_img = null;

    public delegate void CallBeforDestoryHandler(AsPanel_Name _panelName);

	[SerializeField]SimpleSprite[] genders = new SimpleSprite[0];
	[SerializeField]SimpleSprite delegateImage = null;
	[SerializeField]SimpleSprite gmMark = null;
	[SerializeField]SimpleSprite[] rankMark = new SimpleSprite[0];
	[SerializeField]SpriteText textRank = null;
	[SerializeField]SimpleSprite authorityMark = null;
	[SerializeField]UIButton btnTargetMark = null; //$yde
	[SerializeField]SimpleSprite imgTargetMark = null; //$yde
    [SerializeField]SimpleSprite imgMonsterTargetMark = null;

	private bool m_bShowCommand = false;
	private AsBaseEntity m_baseEntity = null;
	private Vector3 m_vUIPosRevision;
	private Vector3 m_vUIPosRevision_Img;
	private eNamePanelType m_eNamePanelType = eNamePanelType.eNamePanelType_None;
	private uint m_uiUserUniqueKey = 0;
	private float m_fNamePanelLayer = 15.0f;
	private bool m_bUseIcon = true;
	private string m_strSubTitleName = string.Empty;
	private string m_strName = string.Empty;
	private string m_strGroupName = string.Empty;
	private string m_strPvpGrade = string.Empty;
	private string m_strPvpGradeColor = string.Empty;
	private bool m_bShowPvpGrade = false;

	private StringBuilder m_sbName = new StringBuilder();

    public  CallBeforDestoryHandler callBeforDestory;

	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( NameText, true);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( textRank, true);
		
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	void LateUpdate()
	{
		if( true == m_bShowCommand)
		{
			if( null == m_baseEntity || null == m_baseEntity.ModelObject || true == _isMonsterDeath())
			{
				_Remove();
			}
			else
			{
				_UpdateText();
				_UpdateColor();
				_UpdatePos();
			}
			
			_CheckOption();
		}
	}
	
	public void Create( AsBaseEntity baseEntity, string strName, Color nameColor, eNamePanelType eType, uint uiUserUniqueKey, float fSize)
	{
		m_bShowCommand = true;
		m_eNamePanelType = eType;
		m_uiUserUniqueKey = uiUserUniqueKey;
		m_baseEntity = baseEntity;
		
		m_strSubTitleName = _GetSubTitleName( eType);
		m_strName = strName;
		m_strGroupName = _GetGroupName( eType);
		m_strPvpGrade = _GetPvpGrade();
#if !NEW_DELEGATE_IMAGE
		m_bShowPvpGrade = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_PvpGrade);
#endif

		gameObject.SetActiveRecursively( true);

		string strNameRes = string.Empty;
		string strNameBuf = string.Empty;
		
//		StringBuilder sb = new StringBuilder( "");

		m_sbName.Remove( 0, m_sbName.Length);

		if( m_strPvpGrade.Length > 0)
		{
			m_sbName.Append( m_strPvpGradeColor);
			m_sbName.Append( m_strPvpGrade);
		}
		
		if( m_strSubTitleName.Length > 0)
		{
			m_sbName.Append( " ");
			m_sbName.Append( _GetSubTitleColor( eType));
			m_sbName.Append( m_strSubTitleName);
		}
		
		if( m_strPvpGrade.Length > 0 || m_strSubTitleName.Length > 0)
			m_sbName.Append( " ");
		
		m_sbName.Append( nameColor.ToString());
		m_sbName.Append( strName);
		strNameBuf = m_sbName.ToString();
		
		if( m_strGroupName.Length > 0)
			strNameRes = strNameBuf + "\n<" + m_strGroupName + ">";
		else
			strNameRes = strNameBuf;
		
		NameText.name = strName;
		NameText.Text = strNameRes;
		NameText.Color = nameColor;
		
		m_vUIPosRevision.x = 0.0f;
		m_vUIPosRevision.y = NameText.BaseHeight;
		m_vUIPosRevision.z = m_fNamePanelLayer;
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == uiUserUniqueKey)
			m_vUIPosRevision.z -= 1.0f;
		
		if( eNamePanelType.eNamePanelType_Npc == eType)
			m_vUIPosRevision.y = 0.0f;

		Transform dummyLeadTop = m_baseEntity.GetDummyTransform( "DummyLeadTop");
		if( null == dummyLeadTop)
		{
			if( true == m_baseEntity.isKeepDummyObj)
			{
				Vector3 vPos = m_baseEntity.transform.position;
				vPos.y += m_baseEntity.characterController.height;
				transform.position = _WorldToUIPoint( vPos, m_vUIPosRevision);
			}
			else
				Debug.LogWarning( "DummyLeadTop is not found: " + strName);
		}
		else
		{
			transform.position = _WorldToUIPoint( dummyLeadTop.position, m_vUIPosRevision);
		}
		
		if( eNamePanelType.eNamePanelType_Npc == eType)
			_CreateImage();
		
		if( eNamePanelType.eNamePanelType_Monster == eType || eNamePanelType.eNamePanelType_Collect == eType )
			NameText.renderer.enabled = false;
		
		NameImage_bg.renderer.enabled = false;
		NameImage_img.renderer.enabled = false;
		authorityMark.renderer.enabled = false;
		btnTargetMark.collider.enabled = false; //$yde
		imgTargetMark.renderer.enabled = false; //$yde
		btnTargetMark.SetInputDelegate( OnTargetMark); //$yde

        UpdateMonsterTarkMark();

		SetRankMark();
		SetGenderMark( eType, baseEntity);
		SetDelegateImage( eType, baseEntity);

		baseEntity.namePanel = this;
	}

	public void SetDelegateImage(eNamePanelType type, AsBaseEntity entity)
	{
#if NEW_DELEGATE_IMAGE
		if( AsPanel_Name.eNamePanelType.eNamePanelType_User != type)
		{
			delegateImage.renderer.enabled = false;
			return;
		}

		AsUserEntity userEntity = entity as AsUserEntity;
		UpdateDelegateImage( userEntity.nUserDelegateImageIndex);
		UpdateGMMark( userEntity.IsGM);
#else
		delegateImage.renderer.enabled = false;
#endif
	}

	public void SetGenderMark( eNamePanelType type, AsBaseEntity entity)
	{
		if( AsPanel_Name.eNamePanelType.eNamePanelType_User != type)
		{
			genders[0].renderer.enabled = false;
			genders[1].renderer.enabled = false;
			gmMark.renderer.enabled = false;
			return;
		}

#if NEW_DELEGATE_IMAGE
		genders[0].renderer.enabled = false;
		genders[1].renderer.enabled = false;

		if( AsPanel_Name.eNamePanelType.eNamePanelType_User == type)
		{
			AsUserEntity userEntity = entity as AsUserEntity;
			UpdateGMMark( userEntity.IsGM);
		}
		else
			gmMark.renderer.enabled = false;
#else
		AsUserEntity userEntity = entity as AsUserEntity;
		UpdateGender( userEntity.UserGender);
		UpdateGMMark( userEntity.IsGM);
#endif
	}
	
	public void SetAuthority( bool authority)
	{
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_MonsterName))
			return;
		
		NameText.Color = ( true == authority) ? Color.white : Color.gray;
		authorityMark.renderer.enabled = !authority;

		if( false == authority)
		{
			int lineCount = NameText.GetDisplayLineCount();
			if( 1 < lineCount)
			{
				float textHeight = NameText.lineSpacing * lineCount;
				authorityMark.transform.localPosition = new Vector3( ( -( NameText.TotalWidth + authorityMark.width) * 0.5f), ( textHeight - authorityMark.height) * 0.5f, 0.0f);
			}
			else
			{
				authorityMark.transform.localPosition = new Vector3( ( -( NameText.TotalWidth + authorityMark.width) * 0.5f), 0.0f, 0.0f);
			}
		}
	}
	
	public void SetRankMark()
	{
		textRank.renderer.enabled = false;
		for( int i = 0; i < 5; i++)
			rankMark[i].renderer.enabled = false;

		if( AsPanel_Name.eNamePanelType.eNamePanelType_User != m_eNamePanelType)
			return;

#if !NEW_DELEGATE_IMAGE
		if( false == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_RankMark))
			return;

		int nRankPoint = 0;
		
		//if( m_uiUserUniqueKey == AsUserInfo.Instance.GetCurrentUserEntity().UniqueId)
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == m_uiUserUniqueKey)
		{
			nRankPoint = AsUserInfo.Instance.RankPoint;
		}
		else
		{
			AsUserEntity userEntity = m_baseEntity as AsUserEntity;
			nRankPoint = userEntity.RankPoint;
		}
		
		int nRes = 0;
		int nImgIndex = 0;
		
		if( nRankPoint >= 10000000)
		{
			nRes = nRankPoint / 10000000;
			nImgIndex = 4;
		}
		else if( nRankPoint >= 1000000)
		{
			nRes = nRankPoint / 1000000;
			nImgIndex = 3;
		}
		else if( nRankPoint >= 100000)
		{
			nRes = nRankPoint / 100000;
			nImgIndex = 2;
		}
		else if( nRankPoint >= 10000)
		{
			nRes = nRankPoint / 10000;
			nImgIndex = 1;
		}
		else if( nRankPoint >= 1000)
		{
			nRes = nRankPoint / 1000;
			nImgIndex = 0;
		}
		else
		{
			nRes = 1;
			nImgIndex = 0;
		}

		textRank.renderer.enabled = true;
		rankMark[nImgIndex].renderer.enabled = true;
		
		textRank.Text = nRes.ToString();

		int lineCount = NameText.GetDisplayLineCount();
		if( 1 < lineCount)
		{
			float textHeight = NameText.lineSpacing * lineCount;
			rankMark[nImgIndex].transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f), ( textHeight - gmMark.height) * 0.5f, 0.0f);
			textRank.transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f), ( textHeight - gmMark.height) * 0.5f, -0.2f);
		}
		else
		{
			rankMark[nImgIndex].transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f), 0.0f, 0.0f);
			textRank.transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f), 0.0f, -0.2f);
		}
#endif
	}
	
	#region - target mark -
	public void SetTargetMark( bool _active)
	{
		if( btnTargetMark != null)
			btnTargetMark.collider.enabled = _active; //$yde
		if( imgTargetMark != null)
			imgTargetMark.renderer.enabled = _active; //$yde
	}
	
	void OnTargetMark( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsEntityManager.Instance.MessageToPlayer( new Msg_TargetIndication( m_baseEntity, eMessageType.INPUT_TARGETING));
		}
	}
	#endregion

	#region -GMMark
	private void UpdateGMMark( bool isGM)
	{
		gmMark.renderer.enabled = isGM;

		if( false == isGM)
			return;
		
		float fWidthFlag = 0;
#if !NEW_DELEGATE_IMAGE
		if( true == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_RankMark))
			fWidthFlag = rankMark[0].width;
#endif

		int lineCount = NameText.GetDisplayLineCount();
		if( 1 < lineCount)
		{
			float textHeight = NameText.lineSpacing * lineCount;
			gmMark.transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f) + fWidthFlag, ( textHeight - gmMark.height) * 0.5f, 0.0f);
		}
		else
		{
			gmMark.transform.localPosition = new Vector3( ( ( NameText.TotalWidth + gmMark.width) * 0.5f) + fWidthFlag, 0.0f, 0.0f);
		}
	}
	#endregion
	
	private void UpdateGender( eGENDER gender)
	{
#if !NEW_DELEGATE_IMAGE
		if( eGENDER.eGENDER_NOTHING == gender)
		{
			genders[0].renderer.enabled = false;
			genders[1].renderer.enabled = false;
		}
		else
		{
			int lineCount = NameText.GetDisplayLineCount();

			if( eGENDER.eGENDER_MALE == gender)
			{
				if( 1 < lineCount)
				{
					float textHeight = NameText.lineSpacing * lineCount;
					genders[0].transform.localPosition = new Vector3( -( ( NameText.TotalWidth + genders[0].width) * 0.5f), ( textHeight - genders[0].height) * 0.5f, 0.0f);
				}
				else
				{
					genders[0].transform.localPosition = new Vector3( -( ( NameText.TotalWidth + genders[0].width) * 0.5f), 0.0f, 0.0f);
				}
				genders[0].renderer.enabled = true;
				genders[1].renderer.enabled = false;
			}
			else
			{
				if( 1 < lineCount)
				{
					float textHeight = NameText.lineSpacing * lineCount;
					genders[1].transform.localPosition = new Vector3( -( ( NameText.TotalWidth + genders[0].width) * 0.5f), ( textHeight - genders[0].height) * 0.5f, 0.0f);
				}
				else
				{
					genders[1].transform.localPosition = new Vector3( -( ( NameText.TotalWidth + genders[0].width) * 0.5f), 0.0f, 0.0f);
				}
				genders[0].renderer.enabled = false;
				genders[1].renderer.enabled = true;
			}
		}
#endif
	}

#if NEW_DELEGATE_IMAGE
	public void UpdateDelegateImage( int nUserDelegateImageIndex)
	{
		DelegateImageData data = AsDelegateImageManager.Instance.GetDelegateImage( nUserDelegateImageIndex);
		if( null == data)
		{
			delegateImage.renderer.enabled = false;
			return;
		}

		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImageSmall/");
		sb.Append( data.iconName);
		sb.Append( "_s");
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		delegateImage.renderer.material.mainTexture = tex;

		int lineCount = NameText.GetDisplayLineCount();
		
		if( 1 < lineCount)
		{
			float textHeight = NameText.lineSpacing * lineCount;
			delegateImage.transform.localPosition = new Vector3( -( ( NameText.TotalWidth + delegateImage.width) * 0.5f), ( textHeight - delegateImage.height) * 0.5f, 0.0f);
		}
		else
		{
			delegateImage.transform.localPosition = new Vector3( -( ( NameText.TotalWidth + delegateImage.width) * 0.5f), 0.0f, 0.0f);
		}

		delegateImage.renderer.enabled = true;
	}
#endif
	
	public void OnEnable()
	{
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	public void OnDisable()
	{
		if( true == m_bShowCommand)
			transform.position = Vector3.zero;
	}
	
	public void SetSubTitleName(string strSubTitleName)
	{
		if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == m_uiUserUniqueKey)
		{
			if( true == AsUserInfo.Instance.SavedCharStat.bSubtitleHide)
				m_strSubTitleName = "";
			else
				m_strSubTitleName = strSubTitleName;
		}
		else
		{
			AsUserEntity userEntity = m_baseEntity as AsUserEntity;
			if( true == userEntity.SubTitleHide)
				m_strSubTitleName = "";
			else
				m_strSubTitleName = strSubTitleName;
		}
		
		_ApplyName();
		
		SetRankMark();

#if NEW_DELEGATE_IMAGE
		AsUserEntity userEntity2 = m_baseEntity as AsUserEntity;
		UpdateDelegateImage( userEntity2.nUserDelegateImageIndex);
#endif
	}
	
	public void SetGuildName(string strGuildName)
	{
		m_strGroupName = strGuildName;
		_ApplyName();
		
		SetRankMark();

#if NEW_DELEGATE_IMAGE
		AsUserEntity userEntity = m_baseEntity as AsUserEntity;
		UpdateDelegateImage( userEntity.nUserDelegateImageIndex);
#endif
	}
	
	public void SetUserName(string strName)
	{
		if( eNamePanelType.eNamePanelType_User != m_eNamePanelType)
			return;
		
		m_strName = strName;
		
		_ApplyName();
		
		AsUserEntity userEntity = m_baseEntity as AsUserEntity;
		UpdateGender( userEntity.UserGender);
		SetRankMark();
		UpdateGMMark( userEntity.IsGM);
#if NEW_DELEGATE_IMAGE
		UpdateDelegateImage( userEntity.nUserDelegateImageIndex);
#endif
	}
	
	public void SetPetName(string strName)
	{
		m_strName = strName;
		NameText.Text = strName;
	}
	
	// < private
	private Vector3 _WorldToUIPoint(Vector3 vWorldPos, Vector3 vUIPosRevision)
	{
		Vector3 vScreenPos = _WorldToScreenPoint( vWorldPos);
		Vector3 vRes = _ScreenPointToUIRay( vScreenPos, vUIPosRevision);
		return vRes;
	}

	private Vector3 _WorldToScreenPoint(Vector3 vWorldPos)
	{
		return CameraMgr.Instance.WorldToScreenPoint( vWorldPos);
	}
	
	private Vector3 _ScreenPointToUIRay(Vector3 vScreenPos, Vector3 vUIPosRevision)
	{
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		vRes.x += vUIPosRevision.x;
		vRes.y += vUIPosRevision.y;
		vRes.z = vUIPosRevision.z;
		return vRes;
	}
	
	private void _Remove()
	{
        if (callBeforDestory != null)
            callBeforDestory(this);

		transform.parent = null;
		DestroyImmediate( gameObject);
	}
	
	private void _UpdateText()
	{
	}
	
	private void _UpdateColor()
	{
		if( eNamePanelType.eNamePanelType_User != m_eNamePanelType)
			return;

		if (AsFrameSkipManager.Instance.IsFrameSkip_LateUpdate () == true) 
			return;

		// not party
		if( null == AsPartyManager.Instance.GetPartyMember( m_uiUserUniqueKey))
		{
			if( false == TargetDecider.CheckCurrentMapIsArena())
			{
				if( AsHUDController.Instance.panelManager.m_NameColor_User != NameText.Color)
				{
					NameText.Color = AsHUDController.Instance.panelManager.m_NameColor_User;
					_ApplyName();
				}
			}
			else
			{
				if( true == AsPvpDlgManager.Instance.IsOpenPvpUserInfoDlg || true == AsPvpDlgManager.Instance.IsOpenPvpCountDlg)
				{
					if( AsHUDController.Instance.panelManager.m_NameColor_User != NameText.Color)
					{
						NameText.Color = AsHUDController.Instance.panelManager.m_NameColor_User;
						_ApplyName( false);
					}
				}
				else
				{
					if( AsHUDController.Instance.panelManager.m_NameColor_Monster_Offensive != NameText.Color)
					{
						NameText.Color = AsHUDController.Instance.panelManager.m_NameColor_Monster_Offensive;
						_ApplyName( false);
					}
				}
			}
		}
		else // party
		{
			if( AsHUDController.Instance.panelManager.m_NameColor_Party != NameText.Color)
			{
				NameText.Color = AsHUDController.Instance.panelManager.m_NameColor_Party;
				_ApplyName( false);
			}
		}
	}
	
	private void _UpdatePos()
	{
		Vector3 vScreenPos = Vector3.zero;
		Transform dummyLeadTop = m_baseEntity.GetDummyTransform( "DummyLeadTop");
		if( null == dummyLeadTop)
		{
			if( true == m_baseEntity.isKeepDummyObj)
			{
				Vector3 vPos = m_baseEntity.transform.position;
				vPos.y += m_baseEntity.characterController.height;
				vScreenPos = _WorldToScreenPoint( vPos);
			}
		}
		else
		{
			vScreenPos = _WorldToScreenPoint( dummyLeadTop.position);
		}
		
		if( false == NameImage_bg.renderer.enabled)
			transform.position =  _ScreenPointToUIRay( vScreenPos, m_vUIPosRevision);
		else
			transform.position =  _ScreenPointToUIRay( vScreenPos, m_vUIPosRevision_Img);
	}
	
	private bool _isMonsterDeath()
	{
		if( eNamePanelType.eNamePanelType_Monster != m_eNamePanelType)
			return false;

		AsNpcEntity npcEntity = m_baseEntity as AsNpcEntity;
		float fHP = npcEntity.GetProperty<float>(eComponentProperty.HP_CUR);
		if( fHP < 1f)
			return true;
		
		return false;
	}
	
	private string _GetGroupName(eNamePanelType eType)
	{
		string strBuf = string.Empty;
		
		if( eNamePanelType.eNamePanelType_User == eType)
		{
			// guild check
			string strGuildName = m_baseEntity.GetProperty<string>( eComponentProperty.GUILD_NAME);
			if( strGuildName.Length > 0)
			{
//				strBuf = '<' + strGuildName + '>';
				strBuf = strGuildName;
			}
		}
		else if( eNamePanelType.eNamePanelType_Npc == eType || eNamePanelType.eNamePanelType_Monster == eType || eNamePanelType.eNamePanelType_Collect == eType)
		{
			Tbl_Npc_Record NpcRecord = AsTableManager.Instance.GetTbl_Npc_Record( ((AsNpcEntity)m_baseEntity).TableIdx);
			if( null != NpcRecord)
			{
				if( false == NpcRecord.NpcGName.ToLower().Equals( "none"))
					strBuf = '<' + NpcRecord.NpcGName + '>';
			}
		}
		
		return strBuf;
	}
	
	private string _GetSubTitleName(eNamePanelType eType)
	{
		string strBuf = string.Empty;
		
		if( eNamePanelType.eNamePanelType_User == eType)
		{
			AsUserEntity userEntity = m_baseEntity as AsUserEntity;
			
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == m_uiUserUniqueKey)
			{
				if( true == AsUserInfo.Instance.SavedCharStat.bSubtitleHide)
					return "";
			}
			else
			{
				if( true == userEntity.SubTitleHide)
					return "";
			}
			
			if( 0 < userEntity.DesignationID)
			{
				DesignationData data = AsDesignationManager.Instance.GetDesignation( userEntity.DesignationID);
				if( null == data)
				{
					Debug.Log( "AsPanel_Name::_GetBubTitleName(), Not Found SubTitleNameID: " + userEntity.DesignationID);
					return "";
				}

				strBuf = AsTableManager.Instance.GetTbl_String( data.name);
			}
		}
		
		return strBuf;
	}
	
	private string _GetSubTitleColor(eNamePanelType eType)
	{
		string strBuf = AsHUDController.Instance.panelManager.m_NameColor_User.ToString();
		
		if( eNamePanelType.eNamePanelType_User == eType)
		{
			if( null == AsPartyManager.Instance.GetPartyMember( m_uiUserUniqueKey))
			{
				AsUserEntity userEntity = m_baseEntity as AsUserEntity;
				if( 0 < userEntity.DesignationID)
				{
					DesignationData data = AsDesignationManager.Instance.GetDesignation( userEntity.DesignationID);
					if( null != data)
						strBuf = data.nameColor;
				}
			}
		}
		
		return strBuf;
	}
	
	private string _GetPvpGrade()
	{
		if( eNamePanelType.eNamePanelType_User != m_eNamePanelType)
			return "";
#if !NEW_DELEGATE_IMAGE
		if( true == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_PvpGrade))
		{
			int nPvpPoint = 0;
			uint nPvpRate = 0;
			
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == m_uiUserUniqueKey)
			{
				nPvpPoint = AsUserInfo.Instance.YesterdayPvpPoint;
				nPvpRate = AsUserInfo.Instance.YesterdayPvpRankRate;
			}
			else
			{
				AsUserEntity userEntity = m_baseEntity as AsUserEntity;
				nPvpPoint = userEntity.YesterdayPvpPoint;
				nPvpRate = userEntity.YesterdayPvpRankRate;
			}
			
			int nID = AsPvpManager.Instance.FindPvpGradeID( nPvpPoint, nPvpRate);
			Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( nID);
			
			if( null == record)
			{
				Debug.Log( "AsPanel_Name::_GetPvpGrade(): record == null, nID: " + nID);
				return "";
			}
			
			// color
			if( null == AsPartyManager.Instance.GetPartyMember( m_uiUserUniqueKey))
				m_strPvpGradeColor = record.GradeColor;
			else
				m_strPvpGradeColor = "";
			
			if( true == TargetDecider.CheckCurrentMapIsArena())
				m_strPvpGradeColor = "";
			
			if( 1 == nID)
				return "";
			else
				return AsTableManager.Instance.GetTbl_String( record.GradeNameID);
		}
#endif
		
		return "";
	}

	private void _CreateImage()
	{
		Tbl_Npc_Record NpcRecord = AsTableManager.Instance.GetTbl_Npc_Record( ((AsNpcEntity)m_baseEntity).TableIdx);
		if( null == NpcRecord)
			return;
		
		int nIndex = NpcRecord.NpcIcon - 1;
		
		if( nIndex < 0 || nIndex > 25)
		{
			m_bUseIcon = false;
			return;
		}
		
		int nLow = nIndex / 5;
		int nCol = nIndex % 5;
		
		int nX = (int)(NameImage_img.lowerLeftPixel.x) + 50 * nCol;
		int nY = (int)(NameImage_img.lowerLeftPixel.y) + 50 * nLow;
		
		NameImage_img.SetLowerLeftPixel( nX, nY);

		m_vUIPosRevision_Img.x = 0.0f;
		m_vUIPosRevision_Img.y = NameImage_bg.height * 0.5f;
		m_vUIPosRevision_Img.z = m_fNamePanelLayer;
	}
	
	private void _CheckOption()
	{
		if( null == AsHudDlgMgr.Instance)
			return;
		
		if( eNamePanelType.eNamePanelType_Npc == m_eNamePanelType)
		{
			bool isShowNpcName = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_NpcName);
			
			if( null != NameText && isShowNpcName != NameText.renderer.enabled)
			{
				if( null != NameText)
					NameText.renderer.enabled = isShowNpcName;
				if( null != NameImage_bg && true == m_bUseIcon)
					NameImage_bg.renderer.enabled = !isShowNpcName;
				if( null != NameImage_img && true == m_bUseIcon)
					NameImage_img.renderer.enabled = !isShowNpcName;
			}
		}
		if( eNamePanelType.eNamePanelType_Monster == m_eNamePanelType || eNamePanelType.eNamePanelType_Collect == m_eNamePanelType)
		{
			bool isShowMonsterName = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_MonsterName);
			
			if( null != NameText && isShowMonsterName != NameText.renderer.enabled)
			{
				if( eNamePanelType.eNamePanelType_Collect == m_eNamePanelType && null != m_baseEntity.collectionMark)
				{
					if( true == isShowMonsterName && true == m_baseEntity.collectionMark.Visible)
					{
						if( false == NameText.renderer.enabled)
							NameText.renderer.enabled = true;
					}
					else
					{
						if( true == NameText.renderer.enabled)
							NameText.renderer.enabled = false;
					}
				}
				else
				{
					NameText.renderer.enabled = isShowMonsterName;
					if( false == isShowMonsterName && true == authorityMark.renderer.enabled)
						authorityMark.renderer.enabled = false;
				}
			}
		}

#if !NEW_DELEGATE_IMAGE
		bool bPvpGradeOption = AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_PvpGrade);
		
		if( m_bShowPvpGrade != bPvpGradeOption && eNamePanelType.eNamePanelType_User == m_eNamePanelType)
		{
			m_bShowPvpGrade = bPvpGradeOption;
			
			_ApplyName();
			AsUserEntity userEntity = m_baseEntity as AsUserEntity;
			UpdateGender( userEntity.UserGender);
			SetRankMark();
			UpdateGMMark( userEntity.IsGM);
		}

		if( eNamePanelType.eNamePanelType_User == m_eNamePanelType)
		{
			if( true == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_RankMark))
			{
				if( null != textRank && false == textRank.renderer.enabled)
				{
					AsUserEntity userEntity = m_baseEntity as AsUserEntity;
					SetRankMark();
					UpdateGMMark( userEntity.IsGM);
				}
			}
			else
			{
				if( null != textRank && true == textRank.renderer.enabled)
				{
					AsUserEntity userEntity = m_baseEntity as AsUserEntity;
					SetRankMark();
					UpdateGMMark( userEntity.IsGM);
				}
			}
		}
#endif
	}
	
	private void _ApplyName( bool applySubTitleColor = true)
	{
		if( eNamePanelType.eNamePanelType_User != m_eNamePanelType)
			return;
		
//		string strNameRes = string.Empty;
//		string strNameBuf = string.Empty;
		string strSubTitleColor = "";
		m_strPvpGrade = _GetPvpGrade();
		
		if( true == applySubTitleColor)
			strSubTitleColor = _GetSubTitleColor( eNamePanelType.eNamePanelType_User);

		StringBuilder sb = new StringBuilder();
		
		if( m_strPvpGrade.Length > 0)
		{
			sb.Append( m_strPvpGradeColor);
			sb.Append( m_strPvpGrade);
		}
		
		if( m_strSubTitleName.Length > 0)
		{
			sb.Append( " ");
			sb.Append( strSubTitleColor);
			sb.Append( m_strSubTitleName);
		}
		
		if( m_strPvpGrade.Length > 0 || m_strSubTitleName.Length > 0)
			sb.Append( " ");
		
		sb.Append( AsHUDController.Instance.panelManager.m_NameColor_User.ToString());
		sb.Append( m_strName);

		if( 0 < m_strGroupName.Length)
		{
			sb.AppendLine();
			sb.Append( '<');
			sb.Append( m_strGroupName);
			sb.Append( '>');
		}

		NameText.Text = sb.ToString();

//		strNameBuf = sb.ToString();
//
//		if( m_strGroupName.Length > 0)
//			strNameRes = strNameBuf + "\n" + '<' + m_strGroupName + '>';
//		else
//			strNameRes = strNameBuf;
//		
//		NameText.Text = strNameRes;
		m_vUIPosRevision.y = NameText.BaseHeight;
	}

    public void UpdateMonsterTarkMark()
    {
        if (m_baseEntity.FsmType != eFsmType.MONSTER)
        {
            if (imgMonsterTargetMark != null)
                imgMonsterTargetMark.renderer.enabled = false;

            return;
        }

        AsNpcEntity npcEntity = m_baseEntity as AsNpcEntity;

        int monsterID = m_baseEntity.GetProperty<int>(eComponentProperty.MONSTER_ID);
        int monsterKindID = m_baseEntity.GetProperty<int>(eComponentProperty.MONSTER_KIND_ID);

        if (ArkQuestmanager.instance.CheckShowTargetMark(monsterID, monsterKindID, npcEntity.m_isChampion))
        {
            if (true == AsGameMain.GetOptionState(OptionBtnType.OptionBtnType_MonsterName))
            {
                Vector3 namePos = NameText.transform.localPosition;
                Vector3 markPos = new Vector3(namePos.x - (NameText.TotalWidth * 0.5f + imgMonsterTargetMark.width * 0.5f), namePos.y, namePos.z);
                imgMonsterTargetMark.transform.localPosition = markPos;
            }
            else
            {
                imgMonsterTargetMark.transform.localPosition = Vector3.zero;
            }

            imgMonsterTargetMark.renderer.enabled = true;
        }
        else
            imgMonsterTargetMark.renderer.enabled = false;
    }
}
