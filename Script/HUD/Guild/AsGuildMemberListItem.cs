using UnityEngine;
using System.Collections;

public class AsGuildMemberListItem : MonoBehaviour
{
	[SerializeField]SpriteText nameField = null;
	[SerializeField]SpriteText level = null;
	[SerializeField]SpriteText cls = null;
	[SerializeField]SpriteText online = null;
	private body2_SC_GUILD_MEMBER_INFO_RESULT data = null;
	public body2_SC_GUILD_MEMBER_INFO_RESULT Data
	{
		get	{ return data; }
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body2_SC_GUILD_MEMBER_INFO_RESULT data)
	{
		this.data = data;
			
		Color textColor = ( false == data.bConnect) ? Color.gray : Color.black;
		
		nameField.Text = data.szCharName;
		nameField.Color = textColor;
		level.Text = data.nLevel.ToString();
		level.Color = textColor;
		
		switch( data.eClass)
		{
		case eCLASS.DIVINEKNIGHT:	cls.Text = AsTableManager.Instance.GetTbl_String(1054);	break;
		case eCLASS.MAGICIAN:	cls.Text = AsTableManager.Instance.GetTbl_String(1055);	break;
		case eCLASS.CLERIC:	cls.Text = AsTableManager.Instance.GetTbl_String(1057);	break;
		case eCLASS.HUNTER:	cls.Text = AsTableManager.Instance.GetTbl_String(1056);	break;
		case eCLASS.ASSASSIN:	cls.Text = AsTableManager.Instance.GetTbl_String(1058);	break;
		default:	cls.Text = "Error";	break;
		}
		cls.Color = textColor;
		
		if( data.bConnect == false )
		{
			int	nOffDayCount = GetOffLineDay(data.nLastConnectTime);
			string strMsg = string.Format( AsTableManager.Instance.GetTbl_String(2038) , nOffDayCount );
			online.Text = strMsg;
			online.Color = textColor;
		}
		else
		{
			online.Text = "";
		}
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
