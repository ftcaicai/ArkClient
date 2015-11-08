using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPromotionManager : MonoBehaviour {
	
	#region - singleton -
	static AsPromotionManager m_Instance; public static AsPromotionManager Instance{get{return m_Instance;}}
	#endregion
	
	AsPromotionDlg m_Dlg;
	
	AsUserEntity m_Player = null;
	Tbl_Promotion_Record m_Record = null;
	
	Def_Promotion.eCondition m_ReservedCondition = Def_Promotion.eCondition.Login;
	
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	bool SetPlayerInfo(Def_Promotion.eCondition _condition)
	{
		m_Player = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if(m_Player != null)
		{
			Tbl_Promotion_Table promotionTable = AsTableManager.Instance.GetTbl_PromotionTable();
			
			eCLASS __class = m_Player.GetProperty<eCLASS>(eComponentProperty.CLASS);
			
			if(promotionTable != null)
			{
				MultiDictionary<Def_Promotion.eCondition, Tbl_Promotion_Record> mdicRecords = promotionTable.GetRecordsByClass(__class);
				if(mdicRecords != null)
				{
					List<Tbl_Promotion_Record> listRecord = mdicRecords[_condition];
					
					if(listRecord.Count == 0)
					{
						Debug.LogWarning("AsPromotionManager::SetPlayerInfo: no conditioned record. check promotion table. __class = " + __class);
						return false;
					}
					
					m_Record = null;
					foreach(Tbl_Promotion_Record node in listRecord)
					{
						if(node.CheckValidLevel(m_Player.GetProperty<int>(eComponentProperty.LEVEL)) == true)
						{
							if(m_Record == null || node.Priority < m_Record.Priority )
								m_Record = node;
						}
					}
					
					if(m_Record != null)
						return true;
					else
						return false;
				}
				else
				{
					Debug.LogWarning("AsPromotionManager::SetPlayerInfo: no classified records. __class = " + __class);
					return false;
				}
			}
			else
			{
				Debug.LogError("AsPromotionManager::SetPlayerInfo: promotionTable is null. AsGameMain.s_gameState = " + AsGameMain.s_gameState);
				return false;
			}
		}
		else
		{
			Debug.LogError("AsPromotionManager::SetPlayerInfo: player entity is null. AsGameMain.s_gameState = " + AsGameMain.s_gameState);
			return false;
		}
	}
	
	void InitDlg()
	{
		float prob = Random.Range(0f, 1000f);
		if(prob > m_Record.Probability)
			return;
		
		if(m_Dlg != null)
			Destroy(m_Dlg.gameObject);
		
		GameObject goDlg = Instantiate(Resources.Load("UI/AsGUI/GUI_PromotionDlg")) as GameObject;
		m_Dlg = goDlg.GetComponent<AsPromotionDlg>();
		m_Dlg.Init(m_Record);
	}
	
	public void Revived()
	{
		if(SetPlayerInfo(Def_Promotion.eCondition.Revival) == false)
			return;
		
		InitDlg();
	}
	
	public void SceneLoaded()
	{
		bool succeed = false;
		
		switch(m_ReservedCondition)
		{
		case Def_Promotion.eCondition.Login:
			succeed = SetPlayerInfo(Def_Promotion.eCondition.Login);
			break;
		case Def_Promotion.eCondition.Zone:
			succeed = SetPlayerInfo(Def_Promotion.eCondition.Zone);
			break;
		case Def_Promotion.eCondition.Channel:
			succeed = SetPlayerInfo(Def_Promotion.eCondition.Channel);
			break;
		case Def_Promotion.eCondition.Revival:
			succeed = SetPlayerInfo(Def_Promotion.eCondition.Revival);
			break;
		default:
			Debug.LogWarning("AsPromotionManager::SceneLoaded: m_ReservedCondition = " + m_ReservedCondition);
			return;
		}
		
		if(succeed == true)
			InitDlg();
	}
	
	public void ConditionExhausted()
	{
		if(SetPlayerInfo(Def_Promotion.eCondition.Condition) == false)
			return;
		
		InitDlg();
	}
	
	public void Reserve_LoggedIn()
	{
		m_ReservedCondition = Def_Promotion.eCondition.Login;
	}
	
	public void Reserve_ZoneMoved()
	{
		m_ReservedCondition = Def_Promotion.eCondition.Zone;
	}
	
	public void Reserve_ChannelChanged()
	{
		m_ReservedCondition = Def_Promotion.eCondition.Channel;
	}
	
	public void Reserve_Revive()
	{
		m_ReservedCondition = Def_Promotion.eCondition.Revival;
	}
}
