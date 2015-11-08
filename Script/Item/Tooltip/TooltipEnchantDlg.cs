using UnityEngine;
using System.Collections;

public class TooltipEnchantDlg : TooltipDlg 
{
	//public GameObject 
    public SpriteText[] enchantTexts;
    public SimpleSprite[] enchantIcons;

    private System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();

    private void Clear()
    {      
        foreach (SpriteText text in enchantTexts)
        {
            text.Text = "";
        }

        foreach (SimpleSprite _sprite in enchantIcons)
        {
            _sprite.gameObject.SetActive( false );
        }
    }
	
  	public void Open( sITEM _sitem )
	{
		if (null == _sitem)
        {
            Debug.LogError("TooltipEnchantDlg::SetSItem() [ null == _sitem ]");
            return;
        }
				
		Item _item = ItemMgr.ItemManagement.GetItem( _sitem.nItemTableIdx );
		
		if( null == _item )
		{
			Debug.LogError("TooltipEnchantDlg::SetSItem() [ null == _item ] item id : " + _sitem.nItemTableIdx );
            return;
		}
		
		if( false == SetItem(_item) )
            return;
		
		if( Item.eITEM_TYPE.EquipItem != _item.ItemData.GetItemType() && 
			Item.eITEM_TYPE.CosEquipItem != _item.ItemData.GetItemType() )
		{
			Debug.LogError("TooltipEnchantDlg::Open()[ no equip ]" );
		}
		
		
		Clear();
       
		
		if( 2 > _sitem.nEnchantInfo.Length )
		{
			Debug.LogError("TooltipEnchantDlg::Open()[2 > _sitem.nEnchantInfo.Length ]" +  _sitem.nEnchantInfo.Length );
			return;
		}
		
		bool isStrengthenGray = _sitem.nStrengthenCount < ItemMgr.Instance.GetEnchantStrengthenCount();
		
		if( _item.ItemData.GetItemType() == Item.eITEM_TYPE.CosEquipItem )
		{
			isStrengthenGray = false;
		}
		
		if( _sitem.nEnchantInfo[0] == -1 && _sitem.nEnchantInfo[1] != -1 )
		{
			SetEnchantIcon(0, _sitem.nEnchantInfo[1], isStrengthenGray );
			SetEnchantText(0, _sitem.nEnchantInfo[1], isStrengthenGray );
			enchantIcons[0].gameObject.SetActive( false );
			
			enchantIcons[1].gameObject.SetActive(true);
		}
		else if( _sitem.nEnchantInfo[0] != -1 && _sitem.nEnchantInfo[1] != -1 )
		{
			SetEnchantIcon(0, _sitem.nEnchantInfo[0], false );
			SetEnchantText(0, _sitem.nEnchantInfo[0], false );
			enchantIcons[0].gameObject.SetActive(false);
			
			SetEnchantIcon(1, _sitem.nEnchantInfo[1], isStrengthenGray );
			SetEnchantText(1, _sitem.nEnchantInfo[1], isStrengthenGray );
			enchantIcons[1].gameObject.SetActive(false);
		}
		else if( _sitem.nEnchantInfo[0] != -1 && _sitem.nEnchantInfo[1] == -1 )
		{
			SetEnchantIcon(0, _sitem.nEnchantInfo[0], false );
			SetEnchantText(0, _sitem.nEnchantInfo[0], false );
			enchantIcons[0].gameObject.SetActive(false);
			enchantIcons[1].gameObject.SetActive(true);
		}	
	}
	
	

    private void SetEnchantIcon(int iIndex, int iEnchantItemId, bool isStrengthEnable )
    {
        if ( -1 == iEnchantItemId)
            return;

        if (iIndex >= enchantIcons.Length)
        {
            Debug.LogError("iIndex >= enchantIcons.Length [ index : " + iIndex);
            return;
        }	

        enchantIcons[iIndex].gameObject.SetActive(true);

        if (0 == iEnchantItemId)
            return;	
		

        Item _item = ItemMgr.ItemManagement.GetItem(iEnchantItemId);
        if (null == _item)
        {
            Debug.LogError("TooltipEnchantDlg::SetEnchantIcon[null == _item] id :" + iEnchantItemId);
            return;
        }

        GameObject goTemp = GameObject.Instantiate(_item.GetIcon()) as GameObject;
        UISlotItem _slotitem = goTemp.GetComponent<UISlotItem>();
        if (null == _slotitem)
        {
            Destroy(goTemp);
            Debug.LogError("TooltipEnchantDlg::SetEnchantIcon[null == UISlotItem] id :" + iEnchantItemId);
            return;
        }
        _slotitem.transform.parent = gameObject.transform;// enchantIcons[iIndex].transform;
        _slotitem.transform.localPosition = new Vector3( enchantIcons[iIndex].transform.localPosition.x, enchantIcons[iIndex].transform.localPosition.y, -1.0f );
        _slotitem.transform.localScale = Vector3.one;
        _slotitem.transform.localRotation = Quaternion.identity;
		
		if( null == _slotitem.iconImg )
			AsUtil.ShutDown( "TooltipEnchantDlg::SetEnchantIcon : null == _slotitem.iconImg");

        _slotitem.iconImg.SetSize(enchantIcons[iIndex].width, enchantIcons[iIndex].height);      
		
		if( isStrengthEnable )
		{
			Color color = Color.blue;
			color.a = 0.4f;
			_slotitem.iconImg.gameObject.renderer.material.SetColor("_Color", color );
		}
    }

    private void SetEnchantText(int iIndex, int iEnchantItemId, bool isStrengthEnable)
    {
        if (0 == iEnchantItemId || -1 == iEnchantItemId)
            return;

        if (iIndex >= enchantTexts.Length)
        {
            Debug.LogError("iIndex >= enchantTexts.Length [ index : " + iIndex);
            return;
        }

        Item _item = ItemMgr.ItemManagement.GetItem(iEnchantItemId);
        if (null == _item)
        {
            Debug.LogError("TooltipEnchantDlg::SetEnchantText[null == _item] id :" + iEnchantItemId);
            return;
        }
		
		m_sbTemp.Length = 0;
		if (isStrengthEnable)
        {            
            m_sbTemp.Append(Color.gray);          
        }		
		
		if( true == EnchantDlg.IsSkillEnchant( _item ) )
		{		
			Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( _item.ItemData.itemSkill );		
			if( null == record )
			{
				Debug.LogError("TooltipEnchantDlg::SetEnchantText() [ null == Tbl_Skill_Record ] skill id : " + _item.ItemData.itemSkill );
				return;
			}
			
			Tbl_SoulStoneEnchant_Record _record = AsTableManager.Instance.GetSoulStoneEnchantTable( _item.ItemID );
			if( null == _record )
			{
				Debug.LogError("TooltipEnchantDlg::SetEnchantText() [ null == Tbl_SoulStoneEnchant_Record ] id : " + _item.ItemID );
				return;
			}
			
			
			string szDesc = AsTableManager.Instance.GetTbl_String( record.Description_Index);
			szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, _item.ItemData.itemSkill, _item.ItemData.itemSkillLevel, 0);
			
			m_sbTemp.AppendFormat(AsTableManager.Instance.GetTbl_String(_item.ItemData.destId), (float)_record.getValue*0.1f);
			m_sbTemp.Append(szDesc);			
		}
		else
		{
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(_item.ItemData.destId) );
		}
       
       enchantTexts[iIndex].Text = m_sbTemp.ToString();       
    }
}
