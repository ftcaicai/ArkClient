using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsChatMacroContainer : MonoBehaviour {
	
	[SerializeField] List<UIButton> m_listButton = new List<UIButton>();
	[SerializeField] AsSlotUseEffect m_Effect;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 3f);
		
		m_Effect.SetLoop(true);
		m_Effect.Enable = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ButtonActivate(int _idx)//, EZInputDelegate _del)
	{
		UIButton btn = m_listButton[_idx];
		btn.SetInputDelegate(OnIconClicked);
		
		btn.GetComponentInChildren<SpriteText>().Text = AsTableManager.Instance.GetTbl_String(AsTableManager.Instance.GetTbl_Emoticon_Record(_idx + 1).ButtonString);
		
		foreach(UIButton node in m_listButton)
		{
			if(node != btn)
				Destroy(node.gameObject);
		}
	}
	
	void OnIconClicked(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsEmotionManager.Instance.ButtonClicked();
			Destroy(gameObject);
		}
	}
}
