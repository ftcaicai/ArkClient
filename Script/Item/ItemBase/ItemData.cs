using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public enum eItem_Disassemble
{
	NONE,
	NORMAL,
	GACHA,
	COSTUME
}

public class ItemData : AsTableRecord
{
    //---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------	
	public int m_iID = 0;
	public int m_iNameID = 0;
	public int m_iDestID = 0;
	public Item.eITEM_TYPE m_eItemType;
	public int m_iSubType = 0;
	public Item.eGRADE m_eGrade;
	public int m_iBuyAmount;
    public int m_iSellAmount;
	public int m_iRankPoint;
	public Item.eGOODS_TYPE m_GoodsType;
	//public int m_iItem_Probability;
	public eGENDER m_eGender;
	public int m_iLevelLimit;
	public eCLASS m_eNeedClass;
	public int m_iOverlapCount;
	public sbyte m_sbItemTradeLimit = 0;
	public Item.eUSE_TIME_TYPE m_eUseTimeType;
	public int m_iItemUseTime;
//	public bool m_bItem_Onlyone;
	public bool m_bItem_Storage_Limit;
	public bool m_bDropSeal;
	public bool m_bDump;	
	public bool m_bItemStrengthen;	
	public bool m_bShopSell;
	
	public int m_iItem_PatkDmg_Min;
	public int m_iItem_PatkDmg_Max;
	public int m_iItem_MatkDmg_Min;
	public int m_iItem_MatkDmg_Max;	
	public int m_iItem_Pdef;	
	public int m_iItem_Mdef;	
	public int m_iItem_SkilI_ID;
	public int m_iItem_Skill_Level;
    public int m_iItem_Buy_Limit;
	
	public bool m_isItem_OptionType;
	public eITEM_EFFECT m_iItem_Fix1_ID;
	public int m_iItem_Fix1_Value;
	public eITEM_EFFECT m_iItem_Fix2_ID;
	public int m_iItem_Fix2_Value;	
	
	public int m_iItem_Rand_ID;
	public int m_iItem_SetGroup_ID;
	public string m_strIcon;
	public string m_strDropItem;
	public string m_strPartsItem_M;
    public string m_strPartsDiff_M;
	public string m_strPartsItem_W;
    public string m_strPartsDiff_W;	
	public string m_strDropSound;
    //public string m_strRootSound;
	public string m_strUseSound;		
	//public string m_strItemHitSound;	
	//public string m_strItem_Root_Effect;	
	//public string m_strItem_Hit_Effect;	
	
	public int m_Item_Costume_PatkDmg;
	public int m_Item_Costume_MatkDmg;
	public int m_Item_Costume_Pdef;
	public int m_Item_Costume_Mdef;
	
	public eItem_Disassemble m_Item_Disassemble;
	public bool m_Item_MixEnchant;
	public int m_AbsorbExp;
	
	//$yde
	#region - pet -
	public string petClass_ = "NONE";
	#endregion
	
    //---------------------------------------------------------------------
    /* Function */
    //---------------------------------------------------------------------
   
    // Get
	public bool isDisassemble
	{
		get
		{
			return eItem_Disassemble.NORMAL == m_Item_Disassemble || eItem_Disassemble.GACHA == m_Item_Disassemble || eItem_Disassemble.COSTUME == m_Item_Disassemble;
		}
	}

	
	// id
	public int GetID()
    {
        return m_iID;
    }
	
	
	// name 
	public int nameId
	{
		get
		{
			return m_iNameID;
		}
	}
	
	  
	
	
	// dest
	public int destId
	{
		get
		{
			return m_iDestID;
		}
	}	

	// item type
	public Item.eITEM_TYPE GetItemType()
    {
        return m_eItemType;
    }
	
	
	// item sub type
	public int GetSubType()
    {
        return m_iSubType;
    }
	
	
	// grade 	
	public Item.eGRADE grade
	{
		get
		{
			return m_eGrade;
		}
	}
	
