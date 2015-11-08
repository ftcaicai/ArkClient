using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsGuideListDlg : MonoBehaviour
{
	[SerializeField] SpriteText title = null;
	[SerializeField] UIButton closeBtn = null;
	[SerializeField] UIScrollList list = null;
	[SerializeField] GameObject listItem = null;

	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1992);

		Dictionary<eGameGuideType,List<GameGuideData>> dicGuide = AsGameGuideManager.Instance.DataContainer;
		foreach( KeyValuePair<eGameGuideType,List<GameGuideData>> pair in dicGuide)
		{
			foreach( GameGuideData guideData in pair.Value)
			{
				if( 0 == guideData.nameIdx)
					continue;

				UIListItemContainer con = list.CreateItem( listItem) as UIListItemContainer;
				con.Text = AsTableManager.Instance.GetTbl_String( guideData.nameIdx);
				GuideListItem item = con.gameObject.GetComponent<GuideListItem>();
				Debug.Assert( null != item);
				item.GuideData = guideData;
			}
		}
		list.UpdateCamera();
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		GameObject.Destroy( gameObject.transform.parent.gameObject);
	}
	
	public void Close()
	{
		GameObject.Destroy( gameObject.transform.parent.gameObject);
	}
}
