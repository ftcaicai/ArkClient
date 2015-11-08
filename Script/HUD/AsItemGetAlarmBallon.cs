using UnityEngine;
using System.Collections;

public class AsItemGetAlarmBallon : AsUIItemGetEquipAlram
{	
	public AsDlgBase m_ArkDlg;
	public AsDlgBase m_EpicDlg;
	public AsDlgBase m_RareDlg;
	public AsDlgBase m_NormalDlg;
	
	public Vector3 localPos;
	//public SpriteText textItemName;
	//public SpriteText textRankPont;
	//public SimpleSprite imgIcon;

	private AsUserEntity entity = null;
	//private Item m_item;
	//private sITEM m_sitem;
	private float m_GAP_TIME;
	private float m_fTime = 0;

	//public Collider tooltipCollider;
	//private UISlotItem m_SlotItem = null;
	private bool m_isChatMsg = false;
	
	

	public AsUserEntity Owner
	{
		set	{ entity = value; }
	}
	
	
	public override void Open (sITEM _sitem, AsUserEntity _entity)
	{
		Item _item = ItemMgr.ItemManagement.GetItem( _sitem.nItemTableIdx);
		if( null == _item )
			return;		
						
		switch( _item.ItemData.grade )
		{
		case Item.eGRADE.Ark:			
			m_BaseDlg = m_ArkDlg;
			break;
			
		case Item.eGRADE.Epic:			
			m_BaseDlg = m_EpicDlg;
			break;
			
		case Item.eGRADE.Rare:			
			m_BaseDlg = m_RareDlg;
			break;
			
		default:
			m_BaseDlg = m_NormalDlg;
			break;
		}			
		
		
		base.Open (_sitem, _entity);
		
		
		switch( _item.ItemData.grade )
		{
		case Item.eGRADE.Ark:				
			m_EpicDlg.gameObject.SetActiveRecursively(false);
			m_RareDlg.gameObject.SetActiveRecursively(false);
			m_NormalDlg.gameObject.SetActiveRecursively(false);
			break;
			
		case Item.eGRADE.Epic:			
			m_ArkDlg.gameObject.SetActiveRecursively(false);			
			m_RareDlg.gameObject.SetActiveRecursively(false);
			m_NormalDlg.gameObject.SetActiveRecursively(false);
			break;
			
		case Item.eGRADE.Rare:			
			m_ArkDlg.gameObject.SetActiveRecursively(false);
			m_EpicDlg.gameObject.SetActiveRecursively(false);			
			m_NormalDlg.gameObject.SetActiveRecursively(false);
			break;
			
		default:
			m_ArkDlg.gameObject.SetActiveRecursively(false);
			m_EpicDlg.gameObject.SetActiveRecursively(false);
			m_RareDlg.gameObject.SetActiveRecursively(false);			
			break;			
		}			
	}
	// Use this for initialization
	void Start()
	{
		m_GAP_TIME = AsTableManager.Instance.GetTbl_GlobalWeight_Record(114).Value;
	}

	// Update is called once per frame
	void Update()
	{
		if( null == entity)
		{
			GameObject.Destroy( gameObject);
			return;
		}

		if( null == entity.ModelObject)
			return;

		Vector3 worldPos = entity.ModelObject.transform.position;
		worldPos.y += entity.characterController.height;
		Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
		vRes.x += localPos.x;
		vRes.y += localPos.y;
		vRes.z = localPos.z;
		gameObject.transform.position = vRes;

		m_fTime += Time.deltaTime;

		if( m_GAP_TIME <= m_fTime)
		{
			GameObject.Destroy( gameObject);
			return;
		}

		if( Input.touchCount == 1 || Input.GetMouseButtonDown(0))
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);

#if !UNITY_EDITOR
			if( Input.GetTouch(0).phase != TouchPhase.Began)
				return;
#endif

			if( m_SlotItem == null || m_sitem == null)
				return;

			if( tooltipCollider == null)
				return;

			if( true == AsUtil.PtInCollider( tooltipCollider, ray))
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, m_sitem, -10.0f);

//				if( false == m_isChatMsg)
//				{
//					string strRes = AsTableManager.Instance.GetTbl_String(17335);
//					if( 0 == Random.Range( 0, 2))
//						strRes = AsTableManager.Instance.GetTbl_String(17336);
//
//					body_CS_CHAT_MESSAGE chat = new body_CS_CHAT_MESSAGE( strRes, eCHATTYPE.eCHATTYPE_PUBLIC);
//					byte[] data = chat.ClassToPacketBytes();
//					AsNetworkMessageHandler.Instance.Send( data);
//
//					m_isChatMsg = true;
//				}
			}
		}
	}
}
