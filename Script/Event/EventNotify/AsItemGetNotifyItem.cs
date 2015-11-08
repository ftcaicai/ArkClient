using UnityEngine;
using System.Text;
using System.Collections;


public class AsItemGetNotifyItem : MonoBehaviour
{
	public SpriteText m_ItemNameText = null;
	public AsDlgBase m_BaseDlg;
	public SimpleSprite itemImgPos;
	public Vector2 minusItemSize;

	private bool m_bShowCommand = false;
	public bool ShowCommand
	{
		get { return m_bShowCommand; }
		set { m_bShowCommand = value;}
	}
//	private GameObject m_go = null;
	protected UISlotItem m_SlotItem = null;

	private Color m_curColor = Color.white;

	private float m_fShowTime = 0.0f;
	private Vector3 m_vWorldPosRevision;
	private Vector3 m_vUIPosRevision;
	private float m_fAlphaDecreaseSpeed = 2.0f; // default: 2.0f
//	private float m_fMoveSpeedY = 5.0f; // default: 5.0f
	private float m_DurationTime = 0.1f;
	private float m_fPlayTime = 0.5f;
	private float m_fStopTime = 0.5f;

	private const float POS_Y_OFFSET = 5.0f;
	protected const float MARGIN_OFFSET = 0.3f;

	// Use this for initialization
	void Start()
	{
//		m_fMoveSpeedY = AsTableManager.Instance.GetTbl_GlobalWeight_Record(46).Value / 1000.0f;
		m_DurationTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(45).Value / 1000.0f;
		m_fStopTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(47).Value / 1000.0f;
		m_fAlphaDecreaseSpeed = ( 1.0f / m_DurationTime);
	}

	GameObject m_ParentObj;
	public GameObject ParentObject
	{
		get { return m_ParentObj; }
		//set { m_ParentObj = value;}
	}
	
	protected void UpdateLogic()
	{
		if( ( Time.time - m_fShowTime) < m_fStopTime)
		{
			m_fPlayTime = Time.time;
			return;
		}

		if( ( Time.time - m_fPlayTime) > m_DurationTime)
		{
			Remove();
			return;
		}

		m_curColor.a -= ( Time.deltaTime * m_fAlphaDecreaseSpeed);
		SetColor( m_curColor);
	}
	// Update is called once per frame
	void Update()
	{
		UpdateLogic();
	}

	void SetColor( Color color)
	{
		m_ItemNameText.SetColor( color);
		itemImgPos.gameObject.renderer.material.SetColor( "_Color", color);
		m_BaseDlg.SetColor( color);

		if( null != m_SlotItem)
			m_SlotItem.iconImg.gameObject.renderer.material.SetColor( "_Color", color);
	}

	public void Show()
	{
		m_bShowCommand = true;
		gameObject.SetActiveRecursively( m_bShowCommand);
		m_fShowTime = Time.time;
	}

	public void SetData( int itemId, GameObject go, bool isIconCollider = true )
	{
		Item item = ItemMgr.ItemManagement.GetItem( itemId);
		if( null == item)
			return;

		float fTextWidth = m_ItemNameText.GetWidth( AsTableManager.Instance.GetTbl_String( item.ItemData.nameId));
		ChangePanelSize( fTextWidth);

		StringBuilder sbItemName = new StringBuilder();
		sbItemName.Insert( 0, item.ItemData.GetGradeColor().ToString());
		sbItemName.AppendFormat( "{0}", AsTableManager.Instance.GetTbl_String( item.ItemData.nameId));
		m_ItemNameText.Text = sbItemName.ToString();

		if( null != m_SlotItem)
			GameObject.Destroy( m_SlotItem.gameObject);

		m_SlotItem = ResourceLoad.CreateItemIcon( item.GetIcon(), itemImgPos, Vector3.back, minusItemSize, isIconCollider );

		m_ParentObj = go;
		Debug.Log( "SetData" + transform.position);
		m_bShowCommand = false;
		gameObject.SetActiveRecursively( m_bShowCommand);
	}

	private void ChangePanelSize( float fTextWidth)
	{
		Vector3 pos = m_ItemNameText.transform.localPosition;
		pos.x = -( fTextWidth * 0.5f) + ( itemImgPos.width * 0.5f);
		m_ItemNameText.transform.localPosition = pos;

		pos = itemImgPos.transform.localPosition;
		pos.x = -( ( fTextWidth * 0.5f) + MARGIN_OFFSET);
		itemImgPos.transform.localPosition = pos;
		m_BaseDlg.width = fTextWidth + itemImgPos.width;
		m_BaseDlg.Assign();
	}

	public void OnDisable()
	{
		if( true == m_bShowCommand)
			transform.position = Vector3.zero;
	}

	public void Remove()
	{
		DestroyImmediate( ParentObject);
	}
}
