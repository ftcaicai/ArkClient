
using UnityEngine;
using System.Collections;


public class UIBuffSlot : MonoBehaviour
{
	//---------------------------------------------------------------------
	/* Public Variable */
	//---------------------------------------------------------------------
	public SpriteText timeText;
	public Collider buffTooltip;
	
	//---------------------------------------------------------------------
	/* Private Variable */
	//---------------------------------------------------------------------
	private UISlotItem m_Icon;
	private BuffBaseData m_BaseData;	
	
	private BuffTooltip m_tooltip;
	
	private float m_fXGap = 3f;

	//---------------------------------------------------------------------
	/* protected function*/
	//---------------------------------------------------------------------
	
	protected void CreateIcon( string tex)
	{
		GameObject go =  Resources.Load( tex) as GameObject;
		if( null == go)
		{
			Debug.LogError( "GameObject load failed [ name : " + tex);
			return;
		}
		
		DeleteIcon();
		
		GameObject goIns = GameObject.Instantiate( go) as GameObject;
		m_Icon = goIns.GetComponentInChildren<UISlotItem>();
		m_Icon.transform.parent = transform;
		m_Icon.transform.localPosition= Vector3.zero;
		m_Icon.transform.localRotation= Quaternion.identity;
		m_Icon.transform.localScale= Vector3.one;
		
		m_Icon.slotType = UISlotItem.eSLOT_TYPE.BUFF_ICON;
		m_Icon.isUseRealItem = false;
	}
	
	protected void DeleteIcon()
	{
		if( null != m_Icon)
		{
			GameObject.DestroyImmediate( m_Icon.gameObject);
			m_Icon = null;
		}
	}
	
	//---------------------------------------------------------------------
	/* Public function*/
	//---------------------------------------------------------------------
	
	public void OpenBuffSlot( BuffBaseData _buffData)
	{
		if( null == _buffData)
		{
			AsUtil.ShutDown( "UIBuffSlot::OpenBuffSlot()[ null == BuffBaseData ]");
			return;
		}
		
		m_BaseData = _buffData;
		
		timeText.gameObject.active = true;
		if( null != buffTooltip)
			buffTooltip.gameObject.active = true;
		CreateIcon( m_BaseData.getIconPath);
	}
	
	
	public void OffBuffSlot()
	{
		DeleteIcon();
		m_BaseData = null;
		CloseTooltip();
		timeText.Text = string.Empty;
		if( null != buffTooltip)
			buffTooltip.gameObject.active = false;
		timeText.gameObject.active = false;
	}
	
	public void CloseTooltip()
	{
		if( null != m_tooltip)
			GameObject.Destroy( m_tooltip.gameObject);
		m_tooltip = null;
	}
	
	private void ClickTooltip( Ray inputRay)
	{
		if( null == m_BaseData)
			return;
		
		if( null == buffTooltip || false == buffTooltip.gameObject.active)
			return;
		
		if( false == AsUtil.PtInCollider( buffTooltip, inputRay))
		{
			CloseTooltip();
			return;
		}
		
		if( null != m_tooltip)
			return;
		
		GameObject obj = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_BuffTooltip");
		if( null == obj)
			return;
		
		m_tooltip = obj.GetComponent< BuffTooltip>();
		if( null == m_tooltip)
			return;
		
		m_tooltip.Open( m_BaseData);
		
		Vector3 temp = m_tooltip.transform.position;
		temp.x = inputRay.origin.x + m_fXGap;
		temp.y = inputRay.origin.y;
		m_tooltip.transform.position = temp;
	}
	
	public void GuiInputDown( Ray inputRay)
	{
		ClickTooltip( inputRay);
	}
	
	public void InputMove( Ray inputRay)
	{
		/*if( null != m_tooltip)
		{
			Vector3 temp = m_tooltip.transform.position;
			temp.x = inputRay.origin.x + m_fXGap;
			temp.y = inputRay.origin.y;
			m_tooltip.transform.position = temp;
		}*/
	}
	
		
	//---------------------------------------------------------------------
	/* private Virtual */
	//---------------------------------------------------------------------
	
		
	//Awake
	void Awake()
	{
		timeText.Text = "";
		if( null != buffTooltip)
			buffTooltip.gameObject.active = false;
	}
	
	// Use this for initialization
	void Start()
	{
		OffBuffSlot();
		
		// < ilmeda, 20120822
		AsLanguageManager.Instance.SetFontFromSystemLanguage( timeText);
		// ilmeda, 20120822 >
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null != m_BaseData && null != m_Icon)
		{
			m_Icon.SetCooolTimeValue( m_BaseData.getCurCoolTime);
			//float remain = m_BaseData.getRemainCoolTime;
			/*if( 1.0f > remain)
				timeText.Text = string.Format( "{0:F1}", remain);
			else
				timeText.Text = string.Format( "{0:D}", ( int)remain);*/
			timeText.Text = AsMath.GetCoolTimeRemainTime( m_BaseData.getRemainCoolTime);
		}
	}
}
