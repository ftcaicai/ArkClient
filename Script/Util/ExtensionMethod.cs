using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethod
{
	public static int GetRandomValue(this List<int> _var)
	{
		return _var[Random.Range(0, _var.Count)];

	}
}
