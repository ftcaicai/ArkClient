using UnityEngine;
using System.Collections;
using System.Text;

public class AsSoundManager : MonoBehaviour
{
	GameObject m_kSoundObject;
	GameObject m_kBGM;
	GameObject soundRoot;

	private static AsSoundManager m_kInstance = null;
	public static AsSoundManager Instance
	{
		get	{ return m_kInstance; }
	}
	
	private string m_strBGMBuf = string.Empty;

	void Awake()
	{
		m_kInstance = this;
		soundRoot = new GameObject( "SoundRoot");
		DontDestroyOnLoad( soundRoot);
		DDOL_Tracer.RegisterDDOL(this, soundRoot);//$ yde
		
		m_kSoundObject = Resources.Load( "UseScript/SoundObject") as GameObject;
		m_kBGM = GameObject.Instantiate( m_kSoundObject, Vector3.zero, Quaternion.identity) as GameObject;
		m_kBGM.name = "BGM_";
		m_kBGM.audio.loop = true;
		m_kBGM.audio.panLevel = 0;
		m_kBGM.audio.volume = 0.3f;
		m_kBGM.transform.parent = soundRoot.transform;
	}

	void Start()
	{
	}

	void OnApplicationQuit()
	{
		Destroy( m_kBGM);
	}

	public void PlayBGM( string _filename)
	{
		m_strBGMBuf = _filename;

		if( false == _isEnableBGM())
			return;
		
		StringBuilder sb = new StringBuilder( "BGM_");
		sb.Append( _filename);
		
		m_kBGM.name = sb.ToString();
//		m_kBGM.name = "BGM_" + _filename;
//		m_kBGM.audio.clip = Resources.Load( "Sound/World/BGM/" + _filename) as AudioClip;
		//m_kBGM.audio.clip = Resources.Load( _filename) as AudioClip;
		m_kBGM.audio.clip = ResourceLoad.LoadAudioClip( _filename);
		m_kBGM.audio.Play();
	}
	
	public void PlayBGM_Patcher()
	{
		m_kBGM.name = "S0013_BGM_Patch";
		m_kBGM.audio.clip = Resources.Load( "UseScript/S0013_BGM_Patch") as AudioClip;
		m_kBGM.audio.Play();
	}
	
	public void PlayBGMBuf()
	{
		PlayBGM( m_strBGMBuf);
	}

	public void StopBGM()
	{
		if( m_kBGM)
			m_kBGM.audio.Stop();
	}
	
	public bool IsPlayingBGM()
	{
		if( null == m_kBGM)
			return false;
		
		return m_kBGM.audio.isPlaying;
	}
	
	// delay : 44100
	public AudioSource PlaySound( string _filename, Vector3 _pos, bool loop, bool bLoadDirect = false)
	{
		if( false == _isEnableEffSound())
			return null;
		
		if( true == _filename.ToLower().Contains( "none"))
			return null;
		
		AudioClip clip = ResourceLoad.LoadAudioClip( _filename, bLoadDirect);
		
		if( null == clip)
			return null;
		
		if( _pos == Vector3.zero)
		{
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null == userEntity)
				_pos = Camera.main.transform.position;
			else
				_pos = userEntity.gameObject.transform.position;
		}
		
		StringBuilder sb = new StringBuilder( "SND_");
		sb.Append( _filename);
		
		GameObject go = GameObject.Instantiate( m_kSoundObject, _pos, Quaternion.identity) as GameObject;
		go.name = sb.ToString();
//		go.name = "SND_" + _filename;
		go.audio.clip = clip;
		go.audio.loop = loop;
		go.audio.Play();
		go.transform.parent = soundRoot.transform;
		
