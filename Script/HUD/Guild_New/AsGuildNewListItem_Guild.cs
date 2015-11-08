using UnityEngine;
using System.Collections;

public class AsGuildNewListItem_Guild : MonoBehaviour 
{
	[SerializeField]SpriteText m_name = null;
	[SerializeField]SpriteText m_master = null;
	[SerializeField]SpriteText m_level = null;
	[SerializeField]SpriteText m_member = null;
	[SerializeField]SpriteText m_notice = null;

	private body2_SC_GUILD_SEARCH_RESULT data = null;
	public body2_SC_GUILD_SEARCH_RESULT Data
	{
		set	{ data = value; }
	}


	// Use this for initialization
	void Start () 
	{
		m_name.Text = AsUtil.GetRealString( data.szGuildName);
		m_level.Text = data.nLevel.ToString();
		m_master.Text = AsUtil.GetRealString( data.szMasterName);
		m_member.Text = string.Format( "{0}/{1}", 0, 10);
		m_notice.Text = AsUtil.GetRealString( data.szGuildPublicize);
	}

	public GameObject Select( GameObject parent)
	{
		Debug.Log( "OnSelect");
		
		GameObject go = Instantiate( Resources.Load( "UI/AsGUI/Guild_new/Popup_GuildList")) as GameObject;
		AsGuildNewPopup_GuildList dlg = go.GetComponentInChildren<AsGuildNewPopup_GuildList>();
		dlg.Open( parent , data );
		return go;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