	public bool isItem_OptionType	
	{
		get
		{
			return 	m_isItem_OptionType;
		}
	}
	
	
	public eITEM_EFFECT getItem_Fix1_ID	
	{
		get
		{
			return 	m_iItem_Fix1_ID;
		}
	}
	
	public int getItem_Fix1_Value	
	{
		get
		{
			return 	m_iItem_Fix1_Value;
		}
	}
	
	
	public eITEM_EFFECT getItem_Fix2_ID	
	{
		get
		{
			return 	m_iItem_Fix2_ID;
		}
	}
	
	public int getItem_Fix2_Value	
	{
		get
		{
			return 	m_iItem_Fix2_Value;
		}
	}
	
	
	public Color GetGradeColor()
    {
        switch (m_eGrade)
        {
            case Item.eGRADE.Normal:
                return ItemMgr.Instance.colorItemGradeNormal;

            case Item.eGRADE.Magic:
                return ItemMgr.Instance.colorItemGradeMagic;

            case Item.eGRADE.Rare:
                return ItemMgr.Instance.colorItemGradeRare;

            case Item.eGRADE.Epic:
                return ItemMgr.Instance.colorItemGradeEpic;
			
			case Item.eGRADE.Ark:
				return ItemMgr.Instance.colorItemGradeArk;

            //case Item.eGRADE.Set:
             //   return ItemMgr.Instance.colorItemGradeSet;
        }

        Debug.LogError("TooltipMgr::GetGradeColor()[ eGRADE : " + m_eGrade);
        return Color.black;
    }  
	
	
	// buy amount
	public int buyAmount
	{
		get
		{
			return m_iBuyAmount;
		}
	}
	
	// sell amount
	public int sellAmount
	{
		get
		{
			return m_iSellAmount;
		}
	}
	
	// level limit
	public int levelLimit
	{
		get
		{
			return m_iLevelLimit;
		}
	}
	
	// need job
	public eCLASS needClass
	{
		get
		{
			return m_eNeedClass;
		}
	}
	
	// overlap count
	public int overlapCount
	{
		get
		{
			return m_iOverlapCount;
		}
	}
	
	// item trade limit
	public bool isTradeLimit
	{
		get
		{
			return 0 == m_sbItemTradeLimit;
		}
	}
	
	// item use time
	public int useItemTime
	{
		get
		{
			return m_iItemUseTime;
		}
	}
	
	public string GetIcon()
    {
        return m_strIcon;
    }
	
	public string GetDropItem()
    {
        return m_strDropItem;
    }

    public string GetPartsItem_W()
    {
        return m_strPartsItem_W;
    }
	
	public string GetPartsItemDiff_W()
    {
        return m_strPartsDiff_W;
    }
	
	public string GetPartsItem_M()
    {
        return m_strPartsItem_M;
    }
	
	public string GetPartsItemDiff_M()
    {
        return m_strPartsDiff_M;
    }
	
	public int itemSkill
	{
		get
		{
			return m_iItem_SkilI_ID;
		}
	}
	
	public int itemSkillLevel
	{
		get
		{
			return m_iItem_Skill_Level;
		}
	}
	
	public int parkDmgMin
	{
		get
		{
			return m_iItem_PatkDmg_Min;			
		}
	}
	public int parkDmgMax
	{
		get
		{
			return m_iItem_PatkDmg_Max;			
		}
	}
	public int matkDmgMin
	{
		get
		{
			return m_iItem_MatkDmg_Min;			
		}
	}
	public int matkDmgMax
	{
		get
		{
			return m_iItem_MatkDmg_Max;			
		}
	}
	
	public int pDef
	{
		get
		{
			return m_iItem_Pdef;
		}
	}
	
	public int mDef
	{
		get
		{
			return m_iItem_Mdef;
		}
	}
		

	public bool isDump
	{
		get
		{
			return m_bDump;
		}
	}
	
	public eGENDER getGender
	{
		get
		{
			return m_eGender;
		}
	}

	public int getSetGroupID
	{
		get
		{
			return m_iItem_SetGroup_ID;
		}
	}
	
	public string getStrDropSound	
	{
		get
		{
			return m_strDropSound;
		}
	}
	
