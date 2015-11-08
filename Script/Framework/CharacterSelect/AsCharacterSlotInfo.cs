using UnityEngine;
using System.Collections;

public class AsCharacterSlotInfo : MonoBehaviour
{
	[SerializeField]SimpleSprite empty = null;
	[SerializeField]SimpleSprite locked = null;
	[SerializeField]SpriteText level = null;
	[SerializeField]SpriteText nameLabel = null;
	[SerializeField]SimpleSprite emptySlot = null;
	[SerializeField]SpriteText txtLocked = null;//$yde

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void SetData( AsUserEntity data, bool isLocked)
	{
		// < ilmeda 20120814
		AsLanguageManager.Instance.SetFontFromSystemLanguage( level);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( nameLabel);
		// ilmeda 20120814 >

		txtLocked.gameObject.SetActiveRecursively( false);
		locked.gameObject.SetActiveRecursively( isLocked);

		if( null == data)
		{
			empty.gameObject.SetActiveRecursively( false);
			level.gameObject.SetActiveRecursively( false);
			nameLabel.gameObject.SetActiveRecursively( false);
			emptySlot.gameObject.SetActiveRecursively( true);
		}
		else
		{
			bool isEmpty = data.ContainProperty( eComponentProperty.EMPTY_SLOT);
			
			if( true == isEmpty)
			{
				if( AsCharacterSlotManager.PossibleCharCreate == true)
				{
					empty.gameObject.SetActiveRecursively( true);
					txtLocked.gameObject.SetActiveRecursively( false);
				}
				else
				{
					Debug.Log( "empty.gameObject.SetActiveRecursively( false)_2");
					empty.gameObject.SetActiveRecursively( false);
					txtLocked.gameObject.SetActiveRecursively( true);
					txtLocked.Text = AsTableManager.Instance.GetTbl_String(1531);
				}

				level.gameObject.SetActiveRecursively( false);
				nameLabel.gameObject.SetActiveRecursively( false);
				emptySlot.gameObject.SetActiveRecursively( true);
			}
			else
			{
				Debug.Log( "empty.gameObject.SetActiveRecursively( false)_3");
				empty.gameObject.SetActiveRecursively( false);
				level.gameObject.SetActiveRecursively( true);
				nameLabel.gameObject.SetActiveRecursively( true);
				emptySlot.gameObject.SetActiveRecursively( false);

				level.Text = string.Format( "Lv.{0}", data.GetProperty<int>( eComponentProperty.LEVEL));
				nameLabel.Text = data.GetProperty<string>( eComponentProperty.NAME);
			}
		}
	}
}
