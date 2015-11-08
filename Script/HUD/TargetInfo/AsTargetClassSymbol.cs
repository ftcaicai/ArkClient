using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsTargetClassSymbol : MonoBehaviour
{
	public List<SimpleSprite> symbols = new List<SimpleSprite>();
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( eCLASS targetClass)
	{
		foreach( SimpleSprite symbol in symbols)
		{
			symbol.gameObject.SetActiveRecursively( false);
		}
		
		symbols[ (int)targetClass].gameObject.SetActiveRecursively( true);
	}
}
