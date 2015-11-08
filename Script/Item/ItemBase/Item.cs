using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class Item 
{
    //---------------------------------------------------------------------
    /* enum */
    //---------------------------------------------------------------------

    // eITEM_TYPE
	public enum eITEM_TYPE
	{
        None      = 0,
        EquipItem = 1,
        CosEquipItem,
        //SkillItem,        
        EtcItem, 
		UseItem,
		ActionItem,
	};
	
	
	public const int equipMaxCount = 10;	
	
	// eRealItem
    public enum eEQUIP
    {
		// cur
//        Weapon = 1,
//        Head = 2,
//        Armor = 3,
//        Gloves = 4,
//        Point = 5,
//        Earring = 6,
//        Necklace = 7,
//        Ring = 8,
//
//		Wing = 9, //$yde
//		Fairy = 10, //$yde
//        SubAcc = 11, //$yde
//		Pet = 12,//$yde
//
//		MAX = 13, //$yde
//
//		Hair = 14, //$yde - exceptional case
//		Face = 15, //$yde - exceptional case

		// prev
		Weapon = 1,
		Head = 2,
		Armor = 3,
		Gloves = 4,
		Point = 5,
		Earring = 6,
		Necklace = 7,
		Ring = 8,
		SubAcc = 9, 
		Hair,
		Face,   
		
		Fairy,
		Wing,
		
		Pet,//$yde
    };	
	
	
	
	// eEtcItem
    public enum eEtcItem
    {
        Enchant = 1,
        Strengthen,  
		Material,
		Etc,
		//Cash,
		Gold,
		Quest,
		Vip,
    };
	
	
	//eGRADE
	public enum eGRADE
	{	
		Normal,	
        Magic,
        Rare,
        Epic,    
		Ark,
    }
	
	
	 // NEED JOB
    public enum eNEED_JOB
    {
        DivineKnight = 1,
		Magician,
		Hunter,
		Healler,
		Assassin,
		Common = 99,
    }
	
	public enum eGENDER
	{
		NONE = 0, // man and women
		Men,
		Women
	}
 	
	public enum eGOODS_TYPE
	{
		Gold,
		Cash,
		Point,
		Gacha,
	}
	
	public enum eUSE_ITEM
	{
		SkillReset,
		Random,
		PrivateStore1,
		PrivateStore2,
		PrivateStore3,
		//
		PrivateStore4,
		PrivateStore5,
        Gold,
        Miracle,
		ConsumeQuest,
		//
		InfiniteQuest,
		GetQuest,
		Summon,
		ConsumeHair,
		ChatChannel,
		//
		ChatServer,
		QuestRandom,
		CharacterNameReset,
		GuildNameReset,
		PetEgg,
		//
		PetFood,
		Event,
		//
		ImageGet,
	}
	
	public enum eUSE_TIME_TYPE
	{
		NONE,
		Finish
	}
	
	//---------------------------------------------------------------------
	/* Variable*/
	//---------------------------------------------------------------------
	
	private ItemData m_ItemData = null;
#if USE_ITEM_RES
	private ItemRes m_ItemRes = null;
#endif

    
    //---------------------------------------------------------------------
    /* static function */
    //---------------------------------------------------------------------
	static System.Text.StringBuilder s_strTemp = new System.Text.StringBuilder();
  


	//---------------------------------------------------------------------
	/* functon */
	//---------------------------------------------------------------------

    
    public Item(XmlElement _element)
    {
        m_ItemData = new ItemData(_element);
#if USE_ITEM_RES
        m_ItemRes = new ItemRes();
#endif
    }

    public Item(BinaryReader br)
	{
        m_ItemData = new ItemData( br);
#if USE_ITEM_RES
        m_ItemRes = new ItemRes();
#endif
	}


	// Get

    public ItemData ItemData
    {
        get
        {
            return m_ItemData;
        }
    }
#if USE_ITEM_RES
    protected ItemRes ItemRes
    {
        get
        {
            return m_ItemRes;
        }
    }
#endif
    public int ItemID
    {
        get
        {
            return ItemData.GetID();
        }
    }

#if USE_ITEM_RES
    public GameObject GetGameObject(ItemRes.eRES_GO eType)
    {
        string strPath = GetGoResPath(eType);
        if (null == strPath)
            return null;

        GameObject go = ItemRes.GetGameObject(eType);
        if (null == go)
        {
            if (false == ItemRes.LoadGameObject(eType, strPath))
                return null;
        }

        return ItemRes.GetGameObject(eType);
    }

    public Texture GetTexture(ItemRes.eRES_TEX eType)
    {
        string strPath = GetTexResPath(eType);
        if (null == strPath)
            return null;

        Texture tex = ItemRes.GetTexture(eType);
        if (null == tex)
        {
            if (false == ItemRes.LoadTexture(eType, strPath))
                return null;
        }

        return ItemRes.GetTexture(eType);
    }

    public GameObject GetDropItem()
    {
        return GetGameObject(ItemRes.eRES_GO.DROP);
    }

    public GameObject GetIcon()
    {
        return GetGameObject(ItemRes.eRES_GO.ICON);
    }

    public GameObject GetPartsItem_W()
    {
        return GetGameObject(ItemRes.eRES_GO.PARTS_W);
    }
	
	public GameObject GetPartsItem_M()
    {
        return GetGameObject(ItemRes.eRES_GO.PARTS_M);
    }

    public Texture GetPartsDiff_W()
    {
        return GetTexture(ItemRes.eRES_TEX.DIFF_W);
    }
	
	public Texture GetPartsDiff_M()
    {
        return GetTexture(ItemRes.eRES_TEX.DIFF_M);
    }

    public Texture GetPartsSpec()
    {
        return GetTexture(ItemRes.eRES_TEX.SPEC);
    }

       
    public string GetTexResPath(ItemRes.eRES_TEX eType)
    {
        switch (eType)
        {
        case ItemRes.eRES_TEX.DIFF_W:			
			return ItemData.GetPartsItemDiff_W();
		case ItemRes.eRES_TEX.DIFF_M:
			return ItemData.GetPartsItemDiff_M();
		}

        return null;
    }

    public string GetGoResPath(ItemRes.eRES_GO eType)
    {
        switch (eType)
        {
			 case ItemRes.eRES_GO.ICON:
                return ItemData.GetIcon();	
			
            case ItemRes.eRES_GO.DROP:
                return ItemData.GetDropItem();
			
            case ItemRes.eRES_GO.PARTS_W:
                return ItemData.GetPartsItem_W();
			
			case ItemRes.eRES_GO.PARTS_M:
                return ItemData.GetPartsItem_M();
        }
        return null;
    }

    
    // Load
    public bool LoadTexRes(ItemRes.eRES_TEX eType )
    {
        string strPath = GetTexResPath( eType );
        if (null == strPath)                   
            return false;                  

        return ItemRes.LoadTexture(eType, strPath);        
    }


    public bool LoadGoRes(ItemRes.eRES_GO eType)
    {
        string strPath = GetGoResPath(eType);
        if (null == strPath)
            return false;

        return ItemRes.LoadGameObject(eType, strPath);
    }
#else
	
	protected Texture LoadTexture(string strPath)
    {      
        Texture tex = ResourceLoad.Loadtexture(strPath);

        if (null == tex)
        {
            Debug.LogError("Load failed. tex file path : " + strPath);           
        }

        return tex;
    }


    protected GameObject LoadGameObject(string strPath)
    {   
        GameObject go = ResourceLoad.LoadGameObject(strPath);
        if (null == go)
        {
            Debug.LogError("Load failed. GameObject path : " + strPath);           
        }

        return go;
    }    
	
	
	public GameObject GetDropItem()
    {
        string strPath = ItemData.GetDropItem();
        if (null == strPath)
		{
			Debug.LogError("Item::GetDropItem() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadGameObject(strPath);
    }

    public GameObject GetIcon()
    {
		string strPath = ItemData.GetIcon();
        if (null == strPath)
		{
			Debug.LogError("Item::GetIcon() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadGameObject(strPath);
    }

    public GameObject GetPartsItem_W()
    {
		string strPath = ItemData.GetPartsItem_W();
        if (null == strPath)
		{
			Debug.LogError("Item::GetPartsItem_W() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadGameObject(strPath);      
    }
	
	public GameObject GetPartsItem_M()
    {
		string strPath = ItemData.GetPartsItem_M();
        if (null == strPath)
		{
			Debug.LogError("Item::GetPartsItem_M() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadGameObject(strPath);        
    }

    public Texture GetPartsDiff_W()
    {
		string strPath = ItemData.GetPartsItemDiff_W();
        if (null == strPath)
		{
			Debug.LogError("Item::GetPartsItemDiff_W() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadTexture(strPath);        
    }
	
	public Texture GetPartsDiff_M()
    {
       string strPath = ItemData.GetPartsItemDiff_M();
        if (null == strPath)
		{
			Debug.LogError("Item::GetPartsItemDiff_M() path null [ id: " + ItemData.m_iID );
			return null;
		}
		
        return LoadTexture(strPath);  
    }
    
#endif
	public string GetStrKind()
	{
		int iStringID = 0;
		
		if( eITEM_TYPE.EquipItem == ItemData.GetItemType() || eITEM_TYPE.CosEquipItem == ItemData.GetItemType() )
		{
			switch( (Item.eEQUIP)ItemData.GetSubType() )
			{
			case eEQUIP.Weapon:
				iStringID = 1032;
				break;
			case eEQUIP.Head:
				iStringID = 1033;
				break;
			case eEQUIP.Armor:
				iStringID = 1034;
				break;				
			case eEQUIP.Gloves:
				iStringID = 1035;
				break;
			case eEQUIP.Point:
				iStringID = 1036;
				break;
			case eEQUIP.Earring:
				iStringID = 1037;
				break;
			case eEQUIP.Necklace:
				iStringID = 1038;
				break;
			case eEQUIP.Ring:
				iStringID = 1039;
				break;
			case eEQUIP.SubAcc:
				iStringID = 1040;
				break;
			case eEQUIP.Fairy:
				iStringID = 1570;
				break;
			case eEQUIP.Wing:
				iStringID = 1571;
				break;
			case eEQUIP.Pet:
				iStringID = 2316;
				break;
			}		
		}		
		else if( eITEM_TYPE.ActionItem == ItemData.GetItemType() ) 
		{
			iStringID = 1041;
		}
		else if( eITEM_TYPE.EtcItem == ItemData.GetItemType() )
		{
			switch( (eEtcItem)ItemData.GetSubType() )
			{
			case eEtcItem.Strengthen:
				iStringID = 1044;
				break;
			case eEtcItem.Material:
				iStringID = 1045;
				break;
				
			case eEtcItem.Etc:
				iStringID = 1046;
				break;
				
			case eEtcItem.Quest:
				iStringID = 1620;
				break;
				
			default:
				return string.Empty;
			}
			
		}
		else if( eITEM_TYPE.UseItem == ItemData.GetItemType() )
		{
			switch( (eUSE_ITEM)ItemData.GetSubType() )
			{
			case eUSE_ITEM.SkillReset:
				iStringID = 1623;
				break;
				
			case eUSE_ITEM.QuestRandom:
			case eUSE_ITEM.Random:
				iStringID = 1621;
				break;			
				
			case eUSE_ITEM.PrivateStore1:
				iStringID = 1622;
				break;
			case eUSE_ITEM.PrivateStore2:
				iStringID = 1622;
				break;
			case eUSE_ITEM.PrivateStore3:
				iStringID = 1622;
				break;
			case eUSE_ITEM.PrivateStore4:
				iStringID = 1622;
				break;
			case eUSE_ITEM.PrivateStore5:
				iStringID = 1622;
				break;
				
			case eUSE_ITEM.Gold:
				iStringID = 1616;
				break;
				
			case eUSE_ITEM.Miracle:
				iStringID = 1917;
				break;
				
			case eUSE_ITEM.ConsumeQuest:
			case eUSE_ITEM.GetQuest:
			case eUSE_ITEM.InfiniteQuest:
				iStringID = 1620;
				break;
				
			case eUSE_ITEM.Summon:
				iStringID = 1497;
				break;
			case eUSE_ITEM.ConsumeHair:
				iStringID = 1843;
				break;
			case eUSE_ITEM.ChatChannel:
				iStringID = 1844;
				break;
			case eUSE_ITEM.ChatServer:
				iStringID = 1845;
				break;
			case eUSE_ITEM.CharacterNameReset:
				iStringID = 45578;
				break;
			case eUSE_ITEM.GuildNameReset:
				iStringID = 45577;
				break;
			case eUSE_ITEM.PetEgg:
				iStringID = 2314;
				break;
			case eUSE_ITEM.PetFood:
				iStringID = 2315;
				break;
			default:
				return string.Empty;
			}
		}
		else ///if( eITEM_TYPE.EtcItem == ItemData.GetItemType() ) 
		{
			return string.Empty;
		}	
		
		
		return AsTableManager.Instance.GetTbl_String( iStringID );
	}
	
	public string GetStrNeedJob()
	{
		int iStringID = 0;
		switch( ItemData.needClass )
		{
		case eCLASS.DIVINEKNIGHT:
			iStringID = 1054;
			break;
		case eCLASS.MAGICIAN:
			iStringID = 1055;
			break;
		case eCLASS.CLERIC:
			iStringID = 1057;
			break;
		case eCLASS.HUNTER:
			iStringID = 1056;
			break;
		case eCLASS.ASSASSIN:
			iStringID = 1058;
			break;
		case eCLASS.PET://$yde
			iStringID = 2258;
			break;
		case eCLASS.All:
			return "";
		}

		
		Tbl_String_Record strRecord = AsTableManager.Instance.GetTbl_String_Record( iStringID );
		if( null == strRecord )
		{
			Debug.LogError("Item::GetStrKind() [ null == strRecord ] " );
			return "";
		}	
		
		return strRecord.String;
	}
	
	
	public string GetStrGrade()
	{	
		s_strTemp.Length = 0;
		
		switch( ItemData.grade )
		{
		case eGRADE.Normal:
			s_strTemp.Append( ItemMgr.Instance.colorItemGradeNormal );
			s_strTemp.Append( AsTableManager.Instance.GetTbl_String( 1028 ) );			
			break;
		case eGRADE.Magic:
			s_strTemp.Append( ItemMgr.Instance.colorItemGradeMagic );
			s_strTemp.Append( AsTableManager.Instance.GetTbl_String( 1029 ) );			
			break;
		case eGRADE.Rare:
			s_strTemp.Append( ItemMgr.Instance.colorItemGradeRare );
			s_strTemp.Append( AsTableManager.Instance.GetTbl_String( 1030 ) );			
			break;
		case eGRADE.Epic:
			s_strTemp.Append( ItemMgr.Instance.colorItemGradeEpic );
			s_strTemp.Append( AsTableManager.Instance.GetTbl_String( 1031 ) );			
			break;
			
		case eGRADE.Ark:
			s_strTemp.Append( ItemMgr.Instance.colorItemGradeArk );
			s_strTemp.Append( AsTableManager.Instance.GetTbl_String( 1699 ) );	
			break;
		}
		
		if( 0 >= s_strTemp.Length )
			return string.Empty;
			
		return s_strTemp.ToString();
	}
	
	
	
	public static int GetCosEquipSlotIdx( Item.eEQUIP _type )
	{		
		switch( _type )
		{
		case Item.eEQUIP.Weapon:
			return 0;
		case Item.eEQUIP.Head:
			return 1;
		case Item.eEQUIP.Armor:
			return 2;
		case Item.eEQUIP.Gloves:
			return 3;
		case Item.eEQUIP.Point:
			return 4;
			
		case Item.eEQUIP.Wing:
			return Inventory.wingEquipSlotIdx;	
		case Item.eEQUIP.Fairy:
			return Inventory.fairyEquipSlotIdx;
			
		}
		
		Debug.LogError("GetCosEquipSlotIdx() error equip : " + _type );
		return 0;
	}
	
	public static bool IsCheckEquipIndex( int iIndex )
	{
		if( iIndex < 0 || iIndex >= equipMaxCount )
			return false;
		
		return true;
	}
	
	public static bool IsCheckCosEquipIndex( int iIndex )
	{
		if( iIndex < equipMaxCount || iIndex >= (equipMaxCount+equipMaxCount) )
			return false;
		
		return true;
	}

	public static bool CheckItem_PetEgg(Item _item)
	{
		if(_item == null)
			return false;

		if(eITEM_TYPE.UseItem == _item.ItemData.GetItemType() && eUSE_ITEM.PetEgg == (eUSE_ITEM)_item.ItemData.GetSubType() )
			return true;
		else
			return false;
	}

	public static bool CheckItem_PetFood(Item _item)
	{
		if(_item == null)
			return false;
		
		if(eITEM_TYPE.UseItem == _item.ItemData.GetItemType() && eUSE_ITEM.PetFood == (eUSE_ITEM)_item.ItemData.GetSubType() )
			return true;
		else
			return false;
	}
	
	/*public bool IsNeedSkillEffect()
	{
		return ItemData.GetItemType() == Item.eITEM_TYPE.SkillItem;
	}*/

}
