using UnityEngine;
using System.Collections;

public class AsNpcData : MonoBehaviour {
	
	
	private string m_strNpcID;
    private string m_strMapID;
    private string m_strSpawnType;
	private string m_strGroupIndex;
	private string m_strLinkIndex;
    private ArrayList m_MovePathList = null;

    public ArrayList MovePathList
    {
        get
        {
            return m_MovePathList;
        }
    }
    
    public string strNpcID
    {
        get
        {
            return m_strNpcID;
        }       
    }
    public string strMapID
    {
        get
        {
            return m_strMapID;
        }       
    }
    public string strSpawnType
    {
        get
        {
            return m_strSpawnType;
        }      
    }
	
	public string strGroupIndex
	{
		get
		{
			return m_strGroupIndex;
		}
	}
	
	public string strLinkIndex
	{
		get
		{
			return m_strLinkIndex;
		}
	}


    public int GetNpcID()
    {
        return int.Parse(m_strNpcID);
    }

    public int GetMapID()
    {
        return int.Parse(m_strMapID);
    }

    public int GetSpawnType()
    {
        return int.Parse(m_strSpawnType);
    }
	
	public int GetGroupIndex()
	{
		return int.Parse(m_strGroupIndex);
	}
	
	public int GetLinkIndex()
	{
		return int.Parse(m_strLinkIndex);
	}

    public void SetNpcID(int idata)
    {
        m_strNpcID = idata.ToString();
    }

    public void SetMapID(int idata)
    {
        m_strMapID = idata.ToString();
    }

    public void SetSpawnType(int idata)
    {
        m_strSpawnType = idata.ToString();
    }
	
	public void SetGroupIndex(int idata)
	{
		m_strGroupIndex = idata.ToString();
	}
	
	public void SetLinkIndex(int idata)
	{
		m_strLinkIndex = idata.ToString();
	}

    public void AddMovePath(Vector3 vec3Data)
    {
        CreateMovePathList();

        m_MovePathList.Add(vec3Data);
    }

    public void RemoveMovePath()
    {
        if (null == m_MovePathList)
            return;
        m_MovePathList.RemoveAt(m_MovePathList.Count - 1);
    }

    public void SetMovePathList(ArrayList dataList)
    {
        m_MovePathList = dataList;
    }


    public int GetMovePathCount()
    {
        if (null == m_MovePathList)
            return 0;
        return m_MovePathList.Count;
    }

    public Vector3 GetMovePathData(int iIndex)
    {
        if (GetMovePathCount() <= iIndex)
        {
            Debug.LogWarning("AsNpcData::GetMovePathData() m_MovePathList overflow");
            return Vector3.zero;
        }

        return (Vector3)m_MovePathList[iIndex];
    }

    public void SetMovePathData(int iIndex, Vector3 vec3Data)
    {
        if (GetMovePathCount() <= iIndex)
        {
            Debug.LogWarning("AsNpcData::SetMovePathData() m_MovePathList overflow");
            return;
        }

        m_MovePathList[iIndex] = vec3Data;
    }


    protected void CreateMovePathList()
    {
        if (null != m_MovePathList)
            return;

        m_MovePathList = new ArrayList();
    }
   

	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
