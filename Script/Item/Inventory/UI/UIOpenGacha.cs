using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIOpenGacha : MonoBehaviour 
{
	public int testRandomItem;

	public SimpleSprite itemImgPos;
	public GameObject	strengthenSuccEffect;
	public AsChargingEffect strengthenIngEffect;
	public Vector2			minusItemSize;	

	public float speedAdd = 0.4f;
	public float FirstSpeed = 1.5f;
	public float fTotalTime = 8.0f;
	public float MinSpeed = 0.07f;

	protected float m_fMaxSpeed = 0f;
	protected float m_fSpeed = 0f;
	protected int	m_iActionIndex = 0;
	protected bool	m_isNeedAction = false;
	protected float m_fTime = 0f;
	protected float m_fShowTextTime = 0f;
	protected List<UISlotItem> m_SlotItemList = new List<UISlotItem>();
	protected UISlotItem m_SelfSlotItem;


	public void Open(int iUseItemId, int iReceiveItemId)
	{
		Item _ReceiveItem = ItemMgr.ItemManagement.GetItem(iUseItemId);
		if (null == _ReceiveItem)
		{
			Debug.LogError("UIRandomItem::Open() [null == RealItem ] item id : " + iUseItemId);
			return;
		}

		Tbl_Lottery_Record _record = AsTableManager.Instance.GetTbl_Lottery_Record(_ReceiveItem.ItemData.m_iItem_Rand_ID);
		if (null == _record)
			return;

		foreach (int iIndex in _record.idlist)
		{

			Tbl_RandItem_Record _randItemRecord = AsTableManager.Instance.GetTbl_RandItem_Record(iIndex);
			if (null == _randItemRecord)
				continue;

			foreach (int iItemTableIndex in _randItemRecord.idlist)
			{
				Item _item = ItemMgr.ItemManagement.GetItem(iItemTableIndex);
				if (null == _item)
				{
					Debug.LogError("AsUIRandItem::Open()[ null == item ] id: " + iItemTableIndex);
					continue;
				}

				UISlotItem _SlotItem = ResourceLoad.CreateItemIcon(_item.GetIcon(), itemImgPos, Vector3.back, minusItemSize, false);
				if (null == _SlotItem)
				{
					Debug.LogError("AsUIRandItem::Open()[ null == UISlotItem ] id: " + iItemTableIndex);
					continue;
				}


				_SlotItem.tempItem = _item;
				m_SlotItemList.Add(_SlotItem);

				_SlotItem.gameObject.SetActive(false);

				if (m_SlotItemList.Count > 10)
					break;
			}

			if (m_SlotItemList.Count > 10)
				break;
		}

		m_fSpeed = FirstSpeed;
		m_fTime = 0f;
		m_isNeedAction = true;
		m_iActionIndex = 0;

		m_fMaxSpeed = FirstSpeed;
		m_fShowTextTime = 0f;

		if (fTotalTime <= 0f)
			fTotalTime = 1f;


		SetStrengthenSuccEffect(false);
		SetStrengthenIngEffect(true, itemImgPos.transform.position);

	}


	public virtual void SetStrengthenSuccEffect(bool isActive)
	{
		if (null == strengthenSuccEffect)
			return;

		strengthenSuccEffect.SetActive(isActive);
	}

	public virtual void SetStrengthenIngEffect(bool isActive, Vector3 pos)
	{
		if (null == strengthenIngEffect)
			return;

		if (false == isActive)
		{
			strengthenIngEffect.Enable = isActive;
			return;
		}

		strengthenIngEffect.Enable = true;
		strengthenIngEffect.transform.position = new Vector3(pos.x, pos.y, pos.z - 2.0f);
	}

	protected virtual void Updating()
	{
		if (false == m_isNeedAction)
			return;

		m_fTime += Time.deltaTime;
		m_fSpeed += Time.deltaTime;

		if (m_fSpeed > m_fMaxSpeed && 0 < m_SlotItemList.Count)
		{
			if (0 <= m_iActionIndex && m_iActionIndex < m_SlotItemList.Count)
			{
				m_SlotItemList[m_iActionIndex].gameObject.SetActiveRecursively(false);
			}
			int iPreActionIndex = m_iActionIndex;
			m_iActionIndex = Random.Range(0, m_SlotItemList.Count);

			if (iPreActionIndex == m_iActionIndex)
			{
				if (m_iActionIndex == m_SlotItemList.Count - 1)
				{
					m_iActionIndex -= 1;
				}
				else
				{
					m_iActionIndex += 1;
				}
			}

			if (m_SlotItemList.Count <= m_iActionIndex || m_iActionIndex < 0)
			{
				m_iActionIndex = 0;
			}

			m_SlotItemList[m_iActionIndex].gameObject.SetActiveRecursively(true);


			m_fSpeed = 0f;
		}

		m_fMaxSpeed -= (Time.deltaTime * speedAdd);
		if (m_fMaxSpeed < MinSpeed)
			m_fMaxSpeed = MinSpeed;
	}

	void Update()
	{
		Updating();
	}

	public void SetSelfItem(int iItemTableIndex, SimpleSprite parentSprite)
	{
		Item _item = ItemMgr.ItemManagement.GetItem(iItemTableIndex);
		if (null == _item)
		{
			Debug.LogError("AsUIRandItem::Open()[ null == item ] id: " + iItemTableIndex);
			return;
		}

		UISlotItem _SlotItem = ResourceLoad.CreateItemIcon(_item.GetIcon(), parentSprite, Vector3.back, minusItemSize, false);
		if (null == _SlotItem)
		{
			Debug.LogError("AsUIRandItem::Open()[ null == UISlotItem ] id: " + iItemTableIndex);
			return;
		}


		_SlotItem.tempItem = _item;

		m_SelfSlotItem = _SlotItem;

		m_SelfSlotItem.gameObject.SetActive(false);
	}

	public virtual void CloseState()
	{
		m_isNeedAction = false;

		if (null != m_SelfSlotItem && null != m_SelfSlotItem.tempItem)
			m_SelfSlotItem.gameObject.SetActive(true);

		foreach (UISlotItem slot in m_SlotItemList)
			slot.gameObject.SetActive(false);

		SetStrengthenIngEffect(false, Vector3.zero);
		SetStrengthenSuccEffect(true);
	}
}
