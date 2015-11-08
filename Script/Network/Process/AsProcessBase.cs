using UnityEngine;

abstract public class AsProcessBase : MonoBehaviour
{
	abstract public void Process( byte[] packet);
}
