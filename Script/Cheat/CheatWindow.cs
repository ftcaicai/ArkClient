using UnityEngine;
using System.Collections;

public class CheatWindow : MonoBehaviour
{
	[SerializeField] SpriteText title = null;
	[SerializeField] UIScrollList list = null;
	[SerializeField] UITextField edit = null;
	[SerializeField] SpriteText info = null;
	[SerializeField] AsDlgBase background = null;
	[SerializeField] GameObject listItem = null;
	string defaultText = string.Empty;
	static private string[] m_strCheatCommands;
	static private string[] m_strCheatCommandsRes;
	static private string[] m_strCheatCommandsErr;
	
	// Use this for initialization
	void Start()
	{
		defaultText = AsTableManager.Instance.GetTbl_String(1475);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( edit);
		
		title.Text = AsTableManager.Instance.GetTbl_String(0);
		edit.Text = defaultText;
		edit.SetFocusDelegate( FocusDelegate);
		
		AsCheatManager.Instance.SetCheatResultTextErrDelegate = SetCheatResultTextErr;
		AsCheatManager.Instance.SetCheatResultTextDelegate = SetCheatResultText;
		AsCheatManager.Instance.LoadCheatTable();
		
		// set cheat commands
		int nIndex = 0;
		if( null == m_strCheatCommands)
		{
			m_strCheatCommands = new string[(int)AsCheatManager.eCheatType.CT_Max];
			for( int i = 0; i < AsCheatManager.Instance.GetCheatTableDataCount(); i++)
			{
				if( nIndex >= (int)AsCheatManager.eCheatType.CT_Max)
					break;
				if( true == AsCheatManager.Instance.GetCheatString( i + 1, out m_strCheatCommands[nIndex]))
					nIndex++;
			}
		}
		
		// set cheat command result description
		if( null == m_strCheatCommandsRes)
		{
			nIndex = 0;
			m_strCheatCommandsRes = new string[(int)AsCheatManager.eCheatType.CT_Max];
			for( int i = 0; i < AsCheatManager.Instance.GetCheatTableDataCount(); i++)
			{
				if( nIndex >= (int)AsCheatManager.eCheatType.CT_Max)
					break;
				if( true == AsCheatManager.Instance.GetCheatString( i + 100, out m_strCheatCommandsRes[nIndex]))
					nIndex++;
			}
		}
		
		// set cheat error description
		if( null == m_strCheatCommandsErr)
		{
			nIndex = 0;
			m_strCheatCommandsErr = new string[(int)AsCheatManager.eCheatErrType.CE_Max];
			for( int i = 0; i < AsCheatManager.Instance.GetCheatTableDataCount(); i++)
			{
				if( nIndex >= (int)AsCheatManager.eCheatErrType.CE_Max)
					break;
				if( true == AsCheatManager.Instance.GetCheatString( i + 1000, out m_strCheatCommandsErr[nIndex]))
					nIndex++;
			}
		}

		// set help text
//		if( m_strScrollText.Length == 0)
//		{
//			string strBuf = "";
//			nIndex = 0;
//			for( int i = 0; i < AsCheatManager.Instance.GetCheatTableDataCount(); i++)
//			{
//				if( nIndex >= (int)eCheatHelpType.CH_Max)
//					break;
//				if( true == AsCheatManager.Instance.GetCheatString( i + 10000, out strBuf))
//				{
//					m_strScrollText += strBuf;
//					nIndex++;
//				}
//			}
//		}
	}
	
	void OnEnable()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	void OnDisable()
	{
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}
	
