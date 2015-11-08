using UnityEngine;
using System.Collections;

public class AsMemoryController : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
	}

	public void GarbageCollect()
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}
}
