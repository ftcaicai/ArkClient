using UnityEngine;
using System.Collections;

public enum CHARACTER_HAIR_STYLE
{
	STYLE_INVALID = -1,
	
	STYLE_1,
	STYLE_2,
	STYLE_3,
	
	MAX_STYLE
};

public enum CHARACTER_COLOR
{
	COLOR_INVALID = -1,
	
	COLOR_1,
	COLOR_2,
	COLOR_3,

	MAX_COLOR
};

public class AsCharacterRepresentative : MonoBehaviour
{
	public SimpleSprite[] representatives = new SimpleSprite[0];
	
	public bool Enable
	{
		set
		{
			gameObject.SetActiveRecursively( value);
		}
		
		get { return gameObject.active; }
	}
	
	// Use this for initialization
	void Start()
	{
		Reset();
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetRace( eRACE race)
	{
		Reset();
		
		representatives[ (int)race - 1].gameObject.SetActiveRecursively( true);
	}
	
	private void Reset()
	{
		foreach( SimpleSprite rep in representatives)
			rep.gameObject.SetActiveRecursively( false);
	}
}
