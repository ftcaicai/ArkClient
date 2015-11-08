using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsEmoticonPanel : MonoBehaviour
{
//	static readonly int s_HuntBtnSize = 21;
//	static readonly int s_NormalBtnSize = 18;
	
	public static readonly int s_BtnCount = 20;
	[SerializeField] float closeTime_ = 3f;
	
//	[SerializeField] UIButton btnClose_ = null;
//	
//	[SerializeField] UIPanelTab panelTabHunt_ = null;
//	[SerializeField] UIPanelTab panelTabNormal_ = null;
//	
//	[SerializeField] GameObject parentHunt_ = null;
//	[SerializeField] GameObject parentNormal_ = null;
	
	[SerializeField] UIScrollList m_scrlList;
	
//	[SerializeField] GameObject dirLeft_ = null;
//	[SerializeField] GameObject dirRight_ = null;
	
	[SerializeField] UIButton[] btnHunt_ = new UIActionBtn[s_BtnCount];
	[SerializeField] UIButton[] btnNormal_ = new UIActionBtn[s_BtnCount];
	
	Dictionary<int, int> m_dicIndex = new Dictionary<int, int>();
	
	void Awake()
	{
		for(int i=0; i<btnHunt_.Length; ++i)
		{			
			UIButton btn = btnHunt_[i];
			Tbl_Emoticon_Record record = AsTableManager.Instance.GetTbl_Emoticon_Record(i + 1);
			string str = AsTableManager.Instance.GetTbl_String(record.ButtonString);		
			SpriteText text = btn.GetComponentInChildren<SpriteText>(); text.Text = str;// text.gameObject.layer = LayerMask.NameToLayer("GUI");
//			SimpleSprite sprite = btn.GetComponentInChildren<SimpleSprite>(); sprite.gameObject.layer = LayerMask.NameToLayer("GUI");
			
			btn.GetComponentInChildren<SpriteText>().Text = str;
			btn.SetInputDelegate(OnBtnClicked);
			
			m_dicIndex.Add(btn.gameObject.GetInstanceID(), i);
		}
		
		for(int i=0; i<btnNormal_.Length; ++i)
		{
			UIButton btn = btnNormal_[i];
			Tbl_Emoticon_Record record = AsTableManager.Instance.GetTbl_Emoticon_Record(s_BtnCount + i + 1);
			string str = AsTableManager.Instance.GetTbl_String(record.ButtonString);
			SpriteText text = btn.GetComponentInChildren<SpriteText>(); text.Text = str;// text.gameObject.layer = LayerMask.NameToLayer("GUI");
//			SimpleSprite sprite = btn.GetComponentInChildren<SimpleSprite>(); sprite.gameObject.layer = LayerMask.NameToLayer("GUI");
			btn.SetInputDelegate(OnBtnClicked);
			
			m_dicIndex.Add(btn.gameObject.GetInstanceID(), s_BtnCount + i);
		}
		
//		foreach(UIButton node in btnHunt_)
//		{
//			if(node.transform.parent != null)
//				m_scrlList.AddItem(node.transform.parent.gameObject);
//			else
//				Debug.LogError("AsEmoticonPanel::Awake: parent is not found");
//		}
		
//		btnClose_.SetInputDelegate(OnBtnCloseClicked);
//		
//		panelTabHunt_.SetInputDelegate(OnTabHuntClicked);
//		panelTabHunt_.GetComponentInChildren<SpriteText>().Text = AsTableManager.Instance.GetTbl_String(1934);
//		
//		panelTabNormal_.SetInputDelegate(OnTabNormalClicked);
//		panelTabNormal_.GetComponentInChildren<SpriteText>().Text = AsTableManager.Instance.GetTbl_String(1935);
//		
//		m_scrlHunt = parentHunt_.GetComponent<UIScrollList>();
	}
	
	void Start ()
	{
//		m_scrlList.itemSpacing = 0;
//		
//		foreach(UIButton node in btnHunt_)
//		{
//			if(node.transform.parent != null)
//				m_scrlList.AddItem(node.transform.parent.gameObject);
//			else
//				Debug.LogError("AsEmoticonPanel::Awake: parent is not found");
//		}
//		
//		m_scrlList.ClipItems();
//		m_scrlList.itemSpacing = 0;
		
		AsChatFullPanel.Instance.EmoticonPanelCreated();
		
		Invoke("CloseProcess", closeTime_);
	}
	
	void CloseProcess()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsEmotionManager.Instance.CloseEmoticonPanel();
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if(AsEmotionManager.Instance.curTab == eEmoticonType.Hunt)
//		{
//			if(m_scrlHunt.ScrollPosition <= 0)
//				dirLeft_.SetActiveRecursively(false);
//			else if(dirLeft_.active == false)
//				dirLeft_.SetActiveRecursively(true);
//			
//			if(m_scrlHunt.ScrollPosition >= 1)
//				dirRight_.SetActiveRecursively(false);
//			else if(dirLeft_.active == false)
//				dirRight_.SetActiveRecursively(true);
//		}
		
		if( true == AsNotify.Instance.IsOpenDeathDlg)
		{
			gameObject.transform.localPosition = new Vector3( gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -23.0f);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3( gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -14.0f);
		}
		
		Touch touch = new Touch();
		if(Input.touchCount > 1)
			touch = Input.GetTouch(0);
		
		if(touch.phase == TouchPhase.Ended || Input.GetMouseButtonUp(0) == true)
		{
			Ray ray = UIManager.instance.rayCamera.ScreenPointToRay( Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) == true)
			{
				if(hit.transform.gameObject == gameObject)
					CloseProcess();
			}
		}
	}
	
