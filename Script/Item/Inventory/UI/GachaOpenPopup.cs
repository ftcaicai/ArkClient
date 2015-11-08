using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GachaOpenPopup : MonoBehaviour 
{
	public int openedCount = 0;
	public GameObject objGachaOpenPrefab;
	public GameObject objEndEffect;
	public GameObject objendEffect_Epik;
	public SimpleSprite[] slots;
	public UIButton btnClose;
	public SpriteText txtItemName;
	public SpriteText txtPopupTitle;

	private List<UIOpenGacha> listOpenGacha = new List<UIOpenGacha>();
	private List<int> listSelectedItemID = new List<int>();
    private List<byte> listSeledItemStrengthen = new List<byte>();
	private float showTime	= 0.0f;
	private float nextTime	= 0.0f;
	private float nowTime	= 0.0f;
	private bool showed = false;
	private bool showedAll = false;
	private bool isOverEpik = false;

	public delegate void CallBeforClose();
	private CallBeforClose callBeforCloseFunc;

	private AudioSource gachaSound = null;

	void Start()
	{
		if (btnClose != null)
			btnClose.SetInputDelegate(CloseInputProcess);

		showTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(166).Value * 0.001f;
		nextTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(167).Value * 0.001f;

		if (objEndEffect != null)
			objEndEffect.SetActive(false);

		if (objendEffect_Epik != null)
			objendEffect_Epik.SetActive(false);

		if (txtItemName != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtItemName);
			txtItemName.Text = AsTableManager.Instance.GetTbl_String(2276);
		}

		if (txtPopupTitle != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage(txtPopupTitle);
			txtPopupTitle.Text = string.Empty;
			//txtPopupTitle.Text = AsTableManager.Instance.GetTbl_String(2276);
		}
	}

	public void Open(List<int> _listGachaItemID, List<int> _listSelectedItemID, List<byte> _listStrengthen,  CallBeforClose _callBeforCloseFunc)
	{
		if (_listGachaItemID.Count != _listSelectedItemID.Count)
			return;

		// check over epik
		isOverEpik = false;
		foreach (int itemID in _listSelectedItemID)
		{
			Item item = ItemMgr.ItemManagement.GetItem(itemID);

			Item.eGRADE grade = item.ItemData.grade;

			Item.eITEM_TYPE itemType = Item.eITEM_TYPE.None;

			if (item.ItemData != null)
				itemType = item.ItemData.GetItemType();

			if (itemType == Item.eITEM_TYPE.UseItem || item.ItemData.GetSubType() == (int)Item.eUSE_ITEM.PetEgg)
			{
				if (grade != Item.eGRADE.Normal && grade != Item.eGRADE.Magic)
				{
					isOverEpik = true;
					break;
				}
			}

			if (itemType == Item.eITEM_TYPE.EquipItem || itemType == Item.eITEM_TYPE.CosEquipItem)
			{
				if (grade == Item.eGRADE.Epic || grade == Item.eGRADE.Ark)
				{
					isOverEpik = true;
					break;
				}
			}
		}

		listSelectedItemID = _listSelectedItemID;

		callBeforCloseFunc = _callBeforCloseFunc;

        listSeledItemStrengthen = _listStrengthen;

		for (int i = 0; i < slots.Length; i++)
		{
			GameObject gachaDirectObject = GameObject.Instantiate(objGachaOpenPrefab) as GameObject;

			gachaDirectObject.transform.parent = gameObject.transform;
			gachaDirectObject.transform.localPosition = Vector3.zero;


			UIOpenGacha openGacha = gachaDirectObject.GetComponent<UIOpenGacha>();

			openGacha.strengthenSuccEffect.transform.localPosition = slots[i].transform.localPosition;

			openGacha.itemImgPos = slots[i];

			openGacha.Open(_listGachaItemID[i], _listSelectedItemID[i]);

			openGacha.SetSelfItem(_listSelectedItemID[i], slots[i]);

			listOpenGacha.Add(openGacha);
		}

		if (slots.Length == 1)
			gachaSound = AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGacha1x, Vector3.zero, false);
		else if (slots.Length == 5)
			gachaSound = AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGacha5x, Vector3.zero, false);
		else if (slots.Length == 11)
			gachaSound = AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGacha11x, Vector3.zero, false);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && listOpenGacha.Count >= 1 && showedAll == false)
		{
			showedAll = true;

			for (int i = openedCount; i < listOpenGacha.Count; i++)
				listOpenGacha[i].CloseState();

			ShowEndEffect(isOverEpik);

			int itemID = listSelectedItemID[listSelectedItemID.Count - 1];

            byte strengthen = listSeledItemStrengthen[listSelectedItemID.Count - 1];

			SetSelectedItemName(itemID, strengthen);

			if (gachaSound != null)
				AsSoundManager.Instance.StopSound(gachaSound);

			if (isOverEpik == true)
				AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGachaFinish2, Vector3.zero, false);
			else
				AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGachaFinish, Vector3.zero, false);
		}

		UpdateAutoOpen();
	}

	void SetSelectedItemName(int _itemID, byte _strenthen)
	{
		Item item = ItemMgr.ItemManagement.GetItem(_itemID);

		int level = item.ItemData.levelLimit;

		if (item != null)
		{
			if (item.ItemData == null)
			{
				Debug.LogWarning("Item data is null");
				return;
			}

			Item.eITEM_TYPE itemType = item.ItemData.GetItemType();

			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0}", item.ItemData.GetGradeColor());
			sb.Append(AsTableManager.Instance.GetTbl_String(item.ItemData.nameId));

            if (itemType == Item.eITEM_TYPE.EquipItem)
            {
                StringBuilder innserSb = new StringBuilder();

                if (_strenthen >= 1)
                {
                    innserSb.Append(Color.white);
                    innserSb.Append("+");
                    innserSb.Append(_strenthen);
                    innserSb.Append(" ");
                    innserSb.Append(item.ItemData.GetGradeColor());
                    innserSb.Append(AsTableManager.Instance.GetTbl_String(item.ItemData.nameId));

                    sb.Remove(0, sb.Length);
                    sb.AppendFormat(AsTableManager.Instance.GetTbl_String(2278), level.ToString(), innserSb.ToString());

                    txtItemName.Text = sb.ToString();
                }
                else
                {
                    innserSb.AppendFormat(AsTableManager.Instance.GetTbl_String(2278), level.ToString(), sb.ToString());

                    txtItemName.Text = innserSb.ToString();
                }
            }
            else
                txtItemName.Text = sb.ToString();
		}
		else
			Debug.LogError("item is null in GachaOpenPopup.cpp");
	}

	void UpdateAutoOpen()
	{
		if (showedAll == true)
			return;

		if (listOpenGacha.Count <= 0)
			return;

		if (showed == false)
		{
			showTime -= Time.deltaTime;

			if (showTime <= 0.0f)
				showed = true;
		}

		if (showed == true)
		{
			nowTime += Time.deltaTime;

			if (nowTime >= nextTime)
			{
				if (openedCount < listOpenGacha.Count)
				{
					nowTime = 0.0f;

					listOpenGacha[openedCount].CloseState();

					// set name
                    SetSelectedItemName(listSelectedItemID[openedCount], listSeledItemStrengthen[openedCount]);

					openedCount++;

					if (openedCount == listOpenGacha.Count)
					{
						if (isOverEpik == true)
							gachaSound = AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGachaFinish2, Vector3.zero, false);
						else
							gachaSound = AsSoundManager.Instance.PlaySound(AsSoundPath.OpenGachaFinish, Vector3.zero, false);
					}
				}
				else
				{
					showedAll = true;
				}
			}

			// end effect
			if (openedCount == listOpenGacha.Count)
				ShowEndEffect(isOverEpik);
		}
	}

	void CloseInputProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (AsHudDlgMgr.Instance.IsOpenCashStore)
				AsHudDlgMgr.Instance.cashStore.UpdateFreeGachaPoint();

			if (callBeforCloseFunc != null)
				callBeforCloseFunc();

			DestroyObject(gameObject);
		}
	}


	void ShowEndEffect(bool _isOverEpik)
	{
		// special effect
		if (_isOverEpik == true)
		{
			if (objendEffect_Epik != null && objendEffect_Epik.activeSelf == false)
				objendEffect_Epik.SetActive(true);

			if (txtPopupTitle != null)
				txtPopupTitle.Text = AsTableManager.Instance.GetTbl_String(2343);
		}
		else
		{
			if (objEndEffect != null && objEndEffect.activeSelf == false)
				objEndEffect.SetActive(true);


			if (txtPopupTitle != null)
				txtPopupTitle.Text = AsTableManager.Instance.GetTbl_String(2342);
		}

	}
}
