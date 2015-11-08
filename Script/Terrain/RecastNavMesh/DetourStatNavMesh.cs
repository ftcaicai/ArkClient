using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;



public class DetourStatNavMesh
{		
	//---------------------------------------------------------------------
	/* Public Static Variable */
	//---------------------------------------------------------------------		
	public static int DT_STAT_VERTS_PER_POLYGON = 6;	
	public static int DT_STAT_NAVMESH_MAGIC = 1312904781;
	public static int DT_STAT_NAVMESH_VERSION = 3;	
	public static float FLT_MAX = 3.402823466e+38F;       // max value
		
		
	//---------------------------------------------------------------------
	/* Private Variable */
	//---------------------------------------------------------------------			
	/*private byte[] m_data;
	//private int m_dataSize;*/
	private dtStatNavMeshHeader m_header;
	private dtNodePool m_nodePool;
	private dtNodeQueue m_openList;
	private int m_iIndex = 0;
	
	
	//---------------------------------------------------------------------
	/* Public Function */
	//---------------------------------------------------------------------	
	
	public int index 
	{
		get
		{
			return m_iIndex;
		}
	}
	
	// Initializes the navmesh with data.
	// Params:
	//	data - (in) Pointer to navmesh data.
	//	dataSize - (in) size of the navmesh data.
	//	ownsData - (in) Flag indicating if the navmesh should own and delete the data.
	public bool init( byte[] data, int dataSize, bool ownsData, int iIndex )
	{
		m_iIndex = iIndex;
		m_header = new dtStatNavMeshHeader();
		
		MemoryStream fs = new MemoryStream (data);			
		BinaryReader readerStream = new BinaryReader(fs);	
		
		readerStream.BaseStream.Position = 0;
			
		int iFiledataSize = readerStream.ReadInt32() + 4;		
		m_header.magic = readerStream.ReadInt32();
		m_header.version = readerStream.ReadInt32();
		
		if( iFiledataSize != dataSize )			
		{
			Debug.LogError("DetourStatNavMesh::init() [ dataSize != dataSize ]file size : " + iFiledataSize + " data Size :" + dataSize);
			return false;
		}
		
		if( m_header.magic != 1312904781 )
		{
			Debug.LogError("DetourStatNavMesh::init() [ magic diff ]");
			return false;
		}
		
		if( m_header.version != 3 )
		{
			Debug.LogError("DetourStatNavMesh::init() [ version diff ]");
			return false;
		}
		
		m_header.npolys = readerStream.ReadInt32();
		m_header.nverts = readerStream.ReadInt32();
		m_header.nnodes = readerStream.ReadInt32();
		m_header.ndmeshes = readerStream.ReadInt32();
		m_header.ndverts = readerStream.ReadInt32();
		m_header.ndtris = readerStream.ReadInt32();
		m_header.cs = readerStream.ReadSingle();
		
		m_header.bmin[0] = readerStream.ReadSingle();
		m_header.bmin[1] = readerStream.ReadSingle();
		m_header.bmin[2] = readerStream.ReadSingle();		
		m_header.bmax[0] = readerStream.ReadSingle();
		m_header.bmax[1] = readerStream.ReadSingle();
		m_header.bmax[2] = readerStream.ReadSingle();
		
		
		int headerSize = dtStatNavMeshHeader.size+4; 
		int vertsSize = m_header.nverts;
		int polysSize = m_header.npolys;
		int nodesSize = m_header.npolys*2;
		int detailMeshesSize = m_header.ndmeshes;
		int detailVertsSize = m_header.ndverts;
		int detailTrisSize = m_header.ndtris;
		
		//------
		readerStream.BaseStream.Position = headerSize;
		m_header.verts = new Vector3[vertsSize];
		for( int i=0; i<vertsSize; ++i )
		{
			m_header.verts[i].x = readerStream.ReadSingle();
			m_header.verts[i].y = readerStream.ReadSingle();
			m_header.verts[i].z = readerStream.ReadSingle();
		}		
		
		//------
		m_header.polys = new dtStatPoly[polysSize];
		for( int i=0; i<polysSize; ++i )
		{
			m_header.polys[i] = new dtStatPoly();
			for( int k=0; k<DT_STAT_VERTS_PER_POLYGON; ++k )
			{
				m_header.polys[i].v[k] = readerStream.ReadUInt16();
			}
			
			for( int k=0; k<DT_STAT_VERTS_PER_POLYGON; ++k )
			{
				m_header.polys[i].n[k] = readerStream.ReadUInt16();
			}
			
			m_header.polys[i].nv = readerStream.ReadByte();
			m_header.polys[i].flags = readerStream.ReadByte();
		}		
		
		//------
		m_header.bvtree = new dtStatBVNode[nodesSize];
		for( int i=0; i<nodesSize; ++i )
		{
			m_header.bvtree[i] = new dtStatBVNode();
			for( int k=0; k<3; ++k )
			{
				m_header.bvtree[i].bmin[k] = readerStream.ReadUInt16();
			}
			
			for( int k=0; k<3; ++k )
			{
				m_header.bvtree[i].bmax[k] = readerStream.ReadUInt16();
			}
			
			m_header.bvtree[i].i = readerStream.ReadInt32();
		}
		
		
		//------
		m_header.dmeshes = new dtStatPolyDetail[detailMeshesSize];
		for( int i=0; i<detailMeshesSize; ++i )
		{
			m_header.dmeshes[i] = new dtStatPolyDetail();		
			m_header.dmeshes[i].vbase = readerStream.ReadUInt16();
			m_header.dmeshes[i].nverts = readerStream.ReadUInt16();
			m_header.dmeshes[i].tbase = readerStream.ReadUInt16();
			m_header.dmeshes[i].ntris = readerStream.ReadUInt16();
		}
		
		
		m_header.dverts = new Vector3[detailVertsSize];
		for( int i=0; i<detailVertsSize; ++i )
		{
			m_header.dverts[i].x = readerStream.ReadSingle();
			m_header.dverts[i].y = readerStream.ReadSingle();
			m_header.dverts[i].z = readerStream.ReadSingle();
		}
		
		m_header.dtris = new dtTriIdx[detailTrisSize];
		for( int i=0; i<detailTrisSize; ++i )
		{
			m_header.dtris[i] = new dtTriIdx();
			m_header.dtris[i].datas[0] = readerStream.ReadByte();
			m_header.dtris[i].datas[1] = readerStream.ReadByte();
			m_header.dtris[i].datas[2] = readerStream.ReadByte();
			m_header.dtris[i].datas[3] = readerStream.ReadByte();
		}
		
		m_nodePool = new dtNodePool(2048, 256);		
		m_openList = new dtNodeQueue(2048);
		
		
		/*if( ownsData )
		{
			m_data = data;
			m_dataSize = dataSize;
		}*/

		return true;
	}
	
	
	// Finds the nearest navigation polygon around the center location.
	// Params:
	//	center - (in) The center of the search box.
	//	extents - (in) The extents of the search box.
	// Returns: Reference identifier for the polygon, or 0 if no polygons found.
	public ushort findNearestPoly( Vector3 center, Vector3 extents )
	{		
		if (null == m_header) 
			return 0;
		
		// Get nearby polygons from proximity grid.
		ushort[] polys = new ushort[128];
		int npolys = queryPolygons(center, extents, polys, 128);
	
		// Find nearest polygon amongst the nearby polygons.
		ushort nearest = 0;
		float nearestDistanceSqr = FLT_MAX;
		for (int i = 0; i < npolys; ++i)
		{
			ushort _ref = polys[i];
			Vector3 closest = new Vector3();
			if (!closestPointToPoly(_ref, center, ref closest))
				continue;			
			
			float d = vdistSqr(center, closest);
			if (d < nearestDistanceSqr)
			{
				nearestDistanceSqr = d;
				nearest = _ref;
			}
		}
	
		return nearest;
	}
	
