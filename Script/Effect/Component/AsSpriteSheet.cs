using UnityEngine;
using System.Collections;
//This script animates a texture containing tiles of an animation

public class AsSpriteSheet : MonoBehaviour 
{
	public bool m_random = false;
	public int m_uvTieX = 1;
    public int m_uvTieY = 1;
    public int m_fps = 10;
    
    private Vector2 m_size;  
    private int m_lastIndex = -1;

	private float m_changeTime = 0f;
    void Start () 
    {
        m_size = new Vector2 (1.0f / m_uvTieX , 1.0f / m_uvTieY);
     
        if(renderer == null)
            enabled = false;
    }
    // Update is called once per frame
	
	void SetSpriteSheet(int index)
	{
 		 // split into horizontal and vertical index
        int uIndex = index % m_uvTieX;
        int vIndex = index / m_uvTieX;
  
        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        Vector2 offset = new Vector2 (uIndex * m_size.x, 1.0f - m_size.y - vIndex * m_size.y);
        
        renderer.material.SetTextureOffset ("_MainTex", offset);
        renderer.material.SetTextureScale ("_MainTex", m_size);
        
        m_lastIndex = index;
	}
    void Update()
    {
		
	
		if(m_random)
		{		
	     
	        if(Time.time -  m_changeTime > 1f / m_fps )
	        {
				m_changeTime = Time.time;
				// random index
			 	int index =  UnityEngine.Random.Range(0, m_uvTieX * m_uvTieY);				
	            SetSpriteSheet(index);
	        }
		
		}
		else
		{
			// Calculate index
	        int index = (int)(Time.time * m_fps) % (m_uvTieX * m_uvTieY);
	        if(index != m_lastIndex)
	        {				
	             SetSpriteSheet(index);
      	 	}
		}
    
    }


}
