using UnityEngine;
using System.Collections;

public class TooltipInfoDlg : TooltipDlg 
{
	
	//-----------------------------------------------
    /* Variable */
    //-----------------------------------------------	
	public SpriteText nameText;
	public SpriteText limitLevelText;
	public SpriteText contentText_1;
	public SpriteText contentText_2;
	public SpriteText contentText_3;
	public SpriteText contentText_4;
	public SpriteText itemKindText;
	public SpriteText itemNeedJob;	
	public SpriteText textEquip;	
	public GameObject itemIconImgPos;
	public SpriteText textGrade;
	[SerializeField]SpriteText rankPointLabel = null;
	[SerializeField]SpriteText rankPoint = null;
	public Color colorStrength = Color.white;
	public Color colorStrengthAdd = Color.white;
	
	private sITEM m_sItem;    
	private GameObject m_goIconImg;
	
	
	System.Text.StringBuilder m_sbCommTemp = new System.Text.StringBuilder();
	System.Text.StringBuilder m_sbEquipTemp = new System.Text.StringBuilder();
	
	private void SetTextEquip( bool isEquip )
	{
		if( false == isEquip )
		{
			textEquip.Text = string.Empty;
			
			if( null != getItem && (Item.eITEM_TYPE.EquipItem == getItem.ItemData.GetItemType() || Item.eITEM_TYPE.CosEquipItem == getItem.ItemData.GetItemType() ) )
			{
				int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>(eComponentProperty.LEVEL);				
				eCLASS __class = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
						
				m_sbCommTemp.Length = 0;
				if( iCurLevel < getItem.ItemData.levelLimit || __class != getItem.ItemData.needClass && eCLASS.All != getItem.ItemData.needClass )
				{
					m_sbCommTemp.Append( TooltipMgr.Instance.colorItemLimit );
					m_sbCommTemp.Append( AsTableManager.Instance.GetTbl_String(381) );
					textEquip.Text = m_sbCommTemp.ToString();
				}
				
				if( AsPetManager.Instance.CheckPetEquipEnable( getItem.ItemData) == false)
				{
					textEquip.Text = string.Empty;
					
					m_sbCommTemp.Append( TooltipMgr.Instance.colorItemLimit );
					m_sbCommTemp.Append( AsTableManager.Instance.GetTbl_String(381) );
					textEquip.Text = m_sbCommTemp.ToString();
				}
			}
		}
		else
		{
			m_sbCommTemp.Append( TooltipMgr.Instance.colorEquipEnable );
			m_sbCommTemp.Append( AsTableManager.Instance.GetTbl_String(332) );
			
			textEquip.Text = m_sbCommTemp.ToString();
		}
	}
	
	#region - check equip enable -
	#endregion
	
	private void SetTextGrade( Item _item )
	{
		if( null == textGrade )
			return;
		if( null == _item )
		{
			textGrade.Text = string.Empty;			
		}
		else
		{
			textGrade.Text = _item.GetStrGrade();
		}
		
		
	}
	
	
	public void Open( Item _item, bool isRandomItemAuto, bool isEquip )
	{
		if( false == SetItem( _item ) )
			return;
		
		SetTextEquip(isEquip);
		SetCommonItem();
		SetTextGrade( getItem );		
		
		
		switch (_item.ItemData.GetItemType())
		{
		case Item.eITEM_TYPE.EquipItem:
			SetEquipItem(isRandomItemAuto); 
			CalRankPointDef();
			break;
		case Item.eITEM_TYPE.CosEquipItem:
			SetCosEquipItem(isRandomItemAuto); 		
			CalRankPointDef();
			break;			
		
		case Item.eITEM_TYPE.ActionItem:
			SetConsumeItem();	
			SetRankPoint(0);
			break;
			
		case Item.eITEM_TYPE.EtcItem:
			if( Item.eEtcItem.Enchant == (Item.eEtcItem)_item.ItemData.GetSubType() )
			{				
				if( true == EnchantDlg.IsSkillEnchant( _item ) )
				{
					SetSkillEnchantItem();
				}
				else
				{
					SetEtcItem();
				}
				CalRankPointDef();			
				
			}
			else
			{
				SetEtcItem();
				SetRankPoint(0);
			}
			break;
			
		default:			
			SetEtcItem();
			SetRankPoint(0);
			break;			
		}	
	}
	
