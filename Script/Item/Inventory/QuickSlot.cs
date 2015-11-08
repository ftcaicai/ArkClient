using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class QuickSlot
{
	private sQUICKSLOT[] m_QuickSlotList;
	private bool m_bNeedUiUpdate = false;

	public sQUICKSLOT[] getQuickSlots
	{
		get	{ return m_QuickSlotList; }
	}

	public void SetQuickSlotList( sQUICKSLOT[] slotList)
	{
		if( null == slotList)
		{
			Debug.LogError( "QuickSlot::SetQuickSlotList() [ null==sQUICKSLOT ] ");
			return;
		}

		m_QuickSlotList = slotList;
		m_bNeedUiUpdate = true;
	}

	public void SetQuickSlot( int iIndex, sQUICKSLOT data)
	{
		if( null == m_QuickSlotList)
		{
			Debug.LogError( "QuickSlotContain::SetQuickSlot() [ null==m_QuickSlotList ] ");
			return;
		}

		if( m_QuickSlotList.Length <= iIndex)
		{
			Debug.LogError( "QuickSlotContain::SetQuickSlot() [ m_QuickSlotList.Length <= iIndex ] iIndex :  " + iIndex);
			return;
		}

		m_QuickSlotList[iIndex] = data;

		ResetSlot();
	}

	public void SetQuickSlot( int iIndex, int iValue, eQUICKSLOT_TYPE eType)
	{
		sQUICKSLOT _data = new sQUICKSLOT();
		_data.nValue = iValue;
		_data.eType = (int)eType;
		SetQuickSlot( iIndex, _data);
		
		#region - auto combat -
		AutoCombatManager.Instance.ItemRemovedFromQuickSlot( iIndex, iValue, eType);
		#endregion
	}

	public void ResetSlot()
	{
		if( null == m_QuickSlotList || null == AsQuickSlotManager.Instance)
			return;

		AsQuickSlotManager.Instance.SetSlots( m_QuickSlotList);
	}

	public bool IsHadSlotPage( int iPage)
	{
		for( int i = iPage * 8; i < ( iPage + 1) * 8; ++i)
		{
			if( m_QuickSlotList.Length <= i)
				return false;

			if( 0 != m_QuickSlotList[i].nValue)
			{
				if( eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_ITEM == (eQUICKSLOT_TYPE)m_QuickSlotList[i].eType)
				{
					if( 0 < ItemMgr.HadItemManagement.Inven.GetItemTotalCount( m_QuickSlotList[i].nValue))
						return true;
					else
						continue;
				}
				return true;
			}
		}

		return false;
	}

	public bool IsHadSkillIndex( int iSkill)
	{
		for( int i = 0; i < m_QuickSlotList.Length; ++i)
		{
			sQUICKSLOT _quickSlot = m_QuickSlotList[i];

			if( null == _quickSlot)
				continue;

			if( _quickSlot.nValue == iSkill)
				return true;
		}

		return false;
	}

	public void SetLearnSkill( int iSkill, int iSkillLevel)
	{
		if( true == IsHadSkillIndex( iSkill))
		{
			ResetSlot();
			return;
		}

		if( true == SetReanSkill( iSkill, iSkillLevel, 4, 8))
			return;
		if( true == SetReanSkill( iSkill, iSkillLevel, 0, 4))
			return;
		if( true == SetReanSkill( iSkill, iSkillLevel, 12, 16))
			return;
		if( true == SetReanSkill( iSkill, iSkillLevel, 8, 12))
			return;
		if( true == SetReanSkill( iSkill, iSkillLevel, 20, 24))
			return;
		if( true == SetReanSkill( iSkill, iSkillLevel, 16, 20))
			return;
	}

	private bool SetReanSkill( int iSkill, int iSkillLevel, int startIdx, int endIdx)
	{
		for( int i = startIdx; i < endIdx; ++i)
		{
			sQUICKSLOT _quickSlot = m_QuickSlotList[i];

			if( null == _quickSlot)
				continue;

			if( ( 0 == _quickSlot.nValue) && ( eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_NOTHING == (eQUICKSLOT_TYPE)_quickSlot.nValue))
			{
				_quickSlot.nValue = iSkill;
				_quickSlot.eType = (int)eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL;
				AsCommonSender.SendQuickslotChange( (short)i, iSkill, eQUICKSLOT_TYPE.eQUICKSLOT_TYPE_SKILL);
				ResetSlot();
				return true;
			}
		}

		return false;
	}

	public void ApplyAsSlot()
	{
		if( false == m_bNeedUiUpdate)
			return;

		ResetSlot();
		m_bNeedUiUpdate = false;
	}
}
