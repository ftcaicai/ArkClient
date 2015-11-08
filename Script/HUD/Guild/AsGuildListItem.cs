using UnityEngine;
using System.Collections;

public class AsGuildListItem : MonoBehaviour
{
	[SerializeField]SpriteText nameField = null;
	[SerializeField]SpriteText level = null;
	[SerializeField]SpriteText master = null;
	private body2_SC_GUILD_SEARCH_RESULT data = null;
	public body2_SC_GUILD_SEARCH_RESULT Data
	{
		set	{ data = value; }
	}
	
	// Use this for initialization
	void Start()
	{
		nameField.Text = AsUtil.GetRealString( data.szGuildName);
		level.Text = data.nLevel.ToString();
		master.Text = AsUtil.GetRealString( data.szMasterName);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public GameObject Select( GameObject parent)
	{
		Debug.Log( "OnSelect");
		
		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild/GUI_GuildInfo")) as GameObject;
		AsGuildJoinRequestDlg dlg = go.GetComponentInChildren<AsGuildJoinRequestDlg>();
		dlg.Open( data, parent);
		
		return go;
	}
}
