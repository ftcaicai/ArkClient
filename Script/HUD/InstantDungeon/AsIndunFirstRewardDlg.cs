using UnityEngine;
using System.Collections;

public class AsIndunFirstRewardDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;

	public SpriteText m_textTitle = null;
	public SpriteText m_textDesc = null;
	public SpriteText m_textOk = null;
	public EnchantSlot m_RewardItem;
	public SpriteText m_textRewardItem = null;
	public ParticleSystem m_Particle;

	void Start()
	{
	}
	
	void Update()
	{
	}

	public void Open(GameObject goRoot, int nIndunBranchTableIndex)
	{
		m_goRoot = goRoot;

		Tbl_InsDungeonReward_Record record = AsTableManager.Instance.GetInsRewardRecord( nIndunBranchTableIndex);
		
		if( null == record)
		{
			Debug.LogError( "AsIndunFirstRewardDlg::Open(), null == record, nIndunBranchTableIndex: " + nIndunBranchTableIndex);
			Close();
			return;
		}

		m_textTitle.Text = AsTableManager.Instance.GetTbl_String( 126);
		m_textOk.Text = AsTableManager.Instance.GetTbl_String( 1152);

		Tbl_InDun_Record record2 = AsTableManager.Instance.GetInDunRecord( record.InstanceTableIdx);
		string strIndunName = AsTableManager.Instance.GetTbl_String( record2.Name);
		string strPlayerCount = record.MaxPlayerCount.ToString();
		string strIndunGrade = AsTableManager.Instance.GetTbl_String( 1873);
		if( true == record.Grade.ToLower().Contains( "hard"))
			strIndunGrade = AsTableManager.Instance.GetTbl_String( 1874);
		else if( true == record.Grade.ToLower().Contains( "hell"))
			strIndunGrade = AsTableManager.Instance.GetTbl_String( 1897);

		m_textDesc.Text = string.Format( AsTableManager.Instance.GetTbl_String( 2349), strIndunName, strPlayerCount, strIndunGrade);

		if( false == _SetRewardItem( record.First_Reward, record.First_Reward_Count))
		{
			Close();
			return;
		}
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);
		
		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	public void OnBtnClose()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}
	
	public void OnBtnOk()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}

	private bool _SetRewardItem(int nRewardItemID, int nCount)
	{
		Item item = ItemMgr.ItemManagement.GetItem( nRewardItemID);
		
		if( null == item)
		{
			Debug.Log( "AsIndunFirstRewardDlg::_SetRewardItem(), null == item, nRewardItemID: " + nRewardItemID);
			return false;
		}

		if( nCount <= 0)
		{
			Debug.Log( "AsIndunFirstRewardDlg::_SetRewardItem(), nCount <= 0, nRewardItemID: " + nRewardItemID);
			return false;
		}
		
		m_RewardItem.CreateSlotItem( item, gameObject.transform);
		m_RewardItem.SetItemCountText( nCount);
		m_textRewardItem.Text = AsTableManager.Instance.GetTbl_String( item.ItemData.nameId);

		m_Particle.Play();
		return true;
	}
}
