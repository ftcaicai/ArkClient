using UnityEngine;
using System.Collections.Generic;

public class TronTrail_CS : MonoBehaviour
{
	public float height = 2f;
	public float time = 2f;
	public bool alwaysUp = false;
	public float minDistance = 0.1f;

	public Color startColor = Color.white;
	public Color endColor = new Color( 1f, 1f, 1f, 0f);

	public Texture defaultTexture = null;

	List<TronTrailSection> sections = new List<TronTrailSection>();

	public bool update = false;
	MeshFilter mf;
	Transform myTransform;

	void Awake()
	{
		mf = GetComponent<MeshFilter>() as MeshFilter;
		myTransform = transform;
	}

	// Use this for initialization
	void Start ()
	{
		Transform d1 = null;
		Transform d2 = null;

		Transform[] trans = myTransform.parent.gameObject.GetComponentsInChildren<Transform>();
		foreach( Transform tran in trans)
		{
			if( tran.name == "blade01")
				d1 = tran;
			else if( tran.name == "blade02")
				d2 = tran;

			if( d1 != null && d2 != null)
			{
				myTransform.position = d1.position;
				myTransform.rotation = d1.rotation;
				height = Vector3.Distance( d1.position, d2.position);
				break;
			}
		}

		defaultTexture = renderer.material.mainTexture;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if( update == false)
			return;

		Vector3 position = myTransform.position;
		float now = Time.time;

		// Remove old Sections
		for( int i = 0; i < sections.Count;)
		{
			if( now > sections[i].time + time)
				sections.Remove( sections[i++]);
			else
				++i;
		}

		// Rebuild the mesh
		mf.mesh.Clear();

		// Add new trail Section
		if( sections.Count == 0 || ( sections[0].point - position).sqrMagnitude > minDistance * minDistance)
		{
			TronTrailSection section = new TronTrailSection();
			section.point = position;
			if( alwaysUp)
				section.upDir = Vector3.up;
			else
				section.upDir = myTransform.TransformDirection( Vector3.up);

			section.time = now;
			sections.Insert( 0, section);
		}

		// We need least 2 Section to Create the Line
		if( sections.Count < 2)
			return;

		Vector3[] vertices = new Vector3[sections.Count * 2];
		Color[] colors = new Color[sections.Count * 2];
		Vector2[] uv = new Vector2[sections.Count * 2];

		TronTrailSection currentSection = sections[0];

		// Use matrix instead of transform. TransformPoint for performance reasons
		Matrix4x4 localSpaceTransform = myTransform.worldToLocalMatrix;

		// Generate vertex, uv and colors
		for( int i = 0; i < sections.Count; ++i)
		{
			currentSection = sections[i];

			// Calculate u for texture uv and color interpolation
			float u = 0f;
			if( i != 0)
				u = Mathf.Clamp01( ( Time.time - currentSection.time) / time);

			// Calculate upwards direction
			Vector3 upDir = currentSection.upDir;

			// Generate verices
			vertices[ i * 2 + 0 ] = localSpaceTransform.MultiplyPoint( currentSection.point);
			vertices[ i * 2 + 1 ] = localSpaceTransform.MultiplyPoint( currentSection.point + upDir * height);

			uv[ i * 2 + 0 ] = new Vector2( u, 0);
			uv[ i * 2 + 1 ] = new Vector2( u, 1);

			// fade colors out over time
			Color interpolateColor = Color.Lerp( startColor, endColor, u);
			colors[ i * 2 + 0] = interpolateColor;
			colors[ i * 2 + 1] = interpolateColor;
		}

		// Generate triangles indices
		int[] triangles = new int[ ( sections.Count - 1) * 2 * 3];
		for( int i = 0; i  < triangles.Length / 6; ++i)
		{
			triangles[i * 6 + 0] = i * 2;
			triangles[i * 6 + 1] = i * 2 + 1;
			triangles[i * 6 + 2] = i * 2 + 2;

			triangles[i * 6 + 3] = i * 2 + 2;
			triangles[i * 6 + 4] = i * 2 + 1;
			triangles[i * 6 + 5] = i * 2 + 3;
		}

		// Assign to mesh
		mf.mesh.vertices = vertices;
		mf.mesh.colors = colors;
		mf.mesh.uv = uv;
		mf.mesh.triangles = triangles;
	}

	public void SetUpdate( bool _update)
	{
		update = _update;

		if( !_update)
		{
			for( int i = 0; i < sections.Count;)
			{
				sections.Remove( sections[i++]);
			}

			mf.mesh.Clear();
		}
	}

	public void SetTime( float _time)
	{
		time = _time;
	}

	public void SetTexture( string _textureName)
	{
		//renderer.material.mainTexture
	}

	public void SetDefaultTexture()
	{
		renderer.material.mainTexture = defaultTexture;
	}
}

public class TronTrailSection
{
	public Vector3 point = Vector3.zero;
	public Vector3 upDir = Vector3.zero;
	public float time = 0f;
}
