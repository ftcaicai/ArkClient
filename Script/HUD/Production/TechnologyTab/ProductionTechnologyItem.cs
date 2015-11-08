using UnityEngine;
using System.Collections;

public class ProductionTechnologyItem : MonoBehaviour 
{	
	public SpriteText textTechName;	
	public UIButton btnListView;
	
	private eITEM_PRODUCT_TECHNIQUE_TYPE m_eType;
	private sPRODUCT_INFO m_info = null;
	
	public eITEM_PRODUCT_TECHNIQUE_TYPE getProductTechType
	{
		get
		{
			return m_eType;
		}
	}
	
	public sPRODUCT_INFO getProductInfo
	{
		get
		{
			return m_info;
		}
	}
	
	protected virtual void SetTechName( string strName )
	{
		if( null == textTechName )
		{
			Debug.LogError("ProductionTechnologyItem::SetTechName()[null == textTechName]");
			return;
		}
		textTechName.Text = strName;
	}
	
	
	public virtual void Open( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, sPRODUCT_INFO _info )
	{
		m_info = _info;
		m_eType = _eType;
		
		//string strText = GetName(m_eType) + " ";
		if( null != _info )
		{
			//strText += _info.nLevel;			
			SetTechName( string.Format( "{0} {1:0}", GetName(m_eType), _info.nLevel ) );
		}
		else
		{
			//strText = GetName(m_eType);
			SetTechName( GetName(m_eType) );
		}
		
		
		if( null != btnListView )
			btnListView.SetInputDelegate(ListViewBtnDelegate);
		
	}
	
	private void ListViewBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{				
			if( false == AsHudDlgMgr.Instance.IsOpenProductionDlg )
				return;
			
			AsHudDlgMgr.Instance.productRadioIndex = m_eType;
			AsHudDlgMgr.Instance.productionDlg.SetTab(ProductionDlg.ePRODUCTION_STATE.LIST);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
					
		}
	}
	
	
	
	public static string GetName( eITEM_PRODUCT_TECHNIQUE_TYPE _eType )
	{
		switch( _eType )
		{
#if OLD_PRODUCT
		case eITEM_PRODUCT_TECHNIQUE_TYPE.WEAPON:
			return AsTableManager.Instance.GetTbl_String(1006);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.ARMOR:
			return AsTableManager.Instance.GetTbl_String(1007);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.ACCESSORY:
			return AsTableManager.Instance.GetTbl_String(1008);
#else
		case eITEM_PRODUCT_TECHNIQUE_TYPE.RING:
			return AsTableManager.Instance.GetTbl_String(2221);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.NECKLACE:
			return AsTableManager.Instance.GetTbl_String(2222);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.EARRING:
			return AsTableManager.Instance.GetTbl_String(2223);
#endif
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.POTION:
			return AsTableManager.Instance.GetTbl_String(1009);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL:			
			return AsTableManager.Instance.GetTbl_String(1010);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS:			
			return AsTableManager.Instance.GetTbl_String(1011);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT:
			return AsTableManager.Instance.GetTbl_String(1012);
		}	
		
		return "error max index";
	}
	
	
	protected ulong GetTechCost()
	{
		Tbl_GlobalWeight_Record record = null;
		switch ( AsUserInfo.Instance.GetProductTechniqueHaveCount() )
		{
		case 1:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(60);
			return (ulong)record.Value;			
			
		case 2:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(61);
			return (ulong)record.Value;
			
		case 3:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(62);
			return (ulong)record.Value;
			
		case 4:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(63);
			return (ulong)record.Value;
			
		case 5:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(64);
			return (ulong)record.Value;
			
		case 6:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(65);
			return (ulong)record.Value;
			
		case 7:
			record = AsTableManager.Instance.GetTbl_GlobalWeight_Record(66);
			return (ulong)record.Value;
		}
		
		return 0;
	}
	
	
	protected string GetTypeString( eITEM_PRODUCT_TECHNIQUE_TYPE _eType )
	{
		switch( _eType )
		{
#if OLD_PRODUCT
		case eITEM_PRODUCT_TECHNIQUE_TYPE.WEAPON:
			return AsTableManager.Instance.GetTbl_String(1334);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.ARMOR:
			return AsTableManager.Instance.GetTbl_String(1671);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.ACCESSORY:
			return AsTableManager.Instance.GetTbl_String(1672);
#else
		case eITEM_PRODUCT_TECHNIQUE_TYPE.RING:
			return AsTableManager.Instance.GetTbl_String(1334);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.NECKLACE:
			return AsTableManager.Instance.GetTbl_String(1671);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.EARRING:
			return AsTableManager.Instance.GetTbl_String(1672); 
#endif
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.POTION:
			return AsTableManager.Instance.GetTbl_String(1673);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL:			
			return AsTableManager.Instance.GetTbl_String(1674);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS:			
			return AsTableManager.Instance.GetTbl_String(1675);
			
		case eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT:
			return AsTableManager.Instance.GetTbl_String(1676);
		}	
		
		return "error max index";
	}
	
	
	// Use this for initialization
	void Start () 
	{	
		
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
