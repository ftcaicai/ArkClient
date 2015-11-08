using UnityEngine;
using System.Collections;

public class AsCharacterClassSymbol : MonoBehaviour
{
	public SimpleSprite symbol_1 = null;
	public SimpleSprite symbol_2 = null;
	public SimpleSprite symbol_3 = null;
	public SimpleSprite symbol_4 = null;
	public SimpleSprite symbol_5 = null;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetType( eCLASS type)
	{
		switch( type)
		{
		case eCLASS.DIVINEKNIGHT:
			symbol_1.renderer.enabled = true;
			symbol_2.renderer.enabled = false;
			symbol_3.renderer.enabled = false;
			symbol_4.renderer.enabled = false;
			symbol_5.renderer.enabled = false;
			break;
//		case eCLASS.MAGICIAN:
//			symbol_1.renderer.enabled = false;
//			symbol_2.renderer.enabled = true;
//			symbol_3.renderer.enabled = false;
//			symbol_4.renderer.enabled = false;
//			symbol_5.renderer.enabled = false;
//			break;
		default:
			symbol_1.renderer.enabled = false;
			symbol_2.renderer.enabled = false;
			symbol_3.renderer.enabled = false;
			symbol_4.renderer.enabled = false;
			symbol_5.renderer.enabled = false;
//			Debug.LogError( "Invalid class type!!!");
			break;
		}
	}
}