	public bool IsCell( Vector3 center, Vector3 extents )
	{
		if (null == m_header) 
			return false;

		// Get nearby polygons from proximity grid.
		ushort[] polys = new ushort[128];
		int npolys = queryPolygons(center, extents, polys, 128);
	
		Vector3 tcenter = center;		
		tcenter.y = 0.0f;
	
		// Find nearest polygon amongst the nearby polygons.
		ushort nearest = 0;
		float nearestDistanceSqr = 0.1f; // 오차범위
		for (int i = 0; i < npolys; ++i)
		{
			ushort _ref = polys[i];
			Vector3 closest = new Vector3();
			if (!closestPointToPoly2D(_ref, center, ref closest)) // y좌표를 0으로 만들어 y축에 대한 오차의 범위를 0으로 만듬
				continue;
	
			closest.y = 0.0f;
			float d = vdistSqr(tcenter, closest);
			if (d < nearestDistanceSqr)
			{
				nearestDistanceSqr = d;
				nearest = _ref;				
				return true;
			}
		}
	
		if( 0 != nearest )
			return true;
	
		return false;
	}
	
	
	/*public bool IsCell( Vector3 center, Vector3 extents )
	{
		if (null == m_header) 
			return false;

		// Get nearby polygons from proximity grid.
		ushort[] polys = new ushort[128];
		int npolys = queryPolygons(center, extents, polys, 128);
	
		Vector3 tcenter = center;		
		tcenter.y = 0.0f;
	
		// Find nearest polygon amongst the nearby polygons.
		ushort nearest = 0;
		float nearestDistanceSqr = 0.1f; // 오차범위
		for (int i = 0; i < npolys; ++i)
		{
			ushort _ref = polys[i];		
			if ( true == IsClosestPointToPoly2D(_ref, center)) 
				return true;				
		}		
	
		return false;
	}*/
	
	
	// Returns polygons which touch the query box.
	// Params:
	//	center - (in) the center of the search box.
	//	extents - (in) the extents of the search box.
	//	polys - (out) array holding the search result.
	//	maxPolys - (in) The max number of polygons the polys array can hold.
	// Returns: Number of polygons in search result array.
	public int queryPolygons(Vector3 center, Vector3 extents, ushort[] polys, int maxIds)
	{	
		if (null == m_header) 
			return 0;		
	
		// Calculate quantized box
		float ics = 1.0f / m_header.cs;
		ushort []bmin = new ushort[3];
		ushort []bmax = new ushort[3];
		
		// Clamp query box to world box.
		float minx = Mathf.Clamp(center[0] - extents[0], m_header.bmin[0], m_header.bmax[0]) - m_header.bmin[0];
		float miny = Mathf.Clamp(center[1] - extents[1], m_header.bmin[1], m_header.bmax[1]) - m_header.bmin[1];
		float minz = Mathf.Clamp(center[2] - extents[2], m_header.bmin[2], m_header.bmax[2]) - m_header.bmin[2];
		float maxx = Mathf.Clamp(center[0] + extents[0], m_header.bmin[0], m_header.bmax[0]) - m_header.bmin[0];
		float maxy = Mathf.Clamp(center[1] + extents[1], m_header.bmin[1], m_header.bmax[1]) - m_header.bmin[1];
		float maxz = Mathf.Clamp(center[2] + extents[2], m_header.bmin[2], m_header.bmax[2]) - m_header.bmin[2];
		// Quantize
		bmin[0] = (ushort)((ushort)(ics * minx) & 0xfffe); 
		bmin[1] = (ushort)((ushort)(ics * miny) & 0xfffe);
		bmin[2] = (ushort)((ushort)(ics * minz) & 0xfffe);
		bmax[0] = (ushort)((ushort)(ics * maxx + 1) | 1);
		bmax[1] = (ushort)((ushort)(ics * maxy + 1) | 1);
		bmax[2] = (ushort)((ushort)(ics * maxz + 1) | 1);	
		
		
		int nodeIndex = 0;
		
		int n = 0;
		while (nodeIndex < m_header.nnodes)
		{
			if( nodeIndex < 0 )
			{
				Debug.LogError("DetourStatMesh::queryPolygons()[ nodeIndex < 0 } nodeIndex : " + nodeIndex );
				break;
			}
			dtStatBVNode node = m_header.bvtree[nodeIndex];
			
			bool overlap = DetourComm.checkOverlapBox(bmin, bmax, node.bmin, node.bmax);
			bool isLeafNode = node.i >= 0;
			
	 		if (isLeafNode && overlap)
			{
				if (n < maxIds)
				{
					polys[n] = (ushort)node.i;
					n++;
				}
			}
			
			if (overlap || isLeafNode)
				nodeIndex++;
			else
			{
				int escapeIndex = -node.i;
				nodeIndex += escapeIndex;
			}
		}
		
		return n;
	}
	
	
	
	
	// Finds path from start polygon to end polygon.
	// If target polygon canno be reached through the navigation graph,
	// the last node on the array is nearest node to the end polygon.
	// Params:
	//	startRef - (in) ref to path start polygon.
	//	endRef - (in) ref to path end polygon.
	//	path - (out) array holding the search result.
	//	maxPathSize - (in) The max number of polygons the path array can hold.
	// Returns: Number of polygons in search result array.
	public int findPath(ushort startRef, ushort endRef, Vector3 startPos, Vector3 endPos, ushort[] path, int maxPathSize)
	{
		if (null == m_header) 
			return 0;
		
		if ( 0 == startRef || 0 == endRef)
			return 0;
	
		if (0 == maxPathSize)
			return 0;
	
		if (startRef == endRef) 
		{	
			path[0] = startRef;
			return 1;
		}
	
		m_nodePool.clear();
		m_openList.clear();
	
		const float H_SCALE = 1.1f;	// Heuristic scale.
		
		dtNode startNode = m_nodePool.getNode(startRef);
		startNode.pidx = 0;
		startNode.cost = 0;
		startNode.total = vdist(startPos, endPos) * H_SCALE;
		startNode.id = startRef;
		startNode.flags = (uint)dtNodeFlags.DT_NODE_OPEN;
		m_openList.push(startNode);
	
		dtNode lastBestNode = startNode;
		float lastBestNodeCost = startNode.total;
		while (!m_openList.empty())
		{
			dtNode bestNode = m_openList.pop();
		
			if (bestNode.id == endRef)
			{
				lastBestNode = bestNode;
				break;
			}
	
			dtStatPoly poly = getPoly((int)bestNode.id-1);
			for (int i = 0; i < (int)poly.nv; ++i)
			{
				ushort neighbour = poly.n[i]; 
				if ( 0!=neighbour)
				{
					// Skip parent node.
					if ( 0!=bestNode.pidx && m_nodePool.getNodeAtIdx(bestNode.pidx).id == neighbour)
						continue;
	
					dtNode parent = bestNode;
					dtNode newNode = new dtNode();
					newNode.pidx = m_nodePool.getNodeIdx(parent);
					newNode.id = neighbour;
	
					// Calculate cost.
					//float p0[3], p1[3];
					Vector3 p0 = new Vector3();
					Vector3 p1 = new Vector3();
					if (0==parent.pidx)						
						p0 = startPos;
					else
						getEdgeMidPoint(m_nodePool.getNodeAtIdx(parent.pidx).id, parent.id, ref p0);
					getEdgeMidPoint(parent.id, newNode.id, ref p1);
					newNode.cost = parent.cost + vdist(p0,p1);
					// Special case for last node.
					if (newNode.id == endRef)
						newNode.cost += vdist(p1, endPos);
					
					// Heuristic
					float h = vdist(p1,endPos)*H_SCALE;
					newNode.total = newNode.cost + h;
					
					dtNode actualNode = m_nodePool.getNode(newNode.id);
					if (null == actualNode)
						continue;
							
					if (!( (0!=(actualNode.flags & (uint)dtNodeFlags.DT_NODE_OPEN)) && newNode.total > actualNode.total) &&
						!( (0!=(actualNode.flags & (uint)dtNodeFlags.DT_NODE_CLOSED)) && newNode.total > actualNode.total))
					{
						actualNode.flags &= ~ ((uint)dtNodeFlags.DT_NODE_CLOSED);
						actualNode.pidx = newNode.pidx;
						actualNode.cost = newNode.cost;
						actualNode.total = newNode.total;
	
						if (h < lastBestNodeCost)
						{
							lastBestNodeCost = h;
							lastBestNode = actualNode;
						}
	
						if ( 0 != (actualNode.flags & (uint)dtNodeFlags.DT_NODE_OPEN) )
						{
							m_openList.modify(actualNode);
						}
						else
						{
							actualNode.flags |= (uint)dtNodeFlags.DT_NODE_OPEN;
							m_openList.push(actualNode);
						}
					}
				}
			}
			bestNode.flags |= (uint)dtNodeFlags.DT_NODE_CLOSED;
		}
	
		// Reverse the path.
		dtNode prev = null;
		dtNode node = lastBestNode;
		do
		{
			dtNode next = m_nodePool.getNodeAtIdx(node.pidx);
			node.pidx = m_nodePool.getNodeIdx(prev);
			prev = node;
			node = next;
		}
		while ( null != node );
		
		// Store path
		node = prev;
		int n = 0; 
		do
		{
			path[n++] = (ushort)node.id;
			node = m_nodePool.getNodeAtIdx(node.pidx);
		}
		while ( null != node && n < maxPathSize);
	
		return n;
	}
	
