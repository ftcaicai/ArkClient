using UnityEngine;
using System.Collections;

public class FingerPointerController : MonoBehaviour
{
	public  UISimpleSpriteAni  ani;
	private bool bShowPointer = false;

	public bool ShowFingerPointer
	{
		get { return bShowPointer; }
		set
		{
			bShowPointer = value;
			gameObject.SetActiveRecursively(value);
			if (value == true)
				ani.Reset();
		}
	}

	void Start ()
	{
		gameObject.SetActiveRecursively(bShowPointer);
	}

	void OnEnable()
	{
		Debug.LogWarning("bShowPointer = " + bShowPointer);
		gameObject.SetActiveRecursively(bShowPointer);
	}
}
