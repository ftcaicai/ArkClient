using UnityEngine;
using System.Collections.Generic;

public class GachaFreeLineupListItemController : MonoBehaviour
{
	public UIListItemContainer uiListItemContainer;
	public GameObject iconParentObj;
	public SpriteText txtGrade;
	public SpriteText txtLevel;
	public SpriteText txtItemName;
	public int itemID;
	public BoxCollider boxCollider;

	// Use this for initialization
	void Start ()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtGrade);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtLevel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtItemName);
	}


	public void SetInfo(GameObject _objIcon, string _grade, int _level, string _itemName)
	{
		_objIcon.transform.parent = iconParentObj.transform;
		_objIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		_objIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
		
		txtGrade.Text	= _grade;
		txtItemName.Text = _itemName;
		txtLevel.Text = _level.ToString();

		uiListItemContainer.ScanChildren();
		uiListItemContainer.UpdateCollider();
	}
}
