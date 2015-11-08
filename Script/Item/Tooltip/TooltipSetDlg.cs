using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipSetDlg : TooltipDlg 
{
	public SpriteText title;
	public SpriteText[] setItems;
	public SpriteText[] setAbilitys;
	private int m_iSetItemCount = 0;
	
	private void Clear()
	{
		title.Text = "";
		foreach( SpriteText text in setItems )
		{
			text.Text = "";
		}
		
		foreach( SpriteText text in setAbilitys )
		{
			text.Text = "";
		}
		
	}	
	
	public void Open( Item _item )
	{
		if(false ==SetItem(_item))
			return;
		
		Clear();
		
		Tbl_SetItem_Record setRecord = AsTableManager.Instance.GetTbl_SetItem_Record( getItem.ItemData.getSetGroupID );
		if( null == setRecord )
		{
            Debug.LogError("TooltipSetDlg::SetItem() [ null == setRecord ] id : " + getItem.ItemData.getSetGroupID);
			return;
		}
		
		SetTitleName( setRecord.getNameID );
		SetItems( setRecord );
		SetAbilityText( setRecord );
	}
	
	
	private void SetTitleName( int iStringId )
	{
		if( int.MaxValue == iStringId )
		{
			title.Text = string.Empty;
			return;
		}
		
		Tbl_String_Record record = AsTableManager.Instance.GetTbl_String_Record( iStringId );
		if( null == record )
		{
			Debug.LogError("TooltipSetDlg::SetTitleName() [ null == string record ] id : " + iStringId );
			return;
		}
		title.Text = record.String;
	}
	
	private void SetItems( Tbl_SetItem_Record setRecord )
	{		
		m_iSetItemCount = 0;
		
		for( int i=0; i<setItems.Length; ++i )
		{
			int iItemNameID = setRecord.GetItemNameID( i );
			if( iItemNameID == -1 || int.MaxValue == iItemNameID )
			{
				setItems[i].Text = string.Empty;
				continue;	
			}
			
			string strText = AsTableManager.Instance.GetTbl_String( iItemNameID );
			
			
			if( setRecord.IsHaveItemID(i) )
			{
				++m_iSetItemCount;
				setItems[i].Text = ItemMgr.Instance.colorHasSetItem.ToString() + strText;
			}
			else
			{
				setItems[i].Text = strText;
			}		
		}
	}
	
	private void SetAbilityText( Tbl_SetItem_Record setRecord )
	{
		string strRecord = AsTableManager.Instance.GetTbl_String(1061);
		
		
		for( int i=0; i<setAbilitys.Length; ++i )
		{
			Tbl_SetItem_Record.CSetApply data = setRecord.GetSetApply( i );
			if( null == data )
				continue;
					
			
			if( data.iApply <= m_iSetItemCount )			
				setAbilitys[i].Text = ItemMgr.Instance.colorCompleteSetItem.ToString() + data.iApply.ToString() + strRecord + 
					AsTableManager.Instance.GetTbl_String(data.iStringID);
			else
				setAbilitys[i].Text = data.iApply.ToString() + strRecord + AsTableManager.Instance.GetTbl_String(data.iStringID);
		}
	}
	
}
