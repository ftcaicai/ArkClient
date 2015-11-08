using UnityEngine;
using System.Collections;

public class QuestMarkCotroller : MonoBehaviour {
	
    public GameObject targetObject;
	public GameObject objClearMark;
	public GameObject objNothingMark;
    public GameObject objUpperLvMark;
	public GameObject objAppearEvent;
	public QuestMarkType nowType = QuestMarkType.NOTHING;
	bool       bInit       = false;

    Quaternion quPrevParentRotation = Quaternion.identity;
    Transform  parentTransform      = null;

    public void Init(AsBaseEntity _target)
    {
        targetObject = _target.gameObject;

        //Transform tmDummyTop = _target.GetDummyTransform("DummyLeadTop");

        //if (tmDummyTop == null)
       // {
            transform.parent        = _target.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            CharacterController controller = targetObject.GetComponentInChildren<CharacterController>();

            if (controller != null)
                transform.localPosition = new Vector3(0.0f, controller.height + 1.1f * 1.0f / _target.transform.lossyScale.y, 0.0f);

            //Debug.LogWarning("Can't find dummyleadTop = " + _target.GetComponent<AsNpcEntity>().GetProperty<string>(eComponentProperty.NAME));
       // }
       // else
       // {
         //   transform.parent        = tmDummyTop;
        //    transform.localRotation = Quaternion.identity;
        //    transform.localPosition = Vector3.zero;
       // }

        objClearMark.SetActiveRecursively(false);
        objNothingMark.SetActiveRecursively(false);
        objUpperLvMark.SetActiveRecursively(false);

        parentTransform = transform.parent;

        bInit = true;
    }

    void Update()
    {
        if (AsHUDController.Instance.m_NpcMenu.targetObject != null)
        {
            if (parentTransform == null)
                return;

            if (quPrevParentRotation != parentTransform.rotation)
            {
                Quaternion inverse = Quaternion.Inverse(transform.parent.rotation);

                transform.localRotation = Quaternion.Euler(inverse.eulerAngles.x, inverse.eulerAngles.y + 180.0f, inverse.eulerAngles.z);

                quPrevParentRotation = parentTransform.rotation;
            }
        }
    }
	
	public void SetMarkType(QuestMarkType _type)
	{
		try
		{
			if (bInit == false)
				throw new System.Exception("Not Init");
				
			nowType = _type;

            objClearMark.SetActiveRecursively(false);
            objNothingMark.SetActiveRecursively(false);
            objUpperLvMark.SetActiveRecursively(false);

            if (nowType == QuestMarkType.CLEAR || nowType == QuestMarkType.CLEAR_AND_HAVE || nowType == QuestMarkType.CLEAR_REMAINTALK)
                objClearMark.SetActiveRecursively(true);
            else if (nowType == QuestMarkType.HAVE)
                objNothingMark.SetActiveRecursively(true);
            else if (nowType == QuestMarkType.UPPERLEVEL)
                objUpperLvMark.SetActiveRecursively(true);
		}
		catch(System.Exception e)
		{
			Debug.LogError(e.Message);	
		}
		
	}
}
