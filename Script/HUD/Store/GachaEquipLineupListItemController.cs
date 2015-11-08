using UnityEngine;
using System.Collections.Generic;

public class GachaEquipLineupListItemController : MonoBehaviour
{

	public UIListItemContainer uiListItemContainer;
	public GameObject iconParentObj;
	public SpriteText txtGrade;
	public SpriteText txtLevel;
	public SpriteText txtMin;
	public SpriteText txtMax;
	public SpriteText txtOption;
	public BoxCollider boxCollider;
	public int itemID;

    public int idx;

	// Use this for initialization
	void Start ()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtGrade);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtLevel);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMin);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMax);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtOption);
	}


	public void SetInfo(GameObject _objIcon, string _name, string _level, string _min, string _max, string _option)
	{
		_objIcon.transform.parent = iconParentObj.transform;
		_objIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		_objIcon.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
		
		txtGrade.Text	= _name;
		txtLevel.Text	= _level;
		txtMin.Text		= _min;
		txtMax.Text		= _max;
		txtOption.Text	= _option;

		uiListItemContainer.ScanChildren();
		uiListItemContainer.UpdateCollider();
	}
}
