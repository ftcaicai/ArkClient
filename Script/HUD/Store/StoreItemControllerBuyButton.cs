using UnityEngine;
using System.Collections;

public class StoreItemControllerBuyButton : StoreItemController
{
	public UIButton btnBuy;


	void Start()
	{
		if (btnBuy != null)
		{
			if (btnBuy.spriteText != null)
			{
				AsLanguageManager.Instance.SetFontFromSystemLanguage(btnBuy.spriteText);
				btnBuy.Text = AsTableManager.Instance.GetTbl_String(1145);
			}
		}
	}
}
