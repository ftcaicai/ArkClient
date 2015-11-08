using UnityEngine;
using System.Collections;

[AddComponentMenu( "ArkSphere/MeshCombiner")]
[RequireComponent( typeof( MeshFilter))]
[RequireComponent( typeof( MeshRenderer))]

public class AsMeshCombiner : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh.Clear();
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>( true);
		MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
		meshRenderer.materials = meshFilters[0].renderer.sharedMaterials;
		
		CombineInstance[] combine = new CombineInstance[ meshFilters.Length - 1];
		
		int i = 0;
		int ci = 0;
		while( i < meshFilters.Length)
		{
			if( meshFilter != meshFilters[i])
			{
				combine[ci].mesh = meshFilters[i].sharedMesh;
				combine[ci].transform = meshFilters[i].transform.localToWorldMatrix;
				++ci;
			}
			
			meshFilters[i].gameObject.active = false;
			i++;
		}
		
		meshFilter.mesh.CombineMeshes( combine);
		transform.gameObject.active = true;
		
		MeshCollider meshCollider = transform.gameObject.GetComponent<MeshCollider>();
		if( null != meshCollider)
			meshCollider.sharedMesh = transform.gameObject.GetComponent<MeshFilter>().mesh;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
