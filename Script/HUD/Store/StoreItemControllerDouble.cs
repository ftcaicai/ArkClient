using UnityEngine;
using System.Collections;

public class StoreItemControllerDouble : MonoBehaviour
{

    public  StoreItemControllerInfo itemUp;
    public  StoreItemControllerInfo itemDown;
    public  GameObject  slotDown;
    private bool        bDisableSlotDown = false;

    void OnEnable()
    {
        if (bDisableSlotDown == true)
            slotDown.SetActiveRecursively(false);
    }

    public StoreItemInfoUI GetStoreItemUIInfo(StoreTouchType _touchType)
    {
        if (_touchType == StoreTouchType.STORE_TOUCH_LEFT)
            return itemUp.itemUIInfoData;
        else if (_touchType == StoreTouchType.STORE_TOUCH_RIGHT)
            return itemDown.itemUIInfoData;

        return null;
    }

    //public void SetDownSlotVisible(bool _visible)
    //{
    //    if (slotDown != null)
    //    {
    //        slotDown.SetActiveRecursively(_visible);
    //        bDisableSlotDown = !_visible;
    //    }
    //}

    public void SetFocus(GameObject _focusObject, StoreTouchInfo _touchInfo)
    {
        if (_touchInfo == null)
            return;

        StoreItemControllerInfo selectItem = null;

        _focusObject.SetActiveRecursively(true);

        SimpleSprite sprite = _focusObject.GetComponent<SimpleSprite>();
        sprite.Unclip();


        if (_touchInfo.type == StoreTouchType.STORE_TOUCH_LEFT)
            selectItem = itemUp;
        else if (_touchInfo.type == StoreTouchType.STORE_TOUCH_RIGHT)
            selectItem = itemDown;

        if (selectItem != null)
        {
            selectItem.itemFocusObject           = _focusObject;
            _focusObject.transform.parent        = selectItem.itemSlotSprite.transform;
            _focusObject.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f);
            selectItem.uiListItemContainer.ScanChildren();
        }
    }

    public void RemoveFocus(Transform _parent)
    {
        if (itemUp.itemFocusObject != null)
        {
            itemUp.itemFocusObject.transform.parent = _parent;
            itemUp.itemFocusObject.SetActiveRecursively(false);
            itemUp.itemFocusObject = null;
            itemUp.uiListItemContainer.ScanChildren();
        }

        if (itemDown.itemFocusObject != null)
        {
            itemDown.itemFocusObject.transform.parent = _parent;
            itemDown.itemFocusObject.SetActiveRecursively(false);
            itemDown.itemFocusObject = null;
            itemDown.uiListItemContainer.ScanChildren();
        }
    }
}