	// Finds a straight path from start to end locations within the corridor
	// described by the path polygons.
	// Start and end locations will be clamped on the corridor.
	// Params:
	//	startPos - (in) Path start location.
	//	endPos - (in) Path end location.
	//	path - (in) Array of connected polygons describing the corridor.
	//	pathSize - (in) Number of polygons in path array.
	//	straightPath - (out) Points describing the straight path.
	//	maxStraightPathSize - (in) The max number of points the straight path array can hold.
	// Returns: Number of points in the path.	
	public  int findStraightPath( Vector3 startPos, Vector3 endPos, ushort[] path, int pathSize, Vector3[] straightPath, int maxStraightPathSize)
	{ 
		if (null==m_header) 
			return 0;
		
		if (0 == maxStraightPathSize)
			return 0;
	
		if (0 == path[0])
			return 0;
	
		int straightPathSize = 0;
		
		//float closestStartPos[3];
		Vector3 closestStartPos = new Vector3();
		if (!closestPointToPoly(path[0], startPos, ref closestStartPos))
			return 0;
	
		// Add start point.		
		straightPath[straightPathSize] = closestStartPos;
		straightPathSize++;
		if (straightPathSize >= maxStraightPathSize)
			return straightPathSize;
	
		Vector3 closestEndPos = new Vector3();		
		if (!closestPointToPoly(path[pathSize-1], endPos, ref closestEndPos))
			return 0;
	
		//float portalApex[3], portalLeft[3], portalRight[3];
		Vector3 portalApex = new Vector3();
		Vector3 portalLeft = new Vector3();
		Vector3 portalRight = new Vector3();
	
		if (pathSize > 1)
		{			
			portalApex = closestStartPos;
			portalLeft = portalApex;
			portalRight = portalApex;			
			int apexIndex = 0;
			int leftIndex = 0;
			int rightIndex = 0;
	
			for (int i = 0; i < pathSize; ++i)
			{				
				Vector3 left = new Vector3();
				Vector3 right = new Vector3();
				if (i < pathSize-1)
				{
					// Next portal.
					getPortalPoints(path[i], path[i+1], ref left, ref right);
				}
				else
				{
					// End of the path.				
					left= closestEndPos;
					right= closestEndPos;
				}
	
				// Right vertex.
				if (vequal(portalApex, portalRight))
				{					
					portalRight = right;
					rightIndex = i;
				}
				else
				{
					if (DetourComm.triArea2D(portalApex, portalRight, right) <= 0.0f)
					{
						if (DetourComm.triArea2D(portalApex, portalLeft, right) > 0.0f)
						{							
							portalRight= right;
							rightIndex = i;
						}
						else
						{						
							portalApex= portalLeft;
							apexIndex = leftIndex;
	
							if (!vequal( straightPath[(straightPathSize-1)], portalApex))
							{								
								straightPath[straightPathSize] = portalApex;
									
								straightPathSize++;
								if (straightPathSize >= maxStraightPathSize)
									return straightPathSize;
							}	
							
							portalLeft= portalApex;
							portalRight= portalApex;
								
							leftIndex = apexIndex;
							rightIndex = apexIndex;
	
							// Restart
							i = apexIndex;
	
							continue;
						}
					}
				}
	
				// Left vertex.
				if (vequal(portalApex, portalLeft))
				{					
					portalLeft= left;
					leftIndex = i;
				}
				else
				{
					if (DetourComm.triArea2D(portalApex, portalLeft, left) >= 0.0f)
					{
						if (DetourComm.triArea2D(portalApex, portalRight, left) < 0.0f)
						{							
							portalLeft= left;
							leftIndex = i;
						}
						else
						{							
							portalApex= portalRight;
							apexIndex = rightIndex;
	
							if (!vequal( straightPath[(straightPathSize-1)], portalApex))
							{								
								straightPath[straightPathSize]= portalApex;
								straightPathSize++;
								if (straightPathSize >= maxStraightPathSize)
									return straightPathSize;
							}
								
							portalLeft= portalApex;
							portalRight= portalApex;
								
							leftIndex = apexIndex;
							rightIndex = apexIndex;
	
							// Restart
							i = apexIndex;
	
							continue;
						}
					}
				}
			}
		}
	
		// Add end point.	
		straightPath[straightPathSize]= closestEndPos;
		straightPathSize++;
		
		return straightPathSize;
	}
	
	
	
