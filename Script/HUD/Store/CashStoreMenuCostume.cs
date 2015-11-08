using UnityEngine;
using System.Collections;

public class CashStoreMenuCostume : CashStoreMenuGacha {

	public UIButton leftRot;
	public UIButton rightRot;
	protected float m_PartsRot = 180f;
	protected bool m_isLeftRot = false;
	protected bool m_isRightRot = false;
	protected float rotSpeed = 50f;
	protected bool m_isShowInit = false;
	protected float m_fEffectDlayTime = 0f;

	public GameObject backgroundObject;
	public GameObject objCategoryListItem;


	void Update()
	{
		UpdateForModel();
	}

	protected void UpdateForModel()
	{
		if (null != AsHudDlgMgr.Instance.cashShopEntity &&
			eModelLoadingState.Finished == AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState() &&
			true == m_isShowInit &&
			null != backgroundObject)
		{
			if (m_fEffectDlayTime < 0.1f)
			{
				m_fEffectDlayTime += Time.deltaTime;
				return;
			}

			if (userClass == eCLASS.HUNTER)
			{
				eGENDER gender = AsHudDlgMgr.Instance.cashShopEntity.GetProperty<eGENDER>(eComponentProperty.GENDER);
				if (gender == eGENDER.eGENDER_MALE)
				{
					AsHudDlgMgr.Instance.cashShopEntity.transform.position = new Vector3(-503.5f, 4.5f, -4.2f);
					AsHudDlgMgr.Instance.cashShopEntity.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
				}
				else
				{
					AsHudDlgMgr.Instance.cashShopEntity.transform.position = new Vector3(-504f, 4.35f, -4.2f);
					AsHudDlgMgr.Instance.cashShopEntity.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
				}
			}
			else
			{
				AsHudDlgMgr.Instance.cashShopEntity.transform.position = backgroundObject.transform.position + new Vector3(-0.1f, -3.3f, -4f);
			}
			AsHudDlgMgr.Instance.cashShopEntity.transform.rotation = Quaternion.Euler(25f, -147f, 15.5f) * Quaternion.AngleAxis(m_PartsRot, Vector3.up);
			ResourceLoad.SetLayerHierArchy(AsHudDlgMgr.Instance.cashShopEntity.transform, LayerMask.NameToLayer("GUI"));

			if (null != AsHudDlgMgr.Instance.cashStore)
			{
				AsHudDlgMgr.Instance.cashStore.LockInput(false);
				AsHudDlgMgr.Instance.cashStore.HideLoadingIndigator();
			}
			m_isShowInit = false;
		}
		else
		{
			if (m_isLeftRot)
				SetPartsLeftRot();

			if (m_isRightRot)
				SetPartsRightRot();
		}
	}

