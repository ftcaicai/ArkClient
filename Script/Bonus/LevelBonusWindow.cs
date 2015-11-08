using UnityEngine;
using System.Collections;

public delegate void intDlt(int _i);

public class LevelBonusWindow : MonoBehaviour {
	
	#region - member -
	readonly int MaxCountInPage = 6;
	
	[SerializeField] GameObject parent;
	
	[SerializeField] UIButton btn_Closing;
	[SerializeField] SpriteText txt_Title;
	[SerializeField] UIScrollList m_listSlot;
	[SerializeField] UIButton btn_Left;
	[SerializeField] UIButton btn_Right;
	[SerializeField] SpriteText txt_Page;
	[SerializeField] SpriteText txt_Content;
	
	int m_CurFinished = 0;
	int m_CurPage = 1;
	#endregion
	#region - init & update & release -
	void Awake()
	{
		#region - set string -
		txt_Title.Text = AsTableManager.Instance.GetTbl_String(4105);
		txt_Content.Text = AsTableManager.Instance.GetTbl_String(4106);
		#endregion
	}
	
	public void Init(int _finished)
	{
		m_CurFinished = _finished;
		if(m_CurFinished < Tbl_UserLevel_Table.FirstLevelUpRewardLv)
			m_CurFinished = Tbl_UserLevel_Table.FirstLevelUpRewardLv;
		
		_InitList(m_CurPage);
	}
	
	void _InitList(int _page)
	{
		m_listSlot.ClearList(true);
		
		int lv = m_CurFinished + MaxCountInPage * (_page - 1);
		eCLASS playerClass = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
		int curPlayerLv = AsUserInfo.Instance.SavedCharStat.level_;
		
		for(int i=0;i<MaxCountInPage; ++i)
		{
			++lv;
			
			Tbl_UserLevel_Record rec = AsTableManager.Instance.GetTbl_Level_Record(playerClass, lv);
			
			if(rec != null)
			{
				if(rec.Lv_Bonus != int.MaxValue)
				{
					GameObject objItemBg = Instantiate( Resources.Load( "UI/Optimization/ListItem/LevelUpRewardListItemContainer")) as GameObject;
					LevelBonusSlot slot = objItemBg.GetComponent<LevelBonusSlot>();
					m_listSlot.AddItem(objItemBg.GetComponent<UIListItemContainer>());
					
					bool active = false;
					if(curPlayerLv > m_CurFinished && m_CurFinished + 1 == lv)
						active = true;
					
					slot.Init(rec, Dlt_SlotClicked, active);
				}
				else
					--i;
			}
			else
				break;
			
			
		}
		
		m_listSlot.ClipItems();
	}
	
	// Use this for initialization
	void Start () {
		#region - set input delegate -
		btn_Closing.SetInputDelegate(Del_Closing);
		btn_Left.SetInputDelegate(Del_Left);
		btn_Right.SetInputDelegate(Del_Right);
		#endregion
		
		RefreshPageText();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion
	#region - delegate -
	void Del_Closing(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Destroy(parent);
		}
	}
	
	void Del_Left(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(m_CurPage > 1)
			{
				--m_CurPage;
				_InitList(m_CurPage);
				
				RefreshPageText();
			}
		}
	}
	
	void Del_Right(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			float maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(69).Value;
			float fMaxPage = (float)(maxLv - m_CurFinished) / (float)MaxCountInPage;
			int maxPage = Mathf.CeilToInt(fMaxPage);
			
			if(m_CurPage < maxPage)
			{
				++m_CurPage;
				_InitList(m_CurPage);
				
				RefreshPageText();
			}
		}
	}
	
	void Dlt_SlotClicked(int _lv)
	{
		BonusManager.Instance.Send_LevelUpBonus(_lv);
	}
	#endregion
	
	#region - method -
	void RefreshPageText()
	{
		float maxLv = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record(69).Value;
		float fMaxPage = (float)(maxLv - m_CurFinished) / (float)MaxCountInPage;
		int maxPage = Mathf.CeilToInt(fMaxPage);
		
		txt_Page.Text = m_CurPage + " / " + maxPage;
	}
	#endregion
	#region - public -
	public void Close()
	{
		Destroy(parent);
	}
	#endregion
}
