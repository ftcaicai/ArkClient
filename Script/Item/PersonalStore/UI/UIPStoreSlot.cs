using UnityEngine;
using System.Collections;
using System.Globalization;

public enum ePStoreSlotState {}

public class UIPStoreSlot : MonoBehaviour
{
	#region - member -
	public enum eState {Blank, Filled, Closed}

	[SerializeField] int m_Idx;
	public int slotIndex	{ get { return m_Idx; } }
	[SerializeField] Transform m_IconSlot;
	public Transform IconSlot	{ get { return m_IconSlot; } }
	[SerializeField] Transform m_LockedSlot;
	public Transform LockedSlot	{ get { return m_LockedSlot; } }
	[SerializeField] SpriteText textName;
	string m_TextName;
	public string TextName	{ get { return m_TextName; } }
	[SerializeField] SpriteText textGold;
	[SerializeField] SimpleSprite spriteCoin;
	[SerializeField] SimpleSprite spriteBg;
	[SerializeField] SimpleSprite spriteSelected;

	UISlotItem m_SlotItem;
	public UISlotItem slotItem	{ get { return m_SlotItem; } }

	eState m_State;
	public eState state	{ get { return m_State; } }

	int m_InvenIdx;
	public int InvenIdx	{ get { return m_InvenIdx; } }
	ulong m_Gold;
	public ulong gold	{ get { return m_Gold; } }

	UIListItemContainer listContatiner;

	bool m_Initialized = false;
	bool m_Selected = false;

	public Color colorStrength = Color.white;
	#endregion

	#region - init & update -
	void Awake()
	{
		listContatiner = transform.parent.GetComponent<UIListItemContainer>();

		m_Gold = 0;

		if( textName != null && textGold != null)
		{
			textName.Text = "";
			m_TextName = "";
			textGold.Text = "";
		}
	}

	public void Init( int _idx)
	{
		m_Idx = _idx;
	}