	// Finds intersection againts walls starting from start pos.
	// Params:
	//	startRef - (in) ref to the polygon where the start lies.
	//	startPos - (in) start position of the query.
	//	endPos - (in) end position of the query.
	//	t - (out) hit parameter along the segment, 0 if no hit.
	//	endRef - (out) ref to the last polygon which was processed.
	// Returns: Number of polygons in path or 0 if failed.
	public int raycast( ushort centerRef, Vector3 startPos, Vector3 endPos, ref float t, ushort[] path, int pathSize )
	{
		if ( null == m_header) 
			return 0;
		
		if ( 0 == centerRef) 
			return 0;
		
		//ushort prevRef = centerRef;
		ushort curRef = centerRef;
		t = 0;
	
		Vector3 []verts = new Vector3[DT_STAT_VERTS_PER_POLYGON];
		int n = 0;		
		
	
		while (0 != curRef)
		{
			// Cast ray against current polygon.
			int nv = getPolyVerts(curRef, verts);
			if (nv < 3)
			{
				// Hit bad polygon, report hit.
				return n;
			}
			
			float tmin = new float();
			float tmax = new float();
			int segMin = new int();
			int segMax = new int();
			
			if (!DetourComm.intersectSegmentPoly2D(startPos, endPos, verts, nv, ref tmin, ref tmax, ref segMin, ref segMax))
			{
				// Could not a polygon, keep the old t and report hit.
				return n;
			}
			// Keep track of furthest t so far.
			if (tmax > t)
				t = tmax;
	
			if (n < pathSize)
				path[n++] = curRef;
	
			// Check the neighbour of this polygon.
			dtStatPoly poly = getPolyByRef(curRef);
			ushort nextRef = 0;
			if( segMax >= 0 && segMax < DT_STAT_VERTS_PER_POLYGON )
				nextRef = poly.n[segMax];			
			
			if ( 0 == nextRef)
			{
				// No neighbour, we hit a wall.
				return n;
			}
			
			// No hit, advance to neighbour polygon.
			//prevRef = curRef;
			curRef = nextRef;
		}
		
		return n;
	}
		
	
	
