using UnityEngine;
using System.Collections;

public class AsMessageBoxGender : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText info = null;
	[SerializeField]UIButton closeBtn = null;
	[SerializeField]UIRadioBtn femaleBtn = null;
	[SerializeField]UIRadioBtn maleBtn = null;
	[SerializeField]UIButton confirmBtn = null;

	private eGENDER gender = eGENDER.eGENDER_MALE;

	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(4202);
		info.Text = AsTableManager.Instance.GetTbl_String(4204);
		confirmBtn.Text = AsTableManager.Instance.GetTbl_String(4201);

		gender = AsUserInfo.Instance.accountGender;
		switch( gender)
		{
		case eGENDER.eGENDER_NOTHING:
		case eGENDER.eGENDER_MALE:
			maleBtn.Value = true;
			femaleBtn.Value = false;
			gender = eGENDER.eGENDER_MALE;
			break;
		case eGENDER.eGENDER_FEMALE:
			maleBtn.Value = false;
			femaleBtn.Value = true;
			gender = eGENDER.eGENDER_FEMALE;
			break;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnFemaleBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		gender = eGENDER.eGENDER_FEMALE;
		maleBtn.Value = false;
		femaleBtn.Value = true;
	}

	void OnMaleBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		gender = eGENDER.eGENDER_MALE;
		maleBtn.Value = true;
		femaleBtn.Value = false;
	}

	void OnConfirmBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Confirm();
		Close();
	}

	public void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		if( eGENDER.eGENDER_NOTHING == AsUserInfo.Instance.accountGender)
			Confirm();
		Close();
	}

	private void Confirm()
	{
		body_CS_USERGENDER_SET genderSet = new body_CS_USERGENDER_SET( gender);
		byte[] packet = genderSet.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
	}

	public void Close()
	{
		PlayerPrefs.SetString( "LatestGenderSetTime", System.DateTime.Now.Ticks.ToString());
		GameObject.Destroy( gameObject);
	}
}
