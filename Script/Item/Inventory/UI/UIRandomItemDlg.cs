using UnityEngine;
using System.Collections;

public class UIRandomItemDlg : MonoBehaviour 
{
	public UIProgressBar randomItemGaugeBar;
	public SpriteText randomItemText;
	
	private float m_fRandomBarTime = 0.0f;
	private float m_fMaxRandomBarTiem = 1.0f;
	private RealItem m_RealItem;
	
	
	public void Open( RealItem _realItem )
	{						
		m_RealItem = _realItem;
		
		
		Tbl_RandomCoolTime_Record _randomRecord = AsTableManager.Instance.GetTbl_RandomCoolTime_Record( m_RealItem.item.ItemID );
		if( null == _randomRecord || 0.0f >= _randomRecord.ValueTime )
		{
			m_RealItem.SendUseItem();
			m_RealItem = null;
			randomItemGaugeBar.gameObject.SetActiveRecursively(false);
			randomItemText.gameObject.active = false;			
		}
		else
		{
			m_fRandomBarTime = 0.0f;
			m_fMaxRandomBarTiem = _randomRecord.ValueTime;
			randomItemGaugeBar.gameObject.SetActiveRecursively(true);
			randomItemText.gameObject.active = true;
			randomItemGaugeBar.Value = 0f;
			randomItemText.Text = "0%";
		}
	}
	
	public void Close()
	{
		m_RealItem = null;
		randomItemGaugeBar.gameObject.SetActiveRecursively(false);
		randomItemText.gameObject.active = false;			
	}
	
	public bool isOpen
	{
		get
		{
			return randomItemGaugeBar.gameObject.active;
		}
	}
	

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == m_RealItem || false == randomItemGaugeBar.gameObject.active || false == randomItemText.gameObject.active )
			return;	
		
		
		m_fRandomBarTime += Time.deltaTime;
		if( m_fRandomBarTime >= m_fMaxRandomBarTiem || 0.0f >= m_fMaxRandomBarTiem )
		{		
			m_RealItem.SendUseItem();
			m_RealItem = null;
			
			randomItemGaugeBar.gameObject.SetActiveRecursively(false);
			randomItemText.gameObject.active = false;		
		}
		else
		{			
			randomItemGaugeBar.Value = m_fRandomBarTime / m_fMaxRandomBarTiem;	
			int iTemp = (int)(randomItemGaugeBar.Value * 100f);
			randomItemText.Text = iTemp.ToString() + "%";
		}
		
	}
}
