using UnityEngine;
using System.Collections;
using System.Text;

public class AsUIItemGetEquipAlram : AsItemGetNotifyItem
{
	public bool isMyselfDlg;
	public SpriteText textRankPont;
	
	public GameObject effect_one;
	
	//public GameObject effect_Magic;
    public GameObject effect_Rare;
    public GameObject effect_Epic;
    public GameObject effect_Ark;

	public BoxCollider tooltipCollider;       
	protected Item m_item;
	protected sITEM m_sitem;


	public virtual void Open( sITEM _sitem, AsUserEntity _entity)
	{
		if( null == _sitem)
		{
			Debug.LogError( "AsUIItemGetEquipAlram::Open()[  null == _sitem  ]");
			AsHudDlgMgr.Instance.DeleteItemGetEquipAlramDlg();
			return;
		}

		Item _item = ItemMgr.ItemManagement.GetItem( _sitem.nItemTableIdx);
		if( null == _item)
		{
			Debug.LogError( "AsUIItemGetEquipAlram::Open()[  null == _item  ] id : " + _sitem.nItemTableIdx);
			AsHudDlgMgr.Instance.DeleteItemGetEquipAlramDlg();
			return;
		}

		m_item = _item;
		m_sitem = _sitem;

		System.Text.StringBuilder _sbTemp = new System.Text.StringBuilder();
		_sbTemp.Append( AsTableManager.Instance.GetTbl_String(1666));
		_sbTemp.Append( "+");
		_sbTemp.Append( ItemMgr.GetRealRankPoint( _sitem, _item));

		textRankPont.Text = _sbTemp.ToString();
		
		
		
		
		if( true == isMyselfDlg )
		{		
			if( null != m_SlotItem)
				GameObject.Destroy( m_SlotItem.gameObject);
			m_SlotItem = ResourceLoad.CreateItemIcon( _item.GetIcon(), itemImgPos, Vector3.back, minusItemSize, false );	
			
			if( Item.eGRADE.Rare == _item.ItemData.grade )
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6123_EFF_GetRareItem", Vector3.zero, false);				
			}
			else				
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6124_EFF_GetEpicArcItem", Vector3.zero, false);
			}
			
			Show();	
			StringBuilder sbItemName = new StringBuilder();
			sbItemName.Insert( 0, _item.ItemData.GetGradeColor().ToString());
			sbItemName.AppendFormat( "{0}", AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId));
			m_ItemNameText.Text = sbItemName.ToString();
			PlayEffect(m_item, _entity); 
		}
		else
		{
			float fTextWidth = textRankPont.GetWidth( _sbTemp.ToString());
			ChangePanelSize( fTextWidth);
			
			SetData( m_item.ItemID, gameObject, false);
			
			Vector3 tempSize = tooltipCollider.size;
			tempSize.x = m_BaseDlg.width;
			tooltipCollider.size = tempSize;
			Show();	
		}
		
		
			
		//PlayEffect(m_item, _entity);  
	}
	
	private void PlayEffect(Item _item, AsUserEntity _entity)
	{		
		//effect_Magic.SetActiveRecursively(false);
        effect_Rare.SetActiveRecursively(false);
        effect_Epic.SetActiveRecursively(false);
        effect_Ark.SetActiveRecursively(false);
		
        if (null != _entity /*&& true == _entity.IsCheckEquipEnable(_item)*/)
        {
            switch (_item.ItemData.grade)
            {
//            case Item.eGRADE.Magic:
//                effect_Magic.SetActiveRecursively(true);
//				if( false == isMyselfDlg )
//                	effect_Magic.transform.localScale = new Vector3(m_BaseDlg.width / 4.8f, effect_Magic.transform.localScale.y, effect_Magic.transform.localScale.z);
//                break;
            case Item.eGRADE.Rare:
                effect_Rare.SetActiveRecursively(true);
				if( false == isMyselfDlg )
                	effect_Rare.transform.localScale = new Vector3(m_BaseDlg.width / 4.8f, effect_Rare.transform.localScale.y, effect_Rare.transform.localScale.z);
                break;
            case Item.eGRADE.Epic:
                effect_Epic.SetActiveRecursively(true);
				if( false == isMyselfDlg )
                	effect_Epic.transform.localScale = new Vector3(m_BaseDlg.width / 4.8f, effect_Epic.transform.localScale.y, effect_Epic.transform.localScale.z);
                break;
            case Item.eGRADE.Ark:
                effect_Ark.SetActiveRecursively(true);
				if( false == isMyselfDlg )
                	effect_Ark.transform.localScale = new Vector3(m_BaseDlg.width / 4.8f, effect_Ark.transform.localScale.y, effect_Ark.transform.localScale.z);
                break;           
            }            
        }  	
	}

	private void ChangePanelSize( float fTextWidth)
	{
		if( null == m_BaseDlg )
			return;
		
		Vector3 pos = textRankPont.transform.localPosition;
		pos.x = -( fTextWidth * 0.5f) + ( itemImgPos.width * 0.5f);
		textRankPont.transform.localPosition = pos;

		pos = itemImgPos.transform.localPosition;
		pos.x = -( ( fTextWidth * 0.5f) + MARGIN_OFFSET);
		itemImgPos.transform.localPosition = pos;
		m_BaseDlg.width = fTextWidth + itemImgPos.width;
		m_BaseDlg.Assign();
	}

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		//UpdateLogic();

		if( Input.touchCount == 1 || Input.GetMouseButtonDown(0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);

			#if !UNITY_EDITOR
			if( Input.GetTouch( 0).phase != TouchPhase.Began)
				return;
			#endif

			if( m_SlotItem == null || m_sitem == null)
				return;

			if( tooltipCollider == null)
				return;

			if( true == AsUtil.PtInCollider( tooltipCollider, ray))
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_sitem, -10.0f);
		}
	}
}
