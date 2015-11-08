using UnityEngine;
using System.Collections;

public delegate void SelectDelegate();

public class SystemListItem : MonoBehaviour
{
	private SelectDelegate selectDelegate = null;
	public SelectDelegate SelectCallback
	{
		set { selectDelegate = value; }
	}

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	private void OnClick()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		selectDelegate();
	}
}
