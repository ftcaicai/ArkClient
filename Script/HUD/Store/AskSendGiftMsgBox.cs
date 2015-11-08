using UnityEngine;
using System.Collections;

public class AskSendGiftMsgBox : MonoBehaviour {

    public SpriteText txtTitle;
    public UIButton   btnBuy;
    public UIButton   btnGift;
    public UIButton   btnClose;

    MonoBehaviour script = null;
    string buyFuncName;
    string sendGiftFuncName;
    string closeFuncName;

	void Start () 
    {
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnBuy.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnGift.spriteText); 

        // change
        txtTitle.Text           = AsTableManager.Instance.GetTbl_String(1716);
        btnBuy.spriteText.Text  = AsTableManager.Instance.GetTbl_String(1715);
        btnGift.spriteText.Text = AsTableManager.Instance.GetTbl_String(1716);

        // set delegate
        btnBuy.SetInputDelegate(BuyDelegate);
        btnGift.SetInputDelegate(SendGiftDeleate);
        btnClose.SetInputDelegate(Close);
	}

    public void Initilize(MonoBehaviour _script, string _szBuyFunc, string _szSendGiftFunc, string _szCloseFunc)
    {
        script = _script;

        buyFuncName      = _szBuyFunc;
        sendGiftFuncName = _szSendGiftFunc;
        closeFuncName    = _szCloseFunc;
		if(WemeSdkManager.Instance.IsConfirmGuest)
		{
			btnGift.SetControlState(UIButton.CONTROL_STATE.DISABLED);
		}
    }

    public void BuyDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            if (script != null)
                script.Invoke(buyFuncName, 0.0f);

            GameObject.Destroy(gameObject);
        }
    }

    public void SendGiftDeleate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            if (script != null)
                script.Invoke(sendGiftFuncName, 0.0f);

            GameObject.Destroy(gameObject);
        }
    }

    public void Close(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            if (script != null)
                script.Invoke(closeFuncName, 0.0f);

            GameObject.Destroy(gameObject);
        }
    }
}
