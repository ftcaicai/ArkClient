using UnityEngine;
using System.Collections;
using System.Text;

public class AsPvpUserInfo : MonoBehaviour
{
	public SpriteText m_Loading = null;
	public SpriteText m_Offline = null;
	
	public SimpleSprite m_ImgRank;
	public SpriteText m_TextLevel = null;
	public SpriteText m_TextName = null;
	public SpriteText m_TextRate = null;
	
	public GameObject m_goClassIcon = null;
	public GameObject m_goGrade_Platinum = null;
	public GameObject m_goGrade_Gold = null;
	public GameObject m_goGrade_Silver = null;
	public GameObject m_goGrade_Bronze = null;
	public GameObject m_goBG = null;
	
	private sARENAUSERINFO m_ArenaUserInfo = new sARENAUSERINFO();
	private Color m_Color_Ally = new Color( 0.09f, 0.6f, 1.0f, 1.0f);
	private Color m_Color_Enemy = new Color( 0.9f, 0.23f, 0.08f, 1.0f);
	
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Loading);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_Offline);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextLevel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextRate);
		
		m_Loading.Text = AsTableManager.Instance.GetTbl_String( 918);
		m_Offline.Text = AsTableManager.Instance.GetTbl_String( 925);
	}
	
	void Update()
	{
	}
	
	public void SetUserInfo(sARENAUSERINFO userInfo, bool isMyTeam)
	{
		m_ArenaUserInfo = userInfo;
		
		m_TextName.Text = userInfo.szCharName;
		if( true == isMyTeam)
			m_TextName.SetColor( m_Color_Ally);
		else
			m_TextName.SetColor( m_Color_Enemy);
		
		m_TextLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String( 901), userInfo.nLevel);
		m_TextRate.Text = string.Format( AsTableManager.Instance.GetTbl_String( 982), userInfo.nRate);
//		_SetRankImage( userInfo.nPvpPoint);
		_SetDelegateImage( userInfo.nImageTableIdx);
		_SetClassIcon( userInfo.eClass);
		
		_ApplyUserInfoState( (eARENAUSER_ENTERSTATE)m_ArenaUserInfo.nEnterState);
	}
	
	public void SetUserState(int nEnterState)
	{
		m_ArenaUserInfo.nEnterState = nEnterState;
		_ApplyUserInfoState( (eARENAUSER_ENTERSTATE)m_ArenaUserInfo.nEnterState);
	}
	
	public uint GetCharUniqKey()
	{
		return m_ArenaUserInfo.nCharUniqKey;
	}
	
	#region -private
	private void _ApplyUserInfoState(eARENAUSER_ENTERSTATE eArenaUserEnterState)
	{
		switch( eArenaUserEnterState)
		{
		case eARENAUSER_ENTERSTATE.eARENAUSER_ENTERSTATE_NOTHING: _ActiveInfo( true, false, false); break;
		case eARENAUSER_ENTERSTATE.eARENAUSER_ENTERSTATE_LOADING: _ActiveInfo( true, false, false); break;
		case eARENAUSER_ENTERSTATE.eARENAUSER_ENTERSTATE_ENTERED: _ActiveInfo( false, false, true); break;
		case eARENAUSER_ENTERSTATE.eARENAUSER_ENTERSTATE_LOGOUT: _ActiveInfo( false, true, false); break;
		case eARENAUSER_ENTERSTATE.eARENAUSER_ENTERSTATE_NOTENTERING: _ActiveInfo( false, true, false); break;
		}
	}
	
