using UnityEngine;
using System.Collections;

public class DetourComm 
{
	public static bool checkOverlapBox(ushort[] amin, ushort[] amax, ushort[] bmin, ushort[] bmax)
	{
		bool overlap = true;
		overlap = (amin[0] > bmax[0] || amax[0] < bmin[0]) ? false : overlap;
		overlap = (amin[1] > bmax[1] || amax[1] < bmin[1]) ? false : overlap;
		overlap = (amin[2] > bmax[2] || amax[2] < bmin[2]) ? false : overlap;
		return overlap;
	}
	
	public static void closestPtPointTriangle2D(ref Vector3 closest, Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 tp = p;
		Vector3 ta = a;
		Vector3 tb = b;
		Vector3 tc = c;		
	 
		tp.y = 0.0f;
		ta.y = 0.0f;
		tb.y = 0.0f;
		tc.y = 0.0f;
	
		closestPtPointTriangle( ref closest, tp, ta, tb, tc );
	}
	
	
	public static float Vec2Dot(Vector3 vec1, Vector3 vec2)
	{
		return vec1.x*vec2.x+vec1.z*vec2.z;
	}
	
	public static void Vec2Cross( ref Vector3 outVec, Vector3 inVec)
	{
		outVec.x = -inVec.z;
		outVec.z = inVec.x;
	}
	
	public static bool IsClosestPtPointTriangle2D( Vector3 PointPos, Vector3[] TrianglePos )
	{
		Vector3 Nvec = Vector3.zero;

		float a,b;
		
		Vector3 []vec2 = new Vector3[3];
	
		vec2[0].x = TrianglePos[2].x - TrianglePos[0].x;
		vec2[0].z = TrianglePos[2].z - TrianglePos[0].z;
		vec2[1].x = TrianglePos[1].x - TrianglePos[0].x;
		vec2[1].z = TrianglePos[1].z - TrianglePos[0].z;
		vec2[2].x = PointPos.x - TrianglePos[0].x;
		vec2[2].z = PointPos.z - TrianglePos[0].z;
	
		
		Vec2Cross(ref Nvec,vec2[0]);
		a = Vec2Dot(Nvec,vec2[1]);
		b = Vec2Dot(Nvec,vec2[2]);
		if( a*b < 0)
			return false;
	
		vec2[0].x = TrianglePos[0].x - TrianglePos[1].x;
		vec2[0].z = TrianglePos[0].z - TrianglePos[1].z;
		vec2[1].x = TrianglePos[2].x - TrianglePos[1].x;
		vec2[1].z = TrianglePos[2].z - TrianglePos[1].z;
		vec2[2].x = PointPos.x - TrianglePos[1].x;
		vec2[2].z = PointPos.z - TrianglePos[1].z;
		Vec2Cross(ref Nvec,vec2[0]);
		a = Vec2Dot(Nvec,vec2[1]);
		b = Vec2Dot(Nvec,vec2[2]);
		if( a*b < 0)
			return false;
	
		vec2[0].x = TrianglePos[1].x - TrianglePos[2].x;
		vec2[0].z = TrianglePos[1].z - TrianglePos[2].z;
		vec2[1].x = TrianglePos[0].x - TrianglePos[2].x;
		vec2[1].z = TrianglePos[0].z - TrianglePos[2].z;
		vec2[2].x = PointPos.x - TrianglePos[2].x;
		vec2[2].z = PointPos.z - TrianglePos[2].z;
		Vec2Cross(ref Nvec,vec2[0]);
		a = Vec2Dot(Nvec,vec2[1]);
		b = Vec2Dot(Nvec,vec2[2]);
		if( a*b < 0)
			return false;
		
		return true;
	}
	
