using UnityEngine;
using System.Collections;




public class ItemMgr : MonoBehaviour 
{
	
	public class CEffectDropData
	{
		private string m_strDropEffect = null;
		private string m_strGainEffect = null;
		private string m_strGainSound = null;
		
		public CEffectDropData( string _strDropEffect, string _strGainEffect, string _strGainSound )
		{
			m_strDropEffect = _strDropEffect;
			m_strGainEffect = _strGainEffect;
			m_strGainSound = _strGainSound;
		}
		
		public void PlayGainSound( Vector3 pos )
		{
			if(null == m_strGainSound)
				return;
			
			AsSoundManager.Instance.PlaySound( m_strGainSound, pos, false );
		}	
		
		public void PlayGainEffect( Vector3 pos )
		{
			if(null == m_strGainEffect)
				return;			
			
			AsEffectManager.Instance.PlayEffect( m_strGainEffect, pos );
		}
		
		public void PlayDropEffect( Transform _trs )
		{
			if( null == m_strDropEffect )
				return;
			
			AsEffectManager.Instance.PlayEffect( m_strDropEffect, _trs, true );
		}
	}
	//---------------------------------------------------------------------
	/* Variable */
	//---------------------------------------------------------------------	
	
	// Grade color
	public Color colorItemGradeNormal;
	public Color colorItemGradeMagic;
	public Color colorItemGradeRare;
	public Color colorItemGradeEpic;
	public Color colorItemGradeArk;
	public Color colorItemGradeSet;	
	public Color colorHasSetItem;
	public Color colorCompleteSetItem;
	public Color colorRadomOption;
	
	public static Color GetGradeColor( Item.eGRADE _grade)
	{
		switch( _grade )
		{
		case Item.eGRADE.Normal:
			return ItemMgr.Instance.colorItemGradeNormal;
		
		case Item.eGRADE.Magic:
			return ( ItemMgr.Instance.colorItemGradeMagic );					
			
		case Item.eGRADE.Rare:
			return( ItemMgr.Instance.colorItemGradeRare );			
		
		case Item.eGRADE.Epic:
			return( ItemMgr.Instance.colorItemGradeEpic );
			
			
		case Item.eGRADE.Ark:
			return( ItemMgr.Instance.colorItemGradeArk );		
		}
		
		return Color.white;
	}
	
	/*public string strRemoveDropItemEffect = "FX/Effect/Item/Fx_Dropltem@GoldBag";
	public string strRemoveDropItemSound = "Sound/Effect/S1009_EFF_Levelup";
	public string strRemoveDropGoldItemSound = "Sound/Effect/S1006_EFF_Jump";*/
	
