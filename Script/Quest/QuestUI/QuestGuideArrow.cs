using UnityEngine;
using System.Collections;


public class QuestGuideArrow : MonoBehaviour 
{

    public GameObject[]        objGuideArrows;
	public GameObject          objGuideGo;
    public GameObject          nowGuideArrow;
    public QuestGuideDirection nowDirection    = QuestGuideDirection.NONE;
    public bool                bVisible        = false;
    public bool                bTwinkle        = false;
    public int                 nTwinkleCount   = 2;
    public float               fHideTime       = 3.0f;
    public float               fDestAlpha      = 0.2f;
    public float               fShowTime       = 0.5f;
 
    float fRemainShowTime   = 0.0f;

	void Start () 
    { 
		
	}

    public void Init()
    {
        bVisible          = false;
        DisableAllArrow();
    }

    public void DisableAllArrow()
    {
        foreach (GameObject arrow in objGuideArrows)
            arrow.SetActiveRecursively(false);

		objGuideGo.SetActiveRecursively(false);
    }
	
	// Update is called once per frame
	void Update () 
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.J))
            Init();

        if (Input.GetKeyDown(KeyCode.K))
            ShowGuideArrow(nowDirection);
#endif


		if (fRemainShowTime > 0.0f)
		{
			fRemainShowTime -= Time.deltaTime;
			//return;
		}
		else
		{
			if (nowGuideArrow != null)
			{
				nowGuideArrow.SetActiveRecursively(false);
				objGuideGo.SetActiveRecursively(false);
			}
		}


		//if (bVisible == true && nowTwinkleCount > 0)
		//{
		//    fNowAlpha = 1.0f - (fTime / fHideTime);

		//    if (fNowAlpha >= fNowDestAlpha)
		//    {
		//        objGuideGo.renderer.sharedMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, fNowAlpha));
		//    }
		//    else
		//    {
		//        fTime = 0.0f;
		//        fNowAlpha = 0.0f;
		//        nowTwinkleCount--;
		//        objGuideGo.SetActiveRecursively(false);

		//        if (nowTwinkleCount == 1)
		//            fNowDestAlpha = 0.0f;

		//        if (nowTwinkleCount > 0)
		//        {
		//            objGuideGo.SetActiveRecursively(true);
		//            objGuideGo.renderer.sharedMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
		//            fRemainShowTime = fShowTime;
		//        }
		//    }

		//    fTime += Time.deltaTime;
		//}
	}

    //public void ShowGuideArrow(QuestGuideDirection _quideDirect)
    //{
    //    if (objGuideArrow == null || _quideDirect == QuestGuideDirection.NONE || _quideDirect  == QuestGuideDirection.CENTER)
    //        return;

    //    nowDirection = _quideDirect;

    //    int idx = (int)_quideDirect;

    //    Debug.LogWarning("idx = " + idx);
    //    Debug.LogWarning("angle = " + rotationAngles[idx]);
    //    Debug.LogWarning("ani name = " + directionAnimations[idx].name);

    //    objGuideArrow.transform.rotation = Quaternion.AngleAxis(rotationAngles[idx], Vector3.forward);
    //    objGuideText.transform.localRotation = Quaternion.AngleAxis(-rotationAngles[idx], Vector3.forward);

    //    if (_quideDirect != QuestGuideDirection.LEFT)
    //    {
    //        ani.clip = directionAnimations[idx];
    //        ani.Play(directionAnimations[idx].name, AnimationPlayMode.Stop);
    //    }
       
    //    objGuideArrow.SetActiveRecursively(true);
    //    bVisible = true;
      
    //    fTime = 0.0f;
    //}

    public void ShowGuideArrow(QuestGuideDirection _quideDirect)
    {
        DisableAllArrow();

        if (_quideDirect == QuestGuideDirection.NONE || _quideDirect == QuestGuideDirection.CENTER)
            return;

        nowDirection = _quideDirect;

        int idx = (int)_quideDirect;

        nowGuideArrow = objGuideArrows[idx];
		objGuideGo.SetActiveRecursively(true);
        nowGuideArrow.SetActiveRecursively(true);
        nowGuideArrow.renderer.sharedMaterial.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        bVisible = true;
        fRemainShowTime = fShowTime;
    }
}
