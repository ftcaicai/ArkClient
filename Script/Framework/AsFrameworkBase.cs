using UnityEngine;
using System.Collections;

public abstract class AsFrameworkBase : MonoBehaviour
{
	public abstract void PrevStep();
	public abstract void NextStep();
	public abstract void DeactivePrevStep();
	public abstract void OnNotify( NOTIFY_MSG msg);
}
