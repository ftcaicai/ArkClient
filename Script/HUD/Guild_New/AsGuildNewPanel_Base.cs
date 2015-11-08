using UnityEngine;
using System.Collections;

public class AsGuildNewPanel_Base : MonoBehaviour 
{
	protected 	eGuildNewPanelState		m_panelState = eGuildNewPanelState.Invalid;

	public eGuildNewPanelState PanelState
	{
		get{return m_panelState;}
		set{m_panelState = value;}
	}

	public virtual void Init(System.Object data) { }
	public virtual void InsertData( System.Object data) {}
	public virtual void UpdateData( System.Object data) {}
	public virtual void RequestCurrentPage() { Debug.LogError ("AsGuildNewPanel_Base RequestCurrentPage"); }
}
