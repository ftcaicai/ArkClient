using UnityEngine;
using System.Collections;

public class CashStoreCategoryItem : MonoBehaviour
{
    public int btnNameIndex;
    public SpriteText textBtnName;
	public eCashStoreMainCategory mainCategory;
    public BoxCollider col;

    void Start()
    {
		if( textBtnName != null)
		{
			AsLanguageManager.Instance.SetFontFromSystemLanguage( textBtnName);
			textBtnName.Text = AsTableManager.Instance.GetTbl_String( btnNameIndex);
		}
    }
}
