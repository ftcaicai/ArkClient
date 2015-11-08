using UnityEngine;
using System.Collections;
using System.Globalization;

public class MiracleInfoPopup : MonoBehaviour {

	public float fShowTime = 5.0f;
	public SpriteText txtMiracleTitle;
	public SpriteText txtFreeMiracleTitle;
	public SpriteText txtMiracle;
	public SpriteText txtFreeMiracle;

	private float fRemainTime = 0.0f;

	// Use this for initialization
	void Start () 
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtMiracleTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage(txtFreeMiracleTitle);
		txtMiracleTitle.Text = AsTableManager.Instance.GetTbl_String(2232);
		txtFreeMiracleTitle.Text = AsTableManager.Instance.GetTbl_String(2233);
	}

	public void Show(System.Int64 _miracle, System.Int64 _freeMiracle)
	{
		txtMiracle.Text = _miracle.ToString("#,#0", CultureInfo.InvariantCulture);
		txtFreeMiracle.Text = _freeMiracle.ToString("#,#0", CultureInfo.InvariantCulture);
		fRemainTime = fShowTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateTime();
	}

	void UpdateTime()
	{
		if (fRemainTime >= 0.0f)
		{
			fRemainTime -= Time.deltaTime;

			if (fRemainTime <= 0.0f)
				gameObject.SetActive(false);
		}
	}

}