	public static void closestPtPointTriangle( ref Vector3 closest, Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		// Check if P in vertex region outside A		
		Vector3 ab = b - a; 
		Vector3 ac = c - a; 
		Vector3 ap = p - a; 			
		float d1 = Vector3.Dot(ab, ap);
		float d2 = Vector3.Dot(ac, ap);
		
		if (d1 <= 0.0f && d2 <= 0.0f)
		{
			// barycentric coordinates (1,0,0)
			closest = a;			
			return;
		}
		
		// Check if P in vertex region outside B
		
		Vector3 bp = p - b;			
		float d3 = Vector3.Dot(ab, bp);
		float d4 = Vector3.Dot(ac, bp);
		if (d3 >= 0.0f && d4 <= d3)
		{
			// barycentric coordinates (0,1,0)			
			closest = b;			
			return;
		}
		
		// Check if P in edge region of AB, if so return projection of P onto AB
		float vc = d1*d4 - d3*d2;
		if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
		{
			// barycentric coordinates (1-v,v,0)
			float v = d1 / (d1 - d3);
			closest.x = a.x + v * ab.x;
			closest.y = a.y + v * ab.y;
			closest.z = a.z + v * ab.z;
			return;
		}
		
		// Check if P in vertex region outside C		
		Vector3 cp = p - c;
		float d5 = Vector3.Dot(ab, cp);
		float d6 = Vector3.Dot(ac, cp);
		if (d6 >= 0.0f && d5 <= d6)
		{
			// barycentric coordinates (0,0,1)			
			closest = c;		
			return;
		}
		
		// Check if P in edge region of AC, if so return projection of P onto AC
		float vb = d5*d2 - d1*d6;
		if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
		{
			// barycentric coordinates (1-w,0,w)
			float w = d2 / (d2 - d6);
			closest.x = a.x + w * ac.x;
			closest.y = a.y + w * ac.y;
			closest.z = a.z + w * ac.z;
			return;
		}
		
		// Check if P in edge region of BC, if so return projection of P onto BC
		float va = d3*d6 - d5*d4;
		if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
		{
			// barycentric coordinates (0,1-w,w)
			float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
			closest.x = b.x + w * (c.x - b.x);
			closest.y = b.y + w * (c.y - b.y);
			closest.z = b.z + w * (c.z - b.z);
			return;
		}
		
		// P inside face region. Compute Q through its barycentric coordinates (u,v,w)
		float denom = 1.0f / (va + vb + vc);
		float vv = vb * denom;
		float ww = vc * denom;
		closest.x = a.x + ab.x * vv + ac.x * ww;
		closest.y = a.y + ab.y * vv + ac.y * ww;
		closest.z = a.z + ab.z * vv + ac.z * ww;
	}
	
	public static float triArea2D(Vector3 a, Vector3 b, Vector3 c)
	{
		return ((b.x*a.z - a.x*b.z) + (c.x*b.z - b.x*c.z) + (a.x*c.z - c.x*a.z)) * 0.5f;
	}
	
	public static bool intersectSegmentPoly2D( Vector3 p0, Vector3 p1, Vector3[] verts, int nverts, ref float tmin, ref float tmax, ref int segMin, ref int segMax)
	{
		float EPS = 0.00000001f;
		
		tmin = 0;
		tmax = 1;
		segMin = -1;
		segMax = -1;
		
		Vector3 dir;
		//vsub(dir, p1, p0);
		dir = p1 - p0;
		
		for (int i = 0, j = nverts-1; i < nverts; j=i++)
		{
			Vector3 edge = verts[i] - verts[j];
			Vector3 diff = p0 - verts[j];			
			float n = vperp2D(edge, diff);  
			float d = -vperp2D(edge, dir);
			if (Mathf.Abs(d) < EPS)
			{
				// S is nearly parallel to this edge
				if (n < 0)
					return false;
				else
					continue;
			}
			float t = n / d;
			if (d < 0)
			{
				// segment S is entering across this edge
				if (t > tmin)
				{
					tmin = t;
					segMin = j;
					// S enters after leaving polygon
					if (tmin > tmax)
						return false;
				}
			}
			else
			{
				// segment S is leaving across this edge
				if (t < tmax)
				{
					tmax = t;
					segMax = j;
					// S leaves before entering polygon
					if (tmax < tmin)
						return false;
				}
			}
		}
		
		return true;
	}
	
	
	public static float vperp2D(Vector3 u, Vector3 v)
	{
		return u.z*v.x - u.x*v.z;
	}
	
}
