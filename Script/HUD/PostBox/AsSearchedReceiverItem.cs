using UnityEngine;
using System.Collections;

public class AsSearchedReceiverItem : MonoBehaviour
{
	[SerializeField] SpriteText nameText = null;
	[SerializeField] SpriteText levelText = null;
	[SerializeField] SpriteText classText = null;

	public string Name	{ get { return nameText.Text; } }

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	public void Init( body2_SC_POST_ADDRESS_BOOK info)
	{
		Color textColor = ( true == info.bConnect) ? Color.black : Color.gray;

		nameText.Text = info.szCharName;
		nameText.Color = textColor;
		levelText.Text = info.nLevel.ToString();
		levelText.Color = textColor;

		switch( info.eClass)
		{
		case eCLASS.DIVINEKNIGHT:	classText.Text = AsTableManager.Instance.GetTbl_String(1054);	break;
		case eCLASS.MAGICIAN:	classText.Text = AsTableManager.Instance.GetTbl_String(1055);	break;
		case eCLASS.CLERIC:	classText.Text = AsTableManager.Instance.GetTbl_String(1057);	break;
		case eCLASS.HUNTER:	classText.Text = AsTableManager.Instance.GetTbl_String(1056);	break;
		case eCLASS.ASSASSIN:	classText.Text = AsTableManager.Instance.GetTbl_String(1058);	break;
		default:	classText.Text = "Error";	break;
		}
		classText.Color = textColor;
	}
}
