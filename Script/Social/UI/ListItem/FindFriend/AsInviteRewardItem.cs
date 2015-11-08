using UnityEngine;
using System.Collections;

public class AsInviteRewardItem : MonoBehaviour {
	eGAME_INVITE_PLATFORM m_eGAME_INVITE_PLATFORM;
//	private bool m_isFacebook = false;
	private int m_id = 0;
	public UIButton m_RewardBtn = null;
	public SpriteText m_ItemNameText = null;
	public SpriteText m_MessageText = null;
	public SpriteText m_CountText = null;
	private UISlotItem m_RewardSlotItem = null;
	
	public Color m_FbMsgColor = new Color( 0.3f, 0.6f, 1f, 1f);
	public Color m_SmsMsgColor = new Color( 0.7f, 0.8f, 0.4f, 1f);
	private Item m_RewardItem = null;
	public SimpleSprite m_RewarditemImgPos = null;

	public Vector2 minusItemSize;
	// Use this for initialization
	void Start () {
		m_RewardBtn.SetInputDelegate(RewardBtnDelegate);
		m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1568);
	}
	
	void Update()
	{
#if UNITY_EDITOR
		if( true == Input.GetMouseButtonDown( 0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
			
			if( m_RewardSlotItem == null || m_RewardItem == null)
				return;
	
			if( m_RewardSlotItem.iconImg.collider == null)
				return;

			if( true == AsUtil.PtInCollider( m_RewardSlotItem.iconImg.collider, ray))
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_RewardItem, false, -10.0f);		
		}
#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch( 0);
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( touch.position);
			if( TouchPhase.Began == touch.phase)
			{
				if( m_RewardSlotItem == null || m_RewardItem == null)
					return;
				if( m_RewardSlotItem.iconImg.collider == null)
					return;
				if( true == AsUtil.PtInCollider( m_RewardSlotItem.iconImg.collider, ray))
					TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_RewardItem, false, -10.0f);
				
			}
		}
