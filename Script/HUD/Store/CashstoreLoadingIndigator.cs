
using UnityEngine;
using System.Collections;

public class CashstoreLoadingIndigator : MonoBehaviour
{
	[SerializeField]private SpriteText   _msgSpriteText = null;
	[SerializeField]private PackedSprite _gearSprite    = null;
	
	// Use this for initialization
	void Start()
	{
	}


	public void ShowIndigator(string msg)
	{
		_msgSpriteText.gameObject.SetActive(true);
		_gearSprite.gameObject.SetActive(true);
		_msgSpriteText.Text = msg;
	}
	
	public void HideIndigator()
	{
		if( GAME_STATE.STATE_LOADING == AsGameMain.s_gameState)
			return;

		_msgSpriteText.gameObject.SetActive(false);
		_gearSprite.gameObject.SetActive(false);
	}
}

