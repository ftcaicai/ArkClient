using UnityEngine;
using System;
using System.Collections;

public class AsSkillChargeGuage : MonoBehaviour
{
	private AsChargeGuage screen = null;
	private float chargeTime = 0.0f;
	private float remain = 0.0f;
	private MonoBehaviour parentMono = null;
	private bool canceled = false;
	private int step = 1;
	private int curStep = 0;
	private SpriteText text = null;
	private int iSkill;
	private int iSkillLevel;
	private bool isPaused = false;

	public MonoBehaviour ParentSlot
	{
		set	{ parentMono = value; }
	}

	public float ChargeTime
	{
		get	{ return chargeTime; }
		set
		{
			chargeTime = value;
			remain = chargeTime;
			Enable = true;
			screen.Value = ( chargeTime - remain) / chargeTime;
		}
	}
	
	//$yde
	public int Step
	{
		get	{ return curStep; }
	}

	public float Remain
	{
		get	{ return remain; }
	}

	public bool Enable
	{
		get	{ return gameObject.active; }
		set	{ gameObject.SetActiveRecursively( value); }
	}

	public bool Canceled
	{
		get	{ return canceled; }
		set
		{
			canceled = value;
			Enable = !value;
			remain = chargeTime;
			screen.Value = 0.0f;
			step = 1;
			curStep = 0;
		}
	}

	void Awake()
	{
		screen = gameObject.GetComponentInChildren<AsChargeGuage>();
		text = gameObject.GetComponentInChildren<SpriteText>();
	}

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( false == gameObject.active)
			return;
		
		if( false == isPaused)
		{
			remain -= Time.deltaTime;
		
			screen.Value = ( chargeTime - remain) / chargeTime;
			
			float stepElement = 1.0f / this.step;
			if( screen.Value >= stepElement * ( curStep + 1))
			{
				if( true == IsSufficientMP())
				{
					curStep++;
					
					if( null != parentMono)
						parentMono.SendMessage( "StepChanged", curStep);
				}
				else
				{
					isPaused = true;
					AsMyProperty.Instance.AlertMP();
				}
			}
			
			if( null != text)
				text.Text = String.Format( "{0:D}", curStep);
		}
	}
	
	private bool IsHaveSlotData
	{
		get	{ return 0 != iSkill && 0 != iSkillLevel; }
	}
	
	private bool IsSufficientMP()
	{
		if( false == IsHaveSlotData)
			return true;
		
		Tbl_SkillLevel_Record skillLevelRecord = AsTableManager.Instance.GetTbl_SkillLevel_Record( iSkillLevel, iSkill, curStep + 1);
		if( null == skillLevelRecord)
			return false;

		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		float curMP = userEntity.GetProperty<float>( eComponentProperty.MP_CUR);
		if( curMP < skillLevelRecord.Mp_Decrease)
			return false;
		
		return true;
	}
	
	public void Begin( float time, int step, int _iSkill, int _iSkillLevel )
	{
		iSkill = _iSkill;
		iSkillLevel = _iSkillLevel;
		ChargeTime = time;
		this.step = step;
		curStep = 0;
		isPaused = false;
	}

	public void Cancel()
	{
		Enable = false;
		remain = chargeTime;
		screen.Value = 0.0f;
		curStep = 0;
	}
}
