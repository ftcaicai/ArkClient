using UnityEngine;
using System.Collections;

public class TooltipDlg : MonoBehaviour 
{
    private Item m_item = null;	
	public float backImgHeight;
	
	public Item getItem
	{
		get 
		{
            return m_item;
		}
	}


    public virtual bool SetItem(Item item)
    {
        if (null == item)
        {
            Debug.LogError("TooltipDlg::SetItem() [ null == iTem ]");
            return false;
        }

        m_item = item;	
		
		return true;
	}
	
	
	void Awake()
	{				
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
