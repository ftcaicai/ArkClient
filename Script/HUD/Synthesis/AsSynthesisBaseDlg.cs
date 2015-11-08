using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public enum eSynthesisSlotType
{
	Target,
	RawMaterial,
};

public class AsSynthesisSlot
{
	public UIInvenSlot				invenSlot = null;
	public eSynthesisSlotType		slotType = eSynthesisSlotType.Target;
	public int							slotTypeSeq = 0;
}

public class AsSynthesisBaseDlg : MonoBehaviour 
{
	
	public SpriteText txtTitle;
	public SpriteText[] txtEnchantSlotName;
	
	public UIInvenSlot targetEnchantSlot;
	public UIInvenSlot[] enchantSlots;
	
	public UIButton btnApply;
	public UIButton btnClose;
	
	public Collider rectCollider;
	protected bool m_bInputDown = false;
	public SpriteText txtNeedGold;
	public AsUISynthesisSlider actionSlider;
	
	public GameObject[] effectLists;
	
	protected float m_iconSize = 5f;
	private AsMessageBox m_msgBox;

	protected List<AsSynthesisSlot> listSynthesisSlot = new List<AsSynthesisSlot>();
	
	public bool isMsgBox
	{
		get
		{
			return null != m_msgBox;
		}
	}
	
	protected bool IsUseEmerald( Item.eITEM_TYPE itemtype, Item.eGRADE _grade )
	{
		if( itemtype == Item.eITEM_TYPE.EquipItem )
		{
			if( Item.eGRADE.Normal == _grade || 
				Item.eGRADE.Magic == _grade ||
				Item.eGRADE.Rare == _grade )
			{
				return false;
			}						
		}	
		
		return true;
	}
	
	public void SetMsgBox( AsMessageBox _msg)
	{
		if( null != m_msgBox)
		{
			GameObject.DestroyObject( m_msgBox.gameObject);
		}

		m_msgBox = _msg;
	}

	public void CloseMsgBox()
	{
		if( null != m_msgBox)
		{
			GameObject.DestroyObject( m_msgBox.gameObject);
		}
	}
	
	protected void SetPlayEffect( bool _bool )
	{
		foreach( GameObject _obj in effectLists )
		{
			if( null == _obj )
				continue;
			
			_obj.SetActive( _bool );
		}
	}
	
	protected void SetNeedGold( int iGold )
	{
		SetNeedGold( iGold.ToString( "#,#0", CultureInfo.InvariantCulture) );
	}
	
	protected void SetNeedGold( string _str )
	{
		if( null == txtNeedGold )
			return;	
		
		txtNeedGold.Text = _str;
	}
	
	
	
	protected void SetEnableApply( bool _isEnable )
	{
		if( _isEnable )	
		{
			if( btnApply != null )
				btnApply.spriteText.Color = Color.black;
		}
		else
		{
			if( btnApply != null )
				btnApply.spriteText.Color = Color.gray;
		}

		if( btnApply != null )
			btnApply.controlIsEnabled = _isEnable;
	}
	
	public virtual void Open()
	{
		m_bInputDown = false;
		SetNeedGold(string.Empty);
		SetPlayEffect( false );
		SetEnableApply(false);

		if( actionSlider != null )
			actionSlider.SetAction( SendPacket );
	}
	
	public virtual void Close()
	{
		ResetSlotMoveLock();
		CloseMsgBox();
	}

	protected AsSynthesisSlot AddSynthesisSlot(UIInvenSlot _invenSlot , eSynthesisSlotType _type , int _seq = 0 )
	{
		AsSynthesisSlot	synthesisSlot = new AsSynthesisSlot();
		synthesisSlot.invenSlot 		= _invenSlot;
		synthesisSlot.slotType 		= _type;
		synthesisSlot.slotTypeSeq = _seq;

		listSynthesisSlot.Add (synthesisSlot);

		return synthesisSlot;
	}
	
	protected void ResetSlotMoveLock()
	{
		ItemMgr.HadItemManagement.Inven.ResetInvenSlotMoveLock();
		if( AsHudDlgMgr.Instance.IsOpenInven )
		{
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		}
	}

	protected void ResetSlotMoveLock(int _slotIndex , bool _isLock)
	{
		ItemMgr.HadItemManagement.Inven.SetSlotMoveLock(_slotIndex , _isLock);
		if( AsHudDlgMgr.Instance.IsOpenInven )
		{
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		}
	}

	public bool IsRect( Ray inputRay )
	{
		if( null == rectCollider )
			return false;
		
		return rectCollider.bounds.IntersectRay( inputRay );
	}	
	
	
	
	
	protected virtual void SetItemInSlot( RealItem _realItem )
	{
		if( null == enchantSlots[0].slotItem ) 	 
		{
			SetItemInSlot( enchantSlots[0], _realItem );
		}
		else if( null == enchantSlots[1].slotItem ) 	 
		{
			SetItemInSlot( enchantSlots[1], _realItem );
		}
		else
		{
			SetItemInSlot( enchantSlots[0], _realItem );
		}
	}
	 
	protected virtual void SetItemInSlot( UIInvenSlot _slot, RealItem _realItem )
	{
		if( null == _slot )
			return;
		
		if( null == _realItem )
			return;
		
		_slot.DeleteSlotItem();
		_slot.CreateSlotItem( _realItem, _slot.transform );
		_slot.ResetSlotItemLocalPosition(-0.5f);
		ItemMgr.HadItemManagement.Inven.SetSlotMoveLock( _realItem.getSlot, true );
		if( AsHudDlgMgr.Instance.IsOpenInven )
		{
			AsHudDlgMgr.Instance.invenDlg.ApplySlotMoveLock();
		}
	}
	
	public virtual void GuiInputDown(Ray inputRay)
	{ 
		if( false == IsRect( inputRay ) )
			return;
		
		m_bInputDown = true;
	}
	
	public virtual void GuiInputMove(Ray inputRay)
	{
		
	}
	
	public virtual void InputDown(Ray inputRay)
	{
		
	}
	
	public virtual void InputMove(Ray inputRay)
	{
		
	}
	
	public virtual void GuiInputDClickUp(Ray inputRay)
	{
		
	}
	
	public virtual void GuiInputUp(Ray inputRay)
	{ 	
		if( false == m_bInputDown )
			return;
		
		if( null != targetEnchantSlot && true == targetEnchantSlot.IsIntersect(inputRay) )
		{
			if( null != targetEnchantSlot.slotItem )
			{
				TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, targetEnchantSlot.slotItem.realItem );
			}
			return;
		}	
		
		if( null != enchantSlots )
		{
			foreach( UIInvenSlot _slot in enchantSlots )
			{
				if( null == _slot)
					continue;
				
				
				if( null == _slot.slotItem )
					continue;
				
				if( true == _slot.IsIntersect( inputRay ) )
				{				
					TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.normal, _slot.slotItem.realItem );
					return;
				}
			}	
		}
		
		m_bInputDown = false;		
	}
	
	
	public virtual bool SetInputUpRealItem( RealItem _realItem, Ray inputRay )
	{
		if( null == _realItem )
			return false;
		
		
		return false;
	}
	
	public virtual bool SetDClickRealItem( RealItem _realItem )
	{
		if( null == _realItem )
			return false;
		
		
		return false;
	}
	
	
	public virtual void ReceivePacket( body_SC_ITEM_MIX_RESULT _result)
	{
		
	}
	
	
	protected virtual void SendPacket()
	{
		
	}
}
