using UnityEngine;
using System.Collections;

public class AsRacePanel : AsCharacterCreatePanel
{
	[SerializeField] AsCharacterRepresentative representative = null;
	[SerializeField] SpriteText raceName = null;
	[SerializeField] SpriteText mainPosition = null;
	[SerializeField] SpriteText mainWeapon = null;
	[SerializeField] SpriteText historyDesc = null;
	[SerializeField] UIRadioBtn demigodBtn = null;
	[SerializeField] UIRadioBtn elfBtn = null;
	[SerializeField] UIRadioBtn lumicleBtn = null;
	[SerializeField] UIRadioBtn armanBtn = null;
	[SerializeField] SimpleSprite[] attackTendencies = new SimpleSprite[0];
	[SerializeField] SimpleSprite[] defenseTendencies = new SimpleSprite[0];
	[SerializeField] SimpleSprite[] supportTendencies = new SimpleSprite[0];
	[SerializeField] SimpleSprite widthBase = null;
	[SerializeField] SpriteText info = null;
	
	// Use this for initialization
	void Start()
	{
		demigodBtn.Text = AsTableManager.Instance.GetTbl_String(301);
		elfBtn.Text = AsTableManager.Instance.GetTbl_String(302);
		lumicleBtn.Text = AsTableManager.Instance.GetTbl_String(303);
		armanBtn.Text = AsTableManager.Instance.GetTbl_String(304);
		info.Text = AsTableManager.Instance.GetTbl_String(2093);

		demigodBtn.Value = true;
		historyDesc.maxWidthInPixels = true;
		historyDesc.maxWidth = widthBase.PixelSize.x;

		SetDesc();
	}

	void OnEnable()
	{
		AsCharacterCreateFramework.curRace = (int)eRACE.DEMIGOD;
		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;

		demigodBtn.Value = true;

		SetDesc();
	}

	public override void Initialize()
	{
		SetDesc();
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	private void OnDemigodBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsCharacterCreateFramework.curRace = (int)eRACE.DEMIGOD;
		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;
		SetDesc();
	}

	private void OnElfBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsCharacterCreateFramework.curRace = (int)eRACE.ELF;
		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;
		SetDesc();
	}

	private void OnLumicleBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsCharacterCreateFramework.curRace = (int)eRACE.LUMICLE;
		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;
		SetDesc();
	}

	private void OnAmanBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		AsCharacterCreateFramework.curRace = (int)eRACE.ARMAN;
		AsCharacterCreateFramework.curGender = (int)eGENDER.eGENDER_MALE;
		SetDesc();
	}
	
	
	private void SetDesc()
	{
		switch( (eRACE)AsCharacterCreateFramework.curRace)
		{
		case eRACE.DEMIGOD:
			AsCharacterCreateFramework.curJob = (int)eCLASS.DIVINEKNIGHT;
			mainPosition.Text = AsTableManager.Instance.GetTbl_String(311);
			mainWeapon.Text = AsTableManager.Instance.GetTbl_String(316);
			break;
		case eRACE.ELF:
			AsCharacterCreateFramework.curJob = (int)eCLASS.MAGICIAN;
			mainPosition.Text = AsTableManager.Instance.GetTbl_String(312);
			mainWeapon.Text = AsTableManager.Instance.GetTbl_String(317);
			break;
		case eRACE.LUMICLE:
			AsCharacterCreateFramework.curJob = (int)eCLASS.CLERIC;
			mainPosition.Text = AsTableManager.Instance.GetTbl_String(313);
			mainWeapon.Text = AsTableManager.Instance.GetTbl_String(318);
			break;
		case eRACE.ARMAN:
			AsCharacterCreateFramework.curJob = (int)eCLASS.HUNTER;
			mainPosition.Text = AsTableManager.Instance.GetTbl_String(314);
			mainWeapon.Text = AsTableManager.Instance.GetTbl_String(319);
			break;
		case eRACE.DEMIEVIL:
			AsCharacterCreateFramework.curJob = (int)eCLASS.ASSASSIN;
			mainPosition.Text = AsTableManager.Instance.GetTbl_String(0);
			mainWeapon.Text = AsTableManager.Instance.GetTbl_String(0);
			break;
		default:
			Debug.LogError( "Invlaid race");
			break;
		}
		
		representative.SetRace( (eRACE)AsCharacterCreateFramework.curRace);
		
		AsCharacterCreateData raceData = AsTableManager.Instance.GetTbl_CreateCharacter_RaceData( (eRACE)AsCharacterCreateFramework.curRace);
		raceName.Text = AsTableManager.Instance.GetTbl_String( raceData.raceName);
		historyDesc.Text = raceData.history;

		AsCharacterCreateClassData classData = raceData.GetData( (eCLASS)AsCharacterCreateFramework.curJob);
		Debug.Assert( null != classData);
		SetAttackTendency( classData.damage);
		SetDefenseTendency( classData.defense);
		SetSupportTendency( classData.support);
	}

	private void SetAttackTendency( int point)
	{
		foreach( SimpleSprite spr in attackTendencies)
			spr.gameObject.SetActiveRecursively( false);

		for( int i = 0; i < point; i++)
			attackTendencies[i].gameObject.SetActiveRecursively( true);
	}

	private void SetDefenseTendency( int point)
	{
		foreach( SimpleSprite spr in defenseTendencies)
			spr.gameObject.SetActiveRecursively( false);

		for( int i = 0; i < point; i++)
			defenseTendencies[i].gameObject.SetActiveRecursively( true);
	}

	private void SetSupportTendency( int point)
	{
		foreach( SimpleSprite spr in supportTendencies)
			spr.gameObject.SetActiveRecursively( false);

		for( int i = 0; i < point; i++)
			supportTendencies[i].gameObject.SetActiveRecursively( true);
	}
}
