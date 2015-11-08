using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsTraceViewer : MonoBehaviour {
	
	LineRenderer m_LineRenderer;
	
	public Color c1 = Color.yellow;
	public Color c2 = Color.blue;
	
	public float frame_ = 0.03f;

	public float m_ReleaseBegin = 1.5f;
	public float m_ReleaseInterval = 0.005f;
	bool m_BeginBufferRelease = false;
	bool m_ButtonPressed = false;

	LinkedList<Vector3> m_listPos = new LinkedList<Vector3>();
	List<GameObject> m_listObj = new List<GameObject>();
	
	void Awake()
	{
		m_LineRenderer = gameObject.AddComponent<LineRenderer>();
		m_LineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		m_LineRenderer.SetColors(c1, c2);
		m_LineRenderer.SetWidth(0.05f, 0.05f);
		BoxCollider col = gameObject.AddComponent<BoxCollider>();
		col.size = new Vector3(10, 10, 0.1f);
		col.center = new Vector3(0, 0, 1);
		
		StartCoroutine("UpdateByFrame");
		
//		InvokeRepeating("ReleaseTraceInSequence" , 0, m_ReleaseInterval);
	}
	
	IEnumerator UpdateByFrame () {
		
		while(true)
		{
			yield return new WaitForSeconds(frame_);
			
			if(m_ButtonPressed == true)
			{
				Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit) == true)
				{
					m_listPos.AddLast(hit.point);
					
					GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
					m_listObj.Add(obj);
					obj.name = m_listPos.Count.ToString();
					obj.transform.position = hit.point;
					obj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
				}
			}
			
			m_LineRenderer.SetVertexCount(m_listPos.Count);
			
			int i=0;
			foreach(Vector3 pos in m_listPos)
			{
				m_LineRenderer.SetPosition(i, pos);
				++i;
			}
		}
	}
		
	void OnMouseDown()
	{
		m_BeginBufferRelease = false;
		m_listPos.Clear();
		foreach(GameObject obj in m_listObj) Destroy(obj); m_listObj.Clear();
		
//		yield return new WaitForSeconds(m_ReleaseBegin);
		m_BeginBufferRelease = true;
		
		m_ButtonPressed = true;
	}
	
//	void OnMouseDrag()
//	{
//		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
//		RaycastHit hit;
//		if(Physics.Raycast(ray, out hit) == true)
//		{
//			m_listPos.AddLast(hit.point);
//			
//			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			m_listObj.Add(obj);
//			obj.name = m_listPos.Count.ToString();
//			obj.transform.position = hit.point;
//			obj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
//		}
//	}
	
	void OnMouseUp()
	{
		m_ButtonPressed = false;
	}
	
	void ReleaseTraceInSequence()
	{
		if(m_BeginBufferRelease == true)
			m_listPos.RemoveFirst();
	}
}
