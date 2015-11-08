using UnityEngine;
using System.Collections;

public enum TutorialFingerType
{
	HIDE,
	TOP,
	BOTTOM,
	LEFT,
	RIGHT,
};

public class TutorialFingerController : MonoBehaviour
{
	public bool doubleTouch = false;
	public TutorialFingerType fingerType = TutorialFingerType.TOP;
	public GameObject[] objFingers = null;

	// Use this for initialization
	void Start()
	{
		foreach( GameObject finger in objFingers)
		{
			SpriteText text = finger.transform.GetChild(0).gameObject.GetComponent<SpriteText>();
			AsLanguageManager.Instance.SetFontFromSystemLanguage( text);
			text.Text = AsTableManager.Instance.GetTbl_String(1759);
		}

		DisableAllFinger();
	}

	void OnEnable()
	{
		SetFingerType(fingerType, doubleTouch);
	}

	public void SetFingerType( TutorialFingerType _type, bool _doubleTouch = false)
	{
		DisableAllFinger();

		fingerType = _type;
		doubleTouch = _doubleTouch;

		if( fingerType != TutorialFingerType.HIDE)
		{
			GameObject nowFinger = objFingers[(int)_type - 1];

			nowFinger.SetActiveRecursively(true);

			if( _doubleTouch == false)
				nowFinger.transform.GetChild(0).gameObject.SetActiveRecursively( false);
			else
				nowFinger.transform.GetChild(0).gameObject.SetActiveRecursively( true);
		}
	}

	public void DisableAllFinger()
	{
		foreach( GameObject obj in objFingers)
			obj.SetActiveRecursively( false);
	}
}
