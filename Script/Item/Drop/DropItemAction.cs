using UnityEngine;
using System.Collections;


[AddComponentMenu("Drop/DropItemAction")]
public class DropItemAction : MonoBehaviour 
{

    //---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------	
	//private bool m_bSendPacket = false;	
	//public GameObject shadow = null;
	private string strAniName = "Action";
	private AS_body2_SC_DROPITEM_APPEAR m_DropData;
	public GameObject rootNode;
	
	private float m_fTime = 0f;
	private bool m_isAuto = true;
    //---------------------------------------------------------------------
    /* function */
    //---------------------------------------------------------------------	

    public void Init( AS_body2_SC_DROPITEM_APPEAR dropItem )
    {
        m_DropData = dropItem;
    }
	
	
	public AS_body2_SC_DROPITEM_APPEAR getDropItemData
	{
		get	{ return m_DropData; }
	}


    //---------------------------------------------------------------------
    /* virtual */
    //---------------------------------------------------------------------


	// Use this for initialization
	void Start () 
	{
		if( null != animation )
		{
			if( null != animation[ strAniName ] )
			{
				animation.CrossFade(strAniName);	
			}
			else
			{
				Debug.LogError("animation no have [ name : " + strAniName + " gameobject name : " + gameObject.name );
			}
		}
		
		if( null != audio )
			audio.Play();
		
		/*if( null != collider )
		{
			collider.enabled = false;
		}*/
	}
	
	// Update is called once per frame
	void Update () 
    {     
		m_fTime += Time.deltaTime;
		if( m_fTime > 1.5f && true == m_isAuto )
		{		
			if( null != m_DropData )
			{
				if( true == IsCanSendPickDrop(m_DropData.nItemTableIdx) || true == AsPartyManager.Instance.IsPartying )		
				{
					AsCommonSender.SendPickupItem(m_DropData.nDropItemIdx, false);
				}
				else
				{
					AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(20), eCHATTYPE.eCHATTYPE_SYSTEM);
				}
			}
			
			m_isAuto = false;
		}
	}
	
	private bool IsAnimationPlay()
	{
		if( null == animation )
			return false;
		
		return animation.isPlaying;
	}
	
	private bool IsCanSendPickDrop( int iItemID )
	{
		Item _item = ItemMgr.ItemManagement.GetItem( iItemID );
		if( null == _item )
			return false;
		
		if( Item.eITEM_TYPE.CosEquipItem == _item.ItemData.GetItemType() || Item.eITEM_TYPE.EquipItem == _item.ItemData.GetItemType() )
		{
			if( -1 == ItemMgr.HadItemManagement.Inven.GetEmptyInvenSlot() )
				return false;
		}
		
		return true;
	}
	
	public void OnTriggerEnter(Collider collider)
    {        
		if( null == m_DropData )
			return;
		
		if( false == IsCanSendPickDrop(m_DropData.nItemTableIdx) )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(20), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}
        ItemMgr.DropItemManagement.OnTriggerEnter( m_DropData.nDropItemIdx );		      
    }
	
	public void OnTriggerExit(Collider collider)
    {    
		if( null == m_DropData )
			return;
		
		if( false == IsCanSendPickDrop(m_DropData.nItemTableIdx) )
		{
			AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String(20), eCHATTYPE.eCHATTYPE_SYSTEM);
			return;
		}
        ItemMgr.DropItemManagement.OnTriggerEnter( m_DropData.nDropItemIdx );		      
    }
}
