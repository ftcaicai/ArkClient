using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsPartyJoinRequestDlg : MonoBehaviour
{
	public SpriteText title = null;
	public SpriteText message = null;
	public UIButton okBtn = null;
	public UIButton cancelBtn = null;
	private body_SC_PARTY_JOIN_REQUEST_NOTIFY m_JoinRequestNotify;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Init( body_SC_PARTY_JOIN_REQUEST_NOTIFY data)
	{
		m_JoinRequestNotify = data;

		if ( AsHudDlgMgr.Instance.IsOpenCashStore == true) // if open cash store send refuse
		{
			AsPartySender.SendPartyJoinRequestAccept( m_JoinRequestNotify.nCharUniqKey, false);
			GameObject.DestroyImmediate( gameObject);
		}
		else
		{
			title.Text = AsTableManager.Instance.GetTbl_String(1730);
			string userName = Encoding.UTF8.GetString( m_JoinRequestNotify.szCharName);
			message.Text = string.Format( AsTableManager.Instance.GetTbl_String(2008), m_JoinRequestNotify.nLevel, AsUtil.GetClassName( ( (eCLASS) m_JoinRequestNotify.eClass)),m_JoinRequestNotify.nCurItemRankPoint, AsUtil.GetRealString( userName));

			cancelBtn.Text = AsTableManager.Instance.GetTbl_String(1151);
			okBtn.Text = AsTableManager.Instance.GetTbl_String(1152);
		}
	}

	private void OnOkBtn()
	{
		AsPartyManager.Instance.JoinRequestCount--;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false); //#20265.
		AsPartySender.SendPartyJoinRequestAccept( m_JoinRequestNotify.nCharUniqKey, true);
		GameObject.DestroyImmediate( gameObject);
	}

	private void OnCancelBtn()
	{
		AsPartyManager.Instance.JoinRequestCount--;
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false); //#20265.
		AsPartySender.SendPartyJoinRequestAccept( m_JoinRequestNotify.nCharUniqKey, false);
		GameObject.DestroyImmediate( gameObject);
	}
}
