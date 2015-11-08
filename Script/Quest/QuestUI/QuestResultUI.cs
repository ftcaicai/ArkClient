using UnityEngine;
using System.Collections.Generic;

public enum QuestResult
{
	QUEST_ACCEPT,
	QUEST_FAILED,
	QUEST_CLEAR,
	QUEST_GIVEUP,
    QUEST_COMPLETE,
	QUEST_RESULT_MAX,
}

public class QuestResultUI : MonoBehaviour {
	
	public  List<GameObject>                      listSprites        = new List<GameObject>();
	public  Dictionary<QuestResult, GameObject>   dicSprites         = new Dictionary<QuestResult, GameObject>();
    private Dictionary<QuestResult, SimpleSprite> dicSimpleSprite    = new Dictionary<QuestResult, SimpleSprite>();
	private SimpleSprite                          nowSpite           = null;
    private bool                                  bShow              = false;
    private bool                                  bHide              = false;
    public  float                                 fShowingTime       = 1.5f;
    private float                                 fPassedTime        = 0.0f;

	// Use this for initialization
	public void Init () 
	{
        
		Object resultUIPrefab = Resources.Load("UI/Optimization/Prefab/Quest_Result");
		
		GameObject objResultUI = GameObject.Instantiate(resultUIPrefab) as GameObject;

        objResultUI.transform.parent = gameObject.transform;
		
		listSprites.Add(objResultUI.transform.FindChild("Accept" ).gameObject);
		listSprites.Add(objResultUI.transform.FindChild("Failed" ).gameObject);
		listSprites.Add(objResultUI.transform.FindChild("Success").gameObject);
		listSprites.Add(objResultUI.transform.FindChild("Giveup" ).gameObject);
        listSprites.Add(objResultUI.transform.FindChild("Done"   ).gameObject);
		
		for(int i=0 ; i < (int)listSprites.Count ; i++)
		{
			dicSprites.Add((QuestResult)i, listSprites[i]);
            SimpleSprite sprite = listSprites[i].GetComponent<SimpleSprite>();
            dicSimpleSprite.Add((QuestResult)i, sprite);
            sprite.Hide(true);
		}
	}
	
	public void ShowResultMessage(QuestResult _result)
	{
        // play sound effect
        PlayResultSound(_result);

        HideAll();
        if (dicSimpleSprite.ContainsKey(_result))
        {
            nowSpite = dicSimpleSprite[_result];
            nowSpite.Start();
            fPassedTime = 0.0f;
            bShow = true;
            nowSpite.Hide(false);
        }
	}
	
	public void PlayResultSound(QuestResult _result)
	{
		string sound = string.Empty;

		switch (_result)
		{
			case QuestResult.QUEST_ACCEPT:
				sound = "Sound/Interface/S6012_EFF_QuestAccept";
				break;

			case QuestResult.QUEST_GIVEUP:
				sound = "Sound/Interface/S6015_EFF_QuestFail";
				break;

			case QuestResult.QUEST_CLEAR:
				sound = "Sound/Interface/S6013_EFF_QuestSuccess";
				break;

			case QuestResult.QUEST_COMPLETE:
				sound = AsSoundPath.QuestComplete;
				break;
			case QuestResult.QUEST_FAILED:
				sound = "Sound/Interface/S6015_EFF_QuestFail";
				break; 
		}
		
		if (sound != string.Empty)
		{
			Debug.Log(sound);
			AsSoundManager.Instance.PlaySound( sound, Vector3.zero, false);
		}
	}

    void HideAll()
    {
        foreach (SimpleSprite sprite in dicSimpleSprite.Values)
            sprite.Hide(true);
    }

    void Update()
    {
        if (nowSpite != null)
        {
            if (bShow == true)
            {
                fPassedTime += Time.deltaTime;

                if (fPassedTime >= fShowingTime)
                {
                    //fHideRate   = 1.0f / fHidingTime;
                    bShow       = false;
                    bHide       = true;
                    fPassedTime = 0.0f;
                }
            }
            
			if (bHide == true)
            {
                bHide = false;
                nowSpite.Hide(true);
            }
        }

    }
}