	public void Open( sITEM _sItem, bool isEquip )
	{
		Item _item = ItemMgr.ItemManagement.GetItem( _sItem.nItemTableIdx );
		if( false == SetItem( _item ) )
			return;
		
		SetTextEquip(isEquip);
		
		m_sItem = _sItem;
		
		SetCommonItem();
		SetTextGrade( getItem );
		
		
		switch (getItem.ItemData.GetItemType())
		{
		case Item.eITEM_TYPE.EquipItem:
			 
			SetEquipItem( false );   
			CalRankPoint();
			break;
		case Item.eITEM_TYPE.CosEquipItem:
			SetCosEquipItem( false );  
			CalRankPoint();
			break;			
		
		case Item.eITEM_TYPE.ActionItem:
			SetConsumeItem();
			SetRankPoint(0);
			break;
			
		case Item.eITEM_TYPE.EtcItem:
			if( Item.eEtcItem.Enchant == (Item.eEtcItem)_item.ItemData.GetSubType() )
			{
				if( true == EnchantDlg.IsSkillEnchant( _item ) )
				{
					SetSkillEnchantItem();
				}
				else
				{
					SetEtcItem();
				}
				CalRankPointDef();
			}
			else
			{
				SetEtcItem();
				SetRankPoint(0);
			}
			break;
			
		default:
			SetEtcItem();
			SetRankPoint(0);
			break;			
		}	
	}

	private void CalRankPointDef()
	{
		if( true == getItem.ItemData.isItem_OptionType )
		{
			float iOptionTableValue_1 = 0f;
			float iOptionTableValue_2 = 0f;
		
			if( 0 != getItem.ItemData.getItem_Fix1_ID )
			{
				Tbl_ItemRankWeight_Record itemRecord_1 = AsTableManager.Instance.GetTblItemRankWeightRecord( 
					getItem.ItemData.levelLimit, (eITEM_EFFECT)getItem.ItemData.getItem_Fix1_ID );
				
				if( null !=itemRecord_1 )
				{
					if( true == itemRecord_1.m_itemClassList.ContainsKey( getItem.ItemData.needClass ) )
					{
						iOptionTableValue_1 = itemRecord_1.m_itemClassList[getItem.ItemData.needClass] * (float)getItem.ItemData.getItem_Fix1_Value;
					}
				}
			}	
			
			if( 0 != getItem.ItemData.getItem_Fix2_ID )
			{
				Tbl_ItemRankWeight_Record itemRecord_2 = AsTableManager.Instance.GetTblItemRankWeightRecord( 
					getItem.ItemData.levelLimit, (eITEM_EFFECT)getItem.ItemData.getItem_Fix2_ID );
				
				if( null !=itemRecord_2 )
				{
					if( true == itemRecord_2.m_itemClassList.ContainsKey( getItem.ItemData.needClass ) )
					{
						iOptionTableValue_2 = itemRecord_2.m_itemClassList[getItem.ItemData.needClass] * (float)getItem.ItemData.getItem_Fix2_Value;
					}
				}
			}	
			
			SetRankPoint( (int)((float)getItem.ItemData.m_iRankPoint + iOptionTableValue_1 + iOptionTableValue_2 ) );		
		}
		else
		{
			SetRankPoint( getItem.ItemData.m_iRankPoint );		
		}
	}
	
	
	
	
	private void CalRankPoint()
	{
		
		
		SetRankPoint( ItemMgr.GetRealRankPoint( getSItem, getItem ) );
	}
	
	
		
	
		
		
	// Get		
	public sITEM getSItem
	{
		get
        {
            return m_sItem;
        }
	}
	
	private SpriteText GetContentText()
	{
		if( contentText_1.text.Length < 1 )
			return contentText_1;
		else if ( contentText_2.text.Length < 1 )
			return contentText_2;
		else if ( contentText_3.text.Length < 1 )
			return  contentText_3;
		else
			return  contentText_4;
	}
	
	
	public bool SetSItem( sITEM _sItem )
	{
		if (null == _sItem)
        {
            Debug.LogError("TooltipInfoDlg::setRealItem() [ null == realitem ]");
            return false;
        }

        m_sItem = _sItem;
		
		Item _item = ItemMgr.ItemManagement.GetItem( m_sItem.nItemTableIdx );
		if( null == _item )
		{
			Debug.LogError("TooltipInfoDlg::SetSItem() [ null == _item ]");
            return false;
		}
		
		
		if( false == SetItem( _item ) )
			return false;
		
		return true;
	}
	
	
	protected void SetCommonItem()
	{
		// name text
		SetNameText();
		
		// icon img
		SetIconImg();		
		
		// level text
		SetLevelText();
		
		// item kind
		SetItemKindText();
		
		// need job
		SetItemNeedJob();	
	}	
		
