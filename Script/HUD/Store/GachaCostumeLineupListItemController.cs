using UnityEngine;
using System.Collections;

public class GachaCostumeLineupListItemController : MonoBehaviour {

	public UIListItemContainer uiListItemContainer;
	public GameObject[] slotObjects;
	public SimpleSprite[] slotImg;

	public void AddItemIcons(params int[] _itemID)
	{
		int count = _itemID.Length;

		// disable all slot img
		foreach (SimpleSprite img in slotImg)
			img.gameObject.SetActive(false);

		// enable slot img

		for (int i = 0; i < count; i++)
		{
			Item item = ItemMgr.ItemManagement.GetItem(_itemID[i]);

			if (item == null)
				continue;

			slotImg[i].gameObject.SetActive(true);

			GameObject iconObj = GameObject.Instantiate(item.GetIcon()) as GameObject;

			if (iconObj != null)
			{
				iconObj.transform.parent = slotObjects[i].transform;
				iconObj.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
				iconObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
		}

		uiListItemContainer.ScanChildren();
		uiListItemContainer.UpdateCamera();
	}
}
