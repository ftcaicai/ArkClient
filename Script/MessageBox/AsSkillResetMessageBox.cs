using UnityEngine;
using System.Collections;

public class AsSkillResetMessageBox : AsMessageBox
{
	public SpriteText text_Cash = null;
	public SpriteText text_HaveCash = null;
	public SpriteText text_CashTitle = null;
	public SpriteText text_HaveCashTitle = null;

	public void SetCashText( long cost)
	{
		text_Cash.Text = cost.ToString();
	}

	public void SetCashText( string cost)
	{
		text_Cash.Text = cost;
	}

	public void SetHaveCashText( long cost)
	{
		text_HaveCash.Text = cost.ToString();
	}

	public void SetHaveCashText( string cost)
	{
		text_HaveCash.Text = cost;
	}
}
