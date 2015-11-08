using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AsPanelManager : MonoBehaviour
{
	public AsPanel_Damage DamagePanel = null;
	public AsPanel_Name   NamePanel   = null;
	public AsPanel_Quest  QuestPanel  = null;
	public AsPanel_UseItemToTarget UseItemToTargetPanel = null;
	
	public Color m_NameColor_Default = Color.white;
	public Color m_NameColor_User = Color.white;
	public Color m_NameColor_Npc = Color.green;
	public Color m_NameColor_Monster_Fool = Color.green;
	public Color m_NameColor_Monster_Peaceful = Color.yellow;
	public Color m_NameColor_Monster_Offensive = Color.red;
	public Color m_NameColor_Party = Color.cyan;
	public Color m_NameColor_Guild = Color.yellow;
	public float m_fSpeed_Damage = 1.0f;
	public float m_fSpeed_Damage_Miss = 1.0f;
	public float m_fSpeed_Damage_Dodge = 1.0f;
	public float m_fSpeed_Damage_Critical = 1.0f;
	public float m_fSpeed_Damage_HP = 1.0f;
	public float m_fSpeed_Damage_MP = 1.0f;
	public float m_fSpeed_Damage_DotHeal = 1.0f;
	public float m_fSpeed_Damage_Gold = 1.0f;
	public float m_fSpeed_Damage_Exp = 1.0f;
	public float m_fSpeed_Damage_Exp2 = 1.0f;
	public float m_fSpeed_Damage_Buff = 1.0f;
	public float m_fScale_Damage = 1.2f;
	public float m_fScale_Damage_Miss = 1.2f;
	public float m_fScale_Damage_Dodge = 1.2f;
	public float m_fScale_Damage_Critical = 1.2f;
	public float m_fScale_Damage_HP = 1.2f;
	public float m_fScale_Damage_MP = 1.2f;
	public float m_fScale_Damage_DotHeal = 1.2f;
	public float m_fScale_Damage_Gold = 0.8f;
	public float m_fScale_Damage_Exp = 0.8f;
	public float m_fScale_Damage_Exp2 = 0.8f;
	public float m_fScale_Damage_Buff = 1.2f;
	
	private float m_fSpeedMin = 0.1f;
	private float m_fSpeedMax = 10.0f;
	private float m_fScaleMin = 0.1f;
	private float m_fScaleMax = 10.0f;
	
	// CustomFont KeyCode
	// 0~9(48~57) : red 0~9
	// A~J(65~74) : green 0~9
	// K~T(75~84) : blue 0~9
	// a~j(97~106) : yellow 0~9
	// k~t(107~116) : white 0~9
	// U~Z(85~90), u~x(117~120) : red_user 0~9
	// !(33) : critical
	// #(35) : Miss
	// %(37) : Dodge
	// /(47) : Resist
	// $(36) : G
	// &(38) : EXP(white)
	// ((40) : EXP(blue)
	// -(45) : EXP(red)
	// <(60): Stun
	// =(61): Bleed
	// >(62): Poison
	// ?(63): Burning
	// [(91): hold
	// ](93): Blind
	// ^(94): Freeze
	// {(123): Sleep
	// }(125): Slow
	// _(95): Debuff Resist
	// *(42) : Strengthen succeeded
	// +(43) : Strengthen failed
	// @(64) : Strengthen destroy
    // ~(126): Miracle
	// y(121): cos mix greate success
	// z(122): cos mix success
	// \(124): upgrade complete
    // :(58) : (AP)Ark Point
	public enum eCustomFontType { eCustomFontType_HP, eCustomFontType_MP, eCustomFontType_GOLD, eCustomFontType_EXP1, eCustomFontType_EXP2, eCustomFontType_MIRACLE, eCustomFontType_ArkPoint, eCustomFontType_AllyDamage}
	private char[] m_RedFontBuf = new char[10]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
	private char[] m_GreenFontBuf = new char[10]{ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'};
	private char[] m_BlueFontBuf = new char[10]{ 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'};
	private char[] m_YellowFontBuf = new char[10]{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'};
	private char[] m_WhiteFontBuf = new char[10]{ 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't'};
	private char[] m_RedFontBuf_User = new char[10]{ 'U', 'V', 'W', 'X', 'Y', 'Z', 'u', 'v', 'w', 'x'};

    private List<AsPanel_Name> listMonsterNamePanel = new List<AsPanel_Name>();
	
	private struct stDamageDuration
	{
		public GameObject _go;
		public int _nDamage;
		public eDAMAGETYPE _eDamageType;
		public float _fTime;
		public bool _bEnemy;
	}
	
	private List<stDamageDuration> m_listDamageDuration = new List<stDamageDuration>();
	private List<stDamageDuration> m_listRemoveBuf = new List<stDamageDuration>();
	
	private float m_fShowTime_Gold = 0.0f;
	private float m_fShowTime_Exp = 0.0f;
	
	// Use this for initialization
	void Start()
	{
		_Init();
	}
	
	// Update is called once per frame
	void Update()
	{
		int nCount = m_listDamageDuration.Count;
		if( 0 == nCount)
			return;
		
		float fCurTime = Time.time;
		
		foreach( stDamageDuration stData in m_listDamageDuration)
		{
			if( null == stData._go)
			{
				m_listRemoveBuf.Add( stData);
			}
			else
			{
				if( stData._fTime <= fCurTime)
				{
					ShowNumberPanel( stData._go, stData._nDamage, stData._eDamageType, stData._bEnemy);
					m_listRemoveBuf.Add( stData);
				}
			}
		}
		
		if( 0 < m_listRemoveBuf.Count)
		{
			foreach( stDamageDuration stRemoveData in m_listRemoveBuf)
				m_listDamageDuration.Remove( stRemoveData);
			
			m_listRemoveBuf.Clear();
		}
	}
	
	// < name panel
	public void CreateNamePanel_User( AsBaseEntity baseEntity, string strName, uint uiUniqueKey)
	{
		#region -Designation
//		AsUserEntity userEntity = baseEntity as AsUserEntity;
//		if( 0 < userEntity.DesignationID)
//		{
//			DesignationData data = AsDesignationManager.Instance.GetDesignation( userEntity.DesignationID);
//			strName = AsTableManager.Instance.GetTbl_String( data.name) + " " + strName;
//		}
		
		_CreateNamePanel( baseEntity, strName, uiUniqueKey, 0.0f, "none");
		#endregion
		
//		_CreateNamePanel( baseEntity, strName, uiUniqueKey, 0.0f);
	}
	
	public void CreateNamePanel_Npc(AsBaseEntity baseEntity, string strName, int nTableIndex)
	{
		/*
		Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record( nTableIndex);
		if( null == npcRec)
			return;

		_CreateNamePanel( baseEntity, strName, 0, npcRec.Scale);
		*/
		
		//$yde
		if( baseEntity.FsmType != eFsmType.PET)
		{
			Tbl_Npc_Record npcRec = AsTableManager.Instance.GetTbl_Npc_Record( nTableIndex);
			if( null == npcRec)
				return;
			
			_CreateNamePanel( baseEntity, strName, 0, 0.0f, npcRec.NickColor.ToLower());
		}
		else
		{
			Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord( nTableIndex);
			if( null == petRec)
				return;
			
			_CreateNamePanel( baseEntity, strName, 0, 0.0f, "none");
		}
	}
	// name panel >


	public void CreateQuestPanel_Npc(AsBaseEntity baseEntity, float namePanelPosY)
	{
		if (baseEntity.FsmType == eFsmType.NPC)
		{
			AsPanel_Quest questPanel = Instantiate(QuestPanel) as AsPanel_Quest;
			questPanel.gameObject.transform.parent = this.transform;
			questPanel.gameObject.transform.localPosition = Vector3.zero;

			questPanel.gameObject.name += baseEntity.name;

			questPanel.Create(baseEntity, namePanelPosY);
		}
	}

	public void CreateUseItemToTargetPanel(AsBaseEntity baseEntity)
	{
		if (baseEntity.FsmType == eFsmType.NPC || baseEntity.FsmType == eFsmType.COLLECTION)
			AsUseItemToTargetPanelManager.instance.CreatePanel(baseEntity);
	}
	
	public void ShowNumberPanel(GameObject go, int nDamage, bool isEnemy)
	{
		if( nDamage <= 0)
			return;
		
		if( true == isEnemy)
		{
			// damage(red font)
			_ShowNumberPanel( go, nDamage.ToString(), 0, true, false, m_fSpeed_Damage, m_fScale_Damage);
		}
		else
		{
			string strCustom = _GetCustomFont( nDamage, eCustomFontType.eCustomFontType_AllyDamage);
			_ShowNumberPanel( go, strCustom, 0, true, false, m_fSpeed_Damage, m_fScale_Damage);
		}
	}
	
	public void ShowNumberPanel_HP(GameObject go, int nHeal)
	{
		if( nHeal <= 0)
			return;
		
		// heal(green font)
		string strCustomFont = _GetCustomFont( nHeal, eCustomFontType.eCustomFontType_HP);
		_ShowNumberPanel( go, strCustomFont, 0, true, false, m_fSpeed_Damage_HP, m_fScale_Damage_HP);
	}
	
	public void ShowNumberPanel(GameObject go, int nDamage, eDAMAGETYPE eDamageType, bool isEnemy)
	{
		switch( eDamageType)
		{
		case eDAMAGETYPE.eDAMAGETYPE_MISS:
			_ShowNumberPanel( go, "#", 0, true, true, m_fSpeed_Damage_Miss, m_fScale_Damage_Miss); // miss
			break;
		
		case eDAMAGETYPE.eDAMAGETYPE_DODGE:
			_ShowNumberPanel( go, "%", 0, true, true, m_fSpeed_Damage_Dodge, m_fScale_Damage_Dodge); // dodge
			break;
			
		case eDAMAGETYPE.eDAMAGETYPE_REGIST:
			_ShowNumberPanel( go, "/", 0, true, true, m_fSpeed_Damage_Dodge, m_fScale_Damage_Dodge); // resist
			break;

		case eDAMAGETYPE.eDAMAGETYPE_NORMAL:
			if( nDamage > 0)
			{
				if( true == isEnemy)
				{
					_ShowNumberPanel( go, nDamage.ToString(), 0, true, false, m_fSpeed_Damage, m_fScale_Damage); // damage(red font)
				}
				else
				{
					string strCustom = _GetCustomFont( nDamage, eCustomFontType.eCustomFontType_AllyDamage);
					_ShowNumberPanel( go, strCustom, 0, true, false, m_fSpeed_Damage, m_fScale_Damage);
				}
			}
			break;

		case eDAMAGETYPE.eDAMAGETYPE_CRITICAL:
			_ShowNumberPanel( go, "!", 2, true, true, m_fSpeed_Damage_Critical, m_fScale_Damage_Critical); // critical
			if( nDamage > 0)
			{
				if( true == isEnemy)
				{
					_ShowNumberPanel( go, nDamage.ToString(), 0, true, false, m_fSpeed_Damage, m_fScale_Damage); // damage(red font)
				}
				else
				{
					string strCustom = _GetCustomFont( nDamage, eCustomFontType.eCustomFontType_AllyDamage);
					_ShowNumberPanel( go, strCustom, 0, true, false, m_fSpeed_Damage, m_fScale_Damage);
				}
			}
			break;
		}
	}
	
	public void ShowNumberPanel(GameObject go, int nDamage, eDAMAGETYPE eDamageType, Tbl_Action_Record actionRecord, bool isEnemy)
	{
		if( eDAMAGETYPE.eDAMAGETYPE_DODGE == eDamageType || eDAMAGETYPE.eDAMAGETYPE_REGIST == eDamageType || eDAMAGETYPE.eDAMAGETYPE_MISS == eDamageType)
		{
			ShowNumberPanel( go, nDamage, eDamageType, isEnemy);
			return;
		}
		
		if( null == actionRecord)
			return;
		if( null == actionRecord.HitAnimation)
			return;
		if( null == actionRecord.HitAnimation.hitInfo)
			return;
		
		Tbl_Action_HitInfo hitInfo = actionRecord.HitAnimation.hitInfo;
		
		if( eValueLookType.Duration == hitInfo.HitValueLookType)
		{
			stDamageDuration stData = new stDamageDuration();
			
			stData._go = go;
			stData._eDamageType = eDamageType;
			stData._bEnemy = isEnemy;

			int nDamageOnce = nDamage / (int)hitInfo.HitValueLookCount;
			int nDamageBuf = (int)( (float)nDamageOnce * 0.07f);
			int nTotalDamage = 0;
			float fTimeGab = hitInfo.HitValueLookDuration / ( hitInfo.HitValueLookCount * 1000);
			
			for( int i = 0; i < hitInfo.HitValueLookCount; i++)
			{
				if( i > 0 && eDAMAGETYPE.eDAMAGETYPE_CRITICAL == stData._eDamageType)
					stData._eDamageType = eDAMAGETYPE.eDAMAGETYPE_NORMAL;
				
				if( i == ( hitInfo.HitValueLookCount - 1))
					stData._nDamage = nDamage - nTotalDamage;
				else
				{
					stData._nDamage = ( i * nDamageBuf) + nDamageOnce;
					nTotalDamage += stData._nDamage;
				}
				
				stData._fTime = Time.time + ( fTimeGab * i);
				m_listDamageDuration.Add( stData);
			}
		}
		else
			ShowNumberPanel( go, nDamage, eDamageType, isEnemy);
	}
	
	// hp, mp
	public void ShowNumberPanel(AsUserEntity userEntity, int nValue, eATTRCHANGECONTENTS eContents, eCustomFontType eType)
	{
		ShowNumberPanel( userEntity, nValue, eContents, eType, eDAMAGETYPE.eDAMAGETYPE_NORMAL);
	}
	
	// hp, mp
	public void ShowNumberPanel(AsUserEntity userEntity, int nValue, eATTRCHANGECONTENTS eContents, eCustomFontType eType, eDAMAGETYPE eDamageType)
	{
		if( null == userEntity)
			return;
		
		if( eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_USING_SKILL == eContents
			|| eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_KILLNPC == eContents)
		{
			if( nValue <= 0)
				return;
			
			int nRes = nValue;
			float fSpeed = 1.0f;
			float fScale = 1.2f;
			
			if( eCustomFontType.eCustomFontType_HP == eType)
			{
				float maxHP = userEntity.GetProperty<float>( eComponentProperty.HP_MAX);
				float curHP = userEntity.GetProperty<float>( eComponentProperty.HP_CUR);
				float subHP = maxHP - curHP;
				if( subHP < nValue)
					nRes = (int)subHP;
				fSpeed = m_fSpeed_Damage_HP;
				fScale = m_fScale_Damage_HP;
			}
			else if( eCustomFontType.eCustomFontType_MP == eType)
			{
				float maxMP = userEntity.GetProperty<float>( eComponentProperty.MP_MAX);
				float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
				float subMP = maxMP - curMP;
				if( subMP < nValue)
					nRes = (int)subMP;
				fSpeed = m_fSpeed_Damage_MP;
				fScale = m_fScale_Damage_MP;
			}
			
			if( nRes >= 0)
			{
				if( eDAMAGETYPE.eDAMAGETYPE_CRITICAL == eDamageType)
					_ShowNumberPanel( userEntity.transform.gameObject, "!", 2, true, true, m_fSpeed_Damage_Critical, m_fScale_Damage_Critical); // critical user
				
				string str = _GetCustomFont( nRes, eType);
				_ShowNumberPanel( userEntity.transform.gameObject, str, 0, false, false, fSpeed, fScale); // HP, MP user
			}
		}
		else if( eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_DOT_DAMAGE == eContents)
		{
//			if( nValue <= 0)
//				return;
//
//			float maxHP = userEntity.GetProperty<float>( eComponentProperty.HP_MAX);
//			float curHP = userEntity.GetProperty<float>( eComponentProperty.HP_CUR);
//			float subHP = maxHP - curHP;
//			int nRes = nValue;
//			if( subHP < nValue)
//				nRes = (int)subHP;
//			
//			if( nRes > 0)
//			{
//				string str = _GetCustomFont( nRes, eType);
//				_ShowNumberPanel( userEntity.transform.gameObject, str, 0, false, false, m_fSpeed_Damage_DotHeal, m_fScale_Damage_DotHeal); // Dot Healling user
//			}
			
			string str = "";
			int nRes = nValue;
			
			if( nValue <= 0)
			{
				nRes *= -1;
				//str = nRes.ToString(); // dot damage
				str = _GetCustomFont( nRes, eCustomFontType.eCustomFontType_AllyDamage);
			}
			else
				str = _GetCustomFont( nRes, eType); // dot healling
			
			_ShowNumberPanel( userEntity.transform.gameObject, str, 0, false, false, m_fSpeed_Damage_DotHeal, m_fScale_Damage_DotHeal); // Dot Healling user
		}
		else if( eATTRCHANGECONTENTS.eATTRCHANGECONTENTS_HP_INSTEAD_MP == eContents)
		{
			int nRes = nValue;
			if( nValue <= 0)
				nRes *= -1;
			
			if( eDAMAGETYPE.eDAMAGETYPE_CRITICAL == eDamageType)
				_ShowNumberPanel( userEntity.transform.gameObject, "!", 2, true, true, m_fSpeed_Damage_Critical, m_fScale_Damage_Critical); // critical user
			
			_ShowNumberPanel( userEntity.transform.gameObject, nRes.ToString(), 0, true, false, m_fSpeed_Damage, m_fScale_Damage); // damage(red font)
		}
	}
	
	// UserPlayer Only( gold)
	public void ShowNumberPanel(int nValue, eCustomFontType eType)
	{
		ShowNumberPanel( (long)nValue, eType);
	}

	// UserPlayer Only( gold, exp)
	public void ShowNumberPanel(long nValue, eCustomFontType eType)
	{
		if ((0 >= nValue) && ((eCustomFontType.eCustomFontType_GOLD == eType) || (eCustomFontType.eCustomFontType_EXP2 == eType) || 
                              (eCustomFontType.eCustomFontType_MIRACLE == eType) || (eCustomFontType.eCustomFontType_ArkPoint == eType)))
			return;
		
		Transform charTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		if( null == charTransform)
			return;
		
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		if( null == userEntity)
			return;
		
		string str = _GetCustomFont( nValue, eType);
		int nUIYPosRevisionFlag = 0;
		float fSpeed = 1.0f;
		float fScale = 1.0f;

		if( eCustomFontType.eCustomFontType_GOLD == eType)
		{
			str = str + '$';
			if( Time.time - m_fShowTime_Exp < 0.2f)
				nUIYPosRevisionFlag = 2;
			m_fShowTime_Gold = Time.time;
			fSpeed = m_fSpeed_Damage_Gold;
			fScale = m_fScale_Damage_Gold;
		}
		else if (eCustomFontType.eCustomFontType_MIRACLE == eType)
		{
			str = str + '~';
			if (Time.time - m_fShowTime_Exp < 0.2f)
				nUIYPosRevisionFlag = 2;
			m_fShowTime_Gold = Time.time;
			fSpeed = m_fSpeed_Damage_Gold;
			fScale = m_fScale_Damage_Gold;
		}
		else if (eCustomFontType.eCustomFontType_EXP1 == eType)
		{
			if (0 == nValue)
				str += '-';
			else
				str += '&';
			if (Time.time - m_fShowTime_Gold < 0.2f)
				nUIYPosRevisionFlag = 2;
			m_fShowTime_Exp = Time.time;
			fSpeed = m_fSpeed_Damage_Exp;
			fScale = m_fScale_Damage_Exp;
		}
		else if (eCustomFontType.eCustomFontType_EXP2 == eType)
		{
			str = str + '(';
			StartCoroutine(_ShowExp2(charTransform.gameObject, str, nUIYPosRevisionFlag, false, false, m_fSpeed_Damage_Exp2, m_fScale_Damage_Exp2)); // exp2
			return;
		}
        else if (eCustomFontType.eCustomFontType_ArkPoint == eType)
        {
            str = str + ':';
            if (Time.time - m_fShowTime_Exp < 0.2f)
                nUIYPosRevisionFlag = 2;
            m_fShowTime_Gold = Time.time;
            fSpeed = m_fSpeed_Damage_Gold;
            fScale = m_fScale_Damage_Gold;
        }
		
		_ShowNumberPanel( charTransform.gameObject, str, nUIYPosRevisionFlag, false, false, fSpeed, fScale); // gold, exp
	}
	
	public void ShowBuffPanel(GameObject go, eBUFFTYPE buffType)
	{
		if( null == go)
			return;
		
		string strCustom = "";
		
		switch( buffType)
		{
		case eBUFFTYPE.eBUFFTYPE_STUN_NOTHING: strCustom = "<"; break;
		case eBUFFTYPE.eBUFFTYPE_FREEZE_PROB: strCustom = "^"; break;
		case eBUFFTYPE.eBUFFTYPE_SLEEP_PROB: strCustom = "{"; break;
		case eBUFFTYPE.eBUFFTYPE_BURNING_ADDPROB:
			strCustom = "?"; break;
		case eBUFFTYPE.eBUFFTYPE_BLEEDING_ADDPROB:
			strCustom = "="; break;
		case eBUFFTYPE.eBUFFTYPE_POISON_ADDPROB:
			strCustom = ">"; break;
		case eBUFFTYPE.eBUFFTYPE_BIND_NOTHING: strCustom = "["; break;
		case eBUFFTYPE.eBUFFTYPE_BLIND_PROB: strCustom = "]"; break;
		case eBUFFTYPE.eBUFFTYPE_SLOW_PROB: strCustom = "}"; break;
		}

		if( strCustom.Length > 0)
			_ShowNumberPanel( go, strCustom, -2, true, true, m_fSpeed_Damage_Buff, m_fScale_Damage_Buff); // buff
	}
	
	public void ShowDebuffResist(GameObject go)
	{
		if( null == go)
			return;
		
		string strCustom = "_";
		_ShowNumberPanel( go, strCustom, -2, true, true, m_fSpeed_Damage_Buff, m_fScale_Damage_Buff); // debuff resist
	}
	
	public void ShowStrengthenPanel( GameObject go, eRESULTCODE resultCode, uint uniqKey)
	{
		// *(42) : Strengthen succeeded
		// +(43) : Strengthen failed
		// @(64) : Strengthen destroy
		switch( resultCode)
		{
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL:
			_ShowNumberPanel( go, "@", 0, false, false, 1.0f, 1.2f);
			break;
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_FAIL_PROTECT:
			_ShowNumberPanel( go, "+", 0, false, false, 1.0f, 1.2f);
			break;
		case eRESULTCODE.eRESULT_ITEM_STRENGTHEN_SUCCESS:
			_ShowNumberPanel( go, "*", 0, false, false, 1.0f, 1.2f);
			#region -GameGuide_Upgrade
			if( AsUserInfo.Instance.SavedCharStat.uniqKey_ == uniqKey)
				AsGameGuideManager.Instance.CheckUp( eGameGuideType.Upgrade, -1);
			#endregion
			break;
		}
	}

	public void ShowCosMixResult( string str )
	{
		// y(121): cos mix success
		// z(122): cos mix greate success
		// \(124): upgrade complete

		Transform charTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		if( null == charTransform)
			return;

		_ShowNumberPanel( charTransform.gameObject , str , 0, false, false, 1.0f, 1.2f);
	}

	// < private
	private void _ShowNumberPanel(GameObject go, string strCustom, int nUIYPosRevisionFlag, bool isUseHeightScale, bool isUseInteractivePanel, float fSpeed, float fScale)
	{
		if( strCustom.Length == 0)
			return;
		
		AsPanel_Damage damagePanel = Instantiate( DamagePanel) as AsPanel_Damage;
		damagePanel.gameObject.transform.parent = this.transform;
		damagePanel.gameObject.transform.localPosition = Vector3.zero;
		damagePanel.Show( go, strCustom, Color.white, fScale, nUIYPosRevisionFlag, isUseHeightScale, isUseInteractivePanel, fSpeed);
	}
	
	private string _GetCustomFont(int nValue, eCustomFontType eType)
	{
		return _GetCustomFont( (long)nValue, eType);
	}
	
	private string _GetCustomFont(long nValue, eCustomFontType eType)
	{
		if( nValue < 0)
			nValue = 0;

		char[] FontBuf = new char[10];
		if( eCustomFontType.eCustomFontType_HP == eType)
			FontBuf = m_GreenFontBuf;
		else if( eCustomFontType.eCustomFontType_MP == eType || eCustomFontType.eCustomFontType_MIRACLE == eType)
			FontBuf = m_BlueFontBuf;
		else if( eCustomFontType.eCustomFontType_GOLD == eType || eCustomFontType.eCustomFontType_ArkPoint == eType)
			FontBuf = m_YellowFontBuf;
		else if( eCustomFontType.eCustomFontType_EXP1 == eType)
			FontBuf = ( 0 == nValue) ? m_RedFontBuf_User : m_WhiteFontBuf;
		else if( eCustomFontType.eCustomFontType_EXP2 == eType)
			FontBuf = m_BlueFontBuf;
		else if( eCustomFontType.eCustomFontType_AllyDamage == eType)
			FontBuf = m_RedFontBuf_User;
		else
			return "";
		
		int nBuf = 0;
		string str = nValue.ToString();
		char[] strBuf = new char[str.Length];

		for( int i = 0; i < str.Length; i++)
		{
			nBuf = int.Parse( str[i].ToString());
			strBuf[i] = FontBuf[nBuf];
		}
		
		string strRes = new string( strBuf);
		return strRes;
	}

	private AsPanel_Name _CreateNamePanel(AsBaseEntity baseEntity, string strName, uint uiUserUniqueKey, float fSize, string strColorLower)
	{
		if( GAME_STATE.STATE_CHARACTER_SELECT == AsGameMain.s_gameState || GAME_STATE.STATE_CHARACTER_CREATE == AsGameMain.s_gameState)
			return null;

		if( null == baseEntity || null == baseEntity.ModelObject || string.Empty == strName)
			return null;
		
		AsPanel_Name namePanel = Instantiate( NamePanel) as AsPanel_Name;
		namePanel.gameObject.transform.parent = this.transform;
		namePanel.gameObject.transform.localPosition = Vector3.zero;

		Color nameColor = Color.white;
		AsPanel_Name.eNamePanelType eType = AsPanel_Name.eNamePanelType.eNamePanelType_None;
		
		if( eEntityType.USER == baseEntity.EntityType)
		{
			// user
			nameColor = m_NameColor_User;
			eType = AsPanel_Name.eNamePanelType.eNamePanelType_User;
		}
		else if( eEntityType.NPC == baseEntity.EntityType && eFsmType.NPC == baseEntity.FsmType)
		{
			// npc
			Color tableColor = _GetStringToColor( strColorLower);
			nameColor = ( Color.clear != tableColor) ? tableColor : m_NameColor_Npc;
			eType = AsPanel_Name.eNamePanelType.eNamePanelType_Npc;
		}
		else if( eEntityType.NPC == baseEntity.EntityType && (eFsmType.MONSTER == baseEntity.FsmType ||eFsmType.COLLECTION == baseEntity.FsmType))
		{
			// monster
			Color tableColor = _GetStringToColor( strColorLower);
			if( eFsmType.COLLECTION == baseEntity.FsmType )
				nameColor = ( Color.clear != tableColor) ? tableColor : m_NameColor_Default;
			else
			{
				eMonster_AttackType eAttackType = baseEntity.GetProperty<eMonster_AttackType>( eComponentProperty.MONSTER_ATTACK_TYPE);
				if( eMonster_AttackType.Fool == eAttackType)
					nameColor = ( Color.clear != tableColor) ? tableColor : m_NameColor_Monster_Fool;
				else if( eMonster_AttackType.Peaceful == eAttackType)
					nameColor = ( Color.clear != tableColor) ? tableColor : m_NameColor_Monster_Peaceful;
				else if( eMonster_AttackType.Offensive == eAttackType)
					nameColor = ( Color.clear != tableColor) ? tableColor : m_NameColor_Monster_Offensive;
			}
			if( eFsmType.COLLECTION == baseEntity.FsmType )
				eType = AsPanel_Name.eNamePanelType.eNamePanelType_Collect;
			else
				eType = AsPanel_Name.eNamePanelType.eNamePanelType_Monster;
		}
		else
		{
			nameColor = m_NameColor_Default;
			eType = AsPanel_Name.eNamePanelType.eNamePanelType_None;
		}
		
		namePanel.Create( baseEntity, strName, nameColor, eType, uiUserUniqueKey, fSize);


        // for monster target mark
        if (baseEntity.FsmType == eFsmType.MONSTER)
        {
            if (namePanel != null)
            {
                namePanel.callBeforDestory = DeleteMonsterNamePanel;
                listMonsterNamePanel.Add(namePanel);
            }
        }
		
		return namePanel;
	}
	
	private Color _GetStringToColor(string strColorLower)
	{
		if( null == strColorLower || 0 == strColorLower.Length || strColorLower.Equals( "none"))
			return Color.clear;

		int n1 = strColorLower.IndexOf('(');
		int n2 = strColorLower.IndexOf(')');
		string strBuff = strColorLower.Substring( n1 + 1, n2 - n1 - 1);
		string[] res = strBuff.Split( ',');
		return new Color( Convert.ToSingle( res[0]), Convert.ToSingle( res[1]), Convert.ToSingle( res[2]), Convert.ToSingle( res[3]));
	}
	
	private IEnumerator _ShowExp2(GameObject go, string strCustom, int nUIYPosRevisionFlag, bool isUseHeightScale, bool isUseInteractivePanel, float fSpeed, float fScale)
	{
		yield return new WaitForSeconds( 0.2f);
		_ShowNumberPanel( go, strCustom, nUIYPosRevisionFlag, isUseHeightScale, isUseInteractivePanel, fSpeed, fScale);
	}
	
	private void _Init()
	{
		_InitSpeed( ref m_fSpeed_Damage);
		_InitSpeed( ref m_fSpeed_Damage_Miss);
		_InitSpeed( ref m_fSpeed_Damage_Dodge);
		_InitSpeed( ref m_fSpeed_Damage_Critical);
		_InitSpeed( ref m_fSpeed_Damage_HP);
		_InitSpeed( ref m_fSpeed_Damage_MP);
		_InitSpeed( ref m_fSpeed_Damage_DotHeal);
		_InitSpeed( ref m_fSpeed_Damage_Gold);
		_InitSpeed( ref m_fSpeed_Damage_Exp);
		_InitSpeed( ref m_fSpeed_Damage_Exp2);
		_InitSpeed( ref m_fSpeed_Damage_Buff);

		_InitScale( ref m_fScale_Damage);
		_InitScale( ref m_fScale_Damage_Miss);
		_InitScale( ref m_fScale_Damage_Dodge);
		_InitScale( ref m_fScale_Damage_Critical);
		_InitScale( ref m_fScale_Damage_HP);
		_InitScale( ref m_fScale_Damage_MP);
		_InitScale( ref m_fScale_Damage_DotHeal);
		_InitScale( ref m_fScale_Damage_Gold);
		_InitScale( ref m_fScale_Damage_Exp);
		_InitScale( ref m_fScale_Damage_Exp2);
		_InitScale( ref m_fScale_Damage_Buff);
	}
	
	private void _InitSpeed(ref float f)
	{
		if( f < m_fSpeedMin)
			f = m_fSpeedMin;
		if( f > m_fSpeedMax)
			f = m_fSpeedMax;
	}
	
	private void _InitScale(ref float f)
	{
		if( f < m_fScaleMin)
			f = m_fScaleMin;
		if( f > m_fScaleMax)
			f = m_fScaleMax;
	}

    private void DeleteMonsterNamePanel(AsPanel_Name _panelName)
    {
        if (listMonsterNamePanel.Contains(_panelName))
            listMonsterNamePanel.Remove(_panelName);
        else
            Debug.LogWarning("Not exist Monster Name Panel");
    }

    public void UpdateMonsterNamePanel()
    {
        for (int i = 0; i < listMonsterNamePanel.Count; i++)
            listMonsterNamePanel[i].UpdateMonsterTarkMark();
    }
	// private >
}
