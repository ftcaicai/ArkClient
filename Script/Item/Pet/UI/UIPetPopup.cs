using UnityEngine;
using System.Collections;

public class UIPetPopup : MonoBehaviour {
	
	void OnEnable()
	{
		AsPetManager.SetPopup( true);
	}
	
	void OnDestroy()
	{
		AsPetManager.SetPopup( false);
	}
}
