using UnityEngine;
using System.Collections;
using System.Globalization;


public class ProductionTechItemCash : ProductionTechnologyItem
{
	public UIButton btnCashLearn;
	public UIButton btnCash;
	public SpriteText textBtn;

	private ulong m_iOpenCashCost = 0;
	System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();

	public override void Open( eITEM_PRODUCT_TECHNIQUE_TYPE _eType, sPRODUCT_INFO _info)
	{
		base.Open( _eType, _info);

		SetTechName( GetName(_eType));
	}

	public void SendCashLearn()
	{
		if( m_iOpenCashCost > AsUserInfo.Instance.SavedCharStat.nGold)
		{
			AsHudDlgMgr.Instance.CloseProductionDlg();
			AsHudDlgMgr.Instance.OpenNeedGoldDlg();
			return;
		}

		AsCommonSender.SendItemProductCashTechniqueOpen( getProductTechType);
	}

	private void CashLearnBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox)
				return;

			string strTitle = AsTableManager.Instance.GetTbl_String(126);
			m_sbTemp.Length = 0;
			m_sbTemp.AppendFormat( AsTableManager.Instance.GetTbl_String(263), m_iOpenCashCost);
			m_sbTemp.Append( "\n");
			m_sbTemp.Append( GetTypeString( getProductTechType));

			AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.MessageBox( strTitle, m_sbTemp.ToString(), this, "SendCashLearn",
				AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION));

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}

	// Use this for initialization
	void Start()
	{
		textBtn.Text = AsTableManager.Instance.GetTbl_String(1013);
		btnCashLearn.SetInputDelegate( CashLearnBtnDelegate);
		btnCash.SetInputDelegate( CashLearnBtnDelegate);

		m_iOpenCashCost = GetTechCost();
		btnCash.Text = m_iOpenCashCost.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	// Update is called once per frame
	void Update()
	{
	}
}
