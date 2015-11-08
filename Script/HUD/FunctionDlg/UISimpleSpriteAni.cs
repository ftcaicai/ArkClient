using UnityEngine;
using System.Collections;

public class UISimpleSpriteAni : MonoBehaviour 
{
	public SimpleSprite[] sprites;
	public float speed = 0.5f;
    public bool  loop  = false;
	
	private bool m_isEnd = false;
	private int m_iCurIndex = 0;
	private float m_fTime = 2.0f;
	
	public static UISimpleSpriteAni Create( string strPath, Transform trs, Vector3 pos )
	{
		if( null == trs )
		{
			Debug.LogError("UISimpleSpriteAni::Create() [ null == transform ] ");
			return null;
		}
		
		GameObject goIns =  ResourceLoad.CreateGameObject( strPath, trs ); 
		UISimpleSpriteAni spriteTemp = goIns.GetComponent<UISimpleSpriteAni>();
		if( null == spriteTemp )
		{
			Destroy(goIns);
			Debug.LogError("UISimpleSpriteAni::Create() [null == UISimpleSpriteAni]");
			return null;
		}
		
		spriteTemp.gameObject.transform.localPosition = pos;		
		spriteTemp.Play();
		
		return spriteTemp;
		
	}
	
	public float GetEndTime()
	{
		return sprites.Length * speed;
	}
	
	
	public bool isEnd
	{
		get
		{
			return m_isEnd;
		}
	}

    void OnEnable()
    {
        if (loop == true)
        {
            m_iCurIndex = 0;
            m_fTime = 0.0f;
            sprites[1].gameObject.active = false;
        }
    }
	
	public void Play()
	{
		if( 0 >= sprites.Length )
		{
			Debug.LogError("UISimpleSpriteAni::play()[ 0 >= sprites.Length ]");
			return;
		}
		
		m_isEnd = false;
		m_iCurIndex = 0;
		m_fTime = 0.0f;
		
		sprites[0].gameObject.active = true;
        sprites[1].gameObject.active = false;
	}

    public void Reset()
    {
        sprites[0].gameObject.active = true;

        for(int i=1 ; i < sprites.Length ; i++)
            sprites[i].gameObject.active = false;

        m_isEnd = false;
        m_iCurIndex = 0;
        m_fTime = 2.0f;
    }
	
	// Use this for initialization
	void Awake () 
	{
		foreach( SimpleSprite _sprite in sprites )	
		{
			_sprite.gameObject.active = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == isEnd )
			return;
		
		
		m_fTime += Time.deltaTime;
		
		if( speed <= m_fTime )
		{
			m_fTime = 0.0f;
			++m_iCurIndex;
			if( m_iCurIndex >= sprites.Length )
			{
                if (loop == true)
                {
                    m_iCurIndex = 0;

                    foreach (SimpleSprite sprite in sprites)
                        sprite.gameObject.active = false;

                    sprites[m_iCurIndex].gameObject.active = true;
                    return;
                }
                else
                {
                    m_isEnd = true;
                    Destroy(gameObject);
                    return;
                }
			}			
			sprites[m_iCurIndex-1].gameObject.active = false;
			sprites[m_iCurIndex].gameObject.active = true;
		}		
	}
}
