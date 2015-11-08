using UnityEngine;
using System.Collections;

public class AsSkillTooltipInBook : MonoBehaviour
{
	public GameObject icon = null;
	public SpriteText nameField = null;
	public SpriteText lv = null;
	public SpriteText coolTimeText = null;
	public SpriteText coolTime = null;
	public SpriteText manaCostText = null;
	public SpriteText manaCost = null;
	public SpriteText desc = null;
	public SpriteText chargeStep = null;
	public SpriteText chargeStepText = null;
	public UIButton prevBtn = null;
	public UIButton nextBtn = null;
	
	private Camera cam = null;
	private int id = -1;
	public int ID	{ set	{ id = value; } }
	private int level = -1;
	public int Level	{ set	{ level = value; } }
	private GameObject goIcon = null;
	private int curChargeStep = 1;
	private int maxChargeStep = 0;
	
	// Use this for initialization
	void Start()
	{
		GameObject camObj = GameObject.Find( "UICamera");
		cam = camObj.GetComponent<Camera>();

		AsLanguageManager.Instance.SetFontFromSystemLanguage( coolTimeText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( manaCostText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( chargeStepText);
		
		coolTimeText.Text = AsTableManager.Instance.GetTbl_String( 360);
		manaCostText.Text = AsTableManager.Instance.GetTbl_String( 361);
		chargeStepText.Text = AsTableManager.Instance.GetTbl_String( 362);
	}
	
	// Update is called once per frame
	void Update()
	{
		//$yde
		if(m_Destroyable == false)
			return;

#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
		if( true == Input.GetMouseButtonUp(0))
		{
			if( true == prevBtn.gameObject.active)
			{
				if( ( false == AsUtil.PtInCollider( cam, prevBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
					&& ( false == AsUtil.PtInCollider( cam, nextBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))))
					GameObject.DestroyImmediate( gameObject);
			}
			else
			{
				GameObject.DestroyImmediate( gameObject);
			}
		}
#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch(0);
			if( TouchPhase.Ended == touch.phase)
			{
				if( true == prevBtn.gameObject.active)
				{
					if( ( false == AsUtil.PtInCollider( cam, prevBtn.collider, touch.position))
						&& ( false == AsUtil.PtInCollider( cam, nextBtn.collider, touch.position)))
						GameObject.DestroyImmediate( gameObject);
				}
				else
				{
					GameObject.DestroyImmediate( gameObject);
				}
			}
		}
#endif
	}
	
	public void Init( Tbl_Skill_Record skillRecord, Tbl_SkillLevel_Record skillLevelRecord)
	{
		GameObject obj = Resources.Load( skillRecord.Skill_Icon) as GameObject;
		if( null == obj )
		{
			Debug.LogError("AsSkillTooltip::Init()");
			return;
		}
		
		goIcon = GameObject.Instantiate( obj) as GameObject;
		goIcon.transform.parent = icon.transform;
		goIcon.transform.localPosition = new Vector3( 0.0f, 0.0f, -0.5f);
		goIcon.transform.localRotation = Quaternion.identity;
		goIcon.transform.localScale = Vector3.one;
		
		if( eSKILL_TYPE.Passive == skillRecord.Skill_Type)
		{
			coolTimeText.gameObject.SetActiveRecursively( false);
			coolTime.gameObject.SetActiveRecursively( false);
			manaCostText.gameObject.SetActiveRecursively( false);
			manaCost.gameObject.SetActiveRecursively( false);
		}
		
		maxChargeStep = skillLevelRecord.ChargeMaxStep;
		if( int.MaxValue != maxChargeStep)
		{
			chargeStep.Text = curChargeStep.ToString();
			chargeStep.gameObject.SetActiveRecursively( true);
			chargeStepText.gameObject.SetActiveRecursively( true);
			prevBtn.gameObject.SetActiveRecursively( true);
			nextBtn.gameObject.SetActiveRecursively( true);
			skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, curChargeStep);
		}
		else
		{
			chargeStep.gameObject.SetActiveRecursively( false);
			chargeStepText.gameObject.SetActiveRecursively( false);
			prevBtn.gameObject.SetActiveRecursively( false);
			nextBtn.gameObject.SetActiveRecursively( false);
		}
		
		string szDesc = AsTableManager.Instance.GetTbl_String( skillRecord.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, id, level, curChargeStep);
		
		nameField.Text = AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index);
		lv.Text = "Lv." + level;
		desc.Text = szDesc;

		if( int.MaxValue == skillLevelRecord.CoolTime)
		{
			coolTime.Text = "00h 00m 00s";
		}
		else
		{
			int nCoolTime = skillLevelRecord.CoolTime / 1000;
			int hour = nCoolTime / 3600;
			int min = ( nCoolTime % 3600) / 60;
			int sec = ( nCoolTime % 3600) % 60;
			
			
			if( 0 == hour && 0 == min )
			{
				//sec
				coolTime.Text = string.Format( "{0:D}{1}", sec, AsTableManager.Instance.GetTbl_String(90)); // sec
			}
			else if( 0 == hour )					
			{
				//min
				if( 0 == sec )
				{
					coolTime.Text = string.Format( "{0:D}{1}", min, AsTableManager.Instance.GetTbl_String(89)); 	
				}
				else
				{
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3}", min, AsTableManager.Instance.GetTbl_String(89),
						sec, AsTableManager.Instance.GetTbl_String(90));
				}
			}
			else
			{
				// hour
				if( 0 == min && 0 == sec )
				{
					coolTime.Text = string.Format( "{0:D}{1}", hour, AsTableManager.Instance.GetTbl_String(931)); 
				}
				else if( 0 == sec )
				{
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3}", hour, AsTableManager.Instance.GetTbl_String(931),
						min, AsTableManager.Instance.GetTbl_String(89));
				}
				else if( 0 == min )
				{
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3}", hour, AsTableManager.Instance.GetTbl_String(931),
						sec, AsTableManager.Instance.GetTbl_String(90));
				}
				else
				{
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3} {4:D}{5}", hour, AsTableManager.Instance.GetTbl_String(931),
						min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
				}
			}		
		}
		
		manaCost.Text = skillLevelRecord.Mp_Decrease.ToString();

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
	
	public void DisplayPrevChargeStepInfo()
	{
		curChargeStep--;
		
		if( 0 >= curChargeStep)
			curChargeStep = maxChargeStep;
		
		int totMpDec = 0;
		for( int i = 1; i <= curChargeStep; i++)
		{
			Tbl_SkillLevel_Record rec = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, i);
			totMpDec += rec.Mp_Decrease;
		}
		
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, curChargeStep);
		if( null == skillLevelRecord)
		{
			Debug.LogError( "null == skillLevelRecord");
			return;
		}
		
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( skillLevelRecord.Skill_GroupIndex);
		if( null == skillRecord)
		{
			Debug.LogError( "null == skillRecord");
			return;
		}
		
		if( int.MaxValue == skillLevelRecord.CoolTime)
		{
			coolTime.Text = "00h 00m 00s";
		}
		else
		{
			int nCoolTime = skillLevelRecord.CoolTime / 1000;
			int hour = nCoolTime / 3600;
			int min = ( nCoolTime % 3600) / 60;
			int sec = ( nCoolTime % 3600) % 60;
			
			if( 0 == hour)
			{
				if( 0 == min)
					coolTime.Text = string.Format( "{0:D}{1}", sec, AsTableManager.Instance.GetTbl_String(90));
				else
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3}", min, AsTableManager.Instance.GetTbl_String(89),
						sec, AsTableManager.Instance.GetTbl_String(90));
			}
			else
			{
				coolTime.Text = string.Format( "{0:D}{1} {2:D}{3} {4:D}{5}", hour, AsTableManager.Instance.GetTbl_String(88),
					min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
			}
		}
		manaCost.Text = totMpDec.ToString();
		
		string szDesc = AsTableManager.Instance.GetTbl_String( skillRecord.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, id, level, curChargeStep);
		desc.Text = szDesc;
		chargeStep.Text = curChargeStep.ToString();
	}
	
	public void DisplayNextChargeStepInfo()
	{
		curChargeStep++;

		if( maxChargeStep < curChargeStep)
			curChargeStep = 1;
		
		int totMpDec = 0;
		for( int i = 1; i <= curChargeStep; i++)
		{
			Tbl_SkillLevel_Record rec = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, i);
			totMpDec += rec.Mp_Decrease;
		}
		
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( level, id, curChargeStep);
		if( null == skillLevelRecord)
		{
			Debug.LogError( "null == skillLevelRecord");
			return;
		}
		
		Tbl_Skill_Record skillRecord = AsTableManager.Instance.GetTbl_Skill_Record( skillLevelRecord.Skill_GroupIndex);
		if( null == skillRecord)
		{
			Debug.LogError( "null == skillRecord");
			return;
		}
		
		if( int.MaxValue == skillLevelRecord.CoolTime)
		{
			coolTime.Text = "00h 00m 00s";
		}
		else
		{
			int nCoolTime = skillLevelRecord.CoolTime / 1000;
			int hour = nCoolTime / 3600;
			int min = ( nCoolTime % 3600) / 60;
			int sec = ( nCoolTime % 3600) % 60;
			
			if( 0 == hour)
			{
				if( 0 == min)
					coolTime.Text = string.Format( "{0:D}{1}", sec, AsTableManager.Instance.GetTbl_String(90));
				else
					coolTime.Text = string.Format( "{0:D}{1} {2:D}{3}", min, AsTableManager.Instance.GetTbl_String(89),
						sec, AsTableManager.Instance.GetTbl_String(90));
			}
			else
			{
				coolTime.Text = string.Format( "{0:D}{1} {2:D}{3} {4:D}{5}", hour, AsTableManager.Instance.GetTbl_String(88),
					min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
			}
		}
		manaCost.Text = totMpDec.ToString();
		
		string szDesc = AsTableManager.Instance.GetTbl_String( skillRecord.Description_Index);
		szDesc = AsUtil.ModifyDescriptionInTooltip( szDesc, id, level, curChargeStep);
		desc.Text = szDesc;
		chargeStep.Text = curChargeStep.ToString();
	}
}