//	public void ActivateBtnHunt()
//	{		
//		ActivateDirection(true);
//	}
//	
//	public void ActivateBtnNormal()
//	{
//		panelTabHunt_.SetState(1);
//		panelTabNormal_.SetState(0);
//		
//		parentHunt_.SetActiveRecursively(false);
//		parentNormal_.SetActiveRecursively(true);
//		
//		ActivateDirection(false);
//	}
//	
//	void OnBtnCloseClicked(ref POINTER_INFO ptr)
//	{
//		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			AsEmotionManager.Instance.CloseEmoticonPanel();
//		}
//	}
//	
//	void OnTabHuntClicked(ref POINTER_INFO ptr)
//	{
//		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			ActivateBtnHunt();
//			AsEmotionManager.Instance.SetCurTab(eEmoticonType.Hunt);
//		}
//	}
//	
//	void OnTabNormalClicked(ref POINTER_INFO ptr)
//	{
//		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
//			ActivateBtnNormal();
//			AsEmotionManager.Instance.SetCurTab(eEmoticonType.Normal);
//		}
//	}
	
	void OnBtnClicked(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if(AsChatFullPanel.Instance.CheckChatLocked() == false)
			{
				int instanceId = ptr.hitInfo.transform.gameObject.GetInstanceID();
				if(m_dicIndex.ContainsKey(instanceId) == true)
				{
					int idx = m_dicIndex[instanceId];
					AsEmotionManager.Instance.Request_Emoticon(idx);
				}
			}
			
			AsEmotionManager.Instance.CloseEmoticonPanel();
		}
	}
	
//	void OnMouseUpAsButton()
//	{
//		CloseProcess();
//	}
	
//	void SetLayerHunt()
//	{
//		for(int i=0; i<btnHunt_.Length; ++i)
//		{			
//			UIButton btn = btnHunt_[i];
//			
//			SpriteText text = btn.GetComponentInChildren<SpriteText>(); text.gameObject.layer = LayerMask.NameToLayer("GUI");
//			SimpleSprite sprite = btn.GetComponentInChildren<SimpleSprite>(); sprite.gameObject.layer = LayerMask.NameToLayer("GUI");
//		}
//	}
//	
//	void ActivateDirection(bool _active)
//	{
//		dirLeft_.SetActiveRecursively(_active);
//		dirRight_.SetActiveRecursively(_active);
//	}
}

