using UnityEngine;
using System.Collections;

public class AsGuildInviteDlg : MonoBehaviour
{
	[SerializeField]SpriteText title = null;
	[SerializeField]SpriteText guide = null;
	[SerializeField]UITextField nameField = null;
	[SerializeField]UIButton cancel = null;
	[SerializeField]UIButton confirm = null;
	private GameObject parent = null;
	public GameObject Parent
	{
		set	{ parent = value; }
		get	{ return parent; }
	}
	
	// Use this for initialization
	void Start()
	{
		title.Text = AsTableManager.Instance.GetTbl_String(1165);
		guide.Text = AsTableManager.Instance.GetTbl_String(1200);
		cancel.Text = AsTableManager.Instance.GetTbl_String(1151);
		confirm.Text = AsTableManager.Instance.GetTbl_String(1152);
		nameField.Text = string.Empty;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void DisplayErrorMsg( string msg)
	{
		AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
	}
	
	private void OnCancelBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		parent.BroadcastMessage( "CloseInviteDlg");
	}
	
	private void OnConfirmBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		if( false == AsUtil.CheckCharacterName( nameField.Text))
			return;
		
		if( 0 >= nameField.Text.Length)
			return;
		
		body_CS_GUILD_INVITE guildInvite = new body_CS_GUILD_INVITE( nameField.Text);
		byte[] packet = guildInvite.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		
		string msg = string.Format( AsTableManager.Instance.GetTbl_String(239), nameField.Text);
		AsChatManager.Instance.InsertChat( msg, eCHATTYPE.eCHATTYPE_SYSTEM);
		
		nameField.Text = "";
	}
}
