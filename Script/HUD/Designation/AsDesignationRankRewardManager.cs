using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DesignationRankRewardData : AsTableRecord
{
	public int id = -1;
	public int rankRewardValue = 0;

	public int DivineKnight_Item_ID = -1;
	public int DivineKnight_Item_Count = 0;
	public int Magician_Item_ID = -1;
	public int Magician_Item_Count = 0;
	public int Cleric_Item_ID = -1;
	public int Cleric_Item_Count = 0;
	public int Hunter_Item_ID = -1;
	public int Hunter_Item_Count = 0;


	public int nMinRankRewardPoint = 0;
	public int nMaxRankRewardPoint = 0;

	public DesignationRankRewardData( XmlNode node)
	{
		try
		{
			SetValue( ref id, node, "ID");
			SetValue( ref rankRewardValue, node, "RankReward_Value");
			SetValue( ref DivineKnight_Item_ID, node, "DivineKnight_Item_ID");
			SetValue( ref DivineKnight_Item_Count, node, "DivineKnight_Item_Count");
			SetValue( ref Magician_Item_ID, node, "Magician_Item_ID");
			SetValue( ref Magician_Item_Count, node, "Magician_Item_Count");
			SetValue( ref Cleric_Item_ID, node, "Cleric_Item_ID");
			SetValue( ref Cleric_Item_Count, node, "Cleric_Item_Count");
			SetValue( ref Hunter_Item_ID, node, "Hunter_Item_ID");
			SetValue( ref Hunter_Item_Count, node, "Hunter_Item_Count");
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
		}
	}
}

public class AsDesignationRankRewardManager
{
	private static AsDesignationRankRewardManager instance = null;
	public static AsDesignationRankRewardManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsDesignationRankRewardManager();
			
			return instance;
		}
	}

	private int nLastReceiveRewardRankPoint = 0;
	public int LastReceiveRewardRankPoint
	{
		get{return nLastReceiveRewardRankPoint;}
		set{nLastReceiveRewardRankPoint = value;}
	}



	private List<DesignationRankRewardData> rankRewardDesignation = new List<DesignationRankRewardData>();

	private AsDesignationRankRewardManager()
	{
	}

	public void LoadTable()
	{
		try
		{
			string strTableName = "Table/SubTitleRankRewardTable";
			
			XmlElement root = AsTableBase.GetXmlRootElement( strTableName );
			XmlNodeList nodes = root.ChildNodes;
			
			foreach( XmlNode node in nodes)
			{
				DesignationRankRewardData designationRankReward = new DesignationRankRewardData( node);
				rankRewardDesignation.Add( designationRankReward );
			}

			CalculateMinMaxRankPoint();
		}
		catch( System.Exception e)
		{
			Debug.LogError(e);
			AsUtil.ShutDown( "AsDesignationRankRewardManager:LoadTable");
		}
	}

	public void CalculateMinMaxRankPoint()
	{
		//	sort ascending
		rankRewardDesignation.Sort( delegate( DesignationRankRewardData p1, DesignationRankRewardData p2) { return (p1.rankRewardValue.CompareTo(p2.rankRewardValue)); });

		int minRankPoint = 1;
		foreach (DesignationRankRewardData data in rankRewardDesignation) 
		{
			data.nMinRankRewardPoint = minRankPoint;
			data.nMaxRankRewardPoint = data.rankRewardValue;

			minRankPoint = data.nMaxRankRewardPoint + 1;
		}
	}

	public DesignationRankRewardData GetNextRankRewardData( int _lastReceiveRewardRankPoint )
	{
		int nNextMinRewardRankPoint = _lastReceiveRewardRankPoint + 1;
		foreach (DesignationRankRewardData data in rankRewardDesignation) 
		{
			if( nNextMinRewardRankPoint >= data.nMinRankRewardPoint && nNextMinRewardRankPoint <= data.nMaxRankRewardPoint )
				return data;
		}

		return null;
	}

	public void DesignationIndexRewardResult( int _designationID , bool _isReceiveSuccess )
	{
		PlayerStatusDlg	statusDlg = AsHudDlgMgr.Instance.GetPlayerStatusDlg();
		if( statusDlg != null )
		{
			GameObject	goDesignationDlg = statusDlg.DesignationDlg;
			if( goDesignationDlg != null )
			{
				AsDesignationDlg	designationDlg = goDesignationDlg.GetComponentInChildren<AsDesignationDlg>();

				if( designationDlg != null )
					designationDlg.SetDesignationIndexReward( _designationID , _isReceiveSuccess );
			}
		}
	}

	public void DesignationAccrueRewardResult()
	{
		PlayerStatusDlg	statusDlg = AsHudDlgMgr.Instance.GetPlayerStatusDlg();
		if( statusDlg != null )
		{
			GameObject	goDesignationDlg = statusDlg.DesignationDlg;
			if( goDesignationDlg != null )
			{
				AsDesignationDlg	designationDlg = goDesignationDlg.GetComponentInChildren<AsDesignationDlg>();
				
				if( designationDlg != null )
					designationDlg.SetDesignationAccrueReward();
			}
		}
	}


	public void Recv_SubtitleIndexRewardResult(body_SC_SUBTITLE_INDEX_REWARD_RESULT _rewardResult)
	{
		AsDesignationManager.Instance.SendRequestReward = false;

		switch(_rewardResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			DesignationIndexRewardResult( _rewardResult.nSubTitleTableIdx , true );
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(2777));
			break;
		case eRESULTCODE.eRESULT_FAIL:
			Debug.LogWarning("SubtitleIndexRewardResult fail , sustitle index : " + _rewardResult.nSubTitleTableIdx );
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1630));
			break;
		case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
			                             null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		}
	}

	public void Recv_SubtitleAccrueRewardResult(body_SC_SUBTITLE_ACCRUE_REWARD_RESULT _rewardResult)
	{
		AsDesignationManager.Instance.SendRequestReward = false;

		switch(_rewardResult.eResult)
		{
		case eRESULTCODE.eRESULT_SUCC:
			AsDesignationRankRewardManager.Instance.LastReceiveRewardRankPoint = _rewardResult.nAccrueRewardPoint;
			DesignationAccrueRewardResult();
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(2730));
			break;
		case eRESULTCODE.eRESULT_FAIL:
			Debug.LogWarning("SubtitleAccrueRewardResult fail , accrue reward point : " + _rewardResult.nAccrueRewardPoint );
			AsEventNotifyMgr.Instance.CenterNotify.AddMessage(AsTableManager.Instance.GetTbl_String(1630));
			break;
		case eRESULTCODE.eRESULT_FAIL_IVNENTORY_FULL:
			AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(118),
			                             null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
			break;
		}
	}

}











































