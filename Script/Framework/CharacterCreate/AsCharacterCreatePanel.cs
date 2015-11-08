using UnityEngine;
using System.Collections;

public abstract class AsCharacterCreatePanel : MonoBehaviour
{
	[HideInInspector]
	public AsCharacterCreateFramework prtFramework = null;
	public abstract void Initialize();
	public void Hide( bool flag)
	{
		gameObject.SetActiveRecursively( !flag);
		
		if( false == flag)
			Initialize();
	}
}
