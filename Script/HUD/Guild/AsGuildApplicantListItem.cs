using UnityEngine;
using System.Collections;

public class AsGuildApplicantListItem : MonoBehaviour
{
	[SerializeField]SpriteText nameLabel = null;
	[SerializeField]SpriteText level = null;
	[SerializeField]SpriteText cls = null;
	
	private body2_SC_GUILD_MEMBER_INFO_RESULT data =  null;
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
			
		Color textColor = Color.black;
		if( false == data.bConnect)
			textColor = Color.gray;
		
		nameLabel.Text = data.szCharName;
		nameLabel.Color = textColor;
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
	}

	public GameObject PromptPopup( GameObject parent)
	{
		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildApplicantPopup")) as GameObject;
		AsGuildApplicantPopup popup = go.GetComponentInChildren<AsGuildApplicantPopup>();
		popup.Init( this.data, parent);
		
		return go;
	}
}
