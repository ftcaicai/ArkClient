using UnityEngine;
using System.Collections;

public class QuestWantedBtnControll : MonoBehaviour
{
    [SerializeField]UIButton btn = null;
    [SerializeField]SpriteText btnText = null;
    [SerializeField]SimpleSprite spriteNormal = null;
    [SerializeField]SimpleSprite spriteDone = null;
	[SerializeField]GameObject balloon = null;
    public QuestProgressStateNet wantedState = QuestProgressStateNet.QUEST_NOTHING;
    private bool bClear = false;
	private System.DateTime startTime;
	private bool blinkState = false;

    public bool Clear
    {
        get { return bClear; }
        set
        {
            bClear = value;

            spriteNormal.Hide( value);
            spriteDone.Hide( !value);
        }
    }

	// Use this for initialization
	void Start() 
    {
        AsLanguageManager.Instance.SetFontFromSystemLanguage( btnText);
        btn.Text = AsTableManager.Instance.GetTbl_String(874);
        btn.AddInputDelegate( ButtonInputProcess);
	}
	
	public void CheckVisible()
	{
        if (wantedState == QuestProgressStateNet.QUEST_WANTED_NEW)
        {
            startTime = System.DateTime.Now;
            balloon.SetActiveRecursively(true);
            blinkState = false;

            CancelInvoke("Blink");
            InvokeRepeating("Blink", 0.0f, 0.5f);
        }
        else
            CancelBlink();
	}

    void OnEnable()
    {
        if (wantedState != QuestProgressStateNet.QUEST_WANTED_NEW)
            balloon.SetActiveRecursively(false);
    }

    void CancelBlink()
    {
        wantedState = QuestProgressStateNet.QUEST_NOTHING;
        blinkState = false;
        spriteNormal.Hide(false);
        spriteDone.Hide(true);
        balloon.SetActiveRecursively(false);
        CancelInvoke("Blink");
    }

	void Blink()
	{
		System.TimeSpan timeSpan = System.DateTime.Now - startTime;
		if( 5 <= timeSpan.TotalSeconds)
		{
            CancelBlink();
			return;
		}
		
        spriteNormal.Hide( blinkState);
        spriteDone.Hide( !blinkState);
		blinkState = !blinkState;
	}
	
    void ButtonInputProcess( ref POINTER_INFO ptr)
    {
        if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
			if (AsUserInfo.Instance.IsDied())
				return;

			AsChatFullPanel.Instance.Close();
			
            ArkQuest wantedQuest = ArkQuestmanager.instance.GetWantedQuest();

            if( wantedQuest != null)
            {
                if (AsEntityManager.Instance.UserEntity.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == true)
                    AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365));
                else
                {
                    AsHudDlgMgr.Instance.OpenQuestAcceptUI(wantedQuest.GetQuestData(), true);
                    CancelBlink();
                }
            }
            else
                Debug.Log( "wanted quest is null");
        }
    }
}
