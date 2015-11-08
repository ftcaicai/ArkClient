using UnityEngine;
using System.Collections;


public enum eLISTEN_EVENT
{
	NONE = -1,
	
	partyMenuBtnClose,
}	


public class AsEventHeader
{
	public eLISTEN_EVENT	nType = eLISTEN_EVENT.NONE;
	
	public AsEventHeader(eLISTEN_EVENT type)
	{
		nType = type;
	}
}

public interface IEventListener
{
	void	ListenEvent(AsEventHeader	eventData);
}
