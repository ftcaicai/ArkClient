using UnityEngine;
using System.Collections;

public class AsMonsterAttackStyleManager : MonoBehaviour
{
	public SimpleSprite[] styles = new SimpleSprite[0];
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Display( AsBaseEntity target)
	{
		gameObject.SetActive(true);
		
		styles[0].gameObject.SetActive( false);
		styles[1].gameObject.SetActive( false);
		
		if( null == target)
			return;
		
		eMonster_AttackStyle attackStyle = target.GetProperty<eMonster_AttackStyle>( eComponentProperty.MONSTER_ATTACK_STYLE);
		if( eMonster_AttackStyle.NONE == attackStyle)
			return;
		
		styles[ (int)attackStyle - 1].gameObject.SetActive( true);
	}
}
