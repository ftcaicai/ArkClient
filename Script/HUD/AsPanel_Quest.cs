using UnityEngine;
using System.Collections;

public enum QuestPanelType : int
{
    QUEST_NEW,
    QUEST_NEW_UPPERLEVEL,
    QUEST_NEW_TALK,
    QUEST_CLEAR,
	QUEST_HAVE_EVENT,
	QUEST_HAVE_NPC_DAILY,
	QUEST_CLEAR_NPC_DAILY,
}

public class AsPanel_Quest : MonoBehaviour {

    private AsBaseEntity m_baseEntity   = null;
    public SimpleSprite[] questSprites  = null;
    private Vector3      m_vUIPosRevision_Img;
    private Vector3      m_vUIPosRevision;
    private float        m_fNamePanelLayer = 15.0f;
    private Transform    dummyLeadTop;
    private bool         m_bShowCommand = false;
    private QuestMarkType nowType = QuestMarkType.NOTHING;


    void Start () 
    {
        if (false == m_bShowCommand)
        {
            gameObject.SetActive(false);

            foreach (SimpleSprite sprite in questSprites)
                sprite.renderer.enabled = false;
        }
	}

    public void SetMarkType(QuestMarkType _type)
    {
        try
        {
            nowType = _type;

            foreach (SimpleSprite sprite in questSprites)
				sprite.gameObject.SetActive(false);

			if (nowType == QuestMarkType.CLEAR_REMAINTALK || nowType == QuestMarkType.TALK_CLEAR || nowType == QuestMarkType.TALK_HAVE)
				questSprites[(int)QuestPanelType.QUEST_NEW_TALK].gameObject.SetActive(true);
			else if (nowType == QuestMarkType.CLEAR || nowType == QuestMarkType.CLEAR_AND_HAVE)
				questSprites[(int)QuestPanelType.QUEST_CLEAR].gameObject.SetActive(true);
			else if (nowType == QuestMarkType.HAVE)
				questSprites[(int)QuestPanelType.QUEST_NEW].gameObject.SetActive(true);
			else if (nowType == QuestMarkType.UPPERLEVEL)
				questSprites[(int)QuestPanelType.QUEST_NEW_UPPERLEVEL].renderer.enabled = true;
			else if (nowType == QuestMarkType.HAVE_EVENT || nowType == QuestMarkType.HAVE_EVENT_AND_PROGRESS || nowType == QuestMarkType.LOWERLEVEL_AND_HAVE_EVENT)
				questSprites[(int)QuestPanelType.QUEST_HAVE_EVENT].gameObject.SetActive(true);
			else if (nowType == QuestMarkType.HAVE_NPC_DAILY)
				questSprites[(int)QuestPanelType.QUEST_HAVE_NPC_DAILY].gameObject.SetActive(true);
			else if (nowType == QuestMarkType.CLEAR_NPC_DAILY)
				questSprites[(int)QuestPanelType.QUEST_CLEAR_NPC_DAILY].gameObject.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

    }

    public void Create(AsBaseEntity _baseEntity, float _namePanelPosY)
    {
        m_bShowCommand = true;

        m_baseEntity = _baseEntity;

        gameObject.SetActive(true);

//        string strNameRes = string.Empty;
//        string strNameBuf = string.Empty;

        m_vUIPosRevision.x = 0.0f;
        m_vUIPosRevision.y = _namePanelPosY  + questSprites[0].height * 0.5f;
        m_vUIPosRevision.z = m_fNamePanelLayer;


        dummyLeadTop = m_baseEntity.GetDummyTransform("DummyLeadTop");
        if (null == dummyLeadTop)
		{
			if( true == m_baseEntity.isKeepDummyObj)
			{
				Vector3 vPos = m_baseEntity.transform.position;
				vPos.y += m_baseEntity.characterController.height;
				transform.position = _WorldToUIPoint( vPos, m_vUIPosRevision);
			}
			else
				Debug.LogWarning("DummyLeadTop is not found");
		}
        else
            transform.position = _WorldToUIPoint(dummyLeadTop.position, m_vUIPosRevision);//////////

        _baseEntity.questPanel = this;
    }

    void LateUpdate()
    {
        if (false == m_bShowCommand)
            return;

        if (null == m_baseEntity || null == m_baseEntity.ModelObject)
        {
            Remove();
        }
        else
            UpdatePos();
    }


    private void UpdatePos()
    {
        Vector3 vScreenPos = Vector3.zero;
		if( null == dummyLeadTop)
		{
			if( true == m_baseEntity.isKeepDummyObj)
			{
				Vector3 vPos = m_baseEntity.transform.position;
				vPos.y += m_baseEntity.characterController.height;
				vScreenPos = _WorldToScreenPoint( vPos);
			}
		}
		else
             vScreenPos = _WorldToScreenPoint(dummyLeadTop.position);

        transform.position = _ScreenPointToUIRay(vScreenPos, m_vUIPosRevision);
    }

    public void OnEnable()
    {
        if (false == m_bShowCommand)
        {
            SetMarkType(nowType);
            gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        if (true == m_bShowCommand)
        {
            SetMarkType(nowType);
            transform.position = Vector3.zero;
        }
    }

    private void Remove()
    {
        transform.parent = null;
        DestroyImmediate(gameObject);
    }

    // < private
    private Vector3 _WorldToUIPoint(Vector3 vWorldPos, Vector3 vUIPosRevision)
    {
        Vector3 vScreenPos = _WorldToScreenPoint(vWorldPos);
        Vector3 vRes       = _ScreenPointToUIRay(vScreenPos, vUIPosRevision);
        return vRes;
    }

    private Vector3 _WorldToScreenPoint(Vector3 vWorldPos)
    {
        return CameraMgr.Instance.WorldToScreenPoint(vWorldPos);
    }

    private Vector3 _ScreenPointToUIRay(Vector3 vScreenPos, Vector3 vUIPosRevision)
    {
        Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay(vScreenPos);
        vRes.x += vUIPosRevision.x;
        vRes.y += vUIPosRevision.y;
        vRes.z  = vUIPosRevision.z;
        return vRes;
    }
	
}
