using UnityEngine;
using System.Collections;

public class QuestBookBtnControll : MonoBehaviour {

    public SimpleSprite spriteFrame;
    public SpriteText   spriteTextCount;

	// Use this for initialization
	void Start () 
    {
 
	}

    public void UpdateQuestBookBtn()
    {
        try
        {
            int completeCount = ArkQuestmanager.instance.GetClearQuestCount();

            if (completeCount <= 0)
            {
                spriteFrame.gameObject.SetActiveRecursively(false);
                spriteTextCount.gameObject.SetActiveRecursively(false);
            }
            else
            {
                spriteFrame.gameObject.SetActiveRecursively(true);
                spriteTextCount.gameObject.SetActiveRecursively(true);
                spriteTextCount.Text = completeCount.ToString();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

	void OnEnable()
	{
		UpdateQuestBookBtn();
	}
}
