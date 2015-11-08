
using UnityEngine;
using System.Collections;

public class UISlotItem : MonoBehaviour 
{
	public enum eSLOT_TYPE
	{
		ICON,
		BUFF_ICON,
		USE_COOLTIME,
		NO_COOLTIME,
	}
	public SimpleSprite iconImg;
	public AsIconCooltime coolTime;	
	public SpriteText itemCountText;	
	public bool isUseRealItem = true;
	public eSLOT_TYPE slotType = eSLOT_TYPE.ICON;
	
	public Item tempItem;
	
	private RealItem m_RealItem; 
	private bool m_bCoolTimeShow = true;
	
	
	// Set 
	public void SetItemCountText( int iCount )
	{
		if( null == itemCountText )
		{
			Debug.LogError("UISlotItem::SetItemCountText() [ null == itemCountText ]");
			return;
		}
		
		if( 2 > iCount )
		{
//			itemCountText.gameObject.active = false;
			itemCountText.Text = string.Empty;
			//Debug.Log("UISlotItem::SetItemCountText(1): iCount = "  + iCount.ToString());
		}
		else
		{
//			itemCountText.gameObject.active = false;
			itemCountText.gameObject.active = true;
			itemCountText.Text = iCount.ToString();
			//Debug.Log("UISlotItem::SetItemCountText(2): iCount = "  + iCount.ToString());
		}
	}	
	
	
	public void SetItem( RealItem realItem )
	{
		if( null == realItem || false == isUseRealItem )
		{
			Debug.LogError("UISlotItem::SetItem() [ null == realItem ]");
			return;
		}		
		
		m_RealItem = realItem;		
		SetItemCountText( m_RealItem.sItem.nOverlapped );	
		UpdateCoolTime();
		ShowCoolTime( true );		
	}
	
	
	public void ShowCoolTime( bool bShow )
	{
		if( eSLOT_TYPE.NO_COOLTIME == slotType )
			return;
		
		m_bCoolTimeShow = bShow;
		
		if( eSLOT_TYPE.BUFF_ICON == slotType )
			return;
		
		if( null == coolTime)
		{
			Debug.LogError ("UISlotItem::ShowCoolTime() [ null == coolTime ] " );
			return;
		}

		if (false == bShow)
			coolTime.gameObject.SetActive(bShow);
	}
	
	public void SetCooolTimeValue( float fValue )
	{	
		if( eSLOT_TYPE.NO_COOLTIME == slotType )
			return;
		
		if( eSLOT_TYPE.BUFF_ICON == slotType )
			return;
		
		if( null == coolTime)
		{
			Debug.LogError ("UISlotItem::SetCoolTimeValue() [ null == coolTime ] " );
			return;
		}
		
		if ( false == coolTime.gameObject.active )
		{
			coolTime.gameObject.active = true;
		}
		
		coolTime.Value = fValue;
	}
	
	// get	
	public RealItem realItem
	{
		get
		{
			return m_RealItem;
		}
	}
	
	
	
	
	public void UpdateCoolTime()
	{
		if( eSLOT_TYPE.NO_COOLTIME == slotType )
			return;
		
		if( eSLOT_TYPE.BUFF_ICON == slotType )
			return;
		
		if( eSLOT_TYPE.USE_COOLTIME == slotType )
			return;
		
		if( false == isUseRealItem )
			return;
		
		if( false == m_bCoolTimeShow )
			return;
		
		if( null == coolTime )
		{			
			return;
		}
		if( null == m_RealItem )
		{
			coolTime.gameObject.active = false;
			return;
		}
		
		if( !( 	Item.eITEM_TYPE.ActionItem == m_RealItem.item.ItemData.GetItemType() || 
				Item.eITEM_TYPE.UseItem == m_RealItem.item.ItemData.GetItemType() ) )
			return;
		
		if( false == m_RealItem.IsCanCoolTimeActive() )
		{
			coolTime.gameObject.active = false;
			return;
		} 
		else if ( false == coolTime.gameObject.active )
		{
			coolTime.gameObject.active = true;
		}
		
		coolTime.Value = m_RealItem.getCoolTimeGroup.getCoolTimeValue;
	}
	
	void Awake()
    {
		itemCountText.text = "";
		AsLanguageManager.Instance.SetFontFromSystemLanguage( itemCountText); // ilmeda, 20120822
		UpdateCoolTime();
	}
	
	void Start()
	{
		if(null== iconImg)
		{
			iconImg = gameObject.GetComponentInChildren<SimpleSprite>();
		}
		
		if( eSLOT_TYPE.NO_COOLTIME == slotType )
		{
			if(null != coolTime)
			{
				GameObject.Destroy( coolTime.gameObject );
			}
		}
		else
		{
			if(null== coolTime)
			{
				coolTime = gameObject.GetComponentInChildren<AsIconCooltime>();
			}
		}
		
		
		if(null== itemCountText)
		{
			itemCountText = gameObject.GetComponentInChildren<SpriteText>();
		}

	}
		
	void Update () 
	{
		UpdateCoolTime();		
	}
	
}