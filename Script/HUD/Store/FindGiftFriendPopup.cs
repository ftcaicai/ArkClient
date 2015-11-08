using UnityEngine;
using System.Collections;

public class FindGiftFriendPopup : MonoBehaviour {

    public UITextField txtEdit;
    public SpriteText txtName;
    public SpriteText txtLevel;
    public SpriteText txtClass;
    public SpriteText txtNameLabel;
    public SpriteText txtLvLabel;
    public SpriteText txtClassLabel;
    public SpriteText txtNoticeLabel;
    public SpriteText txtTitle;

    public UIButton btnConfirm;
    public UIButton btnCancel;
    public UIButton btnClose;
    public UIButton btnFind;


    string confirmFuncName;
    string cancelFuncName;
    MonoBehaviour script;

    bool receiveGiftFriendInfo = false;
    bool lockInput             = false;

	// Use this for initialization
	void Start () 
    {
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtEdit);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtName);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtLevel);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtClass);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnConfirm.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnCancel.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnClose.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(btnFind.spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtNameLabel);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtLvLabel);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtClassLabel);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtNoticeLabel);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(txtTitle);

        btnCancel.SetInputDelegate(Cancel);
        btnClose.SetInputDelegate(Cancel);
        btnFind.SetInputDelegate(FindFriend);
		btnConfirm.SetInputDelegate(Confirm);

        btnConfirm.spriteText.Text = AsTableManager.Instance.GetTbl_String(1141);
        btnCancel.spriteText.Text  = AsTableManager.Instance.GetTbl_String(1027);
        btnFind.spriteText.Text    = AsTableManager.Instance.GetTbl_String(1718);
        

        txtEdit.SetValidationDelegate(OnValidateName);

        txtTitle.Text        = AsTableManager.Instance.GetTbl_String(1716);
        txtNameLabel.Text    = AsTableManager.Instance.GetTbl_String(1717);
        txtNoticeLabel.Text  = AsTableManager.Instance.GetTbl_String(1723);
        txtLvLabel.Text      = AsTableManager.Instance.GetTbl_String(1724);
        txtClassLabel.Text   = AsTableManager.Instance.GetTbl_String(1250);
        

        btnConfirm.SetControlState(UIButton.CONTROL_STATE.DISABLED);
	}

    public void Initilize(MonoBehaviour _script, string _confirmFuncName, string _cancelFuncName)
    {
        script = _script;
        confirmFuncName = _confirmFuncName;
        cancelFuncName  = _cancelFuncName;
    }

    void FindFriend(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && lockInput == false)
        {
            if (txtName.Text != string.Empty)
            {
				AsSoundManager.Instance.PlaySound(AsSoundPath.Cashshop_FindFriend_Btn, Vector3.zero, false);
                receiveGiftFriendInfo = false;
                AsLoadingIndigator.Instance.ShowIndigator("");
                lockInput = true;
                AsCommonSender.SendRequestFindGiftFirend(txtName.Text);
            }
        }
    }

    void Confirm(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP && lockInput == false)
        {
            if (script != null)
				if (receiveGiftFriendInfo == true)
				{
					script.Invoke(confirmFuncName, 0.0f);
					AsSoundManager.Instance.PlaySound(AsSoundPath.Cashshop_FindFriend_Btn, Vector3.zero, false);
				}

            GameObject.Destroy(gameObject);
        }
    }

    public void Cancel(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            if (script != null)
                script.Invoke(cancelFuncName, 0.0f);

            GameObject.Destroy(gameObject);
        }
    }

    public void ReceiveFriendInfo(uint _nGiftAccount, int _nLevel, int _eClass)
    {
        if (_nGiftAccount != 0)
        {
            txtLevel.Text = _nLevel.ToString();
            txtClass.Text = AsUtil.GetClassName(((eCLASS)_eClass));
            receiveGiftFriendInfo = true;
            btnConfirm.SetControlState(UIButton.CONTROL_STATE.NORMAL);
        }
        else
        {
            ResetGiftFriendInfo();
        }

        lockInput = false;
        AsLoadingIndigator.Instance.HideIndigator();
    }

    void OnEnable()
    {
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    void OnDisable()
    {
        Input.imeCompositionMode = IMECompositionMode.Auto;
    }

    void OnDestory()
    {
        AsLoadingIndigator.Instance.HideIndigator();
    }

    void ResetGiftFriendInfo()
    {
        receiveGiftFriendInfo = false;
        txtLevel.Text = string.Empty;
        txtClass.Text = string.Empty;
        btnConfirm.SetControlState(UIButton.CONTROL_STATE.DISABLED);
    }

    private string OnValidateName(UITextField field, string text, ref int insPos)
    {
        ResetGiftFriendInfo();

        while (true)
        {
            int byteCount = System.Text.UTF8Encoding.UTF8.GetByteCount(text);
            int charCount = System.Text.UTF8Encoding.UTF8.GetCharCount(System.Text.UTF8Encoding.UTF8.GetBytes(text));
            if ((byteCount <= 24) && (charCount <= 12))
                break;

            text = text.Remove(text.Length - 1);
        }

        return System.Text.RegularExpressions.Regex.Replace(text, "\\s+", "");
    }
}