	public bool isItemStrengthen
	{
		get
		{
			return m_bItemStrengthen;
		}
	}
	
	public bool isShopSell
	{
		get
		{
			return m_bShopSell;
		}
	}
	
	public Item.eGOODS_TYPE getGoodsType
	{
		get
		{			
			return m_GoodsType;	
		}
	}
   // Read 

    public ItemData(XmlElement _element)
    {
        try
        {			
			XmlNode node = (XmlElement)_element;		
		
			SetValue( ref m_iID, node, "Item_ID" );
			SetValue( ref m_iNameID, node, "Item_Name_ID" );
            SetValue( ref m_iDestID, node, "Item_Description_ID" );
            SetValue < Item.eITEM_TYPE>(ref m_eItemType, node, "Item_Type");
			
            if( m_eItemType == Item.eITEM_TYPE.EquipItem || m_eItemType == Item.eITEM_TYPE.CosEquipItem )
            {
                Item.eEQUIP temp = new Item.eEQUIP();
                SetValue<Item.eEQUIP>(ref temp, node, "Item_Kind");
                m_iSubType = (int)temp;
            }            
            else if( m_eItemType == Item.eITEM_TYPE.EtcItem )
            {
				Item.eEtcItem temp = new Item.eEtcItem();
                SetValue<Item.eEtcItem>(ref temp, node, "Item_Kind");
                m_iSubType = (int)temp;
            }
			else if( m_eItemType == Item.eITEM_TYPE.UseItem )
			{
				Item.eUSE_ITEM temp = new Item.eUSE_ITEM();
                SetValue<Item.eUSE_ITEM>(ref temp, node, "Item_Kind");
                m_iSubType = (int)temp;
			}
			else if( m_eItemType == Item.eITEM_TYPE.ActionItem )
			{
				
			}
			else
			{
				Debug.LogError("error ItemSubType");
			}
			
			
			SetValue<Item.eGOODS_TYPE>( ref m_GoodsType, node, "Item_GoodsType" );
            SetValue < Item.eGRADE>(ref m_eGrade, node, "Item_Grade");                   
            SetValue(ref m_iBuyAmount, node, "Item_BuyAmount");
            SetValue(ref m_iSellAmount, node, "Item_SellAmount");
			SetValue( ref m_iRankPoint, node, "Item_RankPoint");
			//SetValue(ref m_iItem_Probability, node, "Item_Probability");
			SetValue(ref m_eGender, node, "Item_Gender");			
            SetValue(ref m_iLevelLimit, node, "Item_Level_Limit");
			
			#region - eCLASS parsing within PET -
			if( CheckPetItem() == true)
			{
				SetValue (ref petClass_, node, "Item_Need_Class");
				m_eNeedClass = eCLASS.PET;
			}
			else
				SetValue <eCLASS>(ref m_eNeedClass, node, "Item_Need_Class");
			#endregion
            
            SetValue(ref m_iOverlapCount, node, "Item_Overlap_Count");
			SetValue(ref m_sbItemTradeLimit, node, "Item_Trade_Limit");
			SetValue<Item.eUSE_TIME_TYPE>(ref m_eUseTimeType, node, "Item_UseTimeType" );
			SetValue(ref m_iItemUseTime, node, "Item_UseTime");			
//			SetValue(ref m_bItem_Onlyone, node, "Item_Onlyone");		
			SetValue(ref m_bItem_Storage_Limit, node, "Item_Storage_Limit");
			SetValue(ref m_bDropSeal, node, "Item_DropSeal");
			SetValue(ref m_bDump, node, "Item_Dump");				
			SetValue(ref m_bItemStrengthen, node, "Item_Strengthen");	
			SetValue(ref m_bShopSell, node, "Item_ShopSell");				
			SetValue(ref m_iItem_PatkDmg_Min, node, "Item_PatkDmg_Min");
			SetValue(ref m_iItem_PatkDmg_Max, node, "Item_PatkDmg_Max");
			SetValue(ref m_iItem_MatkDmg_Min, node, "Item_MatkDmg_Min");
			SetValue(ref m_iItem_MatkDmg_Max, node, "Item_MatkDmg_Max");
			SetValue(ref m_iItem_Pdef, node, "Item_Pdef");			
			SetValue(ref m_iItem_Mdef, node, "Item_Mdef");
			SetValue(ref m_iItem_SkilI_ID, node, "Item_SkilI_ID");
			SetValue(ref m_iItem_Skill_Level, node, "Item_Skill_Level");			
			
			SetValue(ref m_isItem_OptionType, node, "Item_OptionType");
			SetValue<eITEM_EFFECT>(ref m_iItem_Fix1_ID, node, "Item_Fix1_ID");
			SetValue(ref m_iItem_Fix1_Value, node, "Item_Fix1_Value");
			SetValue<eITEM_EFFECT>(ref m_iItem_Fix2_ID, node, "Item_Fix2_ID");
			SetValue(ref m_iItem_Fix2_Value, node, "Item_Fix2_Value");			
			
			SetValue(ref m_iItem_Rand_ID, node, "Item_Rand_ID");			
			SetValue(ref m_iItem_SetGroup_ID, node, "Item_SetGroup_ID");	
            SetValue(ref m_strIcon, node, "Item_Icon");
			SetValue(ref m_strDropItem, node, "Item_Drop_Mesh");
            SetValue(ref m_iItem_Buy_Limit, node, "Item_Buy_Limit");
			SetValue(ref m_strPartsItem_M, node, "Item_PartItem_M");
            SetValue(ref m_strPartsDiff_M, node, "Item_PartDiff_M");			
			SetValue(ref m_strPartsItem_W, node, "Item_PartItem_W");
            SetValue(ref m_strPartsDiff_W, node, "Item_PartDiff_W");            
			
			SetValue(ref m_strDropSound, node, "Item_Drop_Sound");
            //SetValue(ref m_strRootSound, node, "Item_Root_Sound");
            SetValue(ref m_strUseSound, node, "Item_Use_Sound");
			//SetValue(ref m_strItemHitSound, node, "Item_Hit_Sound");			
			//SetValue(ref m_strItem_Root_Effect, node, "Item_Root_Effect");
			//SetValue(ref m_strItem_Hit_Effect, node, "Item_Hit_Effect");	
			
			
			SetValue(ref m_Item_Costume_PatkDmg, node, "Item_Costume_PatkDmg");
			SetValue(ref m_Item_Costume_MatkDmg, node, "Item_Costume_MatkDmg");
			SetValue(ref m_Item_Costume_Pdef, node, "Item_Costume_Pdef");
			SetValue(ref m_Item_Costume_Mdef, node, "Item_Costume_Mdef");
			
			
			SetValue<eItem_Disassemble>(ref m_Item_Disassemble, node, "Item_Disassemble");
			SetValue(ref m_Item_MixEnchant, node, "Item_MixEnchant");
			SetValue(ref m_AbsorbExp, node, "AbsorbEXP");
        } 
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }  
	
