using UnityEngine;
using System.Collections;
using System.Text;

public class AsMyProperty : MonoBehaviour
{
	public UIProgressBar expProgress = null;
	public SpriteText expDigit = null;
	public UIProgressBar hpProgress = null;
	public SpriteText hpDigit = null;
	public UIProgressBar mpProgress = null;
	public SpriteText mpDigit = null;
	public UIProgressBar conditionProgress = null;
	public SpriteText conditionDigit = null;
	public UIProgressBar eventProgress = null;
	public SpriteText levelText = null;
	public SpriteText nameText = null;
	public SimpleSprite portrait = null;
	public AsPropertyAlert alertManager = null;
	public Animation equipAlertEffect;

	private GameObject levelupEffect = null;
	private Texture2D portraitTex = null;

	private StringBuilder m_sbHP = new StringBuilder();
	private StringBuilder m_sbMP = new StringBuilder();
	private StringBuilder m_sbCondition = new StringBuilder();
	private StringBuilder m_sbLevel = new StringBuilder();
	private StringBuilder m_sbExp = new StringBuilder();

	private static AsMyProperty instance = null;
	public static AsMyProperty Instance
	{
		get
		{
			if( null == instance)
				instance = FindObjectOfType( typeof( AsMyProperty)) as AsMyProperty;

			if( null == instance)
			{
				GameObject obj = new GameObject( "AsMyProperty");
				instance = obj.AddComponent( typeof( AsMyProperty)) as AsMyProperty;
			}

			return instance;
		}
	}

	//dopamin
	public SimpleSprite m_Captain_Image;
	public void SetCaptain( bool bIs)
	{
		m_Captain_Image.gameObject.SetActiveRecursively( bIs);
		m_Captain_Image.gameObject.active = bIs;
	}

	void OnDestroy()
	{
		if( null != portraitTex)
			DestroyImmediate( portraitTex);
	}

	void OnApplicationQuit()
	{
		instance = null;
	}

	void Awake()
	{
		// < ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( expDigit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( hpDigit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( mpDigit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( levelText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( nameText);
		// ilmeda, 20120822 >
		
	}

	// Use this for initialization
	void Start()
	{
		eventProgress.Text = AsTableManager.Instance.GetTbl_String(1648);
		eventProgress.Value = 1.0f;
		SetConditionEventState( false);
		
		if( null != equipAlertEffect)
			equipAlertEffect.gameObject.active = false;
	}

	// Update is called once per frame
	void Update()
	{
		if( null != equipAlertEffect)
		{
			if( true == equipAlertEffect.gameObject.active )
			{
				if( false == equipAlertEffect.isPlaying )
					equipAlertEffect.gameObject.active = false;
			}
		}
	}

	#region -DelegateImage
	public void ClearDelegateImage()
	{
		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/Default");
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}

	public void SetDelegateImage()
	{
		DelegateImageData data = AsDelegateImageManager.Instance.GetSelectDelegateImage();
		if( null == data)
		{
			data = AsDelegateImageManager.Instance.GetAssignedDelegateImage();
			if( null == data)
				return;
		}

		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( data.iconName);
		Texture2D tex = ResourceLoad.Loadtexture( sb.ToString()) as Texture2D;
		portrait.SetTexture( tex);
		portrait.SetUVsFromPixelCoords( new Rect( 0.0f, 0.0f, tex.width, tex.height));
	}
	#endregion

	public void Init()
	{
		StopCoroutine( "UpdateStats");
		StartCoroutine( UpdateStats());
#if false
		if( true == IsInvoking( "DecreaseConditionValue"))
			CancelInvoke( "DecreaseConditionValue");
		InvokeRepeating( "DecreaseConditionValue", 0.0f, 10.0f);
#endif

		#region -DelegateImage
		if( 0 >= AsDelegateImageManager.Instance.AssignedImageID)
			ClearDelegateImage();
		else
			SetDelegateImage();
		#endregion

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
		{
			Debug.LogWarning( "AsMyProperty::Init: Init() played");
			return;
		}

		string name = userEntity.GetProperty<string>( eComponentProperty.NAME);
		nameText.Text = name;

		UpdateProperty();
		SetCaptain( false);
		SetConditionEventState( ( null == AsEventManager.Instance.Get( eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION)) ? false : true);
	}

	public void SetHP( float hp)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			float maxHP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fHPMax;
			float curHP = userEntity.GetProperty<float>( eComponentProperty.HP_CUR);
			if( curHP > maxHP)
				curHP = maxHP;

