
using UnityEngine;
using System.Collections;
using System.Text;

public class AsHUD_TargetInfo : MonoBehaviour
{
	public SpriteText nameText = null;
	public UIProgressBar hpProgress = null;
	public UIProgressBar collectProgress = null;
	public UIProgressBar chargeProgress = null;
	public SpriteText hpDigit = null;
	public SpriteText level = null;
	public SpriteText classText = null;
	public SimpleSprite npcMark = null;
	public SimpleSprite objectMark = null;
	public UIButton userInfoButton = null;
	public OtherBuffMgr mobBuffMgr;
	public AsMonsterAttrManager attrMgr = null;
	public AsTargetGradeManager gradeMgr = null;
	public AsCollectionTypeManager collectionTypeMgr = null;
	public AsMonsterAttackStyleManager attackStyleMgr = null;
	
	[SerializeField] UIButton targetMarkBtn = null;
	
	[HideInInspector]
	public string targetName = "Giblin";
	[HideInInspector]
	public float hp_max = 0;
	[HideInInspector]
	public float hp_cur = 0;
	[HideInInspector]
	public float casting = 0.0f;
	[HideInInspector]
	public float castTime = 0.0f;
	private float chargeStartTime = 0.0f;
	
	private bool isEnable = false;
	private bool chargeBarEnabled = false;
	
	private AsBaseEntity m_CurBaseEntity;
	private AsBaseEntity m_CollectBaseEntity = null;

	// < ilmeda 20120425
	private int m_nTargetSelectEff = 0;
	private string m_strTargetSelectEffPath_Red = "FX/Effect/Decal/Character_select_Mob_Dummy";
	private string m_strTargetSelectEffPath_White = "FX/Effect/Decal/Character_select_Pc_Dummy";
	// ilmeda 20120425 >
	
	public void OnEnable()
	{
		if( null == AsEntityManager.Instance)
			return;
		
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null == playerFsm)
		{
			gameObject.SetActiveRecursively( false);
			return;
		}
		
		#region -Authorith
		nameText.Color = Color.white;
		#endregion
		
