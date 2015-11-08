using UnityEngine;
using System.Collections;

/* delegate */
public delegate void AreaMapBtnClickDelegate(int iMapId, int iWarpIdx );
public class UIAreaBtn : MonoBehaviour 
{	
	public UIButton btnMap;	
	public SpriteText textMapName;
	public int mapIdx;
	public int warpIdx;
	public AsNameBackImg textBackImg;
	
	private GameObject partyImg;	
	private Tbl_ZoneMap_Record m_ZoneMapRecord;
	/* delegate */
	private AreaMapBtnClickDelegate m_AreaMapBtnClickDelegate;	
	public  void SetAreaMapBtnClickDelegate(AreaMapBtnClickDelegate callback) 
	{
		m_AreaMapBtnClickDelegate = callback;
	}  
	/// dopamin
	public void Open()
	{
		m_ZoneMapRecord = AsTableManager.Instance.GetZoneMapRecord( mapIdx );
		if( null == m_ZoneMapRecord )
		{
			AsUtil.ShutDown("UIAreaBtn::Open()[ null == map ] map id : " + mapIdx );
			return;
		}
		
		textMapName.Text = AsTableManager.Instance.GetTbl_String( m_ZoneMapRecord.getTooltipStrIdx );
		if( null != textBackImg )
		{
			textBackImg.SetText( textMapName );
		}
		
	}
	
	public void OpenBtn()
	{	
		//btnMap.SetControlState(UIButton.CONTROL_STATE.NORMAL);
        btnMap.controlIsEnabled = true;
	}
	
	public void CloseBtn()
	{			
		//btnMap.SetControlState(UIButton.CONTROL_STATE.DISABLED);
		btnMap.controlIsEnabled = false;       
	}
	
	public void ClearPartyImg()
	{
		if( null != partyImg )
		{
			GameObject.DestroyObject( partyImg );
		}
	}
	
	public void SetPartyImg( string strPath )
	{
		ClearPartyImg();
		
		partyImg = ResourceLoad.CreateGameObject( strPath, transform );
		if( null == partyImg )
			return;
		float fImgHeight = 0f;
		float fBtnHeight = btnMap.height / 2f;
		
		SimpleSprite sprite = partyImg.GetComponentInChildren<SimpleSprite>();
		if( null != sprite )
		{
			fImgHeight = sprite.height / 2f;
		}		
		
		partyImg.transform.localPosition = new Vector3( 0f, (fImgHeight+fBtnHeight), 0.0f );
	}
	
	
	private void ClickBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			m_AreaMapBtnClickDelegate(mapIdx, warpIdx);	//dopamin
			//if( true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg ) 
			//{				
			//	AsHudDlgMgr.Instance.worldMapDlg.AreaMapBtnClick(mapIdx, warpIdx);				
			//}
			
			
		}
	}
	
	// Use this for initialization
	void Start () 
	{				
		btnMap.SetInputDelegate(ClickBtnDelegate);	
		Open();		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