	#region -create model-
	protected void CreateRenderTarget(eCLASS _eClass, eGENDER _gender, params int[] _partsIDs)
	{
		if (null == objCategoryListItem)
			return;

		if (null != AsHudDlgMgr.Instance.cashShopEntity)
		{
			AsEntityManager.Instance.RemoveEntity(AsHudDlgMgr.Instance.cashShopEntity);
			AsHudDlgMgr.Instance.cashShopEntity = null;
		}

		m_isShowInit = true;
		m_PartsRot = 300f;

		AS_SC_OTHER_CHAR_APPEAR_2 appear = new AS_SC_OTHER_CHAR_APPEAR_2();
		appear.nCharUniqKey = uint.MaxValue;
		appear.eClass = (int)_eClass;
		appear.eGender = _gender;
		appear.eRace = (int)AsEntityManager.Instance.UserEntity.GetProperty<eRACE>(eComponentProperty.RACE);
		Tbl_Class_Record record = AsTableManager.Instance.GetTbl_Class_Record((eRACE)appear.eRace, (eCLASS)appear.eClass);
		if (null != record)
			appear.nMoveSpeed = (int)record.MoveSpeed;

		appear.bCostumeOnOff = PartsRoot.GetCosOnDef();
		appear.sNormalItemVeiw_1 = new sITEMVIEW();
		appear.sNormalItemVeiw_2 = new sITEMVIEW();
		appear.sNormalItemVeiw_3 = new sITEMVIEW();
		appear.sNormalItemVeiw_4 = new sITEMVIEW();
		appear.sNormalItemVeiw_5 = new sITEMVIEW();

		appear.sCosItemView_1 = new sITEMVIEW();
		appear.sCosItemView_2 = new sITEMVIEW();
		appear.sCosItemView_3 = new sITEMVIEW();
		appear.sCosItemView_4 = new sITEMVIEW();
		appear.sCosItemView_5 = new sITEMVIEW();
		appear.sCosItemView_6 = new sITEMVIEW();
		appear.sCosItemView_7 = new sITEMVIEW();
		appear.fHpMax = 100f;
		appear.fHpCur = 100f;

		int count = 0;
		foreach (int id in _partsIDs)
		{
			Item _item = ItemMgr.ItemManagement.GetItem(id);
			if (null == _item)
				continue;

			if (_item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem &&
				_item.ItemData.GetSubType() == (int)Item.eUSE_ITEM.ConsumeHair &&
				_item.ItemData.needClass == _eClass &&
				count == 0)
			{
				Item hairItem = ItemMgr.ItemManagement.GetItem(_item.ItemData.m_iItem_Rand_ID);

				if (hairItem != null)
				{
					if (hairItem.ItemData.GetItemType() == Item.eITEM_TYPE.EquipItem && hairItem.ItemData.GetSubType() == (int)Item.eEQUIP.Hair)
						appear.nHair = hairItem.ItemID;
				}

				continue;
			}

			count++;

			if (Item.eITEM_TYPE.CosEquipItem != _item.ItemData.GetItemType() && Item.eITEM_TYPE.EquipItem != _item.ItemData.GetItemType())
				continue;

			switch ((Item.eEQUIP)_item.ItemData.GetSubType())
			{
				case Item.eEQUIP.Weapon:
					appear.sNormalItemVeiw_1.nItemTableIdx = PartsRoot.GetDefWeaponItemID(_eClass);
					appear.sCosItemView_1.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Head:
					appear.sCosItemView_2.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Armor:
					appear.sCosItemView_3.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Gloves:
					appear.sCosItemView_4.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Point:
					appear.sCosItemView_5.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Fairy:
					appear.sCosItemView_7.nItemTableIdx = id;
					break;

				case Item.eEQUIP.Wing:
					appear.sCosItemView_6.nItemTableIdx = id;
					break;
			}
		}


		OtherCharacterAppearData creationData = new OtherCharacterAppearData(appear);
		creationData.notRegisterMgr = true;

		AsHudDlgMgr.Instance.cashShopEntity = AsEntityManager.Instance.CreateUserEntity("OtherUser", creationData, true, true, 3f);


		if (null != leftRot)
			leftRot.AddInputDelegate(leftRotDelegate);

		if (null != rightRot)
			rightRot.AddInputDelegate(rightRotDelegate);

		m_isLeftRot = false;
		m_isRightRot = false;
		m_fEffectDlayTime = 0f;
	}

	protected void leftRotDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.NO_CHANGE)
			m_isLeftRot = true;
		else
			m_isLeftRot = false;
	}

	protected void rightRotDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.NO_CHANGE)
			m_isRightRot = true;
		else
			m_isRightRot = false;
	}

	public void SetPartsLeftRot()
	{
		if (null == AsHudDlgMgr.Instance.cashShopEntity)
			return;

		if (eModelLoadingState.Finished != AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState())
			return;

		m_PartsRot += (rotSpeed * Time.deltaTime);
		AsHudDlgMgr.Instance.cashShopEntity.transform.localRotation = Quaternion.Euler(25f, -147f, 15.5f) * Quaternion.AngleAxis(m_PartsRot, Vector3.up);

		if (m_PartsRot > 360f)
			m_PartsRot -= 360f;
	}

	public void SetPartsRightRot()
	{
		if (null == AsHudDlgMgr.Instance.cashShopEntity)
			return;

		if (eModelLoadingState.Finished != AsHudDlgMgr.Instance.cashShopEntity.CheckModelLoadingState())
			return;

		m_PartsRot -= (rotSpeed * Time.deltaTime);
		AsHudDlgMgr.Instance.cashShopEntity.transform.localRotation = Quaternion.Euler(25f, -147f, 15.5f) * Quaternion.AngleAxis(m_PartsRot, Vector3.up);

		if (m_PartsRot < 360f)
			m_PartsRot += 360f;
	}

	#endregion

}
