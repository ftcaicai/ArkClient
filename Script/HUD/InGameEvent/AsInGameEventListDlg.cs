using UnityEngine;
using System.Collections.Generic;

public class AsInGameEventListDlg: MonoBehaviour {
	
	public GameObject   objParent;
	public GameObject	listItemPrefab;
	public UIButton		btnClose;
	public SpriteText	txtTitle;
	public UIScrollList eventScrollList;
	public int			npcID;
	
	private List<Tbl_Event_Record> listEvent = new List<Tbl_Event_Record>();
	
	void Start () 
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtTitle);
		txtTitle.Text = AsTableManager.Instance.GetTbl_String(1100);
		btnClose.SetInputDelegate(CloseBtnInputProcess);	
	}
	
	public int Show(int _npcID = -1)
	{
		AsSoundManager.Instance.PlaySound(AsSoundPath.WindowOpen, Vector3.zero, false, false);

		System.DateTime nowDate = System.DateTime.Now.AddTicks(AsGameMain.serverTickGap);

		listEvent = _npcID == -1 ? AsTableManager.Instance.GetTbl_Event(nowDate) : AsTableManager.Instance.GetTbl_Event(_npcID, nowDate);
		
		npcID = _npcID;

		// list count is 1
		if (listEvent.Count == 1)
		{
			// not visible
			if (listEvent[0].viewList == true)
				return 0;

			ShowEventAchieveDlg(listEvent[0].eventIdx);
			return listEvent.Count;
		}
		else
		{
			int count = 0;
			foreach (Tbl_Event_Record eventRecord in listEvent)
			{
				if (eventRecord.viewList == true)
					continue;

				AddListItem(eventRecord);
				count++;
			}

			return count;
		}
	}
	
	void AddListItem(Tbl_Event_Record _event)
	{
		UIListItem listItem = eventScrollList.CreateItem(listItemPrefab, eventScrollList.Count, true) as UIListItem;
		listItem.AddInputDelegate(EventListInputProcess);

		listItem.spriteText.Text = AsTableManager.Instance.GetTbl_String(_event.titleID);
		listItem.Data			 = _event.eventIdx;
		listItem.SetOffset(new Vector3(0.0f, 0.0f, -0.2f));	
	}
	
	void CloseBtnInputProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			Close(true);	
	}

	public void Close(bool _useSound = true)
	{
		if (_useSound == true)
			AsSoundManager.Instance.PlaySound(AsSoundPath.WindowClose, Vector3.zero, false, false);

		Destroy(objParent);
	}
	
	void EventListInputProcess(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int eventID = (int)ptr.targetObj.Data;

			ShowEventAchieveDlg(eventID);
		}	
	}

	void ShowEventAchieveDlg(int _eventID)
	{
		AsHudDlgMgr.Instance.OpenInGameEventAchievementDlg(npcID, _eventID);

		AsHudDlgMgr.Instance.CloseInGameEventListDlg(false);
	}
}