	// Returns closest point on navigation polygon.
	// Params:
	//	ref - (in) ref to the polygon.
	//	pos - (in) the point to check.
	//	closest - (out) closest point.
	// Returns: true if closest point found.	
	bool closestPointToPoly( ushort _ref, Vector3 pos, ref Vector3 closest)
	{
		int idx = getPolyIndexByRef(_ref);
		if (idx == -1)
			return false;
	
		float closestDistSqr = FLT_MAX;
		dtStatPoly p = getPoly(idx);
		dtStatPolyDetail pd = getPolyDetail(idx);
	
		for (int j = 0; j < pd.ntris; ++j)
		{
			dtTriIdx t = getDetailTri(pd.tbase+j);
			Vector3[] v = new Vector3[3];
			for (int k = 0; k < 3; ++k)
			{
				if (t.datas[k] < p.nv)
					v[k] = getVertex(p.v[t.datas[k]]);
				else
					v[k] = getDetailVertex(pd.vbase+(t.datas[k]-p.nv));
			}
			Vector3 pt = new Vector3();
			DetourComm.closestPtPointTriangle(ref pt, pos, v[0], v[1], v[2]);
			float d = vdistSqr(pos, pt);			
			if (d < closestDistSqr)
			{
				closest = pt;							
				closestDistSqr = d;
			}
		}
		
		return true;
	}
	
	
	bool closestPointToPoly2D( ushort _ref, Vector3 pos, ref Vector3 closest)
	{
		int idx = getPolyIndexByRef(_ref);
		if (idx == -1)
			return false;
		
		
		Vector3 tpos = pos;	
		tpos.y = 0.0f;
	
		float closestDistSqr = FLT_MAX;
		dtStatPoly p = getPoly(idx);
		dtStatPolyDetail pd = getPolyDetail(idx);
	
		for (int j = 0; j < pd.ntris; ++j)
		{
			dtTriIdx t = getDetailTri(pd.tbase+j);
			Vector3[] v = new Vector3[3];
			for (int k = 0; k < 3; ++k)
			{
				if (t.datas[k] < p.nv)
					v[k] = getVertex(p.v[t.datas[k]]);
				else
					v[k] = getDetailVertex(pd.vbase+(t.datas[k]-p.nv));
			}
			Vector3 pt = new Vector3();
			DetourComm.closestPtPointTriangle2D(ref pt, tpos, v[0], v[1], v[2]);
			float d = vdistSqr(tpos, pt);			
			if (d < closestDistSqr)
			{
				closest = pt;							
				closestDistSqr = d;
			}
		}
		
		return true;
	}
	
	
	