#endif
	}
	
	public void SetFbRewardData(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_id = index;
		m_eGAME_INVITE_PLATFORM = eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_FACEBOOK; 
		
		
		m_RewardItem = ItemMgr.ItemManagement.GetItem( data.nFacebook_Reward_ItemIdx[m_id]);
		if( null == m_RewardItem)
		{
			Debug.Log("SetFbRewardData()::  null == m_RewardItem");
			return;
		}
		
		m_ItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem.ItemData.nameId);
		if( null != m_RewardSlotItem)
			GameObject.Destroy( m_RewardSlotItem.gameObject);
		m_RewardSlotItem = ResourceLoad.CreateItemIcon( m_RewardItem.GetIcon(), m_RewarditemImgPos, Vector3.back, minusItemSize, true);	
	
		if(data.nFacebook_Reward_ItemCount[m_id] > 1)
			m_CountText.Text = data.nFacebook_Reward_ItemCount[m_id].ToString();	
		else	
			m_CountText.Text = string.Empty;
	
		m_MessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 4063), data.nFacebook_Goal[m_id]);
		m_MessageText.Color = m_FbMsgColor;
		
		m_RewardBtn.gameObject.SetActiveRecursively( true);
		if( 1 == data.bFacebook_Reward[m_id])
		{
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_RewardBtn.controlIsEnabled = true;
			m_RewardBtn.spriteText.Color = Color.black;
		}
		else
		{
			if( data.nFacebook_Total >= data.nFacebook_Goal[m_id])//#20228.
				m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);

			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
		}
		
	}
	
	public void SetPnRewardData(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_id = index;
		m_eGAME_INVITE_PLATFORM = eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_SMS;
		
		
		m_RewardItem = ItemMgr.ItemManagement.GetItem( data.nSms_Reward_ItemIdx[m_id]);
		if( null == m_RewardItem)
		{
			Debug.Log("SetPnRewardData()::  null == m_RewardItem");
			return;
		}
		m_ItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem.ItemData.nameId);
		if( null != m_RewardSlotItem)
			GameObject.Destroy( m_RewardSlotItem.gameObject);
		m_RewardSlotItem = ResourceLoad.CreateItemIcon( m_RewardItem.GetIcon(), m_RewarditemImgPos, Vector3.back, minusItemSize, true);
	
		if(data.nSms_Reward_ItemCount[m_id] > 1)
			m_CountText.Text = data.nSms_Reward_ItemCount[m_id].ToString();	
		else
			m_CountText.Text = string.Empty;
		
	
		m_MessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 4066), data.nSms_Goal[m_id]);		
		m_MessageText.Color = m_SmsMsgColor;
		
		m_RewardBtn.gameObject.SetActiveRecursively( true);
		if( 1 == data.bSms_Reward[m_id])
		{
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_RewardBtn.controlIsEnabled = true;
			m_RewardBtn.spriteText.Color = Color.black;
		}
		else
		{
			if( data.nSms_Total >= data.nSms_Goal[m_id])//#20228.
					m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);

			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
		}
	}
	
	public void SetLobiRewardData(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_id = index;
		m_eGAME_INVITE_PLATFORM = eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LOBI;
		
		m_RewardItem = ItemMgr.ItemManagement.GetItem( data.nLobi_Reward_ItemIdx );
		if( null == m_RewardItem)
		{
			Debug.Log("SetLobiRewardData()::  null == m_RewardItem");
			return;
		}
		m_ItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem.ItemData.nameId);
		if( null != m_RewardSlotItem)
			GameObject.Destroy( m_RewardSlotItem.gameObject);
		m_RewardSlotItem = ResourceLoad.CreateItemIcon( m_RewardItem.GetIcon(), m_RewarditemImgPos, Vector3.back, minusItemSize, true);
	
		if(data.nSms_Reward_ItemCount[m_id] > 1)
			m_CountText.Text = data.nLobi_Reward_ItemCount.ToString();	
		else
			m_CountText.Text = string.Empty;
		
	
		m_MessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String(4120),data.nLobi_Goal);		
		m_MessageText.Color = m_SmsMsgColor;
		
		m_RewardBtn.gameObject.SetActiveRecursively( true);
		if(1 == data.bLobi_Reward)
		{
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_RewardBtn.controlIsEnabled = true;
			m_RewardBtn.spriteText.Color = Color.black;
		}
		else
		{
			if( 1 >= data.nLobi_Goal )//#20228.
					m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);

			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
		}
	}
	
	public void SetLineRewardData(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_id = index;
		m_eGAME_INVITE_PLATFORM = eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_LINE;
		
		m_RewardItem = ItemMgr.ItemManagement.GetItem( data.nLine_Reward_ItemIdx);
		if( null == m_RewardItem)
		{
			Debug.Log("SetLineRewardData()::  null == m_RewardItem");
			return;
		}
		m_ItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem.ItemData.nameId);
		if( null != m_RewardSlotItem)
			GameObject.Destroy( m_RewardSlotItem.gameObject);
		m_RewardSlotItem = ResourceLoad.CreateItemIcon( m_RewardItem.GetIcon(), m_RewarditemImgPos, Vector3.back, minusItemSize, true);
	
		if(data.nLine_Reward_ItemCount > 1)
			m_CountText.Text = data.nLine_Reward_ItemCount.ToString();	
		else
			m_CountText.Text = string.Empty;
		
	
		m_MessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 4121), data.nLine_Goal);		
		m_MessageText.Color = m_SmsMsgColor;
		
		m_RewardBtn.gameObject.SetActiveRecursively( true);
		if( 1 == data.bLine_Reward)
		{
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_RewardBtn.controlIsEnabled = true;
			m_RewardBtn.spriteText.Color = Color.black;
		}
		else
		{
			if( 1 >= data.nLine_Goal)//#20228.
					m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);

			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
		}
	}
	
	public void SetTwitterRewardData(int index, body_SC_GAME_INVITE_LIST_RESULT data)
	{
		m_id = index;
		m_eGAME_INVITE_PLATFORM = eGAME_INVITE_PLATFORM.eGAME_INVITE_PLATFORM_TWITTER;
		
		
		m_RewardItem = ItemMgr.ItemManagement.GetItem( data.nTwitter_Reward_ItemIdx[m_id]);
		if( null == m_RewardItem)
		{
			Debug.Log("SetTwitterRewardData()::  null == m_RewardItem");
			return;
		}
		m_ItemNameText.Text = AsTableManager.Instance.GetTbl_String( m_RewardItem.ItemData.nameId);
		if( null != m_RewardSlotItem)
			GameObject.Destroy( m_RewardSlotItem.gameObject);
		m_RewardSlotItem = ResourceLoad.CreateItemIcon( m_RewardItem.GetIcon(), m_RewarditemImgPos, Vector3.back, minusItemSize, true);
	
		if(data.nTwitter_Reward_ItemCount[m_id] > 1)
			m_CountText.Text = data.nTwitter_Reward_ItemCount[m_id].ToString();	
		else
			m_CountText.Text = string.Empty;
		
	
		m_MessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 4066), data.nTwitter_Goal[m_id]);		
		m_MessageText.Color = m_SmsMsgColor;
		
		m_RewardBtn.gameObject.SetActiveRecursively( true);
		if( 1 == data.bTwitter_Reward[m_id])
		{
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL);
			m_RewardBtn.controlIsEnabled = true;
			m_RewardBtn.spriteText.Color = Color.black;
		}
		else
		{
			if( data.nTwitter_Total >= data.nTwitter_Goal[m_id])//#20228.
					m_RewardBtn.Text = AsTableManager.Instance.GetTbl_String( 1566);

			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
		}
	}
	
	private void RewardBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
		
			AsCommonSender.SendGameInviteReward( ( int)m_eGAME_INVITE_PLATFORM, m_id);
			
			m_RewardBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			m_RewardBtn.controlIsEnabled = false;
			m_RewardBtn.spriteText.Color = Color.gray;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsCommonSender.SendGameInviteList(); //##20228.
		}
	}
	
}
