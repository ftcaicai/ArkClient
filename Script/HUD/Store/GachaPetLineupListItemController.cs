using UnityEngine;
using System.Collections.Generic;

public class GachaPetLineupListItemController : MonoBehaviour
{
	public UIListItemContainer uiListItemContainer;
	public GameObject iconParentObj;
	public SpriteText txtName;
	public SpriteText txtGrade;
	public SpriteText txtDesc;
	public BoxCollider boxCollider;

	// Use this for initialization
	void Start ()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtGrade);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtName);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtDesc);
	}


	public void SetInfo(GameObject _objIcon, string _name, string _grade, string _desc)
	{
		_objIcon.transform.parent = iconParentObj.transform;
		_objIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		_objIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);

		txtName.Text	= _name;
		txtGrade.Text	= _grade;
		txtDesc.Text	= _desc;

		uiListItemContainer.ScanChildren();
		uiListItemContainer.UpdateCollider();
	}
}

