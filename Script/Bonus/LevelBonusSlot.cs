using UnityEngine;
using System.Collections;

public class LevelBonusSlot : MonoBehaviour {
	
	#region - member -
	[SerializeField] UIButton btn_Notyet;
	[SerializeField] UIButton btn_Reward;
	[SerializeField] SpriteText txt_Reward;
	[SerializeField] SpriteText txt_Count;
	[SerializeField] SimpleSprite img_Icon;
	[SerializeField] SpriteText txt_Condition;
	[SerializeField] SpriteText txt_Title;
	
	intDlt m_Dlt;
	Tbl_UserLevel_Record m_Rec;
	bool m_Active;
	#endregion
	#region - init & update & release -
	void Awake()
	{
	}
	
	public void Init(Tbl_UserLevel_Record _rec, intDlt _dlt, bool _active)
	{
		Item item = ItemMgr.ItemManagement.GetItem(_rec.Lv_Bonus);
		if(item == null)
		{
			Debug.LogError("LevelBonusSlot::Init: there is no item. index = " + _rec.Lv_Bonus);
			return;
		}
		
		m_Rec = _rec;
		m_Active = _active;
		
		GameObject iconObj = Instantiate(item.GetIcon()) as GameObject;
		iconObj.transform.parent = img_Icon.transform;
		iconObj.transform.localPosition = Vector3.zero;
		iconObj.transform.localScale = Vector3.one;
		
		txt_Count.Text = _rec.Lv_BonusCount.ToString();
		txt_Condition.Text = string.Format(AsTableManager.Instance.GetTbl_String(4107) , _rec.Level);
		txt_Title.Text = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
		
		m_Dlt = _dlt;
	}
	
	// Use this for initialization
	void Start () 
	{
		if(m_Active == true)
		{
			txt_Reward.Text = AsTableManager.Instance.GetTbl_String(4109);
			btn_Reward.SetInputDelegate(Dlt_Click);
		}
		else
		{
			txt_Reward.Text = AsTableManager.Instance.GetTbl_String(4110);
			btn_Reward.renderer.enabled = false;
			btn_Notyet.gameObject.SetActive(true);
		}
	}
	#endregion
	#region - delegate -
	void Dlt_Click(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			m_Dlt(m_Rec.Level);
		}
	}
	#endregion
}
