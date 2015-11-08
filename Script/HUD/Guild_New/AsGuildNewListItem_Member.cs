using UnityEngine;
using System.Collections;

public class AsGuildNewListItem_Member : MonoBehaviour 
{
	[SerializeField]SimpleSprite 	m_imgKing = null;
	[SerializeField]SpriteText 		m_txtNum = null;
	[SerializeField]SpriteText 		m_txtTitle = null;

	[SerializeField]SimpleSprite 	m_imgClassAman = null;
	[SerializeField]SimpleSprite 	m_imgClassDemigod = null;
	[SerializeField]SimpleSprite 	m_imgClassElf = null;
	[SerializeField]SimpleSprite 	m_imgClassLumicle = null;

	[SerializeField]SpriteText 		m_txtLevel = null;
	[SerializeField]SpriteText 		m_txtName = null;

	[SerializeField]SimpleSprite 	m_imgAttendance = null;
	[SerializeField]UIButton 			m_btnAttendance = null;

	[SerializeField]SpriteText 		m_txtConnection = null;
	[SerializeField]SpriteText 		m_txtGoldSupport = null;
	[SerializeField]SpriteText 		m_txtGuildPoint = null;

	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	public body2_SC_GUILD_MEMBER_INFO_RESULT Data
	{
		get	{ return data; }
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Init( int nNumber , body2_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		this.data = data;
		
		Color textColor = ( false == data.bConnect) ? Color.gray : Color.white;

		bool isMaster = false;
		if (AsUserInfo.Instance.GuildData.szGuildMaster == data.szCharName)
			isMaster = true;

		//	number
		if (isMaster == true) 
		{
			m_imgKing.gameObject.SetActive(true);
			m_txtNum.gameObject.SetActive(false);
		} 
		else 
		{
			m_imgKing.gameObject.SetActive(false);
			m_txtNum.gameObject.SetActive(true);
			m_txtNum.Text = nNumber.ToString();
		}

		//	title
		m_txtTitle.Text = "ace";

		//	class , level , name
		m_imgClassAman.gameObject.SetActive(false);
		m_imgClassDemigod.gameObject.SetActive(false);
		m_imgClassElf.gameObject.SetActive(false);
		m_imgClassLumicle.gameObject.SetActive(false);
		switch( data.eClass)
		{
		case eCLASS.DIVINEKNIGHT:	m_imgClassDemigod.gameObject.SetActive(true);	break;
		case eCLASS.CLERIC:	m_imgClassLumicle.gameObject.SetActive(true);	break;
		case eCLASS.HUNTER:	m_imgClassAman.gameObject.SetActive(true);	break;
		case eCLASS.MAGICIAN:	m_imgClassElf.gameObject.SetActive(true);	break;
		default:	break;
		}

		m_txtLevel.Text = data.nLevel.ToString();
		m_txtName.Text = data.szCharName;


		//	attendance
		if (AsUserInfo.Instance.GetCharacterName () == data.szCharName) 
		{
			m_imgAttendance.gameObject.SetActive(false);
			m_btnAttendance.gameObject.SetActive(true);
		} 
		else 
		{
			m_imgAttendance.gameObject.SetActive(true);
			m_btnAttendance.gameObject.SetActive(false);
		}


		//	connection
		if( data.bConnect == false )
		{
			int	nOffDayCount = GetOffLineDay(data.nLastConnectTime);
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(2038) , nOffDayCount );
			m_txtConnection.Text = strMsg;
			m_txtConnection.Color = textColor;
		}
		else
		{
			m_txtConnection.Text = "Online";
		}

		//	gold support
		m_txtGoldSupport.Text = "10000";

		//	guild point
		m_txtGuildPoint.Text = "34567";
	}
	
	private int  GetOffLineDay(long nTime)
	{		
		System.DateTime lastConnectTime  = new System.DateTime(1970, 1, 1, 9, 0, 0);
		lastConnectTime = lastConnectTime.AddSeconds(nTime);
		System.DateTime curTime = System.DateTime.Now;   
		
		System.TimeSpan span = curTime.Subtract(lastConnectTime);
		
		if(span.Days > 99) 
			return 99;
		
		return span.Days;
	}
	
	public GameObject PromptPopup( GameObject parent)
	{
		if( data.nCharUniqKey == AsUserInfo.Instance.SavedCharStat.uniqKey_)
			return null;
		
		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildMemberPopup")) as GameObject;
		AsGuildMemberPopup popup = go.GetComponentInChildren<AsGuildMemberPopup>();
		popup.Init( this.data, parent);
		
		return go;
	}

}







