			m_sbHP.Remove( 0, m_sbHP.Length);
			m_sbHP.AppendFormat( "{0:D}/{1:D}", (int)curHP, (int)maxHP);
			hpDigit.Text = m_sbHP.ToString();

//			hpDigit.Text = string.Format( "{0:D}/{1:D}", (int)curHP, (int)maxHP);
		}

		if( hp  == hpProgress.Value)
			return;

		if( ( 0.2f >= hp) && ( hpProgress.Value > hp))
			AlertHP();

		hpProgress.Value = hp;
	}

	public void AlertHP()
	{
		AsSpriteBlinker effect = hpProgress.GetComponentInChildren<AsSpriteBlinker>();
		if( null != effect)
			effect.Play();
	}

	public void SetMP( float mp)
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null != userEntity)
		{
			float maxMP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fMPMax;
			float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);

			m_sbMP.Remove( 0, m_sbMP.Length);
			m_sbMP.AppendFormat( "{0:D}/{1:D}", (int)curMP, (int)maxMP);
			mpDigit.Text = m_sbMP.ToString();

//			mpDigit.Text = string.Format( "{0:D}/{1:D}", (int)curMP, (int)maxMP);
		}

		if( mp == mpProgress.Value)
			return;

		if( ( 0.2f >= mp) && ( mpProgress.Value > mp))
			AlertMP();

		mpProgress.Value = mp;
	}

	public void SetConditionValue()
	{
		float maxCondition = AsTableManager.Instance.GetTbl_GlobalWeight_Record(11).Value;
		float curCondition = (float)AsUserInfo.Instance.CurConditionValue;

		float condition = curCondition / maxCondition;

		m_sbCondition.Remove( 0, m_sbCondition.Length);
		m_sbCondition.AppendFormat( "{0:D}%", (int)( condition * 100.0f));
		conditionDigit.Text = m_sbCondition.ToString();

//		conditionDigit.Text = string.Format( "{0:D}%", (int)( condition * 100.0f));

		if( condition == conditionProgress.Value)
			return;

		conditionProgress.Value = condition;
	}

	public void SetConditionEventState( bool flag)
	{
		eventProgress.gameObject.SetActiveRecursively( flag);
		conditionProgress.gameObject.SetActiveRecursively( !flag);
	}

	public void AlertMP()
	{
		AsSpriteBlinker[] effects = mpProgress.GetComponentsInChildren<AsSpriteBlinker>();
		foreach( AsSpriteBlinker effect in effects)
			effect.Play();

		alertManager.ManaAlert();
	}

	public void AlertSkillInTown()
	{
		alertManager.AlertSkillInTown();
	}

	public void AlertUseless()
	{
		alertManager.AlertUseless();
	}

	public void AlertTarget()
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( true == playerFsm.CheckMap_Village())
			alertManager.AlertSkillInTown();
		else
			alertManager.TargetAlert();
	}

	public void AlertCoolTime( COMMAND_SKILL_TYPE type)
	{
		alertManager.CoolTimeAlert( type);
	}

	public void AlertInvalidTarget()
	{
		alertManager.AlertInvalidTarget();
	}

	public void AlertNoWeapon()
	{
		alertManager.AlertNoWeapon();
	}

	public void AlertState()
	{
		alertManager.AlertState();
	}

	public void AlertEmoticon_Seat()
	{
		alertManager.AlertEmotiocon_Seat();
	}
	
	public void AlertNotInPvp()
	{
		alertManager.AlertNotInPvp();
	}
	
	public void AlertRebirthNotInPvp()
	{
		alertManager.AlertRebirthNotInPvp();
	}
	
	public void AlertNotInRaid()
	{
		alertManager.AlertNotInRaid();
	}
	
	public void AlertNotInField()
	{
		alertManager.AlertNotInField();
	}

	public void AlertNotInIndun()
	{
		alertManager.AlertNotInIndun();
	}
	
	public void AlertNotInSummon()
	{
		alertManager.AlertNotInSummon();
	}
	
	public void AlertTargetChanged()//$yde
	{
		alertManager.AlertTargetChanged();
	}

	public void AlertNoExpandQuickSlot()
	{
		alertManager.AlertNoExpandQuickSlot();
	}

	public void SetExp( float exp)
	{
		if( ( 0 != exp) && ( exp == expProgress.Value))
			return;

		expProgress.Value = exp;

		m_sbExp.Remove( 0, m_sbExp.Length);
		m_sbExp.AppendFormat(  "EXP {0:F2}%", exp * 100 );
		expDigit.Text = m_sbExp.ToString();

//		expDigit.Text = string.Format( "EXP {0:F2}%", exp * 100);
	}

	public void SetExpMax()
	{
		expProgress.Value = 1.0f;

		m_sbExp.Remove( 0, m_sbExp.Length);
		m_sbExp.AppendFormat(  "RGBA(1.0,0.7,0.1,1.0) EXP MAX" );
		expDigit.Text = m_sbExp.ToString();

//		expDigit.Text = "RGBA(1.0,0.7,0.1,1.0) EXP MAX";
	}

	public void SetLevel( int level)
	{
		m_sbLevel.Remove( 0, m_sbLevel.Length);
		m_sbLevel.AppendFormat(  "Lv.{0}", level );
		levelText.Text = m_sbLevel.ToString();

//		levelText.Text = string.Format( "Lv.{0}", level);
	}

	public void SetName( string name)
	{
		nameText.Text = name;
	}

	public void LevelUpDisplay()
	{
		levelupEffect = Instantiate( Resources.Load( "UI/UI_effect/GamePlay/Prefab/LevelUp")) as GameObject;

		UIPanel panel = levelupEffect.GetComponent<UIPanel>();
		panel.BringIn();

		StartCoroutine( DeactivateLevelupPanel());
	}

	private IEnumerator DeactivateLevelupPanel()
	{
		yield return new WaitForSeconds( 4.0f);

		GameObject.DestroyImmediate( levelupEffect);
	}

	IEnumerator UpdateStats()
	{
		while( true)
		{
			yield return new WaitForSeconds( 0.1f);

			if( ( null != ItemMgr.HadItemManagement) && ( null != ItemMgr.HadItemManagement.QuickSlot))
				ItemMgr.HadItemManagement.QuickSlot.ApplyAsSlot();

			UpdateProperty();
		}
	}

	public void OnEnable()
	{
		Init();
	}

	private void UpdateProperty()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;

		//	HP
		float maxHP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fHPMax;
		float curHP = userEntity.GetProperty<float>( eComponentProperty.HP_CUR);
		SetHP( curHP / maxHP);

		// MP
		float maxMP = AsUserInfo.Instance.SavedCharStat.sFinalStatus.fMPMax;
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		SetMP( curMP / maxMP);

		// Condition
		SetConditionValue();

		// Level
		int level = userEntity.GetProperty<int>( eComponentProperty.LEVEL);
		SetLevel( level);
		AsUserInfo.Instance.SavedCharStat.level_ = level;

		// EXP
		int maxLevel = AsTableManager.Instance.GetTbl_GlobalWeight_Record(69) != null ? (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(69).Value : AsGameDefine.MAX_LEVEL;

		if( maxLevel > level)
		{
			eCLASS cls = userEntity.GetProperty<eCLASS>( eComponentProperty.CLASS);
			int curExp = userEntity.GetProperty<int>( eComponentProperty.TOTAL_EXP);
			Tbl_UserLevel_Record curRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level);
			Tbl_UserLevel_Record nextRecord = AsTableManager.Instance.GetTbl_Level_Record( cls, level + 1);
			SetExp( (float)( curExp - curRecord.TotalEXP) / (float)( nextRecord.TotalEXP - curRecord.TotalEXP));
			AsUserInfo.Instance.SavedCharStat.totExp_ = curExp;
		}
		else
		{
			SetExpMax();
		}
	}

	private float backgroundEnterTime = 0.0f;
	void OnApplicationPause( bool pause)
	{
		if( true == pause)
		{
			backgroundEnterTime = Time.time;
#if false
			if( true == IsInvoking( "DecreaseConditionValue"))
				CancelInvoke( "DecreaseConditionValue");
#endif
		}
#if false
		else
		{
			if( ( eMAP_TYPE.Field == AsUtil.GetMapType() || eMAP_TYPE.Dungeon == AsUtil.GetMapType() ) && ( 0 < AsUserInfo.Instance.CurConditionValue))
			{
				int conditionConsume = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(9).Value;
				float backgroundPeriod = Time.time - backgroundEnterTime;
				int consumeAmount = (int)( backgroundPeriod * 0.1f) * conditionConsume;

				AsUserInfo.Instance.CurConditionValue -= consumeAmount;

				if( true == IsInvoking( "DecreaseConditionValue"))
					CancelInvoke( "DecreaseConditionValue");
				InvokeRepeating( "DecreaseConditionValue", 0.0f, 10.0f);
			}
		}
#endif
	}
	
#if false
	void DecreaseConditionValue()
	{
		if( null != AsEventManager.Instance.Get( eEVENT_TYPE.eEVENT_TYPE_TIME_CONDITION))
			return;

		int conditionConsume = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(9).Value;

		if( ( eMAP_TYPE.Field == AsUtil.GetMapType() || eMAP_TYPE.Dungeon == AsUtil.GetMapType() ) && ( 0 < AsUserInfo.Instance.CurConditionValue))
			AsUserInfo.Instance.CurConditionValue -= conditionConsume;
	}
#endif

	void OnRecoverCondition()
	{
#if false
		AsHudDlgMgr.Instance.CloseRecommendInfoDlg();	// #17479
		if( AsGameMain.useCashShop == true)
			AsHudDlgMgr.Instance.OpenCashStore( 0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>( eComponentProperty.CLASS), eCashStoreMenuMode.USEITEM, 2, 4);
#endif
		AsChatFullPanel.Instance.Close();

		GameGuideData data = new GameGuideData();
		data.imagePath = "Guide_012";
		AsGameGuideManager.Instance.DisplayGuide( data);
	}
}
