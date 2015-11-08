using UnityEngine;
using System.Collections;

public class QuestCashClearPopupController : MonoBehaviour
{
    public SimpleSprite miracleImg;
    public SimpleSprite goldImg;
    public SpriteText textPrice;
    public SpriteText textTitle;
    public SpriteText textSub;
    public UIButton buttonOK;
    public UIButton buttonCancel;

	void Start() 
    {
        AsLanguageManager.Instance.SetFontFromSystemLanguage( textTitle);
        AsLanguageManager.Instance.SetFontFromSystemLanguage( textSub);
        AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonOK.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage( buttonCancel.spriteText);

        textTitle.Text = AsTableManager.Instance.GetTbl_String(816);
        textSub.Text = AsTableManager.Instance.GetTbl_String(817);
        buttonOK.spriteText.Text = AsTableManager.Instance.GetTbl_String(1152);
        buttonCancel.spriteText.Text = AsTableManager.Instance.GetTbl_String(1151);
	}

    public void ShowMessageBox( QuestClearType _clearType, int _clearPrice, MonoBehaviour _mono, string _okFunc, string _calcelFunc)
    {
        gameObject.SetActiveRecursively( true);

        if( _clearType == QuestClearType.CASH)
        {
            miracleImg.Hide( false);
            goldImg.Hide( true);
        }
        else
        {
            miracleImg.Hide( true);
            goldImg.Hide( false);
        }

        textPrice.Text = _clearPrice.ToString();

        buttonOK.scriptWithMethodToInvoke = _mono;
        buttonOK.methodToInvoke = _okFunc;

        buttonCancel.scriptWithMethodToInvoke = _mono;
        buttonCancel.methodToInvoke = _calcelFunc;
    }
}
