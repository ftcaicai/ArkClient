using UnityEngine;
using System.Collections;

public class TerrainAlphaMapTest : MonoBehaviour 
{
	private TerrainData m_terrainData = null;
	private Vector3 m_vec3TerrainPos = Vector3.zero;
	private string m_strResultText;
	
	
	private float m_fTextureIndex1 = 0.0f;
	private float m_fTextureIndex2 = 0.0f;
	
	
	// Use this for initialization
	void Start () 
	{
		m_terrainData = Terrain.activeTerrain.terrainData;
		m_vec3TerrainPos = Terrain.activeTerrain.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		int mapX = Mathf.RoundToInt(((transform.position.x - m_vec3TerrainPos.x) / m_terrainData.size.x) * m_terrainData.alphamapWidth);
		int mapZ = Mathf.RoundToInt(((transform.position.z - m_vec3TerrainPos.z) / m_terrainData.size.z) * m_terrainData.alphamapHeight);
 
		float[,,] splatmapData = m_terrainData.GetAlphamaps(mapX, mapZ, 1, 1); 
 
		m_fTextureIndex1 = splatmapData[0,0,0]; //grass texture
		m_fTextureIndex2 = splatmapData[0,0,1]; //sand texture
 
		m_strResultText = " grass  : "+m_fTextureIndex1+"\n sand : "+m_fTextureIndex2;
		
		Debug.Log( m_strResultText );
	}
}
