using UnityEngine;
using System.Collections;

public class AsParticle : MonoBehaviour
{
	public AnimationCurve curveX = AnimationCurve.Linear( 0, 0, 10, 10 );
	public AnimationCurve curveY = AnimationCurve.Linear( 0, 0, 10, 10 );

	void Awake()
	{
		DontDestroyOnLoad( gameObject );
		DDOL_Tracer.RegisterDDOL(this, gameObject);//$ yde
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = new Vector3( curveX.Evaluate( Time.time ), curveY.Evaluate( Time.time ), -1.0f );
	}

	public void SetCurves( AnimationCurve x, AnimationCurve y )
	{
		curveX = x;
		curveY = y;
	}
}