	private void FocusDelegate( UITextField field)
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		field.Text = string.Empty;
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void ApplyCheat()
	{
		string[] strSplit = edit.Text.Split( ' ');
		string strBuf = edit.Text.ToLower();
		string[] strRes = strBuf.Split( ' ');

		string msg = edit.Text;

		UIListItem item = list.CreateItem( listItem) as UIListItem;
		item.collider.enabled = false;
		item.transform.localPosition = new Vector3( -list.viewableArea.x * 0.5f, item.transform.localPosition.y, item.transform.localPosition.z);
		item.SetSize( list.viewableArea.x, 1.0f);
		item.spriteText.maxWidth = background.center.PixelSize.x;
		item.UpdateCamera();
		item.Text = msg;
		list.ScrollListTo( 1.0f);
		list.UpdateCamera();
		
		edit.Text = defaultText;

		for( int i = 0; i < (int)AsCheatManager.eCheatType.CT_Max; i++)
		{
			if( true == strRes[0].Equals( m_strCheatCommands[i].ToLower()))
			{
				switch( (AsCheatManager.eCheatType)i)
				{
				case AsCheatManager.eCheatType.CT_UIDel: AsCheatManager.Instance.Cheat_UIDel(); break;
				case AsCheatManager.eCheatType.CT_Level: AsCheatManager.Instance.Cheat_Level( strRes); break;
				case AsCheatManager.eCheatType.CT_Die: AsCheatManager.Instance.Cheat_Die(); break;
				case AsCheatManager.eCheatType.CT_Tel: AsCheatManager.Instance.Cheat_Tel( strSplit); break;
				case AsCheatManager.eCheatType.CT_Recall: AsCheatManager.Instance.Cheat_Recall( strSplit); break;
				case AsCheatManager.eCheatType.CT_Go: AsCheatManager.Instance.Cheat_Go( strSplit); break;
				case AsCheatManager.eCheatType.CT_Strong: AsCheatManager.Instance.Cheat_Strong( strRes); break;
				case AsCheatManager.eCheatType.CT_Hide: AsCheatManager.Instance.Cheat_Hide( strRes); break;
				case AsCheatManager.eCheatType.CT_Cool: AsCheatManager.Instance.Cheat_Cool( strRes); break;
				case AsCheatManager.eCheatType.CT_Speed: AsCheatManager.Instance.Cheat_Speed( strRes); break;
				case AsCheatManager.eCheatType.CT_HP: AsCheatManager.Instance.Cheat_HP( strRes); break;
				case AsCheatManager.eCheatType.CT_MP: AsCheatManager.Instance.Cheat_MP( strRes); break;
				case AsCheatManager.eCheatType.CT_AttSpeed: AsCheatManager.Instance.Cheat_AttSpeed( strRes); break;
				case AsCheatManager.eCheatType.CT_Accuracy: AsCheatManager.Instance.Cheat_Accuracy( strRes); break;
				case AsCheatManager.eCheatType.CT_Condition: AsCheatManager.Instance.Cheat_Condition( strRes); break;
				case AsCheatManager.eCheatType.CT_InventoryExtendMax: AsCheatManager.Instance.Cheat_InventoryExtendMax(); break;
				case AsCheatManager.eCheatType.CT_ItemMakeDropGroup: AsCheatManager.Instance.Cheat_ItemMakeDropGroup( strRes); break;
				case AsCheatManager.eCheatType.CT_ItemMake: AsCheatManager.Instance.Cheat_ItemMake( strRes); break;
				case AsCheatManager.eCheatType.CT_ItemUp: AsCheatManager.Instance.Cheat_ItemUp( strRes); break;
				case AsCheatManager.eCheatType.CT_Gold: AsCheatManager.Instance.Cheat_Gold( strRes); break;
				case AsCheatManager.eCheatType.CT_Miracle: AsCheatManager.Instance.Cheat_Miracle( strRes); break;
				case AsCheatManager.eCheatType.CT_ItemDel: AsCheatManager.Instance.Cheat_ItemDel(); break;
				case AsCheatManager.eCheatType.CT_Inchent: AsCheatManager.Instance.Cheat_Inchent(); break;
				case AsCheatManager.eCheatType.CT_DropRate: AsCheatManager.Instance.Cheat_DropRate( strRes); break;
				case AsCheatManager.eCheatType.CT_MobDel: AsCheatManager.Instance.Cheat_MobDel(); break;
				case AsCheatManager.eCheatType.CT_MobSpawn: AsCheatManager.Instance.Cheat_MobSpawn( strRes); break;
				case AsCheatManager.eCheatType.CT_MobStop: AsCheatManager.Instance.Cheat_MobStop( strRes); break;
				case AsCheatManager.eCheatType.CT_QAccept: AsCheatManager.Instance.Cheat_QAccept( strRes); break;
				case AsCheatManager.eCheatType.CT_QDel: AsCheatManager.Instance.Cheat_QDel( strRes); break;
				case AsCheatManager.eCheatType.CT_QReset: AsCheatManager.Instance.Cheat_QReset( strRes); break;
				case AsCheatManager.eCheatType.CT_QClear: AsCheatManager.Instance.Cheat_QClear( strRes); break;
				case AsCheatManager.eCheatType.CT_QComplete: AsCheatManager.Instance.Cheat_QComplete( strRes); break;
				case AsCheatManager.eCheatType.CT_QSet: AsCheatManager.Instance.Cheat_QSet( strRes); break;
				case AsCheatManager.eCheatType.CT_CollectMake: AsCheatManager.Instance.Cheat_CollectMake(); break;
				case AsCheatManager.eCheatType.CT_PrivateShop: AsCheatManager.Instance.Cheat_PrivateShop( strRes); break;
				case AsCheatManager.eCheatType.CT_SocialPoint: AsCheatManager.Instance.Cheat_SocialPoint( strRes); break;
				case AsCheatManager.eCheatType.CT_SkillLearn: AsCheatManager.Instance.Cheat_SkillLearn(); break;
				case AsCheatManager.eCheatType.CT_WayPoint: AsCheatManager.Instance.Cheat_WayPoint(); break;
				case AsCheatManager.eCheatType.CT_Rank: AsCheatManager.Instance.Cheat_Rank( strRes); break;
				case AsCheatManager.eCheatType.CT_EquipItemUpgrade: AsCheatManager.Instance.Cheat_EquipItemUpgrade( strRes); break;
				case AsCheatManager.eCheatType.CT_PvPPointModify: AsCheatManager.Instance.Cheat_PvPPointModify( strRes); break;
				case AsCheatManager.eCheatType.CT_UIOpen: AsCheatManager.Instance.Cheat_UIOpen(); break;
				case AsCheatManager.eCheatType.CT_PetLevel: AsCheatManager.Instance.Cheat_PetLevel( strRes); break;
				case AsCheatManager.eCheatType.CT_PetHungry: AsCheatManager.Instance.Cheat_PetHungry( strRes); break;
				case AsCheatManager.eCheatType.CT_AttendanceBonus: AsCheatManager.Instance.Cheat_AttendanceBonus(); break;
                case AsCheatManager.eCheatType.CT_ApPoint: AsCheatManager.Instance.Cheat_ApPointUp(strRes); break;
                case AsCheatManager.eCheatType.CT_ApSort: AsCheatManager.Instance.Cheat_ApRankSort(); break;
				case AsCheatManager.eCheatType.CT_Test: AsCheatManager.Instance.Cheat_Test( strRes); break;
				}
				
				return;
			}
			else
			{
				SetCheatResultTextErr( AsCheatManager.eCheatErrType.CE_NotFoundCommand);
			}
		}
	}
	
	public void SetCheatResultTextErr( AsCheatManager.eCheatErrType cheatErrType)
	{
		info.Text = "Note: " + m_strCheatCommandsErr[ (int)cheatErrType];
		info.Color = Color.red;
	}
	
	public void SetCheatResultText( AsCheatManager.eCheatType cheatType)
	{
		SetCheatResultText( cheatType, "", "");
	}

	public void SetCheatResultText( AsCheatManager.eCheatType cheatType, string strFront, string strTail)
	{
		info.Text = "Note: " + strFront + m_strCheatCommandsRes[(int)cheatType] + strTail;
		info.Color = Color.white;
	}
	
	void OnCloseBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		Close();
	}
	
	public void Close()
	{
		GameObject.Destroy( gameObject);
	}
}
