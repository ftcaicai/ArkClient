using UnityEngine;
using System.Collections;

public class AsChargeGuage : MonoBehaviour
{
	private Vector2 center = Vector2.zero;
	private float angle = 0.0f;
	private float percent = 0.0f;

	public float Value
	{
		get	{ return percent; }
		set
		{
			percent = Mathf.Clamp01( value);
			angle = 360.0f * percent;

			renderer.material.SetFloat( "_Angle", angle);
		}
	}

	public bool Enable
	{
		get	{ return gameObject.active; }
		set	{ gameObject.SetActiveRecursively( value); }
	}

	// Use this for initialization
	void Start()
	{
		GameObject go = GameObject.Find( "UICamera");
		Camera cam = go.GetComponent<Camera>();

		Vector3 pos = cam.WorldToViewportPoint( transform.position);

		renderer.material.SetFloat( "_Angle", angle);

		center = new Vector2( pos.x, pos.y);
		renderer.material.SetVector( "_Center", center);
	}
}