	bool IsClosestPointToPoly2D( ushort _ref, Vector3 pos )
	{
		int idx = getPolyIndexByRef(_ref);
		if (idx == -1)
			return false;		
		
		Vector3 tpos = pos;	
		tpos.y = 0.0f;
		
		dtStatPoly p = getPoly(idx);
		dtStatPolyDetail pd = getPolyDetail(idx);
	
		for (int j = 0; j < pd.ntris; ++j)
		{
			dtTriIdx t = getDetailTri(pd.tbase+j);
			Vector3[] v = new Vector3[3];
			for (int k = 0; k < 3; ++k)
			{
				if (t.datas[k] < p.nv)
					v[k] = getVertex(p.v[t.datas[k]]);
				else
					v[k] = getDetailVertex(pd.vbase+(t.datas[k]-p.nv));
			}
			
			if( true == DetourComm.IsClosestPtPointTriangle2D(tpos, v))
				return true;			
		}
		
		return false;
	}
	
	
	// Returns pointer to a polygon based on ref.
	public dtStatPoly getPolyByRef(ushort _ref)
	{
		if ( null == m_header || _ref == 0 || (int)_ref > m_header.npolys) 
			return null;
		
		return m_header.polys[_ref-1];
	}	
	
	// Returns polygon index based on ref, or -1 if failed.
	public int getPolyIndexByRef(ushort _ref)
	{
		if( null == m_header || _ref == 0 || (int)_ref > m_header.npolys) 
			return -1;
		
		return (int)_ref-1;
	}
	
