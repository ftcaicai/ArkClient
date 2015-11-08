using UnityEngine;
using System.Collections;

public class QuestClearBtnContoller : MonoBehaviour {

    public SimpleSprite miracleImg;
    public SimpleSprite goldImg;
    public SpriteText   textPrice;
    public SpriteText   textTitle;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowButton(QuestProgressState _questProgress, QuestClearType _clearType, int _price)
    {
        if (miracleImg == null || goldImg == null || textPrice == null || textTitle == null || _questProgress != QuestProgressState.QUEST_PROGRESS_IN)
        {
            gameObject.SetActiveRecursively(false);
            miracleImg.Hide(true);
            goldImg.Hide(true);
            return;
        }

        if (_clearType == QuestClearType.NONE)
        {
            gameObject.SetActiveRecursively(false);
            miracleImg.Hide(true);
            goldImg.Hide(true);
        }
        else if (_clearType == QuestClearType.CASH)
        {
            gameObject.SetActiveRecursively(true);
            miracleImg.Hide(false);
            goldImg.Hide(true);
            textPrice.Text = _price.ToString();
        }
        else if (_clearType == QuestClearType.GOLD)
        {
            gameObject.SetActiveRecursively(true);
            miracleImg.Hide(true);
            goldImg.Hide(false);
            textPrice.Text = _price.ToString();
        }
    }
}
