using UnityEngine;
using System.Collections;

public class AsRecommendTab : AsSocialTab
{
	public UIScrollList m_list = null;
	public GameObject m_objChoiceItem = null;
	public SpriteText m_CountMessageText = null;

	private body_SC_SOCIAL_RECOMMEND m_data;

	// Use this for initialization
	void Start()
	{
	}

	void OnEnable()
	{
		m_list.ClearList( true);
	}

	override public void Init()
	{
	}

	public override UIScrollList getList()
	{
		return null;
	}

	public void SetData( body_SC_SOCIAL_RECOMMEND data)
	{
		m_list.ClearList( true);
		m_data = data;

		for( int i = 0; i < AsGameDefine.eRECOMMEND_ACCURE_MAX; ++i)
		{
			if( data.arrItemIndex[i] != 0)
			{
				UIListItemContainer item = m_list.CreateItem( m_objChoiceItem) as UIListItemContainer;

				AsRecommendItem recommendItem = item.gameObject.GetComponent<AsRecommendItem>();
				recommendItem.SetData( data, i);
				item.ScanChildren();
				m_list.ClipItems();
			}
		}

		m_CountMessageText.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1911),m_data.nRecommendCompleteCount);
	}

	// Update is called once per frame
	void Update()
	{
	}
}
