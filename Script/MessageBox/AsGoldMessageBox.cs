using UnityEngine;
using System.Collections;
using System.Globalization;

public class AsGoldMessageBox : AsMessageBox
{
	public SpriteText text_Gold = null;

	public void SetCashText( ulong cost)
	{
		text_Gold.Text = cost.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void SetCashText( string cost)
	{
		text_Gold.Text = cost;
	}
}