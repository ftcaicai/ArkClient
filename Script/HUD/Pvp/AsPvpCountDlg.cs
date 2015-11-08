using UnityEngine;
using System.Collections;

public class AsPvpCountDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	private float m_fTime = 0.0f;

	void Start()
	{
	}
	
	void Update()
	{
		float fCurTime = Time.realtimeSinceStartup;
		
		if( fCurTime > m_fTime + 0.2f)
		{
			m_fTime = fCurTime;
			
			UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
			foreach( UIInteractivePanel panel in panels)
			{
				if( false == panel.IsTransitioning)
					panel.Reveal();
			}
		}
		
		if( AsUserInfo.Instance.SavedCharStat.hpCur_ <= 0)
			Close();
	}

	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		_InteractivePanel_Reveal();
		
		AsInputManager.Instance.m_Activate = true;
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}

	private void _InteractivePanel_Reveal()
	{
		UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
		foreach( UIInteractivePanel panel in panels)
			panel.Reveal();
	}
	
	private void _InteractivePanel_End()
	{
		UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
		foreach( UIInteractivePanel panel in panels)
			panel.Hide();
	}
}
