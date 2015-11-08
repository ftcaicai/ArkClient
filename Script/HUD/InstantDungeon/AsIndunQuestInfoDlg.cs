using UnityEngine;
using System.Collections;

public class AsIndunQuestInfoDlg : MonoBehaviour
{
	private GameObject m_goRoot = null;
	
	public AsDlgBase bgGrid;
	public SpriteText textInfoTitle = null;
	public SpriteText textInfo_1 = null;
	public SpriteText textInfo_1_Count = null;
	public SpriteText textInfo_2 = null;
	public SpriteText textInfo_2_Count = null;
	public SpriteText textInfo_3 = null;
	public SpriteText textInfo_3_Count = null;
	public Color textIndunQuestColor = Color.white;
	public Color textIndunQuestFinishColor = new Color( 0.6f, 1.0f, 0.2f, 1.0f);

	void Start()
	{
	}
	
	void Update()
	{
	}

	public void Open(GameObject goRoot, body_SC_INDUN_QUEST_PROGRESS_INFO data)
	{
		m_goRoot = goRoot;
		gameObject.SetActiveRecursively( true);
	
		_ReSizeBg( data);
		
		textInfoTitle.Text = AsTableManager.Instance.GetTbl_String( 1891);
		textInfo_1.Text = "";
		textInfo_1_Count.Text = "";
		textInfo_2.Text = "";
		textInfo_2_Count.Text = "";
		textInfo_3.Text = "";
		textInfo_3_Count.Text = "";

		Tbl_InsQuestGroup_Record record = AsTableManager.Instance.GetInsQuestGroupRecord( data.nInsQuestGroupTableUniqCode);
		
		if( null == record)
		{
			Debug.LogError( "AsIndunQuestInfoDlg::Open(), null == record, data.nInsQuestGroupTableUniqCode: " + data.nInsQuestGroupTableUniqCode);
			return;
		}

		if( record.Monster1_Kill_Count > 0)
			textInfo_1.Text = _GetMonsterKindName( record.Monster_Kind_ID1);
		
		if( record.Monster2_Kill_Count > 0)
			textInfo_2.Text = _GetMonsterKindName( record.Monster_Kind_ID2);
		
		if( record.Monster3_Kill_Count > 0)
			textInfo_3.Text = _GetMonsterKindName( record.Monster_Kind_ID3);
	}

	public void Close()
	{
		gameObject.SetActiveRecursively( false);

		if( null != m_goRoot)
			Destroy( m_goRoot);
	}
	
	public void SetIndunQuestInfo(body_SC_INDUN_QUEST_PROGRESS_INFO data)
	{
		Tbl_InsQuestGroup_Record record = AsTableManager.Instance.GetInsQuestGroupRecord( data.nInsQuestGroupTableUniqCode);
		
		if( null == record)
		{
			Debug.LogError( "AsIndunQuestInfoDlg::SetIndunQuestInfo(), null == record, data.nInsQuestGroupTableUniqCode: " + data.nInsQuestGroupTableUniqCode);
			return;
		}

		Color textColor = Color.white;

		if( record.Monster1_Kill_Count > 0)
		{
			if( data.nQuestProgress_1 == record.Monster1_Kill_Count)
				textColor = textIndunQuestFinishColor;
			else
				textColor = textIndunQuestColor;

			textInfo_1.Text = textColor + _GetMonsterKindName( record.Monster_Kind_ID1);
			textInfo_1_Count.Text = textColor + "(" + data.nQuestProgress_1.ToString() + "/" + record.Monster1_Kill_Count.ToString() + ")";
		}
		
		if( record.Monster2_Kill_Count > 0)
		{
			if( data.nQuestProgress_2 == record.Monster2_Kill_Count)
				textColor = textIndunQuestFinishColor;
			else
				textColor = textIndunQuestColor;

			textInfo_2.Text = textColor + _GetMonsterKindName( record.Monster_Kind_ID2);
			textInfo_2_Count.Text = textColor + "(" + data.nQuestProgress_2.ToString() + "/" + record.Monster2_Kill_Count.ToString() + ")";
		}
		
		if( record.Monster3_Kill_Count > 0)
		{
			if( data.nQuestProgress_3 == record.Monster3_Kill_Count)
				textColor = textIndunQuestFinishColor;
			else
				textColor = textIndunQuestColor;

			textInfo_3.Text = textColor + _GetMonsterKindName( record.Monster_Kind_ID3);
			textInfo_3_Count.Text = textColor + "(" + data.nQuestProgress_3.ToString() + "/" + record.Monster3_Kill_Count.ToString() + ")";
		}
	}
	
	private string _GetMonsterKindName(int nKindID)
	{
		int nNpcID = AsTableManager.Instance.GetNpcIdByMonsterKindId( nKindID);
		
		if( 0 == nNpcID)
		{
			Debug.LogError( "AsIndunQuestInfoDlg::_GetMonsterKindName(), 0 == nNpcID, nKindID: " + nKindID);
			return "";
		}
		
		Tbl_Npc_Record record = AsTableManager.Instance.GetTbl_Npc_Record( nNpcID);
		
		if( null == record)
		{
			Debug.LogError( "AsIndunQuestInfoDlg::_GetMonsterKindName(), null == record, nID: " + nNpcID);
			return "";
		}

		return record.NpcName;
	}

	private void _ReSizeBg(body_SC_INDUN_QUEST_PROGRESS_INFO data)
	{
		/*
		    y position   bg height
		1:     10.5        1.5
		2:     10.1        2.3
		3:      9.7        3
		*/
		
		Tbl_InsQuestGroup_Record record = AsTableManager.Instance.GetInsQuestGroupRecord( data.nInsQuestGroupTableUniqCode);
		
		if( null == record)
		{
			Debug.LogError( "AsIndunQuestInfoDlg::_ReSizeBg(), null == record, data.nInsQuestGroupTableUniqCode: " + data.nInsQuestGroupTableUniqCode);
			return;
		}
		
		int nQuestCount = 0;
		
		if( record.Monster1_Kill_Count > 0)
			nQuestCount++;
		
		if( record.Monster2_Kill_Count > 0)
			nQuestCount++;
		
		if( record.Monster3_Kill_Count > 0)
			nQuestCount++;
		
		Vector3 vPos = bgGrid.gameObject.transform.localPosition;
		float fHeight = 3.0f;
		
		switch( nQuestCount)
		{
		case 1: vPos.y = 10.5f; fHeight = 1.5f; break;
		case 2: vPos.y = 10.1f; fHeight = 2.3f; break;
		case 3: vPos.y = 9.7f; fHeight = 3.0f; break;
		}

		bgGrid.height = fHeight;
		bgGrid.Assign();
		
		bgGrid.gameObject.transform.localPosition = vPos;
	}
}
