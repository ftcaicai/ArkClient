using UnityEngine;
using System.Collections;

public class AsBlockOutItem : MonoBehaviour
{
	public SpriteText m_NickName = null;
	public UIButton m_UnBlockBtn;
	public uint m_nUserUniqKey;

	// Use this for initialization
	void Start()
	{
		m_UnBlockBtn.SetInputDelegate( UnBlockBtnDelegate);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetBlockOutData( body2_SC_BLOCKOUT_LIST Data)
	{
		m_nUserUniqKey = Data.nUserUniqKey;
		if( Data.szUserId.Length != 0)
			m_NickName.Text = Data.szUserId;
	}
	
	private void UnBlockBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsCommonSender.SendBlockOutDelete( m_nUserUniqKey);
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
}
