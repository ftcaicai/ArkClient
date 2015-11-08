using UnityEngine;
using System.Collections;

public class AsAttribInfoDlg : MonoBehaviour
{
	public SpriteText title = null;
	public SpriteText leftTop = null;
	public SpriteText middleTop = null;
	public SpriteText rightTop = null;
	public SpriteText leftMiddle = null;
	public SpriteText center = null;
	public SpriteText rightMiddle = null;
	public SpriteText leftBottom = null;
	public SpriteText middleBottom = null;
	public SpriteText rightBottom = null;
	
	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( leftTop);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( middleTop);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rightTop);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( leftMiddle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( center);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rightMiddle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( leftBottom);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( middleBottom);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( rightBottom);

		title.Text = AsTableManager.Instance.GetTbl_String(1932);
		leftTop.Text = AsTableManager.Instance.GetTbl_String(1924);
		middleTop.Text = AsTableManager.Instance.GetTbl_String(1930);
		rightTop.Text = AsTableManager.Instance.GetTbl_String(1925);
		leftMiddle.Text = AsTableManager.Instance.GetTbl_String(1926);
		center.Text = AsTableManager.Instance.GetTbl_String(1930);
		rightMiddle.Text = AsTableManager.Instance.GetTbl_String(1927);
		leftBottom.Text = AsTableManager.Instance.GetTbl_String(1928);
		middleBottom.Text = AsTableManager.Instance.GetTbl_String(1931);
		rightBottom.Text = AsTableManager.Instance.GetTbl_String(1929);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	private void OnClose()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		
		AsMonsterAttrManager.infoDlg = null;
		GameObject.DestroyImmediate( gameObject);
	}
}
