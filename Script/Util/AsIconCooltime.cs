using UnityEngine;
using System.Collections;

public class AsIconCooltime : MonoBehaviour
{
	private Vector2 center = Vector2.zero;
	private float angle = 0.0f;
	private float percent = 0.0f;

	public float Value
	{
		get	{ return percent; }
		set
		{
#if USE_NEW_COOLTIME
			angle = value;
			renderer.material.SetFloat( "_Angle", angle );
#else
			percent = Mathf.Clamp01( value);
			angle = 360.0f * percent;

			renderer.material.SetFloat( "_Angle", angle );
#endif
		}
	}

	public bool Enable
	{
		get	{ return renderer.enabled; }
		set	{ renderer.enabled = value; }
	}

	// Use this for initialization
	void Start()
	{
		AlignScreenPos();
	}
	
	public void AlignScreenPos()
	{
		Camera cam = AsInputManager.Instance.UICamera;
		if( null == cam )
			return;

		Vector3 pos = cam.WorldToViewportPoint( transform.position);

		renderer.material.SetFloat( "_Angle", angle);
#if USE_NEW_COOLTIME
#else
		center = new Vector2( pos.x, pos.y);
		renderer.material.SetVector( "_Center", center);
#endif
	}
}
