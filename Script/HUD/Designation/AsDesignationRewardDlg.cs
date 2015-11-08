using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eDesignationRewardType
{
	Normal = 0,
	Accrue,
};

public class AsDesignationRewardDlg : MonoBehaviour 
{
	public SpriteText 		textTitle;
	public SimpleSprite 	itemImgPos;
	public Vector2			minusItemSize;
	public SpriteText 		textItemName;
	public SpriteText 		textInfo;
	public UIButton 		btnOk;
	public UIButton 		btnCancel;

	private eDesignationRewardType 	rewardType;
	private int 									designationID;

	System.Text.StringBuilder m_sbTemp = new System.Text.StringBuilder();

	// Use this for initialization
	void Start () 
	{
		textTitle.Text = AsTableManager.Instance.GetTbl_String(126);

		btnOk.SetInputDelegate(OKBtnDelegate);
		btnOk.Text = AsTableManager.Instance.GetTbl_String(4109);

		btnCancel.SetInputDelegate(CancelBtnDelegate);
	}

	public void Open(eDesignationRewardType _type , int _iItemTableIndex , int _itemCount , bool _isConditionSatisfaction , int _designationID  , int _AccrueRankPoint)
	{
		rewardType = _type;
		designationID = _designationID;

		if (rewardType == eDesignationRewardType.Accrue) 
		{
			textInfo.Text = string.Format (AsTableManager.Instance.GetTbl_String (2729), _AccrueRankPoint); 
		} 
		else if (rewardType == eDesignationRewardType.Normal)
		{
			DesignationData data = AsDesignationManager.Instance.GetDesignation( designationID );
			if( data != null )
				textInfo.Text = string.Format (AsTableManager.Instance.GetTbl_String (2732), AsTableManager.Instance.GetTbl_String( data.name) ); 
		}

		Item _item = ItemMgr.ItemManagement.GetItem(_iItemTableIndex);
		if (null == _item)
		{
			Debug.LogError("AsDesignationRewardDlg::Open()[ null == item ] id: " + _iItemTableIndex);
			return;
		}
		
		UISlotItem _SlotItem = ResourceLoad.CreateItemIcon(_item.GetIcon(), itemImgPos, Vector3.back, minusItemSize, false);
		if (null == _SlotItem)
		{
			Debug.LogError("AsDesignationRewardDlg::Open()[ null == UISlotItem ] id: " + _iItemTableIndex);
			return;
		}

		m_sbTemp.Length = 0;
		m_sbTemp.Append( _item.ItemData.GetGradeColor().ToString() );
		m_sbTemp.Append( AsTableManager.Instance.GetTbl_String(  _item.ItemData.nameId ) );
		textItemName.Text = m_sbTemp.ToString();

		if (_itemCount > 1)
			_SlotItem.itemCountText.Text = _itemCount.ToString ();
		else
			_SlotItem.itemCountText.Text = string.Empty;

		if (_isConditionSatisfaction == false) 
		{
			AsUtil.SetButtonState( btnOk , UIButton.CONTROL_STATE.DISABLED );
		}
	}

	private void OKBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			CloseDlg();

			//	send request reward server
			if( rewardType == eDesignationRewardType.Normal )
			{
				body_CS_SUBTITLE_INDEX_REWARD subtitleIndexReward = new body_CS_SUBTITLE_INDEX_REWARD( designationID );
				byte[] data = subtitleIndexReward.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
			}
			else if( rewardType == eDesignationRewardType.Accrue )
			{
				int nLastReceiveRewardRankPoint = AsDesignationRankRewardManager.Instance.LastReceiveRewardRankPoint;
				DesignationRankRewardData nextRankRewardData = AsDesignationRankRewardManager.Instance.GetNextRankRewardData (nLastReceiveRewardRankPoint);

				body_CS_SUBTITLE_ACCRUE_REWARD subtitleAccrueReward = new body_CS_SUBTITLE_ACCRUE_REWARD( nextRankRewardData.id );
				byte[] data = subtitleAccrueReward.ClassToPacketBytes();
				AsNetworkMessageHandler.Instance.Send( data);
			}

			AsDesignationManager.Instance.SendRequestReward = true;
		}
	}

	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			CloseDlg();
		}
	}

	private void CloseDlg()
	{
		GameObject.Destroy( gameObject );
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
