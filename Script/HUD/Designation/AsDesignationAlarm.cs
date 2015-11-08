using UnityEngine;
using System.Collections;

public class AsDesignationAlarm : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText designation = null;

	// Use this for initialization
	IEnumerator Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1762);
		
		UIPanel[] panels = gameObject.GetComponentsInChildren<UIPanel>();
		foreach( UIPanel panel in panels)
			panel.BringIn();
		
		yield return new WaitForSeconds( 5.0f);
		
		GameObject.DestroyImmediate( gameObject);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetDesignationText( int designationID)
	{
		DesignationData data = AsDesignationManager.Instance.GetDesignation( designationID);
		designation.Text = data.nameColor + AsTableManager.Instance.GetTbl_String( data.name);
	}
}
