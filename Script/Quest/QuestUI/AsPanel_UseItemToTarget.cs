using UnityEngine;
using System.Collections;


public class AsPanel_UseItemToTarget : MonoBehaviour
{

    private AsBaseEntity m_baseEntity = null;
   // public SimpleSprite[] questSprites = null;
    private Vector3 m_vUIPosRevision_Img;
    private Vector3 m_vUIPosRevision;
    private float m_fNamePanelLayer = 15.0f;
    private Transform dummyLeadTop;
    private bool m_bShowCommand = false;
//    private QuestMarkType nowType = QuestMarkType.NOTHING;
    private BoxCollider itemCollider  = null;

	private UISlotItem slotItem;
	private CoolTimeGroup coolTimeGroup;

    AchUseItemToTarget achUseItemToTarget = null;

    public AsBaseEntity GetBaseEntity 
    { 
        get {  return m_baseEntity;  } 
    }

    public AchUseItemToTarget GetAchUseItemToTarget
    {
        get { return achUseItemToTarget; }
    }

	public UISlotItem SlotItem { get { return slotItem; } }
	public CoolTimeGroup CoolTimeGroup { get { return coolTimeGroup; } }

    void Start()
    {
        //if (false == m_bShowCommand)
        //{
        //    gameObject.SetActiveRecursively(false);

        //    foreach (SimpleSprite sprite in questSprites)
        //        sprite.Hide(true);
        //}
    }

    //public void SetMarkType(QuestMarkType _type)
    //{
    //    try
    //    {
    //        nowType = _type;

    //        foreach (SimpleSprite sprite in questSprites)
    //            sprite.Hide(true);

    //        if (nowType == QuestMarkType.CLEAR_REMAINTALK || nowType == QuestMarkType.TALK_CLEAR || nowType == QuestMarkType.TALK_HAVE)
    //            questSprites[(int)QuestPanelType.QUEST_NEW_TALK].Hide(false);
    //        else if (nowType == QuestMarkType.CLEAR || nowType == QuestMarkType.CLEAR_AND_HAVE)
    //            questSprites[(int)QuestPanelType.QUEST_CLAR].Hide(false);
    //        else if (nowType == QuestMarkType.HAVE)
    //            questSprites[(int)QuestPanelType.QUEST_NEW].Hide(false);
    //        else if (nowType == QuestMarkType.UPPERLEVEL)
    //            questSprites[(int)QuestPanelType.QUEST_NEW_UPPERLEVEL].Hide(false);
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError(e.Message);
    //    }

    //}

    public void Initilize(AsBaseEntity _baseEntity, BoxCollider _collider,  float _namePanelPosY, AchUseItemToTarget _achUseItemToTarget, UISlotItem _slotItem, CoolTimeGroup _coolTimeGroup)
    {
        m_bShowCommand = true;

        m_baseEntity = _baseEntity;

        achUseItemToTarget = _achUseItemToTarget;

		slotItem = _slotItem;

		coolTimeGroup = _coolTimeGroup;

        gameObject.SetActiveRecursively(true);

        if (_baseEntity.questPanel != null)
            _baseEntity.questPanel.gameObject.SetActiveRecursively(false);

		if (_baseEntity.collectionMark != null)
			_baseEntity.collectionMark.Visible = false;

        itemCollider = _collider;
        
        // Calculate U.I Position
        m_vUIPosRevision.x = 0.0f;
        m_vUIPosRevision.y = _namePanelPosY;// +questSprites[0].height * 0.5f;
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
            //SetMarkType(nowType);
            gameObject.SetActiveRecursively(false);
        }
    }

    public void OnDisable()
    {
        if (true == m_bShowCommand)
        {
           // SetMarkType(nowType);
            transform.position = Vector3.zero;
        }
    }

    private void Remove()
    {
        transform.parent = null;
        DestroyImmediate(gameObject);
    }

    // < private
    private Vector3 _WorldToUIPoint(Vector3 _vWorldPos, Vector3 _vUIPosRevision)
    {
        Vector3 vScreenPos = _WorldToScreenPoint(_vWorldPos);
        Vector3 vRes = _ScreenPointToUIRay(vScreenPos, _vUIPosRevision);
        return vRes;
    }

    private Vector3 _WorldToScreenPoint(Vector3 _vWorldPos)
    {
        return CameraMgr.Instance.WorldToScreenPoint(_vWorldPos);
    }

    private Vector3 _ScreenPointToUIRay(Vector3 _vScreenPos, Vector3 _vUIPosRevision)
    {
        Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay(_vScreenPos);
        vRes.x += _vUIPosRevision.x;
        vRes.y += _vUIPosRevision.y;
        vRes.z  = _vUIPosRevision.z;
        return vRes;
    }

    public bool UseItem(Ray _ray)
    {
        if (AsUtil.PtInCollider(itemCollider, _ray))
        {
            if (achUseItemToTarget != null)
            {
                RealItem item = ItemMgr.HadItemManagement.Inven.GetRealItem(achUseItemToTarget.ItemID);

				if (item != null)
				{
					if (item.IsCanCoolTimeActive() == false)
					{
						int npcID = -1;

						if (m_baseEntity.FsmType == eFsmType.COLLECTION || m_baseEntity.FsmType == eFsmType.NPC)
						{
							AsNpcEntity npcEntity = (AsNpcEntity)m_baseEntity;
							npcID = npcEntity.TableIdx;
						}

						AsCommonSender.SendUseItem(item.getSlot, npcID);
						return true;
					}
				}
				else
				{
					string msg = AsTableManager.Instance.GetTbl_String(954);
					AsChatManager.Instance.InsertChat(msg, eCHATTYPE.eCHATTYPE_SYSTEM);
					AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(msg, true);
				}
            }
            else
                return false;
        }

        return false;
    }

}
