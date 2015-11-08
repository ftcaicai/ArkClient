using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;


public class AsSkillTooltip : MonoBehaviour
{
	public GameObject icon = null;
	public SpriteText nameField = null;
	public SpriteText lv = null;
	public SpriteText coolTimeText = null;
	public SpriteText coolTime = null;
	public SpriteText manaCostText = null;
	public SpriteText manaCost = null;
	public SpriteText chargeStep = null;
	public SpriteText chargeStepText = null;
	public UIButton prevBtn = null;
	public UIButton nextBtn = null;
	public SpriteText desc = null;
	public SpriteText needLev = null;
	public SpriteText cost = null;
	public GameObject buyBtn = null;

	private Camera cam = null;
	private int id = -1;
	public int ID	{ set	{ id = value; } }
	private int level = -1;
	public int Level	{ set	{ level = value; } }
	private GameObject goIcon = null;
	private AsSkillTooltip sibling = null;
	public AsSkillTooltip Sibling
	{
		set	{ sibling = value; }
	}
	private Tbl_Skill_Record skillRecord = null;
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
		
		UIButton btn = buyBtn.GetComponent<UIButton>();
		btn.Text = AsTableManager.Instance.GetTbl_String(865);
	}
	
	// Update is called once per frame
	void Update()
	{
#if ( UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
//		if( ( true == Input.GetMouseButtonUp(0)) && ( false == AsGameMain.isPopupExist))
		if( true == Input.GetMouseButtonUp(0))
		{
			if( null != sibling)
			{
				if( false == AsUtil.PtInCollider( cam, buyBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))
					&& false == AsUtil.PtInCollider( cam, sibling.buyBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
				{
					if( true == prevBtn.gameObject.active)
					{
						if( false == AsUtil.PtInCollider( cam, prevBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))
						&& false == AsUtil.PtInCollider( cam, nextBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))
						&& false == AsUtil.PtInCollider( cam, sibling.prevBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))
						&& false == AsUtil.PtInCollider( cam, sibling.nextBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
						{
							GameObject.DestroyImmediate( gameObject);
							GameObject.DestroyImmediate( sibling.gameObject);
						}
					}
					else
					{
						GameObject.DestroyImmediate( gameObject);
						GameObject.DestroyImmediate( sibling.gameObject);
					}
				}
			}
			else
			{
				if( false == AsUtil.PtInCollider( cam, buyBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
				{
					if( true == prevBtn.gameObject.active)
					{
						if( false == AsUtil.PtInCollider( cam, prevBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y))
							&& false == AsUtil.PtInCollider( cam, nextBtn.collider, new Vector2( Input.mousePosition.x, Input.mousePosition.y)))
						{
							GameObject.DestroyImmediate( gameObject);
						}
					}
					else
					{
						GameObject.DestroyImmediate( gameObject);
					}
				}
			}
		}
#else
		if( 0 < Input.touchCount)
		{
			Touch touch = Input.GetTouch(0);
//			if( ( TouchPhase.Ended == touch.phase) && ( false == AsGameMain.isPopupExist))
			if( TouchPhase.Ended == touch.phase)
			{
				if( null != sibling)
				{
					if( false == AsUtil.PtInCollider( cam, buyBtn.collider, touch.position)
						&& false == AsUtil.PtInCollider( cam, sibling.buyBtn.collider, touch.position))
					{
						if( true == prevBtn.gameObject.active)
						{
							if( false == AsUtil.PtInCollider( cam, prevBtn.collider, touch.position)
							&& false == AsUtil.PtInCollider( cam, nextBtn.collider, touch.position)
							&& false == AsUtil.PtInCollider( cam, sibling.prevBtn.collider, touch.position)
							&& false == AsUtil.PtInCollider( cam, sibling.nextBtn.collider, touch.position))
							{
								GameObject.DestroyImmediate( gameObject);
								GameObject.DestroyImmediate( sibling.gameObject);
							}
						}
						else
						{
							GameObject.DestroyImmediate( gameObject);
							GameObject.DestroyImmediate( sibling.gameObject);
						}
					}
				}
				else
				{
					if( false == AsUtil.PtInCollider( cam, buyBtn.collider, touch.position))
					{
						if( true == prevBtn.gameObject.active)
						{
							if( false == AsUtil.PtInCollider( cam, prevBtn.collider, touch.position)
							&& false == AsUtil.PtInCollider( cam, nextBtn.collider, touch.position))
							{
								GameObject.DestroyImmediate( gameObject);
							}
						}
						else
						{
							GameObject.DestroyImmediate( gameObject);
						}
					}
				}
			}
		}
#endif
		
		if( null != AsHudDlgMgr.Instance && false == AsHudDlgMgr.Instance.IsOpenedSkillshop)
		{
			GameObject.DestroyImmediate( gameObject);
			if( null != sibling)
				GameObject.DestroyImmediate( sibling.gameObject);
		}
	}
	
	public void Init( Tbl_Skill_Record skillRecord, Tbl_SkillLevel_Record skillLevelRecord, int price)
	{
		StringBuilder sb = new StringBuilder();
		
		this.skillRecord = skillRecord;
//		this.skillLevelRecord = skillLevelRecord;
		
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
		sb.Append ( "Lv.");
		sb.Append( level);
		lv.Text = sb.ToString();
//		lv.Text = "Lv." + level;
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
			
			sb.Remove( 0, sb.Length);
			
			if( 0 == hour)
			{
				if( 0 == min)
				{
					sb.AppendFormat( "{0:D}{1}", sec, AsTableManager.Instance.GetTbl_String(90));
					coolTime.Text = sb.ToString();
				}
				else
				{
					sb.AppendFormat( "{0:D}{1} {2:D}{3}", min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
					coolTime.Text = sb.ToString();
				}
			}
			else
			{
				sb.AppendFormat( "{0:D}{1} {2:D}{3} {4:D}{5}", hour, AsTableManager.Instance.GetTbl_String(88),
					min, AsTableManager.Instance.GetTbl_String(89), sec, AsTableManager.Instance.GetTbl_String(90));
				coolTime.Text = sb.ToString();
			}
		}
		
		manaCost.Text = skillLevelRecord.Mp_Decrease.ToString();
		sb.Remove( 0, sb.Length);
		sb.Append( AsTableManager.Instance.GetTbl_String(134));
		sb.Append( ' ');
		sb.Append( skillLevelRecord.Level_Limit.ToString());

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		int userLevel = userEntity.GetProperty<int>( eComponentProperty.LEVEL);
		if( userLevel < skillLevelRecord.Level_Limit)
		{
			UIButton btn = buyBtn.GetComponent<UIButton>();
			btn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btn.spriteText.Color = Color.gray;
			sb.Insert( 0, Color.red.ToString());
		}
		needLev.Text = sb.ToString();

		sb.Remove( 0, sb.Length);
		sb.Append( price.ToString( "#,#0", CultureInfo.InvariantCulture));
		if( AsUserInfo.Instance.SavedCharStat.nGold < (ulong)price)
		{
			UIButton btn = buyBtn.GetComponent<UIButton>();
			btn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
			btn.spriteText.Color = Color.gray;
			sb.Insert( 0, Color.red.ToString());
			cost.Text = sb.ToString();
		}
		else
		{
			sb.Insert( 0, Color.yellow.ToString());
			cost.Text = sb.ToString();
		}
	}
	
	public void OnBuy()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( true == AsGameMain.isPopupExist)
			return;
		
		AsGameMain.isPopupExist = true;
		
		if( null == sibling)
		{
//			Confirm();
			SkillLearn();
		}
		else
		{
			if(AsHudDlgMgr.Instance.IsOpenedSkillshop == true)//$yde
			{
				AsSkillshopDlg dlg = AsHudDlgMgr.Instance.skillShopObj.GetComponentInChildren<AsSkillshopDlg>();
				Debug.Assert( null != dlg);
				dlg.SkillSelection( skillRecord, id, level);
//				AsHudDlgMgr.Instance.skillShopDlg.SkillSelection(this, id, level);
			}
			
//			AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(353), AsTableManager.Instance.GetTbl_String(72),
//				AsHudDlgMgr.Instance.skillShopDlg, "SkillLearn", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
//			msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
//			msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
		}
	}
	
	public void Confirm()
	{
		string format = AsTableManager.Instance.GetTbl_String(73);
		string msg = string.Format( format, AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index), level);
		AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(353), msg, this, "SkillLearn", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
	}
			
	public void SkillLearn()
	{
		SkillShop.Instance.PurchaseSkill( id, level);
		
		GameObject.DestroyImmediate( gameObject);
		AsGameMain.isPopupExist = false;
		
		if( null != sibling)
			GameObject.DestroyImmediate( sibling.gameObject);
	}
	
	public void OnMessageBoxNotify()
	{
		GameObject.DestroyImmediate( gameObject);
		AsGameMain.isPopupExist = false;
		
		if( null != sibling)
			GameObject.DestroyImmediate( sibling.gameObject);
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