	public ItemData(BinaryReader br)
	{
		m_iID = br.ReadInt32();
		m_iNameID = br.ReadInt32();
		m_iDestID = br.ReadInt32();
		m_eItemType = (Item.eITEM_TYPE)br.ReadInt32();
		m_iSubType = br.ReadInt32();
		m_GoodsType = (Item.eGOODS_TYPE)br.ReadInt32();
		m_eGrade = (Item.eGRADE)br.ReadInt32();
		m_iBuyAmount = br.ReadInt32();
		m_iSellAmount = br.ReadInt32();
		m_iRankPoint = br.ReadInt32();
		m_eGender = (eGENDER)br.ReadInt32();
		m_iLevelLimit = br.ReadInt32();
		
		#region - eCLASS parsing within PET -
		if( CheckPetItem() == true)
		{
			petClass_ = br.ReadString();
			m_eNeedClass = eCLASS.PET;
		}
		else
			m_eNeedClass = (eCLASS)br.ReadInt32();
		#endregion
		
		m_iOverlapCount = br.ReadInt32();
		m_sbItemTradeLimit = br.ReadSByte();
		m_eUseTimeType = (Item.eUSE_TIME_TYPE)br.ReadInt32();
		m_iItemUseTime = br.ReadInt32();
//		m_bItem_Onlyone = br.ReadBoolean();
		m_bItem_Storage_Limit = br.ReadBoolean();
		m_bDropSeal = br.ReadBoolean();
		m_bDump = br.ReadBoolean();
		m_bItemStrengthen = br.ReadBoolean();
		m_bShopSell = br.ReadBoolean();
		m_iItem_PatkDmg_Min = br.ReadInt32();
		m_iItem_PatkDmg_Max = br.ReadInt32();
		m_iItem_MatkDmg_Min = br.ReadInt32();
		m_iItem_MatkDmg_Max = br.ReadInt32();
		m_iItem_Pdef = br.ReadInt32();
		m_iItem_Mdef = br.ReadInt32();
		m_iItem_SkilI_ID = br.ReadInt32();
		m_iItem_Skill_Level = br.ReadInt32();
		m_isItem_OptionType = br.ReadBoolean();
		m_iItem_Fix1_ID = (eITEM_EFFECT)br.ReadInt32();
		m_iItem_Fix1_Value = br.ReadInt32();
		m_iItem_Fix2_ID = (eITEM_EFFECT)br.ReadInt32();
		m_iItem_Fix2_Value = br.ReadInt32();
		m_iItem_Rand_ID = br.ReadInt32();
		m_iItem_SetGroup_ID = br.ReadInt32();
		m_strIcon = br.ReadString();
		m_strDropItem = br.ReadString();
		m_iItem_Buy_Limit = br.ReadInt32();
		m_strPartsItem_M = br.ReadString();
		m_strPartsItem_W = br.ReadString();
		m_strPartsDiff_M = br.ReadString();
		m_strPartsDiff_W = br.ReadString();
		m_strDropSound = br.ReadString();
		//m_strRootSound = br.ReadString();//kij
		m_strUseSound = br.ReadString();
		//m_strItemHitSound = br.ReadString();//kij
		//m_strItem_Root_Effect = br.ReadString();//kij
		//m_strItem_Hit_Effect = br.ReadString();//kij
		
		m_Item_Costume_PatkDmg = br.ReadInt32();
		m_Item_Costume_MatkDmg = br.ReadInt32();
		m_Item_Costume_Pdef = br.ReadInt32();
		m_Item_Costume_Mdef = br.ReadInt32();
		
		
		m_Item_Disassemble = (eItem_Disassemble)br.ReadInt32();
		m_Item_MixEnchant = br.ReadBoolean();
		m_AbsorbExp = br.ReadInt32();
	}
	
	public bool CheckPStoreOpenItem()//$yde
	{
		if( GetItemType() == Item.eITEM_TYPE.UseItem &&
			(GetSubType() == ( int)Item.eUSE_ITEM.PrivateStore1 ||
			GetSubType() == ( int)Item.eUSE_ITEM.PrivateStore2 ||
			GetSubType() == ( int)Item.eUSE_ITEM.PrivateStore3 ||
			GetSubType() == ( int)Item.eUSE_ITEM.PrivateStore4 ||
			GetSubType() == ( int)Item.eUSE_ITEM.PrivateStore5))
		{
			return true;
		}
		else
			return false;
	}
	
	public bool CheckPetItem()
	{
		if( ( (Item.eITEM_TYPE)m_eItemType == Item.eITEM_TYPE.EquipItem && (Item.eEQUIP)m_iSubType == Item.eEQUIP.Pet))
			return true;
		
		if( ( (Item.eITEM_TYPE)m_eItemType == Item.eITEM_TYPE.UseItem &&
			(Item.eUSE_ITEM)m_iSubType == Item.eUSE_ITEM.PetEgg ||
			(Item.eUSE_ITEM)m_iSubType == Item.eUSE_ITEM.PetFood))
			return true;
		
		return false;
	}
}
