using UnityEngine;
using System.Collections;


[AddComponentMenu( "ArkSphere/EffectCulling")]
[ExecuteInEditMode]

public class AsEffectCulling : MonoBehaviour
{
	Bounds m_bounds = new Bounds();
	bool m_bVisible = true;
	public Vector3 m_Size = Vector3.one;
	public Vector3 m_Center = Vector3.zero;

	void Start()
	{
	}

	void SetChildVisible( bool bVisible)
	{
		m_bVisible = bVisible;

		for( int i = 0; i < gameObject.transform.childCount; ++i)
		{
			if( bVisible != gameObject.transform.GetChild(i).gameObject.active)
				gameObject.transform.GetChild(i).gameObject.SetActiveRecursively( bVisible);
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (AsFrameSkipManager.Instance.IsFrameSkip_LateUpdate () == true) 
			return;

		if( true == Application.isEditor)
			m_bounds.size = m_Size;

		m_bounds.center = gameObject.transform.position + m_Center;
		if( true == IsVisibleFrom( m_bounds, Camera.main))
		{
			if( false == m_bVisible)
				SetChildVisible( true);
		}
		else
		{
			SetChildVisible( false);
		}
	}

	void OnEnable()
	{
		m_bounds.size = m_Size;
	}

	public bool IsVisibleFrom( Bounds bounds, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes( camera);
		return GeometryUtility.TestPlanesAABB( planes, bounds);
	}

	void OnDrawGizmosSelected()
	{
		Vector3 center = m_bounds.center;
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube( center, m_bounds.size);
	}
}
