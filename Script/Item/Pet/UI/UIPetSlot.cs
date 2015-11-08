using UnityEngine;
using System.Collections;
using System.Globalization;

public class UIPetSlot : MonoBehaviour
{
	readonly float clickInterval = 0.5f;
	
//	sITEMVIEW m_ItemView;
	int m_ItemIndex;
	
	float m_PrevClickTime;
	void OnMouseUpAsButton()
	{
		if( Time.time - m_PrevClickTime < clickInterval)
		{
			DoubleClickProc();
			m_PrevClickTime = 0f;
		}
		else
		{
			SingleClickProc();
			m_PrevClickTime = Time.time;
		}
	}
	
	void SingleClickProc()
	{
//		if(m_ItemView != null && m_ItemView.nItemTableIdx != 0)
//			TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.right, m_ItemView.nItemTableIdx);
		if(m_ItemIndex != 0)
			TooltipMgr.Instance.OpenTooltip(TooltipMgr.eOPEN_DLG.right, m_ItemIndex);
	}
	
	void DoubleClickProc()
	{
//		ReleaseSlot();
	}
	
	public void SetSlotInfo( sITEMVIEW _itemView)
	{
//		if( _itemView.nItemTableIdx == 0)
//			ReleaseSlot();
		
//		m_ItemView = _itemView;
		m_ItemIndex = _itemView.nItemTableIdx;
	}

	public void SetSlotInfo( sITEM _sItem)
	{
		m_ItemIndex = _sItem.nItemTableIdx;
	}
	
	public void ReleaseSlot()
	{
//		if( m_ItemView != null)
//		{
//			AsPetManager.Instance.ReleaseEquip();
//		}

		m_ItemIndex = 0;
	}
}

