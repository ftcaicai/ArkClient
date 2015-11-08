using UnityEngine;
using System.Collections;

public class AsMenuBtnAction : MonoBehaviour
{
	private AsMenuBtnAnimController animController = null;
	private bool isExpanded = false;
	public bool getExpanded
	{
		get	{ return isExpanded; }
	}
	
	// Use this for initialization
	void Start()
	{
		Transform childTransform = transform.FindChild( "SubRoot");

        if (childTransform != null)
		    animController = childTransform.GetComponent<AsMenuBtnAnimController>();
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Expand()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		isExpanded = !isExpanded;

		if( true == isExpanded)
		{
			animController.Expand();
		}
		else
			animController.Collapse();
	}
	
	public void ForcedCollapse()
	{
		isExpanded = false;
		animController.ForcedCollapse();
	}
}
