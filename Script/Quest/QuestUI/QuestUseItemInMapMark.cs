using UnityEngine;
using System.Collections;

public class QuestUseItemInMapMark : MonoBehaviour {

    public int         questTableID;
    public int         itemID;
    public Vector3     vPos3D;
    public int         radius;
    public BoxCollider collider;
    public Vector3     vOffset;
	public AchMapInto  mapInfo;

	// Use this for initialization
	void Start () {

        vOffset = new Vector3(0.0f, 0.0f, 20.0f);

		//DontDestroyOnLoad( gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePos();
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(vPos3D, (float)radius);
    }

    public void UpdatePos()
    {
        Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay(CameraMgr.Instance.WorldToScreenPoint(vPos3D));

        transform.position = vRes;

        transform.position = new Vector3(vRes.x, vRes.y, 13.0f);
    }

    public bool UseItem(Ray _ray)
    {
        if (AsUtil.PtInCollider(collider, _ray))
        {
             RealItem item = ItemMgr.HadItemManagement.Inven.GetRealItem(itemID);

			 if (item != null)
			 {
				 if (item.item.ItemData.GetItemType() == Item.eITEM_TYPE.UseItem)
				 {
					 if (item.item.ItemData.GetSubType() == (int)Item.eUSE_ITEM.QuestRandom)
						AsHudDlgMgr.Instance.OpenRandomItemDlg(item);
					 else
						 AsCommonSender.SendUseItem(item.getSlot);
				 }
				 return true;
			 }
			 else
			 {
				 string msg = AsTableManager.Instance.GetTbl_String(954);
				 AsChatManager.Instance.InsertChat(msg, eCHATTYPE.eCHATTYPE_SYSTEM);
				 AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessage(msg, true);
			 }
        }

        return false;
    }
}