	// Returns number of navigation polygons.
	public int getPolyCount()
	{ 
		return (null != m_header) ? m_header.npolys : 0; 
	}	
	// Rerturns pointer to specified navigation polygon.
	public dtStatPoly getPoly(int i)
	{ 
		return m_header.polys[i]; 
	}
	// Returns number of vertices.
	public int getVertexCount()
	{ 
		return (null != m_header) ? m_header.nverts : 0; 
	}
	// Returns pointer to specified vertex.
	public Vector3 getVertex(int i)
	{ 
		return m_header.verts[i]; 
	}
	// Returns number of navigation polygons details.
	public int getPolyDetailCount()
	{ 
		return (null != m_header) ? m_header.ndmeshes : 0; 
	}
	// Rerturns pointer to specified navigation polygon detail.
	public dtStatPolyDetail getPolyDetail(int i)
	{ 
		return m_header.dmeshes[i]; 
	}
	// Returns pointer to specified vertex.
	public Vector3 getDetailVertex(int i)
	{ 
		return m_header.dverts[i]; 
	}	
	// Returns pointer to specified vertex.
	public dtTriIdx getDetailTri(int i) 
	{ 
		return m_header.dtris[i]; 
	}
	
	
	
	//----------------------------------------------------------------------------------------------------	
	float vdist(Vector3 v1, Vector3 v2)
	{		
		return (v1 - v2).magnitude;
	}
	
