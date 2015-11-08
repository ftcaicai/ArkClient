using UnityEngine;
using System.Collections;

public class AsCollectionTypeManager : MonoBehaviour
{
	public SimpleSprite[] typeIcons = new SimpleSprite[0];
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Display( AsBaseEntity entity)
	{
		gameObject.SetActive(true);
		
		foreach( SimpleSprite icon in typeIcons)
			icon.gameObject.SetActiveRecursively( false);
		
		eITEM_PRODUCT_TECHNIQUE_TYPE _type = (eITEM_PRODUCT_TECHNIQUE_TYPE)entity.GetProperty<int>( eComponentProperty.COLLECTOR_TECHNIC_TYPE);
		switch( _type)
		{
		case eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST:
			typeIcons[3].gameObject.SetActiveRecursively( true);
			break;
		case eITEM_PRODUCT_TECHNIQUE_TYPE.MINERAL:
			typeIcons[2].gameObject.SetActiveRecursively( true);
			break;
		case eITEM_PRODUCT_TECHNIQUE_TYPE.SPIRIT:
			typeIcons[1].gameObject.SetActiveRecursively( true);
			break;
		case eITEM_PRODUCT_TECHNIQUE_TYPE.PLANTS:
			typeIcons[0].gameObject.SetActiveRecursively( true);
			break;
		}
	}
}
