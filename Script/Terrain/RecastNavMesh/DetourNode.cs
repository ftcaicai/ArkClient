using UnityEngine;
using System.Collections;

public enum dtNodeFlags
{
	DT_NODE_OPEN = 0x01,
	DT_NODE_CLOSED = 0x02,
};


public class dtNode
{
	public uint index;
	public float cost;
	public float total;
	public uint id;
	public uint pidx;
	public uint flags;
};


// Structure holding the navigation polygon data.
public class dtStatPoly
{		
	public static int size = 26;
	
	public ushort []v = new ushort[DetourStatNavMesh.DT_STAT_VERTS_PER_POLYGON];	// Indices to vertices of the poly.
	public ushort []n = new ushort[DetourStatNavMesh.DT_STAT_VERTS_PER_POLYGON];	// Refs to neighbours of the poly.
	public byte nv;												// Number of vertices.
	public byte flags;											// Flags (not used).
};

public class dtStatPolyDetail
{
	public static int size = 8;
	
	public ushort vbase;	// Offset to detail vertex array.
	public ushort nverts;	// Number of vertices in the detail mesh.
	public ushort tbase;	// Offset to detail triangle array.
	public ushort ntris;	// Number of triangles.
};

public class dtStatBVNode
{
	public static int size = 16;
	
	public ushort[] bmin = new ushort[3];
	public ushort[] bmax = new ushort[3];
	public int i;
};

public class dtTriIdx
{
	public byte[] datas = new byte[4];
}

public class dtStatNavMeshHeader
{
	public static int size = 84;
	
	public int magic;
	public int version;
	public int npolys;
	public int nverts;
	public int nnodes;
	public int ndmeshes;
	public int ndverts;
	public int ndtris;
	public float cs;
	public float[] bmin = new float[3];
	public float[] bmax = new float[3];
	public dtStatPoly[] polys;
	public Vector3[] verts;
	public dtStatBVNode[] bvtree;
	public dtStatPolyDetail[] dmeshes;
	public Vector3[] dverts;
	public dtTriIdx[] dtris;
};