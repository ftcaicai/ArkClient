using UnityEngine;
using System.Collections;
using System.Text;


public class ProductionTechItemDef : ProductionTechnologyItem
{
	public UIButton btnLearn;
	public SpriteText textBtn;
	
//	private StringBuilder m_sbLearn = new StringBuilder();
	
	public void SendLearn()
	{
		AsCommonSender.SendItemProductTechniqueRegister(getProductTechType);
		AsHudDlgMgr.Instance.productRadioIndex = getProductTechType;
	}
	
	private void LearnBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( false != AsHudDlgMgr.Instance.productionDlg.isOpenMessageBox )
				return;
			
			
			string strTitle = AsTableManager.Instance.GetTbl_String( 126 );
			
			//m_sbLearn.Remove( 0, m_sbLearn.Length );
			//m_sbLearn.Append( string.Format( AsTableManager.Instance.GetTbl_String( 262 ), GetName(getProductTechType) ) );
			//m_sbLearn.Append( GetTypeString( getProductTechType ) );
			//m_sbLearn.Append( "\n" );
			//m_sbLearn.Append( Color.red );
			//m_sbLearn.Append( AsTableManager.Instance.GetTbl_String( 1334 ) );
			
			
			//string strText = string.Format( AsTableManager.Instance.GetTbl_String( 262 ), GetName(getProductTechType) ) + "\n" + 
			//	Color.red.ToString() + AsTableManager.Instance.GetTbl_String( 1334 );			
			
			AsHudDlgMgr.Instance.productionDlg.SetMessageBox( AsNotify.Instance.MessageBox( strTitle, GetTypeString( getProductTechType ), this, "SendLearn", 
				AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION) ); 
			
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);							
		}
	}
	
	
	
	
	// Use this for initialization
	void Start () 
	{
		btnLearn.Text = AsTableManager.Instance.GetTbl_String(1013);
		btnLearn.SetInputDelegate(LearnBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