	private void SetRankPoint( int point)
	{
		if( null == rankPoint || null == rankPointLabel )
			return;
		if( 0 == point )
		{
			rankPoint.text = string.Empty;
			rankPointLabel.gameObject.active = false;
			return;
		}
		
		rankPointLabel.gameObject.active = true;
		rankPoint.Text = point.ToString();
	}	
	
	private void SetNameText()
	{
		//string strName = AsTableManager.Instance.GetTbl_String( getItem.ItemData.nameId );			
		
		m_sbCommTemp.Remove( 0, m_sbCommTemp.Length );
		if( null != getSItem && 0 < getSItem.nStrengthenCount )
		{			
			//nameText.Text = Color.blue.ToString() + "+" + getSItem.nStrengthenCount + " " + getItem.ItemData.GetGradeColor() +  strName;
			m_sbCommTemp.Append( colorStrength );
			m_sbCommTemp.Append( "+" );
			m_sbCommTemp.Append( getSItem.nStrengthenCount );
			m_sbCommTemp.Append( " " );
			m_sbCommTemp.Append( getItem.ItemData.GetGradeColor() );
			m_sbCommTemp.Append( AsTableManager.Instance.GetTbl_String( getItem.ItemData.nameId ) );
		}
		else
		{
			//nameText.Text = getItem.ItemData.GetGradeColor() +  strName;
			m_sbCommTemp.Append( getItem.ItemData.GetGradeColor() );
			m_sbCommTemp.Append( AsTableManager.Instance.GetTbl_String( getItem.ItemData.nameId ) );
		}
		
		nameText.Text = m_sbCommTemp.ToString();
	}
	
	private void SetIconImg()
	{
		if( null != m_goIconImg )
		{
			GameObject.DestroyObject( m_goIconImg );
		}
		
		GameObject goRes = getItem.GetIcon();
		if( null == goRes )
		{
			Debug.LogError("TooltipInfoDlg::SetIconImg()[ null == goRes ] item id : " + getItem.ItemID );
			return;
		}
		
        m_goIconImg = GameObject.Instantiate(goRes) as GameObject;		
		m_goIconImg.transform.parent = itemIconImgPos.transform;
		m_goIconImg.transform.localPosition = Vector3.zero;
		m_goIconImg.transform.localRotation = Quaternion.identity;
		m_goIconImg.transform.localScale = Vector3.one;
	}
	
	private void SetLevelText()
	{
		int iCurLevel = AsEntityManager.Instance.GetPlayerCharFsm().UserEntity.GetProperty<int>(eComponentProperty.LEVEL);
		m_sbCommTemp.Remove( 0, m_sbCommTemp.Length );
        if (iCurLevel < getItem.ItemData.levelLimit)
		{           
			m_sbCommTemp.Append( TooltipMgr.Instance.colorItemLimit );
			m_sbCommTemp.Append( "Lv." );
			m_sbCommTemp.Append( getItem.ItemData.levelLimit );
		}
		else
		{           
			m_sbCommTemp.Append( "Lv." );
			m_sbCommTemp.Append( getItem.ItemData.levelLimit );
		}
		
		limitLevelText.Text = m_sbCommTemp.ToString();
	}
	
	private void SetItemKindText()
	{
        itemKindText.Text = getItem.GetStrKind();
	}
	
	private void SetItemNeedJob()
	{
		if(	AsGameMain.s_gameState != GAME_STATE.STATE_INGAME ) //#18091 dopamin.
			return;
		
		if( null == AsEntityManager.Instance.UserEntity )
		{
			Debug.LogError("TooltipInfoDlg::SetItemNeedJob()[ null == AsEntityManager.Instance.UserEntity ] ");
			return;
		}
		
		m_sbCommTemp.Remove( 0, m_sbCommTemp.Length);
		eCLASS __class = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS);
        if (__class != getItem.ItemData.needClass)
		{
			//itemNeedJob.Text = TooltipMgr.Instance.colorItemLimit.ToString() + getItem.GetStrNeedJob();
			m_sbCommTemp.Append( TooltipMgr.Instance.colorItemLimit );
			m_sbCommTemp.Append( getItem.GetStrNeedJob() );            
		}
		else
		{
			//itemNeedJob.Text = getItem.GetStrNeedJob();
			m_sbCommTemp.Append( getItem.GetStrNeedJob() );            
		}
		
