using UnityEngine;
using System.Collections;

public class AnimEvent_Base
{
}

public class AnimEventReceiver_Base : MonoBehaviour
{
	public virtual void ReceiveEvent( AnimEvent_Base _event)
	{
	}
}
