using UnityEngine;
using System.Collections;

public class AsMonsterAttrManager : MonoBehaviour
{
	public SimpleSprite[] attrs = new SimpleSprite[0];
	static public GameObject infoDlg = null;
	
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
		
		foreach( SimpleSprite attr in attrs)
			attr.gameObject.SetActiveRecursively( false);
		
		int elementalIndex = entity.GetProperty<int>( eComponentProperty.ATTRIBUTE);
		switch( elementalIndex)
		{
		case 0:
			attrs[ (int)ePotency_Element.NONE].gameObject.SetActiveRecursively( true);
			break;
		case 1:
		case 6:
		case 10:
			attrs[ (int)ePotency_Element.Fire].gameObject.SetActiveRecursively( true);
			break;
		case 2:
		case 7:
		case 11:
			attrs[ (int)ePotency_Element.Ice].gameObject.SetActiveRecursively( true);
			break;
		case 3:
		case 8:
		case 12:
			attrs[ (int)ePotency_Element.Light].gameObject.SetActiveRecursively( true);
			break;
		case 4:
		case 9:
		case 13:
			attrs[ (int)ePotency_Element.Dark].gameObject.SetActiveRecursively( true);
			break;
		case 5:
			attrs[ (int)ePotency_Element.Nature].gameObject.SetActiveRecursively( true);
			break;
		}
	}
	
	private void OnInfoBtn()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		if( null != infoDlg)
		{
			DestroyImmediate( infoDlg);
			infoDlg = null;
			return;
		}
		
		GameObject dlg = ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_ElementalInfo");
		
		infoDlg = GameObject.Instantiate( dlg) as GameObject;
	}
}