		itemNeedJob.Text = m_sbCommTemp.ToString();
	}
	
	
	
	protected void SetEquipItem( bool isRandomOptionAuto )
	{
		contentText_1.Text = "";
		contentText_2.Text = "";
		contentText_3.Text = "";	
		contentText_4.Text = "";
	
		
		Tbl_Strengthen_Record strengthenRecord = null;		
		if( null != getSItem && 0 < getSItem.nStrengthenCount )
		{
			strengthenRecord = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( getItem.ItemData.GetItemType(),
				(Item.eEQUIP)getItem.ItemData.GetSubType(), getItem.ItemData.grade, getItem.ItemData.levelLimit, getSItem.nStrengthenCount );
			if( null == strengthenRecord)
			{
				Debug.LogWarning("TooltipInfoDlg::SetEquipItem()[ null == StrengthenStateTable ] parts : " + (Item.eEQUIP)getItem.ItemData.GetSubType() +
					" grade : " + getItem.ItemData.grade + " levelLimit : " + getItem.ItemData.levelLimit );
			}
		}
		
		Item.eEQUIP eEquip = (Item.eEQUIP)getItem.ItemData.GetSubType();
		
		// attack
		/*switch( eEquip )
		{
		case Item.eEQUIP.Weapon:
			GetContentText().Text = GetEquipWeaponAttackText(strengthenRecord);
			break;
			
		case Item.eEQUIP.Ring:
		case Item.eEQUIP.Earring:
		case Item.eEQUIP.Necklace:
			break;
			
		default:			
			break;
		}*/
		
		if( Item.eEQUIP.Weapon == eEquip )
		{
			GetContentText().Text = GetEquipWeaponAttackText(strengthenRecord);
		}
		
		// defence
		switch( eEquip )
		{
		case Item.eEQUIP.Weapon:			
		case Item.eEQUIP.Ring:
		case Item.eEQUIP.Earring:
		case Item.eEQUIP.Necklace:
		case Item.eEQUIP.Pet: //$yde
			break;
			
		default:
			GetContentText().Text = GetEquipPDefenceText(strengthenRecord);
			GetContentText().Text = GetEquipDefenceText(strengthenRecord);			
			break;
		}	
		
		// random option
		if( true == isRandomOptionAuto )
		{
			GetContentText().Text = AsTableManager.Instance.GetTbl_String(1001);
		}		
		else if( null != getSItem )
		{
			SetSpecialAbliltyText( (eITEM_OPTION)getSItem.nOptID_1, getSItem.nOptValue_1, GetContentText() );
			SetSpecialAbliltyText( (eITEM_OPTION)getSItem.nOptID_2, getSItem.nOptValue_2, GetContentText() );	
		}
		else if( true == getItem.ItemData.isItem_OptionType )
		{
			SetSpecialAbliltyText( Tbl_Strengthen_Record.GetOptionType( getItem.ItemData.getItem_Fix1_ID ), getItem.ItemData.getItem_Fix1_Value, 
									GetContentText() );
			SetSpecialAbliltyText( Tbl_Strengthen_Record.GetOptionType( getItem.ItemData.getItem_Fix2_ID ), getItem.ItemData.getItem_Fix2_Value, 
									GetContentText() );	
		}       
	}
	
	
	protected void SetCosEquipItem( bool isRandomOptionAuto )
	{
		contentText_1.Text = "";
		contentText_2.Text = "";
		contentText_3.Text = "";	
		contentText_4.Text = "";
	
		
		Tbl_Strengthen_Record strengthenRecord = null;		
		if( null != getSItem && 0 < getSItem.nStrengthenCount )
		{
			strengthenRecord = AsTableManager.Instance.GetStrengthenTable().GetStrengthenRecord( getItem.ItemData.GetItemType(),
				(Item.eEQUIP)getItem.ItemData.GetSubType(), getItem.ItemData.grade, getItem.ItemData.levelLimit, getSItem.nStrengthenCount );
			if( null == strengthenRecord)
			{
				Debug.LogWarning("TooltipInfoDlg::SetCosEquipItem()[ null == StrengthenStateTable ] parts : " + (Item.eEQUIP)getItem.ItemData.GetSubType() +
					" grade : " + getItem.ItemData.grade + " levelLimit : " + getItem.ItemData.levelLimit );
			}
		}
		
		Item.eEQUIP eEquip = (Item.eEQUIP)getItem.ItemData.GetSubType();
			
		
		if( Item.eEQUIP.Weapon == eEquip )
		{
			GetContentText().Text = GetCosEquipWeaponAttackText(strengthenRecord);
		}
		
		// defence
		switch( eEquip )
		{
		case Item.eEQUIP.Weapon:			
		case Item.eEQUIP.Ring:
		case Item.eEQUIP.Earring:
		case Item.eEQUIP.Necklace:
			break;
			
		default:
			GetContentText().Text = GetCosEquipPDefenceText(strengthenRecord);
			GetContentText().Text = GetCosEquipDefenceText(strengthenRecord);			
			break;
		}	
		
		// random option
		if( true == isRandomOptionAuto )
		{
			GetContentText().Text = AsTableManager.Instance.GetTbl_String(1001);
		}	
		else if( null != getSItem )
		{
			SetSpecialAbliltyText( (eITEM_OPTION)getSItem.nOptID_1, getSItem.nOptValue_1, GetContentText() );
			SetSpecialAbliltyText( (eITEM_OPTION)getSItem.nOptID_2, getSItem.nOptValue_2, GetContentText() );	
			
		}
		else if( true == getItem.ItemData.isItem_OptionType )	
		{
			SetSpecialAbliltyText( Tbl_Strengthen_Record.GetOptionType( getItem.ItemData.getItem_Fix1_ID ), getItem.ItemData.getItem_Fix1_Value, 
								GetContentText() );
			SetSpecialAbliltyText( Tbl_Strengthen_Record.GetOptionType( getItem.ItemData.getItem_Fix2_ID ), getItem.ItemData.getItem_Fix2_Value, 
								GetContentText() );	
		}				     
	}
	
	private string GetCosEquipWeaponAttackText( Tbl_Strengthen_Record record )
	{
		//string strTemp = "";
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		
		float iValue = 0;
		
			
		if( getItem.ItemData.needClass == eCLASS.CLERIC || getItem.ItemData.needClass == eCLASS.MAGICIAN )
		{
			iValue = getItem.ItemData.m_Item_Costume_MatkDmg*0.1f;		
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1059) );
		}
		else
		{
			iValue = getItem.ItemData.m_Item_Costume_PatkDmg*0.1f;		
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1051) );
		}
		
		
		
		// this is Strengthen state
		if( null != record )
		{			
			float iValueStrengthen = iValue*record.getStrengthenRatiog*0.001f;				
		
			if( iValueStrengthen > 0 )
			{
				m_sbEquipTemp.Append( string.Format("{0:0.0}", iValue ) );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(+" );				
				m_sbEquipTemp.Append( string.Format("{0:0.0}", iValueStrengthen ) );
				m_sbEquipTemp.Append( ")" );				
			}
			else
			{
				m_sbEquipTemp.Append( string.Format("{0:0.0}", iValue ) );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(" );				
				m_sbEquipTemp.Append( string.Format("{0:0.0}", iValueStrengthen ) );
				m_sbEquipTemp.Append( ")" );			
			}
		}
		else	
		{
			m_sbEquipTemp.Append( string.Format("{0:0.0}", iValue ) );
		}
		
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1708) );	
		return m_sbEquipTemp.ToString();
	}
	
	private string GetCosEquipDefenceText( Tbl_Strengthen_Record record )
	{	
		
		float fPdef = (float)getItem.ItemData.m_Item_Costume_Mdef*0.1f;	
		
		
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1053) );	
		m_sbEquipTemp.Append( string.Format("{0:0.0}", fPdef ) );
		
		if( null != record )
		{					
			float min = fPdef*(float)record.getStrengthenRatiog*0.001f;
				
			if( 0 < min )
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(+");				
				m_sbEquipTemp.Append(string.Format("{0:0.0}", min ));
				m_sbEquipTemp.Append(")");				
			}
			else
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(");				
				m_sbEquipTemp.Append(string.Format("{0:0.0}", min ));
				m_sbEquipTemp.Append(")");
			}
		}
		
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1708) );
		
		//return strTemp;			
		return m_sbEquipTemp.ToString();
	}
	
	private string GetCosEquipPDefenceText( Tbl_Strengthen_Record record )
	{					
		
		float fPdef = (float)getItem.ItemData.m_Item_Costume_Pdef*0.1f;		
		
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		m_sbEquipTemp.Append(AsTableManager.Instance.GetTbl_String(1052));
		m_sbEquipTemp.Append( string.Format("{0:0.0}", fPdef ) );
			
		if( null != record )
		{					
			float min = fPdef*(float)record.getStrengthenRatiog*0.001f;
			
			
			if( 0 < min )
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(+");				
				m_sbEquipTemp.Append(string.Format("{0:0.0}", min ));
				m_sbEquipTemp.Append(")");	
			}
			else
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(");				
				m_sbEquipTemp.Append(string.Format("{0:0.0}", min ));
				m_sbEquipTemp.Append(")");
			}
		}
		
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1708) );
		return m_sbEquipTemp.ToString();
	}
	
	
	// equip state
	private string GetEquipWeaponAttackText( Tbl_Strengthen_Record record )
	{
		//string strTemp = "";
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		
		int iMin = 0;
		int iMax = 0;
			
		if( getItem.ItemData.needClass == eCLASS.CLERIC || getItem.ItemData.needClass == eCLASS.MAGICIAN )
		{
			iMin = getItem.ItemData.matkDmgMin;
			iMax = getItem.ItemData.matkDmgMax;
			//strTemp = AsTableManager.Instance.GetTbl_String(1059);
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1059) );
		}
		else
		{
			iMin = getItem.ItemData.parkDmgMin;
			iMax = getItem.ItemData.parkDmgMax;			
			//strTemp = AsTableManager.Instance.GetTbl_String(1051);
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1051) );
		}
		
		
		
		// this is Strengthen state
		if( null != record )
		{
			//string strT = "";			
			int min = iMin*record.getStrengthenRatiog/1000;
			int max = iMax*record.getStrengthenRatiog/1000;
			
			//if( min > 0 )
			//	strT = "+";				
			//strTemp += iMin + "(" + strT + min +")" + "~" + iMax + "(" +strT + max + ")";		
			if( min > 0 )
			{
				m_sbEquipTemp.Append( iMin );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(+" );				
				m_sbEquipTemp.Append( min );
				m_sbEquipTemp.Append( ")~" );
				m_sbEquipTemp.Append( Color.white );
				m_sbEquipTemp.Append( iMax );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(+" );				
				m_sbEquipTemp.Append( max );
				m_sbEquipTemp.Append( ")" );
			}
			else
			{
				m_sbEquipTemp.Append( iMin );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(" );				
				m_sbEquipTemp.Append( min );
				m_sbEquipTemp.Append( ")~" );
				m_sbEquipTemp.Append( Color.white );
				m_sbEquipTemp.Append( iMax );
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append( "(" );				
				m_sbEquipTemp.Append( max );
				m_sbEquipTemp.Append( ")" );
			}
		}
		else	
		{
			//strTemp += iMin + "~" + iMax;
			m_sbEquipTemp.Append( iMin );
			m_sbEquipTemp.Append("~");
			m_sbEquipTemp.Append( iMax );
		}
		
		//return strTemp;		
		return m_sbEquipTemp.ToString();
	}
	
	private string GetEquipDefenceText( Tbl_Strengthen_Record record )
	{			
		//string strTemp = AsTableManager.Instance.GetTbl_String(1053) + getItem.ItemData.mDef;	
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String(1053) );	
		m_sbEquipTemp.Append( getItem.ItemData.mDef );
		
		if( null != record )
		{		
			//string strT = "";
			int min = getItem.ItemData.mDef*record.getStrengthenRatiog/1000;
			//if( 0 < min )
			//	strT = "+";
			
			//strTemp += "(" + strT + min + ")";	
			if( 0 < min )
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(+");				
				m_sbEquipTemp.Append(min);
				m_sbEquipTemp.Append(")");				
			}
			else
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(");				
				m_sbEquipTemp.Append(min);
				m_sbEquipTemp.Append(")");
			}
		}
		
		//return strTemp;			
		return m_sbEquipTemp.ToString();
	}
	
	private string GetEquipPDefenceText( Tbl_Strengthen_Record record )
	{			
		//string strTemp = AsTableManager.Instance.GetTbl_String(1052) + getItem.ItemData.pDef;	
		m_sbEquipTemp.Remove( 0, m_sbEquipTemp.Length );
		m_sbEquipTemp.Append(AsTableManager.Instance.GetTbl_String(1052));
		m_sbEquipTemp.Append(getItem.ItemData.pDef);
			
		if( null != record )
		{		
			//string strT = "";
			int min = getItem.ItemData.pDef*record.getStrengthenRatiog/1000;
			//if( 0 < min )
			//	strT = "+";
			
			//strTemp += "(" + strT + min + ")";	
			
			if( 0 < min )
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(+");				
				m_sbEquipTemp.Append(min);
				m_sbEquipTemp.Append(")");	
			}
			else
			{
				m_sbEquipTemp.Append( colorStrengthAdd );
				m_sbEquipTemp.Append("(");				
				m_sbEquipTemp.Append(min);
				m_sbEquipTemp.Append(")");
			}
		}
		
		//return strTemp;
		return m_sbEquipTemp.ToString();
	}
	
		
	private void SetSpecialAbliltyText( eITEM_OPTION _eItemOption, int ivalue, SpriteText spriteText )
	{		
		int iStringID = 0;
		
		if( _eItemOption  == eITEM_OPTION.eITEM_OPTION_NOTHING )
		{
			spriteText.Text = string.Empty;
			return;
		}
		
		m_sbEquipTemp.Remove(0, m_sbEquipTemp.Length);
		switch( _eItemOption )
		{			
		case eITEM_OPTION.eITEM_OPTION_HP_PLUS:
			iStringID = 1066;			
			break;			
			
		case eITEM_OPTION.eITEM_OPTION_MP_PLUS:
			iStringID = 1067;				
			break;				
			
		case eITEM_OPTION.eITEM_OPTION_ATK_HIT_PROB:
			//iStringID = 1076;				
			//break;
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1076 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
			
		case eITEM_OPTION.eITEM_OPTION_CRITICAL_PROB:
			//iStringID = 1074;			
			//break;
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1074 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
			
		case eITEM_OPTION.eITEM_OPTION_DODGE_PROB:
			//iStringID = 1077;				
			//break;	
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1077 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
			
		//---------	
		case eITEM_OPTION.eITEM_OPTION_PDEF:
			iStringID = 1072;				
			break;		
			
		case eITEM_OPTION.eITEM_OPTION_MDEF:
			iStringID = 1073;			
			break;
			
		case eITEM_OPTION.eITEM_OPTION_PATK_DMG_MIN:
			iStringID = 1068;				
			break;	
			
		case eITEM_OPTION.eITEM_OPTION_PATK_DMG_MAX:
			iStringID = 1069;				
			break;			
		
		case eITEM_OPTION.eITEM_OPTION_PATK_DMG:
			iStringID = 1092;			
			break;
			
		//--
		case eITEM_OPTION.eITEM_OPTION_MATK_DMG_MIN:
			iStringID = 1070;				
			break;	
			
		case eITEM_OPTION.eITEM_OPTION_MATK_DMG_MAX:
			iStringID = 1071;				
			break;
			
		case eITEM_OPTION.eITEM_OPTION_MATK_DMG:
			iStringID = 1093;				
			break;	
			
		case eITEM_OPTION.eITEM_OPTION_HP_REGEN_PROB:
			//iStringID = 1426;				
			//break;	
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1426 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
			
		case eITEM_OPTION.eITEM_OPTION_MP_REGEN_PROB:
			//iStringID = 1427;				
			//break;
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1427 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;


        case eITEM_OPTION.eITEM_OPTION_HP_REGEN:
            iStringID = 1079;				
			break;	
        case eITEM_OPTION.eITEM_OPTION_MP_REGEN:
            iStringID = 1080;				
			break;

		case eITEM_OPTION.eITEM_OPTION_ATK_SPEED_PROB:
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1639 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
			
		case eITEM_OPTION.eITEM_OPTION_MOVE_SPEED_PROB:
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( 1640 ) );
			m_sbEquipTemp.Append( " +" );
			m_sbEquipTemp.AppendFormat( "{0:0.0}%", (ivalue*0.1f) );				
			spriteText.Text = m_sbEquipTemp.ToString();
			return;
		
		case eITEM_OPTION.eITEM_OPTION_FIRE:
			//spriteText.Text = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( 1420 );
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.AppendFormat( AsTableManager.Instance.GetTbl_String( 1420 ), ivalue );
			spriteText.Text = m_sbEquipTemp.ToString();
			return;		
			
		case eITEM_OPTION.eITEM_OPTION_ICE:
			//spriteText.Text = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( 1421 );
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.AppendFormat( AsTableManager.Instance.GetTbl_String( 1421 ), ivalue );
			spriteText.Text = m_sbEquipTemp.ToString();
			return;	
			
		case eITEM_OPTION.eITEM_OPTION_LIGHT:
			//spriteText.Text = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( 1422 );
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.AppendFormat( AsTableManager.Instance.GetTbl_String( 1422 ), ivalue );
			spriteText.Text = m_sbEquipTemp.ToString();
			return;	
			
		case eITEM_OPTION.eITEM_OPTION_DARK:
			//spriteText.Text = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( 1423 );
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.AppendFormat( AsTableManager.Instance.GetTbl_String( 1423 ), ivalue );
			spriteText.Text = m_sbEquipTemp.ToString();
			return;	
			
		case eITEM_OPTION.eITEM_OPTION_NATURE:
			//spriteText.Text = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( 1424 );
			m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
			m_sbEquipTemp.AppendFormat( AsTableManager.Instance.GetTbl_String( 1424 ), ivalue  );
			spriteText.Text = m_sbEquipTemp.ToString();
			return;		
		}	
	
		//string strTemp = ItemMgr.Instance.colorRadomOption.ToString()+ AsTableManager.Instance.GetTbl_String( iStringID ) + " +" + ivalue;	
		m_sbEquipTemp.Append( ItemMgr.Instance.colorRadomOption );
		m_sbEquipTemp.Append( AsTableManager.Instance.GetTbl_String( iStringID ) );
		m_sbEquipTemp.Append( " +" );
		m_sbEquipTemp.Append( ivalue );
		
		//spriteText.Text = strTemp;		
		spriteText.Text = m_sbEquipTemp.ToString();
	}		
		
	
	
	
	// Consume
	protected void SetConsumeItem()
	{
		contentText_1.Text ="";
		contentText_2.Text = "";
		contentText_3.Text = "";
		contentText_4.Text = "";
		
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( getItem.ItemData.itemSkill );		
		if( null == record )
		{
			Debug.LogError("TooltipInfoDlg::SetConsumeItem() [ null == record ] skill level id : " + getItem.ItemData.itemSkill );
			return;
		}
		
		string szDesc = AsTableManager.Instance.GetTbl_String( record.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, getItem.ItemData.itemSkill, getItem.ItemData.itemSkillLevel, 0);		
		contentText_1.Text = szDesc;
	}
	
	
	protected void SetSkillEnchantItem()
	{
		contentText_1.Text ="";
		contentText_2.Text = "";
		contentText_3.Text = "";
		contentText_4.Text = "";
		
		Tbl_Skill_Record record = AsTableManager.Instance.GetTbl_Skill_Record( getItem.ItemData.itemSkill );		
		if( null == record )
		{
			Debug.LogError("TooltipInfoDlg::SetConsumeItem() [ null == Tbl_Skill_Record ] skill level id : " + getItem.ItemData.itemSkill );
			return;
		}
		Tbl_SoulStoneEnchant_Record _record = AsTableManager.Instance.GetSoulStoneEnchantTable( getItem.ItemID );
		if( null == _record )
		{
			Debug.LogError("TooltipInfoDlg::SetConsumeItem() [ null == Tbl_SoulStoneEnchant_Record ] skill level id : " + getItem.ItemData.itemSkill );
			return;
		}
		
		
		string szDesc = AsTableManager.Instance.GetTbl_String( record.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, getItem.ItemData.itemSkill, getItem.ItemData.itemSkillLevel, 0);		
		contentText_1.Text = string.Format( AsTableManager.Instance.GetTbl_String(getItem.ItemData.destId), (float)_record.getValue*0.1f ) + szDesc;
	}
	
	protected void SetEtcItem()
	{
		contentText_1.Text ="";
		contentText_2.Text = "";
		contentText_3.Text = "";
		contentText_4.Text = "";		
		
		contentText_1.Text = AsTableManager.Instance.GetTbl_String( getItem.ItemData.destId );
	}
	
	
	void Awake()
	{
		// < ilmeda, 20120822
		nameText.Text = "";
		limitLevelText.Text = "";
		contentText_1.Text = "";
		contentText_2.Text = "";
		contentText_3.Text = "";
		contentText_4.Text = "";
		itemKindText.Text = "";
		itemNeedJob.Text = "";
		rankPointLabel.Text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( nameText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( limitLevelText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_1);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_2);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_3);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( contentText_4);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( itemKindText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( itemNeedJob);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rankPointLabel);
		// ilmeda, 20120822 >
	}

	// Use this for initialization
	void Start () 
	{
		rankPointLabel.Text = AsTableManager.Instance.GetTbl_String(1666);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
