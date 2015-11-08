using UnityEngine;
using System.Collections;
using System.Globalization;

public class CashShopBtnController : MonoBehaviour
{
	public SpriteText textMiracle;
	public GameObject objFreeMark;

	public void Start()
	{
		if( AsUserInfo.Instance != null)
			SetMiracleTxt( AsUserInfo.Instance.nMiracle);
	}

	public void SetMiracleTxt( long _miracle)
	{
		if( textMiracle != null)
			textMiracle.Text = _miracle.ToString( "#,#0", CultureInfo.InvariantCulture);
	}

	public void SetFreeMark(bool _value)
	{
		if (objFreeMark != null)
			objFreeMark.SetActive(_value);
	}
}
