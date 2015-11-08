using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AsCustomizingPanel : AsCharacterCreatePanel
{
	[SerializeField] SpriteText balloon = null;
	[SerializeField] UIRadioBtn[] genderIcons = new UIRadioBtn[0];
	[SerializeField] UIRadioBtn[] hairStyles = new UIRadioBtn[0];
	[SerializeField] UIRadioBtn[] hairColors = new UIRadioBtn[0];
	[SerializeField] UIRadioBtn[] bodyColors = new UIRadioBtn[0];
	[SerializeField] UIRadioBtn[] pointColors = new UIRadioBtn[0];
	[SerializeField] UIRadioBtn[] gloveColors = new UIRadioBtn[0];
	[SerializeField] UITextField editName = null; // ilmeda, 20120814
	[SerializeField] SpriteText tempMsg = null;
	[SerializeField] UIButton randomBtn = null;
	[SerializeField] SpriteText info = null;

	private AsCharacterCreateGenderData m_CharCreateGenderData;
	private AsCharacterCreateClassData m_CharCreateClassData;
	private AsUserEntity m_ModelEntity;
	[SerializeField] GameObject charDummy = null;
	[SerializeField] GameObject footPlate = null;
	[SerializeField] float rotSpeed = 100.0f;
	private float rotAccum = 0.0f;

	// Use this for initialization
	void OnEnable()
	{
		DisplayModeling();
		
		// ilmeda 20120814
		Input.imeCompositionMode = IMECompositionMode.On;
		editName.Text = "";
	}
	
	void Start()
	{
		balloon.Text = AsTableManager.Instance.GetTbl_String(416);
		randomBtn.Text = AsTableManager.Instance.GetTbl_String(1184);
		info.Text = AsTableManager.Instance.GetTbl_String(2094);
		
		// ilmeda, 20120814
		AsLanguageManager.Instance.SetFontFromSystemLanguage( editName);
		tempMsg.Text = AsTableManager.Instance.GetTbl_String(1406);
		
		editName.SetFocusDelegate( OnFocusName);
		editName.SetCommitDelegate( OnCommitName);
		editName.SetValidationDelegate( OnValidateName);
	}

	private void OnFocusName( UITextField field)
	{
		tempMsg.gameObject.SetActiveRecursively( false);
		prtFramework.SendMessage( "CloseAllAlertDlg");
	}

	private void OnCommitName( IKeyFocusable control)
	{
		if( 0 >= editName.Text.Length)
			tempMsg.gameObject.SetActiveRecursively( true);
	}
	
	private string OnValidateName( UITextField field, string text, ref int insPos)
	{
		while( true)
		{
			int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount( text);
			int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount( System.Text.UTF8Encoding.UTF8.GetBytes( text));
			if( ( byteCount <= 24) && ( charCount <= 12))
				break;

			text = text.Remove( text.Length - 1);
		}

		Vector3 orgPos = field.spriteText.transform.localPosition;
		field.spriteText.transform.localPosition = new Vector3( 0.0f, orgPos.y, 0.0f);

		return Regex.Replace( text, "\\s+", "");
	}
	
	void OnDisable()
	{
		ReleaseModeling();
		
		// ilmeda 20120814
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}

	void OnRandomBtn()
	{
		if(AsEntityManager.Instance.ModelLoading == true)
			return;
		
		if( true == PartsRoot.s_isLoading )
			return;
		
		StartCoroutine( RandomSetting());
	}

	private IEnumerator RandomSetting()
	{
		AsCharacterCreateFramework.curHairStyle = Random.Range( 0, (int)( CHARACTER_HAIR_STYLE.STYLE_3 + 1));
		AsCharacterCreateFramework.curHairColor = Random.Range( 0, (int)( CHARACTER_COLOR.COLOR_3 + 1));
		AsCharacterCreateFramework.curBodyColor = Random.Range( 0, (int)( CHARACTER_COLOR.COLOR_3 + 1));
		AsCharacterCreateFramework.curPointColor = Random.Range( 0, (int)( CHARACTER_COLOR.COLOR_3 + 1));
		AsCharacterCreateFramework.curGloveColor = Random.Range( 0, (int)( CHARACTER_COLOR.COLOR_3 + 1));

		// hair style
		foreach( UIRadioBtn style in hairStyles)
			style.Value = false;
		hairStyles[ AsCharacterCreateFramework.curHairStyle].Value = true;
		
		// hair color
		foreach( UIRadioBtn hairColor in hairColors)
			hairColor.Value = false;
		hairColors[ AsCharacterCreateFramework.curHairColor].Value = true;
		
		// bodyColor
		foreach( UIRadioBtn bodyColor in bodyColors)
			bodyColor.Value = false;
		bodyColors[ AsCharacterCreateFramework.curBodyColor].Value = true;
		
		// point color
		foreach( UIRadioBtn pointColor in pointColors)
			pointColor.Value = false;
		pointColors[ AsCharacterCreateFramework.curPointColor].Value = true;
		
		// glove color
		foreach( UIRadioBtn gloveColor in gloveColors)
			gloveColor.Value = false;
		gloveColors[ AsCharacterCreateFramework.curGloveColor].Value = true;

		while( AsEntityManager.Instance.ModelLoading == true)
			yield return null;

		ChangeHairColor();
		ChangeBodyColor();
		ChangePointColor();
		ChangeGloveColor();

		while( true == PartsRoot.s_isLoading)
			yield return null;

		genderIcons[0].controlIsEnabled = true;
		genderIcons[1].controlIsEnabled = true;
	}

	public override void Initialize()
	{
#if false
		// gender
		foreach( UIRadioBtn icon in genderIcons)
			icon.Value = false;
		genderIcons[ AsCharacterCreateFramework.curGender - 1].Value = true;

		StartCoroutine( RandomSetting());
#endif
		AsCharacterCreateFramework.curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_1;
		AsCharacterCreateFramework.curHairColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curBodyColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curPointColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curGloveColor = (int)CHARACTER_COLOR.COLOR_1;
		
		// gender
		foreach( UIRadioBtn icon in genderIcons)
			icon.Value = false;
		genderIcons[ AsCharacterCreateFramework.curGender - 1].Value = true;
		
		// hair style
		foreach( UIRadioBtn style in hairStyles)
			style.Value = false;
		hairStyles[ AsCharacterCreateFramework.curHairStyle].Value = true;
		
		// hair color
		foreach( UIRadioBtn hairColor in hairColors)
			hairColor.Value = false;
		hairColors[ AsCharacterCreateFramework.curHairColor].Value = true;
		
		// bodyColor
		foreach( UIRadioBtn bodyColor in bodyColors)
			bodyColor.Value = false;
		bodyColors[ AsCharacterCreateFramework.curBodyColor].Value = true;
		
		// point color
		foreach( UIRadioBtn pointColor in pointColors)
			pointColor.Value = false;
		pointColors[ AsCharacterCreateFramework.curPointColor].Value = true;
		
		// glove color
		foreach( UIRadioBtn gloveColor in gloveColors)
			gloveColor.Value = false;
		gloveColors[ AsCharacterCreateFramework.curGloveColor].Value = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		_rotateEntity();
	}
	
	private void InitDefault()
	{
		hairStyles[ AsCharacterCreateFramework.curHairStyle].Value = false;
		hairColors[ AsCharacterCreateFramework.curHairColor].Value = false;
		bodyColors[ AsCharacterCreateFramework.curBodyColor].Value = false;
		pointColors[ AsCharacterCreateFramework.curPointColor].Value = false;
		gloveColors[ AsCharacterCreateFramework.curGloveColor].Value = false;
		
		AsCharacterCreateFramework.curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_1;
		AsCharacterCreateFramework.curHairColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curBodyColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curGloveColor = (int)CHARACTER_COLOR.COLOR_1;
		AsCharacterCreateFramework.curPointColor = (int)CHARACTER_COLOR.COLOR_1;
		
		hairStyles[ AsCharacterCreateFramework.curHairStyle].Value = true;
		hairColors[ AsCharacterCreateFramework.curHairColor].Value = true;
		bodyColors[ AsCharacterCreateFramework.curBodyColor].Value = true;
		pointColors[ AsCharacterCreateFramework.curPointColor].Value = true;
		gloveColors[ AsCharacterCreateFramework.curGloveColor].Value = true;

		rotAccum = 0.0f;
	}
	
	private void ChangeHairColor()
	{
		if( AsEntityManager.Instance.ModelLoading == true)//$yde
			return;
		
		if( null == AsEntityManager.Instance.UserEntity)
			return;
		
		if( AsCharacterCreateFramework.curHairColor >= m_CharCreateGenderData.hairColors.Count)
			return;
		
		if( AsCharacterCreateFramework.curHairStyle >= m_CharCreateGenderData.hairTypes.Count)
			return;
		
		int iType = (int)m_CharCreateGenderData.hairTypes[AsCharacterCreateFramework.curHairStyle];
		int iColor = (int)m_CharCreateGenderData.hairColors[AsCharacterCreateFramework.curHairColor];
		
		AsEntityManager.Instance.UserEntity.AttachParts( Item.eEQUIP.Hair, iType + iColor);
	}

	private void ChangeBodyColor()
	{
		if( AsEntityManager.Instance.ModelLoading == true)//$yde
			return;
		
		if( null == AsEntityManager.Instance.UserEntity)
			return;
		
		if( AsCharacterCreateFramework.curBodyColor >= m_CharCreateGenderData.bodyColors.Count)
			return;
		
		AsEntityManager.Instance.UserEntity.AttachParts( Item.eEQUIP.Armor, (int)m_CharCreateGenderData.bodyColors[AsCharacterCreateFramework.curBodyColor]);
	}
	
	private void ChangePointColor()
	{
		if( AsEntityManager.Instance.ModelLoading == true)//$yde
			return;
		
		if( null == AsEntityManager.Instance.UserEntity)
			return;
		
		if( AsCharacterCreateFramework.curPointColor >= m_CharCreateGenderData.pointColors.Count)
			return;
		
		AsEntityManager.Instance.UserEntity.AttachParts( Item.eEQUIP.Point, (int)m_CharCreateGenderData.pointColors[AsCharacterCreateFramework.curPointColor]);
	}

	private void ChangeGloveColor()
	{
		if( AsEntityManager.Instance.ModelLoading == true)//$yde
			return;
		
		if( null == AsEntityManager.Instance.UserEntity)
			return;

		if( AsCharacterCreateFramework.curGloveColor >= m_CharCreateGenderData.gloveColors.Count)
			return;

		AsEntityManager.Instance.UserEntity.AttachParts( Item.eEQUIP.Gloves,(int)m_CharCreateGenderData.gloveColors[AsCharacterCreateFramework.curGloveColor]);
	}
	
	//$yde
	public void DisplayModeling()
	{
		genderIcons[0].controlIsEnabled = false;
		genderIcons[1].controlIsEnabled = false;

		StartCoroutine( "SetModeling");
	}
	
	IEnumerator SetModeling()
	{
		AsLoadingIndigator.Instance.ShowIndigator( string.Empty);

		while( false == AsEntityManager.Instance.gameObject.active)
			yield return null;
		
		ReleaseModeling();
		
		eRACE race = (eRACE)AsCharacterCreateFramework.curRace;
		eCLASS job = (eCLASS)AsCharacterCreateFramework.curJob;
		eGENDER gender = (eGENDER)AsCharacterCreateFramework.curGender;
		
		sCHARVIEWDATA viewData = new sCHARVIEWDATA();
		viewData.eRace = race;
		viewData.eClass = job;
		viewData.eGender = gender;
		
		m_CharCreateGenderData = AsTableManager.Instance.GetTbl_CreateCharacter_GenderData( race, job, gender);
		if( null == m_CharCreateGenderData)
			Debug.LogError( "AsCustomizingPanel::SetModeling()[ con't find m_CharCreateGenderData ] race : " + race + " job : "+ job +" gender : " +gender);
		
		m_CharCreateClassData = AsTableManager.Instance.GetTbl_CreateCharacter_ClassData( race, job);
		if( null == m_CharCreateClassData)
			Debug.LogError( "AsCustomizingPanel::SetModeling()[ con't find m_CharCreateClassData ] race : " + race + " job : "+ job);
		
		int iHair = 0;
		if( null != m_CharCreateGenderData && null != m_CharCreateClassData)
		{
			iHair = (int)m_CharCreateGenderData.hairTypes[0] + (int)m_CharCreateGenderData.hairColors[0];
			int ibody = (int)m_CharCreateGenderData.bodyColors[0];
			int iPoint = (int)m_CharCreateGenderData.pointColors[0];
			int iGlove = (int)m_CharCreateGenderData.gloveColors[0];
			
			viewData.sNormalItemVeiw_1 = new sITEMVIEW();
			viewData.sNormalItemVeiw_1.nItemTableIdx = m_CharCreateClassData.commonProp.weapon;
			
			viewData.sNormalItemVeiw_2 = new sITEMVIEW();
			viewData.sNormalItemVeiw_2.nItemTableIdx = 0;
			
			viewData.sNormalItemVeiw_3 = new sITEMVIEW();
			viewData.sNormalItemVeiw_3.nItemTableIdx = ibody;
			
			viewData.sNormalItemVeiw_4 = new sITEMVIEW();
			viewData.sNormalItemVeiw_4.nItemTableIdx = iGlove;
			
			viewData.sNormalItemVeiw_5 = new sITEMVIEW();
			viewData.sNormalItemVeiw_5.nItemTableIdx = iPoint;
		}
		
		CharacterLoadData entityCreationData = new CharacterLoadData( ushort.MaxValue, viewData);
		entityCreationData.nHairItemIndex = iHair;
		
		m_ModelEntity = AsEntityManager.Instance.CreateUserEntity( "PlayerChar", entityCreationData, false);
		
		m_ModelEntity.HandleMessage( new Msg_AnimationIndicate( "Idle"));

		charDummy.transform.position = new Vector3( GetCharacterPositionX(), charDummy.transform.position.y, charDummy.transform.position.z);
		m_ModelEntity.gameObject.transform.localPosition = Vector3.zero;
		m_ModelEntity.gameObject.transform.localRotation = Quaternion.identity;
		m_ModelEntity.gameObject.transform.localScale = charDummy.transform.localScale;
		
		while( true)
		{
			if( eModelLoadingState.Finished == m_ModelEntity.CheckModelLoadingState())
				break;
			
			yield return null;
		}

		m_ModelEntity.transform.parent = charDummy.transform;
		SetLayerHierArchy( m_ModelEntity.transform, LayerMask.NameToLayer( "GUI"));
		
		Initialize();

		while( true == PartsRoot.s_isLoading)
			yield return null;

		genderIcons[0].controlIsEnabled = true;
		genderIcons[1].controlIsEnabled = true;

		AsLoadingIndigator.Instance.HideIndigator();
	}
	
	void ReleaseModeling()
	{
		if( m_ModelEntity != null)
		{
			AsEntityManager.Instance.RemoveAllEntities();
			m_ModelEntity = null;
		}
	}
	
	void SetLayerHierArchy( Transform _trn, int _layer)
	{
		_trn.gameObject.layer = _layer;
		
		for( int i = 0; i < _trn.GetChildCount(); ++i)
		{
			SetLayerHierArchy( _trn.GetChild( i), _layer);
		}
	}
	
	private float GetCharacterPositionX()
	{
		Camera cam = editName.RenderCamera;
		float width = cam.orthographicSize * cam.aspect * 2.0f;

		return -( width * 0.2f) - 500.0f;
	}

	private void _rotateEntity()
	{
		if( null == m_ModelEntity)
			return;

		if( eModelLoadingState.Finished != m_ModelEntity.CheckModelLoadingState())
			return;

		rotAccum += ( Time.deltaTime * rotSpeed);
		m_ModelEntity.transform.localRotation = Quaternion.AngleAxis( rotAccum, Vector3.up);
		footPlate.transform.localRotation = Quaternion.AngleAxis( rotAccum, Vector3.up);

		m_ModelEntity.transform.localPosition = Vector3.zero;
	}

	private void OnMaleBtn()
	{
		if( false == genderIcons[0].controlIsEnabled)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == PartsRoot.s_isLoading)
		{
			genderIcons[0].Value = false;
			genderIcons[1].Value = true;
			return;
		}

		if( (int)eGENDER.eGENDER_MALE == AsCharacterCreateFramework.curGender)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;
		InitDefault();
		DisplayModeling();
	}

	private void OnFemaleBtn()
	{
		if( false == genderIcons[1].controlIsEnabled)
			return;

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == PartsRoot.s_isLoading)
		{
			genderIcons[0].Value = true;
			genderIcons[1].Value = false;
			return;
		}

		if( (int)eGENDER.eGENDER_FEMALE == AsCharacterCreateFramework.curGender)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_FEMALE;
		InitDefault();
		DisplayModeling();
	}

	private void OnHairType_A()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_HAIR_STYLE.STYLE_1 == AsCharacterCreateFramework.curHairStyle)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_1;
		ChangeHairColor();
	}

	private void OnHairType_B()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_HAIR_STYLE.STYLE_2 == AsCharacterCreateFramework.curHairStyle)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_2;
		ChangeHairColor();
	}

	private void OnHairType_C()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_HAIR_STYLE.STYLE_3 == AsCharacterCreateFramework.curHairStyle)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairStyle = (int)CHARACTER_HAIR_STYLE.STYLE_3;
		ChangeHairColor();
	}

	private void OnHairColor_A()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_1 == AsCharacterCreateFramework.curHairColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairColor = (int)CHARACTER_COLOR.COLOR_1;
		ChangeHairColor();
	}

	private void OnHairColor_B()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_2 == AsCharacterCreateFramework.curHairColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairColor = (int)CHARACTER_COLOR.COLOR_2;
		ChangeHairColor();
	}

	private void OnHairColor_C()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_3 == AsCharacterCreateFramework.curHairColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curHairColor = (int)CHARACTER_COLOR.COLOR_3;
		ChangeHairColor();
	}

	private void OnBodyColor_A()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_1 == AsCharacterCreateFramework.curBodyColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curBodyColor = (int)CHARACTER_COLOR.COLOR_1;
		ChangeBodyColor();
	}

	private void OnBodyColor_B()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_2 == AsCharacterCreateFramework.curBodyColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curBodyColor = (int)CHARACTER_COLOR.COLOR_2;
		ChangeBodyColor();
	}

	private void OnBodyColor_C()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_3 == AsCharacterCreateFramework.curBodyColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curBodyColor = (int)CHARACTER_COLOR.COLOR_3;
		ChangeBodyColor();
	}

	private void OnGloveColor_A()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_1 == AsCharacterCreateFramework.curGloveColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curGloveColor = (int)CHARACTER_COLOR.COLOR_1;
		ChangeGloveColor();
	}

	private void OnGloveColor_B()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_2 == AsCharacterCreateFramework.curGloveColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curGloveColor = (int)CHARACTER_COLOR.COLOR_2;
		ChangeGloveColor();
	}

	private void OnGloveColor_C()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_3 == AsCharacterCreateFramework.curGloveColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curGloveColor = (int)CHARACTER_COLOR.COLOR_3;
		ChangeGloveColor();
	}

	private void OnPointColor_A()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_1 == AsCharacterCreateFramework.curPointColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curPointColor = (int)CHARACTER_COLOR.COLOR_1;
		ChangePointColor();
	}

	private void OnPointColor_B()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_2 == AsCharacterCreateFramework.curPointColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curPointColor = (int)CHARACTER_COLOR.COLOR_2;
		ChangePointColor();
	}

	private void OnPointColor_C()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( (int)CHARACTER_COLOR.COLOR_3 == AsCharacterCreateFramework.curPointColor)
			return;

		if( true == AsEntityManager.Instance.ModelLoading)
			return;

		AsCharacterCreateFramework.curPointColor = (int)CHARACTER_COLOR.COLOR_3;
		ChangePointColor();
	}
}
