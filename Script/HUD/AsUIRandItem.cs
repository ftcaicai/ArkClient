using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsUIRandItem : UIOpenGacha 
{
	
	public UIProgressBar progress;
	public SpriteText textItemName;
	public UIButton btnOk;
	public SpriteText textShow;
	public SpriteText textUseItemName;
	public SimpleSprite itemReceivePos;
	public SpriteText textResult;
	
	public GameObject epicImg;
	public GameObject normalImg;
	public GameObject rareImg;
	public GameObject magicImg;
	public GameObject arkImg;
	public Collider touchCollider;
	

	private RealItem m_ReceiveRealItem = null;
	private int m_iSlotIndex = 0;
	private AudioSource m_AudioSources = null;
	
	[HideInInspector] public bool isCanUseDlg = true;

	System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();

	public void Open( int iUseItemId, int iReceiveItemId, RealItem _realitem,int iSlot )
	{
		Item _ReceiveItem = ItemMgr.ItemManagement.GetItem( iUseItemId );

		Open(iUseItemId, iReceiveItemId);
		
		SetSelfItem(iReceiveItemId, itemReceivePos);
		itemReceivePos.gameObject.SetActiveRecursively(false);
		textResult.gameObject.SetActiveRecursively(false);
		m_ReceiveRealItem = _realitem;
		m_iSlotIndex = iSlot;
		
		btnOk.gameObject.SetActiveRecursively(false);
		
		m_sbTemp.Length = 0;
		m_sbTemp.Append( _ReceiveItem.ItemData.GetGradeColor().ToString() );
		m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(  _ReceiveItem.ItemData.nameId ) );
		textUseItemName.Text = m_sbTemp.ToString();
		
		m_AudioSources = AsSoundManager.Instance.PlaySound( "Sound/Interface/S6033_EFF_Random_Touch", Vector3.zero, false);
	}
	
	public void Close()	
	{
		
	}
	
	
	private void CloseBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseRandItemUI();
		}
	}
	

	// Use this for initialization
	void Start () 
	{
		btnOk.SetInputDelegate(CloseBtnDelegate);
		progress.Text = AsTableManager.Instance.GetTbl_String(864);
		textShow.Text = AsTableManager.Instance.GetTbl_String(853);
		btnOk.Text = AsTableManager.Instance.GetTbl_String(1152);
		textResult.Text = AsTableManager.Instance.GetTbl_String(876);
	}
	
	// Update is called once per frame
	protected override void  Updating()
	{
		
		if( false == m_isNeedAction )
			return;
		
		m_fTime += Time.deltaTime;
		progress.Value = m_fTime / fTotalTime;
		
		m_fSpeed += Time.deltaTime;
		if( m_fSpeed > m_fMaxSpeed && 0 < m_SlotItemList.Count )
		{						
			if( 0 <= m_iActionIndex && m_iActionIndex < m_SlotItemList.Count )
			{
				m_SlotItemList[ m_iActionIndex ].gameObject.SetActiveRecursively(false);
			}
			int iPreActionIndex = m_iActionIndex;
			m_iActionIndex = Random.Range( 0, m_SlotItemList.Count );
			
			if( iPreActionIndex == m_iActionIndex )
			{
				if( m_iActionIndex == m_SlotItemList.Count-1 )
				{
					m_iActionIndex -= 1;
				}
				else
				{
					m_iActionIndex += 1;
				}
			}
			
			if( m_SlotItemList.Count <= m_iActionIndex || m_iActionIndex < 0 )
			{
				m_iActionIndex = 0;
			}
			
			m_SlotItemList[ m_iActionIndex ].gameObject.SetActiveRecursively(true);
			
			if( null != m_SlotItemList[ m_iActionIndex ].tempItem )
			{
				m_sbTemp.Length = 0;
				m_sbTemp.Append( m_SlotItemList[ m_iActionIndex ].tempItem.ItemData.GetGradeColor().ToString() );
				m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(  m_SlotItemList[ m_iActionIndex ].tempItem.ItemData.nameId ) );
				textItemName.Text = m_sbTemp.ToString();
			}
			else
			{
				textItemName.Text = string.Empty;
			}
			
			m_fSpeed = 0f;
		}
		
		m_fMaxSpeed -= (Time.deltaTime*speedAdd);
		if( m_fMaxSpeed < MinSpeed )
			m_fMaxSpeed = MinSpeed;
		
		m_fShowTextTime += Time.deltaTime;
		if( m_fShowTextTime > 0.7f )
		{
			m_fShowTextTime = 0f;
			if( false == textShow.gameObject.active )
			{
				textShow.gameObject.SetActiveRecursively(true);
			}
			else
			{
				textShow.gameObject.SetActiveRecursively(false);
			}
		}
		
		if( fTotalTime <= m_fTime )
		{
			if( m_iActionIndex < m_SlotItemList.Count )
				m_SlotItemList[ m_iActionIndex ].gameObject.SetActiveRecursively(false);
			
			CloseState();
			m_isNeedAction = false;
		}
	}
	
	public override void  CloseState()
	{
		
		if( null != m_AudioSources )
			m_AudioSources.Stop();

		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6034_EFF_Random_Complete", Vector3.zero, false);
		
		progress.Value = 1.0f;
		m_isNeedAction = false;
		itemReceivePos.gameObject.active =true;
		if( null != m_SelfSlotItem && null != m_SelfSlotItem.tempItem )
		{
			m_sbTemp.Length = 0;
			m_sbTemp.Append( m_SelfSlotItem.tempItem.ItemData.GetGradeColor().ToString() );
			m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(  m_SelfSlotItem.tempItem.ItemData.nameId ) );
			textItemName.Text = m_sbTemp.ToString();
			
			m_SelfSlotItem.gameObject.SetActiveRecursively(true);
		}
		else
		{
			textItemName.Text = string.Empty;
		}
		
		
		textResult.gameObject.SetActiveRecursively(true);
		SetStrengthenIngEffect( false,  Vector3.zero );
		SetStrengthenSuccEffect(true);
		itemImgPos.gameObject.SetActiveRecursively(false);
		progress.gameObject.SetActiveRecursively(false);
		textShow.gameObject.SetActiveRecursively(false);
		btnOk.gameObject.SetActiveRecursively(true);
		
		
		if( null != m_ReceiveRealItem )
		{
			switch( m_ReceiveRealItem.item.ItemData.grade )
			{
			case Item.eGRADE.Epic:
				epicImg.SetActiveRecursively(true);
				break;
				
			case Item.eGRADE.Normal:
				normalImg.SetActiveRecursively(true);
				break;
				
			case Item.eGRADE.Rare:
				rareImg.SetActiveRecursively(true);
				break;
				
			case Item.eGRADE.Magic:
				magicImg.SetActiveRecursively(true);
				break;
				
			case Item.eGRADE.Ark:
				if( null != arkImg )
					arkImg.SetActiveRecursively( true );
				break;
			}
		}
		
		if( true == AsHudDlgMgr.Instance.IsOpenInven && m_iSlotIndex >= Inventory.useInvenSlotBeginIndex && null != m_ReceiveRealItem )
		{
			AsHudDlgMgr.Instance.invenDlg.SetSlotItem( m_iSlotIndex, m_ReceiveRealItem );					
		}	
		
		isCanUseDlg = false;	
		AsEventNotifyMgr.Instance.PlayRandItemNotify();
		
	}
	
	
	public void GuiInputUp(Ray inputRay)
    {		
		if( true == m_isNeedAction &&  true == AsUtil.PtInCollider( touchCollider, inputRay, false ) )
		{
			CloseState();
		}
		else if( false == m_isNeedAction && true == itemReceivePos.gameObject.active && true == AsUtil.PtInCollider( itemReceivePos.collider, inputRay, false ) )
		{
			if( null != m_ReceiveRealItem )
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_ReceiveRealItem );
		}
	}

	void Update()
	{
		Updating();
	}
}
