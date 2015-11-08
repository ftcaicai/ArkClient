using UnityEngine;
using System.Collections;

public class AsGuildApprovalDlg : MonoBehaviour
{
	public SpriteText title = null;
	public SpriteText message = null;
	public UIButton okBtn = null;
	public UIButton cancelBtn = null;
	private body_SC_GUILD_INVITE data;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( body_SC_GUILD_INVITE data)
	{
		this.data = data;

        if (AsHudDlgMgr.Instance.IsOpenCashStore == true) // if open cash store send refuse
        {
            body_CS_GUILD_JOIN guildJoin = new body_CS_GUILD_JOIN(eGUILDJOINTYPE.eGUILDJOINTYPE_REFUSE, data.nGuildIdx, data.nCharUniqKey);
            byte[] packet = guildJoin.ClassToPacketBytes();
            AsNetworkMessageHandler.Instance.Send(packet);
            GameObject.DestroyImmediate(gameObject);
        }
        else
        {
            title.Text = AsTableManager.Instance.GetTbl_String(1240);
            string msg = string.Format(AsTableManager.Instance.GetTbl_String(236), data.szGuildName, data.szCharName);
            message.Text = msg;
            cancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);
            okBtn.Text = AsTableManager.Instance.GetTbl_String(1280);
        }
	}
	
	private void OnOkBtn()
	{
		AsHudDlgMgr.Instance.CloseGuildDlg();
		
		body_CS_GUILD_JOIN guildJoin = new body_CS_GUILD_JOIN( eGUILDJOINTYPE.eGUILDJOINTYPE_ACCEPT, data.nGuildIdx, data.nCharUniqKey);
		byte[] packet = guildJoin.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);

		GameObject.DestroyImmediate( gameObject);
	}
	
	private void OnCancelBtn()
	{
		// #11299 - begin
		body_CS_GUILD_JOIN guildJoin = new body_CS_GUILD_JOIN( eGUILDJOINTYPE.eGUILDJOINTYPE_REFUSE, data.nGuildIdx, data.nCharUniqKey);
		byte[] packet = guildJoin.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( packet);
		// #11299 - end

		GameObject.DestroyImmediate( gameObject);
	}
}
