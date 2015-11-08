using UnityEngine;
using System.Collections;

public class AsInstantDungeonListItem : MonoBehaviour
{
	public SimpleSprite m_InDunImg = null;
	public SpriteText m_TextName;
	public SpriteText m_TextLevel;
//	public SpriteText m_TextCount;
	public GameObject m_goSelect;
	public GameObject m_goRaid;
	public int InDunID = 0;
	
	void Start()
	{
	}

	void Update()
	{
	}
	
	public void Init(int nInDunID, Tbl_InDun_Record record)
	{
		InDunID = nInDunID;

		Texture tex = ResourceLoad.Loadtexture( record.Icon);
		m_InDunImg.renderer.material.mainTexture = tex;

		m_TextName.Text = AsTableManager.Instance.GetTbl_String( record.Name);
//		m_TextLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1702), record.MinLv, record.MaxLv);
		m_TextLevel.Text = string.Format( AsTableManager.Instance.GetTbl_String( 2718), record.MinLv);
//		m_TextCount.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1682), "0");
		
		SetActiveSelect( false);

		if( 0 == record.RaidIcon)
			SetActiveRaidIcon( false);
		else
			SetActiveRaidIcon( true);
	}
	
	public void SetActiveSelect(bool bSelect)
	{
		m_goSelect.SetActive( bSelect);
	}

	public void SetActiveRaidIcon(bool bEnable)
	{
		m_goRaid.SetActive( bEnable);
	}
	
	public void OnBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != AsHudDlgMgr.Instance.m_InstantDungeonDlg)
		{
			AsHudDlgMgr.Instance.m_InstantDungeonDlg.ResetIndunListSelect();
			AsHudDlgMgr.Instance.m_InstantDungeonDlg.Set_InDunData( InDunID);
		}
		
		SetActiveSelect( true);
	}
}
