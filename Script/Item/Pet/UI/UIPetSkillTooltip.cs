using UnityEngine;
using System.Collections;

public class UIPetSkillTooltip : MonoBehaviour
{
	[SerializeField] BoxCollider col = null;
	[SerializeField] SpriteText desc = null;
	
	private Camera cam = null;
	private GameObject goIcon = null;
	
	// Use this for initialization
	void Start()
	{
		GameObject camObj = GameObject.Find( "UICamera");
		cam = camObj.GetComponent<Camera>();
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( desc);
	}

	void OnDisable()
	{
		m_Destroyable = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		//$yde
		if(m_Destroyable == false)
			return;
		
//		#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( true == Input.GetMouseButtonUp(0))
		{
			gameObject.SetActive(false);
		}
//		#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch(0);
			if( TouchPhase.Ended == touch.phase)
			{
				gameObject.SetActive(false);
			}
		}
//		#endif
	}
	
	public void Init( Tbl_Skill_Record skillRecord, Tbl_SkillLevel_Record skillLevelRecord)
	{
		if(goIcon != null)
			Destroy(goIcon);

		if(col == null)
		{
			Debug.LogError("UIPetSkillTooltip:: Init: icon position is not set");
			return;
		}

		GameObject obj = Resources.Load( skillRecord.Skill_Icon) as GameObject;
		if( null == obj )
		{
			Debug.LogError("UIPetSkillTooltip:: Init: invalid path. skillRecord.Skill_Icon = " + skillRecord.Skill_Icon);
			return;
		}

		goIcon = GameObject.Instantiate( obj) as GameObject;
		goIcon.transform.parent = col.transform;
		goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
		goIcon.transform.localRotation = Quaternion.identity;
		goIcon.transform.localScale = Vector3.one;

		UISlotItem slotItem = goIcon.GetComponent<UISlotItem>();
		if(slotItem != null)
			slotItem.iconImg.SetSize( col.size.x, col.size.y);
		
		string szDesc = AsTableManager.Instance.GetTbl_String( skillRecord.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, skillRecord.Index, skillLevelRecord.Skill_Level, 0);
		desc.Text = szDesc;

		gameObject.SetActive(true);

		StartCoroutine(WaitEnoughClose());
	}
	
	//$yde
	float m_WaitTime = 0.2f;
	bool m_Destroyable = true;
	IEnumerator WaitEnoughClose()
	{
		m_Destroyable = false;
		
		yield return new WaitForSeconds(m_WaitTime);
		
		m_Destroyable = true;
	}
}
