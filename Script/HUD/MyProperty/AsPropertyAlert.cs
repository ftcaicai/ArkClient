using UnityEngine;
using System.Collections;

public class AsPropertyAlert : MonoBehaviour
{
	public AsTextBlinker alertMessage = null;
	
//	static AsTextBlinker s_AlertMessage; public static AsTextBlinker TextBlinker{get{return s_AlertMessage;}} //$yde
	
	// Use this for initialization
	void Start()
	{
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		AsLanguageManager.Instance.SetFontFromSystemLanguage( text, true);
		
//		s_AlertMessage = alertMessage; // $yde
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void CoolTimeAlert( COMMAND_SKILL_TYPE type)
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str289_Cannot_Use_Yet); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(289);
		alertMessage.Play();
		
		if( COMMAND_SKILL_TYPE._NONE != type)
			AsSkillDelegatorManager.Instance.AddWarning( type);
		
		AsSkillCoolTimeAlramDelegatorManager.Instance.AddSkillCoolTimeAlram( type);
	}
	
	public void TargetAlert()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str291_Target_Not_Valid); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(291);
		alertMessage.Play();
	}
	
	public void ManaAlert()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str288_Not_Enough_Mana);
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(288);
		alertMessage.Play();
	}
	
	public void AlertInvalidTarget()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str290_Target_Not_Chosen); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(290);
		alertMessage.Play();
	}
	
	public void AlertNoWeapon()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str292_Weapon_Not_Equipped); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(292);
		alertMessage.Play();
	}
	
	public void AlertState()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str293_Cannot_Use_In_Current_State); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(293);
		alertMessage.Play();
	}
	
	public void AlertSkillInTown()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str830_Cannot_Use_In_Village); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(830);
		alertMessage.Play();
	}
	
	public void AlertUseless()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str921_Not_Need_To_Use); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(921);
		alertMessage.Play();
	}
	
	//$yde
	public void AlertEmotiocon_Seat()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str1633_Cannot_Use_In_Combat); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(1633);
		alertMessage.Play();
	}
	
	//$yde
	public void AlertNotInPvp()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str904_Cannot_Use_In_PVP); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(904);
		alertMessage.Play();
	}
	
	//$yde
	public void AlertRebirthNotInPvp()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str903_Cannot_Use_In_Duel); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(903);
		alertMessage.Play();
	}
	
	public void AlertNotInRaid()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str999_Cannot_Use_In_Raid); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(999);
		alertMessage.Play();
	}


    public void AlertNotInField()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str978_Cannot_Use_In_Field); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
        text.Text = AsTableManager.Instance.GetTbl_String(978);
		alertMessage.Play();
	}
	
	public void AlertNotInIndun()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str2348_Cannot_Use_In_Indun);
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(2348);
		alertMessage.Play();
	}

	public void AlertNotInSummon()
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str13_Cannot_Use_Item); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
        text.Text = AsTableManager.Instance.GetTbl_String(13);
		alertMessage.Play();
	}
	
    public void AlertTargetChanged() //$yde
	{
		AsSoundManager.Instance.PlaySound_VoiceBattle(eVoiceBattle.str2136_Target_Changed); //$yde
		
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
        text.Text = AsTableManager.Instance.GetTbl_String(2136);
		alertMessage.Play();
	}

	public void AlertNoExpandQuickSlot()
	{
		SpriteText text = alertMessage.gameObject.GetComponent<SpriteText>();
		text.Text = AsTableManager.Instance.GetTbl_String(2721);
		alertMessage.Play();
	}

}