	// Use this for initialization
	void Start()
	{
		m_Initialized = true;

//		Debug.Log( "UIPStoreSlot::Start: spriteSelected = " + spriteSelected);
//		Debug.Log( "UIPStoreSlot::Start: m_SlotItem = " + m_SlotItem);

		if( spriteSelected != null)
			spriteSelected.gameObject.SetActiveRecursively( false);
//		else
//			Debug.LogWarning( "UIPStoreSlot::Start: spriteSelected is " + spriteSelected);

		if( m_SlotItem != null)
			m_SlotItem.ShowCoolTime( false);
//		else
//			Debug.LogWarning( "UIPStoreSlot::Start: m_SlotItem is " + m_SlotItem);

//		if( m_LockedSlot != null)
//			m_LockedSlot.gameObject.SetActiveRecursively( false);
//		else
//			Debug.LogWarning( "UIPStoreSlot::Start: m_LockedSlot is " + m_LockedSlot);

		OnEnable();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnEnable()
	{
		if( m_Initialized == true)
		{
			if( spriteSelected != null)
			{
				if( m_Selected == true)
					spriteSelected.gameObject.SetActiveRecursively( true);
				else
					spriteSelected.gameObject.SetActiveRecursively( false);
			}

			switch( m_State)
			{
			case eState.Blank:
				Clear();
				break;
			case eState.Closed:
				SetClosed();
				break;
			case eState.Filled:
				RefreshFilled();
				break;
			}
		}
	}
	#endregion

	#region - collider -
	public bool IsIntersect( Ray ray)
	{
		if( null == collider)
		{
			Debug.LogError( "InvenSlot::IsIntersect() [ null == collider ]");
			return false;
		}

//		return collider.bounds.IntersectRay( ray);
		bool result = false;
		collider.enabled = true;
		result = AsUtil.PtInCollider( collider, ray);
		collider.enabled = false;
		return result;
	}
	#endregion

	#region - slot creation -
	public bool CreateSlotItem( body_SC_PRIVATESHOP_REGISTRATION_ITEM _item, Transform trmParent = null)
	{
		if( null == _item)
		{
			Debug.LogError( "UIPStoreSlot::CreateSlotItem() [ null == _item ]");
			return false;
		}

		return _CreateSlotItem( _item.sPrivateShopItem, _item.nInvenSlot, _item.nItemSellGold, trmParent);
	}

	public bool CreateSlotItem( body2_SC_PRIVATESHOP_ITEMLIST _item, Transform trmParent = null)
	{
		if( null == _item)
		{
			Debug.LogError( "UIPStoreSlot::CreateSlotItem() [ null == _item ]");
			return false;
		}

		return _CreateSlotItem( _item.sItem, -1, _item.nItemGold, trmParent);
	}

	public bool CreateSlotItem( body2_SC_PRIVATESHOP_OWNER_ITEMLIST _item, Transform trmParent = null)
	{
		if( null == _item)
		{
			Debug.LogError( "UIPStoreSlot::CreateSlotItem() [ null == _item ]");
			return false;
		}

		return _CreateSlotItem( _item.sItem, _item.nInvenSlot, _item.nItemGold, trmParent);
	}

	bool _CreateSlotItem( sITEM _item, int _idx, ulong _gold, Transform _trnParent)
	{
		m_State = eState.Filled;

		RealItem realItem = new RealItem( _item, _idx);

		GameObject resGo = realItem.item.GetIcon();
		if( null == resGo)
		{
			Debug.LogError( "UIPStoreSlot::CreateSlotItem() [ null == resGo ] item id : " + realItem.item.ItemID);
			return false;
		}

		//-- instantiate image
		GameObject go = GameObject.Instantiate( resGo) as GameObject;
//		go.transform.parent = _trnParent;
		go.transform.parent = IconSlot;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		UISlotItem __slotItem = go.GetComponent<UISlotItem>();
		if( null == __slotItem)
		{
			Debug.LogError( "UIPStoreSlot::CreateSlotItem() [ null == slotItem");
			Destroy( go);
			return false;
		}

		//-- set on top scroll pos
		UIScrollList list = listContatiner.GetScrollList();
//		float curPos = m_Idx - 4; if( curPos < 0) curPos = 0;
//		list.ScrollListTo( curPos / 7f);//( ( float)AsHudDlgMgr.Instance.pstoreDlg.slotCount - 1f));

		float listHeight = list.viewableArea.y;
		Debug.LogWarning( "listContatiner.ClippingRect.height = " + listContatiner.ClippingRect.height);
		float unit_Y = 3.2f;
		float total = AsHudDlgMgr.Instance.pstoreDlg.slotCount;
		float length = ( ( unit_Y * total) + ( 0.01f * ( total - 1)) - listHeight);
		if( m_Idx > 4)
		{
//			float rest = listHeight - unit_Y * 5;
//			float rest = listHeight % ( unit_Y + 0.02f);
			float rest = 0.103f * length;
			float curPos = ( ( ( m_Idx - 5) * unit_Y)/* + ( ( m_Idx - 5) * 0.01f)*/ + rest) / length; if( curPos > 1) curPos = 1;
			list.ScrollListTo( curPos);
		}

		//-- set slot item info
		__slotItem.SetItem( realItem);
		m_SlotItem = __slotItem;
		m_SlotItem.ShowCoolTime( false);
		ResetSlotItemPosition();

//		Vector3 vColliderSize = m_IconSlot.collider.bounds.size;
//		m_SlotItem.iconImg.SetSize( vColliderSize.x, vColliderSize.y);
		float width = m_IconSlot.GetComponent<SimpleSprite>().width;
		float height = m_IconSlot.GetComponent<SimpleSprite>().height;
		m_SlotItem.iconImg.SetSize( width, height);

		m_InvenIdx = _idx;
		if( textName != null && textGold != null)
		{
			string str = m_SlotItem.realItem.item.ItemData.GetGradeColor() + AsTableManager.Instance.GetTbl_String( realItem.item.ItemData.nameId);
			if( m_SlotItem.realItem.sItem.nStrengthenCount > 0)
				str = colorStrength + "+" + m_SlotItem.realItem.sItem.nStrengthenCount + " " + str;
			textName.Text = str;
			m_TextName = str;
			textGold.Text = _gold.ToString( "#,#0", CultureInfo.InvariantCulture);
			m_Gold = _gold;
		}

		//-- scan child
		listContatiner.ScanChildren();
		listContatiner.GetScrollList().ClipItems();

		return true;
	}
	#endregion

	#region - slot operation -
	public void Clear()
	{
		m_State = eState.Blank;

		if( null != m_SlotItem)
		{
			DestroyImmediate( m_SlotItem.gameObject);
			m_SlotItem = null;

			listContatiner.ScanChildren();
		}

		if( textName != null && textGold != null)
		{
			textName.Text = "";
			m_TextName = "";
			textGold.Text = "";
		}

		if( m_LockedSlot != null)
			m_LockedSlot.gameObject.SetActiveRecursively( false);

		if( spriteCoin != null)
			spriteCoin.gameObject.SetActiveRecursively( true);

		if( spriteBg != null)
			spriteBg.renderer.material.SetColor( "_Color", Color.white);
	}

	public void SetClosed()
	{
		m_State = eState.Closed;

		Color color = Color.blue; color.a = 0.4f;

		if( m_LockedSlot != null)
			m_LockedSlot.gameObject.SetActiveRecursively( true);

		if( m_LockedSlot != null)
			m_LockedSlot.renderer.material.SetColor( "_Color", color);

		if( spriteCoin != null)
			spriteCoin.gameObject.SetActiveRecursively( false);

		if( spriteBg != null)
			spriteBg.renderer.material.SetColor( "_Color", color);
	}

	void RefreshFilled()
	{
		if( m_LockedSlot != null)
			m_LockedSlot.gameObject.SetActiveRecursively( false);

		if( m_SlotItem != null)
		{
			m_SlotItem.gameObject.SetActiveRecursively( true);
			m_SlotItem.ShowCoolTime( false);
		}
//		StartCoroutine( RefreshFilledProcess());
	}

//	IEnumerator RefreshFilledProcess()
//	{
//		while( true)
//		{
//			yield return null;
//
//			if( m_LockedSlot != null && m_SlotItem != null)
//			{
//				if( m_LockedSlot != null) m_LockedSlot.gameObject.SetActiveRecursively( false);
//				if( m_SlotItem != null)
//				{
//					m_SlotItem.gameObject.SetActiveRecursively( true);
//					m_SlotItem.ShowCoolTime( false);
//				}
//
//				break;
//			}
//		}
//	}

	public void SetMovingSlot( UIPStoreSlot _slot)
	{
		m_SlotItem = _slot.slotItem;
		m_Idx = _slot.slotIndex;
		m_InvenIdx = _slot.InvenIdx;

		m_TextName = _slot.TextName;
		m_Gold = _slot.gold;
	}

	public void SetSlotItem( UISlotItem _slotItem)
	{
		m_SlotItem = _slotItem;
	}
	#endregion

	#region - slot position -
	public void SetSlotItemPosition( Vector3 vec3Position)
	{
		if( null == m_SlotItem)
		{
			Debug.LogError( "UIPStoreSlot::MoveSlotItem() [ null == m_slotItem ]");
			return;
		}

		Vector3 vec3Temp = Vector3.zero;
		vec3Temp.x = vec3Position.x;
		vec3Temp.y = vec3Position.y;
//		vec3Temp.z = transform.position.z - 1.0f;
		vec3Temp.z = vec3Position.z;
		m_SlotItem.transform.position = vec3Temp;
	}

	public void ResetSlotItemPosition()
	{
		if( null == slotItem)
		{
			Debug.LogError( "UIPStoreSlot::ResetPosition() [ null == m_slotItem ]");
			return;
		}

		Vector3 vec3Temp = transform.position;
		vec3Temp.z -= 1.0f;
		m_SlotItem.transform.position = vec3Temp;
	}
	#endregion

	#region - slot activation -
	public void ActiveSelection()
	{
		if( m_Selected == false)
		{
			m_Selected = true;
			spriteSelected.gameObject.SetActiveRecursively( true);
		}
	}

	public void ReleaseSelection()
	{
		if( m_Selected == true)
		{
			m_Selected = false;
			spriteSelected.gameObject.SetActiveRecursively( false);
		}
	}
	#endregion
}

