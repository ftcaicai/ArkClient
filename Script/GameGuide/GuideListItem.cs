using UnityEngine;
using System.Collections;

public class GuideListItem : MonoBehaviour
{
	private GameGuideData guideData = null;
	public GameGuideData GuideData
	{
		set { guideData = value; }
	}

	void OnEnable()
	{
		UIListItemContainer con = gameObject.GetComponent<UIListItemContainer>();
		Debug.Assert( null != con);
		con.UpdateCamera();
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
		
		if( null == guideData)
			return;

		AsGameGuideManager.Instance.DisplayGuide( guideData);
	}
}