	private CEffectDropData m_goldEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropGold", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpGold", null );
	
	private CEffectDropData m_normalEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropNormal", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpNormal", null );
	
	private CEffectDropData m_magicEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropMagic", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpMagic", null );
	
	private CEffectDropData m_rareEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropRare", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpRare", 
																		"Sound/Item/Drop/S5013_EFF_GetHighGradeItem" );
	
	private CEffectDropData m_EpicEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropEpic", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpEpic", 
																		"Sound/Item/Drop/S5013_EFF_GetHighGradeItem" );
	
	private CEffectDropData m_ArkEffectDropData = new CEffectDropData( "FX/Effect/Item/Fx_Dropltem_DropEpic", 
																		"FX/Effect/Item/Fx_Dropltem_PickUpEpic", 
																		"Sound/Item/Drop/S5013_EFF_GetHighGradeItem" );
	
	public CEffectDropData GetEffectDropData( Item.eGRADE _grade )
	{
		switch( _grade )
		{
		case Item.eGRADE.Normal:
			return getNormalEffectDropData;			
			
		case Item.eGRADE.Magic:
			return ItemMgr.Instance.getMagicEffectDropData;				
			
		case Item.eGRADE.Rare:
			return ItemMgr.Instance.getRareEffectDropData;			
			
		case Item.eGRADE.Epic:
			return ItemMgr.Instance.getEpicEffectDropData;	
			
		case Item.eGRADE.Ark:
			return ItemMgr.Instance.getArkEffectDropData;	
		}	
		
		return null;
	}
	
	public void PlayGainPlaySound( Item _item, Vector3 pos )
	{
		if( null == _item )
			return;
		
		if( Item.eITEM_TYPE.EtcItem == _item.ItemData.GetItemType() && (int)Item.eEtcItem.Gold == _item.ItemData.GetSubType() )
		{				
			AsSoundManager.Instance.PlaySound( "Sound/Item/Drop/S5000_EFF_GoldDrop", pos, false );			
			ItemMgr.Instance.getGoldEffectDropData.PlayGainSound( pos );
		}
		else
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Item/Drop/S5001_EFF_GetItem", pos, false );
			ItemMgr.CEffectDropData _data = ItemMgr.Instance.GetEffectDropData(_item.ItemData.grade);
			if( null != _data )
			{
				_data.PlayGainSound( pos );
			}
		}		
	}
	
	public void PlayGainEffect( Item _item, Vector3 pos )
	{
		if( null == _item )
			return;		
		
		if( Item.eITEM_TYPE.EtcItem == _item.ItemData.GetItemType() && (int)Item.eEtcItem.Gold == _item.ItemData.GetSubType() )
		{					
			ItemMgr.Instance.getGoldEffectDropData.PlayGainEffect( pos );
		}
		else
		{								
			ItemMgr.CEffectDropData _data = ItemMgr.Instance.GetEffectDropData(_item.ItemData.grade);
			if( null != _data )
			{
				_data.PlayGainEffect( pos );
			}
		}		
	}
	
	public void PlayDropEffect( Item _item, Transform _trs )
	{
		if( null == _item || null == _trs )
			return;		
		
		if( Item.eITEM_TYPE.EtcItem == _item.ItemData.GetItemType() && (int)Item.eEtcItem.Gold == _item.ItemData.GetSubType() )
		{					
			ItemMgr.Instance.getGoldEffectDropData.PlayDropEffect( _trs );
		}
		else
		{								
			ItemMgr.CEffectDropData _data = ItemMgr.Instance.GetEffectDropData(_item.ItemData.grade);
			if( null != _data )
			{
				_data.PlayDropEffect( _trs );
			}
		}		
	}
	
	
	public CEffectDropData getGoldEffectDropData
	{
		get
		{
			return m_goldEffectDropData;
		}
	}
	
	public CEffectDropData getNormalEffectDropData
	{
		get
		{
			return m_normalEffectDropData;
		}
	}
	
	public CEffectDropData getMagicEffectDropData
	{
		get
		{
			return m_magicEffectDropData;
		}
	}
	
	public CEffectDropData getRareEffectDropData
	{
		get
		{
			return m_rareEffectDropData;
		}
	}
	
	
	public CEffectDropData getEpicEffectDropData
	{
		get
		{
			return m_EpicEffectDropData;
		}
	}
	
	public CEffectDropData getArkEffectDropData
	{
		get
		{
			return m_ArkEffectDropData;
		}
	}
	
	public static bool IsEnableEnchant( sITEM _slItem )
	{	
		int iIndex = -1;
		foreach( int idata in _slItem.nEnchantInfo )
		{
			++iIndex;
			
			if( ItemMgr.Instance.GetEnchantStrengthenCount() > _slItem.nStrengthenCount )
				return true;
			
			if( -1 != idata )
				return true;
		}
		
		return false;				
	}
	
	public static int GetRealRankPoint(sITEM _sitem, Item _item)
	{
		if( null == _sitem || null == _item )
			return 0;		
		
		
		int iStrengthenRatiog = 0;
		
		if( 1 <= _sitem.nStrengthenCount )
		{
			Tbl_Strengthen_Record record = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( _item.ItemData.GetItemType(), 
				(Item.eEQUIP)_item.ItemData.GetSubType(),
				_item.ItemData.grade, _item.ItemData.levelLimit, _sitem.nStrengthenCount );
			
			if( null == record )
			{
				Debug.LogError("ItemMgr::GetRealRankPoint()[ null == Tbl_Strengthen_Record]");				
				return 0;
			}
			
			iStrengthenRatiog = record.getStrengthenRatiog;
		}
		
		float iOptionTableValue_1 = 0f;
		float iOptionTableValue_2 = 0f;
		
		if( 0 != _sitem.nOptID_1 )
		{
			Tbl_ItemRankWeight_Record itemRecord_1 = AsTableManager.Instance.GetTblItemRankWeightRecord( _item.ItemData.levelLimit, (eITEM_EFFECT)_sitem.nOptID_1 );
			if( null !=itemRecord_1 )
			{
				if( true == itemRecord_1.m_itemClassList.ContainsKey( _item.ItemData.needClass ) )
				{
					iOptionTableValue_1 = itemRecord_1.m_itemClassList[_item.ItemData.needClass] * (float)_sitem.nOptValue_1;
				}
			}
		}
		
		if( 0 != _sitem.nOptID_2 )
		{
			Tbl_ItemRankWeight_Record itemRecord_2 = AsTableManager.Instance.GetTblItemRankWeightRecord( _item.ItemData.levelLimit, (eITEM_EFFECT)_sitem.nOptID_2 );
			if( null !=itemRecord_2 )
			{
				if( true == itemRecord_2.m_itemClassList.ContainsKey( _item.ItemData.needClass ) )
				{
					iOptionTableValue_2 = itemRecord_2.m_itemClassList[_item.ItemData.needClass] * (float)_sitem.nOptValue_2;
				}
			}
		}
		
		int iEnchantPoint = 0;
		if( IsEnableEnchant( _sitem ) )	
		{
			foreach( int idata in _sitem.nEnchantInfo )
			{
				if( -1 == idata || 0 == idata )
					continue;
				
				Tbl_SoulStoneEnchant_Record enchantRecord = AsTableManager.Instance.GetSoulStoneEnchantTable( idata );
				if( null == enchantRecord )
					continue;
		
				iEnchantPoint += enchantRecord.getRankPoint;
			}
		}
		
		
			
		
		return
			(int)(_item.ItemData.m_iRankPoint + 
			((_item.ItemData.m_iRankPoint * iStrengthenRatiog)/1000) +
			(int)iOptionTableValue_1 +
			(int)iOptionTableValue_2 + iEnchantPoint );
	}
	
	public int GetEnchantStrengthenCount()		
	{
		Tbl_GlobalWeight_Record record = AsTableManager.Instance.GetTbl_GlobalWeight_Record( 21 );
		if( null == record )
		{
			return 0;
		}
		
		return (int)record.Value;
			
	}
	
	
	//
	private static ItemMgr ms_kIstance = null;
    private ItemManagement m_ItemManagement = null;
    private DropItemManagement m_DropItemManagement = null;
    private UseItemManagement m_UseItemManagement = null;
    private HadItemManagement m_HadItemManagement = null;

	//---------------------------------------------------------------------
	/* function*/
	//---------------------------------------------------------------------
	
	
	// Get 

	public static ItemMgr Instance
	{
		get
		{
			return ms_kIstance;
		}
	}

    public static ItemManagement ItemManagement
    {
        get
        {
			if( null == Instance )
				return null;
			
            return Instance.m_ItemManagement;
        }
    }

    public static DropItemManagement DropItemManagement
    {
        get
        {
			if( null == Instance )
				return null;
            return Instance.m_DropItemManagement;
        }
    }

    public static UseItemManagement UseItemManagement
    {
        get
        {
			if( null == Instance )
				return null;
			
            return Instance.m_UseItemManagement;
        }
    }

    public static HadItemManagement HadItemManagement
    {
        get
        {
			if( null == Instance )
				return null;
            return Instance.m_HadItemManagement;
        }
    }


	//---------------------------------------------------------------------
	/* Virtual */ 
	//---------------------------------------------------------------------	


    // Awake
	void Awake()
	{       
        m_ItemManagement = new ItemManagement();
        m_DropItemManagement = new DropItemManagement();
        m_UseItemManagement = new UseItemManagement();
        m_HadItemManagement = new HadItemManagement();

        ms_kIstance = this;
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
        
	}


	
	// Update is called once per frame
	void Update () 
	{        
		if( null != HadItemManagement )		
        	HadItemManagement.Inven.Update();
		
		if( null != m_DropItemManagement )
			m_DropItemManagement.UpdateNeedDeleteList();
	}   

}
