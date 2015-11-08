using UnityEngine;
using System.Collections;

public class AsPromotionBtn_Cancel : MonoBehaviour {
	
	AsPromotionDlg.CancelClicked m_Cancel;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		InputProcess();
	}
	
	void OnMouseUpAsButton()
	{
//		collider.enabled = false;
//		
//		m_Cancel();
	}
	
	public void SetInput_Del(AsPromotionDlg.CancelClicked _cancel)
	{
		m_Cancel = _cancel;
	}
	
	void InputProcess()
	{
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
				{
					collider.enabled = false;
					
					m_Cancel();
				}
			}
		}
	}
}
