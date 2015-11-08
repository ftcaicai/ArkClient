using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class TooltipGaugeDlg : TooltipDlg 
{
	[SerializeField] SpriteText 		txtTitle;
	[SerializeField] UIProgressBar 	progressGuage = null;
	[SerializeField] SpriteText 		txtProgress;

	private StringBuilder 				sbProgress = new StringBuilder();

	public void Open( sITEM _sitem , ItemData _itemData )
	{
		if (null == _sitem)
		{
			Debug.LogError("TooltipGaugeDlg::SetSItem() [ null == _sitem ]");
			return;
		}
		
		Item _item = ItemMgr.ItemManagement.GetItem( _sitem.nItemTableIdx );
		
		if( null == _item )
		{
			Debug.LogError("TooltipGaugeDlg::SetSItem() [ null == _item ] item id : " + _sitem.nItemTableIdx );
			return;
		}
		
		if( false == SetItem(_item) )
			return;

		txtTitle.Text = AsTableManager.Instance.GetTbl_String (2423);

		Tbl_SynCosMix_Record	recordCosMix = AsTableManager.Instance.GetSynCosMixRecord( _itemData.grade , (Item.eEQUIP)_itemData.GetSubType()  );
		Tbl_GlobalWeight_Record recordGlobal = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "CosMixExpFactor" );

		int needMaxExp = 0;
		int currentExp = 0;
		if( recordCosMix != null && recordGlobal != null )
		{
//			needMaxExp = (int)((float)recordCosMix.needExp * ( 1.0f - (  recordGlobal.Value * (float)_sitem.nStrengthenCount / 1000.0f ) ));
			needMaxExp = (int)(((float)recordCosMix.needExp * ( 1000.0f - (  recordGlobal.Value * (float)_sitem.nStrengthenCount ) ))/1000.0f);
		}
		
		currentExp = _sitem.nAccreCount;

		if( currentExp >= needMaxExp )
		{
			txtProgress.Text = AsTableManager.Instance.GetTbl_String(2414);
		}
		else
		{
			sbProgress.Remove( 0, sbProgress.Length);
			sbProgress.AppendFormat( "RGBA( 1.0,1.0,1.0,1.0){0} / {1}", currentExp, needMaxExp);
			txtProgress.Text = sbProgress.ToString();
		}

		float fRatioCurrent = (float)currentExp / (float)needMaxExp;
		if( fRatioCurrent >= 1.0f ) fRatioCurrent = 1.0f;
		progressGuage.Value = fRatioCurrent;
	}
}
