using UnityEngine;
using System.Collections;

public class BonusManager : MonoBehaviour
{
	#region - singleton -
	static BonusManager m_Instance; public static BonusManager Instance	{ get { return m_Instance; } }
	#endregion
	
	body_SC_BONUS_ATTENDANCE m_Attend = null;
	body_SC_BONUS_RETURN m_Return = null;
	
	int m_CompleteLevel = 0; public int CompleteLevel{get{return m_CompleteLevel;}}
	LevelBonusWindow m_LevelBonusWindow = null;
	
	[SerializeField] UIButton m_LevelBonusBtn;
	[SerializeField] SimpleSprite m_LevelBonusSprite;
	[SerializeField] SpriteText m_LevelBonusText;

	[SerializeField] GameObject m_LevelBonusEffect;
	bool m_LevelUpBonusReceived = false;
	
	#region - init & update & release -
	void Awake()
	{
		#region - singleton -
		m_Instance = this;
		#endregion
	}

	// Use this for initialization
	void Start()
	{
//		if(m_CompleteLevel <= Tbl_UserLevel_Table.FirstLevelUpRewardLv)
//			m_LevelBonusEffect.gameObject.layer = LayerMask.NameToLayer("Default");
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	#endregion
	#region - ui input -
	public void GuiInputClickUp( Ray inputRay)
	{
		if( AttendBonusWindow.Instance != null )
		{
			AttendBonusWindow.Instance.GuiInputClickUp(inputRay);
		}
	}
	#endregion
	#region - send -
	bool m_PacketSend = false;
	public void Send_LevelUpBonus(int _lv)
	{
		if(m_PacketSend == true)
			return;

		body_CS_LEVEL_BONUS bonus = new body_CS_LEVEL_BONUS(_lv);
		AsCommonSender.Send(bonus.ClassToPacketBytes());

		m_PacketSend = true;
	}
	#endregion
	#region - recv -
	public void Recv_AttendBonus(body_SC_BONUS_ATTENDANCE _bonus)
	{
		Debug.Log("BonusManager::Recv_AttenBonus: _bonus = " + _bonus);
		
		m_Attend = _bonus;
		
		if(CheckMapValidity() == true)
			OpenAttendBonus();
	}
	
	public void Recv_ReturnBonus(body_SC_BONUS_RETURN _bonus)
	{
		Debug.Log("BonusManager::Recv_ReturnBonus: _bonus = " + _bonus);
		
		m_Return = _bonus;
		
		if(CheckMapValidity() == true)
			OpenReturnBonus();
	}
	
	bool CheckMapValidity()
	{
		Map map = TerrainMgr.Instance.GetCurrentMap();
		if(map != null)
		{
			Debug.Log("BonusManager::CHeckMapValidity: map.MapData.getMapType = " + map.MapData.getMapType);
			
			if(map.MapData.getMapType == eMAP_TYPE.Tutorial)
				return false;
			else
				return true;
		}
		else
		{
			Debug.Log("BonusManager::CHeckMapValidity: TerrainMgr.Instance.GetCurrentMap() = " + TerrainMgr.Instance.GetCurrentMap());
			
			return false;
		}
	}
		
	void OpenAttendBonus()
	{
		if(m_Attend != null)
		{
			GameObject obj = Instantiate( Resources.Load( "UI/AsGUI/UI_DailyAttend")) as GameObject;
			AttendBonusWindow window = obj.GetComponent<AttendBonusWindow>();
			window.Init( m_Attend);
			QuestTutorialMgr.Instance.attendBonus = true;
			
			m_Attend = null;
		}
	}
	
	void OpenReturnBonus()
	{
		if(m_Return != null)
		{
			GameObject obj = Instantiate( Resources.Load( "UI/AsGUI/UI_ReturnEvent")) as GameObject;
			ReturnBonusWindow window = obj.GetComponent<ReturnBonusWindow>();
			window.Init( m_Return);
			
			m_Return = null;
		}
	}
	
	public void Recv_LevelBonus(byte[] _packet)
	{
		m_PacketSend = false;

		body_SC_LEVEL_BONUS_RESULT bonus = new body_SC_LEVEL_BONUS_RESULT();
		bonus.PacketBytesToClass(_packet);
		
		Debug.Log("BonusManager::Recv_LevelBonus: bonus.eResult = " + bonus.eResult);
//		Debug.Log("BonusManager::Recv_LevelBonus: bonus.nCompleteLevel = " + bonus.nCompleteLevel);

		if(bonus.eResult == eRESULTCODE.eRESULT_SUCC)
		{
			m_CompleteLevel = bonus.nCompleteLevel;
			if(m_LevelBonusWindow != null)
			{
				m_LevelBonusWindow.Init(m_CompleteLevel);
				
				string title = AsTableManager.Instance.GetTbl_String(126);
	//			string content = string.Format(AsTableManager.Instance.GetTbl_String(4111), m_CompleteLevel);
				eCLASS playerClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
				Tbl_UserLevel_Record rec = AsTableManager.Instance.GetTbl_Level_Record(playerClass, m_CompleteLevel);
				if(rec != null)
				{
					Item item = ItemMgr.ItemManagement.GetItem(rec.Lv_Bonus);
					if(item != null)
					{
						string itemName = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
						string content = string.Format(AsTableManager.Instance.GetTbl_String(4111), m_CompleteLevel, itemName);
						AsNotify.Instance.MessageBox(title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
					}
				}
			}
			
			if(m_CompleteLevel == AsUserInfo.Instance.SavedCharStat.level_)
				m_LevelUpBonusReceived = true;

			if(m_CompleteLevel == Tbl_UserLevel_Table.LastLevelUpRewardLv)
				SetLevelUpBonusBtnActive(false);
		}
		else
		{
			switch(bonus.eResult)
			{
			case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
				string title = AsTableManager.Instance.GetTbl_String(126);
				string content = AsTableManager.Instance.GetTbl_String(115);
				AsNotify.Instance.MessageBox(title, content, AsNotify.MSG_BOX_TYPE.MBT_OK);
				break;
			}
		}
	}
	#endregion
	#region - public -
	public void SceneLoaded()
	{
		if(CheckMapValidity() == true)
		{
			OpenAttendBonus();
			OpenReturnBonus();
		}

		if(AsUserInfo.Instance.SavedCharStat.level_ <= Tbl_UserLevel_Table.FirstLevelUpRewardLv)
			SetLevelUpBonusBtnActive(false);
		else
			SetLevelUpBonusBtnActive(true);
		
		if(m_LevelUpBonusReceived == false)
			m_LevelBonusEffect.animation.Play();

		if(m_CompleteLevel == Tbl_UserLevel_Table.LastLevelUpRewardLv)
			SetLevelUpBonusBtnActive(false);
	}
	
	public void CloseBobusWindow()
	{
		if(AttendBonusWindow.Instance != null)
			AttendBonusWindow.Instance.Destroy();
		if(ReturnBonusWindow.Instance != null)
			ReturnBonusWindow.Instance.Destroy();
	}
	
	public void SetCompleteLevelBonus(int _lv)
	{
		m_CompleteLevel = _lv;
		if(m_CompleteLevel < Tbl_UserLevel_Table.FirstLevelUpRewardLv)
			m_CompleteLevel = Tbl_UserLevel_Table.FirstLevelUpRewardLv;
		
		m_LevelUpBonusReceived = !CheckLevelUpBonusEnale();
	}

	public void OpenLevelBonusWindow()
	{
		if(m_LevelBonusWindow == null)
		{
			GameObject obj = Instantiate(Resources.Load( "UI/AsGUI/GUI_LevelUpReward")) as GameObject;
			m_LevelBonusWindow = obj.GetComponentInChildren<LevelBonusWindow>();
			m_LevelBonusWindow.Init(m_CompleteLevel);
		}
	}
	
	public void PlayerLevelUp(Msg_Level_Up _lvUp)
	{
		if(m_CompleteLevel > Tbl_UserLevel_Table.FirstLevelUpRewardLv)
			m_LevelBonusEffect.gameObject.layer = LayerMask.NameToLayer("GUI");
		
		if(CheckLevelUpBonusEnale() == true)
			m_LevelBonusEffect.animation.Play();

		if(AsUserInfo.Instance.SavedCharStat.level_ > Tbl_UserLevel_Table.FirstLevelUpRewardLv)
			SetLevelUpBonusBtnActive(true);

		if(m_CompleteLevel == Tbl_UserLevel_Table.LastLevelUpRewardLv)
			SetLevelUpBonusBtnActive(false);
	}

	void SetLevelUpBonusBtnActive(bool _active)
	{
		m_LevelBonusBtn.renderer.enabled = _active;
		m_LevelBonusBtn.enabled = _active;
		m_LevelBonusBtn.collider.enabled = _active;

		m_LevelBonusSprite.renderer.enabled = _active;
		m_LevelBonusText.renderer.enabled = _active;
	}

	public bool CheckLevelUpOpened()
	{
		if(m_LevelBonusWindow == null)
			return false;
		else
			return true;
	}

	public void CloseLevelUpWindow()
	{
		if(m_LevelBonusWindow != null)
			m_LevelBonusWindow.Close();
	}
	#endregion
	#region - method -
	bool CheckLevelUpBonusEnale()
	{
		int curPlayerLv = AsUserInfo.Instance.SavedCharStat.level_;
		if(curPlayerLv > m_CompleteLevel)
			return true;
		else
			return false;
	}
	#endregion
}
