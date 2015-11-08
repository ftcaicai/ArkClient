using UnityEngine;
using System.Collections;

public class AsPvpFightDlg : MonoBehaviour
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
					Close();
			}

			if( true == AsPvpDlgManager.Instance.IsOpenPvpCountDlg)
				AsPvpDlgManager.Instance.ClosePvpCountDlg();
		}
	}

	public void Open(GameObject goRoot)
	{
		gameObject.SetActiveRecursively( true);
		m_goRoot = goRoot;
		
		_InteractivePanel_Reveal();
		
		AsSoundManager.Instance.PlaySound( "Sound/PC/Common/Se_Common_PvPFight", Vector3.zero, false);
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