		if( null != playerFsm.Target)
			SetInfo( playerFsm.Target);
		else
			gameObject.SetActiveRecursively( false);
	}
	
	public void OnDisable()
	{
		RemoveTargetSelectEffect();
		mobBuffMgr.ClearBuff();
		
		#region -Authorith
		nameText.Color = Color.white;
		#endregion
	}
	
	public bool Enable
	{
		set
		{ 
			isEnable = value;
			
			SetEnableChargeBar( false);
			
			gameObject.SetActiveRecursively( value);
			
			if( true == value)
				OnEnable();
			else
				OnDisable();
		}
		
		get { return isEnable; }
	}
	
	public AsBaseEntity getCurTargetEntity
	{
		get	{ return m_CurBaseEntity; }
	}
	
	
	public string Name
	{
		set
		{
			Color color = _GetStringColor( m_CurBaseEntity);
			nameText.Text = color + value;
			targetName = color + value;
		}
	}
	
	public int Level
	{
		set
		{
			if( null == m_CurBaseEntity)
				return;
			
			Color color = _GetStringColor( m_CurBaseEntity);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( color.ToString() + "Lv.{0:D}", value);
			level.Text = sb.ToString();
		}
	}
	
	public float HpMax
	{
		set
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "{0:D}/{1:D}", (int)hp_cur, (int)hp_max);
			
			hp_max = value;
			hpProgress.Value = hp_cur / hp_max;
			hpDigit.Text = sb.ToString();
		}
	}
	
	public float HpCur
	{
		set
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "{0:D}/{1:D}", (int)hp_cur, (int)hp_max);
			
			hp_cur = value;
			hpProgress.Value = hp_cur / hp_max;
			hpDigit.Text = sb.ToString();
		}
	}
	
	public float Casting
	{
		set
		{
			casting = value;
			chargeProgress.Value = value;
		}
		
		get { return casting; }
	}
	
	public void SetAuthority( bool authority)
	{
		nameText.Color = ( true == authority) ? Color.white : Color.gray;
	}
	
	// Use this for initialization
	void Start()
	{
		Casting = 0.0f;
		SetEnableChargeBar( false);
		
		// < ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( nameText);
		InitProgressHP();
		AsLanguageManager.Instance.SetFontFromSystemLanguage( hpDigit);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( level);
		// ilmeda, 20120822 >
		
		#region - target mark -
		targetMarkBtn.SetInputDelegate( OnTargetMarkBtnClicked);
		#endregion
	}
	
	// Update is called once per frame
	void Update()
	{
		if( false == chargeBarEnabled)
			return;
		
		Casting = ( Time.time - chargeStartTime) / castTime;
		if( 1.0f < Casting)
		{
			Casting = 0.0f;
			SetEnableChargeBar( false);
		}
	}
	
	public void StartCast( float castTime)
	{
		SetEnableChargeBar( true);
		
		chargeStartTime = Time.time;
		this.castTime = castTime;
		Casting = 0.0f;
	}
	
	private void SetEnableChargeBar( bool flag)
	{
		chargeProgress.gameObject.SetActiveRecursively( flag);
		chargeBarEnabled = flag;
//		chargeProgress.renderer.enabled = flag;
//		
//		Renderer[] renderers = chargeProgress.GetComponentsInChildren<Renderer>();
//		foreach( Renderer ren in renderers)
//			ren.enabled = flag;
	}
	
	public void InitProgressHP()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0:D}/{1:D}", (int)hp_cur, (int)hp_max);
		
		hpProgress.Value = hp_cur / hp_max;
		hpDigit.Text = sb.ToString();
	}
	
	public void InitCollectionProgress()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( "{0:D}/{1:D}", (int)hp_cur, (int)hp_max);
		
		collectProgress.Value = hp_cur / hp_max;
		hpDigit.Text = sb.ToString();
	}

	// < ilmeda 20120425
	public void ShowTargetSelectEffect(AsBaseEntity target)
	{
		StartCoroutine( _ShowTargetSelectEffect(target));
	}
	
	private IEnumerator _ShowTargetSelectEffect(AsBaseEntity target)
	{
		if( null == target.gameObject)
			yield return null;
		
		GameObject goTarget = target.gameObject;
		
		if( null != AsEffectManager.Instance.GetEffectEntity( m_nTargetSelectEff))
			RemoveTargetSelectEffect();
		
		string strTargetSelectEffPath = string.Empty;
		if( eFsmType.MONSTER == target.FsmType)
			strTargetSelectEffPath = m_strTargetSelectEffPath_Red;
		else
			strTargetSelectEffPath = m_strTargetSelectEffPath_White;

		m_nTargetSelectEff = AsEffectManager.Instance.PlayEffect( strTargetSelectEffPath, goTarget.transform, true, 0.0f);
		
		float fScale = 1.0f;
		if( eFsmType.OTHER_USER != target.FsmType)
		{
			int nNpcID = target.GetProperty<int>( eComponentProperty.NPC_ID);
			Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record( nNpcID);

			if( 1 == npcRec.PointScale) fScale = 1.0f;
			else if( 2 == npcRec.PointScale) fScale = 1.2f;
			else if( 3 == npcRec.PointScale) fScale = 1.5f;
			else if( 4 == npcRec.PointScale) fScale = 2.0f;
			else if( 5 == npcRec.PointScale) fScale = 2.5f;
		}
		
		fScale *= target.GetCharacterSize();
		
		AsEffectManager.Instance.SetSize( m_nTargetSelectEff, fScale);
	}
	
	public void RemoveTargetSelectEffect()
	{
		if( null == AsEffectManager.Instance)
			return;
		
		AsEffectManager.Instance.RemoveEffectEntity( m_nTargetSelectEff);
	}
	// ilmeda 20120425 >
	
	public bool UpdateTargetSimpleInfo( AsBaseEntity target)
	{
		float maxHP = target.GetProperty<float>( eComponentProperty.HP_MAX);
		float curHP = target.GetProperty<float>( eComponentProperty.HP_CUR);
		hp_max = maxHP;
		hp_cur = curHP;
		InitProgressHP();
		
		if( 1f > curHP)
		{
			gameObject.SetActiveRecursively( false);
			mobBuffMgr.EmptyOtherUserFsm();		
			
			AsEntityManager.Instance.GetPlayerCharFsm().Target = null;
		}
		
		return ( 0.0f >= curHP) ? false : true;
	}
	
	public bool UpdateCollectionSimpleInfo( AsBaseEntity target)
	{
		float maxHP = target.GetProperty<float>( eComponentProperty.HP_MAX);
		float curHP = target.GetProperty<float>( eComponentProperty.HP_CUR);
		hp_max = maxHP;
		hp_cur = curHP;
		InitCollectionProgress();
		
		if( 0.0f >= curHP)
		{
			gameObject.SetActiveRecursively( false);
		}
		
		return ( 0.0f >= curHP) ? false : true;
	}
	
	public void EmptyCollectEntity()	
	{
		m_CollectBaseEntity = null;
	}
	
	public AsBaseEntity getCollectEntity	{ get { return m_CollectBaseEntity; } }
	
	private void SetCollectionInfo(  AsBaseEntity target )
	{
		attackStyleMgr.gameObject.SetActive( false);
		attrMgr.gameObject.SetActiveRecursively( false);
		gradeMgr.gameObject.SetActiveRecursively( false);
		npcMark.gameObject.SetActiveRecursively( false);
		objectMark.gameObject.SetActiveRecursively( false);
		userInfoButton.gameObject.SetActiveRecursively( false);
		collectProgress.gameObject.SetActiveRecursively( true);
		hpProgress.gameObject.SetActiveRecursively( false);
		chargeProgress.gameObject.SetActiveRecursively( false);
		ShowTargetSelectEffect( target);
		collectionTypeMgr.Display( target);
		
		#region - target mark -
		targetMarkBtn.gameObject.SetActive( false);
		#endregion
		
		eITEM_PRODUCT_TECHNIQUE_TYPE _type = (eITEM_PRODUCT_TECHNIQUE_TYPE)target.GetProperty<int>( eComponentProperty.COLLECTOR_TECHNIC_TYPE);
		switch( _type)
		{
		case eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL:
			classText.Text = AsTableManager.Instance.GetTbl_String(1973);
			break;
		case eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT:
			classText.Text = AsTableManager.Instance.GetTbl_String(1972);
			break;
		case eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS:
			classText.Text = AsTableManager.Instance.GetTbl_String(1971);
			break;
        case eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST:
            classText.Text = AsTableManager.Instance.GetTbl_String(819);
            break;
		}
		
		if( m_CurBaseEntity == target)
			return;
			
		uint iCollectorIdx = target.GetProperty<uint>( eComponentProperty.COLLECTOR_IDX);
		if( 0 != iCollectorIdx )
			return;
		
		m_CollectBaseEntity = target;
		m_CurBaseEntity = target;

        if (_type == eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST)
            Level = 1;
        else
		    Level = target.GetProperty<int>( eComponentProperty.LEVEL);

		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.red.ToString());
		Name = sb.ToString();
		
		float maxHP = target.GetProperty<float>( eComponentProperty.HP_MAX);
		float curHP = target.GetProperty<float>( eComponentProperty.HP_CUR);
		hp_max = maxHP;
		hp_cur = curHP;
		InitCollectionProgress();
	}
	
	private void SetObjectInfo( AsBaseEntity target)
	{
		attackStyleMgr.gameObject.SetActive( false);
		attrMgr.gameObject.SetActiveRecursively( false);
		gradeMgr.gameObject.SetActiveRecursively( false);
		collectionTypeMgr.gameObject.SetActiveRecursively( false);
		npcMark.gameObject.SetActiveRecursively( false);
		objectMark.gameObject.SetActiveRecursively( true);
		hpProgress.gameObject.SetActiveRecursively( true);
		chargeProgress.gameObject.SetActiveRecursively( false);
		collectProgress.gameObject.SetActiveRecursively( false);
		level.gameObject.SetActiveRecursively( true);
		nameText.gameObject.SetActiveRecursively( true);
		classText.Text = AsTableManager.Instance.GetTbl_String(1975);
		userInfoButton.gameObject.SetActiveRecursively( false);
		ShowTargetSelectEffect( target);
		
		#region - target mark -
		targetMarkBtn.gameObject.SetActive( false);
		#endregion

		if( m_CurBaseEntity == target)
			return;	
		
		m_CurBaseEntity = target;

		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.white.ToString());
		Name = sb.ToString();
		int nNpcID = target.GetProperty<int>( eComponentProperty.NPC_ID);
		Tbl_NormalNpc_Record normalNpcRec = AsTableManager.Instance.GetTbl_NormalNpc_Record( nNpcID);
		if( null == normalNpcRec )
			return;
		
		Level = normalNpcRec.NpcLV;
		hp_cur = hp_max = normalNpcRec.NpcHP;
		InitProgressHP();
	}
	
	private void SetNpcInfo( AsBaseEntity target)
	{		
		if( null == target )
			return;
		
		AsNpcEntity _npcEntity = target as AsNpcEntity;
		if( null != _npcEntity )
		{			
			if( false == _npcEntity.isNoWarpIndex )
			{
				gameObject.SetActiveRecursively(false);
				return;
			}						
		}
		npcMark.gameObject.SetActiveRecursively( true);
		objectMark.gameObject.SetActiveRecursively( false);
		hpProgress.gameObject.SetActiveRecursively( true);
		attackStyleMgr.gameObject.SetActive( false);
		attrMgr.gameObject.SetActiveRecursively( false);
		gradeMgr.gameObject.SetActiveRecursively( false);
		collectionTypeMgr.gameObject.SetActiveRecursively( false);
		collectProgress.gameObject.SetActiveRecursively( false);
		userInfoButton.gameObject.SetActiveRecursively( false);
		chargeProgress.gameObject.SetActiveRecursively( false);
		ShowTargetSelectEffect( target);
		
		#region - target mark -
		targetMarkBtn.gameObject.SetActive( false);
		#endregion

		if( m_CurBaseEntity == target)
			return;		
		
		m_CurBaseEntity = target;
		
		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.white.ToString());
		Name = sb.ToString();
		classText.Text = AsTableManager.Instance.GetTbl_String(1974);
		int nNpcID = target.GetProperty<int>( eComponentProperty.NPC_ID);
		Tbl_NormalNpc_Record normalNpcRec = AsTableManager.Instance.GetTbl_NormalNpc_Record( nNpcID);
		if( null != normalNpcRec )
		{
			Level = normalNpcRec.NpcLV;
			hp_cur = hp_max = normalNpcRec.NpcHP;
		}
		InitProgressHP();
	}
	
	private void SetMobInfo( AsBaseEntity target)
	{
		eMonster_Grade eGrade = target.GetProperty<eMonster_Grade>( eComponentProperty.GRADE);
		if( eMonster_Grade.DObject == eGrade || eMonster_Grade.Treasure == eGrade || eMonster_Grade.QObject == eGrade)
		{
			SetMobObjectInfo( target);
			return;
		}
		
		npcMark.gameObject.SetActiveRecursively( false);
		objectMark.gameObject.SetActiveRecursively( false);
		hpProgress.gameObject.SetActiveRecursively( true);
		collectProgress.gameObject.SetActiveRecursively( false);
		userInfoButton.gameObject.SetActiveRecursively( false);
		chargeProgress.gameObject.SetActiveRecursively( false);
		
		m_CurBaseEntity = target;
		
		float maxHP = target.GetProperty<float>( eComponentProperty.HP_MAX);
		float curHP = target.GetProperty<float>( eComponentProperty.HP_CUR);
		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.red.ToString());
		Name = sb.ToString();
		Level = target.GetProperty<int>( eComponentProperty.LEVEL);
		eGrade = target.GetProperty<eMonster_Grade>( eComponentProperty.GRADE);
		switch( eGrade)
		{
		case eMonster_Grade.Normal:
		case eMonster_Grade.DObject:
		case eMonster_Grade.QObject:
			classText.Text = AsTableManager.Instance.GetTbl_String(1966);
			break;
		case eMonster_Grade.Elite:
			classText.Text = AsTableManager.Instance.GetTbl_String(1967);
			break;
		case eMonster_Grade.Champion:
			classText.Text = AsTableManager.Instance.GetTbl_String(1968);
			break;
		case eMonster_Grade.Boss:
			classText.Text = AsTableManager.Instance.GetTbl_String(1970);
			break;
		case eMonster_Grade.Named:
			classText.Text = AsTableManager.Instance.GetTbl_String(1969);
			break;
		}
		
		#region - target mark -
		if( AsPartyManager.Instance.IsCaptain == true &&
			(eGrade == eMonster_Grade.Normal ||
			eGrade == eMonster_Grade.Elite ||
			eGrade == eMonster_Grade.Champion ||
			eGrade == eMonster_Grade.Boss ||
			eGrade == eMonster_Grade.Named ||
			eGrade == eMonster_Grade.DObject))
			targetMarkBtn.gameObject.SetActive( true);
		else
			targetMarkBtn.gameObject.SetActive( false);
		#endregion

		hp_max = maxHP;
		hp_cur = curHP;
		InitProgressHP();
		ShowTargetSelectEffect( target);
		
		attackStyleMgr.Display( target);
		attrMgr.Display( target);
		gradeMgr.Display( target);
		collectionTypeMgr.gameObject.SetActiveRecursively( false);
	}
	
	private void SetMobObjectInfo( AsBaseEntity target)
	{
		attackStyleMgr.gameObject.SetActive( false);
		attrMgr.gameObject.SetActiveRecursively( false);
		gradeMgr.gameObject.SetActiveRecursively( false);
		collectionTypeMgr.gameObject.SetActiveRecursively( false);
		npcMark.gameObject.SetActiveRecursively( false);
		objectMark.gameObject.SetActiveRecursively( true);
		hpProgress.gameObject.SetActiveRecursively( true);
		chargeProgress.gameObject.SetActiveRecursively( false);
		collectProgress.gameObject.SetActiveRecursively( false);
		level.gameObject.SetActiveRecursively( true);
		nameText.gameObject.SetActiveRecursively( true);
		classText.Text = AsTableManager.Instance.GetTbl_String(1975);
		userInfoButton.gameObject.SetActiveRecursively( false);
		ShowTargetSelectEffect( target);
		
		#region - target mark -
		targetMarkBtn.gameObject.SetActive( true);
		#endregion

		if( m_CurBaseEntity == target)
			return;	
		
		m_CurBaseEntity = target;

		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.white.ToString());
		Name = sb.ToString();
		Level = target.GetProperty<int>( eComponentProperty.LEVEL);
		hp_max = target.GetProperty<float>( eComponentProperty.HP_MAX);
		hp_cur = target.GetProperty<float>( eComponentProperty.HP_CUR);
		InitProgressHP();
	}

	private void SetUserInfo( AsBaseEntity target)
	{
		npcMark.gameObject.SetActiveRecursively( false);
		objectMark.gameObject.SetActiveRecursively( false);
		hpProgress.gameObject.SetActiveRecursively( true);
		collectProgress.gameObject.SetActiveRecursively( false);
		userInfoButton.gameObject.SetActiveRecursively( true);
		ShowTargetSelectEffect( target);
		attackStyleMgr.gameObject.SetActive( false);
		attrMgr.gameObject.SetActiveRecursively( false);
		gradeMgr.gameObject.SetActiveRecursively( false);
		collectionTypeMgr.gameObject.SetActiveRecursively( false);
		chargeProgress.gameObject.SetActiveRecursively( false);
		
		#region - target mark -
		if( AsPartyManager.Instance.IsCaptain == true &&
			TargetDecider.CheckOtherUserIsEnemy( target) == true)
			targetMarkBtn.gameObject.SetActive( true);
		else
			targetMarkBtn.gameObject.SetActive( false);
		#endregion

#if false	// dawnsmell
		if( m_CurBaseEntity == target)
			return;
#endif
		
		m_CurBaseEntity = target;
		
		float maxHP = target.GetProperty<float>( eComponentProperty.HP_MAX);
		float curHP = target.GetProperty<float>( eComponentProperty.HP_CUR);
		
		StringBuilder sb = new StringBuilder( target.GetProperty<string>( eComponentProperty.NAME));
		sb.Insert( 0, Color.white.ToString());
		Name = sb.ToString();
		Level = target.GetProperty<int>( eComponentProperty.LEVEL);
		classText.Text = AsTableManager.Instance.GetTbl_String(1965);
		hp_max = maxHP;
		hp_cur = curHP;
		InitProgressHP();
	}
	
	public void SetInfo( AsBaseEntity target)
	{		
		switch( target.FsmType)
		{
		case eFsmType.NPC:
			SetNpcInfo( target);
			break;
		case eFsmType.MONSTER:
			SetMobInfo( target);
			break;
		case eFsmType.OTHER_USER:
			SetUserInfo( target);
			break;
		case eFsmType.OBJECT:
			SetObjectInfo( target);
			break;
		case eFsmType.COLLECTION:
			SetCollectionInfo( target);
			break;
		}
	}
	
	private Color _GetStringColor(AsBaseEntity baseEntity)
	{
		if( null == baseEntity)
			return Color.white;

		Color color = Color.white;
		
		if( eEntityType.USER == baseEntity.EntityType)
		{
			// user
			color = AsHUDController.Instance.panelManager.m_NameColor_User;
		}
		else if( eEntityType.NPC == baseEntity.EntityType && eFsmType.NPC == baseEntity.FsmType)
		{
			// npc
			color = AsHUDController.Instance.panelManager.m_NameColor_Npc;
		}
		else if( eEntityType.NPC == baseEntity.EntityType && (eFsmType.MONSTER == baseEntity.FsmType ||eFsmType.COLLECTION == baseEntity.FsmType))
		{
			// monster
			if( eFsmType.COLLECTION == baseEntity.FsmType )
				color = AsHUDController.Instance.panelManager.m_NameColor_Default;
			else
			{
				eMonster_AttackType eAttackType = baseEntity.GetProperty<eMonster_AttackType>( eComponentProperty.MONSTER_ATTACK_TYPE);
				if( eMonster_AttackType.Fool == eAttackType)
					color = AsHUDController.Instance.panelManager.m_NameColor_Monster_Fool;
				else if( eMonster_AttackType.Peaceful == eAttackType)
					color = AsHUDController.Instance.panelManager.m_NameColor_Monster_Peaceful;
				else if( eMonster_AttackType.Offensive == eAttackType)
					color = AsHUDController.Instance.panelManager.m_NameColor_Monster_Offensive;
			}
		}
		else
		{
			color = AsHUDController.Instance.panelManager.m_NameColor_Default;
		}
		
		return color;
	}

	private void OnTargetCancel()
	{
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		Debug.Assert( null != playerFsm);
		playerFsm.Target = null;
		if( AsUserInfo.Instance.SavedCharStat.hpCur_ > 0)
			playerFsm.SetPlayerFsmState( AsPlayerFsm.ePlayerFsmStateType.BATTLE_IDLE);
		AsHUDController.Instance.SetTargetInfo( null);
	}
	
	#region - target mark -
	public void ActivateTargetMarkBtn( bool _active)
	{
		if( isEnable == false)
			return;
		
		if( AsPartyManager.Instance.IsCaptain == true && _active == true && m_CurBaseEntity != null)
		{
			if( m_CurBaseEntity.FsmType == eFsmType.MONSTER)
			{
				eMonster_Grade eGrade = m_CurBaseEntity.GetProperty<eMonster_Grade>( eComponentProperty.GRADE);
				if( eGrade == eMonster_Grade.Normal ||
					eGrade == eMonster_Grade.Elite ||
					eGrade == eMonster_Grade.Champion ||
					eGrade == eMonster_Grade.Boss ||
					eGrade == eMonster_Grade.Named)
					targetMarkBtn.gameObject.SetActive( true);
			}
			else if( m_CurBaseEntity.FsmType == eFsmType.OTHER_USER)
			{
				if( AsPartyManager.Instance.IsCaptain == true &&
					TargetDecider.CheckOtherUserIsEnemy( m_CurBaseEntity) == true)
					targetMarkBtn.gameObject.SetActive( true);
			}
		}
		else
		{
			targetMarkBtn.gameObject.SetActive( _active);
		}
	}
	
	private void OnTargetMarkBtnClicked( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartyManager.Instance.TargetMarkBtnClicked();
		}
	}
	#endregion
}
