using UnityEngine;
using System;
using System.Collections;

public class AnimEvent_EyeBlink : AnimEvent_Base
{
	public AnimationEventInfo info_;
	public float speed_;

	public AnimEvent_EyeBlink( AnimationEventInfo _info, float _speed)
	{
		info_ = _info;
		speed_ = _speed;
	}
}

[RequireComponent( typeof( MeshFilter))]
[RequireComponent( typeof( MeshRenderer))]
class AnimEventReceiver_EyeBlink : AnimEventReceiver_Base
{
	internal class BlendShapeVertex
	{
		public int originalIndex;
		public Vector3 position;
		public Vector3 normal;
	}

	internal class BlendShape
	{
		public BlendShapeVertex[] vertices;// = new Array();
	}

	public Mesh sourceMesh = null; //The original mesh
	public Mesh attributeMesh = null; // destination mesh
	
	private BlendShape blendShape;
	private Mesh workingMesh;

	private float curDelta = 0;

	void Awake()
	{
		MeshFilter filter = gameObject.GetComponent( typeof( MeshFilter)) as MeshFilter;
		filter.sharedMesh = sourceMesh;
		workingMesh = filter.mesh;

		if ( attributeMesh.vertexCount != sourceMesh.vertexCount)
		{
			Debug.Log( "Target mesh doesn't have the same number of vertices as the source mesh");
			return;
		}

		//Build blend shapes
		BuildBlendShapes();
	}

	void BuildBlendShapes()
	{
		blendShape = new BlendShape();
		blendShape.vertices = new BlendShapeVertex[workingMesh.vertexCount];
		for ( int j = 0; j < workingMesh.vertexCount; j++)
		{
			BlendShapeVertex blendShapeVertex = new BlendShapeVertex();
			blendShapeVertex.originalIndex = j;
			blendShapeVertex.position = attributeMesh.vertices[j] - workingMesh.vertices[j];
			blendShapeVertex.normal = attributeMesh.normals[j] - workingMesh.normals[j];

			blendShape.vertices[j] = blendShapeVertex;
		}
	}

	void SetMorph()
	{
		Vector3[] morphedVertices = sourceMesh.vertices;
		Vector3[] morphedNormals = sourceMesh.normals;

		for ( int i = 0; i < blendShape.vertices.Length; i++)
		{
			morphedVertices[blendShape.vertices[i].originalIndex] += blendShape.vertices[i].position * curDelta;
			morphedNormals[blendShape.vertices[i].originalIndex] += blendShape.vertices[i].normal * curDelta;
		}

		//Update the actual mesh with new vertex and normal information, then recalculate the mesh bounds.
		workingMesh.vertices = morphedVertices;
		workingMesh.normals = morphedNormals;
		workingMesh.RecalculateBounds();
	}

	public override void ReceiveEvent( AnimEvent_Base _event)
	{
		if( _event.GetType() == typeof( AnimEvent_EyeBlink))
		{
			AnimEvent_EyeBlink eyeBlink = _event as AnimEvent_EyeBlink;
			StartCoroutine( "Morphing", eyeBlink.speed_);
		}
	}

	private IEnumerator Morphing( float _speed)
	{
		curDelta = 0;

		while( true)
		{
			yield return new WaitForSeconds( 0.01f);

			curDelta += ( Time.deltaTime * _speed);

			if( 1 < curDelta)
				break;

			SetMorph();
		}
	}
}