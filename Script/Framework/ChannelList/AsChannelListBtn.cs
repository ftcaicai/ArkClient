using UnityEngine;
using System.Collections;

public class AsChannelListBtn : MonoBehaviour
{
	public static bool s_ChannelSelected = false;//$yde
	
	[HideInInspector] public body2_GC_CHANNEL_LIST channelData;
	
	// Use this for initialization
	void Start()
	{
		s_ChannelSelected = false;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void EnterChannel()
	{
		if( s_ChannelSelected == false)
		{
			if( null != AsHudDlgMgr.Instance)
				AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694
			
			s_ChannelSelected = true;
			
//			AsUserInfo.Instance.currentChannelName = channelData.szChannelName;
			
			body_CG_CHANNEL_SELECT channelSelect = new body_CG_CHANNEL_SELECT( channelData.nChannel);
			byte[] data = channelSelect.ClassToPacketBytes();
			AsNetworkMessageHandler.Instance.Send( data);
		}
	}
}