	float vdistSqr(Vector3 v1, Vector3 v2)
	{		
		return (v1 - v2).sqrMagnitude;
	}
	
	
	
	// Copies the locations of vertices of a polygon to an array.
	public int getPolyVerts(ushort _ref, Vector3[] verts)
	{
		if (null==m_header) 
			return 0;
		
		dtStatPoly poly = getPolyByRef(_ref);
		if (null==poly) 
			return 0;
		
		for (int i = 0; i < (int)poly.nv; ++i)
		{
			Vector3 cv = m_header.verts[poly.v[i]];
			verts[i].x = cv.x;
			verts[i].y = cv.y;
			verts[i].z = cv.z;
		}
		return (int)poly.nv;
	}
	
	// Returns portal points between two polygons.
	public bool getPortalPoints(ushort _from, ushort to, ref Vector3 left, ref Vector3 right)
	{
		dtStatPoly fromPoly = getPolyByRef(_from);
		if (null == fromPoly)
			return false;
	
		// Find common edge between the polygons and returns the segment end points.
		for (int i = 0, j = (int)fromPoly.nv - 1; i < (int)fromPoly.nv; j = i++)
		{
			ushort neighbour = fromPoly.n[j];
			if (neighbour == to)
			{
				Vector3 leftTemp = getVertex(fromPoly.v[j]);
				Vector3 rightTemp = getVertex(fromPoly.v[i]);
				
				left.x = leftTemp.x;
				left.y = leftTemp.y;
				left.z = leftTemp.z;
				
				right.x = rightTemp.x;
				right.y = rightTemp.y;
				right.z = rightTemp.z;
				
				return true;
			}
		}
	
		return false;
	}
	
	
	
	// Returns edge mid point between two polygons.
	public bool getEdgeMidPoint(ushort _from, ushort to, ref Vector3 mid)
	{
		Vector3 left = new Vector3();
		Vector3 right = new Vector3();
		
		if (!getPortalPoints(_from, to, ref left,ref right)) 
			return false;
		
		mid.x = (left.x+right.x)*0.5f;
		mid.y = (left.y+right.y)*0.5f;
		mid.z = (left.z+right.z)*0.5f;
		return true;
	}
	
	public bool getEdgeMidPoint(uint _from, uint to, ref Vector3 mid)
	{
		return getEdgeMidPoint( (ushort)_from, (ushort)to, ref mid );
	}	
	
	
	bool vequal( Vector3 p0, Vector3 p1)
	{		
		float thr = Mathf.Sqrt(1.0f/16384.0f);
		
		float d = vdistSqr(p0, p1);
		return d < thr;
	}	
}