		return go.audio;
	}
	
	public void  StopSound( AudioSource source)
	{
		if( null == source)
			return;
		
		source.Stop();
		source = null;
	}
	
	public void UpdatePosition( AudioSource source, Vector3 pos)
	{
		if( null == source)
			return;
		
		source.transform.position = pos;
	}
	
	// < private
	private bool _isEnableBGM()
	{
		return AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SoundBG);
	}
	
	private bool _isEnableEffSound()
	{
		return AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_SoundEff);
	}
	// private >
	
	public void PlaySound_VoiceBattle( eVoiceBattle _voice)
	{
//		Debug.Log("AsSoundManager::PlaySound_VoiceBattle: _voice = " + _voice);
		
		if( AsGameMain.GetOptionState(OptionBtnType.OptionBtnType_VoiceBattle) == false)
			return;
		
		if( AsTextBlinker.Blinking == true)
			return;
		
		string fileName = "";
		
		switch( _voice)
		{
		case eVoiceBattle.str288_Not_Enough_Mana:
			fileName = "Sound/Interface/S6127_Voice_Battle_01";
			break;
		case eVoiceBattle.str289_Cannot_Use_Yet:
			fileName = "Sound/Interface/S6128_Voice_Battle_02";
			break;
		case eVoiceBattle.str290_Target_Not_Chosen:
			fileName = "Sound/Interface/S6129_Voice_Battle_03";
			break;
		case eVoiceBattle.str291_Target_Not_Valid:
			fileName = "Sound/Interface/S6130_Voice_Battle_04";
			break;
		case eVoiceBattle.str292_Weapon_Not_Equipped:
			fileName = "Sound/Interface/S6131_Voice_Battle_05";
			break;
		case eVoiceBattle.str293_Cannot_Use_In_Current_State:
			fileName = "Sound/Interface/S6132_Voice_Battle_06";
			break;
		case eVoiceBattle.str830_Cannot_Use_In_Village:
			fileName = "Sound/Interface/S6133_Voice_Battle_07";
			break;
		case eVoiceBattle.str921_Not_Need_To_Use:
			fileName = "Sound/Interface/S6134_Voice_Battle_08";
			break;
		case eVoiceBattle.str1633_Cannot_Use_In_Combat:
			fileName = "Sound/Interface/S6135_Voice_Battle_09";
			break;
		case eVoiceBattle.str904_Cannot_Use_In_PVP:
			fileName = "Sound/Interface/S6136_Voice_Battle_10";
			break;
		case eVoiceBattle.str903_Cannot_Use_In_Duel:
			fileName = "Sound/Interface/S6137_Voice_Battle_11";
			break;
		case eVoiceBattle.str999_Cannot_Use_In_Raid:
			fileName = "Sound/Interface/S6138_Voice_Battle_12";
			break;
		case eVoiceBattle.str978_Cannot_Use_In_Field:
			fileName = "Sound/Interface/S6139_Voice_Battle_13";
			break;
		case eVoiceBattle.str13_Cannot_Use_Item:
			fileName = "Sound/Interface/S6140_Voice_Battle_14";
			break;
		case eVoiceBattle.str2136_Target_Changed:
			fileName = "Sound/Interface/S6141_Voice_Battle_15";
			break;
		case eVoiceBattle.str2348_Cannot_Use_In_Indun:
			fileName = "Sound/Interface/S6145_Voice_Battle_16";
			break;
		}
		
//		Debug.Log("AsSoundManager::PlaySound_VoiceBattle: fileName = " + fileName + " is played");
		
		PlaySound( fileName, Vector3.zero, false);
	}
}

public enum eVoiceBattle {
	str288_Not_Enough_Mana,
	str289_Cannot_Use_Yet,
	str290_Target_Not_Chosen,
	str291_Target_Not_Valid,
	str292_Weapon_Not_Equipped,
	str293_Cannot_Use_In_Current_State,
	str830_Cannot_Use_In_Village,
	str921_Not_Need_To_Use,
	str1633_Cannot_Use_In_Combat,
	str904_Cannot_Use_In_PVP,
	str903_Cannot_Use_In_Duel,
	str999_Cannot_Use_In_Raid,
	str978_Cannot_Use_In_Field,
	str13_Cannot_Use_Item,
	str2136_Target_Changed,
	str2348_Cannot_Use_In_Indun,
}