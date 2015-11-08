using UnityEngine;
using System.Collections;

public class AsChargingEffect : MonoBehaviour
{
	public bool Enable
	{
		set	{ gameObject.SetActiveRecursively( value); }
	}

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
