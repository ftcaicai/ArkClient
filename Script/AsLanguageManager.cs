using UnityEngine;
using System.Collections;

public class AsLanguageManager : MonoBehaviour
{
	public enum LanguageType
	{
		LanguageType_Korea,
		LanguageType_Japan,
//		LanguageType_China
	};

	private LanguageType m_eLanguageType;
	public LanguageType NowLanguageType	{ get { return m_eLanguageType; } }
	
	private static AsLanguageManager m_kInstance = null;
	public static AsLanguageManager Instance	{ get { return m_kInstance; } }
	
	/*
	// korean
	public TextAsset Korean_text;
	public Material Korean_material;
	public TextAsset Korean_text_outline;
	public Material Korean_material_outline;
	
	// japanese
	public TextAsset Japanese_text;
	public Material Japanese_material;

	// chinese
	[SerializeField] TextAsset chineseAsset;
	[SerializeField] Material chineseMtrl;
	*/
	public TextAsset fontText;
	public Material fontMaterial;
	public TextAsset fontText_Outline;
	public Material fontMaterial_Outline;
	
	void Awake()
	{
		m_kInstance = this;

		GameObject main = GameObject.Find( "GameMain");
		AsGameMain asMain = main.GetComponent<AsGameMain>();
		GAME_LANGUAGE gameLanguage = asMain.GetGameLanguage();

		switch( gameLanguage)
		{
//		case GAME_LANGUAGE.LANGUAGE_AUTO:
//			_InitLanguageType();
//			break;
		case GAME_LANGUAGE.LANGUAGE_KOR:
			m_eLanguageType = LanguageType.LanguageType_Korea;
			break;
		case GAME_LANGUAGE.LANGUAGE_JAP:
			m_eLanguageType = LanguageType.LanguageType_Japan;
			break;
//		case GAME_LANGUAGE.LANGUAGE_CHN:
//			m_eLanguageType = LanguageType.LanguageType_China;
//			break;
		}
	}
	
	void Start()
	{
	}
	
	void Update()
	{
	}
	
	public void SetFontFromSystemLanguage( SpriteText spritetext, bool bOutLine = false)
	{
		if( null != spritetext)
		{
			if( true == bOutLine)
				spritetext.SetFont( fontText_Outline, fontMaterial_Outline);
			else
				spritetext.SetFont( fontText, fontMaterial);
		}
	}
	
	public void SetFontFromSystemLanguage( UITextField textfield, bool bOutLine = false)
	{
		if( null != textfield)
			SetFontFromSystemLanguage( textfield.spriteText, bOutLine);
	}
	
	/*
	void _InitLanguageType()
	{
		switch( Application.systemLanguage)
		{
		case SystemLanguage.Korean:
			m_eLanguageType = LanguageType.LanguageType_Korea;
			break;
		case SystemLanguage.English:
			m_eLanguageType = LanguageType.LanguageType_Korea;
			break;
		case SystemLanguage.Japanese:
			m_eLanguageType = LanguageType.LanguageType_Japan;
			break;
		case SystemLanguage.Chinese:
			m_eLanguageType = LanguageType.LanguageType_China;
			break;
		}
		
		Debug.Log( "System Language: " + Application.systemLanguage);
		Debug.Log( "Current LanguageType: " + m_eLanguageType);
	}
	
	public TextAsset _GetTextAsset( bool bOutLine = false)
	{
		switch( m_eLanguageType)
		{
		case LanguageType.LanguageType_Korea:
			return ( true == bOutLine) ? Korean_text_outline : Korean_text;
		case LanguageType.LanguageType_Japan:
			return Japanese_text;
		case LanguageType.LanguageType_China:
			return chineseAsset;
		default:
			return null;
		}
	}
	
	public Material _GetMaterial( bool bOutLine = false)
	{
		switch( m_eLanguageType)
		{
		case LanguageType.LanguageType_Korea:
			return ( true == bOutLine) ? Korean_material_outline : Korean_material;
		case LanguageType.LanguageType_Japan:
			return Japanese_material;
		case LanguageType.LanguageType_China:
			return chineseMtrl;
		default:
			return null;
		}
	}
	*/
}