//	private void _SetRankImage(int nPvpPoint)
//	{
//		int nID = AsPvpManager.Instance.FindPvpGradeID( nPvpPoint);
//		Tbl_PvpGrade_Record record = AsTableManager.Instance.GetPvpGradeRecord( nID);
//		if( null == record)
//		{
//			Debug.Log( "_SetRankImage(): record == null, nID: " + nID);
//			return;
//		}
//		
//		Texture tex = ResourceLoad.Loadtexture( record.GradeImagePath);
//		m_ImgRank.renderer.material.mainTexture = tex;
//	}
	
	private void _SetDelegateImage(int nImageID)
	{
		DelegateImageData data = AsDelegateImageManager.Instance.GetDelegateImage( nImageID);
		string strIconName = "";
		if( null == data)
			strIconName = "Default";
		else
			strIconName = data.iconName;

		StringBuilder sb = new StringBuilder( "UIPatchResources/DelegateImage/");
		sb.Append( strIconName);
		Texture tex = ResourceLoad.Loadtexture( sb.ToString());
		m_ImgRank.renderer.material.mainTexture = tex;
	}
	
	private void _SetClassIcon(eCLASS eClass)
	{
		Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record( m_ArenaUserInfo.eClass);
		if( null != record)
		{
			GameObject go = Instantiate( ResourceLoad.LoadGameObject( record.JobIcon)) as GameObject;
			
			if( null != go)
			{
				go.transform.parent = m_goClassIcon.transform;
				go.transform.localPosition = Vector3.zero;
			}
		}
		else
		{
			Debug.Log( "AsPvpUserInfo::_SetClassIcon(), Tbl_Class_Record record == null");
		}
	}
	
	private void _ActiveInfo(bool bActiveLoading, bool bActiveOffline, bool bActiveInfo)
	{
		// loading
		if( false == bActiveLoading)
		{
			if( true == m_Loading.gameObject.active)
				m_Loading.gameObject.SetActiveRecursively( false);
		}
		else
		{
			if( false == m_Loading.gameObject.active)
				m_Loading.gameObject.SetActiveRecursively( true);
		}
		
		// offline
		if( false == bActiveOffline)
		{
			if( true == m_Offline.gameObject.active)
				m_Offline.gameObject.SetActiveRecursively( false);
		}
		else
		{
			if( false == m_Offline.gameObject.active)
				m_Offline.gameObject.SetActiveRecursively( true);
		}
		
		// info
		if( false == bActiveInfo)
		{
			if( true == m_ImgRank.gameObject.active)
				m_ImgRank.gameObject.SetActiveRecursively( false);
			if( true == m_TextLevel.gameObject.active)
				m_TextLevel.gameObject.SetActiveRecursively( false);
			if( true == m_TextName.gameObject.active)
				m_TextName.gameObject.SetActiveRecursively( false);
			if( true == m_TextRate.gameObject.active)
				m_TextRate.gameObject.SetActiveRecursively( false);
			
			if( true == m_goClassIcon.gameObject.active)
				m_goClassIcon.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Platinum.gameObject.active)
				m_goGrade_Platinum.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Gold.gameObject.active)
				m_goGrade_Gold.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Silver.gameObject.active)
				m_goGrade_Silver.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Bronze.gameObject.active)
				m_goGrade_Bronze.gameObject.SetActiveRecursively( false);
			if( false == m_goBG.gameObject.active)
				m_goBG.gameObject.SetActiveRecursively( true);
		}
		else
		{
			if( false == m_ImgRank.gameObject.active)
				m_ImgRank.gameObject.SetActiveRecursively( true);
			if( false == m_TextLevel.gameObject.active)
				m_TextLevel.gameObject.SetActiveRecursively( true);
			if( false == m_TextName.gameObject.active)
				m_TextName.gameObject.SetActiveRecursively( true);
			if( false == m_TextRate.gameObject.active)
				m_TextRate.gameObject.SetActiveRecursively( true);
			
			// grade
			if( false == m_goClassIcon.gameObject.active)
				m_goClassIcon.gameObject.SetActiveRecursively( true);
			if( true == m_goGrade_Platinum.gameObject.active)
				m_goGrade_Platinum.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Gold.gameObject.active)
				m_goGrade_Gold.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Silver.gameObject.active)
				m_goGrade_Silver.gameObject.SetActiveRecursively( false);
			if( true == m_goGrade_Bronze.gameObject.active)
				m_goGrade_Bronze.gameObject.SetActiveRecursively( false);
			if( false == m_goBG.gameObject.active)
				m_goBG.gameObject.SetActiveRecursively( true);

			//int nID = AsPvpManager.Instance.FindPvpGradeID( m_ArenaUserInfo.nPvpPoint);
//			AsUserEntity userEntity = AsEntityManager.Instance.GetUserEntityByUniqueId( m_ArenaUserInfo.nCharUniqKey);
//			if( null == userEntity)
//			{
//				Debug.Log( "AsPvpUserInfo::_ActiveInfo(), null == userEntity, m_ArenaUserInfo.nCharUniqKey: " + m_ArenaUserInfo.nCharUniqKey);
//				return;
//			}
//			
//			int nYesterdayPvpPoint = userEntity.YesterdayPvpPoint;
//			uint nYesterdayPvpRankRate = userEntity.YesterdayPvpRankRate;
//			
//			if( m_ArenaUserInfo.nCharUniqKey == AsUserInfo.Instance.SavedCharStat.uniqKey_)
//			{
//				nYesterdayPvpPoint = AsUserInfo.Instance.YesterdayPvpPoint;
//				nYesterdayPvpRankRate = AsUserInfo.Instance.YesterdayPvpRankRate;
//			}
//			
//			int nID = AsPvpManager.Instance.FindPvpGradeID( nYesterdayPvpPoint, nYesterdayPvpRankRate);
			int nID = AsPvpManager.Instance.FindPvpGradeID( m_ArenaUserInfo.nYesterdayPvpPoint, m_ArenaUserInfo.nYesterdayPvpRankRate);
			Tbl_PvpGrade_Record grade = AsTableManager.Instance.GetPvpGradeRecord( nID);
			if( null != grade)
			{
				switch( grade.GradeImageType)
				{
				case 1: m_goGrade_Platinum.SetActiveRecursively( true); break;
				case 2: m_goGrade_Gold.SetActiveRecursively( true); break;
				case 3: m_goGrade_Silver.SetActiveRecursively( true); break;
				case 4: m_goGrade_Bronze.SetActiveRecursively( true); break;
				}
			}
			else
			{
				Debug.Log( "AsPvpUserInfo::_HideInfo(), Tbl_PvpGrade_Record grade == null, PvpPoint: " + m_ArenaUserInfo.nPvpPoint);
			}
		}
	}
	#endregion -private
}
