using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if USE_ITEM_RES

public class ItemRes 
{
    public enum eRES_GO
    {
        PARTS_W = 0,
		PARTS_M,
        DROP,
		ICON,
    };

    public enum eRES_TEX
    {
        //ICON = 0,
        DIFF_M= 0,
		DIFF_W,
        SPEC,
    };
    

    //---------------------------------------------------------------------
    /* Variable*/
    //---------------------------------------------------------------------    
    private Dictionary<eRES_GO, GameObject> m_GoList = new Dictionary<eRES_GO, GameObject>();
    private Dictionary<eRES_TEX, Texture> m_TexList = new Dictionary<eRES_TEX, Texture>();





    //---------------------------------------------------------------------
    /* functon */
    //---------------------------------------------------------------------


    // Get


    public Texture GetTexture(eRES_TEX eTexType)
    {
        if (false == m_TexList.ContainsKey(eTexType))
            return null;

        return m_TexList[eTexType];
    }


    public GameObject GetGameObject(eRES_GO eGoType)
    {
        if (false == m_GoList.ContainsKey(eGoType))
            return null;

        return m_GoList[eGoType];
    }
    

    // Load

    public bool LoadTexture(eRES_TEX eTexType, string strPath)
    {
        if (true == GetTexture(eTexType))
        {
            Debug.LogWarning("pre Loaded. tex file path  : " + strPath);
            return false;
        }

        Texture texTemp = null;
        if (false == LoadTexture(ref texTemp, strPath))
            return false;

        m_TexList[eTexType] = texTemp;

        return true;
    }


    public bool LoadGameObject(eRES_GO eGoType, string strPath)
    {
        if (null != GetGameObject(eGoType))
        {
            return true;
        }

        GameObject goTemp = null;
        if (false == LoadGameObject(ref goTemp, strPath))
            return false;
		
		if( true == m_GoList.ContainsKey(eGoType) )
		{
			m_GoList[eGoType] = goTemp;
		}
		else
		{
			m_GoList.Add( eGoType, goTemp);
		}

        return true;
    }



    protected bool LoadTexture(ref Texture tex, string strPath)
    {
        if (null != tex )
        {
            Debug.LogWarning("pre Loaded. tex file path  : " + strPath);
            return false;
        }

        tex = ResourceLoad.Loadtexture(strPath);

        if (null == tex)
        {
            Debug.LogError("Load failed. tex file path : " + strPath);
            return false;
        }

        return true;
    }



    protected bool LoadGameObject(ref GameObject go, string strPath)
    {
        if (null != go)
        {
            Debug.LogWarning("pre Loaded. GameObject path : " + strPath);
            return true;
        }

        go = ResourceLoad.LoadGameObject(strPath);
        if (null == go)
        {
            Debug.LogError("Load failed. GameObject path : " + strPath);
            return false;
        }

        return true;
    }      
}

#endif