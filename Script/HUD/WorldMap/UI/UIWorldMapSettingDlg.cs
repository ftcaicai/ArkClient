using UnityEngine;
using System.Collections;

public class UIWorldMapSettingDlg : MonoBehaviour 
{
	public static int s_alphaValue = 0;
	public static bool s_bigMap = true;
	
	public SpriteText textTitle;
	public SpriteText textMapSize;
	public UIRadioBtn btnBigMap;
	public UIRadioBtn btnSmallMap;
	public SpriteText textAlpha;
	public UIButton btnAlpha_left;
	public UIButton btnAlpha_right;
	public UIButton btnCancel;
	public UIButton btnClose;
	public UIButton btnOk;
	public SpriteText textAlphaValue;
	
	private int m_AlphaValue = 0;
	private bool m_isBigMap = false;
	
	public void Open()
	{		
		SetAlphaValueText(s_alphaValue);
		
		m_isBigMap = s_bigMap;
		if( true == s_bigMap )
		{
			btnBigMap.SetState( 0 );
			btnSmallMap.SetState( 1 ) ;
		}
		else			
		{
			btnBigMap.SetState( 1 );
			btnSmallMap.SetState( 0 ) ;
		}
	}
	
	private void SetAlphaValueText( int _alphaValue )
	{
		if( _alphaValue > 70 )
			_alphaValue = 70;
		if( _alphaValue < 0 )
			_alphaValue = 0;
		
		textAlphaValue.Text = _alphaValue.ToString() + "%";
		m_AlphaValue = _alphaValue;
	}
	
	
	private void BigMapBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			m_isBigMap = true;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
		}
	}
	
	private void SmallMapBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			m_isBigMap = false;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);			
		}
	}
	
	private void AlphaLeftBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			SetAlphaValueText( m_AlphaValue - 10 );
			
			if( true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg && true == AsHudDlgMgr.Instance.worldMapDlg.isOpenZoneMap )
			{
				AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().SetAlpahMap( m_AlphaValue );
			}
		}		
	}
	
	private void AlphaRightBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			SetAlphaValueText( m_AlphaValue + 10 );
			
			if( true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg && true == AsHudDlgMgr.Instance.worldMapDlg.isOpenZoneMap )
			{
				AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().SetAlpahMap( m_AlphaValue );
			}
		}
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			AsHudDlgMgr.Instance.CloseWorldMapSettingDlg();
			
			if( true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg && true == AsHudDlgMgr.Instance.worldMapDlg.isOpenZoneMap )
			{
				AsHudDlgMgr.Instance.worldMapDlg.GetZoneLogic().SetAlpahMap( s_alphaValue );
			}
		}
	}
	
	private void OkBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
            if (true == AsHudDlgMgr.Instance.IsOpenWorldMapDlg && true == AsHudDlgMgr.Instance.worldMapDlg.isOpenZoneMap && m_isBigMap != s_bigMap )
            {
                AsHudDlgMgr.Instance.worldMapDlg.SetWorldMapSize(m_isBigMap);
            }

			s_alphaValue = m_AlphaValue;
			s_bigMap = m_isBigMap;

            PlayerPrefs.SetInt("worldalpha", s_alphaValue );
            PlayerPrefs.SetInt("worldbig", s_bigMap == true ? 1 : 0);
			PlayerPrefs.Save();
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);	
			AsHudDlgMgr.Instance.CloseWorldMapSettingDlg();

           
		}
	}
	
	void Awake()
	{ 
		textTitle.Text = AsTableManager.Instance.GetTbl_String(987);
		textMapSize.Text = AsTableManager.Instance.GetTbl_String(988);
		btnBigMap.Text = AsTableManager.Instance.GetTbl_String(989);
		btnSmallMap.Text = AsTableManager.Instance.GetTbl_String(990);
		textAlpha.Text = AsTableManager.Instance.GetTbl_String(991);
		btnCancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		btnOk.Text = AsTableManager.Instance.GetTbl_String(1152);
		
		
		btnBigMap.SetInputDelegate(BigMapBtnDelegate);	
		btnSmallMap.SetInputDelegate(SmallMapBtnDelegate);	
		btnAlpha_left.SetInputDelegate(AlphaLeftBtnDelegate);	
		btnAlpha_right.SetInputDelegate(AlphaRightBtnDelegate);	
		btnCancel.SetInputDelegate(CancelBtnDelegate);
		btnClose.SetInputDelegate(CancelBtnDelegate);
		btnOk.SetInputDelegate(OkBtnDelegate);
		
		
		btnBigMap.SetGroup(1);
		btnSmallMap.SetGroup(1); 
		
	}
	
	
	
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
