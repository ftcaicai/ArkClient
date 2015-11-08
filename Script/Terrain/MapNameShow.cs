using UnityEngine;
using System.Collections;


public class MapNameShow : MonoBehaviour
{
	[SerializeField]SpriteText textName = null;
	[SerializeField]SpriteText channelName = null;
	
	private float m_fShowTime = 0f;
	private float m_fMaxShowTime = 0f;
	
	public void Open( string strName, float fMaxTime)
	{
		textName.Text = strName;
		channelName.Text = AsUserInfo.Instance.currentChannelName;
		m_fMaxShowTime = fMaxTime;

		QuestTutorialMgr.Instance.StartAcceptedQuestTutorial();
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		m_fShowTime += Time.deltaTime;
		
		if( m_fMaxShowTime <= m_fShowTime)
		{
			if( QuestTutorialMgr.Instance.attendBonus == false)
				QuestTutorialMgr.Instance.StartQuestTutorial();

			GameObject.DestroyObject( gameObject);
		}
	}
}
