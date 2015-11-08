using UnityEngine;
using System.Collections;

public class AsRecommendItem : MonoBehaviour {
	
	public UIButton     m_RcRewardBtn    		 = null;
	public SpriteText   m_RcItemNameText 		 = null;
	public SpriteText   m_RcMessageText  		 = null;
	private UISlotItem  m_RcRewardSlotItem       = null;
 	public SpriteText   m_CountText = null;
    private Item	    m_RcRewardItem           = null;
	public SimpleSprite m_RcRewarditemImgPos     = null;
	public Vector2      minusItemSize;
	
	private int    		m_ItemIndex     		 = 0;
	private int   		m_ItemCount   			 = 0;
	private int 		m_AccrueCount 			 = 0;
	private int         m_nRecommendCount        = 0;
	
	private body_SC_SOCIAL_RECOMMEND m_data;	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update()
    {
	
#if UNITY_EDITOR
		if( true == Input.GetMouseButtonDown(0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
				
			if( m_RcRewardSlotItem == null || m_RcRewardItem == null)
				return;		
			if(  m_RcRewardSlotItem.iconImg.collider == null)
				return;
			if( true == AsUtil.PtInCollider(  m_RcRewardSlotItem.iconImg.collider, ray))
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_RcRewardItem, false, -10.0f);				
		}
#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch(0);
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( touch.position);
			if( TouchPhase.Began == touch.phase)
			{
				
				if( m_RcRewardSlotItem == null || m_RcRewardItem == null)
					return;		
				if(  m_RcRewardSlotItem.iconImg.collider == null)
					return;		
				if( true == AsUtil.PtInCollider(  m_RcRewardSlotItem.iconImg.collider, ray))
					TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_RcRewardItem, false, -10.0f);	
				
			}
		}
#endif

	}
	public void SetButton()
	{			
		m_RcRewardBtn.gameObject.SetActiveRecursively(true);
		m_RcRewardBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);
		m_RcRewardBtn.controlIsEnabled = false;	
		m_RcRewardBtn.spriteText.Color = Color.gray;
		m_RcRewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1567);
		
		if( m_nRecommendCount  >= m_AccrueCount )
		{
			m_RcRewardBtn.spriteText.Color = Color.gray;
			m_RcRewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);								
		}
	}
	
	public void SetText()
	{		
		
		m_RcRewardItem = ItemMgr.ItemManagement.GetItem( m_ItemIndex );
		if( null == m_RcRewardItem )
			return;			
		m_RcItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RcRewardItem.ItemData.nameId);		
		if( null != m_RcRewardSlotItem)
			GameObject.Destroy( m_RcRewardSlotItem.gameObject);		
		m_RcRewardSlotItem = ResourceLoad.CreateItemIcon( m_RcRewardItem.GetIcon(), m_RcRewarditemImgPos, Vector3.back, minusItemSize, true);		
		m_RcRewarditemImgPos.renderer.enabled = false;
		//m_RcRewardSlotItem.SetItemCountText(m_ItemCount); //#22119
		if(m_ItemCount > 1)
			m_CountText.Text = m_ItemCount.ToString();	
		else	
			m_CountText.Text = string.Empty;
		m_RcMessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1910),m_AccrueCount );	
	}
	
	public void SetData(body_SC_SOCIAL_RECOMMEND data, int id)		
	{
	
	    m_ItemIndex     			 = data.arrItemIndex[id];
	    m_ItemCount   				 = data.arrItemCount[id];
		m_AccrueCount 			     = data.arrAccrueCount[id];
     	m_nRecommendCount            = data.nRecommendCompleteCount;
		SetButton();
		SetText ();
	}	
}
